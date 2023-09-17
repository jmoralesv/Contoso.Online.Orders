using System.Text.Json.Serialization;

namespace ContosoOnlineOrders.Functions
{
    public class CardMetadata
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("createdUtc")]
        public DateTime CreatedUtc { get; set; }

        [JsonPropertyName("creator")]
        public Creator Creator { get; set; }

        [JsonPropertyName("Products")]
        public List<Product> Products { get; set; } = new List<Product>();
    }

    public class Creator
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("profileImage")]
        public string ProfileImage { get; set; }
    }
}
