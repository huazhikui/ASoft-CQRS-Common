using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public interface ICacheSubscriber
    {
        event EventHandler<CacheNotificationEventArgs> CacheUpdate;
        event EventHandler<CacheNotificationEventArgs> CacheDelete;
        Task<object> GetAsync(string key, Type type);

    }
}
