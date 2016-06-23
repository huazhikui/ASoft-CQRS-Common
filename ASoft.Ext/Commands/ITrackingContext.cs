using ASoft.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Commands
{
    public interface ITrackingContext
    {
        /// <summary>Get all the tracked aggregates.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAggregateRoot> GetTrackedAggregateRoots();
        /// <summary>Clear the tracking context.
        /// </summary>
        void Clear();
    }
}
