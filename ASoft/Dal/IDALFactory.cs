using System;
namespace ASoft.Dal
{
    public interface IDALFactory
    {
        System.Configuration.ConnectionStringSettings DbConnectionStringSettings { get; }
        string DbConnectStringKey { get;}
        E GetDal<E, E1>()
            where E : BaseDal<E1>, new()
            where E1 : ASoft.Model.BaseModel, new();
        string Key { get; } 
    }
}
