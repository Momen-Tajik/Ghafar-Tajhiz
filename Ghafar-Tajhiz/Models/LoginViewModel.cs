using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class LoginViewModel
    {



        [Required(ErrorMessage = "شماره تلفن الزامی است")]
        [Display(Name = "شماره تلفن")]
        [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "فرمت شماره موبایل نامعتبر است")]

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
