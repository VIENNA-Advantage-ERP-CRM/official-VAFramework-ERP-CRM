using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class MVAMProductCostType : X_VAM_ProductCostType
    {
        public MVAMProductCostType(Ctx ctx, int VAM_ProductCostType_ID, Trx trxName)
            : base(ctx, VAM_ProductCostType_ID, trxName)
        {

        }	//	MVAMProductCostType

        public MVAMProductCostType(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MVAMProductCostType

        protected override bool BeforeSave(bool newRecord)
        {
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            return true;
        }

        protected override bool BeforeDelete()
        {
            MVABAccountBook[] ass = MVABAccountBook.GetClientAcctSchema(GetCtx(), GetVAF_Client_ID());
            for (int i = 0; i < ass.Length; i++)
            {
                if (ass[i].GetVAM_ProductCostType_ID() == GetVAM_ProductCostType_ID())
                {
                    log.SaveError("CannotDelete", Msg.GetElement(GetCtx(), "VAB_AccountBook_ID") + " - " + ass[i].GetName());
                    return false;
                }
            }
            return true;
        }	//	beforeDelete


        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMProductCostType[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }	//	toString
    }
}
