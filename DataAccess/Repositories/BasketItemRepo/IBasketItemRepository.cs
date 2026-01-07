using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.BasketItemRepo
{
    public interface IBasketItemRepository
    {
        IQueryable<BasketItem> GetAll(Expression<Func<BasketItem, bool>> where = null);
        Task<BasketItem> GetById(int id);
        Task Add(BasketItem product);
        Task Update(BasketItem category);
        Task Delete(int id);
        Task Delete(BasketItem product);
    }
}
