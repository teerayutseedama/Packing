using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_pk_production_line
{
    public int PACKING_LINE_ID { get; set; }

    /// <summary>
    /// join to tbm_plant
    /// </summary>
    public int PLANT_ID { get; set; }

    /// <summary>
    /// join to tbm_pk_sloc
    /// </summary>
    public int SLOC_ID { get; set; }

    public string PK_LINE_NAME { get; set; } = null!;
}
