using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MModuleField : X_AD_ModuleField
    {
        public MModuleField(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MModuleField(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleField WHERE AD_Field_ID=" + GetAD_Field_ID() + " AND AD_ModuleTab_ID=" + GetAD_ModuleTab_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "FieldExist"));
                    return false;
                }
            }
            else
            {
                // check if field value changed for previous record
                if (Is_ValueChanged("AD_Field_ID"))
                {
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleField WHERE AD_Field_ID=" + GetAD_Field_ID() + " AND AD_ModuleTab_ID=" + GetAD_ModuleTab_ID())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "FieldExist"));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
