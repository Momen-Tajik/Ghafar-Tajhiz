using Ghafar_Tajhiz_Admin.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace Ghafar_Tajhiz_Admin.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserListViewModel>> GetUsersAsync()
        {
            return await _userManager.Users
                .AsNoTracking()
                .Select(user => new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed
                })
                .ToListAsync();
        }
    }
}