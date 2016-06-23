using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Events
{
    public interface IEventStore
    {
        void Save<T>(IEnumerable<IDomainEvent> events);

        IEnumerable<IDomainEvent> Get<T>(string aggregateId, int fromVersion);
    }
}
