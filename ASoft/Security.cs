using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace ASoft
{
    /// <summary>
    /// �����ü��ܣ����ܼ�����ת����
    /// </summary>
    public static class Security
    {
        static byte[] defaultDESIV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        static byte[] defaultRC2IV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        static byte[] defaultRijndaelIV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        static byte[] defaultTDESIV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        static UInt32[] crcTable = {   
             0x0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,
             0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
             0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
             0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
             0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
             0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
             0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
             0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
             0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433,
             0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01,
             0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,
             0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
             0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,
             0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
             0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
             0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
             0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683,
             0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1,
             0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,
             0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
             0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
             0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
             0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,
             0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
             0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713,
             0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21,
             0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
             0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
             0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,
             0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
             0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,
             0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d,
            };


        #region ����ת��

        /// <summary>
        ///  ���ַ�ת��ΪHTML����
        /// </summary>
        /// <param name="input">��ת�����ַ���</param>
        /// <returns>ת�����HTML����</returns>
        public static string HtmlEncode(string input)
        {
            return System.Web.HttpUtility.HtmlEncode(input ?? string.Empty);
        }

        /// <summary>
        /// ��HTML����ת��Ϊ�ַ���
        /// </summary>
        /// <param name="input">��ת�����ַ���</param>
        /// <returns>ת������ַ���</returns>
        public static string HtmlDecode(string input)
        {
            return System.Web.HttpUtility.HtmlDecode(input ?? string.Empty);
        }

        /// <summary>
        /// URL��ַ����
        /// </summary>
        /// <param name="input">��ת�����ַ���</param>
        /// <returns>ת������ַ���</returns>
        public static string URLEncode(string input)
        {
            return System.Web.HttpUtility.UrlEncode(input ?? string.Empty);
        }

        /// <summary>
        /// URL��ַ����
        /// </summary>
        /// <param name="input">��ת�����ַ���</param>
        /// <returns>ת������ַ���</returns>
        public static string URLDecode(string input)
        {
            return System.Web.HttpUtility.UrlDecode(input ?? string.Empty);
        }

        /// <summary>
        /// �Դ�����ַ�����������javascript:escape����
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�������ַ���</returns>
        public static string UrlEncodeUnicode(string input)
        {
            return System.Web.HttpUtility.UrlEncodeUnicode(input);
        }

        /// <summary>
        /// �Դ�����ַ�����������javascript:unescape����
        /// </summary>
        /// <param name="input"></param>
        /// <returns>�������ַ���</returns>
        public static string UrlDecodeUnicode(string input)
        {
            return System.Web.HttpUtility.UrlDecode(input ?? string.Empty);
        }

        /// <summary>
        /// �õ�Base64����
        /// </summary>
        /// <param name="input">Ҫ�����ַ���</param>
        /// <param name="encoding">����</param>
        /// <returns>���ر������ַ���</returns>
        public static string Base64Encode(string input, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(input));
        }

        /// <summary>
        /// �õ�Base64����
        /// </summary>
        /// <param name="input">Ҫ�����ֽ�����</param>
        /// <returns>���ر������ַ���</returns>
        public static string Base64Encode(byte[] input)
        {
            return Convert.ToBase64String(input, 0, input.Length, Base64FormattingOptions.None);
        }

        /// <summary>
        /// �õ�Base64����
        /// </summary>
        /// <param name="input">Ҫ�����ֽ�����</param>
        /// <param name="options">��ʽ������</param>
        /// <returns>���ر������ַ���</returns>
        public static string Base64Encode(byte[] input, Base64FormattingOptions options)
        {
            return Convert.ToBase64String(input, 0, input.Length, options);
        }

        /// <summary>
        /// �õ�Base64����
        /// </summary>
        /// <param name="input">Ҫ�����ֽ�����</param>
        /// <param name="offset">ƫ��</param>
        /// <param name="length">����</param>
        /// <returns>���ر������ַ���</returns>
        public static string Base64Encode(byte[] input, int offset, int length)
        {
            return Convert.ToBase64String(input, offset, length, Base64FormattingOptions.None);
        }

        /// <summary>
        /// �õ�Base64����
        /// </summary>
        /// <param name="input">Ҫ�����ֽ�����</param>
        /// <param name="offset">ƫ��</param>
        /// <param name="length">����</param>
        /// <param name="options">��ʽ������</param>
        /// <returns>���ر������ַ���</returns>
        public static string Base64Encode(byte[] input, int offset, int length, Base64FormattingOptions options)
        {
            return Convert.ToBase64String(input, offset, length, options);
        }

        /// <summary>
        /// ��һ����ʹ��Base64�������뵽һ��StringBuilder��
        /// </summary>
        /// <param name="sin">��</param>
        /// <returns></returns>
        public static StringBuilder Base64Encode(Stream sin)
        {
            return Base64Encode(sin, 0, sin.Length, Base64FormattingOptions.None);
        }

        /// <summary>
        /// ��һ����ʹ��Base64�������뵽һ��StringBuilder��
        /// </summary>
        /// <param name="sin">��</param>
        /// <param name="offset">��ʼλ��</param>
        /// <param name="length">����</param>
        /// <returns></returns>
        public static StringBuilder Base64Encode(Stream sin, long offset, long length)
        {
            return Base64Encode(sin, offset, length, Base64FormattingOptions.None);
        }

        /// <summary>
        /// ��һ����ʹ��Base64�������뵽һ��StringBuilder��
        /// </summary>
        /// <param name="sin">��</param>
        /// <param name="options">��ʽ������</param>
        /// <returns></returns>
        public static StringBuilder Base64Encode(Stream sin, Base64FormattingOptions options)
        {
            return Base64Encode(sin, 0, sin.Length, options);
        }

        /// <summary>
        /// ��һ����ʹ��Base64�������뵽һ��StringBuilder��
        /// </summary>
        /// <param name="sin">��</param>
        /// <param name="offset">��ʼλ��</param>
        /// <param name="length">����</param>
        /// <param name="options">��ʽ������</param>
        /// <returns></returns>
        public static StringBuilder Base64Encode(Stream sin, long offset, long length, Base64FormattingOptions options)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "����Ӧ��Ϊ����0�����0������");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "��ʼλ��Ӧ��Ϊ���ڻ����0������");
            }
            if (offset > (sin.Length - length))
            {
                throw new ArgumentOutOfRangeException("offset", "�ӿ�ʼλ�û�ȡ�ĳ��ȳ������ĳ���");
            }

            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[600];
            sin.Position = offset;
            int currentlen = currentlen = sin.Read(buffer, 0, buffer.Length); ;
            int totallen = currentlen;

            while (totallen < length)
            {
                sb.Append(Base64Encode(buffer, options));
                currentlen = currentlen = sin.Read(buffer, 0, buffer.Length); ;
                totallen += currentlen;
            }
            sb.Append(Base64Encode(buffer, 0, currentlen, options));
            return sb;

        }

        /// <summary>
        /// �õ�Base64����
        /// </summary>
        /// <param name="input">Ҫ�����ַ���</param>
        /// <returns>���ؽ������ֽ�����</returns>
        public static byte[] Base64Decode(string input)
        {
            return Convert.FromBase64String(input);
        }

        /// <summary>
        /// �õ�Base64����
        /// </summary>
        /// <param name="input">Ҫ�����ַ���</param>
        /// <param name="encoding">����</param>
        /// <returns>���ؽ������ֽ�����</returns>
        public static string Base64Decode(string input, Encoding encoding)
        {
            return encoding.GetString(Convert.FromBase64String(input));
        }

        /// <summary>
        /// ��һ���ַ�������ʹ��Base64�����,���뵽һ������
        /// </summary>
        /// <param name="input">Ҫ����Ķ���</param>
        /// <param name="sout">�����</param>
        public static void Base64Decode(string input, Stream sout)
        {
            int count = 400;
            int total = count;
            string temp = null;
            byte[] buffer = null;
            while (total < input.Length)
            {
                temp = input.Substring(total - count, count);
                buffer = Base64Decode(temp);
                sout.Write(buffer, 0, buffer.Length);
                total += count;
            }
            temp = input.Substring(total - count);
            buffer = Base64Decode(temp);
            sout.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// �ַ�������ת��
        /// </summary>
        /// <param name="input">ԭʼ�ַ���</param>
        /// <param name="srcEncoding">ԭʼ�ַ����ı���</param>
        /// <param name="dstEncoding">Ŀ���ַ����ı���</param>
        /// <returns></returns>
        public static string ConvertEncoding(string input, System.Text.Encoding srcEncoding, System.Text.Encoding dstEncoding)
        {
            return dstEncoding.GetString(System.Text.Encoding.Convert(srcEncoding, dstEncoding, srcEncoding.GetBytes(input)));
        }

        /// <summary>
        /// ���ֽ�����ת��Ϊ��ֵ 
        /// </summary>
        /// <param name="input">Ҫת�����ֽ�����</param>
        /// <param name="offset">��ʼλ��</param>
        /// <returns></returns>
        public static int ConvertBytesToInt(byte[] input, int offset)
        {
            return BitConverter.ToInt32(input, offset);
        }

        /// <summary>
        /// ����ֵת��Ϊ�ֽ����� 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ConvertIntToBytes(int input)
        {
            byte[] ret = BitConverter.GetBytes(input);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Array.Reverse(ret);
            }
            return ret;
        }

        /// <summary>
        /// ���ֽ�����ת��Ϊ16�����ַ���
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertBytesToHex(byte[] input)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in input)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// �ַ���ת��Ϊ�ֽ�����
        /// </summary>
        /// <param name="input">ԭʼ�ַ���</param>
        /// <returns>ת������ֽ�����</returns>
        public static byte[] ConvertHexToBytes(string input)
        {
            int len = input.Length / 2;
            byte[] ret = new byte[len];
            for (int i = 0; i < len; i++)
                ret[i] = System.Convert.ToByte(input.Substring(i * 2, 2), 16);
            return ret;

        }
        #endregion

        #region ����Hashֵ

        #region MD5

        /// <summary>
        /// �����ַ�����MD5ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�ַ�����MD5ֵ</returns>
        public static string MD5Compute(string input)
        {
            return BitConverter.ToString(HashCompute(System.Text.Encoding.UTF8.GetBytes(input), "MD5")).Replace("-", "");
        }

        /// <summary>
        /// �����ַ�����MD5ֵ(16λ)
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <param name="encode">���뷽ʽ</param>
        /// <returns>�ַ�����MD5ֵ</returns>
        public static string MD5Compute(string input, System.Text.Encoding encode)
        {
            return BitConverter.ToString(HashCompute(encode.GetBytes(input), "MD5")).Replace("-", "");
        }

        /// <summary>
        /// �����ֽ������MD5ֵ
        /// </summary>
        /// <param name="input">Ҫ������ֽ�����</param>
        /// <returns>�ֽ������MD5ֵ</returns>
        public static byte[] MD5Compute(byte[] input)
        {
            return HashCompute(input, "MD5");
        }

        /// <summary>
        /// ��������MD5ֵ
        /// </summary>
        /// <param name="input">Ҫ�������</param>
        /// <returns>����MD5ֵ</returns>
        public static byte[] MD5Compute(Stream input)
        {
            return HashCompute(input, "MD5");
        }
        #endregion

        #region SHA1
        /// <summary>
        /// �����ַ�����SHA1ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�ַ�����SHA1ֵ</returns>
        public static string SHA1Compute(string input)
        {
            return BitConverter.ToString(HashCompute(System.Text.Encoding.UTF8.GetBytes(input), "SHA1")).Replace("-", "");
        }

        /// <summary>
        /// �����ַ�����SHA1ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <param name="encode">���뷽ʽ</param>
        /// <returns>�ַ�����SHA1ֵ</returns>
        public static string SHA1Compute(string input, System.Text.Encoding encode)
        {
            return BitConverter.ToString(HashCompute(encode.GetBytes(input), "SHA1")).Replace("-", "");
        }

        /// <summary>
        /// �����ֽ������SHA1ֵ
        /// </summary>
        /// <param name="input">Ҫ������ֽ�����</param>
        /// <returns>�ֽ������SHA1ֵ</returns>
        public static byte[] SHA1Compute(byte[] input)
        {
            return HashCompute(input, "SHA1");
        }

        /// <summary>
        /// ��������SHA1ֵ(16λ)
        /// </summary>
        /// <param name="input">Ҫ�������</param>
        /// <returns>����SHA1ֵ</returns>
        public static byte[] SHA1Compute(Stream input)
        {
            return HashCompute(input, "SHA1");
        }
        #endregion

        #region RIPEMD160
        /// <summary>
        /// �����ַ�����RIPEMD160ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�ַ�����RIPEMD160ֵ</returns>
        public static string RIPEMD160Compute(string input)
        {
            return BitConverter.ToString(HashCompute(System.Text.Encoding.UTF8.GetBytes(input), "RIPEMD160")).Replace("-", "");
        }

        /// <summary>
        /// �����ַ�����RIPEMD160ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <param name="encode">���뷽ʽ</param>
        /// <returns>�ַ�����RIPEMD160ֵ</returns>
        public static string RIPEMD160Compute(string input, System.Text.Encoding encode)
        {
            return BitConverter.ToString(HashCompute(encode.GetBytes(input), "RIPEMD160")).Replace("-", "");
        }


        /// <summary>
        /// �����ֽ������RIPEMD160ֵ
        /// </summary>
        /// <param name="input">Ҫ������ֽ�����</param>
        /// <returns>�ֽ������RIPEMD160ֵ</returns>
        public static byte[] RIPEMD160Compute(byte[] input)
        {
            return HashCompute(input, "RIPEMD160");
        }

        /// <summary>
        /// ��������RIPEMD160ֵ
        /// </summary>
        /// <param name="input">Ҫ�������</param>
        /// <returns>����RIPEMD160ֵ</returns>
        public static byte[] RIPEMD160Compute(Stream input)
        {
            return HashCompute(input, "RIPEMD160");
        }
        #endregion

        #region SHA256
        /// <summary>
        /// �����ַ�����SHA256ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�ַ�����SHA256ֵ</returns>
        public static string SHA256Compute(string input)
        {
            return BitConverter.ToString(HashCompute(System.Text.Encoding.UTF8.GetBytes(input), "SHA256")).Replace("-", "");
        }

        /// <summary>
        /// �����ַ�����SHA256ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <param name="encode">���뷽ʽ</param>
        /// <returns>�ַ�����SHA256ֵ</returns>
        public static string SHA256Compute(string input, System.Text.Encoding encode)
        {
            return BitConverter.ToString(HashCompute(encode.GetBytes(input), "SHA256")).Replace("-", "");
        }

        /// <summary>
        /// �����ֽ������SHA256ֵ
        /// </summary>
        /// <param name="input">Ҫ������ֽ�����</param>
        /// <returns>�ֽ������SHA256ֵ</returns>
        public static byte[] SHA256Compute(byte[] input)
        {
            return HashCompute(input, "SHA256");
        }

        /// <summary>
        /// ��������SHA256ֵ
        /// </summary>
        /// <param name="input">Ҫ�������</param>
        /// <returns>����SHA256ֵ</returns>
        public static byte[] SHA256Compute(Stream input)
        {
            return HashCompute(input, "SHA256");
        }
        #endregion

        #region SHA384
        /// <summary>
        /// �����ַ�����SHA384ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�ַ�����SHA384ֵ</returns>
        public static string SHA384Compute(string input)
        {
            return BitConverter.ToString(HashCompute(System.Text.Encoding.UTF8.GetBytes(input), "SHA384")).Replace("-", "");
        }

        /// <summary>
        /// �����ַ�����SHA384ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <param name="encode">���뷽ʽ</param>
        /// <returns>�ַ�����SHA384ֵ</returns>
        public static string SHA384Compute(string input, System.Text.Encoding encode)
        {
            return BitConverter.ToString(HashCompute(encode.GetBytes(input), "SHA384")).Replace("-", "");
        }

        /// <summary>
        /// �����ֽ������SHA384ֵ
        /// </summary>
        /// <param name="input">Ҫ������ֽ�����</param>
        /// <returns>�ֽ������SHA384ֵ</returns>
        public static byte[] SHA384Compute(byte[] input)
        {
            return HashCompute(input, "SHA384");
        }

        /// <summary>
        /// ��������SHA384ֵ
        /// </summary>
        /// <param name="input">Ҫ�������</param>
        /// <returns>����SHA384ֵ</returns>
        public static byte[] SHA384Compute(Stream input)
        {
            return HashCompute(input, "SHA384");
        }
        #endregion

        #region SHA512
        /// <summary>
        /// �����ַ�����SHA512ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�ַ�����SHA512ֵ</returns>
        public static string SHA512Compute(string input)
        {
            return BitConverter.ToString(HashCompute(System.Text.Encoding.UTF8.GetBytes(input), "SHA512")).Replace("-", "");
        }

        /// <summary>
        /// �����ַ�����SHA512ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <param name="encode">���뷽ʽ</param>
        /// <returns>�ַ�����SHA512ֵ</returns>
        public static string SHA512Compute(string input, System.Text.Encoding encode)
        {
            return BitConverter.ToString(HashCompute(encode.GetBytes(input), "SHA512")).Replace("-", "");
        }

        /// <summary>
        /// �����ֽ������SHA512ֵ
        /// </summary>
        /// <param name="input">Ҫ������ֽ�����</param>
        /// <returns>�ֽ������SHA512ֵ</returns>
        public static byte[] SHA512Compute(byte[] input)
        {
            return HashCompute(input, "SHA512");
        }

        /// <summary>
        /// ��������SHA512ֵ
        /// </summary>
        /// <param name="input">Ҫ�������</param>
        /// <returns>����SHA512ֵ</returns>
        public static byte[] SHA512Compute(Stream input)
        {
            return HashCompute(input, "SHA512");
        }
        #endregion

        #region  CRC32
        /// <summary>
        /// �����ַ�����CRC32ֵ
        /// </summary>
        /// <param name="input">Ҫ������ַ���</param>
        /// <returns>�ַ�����CRC32ֵ</returns>
        public static string CRC32Compute(string input)
        {
            return CRC32Compute(System.Text.Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// �����ֽ������CRC32ֵ
        /// </summary>
        /// <param name="input">Ҫ������ֽ�����</param>
        /// <returns>�ֽ������CRC32ֵ</returns>
        public static string CRC32Compute(byte[] input)
        {
            return CRC32Compute(new MemoryStream(input));
        }

        /// <summary>
        /// ��������CRC32ֵ
        /// </summary>
        /// <param name="input">Ҫ�������</param>
        /// <returns>����CRC32ֵ</returns>
        public static string CRC32Compute(Stream input)
        {
            UInt32 crc = 0xFFFFFFFF;
            for (int i = 0; i < input.Length; i++)
            {
                crc = ((crc >> 8) & 0x00FFFFFF) ^ crcTable[(crc ^ input.ReadByte()) & 0xFF];
            }
            UInt32 temp = crc ^ 0xFFFFFFFF;
            return Convert.ToString(((int)temp), 16);
        }

        #endregion

        #region ˽�з���
        private static byte[] HashCompute(byte[] input, string type)
        {
            HashAlgorithm ha = HashAlgorithm.Create(type);
            byte[] tmp = ha.ComputeHash(input);
            ha.Clear();
            return tmp;
        }
        private static byte[] HashCompute(Stream input, string type)
        {
            HashAlgorithm ha = HashAlgorithm.Create(type);
            byte[] tmp = ha.ComputeHash(input);
            ha.Clear();
            return tmp;
        }

        #endregion
        #endregion

        #region �ԳƼ���

        #region DES���ܽ���
        /// <summary>
        /// DES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        public static void DesEncrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            byte[] realkey;
            if (key == null || key.Length != 8)
            {
                if (key == null)
                {
                    throw new Exception("û��ָ����Կ");
                }
                else
                {
                    realkey = new byte[8];
                    for (int i = 0; i < 8 && i < key.Length; i++)
                    {
                        realkey[i] = key[i];
                    }
                    for (int i = key.Length; i < 8; i++)
                    {
                        realkey[i] = (byte)(i * i);
                    }
                }
            }
            else
            {
                realkey = key;
            }
            SymmetricEncrypt(input, realkey, iv ?? defaultDESIV, "DES", mode, padding, output);
        }

        /// <summary>
        /// DES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        public static void DesDecrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            byte[] realkey;
            if (key == null || key.Length != 8)
            {
                if (key == null)
                {
                    throw new Exception("û��ָ����Կ");
                }
                else
                {
                    realkey = new byte[8];
                    for (int i = 0; i < 8 && i < key.Length; i++)
                    {
                        realkey[i] = key[i];
                    }
                    for (int i = key.Length; i < 8; i++)
                    {
                        realkey[i] = (byte)(i * i);
                    }
                }
            }
            else
            {
                realkey = key;
            }
            SymmetricDecrypt(input, realkey, iv ?? defaultDESIV, "DES", mode, padding, output);
        }

        /// <summary>
        /// ʹ��Ĭ�ϵļ��ܷ�ʽ����DES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <returns>���ܺ������</returns>
        public static string DesEncrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
            MemoryStream outms = new MemoryStream();
            DesEncrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return Convert.ToBase64String(outms.ToArray());
        }

        public static byte[] DesEncrypt(byte[] input, byte[] key)
        {
            MemoryStream inms = new MemoryStream(input);
            MemoryStream outms = new MemoryStream();
            DesEncrypt(inms, key, null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return outms.ToArray();
        }

        public static byte[] DesEncrypt(byte[] input, byte[] key, byte[] iv)
        {
            MemoryStream inms = new MemoryStream(input);
            MemoryStream outms = new MemoryStream();
            DesEncrypt(inms, key, iv, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return outms.ToArray();
        }

     

        /// <summary>
        /// ʹ��Ĭ�ϵļ��ܷ�ʽ����DES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param> 
        /// <returns>���ܺ������</returns>
        public static String DesEncrypt(String input, byte[] key, byte[] iv)
        {
            MemoryStream inms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
            MemoryStream outms = new MemoryStream();
            DesEncrypt(inms, key, iv, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return Convert.ToBase64String(outms.ToArray());
        }

        /// <summary>
        /// ʹ��Ĭ�ϵļ��ܷ�ʽ����DES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <returns>���ܺ������</returns>
        public static string DesDecrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(Convert.FromBase64String(input));
            MemoryStream outms = new MemoryStream();
            DesDecrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return System.Text.Encoding.UTF8.GetString(outms.ToArray());
        }

        /// <summary>
        /// ʹ��Ĭ�ϵļ��ܷ�ʽ����DES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <returns>���ܺ������</returns>
        public static String DesDecrypt(String input, byte[] key, byte[] iv)
        {
            MemoryStream inms = new MemoryStream(Convert.FromBase64String(input));
            MemoryStream outms = new MemoryStream();
            DesDecrypt(inms, key, iv, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return System.Text.Encoding.UTF8.GetString(outms.ToArray());
        }

        #endregion

        #region 3DES���ܽ���
        /// <summary>
        /// 3DES����
        /// </summary>
        /// <param name="data">����ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������ Ĭ�Ͽ�,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">Ĭ��ECB</param>
        /// <param name="padding">Ĭ��PKCS7</param>
        /// <returns>���ܺ������</returns>
        public static byte[] DES3Encrypt(byte[] data, 
            byte[] key, 
            byte[] iv=null, 
            CipherMode mode = System.Security.Cryptography.CipherMode.ECB, 
            PaddingMode padding = System.Security.Cryptography.PaddingMode.PKCS7)
        {
            ICryptoTransform ct;
            MemoryStream mStream;
            CryptoStream cryptoStream;
            SymmetricAlgorithm algorithmTripleDES = new TripleDESCryptoServiceProvider();
            if (iv != null)
            {
                algorithmTripleDES.IV = iv;
            }
            algorithmTripleDES.Key = key;
            algorithmTripleDES.Mode = System.Security.Cryptography.CipherMode.ECB;
            algorithmTripleDES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = algorithmTripleDES.CreateEncryptor(algorithmTripleDES.Key, algorithmTripleDES.IV);
            mStream = new MemoryStream();
            cryptoStream = new CryptoStream(mStream, ct, CryptoStreamMode.Write);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            return mStream.ToArray();
        }

        /// <summary>
        /// 3DES����
        /// </summary>
        /// <param name="data">����ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������ Ĭ�Ͽ�,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">Ĭ��ECB</param>
        /// <param name="padding">Ĭ��PKCS7</param>
        /// <returns>���ܺ������</returns>
        public static byte[] DES3Decrypt(byte[] data, 
            byte[] key,
            byte[] iv = null,
            CipherMode mode = System.Security.Cryptography.CipherMode.ECB,
            PaddingMode padding = System.Security.Cryptography.PaddingMode.PKCS7)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            SymmetricAlgorithm algorithmTripleDES = new TripleDESCryptoServiceProvider(); 
            algorithmTripleDES.Key = key;
            if(iv!=null)
            {
                algorithmTripleDES.IV = iv;
            }
            algorithmTripleDES.Mode = System.Security.Cryptography.CipherMode.ECB;
            algorithmTripleDES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = algorithmTripleDES.CreateDecryptor(algorithmTripleDES.Key, algorithmTripleDES.IV); 
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return ms.ToArray();
        }

        #endregion


        #region RC2���ܽ���

        /// <summary>
        /// RC2����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ�����ȴ��ڵ���8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        public static void RC2Encrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            SymmetricEncrypt(input, key, iv ?? defaultRC2IV, "RC2", mode, padding, output);
        }

        /// <summary>
        /// RC2����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        public static void RC2Decrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            SymmetricDecrypt(input, key, iv ?? defaultRC2IV, "RC2", mode, padding, output);
        }

        /// <summary>
        /// RC2����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        public static string RC2Encrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
            MemoryStream outms = new MemoryStream();
            RC2Encrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return Convert.ToBase64String(outms.ToArray());
        }

        /// <summary>
        /// RC2����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        public static string RC2Decrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(Convert.FromBase64String(input));
            MemoryStream outms = new MemoryStream();
            RC2Decrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return System.Text.Encoding.UTF8.GetString(outms.ToArray());
        }
        #endregion

        #region Rijndael���ܽ���

        /// <summary>
        /// Rijndael����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        public static void RijndaelEncrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            SymmetricEncrypt(input, key, iv ?? defaultRijndaelIV, "Rijndael", mode, padding, output);
        }

        /// <summary>
        /// Rijndael����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        public static void RijndaelDecrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            SymmetricDecrypt(input, key, iv ?? defaultRijndaelIV, "Rijndael", mode, padding, output);
        }

        /// <summary>
        /// Rijndael����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        public static string RijndaelEncrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
            MemoryStream outms = new MemoryStream();
            RijndaelEncrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return Convert.ToBase64String(outms.ToArray());
        }

        /// <summary>
        /// Rijndael����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        public static string RijndaelDecrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(Convert.FromBase64String(input));
            MemoryStream outms = new MemoryStream();
            RijndaelDecrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return System.Text.Encoding.UTF8.GetString(outms.ToArray());
        }

        #endregion

        #region TripleDES���ܽ���
        /// <summary>
        /// TripleDES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        private static void TripleDESEncrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            SymmetricEncrypt(input, key, iv ?? defaultTDESIV, "TripleDES", mode, padding, output);
        }

        /// <summary>
        /// TripleDES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�������</param>
        /// <param name="key">��Կ</param>
        /// <param name="iv">��ʼ������,�����Ϊ��,�������һ������Ϊ8���ֽڵ�����</param>
        /// <param name="mode">ģʽ</param>
        /// <param name="padding">���</param>
        /// <param name="output">���ܺ��������</param>
        private static void TripleDESDecrypt(Stream input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding, Stream output)
        {
            SymmetricDecrypt(input, key, iv ?? defaultTDESIV, "TripleDES", mode, padding, output);
        }

        /// <summary>
        /// TripleDES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        public static string TripleDESEncrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
            MemoryStream outms = new MemoryStream();
            TripleDESEncrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return Convert.ToBase64String(outms.ToArray());
        }

        /// <summary>
        /// TripleDES����
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ</param>
        public static string TripleDESDecrypt(string input, string key)
        {
            MemoryStream inms = new MemoryStream(Convert.FromBase64String(input));
            MemoryStream outms = new MemoryStream();
            TripleDESDecrypt(inms, System.Text.Encoding.UTF8.GetBytes(key), null, CipherMode.CBC, PaddingMode.PKCS7, outms);
            return System.Text.Encoding.UTF8.GetString(outms.ToArray());
        }

        #endregion

        #region ˽�з���
        private static void SymmetricEncrypt(Stream input, byte[] key, byte[] iv, string type, CipherMode mode, PaddingMode padding, Stream output)
        {
            using (SymmetricAlgorithm sa = SymmetricAlgorithm.Create(type))
            {
                sa.Mode = mode;
                sa.Padding = padding;
                CryptoStream cst = new CryptoStream(output, sa.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                byte[] buffer = new byte[2048];
                int len = 0;
                len = input.Read(buffer, 0, buffer.Length);
                while (len > 0)
                {
                    cst.Write(buffer, 0, len);
                    len = input.Read(buffer, 0, buffer.Length);
                }
                cst.Close();
                input.Close();
            }
        }

        private static void SymmetricDecrypt(Stream input, byte[] key, byte[] iv, string type, CipherMode mode, PaddingMode padding, Stream output)
        {
            using (SymmetricAlgorithm sa = SymmetricAlgorithm.Create(type))
            {
                sa.Mode = mode;
                sa.Padding = padding;
                CryptoStream cst = new CryptoStream(input, sa.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                byte[] buffer = new byte[2048];
                int len = 0;
                len = cst.Read(buffer, 0, buffer.Length);
                while (len > 0)
                {
                    output.Write(buffer, 0, len);
                    len = cst.Read(buffer, 0, buffer.Length);
                }
                input.Close();
                output.Close();
            }
        }
        #endregion

        #endregion

        #region ���ԳƼӽ��� ǩ��

        #region RSA
        /// <summary>
        /// ʹ�� RSA �㷨�����ݽ��м���
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ��Ϣ</param>
        /// <param name="fOAEP">��WinXp���Ժ�İ汾������ΪTrue</param>
        /// <returns>���ܺ������</returns>
        public static byte[] RSAEncrypt(byte[] input, RSAParameters key, bool fOAEP)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            //�������WindowsXP�����ϵİ汾,��fOAEP����Ϊfalse.����ʹ��Ĭ��ֵ
            if (!(Environment.OSVersion.Platform == PlatformID.Win32NT &&
                    (Environment.OSVersion.Version.Major >= 6 ||
                    (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1))
                 )
               )
            {
                fOAEP = false;
            }
            return rsa.Encrypt(input, fOAEP);
        }

        /// <summary>
        /// ʹ�� RSA �㷨�����ݽ��н���
        /// </summary>
        /// <param name="input">Ҫ���ܵ�����</param>
        /// <param name="key">��Կ��Ϣ</param>
        /// <param name="fOAEP">��WinXp���Ժ�İ汾������ΪTrue</param>
        /// <returns>���ܺ������</returns>
        public static byte[] RSADecrypt(byte[] input, RSAParameters key, bool fOAEP)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);

            //�������WindowsXP�����ϵİ汾,��fOAEP����Ϊfalse.����ʹ��Ĭ��ֵ
            if (!(Environment.OSVersion.Platform == PlatformID.Win32NT &&
                   (Environment.OSVersion.Version.Major >= 6 ||
                   (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1))
                   )
                )
            {
                fOAEP = false;
            }

            return rsa.Decrypt(input, fOAEP);
        }

        /// <summary>
        /// ����ָ�����ݵĹ�ϣֵ������ǩ��
        /// </summary>
        /// <param name="input">Ҫǩ����������</param>
        /// <param name="key">RSA�㷨�Ĳ���</param>
        /// <param name="halg">����Hashֵ���㷨</param>
        /// <returns>ǩ���������</returns>
        public static byte[] RSASignData(byte[] input, RSAParameters key, string halg)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            HashAlgorithm ha = HashAlgorithm.Create(halg);
            return rsa.SignData(input, ha);
        }

        /// <summary>
        /// ����ָ�����ݵĹ�ϣֵ������ǩ��
        /// </summary>
        /// <param name="input">Ҫǩ����������</param>
        /// <param name="key">RSA�㷨�Ĳ���</param>
        /// <param name="halg">����Hashֵ���㷨</param>
        /// <returns>ǩ���������</returns>
        public static byte[] RSASignData(Stream input, RSAParameters key, string halg)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            HashAlgorithm ha = HashAlgorithm.Create(halg);
            return rsa.SignData(input, ha);
        }

        /// <summary>
        /// ͨ����˽Կ������м���������ָ����ϣֵ��ǩ��
        /// </summary>
        /// <param name="input">Ҫǩ����������</param>
        /// <param name="key">RSA�㷨�Ĳ���</param>
        /// <param name="halg">����Hashֵ���㷨</param>
        /// <returns>ǩ���������</returns>
        public static byte[] RSASignHash(byte[] input, RSAParameters key, string halg)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            HashAlgorithm ha = HashAlgorithm.Create(halg);
            input = ha.ComputeHash(input);
            return rsa.SignHash(input, CryptoConfig.MapNameToOID(halg));
        }

        /// <summary>
        /// ͨ����ָ����ǩ��������Ϊָ�����ݼ����ǩ�����бȽ�����ָ֤����ǩ������
        /// </summary>
        /// <param name="input">Ҫ��֤��������</param>
        /// <param name="signature">����ǩ��</param>
        /// <param name="key">RSA�㷨�Ĳ���</param>
        /// <param name="halg">����Hashֵ���㷨</param>
        /// <returns>����һ��ֵ��ָʾ����֤��ǩ���Ƿ���Ч</returns>
        public static bool RSAVerifyData(byte[] input, byte[] signature, RSAParameters key, string halg)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            HashAlgorithm ha = HashAlgorithm.Create(halg);
            return rsa.VerifyData(input, ha, signature);
        }

        /// <summary>
        /// ͨ����ָ����ǩ��������Ϊָ����ϣֵ�����ǩ�����бȽ�����ָ֤����ǩ������
        /// </summary>
        /// <param name="input">Ҫ��֤������</param>
        /// <param name="signature">����ǩ��</param>
        /// <param name="key">RSA�㷨�Ĳ���</param>
        /// <param name="halg">����Hashֵ���㷨</param>
        /// <returns>����һ��ֵ��ָʾ����֤��ǩ���Ƿ���Ч</returns>
        public static bool RSAVerifyHash(byte[] input, byte[] signature, RSAParameters key, string halg)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            HashAlgorithm ha = HashAlgorithm.Create(halg);
            input = ha.ComputeHash(input);
            return rsa.VerifyHash(input, CryptoConfig.MapNameToOID(halg), signature);
        }

        #endregion

        #region DSA

        /// <summary>
        /// ����ָ���ֽ�����Ĺ�ϣֵ���Խ����ϣֵǩ��
        /// </summary>
        /// <param name="input">Ҫ�������ϣֵ����������</param>
        /// <param name="key">��Կ/˽Կ</param>
        /// <returns>ָ�����ݵ� DSA ǩ��</returns>
        public static byte[] DSASignData(byte[] input, DSAParameters key)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            dsa.ImportParameters(key);
            return dsa.SignData(input);
        }

        /// <summary>
        /// ����ָ���ֽ�����Ĺ�ϣֵ���Խ����ϣֵǩ��
        /// </summary>
        /// <param name="input">Ҫ�������ϣֵ����������</param>
        /// <param name="key">��Կ/˽Կ</param>
        /// <returns>ָ�����ݵ� DSA ǩ��</returns>
        public static byte[] DSASignData(Stream input, DSAParameters key)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            dsa.ImportParameters(key);
            return dsa.SignData(input);
        }

        /// <summary>
        /// ͨ����˽Կ������м���������ָ����ϣֵ��ǩ��
        /// </summary>
        /// <param name="input">Ҫǩ�������ݵĹ�ϣֵ(20Byte)</param>
        /// <param name="key">��Կ/˽Կ</param>
        /// <param name="halg">���ڴ������ݵĹ�ϣֵ�Ĺ�ϣ�㷨����</param>
        /// <returns></returns>
        public static byte[] DSASignHash(byte[] input, DSAParameters key, string halg)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            dsa.ImportParameters(key);
            return dsa.SignHash(input, CryptoConfig.MapNameToOID(halg));
        }

        /// <summary>
        /// ͨ����ָ����ǩ��������Ϊָ�����ݼ����ǩ�����бȽ�����ָ֤����ǩ������
        /// </summary>
        /// <param name="input">��ǩ��������</param>
        /// <param name="signature">Ҫ��֤��ǩ������</param>
        /// <param name="key">��Կ/˽Կ</param>
        /// <returns>���ǩ����֤Ϊ��Ч,��Ϊ true,����Ϊ false</returns>
        public static bool DSAVerifyData(byte[] input, byte[] signature, DSAParameters key)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            dsa.ImportParameters(key);
            return dsa.VerifyData(input, signature);
        }

        /// <summary>
        /// ͨ����ָ����ǩ��������Ϊָ����ϣֵ�����ǩ�����бȽ�����ָ֤����ǩ������
        /// </summary>
        /// <param name="input">Ҫǩ�������ݵĹ�ϣֵ</param>
        /// <param name="signature">Ҫ��֤��ǩ������</param>
        /// <param name="key">��Կ/˽Կ</param>
        /// <param name="halg">���ڴ������ݵĹ�ϣֵ�Ĺ�ϣ�㷨����</param>
        /// <returns>���ǩ����֤Ϊ��Ч,��Ϊ true,����Ϊ false</returns>
        public static bool DSAVerifyHash(byte[] input, byte[] signature, DSAParameters key, string halg)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            dsa.ImportParameters(key);
            return dsa.VerifyHash(input, CryptoConfig.MapNameToOID(halg), signature);
        }


        /// <summary>
        /// ��ָ֤�����ݵ� DSA ǩ��
        /// </summary>
        /// <param name="input">�� signature ǩ��������</param>
        /// <param name="signature">ҪΪ input ��֤��ǩ��</param>
        /// <param name="key">��Կ/˽Կ</param>
        /// <returns>��� signature ��ʹ��ָ���Ĺ�ϣ�㷨����Կ�� input �ϼ������ǩ��ƥ��,��Ϊ true,����Ϊ false</returns>
        public static bool DSAVerifySignature(byte[] input, byte[] signature, DSAParameters key)
        {
            DSACryptoServiceProvider dsa = new DSACryptoServiceProvider();
            dsa.ImportParameters(key);
            return dsa.VerifySignature(input, signature);
        }
        #endregion


        #endregion

        public static byte[] X99MAC(byte[] tKey,byte[] tIv, byte[] tBuffer, int iOffset, int iLength)
        {
            byte[] tResult = null;
            List<byte[]> vctBlk = new List<byte[]>();
            byte[] tTmp, tBlk, tXor, tDes;
            int iNum, iLen, iPos, iN, i, j;

            if (tKey == null || tBuffer == null) return tResult;

            if (iOffset < 0) iOffset = 0;
            if (iLength < 0) iLength = tBuffer.Length - iOffset;

            // ������ݣ�8�ֽڿ�/Block��
            iLen = 0;
            iPos = iOffset;
            while (iLen < iLength && iPos < tBuffer.Length)
            {
                tBlk = new byte[8];
                for (i = 0; i < tBlk.Length; i++) tBlk[i] = (byte)0;	// clear(0x00)
                for (i = 0; i < tBlk.Length && iLen < iLength && iPos < tBuffer.Length; i++)
                {
                    tBlk[i] = tBuffer[iPos++];
                    iLen++;
                }
                vctBlk.Add(tBlk);	// store (back)
            }

            // ѭ�����㣨XOR + DES��
            tDes = new byte[8];			// ��ʼ����
            for (i = 0; i < tDes.Count(); i++) tDes[i] = (byte)0;	// clear(0x00)

            iNum = vctBlk.Count();
            for (iN = 0; iN < iNum; iN++)
            {
                tBlk = (byte[])vctBlk.ElementAt(iN);
                if (tBlk == null) continue;

                tXor = new byte[Math.Min(tDes.Length, tBlk.Length)];
                for (i = 0; i < tXor.Length; i++) tXor[i] = (byte)(tDes[i] ^ tBlk[i]);		// ���(Xor) 
                tTmp = DesEncrypt(tXor, tKey);	// DES���� 
                for (i = 0; i < tDes.Length; i++) tDes[i] = (byte)0;				// clear
                for (i = 0; i < Math.Min(tDes.Length, tTmp.Length); i++) tDes[i] = tTmp[i];	// copy / transfer
            }

            vctBlk.Clear();		// clear

            tResult = tDes;

            return tResult;
        }

        public static byte[] MAC_CBC(byte[] MacData, byte[] bKey, byte[] bIV)
        {
            try
            {
                int iGroup = 0;  
                byte[] bTmpBuf1 = new byte[8];
                byte[] bTmpBuf2 = new byte[8];
                // init
                Array.Copy(bIV, bTmpBuf1, 8);
                if ((MacData.Length % 8 == 0))
                    iGroup = MacData.Length / 8;
                else
                    iGroup = MacData.Length / 8 + 1;
                int i = 0;
                int j = 0;
                for (i = 0; i < iGroup; i++)
                {
                    Array.Copy(MacData, 8 * i, bTmpBuf2, 0, 8);
                    for (j = 0; j < 8; j++)
                        bTmpBuf1[j] = (byte)(bTmpBuf1[j] ^ bTmpBuf2[j]);

                    bTmpBuf2 = DesEncrypt(bTmpBuf1, bKey);
                    Array.Copy(bTmpBuf2, bTmpBuf1, 8);
                }
                return bTmpBuf2;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("MAC_CBC() Exception caught, exception = {0}", ex.Message);
                throw ex;
            }

        }
    }
}
