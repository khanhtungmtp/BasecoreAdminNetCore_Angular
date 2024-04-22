namespace API.Configurations;

public class JwtSetting
{
    public string SecurityKey { get; set; } = string.Empty;

    public string ValidAudience { get; set; } = string.Empty;

    public string ValidIssuer { get; set; } = string.Empty;

    public TimeSpan ExpireInMinutes { get; set; }

    public string RefreshTokenValidityIn { get; set; } = string.Empty;

}
