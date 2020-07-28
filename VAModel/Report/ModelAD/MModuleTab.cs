using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
   public class MModuleTab:X_AD_ModuleTab
    {

       public MModuleTab(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

       public MModuleTab(Ctx ctx, int dr, Trx trxName)
           : base(ctx, dr, trxName)
       {
       }

       protected override bool BeforeSave(bool newRecord)
       {
           if (newRecord)
           {
               // check if same record is inserting again
               if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleTab WHERE AD_Tab_ID=" + GetAD_Tab_ID() + " AND AD_ModuleWindow_ID=" + GetAD_ModuleWindow_ID())) > 0)
               {
                   log.SaveError("Error", Msg.GetMsg(GetCtx(), "TabExist"));
                   return false;
               }
           }
           else
           {
               // check if tab value changed for previous record
               if (Is_ValueChanged("AD_Tab_ID"))
               {
                   // check if child record exist for same tab
                   if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleField WHERE AD_ModuleTab_ID=" + GetAD_ModuleTab_ID())) > 0)
                   {
                       log.SaveWarning("", Msg.GetMsg(GetCtx(), "ChildTabExist"));
                       return false;
                   }
                   // check if same record is inserting again
                   if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleTab WHERE AD_Tab_ID=" + GetAD_Tab_ID() + " AND AD_ModuleWindow_ID=" + GetAD_ModuleWindow_ID())) > 0)
                   {
                       log.SaveError("Error", Msg.GetMsg(GetCtx(), "TabExist"));
                       return false;
                   }
               }
           }
           return true;
       }

    }
}
