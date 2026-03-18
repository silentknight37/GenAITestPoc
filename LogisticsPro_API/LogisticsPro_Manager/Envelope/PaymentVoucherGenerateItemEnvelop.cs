using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class PaymentVoucherGenerateItemEnvelop
    {
        public PaymentVoucherGenerateItemEnvelop(LineGenerateItem paymentVoucherGenerateItem)
        {
            this.PaymentVoucherGenerateItem = paymentVoucherGenerateItem;
        }

        public LineGenerateItem PaymentVoucherGenerateItem { get; set; }
    }
}
