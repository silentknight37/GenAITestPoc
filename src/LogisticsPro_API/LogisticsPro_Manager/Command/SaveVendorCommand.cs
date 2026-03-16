using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveVendorCommand : IRequest<RequestSaveEnvelop>
    {
        public SaveVendorCommand(int id,string? vendorName,string? contactPersonName,string? email,string? contactNumber, string? trn, string? address1, string? address2, string? city, string? countryCode, string? bankCode, string? iban, string? swiftCode, string? bankName, string? bankBranch,List<int> selectedVendorTypes, int userId) 
        {
            this.Id = id;
            this.VendorName = vendorName;
            this.ContactPersonName= contactPersonName;
            this.Email= email;
            this.ContactNumber= contactNumber;
            this.Trn = trn;
            this.Address1 = address1;
            this.Address2 = address2;
            this.City = city;
            this.CountryCode = countryCode;
            this.BankCode = bankCode;
            this.Iban = iban;
            this.SwiftCode = swiftCode;
            this.BankName = bankName;
            this.BankBranch = bankBranch;
            this.SelectedVendorTypes = selectedVendorTypes;
            this.UserId = userId;
        }

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
        public List<int> SelectedVendorTypes { get; set; }
    }
}
