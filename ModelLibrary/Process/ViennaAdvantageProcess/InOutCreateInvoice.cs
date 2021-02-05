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
        private int _VAM_Inv_InOut_ID = 0;
        //Price List Version		
        private int _VAM_PriceList_ID = 0;
        //Document No					
        private String _InvoiceDocumentNo = null;
        //Document Type
        private int _VAB_DocTypes_ID = 0;
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
                else if (name.Equals("VAM_PriceList_ID"))
                {
                    _VAM_PriceList_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("InvoiceDocumentNo"))
                {
                    _InvoiceDocumentNo = (String)para[i].GetParameter();
                }
                else if (name.Equals("GenerateCharges"))
                {
                    _GenerateCharges = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("VAB_DocTypes_ID"))
                {
                    _VAB_DocTypes_ID = para[i].GetParameterAsInt(); ;
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _VAM_Inv_InOut_ID = GetRecord_ID();
        }

        /// <summary>
        /// Create Invoice.
        /// </summary>
        /// <returns>document no</returns>
        protected override String DoIt()
        {
            StringBuilder invDocumentNo = new StringBuilder();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(" SELECT  Count(*)  FROM VAM_Inv_InOut WHERE  ISSOTRX='Y' AND  VAM_Inv_InOut_ID=" + GetRecord_ID()));
            MInOut ship = null;
            bool isAllownonItem = Util.GetValueOfString(GetCtx().GetContext("$AllowNonItem")).Equals("Y");
            if (count > 0)
            {
                if (_VAM_Inv_InOut_ID == 0)
                {
                    throw new ArgumentException("No Shipment");
                }
                //
                ship = new MInOut(GetCtx(), _VAM_Inv_InOut_ID, Get_Trx());
                if (ship.Get_ID() == 0)
                {
                    throw new ArgumentException("Shipment not found");
                }
                if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()))
                {
                    // JID_0750: done by Bharat on 05 Feb 2019 if Customer Return document and status is not complete it should give message "Customer Return Not Completed".
                    if (ship.IsReturnTrx())
                    {
                        throw new ArgumentException("Customer Return Not Completed");
                    }
                    else
                    {
                        throw new ArgumentException("Shipment Not Completed");
                    }
                }
            }
            else
            {
                if (_VAM_Inv_InOut_ID == 0)
                {
                    throw new ArgumentException("No Material Receipt");
                }
                //
                ship = new MInOut(GetCtx(), _VAM_Inv_InOut_ID, Get_Trx());
                if (ship.Get_ID() == 0)
                {
                    throw new ArgumentException("Material Receipt not found");
                }
                if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()))
                {
                    // JID_0750: done by Bharat on 05 Feb 2019 if Return to vendor document and status is not complete it should give message "Return To Vendor Not Completed".
                    if (ship.IsReturnTrx())
                    {
                        throw new ArgumentException("Return To Vendor Not Completed");
                    }
                    else
                    {
                        throw new ArgumentException("Material Receipt Not Completed");
                    }
                }
            }

            // When record contain more than single order and order having different Payment term or Price List then not to generate invoices
            // JID_0976 - For conversion Type
            if (ship.GetVAB_Order_ID() > 0 && Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT  COUNT(DISTINCT  VAB_Order.VAM_PriceList_id) +  count(distinct VAB_Order.VAB_Paymentterm_id) + count(distinct COALESCE( VAB_Order.VAB_CurrencyType_ID , " + MVABCurrencyType.GetDefault(GetVAF_Client_ID()) + @"))  as recordcount
                            FROM VAM_Inv_InOutLine INNER JOIN VAB_Orderline ON VAM_Inv_InOutLine.VAB_Orderline_id = VAB_Orderline.VAB_Orderline_id
                            INNER JOIN VAB_Order ON VAB_Order.VAB_Order_id = VAB_Orderline.VAB_Order_id
                            WHERE VAM_Inv_InOutLine.VAM_Inv_InOut_id = " + _VAM_Inv_InOut_ID + @"  GROUP BY   VAM_Inv_InOutLine.VAM_Inv_InOut_id ", null, Get_Trx())) > 3)
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
                int _PaymentMethod_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select VA009_PaymentMethod_ID From VAB_Order Where VAB_Order_ID=" + ship.GetVAB_Order_ID(), null, Get_Trx()));

                // during consolidation, payment method need to set that is defined on selected business partner. 
                // If not defined on BP then it will set from order
                int bpPamentMethod_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT " + (ship.IsSOTrx() ? " VA009_PaymentMethod_ID " : " VA009_PO_PaymentMethod_ID ") +
                    @" FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID = " + ship.GetVAB_BusinessPartner_ID(), null, Get_Trx()));

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
                int VA026_LCDetail_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select VA026_LCDetail_ID From VAB_Order Where VAB_Order_ID=" + ship.GetVAB_Order_ID(), null, Get_Trx()));
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

                    //if (invoice.GetVAB_DocTypesTarget_ID() == 0)
                    //{ 
                    if (_VAB_DocTypes_ID > 0)
                    {
                        invoice.SetVAB_DocTypesTarget_ID(_VAB_DocTypes_ID);
                    }
                    else
                    {
                        int VAB_DocTypesTarget_ID = DB.GetSQLValue(null, "SELECT VAB_DocTypesInvoice_ID FROM VAB_DocTypes WHERE VAB_DocTypes_ID=@param1", ship.GetVAB_DocTypes_ID());
                        if (VAB_DocTypesTarget_ID > 0)
                        {
                            invoice.SetVAB_DocTypesTarget_ID(VAB_DocTypesTarget_ID);
                        }
                        else
                        {
                            invoice.SetVAB_DocTypesTarget_ID(ship.IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARCREDITMEMO : MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                        }
                    }
                    invoice.SetIsReturnTrx(ship.IsReturnTrx());
                    invoice.SetIsSOTrx(ship.IsSOTrx());
                }
                else
                {
                    // Sales Return
                    if (_VAB_DocTypes_ID > 0)
                    {
                        invoice.SetVAB_DocTypesTarget_ID(_VAB_DocTypes_ID);
                    }
                    else
                    {
                        if (ship.GetVAB_Order_ID() >= 0)
                        {
                            int VAB_DocTypes_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select VAB_DocTypes_ID From VAB_Order Where VAB_Order_ID=" + ship.GetVAB_Order_ID(), null, Get_Trx()));
                            MDocType dt = MDocType.Get(GetCtx(), VAB_DocTypes_ID);
                            if (dt.GetVAB_DocTypesInvoice_ID() != 0)
                                invoice.SetVAB_DocTypesTarget_ID(dt.GetVAB_DocTypesInvoice_ID(), true);
                            else
                                invoice.SetVAB_DocTypesTarget_ID(ship.IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARCREDITMEMO : MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                        }
                        else
                        {
                            invoice.SetVAB_DocTypesTarget_ID(ship.IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARCREDITMEMO : MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                        }
                    }
                }
            }
            if (_VAM_PriceList_ID != 0)
            {
                invoice.SetVAM_PriceList_ID(_VAM_PriceList_ID);
            }
            //Set InvoiceDocumentNo to InvoiceReference
            if (_InvoiceDocumentNo != null && _InvoiceDocumentNo.Length > 0)
            {
                invoice.Set_Value("InvoiceReference", _InvoiceDocumentNo);
            }
            //Set TargetDoctype  
            if (_VAB_DocTypes_ID > 0 && !ship.IsReturnTrx())
            {
                invoice.Set_Value("VAB_DocTypesTarget_ID", _VAB_DocTypes_ID);
            }

            // Added by Bharat on 30 Jan 2018 to set Inco Term from Order
            if (invoice.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
            {
                invoice.SetVAB_IncoTerm_ID(ship.GetVAB_IncoTerm_ID());
            }
            //To get Payment Rule and set the Payment method
            if (invoice.GetPaymentRule() != "")
            {
                invoice.SetPaymentMethod(invoice.GetPaymentRule());
            }
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
                    " FROM VAM_Inv_InOutLine ml INNER JOIN VAB_InvoiceLine li ON li.VAM_Inv_InOutLine_ID = ml.VAM_Inv_InOutLine_ID INNER JOIN VAB_Invoice ci ON ci.VAB_Invoice_ID = li.VAB_Invoice_ID " +
                    " WHERE ci.DocStatus NOT IN ('VO', 'RE') AND ml.VAM_Inv_InOutLine_ID =" + sLine.GetVAM_Inv_InOutLine_ID() + " GROUP BY ml.MovementQty, ml.QtyEntered";
                ds = DB.ExecuteDataset(sql, null, Get_Trx());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    decimal qtyEntered = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyEntered"]);
                    decimal qtyInvoiced = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyInvoiced"]);
                    if (qtyEntered <= 0)
                    {
                        // Getting document number Count if Invoice already generated for Material Receipt
                        string StrSql = "SELECT ci.DocumentNo,li.VAM_Inv_InOutLine_ID FROM VAB_InvoiceLine li INNER JOIN VAB_Invoice ci ON ci.VAB_Invoice_ID = li.VAB_Invoice_ID " +
                            "  WHERE ci.DocStatus NOT IN ('VO', 'RE') AND li.VAM_Inv_InOutLine_ID = " + sLine.GetVAM_Inv_InOutLine_ID();
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
                                log.Info("Invoice Line already exist for Receipt Line ID - " + sLine.GetVAM_Inv_InOutLine_ID());
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
                            if (line.GetVAM_Product_ID() > 0)
                            {
                                //MProduct pro = new MProduct(GetCtx(), sLine.GetVAM_Product_ID(), Get_TrxName());
                                int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM VAM_Product WHERE VAM_Product_ID = " + sLine.GetVAM_Product_ID(), null, Get_Trx()));
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
                            if (line.GetVAB_Charge_ID() > 0)
                            {
                                //MVABCharge charge = new MVABCharge(GetCtx(), sLine.GetVAB_Charge_ID(), Get_TrxName());
                                int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM VAB_Charge WHERE VAB_Charge_ID = " + sLine.GetVAB_Charge_ID(), null, Get_Trx()));
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
                    if (sLine.GetVAB_Charge_ID() > 0 && (!isAllownonItem || _GenerateCharges))
                    {
                        continue;
                    }
                    line.SetShipLine(sLine);
                    line.SetQtyEntered(sLine.GetQtyEntered());
                    line.SetQtyInvoiced(sLine.GetMovementQty());
                    line.Set_ValueNoCheck("IsDropShip", sLine.Get_Value("IsDropShip")); //Arpit Rai 20-Sept-2017              

                    // Change By Mohit Amortization process -------------
                    if (_CountVA038 > 0)
                    {
                        if (line.GetVAM_Product_ID() > 0)
                        {
                            //MProduct pro = new MProduct(GetCtx(), sLine.GetVAM_Product_ID(), Get_TrxName());
                            int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM VAM_Product WHERE VAM_Product_ID = " + sLine.GetVAM_Product_ID(), null, Get_Trx()));
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
                        if (line.GetVAB_Charge_ID() > 0)
                        {
                            //MVABCharge charge = new MVABCharge(GetCtx(), sLine.GetVAB_Charge_ID(), Get_TrxName());
                            int VA038_AmortizationTemplate_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA038_AmortizationTemplate_ID 
                                     FROM VAB_Charge WHERE VAB_Charge_ID = " + sLine.GetVAB_Charge_ID(), null, Get_Trx()));
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
                OrderSql.Append("   SELECT CO.VAB_ORDER_ID,                    "
                               + "      SUM(ML.QTYENTERED) AS MRLINEQTY,     "
                               + "      SUM(OL.QTYENTERED) AS ORDERQTY       "
                               + "  FROM VAM_Inv_InOutLine ML                      "
                               + "  INNER JOIN VAB_ORDERLINE OL                "
                               + "  ON OL.VAB_ORDERLINE_ID = ML.VAB_ORDERLINE_ID "
                               + " INNER JOIN VAB_ORDER CO                     "
                               + " ON CO.VAB_ORDER_ID     = OL.VAB_ORDER_ID      "
                               + " WHERE ML.VAM_Inv_InOut_ID  = " + _VAM_Inv_InOut_ID
                               + " AND (OL.VAB_CHARGE_ID IS NULL               "
                               + " OR OL.VAB_CHARGE_ID    = 0)                 "
                               + " GROUP BY CO.VAB_ORDER_ID                    ");

                DataSet OrderDS = DB.ExecuteDataset(OrderSql.ToString(), null, Get_Trx());

                if (OrderDS != null && OrderDS.Tables[0].Rows.Count > 0)
                {
                    StringBuilder ChargesSql = new StringBuilder();
                    for (int index = 0; index < OrderDS.Tables[0].Rows.Count; index++)
                    {
                        ds = null;
                        ChargesSql.Clear();
                        ChargesSql.Append(" SELECT VAB_CHARGE_ID,                                             "
                                 + "   VAB_ORDERLINE_ID,                                                      "
                                 + "   VAB_ORDER_ID,                                                          "
                                 + "   VAB_CURRENCY_ID,                                                       "
                                 + "   PRICEENTERED,                                                        "
                                 + "   PRICEACTUAL,                                                         "
                                 + "   LINENETAMT,                                                          "
                                 + "   QTYENTERED,                                                          "
                                 + "   VAB_UOM_ID,                                                            "
                                 + "   VAB_TaxRate_ID,                                                            "
                                 + "   IsDropShip                                                           "
                                 + " FROM VAB_ORDERLINE                                                       "
                                 + " WHERE VAB_ORDER_ID IN                                                    "
                                 + "   ( " + Util.GetValueOfInt(OrderDS.Tables[0].Rows[index]["VAB_ORDER_ID"])
                                 + "   )                                                                    "
                                 + " AND VAB_CHARGE_ID IS NOT NULL                                            "
                                 + " AND VAB_CHARGE_ID  > 0                                                   ");


                        ds = DB.ExecuteDataset(ChargesSql.ToString(), null, Get_Trx());

                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                MInvoiceLine line = new MInvoiceLine(invoice);
                                line.SetQty(1);
                                line.SetQtyEntered(1);
                                line.SetQtyInvoiced(1);
                                line.SetOrderLine(new MOrderLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_ORDERLINE_ID"]), Get_Trx()));
                                line.SetVAB_Charge_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_CHARGE_ID"]));
                                line.SetVAB_UOM_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_UOM_ID"]));
                                line.SetVAB_TaxRate_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAB_TAXRATE_ID"]));

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
                                    if (line.GetVAB_Charge_ID() > 0)
                                    {
                                        MVABCharge charge = new MVABCharge(GetCtx(), line.GetVAB_Charge_ID(), Get_TrxName());
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
                return invoice.GetDocumentNo();
            }
            else
            {
                //Get_Trx().Rollback();
                throw new ArgumentException(Msg.GetMsg(GetCtx(), "InvoiceExist") + ": " + invDocumentNo.ToString());
            }
        }
    }
}

