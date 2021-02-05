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
using VAdvantage.Report;

namespace ViennaAdvantage.Process
{
    public class AccoutingReport : SvrProcess
    {
        #region Variables
        /** AcctSchame Parameter			*/
        //private int _VAB_AccountBook_ID = 0;
        /**	Period Parameter				*/
        private int _VAB_YearPeriod_ID = 0;
        private int _VAGL_Budget_ID = 0;
        private DateTime? _DateAcct_From = null;
        private DateTime? _DateAcct_To = null;
        private DateTime? _DateAcct_Yearly = null;
        /**	Org Parameter					*/
        //private int _VAF_Org_ID = 0;
        /**	Account Parameter				*/
        private int _Account_ID = 0;
        int VAGL_Budget_ID = 0;
        //private String _AccountValue_From = null;
        //private String _AccountValue_To = null;
        /**	BPartner Parameter				*/
        private int _VAB_BusinessPartner_ID = 0;
        /**	Product Parameter				*/
        private int _VAM_Product_ID = 0;
        /**	Project Parameter				*/
        private int _VAB_Project_ID = 0;
        /**	Activity Parameter				*/
        private int _VAB_BillingCode_ID = 0;
        /**	SalesRegion Parameter			*/
        private int _VAB_SalesRegionState_ID = 0;
        /**	Campaign Parameter				*/
        private int _VAB_Promotion_ID = 0;
       // private int VAF_OrgTrx_ID = 0;

        /** Posting Type					*/
       // private String _PostingType = "A";
        /** Hierarchy						*/
       // private int _VAPA_FinancialReportingOrder_ID = 0;

        private int _VAF_OrgTrx_ID = 0;
        private string _AccountType = "";
        private string _SummaryLevel = "";
        private string _LedgerType = "";
        private int _C_LocFrom_ID = 0;
        private int _C_LocTo_ID = 0;
        private int _User1_ID = 0;
        private int _User2_ID = 0;
        private int _UserElement1_ID = 0;
        private int _UserElement2_ID = 0;
        /**	Parameter Where Clause			*/
        private StringBuilder m_parameterWhere = new StringBuilder();
        private StringBuilder m_datesql = new StringBuilder();
        /**	Account							*/
       // private MElementValue m_acct = null;
        private StringBuilder _msql = new StringBuilder();
        StringBuilder _m_OuterSql = new StringBuilder();
        StringBuilder sql = new StringBuilder();
        StringBuilder _m_CaseSql = new StringBuilder();
        List<int> _ListOfParentId = new List<int>();
        string _sqlUpdate = "";
        //String sqlnew = "";
        String sqlcount = "";
        int _Parent_id = 0;
        Decimal _currentAmount = 0;
        Decimal _PreviousAmount = 0;
        Decimal _totalAmount = 0;
        int _Node_id = 0;
        Decimal _Gl_budgetvalue = 0;
        Decimal _Gl_Totalvalue = 0;
        Decimal _totalBudgetValue = 0;
        StringBuilder _sqlAccount = new StringBuilder();
       // int _NoOfRecord = 0;
        string IsIntermediate = "N";
        private static String _Insert = @"INSERT INTO  T_Statement "
             + "( parent_id, AccountType,VAM_Product_ID,VAB_BUSINESSPARTNER_ID,VAB_PROMOTION_ID,VAB_PROJECT_ID,VAB_BILLINGCODE_ID,VAB_SALESREGIONSTATE_ID,C_LOCFROM_ID,C_LOCTO_ID,USER1_ID,USER2_ID,USERELEMENT1_ID,USERELEMENT2_ID,node_id,VAF_Client_ID, VAF_Org_ID,"
             + "  LedgerCode, LedgerName, CrMonVal ,"
             + "  TPMonValue,  TotalValue,ISSUMMARY,isintermediatecode)";

        //private static String _InsertAccount = @"INSERT INTO  T_Statement "
        //     + "( vaf_client_id, vaf_org_id,ACCGRPNAME,VAB_ACCOUNTGROUP_ID,ACCGRPNAME,PARENTID,NODEID)";
        #endregion

        protected override string DoIt()
        {
            String sql1 = @"Select startdate from VAB_YearPeriod where VAB_YearPeriod_id=" + _VAB_YearPeriod_ID;
            DateTime? startdate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql1));
            sql1 = @"select clfn.VAB_AccountBook1_id from vaf_client cl join VAF_ClientDetail clfn  on (cl.vaf_client_id=clfn.vaf_client_id)
                     where cl.vaf_client_id=" + GetVAF_Client_ID();
            int VAB_AccountBook_id = Util.GetValueOfInt(DB.ExecuteScalar(sql1));
            FinBalance.UpdateBalance(GetCtx(), VAB_AccountBook_id, startdate, Get_Trx(), 0, this);
            if (_LedgerType == "N")
            {
                CreateView();
            }
            else
            {
                CreateViewAsTree();

            }
            string query = "Select count(*) from  T_Statement";
            int no = Util.GetValueOfInt(DB.ExecuteScalar(query, null, null));
            if (no > 0)
            {
                query = "Delete from  T_Statement";

                no = DB.ExecuteQuery(query, null, null);

            }
            // query = "Select count(*) from   t_accounts";
            //_NoOfRecord = Util.GetValueOfInt(DB.ExecuteScalar(query, null, null));
            //if (_NoOfRecord > 0)
            //{
            //    query = "Delete  from   T_Statement";

            //    _NoOfRecord = DB.ExecuteQuery(query, null, null);

            //}
            no = 0;
            //  _NoOfRecord = 0;
            _msql.Append(_Insert);

            _m_OuterSql.Append(@"SELECT distinct t1.parent_id,t1.AccountType,t1.VAM_Product_ID,t1.VAB_BUSINESSPARTNER_ID,t1.VAB_PROMOTION_ID,t1.VAB_PROJECT_ID,t1.VAB_BILLINGCODE_ID,t1.VAB_SALESREGIONSTATE_ID,
    t1.C_LOCFROM_ID,t1.C_LOCTO_ID,t1.USER1_ID,t1.USER2_ID,t1.USERELEMENT1_ID,t1.USERELEMENT2_ID,t1.node_id,t1.vaf_client_id,t1.vaf_org_id,t1.value AS value,t1.name as name,nvl(max(t2.Currentamount),0) AS Currentamount,(nvl(max(t1.Total),0)-nvl(max(t2.Currentamount),0)) AS Previous,
                   (nvl(max(t2.Currentamount),0)+((nvl(max(t1.Total),0)-nvl(max(t2.Currentamount),0)))) AS totalamount,t1.IsSummary as IsSummary,t1.isintermediatecode from ");
            _m_CaseSql.Append(@"(SELECT * from (Select distinct    c.AccountType,f.VAM_Product_ID,f.VAB_BUSINESSPARTNER_ID,f.VAB_PROMOTION_ID,f.VAB_PROJECT_ID,f.VAB_BILLINGCODE_ID,f.VAB_SALESREGIONSTATE_ID,
         f.C_LOCFROM_ID,f.C_LOCTO_ID,f.USER1_ID,f.USER2_ID,f.USERELEMENT1_ID,f.USERELEMENT2_ID,a.parent_id,max(c.issummary) as IsSummary,c.isintermediatecode, a.node_id,c.vaf_client_id,c.vaf_org_id,c.value, c.name, CASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (NVL((SUM(f.amtacctdr)),0)-NVL((SUM(f.amtacctcr)),0)) ELSE (NVL((SUM(f.amtacctcr)),0)-NVL((SUM(f.amtacctdr)),0)) END AS Total");
            sql.Append(" FROM VAF_TreeInfoChild a INNER JOIN VAB_Acct_Element c ON c.VAB_Acct_Element_id = a.node_id  INNER JOIN Actual_Acct_Balance f ON (f.account_id =c.VAB_Acct_Element_id and f.VAGL_Budget_ID is null");
            SetDateAcct();
            _msql.Append(_m_OuterSql);
            _msql.Append(_m_CaseSql);
            _msql.Append(sql);
            m_datesql.Append(" and f.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(_DateAcct_Yearly, true) + " AND " + GlobalVariable.TO_DATE(_DateAcct_To, true) + " ");
            _msql.Append(m_datesql);
            Createsql(true);
            _msql.Append(m_parameterWhere);
            _msql.Append(" t1 Inner join");
            _m_CaseSql = new StringBuilder();
            _m_CaseSql.Append(@"(SELECT * From (Select distinct  c.AccountType,f.VAM_Product_ID,f.VAB_BUSINESSPARTNER_ID,f.VAB_PROMOTION_ID,f.VAB_PROJECT_ID,f.VAB_BILLINGCODE_ID,f.VAB_SALESREGIONSTATE_ID,
               f.C_LOCFROM_ID,f.C_LOCTO_ID,f.USER1_ID,f.USER2_ID,f.USERELEMENT1_ID,f.USERELEMENT2_ID,a.parent_id,max(c.issummary) AS IsSummary,c.isintermediatecode, a.node_id,c.vaf_client_id,c.vaf_org_id,c.value, c.name, CASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (NVL((SUM(f.amtacctdr)),0)-NVL((SUM(f.amtacctcr)),0)) ELSE (NVL((SUM(f.amtacctcr)),0)-NVL((SUM(f.amtacctdr)),0)) END AS Currentamount");
            _msql.Append(_m_CaseSql);
            _msql.Append(sql);
            m_datesql = new StringBuilder();
            m_datesql.Append(" and f.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(_DateAcct_From, true) + " AND " + GlobalVariable.TO_DATE(_DateAcct_To, true) + " ");
            _msql.Append(m_datesql);
            m_parameterWhere = new StringBuilder();
            Createsql(false);
            _msql.Append(m_parameterWhere);
            _msql.Append(" t2  ON (t1.value=t2.value) group by t1.parent_id, t1.AccountType,t1.VAM_Product_ID,  t1.VAB_BUSINESSPARTNER_ID,  t1.VAB_PROMOTION_ID,  t1.VAB_PROJECT_ID,  t1.VAB_BILLINGCODE_ID,  t1.VAB_SALESREGIONSTATE_ID,  t1.C_LOCFROM_ID,  t1.C_LOCTO_ID,  t1.USER1_ID,  t1.USER2_ID,  t1.USERELEMENT1_ID,  t1.USERELEMENT2_ID,  t1.node_id,  t1.vaf_client_id,  t1.vaf_org_id,  t1.value     ,  t1.name  ,  t1.IsSummary,t1.isintermediatecode");
            //_msql.Append(" ");
            //_msql.Append(sql);
            //_msql.Append(sql);
            //            sql.Append(@" SELECT   t1.parent_id,t1.node_id,t1.vaf_client_id,t1.vaf_org_id,t1.Created,t1.CreatedBy,t1.Updated,t1.UpdatedBy,t1.value as value, t1.name name,t2.Currentamount as Currentamount,(t1.Total-t2.Currentamount) as Previous,(t2.Currentamount+((t1.Total-t2.Currentamount))) as totalamount,
            //               t1.VAB_BusinessPartner_ID,t1.VAM_Product_ID,t1.VAB_Project_ID,t1.VAB_BillingCode_ID,
            //            t1.VAB_SalesRegionState_ID,t1.VAB_Promotion_ID,t1.C_LocFrom_ID,t1.C_LocTo_ID  ,t1.User1_ID,t1.User2_ID,t1.UserElement1_ID,t1.UserElement2_ID,t1.ACCOUNTTYPE
            //            FROM(SELECT  g.VAGL_Budget_ID,a.parent_id,a.node_id,f.vaf_client_id,f.vaf_org_id,f.Created,f.CreatedBy,f.Updated,f.UpdatedBy,c.value AS value,f.VAB_BusinessPartner_ID,f.VAM_Product_ID,f.VAB_Project_ID,f.VAB_BillingCode_ID,c.ACCOUNTTYPE,
            //      f.VAB_SalesRegionState_ID,f.VAB_Promotion_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID, c.name,CASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (SUM(f.amtacctdr)-SUM(f.amtacctcr))
            //          Else (SUM(f.amtacctcr)-SUM(f.amtacctdr))END AS Total FROM VAF_TreeInfoChild a INNER JOIN VAB_Acct_Element c ON c.VAB_Acct_Element_id = a.node_id LEFT OUTER JOIN Actual_Acct_Balance f
            //        ON(f.account_id     =c.VAB_Acct_Element_id)  inner join VAGL_Budget g on (f.VAGL_Budget_ID=g.VAGL_Budget_ID)  WHERE c.vaf_client_id=" + GetCtx().GetVAF_Client_ID() + "");
            //              SetDateAcct();
            //              sql.Append("  AND f.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(_DateAcct_Yearly, true) + " AND " + GlobalVariable.TO_DATE(_DateAcct_To, true) + "");
            //            //AND f.DATEACCT BETWEEN '01-Jan-13' AND '31-Dec-13'
            //              sql.Append(@"GROUP BY a.parent_id,a.node_id,c.value, c.name,c.AccountType, f.vaf_client_id,f.vaf_org_id,f.Created,f.CreatedBy,f.Updated,f.UpdatedBy,f.VAB_BusinessPartner_ID,f.VAM_Product_ID,f.VAB_Project_ID,f.VAB_BillingCode_ID,
            //            f.VAB_SalesRegionState_ID,f.VAB_Promotion_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID,g.VAGL_Budget_ID) t1 inner JOIN (SELECT g.VAGL_Budget_ID,a.parent_id,a.node_id,c.value AS value,c.name, cASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (SUM(f.amtacctdr)-SUM(f.amtacctcr))
            //          else (SUM(f.amtacctcr)-SUM(f.amtacctdr)) END AS Currentamount,f.VAB_BusinessPartner_ID,f.VAM_Product_ID,f.VAB_Project_ID,f.VAB_BillingCode_ID,c.ACCOUNTTYPE,
            //      f.VAB_SalesRegionState_ID,f.VAB_Promotion_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID FROM VAF_TreeInfoChild a INNER JOIN VAB_Acct_Element c ON c.VAB_Acct_Element_id = a.node_id LEFT OUTER JOIN Actual_Acct_Balance f
            //       ON(f.account_id     =c.VAB_Acct_Element_id)  inner join VAGL_Budget g on (f.VAGL_Budget_ID=g.VAGL_Budget_ID) WHERE f.vaf_client_id=" + GetCtx().GetVAF_Client_ID() + "  AND f.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(_DateAcct_From, true) + " and " + GlobalVariable.TO_DATE(_DateAcct_To, true) + @"
            //          GROUP BY c.value,c.name,c.AccountType,f.VAB_BusinessPartner_ID,f.VAM_Product_ID,f.VAB_Project_ID,f.VAB_BillingCode_ID,c.ACCOUNTTYPE, f.vaf_client_id,f.vaf_org_id,f.Created,f.CreatedBy,f.Updated,f.UpdatedBy,
            //      f.VAB_SalesRegionState_ID,f.VAB_Promotion_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID,a.parent_id,a.node_id,g.VAGL_Budget_ID) t2 ON (t1.value=t2.value) ");
            no = DB.ExecuteQuery(_msql.ToString(), null, null);
            if (no == 0)
            {
                log.Fine(_msql.ToString());
            }
            if (_VAB_YearPeriod_ID != 0 )
            {
                string Sql_Period = "UPDATE t_statement set VAB_YearPeriod_id =" + _VAB_YearPeriod_ID + "";
                int upd = DB.ExecuteQuery(Sql_Period);
            }
            log.Fine("#" + no + " (Account_ID=" + _Account_ID + ")");
            //            _sqlAccount.Append(_InsertAccount);
            //            _sqlAccount.Append(@"SELECT asg.vaf_client_id as vaf_client_id,asg.vaf_org_id as vaf_org_id,ag.name as accountGroupname,asg.VAB_AccountGroup_id as VAB_AccountGroup_id, asg.name AS AccountsubGroupName,
            //                    nvl(asg.VAB_AccountSubGroup_id_1,0)as parent_id,asg.VAB_AccountSubGroup_id as node_id
            //                    from VAB_AccountGroup ag left outer  join VAB_AccountSubGroup asg on (ag.VAB_AccountGroup_id=asg.VAB_AccountGroup_id)
            //                    left outer join VAB_AccountSubGroup asg1 on (asg1.VAB_AccountSubGroup_id = asg.VAB_AccountSubGroup_id)
            //                    where ag.vaf_client_id="+GetCtx().GetVAF_Client_ID());
            //            _NoOfRecord = DB.ExecuteQuery(_msql.ToString(), null, null);
            //            if (_NoOfRecord == 0)
            //            {
            //                log.Fine(_msql.ToString());
            //            }
            String sqlnew = "";
            // if (_SummaryLevel == "N" || _LedgerType=="A")
            if (_LedgerType == "N" || _LedgerType == "A")
            {
                sqlnew = "Select * from t_statement where issummary='N' and vaf_client_id=" + GetCtx().GetVAF_Client_ID();
                DataSet dsrecord = DB.ExecuteDataset(sqlnew, null, null);
                if (dsrecord.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsrecord.Tables[0].Rows.Count; i++)
                    {
                        VAGL_Budget_ID = 0;
                        _Parent_id = Util.GetValueOfInt(dsrecord.Tables[0].Rows[i]["PARENT_ID"]);
                        _Node_id = Util.GetValueOfInt(dsrecord.Tables[0].Rows[i]["NODE_ID"]);
                        sqlnew = "select VAGL_Budget_ID from Actual_Acct_Balance where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and account_id=" + _Node_id + " and VAGL_Budget_ID is not null";
                        VAGL_Budget_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlnew, null, null));
                        sqlnew = "select sum(amtacctdr) as Debit,sum(amtacctcr) as Credit from Actual_Acct_Balance where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and account_id=" + _Node_id + " and VAGL_Budget_ID is not null";
                        DataSet dsglbudget = DB.ExecuteDataset(sqlnew, null, null);
                        if (dsglbudget.Tables[0].Rows.Count > 0)
                        {
                            _Gl_Totalvalue = 0;
                            for (int a = 0; a < dsglbudget.Tables[0].Rows.Count; a++)
                            {
                                _Gl_budgetvalue = Decimal.Subtract(Util.GetValueOfDecimal(dsglbudget.Tables[0].Rows[a]["Debit"]), Util.GetValueOfDecimal(dsglbudget.Tables[0].Rows[a]["Credit"]));
                                _Gl_Totalvalue = _Gl_Totalvalue + _Gl_budgetvalue;
                            }
                        }
                        sqlnew = "Select TOTALVALUE from t_statement where  vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + _Node_id;
                        _totalBudgetValue = Util.GetValueOfDecimal(DB.ExecuteScalar(sqlnew, null, null));
                        if (_Gl_budgetvalue != 0)
                        {
                            _totalBudgetValue = Decimal.Round(Decimal.Multiply(Decimal.Divide(_totalBudgetValue, _Gl_budgetvalue), 100), 2);
                        }
                        else
                        {
                            _totalBudgetValue = 0;
                        }
                        _sqlUpdate = "Update t_statement set VAGL_Budget_ID=" + VAGL_Budget_ID + ",BUDGETVAL=" + _Gl_budgetvalue + ",VARINPERCENT=" + _totalBudgetValue + "  where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + _Node_id;
                        int count = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));

                        if (!_ListOfParentId.Contains(_Parent_id))
                        {

                            _ListOfParentId.Add(_Parent_id);
                            sqlcount = "select sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from t_statement where parent_id=" + _Parent_id + " and ISSUMMARY='N' and vaf_client_id=" + GetCtx().GetVAF_Client_ID();
                            DataSet dscountotal = DB.ExecuteDataset(sqlcount, null, null);
                            if (dscountotal.Tables[0].Rows.Count > 0)
                            {
                                _totalAmount = Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["totalamount"]);
                                _currentAmount = Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["currentamount"]);
                                _PreviousAmount = Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["previousamont"]);
                                _sqlUpdate = "Update t_statement set TOTALVALUE=" + _totalAmount + ",TPMONVALUE=" + _PreviousAmount + ",CRMONVAL=" + _currentAmount + " where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + _Parent_id;
                                count = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                if (count != -1)
                                {

                                }
                            }
                            else
                            {
                                dscountotal.Dispose();
                            }
                        }
                    }


                }
                else
                {
                    dsrecord.Dispose();
                }
            }


            #region accountelement
            if (_LedgerType == "S" || _LedgerType == "A")
            //if(_SummaryLevel=="Y" || _LedgerType=="A")
            {
                sqlnew = "Select * from  t_statement where vaf_client_id=" + GetCtx().GetVAF_Client_ID();
                DataSet dsrecord1 = DB.ExecuteDataset(sqlnew, null, null);
                Decimal BudgetValue = 0;
                //for (int l = 0; l < dsrecord1.Tables[0].Rows.Count; l++)
                //{
                sqlnew = "Select * from  t_statement where parent_ID = 0 ";
                DataSet ds1 = DB.ExecuteDataset(sqlnew, null, null);
                List<int> roots1 = new List<int>();
                if (ds1 != null)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                        {
                            sqlnew = "Select * from  t_statement where parent_id = " + ds1.Tables[0].Rows[k]["Node_ID"];
                            DataSet ds2 = DB.ExecuteDataset(sqlnew, null, null);
                            List<int> roots2 = new List<int>();
                            if (ds2 != null)
                            {
                                if (ds2.Tables[0].Rows.Count > 0)
                                {
                                    for (int m = 0; m < ds2.Tables[0].Rows.Count; m++)
                                    {
                                        sqlnew = "Select * from  t_statement where parent_id = " + ds2.Tables[0].Rows[m]["Node_ID"];
                                        DataSet ds3 = DB.ExecuteDataset(sqlnew, null, null);
                                        List<int> roots3 = new List<int>();
                                        if (ds3 != null)
                                        {
                                            if (ds3.Tables[0].Rows.Count > 0)
                                            {
                                                for (int n = 0; n < ds3.Tables[0].Rows.Count; n++)
                                                {

                                                    sqlnew = "Select * from  t_statement where parent_id = " + ds3.Tables[0].Rows[n]["Node_ID"];
                                                    DataSet ds4 = DB.ExecuteDataset(sqlnew, null, null);
                                                    if (ds4 != null)
                                                    {
                                                        if (ds4.Tables[0].Rows.Count > 0)
                                                        {
                                                            for (int l = 0; l < ds4.Tables[0].Rows.Count; l++)
                                                            {

                                                                sqlnew = "Select * from  t_statement where parent_id = " + ds4.Tables[0].Rows[l]["Node_ID"];
                                                                DataSet ds5 = DB.ExecuteDataset(sqlnew, null, null);
                                                                if (ds5 != null)
                                                                {
                                                                    if (ds5.Tables[0].Rows.Count > 0)
                                                                    {
                                                                        for (int p = 0; p < ds5.Tables[0].Rows.Count; p++)
                                                                        {

                                                                            sqlnew = "Select * from  t_statement where parent_id = " + ds5.Tables[0].Rows[p]["Node_ID"];
                                                                            DataSet ds6 = DB.ExecuteDataset(sqlnew, null, null);
                                                                            if (ds6 != null)
                                                                            {
                                                                                if (ds6.Tables[0].Rows.Count > 0)
                                                                                {
                                                                                    for (int q = 0; q < ds6.Tables[0].Rows.Count; q++)
                                                                                    {
                                                                                        sqlcount = "select sum( budgetval) as BudgetValue,sum( TOTALVALUE)as totalamount,sum( CRMONVAL) as currentamount,sum( TPMONVALUE) as previousamont from  t_statement where parent_id=" + ds6.Tables[0].Rows[q]["Parent_ID"];
                                                                                        DataSet dscountotal6 = DB.ExecuteDataset(sqlcount, null, null);

                                                                                        if (Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["totalamount"]) != 0)
                                                                                        {
                                                                                            if (Util.GetValueOfDecimal(dscountotal6.Tables[0].Rows[0]["BudgetValue"]) == 0)
                                                                                            {
                                                                                                BudgetValue = 0;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                BudgetValue = Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["BudgetValue"]);
                                                                                            }
                                                                                            _sqlUpdate = "Update  t_statement set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["totalamount"]) + ", TPMONVALUE=" + Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["previousamont"]) + ", CRMONVAL=" + Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["currentamount"]) + " where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + ds6.Tables[0].Rows[q]["Parent_ID"];
                                                                                            int count5 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                                                        }
                                                                                    }

                                                                                }
                                                                            }
                                                                            sqlcount = "select sum( budgetval) as BudgetValue,sum( TOTALVALUE)as totalamount,sum( CRMONVAL) as currentamount,sum( TPMONVALUE) as previousamont from  t_statement where parent_id=" + ds5.Tables[0].Rows[p]["Parent_ID"];
                                                                            DataSet dscountotal5 = DB.ExecuteDataset(sqlcount, null, null);

                                                                            if (Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["totalamount"]) != 0)
                                                                            {
                                                                                if (Util.GetValueOfDecimal(dscountotal5.Tables[0].Rows[0]["BudgetValue"]) == 0)
                                                                                {
                                                                                    BudgetValue = 0;
                                                                                }
                                                                                else
                                                                                {
                                                                                    BudgetValue = Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["BudgetValue"]);
                                                                                }
                                                                                _sqlUpdate = "Update  t_statement set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["totalamount"]) + ", TPMONVALUE=" + Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["previousamont"]) + ", CRMONVAL=" + Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["currentamount"]) + " where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + ds5.Tables[0].Rows[p]["Parent_ID"];
                                                                                int count5 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                                            }

                                                                        }
                                                                    }
                                                                }
                                                                sqlcount = "select sum( budgetval) as BudgetValue,sum( TOTALVALUE)as totalamount,sum( CRMONVAL) as currentamount,sum( TPMONVALUE) as previousamont from  t_statement where parent_id=" + ds4.Tables[0].Rows[l]["Parent_ID"];
                                                                DataSet dscountotal4 = DB.ExecuteDataset(sqlcount, null, null);

                                                                if (Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["totalamount"]) != 0)
                                                                {
                                                                    if (Util.GetValueOfDecimal(dscountotal4.Tables[0].Rows[0]["BudgetValue"]) == 0)
                                                                    {
                                                                        BudgetValue = 0;
                                                                    }
                                                                    else
                                                                    {
                                                                        BudgetValue = Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["BudgetValue"]);
                                                                    }
                                                                    _sqlUpdate = "Update  t_statement set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["currentamount"]) + " where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + ds4.Tables[0].Rows[l]["Parent_ID"];
                                                                    int count4 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                                }
                                                            }
                                                        }

                                                    }
                                                    roots3.Add(Util.GetValueOfInt(ds3.Tables[0].Rows[n]["Parent_ID"]));
                                                    sqlcount = "select sum(budgetval) as BudgetValue,sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from t_statement where parent_id=" + ds3.Tables[0].Rows[n]["Parent_ID"];
                                                    DataSet dscountotal3 = DB.ExecuteDataset(sqlcount, null, null);
                                                    if (Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["totalamount"]) != 0)
                                                    {

                                                        if (Util.GetValueOfDecimal(dscountotal3.Tables[0].Rows[0]["BudgetValue"]) == 0)
                                                        {
                                                            BudgetValue = 0;
                                                        }
                                                        else
                                                        {
                                                            BudgetValue = Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["BudgetValue"]);
                                                        }
                                                        _sqlUpdate = "Update t_statement set budgetval=" + BudgetValue + ",TOTALVALUE=" + Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["currentamount"]) + " where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + ds3.Tables[0].Rows[n]["Parent_ID"];
                                                        int count3 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                    }
                                                }
                                            }
                                        }
                                        sqlcount = "select sum(budgetval) as BudgetValue,sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from t_statement where parent_id=" + ds2.Tables[0].Rows[m]["Parent_ID"];
                                        DataSet dscountotal2 = DB.ExecuteDataset(sqlcount, null, null);
                                        if (Convert.ToDecimal(dscountotal2.Tables[0].Rows[0]["totalamount"]) != 0)
                                        {
                                            if (Util.GetValueOfDecimal(dscountotal2.Tables[0].Rows[0]["BudgetValue"]) == 0)
                                            {
                                                BudgetValue = 0;
                                            }
                                            else
                                            {
                                                BudgetValue = Convert.ToDecimal(dscountotal2.Tables[0].Rows[0]["BudgetValue"]);
                                            }
                                            _sqlUpdate = "Update t_statement set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal2.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Convert.ToDecimal(dscountotal2.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal2.Tables[0].Rows[0]["currentamount"]) + " where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + ds2.Tables[0].Rows[m]["Parent_ID"];
                                            int count2 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                        }
                                        //}
                                    }
                                }
                            }
                            sqlcount = "select sum(budgetval) as BudgetValue,sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from t_statement where parent_id=" + ds1.Tables[0].Rows[k]["Parent_ID"];
                            DataSet dscountotal = DB.ExecuteDataset(sqlcount, null, null);
                            if (Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["totalamount"]) != 0)
                            {
                                if (Util.GetValueOfDecimal(dscountotal.Tables[0].Rows[0]["BudgetValue"]) == 0)
                                {
                                    BudgetValue = 0;
                                }
                                else
                                {
                                    BudgetValue = Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["BudgetValue"]);
                                }
                                _sqlUpdate = "Update t_statement set  budgetval=" + BudgetValue + ",TOTALVALUE=" + Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["currentamount"]) + " where vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " and node_id=" + ds1.Tables[0].Rows[k]["Parent_ID"];
                                int count = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                            }
                            //}
                        }
                    }
                }
            }
            #endregion

            _sqlUpdate = "Update t_statement set LedgerType='" + _LedgerType + "'";
            int countledger = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
            if (countledger == -1)
            {
            }

            //}


            return "Completed";
        }
        private void Createsql(bool totalQty)
        {

            //            m_parameterWhere.Append(@"SELECT distinct t1.parent_id,t1.vaf_client_id,t1.vaf_org_id,t1.value AS value,t1.parent_id ,t1.node_id,t1.name as name,t2.Currentamount    AS Currentamount,(nvl(t1.Total,0)-nvl(t2.Currentamount,0)) AS Previous,
            //                   (nvl(t2.Currentamount,0)+((nvl(t1.Total,0)-nvl(t2.Currentamount,0)))) AS totalamount from ");

            //            m_parameterWhere.Append(@"(Select distinct  a.parent_id,max(c.issummary), a.node_id,f.vaf_client_id,f.vaf_org_id,c.value, c.name, CASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (SUM(f.amtacctdr)-SUM(f.amtacctcr)) ELSE (SUM(f.amtacctcr)-SUM(f.amtacctdr)) END AS Total  FROM VAF_TreeInfoChild a INNER JOIN VAB_Acct_Element c
            //                       ON c.VAB_Acct_Element_id = a.node_id  LEFT OUTER JOIN Actual_Acct_Balance f ON (f.account_id =c.VAB_Acct_Element_id ");
            //            SetDateAcct();
            //            sql.Append(" and f.DATEACCT BETWEEN " + _DateAcct_Yearly + " AND " + _DateAcct_To + " ");
            if (_VAGL_Budget_ID != 0)
            {
                m_parameterWhere.Append(" and VAGL_Budget_ID=" + _VAGL_Budget_ID + "");
            }
            if (_VAB_BusinessPartner_ID != 0)
            {
                m_parameterWhere.Append(" and VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID + "");
            }
            if (_VAM_Product_ID != 0)
            {
                m_parameterWhere.Append(" and VAM_Product_ID=" + _VAM_Product_ID + "");
            }
            if (_VAB_Project_ID != 0)
            {
                m_parameterWhere.Append(" and VAB_Project_ID=" + _VAB_Project_ID + "");
            }
            if (_VAB_BillingCode_ID != 0)
            {
                m_parameterWhere.Append(" and VAB_BillingCode_ID=" + _VAB_BillingCode_ID + "");
            }
            if (_VAB_Promotion_ID != 0)
            {
                m_parameterWhere.Append(" and VAB_Promotion_ID=" + _VAB_Promotion_ID + "");
            }
            if (_VAB_SalesRegionState_ID != 0)
            {
                m_parameterWhere.Append(" and VAB_SalesRegionState_ID=" + _VAB_SalesRegionState_ID + "");
            }
            if (_C_LocFrom_ID != 0)
            {
                m_parameterWhere.Append(" and C_LocFrom_ID=" + _C_LocFrom_ID + "");
            }
            if (_C_LocTo_ID != 0)
            {
                m_parameterWhere.Append(" and C_LocTo_ID=" + _C_LocTo_ID + "");
            }
            if (_C_LocFrom_ID != 0)
            {
                m_parameterWhere.Append(" and C_LocFrom_ID=" + _C_LocFrom_ID + "");
            }
            if (_User2_ID != 0)
            {
                m_parameterWhere.Append(" and User2_ID=" + _User2_ID + "");
            }
            if (_User1_ID != 0)
            {
                m_parameterWhere.Append(" and User1_ID=" + _User1_ID + "");
            }
            if (_UserElement1_ID != 0)
            {
                m_parameterWhere.Append(" and User1_ID=" + _UserElement1_ID + "");
            }
            if (_UserElement2_ID != 0)
            {
                m_parameterWhere.Append(" and UserElement2_ID=" + _UserElement2_ID + "");
            }
            if (_VAF_OrgTrx_ID != 0)
            {
                m_parameterWhere.Append(" and VAF_OrgTrx_ID=" + _VAF_OrgTrx_ID + "");
            }
            if (_AccountType != "")
            {
                m_parameterWhere.Append(" and AccountType='" + _AccountType + "'");
            }
            if (_LedgerType == "A" || _LedgerType == "")
            {
                _LedgerType = "A";
                m_parameterWhere.Append(" ) WHERE c.vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " ");
            }
            else
            {
                if (_LedgerType == "N")
                {
                    _SummaryLevel = "N";
                }
                if (_LedgerType == "S")
                {
                    _SummaryLevel = "Y";
                }
                if (IsIntermediate == "Y")
                {
                    m_parameterWhere.Append(" ) WHERE  c.issummary='" + _SummaryLevel + "' and c.vaf_client_id=" + GetCtx().GetVAF_Client_ID() + "  ");
                }
                if (IsIntermediate == "N")
                {
                    m_parameterWhere.Append(" ) WHERE  c.issummary='" + _SummaryLevel + "' and c.vaf_client_id=" + GetCtx().GetVAF_Client_ID() + " AND c.isintermediatecode='N' ");
                }
            }
            GroupBy(totalQty);
            // sql.Append(" t1 Inner join");

        }
        private void GroupBy(bool totalQty)
        {
            if (totalQty)
            {
                m_parameterWhere.Append(@" GROUP BY a.parent_id, c.AccountType,a.node_id,c.value,  c.name,c.AccountType,c.vaf_client_id,c.vaf_org_id,c.AccountType,f.VAM_Product_ID,f.VAB_BUSINESSPARTNER_ID,f.VAB_PROMOTION_ID,f.VAB_PROJECT_ID,f.VAB_BILLINGCODE_ID,f.VAB_SALESREGIONSTATE_ID,
                               f.C_LOCFROM_ID,f.C_LOCTO_ID,f.USER1_ID,f.USER2_ID,f.USERELEMENT1_ID,f.USERELEMENT2_ID,c.isintermediatecode)abc where abc.total!=0)");
            }
            else
            {
                m_parameterWhere.Append(@" GROUP BY a.parent_id, c.AccountType,a.node_id,c.value,  c.name,c.AccountType,c.vaf_client_id,c.vaf_org_id,c.AccountType,f.VAM_Product_ID,f.VAB_BUSINESSPARTNER_ID,f.VAB_PROMOTION_ID,f.VAB_PROJECT_ID,f.VAB_BILLINGCODE_ID,f.VAB_SALESREGIONSTATE_ID,
                               f.C_LOCFROM_ID,f.C_LOCTO_ID,f.USER1_ID,f.USER2_ID,f.USERELEMENT1_ID,f.USERELEMENT2_ID,c.isintermediatecode)abc where abc.Currentamount!=0)");
            }
        }
        private void GroupBy1()
        {

        }

        private void CreateView()
        {
            string _sqlview = @"CREATE OR REPLACE FORCE VIEW STATEMENT_V  as SELECT vaf_client_id,    accounttype,    vaf_org_id,    VAGL_Budget_ID ,    VAM_Product_ID ,    VAB_BUSINESSPARTNER_ID ,    VAB_PROMOTION_ID ,    VAB_PROJECT_ID ,    VAB_BILLINGCODE_ID ,    VAB_SALESREGIONSTATE_ID ,    C_LOCFROM_ID ,    C_LOCTO_ID ,    USER1_ID ,    USER2_ID ,    USERELEMENT1_ID,    USERELEMENT2_ID ,    VAB_YEARPERIOD_ID,    ledgertype              AS LedgerType,    issummary               AS ShowSummaryLevel,    NVL(budgetval,0)   AS budgetval,    NVL(varinpercent,0)AS varinpercent,    LEDGERNAME         AS LEDGER,    NVL(crmonval,0)    AS crmonval,    NVL(tpmonvalue,0)  AS tpmonvalue,    NVL(totalvalue,0)  AS totalvalue,    ledgercode,    SUBSTR(ledgercode,1,1)  AS ledgercode1,    SUBSTR(ledgercode,2,2)  AS ledgercode2,    SUBSTR(ledgercode,4,2)  AS ledgercode3,    SUBSTR(ledgercode,6,2)  AS ledgercode4,    SUBSTR(ledgercode,8,2)  AS ledgercode5,    SUBSTR(ledgercode,10,2) AS ledgercode6,    SUBSTR(ledgercode,12,2) AS ledgercode7,    ledgercode              AS Statement_V_ID  FROM t_statement  order by ledgercode";
            DB.ExecuteQuery(_sqlview, null, null);
        }
        private void CreateViewAsTree()
        {
            string _sqlview = @"CREATE OR REPLACE FORCE VIEW STATEMENT_V AS  SELECT vaf_client_id,    accounttype,    vaf_org_id,    VAGL_Budget_ID ,    VAM_Product_ID ,    VAB_BUSINESSPARTNER_ID ,    VAB_PROMOTION_ID ,    VAB_PROJECT_ID ,    VAB_BILLINGCODE_ID ,    VAB_SALESREGIONSTATE_ID ,    C_LOCFROM_ID ,    C_LOCTO_ID ,   USER1_ID ,    USER2_ID ,    USERELEMENT1_ID,    USERELEMENT2_ID ,    VAB_YEARPERIOD_ID,    ledgertype AS LedgerType,    '.'    || LPAD (' ', LEVEL * 3)    || LEDGERNAME      AS LEDGER,    issummary               AS ShowSummaryLevel,    NVL(budgetval,0)   AS budgetval,    NVL(varinpercent,0)AS varinpercent,   NVL(crmonval,0)    AS crmonval,    NVL(tpmonvalue,0)  AS tpmonvalue,    NVL(totalvalue,0)  AS  totalvalue,     ledgercode,    SUBSTR( ledgercode,1,1)  AS  ledgercode1,    SUBSTR( ledgercode,2,2)  AS  ledgercode2,    SUBSTR( ledgercode,4,2)  AS  ledgercode3,    SUBSTR( ledgercode,6,2)  AS  ledgercode4,    SUBSTR( ledgercode,8,2)  AS  ledgercode5,    SUBSTR( ledgercode,10,2) AS  ledgercode6,   SUBSTR( ledgercode,12,2) AS  ledgercode7,     ledgercode              AS  Statement_V_ID  FROM  t_statement    START WITH parent_id =0    CONNECT BY parent_id = PRIOR node_id  ORDER SIBLINGS BY  ledgercode";
            DB.ExecuteQuery(_sqlview, null, null);
        }
        private void CreateBalanceLine()
        {

            if (_VAGL_Budget_ID != 0 )
            {
                sql.Append(" and e.VAGL_Budget_ID=" + _VAGL_Budget_ID + "");
            }
            if (_VAB_BusinessPartner_ID != 0) 
            {
                sql.Append(" and e.VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID + "");
            }
            if (_VAM_Product_ID != 0)
            {
                sql.Append(" and e.VAM_Product_ID=" + _VAM_Product_ID + "");
            }
            if (_VAB_Project_ID != 0)
            {
                sql.Append(" and e.VAB_Project_ID=" + _VAB_Project_ID + "");
            }
            if (_VAB_BillingCode_ID != 0)
            {
                sql.Append(" and e.VAB_BillingCode_ID=" + _VAB_BillingCode_ID + "");
            }
            if (_VAB_Promotion_ID != 0)
            {
                sql.Append(" and e.VAB_Promotion_ID=" + _VAB_Promotion_ID + "");
            }
            if (_VAB_SalesRegionState_ID != 0)
            {
                sql.Append(" and e.VAB_SalesRegionState_ID=" + _VAB_SalesRegionState_ID + "");
            }
            if (_C_LocFrom_ID != 0)
            {
                sql.Append(" and e.C_LocFrom_ID=" + _C_LocFrom_ID + "");
            }
            if (_C_LocTo_ID != 0)
            {
                sql.Append(" and e.C_LocTo_ID=" + _C_LocTo_ID + "");
            }
            if (_C_LocFrom_ID != 0)
            {
                sql.Append(" and e.C_LocFrom_ID=" + _C_LocFrom_ID + "");
            }
            if (_User2_ID != 0)
            {
                sql.Append(" and e.User2_ID=" + _User2_ID + "");
            }
            if (_User1_ID != 0)
            {
                sql.Append(" and e.User1_ID=" + _User1_ID + "");
            }
            if (_UserElement1_ID != 0)
            {
                sql.Append(" and e.UserElement1_ID=" + _UserElement1_ID + "");
            }
            if (_UserElement2_ID != 0)
            {
                sql.Append(" and e.UserElement2_ID=" + _UserElement2_ID + "");
            }
            if (_VAF_OrgTrx_ID != 0)
            {
                sql.Append(" and e.VAF_OrgTrx_ID=" + _VAF_OrgTrx_ID + "");
            }
            if (_AccountType != "")
            {
                sql.Append(" and e.AccountType=" + _AccountType + "");
            }
            //if (_VAGL_Budget_ID == 0 && _VAB_BusinessPartner_ID == 0 && _VAF_OrgTrx_ID == 0 && _UserElement2_ID == 0 && _UserElement1_ID == 0 && _User1_ID == 0 && _User2_ID == 0 &&
            //    _C_LocFrom_ID == 0 && _C_LocTo_ID == 0 && _VAB_SalesRegionState_ID == 0 && _VAB_BillingCode_ID == 0 && _VAB_Promotion_ID == 0 && _VAB_Project_ID == 0 && _VAB_BusinessPartner_ID == 0 &&
            //    _VAGL_Budget_ID == 0 && _VAM_Product_ID =0)
            //{

            //}

            // DateTime? balanceDay = _DateAcct_From; // TimeUtil.addDays(_DateAcct_From, -1);
            //sql.Append(",null,").Append(DB.TO_DATE(balanceDay, true)).Append(",");

        }
        private void WhereClause()
        {
            //    if (_VAGL_Budget_ID != 0 || _VAGL_Budget_ID != null)
            //    {
            //        sql.Append(" and VAGL_Budget_ID=" + _VAGL_Budget_ID + "");
            //    }
            if (_VAB_BusinessPartner_ID != 0 )
            {
                sql.Append(" and VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID + "");
            }
            if (_VAM_Product_ID != 0 )
            {
                sql.Append(" and VAM_Product_ID=" + _VAM_Product_ID + "");
            }
            if (_VAB_Project_ID != 0 )
            {
                sql.Append(" and VAB_Project_ID=" + _VAB_Project_ID + "");
            }
            if (_VAB_BillingCode_ID != 0)
            {
                sql.Append(" and VAB_BillingCode_ID=" + _VAB_BillingCode_ID + "");
            }
            if (_VAB_Promotion_ID != 0)
            {
                sql.Append(" and VAB_Promotion_ID=" + _VAB_Promotion_ID + "");
            }
            if (_VAB_SalesRegionState_ID != 0)
            {
                sql.Append(" and VAB_SalesRegionState_ID=" + _VAB_SalesRegionState_ID + "");
            }
            if (_C_LocFrom_ID != 0 )
            {
                sql.Append(" and C_LocFrom_ID=" + _C_LocFrom_ID + "");
            }
            if (_C_LocTo_ID != 0 )
            {
                sql.Append(" and C_LocTo_ID=" + _C_LocTo_ID + "");
            }
            if (_C_LocFrom_ID != 0)
            {
                sql.Append(" and C_LocFrom_ID=" + _C_LocFrom_ID + "");
            }
            if (_User2_ID != 0 )
            {
                sql.Append(" and User2_ID=" + _User2_ID + "");
            }
            if (_User1_ID != 0 )
            {
                sql.Append(" and User1_ID=" + _User1_ID + "");
            }
            if (_UserElement1_ID != 0 )
            {
                sql.Append(" and UserElement1_ID=" + _UserElement1_ID + "");
            }
            if (_UserElement2_ID != 0 )
            {
                sql.Append(" and UserElement2_ID=" + _UserElement2_ID + "");
            }
            if (_VAF_OrgTrx_ID != 0 )
            {
                sql.Append(" and VAF_OrgTrx_ID=" + _VAF_OrgTrx_ID + "");
            }
            if (_AccountType != "")
            {
                sql.Append(" and AccountType=" + _AccountType + "");
            }
            //if (_VAGL_Budget_ID == 0 && _VAB_BusinessPartner_ID == 0 && _VAF_OrgTrx_ID == 0 && _UserElement2_ID == 0 && _UserElement1_ID == 0 && _User1_ID == 0 && _User2_ID == 0 &&
            //    _C_LocFrom_ID == 0 && _C_LocTo_ID == 0 && _VAB_SalesRegionState_ID == 0 && _VAB_BillingCode_ID == 0 && _VAB_Promotion_ID == 0 && _VAB_Project_ID == 0 && _VAB_BusinessPartner_ID == 0 &&
            //    _VAGL_Budget_ID == 0 && _VAM_Product_ID =0)
            //{

            //}

            // DateTime? balanceDay = _DateAcct_From; // TimeUtil.addDays(_DateAcct_From, -1);
            //sql.Append(",null,").Append(DB.TO_DATE(balanceDay, true)).Append(",");

        }
        private void GetSumOfDebitCredit()
        {
            sql = new StringBuilder();
            sql.Append(@"");
        }
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
                else if (name.Equals("VAB_YearPeriod_ID"))
                {
                    _VAB_YearPeriod_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAGL_Budget_ID"))
                {
                    _VAGL_Budget_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAM_Product_ID"))
                {
                    _VAM_Product_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_Project_ID"))
                {
                    _VAB_Project_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_BillingCode_ID"))
                {
                    _VAB_BillingCode_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_SalesRegionState_ID"))
                {
                    _VAB_SalesRegionState_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_Promotion_ID"))
                {
                    _VAB_Promotion_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_LocFrom_ID"))
                {
                    _C_LocFrom_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_LocTo_ID"))
                {
                    _C_LocTo_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("User1_ID"))
                {
                    _User1_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("User2_ID"))
                {
                    _User2_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("UserElement1_ID"))
                {
                    _UserElement1_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("UserElement2_ID"))
                {
                    _UserElement2_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAF_OrgTrx_ID"))
                {
                    _VAF_OrgTrx_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("AccountType"))
                {
                    _AccountType = Util.GetValueOfString(para[i].GetParameter());
                }
                //else if (name.Equals("ShowSummaryLevel"))
                //{
                //    _SummaryLevel = Util.GetValueOfString(para[i].GetParameter());
                //}
                else if (name.Equals("LedgerType"))
                {
                    _LedgerType = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (name.Equals("IsIntermediateCode"))
                {
                    IsIntermediate = Util.GetValueOfString(para[i].GetParameter());
                }

            }
        }
        private void SetDateAcct()
        {
            if (_VAB_YearPeriod_ID == 0 || _VAB_YearPeriod_ID == -1)
            {
                _DateAcct_From = DateTime.Now.Date.AddDays(-(DateTime.Now.Day - 1));
                _DateAcct_To = DateTime.Now.Date.AddMonths(1);
                _DateAcct_To = _DateAcct_To.Value.AddDays(-(_DateAcct_To.Value.Day));
                // _DateAcct_To = _DateAcct_To.Value.AddDays(-(_DateAcct_To.Value.Day));
                _DateAcct_Yearly = new DateTime(DateTime.Now.Year, 1, 1);
                return;
            }
            else
            {
                _DateAcct_Yearly = new DateTime(DateTime.Now.Year, 1, 1);
                String sql = "SELECT StartDate, EndDate FROM VAB_YearPeriod WHERE VAB_YearPeriod_ID=@param1";
                SqlParameter[] param = new SqlParameter[1];
                IDataReader idr = null;
                try
                {

                    param[0] = new SqlParameter("@param1", _VAB_YearPeriod_ID);
                    idr = DB.ExecuteReader(sql, param, null);
                    if (idr.Read())
                    {
                        _DateAcct_From = Util.GetValueOfDateTime(idr[0]);// rs.getTimestamp(1);
                        _DateAcct_To = Util.GetValueOfDateTime(idr[1]);// rs.getTimestamp(2);
                        _DateAcct_Yearly = new DateTime(_DateAcct_From.Value.Year, 1, 1);
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
            }
        }
    }
}
