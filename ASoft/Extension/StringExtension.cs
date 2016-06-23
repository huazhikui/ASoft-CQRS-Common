using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft.Extension
{
    /// <summary>
    /// String类的扩展
    /// </summary>
    public static class StringExtension
    {
        public static int ToInt(this string value)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Int))
            {
                return int.Parse(value);
            }
            return 0;
        }

        public static int ToInt(this string value, int defaultValue)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Int))
            {
                return int.Parse(value);
            }
            return defaultValue;
        }

        public static long ToLong(this string value)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Long))
            {
                return long.Parse(value);
            }
            return 0;
        }

        public static long ToLong(this string value, long defaultValue)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Long))
            {
                return long.Parse(value);
            }
            return defaultValue;
        }

        public static double ToDouble(this string value)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Double))
            {
                return double.Parse(value);
            }
            return 0.0;
        }

        public static double ToDouble(this string value, double defaultValue)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Double))
            {
                return double.Parse(value);
            }
            return defaultValue;
        }

        public static float ToFloat(this string value)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Float))
            {
                return float.Parse(value);
            }
            return 0.0f;
        }

        public static float ToFloat(this string value, float defaultValue)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Float))
            {
                return float.Parse(value);
            }
            return defaultValue;
        }

        public static Guid ToGuid(this string value)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Guid))
            {
                return new Guid(value);
            }
            return Guid.Empty;
        }

        public static Guid ToGuid(this string value, Guid defaultValue)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Guid))
            {
                return new Guid(value);
            }
            return defaultValue;
        }

        public static DateTime ToDate(this string value)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Date))
            {
                return ASoft.DateUtils.ToDateTime(value, "yyyy-MM-dd");
            }
            return DateTime.MinValue;
        }

        public static DateTime ToDate(this string value, DateTime defaultValue)
        {
            if (value != null && ASoft.Text.ValidateUtils.CheckString(value, Text.ValidateType.Date))
            {
                return ASoft.DateUtils.ToDateTime(value, "yyyy-MM-dd");
            }
            return defaultValue;
        }

        /// <summary>
        /// value 必须有值（不等于null且length>0），返回true，否则将抛出异常
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CheckArgumentToMustHasValue(this string value)
        {
            if (value != null && value.Length > 0)
            {
                return true;
            }
            throw new ArgumentException("Parameter is invalid.", "key", null); 
        }
    }
}
