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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BankStatementPayment : ProcessEngine.SvrProcess
    {

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
                ibs.GetC_BankAccount_ID(),Utility.Util.GetValueOfDateTime(ibs.GetStatementLineDate() == null ? ibs.GetStatementDate() : ibs.GetStatementLineDate()),
                Utility.Util.GetValueOfDateTime(ibs.GetDateAcct()), ibs.GetDescription(), ibs.GetAD_Org_ID());
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
            MPayment payment = CreatePayment(bsl.GetC_Invoice_ID(), bsl.GetC_BPartner_ID(),
                bsl.GetC_Currency_ID(), bsl.GetStmtAmt(), bsl.GetTrxAmt(),
                bs.GetC_BankAccount_ID(), bsl.GetStatementLineDate(), bsl.GetDateAcct(),
                bsl.GetDescription(), bsl.GetAD_Org_ID());
            if (payment == null)
            {
                throw new SystemException("Could not create Payment");
            }
            //	update statement
            bsl.SetPayment(payment);
            bsl.Save();
            //
            String retString = "@C_Payment_ID@ = " + payment.GetDocumentNo();
            if (Env.Signum(payment.GetOverUnderAmt()) != 0)
            {
                retString += " - @OverUnderAmt@=" + payment.GetOverUnderAmt();
            }
            return retString;
        }


        /// <summary>
        /// Create actual Payment
        /// </summary>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="C_BPartner_ID">partner ignored when invoice exists</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="StmtAmt">statement amount</param>
        /// <param name="TrxAmt">transaction amt</param>
        /// <param name="C_BankAccount_ID">bank account</param>
        /// <param name="DateTrx">transaction date</param>
        /// <param name="DateAcct">accounting date</param>
        /// <param name="Description">description</param>
        /// <param name="AD_Org_ID"></param>
        /// <returns>payment</returns>
        private MPayment CreatePayment(int C_Invoice_ID, int C_BPartner_ID,
            int C_Currency_ID, Decimal stmtAmt, Decimal trxAmt,
            int C_BankAccount_ID, DateTime? dateTrx, DateTime? dateAcct,
            String description, int AD_Org_ID)
        {
            //	Trx Amount = Payment overwrites Statement Amount if defined
            Decimal payAmt = trxAmt;
            if ( Env.ZERO.CompareTo(payAmt) == 0)
            {
                payAmt = stmtAmt;
            }
            if (C_Invoice_ID == 0 && ( Env.ZERO.CompareTo(payAmt) == 0))
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
            payment.SetTenderType(MPayment.TENDERTYPE_Check);
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
            //
            if (C_Invoice_ID != 0)
            {
                MInvoice invoice = new MInvoice(GetCtx(), C_Invoice_ID, null);
                payment.SetC_DocType_ID(invoice.IsSOTrx());		//	Receipt
                payment.SetC_Invoice_ID(invoice.GetC_Invoice_ID());
                payment.SetC_BPartner_ID(invoice.GetC_BPartner_ID());
                if (Env.Signum(payAmt) != 0)	//	explicit Amount
                {
                    payment.SetC_Currency_ID(C_Currency_ID);
                    if (invoice.IsSOTrx())
                    {
                        payment.SetPayAmt(payAmt);
                    }
                    else	//	payment is likely to be negative
                    {
                        payment.SetPayAmt(Decimal.Negate(payAmt));
                    }
                    payment.SetOverUnderAmt(Decimal.Subtract(invoice.GetGrandTotal(true), payment.GetPayAmt()));
                }
                else	// set Pay Amout from Invoice
                {
                    payment.SetC_Currency_ID(invoice.GetC_Currency_ID());
                    payment.SetPayAmt(invoice.GetGrandTotal(true));
                }
            }
            else if (C_BPartner_ID != 0)
            {
                payment.SetC_BPartner_ID(C_BPartner_ID);
                payment.SetC_Currency_ID(C_Currency_ID);
                if (Env.Signum(payAmt) < 0)	//	Payment
                {
                    payment.SetPayAmt(Math.Abs(payAmt));
                    payment.SetC_DocType_ID(false);
                }
                else	//	Receipt
                {
                    payment.SetPayAmt(payAmt);
                    payment.SetC_DocType_ID(true);
                }
            }
            else
            {
                return null;
            }
            payment.Save();
            //
            payment.ProcessIt(MPayment.DOCACTION_Complete);
            payment.Save();
            return payment;
        }
    }
}
