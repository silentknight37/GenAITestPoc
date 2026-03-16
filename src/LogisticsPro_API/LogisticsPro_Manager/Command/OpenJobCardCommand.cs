using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class OpenJobCardCommand : IRequest<RequestSaveEnvelop>
    {
        public OpenJobCardCommand(int id,int userId) 
        {
            this.Id = id;
            UserId = userId;
        }

        public int Id { get; set; }
        public int UserId { get; }
    }
}
