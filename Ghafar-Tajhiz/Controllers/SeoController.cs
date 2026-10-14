using BusinessLogic.ProductServices;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;

namespace Ghafar_Tajhiz.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public sealed class SeoController : Controller
{
    private readonly ProductService _productService;
    private readonly IConfiguration _configuration;

    public SeoController(ProductService productService, IConfiguration configuration)
    {
        _productService = productService;
        _configuration = configuration;
    }

    private string GetSiteUrl()
    {
        // در محیط Production حتماً Seo:SiteUrl را روی دامنه canonical واقعی تنظیم کنید.
        return (_configuration["Seo:SiteUrl"] ?? $"{Request.Scheme}://{Request.Host}").TrimEnd('/');
    }

    [HttpGet("/robots.txt")]
    [Produces("text/plain")]
    public IActionResult Robots()
    {
        var sitemapUrl = $"{GetSiteUrl()}/sitemap.xml";
        var content = $"User-agent: *\nAllow: /\nSitemap: {sitemapUrl}\n";
        return Content(content, "text/plain", Encoding.UTF8);
    }

    [HttpGet("/sitemap.xml")]
    [Produces("application/xml")]
    public async Task<IActionResult> Sitemap()
    {
        var siteUrl = GetSiteUrl();
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Url.Action("Index", "Home") ?? "/",
            Url.Action("AboutUs", "Home") ?? "/Home/AboutUs",
            Url.Action("ProductList", "Product") ?? "/Product/ProductList"
        };

        // This relies on GetProductsWithCategory returning all publicly visible products.
        // If it returns only a homepage subset, replace it with an all-public-products method.
        var products = await _productService.GetProductsWithCategory();
        foreach (var product in products)
        {
            var path = Url.Action("Index", "Product", new { id = product.ProductId });
            if (!string.IsNullOrWhiteSpace(path))
                paths.Add(path);
        }

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urlElements = paths.Select(path =>
        {
            var absoluteUrl = Uri.TryCreate(path, UriKind.Absolute, out var absolute)
                ? absolute.ToString()
                : $"{siteUrl}/{path.TrimStart('/')}";

            return new XElement(ns + "url", new XElement(ns + "loc", absoluteUrl));
        });

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(ns + "urlset", urlElements));

        return Content(document.ToString(SaveOptions.DisableFormatting), "application/xml", Encoding.UTF8);
    }
}
