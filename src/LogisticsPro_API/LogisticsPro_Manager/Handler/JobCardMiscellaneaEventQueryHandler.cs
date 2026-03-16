using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardMiscellaneaEventQueryHandler : IRequestHandler<JobCardMiscellaneaEventQuery, JobCardMiscellaneaQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardMiscellaneaEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardMiscellaneaQueryEnvelop> Handle(JobCardMiscellaneaEventQuery jobCardMiscellaneousEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobs = await jobDomain.GetJob(jobCardMiscellaneousEventQuery.JobCardId);
            var jobCardMiscellaneous = await jobDomain.GetJobMiscellanea(jobCardMiscellaneousEventQuery.JobCardId, jobCardMiscellaneousEventQuery.UserId);

            return new JobCardMiscellaneaQueryEnvelop(jobCardMiscellaneous, jobs.StatusId);
        }
    }
}
