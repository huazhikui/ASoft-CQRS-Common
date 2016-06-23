using StructureMap;

namespace ASoft.Services
{
    public interface IMicroserviceRegister
    {
        void Load(IContainer context);
    }
}