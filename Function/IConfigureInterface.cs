using System;
using Microsoft.EntityFrameworkCore;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
namespace Packing.Function
{
	public interface IConfigureInterface
	{
		Task<bool> SaveWorkShift(IEnumerable<SaveShipDataView> data);
        Task<bool> UpdateBatch(UpdateBatchShiftDatView data); 
	}

    public class ConfigureInterface : IConfigureInterface
    {
        private readonly vms_packingContext _context;
        public ConfigureInterface(vms_packingContext context) {
            _context = context;
                }
        public async Task<bool> SaveWorkShift(IEnumerable<SaveShipDataView> data)
        {
            var check = await _context.tbm_pk_work_shift.ToListAsync();
            if (check.Count > 0)
            {
                 _context.tbm_pk_work_shift.RemoveRange(check);
                await _context.SaveChangesAsync();
            }
            var saveList =new  List<tbm_pk_work_shift>();
            var ID = 0;
            foreach (var item in data)
            {
                ID++;
                var newShip = new tbm_pk_work_shift();
                newShip.ID = ID;
                newShip.TIME_START = item.TIME_START;
                newShip.TIME_END = item.TIME_END;
                newShip.WORK_SHIFT = item.WORK_SHIFT;
                saveList.Add(newShip);
            }
            
            await _context.tbm_pk_work_shift.AddRangeAsync(saveList);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateBatch(UpdateBatchShiftDatView data)
        {
            var batch = await _context.tbm_pk_batch_slip.ToListAsync();

            if (batch.Count >0)
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
            return await _context.SaveChangesAsync() > 0;
                  
        }
    }
}

