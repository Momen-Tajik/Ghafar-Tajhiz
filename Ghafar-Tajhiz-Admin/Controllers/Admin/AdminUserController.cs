using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ghafar_Tajhiz_Admin.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly GhafarTajhizShopDbContext _context;

        public AdminUserController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            GhafarTajhizShopDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            await LoadEditData(user);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string? fullName,
            string? phoneNumber,
            string? email,
            string? role,
            string? newPassword,
            string? confirmPassword)
        {
            if (id <= 0)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            role = role?.Trim();

            // فقط Roleهای موجود در سیستم پذیرفته شوند.
            if (!string.IsNullOrWhiteSpace(role) &&
                !await _roleManager.RoleExistsAsync(role))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "نقش انتخاب شده معتبر نیست.");

                await LoadEditData(user);
                return View(user);
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "رمز عبور جدید و تکرار آن یکسان نیست.");

                    await LoadEditData(user);
                    return View(user);
                }
            }

            user.FullName = string.IsNullOrWhiteSpace(fullName)
                ? null
                : fullName.Trim();

            user.PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
                ? null
                : phoneNumber.Trim();

            user.Email = string.IsNullOrWhiteSpace(email)
                ? null
                : email.Trim();

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                AddIdentityErrors(updateResult);

                await LoadEditData(user);
                return View(user);
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                var token =
                    await _userManager.GeneratePasswordResetTokenAsync(user);

                var passwordResult =
                    await _userManager.ResetPasswordAsync(
                        user,
                        token,
                        newPassword);

                if (!passwordResult.Succeeded)
                {
                    AddIdentityErrors(passwordResult);

                    await LoadEditData(user);
                    return View(user);
                }
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var currentRoles =
                    await _userManager.GetRolesAsync(user);

                var roleChanged =
                    currentRoles.Count != 1 ||
                    !currentRoles.Contains(role);

                if (roleChanged)
                {
                    if (currentRoles.Any())
                    {
                        var removeResult =
                            await _userManager.RemoveFromRolesAsync(
                                user,
                                currentRoles);

                        if (!removeResult.Succeeded)
                        {
                            AddIdentityErrors(removeResult);

                            await LoadEditData(user);
                            return View(user);
                        }
                    }

                    var addRoleResult =
                        await _userManager.AddToRoleAsync(
                            user,
                            role);

                    if (!addRoleResult.Succeeded)
                    {
                        AddIdentityErrors(addRoleResult);

                        await LoadEditData(user);
                        return View(user);
                    }
                }
            }

            TempData["Success"] = "اطلاعات کاربر با موفقیت بروزرسانی شد.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null &&
                user.Id == currentUser.Id)
            {
                TempData["Error"] =
                    "شما نمی‌توانید حساب خودتان را حذف کنید.";

                return RedirectToAction(nameof(Index));
            }

            var hasBasket = await _context.Baskets
                .AsNoTracking()
                .AnyAsync(b => b.UserId == user.Id);

            if (hasBasket)
            {
                TempData["Error"] =
                    "این کاربر دارای سابقه سبد خرید/سفارش است و حذف مستقیم او مجاز نیست.";

                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                AddIdentityErrors(result);

                TempData["Error"] =
                    "حذف کاربر انجام نشد.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] =
                "کاربر با موفقیت حذف شد.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadEditData(User user)
        {
            ViewBag.Roles =
                await _roleManager.Roles
                    .AsNoTracking()
                    .OrderBy(r => r.Name)
                    .ToListAsync();

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            ViewBag.CurrentRole =
                currentRoles.FirstOrDefault();
        }

        private void AddIdentityErrors(
            IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }
        }
    }
}