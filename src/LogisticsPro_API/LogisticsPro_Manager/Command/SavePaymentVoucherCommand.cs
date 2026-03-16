using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SavePaymentVoucherCommand : IRequest<RequestSaveEnvelop>
    {
        public SavePaymentVoucherCommand(List<int> transportationIds, List<int> hotelIds, List<int> visaIds, List<int> miscellaneousIds, int vendorId,string paymentVoucherDateStn,decimal paymentVoucherAmount,string invoice,string remarks,int userId) 
        {
            this.TransportationIds = transportationIds;
            this.HotelIds = hotelIds;
            this.VisaIds = visaIds;
            this.MiscellaneousIds = miscellaneousIds;
            this.VendorId = vendorId;
            this.PaymentVoucherDateStn = paymentVoucherDateStn;
            this.PaymentVoucherAmount = paymentVoucherAmount;
            this.Invoice = invoice;
            this.Remarks = remarks;
            this.UserId = userId;
        }
        public int UserId { get; set; }
        public List<int> TransportationIds { get; set; }
        public List<int> HotelIds { get; set; }
        public List<int> VisaIds { get; set; }
        public List<int> MiscellaneousIds { get; set; }
        public int VendorId { get; set; }
        public string PaymentVoucherDateStn { get; set; }
        public decimal PaymentVoucherAmount { get; set; }
        public string Invoice { get; set; }
        public string Remarks { get; set; }
        
        public DateTime? PaymentVoucherDate
        {
            get
            {
                return PaymentVoucherDateStn != null ? DateTimeOffset.Parse(PaymentVoucherDateStn).LocalDateTime : null;
            }
        }
    }
}
