using System.Text.Json.Serialization;

namespace WebUI.Model;

public class PhotoComment
{
    [JsonPropertyName("display_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string DisplayName { get; set; } = default!;
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

}