using System.Text.Json.Serialization;

namespace WebUI.Model;

public class PhotoData
{
    [JsonPropertyName("published")]
    public DateTimeOffset Published { get; set; }
    
    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = default!;
    
    [JsonPropertyName("location")]
    public string LocationDescription { get; set; } = default!;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;
    
    [JsonPropertyName("comment_count")]
    public uint CommentCount { get; set; }
    
    [JsonPropertyName("like_count")]
    public uint LikeCount { get; set; }
    
    [JsonPropertyName("flag_count")]
    public uint FlagCount { get; set; }
    
    [JsonPropertyName("view_count")]
    public uint ViewCount { get; set; }
    
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = default!;
}