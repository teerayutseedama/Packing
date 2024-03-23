using LinqKit;
using Packing.Models;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Packing.Function
{
    public interface ISummaryInterface
    {
        Task<IEnumerable<SummaryDataView>> GetSummaryList(GetSummaryDataView data);
    }

    public class SummaryInterface : ISummaryInterface
    {
        private readonly vms_packingContext _context;
        private readonly VMS_CORE_2Context _context2;
        private ResponseMessage _response;
        public SummaryInterface(VMS_CORE_2Context _2Context,vms_packingContext context) {
            _response = new ResponseMessage();
            _context= context;
            _context2 = _2Context;
        }

        public async Task<IEnumerable<SummaryDataView>> GetSummaryList(GetSummaryDataView data)
        {
            try
            {
                List<SummaryDataView> result = new List<SummaryDataView>();
                Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.CREATE_DATE.Value.Date >= data.FromDate.Value.Date && x.CREATE_DATE.Value.Date <= data.ToDate.Value.Date);
                Expression<Func<tbm_plant, bool>> plantConds = PredicateBuilder.New<tbm_plant>(true);
                Expression<Func<tbm_material, bool>> materialConds = PredicateBuilder.New<tbm_material>(true);
                if (data.Line != null)
                {
                    batchConds= batchConds.And(x => x.PACKING_LINE_ID == data.Line);
                }
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
             
                var list = await (from nh in _context.tbt_pk_batch_no_header.Where(batchConds)
                                  from m in _context.tbm_material.Where(x=>x.MATERIAL_CODE==nh.MATERIAL_CODE)
                                  select new
                                  {
                                      MaterialCode = nh.MATERIAL_CODE,
                                      MaterialGroup = m.MATERIAL_GROUP,
                                      MaterialName = m.MATERIAL_NAME,
                                      Package = nh.PACKAGE,
                                      MFGDate = nh.MFG_DATE,
                                      ExpireDate = nh.EXPIRE_DATE,
                                      Qty = nh.QTY_TOTAL,
                                      UOM = nh.UOM
                                  }).Distinct().ToListAsync();

                var group = list.GroupBy(x => new { x.MaterialCode, x.MaterialGroup, x.MaterialName, x.Package, x.MFGDate, x.ExpireDate, x.UOM });

                foreach (var item in group)
                {
                    SummaryDataView view = new SummaryDataView();
                    view.MaterialCode = item.Key.MaterialCode;
                    view.MaterialGroup = item.Key.MaterialGroup;
                    view.MaterialName = item.Key.MaterialName;
                    view.Package = item.Key.Package;
                    view.MFGDate = item.Key.MFGDate.ToString("dd/MM/yyyy");
                    view.ExpireDate = item.Key.ExpireDate.ToString("dd/MM/yyyy");
                    view.Qty = list.Where(x =>
                    x.MaterialCode == item.Key.MaterialCode
                    && x.MaterialGroup == item.Key.MaterialGroup
                    && x.MaterialName == item.Key.MaterialName
                    && x.Package == item.Key.Package
                    && x.MFGDate == item.Key.MFGDate
                    && x.ExpireDate == item.Key.ExpireDate
                    ).Sum(x => x.Qty);
                    view.UOM = item.Key.UOM;
                    result.Add(view);
                }
                return result.AsEnumerable();
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
                throw;
            }
          
        }
    

    
    }
}
