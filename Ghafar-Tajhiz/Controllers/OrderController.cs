using BusinessLogic.BasketItemServices;
using BusinessLogic.BasketServices;
using Ghafar_Tajhiz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ghafar_Tajhiz.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly BasketService _basketService;
        private readonly BasketItemService _basketItemService;

        public OrderController(
            BasketService basketService,
            BasketItemService basketItemService)
        {
            _basketService = basketService;
            _basketItemService = basketItemService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToBasket(
            [FromBody] AddBasketDto model)
        {
            if (model == null || model.Qty <= 0)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "اطلاعات نامعتبر است."
                });
            }

            var userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized(new
                {
                    res = false,
                    msg = "لطفاً ابتدا وارد حساب کاربری شوید."
                });
            }

            var result = await _basketService.AddToBasket(
                model.ProductId,
                model.Qty,
                userId.Value);

            if (!result)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "افزودن محصول به سبد خرید انجام نشد."
                });
            }

            return Ok(new
            {
                res = true,
                msg = "محصول با موفقیت به سبد خرید اضافه شد."
            });
        }

        [HttpGet]
        public async Task<IActionResult> Basket()
        {
            var userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Unauthorized();

            var data = await _basketService.GetUserBasket(userId.Value);

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBasketItem(
            [FromBody] RemoveBasketItemDto model)
        {
            if (model == null || model.BasketItemId <= 0)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "شناسه سبد خرید نامعتبر است."
                });
            }

            var userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized(new
                {
                    res = false,
                    msg = "لطفاً ابتدا وارد حساب کاربری شوید."
                });
            }

            var result = await _basketItemService.RemoveBasketItem(
                model.BasketItemId,
                userId.Value);

            if (!result)
            {
                return NotFound(new
                {
                    res = false,
                    msg = "آیتم سبد خرید پیدا نشد."
                });
            }

            return Ok(new
            {
                res = true,
                msg = "محصول از سبد خرید حذف شد."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PayDto model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Basket");

            var userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Unauthorized();

            var result = await _basketService.Pay(
                model.Mobile,
                model.Address,
                userId.Value);

            if (!result)
                return RedirectToAction("Basket");

            return RedirectToAction("Index", "Profile");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBasketCount()
        {
            var userId = GetCurrentUserId();

            if (!userId.HasValue)
                return Ok(0);

            var count =
                await _basketService.GetBasketItemCountAsync(userId.Value);

            return Ok(count);
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (int.TryParse(claim, out var userId))
                return userId;

            return null;
        }
    }
}