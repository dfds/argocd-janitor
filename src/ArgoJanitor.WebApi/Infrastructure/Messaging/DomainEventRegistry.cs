using System;
using System.Collections.Generic;
using System.Linq;
using ArgoJanitor.WebApi.Domain.Events;
using ArgoJanitor.WebApi.EventHandlers;

namespace ArgoJanitor.WebApi.Infrastructure.Messaging
{
    public class DomainEventRegistry
    {
        private readonly List<DomainEventRegistration> _registrations = new List<DomainEventRegistration>();
        private readonly Dictionary<Type, List<object>> _eventHandlers = new Dictionary<Type, List<object>>();

        public DomainEventRegistry Register<TEvent>(string eventTypeName, string topicName, IEventHandler<TEvent> eventHandler)
        {
            _registrations.Add(new DomainEventRegistration
            {
                EventType = eventTypeName,
                EventInstanceType = typeof(TEvent),
                Topic = topicName
            });

            RegisterEventHandler(eventHandler);

            return this;
        }

        private void RegisterEventHandler<TEvent>(IEventHandler<TEvent> eventHandler)
        {
            if (!_eventHandlers.ContainsKey(typeof(TEvent)))
            {
                _eventHandlers.Add(typeof(TEvent), new List<object>());
            }

            List<object> handlersList = _eventHandlers[typeof(TEvent)];
            handlersList.Add(eventHandler);
        }

        public string GetTopicFor(string eventType)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventType == eventType);

            if (registration != null)
            {
                return registration.Topic;
            }

            return null;
        }

        public IEnumerable<string> GetAllTopics()
        {
            var topics = _registrations.Select(x => x.Topic).Distinct();           

            return topics;
        }

        public Type GetInstanceTypeFor(string eventType)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventType == eventType);

            if (registration == null)
            {
                throw new MessagingException($"Error! Could not determine \"event instance type\" due to no registration was found for type {eventType}!");
            }

            return registration.EventInstanceType;
        }

        public List<object> GetEventHandlersFor<TEvent>(TEvent domainEvent)
        {
            var registration = _eventHandlers.SingleOrDefault(eventhandler => eventhandler.Key == typeof(TEvent));

            if (registration.Equals(default(KeyValuePair<Type, List<object>>)))
            {
                throw new MessagingException($"Error! Could not determine \"event handlers\" due to no registration was found for type {domainEvent.GetType().FullName}!");
            }

            return registration.Value;
        }

        public class DomainEventRegistration
        {
            public string EventType { get; set; }
            public Type EventInstanceType { get; set; }
            public string Topic { get; set; }
        }
    }

}