using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectNameWorker.Config;

namespace ProjectNameWorker.Workers
{
    public class ExampleWorker : BackgroundService
    {
        private readonly ExampleWorkerConfig _exampleWorkerConfig;
        private readonly ILogger<ExampleWorker> _logger;
        
        public ExampleWorker(ILogger<ExampleWorker> logger, IOptions<ExampleWorkerConfig> workerConfig)
        {
            _exampleWorkerConfig = workerConfig.Value;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Example worker running. Example config value: " + _exampleWorkerConfig.ExampleConfigValue);
                // Do things
                await Task.Delay(5000);
            }
        }
    }
}