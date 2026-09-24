using BusinessLogic.BasketServices;
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

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string sort = "paiddate")
        {
            var data =
                await _basketService.GetAdminBskets(
                    search,
                    sort);

            ViewBag.Search = search;
            ViewBag.Sort = sort;

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStateCommand(
            [FromBody] StatusDto model)
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

            var result =
                await _basketService.SetState(
                    model.BasketId,
                    model.Status);

            if (!result)
            {
                return BadRequest(new
                {
                    res = false,
                    msg = "تغییر وضعیت سفارش انجام نشد."
                });
            }

            return Ok(new
            {
                res = true,
                msg = "وضعیت سفارش با موفقیت تغییر کرد."
            });
        }
    }
}