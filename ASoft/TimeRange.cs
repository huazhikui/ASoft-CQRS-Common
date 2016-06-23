using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{

    public class DateTimeRangeOneDay : TimeRangeOneDay
    {
        /// <summary>
        /// 按日期构建从当日的时间范围
        /// </summary>
        /// <param name="dateTime"></param>
        public DateTimeRangeOneDay(DateTime dateTime) : base(dateTime.Date.AddDays(1).AddMilliseconds(-1))
        {
            StartDateTime = dateTime.Date;
            EndDateTime = dateTime.Date.AddDays(1).AddMilliseconds(-1);
        }

        public DateTimeRangeOneDay(DateTime startDateTime, DateTime endDateTime) : base(new OnlyTimeOneDay(startDateTime), new OnlyTimeOneDay(endDateTime))
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }

        public DateTime StartDateTime
        {
            private set;
            get;
        }

        public DateTime EndDateTime
        {
            private set;
            get;
        }
    }

    /// <summary>
    /// 单日内的时间范围
    /// </summary>
    public class TimeRangeOneDay : Range<TimeRangeOneDay, OnlyTimeOneDay>
    {
        public DateTime GetStartDateTime(DateTime date)
        {
            return date.Add(Start.TimeSpan);
        }

        public DateTime GetEndDateTime(DateTime date)
        {
            return date.Add(End.TimeSpan);
        }

        /// <summary>
        /// 从当天0时开始到dateTime的时间截止
        /// </summary>
        /// <param name="dateTime"></param>
        public TimeRangeOneDay(DateTime dateTime) : this(new OnlyTimeOneDay(dateTime.Date), new OnlyTimeOneDay(dateTime))
        {

        }

        public TimeRangeOneDay(OnlyTimeOneDay startTime, OnlyTimeOneDay endTime) : base(startTime, endTime)
        {

        }


    }

    public class OnlyTimeOneDay : IComparable<OnlyTimeOneDay>
    {
        public int Hours { private set; get; }

        public int Minutes { private set; get; }

        public int Seconds { private set; get; }

        public TimeSpan TimeSpan
        {
            private set;
            get;
        }

        public OnlyTimeOneDay(DateTime dateTime) : this(dateTime.Hour, dateTime.Minute, dateTime.Second)
        {
        }

        public OnlyTimeOneDay(int hours, int minutes, int seconds)
        {
            if (hours < 0 && hours >= 24)
            {
                throw new ArgumentOutOfRangeException("参数hours超出了允许的范围0~23");
            }
            if (minutes < 0 && minutes >= 60)
            {
                throw new ArgumentOutOfRangeException("参数minutes超出了允许的范围0~23");
            }
            if (seconds < 0 && seconds >= 60)
            {
                throw new ArgumentOutOfRangeException("参数seconds超出了允许的范围0~23");
            }
            this.Hours = hours;
            this.Minutes = minutes;
            Seconds = seconds;
            TimeSpan = new TimeSpan(hours, minutes, seconds);
        }

        public int CompareTo(OnlyTimeOneDay other)
        {
            return this.TimeSpan.CompareTo(other.TimeSpan);
        }

    }
}
