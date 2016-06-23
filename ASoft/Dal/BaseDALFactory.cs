using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Db;
using ASoft.Model;
using System.Configuration;
using System.Collections;
using System.Data;
namespace ASoft.Dal
{
    /// <summary>
    /// 兼容
    /// </summary>
    public abstract class BaseDataFacade : BaseDALFactory
    {
        public BaseDataFacade() : base() { }

        public BaseDataFacade(String key) : base(key) { }
    }
    /// <summary>
    /// DataFactory的基类
    /// </summary>
    public class BaseDALFactory : ASoft.Dal.IDALFactory
    {
        /// <summary>
        /// 构造
        /// </summary>
        public BaseDALFactory()
        {
            String fullName = this.GetType().FullName;
            this.init(fullName);
        }

        private String _key = string.Empty;
        public BaseDALFactory(String key)
        {
            this.init(key);
        }

        public String Key { get { return _key; } }

        protected virtual void init(String key)
        {
            _key = key;
            this.DbConnectStringKey = Config.GetAppSettings(key);
            if (String.IsNullOrEmpty(this.DbConnectStringKey))
            {
                Config.SaveAppSetting(key, "");
                throw new Exception(String.Format("请先配置Web.config中AppSetting节点KEY为{0}的值", key));
            }
            else
            {
                DbConnectionStringSettings = Config.GetConnectionSettings(this.DbConnectStringKey);
            }
        }

        private static Hashtable _hstDal;
        /// <summary>
        /// 数据库链接字符串的KEY，用于GetConnectionSettings方法调用
        /// </summary>
        public String DbConnectStringKey
        {
            protected set;
            get;
        }

        /// <summary>
        /// 数据库连接字符串（GetConnectionSettings）
        /// </summary>
        public ConnectionStringSettings DbConnectionStringSettings
        {
            protected set;
            get;
        }

        public ConnectionStringSettings BaseDbConnectionStringSettings
        {
            protected set;
            get;
        }

        private static IDataAccess baseDb = null;
        /// <summary>
        /// 包含序列生成的基础库（IDataAccess）实例，目前只适用ORACLE
        /// </summary>
        public IDataAccess BaseDb
        {
            get
            {
                try
                {
                    if (baseDb == null)
                    {
                        if (this.BaseDbConnectionStringSettings == null)
                        {
                            this.BaseDbConnectionStringSettings = Config.GetConnectionSettings(this.BaseDbConnectStringKey);
                        }
                        baseDb = createDA(BaseDbConnectionStringSettings);
                    }
                }
                catch (Exception ex)
                {
                    LogAdapter.Db.Error("创建baseDb时出现异常" + ex.StackTrace);
                }
                return baseDb;
            }
        }

        private static Dictionary<String, IDataAccess> _dbDict = new Dictionary<string, IDataAccess>();
        public IDataAccess Db
        {
            get
            {
                try
                {
                    if (!_dbDict.Keys.Contains(Key) || _dbDict[this.Key] == null)
                    {
                        if (DbConnectionStringSettings.Name == this.BaseDbConnectStringKey)
                        {
                            _dbDict[this.Key] = this.BaseDb;
                        }
                        else
                        {
                            _dbDict[this.Key] = createDA(DbConnectionStringSettings);
                        }
                    }
                    return _dbDict[this.Key];
                }
                catch (Exception ex)
                {
                    LogAdapter.Db.Error("创建db时出现异常" + ex.StackTrace);
                }
                return baseDb;
            }

        }


        protected IDataAccess createDA(ConnectionStringSettings connSet)
        {
            String providerName = connSet.ProviderName.ToUpper();
            switch (providerName)
            {
                case "SQLSERVER":
                    return new SqlDataAccess(connSet.ConnectionString);
                case "OLEDB":
                    return new OleDbDataAccess(connSet.ConnectionString);
                case "ODBC":
                    return new OdbcDataAccess(connSet.ConnectionString);
                //case "MYSQL":
                //    return new MySqlDataAccess(connSet.ConnectionString);
                default:
                    return new OracleDataAccess(connSet.ConnectionString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected String BaseDbConnectStringKey
        {
            get
            {
                return "BaseConnectString";
            }
        }

        /// <summary>
        /// 获取实体访问类的实例
        /// </summary>
        /// <typeparam name="E">实体类（对应数据库实体表）</typeparam>
        /// <returns>访问类的实例</returns>
        public virtual E GetDal<E, E1>()
            where E : BaseDal<E1>, new()
            where E1 : BaseModel, new()
        {
            Type type = typeof(E);
            String key = type.FullName + "_" + this.Key;
            E result = null;
            if (_hstDal == null)
            {
                _hstDal = new Hashtable();
            }
            if (_hstDal.ContainsKey(key))
            {
                result = _hstDal[key] as E;
            }
            else
            {
                result = new E();
                result.DALFactory = this;
                if (this.BaseDb == null)
                {
                    LogAdapter.Db.Error("系统的basedb未完成初始化");
                    throw new Exception("异常：系统的basedb未完成初始化");
                }
                result.baseDb = this.BaseDb;
                result.db = this.Db;
                _hstDal[key] = result;
            }
            return result;
       
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConfigKey"></param>
        /// <returns></returns>
        public static IDataAccess GetDataAccess(String dbConfigKey)
        {
            if (_dbDict == null)
            {
                _dbDict = new Dictionary<string, IDataAccess>();
            }
            if (!_dbDict.Keys.Contains(dbConfigKey))
            {
                ConnectionStringSettings connectionSettings = ASoft.Config.GetConnectionSettings(dbConfigKey);
                if (connectionSettings == null)
                {
                    throw new ArgumentException("connectionStrings中没有找到name=\"{0}\"的链接配置", dbConfigKey);
                }
                DbProvider provider = DbTools.GetDbProvider(connectionSettings.ProviderName);
                var myDb = DbTools.CreateDataAccess(connectionSettings.ConnectionString, provider);
                _dbDict[dbConfigKey] = myDb;
            }
            return _dbDict[dbConfigKey];

        }
    }
}
