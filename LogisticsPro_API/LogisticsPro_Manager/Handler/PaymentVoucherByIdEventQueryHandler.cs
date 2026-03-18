using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class PaymentVoucherByIdEventQueryHandler : IRequestHandler<PaymentVoucherByIdEventQuery, PaymentVoucherByIdEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public PaymentVoucherByIdEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<PaymentVoucherByIdEnvelop> Handle(PaymentVoucherByIdEventQuery paymentVoucherByIdEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var paymentVouchers = await logisticsDomain.GetPaymentVoucherById(paymentVoucherByIdEventQuery.Id);

            return new PaymentVoucherByIdEnvelop(paymentVouchers);
        }
    }
}
