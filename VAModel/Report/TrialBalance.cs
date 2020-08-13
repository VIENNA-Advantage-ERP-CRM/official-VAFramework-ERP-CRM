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
        private int _C_AcctSchema_ID = 0;
        /**	Period Parameter				*/
        private int _C_Period_ID = 0;
        private DateTime? _DateAcct_From = null;
        private DateTime? _DateAcct_To = null;
        /**	Org Parameter					*/
        private int _AD_Org_ID = 0;
        /**	Account Parameter				*/
        private int _Account_ID = 0;
        private String _AccountValue_From = null;
        private String _AccountValue_To = null;
        /**	BPartner Parameter				*/
        private int _C_BPartner_ID = 0;
        /**	Product Parameter				*/
        private int _M_Product_ID = 0;
        /**	Project Parameter				*/
        private int _C_Project_ID = 0;
        /**	Activity Parameter				*/
        private int _C_Activity_ID = 0;
        /**	SalesRegion Parameter			*/
        private int _C_SalesRegion_ID = 0;
        /**	Campaign Parameter				*/
        private int _C_Campaign_ID = 0;
        /** Posting Type					*/
        private String _PostingType = "A";
        /** Hierarchy						*/
        private int _PA_Hierarchy_ID = 0;

        private int _AD_OrgTrx_ID = 0;
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
            + "(AD_PInstance_ID, Fact_Acct_ID,"
            + " AD_Client_ID, AD_Org_ID, Created,CreatedBy, Updated,UpdatedBy,"
            + " C_AcctSchema_ID, Account_ID, AccountValue, DateTrx, DateAcct, C_Period_ID,"
            + " AD_Table_ID, Record_ID, Line_ID,"
            + " GL_Category_ID, GL_Budget_ID, C_Tax_ID, M_Locator_ID, PostingType,"
            + " C_Currency_ID, AmtSourceDr, AmtSourceCr, AmtSourceBalance,"
            + " AmtAcctDr, AmtAcctCr, AmtAcctBalance, C_UOM_ID, Qty,"
            + " M_Product_ID, C_BPartner_ID, AD_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,"
            + " C_SalesRegion_ID, C_Project_ID, C_Campaign_ID, C_Activity_ID,"
            + " User1_ID, User2_ID, A_Asset_ID, Description)";


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            StringBuilder sb = new StringBuilder("AD_PInstance_ID=")
                .Append(GetAD_PInstance_ID());
            //	Parameter
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_AcctSchema_ID"))
                {
                    _C_AcctSchema_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_Period_ID"))
                {
                    _C_Period_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("DateAcct"))
                {
                    _DateAcct_From = Utility.Util.GetValueOfDateTime(para[i].GetParameter());
                    _DateAcct_To = Utility.Util.GetValueOfDateTime(para[i].GetParameter_To());
                }
                else if (name.Equals("PA_Hierarchy_ID"))
                {
                    _PA_Hierarchy_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    _AD_Org_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
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
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("M_Product_ID"))
                {
                    _M_Product_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_Project_ID"))
                {
                    _C_Project_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_Activity_ID"))
                {
                    _C_Activity_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_SalesRegion_ID"))
                {
                    _C_SalesRegion_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_Campaign_ID"))
                {
                    _C_Campaign_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());
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
            //	Mandatory C_AcctSchema_ID
            m_parameterWhere.Append("C_AcctSchema_ID=").Append(_C_AcctSchema_ID);
            //	Optional Account_ID
            if (_Account_ID != 0 && _Account_ID!=-1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Account, _Account_ID));
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
                m_parameterWhere.Append(" AND (Account_ID IS NULL OR EXISTS (SELECT * FROM C_ElementValue ev ")
                    .Append("WHERE Account_ID=ev.C_ElementValue_ID AND ev.Value >= ")
                    .Append(DataBase.DB.TO_STRING(_AccountValue_From)).Append(" AND ev.Value <= ")
                    .Append(DataBase.DB.TO_STRING(_AccountValue_To)).Append("))");
            }
            else if (_AccountValue_From != null && _AccountValue_To == null)
            {
                m_parameterWhere.Append(" AND (Account_ID IS NULL OR EXISTS (SELECT * FROM C_ElementValue ev ")
                .Append("WHERE Account_ID=ev.C_ElementValue_ID AND ev.Value >= ")
                .Append(DataBase.DB.TO_STRING(_AccountValue_From)).Append("))");
            }
            else if (_AccountValue_From == null && _AccountValue_To != null)
            {
                m_parameterWhere.Append(" AND (Account_ID IS NULL OR EXISTS (SELECT * FROM C_ElementValue ev ")
                .Append("WHERE Account_ID=ev.C_ElementValue_ID AND ev.Value <= ")
                .Append(DataBase.DB.TO_STRING(_AccountValue_To)).Append("))");
            }
            //	Optional Org
            if (_AD_Org_ID != 0 && _AD_Org_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Organization, _AD_Org_ID));
            }
            //	Optional BPartner
            if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_BPartner, _C_BPartner_ID));
            }
            //	Optional Product
            if (_M_Product_ID != 0 && _M_Product_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Product, _M_Product_ID));
            }
            //	Optional Project
            if (_C_Project_ID != 0 && _C_Project_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Project, _C_Project_ID));
            }
            //	Optional Activity
            if (_C_Activity_ID != 0 && _C_Activity_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Activity, _C_Activity_ID));
            }
            //	Optional Campaign
            if (_C_Campaign_ID != 0 && _C_Campaign_ID != -1)
            {
                m_parameterWhere.Append(" AND C_Campaign_ID=").Append(_C_Campaign_ID);
            }
            //	m_parameterWhere.append(" AND ").append(MReportTree.getWhereClause(getCtx(), 
            //		MAcctSchemaElement.ELEMENTTYPE_Campaign, _C_Campaign_ID));
            //	Optional Sales Region
            if (_C_SalesRegion_ID != 0 && _C_SalesRegion_ID != -1)
            {
                m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_SalesRegion, _C_SalesRegion_ID));
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
            if (_C_Period_ID == 0 || _C_Period_ID == -1)
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
         
            String sql = "SELECT StartDate, EndDate FROM C_Period WHERE C_Period_ID=@param1";
            //PreparedStatement pstmt = null;
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, null);
                param[0] = new SqlParameter("@param1", _C_Period_ID);
                //pstmt.setInt(1, _C_Period_ID);
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

            //	int AD_PrintFormat_ID = 134;
            //	getProcessInfo().setTransientObject (MPrintFormat.get (getCtx(), AD_PrintFormat_ID, false));

            log.Fine((CommonFunctions.CurrentTimeMillis() - m_start) + " ms");
            return "";
        }	//	doIt

        /// <summary>
        /// Create Beginning Balance Line
        /// </summary>
        private void CreateBalanceLine()
        {
            StringBuilder sql = new StringBuilder(_insert);
            //	(AD_PInstance_ID, Fact_Acct_ID,
            sql.Append("SELECT ").Append(GetAD_PInstance_ID()).Append(",0,");
            //	AD_Client_ID, AD_Org_ID, Created,CreatedBy, Updated,UpdatedBy,
            sql.Append(GetAD_Client_ID()).Append(",");
            if (_AD_Org_ID == 0 || _AD_Org_ID==-1)
            {
                sql.Append("0");
            }
            else
            {
                sql.Append(_AD_Org_ID);
            }
            sql.Append(", SysDate,").Append(GetAD_User_ID())
                .Append(",SysDate,").Append(GetAD_User_ID()).Append(",");
            //	C_AcctSchema_ID, Account_ID, AccountValue, DateTrx, DateAcct, C_Period_ID,
            sql.Append(_C_AcctSchema_ID).Append(",");
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
            if (_C_Period_ID == 0 || _C_Period_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_Period_ID);
            }
            sql.Append(",");
            //	AD_Table_ID, Record_ID, Line_ID,
            sql.Append("null,null,null,");
            //	GL_Category_ID, GL_Budget_ID, C_Tax_ID, M_Locator_ID, PostingType,
            sql.Append("null,null,null,null,'").Append(_PostingType).Append("',");
            //	C_Currency_ID, AmtSourceDr, AmtSourceCr, AmtSourceBalance,
            sql.Append("null,null,null,null,");
            //	AmtAcctDr, AmtAcctCr, AmtAcctBalance, C_UOM_ID, Qty,
            sql.Append(" COALESCE(SUM(AmtAcctDr),0),COALESCE(SUM(AmtAcctCr),0),"
                      + "COALESCE(SUM(AmtAcctDr),0)-COALESCE(SUM(AmtAcctCr),0),"
                + " null,COALESCE(SUM(Qty),0),");
            //	M_Product_ID, C_BPartner_ID, AD_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,
            if (_M_Product_ID == 0 ||_M_Product_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_M_Product_ID);
            }
            sql.Append(",");
            if (_C_BPartner_ID == 0 || _C_BPartner_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_BPartner_ID);
            }
            sql.Append(",");
            if (_AD_OrgTrx_ID == 0 || _AD_OrgTrx_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_AD_OrgTrx_ID);
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
            //	C_SalesRegion_ID, C_Project_ID, C_Campaign_ID, C_Activity_ID,
            if (_C_SalesRegion_ID == 0 || _C_SalesRegion_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_SalesRegion_ID);
            }
            sql.Append(",");
            if (_C_Project_ID == 0 || _C_Project_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_Project_ID);
            }
            sql.Append(",");
            if (_C_Campaign_ID == 0 || _C_Campaign_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_Campaign_ID);
            }
            sql.Append(",");
            if (_C_Activity_ID == 0 || _C_Activity_ID==-1)
            {
                sql.Append("null");
            }
            else
            {
                sql.Append(_C_Activity_ID);
            }
            sql.Append(",");
            //	User1_ID, User2_ID, A_Asset_ID, Description)
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
            sql.Append(" FROM Fact_Acct WHERE AD_Client_ID=").Append(GetAD_Client_ID())
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
            //	(AD_PInstance_ID, Fact_Acct_ID,
            sql.Append("SELECT ").Append(GetAD_PInstance_ID()).Append(",Fact_Acct_ID,");
            //	AD_Client_ID, AD_Org_ID, Created,CreatedBy, Updated,UpdatedBy,
            sql.Append(GetAD_Client_ID()).Append(",AD_Org_ID,Created,CreatedBy, Updated,UpdatedBy,");
            //	C_AcctSchema_ID, Account_ID, DateTrx, AccountValue, DateAcct, C_Period_ID,
            sql.Append("C_AcctSchema_ID, Account_ID, null, DateTrx, DateAcct, C_Period_ID,");
            //	AD_Table_ID, Record_ID, Line_ID,
            sql.Append("AD_Table_ID, Record_ID, Line_ID,");
            //	GL_Category_ID, GL_Budget_ID, C_Tax_ID, M_Locator_ID, PostingType,
            sql.Append("GL_Category_ID, GL_Budget_ID, C_Tax_ID, M_Locator_ID, PostingType,");
            //	C_Currency_ID, AmtSourceDr, AmtSourceCr, AmtSourceBalance,
            sql.Append("C_Currency_ID, AmtSourceDr,AmtSourceCr, AmtSourceDr-AmtSourceCr,");
            //	AmtAcctDr, AmtAcctCr, AmtAcctBalance, C_UOM_ID, Qty,
            sql.Append(" AmtAcctDr,AmtAcctCr, AmtAcctDr-AmtAcctCr, C_UOM_ID,Qty,");
            //	M_Product_ID, C_BPartner_ID, AD_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,
            sql.Append("M_Product_ID, C_BPartner_ID, AD_OrgTrx_ID, C_LocFrom_ID,C_LocTo_ID,");
            //	C_SalesRegion_ID, C_Project_ID, C_Campaign_ID, C_Activity_ID,
            sql.Append("C_SalesRegion_ID, C_Project_ID, C_Campaign_ID, C_Activity_ID,");
            //	User1_ID, User2_ID, A_Asset_ID, Description)
            sql.Append("User1_ID, User2_ID, A_Asset_ID, Description");
            //
            sql.Append(" FROM Fact_Acct WHERE AD_Client_ID=").Append(GetAD_Client_ID())
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
                + "(SELECT Value FROM C_ElementValue ev WHERE ev.C_ElementValue_ID=tb.Account_ID) "
                + "WHERE tb.Account_ID IS NOT NULL";
            no = DataBase.DB.ExecuteQuery(sql2,null, Get_Trx());
            if (no > 0)
            {
                log.Fine("Set AccountValue #" + no);
            }

        }	//	createDetailLines

    }	//	TrialBalance
}
