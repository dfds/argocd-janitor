using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.SSM
{
    public class SSMFacade : ISSMFacade
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonSimpleSystemsManagement _ssmClient;

        public async Task<string> GetSecretFromKey(string key)
        {
            var parameterKey = _configuration["ARGOJANITOR_SSM_ADMIN_KEY"];
            var value = await _ssmClient.GetParameterAsync(new GetParameterRequest
            {
                Name = parameterKey,
                WithDecryption = true
            });

            return value.Parameter.Value;
        }

        public SSMFacade(IConfiguration configuration, IAmazonSimpleSystemsManagement ssmClient)
        {
            _configuration = configuration;
            _ssmClient = ssmClient;
        }
    }
}