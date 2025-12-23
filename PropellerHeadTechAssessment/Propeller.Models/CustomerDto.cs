using System.Text.Json.Serialization;

namespace Propeller.Models
{
    [Serializable]
    public class CustomerDto
    {
        [JsonPropertyName("id")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("notes")]
        public IEnumerable<NoteDto> Notes { get; set; } = new List<NoteDto>();

        [JsonPropertyName("contacts")]
        public IEnumerable<ContactDto> Contacts { get; set; } = new List<ContactDto>();

    }
}