/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_ProjectLine
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
    public class MProjectLine : X_VAB_ProjectLine
    {
        /** Parent				*/
        private MProject _parent = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_ProjectLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectLine(Ctx ctx, int VAB_ProjectLine_ID, Trx trxName)
            : base(ctx, VAB_ProjectLine_ID, trxName)
        {
            if (VAB_ProjectLine_ID == 0)
            {
                //  setVAB_Project_ID (0);
                //	setVAB_ProjectLine_ID (0);
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
            SetVAB_Project_ID(project.GetVAB_Project_ID());	// Parent
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

            //	Planned Amount	- Currency Precision
            Decimal plannedAmt = Decimal.Multiply(GetPlannedQty(), GetPlannedPrice());
            if (Env.Scale(plannedAmt) > GetCurPrecision())
            {
                //plannedAmt.setScale(GetCurPrecision(), Decimal.ROUND_HALF_UP);
                Decimal.Round(plannedAmt, GetCurPrecision(), MidpointRounding.AwayFromZero);
            }
            SetPlannedAmt(plannedAmt);

            //	Planned Margin
            if (Is_ValueChanged("VAM_Product_ID") || Is_ValueChanged("VAM_ProductCategory_ID")
                || Is_ValueChanged("PlannedQty") || Is_ValueChanged("PlannedPrice"))
            {
                if (GetVAM_Product_ID() != 0)
                {
                    Decimal marginEach = Decimal.Subtract(GetPlannedPrice(), GetLimitPrice());
                    SetPlannedMarginAmt(Decimal.Multiply(marginEach, GetPlannedQty()));
                }
                else if (GetVAM_ProductCategory_ID() != 0)
                {
                    MProductCategory category = MProductCategory.Get(GetCtx(), GetVAM_ProductCategory_ID());
                    Decimal marginEach = category.GetPlannedMargin();
                    SetPlannedMarginAmt(Decimal.Multiply(marginEach, GetPlannedQty()));
                }
            }

            //	Phase/Task
            if (Is_ValueChanged("VAB_ProjectJob_ID") && GetVAB_ProjectJob_ID() != 0)
            {
                MProjectTask pt = new MProjectTask(GetCtx(), GetVAB_ProjectJob_ID(), Get_TrxName());
                if (pt == null || pt.Get_ID() == 0)
                {
                    log.Warning("Project Task Not Found - ID=" + GetVAB_ProjectJob_ID());
                    return false;
                }
                else
                    SetVAB_ProjectStage_ID(pt.GetVAB_ProjectStage_ID());
            }
            if (Is_ValueChanged("VAB_ProjectStage_ID") && GetVAB_ProjectStage_ID() != 0)
            {
                MProjectPhase pp = new MProjectPhase(GetCtx(), GetVAB_ProjectStage_ID(), Get_TrxName());
                if (pp == null || pp.Get_ID() == 0)
                {
                    log.Warning("Project Phase Not Found - " + GetVAB_ProjectStage_ID());
                    return false;
                }
                else
                    SetVAB_Project_ID(pp.GetVAB_Project_ID());
            }

            return true;
        }

        /// <summary>
        /// Get Currency Precision
        /// </summary>
        /// <returns>2 (hardcoded)</returns>
        protected int GetCurPrecision()
        {
            return 2;
        }

        /// <summary>
        /// Get Limit Price if exists
        /// </summary>
        /// <returns>limit</returns>
        public Decimal GetLimitPrice()
        {
            Decimal limitPrice = GetPlannedPrice();
            if (GetVAM_Product_ID() == 0)
                return limitPrice;
            if (GetProject() == null)
                return limitPrice;
            bool isSOTrx = true;
            MProduct prd = new MProduct(GetCtx(), GetVAM_Product_ID(), null);
            MProductPricing pp = new MProductPricing(GetVAF_Client_ID(), GetVAF_Org_ID(),
                GetVAM_Product_ID(), _parent.GetVAB_BusinessPartner_ID(), GetPlannedQty(), isSOTrx);
            pp.SetVAM_PriceList_ID(_parent.GetVAM_PriceList_ID());
            pp.SetVAM_PriceListVersion_ID(_parent.GetVAM_PriceListVersion_ID());
            pp.SetVAB_UOM_ID(prd.GetVAB_UOM_ID());
            if (pp.CalculatePrice())
                limitPrice = pp.GetPriceLimit();
            return limitPrice;
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <returns>parent</returns>
        public MProject GetProject()
        {
            if (_parent == null && GetVAB_Project_ID() != 0)
            {
                _parent = new MProject(GetCtx(), GetVAB_Project_ID(), Get_TrxName());
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
        /// <param name="VAB_OrderPO_ID">po id</param>
        

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
                "SELECT COALESCE(MAX(Line),0)+10 FROM VAB_ProjectLine WHERE VAB_Project_ID=@param1", GetVAB_Project_ID()));
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldVAM_Product_ID">old value</param>
        /// <param name="newVAM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetVAM_Product_ID(String oldVAM_Product_ID, String newVAM_Product_ID, int windowNo)
        {
            if (newVAM_Product_ID == null || newVAM_Product_ID.Length == 0)
                return;
            int VAM_Product_ID = int.Parse(newVAM_Product_ID);
            base.SetVAM_Product_ID(VAM_Product_ID);
            if (VAM_Product_ID == 0)
                return;
            //
            int VAM_PriceListVersion_ID = GetCtx().GetContextAsInt(windowNo, "VAM_PriceListVersion_ID");
            if (VAM_PriceListVersion_ID == 0)
                return;

            int VAB_BusinessPartner_ID = GetCtx().GetContextAsInt(windowNo, "VAB_BusinessPartner_ID");
            Decimal Qty = GetPlannedQty();
            bool IsSOTrx = true;
            MProductPricing pp = new MProductPricing(GetVAF_Client_ID(), GetVAF_Org_ID(),
                    VAM_Product_ID, VAB_BusinessPartner_ID, Qty, IsSOTrx);
            pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);
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
            SetVAB_ProjectSupply_ID(pi.GetVAB_ProjectSupply_ID());
            SetVAM_Product_ID(pi.GetVAM_Product_ID());
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
            int id = GetVAB_ProjectJob_ID();
            int projID = 0;

            if (id == 0)
            {
                projID = GetVAB_Project_ID();                    // Marketing Campaign Window
            }
            else
            {
                string Sql = "SELECT VAB_Project_ID FROM VAB_ProjectStage WHERE VAB_ProjectStage_ID in(select VAB_ProjectStage_ID FROM" +
                        " VAB_ProjectJob WHERE VAB_ProjectJob_ID =" + id + ")";
                projID = Util.GetValueOfInt(DB.ExecuteScalar(Sql, null, null));
            }
            string sql = "SELECT IsOpportunity FROM VAB_Project WHERE VAB_Project_ID = " + projID;
            string isOpp = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            //Amit
            string isCam = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsCampaign FROM VAB_Project WHERE VAB_Project_ID = " + projID));
            if (isOpp.Equals("N") && isCam.Equals("N") && id != 0)
            {
                // set sum of planned Amount from task line to task
                MProjectTask tsk = new MProjectTask(GetCtx(), id, Get_Trx());
                decimal plannedAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_ProjectJob_ID = " + id));
                tsk.SetPlannedAmt(plannedAmt);
                tsk.Save();
            }
            //Amit
            else if (isOpp.Equals("N") && isCam.Equals("N") && id == 0 && GetVAB_ProjectStage_ID() != 0)
            {
                MProjectPhase projectPhase = new MProjectPhase(GetCtx(), GetVAB_ProjectStage_ID(), null);
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_ProjectStage_ID = " + GetVAB_ProjectStage_ID() + " AND pl.VAB_Project_ID = " + projID));
                projectPhase.SetPlannedAmt(plnAmt);
                projectPhase.Save();
            }
            else if (isOpp.Equals("Y"))                             // Opportunity Window
            {
                MProject prj = new MProject(GetCtx(), projID, Get_TrxName());
                decimal plnAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + projID));
                prj.SetPlannedAmt(plnAmt);
                prj.Save();

                if (Env.IsModuleInstalled("VA077_"))
                {
                    sql = @"UPDATE VAB_Project SET VA077_TotalMarginAmt=(SELECT ROUND(Sum(VA077_MarginAmt),2) FROM VAB_ProjectLine 
                            WHERE VAB_PROJECT_ID=" + projID + @" AND IsActive='Y'),
                            VA077_TotalPurchaseAmt=(SELECT ROUND(Sum(VA077_PurchaseAmt),2) FROM VAB_ProjectLine 
                            WHERE VAB_PROJECT_ID=" + projID + @" AND IsActive='Y'),
                            VA077_MarginPercent=(SELECT ROUND(Sum(VA077_MarginPercent),2) FROM VAB_ProjectLine 
                            WHERE VAB_PROJECT_ID=" + projID + @" AND IsActive='Y') WHERE VAB_Project_ID=" + projID;

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
                decimal plannedAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_ProjectJob_ID = " + id));
                tsk.SetPlannedAmt(plannedAmt);
                tsk.Save();
            }
            else
            {
                sql = "UPDATE VAB_Project p SET " +
   "PlannedAmt=(SELECT COALESCE(SUM(pl.PlannedAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID() + ")" +
   ",PlannedQty=(SELECT COALESCE(SUM(pl.PlannedQty),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID() + ")" +
   ",PlannedMarginAmt=(SELECT COALESCE(SUM(pl.PlannedMarginAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID() + ")" +
   ",CommittedAmt=(SELECT COALESCE(SUM(pl.CommittedAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID() + ")" +
   ",CommittedQty=(SELECT COALESCE(SUM(pl.CommittedQty),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID() + ")" +
   ",InvoicedAmt=(SELECT COALESCE(SUM(pl.InvoicedAmt),0)  FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID() + ")" +
   ", InvoicedQty =(SELECT COALESCE(SUM(pl.InvoicedQty),0) FROM VAB_ProjectLine pl WHERE pl.IsActive = 'Y' AND pl.VAB_Project_ID = " + GetVAB_Project_ID() + ")" +
   " WHERE p.VAB_Project_ID=" + GetVAB_Project_ID();
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
                .Append(",VAB_Project_ID=").Append(GetVAB_Project_ID())
                .Append(",VAB_ProjectStage_ID=").Append(GetVAB_ProjectStage_ID())
                .Append(",VAB_ProjectJob_ID=").Append(GetVAB_ProjectJob_ID())
                .Append(",VAB_ProjectSupply_ID=").Append(GetVAB_ProjectSupply_ID())
                .Append(", VAM_Product_ID=").Append(GetVAM_Product_ID())
                .Append(", PlannedQty=").Append(GetPlannedQty())
                .Append("]");
            return sb.ToString();
        }

    }
}
