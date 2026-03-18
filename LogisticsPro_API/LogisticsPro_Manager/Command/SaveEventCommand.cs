using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveEventCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveEventCommand(int id,string? eventName, DateTime? eventFromDate, DateTime? eventToDate, int? eventTypeId, int? customerId, string? remark,int userId) 
        {
            this.Id = id;
            this.EventName = eventName;
            this.EventFromDate= eventFromDate;
            this.EventToDate= eventToDate;
            this.EventTypeId= eventTypeId;
            this.CustomerId = customerId;
            this.Remark = remark;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
        public string? EventName { get; set; }
        public DateTime? EventFromDate { get; set; }
        public DateTime? EventToDate { get; set; }
        public int? EventTypeId { get; set; }
        public int? CustomerId { get; set; }
        public string? Remark { get; set; }
    }
}
