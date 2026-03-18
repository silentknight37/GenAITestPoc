using LogisticsPro_Common.DTO;
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class ReportEventQuery : IRequest<ReportItemsQueryEnvelop>
    {
        public ReportEventQuery(int userId, string? jobCardCode, string? bookingRef, string? batchNo, List<int>? customerId, List<int>? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo,int? reportTypeId)
        {
            this.UserId = userId;
            this.JobCardCode = jobCardCode;
            this.BookingRef = bookingRef;
            this.BatchNo = batchNo;
            this.ClientRef = clientRef;
            this.CustomerId = customerId;
            this.VendorId=vendorId;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.ReportTypeId = reportTypeId;
        }

        public int UserId { get; set; }
        public string? JobCardCode { get;set; }
        public string? BookingRef { get;set; }
        public string? BatchNo { get; set;}
        public List<int>? CustomerId { get; set; }
        public List<int>? VendorId { get;set; }
        public DateTime? DateFrom { get; set;}
        public DateTime? DateTo { get; set;}
        public string? ClientRef { get; set;}
        public int? ReportTypeId { get; set; }
    }
}
