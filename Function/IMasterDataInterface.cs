using DocumentFormat.OpenXml.InkML;
using Packing.Models;
using Packing.vmsPackingDB;

namespace Packing.Function
{
    public interface IMasterDataInterface
    {
    }

    public class MasterDataInterface: IMasterDataInterface
    {
        private vms_packingContext _context;
        private VMS_CORE_2Context _2Context;
        public MasterDataInterface(vms_packingContext context, VMS_CORE_2Context Context,) {
            _context = context;
            _2Context = Context;
        }
    }
}
