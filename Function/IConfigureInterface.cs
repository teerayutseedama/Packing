using System;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Packing.Models;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
namespace Packing.Function
{
	public interface IConfigureInterface
	{
		Task<ResponseMessage> SaveWorkShift(List<SaveShipDataView> data);
        Task<ResponseMessage> UpdateBatch(UpdateBatchShiftDatView data); 
        Task<IEnumerable<ConfigureDataView>> GetMaterialList(string orderBy=null!);

        Task<tbm_pk_batch_slip> Get_Batch_Slip();
        Task<IEnumerable<tbm_pk_work_shift>> Get_Work_Shifts();

        Task<ResponseMessage> SaveConfigureMaterial(SaveConfigureMaterial data);

    }

    public class ConfigureInterface : IConfigureInterface
    {
        private readonly vms_packingContext _context;
        private readonly VMS_CORE_2Context _context2;
        private ResponseMessage _response;
        public ConfigureInterface(vms_packingContext context, VMS_CORE_2Context context2) {
            _context = context;
             _response=new ResponseMessage();
            _context2= context2;
                }

        public async Task<IEnumerable<ConfigureDataView>> GetMaterialList(string orderBy=null!)
        {

            var list = await (from m in _context.tbm_material
                              from li in _context.tbm_pk_production_line.Where(x => x.PACKING_LINE_ID == m.DEFAULT_PACKING_LINE_ID).DefaultIfEmpty()
                              from st in _context.tbm_pk_batch_status.Where(x => x.ID == m.DEFAULT_HOLD).DefaultIfEmpty()
                              select new ConfigureDataView
                              {
                                 id=m.id,
                                  MATERIAL_CODE = m.MATERIAL_CODE,
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
                                  PACKING_LINE_ID = li.PK_LINE_NAME,
                                  UOM = "",
                                  HOLD = st.BATCH_STATUS,
                                  STATUS_NAME = m.STATUS==0? "CLOSE":m.STATUS==1?"OPEN":"",
                              }).ToListAsync();
            return list;

        }

        public async Task<tbm_pk_batch_slip> Get_Batch_Slip()
        {
            var data= await _context.tbm_pk_batch_slip.FirstOrDefaultAsync()!;
            return data!;
        }

        public async Task<IEnumerable<tbm_pk_work_shift>> Get_Work_Shifts()
        {
            return await _context.tbm_pk_work_shift.ToListAsync();
        }

        public async Task<ResponseMessage> SaveConfigureMaterial(SaveConfigureMaterial data)
        {
            try
            {
                var mat = await _context.tbm_material.Where(x => x.id == data.id).ToListAsync();
                if (mat != null)
                {
                    mat.ForEach(x =>
                    {
                        x.STATUS = data.STATUS;
                        x.FORM_NO = data.FORM_NO;
                        x.REV = data.REV;
                        x.DEFAULT_HOLD = data.DEFAULT_HOLD;
                        x.DEFAULT_PACKING_LINE_ID = data.DEFAULT_PACKING_LINE_ID;
                        x.DEFAULT_UOM = data.DEFAULT_UOM;
                    });
              
                    _context.tbm_material.UpdateRange(mat);
                }
                _response.Status = await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _response.Status = false;
                _response.Error = ex.Message;
               
            }
            return _response;
        }

        public async Task<ResponseMessage> SaveWorkShift(List<SaveShipDataView> data)
        {
            try
            {
                var check = await _context.tbm_pk_work_shift.ToListAsync();
                if (check.Count > 0)
                {
                    _context.tbm_pk_work_shift.RemoveRange(check);
                    await _context.SaveChangesAsync();
                }
                var saveList = new List<tbm_pk_work_shift>();
                var ID = 0;
                foreach (var item in data)
                {
                    ID++;
                    var newShip = new tbm_pk_work_shift();
                    newShip.ID = ID;
                    newShip.TIME_START = SystemClass.ConvertTime(item.TIME_START!);
                    newShip.TIME_END =SystemClass.ConvertTimeEnd(item.TIME_END!, item.TIME_START!);
                    newShip.WORK_SHIFT = item.WORK_SHIFT;
                    saveList.Add(newShip);
                }

                await _context.tbm_pk_work_shift.AddRangeAsync(saveList);
                _response.Message = "บันทึกข้อมูลเสร็จสิ้น";
                _response.Status= await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                _response.Error = ex.Message;
                _response.Status = false;
            }
            return _response;
      
        }

        public async Task<ResponseMessage> UpdateBatch(UpdateBatchShiftDatView data)
        {
            try
            {
                var batch = await _context.tbm_pk_batch_slip.ToListAsync();

                if (batch.Count > 0)
                {
                    _context.tbm_pk_batch_slip.RemoveRange(batch);

                }
                if (await _context.SaveChangesAsync() > 0)
                {
                    var save = new tbm_pk_batch_slip();
                    save.STICKER_WIDTH = data.STICKER_WIDTH;

                    save.STICKER_HEIGH = data.STICKER_HEIGH;

                    save.QR_CODE_WIDTH = data.QR_CODE_WIDTH;

                    save.QR_CODE_HEIGHT = data.QR_CODE_HEIGHT;

                    save.FONT_SIZE = data.FONT_SIZE;

                    save.RUNNING_FONT_SIZE = data.RUNNING_FONT_SIZE;

                    save.FORM_NO_SIZE = data.FORM_NO_SIZE;

                    save.QR_CODE_SIZE_UNIT = data.QR_CODE_SIZE_UNIT;

                    ;
                    await _context.tbm_pk_batch_slip.AddAsync(save);
                }
                _response.Message = "บันทึกข้อมูลเสร็จสิ้น";
                _response.Status= await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _response.Error = ex.Message;
                _response.Status = false;
            }
            return _response;
         
                  
        }
    }
}

