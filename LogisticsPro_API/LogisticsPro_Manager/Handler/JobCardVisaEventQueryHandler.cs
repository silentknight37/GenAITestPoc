using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardVisaEventQueryHandler : IRequestHandler<JobCardVisaEventQuery, JobCardVisaQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardVisaEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardVisaQueryEnvelop> Handle(JobCardVisaEventQuery jobCardVisaEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobCardVisa = await jobDomain.GetJobVisa(jobCardVisaEventQuery.Id);

            return new JobCardVisaQueryEnvelop(jobCardVisa);
        }
    }
}
