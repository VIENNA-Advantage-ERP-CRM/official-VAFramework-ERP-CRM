/********************************************************
 * Module Name    : 
 * Purpose        : Inventory Move Line Model
 * Class Used     : X_VAM_InvTrf_Line
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
    public class MVAMInvTrfLine : X_VAM_InvTrf_Line
    {
        /** Parent							*/
        private MVAMInventoryTransfer _parent = null;
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
        /// <param name="VAM_InvTrf_Line_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMInvTrfLine(Ctx ctx, int VAM_InvTrf_Line_ID, Trx trxName)
            : base(ctx, VAM_InvTrf_Line_ID, trxName)
        {
            if (VAM_InvTrf_Line_ID == 0)
            {
                //	SetVAM_LocatorTo_ID (0);	// @VAM_LocatorTo_ID@
                //	SetVAM_Locator_ID (0);	// @VAM_Locator_ID@
                //	SetVAM_InvTrf_Line_ID (0);			
                //	SetLine (0);	
                //	SetVAM_Product_ID (0);
                SetVAM_PFeature_SetInstance_ID(0);	//	ID
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
        public MVAMInvTrfLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MVAMInvTrfLine(MVAMInventoryTransfer parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_InventoryTransfer_ID(parent.GetVAM_InventoryTransfer_ID());
        }

        /// <summary>
        /// Get AttributeSetInstance To
        /// </summary>
        /// <returns>asi</returns>
        public new int GetVAM_PFeature_SetInstanceTo_ID()
        {
            int VAM_PFeature_SetInstanceTo_ID = base.GetVAM_PFeature_SetInstanceTo_ID();
            if (VAM_PFeature_SetInstanceTo_ID == 0)
                VAM_PFeature_SetInstanceTo_ID = base.GetVAM_PFeature_SetInstance_ID();
            return VAM_PFeature_SetInstanceTo_ID;
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
            if (GetVAM_Product_ID() != 0)
                return MProduct.Get(GetCtx(), GetVAM_Product_ID());
            return null;
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldVAM_Product_ID">old value</param>
        /// <param name="newVAM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetVAM_Product_ID(String oldVAM_Product_ID, String newVAM_Product_ID, int windowNo)
        {
            if (newVAM_Product_ID == null || newVAM_Product_ID.Length == 0)
                return;
            int VAM_Product_ID = int.Parse(newVAM_Product_ID);
            if (VAM_Product_ID == 0)
                return;
            //
            base.SetVAM_Product_ID(VAM_Product_ID);
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_Product_ID") == VAM_Product_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID") != 0)
                SetVAM_PFeature_SetInstance_ID(GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID"));
            else
                SetVAM_PFeature_SetInstance_ID(0);
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
        public MVAMInventoryTransfer GetParent()
        {
            if (_parent == null)
                _parent = new MVAMInventoryTransfer(GetCtx(), GetVAM_InventoryTransfer_ID(), Get_TrxName());
            return _parent;
        }


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            Decimal VA024_ProvisionPrice = 0;
            MProduct product = MProduct.Get(GetCtx(), GetVAM_Product_ID());

            // chck pallet Functionality applicable or not
            bool isContainrApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            // Get Old Value of AttributeSetInstance_ID
            _mvlOldAttId = Util.GetValueOfInt(Get_ValueOld("VAM_PFeature_SetInstance_ID"));

            //	Set Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM VAM_InvTrf_Line WHERE VAM_InventoryTransfer_ID=" + GetVAM_InventoryTransfer_ID();
                int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                SetLine(ii);
            }

            // JID_0775: System is not checking on move line that the attribute set instance is mandatory.
            if (GetVAM_PFeature_SetInstance_ID() == 0)
            {
                if (product != null && product.GetVAM_PFeature_Set_ID() != 0)
                {
                    MVAMPFeatureSet mas = MVAMPFeatureSet.Get(GetCtx(), product.GetVAM_PFeature_Set_ID());
                    if (mas.IsMandatory() || mas.IsMandatoryAlways())
                    {
                        log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "VAM_PFeature_SetInstance_ID"));
                        return false;
                    }
                }
            }

            // Check Locator For Header Warehouse
            MVAMInventoryTransfer mov = new MVAMInventoryTransfer(GetCtx(), GetVAM_InventoryTransfer_ID(), Get_TrxName());
            MVAMLocator loc = new MVAMLocator(GetCtx(), GetVAM_Locator_ID(), Get_TrxName());
            Tuple<string, string, string> aInfo = null;
            if (Env.HasModulePrefix("DTD001_", out aInfo))
            {
                if (mov.GetDTD001_MWarehouseSource_ID() == loc.GetVAM_Warehouse_ID())
                {

                }
                else
                {
                    String sql = "SELECT VAM_Locator_ID FROM VAM_Locator WHERE VAM_Warehouse_ID=" + mov.GetDTD001_MWarehouseSource_ID() + " AND IsDefault = 'Y'";
                    int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                    SetVAM_Locator_ID(ii);
                }
            }

            // when we try to move product from container to container which are in same locator then no need to check this case
            if (GetVAM_Locator_ID() == GetVAM_LocatorTo_ID() &&
                (Get_ColumnIndex("VAM_ProductContainer_ID") > 0 && Get_ColumnIndex("Ref_VAM_ProductContainerTo_ID") > 0 &&
                 !(GetVAM_ProductContainer_ID() > 0 || GetRef_VAM_ProductContainerTo_ID() > 0)))
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "'From @VAM_Locator_ID@' and '@VAM_LocatorTo_ID@' cannot be same."));//change message according to requirement
                return false;
            }

            if (Env.Signum(GetMovementQty()) == 0 && Util.GetValueOfInt(GetTargetQty()) == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "MovementQty"));
                return false;
            }

            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("VAM_Product_ID")) != GetVAM_Product_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ProdNotChanged"));
                    return false;
                }
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("VAM_Locator_ID")) != GetVAM_Locator_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_LocatorNotChanged"));
                    return false;
                }
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("VAM_RequisitionLine_ID")) != GetVAM_RequisitionLine_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ReqNotChanged"));
                    return false;
                }
            }
            //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA203_'", null, null)) > 0)
            if (Env.IsModuleInstalled("VA203_"))
            {
                if (GetVAM_RequisitionLine_ID() > 0)
                {
                    MRequisitionLine reqline = new MRequisitionLine(GetCtx(), GetVAM_RequisitionLine_ID(), Get_Trx());        // Trx used to handle query stuck problem
                    if (GetVAM_PFeature_SetInstance_ID() != reqline.GetVAM_PFeature_SetInstance_ID())
                    {
                        log.SaveError("Message", Msg.GetMsg(GetCtx(), "VA203_AttributeInstanceMustBeSame"));
                        return false;
                    }
                }
            }
            // IF Doc Status = InProgress then No record Save
            MVAMInventoryTransfer move = new MVAMInventoryTransfer(GetCtx(), GetVAM_InventoryTransfer_ID(), Get_Trx());        // Trx used to handle query stuck problem
            if (newRecord && move.GetDocStatus() == "IP")
            {
                log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_CannotCreate"));
                return false;
            }

            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetQtyEntered(GetQtyEntered());

            // change to set Converted Quantity in Movement quantity if there is differnce in UOM of Base Product and UOM Selected on line
            if (newRecord || Is_ValueChanged("QtyEntered") || Is_ValueChanged("VAB_UOM_ID"))
            {
                Decimal? qty = Util.GetValueOfDecimal(Get_Value("QtyEntered"));
                if (product.GetVAB_UOM_ID() != Util.GetValueOfInt(Get_Value("VAB_UOM_ID")))
                {
                    SetMovementQty(MUOMConversion.ConvertProductFrom(GetCtx(), GetVAM_Product_ID(), Util.GetValueOfInt(Get_Value("VAB_UOM_ID")), Util.GetValueOfDecimal(Get_Value("QtyEntered"))));
                }
            }

            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetMovementQty(GetMovementQty());

            string qry;
            if (!mov.IsProcessing() || newRecord)
            {
                MWarehouse wh = null; MWarehouse whTo = null;
                wh = MWarehouse.Get(GetCtx(), mov.GetDTD001_MWarehouseSource_ID());
                whTo = MWarehouse.Get(GetCtx(), MVAMLocator.Get(GetCtx(), GetVAM_LocatorTo_ID()).GetVAM_Warehouse_ID());

                qry = "SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM VAM_Storage where VAM_Locator_id=" + GetVAM_Locator_ID() + " and VAM_Product_id=" + GetVAM_Product_ID();
                if (GetDTD001_AttributeNumber() == null || GetVAM_PFeature_SetInstance_ID() > 0)
                {
                    qry += " AND NVL(VAM_PFeature_SetInstance_ID , 0) =" + GetVAM_PFeature_SetInstance_ID();
                }
                OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry));

                qry = "SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM VAM_Storage where VAM_Locator_id=" + GetVAM_LocatorTo_ID() + " and VAM_Product_id=" + GetVAM_Product_ID();
                qry += " AND NVL(VAM_PFeature_SetInstance_ID, 0) =" + GetVAM_PFeature_SetInstance_ID();
                OnHandQtyTo = Convert.ToDecimal(DB.ExecuteScalar(qry));

                // SI_0635 : System is giving error of insufficient qty if disallow is true in TO warehouse and false in From warehouse
                // when record is in completed & closed stage - then no need to check qty availablity in warehouse
                if ((wh.IsDisallowNegativeInv() || whTo.IsDisallowNegativeInv()) &&
                    (!(move.GetDocStatus() == "CO" || move.GetDocStatus() == "CL" || move.GetDocStatus() == "RE" || move.GetDocStatus() == "VO")))
                {
                    // pick container current qty from transaction based on locator / product / ASI / Container / Movement Date 
                    if (isContainrApplicable && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        //                    qry = @"SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.VAM_Inv_Trx_ID) AS CurrentQty FROM VAM_Inv_Trx t 
                        //                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) +
                        //                                " AND t.VAF_Client_ID = " + GetVAF_Client_ID() +
                        //                                @" AND t.VAM_Locator_ID = " + (move.IsReversal() && GetMovementQty() < 0 ? GetVAM_LocatorTo_ID() : GetVAM_Locator_ID()) +
                        //                                " AND t.VAM_Product_ID = " + GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + GetVAM_PFeature_SetInstance_ID() +
                        //                                " AND NVL(t.VAM_ProductContainer_ID, 0) = " + (move.IsReversal() && !IsMoveFullContainer() && GetMovementQty() < 0 ? GetRef_VAM_ProductContainerTo_ID() : GetVAM_ProductContainer_ID());
                        //                    containerQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion

                        //                    qry = @"SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.VAM_Inv_Trx_ID) AS CurrentQty FROM VAM_Inv_Trx t 
                        //                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) +
                        //                               " AND t.VAF_Client_ID = " + GetVAF_Client_ID() +
                        //                               @" AND t.VAM_Locator_ID = " + (move.IsReversal() && GetMovementQty() < 0 ? GetVAM_Locator_ID() : GetVAM_LocatorTo_ID()) +
                        //                               " AND t.VAM_Product_ID = " + GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + GetVAM_PFeature_SetInstance_ID() +
                        //                               " AND NVL(t.VAM_ProductContainer_ID, 0) = " + (move.IsReversal() && !IsMoveFullContainer() && GetMovementQty() < 0 ? GetVAM_ProductContainer_ID() : GetRef_VAM_ProductContainerTo_ID());
                        //                    containerQtyTo = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion

                        qry = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                                                INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) +
                                    " AND t.VAF_Client_ID = " + GetVAF_Client_ID() +
                                    @" AND t.VAM_Locator_ID = " + GetVAM_Locator_ID() +
                                    " AND t.VAM_Product_ID = " + GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + GetVAM_PFeature_SetInstance_ID() +
                                    " AND NVL(t.VAM_ProductContainer_ID, 0) = " + GetVAM_ProductContainer_ID();
                        containerQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion

                        qry = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC)  AS CurrentQty FROM VAM_Inv_Trx t 
                                                INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(move.GetMovementDate(), true) +
                                   " AND t.VAF_Client_ID = " + GetVAF_Client_ID() +
                                   @" AND t.VAM_Locator_ID = " + GetVAM_LocatorTo_ID() +
                                   " AND t.VAM_Product_ID = " + GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + GetVAM_PFeature_SetInstance_ID() +
                                   " AND NVL(t.VAM_ProductContainer_ID, 0) = " + GetRef_VAM_ProductContainerTo_ID();
                        containerQtyTo = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, null));  // dont use Transaction here - otherwise impact goes wrong on completion

                    }

                    if (wh.IsDisallowNegativeInv() && (OnHandQty - GetMovementQty()) < 0)
                    {
                        // check for From Locator
                        log.SaveError("Info", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                        return false;
                    }
                    else if (isContainrApplicable && wh.IsDisallowNegativeInv() && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && (containerQty - GetMovementQty()) < 0)
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
                    else if (isContainrApplicable && whTo.IsDisallowNegativeInv() && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && (containerQtyTo + GetMovementQty()) < 0)
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
                //qry = "SELECT   NVL(SUM(NVL(QtyOnHand,0)- qtyreserved),0) AS QtyAvailable  FROM VAM_Storage where VAM_Locator_id=" + GetVAM_Locator_ID() + " and VAM_Product_id=" + GetVAM_Product_ID();
                //if (GetVAM_PFeature_SetInstance_ID() != 0)
                //{
                //    qry += " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + GetVAM_PFeature_SetInstance_ID();
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
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT CASE WHEN ((SELECT CASE WHEN o.IsLegalEntity = 'Y' THEN w.VAF_Org_ID
                 ELSE (SELECT VAF_Org_ID FROM VAF_Org WHERE vaf_org_id = o.LegalEntityOrg ) END
                 FROM VAM_Warehouse w INNER JOIN vaf_org o ON o.VAF_Org_ID = w.VAF_Org_ID WHERE w.VAM_Warehouse_id = m.DTD001_MWarehouseSource_ID)) =
                 (SELECT  CASE WHEN o2.IsLegalEntity = 'Y' THEN w2.VAF_Org_ID 
                 ELSE (SELECT VAF_Org_ID FROM VAF_Org WHERE vaf_org_id = o2.LegalEntityOrg) END
                 FROM VAM_Warehouse w2 INNER JOIN vaf_org o2 ON o2.VAF_Org_ID = w2.VAF_Org_ID WHERE VAM_Warehouse_ID = m.VAM_Warehouse_ID )
                 THEN 0 ELSE (SELECT vaf_org_id FROM VAM_Warehouse WHERE VAM_Warehouse_ID = m.VAM_Warehouse_ID ) END AS result FROM VAM_InventoryTransfer m WHERE VAM_InventoryTransfer_id = " + GetVAM_InventoryTransfer_ID(), null, Get_Trx())) > 0)
                {
                    string qry1 = @"SELECT  SUM(o.VA024_UnitPrice)   FROM VA024_t_ObsoleteInventory o 
                                  WHERE o.IsActive = 'Y' AND  o.VAM_Product_ID = " + GetVAM_Product_ID() + @" and 
                                  ( o.VAM_PFeature_SetInstance_ID = " + GetVAM_PFeature_SetInstance_ID() + @" OR o.VAM_PFeature_SetInstance_ID IS NULL )" +
                             " AND o.VAF_Org_ID = " + GetVAF_Org_ID();
                    VA024_ProvisionPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                    SetVA024_UnitPrice(Util.GetValueOfDecimal(VA024_ProvisionPrice * GetMovementQty()));

                    // is used to get cost of binded cost method / costing level of primary accounting schema
                    Decimal cost = MVAMProductCost.GetproductCosts(move.GetVAF_Client_ID(), move.GetVAF_Org_ID(), GetVAM_Product_ID(),
                         GetVAM_PFeature_SetInstance_ID(), Get_Trx(), move.GetDTD001_MWarehouseSource_ID());
                    SetVA024_CostPrice((cost - VA024_ProvisionPrice) * GetMovementQty());
                }
            }

            //	Mandatory Instance
            if (GetVAM_PFeature_SetInstanceTo_ID() == 0)
            {
                if (GetVAM_PFeature_SetInstance_ID() != 0)	//	Set to from
                    SetVAM_PFeature_SetInstanceTo_ID(GetVAM_PFeature_SetInstance_ID());
                else
                {
                    if (Env.HasModulePrefix("DTD001_", out mInfo))
                    {
                        //MProduct product = GetProduct();
                        if (product != null
                            && product.GetVAM_PFeature_Set_ID() != 0)
                        {
                            //MVAMPFeatureSet mas = MVAMPFeatureSet.Get(GetCtx(), product.GetVAM_PFeature_Set_ID());
                            //if (mas.IsInstanceAttribute() 
                            //    && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                            //{
                            //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "VAM_PFeature_SetInstanceTo_ID"));
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
                                if (product.GetVAM_PFeature_Set_ID() == 0 && (GetDTD001_AttributeNumber() == "" || GetDTD001_AttributeNumber() == null))
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
                            && product.GetVAM_PFeature_Set_ID() != 0)
                        {
                            MVAMPFeatureSet mas = MVAMPFeatureSet.Get(GetCtx(), product.GetVAM_PFeature_Set_ID());
                            if (mas.IsInstanceAttribute()
                                && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                            {
                                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "VAM_PFeature_SetInstanceTo_ID"));
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
                _mvlNewAttId = GetVAM_PFeature_SetInstance_ID();
                if (_mvlOldAttId != _mvlNewAttId && !newRecord && GetVAM_RequisitionLine_ID() != 0)
                {
                    //  Set QtyReserved On Storage Correspng to New attributesetinstc_id
                    storage = MStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());     // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }

                    storage = MStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), _mvlOldAttId, Get_Trx());            // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), _mvlOldAttId, Get_Trx());             // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() - qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }
                }//vikas

                if (!newRecord && GetVAM_RequisitionLine_ID() != 0 && GetConfirmedQty() == 0 && String.IsNullOrEmpty(GetDescription()))
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetVAM_RequisitionLine_ID(), Get_Trx());        // Trx used to handle query stuck problem
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + (GetMovementQty() - qtyReserved));
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());             // Trx used to handle query stuck problem
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + (GetMovementQty() - qtyReserved));
                    if (!storage.Save())
                    {
                        return false;
                    }
                }

                if (newRecord && GetVAM_RequisitionLine_ID() != 0 && GetDescription() != "RC")
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetVAM_RequisitionLine_ID(), Get_Trx());            // Trx used to handle query stuck problem
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + GetMovementQty());
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());         // Trx used to handle query stuck problem
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
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT CASE WHEN ((SELECT CASE WHEN o.IsLegalEntity = 'Y' THEN w.VAF_Org_ID
                 ELSE (SELECT VAF_Org_ID FROM VAF_Org WHERE vaf_org_id = o.LegalEntityOrg ) END
                 FROM VAM_Warehouse w INNER JOIN vaf_org o ON o.VAF_Org_ID = w.VAF_Org_ID WHERE w.VAM_Warehouse_id = m.DTD001_MWarehouseSource_ID)) =
                 (SELECT  CASE WHEN o2.IsLegalEntity = 'Y' THEN w2.VAF_Org_ID 
                 ELSE (SELECT VAF_Org_ID FROM VAF_Org WHERE vaf_org_id = o2.LegalEntityOrg) END
                 FROM VAM_Warehouse w2 INNER JOIN vaf_org o2 ON o2.VAF_Org_ID = w2.VAF_Org_ID WHERE VAM_Warehouse_ID = m.VAM_Warehouse_ID )
                 THEN 0 ELSE (SELECT vaf_org_id FROM VAM_Warehouse WHERE VAM_Warehouse_ID = m.VAM_Warehouse_ID ) END AS result FROM VAM_InventoryTransfer m WHERE VAM_InventoryTransfer_id = " + GetVAM_InventoryTransfer_ID(), null, Get_Trx())) > 0)
                {
                    //checking product is provisioned or not
                    if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM va024_t_obsoleteinventory WHERE ISACTIVE = 'Y' AND  VAF_Org_ID = " + GetVAF_Org_ID() +
                           @" AND VAM_Product_ID = " + GetVAM_Product_ID() + "  AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + Util.GetValueOfInt(GetVAM_PFeature_SetInstance_ID()))) > 0)
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
                if (GetVAM_RequisitionLine_ID() != 0)
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetVAM_RequisitionLine_ID(), Get_Trx());        // Trx used to handle query stuck problem
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() - qtyReserved);
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), 0, Get_Trx());         // Trx used to handle query stuck problem
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
            if (MVABLCost.LANDEDCOSTDISTRIBUTION_Costs.Equals(CostDistribution))
            {
                //	TODO Costs!
                log.Severe("Not Implemented yet - Cost");
                return Env.ZERO;
            }
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Line.Equals(CostDistribution))
                return Env.ONE;
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Quantity.Equals(CostDistribution))
                return GetMovementQty();
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Volume.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetMovementQty(), (Decimal)product.GetVolume());
            }
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Weight.Equals(CostDistribution))
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
