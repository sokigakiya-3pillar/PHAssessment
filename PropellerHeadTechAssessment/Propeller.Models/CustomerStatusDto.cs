using System.Text.Json.Serialization;

namespace Propeller.Models
{
    public class CustomerStatusDto
    {
        [JsonPropertyName("id")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

    }
}
