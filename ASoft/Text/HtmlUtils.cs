using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ASoft.Text
{
    /// <summary>
    /// HTML数据处理类
    /// </summary>
    public static class HtmlUtils
    {
        #region HTML相关字符串处理

        /// <summary>
        /// 替换字符串中的换行符(\r\n)为html换行符(<br />)
        /// </summary>
        /// <param name="html">原始字符串</param>
        public static string HtmlStringFormat(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace("\r\n", "<br />").Replace("\n", "<br />");
            }
            return html;
        }

        /// <summary>
        /// 替换html字符串中的单引号，分号，小于号，大于号，空格
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>替换后的字符串</returns>
        public static string EncodeHtml(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Replace(",", "&def").Replace("'", "&dot").Replace(";", "&dec").Replace("<", "&lt").Replace(">", "&gt").Replace(" ", "&nbsp");
            }
            return content;
        }

        /// <summary>
        /// 为脚本替换特殊字符串
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ReplaceStrToScript(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
            }
            return content;
        }

        /// <summary>
        /// 移除script标签
        /// </summary>
        /// <param name="content">原始字符串</param>
        /// <returns>移除script标签后的字符串</returns>
        public static string RemoveScriptTag(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string regexstr = @"<script[^>]*?>.*?</script>";
                content = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            }
            return content;
        }

        /// <summary>
        /// 移除所有Html标签
        /// </summary>
        /// <param name="content">原始字符串</param>
        /// <returns>移除Html标签后的字符串</returns>
        public static string RemoveHtmlTag(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string regexstr = @"<[^>]*>";
                content = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            }
            return content;
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtmlTag(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            }
            return content;
        }

        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                content = regEx.Replace(content, "");
            }
            return content;
        }

        /// <summary>
        /// 智能格式化文本\判断文本还是HTML文本
        /// </summary>
        /// <param name="objString">要格式化排版的对象</param>
        /// <returns>如果是文本则输出文本的相应HTML格式，若为HTML格式则不变。</returns>
        public static string SmartFormat(object objString)
        {
            if (objString == null) return string.Empty;
            string strGet = objString.ToString();
            string strPattern = "<.[^<]*>";
            if (!Regex.IsMatch(strGet, strPattern, RegexOptions.IgnoreCase))
            {
                strGet = Text2Html(strGet);
            }
            return strGet;
        }

        /// <summary>
        /// 普通文本转换为HTML格式
        /// </summary>
        /// <param name="content">转换文本对象</param>
        /// <returns>格式化的HTML格式</returns>
        public static string Text2Html(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                // xml escape
                content = content.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&#39;").Replace("\"", "&#34;");
                // Simple Transfer
                content = content.Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />").Replace(" ", "&nbsp;");
            }
            return content;
        }

        /// <summary>
        /// HTML格式文本转换为普通文本
        /// </summary>
        /// <param name="content">转换HTML文本对象</param>
        /// <returns>格式化的普通文本</returns>
        public static string Html2Text(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                // &nbsp; <p> <br>
                content = Regex.Replace(content, "&nbsp;?", " ", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, "<p\\s?[^>]*>", "\n\n", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, "<BR\\s?/?>", "\n", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, "</p>", "", RegexOptions.IgnoreCase);
                // Other < > wrapped
                content = Regex.Replace(content, "<.[^<]*>", "", RegexOptions.IgnoreCase);
                // xml unescape
                content = content.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&#39;", "'").Replace("&#34;", "\"").Replace("&amp;", "&");
            }
            return content;
        }

        /// <summary>
        /// 提取参数提供的字符串的超级链接,FTP,Mail等信息并加上相关属性
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AddLink(string content)
        {
            if (content == null || content.Trim().Length == 0)
            {
                return string.Empty;
            }
            content = ASoft.Regular.Replace(@"(http:\/\/([\w.]+\/?)\S*)", content, "<a href='$1' target='_blank'>$1</a>");
            content = ASoft.Regular.Replace(@"(ftp:\/\/([\w.]+\/?)\S*)", content, "<a href='$1' target='_blank'>$1</a>");
            content = ASoft.Regular.Replace("([a-z0-9_A-Z\\-\\.]{1,20})@([a-z0-9_\\-]{1,15})\\.([a-z]{2,4})", content, "<a href='mailto:$1@$2.$3'  target='_blank'>$1@$2.$3</a>");
            return content;
        }
        #endregion
    }
}
