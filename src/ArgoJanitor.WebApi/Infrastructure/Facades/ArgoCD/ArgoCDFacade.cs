using System;
using System.Net.Http;
using System.Threading.Tasks;
using ArgoJanitor.WebApi.Infrastructure.Facades.SSM;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{
    public class ArgoCDFacade : IArgoCDFacade
    {
        private readonly HttpClient _httpClient;

        public Task<SessionResponse> GetSessionToken(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<CreateProjectResponse> CreateProject(string projectName)
        {
            throw new NotImplementedException();
        }

        public ArgoCDFacade(HttpClient httpClient, ISSMFacade fa
        )
        {
            _httpClient = httpClient;
        }
    }
}
