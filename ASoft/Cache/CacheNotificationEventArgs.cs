using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft
{
    [Serializable]
    public class CacheNotificationEventArgs : EventArgs
    {
        public string Key { get; set; }

        public string Type { get; set; }

        public string ClientName { get; set; }

        public TimeToLive SpecificTimeToLive { get; set; }
    }
}
