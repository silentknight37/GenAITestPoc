using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveReceiptCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveReceiptCommand(int id, string receiptDateStn, decimal amount,int paymentMethod,int invoiceId, string remark,List<UpdateRecords>updateRecords,  int userId) 
        {
            this.Id = id;
            this.PaymentMethod = paymentMethod;
            this.ReceiptDateStn = receiptDateStn;
            this.Amount = amount;
            this.InvoiceId= invoiceId;
            this.Remark = remark;
            this.UpdateRecords = updateRecords;
            this.UserId = userId;
        }
        public int UserId { get; set; }
        public int Id { get; set; }
        public string ReceiptDateStn { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public int InvoiceId { get; set; }
        public string Remark { get; set; }
        public List<UpdateRecords> UpdateRecords { get; set; }

        public DateTime? ReceiptDate
        {
            get
            {
                return ReceiptDateStn != null ? DateTimeOffset.Parse(ReceiptDateStn).LocalDateTime : null;
            }
        }
    }

    public class UpdateRecords
    {
        public int Id { get; set; }
        public string ServiceType { get; set; }
        public decimal AllocatedAmount { get; set; }

    }
}
