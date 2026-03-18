using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class CloseJobCardCommand : IRequest<RequestSaveEnvelop>
    {
        public CloseJobCardCommand(int id,int userId) 
        {
            this.Id = id;
            UserId = userId;
        }

        public int Id { get; set; }
        public int UserId { get; }
    }
}
