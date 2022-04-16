using JWT;
using JWT.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HairdresserScheduleApp.BusinessLogic.Services
{
    public class JwtService : IJwtService
    {
        private readonly IJwtDecoder decoder;
        private readonly Configurations.JwtSettings jwtSettings;

        public JwtService(IOptions<Configurations.JwtSettings> jwtOptions, IJwtDecoder decoder)
        {
            this.jwtSettings = jwtOptions.Value;
            this.decoder = decoder;
        }

        public (DateTime, string, string) GetAccountDetails(string accessToken)
        {
            try
            {
                var token = decoder.DecodeToObject<JwtToken>(accessToken);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(token.exp);
                return (dateTimeOffset.LocalDateTime, token.nameid, token.role);
            }
            catch (TokenExpiredException)
            {
                return (DateTime.MinValue, default, default);
            }
            catch (SignatureVerificationException)
            {
                return (DateTime.MinValue, default, default);
            }
            catch (Exception ex)
            {
                // ... remember to handle the generic exception ...
                return (DateTime.MinValue, default, default);
            }
        }

        public string GenerateJwt(Models.User user, bool isAdmin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var role = isAdmin ? Configurations.Constants.ROLE_ADMIN : Configurations.Constants.ROLE_USER;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role,role),
                    new Claim(ClaimTypes.NameIdentifier,user.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(jwtSettings.TokenValidityInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public interface IJwtService
    {
        string GenerateJwt(Models.User user, bool isAdmin);

        (DateTime, string, string) GetAccountDetails(string token);
    }

    public class JwtToken
    {
        public long exp { get; set; }
        public string nameid { get; set; }
        public string role { get; set; }
    }
}