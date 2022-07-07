using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MoneySaver.SPA.AuthProviders;
using MoneySaver.SPA.Models;
using System.Text;
using System.Text.Json;

namespace MoneySaver.SPA.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(
            HttpClient client,
            AuthenticationStateProvider authenticationState,
            ILocalStorageService localStorageService
            )
        {
            this._client = client;
            this._options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            this._authStateProvider = authenticationState;
            this._localStorage = localStorageService;
        }
        public async Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthenticationDto)
        {
            var content = JsonSerializer.Serialize(userForAuthenticationDto);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var baseUri = new Uri("http://localhost:7098/");
            var myUri = new Uri(baseUri, "login");
            var authResult = await this._client.PostAsync(myUri, bodyContent);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (!authResult.IsSuccessStatusCode)
            {
                var errors = JsonSerializer.Deserialize<List<string>>(authContent);
                return new AuthResponseDto
                {
                    ErrorMessage = errors.FirstOrDefault(),
                    IsAuthSuccessful = false
                };
            }

            var result = JsonSerializer.Deserialize<AuthResponseDto>(authContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            await this._localStorage.SetItemAsStringAsync("authToken", result.Token);
            ((AuthStateProvider)this._authStateProvider).NotifyUserAuthentication(userForAuthenticationDto.Email);
            this._client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.Token);

            return new AuthResponseDto { IsAuthSuccessful = true };
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
