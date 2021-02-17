/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RecurringScheduler
 * Purpose        : Run Scheduler for all Type of Recurring Process
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Arpit Rai           03-Jan-2017
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
using VAdvantage.ProcessEngine;

namespace ViennaAdvantageServer.Process
{
    public class RecurringScheduler : SvrProcess
    {
        #region Variables
        DateTime? dateDoc = null, dt = null, dt1 = null;
        MRecurring Recurring = null;
        DataSet ds = null;
        #endregion

        protected override void Prepare()
        {
            //  throw new NotImplementedException();
        }
        protected override string DoIt()
        {
            ds = new DataSet();

            ds = DB.ExecuteDataset("SELECT VAB_Recurring_ID From VAB_Recurring Where IsActive='Y' AND VAF_Client_ID=" + GetVAF_Client_ID()
                                  + " AND TRUNC(DateNextRun)=" + GlobalVariable.TO_DATE(DateTime.Now, true));
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (Int32 i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Recurring = new MRecurring(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_Recurring_ID"]), Get_Trx());
                    dateDoc = Recurring.GetDateNextRun();
                    if (dateDoc != null && Recurring.CalculateRuns())
                    {
                        //  throw new Exception("No Runs Left");
                        MRecurringRun run = new MRecurringRun(GetCtx(), Recurring);
                        String msg = "@Created@ ";
                        // if (Recurring.GetDateNextRun() == DateTime.Now.Date || Recurring.GetDateNextRun() == null)
                        //{
                        //	Copy
                        if (Recurring.GetRecurringType().Equals(MRecurring.RECURRINGTYPE_Order))
                        {
                            MVABOrder from = new MVABOrder(GetCtx(), Recurring.GetVAB_Order_ID(), Get_TrxName());
                            MVABOrder order = MVABOrder.CopyFrom(from, dateDoc,
                                from.GetVAB_DocTypes_ID(), false, false, Get_TrxName());
                            run.SetVAB_Order_ID(order.GetVAB_Order_ID());
                            msg += order.GetDocumentNo();
                        }
                        else if (Recurring.GetRecurringType().Equals(MRecurring.RECURRINGTYPE_Invoice))
                        {
                            MVABInvoice from = new MVABInvoice(GetCtx(), Recurring.GetVAB_Invoice_ID(), Get_TrxName());
                            MVABInvoice invoice = MVABInvoice.CopyFrom(from, dateDoc,
                                from.GetVAB_DocTypes_ID(), false, Get_TrxName(), false);
                            run.SetVAB_Invoice_ID(invoice.GetVAB_Invoice_ID());
                            msg += invoice.GetDocumentNo();
                        }
                        else if (Recurring.GetRecurringType().Equals(MRecurring.RECURRINGTYPE_Project))
                        {
                            MVABProject project = MVABProject.CopyFrom(GetCtx(), Recurring.GetVAB_Project_ID(), dateDoc, Get_TrxName());
                            run.SetVAB_Project_ID(project.GetVAB_Project_ID());
                            msg += project.GetValue();
                        }
                        else if (Recurring.GetRecurringType().Equals(MRecurring.RECURRINGTYPE_GLJournalBatch))
                        {
                            MJournalBatch journal = MJournalBatch.CopyFrom(GetCtx(), Recurring.GetVAGL_BatchJRNL_ID(), dateDoc, Get_TrxName());
                            run.SetVAGL_BatchJRNL_ID(journal.GetVAGL_BatchJRNL_ID());
                            msg += journal.GetDocumentNo();
                        }
                        else if (Recurring.GetRecurringType().Equals(MRecurring.RECURRINGTYPE_GLJournal))
                        {
                            MJournal Journal = MJournal.CopyFrom(GetCtx(), Recurring.GetVAGL_JRNL_ID(), dateDoc, Get_TrxName());
                            run.SetVAGL_JRNL_ID(Journal.GetVAGL_JRNL_ID());
                            msg += Journal.GetDocumentNo();
                        }
                        else if (Recurring.GetRecurringType().Equals(MRecurring.RECURRINGTYPE_Payment))
                        {
                            MVABPayment from = new MVABPayment(GetCtx(), Recurring.GetVAB_Payment_ID(), Get_TrxName());
                            MVABPayment payment = MVABPayment.CopyFrom(from, dateDoc,
                                from.GetVAB_DocTypes_ID(), Get_TrxName());
                            run.SetVAB_Payment_ID(payment.GetVAB_Payment_ID());
                            msg += payment.GetDocumentNo();
                        }
                        //else
                        //  return "Invalid @RecurringType@ = " + Recurring.GetRecurringType();
                        if (run.Save(Get_TrxName()))
                        {
                            Recurring.SetDateLastRun(run.GetUpdated());
                            Recurring.SetRunsRemaining(Recurring.GetRunsRemaining() - 1);
                            SetDateNextRun();
                            Recurring.Save(Get_TrxName());
                        }
                        // }
                    }
                }
            }
            DisposeVariables();
            return "";
            //  throw new NotImplementedException();
        }
        private void SetDateNextRun()
        {
            if (Recurring.GetFrequency() < 1)
            {
                Recurring.SetFrequency(1);
            }
            int frequency = Recurring.GetFrequency();
            dt = Recurring.GetDateNextRun();
            System.Globalization.GregorianCalendar gcal = new System.Globalization.GregorianCalendar();

            if (Recurring.GetFrequencyType().Equals("D"))
            {
                dt1 = dt.Value.AddDays(frequency);
            }
            else if (Recurring.GetFrequencyType().Equals("W"))
            {
                dt1 = dt.Value.AddDays(7 * frequency);
            }
            else if (Recurring.GetFrequencyType().Equals("M"))
            {
                dt1 = dt.Value.AddMonths(frequency);
            }
            else if (Recurring.GetFrequencyType().Equals("Q"))
            {
                dt1 = dt.Value.AddMonths(3 * frequency);
            }
            Recurring.SetDateNextRun(dt1);
        }
        private void DisposeVariables()
        {
            dateDoc = null;
            dt = null;
            dt1 = null;
            ds.Dispose();
            Recurring = null;
        }
    }
}
