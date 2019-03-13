using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ArgoJanitor.WebApi.Infrastructure.Facades.SSM;
using ArgoJanitor.WebApi.Infrastructure.Serialization;
using Microsoft.Extensions.Logging;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{


    public class ArgoCDFacade : IArgoCDFacade
    {
        private readonly HttpClient _httpClient;
        
        private readonly JsonSerializer _jsonSerializer;
        private readonly ILogger<ArgoCDFacade> _logger;


        public async Task<CreateProjectResponse> CreateProject(string projectName)
        {
            var normalizedProjectName = ArgoCDEntityNameNormalizer.Normalize(projectName);
            var projectRequest = new CreateProjectRequest();
            projectRequest.Project.Metadata.Name = normalizedProjectName;
            projectRequest.Project.Spec.SourceRepos.Add("*");
            projectRequest.Project.Spec.Destinations.Add(new V1alpha1ApplicationDestination{Namespace = normalizedProjectName, Server = "*"});
            var payload = _jsonSerializer.GetPayload(projectRequest);
            
            var response = await _httpClient.PostAsync("/api/v1/projects", payload);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning($"{normalizedProjectName} already exist as a project in Argo.");
                return new CreateProjectResponse                         
                {                                                        
                    Project = (await GetProject(normalizedProjectName))?.Project   
                };                                                       
            }
             
            // Other errors should throw exception.
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var project = _jsonSerializer.Deserialize<AppProject>(content);

            return new CreateProjectResponse{Project = project};
        }

        public async Task<GetProjectResponse> GetProject(string projectName)
        {
            var normalizedProjectName = ArgoCDEntityNameNormalizer.Normalize(projectName);
            var response = await _httpClient.GetAsync("/api/v1/projects/{normalizedProjectName}");
            
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var project = _jsonSerializer.Deserialize<AppProject>(content);
            
            return new GetProjectResponse{Project = project};
        }


        public ArgoCDFacade(HttpClient httpClient, JsonSerializer jsonSerializer, ILogger<ArgoCDFacade> logger)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _logger = logger;
        }
    }
}
