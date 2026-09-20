using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "نام الزامی است")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "نام باید بین 2 تا 100 کاراکتر باشد")]
        [Display(Name = "نام")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "شماره تلفن الزامی است")]
        [RegularExpression(
            @"^09[0-9]{9}$",
            ErrorMessage = "فرمت شماره موبایل نامعتبر است")]
        [Display(Name = "شماره تلفن")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "کلمه عبور الزامی است")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "کلمه عبور باید بین 6 تا 100 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تکرار کلمه عبور الزامی است")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Password),
            ErrorMessage = "کلمه عبور و تکرار آن مطابقت ندارند")]
        [Display(Name = "تکرار کلمه عبور")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}