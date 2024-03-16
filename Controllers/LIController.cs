using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Packing.Controllers;
using Packing.Function;

namespace Loadin.Controllers
{
    public class LIController : Controller
    {
        private readonly ILogger<LIController> _logger;
        private readonly IStringLocalizer<LIController> _localizer;

        private readonly IConfigureInterface _configure;

        public LIController(IConfigureInterface configure, ILogger<LIController> logger, IStringLocalizer<LIController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
            _configure= configure;
        }
        public IActionResult Li()
        {
            return View();
        }
        public async Task<IActionResult> Configure() { 
            var data=await _configure.GetMaterialList();
            ViewBag.Data = data;
            ViewBag.BatchSlip = await _configure.Get_Batch_Slip();
            return View();
        }
        public IActionResult Loading(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public IActionResult Approval()
        {
            return View();
        }
        public IActionResult History()
        {
            return View();
        }
        public IActionResult Summary()
        {
            return View();
        }
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            //culture = "th-TH";
            //returnUrl = "~/";
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

            return LocalRedirect(returnUrl);
        }
    }
}
