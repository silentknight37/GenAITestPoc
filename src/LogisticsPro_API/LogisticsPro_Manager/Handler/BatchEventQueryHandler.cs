using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class BatchEventQueryHandler : IRequestHandler<BatchEventQuery, BatchQueryEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public BatchEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<BatchQueryEnvelop> Handle(BatchEventQuery batchEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var batches = await logisticsDomain.GetBatches(batchEventQuery.IsFirstLoad,batchEventQuery.BatchCode, batchEventQuery.VendorId, batchEventQuery.BatchDateFrom, batchEventQuery.BatchDateTo, batchEventQuery.JobCardNumber);

            return new BatchQueryEnvelop(batches);
        }
    }
}
