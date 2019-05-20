/********************************************************
 * ModuleName     : 
 * Purpose        : 
 * Class Used     : X_M_Transaction
 * Chronological    Development
 * Raghunandan     08-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;

using VAdvantage.Model;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MTransaction : X_M_Transaction
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param M_Transaction_ID id
	 *	@param trxName transaction
	 */
        public MTransaction(Ctx ctx, int M_Transaction_ID, Trx trxName)
            : base(ctx, M_Transaction_ID, trxName)
        {
            if (M_Transaction_ID == 0)
            {
                //	setM_Transaction_ID (0);		//	PK
                //	setM_Locator_ID (0);
                //	setM_Product_ID (0);
                SetMovementDate(DateTime.Now);
                SetMovementQty(Env.ZERO);
                //	setMovementType (MOVEMENTTYPE_CustomerShipment);
            }
        }

        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param dr result set
	 *	@param trxName transaction
	 */
        public MTransaction(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
        * 	Detail Constructor
        *	@param ctx context
        *	@param AD_Org_ID org
        * 	@param MovementType movement type
        * 	@param M_Locator_ID locator
        * 	@param M_Product_ID product
        * 	@param M_AttributeSetInstance_ID attribute
        * 	@param MovementQty qty
        * 	@param MovementDate optional date
        *	@param trxName transaction
        */
        public MTransaction(Ctx ctx, int AD_Org_ID, String MovementType,
            int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
            Decimal MovementQty, DateTime? MovementDate, Trx trxName)
            : base(ctx, 0, trxName)
        {

            SetAD_Org_ID(AD_Org_ID);
            SetMovementType(MovementType);
            if (M_Locator_ID == 0)
                throw new ArgumentException("No Locator");
            SetM_Locator_ID(M_Locator_ID);
            if (M_Product_ID == 0)
                throw new ArgumentException("No Product");
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            //
            //if (MovementQty != null)		//	Can be 0
            SetMovementQty(MovementQty);
            if (MovementDate == null)
                SetMovementDate(DateTime.Now);
            else
                SetMovementDate(MovementDate);
        }


        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new records</param>
        /// <param name="success">success</param>
        /// <returns>saved</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {

            if (!success)
                return success;

            //int count = PO.Get_Table_ID("M_TransactionSummary");
            //if (count > 0)
            //{
            //    UpdateTransactionSummary();
            //}
            //count = PO.Get_Table_ID("M_ProductStockSummary");
            //if (count > 0)
            //{
            //    UpdateStockSummary();
            //}
            return true;
        }

        public void UpdateStockSummary()
        {
            StringBuilder Qry = new StringBuilder();
            decimal OpeningStock = 0, movementQty = 0;
            MProductStockSummary Trs = null;
            MLocator loc = new MLocator(GetCtx(), GetM_Locator_ID(), Get_TrxName());
            int AD_Org_ID = loc.GetAD_Org_ID();

            Qry.Append("SELECT M_ProductStockSummary_ID FROM M_ProductStockSummary WHERE IsActive = 'Y' AND M_Product_ID = " + GetM_Product_ID() +
                " AND AD_Org_ID = " + AD_Org_ID + " AND MovementFromDate = " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int M_ProductStockSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            Qry.Clear();
            Qry.Append("SELECT Count(*) FROM M_ProductStockSummary WHERE IsActive = 'Y' AND M_Product_ID = " + GetM_Product_ID() +
                        " AND AD_Org_ID = " + AD_Org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int existOld = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            if (existOld > 0)
            {
                Qry.Clear();
                Qry.Append("SELECT QtyCloseStockOrg FROM M_ProductStockSummary WHERE IsActive = 'Y' AND M_Product_ID = " + GetM_Product_ID() +
                            " AND AD_Org_ID = " + AD_Org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true) + " ORDER BY MovementFromDate DESC");
                OpeningStock = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
            }

            if (M_ProductStockSummary_ID > 0)
            {
                Trs = new MProductStockSummary(GetCtx(), M_ProductStockSummary_ID, Get_TrxName());
                Trs.SetQtyOpenStockOrg(OpeningStock);
                Trs.SetQtyCloseStockOrg(Trs.GetQtyCloseStockOrg() + GetMovementQty());
            }
            else
            {
                Trs = new MProductStockSummary(GetCtx(), AD_Org_ID, GetM_Product_ID(),
                        0, GetCurrentQty(), GetMovementDate(), Get_TrxName());
                Trs.SetQtyOpenStockOrg(OpeningStock);                
            }
            Qry.Append("SELECT Count(*) FROM M_ProductStockSummary WHERE IsActive = 'Y' AND M_Product_ID = " + GetM_Product_ID() +
                        " AND AD_Org_ID = " + AD_Org_ID + " AND MovementFromDate > " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int existRec = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
            if (existRec > 0)
            {
                Trs.SetIsStockSummarized(false);
            }            
            if (!Trs.Save())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "StockSummaryNotSaved"));
            }
            else
            {
                if (existOld > 0)
                {
                    Qry.Clear();
                    Qry.Append("SELECT M_ProductStockSummary_ID FROM M_ProductStockSummary WHERE IsActive = 'Y' AND M_Product_ID = " + GetM_Product_ID() +
                                " AND AD_Org_ID = " + AD_Org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true) + " ORDER BY MovementFromDate DESC");
                    int oldSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
                    MProductStockSummary oldTrs = new MProductStockSummary(GetCtx(), oldSummary_ID, Get_Trx());
                    oldTrs.SetMovementToDate(Convert.ToDateTime(GetMovementDate()).AddDays(-1));
                    oldTrs.Save();
                }
            }
        }

        public void UpdateTransactionSummary()
        {
            StringBuilder Qry = new StringBuilder();
            decimal OpeningStock = 0, movementQty = 0;
            MTransactionSummary Trs = null;
            Qry.Append("SELECT M_TransactionSummary_ID FROM M_TransactionSummary WHERE IsActive = 'Y' AND  M_Product_ID = " + GetM_Product_ID() +
                " AND M_Locator_ID = " + GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() +
                " AND MovementDate = " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int M_TransactionSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            Qry.Clear();
            Qry.Append("SELECT Count(*) FROM M_TransactionSummary WHERE IsActive = 'Y' AND  M_Product_ID = " + GetM_Product_ID() +
                            " AND M_Locator_ID = " + GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() +
                            " AND MovementDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int existOld = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            Qry.Clear();
            if (existOld > 0)
            {
                Qry.Append("SELECT ClosingStock FROM M_TransactionSummary WHERE IsActive = 'Y' AND  M_Product_ID = " + GetM_Product_ID() +
                                " AND M_Locator_ID = " + GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() +
                                " AND MovementDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true) + " ORDER BY MovementDate DESC");
            }
            else
            {
                Qry.Append("SELECT NVL(GetStockofWarehouse(" + GetM_Product_ID() + "," + GetM_Locator_ID() + ",0," + GetM_AttributeSetInstance_ID() + ","
                + GlobalVariable.TO_DATE(Convert.ToDateTime(GetMovementDate()).AddDays(-1), true) + "," + GetAD_Client_ID() + "," + GetAD_Org_ID() + "),0) AS Stock FROM DUAL");
            }

            OpeningStock = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            if (M_TransactionSummary_ID > 0)
            {
                Trs = new MTransactionSummary(GetCtx(), M_TransactionSummary_ID, Get_TrxName());
                Trs.SetOpeningStock(OpeningStock);
                Trs.SetClosingStock(GetCurrentQty());
                Qry.Clear();
                Qry.Append("SELECT SUM(MovementQty) FROM M_Transaction WHERE MovementType = '" + GetMovementType() + "' AND  M_Product_ID = " + GetM_Product_ID() +
                           " AND M_Locator_ID = " + GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() +
                           "AND movementdate = " + GlobalVariable.TO_DATE(GetMovementDate(), true));
                movementQty = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
            }
            else
            {

                MLocator loc = new MLocator(GetCtx(), GetM_Locator_ID(), Get_TrxName());
                Trs = new MTransactionSummary(GetCtx(), loc.GetAD_Org_ID(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(),
                        OpeningStock, GetCurrentQty(), GetMovementDate(), Get_TrxName());
                movementQty = GetMovementQty();
            }

            if (GetMovementType() == MOVEMENTTYPE_CustomerReturns)
            {
                Trs.SetQtyCustReturn(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_CustomerShipment)
            {
                Trs.SetQtyCustShipment(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_InventoryIn)
            {
                Trs.SetQtyInventoryIn(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_InventoryOut)
            {
                Trs.SetQtyInventoryOut(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_MovementFrom)
            {
                Trs.SetQtyMoveOut(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_MovementTo)
            {
                Trs.SetQtyMoveTo(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_Production_)
            {
                Trs.SetQtyProductionOut(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_ProductionPlus)
            {
                Trs.SetQtyProductionIn(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_VendorReceipts)
            {
                Trs.SetQtyMaterialIn(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_VendorReturns)
            {
                Trs.SetQtyMaterialOut(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_WorkOrderPlus)
            {
                Trs.SetQtyWorkOrderIn(movementQty);
            }
            else if (GetMovementType() == MOVEMENTTYPE_WorkOrder_)
            {
                Trs.SetQtyWorkOrderOut(movementQty);
            }
            if (!Trs.Save())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "TrxSummaryNotSaved"));
            }
        }


        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MTransaction[");
            sb.Append(Get_ID()).Append(",").Append(GetMovementType())
                .Append(",Qty=").Append(GetMovementQty())
                .Append(",M_Product_ID=").Append(GetM_Product_ID())
                .Append(",ASI=").Append(GetM_AttributeSetInstance_ID())
                .Append("]");
            return sb.ToString();
        }

    }
}