using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ASoft
{
    /// <summary>
    /// 日期处理常用方法
    /// </summary>
    public class DateUtils
    {
        private static ChineseLunisolarCalendar chineseData = new ChineseLunisolarCalendar();

        public static CultureInfo cnci = new CultureInfo("zh-cn");
        #region 1970-01-01 00:00:00到现在的时间
        /// <summary>
        /// 时间戳转换成时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime  FromUnixTime(long timeStamp)
        {
            return DateTime.Parse("1970-01-01 00:00:00").AddSeconds(timeStamp);
        }

        /// <summary>
        /// 时间转换成时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long UnixTimeStamp(DateTime dateTime)
        {
            return (dateTime.Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000000;
        }
        #endregion

        #region 获取一个月的所有星期几
        /// <summary>
        /// 获取一个月的所有星期几
        /// </summary>
        /// <param name="dt">这个月的某个时间</param>
        /// <param name="day">星期几</param>
        /// <returns>这个月所有星期几的列表</returns>
        public static List<DateTime> GetWeekdayOfMonth(DateTime dt, DayOfWeek day)
        {
            List<DateTime> result = new List<DateTime>();
            DateTime firstday = dt.AddDays(1 - dt.Day).Date; //获取当月第一天
            DateTime lastday = firstday.AddMonths(1).AddDays(-1);   //获取当月最后一天

            if (dt.DayOfWeek != day)
            {
                dt = firstday.AddDays(((int)day + 7 - (int)firstday.DayOfWeek) % 7);//取当月第一个星期几
            }
            result.Add(dt);
            dt = dt.AddDays(7);
            while (dt.Date <= lastday.Date)
            {
                result.Add(dt);
                dt = dt.AddDays(7);
            }
            return result;
        }

        /// <summary>
        /// 获取星期几
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>返回星期几</returns>
        public static string GetWeekDay(DateTime date)
        {
            return date.ToString("dddd", cnci);
        }
        #endregion

        #region 获取一年的所有星期几
        /// <summary>
        /// 获取一个月的所有星期几
        /// </summary>
        /// <param name="dt">这个月的某个时间</param>
        /// <param name="day">星期几</param>
        /// <returns>这个月所有星期几的列表</returns>
        public static List<DateTime> GetWeekdayOfYear(DateTime dt, DayOfWeek day)
        {
            List<DateTime> result = new List<DateTime>();
            DateTime firstday = dt.AddDays(1 - dt.DayOfYear).Date; //获取当年第一天
            DateTime lastday = firstday.AddYears(1).AddDays(-1).Date;   //获取当年最后一天


            if (dt.DayOfWeek != day)
            {
                dt = firstday.AddDays(((int)day + 7 - (int)firstday.DayOfWeek) % 7);//取当年第一个星期几
            }
            result.Add(dt);
            dt = dt.AddDays(7);
            while (dt.Date <= lastday.Date)
            {
                result.Add(dt);
                dt = dt.AddDays(7);
            }
            return result;
        }
        #endregion

        #region 获取某个时间是当年的第几周
        /// <summary>
        /// 获取某个时间是当年的第几周,默认一周的开始时间是星期一。
        /// </summary>
        /// <param name="dt">给定的时间</param>
        /// <returns>返回给定的时间是当前的第几周</returns>
        public static int GetWeekOfYear(DateTime dt)
        {
            return GetWeekOfYear(dt, DayOfWeek.Monday);
        }
        #endregion

        #region 获取某个时间是当年的第几周
        /// <summary>
        /// 获取某个时间是当年的第几周
        /// </summary>
        /// <param name="dt">给定的时间</param>
        /// <param name="firstDay">指定一周的第一天是哪一天</param>
        /// <returns>返回给定的时间是当前的第几周</returns>
        public static int GetWeekOfYear(DateTime dt, DayOfWeek firstDay)
        {
            return cnci.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, firstDay);
        }
        #endregion

       
        /// <summary>
        /// 获取某日期的季度的开始时间和结束时间
        /// </summary>
        /// <param name="date"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void GetQuarterlyScope(DateTime date,out DateTime start, out DateTime end)
        {
            int endMonth = ((int)(date.Month / 3) +1) * 3;
            if (endMonth > 12)
            {
                  endMonth = 12;
            }
            int startMonth = endMonth-2;
            //月份中的天数
            int endMonthDays = GetDaysOfMonth(date.Year, endMonth);
            start= new DateTime(date.Year,startMonth,1);
            end = new DateTime(date.Year,endMonth,endMonthDays);
        }



        #region 获取某周的开始时间和结束时间
        /// <summary>
        /// 获取某周的开始时间和结束时间
        /// </summary>
        /// <param name="week">要获取第几周</param>
        /// <param name="year">年份</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        public static void GetWeekScope(int week, int year, out DateTime start, out DateTime end)
        {
            GetWeekScope(week, year, DayOfWeek.Monday, out start, out end);
        }
        #endregion

        #region 获取某周的开始时间和结束时间
        /// <summary>
        /// 获取某周的开始时间和结束时间
        /// </summary>
        /// <param name="week">要获取第几周</param>
        /// <param name="year">年份</param>
        /// <param name="firstDay">一周的开始为哪一天</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        public static void GetWeekScope(int week, int year, DayOfWeek firstDay, out DateTime start, out DateTime end)
        {
            if (week == 0 || week > 54)
            {
                throw new Exception("week的值应在1到54之间");
            }
            if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
            {
                throw new Exception(string.Format("year的值应在{0}到{1}之间", DateTime.MinValue.Year, year > DateTime.MaxValue.Year));
            }
            DateTime dt = new DateTime(year, 1, 1);
            start = dt.AddDays((week - 1) * 7 - ((int)dt.DayOfWeek + 7 - (int)firstDay) % 7);
            end = start.AddDays(7).AddTicks(-1);
        }
        #endregion

        #region 获取格式化的日期时间
        /// <summary>
        /// 返回默认格式化的日期(格式化字符串:yyyy-MM-dd)
        /// </summary>
        /// <param name="dt">要格式化的时间</param>
        /// <returns>格式化后的时间</returns>
        public static string GetDate(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 返回默认格式化的时间(格式化字符串:HH:mm:ss)
        /// </summary>
        /// <param name="dt">要格式化的时间</param>
        /// <returns>格式化后的时间</returns>
        public static string GetTime(DateTime dt)
        {
            return dt.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回默认格式化的日期和时间(格式化字符串:yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="dt">要格式化的时间</param>
        /// <returns>格式化后的时间</returns>
        public static string GetDateTime(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回使用指定格式化串的日期/时间
        /// </summary>
        /// <param name="dt">要格式化的时间</param>
        /// <param name="format">格式化时间使用的字符串</param>
        /// <returns>格式化后的时间</returns>
        public static string GetFormatTime(DateTime dt, string format)
        {
            return dt.ToString(format);
        }
        #endregion

        #region 把一个表示时间字符串按照指定的格式转换为时间
        /// <summary>
        /// 把一个表示时间字符串按照指定的格式转换为时间
        /// </summary>
        /// <param name="date">时间(如:20090921)</param>
        /// <param name="format">样式(如:yyyyMMdd)</param>
        /// <returns>转换后的时间</returns>
        public static DateTime ToDateTime(string date, string format)
        {
            return DateTime.ParseExact(date, format, cnci);
        }
        #endregion

        #region 返回指定的年月的天数
        /// <summary>
        /// 获取某个月有多少天
        /// </summary>
        /// <param name="dt">给定日期</param>
        /// <returns>返回指定的年月的天数</returns>
        public static int GetDaysOfMonth(DateTime dt)
        {
            return GetDaysOfMonth(dt.Year, dt.Month);
        }

        /// <summary>
        /// 获取某个月有多少天
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>返回指定的年月的天数</returns>
        public static int GetDaysOfMonth(int year, int month)
        {
            if (month > 12 || month < 1)
            {
                throw new Exception("给定的年份不正确");
            }
            int days = 0;
            switch (month)
            {
                case 2:
                    {
                        if (DateTime.IsLeapYear(year))
                        {
                            days = days = 29;
                        }
                        else
                        {
                            days = 28;
                        }
                        break;
                    }
                case 4:
                case 6:
                case 9:
                case 11:
                    {
                        days = 30;
                        break;
                    }
                default:
                    {
                        days = 31;
                        break;
                    }
            }
            return days;
        }
        #endregion

        #region 星期相关
        /// <summary>
        /// 返回当前日期的星期名称
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>星期名称</returns>
        public static DayOfWeekCn GetDayOfWeekCn(DateTime dt)
        {
            return (DayOfWeekCn)(int)dt.DayOfWeek;
        }
        /// <summary>
        /// 返回当前日期的星期编号(星期天对应7)
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>星期数字编号</returns>
        public static int GetWeekNumberOfDay(DateTime dt)
        {
            return (int)dt.DayOfWeek == 0 ? 7 : (int)dt.DayOfWeek;
        }
        #endregion

        #region 星座相关
        /// <summary>
        /// 根据输入的日期返回对应的星座
        /// </summary>
        /// <param name="dt">要查询的日期</param>
        /// <returns>查询的日期对应的星座</returns>
        public Constellation GetConstellation(DateTime dt)
        {
            int calc = dt.Month * 100 + dt.Day;

            if ((calc >= 321) && (calc <= 419))
            {
                return Constellation.白羊座;
            }

            else if ((calc >= 420) && (calc <= 520))
            {
                return Constellation.金牛座;
            }
            else if ((calc >= 521) && (calc <= 620))
            {
                return Constellation.双子座;
            }

            else if ((calc >= 621) && (calc <= 722))
            {
                return Constellation.巨蟹座;
            }

            else if ((calc >= 723) && (calc <= 822))
            {
                return Constellation.狮子座;
            }

            else if ((calc >= 823) && (calc <= 922))
            {
                return Constellation.处女座;
            }

            else if ((calc >= 923) && (calc <= 1023))
            {
                return Constellation.天秤座;
            }

            else if ((calc >= 1024) && (calc <= 1122))
            {
                return Constellation.天蝎座;
            }

            else if ((calc >= 1123) && (calc <= 1221))
            {
                return Constellation.射手座;
            }

            else if ((calc >= 1222) || (calc <= 119))
            {
                return Constellation.摩羯座;
            }

            else if ((calc >= 120) && (calc <= 218))
            {
                return Constellation.水瓶座;
            }

            else { return Constellation.双鱼座; }
        }
        #endregion

        /// <summary>
        /// 获取某个指定日期的生肖
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static AnimalSign GetAnimalSign(DateTime dt)
        {
            if (IsLunisolarSupported(dt.Year))
            {
                return (AnimalSign)chineseData.GetTerrestrialBranch(chineseData.GetSexagenaryYear(dt));
            }
            else
            {
                throw new Exception("不支持给定的时间");
            }

        }

     

        /// <summary>
        /// 检查农历是否支持给定的年份
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool IsLunisolarSupported(int year)
        {
            if (year > 1901 && year < 2101)
            {
                return true;
            }
            return false;

        }

        #region 节气相关
        #endregion
    }

    /// <summary>
    /// 星座列表
    /// </summary>
    public enum Constellation
    {
        /// <summary>
        /// 水瓶座01月20日~02月18日
        /// </summary>
        水瓶座,

        /// <summary>
        /// 双鱼座02月19日~03月20日
        /// </summary>
        双鱼座,

        /// <summary>
        /// 白羊座03月21日~04月19日
        /// </summary>
        白羊座,

        /// <summary>
        /// 金牛座04月20日~05月20日
        /// </summary>
        金牛座,

        /// <summary>
        /// 双子座05月21日~06月21日
        /// </summary>
        双子座,

        /// <summary>
        /// 巨蟹座06月22日~07月22日
        /// </summary>
        巨蟹座,

        /// <summary>
        /// 狮子座07月23日~08月22日
        /// </summary>
        狮子座,

        /// <summary>
        /// 处女座08月23日~09月22日
        /// </summary>
        处女座,

        /// <summary>
        /// 天秤座09月23日~10月23日
        /// </summary>
        天秤座,

        /// <summary>
        /// 天蝎座10月24日~11月22日
        /// </summary>
        天蝎座,

        /// <summary>
        /// 射手座11月23日~12月21日
        /// </summary>
        射手座,

        /// <summary>
        /// 魔羯座12月22日~01月19日
        /// </summary>
        摩羯座
    }
    public enum ChineseMonth { 
        一月=1,
        二月 = 2,
        三月 = 3,
        四月 = 4,
        五月 = 5,
        六月 = 6,
        七月 = 7,
        八月 = 8,
        九月 = 9,
        十月 = 10,
        十一月 = 11,
        十二月 = 12
    }


    /// <summary>
    /// 12生肖
    /// </summary>
    public enum AnimalSign
    {
        /// <summary>
        /// 鼠
        /// </summary>
        鼠 = 1,

        /// <summary>
        /// 牛
        /// </summary>
        牛 = 2,

        /// <summary>
        /// 虎
        /// </summary>
        虎 = 3,

        /// <summary>
        /// 兔
        /// </summary>
        兔 = 4,

        /// <summary>
        /// 龙
        /// </summary>
        龙 = 5,

        /// <summary>
        /// 蛇
        /// </summary>
        蛇 = 6,

        /// <summary>
        /// 马
        /// </summary>
        马 = 7,

        /// <summary>
        /// 羊
        /// </summary>
        羊 = 8,

        /// <summary>
        /// 猴
        /// </summary>
        猴 = 9,

        /// <summary>
        /// 鸡
        /// </summary>
        鸡 = 10,

        /// <summary>
        /// 狗
        /// </summary>
        狗 = 11,

        /// <summary>
        /// 猪
        /// </summary>
        猪 = 12
    }

    /// <summary>
    /// 12地支
    /// </summary>
    public enum EarthlyBranch
    {
        /// <summary>
        /// 子
        /// </summary>
        子,

        /// <summary>
        /// 丑
        /// </summary>
        丑,

        /// <summary>
        /// 寅
        /// </summary>
        寅,

        /// <summary>
        /// 卯
        /// </summary>
        卯,

        /// <summary>
        /// 辰
        /// </summary>
        辰,

        /// <summary>
        /// 巳
        /// </summary>
        巳,

        /// <summary>
        /// 午
        /// </summary>
        午,

        /// <summary>
        /// 未
        /// </summary>
        未,

        /// <summary>
        /// 申
        /// </summary>
        申,

        /// <summary>
        /// 酉
        /// </summary>
        酉,

        /// <summary>
        /// 戌
        /// </summary>
        戌,

        /// <summary>
        /// 亥
        /// </summary>
        亥
    }

    /// <summary>
    /// 天干
    /// </summary>
    public enum HeavenlyStem
    {
        /// <summary>
        /// 甲
        /// </summary>
        甲,

        /// <summary>
        /// 乙
        /// </summary>
        乙,

        /// <summary>
        /// 丙
        /// </summary>
        丙,

        /// <summary>
        /// 丁
        /// </summary>
        丁,

        /// <summary>
        /// 戊
        /// </summary>
        戊,

        /// <summary>
        /// 己
        /// </summary>
        己,

        /// <summary>
        /// 庚
        /// </summary>
        庚,

        /// <summary>
        /// 辛
        /// </summary>
        辛,

        /// <summary>
        /// 壬
        /// </summary>
        壬,

        /// <summary>
        /// 癸
        /// </summary>
        癸
    }

    /// <summary>
    /// 24个节气
    /// </summary>
    public enum SolarTerms
    {
        /// <summary>
        /// 立春
        /// </summary>
        立春,

        /// <summary>
        /// 雨水
        /// </summary>
        雨水,

        /// <summary>
        /// 惊蛰
        /// </summary>
        惊蛰,

        /// <summary>
        /// 春分
        /// </summary>
        春分,

        /// <summary>
        /// 清明
        /// </summary>
        清明,

        /// <summary>
        /// 谷雨
        /// </summary>
        谷雨,

        /// <summary>
        /// 立夏
        /// </summary>
        立夏,

        /// <summary>
        /// 小满
        /// </summary>
        小满,

        /// <summary>
        /// 芒种
        /// </summary>
        芒种,

        /// <summary>
        /// 夏至
        /// </summary>
        夏至,

        /// <summary>
        /// 小暑
        /// </summary>
        小暑,

        /// <summary>
        /// 大暑
        /// </summary>
        大暑,

        /// <summary>
        /// 立秋
        /// </summary>
        立秋,

        /// <summary>
        /// 处暑
        /// </summary>
        处暑,

        /// <summary>
        /// 白露
        /// </summary>
        白露,

        /// <summary>
        /// 秋分
        /// </summary>
        秋分,

        /// <summary>
        /// 寒露
        /// </summary>
        寒露,

        /// <summary>
        /// 霜降
        /// </summary>
        霜降,

        /// <summary>
        /// 立冬
        /// </summary>
        立冬,

        /// <summary>
        /// 小雪
        /// </summary>
        小雪,
        /// <summary>
        /// 大雪
        /// </summary>
        大雪,

        /// <summary>
        /// 冬至
        /// </summary>
        冬至,

        /// <summary>
        /// 小寒
        /// </summary>
        小寒,

        /// <summary>
        /// 大寒
        /// </summary>
        大寒
    }

    /// <summary>
    /// 中文的星期列表
    /// </summary>
    public enum DayOfWeekCn
    {
        /// <summary>
        /// 星期日
        /// </summary>
        星期日 = 0,

        /// <summary>
        /// 星期一
        /// </summary>
        星期一 = 1,

        /// <summary>
        /// 星期二
        /// </summary>
        星期二 = 2,

        /// <summary>
        /// 
        /// </summary>
        星期三 = 3,

        /// <summary>
        /// 星期四
        /// </summary>
        星期四 = 4,

        /// <summary>
        /// 星期五
        /// </summary>
        星期五 = 5,

        /// <summary>
        /// 星期六
        /// </summary>
        星期六 = 6

    }
}
