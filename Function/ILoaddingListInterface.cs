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
                var pl = await _context2.tbm_plants.ToListAsync();

                var ret = await (from head in _context.tbt_pk_batch_no_header.Where(h => (data.BatchNo == null || h.BATCH_NO == data.BatchNo) && h.MFG_DATE == data.MFGDate && h.APPROVE_BY == null )
                                 join mat in _context.tbm_material.Where(m => (data.MaterialCode == null || m.MATERIAL_CODE == data.MaterialCode) && (data.MaterialName == null || m.MATERIAL_NAME == data.MaterialName)) on head.MATERIAL_CODE equals mat.MATERIAL_CODE
                                 join pdl in _context.tbm_pk_production_line.Where(l => data.Line == null || l.PACKING_LINE_ID.ToString() == data.Line) on head.PACKING_LINE_ID equals pdl.PACKING_LINE_ID
                                 join sloc in _context.tbm_pk_sloc on mat.SLOC_ID equals sloc.ID.ToString()
                                 //join plant in pl on head.PLANT_ID equals plant.PLANT
                                 join ws in _context.tbm_pk_work_shift.Where(s => data.Shift == null || s.ID.ToString() == data.Shift) on head.WORK_SHIFT_ID equals ws.ID
                                 join st in _context.tbm_pk_batch_status on head.BATCH_STATUS equals st.ID into sx
                                 from st in sx.DefaultIfEmpty()
                                 select new LoaddingDataView
                                 {
                                     BatchNo = head.BATCH_NO,
                                     SubBatch = head.SUB_BATCH,
                                     //Plant = pl.FirstOrDefault(x => x.PLANT == head.PLANT_ID)!.PLANT_NAME,
                                     Plant = pdl.PLANT_ID ==1 ? "TVO1": pdl.PLANT_ID == 2 ? "TVO2" : pdl.PLANT_ID == 3 ? "TVO3" :"",
                                     id_status = head.BATCH_STATUS == null ? 0 : head.BATCH_STATUS,
                                     sloc = sloc.SLOC,
                                     Line = pdl.PK_LINE_NAME,
                                     MaterialCode = head.MATERIAL_CODE,
                                     MaterialName = mat.MATERIAL_NAME,
                                     Package = head.PACKAGE,
                                     MFGDate = head.MFG_DATE.ToString("dd/MM/yyyy"),
                                     Shift = ws.WORK_SHIFT,
                                     RunNo = head.QTY_FROM.ToString() + " - " + head.QTY_TO.ToString(),
                                     Qty = head.QTY_TOTAL,
                                     UOM = head.UOM,                                     
                                     Status = st.BATCH_STATUS == null ? "WIP" : st.BATCH_STATUS + " [รออนุมัติ]",
                                 }).Distinct().ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                throw;
            }

            //try
            //{

            //    //CultureInfo englishCulture = new CultureInfo("en-US");
            //    var st =SystemClass.ToTimeStampTz(DateTime.Now);
            //    var sts = DateTime.Now;

            //    //foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
            //    //{
            //    //  string sssss= z.Id.ToString();
            //    //}
            //    var status = await _context.tbm_pk_batch_status.FirstOrDefaultAsync(x => x.BATCH_STATUS == "HOLD");
            //    var istatus = await _context.tbm_pk_batch_status.ToListAsync();
            //    List<LoaddingDataView> result = new List<LoaddingDataView>();
            //    Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.BATCH_STATUS == null || x.BATCH_STATUS == status.ID!);
            //    Expression<Func<tbm_plant, bool>> plantConds = PredicateBuilder.New<tbm_plant>(true);
            //    Expression<Func<tbm_material, bool>> materialConds = PredicateBuilder.New<tbm_material>(true);
            //    if (data.MaterialCode != null)
            //    {
            //        batchConds = batchConds.And(x => x.MATERIAL_CODE == data.MaterialCode);
            //    }
            //    if (data.MaterialName != null)
            //    {
            //        materialConds= materialConds.And(x => x.MATERIAL_NAME == data.MaterialName);
            //    }
            //    if (data.BatchNo != null)
            //    {
            //        batchConds = batchConds.And(x => x.BATCH_NO == data.BatchNo);
            //    }
            //    if (data.MFGDate != null)
            //    {
            //        batchConds = batchConds.And(x => x.MFG_DATE.Date == data.MFGDate.Value.Date);
            //    }
            //    //var test = await _context.tbt_pk_batch_no_header.Where(batchConds).ToListAsync();
            //    var pl = await _context2.tbm_plants.FirstOrDefaultAsync(plantConds);
            //    var list = await (from nh in _context.tbt_pk_batch_no_header.Where(batchConds)
            //                      from m in _context.tbm_material.Where(x => x.MATERIAL_CODE == nh.MATERIAL_CODE)
            //                      from sloc in _context.tbm_pk_sloc.Where(x => x.ID.ToString() == m.SLOC_ID)
            //                      from pdl in _context.tbm_pk_production_line.Where(x => x.PACKING_LINE_ID == nh.PACKING_LINE_ID)
            //                      select new LoaddingDataView
            //                      {
            //                          MaterialName = m.MATERIAL_NAME,
            //                          Package = nh.PACKAGE,
            //                          MFGDate = nh.MFG_DATE.ToString("dd/MM/yyyy"),
            //                          Qty = nh.QTY_TOTAL,
            //                          UOM = nh.UOM,
            //                          BatchNo = nh.BATCH_NO,
            //                          SubBatch = nh.SUB_BATCH,
            //                          Plant = pl!.PLANT_NAME,
            //                          Line = pdl.PK_LINE_NAME,
            //                          status = nh.BATCH_STATUS,
            //                      }).Distinct().ToListAsync();

            //    result = list.OrderBy(x => x.BatchNo).ThenBy(x=>x.SubBatch).ToList();


            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    string mess = ex.Message;
            //    throw;
            //}

        }
    }

}
