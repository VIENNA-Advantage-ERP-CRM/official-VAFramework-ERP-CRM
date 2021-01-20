using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MModuleWindow : X_VAF_ModuleWindow
    {
        public MModuleWindow(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MModuleWindow(Ctx ctx, int dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleWindow WHERE AD_Window_ID=" + GetAD_Window_ID() + " AND VAF_ModuleInfo_ID=" + GetVAF_ModuleInfo_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "WindowExist"));
                    return false;
                }
            }
            else
            {
                // check if window value changed for previous record
                if (Is_ValueChanged("AD_Window_ID"))
                {
                    // check if child record exist for same tab
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleTab WHERE VAF_ModuleWindow_ID=" + GetVAF_ModuleWindow_ID())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "ChildTabExist"));
                        return false;
                    }
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleWindow WHERE AD_Window_ID=" + GetAD_Window_ID() + " AND VAF_ModuleInfo_ID=" + GetVAF_ModuleInfo_ID())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "WindowExist"));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
