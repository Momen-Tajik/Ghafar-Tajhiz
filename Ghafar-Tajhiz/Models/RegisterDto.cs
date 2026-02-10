using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class RegisterDto
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "شماره تلفن الزامی است")]
        [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "فرمت شماره موبایل نامعتبر است")]
        [Display(Name = "شماره تلفن")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "کلمه عبور الزامی است")]
        [StringLength(100, MinimumLength =3, ErrorMessage = "کلمه عبور باید بین 6 تا 100 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "تکرار کلمه عبور الزامی است")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "کلمه عبور و تکرار آن مطابقت ندارند")]
        [Display(Name = "تکرار کلمه عبور")]
        public string ConfirmPassword { get; set; }
    }
}