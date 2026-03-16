using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardMiscellaneousEventQueryHandler : IRequestHandler<JobCardMiscellaneousEventQuery, JobCardMiscellaneousQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardMiscellaneousEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardMiscellaneousQueryEnvelop> Handle(JobCardMiscellaneousEventQuery jobCardMiscellaneousEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobCardMiscellaneous = await jobDomain.GetJobMiscellaneous(jobCardMiscellaneousEventQuery.Id, jobCardMiscellaneousEventQuery.UserId);

            return new JobCardMiscellaneousQueryEnvelop(jobCardMiscellaneous);
        }
    }
}
