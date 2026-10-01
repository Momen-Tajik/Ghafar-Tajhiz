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
        PendingPayment = 0,

        [Display(Name = "در انتظار بررسی پرداخت")]
        AwaitingPaymentVerification = 1,

        [Display(Name = "پرداخت تأیید شد")]
        PaymentApproved = 2,

        [Display(Name = "پرداخت رد شد")]
        PaymentRejected = 3,

        [Display(Name = "ارسال شده")]
        Shipped = 4,

        [Display(Name = "لغو شده")]
        Cancelled = 5
    }
}
