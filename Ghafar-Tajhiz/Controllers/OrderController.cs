
using BusinessLogic.BasketItemServices;
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
        private readonly BasketItemService _basketItemService;
        public OrderController(BasketService basketService, BasketItemService basketItemService)
        {
            _basketService = basketService;
            _basketItemService = basketItemService;
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
                return RedirectToAction("Login", "Account");
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


        [HttpPost]
        public async Task<IActionResult> RemoveBasketItem([FromBody] RemoveBasketItemDto model)
        {
            var res = await _basketItemService.RemoveBasketItem(model.BasketItemId);
            return Ok(new { res = true });

        }

        [Authorize]
        public async Task<IActionResult> Pay(PayDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _basketService.Pay(model.mobile,model.address,Convert.ToInt32(userId));

            return RedirectToAction("Index","Profile");
        }
    }
}
