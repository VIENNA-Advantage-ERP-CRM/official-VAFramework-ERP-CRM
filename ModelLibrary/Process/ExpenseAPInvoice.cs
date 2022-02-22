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
        private int _C_BPartner_ID = 0;
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
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
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
                + "FROM S_TimeExpense e "
                + "WHERE e.Processed='Y'"
                + " AND e.AD_Client_ID=@param1");				//	#1
            if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
            {
                index++;
                sql.Append(" AND e.C_BPartner_ID=@param2");	//	#2
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
            sql.Append(" AND EXISTS (SELECT * FROM S_TimeExpenseLine el "
                + "WHERE e.S_TimeExpense_ID=el.S_TimeExpense_ID"
                + " AND el.C_InvoiceLine_ID IS NULL"
                + " AND el.ConvertedAmt<>0) "
                + "ORDER BY e.C_BPartner_ID, e.S_TimeExpense_ID");

            //
            int old_BPartner_ID = -1;
            MInvoice invoice = null;
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
                //pstmt.setInt(par++, getAD_Client_ID());
                param[0] = new SqlParameter("@param1", GetAD_Client_ID());
                if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
                {
                    par++;
                    //pstmt.setInt (par++, _C_BPartner_ID);
                    param[par] = new SqlParameter("@param2", _C_BPartner_ID);
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
                        if (te.GetC_BPartner_ID() != old_BPartner_ID)
                        {

                            CompleteInvoice(invoice, te);
                            MBPartner bp = new MBPartner(GetCtx(), te.GetC_BPartner_ID(), Get_TrxName());

                            log.Info("New Invoice for " + bp);
                            invoice = new MInvoice(GetCtx(), 0, Get_TrxName());


                            invoice.SetBPartner(bp);
                            if (invoice.GetC_BPartner_Location_ID() == 0)
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
                                    if (!noPayMethEmp.Contains(bp.GetC_BPartner_ID()))
                                    {
                                        noPayMethEmp.Add(bp.GetC_BPartner_ID());
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

                            // Checking  payment term

                            if (bp.GetPO_PaymentTerm_ID() <= 0)
                            {
                                payterm = GetPaymentTerm(te);
                                if (payterm <= 0)
                                {

                                    if (!PayTermEmp.Contains(bp.GetC_BPartner_ID()))
                                    {
                                        PayTermEmp.Add(bp.GetC_BPartner_ID());
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
                                    invoice.SetC_PaymentTerm_ID(payterm);

                                }


                            }
                            else
                            {
                                //JID_1783_1 if active paymentTerm not found ,then dont create the invoice.
                                if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsActive FROM C_PaymentTerm WHERE C_PaymentTerm_ID=" + bp.GetPO_PaymentTerm_ID(), null, Get_Trx())).Equals("Y"))
                                {
                                    invoice.SetC_PaymentTerm_ID(bp.GetPO_PaymentTerm_ID());
                                }
                                else
                                {
                                    return Msg.GetMsg(GetCtx(), "IsActivePaymentTermInv");
                                }
                            }


                            invoice.SetIsExpenseInvoice(true); //added by arpit asked by Surya Sir on DEC 28,2015                          
                            invoice.SetClientOrg(te.GetAD_Client_ID(), te.GetAD_Org_ID());

                            //invoice.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                            // JID_0868
                            // chanegs done by Bharat on 12 September 2018 to set the document type where Expense Invoice checkbox is true.
                            // String qry = "SELECT C_DocType_ID FROM C_DocType "
                            //+ "WHERE AD_Client_ID=@param1 AND DocBaseType=@param2"
                            //+ " AND IsActive='Y' AND IsExpenseInvoice = 'Y' "
                            //+ "ORDER BY C_DocType_ID DESC ,   IsDefault DESC";
                            String qry = "SELECT C_DocType_ID FROM C_DocType "
                      + "WHERE AD_Client_ID=" + GetAD_Client_ID() + @" AND DocBaseType='" + MDocBaseType.DOCBASETYPE_APINVOICE + @"'"
                      + " AND IsActive='Y' AND IsExpenseInvoice = 'Y'  AND AD_Org_ID IN(0," + te.GetAD_Org_ID() + ") "
                      + " ORDER BY AD_Org_ID Desc, C_DocType_ID DESC ,   IsDefault DESC";

                            //int C_DocType_ID = DB.GetSQLValue(null, qry, GetAD_Client_ID(), MDocBaseType.DOCBASETYPE_APINVOICE);
                            int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            if (C_DocType_ID <= 0)
                            {
                                log.Log(Level.SEVERE, "Not found for AC_Client_ID="
                                    + GetAD_Client_ID() + " - " + MDocBaseType.DOCBASETYPE_APINVOICE);
                                return Msg.GetMsg(GetCtx(), "NoDocTypeExpInvoice");
                            }
                            else
                            {
                                log.Fine(MDocBaseType.DOCBASETYPE_APINVOICE);
                            }
                            invoice.SetC_DocTypeTarget_ID(C_DocType_ID);
                            //invoice.SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_APINVOICE);	//	API

                            //commented by Arpit on Jan 4,2015       Mentis issue no.   0000310
                            //invoice.SetDocumentNo(te.GetDocumentNo());
                            //

                            invoice.SetM_PriceList_ID(te.GetM_PriceList_ID());
                            invoice.SetSalesRep_ID(te.GetDoc_User_ID());
                            String descr = Msg.Translate(GetCtx(), "S_TimeExpense_ID")
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
                                old_BPartner_ID = bp.GetC_BPartner_ID();
                            }
                            //end***************
                        }
                        // JID_0868
                        //Description include all document numbers which is come from Time And Expense Recording window to expense invoice in case of multiple records
                        else if (old_BPartner_ID > 0)
                        {
                            String descr = invoice.GetDescription() + "\n" + Msg.Translate(GetCtx(), "S_TimeExpense_ID")
                                + ": " + te.GetDocumentNo() + " "
                                + DisplayType.GetDateFormat(DisplayType.Date).Format(te.GetDateReport());
                            invoice.SetDescription(descr);
                        }
                        MTimeExpenseLine[] tel = te.GetLines(false);
                        for (int i = 0; i < tel.Length; i++)
                        {
                            MTimeExpenseLine line = tel[i];

                            //	Already Invoiced or nothing to be reimbursed
                            if (line.GetC_InvoiceLine_ID() != 0
                                || Env.ZERO.CompareTo(line.GetQtyReimbursed()) == 0
                                || Env.ZERO.CompareTo(line.GetPriceReimbursed()) == 0)
                            {
                                continue;
                            }
                            //	Update Header info
                            if (line.GetC_Activity_ID() != 0 && line.GetC_Activity_ID() != invoice.GetC_Activity_ID())
                            {
                                invoice.SetC_Activity_ID(line.GetC_Activity_ID());
                            }
                            if (line.GetC_Campaign_ID() != 0 && line.GetC_Campaign_ID() != invoice.GetC_Campaign_ID())
                            {
                                invoice.SetC_Campaign_ID(line.GetC_Campaign_ID());
                            }
                            if (line.GetC_Project_ID() != 0 && line.GetC_Project_ID() != invoice.GetC_Project_ID())
                            {
                                invoice.SetC_Project_ID(line.GetC_Project_ID());
                            }
                            if (!invoice.Save())
                            {
                                return GetRetrievedError(invoice, "Cannot save Invoice");
                                //new Exception("Cannot save Invoice");
                            }

                            //	Create OrderLine
                            MInvoiceLine il = new MInvoiceLine(invoice);
                            
                            if (line.GetM_Product_ID() != 0)
                            {
                                il.SetM_Product_ID(line.GetM_Product_ID(), true);
                                //190 - Get Print description and set                                                              
                                if (il.Get_ColumnIndex("PrintDescription") >= 0)
                                {
                                    string printDesc = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocumentNote FROM M_Product WHERE M_Product_ID=" + line.GetM_Product_ID()));
                                    il.Set_Value("PrintDescription", printDesc);
                                }
                            }
                            //added by arpit asked by Surya Sir on 28/12/2015_____***************************
                            if (line.GetC_Charge_ID() != 0)
                            {
                                il.SetC_Charge_ID(line.GetC_Charge_ID());
                                //190 - Get Print description and set                                                                
                                if (il.Get_ColumnIndex("PrintDescription") >= 0)
                                {
                                    string printDesc = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT PrintDescription FROM C_Charge WHERE C_Charge_ID=" + line.GetC_Charge_ID()));
                                    il.Set_Value("PrintDescription", printDesc);
                                }
                            }
                            //end here *****************************
                            il.SetQty(line.GetQtyReimbursed());     //	Entered/Invoiced
                            il.SetDescription(line.GetDescription());
                            //
                            il.SetC_Project_ID(line.GetC_Project_ID());
                            il.SetC_ProjectPhase_ID(line.GetC_ProjectPhase_ID());
                            il.SetC_ProjectTask_ID(line.GetC_ProjectTask_ID());
                            il.SetC_Activity_ID(line.GetC_Activity_ID());
                            il.SetC_Campaign_ID(line.GetC_Campaign_ID());
                            //
                            //	il.setPrice();	//	not really a list/limit price for reimbursements
                            il.SetPrice(line.GetPriceReimbursed()); //
                            il.SetC_UOM_ID(line.GetC_UOM_ID());

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
                            line.SetC_InvoiceLine_ID(il.GetC_InvoiceLine_ID());
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
            sqlqry = "SELECT VA009_PaymentMethod_ID FROM VA009_PaymentMethod WHERE VA009_PAYMENTBASETYPE='S' AND AD_Client_ID= " + te.GetAD_Client_ID() + " AND AD_Org_ID IN(0," + te.GetAD_Org_ID() + ") AND IsActive='Y' ORDER BY AD_Org_ID DESC, VA009_PAYMENTMETHOD_ID DESC FETCH NEXT 1 ROWS ONLY";
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
            sqlqry1 = "SELECT C_PaymentTerm_ID FROM C_PaymentTerm WHERE ISDEFAULT='Y' AND AD_Client_ID= " + te.GetAD_Client_ID() + "  AND AD_Org_ID IN(0, " + te.GetAD_Org_ID() + " ) AND IsActive='Y' ORDER BY AD_Org_ID DESC, C_PaymentTerm_ID DESC FETCH NEXT 1 ROWS ONLY";
            pt = Util.GetValueOfInt(DB.ExecuteScalar(sqlqry1));
            return pt;
        }

        /// <summary>
        /// Complete Invoice
        /// </summary>
        /// <param name="invoice">invoice</param>
        private void CompleteInvoice(MInvoice invoice, MTimeExpense te)
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
                if (!IncompleteInvoice.Contains(te.GetS_TimeExpense_ID()))
                {
                    IncompleteInvoice.Add(te.GetS_TimeExpense_ID());
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
