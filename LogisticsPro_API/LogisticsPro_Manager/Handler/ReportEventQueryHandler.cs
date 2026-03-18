using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class ReportEventQueryHandler : IRequestHandler<ReportEventQuery, ReportItemsQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public ReportEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<ReportItemsQueryEnvelop> Handle(ReportEventQuery reportEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            LineGenerateItem fullReportItems= new LineGenerateItem();
            List<ReportItem> dailyReportItems= new List<ReportItem>();
            if (reportEventQuery.ReportTypeId == 2)
            {
                var fullReportItemsList = await jobDomain.GetFullReportItems(reportEventQuery.JobCardCode, reportEventQuery.BookingRef, reportEventQuery.BatchNo, reportEventQuery.ClientRef, reportEventQuery.CustomerId, reportEventQuery.VendorId, reportEventQuery.DateFrom, reportEventQuery.DateTo);
                fullReportItems=fullReportItemsList;
            }
            else
            {
                var dailyReportItemsList = await jobDomain.GetDailyReportItems(reportEventQuery.BookingRef, reportEventQuery.BatchNo, reportEventQuery.ClientRef, reportEventQuery.CustomerId, reportEventQuery.VendorId, reportEventQuery.DateFrom);
                dailyReportItems.AddRange(dailyReportItemsList);
            }
            return new ReportItemsQueryEnvelop(fullReportItems, dailyReportItems);
        }
    }
}
