using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_material_bak
{
    public string MATERIAL_CODE { get; set; } = null!;

    public string MATERIAL_INFO { get; set; } = null!;

    public string PKG_SIZE_KG { get; set; } = null!;

    public string BUN { get; set; } = null!;

    public string MATERIAL_GROUP { get; set; } = null!;

    public string SLOC { get; set; } = null!;

    public string SAP_SLOC { get; set; } = null!;

    public string? SHELF_LIFT_MONTH { get; set; }
}
