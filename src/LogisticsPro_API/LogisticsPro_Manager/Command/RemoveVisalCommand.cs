using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class RemoveVisalCommand : IRequest<RequestRemoveEnvelop>
    {
        public RemoveVisalCommand(int id,int userId) 
        {
            this.Id = id;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
