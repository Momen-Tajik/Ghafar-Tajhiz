using DataAccess.Models;

namespace DataAccess.Repositories.BasketRepo
{
    public interface IBasketRepository
    {
        Task<Basket?> GetByIdAsync(int id);
        Task<IReadOnlyList<Basket>> GetAllAsync();
        Task AddAsync(Basket basket);
        void Update(Basket basket);
        void Delete(Basket basket);
    }
}