using Microsoft.AspNetCore.Mvc;

namespace BarCrudMVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
