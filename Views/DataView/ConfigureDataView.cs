using Packing.vmsPackingDB;

namespace Packing.Views.DataView
{
    public class ConfigureDataView:tbm_material
    {
        public string? PACKING_LINE_ID { get; set; }

        public string? UOM { get; set; }

        public string? HOLD { get; set; }

        public string? STATUS_NAME { get; set; }
    }

    public class SaveConfigureMaterial
    {
        public int id { get; set; }
        public string MATERIAL_CODE { get; set; } = null!;
        public int? DEFAULT_PACKING_LINE_ID { get; set; }
        public string? DEFAULT_UOM { get; set; }
        public int? DEFAULT_HOLD { get; set; }
        public int? STATUS { get; set; }
        public string? FORM_NO { get; set; }
        public string? REV { get; set; }
    }
}
