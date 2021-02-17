/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_LCost
 * Chronological Development
 * Veena Pandey     16-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MLandedCost : X_VAB_LCost
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MLandedCost).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_LCost_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLandedCost(Ctx ctx, int VAB_LCost_ID, Trx trxName)
            : base(ctx, VAB_LCost_ID, trxName)
        {
            if (VAB_LCost_ID == 0)
            {
                //	setVAB_InvoiceLine_ID (0);
                //	setVAM_ProductCostElement_ID (0);
                SetLandedCostDistribution(LANDEDCOSTDISTRIBUTION_Quantity);	// Q
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MLandedCost(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Costs of Invoice Line
        /// </summary>
        /// <param name="il">invoice line</param>
        /// <returns>array of landed cost lines</returns>
        public static MLandedCost[] GetLandedCosts(MVABInvoiceLine il)
        {
            List<MLandedCost> list = new List<MLandedCost>();
            String sql = "SELECT * FROM VAB_LCost WHERE VAB_InvoiceLine_ID=" + il.GetVAB_InvoiceLine_ID();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, il.Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MLandedCost(il.GetCtx(), dr, il.Get_TrxName()));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            //
            MLandedCost[] retValue = new MLandedCost[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if ok</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	One Reference
            if (GetLandedCostDistribution() == LANDEDCOSTDISTRIBUTION_ImportValue)
            {
                // atleast one reference from Product, Invoice or Invoice Line must be selected if Distribution is based on Import Value.
                if (GetVAM_Product_ID() == 0 && GetRef_Invoice_ID() == 0 && GetRef_InvoiceLine_ID() == 0)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                        "@NotFound@ @VAM_Product_ID@ | @VAB_Invoice_ID@ | @VAB_InvoiceLine_ID@"));
                    return false;
                }
            }
            else if (GetVAM_Product_ID() == 0
                && GetVAM_Inv_InOut_ID() == 0
                && GetVAM_Inv_InOutLine_ID() == 0)
            {
                // atleast one reference from Product, Receipt, Receipt Line, Invoice or Invoice Line must be selected if distribution is other than Import Value.
                if (Get_ColumnIndex("VAM_InventoryTransfer_ID") > 0)
                {
                    if (GetVAM_InventoryTransfer_ID() == 0 && GetVAM_InvTrf_Line_ID() == 0)
                    {
                        log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                        "@NotFound@ @VAM_Product_ID@ | @VAM_Inv_InOut_ID@ | @VAM_Inv_InOutLine_ID@ | @VAM_InventoryTransfer_ID@ | @VAM_InvTrf_Line_ID@"));
                        return false;
                    }
                }
                else
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                        "@NotFound@ @VAM_Product_ID@ | @VAM_Inv_InOut_ID@ | @VAM_Inv_InOutLine_ID@"));
                    return false;
                }
            }

            //	No Product if Line entered
            if (GetLandedCostDistribution() == LANDEDCOSTDISTRIBUTION_ImportValue)
            {
                if (GetRef_InvoiceLine_ID() != 0 && GetVAM_Product_ID() != 0)
                    SetVAM_Product_ID(0);
            }
            else if (Get_ColumnIndex("VAM_InventoryTransfer_ID") > 0)
            {
                if ((GetVAM_Inv_InOutLine_ID() != 0 || GetVAM_InvTrf_Line_ID() != 0) && GetVAM_Product_ID() != 0)
                    SetVAM_Product_ID(0);
            }
            else
            {
                if (GetVAM_Inv_InOutLine_ID() != 0 && GetVAM_Product_ID() != 0)
                    SetVAM_Product_ID(0);
            }

            //JID_0032 : Unique Constraint handling
            StringBuilder sql = new StringBuilder();
            if (GetVAM_Inv_InOut_ID() > 0)
            {
                // Saved unique data basd on CostElement / Shipment / ShipmentLine
                // not to consider Reversed or voided InvoiceLine record
                sql.Clear();
                sql.Append("SELECT COUNT(VAB_LCost_ID) FROM VAB_LCost WHERE VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID() +
                    " AND VAM_Inv_InOut_ID = " + GetVAM_Inv_InOut_ID() + " AND (VAM_Inv_InOutLine_ID = " + GetVAM_Inv_InOutLine_ID() + " OR NVL(VAM_Inv_InOutLine_ID,0) = 0) " +
                    @" AND NOT EXISTS (SELECT VAB_InvoiceLine_ID FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice i ON i.VAB_Invoice_ID = il.VAB_Invoice_ID 
                   AND VAB_LCost.VAB_InvoiceLine_ID = il.VAB_InvoiceLine_ID WHERE i.DocStatus IN ('RE' , 'VO'))");
                if (Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    // during reversal, exclude orignal record
                    sql.Append(" AND VAB_InvoiceLine_ID    <> " + GetReversalDoc_ID());
                }
                if (!newRecord)
                {
                    // during updation, not consider current record
                    sql.Append(" AND VAB_LCost_ID <> " + GetVAB_LCost_ID());
                }
            }
            else if (GetVAM_InventoryTransfer_ID() > 0)
            {
                // Saved unique data basd on CostElement / Movement / MovementLine
                sql.Clear();
                sql.Append("SELECT COUNT(VAB_LCost_ID) FROM VAB_LCost WHERE VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID() +
                       " AND VAM_InventoryTransfer_ID = " + GetVAM_InventoryTransfer_ID() + " AND (VAM_InvTrf_Line_ID = " + GetVAM_InvTrf_Line_ID() + " OR NVL(VAM_InvTrf_Line_ID,0) = 0) " +
                       @" AND NOT EXISTS (SELECT VAB_InvoiceLine_ID FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice i ON i.VAB_Invoice_ID = il.VAB_Invoice_ID 
                   AND VAB_LCost.VAB_InvoiceLine_ID = il.VAB_InvoiceLine_ID WHERE i.DocStatus IN ('RE' , 'VO'))");
                if (Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    sql.Append(" AND VAB_InvoiceLine_ID    <> " + GetReversalDoc_ID());
                }
                if (!newRecord)
                {
                    sql.Append(" AND VAB_LCost_ID <> " + GetVAB_LCost_ID());
                }
            }
            else if (GetRef_Invoice_ID() > 0)
            {
                // Saved unique data basd on CostElement / Invoice / InvoiceLine
                sql.Clear();
                sql.Append("SELECT COUNT(VAB_LCost_ID) FROM VAB_LCost WHERE VAM_ProductCostElement_ID = " + GetVAM_ProductCostElement_ID() +
                       " AND Ref_Invoice_ID = " + GetRef_Invoice_ID() + " AND (Ref_InvoiceLine_ID = " + GetRef_InvoiceLine_ID() + " OR NVL(Ref_InvoiceLine_ID,0) = 0) " +
                       @" AND NOT EXISTS (SELECT VAB_InvoiceLine_ID FROM VAB_InvoiceLine il INNER JOIN VAB_Invoice i ON i.VAB_Invoice_ID = il.VAB_Invoice_ID 
                   AND VAB_LCost.VAB_InvoiceLine_ID = il.VAB_InvoiceLine_ID WHERE i.DocStatus IN ('RE' , 'VO'))");
                if (Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    sql.Append(" AND VAB_InvoiceLine_ID    <> " + GetReversalDoc_ID());
                }
                if (!newRecord)
                {
                    sql.Append(" AND VAB_LCost_ID <> " + GetVAB_LCost_ID());
                }
            }
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
            if (count > 0)
            {
                log.SaveError("VIS_DuplicateRecord", "");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Allocate Costs.
        ///	Done at Invoice Line Level
        /// </summary>
        /// <returns>error message or ""</returns>
        public String AllocateCosts()
        {
            MVABInvoiceLine il = new MVABInvoiceLine(GetCtx(), GetVAB_InvoiceLine_ID(), Get_TrxName());
            return il.AllocateLandedCosts();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MLandedCost[");
            sb.Append(Get_ID())
                .Append(",CostDistribution=").Append(GetLandedCostDistribution())
                .Append(",VAM_ProductCostElement_ID=").Append(GetVAM_ProductCostElement_ID());
            if (GetVAM_Inv_InOut_ID() != 0)
                sb.Append(",VAM_Inv_InOut_ID=").Append(GetVAM_Inv_InOut_ID());
            if (GetVAM_Inv_InOutLine_ID() != 0)
                sb.Append(",VAM_Inv_InOutLine_ID=").Append(GetVAM_Inv_InOutLine_ID());
            if (GetVAM_Product_ID() != 0)
                sb.Append(",VAM_Product_ID=").Append(GetVAM_Product_ID());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
