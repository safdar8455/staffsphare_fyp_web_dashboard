using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace Webclient.Controllers.Api
{
    public static class CustomClaimTypes
    {
        public const string CompanyId = "CompanyId";
        public const string UserId = "UserId";
    }

    public static class IdentityExtensions
    {
        public static int GetCompanyId(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.CompanyId);

            if (claim == null)
                return 0;

            return int.Parse(claim.Value);
        }

        public static string GetUserId(this IIdentity identity)
        {

            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.UserId);

            if (claim == null)
                return string.Empty;

            return claim.Value;
        }

    }

    public class TokenManager
    {
        private const string Secret = "localhostQuuxIsTheStandardTypeOfStringWeUse12345";

     
        public static string GenerateToken(string username,string uId,int companyId, int expireMonths = 1)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(CustomClaimTypes.CompanyId, companyId.ToString(), ClaimValueTypes.Integer),
                            new Claim(CustomClaimTypes.UserId, uId)
                        }),

                Expires = now.AddMonths(expireMonths),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }

    }
}