using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Packing.Function;
using Packing.Views.DataView;

namespace Packing.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IHistoryInterface _historyInterface;
        public HistoryController(IHistoryInterface historyInterface)
        {
            _historyInterface = historyInterface;
        }
        // GET: HistoryController
        public ActionResult Index()
        {
            return View();
        }

        // GET: HistoryController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HistoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HistoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HistoryController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HistoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HistoryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

  
        [HttpPost]
     
        public async Task<IActionResult> GetHistoryDataViews(GetHistoryData data)
        {
            return Ok(await _historyInterface.GetHistoryDataViews(data));
        }

        public async Task<IActionResult> DownloadHistoryExcel(GetHistoryData data)
        {
            var dataList = await _historyInterface.GetHistoryDataViews(data);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("History");
                // สร้าง Header
                worksheet.Cell(1, 1).Value = "BatchNo";
                worksheet.Cell(1, 2).Value = "Plant";
                worksheet.Cell(1, 3).Value = "Line";
                worksheet.Cell(1, 4).Value = "MaterialCode";
                worksheet.Cell(1, 5).Value = "MaterialName";
                worksheet.Cell(1, 6).Value = "Package";
                worksheet.Cell(1, 7).Value = "MFGDate";
                worksheet.Cell(1, 8).Value = "Shift";
                worksheet.Cell(1, 9).Value = "RunNo";
                worksheet.Cell(1, 10).Value = "Qty";
                worksheet.Cell(1, 11).Value = "UOM";
                worksheet.Cell(1, 12).Value = "Stataus";
        // เขียนข้อมูลจาก List ลงใน Excel
        int row = 2;
                foreach (var item in dataList)
                {
                    worksheet.Cell(row, 1).Value = item.BatchNo;
                    worksheet.Cell(row, 2).Value = item.Plant;
                    worksheet.Cell(row, 3).Value = item.Line;
                    worksheet.Cell(row, 4).Value = item.MaterialCode;
                    worksheet.Cell(row, 5).Value = item.MaterialName;
                    worksheet.Cell(row, 6).Value = item.Package;
                    worksheet.Cell(row, 7).Value = item.MFGDate;
                    worksheet.Cell(row, 8).Value = item.Shift;
                    worksheet.Cell(row, 9).Value = item.RunNo;
                    worksheet.Cell(row, 10).Value = item.Qty;
                    worksheet.Cell(row, 11).Value = item.UOM;
                    worksheet.Cell(row, 12).Value = item.Stataus;
                    row++;
                }

                // สร้าง MemoryStream เพื่อเก็บข้อมูล Excel
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // ส่งไฟล์ Excel กลับเป็น response
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }
    }

}
