using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Packing.Views.DataView
{
    public class SummaryDataView
    {
        public string? MaterialCode { get; set; }
        public string? MaterialGroup { get; set; }
        public string? MaterialName { get; set; }
        public string? Package { get; set; }
        public string? MFGDate { get; set; }
        public string? ExpireDate { get; set; }
        public int? Qty { get; set; }
        public string? UOM { get; set; }
    }

    public  class GetSummaryDataView
    {
       public string? Plant { get; set; }
       public int? Line { get; set; }
       public string? MaterialCode { get; set; }
       public string? MaterialName{ get; set; }
       public string? BatchNo{ get; set; }
       public string? Shift { get; set; }
       public DateTime? FromDate{ get; set; }
       public DateTime? ToDate{ get; set; }
    }
}
