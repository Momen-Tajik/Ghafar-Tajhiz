using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.BasketRepo
{
    public interface IBasketRepository
    {
        IQueryable<Basket> GetAll(Expression<Func<Basket, bool>> where = null);
        Task<Basket> GetById(int id);
        Task Add(Basket product);
        Task Update(Basket category);
        Task Delete(int id);
        Task Delete(Basket product);
    }
}
