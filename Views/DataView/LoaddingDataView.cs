using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.SignalR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Packing.Views.DataView
{
    public class LoaddingDataView
    {
        public string? BatchNo { get; set; }
        public int? SubBatch { get; set; }
        public string? Plant { get; set; }
        public string? MaterialName { get; set; }
        public string? Package { get; set; }
        public string? Line { get; set; }
        public string? MFGDate { get; set; }
        public int? Qty { get; set; }
        public string? UOM { get; set; }
    }
    public class GetLoaddingData
    {
        public string? Plant { get; set; }
        public string? Line { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? Shift { get; set; }
        public DateTime? MFGDate { get; set; }
        public string? Stataus { get; set; }
        public string? BatchNo { get; set; }
        public string? RunNo { get; set; }
    }
}
