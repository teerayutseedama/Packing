using DocumentFormat.OpenXml.Wordprocessing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO.Packaging;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Vml;

namespace Packing.Views.DataView
{
    public class ApprovalDataView
    {
        public string? BatchNo { get; set; }
        public int? BatchSub { get; set; }
        public string? Plant { get; set; }
        public string? MaterialName { get; set; }
        public string? Line { get; set; }
        public string? Package { get; set; }
        public string? RunNo { get; set; }
        public int? Qty { get; set; }
        public string? UOM { get; set; }
        public string? MFGDate { get; set; }
        public string? ExpireDate { get; set; }
        public string? Status { get; set; }
        public string? sloc { get; set; }
        public string? MaterialCode { get; set; }
        public string? Shift { get; set; }
        public int id_status { get; set; }
    }

    public class GetApprovalData
    {
        public string? Plant { get; set; }
        public string? Line { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? Shift { get; set; }
        public DateTime? MFGDate { get; set; }
        public string? Status { get; set; }
        public string? BatchNo { get; set; }
        public string? RunNo { get; set; }
    }

    public class SaveApprovalData
    {
        public string? BatchNo { get; set; }
        public int BatchSub { get; set; } = 0;
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public string? User { get; set; }
    }
}
