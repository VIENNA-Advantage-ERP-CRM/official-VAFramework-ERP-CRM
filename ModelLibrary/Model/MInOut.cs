
/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInOut
 * Purpose        : Class linked with the shipment,invoice window
 * Class Used     : X_M_InOut, DocAction
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;

using System.Data.SqlClient;
using System.Net.Http.Headers;
using com.sun.xml.@internal.bind.v2.schemagen.xmlschema;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Model
{
    public class MInOut : X_M_InOut, DocAction
    {
        #region variable
        //	Process Message 			
        private String _processMsg = null;
        //	Just Prepared Flag			
        private bool _justPrepared = false;
        //	Lines					
        private MInOutLine[] _lines = null;
        // Confirmations			
        private MInOutConfirm[] _confirms = null;
        // BPartner				
        private MBPartner _partner = null;
        // Reversal Flag		
        public bool _reversal = false;
        private bool set = false;

        private string sql = "";
        private Decimal? trxQty = 0;
        private bool isGetFromStorage = false;

        MOrderLine orderLine = null;
        MProduct productCQ = null;
        decimal amt = 0;
        decimal currentCostPrice = 0;
        string conversionNotFoundInOut = "";
        string conversionNotFoundInOut1 = "";
        string conversionNotFoundInvoice = "";

        private static VLogger _log = VLogger.GetVLogger(typeof(MInOut).FullName);

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
        /// <param name="M_InOut_ID"></param>
        /// <param name="trxName">rx name</param>
        public MInOut(Ctx ctx, int M_InOut_ID, Trx trxName)
            : base(ctx, M_InOut_ID, trxName)
        {

            if (M_InOut_ID == 0)
            {
                //	setDocumentNo (null);
                //	setC_BPartner_ID (0);
                //	setC_BPartner_Location_ID (0);
                //	setM_Warehouse_ID (0);
                //	setC_DocType_ID (0);
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
        public static MInOut CreateFrom(MOrder order, DateTime? movementDate,
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
            MInOut retValue = new MInOut(order, 0, movementDate);
            retValue.SetDocAction(complete ? DOCACTION_Complete : DOCACTION_Prepare);

            //	Check if we can create the lines
            MOrderLine[] oLines = order.GetLines(true, "M_Product_ID");
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
                MStorage[] storages = null;
                MProduct product = oLines[i].GetProduct();
                if (product != null && product.Get_ID() != 0 && product.IsStocked())
                {
                    MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetM_Product_Category_ID());
                    String MMPolicy = pc.GetMMPolicy();
                    if (MMPolicy == null || MMPolicy.Length == 0)
                    {
                        MClient client = MClient.Get(order.GetCtx());
                        MMPolicy = client.GetMMPolicy();
                    }
                    storages = MStorage.GetWarehouse(order.GetCtx(), order.GetM_Warehouse_ID(),
                        oLines[i].GetM_Product_ID(), oLines[i].GetM_AttributeSetInstance_ID(),
                        product.GetM_AttributeSet_ID(),
                        allAttributeInstances, minGuaranteeDate,
                        MClient.MMPOLICY_FiFo.Equals(MMPolicy), trxName);
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
                    MInOutLine line = new MInOutLine(retValue);
                    line.SetOrderLine(oLines[i], storages[ll].GetM_Locator_ID(),
                        order.IsSOTrx() ? lineQty : Env.ZERO);
                    line.SetQty(lineQty);	//	Correct UOM for QtyEntered
                    if (oLines[i].GetQtyEntered().CompareTo(oLines[i].GetQtyOrdered()) != 0)
                    {
                        //line.SetQtyEntered(lineQty.multiply(oLines[i].getQtyEntered()).divide(oLines[i].getQtyOrdered(), 12, Decimal.ROUND_HALF_UP));
                        line.SetQtyEntered(Decimal.Multiply(lineQty, Decimal.Divide(oLines[i].GetQtyEntered(), Decimal.Round(oLines[i].GetQtyOrdered(), 12, MidpointRounding.AwayFromZero))));
                    }
                    line.SetC_Project_ID(oLines[i].GetC_Project_ID());
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
        public static MInOut CreateShipment(MOrder order, MInOut inout, DateTime? movementDate, bool forceDelivery,
                    bool allAttributeInstances, int M_Warehouse_ID, DateTime? minGuaranteeDate, Trx trxName)
        {
            if (order == null)
            {
                throw new ArgumentException("No Order");
            }

            //	Create Meader
            MInOut retValue = new MInOut(order, 0, movementDate);
            retValue.SetM_Warehouse_ID(M_Warehouse_ID);
            retValue.SetIsDropShip(true);
            //	Check if we can create the lines
            MInOutLine[] iolines = inout.GetLines(false);
            int AttributeSetInstance_ID = 0;
            MClient client = MClient.Get(order.GetCtx());
            for (int i = 0; i < iolines.Length; i++)
            {
                //MOrderLine ol = new MOrderLine(inout.GetCtx(), iolines[i].GetC_OrderLine_ID(), inout.Get_Trx());
                int Ref_OrderLine_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Ref_OrderLine_ID FROM C_OrderLine WHERE C_OrderLine_ID ="
                                    + iolines[i].GetC_OrderLine_ID(), null, inout.Get_Trx()));
                MOrderLine olines = new MOrderLine(inout.GetCtx(), Ref_OrderLine_ID, inout.Get_Trx());
                Decimal qty = olines.GetQtyEntered();
                // Done by Rakesh Kumar 31/Mar/2021 suggested by Mandeep Singh and Bharat Singla
                // When AttributeInstanceId set on Orderline
                if (olines.GetM_AttributeSetInstance_ID() > 0)
                {
                    AttributeSetInstance_ID = olines.GetM_AttributeSetInstance_ID();
                }
                else if (iolines[i].GetM_AttributeSetInstance_ID() > 0)
                {
                    // When AttributeInstanceId set on Material Receipt Line
                    AttributeSetInstance_ID = iolines[i].GetM_AttributeSetInstance_ID();
                }
                //	Nothing to deliver
                if (qty == 0)
                {
                    continue;
                }
                //	Stock Info
                MStorage[] storages = null;
                MProduct product = olines.GetProduct();
                if (product != null && product.Get_ID() != 0 && product.IsStocked())
                {
                    MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetM_Product_Category_ID());
                    String MMPolicy = pc.GetMMPolicy();
                    if (MMPolicy == null || MMPolicy.Length == 0)
                    {
                        MMPolicy = client.GetMMPolicy();
                    }
                    storages = MStorage.GetWarehouse(order.GetCtx(), M_Warehouse_ID,
                        olines.GetM_Product_ID(), AttributeSetInstance_ID,
                        product.GetM_AttributeSet_ID(),
                        allAttributeInstances, minGuaranteeDate,
                        MClient.MMPOLICY_FiFo.Equals(MMPolicy), trxName);
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
                    MInOutLine line = new MInOutLine(retValue);
                    line.SetIsDropShip(true);

                    line.SetOrderLine(olines, storages[ll].GetM_Locator_ID(),
                        order.IsSOTrx() ? lineQty : Env.ZERO);
                    // Done by Rakesh Kumar 30/Mar/2021 suggested by Mandeep Singh and Bharat Singla
                    // Set AttributeInstanceId on Material Receipt Line
                    if (AttributeSetInstance_ID > 0)
                    {
                        line.SetM_AttributeSetInstance_ID(AttributeSetInstance_ID);
                    }
                    line.SetQty(lineQty);	//	Correct UOM for QtyEntered
                    if (olines.GetQtyEntered().CompareTo(olines.GetQtyEntered()) != 0)
                    {
                        //line.SetQtyEntered(lineQty.multiply(olines.getQtyEntered()).divide(olines.getQtyOrdered(), 12, Decimal.ROUND_HALF_UP));
                        line.SetQtyEntered(Decimal.Multiply(lineQty, Decimal.Divide(olines.GetQtyEntered(), Decimal.Round(olines.GetQtyEntered(), 12, MidpointRounding.AwayFromZero))));
                    }
                    line.SetC_Project_ID(olines.GetC_Project_ID());
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
         * 	@param C_DocType_ID doc type
         * 	@param isSOTrx sales order
         * 	@param counter create counter links
         * 	@param trxName trx
         * 	@param setOrder set the order link
         *	@return Shipment
         */
        public static MInOut CopyFrom(MInOut from, DateTime? dateDoc,
            int C_DocType_ID, bool isSOTrx, bool isReturnTrx,
            bool counter, Trx trxName, bool setOrder)
        {
            MInOut to = new MInOut(from.GetCtx(), 0, null);
            to.Set_TrxName(trxName);
            CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            to.Set_ValueNoCheck("M_InOut_ID", I_ZERO);
            to.Set_ValueNoCheck("DocumentNo", null);
            //
            to.SetDocStatus(DOCSTATUS_Drafted);		//	Draft
            to.SetDocAction(DOCACTION_Complete);
            //
            to.SetC_DocType_ID(C_DocType_ID);
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
            to.SetC_Invoice_ID(0);
            to.SetTrackingNo(null);
            to.SetIsInDispute(false);
            //
            to.SetPosted(false);
            to.SetProcessed(false);
            to.SetC_Order_ID(0);	//	Overwritten by setOrder
            if (counter)
            {
                to.SetC_Order_ID(0);
                to.SetRef_InOut_ID(from.GetM_InOut_ID());
                //	Try to find Order/Invoice link
                if (from.GetC_Order_ID() != 0)
                {
                    MOrder peer = new MOrder(from.GetCtx(), from.GetC_Order_ID(), from.Get_TrxName());
                    if (peer.GetRef_Order_ID() != 0)
                        to.SetC_Order_ID(peer.GetRef_Order_ID());
                }
                if (from.GetC_Invoice_ID() != 0)
                {
                    MInvoice peer = new MInvoice(from.GetCtx(), from.GetC_Invoice_ID(), from.Get_TrxName());
                    if (peer.GetRef_Invoice_ID() != 0)
                        to.SetC_Invoice_ID(peer.GetRef_Invoice_ID());
                }
            }
            else
            {
                to.SetRef_InOut_ID(0);
                if (setOrder)
                    to.SetC_Order_ID(from.GetC_Order_ID());
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
                from.SetRef_InOut_ID(to.GetM_InOut_ID());
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
        public MInOut(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Order Constructor - create header only
         *	@param order order
         *	@param movementDate optional movement date (default today)
         *	@param C_DocTypeShipment_ID document type or 0
         */
        public MInOut(MOrder order, int C_DocTypeShipment_ID, DateTime? movementDate)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {

            SetOrder(order);

            if (C_DocTypeShipment_ID == 0)
            {
                C_DocTypeShipment_ID = VAdvantage.Utility.Util.GetValueOfInt(ExecuteQuery.ExecuteScalar("SELECT C_DocTypeShipment_ID FROM C_DocType WHERE C_DocType_ID=" + order.GetC_DocTypeTarget_ID()));
            }
            SetC_DocType_ID(C_DocTypeShipment_ID, true);

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
         *	@param C_DocTypeShipment_ID document type or 0
         *	@param movementDate optional movement date (default today)
         *	@param M_Warehouse_ID warehouse
         */
        public MInOut(MInvoice invoice, int C_DocTypeShipment_ID,
            DateTime? movementDate, int M_Warehouse_ID)
            : this(invoice.GetCtx(), 0, invoice.Get_TrxName())
        {
            SetClientOrg(invoice);
            MOrder ord = new MOrder(GetCtx(), invoice.GetC_Order_ID(), null);
            SetC_BPartner_ID(ord.GetC_BPartner_ID());
            //SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(ord.GetC_BPartner_Location_ID());	//	shipment address
            SetAD_User_ID(ord.GetAD_User_ID());
            //
            SetM_Warehouse_ID(M_Warehouse_ID);
            SetIsSOTrx(invoice.IsSOTrx());
            SetIsReturnTrx(invoice.IsReturnTrx());

            if (!IsReturnTrx())
                SetMovementType(invoice.IsSOTrx() ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
            else
                SetMovementType(invoice.IsSOTrx() ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);

            MOrder order = null;
            if (invoice.GetC_Order_ID() != 0)
                order = new MOrder(invoice.GetCtx(), invoice.GetC_Order_ID(), invoice.Get_TrxName());
            if (C_DocTypeShipment_ID == 0 && order != null)
                C_DocTypeShipment_ID = int.Parse(ExecuteQuery.ExecuteScalar("SELECT C_DocTypeShipment_ID FROM C_DocType WHERE C_DocType_ID=" + order.GetC_DocType_ID()));
            if (C_DocTypeShipment_ID != 0)
                SetC_DocType_ID(C_DocTypeShipment_ID, true);
            else
                SetC_DocType_ID();

            //	Default - Today
            if (movementDate != null)
                SetMovementDate(movementDate);
            SetDateAcct(GetMovementDate());

            //	Copy from Invoice
            SetC_Order_ID(invoice.GetC_Order_ID());
            SetSalesRep_ID(invoice.GetSalesRep_ID());
            //
            SetC_Activity_ID(invoice.GetC_Activity_ID());
            SetC_Campaign_ID(invoice.GetC_Campaign_ID());
            SetC_Charge_ID(invoice.GetC_Charge_ID());
            SetChargeAmt(invoice.GetChargeAmt());
            //
            SetC_Project_ID(invoice.GetC_Project_ID());
            SetDateOrdered(invoice.GetDateOrdered());
            SetDescription(invoice.GetDescription());
            SetPOReference(invoice.GetPOReference());
            SetAD_OrgTrx_ID(invoice.GetAD_OrgTrx_ID());
            SetUser1_ID(invoice.GetUser1_ID());
            SetUser2_ID(invoice.GetUser2_ID());

            if (order != null)
            {
                SetDeliveryRule(order.GetDeliveryRule());
                SetDeliveryViaRule(order.GetDeliveryViaRule());
                SetM_Shipper_ID(order.GetM_Shipper_ID());
                SetFreightCostRule(order.GetFreightCostRule());
                SetFreightAmt(order.GetFreightAmt());
            }
        }

        /**
         * 	Copy Constructor - create header only
         *	@param original original 
         *	@param movementDate optional movement date (default today)
         *	@param C_DocTypeShipment_ID document type or 0
         */
        public MInOut(MInOut original, int C_DocTypeShipment_ID, DateTime? movementDate)
            : this(original.GetCtx(), 0, original.Get_TrxName())
        {

            SetClientOrg(original);
            SetC_BPartner_ID(original.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(original.GetC_BPartner_Location_ID());	//	shipment address
            SetAD_User_ID(original.GetAD_User_ID());
            //
            SetM_Warehouse_ID(original.GetM_Warehouse_ID());
            SetIsSOTrx(original.IsSOTrx());
            SetMovementType(original.GetMovementType());
            if (C_DocTypeShipment_ID == 0)
            {
                SetC_DocType_ID(original.GetC_DocType_ID());
                SetIsReturnTrx(original.IsReturnTrx());
            }
            else
                SetC_DocType_ID(C_DocTypeShipment_ID, true);

            //	Default - Today
            if (movementDate != null)
                SetMovementDate(movementDate);
            SetDateAcct(GetMovementDate());

            //	Copy from Order
            SetC_Order_ID(original.GetC_Order_ID());
            SetDeliveryRule(original.GetDeliveryRule());
            SetDeliveryViaRule(original.GetDeliveryViaRule());
            SetM_Shipper_ID(original.GetM_Shipper_ID());
            SetFreightCostRule(original.GetFreightCostRule());
            SetFreightAmt(original.GetFreightAmt());
            SetSalesRep_ID(original.GetSalesRep_ID());
            //
            SetC_Activity_ID(original.GetC_Activity_ID());
            SetC_Campaign_ID(original.GetC_Campaign_ID());
            SetC_Charge_ID(original.GetC_Charge_ID());
            SetChargeAmt(original.GetChargeAmt());
            //
            SetC_Project_ID(original.GetC_Project_ID());
            SetDateOrdered(original.GetDateOrdered());
            SetDescription(original.GetDescription());
            SetPOReference(original.GetPOReference());
            SetSalesRep_ID(original.GetSalesRep_ID());
            SetAD_OrgTrx_ID(original.GetAD_OrgTrx_ID());
            SetUser1_ID(original.GetUser1_ID());
            SetUser2_ID(original.GetUser2_ID());
        }

        /**
         * 	Get Document Status
         *	@return Document Status Clear Text
         */
        public String GetDocStatusName()
        {
            return MRefList.GetListName(GetCtx(), 131, GetDocStatus());
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
            StringBuilder sb = new StringBuilder("MInOut[")
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
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
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
                string filePath = Path.Combine(GlobalVariable.PhysicalPath, "TempDownload", fileName);


                ReportEngine_N re = ReportEngine_N.Get(GetCtx(), ReportEngine_N.SHIPMENT, GetC_Invoice_ID());
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

            //ReportEngine re = ReportEngine.get(getCtx(), ReportEngine.SHIPMENT, getC_Invoice_ID());
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
        public MInOutLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MInOutLine> list = new List<MInOutLine>();
            String sql = "SELECT * FROM M_InOutLine WHERE M_InOut_ID=" + GetM_InOut_ID() + " ORDER BY Line";
            DataSet ds = null;
            DataRow dr = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    list.Add(new MInOutLine(GetCtx(), dr, Get_TrxName()));
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
            _lines = new MInOutLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /**
         * 	Get Lines of Shipment
         * 	@return lines
         */
        public MInOutLine[] GetLines()
        {
            return GetLines(false);
        }

        /**
         * 	Get Confirmations
         * 	@param requery requery
         *	@return array of Confirmations
         */
        public MInOutConfirm[] GetConfirmations(bool requery)
        {
            if (_confirms != null && !requery)
                return _confirms;

            List<MInOutConfirm> list = new List<MInOutConfirm>();
            String sql = "SELECT * FROM M_InOutConfirm WHERE M_InOut_ID=" + GetM_InOut_ID();
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
                    list.Add(new MInOutConfirm(GetCtx(), dr, Get_TrxName()));
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
            _confirms = new MInOutConfirm[list.Count];
            _confirms = list.ToArray();
            return _confirms;
        }

        //Pratap VAWMS 31-8-2015
        public Boolean AddServiceLines()
        {
            String sql = "SELECT C_OrderLine_ID "
                          + " FROM C_OrderLine ol"
                          + " LEFT OUTER JOIN M_Product p ON (ol.M_Product_ID=p.M_Product_ID)"
                          + " WHERE ol.C_Order_ID=@param1"
                          + " AND (ol.M_Product_ID IS NULL"
                          + " OR p.IsStocked = 'N'"
                          + " OR p.ProductType != 'I')"
                          + " AND (QtyOrdered=0 OR (QtyOrdered > QtyDelivered))"
                          + " AND NOT EXISTS (SELECT 1 FROM M_InOut io "
                          + " INNER JOIN M_InOutLine iol ON (io.M_InOut_ID=iol.M_InOut_ID)"
                          + " WHERE io.M_InOut_ID=@param2"
                          + " AND iol.C_OrderLine_ID=ol.C_OrderLine_ID)"
                          + " ORDER BY C_OrderLine_ID";
            //PreparedStatement pstmt = null;
            //ResultSet rs = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", GetC_Order_ID());
                param[1] = new SqlParameter("@param2", GetM_InOut_ID());
                idr = DB.ExecuteReader(sql, param, Get_Trx());
                //pstmt = DB.prepareStatement(sql, get_Trx());
                //pstmt.setInt(1, getC_Order_ID());
                //pstmt.setInt(2, getM_InOut_ID());
                //rs = pstmt.executeQuery();
                ArrayList serviceLines = new ArrayList();// ArrayList<MInOutLine> serviceLines = new ArrayList<MInOutLine>();
                ////while (rs.next())
                ////{
                ////    //list.add(new MInOutLine(getCtx(), rs, get_Trx()));
                while (idr.Read())
                {
                    // int C_OrderLine_ID = rs.getInt(1);
                    int C_OrderLine_ID = VAdvantage.Utility.Util.GetValueOfInt(idr[0]);
                    MOrderLine oLine = new MOrderLine(GetCtx(), C_OrderLine_ID, Get_TrxName());
                    MInOutLine line = new MInOutLine(this);
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
        public int CopyLinesFrom(MInOut otherShipment, bool counter, bool setOrder)
        {
            if (IsProcessed() || IsPosted() || otherShipment == null)
                return 0;
            MInOutLine[] fromLines = otherShipment.GetLines(false);
            int count = 0;
            for (int i = 0; i < fromLines.Length; i++)
            {
                MInOutLine line = new MInOutLine(this);
                MInOutLine fromLine = fromLines[i];
                line.Set_TrxName(Get_TrxName());
                if (counter)	//	header
                    PO.CopyValues(fromLine, line, GetAD_Client_ID(), GetAD_Org_ID());
                else
                    PO.CopyValues(fromLine, line, fromLine.GetAD_Client_ID(), fromLine.GetAD_Org_ID());
                line.SetM_InOut_ID(GetM_InOut_ID());
                line.Set_ValueNoCheck("M_InOutLine_ID", I_ZERO);	//	new
                //	Reset
                if (!setOrder)
                    line.SetC_OrderLine_ID(0);
                // SI_0642 : when we reverse MR or Customer Return, at that tym - on Save - system also check - qty availablity agaisnt same attribute 
                // on storage. If we set ASI as 0, then system not find qty and not able to save record
                if (!counter && !IsReversal())
                    line.SetM_AttributeSetInstance_ID(0);
                //	line.setS_ResourceAssignment_ID(0);
                line.SetRef_InOutLine_ID(0);
                line.SetIsInvoiced(false);
                //
                line.SetConfirmedQty(Env.ZERO);
                line.SetPickedQty(Env.ZERO);
                line.SetScrappedQty(Env.ZERO);
                line.SetTargetQty(Env.ZERO);
                //	Set Locator based on header Warehouse
                if (GetM_Warehouse_ID() != otherShipment.GetM_Warehouse_ID())
                {
                    line.SetM_Locator_ID(0);
                    line.SetM_Locator_ID((int)Env.ZERO);
                }
                //
                if (counter)
                {
                    line.SetC_OrderLine_ID(0);
                    line.SetRef_InOutLine_ID(fromLine.GetM_InOutLine_ID());
                    if (fromLine.GetC_OrderLine_ID() != 0)
                    {
                        MOrderLine peer = new MOrderLine(GetCtx(), fromLine.GetC_OrderLine_ID(), Get_TrxName());
                        if (peer.GetRef_OrderLine_ID() != 0)
                            line.SetC_OrderLine_ID(peer.GetRef_OrderLine_ID());
                    }
                }
                //
                if (IsReversal())
                {
                    line.SetQtyEntered(Decimal.Negate(line.GetQtyEntered()));
                    line.SetMovementQty(Decimal.Negate(line.GetMovementQty()));
                    if (line.Get_ColumnIndex("ReversalDoc_ID") > 0)
                    {
                        line.SetReversalDoc_ID(fromLine.GetM_InOutLine_ID());
                    }
                    // to set OrderLine in case of reversal if it is available 
                    line.SetC_OrderLine_ID(fromLine.GetC_OrderLine_ID());
                    //set container reference(if, not a copy record)
                    line.SetM_ProductContainer_ID(fromLine.GetM_ProductContainer_ID());

                    // VIS0060: Set Asset Values on Reversal Line in case of Sale of Asset.
                    if (otherShipment.IsSOTrx() && fromLine.GetA_Asset_ID() > 0 && Env.IsModuleInstalled("VAFAM_") && fromLine.Get_ColumnIndex("VAFAM_AssetValue") >= 0)
                    {
                        line.SetA_Asset_ID(fromLine.GetA_Asset_ID());
                        line.SetVAFAM_Quantity(fromLine.GetVAFAM_Quantity());
                        line.SetVAFAM_AssetValue(decimal.Negate(fromLine.GetVAFAM_AssetValue()));
                        line.SetVAFAM_DepAmount(decimal.Negate(fromLine.GetVAFAM_DepAmount()));
                    }
                }
                line.SetProcessed(false);

                // VA077 specific 
                if (Env.IsModuleInstalled("VA077_"))
                {
                    // Handle the case of reverse, set margin amt negative
                    line.Set_Value("VA077_MarginAmt", decimal.Negate(Util.GetValueOfDecimal(line.Get_Value("VA077_MarginAmt"))));
                }

                if (line.Save(Get_TrxName()))
                    count++;
                //	Cross Link
                if (counter)
                {
                    fromLine.SetRef_InOutLine_ID(line.GetM_InOutLine_ID());
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
        private void SetOrder(MOrder order)
        {
            SetClientOrg(order);
            SetC_Order_ID(order.GetC_Order_ID());
            //
            SetC_BPartner_ID(order.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(order.GetC_BPartner_Location_ID());	//	shipment address
            SetAD_User_ID(order.GetAD_User_ID());
            //
            SetM_Warehouse_ID(order.GetM_Warehouse_ID());
            SetIsSOTrx(order.IsSOTrx());
            SetIsReturnTrx(order.IsReturnTrx());

            if (!IsReturnTrx())
                SetMovementType(order.IsSOTrx() ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
            else
                SetMovementType(order.IsSOTrx() ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);
            //
            SetDeliveryRule(order.GetDeliveryRule());
            SetDeliveryViaRule(order.GetDeliveryViaRule());
            SetM_Shipper_ID(order.GetM_Shipper_ID());
            SetFreightCostRule(order.GetFreightCostRule());
            SetFreightAmt(order.GetFreightAmt());
            SetSalesRep_ID(order.GetSalesRep_ID());
            //
            SetC_Activity_ID(order.GetC_Activity_ID());
            SetC_Campaign_ID(order.GetC_Campaign_ID());
            SetC_Charge_ID(order.GetC_Charge_ID());
            SetChargeAmt(order.GetChargeAmt());
            //
            SetC_Project_ID(order.GetC_Project_ID());
            SetDateOrdered(order.GetDateOrdered());
            SetDescription(order.GetDescription());
            SetPOReference(order.GetPOReference());
            SetSalesRep_ID(order.GetSalesRep_ID());
            SetAD_OrgTrx_ID(order.GetAD_OrgTrx_ID());
            SetUser1_ID(order.GetUser1_ID());
            SetUser2_ID(order.GetUser2_ID());

        }

        /**
         * 	Set Order - Callout
         *	@param oldC_Order_ID old BP
         *	@param newC_Order_ID new BP
         *	@param windowNo window no
         */
        //@UICallout Web user interface method
        public void SetC_Order_ID(String oldC_Order_ID, String newC_Order_ID, int windowNo)
        {
            if (newC_Order_ID == null || newC_Order_ID.Length == 0)
                return;
            int C_Order_ID = int.Parse(newC_Order_ID);
            if (C_Order_ID == 0)
                return;
            //	Get Details
            MOrder order = new MOrder(GetCtx(), C_Order_ID, null);
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
            String sql = "UPDATE M_InOutLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE M_InOut_ID=" + GetM_InOut_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - Lines=" + noLine);
        }

        /**
         * 	Get BPartner
         *	@return partner
         */
        public MBPartner GetBPartner()
        {
            if (_partner == null)
                _partner = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            return _partner;
        }

        /**
         * 	Set Document Type
         * 	@param DocBaseType doc type MDocBaseType.DOCBASETYPE_
         */
        public void SetC_DocType_ID(String DocBaseType)
        {
            String sql = "SELECT C_DocType_ID FROM C_DocType "
                + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND DocBaseType=" + DocBaseType
                + " AND IsActive='Y' AND IsReturnTrx='N'"
                + " AND IsSOTrx='" + (IsSOTrx() ? "Y" : "N") + "' "
                + "ORDER BY IsDefault DESC";
            int C_DocType_ID = int.Parse(ExecuteQuery.ExecuteScalar(sql));
            if (C_DocType_ID <= 0)
            {
                log.Log(Level.SEVERE, "Not found for AC_Client_ID="
                     + GetAD_Client_ID() + " - " + DocBaseType);
            }
            else
            {
                log.Fine("DocBaseType=" + DocBaseType + " - C_DocType_ID=" + C_DocType_ID);
                SetC_DocType_ID(C_DocType_ID);
                bool isSOTrx = MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(DocBaseType);
                SetIsSOTrx(isSOTrx);
                SetIsReturnTrx(false);
            }
        }

        /**
         * 	Set Default C_DocType_ID.
         * 	Based on SO flag
         */
        public void SetC_DocType_ID()
        {
            if (IsSOTrx())
                SetC_DocType_ID(MDocBaseType.DOCBASETYPE_MATERIALDELIVERY);
            else
                SetC_DocType_ID(MDocBaseType.DOCBASETYPE_MATERIALRECEIPT);
        }

        /**
         * 	Set Document Type
         *	@param C_DocType_ID dt
         *	@param setReturnTrx if true set IsRteurnTrx and SOTrx
         */
        public void SetC_DocType_ID(int C_DocType_ID, bool setReturnTrx)
        {
            base.SetC_DocType_ID(C_DocType_ID);
            if (setReturnTrx)
            {
                MDocType dt = MDocType.Get(GetCtx(), C_DocType_ID);
                SetIsReturnTrx(dt.IsReturnTrx());
                SetIsSOTrx(dt.IsSOTrx());
            }
        }

        /**
         * 	Set Document Type - Callout.
         * 	Sets MovementType, DocumentNo
         * 	@param oldC_DocType_ID old ID
         * 	@param newC_DocType_ID new ID
         * 	@param windowNo window
         */
        //	@UICallout
        public void SetC_DocType_ID(String oldC_DocType_ID,
               String newC_DocType_ID, int windowNo)
        {
            if (newC_DocType_ID == null || newC_DocType_ID.Length == 0)
                return;
            int C_DocType_ID = int.Parse(newC_DocType_ID);
            if (C_DocType_ID == 0)
                return;
            String sql = "SELECT d.DocBaseType, d.IsDocNoControlled, s.CurrentNext, d.IsReturnTrx "
                + "FROM C_DocType d, AD_Sequence s "
                + "WHERE C_DocType_ID=" + C_DocType_ID		//	1
                + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
            try
            {
                DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetC_DocType_ID(C_DocType_ID);
                    /********************************************************************************************/
                    /********************************************************************************************/
                    //Consequences of - Field Value Changes	- New Row- Save (Update/Insert) Row
                    //Set Ctx - add to changed Ctx
                    //p_changeVO.setContext(getCtx(), windowNo, "C_DocTypeTarget_ID", C_DocType_ID);
                    //	Set Movement Type
                    String DocBaseType = dr["DocBaseType"].ToString();
                    Boolean IsReturnTrx = "Y".Equals(dr[3].ToString());

                    if (DocBaseType.Equals(MDocBaseType.DOCBASETYPE_MATERIALDELIVERY))		//	Shipments
                    {
                        if (IsReturnTrx)
                            SetMovementType(MOVEMENTTYPE_CustomerReturns);
                        else
                            SetMovementType(MOVEMENTTYPE_CustomerShipment);
                    }
                    else if (DocBaseType.Equals(MDocBaseType.DOCBASETYPE_MATERIALRECEIPT))	//	Receipts
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
        public void SetBPartner(MBPartner bp)
        {
            if (bp == null)
                return;

            SetC_BPartner_ID(bp.GetC_BPartner_ID());

            //	Set Locations*******************************************************************************
            MBPartnerLocation[] locs = bp.GetLocations(false);
            if (locs != null)
            {
                for (int i = 0; i < locs.Length; i++)
                {
                    if (locs[i].IsShipTo())
                        SetC_BPartner_Location_ID(locs[i].GetC_BPartner_Location_ID());
                }
                //	set to first if not set
                if (GetC_BPartner_Location_ID() == 0 && locs.Length > 0)
                    SetC_BPartner_Location_ID(locs[0].GetC_BPartner_Location_ID());
            }
            if (GetC_BPartner_Location_ID() == 0)
            {
                log.Log(Level.SEVERE, "Has no To Address: " + bp);
            }

            //	Set Contact
            MUser[] contacts = bp.GetContacts(false);
            if (contacts != null && contacts.Length > 0)	//	get first User
                SetAD_User_ID(contacts[0].GetAD_User_ID());
        }

        /// <summary>
        /// Set Business Partner - Callout
        /// </summary>
        /// <param name="oldC_BPartner_ID">old BP</param>
        /// <param name="newC_BPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetC_BPartner_ID(String oldC_BPartner_ID,
               String newC_BPartner_ID, int windowNo)
        {
            if (newC_BPartner_ID == null || newC_BPartner_ID.Length == 0)
                return;
            int C_BPartner_ID = int.Parse(newC_BPartner_ID);
            if (C_BPartner_ID == 0)
                return;
            String sql = "SELECT p.AD_Language, p.POReference,"
                + "SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "l.C_BPartner_Location_ID, c.AD_User_ID "
                + "FROM C_BPartner p"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID)"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID;		//	1
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetC_BPartner_ID(C_BPartner_ID);
                    //	Location
                    int ii = (int)dr["C_BPartner_Location_ID"];
                    if (ii != 0)
                        SetC_BPartner_Location_ID(ii);
                    //	Contact
                    ii = (int)dr["AD_User_ID"];
                    SetAD_User_ID(ii);

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
        /// <param name="oldC_BPartner_ID">old BP</param>
        /// <param name="newC_BPartner_ID">new BP</param>
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
        /// <param name="M_Warehouse_ID">id</param>
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID == 0)
            {
                log.Severe("Ignored - Cannot set AD_Warehouse_ID to 0");
                return;
            }
            base.SetM_Warehouse_ID(M_Warehouse_ID);
            //
            MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
            if (wh.GetAD_Org_ID() != GetAD_Org_ID())
            {
                log.Warning("M_Warehouse_ID=" + M_Warehouse_ID
                + ", Overwritten AD_Org_ID=" + GetAD_Org_ID() + "->" + wh.GetAD_Org_ID());
                SetAD_Org_ID(wh.GetAD_Org_ID());
            }
        }

        /// <summary>
        /// Set Business Partner - Callout
        /// </summary>
        /// <param name="oldC_BPartner_ID">old BP</param>
        /// <param name="newC_BPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetM_Warehouse_ID(String oldM_Warehouse_ID,
            String newM_Warehouse_ID, int windowNo)
        {
            if (newM_Warehouse_ID == null || newM_Warehouse_ID.Length == 0)
                return;
            int M_Warehouse_ID = int.Parse(newM_Warehouse_ID);
            if (M_Warehouse_ID == 0)
                return;
            //
            String sql = "SELECT w.AD_Org_ID, l.M_Locator_ID "
                + "FROM M_Warehouse w"
                + " LEFT OUTER JOIN M_Locator l ON (l.M_Warehouse_ID=w.M_Warehouse_ID AND l.IsDefault='Y') "
                + "WHERE w.M_Warehouse_ID=" + M_Warehouse_ID;		//	1

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetM_Warehouse_ID(M_Warehouse_ID);
                    //	Org
                    int AD_Org_ID = (int)dr[0];
                    SetAD_Org_ID(AD_Org_ID);
                    //	Locator
                    int M_Locator_ID = (int)dr[1];
                    if (M_Locator_ID != 0)
                    {
                        //p_changeVO.setContext(getCtx(), windowNo, "M_Locator_ID", M_Locator_ID);
                    }
                    else
                    {
                        //p_changeVO.setContext(getCtx(), windowNo, "M_Locator_ID", (String)null);
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
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
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
                MInOutConfirm[] confirmations = GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MInOutConfirm confirm = confirmations[i];
                    if (MInOutConfirm.CONFIRMTYPE_PickQAConfirm.Equals(confirm.GetConfirmType()))
                    {
                        if (!confirm.IsProcessed())		//	wait until done
                        {
                            log.Fine("Unprocessed: " + confirm);
                            return;
                        }
                        havePick = true;
                    }
                    else if (MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm.Equals(confirm.GetConfirmType()))
                        haveShip = true;
                }
                //	Create Pick
                if (!havePick)
                {
                    MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_PickQAConfirm, false);
                    return;
                }
                //	Create Ship
                if (!haveShip)
                {
                    MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, false);
                    return;
                }
                return;
            }
            //	Create just one
            if (pick)
                MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_PickQAConfirm, true);
            else if (ship)
            {
                if (!checkDocStatus)
                    MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, true);
                else
                    MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, false);
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
                MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                if (wh.GetAD_Org_ID() != GetAD_Org_ID())
                {
                    log.SaveError("WarehouseOrgConflict", "");
                    return false;
                }
            }
            else if (Is_ValueChanged("M_Warehouse_ID"))
            {
                //JID_0858: when line exist then not able to update / change warehouse
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_InoutLine_ID) FROM M_InoutLine WHERE M_Inout_ID = " + GetM_InOut_ID(), null, Get_Trx())) > 0)
                {
                    log.SaveError("VIS_WarehouseCantChange", "");
                    return false;
                }
            }

            //JID_1483- Accounts date should remain same as movement date
            SetDateAcct(GetMovementDate());

            //	Shipment - Needs Order
            if (IsSOTrx() && GetC_Order_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.Translate(GetCtx(), "C_Order_ID"));
                return false;
            }

            // VIS0060: Check if Document Type is not selected.
            if (newRecord && GetC_Order_ID() > 0 && GetC_DocType_ID() == 0)
            {
                log.SaveError("VIS_ShipDocTypeNotFound", "");
                return false;
            }

            if (newRecord || Is_ValueChanged("C_BPartner_ID"))
            {
                //MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
                string bpActive = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM C_BPartner WHERE C_BPartner_ID = " + GetC_BPartner_ID(), null, Get_Trx()));
                if (bpActive.Equals("N"))
                {
                    log.SaveError("NotActive", Msg.GetMsg(GetCtx(), "C_BPartner_ID"));
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
            //    MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
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
            //    else if(bp.GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerLocation)
            //    {
            //        MBPartnerLocation bpl = new MBPartnerLocation(GetCtx(), GetC_BPartner_Location_ID(), null);
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
                if (Is_ValueChanged("AD_Org_ID"))
                {
                    String sql = "UPDATE M_InOutLine ol"
                        + " SET AD_Org_ID ="
                            + "(SELECT AD_Org_ID"
                            + " FROM M_InOut o WHERE ol.M_InOut_ID=o.M_InOut_ID) "
                        + "WHERE M_InOut_ID=" + GetC_Order_ID();
                    int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
                    log.Fine("Lines -> #" + no);
                }
            }

            if (((IsSOTrx() && !IsReturnTrx()) || GetMovementType() == "V-") && !IsReversal())
            {
                MOrder ord = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
                Decimal grandTotal = MConversionRate.ConvertBase(GetCtx(),
                        ord.GetGrandTotal(), GetC_Currency_ID(), GetDateOrdered(),
                        ord.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());

                MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_Trx());
                string retMsg = "";
                bool crdAll = bp.IsCreditAllowed(GetC_BPartner_Location_ID(), grandTotal, out retMsg);
                if (!crdAll)
                    log.SaveWarning("Warning", retMsg);
                else if (bp.IsCreditWatch(GetC_BPartner_Location_ID()))
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

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            SetIsReturnTrx(dt.IsReturnTrx());
            SetIsSOTrx(dt.IsSOTrx());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            // Added by Vivek on 08/11/2017 assigned by Mukesh sir
            // check if Linked PO is not in completed or closed stage then not complete this record
            if (GetC_Order_ID() != 0 && !IsSOTrx() && !IsReturnTrx())
            {
                //MOrder order = new MOrder(GetCtx(), GetC_Order_ID(), Get_Trx());
                string docStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM C_Order WHERE C_Order_ID = " + GetC_Order_ID(), null, Get_Trx()));
                if (docStatus != "CO" && docStatus != "CL")
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
                    Decimal? onCreditAmt = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT VAPOS_CreditAmt FROM C_Order WHERE C_Order_ID = " + GetC_Order_ID()));
                    if (onCreditAmt <= 0)
                        checkCreditStatus = false;
                }
                if (checkCreditStatus)
                {
                    MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), null);
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

                    Decimal notInvoicedAmt = MBPartner.GetNotInvoicedAmt(GetC_BPartner_ID());
                    if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus(notInvoicedAmt)))
                    {
                        _processMsg = "@BPartnerOverSCreditHold@ - @TotalOpenBalance@="
                            + bp.GetTotalOpenBalance() + ", @NotInvoicedAmt@=" + notInvoicedAmt
                            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                        return DocActionVariables.STATUS_INVALID;
                    }

                    // check for Credit limit and Credit validation on Customer Master or Location
                    string retMsg = "";
                    bool crdAll = bp.IsCreditAllowed(GetC_BPartner_Location_ID(), 0, out retMsg);
                    if (!crdAll)
                    {
                        if (bp.ValidateCreditValidation("B,D,E,F", GetC_BPartner_Location_ID()))
                        {
                            _processMsg = retMsg;
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }


            if (Env.IsModuleInstalled("VA009_"))
            {
                if (GetC_Order_ID() != 0)
                {
                    int _countschedule = Util.GetValueOfInt(DB.ExecuteScalar("Select Count(VA009_OrderPaySchedule_ID) From VA009_OrderPaySchedule Where C_Order_ID=" + GetC_Order_ID()));
                    if (_countschedule > 0)
                    {
                        if (Util.GetValueOfInt(DB.ExecuteScalar("Select Count(VA009_OrderPaySchedule_ID) From VA009_OrderPaySchedule Where C_Order_ID=" + GetC_Order_ID() + " AND VA009_Ispaid='Y'")) != _countschedule)
                        {
                            _processMsg = Msg.GetMsg(Env.GetCtx(), "VIS_PayAdvance"); // "Please Do Advance Payment against order";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }

            //	Lines
            MInOutLine[] lines = GetLines(true);
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
                MInOutLine line = lines[i];
                MProduct product = line.GetProduct();
                if (product != null)
                {
                    Volume = Decimal.Add(Volume, Decimal.Multiply((Decimal)product.GetVolume(),
                        line.GetMovementQty()));
                    Weight = Decimal.Add(Weight, Decimal.Multiply(product.GetWeight(),
                        line.GetMovementQty()));
                }
                if (line.GetM_AttributeSetInstance_ID() != 0)
                    continue;
                if (product != null)
                {
                    int M_AttributeSet_ID = product.GetM_AttributeSet_ID();
                    if (M_AttributeSet_ID != 0)
                    {
                        MAttributeSet mas = MAttributeSet.Get(GetCtx(), M_AttributeSet_ID);
                        if (mas != null
                            && ((IsSOTrx() && mas.IsMandatory())
                                || (!IsSOTrx() && mas.IsMandatoryAlways())))
                        {
                            // JID_0259: Here we need to show the name of the product for which attribute is mandatory.
                            _processMsg = product.GetName() + ": " + "@M_AttributeSet_ID@ @IsMandatory@";
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
        /// Check DocTypeConfimation
        /// </summary>
        /// <param name="dt">DocumentType</param>
        /// <returns>error Message if Confiramtion Doct Tpye not Selected</returns>
        private string CheckConfimationDocType(MDocType dt)
        {
            if (dt.Get_ColumnIndex("C_DocTypeConfrimation_ID") > -1)
            {
                int conDocType = Util.GetValueOfInt(dt.Get_Value("C_DocTypeConfrimation_ID"));
                if (conDocType == 0)
                { return Msg.GetMsg(GetCtx(), "VIS_ConfirmationDocNotFound"); }
            }
            return null;
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
            isContainrApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            //************* Change By Lokesh Chauhan ***************
            // If qty on locator is insufficient then return
            // Will not complete.
            StringBuilder sql = new StringBuilder();
            //MProduct pro = null;
            //Dictionary<int, MInOutLineMA[]> lineAttributes = null;
            //if (IsSOTrx())
            //{

            String MovementTyp = GetMovementType();

            int VAPOS_POSTerminal_ID = 0;
            if (Env.IsModuleInstalled("VAPOS_"))
            {
                VAPOS_POSTerminal_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAPOS_POSTERMINAL_ID FROM C_Order WHERE C_ORDER_ID=" + GetC_Order_ID()));
            }
            if (!(VAPOS_POSTerminal_ID > 0))
            {
                if (MovementTyp.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                {
                    #region Prevent from completing, If qty on Product trnsaction on specified movement date not available as per qty entered at line and Disallow negative is true at Warehouse
                    sql.Clear();
                    sql.Append("SELECT ISDISALLOWNEGATIVEINV FROM M_Warehouse WHERE M_Warehouse_ID = " + Util.GetValueOfInt(GetM_Warehouse_ID()));
                    string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                    if (disallow.ToUpper() == "Y")
                    {
                        // is used to handle Non Stocked Item which are not in Storage
                        string whereClause = "M_Inout_ID = " + GetM_InOut_ID() + @" AND C_Charge_ID IS NULL AND M_Product_ID NOT IN 
                            (SELECT M_InoutLine.M_Product_ID FROM M_InoutLine INNER JOIN M_Product ON M_InoutLine.M_Product_ID = M_Product.M_Product_ID 
                            WHERE M_Product.IsStocked = 'N' AND M_InoutLine.M_Inout_ID  = " + GetM_InOut_ID() + " ) ";
                        int[] ioLine = MInOutLine.GetAllIDs("M_InoutLine", whereClause, Get_TrxName());
                        int m_locator_id = 0;
                        int m_product_id = 0;
                        StringBuilder products = new StringBuilder();   // Added by sukhwinder for storing product aand locators IDs. on 19Dec, 2017
                        StringBuilder locators = new StringBuilder();
                        bool check = false;
                        for (int i = 0; i < ioLine.Length; i++)
                        {
                            MInOutLine iol = new MInOutLine(Env.GetCtx(), ioLine[i], Get_TrxName());
                            m_locator_id = Util.GetValueOfInt(iol.GetM_Locator_ID());
                            m_product_id = Util.GetValueOfInt(iol.GetM_Product_ID());

                            sql.Clear();
                            sql.Append("SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + m_product_id);
                            int m_attribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                            if (m_attribute_ID == 0)
                            {
                                sql.Clear();
                                if (!isContainrApplicable)
                                {
                                    sql.Append("SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id);
                                }
                                else
                                {
                                    sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID, NVL(t.M_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                                        INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                            " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + iol.GetM_Locator_ID() +
                                            " AND t.M_Product_ID = " + iol.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + iol.GetM_AttributeSetInstance_ID() +
                                            " AND NVL(t.M_ProductContainer_ID, 0) = " + iol.GetM_ProductContainer_ID());
                                }
                                decimal qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                                decimal qtyToMove = iol.GetMovementQty();
                                if (qty < qtyToMove)
                                {
                                    check = true;
                                    products.Append(m_product_id + ", ");
                                    locators.Append(m_locator_id + ", ");
                                    continue;
                                }
                            }
                            else
                            {
                                sql.Clear();
                                if (!isContainrApplicable)
                                {
                                    sql.Append("SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id + " AND NVL(M_AttributeSetInstance_ID , 0) = " + iol.GetM_AttributeSetInstance_ID());
                                }
                                else
                                {
                                    sql.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID, NVL(t.M_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                                        INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                            " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + iol.GetM_Locator_ID() +
                                            " AND t.M_Product_ID = " + iol.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + iol.GetM_AttributeSetInstance_ID() +
                                            " AND NVL(t.M_ProductContainer_ID, 0) = " + iol.GetM_ProductContainer_ID());
                                }
                                decimal qty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                                decimal qtyToMove = iol.GetMovementQty();
                                if (qty < qtyToMove)
                                {
                                    check = true;
                                    products.Append(m_product_id + ",");
                                    locators.Append(m_locator_id + ",");
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
            //    //Dictionary<int, MInOutLineMA[]> lineAttributes = null;
            //    //MProduct pro = null;
            //    lineAttributes = new Dictionary<int, MInOutLineMA[]>();
            //    sql = "select m_inoutline_id,qtyentered,line,m_Product_id from m_inoutline where m_inout_id=" + GetM_InOut_ID();
            //    idr = DB.ExecuteReader(sql);
            //    while (idr.Read())
            //    {
            //        pro = new MProduct(GetCtx(), Util.GetValueOfInt(idr["m_Product_id"]), null);
            //        if (pro.GetM_AttributeSetInstance_ID() != 0)
            //        {
            //            MInOutLineMA[] attrib = MInOutLineMA.Get(GetCtx(), Util.GetValueOfInt(idr[0]), Get_Trx());
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
            //    X_M_InOut inOut = new X_M_InOut(GetCtx(), GetM_InOut_ID(), null);
            //    string sqlL = "select name from c_bpartner_location where c_bpartner_location_id=" + inOut.GetC_BPartner_Location_ID();
            //    string name = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sqlL));
            //    Boolean result = name.StartsWith("POS -");
            //    if (result)
            //    {
            //        int[] allIds = X_M_InOutLine.GetAllIDs("M_InOutLine", "M_InOut_ID=" + inOut.GetM_InOut_ID(), null);
            //        if (allIds.Length > 0)
            //        {
            //            for (int i = 0; i < allIds.Length; i++)
            //            {
            //                X_M_InOutLine IOLine = new X_M_InOutLine(GetCtx(), allIds[i], null);
            //                MProdReceived ProdReceived = new MProdReceived(GetCtx(), 0, null);
            //                ProdReceived.SetAD_Client_ID(inOut.GetAD_Client_ID());
            //                ProdReceived.SetAD_Org_ID(inOut.GetAD_Org_ID());
            //                ProdReceived.SetC_BPartner_ID(inOut.GetC_BPartner_ID());
            //                ProdReceived.SetC_BPartner_Location_ID(inOut.GetC_BPartner_Location_ID());
            //                ProdReceived.SetDS_AREA_ID(inOut.GetDS_AREA_ID());
            //                ProdReceived.SetDS_SUBLOCATION_ID(inOut.GetDS_SUBLOCATION_ID());
            //                ProdReceived.SetM_InOut_ID(GetM_InOut_ID());
            //                ProdReceived.SetM_Product_ID(IOLine.GetM_Product_ID());
            //                sqlL = "select m_product_category_id from M_product where m_product_id =" + IOLine.GetM_Product_ID();
            //                int M_Product_Category_ID = VAdvantage.Utility.Util.GetValueOfInt(DB.ExecuteScalar(sqlL));
            //                ProdReceived.SetM_Product_Category_ID(M_Product_Category_ID);
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
                sql.Append(DBFunctionCollection.MInOutContainerNotMatched(GetM_InOut_ID()));
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
                sql.Append(DBFunctionCollection.MInOutContainerNotAvailable(GetM_InOut_ID()));
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

            // Set Document Date based on setting on Document Type
            SetCompletedDocumentDate();

            // To check weather future date records are available in Transaction window
            // this check implement after "SetCompletedDocumentDate" function, because this function overwrit movement date
            _processMsg = MTransaction.CheckFutureDateRecord(GetMovementDate(), Get_TableName(), GetM_InOut_ID(), Get_Trx());
            if (!string.IsNullOrEmpty(_processMsg))
            {
                return DocActionVariables.STATUS_INVALID;
            }

            if (Env.IsModuleInstalled("VA024_"))
            {
                sql.Clear();
                sql.Append(@"SELECT AD_TABLE_ID  FROM AD_TABLE WHERE tablename LIKE 'VA024_T_ObsoleteInventory' AND IsActive = 'Y'");
                tableId = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            }

            if (Env.IsModuleInstalled("VA026_"))
            {
                sql.Clear();
                sql.Append(@"SELECT AD_TABLE_ID  FROM AD_TABLE WHERE tablename LIKE 'VA026_LCDetail' AND IsActive = 'Y'");
                tableId1 = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            }


            //Lakhwinder 10 Feb 2021
            //Show Error if Confiramtion Doct Tpye not Selected on DocType
            MDocType docType = MDocType.Get(GetCtx(), GetC_DocType_ID());
            if (docType.IsShipConfirm())
            {
                string s = CheckConfimationDocType(docType);
                if (!String.IsNullOrEmpty(s))
                {
                    _processMsg = s;
                    SetProcessMsg(_processMsg);
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            // for checking - costing calculate on completion or not
            // IsCostImmediate = true - calculate cost on completion
            MClient client = MClient.Get(GetCtx(), GetAD_Client_ID());

            // get Warehouse Stock availablity, PO price in base currency
            DataSet dsAvailableStock = DB.ExecuteDataset(@"SELECT M_InOutLine.M_InOutLine_ID , M_Storage.QtyOnHand, 
                                        NVL(CurrencyConvert(C_OrderLine.PriceEntered , C_Order.C_Currency_ID , " + GetCtx().GetContextAsInt("$C_Currency_ID") +
                                        @", M_InOut.DateAcct, C_Order.C_ConversionType_ID, M_InOut.AD_Client_ID, M_InOut.AD_Org_ID) , 0) as PriceEntered
                                        FROM M_InOutLine 
                                        INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                        LEFT JOIN M_Storage ON(M_InOutLine.M_Locator_ID = M_Storage.M_Locator_ID
                                        AND M_InOutLine.M_Product_ID = M_Storage.M_Product_ID
                                        AND NVL(M_InOutLine.M_AttributeSetInstance_ID, 0) = NVL(M_Storage.M_AttributeSetInstance_ID, 0))
                                        LEFT JOIN C_OrderLine ON M_InOutLine.C_OrderLine_ID = C_OrderLine.C_OrderLine_ID
                                        LEFT JOIN C_Order ON C_Order.C_Order_ID = C_OrderLine.C_Order_ID
                                        WHERE M_InOut.M_InOut_ID = " + GetM_InOut_ID());

            //	Outstanding (not processed) Incoming Confirmations ?
            MInOutConfirm[] confirmations = GetConfirmations(true);
            Int32 confirmationCount = 0;
            for (int i = 0; i < confirmations.Length; i++)
            {
                MInOutConfirm confirm = confirmations[i];
                //New Code added Here 
                confirmationCount += 1;
                //Arpit to check docStatus on of confirm doc 
                if (!confirm.IsProcessed())
                {
                    if (MInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                        continue;
                    if (confirm.GetDocStatus() == DOCSTATUS_Voided)
                    {
                        if (confirmationCount == confirmations.Length)
                        {
                            CreateConfirmation(true); //Paasing optional Parameter to false the value of existing
                            //  _processMsg = Msg.GetMsg(GetCtx(),"NoConfirmationFoundForMR") + GetDocumentNo();
                            //Message-No Confirmation found for Material Receipt No: Key -->NoConfirmationFoundForMR
                            //  return _processMsg;
                            if (MInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                                continue;
                            //
                            _processMsg = "Open @M_InOutConfirm_ID@: " +
                                confirm.GetConfirmTypeName() + " - " + confirm.GetDocumentNo();
                            // VIS-158 : Cable Industry specific change to set MR confirmation number
                            if (Env.IsModuleInstalled("VA076_") && Get_ColumnIndex("VA076_ConfirmationNo") >= 0)
                            {
                                Set_Value("VA076_ConfirmationNo", confirm.GetDocumentNo());
                            }
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
                        if (MInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                            continue;
                        //
                        _processMsg = "Open @M_InOutConfirm_ID@: " +
                            confirm.GetConfirmTypeName() + " - " + confirm.GetDocumentNo();
                        // VIS-158 : Cable Industry specific change to set MR confirmation number
                        if (Env.IsModuleInstalled("VA076_") && Get_ColumnIndex("VA076_ConfirmationNo") >= 0)
                        {
                            Set_Value("VA076_ConfirmationNo", confirm.GetDocumentNo());
                        }
                        FreezeDoc();//Arpit                  
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                }
                //END of New Code
                // Old Code Commented Here 
                //if (!confirm.IsProcessed())
                //{
                //    if (MInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                //        continue;
                //    //
                //    _processMsg = "Open @M_InOutConfirm_ID@: " +
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
            MInOutLine[] lines = GetLines(false);
            if (!(VAPOS_POSTerminal_ID > 0))
            {
                //for (int Index = 0; Index < lines.Length; Index++)
                //{
                //    MInOutLine Line = lines[Index];
                //    // Change done by mohit as discussed by ravikant and mukesh sir - do not check locator if there is charge on shipment line- 01/06/2017 (PMS TaskID=3893)
                //    if (Line.GetM_Locator_ID() == 0 && Line.GetM_Product_ID() != 0)
                //    {
                //        _processMsg = Msg.GetMsg(Env.GetCtx(), "LocatorNotFound");
                //        return DocActionVariables.STATUS_INVALID;
                //    }
                //}

                int noLocCnt = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_InOutLine_ID) FROM M_InOutLine WHERE IsActive = 'Y' AND M_InOut_ID =" + GetM_InOut_ID()
                    + " AND M_Locator_ID = 0 AND M_Product_ID != 0", null, Get_Trx()));
                if (noLocCnt > 0)
                {
                    _processMsg = Msg.GetMsg(Env.GetCtx(), "LocatorNotFound");
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            bool countVA026 = Env.IsModuleInstalled("VA009_") && Env.IsModuleInstalled("VA026_");
            #region [Process All Lines]
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                MInOutLine sLine = lines[lineIndex];
                MProduct product = sLine.GetProduct();
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
                        MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
                        if (VAPOS_POSTerminal_ID > 0)
                        {
                            if (IsSOTrx() && pc.GetA_Asset_Group_ID() > 0 && sLine.GetA_Asset_ID() == 0)
                            {
                                _processMsg = "AssetNotSetONShipmentLine: LineNo" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                        }
                        else
                        {
                            if (IsSOTrx() && !IsReturnTrx() && pc.GetA_Asset_Group_ID() > 0 && sLine.GetA_Asset_ID() == 0 && !Env.IsModuleInstalled("VA077_"))
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
                            if (!UpdateAssetValues(sLine))
                            {
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                        }
                    }
                    #endregion
                }

                #region done by Amit on behalf of surya 30-9-2015 vawms
                // on Warehouse, Set Qty Allocated only for Item type Product
                if (sLine.GetC_OrderLine_ID() != 0 && product != null && product.GetProductType() == MProduct.PRODUCTTYPE_Item && IsSOTrx() && !IsReturnTrx())
                {
                    if (Env.IsModuleInstalled("VAWMS_"))
                    {
                        MStorage allocatedStorage = MStorage.GetCreate(GetCtx(), sLine.GetM_Locator_ID(), sLine.GetM_Product_ID(),
                            sLine.GetM_AttributeSetInstance_ID(), Get_Trx());
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
                MOrderLine oLine = null;
                if (sLine.GetC_OrderLine_ID() != 0)
                {
                    oLine = new MOrderLine(GetCtx(), sLine.GetC_OrderLine_ID(), Get_TrxName());
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
                    MTransaction mtrx = null;

                    // Added By Amit 3-8-2015 VAMRP
                    #region VAMRP
                    //bool attribCheck = false;
                    //if (Env.HasModulePrefix("VAMRP_", out mInfo))
                    //{                 
                    //if (pro != null)
                    //{
                    //    attribCheck = pro.GetM_AttributeSetInstance_ID() != 0;
                    //}
                    ////attribute stirage update logic
                    //if (IsSOTrx() && attribCheck)
                    //{
                    //    MInOutLineMA[] mas = lineAttributes[sLine.GetM_InOutLine_ID()];
                    //    for (int j = 0; j < mas.Length; j++)
                    //    {
                    //        MInOutLineMA ma = mas[j];
                    //        Decimal QtyMA = ma.GetMovementQty();
                    //        //if (MovementType.charAt(1) == '-')	//	C- Customer Shipment - V- Vendor Return
                    //        if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                    //        {
                    //            QtyMA = Decimal.Negate(QtyMA);
                    //        }
                    //        Decimal QtySOMA = Env.ZERO;
                    //        Decimal QtyPOMA = Env.ZERO;

                    //        // nnayak - Don't update qty reserved or qty ordered for Returns
                    //        if (sLine.GetC_OrderLine_ID() != 0 && !IsReturnTrx())
                    //        {
                    //            if (IsSOTrx())
                    //                QtySOMA = Decimal.Negate(ma.GetMovementQty());
                    //            else
                    //                QtyPOMA = Decimal.Negate(ma.GetMovementQty());
                    //        }

                    //        log.Fine("QtyMA : " + QtyMA + " QtySOMA " + QtySOMA + " QtyPOMA " + QtyPOMA);

                    //        //	Update Storage - see also VMatch.createMatchRecord
                    //        /**********/
                    //        string sqlatr = "SELECT SUM(movementqty) FROM m_inoutlinema WHERE m_inoutline_id=" + sLine.GetM_InOutLine_ID();
                    //        int totalmovqty = Util.GetValueOfInt(DB.ExecuteScalar(sqlatr, null, null));

                    //        string sqty = "SELECT qtyentered FROM m_inoutline WHERE m_inoutline_id=" + sLine.GetM_InOutLine_ID();
                    //        int quantity = Util.GetValueOfInt(DB.ExecuteScalar(sqty, null, null));

                    //        //MInOutLine minline = new MInOutLine(GetCtx(), m_inoutline_id, Get_TrxName());
                    //        if (totalmovqty == quantity)
                    //        {

                    //            if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                    //              sLine.GetM_Locator_ID(),
                    //              sLine.GetM_Product_ID(),
                    //              ma.GetM_AttributeSetInstance_ID(), ma.GetM_AttributeSetInstance_ID(),
                    //              QtyMA, QtySOMA, QtyPOMA, Get_Trx(), sLine.GetM_InOutLine_ID()))
                    //            {
                    //                _processMsg = "Cannot correct Inventory (MA)";
                    //                return DocActionVariables.STATUS_INVALID;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            return "";
                    //        }
                    //        if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                    //            sLine.GetM_Locator_ID(),
                    //            sLine.GetM_Product_ID(),
                    //            ma.GetM_AttributeSetInstance_ID(), ma.GetM_AttributeSetInstance_ID(),
                    //            QtyMA, QtySOMA, QtyPOMA, Get_TrxName()))
                    //        {
                    //            _processMsg = "Cannot correct Inventory (MA)";
                    //            return DocActionVariables.STATUS_INVALID;
                    //        }
                    //        //	Create Transaction
                    //        mtrx = new MTransaction(GetCtx(), sLine.GetAD_Org_ID(),
                    //            MovementType, sLine.GetM_Locator_ID(),
                    //            sLine.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(),
                    //            QtyMA, GetMovementDate(), Get_TrxName());
                    //        mtrx.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
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
                    int reservationAttributeSetInstance_ID = 0; // sLine.getM_AttributeSetInstance_ID();
                    if (oLine != null)
                        reservationAttributeSetInstance_ID = oLine.GetM_AttributeSetInstance_ID();
                    //
                    if (sLine.GetM_AttributeSetInstance_ID() == 0 || sLine.GetM_AttributeSetInstance_ID() != 0)
                    {
                        MInOutLineMA[] mas = MInOutLineMA.Get(GetCtx(),
                            sLine.GetM_InOutLine_ID(), Get_TrxName());
                        for (int j = 0; j < mas.Length; j++)
                        {
                            Decimal? containerCurrentQty = 0;
                            MInOutLineMA ma = mas[j];
                            Decimal QtyMA = ma.GetMovementQty();
                            //if (MovementType.charAt(1) == '-')	//	C- Customer Shipment - V- Vendor Return
                            if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                            {
                                QtyMA = Decimal.Negate(QtyMA);
                            }
                            Decimal QtySOMA = Env.ZERO;
                            Decimal QtyPOMA = Env.ZERO;

                            // nnayak - Don't update qty reserved or qty ordered for Returns
                            if (sLine.GetC_OrderLine_ID() != 0 && !IsReturnTrx())
                            {
                                if (IsSOTrx())
                                    QtySOMA = Decimal.Negate(ma.GetMovementQty());
                                else
                                    QtyPOMA = Decimal.Negate(ma.GetMovementQty());
                            }

                            log.Fine("QtyMA : " + QtyMA + " QtySOMA " + QtySOMA + " QtyPOMA " + QtyPOMA);
                            //	Update Storage - see also VMatch.createMatchRecord



                            if (sLine.GetC_OrderLine_ID() != 0 && !IsReturnTrx())
                            {
                                #region Update Storage when record created with refenece of Orderline and not a RETURN trx
                                MOrderLine ordLine = new MOrderLine(GetCtx(), sLine.GetC_OrderLine_ID(), Get_TrxName());
                                //MOrder ord = new MOrder(GetCtx(), ordLine.GetC_Order_ID(), Get_TrxName());
                                int OrdWh_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Warehouse_ID FROM C_Order WHERE C_Order_ID ="
                                    + ordLine.GetC_Order_ID(), null, Get_Trx()));

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
                                        if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                        sLine.GetM_Locator_ID(), OrdWh_ID,
                                        sLine.GetM_Product_ID(),
                                        sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                                        if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                        sLine.GetM_Locator_ID(), GetM_Warehouse_ID(),
                                        sLine.GetM_Product_ID(),
                                        sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                                    if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                   sLine.GetM_Locator_ID(),
                                   sLine.GetM_Product_ID(),
                                   ma.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                                if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                    sLine.GetM_Locator_ID(),
                                    sLine.GetM_Product_ID(),
                                    ma.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                            sql.Append(@"SELECT DISTINCT First_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                                   " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + sLine.GetM_Locator_ID() +
                               " AND t.M_Product_ID = " + sLine.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + sLine.GetM_AttributeSetInstance_ID());
                            trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                            // get container Current qty from transaction
                            if (isContainrApplicable && sLine.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                containerCurrentQty = GetContainerQtyFromTransaction(sLine, GetMovementDate());
                            }

                            //	Create Transaction
                            mtrx = new MTransaction(GetCtx(), sLine.GetAD_Org_ID(),
                                MovementType, sLine.GetM_Locator_ID(),
                                sLine.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(),
                                QtyMA, GetMovementDate(), Get_TrxName());
                            mtrx.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
                            mtrx.SetCurrentQty(trxQty + QtyMA);
                            // set Material Policy Date
                            mtrx.SetMMPolicyDate(ma.GetMMPolicyDate());
                            if (isContainrApplicable && mtrx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                            {
                                // Update Product Container on Transaction
                                mtrx.SetM_ProductContainer_ID(sLine.GetM_ProductContainer_ID());
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
                            if (isContainrApplicable && mtrx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
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

                        if (sLine.GetC_OrderLine_ID() != 0 && !IsReturnTrx())
                        {
                            MOrderLine ordLine = new MOrderLine(GetCtx(), sLine.GetC_OrderLine_ID(), Get_TrxName());
                            //MOrder ord = new MOrder(GetCtx(), ordLine.GetC_Order_ID(), Get_TrxName());
                            int OrdWh_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Warehouse_ID FROM C_Order WHERE C_Order_ID ="
                                   + ordLine.GetC_Order_ID(), null, Get_Trx()));

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
                                    if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                                                sLine.GetM_Locator_ID(), OrdWh_ID,
                                                                sLine.GetM_Product_ID(),
                                                                sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                                    if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                                                sLine.GetM_Locator_ID(), GetM_Warehouse_ID(),
                                                                sLine.GetM_Product_ID(),
                                                                sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                                if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                sLine.GetM_Locator_ID(),
                                sLine.GetM_Product_ID(),
                                sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                            if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                sLine.GetM_Locator_ID(),
                                sLine.GetM_Product_ID(),
                                sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
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
                        sql.Append(@"SELECT DISTINCT FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                               " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + sLine.GetM_Locator_ID() +
                           " AND t.M_Product_ID = " + sLine.GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + sLine.GetM_AttributeSetInstance_ID());
                        trxQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                        if (isContainrApplicable && sLine.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            // get Container qty from transaction
                            containerCurrentQty = GetContainerQtyFromTransaction(sLine, GetMovementDate());
                        }

                        //	FallBack: Create Transaction
                        mtrx = new MTransaction(GetCtx(), sLine.GetAD_Org_ID(), MovementType, sLine.GetM_Locator_ID(),
                            sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(),
                            Qty, GetMovementDate(), Get_TrxName());
                        mtrx.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
                        mtrx.SetCurrentQty(trxQty + Qty);
                        if (isContainrApplicable && mtrx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                        {
                            // Update Product Container on Transaction
                            mtrx.SetM_ProductContainer_ID(sLine.GetM_ProductContainer_ID());
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
                        if (isContainrApplicable && mtrx.Get_ColumnIndex("M_ProductContainer_ID") >= 0)
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
                                || sLine.GetM_Product_ID() == 0)	//	PO Charges, empty lines
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
                        MOrderLine origOrderLine = new MOrderLine(GetCtx(), oLine.GetOrig_OrderLine_ID(), Get_TrxName());
                        if (IsSOTrx()							//	PO is done by Matching
                                || sLine.GetM_Product_ID() == 0)	//	PO Charges, empty lines
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
                    if (oLine != null && (oLine.GetC_OrderLine_Blanket_ID() > 0 || oLine.GetOrig_OrderLine_ID() > 0))
                    {
                        MOrderLine lineBlanket1 = new MOrderLine(GetCtx(), oLine.GetC_OrderLine_Blanket_ID(), Get_TrxName());
                        MOrderLine origOrderLine = new MOrderLine(GetCtx(), oLine.GetOrig_OrderLine_ID(), Get_TrxName());
                        MOrderLine lineBlanket = null;

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
                        if (origOrderLine.GetC_OrderLine_Blanket_ID() > 0)
                        {
                            lineBlanket = new MOrderLine(GetCtx(), origOrderLine.GetC_OrderLine_Blanket_ID(), Get_TrxName());

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

                MMatchPO Matchpo = null;
                List<MMatchInv> matchedInvoice = new List<MMatchInv>();
                //	Matching
                if (!IsSOTrx()
                    && sLine.GetM_Product_ID() != 0
                    && !IsReversal())
                {
                    Decimal matchQty = sLine.GetMovementQty();
                    //	Invoice - Receipt Match (requires Product)
                    MInvoiceLine iLine = MInvoiceLine.GetOfInOutLine(sLine);

                    if (iLine != null && iLine.GetM_Product_ID() != 0)
                    {
                        if (matchQty.CompareTo(iLine.GetQtyInvoiced()) > 0)
                            matchQty = iLine.GetQtyInvoiced();

                        // JID_0209: Need to set the correct qty on ‘Matched Receipts/Invoice’. System should set minimum qty of MR and invoice, whichever is lower. on match receipt

                        decimal alreadyMatchedQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(QTY) FROM M_MatchInv WHERE C_InvoiceLine_ID = " + iLine.GetC_InvoiceLine_ID(), null, Get_Trx()));

                        if ((alreadyMatchedQty + matchQty) > iLine.GetQtyInvoiced())
                        {
                            matchQty = iLine.GetQtyInvoiced() - alreadyMatchedQty;
                        }

                        MMatchInv[] matches = MMatchInv.Get(GetCtx(),
                            sLine.GetM_InOutLine_ID(), iLine.GetC_InvoiceLine_ID(), Get_TrxName());
                        if (matches == null || matches.Length == 0)
                        {
                            MMatchInv inv = new MMatchInv(iLine, GetMovementDate(), matchQty);
                            if (sLine.GetM_AttributeSetInstance_ID() != iLine.GetM_AttributeSetInstance_ID())
                            {
                                iLine.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                                iLine.Save();	//	update matched invoice with ASI
                                inv.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                            }
                            try
                            {
                                inv.Set_ValueNoCheck("C_BPartner_ID", GetC_BPartner_ID());
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
                    if (sLine.GetC_OrderLine_ID() != 0)
                    {
                        log.Fine("PO Matching");
                        //	Ship - PO
                        Matchpo = MMatchPO.Create(null, sLine, GetMovementDate(), matchQty);
                        try
                        {
                            Matchpo.Set_ValueNoCheck("C_BPartner_ID", GetC_BPartner_ID());
                        }
                        catch { }

                        Matchpo.SetIsSOTrx(IsSOTrx());
                        Matchpo.SetIsReturnTrx(IsReturnTrx());
                        if (dsAvailableStock != null && dsAvailableStock.Tables[0].Rows.Count > 0)
                        {
                            Matchpo.SetAvailableStock(Util.GetValueOfDecimal(
                                dsAvailableStock.Tables[0].Select("M_InOutLine_ID = " + sLine.GetM_InOutLine_ID())[0]["QtyOnHand"]));
                            Matchpo.SetPricePO(Util.GetValueOfDecimal(
                                dsAvailableStock.Tables[0].Select("M_InOutLine_ID = " + sLine.GetM_InOutLine_ID())[0]["PriceEntered"]));
                        }

                        if (!Matchpo.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not create PO Matching";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        //	Update PO with ASI                      Commented by Bharat
                        //if (oLine != null && oLine.GetM_AttributeSetInstance_ID() == 0)
                        //{
                        //    oLine.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                        //    oLine.Save(Get_TrxName());
                        //}
                    }
                    else	//	No Order - Try finding links via Invoice
                    {
                        //	Invoice has an Order Link
                        if (iLine != null && iLine.GetC_OrderLine_ID() != 0)
                        {
                            //	Invoice is created before  Shipment
                            log.Fine("PO(Inv) Matching");
                            //	Ship - Invoice
                            Matchpo = MMatchPO.Create(iLine, sLine, GetMovementDate(), matchQty);
                            try
                            {
                                Matchpo.Set_ValueNoCheck("C_BPartner_ID", GetC_BPartner_ID());
                            }
                            catch { }
                            Matchpo.SetIsSOTrx(IsSOTrx());
                            Matchpo.SetIsReturnTrx(IsReturnTrx());
                            if (dsAvailableStock != null)
                            {
                                Matchpo.SetAvailableStock(Util.GetValueOfDecimal(
                                    dsAvailableStock.Tables[0].Select("M_InOutLine_ID = " + sLine.GetM_InOutLine_ID())[0]["QtyOnHand"]));
                                Matchpo.SetPricePO(Util.GetValueOfDecimal(
                               dsAvailableStock.Tables[0].Select("M_InOutLine_ID = " + sLine.GetM_InOutLine_ID())[0]["PriceEntered"]));
                            }
                            if (!Matchpo.Save(Get_TrxName()))
                            {
                                _processMsg = "Could not create PO(Inv) Matching";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            //	Update PO with ASI                   Commented by Bharat
                            //oLine = new MOrderLine(GetCtx(), po.GetC_OrderLine_ID(), Get_TrxName());
                            //if (oLine != null && oLine.GetM_AttributeSetInstance_ID() == 0)
                            //{
                            //    oLine.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                            //    oLine.Save(Get_TrxName());
                            //}
                        }
                    }	//	No Order
                }	//	PO Matching

                #region Calculate Foreign Cost for Average PO
                //try
                //{
                //    if (!IsSOTrx() && !IsReturnTrx() && sLine.GetC_OrderLine_ID() > 0) // for MR against PO
                //    {
                //        MProduct product1 = new MProduct(GetCtx(), sLine.GetM_Product_ID(), Get_Trx());
                //        MOrderLine orderLine = new MOrderLine(GetCtx(), lines[lineIndex].GetC_OrderLine_ID(), null);
                //        MOrder order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), Get_Trx());
                //        if (product1 != null && product1.GetProductType() == "I" && product1.GetM_Product_ID() > 0) // for Item Type product
                //        {
                //            //if (!MCostForeignCurrency.InsertForeignCostAveragePO(GetCtx(), order, orderLine, sLine, Get_Trx()))
                //            //{
                //            //    Get_Trx().Rollback();
                //            //    log.Severe("Error occured during updating/inserting M_Cost_ForeignCurrency against Average PO.");
                //            //    _processMsg = "Could not update Foreign Currency Cost";
                //            //    return DocActionVariables.STATUS_INVALID;
                //            //}
                //        }
                //    }
                //}
                //catch (Exception) { }
                #endregion

                //Enhanced By amit
                if (client.IsCostImmediate())
                {
                    #region Manage Cost Queue
                    bool isCostAdjustableOnLost = false;
                    int count = 0;
                    String costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), sLine.GetM_Product_ID(), Get_Trx());

                    productCQ = new MProduct(GetCtx(), sLine.GetM_Product_ID(), Get_Trx());
                    if (sLine.GetM_Product_ID() > 0 && productCQ.GetProductType() == "I") // for Item Type product
                    {
                        //if (GetReversalDoc_ID() > 0 || (GetDescription() != null && GetDescription().Contains("RC")))
                        //{
                        //    // do not update current cost price during reversal, this time reverse doc contain same amount which are on original document
                        //}
                        //else
                        //{
                        // get price from m_cost (Current Cost Price)
                        currentCostPrice = 0;
                        if ((!IsSOTrx() && IsReturnTrx()) || (IsSOTrx() && !IsReturnTrx())) // Return to vendor / Shipment
                        {
                            currentCostPrice = MCost.GetproductCosts(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID());
                        }
                        else // Material Receipt / Customer Return
                        {
                            currentCostPrice = MCost.GetproductCostAndQtyMaterial(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                               sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID(), false);
                        }
                        DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice = " + currentCostPrice +
                                                          @" WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());

                        // update Price Difference as (PO Cost - GRN Current Cost Price)) on match PO
                        if (Matchpo != null && Matchpo.Get_ID() > 0)
                        {
                            DB.ExecuteQuery(@"UPDATE M_MatchPO SET PriceDifference = " + (currentCostPrice == 0 ? 0 : (Matchpo.GetPricePO() - currentCostPrice))
                                + ", CurrentCostPrice = " + currentCostPrice + @" WHERE M_MatchPO_ID = " + Matchpo.GetM_MatchPO_ID(), null, Get_Trx());
                        }

                        _partner = new MBPartner(GetCtx(), GetC_BPartner_ID(), null);
                        orderLine = new MOrderLine(GetCtx(), lines[lineIndex].GetC_OrderLine_ID(), null);
                        if (!IsSOTrx() && !IsReturnTrx()) // Material Receipt
                        {
                            bool isUpdatePostCurrentcostPriceFromMR = MCostElement.IsPOCostingmethod(GetCtx(), GetAD_Client_ID(), productCQ.GetM_Product_ID(), Get_Trx());
                            if (orderLine == null || orderLine.GetC_OrderLine_ID() == 0)  // MR without PO
                            {
                                #region MR without PO
                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), productCQ, sLine.GetM_AttributeSetInstance_ID(),
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
                                    //if (costingMethod != "")
                                    //{
                                    //    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                    //                   sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), 0, sLine.GetM_InOutLine_ID(), costingMethod,
                                    //                   GetM_Warehouse_ID(), false, Get_Trx());
                                    //    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                    //                       @" WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    //}

                                    currentCostPrice = MCost.GetproductCostAndQtyMaterial(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                                                         sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID(), false);
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                      @" END , IsCostImmediate = 'Y' , 
                                                     PostCurrentCostPrice = CASE WHEN 1 = " + (isUpdatePostCurrentcostPriceFromMR ? 1 : 0) +
                                                     @" THEN " + currentCostPrice + @" ELSE PostCurrentCostPrice END 
                                                WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                                #endregion
                            }
                            else if (orderLine != null && orderLine.GetC_Order_ID() > 0) // MR with PO
                            {
                                #region MR with PO
                                // check IsCostAdjustmentOnLost exist on product 
                                sql.Clear();
                                sql.Append(@"SELECT COUNT(AD_Column_ID) FROM AD_Column WHERE IsActive = 'Y' AND 
                                       AD_Table_ID =  ( SELECT AD_Table_ID FROM AD_Table WHERE IsActive = 'Y' AND TableName LIKE 'M_Product' ) 
                                        AND ColumnName LIKE 'IsCostAdjustmentOnLost' ");
                                count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));

                                if (count > 0)
                                {
                                    isCostAdjustableOnLost = productCQ.IsCostAdjustmentOnLost();
                                }

                                //MOrder order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), null);
                                string Ord_DocStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM C_Order WHERE C_Order_ID ="
                                                + orderLine.GetC_Order_ID(), null, Get_Trx()));

                                if (Ord_DocStatus != "VO")
                                {
                                    if (orderLine.GetQtyOrdered() == 0)
                                        continue;
                                }

                                Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                                Decimal ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();

                                amt = 0;
                                if (isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && Ord_DocStatus != "VO")
                                {
                                    if (sLine.GetMovementQty() > 0)
                                        amt = ProductOrderLineCost;
                                    else
                                        amt = Decimal.Negate(ProductOrderLineCost);
                                }
                                else if (!isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && Ord_DocStatus != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), sLine.GetMovementQty());
                                }
                                else if (Ord_DocStatus != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), sLine.GetMovementQty());
                                }
                                else if (Ord_DocStatus == "VO")
                                {
                                    amt = Decimal.Multiply(ProductOrderPriceActual, sLine.GetQtyEntered());
                                }

                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), productCQ, sLine.GetM_AttributeSetInstance_ID(),
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
                                    //if (costingMethod != "")
                                    //{
                                    //    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                    //                   sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), 0, sLine.GetM_InOutLine_ID(), costingMethod,
                                    //                   GetM_Warehouse_ID(), false, Get_Trx());
                                    //    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                    //                       @"  , IsCostImmediate = 'Y'
                                    //                        WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    //}
                                    //else
                                    //{
                                    currentCostPrice = MCost.GetproductCostAndQtyMaterial(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                                                         sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID(), false);
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                     @" END , IsCostImmediate = 'Y' ,
                                                      PostCurrentCostPrice = CASE WHEN 1 = " + (isUpdatePostCurrentcostPriceFromMR ? 1 : 0) +
                                                      @" THEN " + currentCostPrice + @" ELSE PostCurrentCostPrice END 
                                                    WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    //}

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
                                                currentCostPrice = MCost.GetproductCostAndQtyMaterial(GetAD_Client_ID(), GetAD_Org_ID(),
                                                                   matchedInvoice[mi].GetM_Product_ID(), matchedInvoice[mi].GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID(), false);
                                                DB.ExecuteQuery("UPDATE M_MatchInv SET CurrentCostPrice = " + currentCostPrice +
                                                                 @" WHERE M_MatchInv_ID = " + matchedInvoice[mi].GetM_MatchInv_ID(), null, Get_Trx());

                                            }

                                            // calculate invoice line costing after calculating costing of linked MR line 
                                            MInvoiceLine invoiceLine = new MInvoiceLine(GetCtx(), matchedInvoice[mi].GetC_InvoiceLine_ID(), Get_Trx());
                                            Decimal ProductLineCost = invoiceLine.GetProductLineCost(invoiceLine);
                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), productCQ, matchedInvoice[mi].GetM_AttributeSetInstance_ID(),
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
                                                DB.ExecuteQuery("UPDATE C_InvoiceLine SET IsCostImmediate = 'Y' WHERE C_InvoiceLine_ID = " + matchedInvoice[mi].GetC_InvoiceLine_ID(), null, Get_Trx());

                                                if (matchedInvoice[mi].Get_ColumnIndex("PostCurrentCostPrice") >= 0)
                                                {
                                                    // get cost from Product Cost after cost calculation
                                                    currentCostPrice = MCost.GetproductCostAndQtyMaterial(GetAD_Client_ID(), GetAD_Org_ID(),
                                                                     matchedInvoice[mi].GetM_Product_ID(), matchedInvoice[mi].GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID(), false);
                                                    matchedInvoice[mi].SetPostCurrentCostPrice(currentCostPrice);
                                                }
                                                matchedInvoice[mi].SetIsCostImmediate(true);
                                                matchedInvoice[mi].Save(Get_Trx());

                                                // update the Post current price after Invoice receving on inoutline
                                                if (!isUpdatePostCurrentcostPriceFromMR)
                                                {
                                                    DB.ExecuteQuery("UPDATE M_InoutLine SET PostCurrentCostPrice =  " + currentCostPrice +
                                                                 @"  WHERE M_InoutLine_ID = " + matchedInvoice[mi].GetM_InOutLine_ID(), null, Get_Trx());
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

                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                break;

                            Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);

                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), productCQ, sLine.GetM_AttributeSetInstance_ID(),
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
                                if (costingMethod != "")
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(), sLine.GetM_Product_ID(),
                                        sLine.GetM_AttributeSetInstance_ID(), 0, sLine.GetM_InOutLine_ID(), costingMethod, GetM_Warehouse_ID(), false, Get_Trx());
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                                                         @"  , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                                else
                                {
                                    currentCostPrice = MCost.GetproductCostAndQtyMaterial(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                                         sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID(), false);
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                         @" END , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                            }
                            #endregion
                        }
                        else if (IsSOTrx() && !IsReturnTrx())  // shipment
                        {
                            #region shipment
                            if (GetC_Order_ID() <= 0)
                            {
                                break;
                            }

                            if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0) { break; }

                            Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);

                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), productCQ, sLine.GetM_AttributeSetInstance_ID(),
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
                                if (costingMethod != "")
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                        sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), 0, sLine.GetM_InOutLine_ID(), costingMethod,
                                        GetM_Warehouse_ID(), true, Get_Trx());
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                                                         @"  , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                                else
                                {
                                    currentCostPrice = MCost.GetproductCosts(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                   sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID());
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                         @" END , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                            }
                            #endregion
                        }
                        else if (!IsSOTrx() && IsReturnTrx()) // Return To Vendor
                        {
                            if (GetOrig_Order_ID() == 0 || orderLine == null || orderLine.GetC_OrderLine_ID() == 0)
                            {
                                #region Return To Vendor -- without order refernce
                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), productCQ, sLine.GetM_AttributeSetInstance_ID(),
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
                                    //if (costingMethod != "")
                                    //{
                                    //    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                    //        sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), 0, sLine.GetM_InOutLine_ID(), costingMethod,
                                    //        GetM_Warehouse_ID(), true, Get_Trx());
                                    //    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                    //                                         @"  , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    //}
                                    //else
                                    {
                                        currentCostPrice = MCost.GetproductCosts(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                                            sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID());
                                        DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                          @" END , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    }
                                }
                                #endregion
                            }
                            else if (orderLine != null && orderLine.GetC_Order_ID() > 0)
                            {
                                #region Return To Vendor -- with order refernce
                                //MOrder order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), null);
                                string Ord_DocStatus = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocStatus FROM C_Order WHERE C_Order_ID ="
                                                + orderLine.GetC_Order_ID(), null, Get_Trx()));
                                if (Ord_DocStatus != "VO")
                                {
                                    if (orderLine.GetQtyOrdered() == 0)
                                        continue;
                                }
                                // check IsCostAdjustmentOnLost exist on product 
                                sql.Clear();
                                sql.Append(@"SELECT COUNT(AD_Column_ID) FROM AD_Column WHERE IsActive = 'Y' AND 
                                       AD_Table_ID =  ( SELECT AD_Table_ID FROM AD_Table WHERE IsActive = 'Y' AND TableName LIKE 'M_Product' ) 
                                       AND ColumnName LIKE 'IsCostAdjustmentOnLost' ");
                                count = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));

                                if (count > 0)
                                {
                                    isCostAdjustableOnLost = productCQ.IsCostAdjustmentOnLost();
                                }

                                Decimal ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                                Decimal ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();

                                amt = 0;
                                if (isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && Ord_DocStatus != "VO")
                                {
                                    // Cost Adjustment case
                                    if (sLine.GetMovementQty() < 0)
                                        amt = ProductOrderLineCost;
                                    else
                                        amt = Decimal.Negate(ProductOrderLineCost);
                                }
                                else if (!isCostAdjustableOnLost && sLine.GetMovementQty() < orderLine.GetQtyOrdered() && Ord_DocStatus != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(sLine.GetMovementQty()));
                                }
                                else if (Ord_DocStatus != "VO")
                                {
                                    amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(sLine.GetMovementQty()));
                                }
                                else if (Ord_DocStatus == "VO")
                                {
                                    amt = Decimal.Multiply(ProductOrderPriceActual, Decimal.Negate(sLine.GetQtyEntered()));
                                }

                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), productCQ, sLine.GetM_AttributeSetInstance_ID(),
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
                                    //     if (!String.IsNullOrEmpty(costingMethod))
                                    //     {
                                    //         currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                                    //sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), 0, sLine.GetM_InOutLine_ID(), costingMethod,
                                    //GetM_Warehouse_ID(), true, Get_Trx());
                                    //         DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice =  " + currentCostPrice +
                                    //                                              @"  , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    //     }
                                    //     else
                                    //{
                                    currentCostPrice = MCost.GetproductCosts(sLine.GetAD_Client_ID(), sLine.GetAD_Org_ID(),
                           sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(), Get_Trx(), GetM_Warehouse_ID());
                                    DB.ExecuteQuery("UPDATE M_InoutLine SET CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                                                                     @" END , IsCostImmediate = 'Y' WHERE M_InoutLine_ID = " + sLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    //}
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
                    if ((product != null && product.GetProductType() == X_M_Product.PRODUCTTYPE_Item && product.IsCreateAsset() && sLine.GetMovementQty() > 0
                       && !IsReversal() && !IsReturnTrx() && !IsSOTrx() && sLine.GetA_Asset_ID() == 0) ||
                       (Env.IsModuleInstalled("VA077_") && product != null
                       && (product.GetProductType() == X_M_Product.PRODUCTTYPE_Item || product.GetProductType() == X_M_Product.PRODUCTTYPE_Service)
                       && product.IsCreateAsset() && sLine.GetMovementQty() > 0
                       && !IsReversal() && !IsReturnTrx() && IsSOTrx() && sLine.GetA_Asset_ID() == 0))
                    {
                        log.Fine("Asset");
                        Info.Append("@A_Asset_ID@: ");
                        int noAssets = (int)sLine.GetMovementQty();

                        // Check Added only run when Product is one Asset Per UOM
                        if (product.IsOneAssetPerUOM())
                        {
                            for (int i = 0; i < noAssets; i++)
                            {
                                if (i > 0)
                                    Info.Append(" - ");
                                //int deliveryCount = i + 1;
                                //if (product.IsOneAssetPerUOM())
                                //    deliveryCount = 0;
                                MAsset asset = new MAsset(this, sLine, 0);
                                if (!asset.Save(Get_TrxName()))
                                {
                                    _processMsg = "Could not create Asset";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                                else
                                {
                                    if (Env.IsModuleInstalled("VA077_"))
                                        asset.SetName(asset.GetName());
                                    else
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
                                MAsset asset = new MAsset(this, sLine, noAssets);
                                if (!asset.Save(Get_TrxName()))
                                {
                                    _processMsg = "Could not create Asset";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                                else
                                {
                                    if (Env.IsModuleInstalled("VA077_"))
                                        asset.SetName(asset.GetName());
                                    else
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
                        if (sLine.GetM_Product_ID() > 0 &&
                            ((IsSOTrx() && !IsReturnTrx()) || (!IsSOTrx() && IsReturnTrx())))
                        {
                            sql.Clear();
                            sql.Append(@"SELECT * FROM va024_t_obsoleteinventory
                                     WHERE ad_client_id    = " + GetAD_Client_ID() + " AND ad_org_id = " + GetAD_Org_ID() +
                                     "   AND M_Product_ID = " + sLine.GetM_Product_ID() +
                                       " AND NVL(M_AttributeSetInstance_ID , 0) = " + sLine.GetM_AttributeSetInstance_ID());
                            //" AND M_Warehouse_Id = " + GetM_Warehouse_ID();
                            if (GetDescription() != null && !GetDescription().Contains("{->"))
                            {
                                sql.Append(" AND va024_isdelivered = 'N' ");
                            }
                            sql.Append(" ORDER BY va024_t_obsoleteinventory_id DESC");
                            DataSet dsObsoleteInventory = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                            if (dsObsoleteInventory != null && dsObsoleteInventory.Tables.Count > 0 && dsObsoleteInventory.Tables[0].Rows.Count > 0)
                            {
                                Decimal remainigQty = sLine.GetQtyEntered();
                                MTable tbl = new MTable(GetCtx(), tableId, null);
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
                if (sLine.GetC_OrderLine_ID() != 0)
                {
                    try
                    {
                        string PaymentBaseType = "";
                        if (countVA026)
                        {
                            MOrderLine ordLine = new MOrderLine(GetCtx(), sLine.GetC_OrderLine_ID(), Get_TrxName());
                            MOrder ord = new MOrder(GetCtx(), ordLine.GetC_Order_ID(), Get_TrxName());
                            if (tableId1 <= 0 && ord.GetPaymentMethod() == "L")
                            {
                                _processMsg = "Could not Update Letter of Credit";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            MTable tbl = new MTable(GetCtx(), tableId1, null);
                            // Change By Mohit- to pick payment base type from VA009_PaymentMthod----------------------
                            PaymentBaseType = Util.GetValueOfString(DB.ExecuteScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod  WHERE VA009_PaymentMethod_ID=" + ord.GetVA009_PaymentMethod_ID(), null, null));
                            //End change--------------------------------------------------------------------------------
                            if (PaymentBaseType == "L")
                            {

                                if (ord.IsSOTrx())
                                {
                                    VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(VA026_LCDetail_ID)  FROM VA026_LCDetail 
                                                            WHERE IsActive = 'Y' AND DocStatus IN ('CO' , 'CL')  AND
                                                            VA026_Order_ID =" + ord.GetC_Order_ID(), null, Get_Trx()));
                                    // Check SO Detail tab of Letter of Credit
                                    if (VA026_LCDetail_ID == 0)
                                    {
                                        VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(lc.VA026_LCDetail_ID)  FROM VA026_LCDetail lc
                                                        INNER JOIN VA026_SODetail sod ON sod.VA026_LCDetail_ID = lc.VA026_LCDetail_ID 
                                                            WHERE sod.IsActive = 'Y' AND lc.IsActive = 'Y' AND lc.DocStatus IN ('CO' , 'CL')  AND
                                                            sod.C_Order_ID =" + ord.GetC_Order_ID(), null, Get_Trx()));
                                    }
                                }
                                else if (!ord.IsSOTrx() && !ord.IsReturnTrx())
                                {
                                    VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(VA026_LCDetail_ID)  FROM VA026_LCDetail 
                                                            WHERE IsActive = 'Y' AND DocStatus IN ('CO' , 'CL')  AND
                                                            c_order_id =" + ord.GetC_Order_ID(), null, Get_Trx()));
                                    // Check PO Detail tab of Letter of Credit
                                    if (VA026_LCDetail_ID == 0)
                                    {
                                        VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT MIN(lc.VA026_LCDetail_ID)  FROM VA026_LCDetail lc
                                                        INNER JOIN VA026_PODetail sod ON sod.VA026_LCDetail_ID = lc.VA026_LCDetail_ID 
                                                            WHERE sod.IsActive = 'Y' AND lc.IsActive = 'Y' AND lc.DocStatus IN ('CO' , 'CL')  AND
                                                            sod.C_Order_ID =" + ord.GetC_Order_ID(), null, Get_Trx()));
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
                                        po.Set_Value("M_InOut_ID", GetM_InOut_ID());
                                    }
                                    else if (Util.GetValueOfString(po.Get_Value("VA026_ReceievedIssue")) == "R")
                                    {
                                        po.Set_Value("VA026_ShipmentDate", GetDateAcct());
                                        po.Set_Value("Va026_InOut_ID", GetM_InOut_ID());
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
                //if (IsSOTrx() && !IsReturnTrx() && sLine.GetA_Asset_ID() > 0)
                //{
                //    DB.ExecuteQuery(@"UPDATE A_Asset Set IsInposession = 'N' 
                //                WHERE IsInposession = 'Y' AND A_Asset_ID = " + sLine.GetA_Asset_ID(), null, Get_Trx());
                //}
                #endregion

            }	//	for all lines
            #endregion

            // By Amit For Foreign cost
            //try
            //{
            //    if (!IsSOTrx() && !IsReturnTrx()) // for MR against PO
            //    {
            //        if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_InoutLine_ID) FROM M_InoutLine WHERE IsFutureCostCalculated = 'N' AND M_InOut_ID = " + GetM_InOut_ID(), null, Get_Trx())) <= 0)
            //        {
            //            int no = Util.GetValueOfInt(DB.ExecuteQuery("UPDATE M_Inout Set IsFutureCostCalculated = 'Y' WHERE M_Inout_ID = " + GetM_InOut_ID(), null, Get_Trx()));
            //        }
            //    }
            //}
            //catch (Exception) { }
            //end

            //	Counter Documents
            try
            {
                //if counter document not created due to some error not to completed the record
                //and it should return the error message to the user
                MInOut counter = CreateCounterDoc();
                if (counter != null)
                {
                    Info.Append(" - @CounterDoc@: @M_InOut_ID@=").Append(counter.GetDocumentNo());
                }
            }
            catch (Exception e)
            {
                //if get exception while creating counter document then return the message to the user
                //Info.Append(" - @CounterDoc@: ").Append(e.Message.ToString());
                _processMsg = e.Message.ToString();
                return DocActionVariables.STATUS_INPROGRESS;
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
                        MOrder order = new MOrder(GetCtx(), GetC_Order_ID(), Get_Trx());
                        MOrder ref_order = new MOrder(GetCtx(), order.GetRef_Order_ID(), Get_Trx());
                        MInOut ret_Shipment = CreateShipment(ref_order, this, GetMovementDate(),
                                    true, false, GetM_Warehouse_ID(), GetMovementDate(), Get_Trx());

                        // VIS0060: Commit the Transaction here to call workflow to complete Shipment.
                        Get_Trx().Commit();
                        // Show respective message if shipment completed or not completed
                        // Done by Rakesh Kumar On 29/Apr/2021 suggested by Mandeep and Bharat

                        // VIS0060: Work done to call workflow to complete Shipment.
                        string result = DocumentEngine.CompleteOrReverse(GetCtx(), MInOut.Table_Name, MInOut.Table_ID, ret_Shipment.GetM_InOut_ID(), 109, DOCACTION_Complete);
                        if (!String.IsNullOrEmpty(result))
                        {
                            _processMsg = Msg.GetMsg(GetCtx(), "VIS_ShipmentNotCompleted") + ": " + ret_Shipment.GetProcessMsg() + " - @DocumentNo@: " + ret_Shipment.GetDocumentNo();
                        }
                        else
                        {
                            _processMsg = Msg.GetMsg(GetCtx(), "VIS_ShipmentGenerated") + ": " + ret_Shipment.GetDocumentNo();
                        }

                        //if (ret_Shipment.CompleteIt() == "CO")
                        //{
                        //    ret_Shipment.SetDocStatus(DOCACTION_Complete);
                        //    _processMsg = Msg.GetMsg(GetCtx(), "VIS_ShipmentGenerated") + ": " + ret_Shipment.GetDocumentNo();
                        //}
                        //else
                        //{
                        //    _processMsg = Msg.GetMsg(GetCtx(), "VIS_ShipmentNotCompleted") + ": " + ret_Shipment.GetProcessMsg() + " - @DocumentNo@: " + ret_Shipment.GetDocumentNo();
                        //}

                        // Set References
                        ret_Shipment.SetRef_ShipMR_ID(GetM_InOut_ID());
                        ret_Shipment.Save(Get_Trx());
                        SetRef_ShipMR_ID(ret_Shipment.GetM_InOut_ID());
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

            // Set the document number from completede document sequence after completed (if needed)
            SetCompletedDocumentNo();

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

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") > 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MSequence.GetDocumentNo(GetC_DocType_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
                }
            }
        }

        /// <summary>
        /// Overwrite the document date based on setting on Document Type
        /// </summary>
        private void SetCompletedDocumentDate()
        {
            // if Reversal document then no need to get Document no from Completed sequence
            if (IsReversal())
            {
                return;
            }

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetMovementDate(DateTime.Now.Date);
                if (GetDateAcct().Value.Date < GetMovementDate().Value.Date)
                {
                    SetDateAcct(GetMovementDate());

                    //	Std Period open?
                    if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
                    {
                        throw new Exception("@PeriodClosed@");
                    }
                }
            }
        }

        /// <summary>
        /// Update Values on Asset
        /// VIS0060: 16-Feb-2022
        /// </summary>
        /// <param name="line">Invoice Line</param>
        /// <returns>False, if not updated</returns>
        private bool UpdateAssetValues(MInOutLine sLine)
        {
            StringBuilder sql = new StringBuilder();
            MAsset ast = new MAsset(GetCtx(), sLine.GetA_Asset_ID(), Get_TrxName());
            // VIS0060: Set Disposal on Asset only if full Asset is Sale.
            if (sLine.GetMovementQty().Equals(sLine.GetVAFAM_Quantity()))
            {
                ast.SetIsDisposed(true);
                ast.SetAssetDisposalDate(GetDateAcct());
            }
            else if (IsReversal())
            {
                ast.SetIsDisposed(false);
                ast.SetAssetDisposalDate(null);
            }
            ast.SetQty(decimal.Subtract(Util.GetValueOfDecimal(ast.Get_Value("Qty")), sLine.GetMovementQty()));

            // VIS0060: Set Disposal Qty and Asset Values on related Asset.
            if (Env.IsModuleInstalled("VAFAM_") && sLine.Get_ColumnIndex("VAFAM_AssetValue") >= 0)
            {
                ast.Set_Value("VAFAM_DisposeQty", decimal.Add(Util.GetValueOfDecimal(ast.Get_Value("VAFAM_DisposeQty")), sLine.GetMovementQty()));
                ast.Set_Value("VAFAM_AssetGrossValue", decimal.Subtract(Util.GetValueOfDecimal(ast.Get_Value("VAFAM_AssetGrossValue")), sLine.GetVAFAM_AssetValue()));
                ast.Set_Value("VAFAM_SLMDepreciation", decimal.Subtract(Util.GetValueOfDecimal(ast.Get_Value("VAFAM_SLMDepreciation")), sLine.GetVAFAM_DepAmount()));

                if (!ast.Save(Get_TrxName()))
                {
                    Get_TrxName().Rollback();
                    _processMsg = "AssetNotUpdated" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                    return false;
                }

                // Create entry on Asset Disposal Details.
                if (!string.IsNullOrEmpty(CreateDisposalDetailsEntry(sLine.GetM_Product_ID(), sLine.GetC_Charge_ID(),
                    sLine.GetM_InOutLine_ID(), sLine.GetA_Asset_ID(), sLine.GetMovementQty(), GetDateAcct(), sLine.GetVAFAM_AssetValue(), sLine.GetVAFAM_DepAmount())))
                {
                    return false;
                }

                MAsset asset = null;
                decimal cmpQty = 0, assetCost = 0, chargedDepreciation = 0;

                // VIS0060: In Case of Parent Asset, adjust Quantity and Value on Component Assets.
                if (ast.Get_ColumnIndex("VAFAM_HasComponent") >= 0 && Util.GetValueOfBool(ast.Get_Value("VAFAM_HasComponent")))
                {
                    DataSet dsDsp = null;
                    DataSet dscmp = DB.ExecuteDataset(@"SELECT VAFAM_AssetComponent_ID, VAFAM_Quantity FROM VAFAM_ComponentAsset WHERE A_Asset_ID = "
                            + sLine.GetA_Asset_ID(), null, Get_TrxName());

                    if (dscmp != null && dscmp.Tables.Count > 0 && dscmp.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dscmp.Tables[0].Rows.Count; j++)
                        {
                            asset = new MAsset(GetCtx(), Util.GetValueOfInt(dscmp.Tables[0].Rows[j]["VAFAM_AssetComponent_ID"]), Get_Trx());
                            cmpQty = Decimal.Multiply(sLine.GetMovementQty(), Util.GetValueOfDecimal(dscmp.Tables[0].Rows[j]["VAFAM_Quantity"]));

                            // In Case of Reversal get Gross Value, Dep Value and Quantity of component from Disposal details.
                            if (IsReversal())
                            {
                                sql.Clear();
                                sql.Append(@"SELECT VAFAM_GrossValDispAsset, VAFAM_DisposedQty, VAFAM_AccDepforDispAsset FROM VAFAM_DisposalDetails WHERE A_Asset_ID = "
                                        + asset.Get_ID() + " AND M_InOutLine_ID=" + sLine.GetReversalDoc_ID());
                                dsDsp = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsDsp != null && dsDsp.Tables.Count > 0 && dsDsp.Tables[0].Rows.Count > 0)
                                {
                                    assetCost = Decimal.Negate(Util.GetValueOfDecimal(dsDsp.Tables[0].Rows[0]["VAFAM_GrossValDispAsset"]));
                                    chargedDepreciation = Decimal.Negate(Util.GetValueOfDecimal(dsDsp.Tables[0].Rows[0]["VAFAM_AccDepforDispAsset"]));
                                    cmpQty = Decimal.Negate(Util.GetValueOfDecimal(dsDsp.Tables[0].Rows[0]["VAFAM_DisposedQty"]));

                                    asset.Set_Value("VAFAM_AssetGrossValue", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_AssetGrossValue")) - assetCost);
                                    asset.SetVAFAM_SLMDepreciation(asset.GetVAFAM_SLMDepreciation() - chargedDepreciation);

                                    asset.SetQty(Util.GetValueOfDecimal(asset.Get_Value("Qty")) - cmpQty);
                                    asset.Set_Value("VAFAM_DisposeQty", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_DisposeQty")) + cmpQty);
                                }
                                else
                                {
                                    continue;
                                }

                                asset.SetIsDisposed(false);
                                asset.SetAssetDisposalDate(null);
                            }
                            else if (Util.GetValueOfDecimal(asset.Get_Value("Qty")) > 0)
                            {
                                if (cmpQty > Util.GetValueOfDecimal(asset.Get_Value("Qty")))
                                {
                                    cmpQty = Util.GetValueOfDecimal(asset.Get_Value("Qty"));
                                }

                                assetCost = Decimal.Multiply(Decimal.Divide(Util.GetValueOfDecimal(asset.Get_Value("VAFAM_AssetGrossValue")), Util.GetValueOfDecimal(asset.Get_Value("Qty"))), cmpQty);
                                chargedDepreciation = Decimal.Multiply(Decimal.Divide(Util.GetValueOfDecimal(asset.Get_Value("VAFAM_SLMDepreciation")), Util.GetValueOfDecimal(asset.Get_Value("Qty"))), cmpQty);

                                asset.Set_Value("VAFAM_AssetGrossValue", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_AssetGrossValue")) - assetCost);
                                asset.SetVAFAM_SLMDepreciation(asset.GetVAFAM_SLMDepreciation() - chargedDepreciation);

                                if (ast.IsDisposed())
                                {
                                    asset.SetIsDisposed(true);
                                    asset.SetAssetDisposalDate(GetDateAcct());

                                    asset.Set_Value("VAFAM_DisposeQty", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_DisposeQty")) + Util.GetValueOfDecimal(asset.Get_Value("Qty")));
                                    asset.SetQty(0);
                                }
                                else
                                {
                                    if (cmpQty.Equals(Util.GetValueOfDecimal(asset.Get_Value("Qty"))))
                                    {
                                        asset.SetIsDisposed(true);
                                        asset.SetAssetDisposalDate(GetDateAcct());
                                    }
                                    else
                                    {
                                        asset.SetIsDisposed(false);
                                        asset.SetAssetDisposalDate(null);
                                    }

                                    asset.SetQty(Util.GetValueOfDecimal(asset.Get_Value("Qty")) - cmpQty);
                                    asset.Set_Value("VAFAM_DisposeQty", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_DisposeQty")) + cmpQty);
                                }
                            }
                            else
                            {
                                continue;
                            }

                            if (!asset.Save())
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                _processMsg = Msg.GetMsg(GetCtx(), "AssetNotSaved") + ": " + asset.GetName()
                                    + (pp != null ? " - " + Msg.GetMsg(GetCtx(), pp.GetValue()) + " " + pp.ToString() : "");
                                Get_TrxName().Rollback();
                                return false;
                            }

                            // Create entry on Asset Disposal Details.
                            if (!string.IsNullOrEmpty(CreateDisposalDetailsEntry(asset.GetM_Product_ID(), Util.GetValueOfInt(asset.Get_Value("C_Charge_ID")),
                                sLine.GetM_InOutLine_ID(), Util.GetValueOfInt(dscmp.Tables[0].Rows[j]["VAFAM_AssetComponent_ID"]), cmpQty, GetDateAcct(), assetCost, chargedDepreciation)))
                            {
                                Get_TrxName().Rollback();
                                return false;
                            }
                        }
                    }
                }

                // VIS0060: In Case of Component Asset, adjust Value on Parent Asset.
                else if (Util.GetValueOfBool(ast.Get_Value("VAFAM_IsComponent")))
                {
                    assetCost = sLine.GetVAFAM_AssetValue();
                    chargedDepreciation = sLine.GetVAFAM_DepAmount();

                    asset = new MAsset(GetCtx(), Util.GetValueOfInt(ast.Get_Value("VAFAM_ParentAsset_ID")), Get_Trx());
                    asset.Set_Value("VAFAM_AssetGrossValue", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_AssetGrossValue")) - assetCost);
                    asset.SetVAFAM_SLMDepreciation(asset.GetVAFAM_SLMDepreciation() - chargedDepreciation);

                    // Set Asset to disposed if all the components are disposed.
                    sql.Clear();
                    sql.Append("SELECT COUNT(ast.A_Asset_ID) FROM VAFAM_ComponentAsset cmp INNER JOIN A_Asset ast ON ast.A_Asset_ID = cmp.VAFAM_AssetComponent_ID" +
                                    " WHERE cmp.A_Asset_ID =" + Util.GetValueOfInt(ast.Get_Value("VAFAM_ParentAsset_ID")) + " AND ast.IsDisposed = 'N'");
                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) == 0)
                    {
                        asset.SetIsDisposed(true);
                        asset.SetAssetDisposalDate(GetDateAcct());

                        cmpQty = Util.GetValueOfDecimal(asset.Get_Value("Qty"));
                        asset.Set_Value("VAFAM_DisposeQty", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_DisposeQty")) + cmpQty);
                        asset.SetQty(0);
                    }

                    // In Case of Reversal get Quantity of Asset from Disposal details.
                    if (IsReversal())
                    {
                        sql.Clear();
                        sql.Append(@"SELECT VAFAM_DisposedQty FROM VAFAM_DisposalDetails WHERE A_Asset_ID = " + Util.GetValueOfInt(ast.Get_Value("VAFAM_ParentAsset_ID"))
                            + " AND M_InOutLine_ID=" + sLine.GetReversalDoc_ID());
                        cmpQty = Decimal.Negate(Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())));

                        asset.SetQty(Util.GetValueOfDecimal(asset.Get_Value("Qty")) - cmpQty);
                        asset.Set_Value("VAFAM_DisposeQty", Util.GetValueOfDecimal(asset.Get_Value("VAFAM_DisposeQty")) + cmpQty);

                        asset.SetIsDisposed(false);
                        asset.SetAssetDisposalDate(null);
                    }

                    if (!asset.Save())
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _processMsg = Msg.GetMsg(GetCtx(), "AssetNotSaved") + ": " + asset.GetName()
                            + (pp != null ? " - " + Msg.GetMsg(GetCtx(), pp.GetValue()) + " " + pp.ToString() : "");
                        Get_TrxName().Rollback();
                        return false;
                    }

                    // Create entry on Asset Disposal Details.
                    if (!string.IsNullOrEmpty(CreateDisposalDetailsEntry(asset.GetM_Product_ID(), Util.GetValueOfInt(asset.Get_Value("C_Charge_ID")),
                    sLine.GetM_InOutLine_ID(), Util.GetValueOfInt(ast.Get_Value("VAFAM_ParentAsset_ID")), cmpQty, GetDateAcct(), assetCost, chargedDepreciation)))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!ast.Save(Get_TrxName()))
                {
                    _processMsg = "AssetNotUpdated" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// to save record on disposal details tab against selected asset
        /// </summary>
        /// <param name="M_Product_ID"> Product ID</param>
        /// <param name="C_Charge_ID">Charge ID</param>
        /// <param name="LineID">Shipment Line ID</param>
        /// <param name="Asset_ID">Asset ID</param>
        /// <param name="qty">Qty Entered</param>
        /// <param name="dateAcct">Account Date</param>
        /// <param name="grossValue">Written Down Amount</param>
        /// <param name="depAmt">Depreciation Amount</param>
        /// <returns>If saved then return empty otherwise Error Msg</returns>
        private string CreateDisposalDetailsEntry(int M_Product_ID, int C_Charge_ID, int LineID, int Asset_ID, decimal qty, DateTime? dateAcct, decimal grossValue, decimal depAmt)
        {
            MTable table = MTable.Get(GetCtx(), "VAFAM_DisposalDetails");
            PO disDetails = table.GetPO(GetCtx(), 0, Get_TrxName());
            disDetails.SetAD_Client_ID(GetAD_Client_ID());
            disDetails.SetAD_Org_ID(GetAD_Org_ID());
            disDetails.Set_ValueNoCheck("A_Asset_ID", Asset_ID);
            if (M_Product_ID > 0)
                disDetails.Set_Value("M_Product_ID", M_Product_ID);
            if (C_Charge_ID > 0)
                disDetails.Set_Value("C_Charge_ID", C_Charge_ID);
            disDetails.Set_Value("VAFAM_DisposedQty", qty);
            disDetails.Set_Value("VAFAM_GrossValDispAsset", grossValue);
            disDetails.Set_Value("VAFAM_AccDepforDispAsset", depAmt);

            if (disDetails.Get_ColumnIndex("M_InOutLine_ID") >= 0)
            {
                disDetails.Set_Value("M_InOutLine_ID", LineID);
            }

            disDetails.Set_Value("IsDisposed", true);
            disDetails.Set_Value("DateTrx", dateAcct);
            if (!disDetails.Save())
            {
                Get_Trx().Rollback();
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                {
                    _processMsg = pp.GetName();
                    log.Info(_processMsg);
                    return _processMsg;
                }
                else
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VAFAM_DisDetailsNotSaved");
                    log.Info(_processMsg);
                    return _processMsg;
                }
            }
            return string.Empty;
        }

        // Amit not used 24-12-2015
        private void updateCostQueue(MProduct product, int M_ASI_ID, MAcctSchema mas,
            int Org_ID, MCostElement ce, decimal movementQty)
        {
            //MCostQueue[] cQueue = MCostQueue.GetQueue(product1, sLine.GetM_AttributeSetInstance_ID(), acctSchema, GetAD_Org_ID(), costElement, null);
            MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID, mas, Org_ID, ce, null);
            if (cQueue != null && cQueue.Length > 0)
            {
                Decimal qty = movementQty;
                bool value = false;
                for (int cq = 0; cq < cQueue.Length; cq++)
                {
                    MCostQueue queue = cQueue[cq];
                    if (queue.GetCurrentQty() < 0) continue;
                    if (queue.GetCurrentQty() > qty)
                    {
                        value = true;
                    }
                    else
                    {
                        value = false;
                    }
                    qty = MCostQueue.Quantity(queue.GetCurrentQty(), qty);
                    //if (cq == cQueue.Length - 1 && qty < 0) // last record
                    //{
                    //    queue.SetCurrentQty(qty);
                    //    if (!queue.Save())
                    //    {
                    //        ValueNamePair pp = VLogger.RetrieveError();
                    //        log.Info("Cost Queue not updated for  <===> " + product.GetM_Product_ID() + " Error Type is : " + pp.GetName());
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
                            log.Info("Cost Queue not updated for  <===> " + product.GetM_Product_ID() + " Error Type is : " + pp.GetName());
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
        private String UpdateTransactionContainer(MInOutLine sLine, MTransaction mtrx, decimal Qty)
        {
            string errorMessage = null;
            MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            Decimal containerCurrentQty = mtrx.GetContainerCurrentQty();
            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty , NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty  ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID()
                              + " ORDER BY movementdate ASC , m_transaction_id ASC, created ASC";
                }
                else
                {
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty, NVL(ContainerCurrentQty, 0) AS ContainerCurrentQty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 "
                              + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
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
                                 Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), Get_TrxName());
                                if (!inventory.IsInternalUse())
                                {
                                    #region update Physical Inventory
                                    if (inventoryLine.GetM_ProductContainer_ID() == sLine.GetM_ProductContainer_ID())
                                    {
                                        inventoryLine.SetParent(inventory);
                                        inventoryLine.SetQtyBook(containerCurrentQty);
                                        inventoryLine.SetOpeningStock(containerCurrentQty);
                                        inventoryLine.SetDifferenceQty(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"])));
                                        if (!inventoryLine.Save())
                                        {
                                            log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
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
                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    if (trx.GetM_ProductContainer_ID() == sLine.GetM_ProductContainer_ID())
                                    {
                                        trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(containerCurrentQty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"]))));
                                    }
                                    else
                                    {
                                        trx.SetCurrentQty(Decimal.Add(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["movementqty"])));
                                    }
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
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
                                        if (sLine.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == sLine.GetM_ProductContainer_ID())
                                            containerCurrentQty = trx.GetContainerCurrentQty();
                                    }
                                    #endregion

                                    #region update Storage if last record of loop
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
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
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(Qty + trx.GetMovementQty());
                            if (trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == sLine.GetM_ProductContainer_ID())
                                trx.SetContainerCurrentQty(containerCurrentQty + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
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
                                if (trx.Get_ColumnIndex("M_ProductContainer_ID") >= 0 && trx.GetM_ProductContainer_ID() == sLine.GetM_ProductContainer_ID())
                                    containerCurrentQty = trx.GetContainerCurrentQty();
                            }
                            #endregion

                            #region update Storage if last record of loop
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
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
        private void UpdateTransaction(MInOutLine sLine, MTransaction mtrx, decimal Qty)
        {
            MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID();
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID()
                              + " ORDER BY movementdate ASC , m_transaction_id ASC, created ASC";
                }
                else
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ";
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 "
                              + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
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
                                 Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), Get_TrxName());
                                if (!inventory.IsInternalUse())
                                {
                                    inventoryLine.SetParent(inventory);
                                    inventoryLine.SetQtyBook(Qty);
                                    inventoryLine.SetOpeningStock(Qty);
                                    inventoryLine.SetDifferenceQty(Decimal.Subtract(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"])));
                                    if (!inventoryLine.Save())
                                    {
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                    }

                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"]))));
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                                    }
                                    else
                                    {
                                        Qty = trx.GetCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
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
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(Qty + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                            }
                            else
                            {
                                Qty = trx.GetCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
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

        private void UpdateCurrentRecord(MInOutLine line, MTransaction trxM, decimal qtyDiffer)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";

            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

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

                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //     + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                }
                else
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

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
                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
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
        private decimal? GetProductQtyFromStorage(MInOutLine sLine)
        {
            return 0;
            //    MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
            //    int attribSet_ID = pro.GetM_AttributeSet_ID();
            //    string sql = "";

            //    if (attribSet_ID > 0)
            //    {
            //        sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID()
            //             + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID();
            //    }
            //    else
            //    {
            //        sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID();
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
        private decimal? GetProductQtyFromTransaction(MInOutLine line, DateTime? movementDate, bool isAttribute)
        {
            decimal result = 0;
            string sql = "";

            if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID(), null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                   AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id )   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                      AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ", null, Get_Trx())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
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
        private Decimal? GetContainerQtyFromTransaction(MInOutLine line, DateTime? movementDate)
        {
            Decimal result = 0;
            string sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS ContainerCurrentQty                           FROM M_Transaction t
                           WHERE t.MovementDate <=" + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND t.AD_Client_ID                       = " + line.GetAD_Client_ID() + @"
                           AND t.M_Locator_ID                       = " + line.GetM_Locator_ID() + @"
                           AND t.M_Product_ID                       = " + line.GetM_Product_ID() + @"
                           AND NVL(t.M_AttributeSetInstance_ID , 0) = COALESCE(" + line.GetM_AttributeSetInstance_ID() + @",0)
                           AND NVL(t.M_ProductContainer_ID, 0)              = " + line.GetM_ProductContainer_ID();
            result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
            return result;
        }

        /// <summary>
        /// Check Material Policy
        /// </summary>
        /// <param name="line">Sets line ASI</param>
        private void CheckMaterialPolicy(MInOutLine line)
        {
            int no = MInOutLineMA.DeleteInOutLineMA(line.GetM_InOutLine_ID(), Get_TrxName());
            if (no > 0)
            {
                log.Config("Delete old #" + no);
            }

            // check is any record available of physical Inventory after date Movement -
            // if available - then not to create Attribute Record - neither to take impacts on Container Storage
            if (isContainrApplicable && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(M_Product_ID) FROM M_ContainerStorage WHERE IsPhysicalInventory = 'Y'
                 AND MMPolicyDate > " + GlobalVariable.TO_DATE(GetMovementDate(), true) +
                      @" AND M_Product_ID = " + line.GetM_Product_ID() +
                      @" AND NVL(M_AttributeSetInstance_ID , 0) = " + line.GetM_AttributeSetInstance_ID() +
                      @" AND M_Locator_ID = " + line.GetM_Locator_ID() +
                      @" AND NVL(M_ProductContainer_ID , 0) = " + line.GetM_ProductContainer_ID(), null, Get_Trx())) > 0)
            {
                return;
            }

            //	Incoming Trx
            String MovementType = GetMovementType();
            //bool inTrx = MovementType.charAt(1) == '+';	//	V+ Vendor Receipt, C+ Customer Return
            bool inTrx = MovementType.IndexOf('+') == 1;	//	V+ Vendor Receipt, C+ Customer Return
            MClient client = MClient.Get(GetCtx());

            bool needSave = false;
            MProduct product = line.GetProduct();

            //	Need to have Location
            if (product != null
                && line.GetM_Locator_ID() == 0)
            {
                line.SetM_Warehouse_ID(GetM_Warehouse_ID());
                line.SetM_Locator_ID(inTrx ? Env.ZERO : line.GetMovementQty());	//	default Locator
                needSave = true;
            }

            if (product != null)
            {
                if (GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReceipts) == 0 && line.GetMovementQty() > 0)
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
                        MInOutLineMA ma = MInOutLineMA.GetOrCreate(line, line.GetM_AttributeSetInstance_ID(), qtyToReceive, GetMovementDate());
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
                else if (GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_CustomerReturns) == 0)
                {
                    #region CustomerReturns
                    Decimal qtyToReturn = line.GetMovementQty();
                    if (isContainrApplicable)
                    {
                        qtyToReturn = autoBalanceNegative(line, product, line.GetMovementQty());
                    }

                    if (line.GetC_OrderLine_ID() != 0 && qtyToReturn.CompareTo(Env.ZERO) > 0)
                    {
                        {
                            //Linking to shipment line
                            MOrderLine rmaLine = new MOrderLine(GetCtx(), line.GetC_OrderLine_ID(), Get_Trx());
                            if (rmaLine.GetOrig_InOutLine_ID() > 0)
                            {
                                //retrieving ASI which is not already returned
                                MInOutLineMA[] shipmentMAS = MInOutLineMA.getNonReturned(GetCtx(), rmaLine.GetOrig_InOutLine_ID(), Get_Trx());

                                for (int ii = 0; ii < shipmentMAS.Length; ii++)
                                {
                                    MInOutLineMA sMA = shipmentMAS[ii];
                                    Decimal lineMAQty = sMA.GetMovementQty();
                                    if (lineMAQty.CompareTo(qtyToReturn) > 0)
                                    {
                                        lineMAQty = qtyToReturn;
                                    }

                                    MInOutLineMA ma = MInOutLineMA.GetOrCreate(line, sMA.GetM_AttributeSetInstance_ID(), lineMAQty, sMA.GetMMPolicyDate());
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
                            MInOutLineMA ma = MInOutLineMA.GetOrCreate(line, line.GetM_AttributeSetInstance_ID(), qtyToReturn, GetMovementDate());
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
                else if (GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReturns) == 0 && line.GetC_OrderLine_ID() != 0)
                {
                    #region Vendor Return with respect to PO
                    Decimal qtyToReturn = line.GetMovementQty();
                    if (qtyToReturn.CompareTo(Env.ZERO) > 0)
                    {
                        {
                            //Linking to MR line
                            MOrderLine rmaLine = new MOrderLine(GetCtx(), line.GetC_OrderLine_ID(), Get_Trx());
                            if (rmaLine.GetOrig_InOutLine_ID() > 0)
                            {
                                //retrieving ASI which is not already returned
                                MInOutLineMA[] shipmentMAS = MInOutLineMA.getNonReturned(GetCtx(), rmaLine.GetOrig_InOutLine_ID(), Get_Trx());

                                for (int ii = 0; ii < shipmentMAS.Length; ii++)
                                {
                                    MInOutLineMA sMA = shipmentMAS[ii];
                                    Decimal lineMAQty = sMA.GetMovementQty();
                                    if (lineMAQty.CompareTo(qtyToReturn) > 0)
                                    {
                                        lineMAQty = qtyToReturn;
                                    }

                                    MInOutLineMA ma = MInOutLineMA.GetOrCreate(line, sMA.GetM_AttributeSetInstance_ID(), lineMAQty, sMA.GetMMPolicyDate());
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
                            MInOutLineMA ma = MInOutLineMA.GetOrCreate(line, line.GetM_AttributeSetInstance_ID(), qtyToReturn, GetMovementDate());
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
                else if (GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReturns) == 0 ||
                         GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_CustomerShipment) == 0 ||
                    GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReceipts) == 0 && line.GetMovementQty() < 0)
                {
                    #region VendorReturns / CustomerShipment
                    //bool isLifoChecked = false;
                    dynamic[] storages = null;

                    // Is Used to get Material Policy
                    MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
                    String MMPolicy = pc.GetMMPolicy();

                    // Is used for handling Gurantee Date - In Case of ASI
                    //DateTime? minGuaranteeDate = GetMovementDate();

                    // Get Data from Container Storage based on Policy
                    if (isContainrApplicable)
                    {
                        storages = MProductContainer.GetContainerStorage(GetCtx(), 0, line.GetM_Locator_ID(), line.GetM_ProductContainer_ID(),
                      line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                      line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                      MClient.MMPOLICY_FiFo.Equals(MMPolicy), false, Get_TrxName());
                    }
                    else
                    {
                        storages = MStorage.GetWarehouse(GetCtx(), GetM_Warehouse_ID(), line.GetM_Product_ID(),
                            line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                               line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                               MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                    }

                    // qty which is need to handle
                    // In Case of Vendor Receipt - need to negate movement qty
                    Decimal qtyToDeliver = GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReceipts) == 0 ? Decimal.Negate(line.GetMovementQty()) : line.GetMovementQty();

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
                            MInOutLineMA ma = MInOutLineMA.GetOrCreate(line,
                                    storage.GetM_AttributeSetInstance_ID(),
                                    GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReceipts) == 0 ? Decimal.Negate(qtyToDeliver) : qtyToDeliver,
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
                            MInOutLineMA ma = MInOutLineMA.GetOrCreate(line,
                                    storage.GetM_AttributeSetInstance_ID(),
                                    GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReceipts) == 0 ?
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

                        if (isContainrApplicable && ii == storages.Length - 1 && !MClient.MMPOLICY_FiFo.Equals(MMPolicy))
                        {
                            storages = MProductContainer.GetContainerStorage(GetCtx(), 0, line.GetM_Locator_ID(), line.GetM_ProductContainer_ID(),
                                         line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                                         line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                                         MClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
                            ii = -1;
                        }
                    }

                    //if (isContainrApplicable && !MClient.MMPOLICY_FiFo.Equals(MMPolicy) && !isLifoChecked && qtyToDeliver != 0)
                    //{
                    //    isLifoChecked = true;
                    //    // Get Data from Container Storage based on Policy
                    //    storages = MProductContainer.GetContainerStorage(GetCtx(), 0, line.GetM_Locator_ID(), line.GetM_ProductContainer_ID(),
                    //  line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), product.GetM_AttributeSet_ID(),
                    //  line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)GetMovementDate(),
                    //  MClient.MMPOLICY_FiFo.Equals(MMPolicy), true, Get_TrxName());
                    //    goto LIFOManage;
                    //}

                    // In Case of Over Delivery, then did entry on Attribute Tab with remaning qty
                    if (qtyToDeliver != 0)
                    {
                        //Over Delivery
                        MInOutLineMA ma = MInOutLineMA.GetOrCreate(line, line.GetM_AttributeSetInstance_ID(),
                            GetMovementType().CompareTo(MInOut.MOVEMENTTYPE_VendorReceipts) == 0 ? Decimal.Negate(qtyToDeliver) : qtyToDeliver, GetMovementDate());
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
        protected Decimal autoBalanceNegative(MInOutLine line, MProduct product, Decimal qtyToReceive)
        {
            // Is Used to get Material Policy
            MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
            string MMPolicy = pc.GetMMPolicy();

            // Is Used to get all Negative records from Contaner Storage
            X_M_ContainerStorage[] storages = MProductContainer.GetContainerStorageNegative(GetCtx(), GetM_Warehouse_ID(), line.GetM_Locator_ID(),
                                              line.GetM_ProductContainer_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                    null, MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_Trx());

            DateTime? dateMPolicy = null;

            for (int ii = 0; ii < storages.Length; ii++)
            {
                X_M_ContainerStorage storage = storages[ii];
                if (storage.GetQty() < 0 && qtyToReceive.CompareTo(Env.ZERO) > 0)
                {
                    dateMPolicy = storage.GetMMPolicyDate();
                    Decimal lineMAQty = qtyToReceive;
                    if (lineMAQty.CompareTo(Decimal.Negate(storage.GetQty())) > 0)
                        lineMAQty = Decimal.Negate(storage.GetQty());

                    //Using ASI from storage record
                    MInOutLineMA ma = MInOutLineMA.GetOrCreate(line, storage.GetM_AttributeSetInstance_ID(), lineMAQty, dateMPolicy);
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
        private MInOut CreateCounterDoc()
        {
            //	Is this a counter doc ?
            if (GetRef_InOut_ID() != 0)
            {
                return null;
            }

            //	Document Type
            //check weather Counter Document created & Acitve or not 
            int C_DocTypeTarget_ID = 0;
            bool isReturnTrx = false;
            MDocTypeCounter counterDT = MDocTypeCounter.GetCounterDocType(GetCtx(), GetC_DocType_ID());
            if (counterDT != null)
            {
                log.Fine(counterDT.ToString());
                //if Created Inter Company Document is not Valid the does not allow user to complete the record
                //it should return message to user the Counter Document is not Valid
                if (!counterDT.IsCreateCounter() || !counterDT.IsValid())
                {
                    //erro save into the log
                    log.Info("Counter Document Type is not Valid one!");
                    return null;
                }
                C_DocTypeTarget_ID = counterDT.GetCounter_C_DocType_ID();
                isReturnTrx = counterDT.GetCounterDocType().IsReturnTrx();
                //if Counter document type not found then return message to the user
                if (C_DocTypeTarget_ID <= 0)
                {
                    //Info save into the log
                    log.Info("Counter Document Type not found on Inter Company Document window.");
                    return null;
                }
            }
            else
            {
                return null;
            }


            //	Org Must be linked to BPartner
            MOrg org = MOrg.Get(GetCtx(), GetAD_Org_ID());
            //jz int counterC_BPartner_ID = org.getLinkedC_BPartner_ID(get_TrxName()); 
            int counterC_BPartner_ID = org.GetLinkedC_BPartner_ID(Get_TrxName());
            //if counter BP not found the return the message Counter Bp not found for Linked Org
            if (counterC_BPartner_ID == 0)
            {
                //Info save into the log
                log.Info("Business Partner is not found on Customer/Vendor master window to create the Counter Document.");
                return null;
            }
            //	Business Partner needs to be linked to Org
            //jz MBPartner bp = new MBPartner (getCtx(), getC_BPartner_ID(), null);
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            int counterAD_Org_ID = bp.GetAD_OrgBP_ID_Int();
            //if Org is not link the for BP then save info into the log, not found Link Org with the BP
            if (counterAD_Org_ID == 0)
            {
                //if Link Org not found then save the info into the log
                log.Info("Linked Organization is not found on Customer/Vendor master window to create the Counter Document.");
                return null;
            }

            //System should not allow to create counter document with same BP and organization.
            if (counterAD_Org_ID == GetAD_Org_ID() || counterC_BPartner_ID == GetC_BPartner_ID())
            {
                log.Info("On Counter Document Organization or Business Partner should not allow the same with the Document.");
                return null;
            }

            //jz MBPartner counterBP = new MBPartner (getCtx(), counterC_BPartner_ID, null);
            MBPartner counterBP = new MBPartner(GetCtx(), counterC_BPartner_ID, Get_TrxName());
            MOrgInfo counterOrgInfo = MOrgInfo.Get(GetCtx(), counterAD_Org_ID, null);
            log.Info("Counter BP=" + counterBP.GetName());

            // ReversalDoc_ID --> contain reference of Orignal Document which is to be reversed
            // Ref_InOut_ID --> contain reference of counter document which is to be created against this document
            // when we reverse document, and if counter document is created agaisnt its orignal document then need to reverse that document also
            if (Get_ColumnIndex("ReversalDoc_ID") > 0 && GetReversalDoc_ID() > 0)
            {
                int counterOrderId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Ref_InOut_ID FROM M_InOut WHERE M_InOut_ID = " + GetReversalDoc_ID(), null, Get_Trx()));
                MInOut counterReversed = new MInOut(GetCtx(), counterOrderId, Get_Trx());
                if (counterReversed != null && counterReversed.GetM_InOut_ID() > 0)
                {
                    counterReversed.SetDocAction(DOCACTION_Void);
                    counterReversed.ProcessIt(DOCACTION_Void);
                    counterReversed.Save(Get_Trx());
                    return counterReversed;
                }
                return null;
            }

            //	Deep Copy
            MInOut counter = CopyFrom(this, GetMovementDate(),
                C_DocTypeTarget_ID, !IsSOTrx(), isReturnTrx, true, Get_TrxName(), true);

            //
            counter.SetAD_Org_ID(counterAD_Org_ID);
            counter.SetM_Warehouse_ID(counterOrgInfo.GetM_Warehouse_ID());
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
            MInOutLine[] counterLines = counter.GetLines(true);
            for (int i = 0; i < counterLines.Length; i++)
            {
                MInOutLine counterLine = counterLines[i];
                counterLine.SetClientOrg(counter);
                counterLine.SetM_Warehouse_ID(counter.GetM_Warehouse_ID());
                counterLine.SetM_Locator_ID(0);
                int locatorId = 0;
                if (!inTrx)
                {
                    locatorId = MStorage.GetM_Locator_ID(counter.GetM_Warehouse_ID(), counterLine.GetM_Product_ID(), counterLine.GetM_AttributeSetInstance_ID(),
                                                            counterLine.GetMovementQty(), Get_TrxName());
                }
                else
                {
                    locatorId = MWarehouse.Get(GetCtx(), counter.GetM_Warehouse_ID()).GetDefaultM_Locator_ID();
                }
                //counterLine.SetM_Locator_ID(Convert.ToInt32(inTrx ? Env.ZERO : counterLine.GetMovementQty()));
                counterLine.SetM_Locator_ID(locatorId);
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
            // int countVA038 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA038_' "));
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
                MInOutLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    Asset_ID = 0;
                    MInOutLine line = lines[i];
                    Decimal old = line.GetMovementQty();
                    if (old != 0)
                    {
                        line.SetQty(Env.ZERO);
                        line.AddDescription("Void (" + old + ")");
                        if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
                        {
                            line.Set_Value("VA077_MarginAmt", Env.ZERO);
                            line.Set_Value("VA077_MarginPercent", Env.ZERO);
                        }

                        line.Save(Get_TrxName());
                        //if (countVA038 > 0)
                        //{
                        //    Asset_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT A_Asset_ID FROM A_Asset WHERE M_INOUTLINE_ID=" + line.GetM_InOutLine_ID()));
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
                MInOutConfirm[] confirmations = GetConfirmations(false);
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
        public bool IsOnlyForOrder(MOrder order)
        {
            //	TODO Compare Lines
            return GetC_Order_ID() == order.GetC_Order_ID();
        }

        /// <summary>
        /// Reverse Correction - same date
        /// </summary>
        /// <param name="order">if not null only for this order</param>
        /// <returns>true if success </returns>
        public bool ReverseCorrectIt(MOrder order)
        {
            log.Info(ToString());
            string reversedDocno = null;
            string ss = ToString();
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return false;
            }

            MOrder ord = new MOrder(GetCtx(), GetC_Order_ID(), Get_Trx());
            MDocType dtOrder = MDocType.Get(GetCtx(), ord.GetC_DocType_ID());
            String DocSubTypeSO = dtOrder.GetDocSubTypeSO();

            // if any linked record exist on invoice for PO/SO cycle then not able to reverse this record
            if (MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)	//	(W)illCall(I)nvoice
                    || MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                    || MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))			//	(W)alkIn(R)eceipt
            {
                // when we void SO then system void all transaction which is linked with that inout
            }
            else
            {
                if (!linkedDocumentAgainstInOut(GetM_InOut_ID()))
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "LinkedDocStatus");
                    return false;
                }


                // Added by Vivek on 08/11/2017 assigned by Mukesh sir
                // return false if linked document is in completed or closed stage
                // stuck case for this -- Order1 --> M1 & MR2  --> create invoice against M1 and try to reverse MR2, system not allowing by this check
                // linked doc check above, so not used this function
                //if (!linkedDocument(GetC_Order_ID()))
                //{
                //    _processMsg = Msg.GetMsg(GetCtx(), "LinkedDocStatus");
                //    return false;
                //}
            }
            //Added by Vivek assigned by Pradeep on 27/09/2017
            // Stopped voiding manually for drop ship type of shipment
            if (IsSOTrx() && !IsReturnTrx() && IsDropShip())
            {
                MInOut _inout = new MInOut(GetCtx(), GetRef_ShipMR_ID(), Get_TrxName());
                if (_inout.GetDocStatus() != "VO" && _inout.GetDocStatus() != "RE")
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "VIS_ShipmentRevStop");
                    return false;
                }
            }

            //	Reverse/Delete Matching
            if (!IsSOTrx())
            {
                MMatchInv[] mInv = MMatchInv.GetInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
                for (int i = 0; i < mInv.Length; i++)
                    mInv[i].Delete(true);
                MMatchPO[] mPO = MMatchPO.GetInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
                for (int i = 0; i < mPO.Length; i++)
                {
                    if (mPO[i].GetC_InvoiceLine_ID() == 0)
                        mPO[i].Delete(true);
                    else
                    {
                        mPO[i].SetM_InOutLine_ID(0);
                        mPO[i].Save();
                    }
                }
            }

            //	Deep Copy
            MInOut reversal = CopyFrom(this, GetMovementDate(),
                GetC_DocType_ID(), IsSOTrx(), dt.IsReturnTrx(), false, Get_TrxName(), true);
            if (reversal.Get_ColumnIndex("IsFutureCostCalculated") > 0)
            {
                reversal.SetIsFutureCostCalculated(false);
            }
            if (reversal.Get_ColumnIndex("ReversalDoc_ID") > 0)
            {
                reversal.SetReversalDoc_ID(GetM_InOut_ID());
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
            MInOutLine[] sLines = GetLines(false);
            MInOutLine[] rLines = reversal.GetLines(false);
            for (int i = 0; i < rLines.Length; i++)
            {
                MInOutLine rLine = rLines[i];
                //rLine.SetQtyEntered(Decimal.Negate(rLine.GetQtyEntered()));
                //rLine.SetMovementQty(Decimal.Negate(rLine.GetMovementQty()));
                rLine.SetM_AttributeSetInstance_ID(sLines[i].GetM_AttributeSetInstance_ID());
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
                MInOutLineMA[] mas = MInOutLineMA.Get(GetCtx(), rLine.GetReversalDoc_ID(), Get_TrxName());
                for (int j = 0; j < mas.Length; j++)
                {
                    MInOutLineMA ma = new MInOutLineMA(rLine, mas[j].GetM_AttributeSetInstance_ID(), Decimal.Negate(mas[j].GetMovementQty()), mas[j].GetMMPolicyDate());
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

                List<MAsset> asset = MAsset.GetFromShipment(GetCtx(), sLines[i].GetM_InOutLine_ID(), Get_TrxName());
                foreach (MAsset ass in asset)
                {
                    ass.SetIsActive(false);
                    ass.AddDescription("(" + reversal.GetDocumentNo() + " #" + rLine.GetLine() + "<-)");
                    ass.Save();
                }
            }
            reversal.SetC_Order_ID(GetC_Order_ID());
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
                MInOutConfirm[] confirmations = GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MInOutConfirm confirm = confirmations[i];
                    if (confirm.GetDocStatus() == DOCSTATUS_Completed)
                    {
                        if (confirm.GetM_Inventory_ID() > 0)
                        {   //For inventory reversal 
                            MInventory _Inventory = new MInventory(GetCtx(), confirm.GetM_Inventory_ID(), Get_TrxName());

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
                MInOut ino = new MInOut(GetCtx(), GetRef_ShipMR_ID(), Get_Trx());
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
        /// <returns>AD_User_ID</returns>
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
        /// Get C_Currency_ID
        /// </summary>
        /// <returns>Accounting Currency</returns>
        public int GetC_Currency_ID()
        {
            return GetCtx().GetContextAsInt("$C_Currency_ID ");
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
        /// <param name="C_Order_ID"></param>
        /// <returns>True/False</returns>
        private bool linkedDocument(int C_Order_ID)
        {
            string sql = "SELECT COUNT(C_Order_ID) FROM C_Invoice WHERE C_Order_ID = " + C_Order_ID + "  AND DocStatus NOT IN ('RE','VO')";
            int _countOrder = Util.GetValueOfInt(DB.ExecuteScalar(sql));
            if (_countOrder > 0)
            {
                return false;
            }
            return true;
        }

        // checked M_Inout reference is available on C_invoice then not to reverse this M_Inout
        private bool linkedDocumentAgainstInOut(int M_Inout_ID)
        {
            // JID_1310: At the time of void MR need to check if any RMA and Custmer/Vendor RMA has MR refrence. If found need to give message
            string sql = @"SELECT SUM(Result) From (
                          SELECT COUNT(il.M_InOutLine_ID) AS Result FROM C_Invoice i INNER JOIN C_InvoiceLine il ON i.C_Invoice_ID = il.C_Invoice_ID
                          WHERE il.M_InOutLine_ID = (SELECT M_InOutLine_ID FROM M_InOutLine mil WHERE mil.M_InOut_ID = " + M_Inout_ID + @"
                          AND mil.M_InOutLine_ID = il.M_InOutLine_ID) AND il.IsActive = 'Y' AND i.DocStatus NOT IN ('RE' , 'VO')
                          UNION ALL
                          SELECT COUNT(o.C_Order_ID) AS Result FROM C_Order o WHERE o.Orig_InOut_ID = " + M_Inout_ID + @" AND o.DocStatus NOT IN ('RE' , 'VO')) t";
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
                    MInOutLine[] lines = GetLines();
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
