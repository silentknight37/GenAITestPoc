using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class ReportItemsQueryEnvelop
    {
        public ReportItemsQueryEnvelop(LineGenerateItem fullReport, List<ReportItem> dailyReport)
        {
            this.FullReport = fullReport;
            this.DailyReport = dailyReport;
        }

        public LineGenerateItem FullReport { get; set; }
        public List<ReportItem> DailyReport { get; set; }
    }
}
