using IdentityService.Application.Models;

namespace IdentityService.Application.Services.Abstract
{
    public interface IIdentityService
    {
        Task<LoginResponseModel> Login(LoginRequestModel requestModel);
    }
}
