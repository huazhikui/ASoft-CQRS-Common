using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft
{
    /// <summary>
    /// �������������
    /// </summary>
    public static class Rand
    {
        static readonly char[] cpattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        static readonly char[] spattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        /// <summary>
        /// �õ�����Ϊlength���ַ���,���lengthС�ڵ���0,length=6
        /// </summary>
        /// <param name="length">Ҫ�õ����ַ����ĳ���</param>
        /// <returns>�������ɵ��ַ���</returns>
        public static string Number(int length)
        {
            return Number(length, false);
        }

        /// <summary>
        /// �õ�����Ϊlength���ַ���,���lengthС�ڵ���0,length=6
        /// </summary>
        /// <param name="length">Ҫ�õ����ַ����ĳ���</param>
        /// <param name="sleep">�Ƿ�Ҫ������ǰ����ǰ�߳���ֹ�Ա����ظ�</param>
        /// <returns>�������ɵ��ַ���</returns>

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
        /// �õ�����Ϊlength���ַ���,���lengthС�ڵ���0,length=6
        /// </summary>
        /// <param name="length">Ҫ�õ����ַ����ĳ���</param>
        /// <returns>�������ɵ��ַ���</returns>
        public static string Chars(int length)
        {
            return Chars(length, false);
        }

        /// <summary>
        /// �õ�����Ϊlength���ַ���,���lengthС�ڵ���0,length=6
        /// </summary>
        /// <param name="length">Ҫ�õ����ַ����ĳ���</param>
        /// <param name="sleep">�Ƿ�Ҫ������ǰ����ǰ�߳���ֹ�Ա����ظ�</param>
        /// <returns>�������ɵ��ַ���</returns>
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
        /// �õ�����Ϊlength���ַ���,���lengthС�ڵ���0,length=6
        /// </summary>
        /// <param name="length">Ҫ�õ����ַ����ĳ���</param>
        /// <returns>�������ɵ��ַ���</returns>
        public static string NumberChar(int length)
        {
            return NumberChar(length, false);
        }

        /// <summary>
        /// �õ�����Ϊlength���ַ���,���lengthС�ڵ���0,length=6
        /// </summary>
        /// <param name="length">Ҫ�õ����ַ����ĳ���</param>
        /// <param name="sleep">�Ƿ�Ҫ������ǰ����ǰ�߳���ֹ�Ա����ظ�</param>
        /// <returns>�������ɵ��ַ���</returns>
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
        /// ��ȡһ������Ĳ���ֵ
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
        /// ���ص�ǰϵͳʵ�ʵĿ̶�,ÿ�η���ʱ�߳���ͣ1΢��
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
