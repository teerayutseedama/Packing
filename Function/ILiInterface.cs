using Packing.Models;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Drawing;
using Packing.Views;
using System.IO.Packaging;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Drawing;
using QRCoder;
using System.Globalization;
using System.Data.Common;
using System.Linq;
using Packing.Function;
using System.Data;

namespace Packing.Function
{
    public interface ILiInterface
    {
        Task<LiMaterial> GetMaterial(string MaterialCode);
        Task<LiCheckBatchNo> CheckBatchNo(string batchNo ,int SubBatch);
        Task<ResponseMessage> ValidRunNo(string batchNo,int RunFrom,int RunTo);
        Task<LiCheckDate> CheckShift(DateTime date);
        Task<ResponseMessage> SaveLIData(SaveLiData data);
        Task<LiDataView> LoadLiDataView(string batchNo,int subBatch);
        Task<IEnumerable<QrCodeData>> GetQrCodeData(GetQrCodeData data);
        Task<tbm_pk_batch_slip> GetConfig();

        Task<ResponseMessage> CloseJob(LiCloseJob data);

        Task<IEnumerable<tbm_pk_production_line>> GetLine(int sloc_id);
        Task<IEnumerable<tbm_pk_sloc>> GetStorageLocation(int plantid);
        Task<Mat_Line_Sloc> GetMaterialsLines(string sloc_id);
    }

    public class LiInterface: ILiInterface
    {
        private vms_packingContext _context;
        private VMS_CORE_2Context _2Context;
        private ResponseMessage _response;
        private DB db;
        public LiInterface(vms_packingContext context, VMS_CORE_2Context Context)
        {
            _context = context;
            _2Context = Context;
            _response = new ResponseMessage();
        }

        public async Task<LiCheckBatchNo> CheckBatchNo(string batchNo,int SubBatch)
        {
            var list = await _context.tbt_pk_batch_no_header.Where(x => x.BATCH_NO == batchNo).OrderByDescending(x=>x.SUB_BATCH).ToListAsync();
            return list.Select(x=> new LiCheckBatchNo { BatchNo = x.BATCH_NO, SubBatch = x.SUB_BATCH, QTY_FROM = x.QTY_FROM, QTY_TO = x.QTY_TO, QTY_TOTAL = list.Sum(x=>x.QTY_TOTAL)}).FirstOrDefault()!;
        }

        public async Task<LiCheckDate> CheckShift(DateTime date)
        {
            LiCheckDate result = new LiCheckDate();
            DateTime idate = new DateTime(2000, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            var shift = await _context.tbm_pk_work_shift.FirstOrDefaultAsync(x=>x.TIME_START.Value < idate && x.TIME_END.Value>=idate);
   
            if (shift!=null)
            {
                result.Shift = shift.WORK_SHIFT;
                result.ShiftID=shift.ID;
            }
            result.ExpiredDate = date.AddMonths(0);
            return result;
        }

        public async Task<ResponseMessage> CloseJob(LiCloseJob data)
        {
            try
            {               
                var head = await _context.tbt_pk_batch_no_header.FirstOrDefaultAsync(x => x.SUB_BATCH == data.SUB_BATCH && x.BATCH_NO==data.BATCH_NO);
                var detail = await _context.tbt_pk_batch_no_detail.Where(x => x.BATCH_NO == data.BATCH_NO && x.SUB_BATCH == data.SUB_BATCH).ToListAsync();
                if (head != null && detail.Count>0)
                {
                    var istatus = await _context.tbm_material.FirstOrDefaultAsync(x => x.MATERIAL_CODE == head.MATERIAL_CODE);
                    //var batch_status = await _context.tbm_pk_batch_status.ToListAsync();
                    if (istatus != null)
                    {
                        var status = 0;
                        if (istatus.DEFAULT_HOLD == null)
                        {
                            status = 1;
                        }
                        else
                        {
                            status = istatus.DEFAULT_HOLD.Value;
                        }
                        head.BATCH_STATUS = status;
                        detail.ForEach(x => {
                            x.BATCH_STATUS = status;
                        });
                    }
                     _context.tbt_pk_batch_no_header.Update(head);
                     _context.tbt_pk_batch_no_detail.UpdateRange(detail);
                }
                _response.Status = await _context.SaveChangesAsync()>0;
            
            }
            catch (Exception ex)
            {
                _response.Status = false;
                _response.Error = ex.Message;
            }
            return _response;
        }

        public async Task<tbm_pk_batch_slip> GetConfig()
        {
            return await _context.tbm_pk_batch_slip.FirstOrDefaultAsync();
        }

        public async Task<LiMaterial> GetMaterial(string MaterialCode)
        {
            var list = await (from m in _context.tbm_material.Where(x => x.MATERIAL_CODE == MaterialCode)
                              from mt in _context.tbm_materail_type.Where(x => x.ID.ToString() == m.MATERIAL_TYPE_ID)
                              select new LiMaterial {
                           MATERIAL_CODE=m.MATERIAL_CODE,
                           MaterialType = mt.MATERIAL_TYPE,
                                  MATERIAL_NAME = m.MATERIAL_NAME,
                                  MATERIAL_TYPE_ID = m.MATERIAL_TYPE_ID,
                                  PKG_SIZE_KG = m.PKG_SIZE_KG,
                                  BUN = m.BUN,
                                  MATERIAL_GROUP = m.MATERIAL_GROUP,
                                  SLOC_ID = m.SLOC_ID,
                                  SHELF_LIFT_MONTH = m.SHELF_LIFT_MONTH,
                                  FORM_NO = m.FORM_NO,
                                  REV = m.REV,
                                  DEFAULT_PACKING_LINE_ID = m.DEFAULT_PACKING_LINE_ID,
                                  DEFAULT_UOM = m.DEFAULT_UOM,
                                  DEFAULT_HOLD = m.DEFAULT_HOLD,
                                  STATUS = m.STATUS,
                              }).FirstOrDefaultAsync();
            return list;
        }

        public async Task<IEnumerable<QrCodeData>> GetQrCodeData(GetQrCodeData data)
        {

            try
            {
      
                var list= await (from he in _context.tbt_pk_batch_no_header.Where(x => x.BATCH_NO == data.BATCH_NO && x.SUB_BATCH == data.SUB_BATCH)
                                 from m in _context.tbm_material.Where(x => x.MATERIAL_CODE == he.MATERIAL_CODE).DefaultIfEmpty()
                                 from mt in _context.tbm_materail_type.Where(x => x.ID.ToString() == m.MATERIAL_TYPE_ID).DefaultIfEmpty()
                                 from s in _context.tbm_pk_work_shift.Where(x => x.ID == he.WORK_SHIFT_ID).DefaultIfEmpty()
                                 from li in _context.tbm_pk_production_line.Where(x=>x.PACKING_LINE_ID==he.PACKING_LINE_ID).DefaultIfEmpty()
                              from d in _context.tbt_pk_batch_no_detail.Where(x => x.BATCH_NO == data.BATCH_NO && x.SUB_BATCH == data.SUB_BATCH && data.BATCH_RUNNING_NO.Contains(x.BATCH_RUNNING_NO))
                              select new 
                              {
                                  Material_GROUP = m.MATERIAL_GROUP,
                                  Material_Type = mt.MATERIAL_TYPE ?? "",
                                  BATCH_RUNNING_NO = d.BATCH_RUNNING_NO,
                                  Line = li.PK_LINE_NAME,
                                  CodeQR = m.MATERIAL_CODE + "." + he.BATCH_NO + "." + li.PK_LINE_NAME + "." + d.BATCH_RUNNING_NO,
                                  WORK_SHIFT​ = s.WORK_SHIFT ?? "",
                                  BATCH_NO = he.BATCH_NO,
                                  MFG_DATE = he.MFG_DATE,
                                  EXPIRE_DATE​ = he.EXPIRE_DATE,
                                  FORM_NO​ = m.FORM_NO ?? "",
                                  REV = m.REV == null ? "" : m.REV == "" ? "" : "แก้ไขครั้งที่ " + m.REV,
                              }).Distinct().ToListAsync();
           
                var qr = new List<QrCodeData>();
                foreach (var item in list)
                {
                    // สร้าง QR Code จากข้อมูล BATCH_NO และ BATCH_RUNNING_NO
                    var qrCode = await GenerateQRCode(item.CodeQR);

                    // สร้าง QrCodeData จากข้อมูลที่ได้
                    var qrCodeData = new QrCodeData
                    {
                        Material_GROUP = item.Material_GROUP,
                        Material_Type = item.Material_Type,
                        BATCH_RUNNING_NO = item.BATCH_RUNNING_NO,
                        Line = item.Line,
                        CodeQR = qrCode,
                        WORK_SHIFT = item.WORK_SHIFT,
                        BATCH_NO = item.BATCH_NO,
                        MFG_DATE = item.MFG_DATE,
                        EXPIRE_DATE = item.EXPIRE_DATE,
                        FORM_NO = item.FORM_NO,
                        REV = item.REV ,
                    };

                    qr.Add(qrCodeData);
                }
                return qr.AsEnumerable();
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                throw;
            }
        
        }

        public async Task<LiDataView> LoadLiDataView(string batchNo, int subBatch)
        {
            LiDataView result = new LiDataView();
            try
            {
                var data=await _context.tbt_pk_batch_no_header.FirstOrDefaultAsync(x=>x.BATCH_NO==batchNo && x.SUB_BATCH==subBatch);

                var mat = await _context.tbm_material.Where(x => x.MATERIAL_CODE == data.MATERIAL_CODE).FirstOrDefaultAsync();
                var mattype = await _context.tbm_materail_type.Where(x => x.ID.ToString() == mat.MATERIAL_TYPE_ID).FirstOrDefaultAsync();
                var shift = await _context.tbm_pk_work_shift.Where(x => x.ID == data.WORK_SHIFT_ID).FirstOrDefaultAsync();
                if (data != null)
                {
                    var status = await _context.tbm_pk_batch_status.FirstOrDefaultAsync(x => x.ID == data.BATCH_STATUS);
                    result.BATCH_NO = data.BATCH_NO;
                    result.SUB_BATCH = data.SUB_BATCH;
                    result.PACKING_LINE_ID = data.PACKING_LINE_ID;
                    result.SLOC = data.SLOC;
                    result.WORK_SHIFT_ID = data.WORK_SHIFT_ID;
                    result.MATERIAL_CODE = data.MATERIAL_CODE;
                    result.QTY_TOTAL = data.QTY_TOTAL;
                    result.QTY_FROM = data.QTY_FROM;
                    result.QTY_TO = data.QTY_TO;
                    result.UOM = data.UOM;
                    result.PACKAGE = data.PACKAGE;
                    result.MFG_DATE = data.MFG_DATE;
                    result.EXPIRE_DATE = data.EXPIRE_DATE;
                    result.SHELF_LIFT_MONTH = mat.SHELF_LIFT_MONTH;
                    result.MATERIAL_NAME = mat.MATERIAL_NAME;
                    result.MATERIAL_GROUP = mat.MATERIAL_GROUP;
                    result.MATERIAL_TYPE = mattype.MATERIAL_TYPE;
                    result.HOLD = mat.DEFAULT_HOLD == null ? "" : mat.DEFAULT_HOLD == 1 ? "PASS" : "HOLD";
                    result.SHIFT = shift.WORK_SHIFT;
                    result.SHIFT_ID = shift.ID;
                    if (status != null)
                    {
                        result.BATCH_STATUS = status!.BATCH_STATUS;
                    }
                  
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

      

        public async Task<ResponseMessage> SaveLIData(SaveLiData data)
        {
            try
            {

                DateTime now = DateTime.Now;

                var header = _context.tbt_pk_batch_no_header.Where(x => x.BATCH_NO == data.BATCH_NO && x.SUB_BATCH == data.SUB_BATCH).SingleOrDefault();
                if (header != null)
                {
                    _context.tbt_pk_batch_no_header.Remove(header);
                }

                var dt = _context.tbt_pk_batch_no_detail.Where(x=>x.BATCH_NO == data.BATCH_NO && x.SUB_BATCH == data.SUB_BATCH).ToList();
                if (dt.Count>0) { 
                    _context.tbt_pk_batch_no_detail.RemoveRange(dt);
                }

                var save = new tbt_pk_batch_no_header();
                save.BATCH_NO = data.BATCH_NO;
                save.SUB_BATCH = data.SUB_BATCH;
                save.PACKING_LINE_ID = data.PACKING_LINE_ID;
                save.SLOC = data.SLOC;
                save.WORK_SHIFT_ID = data.WORK_SHIFT_ID;
                save.MATERIAL_CODE = data.MATERIAL_CODE;
                save.QTY_TOTAL = data.QTY_TOTAL;
                save.QTY_FROM = data.QTY_FROM;
                save.QTY_TO = data.QTY_TO;
                save.UOM = data.UOM;
                save.PACKAGE = data.PACKAGE;
                save.MFG_DATE = data.MFG_DATE;
                save.EXPIRE_DATE = data.EXPIRE_DATE;
                save.BATCH_STATUS = data.BATCH_STATUS;
                save.CREATE_BY = data.User;
                save.CREATE_DATE = now;
                save.UPDATE_BY = null;
                save.UPDATE_DATE = null;
                save.APPROVE_BY =null;
                save.APPROVE_DATE = null;
                await _context.tbt_pk_batch_no_header.AddAsync(save);
                var detailList = new List<tbt_pk_batch_no_detail>();
                for (int i = 0; i < data.QTY_TOTAL; i++)
                {
                    var detail = new tbt_pk_batch_no_detail();
                    detail.BATCH_NO = data.BATCH_NO;
                    detail.SUB_BATCH = data.SUB_BATCH;
                    detail.BATCH_RUNNING_NO = i+data.QTY_FROM;
                    detail.WORK_SHIFT_ID = data.WORK_SHIFT_ID;
                    detail.BATCH_STATUS = data.BATCH_STATUS;
                    detail.REMARK_REJECT = "";
                    detail.REMARK_HOLD = "";
                    detail.REMARK_HOLD_TO_PASS = "";
                    detail.CREATE_BY = data.User;
                    detail.CREATE_DATE = now;
                    detail.UPDATE_BY = null;
                    detail.UPDATE_DATE = null;
                    detail.APPROVE_BY = null;
                    detail.APPROVE_DATE = null;
                    detailList.Add(detail);
                }



                await _context.tbt_pk_batch_no_detail.AddRangeAsync(detailList);
                _response.Status=  await _context.SaveChangesAsync()>0;  
            }
            catch (Exception ex)
            {
                _response.Status = false;
                _response.Error=ex.Message;
                throw;
            }
            return _response;
        }


        public async  Task<byte[]> GenerateQRCode(string value)
        {
            byte[] qrCodeImageBytes = await Task.Run(() =>
            {
                // สร้าง QR Code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                // แปลง QR Code เป็น Bitmap
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                // แปลง Bitmap เป็น byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    qrCodeImage.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            });
            return qrCodeImageBytes;
        }

        public async Task<IEnumerable<tbm_pk_production_line>> GetLine(int sloc_id)
        {
            var data = await _context.tbm_pk_production_line.Where(x => x.SLOC_ID == sloc_id).ToListAsync();
            return data;
        }

        public async Task<ResponseMessage> ValidRunNo(string batchNo, int RunFrom, int RunTo)
        {
            var valid = await _context.tbt_pk_batch_no_detail.Where(x=> x.BATCH_NO == batchNo && (x.BATCH_RUNNING_NO > RunFrom && x.BATCH_RUNNING_NO <= RunTo)).ToListAsync();
                if (valid.Count > 0)
            {
                _response.Status = false;
            }
            else
            {
                _response.Status = true;
            }
            return _response;
        }

        public async Task<IEnumerable<tbm_pk_sloc>> GetStorageLocation(int plantid)
        {
            try
            {
                var lst = await (from line in _context.tbm_pk_production_line.Where(x => x.PLANT_ID == plantid)
                                 join sloc in _context.tbm_pk_sloc on line.SLOC_ID equals sloc.ID
                                 select new tbm_pk_sloc()
                                 {
                                     ID = sloc.ID,
                                     SLOC = sloc.SLOC
                                 }).OrderBy(x => x.SLOC).ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Mat_Line_Sloc> GetMaterialsLines(string sloc_id)
        {
            var mat = await (from m in _context.tbm_material.Where(x => x.SLOC_ID == sloc_id)
                              from mt in _context.tbm_materail_type.Where(x => x.ID.ToString() == m.MATERIAL_TYPE_ID)
                              select new LiMaterial
                              {
                                  MATERIAL_CODE = m.MATERIAL_CODE,
                                  MaterialType = mt.MATERIAL_TYPE,
                                  MATERIAL_NAME = m.MATERIAL_NAME,
                                  MATERIAL_TYPE_ID = m.MATERIAL_TYPE_ID,
                                  PKG_SIZE_KG = m.PKG_SIZE_KG,
                                  BUN = m.BUN,
                                  MATERIAL_GROUP = m.MATERIAL_GROUP,
                                  SLOC_ID = m.SLOC_ID,
                                  SHELF_LIFT_MONTH = m.SHELF_LIFT_MONTH,
                                  FORM_NO = m.FORM_NO,
                                  REV = m.REV,
                                  DEFAULT_PACKING_LINE_ID = m.DEFAULT_PACKING_LINE_ID,
                                  DEFAULT_UOM = m.DEFAULT_UOM,
                                  DEFAULT_HOLD = m.DEFAULT_HOLD,
                                  STATUS = m.STATUS,
                              }).OrderBy(x=>x.MATERIAL_CODE).OrderBy(x=>x.MATERIAL_CODE).ToListAsync();

            var line = await _context.tbm_pk_production_line.Where(x => x.SLOC_ID == Convert.ToInt32(sloc_id)).OrderBy(x=>x.PK_LINE_NAME).ToListAsync();

            var ret = new Mat_Line_Sloc();
            ret.Mat = mat;
            ret.Lines = line;
          
            return ret;
        }
    }

    public class Mat_Line_Sloc
    {
        public IEnumerable<tbm_pk_production_line> Lines { get; set; }
        public IEnumerable<LiMaterial> Mat { get; set; }
    }
}
