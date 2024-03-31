using DocumentFormat.OpenXml.InkML;
using Packing.Models;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using Microsoft.EntityFrameworkCore;
namespace Packing.Function
{
    public interface IMasterDataInterface
    {
        Task<MasterDataView> GetMasterDataView();

    }

    public class MasterDataInterface: IMasterDataInterface
    {
        private vms_packingContext _context;
        private VMS_CORE_2Context _2Context;
        public MasterDataInterface(vms_packingContext context, VMS_CORE_2Context Context) {
            _context = context;
            _2Context = Context;
        }

        public async Task<MasterDataView> GetMasterDataView()
        {
           var result= new MasterDataView();
            try
            {
                result.LineList = await _context.tbm_pk_production_line.Select(x => new LineList { PK_LINE_NAME = x.PK_LINE_NAME, PACKING_LINE_ID = x.PACKING_LINE_ID,ID_PLANT = x.PLANT_ID }).ToListAsync();
                result.StatausList = await _context.tbm_pk_batch_status.ToListAsync();
                result.ShiftList = await _context.tbm_pk_work_shift.ToListAsync();
                result.PlantList = await _2Context.tbm_plants.Select(x => new PlantList { PLANT = x.PLANT, PLANT_NAME = x.PLANT_NAME }).ToListAsync();
                result.MaterialList = await _context.tbm_material.Select(x => new MaterialList { MATERIAL_CODE = x.MATERIAL_CODE, MATERIAL_NAME = x.MATERIAL_NAME,MATERIAL_GROUP=x.MATERIAL_GROUP,PKG_SIZE_KG=x.PKG_SIZE_KG }).ToListAsync();
                result.UomtList=await _2Context.tbm_units.Where(x=>x.ACTIVE==true).ToListAsync();
                result.SlocList=await _context.tbm_pk_sloc.ToListAsync();

            }
            catch (Exception ex)
            {
                string mess=ex.Message;
            }
          
        return result;
        }
    }
}
