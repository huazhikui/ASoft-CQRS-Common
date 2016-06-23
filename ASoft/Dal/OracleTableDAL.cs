using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft.Model;
using ASoft.Db;

namespace ASoft.Dal
{
    public class OracleTableDAL : BaseDal<Table>, ITableDAL
    {
        public OracleTableDAL(IDataAccess db, IDataAccess baseDb) : base(db, baseDb)
        {

        }
        public List<Table> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Table> GetAllFromDBMS()
        {
            throw new NotImplementedException();
        }

        public List<Table> QueryByName(string name)
        {
            throw new NotImplementedException();
        }

        public Table GetByNameFromDBMS(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool AddToConfig(Table table)
        {
            List<DataCommand> sqls = new List<DataCommand>();
            sqls.Add(this.db.GetCommandByInsert(table));
            if (table.FieldSet != null)
            {
                foreach (var field in table.FieldSet)
                {
                    sqls.Add(this.db.GetCommandByInsert(field));
                }
            }
            return this.db.ExecuteTransaction(sqls) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Table GetByName(string name)
        {
            try
            {
                return this.Where("TABLE_NAME = {0}", name).First();
            }
            catch(Exception ex) {
                return null;
            }
        } 
    }
}
