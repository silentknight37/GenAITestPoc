using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class RemovePaymentVoucherItemCommand : IRequest<RequestRemoveEnvelop>
    {
        public RemovePaymentVoucherItemCommand(int id,string itemType,int userId) 
        {
            this.Id = id;
            this.ItemType = itemType;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public string ItemType { get; set; }
        public int Id { get; set; }
    }
}
