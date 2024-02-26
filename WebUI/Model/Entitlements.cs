using System.Text.Json.Serialization;

namespace WebUI.Model;

public class Entitlements
{
    [JsonPropertyName("photo_share")]
    public bool PhotoShare { get; set; }
    [JsonPropertyName("investor")]
    public bool Investor { get; set; }
    [JsonPropertyName("food_truck")]
    public bool FoodTruck { get; set; }
}