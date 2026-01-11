using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "کلمه عبور باید بین 6 تا 100 کاراکتر باشد")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "شماره تلفن الزامی است")]
        [Phone(ErrorMessage = "لطفاً شماره تلفن معتبر وارد کنید")]
        [Display(Name = "شماره تلفن")]
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
