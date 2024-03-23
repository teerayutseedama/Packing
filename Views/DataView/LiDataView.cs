using Packing.vmsPackingDB;
using System;
namespace Packing.Views.DataView
{
	public class LiDataView
	{
        public string BATCH_NO { get; set; } = null!;
        public int SUB_BATCH { get; set; }
        public int PACKING_LINE_ID { get; set; }
        public string SLOC { get; set; } = null!;
        public int WORK_SHIFT_ID { get; set; }
        public string? MATERIAL_CODE { get; set; }
        public int QTY_TOTAL { get; set; }
        public int QTY_FROM { get; set; }
        public int QTY_TO { get; set; }
        public string UOM { get; set; } = null!;
        public string PACKAGE { get; set; } = null!;
        public DateTime MFG_DATE { get; set; }
        public DateTime EXPIRE_DATE { get; set; }
        public string? BATCH_STATUS { get; set; }
    
    }

	public class LiCheckBatchNo
	{
		public string? BatchNo { get; set; }
		public int SubBatch { get; set; }
	}

	public class LiCheckDate
	{
		public DateTime? ExpiredDate { get; set; }
		public string? Shift { get; set; }
        public int? ShiftID { get; set; }

    }

    public class LiMaterial:tbm_material
    {

        public string? MaterialType { get; set; }
    }

    public class SaveLiData 
    {
        public string BATCH_NO { get; set; } = null!;

        public int SUB_BATCH { get; set; }

        public int PACKING_LINE_ID { get; set; }

        public string SLOC { get; set; } = null!;

        public int WORK_SHIFT_ID { get; set; }

        public string? MATERIAL_CODE { get; set; }

        public int QTY_TOTAL { get; set; }

        public int QTY_FROM { get; set; }

        public int QTY_TO { get; set; }

        public string UOM { get; set; } = null!;

        public string PACKAGE { get; set; } = null!;

        public DateTime MFG_DATE { get; set; }

        public DateTime EXPIRE_DATE { get; set; }

        public int? BATCH_STATUS { get; set; }
        public string? User { get; set; }

    }
}

