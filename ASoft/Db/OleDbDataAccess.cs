using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;

namespace ASoft.Db
{
    /// <summary>
    /// OleDb数据库访问类
    /// </summary>
    public class OleDbDataAccess : DataAccess<OleDbConnection, OleDbDataAdapter, OleDbTransaction, OleDbParameter>
    {
        /// <summary>
        /// 默认的构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public OleDbDataAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// 默认的构造函数
        /// </summary>
        /// <param name="connectionBuilder">数据库连接字符串生成器</param>
        public OleDbDataAccess(OleDbConnectionStringBuilder connectionBuilder) :
            this(connectionBuilder.ConnectionString)
        {
        }

        /// <summary>
        /// 获取存储过程的参数
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>存储过程的参数</returns>
        protected override IDbDataParameter[] GetProcParameters(string procName)
        {
            IDbDataParameter[] pvs = GrabParameters(procName);
            if (pvs == null)
            {
                using (OleDbCommand cmd = new OleDbCommand(procName, CreateConnection() as OleDbConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    OleDbCommandBuilder.DeriveParameters(cmd);
                    cmd.Connection.Dispose();
                    cmd.Parameters.RemoveAt(0);
                    pvs = new OleDbParameter[cmd.Parameters.Count];
                    cmd.Parameters.CopyTo(pvs, 0);
                    SaveParameters(procName, pvs);
                    pvs = GrabParameters(procName);
                }
            }
            return pvs;
        }

        #region 参数管理

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public OleDbParameter MakeIn(string name, OleDbType type)
        {
            return new OleDbParameter(name, type);
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="size">参数长度</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public OleDbParameter MakeIn(string name, OleDbType type, int size)
        {
            return new OleDbParameter(name, type, size);
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public OleDbParameter MakeIn(string name, object value, OleDbType type, int size)
        {
            OleDbParameter p = new OleDbParameter(name, type, size);
            p.Value = value;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public OleDbParameter MakeOut(string name, OleDbType type)
        {
            OleDbParameter p = new OleDbParameter(name, type);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="size">参数长度</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public OleDbParameter MakeOut(string name, OleDbType type, int size)
        {
            OleDbParameter p = new OleDbParameter(name, type, size);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <param name="direction">参数方向</param>
        /// <returns>数据库命令参数</returns>
        public OleDbParameter MakeParam(string name, object value, OleDbType type, ParameterDirection direction, int size)
        {
            OleDbParameter p = new OleDbParameter(name, type);
            if (value != null)
            {
                p.Value = value;
            }
            if (size > 0)
            {
                p.Size = size;
            }
            p.Direction = direction;
            return p;
        }

        #endregion

    }
}