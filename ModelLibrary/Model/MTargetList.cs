using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.Web.Hosting;
using System.Web;
using System.Security.Cryptography;
using System.Web.Security;
using ViennaAdvantage.Model;
using VAdvantage.Classes;

namespace VAdvantage.Model
{
    class MTargetList : X_C_TargetList
    {
        public MTargetList(Context ctx, int C_TargetList_ID, Trx trxName)
            : base(ctx, C_TargetList_ID, trxName)
        {
            
        }


        public MTargetList(Ctx ctx, int C_TargetList_ID, Trx trxName)
               : base(ctx, C_TargetList_ID, trxName)
        {
           
        }
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetC_Lead_ID() == 0 && GetC_BPartner_ID() == 0 && GetRef_BPartner_ID() == 0)
            {
                throw new Exception(Msg.GetMsg(GetCtx(), "VIS_PleaseFillColumn"));
               
            }
            return true;// Msg.GetMsg(GetCtx(), "Record Done");
        }
    }
}
