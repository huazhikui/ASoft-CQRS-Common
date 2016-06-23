using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft
{
    /// <summary>
    /// 随机数生成器类
    /// </summary>
    public static class Rand
    {
        static readonly char[] cpattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        static readonly char[] spattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        /// <summary>
        /// 得到长度为length的字符串,如果length小于等于0,length=6
        /// </summary>
        /// <param name="length">要得到的字符串的长度</param>
        /// <returns>返回生成的字符串</returns>
        public static string Number(int length)
        {
            return Number(length, false);
        }

        /// <summary>
        /// 得到长度为length的字符串,如果length小于等于0,length=6
        /// </summary>
        /// <param name="length">要得到的字符串的长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>返回生成的字符串</returns>

        public static string Number(int length, bool sleep)
        {
            if (sleep)
            {
                System.Threading.Thread.Sleep(1);
            }
            if (length <= 0)
            {
                length = 6;
            }
            Random random = new Random();
            string retValue = "";
            for (int i = 0; i < length; i++)
            {
                retValue += random.Next(10).ToString();
            }
            return retValue;
        }

        /// <summary>
        /// 得到长度为length的字符串,如果length小于等于0,length=6
        /// </summary>
        /// <param name="length">要得到的字符串的长度</param>
        /// <returns>返回生成的字符串</returns>
        public static string Chars(int length)
        {
            return Chars(length, false);
        }

        /// <summary>
        /// 得到长度为length的字符串,如果length小于等于0,length=6
        /// </summary>
        /// <param name="length">要得到的字符串的长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>返回生成的字符串</returns>
        public static string Chars(int length, bool sleep)
        {
            if (sleep)
            {
                System.Threading.Thread.Sleep(1);
            }
            if (length <= 0)
            {
                length = 6;
            }
            Random random = new Random();
            string retValue = "";
            int len = cpattern.Length;
            for (int i = 0; i < length; i++)
            {
                retValue += cpattern[random.Next(len)];
            }
            return retValue;
        }

        /// <summary>
        /// 得到长度为length的字符串,如果length小于等于0,length=6
        /// </summary>
        /// <param name="length">要得到的字符串的长度</param>
        /// <returns>返回生成的字符串</returns>
        public static string NumberChar(int length)
        {
            return NumberChar(length, false);
        }

        /// <summary>
        /// 得到长度为length的字符串,如果length小于等于0,length=6
        /// </summary>
        /// <param name="length">要得到的字符串的长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>返回生成的字符串</returns>
        public static string NumberChar(int length, bool sleep)
        {
            if (sleep)
            {
                System.Threading.Thread.Sleep(1);
            }
            if (length <= 0)
            {
                length = 6;
            }
            Random random = new Random();
            string retValue = "";
            int len = spattern.Length;
            for (int i = 0; i < length; i++)
            {
                retValue += spattern[random.Next(len)];
            }
            return retValue;

        }

        /// <summary>
        /// 获取一个随机的布尔值
        /// </summary>
        public static bool Bool
        {
            get
            {
                System.Threading.Thread.Sleep(1);
                Random r = new Random();
                return r.Next(0, 99) >= 50;
            }
        }

        /// <summary>
        /// 返回当前系统实际的刻度,每次返回时线程暂停1微秒
        /// </summary>
        public static long DateTimeTick
        {
            get
            {
                System.Threading.Thread.Sleep(1);
                return DateTime.Now.Ticks;
            }
        }

    }
}
