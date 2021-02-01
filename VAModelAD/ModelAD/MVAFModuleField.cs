using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVAFModuleField : X_VAF_ModuleField
    {
        public MVAFModuleField(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MVAFModuleField(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleField WHERE VAF_Field_ID=" + GetVAF_Field_ID() + " AND VAF_ModuleTab_ID=" + GetVAF_ModuleTab_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "FieldExist"));
                    return false;
                }
            }
            else
            {
                // check if field value changed for previous record
                if (Is_ValueChanged("VAF_Field_ID"))
                {
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleField WHERE VAF_Field_ID=" + GetVAF_Field_ID() + " AND VAF_ModuleTab_ID=" + GetVAF_ModuleTab_ID())) > 0)
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
