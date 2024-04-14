namespace ViewModels.Auth;

public class AuthResponse
{
    public string Id { get; set; } =string.Empty;
    public string Username { get; set; } =string.Empty;
    public string? Email { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
