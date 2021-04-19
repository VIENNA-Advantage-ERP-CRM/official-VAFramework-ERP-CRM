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
        MVABProfitLoss prof = null;
        int VAB_ProfitLossLines_ID = 0;
        static readonly object lockRecord = new object();

        protected override string DoIt()
        {
            MVABProfitLoss PL = new MVABProfitLoss(GetCtx(), GetRecord_ID(), null);
            prof = new MVABProfitLoss(GetCtx(), GetRecord_ID(), Get_Trx());

            stDate = Util.GetValueOfDateTime(DB.ExecuteScalar("select p.startdate from VAB_YearPeriod p  inner join VAB_Year y on p.VAB_Year_id=y.VAB_Year_id where p.periodno='1' and p.VAB_Year_id= " + prof.GetVAB_Year_ID() + " and y.vaf_client_id= " + GetVAF_Client_ID(), null, null));
            eDate = Util.GetValueOfDateTime(DB.ExecuteScalar("select p.enddate from VAB_YearPeriod p  inner join VAB_Year y on p.VAB_Year_id=y.VAB_Year_id where p.periodno='12' and p.VAB_Year_id= " + prof.GetVAB_Year_ID() + " and y.vaf_client_id= " + GetVAF_Client_ID(), null, null));

            // lock record 
            lock (lockRecord)
            {
                sql.Clear();
                sql.Append("SELECT distinct CP.* FROM VAB_ProfitLoss CP INNER JOIN Actual_Acct_Detail ft ON ft.VAB_AccountBook_ID = Cp.VAB_AccountBook_ID                             "
                            + " INNER JOIN VAB_Acct_Element ev ON ft.account_id         =ev.VAB_Acct_Element_id                                                           "
                            + " WHERE CP.vaf_client_id    = " + GetVAF_Client_ID());

                if (prof.Get_Value("PostingType") != null)
                {
                    sql.Append(" and CP.PostingType = '" + prof.Get_Value("PostingType") + "' ");
                }
                sql.Append(" AND (( " + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " >= CP.DateFrom "
                         + " AND " + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " <= CP.DateTo "
                         + " OR " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " <= CP.DateFrom "
                         + " AND " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " <= CP.DateTo ))  "
                         + " AND (ev.accounttype      ='E' OR ev.accounttype        ='R')     "
                         + " AND ev.isintermediatecode='N' AND CP.VAF_Org_ID        IN (    (SELECT vaf_org_ID   FROM VAF_Org   WHERE isactive      = 'Y'             "
                         + " AND ( " + DBFunctionCollection.TypecastColumnAsInt("legalentityorg") + " =" + PL.GetVAF_Org_ID() + "  OR vaf_org_ID = " + PL.GetVAF_Org_ID() + ")  )) AND DOCstatus in ('CO', 'CL') ");

                if (Util.GetValueOfInt(PL.Get_Value("VAB_AccountBook_ID")) > 0)
                {
                    sql.Append(" AND Cp.VAB_AccountBook_ID=" + Util.GetValueOfInt(PL.Get_Value("VAB_AccountBook_ID")));
                }
                ds1 = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
                {
                    return Msg.GetMsg(GetCtx(), "VIS_RecordsAlreadyGenerated");
                }

                // Get currentNext for generating key
                VAB_ProfitLossLines_ID = MVAFRecordSeq.GetNextID(GetVAF_Client_ID(), "VAB_ProfitLossLines", Get_Trx());
                VAB_ProfitLossLines_ID -= 1;

                // Delete All Record from line
                DB.ExecuteQuery("DELETE FROM VAB_ProfitLossLines WHERE VAB_ProfitLoss_ID=" + GetRecord_ID());

                insert.Clear();
                insert.Append(@"INSERT INTO VAB_ProfitLossLines (VAB_ProfitLossLines_ID , VAF_Client_ID , VAF_Org_ID , VAB_ProfitLoss_ID , VAB_ProfitAndLoss_ID , VAB_AccountBook_ID , PostingType ,
                            AccountCredit ,AccountDebit,Account_ID, VAB_SubAcct_ID,  VAB_BusinessPartner_ID , VAM_Product_ID , VAB_Project_ID , VAB_SalesRegionState_ID ,  VAB_Promotion_ID , VAF_OrgTrx_ID ,
                           VAB_LocFrom_ID , VAB_LocTo_ID , VAB_BillingCode_ID, User1_ID , User2_ID , UserElement1_ID, UserElement2_ID, UserElement3_ID, UserElement4_ID,
                          UserElement5_ID, UserElement6_ID, UserElement7_ID, UserElement8_ID, UserElement9_ID , VAGL_Budget_ID, VAB_ProjectStage_ID, VAB_ProjectJob_ID, LedgerCode,LedgerName, Line ) ");

                qry.Clear();
                qry.Append(@"select " + VAB_ProfitLossLines_ID + " + " + DBFunctionCollection.RowNumAggregation("rownum") + " AS VAB_ProfitLossLines_id, ft.VAF_Client_ID , ft.VAF_Org_ID , " + PL.GetVAB_ProfitLoss_ID() + " , " + prof.GetVAB_ProfitAndLoss_ID() + ",  ft.VAB_AccountBook_ID,ft.PostingType,ft.AmtAcctDr,ft.AmtAcctCr,ft.Account_ID,ft.VAB_SubAcct_ID,ft.VAB_BusinessPartner_ID,ft.VAM_Product_ID,ft.VAB_Project_ID,ft.VAB_SalesRegionState_ID,ft.VAB_Promotion_ID,ft.VAF_OrgTrx_ID,ft.VAB_LocFrom_ID,ft.VAB_LocTo_ID,ft.VAB_BillingCode_ID,ft.User1_ID,ft.User2_ID,ft.UserElement1_ID,ft.UserElement2_ID,"
                         + " ft.UserElement3_ID,ft.UserElement4_ID, ft.UserElement5_ID, ft.UserElement6_ID, ft.UserElement7_ID,ft.UserElement8_ID, ft.UserElement9_ID,ft.VAGL_Budget_ID,ft.VAB_ProjectStage_ID,ft.VAB_ProjectJob_ID,"
                         + @" ev.Value as LedgerCode,ev.Name as LedgerName , (SELECT NVL(MAX(Line),0) FROM VAB_ProfitLossLines   WHERE VAB_ProfitLoss_ID=" + PL.GetVAB_ProfitLoss_ID() + "   ) + (" + DBFunctionCollection.RowNumAggregation("rownum") + " *10) AS lineno from Actual_Acct_Detail ft inner join VAB_Acct_Element ev on ft.account_id=ev.VAB_Acct_Element_id where ft.vaf_client_id= " + GetVAF_Client_ID());


                // Added by SUkhwinder on 27 Nov 2017, for filtering query on the basis of postingtype. And string variable converted to stringBuilder also.
                if (prof.Get_Value("PostingType") != null)
                {
                    qry.Append(" and ft.PostingType = '" + prof.Get_Value("PostingType") + "'");
                }

                qry.Append(" and ft.DateAcct >=" + GlobalVariable.TO_DATE(prof.GetDateFrom(), true) + " AND ft.DateAcct <= " + GlobalVariable.TO_DATE(prof.GetDateTo(), true) + " AND (ev.accounttype='E' OR ev.accounttype='R') and ev.isintermediatecode='N'"
                + "   AND ft.VAF_Org_ID IN ( (SELECT vaf_org_ID FROM VAF_Org WHERE isactive = 'Y' AND (" + DBFunctionCollection.TypecastColumnAsInt("legalentityorg") + " =" + PL.GetVAF_Org_ID() + " OR vaf_org_ID = " + PL.GetVAF_Org_ID() + ")))");

                if (Util.GetValueOfInt(PL.Get_Value("VAB_AccountBook_ID")) > 0)
                {
                    qry.Append(" AND ft.VAB_AccountBook_ID=" + Util.GetValueOfInt(PL.Get_Value("VAB_AccountBook_ID")));
                }
                insert.Append(qry.ToString());

                int no = Util.GetValueOfInt(DB.ExecuteQuery(insert.ToString(), null, Get_Trx()));
                if (no > 0)
                {
                    // Update curentNext in VAF_Record_Seq 
                    VAB_ProfitLossLines_ID += (no + 1);
                    String updateSQL = "UPDATE VAF_Record_Seq SET  CurrentNext = " + VAB_ProfitLossLines_ID + ", CurrentNextSys = " + VAB_ProfitLossLines_ID + " "
                    + " WHERE Upper(Name)=Upper('VAB_ProfitLossLines')"
                    + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' ";
                    DB.ExecuteQuery(updateSQL, null, Get_Trx());

                    // Update Profit Before tax on Header
                    ProfitBeforeTax = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(AccountDebit) - SUM(AccountCredit) FROM VAB_ProfitLossLines WHERE  VAB_ProfitAndLoss_ID > 0 AND  IsActive='Y' AND VAB_ProfitLoss_ID=" + GetRecord_ID(), null, Get_Trx()));
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
        private void InsertProfitLossLine(MVABProfitLoss Profit)
        {
            // get consolidated profit amount agsint Organization
            String Sql = @"SELECT VAF_Org_ID , (SUM(AccountDebit) - SUM(AccountCredit)) AS ProfitAmt
                                    FROM VAB_ProfitLossLines WHERE VAB_ProfitAndLoss_ID > 0 AND VAB_ProfitLoss_ID = " + Profit.GetVAB_ProfitLoss_ID() + @" GROUP BY VAF_Org_ID";
            DataSet dsProfit = DB.ExecuteDataset(Sql, null, Get_Trx());
            if (dsProfit != null && dsProfit.Tables.Count > 0 && dsProfit.Tables[0].Rows.Count > 0)
            {
                // get max line no 
                int lineNo = Convert.ToInt32(DB.ExecuteScalar(@"SELECT NVL(MAX(Line),0)+10 AS line FROM VAB_ProfitLossLines WHERE VAB_ProfitLoss_ID = " + Profit.GetVAB_ProfitLoss_ID(), null, Get_Trx()));

                // get Valid Combination against Income Summary Acct  from accounting schema
                int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT IncomeSummary_Acct FROM VAB_AccountBook_GL WHERE VAB_AccountBook_ID=" + Util.GetValueOfInt(Profit.Get_Value("VAB_AccountBook_ID"))));
                // get account id 
                MVABAccount acct = MVABAccount.Get(GetCtx(), validComID);

                for (int i = 0; i < dsProfit.Tables[0].Rows.Count; i++)
                {
                    Decimal profitAmt = Util.GetValueOfDecimal(dsProfit.Tables[0].Rows[i]["ProfitAmt"]);

                    // when difference of DR - CR is ZERO then not to create line
                    if (profitAmt == 0)
                    {
                        continue;
                    }
                    MVABProfitLossLines profitLossLines = new MVABProfitLossLines(GetCtx(), 0, Get_Trx());
                    profitLossLines.SetVAF_Client_ID(Profit.GetVAF_Client_ID());
                    profitLossLines.SetVAF_Org_ID(Util.GetValueOfInt(dsProfit.Tables[0].Rows[i]["VAF_Org_ID"]));
                    profitLossLines.SetVAB_ProfitLoss_ID(Profit.GetVAB_ProfitLoss_ID());
                    profitLossLines.SetLine(lineNo);
                    profitLossLines.SetVAB_AccountBook_ID(Util.GetValueOfInt(Profit.Get_Value("VAB_AccountBook_ID")));
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
