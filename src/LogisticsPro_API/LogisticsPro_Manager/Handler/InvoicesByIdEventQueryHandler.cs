using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class InvoicesByIdEventQueryHandler : IRequestHandler<InvoiceByIdEventQuery, InvoiceByIdEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public InvoicesByIdEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<InvoiceByIdEnvelop> Handle(InvoiceByIdEventQuery invoiceByIdEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var invoice = await logisticsDomain.GetInvoice(invoiceByIdEventQuery.Id);

            return new InvoiceByIdEnvelop(invoice);
        }
    }
}
