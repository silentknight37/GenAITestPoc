using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Vendor
    {
        public int Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string? VendorType { get; set; }
        public string VendorTypeIds { get; set; }
        public string ContactPersonName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Trn { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string BankCode { get; set; }
        public string Iban { get; set; }
        public string SwiftCode { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public int? TotalBookingCount { get; set; }

        public List<int> SelectedVendorTypes
        {
            get
            {
                if (VendorTypeIds == null)
                {
                   return new List<int>();
                }
                var vendorIds = VendorTypeIds.Split(',');

                return vendorIds.Any() ? vendorIds.Where(i=>i!="").Select(i => int.Parse(i)).ToList():new List<int>();
            }
        }
    }
}
