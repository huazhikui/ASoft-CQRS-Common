using System;
namespace ASoft.Dal
{
    interface IBaseDal<E>
     where E : ASoft.Model.BaseModel, new()
    {
        ASoft.Db.IDataAccess baseDb { get; set; }
        ASoft.Db.IDataAccess db { get; set; }
        void Delete(E e);
        bool Delete(string id);
        bool Exists(string id);
        E Get(string id);
        System.Collections.Generic.List<E> GetAll();
        System.Collections.Generic.List<E> GetAll(int page, int rows, out int total);
        E Insert(E entity);
        string KeyField { get; }
        string Table { get; set; }
        bool Update(E entity);
        System.Collections.Generic.List<E> Where(string where);
        System.Collections.Generic.List<E> Where(string where, int page, int rows, out int total);
    }
}
