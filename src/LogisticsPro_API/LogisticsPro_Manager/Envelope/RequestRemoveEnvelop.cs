using LogisticsPro_Common.Common;

namespace LogisticsPro_Manager.Envelope
{
    public class RequestRemoveEnvelop
    {
        public RequestRemoveEnvelop(bool removed,string message, Error error)
        {
            this.Removed = removed;
            this.Message = message;
            this.Error = error;
        }

        public RequestRemoveEnvelop(bool removed, string message, long id, Error error)
        {
            this.Removed = removed;
            this.Message = message;
            this.Error = error;
            this.Id = id;
        }

        public bool Removed { get; set; }
        public long Id { get; set; }
        public string Message { get; set; }
        public Error Error { get; set; }
    }
}
