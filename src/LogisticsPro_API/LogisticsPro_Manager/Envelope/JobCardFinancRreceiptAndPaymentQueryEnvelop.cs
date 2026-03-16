using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class JobCardFinancRreceiptAndPaymentQueryEnvelop
    {
        public JobCardFinancRreceiptAndPaymentQueryEnvelop(JobCardFinance jobCardFinance)
        {
            this.JobCardFinance = jobCardFinance;
        }

        public JobCardFinance JobCardFinance { get; set; }
    }
}
