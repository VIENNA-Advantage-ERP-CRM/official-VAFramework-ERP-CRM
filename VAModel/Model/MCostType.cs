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
    public class MCostType : X_M_CostType
    {
        public MCostType(Ctx ctx, int M_CostType_ID, Trx trxName)
            : base(ctx, M_CostType_ID, trxName)
        {

        }	//	MCostType

        public MCostType(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MCostType

        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
            return true;
        }

        protected override bool BeforeDelete()
        {
            MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(GetCtx(), GetAD_Client_ID());
            for (int i = 0; i < ass.Length; i++)
            {
                if (ass[i].GetM_CostType_ID() == GetM_CostType_ID())
                {
                    log.SaveError("CannotDelete", Msg.GetElement(GetCtx(), "C_AcctSchema_ID") + " - " + ass[i].GetName());
                    return false;
                }
            }
            return true;
        }	//	beforeDelete


        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MCostType[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }	//	toString
    }
}
