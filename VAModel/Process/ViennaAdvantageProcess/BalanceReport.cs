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
    public class BalanceReport : SvrProcess
    {
        #region Variables
        /** AcctSchame Parameter			*/
      //  private int _C_AcctSchema_ID = 0;
        /**	Period Parameter				*/
        private int _C_Period_ID = 0;
        private int _GL_Budget_ID = 0;
        private DateTime? _DateAcct_From = null;
        private DateTime? _DateAcct_To = null;
        private DateTime? _DateAcct_Yearly = null;
        /**	Org Parameter					*/
       // private int _AD_Org_ID = 0;
        /**	Account Parameter				*/
        private int _Account_ID = 0;
        int GL_budget_id = 0;
       // private String _AccountValue_From = null;
       // private String _AccountValue_To = null;
        /**	BPartner Parameter				*/
       // private int _C_BPartner_ID = 0;
        /**	Product Parameter				*/
      //  private int _M_Product_ID = 0;
        /**	Project Parameter				*/
        private int _C_Project_ID = 0;
        /**	Activity Parameter				*/
       // private int _C_Activity_ID = 0;
        /**	SalesRegion Parameter			*/
       // private int _C_SalesRegion_ID = 0;
        /**	Campaign Parameter				*/
      //  private int _C_Campaign_ID = 0;
       // private int AD_OrgTrx_ID = 0;

        /** Posting Type					*/
       // private String _PostingType = "A";
        /** Hierarchy						*/
       // private int _PA_Hierarchy_ID = 0;

       // private int _AD_OrgTrx_ID = 0;
        private string _AccountType = "";
        private string _SummaryLevel = "";
        private string _LedgerType = "";
      //  private int _C_LocFrom_ID = 0;
       // private int _C_LocTo_ID = 0;
       // private int _User1_ID = 0;
       // private int _User2_ID = 0;
       // private int _UserElement1_ID = 0;
       // private int _UserElement2_ID = 0;
        /**	Parameter Where Clause			*/
        private StringBuilder m_parameterWhere = new StringBuilder();
        private StringBuilder m_datesql = new StringBuilder();
        /**	Account							*/
      //  private MElementValue m_acct = null;
        private StringBuilder _msql = new StringBuilder();
        StringBuilder _m_OuterSql = new StringBuilder();
        StringBuilder sql = new StringBuilder();
        StringBuilder _m_CaseSql = new StringBuilder();
        List<int> _ListOfParentId = new List<int>();
        string _sqlUpdate = "";
     //   String sqlnew = "";
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
     //   int _NoOfRecord = 0;
        string IsIntermediate = "N";
        private static String _Insert = @"INSERT INTO  T_BALSHEET "
             + "( parent_id, AccountType,C_PROJECT_ID,node_id,AD_Client_ID, AD_Org_ID,"
             + "  LedgerCode, LedgerName, CrMonVal ,"
             + "  TPMonValue,  TotalValue,ISSUMMARY,PMCREDIT,PMDEBIT,CMCREDIT,CMDEBIT,TOTALDEBIT,TOTALCREDIT,DIFFDEBIT,DIFFCREDIT,isintermediatecode)";

       // private static String _InsertAccount = @"INSERT INTO  T_BALSHEET "
      //       + "( Ad_client_id, Ad_Org_id,ACCGRPNAME,C_ACCOUNTGROUP_ID,ACCGRPNAME,PARENTID,NODEID)";
        #endregion

        protected override string DoIt()
        {
            String sql1 = @"Select startdate from c_period where c_period_id=" + _C_Period_ID;
            DateTime? startdate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql1));
            sql1 = @"select clfn.c_acctschema1_id from ad_client cl join ad_clientinfo clfn  on (cl.ad_client_id=clfn.ad_client_id)
                     where cl.ad_client_id=" + GetAD_Client_ID();
            int c_acctschema_id = Util.GetValueOfInt(DB.ExecuteScalar(sql1));
            FinBalance.UpdateBalance(GetCtx(), c_acctschema_id, startdate, Get_Trx(), 0, this);
            if (_LedgerType == "N")
            {
                CreateView();
            }
            else
            {
                CreateViewAsTree();

            }
            string query = "Select count(*) from  T_BALSHEET";
            int no = Util.GetValueOfInt(DB.ExecuteScalar(query, null, null));
            if (no > 0)
            {
                query = "Delete from  T_BALSHEET";

                no = DB.ExecuteQuery(query, null, null);

            }
            // query = "Select count(*) from   t_accounts";
            //_NoOfRecord = Util.GetValueOfInt(DB.ExecuteScalar(query, null, null));
            //if (_NoOfRecord > 0)
            //{
            //    query = "Delete  from   T_BALSHEET";

            //    _NoOfRecord = DB.ExecuteQuery(query, null, null);

            //}
            no = 0;
            //  _NoOfRecord = 0;
            _msql.Append(_Insert);

            _m_OuterSql.Append(@"SELECT distinct t1.parent_id,t1.AccountType,t1.C_PROJECT_ID,
    t1.node_id,t1.ad_client_id,t1.ad_org_id,t1.value AS value,t1.name as name,nvl(max(t2.Currentamount),0) AS Currentamount,(nvl(max(t1.Total),0)-nvl(max(t2.Currentamount),0)) AS Previous,
                   (nvl(max(t2.Currentamount),0)+((nvl(max(t1.Total),0)-nvl(max(t2.Currentamount),0)))) AS totalamount,t1.IsSummary as IsSummary ,NVL(t1.PrMonCr,0)  AS PrevMonCr,  NVL(t1.PrMonDb,0) AS PrevMonDb,  NVL(t2.CrMonCr,0)  AS CrMonCr,  NVL(t2.CrMonDb,0) AS CrMonDb,  ( NVL(t1.PrMonDb,0) + NVL(t2.CrMonDb,0)) AS TotalDebit,  ( NVL(t1.PrMonCr,0) + NVL(t2.CrMonCr,0)) AS TotalCredit,  CASE    WHEN  (NVL(t1.PrMonDb,0) + NVL(t2.CrMonDb,0)) -  (NVL(t1.PrMonCr,0) + NVL(t2.CrMonCr,0)) >0    THEN (NVL(t1.PrMonDb,0) + NVL(t2.CrMonDb,0)) -  (NVL(t1.PrMonCr,0) + NVL(t2.CrMonCr,0))  END AS DiffDebit,  CASE WHEN   (NVL(t1.PrMonDb,0) + NVL(t2.CrMonDb,0)) -  (NVL(t1.PrMonCr,0) + NVL(t2.CrMonCr,0)) <0    THEN  ABS( ((NVL(t1.PrMonDb,0) + NVL(t2.CrMonDb,0)) -  (NVL(t1.PrMonCr,0) + NVL(t2.CrMonCr,0))))  END AS DiffCredit,t1.isintermediatecode from ");
            _m_CaseSql.Append(@"(SELECT * from (Select distinct    c.AccountType,f.C_PROJECT_ID,
    a.parent_id,max(c.issummary) as IsSummary,c.isintermediatecode, a.node_id,c.ad_client_id,c.ad_org_id,c.value, c.name,SUM(f.amtacctdr) AS PrMonDb,SUM(f.amtacctcr) AS PrMonCr, CASE WHEN c.AccountType='A'  THEN (NVL((SUM(f.amtacctdr)),0)-NVL((SUM(f.amtacctcr)),0)) ELSE (NVL((SUM(f.amtacctcr)),0)-NVL((SUM(f.amtacctdr)),0)) END AS Total");
            sql.Append(" FROM ad_treenode a INNER JOIN c_elementvalue c ON c.c_elementvalue_id = a.node_id  INNER JOIN fact_acct_balance f ON (f.account_id =c.c_elementvalue_id  and f.GL_budget_id is null");
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
            _m_CaseSql.Append(@"(SELECT * From (Select distinct  c.AccountType,f.C_PROJECT_ID,
               a.parent_id,max(c.issummary) AS IsSummary,c.isintermediatecode, a.node_id,c.ad_client_id,c.ad_org_id,c.value, c.name,SUM(f.amtacctdr) AS CrMonDb,SUM(f.amtacctcr) AS CrMonCr, CASE WHEN c.AccountType='A'  THEN (NVL((SUM(f.amtacctdr)),0)-NVL((SUM(f.amtacctcr)),0)) ELSE (NVL((SUM(f.amtacctcr)),0)-NVL((SUM(f.amtacctdr)),0)) END AS Currentamount");
            _msql.Append(_m_CaseSql);
            _msql.Append(sql);
            m_datesql = new StringBuilder();
            m_datesql.Append(" and f.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(_DateAcct_From, true) + " AND " + GlobalVariable.TO_DATE(_DateAcct_To, true) + " ");
            _msql.Append(m_datesql);
            m_parameterWhere = new StringBuilder();
            Createsql(false);
            _msql.Append(m_parameterWhere);
            _msql.Append(" t2  ON (t1.value=t2.value) where t1.Total>0 group by t1.parent_id, t1.AccountType,  t1.C_PROJECT_ID,   t1.node_id,  t1.ad_client_id,  t1.ad_org_id,  t1.value     ,  t1.name  ,  t1.IsSummary,t1.PrMonDb,  t1.PrMonCr,  t2.CrMonDb,  t2.CrMonCr,t1.isintermediatecode");
            //_msql.Append(" ");
            //_msql.Append(sql);
            //_msql.Append(sql);
            //            sql.Append(@" SELECT   t1.parent_id,t1.node_id,t1.ad_client_id,t1.ad_org_id,t1.Created,t1.CreatedBy,t1.Updated,t1.UpdatedBy,t1.value as value, t1.name name,t2.Currentamount as Currentamount,(t1.Total-t2.Currentamount) as Previous,(t2.Currentamount+((t1.Total-t2.Currentamount))) as totalamount,
            //               t1.C_BPartner_ID,t1.M_Product_ID,t1.C_Project_ID,t1.C_Activity_ID,
            //            t1.C_SalesRegion_ID,t1.C_Campaign_ID,t1.C_LocFrom_ID,t1.C_LocTo_ID  ,t1.User1_ID,t1.User2_ID,t1.UserElement1_ID,t1.UserElement2_ID,t1.ACCOUNTTYPE
            //            FROM(SELECT  g.gl_budget_id,a.parent_id,a.node_id,f.ad_client_id,f.ad_org_id,f.Created,f.CreatedBy,f.Updated,f.UpdatedBy,c.value AS value,f.C_BPartner_ID,f.M_Product_ID,f.C_Project_ID,f.C_Activity_ID,c.ACCOUNTTYPE,
            //      f.C_SalesRegion_ID,f.C_Campaign_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID, c.name,CASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (SUM(f.amtacctdr)-SUM(f.amtacctcr))
            //          Else (SUM(f.amtacctcr)-SUM(f.amtacctdr))END AS Total FROM ad_treenode a INNER JOIN c_elementvalue c ON c.c_elementvalue_id = a.node_id LEFT OUTER JOIN fact_acct_balance f
            //        ON(f.account_id     =c.c_elementvalue_id)  inner join gl_budget g on (f.gl_budget_id=g.gl_budget_id)  WHERE c.ad_client_id=" + GetCtx().GetAD_Client_ID() + "");
            //              SetDateAcct();
            //              sql.Append("  AND f.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(_DateAcct_Yearly, true) + " AND " + GlobalVariable.TO_DATE(_DateAcct_To, true) + "");
            //            //AND f.DATEACCT BETWEEN '01-Jan-13' AND '31-Dec-13'
            //              sql.Append(@"GROUP BY a.parent_id,a.node_id,c.value, c.name,c.AccountType, f.ad_client_id,f.ad_org_id,f.Created,f.CreatedBy,f.Updated,f.UpdatedBy,f.C_BPartner_ID,f.M_Product_ID,f.C_Project_ID,f.C_Activity_ID,
            //            f.C_SalesRegion_ID,f.C_Campaign_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID,g.gl_budget_id) t1 inner JOIN (SELECT g.gl_budget_id,a.parent_id,a.node_id,c.value AS value,c.name, cASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (SUM(f.amtacctdr)-SUM(f.amtacctcr))
            //          else (SUM(f.amtacctcr)-SUM(f.amtacctdr)) END AS Currentamount,f.C_BPartner_ID,f.M_Product_ID,f.C_Project_ID,f.C_Activity_ID,c.ACCOUNTTYPE,
            //      f.C_SalesRegion_ID,f.C_Campaign_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID FROM ad_treenode a INNER JOIN c_elementvalue c ON c.c_elementvalue_id = a.node_id LEFT OUTER JOIN fact_acct_balance f
            //       ON(f.account_id     =c.c_elementvalue_id)  inner join gl_budget g on (f.gl_budget_id=g.gl_budget_id) WHERE f.ad_client_id=" + GetCtx().GetAD_Client_ID() + "  AND f.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(_DateAcct_From, true) + " and " + GlobalVariable.TO_DATE(_DateAcct_To, true) + @"
            //          GROUP BY c.value,c.name,c.AccountType,f.C_BPartner_ID,f.M_Product_ID,f.C_Project_ID,f.C_Activity_ID,c.ACCOUNTTYPE, f.ad_client_id,f.ad_org_id,f.Created,f.CreatedBy,f.Updated,f.UpdatedBy,
            //      f.C_SalesRegion_ID,f.C_Campaign_ID,f.C_LocFrom_ID,f.C_LocTo_ID  ,f.User1_ID,f.User2_ID,f.UserElement1_ID,f.UserElement2_ID,a.parent_id,a.node_id,g.gl_budget_id) t2 ON (t1.value=t2.value) ");
            no = DB.ExecuteQuery(_msql.ToString(), null, null);
            if (no == 0)
            {
                log.Fine(_msql.ToString());
            }
            if (_C_Period_ID != 0 )
            {
                string Sql_Period = "UPDATE t_balsheet set c_period_id =" + _C_Period_ID + "";
                DB.ExecuteQuery(Sql_Period);
            }
            log.Fine("#" + no + " (Account_ID=" + _Account_ID + ")");
            //            _sqlAccount.Append(_InsertAccount);
            //            _sqlAccount.Append(@"SELECT asg.ad_client_id as ad_client_id,asg.ad_org_id as ad_org_id,ag.name as accountGroupname,asg.c_accountgroup_id as c_accountgroup_id, asg.name AS AccountsubGroupName,
            //                    nvl(asg.c_accountsubgroup_id_1,0)as parent_id,asg.c_accountsubgroup_id as node_id
            //                    from c_accountgroup ag left outer  join c_accountsubgroup asg on (ag.c_accountgroup_id=asg.c_accountgroup_id)
            //                    left outer join c_accountsubgroup asg1 on (asg1.c_accountsubgroup_id = asg.c_accountsubgroup_id)
            //                    where ag.ad_client_id="+GetCtx().GetAD_Client_ID());
            //            _NoOfRecord = DB.ExecuteQuery(_msql.ToString(), null, null);
            //            if (_NoOfRecord == 0)
            //            {
            //                log.Fine(_msql.ToString());
            //            }
            String sqlnew = "";
            // if (_SummaryLevel == "N" || _LedgerType=="A")
            if (_LedgerType == "N" || _LedgerType == "A")
            {
                sqlnew = "Select * from T_BALSHEET where issummary='N' and ad_client_id=" + GetCtx().GetAD_Client_ID();
                DataSet dsrecord = DB.ExecuteDataset(sqlnew, null, null);
                if (dsrecord.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsrecord.Tables[0].Rows.Count; i++)
                    {
                        GL_budget_id = 0;
                        _Parent_id = Util.GetValueOfInt(dsrecord.Tables[0].Rows[i]["PARENT_ID"]);
                        _Node_id = Util.GetValueOfInt(dsrecord.Tables[0].Rows[i]["NODE_ID"]);
                        sqlnew = "select GL_budget_id from fact_acct_balance where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and account_id=" + _Node_id + " and GL_budget_id is not null";
                        GL_budget_id = Util.GetValueOfInt(DB.ExecuteScalar(sqlnew, null, null));
                        sqlnew = "select sum(amtacctdr) as Debit,sum(amtacctcr) as Credit from fact_acct_balance where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and account_id=" + _Node_id + " and GL_budget_id is not null";
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
                        sqlnew = "Select TOTALVALUE from T_BALSHEET where  ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + _Node_id;
                        _totalBudgetValue = Util.GetValueOfDecimal(DB.ExecuteScalar(sqlnew, null, null));
                        if (_Gl_budgetvalue != 0)
                        {
                            _totalBudgetValue = Decimal.Round(Decimal.Multiply(Decimal.Divide(_totalBudgetValue, _Gl_budgetvalue), 100), 2);
                        }
                        else
                        {
                            _totalBudgetValue = 0;
                        }
                        _sqlUpdate = "Update T_BALSHEET set GL_budget_id=" + GL_budget_id + ",BUDGETVAL=" + _Gl_budgetvalue + ",VARINPERCENT=" + _totalBudgetValue + "  where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + _Node_id;
                        int count = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));

                        if (!_ListOfParentId.Contains(_Parent_id))
                        {

                            _ListOfParentId.Add(_Parent_id);
                            sqlcount = "select sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from T_BALSHEET where parent_id=" + _Parent_id + " and ISSUMMARY='N' and ad_client_id=" + GetCtx().GetAD_Client_ID();
                            DataSet dscountotal = DB.ExecuteDataset(sqlcount, null, null);
                            if (dscountotal.Tables[0].Rows.Count > 0)
                            {
                                _totalAmount = Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["totalamount"]);
                                _currentAmount = Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["currentamount"]);
                                _PreviousAmount = Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["previousamont"]);
                                _sqlUpdate = "Update T_BALSHEET set TOTALVALUE=" + _totalAmount + ",TPMONVALUE=" + _PreviousAmount + ",CRMONVAL=" + _currentAmount + " where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + _Parent_id;
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
                Decimal BudgetValue = 0;
                sqlnew = "Select * from  T_BALSHEET where ad_client_id=" + GetCtx().GetAD_Client_ID();
                DataSet dsrecord1 = DB.ExecuteDataset(sqlnew, null, null);

                //for (int l = 0; l < dsrecord1.Tables[0].Rows.Count; l++)
                //{
                sqlnew = "Select * from  T_BALSHEET where parent_ID = 0 ";
                DataSet ds1 = DB.ExecuteDataset(sqlnew, null, null);
                List<int> roots1 = new List<int>();
                if (ds1 != null)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                        {
                            sqlnew = "Select * from  T_BALSHEET where parent_id = " + ds1.Tables[0].Rows[k]["Node_ID"];
                            DataSet ds2 = DB.ExecuteDataset(sqlnew, null, null);
                            List<int> roots2 = new List<int>();
                            if (ds2 != null)
                            {
                                if (ds2.Tables[0].Rows.Count > 0)
                                {
                                    for (int m = 0; m < ds2.Tables[0].Rows.Count; m++)
                                    {
                                        sqlnew = "Select * from  T_BALSHEET where parent_id = " + ds2.Tables[0].Rows[m]["Node_ID"];
                                        DataSet ds3 = DB.ExecuteDataset(sqlnew, null, null);
                                        List<int> roots3 = new List<int>();
                                        if (ds3 != null)
                                        {
                                            if (ds3.Tables[0].Rows.Count > 0)
                                            {
                                                for (int n = 0; n < ds3.Tables[0].Rows.Count; n++)
                                                {

                                                    sqlnew = "Select * from  T_BALSHEET where parent_id = " + ds3.Tables[0].Rows[n]["Node_ID"];
                                                    DataSet ds4 = DB.ExecuteDataset(sqlnew, null, null);
                                                    if (ds4 != null)
                                                    {
                                                        if (ds4.Tables[0].Rows.Count > 0)
                                                        {
                                                            for (int l = 0; l < ds4.Tables[0].Rows.Count; l++)
                                                            {

                                                                sqlnew = "Select * from  T_BALSHEET where parent_id = " + ds4.Tables[0].Rows[l]["Node_ID"];
                                                                DataSet ds5 = DB.ExecuteDataset(sqlnew, null, null);
                                                                if (ds5 != null)
                                                                {
                                                                    if (ds5.Tables[0].Rows.Count > 0)
                                                                    {
                                                                        for (int p = 0; p < ds5.Tables[0].Rows.Count; p++)
                                                                        {

                                                                            sqlnew = "Select * from  T_BALSHEET where parent_id = " + ds5.Tables[0].Rows[p]["Node_ID"];
                                                                            DataSet ds6 = DB.ExecuteDataset(sqlnew, null, null);
                                                                            if (ds6 != null)
                                                                            {
                                                                                if (ds6.Tables[0].Rows.Count > 0)
                                                                                {
                                                                                    for (int q = 0; q < ds6.Tables[0].Rows.Count; q++)
                                                                                    {
                                                                                        sqlcount = "select sum( budgetval) as BudgetValue,sum( TOTALVALUE)as totalamount,sum( CRMONVAL) as currentamount,sum( TPMONVALUE) as previousamont from  T_BALSHEET where parent_id=" + ds6.Tables[0].Rows[q]["Parent_ID"];
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
                                                                                            _sqlUpdate = "Update  T_BALSHEET set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["totalamount"]) + ", TPMONVALUE=" + Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["previousamont"]) + ", CRMONVAL=" + Convert.ToDecimal(dscountotal6.Tables[0].Rows[0]["currentamount"]) + " where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + ds6.Tables[0].Rows[q]["Parent_ID"];
                                                                                            int count5 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                                                        }
                                                                                    }

                                                                                }
                                                                            }
                                                                            sqlcount = "select sum( budgetval) as BudgetValue,sum( TOTALVALUE)as totalamount,sum( CRMONVAL) as currentamount,sum( TPMONVALUE) as previousamont from  T_BALSHEET where parent_id=" + ds5.Tables[0].Rows[p]["Parent_ID"];
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
                                                                                _sqlUpdate = "Update  T_BALSHEET set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["totalamount"]) + ", TPMONVALUE=" + Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["previousamont"]) + ", CRMONVAL=" + Convert.ToDecimal(dscountotal5.Tables[0].Rows[0]["currentamount"]) + " where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + ds5.Tables[0].Rows[p]["Parent_ID"];
                                                                                int count5 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                                            }

                                                                        }
                                                                    }
                                                                }
                                                                sqlcount = "select sum( budgetval) as BudgetValue,sum( TOTALVALUE)as totalamount,sum( CRMONVAL) as currentamount,sum( TPMONVALUE) as previousamont from  T_BALSHEET where parent_id=" + ds4.Tables[0].Rows[l]["Parent_ID"];
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
                                                                    _sqlUpdate = "Update  T_BALSHEET set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal4.Tables[0].Rows[0]["currentamount"]) + " where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + ds4.Tables[0].Rows[l]["Parent_ID"];
                                                                    int count4 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                                }
                                                            }
                                                        }

                                                    }
                                                    roots3.Add(Util.GetValueOfInt(ds3.Tables[0].Rows[n]["Parent_ID"]));
                                                    sqlcount = "select sum(budgetval) as BudgetValue,sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from T_BALSHEET where parent_id=" + ds3.Tables[0].Rows[n]["Parent_ID"];
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
                                                        _sqlUpdate = "Update T_BALSHEET set budgetval=" + BudgetValue + ",TOTALVALUE=" + Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal3.Tables[0].Rows[0]["currentamount"]) + " where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + ds3.Tables[0].Rows[n]["Parent_ID"];
                                                        int count3 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                                    }
                                                }
                                            }
                                        }
                                        sqlcount = "select sum(budgetval) as BudgetValue,sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from T_BALSHEET where parent_id=" + ds2.Tables[0].Rows[m]["Parent_ID"];
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
                                            _sqlUpdate = "Update T_BALSHEET set  budgetval=" + BudgetValue + ", TOTALVALUE=" + Convert.ToDecimal(dscountotal2.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Util.GetValueOfInt(dscountotal2.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal2.Tables[0].Rows[0]["currentamount"]) + " where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + ds2.Tables[0].Rows[m]["Parent_ID"];
                                            int count2 = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                                        }
                                        //}
                                    }
                                }
                            }
                            sqlcount = "select sum(budgetval) as BudgetValue,sum(TOTALVALUE)as totalamount,sum(CRMONVAL) as currentamount,sum(TPMONVALUE) as previousamont from T_BALSHEET where parent_id=" + ds1.Tables[0].Rows[k]["Parent_ID"];
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
                                _sqlUpdate = "Update T_BALSHEET set  budgetval=" + BudgetValue + ",TOTALVALUE=" + Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["totalamount"]) + ",TPMONVALUE=" + Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["previousamont"]) + ",CRMONVAL=" + Convert.ToDecimal(dscountotal.Tables[0].Rows[0]["currentamount"]) + " where ad_client_id=" + GetCtx().GetAD_Client_ID() + " and node_id=" + ds1.Tables[0].Rows[k]["Parent_ID"];
                                int count = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
                            }
                            //}
                        }
                    }
                }
            }
            #endregion

            _sqlUpdate = "Update T_BALSHEET set LedgerType='" + _LedgerType + "'";
            int countledger = Util.GetValueOfInt(DB.ExecuteQuery(_sqlUpdate, null, null));
            if (countledger == -1)
            {
            }

            //}


            return "Completed";
        }
        private void Createsql(bool totalQty)
        {

            //            m_parameterWhere.Append(@"SELECT distinct t1.parent_id,t1.ad_client_id,t1.ad_org_id,t1.value AS value,t1.parent_id ,t1.node_id,t1.name as name,t2.Currentamount    AS Currentamount,(nvl(t1.Total,0)-nvl(t2.Currentamount,0)) AS Previous,
            //                   (nvl(t2.Currentamount,0)+((nvl(t1.Total,0)-nvl(t2.Currentamount,0)))) AS totalamount from ");

            //            m_parameterWhere.Append(@"(Select distinct  a.parent_id,max(c.issummary), a.node_id,f.ad_client_id,f.ad_org_id,c.value, c.name, CASE WHEN c.AccountType='A' OR c.AccountType  ='E' THEN (SUM(f.amtacctdr)-SUM(f.amtacctcr)) ELSE (SUM(f.amtacctcr)-SUM(f.amtacctdr)) END AS Total  FROM ad_treenode a INNER JOIN c_elementvalue c
            //                       ON c.c_elementvalue_id = a.node_id  LEFT OUTER JOIN fact_acct_balance f ON (f.account_id =c.c_elementvalue_id ");
            //            SetDateAcct();
            //            sql.Append(" and f.DATEACCT BETWEEN " + _DateAcct_Yearly + " AND " + _DateAcct_To + " ");
            if (_GL_Budget_ID != 0)
            {
                m_parameterWhere.Append(" and gl_budget_id=" + _GL_Budget_ID + "");
            }
            //if (_C_BPartner_ID != 0)
            //{
            //    m_parameterWhere.Append(" and C_BPartner_ID=" + _C_BPartner_ID + "");
            //}
            //if (_M_Product_ID != 0)
            //{
            //    m_parameterWhere.Append(" and M_Product_ID=" + _M_Product_ID + "");
            //}
            if (_C_Project_ID != 0)
            {
                m_parameterWhere.Append(" and C_Project_ID=" + _C_Project_ID + "");
            }
            //if (_C_Activity_ID != 0)
            //{
            //    m_parameterWhere.Append(" and C_Activity_ID=" + _C_Activity_ID + "");
            //}
            //if (_C_Campaign_ID != 0)
            //{
            //    m_parameterWhere.Append(" and C_Campaign_ID=" + _C_Campaign_ID + "");
            //}
            //if (_C_SalesRegion_ID != 0)
            //{
            //    m_parameterWhere.Append(" and C_SalesRegion_ID=" + _C_SalesRegion_ID + "");
            //}
            //if (_C_LocFrom_ID != 0)
            //{
            //    m_parameterWhere.Append(" and C_LocFrom_ID=" + _C_LocFrom_ID + "");
            //}
            //if (_C_LocTo_ID != 0)
            //{
            //    m_parameterWhere.Append(" and C_LocTo_ID=" + _C_LocTo_ID + "");
            //}
            //if (_C_LocFrom_ID != 0)
            //{
            //    m_parameterWhere.Append(" and C_LocFrom_ID=" + _C_LocFrom_ID + "");
            //}
            //if (_User2_ID != 0)
            //{
            //    m_parameterWhere.Append(" and User2_ID=" + _User2_ID + "");
            //}
            //if (_User1_ID != 0)
            //{
            //    m_parameterWhere.Append(" and User1_ID=" + _User1_ID + "");
            //}
            //if (_UserElement1_ID != 0)
            //{
            //    m_parameterWhere.Append(" and User1_ID=" + _UserElement1_ID + "");
            //}
            //if (_UserElement2_ID != 0)
            //{
            //    m_parameterWhere.Append(" and UserElement2_ID=" + _UserElement2_ID + "");
            //}
            //if (_AD_OrgTrx_ID != 0)
            //{
            //    m_parameterWhere.Append(" and AD_OrgTrx_ID=" + _AD_OrgTrx_ID + "");
            //}
            if (_AccountType != "")
            {
                m_parameterWhere.Append(" and AccountType='" + _AccountType + "'");
            }
            if (_LedgerType == "A" || _LedgerType == "")
            {
                _LedgerType = "A";
                m_parameterWhere.Append(" ) WHERE c.ad_client_id=" + GetCtx().GetAD_Client_ID() + " and c.AccountType='A' or c.AccountType='L' or c.AccountType='O' ");
            }
            else
            {
                if (_LedgerType == "N")
                {
                    _SummaryLevel = "N";
                }
                else if (_LedgerType == "S")
                {
                    _SummaryLevel = "Y";
                }
                if (IsIntermediate == "Y")
                {
                    m_parameterWhere.Append(" ) WHERE  c.issummary='" + _SummaryLevel + "' and c.AccountType='A' or c.AccountType='L' or c.AccountType='O' ");
                }
                if (IsIntermediate == "N")
                {
                    // m_parameterWhere.Append(" ) WHERE  c.issummary='" + _SummaryLevel + "' and c.ad_client_id=" + GetCtx().GetAD_Client_ID() + " AND c.isintermediatecode='N' ");
                    m_parameterWhere.Append(" ) WHERE  c.issummary='" + _SummaryLevel + "' and c.AccountType='A' or c.AccountType='L' or c.AccountType='O' AND c.isintermediatecode='N' ");
                }
                //  m_parameterWhere.Append(" ) WHERE  c.issummary='" + _SummaryLevel + "' and c.AccountType='A' or c.AccountType='L' or c.AccountType='O' ");
            }
            GroupBy(totalQty);
            // sql.Append(" t1 Inner join");

        }
        private void GroupBy(bool totalQty)
        {
            if (totalQty)
            {
                m_parameterWhere.Append(@" GROUP BY a.parent_id, c.AccountType,a.node_id,c.value,  c.name,c.AccountType,c.ad_client_id,c.ad_org_id,c.AccountType,f.C_PROJECT_ID,c.isintermediatecode)abc where abc.total!=0)");
            }
            else
            {
                m_parameterWhere.Append(@" GROUP BY a.parent_id, c.AccountType,a.node_id,c.value,  c.name,c.AccountType,c.ad_client_id,c.ad_org_id,c.AccountType,f.C_PROJECT_ID,c.isintermediatecode)abc where abc.Currentamount!=0)");
            }
        }

        private void CreateView()
        {
            string _sqlview = @"CREATE OR REPLACE FORCE VIEW BALSHEET_V  as SELECT ad_client_id            ,  accounttype                   ,  ad_org_id                     ,  Rownum AS BalSheet_V_ID       ,  C_PROJECT_ID                  ,  C_PERIOD_ID                   ,  ledgertype              AS LedgerType      ,  LEDGERNAME              AS LEDGER          ,  issummary               AS ShowSummaryLevel,  NVL(ABS(PMDEBIT),0)     AS PreMonDebit     ,  NVL(ABS(PMCREDIT),0)    AS PreMonCredit    ,  NVL(ABS(CMDEBIT),0)     AS CRMonDebit      ,  NVL(ABS(CMCREDIT),0)    AS CRMonCredit     ,  NVL(ABS(TOTALDEBIT),0)  AS TotalDebit      ,  NVL(ABS(TOTALCREDIT),0) AS TotalCredit     ,  NVL(ABS(DIFFDEBIT),0)   AS DiffDebitVal    ,  NVL(ABS(DIFFCREDIT),0)  AS DiffCreditVal   ,  ledgercode                                 ,  SUBSTR( ledgercode,1,1)  AS ledgercode1     ,  SUBSTR( ledgercode,2,2)  AS ledgercode2     ,  SUBSTR( ledgercode,4,2)  AS ledgercode3     ,  SUBSTR( ledgercode,6,2)  AS ledgercode4     ,  SUBSTR( ledgercode,8,2)  AS ledgercode5     ,  SUBSTR( ledgercode,10,2) AS ledgercode6     ,  SUBSTR( ledgercode,12,2) AS ledgercode7   FROM t_Balsheet  WHERE accounttype ='A' OR accounttype      ='L' OR accounttype      ='O' ORDER BY LEDGERCODE";
            DB.ExecuteQuery(_sqlview, null, null);
        }
        private void CreateViewAsTree()
        {
            string _sqlview = @"CREATE OR REPLACE FORCE VIEW BALSHEET_V AS  SELECT ad_client_id      ,  accounttype             ,  ad_org_id               ,  Rownum AS BalSheet_V_ID ,  C_PROJECT_ID            ,  C_PERIOD_ID             ,  ledgertype AS LedgerType,  '.'  || LPAD (' ', LEVEL * 3)  || LEDGERNAME           AS LEDGER          ,  issummary               AS ShowSummaryLevel,  NVL(ABS(PMDEBIT),0)     AS PreMonDebit     ,  NVL(ABS(PMCREDIT),0)    AS PreMonCredit    ,  NVL(ABS(CMDEBIT),0)     AS CRMonDebit      ,  NVL(ABS(CMCREDIT),0)    AS CRMonCredit     ,  NVL(ABS(TOTALDEBIT),0)  AS TotalDebit      ,  NVL(ABS(TOTALCREDIT),0) AS TotalCredit     ,  NVL(ABS(DIFFDEBIT),0)   AS DiffDebitVal    ,  NVL(ABS(DIFFCREDIT),0)  AS DiffCreditVal   ,  ledgercode                                 ,  SUBSTR( ledgercode,1,1)  AS ledgercode1     ,  SUBSTR( ledgercode,2,2)  AS ledgercode2     ,  SUBSTR( ledgercode,4,2)  AS ledgercode3     ,  SUBSTR( ledgercode,6,2)  AS ledgercode4     ,  SUBSTR( ledgercode,8,2)  AS ledgercode5     ,  SUBSTR( ledgercode,10,2) AS ledgercode6     ,  SUBSTR( ledgercode,12,2) AS ledgercode7   FROM t_Balsheet  WHERE accounttype ='A' OR accounttype      ='L' OR accounttype      ='O' START WITH parent_id      =0 CONNECT BY parent_id = PRIOR node_id ORDER SIBLINGS BY LEDGERCODE";
            DB.ExecuteQuery(_sqlview, null, null);
        }
        private void CreateBalanceLine()
        {
            if (_C_Project_ID != 0 )
            {
                sql.Append(" and e.C_Project_ID=" + _C_Project_ID + "");
            }
            if (_C_Period_ID != 0 )
            {
                sql.Append("and e.C_Period_ID=" + _C_Period_ID + "");
            }
            if (_LedgerType != "")
            {
                sql.Append("and e.LedgerType=" + _LedgerType + "");
            }

            //if (_GL_Budget_ID == 0 && _C_BPartner_ID == 0 && _AD_OrgTrx_ID == 0 && _UserElement2_ID == 0 && _UserElement1_ID == 0 && _User1_ID == 0 && _User2_ID == 0 &&
            //    _C_LocFrom_ID == 0 && _C_LocTo_ID == 0 && _C_SalesRegion_ID == 0 && _C_Activity_ID == 0 && _C_Campaign_ID == 0 && _C_Project_ID == 0 && _C_BPartner_ID == 0 &&
            //    _GL_Budget_ID == 0 && _M_Product_ID =0)
            //{

            //}

            // DateTime? balanceDay = _DateAcct_From; // TimeUtil.addDays(_DateAcct_From, -1);
            //sql.Append(",null,").Append(DB.TO_DATE(balanceDay, true)).Append(",");

        }
        private void WhereClause()
        {
            //    if (_GL_Budget_ID != 0 || _GL_Budget_ID != null)
            //    {
            //        sql.Append(" and GL_Budget_ID=" + _GL_Budget_ID + "");
            //    }

            if (_C_Project_ID != 0 )
            {
                sql.Append(" and C_Project_ID=" + _C_Project_ID + "");
            }
            if (_C_Period_ID != 0 )
            {
                sql.Append(" and _C_Period_ID=" + _C_Period_ID + "");
            }
            if (_LedgerType != "")
            {
                sql.Append(" and _LedgerType=" + _LedgerType + "");
            }

            //if (_GL_Budget_ID == 0 && _C_BPartner_ID == 0 && _AD_OrgTrx_ID == 0 && _UserElement2_ID == 0 && _UserElement1_ID == 0 && _User1_ID == 0 && _User2_ID == 0 &&
            //    _C_LocFrom_ID == 0 && _C_LocTo_ID == 0 && _C_SalesRegion_ID == 0 && _C_Activity_ID == 0 && _C_Campaign_ID == 0 && _C_Project_ID == 0 && _C_BPartner_ID == 0 &&
            //    _GL_Budget_ID == 0 && _M_Product_ID =0)
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

                else if (name.Equals("C_Project_ID"))
                {
                    _C_Project_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_Period_ID"))
                {
                    _C_Period_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
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
            if (_C_Period_ID == 0 || _C_Period_ID == -1)
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
                String sql = "SELECT StartDate, EndDate FROM C_Period WHERE C_Period_ID=@param1";
                SqlParameter[] param = new SqlParameter[1];
                IDataReader idr = null;
                try
                {

                    param[0] = new SqlParameter("@param1", _C_Period_ID);
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
