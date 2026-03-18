using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveUserCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveUserCommand(int id,string userName,string password,int roleId,int statusId,int userId) 
        {
            this.Id = id;
            this.UserName = userName;
            this.Password = password;
            this.RoleId = roleId;
            this.StatusId = statusId;
            this.UserId = userId;
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
    }
}
