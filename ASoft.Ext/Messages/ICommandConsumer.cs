using ASoft.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
   
    public interface ICommandConsumer: IMessageConsumer
    {
        /// <summary>
        /// Gets a list of command handlers that will handle the message.
        /// </summary>
        /// <value>
        /// A list of command handlers.
        /// </value>
        IEnumerable<ICommandHandler> CommandHandlers { get; }
    }
}
