using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
//using ViennaAdvantage.Process;
////using System.Windows.Forms;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;


using VAdvantage.Process;

namespace VAdvantage.Process
{
    public class CreateARInvoice : SvrProcess
    {
        #region Private Variable
        private int _C_BPartner_ID = 0;
        private DateTime? _DateInvoiced = null;
       // private int _noInvoices = 0;
        string sql = "";
       // string msg = "";
        List<int> C_BPartner_ID = new List<int>();
        // List<int> C_BPartner_IDAP = new List<int>();
        List<int> M_Product_ID = new List<int>();
        Dictionary<int, int> BPInvoice = new Dictionary<int, int>();
        // Dictionary<int, int> BPAPInvoice = new Dictionary<int, int>();
        int AD_Org_ID = 0;
        int C_Order_ID = 0;
        string docAction = "";
        string ConsolidateDocument = "";
        private List<int> bPartners = new List<int>();
        private List<int> invoices = new List<int>();
        List<int> orders = new List<int>();
        #endregion

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
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateInvoiced"))
                {
                    _DateInvoiced = (DateTime?)para[i].GetParameter();
                    // _DateTo = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    // _DateFrom = (DateTime?)para[i].GetParameter();
                    AD_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_Order_ID"))
                {
                    // _DateFrom = (DateTime?)para[i].GetParameter();
                    C_Order_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DocAction"))
                {
                    // _DateFrom = (DateTime?)para[i].GetParameter();
                    docAction = para[i].GetParameter().ToString();
                }
                else if (name.Equals("ConsolidateDocument"))
                {
                    // _DateFrom = (DateTime?)para[i].GetParameter();
                    ConsolidateDocument = para[i].GetParameter().ToString();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Generate Shipments
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            bool isExpense = false;
            if (ConsolidateDocument == "Y")
            {

                string sqlSelect = "select * from s_timeexpenseline where processed = 'Y' and (ARInvoice = 'Y' or billtocustomer = 'Y')  and Ref_C_Invoice_ID is null";
                StringBuilder sqlWhere = new StringBuilder();
                if (C_Order_ID != 0)
                {
                    sqlWhere.Append(" AND C_Order_ID = " + C_Order_ID);
                }
                else
                {
                    if (_C_BPartner_ID != 0)
                    {
                        sqlWhere.Append(" AND C_BPartner_ID = " + _C_BPartner_ID);
                    }
                    if (AD_Org_ID != 0)
                    {
                        sqlWhere.Append(" AND AD_Org_ID = " + AD_Org_ID);
                    }
                }

                if (sqlWhere.Length > 0)
                {
                    sql = sqlSelect + sqlWhere.ToString();
                }
                else
                {
                    sql = sqlSelect;
                }

                sql = sql + " order by C_order_ID, c_bpartner_id";

                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        if (C_BPartner_ID.Contains(Util.GetValueOfInt(idr["C_BPartner_ID"])))
                        {
                            VAdvantage.Model.MOrder ord = new VAdvantage.Model.MOrder(GetCtx(), Util.GetValueOfInt(idr["C_Order_ID"]), null);
                            bool chk = false;
                            for (int i = 0; i < invoices.Count; i++)
                            {
                                VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), Util.GetValueOfInt(invoices[i]), null);

                                if ((inv.GetC_PaymentTerm_ID() == ord.GetC_PaymentTerm_ID()) && (inv.GetM_PriceList_ID() == ord.GetM_PriceList_ID()))
                                {
                                    chk = true;
                                    break;
                                }
                            }
                            if (!chk)
                            {
                                VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), Util.GetValueOfInt(idr["s_timeexpenseline_id"]), null);
                                VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);
                                int C_Invoice_ID = GenerateInvoice(tLine, tExp, isExpense);
                                invoices.Add(C_Invoice_ID);
                            }
                        }
                        else
                        {
                            C_BPartner_ID.Add(Util.GetValueOfInt(idr["C_BPartner_ID"]));
                            VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), Util.GetValueOfInt(idr["s_timeexpenseline_id"]), null);
                            VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);
                            int C_Invoice_ID = GenerateInvoice(tLine, tExp, isExpense);
                            invoices.Add(C_Invoice_ID);
                        }
                    }
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }

                    //  isExpense = false;

                    if (invoices.Count > 0)
                    {
                        for (int k = 0; k < invoices.Count; k++)
                        //  foreach (KeyValuePair<int, int> pair in BPInvoice)
                        {
                            sqlWhere = new StringBuilder();
                            if (C_Order_ID != 0)
                            {
                                sqlWhere.Append(" AND C_Order_ID = " + C_Order_ID);
                            }
                            else
                            {
                                if (_C_BPartner_ID != 0)
                                {
                                    sqlWhere.Append(" AND C_BPartner_ID = " + _C_BPartner_ID);
                                }
                                if (AD_Org_ID != 0)
                                {
                                    sqlWhere.Append(" AND AD_Org_ID = " + AD_Org_ID);
                                }
                            }

                            if (sqlWhere.Length > 0)
                            {
                                sql = sqlSelect + sqlWhere.ToString();
                            }
                            else
                            {
                                sql = sqlSelect;
                            }

                            sql = sql + " order by C_order_ID, c_bpartner_id";

                            DataSet ds = DB.ExecuteDataset(sql, null, null);
                            if (ds != null)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                                    {
                                        int invID = 0;
                                        bool chk1 = false;
                                        for (int l = 0; l < invoices.Count; l++)
                                        {
                                            VAdvantage.Model.MOrder ord = new VAdvantage.Model.MOrder(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[j]["C_Order_ID"]), null);
                                            VAdvantage.Model.MInvoice inv1 = new VAdvantage.Model.MInvoice(GetCtx(), Util.GetValueOfInt(invoices[l]), null);
                                            if ((inv1.GetC_PaymentTerm_ID() == ord.GetC_PaymentTerm_ID()) && (inv1.GetC_BPartner_ID() == ord.GetC_BPartner_ID())
                                                && (inv1.GetM_PriceList_ID() == ord.GetM_PriceList_ID()))
                                            {
                                                chk1 = true;
                                                invID = inv1.GetC_Invoice_ID();
                                                break;
                                            }
                                        }
                                        if (chk1)
                                        {
                                            //CreateLine(Util.GetValueOfInt(ds.Tables[0].Rows[j]["S_TimeExpenseLine_ID"]), Util.GetValueOfInt(pair.Value));
                                            CreateLine(Util.GetValueOfInt(ds.Tables[0].Rows[j]["S_TimeExpenseLine_ID"]), invID);
                                        }
                                    }

                                    for (int m = 0; m < invoices.Count; m++)
                                    {
                                        //MInvoice inv = new MInvoice(GetCtx(), Util.GetValueOfInt(pair.Value), null);
                                        VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), Util.GetValueOfInt(invoices[m]), null);
                                        if (docAction == "CO")
                                        {
                                            string comp = inv.CompleteIt();
                                            if (comp == "CO")
                                            {
                                                inv.SetDocAction("CL");
                                                inv.SetDocStatus("CO");
                                                inv.Save();
                                            }
                                        }
                                        else if (docAction == "PR")
                                        {
                                            string prp = inv.PrepareIt();
                                            if (prp == "IP")
                                            {
                                                inv.SetDocAction("PR");
                                                inv.SetDocStatus("IP");
                                                inv.Save();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                catch
                {
                    return Msg.GetMsg(GetCtx(), "ProcessTerminated");
                }
            }

            // If Consolidate Document is Not Checked
            else
            {
                string sqlSelect = "select * from s_timeexpenseline where processed = 'Y' and (ARInvoice = 'Y' or billtocustomer = 'Y')  and Ref_C_Invoice_ID is null";
                StringBuilder sqlWhere = new StringBuilder();
                if (C_Order_ID != 0)
                {
                    sqlWhere.Append(" AND C_Order_ID = " + C_Order_ID);
                }
                else
                {
                    if (_C_BPartner_ID != 0)
                    {
                        sqlWhere.Append(" AND C_BPartner_ID = " + _C_BPartner_ID);
                    }
                    if (AD_Org_ID != 0)
                    {
                        sqlWhere.Append(" AND AD_Org_ID = " + AD_Org_ID);
                    }
                }

                if (sqlWhere.Length > 0)
                {
                    sql = sqlSelect + sqlWhere.ToString();
                }
                else
                {
                    sql = sqlSelect;
                }

                sql = sql + " order by C_order_ID, c_bpartner_id";
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, null);
                    int C_Invoice_ID = 0;
                    while (idr.Read())
                    {
                        if (orders.Contains(Util.GetValueOfInt(idr["C_Order_ID"])))
                        {
                            CreateLine(Util.GetValueOfInt(idr["s_timeexpenseline_id"]), C_Invoice_ID);
                        }
                        else
                        {
                            orders.Add(Util.GetValueOfInt(idr["C_Order_ID"]));
                            VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), Util.GetValueOfInt(idr["s_timeexpenseline_id"]), null);
                            VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);
                            C_Invoice_ID = GenerateInvoice(tLine, tExp, isExpense);
                            invoices.Add(C_Invoice_ID);
                            CreateLine(Util.GetValueOfInt(idr["s_timeexpenseline_id"]), C_Invoice_ID);
                        }
                    }
                    for (int m = 0; m < invoices.Count; m++)
                    {
                        //MInvoice inv = new MInvoice(GetCtx(), Util.GetValueOfInt(pair.Value), null);
                        VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), Util.GetValueOfInt(invoices[m]), null);
                        if (docAction == "CO")
                        {
                            string comp = inv.CompleteIt();
                            if (comp == "CO")
                            {
                                inv.SetDocAction("CL");
                                inv.SetDocStatus("CO");
                                inv.Save();
                            }
                        }
                        else if (docAction == "PR")
                        {
                            string prp = inv.PrepareIt();
                            if (prp == "IP")
                            {
                                inv.SetDocAction("PR");
                                inv.SetDocStatus("IP");
                                inv.Save();
                            }
                        }
                    }

                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                catch
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
            }

            string docNo = "";
            for (int i = 0; i < invoices.Count; i++)
            {
                VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), Util.GetValueOfInt(invoices[i]), null);
                docNo = docNo + ", " + inv.GetDocumentNo();
            }
            if (docNo != "")
            {
                docNo = docNo.Remove(0, 2);
            }
            return Msg.GetMsg(GetCtx(), "InvoicesCreated" + " : " + docNo);
            // return Msg.GetMsg(GetCtx(), "ProcessCompleted");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void CreateLine(int S_TimeExpenseLine_ID, int C_Invoice_ID)
        {
            VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), S_TimeExpenseLine_ID, null);
            VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);

            if (tLine.IsARInvoice())
            {
                if (Util.GetValueOfInt(tLine.GetM_Product_ID()) != 0)
                {
                    sql = "select max(line) from c_invoiceline where c_invoice_id = " + C_Invoice_ID;
                    int lineNo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                    sql = "select C_Invoiceline_ID from C_InvoiceLine where c_invoice_ID = " + C_Invoice_ID + " and m_product_id = " + tLine.GetM_Product_ID();
                    int C_InvoiceLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (C_InvoiceLine_ID != 0)
                    {
                        sql = "select c_uom_id from c_orderline where c_orderline_id = " + tLine.GetC_OrderLine_ID();
                        int C_UOM_IDTo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        sql = "select c_uom_id from m_product where m_product_id = " + tLine.GetM_Product_ID();
                        int C_UOM_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        Decimal? qty = 0;

                        // qty = MUOMConversion.Convert(C_UOM_ID, C_UOM_IDTo, tLine.GetARApprovedHrs(), true);
                        qty = VAdvantage.Model.MUOMConversion.ConvertProductTo(GetCtx(), tLine.GetM_Product_ID(), C_UOM_IDTo, tLine.GetARApprovedHrs());

                        VAdvantage.Model.MInvoiceLine iLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), C_InvoiceLine_ID, null);
                        iLine.SetQtyEntered(Decimal.Add(iLine.GetQtyEntered(), qty.Value));
                        iLine.SetQtyInvoiced(Decimal.Add(iLine.GetQtyInvoiced(), qty.Value));
                        // iLine.SetTaxAmt(Decimal.Add(iLine.GetTaxAmt(), tLine.GetTaxAmt()));
                        iLine.SetLineNetAmt(Decimal.Multiply(iLine.GetQtyEntered(), iLine.GetPriceEntered()));
                        iLine.SetLineTotalAmt(Decimal.Add(iLine.GetLineNetAmt(), iLine.GetTaxAmt()));
                        if (!iLine.Save())
                        {

                        }
                    }
                    else
                    {
                        lineNo = lineNo + 10;
                        Decimal? price = 0;
                       

                        sql = "select c_uom_id from c_orderline where c_orderline_id = " + tLine.GetC_OrderLine_ID();
                        int C_UOM_IDTo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        sql = "select c_uom_id from m_product where m_product_id = " + tLine.GetM_Product_ID();
                        int C_UOM_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        Decimal? qty = 0;

                        qty = VAdvantage.Model.MUOMConversion.ConvertProductTo(GetCtx(), tLine.GetM_Product_ID(), C_UOM_IDTo, tLine.GetARApprovedHrs());

                        VAdvantage.Model.MInvoiceLine iLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), 0, null);
                        iLine.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                        iLine.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                        iLine.SetC_Invoice_ID(C_Invoice_ID);
                        iLine.SetC_Tax_ID(tLine.GetC_Tax_ID());
                        iLine.SetC_UOM_ID(tLine.GetC_UOM_ID());
                        iLine.SetDescription(tLine.GetDescription());
                        iLine.SetM_Product_ID(tLine.GetM_Product_ID());
                        iLine.SetQtyEntered(qty);
                        iLine.SetQtyInvoiced(qty);

                        if (tLine.GetC_OrderLine_ID() != 0)
                        {
                            VAdvantage.Model.MOrderLine oline = new VAdvantage.Model.MOrderLine(GetCtx(), tLine.GetC_OrderLine_ID(), null);
                            price = oline.GetPriceEntered();
                        }

                        iLine.SetPriceEntered(price);
                        iLine.SetPriceList(price);
                        iLine.SetPriceLimit(price);
                        iLine.SetPriceActual(price);
                        // iLine.SetTaxAmt(tLine.GetTaxAmt());
                        iLine.SetLineNetAmt(Decimal.Multiply(iLine.GetQtyEntered(), iLine.GetPriceEntered()));
                        iLine.SetLineTotalAmt(Decimal.Add(iLine.GetLineNetAmt(), iLine.GetTaxAmt()));
                        iLine.SetLine(lineNo);
                        if (!iLine.Save())
                        {

                        }
                    }
                    sql = "update S_TimeExpenseLine set Ref_C_Invoice_ID = " + C_Invoice_ID + " where S_TimeExpenseLine_ID = " + S_TimeExpenseLine_ID;
                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                }
            }

            if (tLine.IsBillToCustomer())
            {
                if (Util.GetValueOfInt(tLine.GetC_Charge_ID()) != 0)
                {
                    sql = "select max(line) from c_invoiceline where c_invoice_id = " + C_Invoice_ID;
                    int lineNo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                    sql = "select C_Invoiceline_ID from C_InvoiceLine where c_invoice_ID = " + C_Invoice_ID + " and c_charge_ID = " + tLine.GetC_Charge_ID() + " and c_tax_id = " + tLine.GetC_Tax_ID();
                    int C_InvoiceLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (C_InvoiceLine_ID != 0)
                    {
                        VAdvantage.Model.MInvoiceLine iLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), C_InvoiceLine_ID, null);
                        iLine.SetPriceEntered(Decimal.Add(tLine.GetApprovedARExpenseAmt(), iLine.GetPriceEntered()));
                        iLine.SetPriceActual(Decimal.Add(tLine.GetApprovedARExpenseAmt(), iLine.GetPriceActual()));
                        iLine.SetPriceLimit(Decimal.Add(tLine.GetApprovedARExpenseAmt(), iLine.GetPriceLimit()));
                        iLine.SetPriceList(Decimal.Add(tLine.GetApprovedARExpenseAmt(), iLine.GetPriceList()));
                        //  iLine.SetTaxAmt(Decimal.Add(tLine.GetTaxAmt(), iLine.GetTaxAmt()));
                        //  iLine.SetLineNetAmt(Decimal.Multiply(iLine.GetQtyEntered(), iLine.GetPriceEntered()));
                        //  iLine.SetLineTotalAmt(Decimal.Add(iLine.GetLineNetAmt(), iLine.GetTaxAmt()));
                        if (!iLine.Save())
                        {

                        }
                    }
                    else
                    {
                        lineNo = lineNo + 10;
                        VAdvantage.Model.MInvoiceLine iLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), 0, null);
                        iLine.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                        iLine.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                        iLine.SetC_Invoice_ID(C_Invoice_ID);
                        iLine.SetC_Tax_ID(tLine.GetC_Tax_ID());
                        iLine.SetC_UOM_ID(100);
                        iLine.SetDescription(tLine.GetDescription());
                        iLine.SetC_Charge_ID(tLine.GetC_Charge_ID());
                        iLine.SetQtyEntered(Decimal.One);
                        iLine.SetQtyInvoiced(Decimal.One);
                        iLine.SetPriceEntered(tLine.GetApprovedARExpenseAmt());
                        iLine.SetPriceActual(tLine.GetApprovedARExpenseAmt());
                        iLine.SetPriceLimit(tLine.GetApprovedARExpenseAmt());
                        iLine.SetPriceList(tLine.GetApprovedARExpenseAmt());
                        // iLine.SetTaxAmt(tLine.GetTaxAmt());
                        // iLine.SetLineNetAmt(Decimal.Multiply(iLine.GetQtyEntered(), iLine.GetPriceEntered()));
                        // iLine.SetLineTotalAmt(Decimal.Add(iLine.GetLineNetAmt(), iLine.GetTaxAmt()));
                        iLine.SetLine(lineNo);
                        if (!iLine.Save())
                        {

                        }

                    }

                    sql = "update S_TimeExpenseLine set Ref_C_Invoice_ID = " + C_Invoice_ID + " where S_TimeExpenseLine_ID = " + S_TimeExpenseLine_ID;
                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tLine"></param>
        private int GenerateInvoice(VAdvantage.Model.X_S_TimeExpenseLine tLine, VAdvantage.Model.X_S_TimeExpense tExp, bool IsExpense)
        {
            int C_PaymentTerm_ID = 0;
            VAdvantage.Model.X_C_Order ord = null;
            if (tLine.GetC_Order_ID() != 0)
            {
                ord = new VAdvantage.Model.X_C_Order(GetCtx(), tLine.GetC_Order_ID(), null);
            }


            sql = "select C_BPartner_Location_ID from c_Bpartner_Location where c_bpartner_ID = " + tLine.GetC_BPartner_ID();
            int C_BPartner_Location_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

            if (_DateInvoiced == null)
            {
                _DateInvoiced = System.DateTime.Now;
            }
            // X_C_Invoice inv = new X_C_Invoice(GetCtx(), 0, null);
            VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), 0, null);
            inv.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
            inv.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
            inv.SetAD_User_ID(tExp.GetAD_User_ID());
            inv.SetC_BPartner_ID(tLine.GetC_BPartner_ID());
            inv.SetC_BPartner_Location_ID(C_BPartner_Location_ID);
            inv.SetC_Currency_ID(tLine.GetC_Currency_ID());
            inv.SetDateInvoiced(_DateInvoiced);
            inv.SetM_PriceList_ID(tExp.GetM_PriceList_ID());
            inv.SetDateAcct(_DateInvoiced);
            inv.SetIsApproved(true);

            //inv.SetPaymentRule();
            if (!IsExpense)
            {
                inv.SetIsSOTrx(true);
                sql = "select C_DocType_ID from c_doctype where name = 'AR Invoice' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                inv.SetC_DocType_ID(C_DocType_ID);
                inv.SetC_DocTypeTarget_ID(C_DocType_ID);
            }
            else
            {
                inv.SetIsSOTrx(false);
                sql = "select C_DocType_ID from c_doctype where docbasetype = 'API' and ad_client_id = " + GetCtx().GetAD_Client_ID();
                int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                inv.SetC_DocType_ID(C_DocType_ID);
                inv.SetC_DocTypeTarget_ID(C_DocType_ID);
            }
            //inv.SetSalesRep_ID();
            inv.SetDocAction("CO");
            inv.SetDocStatus("DR");
            if (ord != null)
            {
                inv.SetC_PaymentTerm_ID(ord.GetC_PaymentTerm_ID());
                inv.SetSalesRep_ID(ord.GetSalesRep_ID());
                inv.SetC_Order_ID(ord.GetC_Order_ID());
                inv.SetM_PriceList_ID(ord.GetM_PriceList_ID());
            }
            else
            {
                sql = " select c_paymentterm_id from c_bpartner where c_bpartner_id = " + tLine.GetC_BPartner_ID();
                C_PaymentTerm_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                if (C_PaymentTerm_ID == 0)
                {
                    sql = "select c_paymentterm_id from c_paymentterm where isdefault = 'Y' and ad_client_id= " + GetCtx().GetAD_Client_ID();
                    C_PaymentTerm_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                }

                sql = "select M_Pricelist_ID from m_Pricelist where issopricelist = 'Y' and isactive = 'Y'";
                inv.SetM_PriceList_ID(Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));

                inv.SetC_PaymentTerm_ID(C_PaymentTerm_ID);
                inv.SetSalesRep_ID(tExp.GetSalesRep_ID());
            }

            if (!inv.Save())
            {
                log.SaveError("InvoiceNotSaved", "InvoiceNotSaved");
                return 0;
            }
            return inv.GetC_Invoice_ID();
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="p"></param>
        //private void CreateLine2(int S_TimeExpenseLine_ID, int C_Invoice_ID)
        //{
        //    X_S_TimeExpenseLine tLine = new X_S_TimeExpenseLine(GetCtx(), S_TimeExpenseLine_ID, null);
        //    X_S_TimeExpense tExp = new X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);

        //    // Case to Generate AP in Generation of AR Invoice
        //    if (tLine.IsExpenseInvoice())
        //    {
        //        if (Util.GetValueOfInt(tLine.GetC_Charge_ID()) != 0)
        //        {
        //            sql = "select max(line) from c_invoiceline where c_invoice_id = " + C_Invoice_ID;
        //            int lineNo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

        //            sql = "select C_Invoiceline_ID from C_InvoiceLine where c_invoice_ID = " + C_Invoice_ID + " and c_charge_ID = " + tLine.GetC_Charge_ID();
        //            int C_InvoiceLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
        //            if (C_InvoiceLine_ID != 0)
        //            {
        //                MInvoiceLine iLine = new MInvoiceLine(GetCtx(), C_InvoiceLine_ID, null);
        //                iLine.SetPriceEntered(Decimal.Add(tLine.GetApprovedExpenseAmt(), iLine.GetPriceEntered()));
        //                iLine.SetPriceActual(Decimal.Add(tLine.GetApprovedExpenseAmt(), iLine.GetPriceActual()));
        //                iLine.SetPriceLimit(Decimal.Add(tLine.GetApprovedExpenseAmt(), iLine.GetPriceLimit()));
        //                iLine.SetPriceList(Decimal.Add(tLine.GetApprovedExpenseAmt(), iLine.GetPriceList()));
        //                iLine.SetTaxAmt(Decimal.Add(tLine.GetTaxAmt(), iLine.GetTaxAmt()));
        //                iLine.SetLineNetAmt(Decimal.Multiply(iLine.GetQtyEntered(), iLine.GetPriceEntered()));
        //                iLine.SetLineTotalAmt(Decimal.Add(iLine.GetLineNetAmt(), iLine.GetTaxAmt()));
        //                if (!iLine.Save())
        //                {

        //                }
        //            }
        //            else
        //            {
        //                lineNo = lineNo + 10;
        //                MInvoiceLine iLine = new MInvoiceLine(GetCtx(), 0, null);
        //                iLine.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
        //                iLine.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
        //                iLine.SetC_Invoice_ID(C_Invoice_ID);
        //                iLine.SetC_Tax_ID(tLine.GetC_Tax_ID());
        //                iLine.SetC_UOM_ID(100);
        //                iLine.SetDescription(tLine.GetDescription());
        //                iLine.SetC_Charge_ID(tLine.GetC_Charge_ID());
        //                iLine.SetQtyEntered(Decimal.One);
        //                iLine.SetQtyInvoiced(Decimal.One);
        //                iLine.SetPriceEntered(tLine.GetApprovedExpenseAmt());
        //                iLine.SetPriceActual(tLine.GetApprovedExpenseAmt());
        //                iLine.SetPriceLimit(tLine.GetApprovedExpenseAmt());
        //                iLine.SetPriceList(tLine.GetApprovedExpenseAmt());
        //                iLine.SetTaxAmt(tLine.GetTaxAmt());
        //                iLine.SetLineNetAmt(Decimal.Multiply(iLine.GetQtyEntered(), iLine.GetPriceEntered()));
        //                iLine.SetLineTotalAmt(Decimal.Add(iLine.GetLineNetAmt(), iLine.GetTaxAmt()));
        //                iLine.SetLine(lineNo);
        //                if (!iLine.Save())
        //                {

        //                }

        //            }
        //        }
        //    }
        //}
    }
}
