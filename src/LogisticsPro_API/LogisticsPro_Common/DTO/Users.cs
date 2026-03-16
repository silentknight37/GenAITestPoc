using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public string StatusName { get; set; }
        public int? StatusId { get; set; }
        public int? RoleId { get; set; }
    }
}
