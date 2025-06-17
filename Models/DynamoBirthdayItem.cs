using System.Text.Json.Serialization;

namespace IlemlamlaBlazor.Models
{
    public class DynamoBirthdayItem
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("Position")]
        public string Position { get; set; } = string.Empty;
    }
} 