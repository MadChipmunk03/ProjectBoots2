using Microsoft.AspNetCore.Mvc;

namespace ProjectBoots2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
