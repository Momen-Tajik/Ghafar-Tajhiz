using BusinessLogic.ProfileServices;
using BusinessLogic.ProfileServices.Models;
using DataAccess.Enums;
using DataAccess.Models;
using Ghafar_Tajhiz.Models;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResubmitReceipt(ResubmitReceiptDto model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }


            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();


            var result = await _profileService.ResubmitReceipt(
                user.Id,
                model.BasketId,
                model.Receipt!);


            if (!result)
            {
                TempData["Error"] =
                    "ارسال مجدد رسید انجام نشد. وضعیت سفارش را بررسی کنید.";

                return RedirectToAction(nameof(Index));
            }


            TempData["Success"] =
                "رسید جدید با موفقیت ارسال شد و در انتظار بررسی است.";


            return RedirectToAction(nameof(Index));
        }
    }
}