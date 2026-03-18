using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class JobEventQuery : IRequest<JobQueryEnvelop>
    {
        public JobEventQuery(bool isFirstLoad,int userId, string? jobCardCode, string? jobCardDescription, List<int>? customerId, DateTime? effectiveDateFrom, DateTime? effectiveDateTo, List<int>? statusIds) 
        {
            this.IsFirstLoad = isFirstLoad;
            this.UserId = userId;
            this.JobCardCode = jobCardCode;
            this.JobCardDescription = jobCardDescription;
            this.CustomerId = customerId;
            this.EffectiveDateFrom = effectiveDateFrom;
            this.EffectiveDateTo = effectiveDateTo;
            this.StatusIds = statusIds;
        }
        public bool IsFirstLoad { get; set; }
        public int UserId { get; set; }
        public string JobCardCode { get; set; }
        public string JobCardDescription { get; set; }
        public List<int>? CustomerId { get; set; }
        public DateTime? EffectiveDateFrom { get; set; }
        public DateTime? EffectiveDateTo { get; set; }
        public List<int>? StatusIds { get; set; }
    }
}
