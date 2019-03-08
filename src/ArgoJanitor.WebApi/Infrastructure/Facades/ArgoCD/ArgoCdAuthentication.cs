using System.Net.Http;
using System.Threading.Tasks;
using ArgoJanitor.WebApi.Infrastructure.Serialization;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{
    public class ArgoCdAuthentication : IArgoCDAuthentication
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _jsonSerializer;

        public async Task<CreateSessionResponse> GetSessionToken(string username, string password)
        {
            var payload = _jsonSerializer.GetPayload(new { Username = username, Password = password });
            var response = await _httpClient.PostAsync("/api/v1/session", payload);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenReponse = _jsonSerializer.Deserialize<CreateSessionResponse>(content);

            return tokenReponse;
        }

        public ArgoCdAuthentication(HttpClient httpClient, JsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }
    }
}