using System;

namespace ASoft.Domain.Exception
{
    public class AggregateNotFoundException : System.Exception
    {
        public AggregateNotFoundException(Type t, string id)
            : base($"Aggregate {id} of type {t.FullName} was not found")
        { }
    }
}