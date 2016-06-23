using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ASoft.Db
{
    /// <summary>
    /// 一个执行数据库命令的Command接口
    /// </summary>
    /// <typeparam name="P">数据命令参数类型</typeparam>
    public interface IDataCommand
    {
        /// <summary>
        /// 数据库命令
        /// </summary>
        string CommandText
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库命令的类型
        /// </summary>
        CommandType CommandType
        {
            get;
            set;
        }

        /// <summary>
        /// 参数
        /// </summary>
        IDbDataParameter[] IDbParameters
        {
            get;
        }
    }
}
