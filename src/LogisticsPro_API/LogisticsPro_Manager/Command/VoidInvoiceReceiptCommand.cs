using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class VoidInvoiceReceiptCommand : IRequest<RequestVoidEnvelop>
    {
        public VoidInvoiceReceiptCommand(int id,int userId) 
        {
            this.Id = id;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
