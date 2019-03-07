using System;
using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{
    public class ArgoCDFacade : IArgoCDFacade
    {
        public ArgoCDFacade()
        {
        }

        public Task<SessionResponse> GetSessionToken(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<CreateProjectResponse> CreateProject(string projectName)
        {
            throw new NotImplementedException();
        }
    }
}
