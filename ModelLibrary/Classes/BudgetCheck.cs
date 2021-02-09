using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Model;

namespace ModelLibrary.Classes
{
    public class BudgetCheck
    {
        #region Private Variables
        StringBuilder sql = null;
        private string tableName = "";
        private int _VAF_TableView_ID = 0;
        private int _Record_ID = 0;
        //private string _trxName = null;
        private Trx _trx = null;
        private int _VAF_Client_ID = 0;
        private int _VAF_Org_ID = 0;
        private StringBuilder whereClause = new StringBuilder();
        private string controlBasis = "";
        private int VAB_AccountBook_ID = 0;
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
            if (_VAF_TableView_ID > 0)
            {
                GetTableName();

                if (_docAction.Equals("--"))
                {
                    if (_VAF_TableView_ID.Equals(259))
                    {
                        sql.Clear();
                        sql.Append("UPDATE " + tableName + " SET IsBudgetViolated = 'N', MaxBudgetViolationAmount = 0 WHERE " + tableName + "_ID = " + _Record_ID);
                        int upd = DB.ExecuteQuery(sql.ToString(), null, _trx);

                        sql.Clear();
                        sql.Append("UPDATE VAB_OrderLine SET BudgetViolationAmount = 0 WHERE " + tableName + "_ID = " + _Record_ID);
                        upd = DB.ExecuteQuery(sql.ToString(), null, _trx);
                    }
                    return "";
                }

                sql.Clear();
                try
                {
                    sql.Append("SELECT BudgetControlBasis FROM VAF_Org WHERE VAF_Org_ID = " + _VAF_Org_ID);

                    controlBasis = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, null));
                }
                catch
                {

                }
                if (controlBasis != "")
                {
                    sql.Clear();
                    sql.Append(@" SELECT DocBaseType, Name, IsSOTrx, IsReturnTrx FROM VAB_DocTypes WHERE VAB_DocTypes_ID = (SELECT VAB_DocTypesTarget_ID FROM " + tableName + " WHERE " + tableName + "_ID = " + _Record_ID + ")");
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
            int VAM_Product_ID = 0;
            DateTime? sDate = null;
            DateTime? eDate = null;
            if (VAdvantage.Model.MDocBaseType.DOCBASETYPE_PURCHASEORDER.Equals(docBaseType))
            {
                VAdvantage.Model.MVABOrder ord = new VAdvantage.Model.MVABOrder(GetCtx(), _Record_ID, _trx);
                if (controlBasis.Equals("A"))
                {
                    sql.Clear();
                    sql.Append(@" SELECT PeriodNo, StartDate , EndDate FROM VAB_YearPeriod WHERE VAB_Year_ID = (SELECT cy.VAB_Year_ID FROM VAB_YearPeriod cp inner join  VAB_Year cy
     ON (cy.VAB_Year_id = cp.VAB_Year_id) WHERE cy.IsActive = 'Y' AND cy.VAF_Client_ID = " + _VAF_Client_ID + @" AND StartDate   <= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @"
  AND EndDate     >= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @"
  AND cy.VAB_Calender_id = (SELECT VAB_Calender_ID FROM VAF_ClientDetail WHERE VAF_ClientDetail.vaf_client_id = " + _VAF_Client_ID + @")) AND PeriodNo IN (1,12)");
                    // sql.Append("SELECT  PeriodNo, StartDate, EndDate FROM VAB_YearPeriod WHERE VAB_Year_ID = (SELECT VAB_Year_ID FROM VAB_YearPeriod WHERE IsActive = 'Y' AND VAF_Client_ID = " + _VAF_Client_ID + " AND StartDate <= " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + " AND EndDate >= " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + ") AND PeriodNo IN (1,12)");
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
                    sql.Append(@" SELECT PeriodNo, StartDate , EndDate FROM VAB_YearPeriod cp INNER JOIN VAB_Year cy ON (cy.VAB_Year_id = cp.VAB_Year_id) WHERE cy.IsActive = 'Y'
AND cy.VAF_Client_ID = " + _VAF_Client_ID + @" AND StartDate   <= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @"
AND EndDate     >= " + GlobalVariable.TO_DATE(ord.GetDateOrdered(), true) + @" AND cy.VAB_Calender_id  = (SELECT VAB_Calender_ID FROM VAF_ClientDetail
      WHERE VAF_ClientDetail.vaf_client_id = " + _VAF_Client_ID + @")");

                    //sql.Append("SELECT  PeriodNo, StartDate, EndDate FROM VAB_YearPeriod WHERE IsActive = 'Y' AND VAF_Client_ID = " + _VAF_Client_ID + " AND StartDate <= " + GlobalVariable.TO_DATE(System.DateTime.Now, false) + " AND EndDate >= " + GlobalVariable.TO_DATE(System.DateTime.Now, false));
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
                sql.Append("SELECT * FROM VAB_OrderLine WHERE VAB_Order_ID = " + _Record_ID + " AND IsActive ='Y'");
                DataSet ds = DB.ExecuteDataset(sql.ToString(), null, null);
                if (ds != null)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        whereClause.Clear();

                        // Amount = 0;

                        VAdvantage.Model.MVABOrderLine ol = new VAdvantage.Model.MVABOrderLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_OrderLine_ID"]), _trx);
                        VAM_Product_ID = ol.GetVAM_Product_ID();
                        int Account_ID = 0;
                        if (VAM_Product_ID > 0)
                        {
                            Account_ID = GetAccount(VAM_Product_ID, 0);
                            whereClause.Append(" WHERE fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.Account_ID = " + Account_ID + " AND fa.VAF_Org_ID = " + _VAF_Org_ID);
                        }
                        else if (ol.GetVAB_Charge_ID() > 0)
                        {
                            Account_ID = GetAccount(0, ol.GetVAB_Charge_ID());
                            whereClause.Append(" WHERE fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.Account_ID = " + Account_ID + " AND fa.VAF_Org_ID = " + _VAF_Org_ID);
                        }
                        whereClause.Append(" AND fa.DateAcct >= " + GlobalVariable.TO_DATE(sDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(eDate, false));
                        //  GetBudgetAgainstDimension(ord);

                        if (Account_ID > 0)
                        {
                            Decimal? lineAmount = GetLineAmount(ord.GetVAB_Currency_ID(), ol.GetLineNetAmt());
                            int BudgetID = 0;
                            //if (ord.GetVAB_Project_ID() > 0)
                            //{
                            //    BudgetID = GetProjectBudget(ord.GetVAB_Project_ID());
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
                                    //sql.Append("UPDATE VAB_OrderLine SET BudgetViolationAmount = " + amt + " WHERE VAB_OrderLine_ID = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_OrderLine_ID"]));
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

        private Decimal? GetLineAmount(int VAB_Currency_ID, Decimal? lineAmt)
        {
            Decimal? amt = 0;
            sql.Clear();
            sql.Append("SELECT VAB_Currency_ID FROM VAB_AccountBook WHERE VAB_AccountBook_ID = (SELECT VAB_AccountBook1_ID FROM VAF_ClientDetail WHERE VAF_Client_ID = " + _VAF_Client_ID + ")");
            int accCurrency = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            if (VAB_Currency_ID.Equals(accCurrency))
            {
                return lineAmt;
            }
            else
            {
                amt = VAdvantage.Model.MVABExchangeRate.Convert(GetCtx(), lineAmt.Value, VAB_Currency_ID, accCurrency, _VAF_Client_ID, _VAF_Org_ID);
                return amt;
            }
        }

        private Decimal? CompareBudget(Decimal? lineAmt, VAdvantage.Model.MVABOrder order)
        {
            Decimal? amt = 0;
            sql.Clear();
            sql.Append(" SELECT SUM(fa.AmtAcctDr) FROM Actual_Acct_Detail fa " + whereClause);
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
            sql.Append("SELECT CommitmentType FROM VAGL_BudgetActivation WHERE VAGL_Budget_ID = " + BudgetID + " AND IsActive = 'Y'");
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
            sql.Append("SELECT VAGL_Budget_ID FROM VAF_Org WHERE VAF_Org_ID = " + _VAF_Org_ID);
            return Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
        }

        private int GetProjectBudget(int Project_ID)
        {
            sql.Clear();
            sql.Append("SELECT VAGL_Budget_ID FROM VAB_Project WHERE VAB_Project_ID = " + Project_ID);
            return Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
        }

        //
        private int GetBudgetAmount(int AccID, DateTime? startDate, DateTime? endDate, VAdvantage.Model.MVABOrder ord)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            sql.Clear();
            int VAB_Project_ID = Util.GetValueOfInt(ord.GetVAB_Project_ID());
            int VAB_Promotion_ID = Util.GetValueOfInt(ord.GetVAB_Promotion_ID());
            int VAB_BillingCode_ID = Util.GetValueOfInt(ord.GetVAB_BillingCode_ID());
            IDataReader idr = null;

            if (VAB_Project_ID != 0 && VAB_Promotion_ID != 0 && VAB_BillingCode_ID != 0)
            {
                // find Budget against all three Parameters
                budget_ID = GetBudgetAgainstProjectCampaignActivity(AccID, startDate, endDate, VAB_Project_ID, VAB_Promotion_ID, VAB_BillingCode_ID);
                // if not found then find against Project and Campaign
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProjectCampaign(AccID, startDate, endDate, VAB_Project_ID, VAB_Promotion_ID);
                }
                // Then Against Project and Activity
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProjectActivity(AccID, startDate, endDate, VAB_Project_ID, VAB_BillingCode_ID);
                }
                // Then Against Campaign and Activity
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaignActivity(AccID, startDate, endDate, VAB_BillingCode_ID, VAB_Promotion_ID);
                }
                // Against Project
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, VAB_Project_ID);
                }
                // Against Project
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, VAB_Promotion_ID);
                }
                // Against Activity
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, VAB_BillingCode_ID);
                }
            }
            else if (VAB_Project_ID != 0 && VAB_Promotion_ID != 0)
            {
                budget_ID = GetBudgetAgainstProjectCampaign(AccID, startDate, endDate, VAB_Project_ID, VAB_Promotion_ID);
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, VAB_Project_ID);
                }
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, VAB_Promotion_ID);
                }
            }
            else if (VAB_Promotion_ID != 0 && VAB_BillingCode_ID != 0)
            {
                budget_ID = GetBudgetAgainstCampaignActivity(AccID, startDate, endDate, VAB_Promotion_ID, VAB_BillingCode_ID);
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, VAB_Promotion_ID);
                }
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, VAB_BillingCode_ID);
                }
            }
            else if (VAB_Project_ID != 0 && VAB_BillingCode_ID != 0)
            {
                budget_ID = GetBudgetAgainstProjectActivity(AccID, startDate, endDate, VAB_Project_ID, VAB_BillingCode_ID);
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, VAB_Project_ID);
                }
                if (budget_ID == 0)
                {
                    budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, VAB_BillingCode_ID);
                }
            }
            else if (VAB_Project_ID != 0)
            {
                budget_ID = GetBudgetAgainstProject(AccID, startDate, endDate, VAB_Project_ID);
            }
            else if (VAB_Promotion_ID != 0)
            {
                budget_ID = GetBudgetAgainstCampaign(AccID, startDate, endDate, VAB_Promotion_ID);
            }
            else if (VAB_BillingCode_ID != 0)
            {
                budget_ID = GetBudgetAgainstActivity(AccID, startDate, endDate, VAB_BillingCode_ID);
            }

            // If Budget Not Found For Any Dimension Then look for Account + Organization
            if (budget_ID == 0)
            {
                try
                {
                    sql.Clear();
                    sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
            //    if (ord.GetVAB_Promotion_ID() != 0)
            //    {
            //        finalWhereClause.Append(" AND fa.VAB_Promotion_ID = " + ord.GetVAB_Promotion_ID());
            //    }
            //    else
            //    {
            //        finalWhereClause.Append(" AND fa.VAB_Promotion_ID is null ");
            //    }
            //    if (ord.GetVAB_BillingCode_ID() != 0)
            //    {
            //        finalWhereClause.Append(" AND fa.VAB_BillingCode_ID = " + ord.GetVAB_BillingCode_ID());
            //    }
            //    else
            //    {
            //        finalWhereClause.Append(" AND fa.VAB_BillingCode_ID is null ");
            //    }
            //    if (ord.GetVAB_Project_ID() != 0)
            //    {
            //        finalWhereClause.Append(" AND fa.VAB_Project_ID = " + ord.GetVAB_Project_ID());
            //    }
            //    else
            //    {
            //        finalWhereClause.Append(" AND fa.VAB_Project_ID is null ");
            //    }

            //    sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) " + whereClause + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.DateAcct DESC");
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
            //        //    sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' AND RowNum = 1 ORDER BY fa.dateacct DESC");
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

            //sql.Append("SELECT VAGL_Budget_ID FROM VAGL_Budget WHERE " + GlobalVariable.TO_DATE(dateAcct, false) + " >= FromDate AND " + GlobalVariable.TO_DATE(dateAcct, false) + " <= ToDate AND IsActive = 'Y' AND VAF_Client_ID = " + _VAF_Client_ID);
            // sql.Append("SELECT VAGL_Budget_ID FROM VAGL_Budget WHERE VAF_Client_ID = " + _VAF_Client_ID + " AND BudgetControlBasis = '" + controlBasis + "'");
            // int VAGL_Budget_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            // sql.Clear();
            //sql.Append("SELECT VAGL_Budget_ID FROM VAGL_Budget WHERE " + GlobalVariable.TO_DATE(dateAcct, false) + " >= FromDate AND " + GlobalVariable.TO_DATE(dateAcct, false) + " <= ToDate AND IsActive = 'Y' AND VAF_Client_ID = " + _VAF_Client_ID);
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
        /// <param name="VAB_Project_ID"></param>
        /// <param name="VAB_Promotion_ID"></param>
        /// <param name="VAB_BillingCode_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProjectCampaignActivity(int AccID, DateTime? startDate, DateTime? endDate, int VAB_Project_ID, int VAB_Promotion_ID, int VAB_BillingCode_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID + " AND fa.VAB_Project_ID = " + VAB_Project_ID + " AND fa.VAB_Promotion_ID = " + VAB_Promotion_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
                whereClause.Append(" AND fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID + " AND fa.VAB_Project_ID = " + VAB_Project_ID + " AND fa.VAB_Promotion_ID = " + VAB_Promotion_ID);
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
        /// <param name="VAB_Project_ID"></param>
        /// <param name="VAB_Promotion_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProjectCampaign(int AccID, DateTime? startDate, DateTime? endDate, int VAB_Project_ID, int VAB_Promotion_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.VAB_Project_ID = " + VAB_Project_ID + " AND fa.VAB_Promotion_ID = " + VAB_Promotion_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
                whereClause.Append(" AND fa.VAB_Project_ID = " + VAB_Project_ID + " AND fa.VAB_Promotion_ID = " + VAB_Promotion_ID);
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
        /// <param name="VAB_Promotion_ID"></param>
        /// <param name="VAB_BillingCode_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstCampaignActivity(int AccID, DateTime? startDate, DateTime? endDate, int VAB_Promotion_ID, int VAB_BillingCode_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID + " AND fa.VAB_Promotion_ID = " + VAB_Promotion_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
                whereClause.Append(" AND fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID + " AND fa.VAB_Promotion_ID = " + VAB_Promotion_ID);
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
        /// <param name="VAB_Project_ID"></param>
        /// <param name="VAB_BillingCode_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProjectActivity(int AccID, DateTime? startDate, DateTime? endDate, int VAB_Project_ID, int VAB_BillingCode_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID + " AND fa.VAB_Project_ID = " + VAB_Project_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
                whereClause.Append(" AND fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID + " AND fa.VAB_Project_ID = " + VAB_Project_ID);
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
        /// <param name="VAB_Project_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstProject(int AccID, DateTime? startDate, DateTime? endDate, int VAB_Project_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.VAB_Project_ID = " + VAB_Project_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
                whereClause.Append(" AND fa.VAB_Project_ID = " + VAB_Project_ID);
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
        /// <param name="VAB_Promotion_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstCampaign(int AccID, DateTime? startDate, DateTime? endDate, int VAB_Promotion_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.VAB_Promotion_ID = " + VAB_Promotion_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
                whereClause.Append(" AND fa.VAB_Promotion_ID = " + VAB_Promotion_ID);
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
        /// <param name="VAB_BillingCode_ID"></param>
        /// <returns></returns>
        private int GetBudgetAgainstActivity(int AccID, DateTime? startDate, DateTime? endDate, int VAB_BillingCode_ID)
        {
            int B_ID = 0;
            int budget_ID = 0;
            bgtAmount = 0;
            IDataReader idr = null;
            try
            {
                sql.Clear();
                sql.Append("SELECT fa.AmtAcctDr, b.VAGL_Budget_ID FROM Actual_Acct_Detail fa inner join VAGL_Budget b ON (b.VAGL_Budget_ID = fa.VAGL_Budget_ID) WHERE fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID + " AND fa.Account_ID = " + AccID + " AND fa.DateAcct >= " + GlobalVariable.TO_DATE(startDate, false) + " AND fa.DateAcct <= " + GlobalVariable.TO_DATE(endDate, false) + " AND fa.VAF_Org_ID = " + _VAF_Org_ID + " AND fa.VAF_Client_ID = " + _VAF_Client_ID + " AND fa.VAGL_Budget_ID IS Not null AND b.BudgetControlBasis = '" + controlBasis + "' ORDER BY fa.dateacct DESC");
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
                whereClause.Append(" AND fa.VAB_BillingCode_ID = " + VAB_BillingCode_ID);
            }
            return budget_ID;
        }

        private void GetWhereClause(VAdvantage.Model.MVABOrder ord)
        {
            //  StringBuilder where = new StringBuilder();
            //if (ord.GetVAB_BusinessPartner_ID() != 0)
            //{
            //    whereClause.Append(" AND fa.VAB_BusinessPartner_ID = " + ord.GetVAB_BusinessPartner_ID());
            //}
            //else
            //{
            //    whereClause.Append(" AND fa.VAB_BusinessPartner_ID is null ");
            //}
            if (ord.GetVAB_Promotion_ID() != 0)
            {
                whereClause.Append(" AND fa.VAB_Promotion_ID = " + ord.GetVAB_Promotion_ID());
            }
            else
            {
                whereClause.Append(" AND fa.VAB_Promotion_ID is null ");
            }
            if (ord.GetVAB_BillingCode_ID() != 0)
            {
                whereClause.Append(" AND fa.VAB_BillingCode_ID = " + ord.GetVAB_BillingCode_ID());
            }
            else
            {
                whereClause.Append(" AND fa.VAB_BillingCode_ID is null ");
            }
            if (ord.GetVAB_Project_ID() != 0)
            {
                whereClause.Append(" AND fa.VAB_Project_ID = " + ord.GetVAB_Project_ID());
            }
            else
            {
                whereClause.Append(" AND fa.VAB_Project_ID is null ");
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
            //if (ord.GetVAF_OrgTrx_ID() != 0)
            //{
            //    whereClause.Append(" AND fa.VAF_OrgTrx_ID = " + ord.GetVAF_OrgTrx_ID());
            //}
            //else
            //{
            //    whereClause.Append(" AND fa.VAF_OrgTrx_ID is null ");
            //}
            //if (ord.GetVAB_BusinessPartner_ID() != 0)
            //{
            //    where.Append("VAB_BusinessPartner_ID = " + ord.GetVAB_BusinessPartner_ID());
            //}
        }

        private int GetAccount(int VAM_Product_ID, int VAB_Charge_ID)
        {
            sql.Clear();
            sql.Append("SELECT VAB_AccountBook_ID FROM VAB_AccountBook WHERE VAF_OrgOnly_ID = " + _VAF_Org_ID);
            VAB_AccountBook_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            sql.Clear();
            if (VAB_AccountBook_ID.Equals(0))
            {
                sql.Append("SELECT VAB_AccountBook1_ID FROM VAF_ClientDetail WHERE VAF_Client_ID = " + _VAF_Client_ID);
                VAB_AccountBook_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            }
            sql.Clear();
            if (VAM_Product_ID > 0)
            {
                sql.Append("SELECT Account_ID FROM VAB_Acct_ValidParameter WHERE VAB_Acct_ValidParameter_ID = (SELECT P_Expense_Acct FROM VAM_Product_Acct WHERE VAB_AccountBook_ID = " + VAB_AccountBook_ID + " AND VAM_Product_ID = " + VAM_Product_ID + ")");
            }
            else
            {
                sql.Append("SELECT Account_ID FROM VAB_Acct_ValidParameter WHERE VAB_Acct_ValidParameter_ID = (SELECT ch_expense_acct FROM VAB_Charge_Acct WHERE VAB_AccountBook_ID = " + VAB_AccountBook_ID + " AND VAB_Charge_ID = " + VAB_Charge_ID + ")");
            }
            int Account_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            return Account_ID;
        }

        public string GetTableName()
        {
            sql = new StringBuilder("SELECT TableName FROM VAF_TableView WHERE VAF_TableView_ID = " + _VAF_TableView_ID);
            tableName = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, null));
            sql.Clear();
            return tableName;
        }

        public int VAF_TableView_ID
        {
            get
            {
                return _VAF_TableView_ID;
            }
            set
            {
                _VAF_TableView_ID = value;
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

        public int VAF_Client_ID
        {
            get
            {
                return _VAF_Client_ID;
            }
            set
            {
                _VAF_Client_ID = value;
            }
        }

        public int VAF_Org_ID
        {
            get
            {
                return _VAF_Org_ID;
            }
            set
            {
                _VAF_Org_ID = value;
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
                _trx = value;
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

        /// <summary>
        /// This Function is used to get Dimension applicable on which Budget
        /// </summary>
        /// <param name="BudgetControlIds">Budget Control Ids</param>
        /// <returns>dataset of Dimension applicable</returns>
        public DataSet GetBudgetDimension(String BudgetControlIds)
        {
            DataSet dsBudgetControl = null;
            String sql = @"SELECT DISTINCT VAGL_BudgetActivation.VAGL_Budget_ID, VAGL_BudgetActivation.VAGL_BudgetActivation_ID, VAGL_BudgetActivationDim.ElementType, 
                            /* CASE   WHEN VAGL_BudgetActivationDim.ElementType = 'OO' THEN 'VAF_Org_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'BP' THEN 'VAB_BusinessPartner_ID' 
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'AY' THEN 'VAB_BillingCode_ID'
                                   WHEN (VAGL_BudgetActivationDim.ElementType = 'LF' OR VAGL_BudgetActivationDim.ElementType = 'LT') THEN 'VAB_Address_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'MC' THEN 'VAB_Promotion_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'OT' THEN 'VAF_OrgTrx_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'PJ' THEN 'VAB_Project_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'PR' THEN 'VAM_Product_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'SR' THEN 'VAB_SalesRegionState_ID'
                                ELSE CAST(VAF_Column.ColumnName AS varchar(100)) END AS ColumnName, */
                                 CASE  WHEN VAGL_BudgetActivationDim.ElementType = 'OO' THEN 'VAF_Org_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'BP' THEN 'VAB_BusinessPartner_ID' 
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'AY' THEN 'VAB_BillingCode_ID'
                                   WHEN (VAGL_BudgetActivationDim.ElementType = 'LF' OR VAGL_BudgetActivationDim.ElementType = 'LT') THEN 'VAB_Address_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'MC' THEN 'VAB_Promotion_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'OT' THEN 'VAF_OrgTrx_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'PJ' THEN 'VAB_Project_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'PR' THEN 'VAM_Product_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'SR' THEN 'VAB_SalesRegionState_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X1' THEN 'UserElement1_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X2' THEN 'UserElement2_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X3' THEN 'UserElement3_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X4' THEN 'UserElement4_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X5' THEN 'UserElement5_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X6' THEN 'UserElement6_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X7' THEN 'UserElement7_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X8' THEN 'UserElement8_ID'
                                   WHEN VAGL_BudgetActivationDim.ElementType = 'X9' THEN 'UserElement9_ID' END AS ElementName
                           FROM VAGL_BudgetActivation INNER JOIN VAGL_BudgetActivationDim ON VAGL_BudgetActivationDim.VAGL_BudgetActivation_ID = VAGL_BudgetActivation.VAGL_BudgetActivation_ID 
                           INNER JOIN VAB_AccountBook_Element ON (VAGL_BudgetActivation.VAB_AccountBook_ID = VAB_AccountBook_Element.VAB_AccountBook_ID AND
                           VAB_AccountBook_Element.ElementType = VAGL_BudgetActivationDim.ElementType) 
                           LEFT JOIN VAF_Column ON VAF_Column.VAF_Column_ID = VAB_AccountBook_Element.VAF_Column_ID 
                           WHERE VAGL_BudgetActivation.IsActive = 'Y' AND VAGL_BudgetActivationDim.IsActive = 'Y' AND VAB_AccountBook_Element.IsActive = 'Y' 
                                 AND VAGL_BudgetActivation.VAGL_BudgetActivation_ID IN (" + BudgetControlIds + ")";
            dsBudgetControl = DB.ExecuteDataset(sql, null, null);
            return dsBudgetControl;
        }

        /// <summary>
        /// Get Controlled Value against any Budget and Ledger and respective dimension
        /// </summary>
        /// <param name="drDataRecord">posted record</param>
        /// <param name="drBUdgetControl">budget control</param>
        /// <param name="drBudgetComtrolDimension">control dimensin rows</param>
        /// <param name="date">Account Date</param>
        /// <param name="_listBudgetControl">list of budget control</param>
        /// <param name="trxName">trx</param>
        /// <param name="order">isOrder</param>
        /// <param name="OrderId">Order ID</param>
        /// <returns>listBudgetControl</returns>
        public List<BudgetControl> GetBudgetControlValue(DataRow drDataRecord, DataRow drBUdgetControl, DataRow[] drBudgetComtrolDimension,
            DateTime? date, List<BudgetControl> _listBudgetControl, Trx trxName, char order, int OrderId)
        {
            BudgetControl budgetControl = null;
            String groupBy = " Account_ID, VAB_AccountBook_ID ";
            String whereDimension = "";

            String Where = " Account_ID = " + Util.GetValueOfInt(drDataRecord["Account_ID"])
                   + " AND VAB_AccountBook_ID = " + Util.GetValueOfInt(drBUdgetControl["VAB_AccountBook_ID"]);
            if (Util.GetValueOfString(drBUdgetControl["BudgetControlScope"]).Equals(X_VAGL_BudgetActivation.BUDGETCONTROLSCOPE_PeriodOnly))
            {
                Where += " AND VAB_YearPeriod_ID = " + Util.GetValueOfInt(drBUdgetControl["VAB_YearPeriod_ID"]);
            }
            else if (Util.GetValueOfString(drBUdgetControl["BudgetControlScope"]).Equals(X_VAGL_BudgetActivation.BUDGETCONTROLSCOPE_Total))
            {
                Where += " AND VAB_YearPeriod_ID IN (SELECT VAB_YearPeriod_ID FROM VAB_YearPeriod WHERE IsActive = 'Y' AND VAB_Year_ID =  " + Util.GetValueOfInt(drBUdgetControl["VAB_Year_ID"]) + " ) ";
            }
            else if (Util.GetValueOfString(drBUdgetControl["BudgetControlScope"]).Equals(X_VAGL_BudgetActivation.BUDGETCONTROLSCOPE_YearToDate))
            {
                Where += @" AND DateAcct Between (SELECT StartDate FROM VAB_YearPeriod WHERE PeriodNo = 1 AND IsActive = 'Y' 
                            AND VAB_Year_ID =  " + Util.GetValueOfInt(drBUdgetControl["VAB_Year_ID"]) + " )  AND " + GlobalVariable.TO_DATE(date, true);
            }

            String sql = @"SELECT SUM(AmtAcctDr) AS ControlledAmount, ";
            if (drBudgetComtrolDimension != null)
            {
                for (int i = 0; i < drBudgetComtrolDimension.Length; i++)
                {
                    groupBy += ", " + Util.GetValueOfString(drBudgetComtrolDimension[i]["ElementName"]);
                    whereDimension += " AND " + Util.GetValueOfString(drBudgetComtrolDimension[i]["ElementName"]) + "="
                                     + Util.GetValueOfInt(drDataRecord[Util.GetValueOfString(drBudgetComtrolDimension[i]["ElementName"])]);
                    //selectedDimension.Add(Util.GetValueOfString(drBudgetComtrolDimension[i]["ElementType"]));
                }
            }
            //Where += GetNonSelectedDimension(selectedDimension);

            // check budget already checked or not
            if (_listBudgetControl.Exists(x => (x.VAGL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"])) &&
                                               (x.VAGL_BudgetActivation_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"])) &&
                                               (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"]))))
            {
                return _listBudgetControl;
            }

            sql += groupBy + " FROM Actual_Acct_Detail WHERE VAGL_Budget_ID = " + Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"]) + " AND " + Where + " GROUP BY " + groupBy;
            DataSet dsBudgetControlAmount = DB.ExecuteDataset(sql, null, null);
            if (dsBudgetControlAmount != null && dsBudgetControlAmount.Tables.Count > 0)
            {
                for (int i = 0; i < dsBudgetControlAmount.Tables[0].Rows.Count; i++)
                {
                    budgetControl = new BudgetControl();
                    budgetControl.VAGL_Budget_ID = Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"]);
                    budgetControl.VAGL_BudgetActivation_ID = Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"]);
                    budgetControl.VAB_AccountBook_ID = Util.GetValueOfInt(drBUdgetControl["VAB_AccountBook_ID"]);
                    budgetControl.Account_ID = Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["Account_id"]);
                    budgetControl.VAF_Org_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAF_Org_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAF_Org_ID"]) : 0;
                    budgetControl.VAM_Product_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAM_Product_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAM_Product_ID"]) : 0;
                    budgetControl.VAB_BusinessPartner_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAB_BusinessPartner_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAB_BusinessPartner_ID"]) : 0;
                    budgetControl.VAB_BillingCode_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAB_BillingCode_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAB_BillingCode_ID"]) : 0;
                    budgetControl.VAB_AddressFrom_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_LocFrom_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_LocFrom_ID"]) : 0;
                    budgetControl.VAB_AddressTo_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_LocTo_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_LocTo_ID"]) : 0;
                    budgetControl.VAB_Promotion_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAB_Promotion_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAB_Promotion_ID"]) : 0;
                    budgetControl.VAF_OrgTrx_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAF_OrgTrx_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAF_OrgTrx_ID"]) : 0;
                    budgetControl.VAB_Project_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAB_Project_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAB_Project_ID"]) : 0;
                    budgetControl.VAB_SalesRegionState_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("VAB_SalesRegionState_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["VAB_SalesRegionState_ID"]) : 0;
                    budgetControl.UserList1_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserList1_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserList1_ID"]) : 0;
                    budgetControl.UserList2_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserList2_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserList2_ID"]) : 0;
                    budgetControl.UserElement1_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement1_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement1_ID"]) : 0;
                    budgetControl.UserElement2_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement2_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement2_ID"]) : 0;
                    budgetControl.UserElement3_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement3_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement3_ID"]) : 0;
                    budgetControl.UserElement4_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement4_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement4_ID"]) : 0;
                    budgetControl.UserElement5_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement5_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement5_ID"]) : 0;
                    budgetControl.UserElement6_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement6_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement6_ID"]) : 0;
                    budgetControl.UserElement7_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement7_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement7_ID"]) : 0;
                    budgetControl.UserElement8_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement8_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement8_ID"]) : 0;
                    budgetControl.UserElement9_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("UserElement9_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["UserElement9_ID"]) : 0;
                    budgetControl.ControlledAmount = dsBudgetControlAmount.Tables[0].Columns.Contains("ControlledAmount") ? Util.GetValueOfDecimal(dsBudgetControlAmount.Tables[0].Rows[i]["ControlledAmount"]) : 0;
                    budgetControl.WhereClause = Where + whereDimension;
                    _listBudgetControl.Add(budgetControl);
                }

                //for (int i = 0; i < dsBudgetControlAmount.Tables[0].Rows.Count; i++)
                //{
                //    CheckOrCreateDefault(Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"]),
                //    Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"]), Util.GetValueOfInt(drBUdgetControl["VAB_AccountBook_ID"]),
                //    Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["Account_id"]));
                //}

            }

            // Reduce Amount (Budget - Actual - Commitment - Reservation) from respective budget, AlreadyControlledAmount = Actual + Commitment + Reservation
            String query = "SELECT SUM(AmtAcctDr - AmtAcctCr) AS AlreadyControlledAmount FROM Actual_Acct_Detail WHERE PostingType IN ('A' , 'E', 'R') AND " + Where + whereDimension;
            Decimal AlreadyControlledAmount = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, trxName));


            if (order.Equals('O') && Util.GetValueOfString(drBUdgetControl["CommitmentType"]).Equals(X_VAGL_BudgetActivation.COMMITMENTTYPE_CommitmentReservation))
            {
                query = @"SELECT DISTINCT VAM_Requisition_ID AS Col1 FROM VAM_RequisitionLine WHERE VAB_OrderLine_ID IN ( 
                            SELECT DISTINCT VAB_OrderLine_ID FROM VAB_OrderLine WHERE VAM_RequisitionLine_ID > 0 AND  VAB_Order_ID = " + OrderId + ")";
                DataSet ds = DB.ExecuteDataset(query, null, trxName);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object[] requisitionIds = ds.Tables[0].AsEnumerable().Select(r => r.Field<object>("COL1")).ToArray();
                    string result = string.Join(",", requisitionIds);

                    query = "SELECT SUM(AmtAcctDr) AS AlreadyControlledAmount FROM Actual_Acct_Detail WHERE PostingType IN ('R') AND " + Where + whereDimension +
                        @" AND Record_ID IN (" + result + " ) ";
                    AlreadyControlledAmount -= Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, trxName));
                }
            }

            if (AlreadyControlledAmount > 0)
            {
                _listBudgetControl = ReduceAmountFromBudget(drDataRecord, drBUdgetControl, drBudgetComtrolDimension, AlreadyControlledAmount, _listBudgetControl);
            }

            return _listBudgetControl;
        }

        /// <summary>
        /// This Function is used to Reduce From Budget controlled amount
        /// </summary>
        /// <param name="drDataRecord">document Posting Record</param>
        /// <param name="drBUdgetControl">BUdget Control information</param>
        /// <param name="drBudgetComtrolDimension">Budget Control dimension which is applicable</param>
        /// <param name="AlreadyAllocatedAmount">amount (Actual - Commitment - Reservation)</param>
        /// <param name="_listBudgetControl">list of budget control</param>
        /// <returns></returns>
        public List<BudgetControl> ReduceAmountFromBudget(DataRow drDataRecord, DataRow drBUdgetControl,
                        DataRow[] drBudgetComtrolDimension, Decimal AlreadyAllocatedAmount, List<BudgetControl> _listBudgetControl)
        {
            VLogger _log = VLogger.GetVLogger(typeof(MVABOrder).FullName);
            BudgetControl _budgetControl = null;
            List<String> selectedDimension = new List<string>();
            String _budgetMessage = String.Empty;

            if (drBudgetComtrolDimension != null)
            {
                for (int i = 0; i < drBudgetComtrolDimension.Length; i++)
                {
                    selectedDimension.Add(Util.GetValueOfString(drBudgetComtrolDimension[i]["ElementType"]));
                }
            }

            if (_listBudgetControl.Exists(x => (x.VAGL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"])) &&
                                              (x.VAGL_BudgetActivation_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.VAF_Org_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["VAF_Org_ID"]) : 0)) &&
                                              (x.VAB_BusinessPartner_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["VAB_BusinessPartner_ID"]) : 0)) &&
                                              (x.VAM_Product_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["VAM_Product_ID"]) : 0)) &&
                                              (x.VAB_BillingCode_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["VAB_BillingCode_ID"]) : 0)) &&
                                              (x.VAB_AddressFrom_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["VAB_AddressFrom_ID"]) : 0)) &&
                                              (x.VAB_AddressTo_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["VAB_AddressTo_ID"]) : 0)) &&
                                              (x.VAB_Promotion_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["VAB_Promotion_ID"]) : 0)) &&
                                              (x.VAF_OrgTrx_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["VAF_OrgTrx_ID"]) : 0)) &&
                                              (x.VAB_Project_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["VAB_Project_ID"]) : 0)) &&
                                              (x.VAB_SalesRegionState_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["VAB_SalesRegionState_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             ))
            {
                _budgetControl = _listBudgetControl.Find(x => (x.VAGL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_Budget_ID"])) &&
                                              (x.VAGL_BudgetActivation_ID == Util.GetValueOfInt(drBUdgetControl["VAGL_BudgetActivation_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.VAF_Org_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["VAF_Org_ID"]) : 0)) &&
                                              (x.VAB_BusinessPartner_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["VAB_BusinessPartner_ID"]) : 0)) &&
                                              (x.VAM_Product_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["VAM_Product_ID"]) : 0)) &&
                                              (x.VAB_BillingCode_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["VAB_BillingCode_ID"]) : 0)) &&
                                              (x.VAB_AddressFrom_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["VAB_AddressFrom_ID"]) : 0)) &&
                                              (x.VAB_AddressTo_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["VAB_AddressTo_ID"]) : 0)) &&
                                              (x.VAB_Promotion_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["VAB_Promotion_ID"]) : 0)) &&
                                              (x.VAF_OrgTrx_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["VAF_OrgTrx_ID"]) : 0)) &&
                                              (x.VAB_Project_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["VAB_Project_ID"]) : 0)) &&
                                              (x.VAB_SalesRegionState_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["VAB_SalesRegionState_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_VAB_AccountBook_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             );
                _budgetControl.ControlledAmount = Decimal.Subtract(_budgetControl.ControlledAmount, AlreadyAllocatedAmount != 0 ? AlreadyAllocatedAmount : Util.GetValueOfDecimal(drDataRecord["Debit"]));
            }
            return _listBudgetControl;
        }
    }
}
