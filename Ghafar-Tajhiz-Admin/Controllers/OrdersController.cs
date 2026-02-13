using BusinessLogic.BasketServices;
using BusinessLogic.ProductServices;
using Ghafar_Tajhiz_Admin.Models;
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


        [HttpPost]
        public async Task<IActionResult> SetStateCommand([FromBody] StatusDto model)
        {


            await _basketService.SetState(model.BasketItemId, model.Status);

            return Ok(new { res = true, msg = "محصول با موفقیت اضافه شد" });

        }
    }
}
