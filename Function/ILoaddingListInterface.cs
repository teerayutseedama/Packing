using DocumentFormat.OpenXml.Vml;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Packing.Models;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using System.Globalization;
using System.Linq.Expressions;

namespace Packing.Function
{
    public interface ILoaddingListInterface
    {
        Task<IEnumerable<LoaddingDataView>> GetLoaddingDataViews(GetLoaddingData data);
    }
    public class LoaddingListInterface:ILoaddingListInterface {
        private readonly vms_packingContext _context;
        private readonly VMS_CORE_2Context _context2;
        private ResponseMessage _response;
        public LoaddingListInterface(vms_packingContext context, VMS_CORE_2Context context2)
        {
            _context=context;
            _context2=context2;
            _response=new ResponseMessage();
        }

        public async Task<IEnumerable<LoaddingDataView>> GetLoaddingDataViews(GetLoaddingData data)
        {
            try
            {

                //CultureInfo englishCulture = new CultureInfo("en-US");
                var st =SystemClass.ToTimeStampTz(DateTime.Now);
                var sts = DateTime.Now;

                //foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
                //{
                //  string sssss= z.Id.ToString();
                //}
                var status = await _context.tbm_pk_batch_status.FirstOrDefaultAsync(x => x.BATCH_STATUS == "HOLD");
                var istatus = await _context.tbm_pk_batch_status.ToListAsync();
                List<LoaddingDataView> result = new List<LoaddingDataView>();
                Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.BATCH_STATUS == null || x.BATCH_STATUS == status.ID!);
                Expression<Func<tbm_plant, bool>> plantConds = PredicateBuilder.New<tbm_plant>(true);
                Expression<Func<tbm_material, bool>> materialConds = PredicateBuilder.New<tbm_material>(true);
                if (data.MaterialCode != null)
                {
                    batchConds = batchConds.And(x => x.MATERIAL_CODE == data.MaterialCode);
                }
                if (data.MaterialName != null)
                {
                    materialConds= materialConds.And(x => x.MATERIAL_NAME == data.MaterialName);
                }
                if (data.BatchNo != null)
                {
                    batchConds = batchConds.And(x => x.BATCH_NO == data.BatchNo);
                }
                if (data.MFGDate != null)
                {
                    batchConds = batchConds.And(x => x.MFG_DATE.Date == data.MFGDate.Value.Date);
                }
                //var test = await _context.tbt_pk_batch_no_header.Where(batchConds).ToListAsync();
                var pl = await _context2.tbm_plants.FirstOrDefaultAsync(plantConds);
                var list = await (from nh in _context.tbt_pk_batch_no_header.Where(batchConds)
                                  from m in _context.tbm_material.Where(x => x.MATERIAL_CODE == nh.MATERIAL_CODE)
                                  from sloc in _context.tbm_pk_sloc.Where(x => x.ID.ToString() == m.SLOC_ID)
                                  from pdl in _context.tbm_pk_production_line.Where(x => x.PACKING_LINE_ID == nh.PACKING_LINE_ID)
                                  select new LoaddingDataView
                                  {
                                      MaterialName = m.MATERIAL_NAME,
                                      Package = nh.PACKAGE,
                                      MFGDate = nh.MFG_DATE.ToString("dd/MM/yyyy"),
                                      Qty = nh.QTY_TOTAL,
                                      UOM = nh.UOM,
                                      BatchNo = nh.BATCH_NO,
                                      SubBatch = nh.SUB_BATCH,
                                      Plant = pl!.PLANT_NAME,
                                      Line = pdl.PK_LINE_NAME,
                                      status = nh.BATCH_STATUS,
                                  }).Distinct().ToListAsync();

                result = list.OrderBy(x => x.BatchNo).ThenBy(x=>x.SubBatch).ToList();


                return result;
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                throw;
            }
           
        }
    }

}
