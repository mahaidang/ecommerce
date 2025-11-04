using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using OrderingService.Infrastructure.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.RegularExpressions;

namespace Ordering.Infrastructure.Outbox;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly ILogger<OutboxPublisher> _log;
    private readonly IDbContextFactory<OrderingDbContext> _dbFactory;  // ⬅️ dùng factory
    private readonly IConnection _conn;
    private readonly IConfiguration _cfg;

    private string Exchange => _cfg["RabbitMq:Exchange"] ?? "order.events";
    private int BatchSize => int.TryParse(_cfg["RabbitMq:PublishBatchSize"], out var n) ? n : 100;
    private TimeSpan Poll => TimeSpan.FromSeconds(int.TryParse(_cfg["RabbitMq:PollIntervalSeconds"], out var s) ? s : 2);

    public OutboxPublisher(
        ILogger<OutboxPublisher> log,
        IDbContextFactory<OrderingDbContext> dbFactory,   // ⬅️ inject factory
        IConnection conn,
        IConfiguration cfg)
    { _log = log; _dbFactory = dbFactory; _conn = conn; _cfg = cfg; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var ch = _conn.CreateModel())
            ch.ExchangeDeclare(Exchange, ExchangeType.Topic, durable: true, autoDelete: false);

        _log.LogInformation("OutboxPublisher started ({Exchange})", Exchange);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var hasWork = await PublishBatchAsync(stoppingToken);
                if (!hasWork) await Task.Delay(Poll, stoppingToken);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                _log.LogError(ex, "Outbox loop error");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task<bool> PublishBatchAsync(CancellationToken ct)
    {
        // tạo DbContext cho mỗi lượt xử lý
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var items = await db.Set<OutboxMessage>()
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(BatchSize)
            .ToListAsync(ct);

        if (items.Count == 0) return false;

        using var ch = _conn.CreateModel();
        var props = ch.CreateBasicProperties();
        props.ContentType = "application/json";
        props.DeliveryMode = 2;

        foreach (var m in items)
        {
            try
            {
                var rk = ToTopic(m.EventType);
                var body = Encoding.UTF8.GetBytes(m.Payload ?? "{}");
                ch.BasicPublish(exchange: Exchange, routingKey: rk, basicProperties: props, body: body);

                m.ProcessedAtUtc = DateTime.UtcNow;
                m.Error = null;
            }
            catch (Exception ex)
            {
                var s = ex.Message; m.Error = s.Length > 480 ? s[..480] : s;
            }
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    private static string ToTopic(string? eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType)) return "order.event";
        var s = Regex.Replace(eventType.Trim(), "([a-z0-9])([A-Z])", "$1.$2");
        s = Regex.Replace(s, @"\s+", ".");
        return s.Replace("_", ".").ToLowerInvariant().Trim('.');
    }
}
