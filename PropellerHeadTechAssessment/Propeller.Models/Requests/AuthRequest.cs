using System.Text.Json.Serialization;

namespace Propeller.Models.Requests
{
    [Serializable]
    public class AuthRequest
    {
        [JsonPropertyName("uid")]
        public string UserId { get; set; }

        [JsonPropertyName("pwd")]
        public string Password { get; set; }
    }
}
