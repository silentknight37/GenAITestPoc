using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class BatchItemByVendorIdEventQueryHandler : IRequestHandler<PaymentVoucherGenerateItemQuery, PaymentVoucherGenerateItemEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public BatchItemByVendorIdEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<PaymentVoucherGenerateItemEnvelop> Handle(PaymentVoucherGenerateItemQuery batchItemByVendorIdEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var jobCardTransportations = await logisticsDomain.GetBatchJobCardTransportationByVendorId(batchItemByVendorIdEventQuery.VendorId, batchItemByVendorIdEventQuery.FromDate, batchItemByVendorIdEventQuery.ToDate, batchItemByVendorIdEventQuery.JobCardNumber);

            return new PaymentVoucherGenerateItemEnvelop(jobCardTransportations);
        }
    }
}
