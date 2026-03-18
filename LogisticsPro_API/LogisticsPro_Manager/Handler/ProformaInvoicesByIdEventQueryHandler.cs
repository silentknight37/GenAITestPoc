using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class ProformaInvoicesByIdEventQueryHandler : IRequestHandler<ProformaInvoiceByIdEventQuery, ProformaInvoiceByIdEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public ProformaInvoicesByIdEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<ProformaInvoiceByIdEnvelop> Handle(ProformaInvoiceByIdEventQuery proformaInvoiceByIdEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var paymentVoucher = await logisticsDomain.GetProformaInvoice(proformaInvoiceByIdEventQuery.Id);

            return new ProformaInvoiceByIdEnvelop(paymentVoucher);
        }
    }
}
