using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class AddCommentDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "شناسه محصول نامعتبر است")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "متن نظر الزامی است")]
        [StringLength(
            500,
            MinimumLength = 2,
            ErrorMessage = "متن نظر باید بین 2 تا 500 کاراکتر باشد")]
        public string Text { get; set; } = string.Empty;
    }
}