using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardHotelsEventQueryHandler : IRequestHandler<JobCardHotelsEventQuery, JobCardHotelsQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardHotelsEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardHotelsQueryEnvelop> Handle(JobCardHotelsEventQuery jobCardHotelsEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobs = await jobDomain.GetJob(jobCardHotelsEventQuery.JobCardId);
            var jobCardHotel = await jobDomain.GetJobHotels(jobCardHotelsEventQuery.JobCardId);

            return new JobCardHotelsQueryEnvelop(jobCardHotel, jobs.StatusId);
        }
    }
}
