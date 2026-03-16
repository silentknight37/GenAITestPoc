using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveJobCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveJobCommand(int id,string jobDescription, int customerId, string customerRef, DateTime effectiveDate, string remarks,int userId) 
        {
            this.Id = id;
            this.CustomerId = customerId;
            this.JobDescription = jobDescription;
            this.CustomerRef = customerRef;
            this.EffectiveDate = effectiveDate;
            this.Remarks = remarks;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
        public string JobDescription { get; set; }
        public int CustomerId { get; set; }
        public string CustomerRef { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Remarks { get; set; }
    }
}
