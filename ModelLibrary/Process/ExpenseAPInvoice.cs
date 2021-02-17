/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ExpenseAPInvoice
 * Purpose        : Create AP Invoices from Expense Reports
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           2-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class ExpenseAPInvoice : ProcessEngine.SvrProcess
    {
        private int _VAB_BusinessPartner_ID = 0;
        private DateTime? _DateFrom = null;
        private DateTime? _DateTo = null;
        private int _noInvoices = 0;
        private List<int> noPayMethEmp = new List<int>();
        private string bpNameNoPM = "";
        private string message = "";
        private List<int> PayTermEmp = new List<int>();
        private List<int> IncompleteInvoice = new List<int>();
        private string bpNameInvoice = "";
        private string bpNamePT = "";
        private int paymethod = 0;
        private int payterm = 0;
        private string sqlqry, sqlqry1;
        private int pm;
        private int pt;

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
                else if (name.Equals("DateReport"))
                {
                    _DateFrom = (DateTime?)para[i].GetParameter();
                    _DateTo = (DateTime?)para[i].GetParameter_To();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare


        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            int index = 1;
            StringBuilder sql = new StringBuilder("SELECT * "
                + "FROM VAS_ExpenseReport e "
                + "WHERE e.Processed='Y'"
                + " AND e.VAF_Client_ID=@param1");				//	#1
            if (_VAB_BusinessPartner_ID != 0 && _VAB_BusinessPartner_ID != -1)
            {
                index++;
                sql.Append(" AND e.VAB_BusinessPartner_ID=@param2");	//	#2
            }
            if (_DateFrom != null)
            {
                index++;
                sql.Append(" AND e.DateReport >=@param3");	//	#3
            }
            if (_DateTo != null)
            {
                index++;
                sql.Append(" AND e.DateReport <=@param4");	//	#4
            }
            // JID_0868
            // chanegs done by Bharat on 12 September 2018 to handle the case if invoice is created with an expense for the selected Business Partner
            sql.Append(" AND EXISTS (SELECT * FROM VAS_ExpenseReportLine el "
                + "WHERE e.VAS_ExpenseReport_ID=el.VAS_ExpenseReport_ID"
                + " AND el.VAB_InvoiceLine_ID IS NULL"
                + " AND el.ConvertedAmt<>0) "
                + "ORDER BY e.VAB_BusinessPartner_ID, e.VAS_ExpenseReport_ID");

            //
            int old_BPartner_ID = -1;
            MVABInvoice invoice = null;
            MTimeExpense te = null;
            //
            //PreparedStatement pstmt = null;
            SqlParameter[] param = new SqlParameter[index];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql.toString (), get_TrxName());
                int par = 0;
                //pstmt.setInt(par++, getVAF_Client_ID());
                param[0] = new SqlParameter("@param1", GetVAF_Client_ID());
                if (_VAB_BusinessPartner_ID != 0 && _VAB_BusinessPartner_ID != -1)
                {
                    par++;
                    //pstmt.setInt (par++, _VAB_BusinessPartner_ID);
                    param[par] = new SqlParameter("@param2", _VAB_BusinessPartner_ID);
                }
                if (_DateFrom != null)
                {
                    par++;
                    //pstmt.setTimestamp (par++, _DateFrom);
                    param[par] = new SqlParameter("@param3", _DateFrom);
                }
                if (_DateTo != null)
                {
                    par++;
                    //pstmt.setTimestamp (par++, _DateTo);
                    param[par] = new SqlParameter("@param4", _DateTo);
                }
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    //	********* Expense Line Loop
                    {
                        te = new MTimeExpense(GetCtx(), dr, Get_TrxName());

                        //	New BPartner - New Order
                        // 
                        if (te.GetVAB_BusinessPartner_ID() != old_BPartner_ID)
                        {

                            CompleteInvoice(invoice, te);
                            MVABBusinessPartner bp = new MVABBusinessPartner(GetCtx(), te.GetVAB_BusinessPartner_ID(), Get_TrxName());

                            log.Info("New Invoice for " + bp);
                            invoice = new MVABInvoice(GetCtx(), 0, Get_TrxName());

                            invoice.SetBPartner(bp);
                            if (invoice.GetVAB_BPart_Location_ID() == 0)
                            {
                                log.Log(Level.SEVERE, "No BP Location: " + bp);
                                AddLog(0, te.GetDateReport(),
                                    null, "No Location: " + te.GetDocumentNo() + " " + bp.GetName());
                                invoice = null;
                                break;
                            }

                            // Siddheshwar: added a code to check for payment method if null
                            if (bp.GetVA009_PO_PaymentMethod_ID() <= 0)
                            {

                                paymethod = GetPaymentMethod(te);
                                if (paymethod <= 0)
                                {
                                    if (!noPayMethEmp.Contains(bp.GetVAB_BusinessPartner_ID()))
                                    {
                                        noPayMethEmp.Add(bp.GetVAB_BusinessPartner_ID());
                                        if (string.IsNullOrEmpty(bpNameNoPM))
                                        {
                                            bpNameNoPM = bp.GetName();
                                        }
                                        else
                                        {
                                            bpNameNoPM += bp.GetName() + ", ";
                                        }
                                        return Msg.GetMsg(GetCtx(), "NoPayMethEmp") + bpNameNoPM;
                                    }

                                }
                                else
                                {
                                    invoice.SetVA009_PaymentMethod_ID(paymethod);

                                }

                            }
                            else
                            {
                                //JID_1783_1 if active paymentMethod not found ,then dont create the invoice.

                                if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID=" + bp.GetVA009_PO_PaymentMethod_ID(), null, Get_Trx())).Equals("Y"))
                                {
                                    invoice.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                                }
                                else
                                {
                                    return Msg.GetMsg(GetCtx(), "IsActivePaymentMethodInv"); ;
                                }
                            }

                            //Checking Payment Term

                            if (bp.GetPO_PaymentTerm_ID() <= 0)
                            {
                                payterm = GetPaymentTerm(te);
                                if (payterm <= 0)
                                {

                                    if (!PayTermEmp.Contains(bp.GetVAB_BusinessPartner_ID()))
                                    {
                                        PayTermEmp.Add(bp.GetVAB_BusinessPartner_ID());
                                        if (string.IsNullOrEmpty(bpNamePT))
                                        {
                                            bpNamePT = bp.GetName();
                                        }
                                        else
                                        {
                                            bpNamePT += bp.GetName() + ", ";

                                        }
                                        return Msg.GetMsg(GetCtx(), "NoPayTerm") + bpNamePT;
                                    }

                                }
                                else
                                {
                                    invoice.SetVAB_PaymentTerm_ID(payterm);

                                }


                            }
                            else
                            {
                                //JID_1783_1 if active paymentTerm not found ,then dont create the invoice.
                                if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM VAB_PaymentTerm WHERE VAB_PaymentTerm_ID=" + bp.GetPO_PaymentTerm_ID(), null, Get_Trx())).Equals("Y"))
                                {
                                    invoice.SetVAB_PaymentTerm_ID(bp.GetPO_PaymentTerm_ID());
                                }
                                else
                                {
                                    return Msg.GetMsg(GetCtx(), "IsActivePaymentTermInv"); ;
                                }
                            }




                            invoice.SetIsExpenseInvoice(true); //added by arpit asked by Surya Sir on DEC 28,2015
                            invoice.SetClientOrg(te.GetVAF_Client_ID(), te.GetVAF_Org_ID());

                            //invoice.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                            // JID_0868
                            // chanegs done by Bharat on 12 September 2018 to set the document type where Expense Invoice checkbox is true.
                            // String qry = "SELECT VAB_DocTypes_ID FROM VAB_DocTypes "
                            //+ "WHERE VAF_Client_ID=@param1 AND DocBaseType=@param2"
                            //+ " AND IsActive='Y' AND IsExpenseInvoice = 'Y' "
                            //+ "ORDER BY VAB_DocTypes_ID DESC ,   IsDefault DESC";
                            String qry = "SELECT VAB_DocTypes_ID FROM VAB_DocTypes "
                      + "WHERE VAF_Client_ID=" + GetVAF_Client_ID() + @" AND DocBaseType='" + MVABMasterDocType.DOCBASETYPE_APINVOICE + @"'"
                      + " AND IsActive='Y' AND IsExpenseInvoice = 'Y'  AND VAF_Org_ID IN(0," + te.GetVAF_Org_ID() + ") "
                      + " ORDER BY VAF_Org_ID Desc, VAB_DocTypes_ID DESC ,   IsDefault DESC";

                            //int VAB_DocTypes_ID = DB.GetSQLValue(null, qry, GetVAF_Client_ID(), MVABMasterDocType.DOCBASETYPE_APINVOICE);
                            int VAB_DocTypes_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (VAB_DocTypes_ID <= 0)
                            {
                                log.Log(Level.SEVERE, "Not found for AC_Client_ID="
                                    + GetVAF_Client_ID() + " - " + MVABMasterDocType.DOCBASETYPE_APINVOICE);
                                return Msg.GetMsg(GetCtx(), "NoDocTypeExpInvoice");
                            }
                            else
                            {
                                log.Fine(MVABMasterDocType.DOCBASETYPE_APINVOICE);
                            }
                            invoice.SetVAB_DocTypesTarget_ID(VAB_DocTypes_ID);
                            //invoice.SetVAB_DocTypesTarget_ID(MVABMasterDocType.DOCBASETYPE_APINVOICE);	//	API

                            //commented by Arpit on Jan 4,2015       Mentis issue no.   0000310
                            //invoice.SetDocumentNo(te.GetDocumentNo());
                            //
                            invoice.SetBPartner(bp);
                            if (invoice.GetVAB_BPart_Location_ID() == 0)
                            {
                                log.Log(Level.SEVERE, "No BP Location: " + bp);
                                AddLog(0, te.GetDateReport(),
                                    null, "No Location: " + te.GetDocumentNo() + " " + bp.GetName());
                                invoice = null;
                                break;
                            }
                            invoice.SetVAM_PriceList_ID(te.GetVAM_PriceList_ID());
                            invoice.SetSalesRep_ID(te.GetDoc_User_ID());
                            String descr = Msg.Translate(GetCtx(), "VAS_ExpenseReport_ID")
                                + ": " + te.GetDocumentNo() + " "
                                + DisplayType.GetDateFormat(DisplayType.Date).Format(te.GetDateReport());
                            invoice.SetDescription(descr);
                            if (!invoice.Save())
                            {
                                return GetRetrievedError(invoice, "Cannot save Invoice");
                                // new Exception("Cannot save Invoice"); 
                            }
                            //added by arpit asked by Surya Sir on 29/12/2015*******
                            else
                            {
                                old_BPartner_ID = bp.GetVAB_BusinessPartner_ID();
                            }
                            //end***************
                        }
                        // JID_0868
                        //Description include all document numbers which is come from Time And Expense Recording window to expense invoice in case of multiple records
                        else if (old_BPartner_ID > 0)
                        {
                            String descr = invoice.GetDescription() + "\n" + Msg.Translate(GetCtx(), "VAS_ExpenseReport_ID")
                                + ": " + te.GetDocumentNo() + " "
                                + DisplayType.GetDateFormat(DisplayType.Date).Format(te.GetDateReport());
                            invoice.SetDescription(descr);
                        }
                        MTimeExpenseLine[] tel = te.GetLines(false);
                        for (int i = 0; i < tel.Length; i++)
                        {
                            MTimeExpenseLine line = tel[i];

                            //	Already Invoiced or nothing to be reimbursed
                            if (line.GetVAB_InvoiceLine_ID() != 0
                                || Env.ZERO.CompareTo(line.GetQtyReimbursed()) == 0
                                || Env.ZERO.CompareTo(line.GetPriceReimbursed()) == 0)
                            {
                                continue;
                            }
                            //	Update Header info
                            if (line.GetVAB_BillingCode_ID() != 0 && line.GetVAB_BillingCode_ID() != invoice.GetVAB_BillingCode_ID())
                            {
                                invoice.SetVAB_BillingCode_ID(line.GetVAB_BillingCode_ID());
                            }
                            if (line.GetVAB_Promotion_ID() != 0 && line.GetVAB_Promotion_ID() != invoice.GetVAB_Promotion_ID())
                            {
                                invoice.SetVAB_Promotion_ID(line.GetVAB_Promotion_ID());
                            }
                            if (line.GetVAB_Project_ID() != 0 && line.GetVAB_Project_ID() != invoice.GetVAB_Project_ID())
                            {
                                invoice.SetVAB_Project_ID(line.GetVAB_Project_ID());
                            }
                            if (!invoice.Save())
                            {
                                return GetRetrievedError(invoice, "Cannot save Invoice");
                                //new Exception("Cannot save Invoice");
                            }

                            //	Create OrderLine
                            MVABInvoiceLine il = new MVABInvoiceLine(invoice);
                            //
                            if (line.GetVAM_Product_ID() != 0)
                            {
                                il.SetVAM_Product_ID(line.GetVAM_Product_ID(), true);
                            }
                            //added by arpit asked by Surya Sir on 28/12/2015_____***************************
                            if (line.GetVAB_Charge_ID() != 0)
                            {
                                il.SetVAB_Charge_ID(line.GetVAB_Charge_ID());
                            }
                            //end here *****************************
                            il.SetQty(line.GetQtyReimbursed());     //	Entered/Invoiced
                            il.SetDescription(line.GetDescription());
                            //
                            il.SetVAB_Project_ID(line.GetVAB_Project_ID());
                            il.SetVAB_ProjectStage_ID(line.GetVAB_ProjectStage_ID());
                            il.SetVAB_ProjectJob_ID(line.GetVAB_ProjectJob_ID());
                            il.SetVAB_BillingCode_ID(line.GetVAB_BillingCode_ID());
                            il.SetVAB_Promotion_ID(line.GetVAB_Promotion_ID());
                            //
                            //	il.setPrice();	//	not really a list/limit price for reimbursements
                            il.SetPrice(line.GetPriceReimbursed()); //
                            il.SetVAB_UOM_ID(line.GetVAB_UOM_ID());

                            // JID_0868
                            // chanegs done by Bharat on 12 September 2018 to set the Amount in List price column.
                            il.SetPriceList(line.GetPriceReimbursed());
                            il.SetTax();
                            if (!il.Save())
                            {
                                return GetRetrievedError(il, "Cannot save Invoice");
                                //new Exception("Cannot save Invoice Line");
                            }
                            //	Update TEL
                            line.SetVAB_InvoiceLine_ID(il.GetVAB_InvoiceLine_ID());
                            line.SetIsInvoiced(true);
                            line.Save();
                        }   //	for all expense lines
                    }
                }
                else
                {
                    message = Msg.GetMsg(GetCtx(), "NoRecForInv");
                }
                //	********* Expense Line Loop
                dt = null;
                //dt.Clear();
            }
            catch (Exception e)
            {
                if (dt != null)
                {
                    dt = null;
                }
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (dt != null)
                {
                    dt = null;
                }
                if (idr != null)
                {
                    idr.Close();
                }
            }
            CompleteInvoice(invoice, te);
            //if (_noInvoices == 0)
            //{

            //    message = " @No Record Found for Invoice  Creation@";
            //}

            //Code for Showing Message when PaymentTerm is null
            if (!string.IsNullOrEmpty(bpNamePT))
            {
                message += Msg.GetMsg(GetCtx(), "NoPayTerm") + bpNamePT + "\n";
            }

            //code for showing Error Message when Invoice is not completed
            if (!string.IsNullOrEmpty(bpNameInvoice))
            {
                message += Msg.GetMsg(GetCtx(), "NoInvoiceCreated") + bpNameInvoice + "\n";
            }

            if (!string.IsNullOrEmpty(bpNameNoPM))
            {
                message = Msg.GetMsg(GetCtx(), "NoPayMethEmp") + bpNameNoPM + "\n";
            }
            if (_noInvoices > 0)
            {
                message += "" + _noInvoices + " " + Msg.GetMsg(GetCtx(), "VIS_InvGenerated");
            }

            //return "" + _noInvoices + " @Invoices Generated Successfully@";
            return message;
        }   //	doIt

        /// <summary>
        /// Method to get default PaymentMethod
        /// </summary>
        /// <param name="te">TimeExpense</param>
        /// <returns></returns>
        public int GetPaymentMethod(MTimeExpense te)
        {
            //JID_1783_1 add isActive Check
            sqlqry = "SELECT VA009_PaymentMethod_ID FROM VA009_PaymentMethod WHERE VA009_PAYMENTBASETYPE='S' AND VAF_Client_ID= " + te.GetVAF_Client_ID() + " AND VAF_Org_ID IN(0," + te.GetVAF_Org_ID() + ") AND IsActive='Y' ORDER BY VAF_Org_ID DESC, VA009_PAYMENTMETHOD_ID DESC FETCH NEXT 1 ROWS ONLY";
            pm = Util.GetValueOfInt(DB.ExecuteScalar(sqlqry));
            return pm;
        }

        /// <summary>
        /// Method to get default PaymentTerm
        /// </summary>
        /// <param name="te">TimeExpense</param>
        /// <returns></returns>
        public int GetPaymentTerm(MTimeExpense te)
        {
            //JID_1783_1 add isActive Check
            sqlqry1 = "SELECT VAB_PaymentTerm_ID FROM VAB_PaymentTerm WHERE ISDEFAULT='Y' AND VAF_Client_ID= " + te.GetVAF_Client_ID() + "  AND VAF_Org_ID IN(0, " + te.GetVAF_Org_ID() + " ) AND IsActive='Y' ORDER BY VAF_Org_ID DESC, VAB_PaymentTerm_ID DESC FETCH NEXT 1 ROWS ONLY";
            pt = Util.GetValueOfInt(DB.ExecuteScalar(sqlqry1));
            return pt;
        }

        /// <summary>
        /// Complete Invoice
        /// </summary>
        /// <param name="invoice">invoice</param>
        private void CompleteInvoice(MVABInvoice invoice, MTimeExpense te)
        {
            if (invoice == null)
            {
                return;
            }

            invoice.SetDocAction(DocActionVariables.ACTION_PREPARE);
            invoice.ProcessIt(DocActionVariables.ACTION_COMPLETE);
            if (!invoice.Save())
            {
                //Added By Siddheshwar
                if (!IncompleteInvoice.Contains(te.GetVAS_ExpenseReport_ID()))
                {
                    IncompleteInvoice.Add(te.GetVAS_ExpenseReport_ID());
                    if (string.IsNullOrEmpty(bpNameInvoice))
                    {
                        bpNameInvoice = te.GetDocumentNo();
                    }
                    else
                    {
                        bpNameInvoice += te.GetDocumentNo() + ", ";

                    }
                }
                new Exception(invoice + "Cannot save Invoice");
                Rollback();
            }
            else
            {
                Commit();
                _noInvoices++;
                AddLog(invoice.Get_ID(), invoice.GetDateInvoiced(),
                    invoice.GetGrandTotal(), invoice.GetDocumentNo());

            }

        }	//	completeInvoice

    }	//	ExpenseAPInvoice

}
