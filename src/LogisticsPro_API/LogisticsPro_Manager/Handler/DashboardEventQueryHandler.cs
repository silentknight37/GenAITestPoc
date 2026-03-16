using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class DashboardEventQueryHandler : IRequestHandler<DashboardEventQuery, DashboardQueryEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public DashboardEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<DashboardQueryEnvelop> Handle(DashboardEventQuery dashboardEventQuery, CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            var dashboard = await masterDomain.GetDashboard(dashboardEventQuery.UserId);

            return new DashboardQueryEnvelop(dashboard);
        }
    }
}
