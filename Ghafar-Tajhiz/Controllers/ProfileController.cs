using BusinessLogic.ProfileServices;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ghafar_Tajhiz.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<IActionResult> Index(string? search, BasketStatus? status, string sort = "paiddate")
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var model = await _profileService.GetUserProfile(userId, search, status, sort);

            return View(model);
        }
    }
}
