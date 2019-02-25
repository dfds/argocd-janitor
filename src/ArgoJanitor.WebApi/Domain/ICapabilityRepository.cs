using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.Domain
{
    public interface ICapabilityRepository
    {
        Task<Capability> Get(Guid id);
        Task Add(Capability capability);
        Task<List<Capability>> GetCapabilitiesWithoutAzureAdObjectId();
    }
}