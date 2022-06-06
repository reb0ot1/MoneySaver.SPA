using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Services
{
    public interface IAuthenticationService
    {
        //Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);
        Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthenticationDto);
        Task Logout();
    }
}
