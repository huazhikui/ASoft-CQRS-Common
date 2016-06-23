using ASoft.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
    /// <summary>Represents an abstract message.
    /// </summary>
    [Serializable]
    public abstract class Message : IMessage
    {
        /// <summary>Represents the identifier of the message.
        /// </summary>
        public string Id { get; set; }
        /// <summary>Represents the timestamp of the message.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
        /// <summary>Represents the sequence of the message which is belongs to the message stream.
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>Default constructor.
        /// </summary>
        public Message()
        {
            Id = ObjectId.GenerateNewStringId();
            Timestamp = DateTime.Now;
            Sequence = 1;
        }

        /// <summary>Returns null by default.
        /// </summary>
        /// <returns></returns>
        public virtual string GetRoutingKey()
        {
            return null;
        }
        /// <summary>Returns the full type name of the current message.
        /// </summary>
        /// <returns></returns>
        public string GetTypeName()
        {
            return this.GetType().FullName;
        }
    }
}