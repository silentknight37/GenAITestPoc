using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveCustomerCommandHandler:IRequestHandler<SaveCustomerCommand, RequestSaveEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public SaveCustomerCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveCustomerCommand saveCustomerCommand,CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            try
            {
                var customerSaveRequest = new CustomerSaveRequest 
                { 
                    Id= saveCustomerCommand.Id,
                    CustomerName= saveCustomerCommand.CustomerName,
                    ContactPersonName= saveCustomerCommand.ContactPersonName,
                    ContactNumber = saveCustomerCommand.ContactNumber,
                    Trn=saveCustomerCommand.Trn,
                    Address1=saveCustomerCommand.Address1,
                    Address2=saveCustomerCommand.Address2,
                    City=saveCustomerCommand.City,
                    CountryCode=saveCustomerCommand.CountryCode,
                    Email = saveCustomerCommand.Email,
                    UserId=saveCustomerCommand.UserId
                 };

                var response = await masterDomain.SaveCustomer(customerSaveRequest);

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
