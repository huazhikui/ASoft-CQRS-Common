using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;

namespace ASoft.Db
{
    /// <summary>
    /// 数据库访问类
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        /// 获取或设置连接字符串
        /// </summary>
        string ConnectionString
        {
            get;
        }

        void SetConnectString(String myConnectString);

        /// <summary>
        /// 数据库命令参数前缀(如SqlServer数据库为@)
        /// </summary>
        string ParameterPrefix {
            get;
        }

        ICacheable CacheAccess { set; get; }

        #region 通用方法
        /// <summary>
        /// 返回数据库类型
        /// </summary>
        ASoft.Db.DbProvider Provider
        {
            get;
        }

        /// <summary>
        /// 获取配置集合
        /// </summary>
        Dictionary<string, string> ConfigSet
        {
            get;
        }

        SeqGen SequenceGenerator
        {
            get;
        }

        /// <summary>
        /// 创建保存序列的表的SQL脚本(表名:ASoft_Id_Gen)
        /// </summary>
        string SequenceTableSql
        {
            get;
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void SaveConfig(string key, string value);

        #endregion

        #region 返回影响的行数

        /// <summary>
        /// 执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回受影响的行数</returns>
        NonQueryResult ExecuteNonQuery(string commandText);

        /// <summary>
        ///  执行数据库命令,返回受影响的行数
        /// </summary>
        /// <param name="command">参数化数据库命令</param>
        /// <returns>返回受影响的行数</returns>
        NonQueryResult ExecuteNonQuery(DataCommand command);

        #endregion 执行数据库命令,返回受影响的行数

        #region 执行数据库命令,返回结果的第一行第一列

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回结果的第一行第一列</returns>
        ScalerResult ExecuteScalar(string commandText);

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="command">参数化数据库命令</param>
        /// <returns>返回结果的第一行第一列</returns>
        ScalerResult ExecuteScalar(DataCommand command);

        /// <summary>
        /// 执行数据库命令,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库命令文本(参数化)</param>
        /// <param name="commandParameters">数据命令参数</param>
        /// <returns>返回结果的第一行第一列</returns>
        ScalerResult ExecuteScalar(string commandText, params IDbDataParameter[] commandParameters);

        #endregion 执行数据库命令,返回结果的第一行第一列

        #region 执行数据库命令,返回一个DataReader

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据读取器</returns>
        DataReader ExecuteReader(string commandText);

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <param name="commandParameters">数据库命令的参数</param>
        /// <returns>返回一个数据读取器</returns>
        DataReader ExecuteReader(string commandText, params IDbDataParameter[] commandParameters);

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="dataCommand"></param>
        /// <returns></returns>
        DataReader ExecuteReader(DataCommand dataCommand);

        /// <summary>
        /// 执行数据库命令,返回一个数据读取器
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        DataReader ExecuteReader(IDbTransaction transaction, DataCommand command);
        /// <summary>
        /// 执行SQL语句,返回一个数据读取器
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>返回结果的第一行第一列</returns>
        DataReader ExecuteReader(PageSearch search);

        /// <summary>
        /// 为SQL查询条件生成分页查询SQL语句
        /// </summary>
        /// <param name="commandText">查询语句</param> 
        /// <param name="start">返回的查询结果要跳过的行数</param>
        /// <param name="limit">返回记录集的长度</param>
        /// <param name="total">out 查询记录集的总记录数</param>
        /// <returns>返回分页数据读取器</returns>
        DataReader ExecuteReader(string commandText, int strat, int limit, out int total);
        #endregion  执行数据库命令,返回一个DataReader

        #region 执行数据库命令,返回一个数据表

        /// <summary>
        /// 执行数据库命令,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据表</returns>
        DataTableResult ExecuteDataTable(string commandText);


        /// <summary>
        /// 执行SQL语句,返回一个数据表
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>返回一个数据表</returns>
        DataTableResult ExecuteDataTable(PageSearch search);
        #endregion 执行数据库命令,返回一个数据表

        #region 执行数据库命令,返回一个数据集

        /// <summary>
        /// 执行数据库命令,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库命令文本</param>
        /// <returns>返回一个数据集</returns>
        DataSetResult ExecuteDataSet(string commandText);

        #endregion 执行数据库命令,返回一个数据集

        #region 执行事务
        /// <summary>
        /// 执行一系列的事务(返回最后一条语句影响的行数)
        /// </summary>
        /// <param name="commands">数据库命令</param>
        int ExecuteTransaction(IList<string> commands);


        /// <summary>
        /// 执行一系列的事务(返回最后一条语句影响的行数)
        /// </summary>
        /// <param name="commands">参数化的数据库命令</param>
        /// <returns></returns>
        int ExecuteTransaction(IList<DataCommand> commands);

        /// <summary>
        /// 创建一个连接对象
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();
        #endregion

        #region 格式化数据

        /// <summary>
        /// 把一个对象化成为一个可用的SQL数据
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns>格式化后的字符串</returns>
        string ToSqlValue(object value);

        /// <summary>
        /// 把一个整型列表创建一个IN表达式
        /// </summary>
        /// <param name="colume">列名</param>
        /// <param name="length">IN表达式的最大长度(如Oracle最大长度为8000)</param>
        /// <param name="value">IN表达式的值列表</param>
        /// <returns>创建后的IN表达式</returns>
        string BuildInSql(string colume, int length, IList<int> value);

        /// <summary>
        /// 把一个整型列表创建一个IN表达式
        /// </summary>
        /// <param name="colume">列名</param>
        /// <param name="length">IN表达式的最大长度(如Oracle最大长度为8000)</param>
        /// <param name="value">IN表达式的值列表</param>
        /// <returns>创建后的IN表达式</returns>
        string BuildInSql(string colume, int length, IList<long> value);


        /// <summary>
        /// 把一个字符串列表创建一个IN表达式
        /// </summary>
        /// <param name="colume">列名</param>
        /// <param name="length">IN表达式的最大长度(如Oracle最大长度为8000)</param>
        /// <param name="value">IN表达式的值列表</param>
        /// <returns>创建后的IN表达式</returns>
        string BuildInSql(string colume, int length, IList<string> value);

        #endregion

        #region 执行参数化SQL
        /// <summary>
        /// 执行参数化SQL
        /// </summary>
        /// <param name="commandText">sql</param>
        /// <param name="commandParameters">sql参数</param>
        /// <returns></returns>
        NonQueryResult ExecuteSqlNonQuery(String commandText, params object[] commandParameters);
        #endregion

        #region 执行事务,最后返回一个ScalerResult对象

        /// <summary>
        /// 执行一系列的事务,最后一个SQL命令返回Scaler对象
        /// </summary>
        /// <param name="commands">数据库命令</param>
        ScalerResult ExecuteScalerTransaction(IList<string> commands);

        #endregion

        #region 获取所有的表
        /// <summary>
        /// 获取所有的表
        /// </summary>
        /// <returns></returns>
        List<string> Tables
        {
            get;
        }
        #endregion

        #region 获取所有存储过程

        /// <summary>
        /// 
        /// </summary>
        List<string> Procedures
        {
            get;
        }
        #endregion

        #region 实体插入更新删除
        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        void InsertObject(ASoft.Model.BaseModel m);

        /// <summary>
        /// 更新一个实体
        /// </summary>
        /// <param name="m"></param>
        void UpdateObject(ASoft.Model.BaseModel m);

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="m"></param>
        void DeleteObject(ASoft.Model.BaseModel m);

        /// <summary>
        /// 从数据库中查询一个对象(需要设置对象的主键)
        /// </summary>
        /// <param name="m">对象</param>
        void LoadObject<TSource>(TSource m) where TSource : ASoft.Model.BaseModel, new();

        /// <summary>
        /// 从数据读取器中设置一个对象的属性(如果设置了缓存器，则会存入缓存)
        /// </summary>
        /// <param name="m"></param>
        /// <param name="dr"></param>
        TSource SetObjectProperty<TSource>(ASoft.Db.DataReader dr)
            where TSource : ASoft.Model.BaseModel, new();

        /// <summary>
        /// 从数据读取器中设置一个对象的属性
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="model"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        TSource SetObjectProperty<TSource>(TSource model, ASoft.Db.DataReader dr)
              where TSource : ASoft.Model.BaseModel, new();
        #endregion

        /// <summary>
        /// 获取Insert语句
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        String GetSqlByInsert(ASoft.Model.BaseModel m);

        /// <summary>
        /// 获取Insert的Command
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        DataCommand GetCommandByInsert(ASoft.Model.BaseModel m);

        /// <summary>
        /// 获取update语句
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        String GetSqlByUpdate(ASoft.Model.BaseModel m);

        /// <summary>
        ///  获取update语句
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        DataCommand GetCommandByUpdate(ASoft.Model.BaseModel m);

        /// <summary>
        /// 获取delete语句
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        String GetSqlByDelete(ASoft.Model.BaseModel m);

        /// <summary>
        /// 获取delete语句
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        DataCommand GetCommandByDelete(ASoft.Model.BaseModel m);

        #region
        /// <summary>
        /// 执行数据库存储过程,返回一个数据读取器
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据读取器</returns>
        DataReader ExecuteProcReader(string commandText, params object[] ps);


        /// <summary>
        /// 执行数据库存储过程,返回结果的第一行第一列
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回结果的第一行第一列</returns>
        ScalerResult ExecuteProcScalar(string commandText, params object[] ps);



        /// <summary>
        /// 执行数据库存储过程,返回一个数据表
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据表</returns>
        DataTableResult ExecuteProcDataTable(string commandText, params object[] ps);



        /// <summary>
        /// 执行数据库存储过程,返回一个数据集
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回一个数据集</returns>
        DataSetResult ExecuteProcDataSet(string commandText, params object[] ps);

        /// <summary>
        /// 执行数据库存储过程,返回受影响的行数
        /// </summary>
        /// <param name="commandText">数据库存储过程命令</param>
        /// <param name="ps">参数的值列表</param>
        /// <returns>返回受影响的行数</returns>
        NonQueryResult ExecuteProcNonQuery(string commandText, params object[] ps);
        #endregion

        #region
        IDbDataParameter MakeIn(string name, object value);


        DataCommand CreateSQLCommand(String sql, params object[] args);
        #endregion
    }
}
