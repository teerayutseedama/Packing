using DocumentFormat.OpenXml.Vml;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Packing.Models;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using System.Linq.Expressions;

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
            List<HistoryDataView> result = new List<HistoryDataView>();
            Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.CREATE_DATE.Value >= data.ToDate.Value && x.CREATE_DATE.Value <= data.FromDate.Value);
            Expression<Func<tbm_plant, bool>> plantConds = PredicateBuilder.New<tbm_plant>(true);
            Expression<Func<tbm_material, bool>> materialConds = PredicateBuilder.New<tbm_material>(true);
            if (data.Line != null)
            {
               // batchConds.And(x => x.PACKING_LINE_ID == data.Line);
            }
            if (data.MaterialCode != null)
            {
                batchConds.And(x => x.MATERIAL_CODE == data.MaterialCode);
            }
            if (data.MaterialName != null)
            {
                materialConds.And(x => x.MATERIAL_NAME == data.MaterialName);
            }
            if (data.BatchNo != null)
            {
                batchConds.And(x => x.BATCH_NO == data.BatchNo);
            }
            var list = await(from nh in _context.tbt_pk_batch_no_header.Where(batchConds)
                             from m in _context.tbm_material.Where(materialConds)
                             from sloc in _context.tbm_pk_sloc.Where(x=>x.ID.ToString()==m.SLOC_ID)
                             from pdl in _context.tbm_pk_production_line.Where(x=>x.PACKING_LINE_ID==nh.PACKING_LINE_ID)
                             from pl in _context2.tbm_plants.Where(plantConds)
                             from ws in _context.tbm_pk_work_shift.Where(x=>x.ID==nh.WORK_SHIFT_ID)
                            from b in _context.tbm_pk_batch_status.Where(x=>x.ID==nh.BATCH_STATUS)
                             select new HistoryDataView
                             {
                                 BatchNo = nh.BATCH_NO,
                                 Plant = pl.PLANT_NAME,
                                 Line = pdl.PK_LINE_NAME,
                                 MaterialCode = nh.MATERIAL_CODE,
                                 MaterialName = m.MATERIAL_NAME,
                                 Package = nh.PACKAGE,
                                 MFGDate = nh.MFG_DATE,
                                 Shift = ws.WORK_SHIFT,
                                 RunNo =m.FORM_NO,
                                 Qty =nh.QTY_TOTAL,
                                 UOM = nh.UOM,
                                 Stataus = b.BATCH_STATUS,
                             }).ToListAsync();
            result = list.OrderBy(x=>x.BatchNo).ToList();
            //foreach (var item in group)
            //{
            //    HistoryDataView view = new HistoryDataView();
            //    view.BatchNo = item.;
            //    view.Plant = item.;
            //    view.Line = item.;
            //    view.MaterialCode = item.;
            //    view.MaterialName = item.;
            //    view.Package = item.;
            //    view.MFGDate = item.;
            //    view.Shift = item.;
            //    view.RunNo = item.RunNo;
            //    view.Qty = item.Qty;
            //    view.UOM = item.UOM;
            //    view.Stataus = item.Stataus;
            //    result.Add(view);
            //}
            return result;
        }
    }
}
