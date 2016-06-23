using System.Collections.Generic;
using ASoft.Model;

namespace ASoft.Db
{
    /// <summary>
    /// 用于配置和记录数据库操作日志
    /// </summary>
    public interface IAccessDatabaseOperation
    {
        /// <summary>
        /// 获取所有需记录日志的表配置
        /// </summary>
        /// <returns></returns>
        List<Table> GetAllTableConfig();

        /// <summary>
        /// 获取需记录表字段操作日志的字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<TableField> GetFieldConfigByTable(string tableName);

        /// <summary>
        /// 获取表的配置
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        Table GetByTable(string tableName);

        /// <summary>
        ///设置记录日志的表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool RequireTable(string tableName);

        /// <summary>
        /// （从系统表中）查询表的主键字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<string> GetPrimaryKeyFromDBMS(string tableName);

        /// <summary>
        /// 压入新的数据库操作
        /// </summary>
        /// <param name="operationList"></param>
        /// <returns></returns>
        bool PushDbOperation(List<TableOperation> operationList);

   
    }
}