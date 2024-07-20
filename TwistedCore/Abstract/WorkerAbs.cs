using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwistedCore.Abstract;

public abstract class WorkerAbs : BackgroundService
{
    private readonly ILogger<WorkerAbs> _logger;
    protected bool _isRunning;
    protected string _name;

    public WorkerAbs(ILogger<WorkerAbs> logger)
    {
        _logger = logger;
        _name = "WorkerAbs";
    }

    protected abstract void OnStart();

    protected abstract void OnStop();

    protected abstract void OnError(Exception exception);

    protected abstract Task DoWork(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            OnStart();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("{name} running at: {time}", _name, DateTimeOffset.Now);

                await DoWork(stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }

            OnStop();
        }
        catch (Exception e)
        {
            OnError(e);
        }
    }
}