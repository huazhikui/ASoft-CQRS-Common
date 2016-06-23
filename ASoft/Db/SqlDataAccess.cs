using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace ASoft.Db
{
    /// <summary>
    /// SQLServer数据库访问类
    /// </summary>
    public class SqlDataAccess : DataAccess<SqlConnection, SqlDataAdapter, SqlTransaction, SqlParameter>
    {
        /// <summary>
        /// 默认的构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public SqlDataAccess(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// 默认的构造函数
        /// </summary>
        /// <param name="connectionBuilder">数据库连接字符串生成器</param>
        public SqlDataAccess(SqlConnectionStringBuilder connectionBuilder) :
            this(connectionBuilder.ConnectionString)
        {
        }

        public string ParameterPrefix
        {
            get
            {
                return "@";
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
                using (SqlCommand cmd = new SqlCommand(procName, CreateConnection() as SqlConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection.Open();
                    SqlCommandBuilder.DeriveParameters(cmd);
                    cmd.Connection.Dispose();
                    cmd.Parameters.RemoveAt(0);
                    pvs = new SqlParameter[cmd.Parameters.Count];
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
        public SqlParameter MakeIn(string name, SqlDbType type)
        {
            return new SqlParameter(name, type);
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public SqlParameter MakeIn(string name, SqlDbType type, int size)
        {
            return new SqlParameter(name, type, size);
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public SqlParameter MakeIn(string name, SqlDbType type, int size, object value)
        {
            SqlParameter p = new SqlParameter(name, type);
            p.Value = value;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public SqlParameter MakeOut(string name, SqlDbType type)
        {
            SqlParameter p = new SqlParameter(name, type);
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
        public SqlParameter MakeOut(string name, SqlDbType type, int size)
        {
            SqlParameter p = new SqlParameter(name, type, size);
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
        public SqlParameter MakeParam(string name, object value, SqlDbType type, ParameterDirection direction, int size)
        {
            SqlParameter p = new SqlParameter(name, type);
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
                return "SELECT NAME FROM SYSOBJECTS WHERE XTYPE = 'U' AND [NAME] <> 'dtproperties'";
            }
        }

        /// <summary>
        /// 返回所有存储过程名称的SQL语句
        /// </summary>
        protected override string AllProcedureSql
        {
            get
            {
                return "select [name] from dbo.sysobjects where OBJECTPROPERTY(id, N'IsProcedure') = 1 order by name";
                ;
            }
        }

        /// <summary>
        /// 将数据表内的数据批量写入到数据库
        /// </summary>
        /// <param name="dt">数据表</param>
        public void BulkCopy(DataTable dt)
        {
            if (dt == null)
            {
                throw new ArgumentNullException("dt");
            }
            if (string.IsNullOrEmpty(dt.TableName))
            {
                throw new ArgumentException("没有为参数dt指定表名");
            }
            if (dt.Rows.Count == 0)
            {
                return;
            }
            using (SqlBulkCopy bulk = new SqlBulkCopy(this.ConnectionString))
            {
                bulk.BatchSize = 100000;
                bulk.BulkCopyTimeout = 60;
                bulk.DestinationTableName = dt.TableName;
                bulk.WriteToServer(dt);
            }
        }
    }

    /// <summary>
    /// 访问SQLServer的数据库的命令
    /// </summary>
    public abstract class SqlDataCommand : DataCommand<SqlParameter>
    {
    }
}
