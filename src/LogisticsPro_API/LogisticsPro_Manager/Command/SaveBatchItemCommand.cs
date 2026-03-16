using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveBatchItemCommand : IRequest<BatchRequestSaveEnvelop>
    {
        public SaveBatchItemCommand(List<int> ids,int vendorId,string batchDateStn,int userId) 
        {
            this.Ids = ids;
            this.VendorId = vendorId;
            this.BatchDateStn = batchDateStn;
            this.UserId = userId;
        }

        public int UserId { get; set; }
        public List<int> Ids { get; set; }
        public int VendorId { get; set; }
        public string? BatchDateStn { get; set; }
        public DateTime? BatchDate
        {
            get
            {
                return BatchDateStn != null ? DateTimeOffset.Parse(BatchDateStn).LocalDateTime : null;
            }
        }
    }
}
