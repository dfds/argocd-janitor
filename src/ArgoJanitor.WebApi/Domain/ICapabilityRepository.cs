using System;
using System.Threading.Tasks;

namespace ArgoJanitor.WebApi.Domain
{
    public interface ICapabilityRepository
    {
        Task<Capability> Get(Guid id);
        Task Add(Capability capability);
    }
}