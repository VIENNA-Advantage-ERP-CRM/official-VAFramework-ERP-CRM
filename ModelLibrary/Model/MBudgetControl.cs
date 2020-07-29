using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MBudgetControl : X_GL_BudgetControl
    {
        public MBudgetControl(Ctx ctx, int GL_BudgetControl_ID, Trx trxName)
           : base(ctx, GL_BudgetControl_ID, trxName)
        {

        }

        public MBudgetControl(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// This Function is used to get Dimension applicable on which Budget
        /// </summary>
        /// <param name="BudgetControlIds">Budget Control Ids</param>
        /// <returns>dataset of Dimension applicable</returns>
        public static DataSet GetBudgetDimension(String BudgetControlIds)
        {
            DataSet dsBudgetControl = null;
            String sql = @"SELECT DISTINCT GL_BudgetControl.GL_Budget_ID, GL_BudgetControl.GL_BudgetControl_ID, GL_BudgetControlDimension.ElementType, 
                            /* CASE   WHEN GL_BudgetControlDimension.ElementType = 'OO' THEN 'AD_Org_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'BP' THEN 'C_BPartner_ID' 
                                   WHEN GL_BudgetControlDimension.ElementType = 'AY' THEN 'C_Activity_ID'
                                   WHEN (GL_BudgetControlDimension.ElementType = 'LF' OR GL_BudgetControlDimension.ElementType = 'LT') THEN 'C_Location_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'MC' THEN 'C_Campaign_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'OT' THEN 'AD_OrgTrx_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'PJ' THEN 'C_Project_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'PR' THEN 'M_Product_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'SR' THEN 'C_SalesRegion_ID'
                                ELSE CAST(AD_Column.ColumnName AS varchar(100)) END AS ColumnName, */
                                 CASE  WHEN GL_BudgetControlDimension.ElementType = 'OO' THEN 'AD_Org_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'BP' THEN 'C_BPartner_ID' 
                                   WHEN GL_BudgetControlDimension.ElementType = 'AY' THEN 'C_Activity_ID'
                                   WHEN (GL_BudgetControlDimension.ElementType = 'LF' OR GL_BudgetControlDimension.ElementType = 'LT') THEN 'C_Location_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'MC' THEN 'C_Campaign_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'OT' THEN 'AD_OrgTrx_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'PJ' THEN 'C_Project_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'PR' THEN 'M_Product_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'SR' THEN 'C_SalesRegion_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X1' THEN 'UserElement1_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X2' THEN 'UserElement2_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X3' THEN 'UserElement3_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X4' THEN 'UserElement4_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X5' THEN 'UserElement5_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X6' THEN 'UserElement6_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X7' THEN 'UserElement7_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X8' THEN 'UserElement8_ID'
                                   WHEN GL_BudgetControlDimension.ElementType = 'X9' THEN 'UserElement9_ID' END AS ElementName
                           FROM GL_BudgetControl INNER JOIN GL_BudgetControlDimension ON GL_BudgetControlDimension.GL_BudgetControl_ID = GL_BudgetControl.GL_BudgetControl_ID 
                           INNER JOIN C_AcctSchema_Element ON (GL_BudgetControl.C_AcctSchema_ID = C_AcctSchema_Element.C_AcctSchema_ID AND
                           C_AcctSchema_Element.ElementType = GL_BudgetControlDimension.ElementType) 
                           LEFT JOIN AD_Column ON AD_Column.AD_Column_ID = C_AcctSchema_Element.AD_Column_ID 
                           WHERE GL_BudgetControl.IsActive = 'Y' AND GL_BudgetControlDimension.IsActive = 'Y' AND C_AcctSchema_Element.IsActive = 'Y' 
                                 AND GL_BudgetControl.GL_BudgetControl_ID IN (" + BudgetControlIds + ")";
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
        public static List<BudgetControl> GetBudgetControlValue(DataRow drDataRecord, DataRow drBUdgetControl, DataRow[] drBudgetComtrolDimension,
            DateTime? date, List<BudgetControl> _listBudgetControl, Trx trxName, char order, int OrderId)
        {
            BudgetControl budgetControl = null;
            String groupBy = " Account_ID, C_AcctSchema_ID ";
            String whereDimension = "";

            String Where = " Account_ID = " + Util.GetValueOfInt(drDataRecord["Account_ID"])
                   + " AND C_AcctSchema_ID = " + Util.GetValueOfInt(drBUdgetControl["C_AcctSchema_ID"]);
            if (Util.GetValueOfString(drBUdgetControl["BudgetControlScope"]).Equals(X_GL_BudgetControl.BUDGETCONTROLSCOPE_PeriodOnly))
            {
                Where += " AND C_Period_ID = " + Util.GetValueOfInt(drBUdgetControl["C_Period_ID"]);
            }
            else if (Util.GetValueOfString(drBUdgetControl["BudgetControlScope"]).Equals(X_GL_BudgetControl.BUDGETCONTROLSCOPE_Total))
            {
                Where += " AND C_Period_ID IN (SELECT C_Period_ID FROM C_Period WHERE IsActive = 'Y' AND C_Year_ID =  " + Util.GetValueOfInt(drBUdgetControl["C_Year_ID"]) + " ) ";
            }
            else if (Util.GetValueOfString(drBUdgetControl["BudgetControlScope"]).Equals(X_GL_BudgetControl.BUDGETCONTROLSCOPE_YearToDate))
            {
                Where += @" AND DateAcct Between (SELECT StartDate FROM C_Period WHERE PeriodNo = 1 AND IsActive = 'Y' 
                            AND C_Year_ID =  " + Util.GetValueOfInt(drBUdgetControl["C_Year_ID"]) + " )  AND " + GlobalVariable.TO_DATE(date, true);
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
            if (_listBudgetControl.Exists(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                               (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                               (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"]))))
            {
                return _listBudgetControl;
            }

            sql += groupBy + " FROM Fact_Acct WHERE GL_Budget_ID = " + Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"]) + " AND " + Where + " GROUP BY " + groupBy;
            DataSet dsBudgetControlAmount = DB.ExecuteDataset(sql, null, null);
            if (dsBudgetControlAmount != null && dsBudgetControlAmount.Tables.Count > 0)
            {
                for (int i = 0; i < dsBudgetControlAmount.Tables[0].Rows.Count; i++)
                {
                    budgetControl = new BudgetControl();
                    budgetControl.GL_Budget_ID = Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"]);
                    budgetControl.GL_BudgetControl_ID = Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"]);
                    budgetControl.C_AcctSchema_ID = Util.GetValueOfInt(drBUdgetControl["C_AcctSchema_ID"]);
                    budgetControl.Account_ID = Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["Account_id"]);
                    budgetControl.AD_Org_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("AD_Org_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["AD_Org_ID"]) : 0;
                    budgetControl.M_Product_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("M_Product_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["M_Product_ID"]) : 0;
                    budgetControl.C_BPartner_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_BPartner_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_BPartner_ID"]) : 0;
                    budgetControl.C_Activity_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_Activity_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_Activity_ID"]) : 0;
                    budgetControl.C_LocationFrom_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_LocFrom_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_LocFrom_ID"]) : 0;
                    budgetControl.C_LocationTo_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_LocTo_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_LocTo_ID"]) : 0;
                    budgetControl.C_Campaign_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_Campaign_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_Campaign_ID"]) : 0;
                    budgetControl.AD_OrgTrx_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("AD_OrgTrx_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["AD_OrgTrx_ID"]) : 0;
                    budgetControl.C_Project_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_Project_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_Project_ID"]) : 0;
                    budgetControl.C_SalesRegion_ID = dsBudgetControlAmount.Tables[0].Columns.Contains("C_SalesRegion_ID") ? Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["C_SalesRegion_ID"]) : 0;
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
                //    CheckOrCreateDefault(Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"]),
                //    Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"]), Util.GetValueOfInt(drBUdgetControl["C_AcctSchema_ID"]),
                //    Util.GetValueOfInt(dsBudgetControlAmount.Tables[0].Rows[i]["Account_id"]));
                //}

            }

            // Reduce Amount (Budget - Actual - Commitment - Reservation) from respective budget, AlreadyControlledAmount = Actual + Commitment + Reservation
            String query = "SELECT SUM(AmtAcctDr - AmtAcctCr) AS AlreadyControlledAmount FROM Fact_Acct WHERE PostingType IN ('A' , 'E', 'R') AND " + Where + whereDimension;
            Decimal AlreadyControlledAmount = Util.GetValueOfDecimal(DB.ExecuteScalar(query, null, trxName));


            if (order.Equals('O') && Util.GetValueOfString(drBUdgetControl["CommitmentType"]).Equals(X_GL_BudgetControl.COMMITMENTTYPE_CommitmentReservation))
            {
                query = @"SELECT DISTINCT M_Requisition_ID AS Col1 FROM M_RequisitionLine WHERE M_RequisitionLine_ID IN ( 
                            SELECT DISTINCT M_RequisitionLine_ID FROM C_OrderLine WHERE C_Order_ID = " + OrderId + ")";
                DataSet ds = DB.ExecuteDataset(query, null, trxName);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object[] requisitionIds = ds.Tables[0].AsEnumerable().Select(r => r.Field<object>("COL1")).ToArray();
                    string result = string.Join(",", requisitionIds);

                    query = "SELECT SUM(AmtAcctDr) AS AlreadyControlledAmount FROM Fact_Acct WHERE PostingType IN ('R') AND " + Where + whereDimension +
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
        public static List<BudgetControl> ReduceAmountFromBudget(DataRow drDataRecord, DataRow drBUdgetControl,
                        DataRow[] drBudgetComtrolDimension, Decimal AlreadyAllocatedAmount, List<BudgetControl> _listBudgetControl)
        {
            VLogger _log = VLogger.GetVLogger(typeof(MOrder).FullName);
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

            if (_listBudgetControl.Exists(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                              (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.AD_Org_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["AD_Org_ID"]) : 0)) &&
                                              (x.C_BPartner_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["C_BPartner_ID"]) : 0)) &&
                                              (x.M_Product_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["M_Product_ID"]) : 0)) &&
                                              (x.C_Activity_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["C_Activity_ID"]) : 0)) &&
                                              (x.C_LocationFrom_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["C_LocationFrom_ID"]) : 0)) &&
                                              (x.C_LocationTo_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["C_LocationTo_ID"]) : 0)) &&
                                              (x.C_Campaign_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["C_Campaign_ID"]) : 0)) &&
                                              (x.AD_OrgTrx_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["AD_OrgTrx_ID"]) : 0)) &&
                                              (x.C_Project_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["C_Project_ID"]) : 0)) &&
                                              (x.C_SalesRegion_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["C_SalesRegion_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             ))
            {
                _budgetControl = _listBudgetControl.Find(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                              (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.AD_Org_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["AD_Org_ID"]) : 0)) &&
                                              (x.C_BPartner_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["C_BPartner_ID"]) : 0)) &&
                                              (x.M_Product_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["M_Product_ID"]) : 0)) &&
                                              (x.C_Activity_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["C_Activity_ID"]) : 0)) &&
                                              (x.C_LocationFrom_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["C_LocationFrom_ID"]) : 0)) &&
                                              (x.C_LocationTo_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["C_LocationTo_ID"]) : 0)) &&
                                              (x.C_Campaign_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["C_Campaign_ID"]) : 0)) &&
                                              (x.AD_OrgTrx_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["AD_OrgTrx_ID"]) : 0)) &&
                                              (x.C_Project_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["C_Project_ID"]) : 0)) &&
                                              (x.C_SalesRegion_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["C_SalesRegion_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             );
                _budgetControl.ControlledAmount = Decimal.Subtract(_budgetControl.ControlledAmount, AlreadyAllocatedAmount != 0 ? AlreadyAllocatedAmount : Util.GetValueOfDecimal(drDataRecord["Debit"]));
            }
            return _listBudgetControl;
        }


    }
}
