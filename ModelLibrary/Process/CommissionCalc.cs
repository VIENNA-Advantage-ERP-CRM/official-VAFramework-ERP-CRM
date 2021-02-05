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
        private MVABWorkCommission m_com;

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
            log.Info("VAB_WorkCommission_ID=" + GetRecord_ID() + ", StartDate=" + p_StartDate);
            if (p_StartDate == null)
                p_StartDate = DateTime.Now;
            m_com = new MVABWorkCommission(GetCtx(), GetRecord_ID(), Get_Trx());
            if (m_com.Get_ID() == 0)
                throw new Exception("No Commission");

            //	Create Commission	
            MVABWorkCommissionCalc comRun = new MVABWorkCommissionCalc(m_com);
            SetStartEndDate();
            comRun.SetStartDate(p_StartDate);
            System.Threading.Thread.CurrentThread.CurrentCulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetLoginLanguage(GetCtx()).GetVAF_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetLoginLanguage(GetCtx()).GetVAF_Language());
            //	01-Jan-2000 - 31-Jan-2001 - USD
            Classes.SimpleDateFormat format = Classes.DisplayType.GetDateFormat(Classes.DisplayType.Date);
            String description = format.Format(p_StartDate)
                + " - " + format.Format(m_EndDate)
                + " - " + MVABCurrency.GetISO_Code(GetCtx(), m_com.GetVAB_Currency_ID());

            //string description = p_StartDate
            //    + " - " + m_EndDate
            //    + " - " + MCurrency.GetISO_Code(GetCtx(), m_com.GetVAB_Currency_ID());

            comRun.SetDescription(description);
            System.Threading.Thread.CurrentThread.CurrentCulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetBaseVAF_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Env.GetLanguage(GetCtx()).GetCulture(Utility.Env.GetBaseVAF_Language());

            if (!comRun.Save())
                throw new Exception("Could not save Commission Run please check Organization");

            MVABWorkCommissionLine[] lines = m_com.GetLines();
            //should not generate commission if there is no active commission lines
            if (lines.Length <= 0)
            {
                msg = Msg.GetMsg(GetCtx(), "NoActiveRecordsFound");
                return msg;
            }
            for (int i = 0; i < lines.Length; i++)
            {
                #region
                #endregion
                //	Amt for Line - Updated By Trigger
                MVABWorkCommissionAmt comAmt = new MVABWorkCommissionAmt(comRun, lines[i].GetVAB_WorkCommissionLine_ID());
                if (!comAmt.Save())
                    throw new SystemException("Could not save Commission Amt");
                //
                StringBuilder sql = new StringBuilder();
                if (MVABWorkCommission.DOCBASISTYPE_Receipt.Equals(m_com.GetDocBasisType()))
                {
                    if (m_com.IsListDetails())
                    {
                        sql.Append("SELECT h.VAB_Currency_ID, (l.LineNetAmt*al.Amount/h.GrandTotal) AS Amt,"
                            + " (l.QtyInvoiced*al.Amount/h.GrandTotal) AS Qty,"
                            + " NULL, l.VAB_InvoiceLine_ID, p.DocumentNo||'_'||h.DocumentNo,"
                            + " COALESCE(prd.Value,l.Description), h.DateInvoiced "
                            + "FROM VAB_Payment p"
                            + " INNER JOIN VAB_DocAllocationLine al ON (p.VAB_Payment_ID=al.VAB_Payment_ID)"
                            + " INNER JOIN VAB_Invoice h ON (al.VAB_Invoice_ID = h.VAB_Invoice_ID)"
                            + " INNER JOIN VAB_InvoiceLine l ON (h.VAB_Invoice_ID = l.VAB_Invoice_ID) "
                            + " LEFT OUTER JOIN VAM_Product prd ON (l.VAM_Product_ID = prd.VAM_Product_ID) "
                            + "WHERE p.DocStatus IN ('CL','CO','RE')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND p.VAF_Client_ID = @clientid"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND p.DateTrx BETWEEN @sdate AND @edate");
                    }
                    else
                    {
                        sql.Append("SELECT h.VAB_Currency_ID, SUM(l.LineNetAmt*al.Amount/h.GrandTotal) AS Amt,"
                            + " SUM(l.QtyInvoiced*al.Amount/h.GrandTotal) AS Qty,"
                            + " NULL, NULL, NULL, NULL, MAX(h.DateInvoiced) "
                            + "FROM VAB_Payment p"
                            + " INNER JOIN VAB_DocAllocationLine al ON (p.VAB_Payment_ID=al.VAB_Payment_ID)"
                            + " INNER JOIN VAB_Invoice h ON (al.VAB_Invoice_ID = h.VAB_Invoice_ID)"
                            + " INNER JOIN VAB_InvoiceLine l ON (h.VAB_Invoice_ID = l.VAB_Invoice_ID) "
                            + "WHERE p.DocStatus IN ('CL','CO','RE')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND p.VAF_Client_ID = @clientid"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND p.DateTrx BETWEEN @sdate AND @edate");
                    }
                }
                else if (MVABWorkCommission.DOCBASISTYPE_Order.Equals(m_com.GetDocBasisType()))
                {
                    if (m_com.IsListDetails())
                    {
                        sql.Append("SELECT h.VAB_Currency_ID, l.LineNetAmt, l.QtyOrdered, "
                            + "l.VAB_OrderLine_ID, NULL, h.DocumentNo,"
                            + " COALESCE(prd.Value,l.Description),h.DateOrdered "
                            + "FROM VAB_Order h"
                            + " INNER JOIN VAB_OrderLine l ON (h.VAB_Order_ID = l.VAB_Order_ID)"
                            + " LEFT OUTER JOIN VAM_Product prd ON (l.VAM_Product_ID = prd.VAM_Product_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND h.VAF_Client_ID = @clientid"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.DateOrdered BETWEEN @sdate AND @edate");
                    }
                    else
                    {
                        sql.Append("SELECT h.VAB_Currency_ID, SUM(l.LineNetAmt) AS Amt,"
                            + " SUM(l.QtyOrdered) AS Qty, "
                            + "l.VAB_OrderLine_ID, NULL, NULL, NULL, MAX(h.DateOrdered) "
                            + "FROM VAB_Order h"
                            + " INNER JOIN VAB_OrderLine l ON (h.VAB_Order_ID = l.VAB_Order_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.VAF_Client_ID = @clientid"
                            + " AND h.DateOrdered BETWEEN @sdate AND @edate");
                    }
                }
                else 	//	Invoice Basis
                {
                    if (m_com.IsListDetails())
                    {
                        sql.Append("SELECT h.VAB_Currency_ID, l.LineNetAmt, l.QtyInvoiced, "
                            + "NULL, l.VAB_InvoiceLine_ID, h.DocumentNo,"
                            + " COALESCE(prd.Value,l.Description),h.DateInvoiced "
                            + "FROM VAB_Invoice h"
                            + " INNER JOIN VAB_InvoiceLine l ON (h.VAB_Invoice_ID = l.VAB_Invoice_ID)"
                            + " LEFT OUTER JOIN VAM_Product prd ON (l.VAM_Product_ID = prd.VAM_Product_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.VAF_Client_ID = @clientid"
                            + " AND h.DateInvoiced BETWEEN @sdate AND @edate");
                    }
                    else
                    {
                        sql.Append("SELECT h.VAB_Currency_ID, SUM(l.LineNetAmt) AS Amt,"
                            + " SUM(l.QtyInvoiced) AS Qty, "
                            + "NULL, l.VAB_InvoiceLine_ID, NULL, NULL, MAX(h.DateInvoiced) "
                            + "FROM VAB_Invoice h"
                            + " INNER JOIN VAB_InvoiceLine l ON (h.VAB_Invoice_ID = l.VAB_Invoice_ID) "
                            + "WHERE h.DocStatus IN ('CL','CO')"
                            + " AND h.IsSOTrx='Y'"
                            + " AND l.IsCommissionCalculated = 'N' "
                            + " AND h.VAF_Client_ID = @clientid"
                            + " AND h.DateInvoiced BETWEEN @sdate AND @edate");
                    }
                }
                //	CommissionOrders/Invoices
                if (lines[i].IsCommissionOrders())
                {
                    MVAFUserContact[] users = MVAFUserContact.GetOfBPartner(GetCtx(), m_com.GetVAB_BusinessPartner_ID());
                    if (users == null || users.Length == 0)
                        throw new Exception("Commission Business Partner has no Users/Contact");
                    if (users.Length == 1)
                    {
                        int SalesRep_ID = users[0].GetVAF_UserContact_ID();
                        sql.Append(" AND h.SalesRep_ID=").Append(SalesRep_ID);
                    }
                    else
                    {
                        log.Warning("Not 1 User/Contact for VAB_BusinessPartner_ID="
                            + m_com.GetVAB_BusinessPartner_ID() + " but " + users.Length);
                        sql.Append(" AND h.SalesRep_ID IN (SELECT VAF_UserContact_ID FROM VAF_UserContact WHERE VAB_BusinessPartner_ID=")
                            .Append(m_com.GetVAB_BusinessPartner_ID()).Append(")");
                    }
                }
                //added by Arpit Rai on 7-May-2016 asked & Tested by Ravikant Sir
                //To calculate Commission Amount For the Particular Agent If Not Selected in Line Tab Of Commission
                else
                {
                    MVAFUserContact[] users = MVAFUserContact.GetOfBPartner(GetCtx(), m_com.GetVAB_BusinessPartner_ID());
                    if (users == null || users.Length == 0)
                    {
                        throw new Exception("Commission Business Partner has no Users/Contact");
                    }
                    if (users.Length == 1)
                    {
                        int SaleRepID = users[0].GetVAF_UserContact_ID();
                        sql.Append(" AND h.SalesRep_ID = ");
                        sql.Append(SaleRepID);
                    }
                    else
                    {
                        log.Warning("Not 1 User/Contact for VAB_BusinessPartner_ID="
                            + m_com.GetVAB_BusinessPartner_ID() + " but " + users.Length);
                        sql.Append(" AND h.SalesRep_ID IN (SELECT VAF_UserContact_ID FROM VAF_UserContact WHERE VAB_BusinessPartner_ID=")
                            .Append(m_com.GetVAB_BusinessPartner_ID()).Append(")");
                    }
                }

                //	Organization
                if (lines[i].GetOrg_ID() != 0)
                    sql.Append(" AND h.VAF_Org_ID=").Append(lines[i].GetOrg_ID());
                //	BPartner
                if (lines[i].GetVAB_BusinessPartner_ID() != 0)
                    sql.Append(" AND h.VAB_BusinessPartner_ID=").Append(lines[i].GetVAB_BusinessPartner_ID());
                //	BPartner Group
                if (lines[i].GetVAB_BPart_Category_ID() != 0)
                    sql.Append(" AND h.VAB_BusinessPartner_ID IN "
                        + "(SELECT VAB_BusinessPartner_ID FROM VAB_BusinessPartner WHERE VAB_BPart_Category_ID=").Append(lines[i].GetVAB_BPart_Category_ID()).Append(")");
                //	Sales Region
                if (lines[i].GetVAB_SalesRegionState_ID() != 0)
                    sql.Append(" AND h.VAB_BPart_Location_ID IN "
                        + "(SELECT VAB_BPart_Location_ID FROM VAB_BPart_Location WHERE VAB_SalesRegionState_ID=").Append(lines[i].GetVAB_SalesRegionState_ID()).Append(")");
                //	Product
                if (lines[i].GetVAM_Product_ID() != 0)
                    sql.Append(" AND l.VAM_Product_ID=").Append(lines[i].GetVAM_Product_ID());
                //	Product Category
                if (lines[i].GetVAM_ProductCategory_ID() != 0)
                    sql.Append(" AND l.VAM_Product_ID IN "
                        + "(SELECT VAM_Product_ID FROM VAM_Product WHERE VAM_ProductCategory_ID=").Append(lines[i].GetVAM_ProductCategory_ID()).Append(")");
                //	Grouping according to Calculation Basis
                if (!m_com.IsListDetails() && m_com.GetDocBasisType().Equals("O"))
                {
                    sql.Append(" GROUP BY h.VAB_Currency_ID,l.VAB_OrderLine_ID");
                }
                else
                    sql.Append(" GROUP BY h.VAB_Currency_ID,l.VAB_InvoiceLine_ID");

                //
                log.Fine("Line=" + lines[i].GetLine() + " - " + sql);
                //
                CreateDetail(sql.ToString(), comAmt);
                //comAmt.CalculateCommission();
                comAmt.CalculatecommissionwithNewLogic();
                comAmt.Save();

                int countDetails = Util.GetValueOfInt(DataBase.DB.ExecuteScalar("SELECT COUNT(*) FROM VAB_WorkCommissionDetail WHERE VAB_WorkCommission_Amt_ID=" + comAmt.GetVAB_WorkCommission_Amt_ID(), null, Get_TrxName()));
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

            return "@VAB_WorkCommission_Calc_ID@ = " + comRun.GetDocumentNo()
                + " - " + comRun.GetDescription();
        }

        /// <summary>
        /// Set Start and End Date
        /// </summary>
        private void SetStartEndDate()
        {
            DateTime dtCal = p_StartDate.Date;

            //	Yearly
            if (MVABWorkCommission.FREQUENCYTYPE_Yearly.Equals(m_com.GetFrequencyType()))
            {
                dtCal = new DateTime(dtCal.Year, 12, 31);
                m_EndDate = dtCal;
            }
            //	Quarterly
            else if (MVABWorkCommission.FREQUENCYTYPE_Quarterly.Equals(m_com.GetFrequencyType()))
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
            else if (MVABWorkCommission.FREQUENCYTYPE_Weekly.Equals(m_com.GetFrequencyType()))
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
        private void CreateDetail(String sql, MVABWorkCommissionAmt comAmt)
        {
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@clientid", m_com.GetVAF_Client_ID());
                param[1] = new SqlParameter("@sdate", p_StartDate.Date);
                param[2] = new SqlParameter("@edate", m_EndDate.Date);

                idr = DataBase.DB.ExecuteReader(sql, param, Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                //while (idr.Read())
                {
                    //	CommissionAmount, VAB_Currency_ID, Amt, Qty,
                    MVABWorkCommissionDetail cd = new MVABWorkCommissionDetail(comAmt,
                        Utility.Util.GetValueOfInt(dr[0]), Utility.Util.GetValueOfDecimal(dr[1]), Utility.Util.GetValueOfDecimal(dr[2]));

                    //	VAB_OrderLine_ID, VAB_InvoiceLine_ID,
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
