using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data.Models;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;

namespace LogisticsPro_Manager.Handler
{
    public class SaveUserCommandHandler : IRequestHandler<SaveUserCommand, RequestSaveEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public SaveUserCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(SaveUserCommand saveUserCommand,CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            try
            {
                var userSaveRequest = new UserSaveRequest 
                { 
                    Id= saveUserCommand.Id,
                    UserName=saveUserCommand.UserName,
                    Password=saveUserCommand.Password,
                    RoleId=saveUserCommand.RoleId,
                    StatusId=saveUserCommand.StatusId,
                    UserId=saveUserCommand.UserId
                 };

                var response = await masterDomain.SaveUser(userSaveRequest);

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
