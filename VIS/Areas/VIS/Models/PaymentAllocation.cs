using System;
using System.Collections;
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
using VAdvantage.ProcessEngine;
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
        /// <param name="rowsCash"> Selected cash line data</param>
        /// <param name="rowsInvoice">Selected invoice data</param>
        /// <param name="currency">Currency ID</param>
        /// <param name="isCash"> bool Value </param>
        /// <param name="_C_BPartner_ID"> Business Partner ID </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="payment"> Payment ID </param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="applied"> Applied Amount </param>
        /// <param name="discount">Discount Amount</param>
        /// <param name="writeOff">Writeoff Amount</param>
        /// <param name="open">Open Amount</param>
        /// <param name="DateAcct">Account Date</param>
        /// <param name="_CurrencyType_ID">Currency ConversionType ID</param>
        /// <param name="isInterBPartner">Inter Business Partner(Yes/No)</param>
        /// <param name="conversionDate"> Conversion Date </param>
        /// <param name="chkMultiCurrency"> bool MultiCurrency </param>
        /// <returns>string either error or empty string</returns>
        public string SaveCashData(List<Dictionary<string, string>> rowsCash, List<Dictionary<string, string>> rowsInvoice, string currency,
            bool isCash, int _C_BPartner_ID, int _windowNo, string payment, DateTime DateTrx, string applied, string discount, string writeOff, string open, DateTime DateAcct, int _CurrencyType_ID, bool isInterBPartner, DateTime conversionDate, bool chkMultiCurrency)
        {
            #region ValidateRecords
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
            //if (paymentData.Count > 0)
            //{
            //    AD_Org_ID = Util.GetValueOfInt(paymentData[0]["Org"]);
            //}
            if (rowsCash.Count > 0)
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

            Trx trx = Trx.GetTrx(Trx.CreateTrxName("AL"));
            // trx.TrxIsolationLevel = IsolationLevel.RepeatableRead;

            //msg = ValidateRecords(paymentData, "cpaymentid", false, false, trx); //Payment
            //if (msg != string.Empty)
            //{
            //    trx.Rollback();
            //    trx.Close();
            //   return msg;
            //}

            msg = ValidateRecords(rowsCash, "ccashlineid", isCash, false, false, trx); //CashLine
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsInvoice, "c_invoicepayschedule_id", false, true, false, trx); //InvoicePaySchedule
            if (msg != string.Empty)
            {
                //set isProcessing false
                Isprocess(null, rowsCash, rowsInvoice, null, trx);
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = string.Empty;

            //Stop Cash to Cash Allocation
            //if (null.Count == 0 && rowsInvoice.Count == 0)
            //{
            //    trx.Rollback();
            //    trx.Close();
            //    return Msg.GetMsg(ctx, "CashToCashAllocationnotpossible");
            //}
            //end

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

            #endregion

            //  CashLines - Loop and Add them to cashList/CashAmountList
            #region CashLines-Loop
            // int cRows = vdgvCashLines.RowCount;
            // IList rowsCash = vdgvCashLine.ItemsSource as IList;

            List<int> cashList = new List<int>(rowsCash.Count);
            List<Decimal> CashAmtList = new List<Decimal>(rowsCash.Count);
            Decimal cashAppliedAmt = Env.ZERO;

            List<Dictionary<string, string>> negInvList = new List<Dictionary<string, string>>();
            Decimal negInvtotAmt = 0;
            //VA228:Variable to hold invoice and invoiceScheduleIds
            List<int> invoiceIds = new List<int>();
            List<int> invoiceScheduleIds = new List<int>();
            if (rowsInvoice.Count != 0)
            {
                foreach (var item in rowsInvoice)
                {
                    if (Util.GetValueOfDecimal(item[applied]) < 0)
                    {
                        negInvList.Add(item);
                        negInvtotAmt = Decimal.Add(negInvtotAmt, Util.GetValueOfDecimal(item[applied]));
                    }
                    //Get invoice ids
                    if (!invoiceIds.Contains(Util.GetValueOfInt(item["cinvoiceid"])))
                        invoiceIds.Add(Util.GetValueOfInt(item["cinvoiceid"]));
                    //Get invoice pay schedule ids
                    invoiceScheduleIds.Add(Util.GetValueOfInt(item["c_invoicepayschedule_id"]));
                }
            }
            List<int> neg_Invoice_IDS = new List<int>(negInvList.Count);
            List<Dictionary<string, string>> negCashList = new List<Dictionary<string, string>>();
            Decimal negCashAmt = 0;
            if (rowsCash.Count != 0)
            {
                foreach (var item in rowsCash)
                {
                    if (Util.GetValueOfDecimal(item[applied]) < 0)
                    {
                        negCashList.Add(item);
                        negCashAmt = Decimal.Add(negCashAmt, Util.GetValueOfDecimal(item[applied]));
                    }
                }
            }
            for (int i = 0; i < rowsCash.Count; i++)
            {
                //  Payment line is selected
                //bool boolValue = false;
                //bool flag = false;
                // if (boolValue)
                {
                    //  Payment variables
                    C_CashLine_ID = Util.GetValueOfInt(rowsCash[i]["ccashlineid"]);
                    cashList.Add(C_CashLine_ID);
                    //Decimal PaymentAmt = Util.GetValueOfDecimal(((BindableObject)rowsCash[i]).GetValue(_payment));  //  Applied Payment
                    Decimal PaymentAmt = Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"]);  //  Applied Payment
                    CashAmtList.Add(PaymentAmt);
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

            #region AllocationHeader
            //	Create Allocation - but don't save yet
            // allocation should be created with current date 
            MAllocationHdr alloc = new MAllocationHdr(ctx, true,	//	manual
                DateTime.Now, C_Currency_ID, ctx.GetContext("#AD_User_Name"), trx);
            alloc.SetAD_Org_ID(AD_Org_ID);
            alloc.SetDateAcct(DateAcct);// to set Account date on allocation header because posting and conversion are calculating on the basis of Date Account
            alloc.SetC_ConversionType_ID(_CurrencyType_ID); // to set Conversion Type on allocation header because posting and conversion are calculating on the basis of Conversion Type
            alloc.SetDateTrx(DateTrx);
            //when select a MultiCurrency then the ConversionDate will set into AllocationHdr
            if (chkMultiCurrency)
            {
                alloc.SetConversionDate(conversionDate);
            }
            #endregion

            //	For all invoices
            int invoiceLines = 0;
            //for (int i = 0; i < rowsCash.Count; i++)
            MInvoicePaySchedule mpay = null;
            int C_InvoicePaySchedule_ID = 0;
            int Neg_C_InvoicePaySchedule_Id = 0;
            bool isScheduleAllocated = false;
            bool is_NegScheduleAllocated = false;

            StringBuilder sbQuery = new StringBuilder();
            DataSet DsInv = null;
            DataSet DsInvSch = null;
            if (rowsInvoice.Count > 0)
            {
                //VA228:Get invoice and invoice pay schedule data
                sbQuery.Append("Select C_Invoice_ID,C_Currency_ID,C_ConversionType_ID,DateAcct,AD_Client_ID,AD_Org_ID from C_Invoice where C_Invoice_ID IN(" + string.Join(",", invoiceIds) + ")");
                DsInv = DB.ExecuteDataset(sbQuery.ToString());

                sbQuery.Clear();
                sbQuery.Append("Select * from C_InvoicePaySchedule where C_InvoicePaySchedule_ID IN(" + string.Join(",", invoiceScheduleIds) + ")");
                DsInvSch = DB.ExecuteDataset(sbQuery.ToString());
            }
            DataSet DsCashLine = null;
            if (rowsCash.Count > 0)
            {
                //VA228:Get cashline data
                sbQuery.Clear();
                sbQuery.Append(@"SELECT CL.C_CashLine_ID,CL.C_Cash_ID,CL.C_ConversionType_ID,C.DateAcct from C_CashLine CL 
                                    INNER JOIN C_Cash C ON C.C_Cash_ID = CL.C_Cash_ID WHERE C_CashLine_ID IN(" + string.Join(",", cashList) + ")");
                DsCashLine = DB.ExecuteDataset(sbQuery.ToString());
            }

            //loop for Invoice to Cash with Invoice to Invoice									
            for (int i = 0; i < rowsInvoice.Count; i++)
            {
                //  Invoice line is selected
                // bool boolValue = false;
                //bool flag = false;
                isScheduleAllocated = false;
                // if (boolValue)
                {
                    int C_Invoice_ID = 0;// Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                    Decimal AppliedAmt = Util.GetValueOfDecimal(rowsInvoice[i][applied]);
                    //  semi-fixed fields (reset after first invoice)
                    Decimal DiscountAmt = Util.GetValueOfDecimal(rowsInvoice[i][discount]);
                    Decimal WriteOffAmt = Util.GetValueOfDecimal(rowsInvoice[i][writeOff]);
                    //	OverUnderAmt needs to be in Allocation Currency
                    Decimal OverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(rowsInvoice[i][open]),
                        Decimal.Add(AppliedAmt, Decimal.Add(DiscountAmt, WriteOffAmt)));
                    Decimal NOverUnderAmt = Env.ZERO;
                    Decimal diffAmt = Env.ZERO;

                    //log.Config("Invoice #" + i + " - AppliedAmt=" + AppliedAmt);// + " -> " + AppliedAbs);

                    //CashLines settelment************
                    //  loop through all payments until invoice applied
                    int noCashlines = 0;
                    MInvoicePaySchedule mpay2 = null;
                    DataRow[] drSch = null;
                    DataRow[] drNegSch = null;
                    DataRow[] drInv = null;
                    DataRow[] drNegInv = null;
                    for (int j = 0; j < cashList.Count && Env.Signum(AppliedAmt) != 0; j++)
                    {
                        //VA228:fetch datarow of invoice and invoicepayschedule
                        drSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                        mpay = new MInvoicePaySchedule(ctx, drSch[0], trx);
                        //Fetch invoice datarow based on invoiceid
                        drInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));
                        //Bypass payschedule condition
                        mpay.ByPassValidatePayScheduleCondition(true);

                        invoiceLines++;
                        ////  Invoice variables
                        C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                        #region cash to invoice matching
                        mpay2 = null;
                        C_CashLine_ID = Util.GetValueOfInt(cashList[j]);
                        //objCashline = new MCashLine(ctx, C_CashLine_ID, trx);
                        DataRow[] drCashLine = DsCashLine.Tables[0].Select("C_CashLine_ID=" + C_CashLine_ID);

                        //cashobj = new MCash(ctx, objCashline.GetC_Cash_ID(), trx);
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
                                //Get Error Message.
                                msg = AllocationHdrFaildToSave(trx);
                                //Set Isprocess false
                                Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                return msg;
                            }

                            //if the invoice amount is +ve check the codition with C_InvoicePaySchedule_ID otherwise check with -ve List
                            if (AppliedAmt > 0)
                            {
                                if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                {
                                    OverUnderAmt = 0;
                                }
                            }
                            else
                            {
                                //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                if (neg_Invoice_IDS.Contains(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"])))
                                {
                                    OverUnderAmt = 0;
                                    isScheduleAllocated = true;
                                }
                            }

                            // when 
                            if (!isScheduleAllocated)
                            {
                                isScheduleAllocated = true;
                                if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                {
                                    var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)), Decimal.Add(DiscountAmt, WriteOffAmt)), C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]), Util.GetValueOfDateTime(drCashLine[0]["DateAcct"]), Util.GetValueOfInt(drCashLine[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                    if (AppliedAmt == amount)
                                    {
                                        //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                        diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                        if (diffAmt != Env.ZERO)
                                        {
                                            mpay.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                    }
                                    else
                                    {
                                        mpay.SetDueAmt(Math.Abs(conertedAmount));
                                    }
                                }
                                else
                                    mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                   Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));
                                if (!mpay.Save(trx))
                                {
                                    msg = ValidateSaveInvoicePaySchedule(trx);
                                    //Set Isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }
                            }
                            // Create New schedule with split 
                            else if (isScheduleAllocated)
                            {
                                mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                PO.CopyValues(mpay, mpay2);
                                //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                //Bypass update paid amount on invoice from invoice pay schedule
                                mpay2.ByPassValidatePayScheduleCondition(true);
                                if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                {
                                    var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]), Util.GetValueOfDateTime(drCashLine[0]["DateAcct"]), Util.GetValueOfInt(drCashLine[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                    if (AppliedAmt == amount)
                                    {
                                        //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                        diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                        mpay2.SetDueAmt(Math.Abs(diffAmt));
                                    }
                                    else
                                    {
                                        mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                    }
                                }
                                else
                                    mpay2.SetDueAmt(Math.Abs(amount));

                                if (!mpay2.Save(trx))
                                {
                                    msg = ValidateSaveInvoicePaySchedule(trx);
                                    //Set isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }
                            }
                            //if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                            //{
                            //    OverUnderAmt = 0;
                            //}
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
                                trx.Rollback();
                                trx.Close();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                                }
                                else
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                                }
                                return msg;
                            }

                            if (AppliedAmt < 0)
                            {
                                neg_Invoice_IDS.Add(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
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
                                CashAmtList[j] = PaymentAmt;  //  update//set
                                rowsCash[j].Remove(payment);
                                rowsCash[j].Add(payment, PaymentAmt.ToString());
                                rowsInvoice[i].Remove(applied);
                                rowsInvoice[i].Add(applied, AppliedAmt.ToString());
                            }

                        }	//	for all applied amounts
                        #endregion
                    }   //	loop through Cash for invoice(Charge)

                    //  No Cashlines allocated and none existing
                    //invoice to invoice allocation when no cashlines											 
                    if (noCashlines == 0 && cashList.Count == 0)
                    {
                        #region when match invoice to invoice
                        C_CashLine_ID = 0;
                        //	Allocation Header
                        if (alloc.Get_ID() == 0 && !alloc.Save())
                        {
                            //Get Error Message.
                            msg = AllocationHdrFaildToSave(trx);
                            //Set Isprocess false
                            Isprocess(null, rowsCash, rowsInvoice, null, trx);
                            return msg;
                        }
                        // invoice to invoice allocation if applied amount is positive 
                        if (AppliedAmt > 0)
                        {
                            Decimal value;
                            MAllocationLine aLine;
                            for (int c = 0; c < negInvList.Count; c++)
                            {
                                //VA228:fetch datarow of invoice and invoicepayschedule
                                drSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                mpay = new MInvoicePaySchedule(ctx, drSch[0], trx);
                                drInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));
                                //Bypass update paid amount on invoice from invoice pay schedule
                                mpay.ByPassValidatePayScheduleCondition(true);
                                Decimal NDiscountAmt = Util.GetValueOfDecimal(negInvList[c][discount]);
                                Decimal NWriteOffAmt = Util.GetValueOfDecimal(negInvList[c][writeOff]);
                                //VA228:Get negative invoice
                                drNegInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(negInvList[c]["cinvoiceid"]));

                                Decimal amount;
                                mpay2 = null;
                                if (AppliedAmt == Env.ZERO)
                                {
                                    break;
                                }
                                Decimal postAppliedAmt = Util.GetValueOfDecimal(negInvList[c][applied]);
                                if (postAppliedAmt != 0)
                                {
                                    value = AppliedAmt - Math.Abs(postAppliedAmt);
                                    if (value >= 0)
                                    {
                                        amount = Math.Abs(postAppliedAmt);
                                    }
                                    else
                                    {
                                        amount = AppliedAmt;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                // when 
                                if (!isScheduleAllocated)
                                {
                                    isScheduleAllocated = true;
                                    if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                        if (AppliedAmt == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                       Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));

                                    if (!mpay.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                }
                                // Create New schedule with split 
                                else if (isScheduleAllocated)
                                {
                                    mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                    PO.CopyValues(mpay, mpay2);
                                    //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                    mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                    mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                    //Bypass update paid amount on invoice from invoice pay schedule
                                    mpay2.ByPassValidatePayScheduleCondition(true);
                                    if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                        if (AppliedAmt == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                            mpay2.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                        else
                                        {
                                            mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(amount));

                                    if (!mpay2.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set Isprocessing false
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }

                                }

                                //new allocation
                                C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                invoiceLines++;

                                //  Invoice variables
                                int Ref_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);

                                if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                {
                                    OverUnderAmt = Env.ZERO;
                                }

                                C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                //allocation for positive Appliedamount Invoice
                                aLine = new MAllocationLine(alloc, amount, DiscountAmt, WriteOffAmt, OverUnderAmt);
                                aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);

                                //get InvoiceSchedule_ID and Initalize to positiveAmtInvSchdle_ID
                                int positiveAmtInvSchdle_ID;
                                if (mpay2 != null)
                                {
                                    positiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                }
                                else
                                {
                                    positiveAmtInvSchdle_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                }
                                //set the trx Date and InvoicePayschedule_ID
                                msg = InvAlloc(C_InvoicePaySchedule_ID, mpay2, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    //Set Isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }
                                int aLine_ID = aLine.GetC_AllocationLine_ID();

                                //VA228:Get negative invoice schedules
                                drNegSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                // VA228:Get InvoicePaySchedule based on datarow
                                mpay = new MInvoicePaySchedule(ctx, drNegSch[0], trx);
                                //Bypass update paid amount on invoice from invoice pay schedule
                                mpay.ByPassValidatePayScheduleCondition(true);
                                mpay2 = null;

                                //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                if (!neg_Invoice_IDS.Contains(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"])))
                                {
                                    //// Updated over/under amount on allocation line, it should be open -( applied + discount + writeoff ) Update by vivek on 05/01/2018 issue reported by Savita
                                    NOverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(negInvList[c][open]),
                                    Decimal.Add(Util.GetValueOfDecimal(negInvList[c][applied]), Decimal.Add(NDiscountAmt, NWriteOffAmt)));
                                    neg_Invoice_IDS.Add(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                    is_NegScheduleAllocated = false;
                                }
                                else
                                {
                                    NOverUnderAmt = Env.ZERO;
                                    is_NegScheduleAllocated = true;
                                }

                                if (!is_NegScheduleAllocated)
                                {
                                    is_NegScheduleAllocated = true;
                                    if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)), Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                        if (Math.Abs(postAppliedAmt) == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)),
                                                       Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))));

                                    if (!mpay.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set isprocessing false
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                }
                                // Create New schedule with split 
                                else if (is_NegScheduleAllocated)
                                {
                                    mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                    PO.CopyValues(mpay, mpay2);
                                    //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                    mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                    mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                    //Bypass update paid amount on invoice from invoice pay schedule
                                    mpay2.ByPassValidatePayScheduleCondition(true);
                                    if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                        if (Math.Abs(postAppliedAmt) == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                            mpay2.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                        else
                                        {
                                            mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(amount));

                                    if (!mpay2.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set isprocessing false
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }

                                }

                                Neg_C_InvoicePaySchedule_Id = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                C_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);
                                invoiceLines++;
                                //  Invoice variables
                                Ref_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                //allocation for negative Amount Invoice
                                aLine = new MAllocationLine(alloc, Decimal.Negate(amount), NDiscountAmt, NWriteOffAmt, NOverUnderAmt);
                                aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);
                                aLine.SetRef_Invoiceschedule_ID(positiveAmtInvSchdle_ID);

                                //get the C_InvoicePaySchedule_ID and Initialize to negtiveAmtInvSchdle_ID
                                int negtiveAmtInvSchdle_ID;
                                if (mpay2 != null)
                                {
                                    negtiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                }
                                else
                                {
                                    negtiveAmtInvSchdle_ID = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                }

                                //set the trx Date and InvoicePayschedule_ID
                                msg = InvAlloc(Neg_C_InvoicePaySchedule_Id, mpay2, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    //set isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }

                                //Updating +ve Invoice allocationLine to set Ref_InvoicePaySchedule_ID
                                aLine = new MAllocationLine(ctx, aLine_ID, trx);
                                aLine.SetRef_Invoiceschedule_ID(negtiveAmtInvSchdle_ID);
                                if (!aLine.Save())
                                {
                                    _log.SaveError("Error: ", "Allocation line Ref_InvoicePaySchedule_ID not updated!");
                                    trx.Rollback();
                                    trx.Close();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null)
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated") + ":- " + pp.GetName();
                                    }
                                    else
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated");
                                    }
                                    //set Isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }
                                //value greater or equal to negative amount then allocating for invoice to invoice
                                if (value >= 0)
                                {
                                    negInvList[c].Remove(applied);
                                    negInvList[c].Add(applied, (0).ToString());
                                    rowsInvoice[i].Remove(applied);
                                    rowsInvoice[i].Add(applied, value.ToString());
                                    AppliedAmt -= Math.Abs(postAppliedAmt);
                                }
                                //value greater than negative amount then allocating for invoice to invoice
                                else
                                {
                                    rowsInvoice[i].Remove(applied);
                                    rowsInvoice[i].Add(applied, (0).ToString());
                                    negInvList[c].Remove(applied);
                                    negInvList[c].Add(applied, value.ToString());
                                    // set  writeoff and  discount amoun t as zero, so that not to include for next iteration
                                    negInvList[c].Remove(discount.ToLower());
                                    negInvList[c].Add(discount.ToLower(), (0).ToString());
                                    negInvList[c].Remove(writeOff.ToLower());
                                    negInvList[c].Add(writeOff.ToLower(), (0).ToString());

                                    AppliedAmt = Env.ZERO;
                                }
                                WriteOffAmt = Env.ZERO;
                                DiscountAmt = Env.ZERO;
                                OverUnderAmt = Env.ZERO;
                                NWriteOffAmt = Env.ZERO;
                                NDiscountAmt = Env.ZERO;
                                NOverUnderAmt = Env.ZERO;
                                // if the difference of positive AppliedAmt and negative AppliedAmt is equal to Zero this block will break the loop
                                if (value == 0)
                                {
                                    break;
                                }
                            }
                        }
                        #endregion
                    }
                    //when we match invoice to invoice and invoice to cash for same schedule 																		 
                    else if (AppliedAmt != 0 && cashList.Count != 0)
                    {
                        #region Invoice to invoice allocation when same matched with cash
                        // when we match invoice to invoice and invoice to cash for same schedule 
                        // then we have to create a new schedule for match invoice to invoice

                        //Validate Allocation Header is save or not
                        if (alloc.Get_ID() == 0 && !alloc.Save())
                        {
                            //Get Error Message.
                            msg = AllocationHdrFaildToSave(trx);
                            //Set Isprocess false
                            Isprocess(null, rowsCash, rowsInvoice, null, trx);
                            return msg;
                        }

                        // invoice to invoice allocation if applied amount is positive 
                        if (AppliedAmt > 0)
                        {
                            Decimal value = 0;
                            MAllocationLine aLine = null;
                            for (int c = 0; c < negInvList.Count; c++)
                            {
                                //mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                                //invoice = new MInvoice(ctx, Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]), trx);

                                //VA228:fetch datarow of invoice and invoicepayschedule
                                drSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                mpay = new MInvoicePaySchedule(ctx, drSch[0], trx);
                                drInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));
                                mpay.ByPassValidatePayScheduleCondition(true);
                                Decimal NDiscountAmt = Util.GetValueOfDecimal(negInvList[c][discount]);
                                Decimal NWriteOffAmt = Util.GetValueOfDecimal(negInvList[c][writeOff]);
                                //MInvoice Neg_invoice = new MInvoice(ctx, Util.GetValueOfInt(negInvList[c]["cinvoiceid"]), trx);
                                drNegInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(negInvList[c]["cinvoiceid"]));

                                Decimal amount = Env.ZERO;
                                mpay2 = null;
                                if (AppliedAmt == Env.ZERO)
                                {
                                    break;
                                }
                                Decimal postAppliedAmt = Util.GetValueOfDecimal(negInvList[c][applied]);
                                if (postAppliedAmt != 0)
                                {
                                    value = AppliedAmt - Math.Abs(postAppliedAmt);
                                    if (value >= 0)
                                    {
                                        amount = Math.Abs(postAppliedAmt);
                                    }
                                    else
                                    {
                                        amount = AppliedAmt;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                // when 
                                if (!isScheduleAllocated)
                                {
                                    isScheduleAllocated = true;
                                    if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                        if (AppliedAmt == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                       Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));
                                    if (!mpay.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set isprocessing false
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }

                                }
                                // Create New schedule with split 
                                else if (isScheduleAllocated)
                                {
                                    mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                    PO.CopyValues(mpay, mpay2);
                                    //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                    mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                    mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                    //VA228:Bypass update paid amount on invoice from invoice pay schedule
                                    mpay2.ByPassValidatePayScheduleCondition(true);
                                    if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                        if (AppliedAmt == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                            mpay2.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                        else
                                        {
                                            mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(amount));

                                    if (!mpay2.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set isprocessing false
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                }

                                //new allocation
                                C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                invoiceLines++;

                                //  Invoice variables
                                int Ref_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);

                                if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                {
                                    OverUnderAmt = Env.ZERO;
                                }

                                C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                //allocation for positive Appliedamount Invoice
                                aLine = new MAllocationLine(alloc, amount, DiscountAmt, WriteOffAmt, OverUnderAmt);

                                aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);

                                //get InvoiceSchedule_ID and Initalize to positiveAmtInvSchdle_ID
                                int positiveAmtInvSchdle_ID;
                                if (mpay2 != null)
                                {
                                    positiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                }
                                else
                                {
                                    positiveAmtInvSchdle_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                }
                                //set the trx Date and InvoicePayschedule_ID
                                msg = InvAlloc(C_InvoicePaySchedule_ID, mpay2, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    //Set isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }
                                int aLine_ID = aLine.GetC_AllocationLine_ID();//get the ID and initialize to aLine_ID
                                // when 
                                //mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]), trx);
                                //mpay2 = null;

                                drNegSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                // VA228:Get InvoicePaySchedule based on datarow
                                mpay = new MInvoicePaySchedule(ctx, drNegSch[0], trx);
                                mpay.ByPassValidatePayScheduleCondition(true);
                                mpay2 = null;

                                //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                if (!neg_Invoice_IDS.Contains(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"])))
                                {
                                    //// Updated over/under amount on allocation line, it should be open -( applied + discount + writeoff ) Update by vivek on 05/01/2018 issue reported by Savita
                                    NOverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(negInvList[c][open]),
                                    Decimal.Add(Util.GetValueOfDecimal(negInvList[c][applied]), Decimal.Add(NDiscountAmt, NWriteOffAmt)));
                                    neg_Invoice_IDS.Add(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                    is_NegScheduleAllocated = false;
                                }
                                else
                                {
                                    NOverUnderAmt = Env.ZERO;
                                    is_NegScheduleAllocated = true;
                                }

                                if (!is_NegScheduleAllocated)
                                {
                                    is_NegScheduleAllocated = true;
                                    if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)), Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                        if (Math.Abs(postAppliedAmt) == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)),
                                                       Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))));
                                    if (!mpay.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //Set Isprocessing false
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                }
                                // Create New schedule with split 
                                else if (is_NegScheduleAllocated)
                                {
                                    mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                    PO.CopyValues(mpay, mpay2);
                                    //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                    mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                    mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                    //VA228:Bypass update paid amount on invoice from invoice pay schedule
                                    mpay2.ByPassValidatePayScheduleCondition(true);
                                    if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                        if (Math.Abs(postAppliedAmt) == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay2.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(amount));

                                    if (!mpay2.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //Set Isprocessing false
                                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                }

                                //new allocation for -ve amount
                                Neg_C_InvoicePaySchedule_Id = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                C_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);
                                invoiceLines++;
                                //  Invoice variables
                                Ref_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                //allocation for negative Amount Invoice
                                aLine = new MAllocationLine(alloc, Decimal.Negate(amount), NDiscountAmt, NWriteOffAmt, NOverUnderAmt);
                                aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);
                                aLine.SetRef_Invoiceschedule_ID(positiveAmtInvSchdle_ID);

                                //get the C_InvoicePaySchedule_ID and Initialize to negtiveAmtInvSchdle_ID
                                int negtiveAmtInvSchdle_ID;
                                if (mpay2 != null)
                                {
                                    negtiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                }
                                else
                                {
                                    negtiveAmtInvSchdle_ID = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                }

                                //set the trx Date and InvoicePayschedule_ID
                                msg = InvAlloc(Neg_C_InvoicePaySchedule_Id, mpay2, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    //Set Isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }

                                //Updating +ve Invoice allocationLine to set Ref_InvoicePaySchedule_ID
                                aLine = new MAllocationLine(ctx, aLine_ID, trx);
                                aLine.SetRef_Invoiceschedule_ID(negtiveAmtInvSchdle_ID);
                                if (!aLine.Save())
                                {
                                    _log.SaveError("Error: ", "Allocation line Ref_InvoicePaySchedule_ID not updated!");
                                    trx.Rollback();
                                    trx.Close();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null)
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated") + ":- " + pp.GetName();
                                    }
                                    else
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated");
                                    }
                                    //set Isprocessing false
                                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                    return msg;
                                }
                                //value greater or equal to negative amount then allocating for invoice to invoice
                                if (value >= 0)
                                {
                                    negInvList[c].Remove(applied);
                                    negInvList[c].Add(applied, (0).ToString());
                                    rowsInvoice[i].Remove(applied);
                                    rowsInvoice[i].Add(applied, value.ToString());
                                    AppliedAmt -= Math.Abs(postAppliedAmt);
                                }
                                //value greater than negative amount then allocating for invoice to invoice
                                else
                                {
                                    rowsInvoice[i].Remove(applied);
                                    rowsInvoice[i].Add(applied, (0).ToString());
                                    negInvList[c].Remove(applied);
                                    negInvList[c].Add(applied, value.ToString());
                                    // set  writeoff and  discount amoun t as zero, so that not to include for next iteration
                                    negInvList[c].Remove(discount.ToLower());
                                    negInvList[c].Add(discount.ToLower(), (0).ToString());
                                    negInvList[c].Remove(writeOff.ToLower());
                                    negInvList[c].Add(writeOff.ToLower(), (0).ToString());
                                    AppliedAmt = Env.ZERO;
                                }
                                //overunder amount for the first time only,for next iteration set to Zero.
                                WriteOffAmt = Env.ZERO;
                                DiscountAmt = Env.ZERO;
                                OverUnderAmt = Env.ZERO;
                                NWriteOffAmt = Env.ZERO;
                                NDiscountAmt = Env.ZERO;
                                NOverUnderAmt = Env.ZERO;
                                // if the difference of positive AppliedAmt and negative AppliedAmt is equal to Zero this block will break the loop
                                if (value == 0)
                                {
                                    break;
                                }
                            }

                        }
                        #endregion
                    }
                    totalAppliedAmt = Decimal.Add(totalAppliedAmt, AppliedAmt);
                    //log.Config("TotalRemaining=" + totalAppliedAmt);
                }   //  invoice selected
            }   //  invoice loop

            #endregion

            #region Allocation Cash Journals
            if ((rowsCash.Count > 0 && Env.Signum(cashAppliedAmt) == 0) || (CashAmtList.Count > 0 && (CashAmtList.Min() != 0 || CashAmtList.Max() != 0))) // Cash Journal TO Cash Journal ALLOCATION WITH INVOICE
            {
                int noCashlines = 0;
                for (int i = 0; i < cashList.Count; i++)
                {
                    Decimal PaymentAmt = Util.GetValueOfDecimal(CashAmtList[i]);

                    //	Allocation Header
                    if (alloc.Get_ID() == 0 && !alloc.Save())
                    {
                        //Get Error Message.
                        msg = AllocationHdrFaildToSave(trx);
                        //Set Isprocess false
                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                        return msg;
                    }

                    if (PaymentAmt > 0)
                    {
                        Decimal value = 0;
                        MAllocationLine aLine = null;
                        for (int c = 0; c < negCashList.Count; c++)
                        {
                            //break the loop if PaymentAmt is Zero
                            if (PaymentAmt == Env.ZERO)
                            {
                                break;
                            }
                            Decimal amount = Env.ZERO;
                            Decimal postAppliedAmt = Util.GetValueOfDecimal(negCashList[c][payment]);
                            if (postAppliedAmt != 0)
                            {
                                value = PaymentAmt - Math.Abs(postAppliedAmt);
                                if (value > 0)
                                {
                                    amount = Math.Abs(postAppliedAmt);
                                }
                                else
                                {
                                    amount = PaymentAmt;
                                }
                            }
                            else
                            {
                                continue;
                            }
                            //new allocation for +ve payment
                            C_CashLine_ID = Util.GetValueOfInt(rowsCash[i]["ccashlineid"]);
                            noCashlines++;
                            int Ref_CashLine_ID = Util.GetValueOfInt(negCashList[c]["ccashlineid"]);

                            //allocation for positive Appliedamount for Payment
                            aLine = new MAllocationLine(alloc, Math.Abs(amount), Env.ZERO, Env.ZERO, Env.ZERO);
                            aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                            aLine.SetPaymentInfo(0, C_CashLine_ID);
                            aLine.SetRef_CashLine_ID(Ref_CashLine_ID);
                            msg = InvAlloc(0, null, aLine, DateTrx, trx);
                            if (msg != string.Empty)
                            {
                                //Set Isprocess false
                                Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                return msg;
                            }

                            //allocation for same amount matched with +ve appliedAmt
                            C_CashLine_ID = Util.GetValueOfInt(negCashList[c]["ccashlineid"]);
                            noCashlines++;
                            Ref_CashLine_ID = Util.GetValueOfInt(rowsCash[i]["ccashlineid"]);

                            //for negative Amount Payment matched with +ve amount
                            aLine = new MAllocationLine(alloc, Decimal.Negate(amount), Env.ZERO, Env.ZERO, Env.ZERO);
                            aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                            aLine.SetPaymentInfo(0, C_CashLine_ID);//set the cashline and Payment to Zero
                            aLine.SetRef_CashLine_ID(Ref_CashLine_ID);
                            msg = InvAlloc(0, null, aLine, DateTrx, trx);
                            if (msg != string.Empty)
                            {
                                //Set Isprocess false
                                Isprocess(null, rowsCash, rowsInvoice, null, trx);
                                return msg;
                            }

                            if (value > 0)
                            {
                                negCashList[c].Remove(payment);
                                negCashList[c].Add(payment, (0).ToString());
                                rowsCash[i].Remove(payment);
                                rowsCash[i].Add(payment, value.ToString());
                                PaymentAmt -= Math.Abs(postAppliedAmt);
                            }
                            else
                            {
                                rowsCash[i].Remove(payment);
                                rowsCash[i].Add(payment, (0).ToString());
                                negCashList[c].Remove(payment);
                                negCashList[c].Add(payment, value.ToString());
                                PaymentAmt = Env.ZERO;
                            }
                            //exist from the loop if value is Zero
                            if (value == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }   //	onlyCashJournals
            #endregion

            //	Should start WF
            if (alloc.Get_ID() != 0)
            {
                CompleteOrReverse(ctx, alloc.Get_ID(), 150, DocActionVariables.ACTION_COMPLETE, trx);
                //alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                if (!alloc.Save())
                {
                    _log.SaveError("Error: ", "Allocation not completed");
                    trx.Rollback();
                    trx.Close();
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        msg = Msg.GetMsg(ctx, "VIS_AllocNotCompleted") + ":- " + pp.GetName();
                    }
                    else
                    {
                        msg = Msg.GetMsg(ctx, "VIS_AllocNotCompleted");
                    }
                    //Set Isprocess false
                    Isprocess(null, rowsCash, rowsInvoice, null, trx);
                    return msg;
                }
                msg = alloc.GetDocumentNo();
            }

            //  Test/Set CashLine is fully allocated
            #region Set CashLine Allocated

            // added by vivek to set isallocated checkbox true on cashline on 06/01/2018
            if (rowsCash.Count > 0)
            {
                for (int i = 0; i < rowsCash.Count; i++)
                {
                    int _cashine_ID = Util.GetValueOfInt(rowsCash[i]["ccashlineid"]);
                    //VA228:Get open amount and removed MCashLine class object
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
                    if (DB.ExecuteQuery("UPDATE C_CashLine set IsAllocated='" + (amtPayment == Env.ZERO ? "Y" : "N") + "' " +
                              "WHERE C_CashLine_ID=" + _cashine_ID, null, trx) <= 0)
                    {
                        _log.SaveError("Error: ", "Cash Line not set allocated");
                        trx.Rollback();
                        trx.Close();
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            msg = Msg.GetMsg(ctx, "VIS_CashLineNotUpdate") + ":- " + pp.GetName();
                        }
                        else
                        {
                            msg = Msg.GetMsg(ctx, "VIS_CashLineNotUpdate");
                        }
                        //Set Isprocess false
                        Isprocess(null, rowsCash, rowsInvoice, null, trx);
                        return msg;
                    }
                    //log.Config("Payment #" + i + (pay.IsAllocated() ? " not" : " is")
                    //    + " fully allocated");
                }
            }
            #endregion

            cashList.Clear();
            CashAmtList.Clear();
            //Set Isprocess false
            Isprocess(null, rowsCash, rowsInvoice, null, trx);
            //SetIsprocessingFalse(paymentData, "cpaymentid", false, false, trx); //Payment
            //SetIsprocessingFalse(rowsCash, "ccashlineid", true, false, trx); //CashLine
            //SetIsprocessingFalse(rowsInvoice, "c_invoicepayschedule_id", false, true, trx); //InvoicePaySchedule
            trx.Commit();
            trx.Close();
            return Msg.GetMsg(ctx, "AllocationCreatedWith") + msg;
        }
      
        /// <summary>
        /// Return Error Meassage if AllocationHdr is not Save
        /// </summary>
        /// <param name="trx">Current Transaction</param>
        /// <returns>string value</returns>
        public string AllocationHdrFaildToSave(Trx trx)
        {
            string msg = string.Empty;
            _log.SaveError("Error: ", "Allocation not created");
            trx.Rollback();
            trx.Close();
            ValueNamePair pp = VLogger.RetrieveError();
            if (pp != null)
            {
                msg = Msg.GetMsg(ctx, "VIS_AllocationHdrNotSaved") + ":- " + pp.GetName();
            }
            else
            {
                msg = Msg.GetMsg(ctx, "VIS_AllocationHdrNotSaved");
            }
            return msg;
        }

        /// <summary>
        /// Validation for InvoicePaySchedule 
        /// </summary>
        /// <param name="trx"></param>
        /// <returns>return Error Message if schedule is not saved</returns>
        public string ValidateSaveInvoicePaySchedule(Trx trx)
        {
            string msg = string.Empty;
            _log.SaveError("Error: ", "Due amount not set on invoice schedule");
            trx.Rollback();
            trx.Close();
            ValueNamePair pp = VLogger.RetrieveError();
            if (pp != null)
            {
                msg = Msg.GetMsg(ctx, "VIS_ScheduleNotUpdate") + ":- " + pp.GetName();
            }
            else
            {
                msg = Msg.GetMsg(ctx, "VIS_ScheduleNotUpdate");
            }
            return msg;
        }

        /// <summary>
        /// set Payment Schedule for Invoices
        /// </summary>
        /// <param name="invoicePaySchedule_ID"></param>
        /// <param name="mpay2"></param>
        /// <param name="aLine"></param>
        /// <param name="DateTrx"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public string InvAlloc(int invoicePaySchedule_ID, MInvoicePaySchedule mpay2, MAllocationLine aLine, DateTime DateTrx, Trx trx)
        {
            //change for Schedule Management
            if (Env.IsModuleInstalled("VA009_"))
            {
                if (mpay2 != null)
                {
                    aLine.SetC_InvoicePaySchedule_ID(mpay2.GetC_InvoicePaySchedule_ID());
                }
                else if (mpay2 == null)
                {
                    aLine.SetC_InvoicePaySchedule_ID(invoicePaySchedule_ID);
                }
            }
            //end

            //to set transaction on allocation line
            aLine.SetDateTrx(DateTrx);

            if (aLine.GetC_Payment_ID() > 0 && (aLine.GetWithholdingAmt() == 0 && aLine.GetBackupWithholdingAmount() == 0))
            {
                // set withholding amount based on porpotionate
                DataSet ds = DB.ExecuteDataset(@"SELECT (SELECT ROUND((" + aLine.GetAmount() + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.C_Withholding_ID ) AS withholdingAmt,
                                                  (SELECT ROUND((" + aLine.GetAmount() + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.BackupWithholding_ID ) AS BackupwithholdingAmt
                                                FROM C_Payment WHERE C_Payment.IsActive   = 'Y' AND C_Payment.C_Payment_ID = " + aLine.GetC_Payment_ID(), null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    aLine.SetWithholdingAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["withholdingAmt"]));
                    aLine.SetBackupWithholdingAmount(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["BackupwithholdingAmt"]));
                }
            }
            string msg = string.Empty;
            if (!aLine.Save())
            {
                _log.SaveError("Error: ", "Allocation line not created");
                trx.Rollback();
                trx.Close();
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                }
                else
                {
                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                }
                return msg;
            }
            return msg;
        }

        /// <summary>
        /// Get the Invoice DueAmount by 
        /// Compareing with GrandTotal or GrandTotalAfterWithHolding Amount 
        /// and Sum of DueAmt from InvoiceSchedule
        /// </summary>
        /// <param name="invoice">MClass of Invoice which is holding Invocie Record Details.</param>
        /// <param name="trx">Currenct Transection</param>
        /// <returns>DueAmount</returns>
        public Decimal GetDifference(MInvoice invoice, Trx trx)
        {
            string sql = "SELECT (CASE "
                + "WHEN i.GrandTotalAfterWithHolding != 0 THEN i.GrandTotalAfterWithHolding "
                + "ELSE i.GrandTotal END) - SUM(ps.DueAmt) AS DiffAmt "
                + "FROM C_InvoicePaySchedule ps INNER JOIN C_Invoice i ON ps.C_Invoice_ID = i.C_Invoice_ID "
                + "WHERE i.C_Invoice_ID =" + invoice.GetC_Invoice_ID() + " "
                + "GROUP BY CASE WHEN i.GrandTotalAfterWithHolding != 0 THEN i.GrandTotalAfterWithHolding ELSE i.GrandTotal END";
            return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trx));
        }
        /// <summary>
        /// Get the Invoice DueAmount by 
        /// Compareing with GrandTotal or GrandTotalAfterWithHolding Amount 
        /// and Sum of DueAmt from InvoiceSchedule
        /// </summary>
        /// <param name="invoiceId">InvoiceId which is holding Invoice Record Details.</param>
        /// <param name="trx">Currenct Transection</param>
        /// <returns>DueAmount</returns>
        public Decimal GetDifference(int invoiceId, Trx trx)
        {
            string sql = "SELECT (CASE "
                + "WHEN i.GrandTotalAfterWithHolding != 0 THEN i.GrandTotalAfterWithHolding "
                + "ELSE i.GrandTotal END) - SUM(ps.DueAmt) AS DiffAmt "
                + "FROM C_InvoicePaySchedule ps INNER JOIN C_Invoice i ON ps.C_Invoice_ID = i.C_Invoice_ID "
                + "WHERE i.C_Invoice_ID =" + invoiceId + " "
                + "GROUP BY CASE WHEN i.GrandTotalAfterWithHolding != 0 THEN i.GrandTotalAfterWithHolding ELSE i.GrandTotal END";
            return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trx));
        }
        /// <summary>
        /// set IsprocessingFalse for grid's
        /// </summary>
        /// <param name="rowsPayment">list of Payment Records</param>
        /// <param name="rowsCash">list of Cash Journal Records</param>
        /// <param name="rowsInvoice">list of Invoice Records</param>
        /// <param name="trx">current transaction</param>
        public void Isprocess(List<Dictionary<string, string>> rowsPayment, List<Dictionary<string, string>> rowsCash, List<Dictionary<string, string>> rowsInvoice, List<Dictionary<string, string>> rowsGL, Trx trx)
        {
            SetIsprocessingFalse(rowsPayment, "cpaymentid", false, false, true, trx); //Payment
            SetIsprocessingFalse(rowsCash, "ccashlineid", true, false, false, trx); //CashLine
            SetIsprocessingFalse(rowsInvoice, "c_invoicepayschedule_id", false, true, false, trx); //InvoicePaySchedule
            SetIsprocessingFalse(rowsGL, "GL_Journal_ID", false, false, false, trx); //GL_Journal
        }
        /// <summary>
        /// to create view allocation against Payment
        /// </summary>
        /// <param name="rowsPayment">Selected payment data</param>
        /// <param name="rowsInvoice">Selected invoice data</param>
        /// <param name="currency">Currency ID</param>
        /// <param name="_C_BPartner_ID"> Business Partner ID </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="payment"> Payment ID </param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="applied"> Applied Amount </param>
        /// <param name="discount">Discount Amount</param>
        /// <param name="writeOff">Writeoff Amount</param>
        /// <param name="open">Open Amount</param>
        /// <param name="DateAcct">Account Date</param>
        /// <param name="_CurrencyType_ID">Currency ConversionType ID</param>
        /// <param name="isInterBPartner">Inter Business Partner(Yes/No)</param>
        /// <param name="conversionDate"> conversion Date </param>
        /// <param name="chkMultiCurrency"> is MultiCurrency </param>
        /// <returns>string either error or empty string</returns>
        public string SavePaymentData(List<Dictionary<string, string>> rowsPayment, List<Dictionary<string, string>> rowsInvoice, string currency,
   int _C_BPartner_ID, int _windowNo, string payment, DateTime DateTrx, string applied, string discount, string writeOff, string open, DateTime DateAcct, int _CurrencyType_ID, bool isInterBPartner, DateTime conversionDate, bool chkMultiCurrency)
        {
            #region ValidateRecords
            //  fixed fields
            int AD_Client_ID = ctx.GetContextAsInt(_windowNo, "AD_Client_ID");
            int AD_Org_ID = ctx.GetContextAsInt(_windowNo, "AD_Org_ID");
            int C_BPartner_ID = _C_BPartner_ID;
            int C_Order_ID = 0;
            string msg = string.Empty;
            Trx trx = Trx.GetTrx(Trx.CreateTrxName("AL"));

            msg = ValidateRecords(rowsPayment, "cpaymentid", false, false, true, trx); //Payment
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            //msg = ValidateRecords(rowsCash, "ccashlineid", true, false, trx); //CashLine
            //if (msg != string.Empty)
            //{
            //    trx.Rollback();
            //    trx.Close();
            //    return msg;
            //}

            msg = ValidateRecords(rowsInvoice, "c_invoicepayschedule_id", false, true, false, trx); //InvoicePaySchedule
            if (msg != string.Empty)
            {
                //set isProcessing false
                Isprocess(rowsPayment, null, rowsInvoice, null, trx);
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
            //else if (rowsCash.Count > 0)
            //{
            //    AD_Org_ID = Util.GetValueOfInt(rowsCash[0]["Org"]);
            //}
            else if (rowsInvoice.Count > 0)
            {
                AD_Org_ID = Util.GetValueOfInt(rowsInvoice[0]["Org"]);
            }
            else
            {
                //set isProcessing false
                Isprocess(rowsPayment, null, rowsInvoice, null, trx);
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

            #endregion
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

                #region AllocationHeader
                //	Create Allocation - but don't save yet
                // to be save Current date on allocation -- not to pick either payment or schedule date (on behalf of mukesh sir)
                MAllocationHdr alloc = new MAllocationHdr(ctx, true,	//	manual
                    DateTime.Now, C_Currency_ID, ctx.GetContext("#AD_User_Name"), trx);
                alloc.SetAD_Org_ID(AD_Org_ID);
                //to set transaction and account date on allocation header
                alloc.SetDateAcct(DateAcct);// to set Account date on allocation header because posting and conversion are calculating on the basis of Date Account
                alloc.SetC_ConversionType_ID(_CurrencyType_ID); // to set Conversion Type on allocation header because posting and conversion are calculating on the basis of Conversion Type
                alloc.SetDateTrx(DateTrx);
                //when select a MultiCurrency then the ConversionDate will set into AllocationHdr
                if (chkMultiCurrency)
                {
                    alloc.SetConversionDate(conversionDate);
                }
                #endregion

                int C_InvoicePaySchedule_ID = 0;
                int Neg_C_InvoicePaySchedule_Id = 0;

                //	For all invoices
                int invoiceLines = 0;
                MInvoicePaySchedule mpay = null;
                bool isScheduleAllocated = false;
                bool is_NegScheduleAllocated = false;

                // seprate list for negative Value Invoices
                List<Dictionary<string, string>> negInvList = new List<Dictionary<string, string>>();
                Decimal negInvtotAmt = 0;
                //List<Dictionary<string, string>> positiveInvList = new List<Dictionary<string, string>>();

                //VA228:Variable to hold invoice and invoiceScheduleIds
                List<int> invoiceIds = new List<int>();
                List<int> invoiceScheduleIds = new List<int>();
                if (rowsInvoice.Count != 0)
                {
                    foreach (var item in rowsInvoice)
                    {
                        if (Util.GetValueOfDecimal(item[applied.ToLower()]) < 0)
                        {
                            negInvList.Add(item);
                            negInvtotAmt = Decimal.Add(negInvtotAmt, Util.GetValueOfDecimal(item[applied.ToLower()]));
                        }
                        //Get invoice ids
                        if (!invoiceIds.Contains(Util.GetValueOfInt(item["cinvoiceid"])))
                            invoiceIds.Add(Util.GetValueOfInt(item["cinvoiceid"]));
                        //Get invoice pay schedule ids
                        invoiceScheduleIds.Add(Util.GetValueOfInt(item["c_invoicepayschedule_id"]));
                    }
                }
                // seprate list for negative Value Payments
                List<Dictionary<string, string>> negPayList = new List<Dictionary<string, string>>();

                //List<Dictionary<string, string>> positiveInvList = new List<Dictionary<string, string>>();
                if (rowsPayment.Count != 0)
                {
                    foreach (var item in rowsPayment)
                    {
                        if (Util.GetValueOfDecimal(item[payment.ToLower()]) < 0)
                        {
                            negPayList.Add(item);
                        }
                    }
                }

                StringBuilder sbQuery = new StringBuilder();
                DataSet DsInv = null;
                DataSet DsInvSch = null;
                if (rowsInvoice.Count > 0)
                {
                    //VA228:Get invoice and invoice pay schedule data
                    sbQuery.Append("Select C_Invoice_ID,C_Currency_ID,C_ConversionType_ID,DateAcct,AD_Client_ID,AD_Org_ID from C_Invoice where C_Invoice_ID IN(" + string.Join(",", invoiceIds) + ")");
                    DsInv = DB.ExecuteDataset(sbQuery.ToString());

                    sbQuery.Clear();
                    sbQuery.Append("Select * from C_InvoicePaySchedule where C_InvoicePaySchedule_ID IN(" + string.Join(",", invoiceScheduleIds) + ")");
                    DsInvSch = DB.ExecuteDataset(sbQuery.ToString());
                }
                DataSet DsPayment = null;
                if (rowsPayment.Count > 0)
                {
                    //VA228:Get Payment data
                    sbQuery.Clear();
                    sbQuery.Append(@"SELECT P.C_Payment_ID,P.C_ConversionType_ID,P.DateAcct,P.C_Withholding_ID,P.BackupWithholding_ID,WH.PayPercentage,BWH.PayPercentage AS BackupPayPercentage 
                                        FROM C_Payment P 
                                        LEFT JOIN C_Withholding WH ON WH.C_Withholding_ID=P.C_Withholding_ID
                                        LEFT JOIN C_Withholding BWH ON BWH.C_Withholding_ID=P.BackupWithholding_ID 
                                        WHERE P.C_Payment_ID IN(" + string.Join(",", paymentList) + ")");
                    DsPayment = DB.ExecuteDataset(sbQuery.ToString());
                }
                List<int> neg_Invoice_IDS = new List<int>(negPayList.Count);

                // loop for invoices with payments
                for (int i = 0; i < rowsInvoice.Count; i++)
                {
                    //  Invoice line is selected
                    isScheduleAllocated = false;
                    {
                        //mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                        //invoice = new MInvoice(ctx, Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]), trx);
                        //invoiceLines++;
                        ////  Invoice variables
                        int C_Invoice_ID = 0;// Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                        Decimal AppliedAmt = Util.GetValueOfDecimal(rowsInvoice[i][applied.ToLower()]);
                        Decimal DiscountAmt = Util.GetValueOfDecimal(rowsInvoice[i][discount.ToLower()]);
                        Decimal WriteOffAmt = Util.GetValueOfDecimal(rowsInvoice[i][writeOff.ToLower()]);
                        Decimal NOverUnderAmt = Env.ZERO;
                        Decimal diffAmt = Env.ZERO;

                        // Updated over/under amount on allocation line, it should be open -( applied + discount + writeoff ) Update by vivek on 05/01/2018 issue reported by Savita
                        Decimal OverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(rowsInvoice[i][open]),
                        Decimal.Add(AppliedAmt, Decimal.Add(DiscountAmt, WriteOffAmt)));

                        //Payment Settelment**********
                        //  loop through all payments until invoice applied
                        int noPayments = 0;
                        MInvoicePaySchedule mpay2 = null;
                        DataRow[] drSch = null;
                        DataRow[] drNegSch = null;
                        DataRow[] drInv = null;
                        DataRow[] drNegInv = null;

                        #region PaymentLoop
                        for (int j = 0; j < paymentList.Count && Env.Signum(AppliedAmt) != 0; j++)
                        {
                            //VA228:fetch datarow of invoice and invoicepayschedule
                            drSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                            mpay = new MInvoicePaySchedule(ctx, drSch[0], trx);
                            //Fetch invoice datarow based on invoiceid
                            drInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));
                            //Bypass payschedule condition
                            mpay.ByPassValidatePayScheduleCondition(true);

                            invoiceLines++;
                            ////  Invoice variables
                            C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                            #region payment match
                            mpay2 = null;
                            int C_Payment_ID = Util.GetValueOfInt(paymentList[j]);
                            //VA228:Get Payment record based on payment id
                            DataRow[] drPayment = DsPayment.Tables[0].Select("C_Payment_ID=" + C_Payment_ID);
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

                                //if the invoice amount is +ve check the codition with C_InvoicePaySchedule_ID otherwise check with -ve List
                                if (AppliedAmt > 0)
                                {
                                    if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                    {
                                        OverUnderAmt = 0;
                                    }
                                }
                                else
                                {
                                    //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                    if (neg_Invoice_IDS.Contains(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"])))
                                    {
                                        OverUnderAmt = 0;
                                        isScheduleAllocated = true;
                                    }
                                }
                                // when 
                                if (!isScheduleAllocated)
                                {
                                    isScheduleAllocated = true;
                                    if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]), Util.GetValueOfDateTime(drPayment[0]["DateAcct"]), Util.GetValueOfInt(drPayment[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                        if (AppliedAmt == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                       Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));
                                    if (!mpay.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set isProcessing false
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                }
                                // Create New schedule with split 
                                else if (isScheduleAllocated)
                                {
                                    mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                    PO.CopyValues(mpay, mpay2);
                                    //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                    mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                    mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                    //Bypass update paid amount on invoice from invoice pay schedule
                                    mpay2.ByPassValidatePayScheduleCondition(true);

                                    if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]), Util.GetValueOfDateTime(drPayment[0]["DateAcct"]), Util.GetValueOfInt(drPayment[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                        if (AppliedAmt == amount)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                            mpay2.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                        else
                                        {
                                            mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(amount));

                                    if (!mpay2.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                }

                                //	Allocation Header
                                _log.SaveError("First Allocation Save Start", "First Allocation Save Start");
                                if (alloc.Get_ID() == 0 && !alloc.Save())
                                {
                                    //return Error Meassage
                                    msg = AllocationHdrFaildToSave(trx);
                                    //set Isprocess false
                                    Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                    return msg;
                                }
                                _log.SaveError("First Allocation Saved", "First Allocation Saved");

                                //if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                //{
                                //    OverUnderAmt = 0;
                                //}

                                C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);

                                //	Allocation Line
                                MAllocationLine aLine = new MAllocationLine(alloc, amount,
                                    DiscountAmt, WriteOffAmt, OverUnderAmt);
                                aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);

                                // set withholding amount based on porpotionate
                                if (Util.GetValueOfInt(drPayment[0]["C_Withholding_ID"]) > 0 || Util.GetValueOfInt(drPayment[0]["BackupWithholding_ID"]) > 0)
                                {
                                    //VA228:Calculate withholding and backupwithholding amount
                                    decimal withholdingAmt = (amount * Util.GetValueOfDecimal(drPayment[0]["PayPercentage"])) / 100;
                                    decimal backupWithholdingAmt = (amount * Util.GetValueOfDecimal(drPayment[0]["BackupPayPercentage"])) / 100;
                                    aLine.SetWithholdingAmt(Math.Round(withholdingAmt, 2, MidpointRounding.AwayFromZero));
                                    aLine.SetBackupWithholdingAmount(Math.Round(backupWithholdingAmt, 2, MidpointRounding.AwayFromZero));

                                    //DataSet ds = DB.ExecuteDataset(@"SELECT (SELECT ROUND((" + amount + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                    //              FROM C_Withholding WHERE C_Withholding_ID = C_Payment.C_Withholding_ID ) AS withholdingAmt,
                                    //              (SELECT ROUND((" + amount + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                    //              FROM C_Withholding WHERE C_Withholding_ID = C_Payment.BackupWithholding_ID ) AS BackupwithholdingAmt
                                    //            FROM C_Payment WHERE C_Payment.IsActive   = 'Y' AND C_Payment.C_Payment_ID = " + C_Payment_ID, null, null);
                                    //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                    //{
                                    //    aLine.SetWithholdingAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["withholdingAmt"]));
                                    //    aLine.SetBackupWithholdingAmount(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["BackupwithholdingAmt"]));
                                    //}
                                }

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
                                    _log.SaveError("Error: ", "Allocation line not created");
                                    trx.Rollback();
                                    trx.Close();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null)
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                                    }
                                    else
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                                    }
                                    //set Isprocessing false
                                    Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                    return msg;
                                }

                                //if the amount is -ve then the id will add in this list for Set OverUnderAmt for invoice to invoice allocation for -ve amount 
                                if (AppliedAmt < 0)
                                {
                                    neg_Invoice_IDS.Add(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
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
                                rowsPayment[j].Remove(payment.ToLower());
                                rowsPayment[j].Add(payment.ToLower(), PaymentAmt.ToString());
                                rowsInvoice[i].Remove(applied.ToLower());
                                rowsInvoice[i].Add(applied.ToLower(), AppliedAmt.ToString());
                            }	//	for all applied amounts

                            // MPayment pay1 = new MPayment(ctx, C_Payment_ID, trx);
                            #endregion
                        }   //	loop through payments for invoice
                        #endregion
                        //  No Payments allocated and none existing (e.g. Inv/CM)
                        _log.SaveError("Loop Completed", "Loop Completed");

                        //loop for invoice to invoice
                        if (noPayments == 0 && paymentList.Count == 0)
                        {
                            #region when match invoice to invoice
                            //int C_Payment_ID = 0;

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                //Get Error Message.
                                msg = AllocationHdrFaildToSave(trx);
                                //Set Isprocess false
                                Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                return msg;
                            }
                            // invoice to invoice allocation if applied amount is positive 
                            if (AppliedAmt > 0)
                            {
                                Decimal value = 0;
                                MAllocationLine aLine = null;
                                for (int c = 0; c < negInvList.Count; c++)
                                {
                                    //VA228:fetch datarow of invoice and invoicepayschedule
                                    drSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                    mpay = new MInvoicePaySchedule(ctx, drSch[0], trx);
                                    drInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));
                                    //Bypass update paid amount on invoice from invoice pay schedule
                                    mpay.ByPassValidatePayScheduleCondition(true);

                                    Decimal NDiscountAmt = Util.GetValueOfDecimal(negInvList[c][discount.ToLower()]);
                                    Decimal NWriteOffAmt = Util.GetValueOfDecimal(negInvList[c][writeOff.ToLower()]);
                                    drNegInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(negInvList[c]["cinvoiceid"]));

                                    Decimal amount = Env.ZERO;
                                    mpay2 = null;
                                    if (AppliedAmt == Env.ZERO)
                                    {
                                        break;
                                    }
                                    Decimal postAppliedAmt = Util.GetValueOfDecimal(negInvList[c][applied.ToLower()]);
                                    if (postAppliedAmt != 0)
                                    {
                                        value = AppliedAmt - Math.Abs(postAppliedAmt);
                                        if (value >= 0)
                                        {
                                            amount = Math.Abs(postAppliedAmt);
                                        }
                                        else
                                        {
                                            amount = AppliedAmt;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    // when 
                                    if (!isScheduleAllocated)
                                    {
                                        isScheduleAllocated = true;

                                        if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                            if (AppliedAmt == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                                if (diffAmt != Env.ZERO)
                                                {
                                                    mpay.SetDueAmt(Math.Abs(diffAmt));
                                                }
                                            }
                                            else
                                            {
                                                mpay.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                           Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));

                                        if (!mpay.Save(trx))
                                        {
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }
                                    }
                                    // Create New schedule with split 
                                    else if (isScheduleAllocated)
                                    {
                                        mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                        PO.CopyValues(mpay, mpay2);
                                        //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                        mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                        mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                        mpay2.ByPassValidatePayScheduleCondition(true);
                                        if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                            //Decimal withHoldingAmt = invoice.GetGrandTotalAfterWithholding();
                                            if (AppliedAmt == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                                mpay2.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                            else
                                            {
                                                mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay2.SetDueAmt(Math.Abs(amount));

                                        if (!mpay2.Save(trx))
                                        {
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }

                                    }

                                    //new allocation
                                    C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                    invoiceLines++;

                                    //  Invoice variables
                                    int Ref_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);

                                    if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                    {
                                        OverUnderAmt = Env.ZERO;
                                    }

                                    C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                    //allocation for positive Appliedamount Invoice
                                    aLine = new MAllocationLine(alloc, amount, DiscountAmt, WriteOffAmt, OverUnderAmt);
                                    aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                    aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);
                                    //aLine.SetRef_Invoiceschedule_ID(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                    int positiveAmtInvSchdle_ID = 0;
                                    if (mpay2 != null)
                                    {
                                        positiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                    }
                                    else
                                    {
                                        positiveAmtInvSchdle_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                    }
                                    //set the trx Date and InvoicePayschedule_ID
                                    msg = InvAlloc(C_InvoicePaySchedule_ID, mpay2, aLine, DateTrx, trx);
                                    if (msg != string.Empty)
                                    {
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                    int aLine_ID = aLine.GetC_AllocationLine_ID();
                                    drNegSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                    // VA228:Get InvoicePaySchedule based on datarow
                                    mpay = new MInvoicePaySchedule(ctx, drNegSch[0], trx);
                                    //Bypass update paid amount on invoice from invoice pay schedule
                                    mpay.ByPassValidatePayScheduleCondition(true);
                                    mpay2 = null;

                                    //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                    if (!neg_Invoice_IDS.Contains(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"])))
                                    {
                                        //// Updated over/under amount on allocation line, it should be open -( applied + discount + writeoff ) Update by vivek on 05/01/2018 issue reported by Savita
                                        NOverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(negInvList[c][open]),
                                        Decimal.Add(Util.GetValueOfDecimal(negInvList[c][applied.ToLower()]), Decimal.Add(NDiscountAmt, NWriteOffAmt)));
                                        neg_Invoice_IDS.Add(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                        is_NegScheduleAllocated = false;
                                    }
                                    else
                                    {
                                        NOverUnderAmt = Env.ZERO;
                                        is_NegScheduleAllocated = true;
                                    }

                                    if (!is_NegScheduleAllocated)
                                    {
                                        is_NegScheduleAllocated = true;
                                        if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)), Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                            if (Math.Abs(postAppliedAmt) == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                                if (diffAmt != Env.ZERO)
                                                {
                                                    mpay.SetDueAmt(Math.Abs(diffAmt));
                                                }
                                            }
                                            else
                                            {
                                                mpay.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)),
                                                           Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))));

                                        if (!mpay.Save(trx))
                                        {
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }
                                    }
                                    // Create New schedule with split 
                                    else if (is_NegScheduleAllocated)
                                    {
                                        mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                        PO.CopyValues(mpay, mpay2);
                                        //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                        mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                        mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                        mpay2.ByPassValidatePayScheduleCondition(true);

                                        if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                            if (Math.Abs(postAppliedAmt) == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                                mpay2.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                            else
                                            {
                                                mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay2.SetDueAmt(Math.Abs(amount));

                                        if (!mpay2.Save(trx))
                                        {
                                            //Get Error Message
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            //Set Isprocessing false
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }

                                    }
                                    //new allocation for -ve amount

                                    Neg_C_InvoicePaySchedule_Id = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                    C_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);
                                    invoiceLines++;
                                    //  Invoice variables
                                    Ref_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                    //allocation for negative Amount Invoice
                                    aLine = new MAllocationLine(alloc, Decimal.Negate(amount), NDiscountAmt, NWriteOffAmt, NOverUnderAmt);
                                    aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                    aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);
                                    aLine.SetRef_Invoiceschedule_ID(positiveAmtInvSchdle_ID);

                                    //get the InvoicePaySchedule_ID and initilaze to negtiveAmtInvSchdle_ID
                                    int negtiveAmtInvSchdle_ID = 0;
                                    if (mpay2 != null)
                                    {
                                        negtiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                    }
                                    else
                                    {
                                        negtiveAmtInvSchdle_ID = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                    }

                                    //set the trx Date and InvoicePayschedule_ID
                                    msg = InvAlloc(Neg_C_InvoicePaySchedule_Id, mpay2, aLine, DateTrx, trx);
                                    if (msg != string.Empty)
                                    {
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }

                                    //Updating +ve Invoice allocationLine to set Ref_InvoicePaySchedule_ID
                                    aLine = new MAllocationLine(ctx, aLine_ID, trx);
                                    aLine.SetRef_Invoiceschedule_ID(negtiveAmtInvSchdle_ID);
                                    if (!aLine.Save())
                                    {
                                        _log.SaveError("Error: ", "Allocation line Ref_InvoicePaySchedule_ID not updated!");
                                        trx.Rollback();
                                        trx.Close();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null)
                                        {
                                            msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated") + ":- " + pp.GetName();
                                        }
                                        else
                                        {
                                            msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated");
                                        }
                                        //set Isprocessing false
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }

                                    //value greater or equal to negative amount then allocating for invoice to invoice
                                    if (value >= 0)
                                    {
                                        negInvList[c].Remove(applied.ToLower());
                                        negInvList[c].Add(applied.ToLower(), (0).ToString());
                                        rowsInvoice[i].Remove(applied.ToLower());
                                        rowsInvoice[i].Add(applied.ToLower(), value.ToString());
                                        AppliedAmt -= Math.Abs(postAppliedAmt);
                                    }
                                    //value greater than negative amount then allocating for invoice to invoice
                                    else
                                    {
                                        rowsInvoice[i].Remove(applied.ToLower());
                                        rowsInvoice[i].Add(applied.ToLower(), (0).ToString());
                                        negInvList[c].Remove(applied.ToLower());
                                        negInvList[c].Add(applied.ToLower(), value.ToString());
                                        // set  writeoff and  discount amoun t as zero, so that not to include for next iteration
                                        negInvList[c].Remove(discount.ToLower());
                                        negInvList[c].Add(discount.ToLower(), (0).ToString());
                                        negInvList[c].Remove(writeOff.ToLower());
                                        negInvList[c].Add(writeOff.ToLower(), (0).ToString());

                                        AppliedAmt = Env.ZERO;
                                    }
                                    WriteOffAmt = Env.ZERO;
                                    DiscountAmt = Env.ZERO;
                                    OverUnderAmt = Env.ZERO;
                                    NWriteOffAmt = Env.ZERO;
                                    NDiscountAmt = Env.ZERO;
                                    NOverUnderAmt = Env.ZERO;
                                    // if the difference of positive AppliedAmt and negative AppliedAmt is equal to Zero this block will break the loop
                                    if (value == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (AppliedAmt != 0 && paymentList.Count != 0)
                        {
                            #region Invoice to invoice allocation when same matched with payment
                            // when we match invoice to invoice and invoice to payment for same schedule 
                            // then we have to create a new schedule for match invoice to invoice
                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                //Get Error Message.
                                msg = AllocationHdrFaildToSave(trx);
                                //Set Isprocess false
                                Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                return msg;
                            }

                            // invoice to invoice allocation if applied amount is positive 
                            if (AppliedAmt > 0)
                            {
                                Decimal value = 0;
                                MAllocationLine aLine = null;
                                for (int c = 0; c < negInvList.Count; c++)
                                {
                                    //VA228:fetch datarow of invoice and invoicepayschedule
                                    drSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                    mpay = new MInvoicePaySchedule(ctx, drSch[0], trx);
                                    drInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));
                                    mpay.ByPassValidatePayScheduleCondition(true);

                                    //mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                                    //invoice = new MInvoice(ctx, Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]), trx);
                                    Decimal NDiscountAmt = Util.GetValueOfDecimal(negInvList[c][discount.ToLower()]);
                                    Decimal NWriteOffAmt = Util.GetValueOfDecimal(negInvList[c][writeOff.ToLower()]);
                                    //MInvoice Neg_invoice = new MInvoice(ctx, Util.GetValueOfInt(negInvList[c]["cinvoiceid"]), trx);
                                    drNegInv = DsInv.Tables[0].Select("c_invoice_id=" + Util.GetValueOfInt(negInvList[c]["cinvoiceid"]));

                                    Decimal amount = Env.ZERO;
                                    mpay2 = null;
                                    if (AppliedAmt == Env.ZERO)
                                    {
                                        break;
                                    }
                                    Decimal postAppliedAmt = Util.GetValueOfDecimal(negInvList[c][applied.ToLower()]);
                                    if (postAppliedAmt != 0)
                                    {
                                        value = AppliedAmt - Math.Abs(postAppliedAmt);
                                        if (value >= 0)
                                        {
                                            amount = Math.Abs(postAppliedAmt);
                                        }
                                        else
                                        {
                                            amount = AppliedAmt;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    // when 
                                    if (!isScheduleAllocated)
                                    {
                                        isScheduleAllocated = true;
                                        if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                            if (AppliedAmt == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                                if (diffAmt != Env.ZERO)
                                                {
                                                    mpay.SetDueAmt(Math.Abs(diffAmt));
                                                }
                                            }
                                            else
                                            {
                                                mpay.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(OverUnderAmt)),
                                                           Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));

                                        if (!mpay.Save(trx))
                                        {
                                            //Get Error Message
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            //Set Isprocessing false
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }
                                    }
                                    // Create New schedule with split 
                                    else if (isScheduleAllocated)
                                    {
                                        mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                        PO.CopyValues(mpay, mpay2);
                                        //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                        mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                        mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                        mpay2.ByPassValidatePayScheduleCondition(true);

                                        if (Util.GetValueOfInt(drInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drNegInv[0]["DateAcct"])), Util.GetValueOfInt(drNegInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drInv[0]["AD_Org_ID"]));
                                            if (AppliedAmt == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drInv[0]["C_Invoice_ID"]), trx);
                                                mpay2.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                            else
                                            {
                                                mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay2.SetDueAmt(Math.Abs(amount));

                                        if (!mpay2.Save(trx))
                                        {
                                            //Get Error message
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            //Set Isprocesssing false
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }

                                    }

                                    //new allocation
                                    C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                    invoiceLines++;

                                    //  Invoice variables
                                    int Ref_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);

                                    if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                    {
                                        OverUnderAmt = Env.ZERO;
                                    }

                                    C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                    //allocation for positive Appliedamount Invoice
                                    aLine = new MAllocationLine(alloc, amount, DiscountAmt, WriteOffAmt, OverUnderAmt);
                                    aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                    aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);

                                    //get InvoiceSchedule_ID and Initalize to positiveAmtInvSchdle_ID
                                    int positiveAmtInvSchdle_ID = 0;
                                    if (mpay2 != null)
                                    {
                                        positiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                    }
                                    else
                                    {
                                        positiveAmtInvSchdle_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                    }

                                    //set the trx Date and InvoicePayschedule_ID
                                    msg = InvAlloc(C_InvoicePaySchedule_ID, mpay2, aLine, DateTrx, trx);
                                    if (msg != string.Empty)
                                    {
                                        //Set Isprocessing false
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }
                                    //get allocationLine_ID and Inilizing to aLine_ID
                                    int aLine_ID = aLine.GetC_AllocationLine_ID();

                                    drNegSch = DsInvSch.Tables[0].Select("c_invoicepayschedule_id=" + Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                    // VA228:Get InvoicePaySchedule based on datarow
                                    mpay = new MInvoicePaySchedule(ctx, drNegSch[0], trx);
                                    mpay.ByPassValidatePayScheduleCondition(true);
                                    mpay2 = null;

                                    //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                    if (!neg_Invoice_IDS.Contains(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"])))
                                    {
                                        //// Updated over/under amount on allocation line, it should be open -( applied + discount + writeoff ) Update by vivek on 05/01/2018 issue reported by Savita
                                        NOverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(negInvList[c][open]),
                                        Decimal.Add(Util.GetValueOfDecimal(negInvList[c][applied.ToLower()]), Decimal.Add(NDiscountAmt, NWriteOffAmt)));
                                        neg_Invoice_IDS.Add(Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]));
                                        is_NegScheduleAllocated = false;
                                    }
                                    else
                                    {
                                        NOverUnderAmt = Env.ZERO;
                                        is_NegScheduleAllocated = true;
                                    }

                                    if (!is_NegScheduleAllocated)
                                    {
                                        is_NegScheduleAllocated = true;
                                        if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)), Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))), C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                            if (Math.Abs(postAppliedAmt) == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                                if (diffAmt != Env.ZERO)
                                                {
                                                    mpay.SetDueAmt(Math.Abs(diffAmt));
                                                }
                                            }
                                            else
                                            {
                                                mpay.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(amount), Math.Abs(NOverUnderAmt)),
                                                           Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))));

                                        if (!mpay.Save(trx))
                                        {
                                            //Get Error message
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            //Set Isprocessing false
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }
                                    }
                                    // Create New schedule with split 
                                    else if (is_NegScheduleAllocated)
                                    {
                                        mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                        PO.CopyValues(mpay, mpay2);
                                        //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                        mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                        mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                        mpay2.ByPassValidatePayScheduleCondition(true);

                                        if (Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]) != C_Currency_ID)
                                        {
                                            var conertedAmount = MConversionRate.Convert(ctx, amount, C_Currency_ID, Util.GetValueOfInt(drNegInv[0]["C_Currency_ID"]),
                                                (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : Util.GetValueOfDateTime(drInv[0]["DateAcct"])), Util.GetValueOfInt(drInv[0]["C_ConversionType_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Client_ID"]), Util.GetValueOfInt(drNegInv[0]["AD_Org_ID"]));
                                            if (Math.Abs(postAppliedAmt) == amount)
                                            {
                                                //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                                diffAmt = GetDifference(Util.GetValueOfInt(drNegInv[0]["C_Invoice_ID"]), trx);
                                                mpay2.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                            else
                                            {
                                                mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                            }
                                        }
                                        else
                                            mpay2.SetDueAmt(Math.Abs(amount));

                                        if (!mpay2.Save(trx))
                                        {
                                            msg = ValidateSaveInvoicePaySchedule(trx);
                                            //Set Isprocessing false
                                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                            return msg;
                                        }

                                    }

                                    //new allocation for -ve amount
                                    Neg_C_InvoicePaySchedule_Id = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                    C_Invoice_ID = Util.GetValueOfInt(negInvList[c]["cinvoiceid"]);
                                    invoiceLines++;
                                    //  Invoice variables
                                    Ref_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                    //allocation for negative Amount Invoice
                                    aLine = new MAllocationLine(alloc, Decimal.Negate(amount), NDiscountAmt, NWriteOffAmt, NOverUnderAmt);
                                    aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                    aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);
                                    aLine.SetRef_Invoiceschedule_ID(positiveAmtInvSchdle_ID);

                                    //get the InvoicePaySchedule_ID and initilaze to negtiveAmtInvSchdle_ID
                                    int negtiveAmtInvSchdle_ID = 0;
                                    if (mpay2 != null)
                                    {
                                        negtiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                    }
                                    else
                                    {
                                        negtiveAmtInvSchdle_ID = Util.GetValueOfInt(negInvList[c]["c_invoicepayschedule_id"]);
                                    }

                                    //set the trx Date and InvoicePayschedule_ID
                                    msg = InvAlloc(Neg_C_InvoicePaySchedule_Id, mpay2, aLine, DateTrx, trx);
                                    if (msg != string.Empty)
                                    {
                                        //set isprocessing false
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }

                                    //Updating +ve Invoice allocationLine to set Ref_InvoicePaySchedule_ID
                                    aLine = new MAllocationLine(ctx, aLine_ID, trx);
                                    aLine.SetRef_Invoiceschedule_ID(negtiveAmtInvSchdle_ID);
                                    if (!aLine.Save())
                                    {
                                        _log.SaveError("Error: ", "Allocation line Ref_InvoicePaySchedule_ID not updated!");
                                        trx.Rollback();
                                        trx.Close();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        if (pp != null)
                                        {
                                            msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated") + ":- " + pp.GetName();
                                        }
                                        else
                                        {
                                            msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated");
                                        }
                                        //set Isprocessing false
                                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                        return msg;
                                    }

                                    //value greater or equal to negative amount then allocating for invoice to invoice
                                    if (value >= 0)
                                    {
                                        negInvList[c].Remove(applied.ToLower());
                                        negInvList[c].Add(applied.ToLower(), (0).ToString());
                                        rowsInvoice[i].Remove(applied.ToLower());
                                        rowsInvoice[i].Add(applied.ToLower(), value.ToString());
                                        AppliedAmt -= Math.Abs(postAppliedAmt);
                                    }
                                    //value greater than negative amount then allocating for invoice to invoice
                                    else
                                    {
                                        rowsInvoice[i].Remove(applied.ToLower());
                                        rowsInvoice[i].Add(applied.ToLower(), (0).ToString());
                                        negInvList[c].Remove(applied.ToLower());
                                        negInvList[c].Add(applied.ToLower(), value.ToString());
                                        // set  writeoff and  discount amoun t as zero, so that not to include for next iteration
                                        negInvList[c].Remove(discount.ToLower());
                                        negInvList[c].Add(discount.ToLower(), (0).ToString());
                                        negInvList[c].Remove(writeOff.ToLower());
                                        negInvList[c].Add(writeOff.ToLower(), (0).ToString());

                                        AppliedAmt = Env.ZERO;
                                    }
                                    //overunder amount for the first time only,for next iteration set to Zero.
                                    WriteOffAmt = Env.ZERO;
                                    DiscountAmt = Env.ZERO;
                                    OverUnderAmt = Env.ZERO;
                                    NWriteOffAmt = Env.ZERO;
                                    NDiscountAmt = Env.ZERO;
                                    NOverUnderAmt = Env.ZERO;
                                    // if the difference of positive AppliedAmt and negative AppliedAmt is equal to Zero this block will break the loop
                                    if (value == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                        totalAppliedAmt = Decimal.Add(totalAppliedAmt, AppliedAmt);
                    }   //  invoice selected
                }   //  invoice loop

                #endregion

                //	Only Payments and total of 0 (e.g. Payment/Reversal)
                #region Reversal Payments
                if (((invoiceLines == 0 || invoiceLines != 0) && paymentList.Count > 0
                    && Env.Signum(paymentAppliedAmt) == 0) ||
                    (paymentList.Count > 0 && (amountList.Min() != 0 || amountList.Max() != 0))) // PAYMENT TO PAYMENT ALLOCATION WITH INVOICE
                {
                    int noPayments = 0;
                    //loop for payment to payment
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        //int C_Payment_ID = Util.GetValueOfInt(paymentList[i]);
                        Decimal PaymentAmt = Util.GetValueOfDecimal(amountList[i]);

                        //	Allocation Header
                        if (alloc.Get_ID() == 0 && !alloc.Save())
                        {
                            //Get Error Message.
                            msg = AllocationHdrFaildToSave(trx);
                            //Set Isprocess false
                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                            return msg;
                        }

                        //if Payment allocation is +ve then allocate the payment to Payment allocation with -ve amount
                        if (PaymentAmt > 0)
                        {
                            Decimal value = 0;
                            MAllocationLine aLine = null;
                            for (int c = 0; c < negPayList.Count; c++)
                            {
                                if (PaymentAmt == Env.ZERO)
                                {
                                    break;
                                }
                                Decimal amount = Env.ZERO;
                                Decimal postAppliedAmt = Util.GetValueOfDecimal(negPayList[c][payment.ToLower()]);
                                if (postAppliedAmt != 0)
                                {
                                    value = PaymentAmt - Math.Abs(postAppliedAmt);
                                    if (value > 0)
                                    {
                                        amount = Math.Abs(postAppliedAmt);
                                    }
                                    else
                                    {
                                        amount = PaymentAmt;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                //new allocation
                                int C_Payment_ID = Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]);
                                noPayments++;
                                int Ref_Payment_ID = Util.GetValueOfInt(negPayList[c]["cpaymentid"]);

                                //for positive Appliedamount Invoice
                                aLine = new MAllocationLine(alloc, Math.Abs(amount), Env.ZERO, Env.ZERO, Env.ZERO);
                                aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                                aLine.SetPaymentInfo(C_Payment_ID, 0);
                                aLine.SetRef_Payment_ID(Ref_Payment_ID);
                                PaymentAmt -= Math.Abs(postAppliedAmt);
                                msg = InvAlloc(0, null, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                    return msg;
                                }

                                //new allocation
                                C_Payment_ID = Util.GetValueOfInt(negPayList[c]["cpaymentid"]);
                                noPayments++;
                                //  Invoice variables
                                Ref_Payment_ID = Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]);

                                //for negative Amount Invoice
                                aLine = new MAllocationLine(alloc, Decimal.Negate(amount), Env.ZERO, Env.ZERO, Env.ZERO);
                                aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                                aLine.SetPaymentInfo(C_Payment_ID, 0);
                                aLine.SetRef_Payment_ID(Ref_Payment_ID);
                                msg = InvAlloc(0, null, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                                    return msg;
                                }
                                if (value <= 0)
                                {
                                    rowsPayment[i].Remove(payment.ToLower());
                                    rowsPayment[i].Add(payment.ToLower(), (0).ToString());
                                    negPayList[c].Remove(payment.ToLower());
                                    negPayList[c].Add(payment.ToLower(), value.ToString());
                                    PaymentAmt = Env.ZERO;
                                }
                                else
                                {
                                    negPayList[c].Remove(payment.ToLower());
                                    negPayList[c].Add(payment.ToLower(), (0).ToString());
                                    rowsPayment[i].Remove(payment.ToLower());
                                    rowsPayment[i].Add(payment.ToLower(), value.ToString());
                                }
                                //exit from the loop when value get zero
                                if (value == 0)
                                {
                                    break;
                                }
                            }
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
                    //alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                    CompleteOrReverse(ctx, alloc.Get_ID(), 150, DocActionVariables.ACTION_COMPLETE, trx);
                    if (!alloc.Save())
                    {
                        //Get Error Message.
                        msg = AllocationHdrFaildToSave(trx);
                        //Set Isprocess false
                        Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                        return msg;
                    }
                    else
                    {
                        msg = alloc.GetDocumentNo();
                    }
                }

                //  Test/Set Payment is fully allocated
                #region Set Payment Allocated
                if (rowsPayment.Count > 0)
                {
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        int C_Payment_ID = Util.GetValueOfInt(paymentList[i]);
                        //VA228:Get open amount and removed MPayment class object
                        string sqlGetOpenPayments = "SELECT currencyConvert(ALLOCPAYMENTAVAILABLE(p.C_Payment_ID) ,p.C_Currency_ID ," + alloc.GetC_Currency_ID() + ", p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID) FROM C_Payment p Where C_Payment_ID = " + C_Payment_ID;
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

                        if (DB.ExecuteQuery("UPDATE C_Payment set IsAllocated='" + (amtPayment == 0 ? "Y" : "N") + "' " +
                                "WHERE C_Payment_ID=" + C_Payment_ID, null, trx) <= 0)
                        {
                            _log.SaveError("Error: ", "Payment not saved");
                            trx.Rollback();
                            trx.Close();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                msg = Msg.GetMsg(ctx, "PaymentNotCreated") + ":- " + pp.GetName();
                            }
                            else
                            {
                                msg = Msg.GetMsg(ctx, "PaymentNotCreated");
                            }
                            //Set Isprocess false
                            Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                            return msg;
                        }

                        //log.Config("Payment #" + i + (pay.IsAllocated() ? " not" : " is")
                        //    + " fully allocated");
                    }
                }
                #endregion

                paymentList.Clear();
                amountList.Clear();
                //set isprocessing false
                Isprocess(rowsPayment, null, rowsInvoice, null, trx);
                //SetIsprocessingFalse(rowsPayment, "cpaymentid", false, false, trx); //Payment
                //SetIsprocessingFalse(rowsCash, "ccashlineid", true, false, trx); //CashLine
                //SetIsprocessingFalse(rowsInvoice, "c_invoicepayschedule_id", false, true, trx); //InvoicePaySchedule
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

        /// <summary>
        /// to check period is open or not for allocation
        /// </summary>
        /// <param name="DateTrx">Transaction Date</param>
        /// <param name="AD_Org_ID"> Trx_Organisation_ID </param>
        /// <returns>Return Empty if period is OPEN else it will return ErrorMsg</returns>
        public string CheckPeriodState(DateTime DateTrx, int AD_Org_ID)
        {
            if (!MPeriod.IsOpen(ctx, DateTrx, MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, AD_Org_ID))
            {
                return Msg.GetMsg(ctx, "PeriodClosed");
            }
            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(ctx, DateTrx, AD_Org_ID))
            {
                return Msg.GetMsg(ctx, "DateIsInNonBusinessDay");
            }
            return "";
        }

        /// <summary>
        /// To get all the unallocated payments
        /// </summary>
        /// <param name="AD_Org_ID">Organisation</param>
        /// <param name="_C_Currency_ID">Currency</param>
        /// <param name="_C_BPartner_ID">Business Partner</param>
        /// <param name="isInterBPartner">Inter-Business Partner</param>
        /// <param name="chk">For MultiCurrency Check</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Page Size</param>
        /// <param name="c_docType_ID">DocmentType</param>
        /// <param name="docBaseType">DocumentBase Type</param>
        /// <param name="payMethod_ID">Payment Method ID</param>
        /// <param name="fromDate">From Date</param>
        /// <param name="toDate">To Date</param>
        /// <param name="srchText">search Document No</param>
        /// <param name="isInterComp">Inter Company Flag</param>
        /// <returns>No of unallocated payments</returns>
        public List<VIS_PaymentData> GetPayments(int AD_Org_ID, int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, int page, int size, int c_docType_ID, string docBaseType, int payMethod_ID, DateTime? fromDate, DateTime? toDate, string srchText, bool isInterComp)
        {
            //used to get related business partner against selected business partner 
            string relatedBpids = string.Empty;
            if (isInterBPartner)
            {
                relatedBpids = GetRelatedBP(_C_BPartner_ID);
            }
            int countRecord = 0;
            // used to create for preciosion handling
            MCurrency objCurrency = MCurrency.Get(ctx, _C_Currency_ID);

            //Changed DateTrx to DateAcct because we have to convert currency on Account Date Not on Transaction Date 
            StringBuilder sql = new StringBuilder("SELECT 'false' as SELECTROW, TO_CHAR(p.DateTrx,'YYYY-MM-DD') as DATE1,p.DocumentNo As DOCUMENTNO,p.C_Payment_ID As CPAYMENTID,"  //  1..3
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
              + "ROUND(currencyConvert(ALLOCPAYMENTAVAILABLE(p.C_Payment_ID) ,p.C_Currency_ID ," + _C_Currency_ID + ",cp.DATEACCT ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID ), " + objCurrency.GetStdPrecision() + ") AS CONVERTEDAMOUNT,"//  6   #1
              + "ROUND(currencyConvert(ALLOCPAYMENTAVAILABLE(p.C_Payment_ID) ,p.C_Currency_ID ," + _C_Currency_ID + ",cp.DATEACCT ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID), " + objCurrency.GetStdPrecision() + ") as OPENAMT,"  //  7   #2
              + "p.MultiplierAP as MULTIPLIERAP, 0 as APPLIEDAMT , TO_CHAR(cp.DATEACCT ,'YYYY-MM-DD') AS DATEACCT, p.AD_Org_ID , o.Name, d.docbasetype , pm.VA009_Name "
              //+ " , dc.name AS DocTypeName "
              + " FROM C_Payment_v p"		//	Corrected for AP/AR
              + " INNER JOIN C_DocType d ON (p.C_DOCTYPE_ID = d.C_DOCTYPE_ID)"  //getting docbasetype
              + " INNER JOIN AD_Org o ON (o.AD_Org_ID = p.AD_Org_ID) "
              + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID) "
              + " INNER JOIN C_Payment cp ON (p.C_Payment_ID = cp.C_Payment_ID) "
              + " INNER JOIN VA009_PaymentMethod pm ON (cp.VA009_PaymentMethod_ID = pm.VA009_PaymentMethod_ID) "
              // + " INNER JOIN C_DOCTYPE DC ON (P.C_DOCTYPE_ID=DC.C_DOCTYPE_ID) "
              + " WHERE   ((p.IsAllocated ='N' and p.c_charge_id is null) "
              + " OR (p.isallocated = 'N' AND p.c_charge_id is not null and p.isprepayment = 'Y'))"
              + " AND p.Processed='Y' AND p.processing ='N' AND p.DocStatus IN ('CO','CL') ");
            //+ " AND p.C_BPartner_ID=" + _C_BPartner_ID);

            //to get payments against related business partner
            if (!string.IsNullOrEmpty(relatedBpids))
                sql.Append(" AND p.C_BPartner_ID IN ( " + relatedBpids + " , " + _C_BPartner_ID + " ) ");
            else
                sql.Append(" AND p.C_BPartner_ID = " + _C_BPartner_ID);

            if (!isInterComp)
            {
                //filter based on Organization
                if (AD_Org_ID != 0)
                {
                    sql.Append(" AND p.AD_Org_ID=" + AD_Org_ID);
                }
            }
            //when paymnet having order schedule ref - those not to be shown in grid 
            if (Env.IsModuleInstalled("VA009_"))
            {
                sql.Append(" AND cp.VA009_OrderPaySchedule_ID IS NULL ");
                //if payment method is selected in filter than get records based on payment method
                if (payMethod_ID > 0)
                {
                    sql.Append(" AND cp.VA009_PaymentMethod_ID=" + payMethod_ID);
                }
            }
            if (!chk)
            {
                sql.Append(" AND p.C_Currency_ID=" + _C_Currency_ID);				//      #4
            }
            if (c_docType_ID > 0)
            {
                sql.Append(" AND d.c_docType_ID=" + c_docType_ID);
            }
            if (docBaseType != "0" && docBaseType != null)
            {
                sql.Append(" AND d.DocBaseType='" + docBaseType + "'");
            }
            if (srchText != string.Empty)
            {
                //JID_1793 -- when search text contain "=" then serach with documnet no only
                if (srchText.Contains("="))
                {
                    String[] myStringArray = srchText.TrimStart(new Char[] { ' ', '=' }).Split(',');
                    if (myStringArray.Length > 0)
                    {
                        sql.Append(" AND UPPER(p.DocumentNo) IN ( ");
                        for (int z = 0; z < myStringArray.Length; z++)
                        {
                            if (z != 0)
                            { sql.Append(","); }
                            sql.Append(" UPPER('" + myStringArray[z].Trim() + "')");
                        }
                        sql.Append(")");
                    }
                }
                else
                {
                    sql.Append(" AND UPPER(p.DocumentNo) LIKE UPPER('%" + srchText.Trim() + "%')");
                }
            }
            //added from and to dates to filter the records which is under these dates
            if (fromDate != null)
            {
                if (toDate != null)
                {
                    sql.Append(" AND p.DateTrx BETWEEN " + GlobalVariable.TO_DATE(fromDate, true) + " AND " + GlobalVariable.TO_DATE(toDate, true));
                }
                else
                {
                    sql.Append(" AND p.DateTrx >= " + GlobalVariable.TO_DATE(fromDate, true));
                }
            }
            if (fromDate == null && toDate != null)
            {
                sql.Append(" AND p.DateTrx <=" + GlobalVariable.TO_DATE(toDate, true));
            }

            sql.Append(" ORDER BY p.DateTrx,p.DocumentNo");
            string qry = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "p", true, false);

            List<VIS_PaymentData> payData = new List<VIS_PaymentData>();

            // count record for paging
            if (page == 1)
            {
                string sql1 = @"SELECT COUNT(*) FROM C_Payment_v p"
              + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID) "
              + " WHERE   ((p.IsAllocated ='N' and p.c_charge_id is null) "
              + " OR (p.isallocated = 'N' AND p.c_charge_id is not null and p.isprepayment = 'Y'))"
              + " AND p.Processed='Y' AND p.DocStatus IN ('CO','CL') ";
                //to get payments against related business partner
                if (!string.IsNullOrEmpty(relatedBpids))
                    sql1 += (" AND p.C_BPartner_ID IN ( " + relatedBpids + " , " + _C_BPartner_ID + " ) ");
                else
                    sql1 += (" AND p.C_BPartner_ID = " + _C_BPartner_ID);

                if (!chk)
                {
                    sql1 += " AND p.C_Currency_ID=" + _C_Currency_ID;
                }

                sql1 = MRole.GetDefault(ctx).AddAccessSQL(sql1, "p", true, false);
                countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql1, null, null));
            }

            DataSet dr = VIS.DBase.DB.ExecuteDatasetPaging(sql.ToString(), page, size);

            if (dr != null && dr.Tables.Count > 0 && dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    //pData
                    VIS_PaymentData pData = new VIS_PaymentData();
                    pData.SelectRow = "false";
                    pData.PaymentRecord = countRecord;
                    // Converted into DateTime to handle in Central America TimeZone.
                    pData.Date1 = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["DATE1"]);
                    pData.Documentno = dr.Tables[0].Rows[i]["DOCUMENTNO"].ToString();
                    pData.CpaymentID = dr.Tables[0].Rows[i]["CPAYMENTID"].ToString();
                    pData.Isocode = dr.Tables[0].Rows[i]["ISOCODE"].ToString();
                    pData.Payment = dr.Tables[0].Rows[i]["PAYMENT"].ToString();
                    pData.ConvertedAmount = string.IsNullOrEmpty(dr.Tables[0].Rows[i]["CONVERTEDAMOUNT"].ToString()) ? "0" : dr.Tables[0].Rows[i]["CONVERTEDAMOUNT"].ToString();
                    pData.OpenAmt = string.IsNullOrEmpty(dr.Tables[0].Rows[i]["OPENAMT"].ToString()) ? "0" : dr.Tables[0].Rows[i]["OPENAMT"].ToString();
                    pData.Multiplierap = dr.Tables[0].Rows[i]["MULTIPLIERAP"].ToString();
                    pData.AppliedAmt = dr.Tables[0].Rows[i]["APPLIEDAMT"].ToString();
                    pData.C_ConversionType_ID = Util.GetValueOfInt(dr.Tables[0].Rows[i]["C_CONVERSIONTYPE_ID"]);
                    pData.ConversionName = Util.GetValueOfString(dr.Tables[0].Rows[i]["CONVERSIONNAME"]);
                    pData.DATEACCT = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["DATEACCT"]);
                    pData.AD_Org_ID = Convert.ToInt32(dr.Tables[0].Rows[i]["AD_Org_ID"]);
                    pData.OrgName = Convert.ToString(dr.Tables[0].Rows[i]["Name"]);
                    pData.DocBaseType = dr.Tables[0].Rows[i]["docbasetype"].ToString();
                    pData.PayName = Util.GetValueOfString(dr.Tables[0].Rows[i]["VA009_Name"]);
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

        /// <summary>
        /// To get all the unallocated Cash Lines
        /// </summary>
        /// <param name="AD_Org_ID">Organisation</param>
        /// <param name="_C_Currency_ID">Currency</param>
        /// <param name="_C_BPartner_ID">Business Partner</param>
        /// <param name="isInterBPartner">Inter-Business Partner</param>
        /// <param name="chk">For MultiCurrency Check</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Page Size</param>
        /// <param name="fromDate"> From Date </param>
        /// <param name="toDate">To Date</param>
        /// <param name="paymentType_ID">Payment Type</param>
        /// <param name="srchText">Search Document No</param>
        /// <param name="isInterComp">Inter Company Flag</param>
        /// <returns>No of unallocated Cash Lines</returns>
        public List<VIS_CashData> GetCashJounral(int AD_Org_ID, int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, int page, int size, DateTime? fromDate, DateTime? toDate, string paymentType_ID, string srchText, bool isInterComp)
        {
            //used to get related business partner against selected business partner 
            string relatedBpids = string.Empty;
            if (isInterBPartner)
            {
                relatedBpids = GetRelatedBP(_C_BPartner_ID);
            }

            int countRecord = 0;
            // used to create for preciosion handling
            MCurrency objCurrency = MCurrency.Get(ctx, _C_Currency_ID);

            //Changed created date to DateAcct because we have to convert currency on Account Date Not on Created Date 
            string sqlCash = "SELECT 'false' as SELECTROW, TO_CHAR(cn.DATEACCT ,'YYYY-MM-DD') as CREATED, cn.receiptno AS RECEIPTNO, cn.c_cashline_id AS CCASHLINEID,cl.VSS_PaymentType "
                             + @",c.ISO_Code AS ISO_CODE,
                                CASE
                                 WHEN NVL(cn.C_CONVERSIONTYPE_ID , 0) !=0 THEN cn.C_CONVERSIONTYPE_ID
                                 WHEN (GetConversionType(cn.AD_Client_ID) != 0 ) THEN GetConversionType(cn.AD_Client_ID)
                                 ELSE (GetConversionType(0)) END AS C_CONVERSIONTYPE_ID,
                                CASE 
                                  WHEN NVL(cn.C_CONVERSIONTYPE_ID , 0) !=0 THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = cn.C_CONVERSIONTYPE_ID )
                                  WHEN (GetConversionType(cn.AD_Client_ID) != 0 ) THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = GetConversionType(cn.AD_Client_ID))
                                  ELSE (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID =(GetConversionType(0)) ) END AS CONVERSIONNAME ,
                                CASE
                                  WHEN NVL(cl.VSS_PaymentType,'0')!='0' THEN (SELECT Name FROM AD_Ref_List WHERE AD_Reference_ID=(SELECT AD_Reference_Value_ID FROM AD_Column WHERE ColumnName ='VSS_PAYMENTTYPE' AND AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE TableName='C_CashLine')) AND IsActive='Y' AND value=cl.VSS_PaymentType) END AS Payment,
                                  ROUND(cn.amount, " + objCurrency.GetStdPrecision() + ") AS AMOUNT, "
                             + " ROUND(currencyConvert(ALLOCCASHAVAILABLE(cn.C_CashLine_ID),cn.C_Currency_ID ," + _C_Currency_ID + ",cn.DATEACCT ,cn.C_ConversionType_ID  ,cn.AD_Client_ID ,cn.AD_Org_ID ) , " + objCurrency.GetStdPrecision() + ") AS CONVERTEDAMOUNT,"//  6   #1cn.amount as OPENAMT,"
                             + " ROUND(currencyConvert(ALLOCCASHAVAILABLE(cn.C_CashLine_ID),cn.C_Currency_ID ," + _C_Currency_ID + ",cn.DATEACCT,cn.C_ConversionType_ID ,cn.AD_Client_ID ,cn.AD_Org_ID), " + objCurrency.GetStdPrecision() + ") as OPENAMT,"  //  7   #2
                                                                                                                                                                                                                                                              //+ " currencyConvert(cn.Amount ,cn.C_Currency_ID ," + _C_Currency_ID + ",cn.Created ,114 ,cn.AD_Client_ID ,cn.AD_Org_ID ) as OPENAMT,"
                              + " cn.MultiplierAP AS MULTIPLIERAP,0 as APPLIEDAMT,TO_CHAR(cn.DATEACCT ,'YYYY-MM-DD') as DATEACCT , o.AD_Org_ID  , o.Name " +
                             " FROM C_CashLine_new cn"
                             + " INNER JOIN C_Cashline cl ON (cl.C_Cashline_ID=cn.C_Cashline_ID)"
                              + " INNER JOIN AD_Org o ON (o.AD_Org_ID = cn.AD_Org_ID) "
                             + " INNER JOIN c_currency c ON (cn.C_Currency_ID=c.C_Currency_ID) WHERE cn.IsAllocated   ='N' AND cn.Processed ='Y'"
                             //+ " and cn.cashtype IN ('I' , 'B') and cn.docstatus in ('CO','CL') "

                             //Enhancement ID- JID_0593,  Cash line  is created with refrence of Invoice. Void View allocation. Cash line do not show on payment allocation. 
                             + " and cn.cashtype IN ('B', 'I') and cn.docstatus in ('CO','CL') ";// AND cn.Processing = 'N' "

            // Commented because Against Business Partner there is no charge
            // + " AND cn.C_Charge_ID  IS Not NULL"

            // get all records for related business partners
            if (!string.IsNullOrEmpty(relatedBpids))
                sqlCash += " AND cn.C_BPartner_ID IN ( " + relatedBpids + " , " + _C_BPartner_ID + " )";
            else
                sqlCash += " AND cn.C_BPartner_ID = " + _C_BPartner_ID;
            if (!isInterComp)
            {
                //filter based on Organization
                if (AD_Org_ID != 0)
                {
                    sqlCash += " AND cn.AD_Org_ID=" + AD_Org_ID;
                }
            }

            if (!chk)
            {
                sqlCash += " AND cn.C_Currency_ID=" + _C_Currency_ID;
            }
            if (fromDate != null)
            {
                if (toDate != null)
                {
                    sqlCash += " AND cn.DATEACCT BETWEEN " + GlobalVariable.TO_DATE(fromDate, true) + " AND " + GlobalVariable.TO_DATE(toDate, true);
                }
                else
                {
                    sqlCash += " AND cn.DATEACCT >= " + GlobalVariable.TO_DATE(fromDate, true);
                }
            }
            if (fromDate == null && toDate != null)
            {
                sqlCash += " AND cn.DATEACCT <=" + GlobalVariable.TO_DATE(toDate, true);
            }
            if (paymentType_ID != null && paymentType_ID != "0")
            {
                sqlCash += " AND cl.VSS_PaymentType ='" + paymentType_ID.ToString() + "'";
            }
            if (srchText != string.Empty)
            {
                //JID_1793 -- when search text contain "=" then serach with documnet no only
                if (srchText.Contains("="))
                {
                    String[] myStringArray = srchText.TrimStart(new Char[] { ' ', '=' }).Split(',');
                    if (myStringArray.Length > 0)
                    {
                        sqlCash += " AND UPPER(cn.receiptno) IN ( ";
                        for (int z = 0; z < myStringArray.Length; z++)
                        {
                            if (z != 0)
                            { sqlCash += ","; }
                            sqlCash += " UPPER('" + myStringArray[z].Trim() + "')";
                        }
                        sqlCash += ")";
                    }
                }
                else
                {
                    sqlCash += " AND UPPER(cn.receiptno) LIKE UPPER('%" + srchText.Trim() + "%')";
                }
            }

            sqlCash += " ORDER BY cn.created,cn.receiptno";

            sqlCash = MRole.GetDefault(ctx).AddAccessSQL(sqlCash, "cn", true, false);

            List<VIS_CashData> payData = new List<VIS_CashData>();

            // count record for paging
            if (page == 1)
            {
                string sql = @"SELECT COUNT(*) FROM c_cashline_new cn"
                             + " INNER join c_currency c ON (cn.C_Currency_ID=c.C_Currency_ID) WHERE cn.IsAllocated   ='N' AND cn.Processed ='Y'"
                             + " and cn.cashtype = 'B' and cn.docstatus in ('CO','CL') ";
                // get all records for related business partners
                if (!string.IsNullOrEmpty(relatedBpids))
                    sql += " AND cn.C_BPartner_ID IN ( " + relatedBpids + " , " + _C_BPartner_ID + " )";
                else
                    sql += " AND cn.C_BPartner_ID = " + _C_BPartner_ID;

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
                    pData.VSS_paymenttype = dr.Tables[0].Rows[i]["VSS_paymenttype"].ToString();
                    pData.Isocode = dr.Tables[0].Rows[i]["ISO_CODE"].ToString();
                    pData.Amount = dr.Tables[0].Rows[i]["Amount"].ToString();
                    pData.ConvertedAmount = string.IsNullOrEmpty(dr.Tables[0].Rows[i]["CONVERTEDAMOUNT"].ToString()) ? "0" : dr.Tables[0].Rows[i]["CONVERTEDAMOUNT"].ToString();
                    pData.OpenAmt = string.IsNullOrEmpty(dr.Tables[0].Rows[i]["OPENAMT"].ToString()) ? "0" : dr.Tables[0].Rows[i]["OPENAMT"].ToString();
                    pData.Multiplierap = dr.Tables[0].Rows[i]["MULTIPLIERAP"].ToString();
                    pData.CcashlineiID = dr.Tables[0].Rows[i]["CCASHLINEID"].ToString();
                    pData.AppliedAmt = dr.Tables[0].Rows[i]["APPLIEDAMT"].ToString();
                    pData.Payment = dr.Tables[0].Rows[i]["Payment"].ToString();
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

        //Added new parameters---Neha---
        /// <summary>
        /// To get all the invoices 
        /// </summary>
        /// <param name="AD_Org_ID">Organization ID</param>
        /// <param name="_C_Currency_ID">Currency ID</param>
        /// <param name="_C_BPartner_ID"> Business Partner ID</param>
        /// <param name="isInterBPartner">bool Value </param>
        /// <param name="chk">bool Value </param>
        /// <param name="date">Transaction Date</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Total Page Size</param>
        /// <param name="docNo">Document Number</param>
        /// <param name="c_docType_ID">Document Type ID</param>
        /// <param name="docBaseType">DocBaseType</param>
        /// <param name="payMethod_ID">Payment Method ID</param>
        /// <param name="fromDate">From Date</param>
        /// <param name="toDate">To Date</param>
        /// <param name="conversionDate">ConversionType Date</param>
        /// <param name="srchText">Search Document No</param>
        /// <param name="isInterComp">Inter Company Flag</param>
        /// <returns></returns>
        public List<VIS_InvoiceData> GetInvoice(int AD_Org_ID, int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, string date, int page, int size, string docNo, int c_docType_ID, string docBaseType, int payMethod_ID, DateTime? fromDate, DateTime? toDate, string conversionDate, string srchText, bool isInterComp)
        {
            //used to get related business partner against selected business partner 
            string relatedBpids = string.Empty;
            if (isInterBPartner)
            {
                relatedBpids = GetRelatedBP(_C_BPartner_ID);
            }

            int countRecord = 0;
            // used to create for preciosion handling
            MCurrency objCurrency = MCurrency.Get(ctx, _C_Currency_ID);

            //Changed DateInvoiced to DateAcct because we have to convert currency on Account Date Not on Invoiced Date 
            //Query Replaced with new optimized query
            StringBuilder sqlInvoice = new StringBuilder(@" WITH Invoice AS ( SELECT 'false' as SELECTROW, 
            TO_CHAR(i.DateInvoiced, 'YYYY-MM-DD') as DATE1, i.DocumentNo AS DOCUMENTNO, 
            i.C_Invoice_ID AS CINVOICEID, c.ISO_Code AS ISO_CODE, i.C_CONVERSIONTYPE_ID, i.AD_Client_ID, 
            i.AD_Org_ID, i.C_Currency_ID, i.MultiplierAP, i.docbasetype, 0 as WRITEOFF, 0 as APPLIEDAMT, 
            i.DATEACCT, i.C_InvoicePaySchedule_ID, i.C_Invoice_ID, o.Name, pm.VA009_Name FROM C_Invoice_PA_v i 
            INNER JOIN AD_Org o ON (o.AD_Org_ID = i.AD_Org_ID) INNER JOIN C_Currency c ON (i.C_Currency_ID = c.C_Currency_ID)
            INNER JOIN C_InvoicePaySchedule ips ON (i.C_Invoice_ID = ips.C_Invoice_ID AND i.C_InvoicePaySchedule_ID=ips.C_InvoicePaySchedule_ID ) 
            INNER JOIN VA009_PaymentMethod pm ON (ips.VA009_PaymentMethod_ID = pm.VA009_PaymentMethod_ID) 
            WHERE i.IsPaid='N' AND i.Processed = 'Y' AND ips.IsHoldPayment='N'");

            //to get invoice schedules against related business partner
            if (!string.IsNullOrEmpty(relatedBpids))
                sqlInvoice.Append(" AND i.C_BPartner_ID IN ( " + relatedBpids + " , " + _C_BPartner_ID + " ) ");
            else
                sqlInvoice.Append(" AND i.C_BPartner_ID = " + _C_BPartner_ID);

            #region Commented because we optimize the query
            //string sqlInvoice = "SELECT 'false' as SELECTROW , TO_CHAR(i.DateInvoiced,'YYYY-MM-DD')  as DATE1  ,  i.DocumentNo    AS DOCUMENTNO  , i.C_Invoice_ID AS CINVOICEID,"
            //                    + @"  c.ISO_Code AS ISO_CODE , 
            //                         CASE
            //                          WHEN NVL(i.C_CONVERSIONTYPE_ID , 0) !=0 THEN i.C_CONVERSIONTYPE_ID
            //                          WHEN (GetConversionType(i.AD_Client_ID) != 0 ) THEN GetConversionType(i.AD_Client_ID)
            //                          ELSE (GetConversionType(0)) END AS C_CONVERSIONTYPE_ID,
            //                        CASE 
            //                          WHEN NVL(i.C_CONVERSIONTYPE_ID , 0) !=0 THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = i.C_CONVERSIONTYPE_ID )
            //                          WHEN (GetConversionType(i.AD_Client_ID) != 0 ) THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = GetConversionType(i.AD_Client_ID))
            //                          ELSE (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID =(GetConversionType(0)) ) END AS CONVERSIONNAME ,ROUND((invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP), " + objCurrency.GetStdPrecision() + ") AS CURRENCY ,"
            //                    + "ROUND(currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP,i.C_Currency_ID ," + _C_Currency_ID + ", " + (conversionDate != "" ? GlobalVariable.TO_DATE(Convert.ToDateTime(conversionDate), true) : " i.DATEACCT ") + ",i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID ), " + objCurrency.GetStdPrecision() + ") AS CONVERTED  ,"
            //                    + " ROUND(currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + _C_Currency_ID + "," + (conversionDate != "" ? GlobalVariable.TO_DATE(Convert.ToDateTime(conversionDate), true) : " i.DATEACCT ") + ",i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID) * i.MultiplierAP , " + objCurrency.GetStdPrecision() + ") AS AMOUNT,"
            //                    + "  ROUND((currencyConvert(invoiceDiscount(i.C_Invoice_ID ," + date + ",C_InvoicePaySchedule_ID),i.C_Currency_ID ," + _C_Currency_ID + "," + (conversionDate != "" ? GlobalVariable.TO_DATE(Convert.ToDateTime(conversionDate), true) : " i.DATEACCT ") + " ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID )*i.Multiplier*i.MultiplierAP) , " + objCurrency.GetStdPrecision() + ") AS DISCOUNT ,"
            //                    + "  i.MultiplierAP ,i.docbasetype  ,0 as WRITEOFF ,0 as APPLIEDAMT ,TO_CHAR(i.DATEACCT ,'YYYY-MM-DD') as DATEACCT, i.C_InvoicePaySchedule_ID,(select TO_CHAR(Ip.Duedate,'YYYY-MM-DD') from C_InvoicePaySchedule ip where C_InvoicePaySchedule_ID=i.C_InvoicePaySchedule_ID) Scheduledate "
            //                    //  + ", dc.name AS DocTypeName "
            //                    + " , i.AD_Org_ID , o.Name "
            //                    + " FROM C_Invoice_v i"		//  corrected for CM/Split
            //                    + " INNER JOIN AD_Org o ON o.AD_Org_ID = i.AD_Org_ID "
            //                    + " INNER JOIN C_Currency c ON (i.C_Currency_ID=c.C_Currency_ID) "
            //                    // + " INNER JOIN C_DOCTYPE DC ON (i.C_DOCTYPE_ID =DC.C_DOCTYPE_ID)"
            //                    + " WHERE i.IsPaid='N' AND i.Processed='Y'"
            //                    + " AND i.C_BPartner_ID=" + _C_BPartner_ID;
            #endregion

            //------Filter data on the basis of inter company parameters
            if (!isInterComp)
            {
                if (AD_Org_ID != 0)
                {
                    sqlInvoice.Append(" AND i.AD_Org_ID=" + AD_Org_ID);
                }
            }
            if (!chk)
            {
                sqlInvoice.Append(" AND i.C_Currency_ID=" + _C_Currency_ID);                                   //  #6
            }
            //sqlInvoice += " AND ROUND((invoiceOpen(C_Invoice_ID, C_InvoicePaySchedule_ID) * i.MultiplierAP), " + objCurrency.GetStdPrecision() + ") <> 0 ";
            //sqlInvoice += " AND (currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + _C_Currency_ID + ",i.DATEACCT,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID) *i.MultiplierAP ) <> 0 ";


            //------Filter data on the basis of new parameters
            if (!String.IsNullOrEmpty(docNo))
            {
                sqlInvoice.Append("AND Upper(i.documentno) LIKE Upper('%" + docNo + "%')");
            }
            if (c_docType_ID > 0)
            {
                sqlInvoice.Append(" AND i.C_DOCTYPETARGET_ID=" + c_docType_ID);
            }
            if (docBaseType != "0" && docBaseType != null)
            {
                sqlInvoice.Append(" AND i.DocBaseType='" + docBaseType + "'");
            }
            if (payMethod_ID > 0)
            {
                sqlInvoice.Append(" AND ips.VA009_PaymentMethod_ID=" + payMethod_ID);
            }
            if (srchText != string.Empty)
            {
                //JID_1793 -- when search text contain "=" then serach with documnet no only
                if (srchText.Contains("="))
                {
                    String[] myStringArray = srchText.TrimStart(new Char[] { ' ', '=' }).Split(',');
                    if (myStringArray.Length > 0)
                    {
                        sqlInvoice.Append(" AND UPPER(i.documentno) IN ( ");
                        for (int z = 0; z < myStringArray.Length; z++)
                        {
                            if (z != 0)
                            { sqlInvoice.Append(","); }
                            sqlInvoice.Append(" UPPER('" + myStringArray[z].Trim() + "')");
                        }
                        sqlInvoice.Append(")");
                    }
                }
                else
                {
                    sqlInvoice.Append(" AND UPPER(i.documentno) LIKE UPPER('%" + srchText.Trim() + "%')");
                }
            }

            if (fromDate != null)
            {
                if (toDate != null)
                {
                    sqlInvoice.Append(" AND I.DATEINVOICED BETWEEN " + GlobalVariable.TO_DATE(fromDate, true) + " AND " + GlobalVariable.TO_DATE(toDate, true));
                }
                else
                {
                    sqlInvoice.Append(" AND I.DATEINVOICED >= " + GlobalVariable.TO_DATE(fromDate, true));
                }
            }
            if (fromDate == null && toDate != null)
            {
                sqlInvoice.Append(" AND I.DATEINVOICED <=" + GlobalVariable.TO_DATE(toDate, true));
            }
            //--------------------------------------

            string sqlnew = string.Empty;
            sqlnew = MRole.GetDefault(ctx).AddAccessSQL(sqlInvoice.ToString(), "i", true, false);
            sqlInvoice.Clear();
            sqlInvoice.Append(sqlnew);
            sqlnew = null;

            sqlInvoice.Append(@" ), OpenInvoice AS ( SELECT 	SELECTROW,Name, DATE1, DOCUMENTNO, CINVOICEID, ISO_CODE, T1.C_CONVERSIONTYPE_ID, 
                                AD_Client_ID, AD_Org_ID, T1.C_Currency_ID, T1.MultiplierAP, docbasetype, WRITEOFF, APPLIEDAMT, DATEACCT, 
                                T1.C_InvoicePaySchedule_ID, T1.C_Invoice_ID, 
                                /*INVOICEOPEN_NEW(T2.C_Invoice_ID, T2.C_InvoicePaySchedule_ID, 
                                T2.C_Currency_ID, T2.C_CONVERSIONTYPE_ID, T2.GRANDTOTAL, T2.MULTIPLIERAP, T2.MULTIPLIER, T2.ModCount)*/
                                T2.GRANDTOTAL  AS invoiceOpen, t1.VA009_Name
                                FROM Invoice T1 INNER JOIN C_INVOICE_V_NEW_PA T2
                                ON (T1.C_Invoice_ID = T2.C_Invoice_ID AND NVL(T1.C_InvoicePaySchedule_ID, 0) = NVL (T2.C_InvoicePaySchedule_ID, 0)) ) ");
            List<VIS_InvoiceData> payData = new List<VIS_InvoiceData>();

            // count record for paging
            if (page == 1)
            {
                StringBuilder sql = new StringBuilder(@"SELECT COUNT(ci.C_InvoicePaySchedule_ID) FROM C_Invoice i
                                 INNER JOIN C_InvoicePaySchedule ci ON (ci.C_Invoice_ID = i.C_Invoice_ID)
                                 INNER JOIN C_Currency c ON (i.C_Currency_ID=c.C_Currency_ID) 
                              WHERE i.IsPaid='N' AND i.Processed='Y' AND ci.VA009_IsPaid = 'N'");

                //to get invoice schedules against related business partner
                if (!string.IsNullOrEmpty(relatedBpids))
                    sql.Append(" AND i.C_BPartner_ID IN ( " + relatedBpids + " , " + _C_BPartner_ID + " ) ");
                else
                    sql.Append(" AND i.C_BPartner_ID = " + _C_BPartner_ID);

                if (!chk)
                {
                    sql.Append(" AND i.C_Currency_ID=" + _C_Currency_ID);
                }

                if (!String.IsNullOrEmpty(docNo))
                {
                    sql.Append(" AND Upper(i.documentno) LIKE Upper('%" + docNo + "%')");
                }
                if (c_docType_ID > 0)
                {
                    sql.Append(" AND i.C_DOCTYPETARGET_ID=" + c_docType_ID);
                }
                //if (docBaseType != "0" && docBaseType != null)
                //{
                //    sqlInvoice.Append(" AND i.DocBaseType='" + docBaseType + "'");
                //}
                if (payMethod_ID > 0)
                {
                    sql.Append(" AND ci.VA009_PaymentMethod_ID=" + payMethod_ID);
                }
                if (srchText != string.Empty)
                {
                    //JID_1793 -- when search text contain "=" then serach with documnet no only
                    if (srchText.Contains("="))
                    {
                        String[] myStringArray = srchText.TrimStart(new Char[] { ' ', '=' }).Split(',');
                        if (myStringArray.Length > 0)
                        {
                            sql.Append(" AND UPPER(i.documentno) IN ( ");
                            for (int z = 0; z < myStringArray.Length; z++)
                            {
                                if (z != 0)
                                { sql.Append(","); }
                                sql.Append(" UPPER('" + myStringArray[z].Trim() + "')");
                            }
                            sql.Append(")");
                        }
                    }
                    else
                    {
                        sql.Append(" AND UPPER(i.documentno) LIKE UPPER('%" + srchText.Trim() + "%')");
                    }
                }

                if (fromDate != null)
                {
                    if (toDate != null)
                    {
                        sql.Append(" AND I.DATEINVOICED BETWEEN " + GlobalVariable.TO_DATE(fromDate, true) + " AND " + GlobalVariable.TO_DATE(toDate, true));
                    }
                    else
                    {
                        sql.Append(" AND I.DATEINVOICED >= " + GlobalVariable.TO_DATE(fromDate, true));
                    }
                }
                if (fromDate == null && toDate != null)
                {
                    sql.Append(" AND I.DATEINVOICED <=" + GlobalVariable.TO_DATE(toDate, true));
                }

                //sql += " AND ((invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)) *i.MultiplierAP ) <> 0 ";
                sql.Append(" AND ci.IsValid = 'Y' AND NVL(ci.C_Payment_ID , 0 ) = 0 AND NVL(ci.C_CashLine_ID, 0) = 0");

                string newsql = MRole.GetDefault(ctx).AddAccessSQL(sql.ToString(), "C_Invoice", true, false);
                countRecord = Util.GetValueOfInt(DB.ExecuteScalar(newsql, null, null));
            }
            if (String.IsNullOrEmpty(conversionDate))
            {
                conversionDate = "DATEACCT";
            }
            else
            {
                conversionDate = GlobalVariable.TO_DATE(Convert.ToDateTime(conversionDate), true);
            }
            sqlInvoice.Append(@" SELECT 	SELECTROW, Name,  DATE1, DOCUMENTNO, CINVOICEID, ISO_CODE, 
CASE 	WHEN NVL(C_CONVERSIONTYPE_ID, 0) !=0 THEN C_CONVERSIONTYPE_ID WHEN GetConversionType(AD_Client_ID) != 0 THEN GetConversionType(AD_Client_ID) 
ELSE 	GetConversionType(0) END AS C_CONVERSIONTYPE_ID, 
CASE 	WHEN NVL(C_CONVERSIONTYPE_ID, 0) !=0 THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = Result.C_CONVERSIONTYPE_ID ) 
WHEN GetConversionType(AD_Client_ID) != 0 THEN (SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = GetConversionType(Result.AD_Client_ID)) 
ELSE 	(SELECT name FROM C_CONVERSIONTYPE WHERE C_CONVERSIONTYPE_ID = GetConversionType(0)) END AS CONVERSIONNAME, (invoiceOpen * MultiplierAP) AS CURRENCY,
currencyConvert(invoiceOpen * MultiplierAP, C_Currency_ID, " + _C_Currency_ID + ", " + conversionDate +
", C_ConversionType_ID, AD_Client_ID, AD_Org_ID) AS CONVERTED, currencyConvert(invoiceOpen, C_Currency_ID," + _C_Currency_ID + ", "
+ conversionDate + ", C_ConversionType_ID," +
" AD_Client_ID, AD_Org_ID) * MultiplierAP AS AMOUNT, (currencyConvert(invoiceDiscount(C_Invoice_ID, " + date + ", C_InvoicePaySchedule_ID)," +
" C_Currency_ID," + _C_Currency_ID + ", " + conversionDate + ", C_ConversionType_ID, " +
"AD_Client_ID, AD_Org_ID) * MultiplierAP) AS DISCOUNT, MultiplierAP, docbasetype, WRITEOFF, APPLIEDAMT, DATEACCT, C_InvoicePaySchedule_ID," +
" (select TO_CHAR(Ip.Duedate,'YYYY-MM-DD') from C_InvoicePaySchedule ip where C_InvoicePaySchedule_ID = Result.C_InvoicePaySchedule_ID) Scheduledate, " +
"AD_Org_ID, VA009_Name FROM	OpenInvoice Result WHERE 	invoiceOpen * MultiplierAP <> 0");

            //IDataReader dr = DB.ExecuteReader(sqlInvoice);
            DataSet dr = VIS.DBase.DB.ExecuteDatasetPaging(sqlInvoice.ToString(), page, size);
            if (dr != null && dr.Tables.Count > 0 && dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    VIS_InvoiceData pData = new VIS_InvoiceData();
                    pData.SelectRow = "false";
                    pData.InvoiceRecord = countRecord;
                    // Converted into DateTime to handle in Central America TimeZone.
                    pData.Date1 = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["DATE1"]);
                    pData.Documentno = dr.Tables[0].Rows[i]["DOCUMENTNO"].ToString();
                    pData.CinvoiceID = dr.Tables[0].Rows[i]["CINVOICEID"].ToString();
                    pData.Isocode = dr.Tables[0].Rows[i]["ISO_CODE"].ToString();
                    pData.Currency = dr.Tables[0].Rows[i]["CURRENCY"].ToString();
                    //pData.Converted = dr.Tables[0].Rows[i]["CONVERTED"].ToString();
                    pData.Converted = string.IsNullOrEmpty(dr.Tables[0].Rows[i]["CONVERTED"].ToString()) ? "0" : dr.Tables[0].Rows[i]["CONVERTED"].ToString();//if the value is null then set it as zero to avoid null value.
                    //pData.Amount = dr.Tables[0].Rows[i]["Amount"].ToString();
                    pData.Amount = string.IsNullOrEmpty(dr.Tables[0].Rows[i]["Amount"].ToString()) ? "0" : dr.Tables[0].Rows[i]["Amount"].ToString();//if the value is null then set it as zero to avoid null value.
                    //commented because as per ashish and surya user will enter writeoff and discount if he/she want to give
                    //pData.Discount = dr.Tables[0].Rows[i]["DISCOUNT"].ToString();
                    //pData.Writeoff = dr.Tables[0].Rows[i]["WRITEOFF"].ToString();
                    pData.Discount = decimal.Zero.ToString();
                    pData.Writeoff = decimal.Zero.ToString();
                    pData.Multiplierap = dr.Tables[0].Rows[i]["MULTIPLIERAP"].ToString();
                    pData.DocBaseType = dr.Tables[0].Rows[i]["docbasetype"].ToString();
                    pData.AppliedAmt = dr.Tables[0].Rows[i]["APPLIEDAMT"].ToString();
                    pData.C_InvoicePaySchedule_ID = dr.Tables[0].Rows[i]["C_InvoicePaySchedule_ID"].ToString();
                    // Converted into DateTime to handle in Central America TimeZone.
                    pData.InvoiceScheduleDate = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["Scheduledate"]);
                    pData.C_ConversionType_ID = Util.GetValueOfInt(dr.Tables[0].Rows[i]["C_CONVERSIONTYPE_ID"]);
                    pData.ConversionName = Util.GetValueOfString(dr.Tables[0].Rows[i]["CONVERSIONNAME"]);
                    pData.DATEACCT = Util.GetValueOfDateTime(dr.Tables[0].Rows[i]["DATEACCT"]);
                    pData.AD_Org_ID = Convert.ToInt32(dr.Tables[0].Rows[i]["AD_Org_ID"]);
                    pData.OrgName = Convert.ToString(dr.Tables[0].Rows[i]["Name"]);
                    pData.PayName = Util.GetValueOfString(dr.Tables[0].Rows[i]["VA009_Name"]);
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

        /// <summary>
        /// to get DocBaseType of Invoice for Invoice Grid Filter.
        /// to bind these values to dropdown of Invoice grid.
        /// </summary>
        /// <returns>List of DocBase Types</returns>
        /// Payment Grid
        public List<VIS_DocbaseType> GetpayDocbaseType()
        {
            List<VIS_DocbaseType> DocbaseType = new List<VIS_DocbaseType>();
            //string _sql = "SELECT C_DocType.DocBaseType, C_DocType.Name FROM C_DOCTYPE C_DOCTYPE INNER JOIN C_DOCBASETYPE DB ON C_DocType.DOCBASETYPE=DB.DOCBASETYPE WHERE DB.DOCBASETYPE IN ('APP','ARR') AND C_DocType.ISACTIVE='Y'";
            string _sql = "SELECT C_DOCBASETYPE.DocBaseType, C_DOCBASETYPE.Name FROM C_DOCBASETYPE C_DOCBASETYPE WHERE C_DOCBASETYPE.DOCBASETYPE IN ('APP','ARR') AND C_DOCBASETYPE.ISACTIVE='Y'";
            _sql = MRole.GetDefault(ctx).AddAccessSQL(_sql, "C_DocBaseType", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(_sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DocbaseType.Add(new VIS_DocbaseType() { DocbaseType = Convert.ToString(ds.Tables[0].Rows[i]["DocBaseType"]), Name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]) });
                }
                ds.Dispose();
            }
            return DocbaseType;
        }

        /// <summary>
        /// to get DocBaseType for Invoice grid Filter.
        /// for append the values to dropdown
        /// </summary>
        /// <returns>List of DobBase Types</returns>
        /// Invoice Grid
        public List<VIS_DocbaseType> GetDocbaseType()
        {
            List<VIS_DocbaseType> DocbaseType = new List<VIS_DocbaseType>();
            //string _sql = "SELECT DB.DocBaseType, DB.Name FROM C_DOCTYPE C_DOCTYPE INNER JOIN C_DOCBASETYPE DB ON C_DocType.DOCBASETYPE=DB.DOCBASETYPE WHERE DB.DOCBASETYPE IN ('API','APR','APC','ARC') AND C_DocType.ISACTIVE='Y'";
            string _sql = "SELECT C_DOCBASETYPE.DocBaseType, C_DOCBASETYPE.Name FROM C_DocBaseType C_DOCBASETYPE WHERE C_DOCBASETYPE.DOCBASETYPE IN ('API','ARI','APC','ARC') AND C_DOCBASETYPE.ISACTIVE='Y'";
            _sql = MRole.GetDefault(ctx).AddAccessSQL(_sql, "C_DocBaseType", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(_sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DocbaseType.Add(new VIS_DocbaseType() { DocbaseType = Convert.ToString(ds.Tables[0].Rows[i]["DocBaseType"]), Name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]) });
                }
                ds.Dispose();
            }
            return DocbaseType;
        }
        /// <summary>
        /// to get DocTypes for Invoice grid filter
        /// </summary>
        /// <returns>List of Doc Types</returns>
        public List<VIS_DocType> GetDocType()
        {
            List<VIS_DocType> DocType = new List<VIS_DocType>();
            string _sql = @"SELECT C_DocType.NAME, C_DocType.C_DOCTYPE_ID FROM C_DocType C_DOCTYPE 
                            INNER JOIN C_DocBaseType DB ON (C_DocType.DOCBASETYPE=DB.DOCBASETYPE )
                            WHERE DB.DOCBASETYPE IN ('APR','API','ARC','APC','ARI') AND C_DocType.ISACTIVE='Y'";
            //string _sql = "SELECT C_DocType.Name,C_DocType_ID FROM C_DocType INNER JOIN c_docbasetype ON c_doctype.docbasetype = c_docbasetype.docbasetype WHERE  C_DocBaseType_ID IN (SELECT AD_Reference_ID FROM AD_Column WHERE ColumnName ='DocBaseType' AND AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE TableName='C_DocType')) AND c_doctype.isactive='Y'";
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

        /// <summary>
        /// To get DocumentType for Payment grid to filter the Records
        /// </summary>
        /// <returns>List of Docment Type</returns>
        public List<VIS_DocType> GetpayDocType()
        {
            List<VIS_DocType> DocType = new List<VIS_DocType>();
            string _sql = @"SELECT C_DocType.NAME, C_DocType.C_DOCTYPE_ID FROM C_DOCTYPE C_DocType 
                            INNER JOIN C_DocBaseType DB ON (C_DocType.DOCBASETYPE=DB.DOCBASETYPE )
                            WHERE DB.DOCBASETYPE IN ('APP','ARR') AND C_DocType.ISACTIVE='Y'";
            //string _sql = "SELECT C_DocType.Name,C_DocType_ID FROM C_DocType INNER JOIN c_docbasetype ON c_doctype.docbasetype = c_docbasetype.docbasetype WHERE  C_DocBaseType_ID IN (SELECT AD_Reference_ID FROM AD_Column WHERE ColumnName ='DocBaseType' AND AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE TableName='C_DocType')) AND c_doctype.isactive='Y'";
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

        /// <summary>
        /// to get PaymentType in the Cash Journal Line to filter the Records.
        /// </summary>
        /// <returns>List of Payment Types</returns>
        public List<VIS_PayType> GetPaymentType()
        {
            List<VIS_PayType> payType = new List<VIS_PayType>();
            string _sql = "SELECT Value,Name FROM AD_Ref_List WHERE AD_Reference_ID=(SELECT AD_Reference_Value_ID FROM AD_Column WHERE ColumnName ='VSS_PAYMENTTYPE' AND AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE TableName='C_CashLine')) AND IsActive='Y'";
            _sql = MRole.GetDefault(ctx).AddAccessSQL(_sql, "AD_Ref_List", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            DataSet ds = DB.ExecuteDataset(_sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    payType.Add(new VIS_PayType() { Name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]), Value = Convert.ToString(ds.Tables[0].Rows[i]["Value"]) });
                }
                ds.Dispose();
            }
            return payType;
        }
        //Neha

        /// <summary>
        /// TO get currency precision from currency window
        /// </summary>
        /// <param name="_C_Currency_ID">Currency</param>
        /// <returns>precision of currency</returns>
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
        /// <returns>AD_Org_ID and Organization Name</returns> //Added by Koteswar on 10/07/2020 
        public List<NameValue> GetOrg(Ctx ctx)
        {
            List<NameValue> retValue = new List<NameValue>();
            string _sql = " SELECT AD_Org.AD_Org_ID, AD_Org.Name FROM AD_Org AD_Org WHERE AD_Org.AD_Org_ID NOT IN (0) AND AD_Org.IsSummary='N' AND AD_Org.IsActive='Y' AND AD_Org.IsCostCenter='N' AND AD_Org.IsProfitCenter='N' ";
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

        ///  <summary>
        /// Get all Organization which are accessable by login user
        /// </summary>        
        /// <param name="ctx"> Context Object </param>
        /// <returns>AD_Org_ID and Organization Name</returns> //Added by manjot on 27/02/2019 
        public List<NameValue> GetOrganization(Ctx ctx)
        {
            List<NameValue> retValue = new List<NameValue>();
            string _sql = " SELECT AD_Org.AD_Org_ID, AD_Org.Name FROM AD_Org AD_Org WHERE AD_Org.AD_Org_ID NOT IN (0) AND AD_Org.IsSummary='N' AND AD_Org.IsActive='Y' AND AD_Org.IsCostCenter='N' AND AD_Org.IsProfitCenter='N' ";
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
        /// <param name="isPayment"> Payment </param>
        /// <param name="trx"> Trx </param>
        /// <returns>string Value</returns>it'll return either empty string or any error msg
        /// </summary>
        public string ValidateRecords(List<Dictionary<string, string>> rows, string colName, bool isCash, bool isInvoice, bool isPayment, Trx trx)
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
                else if (isPayment)
                {
                    str = (@" SELECT PROCESSING, isAllocated FROM C_Payment WHERE C_PAYMENT_ID IN (" + msg.ToString() + ") FOR UPDATE ");
                    updateQry = (" UPDATE C_Payment SET PROCESSING ='Y' WHERE C_PAYMENT_ID IN (" + msg.ToString() + ")");
                }
                else
                {
                    str = (@" SELECT GL.PROCESSING, GLL.isAllocated from GL_JOURNAL GL INNER JOIN GL_JOURNALLINE GLL ON GL.GL_JOURNAL_ID = GLL.GL_JOURNAL_ID WHERE GLL.GL_JOURNALLINE_ID IN (" + msg.ToString() + ") FOR UPDATE ");
                    updateQry = (" UPDATE GL_JOURNAL SET PROCESSING ='Y' WHERE GL_JOURNAL_ID IN (SELECT DISTINCT(GL_JOURNAL_ID) from GL_Journalline WHERE GL_JOURNALLine_ID IN (" + msg.ToString() + "))");
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
                else
                {
                    //VA228:Return when any of selected record under processing assigned by amit
                    return Msg.GetMsg(ctx, "VIS_SelectedRecordsUnderProcessing");
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
        /// <param name="isPayment"> Payment </param>
        /// <param name="trx"> Trx </param>
        /// </summary>
        public void SetIsprocessingFalse(List<Dictionary<string, string>> rows, string colName, bool isCash, bool isInvoice, bool isPayment, Trx trx)
        {
            StringBuilder msg = new StringBuilder();
            //this condition Check's the list is null or not
            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    int ID = Util.GetValueOfInt(rows[i][colName]);
                    msg.Append(ID);
                    if ((rows.Count > 1) && (i != rows.Count - 1))
                        msg.Append(",");
                }
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
                else if (isPayment)
                {
                    updated = DB.ExecuteQuery(" UPDATE C_Payment SET PROCESSING ='N' WHERE C_PAYMENT_ID IN (" + msg.ToString() + ")", null, trx);
                }
                else
                {
                    updated = DB.ExecuteQuery(" UPDATE GL_JOURNAL SET PROCESSING ='N' WHERE GL_JOURNAL_ID IN (" + msg.ToString() + ")", null, trx);
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
            ds = DB.ExecuteDataset(@" SELECT C_BPartnerRelation_ID FROM C_BP_Relation WHERE C_BPartner_ID = " + C_BPartner_ID + " AND ispayfrom = 'Y' ");
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

        /// <summary>
        /// To get all the unallocated GL Lines
        /// <param name="AD_Org_ID">Organization</param>
        /// <param name="_C_Currency_ID">Currency</param>
        /// <param name="_C_BPartner_ID">Business Partner</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Page Size</param>
        /// <param name="fromDate">From Date</param>
        /// <param name="toDate">To Date</param>
        /// <param name="srchText">Search Document NO</param>
        /// <param name="chk"> bool MultiCurrency </param>
        /// <param name="isInterComp"> Inter Company Flag </param>
        /// <returns>No of unallocated GL Lines</returns>
        public List<GLData> GetGLData(int AD_Org_ID, int _C_Currency_ID, int _C_BPartner_ID, int page, int size, DateTime? fromDate, DateTime? toDate, string srchText, bool chk, bool isInterComp)
        {
            List<GLData> glData = new List<GLData>();
            StringBuilder sql = new StringBuilder();
            MCurrency objCurrency = MCurrency.Get(ctx, _C_Currency_ID);
            sql.Append(@" SELECT EV.AccountType, JL.C_BPARTNER_ID,  CB.ISCUSTOMER,  CB.ISVENDOR, J.DATEDOC, J.DATEACCT, J.DOCUMENTNO,  NVL(JL.AMTSOURCEDR, 0),  NVL(JL.AMTSOURCECR,0),c.ISO_Code AS ISO_CODE,
                JL.C_CONVERSIONTYPE_ID, CT.name as CONVERSIONNAME, o.AD_Org_ID, o.Name, EV.Name AS Account,
                NVL(ROUND(CURRENCYCONVERT(JL.AMTSOURCEDR ,JL.C_CURRENCY_ID ," + _C_Currency_ID + @",J.DATEACCT ,Jl.C_CONVERSIONTYPE_ID ,J.AD_CLIENT_ID ,J.AD_ORG_ID ), " + objCurrency.GetStdPrecision() + @"),0) as AMTACCTDR, 
                NVL(ROUND(currencyConvert(JL.AMTSOURCECR ,jl.C_Currency_ID ," + _C_Currency_ID + @",j.DATEACCT ,jl.C_ConversionType_ID ,j.AD_Client_ID ,j.AD_Org_ID ), " + objCurrency.GetStdPrecision() + @"),0) AS AMTACCTCR, 
                j.GL_Journal_ID, jl.GL_JOURNALLINE_ID FROM GL_Journal j 
                INNER JOIN AD_Org o ON o.AD_Org_ID = j.AD_Org_ID 
                INNER JOIN GL_JOURNALLINE JL ON JL.GL_JOURNAL_ID=J.GL_JOURNAL_ID 
                INNER JOIN C_Currency c ON c.C_Currency_ID = jl.C_Currency_ID 
                INNER JOIN C_CONVERSIONTYPE CT ON ct.C_CONVERSIONTYPE_ID= jl.C_CONVERSIONTYPE_ID INNER JOIN C_ELEMENTVALUE EV ON ev.c_elementvalue_ID=JL.ACCOUNT_ID INNER JOIN C_BPARTNER CB
                ON cb.C_BPartner_ID = jl.C_BPartner_ID WHERE j.docstatus IN ('CO','CL') AND jl.isallocated ='N' AND EV.isAllocationrelated='Y' AND EV.AccountType IN ('A','L')");

            //filter based on inter company parameter
            if (!isInterComp)
            {
                //filter based on Organization
                if (AD_Org_ID != 0)
                {
                    sql.Append(" AND J.AD_Org_ID=" + AD_Org_ID);
                }
            }
            if (!chk)
            {
                sql.Append(" AND JL.C_Currency_ID=" + _C_Currency_ID);
            }
            if (srchText != string.Empty)
            {
                //JID_1793 -- when search text contain "=" then serach with documnet no only
                if (srchText.Contains("="))
                {
                    String[] myStringArray = srchText.TrimStart(new Char[] { ' ', '=' }).Split(',');
                    if (myStringArray.Length > 0)
                    {
                        sql.Append(" AND UPPER(J.DOCUMENTNO) IN ( ");
                        for (int z = 0; z < myStringArray.Length; z++)
                        {
                            if (z != 0)
                            { sql.Append(","); }
                            sql.Append(" UPPER('" + myStringArray[z].Trim() + "')");
                        }
                        sql.Append(")");
                    }
                }
                else
                {
                    sql.Append(" AND UPPER(J.DOCUMENTNO) LIKE UPPER('%" + srchText.Trim() + "%')");
                }
            }

            if (_C_BPartner_ID > 0)
                sql.Append(" AND JL.C_BPartner_ID= " + _C_BPartner_ID);

            if (fromDate != null)
            {
                if (toDate != null)
                {
                    sql.Append(" AND J.DATEDOC BETWEEN " + GlobalVariable.TO_DATE(fromDate, true) + " AND " + GlobalVariable.TO_DATE(toDate, true));
                }
                else
                {
                    sql.Append(" AND J.DATEDOC >= " + GlobalVariable.TO_DATE(fromDate, true));
                }
            }
            if (fromDate == null && toDate != null)
            {
                sql.Append(" AND J.DATEDOC <=" + GlobalVariable.TO_DATE(toDate, true));
            }

            sql.Append(" ORDER BY J.DOCUMENTNO ASC");
            //DataSet ds = DB.ExecuteDataset(sql.ToString(), null, null);
            //added Page size for GL Journal grid
            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(sql.ToString(), page, size);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                decimal? openAmt = 0;
                decimal? alreadyPaidAmt = 0;
                bool isVendor = false;
                bool isCustomer = false;
                //MJournalLine aline = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    isCustomer = ds.Tables[0].Rows[i]["ISCUSTOMER"].ToString() == "Y" ? true : false;
                    isVendor = ds.Tables[0].Rows[i]["ISVENDOR"].ToString() == "Y" ? true : false;
                    GLData gData = new GLData();
                    gData.SelectRow = "false";
                    gData.GLRecords = ds.Tables[0].Rows.Count;
                    gData.DATEDOC = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["DATEDOC"]);
                    gData.DATEACCT = Util.GetValueOfDateTime(ds.Tables[0].Rows[i]["DATEACCT"]);
                    gData.DOCUMENTNO = ds.Tables[0].Rows[i]["DOCUMENTNO"].ToString();
                    //added new Column Account 
                    gData.Account = ds.Tables[0].Rows[i]["Account"].ToString();
                    gData.C_BPartner_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPARTNER_ID"]);
                    gData.isCustomer = isCustomer;
                    gData.isVendor = isVendor;
                    gData.Isocode = ds.Tables[0].Rows[i]["ISO_CODE"].ToString();
                    gData.C_ConversionType_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_CONVERSIONTYPE_ID"]);
                    gData.ConversionName = Util.GetValueOfString(ds.Tables[0].Rows[i]["ConversionName"]);
                    gData.OpenAmount = Util.GetNullableDecimal(ds.Tables[0].Rows[i]["AmtAcctCr"]);
                    gData.GL_Journal_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["GL_Journal_ID"]);
                    gData.AD_Org_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Org_ID"]);
                    gData.OrgName = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]).ToString();
                    gData.GL_JOURNALLINE_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["GL_JOURNALLINE_ID"]);
                    alreadyPaidAmt = getAlreadyPaidAmount(_C_Currency_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["GL_JOURNALLINE_ID"]));
                    if (Util.GetNullableDecimal(ds.Tables[0].Rows[i]["AMTACCTDR"]) > 0)
                    {
                        openAmt = Util.GetNullableDecimal(ds.Tables[0].Rows[i]["AMTACCTDR"]);
                        openAmt = openAmt - alreadyPaidAmt;
                        if (openAmt == 0)
                        {
                            //aline = new MJournalLine(ctx, Util.GetValueOfInt(ds.Tables[0].Rows[i]["GL_JOURNALLINE_ID"]), null);
                            //if (!aline.Save())
                            //{
                            //    _log.SaveError("Error: ", "Allocation Line not created");
                            //}
                            continue;
                        }
                        openAmt = getAmount(Util.GetValueOfString(ds.Tables[0].Rows[i]["AccountType"]), isCustomer, isVendor, openAmt, 0);
                        gData.OpenAmount = openAmt;
                        gData.ConvertedAmount = getAmount(Util.GetValueOfString(ds.Tables[0].Rows[i]["AccountType"]), isCustomer, isVendor, Util.GetNullableDecimal(ds.Tables[0].Rows[i]["AMTACCTDR"]), 0);
                        gData.AppliedAmt = 0;
                    }
                    else if (Util.GetNullableDecimal(ds.Tables[0].Rows[i]["AmtAcctCr"]) > 0)
                    {
                        openAmt = Util.GetNullableDecimal(ds.Tables[0].Rows[i]["AmtAcctCr"]);
                        openAmt = openAmt - alreadyPaidAmt;
                        if (openAmt == 0)
                        {
                            //aline = new MJournalLine(ctx, Util.GetValueOfInt(ds.Tables[0].Rows[i]["GL_JOURNALLINE_ID"]), null);
                            //if (!aline.Save())
                            //{
                            //    _log.SaveError("Error: ", "Allocation Line not created");
                            //}
                            continue;
                        }
                        openAmt = getAmount(Util.GetValueOfString(ds.Tables[0].Rows[i]["AccountType"]), isCustomer, isVendor, 0, openAmt);
                        gData.OpenAmount = openAmt;
                        gData.ConvertedAmount = getAmount(Util.GetValueOfString(ds.Tables[0].Rows[i]["AccountType"]), isCustomer, isVendor, 0, Util.GetNullableDecimal(ds.Tables[0].Rows[i]["AmtAcctCr"]));
                        gData.AppliedAmt = 0;
                    }
                    if (gData.OpenAmount == 0) { continue; }
                    glData.Add(gData);
                }
            }
            return glData;
        }

        /// <summary>
        /// to get amount from given conditions
        /// </summary>
        /// <param name="AccountType">Account Type</param>
        /// <param name="isCustomer">Is Customer</param>
        /// <param name="isVendor">Is Vendor</param>
        /// <param name="crAmt">Source Credit</param>
        /// <param name="dbAmt">Source Debit</param>
        /// <returns> Amount </returns>
        public decimal? getAmount(string AccountType, bool isCustomer, bool isVendor, decimal? dbAmt, decimal? crAmt)
        {
            decimal? amt = 0;
            //to get amount for gl line as suggested by ashish on 28 june 2020 we need to check if dbAmount then return positive otherwise negeative 
            if (dbAmt != 0)
            {
                amt = Math.Abs(dbAmt.Value);
            }
            if (crAmt != 0 && crAmt > 0)
            {
                amt = Decimal.Negate(crAmt.Value);
            }
            //if (isCustomer && isVendor) // Customer & Vendor Both
            //{
            //    if (AccountType == "A" || AccountType == "L")
            //    {
            //        if (dbAmt > 0)
            //        {
            //            amt = dbAmt;
            //        }
            //        if (crAmt > 0)
            //        {
            //            amt = (-1 * crAmt);
            //        }
            //    }
            //}
            //else if (isCustomer && !isVendor) // Only Customer
            //{
            //    if (AccountType == "A")
            //    {
            //        if (dbAmt > 0)
            //        {
            //            amt = dbAmt;
            //        }
            //        if (crAmt > 0)
            //        {
            //            amt = (-1 * crAmt);
            //        }
            //    }

            //    if (AccountType == "L")
            //    {
            //        if (dbAmt > 0)
            //        {
            //            amt = dbAmt;
            //        }
            //        if (crAmt > 0)
            //        {
            //            amt = (-1 * crAmt);
            //        }
            //    }

            //}
            //else if (!isCustomer && isVendor) // Only Vendor
            //{
            //    if (AccountType == "A")
            //    {
            //        if (dbAmt > 0)
            //        {
            //            amt = (-1 * dbAmt);
            //        }
            //        if (crAmt > 0)
            //        {
            //            amt = crAmt;
            //        }
            //    }
            //    if (AccountType == "L")
            //    {
            //        if (dbAmt > 0)
            //        {
            //            amt = (-1 * dbAmt);
            //        }
            //        if (crAmt > 0)
            //        {
            //            amt = crAmt;
            //        }
            //    }
            //}
            return amt;
        }

        /// <summary>
        /// To get already paid amount
        /// </summary>
        /// <param name="C_Currency_ID"> Currency ID</param>
        /// <param name="GL_JOURNALLINE_ID">GL Journal Line ID</param>
        /// <returns>Already Paid Amount</returns>
        public decimal? getAlreadyPaidAmount(int C_Currency_ID, int GL_JOURNALLINE_ID)
        {
            string sql = @"SELECT NVL(SUM(ROUND(CURRENCYCONVERT(AL.AMOUNT ,AR.C_CURRENCY_ID ," + C_Currency_ID + @",AR.DATEACCT ,AR.C_CONVERSIONTYPE_ID ,AR.AD_CLIENT_ID ,AR.AD_ORG_ID ), 2)),0) AS PaidAmt
                        FROM C_ALLOCATIONLINE AL
                        INNER JOIN C_ALLOCATIONHDR AR
                        ON ar.C_AllocationHdr_ID   =al.C_AllocationHdr_ID
                        WHERE al.GL_JOURNALLINE_ID = " + GL_JOURNALLINE_ID + " AND AR.DOCSTATUS ='CO'";
            decimal? amt = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
            if (amt < 0)
                amt = -1 * amt;
            return amt;
        }

        /// <summary>
        /// to create view allocation against GL journal line
        /// </summary>
        /// <param name="rowsPayment">Selected payment data</param>
        /// <param name="rowsInvoice">Selected invoice data</param>
        /// <param name="rowsCash"> Selected cash line data</param>
        /// <param name="rowsGL"> Selected gl line data</param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="C_Currency_ID">Currency</param>
        /// <param name="C_BPartner_ID"> Business Partner</param>
        /// <param name="AD_Org_ID">Org ID</param>
        /// <param name="C_CurrencyType_ID">Currency ConversionType ID</param>
        /// <param name="applied">Applied Amount</param>
        /// <param name="DateAcct">Account Date</param>
        /// <param name="discount">Discount Amount</param>
        /// <param name="open">Open Amount</param>
        /// <param name="payment">Payment Amount or Applied Amount</param>
        /// <param name="writeOff">WrittenOff Amount</param>
        /// <param name="conversionDate"> Conversion Date </param>
        /// <param name="chkMultiCurrency"> bool MultiCurrency </param>
        /// <returns>Will Return Msg Either Allocation is Saved or Not Saved</returns>
        public string SaveGLData(List<Dictionary<string, string>> rowsPayment, List<Dictionary<string, string>> rowsInvoice, List<Dictionary<string, string>> rowsCash, List<Dictionary<string, string>> rowsGL, DateTime DateTrx, int _windowNo, int C_Currency_ID, int C_BPartner_ID, int AD_Org_ID, int C_CurrencyType_ID, DateTime DateAcct, string applied, string discount, string open, string payment, string writeOff, DateTime conversionDate, bool chkMultiCurrency)
        {
            decimal paid = 0; decimal actualAmt = 0;
            decimal amtToAllocate = 0, remainingAmt = 0, netAmt = 0;
            decimal balanceAmt = 0;
            string msg = string.Empty;
            int C_InvoicePaySchedule_ID = 0;
            int Neg_C_InvoicePaySchedule_Id = 0;

            Trx trx = Trx.GetTrx(Trx.CreateTrxName("GL"));

            msg = ValidateRecords(rowsPayment, "cpaymentid", false, false, true, trx); //Payment
            if (msg != string.Empty)
            {
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsCash, "ccashlineid", true, false, false, trx); //CashLine
            if (msg != string.Empty)
            {
                //set isProcessing false
                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsInvoice, "c_invoicepayschedule_id", false, true, false, trx); //InvoicePaySchedule
            if (msg != string.Empty)
            {
                //set isProcessing false
                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = ValidateRecords(rowsGL, "GL_JournalLine_ID", false, false, false, trx); //GL_JOURNALLINE
            if (msg != string.Empty)
            {
                //set isProcessing false
                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                trx.Rollback();
                trx.Close();
                return msg;
            }

            msg = string.Empty;

            #region GL-Loop
            List<int> glList = new List<int>(rowsGL.Count);
            List<Decimal> amountList = new List<Decimal>(rowsGL.Count);
            Decimal glAppliedAmt = Env.ZERO;
            for (int i = 0; i < rowsGL.Count; i++)
            {
                //  Payment line is selected
                //  Payment variables
                int GL_JournalLine_ID = Util.GetValueOfInt(rowsGL[i]["GL_JournalLine_ID"]);
                glList.Add(GL_JournalLine_ID);
                //
                Decimal GLAmt = Util.GetValueOfDecimal(rowsGL[i]["AppliedAmt"]);  //  Applied Payment
                amountList.Add(GLAmt);
                //
                glAppliedAmt = Decimal.Add(glAppliedAmt, GLAmt);
            }
            #endregion

            List<Dictionary<string, string>> negList = new List<Dictionary<string, string>>();
            Decimal negTotAmt = 0;
            if (rowsInvoice.Count != 0)
            {
                foreach (var item in rowsInvoice)
                {
                    if (Util.GetValueOfDecimal(item[applied]) < 0)
                    {
                        negList.Add(item);
                        negTotAmt = Decimal.Add(negTotAmt, Util.GetValueOfDecimal(item[applied]));
                    }
                }
            }
            else if (rowsPayment.Count != 0)
            {
                foreach (var item in rowsPayment)
                {
                    if (Util.GetValueOfDecimal(item[payment]) < 0)
                    {
                        negList.Add(item);
                        negTotAmt = Decimal.Add(negTotAmt, Util.GetValueOfDecimal(item[payment]));
                    }
                }
            }
            else if (rowsCash.Count != 0)
            {
                foreach (var item in rowsCash)
                {
                    if (Util.GetValueOfDecimal(item[payment]) < 0)
                    {
                        negList.Add(item);
                        negTotAmt = Decimal.Add(negTotAmt, Util.GetValueOfDecimal(item[payment]));
                    }
                }
            }

            List<int> neg_Invoice_IDs = new List<int>(negList.Count);

            MAllocationHdr alloc = new MAllocationHdr(ctx, true,	//	manual
               DateTime.Now, C_Currency_ID, ctx.GetContext("#AD_User_Name"), trx);
            alloc.SetAD_Org_ID(AD_Org_ID);
            alloc.SetDateAcct(DateAcct);// to set Account date on allocation header because posting and conversion are calculating on the basis of Date Account
            alloc.Set_Value("C_ConversionType_ID", C_CurrencyType_ID); // to set Conversion Type on allocation header because posting and conversion are calculating on the basis of Conversion Type
            alloc.SetDateTrx(DateTrx);
            alloc.Set_Value("C_BPartner_ID", C_BPartner_ID);
            //when select a MultiCurrency then the ConversionDate will set into AllocationHdr
            if (chkMultiCurrency)
            {
                alloc.SetConversionDate(conversionDate);
            }

            //	For all invoices
            //int invoiceLines = 0;
            MInvoicePaySchedule mpay = null;
            MInvoice invoice = null;
            bool isScheduleAllocated = false;
            bool is_NegScheduleAllocated = false;

            if (alloc.Save())
            {
                //create allocation line if cash to cash row selected
                for (int i = 0; i < rowsCash.Count; i++)
                {
                    //amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"]));
                    amtToAllocate = Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"]) - Math.Abs(Util.GetValueOfDecimal(rowsCash[i]["paidAmt"]));
                    remainingAmt = amtToAllocate;
                    if (Util.GetValueOfBool(rowsCash[i]["IsPaid"]))
                        continue;

                    if (remainingAmt > 0)
                    {
                        MAllocationLine aLine = null;
                        for (int j = 0; j < negList.Count; j++)
                        {
                            if (Util.GetValueOfBool(negList[j]["IsPaid"]))
                                continue;

                            actualAmt = Math.Abs(Util.GetValueOfDecimal(negList[j]["AppliedAmt"])) - Math.Abs(Util.GetValueOfDecimal(negList[j]["paidAmt"]));
                            if (remainingAmt >= actualAmt)
                            {
                                remainingAmt -= actualAmt;
                                netAmt = actualAmt;
                                balanceAmt = 0;
                            }
                            else
                            {
                                netAmt = remainingAmt;
                                balanceAmt = actualAmt - remainingAmt;
                                remainingAmt = 0;
                            }
                            aLine = new MAllocationLine(alloc, netAmt, Env.ZERO, Env.ZERO, Env.ZERO);
                            aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                            aLine.SetRef_CashLine_ID(Util.GetValueOfInt(negList[j]["ccashlineid"]));
                            aLine.SetDateTrx(DateTrx);
                            aLine.SetC_CashLine_ID(Util.GetValueOfInt(rowsCash[i]["ccashlineid"]));
                            if (!aLine.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                                }
                                else
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                                }
                                //set Isprocessing false
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }
                            //for -ve Value of Cash journal
                            aLine = new MAllocationLine(alloc, Decimal.Negate(netAmt), Env.ZERO, Env.ZERO, Env.ZERO);
                            aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                            aLine.SetC_CashLine_ID(Util.GetValueOfInt(negList[j]["ccashlineid"]));
                            aLine.SetDateTrx(DateTrx);
                            aLine.SetRef_CashLine_ID(Util.GetValueOfInt(rowsCash[i]["ccashlineid"]));
                            if (!aLine.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                                }
                                else
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                                }
                                //set Isprocessing false
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }
                            paid = Util.GetValueOfDecimal(negList[j]["paidAmt"]) + Decimal.Negate(netAmt);
                            negList[j]["paidAmt"] = paid.ToString();
                            rowsCash[i]["paidAmt"] = (Util.GetValueOfDecimal(rowsCash[i]["paidAmt"]) + netAmt).ToString();

                            if (balanceAmt == 0)
                            {
                                negList[j]["IsPaid"] = true.ToString();
                            }
                            if (remainingAmt == 0)
                            {
                                rowsCash[i]["IsPaid"] = true.ToString();
                                break;
                            }
                        }
                    }
                }
                //create allocation line if cash row selected
                for (int i = 0; i < rowsCash.Count; i++)
                {
                    //amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"]));
                    amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"])) - Math.Abs(Util.GetValueOfDecimal(rowsCash[i]["paidAmt"]));
                    remainingAmt = Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"]) > 0 ? amtToAllocate : Decimal.Negate(amtToAllocate);
                    if (Util.GetValueOfBool(rowsCash[i]["IsPaid"]))
                        continue;

                    for (int j = 0; j < rowsGL.Count; j++)
                    {
                        if (Util.GetValueOfBool(rowsGL[j]["IsPaid"]))
                            continue;

                        actualAmt = Util.GetValueOfDecimal(rowsGL[j]["AppliedAmt"]) - Util.GetValueOfDecimal(rowsGL[j]["paidAmt"]);
                        // check match payment with DebitAmt && receipt with CreditAmt
                        // not receipt with DebitAmt
                        if (remainingAmt >= 0 && actualAmt <= 0)
                            continue;
                        if (remainingAmt <= 0 && actualAmt >= 0)
                            continue;

                        if (Math.Abs(remainingAmt) >= Math.Abs(actualAmt))
                        {
                            remainingAmt -= actualAmt;
                            netAmt = actualAmt;
                            balanceAmt = 0;
                        }
                        else
                        {
                            netAmt = remainingAmt;
                            balanceAmt = actualAmt - remainingAmt;
                            remainingAmt = 0;
                        }
                        MAllocationLine aLine = new MAllocationLine(alloc, netAmt, Env.ZERO, Env.ZERO, Env.ZERO);
                        aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                        aLine.Set_Value("GL_JournalLine_ID", Util.GetValueOfInt(rowsGL[j]["GL_JournalLine_ID"]));
                        aLine.SetDateTrx(DateTrx);
                        aLine.SetC_CashLine_ID(Util.GetValueOfInt(rowsCash[i]["ccashlineid"]));
                        if (!aLine.Save())
                        {
                            _log.SaveError("Error: ", "Allocation not created");
                            trx.Rollback();
                            trx.Close();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                            }
                            else
                            {
                                msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                            }
                            //set Isprocessing false
                            Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                            return msg;
                        }
                        else
                        {
                            paid = Util.GetValueOfDecimal(rowsGL[j]["paidAmt"]) + netAmt;
                            rowsGL[j]["paidAmt"] = paid.ToString();

                            if (balanceAmt == 0)
                            {
                                rowsGL[j]["IsPaid"] = true.ToString();
                            }
                            if (remainingAmt == 0)
                            {
                                rowsCash[i]["IsPaid"] = true.ToString();
                                break;
                            }

                        }
                    }
                }
                MPayment objPayment = null;
                //create allocation line if payment to payment row selected
                for (int i = 0; i < rowsPayment.Count; i++)
                {
                    //amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsCash[i]["AppliedAmt"]));
                    amtToAllocate = Util.GetValueOfDecimal(rowsPayment[i]["AppliedAmt"]) - Math.Abs(Util.GetValueOfDecimal(rowsPayment[i]["paidAmt"]));
                    remainingAmt = amtToAllocate;
                    if (Util.GetValueOfBool(rowsPayment[i]["IsPaid"]))
                        continue;

                    if (remainingAmt > 0)
                    {
                        MAllocationLine aLine = null;
                        for (int j = 0; j < negList.Count; j++)
                        {
                            if (Util.GetValueOfBool(negList[j]["IsPaid"]))
                                continue;

                            objPayment = new MPayment(ctx, Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]), trx);

                            actualAmt = Math.Abs(Util.GetValueOfDecimal(negList[j]["AppliedAmt"])) - Math.Abs(Util.GetValueOfDecimal(negList[j]["paidAmt"]));
                            if (Math.Abs(remainingAmt) >= actualAmt)
                            {
                                remainingAmt -= actualAmt;
                                netAmt = actualAmt;
                                balanceAmt = 0;
                            }
                            else
                            {
                                netAmt = remainingAmt;
                                balanceAmt = actualAmt - remainingAmt;
                                remainingAmt = 0;
                            }
                            aLine = new MAllocationLine(alloc, netAmt, Env.ZERO, Env.ZERO, Env.ZERO);
                            aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                            aLine.SetRef_Payment_ID(Util.GetValueOfInt(negList[j]["cpaymentid"]));
                            aLine.SetDateTrx(DateTrx);
                            aLine.SetC_Payment_ID(Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]));
                            // set withholding amount based on porpotionate
                            if (objPayment.GetC_Withholding_ID() > 0 || objPayment.GetBackupWithholding_ID() > 0)
                            {
                                DataSet ds = DB.ExecuteDataset(@"SELECT (SELECT ROUND((" + netAmt + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.C_Withholding_ID ) AS withholdingAmt,
                                                  (SELECT ROUND((" + netAmt + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.BackupWithholding_ID ) AS BackupwithholdingAmt
                                                FROM C_Payment WHERE C_Payment.IsActive   = 'Y' AND C_Payment.C_Payment_ID = " + Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]), null, trx);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    aLine.SetWithholdingAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["withholdingAmt"]));
                                    aLine.SetBackupWithholdingAmount(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["BackupwithholdingAmt"]));
                                }
                            }
                            if (!aLine.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                                }
                                else
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                                }
                                //Set Isprocessing False
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }

                            objPayment = new MPayment(ctx, Util.GetValueOfInt(negList[j]["cpaymentid"]), trx);

                            aLine = new MAllocationLine(alloc, Decimal.Negate(netAmt), Env.ZERO, Env.ZERO, Env.ZERO);
                            aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                            aLine.SetC_Payment_ID(Util.GetValueOfInt(negList[j]["cpaymentid"]));
                            aLine.SetDateTrx(DateTrx);
                            aLine.SetRef_Payment_ID(Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]));
                            // set withholding amount based on porpotionate
                            if (objPayment.GetC_Withholding_ID() > 0 || objPayment.GetBackupWithholding_ID() > 0)
                            {
                                DataSet ds = DB.ExecuteDataset(@"SELECT (SELECT ROUND((" + Decimal.Negate(netAmt) + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.C_Withholding_ID ) AS withholdingAmt,
                                                  (SELECT ROUND((" + Decimal.Negate(netAmt) + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.BackupWithholding_ID ) AS BackupwithholdingAmt
                                                FROM C_Payment WHERE C_Payment.IsActive   = 'Y' AND C_Payment.C_Payment_ID = " + Util.GetValueOfInt(negList[j]["cpaymentid"]), null, trx);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    aLine.SetWithholdingAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["withholdingAmt"]));
                                    aLine.SetBackupWithholdingAmount(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["BackupwithholdingAmt"]));
                                }
                            }
                            if (!aLine.Save())
                            {
                                _log.SaveError("Error: ", "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                                }
                                else
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                                }
                                //Set Isprocessing False
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }
                            paid = Util.GetValueOfDecimal(negList[j]["paidAmt"]) + Decimal.Negate(netAmt);
                            negList[j]["paidAmt"] = paid.ToString();
                            rowsPayment[i]["paidAmt"] = Decimal.Add(Util.GetValueOfDecimal(rowsPayment[i]["paidAmt"]), netAmt).ToString();
                            if (balanceAmt == 0)
                            {
                                negList[j]["IsPaid"] = true.ToString();
                            }
                            if (remainingAmt == 0)
                            {
                                rowsPayment[i]["IsPaid"] = true.ToString();
                                break;
                            }
                        }
                    }
                }

                //create allocation line if payment row selected
                for (int i = 0; i < rowsPayment.Count; i++)
                {
                    //amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsPayment[i]["AppliedAmt"]));
                    amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsPayment[i]["AppliedAmt"])) - Math.Abs(Util.GetValueOfDecimal(rowsPayment[i]["paidAmt"]));
                    remainingAmt = Util.GetValueOfDecimal(rowsPayment[i]["AppliedAmt"]) > Env.ZERO ? amtToAllocate : Decimal.Negate(amtToAllocate);
                    if (Util.GetValueOfBool(rowsPayment[i]["IsPaid"]))
                        continue;

                    objPayment = new MPayment(ctx, Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]), trx);

                    for (int j = 0; j < rowsGL.Count; j++)
                    {
                        if (Util.GetValueOfBool(rowsGL[j]["IsPaid"]))
                            continue;

                        actualAmt = Util.GetValueOfDecimal(rowsGL[j]["AppliedAmt"]) - Util.GetValueOfDecimal(rowsGL[j]["paidAmt"]);
                        // check match payment with DebitAmt && receipt with CreditAmt
                        // not receipt with DebitAmt
                        if (remainingAmt >= 0 && actualAmt <= 0)
                            continue;
                        if (remainingAmt <= 0 && actualAmt >= 0)
                            continue;

                        if (Math.Abs(remainingAmt) >= Math.Abs(actualAmt))
                        {
                            remainingAmt -= actualAmt;
                            netAmt = actualAmt;
                            balanceAmt = 0;
                        }
                        else
                        {
                            netAmt = remainingAmt;
                            balanceAmt = actualAmt - remainingAmt;
                            remainingAmt = 0;
                        }
                        MAllocationLine aLine = new MAllocationLine(alloc, netAmt, Env.ZERO, Env.ZERO, Env.ZERO);
                        aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                        aLine.Set_Value("GL_JournalLine_ID", Util.GetValueOfInt(rowsGL[j]["GL_JournalLine_ID"]));
                        aLine.SetDateTrx(DateTrx);
                        aLine.SetC_Payment_ID(Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]));

                        // set withholding amount based on porpotionate
                        if (objPayment.GetC_Withholding_ID() > 0 || objPayment.GetBackupWithholding_ID() > 0)
                        {
                            DataSet ds = DB.ExecuteDataset(@"SELECT (SELECT ROUND((" + netAmt + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.C_Withholding_ID ) AS withholdingAmt,
                                                  (SELECT ROUND((" + netAmt + @" * PayPercentage)/100 , 2) AS withholdingAmt
                                                  FROM C_Withholding WHERE C_Withholding_ID = C_Payment.BackupWithholding_ID ) AS BackupwithholdingAmt
                                                FROM C_Payment WHERE C_Payment.IsActive   = 'Y' AND C_Payment.C_Payment_ID = " + Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]), null, trx);
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                aLine.SetWithholdingAmt(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["withholdingAmt"]));
                                aLine.SetBackupWithholdingAmount(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["BackupwithholdingAmt"]));
                            }
                        }

                        if (!aLine.Save())
                        {
                            _log.SaveError("Error: ", "Allocation not created");
                            trx.Rollback();
                            trx.Close();
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated") + ":- " + pp.GetName();
                            }
                            else
                            {
                                msg = Msg.GetMsg(ctx, "VIS_AllocLineNotCreated");
                            }
                            //Set Isprocessing False
                            Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                            return msg;
                        }
                        else
                        {
                            paid = Util.GetValueOfDecimal(rowsGL[j]["paidAmt"]) + netAmt;
                            rowsGL[j]["paidAmt"] = paid.ToString();

                            if (balanceAmt == 0)
                            {
                                rowsGL[j]["IsPaid"] = true.ToString();
                            }
                            if (remainingAmt == 0)
                            {
                                rowsPayment[i]["IsPaid"] = true.ToString();
                                break;
                            }
                        }
                    }
                }

                decimal overUnderAmt, DiscountAmt;
                decimal WriteOffAmt;
                string docbasetype;

                //create allocation line if invoice to invoice row selected
                for (int i = 0; i < rowsInvoice.Count; i++)
                {
                    //isScheduleAllocated = false;
                    //amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]));
                    amtToAllocate = Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]);
                    remainingAmt = amtToAllocate;
                    int C_Invoice_ID;
                    isScheduleAllocated = false;
                    DiscountAmt = Util.GetValueOfDecimal(rowsInvoice[i]["Discount"]);
                    WriteOffAmt = Util.GetValueOfDecimal(rowsInvoice[i]["Writeoff"]);
                    overUnderAmt = Decimal.Subtract(Math.Abs(Util.GetValueOfDecimal(rowsInvoice[i]["Amount"])),
                        Math.Abs(Decimal.Add(Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]), Decimal.Add(Util.GetValueOfDecimal(rowsInvoice[i]["Discount"]), Util.GetValueOfDecimal(rowsInvoice[i]["Writeoff"])))));

                    if (Util.GetValueOfBool(rowsInvoice[i]["IsPaid"]))
                        continue;

                    MAllocationLine aLine = null;
                    MInvoicePaySchedule mpay2 = null;
                    Decimal diffAmt = Env.ZERO;

                    #region GL to Invoice -- create allocation line 
                    if (remainingAmt != 0)
                    {
                        //amtToAllocate = Math.Abs(Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]));
                        amtToAllocate = Decimal.Subtract(Math.Abs(Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"])), Math.Abs(Util.GetValueOfDecimal(rowsInvoice[i]["paidAmt"])));
                        remainingAmt = amtToAllocate;
                        if (Util.GetValueOfBool(rowsInvoice[i]["IsPaid"]))
                            continue;




                        //MJournalLine journalLine = null;
                        for (int j = 0; j < rowsGL.Count; j++)

                        {
                            mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                            invoice = new MInvoice(ctx, Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]), trx);
                            MJournalLine journalLine = new MJournalLine(ctx, Util.GetValueOfInt(rowsGL[j]["GL_JournalLine_ID"]), trx);

                            if (Util.GetValueOfBool(rowsGL[j]["IsPaid"]))
                                continue;

                            docbasetype = Util.GetValueOfString(DB.ExecuteScalar(@" SELECT dt.docbasetype FROM c_invoice i
                                INNER JOIN C_DocType dt ON dt.C_DocType_ID=i.C_DocType_ID WHERE i.c_invoice_id=
                                (SELECT c_invoice_id   FROM c_invoicepayschedule  WHERE 
                                c_invoicepayschedule_id=" + Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]) + ")", null, trx)
                            );

                            actualAmt = Math.Abs(Util.GetValueOfDecimal(rowsGL[j]["AppliedAmt"])) - Math.Abs(Util.GetValueOfDecimal(rowsGL[j]["paidAmt"]));

                            //overUnderAmt = Decimal.Subtract(Math.Abs(Util.GetValueOfDecimal(rowsInvoice[i]["Amount"])),
                            //Math.Abs(Decimal.Add(Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]), Decimal.Add(Util.GetValueOfDecimal(rowsInvoice[i]["Discount"]), Util.GetValueOfDecimal(rowsInvoice[i]["Writeoff"])))));

                            Decimal appliedAmt = Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]);
                            Decimal gLAmount = Util.GetValueOfDecimal(rowsGL[j]["AppliedAmt"]);
                            // check match receipt with receipt && payment with payment
                            // not payment with receipt
                            if (gLAmount >= 0 && appliedAmt >= 0)
                                continue;
                            if (gLAmount <= 0 && appliedAmt <= 0)
                                continue;

                            // if amount is negetive than * by -1 to convert it into positive.
                            if (overUnderAmt < 0)
                                overUnderAmt = -1 * overUnderAmt;

                            if (remainingAmt >= actualAmt)
                            {
                                remainingAmt -= actualAmt;
                                netAmt = actualAmt;
                                balanceAmt = 0;
                            }
                            else
                            {
                                netAmt = remainingAmt;
                                balanceAmt = actualAmt - remainingAmt;
                                remainingAmt = 0;
                            }

                            //if the invoice amount is +ve check the codition with C_InvoicePaySchedule_ID otherwise check with -ve List
                            if (Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]) > 0)
                            {
                                if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                {
                                    overUnderAmt = 0;
                                }
                            }
                            else
                            {
                                //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                if (neg_Invoice_IDs.Contains(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"])))
                                {
                                    overUnderAmt = 0;
                                }
                            }
                            C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                            //InvoicePaySchedule Update/Create
                            if (!isScheduleAllocated)
                            {
                                isScheduleAllocated = true;
                                if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                {
                                    var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, invoice.GetC_Currency_ID(), journalLine.GetDateAcct(), journalLine.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                    if (remainingAmt == Env.ZERO)
                                    {
                                        //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                        diffAmt = GetDifference(invoice, trx);
                                        if (diffAmt != Env.ZERO)
                                        {
                                            mpay.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                    }
                                    else
                                    {
                                        mpay.SetDueAmt(Math.Abs(conertedAmount));
                                    }
                                }
                                else
                                    mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt)),
                                                   Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));

                                if (!mpay.Save(trx))
                                {
                                    msg = ValidateSaveInvoicePaySchedule(trx);
                                    //set Isprocessing false
                                    Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                    return msg;
                                }
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
                                    var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, invoice.GetC_Currency_ID(), journalLine.GetDateAcct(), journalLine.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                    if (remainingAmt == Env.ZERO)
                                    {
                                        //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                        diffAmt = GetDifference(invoice, trx);
                                        mpay2.SetDueAmt(Math.Abs(diffAmt));
                                    }
                                    else
                                    {
                                        mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                    }
                                }
                                else
                                    mpay2.SetDueAmt(Math.Abs(netAmt));

                                if (!mpay2.Save(trx))
                                {
                                    msg = ValidateSaveInvoicePaySchedule(trx);
                                    //set Isprocessing false
                                    Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                    return msg;
                                }
                            }

                            // to set amount -ve on allocation line when AR Credit Memo and AP Invoice 
                            if (docbasetype.Equals(MDocBaseType.DOCBASETYPE_ARCREDITMEMO) || docbasetype.Equals(MDocBaseType.DOCBASETYPE_APINVOICE))
                            {
                                netAmt = Decimal.Negate(netAmt);
                                overUnderAmt = Decimal.Negate(overUnderAmt);
                                //neg_Invoice_IDs.Add(Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));
                            }

                            aLine = new MAllocationLine(alloc, netAmt, DiscountAmt, WriteOffAmt, overUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                            aLine.Set_Value("GL_JournalLine_ID", Util.GetValueOfInt(rowsGL[j]["GL_JournalLine_ID"]));
                            aLine.SetC_Invoice_ID(Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]));

                            //set the trx Date and InvoicePayschedule_ID
                            msg = InvAlloc(C_InvoicePaySchedule_ID, mpay2, aLine, DateTrx, trx);
                            if (msg != string.Empty)
                            {
                                //set Isprocessing false
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }
                            else
                            {
                                if (Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]) < 0)
                                {
                                    neg_Invoice_IDs.Add(Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]));
                                }
                                paid = (Util.GetValueOfDecimal(rowsGL[j]["paidAmt"]) + netAmt);
                                rowsGL[j]["paidAmt"] = paid.ToString();
                                if (Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]) > 0)
                                {
                                    paid = Util.GetValueOfDecimal(rowsInvoice[i]["paidAmt"]) + netAmt;
                                    rowsInvoice[i]["paidAmt"] = paid.ToString();
                                }
                                else
                                {
                                    for (int k = 0; k < negList.Count; k++)
                                    {
                                        if (Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]) == Util.GetValueOfInt(negList[k]["cinvoiceid"]))
                                        {
                                            paid = Util.GetValueOfDecimal(negList[k]["paidAmt"]) + netAmt;
                                            negList[k]["paidAmt"] = paid.ToString();
                                        }
                                    }
                                }
                                //dueamt = Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt));
                                if (balanceAmt == 0)
                                {
                                    rowsGL[j]["IsPaid"] = true.ToString();
                                }
                                if (remainingAmt == 0)
                                {
                                    rowsInvoice[i]["IsPaid"] = true.ToString();
                                    break;
                                }
                            }

                            //  Apply Discounts and WriteOff only first time
                            DiscountAmt = Env.ZERO;
                            WriteOffAmt = Env.ZERO;
                            overUnderAmt = Env.ZERO;
                        }


                        //MInvoice neg_InvoiceObj = null;
                        //only when the invoice is have + ve AppliedAmt
                        if (remainingAmt > 0 && Util.GetValueOfDecimal(rowsInvoice[i]["AppliedAmt"]) > 0)
                        {
                            Decimal NOverUnderAmt = Env.ZERO;
                            for (int j = 0; j < negList.Count; j++)


                            {

                                mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]), trx);
                                invoice = new MInvoice(ctx, Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]), trx);
                                Decimal NDiscountAmt = Util.GetValueOfDecimal(negList[j][discount]);
                                Decimal NWriteOffAmt = Util.GetValueOfDecimal(negList[j][writeOff]);
                                MInvoice neg_InvoiceObj = new MInvoice(ctx, Util.GetValueOfInt(negList[j]["cinvoiceid"]), trx);

                                if (Util.GetValueOfBool(negList[j]["IsPaid"]))
                                    continue;

                                actualAmt = Math.Abs(Util.GetValueOfDecimal(negList[j]["AppliedAmt"])) - Math.Abs(Util.GetValueOfDecimal(negList[j]["paidAmt"]));

                                //if amount is negetive than * by - 1 to convert it into positive.
                                if (overUnderAmt < 0)
                                    overUnderAmt = -1 * overUnderAmt;

                                if (remainingAmt >= actualAmt)
                                {
                                    remainingAmt -= actualAmt;
                                    netAmt = actualAmt;
                                    balanceAmt = 0;
                                }
                                else
                                {
                                    netAmt = remainingAmt;
                                    balanceAmt = actualAmt - remainingAmt;
                                    remainingAmt = 0;
                                }

                                //InvoicePaySchedule Update/Create
                                if (!isScheduleAllocated)
                                {
                                    isScheduleAllocated = true;
                                    if (invoice.GetC_Currency_ID() != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, invoice.GetC_Currency_ID(),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : neg_InvoiceObj.GetDateAcct()), neg_InvoiceObj.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                        if (remainingAmt == Env.ZERO)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(invoice, trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt)),
                                                       Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))));

                                    if (!mpay.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set Isprocessing false
                                        Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                        return msg;
                                    }
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
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt)), Decimal.Add(Math.Abs(DiscountAmt), Math.Abs(WriteOffAmt))), C_Currency_ID, invoice.GetC_Currency_ID(),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : neg_InvoiceObj.GetDateAcct()), neg_InvoiceObj.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                                        if (remainingAmt == Env.ZERO)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(invoice, trx);
                                            mpay2.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                        else
                                        {
                                            mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(netAmt));

                                    if (!mpay2.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set Isprocessing false
                                        Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                        return msg;
                                    }
                                }

                                C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                                int Ref_Invoice_ID = Util.GetValueOfInt(negList[j]["cinvoiceid"]);
                                if (C_InvoicePaySchedule_ID == Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]))
                                {
                                    overUnderAmt = 0;
                                }

                                C_InvoicePaySchedule_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);

                                aLine = new MAllocationLine(alloc, netAmt, DiscountAmt, WriteOffAmt, overUnderAmt);
                                aLine.SetDocInfo(C_BPartner_ID, 0, C_Invoice_ID);
                                aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);

                                //get InvoiceSchedule_ID and Initalize to positiveAmtInvSchdle_ID
                                int positiveAmtInvSchdle_ID = 0;
                                if (mpay2 != null)
                                {
                                    positiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                }
                                else
                                {
                                    positiveAmtInvSchdle_ID = Util.GetValueOfInt(rowsInvoice[i]["c_invoicepayschedule_id"]);
                                }

                                //set the trx Date and InvoicePayschedule_ID
                                msg = InvAlloc(C_InvoicePaySchedule_ID, mpay2, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    //set Isprocessing false
                                    Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                    return msg;
                                }

                                else
                                {
                                    paid = (Util.GetValueOfDecimal(negList[j]["paidAmt"]) + netAmt);
                                    negList[j]["paidAmt"] = paid.ToString();
                                    rowsInvoice[i]["paidAmt"] = (Util.GetValueOfDecimal(rowsInvoice[i]["paidAmt"]) + netAmt).ToString();
                                    //dueamt = Decimal.Add(Math.Abs(netAmt), Math.Abs(overUnderAmt));
                                }
                                //Get AllocationLine_ID and Inintilaze to aLine_ID
                                int aLine_ID = aLine.GetC_AllocationLine_ID();

                                // when 
                                mpay = new MInvoicePaySchedule(ctx, Util.GetValueOfInt(negList[j]["c_invoicepayschedule_id"]), trx);
                                mpay2 = null;
                                //if the invoice id for -ve amount will contain in this list the overunderamt set as Zero.
                                if (!neg_Invoice_IDs.Contains(Util.GetValueOfInt(negList[j]["c_invoicepayschedule_id"])))
                                {
                                    //// Updated over/under amount on allocation line, it should be open -( applied + discount + writeoff ) Update by vivek on 05/01/2018 issue reported by Savita
                                    NOverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(negList[j][open]),
                                    Decimal.Add(Util.GetValueOfDecimal(negList[j][applied]), Decimal.Add(NDiscountAmt, NWriteOffAmt)));
                                    neg_Invoice_IDs.Add(Util.GetValueOfInt(negList[j]["c_invoicepayschedule_id"]));
                                    is_NegScheduleAllocated = false;
                                }
                                else
                                {
                                    NOverUnderAmt = Env.ZERO;
                                    is_NegScheduleAllocated = true;
                                }

                                ////InvoicePaySchedule Update/Create
                                //msg = ScheduleUpdateOrCreate(is_NegScheduleAllocated, mpay, mpay2, netAmt, rowsPayment, rowsInvoice, rowsCash, rowsGL, neg_InvoiceObj, invoice, null, null, null, null,
                                //    NOverUnderAmt, NDiscountAmt, NWriteOffAmt, C_Currency_ID, ctx, trx);
                                //if (msg != String.Empty)
                                //{
                                //    return msg;
                                //}
                                //InvoicePaySchedule Update/Create
                                if (!is_NegScheduleAllocated)
                                {
                                    is_NegScheduleAllocated = true;
                                    if (neg_InvoiceObj.GetC_Currency_ID() != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(NOverUnderAmt)), Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))), C_Currency_ID, neg_InvoiceObj.GetC_Currency_ID(),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : invoice.GetDateAcct()), invoice.GetC_ConversionType_ID(), neg_InvoiceObj.GetAD_Client_ID(), neg_InvoiceObj.GetAD_Org_ID());
                                        if (balanceAmt == Env.ZERO)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(neg_InvoiceObj, trx);
                                            if (diffAmt != Env.ZERO)
                                            {
                                                mpay.SetDueAmt(Math.Abs(diffAmt));
                                            }
                                        }
                                        else
                                        {
                                            mpay.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay.SetDueAmt(Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(NOverUnderAmt)),
                                                       Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))));

                                    if (!mpay.Save(trx))
                                    {
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set Isprocessing false
                                        Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                        return msg;
                                    }
                                }
                                // Create New schedule with split 
                                else if (is_NegScheduleAllocated)
                                {
                                    mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
                                    PO.CopyValues(mpay, mpay2);
                                    //Set AD_Org_ID and AD_Client_ID when we split the schedule
                                    mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
                                    mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
                                    if (neg_InvoiceObj.GetC_Currency_ID() != C_Currency_ID)
                                    {
                                        var conertedAmount = MConversionRate.Convert(ctx, Decimal.Add(Decimal.Add(Math.Abs(netAmt), Math.Abs(NOverUnderAmt)), Decimal.Add(Math.Abs(NDiscountAmt), Math.Abs(NWriteOffAmt))), C_Currency_ID, neg_InvoiceObj.GetC_Currency_ID(),
                                            (alloc.GetConversionDate() != null ? alloc.GetConversionDate() : invoice.GetDateAcct()), invoice.GetC_ConversionType_ID(), neg_InvoiceObj.GetAD_Client_ID(), neg_InvoiceObj.GetAD_Org_ID());
                                        if (balanceAmt == Env.ZERO)
                                        {
                                            //get the difference DueAmt by Compare with Total Invoice Amount with sum of Schedule DueAmt's
                                            diffAmt = GetDifference(neg_InvoiceObj, trx);
                                            mpay2.SetDueAmt(Math.Abs(diffAmt));
                                        }
                                        else
                                        {
                                            mpay2.SetDueAmt(Math.Abs(conertedAmount));
                                        }
                                    }
                                    else
                                        mpay2.SetDueAmt(Math.Abs(netAmt));

                                    if (!mpay2.Save(trx))
                                    {
                                        //return error Message
                                        msg = ValidateSaveInvoicePaySchedule(trx);
                                        //set Isprocessing false
                                        Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                        return msg;
                                    }
                                }

                                Neg_C_InvoicePaySchedule_Id = Util.GetValueOfInt(negList[j]["c_invoicepayschedule_id"]);
                                C_Invoice_ID = Util.GetValueOfInt(negList[j]["cinvoiceid"]);
                                //  Invoice variables
                                Ref_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                                //allocation for negative Amount Invoice
                                aLine = new MAllocationLine(alloc, Decimal.Negate(netAmt), NDiscountAmt, NWriteOffAmt, Decimal.Negate(NOverUnderAmt));
                                aLine.SetDocInfo(C_BPartner_ID, 0, C_Invoice_ID);
                                aLine.SetRef_C_Invoice_ID(Ref_Invoice_ID);
                                aLine.SetRef_Invoiceschedule_ID(positiveAmtInvSchdle_ID);
                                //get the C_InvoicePaySchedule_ID and Initialize to negtiveAmtInvSchdle_ID
                                int negtiveAmtInvSchdle_ID = 0;
                                if (mpay2 != null)
                                {
                                    negtiveAmtInvSchdle_ID = mpay2.GetC_InvoicePaySchedule_ID();
                                }
                                else
                                {
                                    negtiveAmtInvSchdle_ID = Util.GetValueOfInt(negList[j]["c_invoicepayschedule_id"]);
                                }
                                //set the trx Date and InvoicePayschedule_ID
                                msg = InvAlloc(Neg_C_InvoicePaySchedule_Id, mpay2, aLine, DateTrx, trx);
                                if (msg != string.Empty)
                                {
                                    //set Isprocessing false
                                    Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                    return msg;
                                }

                                //Updating +ve Invoice allocationLine to set Ref_InvoicePaySchedule_ID
                                aLine = new MAllocationLine(ctx, aLine_ID, trx);
                                aLine.SetRef_Invoiceschedule_ID(negtiveAmtInvSchdle_ID);
                                if (!aLine.Save())
                                {
                                    _log.SaveError("Error: ", "Allocation line Ref_InvoicePaySchedule_ID not updated!");
                                    trx.Rollback();
                                    trx.Close();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    if (pp != null)
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated") + ":- " + pp.GetName();
                                    }
                                    else
                                    {
                                        msg = Msg.GetMsg(ctx, "VIS_InvPaySchedle_IDNotUpdated");
                                    }
                                    //set Isprocessing false
                                    Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                    return msg;
                                }

                                if (balanceAmt == 0)
                                {
                                    //set the paid Amount to the Invoice
                                    for (int a = 0; a < rowsInvoice.Count; a++)
                                    {
                                        if (Util.GetValueOfInt(rowsInvoice[a]["cinvoiceid"]) == Util.GetValueOfInt(negList[j]["cinvoiceid"]))
                                        {
                                            rowsInvoice[a]["paidAmt"] = negList[j]["paidAmt"];
                                            rowsInvoice[a]["IsPaid"] = true.ToString();
                                        }
                                    }
                                    negList[j]["IsPaid"] = true.ToString();
                                }
                                if (remainingAmt == 0)
                                {
                                    //set the isPaid as true and Exit from the loop
                                    rowsInvoice[i]["IsPaid"] = true.ToString();
                                    break;
                                }
                                //  Apply Discounts and WriteOff only first time
                                DiscountAmt = Env.ZERO;
                                WriteOffAmt = Env.ZERO;
                                overUnderAmt = Env.ZERO;
                            }
                        }
                    }
                }
                #endregion

                if (rowsCash.Count == 0 && rowsPayment.Count == 0 && rowsInvoice.Count == 0)
                {
                    trx.Rollback();
                    trx.Close();
                    return Msg.GetMsg(ctx, "GLtoGLAllocationnotpossible");
                }

                if (alloc.Get_ID() != 0)
                {
                    CompleteOrReverse(ctx, alloc.Get_ID(), 150, DocActionVariables.ACTION_COMPLETE, trx);
                    //alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                    if (alloc.Save())
                    {
                        msg = alloc.GetDocumentNo();
                    }
                    else
                    {
                        _log.SaveError("Error: ", "Allocation not completed");
                        trx.Rollback();
                        trx.Close();
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            msg = Msg.GetMsg(ctx, "VIS_AllocationHdrNotSaved") + ":- " + pp.GetName();
                        }
                        else
                        {
                            msg = Msg.GetMsg(ctx, "VIS_AllocationHdrNotSaved");
                        }
                        //set Isprocessing false
                        Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                        return msg;
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
                    if (Env.Signum(opens) == 0)
                    {
                        sql = "UPDATE C_Invoice SET IsPaid='Y' "
                            + "WHERE C_Invoice_ID=" + C_Invoice_ID;
                        int no = DB.ExecuteQuery(sql, null, trx);
                    }
                }
                #endregion

                //  Test/Set Payment is fully allocated
                #region Set Payment Allocated
                if (rowsPayment.Count > 0)
                {
                    for (int i = 0; i < rowsPayment.Count; i++)
                    {
                        int C_Payment_ID = Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]);
                        MPayment pay = new MPayment(ctx, C_Payment_ID, trx);
                        if (pay.TestAllocation())
                        {
                            if (!pay.Save())
                            {
                                trx.Rollback();
                                trx.Close();
                                _log.SaveError("Error: ", "Payment not allocated");
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    msg = Msg.GetMsg(ctx, "PaymentNotCreated") + ":- " + pp.GetName();
                                }
                                else
                                {
                                    msg = Msg.GetMsg(ctx, "PaymentNotCreated");
                                }
                                //set isprocessing false
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }

                        }

                        string sqlGetOpenPayments = "SELECT  NVL(currencyConvert(ALLOCPAYMENTAVAILABLE(C_Payment_ID) ,p.C_Currency_ID ," + C_Currency_ID + ",p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID),0) as amt FROM C_Payment p Where C_Payment_ID = " + C_Payment_ID;
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
                        if (!pay.Save())
                        {
                            trx.Rollback();
                            trx.Close();
                            _log.SaveError("Error: ", "Payment not allocated");
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                msg = Msg.GetMsg(ctx, "PaymentNotCreated") + ":- " + pp.GetName();
                            }
                            else
                            {
                                msg = Msg.GetMsg(ctx, "PaymentNotCreated");
                            }
                            //set isprocessing false
                            Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                            return msg;
                        }
                    }
                }
                #endregion

                // CashLine set IsAllocated 
                #region Set CashLine Allocated
                if (rowsCash.Count > 0)
                {
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
                        if (!cash.Save())
                        {
                            trx.Rollback();
                            trx.Close();
                            _log.SaveError("Error: ", "Cash Line not allocated");
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null)
                            {
                                msg = Msg.GetMsg(ctx, "VIS_CashLineNotUpdate") + ":- " + pp.GetName();
                            }
                            else
                            {
                                msg = Msg.GetMsg(ctx, "VIS_CashLineNotUpdate");
                            }
                            //Set Isprocessing false
                            Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                            return msg;
                        }
                    }
                }
                #endregion

                //set gl line allocated
                #region Set glLine Allocated
                if (rowsGL.Count > 0)
                {
                    int chk = 0;
                    for (int i = 0; i < rowsGL.Count; i++)
                    {
                        //int _GL_JournalLine_ID = Util.GetValueOfInt(rowsGL[i]["GL_JournalLine_ID"]);
                        //string sqlGetOpenGlAmt = @"SELECT (ABS(NVL(SUM(ROUND(CURRENCYCONVERT(AL.AMOUNT ,AR.C_CURRENCY_ID ," + C_Currency_ID + @",AR.DATEACCT ,AR.C_CONVERSIONTYPE_ID ,
                        //                    AR.AD_CLIENT_ID ,AR.AD_ORG_ID ), 2)),0)) - ABS(SUM(NVL(ROUND(CURRENCYCONVERT(JL.AMTSOURCEDR ,JL.C_CURRENCY_ID ,
                        //                    " + C_Currency_ID + @",J.DATEACCT ,J.C_CONVERSIONTYPE_ID ,J.AD_CLIENT_ID ,J.AD_ORG_ID ), 2),0))) - ABS(SUM(NVL(ROUND(currencyConvert
                        //                    (JL.AMTSOURCECR ,jl.C_Currency_ID ," + C_Currency_ID + @",j.DATEACCT ,j.C_ConversionType_ID ,j.AD_Client_ID ,j.AD_Org_ID ), 2),0)))) 
                        //                    AS balanceamt FROM C_ALLOCATIONLINE AL INNER JOIN C_ALLOCATIONHDR AR ON ar.C_AllocationHdr_ID = al.C_AllocationHdr_ID
                        //                    INNER JOIN GL_JOURNALLINE jl ON jl.GL_JOURNALLINE_ID = al.GL_JOURNALLINE_ID INNER JOIN GL_JOURNAL j ON j.GL_JOURNAL_ID 
                        //                    = jl.GL_JOURNAL_ID WHERE al.GL_JOURNALLINE_ID = " + _GL_JournalLine_ID + @" AND AR.DOCSTATUS IN('CO', 'CL') ";
                        //decimal result = Util.GetValueOfDecimal(DB.ExecuteScalar(sqlGetOpenGlAmt, null, trx));
                        if (Util.GetValueOfBool(rowsGL[i]["IsPaid"]) == true)
                        {
                            chk = DB.ExecuteQuery(@" UPDATE GL_JOURNALLINE SET isAllocated ='Y' WHERE GL_JOURNALLINE_ID =" + Util.GetValueOfInt(rowsGL[i]["GL_JournalLine_ID"]), null, trx);
                            if (chk < 0)
                            {
                                trx.Rollback();
                                trx.Close();
                                _log.SaveError("Error: ", "Journal Line not allocated");
                                msg = Msg.GetMsg(ctx, "VIS_GLLineNotAllocated");
                                //set Isprocessing false
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }
                        }
                    }
                }
                #endregion

                //set gl id on invoice schedule and Payment
                #region set gl journal id on invoice pay schedule
                if (rowsInvoice.Count > 0 && rowsGL.Count > 0)
                {
                    string sql = @"SELECT al.c_invoicepayschedule_id, al.gl_journalline_id FROM c_allocationline al WHERE 
                                 al.C_AllocationHdr_ID IN (" + alloc.GetC_AllocationHdr_ID() + ") AND al.gl_journalline_id IS NOT NULL ";
                    DataSet ds = DB.ExecuteDataset(sql, null, trx);
                    int chk = 0;
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            chk = DB.ExecuteQuery(@" UPDATE c_invoicepayschedule SET gl_journalline_id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["gl_journalline_id"]) + ""
                                         + " WHERE c_invoicepayschedule_id = " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["c_invoicepayschedule_id"]), null, trx);
                            if (chk < 0)
                            {
                                trx.Rollback();
                                trx.Close();
                                _log.SaveError("Error: ", "Journal ID not Updated on Invoice Schedule");
                                ValueNamePair pp = VLogger.RetrieveError();
                                if (pp != null)
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_ScheduleNotAllocated") + ":- " + pp.GetName();
                                }
                                else
                                {
                                    msg = Msg.GetMsg(ctx, "VIS_ScheduleNotAllocated");
                                }
                                //set Isprocessing false
                                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                                return msg;
                            }
                        }
                    }
                }
                #endregion
                //set Isprocessing false
                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
            }
            else
            {
                //Get Error Message.
                msg = AllocationHdrFaildToSave(trx);
                //set Isprocessing false
                Isprocess(rowsPayment, rowsCash, rowsInvoice, rowsGL, trx);
                return msg;
            }
            trx.Commit();
            trx.Close();
            return Msg.GetMsg(ctx, "AllocationCreatedWith") + msg;
        }

        /// <summary>
        /// TO create new schedule 
        /// </summary>
        /// <param name="mpay">Invoice Pay Schedule object</param>
        /// <param name="invoice">Invoice Header Object</param>
        /// <param name="journalLine">Journal Line Object</param>
        /// <param name="aLine">Allocation Line Object</param>
        /// <param name="amount">Amount to create schedule</param>
        /// <param name="trx">Transaction Object</param>
        /// <returns>Return new schedule object</returns>
        public MInvoicePaySchedule CreateNewSchedule(MInvoicePaySchedule mpay, MInvoice invoice, MJournalLine journalLine, MAllocationLine aLine, Decimal amount, Trx trx)
        {
            MInvoicePaySchedule mpay2 = new MInvoicePaySchedule(ctx, 0, trx);
            MJournal journal = new MJournal(ctx, journalLine.GetGL_Journal_ID(), trx);
            PO.CopyValues(mpay, mpay2);
            //Set AD_Org_ID and AD_Client_ID when we split the schedule
            mpay2.SetAD_Client_ID(mpay.GetAD_Client_ID());
            mpay2.SetAD_Org_ID(mpay.GetAD_Org_ID());
            mpay2.SetC_CashLine_ID(0);
            mpay2.SetC_Payment_ID(0);
            mpay2.Set_Value("GL_JournalLine_ID", 0);
            if (invoice.GetC_Currency_ID() != journalLine.GetC_Currency_ID())
            {
                var conertedAmount = MConversionRate.Convert(ctx, amount, journalLine.GetC_Currency_ID(), invoice.GetC_Currency_ID(), journal.GetDateAcct(), journalLine.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                mpay2.SetDueAmt(Math.Abs(conertedAmount));
            }
            else
                mpay2.SetDueAmt(Math.Abs(amount));
            //mpay2.SetVA009_OpenAmnt(Math.Abs(amount));
            //mpay2.SetVA009_OpnAmntInvce(Math.Abs(amount));
            if (!mpay2.Save(trx))
            {
                _log.SaveError("Error: ", "Due amount not set on invoice schedule");
                trx.Rollback();
                trx.Close();
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    Msg.GetMsg(ctx, "Error: " + pp.GetName());
                }
                else
                {
                    Msg.GetMsg(ctx, "VIS_ScheduleNotUpdate");
                }
            }
            aLine.SetC_InvoicePaySchedule_ID(mpay2.GetC_InvoicePaySchedule_ID());
            if (!aLine.Save(trx))
            {
                _log.SaveError("Error: ", "invoice schedule Not updated on allocation line");
                trx.Rollback();
                trx.Close();
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    Msg.GetMsg(ctx, "Error: " + pp.GetName());
                }
                else
                {
                    Msg.GetMsg(ctx, "VIS_AllocationNotUpdate");
                }
            }
            return mpay2;
        }


        /// <summary>
        /// Mehtod added to complete and reverse the document and execute the workflow as well.
        /// </summary>
        /// <param name="ctx">Current context.</param>
        /// <param name="Record_ID">Record id for which the workflow to be processed.</param>
        /// <param name="Process_ID">Process id needed to be processed.</param>
        /// <param name="DocAction">Document Action</param>
        /// <returns>Returns the result of completion or reversal in a string array.</returns>
        private string[] CompleteOrReverse(Ctx ctx, int Record_ID, int Process_ID, string DocAction, Trx trx)
        {
            string[] result = new string[2];
            MRole role = MRole.Get(ctx, ctx.GetAD_Role_ID());
            if (Util.GetValueOfBool(role.GetProcessAccess(Process_ID)))
            {
                if (Process_ID == 150)
                {
                    if (DB.ExecuteQuery("UPDATE C_AllocationHdr SET DocAction = '" + DocAction + "' WHERE C_AllocationHdr_ID = " + Record_ID, null, trx) < 0)
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
                            errorMsg = Msg.GetMsg(ctx, "VA028_DocNotCompleted");
                        result[0] = errorMsg;
                        result[1] = "N";
                        trx.Rollback();
                        return result;
                    }
                }
                trx.Commit();
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
                        errorMsg = Msg.GetMsg(ctx, "VA028_DocNotCompleted");
                    result[0] = errorMsg;
                    result[1] = "N";
                    return result;
                }

                //MPInstancePara para = new MPInstancePara(pin, 20);
                //para.setParameter("DocAction", DocAction);
                //if (!para.Save())
                //{
                //    //String msg = "No DocAction Parameter added";  //  not translated
                //}

                VAdvantage.ProcessEngine.ProcessInfo pi = new VAdvantage.ProcessEngine.ProcessInfo("WF", Process_ID);
                pi.SetAD_User_ID(ctx.GetAD_User_ID());
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                pi.SetAD_PInstance_ID(pin.GetAD_PInstance_ID());
                pi.SetRecord_ID(Record_ID);
                pi.SetTable_ID(Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE Export_ID ='VIS_735'")));
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
                        errorMsg = Msg.GetMsg(ctx, "VA028_DocNotCompleted");
                    result[0] = errorMsg;
                    result[1] = "N";
                    return result;
                }
                else
                    Msg.GetMsg(ctx, "VA028_CompSuccess");

                result[0] = "";
                result[1] = "Y";
            }
            else
            {
                result[0] = Msg.GetMsg(ctx, "VA028_NoAccess");
                return result;
            }
            return result;
        }

        #region Properties 
        public class NameValue
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        public class VIS_DocbaseType
        {
            public string DocbaseType { get; set; }
            public string Name { get; set; }
        }
        public class VIS_DocType
        {
            public string DocType { get; set; }
            public int C_DocType_ID { get; set; }
        }
        public class VIS_PayType
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public class VIS_InvoiceData
        {
            public string SelectRow { get; set; }
            public DateTime? Date1 { get; set; }
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
            public DateTime? InvoiceScheduleDate { get; set; }
            public int InvoiceRecord { get; set; }
            public int C_ConversionType_ID { get; set; }
            public string ConversionName { get; set; }
            public DateTime? DATEACCT { get; set; }
            public int AD_Org_ID { get; set; }
            public string OrgName { get; set; }
            public string PayName { get; set; }
        }

        public class VIS_PaymentData
        {
            public string SelectRow { get; set; }
            public DateTime? Date1 { get; set; }
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
            public string DocBaseType { get; set; }
            public string PayName { get; set; }
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
            public string Payment { get; set; }
            public string VSS_paymenttype { get; internal set; }
        }

        public class GLData
        {
            public string SelectRow { get; set; }
            public string DOCUMENTNO { get; set; }
            public decimal? AMTSOURCEDR { get; set; }
            public decimal? AMTSOURCECR { get; set; }
            public decimal? AMTACCTDR { get; set; }
            public decimal? AmtAcctCr { get; set; }
            public decimal? AppliedAmt { get; set; }
            public decimal? ConvertedAmount { get; set; }
            public decimal? OpenAmount { get; set; }
            public int GL_JOURNALLINE_ID { get; set; }
            public int GL_Journal_ID { get; set; }
            public int C_ConversionType_ID { get; set; }
            public string ConversionName { get; set; }
            public DateTime? DATEACCT { get; set; }
            public DateTime? DATEDOC { get; set; }
            public int GLRecords { get; set; }
            public int C_BPartner_ID { get; set; }
            public bool isCustomer { get; set; }
            public bool isVendor { get; set; }
            public string Isocode { get; internal set; }
            public int AD_Org_ID { get; set; }
            public string OrgName { get; set; }
            public string Account { get; set; }
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

        #endregion

    }
}