using System;
using System.Collections.Generic;

namespace Packing.vmsPackingDB;

public partial class tbm_pk_batch_slip
{
    public int id { get; set; }
    public string? STICKER_WIDTH { get; set; }

    public string? STICKER_HEIGH { get; set; }

    public string? QR_CODE_WIDTH { get; set; }

    public string? QR_CODE_HEIGHT { get; set; }

    public string? FONT_SIZE { get; set; }

    public string? RUNNING_FONT_SIZE { get; set; }

    public string? FORM_NO_SIZE { get; set; }

    public string? QR_CODE_SIZE_UNIT { get; set; }
}
