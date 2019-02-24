using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgoJanitor.WebApi.Domain.Events;
using ArgoJanitor.WebApi.EventHandlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ArgoJanitor.WebApi.Infrastructure.Messaging
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly ILogger<EventDispatcher> _logger;
        private readonly DomainEventRegistry _eventRegistry;
        
        public EventDispatcher(
            ILogger<EventDispatcher> logger,
            DomainEventRegistry eventRegistry)
        {
            _logger = logger;
            _eventRegistry = eventRegistry;
        }

        public async Task Send(string generalDomainEventJson)
        {
            var generalDomainEventObj = JsonConvert.DeserializeObject<GeneralDomainEvent>(generalDomainEventJson);
            await SendAsync(generalDomainEventObj);
        }

        public async Task SendAsync(GeneralDomainEvent generalDomainEvent)
        {
            var eventType = _eventRegistry.GetInstanceTypeFor(generalDomainEvent.Type);
            dynamic domainEvent = Activator.CreateInstance(eventType, generalDomainEvent);
            dynamic handlersList = _eventRegistry.GetEventHandlersFor(domainEvent);
            
            foreach (var handler in handlersList)
            {
                await handler.HandleAsync(domainEvent);
            }
        }
    }
}