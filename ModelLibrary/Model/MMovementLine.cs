/********************************************************
 * Module Name    : 
 * Purpose        : Inventory Move Line Model
 * Class Used     : X_M_Movementline
 * Chronological Development
 * Veena         26-Oct-2009
 ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Inventory Move Line Model
    /// </summary>
    public class MMovementLine : X_M_MovementLine
    {
        /** Parent							*/
        private MMovement _parent = null;
        public Decimal? OnHandQty = 0;
        private Decimal? OnHandQtyTo = 0;
        private Decimal? containerQty = 0;
        private Decimal? containerQtyTo = 0;
        private decimal qtyReserved = 0;
        private MStorage storage = null;
        //private decimal qtyAvailable = 0;
        private int _mvlOldAttId = 0, _mvlNewAttId = 0;
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_MovementLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MMovementLine(Ctx ctx, int M_MovementLine_ID, Trx trxName)
            : base(ctx, M_MovementLine_ID, trxName)
        {
            if (M_MovementLine_ID == 0)
            {
                //	SetM_LocatorTo_ID (0);	// @M_LocatorTo_ID@
                //	SetM_Locator_ID (0);	// @M_Locator_ID@
                //	SetM_MovementLine_ID (0);			
                //	SetLine (0);	
                //	SetM_Product_ID (0);
                SetM_AttributeSetInstance_ID(0);	//	ID
                SetMovementQty(Env.ZERO);	// 1
                SetTargetQty(Env.ZERO);	// 0
                SetScrappedQty(Env.ZERO);
                SetConfirmedQty(Env.ZERO);
                SetQtyEntered(Env.ZERO);//Arpit
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MMovementLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MMovementLine(MMovement parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetParent(parent);
            SetClientOrg(parent);
            SetM_Movement_ID(parent.GetM_Movement_ID());
        }

        /// <summary>
        /// Get AttributeSetInstance To
        /// </summary>
        /// <returns>asi</returns>
        public new int GetM_AttributeSetInstanceTo_ID()
        {
            int M_AttributeSetInstanceTo_ID = base.GetM_AttributeSetInstanceTo_ID();
            if (M_AttributeSetInstanceTo_ID == 0)
                M_AttributeSetInstanceTo_ID = base.GetM_AttributeSetInstance_ID();
            return M_AttributeSetInstanceTo_ID;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">description text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <returns>product or null if not defined</returns>
        public MProduct GetProduct()
        {
            if (GetM_Product_ID() != 0)
                return MProduct.Get(GetCtx(), GetM_Product_ID());
            return null;
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
                return;
            int M_Product_ID = int.Parse(newM_Product_ID);
            if (M_Product_ID == 0)
                return;
            //
            base.SetM_Product_ID(M_Product_ID);
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID") != 0)
                SetM_AttributeSetInstance_ID(GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID"));
            else
                SetM_AttributeSetInstance_ID(0);
        }


        /// <summary>
        /// Set Movement Qty - enforce UOM
        /// </summary>
        /// <param name="movementQty">qty</param>
        public new void SetMovementQty(Decimal? movementQty)
        {
            if (movementQty != null)
            {
                MProduct product = GetProduct();
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    movementQty = Decimal.Round(movementQty.Value, precision, MidpointRounding.AwayFromZero);
                }
            }
            base.SetMovementQty(movementQty);
        }

        /// <summary>
        /// Set Qty Entered - enforce UOM
        /// </summary>
        /// <param name="movementQty">qty</param>
        public new void SetQtyEntered(Decimal? movementQty)
        {
            if (movementQty != null)
            {
                MProduct product = GetProduct();
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    movementQty = Decimal.Round(movementQty.Value, precision, MidpointRounding.AwayFromZero);
                }
            }
            base.SetQtyEntered(movementQty);
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// 
        /// <returns>Parent Movement</returns>
        public MMovement GetParent()
        {
            if (_parent == null)
                _parent = new MMovement(GetCtx(), GetM_Movement_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">parent</param>
        public void SetParent(MMovement parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            Decimal VA024_ProvisionPrice = 0;
            MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());

            // chck pallet Functionality applicable or not
            bool isContainrApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            // Get Old Value of AttributeSetInstance_ID
            _mvlOldAttId = Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID"));

            //	Set Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM M_MovementLine WHERE M_Movement_ID=" + GetM_Movement_ID();
                int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                SetLine(ii);
            }

            // JID_0775: System is not checking on move line that the attribute set instance is mandatory.
            if (GetM_AttributeSetInstance_ID() == 0)
            {
                if (product != null && product.GetM_AttributeSet_ID() != 0)
                {
                    MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                    if (mas.IsMandatory() || mas.IsMandatoryAlways())
                    {
                        log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "M_AttributeSetInstance_ID"));
                        return false;
                    }
                }
            }

            // Check Locator For Header Warehouse
            MMovement mov = GetParent();
            MLocator loc = new MLocator(GetCtx(), GetM_Locator_ID(), Get_TrxName());

            if (Env.IsModuleInstalled("DTD001_"))
            {
                if (mov.GetDTD001_MWarehouseSource_ID() == loc.GetM_Warehouse_ID())
                {

                }
                else
                {
                    String sql = "SELECT M_Locator_ID FROM M_Locator WHERE M_Warehouse_ID=" + mov.GetDTD001_MWarehouseSource_ID() + " AND IsDefault = 'Y'";
                    int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                    SetM_Locator_ID(ii);
                }
            }

            // when we try to move product from container to container which are in same locator then no need to check this case
            if (GetM_Locator_ID() == GetM_LocatorTo_ID() &&
                (Get_ColumnIndex("M_ProductContainer_ID") > 0 && Get_ColumnIndex("Ref_M_ProductContainerTo_ID") > 0 &&
                 !(GetM_ProductContainer_ID() > 0 || GetRef_M_ProductContainerTo_ID() > 0)))
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "'From @M_Locator_ID@' and '@M_LocatorTo_ID@' cannot be same."));//change message according to requirement
                return false;
            }

            // VIS0060: when we try to move product from container to container then from container and to container can not be same
            if (GetM_Locator_ID() == GetM_LocatorTo_ID() && Get_ColumnIndex("M_ProductContainer_ID") > 0 && Get_ColumnIndex("Ref_M_ProductContainerTo_ID") > 0
                && GetM_ProductContainer_ID() > 0 && GetRef_M_ProductContainerTo_ID() > 0 && GetM_ProductContainer_ID() == GetRef_M_ProductContainerTo_ID())
            {
                log.SaveError("VIS_ContainerCantSame", "");
                return false;
            }

            if (Env.Signum(GetMovementQty()) == 0 && Util.GetValueOfInt(GetTargetQty()) == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "MovementQty"));
                return false;
            }

            if (Env.IsModuleInstalled("DTD001_"))
            {
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("M_Product_ID")) != GetM_Product_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ProdNotChanged"));
                    return false;
                }
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("M_Locator_ID")) != GetM_Locator_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_LocatorNotChanged"));
                    return false;
                }
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("M_RequisitionLine_ID")) != GetM_RequisitionLine_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ReqNotChanged"));
                    return false;
                }
            }
            //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA203_'", null, null)) > 0)
            if (Env.IsModuleInstalled("VA203_"))
            {
                if (GetM_RequisitionLine_ID() > 0)
                {
                    MRequisitionLine reqline = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), Get_Trx());        // Trx used to handle query stuck problem
                    if (GetM_AttributeSetInstance_ID() != reqline.GetM_AttributeSetInstance_ID())
                    {
                        log.SaveError("Message", Msg.GetMsg(GetCtx(), "VA203_AttributeInstanceMustBeSame"));
                        return false;
                    }
                }
            }
            // IF Doc Status = InProgress then No record Save
            // MMovement move = new MMovement(GetCtx(), GetM_Movement_ID(), Get_Trx());        // Trx used to handle query stuck problem
            if (newRecord && mov.GetDocStatus() == "IP")
            {
                log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_CannotCreate"));
                return false;
            }

            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetQtyEntered(GetQtyEntered());

            // change to set Converted Quantity in Movement quantity if there is differnce in UOM of Base Product and UOM Selected on line
            if (newRecord || Is_ValueChanged("QtyEntered") || Is_ValueChanged("C_UOM_ID"))
            {
                Decimal? qty = Util.GetValueOfDecimal(Get_Value("QtyEntered"));
                if (product.GetC_UOM_ID() != Util.GetValueOfInt(Get_Value("C_UOM_ID")))
                {
                    SetMovementQty(MUOMConversion.ConvertProductFrom(GetCtx(), GetM_Product_ID(), Util.GetValueOfInt(Get_Value("C_UOM_ID")), Util.GetValueOfDecimal(Get_Value("QtyEntered"))));
                }
            }

            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetMovementQty(GetMovementQty());

            StringBuilder qry = new StringBuilder();
            if (!mov.IsProcessing() || newRecord)
            {
                MWarehouse wh = null; MWarehouse whTo = null;
                wh = MWarehouse.Get(GetCtx(), mov.GetDTD001_MWarehouseSource_ID());
                whTo = MWarehouse.Get(GetCtx(), MLocator.Get(GetCtx(), GetM_LocatorTo_ID()).GetM_Warehouse_ID());

                qry.Append("SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM M_Storage where m_locator_id=" + GetM_Locator_ID() + " and m_product_id=" + GetM_Product_ID());
                if (GetDTD001_AttributeNumber() == null || GetM_AttributeSetInstance_ID() > 0)
                {
                    qry.Append(" AND NVL(M_AttributeSetInstance_ID , 0) =" + GetM_AttributeSetInstance_ID());
                }
                OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry.ToString()));

                qry.Clear();
                qry.Append("SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM M_Storage where m_locator_id=" + GetM_LocatorTo_ID() + " and m_product_id=" + GetM_Product_ID()
                            + " AND NVL(M_AttributeSetInstance_ID, 0) =" + GetM_AttributeSetInstance_ID());
                OnHandQtyTo = Convert.ToDecimal(DB.ExecuteScalar(qry.ToString()));

                // SI_0635 : System is giving error of insufficient qty if disallow is true in TO warehouse and false in From warehouse
                // when record is in completed & closed stage - then no need to check qty availablity in warehouse
                if ((wh.IsDisallowNegativeInv() || whTo.IsDisallowNegativeInv()) &&
                    (!(mov.GetDocStatus() == "CO" || mov.GetDocStatus() == "CL" || mov.GetDocStatus() == "RE" || mov.GetDocStatus() == "VO")))
                {
                    // pick container current qty from transaction based on locator / product / ASI / Container / Movement Date 
                    if (isContainrApplicable && Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        //                    qry = @"SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.M_Transaction_ID) AS CurrentQty FROM m_transaction t 
                        //                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) +
                        //                                " AND t.AD_Client_ID = " + GetAD_Client_ID() +
                        //                                @" AND t.M_Locator_ID = " + (move.IsReversal() && GetMovementQty() < 0 ? GetM_LocatorTo_ID() : GetM_Locator_ID()) +
                        //                                " AND t.M_Product_ID = " + GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + GetM_AttributeSetInstance_ID() +
                        //                                " AND NVL(t.M_ProductContainer_ID, 0) = " + (move.IsReversal() && !IsMoveFullContainer() && GetMovementQty() < 0 ? GetRef_M_ProductContainerTo_ID() : GetM_ProductContainer_ID());
                        //                    containerQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion

                        //                    qry = @"SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.M_Transaction_ID) AS CurrentQty FROM m_transaction t 
                        //                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) +
                        //                               " AND t.AD_Client_ID = " + GetAD_Client_ID() +
                        //                               @" AND t.M_Locator_ID = " + (move.IsReversal() && GetMovementQty() < 0 ? GetM_Locator_ID() : GetM_LocatorTo_ID()) +
                        //                               " AND t.M_Product_ID = " + GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + GetM_AttributeSetInstance_ID() +
                        //                               " AND NVL(t.M_ProductContainer_ID, 0) = " + (move.IsReversal() && !IsMoveFullContainer() && GetMovementQty() < 0 ? GetM_ProductContainer_ID() : GetRef_M_ProductContainerTo_ID());
                        //                    containerQtyTo = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion
                        qry.Clear();
                        qry.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                                    INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(mov.GetMovementDate(), true) +
                                    " AND t.AD_Client_ID = " + GetAD_Client_ID() +
                                    @" AND t.M_Locator_ID = " + GetM_Locator_ID() +
                                    " AND t.M_Product_ID = " + GetM_Product_ID() + " AND NVL(t.M_ProductContainer_ID, 0) = " + GetM_ProductContainer_ID());

                        // In Case of Attribute Number do not check qty with attribute from storage
                        if (GetDTD001_AttributeNumber() == null || GetM_AttributeSetInstance_ID() > 0)
                        {
                            qry.Append(" AND NVL(M_AttributeSetInstance_ID , 0) =" + GetM_AttributeSetInstance_ID());
                        }

                        containerQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion
                        qry.Clear();
                        qry.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC)  AS CurrentQty FROM m_transaction t 
                                   INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(mov.GetMovementDate(), true) +
                                   " AND t.AD_Client_ID = " + GetAD_Client_ID() +
                                   @" AND t.M_Locator_ID = " + GetM_LocatorTo_ID() +
                                   " AND t.M_Product_ID = " + GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + GetM_AttributeSetInstance_ID() +
                                   " AND NVL(t.M_ProductContainer_ID, 0) = " + GetRef_M_ProductContainerTo_ID());
                        containerQtyTo = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion

                    }

                    if (wh.IsDisallowNegativeInv() && (OnHandQty - GetMovementQty()) < 0)
                    {
                        // check for From Locator
                        log.SaveError("Info", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                        return false;
                    }
                    else if (isContainrApplicable && wh.IsDisallowNegativeInv() && Get_ColumnIndex("M_ProductContainer_ID") >= 0 && (containerQty - GetMovementQty()) < 0)
                    {
                        // check container qty -  for From Locator
                        log.SaveError("Info", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQtyContainer") + containerQty);
                        return false;
                    }
                    else if (whTo.IsDisallowNegativeInv() && (OnHandQtyTo + GetMovementQty()) < 0)
                    {
                        // check for To Locator
                        log.SaveError("Info", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQtyTo);
                        return false;
                    }
                    else if (isContainrApplicable && whTo.IsDisallowNegativeInv() && Get_ColumnIndex("M_ProductContainer_ID") >= 0 && (containerQtyTo + GetMovementQty()) < 0)
                    {
                        // check for To Locator
                        log.SaveError("Info", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQtyContainerTo") + containerQtyTo);
                        return false;
                    }
                }
            }

            if (Env.IsModuleInstalled("DTD001_"))
            {
                // not used this variable, that why commented 
                //qry = "SELECT   NVL(SUM(NVL(QtyOnHand,0)- qtyreserved),0) AS QtyAvailable  FROM M_Storage where m_locator_id=" + GetM_Locator_ID() + " and m_product_id=" + GetM_Product_ID();
                //if (GetM_AttributeSetInstance_ID() != 0)
                //{
                //    qry += " AND NVL(M_AttributeSetInstance_ID , 0) = " + GetM_AttributeSetInstance_ID();
                //}
                //qtyAvailable = Convert.ToDecimal(DB.ExecuteScalar(qry));
                qtyReserved = Util.GetValueOfDecimal(Get_ValueOld("MovementQty"));
                //if (wh.IsDisallowNegativeInv() == true)
                //{
                //    if ((qtyAvailable < (GetMovementQty() - qtyReserved)))
                //    {
                //        log.SaveError("Message", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "DTD001_QtyNotAvailable"));
                //        return false;
                //    }
                //}
            }

            //By Amit - 17-April-2017
            if (Env.IsModuleInstalled("VA024_"))
            {
                // checking are we moving product from one warehouse to other warehouse
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT CASE WHEN ((SELECT CASE WHEN o.IsLegalEntity = 'Y' THEN w.AD_Org_ID
                 ELSE (SELECT AD_Org_ID FROM AD_Org WHERE Ad_Org_id = o.LegalEntityOrg ) END
                 FROM m_warehouse w INNER JOIN ad_org o ON o.AD_Org_ID = w.AD_Org_ID WHERE w.m_warehouse_id = m.DTD001_MWarehouseSource_ID)) =
                 (SELECT  CASE WHEN o2.IsLegalEntity = 'Y' THEN w2.AD_Org_ID 
                 ELSE (SELECT AD_Org_ID FROM AD_Org WHERE Ad_Org_id = o2.LegalEntityOrg) END
                 FROM m_warehouse w2 INNER JOIN ad_org o2 ON o2.AD_Org_ID = w2.AD_Org_ID WHERE M_Warehouse_ID = m.M_Warehouse_ID )
                 THEN 0 ELSE (SELECT ad_org_id FROM m_warehouse WHERE M_Warehouse_ID = m.M_Warehouse_ID ) END AS result FROM m_movement m WHERE m_movement_id = " + GetM_Movement_ID(), null, Get_Trx())) > 0)
                {
                    string qry1 = @"SELECT  SUM(o.VA024_UnitPrice)   FROM VA024_t_ObsoleteInventory o 
                                  WHERE o.IsActive = 'Y' AND  o.M_Product_ID = " + GetM_Product_ID() + @" and 
                                  ( o.M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() + @" OR o.M_AttributeSetInstance_ID IS NULL )" +
                             " AND o.AD_Org_ID = " + GetAD_Org_ID();
                    VA024_ProvisionPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                    SetVA024_UnitPrice(Util.GetValueOfDecimal(VA024_ProvisionPrice * GetMovementQty()));

                    // is used to get cost of binded cost method / costing level of primary accounting schema
                    Decimal cost = MCost.GetproductCosts(mov.GetAD_Client_ID(), mov.GetAD_Org_ID(), GetM_Product_ID(),
                         GetM_AttributeSetInstance_ID(), Get_Trx(), mov.GetDTD001_MWarehouseSource_ID());
                    SetVA024_CostPrice((cost - VA024_ProvisionPrice) * GetMovementQty());
                }
            }

            //	Mandatory Instance
            if (GetM_AttributeSetInstanceTo_ID() == 0)
            {
                if (GetM_AttributeSetInstance_ID() != 0)	//	Set to from
                    SetM_AttributeSetInstanceTo_ID(GetM_AttributeSetInstance_ID());
                else
                {
                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        //MProduct product = GetProduct();
                        if (product != null
                            && product.GetM_AttributeSet_ID() != 0)
                        {
                            //MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                            //if (mas.IsInstanceAttribute() 
                            //    && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                            //{
                            //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "M_AttributeSetInstanceTo_ID"));
                            //    return false;
                            //}

                            // Code Addeded by Bharat as Discussed with Mukesh Sir
                            if (String.IsNullOrEmpty(GetDTD001_AttributeNumber()))
                            {
                                return true;
                            }
                            else
                            {
                                if (GetDTD001_AttributeNumber() == "" || GetDTD001_AttributeNumber() == null)
                                {
                                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "DTD001_AttributeNumber"));

                                    return false;
                                }
                            }

                            //JID_1422: Check No Of Attributes Are Equal To Quantity Or Less Than

                            int Count = CountAttributes(GetDTD001_AttributeNumber());
                            if (Count != GetQtyEntered())
                            {
                                if (Count > GetQtyEntered())
                                {
                                    log.SaveError("DTD001_MovementAttrbtGreater", "");
                                    return false;
                                }
                                else
                                {
                                    log.SaveError("DTD001_MovementAttrbtLess", "");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (product != null)
                            {
                                if (product.GetM_AttributeSet_ID() == 0 && (GetDTD001_AttributeNumber() == "" || GetDTD001_AttributeNumber() == null))
                                    return true;
                                else
                                {
                                    //log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "DTD001_AttributeNumber"));
                                    //ShowMessage.Info("a", true, "Product is not of Attribute Type", null); 
                                    log.SaveError("Product is not of Attribute Type", Msg.GetElement(GetCtx(), "DTD001_AttributeNumber"));
                                    return false;
                                }

                                //Check No Of Attributes Are Equal To Quantity Or Less Than

                                //int Count = CountAttributes(GetDTD001_AttributeNumber());
                                //if (Count != GetMovementQty())
                                //{
                                //    if (Count > GetMovementQty())
                                //    {
                                //        log.SaveError("Error", Msg.GetMsg(GetCtx(), "DTD001_MovementAttrbtGreater"));
                                //        return false;
                                //    }
                                //    else
                                //    {
                                //        log.SaveError("Error", Msg.GetMsg(GetCtx(), "DTD001_MovementAttrbtLess"));
                                //        return false;
                                //    }
                                //}
                            }

                        }

                    }
                    else
                    {
                        //MProduct product = GetProduct();
                        if (product != null
                            && product.GetM_AttributeSet_ID() != 0)
                        {
                            MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                            if (mas.IsInstanceAttribute()
                                && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                            {
                                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "M_AttributeSetInstanceTo_ID"));
                                return false;
                            }
                        }
                    }
                }
            }    //	ASI
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                // JID_0657: Requistion is without ASI but on move selected the ASI system is minus the Reserved qty from ASI field but not removing the reserved qty without ASI
                _mvlNewAttId = GetM_AttributeSetInstance_ID();
                if (_mvlOldAttId != _mvlNewAttId && !newRecord && GetM_RequisitionLine_ID() != 0)
                {
                    //  Set QtyReserved On Storage Correspng to New attributesetinstc_id
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());     // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }

                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), _mvlOldAttId, Get_Trx());            // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), _mvlOldAttId, Get_Trx());             // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() - qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }
                }//vikas

                if (!newRecord && GetM_RequisitionLine_ID() != 0 && GetConfirmedQty() == 0 && String.IsNullOrEmpty(GetDescription()))
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), Get_Trx());        // Trx used to handle query stuck problem
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + (GetMovementQty() - qtyReserved));
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());             // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + (GetMovementQty() - qtyReserved));
                    if (!storage.Save())
                    {
                        return false;
                    }
                }

                if (newRecord && GetM_RequisitionLine_ID() != 0 && GetDescription() != "RC")
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), Get_Trx());            // Trx used to handle query stuck problem
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + GetMovementQty());
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());         // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + GetMovementQty());
                    if (!storage.Save())
                    {
                        return false;
                    }
                }
            }

            // 17-April-2017 Amit
            // If Provision already occured against any product then need to give message to client that 
            //Inventory Provision on the selected Product/s is/are done with the Specific Organization.
            if (Env.IsModuleInstalled("VA024_"))
            {
                // checking are we moving product from one warehouse to other warehouse
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT CASE WHEN ((SELECT CASE WHEN o.IsLegalEntity = 'Y' THEN w.AD_Org_ID
                 ELSE (SELECT AD_Org_ID FROM AD_Org WHERE Ad_Org_id = o.LegalEntityOrg ) END
                 FROM m_warehouse w INNER JOIN ad_org o ON o.AD_Org_ID = w.AD_Org_ID WHERE w.m_warehouse_id = m.DTD001_MWarehouseSource_ID)) =
                 (SELECT  CASE WHEN o2.IsLegalEntity = 'Y' THEN w2.AD_Org_ID 
                 ELSE (SELECT AD_Org_ID FROM AD_Org WHERE Ad_Org_id = o2.LegalEntityOrg) END
                 FROM m_warehouse w2 INNER JOIN ad_org o2 ON o2.AD_Org_ID = w2.AD_Org_ID WHERE M_Warehouse_ID = m.M_Warehouse_ID )
                 THEN 0 ELSE (SELECT ad_org_id FROM m_warehouse WHERE M_Warehouse_ID = m.M_Warehouse_ID ) END AS result FROM m_movement m WHERE m_movement_id = " + GetM_Movement_ID(), null, Get_Trx())) > 0)
                {
                    //checking product is provisioned or not
                    if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM va024_t_obsoleteinventory WHERE ISACTIVE = 'Y' AND  AD_Org_ID = " + GetAD_Org_ID() +
                           @" AND M_Product_ID = " + GetM_Product_ID() + "  AND NVL(M_AttributeSetInstance_ID , 0) = " + Util.GetValueOfInt(GetM_AttributeSetInstance_ID()))) > 0)
                    {
                        log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "VA024_AlreadyProvisionConvey"));
                    }
                }
            }
            return true;
        }

        protected override bool BeforeDelete()
        {
            qtyReserved = GetMovementQty();
            return true;
        }
        protected override bool AfterDelete(bool success)
        {
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                if (GetM_RequisitionLine_ID() != 0)
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), Get_Trx());        // Trx used to handle query stuck problem
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() - qtyReserved);
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), 0, Get_Trx());         // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() - qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static int CountAttributes(string Attributes)
        {
            int n = 0;
            if (Attributes != null)
            {
                foreach (var c in Attributes)
                {
                    if ((c == '\n') || (c == '\r')) n++;
                }

            }
            return n + 1;
        }


        /// <summary>
        /// Get Base value for Cost Distribution
        /// </summary>
        /// <param name="CostDistribution">Cost Distribution</param>
        /// <returns>base number</returns>
        public Decimal GetBase(String CostDistribution)
        {
            if (MLandedCost.LANDEDCOSTDISTRIBUTION_Costs.Equals(CostDistribution))
            {
                //log.Severe("Not Implemented yet - Cost");
                return Decimal.Multiply(GetMovementQty(), GetToPostCurrentCostPrice());
            }
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Line.Equals(CostDistribution))
                return Env.ONE;
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Quantity.Equals(CostDistribution))
                return GetMovementQty();
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Volume.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetMovementQty(), (Decimal)product.GetVolume());
            }
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Weight.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetMovementQty(), product.GetWeight());
            }
            log.Severe("Invalid Criteria: " + CostDistribution);
            return Env.ZERO;
        }
    }
}
