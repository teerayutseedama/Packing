using DocumentFormat.OpenXml.Vml;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Packing.Models;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using System.Data.Common;
using System.IO.Packaging;
using System.Linq;
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
            var status=await _context.tbm_pk_batch_status.FirstOrDefaultAsync(x=>x.BATCH_STATUS== "HOLD");
            List<LoaddingDataView> result = new List<LoaddingDataView>();
            Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.BATCH_STATUS==null && x.BATCH_STATUS== status.ID!);
            Expression<Func<tbm_plant, bool>> plantConds = PredicateBuilder.New<tbm_plant>(true);
            Expression<Func<tbm_material, bool>> materialConds = PredicateBuilder.New<tbm_material>(true);
         
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
            if(data.MFGDate != null)
            {
                batchConds.And(x => x.CREATE_DATE.Value.ToString() == data.MFGDate); 
            }
            var list = await(from nh in _context.tbt_pk_batch_no_header.Where(batchConds)
                             from m in _context.tbm_material.Where(materialConds)
                             from sloc in _context.tbm_pk_sloc.Where(x => x.ID.ToString() == m.SLOC_ID)
                             from pdl in _context.tbm_pk_production_line.Where(x => x.PACKING_LINE_ID == nh.PACKING_LINE_ID)
                             from pl in _context2.tbm_plants.Where(plantConds)
                             from ws in _context.tbm_pk_work_shift.Where(x => x.ID == nh.WORK_SHIFT_ID)
                             from b in _context.tbm_pk_batch_status.Where(x => x.ID == nh.BATCH_STATUS)
                             select new LoaddingDataView
                             {
                                 MaterialName = m.MATERIAL_NAME,
                                 Package = nh.PACKAGE,
                                 MFGDate = nh.MFG_DATE,
                                 Qty = nh.QTY_TOTAL,
                                 UOM = nh.UOM,
                                  BatchNo="",
                                  SubBatch = "",
                                 Plant = "",
                                 Line = "",
                                 RunNo = "",
                             }).ToListAsync();

            result = list.OrderBy(x=>x.BatchNo).ToList();

            //foreach (var item in group)
            //{
            //    LoaddingDataView view = new LoaddingDataView();
            //    view.BatchNo = "";
            //    view.SubBatch = "";
            //    view.Plant = "";
            //    view.MaterialName = "";
            //    view.Package = "";
            //    view.Line = "";
            //    view.MFGDate = "";
            //    view.RunNo = "";
            //    view.Qty = "";
            //    view.UOM = "";
            //    result.Add(view);
            //}
            return result;
        }
    }

}
