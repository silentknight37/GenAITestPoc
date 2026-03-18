using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardTransportationsEventQueryHandler : IRequestHandler<JobCardTransportationsEventQuery, TransportationsQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardTransportationsEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<TransportationsQueryEnvelop> Handle(JobCardTransportationsEventQuery jobCardTransportationsEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobs = await jobDomain.GetJob(jobCardTransportationsEventQuery.JobCardId);
            var jobCardTransportations = await jobDomain.GetJobTransportations(jobCardTransportationsEventQuery.JobCardId);

            return new TransportationsQueryEnvelop(jobCardTransportations, jobs.StatusId);
        }
    }
}
