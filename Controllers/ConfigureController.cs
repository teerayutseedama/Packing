using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Packing.Function;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Packing.Controllers
{
    public class ConfigureController : Controller
    {
     
        private readonly IConfigureInterface _configure;
        private readonly ISummaryInterface _summary;
        public ConfigureController(IConfigureInterface configure, ISummaryInterface summary)
        {
            _configure = configure;
            _summary = summary;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SaveStickerFormat(UpdateBatchShiftDatView data)
        {
            return Ok(await _configure.UpdateBatch(data));
        }
        
        public async Task<IActionResult> SaveShift(List<SaveShipDataView> DataSave)
        {
            return Ok(await _configure.SaveWorkShift(DataSave));
        }

        public async Task<IActionResult> GetSummaryList(GetSummaryDataView data)
        {
            return Ok(await _summary.GetSummaryList(data));
        }
    }
}

