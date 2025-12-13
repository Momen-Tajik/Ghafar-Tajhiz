using BusinessLogic.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz.Component
{
    public class HomeProducts:ViewComponent
    {
        private readonly ProductService _productService;
        public HomeProducts(ProductService productService)
        {
            _productService = productService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string productId)
        {
            var data = await _productService.GetProductsWithCategory();
            return View("/Views/Shared/Component/HomeProduct.cshtml",data);
        }
    }
}
