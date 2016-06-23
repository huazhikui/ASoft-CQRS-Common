using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public interface IRange<TA, TB> : IComparable<TA>
    {
        TB End { get; }
        TB Start { get; }
        RangeCompareResult TimeRangeCompareTo(TA other);

        TA PaddingTo(TA other);

        bool Contains(TB b);
    }
}
