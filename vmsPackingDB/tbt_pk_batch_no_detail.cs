using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbt_pk_batch_no_detail
{
    public int id { get; set; }
    public string BATCH_NO { get; set; } = null!;

    public int SUB_BATCH { get; set; }

    public int BATCH_RUNNING_NO { get; set; }

    /// <summary>
    /// กะการทำงาน -&gt; tbm_pk_work_shift
    /// </summary>
    public int WORK_SHIFT_ID { get; set; }

    /// <summary>
    /// Pass/Hold/Reject -&gt; tbm_pk_batch_status
    /// </summary>
    public int? BATCH_STATUS { get; set; }

    public string? REMARK_REJECT { get; set; }

    public string? REMARK_HOLD { get; set; }

    public string? REMARK_HOLD_TO_PASS { get; set; }

    public string? CREATE_BY { get; set; }

    public DateTime? CREATE_DATE { get; set; }

    public string? UPDATE_BY { get; set; }

    public DateTime? UPDATE_DATE { get; set; }

    public string? APPROVE_BY { get; set; }

    public DateTime? APPROVE_DATE { get; set; }
}
