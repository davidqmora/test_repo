using System.Text.Json.Serialization;

namespace WebUI.Model;

public class RequiredProperties
{
    [JsonPropertyName("first_name")]
    public bool FirstName { get; set; }
    
    [JsonPropertyName("last_name")] 
    public bool LastName { get; set; }
    
    [JsonPropertyName("social_handle")]
    public bool SocialHandle { get; set; }
    
    [JsonPropertyName("preferred_email")]
    public bool PreferredEmail { get; set; }
    
    [JsonPropertyName("country_region")] 
    public bool CountryOrRegion { get; set; }
    
    [JsonPropertyName("age_indemnity")]
    public bool AgeOfIndemnity { get; set; }
    
    [JsonPropertyName("terms_of_use")]
    public bool TermsOfUseAgreement { get; set; }
}