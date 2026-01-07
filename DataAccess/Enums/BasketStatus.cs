using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Enums
{
    public enum BasketStatus
    {
        [Display(Name = "در انتظار پرداخت")]
        Pending = 0,

        [Display(Name = "پرداخت شده")]
        Paid = 1,

        [Display(Name = "ارسال شده")]
        Shipped = 2,

        [Display(Name = "لغو شده")]
        Cancelled = 3
    }
}
