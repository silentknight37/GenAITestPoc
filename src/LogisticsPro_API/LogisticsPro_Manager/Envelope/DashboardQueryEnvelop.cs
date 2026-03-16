using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class DashboardQueryEnvelop
    {
        public DashboardQueryEnvelop(Dashboard dashboard)
        {
            this.Dashboard = dashboard;
        }

        public Dashboard Dashboard { get; set; }
    }
}
