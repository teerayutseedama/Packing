using System;
using Microsoft.EntityFrameworkCore;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
namespace Packing.Function
{
	public interface IConfigureInterface
	{
		Task<ResponseMessage> SaveWorkShift(List<SaveShipDataView> data);
        Task<ResponseMessage> UpdateBatch(UpdateBatchShiftDatView data); 
        Task<IEnumerable<tbm_material>> GetMaterialList(string orderBy=null!);

        Task<tbm_pk_batch_slip> Get_Batch_Slip();
        Task<IEnumerable<tbm_pk_work_shift>> Get_Work_Shifts();
	}

    public class ConfigureInterface : IConfigureInterface
    {
        private readonly vms_packingContext _context;
        private ResponseMessage _response;
        public ConfigureInterface(vms_packingContext context) {
            _context = context;
             _response=new ResponseMessage();
                }

        public async Task<IEnumerable<tbm_material>> GetMaterialList(string orderBy=null!)
        {
            if (orderBy == null)
            {
              return  await _context.tbm_material.ToListAsync();
            } else if (orderBy == "code")
            {
                return await _context.tbm_material.OrderBy(x => x.MATERIAL_CODE).ToListAsync();
            }
            else if (orderBy == "group")
            {
                return await _context.tbm_material.OrderBy(x => x.MATERIAL_GROUP).ToListAsync();
            }
            else 
            {
                return await _context.tbm_material.OrderBy(x=>x.MATERIAL_NAME).ToListAsync();
            }

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
                    newShip.TIME_END =SystemClass.ConvertTime(item.TIME_END!);
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

