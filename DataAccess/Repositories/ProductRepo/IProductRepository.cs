using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        IQueryable<Product> GetAll(Expression<Func<Product,bool>> where= null);
        Task<Product> GetById(int id);
        Task Add(Product product);
        Task Update(Product category);
        Task Delete(int id);
        Task Delete(Product product);
    }
}
