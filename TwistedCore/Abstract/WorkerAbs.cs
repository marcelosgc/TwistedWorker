using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwistedCore.Abstract;

public abstract class WorkerAbs : BackgroundService
{
    private readonly ILogger<WorkerAbs> _logger;
    protected string _defaultCron;
    protected bool _isRunning;
    protected string _name;
    protected DateTime _nextRun;

    public WorkerAbs(ILogger<WorkerAbs> logger)
    {
        _logger = logger;
        _name = "WorkerAbs";
        _defaultCron = " */1 * * * *";
    }

    protected abstract void OnStart();

    protected abstract void OnStop();

    protected abstract void OnError(Exception exception);

    protected abstract Task DoWork(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            SetNextRun();
            OnStart();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (IsItTimeToExecute())
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("{name} running at: {time}", _name, DateTimeOffset.Now);

                    await DoWork(stoppingToken);
                    SetNextRun();
                }

                await Task.Delay(4000, stoppingToken);
            }

            OnStop();
        }
        catch (Exception e)
        {
            OnError(e);
        }
    }

    private bool IsItTimeToExecute()
    {
        var utcNow = DateTime.UtcNow;
        return utcNow >= _nextRun;
    }

    private void SetNextRun()
    {
        var utcNow = DateTime.UtcNow;
        var cronExpression = CronExpression.Parse(_defaultCron);
        _nextRun = cronExpression.GetNextOccurrence(utcNow).GetValueOrDefault();
        _logger.LogInformation($"The CRON is {_defaultCron} and next run will be {_nextRun} ... ");
    }
}