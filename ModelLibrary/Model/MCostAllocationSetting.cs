using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MCostAllocationSetting : X_M_CostAllocationSetting
    {
        public MCostAllocationSetting(Ctx ctx, int M_CostAllocationSetting_ID, Trx trx) : base(ctx, M_CostAllocationSetting_ID, trx) { }
        public MCostAllocationSetting(Ctx ctx, DataRow dr, Trx trx) : base(ctx, dr, trx) { }

        protected override bool BeforeSave(bool newRecord)
        {
            string sql = "select * from M_COSTALLOCATIONSETTING where c_DocType_ID=" + GetC_DocType_ID() + " and InvRef_DocType_ID=" + GetInvRef_DocType_ID() + " and M_CostAllocationSetting_ID !="+GetM_CostAllocationSetting_ID();
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {


                    log.SaveWarning("Warning", "Can’t save the record with the same document type Payment and Invoice");
                    return false;


                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

    }
}
