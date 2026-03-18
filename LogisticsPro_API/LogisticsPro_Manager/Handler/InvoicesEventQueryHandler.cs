using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class InvoicesEventQueryHandler : IRequestHandler<InvoiceEventQuery, InvoiceEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public InvoicesEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<InvoiceEnvelop> Handle(InvoiceEventQuery invoiceEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var invoices = await logisticsDomain.GetInvoices(invoiceEventQuery.IsFirstLoad,invoiceEventQuery.InvoiceCode, invoiceEventQuery.CustomerId, invoiceEventQuery.InvoiceDateFrom, invoiceEventQuery.InvoiceDateTo, invoiceEventQuery.InvoiceDueDateFrom, invoiceEventQuery.InvoiceDueDateTo, invoiceEventQuery.StatusId, invoiceEventQuery.JobCardNumber);
            foreach (var invoice in invoices)
            {
                var proformaInvoiceReceipts = await logisticsDomain.GetLinkedProformaInvoiceReceipts(invoice.Id);
                invoice.LinkedProformaInvoices.AddRange(proformaInvoiceReceipts);
            }
           
            return new InvoiceEnvelop(invoices);
        }
    }
}
