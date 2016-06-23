using System;
using System.Text.RegularExpressions;

namespace ASoft
{
    /// <summary>
    /// 提供正则验证功能.
    /// </summary>
    public static class Regular
    {
        /// <summary>
        /// 调用Regex中IsMatch函数实现一般的正则表达式匹配
        /// </summary>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false。</returns>
        public static bool IsMatch(string pattern, string input)
        {
            return Regex.IsMatch(input, pattern);
        }


        /// <summary>
        /// 调用Regex中IsMatch函数实现一般的正则表达式匹配
        /// </summary>
        /// <param name="pattern">要匹配的正则表达式模式。</param>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <param name="options">正则表达式选项</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false。</returns>
        public static bool IsMatch(string pattern, string input, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        /// <summary>
        /// 从输入字符串中的第一个字符开始，用替换字符串替换指定的正则表达式模式的所有匹配项。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="replacement">用于替换的字符串</param>
        /// <returns>返回被替换后的结果</returns>
        public static string Replace(string pattern, string input, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        /// <summary>
        /// 从输入字符串中的第一个字符开始，用替换字符串替换指定的正则表达式模式的所有匹配项。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="evaluator">找到每个正则表达式时调用的方法</param>
        /// <param name="options">正则表达式找到匹配项</param>
        /// <returns>返回被替换后的结果</returns>
        public static string Replace(string input, string pattern, MatchEvaluator evaluator, RegexOptions options)
        {
            return Regex.Replace(input, pattern, evaluator, options);
        }

        /// <summary>
        /// 从输入字符串中的第一个字符开始，用替换字符串替换指定的正则表达式模式的所有匹配项。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="replacement">用于替换的字符串</param>
        /// <param name="options">正则表达式找到匹配项</param>
        /// <returns>返回被替换后的结果</returns>
        public static string Replace(string input, string pattern, string replacement, RegexOptions options)
        {
            return Regex.Replace(input, pattern, replacement, options);
        }

        /// <summary>
        /// 在由正则表达式模式定义的位置拆分输入字符串。
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="options">正则表达式选项</param>
        /// <returns>分割后的字符串数组</returns>
        public static string[] Split(string pattern, string input, RegexOptions options)
        {
            return Regex.Split(input, pattern, options);
        }

        /// <summary>
        /// 获取匹配字符串
        /// </summary>
        /// <param name="pattern">模式字符串</param>
        /// <param name="input">输入字符串</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static MatchCollection Matches(string pattern, string input, RegexOptions options)
        {
            return Regex.Matches(input, pattern, options);
        }
    }
}
