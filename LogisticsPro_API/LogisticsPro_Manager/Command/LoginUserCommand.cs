using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class LoginUserCommand : IRequest<RequestSaveEnvelop>
    {
        public LoginUserCommand(string userName,string password) 
        {
            this.UserName = userName;
            this.Password = password;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
