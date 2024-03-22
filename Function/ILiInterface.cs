using Packing.Models;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using Microsoft.EntityFrameworkCore;

namespace Packing.Function
{
    public interface ILiInterface
    {
        Task<MaterialList> GetMaterial(string MaterialCode);
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

        public async Task<MaterialList> GetMaterial(string MaterialCode)
        {
            return await _context.tbm_material.Select(x => new MaterialList { MATERIAL_CODE = x.MATERIAL_CODE, MATERIAL_NAME = x.MATERIAL_NAME, MATERIAL_GROUP = x.MATERIAL_GROUP, PKG_SIZE_KG = x.PKG_SIZE_KG }).FirstOrDefaultAsync(x => x.MATERIAL_CODE == MaterialCode);
        }
    }
}
