using TwistedCore.Abstract;

namespace TwistedWorker;

public class Worker : WorkerAbs
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger) : base(logger)
    {
        _logger = logger;
        _name = "Worker";
    }

    protected override void OnStart()
    {
        _isRunning = true;
    }

    protected override void OnStop()
    {
        _isRunning = false;
    }

    protected override void OnError(Exception exception)
    {
        Console.WriteLine(exception.Message);
    }

    protected override async Task DoWork(CancellationToken stoppingToken)
    {
        for (var i = 0; i < 30; i++) Console.WriteLine($"Tchau França {i}");
    }
}