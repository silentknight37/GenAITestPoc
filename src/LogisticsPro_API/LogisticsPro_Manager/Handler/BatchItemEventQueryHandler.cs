using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class BatchItemEventQueryHandler : IRequestHandler<BatchItemEventQuery, BatchItemQueryEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public BatchItemEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<BatchItemQueryEnvelop> Handle(BatchItemEventQuery batchItemEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var jobCardTransportations = await logisticsDomain.GetBatchJobCardTransportation(batchItemEventQuery.BatchDate);

            return new BatchItemQueryEnvelop(jobCardTransportations);
        }
    }
}
