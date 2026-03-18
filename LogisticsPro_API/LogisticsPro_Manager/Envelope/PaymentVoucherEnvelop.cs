using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class PaymentVoucherEnvelop
    {
        public PaymentVoucherEnvelop(List<PaymentVoucher> paymentVouchers)
        {
            this.PaymentVouchers = paymentVouchers;
        }

        public List<PaymentVoucher> PaymentVouchers { get; set; }
    }
}
