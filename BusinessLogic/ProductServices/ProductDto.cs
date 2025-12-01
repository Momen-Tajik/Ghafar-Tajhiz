using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.ProductServices
{
    public class ProductDto
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "نام محصول الزامی است")]
        [Display(Name = "نام محصول")]
        [StringLength(100, ErrorMessage = "نام محصول نمی‌تواند بیش از 100 کاراکتر باشد")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "توضیحات")]
        [DataType(DataType.MultilineText)]
        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = "قیمت محصول الزامی است")]
        [Display(Name = "قیمت")]
        [Range(0, double.MaxValue, ErrorMessage = "قیمت باید عددی مثبت باشد")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "تعداد موجودی")]
        [Range(0, int.MaxValue, ErrorMessage = "تعداد موجودی نمی‌تواند منفی باشد")]
        public int StockQuantity { get; set; }

        [Display(Name = "آدرس تصویر")]
        //[Url(ErrorMessage = "آدرس تصویر باید یک URL معتبر باشد")]
        public IFormFile? ImageUrl {  get; set; }

        public string? ImageName { get; set; }

        [Display(Name = "وضعیت موجودی")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "تاریخ ایجاد")]
        [DataType(DataType.DateTime)]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        // دسته‌بندی
        [Required(ErrorMessage = "دسته‌بندی الزامی است")]
        public int CategoryId { get; set; }

        [Display(Name = "دسته‌بندی")]
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
