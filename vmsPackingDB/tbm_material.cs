using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_material
{
    public int id { get; set; } 
    public string MATERIAL_CODE { get; set; } = null!;

    public string MATERIAL_NAME { get; set; } = null!;

    /// <summary>
    /// join to tbm_materail_type
    /// </summary>
    public string MATERIAL_TYPE_ID { get; set; } = null!;

    public string PKG_SIZE_KG { get; set; } = null!;

    public string BUN { get; set; } = null!;

    public string MATERIAL_GROUP { get; set; } = null!;

    /// <summary>
    /// join to tbm_pk_sloc
    /// </summary>
    public string SLOC_ID { get; set; } = null!;

    public string? SHELF_LIFT_MONTH { get; set; }

    public string? FORM_NO { get; set; }

    public string? REV { get; set; }

    public int? DEFAULT_PACKING_LINE_ID { get; set; }

    public string? DEFAULT_UOM { get; set; }

    public int? DEFAULT_HOLD { get; set; }

    public int? STATUS { get; set; }
}
