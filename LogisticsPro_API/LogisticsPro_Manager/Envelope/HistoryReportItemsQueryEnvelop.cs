using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class HistoryReportItemsQueryEnvelop
    {
        public HistoryReportItemsQueryEnvelop(HistoryGenerateItem historyGenerateItem)
        {
            this.HistoryGenerateItem = historyGenerateItem;
        }

        public HistoryGenerateItem HistoryGenerateItem { get; set; }
    }
}
