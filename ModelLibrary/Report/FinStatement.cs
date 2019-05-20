/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : FinStatement
 * Purpose        : Statement of Account
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           13-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Model;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Login;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Print;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Report
{ 
    public class FinStatement:ProcessEngine.SvrProcess
    {
        /** AcctSchame Parameter			*/
        private int _C_AcctSchema_ID = 0;
        /** Posting Type					*/
        private String _PostingType = "A";
        /**	Period Parameter				*/
        private int _C_Period_ID = 0;
        private DateTime? _DateAcct_From = null;
        private DateTime? _DateAcct_To = null;
        /**	Org Parameter					*/
        private int _AD_Org_ID = 0;
        /**	Account Parameter				*/
        private int _Account_ID = 0;
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
        /** Update Balances Parameter		*/
        private Boolean _UpdateBalances = true;
        /** Hierarchy						*/
        private int _PA_Hierarchy_ID = 0;

        /**	Parameter Where Clause			*/
        private StringBuilder _parameterWhere = new StringBuilder();
        /**	Account							*/
        private MElementValue _acct = null;

        /**	Start Time						*/
        private long _start = CommonFunctions.CurrentTimeMillis(); //System.currentTimeMillis();

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            StringBuilder sb = new StringBuilder("Record_ID=")
                .Append(GetRecord_ID());
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
                    _C_AcctSchema_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                }
                else if (name.Equals("PostingType"))
                {
                    _PostingType = (String)para[i].GetParameter();
                }
                else if (name.Equals("C_Period_ID"))
                {
                    _C_Period_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
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
                    _AD_Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                }
                else if (name.Equals("Account_ID"))
                {
                    _Account_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("M_Product_ID"))
                {
                    _M_Product_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_Project_ID"))
                {
                    _C_Project_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_Activity_ID"))
                {
                    _C_Activity_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_SalesRegion_ID"))
                {
                    _C_SalesRegion_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_Campaign_ID"))
                {
                    _C_Campaign_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("UpdateBalances"))
                {
                    _UpdateBalances = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            //	Mandatory C_AcctSchema_ID, PostingType
            _parameterWhere.Append("C_AcctSchema_ID=").Append(_C_AcctSchema_ID)
                .Append(" AND PostingType='").Append(_PostingType).Append("'");
            //	Optional Account_ID
            if (_Account_ID != 0 && _Account_ID != -1)
            {
                _parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Account, _Account_ID));
            }
            //	Optional Org
            if (_AD_Org_ID != 0 && _AD_Org_ID != -1)
            {
                _parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Organization, _AD_Org_ID));
            }
            //	Optional BPartner
            if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
            {
                _parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_BPartner, _C_BPartner_ID));
            }
            //	Optional Product
            if (_M_Product_ID != 0 && _M_Product_ID != -1)
            {
                _parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Product, _M_Product_ID));
            }
            //	Optional Project
            if (_C_Project_ID != 0 && _C_Project_ID != -1)
            {
                _parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Project, _C_Project_ID));
            }
            //	Optional Activity
            if (_C_Activity_ID != 0 && _C_Activity_ID != -1)
            {
                _parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Activity, _C_Activity_ID));
            }
            //	Optional Campaign
            if (_C_Campaign_ID != 0 && _C_Campaign_ID != -1)
            {
                _parameterWhere.Append(" AND C_Campaign_ID=").Append(_C_Campaign_ID);
            }
            //	_parameterWhere.append(" AND ").append(MReportTree.getWhereClause(getCtx(), 
            //		MAcctSchemaElement.ELEMENTTYPE_Campaign, _C_Campaign_ID));
            //	Optional Sales Region
            if (_C_SalesRegion_ID != 0 && _C_SalesRegion_ID != -1)
            {
                _parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(),
                    _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_SalesRegion, _C_SalesRegion_ID));
            }
            //
            SetDateAcct();
            sb.Append(" - DateAcct ").Append(_DateAcct_From).Append("-").Append(_DateAcct_To);
            sb.Append(" - Where=").Append(_parameterWhere);
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
                    _DateAcct_To = DateTime.Now;// new Timestamp(System.currentTimeMillis());
                }
                return;
            }
            //	Get Date from Period
            if (_C_Period_ID == 0 || _C_Period_ID==-1)
            {
                //GregorianCalendar cal = new GregorianCalendar(Language.getLoginLanguage().getLocale());
                //cal.setTimeInMillis(System.currentTimeMillis());
                //cal.set(Calendar.HOUR_OF_DAY, 0);
                //cal.set(Calendar.MINUTE, 0);
                //cal.set(Calendar.SECOND, 0);
                //cal.set(Calendar.MILLISECOND, 0);
                //cal.set(Calendar.DAY_OF_MONTH, 1);		//	set to first of month
                //_DateAcct_From = new Timestamp(cal.getTimeInMillis());
                _DateAcct_From=DateTime.Now.Date.AddDays(-(DateTime.Now.Day - 1));
                //cal.add(Calendar.MONTH, 1);
                //cal.add(Calendar.DAY_OF_YEAR, -1);		//	last of month
                //_DateAcct_To = new Timestamp(cal.getTimeInMillis());
                _DateAcct_To = DateTime.Now.Date.AddMonths(1);
                _DateAcct_To = _DateAcct_To.Value.AddDays(-(_DateAcct_To.Value.Day));
                return;
            }

            String sql = "SELECT StartDate, EndDate FROM C_Period WHERE C_Period_ID=@param";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, _C_Period_ID);
                param[0] = new SqlParameter("@param", _C_Period_ID);
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    _DateAcct_From = Utility.Util.GetValueOfDateTime(idr[0]); //rs.getTimestamp(1);
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
            //	Update AcctSchema Balances
            if (_UpdateBalances)
            {
                //FinBalance.UpdateBalance(_C_AcctSchema_ID, false);
                FinBalance.UpdateBalance(GetCtx(), _C_AcctSchema_ID,
                   _DateAcct_From, Get_TrxName(), 0, this);

            }

            CreateBalanceLine();
            CreateDetailLines();

            int AD_PrintFormat_ID = 134;
            if (Ini.IsClient())
            {
                GetProcessInfo().SetTransientObject(MPrintFormat.Get(GetCtx(), AD_PrintFormat_ID, false));
            }
            else
            {
                GetProcessInfo().SetSerializableObject(MPrintFormat.Get(GetCtx(), AD_PrintFormat_ID, false));
            }

            log.Fine((CommonFunctions.CurrentTimeMillis() - _start) + " ms");
            return "";
        }	//	doIt

        /// <summary>
        /// Create Beginning Balance Line
        /// </summary>
        private void CreateBalanceLine()
        {
            StringBuilder sb = new StringBuilder("INSERT INTO T_ReportStatement "
                + "(AD_PInstance_ID, Fact_Acct_ID, LevelNo,"
                + "DateAcct, Name, Description,"
                + "AmtAcctDr, AmtAcctCr, Balance, Qty) ");
            sb.Append("SELECT ").Append(GetAD_PInstance_ID()).Append(",0,0,")
                .Append(DataBase.DB.TO_DATE(_DateAcct_From, true)).Append(",")
                .Append(DataBase.DB.TO_STRING(Msg.GetMsg(GetCtx(), "BeginningBalance"))).Append(",NULL,"
                + "COALESCE(SUM(AmtAcctDr),0), COALESCE(SUM(AmtAcctCr),0), COALESCE(SUM(AmtAcctDr-AmtAcctCr),0), COALESCE(SUM(Qty),0) "
                + "FROM Fact_Acct_Balance "
                + "WHERE ").Append(_parameterWhere)
                .Append(" AND DateAcct < ").Append(DataBase.DB.TO_DATE(_DateAcct_From));

            //	Start Beginning of Year
            if (_Account_ID > 0)
            {
                _acct = new MElementValue(GetCtx(), _Account_ID, Get_TrxName());
                if (!_acct.IsBalanceSheet())
                {
                    MPeriod first = MPeriod.GetFirstInYear(GetCtx(), _DateAcct_From);
                    if (first != null)
                    {
                        sb.Append(" AND DateAcct >= ").Append(DataBase.DB.TO_DATE(first.GetStartDate()));
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "First period not found");
                    }
                }
            }
            //
            int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
            log.Fine("#" + no + " (Account_ID=" + _Account_ID + ")");
            log.Finest(sb.ToString());
        }	//	createBalanceLine

        /// <summary>
        /// Create Beginning Balance Line
       /// </summary>
       // vikas 4 -dec-2016 Update Running Balnc
        private void CreateDetailLines()
        {
            decimal balnc = 0;
            decimal lastbalnc = 0;
            StringBuilder sb = new StringBuilder("INSERT INTO T_ReportStatement "
                + "(AD_PInstance_ID, Fact_Acct_ID, LevelNo,"
                + "DateAcct,SeqNo, Name, Description,"
                + "AmtAcctDr, AmtAcctCr, Balance, Qty) ");
            sb.Append("SELECT ").Append(GetAD_PInstance_ID()).Append(",Fact_Acct_ID,1,")
                .Append("DateAcct,rownum,NULL,NULL,"
                + "AmtAcctDr, AmtAcctCr, AmtAcctDr-AmtAcctCr, Qty "
                + "FROM Fact_Acct "
                + "WHERE ").Append(_parameterWhere)
                .Append(" AND DateAcct BETWEEN ").Append(DataBase.DB.TO_DATE(_DateAcct_From))
                .Append(" AND ").Append(DataBase.DB.TO_DATE(_DateAcct_To));
            //
            int no = DataBase.DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
            log.Fine("#" + no);
            log.Finest(sb.ToString());

            //	Set Name,Description
            String sql_select = "SELECT e.Name, fa.Description "
                + "FROM Fact_Acct fa"
                + " INNER JOIN AD_Table t ON (fa.AD_Table_ID=t.AD_Table_ID)"
                + " INNER JOIN AD_Element e ON (t.TableName||'_ID'=e.ColumnName) "
                + "WHERE r.Fact_Acct_ID=fa.Fact_Acct_ID";
            //	Translated Version ...
            sb = new StringBuilder("UPDATE T_ReportStatement r SET (Name,Description)=(")
                .Append(sql_select).Append(") "
                + "WHERE Fact_Acct_ID <> 0 AND AD_PInstance_ID=").Append(GetAD_PInstance_ID());
            //
            no = DataBase.DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
            log.Fine("Name #" + no);
            log.Finest("Name - " + sb);

            // new
            sb.Clear();
            DataSet ds = null;
            int _count = 0;
            sb.Append("SELECT  levelno, seqno, dateacct, description, amtacctdr, amtacctcr, balance, name FROM T_ReportStatement WHERE AD_PInstance_ID=" + GetAD_PInstance_ID() + " order by  levelno,dateacct,name,description");
            ds = DB.ExecuteDataset(sb.ToString(), null, Get_TrxName());
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        // calculate running balnc
                        if (i == 0)
                        {
                            Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["balance"]);
                            decimal begbalnc = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT balance from t_reportstatement where ad_pinstance_id=" + GetAD_PInstance_ID(), null, Get_TrxName()));
                            //  balnc = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["amtacctdr"]) - Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["amtacctcr"]);
                            balnc = begbalnc;
                            lastbalnc = balnc;
                        }
                        if (i > 0)
                        {
                            balnc = 0;

                            balnc = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["amtacctdr"]) - Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["amtacctcr"]);
                            if (balnc > 0)
                            {
                                balnc = lastbalnc + balnc;
                                lastbalnc = balnc;
                            }
                            else
                            {
                                balnc = lastbalnc + balnc;
                                lastbalnc = balnc;
                            }
                        }
                        _count++;
                        sb.Clear();
                        sb.Append("UPDATE T_ReportStatement  SET  Counter=" + _count + " , balance=" + lastbalnc + " WHERE ad_pinstance_id=" + GetAD_PInstance_ID() + " AND seqno=" + Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["seqno"]));
                        DB.ExecuteQuery(sb.ToString(), null, Get_TrxName());
                    }
                }
            }
        }	//	createDetailLines
        //End

    }	//	FinStatement

}




     //private void CreateDetailLines()
     //   {
     //       StringBuilder sb = new StringBuilder("INSERT INTO T_ReportStatement "
     //           + "(AD_PInstance_ID, Fact_Acct_ID, LevelNo,"
     //           + "DateAcct, Name, Description,"
     //           + "AmtAcctDr, AmtAcctCr, Balance, Qty) ");
     //       sb.Append("SELECT ").Append(GetAD_PInstance_ID()).Append(",Fact_Acct_ID,1,")
     //           .Append("DateAcct,NULL,NULL,"
     //           + "AmtAcctDr, AmtAcctCr, AmtAcctDr-AmtAcctCr, Qty "
     //           + "FROM Fact_Acct "
     //           + "WHERE ").Append(_parameterWhere)
     //           .Append(" AND DateAcct BETWEEN ").Append(DataBase.DB.TO_DATE(_DateAcct_From))
     //           .Append(" AND ").Append(DataBase.DB.TO_DATE(_DateAcct_To));
     //       //
     //       int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
     //       log.Fine("#" + no);
     //       log.Finest(sb.ToString());

     //       //	Set Name,Description
     //       String sql_select = "SELECT e.Name, fa.Description "
     //           + "FROM Fact_Acct fa"
     //           + " INNER JOIN AD_Table t ON (fa.AD_Table_ID=t.AD_Table_ID)"
     //           + " INNER JOIN AD_Element e ON (t.TableName||'_ID'=e.ColumnName) "
     //           + "WHERE r.Fact_Acct_ID=fa.Fact_Acct_ID";
     //       //	Translated Version ...
     //       sb = new StringBuilder("UPDATE T_ReportStatement r SET (Name,Description)=(")
     //           .Append(sql_select).Append(") "
     //           + "WHERE Fact_Acct_ID <> 0 AND AD_PInstance_ID=").Append(GetAD_PInstance_ID());
     //       //
     //       no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
     //       log.Fine("Name #" + no);
     //       log.Finest("Name - " + sb);

     //   }	//	createDetailLines

