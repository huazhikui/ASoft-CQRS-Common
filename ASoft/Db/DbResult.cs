using System;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
namespace ASoft.Db
{
    /// <summary>
    /// 数据库命令的参数集合
    /// </summary>
    public class DataParameterCollection : IDisposable, IEnumerable
    {
        /// <summary>
        /// 内部构造器
        /// </summary>
        /// <param name="parms"></param>
        public DataParameterCollection(IDataParameterCollection parms)
        {
            this.parameters = parms;
        }
        private IDataParameterCollection parameters = null;

        /// <summary>
        /// 获取指定位置处的数据库命令参数
        /// </summary>
        /// <param name="index">指定位置</param>
        /// <returns>数据库命令参数</returns>
        public IDbDataParameter this[int index]
        {
            get
            {
                if (parameters != null)
                {
                    return (IDbDataParameter)this.parameters[index];
                }
                return null;
            }
        }

        /// <summary>
        /// 获取指定名称的数据库命令参数
        /// </summary>
        /// <param name="index">定名称</param>
        /// <returns>数据库命令参数</returns>
        public IDbDataParameter this[string index]
        {
            get
            {
                if (parameters != null)
                {
                    return (IDbDataParameter)this.parameters[index];
                }
                return null;
            }
        }

        /// <summary>
        /// 检查是否包含某个指定名称的参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            if (parameters != null)
            {
                return parameters.Contains(name);
            }
            return false;
        }

        /// <summary>
        /// 清空参数列表
        /// </summary>
        public void Clear()
        {
            if (parameters != null)
            {
                parameters.Clear();
            }
        }

        /// <summary>
        /// 返回参数的个数
        /// </summary>
        public int Count
        {
            get
            {
                return parameters != null ? parameters.Count : 0;
            }
        }

        #region IDisposable 成员
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.parameters != null)
            {
                this.parameters.Clear();
                this.parameters = null;
            }
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 获取枚举的访问器
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// 数据库命令执行产生的结果
    /// </summary>
    public class ResultBase : IDisposable
    {
        public string commandText = null;
        /// <summary>
        /// 数据库命令
        /// </summary>
        public string CommandText
        {
            get
            {
                return commandText;
            }
        }

        public CommandType commandType;
        /// <summary>
        /// 数据库命令的类型
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }

        public DataParameterCollection parameters = null;

        /// <summary>
        /// 获取数据库命令的参数
        /// </summary>
        public DataParameterCollection Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        /// 根据Command命令生成一个结果
        /// <para>
        /// 如果数据库命令的参数中有输出参数,则会记录所有的参数信息,否则不会记录参数
        /// </para>
        /// </summary>
        /// <param name="command">Command命令</param>
        public ResultBase(IDbCommand command)
        {
            if (command != null)
            {
                this.commandText = command.CommandText;
                this.commandType = command.CommandType;
                this.parameters = new DataParameterCollection(command.Parameters);
                command.Dispose();
            }
        }

        /// <summary>
        /// 根据Command命令生成一个结果
        /// <para>
        /// 如果数据库命令的参数中有输出参数,则会记录所有的参数信息,否则不会记录参数
        /// </para>
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        public ResultBase(string commandText, CommandType commandType, IDataParameterCollection parameters)
        {
            this.commandText = commandText;
            this.commandType = commandType;
            this.parameters = new DataParameterCollection(parameters);
        }

        /// <summary>
        /// 根据Command命令生成一个结果
        /// <para>
        /// 如果数据库命令的参数中有输出参数,则会记录所有的参数信息,否则不会记录参数
        /// </para>
        /// </summary>
        /// <param name="commandText">命令文本</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        public ResultBase(string commandText, CommandType commandType, DataParameterCollection parameters)
        {
            this.commandText = commandText;
            this.commandType = commandType;
            this.parameters = parameters;
        }

        #region IDisposable Members
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.commandText = null;
            if (this.parameters != null)
            {
                this.parameters.Dispose();
            }
        }

        #endregion
    }

    #region NonQueryResult
    /// <summary>
    /// 执行Command对象的ExecuteNonQuery()方法产生的结果
    /// </summary>
    public class NonQueryResult : ResultBase, IDisposable
    {
        private StringBuilder sbdesc = null;
        private int value = 0;
        /// <summary>
        /// 数据库命令执行后的返回值
        /// </summary>
        public int Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 使用一个数据库命令的执行后的返回值和数据库命令初始化对象
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="command">命令</param>
        public NonQueryResult(int result, IDbCommand command)
            : base(command)
        {
            this.value = result;
            if (ASoft.LogAdapter.CanDebug)
            {
                ASoft.LogAdapter.Db.Debug(this.Description);
            }
        }

        /// <summary>
        /// 结果描述
        /// </summary>
        public string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库返回结果信息如下");
                    sbdesc.AppendLine("类型类型:" + CommandType);
                    sbdesc.AppendLine("命令文本:" + CommandText);
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        sbdesc.AppendLine("参数列表:");
                        int max = 0;
                        foreach (IDataParameter p in Parameters)
                        {
                            max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                        }
                        foreach (IDataParameter p in Parameters)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                        }
                    }
                    sbdesc.AppendLine("影响行数:" + value);
                }
                return sbdesc.ToString();
            }
        }

        #region IDisposable Members

        /// <summary>
        /// 释放资源
        /// </summary>
        void IDisposable.Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
    #endregion

    #region ScalerResult
    /// <summary>
    /// 执行Command对象的ExecuteScaler()方法产生的结果
    /// </summary>
    public class ScalerResult : ResultBase, IDisposable
    {
        private StringBuilder sbdesc = null;
        private object value;
        /// <summary>
        /// 数据库命令执行后的返回值
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 使用一个数据库命令的执行后的返回值和数据库命令初始化对象
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="command">命令</param>
        public ScalerResult(object result, IDbCommand command)
            : base(command)
        {
            this.value = result;
            if (ASoft.LogAdapter.CanDebug)
            {
                ASoft.LogAdapter.Db.Debug(this.Description);
            }
        }

        /// <summary>
        /// 检测返回的值是否为null或者DBNull.Value(如果数据库没有返回行数据,则为null,如果数据库返回了行数据,但为NULL,则为DBNull.Value
        /// </summary>
        public bool IsNullOrDbNull
        {
            get
            {
                return value == null || Convert.IsDBNull(value);
            }
        }

        /// <summary>
        /// 如果数据库没有返回行数据,则返回true,否则返回false
        /// </summary>
        public bool IsNull
        {
            get
            {
                return value == null;
            }
        }

        /// <summary>
        /// 如果数据返回了数据,但第一行第一列为NULL,则返回true,否则返回false
        /// </summary>
        public bool IsDBNull
        {
            get
            {
                return Convert.IsDBNull(value);
            }
        }

        /// <summary>
        /// 返回Int32类型的值
        /// </summary>
        public int IntValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToInt32(this.value);
            }
        }

        /// <summary>
        /// 返回Int64类型的值
        /// </summary>
        public long LongValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToInt64(this.value);
            }
        }

        /// <summary>
        /// 返回浮点类型的值
        /// </summary>
        public float FloatValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToSingle(this.value);
            }
        }

        /// <summary>
        /// 返回双精度类型的值
        /// </summary>
        public double DoubleValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToDouble(this.value);
            }
        }

        public Decimal DecimalValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToDecimal(this.value);
            }
        }


        /// <summary>
        /// 返回布尔类型的值(只检查是否为true,t,1)
        /// </summary>
        public bool BoolValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                string val = this.StringValue.ToLower();
                return val == "true" || val == "1" || val == "t";
            }
        }

        /// <summary>
        /// 返回时间类型的值
        /// </summary>
        public DateTime DateTimeValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToDateTime(this.value);
            }
        }

        /// <summary>
        /// 返回String类型的值
        /// </summary>
        public string StringValue
        {
            get
            {
                if (IsNullOrDbNull)
                {
                    throw new Exception("从数据库返回了NULL或DBNULL数据");
                }
                return Convert.ToString(this.value);
            }
        }

        /// <summary>
        /// 结果描述
        /// </summary>
        public string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库返回结果信息如下");
                    sbdesc.AppendLine("类型类型:" + CommandType);
                    sbdesc.AppendLine("命令文本:" + CommandText);
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        sbdesc.AppendLine("参数列表:");
                        int max = 0;
                        foreach (IDataParameter p in Parameters)
                        {
                            max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                        }
                        foreach (IDataParameter p in Parameters)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                        }
                    }
                    sbdesc.AppendLine("查询结果:" + value);
                }
                return sbdesc.ToString();
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>结果转换后的字符串</returns>
        public override string ToString()
        {
            return this.value.ToString();
        }

        #region IDisposable Members

        /// <summary>
        /// 释放资源
        /// </summary>
        void IDisposable.Dispose()
        {
            this.value = null;
            base.Dispose();
        }

        #endregion
    }
    #endregion

    #region DataTableResult
    /// <summary>
    /// 执行CommandAdapter对象填充的表结果
    /// </summary>
    public class DataTableResult : ResultBase, IListSource, IDisposable
    {
        private StringBuilder sbdesc = null;
        private DataTable value;
        /// <summary>
        /// 数据库命令执行后的返回值
        /// </summary>
        public DataTable Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 使用一个数据库命令的执行后的返回值和数据库命令初始化对象
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="command">命令</param>
        public DataTableResult(DataTable result, IDbCommand command)
            : base(command)
        {
            this.value = result;
            if (ASoft.LogAdapter.CanDebug)
            {
                ASoft.LogAdapter.Db.Debug(this.Description);
            }
        }

        public DataTableResult(DataTable result, string commandText, CommandType commandType, IDataParameterCollection parameters)
            : base(commandText, commandType, parameters)
        {
            this.value = result;
        }

        public DataTableResult(DataTable result, string commandText, CommandType commandType, DataParameterCollection parameters)
            : base(commandText, commandType, parameters)
        {
            this.value = result;
        }

        /// <summary>
        /// 结果描述
        /// </summary>
        public string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库返回结果信息如下");
                    sbdesc.AppendLine("类型类型:" + CommandType);
                    sbdesc.AppendLine("命令文本:" + CommandText);
                    int max = 0;
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        sbdesc.AppendLine("参数列表:");
                        foreach (IDataParameter p in Parameters)
                        {
                            max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                        }
                        foreach (IDataParameter p in Parameters)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                        }
                    }
                    max = 0;
                    sbdesc.AppendLine("结果行数:" + value.Rows.Count.ToString());
                    sbdesc.AppendLine("各列类型:");
                    foreach (DataColumn dc in value.Columns)
                    {
                        max = dc.ColumnName.Length > max ? dc.ColumnName.Length : max;
                    }
                    foreach (DataColumn dc in value.Columns)
                    {
                        sbdesc.AppendLine(string.Format("{0} : {1}", dc.ColumnName.PadRight(max, ' '), dc.DataType.ToString().PadRight(11, ' ')));
                    }
                }
                return sbdesc.ToString();
            }
        }

        #region IDisposable Members

        /// <summary>
        /// 释放资源
        /// </summary>
        void IDisposable.Dispose()
        {
            this.value.Dispose();
            base.Dispose();
        }

        #endregion

        #region IListSource Members

        /// <summary>
        /// 集合是 IList 对象集合
        /// </summary>
        public bool ContainsListCollection
        {
            get { return (this.value as IListSource).ContainsListCollection; }
        }

        /// <summary>
        /// 返回一个数据绑定控件可用的数据源
        /// </summary>
        /// <returns>数据源</returns>
        public IList GetList()
        {
            return (this.value as IListSource).GetList();
        }


        #endregion
    }
    #endregion

    #region DataSetResult
    /// <summary>
    /// 执行CommandAdapter对象填充的数据集结果
    /// </summary>
    public class DataSetResult : ResultBase, IListSource, IDisposable
    {
        private StringBuilder sbdesc = null;
        private DataSet value;
        /// <summary>
        /// 数据库命令执行后的返回值
        /// </summary>
        public DataSet Result
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 使用一个数据库命令的执行后的返回值和数据库命令初始化对象
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="command">命令</param>
        public DataSetResult(DataSet result, IDbCommand command)
            : base(command)
        {
            this.value = result;
            if (ASoft.LogAdapter.CanDebug)
            {
                ASoft.LogAdapter.Db.Debug(this.Description);
            }
        }

        /// <summary>
        /// 结果描述
        /// </summary>
        public string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库返回结果信息如下");
                    sbdesc.AppendLine("类型类型:" + CommandType);
                    sbdesc.AppendLine("命令文本:" + CommandText);
                    int max = 0;
                    if (Parameters != null && Parameters.Count > 0)
                    {
                        sbdesc.AppendLine("参数列表:");
                        foreach (IDataParameter p in Parameters)
                        {
                            max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                        }
                        foreach (IDataParameter p in Parameters)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                        }
                    }
                    foreach (DataTable dt in value.Tables)
                    {
                        max = 0;
                        sbdesc.AppendLine("表名    :" + dt.TableName);
                        sbdesc.AppendLine("结果行数:" + dt.Rows.Count);
                        sbdesc.AppendLine("各列类型:");
                        foreach (DataColumn dc in dt.Columns)
                        {
                            max = dc.ColumnName.Length > max ? dc.ColumnName.Length : max;
                        }
                        foreach (DataColumn dc in dt.Columns)
                        {
                            sbdesc.AppendLine(string.Format("{0} : {1}", dc.ColumnName.PadRight(max, ' '), dc.DataType.ToString().PadRight(11, ' ')));
                        }
                    }
                }
                return sbdesc.ToString();
            }
        }

        #region IDisposable Members

        /// <summary>
        /// 释放资源
        /// </summary>
        void IDisposable.Dispose()
        {
            this.sbdesc.Remove(0, this.sbdesc.Length);
            this.value.Dispose();
            base.Dispose();
        }
        #endregion

        #region IListSource Members

        /// <summary>
        /// 集合是 IList 对象集合(true)
        /// </summary>
        public bool ContainsListCollection
        {
            get { return (this.value as IListSource).ContainsListCollection; }
        }

        /// <summary>
        /// 返回一个数据绑定控件可用的数据源
        /// </summary>
        /// <returns>数据源</returns>
        public IList GetList()
        {
            return (this.value as IListSource).GetList();
        }

        #endregion
    }
    #endregion

    #region DataReader
    /// <summary>
    /// 包装的数据读取器
    /// </summary>
    public class DataReader : ResultBase, IDisposable, IEnumerable
    {
        private IDataReader dr;
        private DbDataReader dbdr = null;
        private int rowidx = -1;
        private List<string> columns = new List<string>();

        /// <summary>
        /// 根据数据读取器和相对应的Command生成一个包装后的数据读取器对象
        /// </summary>
        /// <param name="dr">原始数据读取器</param>
        /// <param name="command">数据命令相关信息</param>
        public DataReader(IDataReader dr, IDbCommand command)
            : base(command)
        {
            this.dr = dr;
            this.dbdr = dr as DbDataReader;
            this.RefreshSchema();
            if (ASoft.LogAdapter.CanDebug)
            {
                ASoft.LogAdapter.Db.Debug(ASoft.LogFileSpan.Day, this.Description);
            }
        }

        private void RefreshSchema()
        {
            columns.Clear();
            DataTable dt = this.dr.GetSchemaTable();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName.ToUpper());
            }
        }

        /// <summary>
        /// 检查数据读取器是否包含某个指定的字段
        /// </summary>
        /// <param name="filedName">要检查的字段的名称</param>
        /// <returns>返回一个值,指示是否包含这个字段</returns>
        public bool HasField(string filedName)
        {
            return columns.Contains(filedName.ToUpper());
        }

        /// <summary>
        /// 指示数据读取器的游标向前移动指定长度
        /// </summary>
        /// <param name="skip">游标移动长度</param>
        public void Skip(long skip)
        {
            for (; skip > 0 && dr.Read(); rowidx++, skip--) ;
        }

        /// <summary>
        /// 原始的DataReader对象
        /// </summary>
        public IDataReader OriDataReader
        {
            get
            {
                return this.dr;
            }
        }

        /// <summary>
        /// 返回指定列的名称
        /// </summary>
        /// <param name="index">列的序号</param>
        /// <returns>指定列的名称</returns>
        public string GetName(int index)
        {
            return this.dr.GetName(index);
        }

        /// <summary>
        /// 返回指定列的序号
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>列的序号</returns>
        public int GetOrdinal(string name)
        {
            try
            {
                return this.dr.GetOrdinal(name);
            }
            catch
            {
                throw new Exception(string.Format("未找到数据读取器字段:{1}{0}{2}", Environment.NewLine, name, Description));
            }
        }

        /// <summary>
        /// 检查数据读取器的某个字段是否为DBNull
        /// </summary>
        /// <param name="name"></param>
        private void CheckFieldNotNull(string name)
        {
            if (IsDBNull(name))
            {
                throw new Exception(string.Format("要获取的列{1}的为DBNull{0}{2}", Environment.NewLine, name, Description));
            }
        }

        /// <summary>
        /// 检查数据读取器的某个字段是否为DBNull
        /// </summary>
        /// <param name="ordinal">列索引</param>
        private void CheckFieldNotNull(int ordinal)
        {
            if (dr.IsDBNull(ordinal))
            {
                throw new Exception(string.Format("要获取的列{1}的为DBNull{0}{2}", Environment.NewLine, ordinal, Description));
            }
        }

        /// <summary>
        /// 执行记取命令
        /// </summary>
        /// <returns>返回一人值,指示DataReader是否成功读取了一条数据</returns>
        public bool Read()
        {
            if (this.dr.Read())
            {
                rowidx++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 行索引值(默认为-1,当读取了第一行行,变为0)
        /// </summary>
        public int CurrentRowIndex
        {
            get
            {
                return this.rowidx;
            }
        }

        #region GetBoolean

        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(string name)
        {
            CheckFieldNotNull(name);
            string o = this.dr[name].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            string o = this.dr[ordinal].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);

        }

        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(string name, bool defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetBoolean(name);
        }

        /// <summary>
        /// 返回bool
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public bool GetBoolean(int ordinal, bool defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetBoolean(ordinal);
        }

        /// <summary>
        /// 返回指定列的Boolean值,如果指定列为DBNull,则返回null
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public bool? GetBooleanNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            string o = this.dr[name].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回指定列的Boolean值,如果指定列为DBNull,则返回null
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public bool? GetBooleanNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            string o = this.dr[ordinal].ToString().Trim();
            return o == "1" || o.Equals("T", StringComparison.OrdinalIgnoreCase) || o.Equals("True", StringComparison.OrdinalIgnoreCase) || o.Equals("Y", StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region GetByte

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetByte(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetByte(ordinal);
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(string name, byte defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetByte(name);
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public byte GetByte(int ordinal, byte defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetByte(ordinal);
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public byte? GetByteNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetByte(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回byte
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public byte? GetByteNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetByte(ordinal);
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">缓冲区</param>
        /// <param name="bufferoffset">缓冲区偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>实际读取的内容的长度</returns>
        public long GetBytes(string name, long fieldoffset, byte[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(name);
            return this.dr.GetBytes(this.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">缓冲区</param>
        /// <param name="bufferoffset">缓冲区偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>实际读取的内容的长度</returns>
        public long GetBytes(int ordinal, long fieldoffset, byte[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetBytes(ordinal, fieldoffset, buffer, bufferoffset, length);
        }


        #endregion

        #region GetChars

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetChar(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetChar(ordinal);
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(string name, char defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetChar(name);
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public char GetChar(int ordinal, char defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetChar(ordinal);
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public char? GetCharNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetChar(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Char
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public char? GetCharNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetChar(ordinal);
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">目标</param>
        /// <param name="bufferoffset">目标偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>读取的字节长度</returns>
        public long GetChars(string name, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(name);
            return this.dr.GetChars(this.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="fieldoffset">偏移量</param>
        /// <param name="buffer">目标</param>
        /// <param name="bufferoffset">目标偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>读取的字节长度</returns>
        public long GetChars(int ordinal, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetChars(ordinal, fieldoffset, buffer, bufferoffset, length);
        }

        #endregion

        #region GetInt16

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetInt16(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetInt16(ordinal);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToInt16(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetInt16(name);
            }
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToInt16(this[ordinal]);
            }
            else
            {
                return GetInt16(ordinal);
            }
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name, short defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt16(name);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal, short defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt16(ordinal);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(string name, short defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt16(name, convert);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt16来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public short GetInt16(int ordinal, short defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt16(ordinal, convert);
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public short? GetInt16Null(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetInt16(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int16
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public short? GetInt16Null(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetInt16(ordinal);
        }
        #endregion

        #region GetInt32

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetInt32(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetInt32(ordinal);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToInt32(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetInt32(name);
            }
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToInt32(this[ordinal]);
            }
            else
            {
                return GetInt32(ordinal);
            }
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name, int defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt32(name);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal, int defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt32(ordinal);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(string name, int defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt32(name, convert);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt32来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public int GetInt32(int ordinal, int defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt32(ordinal, convert);
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public int? GetInt32Null(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetInt32(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回int
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public int? GetInt32Null(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetInt32(ordinal);
        }
        #endregion

        #region GetInt64

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetInt64(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetInt64(ordinal);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToInt64(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetInt64(name);
            }
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToInt64(this[ordinal]);
            }
            else
            {
                return GetInt64(ordinal);
            }
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name, long defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt64(name);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal, long defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt64(ordinal);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(string name, long defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetInt64(name, convert);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToInt64来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public long GetInt64(int ordinal, long defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetInt64(ordinal, convert);
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public long? GetInt64Null(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetInt64(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Int64
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public long? GetInt64Null(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetInt64(ordinal);
        }
        #endregion

        #region GetFloat

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetFloat(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetFloat(ordinal);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToSingle(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetFloat(name);
            }
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToSingle(this[ordinal]);
            }
            else
            {
                return GetFloat(ordinal);
            }
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name, float defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetFloat(name);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal, float defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetFloat(ordinal);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(string name, float defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetFloat(name, convert);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToSingle来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public float GetFloat(int ordinal, float defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetFloat(ordinal, convert);
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public float? GetFloatNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetFloat(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回float
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public float? GetFloatNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetFloat(ordinal);
        }

        #endregion

        #region GetDouble

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetDouble(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetDouble(ordinal);
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToDouble来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToDouble(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetDouble(name);
            }
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToDouble来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToDouble(this[ordinal]);
            }
            else
            {
                return GetDouble(ordinal);
            }
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name, double defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetDouble(name);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal, double defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDouble(ordinal);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(string name, double defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetDouble(name, convert);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public double GetDouble(int ordinal, double defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDouble(ordinal, convert);
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public double? GetDoubleNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return GetDouble(this.GetOrdinal(name), true);
        }

        /// <summary>
        /// 返回double
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public double? GetDoubleNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetDouble(ordinal);
        }
        #endregion

        #region GetDecimal

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetDecimal(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetDecimal(ordinal);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToDecimal(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetDecimal(name);
            }
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToDecimal(this[ordinal]);
            }
            else
            {
                return GetDecimal(ordinal);
            }
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name, decimal defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetDecimal(name);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal, decimal defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDecimal(ordinal);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(string name, decimal defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetDecimal(name, convert);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDecimal来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public decimal GetDecimal(int ordinal, decimal defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDecimal(ordinal, convert);
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public decimal? GetDecimalNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            };
            return this.dr.GetDecimal(this.GetOrdinal(name));
        }

        /// <summary>
        /// 返回decimal
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public decimal? GetDecimalNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetDecimal(ordinal);
        }
        #endregion

        #region GetDateTime

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetDateTime(this.dr.GetOrdinal(name));
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetDateTime(ordinal);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name, bool convert)
        {
            CheckFieldNotNull(name);
            if (convert)
            {
                return Convert.ToDateTime(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetDateTime(name);
            }
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal, bool convert)
        {
            CheckFieldNotNull(ordinal);
            if (convert)
            {
                return Convert.ToDateTime(this[ordinal]);
            }
            else
            {
                return GetDateTime(ordinal);
            }
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name, DateTime defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : GetDateTime(name);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal, DateTime defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDateTime(ordinal);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(string name, DateTime defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetDateTime(name, convert);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToDateTime来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public DateTime GetDateTime(int ordinal, DateTime defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetDateTime(ordinal, convert);
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public DateTime? GetDateTimeNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetDateTime(this.dr.GetOrdinal(name));
        }

        /// <summary>
        /// 返回DateTime
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public DateTime? GetDateTimeNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetDateTime(ordinal);
        }
        #endregion

        #region GetGuid

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(string name)
        {
            CheckFieldNotNull(name);
            return this.dr.GetGuid(this.dr.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return this.dr.GetGuid(ordinal);
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(string name, Guid defaultValue)
        {
            return this.IsDBNull(name) ? defaultValue : this.dr.GetGuid(this.dr.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public Guid GetGuid(int ordinal, Guid defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : this.dr.GetGuid(ordinal);
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public Guid? GetGuidNull(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetGuid(this.dr.GetOrdinal(name));
        }

        /// <summary>
        /// 返回Guid
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public Guid? GetGuidNull(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetGuid(ordinal);
        }

        #endregion

        #region GetString

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            return this.dr.GetString(this.dr.GetOrdinal(name));
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            return this.dr.GetString(ordinal);
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name, bool convert)
        {
            if (this.IsDBNull(name))
            {
                return null;
            }
            if (convert)
            {
                return Convert.ToString(this[this.GetOrdinal(name)]);
            }
            else
            {
                return GetString(name);
            }
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal, bool convert)
        {
            if (this.IsDBNull(ordinal))
            {
                return null;
            }
            if (convert)
            {
                return Convert.ToString(this[ordinal]);
            }
            else
            {
                return GetString(ordinal);
            }
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name, string defaultValue)
        {
            return (this.IsDBNull(name) ? defaultValue : this.GetString(name));
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal, string defaultValue)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetString(ordinal);
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(string name, string defaultValue, bool convert)
        {
            return this.IsDBNull(name) ? defaultValue : GetString(name, convert);
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="convert">是否使用Convert.ToString来转换返回的结果</param>
        /// <returns>要查找的列的值</returns>
        public string GetString(int ordinal, string defaultValue, bool convert)
        {
            return this.IsDBNull(ordinal) ? defaultValue : GetString(ordinal, convert);
        }

        #endregion

        #region GetValue

        /// <summary>
        /// 返回指定列的值
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns></returns>
        public object GetValue(string name)
        {
            CheckFieldNotNull(name);
            return dr.GetValue(dr.GetOrdinal(name));
        }

        /// <summary>
        /// 返回指定列的值
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns></returns>
        public object GetValue(int ordinal)
        {
            CheckFieldNotNull(ordinal);
            return dr.GetValue(ordinal);
        }

        #endregion

        #region GetFieldType

        /// <summary>
        /// 获取指定列的数据类型
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns></returns>
        public Type GetFieldType(int ordinal)
        {
            return this.dr.GetFieldType(ordinal);
        }

        /// <summary>
        /// 获取指定列的数据类型
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns></returns>
        public Type GetFieldType(string name)
        {
            return this.dr.GetFieldType(this.dr.GetOrdinal(name));
        }

        #endregion

        /// <summary>
        /// 数据读取器的字段个数
        /// </summary>
        public int FieldCount
        {
            get
            {
                return dr.FieldCount;
            }
        }

        /// <summary>
        /// 返回一个值,指示检索到的结果中是否包含行结果.
        /// <para>
        /// 如果数据读取器不支持此操作,则抛出一个异常
        /// </para>
        /// </summary>
        public bool HasRows
        {
            get
            {
                if (this.dbdr != null)
                {
                    return this.dbdr.HasRows;
                }
                return (bool)ASoft.Reflect.GetProperty(this.dr, "HasRows");
            }
        }

        /// <summary>
        /// 当读取批处理 Transact-SQL 语句的结果时，使数据读取器前进到下一个结果
        /// </summary>
        /// <returns></returns>
        public bool NextResult()
        {
            this.rowidx = -1;
            bool result = !dr.IsClosed && dr.NextResult();
            if (result)
            {
                this.RefreshSchema();
            }
            return result;
        }

        /// <summary>
        /// 获取结果集的构架信息
        /// </summary>
        /// <returns>结果集的构架信息</returns>
        public DataTable GetSchemaTable()
        {
            return this.dr.GetSchemaTable();
        }

        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return dr.RecordsAffected;
            }
        }

        /// <summary>
        /// 获取一个值，用于指示当前行的嵌套深度
        /// </summary>
        public int Depth
        {
            get
            {
                return dr.Depth;
            }
        }

        /// <summary>
        /// 检查数据读取器是否已关闭
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return dr.IsClosed;
            }
        }

        /// <summary>
        /// 检查某一列是否是DBNull值
        /// </summary>
        /// <param name="name">列名</param>
        /// <returns>返回一个值,指示获取的列是否为NULL</returns>
        public bool IsDBNull(string name)
        {
            return dr.IsDBNull(this.GetOrdinal(name));
        }

        /// <summary>
        /// 检查某一列是否是DBNull值
        /// </summary>
        /// <param name="ordinal">列编号</param>
        /// <returns>返回一个值,指示获取的列是否为NULL</returns>
        public bool IsDBNull(int ordinal)
        {
            return dr.IsDBNull(ordinal);
        }

        /// <summary>
        /// 关闭DataReader
        /// </summary>
        public void Close()
        {
            this.dr.Close();
        }

        /// <summary>
        /// 获取位于指定索引处的列。
        /// </summary>
        /// <param name="index">要获取的列的从零开始的索引</param>
        /// <returns>作为 Object 位于指定索引处的列</returns>
        public object this[int index]
        {
            get
            {
                return dr[index];
            }
        }

        private System.Text.StringBuilder sbdesc = null;
        /// <summary>
        /// 获取结果集的描述信息
        /// </summary>
        /// <returns>结果集的描述信息</returns>
        public string Description
        {
            get
            {
                if (sbdesc == null)
                {
                    sbdesc = new System.Text.StringBuilder();

                    sbdesc.AppendLine("数据库命令描述信息如下");
                    sbdesc.AppendLine("命令    :" + CommandText);
                    sbdesc.AppendLine("类型    :" + CommandType);
                    sbdesc.AppendLine("参数    :");
                    int max = 0;
                    foreach (IDataParameter p in Parameters)
                    {
                        max = p.ParameterName.Length > max ? p.ParameterName.Length : max;
                    }
                    foreach (IDataParameter p in Parameters)
                    {
                        sbdesc.AppendLine(string.Format("{0} : {1} = {2}", p.ParameterName.PadRight(max, ' '), p.Direction.ToString().PadRight(11, ' '), p.Value));
                    }
                    max = 0;
                    if (dr.FieldCount > 0)
                    {
                        sbdesc.AppendLine("字段个数:" + dr.FieldCount.ToString());
                        sbdesc.AppendLine("字段列表:");
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            max = dr.GetName(i).Length > max ? dr.GetName(i).Length : max;
                        }
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            sbdesc.AppendLine(string.Format("{0}:{1}", dr.GetName(i).PadRight(max, ' '), dr.GetDataTypeName(i)));
                        }
                    }
                }
                return sbdesc.ToString();
            }
        }

        /// <summary>
        /// 获取具有指定名称的列。
        /// </summary>
        /// <param name="name">要查找的列的名称。</param>
        /// <returns>名称指定为 Object 的列。</returns>
        public object this[string name]
        {
            get
            {
                return dr[this.GetOrdinal(name)];
            }
        }

        private List<string> _fields;
        /// <summary>
        /// 当前读取器返回的列
        /// </summary>
        public List<string> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new List<string>();
                    for (int i = 0; i < this.FieldCount; i++)
                    {
                        _fields.Add(this.GetName(i));
                    }
                }
                return _fields;
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public new void Dispose()
        {
            if (!this.dr.IsClosed)
            {
                this.dr.Close();
            }
            this.dr.Dispose();
            base.Dispose();
        }

        #region IEnumerable Members

        /// <summary>
        /// 遍历数据读取器
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            //DbEnumerator有一个重载的构造函数,如果使用DbEnumerator(IDataReader reader, bool closeReader)的话,如果后面的参数为true,则游标指定到第一个结果
            //集的最后一个结果时,会自动关闭.
            return new DbEnumerator(this.dr);
        }

        #endregion
    }
    #endregion
}

