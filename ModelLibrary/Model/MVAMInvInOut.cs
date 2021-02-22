
/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMInvInOut
 * Purpose        : Class linked with the shipment,invoice window
 * Class Used     : X_VAM_Inv_InOut, DocAction
 * Chronological    Development
 * Raghunandan     05-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MVAMInvInOut : X_VAM_Inv_InOut, DocAction
    {
        #region variable
        //	Process Message 			
        private String _processMsg = null;
        //	Just Prepared Flag			
        private bool _justPrepared = false;
        //	Lines					
        private MVAMInvInOutLine[] _lines = null;
        // Confirmations			
        private MVAMInvInOutConfirm[] _confirms = null;
        // BPartner				
        private MVABBusinessPartner _partner = null;
        // Reversal Flag		
        public bool _reversal = false;
        private bool set = false;

        private string sql = "";
        private Decimal? trxQty = 0;
        private bool isGetFroMVAMStorage = false;

        MVABOrderLine orderLine = null;
        MVAMProduct productCQ = null;
        decimal amt = 0;
        decimal currentCostPrice = 0;
        string conversionNotFoundInOut = "";
        string conversionNotFoundInOut1 = "";
        string conversionNotFoundInvoice = "";

        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInvInOut).FullName);

        private int tableId1 = 0;
        private int tableId = 0;
        private int VA026_LCDetail_ID = 0;

        ValueNamePair pp = null;

        /**is container applicable */
        private bool isContainrApplicable = false;

        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Inv_InOut_ID"></param>
        /// <param name="trxName">rx name</param>
        public MVAMInvInOut(Ctx ctx, int VAM_Inv_InOut_ID, Trx trxName)
            : base(ctx, VAM_Inv_InOut_ID, trxName)
        {

            if (VAM_Inv_InOut_ID == 0)
            {
                //	setDocumentNo (null);
                //	setVAB_BusinessPartner_ID (0);
                //	setVAB_BPart_Location_ID (0);
                //	setVAM_Warehouse_ID (0);
                //	setVAB_DocTypes_ID (0);
                SetIsSOTrx(false);
                SetMovementDate(DateTime.Now);
                SetDateAcct(GetMovementDate());
                //	setMovementType (MOVEMENTTYPE_CustomerShipment);
                SetDeliveryRule(DELIVERYRULE_Availability);
                SetDeliveryViaRule(DELIVERYVIARULE_Pickup);
                SetFreightCostRule(FREIGHTCOSTRULE_FreightIncluded);
                SetDocStatus(DOCSTATUS_Drafted);
                SetDocAction(DOCACTION_Complete);
                SetPriorityRule(PRIORITYRULE_Medium);
                SetNoPackages(0);
                SetIsInTransit(false);
                SetIsPrinted(false);
                SetSendEMail(false);
                SetIsInDispute(false);
                SetIsReturnTrx(false);
                //
                SetIsApproved(false);
                base.SetProcessed(false);
                SetProcessing(false);
                SetPosted(false);
            }
        }

        /* Create Shipment From Order
        *	@param order order
        *	@param movementDate optional movement date
        *	@param forceDelivery ignore order delivery rule
        *	@param allAttributeInstances if true, all attribute set instances
        *	@param minGuaranteeDate optional minimum guarantee date if all attribute instances
        *	@param complete complete document (Process if false, Complete if true)
        *	@param trxName transaction  
        *	@return Shipment or null
        */
        public static MVAMInvInOut CreateFrom(MVABOrder order, DateTime? movementDate,
            bool forceDelivery, bool allAttributeInstances, DateTime? minGuaranteeDate,
            bool complete, Trx trxName)
        {
            if (order == null)
            {
                throw new ArgumentException("No Order");
            }
            //
            if (!forceDelivery && DELIVERYRULE_CompleteLine.Equals(order.GetDeliveryRule()))
            {
                return null;
            }

            //	Create Meader
            MVAMInvInOut retValue = new MVAMInvInOut(order, 0, movementDate);
            retValue.SetDocAction(complete ? DOCACTION_Complete : DOCACTION_Prepare);

            //	Check if we can create the lines
            MVABOrderLine[] oLines = order.GetLines(true, "VAM_Product_ID");
            for (int i = 0; i < oLines.Length; i++)
            {
                //Decimal qty = oLines[i].GetQtyOrdered().subtract(oLines[i].getQtyDelivered());
                Decimal qty = Decimal.Subtract(oLines[i].GetQtyOrdered(), oLines[i].GetQtyDelivered());
                //	Nothing to deliver
                if (qty == 0)
                {
                    continue;
                }
                //	Stock Info
                MVAMStorage[] storages = null;
                MVAMProduct product = oLines[i].GetProduct();
                if (product != null && product.Get_ID() != 0 && product.IsStocked())
                {
                    MVAMProductCategory pc = MVAMProductCategory.Get(order.GetCtx(), product.GetVAM_ProductCategory_ID());
                    String MMPolicy = pc.GetMMPolicy();
                    if (MMPolicy == null || MMPolicy.Length == 0)
                    {
                        MVAFClient client = MVAFClient.Get(order.GetCtx());
                        MMPolicy = client.GetMMPolicy();
                    }
                    storages = MVAMStorage.GetWarehouse(order.GetCtx(), order.GetVAM_Warehouse_ID(),
                        oLines[i].GetVAM_Product_ID(), oLines[i].GetVAM_PFeature_SetInstance_ID(),
                        product.GetVAM_PFeature_Set_ID(),
                        allAttributeInstances, minGuaranteeDate,
                        MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), trxName);
                }
                if (!forceDelivery)
                {
                    Decimal maxQty = Env.ZERO;
                    for (int ll = 0; ll < storages.Length; ll++)
                        maxQty = Decimal.Add(maxQty, storages[ll].GetQtyOnHand());
                    if (DELIVERYRULE_Availability.Equals(order.GetDeliveryRule()))
                    {
                        if (maxQty.CompareTo(qty) < 0)
                            qty = maxQty;
                    }
                    else if (DELIVERYRULE_CompleteLine.Equals(order.GetDeliveryRule()))
                    {
                        if (maxQty.CompareTo(qty) < 0)
                            continue;
                    }
                }
                //	Create Line
                if (retValue.Get_ID() == 0)	//	not saved yet
                    retValue.Save(trxName);
                //	Create a line until qty is reached
                for (int ll = 0; ll < storages.Length; ll++)
                {
                    Decimal lineQty = storages[ll].GetQtyOnHand();
                    if (lineQty.CompareTo(qty) > 0)
                        lineQty = qty;
                    MVAMInvInOutLine line = new MVAMInvInOutLine(retValue);
                    line.SetOrderLine(oLines[i], storages[ll].GetVAM_Locator_ID(),
                        order.IsSOTrx() ? lineQty : Env.ZERO);
                    line.SetQty(lineQty);	//	Correct UOM for QtyEntered
                    if (oLines[i].GetQtyEntered().CompareTo(oLines[i].GetQtyOrdered()) != 0)
                    {
                        //line.SetQtyEntered(lineQty.multiply(oLines[i].getQtyEntered()).divide(oLines[i].getQtyOrdered(), 12, Decimal.ROUND_HALF_UP));
                        line.SetQtyEntered(Decimal.Multiply(lineQty, Decimal.Divide(oLines[i].GetQtyEntered(), Decimal.Round(oLines[i].GetQtyOrdered(), 12, MidpointRounding.AwayFromZero))));
                    }
                    line.SetVAB_Project_ID(oLines[i].GetVAB_Project_ID());
                    line.Save(trxName);
                    //	Delivered everything ?
                    qty = Decimal.Subtract(qty, lineQty);
                    if (qty == 0)
                    {
                        break;
                    }
                }
            }	//	for all order lines

            //	No Lines saved		
            if (retValue.Get_ID() == 0)
            {
                return null;
            }

            return retValue;
        }

        // New function added for creation of shipment by Vivek on 27/09/2017
        /* Create Shipment Against Drop Shipment
        *	@param order order
        *	@param movementDate optional movement date
        *	@param forceDelivery ignore order delivery rule
        *	@param allAttributeInstances if true, all attribute set instances
        *	@param Warehouse_ID
        *	@param minGuaranteeDate optional minimum guarantee date if all attribute instances      
        *	@param trxName transaction  
        *	@return Shipment or null
        */
        public static MVAMInvInOut CreateShipment(MVABOrder order, MVAMInvInOut inout, DateTime? movementDate, bool forceDelivery,
                    bool allAttributeInstances, int VAM_Warehouse_ID, DateTime? minGuaranteeDate, Trx trxName)
        {
            if (order == null)
            {
                throw new ArgumentException("No Order");
            }

            //	Create Meader
            MVAMInvInOut retValue = new MVAMInvInOut(order, 0, movementDate);
            retValue.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
            retValue.SetIsDropShip(true);
            //	Check if we can create the lines
            MVAMInvInOutLine[] iolines = inout.GetLines(false);

            //MVABOrderLine[] oLines = order.GetLines('
            for (int i = 0; i < iolines.Length; i++)
            {
                MVABOrderLine ol = new MVABOrderLine(inout.GetCtx(), iolines[i].GetVAB_OrderLine_ID(), inout.Get_Trx());
                MVABOrderLine olines = new MVABOrderLine(inout.GetCtx(), ol.GetRef_OrderLine_ID(), inout.Get_Trx());
                Decimal qty = olines.GetQtyEntered();
                //	Nothing to deliver
                if (qty == 0)
                {
                    continue;
                }
                //	Stock Info
                MVAMStorage[] storages = null;
                MVAMProduct product = olines.GetProduct();
                if (product != null && product.Get_ID() != 0 && product.IsStocked())
                {
                    MVAMProductCategory pc = MVAMProductCategory.Get(order.GetCtx(), product.GetVAM_ProductCategory_ID());
                    String MMPolicy = pc.GetMMPolicy();
                    if (MMPolicy == null || MMPolicy.Length == 0)
                    {
                        MVAFClient client = MVAFClient.Get(order.GetCtx());
                        MMPolicy = client.GetMMPolicy();
                    }
                    storages = MVAMStorage.GetWarehouse(order.GetCtx(), VAM_Warehouse_ID,
                        olines.GetVAM_Product_ID(), olines.GetVAM_PFeature_SetInstance_ID(),
                        product.GetVAM_PFeature_Set_ID(),
                        allAttributeInstances, minGuaranteeDate,
                        MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), trxName);
                }
                if (!forceDelivery)
                {
                    Decimal maxQty = Env.ZERO;
                    for (int ll = 0; ll < storages.Length; ll++)
                        maxQty = Decimal.Add(maxQty, storages[ll].GetQtyOnHand());
                    if (DELIVERYRULE_Availability.Equals(order.GetDeliveryRule()))
                    {
                        if (maxQty.CompareTo(qty) < 0)
                            qty = maxQty;
                    }
                    else if (DELIVERYRULE_CompleteLine.Equals(order.GetDeliveryRule()))
                    {
                        if (maxQty.CompareTo(qty) < 0)
                            continue;
                    }
                }
                //	Create Line
                if (retValue.Get_ID() == 0)	//	not saved yet
                    retValue.Save(trxName);
                //	Create a line until qty is reached
                for (int ll = 0; ll < storages.Length; ll++)
                {
                    Decimal lineQty = storages[ll].GetQtyOnHand();
                    if (lineQty.CompareTo(qty) > 0)
                        lineQty = qty;
                    MVAMInvInOutLine line = new MVAMInvInOutLine(retValue);
                    line.SetIsDropShip(true);

                    line.SetOrderLine(olines, storages[ll].GetVAM_Locator_ID(),
                        order.IsSOTrx() ? lineQty : Env.ZERO);
                    line.SetQty(lineQty);	//	Correct UOM for QtyEntered
                    if (olines.GetQtyEntered().CompareTo(olines.GetQtyEntered()) != 0)
                    {
                        //line.SetQtyEntered(lineQty.multiply(olines.getQtyEntered()).divide(olines.getQtyOrdered(), 12, Decimal.ROUND_HALF_UP));
                        line.SetQtyEntered(Decimal.Multiply(lineQty, Decimal.Divide(olines.GetQtyEntered(), Decimal.Round(olines.GetQtyEntered(), 12, MidpointRounding.AwayFromZero))));
                    }
                    line.SetVAB_Project_ID(olines.GetVAB_Project_ID());
                    line.Save(trxName);
                    //	Delivered everything ?
                    qty = Decimal.Subtract(qty, lineQty);
                    if (qty == 0)
                    {
                        break;
                    }
                }
            }	//	for all order lines

            //	No Lines saved		
            if (retValue.Get_ID() == 0)
            {
                return null;
            }

            return retValue;
        }

        /**
         * 	Create new Shipment by copying
         * 	@param from shipment
         * 	@param dateDoc date of the document date
         * 	@param VAB_DocTypes_ID doc type
         * 	@param isSOTrx sales order
         * 	@param counter create counter links
         * 	@param trxName trx
         * 	@param setOrder set the order link
         *	@return Shipment
         */
        public static MVAMInvInOut CopyFrom(MVAMInvInOut from, DateTime? dateDoc,
            int VAB_DocTypes_ID, bool isSOTrx, bool isReturnTrx,
            bool counter, Trx trxName, bool setOrder)
        {
            MVAMInvInOut to = new MVAMInvInOut(from.GetCtx(), 0, null);
            to.Set_TrxName(trxName);
            CopyValues(from, to, from.GetVAF_Client_ID(), from.GetVAF_Org_ID());
            to.Set_ValueNoCheck("VAM_Inv_InOut_ID", I_ZERO);
            to.Set_ValueNoCheck("DocumentNo", null);
            //
            to.SetDocStatus(DOCSTATUS_Drafted);		//	Draft
            to.SetDocAction(DOCACTION_Complete);
            //
            to.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
            to.SetIsReturnTrx(isReturnTrx);
            to.SetIsSOTrx(isSOTrx);
            if (counter)
            {
                if (!isReturnTrx)
                {
                    to.SetMovementType(isSOTrx ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
                }
                else
                {
                    to.SetMovementType(isSOTrx ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);
                }
            }

            // JID_0799: Order date should be original order date.
            to.SetDateOrdered(from.GetDateOrdered());
            to.SetDateAcct(dateDoc);
            to.SetMovementDate(dateDoc);
            to.SetDatePrinted(null);
            to.SetIsPrinted(false);
            to.SetDateReceived(null);
            to.SetNoPackages(0);
            to.SetShipDate(null);
            to.SetPickDate(null);
            to.SetIsInTransit(false);
            //
            to.SetIsApproved(false);
            to.SetVAB_Invoice_ID(0);
            to.SetTrackingNo(null);
            to.SetIsInDispute(false);
            //
            to.SetPosted(false);
            to.SetProcessed(false);
            to.SetVAB_Order_ID(0);	//	Overwritten by setOrder
            if (counter)
            {
                to.SetVAB_Order_ID(0);
                to.SetRef_InOut_ID(from.GetVAM_Inv_InOut_ID());
                //	Try to find Order/Invoice link
                if (from.GetVAB_Order_ID() != 0)
                {
                    MVABOrder peer = new MVABOrder(from.GetCtx(), from.GetVAB_Order_ID(), from.Get_TrxName());
                    if (peer.GetRef_Order_ID() != 0)
                        to.SetVAB_Order_ID(peer.GetRef_Order_ID());
                }
                if (from.GetVAB_Invoice_ID() != 0)
                {
                    MVABInvoice peer = new MVABInvoice(from.GetCtx(), from.GetVAB_Invoice_ID(), from.Get_TrxName());
                    if (peer.GetRef_Invoice_ID() != 0)
                        to.SetVAB_Invoice_ID(peer.GetRef_Invoice_ID());
                }
            }
            else
            {
                to.SetRef_InOut_ID(0);
                if (setOrder)
                    to.SetVAB_Order_ID(from.GetVAB_Order_ID());
            }

            // for copy document set Temp Document No to empty
            if (to.Get_ColumnIndex("TempDocumentNo") > 0)
            {
                to.SetTempDocumentNo("");
            }

            if (!counter)
            {
                //for Reverse Case handle 8-jan-2015
                to.SetDescription("RC");
                // when we reverse a record then to set
                to.SetIsReversal(true);
            }
            //
            if (!to.Save(trxName))
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    to._processMsg = "Could not create Shipment, " + pp.GetName();
                else
                    to._processMsg = "Could not create Shipment";

                throw new Exception(to._processMsg);
            }
            if (counter)
            {
                from.SetRef_InOut_ID(to.GetVAM_Inv_InOut_ID());
            }

            // Check applied by Mohit - JID_1640 - 18 Feb 2020
            // in case of counter document - do not create the lines here - it will be created while creating the counter document.
            // in case of reversal, need to copy the lines as well
            if (!counter && to.CopyLinesFrom(from, counter, setOrder) == 0)
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    to._processMsg = "Could not create Shipment Lines, " + pp.GetName();
                else
                    to._processMsg = "Could not create Shipment Lines";

                throw new Exception(to._processMsg);
            }

            return to;
        }

        /**
         *  Load Constructor
         *  @param ctx context
         *  @param dr result set record
         *	@param trxName transaction  
         */
        public MVAMInvInOut(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Order Constructor - create header only
         *	@param order order
         *	@param movementDate optional movement date (default today)
         *	@param VAB_DocTypesShipment_ID document type or 0
         */
        public MVAMInvInOut(MVABOrder order, int VAB_DocTypesShipment_ID, DateTime? movementDate)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {

            SetOrder(order);

            if (VAB_DocTypesShipment_ID == 0)
            {
                VAB_DocTypesShipment_ID = VAdvantage.Utility.Util.GetValueOfInt(ExecuteQuery.ExecuteScalar("SELECT VAB_DocTypesShipment_ID FROM VAB_DocTypes WHERE VAB_DocTypes_ID=" + order.GetVAB_DocTypesTarget_ID()));
            }
            SetVAB_DocTypes_ID(VAB_DocTypesShipment_ID, true);

            //	Default - Today
            if (movementDate != null)
            {
                SetMovementDate(movementDate);
            }
            SetDateAcct(GetMovementDate());

        }



        /**
         * 	Invoice Constructor - create header only
         *	@param invoice invoice
         *	@param VAB_DocTypesShipment_ID document type or 0
         *	@param movementDate optional movement date (default today)
         *	@param VAM_Warehouse_ID warehouse
         */
        public MVAMInvInOut(MVABInvoice invoice, int VAB_DocTypesShipment_ID,
            DateTime? movementDate, int VAM_Warehouse_ID)
            : this(invoice.GetCtx(), 0, invoice.Get_TrxName())
        {
            SetClientOrg(invoice);
            MVABOrder ord = new MVABOrder(GetCtx(), invoice.GetVAB_Order_ID(), null);
            SetVAB_BusinessPartner_ID(ord.GetVAB_BusinessPartner_ID());
            //SetVAB_BusinessPartner_ID(invoice.GetVAB_BusinessPartner_ID());
            SetVAB_BPart_Location_ID(ord.GetVAB_BPart_Location_ID());	//	shipment address
            SetVAF_UserContact_ID(ord.GetVAF_UserContact_ID());
            //
            SetVAM_Warehouse_ID(VAM_Warehouse_ID);
            SetIsSOTrx(invoice.IsSOTrx());
            SetIsReturnTrx(invoice.IsReturnTrx());

            if (!IsReturnTrx())
                SetMovementType(invoice.IsSOTrx() ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
            else
                SetMovementType(invoice.IsSOTrx() ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);

            MVABOrder order = null;
            if (invoice.GetVAB_Order_ID() != 0)
                order = new MVABOrder(invoice.GetCtx(), invoice.GetVAB_Order_ID(), invoice.Get_TrxName());
            if (VAB_DocTypesShipment_ID == 0 && order != null)
                VAB_DocTypesShipment_ID = int.Parse(ExecuteQuery.ExecuteScalar("SELECT VAB_DocTypesShipment_ID FROM VAB_DocTypes WHERE VAB_DocTypes_ID=" + order.GetVAB_DocTypes_ID()));
            if (VAB_DocTypesShipment_ID != 0)
                SetVAB_DocTypes_ID(VAB_DocTypesShipment_ID, true);
            else
                SetVAB_DocTypes_ID();

            //	Default - Today
            if (movementDate != null)
                SetMovementDate(movementDate);
            SetDateAcct(GetMovementDate());

            //	Copy from Invoice
            SetVAB_Order_ID(invoice.GetVAB_Order_ID());
            SetSalesRep_ID(invoice.GetSalesRep_ID());
            //
            SetVAB_BillingCode_ID(invoice.GetVAB_BillingCode_ID());
            SetVAB_Promotion_ID(invoice.GetVAB_Promotion_ID());
            SetVAB_Charge_ID(invoice.GetVAB_Charge_ID());
            SetChargeAmt(invoice.GetChargeAmt());
            //
            SetVAB_Project_ID(invoice.GetVAB_Project_ID());
            SetDateOrdered(invoice.GetDateOrdered());
            SetDescription(invoice.GetDescription());
            SetPOReference(invoice.GetPOReference());
            SetVAF_OrgTrx_ID(invoice.GetVAF_OrgTrx_ID());
            SetUser1_ID(invoice.GetUser1_ID());
            SetUser2_ID(invoice.GetUser2_ID());

            if (order != null)
            {
                SetDeliveryRule(order.GetDeliveryRule());
                SetDeliveryViaRule(order.GetDeliveryViaRule());
                SetVAM_ShippingMethod_ID(order.GetVAM_ShippingMethod_ID());
                SetFreightCostRule(order.GetFreightCostRule());
                SetFreightAmt(order.GetFreightAmt());
            }
        }

        /**
         * 	Copy Constructor - create header only
         *	@param original original 
         *	@param movementDate optional movement date (default today)
         *	@param VAB_DocTypesShipment_ID document type or 0
         */
        public MVAMInvInOut(MVAMInvInOut original, int VAB_DocTypesShipment_ID, DateTime? movementDate)
            : this(original.GetCtx(), 0, original.Get_TrxName())
        {

            SetClientOrg(original);
            SetVAB_BusinessPartner_ID(original.GetVAB_BusinessPartner_ID());
            SetVAB_BPart_Location_ID(original.GetVAB_BPart_Location_ID());	//	shipment address
            SetVAF_UserContact_ID(original.GetVAF_UserContact_ID());
            //
            SetVAM_Warehouse_ID(original.GetVAM_Warehouse_ID());
            SetIsSOTrx(original.IsSOTrx());
            SetMovementType(original.GetMovementType());
            if (VAB_DocTypesShipment_ID == 0)
            {
                SetVAB_DocTypes_ID(original.GetVAB_DocTypes_ID());
                SetIsReturnTrx(original.IsReturnTrx());
            }
            else
                SetVAB_DocTypes_ID(VAB_DocTypesShipment_ID, true);

            //	Default - Today
            if (movementDate != null)
                SetMovementDate(movementDate);
            SetDateAcct(GetMovementDate());

            //	Copy from Order
            SetVAB_Order_ID(original.GetVAB_Order_ID());
            SetDeliveryRule(original.GetDeliveryRule());
            SetDeliveryViaRule(original.GetDeliveryViaRule());
            SetVAM_ShippingMethod_ID(original.GetVAM_ShippingMethod_ID());
            SetFreightCostRule(original.GetFreightCostRule());
            SetFreightAmt(original.GetFreightAmt());
            SetSalesRep_ID(original.GetSalesRep_ID());
            //
            SetVAB_BillingCode_ID(original.GetVAB_BillingCode_ID());
            SetVAB_Promotion_ID(original.GetVAB_Promotion_ID());
            SetVAB_Charge_ID(original.GetVAB_Charge_ID());
            SetChargeAmt(original.GetChargeAmt());
            //
            SetVAB_Project_ID(original.GetVAB_Project_ID());
            SetDateOrdered(original.GetDateOrdered());
            SetDescription(original.GetDescription());
            SetPOReference(original.GetPOReference());
            SetSalesRep_ID(original.GetSalesRep_ID());
            SetVAF_OrgTrx_ID(original.GetVAF_OrgTrx_ID());
            SetUser1_ID(original.GetUser1_ID());
            SetUser2_ID(original.GetUser2_ID());
        }

        /**
         * 	Get Document Status
         *	@return Document Status Clear Text
         */
        public String GetDocStatusName()
        {
            return MVAFCtrlRefList.GetListName(GetCtx(), 131, GetDocStatus());
        }

        /**
         * 	Add to Description
         *	@param description text
         */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /**
         *	String representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInvInOut[")
                .Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append(",DocStatus=").Append(GetDocStatus())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info (untranslated)
         */
        public String GetDocumentInfo()
        {
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /**
         * 	Create PDF
         *	@return File or null
         */
        public FileInfo CreatePDF()
        {
            try
            {
                //string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                String fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo() + ".pdf";
                string filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload", fileName);


                ReportEngine_N re = ReportEngine_N.Get(GetCtx(), ReportEngine_N.SHIPMENT, GetVAB_Invoice_ID());
                if (re == null)
                    return null;

                re.GetView();
                bool b = re.CreatePDF(filePath);

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    re.CreatePDF(filePath);
                    return new FileInfo(filePath);
                }
                else
                    return temp;
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {

            //ReportEngine re = ReportEngine.get(getCtx(), ReportEngine.SHIPMENT, getVAB_Invoice_ID());
            //if (re == null)
            //    return null;
            //return re.getPDF(file);
            return null;
        }

        /**
         * 	Get Lines of Shipment
         * 	@param requery refresh from db
         * 	@return lines
         */
        public MVAMInvInOutLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MVAMInvInOutLine> list = new List<MVAMInvInOutLine>();
            String sql = "SELECT * FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID=" + GetVAM_Inv_InOut_ID() + " ORDER BY Line";
            DataSet ds = null;
            DataRow dr = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMInvInOutLine(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
                list = null;
            }
            ds = null;
            //
            if (list == null)
                return null;
            _lines = new MVAMInvInOutLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /**
         * 	Get Lines of Shipment
         * 	@return lines
         */
        public MVAMInvInOutLine[] GetLines()
        {
            return GetLines(false);
        }

        /**
         * 	Get Confirmations
         * 	@param requery requery
         *	@return array of Confirmations
         */
        public MVAMInvInOutConfirm[] GetConfirmations(bool requery)
        {
            if (_confirms != null && !requery)
                return _confirms;

            List<MVAMInvInOutConfirm> list = new List<MVAMInvInOutConfirm>();
            String sql = "SELECT * FROM VAM_Inv_InOutConfirm WHERE VAM_Inv_InOut_ID=" + GetVAM_Inv_InOut_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMInvInOutConfirm(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }
            _confirms = new MVAMInvInOutConfirm[list.Count];
            _confirms = list.ToArray();
            return _confirms;
        }

        //Pratap VAWMS 31-8-2015
        public Boolean AddServiceLines()
        {
            String sql = "SELECT VAB_OrderLine_ID "
                          + " FROM VAB_OrderLine ol"
                          + " LEFT OUTER JOIN VAM_Product p ON (ol.VAM_Product_ID=p.VAM_Product_ID)"
                          + " WHERE ol.VAB_Order_ID=@param1"
                          + " AND (ol.VAM_Product_ID IS NULL"
                          + " OR p.IsStocked = 'N'"
                          + " OR p.ProductType != 'I')"
                          + " AND (QtyOrdered=0 OR (QtyOrdered > QtyDelivered))"
                          + " AND NOT EXISTS (SELECT 1 FROM VAM_Inv_InOut io "
                          + " INNER JOIN VAM_Inv_InOutLine iol ON (io.VAM_Inv_InOut_ID=iol.VAM_Inv_InOut_ID)"
                          + " WHERE io.VAM_Inv_InOut_ID=@param2"
                          + " AND iol.VAB_OrderLine_ID=ol.VAB_OrderLine_ID)"
                          + " ORDER BY VAB_OrderLine_ID";
            //PreparedStatement pstmt = null;
            //ResultSet rs = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", GetVAB_Order_ID());
                param[1] = new SqlParameter("@param2", GetVAM_Inv_InOut_ID());
                idr = DB.ExecuteReader(sql, param, Get_Trx());
                //pstmt = DB.prepareStatement(sql, get_Trx());
                //pstmt.setInt(1, getVAB_Order_ID());
                //pstmt.setInt(2, getVAM_Inv_InOut_ID());
                //rs = pstmt.executeQuery();
                ArrayList serviceLines = new ArrayList();// ArrayList<MVAMInvInOutLine> serviceLines = new ArrayList<MVAMInvInOutLine>();
                ////while (rs.next())
                ////{
                ////    //list.add(new MVAMInvInOutLine(getCtx(), rs, get_Trx()));
                while (idr.Read())
                {
                    // int VAB_OrderLine_ID = rs.getInt(1);
                    int VAB_OrderLine_ID = VAdvantage.Utility.Util.GetValueOfInt(idr[0]);
                    MVABOrderLine oLine = new MVABOrderLine(GetCtx(), VAB_OrderLine_ID, Get_TrxName());
                    MVAMInvInOutLine line = new MVAMInvInOutLine(this);
                    line.SetOrderLine(oLine, 0, Env.ZERO);
                    // BigDecimal qty = oLine.getQtyOrdered().subtract(oLine.getQtyDelivered());
                    Decimal qty = Decimal.Subtract(oLine.GetQtyOrdered(), (oLine.GetQtyDelivered()));
                    //if (qty != null && qty.signum() > 0)
                    //{
                    //    line.SetQty(qty);
                    //    if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                    //       line.SetQtyEntered(qty.multiply(oLine.GetQtyEntered()).divide(oLine.GetQtyOrdered(), 12, BigDecimal.ROUND_HALF_UP));
                    //}
                    if (qty != null && Env.Signum(qty) > 0)
                    {
                        line.SetQty(qty);
                        if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                        {
                            line.SetQtyEntered(
                                Decimal.Multiply(qty, Decimal.Round(Decimal.Divide(oLine.GetQtyEntered(),
                               (oLine.GetQtyOrdered())), 12)));

                        }
                    }
                    /**********************************/
                    if (!line.Save(Get_TrxName()))
                    {
                        log.Log(Level.SEVERE, "Could not save service lines");
                        return false;
                    }

                    /**********************************/
                    serviceLines.Add(line);
                    log.Fine(line.ToString());
                }




                // Commented ***********************
                //if (!PO.SaveAll(Get_TrxName(), serviceLines))
                //{
                //    log.Log(Level.SEVERE, "Could not save service lines");
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, ex);
                // throw new DBException(ex);
            }
            ////finally
            ////{
            ////    DB.closeResultSet(rs);
            ////    DB.closeStatement(pstmt);
            ////}
            return true;
        }
        //

        /**
         * 	Copy Lines From other Shipment
         *	@param otherShipment shipment
         *	@param counter set counter Info
         *	@param setOrder set order link
         *	@return number of lines copied
         */
        public int CopyLinesFrom(MVAMInvInOut otherShipment, bool counter, bool setOrder)
        {
            if (IsProcessed() || IsPosted() || otherShipment == null)
                return 0;
            MVAMInvInOutLine[] fromLines = otherShipment.GetLines(false);
            int count = 0;
            for (int i = 0; i < fromLines.Length; i++)
            {
                MVAMInvInOutLine line = new MVAMInvInOutLine(this);
                MVAMInvInOutLine fromLine = fromLines[i];
                line.Set_TrxName(Get_TrxName());
                if (counter)	//	header
                    PO.CopyValues(fromLine, line, GetVAF_Client_ID(), GetVAF_Org_ID());
                else
                    PO.CopyValues(fromLine, line, fromLine.GetVAF_Client_ID(), fromLine.GetVAF_Org_ID());
                line.SetVAM_Inv_InOut_ID(GetVAM_Inv_InOut_ID());
                line.Set_ValueNoCheck("VAM_Inv_InOutLine_ID", I_ZERO);	//	new
                //	Reset
                if (!setOrder)
                    line.SetVAB_OrderLine_ID(0);
                // SI_0642 : when we reverse MR or Customer Return, at that tym - on Save - system also check - qty availablity agaisnt same attribute 
                // on storage. If we set ASI as 0, then system not find qty and not able to save record
                if (!counter && !IsReversal())
                    line.SetVAM_PFeature_SetInstance_ID(0);
                //	line.setVAS_Res_Assignment_ID(0);
                line.SetRef_InOutLine_ID(0);
                line.SetIsInvoiced(false);
                //
                line.SetConfirmedQty(Env.ZERO);
                line.SetPickedQty(Env.ZERO);
                line.SetScrappedQty(Env.ZERO);
                line.SetTargetQty(Env.ZERO);
                //	Set Locator based on header Warehouse
                if (GetVAM_Warehouse_ID() != otherShipment.GetVAM_Warehouse_ID())
                {
                    line.SetVAM_Locator_ID(0);
                    line.SetVAM_Locator_ID((int)Env.ZERO);
                }
                //
                if (counter)
                {
                    line.SetVAB_OrderLine_ID(0);
                    line.SetRef_InOutLine_ID(fromLine.GetVAM_Inv_InOutLine_ID());
                    if (fromLine.GetVAB_OrderLine_ID() != 0)
                    {
                        MVABOrderLine peer = new MVABOrderLine(GetCtx(), fromLine.GetVAB_OrderLine_ID(), Get_TrxName());
                        if (peer.GetRef_OrderLine_ID() != 0)
                            line.SetVAB_OrderLine_ID(peer.GetRef_OrderLine_ID());
                    }
                }
                //
                if (IsReversal())
                {
                    line.SetQtyEntered(Decimal.Negate(line.GetQtyEntered()));
                    line.SetMovementQty(Decimal.Negate(line.GetMovementQty()));
                    if (line.Get_ColumnIndex("ReversalDoc_ID") > 0)
                    {
                        line.SetReversalDoc_ID(fromLine.GetVAM_Inv_InOutLine_ID());
                    }
                    // to set OrderLine in case of reversal if it is available 
                    line.SetVAB_OrderLine_ID(fromLine.GetVAB_OrderLine_ID());
                    //set container reference(if, not a copy record)
                    line.SetVAM_ProductContainer_ID(fromLine.GetVAM_ProductContainer_ID());
                }
                line.SetProcessed(false);
                if (line.Save(Get_TrxName()))
                    count++;
                //	Cross Link
                if (counter)
                {
                    fromLine.SetRef_InOutLine_ID(line.GetVAM_Inv_InOutLine_ID());
                    fromLine.Save(Get_TrxName());
                }
            }
            if (fromLines.Length != count)
            {
                log.Log(Level.SEVERE, "Line difference - From=" + fromLines.Length + " <> Saved=" + count);
            }
            return count;
        }

        /**
         * 	Set Reversal
         *	@param reversal reversal
         */
        //private void SetReversal(bool reversal)
        //{
        //    _reversal = reversal;
        //}

        /// <summary>
        /// Is Reversal       
        /// now we take physical column, so not required this column
        /// </summary>
        /// <returns>_reversal : TRUE if reversed record</returns>
        //public bool IsReversal()
        //{
        //    return _reversal;
        //}

        /**
         * 	Copy from Order
         *	@param order order
         */
        private void SetOrder(MVABOrder order)
        {
            SetClientOrg(order);
            SetVAB_Order_ID(order.GetVAB_Order_ID());
            //
            SetVAB_BusinessPartner_ID(order.GetVAB_BusinessPartner_ID());
            SetVAB_BPart_Location_ID(order.GetVAB_BPart_Location_ID());	//	shipment address
            SetVAF_UserContact_ID(order.GetVAF_UserContact_ID());
            //
            SetVAM_Warehouse_ID(order.GetVAM_Warehouse_ID());
            SetIsSOTrx(order.IsSOTrx());
            SetIsReturnTrx(order.IsReturnTrx());

            if (!IsReturnTrx())
                SetMovementType(order.IsSOTrx() ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
            else
                SetMovementType(order.IsSOTrx() ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);
            //
            SetDeliveryRule(order.GetDeliveryRule());
            SetDeliveryViaRule(order.GetDeliveryViaRule());
            SetVAM_ShippingMethod_ID(order.GetVAM_ShippingMethod_ID());
            SetFreightCostRule(order.GetFreightCostRule());
            SetFreightAmt(order.GetFreightAmt());
            SetSalesRep_ID(order.GetSalesRep_ID());
            //
            SetVAB_BillingCode_ID(order.GetVAB_BillingCode_ID());
            SetVAB_Promotion_ID(order.GetVAB_Promotion_ID());
            SetVAB_Charge_ID(order.GetVAB_Charge_ID());
            SetChargeAmt(order.GetChargeAmt());
            //
            SetVAB_Project_ID(order.GetVAB_Project_ID());
            SetDateOrdered(order.GetDateOrdered());
            SetDescription(order.GetDescription());
            SetPOReference(order.GetPOReference());
            SetSalesRep_ID(order.GetSalesRep_ID());
            SetVAF_OrgTrx_ID(order.GetVAF_OrgTrx_ID());
            SetUser1_ID(order.GetUser1_ID());
            SetUser2_ID(order.GetUser2_ID());

        }

        /**
         * 	Set Order - Callout
         *	@param oldVAB_Order_ID old BP
         *	@param newVAB_Order_ID new BP
         *	@param windowNo window no
         */
        //@UICallout Web user interface method
        public void SetVAB_Order_ID(String oldVAB_Order_ID, String newVAB_Order_ID, int windowNo)
        {
            if (newVAB_Order_ID == null || newVAB_Order_ID.Length == 0)
                return;
            int VAB_Order_ID = int.Parse(newVAB_Order_ID);
            if (VAB_Order_ID == 0)
                return;
            //	Get Details
            MVABOrder order = new MVABOrder(GetCtx(), VAB_Order_ID, null);
            if (order.Get_ID() != 0)
                SetOrder(order);
        }

        /**
         * 	Set Processed.
         * 	Propergate to Lines/Taxes
         *	@param processed processed
         */
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String sql = "UPDATE VAM_Inv_InOutLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE VAM_Inv_InOut_ID=" + GetVAM_Inv_InOut_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - Lines=" + noLine);
        }

        /**
         * 	Get BPartner
         *	@return partner
         */
        public MVABBusinessPartner GetBPartner()
        {
            if (_partner == null)
                _partner = new MVABBusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), Get_TrxName());
            return _partner;
        }

        /**
         * 	Set Document Type
         * 	@param DocBaseType doc type MVABMasterDocType.DOCBASETYPE_
         */
        public void SetVAB_DocTypes_ID(String DocBaseType)
        {
            String sql = "SELECT VAB_DocTypes_ID FROM VAB_DocTypes "
                + "WHERE VAF_Client_ID=" + GetVAF_Client_ID() + " AND DocBaseType=" + DocBaseType
                + " AND IsActive='Y' AND IsReturnTrx='N'"
                + " AND IsSOTrx='" + (IsSOTrx() ? "Y" : "N") + "' "
                + "ORDER BY IsDefault DESC";
            int VAB_DocTypes_ID = int.Parse(ExecuteQuery.ExecuteScalar(sql));
            if (VAB_DocTypes_ID <= 0)
            {
                log.Log(Level.SEVERE, "Not found for AC_Client_ID="
                     + GetVAF_Client_ID() + " - " + DocBaseType);
            }
            else
            {
                log.Fine("DocBaseType=" + DocBaseType + " - VAB_DocTypes_ID=" + VAB_DocTypes_ID);
                SetVAB_DocTypes_ID(VAB_DocTypes_ID);
                bool isSOTrx = MVABMasterDocType.DOCBASETYPE_MATERIALDELIVERY.Equals(DocBaseType);
                SetIsSOTrx(isSOTrx);
                SetIsReturnTrx(false);
            }
        }

        /**
         * 	Set Default VAB_DocTypes_ID.
         * 	Based on SO flag
         */
        public void SetVAB_DocTypes_ID()
        {
            if (IsSOTrx())
                SetVAB_DocTypes_ID(MVABMasterDocType.DOCBASETYPE_MATERIALDELIVERY);
            else
                SetVAB_DocTypes_ID(MVABMasterDocType.DOCBASETYPE_MATERIALRECEIPT);
        }

        /**
         * 	Set Document Type
         *	@param VAB_DocTypes_ID dt
         *	@param setReturnTrx if true set IsRteurnTrx and SOTrx
         */
        public void SetVAB_DocTypes_ID(int VAB_DocTypes_ID, bool setReturnTrx)
        {
            base.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
            if (setReturnTrx)
            {
                MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), VAB_DocTypes_ID);
                SetIsReturnTrx(dt.IsReturnTrx());
                SetIsSOTrx(dt.IsSOTrx());
            }
        }

        /**
         * 	Set Document Type - Callout.
         * 	Sets MovementType, DocumentNo
         * 	@param oldVAB_DocTypes_ID old ID
         * 	@param newVAB_DocTypes_ID new ID
         * 	@param windowNo window
         */
        //	@UICallout
        public void SetVAB_DocTypes_ID(String oldVAB_DocTypes_ID,
               String newVAB_DocTypes_ID, int windowNo)
        {
            if (newVAB_DocTypes_ID == null || newVAB_DocTypes_ID.Length == 0)
                return;
            int VAB_DocTypes_ID = int.Parse(newVAB_DocTypes_ID);
            if (VAB_DocTypes_ID == 0)
                return;
            String sql = "SELECT d.DocBaseType, d.IsDocNoControlled, s.CurrentNext, d.IsReturnTrx "
                + "FROM VAB_DocTypes d, VAF_Record_Seq s "
                + "WHERE VAB_DocTypes_ID=" + VAB_DocTypes_ID		//	1
                + " AND d.DocNoSequence_ID=s.VAF_Record_Seq_ID(+)";
            try
            {
                DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetVAB_DocTypes_ID(VAB_DocTypes_ID);
                    /********************************************************************************************/
                    /********************************************************************************************/
                    //Consequences of - Field Value Changes	- New Row- Save (Update/Insert) Row
                    //Set Ctx - add to changed Ctx
                    //p_changeVO.setContext(getCtx(), windowNo, "VAB_DocTypesTarget_ID", VAB_DocTypes_ID);
                    //	Set Movement Type
                    String DocBaseType = dr["DocBaseType"].ToString();
                    Boolean IsReturnTrx = "Y".Equals(dr[3].ToString());

                    if (DocBaseType.Equals(MVABMasterDocType.DOCBASETYPE_MATERIALDELIVERY))		//	Shipments
                    {
                        if (IsReturnTrx)
                            SetMovementType(MOVEMENTTYPE_CustomerReturns);
                        else
                            SetMovementType(MOVEMENTTYPE_CustomerShipment);
                    }
                    else if (DocBaseType.Equals(MVABMasterDocType.DOCBASETYPE_MATERIALRECEIPT))	//	Receipts
                    {
                        if (IsReturnTrx)
                            SetMovementType(MOVEMENTTYPE_VendorReturns);
                        else
                            SetMovementType(MOVEMENTTYPE_VendorReceipts);
                    }

                    //	DocumentNo
                    if (dr["IsDocNoControlled"].ToString().Equals("Y"))
                        SetDocumentNo("<" + dr["CurrentNext"].ToString() + ">");
                    SetIsReturnTrx(IsReturnTrx);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
        }

        /**
         * 	Set Business Partner Defaults & Details
         * 	@param bp business partner
         */
        public void SetBPartner(MVABBusinessPartner bp)
        {
            if (bp == null)
                return;

            SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());

            //	Set Locations*******************************************************************************
            MVABBPartLocation[] locs = bp.GetLocations(false);
            if (locs != null)
            {
                for (int i = 0; i < locs.Length; i++)
                {
                    if (locs[i].IsShipTo())
                        SetVAB_BPart_Location_ID(locs[i].GetVAB_BPart_Location_ID());
                }
                //	set to first if not set
                if (GetVAB_BPart_Location_ID() == 0 && locs.Length > 0)
                    SetVAB_BPart_Location_ID(locs[0].GetVAB_BPart_Location_ID());
            }
            if (GetVAB_BPart_Location_ID() == 0)
            {
                log.Log(Level.SEVERE, "Has no To Address: " + bp);
            }

            //	Set Contact
            MVAFUserContact[] contacts = bp.GetContacts(false);
            if (contacts != null && contacts.Length > 0)	//	get first User
                SetVAF_UserContact_ID(contacts[0].GetVAF_UserContact_ID());
        }

        /// <summary>
        /// Set Business Partner - Callout
        /// </summary>
        /// <param name="oldVAB_BusinessPartner_ID">old BP</param>
        /// <param name="newVAB_BusinessPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetVAB_BusinessPartner_ID(String oldVAB_BusinessPartner_ID,
               String newVAB_BusinessPartner_ID, int windowNo)
        {
            if (newVAB_BusinessPartner_ID == null || newVAB_BusinessPartner_ID.Length == 0)
                return;
            int VAB_BusinessPartner_ID = int.Parse(newVAB_BusinessPartner_ID);
            if (VAB_BusinessPartner_ID == 0)
                return;
            String sql = "SELECT p.VAF_Language, p.POReference,"
                + "SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "l.VAB_BPart_Location_ID, c.VAF_UserContact_ID "
                + "FROM VAB_BusinessPartner p"
                + " LEFT OUTER JOIN VAB_BPart_Location l ON (p.VAB_BusinessPartner_ID=l.VAB_BusinessPartner_ID)"
                + " LEFT OUTER JOIN VAF_UserContact c ON (p.VAB_BusinessPartner_ID=c.VAB_BusinessPartner_ID) "
                + "WHERE p.VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID;		//	1
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
                    //	Location
                    int ii = (int)dr["VAB_BPart_Location_ID"];
                    if (ii != 0)
                        SetVAB_BPart_Location_ID(ii);
                    //	Contact
                    ii = (int)dr["VAF_UserContact_ID"];
                    SetVAF_UserContact_ID(ii);

                    //	CreditAvailable
                    if (IsSOTrx() && !IsReturnTrx())
                    {
                        Decimal CreditLimit = Convert.ToDecimal(dr["SO_CreditLimit"]);
                        if (CreditLimit != null && CreditLimit != 0)
                        {
                            Decimal CreditAvailable = Convert.ToDecimal(dr["CreditAvailable"]);
                            //if (p_changeVO != null && CreditAvailable != null && CreditAvailable < 0)
                            {
                                String msg = Msg.Translate(GetCtx(), "CreditLimitOver");//,DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable));
                                //p_changeVO.addError(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
        }

        /// <summary>
        /// Set Movement Date - Callout
        /// </summary>
        /// <param name="oldVAB_BusinessPartner_ID">old BP</param>
        /// <param name="newVAB_BusinessPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetMovementDate(String oldMovementDate,
            String newMovementDate, int windowNo)
        {
            if (newMovementDate == null || newMovementDate.Length == 0)
                return;
            //		DateTime movementDate = PO.convertToTimestamp(newMovementDate);
            DateTime movementDate = Convert.ToDateTime(newMovementDate);
            if (movementDate == null)
                return;
            SetMovementDate(movementDate);
            SetDateAcct(movementDate);
        }

        /// <summary>
        /// Set Warehouse and check/set Organization
        /// </summary>
        /// <param name="VAM_Warehouse_ID">id</param>
        public void SetVAM_Warehouse_ID(int VAM_Warehouse_ID)
        {
            if (VAM_Warehouse_ID == 0)
            {
                log.Severe("Ignored - Cannot set AD_Warehouse_ID to 0");
                return;
            }
            base.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
            //
            MWarehouse wh = MWarehouse.Get(GetCtx(), GetVAM_Warehouse_ID());
            if (wh.GetVAF_Org_ID() != GetVAF_Org_ID())
            {
                log.Warning("VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + ", Overwritten VAF_Org_ID=" + GetVAF_Org_ID() + "->" + wh.GetVAF_Org_ID());
                SetVAF_Org_ID(wh.GetVAF_Org_ID());
            }
        }

        /// <summary>
        /// Set Business Partner - Callout
        /// </summary>
        /// <param name="oldVAB_BusinessPartner_ID">old BP</param>
        /// <param name="newVAB_BusinessPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetVAM_Warehouse_ID(String oldVAM_Warehouse_ID,
            String newVAM_Warehouse_ID, int windowNo)
        {
            if (newVAM_Warehouse_ID == null || newVAM_Warehouse_ID.Length == 0)
                return;
            int VAM_Warehouse_ID = int.Parse(newVAM_Warehouse_ID);
            if (VAM_Warehouse_ID == 0)
                return;
            //
            String sql = "SELECT w.VAF_Org_ID, l.VAM_Locator_ID "
                + "FROM VAM_Warehouse w"
                + " LEFT OUTER JOIN VAM_Locator l ON (l.VAM_Warehouse_ID=w.VAM_Warehouse_ID AND l.IsDefault='Y') "
                + "WHERE w.VAM_Warehouse_ID=" + VAM_Warehouse_ID;		//	1

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetVAM_Warehouse_ID(VAM_Warehouse_ID);
                    //	Org
                    int VAF_Org_ID = (int)dr[0];
                    SetVAF_Org_ID(VAF_Org_ID);
                    //	Locator
                    int VAM_Locator_ID = (int)dr[1];
                    if (VAM_Locator_ID != 0)
                    {
                        //p_changeVO.setContext(getCtx(), windowNo, "VAM_Locator_ID", VAM_Locator_ID);
                    }
                    else
                    {
                        //p_changeVO.setContext(getCtx(), windowNo, "VAM_Locator_ID", (String)null);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

        }


        /// <summary>
        /// Create the missing next Confirmation
        /// </summary>
        /// Modified by Arpit- To Pass Optional Parameters while Creating confirmation
        public void CreateConfirmation(bool _Status = false)
        {
            bool checkDocStatus = _Status;
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            bool pick = dt.IsPickQAConfirm();
            bool ship = dt.IsShipConfirm();
            //	Nothing to do
            if (!pick && !ship)
            {
                log.Fine("No need");
                return;
            }

            //	Create Both .. after each other
            if (pick && ship)
            {
                bool havePick = false;
                bool haveShip = false;
                MVAMInvInOutConfirm[] confirmations = GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MVAMInvInOutConfirm confirm = confirmations[i];
                    if (MVAMInvInOutConfirm.CONFIRMTYPE_PickQAConfirm.Equals(confirm.GetConfirmType()))
                    {
                        if (!confirm.IsProcessed())		//	wait until done
                        {
                            log.Fine("Unprocessed: " + confirm);
                            return;
                        }
                        havePick = true;
                    }
                    else if (MVAMInvInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm.Equals(confirm.GetConfirmType()))
                        haveShip = true;
                }
                //	Create Pick
                if (!havePick)
                {
                    MVAMInvInOutConfirm.Create(this, MVAMInvInOutConfirm.CONFIRMTYPE_PickQAConfirm, false);
                    return;
                }
                //	Create Ship
                if (!haveShip)
                {
                    MVAMInvInOutConfirm.Create(this, MVAMInvInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, false);
                    return;
                }
                return;
            }
            //	Create just one
            if (pick)
                MVAMInvInOutConfirm.Create(this, MVAMInvInOutConfirm.CONFIRMTYPE_PickQAConfirm, true);
            else if (ship)
            {
                if (!checkDocStatus)
                    MVAMInvInOutConfirm.Create(this, MVAMInvInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, true);
                else
                    MVAMInvInOutConfirm.Create(this, MVAMInvInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, false);
            }
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true or false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Warehouse Org
            if (newRecord)
            {
                MWarehouse wh = MWarehouse.Get(GetCtx(), GetVAM_Warehouse_ID());
                if (wh.GetVAF_Org_ID() != GetVAF_Org_ID())
                {
                    log.SaveError("WarehouseOrgConflict", "");
                    return false;
                }
            }
            else if (Is_ValueChanged("VAM_Warehouse_ID"))
            {
                //JID_0858: when line exist then not able to update / change warehouse
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID = " + GetVAM_Inv_InOut_ID(), null, Get_Trx())) > 0)
                {
                    log.SaveError("VIS_WarehouseCantChange", "");
                    return false;
                }
            }

            //JID_1483- Accounts date should remain same as movement date
            SetDateAcct(GetMovementDate());

            //	Shipment - Needs Order
            if (IsSOTrx() && GetVAB_Order_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.Translate(GetCtx(), "VAB_Order_ID"));
                return false;
            }
            if (newRecord || Is_ValueChanged("VAB_BusinessPartner_ID"))
            {
                MVABBusinessPartner bp = MVABBusinessPartner.Get(GetCtx(), GetVAB_BusinessPartner_ID());
                if (!bp.IsActive())
                {
                    log.SaveError("NotActive", Msg.GetMsg(GetCtx(), "VAB_BusinessPartner_ID"));
                    return false;
                }
            }
            // Costing column updation
            if (newRecord)
            {
                // when we create new record then need to set as false
                //if (Get_ColumnIndex("IsCostCalculated") >= 0 && Get_ColumnIndex("ReversalDoc_ID") >= 0 && GetReversalDoc_ID() == 0)
                //{
                //    SetIsCostCalculated(false);
                //}
                if (Get_ColumnIndex("IsReversedCostCalculated") >= 0)
                {
                    SetIsReversedCostCalculated(false);
                }
            }

            ////Added by Bharat for Credit Limit on 24/08/2016
            //if (IsSOTrx())
            //{
            //    MBPartner bp = MBPartner.Get(GetCtx(), GetVAB_BusinessPartner_ID());
            //    if (bp.GetCreditStatusSettingOn() == "CH")
            //    {
            //        decimal creditLimit = bp.GetSO_CreditLimit();
            //        string creditVal = bp.GetCreditValidation();
            //        if (creditLimit != 0)
            //        {
            //            decimal creditAvlb = creditLimit - bp.GetSO_CreditUsed();
            //            if (creditAvlb <= 0)
            //            {
            //                //if (creditVal == "B" || creditVal == "D" || creditVal == "E" || creditVal == "F")
            //                //{
            //                //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CreditUsedShipment"));
            //                //    return false;
            //                //}
            //                //else if (creditVal == "H" || creditVal == "J" || creditVal == "K" || creditVal == "L")
            //                //{
            //                    log.SaveError("Warning", Msg.GetMsg(GetCtx(), "CreditOver"));
            //                //}
            //            }
            //        }
            //    }
            //    // JID_0161 // change here now will check credit settings on field only on Business Partner Header // Lokesh Chauhan 15 July 2019
            //    else if(bp.GetCreditStatusSettingOn() == X_VAB_BusinessPartner.CREDITSTATUSSETTINGON_CustomerLocation)
            //    {
            //        MBPartnerLocation bpl = new MBPartnerLocation(GetCtx(), GetVAB_BPart_Location_ID(), null);
            //        //if (bpl.GetCreditStatusSettingOn() == "CL")
            //        //{
            //            decimal creditLimit = bpl.GetSO_CreditLimit();
            //            string creditVal = bpl.GetCreditValidation();
            //            if (creditLimit != 0)
            //            {
            //                decimal creditAvlb = creditLimit - bpl.GetSO_CreditUsed();
            //                if (creditAvlb <= 0)
            //                {
            //                    //if (creditVal == "B" || creditVal == "D" || creditVal == "E" || creditVal == "F")
            //                    //{
            //                    //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CreditUsedShipment"));
            //                    //    return false;
            //                    //}
            //                    //else if (creditVal == "H" || creditVal == "J" || creditVal == "K" || creditVal == "L")
            //                    //{
            //                        log.SaveError("Warning", Msg.GetMsg(GetCtx(), "CreditOver"));
            //                    //}
            //                }
            //            }
            //        //}
            //    }
            //}
            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                //if (!success || newRecord)
                return success;

            if (!newRecord)
            {
                if (Is_ValueChanged("VAF_Org_ID"))
                {
                    String sql = "UPDATE VAM_Inv_InOutLine ol"
                        + " SET VAF_Org_ID ="
                            + "(SELECT VAF_Org_ID"
                            + " FROM VAM_Inv_InOut o WHERE ol.VAM_Inv_InOut_ID=o.VAM_Inv_InOut_ID) "
                        + "WHERE VAM_Inv_InOut_ID=" + GetVAB_Order_ID();
                    int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
                    log.Fine("Lines -> #" + no);
                }
            }

            if (((IsSOTrx() && !IsReturnTrx()) || GetMovementType() == "V-") && !IsReversal())
            {
                MVABOrder ord = new MVABOrder(GetCtx(), GetVAB_Order_ID(), Get_TrxName());
                Decimal grandTotal = MVABExchangeRate.ConvertBase(GetCtx(),
                        ord.GetGrandTotal(), GetVAB_Currency_ID(), GetDateOrdered(),
                        ord.GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());

                MVABBusinessPartner bp = new MVABBusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), Get_Trx());
                string retMsg = "";
                bool crdAll = bp.IsCreditAllowed(GetVAB_BPart_Location_ID(), grandTotal, out retMsg);
                if (!crdAll)
                    log.SaveWarning("Warning", retMsg);
                else if (bp.IsCreditWatch(GetVAB_BPart_Location_ID()))
                {
                    log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "VIS_BPCreditWatch"));
                }
            }

            return success;
        }

        /****
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public virtual bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        // Added by Amit 1-8-2015 VAMRP
        public virtual bool ProcessIt(String processAction, bool isset)
        {
            if (isset)
            {
                set = true;
            }
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }
        //end

        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public virtual bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public virtual bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            SetIsReturnTrx(dt.IsReturnTrx());
            SetIsSOTrx(dt.IsSOTrx());

            //	Std Period open?
            if (!MVABYearPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetVAF_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetVAF_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            // Added by Vivek on 08/11/2017 assigned by Mukesh sir
            // check if Linked PO is not in completed or closed stage then not complete this record
            if (GetVAB_Order_ID() != 0 && !IsSOTrx() && !IsReturnTrx())
            {
                MVABOrder order = new MVABOrder(GetCtx(), GetVAB_Order_ID(), Get_Trx());
                if (order.GetDocStatus() != "CO" && order.GetDocStatus() != "CL")
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "LinkedPOStatus");
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            //	Credit Check
            if (((IsSOTrx() && !IsReturnTrx()) || GetMovementType() == "V-") && !IsReversal())
            {
                bool checkCreditStatus = true;
                if (Env.IsModuleInstalled("VAPOS_"))
                {
                    // Change Here For On Credit
                    Decimal? onCreditAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT VAPOS_CreditAmt FROM VAB_Order WHERE VAB_Order_ID = " + GetVAB_Order_ID()));
                    if (onCreditAmt <= 0)
                        checkCreditStatus = false;
                }
                if (checkCreditStatus)
                {
                    MVABBusinessPartner bp = new MVABBusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), null);
                    //if (MBPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
                    //{
                    //    _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
                    //        + bp.GetTotalOpenBalance()
                    //        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    //    return DocActionVariables.STATUS_INVALID;
                    //}
                    //if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus()))
                    //{
                    //    _processMsg = "@BPartnerCreditHold@ - @TotalOpenBalance@="
                    //        + bp.GetTotalOpenBalance()
                    //        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    //    return DocActionVariables.STATUS_INVALID;
                    //}

                    Decimal notInvoicedAmt = MVABBusinessPartner.GetNotInvoicedAmt(GetVAB_BusinessPartner_ID());
                    if (MVABBusinessPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus(notInvoicedAmt)))
                    {
                        _processMsg = "@BPartnerOverSCreditHold@ - @TotalOpenBalance@="
                            + bp.GetTotalOpenBalance() + ", @NotInvoicedAmt@=" + notInvoicedAmt
                            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                        return DocActionVariables.STATUS_INVALID;
                    }

                    // check for Credit limit and Credit validation on Customer Master or Location
                    string retMsg = "";
                    bool crdAll = bp.IsCreditAllowed(GetVAB_BPart_Location_ID(), 0, out retMsg);
                    if (!crdAll)
                    {
                        if (bp.ValidateCreditValidation("B,D,E,F", GetVAB_BPart_Location_ID()))
                        {
                            _processMsg = retMsg;
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }


            if (Env.IsModuleInstalled("VA009_"))
            {
                if (GetVAB_Order_ID() != 0)
                {
                    int _countschedule = Util.GetValueOfInt(DB.ExecuteScalar("Select Count(*) From VA009_OrderPaySchedule Where VAB_Order_ID=" + GetVAB_Order_ID()));
                    if (_countschedule > 0)
                    {
                        if (Util.GetValueOfInt(DB.ExecuteScalar("Select Count(*) From VA009_OrderPaySchedule Where VAB_Order_ID=" + GetVAB_Order_ID() + " AND VA009_Ispaid='Y'")) != _countschedule)
                        {
                            _processMsg = "Please Do Advance Payment against order";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }

            //	Lines
            MVAMInvInOutLine[] lines = GetLines(true);
            if (lines == null || lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            Decimal Volume = Env.ZERO;
            Decimal Weight = Env.ZERO;

            //	Mandatory Attributes
            for (int i = 0; i < lines.Length; i++)
            {
                MVAMInvInOutLine line = lines[i];
                MVAMProduct product = line.GetProduct();
                if (product != null)
                {
                    Volume = Decimal.Add(Volume, Decimal.Multiply((Decimal)product.GetVolume(),
                        line.GetMovementQty()));
                    Weight = Decimal.Add(Weight, Decimal.Multiply(product.GetWeight(),
                        line.GetMovementQty()));
                }
                if (line.GetVAM_PFeature_SetInstance_ID() != 0)
                    continue;
                if (product != null)
                {
                    int VAM_PFeature_Set_ID = product.GetVAM_PFeature_Set_ID();
                    if (VAM_PFeature_Set_ID != 0)
                    {
                        MVAMPFeatureSet mas = MVAMPFeatureSet.Get(GetCtx(), VAM_PFeature_Set_ID);
                        if (mas != null
                            && ((IsSOTrx() && mas.IsMandatory())
                                || (!IsSOTrx() && mas.IsMandatoryAlways())))
                        {
                            // JID_0259: Here we need to show the name of the product for which attribute is mandatory.
                            _processMsg = product.GetName() + ": " + "@VAM_PFeature_Set_ID@ @IsMandatory@";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }
            SetVolume(Volume);
            SetWeight(Weight);
            if (!IsReversal())	//	don't change reversal
            {
                /* nnayak - Bug 1750251 : check material policy and update storage
                   at the line level in completeIt()*/
                // checkMaterialPolicy();	//	set MASI
                CreateConfirmation();
            }

            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success </returns>
        public virtual bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success </returns>
        public virtual bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public virtual String CompleteIt()
        {
            // chck pallet Functionality applicable or not
            isContainrApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            //************* Change By Lokesh Chauhan ***************
            // If qty on locator is insufficient then return
            // Will not complete.
            StringBuilder sql = new StringBuilder();
            //MVAMProduct pro = null;
            //Dictionary<int, MVAMInvInOutLineMP[]> lineAttributes = null;
            //if (IsSOTrx())
            //{

            String MovementTyp = GetMovementType();

            int VAPOS_POSTerminal_ID = 0;
            if (Env.IsModuleInstalled("VAPOS_"))
            {
                VAPOS_POSTerminal_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAPOS_POSTERMINAL_ID FROM VAB_Order WHERE VAB_ORDER_ID=" + GetVAB_Order_ID()));
            }
            if (!(VAPOS_POSTerminal_ID > 0))
            {
                if (MovementTyp.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                {
                    #region Prevent from completing, If qty on Product trnsaction on specified movement date not available as per qty entered at line and Disallow negative is true at Warehouse
                    sql.Clear();
                    sql.Append("SELECT ISDISALLOWNEGATIVEINV FROM VAM_Warehouse WHERE VAM_Warehouse_ID = " + Util.GetValueOfInt(GetVAM_Warehouse_ID()));
                    string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                    if (disallow.ToUpper() == "Y")
                    {
                        // is used to handle Non Stocked Item which are not in Storage
                        string whereClause = "VAM_Inv_InOut_ID = " + GetVAM_Inv_InOut_ID() + @" AND VAB_Charge_ID IS NULL AND VAM_Product_ID NOT IN 
                            (SELECT VAM_Inv_InOutLine.VAM_Product_ID FROM VAM_Inv_InOutLine INNER JOIN VAM_Product ON VAM_Inv_InOutLine.VAM_Product_ID = VAM_Product.VAM_Product_ID 
                            WHERE VAM_Product.IsStocked = 'N' AND VAM_Inv_InOutLine.VAM_Inv_InOut_ID  = " + GetVAM_Inv_InOut_ID() + " ) ";
                        int[] ioLine = MVAMInvInOutLine.GetAllIDs("VAM_Inv_InOutLine", whereClause, Get_TrxName());
                        int VAM_Locator_id = 0;
                        int VAM_Product_id = 0;
                        StringBuilder products = new StringBuilder();   // Added by sukhwinder for storing product aand locators IDs. on 19Dec, 2017
                        StringBuilder locators = new StringBuilder();
                        bool check = false;
                        for (int i = 0; i < ioLine.Length; i++)
                        {
                            MVAMInvInOutLine iol = new MVAMInvInOutLine(Env.GetCtx(), ioLine[i], Get_TrxName());
                            VAM_Locator_id = Util.GetValueOfInt(iol.GetVAM_Locator_ID());
                            VAM_Product_id = Util.GetValueOfInt(iol.GetVAM_Product_ID());

                            sql.Clear();
                            sql.Append("SELECT VAM_PFeature_Set_ID FROM VAM_Product WHERE VAM_Product_ID = " + VAM_Product_id);
                            int VAM_ProductFeature_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                            if (VAM_ProductFeature_ID == 0)
                            {
                                sql.Clear();
                                if (!isContainrApplicable)
                                {
                                    sql.Append("SELECT SUM(QtyOnHand) FROM VAM_Storage WHERE VAM_Locator_ID = " + VAM_Locator_id + " AND VAM_Product_ID = " + VAM_Product_id);
                                }
                                else
                                {
                                    sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, t.VAM_Locator_ID, NVL(t.VAM_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                                        INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                            " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + iol.GetVAM_Locator_ID() +
                                            " AND t.VAM_Product_ID = " + iol.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + iol.GetVAM_PFeature_SetInstance_ID() +
                                            " AND NVL(t.VAM_ProductContainer_ID, 0) = " + iol.GetVAM_ProductContainer_ID());
                                }
                                int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                                int qtyToMove = Util.GetValueOfInt(iol.GetMovementQty());
                                if (qty < qtyToMove)
                                {
                                    check = true;
                                    products.Append(VAM_Product_id + ", ");
                                    locators.Append(VAM_Locator_id + ", ");
                                    continue;
                                }
                            }
                            else
                            {
                                sql.Clear();
                                if (!isContainrApplicable)
                                {
                                    sql.Append("SELECT SUM(QtyOnHand) FROM VAM_Storage WHERE VAM_Locator_ID = " + VAM_Locator_id + " AND VAM_Product_ID = " + VAM_Product_id + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + iol.GetVAM_PFeature_SetInstance_ID());
                                }
                                else
                                {
                                    sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, t.VAM_Locator_ID, NVL(t.VAM_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                                        INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                            " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + iol.GetVAM_Locator_ID() +
                                            " AND t.VAM_Product_ID = " + iol.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + iol.GetVAM_PFeature_SetInstance_ID() +
                                            " AND NVL(t.VAM_ProductContainer_ID, 0) = " + iol.GetVAM_ProductContainer_ID());
                                }
                                int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                                int qtyToMove = Util.GetValueOfInt(iol.GetMovementQty());
                                if (qty < qtyToMove)
                                {
                                    check = true;
                                    products.Append(VAM_Product_id + ",");
                                    locators.Append(VAM_Locator_id + ",");
                                    continue;
                                }
                            }
                        }
                        if (check)
                        {
                            sql.Clear();
                            sql.Append(DBFunctionCollection.ConcatinateListOfLocators(locators.ToString()));
                            string loc = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                            sql.Clear();
                            sql.Append(DBFunctionCollection.ConcatinateListOfProducts(products.ToString()));
                            string prod = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                            _processMsg = Msg.GetMsg(Env.GetCtx(), "VIS_InsufficientQuantityFor") + prod + Msg.GetMsg(Env.GetCtx(), "VIS_OnLocators") + loc;

                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    #endregion
                }
            }

            // Change By amit 1-8-2015
            //if (Env.HasModulePrefix("VAMRP_", out mInfo))
            //{
            //    IDataReader idr;
            //    //Dictionary<int, MVAMInvInOutLineMP[]> lineAttributes = null;
            //    //MVAMProduct pro = null;
            //    lineAttributes = new Dictionary<int, MVAMInvInOutLineMP[]>();
            //    sql = "select VAM_Inv_InOutLine_id,qtyentered,line,VAM_Product_id from VAM_Inv_InOutLine where VAM_Inv_InOut_id=" + GetVAM_Inv_InOut_ID();
            //    idr = DB.ExecuteReader(sql);
            //    while (idr.Read())
            //    {
            //        pro = new MVAMProduct(GetCtx(), Util.GetValueOfInt(idr["VAM_Product_id"]), null);
            //        if (pro.GetVAM_PFeature_SetInstance_ID() != 0)
            //        {
            //            MVAMInvInOutLineMP[] attrib = MVAMInvInOutLineMP.Get(GetCtx(), Util.GetValueOfInt(idr[0]), Get_Trx());
            //            if (Util.GetValueOfInt(idr[1]) != attrib.Length)
            //            {
            //                _processMsg = "Number of lines on Attribute Tab not equal to quantity" +
            //                " defined on line No: " + Util.GetValueOfInt(idr[2]);
            //                return DocActionVariables.STATUS_INPROGRESS;
            //            }
            //            lineAttributes.Add(Util.GetValueOfInt(idr[0]), attrib);
            //        }
            //    }
            //    idr.Close();
            //}
            // end 
            // }

            //if (Env.HasModulePrefix("VAMRP_", out mInfo))
            //{
            //    string msg = "";

            //    //----------------------Change by Lokesh Chauhan--------------------------
            //    X_VAM_Inv_InOut inOut = new X_VAM_Inv_InOut(GetCtx(), GetVAM_Inv_InOut_ID(), null);
            //    string sqlL = "select name from VAB_BPart_Location where VAB_BPart_Location_id=" + inOut.GetVAB_BPart_Location_ID();
            //    string name = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sqlL));
            //    Boolean result = name.StartsWith("POS -");
            //    if (result)
            //    {
            //        int[] allIds = X_VAM_Inv_InOutLine.GetAllIDs("VAM_Inv_InOutLine", "VAM_Inv_InOut_ID=" + inOut.GetVAM_Inv_InOut_ID(), null);
            //        if (allIds.Length > 0)
            //        {
            //            for (int i = 0; i < allIds.Length; i++)
            //            {
            //                X_VAM_Inv_InOutLine IOLine = new X_VAM_Inv_InOutLine(GetCtx(), allIds[i], null);
            //                MProdReceived ProdReceived = new MProdReceived(GetCtx(), 0, null);
            //                ProdReceived.SetVAF_Client_ID(inOut.GetVAF_Client_ID());
            //                ProdReceived.SetVAF_Org_ID(inOut.GetVAF_Org_ID());
            //                ProdReceived.SetVAB_BusinessPartner_ID(inOut.GetVAB_BusinessPartner_ID());
            //                ProdReceived.SetVAB_BPart_Location_ID(inOut.GetVAB_BPart_Location_ID());
            //                ProdReceived.SetDS_AREA_ID(inOut.GetDS_AREA_ID());
            //                ProdReceived.SetDS_SUBLOCATION_ID(inOut.GetDS_SUBLOCATION_ID());
            //                ProdReceived.SetVAM_Inv_InOut_ID(GetVAM_Inv_InOut_ID());
            //                ProdReceived.SetVAM_Product_ID(IOLine.GetVAM_Product_ID());
            //                sqlL = "select VAM_ProductCategory_id from VAM_Product where VAM_Product_id =" + IOLine.GetVAM_Product_ID();
            //                int VAM_ProductCategory_ID = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlL));
            //                ProdReceived.SetVAM_ProductCategory_ID(VAM_ProductCategory_ID);
            //                ProdReceived.SetDate1(inOut.GetMovementDate());
            //                ProdReceived.SetQuantity(IOLine.GetQtyEntered());
            //                ProdReceived.SetDocStatus1("C");
            //                ProdReceived.SetProcessed(false);
            //                ProdReceived.SetProcessedIn(true);
            //                if (!ProdReceived.Save())
            //                {
            //                    log.SaveError("ProductReceivedNotSaved", "");
            //                    return msg;
            //                }
            //            }
            //        }
            //    }
            //    //----------------------Change by Lokesh Chauhan--------------------------
            //}

            //*****************************************************

            if (isContainrApplicable)
            {
                #region Check Container existence  in specified Warehouse and Locator
                // during completion - system will verify 
                // if container avialble on line is belongs to same warehouse and locator
                // if not then not to complete this record
                sql.Clear();
                sql.Append(DBFunctionCollection.MVAMInvInOutContainerNotMatched(GetVAM_Inv_InOut_ID()));
                string containerNotMatched = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (!String.IsNullOrEmpty(containerNotMatched))
                {
                    SetProcessMsg(Msg.GetMsg(GetCtx(), "VIS_ContainerNotFound") + containerNotMatched);
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion

                #region Movement Date is less than Last MovementDate on Product Container, then not to complete
                // If User try to complete the Transactions if Movement Date is lesser than Last MovementDate on Product Container
                // then we need to stop that transaction to Complete.
                sql.Clear();
                sql.Append(DBFunctionCollection.MVAMInvInOutContainerNotAvailable(GetVAM_Inv_InOut_ID()));
                string misMatch = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (!String.IsNullOrEmpty(misMatch))
                {
                    SetProcessMsg(misMatch + Msg.GetMsg(GetCtx(), "VIS_ContainerNotAvailable"));
                    return DocActionVariables.STATUS_INVALID;
                }
                #endregion
            }

            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            // JID_1290: Set the document number from completede document sequence after completed (if needed)
            SetCompletedDocumentNo();

            // To check weather future date records are available in Transaction window
            // this check implement after "SetCompletedDocumentNo" function, because this function overwrit movement date
            _processMsg = MVAMInvTrx.CheckFutureDateRecord(GetMovementDate(), Get_TableName(), GetVAM_Inv_InOut_ID(), Get_Trx());
            if (!string.IsNullOrEmpty(_processMsg))
            {
                return DocActionVariables.STATUS_INVALID;
            }

            if (Env.IsModuleInstalled("VA024_"))
            {
                sql.Clear();
                sql.Append(@"SELECT VAF_TABLEVIEW_ID  FROM VAF_TABLEVIEW WHERE tablename LIKE 'VA024_T_ObsoleteInventory' AND IsActive = 'Y'");
                tableId = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            }

            if (Env.IsModuleInstalled("VA026_"))
            {
                sql.Clear();
                sql.Append(@"SELECT VAF_TABLEVIEW_ID  FROM VAF_TABLEVIEW WHERE tablename LIKE 'VA026_LCDetail' AND IsActive = 'Y'");
                tableId1 = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            }


            // for checking - costing calculate on completion or not
            // IsCostImmediate = true - calculate cost on completion
            MVAFClient client = MVAFClient.Get(GetCtx(), GetVAF_Client_ID());

            //	Outstanding (not processed) Incoming Confirmations ?
            MVAMInvInOutConfirm[] confirmations = GetConfirmations(true);
            Int32 confirmationCount = 0;
            for (int i = 0; i < confirmations.Length; i++)
            {
                MVAMInvInOutConfirm confirm = confirmations[i];
                //New Code added Here 
                confirmationCount += 1;
                //Arpit to check docStatus on of confirm doc 
                if (!confirm.IsProcessed())
                {
                    if (MVAMInvInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                        continue;
                    if (confirm.GetDocStatus() == DOCSTATUS_Voided)
                    {
                        if (confirmationCount == confirmations.Length)
                        {
                            CreateConfirmation(true); //Paasing optional Parameter to false the value of existing
                            //  _processMsg = Msg.GetMsg(GetCtx(),"NoConfirmationFoundForMR") + GetDocumentNo();
                            //Message-No Confirmation found for Material Receipt No: Key -->NoConfirmationFoundForMR
                            //  return _processMsg;
                            if (MVAMInvInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                                continue;
                            //
                            _processMsg = "Open @VAM_Inv_InOutConfirm_ID@: " +
                                confirm.GetConfirmTypeName() + " - " + confirm.GetDocumentNo();
                            FreezeDoc();//Arpit
                            return DocActionVariables.STATUS_INPROGRESS;
                        }
                        else
                            continue;
                    }
                    if (confirm.GetDocStatus() == DOCSTATUS_Drafted)
                    {
                        CreateConfirmation(false); //Paasing optional Parameter to false the value of existing
                        //Pasing Optional paramerts to False means to create 
                        //  _processMsg = Msg.GetMsg(GetCtx(),"NoConfirmationFoundForMR") + GetDocumentNo();
                        //Message-No Confirmation found for Material Receipt No: Key -->NoConfirmationFoundForMR
                        //  return _processMsg;
                        if (MVAMInvInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                            continue;
                        //
                        _processMsg = "Open @VAM_Inv_InOutConfirm_ID@: " +
                            confirm.GetConfirmTypeName() + " - " + confirm.GetDocumentNo();
                        FreezeDoc();//Arpit                  
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                }
                //END of New Code
                // Old Code Commented Here 
                //if (!confirm.IsProcessed())
                //{
                //    if (MVAMInvInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                //        continue;
                //    //
                //    _processMsg = "Open @VAM_Inv_InOutConfirm_ID@: " +
                //        confirm.GetConfirmTypeName() + " - " + confirm.GetDocumentNo();
                //    FreezeDoc();//Arpit
                //    return DocActionVariables.STATUS_INPROGRESS;
                //}
            }
            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());
            StringBuilder Info = new StringBuilder();

            //	For all lines
            MVAMInvInOutLine[] lines = GetLines(false);
            if (!(VAPOS_POSTerminal_ID > 0))
            {
                for (int Index = 0; Index < lines.Length; Index++)
                {
                    MVAMInvInOutLine Line = lines[Index];
                    // Change done by mohit as discussed by ravikant and mukesh sir - do not check locator if there is charge on shipment line- 01/06/2017 (PMS TaskID=3893)
                    if (Line.GetVAM_Locator_ID() == 0 && Line.GetVAM_Product_ID() != 0)
                    {
                        _processMsg = Msg.GetMsg(Env.GetCtx(), "LocatorNotFound");
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }
            ////Pratap- For Asset Cost
            //List<int> _assetList = new List<int>();
            ////
            #region [Process All Lines]
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                MVAMInvInOutLine sLine = lines[lineIndex];
                MVAMProduct product = sLine.GetProduct();
                ////Pratap- For Asset Cost
                //_assetList.Clear();
                ////
                // Added by Vivek on 07/10/2017 advised by Pradeep and Ashish
                // Stopped checking of assest binding at shipment line while shipment is of drop ship type
                if (!IsDropShip())
                {
                    #region Asset Work
                    if (product != null)
                    {
                        MVAMProductCategory pc = new MVAMProductCategory(Env.GetCtx(), product.GetVAM_ProductCategory_ID(), Get_TrxName());
                        if (VAPOS_POSTerminal_ID > 0)
                        {
                            if (IsSOTrx() && pc.GetVAA_AssetGroup_ID() > 0 && sLine.GetA_Asset_ID() == 0)
                            {
                                _processMsg = "AssetNotSetONShipmentLine: LineNo" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                        }
                        else
                        {
                            if (IsSOTrx() && !IsReturnTrx() && pc.GetVAA_AssetGroup_ID() > 0 && sLine.GetA_Asset_ID() == 0)
                            {
                                _processMsg = "AssetNotSetONShipmentLine: LineNo" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                        }
                    }
                    if (!(VAPOS_POSTerminal_ID > 0))
                    {
                        if (IsSOTrx() && sLine.GetA_Asset_ID() != 0)
                        {
                            MVAAsset ast = new MVAAsset(Env.GetCtx(), sLine.GetA_Asset_ID(), Get_TrxName());
                            ast.SetIsDisposed(true);
                            ast.SetAssetDisposalDate(GetDateAcct());
                            if (!ast.Save(Get_TrxName()))
                            {
                                _processMsg = "AssetNotUpdated" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                        }
                    }
                    #endregion
                }

                #region done by Amit on behalf of surya 30-9-2015 vawms
                // on Warehouse, Set Qty Allocated only for Item type Product
                if (sLine.GetVAB_OrderLine_ID() != 0 && product != null && product.GetProductType() == MVAMProduct.PRODUCTTYPE_Item && IsSOTrx() && !IsReturnTrx())
                {
                    if (Env.IsModuleInstalled("VAWMS_"))
                    {
                        MVAMStorage allocatedStorage = MVAMStorage.GetCreate(GetCtx(), sLine.GetVAM_Locator_ID(), sLine.GetVAM_Product_ID(),
                            sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx());
                        allocatedStorage.SetQtyAllocated(Decimal.Subtract(allocatedStorage.GetQtyAllocated(), sLine.GetMovementQty()));
                        if (!allocatedStorage.Save())
                        {
                            log.Warning("Warehouse , quantity allocated is not saved");
                            _processMsg = "Warehouse , quantity allocated is not saved";
                            return DocActionVariables.STATUS_INPROGRESS;
                        }
                    }
                }
                #endregion

                //	Qty & Type
                String MovementType = GetMovementType();
                Decimal Qty = sLine.GetMovementQty();
                //if (MovementType.charAt(1) == '-')	//	C- Customer Shipment - V- Vendor Return
                if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                {
                    Qty = Decimal.Negate(Qty);
                }
                Decimal QtySO = Env.ZERO;
                Decimal QtyPO = Env.ZERO;
                decimal qtytoset = Env.ZERO;
                //	Update Order Line
                MVABOrderLine oLine = null;
                if (sLine.GetVAB_OrderLine_ID() != 0)
                {
                    oLine = new MVABOrderLine(GetCtx(), sLine.GetVAB_OrderLine_ID(), Get_TrxName());
                    log.Fine("OrderLine - Reserved=" + oLine.GetQtyReserved()
                    + ", Delivered=" + oLine.GetQtyDelivered());
                    // nnayak - Qty reserved and Qty updated not affected by returns
                    if (!IsReturnTrx())
                    {
                        if (IsSOTrx())
                            QtySO = Decimal.Negate(sLine.GetMovementQty());
                        else
                            QtyPO = Decimal.Negate(sLine.GetMovementQty());
                    }
                }

                log.Info("Line=" + sLine.GetLine() + " - Qty=" + sLine.GetMovementQty() + "Return " + IsReturnTrx());

                //	Stock Movement - Counterpart MOrder.reserveStock
                if (product != null
                    && product.IsStocked())
                {
                    if (!IsReversal())
                    {
                        // material policy to be maintain for stocked Item
                        CheckMaterialPolicy(sLine);
                    }

                    log.Fine("Material Transaction");
                    MVAMInvTrx mtrx = null;

                    // Added By Amit 3-8-2015 VAMRP
                    #region VAMRP
                    //bool attribCheck = false;
                    //if (Env.HasModulePrefix("VAMRP_", out mInfo))
                    //{                 
                    //if (pro != null)
                    //{
                    //    attribCheck = pro.GetVAM_PFeature_SetInstance_ID() != 0;
                    //}
                    ////attribute stirage update logic
                    //if (IsSOTrx() && attribCheck)
                    //{
                    //    MVAMInvInOutLineMP[] mas = lineAttributes[sLine.GetVAM_Inv_InOutLine_ID()];
                    //    for (int j = 0; j < mas.Length; j++)
                    //    {
                    //        MVAMInvInOutLineMP ma = mas[j];
                    //        Decimal QtyMA = ma.GetMovementQty();
                    //        //if (MovementType.charAt(1) == '-')	//	C- Customer Shipment - V- Vendor Return
                    //        if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                    //        {
                    //            QtyMA = Decimal.Negate(QtyMA);
                    //        }
                    //        Decimal QtySOMA = Env.ZERO;
                    //        Decimal QtyPOMA = Env.ZERO;

                    //        // nnayak - Don't update qty reserved or qty ordered for Returns
                    //        if (sLine.GetVAB_OrderLine_ID() != 0 && !IsReturnTrx())
                    //        {
                    //            if (IsSOTrx())
                    //                QtySOMA = Decimal.Negate(ma.GetMovementQty());
                    //            else
                    //                QtyPOMA = Decimal.Negate(ma.GetMovementQty());
                    //        }

                    //        log.Fine("QtyMA : " + QtyMA + " QtySOMA " + QtySOMA + " QtyPOMA " + QtyPOMA);

                    //        //	Update Storage - see also VMatch.createMatchRecord
                    //        /**********/
                    //        string sqlatr = "SELECT SUM(movementqty) FROM VAM_Inv_InOutLineMP WHERE VAM_Inv_InOutLine_id=" + sLine.GetVAM_Inv_InOutLine_ID();
                    //        int totalmovqty = Util.GetValueOfInt(DB.ExecuteScalar(sqlatr, null, null));

                    //        string sqty = "SELECT qtyentered FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOutLine_id=" + sLine.GetVAM_Inv_InOutLine_ID();
                    //        int quantity = Util.GetValueOfInt(DB.ExecuteScalar(sqty, null, null));

                    //        //MVAMInvInOutLine minline = new MVAMInvInOutLine(GetCtx(), VAM_Inv_InOutLine_id, Get_TrxName());
                    //        if (totalmovqty == quantity)
                    //        {

                    //            if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                    //              sLine.GetVAM_Locator_ID(),
                    //              sLine.GetVAM_Product_ID(),
                    //              ma.GetVAM_PFeature_SetInstance_ID(), ma.GetVAM_PFeature_SetInstance_ID(),
                    //              QtyMA, QtySOMA, QtyPOMA, Get_Trx(), sLine.GetVAM_Inv_InOutLine_ID()))
                    //            {
                    //                _processMsg = "Cannot correct Inventory (MA)";
                    //                return DocActionVariables.STATUS_INVALID;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            return "";
                    //        }
                    //        if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                    //            sLine.GetVAM_Locator_ID(),
                    //            sLine.GetVAM_Product_ID(),
                    //            ma.GetVAM_PFeature_SetInstance_ID(), ma.GetVAM_PFeature_SetInstance_ID(),
                    //            QtyMA, QtySOMA, QtyPOMA, Get_TrxName()))
                    //        {
                    //            _processMsg = "Cannot correct Inventory (MA)";
                    //            return DocActionVariables.STATUS_INVALID;
                    //        }
                    //        //	Create Transaction
                    //        mtrx = new MVAMInvTrx(GetCtx(), sLine.GetVAF_Org_ID(),
                    //            MovementType, sLine.GetVAM_Locator_ID(),
                    //            sLine.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(),
                    //            QtyMA, GetMovementDate(), Get_TrxName());
                    //        mtrx.SetVAM_Inv_InOutLine_ID(sLine.GetVAM_Inv_InOutLine_ID());
                    //        if (!mtrx.Save())
                    //        {
                    //            _processMsg = "Could not create Material Transaction (MA)";
                    //            return DocActionVariables.STATUS_INVALID;
                    //        }
                    //    }
                    //}                   
                    // }
                    #endregion


                    ////	Reservation ASI - assume none
                    int reservationAttributeSetInstance_ID = 0; // sLine.getVAM_PFeature_SetInstance_ID();
                    if (oLine != null)
                        reservationAttributeSetInstance_ID = oLine.GetVAM_PFeature_SetInstance_ID();
                    //
                    if (sLine.GetVAM_PFeature_SetInstance_ID() == 0 || sLine.GetVAM_PFeature_SetInstance_ID() != 0)
                    {
                        MVAMInvInOutLineMP[] mas = MVAMInvInOutLineMP.Get(GetCtx(),
                            sLine.GetVAM_Inv_InOutLine_ID(), Get_TrxName());
                        for (int j = 0; j < mas.Length; j++)
                        {
                            Decimal? containerCurrentQty = 0;
                            MVAMInvInOutLineMP ma = mas[j];
                            Decimal QtyMA = ma.GetMovementQty();
                            //if (MovementType.charAt(1) == '-')	//	C- Customer Shipment - V- Vendor Return
                            if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                            {
                                QtyMA = Decimal.Negate(QtyMA);
                            }
                            Decimal QtySOMA = Env.ZERO;
                            Decimal QtyPOMA = Env.ZERO;

                            // nnayak - Don't update qty reserved or qty ordered for Returns
                            if (sLine.GetVAB_OrderLine_ID() != 0 && !IsReturnTrx())
                            {
                                if (IsSOTrx())
                                    QtySOMA = Decimal.Negate(ma.GetMovementQty());
                                else
                                    QtyPOMA = Decimal.Negate(ma.GetMovementQty());
                            }

                            log.Fine("QtyMA : " + QtyMA + " QtySOMA " + QtySOMA + " QtyPOMA " + QtyPOMA);
                            //	Update Storage - see also VMatch.createMatchRecord



                            if (sLine.GetVAB_OrderLine_ID() != 0 && !IsReturnTrx())
                            {
                                #region Update Storage when record created with refenece of Orderline and not a RETURN trx
                                MVABOrderLine ordLine = new MVABOrderLine(GetCtx(), sLine.GetVAB_OrderLine_ID(), Get_TrxName());
                                MVABOrder ord = new MVABOrder(GetCtx(), ordLine.GetVAB_Order_ID(), Get_TrxName());
                                if (!IsReversal())
                                {
                                    qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                    //if (qtytoset >= sLine.GetMovementQty())
                                    if (qtytoset >= ma.GetMovementQty())
                                    {
                                        //qtytoset = sLine.GetMovementQty();
                                        qtytoset = ma.GetMovementQty();
                                    }
                                    qtytoset = Decimal.Negate(qtytoset);
                                }
                                else
                                {
                                    #region during Reversal
                                    if (IsSOTrx())
                                    {
                                        qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                    }
                                    else
                                    {
                                        //qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), Decimal.Add(ordLine.GetQtyDelivered(), Decimal.Negate(sLine.GetMovementQty())));
                                        qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), Decimal.Add(ordLine.GetQtyDelivered(), Decimal.Negate(ma.GetMovementQty())));
                                    }

                                    if (qtytoset < 0)
                                    {
                                        //qtytoset = Decimal.Add(qtytoset, Decimal.Negate(sLine.GetMovementQty()));
                                        qtytoset = Decimal.Add(qtytoset, Decimal.Negate(ma.GetMovementQty()));
                                    }
                                    else
                                    {
                                        //qtytoset = Decimal.Negate(sLine.GetMovementQty());
                                        qtytoset = Decimal.Negate(ma.GetMovementQty());
                                    }
                                    // when Qty Ordered == 0 on sales order then qtytoset become Movement qty from Shipment
                                    // this qty is used to update qtyreserved on storage
                                    if (ordLine.GetQtyOrdered() == 0)
                                    {
                                        qtytoset = Decimal.Negate(ma.GetMovementQty());
                                    }
                                    #endregion
                                }
                                if (IsSOTrx())
                                {
                                    QtySO = qtytoset;
                                }
                                else
                                {
                                    QtyPO = qtytoset;
                                }

                                if (!(VAPOS_POSTerminal_ID > 0))
                                {

                                    //Added by Vivek assigned by Pradeep on 27/09/2017
                                    // if MR/Shipment line is not of drop ship type then second warehouse wil be order's header else current warehouse
                                    if (!sLine.IsDropShip())
                                    {
                                        if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                        sLine.GetVAM_Locator_ID(), ord.GetVAM_Warehouse_ID(),
                                        sLine.GetVAM_Product_ID(),
                                        sLine.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                             //Qty, QtySO, QtyPO, Get_TrxName()))
                                             QtyMA, QtySO, QtyPO, Get_TrxName()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                                _processMsg = "Cannot correct Inventory, " + pp.GetName();
                                            else
                                                _processMsg = "Cannot correct Inventory";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }
                                    else
                                    {
                                        if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                        sLine.GetVAM_Locator_ID(), GetVAM_Warehouse_ID(),
                                        sLine.GetVAM_Product_ID(),
                                        sLine.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                             //Qty, QtySO, QtyPO, Get_TrxName()))
                                             QtyMA, QtySO, QtyPO, Get_TrxName()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                                _processMsg = "Cannot correct Inventory, " + pp.GetName();
                                            else
                                                _processMsg = "Cannot correct Inventory";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                    }

                                }
                                else if (VAPOS_POSTerminal_ID > 0)
                                {
                                    if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                   sLine.GetVAM_Locator_ID(),
                                   sLine.GetVAM_Product_ID(),
                                   ma.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                   QtyMA, QtySOMA, QtyPOMA, Get_TrxName()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                            _processMsg = "Cannot correct Inventory (MA). " + pp.GetName();
                                        else
                                            _processMsg = "Cannot correct Inventory (MA)";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                    sLine.GetVAM_Locator_ID(),
                                    sLine.GetVAM_Product_ID(),
                                    ma.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                    QtyMA, QtySOMA, QtyPOMA, Get_TrxName()))
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        _processMsg = "Cannot correct Inventory (MA). " + pp.GetName();
                                    else
                                        _processMsg = "Cannot correct Inventory (MA)";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }

                            #region Update Transaction / Future Date entry
                            sql.Clear();
                            sql.Append(@"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                   " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() +
                               " AND t.VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + sLine.GetVAM_PFeature_SetInstance_ID());
                            trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                            // get container Current qty from transaction
                            if (isContainrApplicable && sLine.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                            {
                                containerCurrentQty = GetContainerQtyFroMVAMInvTrx(sLine, GetMovementDate());
                            }

                            //	Create Transaction
                            mtrx = new MVAMInvTrx(GetCtx(), sLine.GetVAF_Org_ID(),
                                MovementType, sLine.GetVAM_Locator_ID(),
                                sLine.GetVAM_Product_ID(), ma.GetVAM_PFeature_SetInstance_ID(),
                                QtyMA, GetMovementDate(), Get_TrxName());
                            mtrx.SetVAM_Inv_InOutLine_ID(sLine.GetVAM_Inv_InOutLine_ID());
                            mtrx.SetCurrentQty(trxQty + QtyMA);
                            // set Material Policy Date
                            mtrx.SetMMPolicyDate(ma.GetMMPolicyDate());
                            if (isContainrApplicable && mtrx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                            {
                                // Update Product Container on Transaction
                                mtrx.SetVAM_ProductContainer_ID(sLine.GetVAM_ProductContainer_ID());
                                // update containr or withot container qty Current Qty 
                                mtrx.SetContainerCurrentQty(containerCurrentQty + QtyMA);
                            }
                            if (!mtrx.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = pp.GetName();
                                else
                                    _processMsg = "Could not create Material Transaction (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }

                            //Update Transaction for Current Quantity
                            if (isContainrApplicable && mtrx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                            {
                                //String errorMessage = UpdateTransactionContainer(sLine, mtrx, Qty + trxQty.Value);
                                String errorMessage = UpdateTransactionContainer(sLine, mtrx, QtyMA + trxQty.Value);
                                if (!String.IsNullOrEmpty(errorMessage))
                                {
                                    SetProcessMsg(errorMessage);
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                UpdateTransaction(sLine, mtrx, Qty + trxQty.Value);
                                //UpdateCurrentRecord(sLine, mtrx, Qty);
                            }
                            #endregion

                        }
                    }
                    if (mtrx == null)
                    {
                        #region WHEN ASI available on line - BUT now not to execute this block
                        Decimal? containerCurrentQty = 0;
                        //	Fallback: Update Storage - see also VMatch.createMatchRecord

                        if (sLine.GetVAB_OrderLine_ID() != 0 && !IsReturnTrx())
                        {
                            MVABOrderLine ordLine = new MVABOrderLine(GetCtx(), sLine.GetVAB_OrderLine_ID(), Get_TrxName());
                            MVABOrder ord = new MVABOrder(GetCtx(), ordLine.GetVAB_Order_ID(), Get_TrxName());
                            if (!IsReversal())
                            {
                                qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                if (qtytoset >= sLine.GetMovementQty())
                                {
                                    qtytoset = sLine.GetMovementQty();
                                }
                                qtytoset = Decimal.Negate(qtytoset);
                            }
                            else
                            {
                                if (IsSOTrx())
                                {
                                    qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                }
                                else
                                {
                                    qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), Decimal.Add(ordLine.GetQtyDelivered(), Decimal.Negate(sLine.GetMovementQty())));
                                }

                                if (qtytoset < 0)
                                {
                                    qtytoset = Decimal.Add(qtytoset, Decimal.Negate(sLine.GetMovementQty()));
                                }
                                else
                                {
                                    qtytoset = Decimal.Negate(sLine.GetMovementQty());
                                }

                                // when Qty Ordered == 0 on sales order then qtytoset become Movement qty from Shipment
                                // this qty is used to update qtyreserved on storage
                                if (ordLine.GetQtyOrdered() == 0)
                                {
                                    qtytoset = Decimal.Negate(sLine.GetMovementQty());
                                }
                            }
                            if (IsSOTrx())
                            {
                                QtySO = qtytoset;
                            }
                            else
                            {
                                QtyPO = qtytoset;
                            }
                            if (!(VAPOS_POSTerminal_ID > 0))
                            {
                                //Added by Vivek assigned by Pradeep on 27/09/2017
                                // if MR/Shipment is not of drop ship type then old warehouse will be of order's header else new warehouse
                                if (!sLine.IsDropShip())
                                {
                                    if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                                                sLine.GetVAM_Locator_ID(), ord.GetVAM_Warehouse_ID(),
                                                                sLine.GetVAM_Product_ID(),
                                                                sLine.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                                                Qty, QtySO, QtyPO, Get_TrxName()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                            _processMsg = pp.GetName();
                                        else
                                            _processMsg = "Cannot correct Inventory";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                else
                                {
                                    if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                                                sLine.GetVAM_Locator_ID(), GetVAM_Warehouse_ID(),
                                                                sLine.GetVAM_Product_ID(),
                                                                sLine.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                                                Qty, QtySO, QtyPO, Get_TrxName()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                            _processMsg = "Cannot correct Inventory. " + pp.GetName();
                                        else
                                            _processMsg = "Cannot correct Inventory";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                            }
                            else if (VAPOS_POSTerminal_ID > 0)
                            {
                                if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                sLine.GetVAM_Locator_ID(),
                                sLine.GetVAM_Product_ID(),
                                sLine.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                Qty, QtySO, QtyPO, Get_TrxName()))
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        _processMsg = "Cannot correct Inventory. " + pp.GetName();
                                    else
                                        _processMsg = "Cannot correct Inventory";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                        }
                        else
                        {
                            if (!MVAMStorage.Add(GetCtx(), GetVAM_Warehouse_ID(),
                                sLine.GetVAM_Locator_ID(),
                                sLine.GetVAM_Product_ID(),
                                sLine.GetVAM_PFeature_SetInstance_ID(), reservationAttributeSetInstance_ID,
                                Qty, QtySO, QtyPO, Get_TrxName()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = "Cannot correct Inventory. " + pp.GetName();
                                else
                                    _processMsg = "Cannot correct Inventory";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                        // Get Current Qty from Transaction
                        sql.Clear();
                        sql.Append(@"SELECT DISTINCT FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() +
                           " AND t.VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + sLine.GetVAM_PFeature_SetInstance_ID());
                        trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                        if (isContainrApplicable && sLine.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            // get Container qty from transaction
                            containerCurrentQty = GetContainerQtyFroMVAMInvTrx(sLine, GetMovementDate());
                        }

                        //	FallBack: Create Transaction
                        mtrx = new MVAMInvTrx(GetCtx(), sLine.GetVAF_Org_ID(), MovementType, sLine.GetVAM_Locator_ID(),
                            sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(),
                            Qty, GetMovementDate(), Get_TrxName());
                        mtrx.SetVAM_Inv_InOutLine_ID(sLine.GetVAM_Inv_InOutLine_ID());
                        mtrx.SetCurrentQty(trxQty + Qty);
                        if (isContainrApplicable && mtrx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            // Update Product Container on Transaction
                            mtrx.SetVAM_ProductContainer_ID(sLine.GetVAM_ProductContainer_ID());
                            // update containr Current Qty 
                            mtrx.SetContainerCurrentQty(containerCurrentQty + Qty);
                        }
                        if (!mtrx.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = pp.GetName();
                            else
                                _processMsg = "Could not create Material Transaction";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Update Transaction for Current Quantity
                        if (isContainrApplicable && mtrx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                        {
                            String errorMessage = UpdateTransactionContainer(sLine, mtrx, Qty + trxQty.Value);
                            if (!String.IsNullOrEmpty(errorMessage))
                            {
                                SetProcessMsg(errorMessage);
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else
                        {
                            UpdateTransaction(sLine, mtrx, Qty + trxQty.Value);
                            //UpdateCurrentRecord(sLine, mtrx, Qty);
                        }
                        #endregion
                    }
                }	//	stock movement

                //	Correct Order Line
                if (product != null && oLine != null && !IsReturnTrx())		//	other in VMatch.createMatchRecord
                {
                    if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                    {

                    }
                    else
                    {
                        qtytoset = Decimal.Negate(qtytoset);
                    }
                    if (IsSOTrx())
                    {
                        //oLine.SetQtyReserved(Decimal.Add(oLine.GetQtyReserved(), qtytoset));
                        oLine.SetQtyReserved(Decimal.Add(oLine.GetQtyReserved(), Qty));
                        if (oLine.GetQtyReserved() < 0)
                        {
                            oLine.SetQtyReserved(0);
                        }
                    }
                    else
                    {
                        //oLine.SetQtyReserved(Decimal.Subtract(oLine.GetQtyReserved(), qtytoset));
                        oLine.SetQtyReserved(Decimal.Subtract(oLine.GetQtyReserved(), Qty));
                        if (oLine.GetQtyReserved() < 0)
                        {
                            oLine.SetQtyReserved(0);
                        }
                    }
                }

                //	Update Sales Order Line
                if (oLine != null)
                {
                    if (!IsReturnTrx())
                    {
                        if (IsSOTrx()							//	PO is done by Matching
                                || sLine.GetVAM_Product_ID() == 0)	//	PO Charges, empty lines
                        {
                            if (IsSOTrx())
                                oLine.SetQtyDelivered(Decimal.Subtract(oLine.GetQtyDelivered(), Qty));
                            else
                                oLine.SetQtyDelivered(Decimal.Add(oLine.GetQtyDelivered(), Qty));
                            oLine.SetDateDelivered(GetMovementDate());	//	overwrite=last
                        }
                    }
                    else // Returns
                    {
                        MVABOrderLine origOrderLine = new MVABOrderLine(GetCtx(), oLine.GetOrig_OrderLine_ID(), Get_TrxName());
                        if (IsSOTrx()							//	PO is done by Matching
                                || sLine.GetVAM_Product_ID() == 0)	//	PO Charges, empty lines
                        {
                            if (IsSOTrx())
                            {
                                oLine.SetQtyDelivered(Decimal.Add(oLine.GetQtyDelivered(), Qty));
                                oLine.SetQtyReturned(Decimal.Add(oLine.GetQtyReturned(), Qty));
                                origOrderLine.SetQtyReturned(Decimal.Add(origOrderLine.GetQtyReturned(), Qty));
                            }
                            else
                            {
                                oLine.SetQtyDelivered(Decimal.Subtract(oLine.GetQtyDelivered(), Qty));
                                oLine.SetQtyReturned(Decimal.Subtract(oLine.GetQtyReturned(), Qty));
                                origOrderLine.SetQtyReturned(Decimal.Subtract(origOrderLine.GetQtyReturned(), Qty));
                            }
                        }

                        oLine.SetDateDelivered(GetMovementDate());	//	overwrite=last

                        if (!origOrderLine.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                _processMsg = "Could not update Original Order Line. " + pp.GetName();
                            else
                                _processMsg = "Could not update Original Order Line";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        log.Fine("QtyRet " + origOrderLine.GetQtyReturned().ToString() + " Qty : " + Qty.ToString());

                    }

                    //Update Blanket Sales Order line on 9Aug, 2017, Sukhwinder.
                    if (oLine != null && (oLine.GetVAB_OrderLine_Blanket_ID() > 0 || oLine.GetOrig_OrderLine_ID() > 0))
                    {
                        MVABOrderLine lineBlanket1 = new MVABOrderLine(GetCtx(), oLine.GetVAB_OrderLine_Blanket_ID(), Get_TrxName());
                        MVABOrderLine origOrderLine = new MVABOrderLine(GetCtx(), oLine.GetOrig_OrderLine_ID(), Get_TrxName());
                        MVABOrderLine lineBlanket = null;

                        if (lineBlanket1 != null && lineBlanket1.Get_ID() > 0)
                        {
                            if (IsSOTrx())
                            {
                                lineBlanket1.SetQtyDelivered(Decimal.Subtract(lineBlanket1.GetQtyDelivered(), Qty));
                            }
                            else
                            {
                                lineBlanket1.SetQtyDelivered(Decimal.Add(lineBlanket1.GetQtyDelivered(), Qty));
                            }

                            lineBlanket1.SetDateDelivered(GetMovementDate());   //	overwrite=last                            

                            if (!lineBlanket1.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = "Could not update Blanket Order Line. " + pp.GetName();
                                else
                                    _processMsg = "Could not update Blanket Order Line";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                        // For Return Order created for Blanket Order
                        if (origOrderLine.GetVAB_OrderLine_Blanket_ID() > 0)
                        {
                            lineBlanket = new MVABOrderLine(GetCtx(), origOrderLine.GetVAB_OrderLine_Blanket_ID(), Get_TrxName());

                        }

                        if (lineBlanket != null && lineBlanket.Get_ID() > 0)
                        {
                            if (IsSOTrx())
                            {
                                lineBlanket.SetQtyReturned(Decimal.Add(lineBlanket.GetQtyReturned(), Qty));
                            }
                            else
                            {
                                lineBlanket.SetQtyReturned(Decimal.Subtract(lineBlanket.GetQtyReturned(), Qty));
                            }

                            lineBlanket.SetDateDelivered(GetMovementDate());	//	overwrite=last                            

                            if (!lineBlanket.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = "Could not update Blanket Order Line. " + pp.GetName();
                                else
                                    _processMsg = "Could not update Blanket Order Line";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                    }

                    if (!oLine.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = "Could not update Order Line. " + pp.GetName();
                        else
                            _processMsg = "Could not update Order Line";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    else
                    {
                        log.Fine("OrderLine -> Reserved=" + oLine.GetQtyReserved().ToString()
                        + ", Delivered=" + oLine.GetQtyDelivered().ToString()
                        + ", Returned=" + oLine.GetQtyReturned().ToString());
                    }
                }

                List<MVAMMatchInvoice> matchedInvoice = new List<MVAMMatchInvoice>();
                //	Matching
                if (!IsSOTrx()
                    && sLine.GetVAM_Product_ID() != 0
                    && !IsReversal())
                {
                    Decimal matchQty = sLine.GetMovementQty();
                    //	Invoice - Receipt Match (requires Product)
                    MVABInvoiceLine iLine = MVABInvoiceLine.GetOfInOutLine(sLine);

                    if (iLine != null && iLine.GetVAM_Product_ID() != 0)
                    {
                        if (matchQty.CompareTo(iLine.GetQtyInvoiced()) > 0)
                            matchQty = iLine.GetQtyInvoiced();

                        // JID_0209: Need to set the correct qty on ‘Matched Receipts/Invoice’. System should set minimum qty of MR and invoice, whichever is lower. on match receipt

                        decimal alreadyMatchedQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(QTY) FROM VAM_MatchInvoice WHERE VAB_InvoiceLine_ID = " + iLine.GetVAB_InvoiceLine_ID(), null, Get_Trx()));

                        if ((alreadyMatchedQty + matchQty) > iLine.GetQtyInvoiced())
                        {
                            matchQty = iLine.GetQtyInvoiced() - alreadyMatchedQty;
                        }

                        MVAMMatchInvoice[] matches = MVAMMatchInvoice.Get(GetCtx(),
                            sLine.GetVAM_Inv_InOutLine_ID(), iLine.GetVAB_InvoiceLine_ID(), Get_TrxName());
                        if (matches == null || matches.Length == 0)
                        {
                            MVAMMatchInvoice inv = new MVAMMatchInvoice(iLine, GetMovementDate(), matchQty);
                            if (sLine.GetVAM_PFeature_SetInstance_ID() != iLine.GetVAM_PFeature_SetInstance_ID())
                            {
                                iLine.SetVAM_PFeature_SetInstance_ID(sLine.GetVAM_PFeature_SetInstance_ID());
                                iLine.Save();	//	update matched invoice with ASI
                                inv.SetVAM_PFeature_SetInstance_ID(sLine.GetVAM_PFeature_SetInstance_ID());
                            }
                            try
                            {
                                inv.Set_ValueNoCheck("VAB_BusinessPartner_ID", GetVAB_BusinessPartner_ID());
                            }
                            catch { }
                            if (!inv.Save(Get_TrxName()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                    _processMsg = "Could not create Inv Matching. " + pp.GetName();
                                else
                                    _processMsg = "Could not create Inv Matching";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            else
                            {
                                matchedInvoice.Add(inv);
                            }
                        }
                    }

                    //	Link to Order
                    if (sLine.GetVAB_OrderLine_ID() != 0)
                    {
                        log.Fine("PO Matching");
                        //	Ship - PO
                        MVAMMatchPO po = MVAMMatchPO.Create(null, sLine, GetMovementDate(), matchQty);
                        try
                        {
                            po.Set_ValueNoCheck("VAB_BusinessPartner_ID", GetVAB_BusinessPartner_ID());
                        }
                        catch { }
                        if (!po.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not create PO Matching";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        //	Update PO with ASI                      Commented by Bharat
                        //if (oLine != null && oLine.GetVAM_PFeature_SetInstance_ID() == 0)
                        //{
                        //    oLine.SetVAM_PFeature_SetInstance_ID(sLine.GetVAM_PFeature_SetInstance_ID());
                        //    oLine.Save(Get_TrxName());
                        //}
                    }
                    else	//	No Order - Try finding links via Invoice
                    {
                        //	Invoice has an Order Link
                        if (iLine != null && iLine.GetVAB_OrderLine_ID() != 0)
                        {
                            //	Invoice is created before  Shipment
                            log.Fine("PO(Inv) Matching");
                            //	Ship - Invoice
                            MVAMMatchPO po = MVAMMatchPO.Create(iLine, sLine, GetMovementDate(), matchQty);
                            try
                            {
                                po.Set_ValueNoCheck("VAB_BusinessPartner_ID", GetVAB_BusinessPartner_ID());
                            }
                            catch { }
                            if (!po.Save(Get_TrxName()))
                            {
                                _processMsg = "Could not create PO(Inv) Matching";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            //	Update PO with ASI                   Commented by Bharat
                            //oLine = new MVABOrderLine(GetCtx(), po.GetVAB_OrderLine_ID(), Get_TrxName());
                            //if (oLine != null && oLine.GetVAM_PFeature_SetInstance_ID() == 0)
                            //{
                            //    oLine.SetVAM_PFeature_SetInstance_ID(sLine.GetVAM_PFeature_SetInstance_ID());
                            //    oLine.Save(Get_TrxName());
                            //}
                        }
                    }	//	No Order
                }	//	PO Matching

                #region Calculate Foreign Cost for Average PO
                try
                {
                    if (!IsSOTrx() && !IsReturnTrx() && sLine.GetVAB_OrderLine_ID() > 0) // for MR against PO
                    {
                        MVAMProduct product1 = new MVAMProduct(GetCtx(), sLine.GetVAM_Product_ID(), Get_Trx());
                        MVABOrderLine orderLine = new MVABOrderLine(GetCtx(), lines[lineIndex].GetVAB_OrderLine_ID(), null);
                        MVABOrder order = new MVABOrder(GetCtx(), orderLine.GetVAB_Order_ID(), Get_Trx());
                        if (product1 != null && product1.GetProductType() == "I" && product1.GetVAM_Product_ID() > 0) // for Item Type product
                        {
                            //if (!MVAMVAMProductCostForeignCurrency.InsertForeignCostAveragePO(GetCtx(), order, orderLine, sLine, Get_Trx()))
                            //{
                            //    Get_Trx().Rollback();
                            //    log.Severe("Error occured during updating/inserting VAM_ProductCost_ForeignCurrency against Average PO.");
                            //    _processMsg = "Could not update Foreign Currency Cost";
                            //    return DocActionVariables.STATUS_INVALID;
                            //}
                        }
                    }
                }
                catch (Exception) { }
                #endregion

                //Enhanced By amit
                if (client.IsCostImmediate())
                {
                    #region Manage Cost Queue
                    bool isCostAdjustableOnLost = false;
                    int count = 0;

                    productCQ = new MVAMProduct(GetCtx(), sLine.GetVAM_Product_ID(), Get_Trx());
                    if (productCQ.GetProductType() == "I") // for Item Type product
                    {
                        //if (GetReversalDoc_ID() > 0 || (GetDescription() != null && GetDescription().Contains("RC")))
                        //{
                        //    // do not update current cost price during reversal, this time reverse doc contain same amount which are on original document
                        //}
                        //else
                        //{
                        // get price from VAM_ProductCost (Current Cost Price)
                        currentCostPrice = 0;
                        if ((!IsSOTrx() && IsReturnTrx()) || (IsSOTrx() && !IsReturnTrx())) // Return to vendor / Shipment
                        {
                            currentCostPrice = MVAMVAMProductCost.GetproductCosts(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                                sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID());
                        }
                        else // Material Receipt / Customer Return
                        {
                            currentCostPrice = MVAMVAMProductCost.GetproductCostAndQtyMaterial(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                               sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID(), false);
                        }
                        DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET CurrentCostPrice = " + currentCostPrice +
                                                          @" WHERE VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID(), null, Get_Trx());
                        //sLine.SetCurrentCostPrice(currentCostPrice);
                        //if (!sLine.Save(Get_Trx()))
                        //{
                        //    ValueNamePair pp = VLogger.RetrieveError();
                        //    _log.Info("Error found for Material Receipt for this Line ID = " + sLine.GetVAM_Inv_InOutLine_ID() +
                        //               " Error Name is " + pp.GetName());
                        //}
                        //}

                        _partner = new MVABBusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), null);
                        orderLine = new MVABOrderLine(GetCtx(), lines[lineIndex].GetVAB_OrderLine_ID(), null);
                        if (!IsSOTrx() && !IsReturnTrx()) // Material Receipt
                        {
                            bool isUpdatePostCurrentcostPriceFromMR = MVAMVAMProductCostElement.IsPOCostingmethod(GetCtx(), GetVAF_Client_ID(), productCQ.GetVAM_Product_ID(), Get_Trx());
                            if (orderLine == null || orderLine.GetVAB_OrderLine_ID() == 0)  // MR without PO
                            {
                                #region MR without PO
                                if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), productCQ, sLine.GetVAM_PFeature_SetInstance_ID(),
                                    "Material Receipt", null, sLine, null, null, null, 0, sLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut, optionalstr: "window"))
                                {
                                    if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                    {
                                        conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                    }
                                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                    if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                    {
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                else
                                {
                                    currentCostPrice = MVAMVAMProductCost.GetproductCostAndQtyMaterial(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                                                                             sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID(), false);
                                    DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                      @" END , IsCostImmediate = 'Y' , 
                                                     PostCurrentCostPrice = CASE WHEN 1 = " + (isUpdatePostCurrentcostPriceFromMR ? 1 : 0) +
                                                     @" THEN " + currentCostPrice + @" ELSE PostCurrentCostPrice END 
                                                WHERE VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID(), null, Get_Trx());
                                }
                                #endregion
                            }
                            else if (orderLine != null && orderLine.GetVAB_Order_ID() > 0) // MR with PO
                            {
                                #region MR with PO
                                // check IsCostAdjustmentOnLost exist on product 
                                sql.Clear();
                                sql.Append(@"SELECT COUNT(*) FROM VAF_Column WHERE IsActive = 'Y' AND 
                                       VAF_TableView_ID =  ( SELECT VAF_TableView_ID FROM VAF_TableView WHERE IsActive = 'Y' AND TableName LIKE 'VAM_Product' ) 
                                        AND ColumnName LIKE 'IsCostAdjustmentOnLost' ");
                                count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));

                                if (count > 0)
                                {
                                    isCostAdjustableOnLost = productCQ.IsCostAdjustmentOnLost();
                                }

                                MVABOrder order = new MVABOrder(GetCtx(), orderLine.GetVAB_Order_ID(), null);
                                if (order.GetDocStatus() != "VO")
                                {
                                    if (orderLine.GetQtyOrdered() == 0)
                                        continue;
                                }

                                Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                                Decimal ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();

                                amt = 0;
                                if (isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                {
                                    if (sLine.GetMovementQty() > 0)
                                        amt = ProductOrderLineCost;
                                    else
                                        amt = Decimal.Negate(ProductOrderLineCost);
                                }
                                else if (!isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), sLine.GetMovementQty());
                                }
                                else if (order.GetDocStatus() != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), sLine.GetMovementQty());
                                }
                                else if (order.GetDocStatus() == "VO")
                                {
                                    amt = Decimal.Multiply(ProductOrderPriceActual, sLine.GetQtyEntered());
                                }

                                if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), productCQ, sLine.GetVAM_PFeature_SetInstance_ID(),
                                   "Material Receipt", null, sLine, null, null, null, amt,
                                   sLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut, optionalstr: "window"))
                                {
                                    if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                    {
                                        conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                    }
                                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                    if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                    {
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                else
                                {
                                    currentCostPrice = MVAMVAMProductCost.GetproductCostAndQtyMaterial(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                                                                             sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID(), false);
                                    DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                     @" END , IsCostImmediate = 'Y' ,
                                                      PostCurrentCostPrice = CASE WHEN 1 = " + (isUpdatePostCurrentcostPriceFromMR ? 1 : 0) +
                                                      @" THEN " + currentCostPrice + @" ELSE PostCurrentCostPrice END 
                                                    WHERE VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID(), null, Get_Trx());

                                    // calculate cost of Invoice if invoice created before this MR
                                    if (matchedInvoice.Count > 0)
                                    {
                                        #region calculate cost of Invoice if invoice created before this MR
                                        for (int mi = 0; mi < matchedInvoice.Count; mi++)
                                        {
                                            // when cost alreday calculated, then no need to calculate cost
                                            if (matchedInvoice[mi].IsCostImmediate())
                                                continue;

                                            // calculate Pre Cost - means cost before updation price impact of current record
                                            if (matchedInvoice[mi].Get_ColumnIndex("CurrentCostPrice") >= 0)
                                            {
                                                // get cost from Product Cost before cost calculation
                                                currentCostPrice = MVAMVAMProductCost.GetproductCostAndQtyMaterial(GetVAF_Client_ID(), GetVAF_Org_ID(),
                                                                   matchedInvoice[mi].GetVAM_Product_ID(), matchedInvoice[mi].GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID(), false);
                                                DB.ExecuteQuery("UPDATE VAM_MatchInvoice SET CurrentCostPrice = " + currentCostPrice +
                                                                 @" WHERE VAM_MatchInvoice_ID = " + matchedInvoice[mi].GetVAM_MatchInvoice_ID(), null, Get_Trx());

                                            }

                                            // calculate invoice line costing after calculating costing of linked MR line 
                                            MVABInvoiceLine invoiceLine = new MVABInvoiceLine(GetCtx(), matchedInvoice[mi].GetVAB_InvoiceLine_ID(), Get_Trx());
                                            Decimal ProductLineCost = invoiceLine.GetProductLineCost(invoiceLine);
                                            if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), productCQ, matchedInvoice[mi].GetVAM_PFeature_SetInstance_ID(),
                                                  "Invoice(Vendor)", null, sLine, null, invoiceLine, null,
                                                  count > 0 && isCostAdjustableOnLost && (matchedInvoice[mi].GetQty() < invoiceLine.GetQtyInvoiced()) ? ProductLineCost : Decimal.Multiply(Decimal.Divide(ProductLineCost, invoiceLine.GetQtyInvoiced()), matchedInvoice[mi].GetQty()),
                                                matchedInvoice[mi].GetQty(), Get_Trx(), out conversionNotFoundInvoice, optionalstr: "window"))
                                            {
                                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                                if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                                {
                                                    return DocActionVariables.STATUS_INVALID;
                                                }
                                            }
                                            else
                                            {
                                                DB.ExecuteQuery("UPDATE VAB_InvoiceLine SET IsCostImmediate = 'Y' WHERE VAB_InvoiceLine_ID = " + matchedInvoice[mi].GetVAB_InvoiceLine_ID(), null, Get_Trx());

                                                if (matchedInvoice[mi].Get_ColumnIndex("PostCurrentCostPrice") >= 0)
                                                {
                                                    // get cost from Product Cost after cost calculation
                                                    currentCostPrice = MVAMVAMProductCost.GetproductCostAndQtyMaterial(GetVAF_Client_ID(), GetVAF_Org_ID(),
                                                                     matchedInvoice[mi].GetVAM_Product_ID(), matchedInvoice[mi].GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID(), false);
                                                    matchedInvoice[mi].SetPostCurrentCostPrice(currentCostPrice);
                                                }
                                                matchedInvoice[mi].SetIsCostImmediate(true);
                                                matchedInvoice[mi].Save(Get_Trx());

                                                // update the Post current price after Invoice receving on inoutline
                                                if (!isUpdatePostCurrentcostPriceFromMR)
                                                {
                                                    DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET PostCurrentCostPrice =  " + currentCostPrice +
                                                                     @"  WHERE VAM_Inv_InOutLine_ID = " + matchedInvoice[mi].GetVAM_Inv_InOutLine_ID(), null, Get_Trx());
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                        }
                        else if (IsSOTrx() && IsReturnTrx()) // Customer Return
                        {
                            #region Customer Return
                            if (GetOrig_Order_ID() <= 0)
                            {
                                break;
                            }

                            if (orderLine != null && orderLine.GetVAB_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                break;

                            Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);

                            if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), productCQ, sLine.GetVAM_PFeature_SetInstance_ID(),
                                  "Customer Return", null, sLine, null, null, null, Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), sLine.GetMovementQty()),
                                  sLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut, optionalstr: "window"))
                            {
                                if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                {
                                    conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                }
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                {
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                currentCostPrice = MVAMVAMProductCost.GetproductCostAndQtyMaterial(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                               sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID(), false);
                                DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                     @" END , IsCostImmediate = 'Y' WHERE VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID(), null, Get_Trx());
                            }
                            #endregion
                        }
                        else if (IsSOTrx() && !IsReturnTrx())  // shipment
                        {
                            #region shipment
                            if (GetVAB_Order_ID() <= 0)
                            {
                                break;
                            }

                            if (orderLine != null && orderLine.GetVAB_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0) { break; }

                            Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);

                            if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), productCQ, sLine.GetVAM_PFeature_SetInstance_ID(),
                                  "Shipment", null, sLine, null, null, null, Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(sLine.GetMovementQty())),
                                 Decimal.Negate(sLine.GetMovementQty()), Get_Trx(), out conversionNotFoundInOut, optionalstr: "window"))
                            {
                                if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                {
                                    conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                }
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                {
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                currentCostPrice = MVAMVAMProductCost.GetproductCosts(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                               sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID());
                                DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                     @" END , IsCostImmediate = 'Y' WHERE VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID(), null, Get_Trx());
                            }
                            #endregion
                        }
                        else if (!IsSOTrx() && IsReturnTrx()) // Return To Vendor
                        {
                            if (GetOrig_Order_ID() == 0 || orderLine == null || orderLine.GetVAB_OrderLine_ID() == 0)
                            {
                                #region Return To Vendor -- without order refernce
                                if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), productCQ, sLine.GetVAM_PFeature_SetInstance_ID(),
                               "Return To Vendor", null, sLine, null, null, null, 0, Decimal.Negate(sLine.GetMovementQty()), Get_Trx(),
                                out conversionNotFoundInOut, optionalstr: "window"))
                                {
                                    if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                    {
                                        conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                    }
                                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                    if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                    {
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                else
                                {
                                    currentCostPrice = MVAMVAMProductCost.GetproductCosts(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                               sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID());
                                    DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                      @" END , IsCostImmediate = 'Y' WHERE VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID(), null, Get_Trx());
                                    //sLine.SetIsCostImmediate(true);
                                    //sLine.Save();
                                }
                                #endregion
                            }
                            else if (orderLine != null && orderLine.GetVAB_Order_ID() > 0)
                            {
                                #region Return To Vendor -- with order refernce
                                MVABOrder order = new MVABOrder(GetCtx(), orderLine.GetVAB_Order_ID(), null);
                                if (order.GetDocStatus() != "VO")
                                {
                                    if (orderLine.GetQtyOrdered() == 0)
                                        continue;
                                }
                                // check IsCostAdjustmentOnLost exist on product 
                                sql.Clear();
                                sql.Append(@"SELECT COUNT(*) FROM VAF_Column WHERE IsActive = 'Y' AND 
                                       VAF_TableView_ID =  ( SELECT VAF_TableView_ID FROM VAF_TableView WHERE IsActive = 'Y' AND TableName LIKE 'VAM_Product' ) 
                                       AND ColumnName LIKE 'IsCostAdjustmentOnLost' ");
                                count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));

                                if (count > 0)
                                {
                                    isCostAdjustableOnLost = productCQ.IsCostAdjustmentOnLost();
                                }

                                Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                                Decimal ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();

                                amt = 0;
                                if (isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                {
                                    // Cost Adjustment case
                                    if (sLine.GetMovementQty() < 0)
                                        amt = ProductOrderLineCost;
                                    else
                                        amt = Decimal.Negate(ProductOrderLineCost);
                                }
                                else if (!isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(sLine.GetMovementQty()));
                                }
                                else if (order.GetDocStatus() != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(sLine.GetMovementQty()));
                                }
                                else if (order.GetDocStatus() == "VO")
                                {
                                    amt = Decimal.Multiply(ProductOrderPriceActual, Decimal.Negate(sLine.GetQtyEntered()));
                                }

                                if (!MVAMVAMProductCostQueue.CreateProductCostsDetails(GetCtx(), GetVAF_Client_ID(), GetVAF_Org_ID(), productCQ, sLine.GetVAM_PFeature_SetInstance_ID(),
                                    "Return To Vendor", null, sLine, null, null, null, amt,
                                    Decimal.Negate(sLine.GetMovementQty()), Get_Trx(), out conversionNotFoundInOut, optionalstr: "window"))
                                {
                                    if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                    {
                                        conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                    }
                                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_CostNotCalculated");// "Could not create Product Costs";
                                    if (client.Get_ColumnIndex("IsCostMandatory") > 0 && client.IsCostMandatory())
                                    {
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                else
                                {
                                    currentCostPrice = MVAMVAMProductCost.GetproductCosts(sLine.GetVAF_Client_ID(), sLine.GetVAF_Org_ID(),
                               sLine.GetVAM_Product_ID(), sLine.GetVAM_PFeature_SetInstance_ID(), Get_Trx(), GetVAM_Warehouse_ID());
                                    DB.ExecuteQuery("UPDATE VAM_Inv_InOutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                     @" END , IsCostImmediate = 'Y' WHERE VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID(), null, Get_Trx());
                                    //sLine.SetIsCostImmediate(true);
                                    //sLine.Save();
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                //end

                // Stopped asset craetion while drop shipment is true
                if (!IsDropShip())
                {
                    //	Create Asset during receiving
                    // JID_1251:On Material receipt system will generate the asset for Items type product for which asset group linked with Product Category.
                    if (product != null && product.GetProductType() == X_VAM_Product.PRODUCTTYPE_Item && product.IsCreateAsset() && sLine.GetMovementQty() > 0
                       && !IsReversal() && !IsReturnTrx() && !IsSOTrx() && sLine.GetA_Asset_ID() == 0)
                    {
                        log.Fine("Asset");
                        Info.Append("@VAA_Asset_ID@: ");
                        int noAssets = (int)sLine.GetMovementQty();

                        // Check Added only run when Product is one Asset Per UOM
                        if (product.IsOneAssetPerUOM())
                        {
                            for (int i = 0; i < noAssets; i++)
                            {
                                if (i > 0)
                                    Info.Append(" - ");
                                int deliveryCount = i + 1;
                                if (product.IsOneAssetPerUOM())
                                    deliveryCount = 0;
                                MVAAsset asset = new MVAAsset(this, sLine, deliveryCount);
                                if (!asset.Save(Get_TrxName()))
                                {
                                    _processMsg = "Could not create Asset";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                                else
                                {
                                    asset.SetName(asset.GetName() + "_" + asset.GetValue());
                                    asset.Save(Get_TrxName());
                                }
                                Info.Append(asset.GetValue());
                            }
                        }
                        else
                        {
                            #region[Added by Sukhwinder (mantis ID: 1762, point 1)]
                            if (noAssets > 0)
                            {
                                MVAAsset asset = new MVAAsset(this, sLine, noAssets);
                                if (!asset.Save(Get_TrxName()))
                                {
                                    _processMsg = "Could not create Asset";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                                else
                                {
                                    asset.SetName(asset.GetName() + "_" + asset.GetValue());
                                    asset.Save(Get_TrxName());
                                }
                                Info.Append(asset.GetValue());
                            }
                            #endregion
                        }
                        //Create Asset Delivery
                    }
                }

                #region For Obsolete Inventory (16-05-2016)
                //by Amit -> For Obsolete Inventory (16-05-2016)
                if (Env.IsModuleInstalled("VA024_"))
                {
                    // shipment Or Return To Vendor
                    try
                    {
                        if (sLine.GetVAM_Product_ID() > 0 &&
                            ((IsSOTrx() && !IsReturnTrx()) || (!IsSOTrx() && IsReturnTrx())))
                        {
                            sql.Clear();
                            sql.Append(@"SELECT * FROM va024_t_obsoleteinventory
                                     WHERE vaf_client_id    = " + GetVAF_Client_ID() + " AND vaf_org_id = " + GetVAF_Org_ID() +
                                     "   AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() +
                                       " AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + sLine.GetVAM_PFeature_SetInstance_ID());
                            //" AND VAM_Warehouse_Id = " + GetVAM_Warehouse_ID();
                            if (GetDescription() != null && !GetDescription().Contains("{->"))
                            {
                                sql.Append(" AND va024_isdelivered = 'N' ");
                            }
                            sql.Append(" ORDER BY va024_t_obsoleteinventory_id DESC");
                            DataSet dsObsoleteInventory = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                            if (dsObsoleteInventory != null && dsObsoleteInventory.Tables.Count > 0 && dsObsoleteInventory.Tables[0].Rows.Count > 0)
                            {
                                Decimal remainigQty = sLine.GetQtyEntered();
                                MVAFTableView tbl = new MVAFTableView(GetCtx(), tableId, null);
                                PO po = null;
                                for (int oi = 0; oi < dsObsoleteInventory.Tables[0].Rows.Count; oi++)
                                {
                                    po = tbl.GetPO(GetCtx(), Util.GetValueOfInt(dsObsoleteInventory.Tables[0].Rows[oi]["va024_t_obsoleteinventory_ID"]), Get_Trx());
                                    if (Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")) < remainigQty)
                                    {
                                        po.Set_Value("VA024_RemainingQty", 0);
                                        po.Set_Value("VA024_IsDelivered", true);
                                        po.Set_Value("VA024_ShipmentQty", Decimal.Add(Util.GetValueOfDecimal(po.Get_Value("VA024_ShipmentQty")), Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty"))));
                                        remainigQty = Decimal.Subtract(remainigQty, Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")));
                                        if (!po.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            log.Info("Error Occured when try to reduce Obsolete qty. Error Type : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                            _processMsg = "Not able to Reduce Obsolete Inventory";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else if (Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")) == remainigQty)
                                    {
                                        po.Set_Value("VA024_RemainingQty", 0);
                                        po.Set_Value("VA024_IsDelivered", true);
                                        po.Set_Value("VA024_ShipmentQty", Decimal.Add(Util.GetValueOfDecimal(po.Get_Value("VA024_ShipmentQty")), remainigQty));
                                        if (!po.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            log.Info("Error Occured when try to reduce Obsolete qty. Error Type : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                            _processMsg = "Not able to Reduce Obsolete Inventory";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else if (Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")) > remainigQty)
                                    {
                                        po.Set_Value("VA024_RemainingQty", Decimal.Subtract(Util.GetValueOfDecimal(po.Get_Value("VA024_RemainingQty")), remainigQty));
                                        po.Set_Value("VA024_ShipmentQty", Decimal.Add(Util.GetValueOfDecimal(po.Get_Value("VA024_ShipmentQty")), remainigQty));
                                        po.Set_Value("VA024_IsDelivered", false);
                                        if (!po.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            log.Info("Error Occured when try to reduce Obsolete qty. Error Type : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                            _processMsg = "Not able to Reduce Obsolete Inventory";
                                            return DocActionVariables.STATUS_INVALID;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            dsObsoleteInventory.Dispose();
                        }
                    }
                    catch { }
                }
                //End
                #endregion

                #region update  receipt No and Date Trx on LC - By Amit
                // update  receipt No and Date Trx on LC - By Amit
                if (sLine.GetVAB_OrderLine_ID() != 0)
                {
                    try
                    {
                        string PaymentBaseType = "";
                        if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE IsActive = 'Y' AND PREFIX IN ('VA009_' , 'VA026_' ) ")) >= 2)
                        {
                            MVABOrderLine ordLine = new MVABOrderLine(GetCtx(), sLine.GetVAB_OrderLine_ID(), Get_TrxName());
                            MVABOrder ord = new MVABOrder(GetCtx(), ordLine.GetVAB_Order_ID(), Get_TrxName());
                            if (tableId1 <= 0 && ord.GetPaymentMethod() == "L")
                            {
                                _processMsg = "Could not Update Letter of Credit";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            MVAFTableView tbl = new MVAFTableView(GetCtx(), tableId1, null);
                            // Change By Mohit- to pick payment base type from VA009_PaymentMthod----------------------
                            PaymentBaseType = Util.GetValueOfString(DB.ExecuteScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod  WHERE VA009_PaymentMethod_ID=" + ord.GetVA009_PaymentMethod_ID(), null, null));
                            //End change--------------------------------------------------------------------------------
                            if (PaymentBaseType == "L")
                            {

                                if (ord.IsSOTrx())
                                {
                                    VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(VA026_LCDetail_ID)  FROM VA026_LCDetail 
                                                            WHERE IsActive = 'Y' AND DocStatus IN ('CO' , 'CL')  AND
                                                            VA026_Order_ID =" + ord.GetVAB_Order_ID(), null, Get_Trx()));
                                    // Check SO Detail tab of Letter of Credit
                                    if (VA026_LCDetail_ID == 0)
                                    {
                                        VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(lc.VA026_LCDetail_ID)  FROM VA026_LCDetail lc
                                                        INNER JOIN VA026_SODetail sod ON sod.VA026_LCDetail_ID = lc.VA026_LCDetail_ID 
                                                            WHERE sod.IsActive = 'Y' AND lc.IsActive = 'Y' AND lc.DocStatus IN ('CO' , 'CL')  AND
                                                            sod.VAB_Order_ID =" + ord.GetVAB_Order_ID(), null, Get_Trx()));
                                    }
                                }
                                else if (!ord.IsSOTrx() && !ord.IsReturnTrx())
                                {
                                    VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(VA026_LCDetail_ID)  FROM VA026_LCDetail 
                                                            WHERE IsActive = 'Y' AND DocStatus IN ('CO' , 'CL')  AND
                                                            VAB_Order_id =" + ord.GetVAB_Order_ID(), null, Get_Trx()));
                                    // Check PO Detail tab of Letter of Credit
                                    if (VA026_LCDetail_ID == 0)
                                    {
                                        VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(lc.VA026_LCDetail_ID)  FROM VA026_LCDetail lc
                                                        INNER JOIN VA026_PODetail sod ON sod.VA026_LCDetail_ID = lc.VA026_LCDetail_ID 
                                                            WHERE sod.IsActive = 'Y' AND lc.IsActive = 'Y' AND lc.DocStatus IN ('CO' , 'CL')  AND
                                                            sod.VAB_Order_ID =" + ord.GetVAB_Order_ID(), null, Get_Trx()));
                                    }
                                }

                                if (VA026_LCDetail_ID == 0)
                                {
                                    _processMsg = Msg.GetMsg(GetCtx(), "VA026_LCNotDefine");
                                    return DocActionVariables.STATUS_INVALID;
                                }
                                PO po = tbl.GetPO(GetCtx(), Util.GetValueOfInt(VA026_LCDetail_ID), Get_Trx());
                                if (Util.GetValueOfString(po.Get_Value("DocStatus")) == "CO" || Util.GetValueOfString(po.Get_Value("DocStatus")) == "CL")
                                {
                                    if (Util.GetValueOfString(po.Get_Value("VA026_ReceievedIssue")) == "I")
                                    {
                                        po.Set_Value("VA026_ReceiptDate", GetDateAcct());
                                        po.Set_Value("VAM_Inv_InOut_ID", GetVAM_Inv_InOut_ID());
                                    }
                                    else if (Util.GetValueOfString(po.Get_Value("VA026_ReceievedIssue")) == "R")
                                    {
                                        po.Set_Value("VA026_ShipmentDate", GetDateAcct());
                                        po.Set_Value("Va026_InOut_ID", GetVAM_Inv_InOut_ID());
                                    }
                                    if (!po.Save(Get_Trx()))
                                    {
                                        Get_Trx().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        log.Info("Error Occured when try to update record on LC Detail. Error Type : " + pp.GetValue() + "  , Error Name : " + pp.GetName());
                                        _processMsg = "Could not Update Letter of Credit";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                else
                                {
                                    _processMsg = Msg.GetMsg(GetCtx(), "VA026_CompleteLC");
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                        }
                    }
                    catch { }
                }
                #endregion

                #region If shipment happened against an Asset, then UnCheck 'InPossesion' check box from checkbox.
                if (IsSOTrx() && !IsReturnTrx() && sLine.GetA_Asset_ID() > 0)
                {
                    DB.ExecuteQuery(@"UPDATE VAA_Asset Set IsInposession = 'N' 
                                WHERE IsInposession = 'Y' AND VAA_Asset_ID = " + sLine.GetA_Asset_ID(), null, Get_Trx());
                }
                #endregion

            }	//	for all lines
            #endregion

            // By Amit For Foreign cost
            try
            {
                if (!IsSOTrx() && !IsReturnTrx()) // for MR against PO
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_InOutLine WHERE IsFutureCostCalculated = 'N' AND VAM_Inv_InOut_ID = " + GetVAM_Inv_InOut_ID(), null, Get_Trx())) <= 0)
                    {
                        int no = Util.GetValueOfInt(DB.ExecuteQuery("UPDATE VAM_Inv_InOut Set IsFutureCostCalculated = 'Y' WHERE VAM_Inv_InOut_ID = " + GetVAM_Inv_InOut_ID(), null, Get_Trx()));
                    }
                }
            }
            catch (Exception) { }
            //end

            //	Counter Documents
            try
            {
                MVAMInvInOut counter = CreateCounterDoc();
                if (counter != null)
                    Info.Append(" - @CounterDoc@: @VAM_Inv_InOut_ID@=").Append(counter.GetDocumentNo());
            }
            catch (Exception e)
            {
                Info.Append(" - @CounterDoc@: ").Append(e.Message.ToString());
            }

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            _processMsg = Info.ToString();

            #region Added by vikas 29 August 2016 cost sheet
            try
            {
                if (Env.IsModuleInstalled("VA033_"))
                {
                    if (GetVA033_CostSheet_ID() > 0 && IsSOTrx() == false && IsReturnTrx() == false)
                    {
                        if (IsReversal())
                        {
                            DB.ExecuteQuery(" Update VA033_CostSheet SET VA033_IsUtilized='N' where isactive='Y' and VA033_CostSheet_ID=" + GetVA033_CostSheet_ID(), null, null);
                        }
                        else
                        {
                            DB.ExecuteQuery(" Update VA033_CostSheet SET VA033_IsUtilized='Y' where isactive='Y' and VA033_CostSheet_ID=" + GetVA033_CostSheet_ID(), null, null);
                        }
                    }
                    //else
                    //{
                    //    //Added by Vivek  (Mentis issue no. 0000460)
                    //    SetDateReceived(GetMovementDate());
                    //    FreezeDoc();//Arpit Rai
                    //    return DocActionVariables.STATUS_COMPLETED;
                    //}                    
                }
            }
            catch (Exception e)
            {
                log.Severe("Error in Cost Sheet  - " + e.Message);
            }
            #endregion

            //Added by Vivek  (Mentis issue no. 0000460)

            //Added by Vivek assigned by Pradeep on 27/09/2017
            // Create shipment against drop ship material receipt after its completion
            try
            {
                if (GetDescription() != null && GetDescription().Contains("{->"))
                {
                }
                else
                {
                    if (!IsSOTrx() && !IsReturnTrx() && IsDropShip())
                    {
                        MVABOrder order = new MVABOrder(GetCtx(), GetVAB_Order_ID(), Get_Trx());
                        MVABOrder ref_order = new MVABOrder(GetCtx(), order.GetRef_Order_ID(), Get_Trx());
                        MVAMInvInOut ret_Shipment = CreateShipment(ref_order, this, GetMovementDate(),
                                    true, false, GetVAM_Warehouse_ID(), GetMovementDate(), Get_Trx());
                        if (ret_Shipment.CompleteIt() == "CO")
                        {
                            ret_Shipment.SetRef_ShipMR_ID(GetVAM_Inv_InOut_ID());
                            ret_Shipment.SetDocStatus(DOCACTION_Complete);
                            ret_Shipment.Save(Get_Trx());
                            SetRef_ShipMR_ID(ret_Shipment.GetVAM_Inv_InOut_ID());
                            _processMsg = "Shipment Generated :" + ret_Shipment.GetDocumentNo();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Severe("Error in generating shipment  - " + e.Message);
            }

            // Handled Drop shipment issues
            SetDateReceived(GetMovementDate());
            SetPickDate(DateTime.Now);
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Set the document number from Completed Doxument Sequence after completed
        /// </summary>
        protected void SetCompletedDocumentNo()
        {
            // if Reversal document then no need to get Document no from Completed sequence
            if (IsReversal())
            {
                return;
            }

            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetMovementDate(DateTime.Now.Date);
                if (GetDateAcct().Value.Date < GetMovementDate().Value.Date)
                {
                    SetDateAcct(GetMovementDate());

                    //	Std Period open?
                    if (!MVABYearPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetVAF_Org_ID()))
                    {
                        throw new Exception("@PeriodClosed@");
                    }
                }
            }

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") > 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MVAFRecordSeq.GetDocumentNo(GetVAB_DocTypes_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
                }
            }
        }

        // Amit not used 24-12-2015
        private void updateCostQueue(MVAMProduct product, int M_ASI_ID, MVABAccountBook mas,
            int Org_ID, MVAMVAMProductCostElement ce, decimal movementQty)
        {
            //MVAMVAMProductCostQueue[] cQueue = MVAMVAMProductCostQueue.GetQueue(product1, sLine.GetVAM_PFeature_SetInstance_ID(), acctSchema, GetVAF_Org_ID(), costElement, null);
            MVAMVAMProductCostQueue[] cQueue = MVAMVAMProductCostQueue.GetQueue(product, M_ASI_ID, mas, Org_ID, ce, null);
            if (cQueue != null && cQueue.Length > 0)
            {
                Decimal qty = movementQty;
                bool value = false;
                for (int cq = 0; cq < cQueue.Length; cq++)
                {
                    MVAMVAMProductCostQueue queue = cQueue[cq];
                    if (queue.GetCurrentQty() < 0) continue;
                    if (queue.GetCurrentQty() > qty)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    qty = MVAMVAMProductCostQueue.Quantity(queue.GetCurrentQty(), qty);
                    //if (cq == cQueue.Length - 1 && qty < 0) // last record
                    //{
                    //    queue.SetCurrentQty(qty);
                    //    if (!queue.Save())
                    //    {
                    //        ValueNamePair pp = VLogger.RetrieveError();
                    //        log.Info("Cost Queue not updated for  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                    //    }
                    //}
                    if (qty <= 0)
                    {
                        queue.Delete(true);
                        qty = Decimal.Negate(qty);
                    }
                    else
                    {
                        queue.SetCurrentQty(qty);
                        if (!queue.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            log.Info("Cost Queue not updated for  <===> " + product.GetVAM_Product_ID() + " Error Type is : " + pp.GetName());
                        }
                    }
                    if (value)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Update Current Quantity at Transaction Tab
        /// </summary>
        /// <param name="sLine"></param>
        /// <param name="mtrx"></param>
        /// <param name="Qty"></param>
        private String UpdateTransactionContainer(MVAMInvInOutLine sLine, MVAMInvTrx mtrx, decimal Qty)
        {
            string errorMessage = null;
            MVAMProduct pro = new MVAMProduct(Env.GetCtx(), sLine.GetVAM_Product_ID(), Get_TrxName());
            MVAMInvTrx trx = null;
            MVAMInventoryLine inventoryLine = null;
            MVAMInventory inventory = null;
            int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            string sql = "";
            DataSet ds = new DataSet();
            Decimal containerCurrentQty = mtrx.GetContainerCurrentQty();
            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty , NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty  ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + sLine.GetVAM_PFeature_SetInstance_ID()
                              + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC, created ASC";
                }
                else
                {
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty, NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = 0 "
                              + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC , created ASC";
                }
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                 Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MVAMInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MVAMInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetVAM_Inventory_ID()), null);
                                if (!inventory.IsInternalUse())
                                {
                                    #region update Physical Inventory
                                    if (inventoryLine.GetVAM_ProductContainer_ID() == sLine.GetVAM_ProductContainer_ID())
                                    {
                                        inventoryLine.SetQtyBook(containerCurrentQty);
                                        inventoryLine.SetOpeningStock(containerCurrentQty);
                                        inventoryLine.SetDifferenceQty(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"])));
                                        if (!inventoryLine.Save())
                                        {
                                            log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]));
                                            Get_TrxName().Rollback();
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                            {
                                                _processMsg = pp.GetName();
                                                return Msg.GetMsg(GetCtx(), "VIS_InventoryLineNotSaved") + _processMsg;
                                            }
                                            else
                                            {
                                                return Msg.GetMsg(GetCtx(), "VIS_InventoryLineNotSaved");
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Update Transaction
                                    trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                                    if (trx.GetVAM_ProductContainer_ID() == sLine.GetVAM_ProductContainer_ID())
                                    {
                                        trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"]))));
                                    }
                                    else
                                    {
                                        trx.SetCurrentQty(Decimal.Add(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["movementqty"])));
                                    }
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                                        Get_TrxName().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        {
                                            _processMsg = pp.GetName();
                                            return _processMsg;
                                        }
                                        else
                                        {
                                            return Msg.GetMsg(GetCtx(), "VIS_TranactionNotSaved");
                                        }
                                    }
                                    else
                                    {
                                        Qty = trx.GetCurrentQty();
                                        if (sLine.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && trx.GetVAM_ProductContainer_ID() == sLine.GetVAM_ProductContainer_ID())
                                            containerCurrentQty = trx.GetContainerCurrentQty();
                                    }
                                    #endregion

                                    #region update Storage if last record of loop
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != Qty)
                                        {
                                            storage.SetQtyOnHand(Qty);
                                            if (!storage.Save())
                                            {
                                                Get_TrxName().Rollback();
                                                ValueNamePair pp = VLogger.RetrieveError();
                                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                                {
                                                    _processMsg = pp.GetName();
                                                    return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved") + _processMsg;
                                                }
                                                else
                                                {
                                                    return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved");
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    continue;
                                }
                            }

                            #region Update Transaction
                            trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                            trx.SetCurrentQty(Qty + trx.GetMovementQty());
                            if (trx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && trx.GetVAM_ProductContainer_ID() == sLine.GetVAM_ProductContainer_ID())
                                trx.SetContainerCurrentQty(containerCurrentQty + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                                Get_TrxName().Rollback();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                {
                                    _processMsg = pp.GetName();
                                    return _processMsg;
                                }
                                else
                                {
                                    return Msg.GetMsg(GetCtx(), "VIS_TranactionNotSaved");
                                }
                            }
                            else
                            {
                                Qty = trx.GetCurrentQty();
                                if (trx.Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && trx.GetVAM_ProductContainer_ID() == sLine.GetVAM_ProductContainer_ID())
                                    containerCurrentQty = trx.GetContainerCurrentQty();
                            }
                            #endregion

                            #region update Storage if last record of loop
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != Qty)
                                {
                                    storage.SetQtyOnHand(Qty);
                                    if (!storage.Save())
                                    {
                                        Get_TrxName().Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                                        {
                                            _processMsg = pp.GetName();
                                            return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved") + _processMsg;
                                        }
                                        else
                                        {
                                            return Msg.GetMsg(GetCtx(), "VIS_StorageNotSaved");
                                        }
                                    }
                                }
                            }
                            #endregion

                        }
                    }
                }
                ds.Dispose();
            }
            catch
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
                errorMessage = Msg.GetMsg(GetCtx(), "ExceptionOccureOnUpdateTrx");
            }
            return errorMessage;
        }


        /// <summary>
        /// Update Current Quantity at Transaction Tab
        /// </summary>
        /// <param name="sLine"></param>
        /// <param name="mtrx"></param>
        /// <param name="Qty"></param>
        private void UpdateTransaction(MVAMInvInOutLine sLine, MVAMInvTrx mtrx, decimal Qty)
        {
            MVAMProduct pro = new MVAMProduct(Env.GetCtx(), sLine.GetVAM_Product_ID(), Get_TrxName());
            MVAMInvTrx trx = null;
            MVAMInventoryLine inventoryLine = null;
            MVAMInventory inventory = null;
            int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            string sql = "";
            DataSet ds = new DataSet();
            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + sLine.GetVAM_PFeature_SetInstance_ID();
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + sLine.GetVAM_PFeature_SetInstance_ID()
                              + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC, created ASC";
                }
                else
                {
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = 0 ";
                    sql = @"SELECT VAM_PFeature_SetInstance_ID ,  VAM_Locator_ID ,  VAM_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , VAM_Inv_Trx_id ,  MovementType , VAM_InventoryLine_ID
                              FROM VAM_Inv_Trx WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = 0 "
                              + " ORDER BY movementdate ASC , VAM_Inv_Trx_id ASC , created ASC";
                }
                //int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                 Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MVAMInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MVAMInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetVAM_Inventory_ID()), null);
                                if (!inventory.IsInternalUse())
                                {
                                    //break;
                                    inventoryLine.SetQtyBook(Qty);
                                    inventoryLine.SetOpeningStock(Qty);
                                    inventoryLine.SetDifferenceQty(Decimal.Subtract(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"])));
                                    if (!inventoryLine.Save())
                                    {
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_InventoryLine_ID"]));
                                    }

                                    trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                                    trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"]))));
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                                    }
                                    else
                                    {
                                        Qty = trx.GetCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != Qty)
                                        {
                                            storage.SetQtyOnHand(Qty);
                                            storage.Save();
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MVAMInvTrx(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]), Get_TrxName());
                            trx.SetCurrentQty(Qty + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Inv_Trx_id"]));
                            }
                            else
                            {
                                Qty = trx.GetCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MVAMStorage storage = MVAMStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MVAMStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAM_PFeature_SetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != Qty)
                                {
                                    storage.SetQtyOnHand(Qty);
                                    storage.Save();
                                }
                            }
                        }
                    }
                }
                ds.Dispose();
            }
            catch
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }

        private void UpdateCurrentRecord(MVAMInvInOutLine line, MVAMInvTrx trxM, decimal qtyDiffer)
        {
            MVAMProduct pro = new MVAMProduct(Env.GetCtx(), line.GetVAM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            string sql = "";

            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT Count(*) from VAM_Inv_Trx  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + line.GetVAM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + "  and VAM_Locator_ID=" + line.GetVAM_Locator_ID() + " )order by VAM_Inv_Trx_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + line.GetVAM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + " and VAM_Locator_ID=" + line.GetVAM_Locator_ID() + ") order by VAM_Inv_Trx_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }

                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID()
                    //     + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
                }
                else
                {
                    sql = @"SELECT Count(*) from VAM_Inv_Trx  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + line.GetVAM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + "  and VAM_Locator_ID=" + line.GetVAM_Locator_ID() + " )order by VAM_Inv_Trx_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM VAM_Inv_Trx tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.VAM_Product_id =" + line.GetVAM_Product_ID() + "  and tr.VAM_Locator_ID=" + line.GetVAM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from VAM_Inv_Trx where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and VAM_Product_id =" + line.GetVAM_Product_ID() + " and VAM_Locator_ID=" + line.GetVAM_Locator_ID() + ") order by VAM_Inv_Trx_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }
                    //sql = "UPDATE VAM_Inv_Trx SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID();
                }

                // int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
            }
            catch
            {
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }
        /// <summary>
        /// Returns the OnHandQuantity of a Product from loacator 
        /// Based on Attribute set bind on Product
        /// </summary>
        /// <param name="sLine"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFroMVAMStorage(MVAMInvInOutLine sLine)
        {
            return 0;
            //    MVAMProduct pro = new MVAMProduct(Env.GetCtx(), sLine.GetVAM_Product_ID(), Get_TrxName());
            //    int attribSet_ID = pro.GetVAM_PFeature_Set_ID();
            //    string sql = "";

            //    if (attribSet_ID > 0)
            //    {
            //        sql = @"SELECT SUM(qtyonhand) FROM VAM_Storage WHERE VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID()
            //             + " AND VAM_PFeature_SetInstance_ID = " + sLine.GetVAM_PFeature_SetInstance_ID();
            //    }
            //    else
            //    {
            //        sql = @"SELECT SUM(qtyonhand) FROM VAM_Storage WHERE VAM_Product_ID = " + sLine.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + sLine.GetVAM_Locator_ID();
            //    }
            //    return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /// <summary>
        /// Get Latest Current Quantity based on movementdate
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFroMVAMInvTrx(MVAMInvInOutLine line, DateTime? movementDate, bool isAttribute)
        {
            decimal result = 0;
            string sql = "";

            if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id =
                        (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id =
                        (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID() + @")
                           AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = " + line.GetVAM_PFeature_SetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                   AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id =
                        (SELECT MAX(VAM_Inv_Trx_id )   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                      AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND VAM_PFeature_SetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_id =
                        (SELECT MAX(VAM_Inv_Trx_id)   FROM VAM_Inv_Trx
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND  VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) " + @")
                          AND VAM_Product_ID = " + line.GetVAM_Product_ID() + " AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            return result;
        }

        /// <summary>
        /// This function is used to get current Qty based on Product Container
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <returns></returns>
        private Decimal? GetContainerQtyFroMVAMInvTrx(MVAMInvInOutLine line, DateTime? movementDate)
        {
            Decimal result = 0;
            string sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS ContainerCurrentQty                           FROM VAM_Inv_Trx t
                           WHERE t.MovementDate <=" + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND t.VAF_Client_ID                       = " + line.GetVAF_Client_ID() + @"
                           AND t.VAM_Locator_ID                       = " + line.GetVAM_Locator_ID() + @"
                           AND t.VAM_Product_ID                       = " + line.GetVAM_Product_ID() + @"
                           AND NVL(t.VAM_PFeature_SetInstance_ID , 0) = COALESCE(" + line.GetVAM_PFeature_SetInstance_ID() + @",0)
                           AND NVL(t.VAM_ProductContainer_ID, 0)              = " + line.GetVAM_ProductContainer_ID();
            result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
            return result;
        }

        /// <summary>
        /// Check Material Policy
        /// </summary>
        /// <param name="line">Sets line ASI</param>
        private void CheckMaterialPolicy(MVAMInvInOutLine line)
        {
            int no = MVAMInvInOutLineMP.DeleteInOutLineMA(line.GetVAM_Inv_InOutLine_ID(), Get_TrxName());
            if (no > 0)
            {
                log.Config("Delete old #" + no);
            }

            // check is any record available of physical Inventory after date Movement -
            // if available - then not to create Attribute Record - neither to take impacts on Container Storage
            if (isContainrApplicable && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM VAM_ContainerStorage WHERE IsPhysicalInventory = 'Y'
                 AND MMPolicyDate > " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                      @" AND VAM_Product_ID = " + line.GetVAM_Product_ID() +
                      @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + line.GetVAM_PFeature_SetInstance_ID() +
                      @" AND VAM_Locator_ID = " + line.GetVAM_Locator_ID() +
                      @" AND NVL(VAM_ProductContainer_ID , 0) = " + line.GetVAM_ProductContainer_ID(), null, Get_Trx())) > 0)
            {
                return;
            }

            //	Incoming Trx
            String MovementType = GetMovementType();
            //bool inTrx = MovementType.charAt(1) == '+';	//	V+ Vendor Receipt, C+ Customer Return
            bool inTrx = MovementType.IndexOf('+') == 1;	//	V+ Vendor Receipt, C+ Customer Return
            MVAFClient client = MVAFClient.Get(GetCtx());

            bool needSave = false;
            MVAMProduct product = line.GetProduct();

            //	Need to have Location
            if (product != null
                && line.GetVAM_Locator_ID() == 0)
            {
                line.SetVAM_Warehouse_ID(GetVAM_Warehouse_ID());
                line.SetVAM_Locator_ID(inTrx ? Env.ZERO : line.GetMovementQty());	//	default Locator
                needSave = true;
            }

            if (product != null)
            {
                if (GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReceipts) == 0 && line.GetMovementQty() > 0)
                {
                    #region VendorReceipts
                    Decimal qtyToReceive = line.GetMovementQty();
                    //auto balance negative on hand when container functionality applicable
                    if (isContainrApplicable)
                    {
                        qtyToReceive = autoBalanceNegative(line, product, line.GetMovementQty());
                    }

                    //Allocate remaining qty.
                    if (qtyToReceive.CompareTo(Env.ZERO) > 0)
                    {
                        MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line, line.GetVAM_PFeature_SetInstance_ID(), qtyToReceive, GetMovementDate());
                        if (!ma.Save(line.Get_Trx()))
                        {
                            // Handle exception
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (!String.IsNullOrEmpty(pp.GetName()))
                                throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                            else
                                throw new ArgumentException("Attribute Tab not saved");
                        }
                    }
                    #endregion
                }
                else if (GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_CustomerReturns) == 0)
                {
                    #region CustomerReturns
                    Decimal qtyToReturn = line.GetMovementQty();
                    if (isContainrApplicable)
                    {
                        qtyToReturn = autoBalanceNegative(line, product, line.GetMovementQty());
                    }

                    if (line.GetVAB_OrderLine_ID() != 0 && qtyToReturn.CompareTo(Env.ZERO) > 0)
                    {
                        {
                            //Linking to shipment line
                            MVABOrderLine rmaLine = new MVABOrderLine(GetCtx(), line.GetVAB_OrderLine_ID(), Get_Trx());
                            if (rmaLine.GetOrig_InOutLine_ID() > 0)
                            {
                                //retrieving ASI which is not already returned
                                MVAMInvInOutLineMP[] shipmentMAS = MVAMInvInOutLineMP.getNonReturned(GetCtx(), rmaLine.GetOrig_InOutLine_ID(), Get_Trx());

                                for (int ii = 0; ii < shipmentMAS.Length; ii++)
                                {
                                    MVAMInvInOutLineMP sMA = shipmentMAS[ii];
                                    Decimal lineMAQty = sMA.GetMovementQty();
                                    if (lineMAQty.CompareTo(qtyToReturn) > 0)
                                    {
                                        lineMAQty = qtyToReturn;
                                    }

                                    MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line, sMA.GetVAM_PFeature_SetInstance_ID(), lineMAQty, sMA.GetMMPolicyDate());
                                    if (!ma.Save(line.Get_Trx()))
                                    {
                                        // Handle exception
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (!String.IsNullOrEmpty(pp.GetName()))
                                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                                        else
                                            throw new ArgumentException("Attribute Tab not saved");
                                    }

                                    qtyToReturn = Decimal.Subtract(qtyToReturn, lineMAQty);
                                    if (qtyToReturn.CompareTo(Env.ZERO) == 0)
                                        break;
                                }
                            }
                        }
                        if (qtyToReturn.CompareTo(Env.ZERO) > 0)
                        {
                            //Use movement data for  Material policy if no linkage found to Shipment.
                            MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line, line.GetVAM_PFeature_SetInstance_ID(), qtyToReturn, GetMovementDate());
                            if (ma.Save(line.Get_Trx()))
                            {
                                // Handle exception
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (!String.IsNullOrEmpty(pp.GetName()))
                                    throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                                else
                                    throw new ArgumentException("Attribute Tab not saved");
                            }
                        }
                    }
                    #endregion
                }
                else if (GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReturns) == 0 && line.GetVAB_OrderLine_ID() != 0)
                {
                    #region Vendor Return with respect to PO
                    Decimal qtyToReturn = line.GetMovementQty();
                    if (qtyToReturn.CompareTo(Env.ZERO) > 0)
                    {
                        {
                            //Linking to MR line
                            MVABOrderLine rmaLine = new MVABOrderLine(GetCtx(), line.GetVAB_OrderLine_ID(), Get_Trx());
                            if (rmaLine.GetOrig_InOutLine_ID() > 0)
                            {
                                //retrieving ASI which is not already returned
                                MVAMInvInOutLineMP[] shipmentMAS = MVAMInvInOutLineMP.getNonReturned(GetCtx(), rmaLine.GetOrig_InOutLine_ID(), Get_Trx());

                                for (int ii = 0; ii < shipmentMAS.Length; ii++)
                                {
                                    MVAMInvInOutLineMP sMA = shipmentMAS[ii];
                                    Decimal lineMAQty = sMA.GetMovementQty();
                                    if (lineMAQty.CompareTo(qtyToReturn) > 0)
                                    {
                                        lineMAQty = qtyToReturn;
                                    }

                                    MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line, sMA.GetVAM_PFeature_SetInstance_ID(), lineMAQty, sMA.GetMMPolicyDate());
                                    if (!ma.Save(line.Get_Trx()))
                                    {
                                        // Handle exception
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (!String.IsNullOrEmpty(pp.GetName()))
                                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                                        else
                                            throw new ArgumentException("Attribute Tab not saved");
                                    }

                                    qtyToReturn = Decimal.Subtract(qtyToReturn, lineMAQty);
                                    if (qtyToReturn.CompareTo(Env.ZERO) == 0)
                                        break;
                                }
                            }
                        }
                        if (qtyToReturn.CompareTo(Env.ZERO) > 0)
                        {
                            //Use movement data for  Material policy if no linkage found to MR.
                            MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line, line.GetVAM_PFeature_SetInstance_ID(), qtyToReturn, GetMovementDate());
                            if (ma.Save(line.Get_Trx()))
                            {
                                // Handle exception
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (!String.IsNullOrEmpty(pp.GetName()))
                                    throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                                else
                                    throw new ArgumentException("Attribute Tab not saved");
                            }
                        }
                    }
                    #endregion
                }
                // Create - consume the Product using policy FIFO/LIFO
                else if (GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReturns) == 0 ||
                         GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_CustomerShipment) == 0 ||
                    GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReceipts) == 0 && line.GetMovementQty() < 0)
                {
                    #region VendorReturns / CustomerShipment
                    //bool isLifoChecked = false;
                    dynamic[] storages = null;

                    // Is Used to get Material Policy
                    MVAMProductCategory pc = MVAMProductCategory.Get(GetCtx(), product.GetVAM_ProductCategory_ID());
                    String MMPolicy = pc.GetMMPolicy();

                    // Is used for handling Gurantee Date - In Case of ASI
                    //DateTime? minGuaranteeDate = GetMovementDate();

                    // Get Data from Container Storage based on Policy
                    if (isContainrApplicable)
                    {
                        storages = MVAMProductContainer.GetContainerStorage(GetCtx(), 0, line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID(),
                      line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
                      line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                      MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), false, Get_TrxName());
                    }
                    else
                    {
                        storages = MVAMStorage.GetWarehouse(GetCtx(), GetVAM_Warehouse_ID(), line.GetVAM_Product_ID(),
                            line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
                               line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                               MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                    }

                    // qty which is need to handle
                    // In Case of Vendor Receipt - need to negate movement qty
                    Decimal qtyToDeliver = GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReceipts) == 0 ? Decimal.Negate(line.GetMovementQty()) : line.GetMovementQty();

                    //LIFOManage:
                    // Create Attribute Tab entry 
                    for (int ii = 0; ii < storages.Length; ii++)
                    {
                        dynamic storage = storages[ii];

                        // when storage qty is less than equal to ZERO then continue to other record 
                        if ((isContainrApplicable ? storage.GetQty() : storage.GetQtyOnHand()) <= 0)
                            continue;

                        if ((isContainrApplicable ? storage.GetQty() : storage.GetQtyOnHand()).CompareTo(qtyToDeliver) >= 0)
                        {
                            MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line,
                                    storage.GetVAM_PFeature_SetInstance_ID(),
                                    GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReceipts) == 0 ? Decimal.Negate(qtyToDeliver) : qtyToDeliver,
                                    isContainrApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
                            //ma.SetIsPhysicalInventory(storage.IsPhysicalInventory());
                            if (!ma.Save(line.Get_Trx()))
                            {
                                // Handle exception
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (!String.IsNullOrEmpty(pp.GetName()))
                                    throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                                else
                                    throw new ArgumentException("Attribute Tab not saved");
                            }
                            qtyToDeliver = Env.ZERO;
                        }
                        else
                        {
                            log.Config("Split - " + line);
                            MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line,
                                    storage.GetVAM_PFeature_SetInstance_ID(),
                                    GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReceipts) == 0 ?
                                    Decimal.Negate(isContainrApplicable ? storage.GetQty() : storage.GetQtyOnHand()) :
                                    (isContainrApplicable ? storage.GetQty() : storage.GetQtyOnHand()),
                                    isContainrApplicable ? storage.GetMMPolicyDate() : GetMovementDate());
                            //ma.SetIsPhysicalInventory(storage.IsPhysicalInventory());
                            if (!ma.Save(line.Get_Trx()))
                            {
                                // Handle exception
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (!String.IsNullOrEmpty(pp.GetName()))
                                    throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                                else
                                    throw new ArgumentException("Attribute Tab not saved");
                            }
                            qtyToDeliver = Decimal.Subtract(qtyToDeliver, (isContainrApplicable ? storage.GetQty() : storage.GetQtyOnHand()));
                            log.Fine("#" + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                        }

                        if (qtyToDeliver == 0)
                            break;

                        if (isContainrApplicable && ii == storages.Length - 1 && !MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy))
                        {
                            storages = MVAMProductContainer.GetContainerStorage(GetCtx(), 0, line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID(),
                                         line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
                                         line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                                         MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
                            ii = -1;
                        }
                    }

                    //if (isContainrApplicable && !MClient.MMPOLICY_FiFo.Equals(MMPolicy) && !isLifoChecked && qtyToDeliver != 0)
                    //{
                    //    isLifoChecked = true;
                    //    // Get Data from Container Storage based on Policy
                    //    storages = MVAMProductContainer.GetContainerStorage(GetCtx(), 0, line.GetVAM_Locator_ID(), line.GetVAM_ProductContainer_ID(),
                    //  line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(), product.GetVAM_PFeature_Set_ID(),
                    //  line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                    //  MClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
                    //    goto LIFOManage;
                    //}

                    // In Case of Over Delivery, then did entry on Attribute Tab with remaning qty
                    if (qtyToDeliver != 0)
                    {
                        //Over Delivery
                        MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line, line.GetVAM_PFeature_SetInstance_ID(),
                            GetMovementType().CompareTo(MVAMInvInOut.MOVEMENTTYPE_VendorReceipts) == 0 ? Decimal.Negate(qtyToDeliver) : qtyToDeliver, GetMovementDate());
                        if (!ma.Save(line.Get_Trx()))
                        {
                            // Handle exception
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (!String.IsNullOrEmpty(pp.GetName()))
                                throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                            else
                                throw new ArgumentException("Attribute Tab not saved");
                        }
                        log.Fine("##: " + ma);
                    }
                    #endregion
                }
            }

            if (needSave && !line.Save())
            {
                log.Severe("NOT saved " + line);
            }
        }

        /// <summary>
        /// Is used to make Container Storage Record Qty as Positive when we Receive Qty
        /// </summary>
        /// <param name="line"></param>
        /// <param name="product"></param>
        /// <param name="qtyToReceive"></param>
        /// <returns></returns>
        protected Decimal autoBalanceNegative(MVAMInvInOutLine line, MVAMProduct product, Decimal qtyToReceive)
        {
            // Is Used to get Material Policy
            MVAMProductCategory pc = MVAMProductCategory.Get(GetCtx(), product.GetVAM_ProductCategory_ID());

            // Is Used to get all Negative records from Contaner Storage
            X_VAM_ContainerStorage[] storages = MVAMProductContainer.GetContainerStorageNegative(GetCtx(), GetVAM_Warehouse_ID(), line.GetVAM_Locator_ID(),
                                              line.GetVAM_ProductContainer_ID(), line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                    null, MVAFClient.MMPOLICY_FiFo.Equals(pc.GetMMPolicy()), Get_Trx());

            DateTime? dateMPolicy = null;

            for (int ii = 0; ii < storages.Length; ii++)
            {
                X_VAM_ContainerStorage storage = storages[ii];
                if (storage.GetQty() < 0 && qtyToReceive.CompareTo(Env.ZERO) > 0)
                {
                    dateMPolicy = storage.GetMMPolicyDate();
                    Decimal lineMAQty = qtyToReceive;
                    if (lineMAQty.CompareTo(Decimal.Negate(storage.GetQty())) > 0)
                        lineMAQty = Decimal.Negate(storage.GetQty());

                    //Using ASI from storage record
                    MVAMInvInOutLineMP ma = MVAMInvInOutLineMP.GetOrCreate(line, storage.GetVAM_PFeature_SetInstance_ID(), lineMAQty, dateMPolicy);
                    //ma.SetIsPhysicalInventory(storage.IsPhysicalInventory());
                    if (!ma.Save(line.Get_Trx()))
                    {
                        // Exception need to handle
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            throw new ArgumentException("Attribute Tab not saved. " + pp.GetName());
                        else
                            throw new ArgumentException("Attribute Tab not saved");
                    }
                    qtyToReceive = Decimal.Subtract(qtyToReceive, lineMAQty);
                }
            }
            return qtyToReceive;
        }

        /// <summary>
        /// Create Counter Document
        /// </summary>
        /// <returns>InOut</returns>
        private MVAMInvInOut CreateCounterDoc()
        {
            //	Is this a counter doc ?
            if (GetRef_InOut_ID() != 0)
                return null;

            //	Org Must be linked to BPartner
            MVAFOrg org = MVAFOrg.Get(GetCtx(), GetVAF_Org_ID());
            //jz int counterVAB_BusinessPartner_ID = org.getLinkedVAB_BusinessPartner_ID(get_TrxName()); 
            int counterVAB_BusinessPartner_ID = org.GetLinkedVAB_BusinessPartner_ID(Get_TrxName());
            if (counterVAB_BusinessPartner_ID == 0)
                return null;
            //	Business Partner needs to be linked to Org
            //jz MBPartner bp = new MBPartner (getCtx(), getVAB_BusinessPartner_ID(), null);
            MVABBusinessPartner bp = new MVABBusinessPartner(GetCtx(), GetVAB_BusinessPartner_ID(), Get_TrxName());
            int counterVAF_Org_ID = bp.GetVAF_OrgBP_ID_Int();
            if (counterVAF_Org_ID == 0)
                return null;

            //jz MBPartner counterBP = new MBPartner (getCtx(), counterVAB_BusinessPartner_ID, null);
            MVABBusinessPartner counterBP = new MVABBusinessPartner(GetCtx(), counterVAB_BusinessPartner_ID, Get_TrxName());
            MVAFOrgDetail counterOrgInfo = MVAFOrgDetail.Get(GetCtx(), counterVAF_Org_ID, null);
            log.Info("Counter BP=" + counterBP.GetName());

            //	Document Type
            int VAB_DocTypesTarget_ID = 0;
            bool isReturnTrx = false;
            MVABInterCompanyDoc counterDT = MVABInterCompanyDoc.GetCounterDocType(GetCtx(), GetVAB_DocTypes_ID());
            if (counterDT != null)
            {
                log.Fine(counterDT.ToString());
                if (!counterDT.IsCreateCounter() || !counterDT.IsValid())
                    return null;
                VAB_DocTypesTarget_ID = counterDT.GetCounter_VAB_DocTypes_ID();
                isReturnTrx = counterDT.GetCounterDocType().IsReturnTrx();
            }
            if (VAB_DocTypesTarget_ID <= 0)
                return null;

            // ReversalDoc_ID --> contain reference of Orignal Document which is to be reversed
            // Ref_InOut_ID --> contain reference of counter document which is to be created against this document
            // when we reverse document, and if counter document is created agaisnt its orignal document then need to reverse that document also
            if (Get_ColumnIndex("ReversalDoc_ID") > 0 && GetReversalDoc_ID() > 0)
            {
                int counterOrderId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Ref_InOut_ID FROM VAM_Inv_InOut WHERE VAM_Inv_InOut_ID = " + GetReversalDoc_ID(), null, Get_Trx()));
                MVAMInvInOut counterReversed = new MVAMInvInOut(GetCtx(), counterOrderId, Get_Trx());
                if (counterReversed != null && counterReversed.GetVAM_Inv_InOut_ID() > 0)
                {
                    counterReversed.SetDocAction(DOCACTION_Void);
                    counterReversed.ProcessIt(DOCACTION_Void);
                    counterReversed.Save(Get_Trx());
                    return counterReversed;
                }
                return null;
            }

            //	Deep Copy
            MVAMInvInOut counter = CopyFrom(this, GetMovementDate(),
                VAB_DocTypesTarget_ID, !IsSOTrx(), isReturnTrx, true, Get_TrxName(), true);

            //
            counter.SetVAF_Org_ID(counterVAF_Org_ID);
            counter.SetVAM_Warehouse_ID(counterOrgInfo.GetVAM_Warehouse_ID());
            //
            counter.SetBPartner(counterBP);
            //	Refernces (Should not be required
            counter.SetSalesRep_ID(GetSalesRep_ID());
            //
            counter.SetProcessing(false);
            counter.Save(Get_TrxName());

            // Create Lines
            if (counter.CopyLinesFrom(this, true, true) == 0)
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    counter._processMsg = Msg.GetMsg(GetCtx(), "CouldNotCreateShipLines") + "," + pp.GetName();
                else
                    counter._processMsg = Msg.GetMsg(GetCtx(), "CouldNotCreateShipLines");
                counter = null;
                throw new Exception(counter._processMsg);
            }

            string MovementType = counter.GetMovementType();
            //bool inTrx = MovementType.charAt(1) == '+';	//	V+ Vendor Receipt
            bool inTrx = MovementType.IndexOf('+') == 1;	//	V+ Vendor Receipt

            //	Update copied lines
            MVAMInvInOutLine[] counterLines = counter.GetLines(true);
            for (int i = 0; i < counterLines.Length; i++)
            {
                MVAMInvInOutLine counterLine = counterLines[i];
                counterLine.SetClientOrg(counter);
                counterLine.SetVAM_Warehouse_ID(counter.GetVAM_Warehouse_ID());
                counterLine.SetVAM_Locator_ID(0);
                int locatorId = 0;
                if (!inTrx)
                {
                    locatorId = MVAMStorage.GetVAM_Locator_ID(counter.GetVAM_Warehouse_ID(), counterLine.GetVAM_Product_ID(), counterLine.GetVAM_PFeature_SetInstance_ID(),
                                                            counterLine.GetMovementQty(), Get_TrxName());
                }
                else
                {
                    locatorId = MWarehouse.Get(GetCtx(), counter.GetVAM_Warehouse_ID()).GetDefaultVAM_Locator_ID();
                }
                //counterLine.SetVAM_Locator_ID(Convert.ToInt32(inTrx ? Env.ZERO : counterLine.GetMovementQty()));
                counterLine.SetVAM_Locator_ID(locatorId);
                //
                counterLine.Save(Get_TrxName());
            }
            log.Fine(counter.ToString());
            //	Document Action
            if (counterDT != null)
            {
                if (counterDT.GetDocAction() != null)
                {
                    counter.SetDocAction(counterDT.GetDocAction());
                    counter.ProcessIt(counterDT.GetDocAction());
                    counter.Save(Get_TrxName());
                }
            }
            return counter;
        }

        /// <summary>
        /// 	Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public virtual bool VoidIt()
        {
            // int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA038_' "));
            int Asset_ID = 0;
            log.Info(ToString());

            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }


            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                //	Set lines to 0
                MVAMInvInOutLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    Asset_ID = 0;
                    MVAMInvInOutLine line = lines[i];
                    Decimal old = line.GetMovementQty();
                    if (old != 0)
                    {
                        line.SetQty(Env.ZERO);
                        line.AddDescription("Void (" + old + ")");
                        line.Save(Get_TrxName());
                        //if (countVA038 > 0)
                        //{
                        //    Asset_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAA_Asset_ID FROM VAA_Asset WHERE VAM_Inv_InOutLine_ID=" + line.GetVAM_Inv_InOutLine_ID()));
                        //    if (Asset_ID > 0)
                        //    {
                        //        MAsset Asset = new MAsset(GetCtx(), Asset_ID, Get_TrxName());
                        //        Asset.SetIsActive(false);
                        //        if (Asset.Save())
                        //        {
                        //            log.Fine("Asset Deacticated" + Asset.GetA_Asset_ID());
                        //        }
                        //    }

                        //}

                    }
                }
                //Arpit to set void the Confirmation if confirmation is there
                MVAMInvInOutConfirm[] confirmations = GetConfirmations(false);
                if (confirmations.Length > 0)
                {
                    for (Int32 i = 0; i < confirmations.Length; i++)
                    {
                        if (confirmations[i].GetDocStatus() == DOCSTATUS_Drafted)
                        {
                            confirmations[i].VoidIt();
                            confirmations[i].SetDocAction(DOCACTION_None);
                            confirmations[i].SetDocStatus(DOCSTATUS_Voided);
                            confirmations[i].SetProcessed(true);
                            confirmations[i].Save(Get_TrxName());
                            break;
                        }
                    }
                }
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Close Document.
        /// </summary>
        /// <returns>true if success</returns>
        public virtual bool CloseIt()
        {
            log.Info(ToString());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Is Only For Order
        /// </summary>
        /// <param name="order">order</param>
        /// <returns>true if all shipment lines are from this order</returns>
        public bool IsOnlyForOrder(MVABOrder order)
        {
            //	TODO Compare Lines
            return GetVAB_Order_ID() == order.GetVAB_Order_ID();
        }

        /// <summary>
        /// Reverse Correction - same date
        /// </summary>
        /// <param name="order">if not null only for this order</param>
        /// <returns>true if success </returns>
        public bool ReverseCorrectIt(MVABOrder order)
        {
            log.Info(ToString());
            string reversedDocno = null;
            string ss = ToString();
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            if (!MVABYearPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetVAF_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MVABNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetVAF_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return false;
            }

            MVABOrder ord = new MVABOrder(GetCtx(), GetVAB_Order_ID(), Get_Trx());
            MVABDocTypes dtOrder = MVABDocTypes.Get(GetCtx(), ord.GetVAB_DocTypes_ID());
            String DocSubTypeSO = dtOrder.GetDocSubTypeSO();

            // if any linked record exist on invoice for PO/SO cycle then not able to reverse this record
            if (MVABDocTypes.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)	//	(W)illCall(I)nvoice
                    || MVABDocTypes.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                    || MVABDocTypes.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))			//	(W)alkIn(R)eceipt
            {
                // when we void SO then system void all transaction which is linked with that inout
            }
            else
            {
                if (!linkedDocumentAgainstInOut(GetVAM_Inv_InOut_ID()))
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "LinkedDocStatus");
                    return false;
                }


                // Added by Vivek on 08/11/2017 assigned by Mukesh sir
                // return false if linked document is in completed or closed stage
                // stuck case for this -- Order1 --> M1 & MR2  --> create invoice against M1 and try to reverse MR2, system not allowing by this check
                // linked doc check above, so not used this function
                //if (!linkedDocument(GetVAB_Order_ID()))
                //{
                //    _processMsg = Msg.GetMsg(GetCtx(), "LinkedDocStatus");
                //    return false;
                //}
            }
            //Added by Vivek assigned by Pradeep on 27/09/2017
            // Stopped voiding manually for drop ship type of shipment
            if (IsSOTrx() && !IsReturnTrx() && IsDropShip())
            {
                MVAMInvInOut _inout = new MVAMInvInOut(GetCtx(), GetRef_ShipMR_ID(), Get_TrxName());
                if (_inout.GetDocStatus() != "VO" && _inout.GetDocStatus() != "RE")
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_ShipmentRevStop");
                    return false;
                }
            }

            //	Reverse/Delete Matching
            if (!IsSOTrx())
            {
                MVAMMatchInvoice[] mInv = MVAMMatchInvoice.GetInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
                for (int i = 0; i < mInv.Length; i++)
                    mInv[i].Delete(true);
                MVAMMatchPO[] mPO = MVAMMatchPO.GetInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
                for (int i = 0; i < mPO.Length; i++)
                {
                    if (mPO[i].GetVAB_InvoiceLine_ID() == 0)
                        mPO[i].Delete(true);
                    else
                    {
                        mPO[i].SetVAM_Inv_InOutLine_ID(0);
                        mPO[i].Save();
                    }
                }
            }

            //	Deep Copy
            MVAMInvInOut reversal = CopyFrom(this, GetMovementDate(),
                GetVAB_DocTypes_ID(), IsSOTrx(), dt.IsReturnTrx(), false, Get_TrxName(), true);
            if (reversal.Get_ColumnIndex("IsFutureCostCalculated") > 0)
            {
                reversal.SetIsFutureCostCalculated(false);
            }
            if (reversal.Get_ColumnIndex("ReversalDoc_ID") > 0)
            {
                reversal.SetReversalDoc_ID(GetVAM_Inv_InOut_ID());
            }
            if (reversal.Get_ColumnIndex("IsReversal") > 0)
            {
                reversal.SetIsReversal(true);
            }

            //Set DateAccount as orignal document
            reversal.SetMovementDate(GetMovementDate());

            if (!reversal.Save(Get_TrxName()))
            {
                pp = VLogger.RetrieveError();
                if (!String.IsNullOrEmpty(pp.GetName()))
                    _processMsg = "Could not create Ship Reversal , " + pp.GetName();
                else
                    _processMsg = "Could not create Ship Reversal";
                return false;
            }
            if (reversal == null)
            {
                _processMsg = "Could not create Ship Reversal";
                return false;
            }

            // Added by Vivek on 03/10/2017 Assigned by Pradeep
            // get documentno for saving reversal documentno on new document
            reversedDocno = reversal.GetDocumentNo();

            //	Reverse Line Qty
            MVAMInvInOutLine[] sLines = GetLines(false);
            MVAMInvInOutLine[] rLines = reversal.GetLines(false);
            for (int i = 0; i < rLines.Length; i++)
            {
                MVAMInvInOutLine rLine = rLines[i];
                //rLine.SetQtyEntered(Decimal.Negate(rLine.GetQtyEntered()));
                //rLine.SetMovementQty(Decimal.Negate(rLine.GetMovementQty()));
                rLine.SetVAM_PFeature_SetInstance_ID(sLines[i].GetVAM_PFeature_SetInstance_ID());
                if (rLine.Get_ColumnIndex("IsFutureCostCalculated") > 0)
                {
                    rLine.SetIsFutureCostCalculated(false);
                }
                if (!rLine.Save(Get_TrxName()))
                {
                    pp = VLogger.RetrieveError();
                    if (!String.IsNullOrEmpty(pp.GetName()))
                        _processMsg = "Could not create Ship Reversal Line , " + pp.GetName();
                    else
                        _processMsg = "Could not create Ship Reversal Line";
                    return false;
                }
                //	We need to copy MA (bcz want to copy of material policy line from the actual record)
                MVAMInvInOutLineMP[] mas = MVAMInvInOutLineMP.Get(GetCtx(), rLine.GetReversalDoc_ID(), Get_TrxName());
                for (int j = 0; j < mas.Length; j++)
                {
                    MVAMInvInOutLineMP ma = new MVAMInvInOutLineMP(rLine, mas[j].GetVAM_PFeature_SetInstance_ID(), Decimal.Negate(mas[j].GetMovementQty()), mas[j].GetMMPolicyDate());
                    if (!ma.Save(rLine.Get_Trx()))
                    {
                        pp = VLogger.RetrieveError();
                        if (!String.IsNullOrEmpty(pp.GetName()))
                            _processMsg = "Could not create Reversal Attribute , " + pp.GetName();
                        else
                            _processMsg = "Could not create Reversal Attribute";
                        return false;
                    }
                }
                //	De-Activate Asset 

                List<MVAAsset> asset = MVAAsset.GetFromShipment(GetCtx(), sLines[i].GetVAM_Inv_InOutLine_ID(), Get_TrxName());
                foreach (MVAAsset ass in asset)
                {
                    ass.SetIsActive(false);
                    ass.AddDescription("(" + reversal.GetDocumentNo() + " #" + rLine.GetLine() + "<-)");
                    ass.Save();
                }
            }
            reversal.SetVAB_Order_ID(GetVAB_Order_ID());
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            reversal.Save(Get_TrxName());
            //
            // Added by Amit 1-8-2015 VAMRP
            //if (Env.HasModulePrefix("VAMRP_", out mInfo))
            //{
            //    if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE, set)
            //        || !reversal.GetDocStatus().Equals(DocActionVariables.STATUS_COMPLETED))
            //    {
            //        _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
            //        return false;
            //    }
            //}
            //else
            //{
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE)
                || !reversal.GetDocStatus().Equals(DocActionVariables.STATUS_COMPLETED))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            //}
            reversal.CloseIt();
            reversal.SetProcessing(false);

            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            //reversal.Save(Get_TrxName()); //commented by Arpit
            //Arpit 15 Nov,2017 To void the confirmation when we reverse/correct the document
            if (reversal.Save(Get_TrxName()))
            {
                //Make void --confirmation void
                MVAMInvInOutConfirm[] confirmations = GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MVAMInvInOutConfirm confirm = confirmations[i];
                    if (confirm.GetDocStatus() == DOCSTATUS_Completed)
                    {
                        if (confirm.GetVAM_Inventory_ID() > 0)
                        {   //For inventory reversal 
                            MVAMInventory _Inventory = new MVAMInventory(GetCtx(), confirm.GetVAM_Inventory_ID(), Get_TrxName());

                            //JID_1163: If Internal Use is void and we try to void  Material Recipt System will void Material Recipt and Confirmation
                            if (_Inventory.GetDocStatus() != DOCSTATUS_Voided && _Inventory.GetDocStatus() != DOCSTATUS_Reversed)
                            {
                                if (!_Inventory.VoidIt())
                                {
                                    _processMsg = "Reversal ERROR: " + _Inventory.GetProcessMsg();
                                    return false;
                                }
                                //JID_0125_1: When we reverse MR Internal Use Inventory reverse document status is not set as reversed.
                                else if (!_Inventory.Save())
                                {
                                    pp = VLogger.RetrieveError();
                                    if (!String.IsNullOrEmpty(pp.GetName()))
                                        _processMsg = Msg.GetMsg(GetCtx(), "NotSavedInventory") + pp.GetName();
                                    else
                                        _processMsg = Msg.GetMsg(GetCtx(), "NotSavedInventory");
                                    return false;
                                }
                            }
                        }
                        if (!confirm.VoidIt())
                        {
                            _processMsg = "Reversal ERROR: " + confirm.GetProcessMsg();
                            return false;
                        }
                        break;
                    }
                }
            }
            //End here
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");

            //JID_0889: show on void full message Reversal Document created
            _processMsg = Msg.GetMsg(GetCtx(), "VIS_DocumentReversed") + reversal.GetDocumentNo();
            SetProcessed(true);

            SetDocStatus(DOCSTATUS_Reversed);		//	 may come from void
            SetDocAction(DOCACTION_None);

            //Added by Vivek assigned by Pradeep on 27/09/2017
            // if material receipt is reversed then shipment should also  reverse
            if (!IsSOTrx() && !IsReturnTrx() && IsDropShip())
            {
                // Not getting DocAction and Docstatus values during reversal in case of shipment reversal
                Save(Get_TrxName());
                MVAMInvInOut ino = new MVAMInvInOut(GetCtx(), GetRef_ShipMR_ID(), Get_Trx());
                ino.VoidIt();
                ino.SetProcessed(true);
                ino.SetDocStatus(DOCSTATUS_Reversed);
                ino.SetDocAction(DOCACTION_None);
                ino.Save();
            }
            return true;
        }

        /// <summary>
        /// Reverse Correction - same date
        /// </summary>
        /// <returns>true if success </returns>
        public bool ReverseCorrectIt()
        {
            return ReverseCorrectIt(null);
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public virtual bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            sb.Append(":")
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>VAF_UserContact_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetSalesRep_ID();
        }

        /// <summary>
        /// Get Document Approval Amount
        /// </summary>
        /// <returns>amount</returns>
        public Decimal GetApprovalAmt()
        {
            return Env.ZERO;
        }

        /// <summary>
        /// Get VAB_Currency_ID
        /// </summary>
        /// <returns>Accounting Currency</returns>
        public int GetVAB_Currency_ID()
        {
            return GetCtx().GetContextAsInt("$VAB_Currency_ID ");
        }

        /// <summary>
        /// Document Status is Complete or Closed
        /// </summary>
        /// <returns>true if CO, CL or RE</returns>
        public bool IsComplete()
        {
            String ds = GetDocStatus();
            return DOCSTATUS_Completed.Equals(ds)
                || DOCSTATUS_Closed.Equals(ds)
                || DOCSTATUS_Reversed.Equals(ds);
        }
        /// <summary>
        /// Get Linekd Document for the Order
        /// </summary>
        /// <param name="VAB_Order_ID"></param>
        /// <returns>True/False</returns>
        private bool linkedDocument(int VAB_Order_ID)
        {
            string sql = "SELECT COUNT(VAB_Order_ID) FROM VAB_Invoice WHERE VAB_Order_ID = " + VAB_Order_ID + "  AND DocStatus NOT IN ('RE','VO')";
            int _countOrder = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            if (_countOrder > 0)
            {
                return false;
            }
            return true;
        }

        // checked VAM_Inv_InOut reference is available on VAB_Invoice then not to reverse this VAM_Inv_InOut
        private bool linkedDocumentAgainstInOut(int VAM_Inv_InOut_ID)
        {
            // JID_1310: At the time of void MR need to check if any RMA and Custmer/Vendor RMA has MR refrence. If found need to give message
            string sql = @"SELECT SUM(Result) From (
                          SELECT COUNT(il.VAM_Inv_InOutLine_ID) AS Result FROM VAB_Invoice i INNER JOIN VAB_InvoiceLine il ON i.VAB_Invoice_ID = il.VAB_Invoice_ID
                          WHERE il.VAM_Inv_InOutLine_ID = (SELECT VAM_Inv_InOutLine_ID FROM VAM_Inv_InOutLine mil WHERE mil.VAM_Inv_InOut_ID = " + VAM_Inv_InOut_ID + @"
                          AND mil.VAM_Inv_InOutLine_ID = il.VAM_Inv_InOutLine_ID) AND il.IsActive = 'Y' AND i.DocStatus NOT IN ('RE' , 'VO')
                          UNION ALL
                          SELECT COUNT(o.VAB_Order_ID) AS Result FROM VAB_Order o WHERE o.Orig_InOut_ID = " + VAM_Inv_InOut_ID + @" AND o.DocStatus NOT IN ('RE' , 'VO')) t";
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql)) > 0)
            {
                return false;
            }
            return true;
        }


        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }


        public void SetProcessMsg(string processMsg)
        {
            _processMsg = processMsg;
        }



        #endregion
        //Added By Arpit on 9th Nov,2017
        //Description-: To freeze Lines and current document while creating document in Inprogress while generating shipment with Confirmation */
        public void FreezeDoc()
        {
            try
            {
                if (GetDocStatus() == DOCSTATUS_InProgress)
                {
                    MVAMInvInOutLine[] lines = GetLines();
                    for (Int32 i = 0; i < lines.Length; i++)
                    {
                        lines[i].SetProcessed(true);
                        if (!lines[i].Save(Get_TrxName()))
                        {
                            log.Severe("Error in generating shipment  - ");
                        }
                    }
                    SetProcessed(true);
                    //can not set document action to void only because in this case we have to update in VIS instead of model library
                    if (!Save(Get_TrxName()))
                    {
                        log.Severe("Error in generating shipment  - ");
                    }
                }
            }
            catch (Exception e)
            {
                log.Severe("Error in generating shipment  - " + e.Message);
            }
            //Arpit
        }

    }
}
