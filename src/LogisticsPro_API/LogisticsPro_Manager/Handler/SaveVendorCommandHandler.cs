using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveVendorCommandHandler : IRequestHandler<SaveVendorCommand, RequestSaveEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public SaveVendorCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveVendorCommand saveVendorCommand,CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            try
            {
                var vendorSaveRequest = new VendorSaveRequest 
                { 
                    Id= saveVendorCommand.Id,
                    VendorName= saveVendorCommand.VendorName,
                    ContactPersonName= saveVendorCommand.ContactPersonName,
                    ContactNumber = saveVendorCommand.ContactNumber,
                    Trn= saveVendorCommand.Trn,
                    Email = saveVendorCommand.Email,
                    Address1 = saveVendorCommand.Address1,
                    Address2 = saveVendorCommand.Address2,
                    BankBranch = saveVendorCommand.BankBranch,
                    BankCode= saveVendorCommand.BankCode,
                    BankName= saveVendorCommand.BankName,
                    City= saveVendorCommand.City,
                    CountryCode= saveVendorCommand.CountryCode,
                    Iban = saveVendorCommand.Iban,
                    SwiftCode= saveVendorCommand.SwiftCode,
                    SelectedVendorTypes = string.Join(',',saveVendorCommand.SelectedVendorTypes),
                    UserId = saveVendorCommand.UserId
                 };

                var response = await masterDomain.SaveVendor(vendorSaveRequest);

                if (!response)
                {
                    var errorMessage = "Request fail due to invalid user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }

                return new RequestSaveEnvelop(response, "Request process successfully", null);

            }
            catch (Exception e)
            {
                var errorMessage = e.Message;
                Error error = new Error(ErrorType.BAD_REQUEST, errorMessage);
                return new RequestSaveEnvelop(false, string.Empty, error);
            }
        }
    }
}
