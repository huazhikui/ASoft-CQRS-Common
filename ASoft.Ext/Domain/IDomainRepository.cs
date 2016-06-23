
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASoft.Domain
{
    public interface IDomainRepository
    {
        Task SaveAsync<TAggregateRoot>(TAggregateRoot aggregateRoot, bool purge = true) 
          where TAggregateRoot : class, IAggregateRoot<string>;

        Task<TAggregateRoot> GetByKeyAsync< TAggregateRoot>(string key)
             where TAggregateRoot : class, IAggregateRoot<string>;
    }
}