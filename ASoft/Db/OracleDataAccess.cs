using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using OracleType = Oracle.ManagedDataAccess.Client.OracleDbType;
namespace ASoft.Db
{
    /// <summary>
    /// Oracle微软数据库访问类
    /// </summary>
    public class OracleDataAccess : DataAccess<OracleConnection, OracleDataAdapter, OracleTransaction, OracleParameter>
    {
        /// <summary>
        /// 默认的构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public OracleDataAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// 默认的构造函数
        /// </summary>
        /// <param name="connectionBuilder">数据库连接字符串生成器</param>
        public OracleDataAccess(OracleConnectionStringBuilder connectionBuilder) :
            this(connectionBuilder.ConnectionString)
        {
        }

        public override string ParameterPrefix
        {
            get
            {
                return ":";
            }
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
                using (OracleCommand cmd = new OracleCommand(procName, CreateConnection() as OracleConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    OracleCommandBuilder.DeriveParameters(cmd);
                    cmd.Connection.Dispose();
                    //cmd.Parameters.RemoveAt(0);
                    pvs = new OracleParameter[cmd.Parameters.Count];
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
        public OracleParameter MakeIn(string name, OracleType type)
        { 
            return new OracleParameter(name, type);
        }

        public override IDbDataParameter MakeIn(string name, object value)
        {

            var p = new OracleParameter(name, GetDbType(value));
            if (value is bool)
            {
                p.Value = Convert.ToInt16(value);
            }
            else
            {

                p.Value = value;
            }
            if (value is Array)
            {
                p.Size = (value as Array).Length;
            }
            return p;
        }

        public virtual OracleType GetDbType(object value)
        {
            if (value == null)
            {
                return OracleType.Varchar2;
            }
            String type = value.GetType().ToString();
            if (value is Int16)
            {
                return OracleType.Int16;
            }
            else if (value is Int32)
            {
                return OracleType.Int32;
            }
            else if (value is Int64)
            {
                return OracleType.Int64;
            }
            else if (value is Decimal)
            {
                return OracleType.Decimal;
            }
            else if (value is Double)
            {
                return OracleType.Double;
            }
            else if (value is DateTime)
            {
                return OracleType.Date;
            }
            else if (value is float)
            {
                return OracleType.Single;
            }
            else if (value is byte[])
            {
                return OracleType.Blob;
            }
            else if (value is char[])
            {
                return OracleType.Clob;
            }
            else
            {
                return OracleType.Varchar2;
            }
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="size">参数长度</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public OracleParameter MakeIn(string name, OracleType type, int size)
        {
            return new OracleParameter(name, type, size);
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public OracleParameter MakeIn(string name, OracleType type, int size, object value)
        {
            OracleParameter p = new OracleParameter(name, type, size);
            p.Value = value;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public OracleParameter MakeOut(string name, OracleType type)
        {
            OracleParameter p = new OracleParameter(name, type);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public OracleParameter MakeOut(string name, OracleType type, int size)
        {
            OracleParameter p = new OracleParameter(name, type, size);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数方向</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public OracleParameter MakeParam(string name, object value, OracleType type, ParameterDirection direction, int size)
        {
            OracleParameter p = new OracleParameter(name, type);
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

        /// <summary>
        /// 获取系统所有表的SQL语句
        /// </summary>
        protected override string AllTableSql
        {
            get
            {
                return "select   TABLE_NAME  AS NAME  from   user_tables  ";
            }
        }
    }
}
