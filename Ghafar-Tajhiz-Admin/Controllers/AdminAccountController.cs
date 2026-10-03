using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    public class AdminAccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AdminAccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true &&
                User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string phoneNumber,
            string password,
            bool rememberMe = false)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(password))
            {
                TempData["LoginError"] =
                    "شماره موبایل و رمز عبور را وارد کنید.";

                return View();
            }

            phoneNumber = phoneNumber.Trim();

            var user =
                await _userManager.FindByNameAsync(phoneNumber);

            if (user == null)
            {
                TempData["LoginError"] =
                    "شماره موبایل یا رمز عبور اشتباه است.";

                return View();
            }

            var isAdmin =
                await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                TempData["LoginError"] =
                    "شما اجازه ورود به پنل مدیریت را ندارید.";

                return View();
            }

            var result =
                await _signInManager.PasswordSignInAsync(
                    user,
                    password,
                    rememberMe,
                    lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            if (result.IsLockedOut)
            {
                TempData["LoginError"] =
                    "حساب کاربری شما موقتاً قفل شده است.";

                return View();
            }

            TempData["LoginError"] =
                "شماره موبایل یا رمز عبور اشتباه است.";

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}