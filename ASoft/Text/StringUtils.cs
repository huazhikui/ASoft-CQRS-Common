using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized; 
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace ASoft.Text
{
    /// <summary>
    /// 字符串处理常用类
    /// </summary>
    public static class StringUtils
    {
        #region 简繁转换

        private static string TextConvert(string text, ChineseConversionDirection direction)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            OfficeConversionEngine engine = OfficeConversionEngine.Create();
            if (engine != null)
            {
                return engine.TCSCConvert(text, direction);
            }
            int dwMapFlags = (direction == ChineseConversionDirection.TraditionalToSimplified) ? 0x2000000 : 0x4000000;
            int cb = (text.Length * 2) + 2;
            IntPtr lpDestStr = Marshal.AllocHGlobal(cb);
            NativeMethods.LCMapString(0x804, (uint)dwMapFlags, text, -1, lpDestStr, cb);
            string str = Marshal.PtrToStringUni(lpDestStr);
            Marshal.FreeHGlobal(lpDestStr);
            return str;
        }

        /// <summary>
        /// 将参数转换为繁体中文
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string ConvertToTraditional(string value)
        {
            return TextConvert(value, ChineseConversionDirection.SimplifiedToTraditional);
        }

        /// <summary>
        /// 将参数转换为简体中文
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>转换后的字符串</returns>
        public static string ConvertToSimplified(string value)
        {
            return TextConvert(value, ChineseConversionDirection.TraditionalToSimplified);
        }
        #endregion

        #region 得到字符串的像素宽度
        /// <summary>
        /// 获取字符串的像素宽度
        /// </summary>
        /// <param name="input">要计算的字符串</param>
        /// <param name="intDoubleCharWidth">双字节字符(汉字)宽度</param>
        /// <param name="intCharWidth">单字节字符的宽度</param>
        /// <returns></returns>
        public static int GetStringPixelWidth(String input, int intDoubleCharWidth, int intCharWidth)
        {
            int totalWidth = 0;
            //双字节字符的个数
            int intDoubleCharCount = StringUtils.GetBytesLength(input) - input.Length;
            //单字节字符的个数
            int intCharCount = input.Length - intDoubleCharCount;
            totalWidth = intDoubleCharCount * intDoubleCharWidth + intCharCount * intCharWidth;
            return totalWidth;
        }
        #endregion

        #region 得到字符串的长度(字节)
        /// <summary>
        /// 得到字符串的长度(按字节计算,使用编码GB2312)
        /// </summary>
        /// <param name="input">要计算的字符串</param>
        /// <returns>返回字符串的长度</returns>
        public static int GetBytesLength(string input)
        {
            return GetBytesLength(input, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 得到字符串的长度(按字节计算,使用编码GB2312)
        /// </summary>
        /// <param name="input">要计算的字符串</param>
        /// <param name="encode">要使用的编码</param>
        /// <returns>返回字符串的长度</returns>
        public static int GetBytesLength(string input, Encoding encode)
        {
            return encode.GetBytes(input).Length;
        }
        #endregion

        #region 是否浏览器支持的图片
        /// <summary>
        /// 检查一个文件名是否是浏览器可以显示的图片文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>浏览器是否可以直接显示</returns>
        public static bool IsBrowserImgFile(string filename)
        {
            bool result = false;
            if (filename != null)
            {
                string ext = ASoft.IO.Helper.GetFileExtention(filename);
                if (ext.Length > 0)
                {
                    ext = ext.ToLower();
                    result = (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif");
                }
            }
            return result;
        }
        #endregion

        #region 返回一个字符串的子串,汉字按照两个字符计算
        /// <summary>
        /// 返回一个字符串的字串
        /// </summary>
        /// <param name="input">设置返回的字符串的结尾的值.如:…</param>
        /// <param name="len">要返回的字符串的长度(字节)如果不大于0,则返回string.Empty</param>
        /// <returns>要返回的字符串</returns>
        public static string SubString(string input, int len)
        {
            string myResult = input;
            if (len <= 0)
            {
                return string.Empty;
            }
            byte[] bsSrcString = Encoding.GetEncoding("GB2312").GetBytes(input);
            if (bsSrcString.Length < len)
            {
                return myResult;
            }
            len -= 2;
            int nRealLength = len;
            int[] anResultFlag = new int[len];
            byte[] bsResult = null;
            int nFlag = 0;
            for (int i = 0; i < len; i++)
            {
                if (bsSrcString[i] > 0x7f)
                {
                    nFlag++;
                    if (nFlag == 3)
                    {
                        nFlag = 1;
                    }
                }
                else
                {
                    nFlag = 0;
                }
                anResultFlag[i] = nFlag;
            }
            if ((bsSrcString[len - 1] > 0x7f) && (anResultFlag[len - 1] == 1))
            {
                nRealLength = len + 1;
            }
            bsResult = new byte[nRealLength];
            Array.Copy(bsSrcString, bsResult, nRealLength);
            return Encoding.GetEncoding("GB2312").GetString(bsResult);
        }
        #endregion

        #region 判断一个字符串是否为空或全部为空格
        /// <summary>
        /// 判断一个字符串是否为空或全部为空格
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>返回一个值,指示字符串是否为空或全部是空格</returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 生成一个替换连接符的GUID
        /// <summary>
        /// 生成一个替换连接符的GUID
        /// </summary>
        /// <returns>替换连接符后的字符串</returns>
        public static string CreateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 生成一个替换连接符的GUID
        /// </summary>
        /// <param name="conn">连接符</param>
        /// <returns>替换连接符后的字符串</returns>
        public static string CreateGuid(string conn)
        {
            return Guid.NewGuid().ToString().Replace("-", conn ?? string.Empty);
        }

        /// <summary>
        /// 生成一个替换连接符的GUID
        /// </summary>
        /// <param name="conn">连接符</param>
        /// <returns>替换连接符后的字符串</returns>
        public static string CreateGuid(char conn)
        {
            return Guid.NewGuid().ToString().Replace('-', conn);
        }
        #endregion

        #region 对象合并
        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串,列表的每一项用指定的方法生成字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IEnumerable<T> joins)
        {
            return Join(",", joins);
        }

        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IEnumerable<T> joins, JoinAction<T> action)
        {
            return Join(",", joins, action);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,列表的每一项用指定的方法生成字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="joins">要合并的列表</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IEnumerable<T> joins)
        {
            return Join(separator, joins, (JoinAction<T>)null);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="joins">要合并的列表</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IEnumerable<T> joins, JoinAction<T> action)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in joins)
            {
                if (item == null)
                {
                    continue;
                }
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(action != null ? action(item) : item.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IEnumerable<T> joins, string nul)
        {
            return Join(",", joins, nul, null);
        }

        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IEnumerable<T> joins, string nul, JoinAction<T> action)
        {
            return Join(",", joins, nul, action);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="joins">要合并的列表</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IEnumerable<T> joins, string nul)
        {
            return Join(separator, joins, nul, null);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="joins">要合并的列表</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IEnumerable<T> joins, string nul, JoinAction<T> action)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T item in joins)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                if (item == null)
                {
                    sb.Append(action != null ? action(item) : nul);
                }
                else
                {
                    sb.Append(action != null ? action(item) : item.ToString());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IList<T> joins, int startIndex, int length)
        {
            return Join(",", joins, startIndex, length, (JoinAction<T>)null);
        }

        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IList<T> joins, int startIndex, int length, JoinAction<T> action)
        {
            return Join(",", joins, startIndex, length, action);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="joins">要合并的列表</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IList<T> joins, int startIndex, int length)
        {
            return Join(separator, joins, startIndex, length, (JoinAction<T>)null);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="joins">要合并的列表</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IList<T> joins, int startIndex, int length, JoinAction<T> action)
        {
            StringBuilder sb = new StringBuilder();
            if (joins == null || joins.Count == 0)
            {
                return sb.ToString();
            }
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            if (length > 0 && startIndex < joins.Count)
            {
                for (int i = 0; i < length && startIndex < joins.Count; startIndex++, i++)
                {
                    if (joins[startIndex] == null)
                    {
                        continue;
                    }
                    if (sb.Length > 0)
                    {
                        sb.Append(separator);
                    }
                    sb.Append(action != null ? action(joins[startIndex]) : joins[startIndex].ToString());
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IList<T> joins, int startIndex, int length, string nul)
        {
            return Join(",", joins, startIndex, length, nul, null);
        }

        /// <summary>
        /// 把一个列表合并为用逗号连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(IList<T> joins, int startIndex, int length, string nul, JoinAction<T> action)
        {
            return Join(",", joins, startIndex, length, nul, action);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="separator">分隔符</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IList<T> joins, int startIndex, int length, string nul)
        {
            return Join(separator, joins, startIndex, length, nul, null);
        }

        /// <summary>
        /// 把一个列表合并为用指定地分隔符连接地字符串,如果没有指定action,则使用列表中项的"ToString()"方法转换为字符串
        /// </summary>
        /// <param name="joins">要合并的列表</param>
        /// <param name="separator">分隔符</param>
        /// <param name="startIndex">开始位置,如果小于0则为0</param>
        /// <param name="length">长度,如果不大于0则不合并</param>
        /// <param name="nul">如果列表中值为空,使用的字符串</param>
        /// <param name="action">每一项指定的方法生成字符串</param>
        /// <returns>合并后的字符串</returns>
        public static string Join<T>(string separator, IList<T> joins, int startIndex, int length, string nul, JoinAction<T> action)
        {
            StringBuilder sb = new StringBuilder();
            if (joins == null || joins.Count == 0)
            {
                return sb.ToString();
            }
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            if (length > 0 && startIndex < joins.Count)
            {
                for (int i = 0; i < length && startIndex < joins.Count; startIndex++, i++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(separator);
                    }
                    if (joins[startIndex] == null)
                    {
                        sb.Append(action != null ? action(joins[startIndex]) : nul);
                    }
                    else
                    {
                        sb.Append(action != null ? action(joins[startIndex]) : joins[startIndex].ToString());
                    }
                }
            }

            return sb.ToString();
        }

        #endregion 对象合并

        #region 对象拆分
        /// <summary>
        /// 把一个用逗号分隔的数字串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<int> SplitIntNumber(string input)
        {
            List<int> result = new List<int>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.NumberJoin))
            {
                foreach (string tmp in input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(int.Parse(tmp));
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的数字串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<long> SplitLongNumber(string input)
        {
            List<long> result = new List<long>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.NumberJoin))
            {
                foreach (string tmp in input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(long.Parse(tmp));
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的数字串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<int> SplitStrictNumber(string input)
        {
            List<int> result = new List<int>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.StrictNumberJoin))
            {
                foreach (string tmp in input.Split(','))
                {
                    result.Add(int.Parse(tmp));
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个严格用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的字符串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<string> SplitString(string input)
        {
            List<string> result = new List<string>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.StringJoin))
            {
                foreach (string tmp in input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    result.Add(tmp);
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个用逗号分隔的字符串");
            }
            return result;
        }

        /// <summary>
        /// 把一个用逗号分隔的字符串分解成为一个列表
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>合并后的对象</returns>
        public static List<string> SplitStrictString(string input)
        {
            List<string> result = new List<string>();
            if (input == null)
            {
                return result;
            }
            if (ValidateUtils.CheckString(input, ValidateType.StrictStringJoin))
            {
                foreach (string tmp in input.Split(','))
                {
                    result.Add(tmp);
                }
            }
            else
            {
                throw new Exception("原始字符串不是一个严格用逗号分隔的字符串");
            }
            return result;
        }
        #endregion

        #region Unicode编码转换
        /// <summary>
        /// 把字符串转换为以\u开头的Unicode编码
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <returns>转换后的Unicode编码</returns>
        public static string StringToUnicode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                sb.Append(@"\u");
                sb.Append(bytes[i + 1].ToString("x2"));
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把以\u开头的Unicode编码转换为字符串
        /// </summary>
        /// <param name="value">要转换的Unicode码</param>
        /// <returns>转换后的字符串</returns>
        public static string UnicodeToString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            string v = value.Trim();
            if (Regular.IsMatch(@"^(\\u?(\w{4}))*$", v, RegexOptions.IgnoreCase))
            {
                MatchCollection matchs = Regular.Matches(@"\\u?(\w{4})", v, RegexOptions.IgnoreCase);
                StringBuilder sb = new StringBuilder();
                foreach (Match match in matchs)
                {
                    byte[] bytes = new byte[] { (byte)Convert.ToInt32(match.Groups[1].Value.Substring(2), 16), (byte)Convert.ToInt32(match.Groups[1].Value.Substring(0, 2), 16) };
                    sb.Append(Encoding.Unicode.GetString(bytes));
                }
                return sb.ToString();
            }
            throw new Exception("输入的Unicode编码不正确");

        }
        #endregion

        /// <summary>
        /// 获取文件流的编码格式
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <param name="defaultEncoding">如果不能正确获取,返回的默认编码格式</param>
        /// <returns></returns>
        public static System.Text.Encoding GetEncoding(System.IO.Stream fs, Encoding defaultEncoding)
        {
            Encoding result = null;
            if (fs != null && fs.Length >= 2)
            {
                //保存文件流的前4个字节
                byte[] bytes = new byte[4];

                //保存当前Seek位置
                long oriPos = fs.Seek(0, SeekOrigin.Begin);
                fs.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < bytes.Length && i < fs.Length; i++)
                {
                    bytes[i] = Convert.ToByte(fs.ReadByte());
                }
                #region UTF8BOM
                if (fs.Length > 2 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)//UTF8BOM
                {
                    result = Encoding.UTF8;
                    goto lbreturn;
                }
                #endregion

                #region UnicodeBE
                if (fs.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)//UnicodeBE
                {
                    result = Encoding.BigEndianUnicode;
                    goto lbreturn;
                }
                #endregion

                #region Unicode
                if (fs.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)//Unicode
                {
                    if (fs.Length >= 4 && bytes[2] == 0x00 && bytes[3] == 0x00)
                    {
                        return Encoding.UTF32;
                    }
                    else
                    {
                        result = Encoding.Unicode;
                        goto lbreturn;
                    }
                }
                #endregion

                #region UTF-32BE
                if (fs.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF)
                {
                    result = Encoding.GetEncoding("UTF-32BE");
                    goto lbreturn;
                }
                #endregion

            lbreturn: fs.Seek(oriPos, SeekOrigin.Begin);
            }
            //恢复Seek位置
            if (result == null)
            {
                if (fs.Length > 0 && IsUTF8(fs))
                {
                    result = Encoding.UTF8;
                }
            }

            return result ?? defaultEncoding;
        }

        /// <summary>
        /// 过滤危险字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="notFilterChars"></param>
        /// <returns></returns>
        public static string FilterSpecial(string str, List<String> notFilterChars)
        {
            
            if (str == "")
            {
                return str;
            }
            else
            {
                str = filterSpecial(str, "'", notFilterChars);
                str = filterSpecial(str, "<", notFilterChars);
                str = filterSpecial(str, ">", notFilterChars);
                str = filterSpecial(str, "%", notFilterChars);
                str = filterSpecial(str, "'delete", notFilterChars);
                str = filterSpecial(str, "''", notFilterChars);
                str = filterSpecial(str, "\"\"", notFilterChars);
                str = filterSpecial(str, ",", notFilterChars);
                str = filterSpecial(str, ".", notFilterChars);
                str = filterSpecial(str, ">=", notFilterChars);
                str = filterSpecial(str, "=<", notFilterChars);
                str = filterSpecial(str, "-", notFilterChars);
                str = filterSpecial(str, "_", notFilterChars);
                str = filterSpecial(str, ";", notFilterChars);
                str = filterSpecial(str, "||", notFilterChars);
                str = filterSpecial(str, "[", notFilterChars);
                str = filterSpecial(str, "]", notFilterChars);
                str = filterSpecial(str, "&", notFilterChars);
                str = filterSpecial(str, "#", notFilterChars);
                str = filterSpecial(str, "/", notFilterChars);
                str = filterSpecial(str, "-", notFilterChars);
                str = filterSpecial(str, "|", notFilterChars);
                str = filterSpecial(str, "?", notFilterChars);
                str = filterSpecial(str, ">?", notFilterChars);
                str = filterSpecial(str, "?<", notFilterChars);
                str = filterSpecial(str, " ", notFilterChars);
                return str;
            }
        }

        private static String filterSpecial(String str, String specialChar, List<String> notFilterChars)
        {
            if (notFilterChars !=null && !notFilterChars.Contains(specialChar))
            {
                str = str.Replace(specialChar, "");
            } 
            return str;
        } 



        private static bool IsUTF8(System.IO.Stream fs)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.

            long oriPos = fs.Seek(0, SeekOrigin.Begin);
            long streamlength = fs.Length - oriPos;
            for (int i = 0; i < streamlength; i++)
            {
                curByte = Convert.ToByte(fs.ReadByte());
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            fs.Seek(oriPos, SeekOrigin.Begin);
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;

        }


    }

    /// <summary>
    /// 当合并一系统对象为一个字符串时，对每个对象进行的格式化的操作
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">对象</param>
    /// <returns>对象格式化后的字符串</returns>
    public delegate string JoinAction<T>(T obj);

    #region 简繁转换
    /// <summary>
    /// 简繁转换方向
    /// </summary>
    public enum ChineseConversionDirection
    {
        /// <summary>
        /// 简体到繁体转换
        /// </summary>
        SimplifiedToTraditional,

        /// <summary>
        /// 繁体到简体转换
        /// </summary>
        TraditionalToSimplified
    }

    public static class NativeMethods
    {
        public const uint LCMAP_SIMPLIFIED_CHINESE = 0x2000000;
        public const uint LCMAP_TRADITIONAL_CHINESE = 0x4000000;
        public const int zh_CN = 0x804;

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("KERNEL32.DLL", SetLastError = true)]
        public static extern bool FreeLibrary(HandleRef hModule);
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode)]
        public static extern int LCMapString(int Locale, uint dwMapFlags, [MarshalAs(UnmanagedType.LPTStr)] string lpSrcStr, int cchSrc, IntPtr lpDestStr, int cchDest);
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpFileName);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("MSTR2TSC.DLL", CharSet = CharSet.Unicode)]
        public static extern bool TCSCConvertText([MarshalAs(UnmanagedType.LPTStr)] string pwszInput, int cchInput, out IntPtr ppwszOutput, out int pcchOutput, ChineseConversionDirection dwDirection, [MarshalAs(UnmanagedType.Bool)] bool fCharBase, [MarshalAs(UnmanagedType.Bool)] bool fLocalTerm);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("MSTR2TSC.DLL")]
        public static extern bool TCSCFreeConvertedText(IntPtr pv);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("MSTR2TSC.DLL")]
        public static extern bool TCSCInitialize();
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("MSTR2TSC.DLL")]
        public static extern bool TCSCUninitialize();
    }

    public class OfficeConversionEngine
    {
        private static string MSOPath;
        private static string Mstr2tscPath;

        static OfficeConversionEngine()
        {
            string str = null;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office\12.0\Common\InstallRoot");
            if (key != null)
            {
                str = Convert.ToString(key.GetValue("Path"), null);
            }
            RegistryKey key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office\12.0\Common\FilesPaths");
            if (key2 != null)
            {
                MSOPath = Convert.ToString(key2.GetValue("mso.dll"), null);
            }
            if (!string.IsNullOrEmpty(str))
            {
                Mstr2tscPath = Path.Combine(str, @"ADDINS\MSTR2TSC.DLL");
            }
            if (string.IsNullOrEmpty(Mstr2tscPath) || !File.Exists(Mstr2tscPath))
            {
                Mstr2tscPath = null;
            }
        }

        private OfficeConversionEngine()
        {
        }

        public static OfficeConversionEngine Create()
        {
            if (!string.IsNullOrEmpty(MSOPath) && !string.IsNullOrEmpty(Mstr2tscPath))
            {
                return new OfficeConversionEngine();
            }
            return null;
        }

        public string TCSCConvert(string input, ChineseConversionDirection direction)
        {
            string str2;
            IntPtr zero = IntPtr.Zero;
            IntPtr handle = IntPtr.Zero;
            try
            {
                IntPtr ptr3;
                int num;
                zero = NativeMethods.LoadLibrary(MSOPath);
                handle = NativeMethods.LoadLibrary(Mstr2tscPath);
                if (!NativeMethods.TCSCInitialize())
                {
                    return null;
                }
                string str = null;
                if (NativeMethods.TCSCConvertText(input, input.Length, out ptr3, out num, direction, false, true))
                {
                    str = Marshal.PtrToStringUni(ptr3);
                    NativeMethods.TCSCFreeConvertedText(ptr3);
                }
                NativeMethods.TCSCUninitialize();
                str2 = str;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(new HandleRef(this, handle));
                }
                if (zero != IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(new HandleRef(this, zero));
                }
            }
            return str2;
        }
    }
    #endregion
}
