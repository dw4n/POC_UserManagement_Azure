using Newtonsoft.Json;
using System;

namespace UserManagement.Identity.FrontEndAPI
{
    public class LoginRequestDTO
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
