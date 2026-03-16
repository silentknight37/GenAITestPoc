using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class VendorEventQuery : IRequest<VendorQueryEnvelop>
    {
        public VendorEventQuery(int userId) 
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}
