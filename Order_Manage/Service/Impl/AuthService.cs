using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Models;
using Order_Manage.Repository;
using Order_Manage.Dto.Helper;
using Order_Manage.Dto.Mapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Order_Manage.Service.Impl
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _loginRepository;
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepository;

        public AuthService(IAuthRepository accountRepository, IConfiguration configuration, IAccountRepository repository)
        {
            _loginRepository = accountRepository;
            _configuration = configuration;
            _accountRepository = repository;
        }

        public async Task<ApiResponse<string?>> Create(RegisterRequest request)
        {
            try
            {
                var existingUser = await _loginRepository.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return ApiResponse<string?>.Error(400, "User already exists");
                }
                var roleExists = await _loginRepository.RoleExistsAsync(request.Role);
                if (!string.IsNullOrEmpty(request.Role) && !roleExists)
                {
                    return ApiResponse<string?>.Error(400, "Role does not exist");
                }
                var account = new Account
                {
                    UserName = request.Email,
                    Email = request.Email,
                    AccountName = request.AccountName,
                    Major = request.Major,
                    createAt = DateTime.Now
                };
                var created = await _loginRepository.CreateAsync(account, request.Password);
                if (!created)
                {
                    return ApiResponse<string?>.Error(500, "Failed to create user");
                }
                if (!string.IsNullOrEmpty(request.Role))
                {
                    var roleAssigned = await _loginRepository.AddToRoleAsync(account, request.Role);
                    if (!roleAssigned)
                    {
                        await _loginRepository.DeleteAsync(account);
                        return ApiResponse<string?>.Error(500, "Failed to assign role to user");
                    }
                }
                return ApiResponse<string?>.Success(null, "User registered successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string?>.Error(500, $"An error occurred: {ex.Message}");
            }
        }


        public async Task<ApiResponse<LoginResponse?>> Login(LoginRequest request)
        {
            try
            {
                var user = await _loginRepository.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return ApiResponse<LoginResponse?>.Error(404, "User not found");
                }

                var passwordValid = await _loginRepository.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                {
                    return ApiResponse<LoginResponse?>.Error(401, "Invalid password");
                }

                var roles = await _loginRepository.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:ExpiryInDays"])),
                    signingCredentials: creds
                );

                return ApiResponse<LoginResponse?>.Success(new LoginResponse
                {
                    Successful = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                }, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponse?>.Error(500, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string?>> Register(SignupRequest request)
        {
            try
            {
                var existingUser = await _loginRepository.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return ApiResponse<string?>.Error(400, "User already exists");
                }
                int viaCode = _accountRepository.GetViaCodeByEmail(request.Email);
                if (request.Code!=viaCode)
                {
                    return ApiResponse<string?>.Error(400, "Invalid or expired verification code");
                }
                var account = new Account
                {
                    UserName = request.Email,
                    Email = request.Email,
                    AccountName = request.AccountName,
                    createAt = DateTime.Now
                };
                var created = await _loginRepository.CreateAsync(account, request.Password);
                if (!created)
                {
                    return ApiResponse<string?>.Error(500, "Failed to create user");
                }
                if (!string.IsNullOrEmpty(UserRoles.User))
                {
                    var roleAssigned = await _loginRepository.AddToRoleAsync(account, UserRoles.User);
                    if (!roleAssigned)
                    {
                        await _loginRepository.DeleteAsync(account);
                        return ApiResponse<string?>.Error(500, "Failed to assign role to user");
                    }
                }
                return ApiResponse<string?>.Success(null, "User registered successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string?>.Error(500, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> Delete(string id)
        {
            try
            {
                var account = await _loginRepository.FindByIdAsync(id);
                if (account == null)
                {
                    return ApiResponse<string>.Error(404, "User not found");
                }

                var deleted = await _loginRepository.DeleteAsync(account);
                if (!deleted)
                {
                    return ApiResponse<string>.Error(500, "Failed to delete user");
                }

                return ApiResponse<string>.Success(null, "User deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(500, $"An error occurred: {ex.Message}");
            }

        }

        public async Task<ApiResponse<Account?>> View(string id)
        {
            try
            {
                var account = await _loginRepository.ViewAsync(id);
                if (account == null)
                {
                    return ApiResponse<Account?>.Error(404, "User not found");
                }
                return ApiResponse<Account?>.Success(account);
            }
            catch (Exception ex) {
                return ApiResponse<Account?>.Error(500, $"An error occurred: {ex.Message}");
            }
            
        }

        public async Task<ApiResponse<string>> Update(string id, UpdateUserRequest request)
        {
            try
            {
                if (id != request.Id)
                    return ApiResponse<string>.Error(Exceptions.ErrorCode.CANNOT_UPDATE_ACCOUNT);

                var account = await _loginRepository.FindByIdAsync(id);
                if (account == null)
                {
                    return ApiResponse<string>.Error(404, "User not found");
                }
                UserMapper.MapUpdateRequestToEntity(request, account);

                var updated = await _loginRepository.UpdateAsync(account);
                if (!updated)
                {
                    return ApiResponse<string>.Error(500, "Failed to update user");
                }
                return ApiResponse<string>.Success(null, "User updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
