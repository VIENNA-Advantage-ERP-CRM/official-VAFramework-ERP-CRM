/********************************************************
 * Module Name    : 
 * Purpose        : Commission Copy
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Veena     10-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.Logging;


using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class CommissionCalc : ProcessEngine.SvrProcess
    {
        private DateTime p_StartDate;
        //
        private DateTime m_EndDate;
        private MCommission m_com;

        /// <summary>
        /// Prapare - e.g., Get Parameters.
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
                else if (name.Equals("StartDate"))
                    p_StartDate = (DateTime)para[i].GetParameter();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }

        /// <summary>
        /// Perform Process
        /// </summary>
        /// <returns>Message (text with variables)</returns>
        protected override String DoIt()
        {
            string msg;
            log.Info("C_Commission_ID=" + GetRecord_ID() + ", StartDate=" + p_StartDate);
            if (p_StartDate == null)
                p_StartDate = DateTime.Now;
            m_com = new MCommission(GetCtx(), GetRecord_ID(), Get_Trx());
            if (m_com.Get_ID() == 0)
                throw new Exception("No Commission");

            //	Create Commission	
            MCommissionRun comRun = new MCommissionRun(m_com);
            SetStartEndDate();
            comRun.SetStartDate(p_StartDate);
            System.Threading.Thread.CurrentThread.CurrentCulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetLoginLanguage(GetCtx()).GetAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetLoginLanguage(GetCtx()).GetAD_Language());
            //	01-Jan-2000 - 31-Jan-2001 - USD
            Classes.SimpleDateFormat format = Classes.DisplayType.GetDateFormat(Classes.DisplayType.Date);
            String description = format.Format(p_StartDate)
                + " - " + format.Format(m_EndDate)
                + " - " + MCurrency.GetISO_Code(GetCtx(), m_com.GetC_Currency_ID());

            //string description = p_StartDate
            //    + " - " + m_EndDate
            //    + " - " + MCurrency.GetISO_Code(GetCtx(), m_com.GetC_Currency_ID());

            comRun.SetDescription(description);
            System.Threading.Thread.CurrentThread.CurrentCulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetBaseAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetBaseAD_Language());

            if (!comRun.Save())
                throw new Exception("Could not save Commission Run please check Organization");

            MCommissionLine[] lines = m_com.GetLines();
            //should not generate commission if there is no active commission lines
            if (lines.Length <= 0)
            {
                msg= Msg.GetMsg(GetCtx(), "NoActiveRecordsFound");
                return msg;
            }
            for (int i = 0; i < lines.Length; i++)
            {
                #region
                #endregion
                //	Amt for Line - Updated By Trigger
                MCommissionAmt comAmt = new MCommissionAmt(comRun, lines[i].GetC_CommissionLine_ID());
                if (!comAmt.Save())
                    throw new SystemException("Could not save Commission Amt");
                //
                StringBuilder sql = new StringBuilder();
                if (MCommission.DOCBASISTYPE_Receipt.Equals(m_com.GetDocBasisType()))
                {
                    if (m_com.IsListDetails())
                    {
                        sql.Append("SELECT h.C_Currency_ID, (l.LineNetAmt*al.Amount/h.GrandTotal) AS Amt,"
                            + " (l.QtyInvoiced*al.Amount/h.GrandTotal) AS Qty,"
                            + " NULL, l.C_InvoiceLine_ID, p.DocumentNo||'_'||h.DocumentNo,"
                            + " COALESCE(prd.Value,l.Description), h.DateInvoiced "
                            + "FROM C_Payment p"
                            + " INNER JOIN C_AllocationLine al ON (p.C_Payment_ID=al.C_Payment_ID)"
                            + " INNER JOIN C_Invoice h ON (al.C_Invoice_ID = h.C_Invoice_ID)"
                            + " INNER JOIN C_InvoiceLine l ON (h.C_Invoice_ID = l.C_Invoice_ID) "
                            + " LEFT OUTER JOIN M_Product prd ON (l.M_Product_ID = prd.M_Product_ID) "
                            + "WHERE p.DocStatus IN ('CL','CO','RE')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND p.AD_Client_ID = @clientid"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND p.DateTrx BETWEEN @sdate AND @edate");
                    }
                    else
                    {
                        sql.Append("SELECT h.C_Currency_ID, SUM(l.LineNetAmt*al.Amount/h.GrandTotal) AS Amt,"
                            + " SUM(l.QtyInvoiced*al.Amount/h.GrandTotal) AS Qty,"
                            + " NULL, NULL, NULL, NULL, MAX(h.DateInvoiced) "
                            + "FROM C_Payment p"
                            + " INNER JOIN C_AllocationLine al ON (p.C_Payment_ID=al.C_Payment_ID)"
                            + " INNER JOIN C_Invoice h ON (al.C_Invoice_ID = h.C_Invoice_ID)"
                            + " INNER JOIN C_InvoiceLine l ON (h.C_Invoice_ID = l.C_Invoice_ID) "
                            + "WHERE p.DocStatus IN ('CL','CO','RE')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND p.AD_Client_ID = @clientid"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND p.DateTrx BETWEEN @sdate AND @edate");
                    }
                }
                else if (MCommission.DOCBASISTYPE_Order.Equals(m_com.GetDocBasisType()))
                {
                    if (m_com.IsListDetails())
                    {
                        sql.Append("SELECT h.C_Currency_ID, l.LineNetAmt, l.QtyOrdered, "
                            + "l.C_OrderLine_ID, NULL, h.DocumentNo,"
                            + " COALESCE(prd.Value,l.Description),h.DateOrdered "
                            + "FROM C_Order h"
                            + " INNER JOIN C_OrderLine l ON (h.C_Order_ID = l.C_Order_ID)"
                            + " LEFT OUTER JOIN M_Product prd ON (l.M_Product_ID = prd.M_Product_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND h.AD_Client_ID = @clientid"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.DateOrdered BETWEEN @sdate AND @edate");
                    }
                    else
                    {
                        sql.Append("SELECT h.C_Currency_ID, SUM(l.LineNetAmt) AS Amt,"
                            + " SUM(l.QtyOrdered) AS Qty, "
                            + "l.C_OrderLine_ID, NULL, NULL, NULL, MAX(h.DateOrdered) "
                            + "FROM C_Order h"
                            + " INNER JOIN C_OrderLine l ON (h.C_Order_ID = l.C_Order_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.AD_Client_ID = @clientid"
                            + " AND h.DateOrdered BETWEEN @sdate AND @edate");
                    }
                }
                else 	//	Invoice Basis
                {
                    if (m_com.IsListDetails())
                    {
                        sql.Append("SELECT h.C_Currency_ID, l.LineNetAmt, l.QtyInvoiced, "
                            + "NULL, l.C_InvoiceLine_ID, h.DocumentNo,"
                            + " COALESCE(prd.Value,l.Description),h.DateInvoiced "
                            + "FROM C_Invoice h"
                            + " INNER JOIN C_InvoiceLine l ON (h.C_Invoice_ID = l.C_Invoice_ID)"
                            + " LEFT OUTER JOIN M_Product prd ON (l.M_Product_ID = prd.M_Product_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.AD_Client_ID = @clientid"
                            + " AND h.DateInvoiced BETWEEN @sdate AND @edate");
                    }
                    else
                    {
                        sql.Append("SELECT h.C_Currency_ID, SUM(l.LineNetAmt) AS Amt,"
                            + " SUM(l.QtyInvoiced) AS Qty, "
                            + "NULL, l.C_InvoiceLine_ID, NULL, NULL, MAX(h.DateInvoiced) "
                            + "FROM C_Invoice h"
                            + " INNER JOIN C_InvoiceLine l ON (h.C_Invoice_ID = l.C_Invoice_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.AD_Client_ID = @clientid"
                            + " AND h.DateInvoiced BETWEEN @sdate AND @edate");
                    }
                }
                //	CommissionOrders/Invoices
                if (lines[i].IsCommissionOrders())
                {
                    MUser[] users = MUser.GetOfBPartner(GetCtx(), m_com.GetC_BPartner_ID());
                    if (users == null || users.Length == 0)
                        throw new Exception("Commission Business Partner has no Users/Contact");
                    if (users.Length == 1)
                    {
                        int SalesRep_ID = users[0].GetAD_User_ID();
                        sql.Append(" AND h.SalesRep_ID=").Append(SalesRep_ID);
                    }
                    else
                    {
                        log.Warning("Not 1 User/Contact for C_BPartner_ID="
                            + m_com.GetC_BPartner_ID() + " but " + users.Length);
                        sql.Append(" AND h.SalesRep_ID IN (SELECT AD_User_ID FROM AD_User WHERE C_BPartner_ID=")
                            .Append(m_com.GetC_BPartner_ID()).Append(")");
                    }
                }
                //added by Arpit Rai on 7-May-2016 asked & Tested by Ravikant Sir
                //To calculate Commission Amount For the Particular Agent If Not Selected in Line Tab Of Commission
                else
                {
                    MUser[] users = MUser.GetOfBPartner(GetCtx(), m_com.GetC_BPartner_ID());
                    if (users == null || users.Length == 0)
                    {
                        throw new Exception("Commission Business Partner has no Users/Contact");
                    }
                    if (users.Length == 1)
                    {
                        int SaleRepID = users[0].GetAD_User_ID();
                        sql.Append(" AND h.SalesRep_ID = ");
                        sql.Append(SaleRepID);
                    }
                    else
                    {
                        log.Warning("Not 1 User/Contact for C_BPartner_ID="
                            + m_com.GetC_BPartner_ID() + " but " + users.Length);
                        sql.Append(" AND h.SalesRep_ID IN (SELECT AD_User_ID FROM AD_User WHERE C_BPartner_ID=")
                            .Append(m_com.GetC_BPartner_ID()).Append(")");
                    }
                }

                //	Organization
                if (lines[i].GetOrg_ID() != 0)
                    sql.Append(" AND h.AD_Org_ID=").Append(lines[i].GetOrg_ID());
                //	BPartner
                if (lines[i].GetC_BPartner_ID() != 0)
                    sql.Append(" AND h.C_BPartner_ID=").Append(lines[i].GetC_BPartner_ID());
                //	BPartner Group
                if (lines[i].GetC_BP_Group_ID() != 0)
                    sql.Append(" AND h.C_BPartner_ID IN "
                        + "(SELECT C_BPartner_ID FROM C_BPartner WHERE C_BP_Group_ID=").Append(lines[i].GetC_BP_Group_ID()).Append(")");
                //	Sales Region
                if (lines[i].GetC_SalesRegion_ID() != 0)
                    sql.Append(" AND h.C_BPartner_Location_ID IN "
                        + "(SELECT C_BPartner_Location_ID FROM C_BPartner_Location WHERE C_SalesRegion_ID=").Append(lines[i].GetC_SalesRegion_ID()).Append(")");
                //	Product
                if (lines[i].GetM_Product_ID() != 0)
                    sql.Append(" AND l.M_Product_ID=").Append(lines[i].GetM_Product_ID());
                //	Product Category
                if (lines[i].GetM_Product_Category_ID() != 0)
                    sql.Append(" AND l.M_Product_ID IN "
                        + "(SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID=").Append(lines[i].GetM_Product_Category_ID()).Append(")");
                //	Grouping according to Calculation Basis
                if (!m_com.IsListDetails() && m_com.GetDocBasisType().Equals("O"))
                {
                    sql.Append(" GROUP BY h.C_Currency_ID,l.C_OrderLine_ID");
                }
                else
                    sql.Append(" GROUP BY h.C_Currency_ID,l.C_InvoiceLine_ID");

                //
                log.Fine("Line=" + lines[i].GetLine() + " - " + sql);
                //
                CreateDetail(sql.ToString(), comAmt);
                //comAmt.CalculateCommission();
                comAmt.CalculatecommissionwithNewLogic();
                comAmt.Save();

                int countDetails = Util.GetValueOfInt(DataBase.DB.ExecuteScalar("SELECT COUNT(*) FROM C_CommissionDetail WHERE C_CommissionAmt_ID=" + comAmt.GetC_CommissionAmt_ID(), null, Get_TrxName()));
                if (countDetails == 0)
                {
                    comAmt.Delete(true, Get_Trx());
                }
            }	//	for all commission lines

            //	comRun.updateFromAmt();
            //	comRun.save();

            //	Save Last Run according to the current date and time
            m_com.SetDateLastRun(DateTime.Now);
            m_com.Save();

            return "@C_CommissionRun_ID@ = " + comRun.GetDocumentNo()
                + " - " + comRun.GetDescription();
        }

        /// <summary>
        /// Set Start and End Date
        /// </summary>
        private void SetStartEndDate()
        {
            DateTime dtCal = p_StartDate.Date;

            //	Yearly
            if (MCommission.FREQUENCYTYPE_Yearly.Equals(m_com.GetFrequencyType()))
            {
                dtCal = new DateTime(dtCal.Year, 12, 31);
                m_EndDate = dtCal;
            }
            //	Quarterly
            else if (MCommission.FREQUENCYTYPE_Quarterly.Equals(m_com.GetFrequencyType()))
            {
                dtCal = new DateTime(dtCal.Year, dtCal.Month, 01);
                int month1 = dtCal.Month;
                int mm = 0;

                if (month1 < TimeUtil.APRIL)
                    mm = TimeUtil.JANUARY;
                else if (month1 < TimeUtil.JULY)
                    mm = TimeUtil.APRIL;
                else if (month1 < TimeUtil.SEPTEMBER)
                    mm = TimeUtil.JULY;
                else
                    mm = TimeUtil.OCTOBER;

                dtCal = new DateTime(dtCal.Year, mm, dtCal.Day);
                p_StartDate = dtCal;

                dtCal = dtCal.AddMonths(2);
                dtCal = TimeUtil.GetMonthLastDay(dtCal);
                m_EndDate = dtCal;
            }
            //	Weekly
            else if (MCommission.FREQUENCYTYPE_Weekly.Equals(m_com.GetFrequencyType()))
            {
                dtCal = TimeUtil.Trunc(dtCal, TimeUtil.TRUNC_WEEK);
                p_StartDate = dtCal;
                dtCal = dtCal.AddDays(7);
                m_EndDate = dtCal;
            }
            //	Monthly
            else
            {
                dtCal = new DateTime(dtCal.Year, dtCal.Month, 01);
                p_StartDate = dtCal;
                dtCal = TimeUtil.GetMonthLastDay(dtCal);
                m_EndDate = dtCal;
            }
            log.Fine("SetStartEndDate = " + p_StartDate + " - " + m_EndDate);

            /**
            String sd = DataBase.DB.TO_DATE(p_StartDate, true);
            StringBuilder sql = new StringBuilder ("SELECT ");
            if (MCommission.FREQUENCYTYPE_Quarterly.Equals(m_com.GetFrequencyType()))
                sql.Append("TRUNC(").Append(sd).Append(", 'Q'), TRUNC(").Append(sd).Append("+92, 'Q')-1");
            else if (MCommission.FREQUENCYTYPE_Weekly.Equals(m_com.GetFrequencyType()))
                sql.Append("TRUNC(").Append(sd).Append(", 'DAY'), TRUNC(").Append(sd).Append("+7, 'DAY')-1");
            else	//	Month
                sql.Append("TRUNC(").Append(sd).Append(", 'MM'), TRUNC(").Append(sd).Append("+31, 'MM')-1");
            sql.Append(" FROM DUAL");
            **/
        }

        /// <summary>
        /// Create Commission Detail
        /// </summary>
        /// <param name="sql">sql statement</param>
        /// <param name="comAmt">parent</param>
        private void CreateDetail(String sql, MCommissionAmt comAmt)
        {
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@clientid", m_com.GetAD_Client_ID());
                param[1] = new SqlParameter("@sdate", p_StartDate.Date);
                param[2] = new SqlParameter("@edate", m_EndDate.Date);

                idr = DataBase.DB.ExecuteReader(sql, param, Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                //while (idr.Read())
                {
                    //	CommissionAmount, C_Currency_ID, Amt, Qty,
                    MCommissionDetail cd = new MCommissionDetail(comAmt,
                        Utility.Util.GetValueOfInt(dr[0]), Utility.Util.GetValueOfDecimal(dr[1]), Utility.Util.GetValueOfDecimal(dr[2]));

                    //	C_OrderLine_ID, C_InvoiceLine_ID,
                    cd.SetLineIDs(Utility.Util.GetValueOfInt(dr[3]), Utility.Util.GetValueOfInt(dr[4]));

                    if (Utility.Util.GetValueOfInt(dr[3]) > 0)
                    {
                        MOrderLine _ordline = new MOrderLine(GetCtx(), Utility.Util.GetValueOfInt(dr[3]), Get_TrxName());
                        _ordline.SetIsCommissionCalculated(true);
                        _ordline.Save();
                    }
                    if (Utility.Util.GetValueOfInt(dr[4]) > 0)
                    {
                        MInvoiceLine _invline = new MInvoiceLine(GetCtx(), Utility.Util.GetValueOfInt(dr[4]), Get_TrxName());
                        _invline.SetIsCommissionCalculated(true);
                        _invline.Save();
                    }

                    //	Reference, Info,
                    String s = dr[5].ToString();
                    if (s != null)
                        cd.SetReference(s);
                    s = dr[6].ToString();
                    if (s != null)
                        cd.SetInfo(s);

                    //	Date
                    DateTime? date = Utility.Util.GetValueOfDateTime(dr[7]);
                    cd.SetConvertedAmt(date);

                    //
                    if (!cd.Save())		//	creates memory leak
                    {
                        //idr.Close();
                        throw new ArgumentException("CommissionCalc - Detail Not saved");
                    }
                }
                //idr.Close();
            }
            catch (Exception e)
            {
                idr.Close();
                log.Log(Level.SEVERE, "CreateDetail", e);
            }
        }
    }
}
