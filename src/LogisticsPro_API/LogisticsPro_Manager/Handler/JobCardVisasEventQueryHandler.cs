using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardVisasEventQueryHandler : IRequestHandler<JobCardVisasEventQuery, JobCardVisasQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardVisasEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardVisasQueryEnvelop> Handle(JobCardVisasEventQuery jobCardVisasEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobs = await jobDomain.GetJob(jobCardVisasEventQuery.JobCardId);
            var jobCardVisas = await jobDomain.GetJobVisas(jobCardVisasEventQuery.JobCardId);

            return new JobCardVisasQueryEnvelop(jobCardVisas, jobs.StatusId);
        }
    }
}
