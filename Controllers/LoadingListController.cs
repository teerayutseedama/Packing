using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Packing.Function;
using Packing.Views.DataView;
using System.Data.Common;
using System.IO;

namespace Packing.Controllers
{
    public class LoadingListController : Controller
    {
        private readonly ILoaddingListInterface _loadding;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LoadingListController(ILoaddingListInterface loadding, IWebHostEnvironment webHostEnvironment)
        {
            _loadding = loadding;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: LoadingListController
        public ActionResult Index()
        {
            return View();
        }

        // GET: LoadingListController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LoadingListController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoadingListController/Create
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

        // GET: LoadingListController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoadingListController/Edit/5
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

        // GET: LoadingListController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoadingListController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
        public async Task<IActionResult> GetLoaddingDataViews(GetLoaddingData data)
        {
            return Ok(await _loadding.GetLoaddingDataViews(data));
        }

        public async Task<IActionResult> DownloadLoaddingExcel(GetLoaddingData data)
        {
            try
            {

                var dataList = await _loadding.GetLoaddingDataViews(data);
                string path = _webHostEnvironment.WebRootPath + "/ExcelLoading/Loadding.xlsx";
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("LoaddingList");
                    // สร้าง Header
                    worksheet.Cell(1, 1).Value = "BatchNo";
                    worksheet.Cell(1, 2).Value = "SubBatch";
                    worksheet.Cell(1, 3).Value = "Plant";
                    worksheet.Cell(1, 4).Value = "Line";
                    worksheet.Cell(1, 5).Value = "MaterialName";
                    worksheet.Cell(1, 6).Value = "Package";
                    worksheet.Cell(1, 7).Value = "MFGDate";
                    // worksheet.Cell(1, 8).Value = "RunNo";
                    worksheet.Cell(1, 8).Value = "Qty";
                    worksheet.Cell(1, 9).Value = "UOM";

                    // เขียนข้อมูลจาก List ลงใน Excel
                    int row = 2;
                    foreach (var item in dataList)
                    {
                        worksheet.Cell(row, 1).Value = item.BatchNo;
                        worksheet.Cell(row, 2).Value = item.SubBatch;
                        worksheet.Cell(row, 3).Value = item.Plant;
                        worksheet.Cell(row, 4).Value = item.Line;
                        worksheet.Cell(row, 5).Value = item.MaterialName;
                        worksheet.Cell(row, 6).Value = item.Package;
                        worksheet.Cell(row, 7).Value = item.MFGDate;
                        //worksheet.Cell(row, 8).Value = item.RunNo;
                        worksheet.Cell(row, 8).Value = item.Qty;
                        worksheet.Cell(row, 9).Value = item.UOM;
                        row++;
                    }
                    workbook.SaveAs(path);
                    return Ok(true);
                    // สร้าง MemoryStream เพื่อเก็บข้อมูล Excel
                    //using (var stream = new MemoryStream())
                    //{

                    //    var content = stream.ToArray();

                    //    // ส่งไฟล์ Excel กลับเป็น response
                    //    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
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
