using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "نام دسته‌بندی الزامی است")]
        [Display(Name = "نام دسته‌بندی")]
        [StringLength(50, ErrorMessage = "نام دسته‌بندی نمی‌تواند بیش از 50 کاراکتر باشد")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "توضیحات")]
        [DataType(DataType.MultilineText)]
        public string? CategoryDescription { get; set; }

        public List<Product>? Products { get; set; } = new List<Product>();

        

    }
}
