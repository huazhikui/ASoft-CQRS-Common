using System;
using System.Text.RegularExpressions;

namespace ASoft
{
    /// <summary>
    /// �ṩ������֤����.
    /// </summary>
    public static class Regular
    {
        /// <summary>
        /// ����Regex��IsMatch����ʵ��һ���������ʽƥ��
        /// </summary>
        /// <param name="pattern">Ҫƥ���������ʽģʽ��</param>
        /// <param name="input">Ҫ����ƥ������ַ���</param>
        /// <returns>���������ʽ�ҵ�ƥ�����Ϊ true������Ϊ false��</returns>
        public static bool IsMatch(string pattern, string input)
        {
            return Regex.IsMatch(input, pattern);
        }


        /// <summary>
        /// ����Regex��IsMatch����ʵ��һ���������ʽƥ��
        /// </summary>
        /// <param name="pattern">Ҫƥ���������ʽģʽ��</param>
        /// <param name="input">Ҫ����ƥ������ַ���</param>
        /// <param name="options">������ʽѡ��</param>
        /// <returns>���������ʽ�ҵ�ƥ�����Ϊ true������Ϊ false��</returns>
        public static bool IsMatch(string pattern, string input, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        /// <summary>
        /// �������ַ����еĵ�һ���ַ���ʼ�����滻�ַ����滻ָ����������ʽģʽ������ƥ���
        /// </summary>
        /// <param name="pattern">ģʽ�ַ���</param>
        /// <param name="input">�����ַ���</param>
        /// <param name="replacement">�����滻���ַ���</param>
        /// <returns>���ر��滻��Ľ��</returns>
        public static string Replace(string pattern, string input, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        /// <summary>
        /// �������ַ����еĵ�һ���ַ���ʼ�����滻�ַ����滻ָ����������ʽģʽ������ƥ���
        /// </summary>
        /// <param name="pattern">ģʽ�ַ���</param>
        /// <param name="input">�����ַ���</param>
        /// <param name="evaluator">�ҵ�ÿ��������ʽʱ���õķ���</param>
        /// <param name="options">������ʽ�ҵ�ƥ����</param>
        /// <returns>���ر��滻��Ľ��</returns>
        public static string Replace(string input, string pattern, MatchEvaluator evaluator, RegexOptions options)
        {
            return Regex.Replace(input, pattern, evaluator, options);
        }

        /// <summary>
        /// �������ַ����еĵ�һ���ַ���ʼ�����滻�ַ����滻ָ����������ʽģʽ������ƥ���
        /// </summary>
        /// <param name="pattern">ģʽ�ַ���</param>
        /// <param name="input">�����ַ���</param>
        /// <param name="replacement">�����滻���ַ���</param>
        /// <param name="options">������ʽ�ҵ�ƥ����</param>
        /// <returns>���ر��滻��Ľ��</returns>
        public static string Replace(string input, string pattern, string replacement, RegexOptions options)
        {
            return Regex.Replace(input, pattern, replacement, options);
        }

        /// <summary>
        /// ����������ʽģʽ�����λ�ò�������ַ�����
        /// </summary>
        /// <param name="pattern">ģʽ�ַ���</param>
        /// <param name="input">�����ַ���</param>
        /// <param name="options">������ʽѡ��</param>
        /// <returns>�ָ����ַ�������</returns>
        public static string[] Split(string pattern, string input, RegexOptions options)
        {
            return Regex.Split(input, pattern, options);
        }

        /// <summary>
        /// ��ȡƥ���ַ���
        /// </summary>
        /// <param name="pattern">ģʽ�ַ���</param>
        /// <param name="input">�����ַ���</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static MatchCollection Matches(string pattern, string input, RegexOptions options)
        {
            return Regex.Matches(input, pattern, options);
        }
    }
}
