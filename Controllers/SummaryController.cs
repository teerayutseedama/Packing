using Microsoft.AspNetCore.Mvc;
using Packing.Function;
using Packing.Views.DataView;

namespace Packing.Controllers
{
    public class SummaryController : Controller
    {
        private readonly ISummaryInterface _summary;
        public SummaryController(ISummaryInterface summary) {
            _summary=summary;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetSummaryList([FromBody] GetSummaryDataView data)
        {
            return Ok(await _summary.GetSummaryList(data));
        }
    }
}
