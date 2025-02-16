using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MoneySaver.SPA.Features;
using System.Security.Claims;

namespace MoneySaver.SPA.AuthProviders
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage, IHttpClientFactory httpClientFactory)
        {
            this._httpClient = httpClient;
            this._localStorage = localStorage;
            this._anonymous = new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await this._localStorage.GetItemAsStringAsync("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return this._anonymous;
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var emailClaim = JwtParser.ParseClaimsFromJwt(token).FirstOrDefault(e => e.Type == ClaimTypes.Email);
            this._httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuthType")));
        }

        public void NotifyUserAuthentication(string email)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            this.NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(this._anonymous);
            this.NotifyAuthenticationStateChanged(authState);
        }
    }
}
