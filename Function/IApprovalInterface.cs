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
           
            List<ApprovalDataView> result = new List<ApprovalDataView>();
            Expression<Func<tbt_pk_batch_no_header, bool>> batchConds = PredicateBuilder.New<tbt_pk_batch_no_header>(x => x.BATCH_STATUS != 1 && x.APPROVE_BY == null);
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
                batchConds = batchConds.And(x => x.MFG_DATE.Date == data.MFGDate.Value.Date );
            }
            var test = await _context.tbt_pk_batch_no_header.Where(batchConds).ToListAsync();
            var pl = await _context2.tbm_plants.FirstOrDefaultAsync(plantConds);
            var list = await(from nh in _context.tbt_pk_batch_no_header.Where(batchConds)
                             from m in _context.tbm_material.Where(x=>x.MATERIAL_CODE==nh.MATERIAL_CODE)
                             from pdl in _context.tbm_pk_production_line.Where(x => x.PACKING_LINE_ID == nh.PACKING_LINE_ID)
                             from b in _context.tbm_pk_batch_status.Where(x => x.ID == nh.BATCH_STATUS).DefaultIfEmpty()
                             select new ApprovalDataView
                             {
                                BatchNo=nh.BATCH_NO,
                                 BatchSub=nh.SUB_BATCH,
                                Plant = pl.PLANT_NAME,
                                 MaterialName = m.MATERIAL_NAME,
                                 Line = pdl.PK_LINE_NAME,
                                 Package = nh.PACKAGE,
                                 RunNo = m.FORM_NO,
                                 Qty = nh.QTY_TOTAL,
                                 UOM = nh.UOM,
                                 ExpireDate=nh.EXPIRE_DATE.ToString("dd/MM/yyyy"),
                                 Stataus=b.BATCH_STATUS,
                                 MFGDate=nh.MFG_DATE.ToString("dd/MM/yyyy"),

                             }).Distinct().ToListAsync();

            result = list.OrderBy(x => x.BatchNo).ToList();

    
            return result;
        }

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
