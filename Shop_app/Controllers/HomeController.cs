using Microsoft.AspNetCore.Mvc;

namespace Shop_app.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
