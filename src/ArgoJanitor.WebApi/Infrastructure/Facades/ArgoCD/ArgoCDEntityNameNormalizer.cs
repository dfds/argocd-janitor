namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{
    public class ArgoCDEntityNameNormalizer
    {
        public static string Normalize(string entityName)
        {
            return entityName.ToLowerInvariant();
        }
    }
}