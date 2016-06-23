using ASoft.Domain;
using ASoft.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using ASoft.Messages;

namespace ASoft.Cache
{
    public class CacheRepository : DomainRepository
    {
        private readonly ConcurrentDictionary<string, object> aggregates = new ConcurrentDictionary<string, object>();
        public CacheRepository(IMessagePublisher messagePublisher) : base(messagePublisher)
        {
        }

        protected override Task<TAggregateRoot> GetAggregateAsync<TAggregateRoot>(string aggregateRootKey)
        {
            //var query = from obj in this.aggregates
            //            let ar = obj as TAggregateRoot
            //            where ar != null &&
            //            ar.Id.Equals(aggregateRootKey)
            //            select obj;
            //return Task.FromResult(query.FirstOrDefault() as TAggregateRoot);
            if (aggregates.Keys.Contains(aggregateRootKey))
            {
                return Task.FromResult(aggregates[aggregateRootKey] as TAggregateRoot);
            }
            return null;
        }
         

        protected override Task SaveAggregateAsync<TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            return Task.Run(() => this.aggregates.TryAdd(aggregateRoot.Id,aggregateRoot));
        }

    }
}
