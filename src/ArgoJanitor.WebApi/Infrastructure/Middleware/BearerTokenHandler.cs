using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD;
using ArgoJanitor.WebApi.Infrastructure.Facades.SSM;
using Microsoft.Extensions.Configuration;

namespace ArgoJanitor.WebApi.Infrastructure.Middleware
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IArgoCDAuthentication _argoCdAuthentication;
        private readonly ISSMFacade _ssmFacade;
        private readonly IConfiguration _configuration;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var username = _configuration["ARGOJANITOR_ARGOCD_DEFAULT_USER"];
            var domain = _configuration["ARGOJANITOR_API_BASE_URL"];  
            var password = await _ssmFacade.GetSecretFromKey(username);
            var sessionToken = await _argoCdAuthentication.GetSessionToken(username, password);
            
            request.Headers.Add(HttpRequestHeader.Cookie.ToString(), $"argocd.token={sessionToken.Token}; path=/; domain=.{domain};");
            return await base.SendAsync(request, cancellationToken);
        }

        public BearerTokenHandler(IArgoCDAuthentication argoCdAuthentication, ISSMFacade ssmFacade, IConfiguration configuration)
        {
            _argoCdAuthentication = argoCdAuthentication;
            _ssmFacade = ssmFacade;
            _configuration = configuration;
        }
    }
}