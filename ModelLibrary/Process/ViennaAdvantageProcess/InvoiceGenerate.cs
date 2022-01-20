/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InvoiceGenerate  
 * Purpose        : Generate Invoices
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     07-Sep-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace ViennaAdvantage.Process
{
    public class InvoiceGenerate : VAdvantage.ProcessEngine.SvrProcess
    {
        #region Private Variable
        /**	Manual Selection		*/
        private bool _Selection = false;
        /**	Date Invoiced			*/
        private DateTime? _DateInvoiced = null;
        /**	Org						*/
        private int _AD_Org_ID = 0;
        /** BPartner				*/
        private int _C_BPartner_ID = 0;
        /** Order					*/
        //private int _C_Order_ID = 0;
        private string _C_Order_ID = "";
        /** Consolidate				*/
        private bool _ConsolidateDocument = true;
        /** Invoice Document Action	*/
        private String _docAction = DocActionVariables.ACTION_COMPLETE;

        /**	The current Invoice	*/
        private MInvoice _invoice = null;
        /**	The current Shipment	*/
        private MInOut _ship = null;
        /** Numner of Invoices		*/
        private int _created = 0;
        /**	Line Number				*/
        private int _line = 0;
        /**	Business Partner		*/
        private MBPartner _bp = null;
        //StringBuilder
        private StringBuilder _msg = new StringBuilder();
        // default conversion type
        private int defaultConversionType = 0;
        #endregion

        /**
	 *  Prepare - e.g., get Parameters.
	 */
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("Selection"))
                {
                    _Selection = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("DateInvoiced"))
                {
                    _DateInvoiced = (DateTime?)para[i].GetParameter();
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    _AD_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID = para[i].GetParameter().ToString();
                }
                else if (name.Equals("ConsolidateDocument"))
                {
                    _ConsolidateDocument = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("DocAction"))
                {
                    _docAction = (String)para[i].GetParameter();
                }
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }

            //	Login Date
            if (_DateInvoiced == null)
            {
                //_DateInvoiced = new DateTime(GetCtx().GetContextAsTime("#Date"));
                _DateInvoiced = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime("#Date"));//change at 22-Sep-2009
            }

            //	DocAction check
            if (!DocActionVariables.ACTION_COMPLETE.Equals(_docAction))
            {
                _docAction = DocActionVariables.ACTION_PREPARE;
            }
        }

        /**
         * 	Generate Invoices
         *	@return info
         *	@throws Exception
         */
        protected override String DoIt()
        {
            defaultConversionType = MConversionType.GetDefault(GetAD_Client_ID());
            log.Info("Selection=" + _Selection + ", DateInvoiced=" + _DateInvoiced
                + ", AD_Org_ID=" + _AD_Org_ID + ", C_BPartner_ID=" + _C_BPartner_ID
                + ", C_Order_ID=" + _C_Order_ID + ",  VAdvantage.Process.DocAction=" + _docAction
                + ", Consolidate=" + _ConsolidateDocument + ", Default Conversion Type = " + defaultConversionType);
            //
            String sql = null;

            //Changes done for invoice generation when multiple users generate the invoices at the same time. 02-May-18

            //if (_Selection)	//	VInvoiceGen
            //{
            //    sql = "SELECT * FROM C_Order "
            //        + "WHERE IsSelected='Y' AND DocStatus='CO' AND IsSOTrx='Y' AND AD_Client_ID = " + GetAD_Client_ID() + " AND AD_Org_ID = " + GetAD_Org_ID()
            //        + "ORDER BY M_Warehouse_ID, PriorityRule, C_BPartner_ID, C_PaymentTerm_ID, C_Order_ID";
            //}
            //else
            //{
            // not pick record of "Sales Quotation" And "Blanket Order"
            sql = "SELECT * FROM C_Order o "
                + "WHERE o.DocStatus IN('CO','CL') AND o.IsSOTrx='Y' AND o.IsSalesQuotation = 'N' AND o.isblankettrx = 'N' AND o.AD_Client_ID = " + GetAD_Client_ID();
            if (_AD_Org_ID != 0)
            {
                sql += " AND AD_Org_ID=" + _AD_Org_ID;
            }
            if (_C_BPartner_ID != 0)
            {
                sql += " AND C_BPartner_ID=" + _C_BPartner_ID;
            }
            if (_C_Order_ID != null && _C_Order_ID != string.Empty)
            {
                sql += " AND C_Order_ID IN (" + _C_Order_ID + ")";
            }
            // JID_1237 : While creating invoice need to consolidate order on the basis of Org, Payment Term, BP Location (Bill to Location) and Pricelist.
            sql += " AND EXISTS (SELECT * FROM C_OrderLine ol "
                    + "WHERE o.C_Order_ID=ol.C_Order_ID AND ol.QtyOrdered<>ol.QtyInvoiced AND ol.IsContract ='N') "
                + "ORDER BY AD_Org_ID, C_BPartner_ID, C_PaymentTerm_ID, M_PriceList_ID, C_ConversionType_ID, C_Order_ID, M_Warehouse_ID, PriorityRule";

            //sql += " AND EXISTS (SELECT * FROM C_OrderLine ol INNER JOIN c_order ord "
            //      + "  ON (ord.c_order_id = ol.c_order_id) WHERE ord.C_Order_ID  =ol.C_Order_ID "
            //     + "  AND ol.QtyOrdered <> ol.QtyInvoiced AND ol.Iscontract ='N') "
            //  + "ORDER BY M_Warehouse_ID, PriorityRule, C_BPartner_ID, C_PaymentTerm_ID, C_Order_ID";
            //}
            //	sql += " FOR UPDATE";

            //End of Changes done for invoice generation when multiple users generate the invoices at the same time. 02-May-18

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                return Generate(dt);
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Generate Invoices
        /// </summary>
        /// <param name="idr">pstmt order query</param>
        /// <returns>info</returns>
        //private String Generate(IDataReader idr)
        private String Generate(DataTable dt)
        {
            //JID_1139 Avoided the duplicate charge line created Invoice(customer)
            bool isAllownonItem = Util.GetValueOfString(GetCtx().GetContext("$AllowNonItem")).Equals("Y");
            foreach (DataRow dr in dt.Rows)
            {
                MOrder order = new MOrder(GetCtx(), dr, Get_TrxName());

                // Credit Limit check 
                MBPartner bp = MBPartner.Get(GetCtx(), order.GetC_BPartner_ID());
                if (bp.GetCreditStatusSettingOn() == "CH")
                {
                    decimal creditLimit = bp.GetSO_CreditLimit();
                    string creditVal = bp.GetCreditValidation();
                    if (creditLimit != 0)
                    {
                        decimal creditAvlb = creditLimit - bp.GetSO_CreditUsed();
                        if (creditAvlb <= 0)
                        {
                            if (creditVal == "C" || creditVal == "D" || creditVal == "F")
                            {
                                AddLog(Msg.GetMsg(GetCtx(), "StopInvoice") + bp.GetName());
                                continue;
                            }
                            else if (creditVal == "I" || creditVal == "J" || creditVal == "L")
                            {
                                if (_msg != null)
                                {
                                    _msg.Clear();
                                }
                                _msg.Append(Msg.GetMsg(GetCtx(), "WarningInvoice") + bp.GetName());
                                //AddLog(Msg.GetMsg(GetCtx(), "WarningInvoice") + bp.GetName());
                            }
                        }
                    }
                }
                // JID_0161 // change here now will check credit settings on field only on Business Partner Header // Lokesh Chauhan 15 July 2019
                else if (bp.GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerLocation)
                {
                    MBPartnerLocation bpl = new MBPartnerLocation(GetCtx(), order.GetC_BPartner_Location_ID(), null);
                    //MBPartner bpartner = MBPartner.Get(GetCtx(), order.GetC_BPartner_ID());
                    //if (bpl.GetCreditStatusSettingOn() == "CL")
                    //{
                    decimal creditLimit = bpl.GetSO_CreditLimit();
                    string creditVal = bpl.GetCreditValidation();
                    if (creditLimit != 0)
                    {
                        decimal creditAvlb = creditLimit - bpl.GetSO_CreditUsed();
                        if (creditAvlb <= 0)
                        {
                            if (creditVal == "C" || creditVal == "D" || creditVal == "F")
                            {
                                AddLog(Msg.GetMsg(GetCtx(), "StopInvoice") + bp.GetName() + " " + bpl.GetName());
                                continue;
                            }
                            else if (creditVal == "I" || creditVal == "J" || creditVal == "L")
                            {
                                if (_msg != null)
                                {
                                    _msg.Clear();
                                }
                                _msg.Append(Msg.GetMsg(GetCtx(), "WarningInvoice") + bp.GetName() + " " + bpl.GetName());
                                //AddLog(Msg.GetMsg(GetCtx(), "WarningInvoice") + bp.GetName() + " " + bpl.GetName());
                            }
                        }
                    }
                    //}
                }
                // Credit Limit End

                //	New Invoice Location
                // JID_1237 : While creating invoice need to consolidate order on the basis of Org, Payment Term, BP Location (Bill to Location) and Pricelist.
                if (!_ConsolidateDocument
                    || (_invoice != null
                    && (_invoice.GetC_BPartner_Location_ID() != order.GetBill_Location_ID()
                        || _invoice.GetC_PaymentTerm_ID() != order.GetC_PaymentTerm_ID()
                        || _invoice.GetM_PriceList_ID() != order.GetM_PriceList_ID()
                        || _invoice.GetAD_Org_ID() != order.GetAD_Org_ID()
                        || ((_invoice.GetC_ConversionType_ID() != 0 ? _invoice.GetC_ConversionType_ID() : defaultConversionType)
                             != (order.GetC_ConversionType_ID() != 0 ? order.GetC_ConversionType_ID() : defaultConversionType))
                       )))
                {
                    CompleteInvoice();
                }
                bool completeOrder = MOrder.INVOICERULE_AfterOrderDelivered.Equals(order.GetInvoiceRule());

                //	Schedule After Delivery
                bool doInvoice = false;
                if (MOrder.INVOICERULE_CustomerScheduleAfterDelivery.Equals(order.GetInvoiceRule()))
                {
                    _bp = new MBPartner(GetCtx(), order.GetBill_BPartner_ID(), null);
                    if (_bp.GetC_InvoiceSchedule_ID() == 0)
                    {
                        log.Warning("BPartner has no Schedule - set to After Delivery");
                        order.SetInvoiceRule(MOrder.INVOICERULE_AfterDelivery);
                        order.Save();
                    }
                    else
                    {
                        MInvoiceSchedule ins = MInvoiceSchedule.Get(GetCtx(), _bp.GetC_InvoiceSchedule_ID(), Get_TrxName());
                        if (ins.CanInvoice(order.GetDateOrdered(), order.GetGrandTotal()))
                        {
                            doInvoice = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }	//	Schedule

                //	After Delivery
                if (doInvoice || MOrder.INVOICERULE_AfterDelivery.Equals(order.GetInvoiceRule()))
                {
                    MInOut shipment = null;
                    MInOutLine[] shipmentLines = order.GetShipmentLines();
                    MOrderLine[] oLines = order.GetLines(true, null);
                    for (int i = 0; i < shipmentLines.Length; i++)
                    {
                        MInOutLine shipLine = shipmentLines[i];
                        if (shipLine.IsInvoiced())
                        {
                            continue;
                        }
                        if (shipment == null
                            || shipment.GetM_InOut_ID() != shipLine.GetM_InOut_ID())
                        {
                            shipment = new MInOut(GetCtx(), shipLine.GetM_InOut_ID(), Get_TrxName());
                        }
                        if (!shipment.IsComplete()		//	ignore incomplete or reversals 
                            || shipment.GetDocStatus().Equals(MInOut.DOCSTATUS_Reversed))
                        {
                            continue;
                        }
                        //JID_1139 Avoided the duplicate charge records
                        if (shipLine.GetM_Product_ID() > 0 || isAllownonItem)
                        {
                            CreateLine(order, shipment, shipLine);
                        }
                    }//	shipment lines

                    //JID_1139 Avoided the duplicate charge records
                    if (!isAllownonItem)
                    {
                        for (int i = 0; i < oLines.Length; i++)
                        {
                            MOrderLine oLine = oLines[i];
                            if (oLine.GetC_Charge_ID() > 0)
                            {
                                Decimal toInvoice = Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyInvoiced());
                                log.Fine("Immediate - ToInvoice=" + toInvoice + " - " + oLine);
                                Decimal qtyEntered = toInvoice;
                                //	Correct UOM for QtyEntered
                                if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                                {
                                    qtyEntered = Decimal.Round(Decimal.Divide(Decimal.Multiply(
                                        toInvoice, oLine.GetQtyEntered()),
                                        oLine.GetQtyOrdered()), 12, MidpointRounding.AwayFromZero);
                                }
                                //JID_1139_1 avoided the charge line with 0 qty inserted
                                if (oLine.IsContract() == false && oLine.GetQtyOrdered() > oLine.GetQtyInvoiced())
                                {
                                    CreateLine(order, oLine, toInvoice, qtyEntered);
                                    log.Info("ID " + oLine.Get_ID() + "Qty Ordered " + oLine.GetQtyOrdered() + " Qty Invoiced " + oLine.GetQtyInvoiced());
                                }
                            }
                        }
                    }

                }
                //	After Order Delivered, Immediate
                else
                {
                    MOrderLine[] oLines = order.GetLines(true, null);
                    for (int i = 0; i < oLines.Length; i++)
                    {
                        MOrderLine oLine = oLines[i];
                        Decimal toInvoice = Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyInvoiced());
                        if (toInvoice.CompareTo(Env.ZERO) == 0 && oLine.GetM_Product_ID() != 0)
                        {
                            continue;
                        }
                        //                            
                        bool fullyDelivered = oLine.GetQtyOrdered().CompareTo(oLine.GetQtyDelivered()) == 0;
                        //JID_1136: While creating the Invoices against the charge system should not check the Ordered qty= Delivered qty. need to check this only in case of products
                        if (completeOrder && oLine.GetC_Charge_ID() > 0)
                        {
                            fullyDelivered = true;
                            if (oLine.GetC_Charge_ID() > 0)
                            {
                                log.Fine("After Order Delivery - ToInvoice=" + toInvoice + " - " + oLine);
                                Decimal qtyEntered = toInvoice;
                                //	Correct UOM for QtyEntered
                                if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                                {
                                    qtyEntered = Decimal.Round(Decimal.Divide(Decimal.Multiply(
                                        toInvoice, oLine.GetQtyEntered()),
                                        oLine.GetQtyOrdered()), 12, MidpointRounding.AwayFromZero);
                                }
                                //
                                if (oLine.IsContract() == false && !isAllownonItem)
                                {
                                    CreateLine(order, oLine, toInvoice, qtyEntered);
                                    log.Info("ID " + oLine.Get_ID() + "Qty Ordered " + oLine.GetQtyOrdered() + " Qty Invoiced " + oLine.GetQtyInvoiced());
                                }
                            }
                        }

                        //	Complete Order
                        if (completeOrder && !fullyDelivered)
                        {
                            log.Fine("Failed CompleteOrder - " + oLine);
                            completeOrder = false;
                            break;
                        }
                        //	Immediate
                        else if (MOrder.INVOICERULE_Immediate.Equals(order.GetInvoiceRule()))
                        {
                            log.Fine("Immediate - ToInvoice=" + toInvoice + " - " + oLine);
                            Decimal qtyEntered = toInvoice;
                            //	Correct UOM for QtyEntered
                            if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                            {
                                qtyEntered = Decimal.Round(Decimal.Divide(Decimal.Multiply(
                                    toInvoice, oLine.GetQtyEntered()),
                                    oLine.GetQtyOrdered()), 12, MidpointRounding.AwayFromZero);
                            }
                            //
                            if (oLine.IsContract() == false)
                            {
                                CreateLine(order, oLine, toInvoice, qtyEntered);
                                log.Info("ID " + oLine.Get_ID() + "Qty Ordered " + oLine.GetQtyOrdered() + " Qty Invoiced " + oLine.GetQtyInvoiced());
                            }

                        }
                        else
                        {
                            log.Fine("Failed: " + order.GetInvoiceRule()
                                + " - ToInvoice=" + toInvoice + " - " + oLine);
                        }
                    }	//	for all order lines
                    if (MOrder.INVOICERULE_Immediate.Equals(order.GetInvoiceRule()))
                        _line += 1000;
                }

                //	Complete Order successful
                if (completeOrder && MOrder.INVOICERULE_AfterOrderDelivered.Equals(order.GetInvoiceRule()))
                {
                    MInOut[] shipments = order.GetShipments(true);
                    for (int i = 0; i < shipments.Length; i++)
                    {
                        MInOut ship = shipments[i];
                        if (!ship.IsComplete()		//	ignore incomplete or reversals 
                            || ship.GetDocStatus().Equals(MInOut.DOCSTATUS_Reversed))
                        {
                            continue;
                        }
                        MInOutLine[] shipLines = ship.GetLines(false);
                        for (int j = 0; j < shipLines.Length; j++)
                        {
                            MInOutLine shipLine = shipLines[j];
                            if (!order.IsOrderLine(shipLine.GetC_OrderLine_ID()))
                            {
                                continue;
                            }
                            if (!shipLine.IsInvoiced())
                            {
                                CreateLine(order, ship, shipLine);
                            }
                        }	//	lines
                        _line += 1000;
                    }	//	all shipments
                }	//	complete Order

            }	//	for all orders

            CompleteInvoice();
            return "@Created@ = " + _created;
        }

        /// <summary>
        /// Create Invoice Line from Order Line
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="orderLine">line</param>
        /// <param name="qtyInvoiced">qty</param>
        /// <param name="qtyEntered">qty</param>
        private void CreateLine(MOrder order, MOrderLine orderLine,
            Decimal qtyInvoiced, Decimal qtyEntered)
        {
            if (_invoice == null)
            {
                _invoice = new MInvoice(order, 0, _DateInvoiced);
                int _CountVA009 = Env.IsModuleInstalled("VA009_") ? 1 : 0;
                if (_CountVA009 > 0)
                {
                    int _PaymentMethod_ID = order.GetVA009_PaymentMethod_ID();

                    // Get Payment method from Business partner
                    int bpPamentMethod_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT " + (order.IsSOTrx() ? " VA009_PaymentMethod_ID " : " VA009_PO_PaymentMethod_ID ") +
                        @" FROM C_BPartner WHERE C_BPartner_ID = " + order.GetC_BPartner_ID(), null, Get_Trx()));

                    // during consolidation, payment method need to set that is defined on selected business partner. 
                    // If not defined on BP then it will set from order
                    if (_ConsolidateDocument && bpPamentMethod_ID != 0)
                    {
                        _PaymentMethod_ID = bpPamentMethod_ID;
                    }
                    if (_PaymentMethod_ID > 0)
                    {
                        _invoice.SetVA009_PaymentMethod_ID(_PaymentMethod_ID);
                    }
                }

                int _CountVA026 = Env.IsModuleInstalled("VA026_") ? 1 : 0;
                if (_CountVA026 > 0)
                {
                    _invoice.SetVA026_LCDetail_ID(order.GetVA026_LCDetail_ID());
                }

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order
                if (_invoice.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    _invoice.SetC_IncoTerm_ID(order.GetC_IncoTerm_ID());
                }


                if (Env.IsModuleInstalled("VA077_"))
                {
                    //.setva077
                    _invoice.Set_Value("VA077_AdditionalAnnualCharge", order.Get_Value("VA077_AdditionalAnnualCharge"));
                    _invoice.Set_Value("VA077_ChangeStartDate", order.Get_Value("VA077_ChangeStartDate"));
                    _invoice.Set_Value("VA077_ContractCPEndDate", order.Get_Value("VA077_ContractCPEndDate"));
                    _invoice.Set_Value("VA077_ContractCPStartDate", order.Get_Value("VA077_ContractCPStartDate"));
                    _invoice.Set_Value("VA077_HistoricContractDate", order.Get_Value("VA077_HistoricContractDate"));
                    _invoice.Set_Value("VA077_IsLegalEntity", order.Get_Value("VA077_IsLegalEntity"));
                    _invoice.Set_Value("VA077_MarginPercent", order.Get_Value("VA077_MarginPercent"));
                    _invoice.Set_Value("VA077_NewAnnualContractTotal", order.Get_Value("VA077_NewAnnualContractTotal"));
                    _invoice.Set_Value("VA077_OldAnnualContractTotal", order.Get_Value("VA077_OldAnnualContractTotal"));
                    _invoice.Set_Value("VA077_PartialAmtCatchUp", order.Get_Value("VA077_PartialAmtCatchUp"));

                    _invoice.Set_Value("VA077_SalesCoWorker", order.Get_Value("VA077_SalesCoWorker"));
                    _invoice.Set_Value("VA077_SalesCoWorkerPer", order.Get_Value("VA077_SalesCoWorkerPer"));
                    _invoice.Set_Value("VA077_TotalMarginAmt", order.Get_Value("VA077_TotalMarginAmt"));
                    _invoice.Set_Value("VA077_TotalPurchaseAmt", order.Get_Value("VA077_TotalPurchaseAmt"));
                    _invoice.Set_Value("VA077_TotalSalesAmt", order.Get_Value("VA077_TotalSalesAmt"));
                }

                _invoice.SetConditionalFlag(MInvoice.CONDITIONALFLAG_PrepareIt);

                if (!_invoice.Save())
                {
                    ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                    if (pp != null)
                        throw new ArgumentException("Could not create Invoice (o). " + pp.GetName());
                    throw new Exception("Could not create Invoice (o)");
                }
            }
            //	
            MInvoiceLine line = new MInvoiceLine(_invoice);
            line.SetOrderLine(orderLine);
            line.SetQtyInvoiced(qtyInvoiced);
            // if drop ship line true 
            line.SetIsDropShip(orderLine.IsDropShip());

            log.Info("Qty Invoiced" + line.GetQtyInvoiced());
            line.SetQtyEntered(qtyEntered);
            line.SetLine(_line + orderLine.GetLine());

            //190 - Set Print Description
            if (line.Get_ColumnIndex("PrintDescription") >= 0)
                line.Set_Value("PrintDescription", orderLine.Get_Value("PrintDescription"));

            if (Env.IsModuleInstalled("VA077_"))
            {
                //invoice line
                line.Set_Value("VA077_CNAutodesk", orderLine.Get_Value("VA077_CNAutodesk"));
                line.Set_Value("VA077_Duration", orderLine.Get_Value("VA077_Duration"));
                line.Set_Value("VA077_MarginAmt", orderLine.Get_Value("VA077_MarginAmt"));
                line.Set_Value("VA077_MarginPercent", orderLine.Get_Value("VA077_MarginPercent"));
                line.Set_Value("VA077_OldSN", orderLine.Get_Value("VA077_OldSN"));
                line.Set_Value("VA077_ProductInfo", orderLine.Get_Value("VA077_ProductInfo"));
                line.Set_Value("VA077_PurchasePrice", orderLine.Get_Value("VA077_PurchasePrice"));
                line.Set_Value("VA077_RegEmail", orderLine.Get_Value("VA077_RegEmail"));
                line.Set_Value("VA077_SerialNo", orderLine.Get_Value("VA077_SerialNo"));
                line.Set_Value("VA077_UpdateFromVersn", orderLine.Get_Value("VA077_UpdateFromVersn"));
                line.Set_Value("VA077_UserRef_ID", orderLine.Get_Value("VA077_UserRef_ID"));
                line.Set_Value("VA077_StartDate", orderLine.Get_Value("VA077_StartDate"));
                line.Set_Value("VA077_EndDate", orderLine.Get_Value("VA077_EndDate"));

            }

            if (!line.Save())
            {
                ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                if (pp != null)
                    throw new ArgumentException("Could not create Invoice Line (o). " + pp.GetName());
                throw new Exception("Could not create Invoice Line (o)");
            }
            log.Fine(line.ToString());
        }

        /// <summary>
        /// Create Invoice Line from Shipment
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="ship">shipment header</param>
        /// <param name="sLine">shipment line</param>
        private void CreateLine(MOrder order, MInOut ship, MInOutLine sLine)
        {
            if (_invoice == null)
            {
                _invoice = new MInvoice(order, 0, _DateInvoiced);
                int _CountVA009 = Env.IsModuleInstalled("VA009_") ? 1 : 0;
                if (_CountVA009 > 0)
                {
                    int _PaymentMethod_ID = order.GetVA009_PaymentMethod_ID();
                    // during consolidation, payment method need to set that is defined on selected business partner. 
                    // If not defined on BP then it will set from order
                    // during sale cycle -- VA009_PaymentMethod_ID
                    // during purchase cycle -- VA009_PO_PaymentMethod_ID
                    int bpPamentMethod_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT " + (order.IsSOTrx() ? " VA009_PaymentMethod_ID " : " VA009_PO_PaymentMethod_ID ") +
                        @" FROM C_BPartner WHERE C_BPartner_ID = " + order.GetC_BPartner_ID(), null, Get_Trx()));

                    if (_ConsolidateDocument && bpPamentMethod_ID != 0)
                    {
                        _PaymentMethod_ID = bpPamentMethod_ID;
                    }
                    if (_PaymentMethod_ID > 0)
                    {
                        _invoice.SetVA009_PaymentMethod_ID(_PaymentMethod_ID);
                    }
                }

                int _CountVA026 = Env.IsModuleInstalled("VA026_") ? 1 : 0;
                if (_CountVA026 > 0)
                {
                    _invoice.SetVA026_LCDetail_ID(order.GetVA026_LCDetail_ID());
                }

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order
                if (_invoice.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    _invoice.SetC_IncoTerm_ID(order.GetC_IncoTerm_ID());
                }

                if (Env.IsModuleInstalled("VA077_"))
                {
                    //.setva077
                    _invoice.Set_Value("VA077_AdditionalAnnualCharge", order.Get_Value("VA077_AdditionalAnnualCharge"));
                    _invoice.Set_Value("VA077_ChangeStartDate", order.Get_Value("VA077_ChangeStartDate"));
                    _invoice.Set_Value("VA077_ContractCPEndDate", order.Get_Value("VA077_ContractCPEndDate"));
                    _invoice.Set_Value("VA077_ContractCPStartDate", order.Get_Value("VA077_ContractCPStartDate"));
                    _invoice.Set_Value("VA077_HistoricContractDate", order.Get_Value("VA077_HistoricContractDate"));
                    _invoice.Set_Value("VA077_IsLegalEntity", order.Get_Value("VA077_IsLegalEntity"));
                    _invoice.Set_Value("VA077_MarginPercent", order.Get_Value("VA077_MarginPercent"));
                    _invoice.Set_Value("VA077_NewAnnualContractTotal", order.Get_Value("VA077_NewAnnualContractTotal"));
                    _invoice.Set_Value("VA077_OldAnnualContractTotal", order.Get_Value("VA077_OldAnnualContractTotal"));
                    _invoice.Set_Value("VA077_PartialAmtCatchUp", order.Get_Value("VA077_PartialAmtCatchUp"));
                    _invoice.Set_Value("VA077_SalesCoWorker", order.Get_Value("VA077_SalesCoWorker"));
                    _invoice.Set_Value("VA077_SalesCoWorkerPer", order.Get_Value("VA077_SalesCoWorkerPer"));
                    _invoice.Set_Value("VA077_TotalMarginAmt", order.Get_Value("VA077_TotalMarginAmt"));
                    _invoice.Set_Value("VA077_TotalPurchaseAmt", order.Get_Value("VA077_TotalPurchaseAmt"));
                    _invoice.Set_Value("VA077_TotalSalesAmt", order.Get_Value("VA077_TotalSalesAmt"));
                }

                _invoice.SetConditionalFlag(MInvoice.CONDITIONALFLAG_PrepareIt);

                if (!_invoice.Save())
                {
                    ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                    if (pp != null)
                        throw new ArgumentException("Could not create Invoice (s). " + pp.GetName());
                    throw new Exception("Could not create Invoice (s)");
                }
            }
            #region Comment Create Shipment Comment Line
            //	Create Shipment Comment Line
            //if (_ship == null
            //    || _ship.GetM_InOut_ID() != ship.GetM_InOut_ID())
            //{
            //    MDocType dt = MDocType.Get(GetCtx(), ship.GetC_DocType_ID());
            //    if (_bp == null || _bp.GetC_BPartner_ID() != ship.GetC_BPartner_ID())
            //    {
            //        _bp = new MBPartner(GetCtx(), ship.GetC_BPartner_ID(), Get_TrxName());
            //    }

            //    //	Reference: Delivery: 12345 - 12.12.12
            //    MClient client = MClient.Get(GetCtx(), order.GetAD_Client_ID());
            //    String AD_Language = client.GetAD_Language();
            //    if (client.IsMultiLingualDocument() && _bp.GetAD_Language() != null)
            //    {
            //        AD_Language = _bp.GetAD_Language();
            //    }
            //    if (AD_Language == null)
            //    {
            //        // MessageBox.Show("Set base Language");
            //        //AD_Language = Language.getBaseAD_Language();
            //    }
            //    //java.text.SimpleDateFormat format = DisplayType.getDateFormat
            //    //    (DisplayType.Date, Language.getLanguage(AD_Language));

            //    //String reference = dt.GetPrintName(_bp.GetAD_Language())
            //    //    + ": " + ship.GetDocumentNo()
            //    //    + " - " + format.format(ship.GetMovementDate());
            //    String reference = dt.GetPrintName(_bp.GetAD_Language())
            //        + ": " + ship.GetDocumentNo()
            //        + " - " + ship.GetMovementDate();
            //    _ship = ship;
            //    //
            //    MInvoiceLine line = new MInvoiceLine(_invoice);
            //    line.SetIsDescription(true);
            //    line.SetDescription(reference);
            //    line.SetLine(_line + sLine.GetLine() - 2);
            //    if (!line.Save())
            //    {
            //        throw new Exception("Could not create Invoice Comment Line (sh)");
            //    }
            //    //	Optional Ship Address if not Bill Address
            //    if (order.GetBill_Location_ID() != ship.GetC_BPartner_Location_ID())
            //    {
            //        MLocation addr = MLocation.GetBPLocation(GetCtx(), ship.GetC_BPartner_Location_ID(), null);
            //        line = new MInvoiceLine(_invoice);
            //        line.SetIsDescription(true);
            //        line.SetDescription(addr.ToString());
            //        line.SetLine(_line + sLine.GetLine() - 1);
            //        if (!line.Save())
            //        {
            //            throw new Exception("Could not create Invoice Comment Line 2 (sh)");
            //        }
            //    }
            //}
            #endregion
            //	
            MInvoiceLine line1 = new MInvoiceLine(_invoice);
            line1.SetShipLine(sLine);
            line1.SetQtyEntered(sLine.GetQtyEntered());
            line1.SetQtyInvoiced(sLine.GetMovementQty());
            line1.SetLine(_line + sLine.GetLine());
            line1.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
            if (sLine.GetA_Asset_ID() > 0)
            {
                line1.SetA_Asset_ID(sLine.GetA_Asset_ID());
                if (line1.Get_ColumnIndex("VAFAM_AssetCost") > 0)
                {
                    int PAcctSchema_ID = 0;
                    int pCurrency_ID = 0;
                    MAcctSchema as1 = MClient.Get(GetCtx(), GetAD_Client_ID()).GetAcctSchema();
                    if (as1 != null)
                    {
                        PAcctSchema_ID = as1.GetC_AcctSchema_ID();
                        pCurrency_ID = as1.GetC_Currency_ID();
                    }
                    decimal LineNetAmt = Decimal.Multiply(line1.GetPriceActual(), line1.GetQtyEntered());
                    decimal AssetCost = GetAssetCost(sLine.GetA_Asset_ID(), sLine.GetM_Product_ID(), PAcctSchema_ID);
                    AssetCost = Decimal.Multiply(AssetCost, line1.GetQtyEntered());
                    if (LineNetAmt > 0)
                    {
                        LineNetAmt = MConversionRate.Convert(GetCtx(), LineNetAmt, _invoice.GetC_Currency_ID(), pCurrency_ID, _invoice.GetAD_Client_ID(), _invoice.GetAD_Org_ID());
                    }
                    decimal Diff = LineNetAmt - AssetCost;
                    line1.Set_Value("VAFAM_AssetCost", AssetCost);
                    line1.Set_Value("VAFAM_Difference", Diff);
                }
            }

            //190 - Set Print Description
            if (line1.Get_ColumnIndex("PrintDescription") >= 0)
                line1.Set_Value("PrintDescription", sLine.Get_Value("PrintDescription"));

            if (Env.IsModuleInstalled("VA077_"))
            {
                //invoice line
                line1.Set_Value("VA077_CNAutodesk", sLine.Get_Value("VA077_CNAutodesk"));
                line1.Set_Value("VA077_Duration", sLine.Get_Value("VA077_Duration"));
                line1.Set_Value("VA077_MarginAmt", sLine.Get_Value("VA077_MarginAmt"));
                line1.Set_Value("VA077_MarginPercent", sLine.Get_Value("VA077_MarginPercent"));
                line1.Set_Value("VA077_OldSN", sLine.Get_Value("VA077_OldSN"));
                line1.Set_Value("VA077_ProductInfo", sLine.Get_Value("VA077_ProductInfo"));
                line1.Set_Value("VA077_PurchasePrice", sLine.Get_Value("VA077_PurchasePrice"));
                line1.Set_Value("VA077_RegEmail", sLine.Get_Value("VA077_RegEmail"));
                line1.Set_Value("VA077_SerialNo", sLine.Get_Value("VA077_SerialNo"));
                line1.Set_Value("VA077_UpdateFromVersn", sLine.Get_Value("VA077_UpdateFromVersn"));
                line1.Set_Value("VA077_UserRef_ID", sLine.Get_Value("VA077_UserRef_ID"));
                line1.Set_Value("VA077_EndDate", sLine.Get_Value("VA077_EndDate"));
                line1.Set_Value("VA077_StartDate", sLine.Get_Value("VA077_StartDate"));
            }


            if (!line1.Save())
            {
                ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                if (pp != null)
                    throw new ArgumentException("Could not create Invoice Line (s). " + pp.GetName());
                throw new Exception("Could not create Invoice Line (s)");
            }

            //	Link
            //sLine.SetIsInvoiced(true);
            //if (!sLine.Save())
            //{
            //    ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
            //    if (pp != null)
            //        throw new ArgumentException("Could not update Shipment Line. " + pp.GetName());
            //    throw new Exception("Could not update Shipment Line");
            //}
            DB.ExecuteQuery("UPDATE M_InOutLine SET IsInvoiced = 'Y', Updated = SYSDATE WHERE M_InOutLine_ID = " + sLine.IsInvoiced(), null, Get_Trx());

            log.Fine(line1.ToString());
        }

        private decimal GetAssetCost(int Asset_ID, int Product_ID, int PAcctSchema_ID)
        {
            Decimal AssetCost = 0;
            StringBuilder _Sql = new StringBuilder();

            _Sql.Append(@"SELECT cost.CurrentCostPrice,
                          cost.FutureCostPrice,cost.M_COSTELEMENT_ID ,
                        cost.c_acctschema_id,cost.a_asset_id  , cost.m_product_id
                        FROM M_Cost cost
                        INNER JOIN A_Asset ass
                        ON(Ass.A_Asset_ID =cost.A_Asset_ID)
                        INNER JOIN m_product pro
                        ON(ass.m_product_id =pro.m_product_id AND pro.M_product_id=cost.m_Product_id)
                        INNER JOIN m_costelement costele
                        ON(costele.M_costelement_id =cost.m_costelement_id)
                        LEFT JOIN M_Product_Category acc
                        ON acc.M_Product_Category_ID=pro.M_Product_Category_ID
                        WHERE ass.A_Asset_ID =" + Asset_ID + @" 
                        AND ass.M_Product_ID=" + Product_ID + @"
                        AND costele.costelementtype='M'
                        AND cost.IsAssetCost = 'Y' 
                        AND costele.CostingMethod=acc.CostingMethod");
            if (PAcctSchema_ID > 0)
            {
                _Sql.Append(" AND cost.C_AcctSchema_ID = " + PAcctSchema_ID);
            }
            AssetCost = Util.GetValueOfDecimal(DB.ExecuteScalar(_Sql.ToString()));
            if (AssetCost == 0)
            {
                //Else Check costElement in Accounting Schema
                _Sql.Clear();
                _Sql.Append(@"SELECT cost.CurrentCostPrice,
                          cost.FutureCostPrice,cost.M_COSTELEMENT_ID ,
                        cost.c_acctschema_id,cost.a_asset_id  , cost.m_product_id
                        FROM M_Cost cost
                        INNER JOIN A_Asset ass
                        ON(Ass.A_Asset_ID =cost.A_Asset_ID)
                        INNER JOIN m_product pro
                        ON(ass.m_product_id =pro.m_product_id AND pro.M_product_id=cost.m_Product_id)
                        INNER JOIN m_costelement costele
                        ON(costele.M_costelement_id =cost.m_costelement_id)
                        INNER JOIN AD_ClientInfo ci ON ci.AD_Client_ID = ass.AD_Client_ID
                        left join C_AcctSchema accSch on accsch.C_AcctSchema_ID=ci.C_AcctSchema1_ID
                        WHERE ass.A_Asset_ID =" + Asset_ID + @" 
                        AND ass.M_Product_ID=" + Product_ID + @"
                        AND costele.costelementtype='M'
                        AND cost.IsAssetCost = 'Y' 
                        AND costele.CostingMethod=accsch.CostingMethod");
                if (PAcctSchema_ID > 0)
                {
                    _Sql.Append(" AND cost.C_AcctSchema_ID = " + PAcctSchema_ID);
                }
                AssetCost = Util.GetValueOfDecimal(DB.ExecuteScalar(_Sql.ToString()));
            }
            return AssetCost;
        }

        /// <summary>
        /// Complete Invoice
        /// </summary>
        private void CompleteInvoice()
        {
            if (_invoice != null)
            {
                // Update Subtotal, GrandTotal, backupWithholding Amount
                UpdateInvoiceHeader(_invoice);

                if (!_invoice.ProcessIt(_docAction))
                {
                    log.Warning("completeInvoice - failed: " + _invoice);
                }
                _invoice.SetConditionalFlag(null);
                _invoice.Save();
                //
                if (_msg != null)
                {
                    AddLog(_invoice.GetC_Invoice_ID(), Convert.ToDateTime(_invoice.GetDateInvoiced()), null, _msg + _invoice.GetDocumentNo());
                }
                else
                    AddLog(_invoice.GetC_Invoice_ID(), Convert.ToDateTime(_invoice.GetDateInvoiced()), null, _invoice.GetDocumentNo());
                _created++;
            }
            _invoice = null;
            _ship = null;
            _line = 0;
        }

        /// <summary>
        /// This function is used to update Header Tax
        /// </summary>
        /// <param name="invoice">Invoice Object</param>
        /// <returns>true when updated</returns>
        private bool UpdateInvoiceHeader(MInvoice invoice)
        {
            //	Update Invoice Header
            String sql = "UPDATE C_Invoice i"
            + " SET TotalLines="
                + "(SELECT COALESCE(SUM(LineNetAmt),0) FROM C_InvoiceLine il WHERE i.C_Invoice_ID=il.C_Invoice_ID) "
                + ", AmtDimSubTotal = null "      // reset Amount Dimension if Sub Total Amount is different
                + ", AmtDimGrandTotal = null "     // reset Amount Dimension if Grand Total Amount is different
                + (invoice.Get_ColumnIndex("WithholdingAmt") > 0 ? ", WithholdingAmt = ((SELECT COALESCE(SUM(WithholdingAmt),0) FROM C_InvoiceLine il WHERE i.C_Invoice_ID=il.C_Invoice_ID))" : "")
            + "WHERE C_Invoice_ID=" + invoice.GetC_Invoice_ID();
            int no = DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(1) #" + no);
            }

            if (invoice.IsTaxIncluded())
                sql = "UPDATE C_Invoice i "
                    + "SET GrandTotal=TotalLines "
                    + (invoice.Get_ColumnIndex("WithholdingAmt") > 0 ? " , GrandTotalAfterWithholding = (TotalLines - NVL(WithholdingAmt, 0) - NVL(BackupWithholdingAmount, 0)) " : "")
                    + "WHERE C_Invoice_ID=" + invoice.GetC_Invoice_ID();
            else
                sql = "UPDATE C_Invoice i "
                    + "SET GrandTotal=TotalLines+"
                        + "(SELECT ROUND((COALESCE(SUM(TaxAmt),0)),"+invoice.GetPrecision()+") FROM C_InvoiceTax it WHERE i.C_Invoice_ID=it.C_Invoice_ID) "
                        + (invoice.Get_ColumnIndex("WithholdingAmt") > 0 ? " , GrandTotalAfterWithholding = (TotalLines + (SELECT ROUND((COALESCE(SUM(TaxAmt),0))," + invoice.GetPrecision() + ") FROM C_InvoiceTax it WHERE i.C_Invoice_ID=it.C_Invoice_ID) - NVL(WithholdingAmt, 0) - NVL(BackupWithholdingAmount, 0))" : "")
                        + "WHERE C_Invoice_ID=" + invoice.GetC_Invoice_ID();
            no = DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(2) #" + no);
            }
            else
            {
                // calculate withholdng on header 
                if (invoice.GetC_Withholding_ID() > 0)
                {
                    if (!invoice.SetWithholdingAmount(invoice))
                    {
                        log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "WrongBackupWithholding"));
                    }
                    else
                    {
                        invoice.Save();
                    }
                }
            }
            return no == 1;
        }

    }
}
