using Microsoft.AspNetCore.Mvc;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
