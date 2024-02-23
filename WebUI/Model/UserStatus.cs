
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebUI.Model;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum UserStatus
{
    [EnumMember(Value="enabled")]
    Enabled,
    [EnumMember(Value="not_enabled")]
    NotEnabled,
    [EnumMember(Value="partially_enabled")]
    PartiallyEnabled
}