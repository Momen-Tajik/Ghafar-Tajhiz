using DataAccess.Models;
using Ghafar_Tajhiz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByNameAsync(model.PhoneNumber);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "این شماره موبایل قبلاً ثبت شده است.");

                return View(model);
            }

            var user = new User
            {
                PhoneNumber = model.PhoneNumber,
                UserName = model.PhoneNumber,
                FullName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _signInManager.SignInAsync(
                user,
                isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.PhoneNumber,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "حساب کاربری شما موقتاً قفل شده است.");

                return View(model);
            }

            ModelState.AddModelError(
                string.Empty,
                "شماره موبایل یا رمز عبور اشتباه است.");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}