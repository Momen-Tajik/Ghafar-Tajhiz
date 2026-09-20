using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class PayDto
    {
        [Required(ErrorMessage = "آدرس الزامی است")]
        [StringLength(
            500,
            MinimumLength = 10,
            ErrorMessage = "آدرس باید بین 10 تا 500 کاراکتر باشد")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "شماره موبایل الزامی است")]
        [RegularExpression(
            @"^09[0-9]{9}$",
            ErrorMessage = "فرمت شماره موبایل نامعتبر است")]
        public string Mobile { get; set; } = string.Empty;
    }
}