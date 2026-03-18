using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class BatchEventQuery : IRequest<BatchQueryEnvelop>
    {
        public BatchEventQuery(bool isFirstLoad,int userId, string? batchCode, List<int>? vendorId, DateTime? batchDateFrom, DateTime? batchDateTo,string? jobCardNumber) 
        {
            this.IsFirstLoad= isFirstLoad;
            this.UserId = userId;
            this.BatchCode = batchCode;
            this.VendorId = vendorId;
            this.BatchDateFrom = batchDateFrom;
            this.BatchDateTo = batchDateTo;
            this.JobCardNumber = jobCardNumber;
        }

        public bool IsFirstLoad { get; set; }
        public int UserId { get; set; }
        public string BatchCode { get; set; }
        public List<int>? VendorId { get; set; }
        public DateTime? BatchDateFrom { get; set; }
        public DateTime? BatchDateTo { get; set; }
        public string JobCardNumber { get; set; }
    }
}
