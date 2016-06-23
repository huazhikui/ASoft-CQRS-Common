using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace ASoft.Db
{
    [Flags]
    public enum DataFieldFlags
    {
        None = 0,

        /// <summary>
        /// 插入
        /// </summary>
        Insert = 1,

        /// <summary>
        /// 更新
        /// </summary>
        Update = 2,

        /// <summary>
        /// 选择
        /// </summary>
        Select = 8,

        /// <summary>
        /// 全部
        /// </summary>
        ALL = 255
    }

    /// <summary>
    /// 设置一个实体类对应的数据库表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataEntityAttribute : Attribute
    {
        /// <summary>
        /// 创建一个实体类与数据库表的对应关系
        /// </summary>
        public DataEntityAttribute()
        {
        }

        /// <summary>
        /// 创建一个实体类与数据库表的对应关系
        /// </summary>
        /// <param name="sourceTable">数据库表</param>
        public DataEntityAttribute(string sourceTable)
        {
            this.sourceTable = sourceTable;
        }

        private string sourceTable;
        /// <summary>
        /// 获取实体类对应的数据库表
        /// </summary>
        public string SourceTable
        {
            get
            {
                return this.sourceTable;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DataPropertyAttribute : Attribute
    {
        #region 私有变量
        private string field = string.Empty;
        private int length = 0;
        private string defaultExpression = string.Empty;
        private bool isPrimaryKey = false;
        private bool generateValue = false;
        private string seqName = string.Empty;
        private DataFieldFlags fieldFlags = DataFieldFlags.ALL;
        private bool _isIdentifier = false;
        #endregion

        #region 属性
        /// <summary>
        /// 对应的数据库的对象名称
        /// </summary>
        public string Field
        {
            get
            {
                return field;
            }
            set
            {
                field = value.ToUpper();
            }
        }

        /// <summary>
        /// 数据库中对象的长度
        /// </summary>
        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        /// <summary>
        /// 如果该值为空，对应的数据库的缺省表达式。注意，如果该值为字符串，<b>不要忘记在前后加上引号</b>
        /// </summary>
        public string DefaultExpression
        {
            get
            {
                return defaultExpression;
            }
            set
            {
                defaultExpression = value;
            }
        }

        /// <summary>
        /// 该字段是否为关键字缺省是false
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                return isPrimaryKey;
            }
            set
            {
                isPrimaryKey = value;
            }
        }

        /// <summary>
        /// 如果在某一列上使用序列,设置序列的名称
        /// </summary>
        public bool GenerateValue
        {
            get
            {
                return this.generateValue;
            }
            set
            {
                this.generateValue = value;
            }

        }

        /// <summary>
        /// 如果此列使用序列，绑定的序列名称
        /// </summary>
        public String SeqName
        {
            get
            {
                return this.seqName;
            }
            set
            {
                this.seqName = value;
            }
        }

        /// <summary>
        /// 该字段是否是标示,因为有的时候有多个主键，所以增加这个单列标示的属性,
        /// 该字段有多个时，后一个会覆盖前一个
        /// </summary>
        public Boolean IsIdentifier
        {
            get
            {
                return _isIdentifier;
            }
            set
            {
                _isIdentifier = value;
            }
        }


        /// <summary>
        /// 字段与数据库的映射标识
        /// </summary>
        public DataFieldFlags FieldFlags
        {
            get
            {
                return this.fieldFlags;
            }
            set
            {
                this.fieldFlags = value;
            }

        }
        #endregion
    }

    /// <summary>
    /// 实体属性信息
    /// </summary>
    public class EntityPropertyInfo
    {
        public DataPropertyAttribute PropertyAttribute;
        public PropertyInfo PropertyInfo;
        public EntityInfo EntityInfo;
    }

    /// <summary>
    /// 实体信息
    /// </summary>
    public class EntityInfo
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName
        {
            get;
            set;
        }

        public string EntityFullName
        {
            get;
            set;
        }

        /// <summary>
        /// 实体存储表名
        /// </summary>
        public string DataTable
        {
            get;
            set;
        }

        public string Identifier
        {
            get;
            set;
        }

        public string InsertSql;
        private ConcurrentDictionary<string, EntityPropertyInfo> entityProperty = new ConcurrentDictionary<string, EntityPropertyInfo>();
        public ConcurrentDictionary<string, EntityPropertyInfo> EntityProperty
        {
            get
            {
                return this.entityProperty;
            }
        }
    }

    public static class EntityHelper
    {

        private static ConcurrentDictionary<string, EntityInfo> dict = new ConcurrentDictionary<string, EntityInfo>();

        private static object lockObj = new object();

        public static EntityInfo GetEntityInfo<T>(String fullName)
        {
            if (!dict.Keys.Contains(fullName) || dict[fullName] == null)
            {
                dict[fullName] = new EntityInfo<T>();
                dict[fullName].EntityFullName = fullName;
              
            }

            return dict[fullName];
        }

        /// <summary>
        /// 获取一个实体的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        public static EntityInfo GetEntityAttribute(Model.BaseModel m)
        {
            Type t = m.GetType();

            if (dict.ContainsKey(t.FullName))
            {
                return dict[t.FullName];
            }
            lock (lockObj)
            {
                if (dict.ContainsKey(t.FullName))
                {
                    return dict[t.FullName];
                }
                else
                {
                    EntityInfo result = new EntityInfo();
                    result.EntityFullName = t.FullName;
                    result.EntityName = t.Name;
                    if (ASoft.Reflect.HasAttribute<DataEntityAttribute>(t))
                    {
                        DataEntityAttribute attr = ASoft.Reflect.GetAttribute<DataEntityAttribute>(t);
                        result.DataTable = string.IsNullOrWhiteSpace(attr.SourceTable) ? t.Name : attr.SourceTable;
                    }
                    else
                    {
                        result.DataTable = t.Name;
                    }
                    result.InsertSql = "INSERT INTO " + result.DataTable + " (";
                    bool hasFieldFlag = false;
                   
                    foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (ASoft.Reflect.HasAttribute<DataPropertyAttribute>(pi))
                        {
                            DataPropertyAttribute dpa = ASoft.Reflect.GetAttribute<DataPropertyAttribute>(pi);
                            if (dpa.IsIdentifier)
                            {
                                result.Identifier = pi.Name;
                            }
                            if ((dpa.FieldFlags & DataFieldFlags.Insert) == 0)
                            {
                                continue;
                            }
                            result.EntityProperty.TryAdd(pi.Name, new EntityPropertyInfo());
                            //
                            result.EntityProperty[pi.Name].PropertyAttribute = ASoft.Reflect.GetAttribute<DataPropertyAttribute>(pi);
                            if (string.IsNullOrWhiteSpace(result.EntityProperty[pi.Name].PropertyAttribute.Field))
                            {
                                result.EntityProperty[pi.Name].PropertyAttribute.Field = pi.Name;
                            }
                            result.EntityProperty[pi.Name].PropertyInfo = pi;
                            result.InsertSql += hasFieldFlag ? "," : string.Empty;
                            result.InsertSql += result.EntityProperty[pi.Name].PropertyAttribute.Field;
                            hasFieldFlag = true;
                        }
                    }
                    result.InsertSql += ") VALUES ";
                    dict[t.Name] = result;
                    //dict.Add(t.Name, result);
                    return result;
                }
            }
        }
    }



    public class EntityInfo<TModel> : EntityInfo
    {

    }

    public class EntityMap<T> where T : class, new()
    {
        internal EntityInfo<T> EntityInfo
        {
            private set;
            get;
        }
        public EntityMap<T> ToTable(string tableName)
        {
            Type sourceType = typeof(T);
            var result = EntityHelper.GetEntityInfo<T>(sourceType.FullName) as EntityInfo<T>;
            result.EntityFullName = sourceType.FullName;
            result.EntityName = sourceType.Name;
            result.DataTable = tableName;
            this.EntityInfo = result;
            return this;
        }
    }

    public static class EntityMapExt
    {
        public static EntityPropertyInfo Property<T>(this EntityInfo<T> entityInfo,  MemberExpression expression)
        {
            if (entityInfo.EntityProperty != null)
            { 
                var name = expression.Member.Name;
                if (entityInfo.EntityProperty != null && !entityInfo.EntityProperty.Keys.Contains(name))
                {
                    var entityPropertyInfo = new EntityPropertyInfo();
                    entityPropertyInfo.PropertyInfo = typeof(T).GetProperty(name);
                    entityPropertyInfo.EntityInfo = entityInfo;
                    entityInfo.EntityProperty.TryAdd(name,entityPropertyInfo);
                }
                return entityInfo.EntityProperty[name];
            }
            return null;
        }

     

        public static EntityPropertyInfo Property<T, TResult>(this EntityMap<T> entityMap, Expression<Func<T, TResult>> expression)
           where T : class, new()
        { 
            var me = (MemberExpression)(expression.Body);
            return entityMap.EntityInfo.Property(me);
        }

        public static EntityPropertyInfo HasColumnName(this EntityPropertyInfo propertyInfo, string columnName)
        {
            propertyInfo.checkNullEntityPropertyInfo();

            propertyInfo.PropertyAttribute.Field = columnName;
            return propertyInfo;
        }

        public static EntityPropertyInfo Default(this EntityPropertyInfo propertyInfo, string defaultExpression)
        {
            propertyInfo.checkNullEntityPropertyInfo();

            propertyInfo.PropertyAttribute.DefaultExpression = defaultExpression;
            return propertyInfo;
        }

        public static EntityPropertyInfo Default(this EntityPropertyInfo propertyInfo, bool isIdentifier)
        {
            propertyInfo.checkNullEntityPropertyInfo();

            propertyInfo.PropertyAttribute.IsIdentifier = isIdentifier;
            return propertyInfo;
        }

        public static EntityPropertyInfo Length(this EntityPropertyInfo propertyInfo, int length)
        {
            propertyInfo.checkNullEntityPropertyInfo();

            propertyInfo.PropertyAttribute.Length = length;
            return propertyInfo;
        }

        /// <summary>
        /// 唯一值
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        //public static EntityPropertyInfo IsIdentifier(this EntityPropertyInfo propertyInfo, bool value=true)
        //{
        //    propertyInfo.requireEntityPropertyInfo(); 
        //    propertyInfo.PropertyAttribute.IsIdentifier = true;
        //    return propertyInfo;
        //}

        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EntityPropertyInfo IsPrimaryKey(this EntityPropertyInfo propertyInfo, bool value = true)
        {
            propertyInfo.checkNullEntityPropertyInfo();
            propertyInfo.PropertyAttribute.IsPrimaryKey = value;
            propertyInfo.PropertyAttribute.IsIdentifier = value;
            //ea.EntityProperty[ea.Identifier]
            // propertyInfo.EntityInfo.EntityProperty[]
            propertyInfo.EntityInfo.Identifier = propertyInfo.PropertyInfo.Name;
            return propertyInfo;
        }

        private static EntityPropertyInfo checkNullEntityPropertyInfo(this EntityPropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyAttribute == null)
            {
                propertyInfo.PropertyAttribute = new DataPropertyAttribute();
            }
            return propertyInfo;
        }
    }
}
