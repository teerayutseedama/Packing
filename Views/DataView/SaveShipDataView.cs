using System;
namespace Packing.Views.DataView
{
	public class SaveShipDataView
	{

        public int ID { get; set; }

        public string WORK_SHIFT { get; set; } = null!;

        public DateTime? TIME_START { get; set; }

        public DateTime? TIME_END { get; set; }
    
	}

    public class UpdateBatchShiftDatView
    {
        public string? STICKER_WIDTH { get; set; }

        public string? STICKER_HEIGH { get; set; }

        public string? QR_CODE_WIDTH { get; set; }

        public string? QR_CODE_HEIGHT { get; set; }

        public string? FONT_SIZE { get; set; }

        public string? RUNNING_FONT_SIZE { get; set; }

        public string? FORM_NO_SIZE { get; set; }

        public string? QR_CODE_SIZE_UNIT { get; set; }
    }
}

