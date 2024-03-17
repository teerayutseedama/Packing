using Microsoft.AspNetCore.Mvc;

namespace Packing.Controllers
{
    public class SummaryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
