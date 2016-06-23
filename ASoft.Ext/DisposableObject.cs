using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public abstract class DisposableObject : IDisposable
    {
        protected readonly log4net.ILog log;
        public DisposableObject()
        { 
            log = log4net.LogManager.GetLogger(this.GetType().FullName);
        }
        ~DisposableObject()
        {
            this.Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }
    }
}
