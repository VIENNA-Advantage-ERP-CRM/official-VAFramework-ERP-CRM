/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : BankStatementPayment
    * Purpose        : Create Payment from Bank Statement Info
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Raghunandan     26-Nov-2009
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
using System.IO;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class BankStatementPayment : ProcessEngine.SvrProcess
    {
        //Used to return error message while Creating Payment
        private string _message;
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
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message </returns>
        protected override String DoIt()
        {
            int Table_ID = GetTable_ID();
            int Record_ID = GetRecord_ID();
            log.Info("Table_ID=" + Table_ID + ", Record_ID=" + Record_ID);

            if (Table_ID == X_I_BankStatement.Table_ID)
            {
                return CreatePayment(new X_I_BankStatement(GetCtx(), Record_ID, Get_Trx()));
            }
            else if (Table_ID == MBankStatementLine.Table_ID)
            {
                return CreatePayment(new MBankStatementLine(GetCtx(), Record_ID, Get_Trx()));
            }

            throw new Exception("??");
        }

        /// <summary>
        /// Create Payment for Import
        /// </summary>
        /// <param name="ibs">import bank statement</param>
        /// <returns></returns>
        private String CreatePayment(X_I_BankStatement ibs)
        {
            if (ibs == null || ibs.GetC_Payment_ID() != 0)
            {
                return "--";
            }
            log.Fine(ibs.ToString());
            if (ibs.GetC_Invoice_ID() == 0 && ibs.GetC_BPartner_ID() == 0)
            {
                throw new Exception("@NotFound@ @C_Invoice_ID@ / @C_BPartner_ID@");
            }
            if (ibs.GetC_BankAccount_ID() == 0)
            {
                throw new Exception("@NotFound@ @C_BankAccount_ID@");
            }
            //
            MPayment payment = CreatePayment(ibs.GetC_Invoice_ID(), ibs.GetC_BPartner_ID(),
                ibs.GetC_Currency_ID(), ibs.GetStmtAmt(), ibs.GetTrxAmt(),
                ibs.GetC_BankAccount_ID(), Utility.Util.GetValueOfDateTime(ibs.GetStatementLineDate() == null ? ibs.GetStatementDate() : ibs.GetStatementLineDate()),
                Utility.Util.GetValueOfDateTime(ibs.GetDateAcct()), ibs.GetDescription(), ibs.GetAD_Org_ID(), 0, 0, 0, null, null, null); //Used Zero's as parameters to Avoid throw Error
            if (payment == null)
            {
                throw new SystemException("Could not create Payment");
            }

            ibs.SetC_Payment_ID(payment.GetC_Payment_ID());
            ibs.SetC_Currency_ID(payment.GetC_Currency_ID());
            ibs.SetTrxAmt(payment.GetPayAmt());
            ibs.Save();
            //
            String retString = "@C_Payment_ID@ = " + payment.GetDocumentNo();
            if (Env.Signum(payment.GetOverUnderAmt()) != 0)
            {
                retString += " - @OverUnderAmt@=" + payment.GetOverUnderAmt();
            }
            return retString;
        }

        /// <summary>
        /// 	Create Payment for BankStatement
        /// </summary>
        /// <param name="bsl">bank statement Line</param>
        /// <returns>Message</returns>
        private String CreatePayment(MBankStatementLine bsl)
        {
            if (bsl == null || bsl.GetC_Payment_ID() != 0)
            {
                return "--";
            }
            log.Fine(bsl.ToString());
            if (bsl.GetC_Invoice_ID() == 0 && bsl.GetC_BPartner_ID() == 0)
            {
                throw new Exception("@NotFound@ @C_Invoice_ID@ / @C_BPartner_ID@");
            }
            //
            MBankStatement bs = new MBankStatement(GetCtx(), bsl.GetC_BankStatement_ID(), Get_Trx());
            //
            //Check the Column Exists or not then Pass the Colums as parameters for CreatePayment method
            int conversionType = bsl.Get_ColumnIndex("C_ConversionType_ID") > 0 ? bsl.Get_ValueAsInt("C_ConversionType_ID") : 0;
            int _order_Id = bsl.Get_ColumnIndex("C_Order_ID") > 0 ? bsl.Get_ValueAsInt("C_Order_ID") : 0;
            //Get CheckDate and TenderType from Bank Statement Line
            //string checkNo = bsl.Get_ColumnIndex("CheckNo") > 0 ? bsl.GetValueAsString("CheckNo") : null;
            DateTime? checkDate = bsl.GetEftValutaDate() != null ? bsl.GetEftValutaDate() : bsl.GetDateAcct();
            string tenderType = bsl.Get_ColumnIndex("TenderType") > 0 ? bsl.GetValueAsString("TenderType") : null;

            MPayment payment = CreatePayment(bsl.GetC_Invoice_ID(), bsl.GetC_BPartner_ID(),
                bsl.GetC_Currency_ID(), bsl.GetStmtAmt(), bsl.GetTrxAmt(),
                bs.GetC_BankAccount_ID(), bsl.GetStatementLineDate(), bsl.GetDateAcct(),
                bsl.GetDescription(), bsl.GetAD_Org_ID(), conversionType, _order_Id, bsl.GetVA009_PaymentMethod_ID(), bsl.GetEftCheckNo(), checkDate, tenderType);

            if (payment == null && !string.IsNullOrEmpty(_message))
            {
                return _message;
            }
            //Update Bank StatementLine
            //Created new Object for Payment to get Updated Payment Record
            MPayment mPayment = new MPayment(GetCtx(), payment.GetC_Payment_ID(), payment.Get_Trx());
            _message = bsl.SetPayments(mPayment);
            if (string.IsNullOrEmpty(_message))
            {
                if (!bsl.Save(Get_Trx()))
                {
                    Get_Trx().Rollback();
                    ValueNamePair pp = VLogger.RetrieveError();
                    //to get Exact Error Message first get Value from GetName() else GetValue()
                    string error = pp != null ? pp.GetName() : "";
                    if (string.IsNullOrEmpty(error))
                    {
                        error = pp != null ? pp.GetValue() : "";
                        if (string.IsNullOrEmpty(error))
                        {
                            error = pp != null ? pp.ToString() : "";
                        }
                    }
                    return !string.IsNullOrEmpty(error) ? error : "DatanotSaved";
                }
                else
                {
                    String retString = "@C_Payment_ID@ = " + mPayment.GetDocumentNo();
                    if (Env.Signum(payment.GetOverUnderAmt()) != 0)
                    {
                        retString += " - @OverUnderAmt@=" + mPayment.GetOverUnderAmt();
                    }
                    Get_Trx().Commit();
                    return retString;
                }

            }
            else
            {
                Get_Trx().Rollback();
                return _message;
            }
        }


        /// <summary>
        /// Create actual Payment
        /// </summary>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="C_BPartner_ID">partner ignored when invoice exists</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="stmtAmt">statement amount</param>
        /// <param name="trxAmt">transaction amt</param>
        /// <param name="C_BankAccount_ID">bank account</param>
        /// <param name="dateTrx">transaction date</param>
        /// <param name="dateAcct">accounting date</param>
        /// <param name="description">description</param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="C_ConversionType_ID">C_ConversionType_ID</param>
        /// <param name="C_Order_ID">C_Order_ID</param>
        /// <param name="_paymentMethod">VA009_PaymentMethod_ID</param>
        /// <param name="_checkNo">Cheque No</param>
        /// <param name="_checkDate">Cheque Date</param>
        /// <param name="_tenderType">Tender Type</param>
        /// <returns>payment</returns>
        private MPayment CreatePayment(int C_Invoice_ID, int C_BPartner_ID,
            int C_Currency_ID, Decimal stmtAmt, Decimal trxAmt,
            int C_BankAccount_ID, DateTime? dateTrx, DateTime? dateAcct,
            String description, int AD_Org_ID, int C_ConversionType_ID, int C_Order_ID, int _paymentMethod, string _checkNo, DateTime? _checkDate, string _tenderType)
        {
            //	Trx Amount = Payment overwrites Statement Amount if defined
            Decimal payAmt = trxAmt;
            if (Env.ZERO.CompareTo(payAmt) == 0)
            {
                payAmt = stmtAmt;
            }
            if (C_Invoice_ID == 0 && C_Order_ID == 0 && (Env.ZERO.CompareTo(payAmt) == 0))
            {
                throw new Exception("@PayAmt@ = 0");
            }
            //if (payAmt == null)
            //{
            //    payAmt = Env.ZERO;
            //}
            //
            MPayment payment = new MPayment(GetCtx(), 0, Get_Trx());
            payment.SetAD_Org_ID(AD_Org_ID);
            payment.SetC_BankAccount_ID(C_BankAccount_ID);
            //payment.SetTenderType(MPayment.TENDERTYPE_Check);//not required it will update on MPayment class
            //Get the C_ConversionType_ID from BankStatementLine and Set C_ConversionType_ID for Payment 
            payment.SetC_ConversionType_ID(C_ConversionType_ID);
            if (dateTrx != null)
            {
                payment.SetDateTrx(dateTrx);
            }
            else if (dateAcct != null)
            {
                payment.SetDateTrx(dateAcct);
            }
            if (dateAcct != null)
            {
                payment.SetDateAcct(dateAcct);
            }
            else
            {
                payment.SetDateAcct(payment.GetDateTrx());
            }
            payment.SetDescription(description);
            //Rakesh:Override autocheckno. with eftcheck number if exists
            if (!string.IsNullOrEmpty(_checkNo))
            {
                payment.SetIsOverrideAutoCheck(true);
            }
            //
            if (C_Invoice_ID != 0)
            {
                MInvoice invoice = new MInvoice(GetCtx(), C_Invoice_ID, Get_Trx());//Used Trx
                payment.SetC_DocType_ID(invoice.IsSOTrx());		//	Receipt
                payment.SetC_BPartner_ID(invoice.GetC_BPartner_ID());
                //set the BPartner Location from the Invoice
                payment.SetC_BPartner_Location_ID(invoice.GetC_BPartner_Location_ID());
                //set the PaymentMethod by the reference of Invoice
                //payment.SetVA009_PaymentMethod_ID(invoice.GetVA009_PaymentMethod_ID());
                //get PaymentMethod,CheckNo,TenderType and checkDate from the BankStatement Line and Set to Payment
                payment.SetVA009_PaymentMethod_ID(_paymentMethod);
                if (!string.IsNullOrEmpty(_checkNo))
                {
                    payment.SetCheckNo(_checkNo);
                }
                if (!string.IsNullOrEmpty(_checkNo) && _checkDate.HasValue)
                {
                    payment.SetCheckDate(_checkDate);
                }

                payment.SetTenderType(_tenderType);
                payment.SetC_Currency_ID(invoice.GetC_Currency_ID());
                decimal _dueAmt = 0;

                string _sql = "SELECT * FROM C_InvoicePaySchedule WHERE VA009_PAIDAMNT IS NULL AND VA009_IsPaid = 'N' AND IsActive = 'Y' AND C_Invoice_ID =" + invoice.GetC_Invoice_ID();
                DataSet _ds = DB.ExecuteDataset(_sql, null, Get_Trx());

                if (_ds != null && _ds.Tables[0].Rows.Count == 1)
                {
                    payment.SetC_Invoice_ID(invoice.GetC_Invoice_ID());//set Invoice Reference
                    payment.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_INVOICEPAYSCHEDULE_ID"]));
                    _dueAmt = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["DUEAMT"]);
                    //Set PayAmt -ve sign Incase of DocBaseType APC or ARC - IsReturnTrx is true
                    //Set PayAmt +ve sign Incase of DocBaseType API or ARI - IsReturnTrx is false
                    if (!invoice.IsReturnTrx())
                    {
                        payment.SetPayAmt(_dueAmt);
                    }
                    else    //	payment is likely to be negative
                    {
                        payment.SetPayAmt(Decimal.Negate(_dueAmt));
                    }
                    if (!payment.Save(Get_Trx()))
                    {
                        Get_Trx().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        //to get Exact Error Message first get Value from GetName() else GetValue()
                        string error = pp != null ? pp.GetName() : "";
                        if (string.IsNullOrEmpty(error))
                        {
                            error = pp != null ? pp.GetValue() : "";
                            if (string.IsNullOrEmpty(error))
                            {
                                error = pp != null ? pp.ToString() : "";
                            }
                        }
                        _message = !string.IsNullOrEmpty(error) ? error : "VA012_PaymentNotSaved";
                        return null;
                    }
                }
                else if (_ds != null && _ds.Tables[0].Rows.Count > 1)
                {
                    if (!payment.Save(Get_Trx()))
                    {
                        Get_Trx().Rollback();
                        ValueNamePair pp = VLogger.RetrieveError();
                        //to get Exact Error Message first get Value from GetName() else GetValue()
                        string error = pp != null ? pp.GetName() : "";
                        if (string.IsNullOrEmpty(error))
                        {
                            error = pp != null ? pp.GetValue() : "";
                            if (string.IsNullOrEmpty(error))
                            {
                                error = pp != null ? pp.ToString() : "";
                            }
                        }
                        _message = !string.IsNullOrEmpty(error) ? error : "VA012_PaymentNotSaved";
                        return null;
                    }
                    else
                    {
                        //Initialize the Object for MPaymentAllocate class
                        MPaymentAllocate PayAlocate = null;
                        for (int i = 0; _ds.Tables[0].Rows.Count > i; i++)
                        {
                            //Create the Object for MPaymentAllocate class for every Iteration
                            PayAlocate = new MPaymentAllocate(GetCtx(), 0, Get_Trx());
                            PayAlocate.SetC_Payment_ID(payment.GetC_Payment_ID());
                            PayAlocate.SetAD_Client_ID(payment.GetAD_Client_ID());
                            //set Organization with the reference of Bank Account
                            PayAlocate.SetAD_Org_ID(payment.GetAD_Org_ID());//set Org_ID from the Header 
                            PayAlocate.SetC_Invoice_ID(invoice.GetC_Invoice_ID());
                            PayAlocate.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[i]["C_INVOICEPAYSCHEDULE_ID"]));

                            //Set PayAmt -ve sign Incase of DocBaseType APC or ARC - IsReturnTrx is true
                            //Set PayAmt +ve sign Incase of DocBaseType API or ARI - IsReturnTrx is false
                            if (!invoice.IsReturnTrx())
                            {
                                PayAlocate.SetInvoiceAmt(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["DUEAMT"]));
                                PayAlocate.SetAmount(Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["DUEAMT"]));
                            }
                            else
                            {
                                PayAlocate.SetInvoiceAmt(-1 * Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["DUEAMT"]));
                                PayAlocate.SetAmount(-1 * Util.GetValueOfDecimal(_ds.Tables[0].Rows[i]["DUEAMT"]));
                            }
                            if (!PayAlocate.Save(Get_Trx()))
                            {
                                Get_Trx().Rollback();
                                ValueNamePair pp = VLogger.RetrieveError();
                                //to get Exact Error Message first get Value from GetName() else GetValue()
                                string error = pp != null ? pp.GetName() : "";
                                if (string.IsNullOrEmpty(error))
                                {
                                    error = pp != null ? pp.GetValue() : "";
                                    if (string.IsNullOrEmpty(error))
                                    {
                                        error = pp != null ? pp.ToString() : "";
                                    }
                                }
                                _message = !string.IsNullOrEmpty(error) ? error : "VA012_PaymentNotSaved";
                                return null;
                            }
                        }
                    }
                }
            }
            else if (C_Order_ID > 0 && C_BPartner_ID > 0) // Create Payment for prePayOrder reference
            {
                MOrder order = new MOrder(GetCtx(), C_Order_ID, Get_Trx());
                payment.SetC_Order_ID(C_Order_ID);
                payment.SetC_DocType_ID(order.IsSOTrx()); //	Receipt
                payment.SetC_BPartner_ID(order.GetC_BPartner_ID());

                payment.SetC_BPartner_Location_ID(order.GetC_BPartner_Location_ID());
                payment.SetC_Currency_ID(order.GetC_Currency_ID());
                //payment.SetVA009_PaymentMethod_ID(order.GetVA009_PaymentMethod_ID());
                //get PaymentMethod from the BankStatement Line
                //get PaymentMethod,CheckNo, tenderType and checkDate from the BankStatement Line and Set to Payment
                payment.SetVA009_PaymentMethod_ID(_paymentMethod);
                if (!string.IsNullOrEmpty(_checkNo))
                {
                    payment.SetCheckNo(_checkNo);
                }
                if (!string.IsNullOrEmpty(_checkNo) && _checkDate.HasValue)
                {
                    payment.SetCheckDate(_checkDate);
                }
                payment.SetTenderType(_tenderType);
                payment.SetPayAmt(order.GetGrandTotal());
                if (!payment.Save(Get_Trx()))
                {
                    Get_Trx().Rollback();
                    ValueNamePair pp = VLogger.RetrieveError();
                    //to get Exact Error Message first get Value from GetName() else GetValue()
                    string error = pp != null ? pp.GetName() : "";
                    if (string.IsNullOrEmpty(error))
                    {
                        error = pp != null ? pp.GetValue() : "";
                        if (string.IsNullOrEmpty(error))
                        {
                            error = pp != null ? pp.ToString() : "";
                        }
                    }
                    _message = !string.IsNullOrEmpty(error) ? error : "VA012_PaymentNotSaved";
                    return null;
                }
            }
            else
            {
                _message = "VA012_ReferenceNotfoundtoCreatePayment";
                return null;
            }
            
            //Commit the Transaction
            Get_Trx().Commit();
            //Call Complete Method to Complete the Record
            //149 is AD_Process_ID for C_Payment_Process
            _message = CompletePayment(GetCtx(), payment.GetC_Payment_ID(), 149, MPayment.DOCACTION_Complete);
            if (!string.IsNullOrEmpty(_message))
            {
                return null;
            }
            //If payment type is cheque
            if (!string.IsNullOrEmpty(_tenderType) && _tenderType.Equals(X_C_Payment.TENDERTYPE_Check))
            {
                //If no check defined on bank stateline update from payment
                if (string.IsNullOrEmpty(_checkNo))
                {
                    //Rakesh(VA228):Get and Update reference of checkno and check date on bank statement line
                    DataSet ds = DB.ExecuteDataset("SELECT CheckNo,CheckDate FROM C_Payment WHERE C_Payment_ID=" + payment.GetC_Payment_ID());
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DB.ExecuteQuery("UPDATE C_BankStatementLine SET EftCheckNo = '" + Util.GetValueOfString(ds.Tables[0].Rows[0]["CheckNo"]) + "', EftValutaDate=" + GlobalVariable.TO_DATE(Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["CheckDate"]), true) + " WHERE C_BankStatementLine_ID = " + GetRecord_ID());
                    }
                }
            }
            return payment;
        }

        /// <summary>
        /// Complete the Payment Record
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="Record_ID">C_Payment_ID</param>
        /// <param name="Process_ID">AD_Process_ID</param>
        /// <param name="DocAction">Documnet Action</param>
        /// <returns>return message</returns>
        public string CompletePayment(Ctx ctx, int Record_ID, int Process_ID, string DocAction)
        {
            string result = "";
            MRole role = MRole.Get(ctx, ctx.GetAD_Role_ID());
            if (Util.GetValueOfBool(role.GetProcessAccess(Process_ID)))
            {
                DB.ExecuteQuery("UPDATE C_Payment SET DocAction = '" + DocAction + "' WHERE C_Payment_ID = " + Record_ID);

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
                        result = Msg.GetMsg(ctx, "DocNotCompleted");

                    return result;
                }

                MPInstancePara para = new MPInstancePara(pin, 20);
                para.setParameter("DocAction", DocAction);
                if (!para.Save())
                {
                    //String msg = "No DocAction Parameter added"; // not translated
                }
                ProcessInfo pi = new ProcessInfo("WF", Process_ID);
                pi.SetAD_User_ID(ctx.GetAD_User_ID());
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                pi.SetAD_PInstance_ID(pin.GetAD_PInstance_ID());
                pi.SetRecord_ID(Record_ID);
                pi.SetTable_ID(335); //AD_Table_ID=335 for C_Payment

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
                        errorMsg = Msg.GetMsg(ctx, "DocNotCompleted");
                    result = errorMsg;
                    return result;
                }
                else
                    result = "";
            }
            else
            {
                result = Msg.GetMsg(ctx, "NoAccess");
                return result;
            }
            return result;
        }
    }
}
