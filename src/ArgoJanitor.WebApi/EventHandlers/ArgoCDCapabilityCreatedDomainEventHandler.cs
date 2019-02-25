using System;
using System.Threading.Tasks;
using ArgoJanitor.WebApi.Domain;
using ArgoJanitor.WebApi.Domain.Events;
using ArgoJanitor.WebApi.Infrastructure.Facades.ArgoCD;
using Microsoft.Extensions.Logging;

namespace ArgoJanitor.WebApi.EventHandlers
{
    public class ArgoCDCapabilityCreatedDomainEventHandler : IEventHandler<CapabilityCreatedDomainEvent>
    {
        private readonly ILogger<ArgoCDCapabilityCreatedDomainEventHandler> _logger;
        private readonly IArgoCDFacade _argocdFacade;
        private readonly ICapabilityRepository _capabilityRepository;

        public ArgoCDCapabilityCreatedDomainEventHandler(
            ILogger<ArgoCDCapabilityCreatedDomainEventHandler> logger,
            IArgoCDFacade argoCDFacade,
            ICapabilityRepository capabilityRepository)
        {
            _logger = logger;
            _argocdFacade = argoCDFacade;
            _capabilityRepository = capabilityRepository;
        }

        public async Task HandleAsync(CapabilityCreatedDomainEvent domainEvent)
        {
            _logger.LogInformation($"Creating capability with name: {domainEvent.Data.CapabilityName}");

            var capability = Capability.Create(
                id: domainEvent.Data.CapabilityId,
                name: domainEvent.Data.CapabilityName);
            await _capabilityRepository.Add(capability);

        }
    }
}