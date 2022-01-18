using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectNameApi.Config;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ProjectNameApi.Services
{
    public class ExampleService
    {
        private readonly DbGateway _dbGateway;
        private readonly SecretConfig _secretConfig;
        private readonly ILogger<ExampleService> _logger;
        
        public ExampleService(DbGateway dbGateway, ILogger<ExampleService> logger, IOptions<SecretConfig> secretConfig)
        {
            _dbGateway = dbGateway;
            _secretConfig = secretConfig.Value;
            //_dbGateway.AddConnection(_secretConfig.ConnectionString);
            _logger = logger;
        }

        public string ExampleMethod()
        {
            return _secretConfig.ExampleSecret;
        }
    }
}