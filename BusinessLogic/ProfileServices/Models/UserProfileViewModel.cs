using DataAccess.Enums;
using DataAccess.Models;

namespace BusinessLogic.ProfileServices.Models
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; } ="کاربر";

        public string? MobileNumber { get; set; }
        public string? Address { get; set; }

        public List<Basket> Orders { get; set; } = new();

        // Filtering & Searching
        public string? Search { get; set; }
        public BasketStatus? Status { get; set; }
        public string Sort { get; set; } = "paiddate";
    }
}
