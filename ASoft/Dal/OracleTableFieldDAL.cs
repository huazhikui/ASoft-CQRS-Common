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
    public class OracleTableFieldDAL : BaseDal<TableField>, ITableFieldDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="baseDb"></param>
        public OracleTableFieldDAL(IDataAccess db, IDataAccess baseDb):base(db, baseDb) {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<TableField> GetByTable(string tableName)
        {
            String sql = " exists (select id from sys_data_table where sys_data_table.id=SYS_DATA_FIELD.Id and sys_data_table.table_name={0})";
            return this.Where(sql, tableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<TableField> GetByTableFromDBMS(string tableName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<String> GetPrimaryKeyFromDBMS(string tableName)
        {
            tableName = tableName.ToUpper();
            List<String> result = new List<string>();
            DataCommand cmd = db.CreateSQLCommand("select a.constraint_name,  a.column_name  from user_cons_columns a, user_constraints b  where a.constraint_name = b.constraint_name  and b.constraint_type = 'P'  and a.table_name = {0} ", tableName);
            using (DataReader dr = db.ExecuteReader(cmd))
            {
                while (dr.Read())
                {
                    result.Add(dr.GetString("COLUMN_NAME"));
                }
                dr.Close();
            }
            return result;
        }

        
    }
}
