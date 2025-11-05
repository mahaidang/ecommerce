//using RabbitMQ.Client;

//namespace OrchestratorService.Worker.Services;

//public sealed class RabbitConnection : IDisposable
//{
//    private readonly IConnection _conn;
//    public RabbitConnection(IConnection conn) => _conn = conn;
//    public IModel CreateChannel(ushort? prefetch = null)
//    {
//        var ch = _conn.CreateModel();
//        if (prefetch.HasValue) ch.BasicQos(0, prefetch.Value, global: false);
//        return ch;
//    }
//    public void Dispose() => _conn?.Dispose();
//}
