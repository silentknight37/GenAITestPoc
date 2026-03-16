using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class PaymentVoucherEventQueryHandler : IRequestHandler<PaymentVoucherEventQuery, PaymentVoucherEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public PaymentVoucherEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<PaymentVoucherEnvelop> Handle(PaymentVoucherEventQuery paymentVoucherEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var paymentVouchers = await logisticsDomain.GetPaymentVouchers(paymentVoucherEventQuery.IsFirstLoad,paymentVoucherEventQuery.PaymentVoucherCode, paymentVoucherEventQuery.InvoiceNo, paymentVoucherEventQuery.VendorId, paymentVoucherEventQuery.PaymentVoucherDateFrom, paymentVoucherEventQuery.PaymentVoucherDateTo,paymentVoucherEventQuery.JobCardNumber);

            return new PaymentVoucherEnvelop(paymentVouchers);
        }
    }
}
