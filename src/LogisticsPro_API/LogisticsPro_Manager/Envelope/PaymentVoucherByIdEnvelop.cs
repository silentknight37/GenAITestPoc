using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class PaymentVoucherByIdEnvelop
    {
        public PaymentVoucherByIdEnvelop(PaymentVoucher paymentVoucher)
        {
            this.PaymentVoucher = paymentVoucher;
        }

        public PaymentVoucher PaymentVoucher { get; set; }
    }
}
