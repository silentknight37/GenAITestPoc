using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class CostTransportationEventQueryHandler : IRequestHandler<CostTransportationsEventQuery, CostTransportationsQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public CostTransportationEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<CostTransportationsQueryEnvelop> Handle(CostTransportationsEventQuery costTransportationsEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobCardTransportations = await jobDomain.GetCostTransportation(costTransportationsEventQuery.JobCardCode, costTransportationsEventQuery.BookingRef, costTransportationsEventQuery.BatchNo, costTransportationsEventQuery.ClientRef, costTransportationsEventQuery.CustomerId, costTransportationsEventQuery.VendorId, costTransportationsEventQuery.DateFrom, costTransportationsEventQuery.DateTo);

            return new CostTransportationsQueryEnvelop(jobCardTransportations);
        }
    }
}
