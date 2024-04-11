namespace API.Configurations;

public class JwtTokenSettings
{
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public string SymmetricSecurityKey { get; set; } = string.Empty;
    public string JwtRegisteredClaimNamesSub { get; set; } = string.Empty;
}
