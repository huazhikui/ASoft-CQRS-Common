﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
    public class SequenceMessage : Message, ISequenceMessage
    {
        private string _aggregateRootId;

        /// <summary>Represents the aggregate root id.
        /// </summary>
        public string AggregateRootId
        {
            get { return _aggregateRootId; }
            set
            {
                _aggregateRootId = value;
                AggregateRootStringId = value.ToString();
            }
        }
        /// <summary>Represents the aggregate root string id.
        /// </summary>
        public string AggregateRootStringId { get; set; }
        /// <summary>Represents the aggregte root type name.
        /// </summary>
        public string AggregateRootTypeName { get; set; }
        /// <summary>Represents the version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>Returns the aggregate root id by default.
        /// </summary>
        /// <returns></returns>
        public override string GetRoutingKey()
        {
            return AggregateRootStringId;
        }
    }
}
