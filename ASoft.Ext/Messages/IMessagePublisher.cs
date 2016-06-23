using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
    public interface IMessagePublisher : IDisposable
    {
        /// <summary>
        /// Publishes the specified message to the message queue.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="message">The message to be published.</param>
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
    }
}
