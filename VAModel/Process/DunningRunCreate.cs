/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DunningRunCreate
 * Purpose        : Create Dunning Run Entries/Lines
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak          10-Nov-2009
  ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class DunningRunCreate : ProcessEngine.SvrProcess
    {
        #region Private Variable
        private Boolean _IncludeInDispute = false;
        private Boolean _OnlySOTrx = false;
        private Boolean _IsAllCurrencies = false;
        private int _SalesRep_ID = 0;
        private int _C_Currency_ID = 0;
        private int _C_BPartner_ID = 0;
        private int _C_BP_Group_ID = 0;
        private int _C_DunningRun_ID = 0;
        private MDunningRun _run = null;
        private MDunningLevel _level = null;

        int C_Invoice_ID = 0; // Invoice id for dunning creation.
        int C_Currency_ID = 0; // Curency id of invoice.
        int DaysDue = 0; // Due days left.
        int C_BPartner_ID = 0; // Customer ID.
        int PaySchedule_ID = 0; // Payment schedule id.
        Decimal GrandTotal = 0; // Grand total of invoice.
        Decimal Open = 0; // invoice open amount.
        bool IsInDispute = false; // parameter value input to include dispute invoice or not.
        int timesDunned = 0; // No of times dunned before the current dunning run date.
        int daysAfterLast = 0; // Days after last dunning happened for invoice.

        StringBuilder sql = new StringBuilder();
        #endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("IncludeInDispute"))
                {
                    _IncludeInDispute = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("OnlySOTrx"))
                {
                    _OnlySOTrx = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("IsAllCurrencies"))
                {
                    _IsAllCurrencies = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("SalesRep_ID"))
                {
                    _SalesRep_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_Currency_ID"))
                {
                    _C_Currency_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_DunningRun_ID = GetRecord_ID();
        }

        /// <summary>
        /// 	Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("C_DunningRun_ID=" + _C_DunningRun_ID
                + ", Dispute=" + _IncludeInDispute
                + ", C_BP_Group_ID=" + _C_BP_Group_ID
                + ", C_BPartner_ID=" + _C_BPartner_ID);
            _run = new MDunningRun(GetCtx(), _C_DunningRun_ID, Get_TrxName());
            if (_run.Get_ID() == 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "NotFndDunRun"));
            }
            if (!_run.DeleteEntries(true))
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "NotDelExistEntry"));
            }
            if (_SalesRep_ID == 0)
            {
                //throw new ArgumentException("No SalesRep");
            }
            if (_C_Currency_ID == 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "NoCurrency"));
            }

            // Pickup the Runlevel
            _level = _run.GetLevel();

            // add up all invoices
            int inv = AddInvoices();
            // add up all payments
            int pay = AddPayments();

            // If the level should charge a fee do it now...
            // Add charge line only if any invoice line is added on run line.
            if (inv > 0 && _level.IsChargeFee())
            {
                AddFees();
            }
            if (_level.IsChargeInterest())
            {
                AddFees();
            }

            // we need to check whether this is a statement or not and some other rules
            CheckDunningEntry();

            int entries = 0;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader("SELECT count(*) FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + _run.Get_ID(), null, Get_TrxName());
                if (idr.Read())
                {
                    entries = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "countResults", e);
            }

            return "@C_DunningRunEntry_ID@ #" + entries;
        }


        /// <summary>
        /// Add Invoices to Run tab of dunning run window.
        /// </summary>
        /// <returns>No of invoices that are created.</returns>
        private int AddInvoices()
        {
            //int size = 3;
            int count = 0;

            SqlParameter[] param = null;
            // Changes for Invoice schedule if valid true

            if (!_level.IsShowNotDue())
            {
                param = new SqlParameter[3];
            }
            else
            {
                param = new SqlParameter[1];
            }

            sql.Append("SELECT I.C_INVOICE_ID,  I.C_CURRENCY_ID, I.GRANDTOTAL, INVOICEOPEN(I.C_INVOICE_ID,Ips.C_INVOICEPAYSCHEDULE_ID),  "
                    + " DAYSBETWEEN(@param1,IPS.DUEDATE),"
                    + " i.IsInDispute, i.C_BPartner_ID, "
                    + " CASE "
                    + " WHEN i.issotrx   ='Y' AND i.isreturntrx='N'" // Invoice Customer
                    + " THEN ips.DueAmt"
                    + " WHEN i.issotrx   ='Y' AND i.isreturntrx='Y'" // AR Credit Memo
                    + " THEN ips.DueAmt * -1"
                    + " WHEN i.issotrx   ='N' AND i.isreturntrx='N'" // Invoice Vendor
                    + " THEN ips.DueAmt * -1"
                    + " ELSE ips.DueAmt"                             // AP Credit Memo
                    + " END AS DueAmt,"
                    + " ips.C_INVOICEPAYSCHEDULE_ID"
                    + " FROM C_Invoice i "
                    + " LEFT JOIN C_INVOICEPAYSCHEDULE ips "
                    + " ON ips.c_invoice_id               =i.c_invoice_id "
                    + " WHERE ips.VA009_IsPaid                    ='N' "
                    + " AND i.AD_Client_ID                = " + _run.GetAD_Client_ID()
                    + " AND i.AD_Org_ID                   = " + _run.GetAD_Org_ID()
                    + " AND i.DocStatus                  IN ('CO','CL') "
                    + " AND (NOT i.InvoiceCollectionType IN ('" + X_C_Invoice.INVOICECOLLECTIONTYPE_CollectionAgency + "', "
                    + "'" + X_C_Invoice.INVOICECOLLECTIONTYPE_LegalProcedure + "', '" + X_C_Invoice.INVOICECOLLECTIONTYPE_Uncollectable + "')"
                    + " OR InvoiceCollectionType IS NULL) ");
            if (_SalesRep_ID > 0)
            {
                sql.Append(" AND i.SalesRep_ID= " + _SalesRep_ID);
            }
            if (!_level.IsShowNotDue())
            {
                sql.Append(" AND IPS.DUEDATE                  <=@param2 "
                + " AND DATEINVOICED                 <=@param3 ");
            }
            sql.Append(" AND IPS.ISVALID='Y' "
             + " AND i.ispayschedulevalid='Y' "
             + " AND EXISTS "
                 + " (SELECT * "
                     + " FROM C_DunningLevel dl "
                     + " WHERE dl.C_DunningLevel_ID= " + _run.GetC_DunningLevel_ID()
                     + " AND dl.C_Dunning_ID      IN "
                         + " (SELECT COALESCE(bp.C_Dunning_ID, bpg.C_Dunning_ID) "
                         + " FROM C_BPartner bp "
                         + " INNER JOIN C_BP_Group bpg "
                         + " ON (bp.C_BP_Group_ID =bpg.C_BP_Group_ID) "
                         + " WHERE i.C_BPartner_ID=bp.C_BPartner_ID "
                         + " ) "
                 + " )");


            if (!_level.IsShowAllDue())
            {
                param[0] = new SqlParameter("@param1", _run.GetDunningDate());
                if (!_level.IsShowNotDue())
                {
                    param[1] = new SqlParameter("@param2", _run.GetDunningDate().Value.AddDays(Util.GetValueOfInt(-1 * _level.GetDaysAfterDue())));
                    param[2] = new SqlParameter("@param3", _run.GetDunningDate().Value.AddDays(Util.GetValueOfInt(-1 * _level.GetDaysAfterDue())));
                }

            }
            else
            {
                param[0] = new SqlParameter("@param1", _run.GetDunningDate());
                if (!_level.IsShowNotDue())
                {
                    param[1] = new SqlParameter("@param2", _run.GetDunningDate());
                    param[2] = new SqlParameter("@param3", _run.GetDunningDate());
                }

            }

            // for specific Business Partner
            if (_C_BPartner_ID != 0)
            {
                //size = size + 1;
                sql.Append(" AND i.C_BPartner_ID=" + _C_BPartner_ID);	//	##6
            }
            // or a specific group
            if (_C_BP_Group_ID != 0)
            {
                //size = size + 1;
                sql.Append(" AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE i.C_BPartner_ID=bp.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")");	//	##;
            }
            // Only Sales Trx
            if (_OnlySOTrx)
            {
                sql.Append(" AND i.IsSOTrx='Y'");
            }
            // Only single currency
            if (!_IsAllCurrencies)
            {
                sql.Append(" AND i.C_Currency_ID=" + _C_Currency_ID);
            }
            //	log.info(sql);

            String sql2 = null;

            // if sequentially we must check for other levels with smaller days for
            // which this invoice is not yet included!
            if (_level.GetParent().IsCreateLevelsSequentially())
            {
                // Build a list of all topmost Dunning Levels
                MDunningLevel[] previousLevels = _level.GetPreviousLevels();
                if (previousLevels != null && previousLevels.Length > 0)
                {

                    for (int i = 0; i < previousLevels.Length; i++)
                    {
                        sql.Append(" AND i.C_Invoice_ID IN (SELECT C_Invoice_ID FROM C_DunningRunLine WHERE " +
                        "C_DunningRunEntry_ID IN (SELECT C_DunningRunEntry_ID FROM C_DunningRunEntry WHERE " +
                        "C_DunningRun_ID IN (SELECT C_DunningRun_ID FROM C_DunningRun WHERE " +
                        "C_DunningLevel_ID=" + previousLevels[i].Get_ID() + ")) AND Processed<>'N')");
                    }

                }
            }
            // ensure that we do only dunn what's not yet dunned, so we lookup the max of last Dunn Date which was processed
            sql2 = "SELECT COUNT(*), COALESCE(DAYSBETWEEN(MAX(dr2.DunningDate), MAX(dr.DunningDate)),0)"
                + "FROM C_DunningRun dr2, C_DunningRun dr"
                + " INNER JOIN C_DunningRunEntry dre ON (dr.C_DunningRun_ID=dre.C_DunningRun_ID)"
                + " INNER JOIN C_DunningRunLine drl ON (dre.C_DunningRunEntry_ID=drl.C_DunningRunEntry_ID) "
                + "WHERE drl.Processed='Y' AND dr2.C_DunningRun_ID=@C_DunningRun_ID AND drl.C_Invoice_ID=@C_Invoice_ID AND C_InvoicePaySchedule_ID=@C_InvoicePaySchedule_ID AND dr.C_DunningRun_ID!=@C_DunningRun_ID"; // ##1 ##2

            Decimal DaysAfterDue = _run.GetLevel().GetDaysAfterDue();
            int DaysBetweenDunning = _run.GetLevel().GetDaysBetweenDunning();

            IDataReader idr = null;
            IDataReader idr1 = null;
            DataTable dt = null;
            DataTable dt1 = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                
                sql.Clear();
                //
                foreach (DataRow dr in dt.Rows)
                // while (idr.Read())
                {
                    C_Invoice_ID = 0; C_Currency_ID = 0; GrandTotal = 0; Open = 0; DaysDue = 0;
                    IsInDispute = false; C_BPartner_ID = 0; PaySchedule_ID = 0;

                    C_Invoice_ID = Utility.Util.GetValueOfInt(dr[0]);
                    C_Currency_ID = Utility.Util.GetValueOfInt(dr[1]);
                    GrandTotal = Utility.Util.GetValueOfDecimal(dr[2]);
                    Open = Utility.Util.GetValueOfDecimal(dr[7]);
                    DaysDue = Utility.Util.GetValueOfInt(dr[4]);
                    IsInDispute = "Y".Equals(Utility.Util.GetValueOfString(dr[5]));
                    C_BPartner_ID = Utility.Util.GetValueOfInt(dr[6]);
                    PaySchedule_ID = Utility.Util.GetValueOfInt(dr[8]);

                    //
                    // Check for Dispute
                    if (!_IncludeInDispute && IsInDispute)
                    {
                        continue;
                    }

                    // Check for an open amount
                    if (Env.ZERO.CompareTo(Open) == 0)
                    {
                        continue;
                    }
                    //
                    timesDunned = 0;
                    daysAfterLast = 0;

                    //2nd record set
                    SqlParameter[] param1 = new SqlParameter[3];
                    param1[0] = new SqlParameter("@C_DunningRun_ID", _run.Get_ID());
                    param1[1] = new SqlParameter("@C_Invoice_ID", C_Invoice_ID);
                    param1[2] = new SqlParameter("@C_InvoicePaySchedule_ID", PaySchedule_ID);
                    idr1 = DataBase.DB.ExecuteReader(sql2, param1, Get_TrxName());
                    dt1 = new DataTable();
                    dt1.Load(idr);
                    idr1.Close();
                    //	SubQuery
                    foreach (DataRow dr1 in dt1.Rows)
                    // if (idr1.Read())
                    {
                        timesDunned = Utility.Util.GetValueOfInt(dr1[0]);
                        daysAfterLast = Utility.Util.GetValueOfInt(dr1[1]);
                    }
                    //idr1.Close();
                    //	SubQuery

                    // Ensure that Daysbetween Dunning is enforced
                    // Ensure Not ShowAllDue and Not ShowNotDue is selected
                    // PROBLEM: If you have ShowAll activated then DaysBetweenDunning is not working, because we don't know whether
                    //          there is something which we really must Dunn.
                    //if (DaysBetweenDunning != 0 && daysAfterLast != 0 && daysAfterLast < DaysBetweenDunning && !_level.IsShowAllDue() && !_level.IsShowNotDue())
                    if (DaysBetweenDunning != 0 && daysAfterLast != 0 && daysAfterLast < DaysBetweenDunning)
                    {
                        continue;
                    }


                    // We will minus the timesDunned if this is the DaysBetweenDunning is not fullfilled.
                    // Remember in checkup later we must reset them!
                    // See also checkDunningEntry()
                    if (daysAfterLast < DaysBetweenDunning)
                    {
                        timesDunned = timesDunned * -1;
                    }
                    //
                    if (CreateInvoiceLine(C_Invoice_ID, C_Currency_ID, GrandTotal, Open,
                        DaysDue, IsInDispute, C_BPartner_ID,
                        timesDunned, daysAfterLast, PaySchedule_ID))
                    {
                        count++;
                    }
                }
               // idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (idr1 != null)
                {
                    idr1.Close();
                }
                log.Log(Level.SEVERE, "addInvoices", e);
            }

            return count;
        }

        /// <summary>
        /// Create Invoice Line on run tab of dunning run window.
        /// </summary>
        /// <param name="C_Invoice_ID">Invoice ID.</param>
        /// <param name="C_Currency_ID">Currency ID.</param>
        /// <param name="GrandTotal">Grand Total.</param>
        /// <param name="Open">Amount.</param>
        /// <param name="DaysDue">Days Due.</param>
        /// <param name="IsInDispute">In Dispute.</param>
        /// <param name="C_BPartner_ID">Business Partner.</param>
        /// <param name="timesDunned">Number of times dunning occurred.</param>
        /// <param name="daysAfterLast">Days after last dunning.</param>
        private bool CreateInvoiceLine(int C_Invoice_ID, int C_Currency_ID,
            Decimal GrandTotal, Decimal Open,
            int DaysDue, bool IsInDispute,
            int C_BPartner_ID, int timesDunned, int daysAfterLast, int PaySchedule_ID)
        {
            MDunningRunEntry entry = _run.GetEntry(C_BPartner_ID, _C_Currency_ID, _SalesRep_ID);
            if (entry.Get_ID() == 0)
            {
                if (!entry.Save())
                {
                    // Change by mohit to pick error message from last error in mclass.
                    //throw new Exception(GetRetrievedError(entry, "Cannot save MDunningRunEntry"));
                    //throw new Exception("Cannot save MDunningRunEntry");
                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunEntry"), vp.GetName());
                    }
                    else
                    {
                        log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunEntry"), "");
                    }
                    return false;
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetInvoice(C_Invoice_ID, C_Currency_ID, GrandTotal, Open,
                new Decimal(0), DaysDue, IsInDispute, timesDunned,
                daysAfterLast);
            line.SetC_InvoicePaySchedule_ID(PaySchedule_ID);
            if (!line.Save())
            {
                // Change by mohit to pick error message from last error in mclass.
                //throw new Exception(GetRetrievedError(line, "Cannot save MDunningRunLine"));
                //throw new Exception("Cannot save MDunningRunLine");
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {

                    log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), vp.GetName());
                }
                else
                {
                    log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), "");
                }
                return false;
            }
            return true;
        }


        /// <summary>
        /// Add Payments to Run tab of dunning run window.
        /// </summary>
        /// <returns>no of payments inserted.</returns>
        private int AddPayments()
        {
            int C_Payment_ID = 0;
            Decimal PayAmt = 0;
            Decimal openAmt = 0;

            sql.Append("SELECT C_Payment_ID, C_Currency_ID, PayAmt,"
               + " paymentAvailable(C_Payment_ID), C_BPartner_ID "
               + "FROM C_Payment_v p "
               + "WHERE AD_Client_ID=" + _run.GetAD_Client_ID()			//	##1
               + " AND AD_Org_ID=" + _run.GetAD_Org_ID()
               + " AND IsAllocated='N' AND C_BPartner_ID IS NOT NULL"
               + " AND C_Charge_ID IS NULL"
               + " AND DocStatus IN ('CO','CL')"
                //	Only BP with Dunning defined
               + " AND EXISTS (SELECT * FROM C_BPartner bp "
                   + "WHERE p.C_BPartner_ID=bp.C_BPartner_ID"
                   + " AND bp.C_Dunning_ID=(SELECT C_Dunning_ID FROM C_DunningLevel WHERE C_DunningLevel_ID=" + _run.GetC_DunningLevel_ID() + "))");	// ##2
            if (_C_BPartner_ID != 0)
            {
                sql.Append(" AND C_BPartner_ID=" + _C_BPartner_ID);		//	##3
            }
            else if (_C_BP_Group_ID != 0)
            {
                sql.Append(" AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE p.C_BPartner_ID=bp.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")");	//	##3
            }
            // If it is not a statement we will add lines only if InvoiceLines exists,
            // because we do not want to dunn for money we owe the customer!
            if (!_level.GetDaysAfterDue().Equals(new Decimal(-9999)))
            {
                sql.Append(" AND C_BPartner_ID IN (SELECT C_BPartner_ID FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + _run.Get_ID() + ")");
            }
            // show only receipts / if only Sales
            if (_OnlySOTrx)
            {
                sql.Append(" AND IsReceipt='Y'");
            }

            int count = 0;
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
              
                    sql.Clear();
                foreach (DataRow dr in dt.Rows)

                    //while (idr.Read())
                {
                    C_Payment_ID = 0; C_Currency_ID = 0; PayAmt = 0; openAmt = 0; C_BPartner_ID = 0;

                    C_Payment_ID = Utility.Util.GetValueOfInt(dr[0]);
                    C_Currency_ID = Utility.Util.GetValueOfInt(dr[1]);
                    PayAmt = Decimal.Negate(Utility.Util.GetValueOfDecimal(dr[2]));//.getBigDecimal(3).negate();
                    openAmt = Decimal.Negate(Utility.Util.GetValueOfDecimal(dr[3]));// rs.getBigDecimal(4).negate();
                    C_BPartner_ID = Utility.Util.GetValueOfInt(dr[4]);//.getInt(5);

                    // checkup the amount
                    if (Env.ZERO.CompareTo(openAmt) == 0)
                    {
                        continue;
                    }
                    //
                    if (CreatePaymentLine(C_Payment_ID, C_Currency_ID, PayAmt, openAmt,
                        C_BPartner_ID))
                    {
                        count++;
                    }
                }
               // idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            return count;
        }

        /// <summary>
        /// Create Payment Line on run tab of dunning run window.
        /// </summary>
        /// <param name="C_Payment_ID">Payment ID.</param>
        /// <param name="C_Currency_ID">Currency ID.</param>
        /// <param name="PayAmt">Amount.</param>
        /// <param name="openAmt">Open.</param>
        /// <param name="C_BPartner_ID">Business Partner ID.</param>
        private bool CreatePaymentLine(int C_Payment_ID, int C_Currency_ID,
            Decimal PayAmt, Decimal openAmt, int C_BPartner_ID)
        {
            MDunningRunEntry entry = _run.GetEntry(C_BPartner_ID, _C_Currency_ID, _SalesRep_ID);
            if (entry.Get_ID() == 0)
            {
                if (!entry.Save())
                {
                    // Change by mohit to pick error message from last error in mclass.
                    //throw new Exception(GetRetrievedError(entry, "Cannot save MDunningRunEntry"));
                    //throw new Exception("Cannot save MDunningRunEntry");
                    //log.SaveError("Cannot save MDunningRunEntry", "");
                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        string val = vp.GetName();
                        log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunEntry"), val);
                    }
                    else
                    {
                        log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunEntry"), "");
                    }
                    return false;
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetPayment(C_Payment_ID, C_Currency_ID, PayAmt, openAmt);

            if (!line.Save())
            {
                // Change by mohit to pick error message from last error in mclass.
                //throw new Exception(GetRetrievedError(line, "Cannot save MDunningRunLine"));
                //throw new Exception("Cannot save MDunningRunLine");
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {

                    log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), vp.GetName());
                }
                else
                {
                    log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), "");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add fee line on run tab of dunning run window.
        /// </summary>
        private void AddFees()
        {
            // Only add a fee if it contains InvoiceLines and is not a statement
            // TODO: Assumes Statement = -9999 and 
            bool onlyInvoices = _level.GetDaysAfterDue().Equals(new Decimal(-9999));
            MDunningRunEntry[] entries = _run.GetEntries(true, onlyInvoices);
            if (entries != null && entries.Length > 0)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    MDunningRunLine line = new MDunningRunLine(entries[i]);
                    line.SetFee(_C_Currency_ID, _level.GetFeeAmt());
                    if (!line.Save())
                    {
                        // Change by mohit to pick error message from last error in mclass.
                        //throw new Exception(GetRetrievedError(line, "Cannot save MDunningRunLine"));
                        //throw new Exception("Cannot save MDunningRunLine");
                        ValueNamePair vp = VLogger.RetrieveError();
                        if (vp != null)
                        {

                            log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), vp.GetName());
                        }
                        else
                        {
                            log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), "");
                        }
                        return;
                    }
                    entries[i].SetQty(Decimal.Subtract(entries[i].GetQty(), new Decimal(1)));
                }
            }
        }

        /// <summary>
        /// Check the dunning run
        /// 1) Check for following Rule: ShowAll should produce only a record if at least one new line is found
        /// </summary>
        private void CheckDunningEntry()
        {
            // Check rule 1)
            if (_level.IsShowAllDue())
            {
                MDunningRunEntry[] entries = _run.GetEntries(true);
                if (entries != null && entries.Length > 0)
                {
                    for (int i = 0; i < entries.Length; i++)
                    {
                        // We start with saying we delete this entry as long as we don't find something new
                        bool entryDelete = true;
                        MDunningRunLine[] lines = entries[i].GetLines(true);
                        for (int j = 0; j < lines.Length; j++)
                        {
                            if (lines[j].GetTimesDunned() < 0)
                            {
                                // We clean up the *-1 from line 255
                                lines[j].SetTimesDunned(lines[j].GetTimesDunned() * -1);
                                if (!lines[j].Save())
                                {
                                    // Change by mohit to pick error message from last error in mclass.
                                    //throw new Exception(GetRetrievedError(lines[j], "Cannot save MDunningRunLine"));
                                    ValueNamePair vp = VLogger.RetrieveError();
                                    if (vp != null)
                                    {

                                        log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), vp.GetName());
                                    }
                                    else
                                    {
                                        log.SaveError(Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"), "");
                                    }

                                }
                            }

                            else
                            {
                                // We found something new, so we would not save anything...
                                entryDelete = false;
                            }
                        }
                        if (entryDelete)
                        {
                            entries[i].Delete(false);
                        }
                    }
                }
            }
        }

    }

}
