using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace UserManagement.Identity.BLL
{
    public class TokenManager
    {
        public static string ValidateAndGenerateToken(string secretKey, string issuer, string audience, DateTime expiration, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                // Add any additional claims you need
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            RSAParameters privateKey;

            try
            {
                byte[] data = Convert.FromBase64String(secretKey);

                privateKey = JsonConvert.DeserializeObject<RSAParameters>(System.Text.Encoding.ASCII.GetString(data));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to parse 'Nexus.Identity.PrivateKey' settings", ex);
            }

            var key = new RsaSecurityKey(privateKey);

            var signingKey = new SigningCredentials(key, SecurityAlgorithms.RsaSsaPssSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingKey
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }


        /// <summary>
        /// Generate 1024-bit RSA key pair to be use as JWT secret
        /// </summary>
        /// <returns></returns>
        public static TokenDTO GenerateKeyPair()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            var privateKey = JsonConvert.SerializeObject(rsa.ExportParameters(true));
            var publicKey = JsonConvert.SerializeObject(rsa.ExportParameters(false));

            return new TokenDTO()
            {
                PrivateKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(privateKey)),

                PublicKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(publicKey))
            };
        }

    }
}
