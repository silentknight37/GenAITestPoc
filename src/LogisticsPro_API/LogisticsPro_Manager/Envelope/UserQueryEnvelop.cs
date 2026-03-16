using LogisticsPro_Common.DTO;

namespace LogisticsPro_Manager.Envelope
{
    public class UserQueryEnvelop
    {
        public UserQueryEnvelop(List<User> users)
        {
            this.Users = users;
        }

        public List<User> Users { get; set; }
    }
}
