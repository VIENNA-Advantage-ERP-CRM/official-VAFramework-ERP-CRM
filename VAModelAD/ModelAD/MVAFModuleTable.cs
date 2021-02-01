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
    class MVAFModuleTable : X_VAF_ModuleTable
    {

        public MVAFModuleTable(Context ctx, int VAF_ModuleTable_ID, Trx trxName)
            : base(ctx, VAF_ModuleTable_ID, trxName)
        {

        }
        public MVAFModuleTable(Ctx ctx, int VAF_ModuleTable_ID, Trx trxName)
            : base(ctx, VAF_ModuleTable_ID, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
            {
                // check if same record is inserting again
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleTable WHERE VAF_TableView_ID=" + GetVAF_TableView_ID() + " AND VAF_ModuleInfo_ID=" + GetVAF_ModuleInfo_ID())) > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "TableExist"));
                    return false;
                }
            }
            else
            {
                // check if table value changed for previous record
                if (Is_ValueChanged("VAF_TableView_ID"))
                {
                    // check if same record is inserting again
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM VAF_ModuleTable WHERE VAF_TableView_ID=" + GetVAF_TableView_ID() + " AND VAF_ModuleInfo_ID=" + GetVAF_ModuleInfo_ID())) > 0)
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
