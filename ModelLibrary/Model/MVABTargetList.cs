using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.DataBase;

using ViennaAdvantage.Model;
using VAdvantage.Classes;

namespace VAdvantage.Model
{
    class MVABTargetList : X_VAB_TargetList
    {
        public MVABTargetList(Context ctx, int C_TargetList_ID, Trx trxName)
            : base(ctx, C_TargetList_ID, trxName)
        {
            
        }


        public MVABTargetList(Ctx ctx, int C_TargetList_ID, Trx trxName)
               : base(ctx, C_TargetList_ID, trxName)
        {
           
        }
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetVAB_Lead_ID() == 0 && GetVAB_BusinessPartner_ID() == 0 && GetRef_BPartner_ID() == 0)
            {
                throw new Exception(Msg.GetMsg(GetCtx(), "VIS_PleaseFillColumn"));
               
            }
            return true;// Msg.GetMsg(GetCtx(), "Record Done");
        }
    }
}
