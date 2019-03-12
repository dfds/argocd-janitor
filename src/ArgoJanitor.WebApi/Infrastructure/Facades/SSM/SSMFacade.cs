using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.SSM
{
    public class SSMFacade : ISSMFacade
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private readonly ILogger<SSMFacade> _logger;

        public async Task<string> GetSecretFromKey(string key)
        {
            _logger.LogInformation($"Getting value of {key} from parameter store");
            var parameterKey = _configuration["ARGOJANITOR_SSM_ADMIN_KEY"];
            var value = await _ssmClient.GetParameterAsync(new GetParameterRequest
            {
                Name = parameterKey,
                WithDecryption = true
            });
    
            _logger.LogInformation($"Found value of {key} from parameter store");
            return value.Parameter.Value;
        }

        public SSMFacade(IConfiguration configuration, IAmazonSimpleSystemsManagement ssmClient, ILogger<SSMFacade> logger)
        {
            _configuration = configuration;
            _ssmClient = ssmClient;
            _logger = logger;
        }
    }
}