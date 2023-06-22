using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Identity.FrontEndAPI
{
    public class Registration
    {
        private readonly ILogger<Registration> _logger;

        public Registration(ILogger<Registration> log)
        {
            _logger = log;
        }

        [FunctionName("Registration")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LoginRequestDTO), Description = "The Order To Create")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req)
        {
         
            // Replace with your own secret key
            string secretKey = "eyJEIjoiaDBHTFRSQW5mZWswK1kvTG9tc3YyOFNBTjhGMUpNVTZqOEZwMkx0ZXhRV1NVMkpZMWxJOFZ4aSs0bGlrSzlhTStkZXB1SHlQNUl4bGN1TytkNzlpOEFZaWZQeVZZbGpNVTkwNkRPQ3gxUlR0dDlwUlRaNjB3dDlsSnR1czQ4eHgwV0NlWm9ZektZL1ZDaFE2VDdYRngzd2JIZVBVVjdQc0J6dWo2RFNoSG9FPSIsIkRQIjoiYklDeFF6WDBuazB3a080dC9rc1BUUEo4Z2pqd29vcGxqK3ZUZ0lxSE5ZbmZLcVRDN1A3RGt0VXJUY1JXa0NleEEzMUFXeU5OYXVtb0M3ZmhyaWo1WHc9PSIsIkRRIjoiNU0xUyt5N3RGRzVJV3BrRi9GYkNCKzJKaDhiaDRXeS9oVllZVm1POGI0MHVwUjFNRitFSENjeFVKbitvckNubERCWThva3JkVHQrdkJ4cGgxa1pSenc9PSIsIkV4cG9uZW50IjoiQVFBQiIsIkludmVyc2VRIjoiVy9PZ3V2NVJWZDgwSzFmWm9Pdm1sMjV1bWwzV2NLVjNMQWlrK3h4ZndaMkx3cStMSnVPdG5ENWM0ZGJjek4yWHhRWDFraEYvSVBzcUpOZ29hT2ZYOEE9PSIsIk1vZHVsdXMiOiJ0V2cxRGJreE1lRVZlVHcybE5ML2lKS3IvUDI0Y2s2ZnRqYWV1NkJsUXhVVUZMcW9SL3E3cGVwU0gwNVRnejNUSit5OU1WSmNDNHhmNURmckJTK1ZvVmJ2eXJPRWgxMnkwdUk4U2FhcXIxUWlXdFFZaG9EZm9Mc2pRakgvS2xMRDZZUll4ZVpQeTBkUXYxN2NQdTdMWUJoLzFBdnlrSy9YQUQzQnJUeVg3a1U9IiwiUCI6InhURW8wMzJFaFRoU3U4cExmcmdQNXVrTGVGVW5DTDhOSFVBLzdEamdSbEVsdzRCSTBoYUpXYVUzeUx6NjIwNkF1cHU5RXBjTzV3cEhlQmc3WkNiQjl3PT0iLCJRIjoiNjRIdFVtOTJTQTVNOTVrcmN1VEM3czFHN3hRalhmWW5OYndVNEpRcU92RUVDSzgxb3UwUDlBeExoUCs2THpSMmFKdGNqY3lHbWxka2tuaDRseTJDb3c9PSJ9";
            string publicKey = "eyJEIjpudWxsLCJEUCI6bnVsbCwiRFEiOm51bGwsIkV4cG9uZW50IjoiQVFBQiIsIkludmVyc2VRIjpudWxsLCJNb2R1bHVzIjoidFdnMURia3hNZUVWZVR3MmxOTC9pSktyL1AyNGNrNmZ0amFldTZCbFF4VVVGTHFvUi9xN3BlcFNIMDVUZ3ozVEoreTlNVkpjQzR4ZjVEZnJCUytWb1ZidnlyT0VoMTJ5MHVJOFNhYXFyMVFpV3RRWWhvRGZvTHNqUWpIL0tsTEQ2WVJZeGVaUHkwZFF2MTdjUHU3TFlCaC8xQXZ5a0svWEFEM0JyVHlYN2tVPSIsIlAiOm51bGwsIlEiOm51bGx9";

            // Replace with your own issuer and audience
            string issuer = "your_issuer";
            string audience = "your_audience";

            // Replace with the desired expiration time
            DateTime expiration = DateTime.UtcNow.AddHours(1);

            // Retrieve user credentials from the request (e.g., username and password)
            // Validate the credentials and generate the claims for the token
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<LoginRequestDTO>(requestBody);

            var username = data.Email;
            var password = data.Password;

            if (IsValidCredentials(username, password))
            {
                var result =  BLL.TokenManager.ValidateAndGenerateToken(secretKey, issuer, audience, expiration, username);
                return new OkObjectResult(result);
            }

            return new UnauthorizedResult();
        }

       
        private bool IsValidCredentials(StringValues username, StringValues password)
        {
            // ToDo : Logic ke database
            return true;
        }
    }
}

