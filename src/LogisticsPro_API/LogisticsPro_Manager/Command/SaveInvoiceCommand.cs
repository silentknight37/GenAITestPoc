using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveInvoiceCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveInvoiceCommand(List<int> transportationIds, List<int> hotelIds, List<int> visaIds, List<int> miscellaneousIds, int customerId, string invoiceDateStn,string invoiceDueDateStn, decimal invoiceAmount,string remarks, string transpotationDescription, string hotelDescription, string visaDescription, string misDescription, List<int> performaInvoiceIds,int userId) 
        {
            this.TransportationIds = transportationIds;
            this.HotelIds = hotelIds;
            this.VisaIds = visaIds;
            this.MiscellaneousIds = miscellaneousIds;
            this.CustomerId = customerId;
            this.InvoiceDateStn = invoiceDateStn;
            this.InvoiceDueDateStn = invoiceDueDateStn;
            this.InvoiceAmount = invoiceAmount;
            this.Remarks = remarks;
            this.TransportDescription = transpotationDescription;
            this.HotelDescription = hotelDescription;
            this.VisaDescription = visaDescription;
            this.MiscellaneousDescription = misDescription;
            this.PerformaInvoiceIds = performaInvoiceIds;
            this.UserId = userId;
        }
        public int UserId { get; set; }
        public List<int> TransportationIds { get; set; }
        public List<int> HotelIds { get; set; }
        public List<int> VisaIds { get; set; }
        public List<int> MiscellaneousIds { get; set; }
        public int CustomerId { get; set; }
        public string InvoiceDateStn { get; set; }
        public string InvoiceDueDateStn { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string Remarks { get; set; }
        public string TransportDescription { get; set; }
        public string HotelDescription { get; set; }
        public string VisaDescription { get; set; }
        public string MiscellaneousDescription { get; set; }
        public List<int> PerformaInvoiceIds { get; set; }

        public DateTime? InvoiceDate
        {
            get
            {
                return InvoiceDateStn != null ? DateTimeOffset.Parse(InvoiceDateStn).LocalDateTime : null;
            }
        }

        public DateTime? InvoiceDueDate
        {
            get
            {
                return InvoiceDueDateStn != null ? DateTimeOffset.Parse(InvoiceDueDateStn).LocalDateTime : null;
            }
        }
    }
}
