using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Packing.Controllers;
using Packing.Function;
using Packing.Models;
using Packing.Views.DataView;
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
        
        public async Task<IActionResult> Li(string id)
        {
           
            ViewBag.Id = id;
            ViewBag.MasterData = await _masterData.GetMasterDataView();
            return View();
        }
        public async Task<IActionResult> LoadQrData(GetQrCodeData data)
        {
            var list = await _liInterface.GetQrCodeData(data);
            HttpContext.Session.SetString("QrData", JsonConvert.SerializeObject(list));
            return Ok(list.Count()>0);
        }

        public async Task<IActionResult> QrCode()
        {
        var data= JsonConvert.DeserializeObject<IEnumerable<QrCodeData>>(HttpContext.Session.GetString("QrData"));
            ViewBag.QrData = data;
        ViewBag.Config = await _liInterface.GetConfig();
            return View();
        }
        public async Task<IActionResult> LoadLiData(string id)
        {
            LiDataView data = new LiDataView();
            if (id != null)
            {
                var bat = id.Split(' ');
                if (bat.Length > 0)
                {
                    data = await _liInterface.LoadLiDataView(bat[0], Convert.ToInt32(bat[1]));
                }
            }
            return Ok(data);
        }
        public async Task<IActionResult> Configure() { 
            var data=await _configure.GetMaterialList();
            ViewBag.Data = data;
            ViewBag.BatchSlip = await _configure.Get_Batch_Slip();
            ViewBag.WorkSlift=await _configure.Get_Work_Shifts();
            ViewBag.MasterData = await _masterData.GetMasterDataView();
            return View();
        }
        public async Task<IActionResult> Loading()
        {
         
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
        public async Task<IActionResult> CheckBatchNo(string batchNo,int SubBatch)
        {
            return Ok(await _liInterface.CheckBatchNo(batchNo, SubBatch));
        }
        public async Task<IActionResult> CheckShift(DateTime date)
        {
            return Ok(await _liInterface.CheckShift(date));
        }
        public async Task<IActionResult> SaveLIData(SaveLiData data)
        {
            return Ok(await _liInterface.SaveLIData(data));
        }
        public async Task<IActionResult> CloseJob(LiCloseJob data)
        {
            return Ok(await _liInterface.CloseJob(data));
        }
        



    }
}
