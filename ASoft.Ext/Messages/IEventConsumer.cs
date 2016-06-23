using ASoft.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
    public interface IEventConsumer : IMessageConsumer
    {
        /// <summary>
        /// Gets a list of domain event handlers that will handle and process the
        /// domain events.
        /// </summary>
        /// <value>
        /// The event handlers.
        /// </value>
        IEnumerable<IEventHandler> EventHandlers { get; }
    }
}
