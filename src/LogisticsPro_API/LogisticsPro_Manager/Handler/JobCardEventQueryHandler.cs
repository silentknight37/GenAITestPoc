using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardEventQueryHandler : IRequestHandler<JobCardEventQuery, JobCardQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardQueryEnvelop> Handle(JobCardEventQuery jobCardEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobs = await jobDomain.GetJob(jobCardEventQuery.JobCardId);

            return new JobCardQueryEnvelop(jobs);
        }
    }
}
