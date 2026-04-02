using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SchoolJournalAuthApi.Services
{
    public class JwtClaimReader
    {
        public static IEnumerable<Claim> ReadClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims;
        }
        public static string? GetClaim(string token, string claimType)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
