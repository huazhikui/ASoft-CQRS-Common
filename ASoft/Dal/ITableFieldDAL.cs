using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ASoft.Model;
using ASoft.Db;

namespace ASoft.Dal
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITableFieldDAL
    {
        /// <summary>
        /// （从系统表中）查询表的主键字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<String> GetPrimaryKeyFromDBMS(String tableName);

        /// <summary>
        /// 查询表的字段（从系统表中）
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<TableField> GetByTableFromDBMS(String tableName);

        /// <summary>
        /// 查询表的(SYS_DATA_OPERATION)已配置字段
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        List<TableField> GetByTable(String tableName);
    }
}
