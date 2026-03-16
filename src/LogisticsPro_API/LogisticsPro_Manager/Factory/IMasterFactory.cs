using LogisticsPro_Manager.Domain;

namespace LogisticsPro_Manager.Factory
{
    public interface IMasterFactory
    {
        IMasterDomain Create();
    }
}
