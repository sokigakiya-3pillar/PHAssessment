using System.Text.Json.Serialization;

namespace Propeller.Models
{
    public class NoteDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime TimeStamp { get; set; }
    }
}
