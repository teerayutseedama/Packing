using DocumentFormat.OpenXml.Vml;
using LinqKit;
using Packing.Models;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Packing.Function
{
    public interface IHistoryInterface
    {
        Task<IEnumerable<HistoryDataView>> GetHistoryDataViews(GetHistoryData data);        
    }

    public class HistoryInterface:IHistoryInterface
    {
        private readonly vms_packingContext _context;
        private readonly VMS_CORE_2Context _context2;
        private ResponseMessage _response;
        public HistoryInterface(vms_packingContext context , VMS_CORE_2Context context2) {
            _context = context;
            _context2 = context2;
        }

        public async Task<IEnumerable<HistoryDataView>> GetHistoryDataViews(GetHistoryData data)
        {
            try
            {
                List<HistoryDataView> result = new List<HistoryDataView>();
                Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.MFG_DATE.Date >= data.FromDate.Value.Date && x.MFG_DATE.Date <= data.ToDate.Value.Date);
                Expression<Func<tbm_plant, bool>> plantConds = PredicateBuilder.New<tbm_plant>(true);
                Expression<Func<tbm_material, bool>> materialConds = PredicateBuilder.New<tbm_material>(true);
                if (data.Line != null)
                {
                    batchConds.And(x => x.PACKING_LINE_ID.ToString() == data.Line);
                }
                if (data.MaterialCode != null)
                {
                    batchConds = batchConds.And(x => x.MATERIAL_CODE == data.MaterialCode);
                }
                if (data.MaterialName != null)
                {
                    materialConds = materialConds.And(x => x.MATERIAL_NAME == data.MaterialName);
                }
                if (data.BatchNo != null)
                {
                    batchConds = batchConds.And(x => x.BATCH_NO == data.BatchNo);
                }

                //var pl = await _context2.tbm_plants.FirstOrDefaultAsync(plantConds);
                var pl = await _context2.tbm_plants.ToListAsync();

                var ret = await (from head in _context.tbt_pk_batch_no_header.Where((h => data.Status == null || h.BATCH_STATUS.ToString() == data.Status))
                                 join mat in _context.tbm_material.Where(m => (data.MaterialCode == null || m.MATERIAL_CODE == data.MaterialCode) && (data.MaterialName == null || m.MATERIAL_NAME == data.MaterialName)) on head.MATERIAL_CODE equals mat.MATERIAL_CODE
                                 join pdl in _context.tbm_pk_production_line.Where(l => data.Line == null || l.PACKING_LINE_ID.ToString() == data.Line) on head.PACKING_LINE_ID equals pdl.PACKING_LINE_ID
                                 //join plant in pl on pdl.PLANT_ID.ToString() equals plant.PLANT
                                 join ws in _context.tbm_pk_work_shift.Where(s => data.Shift == null || s.ID.ToString() == data.Shift) on head.WORK_SHIFT_ID equals ws.ID
                                 join st in _context.tbm_pk_batch_status on head.BATCH_STATUS equals st.ID into sx
                                 from st in sx.DefaultIfEmpty()
                                 select new HistoryDataView
                                 {
                                     BatchNo = head.BATCH_NO,
                                     sub_batch = head.SUB_BATCH,
                                     //Plant = pl.FirstOrDefault(x => x.PLANT == pdl.PLANT_ID.ToString()).PLANT_NAME.ToString(),
                                     //Plant = plant.PLANT_NAME,
                                     Line = pdl.PK_LINE_NAME,
                                     MaterialCode = head.MATERIAL_CODE,
                                     MaterialName = mat.MATERIAL_NAME,
                                     Package = head.PACKAGE,
                                     MFGDate = head.MFG_DATE.ToString("dd/MM/yyyy"),
                                     Shift = ws.WORK_SHIFT,
                                     RunNo = head.QTY_FROM.ToString() + " - " + head.QTY_TO.ToString(), 
                                     Qty = head.QTY_TOTAL,
                                     UOM = head.UOM,
                                     Stataus = st.BATCH_STATUS == null ? "WIP" : st.BATCH_STATUS,
                                 }).Distinct().ToListAsync();
                return ret;
                //var list = await (from nh in _context.tbt_pk_batch_no_header
                //                  //from runno in minmax.Where(x => x.batch_no == nh.BATCH_NO && x.sub_batch == nh.SUB_BATCH)
                //                  from m in _context.tbm_material.Where(x => x.MATERIAL_CODE == nh.MATERIAL_CODE)
                //                  from pdl in _context.tbm_pk_production_line.Where(x => x.PACKING_LINE_ID == nh.PACKING_LINE_ID)
                //                  //from plant in pl.Where(x => Convert.ToUInt32( x.PLANT) == pdl.PLANT_ID)
                //                  from ws in _context.tbm_pk_work_shift.Where(x => x.ID == nh.WORK_SHIFT_ID).DefaultIfEmpty()
                //                  from b in _context.tbm_pk_batch_status.Where(x => x.ID == nh.BATCH_STATUS).DefaultIfEmpty()
                //                  select new HistoryDataView
                //                  {
                //                      BatchNo = nh.BATCH_NO,
                //                      //Plant = plant.PLANT_NAME,
                //                      Line = pdl.PK_LINE_NAME,
                //                      MaterialCode = nh.MATERIAL_CODE,
                //                      MaterialName = m.MATERIAL_NAME,
                //                      Package = nh.PACKAGE,
                //                      MFGDate = nh.MFG_DATE.ToString("dd/MM/yyyy"),
                //                      Shift = ws.WORK_SHIFT,
                //                      RunNo = minmax.FirstOrDefault(x => x.batch_no == nh.BATCH_NO && x.sub_batch == nh.SUB_BATCH)!.mn.ToString(),
                //                      Qty = nh.QTY_TOTAL,
                //                      UOM = nh.UOM,
                //                      Stataus = b.BATCH_STATUS,
                //                  }).Distinct().ToListAsync();
                //result = list.OrderBy(x => x.BatchNo).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
