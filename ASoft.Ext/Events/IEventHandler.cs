using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Events
{
    public interface IEventHandler
    {

    }

    public interface IEventHandler<in T> : IEventHandler
        where T : class, IDomainEvent
    {
        Task HandleAsync(T message);
    }
}
