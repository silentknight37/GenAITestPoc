using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Query
{
    public class DashboardEventQuery : IRequest<DashboardQueryEnvelop>
    {
        public DashboardEventQuery(int userId) 
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}
