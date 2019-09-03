using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class PaymentAllocation
    {
        Ctx ctx = new Ctx();

        static VLogger _log = VLogger.GetVLogger("PaymentAllocation");

        public PaymentAllocation(Ctx ctx)
        {
            this.ctx = ctx;
        }

        /// <summary>
        /// to create view allocation against cash journal line
        /// </summary>
        /// <param name="paymentData">Selected payment data</param>
        /// <param name="cashData"> Selected cash line data</param>
        /// <param name="invoiceData">Selected invoice data</param>
        /// <param name="currency">Currency ID</param>
        /// <param name="isCash"> bool Value </param>
        /// <param name="_C_BPartner_ID"> Business Partner ID </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="payment"> Payment ID </param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="appliedamt"> Applied Amount </param>
        /// <param name="discount">Discount Amount</param>
        /// <param name="writeOff">Writeoff Amount</param>
        /// <param name="open">Open Amount</param>
        /// <param name="DateAcct">Account Date</param>
        /// <param name="_CurrencyType_ID">Currency ConversionType ID</param>
        /// <param name="isInterBPartner">Inter Business Partner(Yes/No)</param>
        /// <returns>string either error or empty string</returns>
        public string SaveCashData(List<Dictionary<string, string>> paymentData, List<Dictionary<string, string>> rowsCash, List<Dictionary<string, string>> rowsInvoice, string currency,
            bool isCash, int _C_BPartner_ID, int _windowNo, string payment, DateTime DateTrx, string applied, string discount, string writeOff, string open, DateTime DateAcct, int _CurrencyType_ID, bool isInterBPartner)
        {
            //if (_noInvoices + _noCashLines == 0)
            //    return "";
            int C_Currency_ID = Convert.ToInt32(currency);
            //  fixed fields
            int AD_Client_ID = ctx.GetContextAsInt(_windowNo, "AD_Client_ID");
            int AD_Org_ID = ctx.GetContextAsInt(_windowNo, "AD_Org_ID");
            int C_BPartner_ID = _C_BPartner_ID;
            int C_Order_ID = 0;
            int C_CashLine_ID = 0;
            string msg = string.Empty;
            //Check weather dateTrx is null than set DateTrx as SystemDate
            if (DateTrx == null)
                DateTrx = DateTime.Now;

            //set the AD_Org_ID because we want to create allocation in the selected organization not in the login orgnization
            if (paymentData.Count > 0)
            {
                AD_Org_ID = Util.GetValueOfInt(paymentData[0]["Org"]);
            }
            else if (rowsCash.Count > 0)
            {
                AD_Org_ID = Util.GetValueOfInt(rowsCash[0]["Org"]);
            }
            else if (rowsInvoice.Count > 0)
            {
                AD_Org_ID = Util.GetValueOfInt(rowsInvoice[0]["Org"]);
            }
            else
            {
                //Classes.ShowMessage.Error("Org0NotAllowed", null);
                return Msg.GetMsg(ctx, "Org0NotAllowed");
            }
            //
            //  log.Config("Client=" + AD_Client_ID + ", Org=" + AD_Org_ID
            //    + ", BPartner=" + C_BPartner_ID + ", Date=" + DateTrx);

            Trx trx = Trx.Get(Trx.CreateTrxName("AL"), true);
            // trx.TrxIsolationLevel = IsolationLevel.RepeatableRead;

            msg = ValidateRecords(paymentData, "cpaymentid", false, false, trx); //Payment
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsCash, "ccashlineid", true, false, trx); //CashLine
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsInvoice, "c_invoicepayschedule_id", false, true, trx); //InvoicePaySchedule
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = string.Empty;


            /**
             * Generation of allocations:               amount/discount/writeOff
             *  - if there is one payment -- one line per invoice is generated
             *    with both the Invoice and Payment reference
             *      Pay=80  Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *    or
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#2
             *
             *  - if there are multiple payment lines -- the amounts are allocated
             *    starting with the first payment and payment
             *      Pay=60  Inv=100 Disc=10 WOff=10 =>  60/10/10    Pay#1   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  20/0/0      Pay#2   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#2   Inv#2
             *
             *  - if you apply a credit memo to an invoice
             *              Inv=10  Disc=0  WOff=0  =>  10/0/0              Inv#1
             *              Inv=-10 Disc=0  WOff=0  =>  -10/0/0             Inv#2
             *
             *  - if you want to write off a (partial) invoice without applying,
             *    enter zero in applied
             *              Inv=10  Disc=1  WOff=9  =>  0/1/9               Inv#1
             *  Issues
             *  - you cannot write-off a payment
             */

            //  CashLines - Loop and Add them to cashList/CashAmountList
            #region CashLines-Loop
            // int cRows = vdgvCashLines.RowCount;
            // IList rowsCash = vdgvCashLine.ItemsSource as IList;

            List<int> cashList = new List<int>(rowsCash.Count);
            List<Decimal> CashAmtList = new List<Decimal>(rowsCash.Count);
            Decimal cashAppliedAmt = Env.ZERO;
            MCash cashobj = null;
            for (int i = 0; i < rowsCash.Count; i++)
            {
                //  Payment line is selected
                bool boolValue = false;
                bool flag = false;
                // if (boolValue)
                {
                    //  Payment variables
                    C_CashLine_ID = Util.GetValueOfInt(rowsCash[i]["ccashlineid"]);
                    cashList.Add(C_CashLine_ID);
                    //
                    //Decimal PaymentAmt = Util.GetValueOfDecimal(((BindableObject)rowsCash[i]).GetValue(_payment));  //  Applied Payment


                    Decimal PaymentAmt = Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"]);  //  Applied Payment

                    CashAmtList.Add(PaymentAmt);
                    //
                    cashAppliedAmt = Decimal.Add(cashAppliedAmt, PaymentAmt);
                    //
                    // log.Fine("C_CashLine_ID=" + C_CashLine_ID
                    //  + " - PaymentAmt=" + PaymentAmt); // + " * " + Multiplier + " = " + PaymentAmtAbs);
                }
            }
            //log.Config("Number of Cashlines=" + cashList.Count + " - Total=" + cashAppliedAmt);
            #endregion

            //  Invoices - Loop and generate alloctions
            #region Invoice-Loop with allocation
            // int iRows = vdgvInvoice.RowCount;
            //  IList rowsInvoice = vdgvInvoice.ItemsSource as IList;
            Decimal totalAppliedAmt = Env.ZERO;

            //	Create Allocation - but don't save yet
            // allocation should be created with current date 
            MAllocationHdr alloc = new MAllocationHdr(ctx, true,	//	manual
                DateTime.Now, C_Currency_ID, ctx.GetContext("#AD_User_Name"), trx);
            alloc.SetAD_Org_ID(AD_Org_ID);
            alloc.SetDateAcct(DateAcct);// to set Account date on allocation header because posting and conversion are calculating on the basis of Date Account
            alloc.SetC_ConversionType_ID(_CurrencyType_ID); // to set Conversion Type on allocation header because posting and conversion are calculating on the basis of Conversion Type
            alloc.SetDateTrx(DateTrx);

            //	For all invoices
            int invoiceLines = 0;
            //for (int i = 0; i < rowsCash.Count; i++)
            MInvoicePaySchedule mpay = null;
            MInvoice invoice = null;
            int C_InvoicePaySchedule_ID = 0;
            bool isScheduleAllocated = false;
            for (int i = 0; i < rowsInvoice.Count; i++)
            {
                //  Invoice line is selected
                bool boolValue = false;
                bool flag = false;
                isScheduleAllocated = false;
                // if (boolValue)
                {
                    mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);

                    invoice = new MInvoice(ctx, Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]), trx);
                    invoiceLines++;
                    //  Invoice variables
                    /// int C_Invoice_ID = Util.GetValueOfInt(((BindableObject)rowsInvoice[i]).GetValue("C_INVOICE_ID"));

                    int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                    Decimal AppliedAmt = Util.GetValueOfDecimal(rowsInvoice[i][applied]);
                    //  semi-fixed fields (reset after first invoice)
                    Decimal DiscountAmt = Util.GetValueOfDecimal(rowsInvoice[i][discount]);
                    Decimal WriteOffAmt = Util.GetValueOfDecimal(rowsInvoice[i][writeOff]);
                    //	OverUnderAmt needs to be in Allocation Currency
                    Decimal OverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(rowsInvoice[i][open]),
                        Decimal.Add(AppliedAmt, Decimal.Add(DiscountAmt, WriteOffAmt)));

                    //log.Config("Invoice #" + i + " - AppliedAmt=" + AppliedAmt);// + " -> " + AppliedAbs);

                    //CashLines settelment************
                    //  loop through all payments until invoice applied
                    int noCashlines = 0;
                    MInvoicePaySchedule mpay2 = null;
                    MCashLine objCashline = null;
                    for (int j = 0; j < cashList.Count && Env.Signum(AppliedAmt) != 0; j++)
                    {
                        #region cash to invoice matching
                        mpay2 = null;
                        C_CashLine_ID = Util.GetValueOfInt(cashList[j]);
                        objCashline = new MCashLine(ctx, C_CashLine_ID, trx);

                        cashobj = new MCash(ctx, objCashline.GetC_Cash_ID(), trx);
                        Decimal PaymentAmt = Util.GetValueOfDecimal(CashAmtList[j]);

                        // check match receipt with receipt && payment with payment
                        // not payment with receipt
                        if (PaymentAmt >= 0 && AppliedAmt <= 0)
                            continue;
                        if (PaymentAmt <= 0 && AppliedAmt >= 0)
                            continue;

                        if (Env.Signum(PaymentAmt) != 0)
                        {
                            noCashlines++;
                            //  use Invoice Applied Amt
                            Decimal amount = Env.ZERO;
                            if ((Math.Abs(AppliedAmt)).CompareTo(Math.Abs(PaymentAmt)) > 0)
                            {
                                amount = PaymentAmt;
                            }
                            else
                            {
                                amount = AppliedAmt;
                            }

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return Msg.GetMsg(ctx, "Allocationnotcreated");
                            }

                            // when 
                            if (!isScheduleAllocated)
                            {
                                isScheduleAllocated = true;
                                if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                {
                                    var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(amount, OverUnderAmt), Decimal.Add(DiscountAmt, WriteOffAmt)), C_Currency_ID, invoice.GetC_Currency_ID(), cashobj.GetDateAcct(), objCashline.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                    mpay.SetDueAmt(Math.Abs(conertedAmount));
                                }
                                else
                                    mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                   Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));
                                mpay.Save(trx);
                            }
                            // Create New schedule with split 
                            else if (isScheduleAllocated)
                            {
                                mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                PO.CopyValues(mpay, mpay2);
                                //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                {
                                    var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, invoice.GetC_Currency_ID(), cashobj.GetDateAcct(), objCashline.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                    mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                }
                                else
                                    mpay2.SetDueAmt(Math.Abs(amount));
                                mpay2.Save(trx);
                            }

                            if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                            {
                                OverUnderAmt = 0;
                            }
                            C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);

                            //if (isInterBPartner) {
                            //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, C_InvoicePaySchedule_ID, trx);
                            //    C_BPartner_ID = invPay.GetC_BPartner_ID();
                            //}
                            //	Allocation Line // Changed PaymentAmt to AppliedAmt 17/4/18
                            MAllocationLine aLine = new MAllocationLine(alloc, amount,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            aLine.SetPaymentInfo(0, C_CashLine_ID); //payment for payment allocation is zero
                            if (Env.IsModuleInstalled("VA009_"))
                            {
                                if (mpay2 == null)
                                    aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                else if (mpay2 != null)
                                    aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(mpay2.GetC_InvoicePaySchedule_ID()));
                            }

                            aLine.SetDateTrx(DateTrx);

                            if (!aLine.Save())
                            {
                                _log.SaveError("Error: ", "Allocation Line not created");
                            }
                            //  Apply Discounts and WriteOff only first time
                            DiscountAmt = Env.ZERO;
                            WriteOffAmt = Env.ZERO;
                            OverUnderAmt = Env.ZERO;
                            //  subtract amount from Payment/Invoice
                            //AppliedAmt = Decimal.Subtract(AppliedAmt, amount);
                            AppliedAmt = Decimal.Subtract(AppliedAmt, amount);
                            PaymentAmt = Decimal.Subtract(PaymentAmt, amount);

                            //amountList.set(j, PaymentAmt);  //  update
                            if (CashAmtList.Count > 0)
                            {
                                // No need to set new amount at cash line
                                // Changes done by Vivek on 02/01/2018 assigned by Mukesh sir
                                //MCashLine cline = new MCashLine(ctx, C_CashLine_ID, null);
                                //cline.SetAmount(Decimal.Subtract(cline.GetAmount(), CashAmtList[j]));
                                //if (!cline.Save())
                                //{
                                // log.SaveError("AmountIsNotUpdated" + C_CashLine_ID.ToString(), "");
                                //}
                                CashAmtList[j] = PaymentAmt;  //  update//set
                            }

                        }	//	for all applied amounts
                        #endregion
                    }	//	loop through Cash for invoice(Charge)

                    //  No Cashlines allocated and none existing 
                    if (rowsCash.Count > 0)
                        if (noCashlines == 0 && cashList.Count == 0)
                        {
                            #region when match invoice to invoice
                            C_CashLine_ID = 0;
                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return Msg.GetMsg(ctx, "Allocationnotcreated");
                            }
                            //	Allocation Line
                            MAllocationLine aLine = new MAllocationLine(alloc, AppliedAmt,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            aLine.SetPaymentInfo(0, C_CashLine_ID);
                            if (Env.IsModuleInstalled("VA009_"))
                            {
                                aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                //if (isInterBPartner)
                                //{
                                //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                                //    aLine.SetC_BPartner_ID(invPay.GetC_BPartner_ID());
                                //}
                            }
                            aLine.SetDateTrx(DateTrx);
                            if (!aLine.Save())
                            {
                                _log.SaveError("Error: ", "Allocation Line not created");
                            }
                            #endregion
                        }
                        else if (AppliedAmt != 0 && cashList.Count != 0)
                        {
                            #region Invoice to invoice allocation when same matched with cash
                            // when we match invoice to invoice and invoice to cash for same schedule 
                            // then we have to create a new schedule for match invoice to invoice
                            if (noCashlines != 0)
                            {
                                mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                PO.CopyValues(mpay, mpay2);
                                //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());

                                if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                {
                                    var conertedAmount = MConversionRate.Convert(ctx, AppliedAmt, C_Currency_ID, invoice.GetC_Currency_ID(), cashobj.GetDateAcct(), objCashline.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                    mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                }
                                else
                                    mpay2.SetDueAmt(Math.Abs(AppliedAmt));
                                mpay2.Save(trx);
                            }

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return Msg.GetMsg(ctx, "Allocationnotcreated");
                            }

                            //	Allocation Line
                            MAllocationLine aLine = new MAllocationLine(alloc, AppliedAmt,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            aLine.SetPaymentInfo(0, 0);
                            if (Env.IsModuleInstalled("VA009_"))
                            {
                                if (mpay2 != null)
                                {
                                    aLine.SetC_InvoicePaySchedule_ID(mpay2.GetC_InvoicePaySchedule_ID());
                                    //if (isInterBPartner)
                                    //{
                                    //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, mpay2.GetC_InvoicePaySchedule_ID(), trx);
                                    //    aLine.SetC_BPartner_ID(invPay.GetC_BPartner_ID());
                                    //}
                                }
                                else
                                {
                                    aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                    //if (isInterBPartner)
                                    //{
                                    //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                                    //    aLine.SetC_BPartner_ID(invPay.GetC_BPartner_ID());
                                    //}
                                }
                            }
                            aLine.SetDateTrx(DateTrx);
                            if (!aLine.Save(trx))
                            {
                                _log.SaveError("Error: ", "Allocation Line not created");
                            }
                            #endregion
                        }
                    totalAppliedAmt = Decimal.Add(totalAppliedAmt, AppliedAmt);
                    //log.Config("TotalRemaining=" + totalAppliedAmt);
                }   //  invoice selected
            }   //  invoice loop

            #endregion
            // Work done to remove check  of totalappliedamt assigned by Mukesh sir on 27/12/2017
            //if (Env.Signum(totalAppliedAmt) != 0)
            //log.Log(Level.SEVERE, "Remaining TotalAppliedAmt=" + totalAppliedAmt);

            //	Should start WF
            if (alloc.Get_ID() != 0)
            {
                alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                alloc.Save();
                msg = alloc.GetDocumentNo();
            }

            //  Test/Set IsPaid for Invoice - requires that allocation is posted
            #region Set Invoice IsPaid
            for (int i = 0; i < rowsInvoice.Count; i++)
            {
                bool boolValue = false;
                //  Invoice line is selected
                bool flag = false;
                //Dispatcher.BeginInvoke(delegate
                //{
                //    boolValue = GetBoolValue(vdgvInvoice, i, 0);
                //    flag = true;
                //    SetBusy(false);
                //});
                //while (!flag)
                //{
                //    System.Threading.Thread.Sleep(1);
                //}
                // if (boolValue)
                {
                    //KeyNamePair pp = (KeyNamePair)vdgvInvoice.Rows[i].Cells[2].Value;    //  Value
                    //KeyNamePair pp = (KeyNamePair)((BindableObject)rowsInvoice[i]).GetValue(2);    //  Value
                    //  Invoice variables
                    int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                    String sql = "SELECT invoiceOpen(C_Invoice_ID, 0) "
                        + "FROM C_Invoice WHERE C_Invoice_ID=@param1";
                    Decimal opens = Util.GetValueOfDecimal(DB.GetSQLValueBD(trx, sql, C_Invoice_ID));
                    if (open != null && Env.Signum(opens) == 0)
                    {
                        sql = "UPDATE C_Invoice SET IsPaid='Y' "
                            + "WHERE C_Invoice_ID=" + C_Invoice_ID;
                        int no = DB.ExecuteQuery(sql, null, trx);
                        // log.Config("Invoice #" + i + " is paid");
                    }
                    else
                    {
                        // log.Config("Invoice #" + i + " is not paid - " + open);
                    }
                }
            }
            #endregion

            //  Test/Set CashLine is fully allocated
            #region Set CashLine Allocated
            //if (rowsCash.Count > 0)
            //    for (int i = 0; i < cashList.Count; i++)
            //    {
            //        C_CashLine_ID = Util.GetValueOfInt(cashList[i]);
            //        MCashLine cash = new MCashLine(ctx, C_CashLine_ID, trx);
            //        //if (cash.GetAmount() == 0)
            //        //{
            //        // changes done to set cash allocated true on if allocation gets completed		
            //        // Assigned by Mukesh sir on 27/12/2017		
            //        if (alloc.GetDocStatus() == "CO")
            //        {
            //            cash.SetIsAllocated(true);
            //            cash.Save();
            //        }
            //        // log.Config("Cash #" + i + (cash.IsAllocated() ? " not" : " is")
            //        //   + " fully allocated");
            //    }

            // added by vivek to set isallocated checkbox true on cashline on 06/01/2018
            if (rowsCash.Count > 0)
                for (int i = 0; i < rowsCash.Count; i++)
                {
                    int _cashine_ID = Util.GetValueOfInt(rowsCash[i]["ccashlineid"]);
                    MCashLine cash = new MCashLine(ctx, _cashine_ID, trx);

                    string sqlGetOpenPayments = "SELECT  ALLOCCASHAVAILABLE(cl.C_CashLine_ID)  FROM C_CashLine cl Where C_CashLine_ID = " + _cashine_ID;
                    object result = DB.ExecuteScalar(sqlGetOpenPayments, null, trx);
                    Decimal? amtPayment = 0;
                    if (result == null || result == DBNull.Value)
                    {
                        amtPayment = -1;
                    }
                    else
                    {
                        amtPayment = Util.GetValueOfDecimal(result);
                    }

                    if (amtPayment == 0)
                    {
                        cash.SetIsAllocated(true);
                    }
                    else
                    {
                        cash.SetIsAllocated(false);
                    }
                    cash.Save();
                    //log.Config("Payment #" + i + (pay.IsAllocated() ? " not" : " is")
                    //    + " fully allocated");
                }
            #endregion

            cashList.Clear();
            CashAmtList.Clear();
            SetIsprocessingFalse(paymentData, "cpaymentid", false, false, trx); //Payment
            SetIsprocessingFalse(rowsCash, "ccashlineid", true, false, trx); //CashLine
            SetIsprocessingFalse(rowsInvoice, "c_invoicepayschedule_id", false, true, trx); //InvoicePaySchedule
            trx.Commit();
            trx.Close();
            return Msg.GetMsg(ctx, "AllocationCreatedWith") + msg;
        }

        /// <summary>
        /// to create view allocation against Payment
        /// </summary>
        /// <param name="paymentData">Selected payment data</param>
        /// <param name="cashData"> Selected cash line data</param>
        /// <param name="invoiceData">Selected invoice data</param>
        /// <param name="currency">Currency ID</param>
        /// <param name="isCash"> bool Value </param>
        /// <param name="_C_BPartner_ID"> Business Partner ID </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="payment"> Payment ID </param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="appliedamt"> Applied Amount </param>
        /// <param name="discount">Discount Amount</param>
        /// <param name="writeOff">Writeoff Amount</param>
        /// <param name="open">Open Amount</param>
        /// <param name="DateAcct">Account Date</param>
        /// <param name="_CurrencyType_ID">Currency ConversionType ID</param>
        /// <param name="isInterBPartner">Inter Business Partner(Yes/No)</param>
        /// <returns>string either error or empty string</returns>
        public string SavePaymentData(List<Dictionary<string, string>> rowsPayment, List<Dictionary<string, string>> rowsCash, List<Dictionary<string, string>> rowsInvoice, string currency,
            bool isCash, int _C_BPartner_ID, int _windowNo, string payment, DateTime DateTrx, string applied, string discount, string writeOff, string open, DateTime DateAcct, int _CurrencyType_ID, bool isInterBPartner)
        {

            //  fixed fields
            int AD_Client_ID = ctx.GetContextAsInt(_windowNo, "AD_Client_ID");
            int AD_Org_ID = ctx.GetContextAsInt(_windowNo, "AD_Org_ID");
            int C_BPartner_ID = _C_BPartner_ID;
            int C_Order_ID = 0;
            string msg = string.Empty;
            Trx trx = Trx.Get(Trx.CreateTrxName("AL"), true);

            msg = ValidateRecords(rowsPayment, "cpaymentid", false, false, trx); //Payment
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsCash, "ccashlineid", true, false, trx); //CashLine
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsInvoice, "c_invoicepayschedule_id", false, true, trx); //InvoicePaySchedule
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = string.Empty;

            //Check weather dateTrx is null than set DateTrx as SystemDate
            if (DateTrx == null)
                DateTrx = DateTime.Now;
            //DateTime? DateTrx = Util.GetValueOfDateTime(vdtpDateField.GetValue());
            int C_Currency_ID = Convert.ToInt32(currency);
            //
            //set the AD_Org_ID because we want to create allocation in the selected organization not in the login orgnization
            if (rowsPayment.Count > 0)
            {
                AD_Org_ID = Util.GetValueOfInt(rowsPayment[0]["Org"]);
            }
            else if (rowsCash.Count > 0)
            {
                AD_Org_ID = Util.GetValueOfInt(rowsCash[0]["Org"]);
            }
            else if (rowsInvoice.Count > 0)
            {
                AD_Org_ID = Util.GetValueOfInt(rowsInvoice[0]["Org"]);
            }
            else
            {
                trx.Rollback();
                trx.Close();
                return Msg.GetMsg(ctx, "Org0NotAllowed");
            }
            //
            // log.Config("Client=" + AD_Client_ID + ", Org=" + AD_Org_ID
            //     + ", BPartner=" + C_BPartner_ID + ", Date=" + DateTrx);



            /**
             * Generation of allocations:               amount/discount/writeOff
             *  - if there is one payment -- one line per invoice is generated
             *    with both the Invoice and Payment reference
             *      Pay=80  Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *    or
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#2
             *
             *  - if there are multiple payment lines -- the amounts are allocated
             *    starting with the first payment and payment
             *      Pay=60  Inv=100 Disc=10 WOff=10 =>  60/10/10    Pay#1   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  20/0/0      Pay#2   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#2   Inv#2
             *
             *  - if you apply a credit memo to an invoice
             *              Inv=10  Disc=0  WOff=0  =>  10/0/0              Inv#1
             *              Inv=-10 Disc=0  WOff=0  =>  -10/0/0             Inv#2
             *
             *  - if you want to write off a (partial) invoice without applying,
             *    enter zero in applied
             *              Inv=10  Disc=1  WOff=9  =>  0/1/9               Inv#1
             *  Issues
             *  - you cannot write-off a payment
             */


            //  Payment - Loop and Add them to paymentList/amountList

            try
            {
                _log.SaveError("Try Start", "Try Start");
                #region Payment-Loop
                List<int> paymentList = new List<int>(rowsPayment.Count);
                List<Decimal> amountList = new List<Decimal>(rowsPayment.Count);
                Decimal paymentAppliedAmt = Env.ZERO;
                for (int i = 0; i < rowsPayment.Count; i++)
                {
                    //  Payment line is selected
                    //  Payment variables
                    int C_Payment_ID = Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]);
                    paymentList.Add(C_Payment_ID);
                    //
                    Decimal PaymentAmt = Util.GetValueOfDecimal(rowsPayment[i][payment.ToLower()]);  //  Applied Payment
                    amountList.Add(PaymentAmt);
                    //
                    paymentAppliedAmt = Decimal.Add(paymentAppliedAmt, PaymentAmt);
                }
                #endregion

                //  Invoices - Loop and generate alloctions
                #region Invoice-Loop with allocation

                Decimal totalAppliedAmt = Env.ZERO;
                _log.SaveError("First Allocation", "First Allocation");
                //	Create Allocation - but don't save yet
                // to be save Current date on allocation -- not to pick either payment or schedule date (on behalf of mukesh sir)
                MAllocationHdr alloc = new MAllocationHdr(ctx, true,	//	manual
                    DateTime.Now, C_Currency_ID, ctx.GetContext("#AD_User_Name"), trx);
                alloc.SetAD_Org_ID(AD_Org_ID);
                //to set transaction and account date on allocation header
                alloc.SetDateAcct(DateAcct);// to set Account date on allocation header because posting and conversion are calculating on the basis of Date Account
                alloc.SetC_ConversionType_ID(_CurrencyType_ID); // to set Conversion Type on allocation header because posting and conversion are calculating on the basis of Conversion Type
                alloc.SetDateTrx(DateTrx);

                int C_InvoicePaySchedule_ID = 0;

                //	For all invoices
                int invoiceLines = 0;
                MInvoicePaySchedule mpay = null;
                MInvoice invoice = null;
                bool isScheduleAllocated = false;
                for (int i = 0; i < rowsInvoice.Count; i++)
                {
                    //  Invoice line is selected
                    isScheduleAllocated = false;
                    {
                        mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                        invoice = new MInvoice(ctx, Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]), trx);
                        invoiceLines++;
                        //  Invoice variables
                        int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                        Decimal AppliedAmt = Util.GetValueOfDecimal(rowsInvoice[i][applied.ToLower()]);
                        Decimal DiscountAmt = Util.GetValueOfDecimal(rowsInvoice[i][discount.ToLower()]);
                        Decimal WriteOffAmt = Util.GetValueOfDecimal(rowsInvoice[i][writeOff.ToLower()]);

                        // Updated over/under amount on allocation line, it should be open -( applied + discount + writeoff ) Update by vivek on 05/01/2018 issue reported by Savita
                        Decimal OverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(rowsInvoice[i][open]),
                        Decimal.Add(AppliedAmt, Decimal.Add(DiscountAmt, WriteOffAmt)));

                        //Payment Settelment**********
                        //  loop through all payments until invoice applied
                        int noPayments = 0;
                        MInvoicePaySchedule mpay2 = null;
                        MPayment objPayment = null;
                        for (int j = 0; j < paymentList.Count && Env.Signum(AppliedAmt) != 0; j++)
                        {
                            #region payment match
                            mpay2 = null;
                            int C_Payment_ID = Util.GetValueOfInt(paymentList[j]);
                            objPayment = new MPayment(ctx, C_Payment_ID, trx);
                            Decimal PaymentAmt = Util.GetValueOfDecimal(amountList[j]);

                            // check match receipt with receipt && payment with payment
                            // not payment with receipt
                            if (PaymentAmt >= 0 && AppliedAmt <= 0)
                                continue;
                            if (PaymentAmt <= 0 && AppliedAmt >= 0)
                                continue;

                            if (Env.Signum(PaymentAmt) != 0)
                            {
                                noPayments++;
                                //  use Invoice Applied Amt
                                Decimal amount = Env.ZERO;
                                if ((Math.Abs(AppliedAmt)).CompareTo(Math.Abs(PaymentAmt)) > 0)
                                {
                                    amount = PaymentAmt;
                                }
                                else
                                {
                                    amount = AppliedAmt;
                                }

                                // when 
                                if (!isScheduleAllocated)
                                {
                                    isScheduleAllocated = true;
                                    if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(amount, OverUnderAmt), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, invoice.GetC_Currency_ID(), objPayment.GetDateAcct(), objPayment.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                        mpay.SetDueAmt(Math.Abs(conertedAmount));
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                       Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));
                                    mpay.Save(trx);
                                }
                                // Create New schedule with split 
                                else if (isScheduleAllocated)
                                {
                                    mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                    PO.CopyValues(mpay, mpay2);
                                    //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                    mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                    mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());

                                    if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, invoice.GetC_Currency_ID(), objPayment.GetDateAcct(), objPayment.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                        mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(amount));
                                    mpay2.Save(trx);
                                }

                                //	Allocation Header
                                _log.SaveError("First Allocation Save Start", "First Allocation Save Start");
                                if (alloc.Get_ID() == 0 && !alloc.Save())
                                {
                                    _log.SaveError("Error: ", "Allocation not created");
                                    trx.Rollback();
                                    trx.Close();
                                    return Msg.GetMsg(ctx, "Allocationnotcreated");
                                }
                                _log.SaveError("First Allocation Saved", "First Allocation Saved");

                                if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                {
                                    OverUnderAmt = 0;
                                }

                                C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);

                                //	Allocation Line
                                MAllocationLine aLine = new MAllocationLine(alloc, amount,
                                    DiscountAmt, WriteOffAmt, OverUnderAmt);
                                aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                //if (isInterBPartner)
                                //{
                                //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, C_InvoicePaySchedule_ID, trx);
                                //    aLine.SetC_BPartner_ID(invPay.GetC_BPartner_ID());
                                //}
                                //aLine.SetPaymentInfo(C_Payment_ID, C_CashLine_ID);
                                aLine.SetPaymentInfo(C_Payment_ID, 0);//cashline for payment allocation is zero

                                if (mpay2 == null)
                                    aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                else if (mpay2 != null)
                                    aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(mpay2.GetC_InvoicePaySchedule_ID()));
                                //end

                                //to set transaction on allocation line
                                aLine.SetDateTrx(DateTrx);

                                if (!aLine.Save())
                                {
                                    // log.Log(Level.SEVERE, "Allocation Line not written - Invoice=" + C_Invoice_ID);
                                }
                                //  Apply Discounts and WriteOff only first time
                                DiscountAmt = Env.ZERO;
                                WriteOffAmt = Env.ZERO;
                                OverUnderAmt = Env.ZERO;
                                //  subtract amount from Payment/Invoice
                                AppliedAmt = Decimal.Subtract(AppliedAmt, amount);
                                //AppliedAmt = Decimal.Subtract(PaymentAmt, AppliedAmt);
                                PaymentAmt = Decimal.Subtract(PaymentAmt, amount);
                                amountList[j] = PaymentAmt;  //  update//set
                            }	//	for all applied amounts

                            // MPayment pay1 = new MPayment(ctx, C_Payment_ID, trx);
                            #endregion
                        }	//	loop through payments for invoice

                        //  No Payments allocated and none existing (e.g. Inv/CM)
                        _log.SaveError("Loop Completed", "Loop Completed");

                        if (noPayments == 0 && paymentList.Count == 0)
                        {
                            #region when match invoice to invoice
                            int C_Payment_ID = 0;

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return Msg.GetMsg(ctx, "Allocationnotcreated");
                            }
                            //	Allocation Line
                            MAllocationLine aLine = new MAllocationLine(alloc, AppliedAmt,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            aLine.SetPaymentInfo(C_Payment_ID, 0);

                            //change by Amit for payment Management
                            if (Env.IsModuleInstalled("VA009_"))
                            {
                                aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                //if (isInterBPartner)
                                //{
                                //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                                //    aLine.SetC_BPartner_ID(invPay.GetC_BPartner_ID());
                                //}
                            }
                            //end

                            //to set transaction on allocation line
                            aLine.SetDateTrx(DateTrx);

                            if (!aLine.Save())
                            {
                                //Log(Level.SEVERE, "Allocation Line not written - Invoice=" + C_Invoice_ID);
                            }
                            #endregion
                        }
                        else if (AppliedAmt != 0 && paymentList.Count != 0)
                        {
                            #region Invoice to invoice allocation when same matched with payment
                            // when we match invoice to invoice and invoice to payment for same schedule 
                            // then we have to create a new schedule for match invoice to invoice
                            if (noPayments != 0)
                            {
                                mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                PO.CopyValues(mpay, mpay2);
                                mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());

                                if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                {
                                    var conertedAmount = MConversionRate.Convert(ctx, AppliedAmt, C_Currency_ID, invoice.GetC_Currency_ID(), objPayment.GetDateAcct(), objPayment.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                    mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                }
                                else
                                    mpay2.SetDueAmt(Math.Abs(AppliedAmt));
                                mpay2.Save(trx);
                            }

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return Msg.GetMsg(ctx, "Allocationnotcreated");
                            }

                            //	Allocation Line
                            MAllocationLine aLine = new MAllocationLine(alloc, AppliedAmt,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            aLine.SetPaymentInfo(0, 0);
                            if (Env.IsModuleInstalled("VA009_"))
                            {
                                if (mpay2 != null)
                                {
                                    aLine.SetC_InvoicePaySchedule_ID(mpay2.GetC_InvoicePaySchedule_ID());
                                    //if (isInterBPartner)
                                    //{
                                    //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, mpay2.GetC_InvoicePaySchedule_ID(), trx);
                                    //    aLine.SetC_BPartner_ID(invPay.GetC_BPartner_ID());
                                    //}
                                }
                                else
                                {
                                    aLine.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                    //if (isInterBPartner)
                                    //{
                                    //    MInvoicePaySchedule invPay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                                    //    aLine.SetC_BPartner_ID(invPay.GetC_BPartner_ID());
                                    //}
                                }
                            }
                            //to set transaction on allocation line
                            aLine.SetDateTrx(DateTrx);

                            if (!aLine.Save(trx))
                            {

                            }
                            #endregion
                        }

                        totalAppliedAmt = Decimal.Add(totalAppliedAmt, AppliedAmt);
                    }   //  invoice selected
                }   //  invoice loop

                #endregion

                //	Only Payments and total of 0 (e.g. Payment/Reversal)
                #region Reversal Payments
                if ((invoiceLines == 0 && paymentList.Count > 0
                    && Env.Signum(paymentAppliedAmt) == 0) ||
                    (paymentList.Count > 0 && (amountList.Min() != 0 || amountList.Max() != 0))) // PAYMENT TO PAYMENT ALLOCATION WITH INVOICE
                {
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        int C_Payment_ID = Util.GetValueOfInt(paymentList[i]);
                        Decimal PaymentAmt = Util.GetValueOfDecimal(amountList[i]);

                        //	Allocation Header
                        if (alloc.Get_ID() == 0 && !alloc.Save())
                        {
                            _log.SaveError("Error: ", "Allocation not created");
                            trx.Rollback();
                            trx.Close();
                            return Msg.GetMsg(ctx, "Allocationnotcreated");
                        }
                        //	Allocation Line
                        MAllocationLine aLine = new MAllocationLine(alloc, PaymentAmt,
                            Env.ZERO, Env.ZERO, Env.ZERO);
                        aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                        aLine.SetPaymentInfo(C_Payment_ID, 0);
                        //to set transaction on allocation line
                        aLine.SetDateTrx(DateTrx);
                        if (!aLine.Save())
                        {
                            //  log.Log(Level.SEVERE, "Allocation Line not saved - Payment=" + C_Payment_ID);
                        }
                    }
                }	//	onlyPayments
                #endregion

                if (Env.Signum(totalAppliedAmt) != 0)
                {
                    //log.Log(Level.SEVERE, "Remaining TotalAppliedAmt=" + totalAppliedAmt);
                }

                //	Should start WF
                if (alloc.Get_ID() != 0)
                {
                    alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                    if (alloc.Save())
                    {
                        msg = alloc.GetDocumentNo();
                    }
                }

                //  Test/Set IsPaid for Invoice - requires that allocation is posted
                #region Set Invoice IsPaid
                for (int i = 0; i < rowsInvoice.Count; i++)
                {
                    //  Invoice line is selected
                    //  Invoice variables
                    int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                    String sql = "SELECT invoiceOpen(C_Invoice_ID, 0) "
                        + "FROM C_Invoice WHERE C_Invoice_ID=@param1";
                    Decimal opens = Util.GetValueOfDecimal(DB.GetSQLValueBD(trx, sql, C_Invoice_ID));
                    if (open != null && Env.Signum(opens) == 0)
                    {
                        sql = "UPDATE C_Invoice SET IsPaid='Y' "
                            + "WHERE C_Invoice_ID=" + C_Invoice_ID;
                        int no = DB.ExecuteQuery(sql, null, trx);
                    }
                    else
                    {
                        //  log.Config("Invoice #" + i + " is not paid - " + open);
                    }
                }
                #endregion

                //  Test/Set Payment is fully allocated
                #region Set Payment Allocated
                if (rowsPayment.Count > 0)
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        int C_Payment_ID = Util.GetValueOfInt(paymentList[i]);
                        MPayment pay = new MPayment(ctx, C_Payment_ID, trx);
                        if (pay.TestAllocation())
                        {
                            pay.Save();
                        }

                        string sqlGetOpenPayments = "SELECT  currencyConvert(ALLOCPAYMENTAVAILABLE(C_Payment_ID) ,p.C_Currency_ID ,260,p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID) FROM C_Payment p Where C_Payment_ID = " + C_Payment_ID;
                        object result = DB.ExecuteScalar(sqlGetOpenPayments, null, trx);
                        Decimal? amtPayment = 0;
                        if (result == null || result == DBNull.Value)
                        {
                            amtPayment = -1;
                        }
                        else
                        {
                            amtPayment = Util.GetValueOfDecimal(result);
                        }

                        if (amtPayment == 0)
                        {
                            pay.SetIsAllocated(true);
                        }
                        else
                        {
                            pay.SetIsAllocated(false);
                        }
                        pay.Save();

                        //log.Config("Payment #" + i + (pay.IsAllocated() ? " not" : " is")
                        //    + " fully allocated");
                    }
                #endregion

                paymentList.Clear();
                amountList.Clear();
                SetIsprocessingFalse(rowsPayment, "cpaymentid", false, false, trx); //Payment
                SetIsprocessingFalse(rowsCash, "ccashlineid", true, false, trx); //CashLine
                SetIsprocessingFalse(rowsInvoice, "c_invoicepayschedule_id", false, true, trx); //InvoicePaySchedule
                trx.Commit();
                trx.Close();
            }
            catch (Exception e)
            {
                if (trx != null)
                {
                    trx.Rollback();
                    trx.Close();
                    trx = null;
                    return e.Message;
                }
            }
            finally
            {
                if (trx != null)
                {
                   // trx.Rollback();
                    trx.Close();
                    trx = null;
                }

            }
            return Msg.GetMsg(ctx, "AllocationCreatedWith") + msg;
        }

        // checking period is open ornot for allocation
        public string CheckPeriodState(DateTime DateTrx)
        {
            if (!MPeriod.IsOpen(ctx, DateTrx, MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION))
            {
                return Msg.GetMsg(ctx, "PeriodClosed");
            }
            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(ctx, DateTrx))
            {
                return Msg.GetMsg(ctx, "DateIsInNonBusinessDay");
            }
            return "";
        }

        public List<VIS_PaymentData> GetPayments(int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, int page, int size)
        {
            //used to get related business partner against selected business partner 
            string relatedBpids = string.Empty;
            //if (isInterBPartner)
            //{
            //    relatedBpids = GetRelatedBP(_C_BPartner_ID);
            //}
            int countRecord = 0;
            // used to create for preciosion handling
            MCurrency objCurrency = MCurrency.Get(ctx, _C_Currency_ID);

            //Changed DateTrx to DateAcct because we have to convert currency on Account Date Not on Transaction Date 
            string sql = "SELECT 'false' as SELECTROW, TO_CHAR(p.DateTrx,'YYYY-MM-DD') as DATE1,p.DocumentNo As DOCUMENTNO,p.C_Payment_ID As CPAYMENTID,"  //  1..3
              + @"c.ISO_Code as ISOCODE, 
                CASE
                  WHEN NVL(p.C_CONVERSIONTYPE_ID , 0) !=0 THEN p.C_CONVERSIONTYPE_ID
                  WHEN (GetConversionType(p.AD_Client_ID) != 0 ) THEN GetConversionType(p.AD_Client_ID)
                  ELSE (GetConversionType(0)) END AS C_CONVERSIONTYPE_ID,
                CASE 
                  WHEN NVL(p.C_CONVERSIONTYPE_ID , 0) !=0 THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = p.C_CONVERSIONTYPE_ID )
                  WHEN (GetConversionType(p.AD_Client_ID) != 0 ) THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = GetConversionType(p.AD_Client_ID))
                  ELSE (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID =(GetConversionType(0)) ) END AS CONVERSIONNAME, 
                  ROUND(p.PayAmt, " + objCurrency.GetStdPrecision() + ") AS PAYMENT,"                            //  4..5
              + "ROUND(currencyConvert(p.PayAmt ,p.C_Currency_ID ," + _C_Currency_ID + ",p.DATEACCT ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID ), " + objCurrency.GetStdPrecision() + ") AS CONVERTEDAMOUNT,"//  6   #1
              + "ROUND(currencyConvert(ALLOCPAYMENTAVAILABLE(p.C_Payment_ID) ,p.C_Currency_ID ," + _C_Currency_ID + ",p.DATEACCT ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID), " + objCurrency.GetStdPrecision() + ") as OPENAMT,"  //  7   #2
              + "p.MultiplierAP as MULTIPLIERAP, 0 as APPLIEDAMT ,   p.DATEACCT AS DATEACCT, p.AD_Org_ID , o.Name "
                //+ " , dc.name AS DocTypeName "
              + "FROM C_Payment_v p"		//	Corrected for AP/AR
              + " INNER JOIN AD_Org o ON o.AD_Org_ID = p.AD_Org_ID "
              + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID) "
              + " INNER JOIN C_Payment cp ON (p.C_Payment_ID = cp.C_Payment_ID) "
                // + " INNER JOIN C_DOCTYPE DC ON (P.C_DOCTYPE_ID=DC.C_DOCTYPE_ID) "
              + " WHERE   ((p.IsAllocated ='N' and p.c_charge_id is null) "
              + " OR (p.isallocated = 'N' AND p.c_charge_id is not null and p.isprepayment = 'Y'))"
              + " AND p.Processed='Y' AND p.processing ='N' AND p.DocStatus IN ('CO','CL') "
              + " AND p.C_BPartner_ID=" + _C_BPartner_ID;

            //when paymnet having order schedule ref - those not to be shown in grid 
            if (Env.IsModuleInstalled("VA009_"))
            {
                sql += " AND cp.VA009_OrderPaySchedule_ID IS NULL ";
            }
            if (!chk)
            {
                sql += " AND p.C_Currency_ID=" + _C_Currency_ID;				//      #4
            }
            //to get payment against related business partner
            if (!string.IsNullOrEmpty(relatedBpids))
                sql += " OR p.C_BPartner_ID IN ( " + relatedBpids + " ) ";

            sql += " ORDER BY p.DateTrx,p.DocumentNo";
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "p", true, false);

            List<VIS_PaymentData> payData = new List<VIS_PaymentData>();

            // count record for paging
            if (page == 1)
            {
                string sql1 = @"SELECT COUNT(*) FROM C_Payment_v p"
              + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID) "
              + " WHERE   ((p.IsAllocated ='N' and p.c_charge_id is null) "
              + " OR (p.isallocated = 'N' AND p.c_charge_id is not null and p.isprepayment = 'Y'))"
              + " AND p.Processed='Y' AND p.DocStatus IN ('CO','CL') "
              + " AND p.C_BPartner_ID=" + _C_BPartner_ID;
                if (!chk)
                {
                    sql1 += " AND p.C_Currency_ID=" + _C_Currency_ID;
                }
                //to get payment against related business partner
                if (!string.IsNullOrEmpty(relatedBpids))
                    sql1 += "   OR p.C_BPartner_ID IN ( " + relatedBpids + " ) ";

                sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql1, "p", true, false);
                countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql1, null, null));
            }

            DataSet dr = VIS.DBase.DB.ExecuteDatasetPaging(sql, page, size);

            if (dr != null && dr.Tables.Count > 0 && dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    //pData
                    VIS_PaymentData pData = new VIS_PaymentData();
                    pData.SelectRow = "false";
                    pData.PaymentRecord = countRecord;
                    pData.Date1 = dr.Tables[0].Rows[i]["DATE1"].ToString();
                    pData.Documentno = dr.Tables[0].Rows[i]["DOCUMENTNO"].ToString();
                    pData.CpaymentID = dr.Tables[0].Rows[i]["CPAYMENTID"].ToString();
                    pData.Isocode = dr.Tables[0].Rows[i]["ISOCODE"].ToString();
                    pData.Payment = dr.Tables[0].Rows[i]["PAYMENT"].ToString();
                    pData.ConvertedAmount = dr.Tables[0].Rows[i]["CONVERTEDAMOUNT"].ToString();
                    pData.OpenAmt = dr.Tables[0].Rows[i]["OPENAMT"].ToString();
                    pData.Multiplierap = dr.Tables[0].Rows[i]["MULTIPLIERAP"].ToString();
                    pData.AppliedAmt = dr.Tables[0].Rows[i]["APPLIEDAMT"].ToString();
                    pData.C_ConversionType_ID = Util.GetValueOfInt(dr.Tables[0].Rows[i]["C_CONVERSIONTYPE_ID"]);
                    pData.ConversionName = Util.GetValueOfString(dr.Tables[0].Rows[i]["CONVERSIONNAME"]);
                    pData.DATEACCT = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["DATEACCT"]);
                    pData.AD_Org_ID = Convert.ToInt32(dr.Tables[0].Rows[i]["AD_Org_ID"]);
                    pData.OrgName = Convert.ToString(dr.Tables[0].Rows[i]["Name"]);
                    payData.Add(pData);
                }
            }

            if (dr != null)
            {
                dr.Dispose();
            }

            return payData;


            //public GetPaymentCashInvoice LoadBPartnerVallocation(Ctx ctx, int c_currencyid, int c_bpartnerid, bool chks, string get_date)
            //{
            //    GetPaymentCashInvoice obj = new GetPaymentCashInvoice();
            //    obj.Payment = GetPaymentForBPpartner(ctx, c_currencyid, c_bpartnerid, chks, get_date);
            //    obj.Cash = GetCashForBPpartner(ctx, c_currencyid, c_bpartnerid, chks, get_date);
            //    obj.Invoice = GetInvoiceForBPpartner(ctx, c_currencyid, c_bpartnerid, chks, get_date);
            //    return obj;
            //}

            //public List<PayAllocPayment> GetPaymentForBPpartner(Ctx ctx, int c_currencyid, int c_bpartnerid, bool chks, string get_date)
            //{

            //    List<PayAllocPayment> obj = new List<PayAllocPayment>();

            //    string sql = "SELECT 'false' as SELECTROW, p.DateTrx as DATE1,p.DocumentNo As DOCUMENTNO,p.C_Payment_ID As CPAYMENTID,"  //  1..3
            //               + "c.ISO_Code as ISOCODE,p.PayAmt AS PAYMENT,"                            //  4..5
            //               + "currencyConvert(p.PayAmt ,p.C_Currency_ID ," + c_currencyid + ",p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID ) AS CONVERTEDAMOUNT,"//  6   #1
            //               + "currencyConvert(ALLOCPAYMENTAVAILABLE(C_Payment_ID) ,p.C_Currency_ID ," + c_currencyid + ",p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID) as OPENAMT,"  //  7   #2
            //               + "p.MultiplierAP as MULTIPLIERAP, "
            //               + "0 as APPLIEDAMT "
            //               + "FROM C_Payment_v p"
            //               + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID) "
            //               + "WHERE "
            //               + "  ((p.IsAllocated ='N' and p.c_charge_id is null) "
            //               + " OR (p.isallocated = 'N' AND p.c_charge_id is not null and p.isprepayment = 'Y'))"
            //               + " AND p.Processed='Y'"
            //               + " AND p.C_BPartner_ID=" + c_bpartnerid;
            //    if (!chks)
            //    {
            //        sql += " AND p.C_Currency_ID=" + c_currencyid;				//      #4
            //    }
            //    sql += " ORDER BY p.DateTrx,p.DocumentNo";

            //    sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "p", true, false);

            //    DataSet ds = DB.ExecuteDataset(sql);



            //    if (ds != null && ds.Tables[0].Rows.Count > 0)
            //    {
            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //        {
            //            PayAllocPayment pp = new PayAllocPayment();
            //            pp.selectrow = Util.GetValueOfString(ds.Tables[0].Rows[i]["SELECTROW"]);
            //            pp.date1 = Convert.ToDateTime(ds.Tables[0].Rows[i]["DATE1"]);
            //            pp.documentno = Util.GetValueOfString(ds.Tables[0].Rows[i]["DOCUMENTNO"]);
            //            pp.CPAYMENTID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["CPAYMENTID"]);
            //            pp.isocode = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISOCODE"]);
            //            pp.payment = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["PAYMENT"]);
            //            pp.convertedamount = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["CONVERTEDAMOUNT"]);
            //            pp.openamt = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["OPENAMT"]);
            //            pp.multiplierap = Util.GetValueOfString(ds.Tables[0].Rows[i]["MULTIPLIERAP"]);
            //            pp.appliedamt = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["APPLIEDAMT"]);
            //            obj.Add(pp);
            //        }
            //    }

            //    return obj;
            //}

            //public List<PayAllocCash> GetCashForBPpartner(Ctx ctx, int c_currencyid, int c_bpartnerid, bool chks, string get_date)
            //{
            //    List<PayAllocCash> obj = new List<PayAllocCash>();

            //    string sqlCash = "SELECT 'false' as SELECTROW, cn.created as CREATED, cn.receiptno AS RECEIPTNO, cn.c_cashline_id AS CCASHLINEID,"
            //              + "c.ISO_Code AS ISO_CODE,cn.amount AS AMOUNT, "
            //              + "currencyConvert(cn.Amount ,cn.C_Currency_ID ," + c_currencyid + ",cn.Created ,114 ,cn.AD_Client_ID ,cn.AD_Org_ID ) AS CONVERTEDAMOUNT,"//  6   #1cn.amount as OPENAMT,"
            //              + " cn.amount as OPENAMT,"
            //              + "cn.MultiplierAP AS MULTIPLIERAP,"
            //              + "0 as APPLIEDAMT "
            //              + " from c_cashline_new cn"

            //              + " INNER join c_currency c ON (cn.C_Currency_ID=c.C_Currency_ID)"
            //              //+ " WHERE cn.IsAllocated   ='N' AND cn.Processed ='Y'"
            //              + " WHERE cn.IsAllocated   ='N'"// AND cn.Processed ='Y'"
            //              + " and cn.cashtype = 'B' and cn.docstatus in ('CO','CL') "
            //              + " AND cn.C_BPartner_ID=" + c_bpartnerid;
            //    if (!chks)
            //    {
            //        sqlCash += " AND cn.C_Currency_ID=" + c_currencyid;
            //    }
            //    sqlCash += " ORDER BY cn.created,cn.receiptno";

            //    // sqlCash = MRole.GetDefault(ctx).AddAccessSQL(sqlCash, "cn", true, false);

            //    DataSet ds = DB.ExecuteDataset(sqlCash);

            //    if (ds != null && ds.Tables[0].Rows.Count > 0)
            //    {
            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //        {
            //            PayAllocCash pp = new PayAllocCash();
            //            pp.selectrow = Util.GetValueOfString(ds.Tables[0].Rows[i]["SELECTROW"]);
            //            pp.created = Convert.ToDateTime(ds.Tables[0].Rows[i]["CREATED"]);
            //            pp.receiptno = Util.GetValueOfString(ds.Tables[0].Rows[i]["RECEIPTNO"]);
            //            pp.iso_code = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]);
            //            pp.amount = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AMOUNT"]);
            //            pp.convertedamount = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["CONVERTEDAMOUNT"]);
            //            pp.openamt = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["OPENAMT"]);
            //            pp.appliedamt = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["APPLIEDAMT"]);
            //            pp.ccashlineid = Util.GetValueOfInt(ds.Tables[0].Rows[i]["CCASHLINEID"]);
            //            pp.multiplierap = Util.GetValueOfString(ds.Tables[0].Rows[i]["MULTIPLIERAP"]);
            //            obj.Add(pp);
            //        }
            //    }
            //    return obj;
            //}

            //public List<PayAllocInvoice> GetInvoiceForBPpartner(Ctx ctx, int c_currencyid, int c_bpartnerid, bool chks, string get_date)
            //{
            //    List<PayAllocInvoice> obj = new List<PayAllocInvoice>();
            //    string sqlInvoice = "SELECT 'false' as SELECTROW , i.DateInvoiced  as DATE1 ,"
            //                  + "  i.DocumentNo    AS DOCUMENTNO  ,"
            //                  + "  i.C_Invoice_ID AS CINVOICEID,"
            //                  + "  c.ISO_Code AS ISO_CODE    ,"
            //                  + "  (invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP) AS CURRENCY    ,"
            //                  + "currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP,i.C_Currency_ID ," + c_currencyid + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID ) AS CONVERTED  ,"
            //                  + " currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + c_currencyid + ",i.DateInvoiced,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID)                                         *i.MultiplierAP AS AMOUNT,"
            //                  + "  (currencyConvert(invoiceDiscount(i.C_Invoice_ID ," + get_date + ",C_InvoicePaySchedule_ID),i.C_Currency_ID ," + c_currencyid + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID )*i.Multiplier*i.MultiplierAP) AS DISCOUNT ,"
            //                  + "  i.MultiplierAP ,i.docbasetype  ,"
            //                  + "0 as WRITEOFF ,"
            //                  + "0 as APPLIEDAMT , i.C_InvoicePaySchedule_ID "
            //                  + " FROM C_Invoice_v i"
            //                  + " INNER JOIN C_Currency c ON (i.C_Currency_ID=c.C_Currency_ID) "
            //                  + "WHERE i.IsPaid='N' AND i.Processed='Y'"
            //                  + " AND i.C_BPartner_ID=" + c_bpartnerid;                                            //  #5
            //    if (!chks)
            //    {
            //        sqlInvoice += " AND i.C_Currency_ID=" + c_currencyid;                                   //  #6
            //    }
            //    sqlInvoice += " ORDER BY i.DateInvoiced, i.DocumentNo";

            //    // sqlInvoice = MRole.GetDefault(ctx).AddAccessSQL(sqlInvoice, "i", true, false);

            //    DataSet ds = DB.ExecuteDataset(sqlInvoice);


            //    if (ds != null && ds.Tables[0].Rows.Count > 0)
            //    {
            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //        {
            //            PayAllocInvoice pp = new PayAllocInvoice();
            //            pp.selectrow = Util.GetValueOfString(ds.Tables[0].Rows[i]["SELECTROW"]);
            //            pp.date1 = Convert.ToDateTime(ds.Tables[0].Rows[i]["DATE1"]);
            //            pp.documentno = Util.GetValueOfString(ds.Tables[0].Rows[i]["DOCUMENTNO"]);
            //            pp.cinvoiceid = Util.GetValueOfInt(ds.Tables[0].Rows[i]["CINVOICEID"]);
            //            pp.iso_code = Util.GetValueOfString(ds.Tables[0].Rows[i]["ISO_CODE"]);
            //            pp.currency = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["CURRENCY"]);
            //            pp.converted = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["CONVERTED"]);
            //            pp.amount = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AMOUNT"]);
            //            pp.discount = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["DISCOUNT"]);
            //            pp.multiplierap = Util.GetValueOfString(ds.Tables[0].Rows[i]["MultiplierAP"]);
            //            pp.docbasetype = Util.GetValueOfString(ds.Tables[0].Rows[i]["docbasetype"]);                  
            //            pp.writeoff = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["WRITEOFF"]);
            //            pp.appliedamt = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["APPLIEDAMT"]);                                   
            //            pp.c_invoicepayschedule_id = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_InvoicePaySchedule_ID"]);
            //            obj.Add(pp);
            //        }
            //    }


            //    return obj;
            //}






        }

        public List<VIS_CashData> GetCashJounral(int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, int page, int size)
        {
            //used to get related business partner against selected business partner 
            string relatedBpids = string.Empty;
            //if (isInterBPartner)
            //{
            //    relatedBpids = GetRelatedBP(_C_BPartner_ID);
            //}

            int countRecord = 0;
            // used to create for preciosion handling
            MCurrency objCurrency = MCurrency.Get(ctx, _C_Currency_ID);

            //Changed created date to DateAcct because we have to convert currency on Account Date Not on Created Date 
            string sqlCash = "SELECT 'false' as SELECTROW, TO_CHAR(cn.created ,'YYYY-MM-DD') as CREATED, cn.receiptno AS RECEIPTNO, cn.c_cashline_id AS CCASHLINEID,"
                             + @"c.ISO_Code AS ISO_CODE,
                                CASE
                                 WHEN NVL(cn.C_CONVERSIONTYPE_ID , 0) !=0 THEN cn.C_CONVERSIONTYPE_ID
                                 WHEN (GetConversionType(cn.AD_Client_ID) != 0 ) THEN GetConversionType(cn.AD_Client_ID)
                                 ELSE (GetConversionType(0)) END AS C_CONVERSIONTYPE_ID,
                                CASE 
                                  WHEN NVL(cn.C_CONVERSIONTYPE_ID , 0) !=0 THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = cn.C_CONVERSIONTYPE_ID )
                                  WHEN (GetConversionType(cn.AD_Client_ID) != 0 ) THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = GetConversionType(cn.AD_Client_ID))
                                  ELSE (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID =(GetConversionType(0)) ) END AS CONVERSIONNAME ,
                                  ROUND(cn.amount, " + objCurrency.GetStdPrecision() + ") AS AMOUNT, "
                             + " ROUND(currencyConvert(cn.Amount ,cn.C_Currency_ID ," + _C_Currency_ID + ",cn.DATEACCT ,cn.C_ConversionType_ID  ,cn.AD_Client_ID ,cn.AD_Org_ID ) , " + objCurrency.GetStdPrecision() + ") AS CONVERTEDAMOUNT,"//  6   #1cn.amount as OPENAMT,"
                             + " ROUND(currencyConvert(ALLOCCASHAVAILABLE(cn.C_CashLine_ID) ,cn.C_Currency_ID ," + _C_Currency_ID + ",cn.DATEACCT,cn.C_ConversionType_ID ,cn.AD_Client_ID ,cn.AD_Org_ID), " + objCurrency.GetStdPrecision() + ") as OPENAMT,"  //  7   #2
                //+ " currencyConvert(cn.Amount ,cn.C_Currency_ID ," + _C_Currency_ID + ",cn.Created ,114 ,cn.AD_Client_ID ,cn.AD_Org_ID ) as OPENAMT,"
                             + " cn.MultiplierAP AS MULTIPLIERAP,0 as APPLIEDAMT,cn.DATEACCT , o.AD_Org_ID  , o.Name  from c_cashline_new cn"
                              + " INNER JOIN AD_Org o ON o.AD_Org_ID = cn.AD_Org_ID "
                             + " INNER join c_currency c ON (cn.C_Currency_ID=c.C_Currency_ID) WHERE cn.IsAllocated   ='N' AND cn.Processed ='Y'"
                //+ " and cn.cashtype IN ('I' , 'B') and cn.docstatus in ('CO','CL') "
                             + " and cn.cashtype IN ('B') and cn.docstatus in ('CO','CL') "// AND cn.Processing = 'N' "
                // Commented because Against Business Partner there is no charge
                // + " AND cn.C_Charge_ID  IS Not NULL"
                             + " AND cn.C_BPartner_ID=" + _C_BPartner_ID;
            if (!chk)
            {
                sqlCash += " AND cn.C_Currency_ID=" + _C_Currency_ID;
            }
            //to get CashLines against related business partner
            if (!string.IsNullOrEmpty(relatedBpids))
                sqlCash += " OR cn.C_BPartner_ID IN ( " + relatedBpids + " ) ";

            sqlCash += " ORDER BY cn.created,cn.receiptno";

            sqlCash = MRole.GetDefault(ctx).AddAccessSQL(sqlCash, "cn", true, false);

            List<VIS_CashData> payData = new List<VIS_CashData>();

            // count record for paging
            if (page == 1)
            {
                string sql = @"SELECT COUNT(*) FROM c_cashline_new cn"
                             + " INNER join c_currency c ON (cn.C_Currency_ID=c.C_Currency_ID) WHERE cn.IsAllocated   ='N' AND cn.Processed ='Y'"
                             + " and cn.cashtype = 'B' and cn.docstatus in ('CO','CL') "
                             + " AND cn.C_BPartner_ID=" + _C_BPartner_ID;
                if (!chk)
                {
                    sql += " AND cn.C_Currency_ID=" + _C_Currency_ID;
                }
                //to get CashLines against related business partner
                if (!string.IsNullOrEmpty(relatedBpids))
                    sql += "   OR cn.C_BPartner_ID IN ( " + relatedBpids + " ) ";

                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "cn", true, false);
                countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            }

            DataSet dr = VIS.DBase.DB.ExecuteDatasetPaging(sqlCash, page, size);

            if (dr != null && dr.Tables.Count > 0 && dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    //pData
                    VIS_CashData pData = new VIS_CashData();
                    pData.SelectRow = "false";
                    pData.CashRecord = countRecord;
                    pData.Created = dr.Tables[0].Rows[i]["CREATED"].ToString();
                    pData.ReceiptNo = dr.Tables[0].Rows[i]["RECEIPTNO"].ToString();
                    pData.Isocode = dr.Tables[0].Rows[i]["ISO_CODE"].ToString();
                    pData.Amount = dr.Tables[0].Rows[i]["Amount"].ToString();
                    pData.ConvertedAmount = dr.Tables[0].Rows[i]["CONVERTEDAMOUNT"].ToString();
                    pData.OpenAmt = dr.Tables[0].Rows[i]["OPENAMT"].ToString();
                    pData.Multiplierap = dr.Tables[0].Rows[i]["MULTIPLIERAP"].ToString();
                    pData.CcashlineiID = dr.Tables[0].Rows[i]["CCASHLINEID"].ToString();
                    pData.AppliedAmt = dr.Tables[0].Rows[i]["APPLIEDAMT"].ToString();
                    pData.C_ConversionType_ID = Util.GetValueOfInt(dr.Tables[0].Rows[i]["C_CONVERSIONTYPE_ID"]);
                    pData.ConversionName = Util.GetValueOfString(dr.Tables[0].Rows[i]["CONVERSIONNAME"]);
                    pData.DATEACCT = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["DATEACCT"]);
                    pData.AD_Org_ID = Convert.ToInt32(dr.Tables[0].Rows[i]["AD_Org_ID"]);
                    pData.OrgName = Convert.ToString(dr.Tables[0].Rows[i]["Name"]);
                    payData.Add(pData);
                }
            }

            if (dr != null)
            {
                dr.Dispose();
            }

            return payData;

        }

        public List<VIS_InvoiceData> GetInvoice(int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, string date, int page, int size, string docNo, int c_docType_ID, DateTime? fromDate, DateTime? toDate, string conversionDate)
        {
            //used to get related business partner against selected business partner 
            string relatedBpids = string.Empty;
            //if (isInterBPartner)
            //{
            //    relatedBpids = GetRelatedBP(_C_BPartner_ID);
            //}

            int countRecord = 0;
            // used to create for preciosion handling
            MCurrency objCurrency = MCurrency.Get(ctx, _C_Currency_ID);

            //Changed DateInvoiced to DateAcct because we have to convert currency on Account Date Not on Invoiced Date 
            string sqlInvoice = "SELECT 'false' as SELECTROW , TO_CHAR(i.DateInvoiced,'YYYY-MM-DD')  as DATE1  ,  i.DocumentNo    AS DOCUMENTNO  , i.C_Invoice_ID AS CINVOICEID,"
                                + @"  c.ISO_Code AS ISO_CODE , 
                                     CASE
                                      WHEN NVL(i.C_CONVERSIONTYPE_ID , 0) !=0 THEN i.C_CONVERSIONTYPE_ID
                                      WHEN (GetConversionType(i.AD_Client_ID) != 0 ) THEN GetConversionType(i.AD_Client_ID)
                                      ELSE (GetConversionType(0)) END AS C_CONVERSIONTYPE_ID,
                                    CASE 
                                      WHEN NVL(i.C_CONVERSIONTYPE_ID , 0) !=0 THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = i.C_CONVERSIONTYPE_ID )
                                      WHEN (GetConversionType(i.AD_Client_ID) != 0 ) THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = GetConversionType(i.AD_Client_ID))
                                      ELSE (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID =(GetConversionType(0)) ) END AS CONVERSIONNAME   ,ROUND((invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP), " + objCurrency.GetStdPrecision() + ") AS CURRENCY    ,"
                                + "ROUND(currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP,i.C_Currency_ID ," + _C_Currency_ID + ", " + (conversionDate != "" ? GlobalVariable.TO_DATE(Convert.ToDateTime(conversionDate), true) : " i.DATEACCT ") + ",i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID ), " + objCurrency.GetStdPrecision() + ") AS CONVERTED  ,"
                                + " ROUND(currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + _C_Currency_ID + "," + (conversionDate != "" ? GlobalVariable.TO_DATE(Convert.ToDateTime(conversionDate), true) : " i.DATEACCT ") + ",i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID)                                         *i.MultiplierAP , " + objCurrency.GetStdPrecision() + ") AS AMOUNT,"
                                + "  ROUND((currencyConvert(invoiceDiscount(i.C_Invoice_ID ," + date + ",C_InvoicePaySchedule_ID),i.C_Currency_ID ," + _C_Currency_ID + "," + (conversionDate != "" ? GlobalVariable.TO_DATE(Convert.ToDateTime(conversionDate), true) : " i.DATEACCT ") + " ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID )*i.Multiplier*i.MultiplierAP) , " + objCurrency.GetStdPrecision() + ") AS DISCOUNT ,"
                                + "  i.MultiplierAP ,i.docbasetype  ,0 as WRITEOFF ,0 as APPLIEDAMT ,i.DATEACCT, i.C_InvoicePaySchedule_ID,(select TO_CHAR(Ip.Duedate,'YYYY-MM-DD') from C_InvoicePaySchedule ip where C_InvoicePaySchedule_ID=i.C_InvoicePaySchedule_ID) Scheduledate "
                //  + ", dc.name AS DocTypeName "
                                + " , i.AD_Org_ID , o.Name "
                                + " FROM C_Invoice_v i"		//  corrected for CM/Split
                                + " INNER JOIN AD_Org o ON o.AD_Org_ID = i.AD_Org_ID "
                                + " INNER JOIN C_Currency c ON (i.C_Currency_ID=c.C_Currency_ID) "
                // + " INNER JOIN C_DOCTYPE DC ON (i.C_DOCTYPE_ID =DC.C_DOCTYPE_ID)"
                                + " WHERE i.IsPaid='N' AND i.Processed='Y'"
                                + " AND i.C_BPartner_ID=" + _C_BPartner_ID;                                            //  #5
            if (!chk)
            {
                sqlInvoice += " AND i.C_Currency_ID=" + _C_Currency_ID;                                   //  #6
            }
            sqlInvoice += " AND (currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + _C_Currency_ID + ",i.DATEACCT,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID) *i.MultiplierAP ) <> 0 ";


            //------Filter data on the basis of new parameters
            if (!String.IsNullOrEmpty(docNo))
            {
                sqlInvoice += "AND Upper(i.documentno) LIKE Upper('%" + docNo + "%')";
            }
            if (c_docType_ID > 0)
            {
                sqlInvoice += " AND I.C_DOCTYPETARGET_ID=" + c_docType_ID;
            }
            if (fromDate != null)
            {
                if (toDate != null)
                {
                    sqlInvoice += " AND I.DATEINVOICED BETWEEN " + GlobalVariable.TO_DATE(fromDate, true) + " AND " + GlobalVariable.TO_DATE(toDate, true);
                }
                else
                {
                    sqlInvoice += " AND I.DATEINVOICED >= " + GlobalVariable.TO_DATE(fromDate, true);
                }
            }
            if (fromDate == null && toDate != null)
            {
                sqlInvoice += " AND I.DATEINVOICED <=" + GlobalVariable.TO_DATE(toDate, true);
            }
            //to get invoice schedules against related business partner
            if (!string.IsNullOrEmpty(relatedBpids))
                sqlInvoice += "   OR I.C_BPartner_ID IN ( " + relatedBpids + " ) ";
            //--------------------------------------
            sqlInvoice = MRole.GetDefault(ctx).AddAccessSQL(sqlInvoice, "i", true, false);
            List<VIS_InvoiceData> payData = new List<VIS_InvoiceData>();

            // count record for paging
            if (page == 1)
            {
                string sql = @"SELECT COUNT(*) FROM C_Invoice_v i"
                                + " INNER JOIN C_Currency c ON (i.C_Currency_ID=c.C_Currency_ID) WHERE i.IsPaid='N' AND i.Processed='Y'"
                                + " AND i.C_BPartner_ID=" + _C_BPartner_ID;
                if (!chk)
                {
                    sql += " AND i.C_Currency_ID=" + _C_Currency_ID;
                }
                //to get invoice schedules against related business partner
                if (!string.IsNullOrEmpty(relatedBpids))
                    sqlInvoice += "   OR i.C_BPartner_ID IN ( " + relatedBpids + " ) ";

                sql += " AND (currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + _C_Currency_ID + ",i.DateInvoiced,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID) *i.MultiplierAP ) <> 0 ";
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "i", true, false);
                countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            }

            //IDataReader dr = DB.ExecuteReader(sqlInvoice);
            DataSet dr = VIS.DBase.DB.ExecuteDatasetPaging(sqlInvoice, page, size);
            if (dr != null && dr.Tables.Count > 0 && dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    VIS_InvoiceData pData = new VIS_InvoiceData();
                    pData.SelectRow = "false";
                    pData.InvoiceRecord = countRecord;
                    pData.Date1 = dr.Tables[0].Rows[i]["DATE1"].ToString();
                    pData.Documentno = dr.Tables[0].Rows[i]["DOCUMENTNO"].ToString();
                    pData.CinvoiceID = dr.Tables[0].Rows[i]["CINVOICEID"].ToString();
                    pData.Isocode = dr.Tables[0].Rows[i]["ISO_CODE"].ToString();
                    pData.Currency = dr.Tables[0].Rows[i]["CURRENCY"].ToString();
                    pData.Converted = dr.Tables[0].Rows[i]["CONVERTED"].ToString();
                    pData.Amount = dr.Tables[0].Rows[i]["Amount"].ToString();
                    pData.Discount = dr.Tables[0].Rows[i]["DISCOUNT"].ToString();
                    pData.Multiplierap = dr.Tables[0].Rows[i]["MULTIPLIERAP"].ToString();
                    pData.DocBaseType = dr.Tables[0].Rows[i]["docbasetype"].ToString();
                    pData.Writeoff = dr.Tables[0].Rows[i]["WRITEOFF"].ToString();
                    pData.AppliedAmt = dr.Tables[0].Rows[i]["APPLIEDAMT"].ToString();
                    pData.C_InvoicePaySchedule_ID = dr.Tables[0].Rows[i]["C_InvoicePaySchedule_ID"].ToString();
                    pData.InvoiceScheduleDate = dr.Tables[0].Rows[i]["Scheduledate"].ToString();
                    pData.C_ConversionType_ID = Util.GetValueOfInt(dr.Tables[0].Rows[i]["C_CONVERSIONTYPE_ID"]);
                    pData.ConversionName = Util.GetValueOfString(dr.Tables[0].Rows[i]["CONVERSIONNAME"]);
                    pData.DATEACCT = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["DATEACCT"]);
                    pData.AD_Org_ID = Convert.ToInt32(dr.Tables[0].Rows[i]["AD_Org_ID"]);
                    pData.OrgName = Convert.ToString(dr.Tables[0].Rows[i]["Name"]);
                    payData.Add(pData);
                }
            }

            if (dr != null)
            {
                //dr.Close();
                dr.Dispose();
            }

            return payData;

        }

        //Neha 
        public List<VIS_DocType> GetDocType()
        {
            List<VIS_DocType> DocType = new List<VIS_DocType>();
            string _sql = "SELECT C_DocType.NAME, C_DocType.C_DOCTYPE_ID FROM C_DOCTYPE C_DOCTYPE INNER JOIN C_DOCBASETYPE DB ON C_DocType.DOCBASETYPE=DB.DOCBASETYPE WHERE C_DocType.DOCBASETYPE IN ('ARI','API','ARC','APC') AND C_DocType.ISACTIVE='Y'";
            _sql = MRole.GetDefault(ctx).AddAccessSQL(_sql, "C_DocType", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(_sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DocType.Add(new VIS_DocType() { DocType = Convert.ToString(ds.Tables[0].Rows[i]["Name"]), C_DocType_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_DocType_ID"]) });
                }
                ds.Dispose();
            }
            return DocType;
        }
        //Neha

        public int GetCurrencyPrecision(int _C_Currency_ID)
        {
            int precision = 0;
            MCurrency cr = new MCurrency(ctx, _C_Currency_ID, null);
            precision = cr.GetStdPrecision();
            return precision;
        }

        ///  <summary>
        /// Get all Organization which are accessable by login user
        /// </summary>        
        /// <param name="ctx"> Context Object </param>
        /// <returns>AD_Org_ID and Organization Name</returns> //Added by manjot on 27/02/2019 
        public List<NameValue> GetOrganization(Ctx ctx)
        {
            List<NameValue> retValue = new List<NameValue>();
            string _sql = " SELECT AD_Org.AD_Org_ID, AD_Org.Name FROM AD_Org AD_Org WHERE AD_Org.AD_Org_ID NOT IN (0) AND AD_Org.IsSummary='N' AND AD_Org.IsActive='Y' ";
            _sql = MRole.GetDefault(ctx).AddAccessSQL(_sql, "AD_Org", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            _sql += " ORDER BY AD_Org.Name ";
            DataSet _ds = DB.ExecuteDataset(_sql);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
                {
                    retValue.Add(new NameValue() { Name = Util.GetValueOfString(_ds.Tables[0].Rows[i]["Name"]), Value = Util.GetValueOfInt(_ds.Tables[0].Rows[i]["AD_Org_ID"]) });
                }
            }
            return retValue;
        }

        /// <summary>
        /// Validate all the ids and lock them for update
        /// </summary>        
        /// <param name="rows"> Selected records list </param>
        /// <param name="colName"> column Name </param>
        /// <param name="isCash"> Cash </param>
        /// <param name="isInvoice"> Invoice </param>
        /// <param name="trx"> Trx </param>
        /// <returns>string Value</returns>it'll return either empty string or any error msg
        /// </summary>
        public string ValidateRecords(List<Dictionary<string, string>> rows, string colName, bool isCash, bool isInvoice, Trx trx)
        {
            StringBuilder msg = new StringBuilder();
            for (int i = 0; i < rows.Count; i++)
            {
                int ID = Util.GetValueOfInt(rows[i][colName]);
                msg.Append(ID);
                if ((rows.Count > 1) && (i != rows.Count - 1))
                    msg.Append(",");
            }

            if (!string.IsNullOrEmpty(msg.ToString()))
            {

                string str = string.Empty;
                string updateQry = string.Empty;
                DataSet ds = new DataSet();
                int updated = 0;
                if (isCash)
                {
                    str = (" SELECT PROCESSING, isAllocated  FROM C_CASHLINE  WHERE C_CASHLINE_ID IN (" + msg.ToString() + ") FOR UPDATE ");
                    updateQry = (" UPDATE C_CASHLINE SET PROCESSING ='Y' WHERE C_CASHLINE_ID IN (" + msg.ToString() + ")");
                }
                else if (isInvoice)
                {
                    str = (" SELECT PROCESSING, VA009_ISPAID AS isAllocated FROM C_INVOICEPAYSCHEDULE WHERE C_INVOICEPAYSCHEDULE_ID IN (" + msg.ToString() + ") FOR UPDATE ");
                    updateQry = (" UPDATE C_INVOICEPAYSCHEDULE SET PROCESSING ='Y' WHERE C_INVOICEPAYSCHEDULE_ID IN (" + msg.ToString() + ")");
                }
                else
                {
                    str = (@" SELECT PROCESSING, isAllocated FROM C_Payment WHERE C_PAYMENT_ID IN (" + msg.ToString() + ") FOR UPDATE ");
                    updateQry = (" UPDATE C_Payment SET PROCESSING ='Y' WHERE C_PAYMENT_ID IN (" + msg.ToString() + ")");
                }

                ds = DB.ExecuteDataset(str.ToString(), null, trx);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (Util.GetValueOfString(ds.Tables[0].Rows[i]["PROCESSING"]) == "Y" || Util.GetValueOfString(ds.Tables[0].Rows[i]["isAllocated"]) == "Y")
                        {

                            return Msg.GetMsg(ctx, "VIS_RecordsAlrdyAlocated") + ": " + msg.ToString();
                        }
                        else
                        {
                            updated = DB.ExecuteQuery(updateQry.ToString(), null, trx);
                        }
                    }
                }

            }
            return string.Empty;
        }

        /// <summary>
        /// To set processing column value false 
        /// </summary>        
        /// <param name="rows"> Selected records list </param>
        /// <param name="colName"> column Name </param>
        /// <param name="isCash"> Cash </param>
        /// <param name="isInvoice"> Invoice </param>
        /// <param name="trx"> Trx </param>
        /// <returns>string Value</returns>it'll return either empty string or any error msg
        /// </summary>
        public void SetIsprocessingFalse(List<Dictionary<string, string>> rows, string colName, bool isCash, bool isInvoice, Trx trx)
        {
            StringBuilder msg = new StringBuilder();
            for (int i = 0; i < rows.Count; i++)
            {
                int ID = Util.GetValueOfInt(rows[i][colName]);
                msg.Append(ID);
                if ((rows.Count > 1) && (i != rows.Count - 1))
                    msg.Append(",");
            }

            if (!string.IsNullOrEmpty(msg.ToString()))
            {
                int updated = 0;
                if (isCash)
                {
                    updated = DB.ExecuteQuery(" UPDATE C_CASHLINE SET PROCESSING ='N' WHERE C_CASHLINE_ID IN (" + msg.ToString() + ")", null, trx);
                }
                else if (isInvoice)
                {
                    updated = DB.ExecuteQuery(" UPDATE C_INVOICEPAYSCHEDULE SET PROCESSING ='N' WHERE C_INVOICEPAYSCHEDULE_ID IN (" + msg.ToString() + ")", null, trx);
                }
                else
                {
                    updated = DB.ExecuteQuery(" UPDATE C_Payment SET PROCESSING ='N' WHERE C_PAYMENT_ID IN (" + msg.ToString() + ")", null, trx);
                }
            }
        }

        /// <summary>
        /// to get all the related business partner from business partner relation window
        /// </summary>
        /// <param name="C_BPartner_ID">Business Partner</param>
        /// <returns>business partner ids</returns>
        public string GetRelatedBP(int C_BPartner_ID)
        {
            StringBuilder bpids = new StringBuilder();
            DataSet ds = null;
            ds = DB.ExecuteDataset(@" SELECT C_BPartnerRelation_ID FROM C_BP_Relation WHERE C_BPartner_ID = " + C_BPartner_ID + " AND ispayfrom ='Y' ");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    bpids.Append(Util.GetValueOfString(ds.Tables[0].Rows[i]["C_BPartnerRelation_ID"]));
                    if (ds.Tables[0].Rows.Count > 1 && (i != (ds.Tables[0].Rows.Count - 1)))
                        bpids.Append(" , ");
                }
            }
            return bpids.ToString();
        }

        public class NameValue
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public class VIS_DocType
        {
            public string DocType { get; set; }
            public int C_DocType_ID { get; set; }
        }

        public class VIS_InvoiceData
        {
            public string SelectRow { get; set; }
            public string Date1 { get; set; }
            public string Documentno { get; set; }
            public string CinvoiceID { get; set; }
            public string Isocode { get; set; }
            public string Currency { get; set; }
            public string Converted { get; set; }
            public string Amount { get; set; }
            public string Discount { get; set; }
            public string Multiplierap { get; set; }
            public string DocBaseType { get; set; }
            public string Writeoff { get; set; }
            public string AppliedAmt { get; set; }
            public string C_InvoicePaySchedule_ID { get; set; }
            public string InvoiceScheduleDate { get; set; }
            public int InvoiceRecord { get; set; }
            public int C_ConversionType_ID { get; set; }
            public string ConversionName { get; set; }
            public DateTime? DATEACCT { get; set; }
            public int AD_Org_ID { get; set; }
            public string OrgName { get; set; }
        }

        public class VIS_PaymentData
        {
            public string SelectRow { get; set; }
            public string Date1 { get; set; }
            public string Documentno { get; set; }
            public string CpaymentID { get; set; }
            public string Isocode { get; set; }
            public string Payment { get; set; }
            public string ConvertedAmount { get; set; }
            public string OpenAmt { get; set; }
            public string Multiplierap { get; set; }
            public string AppliedAmt { get; set; }
            public int PaymentRecord { get; set; }
            public int C_ConversionType_ID { get; set; }
            public string ConversionName { get; set; }
            public DateTime? DATEACCT { get; set; }
            public int AD_Org_ID { get; set; }
            public string OrgName { get; set; }
        }

        public class VIS_CashData
        {
            public string SelectRow { get; set; }
            public string Created { get; set; }
            public string ReceiptNo { get; set; }
            public string CcashlineiID { get; set; }
            public string Isocode { get; set; }
            public string Amount { get; set; }
            public string ConvertedAmount { get; set; }
            public string OpenAmt { get; set; }
            public string Multiplierap { get; set; }
            public string AppliedAmt { get; set; }
            public int CashRecord { get; set; }
            public int C_ConversionType_ID { get; set; }
            public string ConversionName { get; set; }
            public DateTime? DATEACCT { get; set; }
            public int AD_Org_ID { get; set; }
            public string OrgName { get; set; }

        }

        //public class PaymentDetails
        //{
        //    public decimal appliedAmt { get; set; }
        //    public decimal discount { get; set; }
        //    public decimal writeoff { get; set; }
        //    public decimal cinvoiceid { get; set; }
        //    public decimal converted { get; set; }
        //    public decimal currency { get; set; }
        //    public string date { get; set; }
        //    public string docbasetype { get; set; }
        //    public decimal documentno { get; set; }
        //    public string isocode { get; set; }
        //    public decimal multiplierap { get; set; }
        //    public decimal openamt { get; set; }
        //    public decimal payment { get; set; }
        //    public int ccashlineid { get; set; }
        //    public int cpaymentid { get; set; }
        //    public int c_invoicepayschedule_id { get; set; }
        //}

    }
}