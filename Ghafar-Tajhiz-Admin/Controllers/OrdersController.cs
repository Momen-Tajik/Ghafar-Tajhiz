using BusinessLogic.BasketServices;
using DataAccess.Enums;
using Ghafar_Tajhiz_Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly BasketService _basketService;

        public OrdersController(
            BasketService basketService)
        {
            _basketService = basketService;
        }

        // =========================================================
        // Orders
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            BasketStatus? status,
            string sort = "paiddate")
        {
            var data =
                await _basketService.GetAdminBskets(
                    search,
                    status,
                    sort);

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Sort = sort;

            return View(data);
        }

        // =========================================================
        // Approve Payment
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovePayment(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "شناسه سفارش نامعتبر است."
                });
            }

            var result =
                await _basketService.ApprovePayment(id);

            if (!result)
            {
                return BadRequest(new
                {
                    res = false,
                    msg =
                        "تأیید پرداخت انجام نشد. " +
                        "وضعیت سفارش یا موجودی کالا را بررسی کنید."
                });
            }

            return Ok(new
            {
                res = true,
                msg = "پرداخت با موفقیت تأیید شد."
            });
        }

        // =========================================================
        // Reject Payment
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPayment(
            [FromBody] RejectPaymentDto model)
        {
            if (model == null ||
                model.BasketId <= 0)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "شناسه سفارش نامعتبر است."
                });
            }

            var reason =
                model.Reason?.Trim();

            if (string.IsNullOrWhiteSpace(reason))
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "دلیل رد پرداخت الزامی است."
                });
            }

            if (reason.Length > 500)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "دلیل رد پرداخت نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد."
                });
            }

            var result =
                await _basketService.RejectPayment(
                    model.BasketId,
                    reason);

            if (!result)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "رد پرداخت انجام نشد. وضعیت سفارش را بررسی کنید."
                });
            }

            return Ok(new
            {
                res = true,
                msg = "پرداخت رد شد."
            });
        }

        // =========================================================
        // Ship Order
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShipOrder(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "شناسه سفارش نامعتبر است."
                });
            }

            var result =
                await _basketService.ShipOrder(id);

            if (!result)
            {
                return BadRequest(new
                {
                    res = false,
                    msg =
                        "ارسال سفارش انجام نشد. " +
                        "سفارش باید ابتدا پرداخت تأییدشده داشته باشد."
                });
            }

            return Ok(new
            {
                res = true,
                msg = "سفارش با موفقیت ارسال شد."
            });
        }

        // =========================================================
        // Details
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            if (id <= 0)
                return NotFound();

            var order =
                await _basketService.GetAdminOrderDetail(id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}