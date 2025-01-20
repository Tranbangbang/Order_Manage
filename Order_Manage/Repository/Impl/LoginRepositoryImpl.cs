using Microsoft.AspNetCore.Identity;
using Order_Manage.Models;

namespace Order_Manage.Repository.Impl
{
    public class LoginRepositoryImpl : ILoginRepository
    {

        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginRepositoryImpl(UserManager<Account> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<Account?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CreateAsync(Account account, string password)
        {
            var result = await _userManager.CreateAsync(account, password);
            return result.Succeeded;
        }

        public async Task<IList<string>> GetRolesAsync(Account account)
        {
            return await _userManager.GetRolesAsync(account);
        }

        public async Task<bool> CheckPasswordAsync(Account account, string password)
        {
            return await _userManager.CheckPasswordAsync(account, password);
        }

        public async Task<bool> AddToRoleAsync(Account account, string role)
        {
            var result = await _userManager.AddToRoleAsync(account, role);
            return result.Succeeded;
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<bool> DeleteAsync(Account account)
        {
            var result = await _userManager.DeleteAsync(account);
            return result.Succeeded;
        }

        public Task<bool> RegisterAsync(Account account, string password)
        {
            throw new NotImplementedException();
        }
    }
}
