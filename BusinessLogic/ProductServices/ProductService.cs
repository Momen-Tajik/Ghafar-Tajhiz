using BusinessLogic.FileUpload;
using DataAccess.Models;
using DataAccess.Repositories.ProductRepo;
using Microsoft.EntityFrameworkCore;
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
         public async Task<IEnumerable<Product>> GetProductsWithCategory(Expression<Func<Product, bool>> where=null) 
        {
            return await _productRepo.GetAll(where).Include(p=>p.Category).ToListAsync();
        }
        public async Task<Product> GetProductById(int id) 
        {
            return await _productRepo.GetById(id);
        }

        public async Task CreateProduct(ProductDto productDto)
        {
            var product = new Product()
            {
                ProductName=productDto.ProductName,
                ProductDescription=productDto.ProductDescription,
                Price=productDto.Price,
                StockQuantity=productDto.StockQuantity,
                CategoryId=productDto.CategoryId,
                IsAvailable=productDto.IsAvailable,
                Category=productDto.Category,
            };
            product.ImageUrl=await _fileUploadService.UploadFileAsync(productDto.ImageUrl);
            await _productRepo.Add(product);
        }

        public async Task UpdateProduct(Product product)
        {
            await _productRepo.Update(product);
        }

        public async Task DeleteProduct(int id)
        {
            var p = await _productRepo.GetById(id);
            await _productRepo.Delete(p);
        }
        public async Task DeleteProduct(Product product)
        {
            await _productRepo.Delete(product);
        }

    }
}
