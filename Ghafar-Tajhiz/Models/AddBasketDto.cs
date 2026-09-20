using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class AddBasketDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "شناسه محصول نامعتبر است")]
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "تعداد محصول باید بین 1 تا 100 باشد")]
        public int Qty { get; set; }
    }
}