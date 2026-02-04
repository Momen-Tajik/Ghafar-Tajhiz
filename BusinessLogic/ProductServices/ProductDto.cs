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

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ProductDescription { get; set; }


        public decimal Price { get; set; }

        public int StockQuantity { get; set; }


        public IFormFile? ImageUrl {  get; set; }

        public string? ImageName { get; set; }


        public bool IsAvailable { get; set; } = true;

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
