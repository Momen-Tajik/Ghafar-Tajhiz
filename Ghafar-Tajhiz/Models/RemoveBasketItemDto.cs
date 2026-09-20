using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class RemoveBasketItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "شناسه سبد خرید نامعتبر است")]
        public int BasketItemId { get; set; }
    }
}