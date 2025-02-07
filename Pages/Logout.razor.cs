using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Pages
{
    public partial class Logout
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await AuthenticationService.LogoutAsync();
            NavigationManager.NavigateTo("/");
        }
    }
}
