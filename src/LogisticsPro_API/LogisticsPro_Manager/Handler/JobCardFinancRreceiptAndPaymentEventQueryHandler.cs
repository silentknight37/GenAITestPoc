using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using LogisticsPro_Manager.Query;
using MediatR;

namespace YoutubeShareManager.Handler
{
    public class JobCardFinancRreceiptAndPaymentEventQueryHandler : IRequestHandler<JobCardFinancRreceiptAndPaymentEventQuery, JobCardFinancRreceiptAndPaymentQueryEnvelop>
    {
        private readonly IJobFactory jobFactory;

        public JobCardFinancRreceiptAndPaymentEventQueryHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.jobFactory = new JobFactory(dB_LogisticsproContext);
        }

        public async Task<JobCardFinancRreceiptAndPaymentQueryEnvelop> Handle(JobCardFinancRreceiptAndPaymentEventQuery jobCardFinancRreceiptAndPaymentEventQuery, CancellationToken cancellationToken)
        {
            var jobDomain = this.jobFactory.Create();
            var jobCardFinance = await jobDomain.GetJobFinancRreceiptAndPayments(jobCardFinancRreceiptAndPaymentEventQuery.JobCardId);

            return new JobCardFinancRreceiptAndPaymentQueryEnvelop(jobCardFinance);
        }
    }
}
