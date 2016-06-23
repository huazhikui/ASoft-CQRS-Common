using ASoft.Domain.Exception;
using ASoft.Events;
using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Domain
{
    public abstract class DomainRepository : IDomainRepository
    {
        private readonly IMessagePublisher messagePublisher;

        public DomainRepository(IMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        public async Task<TAggregateRoot> GetByKeyAsync<TAggregateRoot>(string key)
                 where TAggregateRoot : class, IAggregateRoot<string>
        {
            var result = await this.GetAggregateAsync<TAggregateRoot>(key);
            ((IPurgeable)result).Purge();
            return result;
        }

        public async Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot, bool purge)
               where TAggregateRoot : class, IAggregateRoot<string>
        {
            await this.SaveAggregateAsync<TAggregateRoot>(aggregateRoot);
            foreach (var evnt in aggregateRoot.UncommittedEvents)
            {
                messagePublisher.Publish(evnt);
            }

            if (purge)
            {
                ((IPurgeable)aggregateRoot).Purge();
            }
        }



        protected abstract Task SaveAggregateAsync<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : class, IAggregateRoot<string>;

        protected abstract Task<TAggregateRoot> GetAggregateAsync<TAggregateRoot>(string aggregateRootKey)
              where TAggregateRoot : class, IAggregateRoot<string>;
    }
}
