using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class CustomerSaveRequest
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ContactPersonName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string? Trn { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public int? CountryCode { get; set; }
    }
}
