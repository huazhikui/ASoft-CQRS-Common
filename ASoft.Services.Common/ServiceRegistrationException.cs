using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Services
{
    public class ServiceRegistrationException : ASoftException
    {
        public ServiceRegistrationException()
        { }

        public ServiceRegistrationException(string message) : base(message)
        { }

        public ServiceRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public ServiceRegistrationException(string format, params object[] args)
            : base(string.Format(format, args))
        { }
    }
}
