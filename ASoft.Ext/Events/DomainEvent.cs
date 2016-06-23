using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Events
{
    public class DomainEvent : SequenceMessage, IDomainEvent
    {
       
       // public object AggregateRootKey { get; set; }

        //public string AggregateRootType { get; set; }

        public string EventName { get; set; } 
 
    }
}
