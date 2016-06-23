using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ASoft.Db;
using ASoft.Text;
using ASoft.Model;
using System.Linq; 
using System.Configuration;

namespace ASoft.Dal
{

    /// <summary>
    /// 基础dal
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public abstract class BaseDal<E> : ASoft.Dal.IBaseDal<E>
        where E : ASoft.Model.BaseModel, new()
    {
        private E _model = null;

       
        public BaseDALFactory DALFactory {
            set; get;
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseDal()
        {
            log = LogAdapter.GetLogger("Dal");
            _model = new E();
        }

      

        public BaseDal(BaseDALFactory factory):this()
        {
            this.DALFactory = factory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="baseDb"></param>
        public BaseDal(IDataAccess db, IDataAccess baseDb)
        {
            this.db = db;
            this.baseDb = baseDb;
            _model = new E();
            this.log = LogAdapter.GetLogger("db");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual E Get(String id)
        {
            try
            {
                return this.Where(this.KeyField + "={0}", id).First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建一个访问器
        /// </summary>
        /// <param name="db"></param>
        public BaseDal(IDataAccess db)
        {
            this.baseDb = this.db = db;
            log = LogAdapter.GetLogger("Dal");
        }

        /// <summary>
        /// 基础库的DB对象，内含ASOFT_ID_GEN表，用于生成主键值
        /// </summary>
        public IDataAccess baseDb { set; get; }
        private IDataAccess _db = null;
        public IDataAccess db
        {
            set
            {
                _db = value;
            }
            get
            {
                return _db;
            }
        }

        private string _table = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public String Table
        {
            set
            {
                _table = value;
            }
            get
            {
                if (String.IsNullOrEmpty(_table))
                {
                    _table = EntityHelper.GetEntityAttribute(_model).DataTable;
                }
                return _table;
            }
        }

        /// <summary>
        /// 对应主键在数据库的名称
        /// </summary>
        public String KeyField
        {
            get
            {
                var hasfield = false;
                String result = "";
                EntityInfo ea = EntityHelper.GetEntityAttribute(_model);
                foreach (var item in ea.EntityProperty)
                {
                    if (item.Value.PropertyAttribute.IsPrimaryKey)
                    {
                        hasfield = true;
                        result = item.Value.PropertyAttribute.Field;
                        break;
                    }
                }
                if (!hasfield)
                {
                    throw new Exception(string.Format("没有为{0}类型的实体设置关键字,无法按照关键字进行更新", _model.GetType().FullName));
                }
                return result;
            }
        }

        protected LogAdapter log;

        /// <summary>
        /// 插入实体对象到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual E Insert(E entity)
        {
            this.db.InsertObject(entity);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(String id)
        {
            DataCommand sql = this.db.CreateSQLCommand("select count(1) from " + this.Table + " where " + this.KeyField + "={0}", id);
            using (ScalerResult sr = db.ExecuteScalar(sql))
            {
                return sr.IntValue > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public virtual void Delete(E e)
        {
            db.DeleteObject(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(String id)
        {
            DataCommand sql = this.db.CreateSQLCommand("delete from  " + this.Table + " where " + this.KeyField + "={0}", id);
            return db.ExecuteNonQuery(sql).Value > 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool Update(E entity)
        {
            var cmd = db.GetCommandByUpdate(entity); 
            db.ExecuteNonQuery(cmd);
            return true; 
        }

        /// <summary>
        ///获取所有
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<E> GetAll(int page, int rows, out int total)
        {
            return this.Where("", page, rows, out total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<E> GetAll()
        {
            return this.Where("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<E> Where(String where)
        {
            List<E> result = null;
            String sql = String.Format("select * from {0}", this.Table);
            if (!String.IsNullOrEmpty(where))
            {
                sql += String.Format(" where {0}", where);
            }
            DataCommand command = db.CreateSQLCommand(sql);
            LogAdapter.Level = LogLevel.Warn;
            log.Warn("Where(String where)函数已过时，请采用参数化方法，当前SQL：" + sql);
            using (DataReader dr = db.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    if (result == null)
                        result = new List<E>();
                   
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }


        /// <summary>
        /// 执行参数化方法
        /// </summary>
        /// <param name="where"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected List<E> Where(String where, params object[] args)
        {
            List<E> result = null;
            String sql = String.Format("select * from {0}", this.Table);
            if (!String.IsNullOrEmpty(where))
            {
                sql += String.Format(" where {0}", where);
            }
            var command = this.db.CreateSQLCommand(sql, args);
            using (DataReader dr = this.db.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    if (result == null)
                        result = new List<E>();
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandBuilder"></param>
        /// <returns></returns>
        protected List<E> Where(DataCommandBuilder commandBuilder)
        {
            List<E> result = null;
            var command = commandBuilder.CreateCommand();
            using (DataReader dr = this.db.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    if (result == null)
                        result = new List<E>(); 
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<E> Where(String where, int page, int rows, out int total)
        {
            List<E> result = null;
            PageSearch search = new PageSearch();
            search.PageIndex = page;
            //rows==0时 取15条
            search.PageSize = rows > 0 ? rows : 15;
            //获取分页数据时将跳过的记录数
            int skip = 0;
            search.TableName = this.Table;
            search.FieldsName = "*";
            if (!String.IsNullOrEmpty(where))
            {
                search.And(where);
            }
            //总记录数
            total = this.db.ExecuteScalar(DbTools.CreateCountSql(search)).IntValue;
            search.TotalCount = total;
            //创建分页SQL
            String sql = DbTools.CreatePageSql(search, this.db.Provider, null, out skip);
            LogAdapter.Level = LogLevel.Warn;
            log.Warn("Where(String where)函数已过时，请采用参数化方法，当前SQL：" + sql);
            using (DataReader dr = db.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    if (result == null)
                        result = new List<E>();
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="total"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected DataCommand CreatePageCommand(String fieldName, String tableName, String where, int start, int limit, out int total, params object[] args)
        {
            PageSearch search = new PageSearch();
            var page = Convert.ToInt32(Math.Floor((decimal)(start) / limit) + 1);
            search.PageIndex = page;
            //rows==0时 取15条
            search.PageSize = limit > 0 ? limit : 15;
            //获取分页数据时将跳过的记录数
            int skip = 0;
            search.TableName = tableName;
            search.FieldsName = fieldName;

            if (!String.IsNullOrEmpty(where))
            {
                //where条件参数化
                //command = this.db.CreateSQLCommand(where, args);
                //if (command.Parameters != null)
                //{
                //    myParmes = (IDbDataParameter[])command.Parameters.Clone();
                //    myParmes1 = this.db.CreateSQLCommand(where, args).Parameters;
                //}
                search.And(where);
            }
            //总记录数
            total = this.db.ExecuteScalar(db.CreateSQLCommand(DbTools.CreateCountSql(search), args)).IntValue;
            search.TotalCount = total;
            //创建分页SQL
            String sql = DbTools.CreatePageSql(search, this.db.Provider, null, out skip);
            //DbTools.CreatePageSql(sql, db.Provider,)
            return db.CreateSQLCommand(sql, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="total"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected List<E> Where(String where, int page, int rows, out int total, params object[] args)
        {
            List<E> result = null;
            PageSearch search = new PageSearch();
            search.PageIndex = page;
            //rows==0时 取15条
            search.PageSize = rows > 0 ? rows : 15;
            //获取分页数据时将跳过的记录数
            int skip = 0;
            search.TableName = this.Table;
            search.FieldsName = "*";
            if (!String.IsNullOrEmpty(where))
            {
                ////where条件参数化
                //command = this.db.CreateSQLCommand(where, args);
                //if (command.Parameters != null)
                //{
                //    myParmes = (IDbDataParameter[])command.Parameters.Clone();
                //    myParmes1 = this.db.CreateSQLCommand(where, args).Parameters;
                //}
                search.And(where);
            }

            DataCommand countCommand = db.CreateSQLCommand(DbTools.CreateCountSql(search), args);
            //总记录数
            total = this.db.ExecuteScalar(countCommand).IntValue;
            search.TotalCount = total;
            //创建分页SQL
            String sql = DbTools.CreatePageSql(search, this.db.Provider, null, out skip);
            using (DataReader dr = this.db.ExecuteReader(db.CreateSQLCommand(sql, args)))
            {
                while (dr.Read())
                {
                    if (result == null)
                        result = new List<E>();
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandBuilder"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        protected List<E> Where(DataCommandBuilder commandBuilder, int page, int rows, out int total)
        {
            List<E> result = null;
            //总记录数
            total = this.db.ExecuteScalar(commandBuilder.CreateCountCommand(page, rows)).IntValue;
            //创建分页SQL
            var command = commandBuilder.CreatePageCommand(page, rows, total);
            using (DataReader dr = this.db.ExecuteReader(command))
            {
                while (dr.Read())
                {
                    if (result == null)
                        result = new List<E>();
                    E entity = db.SetObjectProperty<E>(dr);
                    result.Add(entity);
                }
                dr.Close();
            }
            return result;
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        protected List<E> WhereStart(String where, int start, int limit, out int total)
        {
            var page = Convert.ToInt32(Math.Floor((decimal)(start) / limit) + 1);
            List<E> list = null;
            total = 0;
            LogAdapter.Level = LogLevel.Warn;
            log.Warn("WhereStart(String where, int start, int limit, out int total)函数已过时，请采用参数化方法，当前Where：" + where);
            if (limit > 0)
            {
                list = this.Where(where, page, (int)limit, out total);
            }
            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="total"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected List<E> WhereStart(String where, int start, int limit, out int total, params object[] args)
        {
            var page = Convert.ToInt32(Math.Floor((decimal)(start) / limit) + 1);
            List<E> list = null;
            total = 0;
            if (limit > 0)
            {
                list = this.Where(where, page, (int)limit, out total, args);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandBuilder"></param>
        /// <param name="start"></param>
        /// <param name="limit"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        protected List<E> WhereStart(DataCommandBuilder commandBuilder, int start, int limit, out int total)
        {
            var page = Convert.ToInt32(Math.Floor((decimal)(start) / limit) + 1);
            List<E> list = null;
            total = 0;
            if (limit > 0)
            {
                list = this.Where(commandBuilder, page, (int)limit, out total);
            }
            return list;
        }
    }
}
