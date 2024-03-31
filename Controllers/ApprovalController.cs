using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Packing.Function;
using Packing.Views.DataView;
using System.IO;
using System.IO.Packaging;

namespace Packing.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IApprovalInterface _approval;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ApprovalController(IApprovalInterface approval, IWebHostEnvironment webHostEnvironment)
        {
            _approval = approval;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        
             public async Task<IActionResult> GetApprovalDataViews(GetApprovalData data)
        {
            return Ok(await _approval.GetApprovalDataViews(data));
        }
        public async Task<IActionResult> SaveApprovalData(List<SaveApprovalData> data)
        {
            return Ok(await _approval.SaveApprovalData(data));
        }
        
        public async Task<IActionResult> DownloadApprovalExcel(GetApprovalData data)
        {
            try
            {
                var dataList = await _approval.GetApprovalDataViews(data);
                string path = _webHostEnvironment.WebRootPath + "/ExcelApproval/Approval.xlsx";
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Approval");
                    // สร้าง Header
                    worksheet.Cell(1, 1).Value = "BatchNo";
                    worksheet.Cell(1, 2).Value = "Plant";
                    worksheet.Cell(1, 3).Value = "MaterialName";
                    worksheet.Cell(1, 4).Value = "Line";
                    worksheet.Cell(1, 5).Value = "Package";
                    worksheet.Cell(1, 6).Value = "RunNo";
                    worksheet.Cell(1, 7).Value = "Qty";
                    worksheet.Cell(1, 8).Value = "UOM";
                    worksheet.Cell(1, 9).Value = "MFGDate";
                    worksheet.Cell(1, 10).Value = "ExpireDate";
                    worksheet.Cell(1, 11).Value = "Stataus";

                    // เขียนข้อมูลจาก List ลงใน Excel
                    int row = 2;
                    foreach (var item in dataList)
                    {
                        worksheet.Cell(row, 1).Value = item.BatchNo;
                        worksheet.Cell(row, 2).Value = item.Plant;
                        worksheet.Cell(row, 3).Value = item.MaterialName;
                        worksheet.Cell(row, 4).Value = item.Line;
                        worksheet.Cell(row, 5).Value = item.Package;
                        worksheet.Cell(row, 6).Value = item.RunNo;
                        worksheet.Cell(row, 7).Value = item.Qty;
                        worksheet.Cell(row, 8).Value = item.UOM;
                        worksheet.Cell(row, 9).Value = item.MFGDate;
                        worksheet.Cell(row, 10).Value = item.ExpireDate;
                        worksheet.Cell(row, 11).Value = item.Status;
                        row++;
                    }
                    workbook.SaveAs(path);
                    return Ok(true);
                    // สร้าง MemoryStream เพื่อเก็บข้อมูล Excel
                    //using (var stream = new MemoryStream())
                    //{
                      
                    //    var content = stream.ToArray();

                    //    // ส่งไฟล์ Excel กลับเป็น response
                    //    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Approval.xlsx");
                    //}
                }
            }
            catch (Exception ex)
            {

                return Ok(ex.Message);
            }
         
        }
    }
}
