using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Tradibit.Client.Shared;
using Tradibit.SharedUI;
using Tradibit.SharedUI.Extensions;
using Tradibit.SharedUI.Interfaces;
using Tradibit.SharedUI.Interfaces.API;

namespace Tradibit.Client
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider, ITokenProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IAccountApi _accountApi;
        private readonly RequestExt _requestExt;

        public TokenAuthenticationStateProvider(ILocalStorageService localStorage,
            IAccountApi accountApi,
            RequestExt requestExt)
        {
            _localStorage = localStorage;
            _accountApi = accountApi;
            _requestExt = requestExt;
        }
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(await GetClaims(), "Google")));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task<string?> GetToken()
        {
            var token = await _localStorage.GetItemAsync<string>(Constants.AUTH_TOKEN_KEY) ?? await GetTokenFromApi();
            var claims = ParseClaimsFromJwt(token).ToList();
            var expiryTime = Convert.ToDouble(claims.FirstOrDefault(x => x.Type == "exp")?.Value ?? "").FromUnixTimeStamp();
            if (expiryTime < DateTime.Now) 
                token = await GetTokenFromApi();

            return token;
        }

        public async Task<IEnumerable<Claim>> GetClaims() => 
            ParseClaimsFromJwt(await GetToken());

        private async Task<string?> GetTokenFromApi()
        {
            var token = await _requestExt.Send(async () => await _accountApi.GetCurrentUserToken());
            if (!string.IsNullOrEmpty(token))
                await _localStorage.SetItemAsync(Constants.AUTH_TOKEN_KEY, token);
            
            return token; 
        }
        public async Task<bool> IsSuperAdmin()
        {
            var claims = await GetClaims();
            return claims.Any(x => x.Type == "UserRoleId" && x.Value == "1");
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string? jwt)
        {
            if (string.IsNullOrEmpty(jwt) || !jwt.Contains('.'))
                return new List<Claim>();
            
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            var jsonBytes = Convert.FromBase64String(payload);
            
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            if (keyValuePairs == null)
                return new List<Claim>();
            
            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);
            if (roles != null)
            {
                if (roles.ToString()?.Trim().StartsWith("[") ?? false)
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);
                    foreach (var parsedRole in parsedRoles!)
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()!));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }
            
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? "")));
            return claims;
        }
    }
}
