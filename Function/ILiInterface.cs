using Packing.Models;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using Microsoft.EntityFrameworkCore;

namespace Packing.Function
{
    public interface ILiInterface
    {
        Task<MaterialList> GetMaterial(string MaterialCode);
        Task<LiCheckBatchNo> CheckBatchNo(string batchNo);
        Task<LiCheckDate> CheckShift(DateTime date);
    }

    public class LiInterface: ILiInterface
    {
        private vms_packingContext _context;
        private VMS_CORE_2Context _2Context;
        public LiInterface(vms_packingContext context, VMS_CORE_2Context Context)
        {
            _context = context;
            _2Context = Context;
        }

        public async Task<LiCheckBatchNo> CheckBatchNo(string batchNo)
        {
            return await _context.tbt_pk_batch_no_header.Select(x => new LiCheckBatchNo { BatchNo = x.BATCH_NO, SubBatch = x.SUB_BATCH }).FirstOrDefaultAsync(x => x.BatchNo == batchNo)!;
        }

        public async Task<LiCheckDate> CheckShift(DateTime date)
        {
            LiCheckDate result = new LiCheckDate();
            
            var shift = await _context.tbm_pk_work_shift.FirstOrDefaultAsync(x=> (DateTime.Now.Hour >= x.TIME_START.Value.Hour && DateTime.Now.Minute >= x.TIME_START.Value.Minute) && (DateTime.Now.Hour <= x.TIME_END.Value.Hour && DateTime.Now.Minute <= x.TIME_END.Value.Minute));
            if (shift!=null)
            {
                result.Shift = shift.WORK_SHIFT;
            }
            result.ExpiredDate = date.AddMonths(0);
            return result;

        }

        public async Task<MaterialList> GetMaterial(string MaterialCode)
        {
            return await _context.tbm_material.Select(x => new MaterialList { MATERIAL_CODE = x.MATERIAL_CODE, MATERIAL_NAME = x.MATERIAL_NAME, MATERIAL_GROUP = x.MATERIAL_GROUP, PKG_SIZE_KG = x.PKG_SIZE_KG,SHELF_LIFT_MONTH=x.SHELF_LIFT_MONTH }).FirstOrDefaultAsync(x => x.MATERIAL_CODE == MaterialCode);
        }
    }
}
