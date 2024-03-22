using Packing.Models;
using Packing.vmsPackingDB;

namespace Packing.Views.DataView
{
    public class MasterDataView
    {
      public IEnumerable<PlantList>?  PlantList { get; set; }
        public IEnumerable<LineList>? LineList { get; set; }
        public IEnumerable<MaterialList>? MaterialList  { get; set; }

public IEnumerable<tbm_pk_batch_status>? StatausList { get; set; }
public IEnumerable<tbm_pk_work_shift>? ShiftList { get; set; }
        public IEnumerable<tbm_pk_sloc>? SlocList { get; set; }
        public IEnumerable<tbm_unit>? UomtList { get; set; }
    }
    public class PlantList
    {
        public string? PLANT { get; set; }

        public string? PLANT_NAME { get; set; }
    }
    public class LineList
    {
        public int PACKING_LINE_ID { get; set; }
        public string? PK_LINE_NAME { get; set; } = null!;
    }

    public class MaterialList
    {
        public string MATERIAL_CODE { get; set; } = null!;
        public string MATERIAL_GROUP { get; set; } = null!;
        public string PKG_SIZE_KG { get; set; } = null!;
        public string MATERIAL_NAME { get; set; } = null!;

    }
}
