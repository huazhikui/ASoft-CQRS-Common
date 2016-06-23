using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public static class RangeExt
    {
        public static T Next<T>(this IEnumerable<T> source, T current)
        {
            var index = 0;
            T result = default(T);
            var count = source.Count();
          
            foreach (var item in source)
            {
                index++;
                if (item.Equals(current) && count>index)
                {
                    result = source.ElementAt(index);
                    return result;
                }
                index++;
            }
            return default(T);
        }
    }

    public class Range<TA, TB> : IRange<TA, TB>
        where TA : class, IRange<TA, TB>
        where TB : class, IComparable<TB>
    {
        public Range(TB startTime, TB endTime)
        {
            var compare = startTime.CompareTo(endTime);
            if (compare != -1)
            {
                throw new ArgumentOutOfRangeException("参数的值非预期范围的值，请确认endTime>startTime");
            }
            this.Start = startTime;
            this.End = endTime;
        }

        public TB End
        {
            get;
            protected set;
        }

        public TB Start
        {
            get;
            protected set;
        }

        /// <summary>
        /// 比较并返回
        /// 大于且相邻 = 1,
        /// 大于不相邻 = 2,
        /// 等于 = 0,
        /// 小于且相邻 = -1,
        /// 小于不相邻 = -2,
        /// 相交 = 3,
        /// 包含 = 4,
        /// 被包含 = 5,
        /// 未比较 = -1000
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TA other)
        {
            var result = TimeRangeCompareTo(other);
            return (int)result;
        }

        /// <summary>
        /// 比较并返回
        /// 大于且相邻 = 1,
        /// 大于不相邻 = 2,
        /// 等于 = 0,
        /// 小于且相邻 = -1,
        /// 小于不相邻 = -2,
        /// 相交 = 3,
        /// 包含 = 4,
        /// 被包含 = 5,
        /// 未比较 = -1000
        /// </summary>
        /// <param name="other"></param>
        /// <returns> 
        /// </returns>
        public RangeCompareResult TimeRangeCompareTo(TA other)
        {
            int compareStart = 0;
            int compareEnd = 0;
            int compareStartToEnd = 0;
            int compareEndToStart = 0;

            if (other==null)
            {
                throw new ArgumentNullException("比较的参数不能为null");
            }

            compareEndToStart = this.Start.CompareTo(other.Start);
            if (compareEndToStart == -1)
            {
                return RangeCompareResult.小于不相邻;

            }
            else if (compareEndToStart == 0)
            {
                return RangeCompareResult.小于且相邻;
            }

            compareStartToEnd = this.Start.CompareTo(other.End);
            if (compareStartToEnd == 1)
            {
                return RangeCompareResult.大于不相邻;
            }
            else if (compareStartToEnd == 0)
            {
                return RangeCompareResult.大于且相邻;
            }

            compareStart = this.Start.CompareTo((TB)other.Start);
            compareEnd = this.End.CompareTo((TB)other.End);
            if (compareStart == 0 && compareEnd == 0)
            {
                return RangeCompareResult.等于;
            }

            if (compareStart == 1)
            {
                if (compareEnd == 1) return RangeCompareResult.相交;
                else if (compareEnd == 0 || compareEnd == -1) return RangeCompareResult.被包含;
            }
            else if (compareStart == -1)
            {
                if (compareEnd == 1 || compareEnd == 0) return RangeCompareResult.包含;
                else if (compareEnd == -1) return RangeCompareResult.被包含;
            }

            return RangeCompareResult.未比较;
        }

        /// <summary>
        /// 根据两个去接获取可将其连接的区间
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public TA PaddingTo(TA other)
        {
            var compare = TimeRangeCompareTo(other);

            if (compare == RangeCompareResult.大于不相邻)
            {
                var result = (new Range<TA, TB>(other.End, this.Start));
                return result as TA;
            }
            else if (compare == RangeCompareResult.小于不相邻)
            {
                var result = (new Range<TA, TB>(this.End, other.Start));
                return result as TA;
            }
            return null;
        }

        /// <summary>
        /// 包含在区间内
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Contains(TB b)
        {
            var compareToStart = this.Start.CompareTo(b);
            var compareToEnd = this.End.CompareTo(b);
            if ((compareToStart == 0 || compareToStart == -1)
                && (compareToEnd == 1 || compareToEnd == 0))
            {
                return true;
            }

            return false;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public enum RangeCompareResult
    {
        大于且相邻 = 1,
        大于不相邻 = 2,
        等于 = 0,
        小于且相邻 = -1,
        小于不相邻 = -2,
        相交 = 3,
        包含 = 4,
        被包含 = 5,
        未比较 = -1000
    }
}
