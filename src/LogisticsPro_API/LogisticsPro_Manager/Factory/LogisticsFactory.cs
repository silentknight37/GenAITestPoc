using LogisticsPro_Data.Models;
using LogisticsPro_Data.Repository;
using LogisticsPro_Manager.Domain;

namespace LogisticsPro_Manager.Factory
{
    public class LogisticsFactory : ILogisticsFactory
    {
        private DB_LogisticsproContext dB_LogisticsproContext;
        public LogisticsFactory(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.dB_LogisticsproContext = dB_LogisticsproContext;
        }

        ILogisticsDomain ILogisticsFactory.Create()
        {
            return new LogisticsDomain(new LogisticsRepository(dB_LogisticsproContext));
        }
    }
}
