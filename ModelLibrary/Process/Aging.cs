/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Aging
 * Purpose        : Invoice Aging Report.
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           14-Jan-2010
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
//////using VAdvantage.Report;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class Aging : ProcessEngine.SvrProcess
    {
        /** The date to calculate the days due from			*/
        private DateTime? _StatementDate = null;
        private Boolean _IsSOTrx = false;
        private int _VAF_Org_ID = 0;
        private int _VAB_Currency_ID = 0;
        private int _VAB_BPart_Category_ID = 0;
        private int _VAB_BusinessPartner_ID = 0;
        private Boolean _IsListInvoices = false;
        /** Number of days between today and statement date	*/
        private int _statementOffset = 0;

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
                else if (name.Equals("StatementDate"))
                {
                    _StatementDate = Utility.Util.GetValueOfDateTime(para[i].GetParameter());
                }
                else if (name.Equals("IsSOTrx"))
                {
                    _IsSOTrx = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("VAB_Currency_ID"))
                {
                    _VAB_Currency_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAF_Org_ID"))
                {
                    _VAF_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_BPart_Category_ID"))
                {
                    _VAB_BPart_Category_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("IsListInvoices"))
                {
                    _IsListInvoices = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            if (_StatementDate == null)
            {
                _StatementDate = DateTime.Now;// new Timestamp (System.currentTimeMillis());
            }
            else
            {
                //_statementOffset = TimeUtil.GetDaysBetween( 
                //	new Timestamp(System.currentTimeMillis()), _StatementDate);
                _statementOffset = TimeUtil.GetDaysBetween(
                    DateTime.Now, _StatementDate);
            }
        }	//	prepare

        /// <summary>
        /// DoIt
        /// </summary>
        /// <returns>Message</returns>
        protected override String DoIt()
        {
            log.Info("StatementDate=" + _StatementDate + ", IsSOTrx=" + _IsSOTrx
                + ", VAB_Currency_ID=" + _VAB_Currency_ID + ",VAF_Org_ID=" + _VAF_Org_ID
                + ", VAB_BPart_Category_ID=" + _VAB_BPart_Category_ID + ", VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID
                + ", IsListInvoices=" + _IsListInvoices);
            //
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT bp.VAB_BPart_Category_ID, oi.VAB_BusinessPartner_ID,oi.VAB_Invoice_ID,oi.VAB_sched_InvoicePayment_ID, "
                + "oi.VAB_Currency_ID, oi.IsSOTrx, "								//	5..6
                + "oi.DateInvoiced, oi.NetDays,oi.DueDate,oi.DaysDue, ");		//	7..10
            if (_VAB_Currency_ID == 0 || _VAB_Currency_ID == -1)
            {
                sql.Append("oi.GrandTotal, oi.PaidAmt, oi.OpenAmt ");			//	11..13
            }
            else
            {
                String s = ",oi.VAB_Currency_ID," + _VAB_Currency_ID + ",oi.DateAcct,oi.VAB_CurrencyType_ID,oi.VAF_Client_ID,oi.VAF_Org_ID)";
                sql.Append("currencyConvert(oi.GrandTotal").Append(s)		//	11..
                    .Append(", currencyConvert(oi.PaidAmt").Append(s)
                    .Append(", currencyConvert(oi.OpenAmt").Append(s);
            }
            sql.Append(",oi.VAB_BillingCode_ID,oi.VAB_Promotion_ID,oi.VAB_Project_ID "	//	14
                + "FROM RV_OpenItem oi"
                + " INNER JOIN VAB_BusinessPartner bp ON (oi.VAB_BusinessPartner_ID=bp.VAB_BusinessPartner_ID) "
                + "WHERE oi.ISSoTrx=").Append(_IsSOTrx ? "'Y'" : "'N'");
            if (_VAF_Org_ID > 0)
            {
                sql.Append(" AND oi.VAF_Org_ID=").Append(_VAF_Org_ID);
            }
            if (_VAB_BusinessPartner_ID > 0)
            {
                sql.Append(" AND oi.VAB_BusinessPartner_ID=").Append(_VAB_BusinessPartner_ID);
            }
            else if (_VAB_BPart_Category_ID > 0)
            {
                sql.Append(" AND bp.VAB_BPart_Category_ID=").Append(_VAB_BPart_Category_ID);
            }
            sql.Append(" ORDER BY oi.VAB_BusinessPartner_ID, oi.VAB_Currency_ID, oi.VAB_Invoice_ID");

            log.Finest(sql.ToString());
            String finalSql = MVAFRole.GetDefault(GetCtx(), false).AddAccessSQL(
                sql.ToString(), "oi", MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);
            log.Finer(finalSql);
            DataSet ds = null;

            // IDataReader idr = null;
            //
            MVATAging aging = null;
            int counter = 0;
            int rows = 0;
            int VAF_JInstance_ID = GetVAF_JInstance_ID();
            //
            try
            {
                //pstmt = DataBase.prepareStatement(finalSql, Get_Trx());
                // idr = DataBase.DB.ExecuteReader(finalSql, null, Get_Trx());
                ds = new DataSet();
                ds = DataBase.DB.ExecuteDataset(finalSql, null, Get_Trx());
                //while (idr.Read())
                foreach (DataRow idr in ds.Tables[0].Rows)
                {
                    int VAB_BPart_Category_ID = Utility.Util.GetValueOfInt(idr[0]);// Utility. rs.getInt(1);
                    int VAB_BusinessPartner_ID = Utility.Util.GetValueOfInt(idr[1]);// rs.getInt(2);
                    int VAB_Invoice_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[2]) : 0;
                    int VAB_sched_InvoicePayment_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[3]) : 0;
                    int VAB_Currency_ID = Utility.Util.GetValueOfInt(idr[4]);// rs.getInt(5);
                    Boolean IsSOTrx = "Y".Equals(Utility.Util.GetValueOfString(idr[5]));
                    //
                    //	Timestamp DateInvoiced = rs.getTimestamp(7);
                    //	int NetDays = rs.getInt(8);
                    DateTime? DueDate = Utility.Util.GetValueOfDateTime(idr[8]);// rs.getTimestamp(9);
                    //	Days Due
                    int DaysDue = Utility.Util.GetValueOfInt(idr[9])// rs.getInt(10)		//	based on today
                        + _statementOffset;
                    //
                    Decimal? GrandTotal = Utility.Util.GetValueOfDecimal(idr[10]);// rs.getBigDecimal(11);
                    //	BigDecimal PaidAmt = rs.getBigDecimal(12);
                    Decimal? OpenAmt = Utility.Util.GetValueOfDecimal(idr[12]);// rs.getBigDecimal(13);
                    //
                    int VAB_BillingCode_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[13]) : 0;
                    int VAB_Promotion_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[14]) : 0;
                    int VAB_Project_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[15]) : 0;

                    rows++;
                    //	New Aging Row
                    if (aging == null 		//	Key
                        || VAF_JInstance_ID != aging.GetVAF_JInstance_ID()
                        || VAB_BusinessPartner_ID != aging.GetVAB_BusinessPartner_ID()
                        || VAB_Currency_ID != aging.GetVAB_Currency_ID()
                        || VAB_Invoice_ID != aging.GetVAB_Invoice_ID()
                        || VAB_sched_InvoicePayment_ID != aging.GetVAB_sched_InvoicePayment_ID())
                    {
                        if (aging != null)
                        {
                            if (aging.Save())
                            {
                                log.Fine("#" + ++counter + " - " + aging);
                            }
                            else
                            {
                                log.Log(Level.SEVERE, "Not saved " + aging);
                                break;
                            }
                        }
                        aging = new MVATAging(GetCtx(), VAF_JInstance_ID, _StatementDate,
                            VAB_BusinessPartner_ID, VAB_Currency_ID,
                            VAB_Invoice_ID, VAB_sched_InvoicePayment_ID,
                            VAB_BPart_Category_ID, DueDate, IsSOTrx, Get_Trx());
                        if (_VAF_Org_ID > 0)
                        {
                            aging.SetVAF_Org_ID(_VAF_Org_ID);
                        }
                        aging.SetVAB_BillingCode_ID(VAB_BillingCode_ID);
                        aging.SetVAB_Promotion_ID(VAB_Promotion_ID);
                        aging.SetVAB_Project_ID(VAB_Project_ID);
                    }
                    //	Fill Buckets
                    aging.Add(DueDate.Value, DaysDue, GrandTotal.Value, OpenAmt.Value);
                }
                ds = null;
                // idr.Close();

                if (aging != null)
                {
                    if (aging.Save())
                    {
                        log.Fine("#" + ++counter + " - " + aging);
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "Not saved " + aging);
                    }
                }
            }
            catch (Exception e)
            {
                if (ds != null)
                {
                    ds = null;
                }
                log.Log(Level.SEVERE, finalSql, e);
            }
            //	
            log.Info("#" + counter + " - rows=" + rows);
           
            return "";
        }	//	doIt
    }	//	Aging


   
}
