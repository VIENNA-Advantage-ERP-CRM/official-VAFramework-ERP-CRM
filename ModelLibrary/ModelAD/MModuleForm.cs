using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MModuleForm : X_AD_ModuleForm
    {
        public MModuleForm(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MModuleForm(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleForm WHERE AD_Form_ID=" + GetAD_Form_ID() + " AND AD_ModuleInfo_ID=" + GetAD_ModuleInfo_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "FormExist"));
                    return false;
                }
            }
            else
            {
                // check if form value changed for previous record
                if (Is_ValueChanged("AD_Form_ID"))
                {
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleForm WHERE AD_Form_ID=" + GetAD_Form_ID() + " AND AD_ModuleInfo_ID=" + GetAD_ModuleInfo_ID())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "FormExist"));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
