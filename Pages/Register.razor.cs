using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Pages
{
    public partial class Register
    {
        private const string SuccessfullMessage = "Registration successfull. You will receive email once your user is active. Thank you!";
        private UserForRegistration _userForRegistration = new UserForRegistration();
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public bool ShowAuthError { get; set; }
        public bool ShowSuccess { get; set; }
        public string Error { get; set; }

        protected async override Task OnInitializedAsync()
        {
            if (await AuthenticationService.UserIsLogged())
            {
                NavigationManager.NavigateTo("/");
            }
        }

        public async Task ExecuteRegister()
        {
            ShowAuthError = false;
            ShowSuccess = false;
            var result = await AuthenticationService.RegisterUser(_userForRegistration);
            if (result.Errors.Any())
            {
                Error = result.Errors.FirstOrDefault();
                ShowAuthError = true;
            }
            else
            {
                ShowSuccess = true;
                StateHasChanged();
            }
        }

        public void GotoLogin()
            => NavigationManager.NavigateTo("/");
    }
}
