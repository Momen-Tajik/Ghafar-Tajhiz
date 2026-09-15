using DataAccess.Models;

namespace DataAccess.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}