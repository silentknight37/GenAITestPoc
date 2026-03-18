using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class ProformaInvoicesEventQueryHandler : IRequestHandler<ProformaInvoiceEventQuery, ProformaInvoiceEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public ProformaInvoicesEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<ProformaInvoiceEnvelop> Handle(ProformaInvoiceEventQuery proformaInvoiceEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var paymentVouchers = await logisticsDomain.GetProformaInvoices(proformaInvoiceEventQuery.IsFirstLoad,proformaInvoiceEventQuery.InvoiceCode, proformaInvoiceEventQuery.CustomerId, proformaInvoiceEventQuery.InvoiceDateFrom, proformaInvoiceEventQuery.InvoiceDateTo, proformaInvoiceEventQuery.InvoiceDueDateFrom, proformaInvoiceEventQuery.InvoiceDueDateTo,proformaInvoiceEventQuery.StatusId,proformaInvoiceEventQuery.JobCardNumber);

            return new ProformaInvoiceEnvelop(paymentVouchers);
        }
    }
}
