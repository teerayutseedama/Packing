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


namespace Packing.Function
{
    public interface ILiInterface
    {
        Task<LiMaterial> GetMaterial(string MaterialCode);
        Task<LiCheckBatchNo> CheckBatchNo(string batchNo);
        Task<LiCheckDate> CheckShift(DateTime date);
        Task<ResponseMessage> SaveLIData(SaveLiData data);
        Task<LiDataView> LoadLiDataView(string batchNo,int subBatch);
        Task<IEnumerable<QrCodeData>> GetQrCodeData(GetQrCodeData data);
        Task<tbm_pk_batch_slip> GetConfig();

        Task<ResponseMessage> CloseJob(LiCloseJob data);
    }

    public class LiInterface: ILiInterface
    {
        private vms_packingContext _context;
        private VMS_CORE_2Context _2Context;
        private ResponseMessage _response;
        public LiInterface(vms_packingContext context, VMS_CORE_2Context Context)
        {
            _context = context;
            _2Context = Context;
            _response = new ResponseMessage();
        }

        public async Task<LiCheckBatchNo> CheckBatchNo(string batchNo)
        {
            return await _context.tbt_pk_batch_no_header.Select(x => new LiCheckBatchNo { BatchNo = x.BATCH_NO, SubBatch = x.SUB_BATCH }).FirstOrDefaultAsync(x => x.BatchNo == batchNo)!;
        }

        public async Task<LiCheckDate> CheckShift(DateTime date)
        {
            LiCheckDate result = new LiCheckDate();
            
            var shift = await _context.tbm_pk_work_shift.FirstOrDefaultAsync(x=>x.TIME_START.Value.TimeOfDay<=DateTime.Now.TimeOfDay && x.TIME_END.Value.TimeOfDay>=DateTime.Now.TimeOfDay);
   
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
                    var status = await _context.tbm_pk_batch_status.FirstOrDefaultAsync(x => x.BATCH_STATUS == data.Status);
                    if (status != null)
                    {
                        head.BATCH_STATUS = status.ID;
                        detail.ForEach(x => {
                            x.BATCH_STATUS = status.ID;
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
                                  CodeQR =  he.BATCH_NO + "." + d.BATCH_RUNNING_NO ,
                                  WORK_SHIFT​ = s.WORK_SHIFT ?? "WORK_SHIFT",
                                  BATCH_NO = m.MATERIAL_CODE+"."+ he.BATCH_NO+"."+li.PK_LINE_NAME+"."+d.BATCH_RUNNING_NO,
                                  MFG_DATE = he.MFG_DATE,
                                  EXPIRE_DATE​ = he.EXPIRE_DATE,
                                  FORM_NO​ = m.FORM_NO ?? "FORM_NO",
                                  REV = m.REV ?? "REV",
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
                save.CREATE_DATE = DateTime.Now;
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
                    detail.BATCH_RUNNING_NO = i+1;
                    detail.WORK_SHIFT_ID = data.WORK_SHIFT_ID;
                    detail.BATCH_STATUS = data.BATCH_STATUS;
                    detail.REMARK_REJECT = "";
                    detail.REMARK_HOLD = "";
                    detail.REMARK_HOLD_TO_PASS = "";
                    detail.CREATE_BY = data.User;
                    detail.CREATE_DATE = DateTime.Now;
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
    }
}
