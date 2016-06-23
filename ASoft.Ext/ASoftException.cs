using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public class ASoftException : Exception
    {
        public ASoftException() { }

        public ASoftException(string message) : base(message)
        { }

        public ASoftException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public ASoftException(string format, params object[] args)
            : base(string.Format(format, args))
        { }
    }
}
