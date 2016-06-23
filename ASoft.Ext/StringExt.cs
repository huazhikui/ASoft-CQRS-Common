using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public static class StringExt
    {
        public static bool IsNullOrLengthEquelsZero (this string source){
            if (source == null || source.Length == 0)
            {
                return true;
            }
            return false;
        }
    }
}
