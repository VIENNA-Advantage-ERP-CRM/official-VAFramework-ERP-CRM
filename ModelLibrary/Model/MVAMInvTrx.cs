/********************************************************
 * ModuleName     : 
 * Purpose        : 
 * Class Used     : X_VAM_Inv_Trx
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
using VAdvantage.Logging;

using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MVAMInvTrx : X_VAM_Inv_Trx
    {

        private new static VLogger log = VLogger.GetVLogger(typeof(PO).FullName);
        private DateTime? _policyDate = null;
        private bool isContainerApplicable = false;

        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param VAM_Inv_Trx_ID id
	 *	@param trxName transaction
	 */
        public MVAMInvTrx(Ctx ctx, int VAM_Inv_Trx_ID, Trx trxName)
            : base(ctx, VAM_Inv_Trx_ID, trxName)
        {
            if (VAM_Inv_Trx_ID == 0)
            {
                //	setVAM_Inv_Trx_ID (0);		//	PK
                //	setVAM_Locator_ID (0);
                //	setVAM_Product_ID (0);
                SetMovementDate(DateTime.Now);
                SetMovementQty(Env.ZERO);
                //	setMovementType (MOVEMENTTYPE_CustomerShipment);
            }
            //isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(ctx);
        }

        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param dr result set
	 *	@param trxName transaction
	 */
        public MVAMInvTrx(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            // isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(ctx);
        }

        /**
        * 	Detail Constructor
        *	@param ctx context
        *	@param VAF_Org_ID org
        * 	@param MovementType movement type
        * 	@param VAM_Locator_ID locator
        * 	@param VAM_Product_ID product
        * 	@param VAM_PFeature_SetInstance_ID attribute
        * 	@param MovementQty qty
        * 	@param MovementDate optional date
        *	@param trxName transaction
        */
        public MVAMInvTrx(Ctx ctx, int VAF_Org_ID, String MovementType,
            int VAM_Locator_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
            Decimal MovementQty, DateTime? MovementDate, Trx trxName)
            : base(ctx, 0, trxName)
        {
            //isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(ctx);
            SetVAF_Org_ID(VAF_Org_ID);
            SetMovementType(MovementType);
            if (VAM_Locator_ID == 0)
                throw new ArgumentException("No Locator");
            SetVAM_Locator_ID(VAM_Locator_ID);
            if (VAM_Product_ID == 0)
                throw new ArgumentException("No Product");
            SetVAM_Product_ID(VAM_Product_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            //
            //if (MovementQty != null)		//	Can be 0
            SetMovementQty(MovementQty);
            if (MovementDate == null)
                SetMovementDate(DateTime.Now);
            else
                SetMovementDate(MovementDate);
        }

        /// <summary>
        /// Before save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // is used to check Container applicable into system
            isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            // system will check - if container qty goes negative then not to save Transaction
            MVAMLocator locator = MVAMLocator.Get(GetCtx(), GetVAM_Locator_ID());
            MWarehouse warehouse = MWarehouse.Get(GetCtx(), locator.GetVAM_Warehouse_ID());
            if (isContainerApplicable && warehouse.IsDisallowNegativeInv() && Get_ColumnIndex("ContainerCurrentQty") >= 0 && GetContainerCurrentQty() < 0)
            {
                log.SaveError("Info", Msg.GetMsg(GetCtx(), "VIS_FutureContainerQtygoesNegative"));
                return false;
            }
            else if (warehouse.IsDisallowNegativeInv() && GetCurrentQty() < 0)
            {
                log.SaveError("Info", Msg.GetMsg(GetCtx(), "VIS_FutureQtygoesNegative"));
                return false;
            }
            return base.BeforeSave(newRecord);
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new records</param>
        /// <param name="success">success</param>
        /// <returns>saved</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            // is used to check Container applicable into system
            isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            if (!success)
                return success;

            // check Container Storage Table exist or not
            if (isContainerApplicable && newRecord && PO.Get_Table_ID("VAM_ContainerStorage") > 0 && GetMMpolicyDate() != null)
            {
                // If Physical Inventory available afetr movement date - then not to do impacts on container storage 
                bool isPhysicalInventory = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM VAM_ContainerStorage WHERE IsPhysicalInventory = 'Y'
                                              AND MMPolicyDate > " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                              @" AND VAM_Product_ID = " + GetVAM_Product_ID() +
                                              @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + GetVAM_PFeature_SetInstance_ID() +
                                              @" AND VAM_Locator_ID = " + GetVAM_Locator_ID() +
                                              @" AND NVL(VAM_ProductContainer_ID , 0) = " + GetVAM_ProductContainer_ID(), null, Get_Trx())) > 0 ? true : false;

                if (!isPhysicalInventory && !UpdateContainerStorage(GetVAM_Inv_Trx_ID(), GetMMpolicyDate()))
                {
                    log.SaveError("Info", Msg.GetMsg(GetCtx(), "VIS_ContainerStorageNotUpdated"));
                    return false;
                }
            }

            //int count = PO.Get_Table_ID("VAM_Inv_TrxSummary");
            //if (count > 0)
            //{
            //    UpdateTransactionSummary();
            //}
            //count = PO.Get_Table_ID("VAM_Prod_StockSummary");
            //if (count > 0)
            //{
            //    UpdateStockSummary();
            //}
            return true;
        }

        /// <summary>
        /// Maintain Container Storage
        /// </summary>
        /// <param name="transactionId">transaction id</param>
        /// <param name="policyDate">Policy Date - when to receive or consume</param>
        /// <returns>TRUE/FALSE</returns>
        /// <writer>Amit Bansal</writer>
        public bool UpdateContainerStorage(int transactionId, DateTime? policyDate)
        {
            //X_VAM_ContainerStorage containerStorage = null;
            MVAMInvTrx transaction = new MVAMInvTrx(GetCtx(), transactionId, Get_Trx());

            if (transaction.GetMovementQty() > 0)
            {
                // Create New Container Storage Record - when movement qty > 0
                return StockReceiveManage(transaction, policyDate);
            }
            else
            {
                // Update Record on Container Storage - when Movement Qty < 0
                return StockConsumptionManage(transaction, policyDate);
            }
        }

        /// <summary>
        /// If record exist on Policy Date then update qty on the same record
        /// not exist, then create a new record on Container Storage
        /// </summary>
        /// <param name="transaction">Transaction class object</param>
        /// <param name="policyDate">impacted Container Storage Date</param>
        /// <returns>TRUE, when during receiving record created on containerr storage OR during consumption record updtaed poperly of container storage</returns>
        /// <writer>Amit Bansal</writer>
        public bool StockReceiveManage(MVAMInvTrx transaction, DateTime? policyDate)
        {
            bool IsPhysicalInventoy = false;
            bool isUpdateActualQty = false;
            // Material Receipt -- Vendor Return -- Shipment -- Return to Customer
            if (transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_CustomerReturns ||
                transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_CustomerShipment ||
                transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_VendorReceipts ||
                transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_VendorReturns)
            {
                IsPhysicalInventoy = false;
                isUpdateActualQty = transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_CustomerReturns ||
                                    transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_VendorReturns ? false : true;
            }
            // Physical Inventory -- Internal Use Inventory
            else if (transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_InventoryIn ||
                transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_InventoryOut)
            {
                String isInternalUseInventory = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsInternalUse FROM VAM_InventoryLine WHERE VAM_InventoryLine_ID = "
                                       + transaction.GetVAM_InventoryLine_ID(), null, transaction.Get_Trx()));
                if (isInternalUseInventory == "Y")
                    IsPhysicalInventoy = false;
                else
                    IsPhysicalInventoy = true;

                isUpdateActualQty = true;
            }
            // Inventory Move
            else if (transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_MovementFrom ||
                transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_MovementTo)
            {
                IsPhysicalInventoy = false;
                isUpdateActualQty = true;
            }
            // Production
            else if (transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_ProductionPlus ||
                transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_Production_)
            {
                IsPhysicalInventoy = false;
                isUpdateActualQty = true;
            }
            // Production Execution (Manufacturing)
            else if (transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_WorkOrderPlus ||
                transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_WorkOrder_)
            {
                IsPhysicalInventoy = false;
                isUpdateActualQty = true;
            }

            int no = DB.ExecuteQuery(@"UPDATE VAM_ContainerStorage SET QTY = QTY + " + Math.Abs(transaction.GetMovementQty()) +
                  @" , isphysicalInventory = CASE WHEN isphysicalInventory ='Y'  THEN 'Y' 
                                                  WHEN isphysicalInventory ='N' THEN '" + (IsPhysicalInventoy ? "Y" : "N") + @"' END WHERE MMPolicyDate = " + GlobalVariable.TO_DATE(policyDate, true) +
                  @" AND VAM_Product_ID = " + transaction.GetVAM_Product_ID() +
                  @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + transaction.GetVAM_PFeature_SetInstance_ID() +
                  @" AND VAM_Locator_ID = " + transaction.GetVAM_Locator_ID() +
                  @" AND NVL(VAM_ProductContainer_ID , 0) = " + transaction.GetVAM_ProductContainer_ID(), null, Get_Trx());
            if (no < 0)
            {
                return false;
            }
            else if (no > 0)
            {
                // delete Record if qty on Container Storage is ZERO
                int nos = DB.ExecuteQuery(@"DELETE FROM VAM_ContainerStorage WHERE ISPHYSICALINVENTORY = 'N' AND 0 = QTY 
                AND MMPolicyDate = " + GlobalVariable.TO_DATE(policyDate, true) +
                 @" AND VAM_Product_ID = " + transaction.GetVAM_Product_ID() +
                 @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + transaction.GetVAM_PFeature_SetInstance_ID() +
                 @" AND VAM_Locator_ID = " + transaction.GetVAM_Locator_ID() +
                 @" AND NVL(VAM_ProductContainer_ID , 0) = " + transaction.GetVAM_ProductContainer_ID(), null, Get_Trx());
                if (nos < 0)
                    return false;
            }
            else
            {
                // create new record
                if (!CreateContainerStorage(transaction, Math.Abs(transaction.GetMovementQty()), policyDate, IsPhysicalInventoy, isUpdateActualQty))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Create a new record on Containe Storage
        /// </summary>
        /// <param name="transaction">Transaction class object</param>
        /// <param name="qty">to be RECEIVE qty</param>
        /// <param name="policyDate">qty receive date</param>
        /// <param name="IsPhysicalInventory">Physical Inventory OR Other Transaction</param>
        /// <param name="isUpdateActualQty">Is update Actual qty on container Storage - Optional</param>
        /// <returns>TRUE when container storage record created</returns>
        /// <writer>Amit Bansal</writer>
        public bool CreateContainerStorage(MVAMInvTrx transaction, Decimal qty, DateTime? policyDate, bool IsPhysicalInventory, bool isUpdateActualQty)
        {
            MVAMLocator locator = MVAMLocator.Get(GetCtx(), transaction.GetVAM_Locator_ID());
            X_VAM_ContainerStorage containerStorage = new X_VAM_ContainerStorage(GetCtx(), 0, transaction.Get_Trx());
            containerStorage.SetVAF_Client_ID(transaction.GetVAF_Client_ID());
            containerStorage.SetVAF_Org_ID(locator.GetVAF_Org_ID());
            containerStorage.SetVAM_Locator_ID(transaction.GetVAM_Locator_ID());
            containerStorage.SetVAM_Product_ID(transaction.GetVAM_Product_ID());
            containerStorage.SetVAM_PFeature_SetInstance_ID(transaction.GetVAM_PFeature_SetInstance_ID());
            containerStorage.SetVAM_ProductContainer_ID(transaction.GetVAM_ProductContainer_ID());
            containerStorage.SetMMPolicyDate(policyDate);
            // Physical Inventory 
            if (IsPhysicalInventory)
            {
                containerStorage.SetIsPhysicalInventory(true);
                containerStorage.SetQtyCalculation(transaction.GetContainerCurrentQty());
            }
            if (isUpdateActualQty)
            {
                containerStorage.SetActualQty(Decimal.Add(containerStorage.GetActualQty(), transaction.GetMovementQty()));
            }
            containerStorage.SetQty(Decimal.Add(containerStorage.GetQty(), qty));
            if (!containerStorage.Save(transaction.Get_Trx()))
            {
                ValueNamePair pp = VLogger.RetrieveError();
                log.Severe("Container Storage not created <===>  Error Type is : " + pp.GetName());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Is Used to consume product qty from date of Policy
        /// </summary>
        /// <param name="transaction">Transaction class object</param>
        /// <param name="policyDate">consume qty from date</param>
        /// <returns></returns>
        /// <writer>Amit Bansal</writer>
        public bool StockConsumptionManage(MVAMInvTrx transaction, DateTime? policyDate)
        {
            String isInternalInventory = "Y";
            if (transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_InventoryIn ||
               transaction.GetMovementType() == MVAMInvTrx.MOVEMENTTYPE_InventoryOut)
            {
                isInternalInventory = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsInternalUse FROM VAM_InventoryLine WHERE VAM_InventoryLine_ID = "
                                          + transaction.GetVAM_InventoryLine_ID(), null, transaction.Get_Trx()));
            }

            // Delete record when qty become ZERO
            int no = DB.ExecuteQuery(@"DELETE FROM VAM_ContainerStorage WHERE ISPHYSICALINVENTORY = 'N' AND 0 = QTY - " + Math.Abs(transaction.GetMovementQty()) +
              @" AND MMPolicyDate = " + GlobalVariable.TO_DATE(policyDate, true) +
              @" AND VAM_Product_ID = " + transaction.GetVAM_Product_ID() +
              @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + transaction.GetVAM_PFeature_SetInstance_ID() +
              @" AND VAM_Locator_ID = " + transaction.GetVAM_Locator_ID() +
              @" AND NVL(VAM_ProductContainer_ID , 0) = " + transaction.GetVAM_ProductContainer_ID(), null, Get_Trx());
            if (no == 0)
            {
                no = DB.ExecuteQuery(@"UPDATE VAM_ContainerStorage SET QTY = QTY - " + Math.Abs(transaction.GetMovementQty()) +
                 @" WHERE MMPolicyDate = " + GlobalVariable.TO_DATE(policyDate, true) +
                 @" AND VAM_Product_ID = " + transaction.GetVAM_Product_ID() +
                 @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + transaction.GetVAM_PFeature_SetInstance_ID() +
                 @" AND VAM_Locator_ID = " + transaction.GetVAM_Locator_ID() +
                 @" AND NVL(VAM_ProductContainer_ID , 0) = " + transaction.GetVAM_ProductContainer_ID(), null, Get_Trx());
                if (no < 0)
                    return false;
                else if (no == 0)
                {
                    // when no record found on same date, then need to create container storage entry
                    if (!CreateContainerStorage(transaction, transaction.GetMovementQty(), policyDate,
                           isInternalInventory == "N" ? true : false, true))
                        return false;
                }
                else if (no > 0 && isInternalInventory == "N")
                {
                    if (!CreateRecordForPhysicalInventory(transaction, transaction.GetMovementDate()))
                        return false;
                }
            }
            else if (no == -1)
                return false;
            else if (no > 0 && isInternalInventory == "N")
            {
                if (!CreateRecordForPhysicalInventory(transaction, transaction.GetMovementDate()))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// when we reduced then system have to check, is same date of physical inventory record fround then OK
        /// else need to create new record with ZERO qty and mark it as Physical Inventory as TRUE
        /// </summary>
        /// <param name="transaction">Transaction class object</param>
        /// <param name="movementDate">movement date</param>
        /// <returns>TRUE when container storage record created</returns>
        /// <writer>Amit Bansal</writer>
        public bool CreateRecordForPhysicalInventory(MVAMInvTrx transaction, DateTime? movementDate)
        {
            // check if PHYSICAL INVENTORY record exist of current date, then no need to do any impact
            int no = DB.ExecuteQuery(@"UPDATE VAM_ContainerStorage SET IsPhysicalInventory = 'Y' 
                    WHERE MMPolicyDate = " + GlobalVariable.TO_DATE(movementDate, true) +
                 @" AND VAM_Product_ID = " + transaction.GetVAM_Product_ID() +
                 @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + transaction.GetVAM_PFeature_SetInstance_ID() +
                 @" AND VAM_Locator_ID = " + transaction.GetVAM_Locator_ID() +
                 @" AND NVL(VAM_ProductContainer_ID , 0) = " + transaction.GetVAM_ProductContainer_ID(), null, Get_Trx());

            if (no <= 0)
            {
                // if record not found, then create record with ZERO qty
                X_VAM_ContainerStorage containerStorage = new X_VAM_ContainerStorage(GetCtx(), 0, transaction.Get_Trx());
                containerStorage.SetVAF_Client_ID(transaction.GetVAF_Client_ID());
                containerStorage.SetVAF_Org_ID(transaction.GetVAF_Org_ID());
                containerStorage.SetVAM_Locator_ID(transaction.GetVAM_Locator_ID());
                containerStorage.SetVAM_Product_ID(transaction.GetVAM_Product_ID());
                containerStorage.SetVAM_PFeature_SetInstance_ID(transaction.GetVAM_PFeature_SetInstance_ID());
                containerStorage.SetVAM_ProductContainer_ID(transaction.GetVAM_ProductContainer_ID());
                containerStorage.SetMMPolicyDate(movementDate);
                containerStorage.SetIsPhysicalInventory(true);
                containerStorage.SetQty(Env.ZERO);
                if (!containerStorage.Save(transaction.Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    log.Severe("Container Storage not created <===>  Error Type is : " + pp.GetName());
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// To check weather future date records are available in Transaction window 
        /// </summary>
        /// <param name="MovementDate">Movement Date</param>
        /// <param name="TableName">Name Of Table (VAM_Inv_InOut,VAM_Inventory,VAM_InventoryTransfer, VAM_Production, VAMFG_M_WRKODRTRANSACTION)</param>
        /// <param name="Record_ID">ID of Current Record</param>
        /// <param name="Trx">Current Transaction Object</param>
        /// <returns>true if any record found on transaction window or false if not found</returns>
        /// <writer>Manjot, Amit</writer>
        public static string CheckFutureDateRecord(DateTime? MovementDate, string TableName, int Record_ID, Trx trx)
        {
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("p_movementdate", MovementDate);
            param[0].SqlDbType = SqlDbType.Date;
            param[0].Direction = ParameterDirection.Input;

            param[1] = new SqlParameter("p_TableName", TableName.ToUpper());
            param[1].SqlDbType = SqlDbType.VarChar;
            param[1].Direction = ParameterDirection.Input;

            param[2] = new SqlParameter("p_Record_ID", Record_ID);
            param[2].SqlDbType = SqlDbType.Int;
            param[2].Direction = ParameterDirection.Input;

            param[3] = new SqlParameter("results", 0);
            param[3].SqlDbType = SqlDbType.Int;
            param[3].Direction = ParameterDirection.Output;

            param = DB.ExecuteProcedure("CheckFutureDateRecord", param, trx);

            if (param != null && param.Length > 0 && Util.GetValueOfInt(param[0].Value) > 0) // If Record Found
            {
                return Msg.GetMsg(Env.GetCtx(), "AlreadyFound");
            }
            return string.Empty;            
        }

        public void UpdateStockSummary()
        {
            StringBuilder Qry = new StringBuilder();
            decimal OpeningStock = 0, movementQty = 0;
            MVAMProdStockSummary Trs = null;
            MVAMLocator loc = new MVAMLocator(GetCtx(), GetVAM_Locator_ID(), Get_TrxName());
            int VAF_Org_ID = loc.GetVAF_Org_ID();

            Qry.Append("SELECT VAM_Prod_StockSummary_ID FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + GetVAM_Product_ID() +
                " AND VAF_Org_ID = " + VAF_Org_ID + " AND MovementFromDate = " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int VAM_Prod_StockSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            Qry.Clear();
            Qry.Append("SELECT Count(*) FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + GetVAM_Product_ID() +
                        " AND VAF_Org_ID = " + VAF_Org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int existOld = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            if (existOld > 0)
            {
                Qry.Clear();
                Qry.Append("SELECT QtyCloseStockOrg FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + GetVAM_Product_ID() +
                            " AND VAF_Org_ID = " + VAF_Org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true) + " ORDER BY MovementFromDate DESC");
                OpeningStock = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
            }

            if (VAM_Prod_StockSummary_ID > 0)
            {
                Trs = new MVAMProdStockSummary(GetCtx(), VAM_Prod_StockSummary_ID, Get_TrxName());
                Trs.SetQtyOpenStockOrg(OpeningStock);
                Trs.SetQtyCloseStockOrg(Trs.GetQtyCloseStockOrg() + GetMovementQty());
            }
            else
            {
                Trs = new MVAMProdStockSummary(GetCtx(), VAF_Org_ID, GetVAM_Product_ID(),
                        0, GetCurrentQty(), GetMovementDate(), Get_TrxName());
                Trs.SetQtyOpenStockOrg(OpeningStock);
            }
            Qry.Append("SELECT Count(*) FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + GetVAM_Product_ID() +
                        " AND VAF_Org_ID = " + VAF_Org_ID + " AND MovementFromDate > " + GlobalVariable.TO_DATE(GetMovementDate(), true));
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
                    Qry.Append("SELECT VAM_Prod_StockSummary_ID FROM VAM_Prod_StockSummary WHERE IsActive = 'Y' AND VAM_Product_ID = " + GetVAM_Product_ID() +
                                " AND VAF_Org_ID = " + VAF_Org_ID + " AND MovementFromDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true) + " ORDER BY MovementFromDate DESC");
                    int oldSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
                    MVAMProdStockSummary oldTrs = new MVAMProdStockSummary(GetCtx(), oldSummary_ID, Get_Trx());
                    oldTrs.SetMovementToDate(Convert.ToDateTime(GetMovementDate()).AddDays(-1));
                    oldTrs.Save();
                }
            }
        }

        public void UpdateTransactionSummary()
        {
            StringBuilder Qry = new StringBuilder();
            decimal OpeningStock = 0, movementQty = 0;
            MVAMInvTrxSummary Trs = null;
            Qry.Append("SELECT VAM_Inv_TrxSummary_ID FROM VAM_Inv_TrxSummary WHERE IsActive = 'Y' AND  VAM_Product_ID = " + GetVAM_Product_ID() +
                " AND VAM_Locator_ID = " + GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + GetVAM_PFeature_SetInstance_ID() +
                " AND MovementDate = " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int VAM_Inv_TrxSummary_ID = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            Qry.Clear();
            Qry.Append("SELECT Count(*) FROM VAM_Inv_TrxSummary WHERE IsActive = 'Y' AND  VAM_Product_ID = " + GetVAM_Product_ID() +
                            " AND VAM_Locator_ID = " + GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + GetVAM_PFeature_SetInstance_ID() +
                            " AND MovementDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true));
            int existOld = Util.GetValueOfInt(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            Qry.Clear();
            if (existOld > 0)
            {
                Qry.Append("SELECT ClosingStock FROM VAM_Inv_TrxSummary WHERE IsActive = 'Y' AND  VAM_Product_ID = " + GetVAM_Product_ID() +
                                " AND VAM_Locator_ID = " + GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + GetVAM_PFeature_SetInstance_ID() +
                                " AND MovementDate < " + GlobalVariable.TO_DATE(GetMovementDate(), true) + " ORDER BY MovementDate DESC");
            }
            else
            {
                Qry.Append("SELECT NVL(GetStockofWarehouse(" + GetVAM_Product_ID() + "," + GetVAM_Locator_ID() + ",0," + GetVAM_PFeature_SetInstance_ID() + ","
                + GlobalVariable.TO_DATE(Convert.ToDateTime(GetMovementDate()).AddDays(-1), true) + "," + GetVAF_Client_ID() + "," + GetVAF_Org_ID() + "),0) AS Stock FROM DUAL");
            }

            OpeningStock = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));

            if (VAM_Inv_TrxSummary_ID > 0)
            {
                Trs = new MVAMInvTrxSummary(GetCtx(), VAM_Inv_TrxSummary_ID, Get_TrxName());
                Trs.SetOpeningStock(OpeningStock);
                Trs.SetClosingStock(GetCurrentQty());
                Qry.Clear();
                Qry.Append("SELECT SUM(MovementQty) FROM VAM_Inv_Trx WHERE MovementType = '" + GetMovementType() + "' AND  VAM_Product_ID = " + GetVAM_Product_ID() +
                           " AND VAM_Locator_ID = " + GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + GetVAM_PFeature_SetInstance_ID() +
                           "AND movementdate = " + GlobalVariable.TO_DATE(GetMovementDate(), true));
                movementQty = Util.GetValueOfDecimal(DB.ExecuteScalar(Qry.ToString(), null, Get_Trx()));
            }
            else
            {

                MVAMLocator loc = new MVAMLocator(GetCtx(), GetVAM_Locator_ID(), Get_TrxName());
                Trs = new MVAMInvTrxSummary(GetCtx(), loc.GetVAF_Org_ID(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(),
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
            StringBuilder sb = new StringBuilder("MVAMInvTrx[");
            sb.Append(Get_ID()).Append(",").Append(GetMovementType())
                .Append(",Qty=").Append(GetMovementQty())
                .Append(",VAM_Product_ID=").Append(GetVAM_Product_ID())
                .Append(",ASI=").Append(GetVAM_PFeature_SetInstance_ID())
                .Append("]");
            return sb.ToString();
        }

        public void SetMMPolicyDate(DateTime? policyDate)
        {
            _policyDate = policyDate;
        }

        public DateTime? GetMMpolicyDate()
        {
            return _policyDate;
        }


        public static bool ProductContainerApplicable(Ctx ctx)
        {
            if (ctx != null)
            {
                return Util.GetValueOfString(ctx.GetContext("#PRODUCT_CONTAINER_APPLICABLE")).Equals("Y");
            }
            else
            {
                return false;
            }
        }

        private static Decimal? GetProductQtyFroMVAMStorage(
          int VAM_Product_ID,
          int ASIID,
          int loc_ID,
          Trx _trx)
        {
            return 0;
        }

        public static Decimal? GetProductQtyFroMVAMInvTrx(
          int ProductID,
          int ASIID,
          DateTime? movementDate,
          bool isAttribute,
          int locatorId,
          Trx _trx)
        {
            Decimal? num1 = 0;
            int num2;
            if (isAttribute)
                num2 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAM_Inv_Trx_ID) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID, null, _trx));
            else
                num2 = 1;
            if (num2 == 0)
            {
                num1 = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT CurrentQty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_ID  = (SELECT MAX(VAM_Inv_Trx_id) FROM VAM_Inv_Trx WHERE movementdate = (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID + ") AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID + ") AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID, null, _trx));
            }
            else
            {
                int num3;
                if (isAttribute)
                    num3 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAM_Inv_Trx_ID) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID, null, _trx));
                else
                    num3 = 1;
                if (num3 == 0)
                {
                    num1 = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT CurrentQty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_ID = (SELECT MAX(VAM_Inv_Trx_id) FROM VAM_Inv_Trx WHERE movementdate = (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID + ") AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID + ") AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = " + ASIID, null, _trx));
                }
                else
                {
                    int num4;
                    if (!isAttribute)
                        num4 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = 0 ", null, _trx));
                    else
                        num4 = 1;
                    if (num4 == 0)
                    {
                        num1 = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id = (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx WHERE movementdate = (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ) AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ) AND VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ", null, _trx));
                    }
                    else
                    {
                        int num5;
                        if (!isAttribute)
                            num5 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND VAM_PFeature_SetInstance_ID = 0 ", null, _trx));
                        else
                            num5 = 1;
                        if (num5 == 0)
                            num1 = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id = (SELECT MAX(VAM_Inv_Trx_id) FROM VAM_Inv_Trx WHERE movementdate = (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + " AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ) AND  VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ) AND VAM_Product_ID = " + ProductID + " AND VAM_Locator_ID = " + locatorId + "   AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ", null, _trx));
                    }
                }
            }
            return num1;
        }

        public static Decimal? GetContainerQtyFroMVAMInvTrx(
          int VAF_Client_ID,
          int ProductID,
          int ASIID,
          DateTime? movementDate,
          int locatorId,
          int containerId,
          Trx _trx)
        {
            Decimal num = new Decimal(0);
            return Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.VAM_Inv_Trx_ID) AS ContainerCurrentQty FROM VAM_Inv_Trx t WHERE TO_DATE(t.MovementDate,'YYYY-MM-DD') <=" + GlobalVariable.TO_DATE(movementDate, true) + "  AND t.VAF_Client_ID = " + VAF_Client_ID + " AND t.VAM_Locator_ID = " + locatorId + " AND t.VAM_Product_ID = " + ProductID + " AND NVL(t.VAM_PFeature_SetInstance_ID , 0) = COALESCE(" + ASIID + ",0) AND NVL(t.VAM_ProductContainer_ID, 0) = " + containerId, null, _trx));
        }

        public static string UpdateProductContainerTransaction(
          Ctx ctx,
          int VAF_Org_ID,
          int VAM_Product_ID,
          int ASIID,
          DateTime? movementDate,
          DateTime? policyDate,
          Decimal? moveQty,
          int VAM_Locator_ID,
          int VAM_ProductContainer_ID,
          string MovementType,
          string LineColumnName,
          int lineID,
          Trx _trx)
        {
            string str1 = "";
            Decimal? nullable1 = 0;
            Decimal? nullable2 = 0;
            bool flag = false;
            StringBuilder stringBuilder = new StringBuilder("");
            if (ASIID > 0)
                stringBuilder.Append("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE IsActive = 'Y' AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + ASIID + " AND movementdate <= " + GlobalVariable.TO_DATE(movementDate, true));
            else
                stringBuilder.Append("SELECT COUNT(*)   FROM VAM_Inv_Trx WHERE IsActive = 'Y' AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(movementDate, true));
            if (Util.GetValueOfInt(DB.ExecuteScalar(stringBuilder.ToString(), null, _trx)) > 0)
            {
                nullable1 = MVAMInvTrx.GetProductQtyFroMVAMInvTrx(VAM_Product_ID, ASIID, movementDate, true, VAM_Locator_ID, _trx);
                flag = true;
            }
            if (!flag)
                nullable1 = MVAMInvTrx.GetProductQtyFroMVAMStorage(VAM_Product_ID, ASIID, VAM_Locator_ID, _trx);
            if (VAM_ProductContainer_ID >= 0)
                nullable2 = MVAMInvTrx.GetContainerQtyFroMVAMInvTrx(ctx.GetVAF_Client_ID(), VAM_Product_ID, ASIID, movementDate, VAM_Locator_ID, VAM_ProductContainer_ID, _trx);
            MVAMInvTrx mtrx = new MVAMInvTrx(ctx, VAF_Org_ID, MovementType, VAM_Locator_ID, VAM_Product_ID, ASIID, moveQty.Value, movementDate, _trx);
            mtrx.Set_Value(LineColumnName, lineID);
            MVAMInvTrx MVAMInvTrx1 = mtrx;
            Decimal? nullable3 = nullable1;
            Decimal? nullable4 = moveQty;
            Decimal? CurrentQty = nullable3.HasValue & nullable4.HasValue ? new Decimal?(nullable3.GetValueOrDefault() + nullable4.GetValueOrDefault()) : new Decimal?();
            MVAMInvTrx1.SetCurrentQty(CurrentQty);
            if (policyDate.HasValue)
                mtrx.SetMMPolicyDate(policyDate);
            else
                mtrx.SetMMPolicyDate(movementDate);
            if (mtrx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
            {
                mtrx.SetVAM_ProductContainer_ID(VAM_ProductContainer_ID);
                MVAMInvTrx MVAMInvTrx2 = mtrx;
                nullable3 = nullable2;
                nullable4 = moveQty;
                Decimal? ContainerCurrentQty = nullable3.HasValue & nullable4.HasValue ? nullable3.GetValueOrDefault() + nullable4.GetValueOrDefault() : new Decimal?();
                MVAMInvTrx2.SetContainerCurrentQty(ContainerCurrentQty);
            }
            if (!mtrx.Save())
            {
                _trx.Rollback();
                ValueNamePair valueNamePair = VLogger.RetrieveError();
                return valueNamePair == null || string.IsNullOrEmpty(valueNamePair.GetName()) ? "Transaction From not inserted (MA)" : valueNamePair.GetName();
            }
            if (mtrx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
            {
                string str2 = MVAMInvTrx.UpdateTransactionContainer(ctx, VAM_Product_ID, ASIID, mtrx, nullable1.Value + moveQty.Value, VAM_Locator_ID, VAM_ProductContainer_ID, _trx);
                if (!string.IsNullOrEmpty(str2))
                    return str2;
            }
            else
                MVAMInvTrx.UpdateTransaction(ctx, VAM_Product_ID, ASIID, movementDate, nullable1.Value + moveQty.Value, VAM_Locator_ID, _trx);
            return str1;
        }

        private static void UpdateTransaction(
          Ctx ctx,
          int VAM_Product_ID,
          int ASIID,
          DateTime? movementDate,
          Decimal qtyMove,
          int loc_ID,
          Trx _trx)
        {
            DataSet dataSet = new DataSet();
            try
            {
                string sql;
                if (ASIID > 0)
                    sql = "SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id , MovementType , VAM_InventoryLine_ID FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(movementDate.Value.AddDays(1.0), true) + " AND VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = " + ASIID + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC ,  created ASC";
                else
                    sql = "SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id , MovementType , VAM_InventoryLine_ID FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(movementDate.Value.AddDays(1.0), true) + " AND VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = 0  ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC ,  created ASC";
                dataSet = DB.ExecuteDataset(sql, null, _trx);
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    for (int index = 0; index < dataSet.Tables[0].Rows.Count; ++index)
                    {
                        if (Util.GetValueOfString(dataSet.Tables[0].Rows[index]["MovementType"]) == "I+" && Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_InventoryLine_ID"]) > 0)
                        {
                            MVAMInventoryLine MVAMInventoryLine = new MVAMInventoryLine(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_InventoryLine_ID"]), _trx);
                            if (!new MVAMInventory(ctx, Util.GetValueOfInt(MVAMInventoryLine.GetVAM_Inventory_ID()), _trx).IsInternalUse())
                            {
                                MVAMInventoryLine.SetQtyBook(qtyMove);
                                MVAMInventoryLine.SetOpeningStock(qtyMove);
                                MVAMInventoryLine.SetDifferenceQty(Decimal.Subtract(qtyMove, Util.GetValueOfDecimal(dataSet.Tables[0].Rows[index]["currentqty"])));
                                if (!MVAMInventoryLine.Save())
                                    MVAMInvTrx.log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_InventoryLine_ID"]));
                                MVAMInvTrx mvamInvTrx = new MVAMInvTrx(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]), _trx);
                                mvamInvTrx.SetMovementQty(Decimal.Negate(Decimal.Subtract(qtyMove, Util.GetValueOfDecimal(dataSet.Tables[0].Rows[index]["currentqty"]))));
                                if (!mvamInvTrx.Save())
                                    MVAMInvTrx.log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]));
                                else
                                    qtyMove = mvamInvTrx.GetCurrentQty();
                                if (index == dataSet.Tables[0].Rows.Count - 1)
                                {
                                    MVAMStorage MVAMStorage = MVAMStorage.Get(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx) ?? MVAMStorage.GetCreate(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx);
                                    if (MVAMStorage.GetQtyOnHand() != qtyMove)
                                    {
                                        MVAMStorage.SetQtyOnHand(qtyMove);
                                        MVAMStorage.Save();
                                    }
                                    continue;
                                }
                                continue;
                            }
                        }
                        MVAMInvTrx MVAMInvTrx1 = new MVAMInvTrx(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]), _trx);
                        MVAMInvTrx1.SetCurrentQty(qtyMove + MVAMInvTrx1.GetMovementQty());
                        if (!MVAMInvTrx1.Save())
                            MVAMInvTrx.log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]));
                        else
                            qtyMove = MVAMInvTrx1.GetCurrentQty();
                        if (index == dataSet.Tables[0].Rows.Count - 1)
                        {
                            MVAMStorage MVAMStorage = MVAMStorage.Get(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx) ?? MVAMStorage.GetCreate(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx);
                            if (MVAMStorage.GetQtyOnHand() != qtyMove)
                            {
                                MVAMStorage.SetQtyOnHand(qtyMove);
                                MVAMStorage.Save();
                            }
                        }
                    }
                }
                dataSet.Dispose();
            }
            catch (Exception ex)
            {
                dataSet.Dispose();
                MVAMInvTrx.log.Info("Current Quantity Not Updated at Transaction Tab :: s" + ex.Message);
            }
        }

        private static string UpdateTransactionContainer(
          Ctx ctx,
          int VAM_Product_ID,
          int ASIID,
          MVAMInvTrx mtrx,
          Decimal Qty,
          int loc_ID,
          int containerId,
          Trx _trx)
        {
            string str = "";
            int MVAMPFeatureSetId = new MVAMProduct(ctx, VAM_Product_ID, _trx).GetVAM_PFeature_Set_ID();
            DataSet dataSet = new DataSet();
            Decimal containerCurrentQty = mtrx.GetContainerCurrentQty();
            try
            {
                string sql;
                if (MVAMPFeatureSetId > 0)
                    sql = "SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty , NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty  ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1.0), true) + " AND VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = " + ASIID + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC, created ASC";
                else
                    sql = "SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty, NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1.0), true) + " AND VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + loc_ID + " AND VAM_PFeature_SetInstance_ID = 0  ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC , created ASC";
                dataSet = DB.ExecuteDataset(sql, null, _trx);
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    for (int index = 0; index < dataSet.Tables[0].Rows.Count; ++index)
                    {
                        if (Util.GetValueOfString(dataSet.Tables[0].Rows[index]["MovementType"]) == "I+" && Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_InventoryLine_ID"]) > 0)
                        {
                            MVAMInventoryLine MVAMInventoryLine = new MVAMInventoryLine(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_InventoryLine_ID"]), _trx);
                            if (!new MVAMInventory(ctx, Util.GetValueOfInt(MVAMInventoryLine.GetVAM_Inventory_ID()), (Trx)null).IsInternalUse())
                            {
                                if (MVAMInventoryLine.GetVAM_ProductContainer_ID() == containerId)
                                {
                                    MVAMInventoryLine.SetQtyBook(containerCurrentQty);
                                    MVAMInventoryLine.SetOpeningStock(containerCurrentQty);
                                    MVAMInventoryLine.SetDifferenceQty(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(dataSet.Tables[0].Rows[index]["ContainerCurrentQty"])));
                                    if (!MVAMInventoryLine.Save())
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_InventoryLine_ID"]));
                                }
                                MVAMInvTrx MVAMInvTrx = new MVAMInvTrx(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]), _trx);
                                if (MVAMInvTrx.GetVAM_ProductContainer_ID() == containerId)
                                    MVAMInvTrx.SetMovementQty(Decimal.Negate(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(dataSet.Tables[0].Rows[index]["ContainerCurrentQty"]))));
                                else
                                    MVAMInvTrx.SetCurrentQty(Decimal.Add(Qty, Util.GetValueOfDecimal(dataSet.Tables[0].Rows[index]["movementqty"])));
                                if (!MVAMInvTrx.Save())
                                {
                                    log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]));
                                    _trx.Rollback();
                                    ValueNamePair valueNamePair = VLogger.RetrieveError();
                                    if (valueNamePair != null && !string.IsNullOrEmpty(valueNamePair.GetName()))
                                        return valueNamePair.GetName();
                                    return Msg.GetMsg(ctx, "VIS_TranactionNotSaved");
                                }
                                Qty = MVAMInvTrx.GetCurrentQty();
                                if (containerId >= 0 && MVAMInvTrx.GetVAM_ProductContainer_ID() == containerId)
                                    containerCurrentQty = MVAMInvTrx.GetContainerCurrentQty();
                                if (index == dataSet.Tables[0].Rows.Count - 1)
                                {
                                    MVAMStorage MVAMStorage = MVAMStorage.Get(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx) ?? MVAMStorage.GetCreate(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx);
                                    if (MVAMStorage.GetQtyOnHand() != Qty)
                                    {
                                        MVAMStorage.SetQtyOnHand(Qty);
                                        if (!MVAMStorage.Save())
                                        {
                                            _trx.Rollback();
                                            ValueNamePair valueNamePair = VLogger.RetrieveError();
                                            if (valueNamePair == null || string.IsNullOrEmpty(valueNamePair.GetName()))
                                                return Msg.GetMsg(ctx, "VIS_StorageNotSaved");
                                            string name = valueNamePair.GetName();
                                            return Msg.GetMsg(ctx, "VIS_StorageNotSaved") + name;
                                        }
                                    }
                                    continue;
                                }
                                continue;
                            }
                        }
                        MVAMInvTrx MVAMInvTrx1 = new MVAMInvTrx(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]), _trx);
                        MVAMInvTrx1.SetCurrentQty(Qty + MVAMInvTrx1.GetMovementQty());
                        if (MVAMInvTrx1.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && MVAMInvTrx1.GetVAM_ProductContainer_ID() == containerId)
                            MVAMInvTrx1.SetContainerCurrentQty(containerCurrentQty + MVAMInvTrx1.GetMovementQty());
                        if (!MVAMInvTrx1.Save())
                        {
                            log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Inv_Trx_id"]));
                            _trx.Rollback();
                            ValueNamePair valueNamePair = VLogger.RetrieveError();
                            if (valueNamePair != null && !string.IsNullOrEmpty(valueNamePair.GetName()))
                                return valueNamePair.GetName();
                            return Msg.GetMsg(ctx, "VIS_TranactionNotSaved");
                        }
                        Qty = MVAMInvTrx1.GetCurrentQty();
                        if (MVAMInvTrx1.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && MVAMInvTrx1.GetVAM_ProductContainer_ID() == containerId)
                            containerCurrentQty = MVAMInvTrx1.GetContainerCurrentQty();
                        if (index == dataSet.Tables[0].Rows.Count - 1)
                        {
                            MVAMStorage MVAMStorage = MVAMStorage.Get(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx) ?? MVAMStorage.GetCreate(ctx, Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Locator_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_Product_ID"]), Util.GetValueOfInt(dataSet.Tables[0].Rows[index]["VAM_PFeature_SetInstance_ID"]), _trx);
                            if (MVAMStorage.GetQtyOnHand() != Qty)
                            {
                                MVAMStorage.SetQtyOnHand(Qty);
                                if (!MVAMStorage.Save())
                                {
                                    _trx.Rollback();
                                    ValueNamePair valueNamePair = VLogger.RetrieveError();
                                    if (valueNamePair == null || string.IsNullOrEmpty(valueNamePair.GetName()))
                                        return Msg.GetMsg(ctx, "VIS_StorageNotSaved");
                                    string name = valueNamePair.GetName();
                                    return Msg.GetMsg(ctx, "VIS_StorageNotSaved") + name;
                                }
                            }
                        }
                    }
                }
                dataSet.Dispose();
            }
            catch (Exception ex)
            {
                dataSet.Dispose();
                log.Info("Current Quantity Not Updated at Transaction Tab");
                str = Msg.GetMsg(ctx, "ExceptionOccureOnUpdateTrx");
            }
            return str;
        }
    }
}