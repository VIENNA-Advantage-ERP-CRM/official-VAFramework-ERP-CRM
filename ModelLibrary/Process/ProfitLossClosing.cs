using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Model;
using VAdvantage.Logging;

namespace VAdvantage.Process
{
    class ProfitLossClosing : SvrProcess
    {
        StringBuilder qry = new StringBuilder("");
        StringBuilder sql = new StringBuilder("");
        StringBuilder insert = new StringBuilder();
        DateTime? stDate, eDate;
        Decimal ProfitBeforeTax = 0.0M;
        DataSet ds = null;
        DataSet ds1 = null;
        MProfitLoss prof = null;
        int C_ProfitLossLines_ID = 0;
        static readonly object lockRecord = new object();

        protected override string DoIt()
        {
            MClient client = MClient.Get(GetCtx());
            MProfitLoss PL = new MProfitLoss(GetCtx(), GetRecord_ID(), null);
            prof = new MProfitLoss(GetCtx(), GetRecord_ID(), Get_Trx());

            stDate = Util.GetValueOfDateTime(DB.ExecuteScalar("select p.startdate from c_period p  inner join c_year y on p.c_year_id=y.c_year_id where p.periodno='1' and p.c_year_id= " + prof.GetC_Year_ID() + " and y.ad_client_id= " + GetAD_Client_ID(), null, null));
            eDate = Util.GetValueOfDateTime(DB.ExecuteScalar("select p.enddate from c_period p  inner join c_year y on p.c_year_id=y.c_year_id where p.periodno='12' and p.c_year_id= " + prof.GetC_Year_ID() + " and y.ad_client_id= " + GetAD_Client_ID(), null, null));

            // lock record 
            lock (lockRecord)
            {
                sql.Clear();
                sql.Append("SELECT DISTINCT CP.* FROM C_ProfitLoss CP INNER JOIN Fact_Acct ft ON ft.C_AcctSchema_ID = Cp.C_AcctSchema_ID                             "
                            + " INNER JOIN C_ElementValue ev ON ft.Account_ID=ev.C_ElementValue_ID                                                           "
                            + " WHERE CP.AD_Client_ID=" + GetAD_Client_ID());

                if (prof.Get_Value("PostingType") != null)
                {
                    sql.Append(" AND CP.PostingType = '" + prof.Get_Value("PostingType") + "'");
                }
                sql.Append(" AND (( " + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " >= CP.DateFrom "
                         + " AND " + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " <= CP.DateTo "
                         + " OR " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " <= CP.DateFrom "
                         + " AND " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " <= CP.DateTo ))  "
                         + " AND (ev.AccountType='E' OR ev.AccountType='R')"
                         + " AND ev.IsIntermediateCode='N' AND CP.DocStatus IN ('CO', 'CL') AND CP.AD_Org_ID IN (SELECT Ad_Org_ID FROM AD_Org WHERE IsActive='Y'");

                // VIS0060: Handle Organization filter based on Profit & Loss closing settings on Tenant
                if (client.Get_ColumnIndex("PLClosing") >= 0 && !string.IsNullOrEmpty(client.GetPLClosing()) && !client.GetPLClosing().Equals("LOW"))
                {
                    sql.Append(" AND Ad_Org_ID = " + PL.GetAD_Org_ID() + ")");
                }
                else
                {
                    sql.Append(" AND (" + DBFunctionCollection.TypecastColumnAsInt("LegalEntityOrg") + "=" + PL.GetAD_Org_ID() + "  OR Ad_Org_ID = " + PL.GetAD_Org_ID() + "))");
                }

                if (Util.GetValueOfInt(PL.Get_Value("C_AcctSchema_ID")) > 0)
                {
                    sql.Append(" AND Cp.C_AcctSchema_ID=" + Util.GetValueOfInt(PL.Get_Value("C_AcctSchema_ID")));
                }
                ds1 = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
                {
                    return Msg.GetMsg(GetCtx(), "VIS_RecordsAlreadyGenerated");
                }

                // Get currentNext for generating key
                C_ProfitLossLines_ID = MSequence.GetNextID(GetAD_Client_ID(), "C_ProfitLossLines", Get_Trx());
                C_ProfitLossLines_ID -= 1;

                // Delete All Record from line
                DB.ExecuteQuery("DELETE FROM C_ProfitLossLines WHERE C_ProfitLoss_ID=" + GetRecord_ID());

                insert.Clear();
                insert.Append(@"INSERT INTO C_ProfitLossLines (C_ProfitLossLines_ID , AD_Client_ID , AD_Org_ID , C_ProfitLoss_ID , C_ProfitAndLoss_ID , C_AcctSchema_ID , PostingType ,
                            AccountCredit ,AccountDebit,Account_ID, C_SubAcct_ID,  C_BPartner_ID , M_Product_ID , C_Project_ID , C_SalesRegion_ID ,  C_Campaign_ID , AD_OrgTrx_ID ,
                           C_LocFrom_ID , C_LocTo_ID , C_Activity_ID, User1_ID , User2_ID , UserElement1_ID, UserElement2_ID, UserElement3_ID, UserElement4_ID,
                          UserElement5_ID, UserElement6_ID, UserElement7_ID, UserElement8_ID, UserElement9_ID , GL_Budget_ID, C_ProjectPhase_ID, C_ProjectTask_ID, LedgerCode,LedgerName, Line ) ");

                qry.Clear();
                qry.Append(@"select " + (!DB.IsPostgreSQL() ? "MAX" : "") + "(" + C_ProfitLossLines_ID + " + " + DBFunctionCollection.RowNumAggregation("rownum") + ") AS C_ProfitLossLines_id, ft.AD_Client_ID , ft.AD_Org_ID , " + PL.GetC_ProfitLoss_ID() + " , " + prof.GetC_ProfitAndLoss_ID() + ",  ft.C_AcctSchema_ID,ft.PostingType, SUM(NVL(ft.AmtAcctDr, 0)),SUM(NVL(ft.AmtAcctCr, 0)),ft.Account_ID,ft.C_SubAcct_ID,ft.C_BPartner_ID,ft.M_Product_ID,ft.C_Project_ID,ft.C_SalesRegion_ID,ft.C_Campaign_ID,ft.AD_OrgTrx_ID,ft.C_LocFrom_ID,ft.C_LocTo_ID,ft.C_Activity_ID,ft.User1_ID,ft.User2_ID,ft.UserElement1_ID,ft.UserElement2_ID,"
                         + " ft.UserElement3_ID,ft.UserElement4_ID, ft.UserElement5_ID, ft.UserElement6_ID, ft.UserElement7_ID,ft.UserElement8_ID, ft.UserElement9_ID,ft.GL_Budget_ID,ft.C_ProjectPhase_ID,ft.C_ProjectTask_ID,"
                         + @" ev.Value as LedgerCode,ev.Name as LedgerName , " + (!DB.IsPostgreSQL() ? "MAX" : "") + @"((SELECT NVL(MAX(Line), 0) FROM C_ProfitLossLines   WHERE C_ProfitLoss_ID = " + PL.GetC_ProfitLoss_ID() + ") + (" + DBFunctionCollection.RowNumAggregation("rownum") + " * 10)) AS lineno from Fact_Acct ft inner join c_elementvalue ev on ft.account_id = ev.c_elementvalue_id where ft.ad_client_id = " + GetAD_Client_ID());


                // Added by SUkhwinder on 27 Nov 2017, for filtering query on the basis of postingtype. And string variable converted to stringBuilder also.
                if (prof.Get_Value("PostingType") != null)
                {
                    qry.Append(" AND ft.PostingType = '" + prof.Get_Value("PostingType") + "'");
                }

                qry.Append(" AND ft.DateAcct >=" + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " AND ft.DateAcct <= " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " AND (ev.accounttype='E' OR ev.accounttype='R') and ev.isintermediatecode='N'"
                + " AND ft.AD_Org_ID IN (SELECT Ad_Org_ID FROM AD_Org WHERE IsActive='Y'");

                // VIS0060: Handle Organization filter based on Profit & Loss closing settings on Tenant.
                if (client.Get_ColumnIndex("PLClosing") >= 0 && !string.IsNullOrEmpty(client.GetPLClosing()) && !client.GetPLClosing().Equals("LOW"))
                {
                    qry.Append(" AND Ad_Org_ID = " + PL.GetAD_Org_ID() + ")");
                }
                else
                {
                    qry.Append(" AND (" + DBFunctionCollection.TypecastColumnAsInt("LegalEntityOrg") + "=" + PL.GetAD_Org_ID() + "  OR Ad_Org_ID = " + PL.GetAD_Org_ID() + "))");
                }

                if (Util.GetValueOfInt(PL.Get_Value("C_AcctSchema_ID")) > 0)
                {
                    qry.Append(" AND ft.C_AcctSchema_ID=" + Util.GetValueOfInt(PL.Get_Value("C_AcctSchema_ID")));
                }

                // group by the records, so that we can reduce the no of entry
                qry.Append(@" GROUP BY ft.AD_Client_ID , ft.AD_Org_ID  , ft.C_AcctSchema_ID,ft.PostingType ,
                           ft.Account_ID,ft.C_SubAcct_ID,ft.C_BPartner_ID,ft.M_Product_ID,ft.C_Project_ID,ft.C_SalesRegion_ID,
                           ft.C_Campaign_ID,ft.AD_OrgTrx_ID,ft.C_LocFrom_ID,ft.C_LocTo_ID,ft.C_Activity_ID,ft.User1_ID,
                           ft.User2_ID,ft.UserElement1_ID,ft.UserElement2_ID, ft.UserElement3_ID,ft.UserElement4_ID, 
                           ft.UserElement5_ID, ft.UserElement6_ID, ft.UserElement7_ID,ft.UserElement8_ID, 
                           ft.UserElement9_ID,ft.GL_Budget_ID,ft.C_ProjectPhase_ID,ft.C_ProjectTask_ID, ev.Value,ev.Name");
                insert.Append(qry.ToString());
                log.Info(insert.ToString());
                int no = Util.GetValueOfInt(DB.ExecuteQuery(insert.ToString(), null, Get_Trx()));
                if (no > 0)
                {
                    // Update curentNext in AD_Sequence 
                    C_ProfitLossLines_ID += (no + 1);
                    String updateSQL = "UPDATE AD_Sequence SET  CurrentNext = " + C_ProfitLossLines_ID + ", CurrentNextSys = " + C_ProfitLossLines_ID + " "
                    + " WHERE Upper(Name)=Upper('C_ProfitLossLines')"
                    + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' ";
                    DB.ExecuteQuery(updateSQL, null, Get_Trx());

                    // Update Profit Before tax on Header
                    ProfitBeforeTax = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(AccountDebit) - SUM(AccountCredit) FROM C_ProfitLossLines WHERE  C_ProfitAndLoss_ID > 0 AND  IsActive='Y' AND C_ProfitLoss_ID=" + GetRecord_ID(), null, Get_Trx()));
                    prof.SetProfitBeforeTax(ProfitBeforeTax);
                    if (!prof.Save(Get_Trx()))
                    {
                        ds.Dispose();
                        Rollback();
                        // return GetReterivedError(prof, "ProfitNotSaved");
                        return Msg.GetMsg(GetCtx(), "ProfitNotSaved");
                    }
                    else
                    {
                        Get_Trx().Commit();

                        // insert record against Income summary acct
                        InsertProfitLossLine(prof);
                    }
                    return Msg.GetMsg(GetCtx(), "LinesGenerated");
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "RecordNoFound");
                }
            }
        }

        protected override void Prepare()
        {

        }

        /// <summary>
        /// Is used to create line against Income Account summary account -- 
        /// </summary>
        /// <param name="Profit">class object of MProfitLoss </param>
        private void InsertProfitLossLine(MProfitLoss Profit)
        {
            // get consolidated profit amount agsint Organization
            String Sql = @"SELECT AD_Org_ID , (SUM(AccountDebit) - SUM(AccountCredit)) AS ProfitAmt
                                    FROM C_ProfitLossLines WHERE C_ProfitAndLoss_ID > 0 AND C_ProfitLoss_ID = " + Profit.GetC_ProfitLoss_ID() + @" GROUP BY AD_Org_ID";
            DataSet dsProfit = DB.ExecuteDataset(Sql, null, Get_Trx());
            if (dsProfit != null && dsProfit.Tables.Count > 0 && dsProfit.Tables[0].Rows.Count > 0)
            {
                // get max line no 
                int lineNo = Convert.ToInt32(DB.ExecuteScalar(@"SELECT NVL(MAX(Line),0)+10 AS line FROM C_ProfitLossLines WHERE C_ProfitLoss_ID = " + Profit.GetC_ProfitLoss_ID(), null, Get_Trx()));

                // get Valid Combination against Income Summary Acct  from accounting schema
                int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT IncomeSummary_Acct FROM C_AcctSchema_GL WHERE C_AcctSchema_ID=" + Util.GetValueOfInt(Profit.Get_Value("C_AcctSchema_ID"))));
                // get account id 
                MAccount acct = MAccount.Get(GetCtx(), validComID);

                for (int i = 0; i < dsProfit.Tables[0].Rows.Count; i++)
                {
                    Decimal profitAmt = Util.GetValueOfDecimal(dsProfit.Tables[0].Rows[i]["ProfitAmt"]);

                    // when difference of DR - CR is ZERO then not to create line
                    if (profitAmt == 0)
                    {
                        continue;
                    }
                    MProfitLossLines profitLossLines = new MProfitLossLines(GetCtx(), 0, Get_Trx());
                    profitLossLines.SetAD_Client_ID(Profit.GetAD_Client_ID());
                    profitLossLines.SetAD_Org_ID(Util.GetValueOfInt(dsProfit.Tables[0].Rows[i]["AD_Org_ID"]));
                    profitLossLines.SetC_ProfitLoss_ID(Profit.GetC_ProfitLoss_ID());
                    profitLossLines.SetLine(lineNo);
                    profitLossLines.SetC_AcctSchema_ID(Util.GetValueOfInt(Profit.Get_Value("C_AcctSchema_ID")));
                    profitLossLines.SetPostingType(Util.GetValueOfString(Profit.Get_Value("PostingType")));
                    profitLossLines.SetAccount_ID(acct.GetAccount_ID());
                    profitLossLines.Set_Value("LedgerCode", acct.GetAccount().Get_Value("Value"));
                    profitLossLines.Set_Value("LedgerName", acct.GetAccount().Get_Value("Name"));
                    if (profitAmt > 0)
                    {
                        profitLossLines.SetAccountCredit(profitAmt);
                    }
                    else
                    {
                        profitLossLines.SetAccountDebit(profitAmt);
                    }
                    if (!profitLossLines.Save(Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        log.Fine("Failed - Profit loss line not saved for income acct summary - " + (pp != null && !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : ""));
                    }
                    else
                    {
                        lineNo += 10;
                    }
                }
            }
        }

    }
}
