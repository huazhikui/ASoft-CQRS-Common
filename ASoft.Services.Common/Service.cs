using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Services
{
    public abstract class Service : DisposableObject, IService
    {
        /// <summary>
        /// Starts the service with specified arguments.
        /// </summary>
        /// <param name="args">The arguments with which the service will use
        /// to start.</param>
        public abstract void Start(object[] args);
    }
}
