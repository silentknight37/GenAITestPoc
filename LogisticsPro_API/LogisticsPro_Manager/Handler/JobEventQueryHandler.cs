using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobEventQueryHandler : IRequestHandler<JobEventQuery, JobQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobQueryEnvelop> Handle(JobEventQuery jobEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobs = await jobDomain.GetJobs(jobEventQuery.IsFirstLoad,jobEventQuery.JobCardCode, jobEventQuery.JobCardDescription, jobEventQuery.CustomerId, jobEventQuery.EffectiveDateFrom, jobEventQuery.EffectiveDateTo, jobEventQuery.StatusIds);

            return new JobQueryEnvelop(jobs);
        }
    }
}
