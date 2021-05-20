/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_ProjectLine
 * Chronological Development
 * Veena Pandey     17-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProjectLine : X_C_ProjectLine
    {
        /** Parent				*/
        private MProject _parent = null;
        private int currencyPrecision = 0;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ProjectLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectLine(Ctx ctx, int C_ProjectLine_ID, Trx trxName)
            : base(ctx, C_ProjectLine_ID, trxName)
        {
            if (C_ProjectLine_ID == 0)
            {
                //  setC_Project_ID (0);
                //	setC_ProjectLine_ID (0);
                SetLine(0);
                SetIsPrinted(true);
                SetProcessed(false);
                SetInvoicedAmt(Env.ZERO);
                SetInvoicedQty(Env.ZERO);
                //
                SetPlannedAmt(Env.ZERO);
                SetPlannedMarginAmt(Env.ZERO);
                SetPlannedPrice(Env.ZERO);
                SetPlannedQty(Env.ONE);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProjectLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="project">parent</param>
        public MProjectLine(MProject project)
            : this(project.GetCtx(), 0, project.Get_TrxName())
        {
            SetClientOrg(project);
            SetC_Project_ID(project.GetC_Project_ID());	// Parent
            SetLine();
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterDelete(bool success)
        {
            UpdateHeader();
            return success;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            UpdateHeader();
            return success;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetLine() == 0)
                SetLine();

            // Work done for Purchase Price and Mergin calculation
            if (Env.IsModuleInstalled("VA077_") && Get_ColumnIndex("VA077_PurchasePrice") >= 0 && Util.GetValueOfDecimal(Get_Value("VA077_PurchasePrice")).Equals(0))
            {
                if (GetProject() != null && _parent.IsOpportunity() && Util.GetValueOfInt(_parent.Get_Value("PO_PriceList_ID")) > 0)
                {
                    Set_Value("VA077_PurchasePrice", GetPurchasePrice());

                    // Calculate Purchase Amount
                    Decimal purchaseAmt = Decimal.Multiply(GetPlannedQty(), Util.GetValueOfDecimal(Get_Value("VA077_PurchasePrice")));
                    if (Env.Scale(purchaseAmt) > GetCurPrecision())
                    {
                        purchaseAmt = Decimal.Round(purchaseAmt, GetCurPrecision(), MidpointRounding.AwayFromZero);
                    }
                    Set_Value("VA077_PurchaseAmt", purchaseAmt);
                }

                // Calculate Margin Amount
                Decimal marginEach = Decimal.Subtract(GetPlannedPrice(), Util.GetValueOfDecimal(Get_Value("VA077_PurchasePrice")));
                Set_Value("VA077_MarginAmt", Decimal.Multiply(marginEach, GetPlannedQty()));

                // Calculate Margin Percentage
                Decimal marginPer = 0;
                if (GetPlannedPrice() > 0)
                {
                    marginPer = Decimal.Round(Decimal.Multiply(Decimal.Divide(marginEach, GetPlannedPrice())
                    , Env.ONEHUNDRED), GetCurPrecision(), MidpointRounding.AwayFromZero);
                }
                Set_Value("VA077_MarginPercent", marginPer);
            }

            //	Planned Amount	- Currency Precision
            Decimal plannedAmt = Decimal.Multiply(GetPlannedQty(), GetPlannedPrice());
            if (Env.Scale(plannedAmt) > GetCurPrecision())
            {
                plannedAmt = Decimal.Round(plannedAmt, GetCurPrecision(), MidpointRounding.AwayFromZero);
            }
            SetPlannedAmt(plannedAmt);

            //	Planned Margin
            if (Is_ValueChanged("M_Product_ID") || Is_ValueChanged("M_Product_Category_ID")
                || Is_ValueChanged("PlannedQty") || Is_ValueChanged("PlannedPrice"))
            {
                if (GetM_Product_ID() != 0)
                {
                    Decimal marginEach = Decimal.Subtract(GetPlannedPrice(), GetLimitPrice());
                    SetPlannedMarginAmt(Decimal.Round(Decimal.Multiply(marginEach, GetPlannedQty()), GetCurPrecision()));
                }
                else if (GetM_Product_Category_ID() != 0)
                {
                    MProductCategory category = MProductCategory.Get(GetCtx(), GetM_Product_Category_ID());
                    Decimal marginEach = category.GetPlannedMargin();
                    SetPlannedMarginAmt(Decimal.Round(Decimal.Multiply(marginEach, GetPlannedQty()), GetCurPrecision()));
                }
            }

            //	Phase/Task
            if (Is_ValueChanged("C_ProjectTask_ID") && GetC_ProjectTask_ID() != 0)
            {
                MProjectTask pt = new MProjectTask(GetCtx(), GetC_ProjectTask_ID(), Get_TrxName());
                if (pt == null || pt.Get_ID() == 0)
                {
                    log.Warning("Project Task Not Found - ID=" + GetC_ProjectTask_ID());
                    return false;
                }
                else
                    SetC_ProjectPhase_ID(pt.GetC_ProjectPhase_ID());
            }
            if (Is_ValueChanged("C_ProjectPhase_ID") && GetC_ProjectPhase_ID() != 0)
            {
                MProjectPhase pp = new MProjectPhase(GetCtx(), GetC_ProjectPhase_ID(), Get_TrxName());
                if (pp == null || pp.Get_ID() == 0)
                {
                    log.Warning("Project Phase Not Found - " + GetC_ProjectPhase_ID());
                    return false;
                }
                else
                    SetC_Project_ID(pp.GetC_Project_ID());
            }

            return true;
        }

        /// <summary>
        /// Get Currency Precision
        /// </summary>
        /// <returns>pick StdPrecision from the Currency binded on price list</returns>
        protected int GetCurPrecision()
        {
            if (currencyPrecision == 0)
            {
                currencyPrecision = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT StdPrecision  FROM C_Currency WHERE C_Currency_ID = 
                                ( SELECT C_Currency_ID FROM M_PriceList WHERE M_PriceList_ID =
                                    (SELECT M_PriceList_ID FROM M_PriceList_Version WHERE M_PriceList_Version_ID = "
                                         + (_parent != null ? _parent.GetM_PriceList_Version_ID().ToString() :
                                            " (SELECT M_PriceList_Version_ID FROM C_Project WHERE C_Project_ID = " + GetC_Project_ID() + ")") + "))", null, Get_Trx()));
            }
            if (currencyPrecision == 0)
            {
                currencyPrecision = 2;
            }
            return currencyPrecision;
        }

        /// <summary>
        /// Get Limit Price if exists
        /// </summary>
        /// <returns>limit</returns>
        public Decimal GetLimitPrice()
        {
            Decimal limitPrice = GetPlannedPrice();
            if (GetM_Product_ID() == 0)
                return limitPrice;
            if (GetProject() == null)
                return limitPrice;
            bool isSOTrx = true;
            MProduct prd = new MProduct(GetCtx(), GetM_Product_ID(), null);
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                GetM_Product_ID(), _parent.GetC_BPartner_ID(), GetPlannedQty(), isSOTrx);
            pp.SetM_PriceList_ID(_parent.GetM_PriceList_ID());
            pp.SetM_PriceList_Version_ID(_parent.GetM_PriceList_Version_ID());

            // Get Price according to Attribute set instance if selected on Project line
            if (Get_ColumnIndex("M_AttributeSetInstance_ID") >= 0)
            {
                pp.SetM_AttributeSetInstance_ID(GetM_AttributeSetInstance_ID());
            }

            pp.SetC_UOM_ID(prd.GetC_UOM_ID());
            if (pp.CalculatePrice())
                limitPrice = pp.GetPriceLimit();
            return limitPrice;
        }

        /// <summary>
        /// Get Purchase Price if exists
        /// </summary>
        /// <returns>limit</returns>
        public Decimal GetPurchasePrice()
        {
            //VA077_PurchasePrice
            Decimal purchasePrice = 0;
            if (GetM_Product_ID() == 0)
                return purchasePrice;
            bool isSOTrx = false;
            MProduct prd = new MProduct(GetCtx(), GetM_Product_ID(), null);
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                GetM_Product_ID(), _parent.GetC_BPartner_ID(), GetPlannedQty(), isSOTrx);
            pp.SetM_PriceList_ID(Util.GetValueOfInt(_parent.Get_Value("PO_PriceList_ID")));

            DateTime? validFrom = null;     // _parent.GetDateContract();
            if (validFrom == null)
            {
                validFrom = DateTime.Now.Date;
            }

            int M_PriceList_Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' 
                        AND M_PriceList_ID = " + Util.GetValueOfInt(_parent.Get_Value("PO_PriceList_ID")) + @" AND ValidFrom <= "
                        + DB.TO_DATE(validFrom, true) + " ORDER BY ValidFrom DESC"));

            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);

            // Get Price according to Attribute set instance if selected on Project line
            if (Get_ColumnIndex("M_AttributeSetInstance_ID") >= 0)
            {
                pp.SetM_AttributeSetInstance_ID(GetM_AttributeSetInstance_ID());
            }

            // Get Price according to UOM if selected on Project line
            if (Get_ColumnIndex("C_UOM_ID") >= 0)
            {
                pp.SetC_UOM_ID(Util.GetValueOfInt(Get_Value("C_UOM_ID")));
            }
            else
            {
                pp.SetC_UOM_ID(prd.GetC_UOM_ID());
            }
            if (pp.CalculatePrice())
                purchasePrice = pp.GetPriceStd();
            return purchasePrice;
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <returns>parent</returns>
        public MProject GetProject()
        {
            if (_parent == null && GetC_Project_ID() != 0)
            {
                _parent = new MProject(GetCtx(), GetC_Project_ID(), Get_TrxName());
                if (Get_TrxName() != null)
                    _parent.Load(Get_TrxName());
            }
            return _parent;
        }

        /// <summary>
        /// Set Amount (Callout)
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        private void SetAmt(int windowNo, String columnName)
        {
            int curPrecision = GetCurPrecision();
            int plPrecision = GetCtx().GetContextAsInt(windowNo, "StdPrecision");

            //	get values
            Decimal plannedQty = GetPlannedQty();
            //if (plannedQty == null)
            //    plannedQty = Env.ONE;
            Decimal plannedPrice = GetPlannedPrice();
            //if (plannedPrice == null)
            //    plannedPrice = Env.ZERO;
            Decimal PriceList = GetPriceList();
            //if (PriceList == null)
            //    PriceList = plannedPrice;
            Decimal Discount = GetDiscount();
            //if (Discount == null)
            //    Discount = Env.ZERO;

            if (columnName.Equals("PlannedPrice"))
            {
                if (Math.Sign(PriceList) == 0)
                    Discount = Env.ZERO;
                else
                {
                    //Decimal multiplier = plannedPrice.multiply(Env.ONEHUNDRED)
                    //    .divide(PriceList, plPrecision, BigDecimal.ROUND_HALF_UP);
                    Decimal multiplier = Decimal.Round(Decimal.Divide(Decimal.Multiply(plannedPrice, Env.ONEHUNDRED)
                        , PriceList), plPrecision, MidpointRounding.AwayFromZero);
                    Discount = Decimal.Subtract(Env.ONEHUNDRED, multiplier);
                }
                SetDiscount(Discount);
                log.Fine("PriceList=" + PriceList + " - Discount=" + Discount
                    + " -> [PlannedPrice=" + plannedPrice + "] (Precision=" + plPrecision + ")");
            }
            else if (columnName.Equals("PriceList"))
            {
                if (Math.Sign(PriceList) == 0)
                    Discount = Env.ZERO;
                else
                {
                    //BigDecimal multiplier = plannedPrice.multiply(Env.ONEHUNDRED)
                    //.divide(PriceList, plPrecision, BigDecimal.ROUND_HALF_UP);
                    Decimal multiplier = Decimal.Round(Decimal.Divide(Decimal.Multiply(plannedPrice, Env.ONEHUNDRED)
                        , PriceList), plPrecision, MidpointRounding.AwayFromZero);
                    Discount = Decimal.Subtract(Env.ONEHUNDRED, multiplier);
                }
                SetDiscount(Discount);
                log.Fine("[PriceList=" + PriceList + "] - Discount=" + Discount
                    + " -> PlannedPrice=" + plannedPrice + " (Precision=" + plPrecision + ")");
            }
            else if (columnName.Equals("Discount"))
            {
                //Decimal multiplier = Discount.divide(Env.ONEHUNDRED, 10, BigDecimal.ROUND_HALF_UP);
                Decimal multiplier = Decimal.Round(Decimal.Divide(Discount, Env.ONEHUNDRED), 10, MidpointRounding.AwayFromZero);
                multiplier = Decimal.Subtract(Env.ONE, multiplier);
                //
                plannedPrice = Decimal.Multiply(PriceList, multiplier);
                if (Env.Scale(plannedPrice) > plPrecision)
                {
                    //plannedPrice = plannedPrice.setScale(plPrecision, Decimal.ROUND_HALF_UP);
                    plannedPrice = Decimal.Round(plannedPrice, plPrecision, MidpointRounding.AwayFromZero);
                }
                SetPlannedPrice(plannedPrice);
                log.Fine("PriceList=" + PriceList + " - [Discount=" + Discount
                   + "] -> PlannedPrice=" + plannedPrice + " (Precision=" + plPrecision + ")");
            }

            //	Calculate Line Amount
            Decimal plannedAmt = Decimal.Multiply(plannedQty, plannedPrice);
            if (Env.Scale(plannedAmt) > curPrecision)
            {
                //plannedAmt = plannedAmt.setScale(curPrecision, Decimal.ROUND_HALF_UP);
                plannedAmt = Decimal.Round(plannedAmt, curPrecision, MidpointRounding.AwayFromZero);
            }
            //
            log.Fine("PlannedQty=" + plannedQty + " * PlannedPrice=" + plannedPrice
                + " -> plannedAmt=" + plannedAmt + " (Precision=" + curPrecision + ")");
            SetPlannedAmt(plannedAmt);
        }

        /// <summary>
        /// Set PO
        /// </summary>
        /// <param name="C_OrderPO_ID">po id</param>


        /// <summary>
        /// Set Discount - Callout
        /// </summary>
        /// <param name="oldDiscount">old value</param>
        /// <param name="newDiscount">new value</param>
        /// <param name="windowNo">window</param>
        // @UICallout
        public void SetDiscount(String oldDiscount, String newDiscount, int windowNo)
        {
            if (newDiscount == null || newDiscount.Length == 0)
                return;
            Decimal Discount = new Decimal(int.Parse(newDiscount));
            base.SetDiscount(Discount);
            SetAmt(windowNo, "Discount");
        }

        /// <summary>
        /// Get the next Line No
        /// </summary>
        private void SetLine()
        {
            SetLine(DataBase.DB.GetSQLValue(Get_TrxName(),
                "SELECT COALESCE(MAX(Line),0)+10 FROM C_ProjectLine WHERE C_Project_ID=@param1", GetC_Project_ID()));
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
                return;
            int M_Product_ID = int.Parse(newM_Product_ID);
            base.SetM_Product_ID(M_Product_ID);
            if (M_Product_ID == 0)
                return;
            //
            int M_PriceList_Version_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_Version_ID");
            if (M_PriceList_Version_ID == 0)
                return;

            int C_BPartner_ID = GetCtx().GetContextAsInt(windowNo, "C_BPartner_ID");
            Decimal Qty = GetPlannedQty();
            bool IsSOTrx = true;
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    M_Product_ID, C_BPartner_ID, Qty, IsSOTrx);
            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            DateTime? date = GetPlannedDate();
            if (date == null)
                date = new DateTime(GetCtx().GetContextAsTime(windowNo, "DateContract"));
            pp.SetPriceDate(date);
            //
            Decimal PriceList = pp.GetPriceList();
            SetPriceList(PriceList);
            Decimal plannedPrice = pp.GetPriceStd();
            SetPlannedPrice(plannedPrice);
            Decimal Discount = pp.GetDiscount();
            SetDiscount(Discount);
            //
            Decimal plannedAmt = pp.GetLineAmt(GetCurPrecision());
            SetPlannedAmt(plannedAmt);
            //	
            //p_changeVO.setContext(GetCtx(), windowNo, "StdPrecision", pp.GetPrecision());
            log.Fine("PlannedQty=" + Qty + " * PlannedPrice=" + plannedPrice + " -> PlannedAmt=" + plannedAmt);
        }

        /// <summary>
        /// Set Product, committed qty, etc.
        /// </summary>
        /// <param name="pi">project issue</param>
        public void SetMProjectIssue(MProjectIssue pi)
        {
            SetC_ProjectIssue_ID(pi.GetC_ProjectIssue_ID());
            SetM_Product_ID(pi.GetM_Product_ID());
            SetCommittedQty(pi.GetMovementQty());
            if (GetDescription() != null)
                SetDescription(pi.GetDescription());
        }

        /// <summary>
        /// Set PlannedPrice - Callout
        /// </summary>
        /// <param name="oldPlannedPrice">old value</param>
        /// <param name="newPlannedPrice">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPlannedPrice(String oldPlannedPrice, String newPlannedPrice, int windowNo)
        {
            if (newPlannedPrice == null || newPlannedPrice.Length == 0)
                return;
            Decimal plannedPrice = new Decimal(int.Parse(newPlannedPrice));
            base.SetPlannedPrice(plannedPrice);
            SetAmt(windowNo, "PlannedPrice");
        }

        /// <summary>
        /// Set PlannedQty - Callout
        /// </summary>
        /// <param name="oldPlannedQty">old value</param>
        /// <param name="newPlannedQty">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPlannedQty(String oldPlannedQty, String newPlannedQty, int windowNo)
        {
            if (newPlannedQty == null || newPlannedQty.Length == 0)
                return;
            Decimal plannedQty = new Decimal(int.Parse(newPlannedQty));
            base.SetPlannedQty(plannedQty);
            SetAmt(windowNo, "PlannedQty");
        }

        /// <summary>
        /// Set PriceList - Callout
        /// </summary>
        /// <param name="oldPriceList">old value</param>
        /// <param name="newPriceList">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPriceList(String oldPriceList, String newPriceList, int windowNo)
        {
            if (newPriceList == null || newPriceList.Length == 0)
                return;
            Decimal PriceList = new Decimal(int.Parse(newPriceList));
            base.SetPriceList(PriceList);
            SetAmt(windowNo, "PriceList");
        }

        /// <summary>
        /// Update Header
        /// </summary>
        private void UpdateHeader()
        {
            int id = GetC_ProjectTask_ID();
            int projID = 0;

            if (id == 0)
            {
                projID = GetC_Project_ID();                    // Marketing Campaign Window
            }
            else
            {
                //Used transaction because total was not updating on header
                string Sql = "SELECT C_Project_ID FROM C_ProjectPhase WHERE C_ProjectPhase_ID in(select C_ProjectPhase_ID FROM" +
                        " C_ProjectTask WHERE C_ProjectTask_ID =" + id + ")";
                projID = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, Get_TrxName()));
            }
            //Used transaction because total was not updating on header
            string sql = "SELECT IsOpportunity FROM C_Project WHERE C_Project_ID = " + projID;
            string isOpp = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));
            //Amit
            string isCam = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCampaign FROM C_Project WHERE C_Project_ID = " + projID, null, Get_TrxName()));
            if (isOpp.Equals("N") && isCam.Equals("N") && id != 0)
            {
                // set sum of planned Amount from task line to task
                MProjectTask tsk = new MProjectTask(GetCtx(), id, Get_Trx());
                //Used transaction because total was not updating on header
                decimal plannedAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_ProjectTask_ID = " + id, null, Get_TrxName()));
                tsk.SetPlannedAmt(plannedAmt);
                tsk.Save();
            }
            //Amit
            else if (isOpp.Equals("N") && isCam.Equals("N") && id == 0 && GetC_ProjectPhase_ID() != 0)
            {
                //Used transaction because total was not updating on header
                MProjectPhase projectPhase = new MProjectPhase(GetCtx(), GetC_ProjectPhase_ID(), Get_TrxName());
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_ProjectPhase_ID = " + GetC_ProjectPhase_ID() + " AND pl.C_Project_ID = " + projID, null, Get_TrxName()));
                projectPhase.SetPlannedAmt(plnAmt);
                projectPhase.Save();
            }
            else if (isOpp.Equals("Y"))                             // Opportunity Window
            {
                //Used transaction because total was not updating on header
                MProject prj = new MProject(GetCtx(), projID, Get_TrxName());
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + projID, null, Get_TrxName()));
                prj.SetPlannedAmt(plnAmt);
                prj.Save();

                if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
                {
                    sql = @"UPDATE C_Project SET VA077_TotalMarginAmt=(SELECT ROUND(Sum(VA077_MarginAmt),2) FROM C_ProjectLine 
                            WHERE C_PROJECT_ID=" + projID + @" AND IsActive='Y'),
                            VA077_TotalPurchaseAmt=(SELECT ROUND(Sum(VA077_PurchaseAmt),2) FROM C_ProjectLine 
                            WHERE C_PROJECT_ID=" + projID + @" AND IsActive='Y'),
                            VA077_MarginPercent=(SELECT CASE WHEN Sum(PlannedAmt) > 0 Then 
                                                 ROUND(((Sum(PlannedAmt)- Sum(NVL(VA077_PurchaseAmt,0)))/Sum(PlannedAmt)*100),2) ELSE 0  END 
                                                 FROM C_ProjectLine WHERE C_PROJECT_ID=" + projID + @" AND IsActive='Y') 
                            WHERE C_Project_ID=" + projID;

                    int no = DB.ExecuteQuery(sql, null, Get_TrxName());
                    if (no != 1)
                    {
                        log.Log(Level.SEVERE, "updateHeader - #" + no);
                    }
                }
            }
            else if (id != 0)
            {
                MProjectTask tsk = new MProjectTask(GetCtx(), id, Get_TrxName());
                //Used transaction because total was not updating on header
                decimal plannedAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_ProjectTask_ID = " + id, null, Get_TrxName()));
                tsk.SetPlannedAmt(plannedAmt);
                tsk.Save();
            }
            else
            {
                sql = "UPDATE C_Project p SET " +
   "PlannedAmt=(SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + GetC_Project_ID() + ")" +
   ",PlannedQty=(SELECT COALESCE(SUM(pl.PlannedQty),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + GetC_Project_ID() + ")" +
   ",PlannedMarginAmt=(SELECT COALESCE(SUM(pl.PlannedMarginAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + GetC_Project_ID() + ")" +
   ",CommittedAmt=(SELECT COALESCE(SUM(pl.CommittedAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + GetC_Project_ID() + ")" +
   ",CommittedQty=(SELECT COALESCE(SUM(pl.CommittedQty),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + GetC_Project_ID() + ")" +
   ",InvoicedAmt=(SELECT COALESCE(SUM(pl.InvoicedAmt),0)  FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + GetC_Project_ID() + ")" +
   ", InvoicedQty =(SELECT COALESCE(SUM(pl.InvoicedQty),0) FROM C_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.C_Project_ID = " + GetC_Project_ID() + ")" +
   " WHERE p.C_Project_ID=" + GetC_Project_ID();
                int no = DB.ExecuteQuery(sql, null, Get_TrxName());

                if (no != 1)
                {
                    log.Log(Level.SEVERE, "updateHeader - #" + no);
                }
            }
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProjectLine[");
            sb.Append(Get_ID()).Append("-")
                .Append(GetLine())
                .Append(",C_Project_ID=").Append(GetC_Project_ID())
                .Append(",C_ProjectPhase_ID=").Append(GetC_ProjectPhase_ID())
                .Append(",C_ProjectTask_ID=").Append(GetC_ProjectTask_ID())
                .Append(",C_ProjectIssue_ID=").Append(GetC_ProjectIssue_ID())
                .Append(", M_Product_ID=").Append(GetM_Product_ID())
                .Append(", PlannedQty=").Append(GetPlannedQty())
                .Append("]");
            return sb.ToString();
        }

    }
}
