using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardHotelEventQueryHandler : IRequestHandler<JobCardHotelEventQuery, JobCardHotelQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardHotelEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardHotelQueryEnvelop> Handle(JobCardHotelEventQuery jobCardHotelEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobCardHotels = await jobDomain.GetJobHotel(jobCardHotelEventQuery.Id);

            return new JobCardHotelQueryEnvelop(jobCardHotels);
        }
    }
}
