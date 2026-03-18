using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class JobCardFinance
    {
        public JobCardFinance()
        {
            JobCardFinanceRreceipt = new List<JobCardFinanceRreceipt>();
            JobCardFinancePaymentVouchers = new List<JobCardFinancePaymentVoucher>();
            JobCardFinanceInvoices = new List<JobCardFinanceInvoice>();
            JobCardFinanceProformaInvoices = new List<JobCardFinanceInvoice>();
            JobCardFinanceProformaInvoicesReceipt = new List<JobCardFinanceRreceipt>();
        }
        public List<JobCardFinanceRreceipt> JobCardFinanceRreceipt { get; set; }
        public List<JobCardFinanceRreceipt> JobCardFinanceProformaInvoicesReceipt { get; set; }
        public List<JobCardFinancePaymentVoucher> JobCardFinancePaymentVouchers { get; set; }
        public List<JobCardFinanceInvoice> JobCardFinanceInvoices { get; set; }
        public List<JobCardFinanceInvoice> JobCardFinanceProformaInvoices { get; set; }
    }
}
