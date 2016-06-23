using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft.Model
{
    /// <summary>
    /// 状态标志位数组
    /// <para>
    /// T是一个枚举类型(枚举的值从0开始,依次递加)
    /// </para>
    /// </summary>
    public class StatusFlag<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        private FlagValue[] flags;

        /// <summary>
        /// 使用默认值全部为false状态初始化标识位(默认长度为8位)
        /// </summary>
        public StatusFlag()
            : this(FlagValue.None)
        {
        }

        /// <summary>
        /// 使用默认值为参数指定的值的状态初始化标志位(默认长度为8位)
        /// </summary>
        /// <param name="flag">指定的标志位</param>
        public StatusFlag(FlagValue flag)
            : this(8, flag)
        {
        }

        /// <summary>
        /// 使用指定的长度初始化标志位
        /// </summary>
        /// <param name="length">长度</param>
        public StatusFlag(int length)
            : this(length, FlagValue.None)
        {

        }

        /// <summary>
        /// 使用指定的长度初始化标志位
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="flag">初始状态</param>
        public StatusFlag(int length, FlagValue flag)
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("泛型参数T必须是枚举类型");
            }
            flags = new FlagValue[length];
            for (int i = 0; i < flags.Length; i++)
            {
                flags[i] = flag;
            }
        }

        /// <summary>
        /// 根据标志位生成一个标志位数组
        /// </summary>
        /// <param name="flags">标志信息</param>
        public StatusFlag(string flags)
        {
            if (flags == null || flags.Length == 0)
            {
                throw new ArgumentException("参数不能为空或长度为0");
            }
            this.flags = new FlagValue[flags.Length];
            for (int i = 0; i < this.flags.Length; i++)
            {
                this[i] = ConvertToFlag(flags[i]);
            }
        }

        /// <summary>
        /// 获取或设置某一位的标志位
        /// </summary>
        /// <param name="index">第几位</param>
        /// <returns>标志位的值</returns>
        public FlagValue this[T index]
        {
            get
            {
                return flags[Convert.ToInt32(index)];
            }
            set
            {
                this.flags[Convert.ToInt32(index)] = value;
            }
        }

        /// <summary>
        /// 获取或设置某一位的标志位
        /// </summary>
        /// <param name="index">第几位</param>
        /// <returns>标志位的值</returns>
        public FlagValue this[int index]
        {
            get
            {
                return flags[index];
            }
            set
            {
                this.flags[index] = value;
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>转换后的字符串</returns>
        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < flags.Length; i++)
            {
                result += ConvertToString(flags[i]);
            }
            return result;
        }

        /// <summary>
        /// 把一个字符串转换为一个标志位数组对象
        /// </summary>
        /// <param name="flags">原始字符串</param>
        /// <returns>转换后的标志位</returns>
        public static StatusFlag<T> Parse(string flags)
        {
            return new StatusFlag<T>(flags);
        }

        /// <summary>
        /// 用参数指定的标志位数组更新当前的标志位数组
        /// </summary>
        /// <param name="flag">新的标志位信息</param>
        /// <returns>更新后的标志位数组信息</returns>
        public void Update(StatusFlag<T> flag)
        {
            if (flag == null || this.Length != flag.Length)
            {
                throw new ArgumentException("参数为空,或参数的长度和当前标志位的长度不相等");
            }
            for (int i = 0; i < flag.Length; i++)
            {
                if (flag[i] != FlagValue.None)
                {
                    this[i] = flag[i];
                }
            }
        }

        /// <summary>
        /// 格式化为数据库中使用的更新字符串
        /// </summary>
        /// <param name="columnName">数据库字段</param>
        /// <param name="provider">数据库类型</param>
        /// <returns>返回格式化后的更新字符串</returns>
        public string ToUpdateString(string columnName, ASoft.Db.DbProvider provider)
        {
            string result = string.Empty;
            int blank = 0; //记录不替换的字符串长度
            bool leftquot = false; //记录是否使用左引号
            string function = "SUBSTR({0},{1},{2})";
            string conn = " || ";
            switch (provider)
            {
                case ASoft.Db.DbProvider.Oracle:
                case ASoft.Db.DbProvider.SqlServer:
                case ASoft.Db.DbProvider.Sqlite:
                case ASoft.Db.DbProvider.FireBird:
                case ASoft.Db.DbProvider.OleDb:
                case ASoft.Db.DbProvider.DB2:
                case ASoft.Db.DbProvider.Informix:
                case ASoft.Db.DbProvider.Sybase:
                case ASoft.Db.DbProvider.PostgreSQL:
                case ASoft.Db.DbProvider.VistaDB:
                    {
                        #region Oracle SqlServer Sqlite VistaDB
                        switch (provider)
                        {
                            case ASoft.Db.DbProvider.SqlServer:
                            case ASoft.Db.DbProvider.OleDb:
                            case ASoft.Db.DbProvider.VistaDB:
                                {
                                    conn = " + ";
                                    function = "SUBSTRING({0},{1},{2})";
                                    break;
                                }
                            case ASoft.Db.DbProvider.FireBird:
                                {
                                    function = "SUBSTRING({0} FROM {1} FOR {2})";
                                    break;
                                }
                            case ASoft.Db.DbProvider.Sybase:
                                {
                                    conn = " + ";
                                    break;
                                }

                        }
                        for (int i = 0; i < this.Length; i++)
                        {
                            if (this[i] != FlagValue.None)
                            {
                                if (result.Length > 0)
                                {
                                    if (!leftquot)
                                    {
                                        result += conn;
                                    }
                                }
                                if (blank != 0)
                                {
                                    result += string.Format(function, columnName, i - blank + 1, blank);
                                    result += conn;
                                }

                                if (!leftquot)
                                {
                                    result += "'";
                                    leftquot = true;
                                }

                                result += ConvertToString(this[i]);

                                blank = 0;
                            }
                            else
                            {
                                if (leftquot)
                                {
                                    result += "'";
                                    leftquot = false;
                                }
                                blank++;
                            }
                        }
                        if (blank != 0)
                        {
                            if (result.Length > 0)
                            {
                                result += conn;
                            }
                            result += string.Format(function, columnName, this.Length - blank + 1, blank);
                        }
                        else
                        {
                            if (leftquot)
                            {
                                result += "'";
                                leftquot = false;
                            }
                        }
                        break;
                        #endregion
                    }
                case ASoft.Db.DbProvider.MySql:
                    {
                        #region MySql
                        result = " CONCAT(";
                        for (int i = 0; i < this.Length; i++)
                        {
                            if (this[i] != FlagValue.None)
                            {
                                if (blank != 0)
                                {
                                    result += string.Format(function, columnName, i - blank + 1, blank);
                                    result += ",";
                                }

                                if (!leftquot)
                                {
                                    result += "'";
                                    leftquot = true;
                                }
                                result += ConvertToString(this[i]);
                                blank = 0;
                            }
                            else
                            {
                                if (leftquot)
                                {
                                    result += "'";
                                    result += ",";
                                    leftquot = false;
                                }
                                blank++;
                            }
                        }
                        if (blank != 0)
                        {
                            result += string.Format(function, columnName, this.Length - blank + 1, blank);
                        }
                        else
                        {
                            if (leftquot)
                            {
                                result += "'";
                                leftquot = false;
                            }
                        }
                        result += ")";
                        #endregion
                        break;
                    }
                default:
                    {
                        throw new Exception("不支持当前的数据库类型");
                    }
            }


            return result;
        }

        /// <summary>
        /// 标志位数组的长度
        /// </summary>
        public int Length
        {
            get
            {
                return this.flags.Length;
            }
        }

        /// <summary>
        /// 把一个具体的状态码转换为字符串
        /// </summary>
        /// <param name="f">状态码</param>
        /// <returns>转换后的字符串</returns>
        private static string ConvertToString(FlagValue f)
        {
            string result = string.Empty;
            switch (f)
            {
                case FlagValue.True:
                    {
                        result = "T";
                        break;
                    }
                case FlagValue.False:
                    {
                        result = "F";
                        break;
                    }
                case FlagValue.Yes:
                    {
                        result = "Y";
                        break;
                    }
                case FlagValue.No:
                    {
                        result = "N";
                        break;
                    }
                default:
                    {
                        result = "_";
                        break;
                    }
            }
            return result;
        }

        /// <summary>
        /// 把一个字符转换为状态码
        /// </summary>
        /// <param name="c">字符</param>
        /// <returns>转换后的状态码</returns>
        private static FlagValue ConvertToFlag(char c)
        {
            switch (c)
            {
                case 'F':
                    {
                        return FlagValue.False;
                    }
                case 'T':
                    {
                        return FlagValue.True;
                    }
                case 'N':
                    {
                        return FlagValue.No;
                    }
                case 'Y':
                    {
                        return FlagValue.Yes;
                    }
                default:
                    {
                        return FlagValue.None;
                    }
            }

        }
    }

    /// <summary>
    /// 标志位可选的值
    /// </summary>
    public enum FlagValue
    {
        /// <summary>
        /// 未设置
        /// </summary>
        None,

        /// <summary>
        /// 真
        /// </summary>
        True,

        /// <summary>
        /// 假
        /// </summary>
        False,

        /// <summary>
        /// Yes
        /// </summary>
        Yes,

        /// <summary>
        /// No
        /// </summary>
        No
    }
}
