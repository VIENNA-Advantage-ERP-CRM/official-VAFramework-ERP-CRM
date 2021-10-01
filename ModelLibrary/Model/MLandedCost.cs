/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_LandedCost
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
    public class MLandedCost : X_C_LandedCost
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MLandedCost).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_LandedCost_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLandedCost(Ctx ctx, int C_LandedCost_ID, Trx trxName)
            : base(ctx, C_LandedCost_ID, trxName)
        {
            if (C_LandedCost_ID == 0)
            {
                //	setC_InvoiceLine_ID (0);
                //	setM_CostElement_ID (0);
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
        public static MLandedCost[] GetLandedCosts(MInvoiceLine il)
        {
            List<MLandedCost> list = new List<MLandedCost>();
            String sql = "SELECT * FROM C_LandedCost WHERE C_InvoiceLine_ID=" + il.GetC_InvoiceLine_ID();
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
                if (GetM_Product_ID() == 0 && GetRef_Invoice_ID() == 0 && GetRef_InvoiceLine_ID() == 0)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                        "@NotFound@ @M_Product_ID@ | @C_Invoice_ID@ | @C_InvoiceLine_ID@"));
                    return false;
                }
            }
            else if (GetM_Product_ID() == 0
                && GetM_InOut_ID() == 0
                && GetM_InOutLine_ID() == 0)
            {
                // atleast one reference from Product, Receipt, Receipt Line, Invoice or Invoice Line must be selected if distribution is other than Import Value.
                if (Get_ColumnIndex("M_Movement_ID") > 0)
                {
                    if (GetM_Movement_ID() == 0 && GetM_MovementLine_ID() == 0)
                    {
                        log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                        "@NotFound@ @M_Product_ID@ | @M_InOut_ID@ | @M_InOutLine_ID@ | @M_Movement_ID@ | @M_MovementLine_ID@"));
                        return false;
                    }
                }
                else
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                        "@NotFound@ @M_Product_ID@ | @M_InOut_ID@ | @M_InOutLine_ID@"));
                    return false;
                }
            }

            //	No Product if Line entered
            if (GetLandedCostDistribution() == LANDEDCOSTDISTRIBUTION_ImportValue)
            {
                if (GetRef_InvoiceLine_ID() != 0 && GetM_Product_ID() != 0)
                    SetM_Product_ID(0);
            }
            else if (Get_ColumnIndex("M_Movement_ID") > 0)
            {
                if ((GetM_InOutLine_ID() != 0 || GetM_MovementLine_ID() != 0) && GetM_Product_ID() != 0)
                    SetM_Product_ID(0);
            }
            else
            {
                if (GetM_InOutLine_ID() != 0 && GetM_Product_ID() != 0)
                    SetM_Product_ID(0);
            }

            //JID_0032 : Unique Constraint handling
            StringBuilder sql = new StringBuilder();
            if (GetM_InOut_ID() > 0)
            {
                // Saved unique data basd on CostElement / Shipment / ShipmentLine
                // not to consider Reversed or voided InvoiceLine record
                sql.Clear();
                sql.Append("SELECT COUNT(C_LandedCost_ID) FROM C_LandedCost WHERE M_CostElement_ID = " + GetM_CostElement_ID() +
                    " AND M_InOut_ID = " + GetM_InOut_ID() + " AND (M_InOutLine_ID = " + GetM_InOutLine_ID() + " OR NVL(M_InOutLine_ID,0) = 0) " +
                    @" AND NOT EXISTS (SELECT C_InvoiceLine_ID FROM C_InvoiceLine il INNER JOIN C_Invoice i ON i.C_Invoice_ID = il.C_Invoice_ID 
                   AND C_LandedCost.C_Invoiceline_ID = il.C_Invoiceline_ID WHERE i.DocStatus IN ('RE' , 'VO'))");
                if (Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    // during reversal, exclude orignal record
                    sql.Append(" AND C_Invoiceline_ID    <> " + GetReversalDoc_ID());
                }
                if (!newRecord)
                {
                    // during updation, not consider current record
                    sql.Append(" AND C_LandedCost_ID <> " + GetC_LandedCost_ID());
                }
            }
            else if (GetM_Movement_ID() > 0)
            {
                // Saved unique data basd on CostElement / Movement / MovementLine
                sql.Clear();
                sql.Append("SELECT COUNT(C_LandedCost_ID) FROM C_LandedCost WHERE M_CostElement_ID = " + GetM_CostElement_ID() +
                       " AND M_Movement_ID = " + GetM_Movement_ID() + " AND (M_MovementLine_ID = " + GetM_MovementLine_ID() + " OR NVL(M_MovementLine_ID,0) = 0) " +
                       @" AND NOT EXISTS (SELECT C_InvoiceLine_ID FROM C_InvoiceLine il INNER JOIN C_Invoice i ON i.C_Invoice_ID = il.C_Invoice_ID 
                   AND C_LandedCost.C_Invoiceline_ID = il.C_Invoiceline_ID WHERE i.DocStatus IN ('RE' , 'VO'))");
                if (Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    sql.Append(" AND C_Invoiceline_ID    <> " + GetReversalDoc_ID());
                }
                if (!newRecord)
                {
                    sql.Append(" AND C_LandedCost_ID <> " + GetC_LandedCost_ID());
                }
            }
            else if (GetRef_Invoice_ID() > 0)
            {
                // Saved unique data basd on CostElement / Invoice / InvoiceLine
                sql.Clear();
                sql.Append("SELECT COUNT(C_LandedCost_ID) FROM C_LandedCost WHERE M_CostElement_ID = " + GetM_CostElement_ID() +
                       " AND Ref_Invoice_ID = " + GetRef_Invoice_ID() + " AND (Ref_InvoiceLine_ID = " + GetRef_InvoiceLine_ID() + " OR NVL(Ref_InvoiceLine_ID,0) = 0) " +
                       @" AND NOT EXISTS (SELECT C_InvoiceLine_ID FROM C_InvoiceLine il INNER JOIN C_Invoice i ON i.C_Invoice_ID = il.C_Invoice_ID 
                   AND C_LandedCost.C_Invoiceline_ID = il.C_Invoiceline_ID WHERE i.DocStatus IN ('RE' , 'VO'))");
                if (Get_ColumnIndex("ReversalDoc_ID") > 0)
                {
                    sql.Append(" AND C_Invoiceline_ID    <> " + GetReversalDoc_ID());
                }
                if (!newRecord)
                {
                    sql.Append(" AND C_LandedCost_ID <> " + GetC_LandedCost_ID());
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
        /// Implement After Delete Logic
        /// </summary>
        /// <param name="success">Success</param>
        /// <returns>true, when success</returns>
        protected override bool AfterDelete(bool success)
        {
            if (!success)
            {
                return false;
            }

            // Delete Line, on deletion of Landed Cost
            int no = DB.ExecuteQuery("DELETE FROM C_LandedCostAllocation WHERE C_LandedCost_ID=" + GetC_LandedCost_ID(), null, Get_Trx());
            if (no != 0)
            {
                log.Info("Landed Cost Allocation Line Deleted #" + no);
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
            MInvoiceLine il = new MInvoiceLine(GetCtx(), GetC_InvoiceLine_ID(), Get_TrxName());
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
                .Append(",M_CostElement_ID=").Append(GetM_CostElement_ID());
            if (GetM_InOut_ID() != 0)
                sb.Append(",M_InOut_ID=").Append(GetM_InOut_ID());
            if (GetM_InOutLine_ID() != 0)
                sb.Append(",M_InOutLine_ID=").Append(GetM_InOutLine_ID());
            if (GetM_Product_ID() != 0)
                sb.Append(",M_Product_ID=").Append(GetM_Product_ID());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
