using LogisticsPro_Data.Models;
using LogisticsPro_Data.Repository;
using LogisticsPro_Manager.Domain;

namespace LogisticsPro_Manager.Factory
{
    public class JobFactory : IJobFactory
    {
        private DB_LogisticsproContext dB_LogisticsproContext;
        public JobFactory(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.dB_LogisticsproContext = dB_LogisticsproContext;
        }

        IJobDomain IJobFactory.Create()
        {
            return new JobDomain(new JobRepository(dB_LogisticsproContext));
        }
    }
}
