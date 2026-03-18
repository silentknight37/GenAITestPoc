using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class ReceiptsByInvoiceIdEventQueryHandler : IRequestHandler<ReceiptsByInvoiceIdEventQuery, ReceiptsEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public ReceiptsByInvoiceIdEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<ReceiptsEnvelop> Handle(ReceiptsByInvoiceIdEventQuery receiptsByInvoiceIdEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var receipts = await logisticsDomain.GetReceipts(receiptsByInvoiceIdEventQuery.InvoiceId);

            return new ReceiptsEnvelop(receipts);
        } 
    }
}
