using System;
namespace Packing.Views.DataView
{
	public class LiDataView
	{
	
	}

	public class LiCheckBatchNo
	{
		public string? BatchNo { get; set; }
		public int SubBatch { get; set; }
	}

	public class LiCheckDate
	{
		public DateTime? ExpiredDate { get; set; }
		public string? Shift { get; set; }

	}
}

