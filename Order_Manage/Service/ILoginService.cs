using Order_Manage.Dto.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;

namespace Order_Manage.Service
{
    public interface ILoginService
    {
        Task<ApiResponse<string?>> Create(RegisterRequest request);
        Task<ApiResponse<LoginResponse?>> Login(LoginRequest request);

        Task<ApiResponse<string?>> Register(SignupRequest request);
    }
}
