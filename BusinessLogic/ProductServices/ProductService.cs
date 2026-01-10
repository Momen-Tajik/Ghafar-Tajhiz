using BusinessLogic.FileUpload;
using DataAccess.Models;
using DataAccess.Repositories.ProductRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ProductServices
{
    public class ProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IFileUploadService _fileUploadService;

        public ProductService(IProductRepository productRepo, IFileUploadService fileUploadService)
        {
            _productRepo = productRepo;
            _fileUploadService = fileUploadService;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _productRepo.GetAll().ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsWithCategory(Expression<Func<Product, bool>> where = null)
        {
            return await _productRepo.GetAll(where).Include(p => p.Category).OrderByDescending(a => a.IsAvailable == true).ToListAsync();
        }
        public async Task<Product> GetProductById(int id)
        {
            return await _productRepo.GetById(id);
        }

        public async Task CreateProduct(ProductDto productDto)
        {
            var product = new Product()
            {
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                CategoryId = productDto.CategoryId,
                IsAvailable = productDto.IsAvailable,
                Category = productDto.Category,
            };
            product.ImageUrl = await _fileUploadService.UploadFileAsync(productDto.ImageUrl);
            await _productRepo.Add(product);
        }

        public async Task UpdateProduct(ProductDto productDto)
        {

            var product = await _productRepo.GetById(productDto.ProductId);
            product.ProductName = productDto.ProductName;
            product.ProductDescription = productDto.ProductDescription;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.CategoryId = productDto.CategoryId;
            product.IsAvailable = productDto.IsAvailable;
            product.Category = productDto.Category;

            if (productDto.ImageUrl != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                    _fileUploadService.DeleteFile(product.ImageUrl);

                product.ImageUrl = await _fileUploadService.UploadFileAsync(productDto.ImageUrl);
            }

            if (productDto.ImageUrl != null)
            {
                product.ImageUrl = await _fileUploadService.UploadFileAsync(productDto.ImageUrl);
            }

            await _productRepo.Update(product);
        }

        public async Task DeleteProduct(int id)
        {
            var p = await _productRepo.GetById(id);

            if (!string.IsNullOrEmpty(p.ImageUrl))
                _fileUploadService.DeleteFile(p.ImageUrl);

            await _productRepo.Delete(p);
        }
        public async Task DeleteProduct(Product product)
        {
            await _productRepo.Delete(product);
        }

        public async Task<ProductDto> GetProductDtoById(int id)
        {
            var product = await _productRepo.GetById(id);
            var productDto = new ProductDto()
            {
                CategoryId = product.CategoryId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ProductId = product.ProductId,
                ImageName = product.ImageUrl,
                IsAvailable = product.IsAvailable,
            };
            return productDto;
        }

        public async Task<PagedProductDto> GetProductPagination(int page, int pageSize, string? search, string sort = "newest")
        {
            
            var products = _productRepo.GetAll();

            //Filtering
            switch (sort.ToLower())
            {
                case "bestselling":
                    // ترکیبی از تاریخ و موجودی
                    products = products.OrderByDescending(p => p.CreateDate)
                                       .ThenByDescending(p => p.StockQuantity);
                    break;

                case "newest":
                    products = products.OrderByDescending(p => p.CreateDate);
                    break;

                case "highestprice":
                    products = products.OrderByDescending(p => p.Price);
                    break;

                case "lowestprice":
                    products = products.OrderBy(p => p.Price);
                    break;

                default:
                    products = products.OrderByDescending(p => p.CreateDate);
                    break;
            }

            // Searching
            if (!search.IsNullOrEmpty())
            {
                products = products.Where(p =>
                    p.ProductName.Contains(search) ||
                    p.ProductDescription.Contains(search) ||
                    p.Category.CategoryName.Contains(search) ||
                    p.Price.ToString().Contains(search));
            }

            // Pagination content
            int totalCount = await products.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Pagination
            var pagedProducts = await products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Convert to DTO
            var productDto = pagedProducts.Select(p => new ProductDto()
            {
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ProductId = p.ProductId,
                ImageName = p.ImageUrl,
                IsAvailable = p.IsAvailable,
                CreateDate = p.CreateDate
            }).ToList();

            // Return the result
            return new PagedProductDto()
            {
                Page = page,
                TotalPage = totalPages,
                Products = productDto,
                TotalCount = totalCount
            };
        }

    }
}
