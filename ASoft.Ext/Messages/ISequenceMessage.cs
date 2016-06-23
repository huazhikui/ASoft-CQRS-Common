using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
    public interface ISequenceMessage : IMessage
    {
        /// <summary>Represents the aggregate root string id of the sequence message.
        /// </summary>
        string AggregateRootStringId { get; set; }
        /// <summary>Represents the aggregate root type name of the sequence message.
        /// </summary>
        string AggregateRootTypeName { get; set; }
        /// <summary>Represents the main version of the sequence message.
        /// </summary>
        int Version { get; set; }
    }
}
