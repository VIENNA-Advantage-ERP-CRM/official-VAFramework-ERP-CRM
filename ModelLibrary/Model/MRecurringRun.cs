/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRecurringRun 
 * Purpose        : MRecurring Run  Model
 * Class Used     :  X_C_Recurring_Run
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
    public class MRecurringRun : X_C_Recurring_Run
    {

        public MRecurringRun(Ctx ctx, int C_Recurring_Run_ID, Trx trxName)
            : base(ctx, C_Recurring_Run_ID, trxName)
        {

        }	//	MRecurringRun

        public MRecurringRun(Ctx ctx, MRecurring recurring)
            : base(ctx, 0, recurring.Get_TrxName())
        {

            if (recurring != null)
            {
                SetAD_Client_ID(recurring.GetAD_Client_ID());
                SetAD_Org_ID(recurring.GetAD_Org_ID());
                SetC_Recurring_ID(recurring.GetC_Recurring_ID());
                SetDateDoc(recurring.GetDateNextRun());
            }
        }	//	MRecurringRun

        public MRecurringRun(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {


        }
    }	




}
