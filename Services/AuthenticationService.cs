using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using MoneySaver.SPA.AuthProviders;
using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using System.Text.Json;
using MoneySaver.SPA.Features;

namespace MoneySaver.SPA.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly string _identityAddress;

        public AuthenticationService(
            HttpClient client,
            AuthenticationStateProvider authenticationState,
            ILocalStorageService localStorageService,
            IOptions<SpaSettings> spaSettings
            )
        {
            this._client = client;
            this._authStateProvider = authenticationState;
            this._localStorage = localStorageService;
            this._identityAddress = spaSettings.Value.AuthenticationAddress;
        }
        public async Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthenticationDto)
        {
            var bodyContent = RequestContent.CreateContent(userForAuthenticationDto);
            
            var authUri = new Uri(new Uri(this._identityAddress), "login");
            var authResult = await this._client.PostAsync(authUri, bodyContent);
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
            var token = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", result.Token);
            this._client.DefaultRequestHeaders.Authorization = token;

            return new AuthResponseDto { IsAuthSuccessful = true };
        }

        public async Task<bool> UserIsLogged()
        {

            var token = await this._localStorage.GetItemAsStringAsync("authToken");

            if (token == null)
            {
                return false;
            }

            var context = await this._authStateProvider.GetAuthenticationStateAsync();
            var emailClaim = context.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Name);
            
            if (emailClaim is null)
            {
                var emailFromToken = JwtParser.ParseClaimsFromJwt(token).FirstOrDefault(e => e.Type == "email");
                ((AuthStateProvider)this._authStateProvider).NotifyUserAuthentication(emailFromToken.Value);
            }

            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<ServiceResult<bool>> RegisterUser(UserForRegistration userForRegistration)
        {
            var bodyContent = RequestContent.CreateContent(userForRegistration);
            var authUri = new Uri(new Uri(this._identityAddress), "register");
            var authResult = await this._client.PostAsync(authUri, bodyContent);
            var authContent = await authResult.Content.ReadAsStringAsync();
            if (!authResult.IsSuccessStatusCode)
            {
                var errors = JsonSerializer.Deserialize<List<string>>(authContent);
                return new ServiceResult<bool>
                {
                    Errors = errors
                };
            }

            return new ServiceResult<bool>
            {
                Result = true
            };
        }
    }
}
