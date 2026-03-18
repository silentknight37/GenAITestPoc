using LogisticsPro_Manager.Envelope;
using MediatR;

namespace LogisticsPro_Manager.Command
{
    public class SaveCustomerCommand:IRequest<RequestSaveEnvelop>
    {
        public SaveCustomerCommand(int id,string customerName,string contactPersonName,string email,string contactNumber, string? trn, string? address1, string? address2, string? city, int? countryCode,int userId) 
        {
            this.Id = id;
            this.CustomerName= customerName;
            this.ContactPersonName= contactPersonName;
            this.Email= email;
            this.ContactNumber= contactNumber;
            this.Trn = trn;
            this.Address1 = address1;
            this.Address2 = address2;
            this.City = city;
            this.CountryCode = countryCode;
            this.UserId = userId;
        }

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
