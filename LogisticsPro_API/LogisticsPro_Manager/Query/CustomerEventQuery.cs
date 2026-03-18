using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class CustomerEventQuery : IRequest<CustomerQueryEnvelop>
    {
        public CustomerEventQuery(int userId) 
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}
