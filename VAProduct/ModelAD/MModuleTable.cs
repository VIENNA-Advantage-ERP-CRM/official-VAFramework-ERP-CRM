using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Classes;

namespace VAdvantage.Model
{
    class MModuleTable : X_AD_ModuleTable
    {

        public MModuleTable(Context ctx, int AD_ModuleTable_ID, Trx trxName)
            : base(ctx, AD_ModuleTable_ID, trxName)
        {

        }
        public MModuleTable(Ctx ctx, int AD_ModuleTable_ID, Trx trxName)
            : base(ctx, AD_ModuleTable_ID, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleTable WHERE AD_Table_ID=" + GetAD_Table_ID() + " AND AD_ModuleInfo_ID=" + GetAD_ModuleInfo_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "TableExist"));
                    return false;
                }
            }
            else
            {
                // check if table value changed for previous record
                if (Is_ValueChanged("AD_Table_ID"))
                {
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_ModuleTable WHERE AD_Table_ID=" + GetAD_Table_ID() + " AND AD_ModuleInfo_ID=" + GetAD_ModuleInfo_ID())) > 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "TableExist"));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
