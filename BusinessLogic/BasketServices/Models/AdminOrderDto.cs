using DataAccess.Enums;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BasketServices.Models
{
    public class AdminOrderDto
    {
        public int AdminOrderId { get; set; }



        public DateTime PaidDate { get; set; }

        public int UserId { get; set; }

        public string Address { get; set; }

        public string MobileNumber { get; set; }

        public BasketStatus Status { get; set; }

        public string UserName { get; set; }

        public List<string> items { get; set; } 
    }

}

