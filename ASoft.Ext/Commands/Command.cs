using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Commands
{
    public class Command : Message, ICommand
    {
        /// <summary>Represents the associated aggregate root id.
        /// </summary>
        public string AggregateRootId { get; set; }

        /// <summary>Default constructor.
        /// </summary>
        public Command() : base() { }
        /// <summary>Parameterized constructor.
        /// </summary>
        /// <param name="aggregateRootId"></param>
        public Command(string aggregateRootId)
        {
            if (aggregateRootId == null)
            {
                throw new ArgumentNullException("aggregateRootId");
            }
            this.Id = aggregateRootId;
            AggregateRootId = aggregateRootId;
        }
 
        public int ExpectedVersion
        {
            set;get;
        }
    }
}
