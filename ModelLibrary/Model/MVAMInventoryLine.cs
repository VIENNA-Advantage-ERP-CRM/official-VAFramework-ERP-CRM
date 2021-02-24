/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_InventoryLine
 * Chronological Development
 * Veena Pandey     22-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMInventoryLine : X_VAM_InventoryLine
    {
        //	Logger				
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInventoryLine).FullName);
        /** Manually created				*/
        private bool _isManualEntry = true;
        /** Parent							*/
        private MVAMInventory _parent = null;
        /** Product							*/
        private MVAMProduct _product = null;
        public Decimal? OnHandQty = 0;
        private Decimal? containerQty = 0;
        private decimal qtyReserved = 0;
        private MVAMStorage storage = null;
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_InventoryLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMInventoryLine(Ctx ctx, int VAM_InventoryLine_ID, Trx trxName)
            : base(ctx, VAM_InventoryLine_ID, trxName)
        {
            if (VAM_InventoryLine_ID == 0)
            {
                //	setVAM_Inventory_ID (0);			//	Parent
                //	setVAM_InventoryLine_ID (0);		//	PK
                //	setVAM_Locator_ID (0);			//	FK
                SetLine(0);
                //	setVAM_Product_ID (0);			//	FK
                SetVAM_PFeature_SetInstance_ID(0);	//	FK
                SetInventoryType(INVENTORYTYPE_InventoryDifference);
                SetQtyBook(Env.ZERO);
                SetQtyCount(Env.ZERO);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAMInventoryLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Detail Constructor.
        /// Locator/Product/AttributeSetInstance must be unique
        /// </summary>
        /// <param name="inventory">parent</param>
        /// <param name="VAM_Locator_ID">locator</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">instance</param>
        /// <param name="qtyBook">book value</param>
        /// <param name="qtyCount">count value</param>
        public MVAMInventoryLine(MVAMInventory inventory, int VAM_Locator_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, Decimal qtyBook, Decimal qtyCount)
            : this(inventory.GetCtx(), 0, inventory.Get_TrxName())
        {
            if (inventory.Get_ID() == 0)
                throw new ArgumentException("Header not saved");
            _parent = inventory;
            SetVAM_Inventory_ID(inventory.GetVAM_Inventory_ID());		//	Parent
            SetClientOrg(inventory.GetVAF_Client_ID(), inventory.GetVAF_Org_ID());
            SetVAM_Locator_ID(VAM_Locator_ID);		//	FK
            SetVAM_Product_ID(VAM_Product_ID);		//	FK
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            //
            // if (qtyBook != null)
            SetQtyBook(qtyBook);
            if (qtyCount != null && Env.Signum(qtyCount) != 0)
                SetQtyCount(qtyCount);
            _isManualEntry = false;
        }

        /// <summary>
        /// Get Inventory Line with parameters
        /// </summary>
        /// <param name="inventory">inventory</param>
        /// <param name="VAM_Locator_ID">locator</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">asi</param>
        /// <returns>line or null</returns>
        public static MVAMInventoryLine Get(MVAMInventory inventory, int VAM_Locator_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID)
        {
            MVAMInventoryLine retValue = null;
            String sql = "SELECT * FROM VAM_InventoryLine "
                + "WHERE VAM_Inventory_ID=@invenid AND VAM_Locator_ID=@locid"
                + " AND VAM_Product_ID=@prodid AND VAM_PFeature_SetInstance_ID=@asiid";
            try
            {
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@invenid", inventory.GetVAM_Inventory_ID());
                param[1] = new SqlParameter("@locid", VAM_Locator_ID);
                param[2] = new SqlParameter("@prodid", VAM_Product_ID);
                param[3] = new SqlParameter("@asiid", VAM_PFeature_SetInstance_ID);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        retValue = new MVAMInventoryLine(inventory.GetCtx(), dr, inventory.Get_TrxName());
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            return retValue;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>true</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            //	Create MA (not to create MA on Save -- we will create MA on Completion only)
            //if (newRecord && success
            //    && _isManualEntry && GetVAM_PFeature_SetInstance_ID() == 0)
            //    CreateMA(true);

            if (!IsInternalUse())
            {
                MVAMInventory inv = new MVAMInventory(GetCtx(), GetVAM_Inventory_ID(), Get_Trx());
                inv.SetIsAdjusted(false);
                if (!inv.Save())
                {

                }
            }
            else                // SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
            {
                if (Env.IsModuleInstalled("DTD001_"))
                {
                    if (!newRecord && GetVAM_RequisitionLine_ID() != 0)
                    {
                        MVAMRequisitionLine requisition = new MVAMRequisitionLine(GetCtx(), GetVAM_RequisitionLine_ID(), Get_Trx());
                        requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + (GetQtyInternalUse() - qtyReserved));
                        if (!requisition.Save())
                        {
                            return false;
                        }
                        storage = MVAMStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                        if (storage == null)
                        {
                            storage = MVAMStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                        }
                        storage.SetQtyReserved(storage.GetQtyReserved() + (GetQtyInternalUse() - qtyReserved));
                        if (!storage.Save())
                        {
                            return false;
                        }
                    }

                    if (newRecord && GetVAM_RequisitionLine_ID() != 0 && GetDescription() != "RC")
                    {
                        MVAMRequisitionLine requisition = new MVAMRequisitionLine(GetCtx(), GetVAM_RequisitionLine_ID(), Get_Trx());
                        requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + GetQtyInternalUse());
                        if (!requisition.Save())
                        {
                            return false;
                        }
                        storage = MVAMStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                        if (storage == null)
                        {
                            storage = MVAMStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                        }
                        storage.SetQtyReserved(storage.GetQtyReserved() + GetQtyInternalUse());
                        if (!storage.Save())
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if can be deleted</returns>
        /// SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
        protected override bool BeforeDelete()
        {
            if (IsInternalUse())
            {
                qtyReserved = GetQtyInternalUse();
            }
            return true;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="sucess">new</param>
        /// <returns>true if can be deleted</returns>
        /// SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
        protected override bool AfterDelete(bool success)
        {
            if (Env.IsModuleInstalled("DTD001_") && IsInternalUse())
            {
                if (GetVAM_RequisitionLine_ID() != 0)
                {
                    MVAMRequisitionLine requisition = new MVAMRequisitionLine(GetCtx(), GetVAM_RequisitionLine_ID(), Get_Trx());        // Trx used to handle query stuck problem
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() - qtyReserved);
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MVAMStorage.Get(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());      // Trx used to handle query stuck problem
                    if (storage == null)
                    {
                        storage = MVAMStorage.GetCreate(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), Get_Trx());         // Trx used to handle query stuck problem
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

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // chck pallet Functionality applicable or not
            bool isContainrApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            Decimal VA024_ProvisionPrice = 0;
            MVAMInventory inventory = new MVAMInventory(GetCtx(), GetVAM_Inventory_ID(), Get_Trx());
            MVAMProduct product = MVAMProduct.Get(GetCtx(), GetVAM_Product_ID());
            if (newRecord && _isManualEntry)
            {
                //	Product requires ASI
                if (GetVAM_PFeature_SetInstance_ID() == 0)
                {
                    if (product.GetVAM_PFeature_Set_ID() != 0)
                    {
                        MVAMPFeatureSet mas = MVAMPFeatureSet.Get(GetCtx(), product.GetVAM_PFeature_Set_ID());
                        //uncomment by Amit on behalf of Mandeep 7-3-2016
                        //if (mas.IsInstanceAttribute()
                        //    && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                        if (mas.IsMandatory() || mas.IsMandatoryAlways())
                        {
                            log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "VAM_PFeature_SetInstance_ID"));
                            return false;
                        }
                    }
                }	//	No ASI
            }	//	new or manual

            // not to create Internal use Inventory with -ve qty -- but during reversal system will create record with -ve qty
            // duing reversal -- ReversalDoc_ID contain refernce o  orignal record id
            if (IsInternalUse() && Get_ColumnIndex("ReversalDoc_ID") > 0 && GetReversalDoc_ID() == 0 && GetQtyInternalUse() < 0)
            {
                log.SaveError("", Msg.GetMsg(GetCtx(), "VIS_CantbeNegative"));
                return false;
            }

            //	Set Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM VAM_InventoryLine WHERE VAM_Inventory_ID=" + GetVAM_Inventory_ID();
                int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                SetLine(ii);
            }

            // SI_0644.1 : Enforce UOM Precision - Rounding Quantities
            if (newRecord || Is_ValueChanged("QtyInternalUse"))
                SetQtyInternalUse(GetQtyInternalUse());

            //JID_0680 set quantity according to precision
            if (Is_ValueChanged("VAB_UOM_ID"))
            {
                Set_Value("QtyEntered", Math.Round(Util.GetValueOfDecimal(Get_Value("QtyEntered")), MVABUOM.GetPrecision(GetCtx(), Util.GetValueOfInt(Get_Value("VAB_UOM_ID")))));
            }

            // change to set Converted Quantity in Internal Use Qty and AsonDateQty and difference qty if there is differnce in UOM of Base Product and UOM Selected on line
            if (newRecord || Is_ValueChanged("QtyEntered") || Is_ValueChanged("VAB_UOM_ID"))
            {
                Decimal? qty = Util.GetValueOfDecimal(Get_Value("QtyEntered"));
                if (product.GetVAB_UOM_ID() != Util.GetValueOfInt(Get_Value("VAB_UOM_ID")))
                {
                    qty = MVABUOMConversion.ConvertProductFrom(GetCtx(), GetVAM_Product_ID(), Util.GetValueOfInt(Get_Value("VAB_UOM_ID")), Util.GetValueOfDecimal(Get_Value("QtyEntered")));
                    if (IsInternalUse())
                        SetQtyInternalUse(qty);
                    else
                    {
                        //JID_1392
                        if (GetAdjustmentType() == ADJUSTMENTTYPE_AsOnDateCount)
                        {
                            SetAsOnDateCount(qty);
                            SetDifferenceQty(Decimal.Negate(qty.Value));
                        }
                        else if (GetAdjustmentType() == ADJUSTMENTTYPE_QuantityDifference)
                        {
                            SetAsOnDateCount(Decimal.Negate(qty.Value));
                            SetDifferenceQty(qty);
                        }
                    }
                }
            }

            // SI_0644 - As on date and difference should be according to the precision of UOM attached.
            if (newRecord || Is_ValueChanged("AsOnDateCount"))
            {
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    SetAsOnDateCount(Decimal.Round(GetAsOnDateCount(), precision, MidpointRounding.AwayFromZero));
                }
            }

            // SI_0644 - As on date and difference should be according to the precision of UOM attached.
            if (newRecord || Is_ValueChanged("DifferenceQty"))
            {
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    SetDifferenceQty(Decimal.Round(GetDifferenceQty(), precision, MidpointRounding.AwayFromZero));
                }
            }

            // SI_0682_1 Need to update the reserved qty on requisition line by internal use line save aslo and should work as work in inventory move.
            if (Env.IsModuleInstalled("DTD001_") && IsInternalUse())
            {
                qtyReserved = Util.GetValueOfDecimal(Get_ValueOld("QtyInternalUse"));
            }

            // no need to check when record is in processing
            if (!inventory.IsProcessing() || newRecord)
            {
                int VAM_Warehouse_ID = 0; MVAMWarehouse wh = null;
                string qry = "select VAM_Warehouse_id from VAM_Locator where VAM_Locator_id=" + GetVAM_Locator_ID();
                VAM_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, Get_TrxName()));

                wh = MVAMWarehouse.Get(GetCtx(), VAM_Warehouse_ID);
                qry = "SELECT QtyOnHand FROM VAM_Storage where VAM_Locator_id=" + GetVAM_Locator_ID() + " and VAM_Product_id=" + GetVAM_Product_ID() +
                      " AND NVL(VAM_PFeature_SetInstance_ID, 0)=" + GetVAM_PFeature_SetInstance_ID();
                OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry, null, Get_TrxName()));

                // when record is in completed & closed stage - then no need to check qty availablity in warehouse
                if (wh.IsDisallowNegativeInv() == true &&
                    (!(inventory.GetDocStatus() == "CO" || inventory.GetDocStatus() == "CL" ||
                       inventory.GetDocStatus() == "RE" || inventory.GetDocStatus() == "VO")))
                {
                    // pick container current qty from transaction based on locator / product / ASI / Container / Movement Date 
                    if (isContainrApplicable && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        qry = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) +
                                    " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + GetVAM_Locator_ID() +
                                    " AND t.VAM_Product_ID = " + GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + GetVAM_PFeature_SetInstance_ID() +
                                    " AND NVL(t.VAM_ProductContainer_ID, 0) = " + GetVAM_ProductContainer_ID();
                        containerQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry, null, null)); // dont use Transaction here - otherwise impact goes wrong on completion
                    }

                    #region DisallowNegativeInv = True
                    if (!IsInternalUse() && GetDifferenceQty() > 0)
                    {
                        if ((OnHandQty - GetDifferenceQty()) < 0)
                        {
                            log.SaveError("", product.GetName() + ", " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                            return false;
                        }
                        if (isContainrApplicable && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && (containerQty - GetDifferenceQty()) < 0)
                        {
                            log.SaveError("", product.GetName() + ", " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQtyContainer") + containerQty);
                            return false;
                        }
                    }
                    else if (IsInternalUse())
                    {
                        if ((OnHandQty - GetQtyInternalUse()) < 0)
                        {
                            log.SaveError("", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                            return false;
                        }
                        if (isContainrApplicable && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && (containerQty - GetQtyInternalUse()) < 0)
                        {
                            log.SaveError("", product.GetName() + ", " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQtyContainer") + containerQty);
                            return false;
                        }
                    }
                    #endregion
                }
            }

            //	Enforce Qty UOM
            if (newRecord || Is_ValueChanged("QtyCount"))
                SetQtyCount(GetQtyCount());

            //	InternalUse Inventory 
            if (IsInternalUse() && Env.Signum(GetQtyInternalUse()) == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "QtyInternalUse"));
                return false;
            }
            if (Env.Signum(GetQtyInternalUse()) != 0)
            {
                if (!INVENTORYTYPE_ChargeAccount.Equals(GetInventoryType()))
                    SetInventoryType(INVENTORYTYPE_ChargeAccount);
                //
                if (GetVAB_Charge_ID() == 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "InternalUseNeedsCharge"));
                    return false;
                }
            }
            else if (INVENTORYTYPE_ChargeAccount.Equals(GetInventoryType()))
            {
                if (GetVAB_Charge_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "VAB_Charge_ID"));
                    return false;
                }
            }
            else if (GetVAB_Charge_ID() != 0)
                SetVAB_Charge_ID(0);

            //	Set VAF_Org to parent if not charge
            if (GetVAB_Charge_ID() == 0)
                SetVAF_Org_ID(GetParent().GetVAF_Org_ID());

            // By Amit for Obsolete Inventory - 25-May-2016
            if (Env.IsModuleInstalled("VA024_"))
            {
                //MVAMInventory inventory = new MVAMInventory(GetCtx(), GetVAM_Inventory_ID(), Get_Trx());
                //shipment and Return to vendor
                if (inventory.IsInternalUse() || (!inventory.IsInternalUse() && (GetQtyBook() - GetQtyCount()) > 0))
                {
                    try
                    {
                        string qry1 = @"SELECT  SUM(o.VA024_UnitPrice)   FROM VA024_t_ObsoleteInventory o 
                                  WHERE o.IsActive = 'Y' AND  o.VAM_Product_ID = " + GetVAM_Product_ID() + @" and 
                                  ( o.VAM_PFeature_SetInstance_ID = " + GetVAM_PFeature_SetInstance_ID() + @" OR o.VAM_PFeature_SetInstance_ID IS NULL )" +
                                 " AND o.VAF_Org_ID = " + GetVAF_Org_ID();
                        //+" AND VAM_Warehouse_ID = " + inventory.GetVAM_Warehouse_ID();
                        VA024_ProvisionPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                        if (!inventory.IsInternalUse() && (GetQtyBook() - GetQtyCount()) > 0)
                        {
                            SetVA024_UnitPrice(Util.GetValueOfDecimal(VA024_ProvisionPrice * (GetQtyBook() - GetQtyCount())));
                        }
                        else
                        {
                            SetVA024_UnitPrice(Util.GetValueOfDecimal(VA024_ProvisionPrice * GetQtyInternalUse()));
                        }

                        if (!inventory.IsInternalUse() && (GetQtyBook() - GetQtyCount()) > 0)
                        {
                            qry1 = @"SELECT (ct.currentcostprice - " + VA024_ProvisionPrice + ") * " + (GetQtyBook() - GetQtyCount());
                        }
                        else
                        {
                            qry1 = @"SELECT (ct.currentcostprice - " + VA024_ProvisionPrice + ") * " + GetQtyInternalUse();
                        }
                        qry1 += @" FROM VAM_Product p  INNER JOIN va024_t_obsoleteinventory oi ON p.VAM_Product_id = oi.VAM_Product_ID
                                 INNER JOIN VAM_ProductCategory pc ON pc.VAM_ProductCategory_ID = p.VAM_ProductCategory_ID
                                 INNER JOIN vaf_client c ON c.VAF_Client_ID = p.vaf_client_ID   INNER JOIN VAF_ClientDetail ci  ON c.VAF_Client_ID = ci.vaf_client_ID
                                 INNER JOIN VAM_ProductCost ct ON ( p.VAM_Product_ID     = ct.VAM_Product_ID  AND ci.VAB_AccountBook1_ID = ct.VAB_AccountBook_ID )
                                 INNER JOIN VAB_AccountBook asch  ON (asch.VAB_AccountBook_ID = ci.VAB_AccountBook1_ID)
                                 INNER JOIN va024_obsoleteinvline oil ON oil.va024_obsoleteinvline_ID = oi.va024_obsoleteinvline_ID ";
                        qry1 += @"    WHERE ct.VAF_Org_ID =  
                          CASE WHEN ( pc.costinglevel IS NOT NULL AND pc.costinglevel = 'O') THEN " + GetVAF_Org_ID() + @" 
                               WHEN ( pc.costinglevel IS NOT NULL AND (pc.costinglevel  = 'C' OR pc.costinglevel = 'B')) THEN 0 
                               WHEN (pc.costinglevel IS NULL AND asch.costinglevel  = 'O') THEN " + GetVAF_Org_ID() + @" 
                               WHEN ( pc.costinglevel IS NULL AND (asch.costinglevel  = 'C' OR asch.costinglevel   = 'B')) THEN 0  END
                          AND ct.VAM_ProductCostElement_id =  
                          CASE WHEN ( pc.costingmethod IS NOT NULL AND pc.costingmethod  != 'C') THEN  (SELECT MIN(VAM_ProductCostElement_id)  FROM VAM_ProductCostElement  
                                     WHERE VAM_ProductCostElement.costingmethod =pc.costingmethod  AND VAM_ProductCostElement.vaf_client_ID  = oi.vaf_client_id  ) 
                                WHEN ( pc.costingmethod IS NOT NULL AND pc.costingmethod = 'C' ) THEN  pc.VAM_ProductCostElement_id 
                                WHEN ( pc.costingmethod IS NULL AND asch.costingmethod  != 'C') THEN  (SELECT MIN(VAM_ProductCostElement_id)  FROM VAM_ProductCostElement 
                                     WHERE VAM_ProductCostElement.costingmethod = asch.costingmethod  AND VAM_ProductCostElement.vaf_client_ID  = oi.vaf_client_id  )
                                WHEN ( pc.costingmethod IS NULL AND asch.costingmethod  = 'C') THEN asch.VAM_ProductCostElement_id  END 
                         AND NVL(ct.VAM_PFeature_SetInstance_ID , 0) =  
                         CASE WHEN ( pc.costinglevel IS NOT NULL AND pc.costinglevel = 'B') THEN " + GetVAM_PFeature_SetInstance_ID() + @" 
                              WHEN ( pc.costinglevel IS NOT NULL AND (pc.costinglevel  = 'C' OR pc.costinglevel = 'O')) THEN 0 
                              WHEN ( pc.costinglevel IS NULL AND asch.costinglevel  = 'B') THEN " + GetVAM_PFeature_SetInstance_ID() + @"
                              WHEN ( pc.costinglevel IS NULL AND (asch.costinglevel  = 'C' OR asch.costinglevel   = 'O')) THEN 0  END 
                         AND p.VAM_Product_ID = " + GetVAM_Product_ID();
                        SetVA024_CostPrice(Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx())));
                    }
                    catch { }
                }
            }

            return true;
        }

        /// <summary>
        /// Create Material Allocations for new Instances
        /// </summary>
        /// <param name="updateQtyBooked"></param>
        public void CreateMA(bool updateQtyBooked)
        {
            int delMA = MVAMInventoryLineMP.DeleteInventoryLineMA(GetVAM_InventoryLine_ID(), Get_TrxName());
            log.Info("DeletedMA=" + delMA);

            MVAMStorage[] storages = MVAMStorage.GetAll(GetCtx(), GetVAM_Product_ID(),
                GetVAM_Locator_ID(), Get_TrxName());
            bool allZeroASI = true;
            for (int i = 0; i < storages.Length; i++)
            {
                if (storages[i].GetVAM_PFeature_SetInstance_ID() != 0)
                {
                    allZeroASI = false;
                    break;
                }
            }
            if (allZeroASI)
                return;

            MVAMInventoryLineMP ma = null;
            Decimal sum = Env.ZERO;
            for (int i = 0; i < storages.Length; i++)
            {
                MVAMStorage storage = storages[i];
                // nnayak - ignore negative layers
                if (Env.Signum(storage.GetQtyOnHand()) <= 0)
                {
                    continue;
                }
                if (ma != null
                    && ma.GetVAM_PFeature_SetInstance_ID() == storage.GetVAM_PFeature_SetInstance_ID())
                {
                    ma.SetMovementQty(Decimal.Add(ma.GetMovementQty(), storage.GetQtyOnHand()));
                }
                else
                {
                    ma = new MVAMInventoryLineMP(this,
                        storage.GetVAM_PFeature_SetInstance_ID(), storage.GetQtyOnHand());
                }
                if (!ma.Save())
                {
                    ;
                }
                sum = Decimal.Add(sum, storage.GetQtyOnHand());
            }
            if (updateQtyBooked && sum.CompareTo(GetQtyBook()) != 0)
            {
                log.Warning("QtyBook=" + GetQtyBook() + " corrected to Sum of MA=" + sum);
                SetQtyBook(sum);
            }
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>parent</returns>
        public MVAMInventory GetParent()
        {
            if (_parent == null)
                _parent = new MVAMInventory(GetCtx(), GetVAM_Inventory_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <returns>product or null if not defined</returns>
        public MVAMProduct GetProduct()
        {
            int VAM_Product_ID = GetVAM_Product_ID();
            if (VAM_Product_ID == 0)
                return null;
            if (_product != null && _product.GetVAM_Product_ID() != VAM_Product_ID)
                _product = null;	//	reset
            if (_product == null)
                _product = MVAMProduct.Get(GetCtx(), VAM_Product_ID);
            return _product;
        }

        /// <summary>
        /// Get Qty Book
        /// </summary>
        /// <returns>Qty Book</returns>
        public new Decimal GetQtyBook()
        {
            Decimal bd = base.GetQtyBook();
            //if (bd == null)
            //    bd = Env.ZERO;
            return bd;
        }

        /// <summary>
        /// Get Qty Count
        /// </summary>
        /// <returns>Qty Count</returns>
        public new Decimal GetQtyCount()
        {
            Decimal bd = base.GetQtyCount();
            //if (bd == null)
            //    bd = Env.ZERO;
            return bd;
        }

        /// <summary>
        /// Set Attribute Set Instance - Callout
        /// </summary>
        /// <param name="oldVAM_PFeature_SetInstance_ID">old value</param>
        /// <param name="newVAM_PFeature_SetInstance_ID">new value</param>
        /// <param name="windowNo">window</param>
        //  @UICallout
        public void SetVAM_PFeature_SetInstance_ID(String oldVAM_PFeature_SetInstance_ID,
            String newVAM_PFeature_SetInstance_ID, int windowNo)
        {
            if (newVAM_PFeature_SetInstance_ID == null || newVAM_PFeature_SetInstance_ID.Length == 0)
                return;
            //
            int VAM_PFeature_SetInstance_ID = int.Parse(newVAM_PFeature_SetInstance_ID);
            base.SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            //
            SetQtyBook(windowNo, "VAM_PFeature_SetInstance_ID");
        }

        /// <summary>
        /// Set Locator - Callout
        /// </summary>
        /// <param name="oldVAM_Locator_ID">old value</param>
        /// <param name="newVAM_Locator_ID">new value</param>
        /// <param name="windowNo">window</param>
        //  @UICallout
        public void SetVAM_Locator_ID(String oldVAM_Locator_ID, String newVAM_Locator_ID, int windowNo)
        {
            if (newVAM_Locator_ID == null || newVAM_Locator_ID.Length == 0)
                return;
            int VAM_Locator_ID = int.Parse(newVAM_Locator_ID);
            base.SetVAM_Locator_ID(VAM_Locator_ID);
            SetQtyBook(windowNo, "VAM_Locator_ID");
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldVAM_Product_ID">old value</param>
        /// <param name="newVAM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //  @UICallout
        public void SetVAM_Product_ID(String oldVAM_Product_ID, String newVAM_Product_ID, int windowNo)
        {
            if (newVAM_Product_ID == null || newVAM_Product_ID.Length == 0)
                return;
            int VAM_Product_ID = int.Parse(newVAM_Product_ID);
            base.SetVAM_Product_ID(VAM_Product_ID);
            //
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_Product_ID") == VAM_Product_ID)
            {
                int VAM_PFeature_SetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID");
                SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            }
            else
                SetVAM_PFeature_SetInstance_ID(-1);

            SetQtyBook(windowNo, "VAM_Product_ID");
        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">parent</param>
        public void SetParent(MVAMInventory parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Set Qty Book from Product, AST, Locator
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        private void SetQtyBook(int windowNo, String columnName)
        {
            int VAM_Product_ID = GetVAM_Product_ID();
            int VAM_Locator_ID = GetVAM_Locator_ID();
            if (VAM_Product_ID == 0 || VAM_Locator_ID == 0)
            {
                SetQtyBook(Env.ZERO);
                return;
            }
            int VAM_PFeature_SetInstance_ID = GetVAM_PFeature_SetInstance_ID();

            // Set QtyBook from first storage location
            Decimal qtyBook = Env.ZERO;
            String sql = "SELECT QtyOnHand FROM VAM_Storage "
                + "WHERE VAM_Product_ID=@prodid"	//	1
                + " AND VAM_Locator_ID=@locid"		//	2
                + " AND VAM_PFeature_SetInstance_ID=@asiid";
            if (VAM_PFeature_SetInstance_ID == 0)
                sql = "SELECT SUM(QtyOnHand) FROM VAM_Storage "
                + "WHERE VAM_Product_ID=@prodid"	//	1
                + " AND VAM_Locator_ID=@locid";	//	2
            IDataReader idr = null;
            SqlParameter[] param = null;
            try
            {
                if (VAM_PFeature_SetInstance_ID != 0)
                {
                    param = new SqlParameter[3];
                }
                else
                {
                    param = new SqlParameter[2];
                }
                param[0] = new SqlParameter("@prodid", VAM_Product_ID);
                param[1] = new SqlParameter("@locid", VAM_Locator_ID);
                if (VAM_PFeature_SetInstance_ID != 0)
                {
                    param[2] = new SqlParameter("@asiid", VAM_PFeature_SetInstance_ID);
                }

                idr = DataBase.DB.ExecuteReader(sql, param);
                if (idr.Read())
                {
                    qtyBook = Utility.Util.GetValueOfDecimal(idr[0]);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }

                log.Log(Level.SEVERE, sql, e);
                //ErrorLog.FillErrorLog("MVAMInventoryLine.SetQtyBook", DataBase.GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            SetQtyBook(qtyBook);
            //
            log.Info("VAM_Product_ID=" + VAM_Product_ID
                + ", VAM_Locator_ID=" + VAM_Locator_ID
                + ", VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID
                + " - QtyBook=" + qtyBook);
        }

        /// <summary>
        /// Set Count Qty - enforce UOM 
        /// </summary>
        /// <param name="qtyCount">qty</param>
        public new void SetQtyCount(Decimal? qtyCount)
        {
            if (qtyCount != null)
            {
                MVAMProduct product = GetProduct();
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    qtyCount = Decimal.Round(qtyCount.Value, precision, MidpointRounding.AwayFromZero);
                }
            }
            base.SetQtyCount(qtyCount);
        }

        /// <summary>
        /// Set Internal Use Qty - enforce UOM
        /// </summary>
        /// <param name="qtyInternalUse">qty</param>
        public new void SetQtyInternalUse(Decimal? qtyInternalUse)
        {
            if (qtyInternalUse != null)
            {
                MVAMProduct product = GetProduct();
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    qtyInternalUse = Decimal.Round(qtyInternalUse.Value, precision, MidpointRounding.AwayFromZero);
                }
            }
            base.SetQtyInternalUse(qtyInternalUse);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInventoryLine[");
            sb.Append(Get_ID())
                .Append("-VAM_Product_ID=").Append(GetVAM_Product_ID())
                .Append(",QtyCount=").Append(GetQtyCount())
                .Append(",QtyInternalUse=").Append(GetQtyInternalUse())
                .Append(",QtyBook=").Append(GetQtyBook())
                .Append(",VAM_PFeature_SetInstance_ID=").Append(GetVAM_PFeature_SetInstance_ID())
                .Append("]");
            return sb.ToString();
        }

    }
}
