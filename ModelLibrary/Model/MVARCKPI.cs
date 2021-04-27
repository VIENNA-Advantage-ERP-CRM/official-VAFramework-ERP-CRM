﻿using System;
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
   public class MVARCKPI : X_VARC_KPI
    {

        public MVARCKPI(Context ctx, int VARC_KPI_ID, Trx trxName)
            : base(ctx, VARC_KPI_ID, trxName)
        {

        }
        public MVARCKPI(Ctx ctx, int VARC_KPI_ID, Trx trxName)
            : base(ctx, VARC_KPI_ID, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {

            if (IsView())
            {
                SetVAF_TableView_ID(-1);
            }
            else
            {
                SetTableView_ID(-1);
            }

            return true;
        }


    }
}