using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAFModuleProcess:X_VAF_ModuleProcess
    {
        public MVAFModuleProcess(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        public MVAFModuleProcess(Ctx ctx, int dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleProcess WHERE VAF_Job_ID=" + GetVAF_Job_ID() + " AND VAF_ModuleInfo_ID=" + GetVAF_ModuleInfo_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "ProcessExist"));
                    return false;
                }
            }
            else
            {
                // check if process value changed for previous record
                if (Is_ValueChanged("VAF_Job_ID"))
                {
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleProcess WHERE VAF_Job_ID=" + GetVAF_Job_ID() + " AND VAF_ModuleInfo_ID=" + GetVAF_ModuleInfo_ID())) > 0)
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
