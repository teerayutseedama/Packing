using DocumentFormat.OpenXml.Vml;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Packing.Models;
using Packing.Views;
using Packing.Views.DataView;
using Packing.vmsPackingDB;
using System.IO.Packaging;
using System.Linq.Expressions;

namespace Packing.Function
{
    public interface IApprovalInterface
    {
        Task<IEnumerable<ApprovalDataView>> GetApprovalDataViews(GetApprovalData data);
        Task<ResponseMessage> SaveApprovalData(List<SaveApprovalData> datas);
    }
    public class ApprovalInterface: IApprovalInterface
    {
        private readonly vms_packingContext _context;
        private readonly VMS_CORE_2Context _context2;
        private ResponseMessage _response;
        public ApprovalInterface(vms_packingContext context, VMS_CORE_2Context context2)
        {
            _context = context;
            _context2 = context2;
            _response = new ResponseMessage();
        }

        public async Task<IEnumerable<ApprovalDataView>> GetApprovalDataViews(GetApprovalData data)
        {
            try
            {
                var pl = await _context2.tbm_plants.ToListAsync();

                var ret = await (from head in _context.tbt_pk_batch_no_header.Where(h => (data.Status == null || h.BATCH_STATUS.ToString() == data.Status) && (data.BatchNo == null || h.BATCH_NO == data.BatchNo) && h.BATCH_STATUS != null)
                                 join mat in _context.tbm_material.Where(m => (data.MaterialCode == null || m.MATERIAL_CODE == data.MaterialCode) && (data.MaterialName == null || m.MATERIAL_NAME == data.MaterialName)) on head.MATERIAL_CODE equals mat.MATERIAL_CODE
                                 join pdl in _context.tbm_pk_production_line.Where(l => data.Line == null || l.PACKING_LINE_ID.ToString() == data.Line) on head.PACKING_LINE_ID equals pdl.PACKING_LINE_ID
                                 join sloc in _context.tbm_pk_sloc on mat.SLOC_ID equals sloc.ID.ToString()
                                 //join plant in pl on head.PLANT_ID equals plant.PLANT
                                 join ws in _context.tbm_pk_work_shift.Where(s => data.Shift == null || s.ID.ToString() == data.Shift) on head.WORK_SHIFT_ID equals ws.ID
                                 join st in _context.tbm_pk_batch_status on head.BATCH_STATUS equals st.ID
                                 select new ApprovalDataView
                                 {
                                     BatchNo = head.BATCH_NO,
                                     BatchSub = head.SUB_BATCH,
                                     //Plant = pl.FirstOrDefault(x => x.PLANT == head.PLANT_ID)!.PLANT_NAME,
                                     //Plant = plant.PLANT_NAME,
                                     sloc = sloc.SLOC,
                                     Line = pdl.PK_LINE_NAME,
                                     MaterialCode = head.MATERIAL_CODE,
                                     MaterialName = mat.MATERIAL_NAME,
                                     Package = head.PACKAGE,
                                     MFGDate = head.MFG_DATE.ToString("dd/MM/yyyy"),
                                     ExpireDate = head.EXPIRE_DATE.ToString("dd/MM/yyyy"),
                                     Shift = ws.WORK_SHIFT,
                                     RunNo = head.QTY_FROM.ToString() + " - " + head.QTY_TO.ToString(),
                                     Qty = head.QTY_TOTAL,
                                     UOM = head.UOM,
                                     id_status = st.ID,
                                     Status = st.BATCH_STATUS == null ? "WIP" : st.BATCH_STATUS,
                                 }).Distinct().ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //List<ApprovalDataView> result = new List<ApprovalDataView>();
        //Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.BATCH_STATUS != null && x.APPROVE_BY == null);
        //Expression<Func<tbm_plant, bool>> plantConds = PredicateBuilder.New<tbm_plant>(true);
        //Expression<Func<tbm_material, bool>> materialConds = PredicateBuilder.New<tbm_material>(true);

        //if (data.MaterialCode != null)
        //{
        //    batchConds = batchConds.And(x => x.MATERIAL_CODE == data.MaterialCode);
        //}
        //if (data.MaterialName != null)
        //{
        //    materialConds= materialConds.And(x => x.MATERIAL_NAME == data.MaterialName);
        //}
        //if (data.BatchNo != null)
        //{
        //    batchConds = batchConds.And(x => x.BATCH_NO == data.BatchNo);
        //}
        //if (data.MFGDate != null)
        //{
        //    batchConds = batchConds.And(x => x.MFG_DATE.Date == data.MFGDate.Value.Date );
        //}
        ////var test = await _context.tbt_pk_batch_no_header.Where(batchConds).ToListAsync();
        //var pl = await _context2.tbm_plants.FirstOrDefaultAsync(plantConds);
        //var list = await(from nh in _context.tbt_pk_batch_no_header.Where(batchConds)
        //                 from m in _context.tbm_material.Where(x=>x.MATERIAL_CODE==nh.MATERIAL_CODE)
        //                 from pdl in _context.tbm_pk_production_line.Where(x => x.PACKING_LINE_ID == nh.PACKING_LINE_ID)
        //                 from b in _context.tbm_pk_batch_status.Where(x => x.ID == nh.BATCH_STATUS).DefaultIfEmpty()
        //                 select new ApprovalDataView
        //                 {
        //                    BatchNo=nh.BATCH_NO,
        //                     BatchSub=nh.SUB_BATCH,
        //                    Plant = pl.PLANT_NAME,
        //                     MaterialName = m.MATERIAL_NAME,
        //                     Line = pdl.PK_LINE_NAME,
        //                     Package = nh.PACKAGE,
        //                     RunNo = m.FORM_NO,
        //                     Qty = nh.QTY_TOTAL,
        //                     UOM = nh.UOM,
        //                     ExpireDate=nh.EXPIRE_DATE.ToString("dd/MM/yyyy"),
        //                     Stataus=b.BATCH_STATUS,
        //                     MFGDate=nh.MFG_DATE.ToString("dd/MM/yyyy"),

        //                 }).Distinct().ToListAsync();

        //result = list.OrderBy(x => x.BatchNo).ToList();


        //return result;
    //}

        public async Task<ResponseMessage> SaveApprovalData(List<SaveApprovalData> datas)
        {
            try
            {
                var status = await _context.tbm_pk_batch_status.ToListAsync();
                var updateStatus = datas.FirstOrDefault()!.Status;
                var checkStatus = status.FirstOrDefault(x => x.ID == updateStatus)!.BATCH_STATUS;
                var user = datas.FirstOrDefault()!.User;
                var bathNOs = datas.Select(x => x.BatchNo).ToList();

                foreach (var item in datas)
                {
                    var batchs = await _context.tbt_pk_batch_no_header.FirstOrDefaultAsync(x => x.SUB_BATCH==item.BatchSub  && x.BATCH_NO==item.BatchNo);
                    var batchDetails = await _context.tbt_pk_batch_no_detail.Where(x => x.SUB_BATCH == item.BatchSub && x.BATCH_NO == item.BatchNo).ToListAsync();
                    if (batchs!=null  && batchDetails.Count > 0)
                    {

                        batchs.BATCH_STATUS = updateStatus;
                        batchs.APPROVE_DATE = DateTime.Now;
                        batchs.APPROVE_BY = user;
                       
                        batchDetails.ForEach(x =>
                        {
                            x.BATCH_STATUS = updateStatus;
                            x.APPROVE_DATE = DateTime.Now;
                            x.APPROVE_BY = user;
                            if (batchs.BATCH_STATUS == status.FirstOrDefault(x => x.BATCH_STATUS == "HOLD")!.ID  && checkStatus == "PASS")
                            {
                                x.REMARK_HOLD = datas.FirstOrDefault()!.Remark;
                            }
                            if (checkStatus == "REJECT")
                            {
                                x.REMARK_REJECT = datas.FirstOrDefault()!.Remark;
                            }
                            if (checkStatus == "HOLD")
                            {
                                x.REMARK_REJECT = datas.FirstOrDefault()!.Remark;
                                x.REMARK_HOLD = datas.FirstOrDefault()!.Remark;
                            }
                        });
                        _context.tbt_pk_batch_no_detail.UpdateRange(batchDetails);
                        _context.tbt_pk_batch_no_header.Update(batchs);
                        _response.Status = await _context.SaveChangesAsync() > 0;
                    }
                }
               
                return _response;

            }
            catch (Exception ex)
            {
                _response.Error = ex.Message;
                _response.Status = false;
                return _response;
               
            }

        }
    }
}
