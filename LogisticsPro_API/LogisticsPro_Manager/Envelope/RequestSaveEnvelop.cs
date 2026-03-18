using LogisticsPro_Common.Common;

namespace LogisticsPro_Manager.Envelope
{
    public class RequestSaveEnvelop
    {
        public RequestSaveEnvelop(bool created,string successMessage, Error error)
        {
            this.Created = created;
            this.SuccessMessage = successMessage;
            this.Error = error;
        }

        public RequestSaveEnvelop(bool created, string successMessage,long id, Error error)
        {
            this.Created = created;
            this.SuccessMessage = successMessage;
            this.Error = error;
            this.Id = id;
        }

        public RequestSaveEnvelop(bool created, string successMessage, long id, long roleId, string token, string userName)
        {
            this.Created = created;
            this.SuccessMessage = successMessage;
            this.RoleId = roleId;
            this.Token = token;
            this.Id = id;
            UserName = userName;
        }


        public bool Created { get; set; }
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public string SuccessMessage { get; set; }
        public Error Error { get; set; }
    }
}
