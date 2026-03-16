using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_API.Request
{
    public class LoginUserRequest
    {
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
