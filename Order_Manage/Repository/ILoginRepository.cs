﻿using Order_Manage.Models;

namespace Order_Manage.Repository
{
    public interface ILoginRepository
    {
        Task<Account?> FindByEmailAsync(string email);
        Task<bool> CreateAsync(Account account, string password);


        Task<bool> RegisterAsync(Account account, string password);
        Task<IList<string>> GetRolesAsync(Account account);
        Task<bool> CheckPasswordAsync(Account account, string password);
        Task<bool> AddToRoleAsync(Account account, string role);
        Task<bool> RoleExistsAsync(string role);
        Task<bool> DeleteAsync(Account account);
    }
}
