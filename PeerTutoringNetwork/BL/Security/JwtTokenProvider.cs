using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeerTutoringNetwork.Security
{
    public class JwtTokenProvider
    {
            public static string CreateToken(string secureKey, int expiration, string subject = null, string userId = null)
            {
                // Get secret key bytes
                var tokenKey = Encoding.UTF8.GetBytes(secureKey);

                // Create a list of claims
                var claims = new List<Claim>();

                // Add subject claim if available
                if (!string.IsNullOrEmpty(subject))
                {
                    claims.Add(new Claim(ClaimTypes.Name, subject)); // User's username
                    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject)); // Standard subject claim
                }

                // Add userId claim if provided
                if (!string.IsNullOrEmpty(userId))
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, userId)); // Standard claim for user ID
                    claims.Add(new Claim("userId", userId)); // Custom claim for user ID
                }

                // Create a token descriptor
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(expiration),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                // Create token using that descriptor
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Serialize and return the token
                return tokenHandler.WriteToken(token);
            }
        }
    }
