using BusinessLogic.BasketServices;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    public class OrdersController : Controller
    {

        private readonly BasketService _basketService;

        public OrdersController(BasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<IActionResult> Index(string search = null, string sort = "paiddate")
        {
            var data = await _basketService.GetAdminBskets(search, sort);

            ViewBag.Search = search;
            ViewBag.Sort = sort;

            return View(data);
        }
    }
}
