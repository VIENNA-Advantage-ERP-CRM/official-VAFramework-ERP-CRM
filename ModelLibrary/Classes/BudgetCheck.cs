using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace ModelLibrary.Classes
{
    public class BudgetCheck
    {
        #region Private Variables
        StringBuilder sql = null;
        private string tableName = "";
        private int _AD_Table_ID = 0;
        private int _Record_ID = 0;
        //private string _trxName = null;
        private Trx _trx = null;
        private int _AD_Client_ID = 0;
        private int _AD_Org_ID = 0;
        private StringBuilder whereClause = new StringBuilder();
        private string controlBasis = "";
        private int C_AcctSchema_ID = 0;
        private Decimal? bgtAmount = 0;
        private Decimal? maxAmount = 0;
        private string _docAction = "";
        protected internal VLogger log = null;
        private Ctx _ctx = null;
        #endregion

        public BudgetCheck()
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
        }

        public string CheckBudget()
        {
            if (_AD_Table_ID > 0)
            {
                GetTableName();

                if (_docAction.Equals("--"))
                {
                    if (_AD_Table_ID.Equals(259))
                    {
                        sql.Clear();
                        sql.Append("UPDATE " + tableName + " SET IsBudgetViolated = 'N', MaxBudgetViolationAmount = 0 WHERE " + tableName + "_ID = " + _Record_ID);
                        int upd = DB.ExecuteQuery(sql.ToString(), null, _trx);

                        sql.Clear();
                        sql.Append("UPDATE C_OrderLine SET BudgetViolationAmount = 0 WHERE " + tableName + "_ID = " + _Record_ID);
                        upd = DB.ExecuteQuery(sql.ToString(), null, _trx);
                    }
                    return "";
                }

                sql.Clear();
                try
                {
                    sql.Append("SELECT BudgetControlBasis FROM AD_Org WHERE AD_Org_ID = " + _AD_Org_ID);

                    controlBasis = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, null));
                }
                catch
                {

                }
                if (controlBasis != "")
                {
                    sql.Clear();
                    sql.Append(@" SELECT DocBaseType, Name, IsSOTrx, IsReturnTrx FROM C_DocType WHERE C_DocType_ID = (SELECT C_DocTypeTarget_ID FROM " + tableName + " WHERE " + tableName + "_ID = " + _Record_ID + ")");
                    DataSet ds = DB.ExecuteDataset(sql.ToString(), null, null);
                    sql.Clear();
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string docBaseType = Util.GetValueOfString(ds.Tables[0].Rows[0]["DocBaseType"]);
                            if (docBaseType.Trim() != "")
                            {
                                ProcessDocBaseType(docBaseType);
                            }
                        }
                    }
                    else
                    {
                        log.SaveError("DocBaseType Not Found", "DocBaseType Not Found");
                    }
                }
                else
                {
                    log.SaveError("Budget Control Basis not Defined", "Budget Control Basis not Defined");
                }
            }

            return "";
        }

        /// <summary>
        /// Document Base type for Process e.g. POO for Purchase Order
        /// </summary>
        /// <param name="docBaseType"></param>
        private void ProcessDocBaseType(string docBaseType)
        {
            int M_Product_ID = 0;
            DateTime? sDate = null;
            DateTime? eDate = null;
            if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_PURCHASEORDER.Equals(docBaseType))
            {
                VAdvantage.Model.MOrder ord = new VAdvantage.Model.MOrder(GetCtx(), _Record_ID, _trx);
                if (controlBasis.Equals("A"))
                {
                    sql.Clear();
                    sql.Append(@" SELECT PeriodNo, StartDate , EndDate FROM C_Period WHERE C_Year_ID = (SELECT cy.C_Year_ID FROM C_Period cp inner join  C_Year cy
     ON (cy.c_year_id = cp.c_Year_id) WHERE cy.IsActive = 'Y' AND cy.AD_Client_ID = " + _AD_Client_ID + @" AND StartDate   <= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @"
  AND EndDate     >= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @"
  AND cy.c_calendar_id = (SELECT C_Calendar_ID FROM AD_ClientInfo WHERE ad_clientinfo.ad_client_id = " + _AD_Client_ID + @")) AND PeriodNo IN (1,12)");
                    // sql.Append("SELECT  PeriodNo, StartDate, EndDate FROM C_Period WHERE C_Year_ID = (SELECT C_Year_ID FROM C_Period WHERE IsActive = 'Y' AND AD_Client_ID = " + _AD_Client_ID + " AND StartDate <= " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + " AND EndDate >= " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + ") AND PeriodNo IN (1,12)");
                    DataSet dsPer = DB.ExecuteDataset(sql.ToString(), null, null);
                    if (dsPer != null)
                    {
                        if (dsPer.Tables[0].Rows.Count > 0)
                        {
                            for (int k = 0; k < dsPer.Tables[0].Rows.Count; k++)
                            {
                                if (Util.GetValueOfInt(dsPer.Tables[0].Rows[k]["PeriodNo"]).Equals(1))
                                {
                                    sDate = Util.GetValueOfDateTime(dsPer.Tables[0].Rows[k]["StartDate"]);
                                }
                                if (Util.GetValueOfInt(dsPer.Tables[0].Rows[k]["PeriodNo"]).Equals(12))
                                {
                                    eDate = Util.GetValueOfDateTime(dsPer.Tables[0].Rows[k]["EndDate"]);
                                }
                            }
                        }
                    }
                    else
                    {
                        log.SaveError("Period Not Found", "Period Not Found");
                    }
                    dsPer.Dispose();
                    //whereClause.Append(" AND DateAcct >= " + GlobalVariable.TO_DATE(sDate, false) + " AND DateAcct <= " + GlobalVariable.TO_DATE(eDate, false));
                }
                else if (controlBasis.Equals("P"))
                {
                    sql.Clear();
                    sql.Append(@" SELECT PeriodNo, StartDate , EndDate FROM C_Period cp INNER JOIN C_Year cy ON (cy.c_year_id = cp.c_Year_id) WHERE cy.IsActive = 'Y'
AND cy.AD_Client_ID = " + _AD_Client_ID + @" AND StartDate   <= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @"
AND EndDate     >= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @" AND cy.c_calendar_id  = (SELECT C_Calendar_ID FROM AD_ClientInfo
      WHERE ad_clientinfo.ad_client_id = " + _AD_Client_ID + @")");

                    //sql.Append("SELECT  PeriodNo, StartDate, EndDate FROM C_Period WHERE IsActive = 'Y' AND AD_Client_ID = " + _AD_Client_ID + " AND StartDate <= " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + " AND EndDate >= " + GlobalVariable.TO_DATE(System.DateTime.Now, false));
                    DataSet dsPer = DB.ExecuteDataset(sql.ToString(), null, null);
                    if (dsPer != null)
                    {
                        if (dsPer.Tables[0].Rows.Count > 0)
                        {
                            sDate = Util.GetValueOfDateTime(dsPer.Tables[0].Rows[0]["StartDate"]);
                            eDate = Util.GetValueOfDateTime(dsPer.Tables[0].Rows[0]["EndDate"]);
                        }
                    }
                    else
                    {
                        log.SaveError("Period Not Found", "Period Not Found");
                    }
                    dsPer.Dispose();
                }
                sql.Clear();
                sql.Append("SELECT * FROM C_OrderLine WHERE C_Order_ID = " + _Record_ID + " AND IsActive ='Y'");
                DataSet ds = DB.ExecuteDataset(sql.ToString(), null, null);
                if (ds != null)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        whereClause.Clear();

                        // Amount = 0;
                       
                        VAdvantage.Model.MOrderLine ol = new VAdvantage.Model.MOrderLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]), _trx);
                        M_Product_ID = ol.GetM_Product_ID();
                        int Account_ID = 0;
                        if (M_Product_ID > 0)
                        {
                            Account_ID = GetAccount(M_Product_ID, 0);
                            whereClause.Append(" WHERE fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.Account_ID = " + Account_ID + " AND fa.AD_Org_ID = " + _AD_Org_ID);
                        }
                        else if (ol.GetC_Charge_ID() > 0)
                        {
                            Account_ID = GetAccount(0, ol.GetC_Charge_ID());
                            whereClause.Append(" WHERE fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.Account_ID = " + Account_ID + " AND fa.AD_Org_ID = " + _AD_Org_ID);
                        }
                        whereClause.Append(" AND fa.DateAcct >= " + GlobalVariable.TO_DATE(sDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(eDate, false));
                        //  GetBudgetAgainstDimension(ord);

                        if (Account_ID > 0)
                        {
                            Decimal? lineAmount = GetLineAmount(ord.GetC_Currency_ID(), ol.GetLineNetAmt());
                            int BudgetID = 0;
                            //if (ord.GetC_Project_ID() > 0)
                            //{
                            //    BudgetID = GetProjectBudget(ord.GetC_Project_ID());
                            //}
                            //if (BudgetID <= 0)
                            //{
                            //    BudgetID = GetOrgBudget();
                            //}

                            BudgetID = GetBudgetAmount(Account_ID, sDate, eDate, ord);
                            if (BudgetID > 0)
                            {
                                GetBudgetControlComparison(BudgetID);
                                Decimal? amt = CompareBudget(lineAmount, ord);
                                sql.Clear();
                                if (amt > 0)
                                {
                                    ol.SetBudgetViolationAmount(amt);
                                    if (!ol.Save())
                                    {

                                    }
                                    //sql.Append("UPDATE C_OrderLine SET BudgetViolationAmount = " + amt + " WHERE C_OrderLine_ID = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_OrderLine_ID"]));
                                    //int res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, null));
                                }
                            }
                            else
                            {
                                log.SaveError("Budget Not Found", "Budget Not Found");
                            }
                        }
                        else
                        {
                            log.SaveError("No Account Found", "No Account Found");
                        }
                        //  bool chk = ol.Save(_trxName);
                    }
                }
                else
                {
                    log.SaveError("No OrderLines Found", "No OrderLines Found");
                }
            }
            #region
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_SALESORDER.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_APCREDITMEMO.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_APINVOICE.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_APPAYMENT.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_ARCREDITMEMO.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_ARINVOICE.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_ARPROFORMAINVOICE.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_ARRECEIPT.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_BANKSTATEMENT.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_CASHJOURNAL.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_FIXASSET.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_GLDOCUMENT.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_GLJOURNAL.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATCHINVOICE.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATCHPO.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALMOVEMENT.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALPICK.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALPRODUCTION.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALPUTAWAY.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALRECEIPT.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_MATERIALREPLENISHMENT.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_PROJECTISSUE.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_PURCHASEREQUISITION.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_STANDARDCOSTUPDATE.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_WORKORDER.Equals(docBaseType))
            {

            }
            else if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_WORKORDERTRANSACTION.Equals(docBaseType))
            {

            }
            #endregion
        }

        private Decimal? GetLineAmount(int C_Currency_ID, Decimal? lineAmt)
        {
            Decimal? amt = 0;
            sql.Clear();
            sql.Append("SELECT C_Currency_ID FROM C_AcctSchema WHERE C_AcctSchema_ID = (SELECT C_AcctSchema1_ID FROM AD_ClientInfo WHERE AD_Client_ID = " + _AD_Client_ID + ")");
            int accCurrency = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            if (C_Currency_ID.Equals(accCurrency))
            {
                return lineAmt;
            }
            else
            {
                amt = VAdvantage.Model.MConversionRate.Convert(GetCtx(), lineAmt.Value, C_Currency_ID, accCurrency, _AD_Client_ID, _AD_Org_ID);
                return amt;
            }
        }

        private Decimal? CompareBudget(Decimal? lineAmt, VAdvantage.Model.MOrder order)
        {
            Decimal? amt = 0;
            sql.Clear();
            sql.Append(" SELECT SUM(fa.AmtAcctDr) FROM Fact_Acct fa " + whereClause);
            Decimal? accBudget = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
            if (bgtAmount < Decimal.Add(accBudget.Value, lineAmt.Value))
            {
                amt = Decimal.Subtract(Decimal.Add(accBudget.Value, lineAmt.Value), bgtAmount.Value);
                if (maxAmount < amt)
                {
                    //sql.Clear();
                    order.SetIsBudgetViolated(true);
                    order.SetMaxBudgetViolationAmount(amt);
                    if (!order.Save())
                    {

                    }
                    //sql.Append("UPDATE " + tableName + " SET IsBudgetViolated = 'Y', MaxBudgetViolationAmount = " + amt + " WHERE " + tableName + "_ID = " + _Record_ID);
                    //int res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, null));
                }
            }
            return amt;
        }

        private void GetBudgetControlComparison(int BudgetID)
        {
            string commType = "'A',";
            sql.Clear();
            sql.Append("SELECT CommitmentType FROM GL_BudgetControl WHERE GL_Budget_ID = " + BudgetID + " AND IsActive = 'Y'");
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    commType = commType + "'" + idr["CommitmentType"] + "',";
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (commType != "")
            {
                whereClause.Append(" AND PostingType IN (" + commType.Remove(commType.Length - 1, 1) + ")");
                //commType = " PostingType IN (" + commType.Remove(commType.Length - 1, 1) + ")";
            }
            //return commType;
        }

        private int GetOrgBudget()
        {
            sql.Clear();
            sql.Append("SELECT GL_Budget_ID FROM AD_Org WHERE AD_Org_ID = " + _AD_Org_ID);
            return Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
        }

        private int GetProjectBudget(int Project_ID)
        {
            sql.Clear();
            sql.Append("SELECT GL_Budget_ID FROM C_Project WHERE C_Project_ID = " + Project_ID);
            return Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
        }

        //
        private int GetBudgetAmount(int AccID, DateTime? startDate, DateTime? endDate, VAdvantage.Model.MOrder ord)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            sql.Clear();
            int C_Project_ID = Util.GetValueOfInt(ord.GetC_Project_ID());
            int C_Campaign_ID = Util.GetValueOfInt(ord.GetC_Campaign_ID());
            int C_Activity_ID = Util.GetValueOfInt(ord.GetC_Activity_ID());
            IDataReader idr = null;

            if (C_Project_ID != 0 && C_Campaign_ID != 0 && C_Activity_ID != 0)
            {
                // find Budget against all three Parameters
                budget_ID = GetBudgetAgainstProjectCampaignActivity(AccID, startDate, endDate, C_Project_ID, C_Campaign_ID, C_Activity_ID);
                // if not found then find against Project and Campaign
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProjectCampaign(AccID, startDate, endDate, C_Project_ID, C_Campaign_ID);
                }
                // Then Against Project and Activity
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProjectActivity(AccID, startDate, endDate, C_Project_ID, C_Activity_ID);
                }
                // Then Against Campaign and Activity
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaignActivity(AccID, startDate, endDate, C_Activity_ID, C_Campaign_ID);
                }
                // Against Project
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, C_Project_ID);
                }
                // Against Project
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, C_Campaign_ID);
                }
                // Against Activity
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, C_Activity_ID);
                }
            }
            else if (C_Project_ID != 0 && C_Campaign_ID != 0)
            {
                budget_ID = GetBudgetAgainstProjectCampaign(AccID, startDate, endDate, C_Project_ID, C_Campaign_ID);
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, C_Project_ID);
                }
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, C_Campaign_ID);
                }
            }
            else if (C_Campaign_ID != 0 && C_Activity_ID != 0)
            {
                budget_ID = GetBudgetAgainstCampaignActivity(AccID, startDate, endDate, C_Campaign_ID, C_Activity_ID);
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, C_Campaign_ID);
                }
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, C_Activity_ID);
                }
            }
            else if (C_Project_ID != 0 && C_Activity_ID != 0)
            {
                budget_ID = GetBudgetAgainstProjectActivity(AccID, startDate, endDate, C_Project_ID, C_Activity_ID);
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, C_Project_ID);
                }
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, C_Activity_ID);
                }
            }
            else if (C_Project_ID != 0)
            {
                budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, C_Project_ID);
            }
            else if (C_Campaign_ID != 0)
            {
                budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, C_Campaign_ID);
            }
            else if (C_Activity_ID != 0)
            {
                budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, C_Activity_ID);
            }

            // If Budget Not Found For Any Dimension Then look for Account + Organization
            if (budget_ID == 0)
            {
                try
                {
                    sql.Clear();
                    sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                    idr = DB.ExecuteReader(sql.ToString(), null, null);
                    while (idr.Read())
                    {
                        if (B_ID == 0)
                        {
                            B_ID = Util.GetValueOfInt(idr[1]);
                        }
                        if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                        {
                            budget_ID = Util.GetValueOfInt(idr[1]);
                            bgtAmount = Util.GetValueOfDecimal(idr[0]);
                        }
                    }
                    idr.Close();
                    idr = null;
                }
                catch
                {
                    idr.Close();
                    idr = null;
                }
            }

            #region
            //while (!gotID)
            //{
            //    finalWhereClause.Clear();
            //    if (ord.GetC_Campaign_ID() != 0)
            //    {
            //        finalWhereClause.Append(" AND fa.C_Campaign_ID = " + ord.GetC_Campaign_ID());
            //    }
            //    else
            //    {
            //        finalWhereClause.Append(" AND fa.C_Campaign_ID is null ");
            //    }
            //    if (ord.GetC_Activity_ID() != 0)
            //    {
            //        finalWhereClause.Append(" AND fa.C_Activity_ID = " + ord.GetC_Activity_ID());
            //    }
            //    else
            //    {
            //        finalWhereClause.Append(" AND fa.C_Activity_ID is null ");
            //    }
            //    if (ord.GetC_Project_ID() != 0)
            //    {
            //        finalWhereClause.Append(" AND fa.C_Project_ID = " + ord.GetC_Project_ID());
            //    }
            //    else
            //    {
            //        finalWhereClause.Append(" AND fa.C_Project_ID is null ");
            //    }

            //    sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) " + whereClause + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.DateAcct DESC");
            //    // IDataReader idr = null;
            //    try
            //    {
            //        // Amount = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
            //        idr = DB.ExecuteReader(sql.ToString(), null, null);
            //        while (idr.Read())
            //        {
            //            gotID = true;
            //            if (B_ID == 0)
            //            {
            //                B_ID = Util.GetValueOfInt(idr[1]);
            //            }
            //            if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
            //            {
            //                budget_ID = Util.GetValueOfInt(idr[1]);
            //                bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
            //            }
            //        }

            //        idr.Close();
            //        idr = null;

            //        //if (budget_ID.Equals(0))
            //        //{
            //        //    sql.Clear();
            //        //    sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' AND RowNum = 1 ORDER BY fa.dateacct DESC");
            //        //    idr = DB.ExecuteReader(sql.ToString(), null, null);
            //        //    while (idr.Read())
            //        //    {
            //        //        budget_ID = Util.GetValueOfInt(idr[1]);
            //        //        bgtAmount = Util.GetValueOfDecimal(idr[0]);
            //        //        break;
            //        //    }

            //        //    idr.Close();
            //        //    idr = null;
            //        //}
            //    }
            //    catch
            //    {
            //        idr.Close();
            //        idr = null;
            //    }

            //}

            //sql.Append("SELECT GL_Budget_ID FROM GL_Budget WHERE " + GlobalVariable.TO_DATE(dateAcct, false) + " >= FromDate AND " + GlobalVariable.TO_DATE(dateAcct, false) + " <= ToDate AND IsActive = 'Y' AND AD_Client_ID = " + _AD_Client_ID);
            // sql.Append("SELECT GL_Budget_ID FROM GL_Budget WHERE AD_Client_ID = " + _AD_Client_ID + " AND BudgetControlBasis = '" + controlBasis + "'");
            // int GL_Budget_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            // sql.Clear();
            //sql.Append("SELECT GL_Budget_ID FROM GL_Budget WHERE " + GlobalVariable.TO_DATE(dateAcct, false) + " >= FromDate AND " + GlobalVariable.TO_DATE(dateAcct, false) + " <= ToDate AND IsActive = 'Y' AND AD_Client_ID = " + _AD_Client_ID);
            #endregion
            return budget_ID;
        }

        /// <summary>
        /// Get Budget Against Project And Campaign And Activity (Mandatory Organization and Account).
        /// Retruns ID of the Budget
        /// </summary>
        /// <param name="AccID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="C_Project_ID"></param>
        /// <param name="C_Campaign_ID"></param>
        /// <param name="C_Activity_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProjectCampaignActivity(int AccID, DateTime? startDate, DateTime? endDate, int C_Project_ID, int C_Campaign_ID, int C_Activity_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.C_Activity_ID = " + C_Activity_ID + " AND fa.C_Project_ID = " + C_Project_ID + " AND fa.C_Campaign_ID = " + C_Campaign_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    if (B_ID == 0)
                    {
                        B_ID = Util.GetValueOfInt(idr[1]);
                    }
                    if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                    {
                        budget_ID = Util.GetValueOfInt(idr[1]);
                        bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
                    }
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (budget_ID > 0)
            {
                whereClause.Append(" AND fa.C_Activity_ID = " + C_Activity_ID + " AND fa.C_Project_ID = " + C_Project_ID + " AND fa.C_Campaign_ID = " + C_Campaign_ID);
            }
            return budget_ID;
        }

        /// <summary>
        /// Get Budget Against Project And Campaign (Mandatory Organization and Account).
        /// Retruns ID of the Budget
        /// </summary>
        /// <param name="AccID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="C_Project_ID"></param>
        /// <param name="C_Campaign_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProjectCampaign(int AccID, DateTime? startDate, DateTime? endDate, int C_Project_ID, int C_Campaign_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.C_Project_ID = " + C_Project_ID + " AND fa.C_Campaign_ID = " + C_Campaign_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    if (B_ID == 0)
                    {
                        B_ID = Util.GetValueOfInt(idr[1]);
                    }
                    if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                    {
                        budget_ID = Util.GetValueOfInt(idr[1]);
                        bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
                    }
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (budget_ID > 0)
            {
                whereClause.Append(" AND fa.C_Project_ID = " + C_Project_ID + " AND fa.C_Campaign_ID = " + C_Campaign_ID);
            }
            return budget_ID;
        }

        /// <summary>
        /// Get Budget Against Activity And Campaign (Mandatory Organization and Account).
        /// Retruns ID of the Budget
        /// </summary>
        /// <param name="AccID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="C_Campaign_ID"></param>
        /// <param name="C_Activity_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstCampaignActivity(int AccID, DateTime? startDate, DateTime? endDate, int C_Campaign_ID, int C_Activity_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.C_Activity_ID = " + C_Activity_ID + " AND fa.C_Campaign_ID = " + C_Campaign_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    if (B_ID == 0)
                    {
                        B_ID = Util.GetValueOfInt(idr[1]);
                    }
                    if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                    {
                        budget_ID = Util.GetValueOfInt(idr[1]);
                        bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
                    }
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (budget_ID > 0)
            {
                whereClause.Append(" AND fa.C_Activity_ID = " + C_Activity_ID + " AND fa.C_Campaign_ID = " + C_Campaign_ID);
            }
            return budget_ID;
        }

        /// <summary>
        /// Get Budget Against Project And Activity (Mandatory Organization and Account).
        /// Retruns ID of the Budget 
        /// </summary>
        /// <param name="AccID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="C_Project_ID"></param>
        /// <param name="C_Activity_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProjectActivity(int AccID, DateTime? startDate, DateTime? endDate, int C_Project_ID, int C_Activity_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.C_Activity_ID = " + C_Activity_ID + " AND fa.C_Project_ID = " + C_Project_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    if (B_ID == 0)
                    {
                        B_ID = Util.GetValueOfInt(idr[1]);
                    }
                    if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                    {
                        budget_ID = Util.GetValueOfInt(idr[1]);
                        bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
                    }
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (budget_ID > 0)
            {
                whereClause.Append(" AND fa.C_Activity_ID = " + C_Activity_ID + " AND fa.C_Project_ID = " + C_Project_ID);
            }
            return budget_ID;
        }

        /// <summary>
        /// Get Budget Against Project (Mandatory Organization and Account).
        /// Retruns ID of the Budget
        /// </summary>
        /// <param name="AccID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="C_Project_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProject(int AccID, DateTime? startDate, DateTime? endDate, int C_Project_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.C_Project_ID = " + C_Project_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    if (B_ID == 0)
                    {
                        B_ID = Util.GetValueOfInt(idr[1]);
                    }
                    if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                    {
                        budget_ID = Util.GetValueOfInt(idr[1]);
                        bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
                    }
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (budget_ID > 0)
            {
                whereClause.Append(" AND fa.C_Project_ID = " + C_Project_ID);
            }
            return budget_ID;
        }

        /// <summary>
        ///  Get Budget Against Campaign (Mandatory Organization and Account).
        /// Retruns ID of the Budget
        /// </summary>
        /// <param name="AccID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="C_Campaign_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstCampaign(int AccID, DateTime? startDate, DateTime? endDate, int C_Campaign_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.C_Campaign_ID = " + C_Campaign_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    if (B_ID == 0)
                    {
                        B_ID = Util.GetValueOfInt(idr[1]);
                    }
                    if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                    {
                        budget_ID = Util.GetValueOfInt(idr[1]);
                        bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
                    }
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (budget_ID > 0)
            {
                whereClause.Append(" AND fa.C_Campaign_ID = " + C_Campaign_ID);
            }
            return budget_ID;
        }

        /// <summary>
        /// Get Budget Against Activity Campaign (Mandatory Organization and Account).
        /// Retruns ID of the Budget
        /// </summary>
        /// <param name="AccID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="C_Activity_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstActivity(int AccID, DateTime? startDate, DateTime? endDate, int C_Activity_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.GL_Budget_ID FROM Fact_Acct fa inner join GL_Budget b ON (b.GL_Budget_ID = fa.GL_Budget_ID) WHERE fa.C_Activity_ID = " + C_Activity_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.AD_Org_ID = " + _AD_Org_ID + " AND fa.AD_Client_ID = " + _AD_Client_ID + " AND fa.GL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
                idr = DB.ExecuteReader(sql.ToString(), null, null);
                while (idr.Read())
                {
                    if (B_ID == 0)
                    {
                        B_ID = Util.GetValueOfInt(idr[1]);
                    }
                    if (B_ID.Equals(Util.GetValueOfInt(idr[1])))
                    {
                        budget_ID = Util.GetValueOfInt(idr[1]);
                        bgtAmount = Decimal.Add(bgtAmount.Value, Util.GetValueOfDecimal(idr[0]));
                    }
                }
                idr.Close();
                idr = null;
            }
            catch
            {
                idr.Close();
                idr = null;
            }
            if (budget_ID > 0)
            {
                whereClause.Append(" AND fa.C_Activity_ID = " + C_Activity_ID);
            }
            return budget_ID;
        }

        private void GetWhereClause(VAdvantage.Model.MOrder ord)
        {
            //  StringBuilder where = new StringBuilder();
            //if (ord.GetC_BPartner_ID() != 0)
            //{
            //    whereClause.Append(" AND fa.C_BPartner_ID = " + ord.GetC_BPartner_ID());
            //}
            //else
            //{
            //    whereClause.Append(" AND fa.C_BPartner_ID is null ");
            //}
            if (ord.GetC_Campaign_ID() != 0)
            {
                whereClause.Append(" AND fa.C_Campaign_ID = " + ord.GetC_Campaign_ID());
            }
            else
            {
                whereClause.Append(" AND fa.C_Campaign_ID is null ");
            }
            if (ord.GetC_Activity_ID() != 0)
            {
                whereClause.Append(" AND fa.C_Activity_ID = " + ord.GetC_Activity_ID());
            }
            else
            {
                whereClause.Append(" AND fa.C_Activity_ID is null ");
            }
            if (ord.GetC_Project_ID() != 0)
            {
                whereClause.Append(" AND fa.C_Project_ID = " + ord.GetC_Project_ID());
            }
            else
            {
                whereClause.Append(" AND fa.C_Project_ID is null ");
            }
            //if (ord.GetUser1_ID() != 0)
            //{
            //    whereClause.Append(" AND fa.User1_ID = " + ord.GetUser1_ID());
            //}
            //else
            //{
            //    whereClause.Append(" AND fa.User1_ID is null ");
            //}
            //if (ord.GetUser2_ID() != 0)
            //{
            //    whereClause.Append(" AND fa.User2_ID = " + ord.GetUser2_ID());
            //}
            //else
            //{
            //    whereClause.Append(" AND fa.User2_ID is null ");
            //}
            //if (ord.GetAD_OrgTrx_ID() != 0)
            //{
            //    whereClause.Append(" AND fa.AD_OrgTrx_ID = " + ord.GetAD_OrgTrx_ID());
            //}
            //else
            //{
            //    whereClause.Append(" AND fa.AD_OrgTrx_ID is null ");
            //}
            //if (ord.GetC_BPartner_ID() != 0)
            //{
            //    where.Append("C_BPartner_ID = " + ord.GetC_BPartner_ID());
            //}
        }

        private int GetAccount(int M_Product_ID, int C_Charge_ID)
        {
            sql.Clear();
            sql.Append("SELECT C_AcctSchema_ID FROM C_AcctSchema WHERE AD_OrgOnly_ID = " + _AD_Org_ID);
            C_AcctSchema_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            sql.Clear();
            if (C_AcctSchema_ID.Equals(0))
            {
                sql.Append("SELECT C_AcctSchema1_ID FROM AD_ClientInfo WHERE AD_Client_ID = " + _AD_Client_ID);
                C_AcctSchema_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            }
            sql.Clear();
            if (M_Product_ID > 0)
            {
                sql.Append("SELECT Account_ID FROM C_ValidCombination WHERE C_ValidCombination_ID = (SELECT P_Expense_Acct FROM M_Product_Acct WHERE C_AcctSchema_ID = " + C_AcctSchema_ID + " AND M_Product_ID = " + M_Product_ID + ")");
            }
            else
            {
                sql.Append("SELECT Account_ID FROM C_ValidCombination WHERE C_ValidCombination_ID = (SELECT ch_expense_acct FROM C_Charge_Acct WHERE C_AcctSchema_ID = " + C_AcctSchema_ID + " AND C_Charge_ID = " + C_Charge_ID + ")");
            }
            int Account_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            return Account_ID;
        }

        public string GetTableName()
        {
            sql = new StringBuilder("SELECT TableName FROM AD_Table WHERE AD_Table_ID = " + _AD_Table_ID);
            tableName = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, null));
            sql.Clear();
            return tableName;
        }

        public int AD_Table_ID
        {
            get
            {
                return _AD_Table_ID;
            }
            set
            {
                _AD_Table_ID = value;
            }
        }

        public int Record_ID
        {
            get
            {
                return _Record_ID;
            }
            set
            {
                _Record_ID = value;
            }
        }

        public int AD_Client_ID
        {
            get
            {
                return _AD_Client_ID;
            }
            set
            {
                _AD_Client_ID = value;
            }
        }

        public int AD_Org_ID
        {
            get
            {
                return _AD_Org_ID;
            }
            set
            {
                _AD_Org_ID = value;
            }
        }

        public Trx TrxName
        {
            get
            {
                return _trx;
            }
            set
            {
                _trx= value;
            }
        }

        public String DocAction
        {
            get
            {
                return _docAction;
            }
            set
            {
                _docAction = value;
            }
        }

        public Ctx GetCtx()
        {
            return _ctx;
        }
    }
}
