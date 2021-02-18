using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
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
    class MVABIncomeTax:X_VAB_IncomeTax,DocAction
    {
        #region Variables
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared;
        private bool _forceCreation = false;
        private MVABIncomeTaxLines[] _lines = null;
        #endregion
        public MVABIncomeTax(Ctx ctx, int VAB_IncomeTax_ID, Trx trxName)
            : base(ctx, VAB_IncomeTax_ID, trxName)
        {
            if (VAB_IncomeTax_ID == 0)
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

        public MVABIncomeTax(Ctx ctx, DataRow dr, Trx trxName)
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
            MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            //SetIsReturnTrx(dt.IsReturnTrx());
            //SetIsSOTrx(dt.IsSOTrx());

            //Std Period open?
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

            //	Lines
            MVABIncomeTaxLines[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            ////	Convert DocType to Target
            //if (GetVAB_DocTypes_ID() != GetVAB_DocTypesTarget_ID())
            //{
            //    //	Cannot change Std to anything else if different warehouses
            //    if (GetVAB_DocTypes_ID() != 0)
            //    {
            //        MVABDocTypes dtOld = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
            //        if (MVABDocTypes.DOCSUBTYPESO_StandardOrder.Equals(dtOld.GetDocSubTypeSO())		//	From SO
            //            && !MVABDocTypes.DOCSUBTYPESO_StandardOrder.Equals(dt.GetDocSubTypeSO()))	//	To !SO
            //        {
            //            for (int i = 0; i < lines.Length; i++)
            //            {
            //                if (lines[i].GetVAM_Warehouse_ID() != GetVAM_Warehouse_ID())
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
            //        || GetVAB_DocTypes_ID() == 0)
            //    {
            //        SetVAB_DocTypes_ID(GetVAB_DocTypesTarget_ID());
            //    }
            //    else	//	convert only if offer
            //    {
            //        if (dt.IsOffer())
            //            SetVAB_DocTypes_ID(GetVAB_DocTypesTarget_ID());
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
            //    + "FROM VAB_OrderLine ol"
            //    + " INNER JOIN VAM_Product p ON (ol.VAM_Product_ID=p.VAM_Product_ID)"
            //    + " INNER JOIN VAM_PFeature_Set pas ON (p.VAM_PFeature_Set_ID=pas.VAM_PFeature_Set_ID) "
            //    + "WHERE pas.MandatoryType" + mandatoryType
            //    + " AND ol.VAM_PFeature_SetInstance_ID IS NULL"
            //    + " AND ol.VAB_Order_ID=" + GetVAB_Order_ID();
            //int no = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
            //if (no != 0)
            //{
            //    _processMsg = "@LinesWithoutProductAttribute@ (" + no + ")";
            //    return DocActionVariables.STATUS_INVALID;
            //}

            ////	Lines
            //if (ExplodeBOM())
            //    lines = GetLines(true, "VAM_Product_ID");
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
            //    MBPartner bp = MBPartner.Get(GetCtx(), GetVAB_BusinessPartner_ID());
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
            //        GetGrandTotal(), GetVAB_Currency_ID(), GetDateOrdered(),
            //        GetVAB_CurrencyType_ID(), GetVAF_Client_ID(), GetVAF_Org_ID());
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


        public MVABIncomeTaxLines[] GetLines(bool requery)
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

        public MVABIncomeTaxLines[] GetLines(String whereClause)
        {
            List<MVABIncomeTaxLines> list = new List<MVABIncomeTaxLines>();
            StringBuilder sql = new StringBuilder("SELECT * FROM VAB_IncomeTaxLines WHERE VAB_IncomeTax_ID=" + GetVAB_IncomeTax_ID() + "");
            if (whereClause != null)
                sql.Append(whereClause);            
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MVABIncomeTaxLines ol = new MVABIncomeTaxLines(GetCtx(), dr, Get_TrxName());
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
            MVABIncomeTaxLines[] lines = new MVABIncomeTaxLines[list.Count];
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
                MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
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
                //    if (MVABDocTypes.DOCSUBTYPESO_Proposal.Equals(DocSubTypeSO)
                //        || MVABDocTypes.DOCSUBTYPESO_Quotation.Equals(DocSubTypeSO))
                //    {
                //        //	Binding
                //        if (MVABDocTypes.DOCSUBTYPESO_Quotation.Equals(DocSubTypeSO))
                //            ReserveStock(dt, GetLines(true, "VAM_Product_ID"));
                //        SetProcessed(true);
                //        SetDocAction(DOCACTION_Close);
                //        return DocActionVariables.STATUS_COMPLETED;
                //    }
                //    //	Waiting Payment - until we have a payment
                //    if (!_forceCreation
                //        && MVABDocTypes.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO)
                //        && GetVAB_Payment_ID() == 0 && GetVAB_CashJRNLLine_ID() == 0)
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
                does not get recreated in the afterSave procedure of the MVABOrderLine class */
                //SetProcessed(true);

                //bool realTimePOS = false;

                ////	Create SO Shipment - Force Shipment
                //MVAMInvInOut shipment = null;
                //if (MVABDocTypes.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)		//	(W)illCall(I)nvoice
                //    || MVABDocTypes.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                //    || MVABDocTypes.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO)			//	(W)alkIn(R)eceipt
                //    || MVABDocTypes.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                //{
                //    if (!DELIVERYRULE_Force.Equals(GetDeliveryRule()))
                //        SetDeliveryRule(DELIVERYRULE_Force);
                //    //
                //    shipment = CreateShipment(dt, realTimePOS ? null : GetDateOrdered());
                //    if (shipment == null)
                //        return DocActionVariables.STATUS_INVALID;
                //    Info.Append("@VAM_Inv_InOut_ID@: ").Append(shipment.GetDocumentNo());
                //    String msg = shipment.GetProcessMsg();
                //    if (msg != null && msg.Length > 0)
                //        Info.Append(" (").Append(msg).Append(")");
                //}


                //	Create SO Invoice - Always invoice complete Order
                //if (MVABDocTypes.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO)
                //    || MVABDocTypes.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)
                //    || MVABDocTypes.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                //{
                //    try
                //    {
                //        DateTime? tSet = realTimePOS ? null : GetDateOrdered();
                //        MVABInvoice invoice = CreateInvoice(dt, shipment, tSet);
                //        if (invoice == null)
                //            return DocActionVariables.STATUS_INVALID;
                //        Info.Append(" - @VAB_Invoice_ID@: ").Append(invoice.GetDocumentNo());
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
            //MVABOrderLine[] lines = GetLines(true, "VAM_Product_ID");
            //log.Info(ToString());
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    MVABOrderLine line = lines[i];
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
            //MOrder ord = new MOrder(GetCtx(), GetVAB_Order_ID(), Get_TrxName());
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
            //MVABOrderLine[] lines = GetLines(true, "VAM_Product_ID");
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    MVABOrderLine line = lines[i];
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

                MVABDocTypes dt = MVABDocTypes.Get(GetCtx(), GetVAB_DocTypes_ID());
                String DocSubTypeSO = dt.GetDocSubTypeSO();

                //	Replace Prepay with POS to revert all doc
                if (MVABDocTypes.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                {
                    MVABDocTypes newDT = null;
                    MVABDocTypes[] dts = MVABDocTypes.GetOfClient(GetCtx());
                    for (int i = 0; i < dts.Length; i++)
                    {
                        MVABDocTypes type = dts[i];
                        if (MVABDocTypes.DOCSUBTYPESO_PrepayOrder.Equals(type.GetDocSubTypeSO()))
                        {
                            if (type.IsDefault() || newDT == null)
                                newDT = type;
                        }
                    }
                    if (newDT == null)
                        return false;
                    else
                    {
                        SetVAB_DocTypes_ID(newDT.GetVAB_DocTypes_ID());
                       // SetIsReturnTrx(newDT.IsReturnTrx());
                    }
                }

                //	PO - just re-open
                //if (!IsSOTrx())
                //{
                //    log.Info("Existing documents not modified - " + dt);
                //}
                ////	Reverse Direct Documents
                //else if (MVABDocTypes.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)	//	(W)illCall(I)nvoice
                //    || MVABDocTypes.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                //    || MVABDocTypes.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))			//	(W)alkIn(R)eceipt
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

        public int GetVAB_Currency_ID()
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
                Info.Append("@VAM_Inv_InOut_ID@:");
                //MVAMInvInOut[] shipments = GetShipments(false);	//	get all (line based)
                //for (int i = 0; i < shipments.Length; i++)
                //{
                //    MVAMInvInOut ship = shipments[i];
                //    //	if closed - ignore
                //    if (MVAMInvInOut.DOCSTATUS_Closed.Equals(ship.GetDocStatus())
                //        || MVAMInvInOut.DOCSTATUS_Reversed.Equals(ship.GetDocStatus())
                //        || MVAMInvInOut.DOCSTATUS_Voided.Equals(ship.GetDocStatus()))
                //        continue;
                //    ship.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MVAMInvInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()))
                //    {
                //        if (ship.VoidIt())
                //            ship.SetDocStatus(MVAMInvInOut.DOCSTATUS_Voided);
                //    }
                //    //	Create new Reversal with only that order
                //    else if (!ship.IsOnlyForOrder(this))
                //    {
                //        ship.ReverseCorrectIt(this);
                //        //	shipLine.setDocStatus(MVAMInvInOut.DOCSTATUS_Reversed);
                //        Info.Append(" Parial ").Append(ship.GetDocumentNo());
                //    }
                //    else if (ship.ReverseCorrectIt()) //	completed shipment
                //    {
                //        ship.SetDocStatus(MVAMInvInOut.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(ship.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse Shipment " + ship;
                //        return false;
                //    }
                //    ship.SetDocAction(MVAMInvInOut.DOCACTION_None);
                //    ship.Save(Get_TrxName());
                //}	//	for all shipments

                //	Reverse All *Invoices*
                Info.Append(" - @VAB_Invoice_ID@:");
                //MVABInvoice[] invoices = GetInvoices(false);	//	get all (line based)
                //for (int i = 0; i < invoices.Length; i++)
                //{
                //    MVABInvoice invoice = invoices[i];
                //    //	if closed - ignore
                //    if (MVABInvoice.DOCSTATUS_Closed.Equals(invoice.GetDocStatus())
                //        || MVABInvoice.DOCSTATUS_Reversed.Equals(invoice.GetDocStatus())
                //        || MVABInvoice.DOCSTATUS_Voided.Equals(invoice.GetDocStatus()))
                //        continue;
                //    invoice.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MVABInvoice.DOCSTATUS_Completed.Equals(invoice.GetDocStatus()))
                //    {
                //        if (invoice.VoidIt())
                //            invoice.SetDocStatus(MVABInvoice.DOCSTATUS_Voided);
                //    }
                //    else if (invoice.ReverseCorrectIt())	//	completed invoice
                //    {
                //        invoice.SetDocStatus(MVABInvoice.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(invoice.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse Invoice " + invoice;
                //        return false;
                //    }
                //    invoice.SetDocAction(MVABInvoice.DOCACTION_None);
                //    invoice.Save(Get_TrxName());
                //}	//	for all shipments

                //	Reverse All *RMAs*
                Info.Append("@VAB_Order_ID@:");
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
                //            rma.SetDocStatus(MVAMInvInOut.DOCSTATUS_Voided);
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
                //    rma.SetDocAction(MVAMInvInOut.DOCACTION_None);
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
