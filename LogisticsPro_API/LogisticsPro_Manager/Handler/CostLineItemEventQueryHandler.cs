using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class CostLineItemEventQueryHandler : IRequestHandler<CostJobLineItemsEventQuery, CostLineItemsQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public CostLineItemEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<CostLineItemsQueryEnvelop> Handle(CostJobLineItemsEventQuery costJobLineItemsEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            switch (costJobLineItemsEventQuery.ReferenceTypeId)
            {
                case 1:
                    {
                        var jobCardTransportations = await jobDomain.GetCostTransportation(costJobLineItemsEventQuery.JobCardCode, costJobLineItemsEventQuery.BookingRef, costJobLineItemsEventQuery.BatchNo, costJobLineItemsEventQuery.ClientRef, costJobLineItemsEventQuery.CustomerId, costJobLineItemsEventQuery.VendorId, costJobLineItemsEventQuery.DateFrom, costJobLineItemsEventQuery.DateTo);

                        return new CostLineItemsQueryEnvelop(1,jobCardTransportations,new List<LogisticsPro_Common.DTO.JobCardHotel>(),new List<LogisticsPro_Common.DTO.JobCardVisa>(),new List<LogisticsPro_Common.DTO.JobCardMiscellaneous>());
                    }
                case 2:
                    {
                        var jobCardHotels = await jobDomain.GetCostHotel(costJobLineItemsEventQuery.JobCardCode, costJobLineItemsEventQuery.BookingRef, costJobLineItemsEventQuery.BatchNo, costJobLineItemsEventQuery.ClientRef, costJobLineItemsEventQuery.CustomerId, costJobLineItemsEventQuery.VendorId, costJobLineItemsEventQuery.DateFrom, costJobLineItemsEventQuery.DateTo);

                        return new CostLineItemsQueryEnvelop(2, new List<LogisticsPro_Common.DTO.JobCardTransportation>(), jobCardHotels, new List<LogisticsPro_Common.DTO.JobCardVisa>(), new List<LogisticsPro_Common.DTO.JobCardMiscellaneous>());
                    }
                case 3:
                    {
                        var jobCardVisas = await jobDomain.GetCostVisa(costJobLineItemsEventQuery.JobCardCode, costJobLineItemsEventQuery.BookingRef, costJobLineItemsEventQuery.BatchNo, costJobLineItemsEventQuery.ClientRef, costJobLineItemsEventQuery.CustomerId, costJobLineItemsEventQuery.VendorId, costJobLineItemsEventQuery.DateFrom, costJobLineItemsEventQuery.DateTo);

                        return new CostLineItemsQueryEnvelop(3, new List<LogisticsPro_Common.DTO.JobCardTransportation>(), new List<LogisticsPro_Common.DTO.JobCardHotel>(), jobCardVisas, new List<LogisticsPro_Common.DTO.JobCardMiscellaneous>());
                    }
                case 4:
                    {
                        var jobCardMiscellaneous = await jobDomain.GetCostMiscellaneous(costJobLineItemsEventQuery.JobCardCode, costJobLineItemsEventQuery.BookingRef, costJobLineItemsEventQuery.BatchNo, costJobLineItemsEventQuery.ClientRef, costJobLineItemsEventQuery.CustomerId, costJobLineItemsEventQuery.VendorId, costJobLineItemsEventQuery.DateFrom, costJobLineItemsEventQuery.DateTo);

                        return new CostLineItemsQueryEnvelop(4, new List<LogisticsPro_Common.DTO.JobCardTransportation>(), new List<LogisticsPro_Common.DTO.JobCardHotel>(), new List<LogisticsPro_Common.DTO.JobCardVisa>(), jobCardMiscellaneous);
                    }
                    default:
                    {
                        return new CostLineItemsQueryEnvelop(0, new List<LogisticsPro_Common.DTO.JobCardTransportation>(), new List<LogisticsPro_Common.DTO.JobCardHotel>(), new List<LogisticsPro_Common.DTO.JobCardVisa>(), new List<LogisticsPro_Common.DTO.JobCardMiscellaneous>());
                    }
            }
            
        }
    }
}
