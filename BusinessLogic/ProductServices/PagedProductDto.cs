using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ProductServices
{
    public class PagedProductDto
    {
        public int TotalPage { get; set; }
        public int Page { get; set; }

        public int TotalCount { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
