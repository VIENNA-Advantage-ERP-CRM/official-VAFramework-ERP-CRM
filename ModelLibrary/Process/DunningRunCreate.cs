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
using System.Windows.Forms;

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
        private Boolean _IncludePDC = false;
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
                else if (name.Equals("IsIncludePDC"))
                {
                    _IncludePDC = "Y".Equals(para[i].GetParameter());
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
            // add up all cash journal
            int cash = AddCashJournal();
            // add up all GL journal
            int GlJournal = AddGLJournal();
            //add up all post dated check if includepdc is true
            if (Env.IsModuleInstalled("VA027_") && _IncludePDC)
            {
                int pdc = AddPostDatedCheck();
            }

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

            sql.Append("SELECT I.C_INVOICE_ID,  I.C_CURRENCY_ID, INVOICEOPEN(I.C_INVOICE_ID,Ips.C_INVOICEPAYSCHEDULE_ID),  "
                    + " DAYSBETWEEN(@param1,IPS.DUEDATE) AS DaysDue ,"
                    + " i.IsInDispute, i.C_BPartner_ID, "
                    + "CASE " 
                    + " WHEN i.issotrx ='Y' AND i.isreturntrx='N' THEN ABS(I.GrandTotal ) "                        //Invoice Customer
                    + " WHEN i.issotrx ='Y' AND i.isreturntrx='Y' AND I.GrandTotal > 0 THEN I.GrandTotal * -1 "    //AR Credit Memo
                    + " WHEN i.issotrx ='N' AND i.isreturntrx ='N' AND I.GrandTotal > 0 THEN I.GrandTotal * -1 "   //Invoice Vendor
                    + " ELSE I.GrandTotal"                                                                         // AP Credit Memo
                    + " END AS GrandTotal,"
                    + " CASE "
                    + " WHEN i.issotrx   ='Y' AND i.isreturntrx='N' "                   // Invoice Customer
                    + " THEN ABS(ips.DueAmt )"
                    + " WHEN i.issotrx   ='Y' AND i.isreturntrx='Y' AND ips.DueAmt > 0" // AR Credit Memo
                    + " THEN ips.DueAmt * -1"
                    + " WHEN i.issotrx   ='N' AND i.isreturntrx='N' AND ips.DueAmt > 0" // Invoice Vendor
                    + " THEN ips.DueAmt * -1"
                    + " ELSE ips.DueAmt"                                                // AP Credit Memo
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

                    C_Invoice_ID = Utility.Util.GetValueOfInt(dr["C_Invoice_ID"]);
                    C_Currency_ID = Utility.Util.GetValueOfInt(dr["C_Currency_ID"]);
                    GrandTotal = Utility.Util.GetValueOfDecimal(dr["GrandTotal"]);
                    Open = Utility.Util.GetValueOfDecimal(dr["DueAmt"]);
                    DaysDue = Utility.Util.GetValueOfInt(dr["DaysDue"]);
                    IsInDispute = "Y".Equals(Utility.Util.GetValueOfString(dr["IsInDispute"]));
                    C_BPartner_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_ID"]);
                    PaySchedule_ID = Utility.Util.GetValueOfInt(dr["C_INVOICEPAYSCHEDULE_ID"]);

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
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + vp.GetName());
                    }
                    else
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
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

                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + vp.GetName());
                }
                else
                {
                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
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
            sql.Clear();
            int VA027_PostDatedCheck_ID = 0;
            int C_Payment_ID = 0;
            Decimal PayAmt = 0;
            Decimal openAmt = 0;

            sql.Append("SELECT C_Payment_ID, C_Currency_ID, PayAmt,"
               + " paymentAvailable(C_Payment_ID) AS OpenAmt, C_BPartner_ID ");

            if (Env.IsModuleInstalled("VA027_") && _IncludePDC)
            {
                sql.Append(", (SELECT VA027_PostDatedCheck_ID FROM C_Payment WHERE C_Payment_ID= p.C_Payment_ID) AS VA027_postdatedcheck_ID ");
            }
            sql.Append("FROM C_Payment_v p "
           + "WHERE AD_Client_ID=" + _run.GetAD_Client_ID()        //	##1
           + " AND AD_Org_ID=" + _run.GetAD_Org_ID()
           + " AND IsAllocated='N' AND C_BPartner_ID IS NOT NULL"
           + " AND C_Charge_ID IS NULL"
           + " AND DocStatus IN ('CO','CL')"
           //	Only BP and BPGroup with Dunning defined
           + "AND EXISTS  (SELECT *  FROM C_DunningLevel dl  WHERE dl.C_DunningLevel_ID=  " + _run.GetC_DunningLevel_ID() +
           " AND dl.C_Dunning_ID  IN  (SELECT COALESCE(bp.C_Dunning_ID, bpg.C_Dunning_ID)  FROM C_BPartner bp  " +
           "INNER JOIN C_BP_Group bpg  ON (bp.C_BP_Group_ID =bpg.C_BP_Group_ID) WHERE p.C_BPartner_ID=bp.C_BPartner_ID ))");
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
                {
                    C_Payment_ID = 0; C_Currency_ID = 0; PayAmt = 0; openAmt = 0; C_BPartner_ID = 0; VA027_PostDatedCheck_ID = 0;

                    C_Payment_ID = Utility.Util.GetValueOfInt(dr["C_Payment_ID"]);
                    C_Currency_ID = Utility.Util.GetValueOfInt(dr["C_Currency_ID"]);
                    PayAmt = Decimal.Negate(Utility.Util.GetValueOfDecimal(dr["PayAmt"]));//.getBigDecimal(3).negate();
                    openAmt = Decimal.Negate(Utility.Util.GetValueOfDecimal(dr["OpenAmt"]));// rs.getBigDecimal(4).negate();
                    C_BPartner_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_ID"]);//.getInt(5);

                    //if include pdc is true then add PDC reference against the payment if payment is generated from PDC
                    if (Env.IsModuleInstalled("VA027_") && _IncludePDC)
                    {
                        VA027_PostDatedCheck_ID = Utility.Util.GetValueOfInt(dr["VA027_PostDatedCheck_ID"]);
                    }
                    // checkup the amount
                    if (Env.ZERO.CompareTo(openAmt) == 0)
                    {
                        continue;
                    }

                    //
                    if (CreatePaymentLine(C_Payment_ID, C_Currency_ID, PayAmt, openAmt,
                        C_BPartner_ID, VA027_PostDatedCheck_ID))
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
            Decimal PayAmt, Decimal OpenAmt, int C_BPartner_ID, int VA027_PostDatedCheck_ID)
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
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + val);
                    }
                    else
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                    }
                    return false;
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetPayment(C_Payment_ID, C_Currency_ID, PayAmt, OpenAmt, VA027_PostDatedCheck_ID);

            if (!line.Save())
            {
                // Change by mohit to pick error message from last error in mclass.
                //throw new Exception(GetRetrievedError(line, "Cannot save MDunningRunLine"));
                //throw new Exception("Cannot save MDunningRunLine");
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {

                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + vp.GetName());
                }
                else
                {
                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add fee l````````````ine on run tab of dunning run window.
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

                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + vp.GetName());
                        }
                        else
                        {
                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                        }
                        return;
                    }
                    entries[i].SetQty(Decimal.Subtract(entries[i].GetQty(), new Decimal(1)));
                }
            }
        }

        /// <summary>
        /// Add CashJournal to line tab of dunnig run Window
        /// </summary>
        /// <returns>no of Cashjournalline inserted.</returns>
        private int AddCashJournal()
        {
            sql.Clear();
            int C_CashLine_ID = 0;
            Decimal Amount = 0;
            Decimal openAmt = 0;
            String PaymentType = "";
            sql.Append("SELECT C_CashLine_ID ,cl.C_Currency_ID, cl.C_BPartner_ID, alloccashavailable(C_CashLine_ID) AS openAmt, cl.Vss_PaymentType, " +
                "CASE " +
                "WHEN(cl.Vss_PaymentType = 'R' OR cl.Vss_PaymentType = 'A') AND cl.Amount > 0 THEN cl.Amount * -1 " + // Amount should be - ve for paymentType : Cash Receipt and Payment Return
                "WHEN(cl.Vss_PaymentType = 'P' OR cl.Vss_PaymentType = 'E') AND cl.Amount < 0 THEN cl.Amount * -1 " +  //Amount should be +ve for PaymentType : Cash Payment and Receipt Return
                "ELSE " +
                "cl.Amount END AS Amount " +              
                "FROM c_CashLine cl" +
                " INNER JOIN C_Cash c on cl.C_Cash_ID = c.C_Cash_ID"+
                " WHERE cl.cashtype ='B' AND cl.IsAllocated='N'  " +
                "AND c.DocStatus In ('CO','Cl') " +
                "AND cl.Ad_Client_ID =" + _run.GetAD_Client_ID() + " AND cl.Ad_Org_ID=  " + _run.GetAD_Org_ID() +
                 //	Only BP and BPGroup with Dunning defined
                 " AND EXISTS  (SELECT *  FROM C_DunningLevel dl  WHERE dl.C_DunningLevel_ID=  " + _run.GetC_DunningLevel_ID() +
                 " AND dl.C_Dunning_ID  IN  (SELECT COALESCE(bp.C_Dunning_ID, bpg.C_Dunning_ID)  FROM C_BPartner bp  " +
                 "INNER JOIN C_BP_Group bpg  ON (bp.C_BP_Group_ID =bpg.C_BP_Group_ID) WHERE cl.C_BPartner_ID=bp.C_BPartner_ID ))");


            if (_C_BPartner_ID != 0)
            {
                sql.Append(" AND cl.C_BPartner_ID=" + _C_BPartner_ID);
            }
            else if (_C_BP_Group_ID != 0)
            {
                sql.Append(" AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE cl.C_BPartner_ID=bp.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")");	//	##3
            }
            //only single currency
            if (!_IsAllCurrencies)
            {
                sql.Append(" AND cl.C_Currency_ID=" + _C_Currency_ID);
            }

            if (!_level.GetDaysAfterDue().Equals(new Decimal(-9999)))
            {
                sql.Append(" AND C_BPartner_ID IN (SELECT C_BPartner_ID FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + _run.Get_ID() + ")");
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
                {
                    C_CashLine_ID = 0; C_Currency_ID = 0; Amount = 0; openAmt = 0; C_BPartner_ID = 0; PaymentType = "";
                    C_CashLine_ID = Utility.Util.GetValueOfInt(dr["C_CashLine_ID"]);
                    C_Currency_ID = Utility.Util.GetValueOfInt(dr["C_Currency_ID"]);
                    C_BPartner_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_ID"]);
                    PaymentType = Utility.Util.GetValueOfString(dr["Vss_PaymentType"]);
                    Amount = Utility.Util.GetValueOfDecimal(dr["Amount"]);
                    openAmt = Utility.Util.GetValueOfDecimal(dr["openAmt"]);

                    // checkup the amount
                    if (Env.ZERO.CompareTo(openAmt) == 0)
                    {
                        continue;
                    }
                    //openAmt should be -ve in case of Cash Receipt and Payment Return 
                    if(PaymentType.Equals("R") || PaymentType.Equals("A"))
                    {
                        if (openAmt > 0)
                        {
                            openAmt = Decimal.Negate(openAmt);
                        }
                    }
                    //openAAmt should be +ve in case of Cash Payment and Receipt Return
                    else if (PaymentType.Equals("P") || PaymentType.Equals("E"))
                    {
                        if (openAmt < 0)
                        {
                            openAmt = Decimal.Negate(openAmt);
                        }
                    }
                    //crate records at line tab of Dunning Run window
                    if (CreatCashLine(C_CashLine_ID, Amount, openAmt, C_Currency_ID, C_BPartner_ID))
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
        /// create cashline on line tab of dunning run window
        /// </summary>
        /// <param name="C_CashLine_ID">C_CashLine_ID</param>
        /// <param name="Amount">Amount</param>
        /// <param name="openAmt">openAmt</param>
        /// <param name="C_Currency_ID">C_Currency_ID</param>
        /// <param name="C_BPartner_ID">C_BPartner_ID</param>
        /// <returns>true/False</returns>
        private bool CreatCashLine(int C_CashLine_ID, Decimal Amount, Decimal OpenAmt, int C_Currency_ID, int C_BPartner_ID)
        {
            MDunningRunEntry entry = _run.GetEntry(C_BPartner_ID, _C_Currency_ID, _SalesRep_ID);
            if (entry.Get_ID() == 0)
            {
                if (!entry.Save())
                {

                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        string val = vp.GetName();
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + val);
                    }
                    else
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                    }
                    return false;
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetcashLine(C_CashLine_ID, Amount, OpenAmt, C_Currency_ID);

            if (!line.Save())
            {
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {

                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + vp.GetName());
                }
                else
                {
                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add GLjournal to Run tab of dunnig run Window
        /// </summary>
        /// <returns>no of GlJournal line inserted.</returns>
        private int AddGLJournal()
        {
            sql.Clear();
            int GL_JournalLine_ID = 0;
            Decimal Amount = 0;
            Decimal openAmt = 0;

            sql.Append("SELECT GL_JournalLine_ID ,GL.C_Currency_ID, GL.C_BPartner_ID, AcctBalance(Gl.Account_ID, Gl.AmtSourceDr, Gl.AmtSourceCr) AS openAmt, " +
               "CASE " +
               "WHEN Gl.AmtSourceDr > 0 Then Gl.AmtSourceDr " +
               "WHEN Gl.AmtSourceCr > 0 Then Gl.AmtSourceCr * -1 " +
               "END AS Amount " +               
                "FROM Gl_JournalLine GL " +
                "INNER JOIN GL_Journal J ON GL.GL_Journal_ID =J.GL_Journal_ID " +
                "INNER JOIN C_ElementValue EV ON Gl.Account_ID = EV.C_ElementValue_ID " +
                "WHERE EV.IsAllocationRelated ='Y' AND GL.IsAllocated='N'  " +
                "AND J.DocStatus In ('CO','Cl') " +
                "AND GL.Ad_Client_ID ="+ _run.GetAD_Client_ID()+" AND GL.Ad_Org_ID=  "+_run.GetAD_Org_ID() +
                //	Only BP and BPGroup with Dunning defined
                " AND EXISTS  (SELECT *  FROM C_DunningLevel dl  WHERE dl.C_DunningLevel_ID=  " + _run.GetC_DunningLevel_ID() +
                " AND dl.C_Dunning_ID  IN  (SELECT COALESCE(bp.C_Dunning_ID, bpg.C_Dunning_ID)  FROM C_BPartner bp  " +
                "INNER JOIN C_BP_Group bpg  ON (bp.C_BP_Group_ID =bpg.C_BP_Group_ID) WHERE Gl.C_BPartner_ID=bp.C_BPartner_ID ))");

  
            if (_C_BPartner_ID != 0)
            {
                sql.Append(" AND GL.C_BPartner_ID=" + _C_BPartner_ID);
            }
            else if (_C_BP_Group_ID != 0)
            {
                sql.Append(" AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE GL.C_BPartner_ID=bp.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")");	//	##3
            }
            //only single currency
            if (!_IsAllCurrencies)
            {
                sql.Append(" AND GL.C_Currency_ID=" + _C_Currency_ID);
            }
            if (!_level.GetDaysAfterDue().Equals(new Decimal(-9999)))
            {
                sql.Append(" AND C_BPartner_ID IN (SELECT C_BPartner_ID FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + _run.Get_ID() + ")");
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
                {
                    GL_JournalLine_ID = 0; C_Currency_ID = 0; Amount = 0; openAmt = 0; C_BPartner_ID = 0;
                    GL_JournalLine_ID = Utility.Util.GetValueOfInt(dr["GL_JournalLine_ID"]);
                    C_Currency_ID = Utility.Util.GetValueOfInt(dr["C_Currency_ID"]);
                    C_BPartner_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_ID"]);
                    Amount = Utility.Util.GetValueOfDecimal(dr["Amount"]);
                    openAmt = Utility.Util.GetValueOfDecimal(dr["openAmt"]);

                    // checkup the amount
                    if (Env.ZERO.CompareTo(openAmt) == 0)
                    {
                        continue;
                    }
                    //crate records at line tab of Dunning Run window
                    if (CreatGLJournalLine(GL_JournalLine_ID, C_Currency_ID, C_BPartner_ID, Amount, openAmt))
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
        /// create journalline on line tab of dunning run window
        /// </summary>
        /// <param name="Gl_JournalLine_ID">Gl_JournalLine_ID</param>
        /// <param name="C_Currency_ID">Currency</param>
        /// <param name="C_BPartner_ID">Business partner</param>
        /// <param name="Amount">Amount</param>
        /// <param name="openAmt">OpenAmount</param>
        /// <returns>true/false</returns>
        private bool CreatGLJournalLine(int Gl_JournalLine_ID, int C_Currency_ID, int C_BPartner_ID, Decimal Amount, Decimal OpenAmt)
        {
            MDunningRunEntry entry = _run.GetEntry(C_BPartner_ID, _C_Currency_ID, _SalesRep_ID);
            if (entry.Get_ID() == 0)
            {
                if (!entry.Save())
                {
                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        string val = vp.GetName();
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + val);

                    }
                    else
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));

                    }
                    return false;
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetJournalLine(Gl_JournalLine_ID, C_Currency_ID, Amount, OpenAmt);

            if (!line.Save())
            {
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {

                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + vp.GetName());
                }
                else
                {
                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                }
                return false;
            }
            return true;
        }


        /// <summary>
        /// Add postdatedcheck to line tab of dunnig run Window
        /// </summary>
        /// <returns>no of postdaetedcheck inserted.</returns>
        private int AddPostDatedCheck()
        {
            sql.Clear();
            int VA027_PostDatedCheck_ID = 0;
            Decimal payAmt = 0;

            sql.Append("SELECT VA027_PostDatedCheck_ID,pdc.C_Currency_ID, pdc.C_BPartner_ID, " +
                "CASE " +
                "WHEN dt.DocBaseType='PDR' AND VA027_PayAmt>0 THEN VA027_PayAmt*-1 " +  //PDC Receivable : Amounts should be -ve 
                "WHEN dt.DocBaseType='PDP' AND VA027_PayAmt<0 THEN VA027_PayAmt*-1 " +   //PDC Payable : Amounts should be +ve 
                "ELSE VA027_PayAmt " +
                "END AS VA027_PayAmt " +
                "FROM  VA027_PostDatedCheck pdc INNER JOIN C_DocType dt on pdc.C_DocType_ID = dt.C_DocType_ID " +
                "WHERE pdc.VA027_PaymentGenerated='N' " +
                "AND DocStatus IN ('CO','CL')" +
                "AND pdc.Ad_Client_ID =" + _run.GetAD_Client_ID() + " AND pdc.Ad_Org_ID=  " + _run.GetAD_Org_ID() +
                 //	Only BP and BPGroup with Dunning defined
                "AND EXISTS  (SELECT *  FROM C_DunningLevel dl  WHERE dl.C_DunningLevel_ID=  " + _run.GetC_DunningLevel_ID() +
                " AND dl.C_Dunning_ID  IN  (SELECT COALESCE(bp.C_Dunning_ID, bpg.C_Dunning_ID)  FROM C_BPartner bp  " +
                "INNER JOIN C_BP_Group bpg  ON (bp.C_BP_Group_ID =bpg.C_BP_Group_ID) WHERE pdc.C_BPartner_ID=bp.C_BPartner_ID ))");


            if (_C_BPartner_ID != 0)
            {
                sql.Append(" AND pdc.C_BPartner_ID=" + _C_BPartner_ID);
            }
            else if (_C_BP_Group_ID != 0)
            {
                sql.Append(" AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE pdc.C_BPartner_ID=bp.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")");	//	##3
            }
            // Only Sales Trx
            if (_OnlySOTrx)
            {
                sql.Append(" AND pdc.IsSOTrx='Y'");
            }
            // only single currency
            if (!_IsAllCurrencies)
            {
                sql.Append(" AND pdc.C_Currency_ID=" + _C_Currency_ID);
            }
            if (!_level.GetDaysAfterDue().Equals(new Decimal(-9999)))
            {
                sql.Append(" AND C_BPartner_ID IN (SELECT C_BPartner_ID FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + _run.Get_ID() + ")");
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
                {
                    VA027_PostDatedCheck_ID = 0; C_Currency_ID = 0; payAmt = 0; C_BPartner_ID = 0;
                    VA027_PostDatedCheck_ID = Utility.Util.GetValueOfInt(dr["VA027_PostDatedCheck_ID"]);
                    C_Currency_ID = Utility.Util.GetValueOfInt(dr["C_Currency_ID"]);
                    C_BPartner_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_ID"]);
                    payAmt = Utility.Util.GetValueOfDecimal(dr["VA027_PayAmt"]);

                    // checkup the amount
                    if (Env.ZERO.CompareTo(payAmt) == 0)
                    {
                        continue;
                    }
                    //crate records at line tab of Dunning Run window
                    if (CreatePostDatedCheck(VA027_PostDatedCheck_ID, payAmt, C_Currency_ID, C_BPartner_ID))
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
        /// Create PDC on line tab of dunning run window
        /// </summary>
        /// <param name="VA027_PostDatedCheck_ID">Postdatedcheque</param>
        /// <param name="payAmt">Amount</param>
        /// <param name="C_Currency_ID">Currency</param>
        /// <param name="C_BPartner_ID">Business Partner</param>
        /// <returns>True/False</returns>
        private bool CreatePostDatedCheck(int VA027_PostDatedCheck_ID, Decimal PayAmt, int C_Currency_ID, int C_BPartner_ID)
        {
            MDunningRunEntry entry = _run.GetEntry(C_BPartner_ID, _C_Currency_ID, _SalesRep_ID);
            if (entry.Get_ID() == 0)
            {
                if (!entry.Save())
                {
                    ValueNamePair vp = VLogger.RetrieveError();
                    if (vp != null)
                    {
                        string val = vp.GetName();
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + val);
                    }
                    else
                    {
                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                    }
                    return false;
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetPostDatedCheque(VA027_PostDatedCheck_ID, C_Currency_ID, PayAmt);

            if (!line.Save())
            {
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {

                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine" + vp.GetName()));
                }
                else
                {
                    log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
                }
                return false;
            }
            return true;
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

                                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine") + vp.GetName());
                                    }
                                    else
                                    {
                                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "NotSaveDunRunLine"));
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
