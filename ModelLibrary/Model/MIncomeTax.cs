using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    class MIncomeTax:X_C_IncomeTax,DocAction
    {
        #region Variables
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared;
        private bool _forceCreation = false;
        private MIncomeTaxLines[] _lines = null;
        #endregion
        public MIncomeTax(Ctx ctx, int C_IncomeTax_ID, Trx trxName)
            : base(ctx, C_IncomeTax_ID, trxName)
        {
            if (C_IncomeTax_ID == 0)
            {
                SetDocStatus(DOCSTATUS_Drafted);
                SetDocAction(DOCACTION_Prepare);
                SetIsApproved(false);
                base.SetProcessed(false);
                SetProcessing(false);
                SetPosted(false);
                //SetDateAcct(Convert.ToDateTime(DateTime.Now));
                //SetTransactionDate(Convert.ToDateTime(DateTime.Now));
            }

        }

        public MIncomeTax(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Set DocAction
        /// </summary>
        /// <param name="docAction">doc action</param>
        public new void SetDocAction(String docAction)
        {
            SetDocAction(docAction, false);
        }

        /// <summary>
        /// Set DocAction
        /// </summary>
        /// <param name="docAction">doc action</param>
        /// <param name="forceCreation">force creation</param>
        public void SetDocAction(String docAction, bool forceCreation)
        {
            base.SetDocAction(docAction);
            _forceCreation = forceCreation;
        }

        public bool ProcessIt(string processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        public bool UnlockIt()
        {
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        public string PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            //SetIsReturnTrx(dt.IsReturnTrx());
            //SetIsSOTrx(dt.IsSOTrx());

            	//Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }
            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            //	Lines
            MIncomeTaxLines[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            ////	Convert DocType to Target
            //if (GetC_DocType_ID() != GetC_DocTypeTarget_ID())
            //{
            //    //	Cannot change Std to anything else if different warehouses
            //    if (GetC_DocType_ID() != 0)
            //    {
            //        MDocType dtOld = MDocType.Get(GetCtx(), GetC_DocType_ID());
            //        if (MDocType.DOCSUBTYPESO_StandardOrder.Equals(dtOld.GetDocSubTypeSO())		//	From SO
            //            && !MDocType.DOCSUBTYPESO_StandardOrder.Equals(dt.GetDocSubTypeSO()))	//	To !SO
            //        {
            //            for (int i = 0; i < lines.Length; i++)
            //            {
            //                if (lines[i].GetM_Warehouse_ID() != GetM_Warehouse_ID())
            //                {
            //                    log.Warning("different Warehouse " + lines[i]);
            //                    _processMsg = "@CannotChangeDocType@";
            //                    return DocActionVariables.STATUS_INVALID;
            //                }
            //            }
            //        }
            //    }

            //    //	New or in Progress/Invalid
            //    if (DOCSTATUS_Drafted.Equals(GetDocStatus())
            //        || DOCSTATUS_InProgress.Equals(GetDocStatus())
            //        || DOCSTATUS_Invalid.Equals(GetDocStatus())
            //        || GetC_DocType_ID() == 0)
            //    {
            //        SetC_DocType_ID(GetC_DocTypeTarget_ID());
            //    }
            //    else	//	convert only if offer
            //    {
            //        if (dt.IsOffer())
            //            SetC_DocType_ID(GetC_DocTypeTarget_ID());
            //        else
            //        {
            //            _processMsg = "@CannotChangeDocType@";
            //            return DocActionVariables.STATUS_INVALID;
            //        }
            //    }
            //}	//	convert DocType

            //	Mandatory Product Attribute Set Instance
            //String mandatoryType = "='Y'";	//	IN ('Y','S')
            //String sql = "SELECT COUNT(*) "
            //    + "FROM C_OrderLine ol"
            //    + " INNER JOIN M_Product p ON (ol.M_Product_ID=p.M_Product_ID)"
            //    + " INNER JOIN M_AttributeSet pas ON (p.M_AttributeSet_ID=pas.M_AttributeSet_ID) "
            //    + "WHERE pas.MandatoryType" + mandatoryType
            //    + " AND ol.M_AttributeSetInstance_ID IS NULL"
            //    + " AND ol.C_Order_ID=" + GetC_Order_ID();
            //int no = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
            //if (no != 0)
            //{
            //    _processMsg = "@LinesWithoutProductAttribute@ (" + no + ")";
            //    return DocActionVariables.STATUS_INVALID;
            //}

            ////	Lines
            //if (ExplodeBOM())
            //    lines = GetLines(true, "M_Product_ID");
            //if (!ReserveStock(dt, lines))
            //{
            //    _processMsg = "Cannot reserve Stock";
            //    return DocActionVariables.STATUS_INVALID;
            //}
            //if (!CalculateTaxTotal())
            //{
            //    _processMsg = "Error calculating tax";
            //    return DocActionVariables.STATUS_INVALID;
            //}

            //	Credit Check
            //if (IsSOTrx() && !IsReturnTrx())
            //{
            //    MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
            //    if (MBPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
            //    {
            //        _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
            //            + bp.GetTotalOpenBalance()
            //            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
            //        return DocActionVariables.STATUS_INVALID;
            //    }
            //    if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus()))
            //    {
            //        _processMsg = "@BPartnerCreditHold@ - @TotalOpenBalance@="
            //            + bp.GetTotalOpenBalance()
            //            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
            //        return DocActionVariables.STATUS_INVALID;
            //    }
            //    Decimal grandTotal = MConversionRate.ConvertBase(GetCtx(),
            //        GetGrandTotal(), GetC_Currency_ID(), GetDateOrdered(),
            //        GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
            //    if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus(grandTotal)))
            //    {
            //        _processMsg = "@BPartnerOverOCreditHold@ - @TotalOpenBalance@="
            //            + bp.GetTotalOpenBalance() + ", @GrandTotal@=" + grandTotal
            //            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
            //        return DocActionVariables.STATUS_INVALID;
            //    }
            //}

            _justPrepared = true;
            if (_justPrepared)
            {
            }
            // dont uncomment
            //if (!DOCACTION_Complete.Equals(getDocAction()))		don't set for just prepare 
            //		setDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }


        public MIncomeTaxLines[] GetLines(bool requery)
        {
            try
            {
                if (_lines != null && !requery)
                {
                    return _lines;
                }

                _lines = GetLines(null);

            }
            catch
            {

                //ShowMessage.Error("MOrder", null, "GetLines");
            }
            return _lines;
        }

        public MIncomeTaxLines[] GetLines(String whereClause)
        {
            List<MIncomeTaxLines> list = new List<MIncomeTaxLines>();
            StringBuilder sql = new StringBuilder("SELECT * FROM C_IncomeTaxLines WHERE C_IncomeTax_ID=" + GetC_IncomeTax_ID() + "");
            if (whereClause != null)
                sql.Append(whereClause);            
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MIncomeTaxLines ol = new MIncomeTaxLines(GetCtx(), dr, Get_TrxName());
                        //ol.SetHeaderInfo(this);
                        list.Add(ol);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            //
            MIncomeTaxLines[] lines = new MIncomeTaxLines[list.Count];
            lines = list.ToArray();
            return lines;
        }

        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        public string CompleteIt()
        {
            try
            {
                MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
                String DocSubTypeSO = dt.GetDocSubTypeSO();

                //	Just prepare
                if (DOCACTION_Prepare.Equals(GetDocAction()))
                {
                    SetProcessed(false);
                    return DocActionVariables.STATUS_INPROGRESS;
                }

                //if (!IsReturnTrx())
                //{
                //    //	Offers
                //    if (MDocType.DOCSUBTYPESO_Proposal.Equals(DocSubTypeSO)
                //        || MDocType.DOCSUBTYPESO_Quotation.Equals(DocSubTypeSO))
                //    {
                //        //	Binding
                //        if (MDocType.DOCSUBTYPESO_Quotation.Equals(DocSubTypeSO))
                //            ReserveStock(dt, GetLines(true, "M_Product_ID"));
                //        SetProcessed(true);
                //        SetDocAction(DOCACTION_Close);
                //        return DocActionVariables.STATUS_COMPLETED;
                //    }
                //    //	Waiting Payment - until we have a payment
                //    if (!_forceCreation
                //        && MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO)
                //        && GetC_Payment_ID() == 0 && GetC_CashLine_ID() == 0)
                //    {
                //        SetProcessed(true);
                //        return DocActionVariables.STATUS_WAITINGPAYMENT;
                //    }

                //    //	Re-Check
                //    if (!_justPrepared)
                //    {
                //        String status = PrepareIt();
                //        if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                //            return status;
                //    }
                //}

                	//Implicit Approval
                if (!IsApproved())
                    ApproveIt();
                GetLines(true);
                log.Info(ToString());
                StringBuilder Info = new StringBuilder();

                /* nnayak - Bug 1720003 - We need to set the processed flag so the Tax Summary Line
                does not get recreated in the afterSave procedure of the MOrderLine class */
                //SetProcessed(true);

                //bool realTimePOS = false;

                ////	Create SO Shipment - Force Shipment
                //MInOut shipment = null;
                //if (MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)		//	(W)illCall(I)nvoice
                //    || MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                //    || MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO)			//	(W)alkIn(R)eceipt
                //    || MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                //{
                //    if (!DELIVERYRULE_Force.Equals(GetDeliveryRule()))
                //        SetDeliveryRule(DELIVERYRULE_Force);
                //    //
                //    shipment = CreateShipment(dt, realTimePOS ? null : GetDateOrdered());
                //    if (shipment == null)
                //        return DocActionVariables.STATUS_INVALID;
                //    Info.Append("@M_InOut_ID@: ").Append(shipment.GetDocumentNo());
                //    String msg = shipment.GetProcessMsg();
                //    if (msg != null && msg.Length > 0)
                //        Info.Append(" (").Append(msg).Append(")");
                //}


                //	Create SO Invoice - Always invoice complete Order
                //if (MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO)
                //    || MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)
                //    || MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                //{
                //    try
                //    {
                //        DateTime? tSet = realTimePOS ? null : GetDateOrdered();
                //        MInvoice invoice = CreateInvoice(dt, shipment, tSet);
                //        if (invoice == null)
                //            return DocActionVariables.STATUS_INVALID;
                //        Info.Append(" - @C_Invoice_ID@: ").Append(invoice.GetDocumentNo());
                //        String msg = invoice.GetProcessMsg();
                //        if (msg != null && msg.Length > 0)
                //            Info.Append(" (").Append(msg).Append(")");
                //    }
                //    catch (NullReferenceException ex)
                //    {
                //        //ShowMessage.Error("Moder",null,"Completeit");
                //    }
                //}

                //	Counter Documents
                //MOrder counter = CreateCounterDoc();
                //if (counter != null)
                //    Info.Append(" - @CounterDoc@: @Order@=").Append(counter.GetDocumentNo());
                ////User Validation
                //String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
                //if (valid != null)
                //{
                //    if (Info.Length > 0)
                //        Info.Append(" - ");
                //    Info.Append(valid);
                //    _processMsg = Info.ToString();
                //    return DocActionVariables.STATUS_INVALID;
                //}

                SetProcessed(true);
                _processMsg = Info.ToString();                
                SetDocAction(DOCACTION_Close);
            }
            catch
            {
                //ShowMessage.Error("MOrder",null,"CompleteIt");
            }
            return DocActionVariables.STATUS_COMPLETED;
        }

        public bool VoidIt()
        {
            //MOrderLine[] lines = GetLines(true, "M_Product_ID");
            //log.Info(ToString());
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    MOrderLine line = lines[i];
            //    Decimal old = line.GetQtyOrdered();
            //    if (System.Math.Sign(old) != 0)
            //    {
            //        line.AddDescription(Msg.GetMsg(GetCtx(), "Voided", true) + " (" + old + ")");
            //        line.SetQtyLostSales(old);
            //        line.SetQty(Env.ZERO);
            //        line.SetLineNetAmt(Env.ZERO);
            //        line.Save(Get_TrxName());
            //    }
            //}
            //AddDescription(Msg.GetMsg(GetCtx(), "Voided", true));
            ////	Clear Reservations
            //if (!ReserveStock(null, lines))
            //{
            //    _processMsg = "Cannot unreserve Stock (void)";
            //    return false;
            //}

            if (!CreateReversals())
                return false;

            //************* Changed ***************************
            // Set Status at Order to Rejected if it is Sales Order 
            //MOrder ord = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
            //if (IsSOTrx())
            //{
            //    ord.SetStatusCode("R");
            //    ord.Save();
            //}

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        public bool CloseIt()
        {
            log.Info(ToString());

            //	Close Not delivered Qty - SO/PO
            //MOrderLine[] lines = GetLines(true, "M_Product_ID");
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    MOrderLine line = lines[i];
            //    Decimal old = line.GetQtyOrdered();
            //    if (old.CompareTo(line.GetQtyDelivered()) != 0)
            //    {
            //        line.SetQtyLostSales(Decimal.Subtract(line.GetQtyOrdered(), line.GetQtyDelivered()));
            //        line.SetQtyOrdered(line.GetQtyDelivered());
            //        //	QtyEntered unchanged
            //        line.AddDescription("Close (" + old + ")");
            //        line.Save(Get_TrxName());
            //    }
            //}
            //	Clear Reservations
            //if (!ReserveStock(null, lines))
            //{
            //    _processMsg = "Cannot unreserve Stock (close)";
            //    return false;
            //}
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return VoidIt();
        }

        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        public bool ReActivateIt()
        {
            try
            {
                log.Info(ToString());

                MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
                String DocSubTypeSO = dt.GetDocSubTypeSO();

                //	Replace Prepay with POS to revert all doc
                if (MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                {
                    MDocType newDT = null;
                    MDocType[] dts = MDocType.GetOfClient(GetCtx());
                    for (int i = 0; i < dts.Length; i++)
                    {
                        MDocType type = dts[i];
                        if (MDocType.DOCSUBTYPESO_PrepayOrder.Equals(type.GetDocSubTypeSO()))
                        {
                            if (type.IsDefault() || newDT == null)
                                newDT = type;
                        }
                    }
                    if (newDT == null)
                        return false;
                    else
                    {
                        SetC_DocType_ID(newDT.GetC_DocType_ID());
                       // SetIsReturnTrx(newDT.IsReturnTrx());
                    }
                }

                //	PO - just re-open
                //if (!IsSOTrx())
                //{
                //    log.Info("Existing documents not modified - " + dt);
                //}
                ////	Reverse Direct Documents
                //else if (MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)	//	(W)illCall(I)nvoice
                //    || MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                //    || MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))			//	(W)alkIn(R)eceipt
                //{
                    //if (!CreateReversals())
                    //    return false;
            //}
                else
                {
                    log.Info("Existing documents not modified - SubType=" + DocSubTypeSO);
                }

                SetDocAction(DOCACTION_Complete);
                SetProcessed(false);
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetBPartner");
            }
            return true;
        }

        public string GetSummary()
        {
            return "";
        }

        public string GetDocumentNo()
        {
            return "";
        }

        public string GetDocumentInfo()
        {
            return "";
        }

        public FileInfo CreatePDF()
        {
            return null;
        }

        public string GetProcessMsg()
        {
            return _processMsg;
        }

        public int GetDoc_User_ID()
        {
            return 0;
        }

        public int GetC_Currency_ID()
        {
            return 0;
        }

        public decimal GetApprovalAmt()
        {
            return 0;
        }

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
            return "";
        }
        private bool CreateReversals()
        {
            try
            {
                //	Cancel only Sales 
                //if (!IsSOTrx())
                //    return true;

                log.Info("");
                StringBuilder Info = new StringBuilder();

                //	Reverse All *Shipments*
                Info.Append("@M_InOut_ID@:");
                //MInOut[] shipments = GetShipments(false);	//	get all (line based)
                //for (int i = 0; i < shipments.Length; i++)
                //{
                //    MInOut ship = shipments[i];
                //    //	if closed - ignore
                //    if (MInOut.DOCSTATUS_Closed.Equals(ship.GetDocStatus())
                //        || MInOut.DOCSTATUS_Reversed.Equals(ship.GetDocStatus())
                //        || MInOut.DOCSTATUS_Voided.Equals(ship.GetDocStatus()))
                //        continue;
                //    ship.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()))
                //    {
                //        if (ship.VoidIt())
                //            ship.SetDocStatus(MInOut.DOCSTATUS_Voided);
                //    }
                //    //	Create new Reversal with only that order
                //    else if (!ship.IsOnlyForOrder(this))
                //    {
                //        ship.ReverseCorrectIt(this);
                //        //	shipLine.setDocStatus(MInOut.DOCSTATUS_Reversed);
                //        Info.Append(" Parial ").Append(ship.GetDocumentNo());
                //    }
                //    else if (ship.ReverseCorrectIt()) //	completed shipment
                //    {
                //        ship.SetDocStatus(MInOut.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(ship.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse Shipment " + ship;
                //        return false;
                //    }
                //    ship.SetDocAction(MInOut.DOCACTION_None);
                //    ship.Save(Get_TrxName());
                //}	//	for all shipments

                //	Reverse All *Invoices*
                Info.Append(" - @C_Invoice_ID@:");
                //MInvoice[] invoices = GetInvoices(false);	//	get all (line based)
                //for (int i = 0; i < invoices.Length; i++)
                //{
                //    MInvoice invoice = invoices[i];
                //    //	if closed - ignore
                //    if (MInvoice.DOCSTATUS_Closed.Equals(invoice.GetDocStatus())
                //        || MInvoice.DOCSTATUS_Reversed.Equals(invoice.GetDocStatus())
                //        || MInvoice.DOCSTATUS_Voided.Equals(invoice.GetDocStatus()))
                //        continue;
                //    invoice.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MInvoice.DOCSTATUS_Completed.Equals(invoice.GetDocStatus()))
                //    {
                //        if (invoice.VoidIt())
                //            invoice.SetDocStatus(MInvoice.DOCSTATUS_Voided);
                //    }
                //    else if (invoice.ReverseCorrectIt())	//	completed invoice
                //    {
                //        invoice.SetDocStatus(MInvoice.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(invoice.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse Invoice " + invoice;
                //        return false;
                //    }
                //    invoice.SetDocAction(MInvoice.DOCACTION_None);
                //    invoice.Save(Get_TrxName());
                //}	//	for all shipments

                //	Reverse All *RMAs*
                Info.Append("@C_Order_ID@:");
                //MOrder[] rmas = GetRMAs();
                //for (int i = 0; i < rmas.Length; i++)
                //{
                //    MOrder rma = rmas[i];
                //    //	if closed - ignore
                //    if (MOrder.DOCSTATUS_Closed.Equals(rma.GetDocStatus())
                //        || MOrder.DOCSTATUS_Reversed.Equals(rma.GetDocStatus())
                //        || MOrder.DOCSTATUS_Voided.Equals(rma.GetDocStatus()))
                //        continue;
                //    rma.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MOrder.DOCSTATUS_Completed.Equals(rma.GetDocStatus()))
                //    {
                //        if (rma.VoidIt())
                //            rma.SetDocStatus(MInOut.DOCSTATUS_Voided);
                //    }
                //    //	Create new Reversal with only that order
                //    else if (rma.ReverseCorrectIt()) //	completed shipment
                //    {
                //        rma.SetDocStatus(MOrder.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(rma.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse RMA " + rma;
                //        return false;
                //    }
                //    rma.SetDocAction(MInOut.DOCACTION_None);
                //    rma.Save(Get_TrxName());
                //}	//	for all shipments


                _processMsg = Info.ToString();
            }
            catch
            {
                //ShowMessage.Error("MOrder",null,"CreateReversals");

            }
            return true;
        }
    }
}
