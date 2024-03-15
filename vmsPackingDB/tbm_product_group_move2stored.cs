using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_product_group_move2stored
{
    public string ZGROUP { get; set; } = null!;

    public string? GROUP_NAME { get; set; }

    public int? PRODUCT_TYPE_ID { get; set; }

    public bool? ACTIVE { get; set; }

    public string? CREATE_BY { get; set; }

    public DateTime? CREATE_DATE { get; set; }

    public string? UPDATE_BY { get; set; }

    public DateTime? UPDATE_DATE { get; set; }
}
