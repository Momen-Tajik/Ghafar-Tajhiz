using DataAccess.Models;
using DataAccess.Repositories.CategoryRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CategoryServices
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepo= categoryRepository;
        }

        public async Task CreatCategory(Category category) 
        {
            await _categoryRepo.Add(category);
        }
        public async Task<IEnumerable<Category>> GetCategories() 
        {
            return await _categoryRepo.GetAll();
        }
        public async Task<Category> GetCategoryById(int id)
        {
            return await _categoryRepo.GetById(id);
        }
        public async Task EditCategory(Category category)
        {
             await _categoryRepo.Update(category);
        }
        public async Task DeleteCategory(int id)
        {
            await _categoryRepo.Delete(id);
        }
        public async Task DeleteCategory(Category category)
        {
            await _categoryRepo.Delete(category);
        }
        
    }
}
