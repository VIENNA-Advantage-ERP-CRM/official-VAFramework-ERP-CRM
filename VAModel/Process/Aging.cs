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
        private int _AD_Org_ID = 0;
        private int _C_Currency_ID = 0;
        private int _C_BP_Group_ID = 0;
        private int _C_BPartner_ID = 0;
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
                else if (name.Equals("C_Currency_ID"))
                {
                    _C_Currency_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    _AD_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
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
                + ", C_Currency_ID=" + _C_Currency_ID + ",AD_Org_ID=" + _AD_Org_ID
                + ", C_BP_Group_ID=" + _C_BP_Group_ID + ", C_BPartner_ID=" + _C_BPartner_ID
                + ", IsListInvoices=" + _IsListInvoices);
            //
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT bp.C_BP_Group_ID, oi.C_BPartner_ID,oi.C_Invoice_ID,oi.C_InvoicePaySchedule_ID, "
                + "oi.C_Currency_ID, oi.IsSOTrx, "								//	5..6
                + "oi.DateInvoiced, oi.NetDays,oi.DueDate,oi.DaysDue, ");		//	7..10
            if (_C_Currency_ID == 0 || _C_Currency_ID == -1)
            {
                sql.Append("oi.GrandTotal, oi.PaidAmt, oi.OpenAmt ");			//	11..13
            }
            else
            {
                String s = ",oi.C_Currency_ID," + _C_Currency_ID + ",oi.DateAcct,oi.C_ConversionType_ID,oi.AD_Client_ID,oi.AD_Org_ID)";
                sql.Append("currencyConvert(oi.GrandTotal").Append(s)		//	11..
                    .Append(", currencyConvert(oi.PaidAmt").Append(s)
                    .Append(", currencyConvert(oi.OpenAmt").Append(s);
            }
            sql.Append(",oi.C_Activity_ID,oi.C_Campaign_ID,oi.C_Project_ID "	//	14
                + "FROM RV_OpenItem oi"
                + " INNER JOIN C_BPartner bp ON (oi.C_BPartner_ID=bp.C_BPartner_ID) "
                + "WHERE oi.ISSoTrx=").Append(_IsSOTrx ? "'Y'" : "'N'");
            if (_AD_Org_ID > 0)
            {
                sql.Append(" AND oi.AD_Org_ID=").Append(_AD_Org_ID);
            }
            if (_C_BPartner_ID > 0)
            {
                sql.Append(" AND oi.C_BPartner_ID=").Append(_C_BPartner_ID);
            }
            else if (_C_BP_Group_ID > 0)
            {
                sql.Append(" AND bp.C_BP_Group_ID=").Append(_C_BP_Group_ID);
            }
            sql.Append(" ORDER BY oi.C_BPartner_ID, oi.C_Currency_ID, oi.C_Invoice_ID");

            log.Finest(sql.ToString());
            String finalSql = MRole.GetDefault(GetCtx(), false).AddAccessSQL(
                sql.ToString(), "oi", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            log.Finer(finalSql);
            DataSet ds = null;

            // IDataReader idr = null;
            //
            MAging aging = null;
            int counter = 0;
            int rows = 0;
            int AD_PInstance_ID = GetAD_PInstance_ID();
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
                    int C_BP_Group_ID = Utility.Util.GetValueOfInt(idr[0]);// Utility. rs.getInt(1);
                    int C_BPartner_ID = Utility.Util.GetValueOfInt(idr[1]);// rs.getInt(2);
                    int C_Invoice_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[2]) : 0;
                    int C_InvoicePaySchedule_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[3]) : 0;
                    int C_Currency_ID = Utility.Util.GetValueOfInt(idr[4]);// rs.getInt(5);
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
                    int C_Activity_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[13]) : 0;
                    int C_Campaign_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[14]) : 0;
                    int C_Project_ID = _IsListInvoices ? Utility.Util.GetValueOfInt(idr[15]) : 0;

                    rows++;
                    //	New Aging Row
                    if (aging == null 		//	Key
                        || AD_PInstance_ID != aging.GetAD_PInstance_ID()
                        || C_BPartner_ID != aging.GetC_BPartner_ID()
                        || C_Currency_ID != aging.GetC_Currency_ID()
                        || C_Invoice_ID != aging.GetC_Invoice_ID()
                        || C_InvoicePaySchedule_ID != aging.GetC_InvoicePaySchedule_ID())
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
                        aging = new MAging(GetCtx(), AD_PInstance_ID, _StatementDate,
                            C_BPartner_ID, C_Currency_ID,
                            C_Invoice_ID, C_InvoicePaySchedule_ID,
                            C_BP_Group_ID, DueDate, IsSOTrx, Get_Trx());
                        if (_AD_Org_ID > 0)
                        {
                            aging.SetAD_Org_ID(_AD_Org_ID);
                        }
                        aging.SetC_Activity_ID(C_Activity_ID);
                        aging.SetC_Campaign_ID(C_Campaign_ID);
                        aging.SetC_Project_ID(C_Project_ID);
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
