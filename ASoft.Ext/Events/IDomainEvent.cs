using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Events
{
    public interface IDomainEvent : ISequenceMessage
    {
        string AggregateRootId { get; set; }

        //string AggregateRootType { get; set; }


    }
}
