using BusinessLogic.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz.Component
{
    public class HomeProductsViewComponent : ViewComponent
    {
        private readonly ProductService _productService;

        public HomeProductsViewComponent(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _productService.GetProductsWithCategory();

            return View(
                "/Views/Shared/Component/HomeProduct.cshtml",
                products);
        }
    }
}