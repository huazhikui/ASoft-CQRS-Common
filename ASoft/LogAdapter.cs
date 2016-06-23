using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;

namespace ASoft
{
    /// <summary>
    /// ��־(Ĭ����־����ΪError.����Ϊ:Debug, Warn, Error, Fatal, Info),����Infoʼ��д��־
    /// </summary>
    public class LogAdapter
    {
        private static string basepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
        private static LogLevel level = LogLevel.Error;
        private static string timeformat = "yyyy-MM-dd HH:mm:ss:fff  ";
        private string type;
        private static Dictionary<string, LogAdapter> dictlog = new Dictionary<string, LogAdapter>();
        private static LogAdapter app = null;
        private static LogAdapter db = null;
        private string logFilePath = null;

        #region ��������

        /// <summary>
        /// ��ȡ��������־�ļ��ĸ�Ŀ¼
        /// </summary>
        public static string BasePath
        {
            get
            {
                return basepath;
            }
            set
            {
                basepath = value;
            }
        }

        /// <summary>
        /// ��ȡ��������־��¼�ļ���(Ĭ��ΪError.����Ϊ:Debug, Warn, Error, Fatal, Info)
        /// </summary>
        public static LogLevel Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        /// <summary>
        /// �ܷ��¼������Ϣ
        /// </summary>
        public static bool CanDebug
        {
            get
            {
                return (int)LogLevel.Debug >= (int)LogAdapter.level;
            }
        }

        /// <summary>
        /// �ܷ��¼������Ϣ
        /// </summary>
        public static bool CanError
        {
            get
            {
                return (int)LogLevel.Error >= (int)LogAdapter.level;
            }
        }

        /// <summary>
        /// �ܷ��¼��������
        /// </summary>
        public static bool CanFatal
        {
            get
            {
                return (int)LogLevel.Fatal >= (int)LogAdapter.level;
            }
        }

        /// <summary>
        /// �ܷ��¼������Ϣ
        /// </summary>
        public static bool CanWarn
        {
            get
            {
                return (int)LogLevel.Warn >= (int)LogAdapter.level;
            }
        }

        #endregion

        /// <summary>
        /// ϵͳĬ����־ʵ��
        /// </summary>
        public static LogAdapter App
        {
            get
            {
                return app;
            }
        }

        /// <summary>
        /// ϵͳ���ݷ��ʲ���־ʵ��
        /// </summary>
        public static LogAdapter Db
        {
            get
            {
                return db;
            }
        }

        /// <summary>
        /// ������־�����,��������Ϊһ������·�������·��
        /// </summary>
        public string LogFilePath
        {
            set
            {
                if (Path.IsPathRooted(value))
                {
                    this.logFilePath = value;
                }
                else
                {
                    this.logFilePath = Path.Combine(LogAdapter.basepath, value);
                }
            }
        }

        #region ���캯��
        /// <summary>
        /// ��̬���캯��,�Զ���ʼ��,ֻ��ʼ��һ�Ρ�
        /// </summary>
        static LogAdapter()
        {

            LogSection log = (LogSection)System.Configuration.ConfigurationManager.GetSection("log");
            if (log != null)
            {
                #region ����·��
                string path = log.Path.Trim();
                if (path.Length > 0)
                {
                    if (System.IO.Path.IsPathRooted(path))
                    {
                        basepath = path;
                    }
                    else
                    {
                        basepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                    }
                }
                #endregion

                #region ������־����
                LogAdapter.level = log.Level;
                #endregion
            }
            app = GetLogger("App");
            db = GetLogger("Db");
        }

        /// <summary>
        /// ��ȡLogAdapter���һ��ʵ��
        /// </summary>
        /// <param name="type">����</param>
        /// <returns>LogAdapter���һ��ʵ��</returns>
        public static LogAdapter GetLogger(string type)
        {
            LogAdapter log = null;
            string lowertype = type.ToLower();
            if (dictlog.ContainsKey(lowertype))
            {
                log = dictlog[lowertype];
            }
            else
            {
                log = new LogAdapter(type);
                dictlog.Add(lowertype, log);
            }
            return log;
        }

        /// <summary>
        /// ��ȡLogAdapter���һ��ʵ��
        /// </summary>
        /// <param name="type">����</param>
        /// <returns>LogAdapter���һ��ʵ��</returns>
        public static LogAdapter GetLogger(Type type)
        {
            return new LogAdapter(type.FullName);
        }
        /// <summary>
        /// ʹ��ָ�������Ƴ�ʼ��һ����־������
        /// </summary>
        /// <param name="type">��־����</param>
        private LogAdapter(string type)
        {
            this.type = type;
        }

        #endregion

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="level">��־����</param>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="type">��־���ݵ�����(�ᰴ�������������ļ���)</param>
        /// <param name="msg">��־����</param>
        /// <param name="compact">�Ƿ�ʹ��ѹ���ĸ�ʽ����¼��־��Ϣ</param>
        private void Write(LogLevel level, LogFileSpan sequence, string type, object msg, bool compact)
        {
            if (level == LogLevel.Info || (int)level >= (int)LogAdapter.level) //����д��־
            {
                #region ʹ��ϵͳ������ļ�
                string path = this.logFilePath;
                if (string.IsNullOrEmpty(path))
                {
                    switch (sequence)
                    {
                        case LogFileSpan.None:
                            {
                                path = string.Format("{1}{0}{2}.txt", Path.DirectorySeparatorChar, level, type);
                                break;
                            }
                        case LogFileSpan.Year:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.Year);
                                break;
                            }
                        case LogFileSpan.Month:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.ToString("yyyyMM"));
                                break;
                            }
                        case LogFileSpan.Week:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}{4}��.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.Year, DateUtils.GetWeekOfYear(DateTime.Now));
                                break;
                            }
                        case LogFileSpan.Hour:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.ToString("yyyyMMddHH"));
                                break;
                            }
                        default:
                            {
                                path = string.Format("{1}{0}{2}{0}{3}.txt", Path.DirectorySeparatorChar, level, type, DateTime.Now.ToString("yyyyMMdd"));
                                break;
                            }
                    }
                    path = Path.Combine(LogAdapter.basepath, path);
                }
                try
                {
                    ASoft.IO.Helper.CreateDirectory(Path.GetDirectoryName(path));
                    using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.UTF8))
                    {
                        if (compact)
                        {
                            sw.Write(DateTime.Now.ToString(timeformat));
                        }
                        else
                        {
                            sw.Write("������������������������������������������  ");
                            sw.Write(DateTime.Now.ToString(timeformat));
                            sw.WriteLine("������������������������������������������");
                        }
                        sw.WriteLine(msg);
                        if (!compact)
                        {
                            sw.WriteLine();
                        }
                        sw.Close();
                    }
                }
                catch
                {
                }
                #endregion
            }
        }

        #region д����Ϣ

        /// <summary>
        /// д����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Info(object msg)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, msg, true);
        }

        /// <summary>
        /// д����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(string format, params object[] msgs)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Info(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Info, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Info, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="msg">��־����</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        public void Info(bool compact, object msg)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Info, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        public void Info(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Info, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Info(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Info, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region д�������Ϣ

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Debug(object msg)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, msg, true);
        }

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(string format, params object[] msgs)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }


        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Debug(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Debug, sequence, this.type, msg, true);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Debug, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Debug(bool compact, object msg)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, msg, compact);

        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Debug, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);

        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Debug(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Debug, sequence, this.type, msg, compact);

        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Debug(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Debug, sequence, this.type, string.Format(format, msgs), compact);

        }
        #endregion

        #region д�뾯����Ϣ

        /// <summary>
        /// д�뾯����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Warn(object msg)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, msg, true);

        }

        /// <summary>
        /// д�뾯����Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(string format, params object[] msgs)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Warn(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Warn, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Warn, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Warn(bool compact, object msg)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Warn, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Warn(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Warn, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д�뾯����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Warn(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Warn, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region д�������Ϣ

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="msg">������Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Error(string msg, Exception ex)
        {
            Error(string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, msg, ex.Message, ex.StackTrace));
        }

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Error(object msg)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, msg, true);

        }

        /// <summary>
        /// д�������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(string format, params object[] msgs)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Error(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Error, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(LogFileSpan sequence, string format, params  object[] msgs)
        {
             Write(LogLevel.Error, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Error(bool compact, object msg)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Error, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Error(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Error, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Error(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Error, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion

        #region д��������Ϣ

        /// <summary>
        /// д�������Ϣ
        /// </summary>
        /// <param name="msg">������Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Fatal(string msg, Exception ex)
        {
            Fatal(string.Format("{1}{0}{2}{0}{3}", Environment.NewLine, msg, ex.Message, ex.StackTrace));
        }

        /// <summary>
        /// д������������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Fatal(object msg)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, msg, true);
        }

        /// <summary>
        /// д������������Ϣ(Ĭ��������־�ļ���Ƶ��Ϊһ��,��default�ļ�����)
        /// </summary>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="msg">��־����</param>
        public void Fatal(LogFileSpan sequence, object msg)
        {
            Write(LogLevel.Fatal, sequence, this.type, msg, false);
        }

        /// <summary>
        /// д����Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(LogFileSpan sequence, string format, params  object[] msgs)
        {
            Write(LogLevel.Fatal, sequence, this.type, string.Format(format, msgs), true);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Fatal(bool compact, object msg)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, msg, compact);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, LogFileSpan.Day, this.type, string.Format(format, msgs), compact);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="msg">��־����</param>
        public void Fatal(LogFileSpan sequence, bool compact, object msg)
        {
            Write(LogLevel.Fatal, sequence, this.type, msg, compact);
        }

        /// <summary>
        /// д������������Ϣ
        /// </summary>
        /// <param name="sequence">��־�ļ�����Ƶ��</param>
        /// <param name="compact">�Ƿ�ʹ�ý��ո�ʽ(ÿ�м�¼һ����Ϣ)</param>
        /// <param name="format">��ʽ���ַ���</param>
        /// <param name="msgs">��־����</param>
        public void Fatal(LogFileSpan sequence, bool compact, string format, params object[] msgs)
        {
            Write(LogLevel.Fatal, sequence, this.type, string.Format(format, msgs), compact);
        }
        #endregion
    }

    /// <summary>
    /// ��Ӧ��config�ļ��Ľڵ㣬����������־�Ļ�����Ϣ
    /// </summary>
    public sealed class LogSection : ConfigurationSection
    {
        /// <summary>
        /// ��־��Ŀ¼��·��
        /// </summary>
        [ConfigurationProperty("path", IsRequired = false, DefaultValue = "log")]
        public string Path
        {
            get
            {
                return (string)base["path"];
            }
        }

        /// <summary>
        /// ��־��Ŀ¼�ļ���
        /// </summary>
        [ConfigurationProperty("level", IsRequired = true, DefaultValue = LogLevel.Error)]
        public LogLevel Level
        {
            get
            {
                return (LogLevel)base["level"];
            }
        }
    }

    /// <summary>
    /// ��־�ļ����ɵ�Ƶ��
    /// </summary>
    public enum LogFileSpan
    {
        /// <summary>
        /// û�м��,��־��Ϣд�뵽һ���ļ���
        /// </summary>
        None,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Year,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Month,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Week,

        /// <summary>
        /// ���������ļ�
        /// </summary>
        Day,

        /// <summary>
        /// Сʱ
        /// </summary>
        Hour
    }

    /// <summary>
    /// ��־����
    /// </summary>
    public enum LogLevel
    {

        /// <summary>
        /// ������Ϣ
        /// </summary>
        Debug = 1,

        /// <summary>
        /// ������Ϣ
        /// </summary>
        Warn = 2,

        /// <summary>
        /// ������Ϣ
        /// </summary>
        Error = 3,

        /// <summary>
        /// ��������
        /// </summary>
        Fatal = 4,

        /// <summary>
        /// ��ͨ��Ϣ(���κ�����¶����¼)
        /// </summary>
        Info = 5

    }
}