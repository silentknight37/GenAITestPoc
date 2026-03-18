using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class HistoryReportEventQueryHandler : IRequestHandler<HistoryReportEventQuery, HistoryReportItemsQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public HistoryReportEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<HistoryReportItemsQueryEnvelop> Handle(HistoryReportEventQuery reportEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();

            var historyGenerateItem = await jobDomain.GetHistoryReportItems(reportEventQuery.JobCardCode, reportEventQuery.RUserId, reportEventQuery.DateFrom, reportEventQuery.DateTo);
            
            return new HistoryReportItemsQueryEnvelop(historyGenerateItem);
        }
    }
}
