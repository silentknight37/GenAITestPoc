using LogisticsPro_Manager.Domain;

namespace LogisticsPro_Manager.Factory
{
    public interface ILogisticsFactory
    {
        ILogisticsDomain Create();
    }
}
