using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class UnInvoicedReportEventQueryHandler : IRequestHandler<UnInvoiceReportEventQuery, ReportItemsQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public UnInvoicedReportEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<ReportItemsQueryEnvelop> Handle(UnInvoiceReportEventQuery reportEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            LineGenerateItem fullReportItems= new LineGenerateItem();
            List<ReportItem> dailyReportItems= new List<ReportItem>();
            if (reportEventQuery.ReportTypeId == 2)
            {
                var fullReportItemsList = await jobDomain.GetUnInvoiceReportItems(reportEventQuery.JobCardCode, reportEventQuery.BookingRef, reportEventQuery.BatchNo, reportEventQuery.ClientRef, reportEventQuery.CustomerId, reportEventQuery.VendorId, reportEventQuery.DateFrom, reportEventQuery.DateTo);
                fullReportItems=fullReportItemsList;
            }
            return new ReportItemsQueryEnvelop(fullReportItems, dailyReportItems);
        }
    }
}
