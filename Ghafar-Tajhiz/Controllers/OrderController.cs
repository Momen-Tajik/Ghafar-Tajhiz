
using BusinessLogic.BasketServices;
using Ghafar_Tajhiz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ghafar_Tajhiz.Controllers
{
    public class OrderController : Controller
    {
        private readonly BasketService _basketService;
        public OrderController(BasketService basketService)
        {
            _basketService = basketService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket([FromBody] AddBasketDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) == null)
            {
                return Ok(new { res = false, msg = "yor not login" });
            }

            var result = await _basketService.AddToBasket(
                model.productId,
                model.qty,
                Convert.ToInt32( userId)
            );



            return Ok(new { res = true });
           
        }


        public async Task<IActionResult> Basket()
        {
            var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);

            var data= await _basketService.GetUserBasket(Convert.ToInt32(userId));

            return View(data);
        }

    }
}
