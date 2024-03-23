using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO.Packaging;

namespace Packing.Views.DataView
{
    public class HistoryDataView
    {
      public string?  BatchNo { get; set; }
       public string? Plant { get; set; }
        public string? Line { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? Package { get; set; }
        public string? MFGDate { get; set; }
        public string? Shift { get; set; }
        public string? RunNo { get; set; }
        public int Qty { get; set; }
        public string? UOM { get; set; }
        public string? Stataus { get; set; }
    }

    public class GetHistoryData
    {
        public string? Plant { get; set; }
        public string? Line { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? BatchNo { get; set; }
        public string? Stataus { get; set; }
        public string? Shift { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
