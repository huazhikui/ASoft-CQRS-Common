using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft
{
    [Serializable]
    public class TimeToLive
    {
        public TimeToLive() { }

        public TimeSpan? LiveTimeSpan { get; private set; }
        public TimeToLive(TimeSpan? timeToLive)
        {
            LiveTimeSpan = timeToLive;
        }
    }
}
