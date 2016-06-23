using ASoft.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Model;

namespace ASoft.Db
{
    /// <summary>
    /// 表
    /// </summary>
    [DataEntityAttribute("SYS_DATA_TABLE")]
    public class Table : BaseModel
    {
        /// <summary>
        /// 实体表名
        /// </summary>
        [DataProperty(Field = "ID")]
        public String ID { set; get; }
        /// <summary>
        /// 实体表名
        /// </summary>
        [DataProperty(Field = "TABLE_NAME")]
        public String TableName { set; get; }

        /// <summary>
        /// 主键字段
        /// </summary>
        public String PrimaryKey { set; get; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public String Title { set; get; }

        /// <summary>
        /// 实体类名
        /// </summary>
        [DataProperty(Field = "NAME")]
        public String Name { set; get; }

        /// <summary>
        /// 表备注
        /// </summary>
        [DataProperty(Field = "Comments")]
        public String Comments { set; get; }

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<TableField> FieldSet { set; get; }
    }

    /// <summary>
    /// 字段
    /// </summary>
    [DataEntityAttribute("SYS_DATA_FIELD")]
    public class TableField : BaseModel
    {
        [DataProperty(Field = "ID")]
        public String ColumnID { set; get; }
        /// <summary>
        /// 字段中文名称
        /// </summary>
        [DataProperty(Field = "TITLE")]
        public String Title { set; get; }

        /// <summary>
        /// 字段名称
        /// </summary>
        [DataProperty(Field = "column_name")]
        public String ColumnName { set; get; }

        /// <summary>
        /// 字段说明
        /// </summary>
        [DataProperty(Field = "Comments")]
        public String Comments { set; get; }


        /// <summary>
        /// 转换后的数据类型 
        /// </summary>
        [DataProperty(Field = "DATA_TYPE")]
        public DataType DataType { set; get; }

        /// <summary>
        /// 原始的数据类型
        /// </summary>
        public String DataTypeSource { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataProperty(Field = "TABLE_ID")]
        public String TableID { set; get; }



        /// <summary>
        /// 值
        /// </summary> 
        public object Value { set; get; }
    }



    /// <summary>
    /// 
    /// </summary>
    [DataEntityAttribute("SYS_DATA_OPERATION")]
    public class TableOperation : BaseModel
    {
        [DataProperty(Field = "ID")]
        public String ID { set; get; }

        [DataProperty(Field = "Operation_Date")]
        public DateTime OperationDate { set; get; }

        [DataProperty(Field = "User_ID")]
        public String UserID { set; get; }

        [DataProperty(Field = "Operation_Type")]
        public OperationType OperationType { set; get; }

        [DataProperty(Field = "Primary_Key")]
        public String PrimaryKey { set; get; }

        [DataProperty(Field = "Primary_Key_VALUE")]
        public String PrimaryKeyValue { set; get; }

        [DataProperty(Field = "Content")]
        public char[] Content { set; get; }

        [DataProperty(Field = "FILTER")]
        public string FilterWhere { set; get; }

        [DataProperty(Field = "IP")]
        public String IP { set; get; }

        [DataProperty(Field = "TYPE")]
        public int Type { set; get; }

        [DataProperty(Field = "TABLE_ID")]
        public String TableID { set; get; }

        [DataProperty(Field = "multiple")]
        public bool IsMultiple { set; get; }

        private Table _table;
        /// <summary>
        /// 操作的表
        /// </summary>
        public Table Table
        {
            get
            {
                if (_table == null && LazyTable != null)
                {
                    _table = LazyTable(this.TableID);
                }
                return _table;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Func<String, Table> LazyTable;

        private IUser _operationUser;
        /// <summary>
        /// 操作人
        /// </summary>
        public IUser OperationUser
        {
            get
            {
                if (_operationUser == null && LazyUser != null && !String.IsNullOrEmpty(UserID))
                {
                    LazyUser(this.UserID);
                }
                return _operationUser;
            }
        }
        public static Func<String, IUser> LazyUser;
    }


    /// <summary>
    /// 业务日志
    ///</summary>
    [DataEntityAttribute("SYS_DATA_BL_LOG")]
    public class BlLogData : BaseModel
    {

        /// <summary>
        /// 
        ///</summary>
        [DataProperty(Field = "REMARK")]
        public String Remark { set; get; }

        /// <summary>
        /// 
        ///</summary>
        [DataProperty(Field = "CREATE_USER")]
        public String CreateUser { set; get; }

        /// <summary>
        /// 
        ///</summary>
        [DataProperty(Field = "CONTENT")]
        public String Content { set; get; }

        /// <summary>
        /// 
        ///</summary>
        [DataProperty(Field = "CREATE_DATE")]
        public DateTime CreateDate { set; get; }

        /// <summary>
        /// 
        ///</summary>
        [DataProperty(Field = "TITLE")]
        public String Title { set; get; }

        /// <summary>
        /// 
        ///</summary>
        [DataProperty(Field = "ID")]
        public String Id { set; get; }

    }

    public interface IUser
    {
        String UserID { set; get; }

        String RealName { set; get; }
    }

    public enum DataType
    {
        Text = 0,
        Number = 1,
        Date = 2
    }
}
