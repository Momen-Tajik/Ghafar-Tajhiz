using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;

namespace Ghafar_Tajhiz_Admin.Controllers.Admin
{
    //[Authorize(Roles = "Admin")]
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

        // GET: /AdminUser/Index
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();

            return View(users);
        }

        // GET: /AdminUser/Details/5
        public async Task<IActionResult> Details(int id)
        {
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
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();

            var currentRoles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;
            ViewBag.CurrentRole = currentRoles.FirstOrDefault();

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
            // پیدا کردن کاربر
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();


            // ==========================================
            // 1. بررسی رمز عبور جدید
            // ==========================================

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError(
                        "",
                        "رمز عبور جدید و تکرار آن یکسان نیست.");

                    await LoadEditData(user);

                    return View(user);
                }
            }


            // ==========================================
            // 2. بروزرسانی اطلاعات کاربر
            // ==========================================

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.Email = email;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error.Description);
                }

                await LoadEditData(user);

                return View(user);
            }


            // ==========================================
            // 3. تغییر رمز عبور
            // ==========================================

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
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(
                            "",
                            error.Description);
                    }

                    await LoadEditData(user);

                    return View(user);
                }
            }


            // ==========================================
            // 4. تغییر Role
            // ==========================================

            if (!string.IsNullOrWhiteSpace(role))
            {
                var currentRoles =
                    await _userManager.GetRolesAsync(user);

                if (currentRoles.Any())
                {
                    var removeResult =
                        await _userManager.RemoveFromRolesAsync(
                            user,
                            currentRoles);

                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                        {
                            ModelState.AddModelError(
                                "",
                                error.Description);
                        }

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
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(
                            "",
                            error.Description);
                    }

                    await LoadEditData(user);

                    return View(user);
                }
            }


            // ==========================================
            // پایان
            // ==========================================

            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminUser/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();


            // جلوگیری از حذف حساب خودش
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && user.Id == currentUser.Id)
            {
                TempData["Error"] =
                    "شما نمی‌توانید حساب خودتان را حذف کنید.";

                return RedirectToAction(nameof(Index));
            }


            // بررسی وجود سبد خرید
            var hasBasket = await _context.Baskets
                .AnyAsync(b => b.UserId == user.Id);

            if (hasBasket)
            {
                TempData["Error"] =
                    "این کاربر دارای سبد خرید است و امکان حذف او وجود ندارد.";

                return RedirectToAction(nameof(Index));
            }


            // حذف کاربر
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = "حذف کاربر انجام نشد.";

                foreach (var error in result.Errors)
                {
                    TempData["Error"] += $" {error.Description}";
                }

                return RedirectToAction(nameof(Index));
            }


            TempData["Success"] =
                "کاربر با موفقیت حذف شد.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadEditData(User user)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;
            ViewBag.CurrentRole = currentRoles.FirstOrDefault();
        }

    }
}