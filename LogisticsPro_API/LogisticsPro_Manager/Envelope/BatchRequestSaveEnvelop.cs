using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class BatchRequestSaveEnvelop
    {
        public BatchRequestSaveEnvelop(bool created,string successMessage, Error error)
        {
            this.Created = created;
            this.SuccessMessage = successMessage;
            this.Error = error;
        }

        public BatchRequestSaveEnvelop(bool created, string successMessage, BatchLineItemResponse batchDetail, Error error)
        {
            this.Created = created;
            this.SuccessMessage = successMessage;
            this.Error = error;
            this.BatchDetail = batchDetail;
        }

        public bool Created { get; set; }
        public long Id { get; set; }
        public string SuccessMessage { get; set; }
        public BatchLineItemResponse BatchDetail { get; set; }
        public Error Error { get; set; }
    }
}
