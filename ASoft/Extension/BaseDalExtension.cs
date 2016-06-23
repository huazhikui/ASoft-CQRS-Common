using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Model;
using ASoft.Dal;
using System.Linq.Expressions;
using ASoft.Db;
using System.Data;

namespace ASoft.Extension
{
    /// <summary>
    /// 
    /// </summary>
    public static class BaseDalExtension
    {
        public static Query<E> Where2<E>(this BaseDal<E> dal, string where, params object[] args)
            where E : ASoft.Model.BaseModel, new()
        {
            where = where.Trim();
            if (where.ToUpper().StartsWith("WHERE"))
            {
                where = where.Remove(0, 5).Trim();
            }

            Query<E> query = new Query<E>(dal.db, where, args);
            return query;
        }

        public static Query<E> Get2<E>(this BaseDal<E> dal, String id)
            where E : ASoft.Model.BaseModel, new()
        {
            Query<E> query = new Query<E>(dal.db, dal.KeyField + "={0}", id);
            return query;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Query<E>
        where E : ASoft.Model.BaseModel, new()
    {
        public Query(IDataAccess db)
        {
            this.db = db;
            log = LogAdapter.GetLogger("Query");
        }
        public Query(IDataAccess db, string where, params object[] args)
        {
            this.db = db;
            this.where = where;
            this.args = args;
            log = LogAdapter.GetLogger("Query");
        }


        #region "Private Property"
        private E _model = new E();
        private string _table = "";
        private string _fields = null;
        #endregion

        protected IDataAccess db = null;
        protected LogAdapter log = null;
        /// <summary>
        /// 表名
        /// </summary>
        public String DataTable
        {
            get
            {
                if (string.IsNullOrEmpty(_table))
                {
                    _table = EntityHelper.GetEntityAttribute(_model).DataTable;
                }
                return _table;
            }
        }
        /// <summary>
        /// 属性集合
        /// </summary>
        public List<string> Properties { get; private set; }
        /// <summary>
        /// 字段
        /// </summary>
        protected string fields
        {
            get
            {
                if (string.IsNullOrEmpty(_fields))
                {
                    List<string> fieldlist = new List<string>();
                    if (Properties == null || !Properties.Any())
                    {
                        throw new Exception("Properties不能为null");
                    }

                    if (Properties.Count() == 1 && Properties[0] == "*")
                    {
                        fieldlist.Add("*");
                    }
                    else
                    {
                        foreach (var item in Properties)
                        {
                            if (EntityHelper.GetEntityAttribute(_model).EntityProperty.Keys.Contains(item))
                            {
                                fieldlist.Add(EntityHelper.GetEntityAttribute(_model).EntityProperty[item].PropertyAttribute.Field);
                            }
                        }
                    }

                    if (fieldlist.Count == 0)
                    {
                        throw new ArgumentException(string.Format("无法识别的字段:{0}", Properties[0]));
                    }

                    foreach (var item in fieldlist)
                    {
                        _fields += string.Format("{0}, ", item);
                    }
                    _fields = _fields.Substring(0, _fields.Length - 2);
                }
                return _fields;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string where { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected object[] args { get; set; }

        #region "分页查询"
        /// <summary>
        /// 分页查询时跳过的记录数
        /// </summary>
        protected int skipCount { get; private set; }
        /// <summary>
        /// 分页查询时获取的记录数
        /// </summary>
        protected int takeCount { get; private set; }
        /// <summary>
        /// 是否启用分页
        /// </summary>
        private bool isPageUsed = false;
        /// <summary>
        /// 总记录数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            var sql = string.Format("select count(1) from {0}", DataTable);
            if (!string.IsNullOrEmpty(where))
            {
                sql += string.Format(" where {0}", where);
            }
            DataCommand countCommand = db.CreateSQLCommand(sql, args);
            return this.db.ExecuteScalar(countCommand).IntValue;
        }

        /// <summary>
        /// 指定跳过的记录数
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Query<E> Skip(int count=0)
        {
            skipCount = count;
            isPageUsed = true;
            return this;
        }

        /// <summary>
        /// 指定获取的记录数
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Query<E> Take(int count = 15)
        {
            takeCount = count;
            isPageUsed = true;
            return this;
        }
        ///// <summary>
        ///// 分页查询
        ///// </summary>
        ///// <param name="fieldExpr">字段表达式 : item=>"*"或item=>item.ID或item=>new {item.ID,item.Name}</param>
        ///// <param name="start"></param>
        ///// <param name="limit"></param>
        ///// <param name="total"></param>
        ///// <returns></returns>
        //public IEnumerable<E> SelectStart(Expression<Func<E, object>> fieldExpr, int start, int limit, out int total)
        //{
        //    Properties = GetPropNames(fieldExpr);
        //    PageSearch search = new PageSearch();
        //    search.PageIndex = start;
        //    //limit==0时 取15条
        //    search.PageSize = limit > 0 ? limit : 15;
        //    //获取分页数据时将跳过的记录数
        //    int skip = 0;
        //    search.TableName = DataTable;
        //    search.FieldsName = fields;
        //    if (!string.IsNullOrEmpty(where))
        //    {
        //        search.And(where);
        //    }

        //    DataCommand countCommand = db.CreateSQLCommand(DbTools.CreateCountSql(search), args);
        //    total = this.db.ExecuteScalar(countCommand).IntValue;

        //    String sql = DbTools.CreatePageSql(search, this.db.Provider, null, out skip);

        //    return ExecuteSQLCommand(sql, args);
        //}
        #endregion

        /// <summary>
        /// 查询全部字段
        /// </summary>
        public IEnumerable<E> Select()
        {
            return Select(item => "*");
        }
        /// <summary>
        /// 查询指定的字段
        /// </summary>
        /// <param name="fieldExpr">字段表达式 : item=>"*"或item=>item.ID或item=>new {item.ID,item.Name}</param>
        public IEnumerable<E> Select(Expression<Func<E, object>> fieldExpr)
        {
            Properties = GetPropNames(fieldExpr);
            String sql = string.Empty;
            if (isPageUsed)
            {
                PageSearch search = new PageSearch();
                search.PageIndex = skipCount + 1;
                //limit==0时 取15条
                search.PageSize = takeCount > 0 ? takeCount : 15;
                //获取分页数据时将跳过的记录数
                int skip = 0;
                search.TableName = DataTable;
                search.FieldsName = fields;
                if (!string.IsNullOrEmpty(where))
                {
                    search.And(where);
                }
                sql = DbTools.CreatePageSql(search, this.db.Provider, null, out skip);
            }
            else
            {
                sql = string.Format("select {0} from {1}", fields, DataTable);

                if (!string.IsNullOrEmpty(where))
                {
                    sql += string.Format(" where {0}", where);
                }
            }

            return ExecuteSQLCommand(sql, args);
        }

        

        #region "加锁"
        /// <summary>
        /// 加锁查询
        /// </summary>
        /// <param name="fieldExpr">字段表达式 : item=>"*"或item=>item.ID或item=>new {item.ID,item.Name}</param>
        /// <param name="transaction">一个事务，通过事务的Commit来解锁</param>
        /// <returns></returns>
        public IEnumerable<E> SelectLock(Expression<Func<E, object>> fieldExpr, out IDbTransaction transaction)
        {
            return SelectLock(fieldExpr, out transaction, LockType.Wait);
        }

        /// <summary>
        /// 加锁查询
        /// </summary>
        /// <param name="fieldExpr">字段表达式 : item=>"*"或item=>item.ID或item=>new {item.ID,item.Name}</param>
        /// <param name="transaction">一个事务，通过事务的Commit来解锁</param>
        /// <param name="type">锁的类型</param>
        /// <param name="timeout">如果type为WaitN，则需指定等待的秒数</param>
        /// <returns></returns>
        public IEnumerable<E> SelectLock(Expression<Func<E, object>> fieldExpr, out IDbTransaction transaction, LockType type, params object[] timeout)
        {
            Properties = GetPropNames(fieldExpr);
            IDbConnection connection = db.CreateConnection();
            connection.Open();
            transaction = connection.BeginTransaction();
            return ExecuteSQLCommand(transaction, CreateLockSQLCommand(type, timeout), args);
        }
        #endregion

        #region "protected"
        protected IEnumerable<E> ExecuteSQLCommand(IDbTransaction transaction, string sql, params object[] args)
        {
            List<E> result = new List<E>();

            try
            {
                using (DataReader dr = db.ExecuteReader(transaction, db.CreateSQLCommand(sql, args)))
                {
                    while (dr.Read())
                    {
                        E e = db.SetObjectProperty<E>(dr);
                        result.Add(e);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("ORA-00054") != -1 || ex.Message.IndexOf("ORA-30006") != -1)
                {
                    throw new ResourceLockedException(ex.Message);
                }
                throw ex;
            }

            return result as IEnumerable<E>;
        }

        protected IEnumerable<E> ExecuteSQLCommand(string sql, params object[] args)
        {
            //#region del 将List集合返回成使用yield
            List<E> result = new List<E>();

            using (DataReader dr = db.ExecuteReader(db.CreateSQLCommand(sql, args)))
            {
                while (dr.Read())
                {
                    E e = db.SetObjectProperty<E>(dr);
                    result.Add(e);
                }
                dr.Close();
            }

            return result as IEnumerable<E>;
            //#endregion

            //using (DataReader dr = db.ExecuteReader(db.CreateSQLCommand(sql, args)))
            //{
            //    while (dr.Read())
            //    {
            //        E e = new E();
            //        db.SetObjectProperty(e, dr);
            //        yield return e;
            //    }
            //    dr.Close();
            //} 
        }

        /// <summary>
        /// 解析字段表达式，获取查询的字段
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        protected List<string> GetPropNames(Expression<Func<E, object>> expr)
        {
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Constant://item=>"*"
                    return new string[] { ((ConstantExpression)expr.Body).Value.ToString() }.ToList();
                case ExpressionType.MemberAccess://item=>item.ID
                    return new string[] { ((MemberExpression)expr.Body).Member.Name }.ToList();
                case ExpressionType.New://item=>new {item.ID, item.Name}
                    return ((NewExpression)expr.Body).Members.Select(item => item.Name).ToList();
                default:
                    return new List<string>();
            }
        }

        protected string CreateLockSQLCommand(LockType type, params object[] timeout)
        {
            var sql = new StringBuilder();
            switch (db.Provider)
            {
                case DbProvider.Oracle:
                case DbProvider.MySql:
                    {
                        sql.AppendFormat("select {0} from {1}", fields, DataTable);

                        if (!string.IsNullOrEmpty(where))
                        {
                            sql.AppendFormat(" where {0}", where);
                        }
                        sql.Append(" FOR UPDATE");


                        switch (type)
                        {
                            case LockType.WaitN:
                                {
                                    if (timeout != null)
                                    {
                                        int n = Convert.ToInt32(timeout[0]);
                                        if (n == 0)
                                        {
                                            throw new Exception("参数timeout的格式错误");
                                        }
                                        sql.AppendFormat(" WAIT {0}", n);
                                    }
                                    break;
                                }
                            case LockType.NoWait:
                                {
                                    sql.Append(" NOWAIT");
                                    break;
                                }
                            case LockType.SkipLocked:
                                {
                                    sql.Append(" SKIP LOCKED");
                                    break;
                                }
                            case LockType.Wait:
                            default:
                                {
                                    break;
                                }
                        }
                        break;
                    }
                case DbProvider.SqlServer:
                    {
                        throw new NotImplementedException();
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            return sql.ToString();
        }

        #endregion
    }
    /// <summary>
    /// 加锁类型
    /// </summary>
    public enum LockType
    {
        /// <summary>
        /// 一直等待
        /// </summary>
        Wait = 0,
        /// <summary>
        /// 等待N秒，N由参数指定
        /// </summary>
        WaitN = 1,
        /// <summary>
        /// 不等待
        /// </summary>
        NoWait = 2,
        /// <summary>
        /// 跳过加锁的行
        /// </summary>
        SkipLocked = 4
    }

    /// <summary>
    /// 请求的行已被加锁
    /// </summary>
    [Serializable]
    public class ResourceLockedException : Exception
    {
        public ResourceLockedException() { }
        public ResourceLockedException(string message) : base(message) { }
        public ResourceLockedException(string message, Exception inner) : base(message, inner) { }
        protected ResourceLockedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }
}
