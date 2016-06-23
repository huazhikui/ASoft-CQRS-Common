using ASoft.Domain.Exception;
using ASoft.Events; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Domain
{
    public abstract class AggregateRoot : IAggregateRoot,IPurgeable
    {

        private static readonly IList<IDomainEvent> _emptyEvents = new List<IDomainEvent>();

        private Queue<IDomainEvent> _uncommittedEvents;

        protected   readonly log4net.ILog log;
        public string Id { get; set; }
        public int Version { get; protected set; }

        public IEnumerable<IDomainEvent> UncommittedEvents => _uncommittedEvents; 

        protected AggregateRoot()
        {
            _uncommittedEvents = new Queue<IDomainEvent>();
             
            log = log4net.LogManager.GetLogger(this.GetType().FullName);
            
        }

        protected AggregateRoot(string id) : this()
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            log.Debug("AggregateRoot id :" + id);
            this.Id = id;
        }

      
        void IPurgeable.Purge()
        {
            if (this._uncommittedEvents.Count > 0)
            {
                this._uncommittedEvents.Clear();
            }
        }

        protected void ApplyEvent<TEvent>(TEvent evnt) where TEvent : IDomainEvent
        {
            var eventHandlerMethods = from m in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                      let parameters = m.GetParameters()
                                      where //m.IsDefined(typeof(InlineEventHandlerAttribute)) &&
                                      m.ReturnType == typeof(void) &&
                                      parameters.Length == 1 &&
                                      parameters[0].ParameterType == evnt.GetType()
                                      select m;

            evnt.AggregateRootTypeName = this.GetType().FullName;

            foreach (var eventHandlerMethod in eventHandlerMethods)
            {
                eventHandlerMethod.Invoke(this, new object[] { evnt });
            }
            log.Debug("ApplyEvent: AggregateRoot id :" + Id);
            evnt.AggregateRootId = Id;
            evnt.Version = this.Version + 1;

            this._uncommittedEvents.Enqueue(evnt);
        }

        public void Replay(IEnumerable<IDomainEvent> events)
        {
            throw new NotImplementedException();
        }
    }
}
