/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRecurringRun 
 * Purpose        : MRecurring Run  Model
 * Class Used     :  X_VAB_RecurringRun
 * Chronological    Development
 * Deepak           03-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;



namespace VAdvantage.Model
{
    public class MVABRecurringRun : X_VAB_RecurringRun
    {

        public MVABRecurringRun(Ctx ctx, int VAB_RecurringRun_ID, Trx trxName)
            : base(ctx, VAB_RecurringRun_ID, trxName)
        {

        }	//	MRecurringRun

        public MVABRecurringRun(Ctx ctx, MVABRecurring recurring)
            : base(ctx, 0, recurring.Get_TrxName())
        {

            if (recurring != null)
            {
                SetVAF_Client_ID(recurring.GetVAF_Client_ID());
                SetVAF_Org_ID(recurring.GetVAF_Org_ID());
                SetVAB_Recurring_ID(recurring.GetVAB_Recurring_ID());
                SetDateDoc(recurring.GetDateNextRun());
            }
        }	//	MRecurringRun

        public MVABRecurringRun(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {


        }
    }	




}
