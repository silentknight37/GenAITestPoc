using LogisticsPro_Data.Models;
using LogisticsPro_Data.Repository;
using LogisticsPro_Manager.Domain;

namespace LogisticsPro_Manager.Factory
{
    public class MasterFactory : IMasterFactory
    {
        private DB_LogisticsproContext dB_LogisticsproContext;
        public MasterFactory(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.dB_LogisticsproContext = dB_LogisticsproContext;
        }

        IMasterDomain IMasterFactory.Create()
        {
            return new MasterDomain(new MasterRepository(dB_LogisticsproContext));
        }
    }
}
