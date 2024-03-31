using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbt_pk_batch_no_header
{
    public int id { get; set; }
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

    /// <summary>
    /// เอามาจาก SHELF_LIFT_MONTH
    /// </summary>
    public DateTime EXPIRE_DATE { get; set; }

    public string? CREATE_BY { get; set; }

    public DateTime? CREATE_DATE { get; set; }

    public string? UPDATE_BY { get; set; }

    public DateTime? UPDATE_DATE { get; set; }

    public int? BATCH_STATUS { get; set; }

    public string? APPROVE_BY { get; set; }

    public DateTime? APPROVE_DATE { get; set; }


}
