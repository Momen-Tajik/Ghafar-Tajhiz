using BusinessLogic.ProfileServices;
using BusinessLogic.ProfileServices.Models;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;
        private readonly UserManager<User> _userManager;

        public ProfileController(
            ProfileService profileService,
            UserManager<User> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            BasketStatus? status,
            string sort = "paiddate")
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var model =
                await _profileService.GetUserProfile(
                    user.Id,
                    user.FullName ?? "کاربر",
                    user.PhoneNumber ?? string.Empty,
                    search,
                    status,
                    sort);

            return View(model);
        }
    }
}