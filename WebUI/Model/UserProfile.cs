using System.Text.Json.Serialization;

namespace WebUI.Model;

public class UserProfile
{
    [JsonPropertyName("social_handle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SocialHandle { get; set; }
    
    [JsonPropertyName("preferred_email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PreferredEmail { get; set; }
    
    [JsonPropertyName("country_region")] 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CountryOrRegion { get; set; }
    
    [JsonPropertyName("age_indemnity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AgeOfIndemnity { get; set; }
    
    [JsonPropertyName("terms_of_use")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? TermsOfUseAgreement { get; set; }
}