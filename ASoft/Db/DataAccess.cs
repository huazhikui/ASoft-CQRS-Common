using System;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Dynamic;
using System.Reflection;
using ASoft.Text;

namespace ASoft.Db
{


    /// <summary>
    /// 数据库访问类
    /// </summary>
    /// <typeparam name="C">数据库连接类型</typeparam>
    /// <typeparam name="A">数据库适配器类型</typeparam>
    /// <typeparam name="T">数据库事务类型</typeparam>
    /// <typeparam name="P">数据命令参数类型</typeparam>
    public abstract class DataAccess<C, A, T, P> : IDataAccess
        where C : class, IDbConnection, new()
        where A : class, IDbDataAdapter
        where T : class, IDbTransaction
        where P : class, IDbDataParameter, new()
    {
        private const string sequenceTableSql = "CREATE TABLE  ASoft_Id_Gen (SeqName VARCHAR(50) Primary Key, SeqValue BIGINT)";
        private const string configTableSql = "CREATE TABLE SYS_ConfigSet ( Config_Key varchar(50) Primary Key, Config_Value varchar(4000))";
        protected SeqGen sequenceGenerator = null;
        //private static IAccessDatabaseOperation _accessDatabaseOperation;

         
        /// <summary>
        /// 创建线程安全的保存数据库命令参数的表.
        /// </summary>
        private Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        private string connectionString;
        /// <summary>
        /// 获取或设置连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString ?? string.Empty;
            }
        }

        private string parameterPrefix = "";
        /// <summary>
        /// 数据库命令参数前缀(如SqlServer数据库为@)
        /// </summary>
        public virtual string ParameterPrefix
        {
            get
            {
                return parameterPrefix;
            }
        }

        static CommandType GuessCommandType(string commandText)
        {
            return commandText.Trim().Contains(" ") ? CommandType.Text : CommandType.StoredProcedure;
        }

        #region 通用方法
        public Version version = new Version();

        private ASoft.Db.DbProvider provider = DbProvider.SqlServer;
        /// <summary>
        /// 返回数据库类型
        /// </summary>
        public ASoft.Db.DbProvider Provider
        {
            get
            {
                return this.provider;
            }
        }

        /// <summary>
        /// 获取配置集合
        /// </summary>
        public Dictionary<string, string> ConfigSet
        {
            get
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                using (DataReader dr = ExecuteReader("SELECT * FROM CONFIGSET"))
                {
                    while (dr.Read())
                    {
                        dict.Add(dr.GetString("CONFIG_KEY"), dr.GetString("CONFIG_VALUE"));
                    }
                }
                return dict;
            }
        }


        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SaveConfig(string key, string value)
        {
            if (!this.Tables.Contains("CONFIGSET"))
            {
                this.ExecuteNonQuery(configTableSql);
            }
            string sql = string.Format("UPDATE CONFIGSET SET CONFIG_VALUE = {0} WHERE UPPER(CONFIG_KEY) = {1}", ToSqlValue(value).ToUpper(), ToSqlValue(key).ToUpper());
            if (ExecuteNonQuery(sql).Value == 0)
            {
                sql = string.Format("INSERT INTO CONFIGSET(CONFIG_KEY,CONFIG_VALUE) VALUES ({0},{1})", ToSqlValue(key).ToUpper(), ToSqlValue(value).ToUpper());
                ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 默认的构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public DataAccess(string connectionString)
        {
            this.connectionString = connectionString;
            this.provider = DbTools.GetDbProvider(typeof(C));

            #region 获取 Version
            switch (this.provider)
            {
                case DbProvider.SqlServer:
                    {
                        this.parameterPrefix = "@";
                        #region SqlServer
                        try
                        {
                            version = new Version(this.ExecuteScalar("SELECT  SERVERPROPERTY('PRODUCTVERSION')").StringValue);
                        }
                        catch
                        {
                            version = new Version(8, 0); //默认为2000
                        }
                        break;
                        #endregion
                    }
                case DbProvider.FireBird:
                    {
                        #region FireBird
                        try
                        {
                            version = new Version(this.ExecuteScalar("SELECT RDB$GET_CONTEXT('SYSTEM', 'ENGINE_VERSION') FROM RDB$DATABASE").StringValue);
                        }
                        catch
                        {
                            version = new Version(1, 0); //默认为1.0
                        }
                        break;
                        #endregion
                    }
                case DbProvider.MySql:
                    {

                        #region MySql
                        try
                        {
                            MatchCollection mc = ASoft.Regular.Matches(@"\d{1,2}(\.(\d{1,10})){0,3}", this.ExecuteScalar("SELECT VERSION()").StringValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase); if (mc.Count > 0)
                            {
                                version = new Version(mc[0].Value);
                            }
                            else
                            {
                                version = new Version(5, 0);
                            }
                        }
                        catch
                        {
                            version = new Version(5, 0); //默认为1.0
                        }
                        break;
                        #endregion
                    }
                case DbProvider.Oracle:
                    {
                        #region Oracle
                        try
                        {
                            MatchCollection mc = ASoft.Regular.Matches(@"\d{1,2}(\.(\d{1,10})){0,3}", this.ExecuteScalar("SELECT * FROM PRODUCT_COMPONENT_VERSION").StringValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (mc.Count > 0)
                            {
                                version = new Version(mc[0].Value);
                            }
                            else
                            {
                                version = new Version(10, 2);
                            }
                        }
                        catch
                        {
                            version = new Version(10, 2);
                        }
                        this.parameterPrefix = ":";
                        break;

                        #endregion
                    }
                case DbProvider.Sqlite:
                    {
                        this.parameterPrefix = "@";
                        break;
                    }
                case DbProvider.SqlCe:
                    {
                        this.parameterPrefix = "@";
                        break;
                    }
                case DbProvider.VistaDB:
                    {
                        this.parameterPrefix = "@";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            #endregion 

        }

        public void SetConnectString(String myConnectString)
        {
            this.connectionString = myConnectString;
        }
        /// <summary>
        /// 创建一个连接对象
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection CreateConnection()
        {
            C conn = new C();
            conn.ConnectionString = this.connectionString;
            return conn;
        }



        /// <summary>
        /// 序列生成器
        /// </summary>
        public SeqGen SequenceGenerator
        {
            get
            {
                if (sequenceGenerator == null)
                {
                    sequenceGenerator = new SeqGen(this);
                }
                return sequenceGenerator;
            }
        }

        /// <summary>
        /// 压缩数据库
        /// </summary>
        public void Compress()
        {
            string sql = string.Empty;
            switch (this.Provider)
            {
                case DbProvider.Sqlite:
                    {
                        sql = "VACUUM";
                        break;
                    }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 创建保存序列的表的SQL脚本(表名:ASoft_Id_Gen)
        /// </summary>
        public virtual string SequenceTableSql
        {
            get
            {
                return sequenceTableSql;
            }
        }

        #endregion

        #region 参数管理

        /// <summary>
        /// 获取存储过程的参数
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>存储过程的参数数组</returns>
        protected abstract IDbDataParameter[] GetProcParameters(string procName);

        /// <summary>
        /// 缓存数据库命令参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="ps">参数列表,如果参数不继承ICloneable,则不缓存</param>
        public IDbDataParameter[] SaveParameters(string key, params IDbDataParameter[] ps)
        {
            if (ps.Length > 0 && ps[0] is ICloneable)
            {
                parmCache[key] = ps;
            }
            return ps;
        }

        /// <summary>
        /// 从缓存中获取数据库命令参数
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>数据库命令参数</returns>
        public IDbDataParameter[] GrabParameters(string key)
        {
            if (parmCache.ContainsKey(key))
            {
                IDbDataParameter[] cachedParms = (IDbDataParameter[])parmCache[key];
                IDbDataParameter[] clonedParms = new P[cachedParms.Length];
                if (cachedParms[0] is ICloneable)
                {
                    for (int i = 0, j = cachedParms.Length; i < j; i++)
                    {
                        clonedParms[i] = (P)((ICloneable)cachedParms[i]).Clone();
                    }
                }
                else
                {
                    for (int i = 0, j = cachedParms.Length; i < j; i++)
                    {
                        clonedParms[i] = new P();
                        clonedParms[i].ParameterName = cachedParms[i].ParameterName;
                        clonedParms[i].DbType = cachedParms[i].DbType;
                        clonedParms[i].Direction = cachedParms[i].Direction;
                        clonedParms[i].Size = cachedParms[i].Size;
                        clonedParms[i].Precision = cachedParms[i].Precision;
                        clonedParms[i].Scale = cachedParms[i].Scale;
                    }
                }
                return clonedParms;
            }
            return null;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>数据库命令参数</returns>
        public P MakeIn(string name)
        {
            P p = new P();
            p.ParameterName = name;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>数据库命令参数</returns>
        public virtual IDbDataParameter MakeIn(string name, object value)
        {
            P p = new P();
            p.ParameterName = name;
            if (value is bool)
            {
                p.Value = Convert.ToInt16(value);
            }
            else
            {
                p.Value = value;
            }

            if (value != null)
            {
                p.DbType = GetDbType(value);
            }
            if (value is Array)
            {
                p.Size = (value as Array).Length;
            }
            return p;
        }

        public virtual DbType GetDbType(object value)
        {
            String type = value.GetType().ToString();
            if (value is Int16)
            {
                return DbType.Int16;
            }
            else if (value is Int32)
            {
                return DbType.Int32;
            }
            else if (value is Int64)
            {
                return DbType.Int64;
            }
            else if (value is Decimal)
            {
                return DbType.Decimal;
            }
            else if (value is Double)
            {
                return DbType.Double;
            }
            else if (value is DateTime)
            {
                return DbType.DateTime;
            }
            else if (value is float)
            {
                return DbType.Single;
            }
            else
            {
                return DbType.String;
            }
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public P MakeIn(string name, DbType type)
        {
            P p = new P();
            p.ParameterName = name;
            p.DbType = type;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public P MakeIn(string name, DbType type, int size)
        {
            P p = new P();
            p.ParameterName = name;
            p.Size = size;
            p.DbType = type;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public P MakeIn(string name, DbType type, int size, object value)
        {
            P p = new P();
            p.ParameterName = name;
            p.Value = value;
            p.DbType = type;
            p.Size = size;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型</param>
        /// <returns>数据库命令参数</returns>
        public P MakeOut(string name, DbType type)
        {
            P p = new P();
            p.ParameterName = name;
            p.DbType = type;
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
        public P MakeOut(string name, DbType type, int size)
        {
            P p = new P();
            p.ParameterName = name;
            p.DbType = type;
            p.Size = size;
            p.Direction = ParameterDirection.Output;
            return p;
        }

        /// <summary>
        /// 创建数据库命令参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="direction">参数方向</param>
        /// <param name="size">参数长度</param>
        /// <returns>数据库命令参数</returns>
        public P MakeParam(string name, object value, DbType type, ParameterDirection direction, int size)
        {
            P p = new P();
            p.ParameterName = name;
            if (value != null)
            {
                p.Value = value;
            }
            if (size > 0)
            {
                p.Size = size;
            }
            p.DbType = type;
            p.Direction = direction;
            return p;
        }
        #endregion
        #region 执行参数化
        /// <summary>
        /// 执行参数化SQL
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>




        public NonQueryResult ExecuteSqlNonQuery(String commandText, params object[] commandParameters)
        {
            C connection = this.CreateConnection() as C;
            NonQueryResult result = null;
            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                connection.Open();
                IDbTransaction trans = connection.BeginTransaction();
                DbTools.PrepareCommand(cmd, connection, trans, CommandType.Text, commandText, (commandParameters as IDataParameter[]));
                try
                {
                    result = new NonQueryResult(cmd.ExecuteNonQuery(), cmd);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return result;
            }
        }
        #region 返回影响的行数



        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(C connection, string commandText)
        {
            return ExecuteNonQuery(connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(IDbTransaction transaction, string commandText)
        {
            return ExecuteNonQuery(transaction, commandText, CommandType.Text, null);
        }


        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteNonQuery(commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteNonQuery(connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteNonQuery(transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C cn = this.CreateConnection() as C)
            {
                return ExecuteNonQuery(cn, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                //---继续 
                try
                {
                    var result = new NonQueryResult(cmd.ExecuteNonQuery(), cmd);
                    return result;
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                try
                {
                    return new NonQueryResult(cmd.ExecuteNonQuery(), cmd);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令
        /// </summary>
        /// <param name="command">数据库命令对象</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(DataCommand command)
        {
            return ExecuteNonQuery(command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="command">数据库命令对象</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(C connection, DataCommand<P> command)
        {
            return ExecuteNonQuery(connection, command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="command">数据库命令对象</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteNonQuery(IDbTransaction transaction, DataCommand<P> command)
        {
            return ExecuteNonQuery(transaction, command.CommandText, command.CommandType, command.Parameters);
        }
        #endregion 执行数据库命令,返回受影响的行数

        #endregion

        #region 执行数据库命令,返回结果的第一行第一列

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text, null);
        }



        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(C connection, string commandText)
        {
            return ExecuteScalar(connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(IDbTransaction transaction, string commandText)
        {
            return ExecuteScalar(transaction, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteScalar(commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteScalar(connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteScalar(transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C cn = CreateConnection() as C)
            {
                return ExecuteScalar(cn, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                try
                {
                    return new ScalerResult(cmd.ExecuteScalar(), cmd);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                try
                {
                    return new ScalerResult(cmd.ExecuteScalar(), cmd);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="command">数据库命令</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(DataCommand command)
        {
            return ExecuteScalar(command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(C connection, DataCommand<P> command)
        {
            return ExecuteScalar(connection, command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="connection">数据库连接对象</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteScalar(IDbTransaction transaction, DataCommand<P> command)
        {
            return ExecuteScalar(transaction, command.CommandText, command.CommandType, command.Parameters);
        }


        #endregion 执行数据库命令,返回结果的第一行第一列


        #region 执行数据库命令,返回一个DataReader

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(commandText, CommandType.Text, null);
        }



        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(C connection, string commandText)
        {
            return ExecuteReader(connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(IDbTransaction transaction, string commandText)
        {
            return ExecuteReader(transaction, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>返回结果的第一行第一列</returns>
        public DataReader ExecuteReader(PageSearch search)
        {
            search.TotalCount = ExecuteScalar(DbTools.CreateCountSql(search)).IntValue;
            int skip;
            string sql = DbTools.CreatePageSql(search, Provider, version, out skip);
            DataReader dr = ExecuteReader(sql);
            dr.Skip(skip);
            return dr;
        }


        /// <summary>
        /// 为SQL查询条件生成分页查询SQL语句
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="start">返回的查询结果要跳过的行数</param>
        /// <param name="limit">返回记录集的长度</param>
        /// <param name="total">out 查询记录集的总记录数</param>
        /// <returns>返回分页数据读取器</returns>
        public DataReader ExecuteReader(string commandText, int start, int limit, out int total)
        {
            String countSql = String.Format("select count(1) from ( {0} ) ", commandText);
            total = ExecuteScalar(countSql).IntValue;
            string sql = DbTools.CreatePageSql(commandText, Provider, start, limit);
            DataReader dr = ExecuteReader(commandText);
            return dr;
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        public DataReader ExecuteReader(PageSearch search, params IDbDataParameter[] commandParameters)
        {
            search.TotalCount = ExecuteScalar(DbTools.CreateCountSql(search), CommandType.Text, commandParameters).IntValue;
            int skip;
            string sql = DbTools.CreatePageSql(search, Provider, version, out skip);
            DataReader dr = ExecuteReader(sql, commandParameters);
            dr.Skip(skip);
            return dr;
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="search">查询条件</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(C connection, PageSearch search)
        {
            search.TotalCount = ExecuteScalar(connection, DbTools.CreateCountSql(search)).IntValue;
            int skip;
            string sql = DbTools.CreatePageSql(search, Provider, version, out skip);
            DataReader dr = ExecuteReader(connection, sql);
            dr.Skip(skip);
            return dr;
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <param name="search">查询条件</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(C connection, PageSearch search, params IDbDataParameter[] commandParameters)
        {
            search.TotalCount = ExecuteScalar(connection, DbTools.CreateCountSql(search), CommandType.Text, commandParameters).IntValue;
            int skip;
            string sql = DbTools.CreatePageSql(search, Provider, version, out skip);
            DataReader dr = ExecuteReader(connection, sql, commandParameters);
            dr.Skip(skip);
            return dr;
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="search">查询条件</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(IDbTransaction transaction, PageSearch search)
        {
            search.TotalCount = ExecuteScalar(transaction, DbTools.CreateCountSql(search)).IntValue;
            int skip;
            string sql = DbTools.CreatePageSql(search, Provider, version, out skip);
            DataReader dr = ExecuteReader(transaction, sql);
            dr.Skip(skip);
            return dr;
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="search">查询条件</param>
        /// <param name="commandParameters">数据库命令类型</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(IDbTransaction transaction, PageSearch search, params IDbDataParameter[] commandParameters)
        {
            search.TotalCount = ExecuteScalar(transaction, DbTools.CreateCountSql(search), CommandType.Text, commandParameters).IntValue;
            int skip;
            string sql = DbTools.CreatePageSql(search, Provider, version, out skip);
            DataReader dr = ExecuteReader(transaction, sql, commandParameters);
            dr.Skip(skip);
            return dr;
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="search">查询条件</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(IDbTransaction transaction, PageSearch search, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            search.TotalCount = ExecuteScalar(transaction, DbTools.CreateCountSql(search), CommandType.Text, commandParameters).IntValue;
            int skip;
            string sql = DbTools.CreatePageSql(search, Provider, version, out skip);
            DataReader dr = ExecuteReader(transaction, sql, commandType, commandParameters);
            dr.Skip(skip);
            return dr;
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteReader(commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteReader(connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteReader(transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(DataCommand command)
        {
            return ExecuteReader(command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(C connection, DataCommand<P> command)
        {
            return ExecuteReader(connection, command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(IDbTransaction transaction, DataCommand command)
        {
            return ExecuteReader(transaction, command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <param name="connectionOwnership">数据库命令的连接类型</param>
        /// <returns>返回一个数据读取器</returns>
        private DataReader ExecuteReader(C connection, IDbTransaction transaction, string commandText, CommandType commandType, IDbDataParameter[] commandParameters, ConnOwnerShip connectionOwnership)
        {
            IDbCommand cmd = connection.CreateCommand();
            DbTools.PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);
            try
            {
                if (connectionOwnership == ConnOwnerShip.External)
                {
                    return new DataReader(cmd.ExecuteReader(), cmd);
                }
                else
                {
                    return new DataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection), cmd);
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                DbTools.WriteDbException(ex, cmd);
                throw ex;
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            C connection = this.CreateConnection() as C;
            return ExecuteReader(connection, null, commandText, commandType, commandParameters, ConnOwnerShip.Internal);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            return ExecuteReader(connection, null, commandText, commandType, commandParameters, ConnOwnerShip.External);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteReader(IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            return ExecuteReader((C)transaction.Connection, transaction, commandText, commandType, commandParameters, ConnOwnerShip.External);
        }

        #endregion  执行数据库命令,返回一个DataReader

        #region 执行数据库命令,返回一个数据表

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, CommandType.Text, null);
        }



        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, string commandText)
        {
            return ExecuteDataTable(connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, string commandText)
        {
            return ExecuteDataTable(transaction, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据表
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(PageSearch search)
        {
            using (C conn = this.CreateConnection() as C)
            {
                return ExecuteDataTable(conn, search);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据表
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(PageSearch search, params IDbDataParameter[] commandParameters)
        {
            using (C conn = this.CreateConnection() as C)
            {
                return ExecuteDataTable(conn, search, commandParameters);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="search">查询条件</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, PageSearch search)
        {
            using (DataReader dr = ExecuteReader(connection, search))
            {
                DataTable dt = new DataTable();
                dt.Load(dr.OriDataReader);
                return new DataTableResult(dt, dr.CommandText, dr.CommandType, dr.Parameters);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="search">查询条件</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, PageSearch search, params IDbDataParameter[] commandParameters)
        {
            using (DataReader dr = ExecuteReader(connection, search, commandParameters))
            {
                DataTable dt = new DataTable();
                dt.Load(dr.OriDataReader);
                return new DataTableResult(dt, dr.CommandText, dr.CommandType, dr.Parameters);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="search">查询条件</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, PageSearch search)
        {
            using (DataReader dr = ExecuteReader(transaction, search))
            {
                DataTable dt = new DataTable();
                dt.Load(dr.OriDataReader);
                return new DataTableResult(dt, dr.CommandText, dr.CommandType, dr.Parameters);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="search">查询条件</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, PageSearch search, params IDbDataParameter[] commandParameters)
        {
            using (DataReader dr = ExecuteReader(transaction, search, commandParameters))
            {
                DataTable dt = new DataTable();
                dt.Load(dr.OriDataReader);
                return new DataTableResult(dt, dr.CommandText, dr.CommandType, dr.Parameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataTable(commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataTable(connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataTable(transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C connection = this.CreateConnection() as C)
            {
                return ExecuteDataTable(connection, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                A da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            DataTable dt = new DataTable();
                            (da as DbDataAdapter).Fill(dt);
                            return new DataTableResult(dt, cmd);
                        }
                    }
                    else
                    {
                        using (da as IDisposable)
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return new DataTableResult(ds.Tables[0], cmd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                A da = ASoft.Reflect.CreateInstance<A>();
                using (da as IDisposable)
                {
                    da.SelectCommand = cmd;
                    try
                    {
                        if (da is DbDataAdapter)
                        {
                            using (da as IDisposable)
                            {
                                DataTable dt = new DataTable();
                                (da as DbDataAdapter).Fill(dt);
                                return new DataTableResult(dt, cmd);
                            }
                        }
                        else
                        {
                            using (da as IDisposable)
                            {
                                DataSet ds = new DataSet();
                                da.Fill(ds);
                                return new DataTableResult(ds.Tables[0], cmd);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DbTools.WriteDbException(ex, cmd);
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(DataCommand<P> command)
        {
            return ExecuteDataTable(command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(C connection, DataCommand<P> command)
        {
            return ExecuteDataTable(connection, command.CommandText, command.CommandType, command.Parameters);
        }
        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteDataTable(IDbTransaction transaction, DataCommand<P> command)
        {
            return ExecuteDataTable(transaction, command.CommandText, command.CommandType, command.Parameters);
        }
        #endregion 执行数据库命令,返回一个数据表

        #region 执行数据库命令,填充一个数据表

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="dt">要填充的数据表</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(DataTable dt, string commandText)
        {
            return FillDataTable(dt, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(C connection, DataTable dt, string commandText)
        {
            return FillDataTable(connection, dt, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(IDbTransaction transaction, DataTable dt, string commandText)
        {
            return FillDataTable(transaction, dt, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(DataTable dt, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataTable(dt, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(C connection, DataTable dt, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataTable(connection, dt, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(IDbTransaction transaction, DataTable dt, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataTable(transaction, dt, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(DataTable dt, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C connection = this.CreateConnection() as C)
            {
                return FillDataTable(connection, dt, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(C connection, DataTable dt, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                A da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            (da as DbDataAdapter).Fill(dt);
                            return new DataTableResult(dt, cmd);
                        }
                    }
                    throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(IDbTransaction transaction, DataTable dt, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                A da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            (da as DbDataAdapter).Fill(dt);
                            return new DataTableResult(dt, cmd);
                        }
                    }
                    throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(DataTable dt, DataCommand<P> command)
        {
            return FillDataTable(dt, command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(C connection, DataTable dt, DataCommand<P> command)
        {
            return FillDataTable(connection, dt, command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="dt">要填充的数据表</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult FillDataTable(IDbTransaction transaction, DataTable dt, DataCommand<P> command)
        {
            return FillDataTable(transaction, dt, command.CommandText, command.CommandType, command.Parameters);
        }
        #endregion 执行数据库命令,返回一个数据表

        #region 执行数据库命令,返回一个数据集

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(commandText, CommandType.Text, null);
        }



        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(C connection, string commandText)
        {
            return ExecuteDataSet(connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(IDbTransaction transaction, string commandText)
        {
            return ExecuteDataSet(transaction, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataSet(commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataSet(connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataSet(transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C cn = this.CreateConnection() as C)
            {
                return ExecuteDataSet(cn, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                A da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return new DataSetResult(ds, cmd);
                        }
                    }
                    throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                A da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return new DataSetResult(ds, cmd);
                        }
                    }
                    throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(DataCommand<P> command)
        {
            return ExecuteDataSet(command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(C connection, DataCommand<P> command)
        {
            return ExecuteDataSet(connection, command.CommandText, command.CommandType, command.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="transaction">数据库事务对象</param>
        /// <param name="command">数据库命令</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteDataSet(IDbTransaction transaction, DataCommand<P> command)
        {
            return ExecuteDataSet(transaction, command.CommandText, command.CommandType, command.Parameters);
        }
        #endregion 执行数据库命令,返回一个数据集

        #region 执行数据库命令,填充一个数据集

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="cmd">数据库命令的文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, DataCommand<P> cmd)
        {
            return FillDataSet(ds, cmd.CommandText, cmd.CommandType, cmd.Parameters);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string commandText)
        {
            return FillDataSet(ds, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, C connection, string commandText)
        {
            return FillDataSet(ds, connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, IDbTransaction transaction, string commandText)
        {
            return FillDataSet(ds, transaction, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataSet(ds, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataSet(ds, connection, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataSet(ds, transaction, commandText, GuessCommandType(commandText), commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="commandType">数据库命令的类型</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C connection = this.CreateConnection() as C)
            {
                return FillDataSet(ds, connection, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">数据库命令的类型</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            if (ds == null)
            {
                throw new Exception("要填充的数据集不能为空");
            }
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                IDbDataAdapter da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                {
                    try
                    {
                        if (da is DbDataAdapter)
                        {
                            using (da as IDisposable)
                            {
                                da.Fill(ds);
                                return new DataSetResult(ds, cmd);
                            }
                        }
                        throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        DbTools.WriteDbException(ex, cmd);
                        throw ex;
                    }

                }
            }
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="commandType">数据库命令的类型</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            if (ds == null)
            {
                throw new Exception("要填充的数据集不能为空");
            }
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                IDbDataAdapter da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                {
                    try
                    {
                        if (da is DbDataAdapter)
                        {
                            using (da as IDisposable)
                            {
                                da.Fill(ds);
                                return new DataSetResult(ds, cmd);
                            }
                        }
                        throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        DbTools.WriteDbException(ex, cmd);
                        throw ex;
                    }

                }
            }
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, string commandText)
        {
            return FillDataSet(ds, srcTable, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, C connection, string commandText)
        {
            return FillDataSet(ds, srcTable, connection, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, IDbTransaction transaction, string commandText)
        {
            return FillDataSet(ds, srcTable, transaction, commandText, CommandType.Text, null);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataSet(ds, srcTable, commandText, CommandType.StoredProcedure, commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, C connection, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataSet(ds, srcTable, connection, commandText, CommandType.StoredProcedure, commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, IDbTransaction transaction, string commandText, params IDbDataParameter[] commandParameters)
        {
            return FillDataSet(ds, srcTable, transaction, commandText, CommandType.StoredProcedure, commandParameters);
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="commandType">数据库命令的类型</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            using (C connection = this.CreateConnection() as C)
            {
                return FillDataSet(ds, srcTable, connection, commandText, commandType, commandParameters);
            }
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="commandType">数据库命令的类型</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult FillDataSet(DataSet ds, string srcTable, C connection, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            if (ds == null)
            {
                throw new Exception("要填充的数据集不能为空");
            }
            using (IDbCommand cmd = connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);
                IDbDataAdapter da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                {
                    try
                    {
                        if (da is DbDataAdapter)
                        {
                            using (da as IDisposable)
                            {
                                (da as DbDataAdapter).Fill(ds, srcTable);
                                return new DataSetResult(ds, cmd);
                            }
                        }
                        throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        DbTools.WriteDbException(ex, cmd);
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行数据库命令,填充一个数据集
        /// </summary>
        /// <param name="ds">要填充的数据集</param>
        /// <param name="srcTable">要填充的数据集的表的名称</param>
        /// <param name="transaction">数据库连接上的事务</param>
        /// <param name="commandType">数据库命令的类型</param>
        /// <param name="commandText">数据库命令的文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据集</returns>
        public static DataSetResult FillDataSet(DataSet ds, string srcTable, IDbTransaction transaction, string commandText, CommandType commandType, params IDbDataParameter[] commandParameters)
        {
            if (ds == null)
            {
                throw new Exception("要填充的数据集不能为空");
            }
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                DbTools.PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
                IDbDataAdapter da = ASoft.Reflect.CreateInstance<A>();
                da.SelectCommand = cmd;
                try
                {
                    if (da is DbDataAdapter)
                    {
                        using (da as IDisposable)
                        {
                            (da as DbDataAdapter).Fill(ds, srcTable);
                            return new DataSetResult(ds, cmd);
                        }
                    }
                    throw new Exception("不支持当前DataAdapter类型,具体类型为:" + da.GetType().FullName);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }
        #endregion

        #region 执行事务

        /// <summary>
        /// 执行一系列的事务
        /// </summary>
        /// <param name="commands">数据库命令</param>
        public int ExecuteTransaction(IList<string> commands)
        {
            using (C conn = this.CreateConnection() as C)
            {
                return ExecuteTransaction(conn, commands);
            }
        }

        /// <summary>
        /// 执行一系列的事务
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commands">数据库命令</param>
        public int ExecuteTransaction(C connection, IList<string> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return 0;
            }
            bool connisopen = true;
            if (connection.State != ConnectionState.Open)
            {
                connisopen = false;
                connection.Open();
            }

            using (IDbCommand cmd = connection.CreateCommand())
            using (IDbTransaction trans = (IDbTransaction)connection.BeginTransaction())
            {
                try
                {
                    int r = ExecuteTransaction(trans, commands);
                    trans.Commit();
                    return r;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
                finally
                {
                    if (connisopen == false)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 在一个事务上执行SQL语句.执行完后不提交,出错后也不回滚,需要手动处理
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commands">要执行的命令列表</param>
        public int ExecuteTransaction(IDbTransaction transaction, IList<string> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return 0;
            }
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                try
                {
                    int r = 0;
                    foreach (string command in commands)
                    {
                        DbTools.PrepareCommand(cmd, transaction.Connection, transaction, CommandType.Text, command, null);
                        r = cmd.ExecuteNonQuery();
                    }
                    return r;
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行一系列的事务
        /// </summary>
        /// <param name="commands">数据库命令</param>
        public int ExecuteTransaction(IList<DataCommand> commands)
        {
            using (C conn = this.CreateConnection() as C)
            {
                return ExecuteTransaction(conn, commands);
            }
        }

        /// <summary>
        /// 执行一系列的事务
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commands">数据库命令</param>
        public int ExecuteTransaction(C connection, IList<DataCommand> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return 0;
            }
            bool connisopen = true;
            if (connection.State != ConnectionState.Open)
            {
                connisopen = false;
                connection.Open();
            }

            using (IDbTransaction trans = (IDbTransaction)connection.BeginTransaction())
            {
                try
                {
                    int r = ExecuteTransaction(trans, commands);
                    trans.Commit();
                    return r;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    if (connisopen == false)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 在一个事务上执行SQL语句.执行完后不提交,出错后也不回滚,需要手动处理
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commands">要执行的命令列表</param>
        public int ExecuteTransaction(IDbTransaction transaction, IList<DataCommand> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return 0;
            }
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                try
                {
                    int r = 0;
                    foreach (DataCommand<IDbDataParameter> command in commands)
                    {
                        DbTools.PrepareCommand(cmd, transaction.Connection, transaction, command.CommandType, command.CommandText, command.Parameters);
                        r = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    return r;
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        #endregion

        #region 执行事务,最后返回一个ScalerResult对象

        /// <summary>
        /// 执行一系列的事务,最后一个SQL命令返回Scaler对象
        /// </summary>
        /// <param name="commands">数据库命令</param>
        public ScalerResult ExecuteScalerTransaction(IList<string> commands)
        {
            using (C conn = this.CreateConnection() as C)
            {
                return ExecuteScalerTransaction(conn, commands);
            }
        }

        /// <summary>
        /// 执行一系列的事务,最后一个SQL命令返回Scaler对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commands">数据库命令</param>
        public ScalerResult ExecuteScalerTransaction(C connection, IList<string> commands)
        {
            ScalerResult result = null;
            if (commands == null || commands.Count == 0)
            {
                return result;
            }
            bool connisopen = true;
            if (connection.State != ConnectionState.Open)
            {
                connisopen = false;
                connection.Open();
            }
            using (IDbTransaction trans = (IDbTransaction)connection.BeginTransaction())
            {
                try
                {
                    ScalerResult r = ExecuteScalerTransaction(trans, commands);
                    trans.Commit();
                    return r;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    if (connisopen == false)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 在一个事务上执行SQL语句.执行完后不提交,出错后也不回滚,需要手动处理
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commands">要执行的命令列表</param>
        public ScalerResult ExecuteScalerTransaction(IDbTransaction transaction, IList<string> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return null;
            }
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                try
                {
                    string lastsql = commands[commands.Count - 1];
                    commands.RemoveAt(commands.Count - 1);
                    foreach (string command in commands)
                    {
                        DbTools.PrepareCommand(cmd, transaction.Connection, transaction, CommandType.Text, command, null);
                        cmd.ExecuteNonQuery();
                    }
                    return ExecuteScalar(transaction, lastsql);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行一系列的事务,最后一个SQL命令返回Scaler对象
        /// </summary>
        /// <param name="commands">数据库命令</param>
        public ScalerResult ExecuteScalerTransaction(IList<DataCommand<P>> commands)
        {
            using (C conn = this.CreateConnection() as C)
            {
                return ExecuteScalerTransaction(conn, commands);
            }
        }

        /// <summary>
        /// 执行一系列的事务,最后一个SQL命令返回Scaler对象
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commands">数据库命令</param>
        public ScalerResult ExecuteScalerTransaction(C connection, IList<DataCommand<P>> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return null;
            }
            bool connisopen = true;
            if (connection.State != ConnectionState.Open)
            {
                connisopen = false;
                connection.Open();
            }

            using (IDbTransaction trans = (IDbTransaction)connection.BeginTransaction())
            {
                try
                {
                    ScalerResult r = ExecuteScalerTransaction(trans, commands);
                    trans.Commit();
                    return r;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    if (connisopen == false)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 在一个事务上执行SQL语句.执行完后不提交,出错后也不回滚,需要手动处理
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commands">要执行的命令列表</param>
        public ScalerResult ExecuteScalerTransaction(IDbTransaction transaction, IList<DataCommand<P>> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return null;
            }
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                try
                {
                    DataCommand<P> lastsql = commands[commands.Count - 1];
                    commands.RemoveAt(commands.Count - 1);
                    foreach (DataCommand<P> command in commands)
                    {
                        DbTools.PrepareCommand(cmd, transaction.Connection, transaction, command.CommandType, command.CommandText, command.Parameters);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    return ExecuteScalar(transaction, lastsql.CommandText, lastsql.CommandType, lastsql.Parameters);
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, cmd);
                    throw ex;
                }
            }
        }
        #endregion

        #region 格式化数据

        public string ToSqlValue(object value)
        {
            string result = string.Empty;
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }
            if (value is int || value is long || value is short || value is byte || value is double || value is float || value is decimal)
            {
                return value.ToString();
            }
            if (value is DateTime)
            {
                if ((DateTime)value == DateTime.MinValue || (DateTime)value == DateTime.MaxValue)
                {
                    return "NULL";
                }
                result = string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")); ;
                switch (provider)
                {
                    case ASoft.Db.DbProvider.Oracle:
                        {
                            result = string.Format("to_date('{0}','YYYY-MM-DD HH24:MI:SS')", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
                            break;
                        }
                    case DbProvider.OleDb:
                        {
                            result = ((DateTime)value).ToString("#yyyy-MM-dd HH:mm:ss#");
                            break;
                        }
                }
                return result;
            }
            if (value is TimeSpan)
            {
                if (((TimeSpan)value) == TimeSpan.Zero)
                {
                    return "NULL";
                }
                return ((TimeSpan)value).TotalSeconds.ToString();
            }
            if (value is string)
            {
                return string.Format("'{0}'", ((string)value).Replace("'", "''"));
            }
            if (value is bool)
            {
                return (bool)value ? "1" : "0";
            }
            if (value is Guid)
            {
                if (((Guid)value) == Guid.Empty)
                {
                    return "NULL";
                }
                return "'" + ((Guid)value).ToString() + "'";
            }
            if (value is Enum)
            {
                return ((int)value).ToString();
            }

            if (value is int[] || value is List<int>)
            {
                return string.Format("'{0}'", StringUtils.Join<int>((IEnumerable<int>)value));
            }
            if (value is long[] || value is List<long>)
            {
                return string.Format("'{0}'", StringUtils.Join<long>((IEnumerable<long>)value));
            }
            if (value is string[] || value is List<string>)
            {
                return string.Format("'{0}'", StringUtils.Join<string>((IEnumerable<string>)value));
            }
            if (result.Length == 0)
            {
                throw new Exception("数据类型映射为数据库类型时错误");
            }
            return result;
        }

        /// <summary>
        /// 返回一个为字符串两边加上%的字符串
        /// </summary>
        /// <param name="value">原始字符串</param>
        /// <returns>格式化后的字符串</returns>
        public string ToLikeValue(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            return string.Format("'%{0}%'", value.Replace("'", "''"));
        }

        /// <summary>
        /// 把一个整型列表创建一个IN表达式
        /// </summary>
        /// <param name="colume">列名</param>
        /// <param name="length">IN表达式的最大长度(如Oracle最大长度为8000)</param>
        /// <param name="value">IN表达式的值列表</param>
        /// <returns>创建后的IN表达式</returns>
        public string BuildInSql(string colume, int length, IList<int> value)
        {
            if (length < 1)
            {
                throw new Exception("IN表达式的长度必须大于0");
            }
            if (ASoft.Text.StringUtils.IsNullOrWhiteSpace(colume))
            {
                throw new Exception("列名不能为空.");
            }
            if (value == null || value.Count == 0)
            {
                throw new Exception("要合并的列表长度不能为0.");
            }
            StringBuilder sb = new StringBuilder("(");
            string join = ASoft.Text.StringUtils.Join(",", value, 0, length);
            sb.AppendFormat("{0} in ({1})", colume, join);
            if (length < value.Count)
            {
                for (int i = length; i < value.Count; i += length)
                {
                    join = ASoft.Text.StringUtils.Join(",", value, i, length);
                    sb.AppendFormat(" OR {0} in ({1})", colume, join);
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// 把一个整型列表创建一个IN表达式
        /// </summary>
        /// <param name="colume">列名</param>
        /// <param name="length">IN表达式的最大长度(如Oracle最大长度为8000)</param>
        /// <param name="value">IN表达式的值列表</param>
        /// <returns>创建后的IN表达式</returns>
        public string BuildInSql(string colume, int length, IList<long> value)
        {
            if (length < 1)
            {
                throw new Exception("IN表达式的长度必须大于0");
            }
            if (ASoft.Text.StringUtils.IsNullOrWhiteSpace(colume))
            {
                throw new Exception("列名不能为空.");
            }
            if (value == null || value.Count == 0)
            {
                throw new Exception("要合并的列表长度不能为0.");
            }
            StringBuilder sb = new StringBuilder("(");
            string join = StringUtils.Join(",", value, 0, length);
            sb.AppendFormat("{0} in ({1})", colume, join);
            if (length < value.Count)
            {
                for (int i = length; i < value.Count; i += length)
                {
                    join = StringUtils.Join(",", value, i, length);
                    sb.AppendFormat(" OR {0} in ({1})", colume, join);
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// 把一个字符串列表创建一个IN表达式
        /// </summary>
        /// <param name="colume">列名</param>
        /// <param name="length">IN表达式的最大长度(如Oracle最大长度为8000)</param>
        /// <param name="value">IN表达式的值列表</param>
        /// <returns>创建后的IN表达式</returns>
        public string BuildInSql(string colume, int length, IList<string> value)
        {
            if (length < 1)
            {
                throw new Exception("IN表达式的长度必须大于0");
            }
            if (string.IsNullOrWhiteSpace(colume))
            {
                throw new Exception("列名不能为空.");
            }
            if (value == null || value.Count == 0)
            {
                throw new Exception("要合并的列表长度不能为0.");
            }
            StringBuilder sb = new StringBuilder("(");
            string join = StringUtils.Join(",", value, 0, length, delegate (string s) { return ToSqlValue(s); });
            sb.AppendFormat("{0} in ({1})", colume, join);
            if (length < value.Count)
            {
                for (int i = length; i < value.Count; i += length)
                {
                    join = StringUtils.Join(",", value, i, length, delegate (string s) { return ToSqlValue(s); });
                    sb.AppendFormat(" OR {0} in ({1})", colume, join);
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
        #endregion

        #region 执行SQL脚本文件
        /// <summary>
        /// 执行SQL脚本文件
        /// </summary>
        /// <param name="path">脚本文件路径</param>
        public void ExecuteFile(string path)
        {
            using (C connection = this.CreateConnection() as C)
            {
                ExecuteFile(connection, path);
            }
        }

        /// <summary>
        /// 执行SQL脚本文件
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="path">脚本文件路径</param>
        public void ExecuteFile(C connection, string path)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new Exception("文件未找到,文件路径:" + path);
            }
            using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
            {
                bool connopen = connection.State == ConnectionState.Open;
                IDbCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 60;
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    while (!reader.EndOfStream)
                    {
                        string sql = DbTools.GetNextSql(reader, this.provider);
                        if (!string.IsNullOrEmpty(sql))
                        {
                            command.CommandText = sql;
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    DbTools.WriteDbException(ex, command);
                    throw ex;
                }
                finally
                {
                    reader.Close();
                    if (!connopen)
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region 返回数据库架构信息

        /// <summary>
        /// 返回数据库的架构信息
        /// </summary>
        /// <returns>数据库的架构信息</returns>
        public DataTable GetSchema()
        {
            using (C connection = this.CreateConnection() as C)
            {
                if (connection is DbConnection)
                {
                    connection.Open();
                    return (connection as DbConnection).GetSchema();
                }
                throw new Exception("不支持当前DbConnection类型,具体类型为:" + connection.GetType().FullName);
            }
        }

        /// <summary>
        /// 返回数据库的架构信息
        /// </summary>
        /// <param name="collectionName">要返回的字段</param>
        /// <returns>数据库的架构信息</returns>
        public DataTable GetSchema(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                return GetSchema();
            }
            return GetSchema(collectionName, null);
        }

        /// <summary>
        /// 返回数据库的架构信息
        /// </summary>
        /// <param name="collectionName">要返回的字段</param>
        /// <param name="restrictionValues">要返回的字段的约束</param>
        /// <returns>数据库的架构信息</returns>
        public DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            using (C connection = this.CreateConnection() as C)
            {
                if (connection is DbConnection)
                {
                    connection.Open();
                    return (connection as DbConnection).GetSchema(collectionName, restrictionValues);
                }
                throw new Exception("不支持当前DbConnection类型,具体类型为:" + connection.GetType().FullName);
            }
        }
        #endregion

        #region 获取所有的表
        /// <summary>
        /// 获取所有的表
        /// </summary>
        /// <returns></returns>
        public List<string> Tables
        {
            get
            {
                List<string> result = new List<string>();
                using (ASoft.Db.DataReader dr = ExecuteReader(AllTableSql))
                {
                    while (dr.Read())
                    {
                        result.Add(dr.GetString(0).Trim().ToUpper());
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 获取系统所有表的语句
        /// </summary>
        protected virtual string AllTableSql
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region 获取所有存储过程

        /// <summary>
        /// 获取所有的存储过程
        /// </summary>
        public List<string> Procedures
        {
            get
            {
                List<string> result = new List<string>();
                using (ASoft.Db.DataReader dr = ExecuteReader(AllProcedureSql))
                {
                    while (dr.Read())
                    {
                        result.Add(dr.GetString(0).Trim().ToUpper());
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 获取所有存储过程的SQL
        /// </summary>
        protected virtual string AllProcedureSql
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 每次调用SetObjectProperty方法时都将调用redis缓存，如果没有则加入缓存（存储和获取记录集时都需要迭代单条的获取并反序列化，因此有效率问题）
        /// </summary>
        public ICacheable CacheAccess
        {
            set; get;
        }

        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行数据库存储过程,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据表</returns>
        public DataTableResult ExecuteProcDataTable(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            if (pvs != null)
            {
                for (int i = 0; i < pvs.Length && i < ps.Length; i++)
                {
                    pvs[i].Value = ps[i];
                }
            }
            return ExecuteDataTable(commandText, CommandType.StoredProcedure, pvs);
        }


        /// <summary>
        /// 执行数据库存储过程,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据集</returns>
        public DataSetResult ExecuteProcDataSet(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteDataSet(commandText, CommandType.StoredProcedure, pvs);
        }



        /// <summary>
        /// 执行数据库存储过程,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回受影响的行数</returns>
        public NonQueryResult ExecuteProcNonQuery(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteNonQuery(commandText, CommandType.StoredProcedure, pvs);
        }


        /// <summary>
        /// 执行数据库存储过程,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回结果的第一行第一列</returns>
        public ScalerResult ExecuteProcScalar(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteScalar(commandText, CommandType.StoredProcedure, pvs);
        }


        /// <summary>
        /// 执行数据库存储过程,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据读取器</returns>
        public DataReader ExecuteProcReader(string commandText, params object[] ps)
        {
            IDbDataParameter[] pvs = GrabParameters(commandText);
            if (pvs == null)
            {
                pvs = GetProcParameters(commandText);
            }
            for (int i = 0; i < pvs.Length && i < ps.Length; i++)
            {
                pvs[i].Value = ps[i];
            }
            return ExecuteReader(commandText, CommandType.StoredProcedure, pvs);
        }
        #endregion

        #region

        /// <summary>
        /// SQL参数化
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataCommand CreateSQLCommand(String sql, params object[] args)
        {
            DataCommandBuilder commandBuilder = new DataCommandBuilder(sql, this, args);
            return commandBuilder.CreateCommand();
        }

        #endregion

        #region 实体插入更新删除获取
        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public void InsertObject(ASoft.Model.BaseModel m)
        {
            ExecuteNonQuery(GetCommandByInsert(m));
        }

        public DataCommand GetInsertSql(ASoft.Model.BaseModel m)
        {
            EntityInfo ea = EntityHelper.GetEntityAttribute(m);
            StringBuilder sbvalue = new StringBuilder("(");
            bool hasfield = false;
            List<P> parameters = null;
            foreach (var item in ea.EntityProperty)
            {
                if (item.Value.PropertyAttribute.GenerateValue)
                {
                    object genid = null;
                    //如果设置了序列名
                    if (item.Value.PropertyAttribute.SeqName != String.Empty)
                    {
                        genid = this.SequenceGenerator[item.Value.PropertyAttribute.SeqName].Next(9);
                    }
                    else
                    {
                        genid = this.SequenceGenerator[string.Format("{0}_{1}", ea.EntityName, item.Key)].Next(9);
                    }


                    if (item.Value.PropertyInfo.PropertyType == typeof(string))
                    {
                        genid = genid.ToString();
                    }
                    else if (item.Value.PropertyInfo.PropertyType == typeof(int))
                    {
                        genid = Convert.ToInt32(genid);
                    }

                    item.Value.PropertyInfo.SetValue(m, genid, null);
                }
                if ((item.Value.PropertyAttribute.FieldFlags & DataFieldFlags.Insert) > 0)
                {
                    sbvalue.Append(hasfield ? ", " : string.Empty);
                    string value = null;
                    if (typeof(byte[]) == item.Value.PropertyInfo.PropertyType)
                    {
                        value = this.parameterPrefix + item.Value.PropertyAttribute.Field;
                        P p = new P();
                        p.DbType = DbType.Binary;
                        p.ParameterName = value;
                        p.Value = item.Value.PropertyInfo.GetValue(m, null);
                        if (parameters == null)
                        {
                            parameters = new List<P>();
                        }
                        parameters.Add(p);
                    }
                    else
                    {
                        value = ToSqlValue(item.Value.PropertyInfo.GetValue(m, null));
                        if (value == "NULL" && !string.IsNullOrWhiteSpace(item.Value.PropertyAttribute.DefaultExpression))
                        {
                            value = item.Value.PropertyAttribute.DefaultExpression;
                        }
                    }
                    if (value == null)
                    {
                        throw new Exception(string.Format("插入数据对象时,数据类型转换错误,类型:{0},属性:{1}", ea.EntityName, item.Key));
                    }
                    sbvalue.Append(value);
                    hasfield = true;
                }
            }
            if (!hasfield)
            {
                throw new Exception(string.Format("没有为{0}类型的实体设置要插入数据库的字段", m.GetType().FullName));
            }
            sbvalue.Append(")");
            return new DataCommand(ea.InsertSql + sbvalue.ToString(), CommandType.Text, parameters == null ? null : parameters.ToArray());
        }


        public DataCommand GetCommandByInsert(ASoft.Model.BaseModel m)
        {
            EntityInfo ea = EntityHelper.GetEntityAttribute(m);
            StringBuilder sbfield = new StringBuilder("insert into " + EntityHelper.GetEntityAttribute(m).DataTable + "(");
            StringBuilder sbvalue = new StringBuilder("(");
            bool hasfield = false;
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            foreach (var item in ea.EntityProperty)
            {
                if (item.Value.PropertyAttribute.GenerateValue)
                {
                    object genid = null;
                    //如果设置了序列名
                    if (item.Value.PropertyAttribute.SeqName != String.Empty)
                    {
                        genid = this.SequenceGenerator[item.Value.PropertyAttribute.SeqName].Next(9);
                    }
                    else
                    {
                        genid = this.SequenceGenerator[string.Format("{0}_{1}", ea.EntityName, item.Key)].Next(9);
                    }

                    if (item.Value.PropertyInfo.PropertyType == typeof(string))
                    {
                        genid = genid.ToString();
                    }
                    else if (item.Value.PropertyInfo.PropertyType == typeof(int))
                    {
                        genid = Convert.ToInt32(genid);
                    }

                    item.Value.PropertyInfo.SetValue(m, genid, null);
                }
                if ((item.Value.PropertyAttribute.FieldFlags & DataFieldFlags.Insert) > 0)
                {
                    var value = item.Value.PropertyInfo.GetValue(m, null);
                    if (string.IsNullOrWhiteSpace(item.Value.PropertyAttribute.DefaultExpression) && value != null)
                    {
                        sbvalue.Append(hasfield ? ", " : string.Empty);
                        sbfield.Append(hasfield ? "," : String.Empty);
                        if (value == null)
                        {
                            value = "";
                            // sbfield.Append(hasfield ? "," : String.Empty);
                            //throw new Exception(string.Format("插入数据对象时,数据类型转换错误,类型:{0},属性:{1}", ea.EntityName, item.Key));
                        }
                        string paramName = null;
                        paramName = this.parameterPrefix + item.Value.PropertyAttribute.Field;
                        IDbDataParameter param = null;

                        if (item.Value.PropertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                        {
                            Type myType = item.Value.PropertyInfo.PropertyType.Assembly.GetType(item.Value.PropertyInfo.PropertyType.FullName);
                            int myValue = (int)Enum.Parse(myType, item.Value.PropertyInfo.GetValue(m, null).ToString());
                            param = this.MakeIn(paramName, myValue);
                        }
                        else
                        {
                            param = this.MakeIn(paramName, item.Value.PropertyInfo.GetValue(m, null));
                        }
                        parameters.Add(param);
                        sbfield.Append(item.Value.PropertyAttribute.Field);
                        sbvalue.Append(paramName);
                    }
                    hasfield = true;
                }
            }
            if (!hasfield)
            {
                throw new Exception(string.Format("没有为{0}类型的实体设置要插入数据库的字段", m.GetType().FullName));
            }

            sbfield.Append(") values ");
            sbvalue.Append(")");
            return new DataCommand(sbfield.ToString() + sbvalue.ToString(), CommandType.Text, parameters == null ? null : parameters.ToArray());
        }
        public String GetSqlByInsert(ASoft.Model.BaseModel m)
        {
            return this.GetInsertSql(m).CommandText;
        }

        public String GetSqlByDelete(ASoft.Model.BaseModel m)
        {
            EntityInfo ea = EntityHelper.GetEntityAttribute(m);
            StringBuilder sbsql = new StringBuilder("DELETE FROM " + ea.DataTable + " WHERE");
            bool hasfield = false;

            foreach (var item in ea.EntityProperty)
            {
                if (item.Value.PropertyAttribute.IsPrimaryKey)
                {
                    sbsql.Append(hasfield ? " AND " : " ");
                    sbsql.Append(item.Value.PropertyAttribute.Field);
                    sbsql.Append(" = ");
                    sbsql.Append(ToSqlValue(item.Value.PropertyInfo.GetValue(m, null)));
                    hasfield = true;
                }
            }
            if (!hasfield)
            {
                throw new Exception(string.Format("没有为{0}类型的实体设置关键字,无法按照关键字进行删除", m.GetType().FullName));
            }
            return sbsql.ToString();
        }

        public DataCommand GetCommandByDelete(ASoft.Model.BaseModel m)
        {
            EntityInfo ea = EntityHelper.GetEntityAttribute(m);
            StringBuilder sbsql = new StringBuilder("DELETE FROM " + ea.DataTable + " WHERE");
            bool hasfield = false;
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            foreach (var item in ea.EntityProperty)
            {
                if (item.Value.PropertyAttribute.IsPrimaryKey)
                {
                    sbsql.Append(hasfield ? " AND " : " ");

                    sbsql.Append(item.Value.PropertyAttribute.Field);
                    sbsql.Append(" = ");
                    String paramName = this.parameterPrefix + item.Value.PropertyAttribute.Field;
                    sbsql.Append(paramName);
                    IDbDataParameter param = this.MakeIn(paramName, item.Value.PropertyInfo.GetValue(m, null));
                    parameters.Add(param);
                    hasfield = true;
                }
            }
            if (!hasfield)
            {
                throw new Exception(string.Format("没有为{0}类型的实体设置关键字,无法按照关键字进行删除", m.GetType().FullName));
            }
            return new DataCommand(sbsql.ToString(), CommandType.Text, parameters == null ? null : parameters.ToArray());
        }

        public String GetSqlByUpdate(ASoft.Model.BaseModel m)
        {
            if (m.Updates == null)
            {
                return "";
            }
            EntityInfo ea = EntityHelper.GetEntityAttribute(m);
            StringBuilder sbUpd = new StringBuilder("UPDATE " + ea.DataTable + " SET ");
            bool hasfield = false;
            List<P> parameters = null;
            foreach (var proName in m.Updates)
            {
                EntityPropertyInfo epi = ea.EntityProperty[proName];
                if ((epi.PropertyAttribute.FieldFlags & DataFieldFlags.Update) > 0)
                {
                    sbUpd.Append(hasfield ? ", " : string.Empty);
                    sbUpd.Append(epi.PropertyAttribute.Field);
                    sbUpd.Append(" = ");
                    string value = null;
                    if (typeof(byte[]) == ea.EntityProperty[proName].PropertyInfo.PropertyType || typeof(char[]) == ea.EntityProperty[proName].PropertyInfo.PropertyType)
                    {
                        value = this.parameterPrefix + epi.PropertyAttribute.Field;
                        P p = new P();
                        p.DbType = DbType.Binary;
                        p.ParameterName = value;
                        p.Value = epi.PropertyInfo.GetValue(m, null);
                        if (parameters == null)
                        {
                            parameters = new List<P>();
                        }
                        parameters.Add(p);
                    }
                    else
                    {
                        value = ToSqlValue(ea.EntityProperty[proName].PropertyInfo.GetValue(m, null));
                        if (value == "NULL" && !string.IsNullOrWhiteSpace(ea.EntityProperty[proName].PropertyAttribute.DefaultExpression))
                        {
                            value = epi.PropertyAttribute.DefaultExpression;
                        }
                    }
                    sbUpd.Append(value);
                    hasfield = true;

                    if (value == null)
                    {
                        throw new Exception(string.Format("插入数据对象时,数据类型转换错误,类型:{0},属性:{1}", ea.EntityName, proName));
                    }
                }
            }
            sbUpd.Append(" WHERE ");
            hasfield = false;
            foreach (var item in ea.EntityProperty)
            {
                if (item.Value.PropertyAttribute.IsPrimaryKey)
                {
                    sbUpd.Append(hasfield ? " AND " : " ");
                    sbUpd.Append(item.Value.PropertyAttribute.Field);
                    sbUpd.Append(" = ");
                    sbUpd.Append(ToSqlValue(item.Value.PropertyInfo.GetGetMethod().Invoke(m, null)));
                    hasfield = true;
                }
            }
            if (!hasfield)
            {
                throw new Exception(string.Format("没有为{0}类型的实体设置关键字,无法按照关键字进行更新", m.GetType().FullName));
            }
            return sbUpd.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public DataCommand GetCommandByUpdate(ASoft.Model.BaseModel m)
        {
            if (m.Updates == null)
            {
                return null;
            }
            EntityInfo ea = EntityHelper.GetEntityAttribute(m);
            StringBuilder sbUpd = new StringBuilder("UPDATE " + ea.DataTable + " SET ");
            bool hasfield = false;
            List<IDbDataParameter> parameters = null;
            foreach (var proName in m.Updates)
            {

                EntityPropertyInfo epi = ea.EntityProperty[proName];
                if ((epi.PropertyAttribute.FieldFlags & DataFieldFlags.Update) > 0)
                {
                    if (parameters == null)
                    {
                        parameters = new List<IDbDataParameter>();
                    }
                    sbUpd.Append(hasfield ? ", " : string.Empty);
                    sbUpd.Append(epi.PropertyAttribute.Field);
                    sbUpd.Append(" = ");
                    String paramName = this.parameterPrefix + epi.PropertyAttribute.Field;
                    sbUpd.Append(paramName);
                    IDbDataParameter param = null;
                    if (epi.PropertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        Type myType = epi.PropertyInfo.PropertyType.Assembly.GetType(epi.PropertyInfo.PropertyType.FullName);
                        int myValue = (int)Enum.Parse(myType, epi.PropertyInfo.GetValue(m, null).ToString());
                        param = this.MakeIn(paramName, myValue);
                    }
                    else
                    {
                        param = this.MakeIn(paramName, epi.PropertyInfo.GetValue(m, null));
                    }
                    parameters.Add(param);
                    hasfield = true;
                }
            }
            sbUpd.Append(" WHERE ");
            hasfield = false;
            foreach (var item in ea.EntityProperty)
            {
                if (item.Value.PropertyAttribute.IsPrimaryKey)
                {
                    sbUpd.Append(hasfield ? " AND " : " ");
                    sbUpd.Append(item.Value.PropertyAttribute.Field);
                    sbUpd.Append(" = ");
                    sbUpd.Append(ToSqlValue(item.Value.PropertyInfo.GetGetMethod().Invoke(m, null)));
                    hasfield = true;
                }
            }
            if (!hasfield)
            {
                throw new Exception(string.Format("没有为{0}类型的实体设置关键字,无法按照关键字进行更新", m.GetType().FullName));
            }

            return new DataCommand(sbUpd.ToString(), CommandType.Text, parameters == null ? null : parameters.ToArray());
        }

        /// <summary>
        /// 更新一个实体
        /// </summary>
        /// <param name="m"></param>
        public void UpdateObject(ASoft.Model.BaseModel m)
        {
            var updateCommand = this.GetCommandByUpdate(m);
            this.ExecuteNonQuery(updateCommand);
        }


        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="m"></param>
        public void DeleteObject(ASoft.Model.BaseModel m)
        {
            String sql = this.GetSqlByDelete(m);
            ExecuteNonQuery(sql);
        }

        public void LoadObject<TSource>(TSource m)
            where TSource : ASoft.Model.BaseModel, new()
        {
            EntityInfo ea = EntityHelper.GetEntityAttribute(m);
            var key = m.GetUniqueID();
            loadCache(key, () => loadMyObject(m, ea));
        }

        protected TSource loadCache<TSource>(string key, Func<TSource> loadData)
             where TSource : ASoft.Model.BaseModel, new()
        {
            if (CacheAccess != null)
            {
                var result = CacheAccess.Get<TSource>(key, loadData);
                return result;
            }
            else
            {
                return loadData();
            }

        }

        protected TSource loadCache<TSource>(TSource m, DataReader dr, Func<TSource> loadData)
             where TSource : ASoft.Model.BaseModel, new()
        {
            if (CacheAccess != null)
            {
                string key = "";

                var identifierField = m.GetIdentifierField();
                if (identifierField != null && dr != null)
                {
                    var identifierFieldValue = dr[identifierField] != null ? dr[identifierField].ToString() : "";
                    key = m.GetUniqueID(identifierFieldValue);
                }
                var result = CacheAccess.Get<TSource>(key, loadData);
                return result;
            }
            else
            {
                return loadData();
            }

        }

        protected TSource loadMyObject<TSource>(TSource m, EntityInfo ea)
            where TSource : ASoft.Model.BaseModel, new()
        {
            StringBuilder sbsql = new StringBuilder("SELECT * FROM " + ea.DataTable + " WHERE");
            bool hasfield = false;
            foreach (var item in ea.EntityProperty)
            {
                if (item.Value.PropertyAttribute.IsPrimaryKey)
                {
                    sbsql.Append(hasfield ? " AND " : " ");
                    sbsql.Append(item.Value.PropertyAttribute.Field);
                    sbsql.Append(" = ");
                    sbsql.Append(ToSqlValue(item.Value.PropertyInfo.GetGetMethod().Invoke(m, null)));
                    hasfield = true;
                }
            }
            if (!hasfield)
            {
                throw new Exception(string.Format("没有为{0}类型的实体设置关键字,无法按照关键字进行查找", m.GetType().FullName));
            }
            using (ASoft.Db.DataReader dr = ExecuteReader(sbsql.ToString()))
            {
                if (dr.Read())
                {
                    m = SetObjectProperty<TSource>(dr);
                }
                else
                {
                    m = default(TSource);
                }
            }
            return m;
        }

        public TSource SetObjectProperty<TSource>(ASoft.Db.DataReader dr)
            where TSource : ASoft.Model.BaseModel, new()
        {
            var model = new TSource();
            var key = model.GetUniqueID(dr[model.GetIdentifierField()].ToString());
            return loadCache<TSource>(key, () => SetObjectProperty(model, dr));
        }

        public TSource SetObjectProperty<TSource>(TSource model, ASoft.Db.DataReader dr)
         where TSource : ASoft.Model.BaseModel, new()
        {
            if (model == null)
            {
                model = new TSource();
            }
            EntityInfo ea = EntityHelper.GetEntityAttribute(model);
            if (ea != null)
            { 
                foreach (var item in ea.EntityProperty)
                {
                    if (!dr.Fields.Contains(item.Value.PropertyAttribute.Field))
                    {
                        continue;
                    }

                    if ((item.Value.PropertyAttribute.FieldFlags & DataFieldFlags.Select) > 0)
                    {
                        object value = ToEntityValue(item.Value.PropertyInfo.PropertyType, dr[item.Value.PropertyAttribute.Field]);
                        if (value != null)
                        {
                            item.Value.PropertyInfo.SetValue(model, value, null);
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 把数据库的数据转换为实体的数据
        /// </summary>
        /// <param name="type">实体的数据类型</param>
        /// <param name="data">数据库字段的值</param>
        /// <returns>实体的数据</returns>
        protected virtual object ToEntityValue(Type type, object data)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (Convert.IsDBNull(data) || data == null)
                {
                    return null;
                }
                type = new System.ComponentModel.NullableConverter(type).UnderlyingType;
            }
            if (type.IsEnum)
            {
                return System.Enum.Parse(type, data.ToString());
            }
            if (data is System.Guid)
            {
                if (type.FullName == "System.String")
                {
                    return Convert.ChangeType(((Guid)data).ToString(), type);
                }
            }
            if (Convert.IsDBNull(data) || data == null)
            {
                return null;
            }

            #region Array
            if (type.IsArray && type == typeof(char[]))
            {
                char[] result = Convert.ChangeType(data, typeof(String)).ToString().ToCharArray();
                return result;
            }
            if ((type.IsArray && type != typeof(byte[])) || (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(List<>))))
            {
                if (type == typeof(int[]) || type == typeof(List<int>))
                {
                    List<int> result = StringUtils.SplitIntNumber(data.ToString());
                    if (type == typeof(int[]))
                    {
                        return result.ToArray();
                    }
                    return result;
                }
                if (type == typeof(long[]) || type == typeof(List<long>))
                {
                    List<long> result = StringUtils.SplitLongNumber(data.ToString());
                    if (type == typeof(long[]))
                    {
                        return result.ToArray();
                    }
                    return result;
                }
                if (type == typeof(string[]) || type == typeof(List<string>))
                {
                    List<string> result = StringUtils.SplitStrictString(data.ToString());
                    if (type == typeof(string[]))
                    {
                        return result.ToArray();
                    }
                    return result;
                }
                if (type == typeof(char[]))
                {
                    char[] result = null;
                    if (data != null)
                    {
                        result = data.ToString().ToCharArray();
                    }
                    return result;
                }

            }
            #endregion
            return Convert.ChangeType(data, type);
        }
        #endregion
       
    }

    /// <summary>
    /// 指示要执行的数据库命令的数据库连接对象是来自于外部还是内部生成的
    /// </summary>
    public enum ConnOwnerShip
    {
        /// <summary>
        /// 当DataReader关闭时,自动关闭数据连接对象
        /// </summary>
        Internal,

        /// <summary>
        /// 当DataReader关闭时,不关闭数据连接对象
        /// </summary>
        External
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbProvider
    {
        /// <summary>
        /// 未知数据库类型
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// ODBC数据源
        /// </summary>
        Odbc = 1,

        /// <summary>
        /// OLEDB数据源
        /// </summary>
        OleDb = 2,

        /// <summary>
        /// SQLSERVER数据库
        /// </summary>
        SqlServer = 3,

        /// <summary>
        /// Oracle数据库
        /// </summary>
        Oracle = 4,

        /// <summary>
        /// Advantage数据库
        /// </summary>
        Ads = 5,

        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql = 6,

        /// <summary>
        /// Postgre数据库
        /// </summary>
        PostgreSQL = 7,

        /// <summary>
        /// SQLite数据库
        /// </summary>
        Sqlite = 8,

        /// <summary>
        /// Firebird数据库
        /// </summary>
        FireBird = 9,

        /// <summary>
        /// IBM DB2数据库
        /// </summary>
        DB2 = 10,

        /// <summary>
        /// Infomix数据库
        /// </summary>
        Informix = 11,

        /// <summary>
        /// Sybase数据库
        /// </summary>
        Sybase = 12,

        /// <summary>
        /// SQL嵌入式数据库
        /// </summary>
        SqlCe = 13,

        /// <summary>
        /// VistaDB数据库
        /// </summary>
        VistaDB = 14
    }
}
