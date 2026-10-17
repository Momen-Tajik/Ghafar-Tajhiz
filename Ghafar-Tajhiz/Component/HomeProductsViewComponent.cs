using BusinessLogic.ProductServices;
using Ghafar_Tajhiz.ViewModels;
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
            var products =
                await _productService.GetProductsWithCategory();

            var categories = products
                .GroupBy(p => new
                {
                    p.CategoryId,
                    CategoryName =
                        p.Category?.CategoryName ?? "سایر محصولات"
                })
                .OrderBy(g => g.Key.CategoryName)
                .Select(g => new HomeProductCategoryViewModel
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,

                    Products = g.Select(p => new HomeProductItemViewModel
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price
                    }).ToList()
                })
                .ToList();

            var viewModel = new HomeProductsViewModel
            {
                Categories = categories
            };

            return View(
                "/Views/Shared/Component/HomeProduct.cshtml",
                viewModel);
        }
    }
}