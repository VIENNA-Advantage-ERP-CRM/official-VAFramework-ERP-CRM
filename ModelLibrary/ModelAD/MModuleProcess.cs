using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MModuleProcess:X_AD_ModuleProcess
    {
        public MModuleProcess(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        public MModuleProcess(Ctx ctx, int dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleProcess WHERE AD_Process_ID=" + GetAD_Process_ID() + " AND AD_ModuleInfo_ID=" + GetAD_ModuleInfo_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "ProcessExist"));
                    return false;
                }
            }
            else
            {
                // check if process value changed for previous record
                if (Is_ValueChanged("AD_Process_ID"))
                {
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleProcess WHERE AD_Process_ID=" + GetAD_Process_ID() + " AND AD_ModuleInfo_ID=" + GetAD_ModuleInfo_ID())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "ProcessExist"));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
