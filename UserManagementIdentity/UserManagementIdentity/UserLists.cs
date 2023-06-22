using System.IdentityModel.Tokens.Jwt;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace UserManagement.Identity.FrontEndAPI
{
    public class UserLists
    {
        private readonly ILogger<UserLists> _logger;

        public UserLists(ILogger<UserLists> log)
        {
            _logger = log;
        }

        [FunctionName("UserLists")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Protected" })]
        [OpenApiSecurity("Bearer", SecuritySchemeType.Http, Name = "authorization", Scheme = OpenApiSecuritySchemeType.Bearer, In = OpenApiSecurityLocationType.Header, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {

            // Retrieve the JWT token from the request headers
            var token = req.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Replace with your own secret key
            string publicKey = "eyJEIjpudWxsLCJEUCI6bnVsbCwiRFEiOm51bGwsIkV4cG9uZW50IjoiQVFBQiIsIkludmVyc2VRIjpudWxsLCJNb2R1bHVzIjoidFdnMURia3hNZUVWZVR3MmxOTC9pSktyL1AyNGNrNmZ0amFldTZCbFF4VVVGTHFvUi9xN3BlcFNIMDVUZ3ozVEoreTlNVkpjQzR4ZjVEZnJCUytWb1ZidnlyT0VoMTJ5MHVJOFNhYXFyMVFpV3RRWWhvRGZvTHNqUWpIL0tsTEQ2WVJZeGVaUHkwZFF2MTdjUHU3TFlCaC8xQXZ5a0svWEFEM0JyVHlYN2tVPSIsIlAiOm51bGwsIlEiOm51bGx9";

            // Replace with your own issuer and audience
            string issuer = "your_issuer";
            string audience = "your_audience";

            var tokenHandler = new JwtSecurityTokenHandler();
      
            RSAParameters publicKeyConverted;

            try
            {
                byte[] data = Convert.FromBase64String(publicKey);

                string jwt = System.Text.Encoding.ASCII.GetString(data);

                publicKeyConverted = JsonConvert.DeserializeObject<RSAParameters>(jwt);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to parse 'Nexus.Identity.PublicKey' settings", ex);
            }


            try
            {
                // Set the validation parameters for token validation
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new RsaSecurityKey(publicKeyConverted)
                };

                // Validate the token
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Access the claims from the validated token
                var username = principal.Identity.Name;

                // Perform your protected function logic here

                return new OkObjectResult($"Hello, {username}! This is a protected function.");
            }
            catch (Exception ex)
            {
                // Token validation failed
                return new UnauthorizedResult();
            }

        }
    }
}

