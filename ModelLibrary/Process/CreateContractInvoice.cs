/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : CreateContractInvoice
 * Purpose        : 
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   02-Feb-2012
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Security.Policy;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
//using ViennaAdvantage.Model;

namespace ViennaAdvantageServer.Process
{
    public class CreateContractInvoice : SvrProcess
    {
        #region Private Variables
        private string _Result = "";
        StringBuilder sql = new StringBuilder();
        //int _count = 0;
        //Neha--Declare X-Class or MClass object publically for further use in class--11 Sep,2018
        VAdvantage.Model.X_C_Contract cont = null;
        VAdvantage.Model.X_C_ContractSchedule contSchedule = null;
        VAdvantage.Model.MInvoice inv = null;
        MBPartner bp = null;
        int _C_DocType_ID = 0;
        StringBuilder msgInvoiceNotSaved = new StringBuilder();
        StringBuilder msgInvoiceSaved = new StringBuilder();
        StringBuilder msgInvoiceSchNotFound = new StringBuilder();
        StringBuilder msgDocTypeNotDefined = new StringBuilder();
        StringBuilder msgPaymentMethodNotDefined = new StringBuilder();
        StringBuilder msgInvoiceNotFound = new StringBuilder();
        string msg = string.Empty;
        DataSet ds = null;
        DataSet dsSch = null;
        int _tableId = 0, _processId = 0;
        bool DocPaymentCheck = true;
        #endregion

        protected override void Prepare()
        {

        }
        /// <summary>
        /// Create AR and AP Invoices
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            int C_Contract_ID = Util.GetValueOfInt(GetRecord_ID());
            // Get Invoice TableId and DocAction ProcessId to execute workflow
            sql.Append("SELECT T.AD_TABLE_ID,C.AD_PROCESS_ID from AD_TABLE T INNER JOIN AD_COLUMN C ON C.AD_TABLE_ID=T.AD_TABLE_ID WHERE UPPER(C.ColumnName)='DOCACTION' AND UPPER(T.TableName)='C_INVOICE'");
            DataSet dsTable = DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
            if (dsTable != null && dsTable.Tables.Count > 0 && dsTable.Tables[0].Rows.Count > 0)
            {
                _tableId = Util.GetValueOfInt(dsTable.Tables[0].Rows[0]["AD_TABLE_ID"]);
                _processId = Util.GetValueOfInt(dsTable.Tables[0].Rows[0]["AD_PROCESS_ID"]);
            }
            else
            {
                return Msg.GetMsg(GetCtx(), "VIS_InvTableProcessNotFound");
            }
            // When process run from Service Contract Window
            if (C_Contract_ID != 0)
            {
                cont = new VAdvantage.Model.X_C_Contract(GetCtx(), C_Contract_ID, Get_TrxName());

                #region CheckDocTypeandPaymentMethod

                // If ContractType not defined on Service Contract Window / Payment Method not defind on business partner
                if (GetDocType() == 0 || !CheckPaymentMethod())
                {
                    DocPaymentCheck = false;
                }
                #endregion

                // If DocType (ContractType) and payment method on business parter found
                if (DocPaymentCheck)
                {
                    sql.Clear();
                    sql.Append("SELECT * FROM C_CONTRACTSCHEDULE WHERE C_CONTRACT_ID = " + cont.GetC_Contract_ID() + " AND NVL(C_INVOICE_ID,0) = 0 AND FROMDATE <=" + GlobalVariable.TO_DATE(DateTime.Now, true));
                    dsSch = DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                    if (dsSch != null && dsSch.Tables.Count > 0 && dsSch.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsSch.Tables[0].Rows.Count; j++)
                        {
                            // Get Contract Schedule
                            contSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), dsSch.Tables[0].Rows[j], Get_TrxName());
                            GenerateInvoice(contSchedule);
                        }
                        sql.Clear();
                        sql.Append("SELECT COUNT(C_ContractSchedule_ID) FROM C_ContractSchedule WHERE C_Contract_ID = " + C_Contract_ID + " AND NVL(C_INVOICE_ID,0) > 0");
                        string sql1 = "UPDATE C_Contract SET InvoicesGenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName())) + " WHERE C_Contract_ID = " + C_Contract_ID;
                        int res = DB.ExecuteQuery(sql1, null, Get_TrxName());
                    }
                    else
                    {
                        msgInvoiceSchNotFound.Append(cont.GetDocumentNo());
                    }
                }
            }
            else
            {
                sql.Clear();
                // Get Active/Approved Contracts whose invoice schedule not completed yet
                sql.Append("SELECT * FROM C_Contract WHERE IsActive = 'Y' AND TOTALINVOICE-INVOICESGENERATED>0 AND AD_Client_ID = " + GetAD_Client_ID() + " ORDER BY C_CONTRACT_ID");
                try
                {
                    ds = DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            DocPaymentCheck = true;
                            //Get Contract Detail
                            cont = new VAdvantage.Model.X_C_Contract(GetCtx(), ds.Tables[0].Rows[i], Get_TrxName());

                            #region CheckDocTypeandPaymentMethod
                            // If ContractType not defined on Service Contract Window / Payment Method not defind on business partner
                            if (GetDocType() == 0 || !CheckPaymentMethod())
                            {
                                //DocPaymentCheck = false;
                                continue;
                            }
                            #endregion

                            // Get Contract Schedules whose invoice not created yet based on ContractId
                            sql.Clear();
                            sql.Append("SELECT * FROM C_CONTRACTSCHEDULE WHERE C_CONTRACT_ID = " + cont.GetC_Contract_ID() + " AND NVL(C_INVOICE_ID,0) = 0 AND FROMDATE <=" + GlobalVariable.TO_DATE(DateTime.Now, true));
                            dsSch = DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                            if (dsSch != null && dsSch.Tables.Count > 0 && dsSch.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsSch.Tables[0].Rows.Count; j++)
                                {
                                    // Get Contract Schedule
                                    contSchedule = new VAdvantage.Model.X_C_ContractSchedule(GetCtx(), dsSch.Tables[0].Rows[j], Get_TrxName());
                                    GenerateInvoice(contSchedule);
                                }
                            }
                            else
                            {
                                msgInvoiceSchNotFound.Append(cont.GetDocumentNo() + ",");
                            }
                            // Update Total Invoice Generated related to Contract
                            sql.Clear();
                            sql.Append("SELECT COUNT(C_ContractSchedule_ID) FROM C_ContractSchedule WHERE C_Contract_ID = " + cont.GetC_Contract_ID() + " AND NVL(C_INVOICE_ID,0) > 0");
                            string sql1 = "UPDATE C_Contract SET InvoicesGenerated = " + Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()))
                                + " WHERE C_Contract_ID = " + cont.GetC_Contract_ID();
                            int res = DB.ExecuteQuery(sql1, null, Get_TrxName());
                        }
                    }
                    else
                    {
                        msgInvoiceNotFound.Append(Msg.GetMsg(GetCtx(), "VIS_InvoiceNotFound"));
                    }
                }
                catch (Exception ex)
                {
                    if (ds != null)
                    {
                        ds.Dispose();
                    }
                    if (dsSch != null)
                    {
                        dsSch.Dispose();
                    }
                    _Result = ex.Message;
                }
            }
            if (ds != null)
            {
                ds.Dispose();
            }
            if (dsSch != null)
            {
                dsSch.Dispose();
            }
            string lineBreak = (C_Contract_ID == 0 ? " </br> " : ", ");
            if (msgDocTypeNotDefined.Length > 0)
            {
                _Result = Msg.GetMsg(GetCtx(), "ContractTypeNotDefined") + " " + msgDocTypeNotDefined.ToString().Substring(0, msgDocTypeNotDefined.Length - 1) + lineBreak;
            }
            if (msgPaymentMethodNotDefined.Length > 0)
            {
                _Result = _Result + Msg.GetMsg(GetCtx(), "VIS_PaymentMethodNotDefined") + " " + msgPaymentMethodNotDefined.ToString().Substring(0, msgPaymentMethodNotDefined.Length - 1) + lineBreak;
            }
            if (msgInvoiceSchNotFound.Length > 0)
            {
                _Result = _Result + " " + Msg.GetMsg(GetCtx(), "VIS_NotFoundInvSchedule") + " " + msgInvoiceSchNotFound.ToString().Substring(0, msgInvoiceSchNotFound.Length - 1) + lineBreak;
            }
            if (msgInvoiceSaved.Length > 0)
            {
                _Result = _Result + " " + Msg.GetMsg(GetCtx(), "VIS_InvGenerated") + " " + msgInvoiceSaved.ToString().Substring(0, msgInvoiceSaved.Length - 1) + lineBreak;
            }
            if (msgInvoiceNotSaved.Length > 0)
            {
                _Result = _Result + " " + Msg.GetMsg(GetCtx(), "VIS_InvNotGenerated") + " " + msgInvoiceNotSaved.ToString().Substring(0, msgInvoiceNotSaved.Length - 1) + lineBreak;
            }
            if (msgInvoiceNotFound.Length > 0)
            {
                _Result = msgInvoiceNotFound.ToString();
            }
            if (string.IsNullOrEmpty(_Result))
            {
                _Result = Msg.GetMsg(GetCtx(), "VIS_InvNotGenerated");
            }
            return _Result;
        }
        /// <summary>
        /// Generate Contract Invoice
        /// </summary>
        /// <param name="contSchedule"></param>
        private void GenerateInvoice(VAdvantage.Model.X_C_ContractSchedule contSchedule)
        {
            try
            {
                if (contSchedule.IsActive())
                {
                    int res = 0;

                    Decimal? price = null;
                    if (!cont.IsCancel())
                    {
                        price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                    }
                    else
                    {
                        sql.Clear();
                        sql.Append("UPDATE C_Contract SET RenewalType = NULL WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                        int res2 = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));

                        if (contSchedule.GetEndDate() <= cont.GetCancellationDate())
                        {
                            price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());
                        }
                        else
                        {
                            sql.Clear();
                            sql.Append("SELECT MAX(C_ContractSchedule_ID) FROM C_ContractSchedule WHERE NVL(C_INVOICE_ID,0) > 0 AND C_Contract_ID = " + cont.GetC_Contract_ID());
                            int c_contractschedule_id = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                            if (c_contractschedule_id != 0)
                            {
                                string date = cont.GetCancellationDate().Value.ToString("dd-MMM-yyyy");

                                sql.Clear();
                                sql.Append("SELECT DaysBetween('" + date + "', EndDate) FROM C_ContractSchedule WHERE C_ContractSchedule_ID = " + c_contractschedule_id);
                                Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                                price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                                sql.Clear();
                                sql.Append("UPDATE C_ContractSchedule SET IsActive = 'N' WHERE EndDate > '" + date + "' AND C_Contract_ID = " + cont.GetC_Contract_ID());
                                res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                            }
                            else
                            {
                                sql.Clear();
                                sql.Append("SELECT DaysBetween(CancellationDate, StartDate) FROM C_Contract WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                                Decimal? diffDays = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

                                price = Decimal.Multiply(cont.GetPriceEntered(), cont.GetQtyEntered());

                                if (diffDays > 0)
                                {
                                    sql.Clear();
                                    sql.Append("UPDATE C_ContractSchedule SET IsActive = 'N' WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                                    res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                                }
                                else
                                {
                                    sql.Clear();
                                    sql.Append("UPDATE C_ContractSchedule SET IsActive = 'N' WHERE C_Contract_ID = " + cont.GetC_Contract_ID());
                                    res = Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                                }
                            }
                        }
                    }

                    //price = Decimal.Round(price.Value, 2, MidpointRounding.AwayFromZero);

                    inv = new VAdvantage.Model.MInvoice(GetCtx(), 0, Get_TrxName());
                    inv.SetAD_Client_ID(cont.GetAD_Client_ID());
                    inv.SetAD_Org_ID(cont.GetAD_Org_ID());
                    inv.SetC_BPartner_ID(cont.GetC_BPartner_ID());
                    if (Util.GetValueOfInt(cont.GetC_Order_ID()) != 0)
                    {
                        inv.SetC_Order_ID(cont.GetC_Order_ID());
                    }
                    // JID_0872: System has To pick the Payment Method defined with the Business Partner against whom the Invoice is getting generated.
                    if (Env.IsModuleInstalled("VA009_"))
                    {
                        // When ContractType is Accounts Receivable
                        // If Contract type not found consider as AR Invoice by default Done by Rakesh 21/May/2021 suggested by Kanika
                        if (bp.GetVA009_PaymentMethod_ID() > 0 && (string.IsNullOrEmpty(cont.GetContractType()) || cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsReceivable)))
                        {
                            inv.SetVA009_PaymentMethod_ID(bp.GetVA009_PaymentMethod_ID());
                            inv.SetIsSOTrx(true);
                        }
                        else if (bp.GetVA009_PO_PaymentMethod_ID() > 0 && (cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsPayable)))
                        {
                            // When ContractType is Accounts Payable
                            inv.SetVA009_PaymentMethod_ID(bp.GetVA009_PO_PaymentMethod_ID());
                            inv.SetIsSOTrx(false);
                        }
                    }
                    // Get count contract schedule of invoice
                    sql.Clear();
                    sql.Append("SELECT COUNT(C_ContractSchedule_ID)+1 From C_ContractSchedule Where NVL(C_INVOICE_ID,0) > 0 AND C_Contract_ID=" + cont.GetC_Contract_ID());
                    int scheduledInvoiceCount = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));
                    inv.Set_Value("InvoiceReference", cont.GetDocumentNo() + "_" + Util.GetValueOfString(scheduledInvoiceCount));
                    inv.SetC_DocType_ID(_C_DocType_ID);
                    inv.SetC_DocTypeTarget_ID(_C_DocType_ID);
                    inv.SetC_BPartner_Location_ID(cont.GetBill_Location_ID());
                    inv.SetC_Currency_ID(cont.GetC_Currency_ID());
                    // JID_1536_3
                    inv.SetC_ConversionType_ID(cont.GetC_ConversionType_ID());
                    inv.SetC_PaymentTerm_ID(cont.GetC_PaymentTerm_ID());
                    inv.SetC_Campaign_ID(cont.GetC_Campaign_ID());

                    inv.SetM_PriceList_ID(cont.GetM_PriceList_ID());
                    inv.SetSalesRep_ID(cont.GetSalesRep_ID());
                    inv.SetC_Contract_ID(cont.GetC_Contract_ID());
                    // When invoice created from service contract set ServiceContract always true
                    inv.SetServiceContract(true);
                    if (!inv.Save(Get_TrxName()))
                    {
                        Get_TrxName().Rollback();
                        msgInvoiceNotSaved.Append(cont.GetDocumentNo() + "_" + contSchedule.GetFROMDATE().Value.ToString("ddMMMyyyy") + ",");
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            msg = pp.GetName();
                            //if GetName is Empty then it will check GetValue
                            if (string.IsNullOrEmpty(msg))
                                msg = Msg.GetMsg("", pp.GetValue());
                        }
                        // save error log info file
                        log.Info("Invoice not saved DocumentNo: " + cont.GetDocumentNo() + " - Schedule From Date: " + contSchedule.GetFROMDATE().Value.ToString("dd-MMM-yyyy") + " Error: " + msg);
                    }
                    else
                    {
                        VAdvantage.Model.MInvoiceLine invLine = new VAdvantage.Model.MInvoiceLine(GetCtx(), 0, Get_TrxName());
                        invLine.SetInvoice(inv);
                        invLine.SetAD_Client_ID(inv.GetAD_Client_ID());
                        invLine.SetAD_Org_ID(inv.GetAD_Org_ID());
                        invLine.SetC_Campaign_ID(inv.GetC_Campaign_ID());
                        invLine.SetC_Invoice_ID(inv.GetC_Invoice_ID());
                        invLine.SetC_UOM_ID(cont.GetC_UOM_ID());
                        invLine.SetM_Product_ID(cont.GetM_Product_ID());
                        invLine.SetM_AttributeSetInstance_ID(cont.GetM_AttributeSetInstance_ID());
                        if (Util.GetValueOfInt(cont.GetC_OrderLine_ID()) != 0)
                        {
                            invLine.SetC_OrderLine_ID(cont.GetC_OrderLine_ID());
                        }
                        invLine.SetC_Tax_ID(cont.GetC_Tax_ID());
                        invLine.SetQty(cont.GetQtyEntered());
                        invLine.SetQtyEntered(cont.GetQtyEntered());
                        invLine.SetPriceActual(cont.GetPriceEntered());
                        invLine.SetPriceEntered(cont.GetPriceEntered());
                        invLine.SetPriceList(cont.GetPriceList());
                        if (!invLine.Save(Get_TrxName()))
                        {
                            Get_TrxName().Rollback();
                            msgInvoiceNotSaved.Append(cont.GetDocumentNo() + "_" + contSchedule.GetFROMDATE().Value.ToString("ddMMMyyyy") + ",");
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                msg = pp.GetName();
                                //if GetName is Empty then it will check GetValue
                                if (string.IsNullOrEmpty(msg))
                                    msg = Msg.GetMsg("", pp.GetValue());
                            }
                            // save error log info file
                            log.Info("Invoice Line not saved DocumentNo: " + cont.GetDocumentNo() + " - Schedule From Date: " + contSchedule.GetFROMDATE().Value.ToString("dd-MMM-yyyy") + " Error: " + msg);
                        }
                        else
                        {
                            //Set InvoiceId Created against Contract Schedule
                            sql.Clear();
                            sql.Append("UPDATE C_ContractSchedule SET C_Invoice_ID = " + inv.GetC_Invoice_ID() + ", Processed = 'Y' WHERE C_ContractSchedule_ID = " + contSchedule.GetC_ContractSchedule_ID());
                            Util.GetValueOfInt(DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
                            msgInvoiceSaved.Append(inv.GetDocumentNo() + ",");
                            //Commit Transaction to execute workflow associated
                            Get_TrxName().Commit();

                            //Complete Invoice and Generate Invoice Payment
                            string msg = CompleteInvoicePayment(GetCtx(), inv.GetC_Invoice_ID(), _processId, MInvoice.DOCACTION_Complete, cont.GetC_Contract_ID());
                            if (!string.IsNullOrEmpty(msg))
                            {
                                log.Info("Workflow not completed: " + msg);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
            }
        }
        /// <summary>
        /// Created By: Rakesh Kumar (VA228)
        /// Created Date: 07/June/2021
        /// Purpose: Get DocType
        /// </summary>
        private int GetDocType()
        {
            _C_DocType_ID = 0;
            sql.Clear();
            sql.Append("SELECT C_DocType_ID FROM C_DocType "
                      + "WHERE AD_Client_ID=" + cont.GetAD_Client_ID() + " AND IsActive='Y' AND AD_Org_ID IN(0," + cont.GetAD_Org_ID() + ") ");
            // If Contract type not found consider as AR Invoice by default
            // When ContractType is Accounts Receivable
            if (string.IsNullOrEmpty(cont.GetContractType()) || cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsReceivable))
            {
                sql.Append(@" AND DocBaseType = '" + MDocBaseType.DOCBASETYPE_ARINVOICE + @"'");
            }
            else if (cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsPayable))
            {
                sql.Append(@" AND DocBaseType = '" + MDocBaseType.DOCBASETYPE_APINVOICE + @"'");
            }

            sql.Append(" ORDER BY AD_Org_ID Desc");
            _C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_TrxName()));

            // Get DocTypeId
            // If ContractType not defined on Service Contract Window
            if (_C_DocType_ID == 0)
            {
                // save error log info file
                log.Info("Create Contract Invoice - Doc Basetype not defind or found: " + cont.GetDocumentNo());
                msgDocTypeNotDefined.Append(cont.GetDocumentNo() + ",");
            }
            return _C_DocType_ID;
        }
        /// <summary>
        /// Created By: Rakesh Kumar (VA228)
        /// Created Date: 07/June/2021
        /// Check payment method for Business Partner selected on Service Contract
        /// </summary>
        /// <returns>true/false</returns>
        private bool CheckPaymentMethod()
        {
            bool result = true;
            // Get business partner detail
            bp = new MBPartner(GetCtx(), cont.GetC_BPartner_ID(), Get_TrxName());
            // JID_0872: System has To pick the Payment Method defined with the Business Partner against whom the Invoice is getting generated.
            if (Env.IsModuleInstalled("VA009_"))
            {
                // When ContractType is Accounts Receivable
                // If Contract type not found consider as AR Invoice by default Done by Rakesh 21/May/2021 suggested by Kanika
                if (bp.GetVA009_PaymentMethod_ID() > 0 && (string.IsNullOrEmpty(cont.GetContractType()) || cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsReceivable)))
                {
                    result = true;
                }
                else if (bp.GetVA009_PO_PaymentMethod_ID() > 0 && (cont.GetContractType().Equals(X_C_Contract.CONTRACTTYPE_AccountsPayable)))
                {
                    result = true;
                }
                else
                {
                    msgPaymentMethodNotDefined.Append(cont.GetDocumentNo() + "/" + bp.GetName() + ",");
                    log.Info("Create Contract Invoice - Payment method not defind or found: " + cont.GetDocumentNo());
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// Complete the Invoice Payment when call this function to complete the record
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="Record_ID">C_Payment_ID</param>
        /// <param name="Process_ID">AD_Process_ID</param>
        /// <param name="DocAction">Documnet Action</param>
        /// <param name="contractId">Service Contract Id</param>
        /// <returns>return message</returns>
        public string CompleteInvoicePayment(Ctx ctx, int Record_ID, int Process_ID, string DocAction, int contractId)
        {
            string result = "";
            MRole role = MRole.Get(ctx, ctx.GetAD_Role_ID());
            if (Util.GetValueOfBool(role.GetProcessAccess(Process_ID)))
            {
                sql.Clear();
                sql.Append("UPDATE C_Invoice SET DocAction = '" + DocAction + "',C_Contract_ID=" + contractId + " WHERE C_Invoice_ID = " + Record_ID);
                DB.ExecuteQuery(sql.ToString());

                MProcess proc = new MProcess(ctx, Process_ID, null);
                MPInstance pin = new MPInstance(proc, Record_ID);
                if (!pin.Save())
                {
                    ValueNamePair vnp = VLogger.RetrieveError();
                    string errorMsg = "";
                    if (vnp != null)
                    {
                        errorMsg = vnp.GetName();
                        if (errorMsg == "")
                            errorMsg = vnp.GetValue();
                    }
                    if (errorMsg == "")
                        result = "DocNotCompleted";

                    log.Info("Document not completed:" + result);
                    return result;
                }

                MPInstancePara para = new MPInstancePara(pin, 20);
                para.setParameter("DocAction", DocAction);
                if (!para.Save())
                {
                    //String msg = "No DocAction Parameter added"; // not translated
                }
                VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo("WF", Process_ID);
                pi.SetAD_User_ID(ctx.GetAD_User_ID());
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                pi.SetAD_PInstance_ID(pin.GetAD_PInstance_ID());
                pi.SetRecord_ID(Record_ID);
                pi.SetTable_ID(_tableId); //AD_Table_ID=318 for C_Invoice

                ProcessCtl worker = new ProcessCtl(ctx, null, pi, null);
                worker.Run();

                if (pi.IsError())
                {
                    ValueNamePair vnp = VLogger.RetrieveError();
                    string errorMsg = "";
                    if (vnp != null)
                    {
                        errorMsg = vnp.GetName();
                        if (errorMsg == "")
                            errorMsg = vnp.GetValue();
                    }

                    if (errorMsg == "")
                        errorMsg = pi.GetSummary();

                    if (errorMsg == "")
                        errorMsg = "DocNotCompleted";
                    result = errorMsg;
                    log.Info("Document not completed:" + result);
                    return result;
                }
                else
                    result = "";
            }
            else
            {
                result = "NoAccess";
            }
            return result;
        }
    }
}
