using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_pk_work_shift
{
    public int ID { get; set; }

    public string WORK_SHIFT { get; set; } = null!;

    public DateTime? TIME_START { get; set; }

    public DateTime? TIME_END { get; set; }
}
