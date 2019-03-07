using System;
using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{
    public interface IArgoCDFacade
    {
        Task<SessionResponse> GetSessionToken(string username, string password);
        Task<CreateProjectResponse> CreateProject(string projectName);
    }
    
    public class SessionResponse
    {
        public string Token { get; set; }
    }
}
