namespace Tradibit.Common.SettingsDTO;

public class AuthConfig
{
    public GoogleAuth GoogleAuth { get; set; }
}

public class GoogleAuth
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}