using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_product_package_move2stored
{
    public string ZSIZE { get; set; } = null!;

    public string? ZSIZE_NAME { get; set; }

    public bool? ACTIVE { get; set; }

    public string? CREATE_BY { get; set; }

    public DateTime? CREATE_DATE { get; set; }

    public string? UPDATE_BY { get; set; }

    public DateTime? UPDATE_DATE { get; set; }
}
