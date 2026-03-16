using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCarditemsByCustomerIdEventQueryHandler : IRequestHandler<JobCardItemByCustomerIdEventQuery, JobCardItemQueryEnvelop>
    {
        private readonly ILogisticsFactory logisticsFactory;

        public JobCarditemsByCustomerIdEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.logisticsFactory = new LogisticsFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardItemQueryEnvelop> Handle(JobCardItemByCustomerIdEventQuery batchItemByVendorIdEventQuery, CancellationToken cancellationToken)
        {
            var logisticsDomain = this.logisticsFactory.Create();
            var jobCardLineItems = await logisticsDomain.GetJobCardItemsByCustomerId(batchItemByVendorIdEventQuery.CustomerId, batchItemByVendorIdEventQuery.FromDate, batchItemByVendorIdEventQuery.ToDate, batchItemByVendorIdEventQuery.JobCardNumber);

            var jobCardIds=new List<int?>();
            jobCardIds.AddRange(jobCardLineItems.Transportations.Select(i=>i.JobCardId).Distinct().ToList());
            jobCardIds.AddRange(jobCardLineItems.Hotels.Select(i=>i.JobCardId).Distinct().ToList());
            jobCardIds.AddRange(jobCardLineItems.Visa.Select(i=>i.JobCardId).Distinct().ToList());
            jobCardIds.AddRange(jobCardLineItems.Miscellaneous.Select(i=>i.JobCardId).Distinct().ToList());

            var proformaInvoiceReceipts = await logisticsDomain.GetProformaInvoiceReceiptsByJobCardIds(jobCardIds.Distinct().ToList());

            return new JobCardItemQueryEnvelop(jobCardLineItems, proformaInvoiceReceipts);
        }
    }
}
