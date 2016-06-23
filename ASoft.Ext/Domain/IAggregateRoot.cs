using System.Collections.Generic;
using ASoft.Events;
using System;

namespace ASoft.Domain
{
    public interface IAggregateRoot:IAggregateRoot<string>
    { }
    public interface IAggregateRoot<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    { 
        int Version { get; }
        IEnumerable<IDomainEvent> UncommittedEvents { get; }

        void Replay(IEnumerable<IDomainEvent> events);
    }
}