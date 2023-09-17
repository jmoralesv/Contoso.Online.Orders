using System.Text.Json.Serialization;

namespace ContosoOnlineOrders.Functions
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("inventoryCount")]
        public int InventoryCount { get; set; }
    }
}
