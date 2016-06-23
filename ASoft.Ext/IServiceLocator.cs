using System;

namespace ASoft
{
    public interface IServiceLocator
    {
        T GetService<T>();
        object GetService(Type type);
    }
}