using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class VendorSaveRequest
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string? VendorName { get; set; }
        public string? ContactPersonName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Trn { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? CountryCode { get; set; }
        public string? BankCode { get; set; }
        public string? Iban { get; set; }
        public string? SwiftCode { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }
        public string SelectedVendorTypes { get; set; }
    }
}
