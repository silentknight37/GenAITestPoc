using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class ProformaInvoiceByIdEventQuery : IRequest<ProformaInvoiceByIdEnvelop>
    {
        public ProformaInvoiceByIdEventQuery(int userId, int id) 
        {
            this.UserId = userId;
            this.Id = id;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
