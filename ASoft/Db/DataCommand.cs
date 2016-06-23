using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using ASoft.Text;

namespace ASoft.Db
{
    public class DataCommand : DataCommand<IDbDataParameter>
    {
        public DataCommand(string commandText, CommandType commandType, IDbDataParameter[] parameters) :
            base(commandText, commandType, parameters)
        {

        }


    }

    /// <summary>
    /// 
    /// </summary>
    public class DataCommandBuilder
    {

        private String _commandText;
        /// <summary>
        /// 
        /// </summary>
        protected IDataAccess db;
         
        /// <summary>
        /// 
        /// </summary>
        protected List<String> commandTextFilterList = null;

        /// <summary>
        /// 
        /// </summary> 
        protected Dictionary<String, object> paramDict = null;

        /// <summary>
        /// 
        /// </summary>
        public object[] Arguments
        {
            get {
                if (paramDict != null && paramDict.Keys.Count > 0)
                {
                   return paramDict.Values.ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CommandText
        {
            get
            {
                if (this.commandTextFilterList != null)
                {
                    if (_commandText!=null && _commandText.ToUpper().IndexOf("WHERE")==-1)
                    {
                        _commandText += " where ";
                    }
                    return _commandText += String.Join(" ",this.commandTextFilterList);
                }
                return _commandText;
            }
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="db"></param>
        /// <param name="args"></param>
        public DataCommandBuilder(String commandText, IDataAccess db, params object[] args)
        {
            this.db = db;
            sbsql = new StringBuilder();
            commandTextFilterList = new List<string>();
            paramDict = new Dictionary<string, object>();
            createCommand(commandText, args); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filesName"></param>
        /// <param name="db"></param>
        /// <param name="args"></param>
        public DataCommandBuilder(String tableName, String filesName, IDataAccess db, params object[] args)
        {
            this.db = db;
            sbsql = new StringBuilder();
            this.tableName = tableName;
            this.FieldsName = filesName;
            commandTextFilterList = new List<string>();
            paramDict = new Dictionary<string, object>();
            this._commandText = String.Format("select {0} from {1} ", this.fieldsName, this.tableName); 
        }

        /// <summary>
        /// 
        /// </summary>
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
                
                this.fieldsName = ( String.IsNullOrEmpty(value) ? "*" :value).Trim();
            }
        }
        #endregion

        private StringBuilder sbsql = null;
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
        /// 查询条件,多表查询时,请在字段前加上表别名
        /// </summary>
        public string GroupBy
        {
            set;
            get;
        }

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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public void And(String sql, params object[] args)
        {
           
            if (sbsql.Length > 0 && !sql.Trim().ToUpper().StartsWith("AND"))
            {
                sbsql.Append(" AND ");
            }
            sbsql.Append(sql);
            AppendCommand(sql, args);
        }

        /// <summary>
        /// 增加command的where条件
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        public void AppendCommand(String sql, params object[] args)
        {

            var commandFilter = createCommand(sql, args);
            if (sbsql.Length > 0 && !commandFilter.CommandText.Trim().ToUpper().StartsWith("AND"))
            {
                sbsql.Append(" AND ");
            }
            sbsql.Append(commandFilter.CommandText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected DataCommandFilter createCommand(String sql, params object[] args)
        {
            var commandFilter = createDataParam(sql, args);
            if (commandFilter.Params != null)
            {
                foreach (var item in commandFilter.Params)
                {
                    paramDict.Add(item.Key, item.Value);
                }
            }
            this.commandTextFilterList.Add(commandFilter.CommandText);
            return commandFilter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataCommand CreateCommand()
        {
            String sql = String.Format("");
            IDbDataParameter[] parameters =  CreateParameters(); 
            return new DataCommand(this.CommandText, CommandType.Text, parameters);
        }

        public DataCommand CreateCountCommand(int pageIndex, int pageSize)  
        {
            PageSearch search = new PageSearch();
            search.PageIndex = pageIndex;
            //rows==0时 取15条
            search.PageSize = pageSize > 0 ? pageSize : 15;
            //获取分页数据时将跳过的记录数 
            search.TableName = this.TableName;
            search.FieldsName = this.FieldsName;
            search.GroupBy = this.GroupBy;
            search.And(this.Where);
            String sql = DbTools.CreateCountSql(search);

            IDbDataParameter[] parameters = CreateParameters();  
            return new DataCommand(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public DataCommand CreatePageCommand(int pageIndex, int pageSize, int total)
        {
            PageSearch search = new PageSearch();
            search.PageIndex = pageIndex;
            //rows==0时 取15条
            search.PageSize = pageSize > 0 ? pageSize : 15;
            //获取分页数据时将跳过的记录数
            int skip = 0;
            search.TableName = this.TableName;
            search.FieldsName = this.FieldsName;
            search.And(this.Where); 
            search.OrderField = orderField;
            search.TotalCount = total;
            String sql = DbTools.CreatePageSql(search, this.db.Provider, null, out skip);
            IDbDataParameter[] parameters = CreateParameters();
            return new DataCommand(sql, CommandType.Text, parameters); 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter[] CreateParameters() {
            IDbDataParameter[] parameters = null;
            if (this.paramDict != null && this.paramDict.Keys.Count > 0)
            {
                int length = this.paramDict.Keys.Count;
                parameters = new IDbDataParameter[length];
                int i = 0;
                foreach (var item in this.paramDict)
                {
                    parameters[i] = this.db.MakeIn(item.Key, item.Value);
                    i++;
                } 
            }
            return parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        protected DataCommandFilter createDataParam(String sql, params object[] args)
        { 
            String sqlHashCode = sql.GetHashCode().ToString().Replace("-", "A"); 
            int argsLength = args.Length;
            System.Text.RegularExpressions.Regex regex = new Regex(@"{\d+}", RegexOptions.IgnoreCase); 
            Dictionary<String, object> myParams = null;
            var match = regex.Matches(sql);
            if (match.Count>0)
            {
                 
                if (args != null && argsLength > 0)
                {
                    //this.db.Provider
                    List<String> argNames = new List<string>();
                    myParams = new Dictionary<String, object>(); 
                    object[] matchArgs = null; 
                   
                    if (match.Count == 1 && argsLength > 1)
                    {
                        matchArgs = args;
                        argsLength = 1;  
                    } 
                    //遍历参数
                    for (int i = 0; i < argsLength; i++)
                    {   
                        //如果是列表或者是数组
                        if (args[i] != null 
                            && ((args[i].GetType().IsArray || args[i] is IEnumerable<object>) || (matchArgs!=null)))
                        {
                            int j = 0;
                            String paramName = "";
                            List<String> paramNames = new List<string>();
                            Array list = null;
                            if(matchArgs!=null && argsLength==1)
                            {
                                list = matchArgs;
                            }
                            else { 
                                try
                                {
                                    list = (args[i] as IEnumerable<object>).ToArray();
                                }
                                catch {
                                    list = args[i] as Array;
                                }
                            }
                            foreach (var argValue in list)
                            { 
                                String argName = String.Format("{0}parm_{1}{2}{3}", this.db.ParameterPrefix, sqlHashCode, i, j);
                                paramNames.Add(argName);
                                myParams.Add(argName, argValue);
                                j++;
                            }
                            paramName = StringUtils.Join(paramNames);
                            argNames.Add(paramName);
                        }
                        else {
                            //生成SQL参数名称
                            String argName = String.Format("{0}parm_{1}{2}", this.db.ParameterPrefix, sqlHashCode, i);
                            argNames.Add(argName);
                            myParams.Add(argName, args[i]);
                        }
                    }
                    System.Text.RegularExpressions.Regex myRegex
                        = new System.Text.RegularExpressions.Regex(@"((?<='{\d})')|('(?={\d+}'))", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    sql = myRegex.Replace(sql, "");
                    if (argNames != null && argNames.Count > 0)
                    {
                        sql = String.Format(sql, argNames.ToArray());
                    }
                    else {
                        sql = String.Format(sql, argNames);
                    }
                }
            }
            else
            {

            }
            sql = sql.Replace("%'", "||'%' ");
            sql = sql.Replace(" '%:parm", " '%'||:parm");
            return new DataCommandFilter()
            {
                CommandText = sql,
                Params = myParams
            };
        }

         
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataCommandFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public String CommandText { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, object> Params { set; get; }
    }

    /// <summary>
    /// 一个执行数据库命令的Command接口
    /// </summary>
    /// <typeparam name="P">数据命令参数类型</typeparam>
    public class DataCommand<P> : IDataCommand
        where P : IDbDataParameter
    {
        public DataCommand()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandType">数据库命令类型</param>
        /// <param name="parameters">数据库命令参数</param>
        public DataCommand(string commandText, CommandType commandType, P[] parameters)
        {
            this.commandText = commandText;
            this.commandType = commandType;
            if (parameters != null)
            {
                this.iparameters = new IDbDataParameter[parameters.Length];
                for (int i = 0; i < this.iparameters.Length; this.iparameters[i] = parameters[i], i++) ;
            }
        }

        /// <summary>
        /// 命令参数
        /// </summary>
        public P[] Parameters
        {
            get
            {
                if (IDbParameters != null)
                {
                    P[] parameters = new P[IDbParameters.Length];
                    for (int i = 0; i < parameters.Length; parameters[i] = (P)IDbParameters[i], i++) ;
                    return parameters;
                }
                return null;
            }
        }

        /// <summary>
        /// 返回指定名称的参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>参数</returns>
        public P this[string name]
        {
            get
            {
                foreach (IDbDataParameter p in IDbParameters)
                {
                    if (p.ParameterName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return (P)p;
                    }
                }
                return default(P);
            }
        }

        private string commandText = string.Empty;
        /// <summary>
        /// 数据库命令
        /// </summary>
        public string CommandText
        {
            get
            {
                return this.commandText;
            }
            set
            {
                this.commandText = value;
            }
        }

        private CommandType commandType = CommandType.Text;
        /// <summary>
        /// 数据库命令的类型
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return commandType;
            }
            set
            {
                this.commandType = value;
            }
        }

        private IDbDataParameter[] iparameters;
        public IDbDataParameter[] IDbParameters
        {
            get
            {
                return iparameters;
            }
        }
    }
}
