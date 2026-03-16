using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LogisticsPro_API
{
    public class JwtService
    {
        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(ConfigurationValues.SECURITY_KEY);
            var key = new SymmetricSecurityKey(keyBytes);

            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                ValidIssuer = ConfigurationValues.API_URL,
                ValidAudience = ConfigurationValues.API_URL,
                IssuerSigningKey = key
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }
    }
}
