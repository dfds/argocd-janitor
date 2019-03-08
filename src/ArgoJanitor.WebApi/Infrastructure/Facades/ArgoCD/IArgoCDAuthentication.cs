using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{
    public class CreateSessionResponse
    {
        public string Token { get; set; }
    }
    public interface IArgoCDAuthentication
    {
        Task<CreateSessionResponse> GetSessionToken(string username, string password);
    }
}