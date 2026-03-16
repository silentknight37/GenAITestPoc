using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class ProformaInvoiceReceiptsByInvoiceIdEventQueryHandler : IRequestHandler<ProformaInvoiceReceiptsByInvoiceIdEventQuery, ProformaInvoiceReceiptsEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public ProformaInvoiceReceiptsByInvoiceIdEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<ProformaInvoiceReceiptsEnvelop> Handle(ProformaInvoiceReceiptsByInvoiceIdEventQuery proformaInvoiceReceiptsByInvoiceIdEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var proformaInvoiceReceipts = await logisticsDomain.GetProformaInvoiceReceipts(proformaInvoiceReceiptsByInvoiceIdEventQuery.InvoiceId);

            return new ProformaInvoiceReceiptsEnvelop(proformaInvoiceReceipts);
        } 
    }
}
