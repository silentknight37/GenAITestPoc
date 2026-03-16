using LogisticsPro_Common;
using LogisticsPro_Common.Common;
using LogisticsPro_Common.DTO;
using LogisticsPro_Data;
using LogisticsPro_Data.Models;
using LogisticsPro_Data.Repository;
using LogisticsPro_Manager.Command;
using LogisticsPro_Manager.Envelope;
using LogisticsPro_Manager.Factory;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogisticsPro_Manager.Handler
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, RequestSaveEnvelop>
    {
        private readonly IMasterFactory masterFactory;

        public LoginUserCommandHandler(DB_LogisticsproContext dB_LogisticsproContext)
        {
            this.masterFactory = new MasterFactory(dB_LogisticsproContext);
        }

        public async Task<RequestSaveEnvelop> Handle(LoginUserCommand loginUserCommand,CancellationToken cancellationToken)
        {
            var masterDomain = this.masterFactory.Create();
            try
            {
                if (!await masterDomain.IsUserAlreadyExsitis(loginUserCommand.UserName))
                {
                    var errorMessage = "Request fail due to user not exists";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }
                var user = await masterDomain.GetUserByLoginDetails(loginUserCommand.UserName, loginUserCommand.Password);
                

                if (user == null)
                {
                    var errorMessage = "Request fail due to invalid login details";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }

                if (user.StatusId == (int)EnumStatus.Deactivated)
                {
                    var errorMessage = "Login failed due to inactive user";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }
                var isUpdated = await masterDomain.UpdateLastLogin(user.Id);

                if (!isUpdated)
                {
                    var errorMessage = "Login failed due to invalid process";
                    Error error = new Error(ErrorType.UNAUTHORIZED, errorMessage);
                    return new RequestSaveEnvelop(false, string.Empty, error);
                }

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName),
                    new Claim(ClaimTypes.Sid,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.RoleId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var keyBytes = Encoding.UTF8.GetBytes(ConfigurationValues.SECURITY_KEY);
                var key = new SymmetricSecurityKey(keyBytes);

                var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    ConfigurationValues.API_URL,
                    ConfigurationValues.API_URL,
                    claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddDays(1),
                    sign
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return new RequestSaveEnvelop(true, "Request process successfully", user.Id, user.RoleId.Value, tokenString, user.UserName);


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
