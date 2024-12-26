using Microsoft.AspNetCore.Mvc;

namespace TurboReserve.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
