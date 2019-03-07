using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.SSM
{
    public interface ISSMFacade
    {
        Task<string> GetSecretFromKey(string key);
    }
}