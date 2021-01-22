using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
   public class MModuleTab:X_VAF_ModuleTab
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
               if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleTab WHERE VAF_Tab_ID=" + GetVAF_Tab_ID() + " AND VAF_ModuleWindow_ID=" + GetVAF_ModuleWindow_ID())) > 0)
               {
                   log.SaveError("Error", Msg.GetMsg(GetCtx(), "TabExist"));
                   return false;
               }
           }
           else
           {
               // check if tab value changed for previous record
               if (Is_ValueChanged("VAF_Tab_ID"))
               {
                   // check if child record exist for same tab
                   if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleField WHERE VAF_ModuleTab_ID=" + GetVAF_ModuleTab_ID())) > 0)
                   {
                       log.SaveWarning("", Msg.GetMsg(GetCtx(), "ChildTabExist"));
                       return false;
                   }
                   // check if same record is inserting again
                   if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleTab WHERE VAF_Tab_ID=" + GetVAF_Tab_ID() + " AND VAF_ModuleWindow_ID=" + GetVAF_ModuleWindow_ID())) > 0)
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
