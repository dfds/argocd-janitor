using System;

namespace ArgoJanitor.WebApi.Infrastructure.Messaging
{
    public class MessagingException : Exception
    {
        public MessagingException(string message) : base(message)
        {
        }
    }
}