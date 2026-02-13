using BusinessLogic.ProfileServices;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ghafar_Tajhiz.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;
        private readonly UserManager<User> _userManager;

        public ProfileController(ProfileService profileService, UserManager<User> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search, BasketStatus? status, string sort = "paiddate")
        {
            var user = await _userManager.GetUserAsync(User);


            var model = await _profileService.GetUserProfile(
                user.Id,
                user.FullName,
                user.PhoneNumber,
                search,
                status,
                sort
            );

            return View(model);
        }


    }

}
