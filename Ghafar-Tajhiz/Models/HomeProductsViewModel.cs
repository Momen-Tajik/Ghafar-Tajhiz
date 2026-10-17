namespace Ghafar_Tajhiz.ViewModels
{
    public class HomeProductsViewModel
    {
        public IReadOnlyList<HomeProductCategoryViewModel> Categories
        { get; init; } = Array.Empty<HomeProductCategoryViewModel>();
    }

    public class HomeProductCategoryViewModel
    {
        public int CategoryId { get; init; }

        public string CategoryName { get; init; } = string.Empty;

        public IReadOnlyList<HomeProductItemViewModel> Products
        { get; init; } = Array.Empty<HomeProductItemViewModel>();
    }

    public class HomeProductItemViewModel
    {
        public int ProductId { get; init; }

        public string ProductName { get; init; } = string.Empty;

        public string? ImageUrl { get; init; }

        public decimal Price { get; init; }
    }
}