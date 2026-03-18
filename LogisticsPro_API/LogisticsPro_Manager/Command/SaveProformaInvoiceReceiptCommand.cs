using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveProformaInvoiceReceiptCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveProformaInvoiceReceiptCommand(int id, string receiptDateStn, decimal amount,int paymentMethod,int proformaInvoiceId, int jobCardId, string remark, int userId) 
        {
            this.Id = id;
            this.PaymentMethod = paymentMethod;
            this.ReceiptDateStn = receiptDateStn;
            this.Amount = amount;
            this.ProformaInvoiceId= proformaInvoiceId;
            this.JobCardId = jobCardId;
            this.Remark = remark;
            this.UserId = userId;
        }
        public int UserId { get; set; }
        public int Id { get; set; }
        public string ReceiptDateStn { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public int ProformaInvoiceId { get; set; }
        public int JobCardId { get; set; }
        public string Remark { get; set; }

        public DateTime? ReceiptDate
        {
            get
            {
                return ReceiptDateStn != null ? DateTimeOffset.Parse(ReceiptDateStn).LocalDateTime : null;
            }
        }
    }
}
