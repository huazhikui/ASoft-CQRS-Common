using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Events
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IEventPublisher _publisher;
        private readonly Dictionary<string, List<IDomainEvent>> _inMemoryDb = new Dictionary<string, List<IDomainEvent>>();

        public InMemoryEventStore(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Save<T>(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                List<IDomainEvent> list;
                _inMemoryDb.TryGetValue(@event.Id, out list);
                if (list == null)
                {
                    list = new List<IDomainEvent>();
                    _inMemoryDb.Add(@event.Id, list);
                }
                list.Add(@event);
                _publisher.Publish(@event);
            }
        }

        public IEnumerable<IDomainEvent> Get<T>(string aggregateId, int fromVersion)
        {
            List<IDomainEvent> events;
            _inMemoryDb.TryGetValue(aggregateId, out events);
            return events?.Where(x => x.Version > fromVersion) ?? new List<IDomainEvent>();
        }
    }
}
