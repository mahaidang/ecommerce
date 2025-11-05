//// OutboxPublisher: Background service dùng để lấy các message chưa xử lý từ bảng OutboxMessage
//// và publish lên RabbitMQ theo mô hình Outbox Pattern (đảm bảo tính nhất quán giữa DB và message broker).

//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Ordering.Domain.Entities;
//using Ordering.Infrastructure.Models;
//using RabbitMQ.Client;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace Ordering.Infrastructure.Outbox;

//// Service chạy nền, định kỳ lấy các message chưa gửi từ DB và publish lên RabbitMQ.
//public sealed class OutboxPublisher : BackgroundService
//{
//    // Logger để ghi log quá trình publish.
//    private readonly ILogger<OutboxPublisher> _log;
//    // Factory tạo DbContext cho mỗi lần xử lý (đảm bảo thread-safe).
//    private readonly IDbContextFactory<OrderingDbContext> _dbFactory;
//    // Kết nối RabbitMQ.
//    private readonly IConnection _conn;
//    // Đọc cấu hình từ appsettings.
//    private readonly IConfiguration _cfg;

//    // Tên exchange RabbitMQ (lấy từ cấu hình).
//    private string Exchange => _cfg["RabbitMq:Exchange"] ?? "order.events";
//    // Số lượng message xử lý mỗi batch.
//    private int BatchSize => int.TryParse(_cfg["RabbitMq:PublishBatchSize"], out var n) ? n : 100;
//    // Thời gian chờ giữa các lần polling.
//    private TimeSpan Poll => TimeSpan.FromSeconds(int.TryParse(_cfg["RabbitMq:PollIntervalSeconds"], out var s) ? s : 2);

//    // Constructor: inject các dependency.
//    public OutboxPublisher(
//        ILogger<OutboxPublisher> log,
//        IDbContextFactory<OrderingDbContext> dbFactory,
//        IConnection conn,
//        IConfiguration cfg)
//    { _log = log; _dbFactory = dbFactory; _conn = conn; _cfg = cfg; }

//    // Hàm chính chạy vòng lặp background.
//    protected override async Task ExecuteAsync(CancellationToken ct)
//    {
//        // Khởi tạo exchange nếu chưa có.
//        using (var ch = _conn.CreateModel())
//            ch.ExchangeDeclare(Exchange, ExchangeType.Topic, durable: true, autoDelete: false);

//        _log.LogInformation("OutboxPublisher started ({Exchange})", Exchange);

//        // Vòng lặp chính: liên tục kiểm tra và publish các message mới.
//        while (!ct.IsCancellationRequested)
//        {
//            try
//            {
//                var hasWork = await PublishBatchAsync(ct);
//                // Nếu không có message mới, chờ một khoảng thời gian.
//                if (!hasWork) await Task.Delay(Poll, ct);
//            }
//            catch (TaskCanceledException) { }
//            catch (Exception ex)
//            {
//                _log.LogError(ex, "Outbox loop error");
//                await Task.Delay(TimeSpan.FromSeconds(5), ct);
//            }
//        }
//    }

//    // Xử lý một batch message: lấy các message chưa gửi, publish lên RabbitMQ, cập nhật trạng thái.
//    private async Task<bool> PublishBatchAsync(CancellationToken ct)
//    {
//        // Tạo DbContext mới cho mỗi lần xử lý.
//        await using var db = await _dbFactory.CreateDbContextAsync(ct);

//        // Lấy các message chưa xử lý, theo thứ tự thời gian.
//        var items = await db.Set<OutboxMessage>()
//            .Where(x => x.ProcessedAtUtc == null)
//            .OrderBy(x => x.OccurredAtUtc)
//            .Take(BatchSize)
//            .ToListAsync(ct);

//        if (items.Count == 0) return false;

//        // Tạo channel RabbitMQ.
//        using var ch = _conn.CreateModel();
//        var props = ch.CreateBasicProperties();
//        props.ContentType = "application/json";
//        props.DeliveryMode = 2; // persistent

//        // Publish từng message lên RabbitMQ.
//        foreach (var m in items)
//        {
//            try
//            {
//                var rk = ToTopic(m.EventType); // Chuyển eventType thành routing key.
//                var body = Encoding.UTF8.GetBytes(m.Payload ?? "{}");
//                ch.BasicPublish(exchange: Exchange, routingKey: rk, basicProperties: props, body: body);

//                m.ProcessedAtUtc = DateTime.UtcNow; // Đánh dấu đã xử lý.
//                m.Error = null;
//            }
//            catch (Exception ex)
//            {
//                // Nếu lỗi, ghi lại thông báo lỗi (tối đa 480 ký tự).
//                var s = ex.Message; m.Error = s.Length > 480 ? s[..480] : s;
//            }
//        }

//        // Lưu thay đổi trạng thái message vào DB.
//        await db.SaveChangesAsync(ct);
//        return true;
//    }

//    // Chuyển eventType sang routing key phù hợp với RabbitMQ topic.
//    private static string ToTopic(string? eventType)
//    {
//        if (string.IsNullOrWhiteSpace(eventType)) return "order.event";
//        var s = Regex.Replace(eventType.Trim(), "([a-z0-9])([A-Z])", "$1.$2");
//        s = Regex.Replace(s, @"\s+", ".");
//        return s.Replace("_", ".").ToLowerInvariant().Trim('.');
//    }
//}
