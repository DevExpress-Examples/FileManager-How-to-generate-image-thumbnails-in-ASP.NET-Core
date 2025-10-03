using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
