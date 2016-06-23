using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft
{
    public interface ICachePublisher
    {
        void NotifyUpdate(string key, string type);
        void NotifyUpdate(string key, string type, TimeSpan? specificTimeToLive);
        void NotifyDelete(string key);
    }
}
