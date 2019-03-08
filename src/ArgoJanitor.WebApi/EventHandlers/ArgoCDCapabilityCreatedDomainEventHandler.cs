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

        public ArgoCDCapabilityCreatedDomainEventHandler(
            ILogger<ArgoCDCapabilityCreatedDomainEventHandler> logger,
            IArgoCDFacade argoCDFacade)
        {
            _logger = logger;
            _argocdFacade = argoCDFacade;
        }

        public async Task HandleAsync(CapabilityCreatedDomainEvent domainEvent)
        {
            _logger.LogInformation($"Creating capability with name: {domainEvent.Data.CapabilityName}");

            var capability = Capability.Create(
                id: domainEvent.Data.CapabilityId,
                name: domainEvent.Data.CapabilityName);

            await _argocdFacade.CreateProject(capability.Name);

        }
    }
}