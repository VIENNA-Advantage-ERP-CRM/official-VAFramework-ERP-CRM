/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : TrialBalance
 * Purpose        : Trial Balance
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           11-Jan-2010
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
using System.Globalization;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Report
{
    public class TrialBalance:ProcessEngine.SvrProcess
    {
        /** AcctSchame Parameter			*/
        private int _VAB_AccountBook_ID = 0;
        /**	Period Parameter				*/
        private int _VAB_YearPeriod_ID = 0;
        private DateTime? _DateAcct_From = null;
        private DateTime? _DateAcct_To = null;
        /**	Org Parameter					*/
        private int _VAF_Org_ID = 0;
        /**	Account Parameter				*/
        private int _Account_ID = 0;
        private String _AccountValue_From = null;
        private String _AccountValue_To = null;
        /**	BPartner Parameter				*/
        private int _VAB_BusinessPartner_ID = 0;
        /**	Product Parameter				*/
        private int _M_Product_ID = 0;
        /**	Project Parameter				*/
        private int _VAB_Project_ID = 0;
        /**	Activity Parameter				*/
        private int _VAB_BillingCode_ID = 0;
        /**	SalesRegion Parameter			*/
        private int _VAB_SalesRegionState_ID = 0;
        /**	Campaign Parameter				*/
        private int _VAB_Promotion_ID = 0;
        /** Posting Type					*/
        private String _PostingType = "A";
        /** Hierarchy						*/
        private int _VAPA_FinancialReportingOrder_ID = 0;

        private int _VAF_OrgTrx_ID = 0;
        private int _C_LocFrom_ID = 0;
        private int _C_LocTo_ID = 0;
        private int _User1_ID = 0;
        private int _User2_ID = 0;


        /**	Parameter Where Clause			*/
        private StringBuilder m_parameterWhere = new StringBuilder();
        /**	Account							*/
        private MElementValue m_acct = null;

        /**	Start Time						*/
        private long m_start =CommonFunctions.CurrentTimeMillis();//  System.currentTimeMillis();
        /**	Insert Statement				*/
        private static String _insert = "INSERT INTO T_TrialBalance "
            + "(VAF_JInstance_ID, Actual_Acct_Detail_ID,"
            + " VAF_Client_ID, VAF_Org_ID, Created,CreatedBy, Updated,UpdatedBy,"
            + " VAB_AccountBook_ID, Account_ID, AccountValue, DateTrx, DateAcct, VAB_YearPeriod_ID,"
            + " VAF_TableView_ID, Record_ID, Line_ID,"
            + " VAGL_Group_ID, VAGL_Budget_ID, VAB_TaxRate_ID, M_Locator_ID, PostingType,"
            + " VAB_Currency_ID, AmtSourceDr, AmtSourceCr, AmtSourceBalance,"
            + " AmtAcctDr, AmtAcctCr, AmtAcctBalance, VAB_UOM_ID, Qty,"
            + " M_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,"
            + " VAB_SalesRegionState_ID, VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID,"
            + " User1_ID, User2_ID, VAA_Asset_ID, Description)";


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            StringBuilder sb = new StringBuilder("VAF_JInstance_ID=")
                .Append(GetVAF_JInstance_ID());
            //	Parameter
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_AccountBook_ID"))
                {
                    _VAB_AccountBook_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_YearPeriod_ID"))
                {
                    _VAB_YearPeriod_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("DateAcct"))
                {
                    _DateAcct_From = Utility.Util.GetValueOfDateTime(para[i].GetParameter());
                    _DateAcct_To = Utility.Util.GetValueOfDateTime(para[i].GetParameter_To());
                }
                else if (name.Equals("VAPA_FinancialReportingOrder_ID"))
                {
                    _VAPA_FinancialReportingOrder_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAF_Org_ID"))
                {
                    _VAF_Org_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("Account_ID"))
                {
                    _Account_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("AccountValue"))
                {
                    _AccountValue_From = Convert.ToString(para[i].GetParameter());
                    _AccountValue_To = Convert.ToString(para[i].GetParameter_To());
                }
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("M_Product_ID"))
                {
                    _M_Product_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_Project_ID"))
                {
                    _VAB_Project_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_BillingCode_ID"))
                {
                    _VAB_BillingCode_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_SalesRegionState_ID"))
                {
                    _VAB_SalesRegionState_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_Promotion_ID"))
                {
                    _VAB_Promotion_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("PostingType"))
                {
                    _PostingType = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            //	Mandatory VAB_AccountBook_ID
            m_parameterWhere.Append("VAB_AccountBook_ID=").Append(_VAB_AccountBook_ID);
            //	Optional Account_ID
            if (_Account_ID != 0 && _Account_ID!=-1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _VAPA_FinancialReportingOrder_ID, MAcctSchemaElement.ELEMENTTYPE_Account, _Account_ID));
            }
            if (_AccountValue_From != null && _AccountValue_From.Length == 0)
            {
                _AccountValue_From = null;
            }
            if (_AccountValue_To != null && _AccountValue_To.Length == 0)
            {
                _AccountValue_To = null;
            }
            if (_AccountValue_From != null && _AccountValue_To != null)
            {
                m_parameterWhere.Append(" AND (Account_ID IS NULL OR EXISTS (SELECT * FROM VAB_Acct_Element ev ")
                    .Append("WHERE Account_ID=ev.VAB_Acct_Element_ID AND ev.Value >= ")
                    .Append(DataBase.DB.TO_STRING(_AccountValue_From)).Append(" AND ev.Value <= ")
                    .Append(DataBase.DB.TO_STRING(_AccountValue_To)).Append("))");
            }
            else if (_AccountValue_From != null && _AccountValue_To == null)
            {
                m_parameterWhere.Append(" AND (Account_ID IS NULL OR EXISTS (SELECT * FROM VAB_Acct_Element ev ")
                .Append("WHERE Account_ID=ev.VAB_Acct_Element_ID AND ev.Value >= ")
                .Append(DataBase.DB.TO_STRING(_AccountValue_From)).Append("))");
            }
            else if (_AccountValue_From == null && _AccountValue_To != null)
            {
                m_parameterWhere.Append(" AND (Account_ID IS NULL OR EXISTS (SELECT * FROM VAB_Acct_Element ev ")
                .Append("WHERE Account_ID=ev.VAB_Acct_Element_ID AND ev.Value <= ")
                .Append(DataBase.DB.TO_STRING(_AccountValue_To)).Append("))");
            }
            //	Optional Org
            if (_VAF_Org_ID != 0 && _VAF_Org_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _VAPA_FinancialReportingOrder_ID, MAcctSchemaElement.ELEMENTTYPE_Organization, _VAF_Org_ID));
            }
            //	Optional BPartner
            if (_VAB_BusinessPartner_ID != 0 && _VAB_BusinessPartner_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _VAPA_FinancialReportingOrder_ID, MAcctSchemaElement.ELEMENTTYPE_BPartner, _VAB_BusinessPartner_ID));
            }
            //	Optional Product
            if (_M_Product_ID != 0 && _M_Product_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _VAPA_FinancialReportingOrder_ID, MAcctSchemaElement.ELEMENTTYPE_Product, _M_Product_ID));
            }
            //	Optional Project
            if (_VAB_Project_ID != 0 && _VAB_Project_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _VAPA_FinancialReportingOrder_ID, MAcctSchemaElement.ELEMENTTYPE_Project, _VAB_Project_ID));
            }
            //	Optional Activity
            if (_VAB_BillingCode_ID != 0 && _VAB_BillingCode_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _VAPA_FinancialReportingOrder_ID, MAcctSchemaElement.ELEMENTTYPE_Activity, _VAB_BillingCode_ID));
            }
            //	Optional Campaign
            if (_VAB_Promotion_ID != 0 && _VAB_Promotion_ID != -1)
            {
                m_parameterWhere.Append(" AND VAB_Promotion_ID=").Append(_VAB_Promotion_ID);
            }
            //	m_parameterWhere.append(" AND ").append(MReportTree.getWhereClause(getCtx(), 
            //		MAcctSchemaElement.ELEMENTTYPE_Campaign, _VAB_Promotion_ID));
            //	Optional Sales Region
            if (_VAB_SalesRegionState_ID != 0 && _VAB_SalesRegionState_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _VAPA_FinancialReportingOrder_ID, MAcctSchemaElement.ELEMENTTYPE_SalesRegion, _VAB_SalesRegionState_ID));
            }
            //	Mandatory Posting Type
            m_parameterWhere.Append(" AND PostingType='").Append(_PostingType).Append("'");
            //
            SetDateAcct();
            sb.Append(" - DateAcct ").Append(_DateAcct_From).Append("-").Append(_DateAcct_To);
            sb.Append(" - Where=").Append(m_parameterWhere);
            log.Fine(sb.ToString());
        }	//	prepare

        /// <summary>
        /// Set Start/End Date of Report - if not defined current Month
        /// </summary>
        private void SetDateAcct()
        {
            //	Date defined
            if (_DateAcct_From != null)
            {
                if (_DateAcct_To == null)
                {
                    //_DateAcct_To = new Timestamp(System.currentTimeMillis());
                    _DateAcct_To = DateTime.Now;
                }
                return;
            }
            //	Get Date from Period
            if (_VAB_YearPeriod_ID == 0 || _VAB_YearPeriod_ID == -1)
            {
                //GregorianCalendar cal = new GregorianCalendar(Login.Language.GetLoginLanguage());
                //cal.setTimeInMillis(CommonFunctions.CurrentTimeMillis());
                //cal.AddHours(DateTime.Now, 0);//;set(Calendar.HOUR_OF_DAY, 0);
                //cal.AddMinutes( set(Calendar.MINUTE, 0);
                //cal.set(Calendar.SECOND, 0);
                //cal.set(Calendar.MILLISECOND, 0);
                //cal.set(Calendar.DAY_OF_MONTH, 1);		//	set to first of month
                _DateAcct_From = DateTime.Now.Date.AddDays(-(DateTime.Now.Day - 1));
                //cal.add(Calendar.MONTH, 1);
                //cal.add(Calendar.DAY_OF_YEAR, -1);		//	last of month
                _DateAcct_To = DateTime.Now.Date.AddMonths(1);                                           
               _DateAcct_To = _DateAcct_To.Value.AddDays(-(_DateAcct_To.Value.Day));
                return;
            }
         
            String sql = "SELECT StartDate, EndDate FROM VAB_YearPeriod WHERE VAB_YearPeriod_ID=@param1";
            //PreparedStatement pstmt = null;
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, null);
                param[0] = new SqlParameter("@param1", _VAB_YearPeriod_ID);
                //pstmt.setInt(1, _VAB_YearPeriod_ID);
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    _DateAcct_From = Utility.Util.GetValueOfDateTime(idr[0]);// rs.getTimestamp(1);
                    _DateAcct_To = Utility.Util.GetValueOfDateTime(idr[1]);// rs.getTimestamp(2);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            
        }	//	setDateAcct


        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message to be translated</returns>
        protected override String DoIt()
        {
            CreateBalanceLine();
            CreateDetailLines();

            //	int VAF_Print_Rpt_Layout_ID = 134;
            //	getProcessInfo().setTransientObject (MPrintFormat.get (getCtx(), VAF_Print_Rpt_Layout_ID, false));

            log.Fine((CommonFunctions.CurrentTimeMillis() - m_start) + " ms");
            return "";
        }	//	doIt

        /// <summary>
        /// Create Beginning Balance Line
        /// </summary>
        private void CreateBalanceLine()
        {
            StringBuilder sql = new StringBuilder(_insert);
            //	(VAF_JInstance_ID, Actual_Acct_Detail_ID,
            sql.Append("SELECT ").Append(GetVAF_JInstance_ID()).Append(",0,");
            //	VAF_Client_ID, VAF_Org_ID, Created,CreatedBy, Updated,UpdatedBy,
            sql.Append(GetVAF_Client_ID()).Append(",");
            if (_VAF_Org_ID == 0 || _VAF_Org_ID==-1)
            {
                sql.Append("0");
            }
            else
            {
                sql.Append(_VAF_Org_ID);
            }
            sql.Append(", SysDate,").Append(GetVAF_UserContact_ID())
                .Append(",SysDate,").Append(GetVAF_UserContact_ID()).Append(",");
            //	VAB_AccountBook_ID, Account_ID, AccountValue, DateTrx, DateAcct, VAB_YearPeriod_ID,
            sql.Append(_VAB_AccountBook_ID).Append(",");
            if (_Account_ID == 0 || _Account_ID == -1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_Account_ID);
            }
            if (_AccountValue_From != null)
            {
                sql.Append(",").Append(DataBase.DB.TO_STRING(_AccountValue_From));
            }
            else if (_AccountValue_To != null)
            {
                sql.Append(",' '");
            }
            else
            {
                sql.Append(",null");
            }
            DateTime? balanceDay = _DateAcct_From; // TimeUtil.addDays(_DateAcct_From, -1);
            sql.Append(",null,").Append(DataBase.DB.TO_DATE(balanceDay, true)).Append(",");
            if (_VAB_YearPeriod_ID == 0 || _VAB_YearPeriod_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_VAB_YearPeriod_ID);
            }
            sql.Append(",");
            //	VAF_TableView_ID, Record_ID, Line_ID,
            sql.Append("null,null,null,");
            //	VAGL_Group_ID, VAGL_Budget_ID, VAB_TaxRate_ID, M_Locator_ID, PostingType,
            sql.Append("null,null,null,null,'").Append(_PostingType).Append("',");
            //	VAB_Currency_ID, AmtSourceDr, AmtSourceCr, AmtSourceBalance,
            sql.Append("null,null,null,null,");
            //	AmtAcctDr, AmtAcctCr, AmtAcctBalance, VAB_UOM_ID, Qty,
            sql.Append(" COALESCE(SUM(AmtAcctDr),0),COALESCE(SUM(AmtAcctCr),0),"
                      + "COALESCE(SUM(AmtAcctDr),0)-COALESCE(SUM(AmtAcctCr),0),"
                + " null,COALESCE(SUM(Qty),0),");
            //	M_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,
            if (_M_Product_ID == 0 ||_M_Product_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_M_Product_ID);
            }
            sql.Append(",");
            if (_VAB_BusinessPartner_ID == 0 || _VAB_BusinessPartner_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_VAB_BusinessPartner_ID);
            }
            sql.Append(",");
            if (_VAF_OrgTrx_ID == 0 || _VAF_OrgTrx_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_VAF_OrgTrx_ID);
            }
            sql.Append(",");
            if (_C_LocFrom_ID == 0 || _C_LocFrom_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_LocFrom_ID);
            }
            sql.Append(",");
            if (_C_LocTo_ID == 0 || _C_LocTo_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_LocTo_ID);
            }
            sql.Append(",");
            //	VAB_SalesRegionState_ID, VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID,
            if (_VAB_SalesRegionState_ID == 0 || _VAB_SalesRegionState_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_VAB_SalesRegionState_ID);
            }
            sql.Append(",");
            if (_VAB_Project_ID == 0 || _VAB_Project_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_VAB_Project_ID);
            }
            sql.Append(",");
            if (_VAB_Promotion_ID == 0 || _VAB_Promotion_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_VAB_Promotion_ID);
            }
            sql.Append(",");
            if (_VAB_BillingCode_ID == 0 || _VAB_BillingCode_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_VAB_BillingCode_ID);
            }
            sql.Append(",");
            //	User1_ID, User2_ID, VAA_Asset_ID, Description)
            if (_User1_ID == 0 || _User1_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_User1_ID);
            }
            sql.Append(",");
            if (_User2_ID == 0 || _User2_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_User2_ID);
            }
            sql.Append(", null,null");
            //
            sql.Append(" FROM Actual_Acct_Detail WHERE VAF_Client_ID=").Append(GetVAF_Client_ID())
                .Append(" AND ").Append(m_parameterWhere)
                .Append(" AND DateAcct < ").Append(DataBase.DB.TO_DATE(_DateAcct_From, true));
            //	Start Beginning of Year
            if (_Account_ID >0)
            {
                m_acct = new MElementValue(GetCtx(), _Account_ID, Get_Trx());
                if (!m_acct.IsBalanceSheet())
                {
                    MPeriod first = MPeriod.GetFirstInYear(GetCtx(), _DateAcct_From);
                    if (first != null)
                    {
                        sql.Append(" AND DateAcct >= ").Append(DataBase.DB.TO_DATE(first.GetStartDate(), true));
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "first period not found");
                    }
                }
            }
            //
            int no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_Trx());
            if (no == 0)
            {
                log.Fine(sql.ToString());
            }
            log.Fine("#" + no + " (Account_ID=" + _Account_ID + ")");
        }	//	createBalanceLine

        /// <summary>
        /// Create Beginning Balance Line
        /// </summary>
        private void CreateDetailLines()
        {
            StringBuilder sql = new StringBuilder(_insert);
            //	(VAF_JInstance_ID, Actual_Acct_Detail_ID,
            sql.Append("SELECT ").Append(GetVAF_JInstance_ID()).Append(",Actual_Acct_Detail_ID,");
            //	VAF_Client_ID, VAF_Org_ID, Created,CreatedBy, Updated,UpdatedBy,
            sql.Append(GetVAF_Client_ID()).Append(",VAF_Org_ID,Created,CreatedBy, Updated,UpdatedBy,");
            //	VAB_AccountBook_ID, Account_ID, DateTrx, AccountValue, DateAcct, VAB_YearPeriod_ID,
            sql.Append("VAB_AccountBook_ID, Account_ID, null, DateTrx, DateAcct, VAB_YearPeriod_ID,");
            //	VAF_TableView_ID, Record_ID, Line_ID,
            sql.Append("VAF_TableView_ID, Record_ID, Line_ID,");
            //	VAGL_Group_ID, VAGL_Budget_ID, VAB_TaxRate_ID, M_Locator_ID, PostingType,
            sql.Append("VAGL_Group_ID, VAGL_Budget_ID, VAB_TaxRate_ID, M_Locator_ID, PostingType,");
            //	VAB_Currency_ID, AmtSourceDr, AmtSourceCr, AmtSourceBalance,
            sql.Append("VAB_Currency_ID, AmtSourceDr,AmtSourceCr, AmtSourceDr-AmtSourceCr,");
            //	AmtAcctDr, AmtAcctCr, AmtAcctBalance, VAB_UOM_ID, Qty,
            sql.Append(" AmtAcctDr,AmtAcctCr, AmtAcctDr-AmtAcctCr, VAB_UOM_ID,Qty,");
            //	M_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,
            sql.Append("M_Product_ID, VAB_BusinessPartner_ID, VAF_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,");
            //	VAB_SalesRegionState_ID, VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID,
            sql.Append("VAB_SalesRegionState_ID, VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID,");
            //	User1_ID, User2_ID, VAA_Asset_ID, Description)
            sql.Append("User1_ID, User2_ID, VAA_Asset_ID, Description");
            //
            sql.Append(" FROM Actual_Acct_Detail WHERE VAF_Client_ID=").Append(GetVAF_Client_ID())
                .Append(" AND ").Append(m_parameterWhere)
                .Append(" AND DateAcct >= ").Append(DataBase.DB.TO_DATE(_DateAcct_From, true))
                .Append(" AND TRUNC(DateAcct,'DD') <= ").Append(DataBase.DB.TO_DATE(_DateAcct_To, true));
            //
            int no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_Trx());
            if (no == 0)
            {
                log.Fine(sql.ToString());
            }
            log.Fine("#" + no + " (Account_ID=" + _Account_ID + ")");

            //	Update AccountValue
            String sql2 = "UPDATE T_TrialBalance tb SET AccountValue = "
                + "(SELECT Value FROM VAB_Acct_Element ev WHERE ev.VAB_Acct_Element_ID=tb.Account_ID) "
                + "WHERE tb.Account_ID IS NOT NULL";
            no = DataBase.DB.ExecuteQuery(sql2,null, Get_Trx());
            if (no > 0)
            {
                log.Fine("Set AccountValue #" + no);
            }

        }	//	createDetailLines

    }	//	TrialBalance
}
