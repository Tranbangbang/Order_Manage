using Order_Manage.Common.Constants.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Models;

namespace Order_Manage.Service
{
    public interface IAuthService
    {
        Task<ApiResponse<string?>> Create(RegisterRequest request);
        Task<ApiResponse<LoginResponse?>> Login(LoginRequest request);

        Task<ApiResponse<string?>> Register(SignupRequest request);

        Task<ApiResponse<Account?>> View(string id);
        Task<ApiResponse<string>> Update(string id, UpdateUserRequest request);
        Task<ApiResponse<string>> Delete(string id);
    }
}
