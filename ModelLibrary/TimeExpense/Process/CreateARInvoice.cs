using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
//using ViennaAdvantage.Process;
//////using System.Windows.Forms;
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
        private int _VAB_BusinessPartner_ID = 0;
        private DateTime? _DateInvoiced = null;
       // private int _noInvoices = 0;
        string sql = "";
       // string msg = "";
        List<int> VAB_BusinessPartner_ID = new List<int>();
        // List<int> VAB_BusinessPartner_IDAP = new List<int>();
        List<int> M_Product_ID = new List<int>();
        Dictionary<int, int> BPInvoice = new Dictionary<int, int>();
        // Dictionary<int, int> BPAPInvoice = new Dictionary<int, int>();
        int VAF_Org_ID = 0;
        int VAB_Order_ID = 0;
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
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateInvoiced"))
                {
                    _DateInvoiced = (DateTime?)para[i].GetParameter();
                    // _DateTo = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("VAF_Org_ID"))
                {
                    // _DateFrom = (DateTime?)para[i].GetParameter();
                    VAF_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("VAB_Order_ID"))
                {
                    // _DateFrom = (DateTime?)para[i].GetParameter();
                    VAB_Order_ID = para[i].GetParameterAsInt();
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

                string sqlSelect = "select * from s_timeexpenseline where processed = 'Y' and (ARInvoice = 'Y' or billtocustomer = 'Y')  and Ref_VAB_Invoice_ID is null";
                StringBuilder sqlWhere = new StringBuilder();
                if (VAB_Order_ID != 0)
                {
                    sqlWhere.Append(" AND VAB_Order_ID = " + VAB_Order_ID);
                }
                else
                {
                    if (_VAB_BusinessPartner_ID != 0)
                    {
                        sqlWhere.Append(" AND VAB_BusinessPartner_ID = " + _VAB_BusinessPartner_ID);
                    }
                    if (VAF_Org_ID != 0)
                    {
                        sqlWhere.Append(" AND VAF_Org_ID = " + VAF_Org_ID);
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

                sql = sql + " order by VAB_Order_ID, VAB_BusinessPartner_id";

                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        if (VAB_BusinessPartner_ID.Contains(Util.GetValueOfInt(idr["VAB_BusinessPartner_ID"])))
                        {
                            VAdvantage.Model.MOrder ord = new VAdvantage.Model.MOrder(GetCtx(), Util.GetValueOfInt(idr["VAB_Order_ID"]), null);
                            bool chk = false;
                            for (int i = 0; i < invoices.Count; i++)
                            {
                                VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), Util.GetValueOfInt(invoices[i]), null);

                                if ((inv.GetVAB_PaymentTerm_ID() == ord.GetVAB_PaymentTerm_ID()) && (inv.GetM_PriceList_ID() == ord.GetM_PriceList_ID()))
                                {
                                    chk = true;
                                    break;
                                }
                            }
                            if (!chk)
                            {
                                VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), Util.GetValueOfInt(idr["s_timeexpenseline_id"]), null);
                                VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);
                                int VAB_Invoice_ID = GenerateInvoice(tLine, tExp, isExpense);
                                invoices.Add(VAB_Invoice_ID);
                            }
                        }
                        else
                        {
                            VAB_BusinessPartner_ID.Add(Util.GetValueOfInt(idr["VAB_BusinessPartner_ID"]));
                            VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), Util.GetValueOfInt(idr["s_timeexpenseline_id"]), null);
                            VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);
                            int VAB_Invoice_ID = GenerateInvoice(tLine, tExp, isExpense);
                            invoices.Add(VAB_Invoice_ID);
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
                            if (VAB_Order_ID != 0)
                            {
                                sqlWhere.Append(" AND VAB_Order_ID = " + VAB_Order_ID);
                            }
                            else
                            {
                                if (_VAB_BusinessPartner_ID != 0)
                                {
                                    sqlWhere.Append(" AND VAB_BusinessPartner_ID = " + _VAB_BusinessPartner_ID);
                                }
                                if (VAF_Org_ID != 0)
                                {
                                    sqlWhere.Append(" AND VAF_Org_ID = " + VAF_Org_ID);
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

                            sql = sql + " order by VAB_Order_ID, VAB_BusinessPartner_id";

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
                                            VAdvantage.Model.MOrder ord = new VAdvantage.Model.MOrder(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[j]["VAB_Order_ID"]), null);
                                            VAdvantage.Model.MInvoice inv1 = new VAdvantage.Model.MInvoice(GetCtx(), Util.GetValueOfInt(invoices[l]), null);
                                            if ((inv1.GetVAB_PaymentTerm_ID() == ord.GetVAB_PaymentTerm_ID()) && (inv1.GetVAB_BusinessPartner_ID() == ord.GetVAB_BusinessPartner_ID())
                                                && (inv1.GetM_PriceList_ID() == ord.GetM_PriceList_ID()))
                                            {
                                                chk1 = true;
                                                invID = inv1.GetVAB_Invoice_ID();
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
                string sqlSelect = "select * from s_timeexpenseline where processed = 'Y' and (ARInvoice = 'Y' or billtocustomer = 'Y')  and Ref_VAB_Invoice_ID is null";
                StringBuilder sqlWhere = new StringBuilder();
                if (VAB_Order_ID != 0)
                {
                    sqlWhere.Append(" AND VAB_Order_ID = " + VAB_Order_ID);
                }
                else
                {
                    if (_VAB_BusinessPartner_ID != 0)
                    {
                        sqlWhere.Append(" AND VAB_BusinessPartner_ID = " + _VAB_BusinessPartner_ID);
                    }
                    if (VAF_Org_ID != 0)
                    {
                        sqlWhere.Append(" AND VAF_Org_ID = " + VAF_Org_ID);
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

                sql = sql + " order by VAB_Order_ID, VAB_BusinessPartner_id";
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, null);
                    int VAB_Invoice_ID = 0;
                    while (idr.Read())
                    {
                        if (orders.Contains(Util.GetValueOfInt(idr["VAB_Order_ID"])))
                        {
                            CreateLine(Util.GetValueOfInt(idr["s_timeexpenseline_id"]), VAB_Invoice_ID);
                        }
                        else
                        {
                            orders.Add(Util.GetValueOfInt(idr["VAB_Order_ID"]));
                            VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), Util.GetValueOfInt(idr["s_timeexpenseline_id"]), null);
                            VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);
                            VAB_Invoice_ID = GenerateInvoice(tLine, tExp, isExpense);
                            invoices.Add(VAB_Invoice_ID);
                            CreateLine(Util.GetValueOfInt(idr["s_timeexpenseline_id"]), VAB_Invoice_ID);
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
        private void CreateLine(int S_TimeExpenseLine_ID, int VAB_Invoice_ID)
        {
            VAdvantage.Model.X_S_TimeExpenseLine tLine = new VAdvantage.Model.X_S_TimeExpenseLine(GetCtx(), S_TimeExpenseLine_ID, null);
            VAdvantage.Model.X_S_TimeExpense tExp = new VAdvantage.Model.X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);

            if (tLine.IsARInvoice())
            {
                if (Util.GetValueOfInt(tLine.GetM_Product_ID()) != 0)
                {
                    sql = "select max(line) from VAB_InvoiceLine where VAB_Invoice_id = " + VAB_Invoice_ID;
                    int lineNo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                    sql = "select VAB_InvoiceLine_ID from VAB_InvoiceLine where VAB_Invoice_ID = " + VAB_Invoice_ID + " and m_product_id = " + tLine.GetM_Product_ID();
                    int VAB_InvoiceLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (VAB_InvoiceLine_ID != 0)
                    {
                        sql = "select VAB_UOM_id from VAB_Orderline where VAB_Orderline_id = " + tLine.GetVAB_OrderLine_ID();
                        int VAB_UOM_IDTo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        sql = "select VAB_UOM_id from m_product where m_product_id = " + tLine.GetM_Product_ID();
                        int VAB_UOM_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        Decimal? qty = 0;

                        // qty = MUOMConversion.Convert(VAB_UOM_ID, VAB_UOM_IDTo, tLine.GetARApprovedHrs(), true);
                        qty = VAdvantage.Model.MUOMConversion.ConvertProductTo(GetCtx(), tLine.GetM_Product_ID(), VAB_UOM_IDTo, tLine.GetARApprovedHrs());

                        VAdvantage.Model.MInvoiceLine iLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), VAB_InvoiceLine_ID, null);
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
                       

                        sql = "select VAB_UOM_id from VAB_Orderline where VAB_Orderline_id = " + tLine.GetVAB_OrderLine_ID();
                        int VAB_UOM_IDTo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        sql = "select VAB_UOM_id from m_product where m_product_id = " + tLine.GetM_Product_ID();
                        int VAB_UOM_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        Decimal? qty = 0;

                        qty = VAdvantage.Model.MUOMConversion.ConvertProductTo(GetCtx(), tLine.GetM_Product_ID(), VAB_UOM_IDTo, tLine.GetARApprovedHrs());

                        VAdvantage.Model.MInvoiceLine iLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), 0, null);
                        iLine.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                        iLine.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                        iLine.SetVAB_Invoice_ID(VAB_Invoice_ID);
                        iLine.SetVAB_TaxRate_ID(tLine.GetVAB_TaxRate_ID());
                        iLine.SetVAB_UOM_ID(tLine.GetVAB_UOM_ID());
                        iLine.SetDescription(tLine.GetDescription());
                        iLine.SetM_Product_ID(tLine.GetM_Product_ID());
                        iLine.SetQtyEntered(qty);
                        iLine.SetQtyInvoiced(qty);

                        if (tLine.GetVAB_OrderLine_ID() != 0)
                        {
                            VAdvantage.Model.MOrderLine oline = new VAdvantage.Model.MOrderLine(GetCtx(), tLine.GetVAB_OrderLine_ID(), null);
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
                    sql = "update S_TimeExpenseLine set Ref_VAB_Invoice_ID = " + VAB_Invoice_ID + " where S_TimeExpenseLine_ID = " + S_TimeExpenseLine_ID;
                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
                }
            }

            if (tLine.IsBillToCustomer())
            {
                if (Util.GetValueOfInt(tLine.GetVAB_Charge_ID()) != 0)
                {
                    sql = "select max(line) from VAB_InvoiceLine where VAB_Invoice_id = " + VAB_Invoice_ID;
                    int lineNo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                    sql = "select VAB_InvoiceLine_ID from VAB_InvoiceLine where VAB_Invoice_ID = " + VAB_Invoice_ID + " and VAB_Charge_ID = " + tLine.GetVAB_Charge_ID() + " and VAB_TaxRate_id = " + tLine.GetVAB_TaxRate_ID();
                    int VAB_InvoiceLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (VAB_InvoiceLine_ID != 0)
                    {
                        VAdvantage.Model.MInvoiceLine iLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), VAB_InvoiceLine_ID, null);
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
                        iLine.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
                        iLine.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
                        iLine.SetVAB_Invoice_ID(VAB_Invoice_ID);
                        iLine.SetVAB_TaxRate_ID(tLine.GetVAB_TaxRate_ID());
                        iLine.SetVAB_UOM_ID(100);
                        iLine.SetDescription(tLine.GetDescription());
                        iLine.SetVAB_Charge_ID(tLine.GetVAB_Charge_ID());
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

                    sql = "update S_TimeExpenseLine set Ref_VAB_Invoice_ID = " + VAB_Invoice_ID + " where S_TimeExpenseLine_ID = " + S_TimeExpenseLine_ID;
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
            int VAB_PaymentTerm_ID = 0;
            VAdvantage.Model.X_VAB_Order ord = null;
            if (tLine.GetVAB_Order_ID() != 0)
            {
                ord = new VAdvantage.Model.X_VAB_Order(GetCtx(), tLine.GetVAB_Order_ID(), null);
            }


            sql = "select VAB_BPart_Location_ID from VAB_BPart_Location where VAB_BusinessPartner_ID = " + tLine.GetVAB_BusinessPartner_ID();
            int VAB_BPart_Location_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

            if (_DateInvoiced == null)
            {
                _DateInvoiced = System.DateTime.Now;
            }
            // X_VAB_Invoice inv = new X_VAB_Invoice(GetCtx(), 0, null);
            VAdvantage.Model.MInvoice inv = new VAdvantage.Model.MInvoice(GetCtx(), 0, null);
            inv.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
            inv.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
            inv.SetVAF_UserContact_ID(tExp.GetVAF_UserContact_ID());
            inv.SetVAB_BusinessPartner_ID(tLine.GetVAB_BusinessPartner_ID());
            inv.SetVAB_BPart_Location_ID(VAB_BPart_Location_ID);
            inv.SetVAB_Currency_ID(tLine.GetVAB_Currency_ID());
            inv.SetDateInvoiced(_DateInvoiced);
            inv.SetM_PriceList_ID(tExp.GetM_PriceList_ID());
            inv.SetDateAcct(_DateInvoiced);
            inv.SetIsApproved(true);

            //inv.SetPaymentRule();
            if (!IsExpense)
            {
                inv.SetIsSOTrx(true);
                sql = "select VAB_DocTypes_ID from VAB_DocTypes where name = 'AR Invoice' and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
                int VAB_DocTypes_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                inv.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
                inv.SetVAB_DocTypesTarget_ID(VAB_DocTypes_ID);
            }
            else
            {
                inv.SetIsSOTrx(false);
                sql = "select VAB_DocTypes_ID from VAB_DocTypes where docbasetype = 'API' and vaf_client_id = " + GetCtx().GetVAF_Client_ID();
                int VAB_DocTypes_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                inv.SetVAB_DocTypes_ID(VAB_DocTypes_ID);
                inv.SetVAB_DocTypesTarget_ID(VAB_DocTypes_ID);
            }
            //inv.SetSalesRep_ID();
            inv.SetDocAction("CO");
            inv.SetDocStatus("DR");
            if (ord != null)
            {
                inv.SetVAB_PaymentTerm_ID(ord.GetVAB_PaymentTerm_ID());
                inv.SetSalesRep_ID(ord.GetSalesRep_ID());
                inv.SetVAB_Order_ID(ord.GetVAB_Order_ID());
                inv.SetM_PriceList_ID(ord.GetM_PriceList_ID());
            }
            else
            {
                sql = " select VAB_Paymentterm_id from VAB_BusinessPartner where VAB_BusinessPartner_id = " + tLine.GetVAB_BusinessPartner_ID();
                VAB_PaymentTerm_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                if (VAB_PaymentTerm_ID == 0)
                {
                    sql = "select VAB_Paymentterm_id from VAB_Paymentterm where isdefault = 'Y' and vaf_client_id= " + GetCtx().GetVAF_Client_ID();
                    VAB_PaymentTerm_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                }

                sql = "select M_Pricelist_ID from m_Pricelist where issopricelist = 'Y' and isactive = 'Y'";
                inv.SetM_PriceList_ID(Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)));

                inv.SetVAB_PaymentTerm_ID(VAB_PaymentTerm_ID);
                inv.SetSalesRep_ID(tExp.GetSalesRep_ID());
            }

            if (!inv.Save())
            {
                log.SaveError("InvoiceNotSaved", "InvoiceNotSaved");
                return 0;
            }
            return inv.GetVAB_Invoice_ID();
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="p"></param>
        //private void CreateLine2(int S_TimeExpenseLine_ID, int VAB_Invoice_ID)
        //{
        //    X_S_TimeExpenseLine tLine = new X_S_TimeExpenseLine(GetCtx(), S_TimeExpenseLine_ID, null);
        //    X_S_TimeExpense tExp = new X_S_TimeExpense(GetCtx(), Util.GetValueOfInt(tLine.GetS_TimeExpense_ID()), null);

        //    // Case to Generate AP in Generation of AR Invoice
        //    if (tLine.IsExpenseInvoice())
        //    {
        //        if (Util.GetValueOfInt(tLine.GetVAB_Charge_ID()) != 0)
        //        {
        //            sql = "select max(line) from VAB_InvoiceLine where VAB_Invoice_id = " + VAB_Invoice_ID;
        //            int lineNo = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

        //            sql = "select VAB_InvoiceLine_ID from VAB_InvoiceLine where VAB_Invoice_ID = " + VAB_Invoice_ID + " and VAB_Charge_ID = " + tLine.GetVAB_Charge_ID();
        //            int VAB_InvoiceLine_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
        //            if (VAB_InvoiceLine_ID != 0)
        //            {
        //                MInvoiceLine iLine = new MInvoiceLine(GetCtx(), VAB_InvoiceLine_ID, null);
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
        //                iLine.SetVAF_Client_ID(GetCtx().GetVAF_Client_ID());
        //                iLine.SetVAF_Org_ID(GetCtx().GetVAF_Org_ID());
        //                iLine.SetVAB_Invoice_ID(VAB_Invoice_ID);
        //                iLine.SetVAB_TaxRate_ID(tLine.GetVAB_TaxRate_ID());
        //                iLine.SetVAB_UOM_ID(100);
        //                iLine.SetDescription(tLine.GetDescription());
        //                iLine.SetVAB_Charge_ID(tLine.GetVAB_Charge_ID());
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
