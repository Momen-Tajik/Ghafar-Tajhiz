using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz.Models
{
    public class PayDto
    {
        public string address { get; set; }

        [Required(ErrorMessage = "شماره تلفن الزامی است")]
        [RegularExpression(@"^09[0-9]{9}$", ErrorMessage = "فرمت شماره موبایل نامعتبر است")]
        [Display(Name = "شماره تلفن")]
        public string mobile { get; set; }
    }
}
