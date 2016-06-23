using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft;

namespace ASoft.Text
{
    public class IDUtils
    {
        /// <summary>
        /// 从身份证号获取性别
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static String GetSex(String code)
        {
            String sex = "";
            String lastChar = code.Substring(code.Length - 2, 1);
            //最后一位是奇偶判断男女
            if (int.Parse(lastChar) % 2 == 0)
            {
                sex = "女";
            }
            else
            {
                sex = "男";
            }
            return sex;
        }

        /// <summary>
        /// 从身份证号获取生日日期
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DateTime GetBirthDay(String code)
        {
            String strBirthDay = "";
            DateTime birth;
            if (code.Length == 15)
            {
                strBirthDay = "19" + code.Substring(6, 6);
            }
            else
            {
                strBirthDay = code.Substring(6, 8);
            }
            birth = DateUtils.ToDateTime(strBirthDay, "yyyyMMdd");
            return birth;
        }

        /// <summary>
        /// 根据身份证号获取年龄
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static uint GetAge(String code)
        {
            String year = "";
            uint age = 0;
            if (code.Length == 15)
            {
                year = "19" + code.Substring(6, 2);
            }
            else
            {
                year = code.Substring(6, 4);
            }
            age = (uint)(DateTime.Now.Year - int.Parse(year));
            return age;
        }

        public bool CheckIDcode(string code)
        {
            string pattern = @"(^[1-9][0-9]{5}((19[0-9]{2})|(200[0-9])|2011)(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[0-9]{3}[0-9xX]$)";
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(code, pattern);
            if (match.Groups.Count > 1)
            {
                return true;
                //return "18位身份证号码有效!";
            }
            pattern = @"(^[1-9][0-9]{5}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[0-9]{3}$)";
            match = System.Text.RegularExpressions.Regex.Match(code, pattern);
            if (match.Groups.Count > 1)
            {
                return true;
                //return "15位身份证号码有效!";
            }
            return false;
        }
    }
}
