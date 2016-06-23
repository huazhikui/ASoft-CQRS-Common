using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Domain
{
    public interface IAggregateStorage
    {
        /// <summary>Get an aggregate from aggregate storage.
        /// </summary>
        /// <param name="aggregateRootType"></param>
        /// <param name="aggregateRootId"></param>
        /// <returns></returns>
        IAggregateRoot Get(Type aggregateRootType, string aggregateRootId);
    }
}
