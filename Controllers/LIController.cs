using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Packing.Controllers;
using Packing.Function;
using Packing.Models;
using Packing.vmsPackingDB;

namespace Loadin.Controllers
{
    public class LIController : Controller
    {
        private readonly ILogger<LIController> _logger;
        private readonly IStringLocalizer<LIController> _localizer;

        private readonly IConfigureInterface _configure;
        private readonly IMasterDataInterface _masterData;
        private readonly ILiInterface _liInterface;

        public LIController(ILiInterface liInterface, IConfigureInterface configure, ILogger<LIController> logger, IStringLocalizer<LIController> localizer, IMasterDataInterface masterData)
        {
            _logger = logger;
            _localizer = localizer;
            _configure = configure;
            _masterData = masterData;
            _liInterface= liInterface;
        }
        
        public async Task<IActionResult> Li()
        {
            ViewBag.MasterData = await _masterData.GetMasterDataView();
            return View();
        }
        public async Task<IActionResult> Configure() { 
            var data=await _configure.GetMaterialList();
            ViewBag.Data = data;
            ViewBag.BatchSlip = await _configure.Get_Batch_Slip();
            ViewBag.WorkSlift=await _configure.Get_Work_Shifts();
            return View();
        }
        public async Task<IActionResult> Loading(string id)
        {
            ViewBag.Id = id;
            ViewBag.MasterData = await _masterData.GetMasterDataView();
            return View();
        }
        public async Task<IActionResult> Approval()
        {
            ViewBag.MasterData = await _masterData.GetMasterDataView();
            return View();
        }
        public async Task<IActionResult> History()
        {
            ViewBag.MasterData = await _masterData.GetMasterDataView();
            return View();
        }
        public async Task<IActionResult> Summary()
        {
            ViewBag.MasterData=await _masterData.GetMasterDataView();
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            ViewBag.MasterData = await _masterData.GetMasterDataView();
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


        public async Task<IActionResult> GetMaterial(string MaterialCode)
        {
            return Ok(await _liInterface.GetMaterial(MaterialCode));
        }
        
    }
}
