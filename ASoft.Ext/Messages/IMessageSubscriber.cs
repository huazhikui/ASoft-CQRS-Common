using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
 
    public interface IMessageSubscriber : IDisposable
    {
        /// <summary>
        /// Subscribe to the message bus.
        /// </summary>
        void Subscribe();

        /// <summary>
        /// Represents the event that occurs when there is any incoming message.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
