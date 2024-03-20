using ClosedXML.Excel;
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
        public async Task<IActionResult> GetSummaryList( GetSummaryDataView data)
        {
            return Ok(await _summary.GetSummaryList(data));
        }
        [HttpPost]
        public async Task<IActionResult> DownloadEmployeesExcel(GetSummaryDataView data)
        {
            var dataList = await _summary.GetSummaryList(data);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Summary");
                // สร้าง Header
                worksheet.Cell(1, 1).Value = "MaterialCode";
                worksheet.Cell(1, 2).Value = "MaterialGroup";
                worksheet.Cell(1, 3).Value = "MaterialName";
                worksheet.Cell(1, 4).Value = "Package";
                worksheet.Cell(1, 5).Value = "MFGDate";
                worksheet.Cell(1, 6).Value = "ExpireDate";
                worksheet.Cell(1, 7).Value = "Qty";
                worksheet.Cell(1, 8).Value = "UOM";

                // เขียนข้อมูลจาก List ลงใน Excel
                int row = 2;
                foreach (var item in dataList)
                {
                    worksheet.Cell(row, 1).Value = item.MaterialCode;
                    worksheet.Cell(row, 2).Value = item.MaterialGroup;
                    worksheet.Cell(row, 3).Value = item.MaterialName;
                    worksheet.Cell(row, 4).Value = item.Package;
                    worksheet.Cell(row, 1).Value = item.MFGDate;
                    worksheet.Cell(row, 2).Value = item.ExpireDate;
                    worksheet.Cell(row, 3).Value = item.Qty;
                    worksheet.Cell(row, 4).Value = item.UOM;
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
