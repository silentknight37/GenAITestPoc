using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class RemoveHotelCommand : IRequest<RequestRemoveEnvelop>
    {
        public RemoveHotelCommand(int id,int userId) 
        {
            this.Id = id;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
