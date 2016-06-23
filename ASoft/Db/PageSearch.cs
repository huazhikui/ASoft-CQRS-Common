using System;
using System.Collections.Generic;
using System.Text;

namespace ASoft.Db
{
    /// <summary>
    /// 数据库查询条件
    /// </summary>
    public class PageSearch
    {
        private StringBuilder sbsql = null;

        #region 查询字段
        private string fieldsName = "*";
        /// <summary>
        /// 查询字段(如果不设置,默认返回*)
        /// </summary>
        public string FieldsName
        {
            get
            {
                return this.fieldsName;
            }
            set
            {
                this.fieldsName = (value ?? "*").Trim();
            }
        }
        #endregion

        #region 表名称
        private string tableName = string.Empty;
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName
        {
            get
            {
                return this.tableName;
            }
            set
            {
                this.tableName = (value ?? string.Empty).Trim();
            }
        }
        #endregion

        #region 主键字段
        private string primaryKey = string.Empty;
        /// <summary>
        /// 主键字段
        /// </summary>
        public string PrimaryKey
        {
            get
            {
                return this.primaryKey;
            }
            set
            {
                this.primaryKey = (value ?? string.Empty).Trim();
            }
        }
        #endregion

        #region 排序字段

        private OrderFieldCollection orderField = new OrderFieldCollection();
        /// <summary>
        /// 排序字段
        /// </summary>
        public OrderFieldCollection OrderField
        {
            get
            {
                return this.orderField;
            }
            set {
                this.orderField = value;
            }
        }
        #endregion

        #region 查询条件
        /// <summary>
        /// 查询条件,多表查询时,请在字段前加上表别名
        /// </summary>
        public string Where
        {
            get
            {
                if (sbsql != null)
                {
                    return sbsql.ToString();
                }
                return string.Empty;
            }
        }

 

        
        /// <summary>
        /// 为查询语句的Where子句添加一个And条件(前面不用加AND)
        /// </summary>
        /// <param name="sql">SQL子句</param>
        /// <param name="Params">参数</param>
        /// <returns></returns>
        public void And(string sql, params object[] args)
        {
            if (sql == null || sql.Trim().Length == 0)
            {
                return;
            }
            if (args != null && args.Length > 0)
            {
                sql = string.Format(sql, args);
            }
            if (sbsql == null)
            {
                sbsql = new StringBuilder();
            }
            if (sql.TrimStart().ToUpper().StartsWith("WHERE "))
            {
                sql = sql.TrimStart().Substring(5);
            }
            if (sql.TrimStart().ToUpper().StartsWith("AND "))
            {
                sql = sql.TrimStart().Substring(3);
            }
            if (sbsql.Length > 0)
            {
                sbsql.Append(" AND ");
            }
            sbsql.Append(sql);
        }
        #endregion

        #region 分组条件
        /// <summary>
        /// 查询条件,多表查询时,请在字段前加上表别名
        /// </summary>
        public string GroupBy
        {
            set;
            get;
        }
        #endregion

        #region 当前页码
        private int pageIndex = 1;
        /// <summary>
        /// 当前页码(从1开始)
        /// </summary>
        public int PageIndex
        {
            get
            {
                return this.pageIndex;
            }
            set
            {
                this.pageIndex = value > 1 ? value : 1;
            }
        }
        #endregion

        #region 每页数量
        private int pageSize = 15;
        /// <summary>
        /// 每页数量(默认15),等于0时,查询全部
        /// </summary>
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value < 0)
                {
                    pageSize = Math.Abs(value);
                }
                else
                {
                    pageSize = value;
                }
            }
        }
        #endregion

        #region 记录总数
        private int totalCount;
        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.totalCount;
            }
            set
            {
                this.totalCount = value > 0 ? value : 0;
            }
        }
        #endregion

        #region 页数
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                {
                    return 1;
                }
                return (totalCount + pageSize - 1) / pageSize;
            }
        }
        #endregion



        public StringBuilder executeSql = new StringBuilder();
        /// <summary>
        /// 获取在数据库上执行的SQL命令
        /// </summary>
        public string ExecuteSql
        {
            get
            {
                return executeSql.ToString();
            }
        }
    }

    /// <summary>
    /// 排序字段集合
    /// </summary>
    public class OrderFieldCollection
    {
        List<KeyValuePair<string, OrderType>> dict = null;

        /// <summary>
        /// 添加排序字段,默认使用升序
        /// </summary>
        /// <param name="field"></param>
        public void Add(string field)
        {
            this.Add(field, OrderType.ASC);
        }

        /// <summary>
        /// 添加一个排序字段
        /// </summary>
        /// <param name="field">排序字段</param>
        /// <param name="type">排序类型</param>
        public void Add(string field, OrderType type)
        {
            if (dict == null)
            {
                dict = new List<KeyValuePair<string, OrderType>>();
            }
            for (int i = 0; i < dict.Count; i++)
            {
                if (dict[i].Key.Equals(field, StringComparison.OrdinalIgnoreCase))
                {
                    dict[i] = new KeyValuePair<string, OrderType>(field.ToUpper(), type);
                    return;
                }
            }
            dict.Add(new KeyValuePair<string, OrderType>(field.ToUpper(), type));
        }

        /// <summary>
        /// 移除一个排序字段
        /// </summary>
        /// <param name="field"></param>
        public void Remove(string field)
        {
            for (int i = 0; i < dict.Count; i++)
            {
                if (dict[i].Key.Equals(field, StringComparison.OrdinalIgnoreCase))
                {
                    dict.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 移除一个排序字段
        /// </summary>
        /// <param name="index">要移除的排序字段的索引</param>
        public void Remove(int index)
        {
            if (dict == null || index >= dict.Count)
            {
                return;
            }
            dict.RemoveAt(index);
        }

        /// <summary>
        /// 把一个要排序的字段放到最前面
        /// </summary>
        /// <param name="field"></param>
        public void MoveFirst(string field)
        {
            for (int i = 0; i < dict.Count; i++)
            {
                if (dict[i].Key.Equals(field, StringComparison.OrdinalIgnoreCase))
                {
                    KeyValuePair<string, OrderType> item = dict[i];
                    dict.RemoveAt(i);
                    dict.Insert(0, item);
                }
            }
        }

        /// <summary>
        /// 将排序字段列表中指定位置的排序字段移动到最前面
        /// </summary>
        /// <param name="index">要移动到最前面的排序字段的索引</param>
        public void MoveFirst(int index)
        {
            if (dict == null || index >= dict.Count || index == 0)
            {
                return;
            }
            KeyValuePair<string, OrderType> item = dict[index];
            dict.RemoveAt(index);
            dict.Insert(0, item);
        }

        /// <summary>
        /// 要排序字段个数
        /// </summary>
        public int Count
        {
            get
            {
                return dict == null ? 0 : dict.Count;
            }
        }

        /// <summary>
        /// 生成排序的SQL
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (dict == null || dict.Count == 0)
            {
                return string.Empty;
            }
            return " ORDER BY " + ASoft.Text.StringUtils.Join<KeyValuePair<string, OrderType>>(dict, ",", FiledOrderJoinAction);
        }

        private static string FiledOrderJoinAction(KeyValuePair<string, OrderType> obj)
        {
            return obj.Key + " " + obj.Value;
        }
    }

    /// <summary>
    /// 字段排序类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 升序排序
        /// </summary>
        ASC,

        /// <summary>
        /// 降序排序
        /// </summary>
        DESC
    }
}
