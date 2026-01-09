using DataAccess.Repositories.BasketItemRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BasketItemServices
{
    public class BasketItemService
    {
        private readonly IBasketItemRepository _basketItemRepository;

        public BasketItemService(IBasketItemRepository basketItemRepository)
        {
            _basketItemRepository = basketItemRepository;
        }

       public async Task<bool> RemoveBasketItem(int id)
        {
            var basketItem = await _basketItemRepository.GetAll(a=>a.BasketItemId==id).FirstOrDefaultAsync();
            await _basketItemRepository.Delete(basketItem);
            return true;
        }
    }
}
