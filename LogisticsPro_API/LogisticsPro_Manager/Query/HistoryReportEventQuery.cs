using LogisticsPro_Common.DTO;
using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class HistoryReportEventQuery : IRequest<HistoryReportItemsQueryEnvelop>
    {
        public HistoryReportEventQuery(int userId, List<int>? rUserId, string? jobCardCode, DateTime? dateFrom, DateTime? dateTo)
        {
            this.UserId = userId;
            this.RUserId = rUserId;
            this.JobCardCode = jobCardCode;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
        }

        public int UserId { get; set; }
        public List<int>? RUserId { get; set; }
        public string? JobCardCode { get;set; }
        public DateTime? DateFrom { get; set;}
        public DateTime? DateTo { get; set;}
    }
}
