using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<bool>> RegisterUser(UserForRegistration userForRegistration);
        Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthenticationDto);
        Task Logout();
        Task<bool> UserIsLogged();
    }
}
