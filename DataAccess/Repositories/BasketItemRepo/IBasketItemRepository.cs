using DataAccess.Models;

namespace DataAccess.Repositories.BasketItemRepo
{
    public interface IBasketItemRepository
    {
        Task<BasketItem?> GetByIdAsync(int id);
        Task<IReadOnlyList<BasketItem>> GetAllAsync();
        Task AddAsync(BasketItem basketItem);
        void Update(BasketItem basketItem);
        void Delete(BasketItem basketItem);
    }
}