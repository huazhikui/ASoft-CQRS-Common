using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Model;
using ASoft.Db;

namespace ASoft.Dal
{
    public interface ITableDAL
    {
        /// <summary>
        /// 根据名称（精确）查询已配置到Sys_Data_Table中的表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Table  GetByName(String name);

        /// <summary>
        /// 根据名称（模糊）查询已配置到Sys_Data_Table中的表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        List<Table> QueryByName(String name);

        /// <summary>
        /// 获取所有已配置到Sys_Data_Table中的表
        /// </summary> 
        /// <returns></returns>
        List<Table> GetAll();


        /// <summary>
        /// 根据名称查询数据库中的表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Table GetByNameFromDBMS(String name);

        /// <summary>
        /// 获取数据库中所有表
        /// </summary> 
        /// <returns></returns>
        List<Table> GetAllFromDBMS();

        /// <summary>
        /// 添加新的配置
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        bool AddToConfig(Table table);

 
    }
}
