using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Commands
{
    [Serializable]
    public class AggregateRootAlreadyExistException : Exception
    {
        private const string ExceptionMessage = "Aggregate root [type={0},id={1}] already exist in command context, cannot be added again.";

        /// <summary>Parameterized constructor.
        /// </summary>
        public AggregateRootAlreadyExistException(object id, Type type) : base(string.Format(ExceptionMessage, type.Name, id)) { }
    }
}
