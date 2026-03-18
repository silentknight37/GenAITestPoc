using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class CostTransportationsEventQuery : IRequest<CostTransportationsQueryEnvelop>
    {
        public CostTransportationsEventQuery(int userId, string? jobCardCode, string? bookingRef, string? batchNo, int? customerId, int? vendorId, string? clientRef, DateTime? dateFrom, DateTime? dateTo)
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
        }

        public int UserId { get; set; }
        public string? JobCardCode { get;set; }
        public string? BookingRef { get;set; }
        public string? BatchNo { get; set;}
        public int? CustomerId { get; set; }
        public int? VendorId { get;set; }
        public DateTime? DateFrom { get; set;}
        public DateTime? DateTo { get; set;}
        public string? ClientRef { get; set;}
    }
}
