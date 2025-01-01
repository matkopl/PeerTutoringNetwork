using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeerTutoringNetwork.Security
{
    public class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expiration, string subject = null, string userId = null, string roleId = null)
        {
            // Get secret key bytes
            var tokenKey = Encoding.UTF8.GetBytes(secureKey);

            // Create claims
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddMinutes(expiration).ToString()),
    };

            if (!string.IsNullOrEmpty(subject))
            {
                claims.Add(new Claim(ClaimTypes.Name, subject));
            }
            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim("userId", userId));
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                claims.Add(new Claim("roleId", roleId));
            }

            // Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
