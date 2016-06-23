using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using System.IO;
using ASoft.Text;
using System.Text.RegularExpressions;

namespace ASoft.Db
{
    /// <summary>
    /// 数据库工具类
    /// </summary>
    public static class DbTools
    {
        /// <summary>
        /// 为Command命令添加相关的信息
        /// </summary>
        /// <param name="command">原始的Command命令</param>
        /// <param name="conn">数据库连接</param>
        /// <param name="tran">事务</param>
        /// <param name="ct">Command命令的类型</param>
        /// <param name="txt">Command命令的文本</param>
        /// <param name="cps">Command命令的参数</param>
        public static void PrepareCommand(IDbCommand command, IDbConnection conn, IDbTransaction tran, CommandType ct, string txt, IDataParameter[] cps)
        {
            command.Connection = conn;
            command.CommandText = txt;
            command.CommandType = ct;
            if (tran != null)
            {
                if (tran.Connection == null) throw new ArgumentException("事务已经回滚或提交.", "transaction");
                command.Transaction = tran;
            }
            if (cps != null && cps.Length > 0)
            {
                foreach (IDataParameter p in cps)
                {
                    if (p != null && (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }
                    command.Parameters.Add(p);
                }
            }
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    ASoft.LogAdapter.Db.Error(ex);
                    throw ex;
                }
            }
        }



        /// <summary>
        /// 记录数据访问异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="cmd">执行的数据库命令</param>
        public static void WriteDbException(Exception ex, IDbCommand cmd)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(string.Format("数据命令类型:{0}", cmd.CommandType));
            sb.AppendLine(string.Format("数据命令文本:{0}", cmd.CommandText));
            if (cmd.Parameters.Count > 0)
            {
                sb.AppendLine("数据命令参数:");
                int max = 0;
                foreach (IDataParameter p in cmd.Parameters)
                {
                    max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                }
                foreach (IDataParameter p in cmd.Parameters)
                {
                    sb.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                }
            }
            LogAdapter.Db.Error(ASoft.LogFileSpan.Day, string.Format("错误位置:{1}.{2}{0}错误描述:{0}{3}{4}{5}",
                Environment.NewLine,
                ex.TargetSite.ReflectedType.FullName,
                ex.TargetSite.Name,
                ex.Message,
                ex.Message.EndsWith(Environment.NewLine) ? string.Empty : Environment.NewLine,
                sb));
        }

        /// <summary>
        /// 根据数据库连接对象的类型获取数据库类型
        /// </summary>
        /// <param name="connType">数据库连接对象类型</param>
        /// <returns>数据库类型</returns>
        public static DbProvider GetDbProvider(Type connType)
        {
            if (connType.FullName.Contains("SqlConnection"))
            {
                return DbProvider.SqlServer;
            }
            if (connType.FullName.Contains("AdsConnection"))
            {
                return DbProvider.Ads;
            }
            if (connType.FullName.Contains("DB2Connection"))
            {
                return DbProvider.DB2;
            }
            if (connType.FullName.Contains("FbConnection"))
            {
                return DbProvider.FireBird;
            }
            if (connType.FullName.Contains("IfxConnection"))
            {
                return DbProvider.Informix;
            }
            if (connType.FullName.Contains("MySqlConnection"))
            {
                return DbProvider.MySql;
            }
            if (connType.FullName.Contains("NpgsqlConnection"))
            {
                return DbProvider.PostgreSQL;
            }
            if (connType.FullName.Contains("OdbcConnection"))
            {
                return DbProvider.Odbc;
            }
            if (connType.FullName.Contains("OleDbConnection"))
            {
                return DbProvider.OleDb;
            }
            if (connType.FullName.Contains("OracleConnection"))
            {
                return DbProvider.Oracle;
            }
            if (connType.FullName.Contains("SqlCeConnection"))
            {
                return DbProvider.SqlCe;
            }
            if (connType.FullName.Contains("SQLiteConnection") || connType.FullName.Contains("SqliteConnection"))
            {
                return DbProvider.Sqlite;
            }
            if (connType.FullName.Contains("AseConnection"))
            {
                return DbProvider.Sybase;
            }
            if (connType.FullName.Contains("VistaDBConnection"))
            {
                return DbProvider.VistaDB;
            }
            return DbProvider.Unknown;
        }


        /// <summary>
        /// 根据数据库连接对象的类型获取数据库类型
        /// </summary>
        /// <param name="connType">数据库连接名称</param>
        /// <returns>数据库类型</returns>
        public static DbProvider GetDbProvider(String connType)
        {
            if (connType.Contains("SqlConnection"))
            {
                return DbProvider.SqlServer;
            }
            if (connType.Contains("AdsConnection"))
            {
                return DbProvider.Ads;
            }
            if (connType.Contains("DB2Connection"))
            {
                return DbProvider.DB2;
            }
            if (connType.Contains("FbConnection"))
            {
                return DbProvider.FireBird;
            }
            if (connType.Contains("IfxConnection"))
            {
                return DbProvider.Informix;
            }
            if (connType.Contains("MySqlConnection"))
            {
                return DbProvider.MySql;
            }
            if (connType.Contains("NpgsqlConnection"))
            {
                return DbProvider.PostgreSQL;
            }
            if (connType.Contains("OdbcConnection"))
            {
                return DbProvider.Odbc;
            }
            if (connType.Contains("OleDbConnection"))
            {
                return DbProvider.OleDb;
            }
            if (connType.Contains("OracleConnection"))
            {
                return DbProvider.Oracle;
            }
            if (connType.Contains("SqlCeConnection"))
            {
                return DbProvider.SqlCe;
            }
            if (connType.Contains("SQLiteConnection") || connType.Contains("SqliteConnection"))
            {
                return DbProvider.Sqlite;
            }
            if (connType.Contains("AseConnection"))
            {
                return DbProvider.Sybase;
            }
            if (connType.Contains("VistaDBConnection"))
            {
                return DbProvider.VistaDB;
            }
            return DbProvider.Unknown;
        }


        /// <summary>
        /// 为SQL查询条件生成分页查询SQL语句
        /// </summary>
        /// <param name="commandText">查询条件</param>
        /// <param name="provider">数据库类型</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="start">返回的查询结果要跳过的行数</param>
        /// <param name="limit">返回记录集的长度</param>
        /// <returns></returns>
        public static string CreatePageSql(String commandText, DbProvider provider,   int start, int limit)
        {
            String sql = "";
            int end = start + limit;
            switch (provider)
            {
                case DbProvider.PostgreSQL:
                case DbProvider.Sqlite:
                    {
                        #region Sqlite 
                        break;
                        #endregion
                    }
                case DbProvider.Oracle:
                    {
                        #region Oracle
                        sql = String.Format("select * from (SELECT * FROM (SELECT ROWNUM AS RN, AA.* FROM ({0}) AA  ) WHERE RN>{1}) where RN<={2}", commandText,  start, end);
                        break;
                        #endregion
                    }
                case DbProvider.MySql:
                    {
                        #region MySql

                        break;
                        #endregion
                    }
                case DbProvider.SqlServer:
                    {
                        #region SqlServer

                        break;
                        #endregion
                    }
                case DbProvider.SqlCe:
                case DbProvider.OleDb:
                case DbProvider.VistaDB:
                    {
                        #region OleDb SqlCe VistaDB
                        break;
                        #endregion
                    }
                case DbProvider.FireBird:
                    {
                        #region FireBird
                        break;
                        #endregion
                    }
                case DbProvider.DB2:
                    {
                        #region DB2
                         break;
                        #endregion
                    }
                case DbProvider.Informix:
                    {
                        #region Informix
                        break;
                        #endregion
                    }
                case DbProvider.Ads:
                case DbProvider.Sybase:
                    {
                        #region
                        break;
                        #endregion
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        RETURN:
            return sql;
        }



        /// <summary>
        /// 为SQL查询条件生成分页查询SQL语句
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="provider">数据库类型</param>
        /// <param name="version">数据库版本</param>
        /// <param name="skip">使用这个命令返回的查询结果要跳过的行数</param>
        /// <returns>生成的SQL查询命令</returns>
        public static string CreatePageSql(PageSearch search, DbProvider provider, Version version, out int skip)
        {
            if (search.PageIndex > search.PageCount)
            {
                search.PageIndex = search.PageCount - 1;
            }
            StringBuilder sbcmd = search.executeSql;
            skip = 0;
            if (string.IsNullOrEmpty(search.TableName) || search.TableName.Trim().Length == 0)
            {
                sbcmd.Append("SELECT ");
                sbcmd.Append(search.FieldsName);
                goto RETURN;
            }
            if (search.PageSize == 0)
            {
                sbcmd.Append("SELECT ");
                sbcmd.Append(search.FieldsName);
                sbcmd.Append(" FROM ");
                sbcmd.Append(search.TableName);
                if (!string.IsNullOrEmpty(search.Where))
                {
                    sbcmd.Append(" WHERE ");
                    sbcmd.Append(search.Where);
                }
                if (search.OrderField.Count > 0)
                {
                    sbcmd.Append(search.OrderField);
                }
                goto RETURN;
            }
            switch (provider)
            {
                case DbProvider.PostgreSQL:
                case DbProvider.Sqlite:
                    {
                        #region Sqlite
                        // select .. from .. where .. order by .. limit .. offset 
                        sbcmd.Append("SELECT ");
                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }
                        if (search.OrderField.Count > 0)
                        {
                            sbcmd.Append(search.OrderField);
                        }
                        sbcmd.Append(" LIMIT ");
                        sbcmd.Append(search.PageSize);
                        sbcmd.Append(" OFFSET ");
                        sbcmd.Append(search.PageSize * (search.PageIndex - 1));
                        break;
                        #endregion
                    }
                case DbProvider.Oracle:
                    {
                        #region Oracle
                        if (search.PageIndex > 0)
                        {
                            sbcmd.Append("SELECT * FROM (");
                        }
                        sbcmd.Append("SELECT ROWNUM AS RN, AA.* FROM ( SELECT ");
                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }
                        //加上分组条件
                        if (!String.IsNullOrEmpty(search.GroupBy))
                        {
                            sbcmd.Append(" Group By ");
                            sbcmd.Append(search.GroupBy);
                        }
                        if (search.OrderField.Count > 0)
                        {
                            sbcmd.Append(search.OrderField);
                        }
                        sbcmd.Append(" ) AA WHERE ROWNUM <= ");
                        sbcmd.Append(search.PageIndex * search.PageSize);
                        if (search.PageIndex > 0)
                        {
                            sbcmd.Append(" ) WHERE RN > ");
                            sbcmd.Append(search.PageSize * (search.PageIndex - 1));
                        }
                        break;
                        #endregion
                    }
                case DbProvider.MySql:
                    {
                        #region MySql
                        sbcmd.Append("SELECT ");
                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }
                        if (search.OrderField.Count > 0)
                        {
                            sbcmd.Append(search.OrderField);
                        }
                        sbcmd.Append(" LIMIT ");
                        sbcmd.Append((search.PageIndex - 1) * search.PageSize);
                        sbcmd.Append(",");
                        sbcmd.Append(search.PageSize);
                        break;
                        #endregion
                    }
                case DbProvider.SqlServer:
                    {
                        #region SqlServer
                        //版本小于2005或没有指定主键和排序字段
                        if ((version.Major < 9) || (search.OrderField.Count == 0 && string.IsNullOrEmpty(search.PrimaryKey)))
                        {
                            skip = search.PageSize * (search.PageIndex - 1);  //结果跳过指定行数
                            sbcmd.Append("SELECT TOP ");
                            sbcmd.Append((search.PageIndex) * search.PageSize);
                            sbcmd.Append(" ");
                            sbcmd.Append(search.FieldsName);
                            sbcmd.Append(" FROM ");
                            sbcmd.Append(search.TableName);
                            if (!string.IsNullOrEmpty(search.Where))
                            {
                                sbcmd.Append(" WHERE ");
                                sbcmd.Append(search.Where);
                            }
                            if (search.OrderField.Count > 0)
                            {
                                sbcmd.Append(search.OrderField);
                            }
                        }
                        else
                        {
                            sbcmd.Append("SELECT TOP ");
                            sbcmd.Append(search.PageSize);
                            sbcmd.Append(" * FROM (SELECT ROW_NUMBER() OVER ( ");
                            if (search.OrderField.Count == 0)
                            {
                                sbcmd.Append("ORDER BY ");
                                sbcmd.Append(search.PrimaryKey);
                            }
                            else
                            {
                                sbcmd.Append(search.OrderField);
                            }
                            sbcmd.Append(") AS ROWID,");
                            sbcmd.Append(search.FieldsName);
                            sbcmd.Append(" FROM ");
                            sbcmd.Append(search.TableName);
                            if (!string.IsNullOrEmpty(search.Where))
                            {
                                sbcmd.Append(" WHERE ");
                                sbcmd.Append(search.Where);
                            }
                            sbcmd.Append(") AS AA WHERE ROWID > ");
                            sbcmd.Append(search.PageSize * (search.PageIndex - 1));
                        }
                        break;
                        #endregion
                    }
                case DbProvider.SqlCe:
                case DbProvider.OleDb:
                case DbProvider.VistaDB:
                    {
                        #region OleDb SqlCe VistaDB
                        skip = search.PageSize * (search.PageIndex - 1); //结果跳过指定行数
                        sbcmd.Append("SELECT TOP ");
                        sbcmd.Append((search.PageIndex) * search.PageSize);
                        sbcmd.Append(" ");
                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }

                        if (search.OrderField.Count > 0)
                        {
                            sbcmd.Append(search.OrderField);
                        }
                        break;
                        #endregion
                    }
                case DbProvider.FireBird:
                    {
                        #region FireBird
                        /*FireBird支持两种分页方式:
                         * 2.0 以上: SELECT * FROM .. WHERE... ORDER BY ... ROWS 10 TO 20
                         * 1.x 版本: SELECT FIRST n SKIP m columns FROM ...  
                         */
                        sbcmd.Append("SELECT ");
                        #region For 1.0
                        if (version.Major < 2)
                        {
                            sbcmd.Append("FIRST ");
                            sbcmd.Append(search.PageSize);
                            sbcmd.Append(" ");
                            if (search.PageIndex > 0)
                            {
                                sbcmd.Append("SKIP ");
                                sbcmd.Append(search.PageIndex * search.PageSize);
                                sbcmd.Append(" ");
                            }
                        }
                        #endregion

                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }
                        if (search.OrderField.Count > 0)
                        {
                            sbcmd.Append(search.OrderField);
                        }
                        #region For 2.0
                        if (version.Major >= 2)
                        {
                            sbcmd.Append(" ROWS ");
                            sbcmd.Append(search.PageIndex * search.PageSize + 1);
                            sbcmd.Append(" TO ");
                            sbcmd.Append(search.PageSize * (search.PageIndex + 1));
                        }
                        #endregion
                        break;
                        #endregion
                    }
                case DbProvider.DB2:
                    {
                        #region DB2
                        //类似Sql Server
                        if (search.OrderField.Count == 0 && string.IsNullOrEmpty(search.PrimaryKey))
                        {
                            throw new Exception("主键或者排序字段必须有一个设置值");
                        }
                        sbcmd.Append("SELECT * FROM (SELECT ROWNUMBER() OVER ( ");

                        if (search.OrderField.Count == 0)
                        {
                            sbcmd.Append("ORDER BY ");
                            sbcmd.Append(search.PrimaryKey);
                        }
                        else
                        {
                            sbcmd.Append(search.OrderField);
                        }
                        sbcmd.Append(") AS ROWID,");
                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }
                        sbcmd.Append(") AS AA WHERE ROWID between ");
                        sbcmd.Append(search.PageSize * search.PageIndex + 1);
                        sbcmd.Append(" AND ");
                        sbcmd.Append((search.PageIndex + 1) * search.PageSize);
                        break;
                        #endregion
                    }
                case DbProvider.Informix:
                    {
                        #region Informix
                        //SELECT SKIP 2 FIRST 2  * FROM ... where ... ORDER BY ...
                        sbcmd.Append("SELECT SKIP ");
                        sbcmd.Append(search.PageIndex * search.PageSize);
                        sbcmd.Append("FIRST ");
                        sbcmd.Append(search.PageSize);
                        sbcmd.Append(" ");
                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }

                        sbcmd.Append(search.OrderField);
                        break;
                        #endregion
                    }
                case DbProvider.Ads:
                case DbProvider.Sybase:
                    {
                        #region
                        // SELECT TOP 2 START AT 5 * FROM ... ORDER BY ....
                        if (search.OrderField.Count == 0 && string.IsNullOrEmpty(search.PrimaryKey))
                        {
                            throw new Exception("主键或者排序字段必须有一个设置值");
                        }
                        sbcmd.Append("SELECT TOP ");
                        sbcmd.Append(search.PageSize);
                        if (search.PageIndex > 0)
                        {
                            sbcmd.Append(" START ");
                            sbcmd.Append(search.PageIndex * search.PageSize);
                        }
                        sbcmd.Append(" ");
                        sbcmd.Append(search.FieldsName);
                        sbcmd.Append(" FROM ");
                        sbcmd.Append(search.TableName);
                        if (!string.IsNullOrEmpty(search.Where))
                        {
                            sbcmd.Append(" WHERE ");
                            sbcmd.Append(search.Where);
                        }
                        if (search.OrderField.Count > 0)
                        {
                            sbcmd.Append(search.OrderField);
                        }
                        break;
                        #endregion
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        RETURN:
            return sbcmd.ToString();
        }

        /// <summary>
        /// 根据查询条件返回这个查询总共有多少条记录的语句
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>COUNT(*)语句</returns>
        public static string CreateCountSql(PageSearch search)
        {
            StringBuilder sbcmd = new StringBuilder();
            if (string.IsNullOrEmpty(search.TableName) || search.TableName.Trim().Length == 0)
            {
                sbcmd.Append("SELECT 1");
            }
            else
            {
                sbcmd.Append(string.Format("SELECT {0} FROM ", search.FieldsName));
                sbcmd.Append(search.TableName);
                if (!string.IsNullOrEmpty(search.Where))
                {
                    sbcmd.Append(" WHERE ");
                    sbcmd.Append(search.Where);
                }

            }
            return string.Format("SELECT COUNT(1) FROM ({0}) ", sbcmd);
        }

        /// <summary>
        /// 从一个读取流中获取一条SQL语句
        /// </summary>
        /// <param name="reader">流</param>
        /// <param name="provider">数据库类型</param>
        /// <returns>读取到的SQL语句</returns>
        public static string GetNextSql(StreamReader reader, DbProvider provider)
        {
            try
            {
                switch (provider)
                {
                    case DbProvider.SqlServer:
                        {
                            StringBuilder builder = new StringBuilder();
                            string sql = reader.ReadLine().Trim();
                            while (!reader.EndOfStream && (!sql.Equals("GO", StringComparison.OrdinalIgnoreCase)))
                            {
                                if ((sql.Length > 0) && (!sql.StartsWith("--")) && (!(sql.StartsWith("/*") && sql.EndsWith("*/"))))
                                {
                                    builder.Append(sql + Environment.NewLine);
                                }
                                sql = reader.ReadLine().Trim();
                            }
                            if ((!sql.Equals("GO", StringComparison.OrdinalIgnoreCase)))
                            {
                                if ((sql.Length > 0) && (!sql.StartsWith("--")) && (!(sql.StartsWith("/*") && sql.EndsWith("*/"))))
                                {
                                    builder.Append(sql + Environment.NewLine);
                                }
                            }
                            if (builder.Length == 0)
                            {
                                return null;
                            }
                            return builder.ToString();
                        }
                    default:
                        {
                            throw new NotImplementedException("没有实现这种类型的数据库:" + provider.ToString());
                        }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="sqls">数据库创建命令</param>
        public static void Init(IDbConnection conn, List<string> sqls)
        {
            using (conn as IDisposable)
            {
                IDbCommand cmd = conn.CreateCommand();
                try
                {
                    conn.Open();
                    foreach (string sql in sqls)
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    WriteDbException(ex, cmd);
                }
            }
        }

        /// <summary>
        /// 升级数据库
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="sqls">数据库升级命令</param>
        public static void Update(IDbConnection conn, Dictionary<int, string> sqls)
        {
            if (sqls == null || sqls.Count == 0)
            {
                return;
            }
            using (conn as IDisposable)
            {
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT CONFIGVALUE FROM CONFIGSET WHERE CONFIGKEY='VERSION'";
                try
                {
                    conn.Open();
                    int cv = Convert.ToInt32(cmd.ExecuteScalar());
                    foreach (int v in sqls.Keys)
                    {
                        if (v <= cv)
                        {
                            continue;
                        }
                        cmd.CommandText = sqls[v];
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = string.Format("UPDATE CONFIGSET SET CONFIGVALUE= '{0}' WHERE CONFIGKEY='VERSION'", v);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    WriteDbException(ex, cmd);
                }
            }
        }

        /// <summary>
        /// 创建一个数据访问器
        /// </summary>
        /// <param name="conn">数据库连接字符串</param>
        /// <param name="provider">数据库类型</param>
        /// <returns>数据访问器</returns>
        public static IDataAccess CreateDataAccess(string conn, DbProvider provider)
        {
            IDataAccess db = null;
            switch (provider)
            {
                //case DbProvider.Ads:
                //    {
                //        db = new AdsDataAccess(conn);
                //        break;
                //    }
                //case DbProvider.Sybase:
                //    {
                //        db = new AseDataAccess(conn);
                //        break;
                //    }
                //case DbProvider.DB2:
                //    {
                //        db = new Db2DataAccess(conn);
                //        break;
                //    }
                //case DbProvider.FireBird:
                //    {
                //        db = new FirebirdDataAccess(conn);
                //        break;
                //    }
                //case DbProvider.Informix:
                //    {
                //        db = new IfxDataAccess(conn);
                //        break;
                //    }
                case DbProvider.MySql:
                    {
                        db = new MySqlDataAccess(conn);
                        break;
                    }
                //case DbProvider.PostgreSQL:
                //    {
                //        db = new NpgsqlDataAccess(conn);
                //        break;
                //    }
                //case DbProvider.SqlCe:
                //    {
                //        db = new SqlCeDataAccess(conn);
                //        break;
                //    }
                //case DbProvider.Sqlite:
                //    {
                //        db = new CsharpSqliteDataAccess(conn);
                //        break;
                //    }
                //case DbProvider.VistaDB:
                //    {
                //        db = new VistaDataAccess(conn);
                //        break;
                //    }
                case DbProvider.SqlServer:
                    {
                        db = new SqlDataAccess(conn);
                        break;
                    }
                case DbProvider.Odbc:
                    {
                        db = new OdbcDataAccess(conn);
                        break;
                    }
                case DbProvider.OleDb:
                    {
                        db = new OleDbDataAccess(conn);
                        break;
                    }
                case DbProvider.Oracle:
                    {
                        db = new OracleDataAccess(conn);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return db;
        }

        #region 常用的SQL语句类
        /// <summary>
        /// 常用的SQL语句类
        /// </summary>
        public static class CommonSql
        {
            /// <summary>
            /// 创建表配置表ConfigSet的语句
            /// </summary>
            public static string CreateConfigSet = "CREATE TABLE CONFIGSET (CONFIGKEY VARCHAR(10) PRIMARY KEY,CONFIGVALUE NVARCHAR(4000) NOT NULL)";

            /// <summary>
            /// 创建记录序列表的语句
            /// </summary>
            public static string CreateSeqGen = "CREATE TABLE SeqGen (SeqName VarChar(10) PRIMARY KEY, SeqValue INT NOT NULL, SeqStep INT NOT NULL, SeqMin INT NOT NULL, SeqMax INT NOT NULL, SeqLoop INT NOT NULL)";

            /// <summary>
            /// 获取插入到ConfigSet表的语句
            /// </summary>
            /// <param name="key">要插入的键</param>
            /// <param name="value">要插入的值</param>
            /// <returns>生成的插入语句</returns>
            public static string GetInsertConfigSetSql(string key, string value)
            {
                return string.Format("INSERT INTO CONFIGSET (CONFIGKEY,CONFIGVALUE) VALUES('{0}','{1}')", key, value);
            }

            /// <summary>
            /// 获取插入到序列表的语句
            /// </summary>
            /// <param name="name">序列名称</param>
            /// <param name="value">序列值</param>
            /// <param name="step">步长</param>
            /// <param name="min">最小值</param>
            /// <param name="max">最大值</param>
            /// <param name="loop">到达最大值后是否从最小值开始循环</param>
            /// <returns></returns>
            public static string GetInsertSeqGenSql(string name, int value, int step, int min, int max, bool loop)
            {
                return string.Format("INSERT INTO SeqGen (SeqName,SeqValue,SeqStep,SeqMin,SeqMax,SeqLoop) VALUES('{0}',{1},{2},{3},{4},{5})", name, value, step, min, max, loop ? 1 : 0);
            }

            /// <summary>
            /// 角色权限表
            /// </summary>
            public static class RolePermsiion
            {
                /// <summary>
                /// 创建角色表的SQL语句
                /// </summary>
                public static string CreateRole = "CREATE TABLE Role (ROLEID INT PRIMARY KEY,RoleName NVARCHAR(50) NOT NULL, Description NVARCHAR(500), IsDefault INT NOT NULL)";

                /// <summary>
                /// 创建角色权限表的SQL语句
                /// </summary>
                public static string CreateRolePermission = "CREATE TABLE RolePermission (RoleId INT NOT NULL,GroupName NVARCHAR(50) NOT NULL,Permission INT NOT NULL)";

                /// <summary>
                /// 实体角色表
                /// </summary>
                public static string CreateModelRole = "CREATE TABLE ModelRole (ModelId INT NOT NULL, ModelType INT NOT NULL, RoleId INT NOT NULL)";

                /// <summary>
                /// 实体权限表
                /// </summary>
                public static string CreateModelPermission = "CREATE TABLE ModelPermission(ModelId INT NOT NULL,ModelType INT NOT NULL,GroupName NVARCHAR(50) NOT NULL,Permission INT NOT NULL)";
            }

            /// <summary>
            /// 创建树形结构的SQL语句
            /// </summary>
            /// <param name="table">表名</param>
            /// <returns>创建树形结构的SQL语句</returns>
            public static string GetTreeSql(string table)
            {
                return string.Format("CREATE TABLE {0}(NodeId INT NOT NULL,NodeName NVARCHAR(100) NOT NULL,LevelNo INT NOT NULL,AddTime DATETIME NOT NULL,ParentId INT NOT NULL,OwnerId INT NOT NULL,OrderId INT NOT NULL,StatusCode INT NOT NULL,NodeDesc NVARCHAR(4000) NOT NULL,ParentIdList VARCHAR(1000) NOT NULL,PicUrl VARCHAR(255),TargetUrl VARCHAR(255))", table);
            }
        }
        #endregion

        #region

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static OperationType GetOperationType(String commandText)
        {
            commandText = commandText.ToUpper();
            if (commandText.StartsWith("INSERT"))
            {
                return OperationType.新增;
            }
            else if (commandText.StartsWith("DELETE"))
            {
                return OperationType.删除;
            }
            else if (commandText.StartsWith("UPDATE"))
            {
                return OperationType.更新;
            }
            else {
                return OperationType.查询;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static Dictionary<String, object> GetKeyValueFromSql(String commandText, OperationType opearationType)
        {
            Dictionary<String, object> dict = new Dictionary<string, object>();
         
            String segment = null;
            switch (opearationType)
            {
                case OperationType.新增:
                    segment = GetInsterSegmentFromSql(commandText);
                    break;
                case OperationType.更新:
                    segment = GetUpdateSegmentFromSql(commandText);
                    dict = GetKeyValueFromUpdateSegment(segment);
                    break;
                case OperationType.删除:
                    break;
                default:
                    break;
            } 
            return dict;
        }

        public static String GetTableNameByNonQuery(String commandText)
        {
            var result = matches(@"(?<=((update)|(insert\s+into)|(delete)))\s+\w*(?=\s+)", commandText);
            if (result != null)
            {
                result = result.Trim();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static Dictionary<String, object> GetKeyValueFromUpdateSegment(String segment)
        {

            Dictionary<String, object> dict = null;
            if (!String.IsNullOrEmpty(segment))
            {
                dict = new Dictionary<string, object>();
                string[] segArray = segment.Split(',');
                if (segArray != null)
                {
                    foreach (var segKv in segArray)
                    {
                        string[] kvs = segKv.Split('=');
                        if (kvs != null)
                        {
                            dict[kvs[0].Trim()] = kvs[1].Trim();
                        }
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static String GetInsterSegmentFromSql(String commandText)
        {
            return matches(@"(?<=insert\s+into\s+\w+\s+).* ", commandText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static String GetUpdateSegmentFromSql(String commandText)
        {
            return matches(@"(?<=update\s+\w+\s+set\s+)\w+\s*=.*(?=where)", commandText);
        }

        public static String GetFirstWhereSegmentFromSql(String commandText)
        {
            return matches(@"(?<=WHERE).*", commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regEx"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static String matches(String regEx, String text)
        {
            MatchCollection mc = ASoft.Regular.Matches(regEx, text, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (mc.Count > 0)
            {
                return mc[0].Value;
            }
            else {
                return null;
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum OperationType
    {
        查询 = 0,
        /// <summary>
        /// INSERT
        /// </summary>
        新增 = 1,
        /// <summary>
        /// UPDATE
        /// </summary>
        更新 = 2,
        /// <summary>
        /// DELETE
        /// </summary>
        删除 = 3
    }
}