using BusinessLogic.FileUpload;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.ProductServices
{
    public class ProductService
    {
        private readonly GhafarTajhizShopDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            GhafarTajhizShopDbContext context,
            IFileUploadService fileUploadService,
            ILogger<ProductService> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Product>> GetProducts()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsWithCategory()
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .OrderByDescending(p => p.IsAvailable)
                .ThenByDescending(p => p.CreateDate)
                .ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.Comments
                    .OrderByDescending(c => c.Created))
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task CreateProduct(ProductDto productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                CategoryId = productDto.CategoryId,
                IsAvailable = productDto.IsAvailable
            };

            if (productDto.ImageUrl != null)
            {
                product.ImageUrl =
                    await _fileUploadService.UploadFileAsync(productDto.ImageUrl);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateProduct(ProductDto productDto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == productDto.ProductId);

            if (product == null)
                return false;

            product.ProductName = productDto.ProductName;
            product.ProductDescription = productDto.ProductDescription;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.CategoryId = productDto.CategoryId;
            product.IsAvailable = productDto.IsAvailable;

            if (product.StockQuantity == 0)
            {
                product.IsAvailable = false;
            }

            if (productDto.ImageUrl != null)
            {
                var oldImage = product.ImageUrl;

                product.ImageUrl =
                    await _fileUploadService.UploadFileAsync(productDto.ImageUrl);

                if (!string.IsNullOrWhiteSpace(oldImage))
                {
                    _fileUploadService.DeleteFile(oldImage);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ProductDeleteResult> DeleteProduct(int id)
        {
            if (id <= 0)
                return ProductDeleteResult.NotFound;

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return ProductDeleteResult.NotFound;

            /*
             * A product that has already been used in an order
             * should not be physically deleted.
             *
             * Otherwise BasketItems.ProductId would point to
             * a non-existing Product.
             */
            var hasOrders = await _context.BasketItems
                .AnyAsync(x => x.ProductId == id);

            if (hasOrders)
            {
                return ProductDeleteResult.HasOrders;
            }

            var imageName = product.ImageUrl;

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to delete product {ProductId}",
                    id);

                return ProductDeleteResult.DatabaseError;
            }

            /*
             * Delete the physical image only after the database
             * deletion has succeeded.
             */
            if (!string.IsNullOrWhiteSpace(imageName))
            {
                try
                {
                    _fileUploadService.DeleteFile(imageName);
                }
                catch (Exception ex)
                {
                    /*
                     * Product has already been deleted from DB.
                     * Therefore an image deletion failure should
                     * not make the whole operation look failed.
                     */
                    _logger.LogWarning(
                        ex,
                        "Product {ProductId} was deleted, but its image {ImageName} could not be deleted.",
                        id,
                        imageName);
                }
            }

            return ProductDeleteResult.Success;
        }

        public async Task<ProductDto?> GetProductDtoById(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageName = p.ImageUrl,
                    IsAvailable = p.IsAvailable,
                    CategoryId = p.CategoryId,
                    CreateDate = p.CreateDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PagedProductDto> GetProductPagination(
            int page,
            int pageSize,
            string? search,
            string sort = "newest")
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 12;

            if (pageSize > 100)
                pageSize = 100;

            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();

                query = query.Where(p =>
                    p.ProductName.Contains(term) ||
                    (p.ProductDescription != null &&
                     p.ProductDescription.Contains(term)) ||
                    p.Category!.CategoryName.Contains(term));
            }

            query = sort.Trim().ToLowerInvariant() switch
            {
                "highestprice" =>
                    query.OrderByDescending(p => p.Price),

                "lowestprice" =>
                    query.OrderBy(p => p.Price),

                "bestselling" =>
                    query.OrderByDescending(p => p.StockQuantity),

                _ =>
                    query.OrderByDescending(p => p.CreateDate)
            };

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageName = p.ImageUrl,
                    IsAvailable = p.IsAvailable,
                    CategoryId = p.CategoryId,
                    CreateDate = p.CreateDate
                })
                .ToListAsync();

            var totalPages =
                (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedProductDto
            {
                Page = page,
                TotalPage = totalPages,
                TotalCount = totalCount,
                Products = products
            };
        }
    }
}