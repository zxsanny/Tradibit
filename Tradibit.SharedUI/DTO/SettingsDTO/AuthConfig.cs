namespace Tradibit.SharedUI.DTO.SettingsDTO;

public class AuthConfig
{
    public GoogleAuth? GoogleAuth { get; set; }
    public string? Secret { get; set; }
    public int TokenExpInSeconds { get; set; }
}

public class GoogleAuth
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}