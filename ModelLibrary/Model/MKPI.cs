using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Classes;
using System.Data.SqlClient;
using VAdvantage.Process;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
   public class MKPI : X_RC_KPI
    {

        public MKPI(Context ctx, int RC_KPI_ID, Trx trxName)
            : base(ctx, RC_KPI_ID, trxName)
        {

        }
        public MKPI(Ctx ctx, int RC_KPI_ID, Trx trxName)
            : base(ctx, RC_KPI_ID, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {

            if (IsView())
            {
                SetAD_Table_ID(-1);
            }
            else
            {
                SetTableView_ID(-1);
            }

            return true;
        }


    }
}
