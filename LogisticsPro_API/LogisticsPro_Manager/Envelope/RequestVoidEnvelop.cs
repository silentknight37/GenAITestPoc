using LogisticsPro_Common.Common;

namespace LogisticsPro_Manager.Envelope
{
    public class RequestVoidEnvelop
    {
        public RequestVoidEnvelop(bool voided,string message, Error error)
        {
            this.Voided = voided;
            this.Message = message;
            this.Error = error;
        }

        public RequestVoidEnvelop(bool voided, string message, long id, Error error)
        {
            this.Voided = voided;
            this.Message = message;
            this.Error = error;
            this.Id = id;
        }

        public bool Voided { get; set; }
        public long Id { get; set; }
        public string Message { get; set; }
        public Error Error { get; set; }
    }
}
