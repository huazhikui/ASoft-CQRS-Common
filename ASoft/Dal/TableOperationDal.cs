using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using ASoft.Db;
namespace ASoft.Dal
{
    public class TableOperationDal : BaseDal<TableOperation>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="baseDb"></param>
        public TableOperationDal(IDataAccess db, IDataAccess baseDb) : base(db, baseDb)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public  bool Insert(List<TableOperation> list)
        {
            List<DataCommand> commands = new List<DataCommand>();
            foreach (var entity in list)
            {
                entity.ID = Sequence.GetInstance(this.db, "dblog").NextLuhmValue;
                commands.Add(db.GetCommandByInsert(entity));
            }
            db.ExecuteTransaction(commands);
            return true;
        }
    }
}
