using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardTransportationEventQueryHandler : IRequestHandler<JobCardTransportationEventQuery, JobCardTransportationQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardTransportationEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardTransportationQueryEnvelop> Handle(JobCardTransportationEventQuery jobCardTransportationEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobCardTransportations = await jobDomain.GetJobTransportation(jobCardTransportationEventQuery.Id);

            return new JobCardTransportationQueryEnvelop(jobCardTransportations);
        }
    }
}
