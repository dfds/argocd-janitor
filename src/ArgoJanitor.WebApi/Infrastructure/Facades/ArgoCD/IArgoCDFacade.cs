using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD
{
    public interface IArgoCDFacade
    {
        Task<CreateProjectResponse> CreateProject(string projectName);
        Task<GetProjectResponse> GetProject(string projectName);

    }


    public class CreateProjectRequest
    {
        public AppProject Project { get; set; } = new AppProject();
    }

    public class GetProjectResponse
    {
        public AppProject Project { get; set; } = new AppProject();
    }


    public class AppProject
    {
        public V1ObjectMeta Metadata { get; set; } = new V1ObjectMeta();
        public AppProjectSpec Spec { get; set; } = new AppProjectSpec();
    }

    public class V1ObjectMeta
    {
        public string Name { get; set; }
    }

    public class AppProjectSpec
    {
        public List<string> SourceRepos { get; set; } = new List<string>();
        public List<string> Destinations { get; set; } = new List<string>();
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> ClusterResourceWhitelist { get; set; } = new List<string>();
        public List<string> NamespaceResourceBlacklist { get; set; } = new List<string>();
    }
}