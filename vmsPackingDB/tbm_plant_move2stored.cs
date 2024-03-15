using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_plant_move2stored
{
    public string PLANT { get; set; } = null!;

    public string? PLANT_NAME { get; set; }

    public bool? ACTIVE { get; set; }

    public string? CREATE_BY { get; set; }

    public DateTime? CREATE_DATE { get; set; }

    public string? UPDATE_BY { get; set; }

    public DateTime? UPDATE_DATE { get; set; }
}
