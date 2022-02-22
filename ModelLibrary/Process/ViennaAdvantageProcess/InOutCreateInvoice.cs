/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : InOutCreateInvoice
    * Purpose        : Create (Generate) Invoice from Shipment
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Raghunandan     20-Aug-2009
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
using VAdvantage.ProcessEngine;


namespace ViennaAdvantage.Process
{
    public class InOutCreateInvoice : VAdvantage.ProcessEngine.SvrProcess
    {
        //	Shipment					
        private int _M_InOut_ID = 0;
        //Price List Version		
        private int _M_PriceList_ID = 0;
        //Document No					
        private String _InvoiceDocumentNo = null;
        //Document Type
        private int _C_DocType_ID = 0;
        //Checkbox for Generating charges proportional to line quantities on Invoice line. By Sukhwnder on 14 Dec, 2017
        private bool _GenerateCharges = false;


        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
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
                else if (name.Equals("M_PriceList_ID"))
                {
                    _M_PriceList_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("InvoiceDocumentNo"))
                {
                    _InvoiceDocumentNo = (String)para[i].GetParameter();
                }
                else if (name.Equals("GenerateCharges"))
                {
                    _GenerateCharges = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("C_DocType_ID"))
                {
                    _C_DocType_ID = para[i].GetParameterAsInt(); ;
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _M_InOut_ID = GetRecord_ID();
        }

        /// <summary>
        /// Create Invoice.
        /// </summary>
        /// <returns>document no</returns>
        protected override String DoIt()
        {
            StringBuilder invDocumentNo = new StringBuilder();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT Count(M_Inout_ID) FROM M_Inout WHERE ISSOTRX='Y' AND M_Inout_ID=" + GetRecord_ID()));
            MInOut ship = null;
            bool isAllownonItem = Util.GetValueOfString(GetCtx().GetContext("$AllowNonItem")).Equals("Y");

            // check if reference found on Provisional Invoice, then not to create Invoice
            if (_M_InOut_ID > 0 && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(iol.M_InOut_ID)
                                      FROM C_ProvisionalInvoiceLine pil
                                      INNER JOIN C_ProvisionalInvoice  pi ON (pi.C_ProvisionalInvoice_ID = pil.C_ProvisionalInvoice_ID)
                                      LEFT JOIN M_InOutLine iol ON (iol.M_InOutLine_ID = pil.M_InOutLine_ID)
                                      WHERE pi.DocStatus NOT IN ( 'RE', 'VO' ) AND iol.M_InOut_ID = " + _M_InOut_ID, null, Get_Trx())) > 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx() , "VIS_ProvisionalInvoiceCreated"));
            }

            if (count > 0)
            {
                if (_M_InOut_ID == 0)
                {
                    throw new ArgumentException("No Shipment");
                }
                //
                ship = new MInOut(GetCtx(), _M_InOut_ID, Get_Trx());
                if (ship.Get_ID() == 0)
                {
                    throw new ArgumentException("Shipment not found");
                }
                if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()) && !MInOut.DOCSTATUS_Closed.Equals(ship.GetDocStatus()))
                {
                    // JID_0750: done by Bharat on 05 Feb 2019 if Customer Return document and status is not complete it should give message "Customer Return Not Completed".
                    if (ship.IsReturnTrx())
                    {
                        throw new ArgumentException(Msg.GetMsg(GetCtx(), "CustomerReturnNotCompleted"));
                    }
                    else
                    {
                        throw new ArgumentException(Msg.GetMsg(GetCtx(), "ShipmentNotCompleted"));
                    }
                }
            }
            else
            {
                if (_M_InOut_ID == 0)
                {
                    throw new ArgumentException("No Material Receipt");
                }
                //
                ship = new MInOut(GetCtx(), _M_InOut_ID, Get_Trx());
                if (ship.Get_ID() == 0)
                {
                    throw new ArgumentException("Material Receipt not found");
                }
                if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()) && !MInOut.DOCSTATUS_Closed.Equals(ship.GetDocStatus()))
                {
                    // JID_0750: done by Bharat on 05 Feb 2019 if Return to vendor document and status is not complete it should give message "Return To Vendor Not Completed".
                    if (ship.IsReturnTrx())
                    {
                        throw new ArgumentException(Msg.GetMsg(GetCtx(), "VendorReturnNotCompleted"));
                    }
                    else
                    {
                        throw new ArgumentException(Msg.GetMsg(GetCtx(), "MRNotCompleted"));
                    }
                }
            }

            // When record contain more than single order and order having different Payment term or Price List then not to generate invoices
            // JID_0976 - For conversion Type
            if (ship.GetC_Order_ID() > 0 && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT  COUNT(DISTINCT  c_order.m_pricelist_id) +  count(distinct c_order.c_paymentterm_id) + count(distinct COALESCE( c_order.C_ConversionType_ID , " + MConversionType.GetDefault(GetAD_Client_ID()) + @"))  as recordcount
                            FROM m_inoutline INNER JOIN c_orderline ON m_inoutline.c_orderline_id = c_orderline.c_orderline_id
                            INNER JOIN c_order ON c_order.c_order_id = c_orderline.c_order_id
                            WHERE m_inoutline.m_inout_id = " + _M_InOut_ID + @"  GROUP BY   m_inoutline.m_inout_id ", null, Get_Trx())) > 3)
            {
                if (ship.IsSOTrx())
                {
                    //Different Payment Terms, Price list found against the selected orders, use "Generate Invoice" process to create multiple invoices.
                    throw new ArgumentException(Msg.GetMsg(GetCtx(), "VIS_SoDifferentPayAndPrice"));
                }
                else
                {
                    //Different Payment Terms, Price list found against the selected orders
                    throw new ArgumentException(Msg.GetMsg(GetCtx(), "VIS_DifferentPayAndPrice"));
                }
            }

            // Create Invoice Header
            MInvoice invoice = new MInvoice(ship, null);

            //Payment Management
            int _CountVA009 = Env.IsModuleInstalled("VA009_") ? 1 : 0;

            // Amortization
            int _CountVA038 = Env.IsModuleInstalled("VA038_") ? 1 : 0;

            if (_CountVA009 > 0)
            {
                int _PaymentMethod_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select VA009_PaymentMethod_ID From C_Order Where C_Order_ID=" + ship.GetC_Order_ID(), null, Get_Trx()));

                // during consolidation, payment method need to set that is defined on selected business partner. 
                // If not defined on BP then it will set from order
                int bpPamentMethod_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT " + (ship.IsSOTrx() ? " VA009_PaymentMethod_ID " : " VA009_PO_PaymentMethod_ID ") +
                    @" FROM C_BPartner WHERE C_BPartner_ID = " + ship.GetC_BPartner_ID(), null, Get_Trx()));

                if (bpPamentMethod_ID != 0)
                {
                    _PaymentMethod_ID = bpPamentMethod_ID;
                }
                if (_PaymentMethod_ID > 0)
                {
                    invoice.SetVA009_PaymentMethod_ID(_PaymentMethod_ID);
                }
            }

            // Letter Of Credit
            int _CountVA026 = Env.IsModuleInstalled("VA026_") ? 1 : 0;
            if (_CountVA026 > 0)
            {
                int VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select VA026_LCDetail_ID From C_Order Where C_Order_ID=" + ship.GetC_Order_ID(), null, Get_Trx()));
                if (VA026_LCDetail_ID > 0)
                {
                    invoice.SetVA026_LCDetail_ID(VA026_LCDetail_ID);
                }
            }
            //end

            if (ship.IsReturnTrx())
            {
                if (!ship.IsSOTrx())
                {
                    // Purchase Return
                    // set target document from documnet type window -- based on documnet type available on material receipt / return to vendor

                    // JID_0779: Create AP Credit memo if we run the Generate TO process from Returm to Vendor window.

                    //if (invoice.GetC_DocTypeTarget_ID() == 0)
                    //{ 
                    if (_C_DocType_ID > 0)
                    {
                        invoice.SetC_DocTypeTarget_ID(_C_DocType_ID);
                    }
                    else
                    {
                        int C_DocTypeTarget_ID = DB.GetSQLValue(null, "SELECT C_DocTypeInvoice_ID FROM C_DocType WHERE C_DocType_ID=@param1", ship.GetC_DocType_ID());
                        if (C_DocTypeTarget_ID > 0)
                        {
                            invoice.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID);
                        }
                        else
                        {
                            invoice.SetC_DocTypeTarget_ID(ship.IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARCREDITMEMO : MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                        }
                    }
                    invoice.SetIsReturnTrx(ship.IsReturnTrx());
                    invoice.SetIsSOTrx(ship.IsSOTrx());
                }
                else
                {
                    // Sales Return
                    if (_C_DocType_ID > 0)
                    {
                        invoice.SetC_DocTypeTarget_ID(_C_DocType_ID);
                    }
                    else
                    {
                        if (ship.GetC_Order_ID() >= 0)
                        {
                            int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select C_DocType_ID From C_Order Where C_Order_ID=" + ship.GetC_Order_ID(), null, Get_Trx()));
                            MDocType dt = MDocType.Get(GetCtx(), C_DocType_ID);
                            if (dt.GetC_DocTypeInvoice_ID() != 0)
                                invoice.SetC_DocTypeTarget_ID(dt.GetC_DocTypeInvoice_ID(), true);
                            else
                                invoice.SetC_DocTypeTarget_ID(ship.IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARCREDITMEMO : MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                        }
                        else
                        {
                            invoice.SetC_DocTypeTarget_ID(ship.IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARCREDITMEMO : MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                        }
                    }
                }
            }
            if (_M_PriceList_ID != 0)
            {
                invoice.SetM_PriceList_ID(_M_PriceList_ID);
            }
            //Set InvoiceDocumentNo to InvoiceReference
            if (_InvoiceDocumentNo != null && _InvoiceDocumentNo.Length > 0)
            {
                invoice.Set_Value("InvoiceReference", _InvoiceDocumentNo);
            }
            //Set TargetDoctype  
            if (_C_DocType_ID > 0 && !ship.IsReturnTrx())
            {
                invoice.Set_Value("C_DocTypeTarget_ID", _C_DocType_ID);
            }

            // Added by Bharat on 30 Jan 2018 to set Inco Term from Order
            if (invoice.Get_ColumnIndex("C_IncoTerm_ID") > 0)
            {
                invoice.SetC_IncoTerm_ID(ship.GetC_IncoTerm_ID());
            }
            //To get Payment Rule and set the Payment method
            if (invoice.GetPaymentRule() != "")
            {
                invoice.SetPaymentMethod(invoice.GetPaymentRule());
            }
            if (invoice.GetC_ConversionType_ID() == 0)
            {
                //1052-- setcurrency type in case order reference is not present
                int currencyType = 0;
                currencyType= Util.GetValueOfInt(GetCtx().GetContext("#C_ConversionType_ID"));
                if (currencyType > 0)
                {
                    invoice.SetC_ConversionType_ID(currencyType);
                }
                else
                {
                    currencyType = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_ConversionType_ID FROM C_ConversionType WHERE IsActive='Y'AND AD_Org_ID IN(" + ship.GetAD_Org_ID() + ",0) AND ISDefault = 'Y' AND AD_Client_ID= "+GetAD_Client_ID()+" ORDER BY C_ConversionType_ID Desc"));
                    if (currencyType > 0)
                    {
                        invoice.SetC_ConversionType_ID(currencyType);
                    }
                    else
                    {
                        throw new ArgumentException(Msg.GetMsg(GetCtx(),"DefaultCurrencyTypeNotFound"));
                    }
                }
            }
            invoice.SetConditionalFlag(MInvoice.CONDITIONALFLAG_PrepareIt);

            if (!invoice.Save())
            {
                //SI_0708 - message was not upto the mark
                ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                if (pp != null)
                    throw new ArgumentException("Cannot save Invoice. " + pp.GetName());
                throw new ArgumentException("Cannot save Invoice");
            }
            MInOutLine[] shipLines = ship.GetLines(false);
            DateTime? AmortStartDate = null;
            DateTime? AmortEndDate = null;
            count = 0;
            DataSet ds = null;
            DataSet dsDoc = null;

            for (int i = 0; i < shipLines.Length; i++)
            {
                MInOutLine sLine = shipLines[i];
                // Changes done by Bharat on 06 July 2017 restrict to create invoice if Invoice already created against that for same quantity
                string sql = @"SELECT ml.QtyEntered - SUM(COALESCE(li.QtyEntered,0)) as QtyEntered, ml.MovementQty- SUM(COALESCE(li.QtyInvoiced, 0)) as QtyInvoiced" +
                    " FROM M_InOutLine ml INNER JOIN C_InvoiceLine li ON li.M_InOutLine_ID = ml.M_InOutLine_ID INNER JOIN C_Invoice ci ON ci.C_Invoice_ID = li.C_Invoice_ID " +
                    " WHERE ci.DocStatus NOT IN ('VO', 'RE') AND ml.M_InOutLine_ID =" + sLine.GetM_InOutLine_ID() + " GROUP BY ml.MovementQty, ml.QtyEntered";
                ds = DB.ExecuteDataset(sql, null, Get_Trx());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    decimal qtyEntered = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyEntered"]);
                    decimal qtyInvoiced = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyInvoiced"]);
                    if (qtyEntered <= 0)
                    {
                        // Getting document number Count if Invoice already generated for Material Receipt
                        string StrSql = "SELECT ci.DocumentNo,li.M_InOutLine_ID FROM C_InvoiceLine li INNER JOIN C_Invoice ci ON ci.C_Invoice_ID = li.C_Invoice_ID " +
                            "  WHERE ci.DocStatus NOT IN ('VO', 'RE') AND li.M_InOutLine_ID = " + sLine.GetM_InOutLine_ID();
                        dsDoc = DB.ExecuteDataset(StrSql, null, Get_Trx());
                        if (dsDoc != null && dsDoc.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsDoc.Tables[0].Rows.Count; j++)
                            {
                                // JID_1358: Need to show document number in message if Invoice already generated for Material Receipt
                                string no = invDocumentNo.ToString();
                                if (invDocumentNo.Length > 0 && no != Util.GetValueOfString(dsDoc.Tables[0].Rows[j]["DocumentNo"]))
                                {
                                    invDocumentNo.Append(", " + Util.GetValueOfString(dsDoc.Tables[0].Rows[j]["DocumentNo"]));
                                }
                                else
                                {
                                    invDocumentNo.Clear();
                                    invDocumentNo.Append(Util.GetValueOfString(dsDoc.Tables[0].Rows[j]["DocumentNo"]));
                                }
                                ds.Dispose();
                                log.Info("Invoice Line already exist for Receipt Line ID - " + sLine.GetM_InOutLine_ID());
                                continue;
                            }
                            dsDoc.Dispose();
                        }
                    }
                    else
                    {
                        MInvoiceLine line = new MInvoiceLine(invoice);
                        line.SetShipLine(sLine);
                        line.SetQtyEntered(qtyEntered);
                        line.SetQtyInvoiced(qtyInvoiced);
                        // Change By Mohit Amortization process -------------
                        if (_CountVA038 > 0)
                        {
                            if (line.GetM_Product_ID() > 0)
                            {
                                //MProduct pro = new MProduct(GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
                                int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM M_Product WHERE M_Product_ID = " + sLine.GetM_Product_ID(), null, Get_Trx()));
                                if (VA038_AmortizationTemplate_ID > 0)
                                {
                                    line.Set_Value("VA038_AmortizationTemplate_ID", VA038_AmortizationTemplate_ID);
                                    DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + VA038_AmortizationTemplate_ID);
                                    AmortStartDate = null;
                                    AmortEndDate = null;
                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                                    {
                                        AmortStartDate = invoice.GetDateAcct();
                                    }
                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                                    {
                                        AmortStartDate = invoice.GetDateInvoiced();
                                    }

                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                                    {
                                        AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                    }
                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                                    {
                                        AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                    }
                                    line.Set_Value("FROMDATE", AmortStartDate);
                                    line.Set_Value("EndDate", AmortEndDate);
                                    if (amrtDS != null)
                                    {
                                        amrtDS.Dispose();
                                    }
                                }
                            }
                            if (line.GetC_Charge_ID() > 0)
                            {
                                //MCharge charge = new MCharge(GetCtx(), sLine.GetC_Charge_ID(), Get_TrxName());
                                int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM C_Charge WHERE C_Charge_ID = " + sLine.GetC_Charge_ID(), null, Get_Trx()));
                                if (VA038_AmortizationTemplate_ID > 0)
                                {
                                    line.Set_Value("VA038_AmortizationTemplate_ID", VA038_AmortizationTemplate_ID);
                                    DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + VA038_AmortizationTemplate_ID);
                                    AmortStartDate = null;
                                    AmortEndDate = null;
                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                                    {
                                        AmortStartDate = invoice.GetDateAcct();
                                    }
                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                                    {
                                        AmortStartDate = invoice.GetDateInvoiced();
                                    }

                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                                    {
                                        AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                    }
                                    if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                                    {
                                        AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                    }
                                    line.Set_Value("FROMDATE", AmortStartDate);
                                    line.Set_Value("EndDate", AmortEndDate);
                                    if (amrtDS != null)
                                    {
                                        amrtDS.Dispose();
                                    }
                                }
                            }
                        }
                        // End Change Amortization process--------------
                        if (!line.Save())
                        {
                            //SI_0708 - message was not upto the mark
                            ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                            if (pp != null)
                                throw new ArgumentException("Cannot save Invoice Line. " + pp.GetName());
                            throw new ArgumentException("Cannot save Invoice Line");
                        }
                        count++;
                    }
                    ds.Dispose();
                }
                else
                {
                    MInvoiceLine line = new MInvoiceLine(invoice);
                    // JID_1850 Avoid the duplicate charge line 
                    if (sLine.GetC_Charge_ID() > 0 && (!isAllownonItem || _GenerateCharges))
                    {
                        continue;
                    }
                    line.SetShipLine(sLine);
                    line.SetQtyEntered(sLine.GetQtyEntered());
                    line.SetQtyInvoiced(sLine.GetMovementQty());
                    line.Set_ValueNoCheck("IsDropShip", sLine.Get_Value("IsDropShip")); //Arpit Rai 20-Sept-2017              
                    //190 - Set Print Description
                    if (line.Get_ColumnIndex("PrintDescription") >= 0)
                        line.Set_ValueNoCheck("PrintDescription", sLine.Get_Value("PrintDescription"));

                    // Change By Mohit Amortization process -------------
                    if (_CountVA038 > 0)
                    {
                        if (line.GetM_Product_ID() > 0)
                        {
                            //MProduct pro = new MProduct(GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
                            int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM M_Product WHERE M_Product_ID = " + sLine.GetM_Product_ID(), null, Get_Trx()));
                            if (VA038_AmortizationTemplate_ID > 0)
                            {
                                line.Set_Value("VA038_AmortizationTemplate_ID", VA038_AmortizationTemplate_ID);
                                DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + VA038_AmortizationTemplate_ID);
                                AmortStartDate = null;
                                AmortEndDate = null;
                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                                {
                                    AmortStartDate = invoice.GetDateAcct();
                                }
                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                                {
                                    AmortStartDate = invoice.GetDateInvoiced();
                                }

                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                                {
                                    AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                }
                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                                {
                                    AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                }
                                line.Set_Value("FROMDATE", AmortStartDate);
                                line.Set_Value("EndDate", AmortEndDate);
                                if (amrtDS != null)
                                {
                                    amrtDS.Dispose();
                                }
                            }
                        }
                        if (line.GetC_Charge_ID() > 0)
                        {
                            //MCharge charge = new MCharge(GetCtx(), sLine.GetC_Charge_ID(), Get_TrxName());
                            int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM C_Charge WHERE C_Charge_ID = " + sLine.GetC_Charge_ID(), null, Get_Trx()));
                            if (VA038_AmortizationTemplate_ID > 0)
                            {
                                line.Set_Value("VA038_AmortizationTemplate_ID", VA038_AmortizationTemplate_ID);
                                DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + VA038_AmortizationTemplate_ID);
                                AmortStartDate = null;
                                AmortEndDate = null;
                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                                {
                                    AmortStartDate = invoice.GetDateAcct();
                                }
                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                                {
                                    AmortStartDate = invoice.GetDateInvoiced();
                                }

                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                                {
                                    AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                }
                                if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                                {
                                    AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                }
                                line.Set_Value("FROMDATE", AmortStartDate);
                                line.Set_Value("EndDate", AmortEndDate);
                                if (amrtDS != null)
                                {
                                    amrtDS.Dispose();
                                }
                            }
                        }
                    }
                    // End Change Amortization process--------------
                    if (!line.Save())
                    {
                        //SI_0708 - message was not upto the mark
                        ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                        if (pp != null)
                            throw new ArgumentException("Cannot save Invoice Line. " + pp.GetName());
                        throw new ArgumentException("Cannot save Invoice Line");
                    }
                    count++;
                }
            }


            #region[Enhancement for Charges Distribution/Generation by Sukhwinder on 13 December 2017]

            if (_GenerateCharges && count > 0)
            {
                StringBuilder OrderSql = new StringBuilder();
                OrderSql.Append("   SELECT CO.C_ORDER_ID,                    "
                               + "      SUM(ML.QTYENTERED) AS MRLINEQTY,     "
                               + "      SUM(OL.QTYENTERED) AS ORDERQTY       "
                               + "  FROM M_INOUTLINE ML                      "
                               + "  INNER JOIN C_ORDERLINE OL                "
                               + "  ON OL.C_ORDERLINE_ID = ML.C_ORDERLINE_ID "
                               + " INNER JOIN C_ORDER CO                     "
                               + " ON CO.C_ORDER_ID     = OL.C_ORDER_ID      "
                               + " WHERE ML.M_INOUT_ID  = " + _M_InOut_ID
                               + " AND (OL.C_CHARGE_ID IS NULL               "
                               + " OR OL.C_CHARGE_ID    = 0)                 "
                               + " GROUP BY CO.C_ORDER_ID                    ");

                DataSet OrderDS = DB.ExecuteDataset(OrderSql.ToString(), null, Get_Trx());

                if (OrderDS != null && OrderDS.Tables[0].Rows.Count > 0)
                {
                    StringBuilder ChargesSql = new StringBuilder();
                    for (int index = 0; index < OrderDS.Tables[0].Rows.Count; index++)
                    {
                        ds = null;
                        ChargesSql.Clear();                        
                        ChargesSql.Append(" SELECT C_CHARGE_ID,                                             "
                                 + "   C_ORDERLINE_ID,                                                      "
                                 + "   C_ORDER_ID,                                                          "
                                 + "   C_CURRENCY_ID,                                                       "
                                 + "   PRICEENTERED,                                                        "
                                 + "   PRICEACTUAL,                                                         "
                                 + "   LINENETAMT,                                                          "
                                 + "   QTYENTERED,                                                          "
                                 + "   C_UOM_ID,                                                            "
                                 + "   C_Tax_ID,                                                            "
                                 + "   IsDropShip,                                                          "
                                 + "   PrintDescription                                                     "
                                 + " FROM C_ORDERLINE                                                       "
                                 + " WHERE C_ORDER_ID IN                                                    "
                                 + "   ( " + Util.GetValueOfInt(OrderDS.Tables[0].Rows[index]["C_ORDER_ID"])
                                 + "   )                                                                    "
                                 + " AND C_CHARGE_ID IS NOT NULL                                            "
                                 + " AND C_CHARGE_ID  > 0                                                   ");
                       

                        ds = DB.ExecuteDataset(ChargesSql.ToString(), null, Get_Trx());

                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                MInvoiceLine line = new MInvoiceLine(invoice);
                                line.SetQty(1);
                                line.SetQtyEntered(1);
                                line.SetQtyInvoiced(1);
                                line.SetOrderLine(new MOrderLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ORDERLINE_ID"]), Get_Trx()));
                                line.SetC_Charge_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_CHARGE_ID"]));
                                line.SetC_UOM_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_UOM_ID"]));
                                line.SetC_Tax_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_TAX_ID"]));

                                decimal SumOfQty = 0;
                                if (Util.GetValueOfInt(OrderDS.Tables[0].Rows[index]["ORDERQTY"]) == 0)
                                {
                                    SumOfQty = 1;
                                }
                                else
                                {
                                    SumOfQty = Util.GetValueOfInt(OrderDS.Tables[0].Rows[index]["ORDERQTY"]);
                                }
                                decimal amt = (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["LINENETAMT"]) / SumOfQty) * Util.GetValueOfInt(OrderDS.Tables[0].Rows[index]["MRLINEQTY"]);

                                line.SetPrice(Decimal.Round(amt, 3));
                                line.SetPriceActual(Decimal.Round(amt, 3));
                                line.SetPriceEntered(Decimal.Round(amt, 3));
                                line.SetLineNetAmt(Decimal.Round(amt, 3));

                                line.Set_ValueNoCheck("IsDropShip", Util.GetValueOfString(ds.Tables[0].Rows[i]["ISDROPSHIP"]));

                                if (_CountVA038 > 0)
                                {
                                    if (line.GetC_Charge_ID() > 0)
                                    {
                                        MCharge charge = new MCharge(GetCtx(), line.GetC_Charge_ID(), Get_TrxName());
                                        if (Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")) > 0)
                                        {
                                            line.Set_Value("VA038_AmortizationTemplate_ID", Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                                            DataSet amrtDS = DB.ExecuteDataset("SELECT VA038_AmortizationType,VA038_AmortizationPeriod,VA038_TermSource,VA038_PeriodType,Name FROM VA038_AmortizationTemplate WHERE IsActive='Y' AND VA038_AMORTIZATIONTEMPLATE_ID=" + Util.GetValueOfInt(charge.Get_Value("VA038_AmortizationTemplate_ID")));
                                            AmortStartDate = null;
                                            AmortEndDate = null;
                                            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "A")
                                            {
                                                AmortStartDate = invoice.GetDateAcct();
                                            }
                                            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_TermSource"]) == "T")
                                            {
                                                AmortStartDate = invoice.GetDateInvoiced();
                                            }

                                            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "M")
                                            {
                                                AmortEndDate = AmortStartDate.Value.AddMonths(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                            }
                                            if (Util.GetValueOfString(amrtDS.Tables[0].Rows[0]["VA038_PeriodType"]) == "Y")
                                            {
                                                AmortEndDate = AmortStartDate.Value.AddYears(Util.GetValueOfInt(amrtDS.Tables[0].Rows[0]["VA038_AmortizationPeriod"]));
                                            }
                                            line.Set_Value("FROMDATE", AmortStartDate);
                                            line.Set_Value("EndDate", AmortEndDate);
                                            if (amrtDS != null)
                                            {
                                                amrtDS.Dispose();
                                            }
                                        }
                                    }
                                }

                                //190 - Get Print description and set
                                if (line.Get_ColumnIndex("PrintDescription") >= 0)
                                    line.Set_Value("PrintDescription", Util.GetValueOfString(ds.Tables[0].Rows[i]["PrintDescription"]));

                                if (!line.Save())
                                {
                                    //SI_0708 - message was not upto the mark
                                    ValueNamePair pp = VAdvantage.Logging.VLogger.RetrieveError();
                                    if (pp != null)
                                        throw new ArgumentException("Cannot save Invoice Line. " + pp.GetName());
                                    throw new ArgumentException("Cannot save Invoice Line");
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            if (count > 0)
            {
                if (!invoice.CalculateTaxTotal())   //	setTotals
                {
                    throw new ArgumentException(Msg.GetMsg(GetCtx(), "ErrorCalculateTax") + ": " + invDocumentNo.ToString());
                }
                UpdateInvoiceHeader(invoice);
                DB.ExecuteQuery("UPDATE C_Invoice SET ConditionalFlag = null WHERE C_Invoice_ID = " + invoice.GetC_Invoice_ID(), null, Get_Trx());
                return invoice.GetDocumentNo();
            }
            else
            {
                //Get_Trx().Rollback();
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "InvoiceExist") + ": " + invDocumentNo.ToString());
            }
        }

        /// <summary>
        /// Update Sub Total and Grand Total on header
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <returns>true when success</returns>
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
                        + "(SELECT COALESCE(SUM(TaxAmt),0) FROM C_InvoiceTax it WHERE i.C_Invoice_ID=it.C_Invoice_ID) "
                        + (invoice.Get_ColumnIndex("WithholdingAmt") > 0 ? " , GrandTotalAfterWithholding = (TotalLines + (SELECT COALESCE(SUM(TaxAmt),0) FROM C_InvoiceTax it WHERE i.C_Invoice_ID=it.C_Invoice_ID) - NVL(WithholdingAmt, 0) - NVL(BackupWithholdingAmount, 0))" : "")
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

