/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_PaymentTerm
 * Chronological Development
 * Veena Pandey     22-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPaymentTerm : X_C_PaymentTerm
    {
        /** 100									*/
        private const Decimal HUNDRED = 100.0M;

        /**	Payment Schedule children			*/
        private MPaySchedule[] _schedule;

        /**
	     * 	Standard Constructor
	     *	@param ctx context
	     *	@param C_PaymentTerm_ID id
	     *	@param trxName transaction
	     */
        public MPaymentTerm(Ctx ctx, int C_PaymentTerm_ID, Trx trxName)
            : base(ctx, C_PaymentTerm_ID, trxName)
        {
            if (C_PaymentTerm_ID == 0)
            {
                SetAfterDelivery(false);
                SetNetDays(0);
                SetDiscount(Env.ZERO);
                SetDiscount2(Env.ZERO);
                SetDiscountDays(0);
                SetDiscountDays2(0);
                SetGraceDays(0);
                SetIsDueFixed(false);
                SetIsValid(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MPaymentTerm(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Payment Schedule
        /// </summary>
        /// <param name="requery">requery if true re-query</param>
        /// <returns>array of schedule</returns>
        public MPaySchedule[] GetSchedule(bool requery)
        {
            if (_schedule != null && !requery)
                return _schedule;
            String sql = null;
            // when Payment Management module available then get advance record first than other
            if (Env.IsModuleInstalled("VA009_"))
            {
                sql = "SELECT * FROM C_PaySchedule WHERE IsActive = 'Y' AND C_PaymentTerm_ID=" + GetC_PaymentTerm_ID() + " ORDER BY COALESCE( VA009_Advance , 'N') DESC, NetDays";
            }
            else
            {
                sql = "SELECT * FROM C_PaySchedule WHERE IsActive = 'Y' AND C_PaymentTerm_ID=" + GetC_PaymentTerm_ID() + " ORDER BY NetDays";
            }
            List<MPaySchedule> list = new List<MPaySchedule>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_Trx());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MPaySchedule ps = new MPaySchedule(GetCtx(), dr, Get_Trx());
                        ps.SetParent(this);
                        list.Add(ps);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "GetSchedule", e);
            }

            _schedule = new MPaySchedule[list.Count];
            _schedule = list.ToArray();
            return _schedule;
        }

        /**
         * 	Validate Payment Term & Schedule
         *	@return Validation Message @OK@ or error
         */
        public String Validate()
        {
            GetSchedule(true);
            if (_schedule.Length == 0)
            {
                SetIsValid(true);
                return "@OK@";
            }
            if (_schedule.Length == 1)
            {
                SetIsValid(false);
                return "@Invalid@ @Count@ # = 1 (@C_PaySchedule_ID@)";
            }

            //	Add up
            Decimal total = Env.ZERO;
            for (int i = 0; i < _schedule.Length; i++)
            {
                Decimal percent = _schedule[i].GetPercentage();
                // if (percent != null)
                total = Decimal.Add(total, percent);
            }
            bool valid = total.CompareTo(HUNDRED) == 0;
            SetIsValid(valid);
            for (int i = 0; i < _schedule.Length; i++)
            {
                if (_schedule[i].IsValid() != valid)
                {
                    _schedule[i].SetIsValid(valid);
                    _schedule[i].Save();
                }
            }
            String msg = "@OK@";
            if (!valid)
                msg = "@Total@ = " + total + " - @Difference@ = " + Decimal.Subtract(HUNDRED, total);
            return Msg.ParseTranslation(GetCtx(), msg);
        }


        /*************************************************************************
         * 	Apply Payment Term to Invoice -
         *	@param C_Invoice_ID invoice
         *	@return true if payment schedule is valid
         */
        public bool Apply(int C_Invoice_ID)
        {
            MInvoice invoice = new MInvoice(GetCtx(), C_Invoice_ID, Get_Trx());
            if (invoice == null || invoice.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "apply - Not valid C_Invoice_ID=" + C_Invoice_ID);
                return false;
            }
            return Apply(invoice);
        }

        /// <summary>
        /// Apply Payment Term to Invoice
        /// </summary>
        /// <param name="invoice">invoice reference</param>
        /// <returns>true if payment schedule is valid</returns>
        public bool Apply(MInvoice invoice)
        {
            if (invoice == null || invoice.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "No valid invoice - " + invoice);
                return false;
            }

            GetSchedule(true);

            if (invoice.GetC_Order_ID() != 0)
            {
                return ApplyNoSchedule(invoice);
            }
            else
            {
                return ApplySchedule(invoice);
            }
        }

        /// <summary>
        /// Apply Payment Term without schedule to Invoice
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <returns>bool, true-false</returns>
        private bool ApplyNoSchedule(MInvoice invoice)
        {
            UpdateOrderSchedule(invoice.GetRef_C_Invoice_ID() > 0 ? invoice.GetRef_C_Invoice_ID() : invoice.GetC_Invoice_ID(), invoice.Get_Trx());
            DeleteInvoicePaySchedule(invoice.GetC_Invoice_ID(), invoice.Get_Trx());
            StringBuilder _sql = new StringBuilder();
            MInvoicePaySchedule schedule = null;
            MPaymentTerm payterm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), invoice.Get_Trx());
            MPaySchedule sched = new MPaySchedule(GetCtx(), invoice.GetC_PaymentTerm_ID(), Get_Trx());

            Decimal remainder = invoice.GetGrandTotal();

            if (Env.IsModuleInstalled("VA009_"))
            {
                // check if payment term is advance then copy order schedules
                if (payterm.IsVA009_Advance())
                {
                    // is used to get record from invoice line which is created against orderline
                    String sql = @"SELECT o.C_Order_ID , NVL(SUM(il.LineTotalAmt) , 0) AS LineTotalAmt 
                                    FROM C_Invoice i INNER JOIN C_Invoiceline il ON i.C_Invoice_ID = il.C_Invoice_ID
                                    INNER JOIN C_Orderline ol ON ol.C_Orderline_ID = il.C_Orderline_ID
                                    INNER JOIN C_Order o ON o.C_Order_ID = ol.C_Order_ID
                                    WHERE i.C_Invoice_ID = " + invoice.GetC_Invoice_ID() + " GROUP BY o.C_Order_ID ";
                    DataSet _dsInvoice = DB.ExecuteDataset(sql, null, Get_Trx());
                    if (_dsInvoice != null && _dsInvoice.Tables.Count > 0 && _dsInvoice.Tables[0].Rows.Count > 0)
                    {
                        int order_Id = 0; // order reference
                        Decimal LineTotalAmt = 0; // amount against order
                        DataSet _dsOrderSchedule = null; // order schedule dataset
                        DataRow dr = null;

                        for (int i = 0; i < _dsInvoice.Tables[0].Rows.Count; i++)
                        {
                            order_Id = Convert.ToInt32(_dsInvoice.Tables[0].Rows[i]["C_Order_ID"]);
                            // JID_1379 : Type cast Issue
                            LineTotalAmt = Math.Abs(Util.GetValueOfDecimal(_dsInvoice.Tables[0].Rows[i]["LineTotalAmt"]));

                            // is used to get order schedule detail which is paid but not fully allocated
                            sql = "SELECT * FROM VA009_OrderPaySchedule WHERE C_Order_ID=" + order_Id + " AND VA009_IsPaid='Y' AND NVL(VA009_AllocatedAmt,0) != DueAmt ORDER BY Created";
                            _dsOrderSchedule = DB.ExecuteDataset(sql, null, invoice.Get_Trx());

                            if (_dsOrderSchedule != null && _dsOrderSchedule.Tables.Count > 0 && _dsOrderSchedule.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < _dsOrderSchedule.Tables[0].Rows.Count; j++)
                                {
                                    // data row - order schedule record
                                    dr = _dsOrderSchedule.Tables[0].Rows[j];

                                    schedule = new MInvoicePaySchedule(GetCtx(), 0, invoice.Get_Trx());
                                    copyorderschedules(schedule, invoice, dr, LineTotalAmt, 100);

                                    if (!schedule.Save(invoice.Get_Trx()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "VIS_ScheduleNotSave") +
                                            (pp != null && !string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : ""));

                                        return false;
                                    }
                                    else
                                    {
                                        // Ref_C_Invoice_ID > 0 its means reverse record, then subtract due amt else add due amt
                                        if (invoice.GetRef_C_Invoice_ID() <= 0)
                                        {
                                            DB.ExecuteQuery("UPDATE VA009_OrderPaySchedule SET VA009_AllocatedAmt = NVL(VA009_AllocatedAmt, 0) + "
                                                + Util.GetValueOfDecimal(schedule.GetDueAmt()) +
                                                @" WHERE VA009_OrderPaySchedule_ID = " + Convert.ToInt32(dr["VA009_OrderPaySchedule_ID"]), null, invoice.Get_Trx());
                                        }
                                    }
                                    remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
                                    LineTotalAmt = Decimal.Subtract(LineTotalAmt, Math.Abs(schedule.GetDueAmt()));
                                    if (LineTotalAmt == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // when remainder > 0 the need to create new invoice schedule  
                    if (remainder.CompareTo(Env.ZERO) != 0 && schedule != null)
                    {
                        //schedule.SetDueAmt(Decimal.Add(schedule.GetDueAmt(), remainder));
                        //schedule.Save(invoice.Get_Trx());

                        MInvoicePaySchedule newSchedule = new MInvoicePaySchedule(GetCtx(), 0, invoice.Get_Trx());
                        PO.CopyValues(schedule, newSchedule, schedule.GetAD_Client_ID(), schedule.GetAD_Org_ID());

                        // set references
                        newSchedule.SetVA009_OrderPaySchedule_ID(0);
                        newSchedule.SetC_Payment_ID(0);
                        // Paid Amount (Base)
                        newSchedule.SetVA009_PaidAmnt(0);
                        // Paid Amount (Invoice)
                        newSchedule.SetVA009_PaidAmntInvce(0);
                        // Variance
                        newSchedule.SetVA009_Variance(0);
                        // set discount
                        newSchedule.SetDiscountAmt(0);
                        newSchedule.SetDiscount2(0);
                        // set payment method from invoice header
                        newSchedule.SetVA009_PaymentMethod_ID(invoice.GetVA009_PaymentMethod_ID());
                        // set Payment execution
                        newSchedule.SetVA009_ExecutionStatus("A");
                        // payment not paid yet
                        newSchedule.SetVA009_IsPaid(false);

                        Decimal baseCurencyAmt = 0;
                        if (invoice.GetC_Currency_ID() != GetCtx().GetContextAsInt("$C_Currency_ID")) //(invoice currency != base Currency)
                        {
                            baseCurencyAmt = MConversionRate.Convert(GetCtx(), remainder, invoice.GetC_Currency_ID(),
                                GetCtx().GetContextAsInt("$C_Currency_ID"), invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(),
                                invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                        }

                        newSchedule.SetDueAmt(Util.GetValueOfDecimal(remainder));
                        // set Open Amount (Invoice)
                        newSchedule.SetVA009_OpnAmntInvce(Util.GetValueOfDecimal(remainder));
                        // set Open Amount (Base)
                        newSchedule.SetVA009_OpenAmnt(Util.GetValueOfDecimal(baseCurencyAmt));

                        if (!newSchedule.Save(invoice.Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "VIS_ScheduleNotSave") +
                                (pp != null && !string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : ""));

                            return false;
                        }

                        log.Fine("Remainder=" + remainder + " - " + schedule);
                    }

                    //	updateInvoice
                    if (invoice.GetC_PaymentTerm_ID() != GetC_PaymentTerm_ID())
                        invoice.SetC_PaymentTerm_ID(GetC_PaymentTerm_ID());
                    return invoice.ValidatePaySchedule();
                }
                else if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(C_PaySchedule_ID) FROM C_PaySchedule WHERE IsActive = 'Y' AND C_PaymentTerm_ID=" + invoice.GetC_PaymentTerm_ID())) < 1)
                {

                    /* code to check Order-invoce is creted form POS or Not */
                    MOrder order = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());

                    if (order.GetVAPOS_POSTerminal_ID() > 0)
                    {
                        return CreatePOSInvoiceSchedules(order, invoice);
                    }
                    else // default
                    {
                        schedule = new MInvoicePaySchedule(GetCtx(), 0, Get_Trx());

                        InsertSchedule(invoice, schedule);

                        if (!schedule.Save())
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "VIS_ScheduleNotSave") +
                                    (pp != null && !string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : ""));

                            return false;
                        }
                        /* we can not commit record in between - otherwise impact can't be rollback 
                               SO (POS) --> shipment created successfully --> Invoice document created but not completed --> 
                               system need to be rollback all those record which are created or impacted */
                        //if (Get_Trx() != null)
                        //{
                        //    Get_Trx().Commit();
                        //}
                        remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
                        if (remainder.CompareTo(Env.ZERO) != 0 && schedule != null)
                        {
                            schedule.SetDueAmt(Decimal.Add(schedule.GetDueAmt(), remainder));
                            schedule.Save(invoice.Get_Trx());
                            log.Fine("Remainder=" + remainder + " - " + schedule);
                        }

                        //	updateInvoice
                        if (invoice.GetC_PaymentTerm_ID() != GetC_PaymentTerm_ID())
                            invoice.SetC_PaymentTerm_ID(GetC_PaymentTerm_ID());
                        return invoice.ValidatePaySchedule();
                    }
                }
                else
                {
                    // Get due Date based on Month offSet and Month Cutoff
                    DateTime? dueDate = GetDueDate(invoice);
                    DateTime? payDueDate = null;
                    for (int i = 0; i < _schedule.Length; i++)
                    {
                        MPaySchedule _sch = new MPaySchedule(GetCtx(), _schedule[i].GetC_PaySchedule_ID(), Get_Trx());
                        if (_sch.IsVA009_Advance())
                        {
                            #region IsAdvance true on Payment Schedule

                            // is used to get record from invoice line which is created against orderline
                            String sql = @"SELECT o.C_Order_ID , NVL(SUM(il.LineTotalAmt) , 0) AS LineTotalAmt 
                                    FROM C_Invoice i INNER JOIN C_Invoiceline il ON i.C_Invoice_ID = il.C_Invoice_ID
                                    INNER JOIN C_Orderline ol ON ol.C_Orderline_ID = il.C_Orderline_ID
                                    INNER JOIN C_Order o ON o.C_Order_ID = ol.C_Order_ID
                                    WHERE i.C_Invoice_ID = " + invoice.GetC_Invoice_ID() + " GROUP BY o.C_Order_ID ";
                            DataSet _dsInvoice = DB.ExecuteDataset(sql, null, Get_Trx());
                            if (_dsInvoice != null && _dsInvoice.Tables.Count > 0 && _dsInvoice.Tables[0].Rows.Count > 0)
                            {
                                int order_Id = 0; // order reference
                                Decimal LineTotalAmt = 0; // amount against order
                                DataSet _dsOrderSchedule = null; // order schedule dataset
                                DataRow dr = null;

                                for (int K = 0; K < _dsInvoice.Tables[0].Rows.Count; K++)
                                {
                                    order_Id = Convert.ToInt32(_dsInvoice.Tables[0].Rows[K]["C_Order_ID"]);
                                    // JID_1379 : Type cast Issue
                                    LineTotalAmt = Math.Abs(Util.GetValueOfDecimal(_dsInvoice.Tables[0].Rows[K]["LineTotalAmt"]));
                                    // percentage of order amount which is to be distribute
                                    LineTotalAmt = Decimal.Multiply(LineTotalAmt, Decimal.Divide(_sch.GetPercentage(),
                                                   Decimal.Round(HUNDRED, MCurrency.GetStdPrecision(GetCtx(), invoice.GetC_Currency_ID()), MidpointRounding.AwayFromZero)));

                                    sql = "SELECT * FROM VA009_OrderPaySchedule WHERE C_Order_ID=" + order_Id + " AND VA009_IsPaid='Y'  AND NVL(VA009_AllocatedAmt,0) != DueAmt AND C_PaySchedule_ID="
                                       + _sch.GetC_PaySchedule_ID() + " ORDER BY Created";
                                    _dsOrderSchedule = DB.ExecuteDataset(sql, null, invoice.Get_Trx());

                                    if (_dsOrderSchedule != null && _dsOrderSchedule.Tables.Count > 0 && _dsOrderSchedule.Tables[0].Rows.Count > 0)
                                    {
                                        for (int j = 0; j < _dsOrderSchedule.Tables[0].Rows.Count; j++)
                                        {
                                            schedule = new MInvoicePaySchedule(GetCtx(), 0, invoice.Get_Trx());
                                            // data row - order schedule record
                                            dr = _dsOrderSchedule.Tables[0].Rows[j];

                                            copyorderschedules(schedule, invoice, dr, LineTotalAmt, _sch.GetPercentage());

                                            if (!schedule.Save(invoice.Get_Trx()))
                                            {
                                                ValueNamePair pp = VLogger.RetrieveError();
                                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "VIS_ScheduleNotSave") +
                                                (pp != null && !string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : ""));

                                                return false;
                                            }
                                            else
                                            {
                                                // Ref_C_Invoice_ID > 0 its means reverse record, then not to update VA009_AllocatedAmt from here 
                                                if (invoice.GetRef_C_Invoice_ID() <= 0)
                                                {
                                                    int no = DB.ExecuteQuery("UPDATE VA009_OrderPaySchedule SET VA009_AllocatedAmt = NVL(VA009_AllocatedAmt, 0) + "
                                                           + Util.GetValueOfDecimal(schedule.GetDueAmt()) +
                                                           @" WHERE VA009_OrderPaySchedule_ID = " + Convert.ToInt32(dr["VA009_OrderPaySchedule_ID"]), null, invoice.Get_Trx());
                                                }
                                            }
                                            remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
                                            LineTotalAmt = Decimal.Subtract(LineTotalAmt, Math.Abs(schedule.GetDueAmt()));
                                            if (LineTotalAmt == 0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region IsAdvance false on Payment Schedule
                            //remainder = invoice.GetGrandTotal();

                            schedule = new MInvoicePaySchedule(invoice, _schedule[i]);

                            MPaySchedule mschedule = new MPaySchedule(GetCtx(), _schedule[i].GetC_PaySchedule_ID(), Get_Trx());

                            InsertSchedule(invoice, schedule);

                            // Get Next Business Day if Next Business Days check box is set to true
                            if (payterm.IsNextBusinessDay())
                            {
                                payDueDate = GetNextBusinessDate(TimeUtil.AddDays(dueDate, mschedule.GetNetDays()));
                            }
                            else
                            {
                                payDueDate = TimeUtil.AddDays(dueDate, mschedule.GetNetDays());
                            }
                            schedule.SetDueDate(payDueDate);

                            schedule.SetVA009_FollowupDate(schedule.GetDueDate().Value.AddDays(payterm.GetGraceDays()));
                            schedule.SetVA009_PlannedDueDate(schedule.GetDueDate());

                            // check next business days in case of Discount Date                                
                            if (payterm.IsNextBusinessDay())
                            {
                                payDueDate = GetNextBusinessDate(schedule.GetDiscountDate());
                            }
                            else
                            {
                                payDueDate = schedule.GetDiscountDate();
                            }
                            schedule.SetDiscountDate(payDueDate);

                            if (!schedule.Save(invoice.Get_Trx()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                log.Log(Level.SEVERE, Msg.GetMsg(GetCtx(), "VIS_ScheduleNotSave") +
                                    (pp != null && !string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : ""));

                                return false;
                            }
                            /* we can not commit record in between - otherwise impact can't be rollback 
                               SO (POS) --> shipment created successfully --> Invoice document created but not completed --> 
                               system need to be rollback all those record which are created or impacted */
                            //if (Get_Trx() != null)
                            //{
                            //    Get_Trx().Commit();
                            //}
                            remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
                            #endregion
                        }
                    }
                    if (remainder.CompareTo(Env.ZERO) != 0 && schedule != null)
                    {
                        schedule.SetDueAmt(Decimal.Add(schedule.GetDueAmt(), remainder));
                        schedule.Save(invoice.Get_Trx());
                        log.Fine("Remainder=" + remainder + " - " + schedule);
                    }

                    //	updateInvoice
                    if (invoice.GetC_PaymentTerm_ID() != GetC_PaymentTerm_ID())
                        invoice.SetC_PaymentTerm_ID(GetC_PaymentTerm_ID());
                    return invoice.ValidatePaySchedule();
                }
            }
            #region
            else
            {
                return false;
            }
            #endregion
            //return true;
        }

        /// <summary>
        /// Create Scehdule for POS Order(terminal)
        /// - order may have partial tender type and contains multicurrencies 
        /// </summary>
        /// <param name="order">Order record</param>
        /// <param name="invoice">Invoice</param>
        /// <returns> trur if schedules created and validated</returns>
        private bool CreatePOSInvoiceSchedules(MOrder order, MInvoice invoice)
        {
            var runForBaseCurrency = true;
            Dictionary<string, int> baseTypeIds = new Dictionary<string, int>();
            var dr = DB.ExecuteReader("SELECT VA009_PaymentBaseType,VA009_PaymentMethod_ID FROM VA009_PaymentMethod WHERE IsActive='Y' AND C_Currency_ID IS NULL AND AD_Client_ID=" + order.GetAD_Client_ID()
                       + " AND VA009_PaymentBaseType IN ('" + X_C_Order.PAYMENTRULE_Cash + "','" + X_C_Order.PAYMENTRULE_OnCredit + "','" + X_C_Order.PAYMENTRULE_CreditCard + "','" + X_C_Order.PAYMENTRULE_ThirdPartyPayment + "')");
            ///currency id null
            while (dr.Read())
            {
                baseTypeIds[dr["VA009_PaymentBaseType"].ToString()] = Util.GetValueOfInt(dr["VA009_PaymentMethod_ID"]);
            }
            dr.Close();


            if (Env.IsModuleInstalled("VA205_") && !String.IsNullOrEmpty(order.GetVA205_Currencies()))
            {
                //Cash
                runForBaseCurrency = false;
                Dictionary<int, Decimal?> CurrencyAmounts = null;
                Dictionary<int, int> CashBooks = null;
                StringBuilder Info = new StringBuilder();
                if (!invoice.GetCashbookAndAmountList(order, Info, out CurrencyAmounts, out CashBooks))
                {
                    runForBaseCurrency = true;
                }
                else
                {
                    foreach (int currency in CurrencyAmounts.Keys)
                    {
                        /*will create Schedule with 0 amount [-ve case] if intetionally pay more amt that totala amt in cash [ cash amt equeal return Amt]  */
                        if (Util.GetValueOfDecimal(CurrencyAmounts[currency]) == 0)
                            continue;
                        //*****create schdeular for cash
                        if (!InsertSchedulePOS(invoice, baseTypeIds[X_C_Order.PAYMENTRULE_Cash], CurrencyAmounts[currency], currency))
                        {
                            log.Severe("(POS)error creating scheule for cash Amount " + CurrencyAmounts[currency]);
                            return false;
                        }
                    }

                    int uIndex = 0; //paymnet line index id , used at time of paymnet line binding with scheule id
                    // For Card Amount
                    var cardAmount = order.GetVAPOS_PayAmt();
                    if (cardAmount > 0)
                    {
                        var pCur = order.GetVA205_PaymentCurrency();
                        string[] pCon = order.GetVA205_CurrencyConversions().Split(',');
                        string[] pCurs = order.GetVA205_Currencies().Split(',');
                        decimal conversion = Convert.ToDecimal(pCon.GetValue(Array.IndexOf(pCurs, pCur)));
                        cardAmount = Math.Round((cardAmount * conversion), MCurrency.GetStdPrecision(GetCtx(), Convert.ToInt32(pCur)));
                        /// create schedules for payment 
                        int pmid = baseTypeIds[X_C_Order.PAYMENTRULE_CreditCard];
                        if (!string.IsNullOrEmpty(order.GetVAPOS_PaymentMethod()))
                        {
                            pmid = Util.GetValueOfInt(order.GetVAPOS_PaymentMethod());

                            if (int.TryParse(order.GetVAPOS_PaymentMethod(), out pmid))
                            {
                                if (!InsertSchedulePOS(invoice, pmid, cardAmount, Convert.ToInt32(pCur)))
                                {
                                    log.Severe("(POS)error creating scheule for Card  Amount " + cardAmount);
                                    return false;
                                }
                                uIndex++;
                            }
                            else
                            {

                                string[] strArr = order.GetVAPOS_PaymentMethod().Split('|');

                                foreach (var data in strArr)
                                {
                                    string[] vals = data.Split(','); // [0] amount [1] paymthdid,[2] curid [3] multipyrate
                                    pmid = Util.GetValueOfInt(vals[1]);

                                    decimal cAmt = Math.Round((Util.GetValueOfDecimal(vals[0]) * Util.GetValueOfDecimal(vals[3])),
                                                     MCurrency.GetStdPrecision(GetCtx(), Convert.ToInt32(vals[2])));
                                    if (!InsertSchedulePOS(invoice, pmid, cAmt, Convert.ToInt32(vals[2]), uIndex))
                                    {
                                        log.Severe("(POS)error creating scheule for Card  Amount " + cardAmount);
                                        return false;
                                    }
                                    uIndex++;
                                }
                            }
                        }
                        else
                        {

                            if (!InsertSchedulePOS(invoice, pmid, cardAmount, Convert.ToInt32(pCur)))
                            {
                                log.Severe("(POS)error creating scheule for card  Amount " + cardAmount);
                                return false;
                            }
                        }
                    }
                    //TPP Amt

                    var tppAmount = order.GetVAPOS_TPPAmt();
                    if (tppAmount > 0)
                    {

                        int tppid = baseTypeIds[X_C_Order.PAYMENTRULE_ThirdPartyPayment];
                        if (!string.IsNullOrEmpty(order.GetVAPOS_TPPInfo()))
                        {
                            string[] strArr = order.GetVAPOS_TPPInfo().Split('|');

                            foreach (var data in strArr)
                            {
                                string[] vals = data.Split(','); // [0] amount [1] tppmethidid,[2] curid [3] multipyrate
                                tppid = Util.GetValueOfInt(vals[1]);

                                decimal cAmt = Math.Round((Util.GetValueOfDecimal(vals[0]) * Util.GetValueOfDecimal(vals[3])),
                                                 MCurrency.GetStdPrecision(GetCtx(), Convert.ToInt32(vals[2])));
                                if (!InsertSchedulePOS(invoice, tppid, cAmt, Convert.ToInt32(vals[2]), uIndex))
                                {
                                    log.Severe("(POS)error creating scheule for Card  Amount " + cardAmount);
                                    return false;
                                }
                                uIndex++;
                            }
                        }
                        else
                        {

                            if (!InsertSchedulePOS(invoice, tppid, tppAmount, invoice.GetC_Currency_ID()))
                            {
                                log.Severe("(POS)error creating scheule for card  Amount " + cardAmount);
                                return false;
                            }
                        }
                    }
                }
            }

            if (runForBaseCurrency) //handle case in case error in multi cur 
            {
                if (order.GetVAPOS_CashPaid() > 0)
                {
                    var cAmount = order.GetVAPOS_CashPaid(); //cash
                    if (!InsertSchedulePOS(invoice, baseTypeIds[X_C_Order.PAYMENTRULE_Cash], cAmount, invoice.GetC_Currency_ID()))
                    {
                        log.Severe("(POS)error creating scheule for cash  Amount " + cAmount);
                        return false;
                    }
                }
                int uIndex = 0; // index of payment line

                if (order.GetVAPOS_PayAmt() > 0)//card
                {
                    var pAmount = order.GetVAPOS_PayAmt();//card

                    int pmid = baseTypeIds[X_C_Order.PAYMENTRULE_CreditCard];

                    if (!string.IsNullOrEmpty(order.GetVAPOS_PaymentMethod()))
                    {
                        if (int.TryParse(order.GetVAPOS_PaymentMethod(), out pmid))
                        {
                            if (!InsertSchedulePOS(invoice, pmid, pAmount, invoice.GetC_Currency_ID()))
                            {
                                log.Severe("(POS)error creating scheule for Card  Amount " + pAmount);
                                return false;
                            }
                            uIndex++;
                        }
                        else
                        {

                            string[] strArr = order.GetVAPOS_PaymentMethod().Split('|');

                            foreach (var data in strArr)
                            {
                                string[] vals = data.Split(','); // [0] amount [1] paymthdid,
                                pmid = Util.GetValueOfInt(vals[1]);

                                if (!InsertSchedulePOS(invoice, pmid, Util.GetValueOfDecimal(vals[0]), invoice.GetC_Currency_ID(), uIndex))
                                {
                                    log.Severe("(POS)error creating scheule for Card  Amount " + pAmount);
                                    return false;
                                }
                                uIndex++;
                            }
                        }
                    }
                    else
                    {

                        if (!InsertSchedulePOS(invoice, pmid, pAmount, invoice.GetC_Currency_ID(), uIndex))
                        {
                            log.Severe("(POS)error creating scheule for Card  Amount " + pAmount);
                            return false;
                        }
                        uIndex++;
                    }
                }

                //TPP Amount
                if (order.GetVAPOS_TPPAmt() > 0)//card
                {
                    var tppAmount = order.GetVAPOS_TPPAmt();//card

                    int tppid = baseTypeIds[X_C_Order.PAYMENTRULE_ThirdPartyPayment];

                    if (!string.IsNullOrEmpty(order.GetVAPOS_TPPInfo()))
                    {
                        string[] strArr = order.GetVAPOS_TPPInfo().Split('|');

                        foreach (var data in strArr)
                        {
                            string[] vals = data.Split(','); // [0] amount [1] TPPMethod,
                            tppid = Util.GetValueOfInt(vals[1]);

                            if (!InsertSchedulePOS(invoice, tppid, Util.GetValueOfDecimal(vals[0]), invoice.GetC_Currency_ID(), uIndex))
                            {
                                log.Severe("(POS)error creating scheule for Card  Amount " + tppAmount);
                                return false;
                            }
                            uIndex++;
                        }
                    }
                    else
                    {

                        if (!InsertSchedulePOS(invoice, tppid, tppAmount, invoice.GetC_Currency_ID(), uIndex))
                        {
                            log.Severe("(POS)error creating scheule for Tpp  Amount " + tppAmount);
                            return false;
                        }
                    }
                }




            }

            //Oncedit 
            if (order.GetVAPOS_CreditAmt() > 0)
            {
                var onCredit = order.GetVAPOS_CreditAmt();
                if (!InsertSchedulePOS(invoice, baseTypeIds[X_C_Order.PAYMENTRULE_OnCredit], onCredit, invoice.GetC_Currency_ID()))
                {
                    log.Severe("(POS)error creating scheule for on Credit Amount " + onCredit);
                    return false;
                }
            }


            if (order.GetVAPOS_CreditAmt() == 0 && order.GetVAPOS_PayAmt() == 0 && order.GetVAPOS_CashPaid() == 0 && order.GetVAPOS_TPPAmt() == 0)
            {
                if (!InsertSchedulePOS(invoice, invoice.GetVA009_PaymentMethod_ID(), invoice.GetGrandTotal(), invoice.GetC_Currency_ID()))
                {
                    log.Severe("(POS)error creating scheule for on default Amount " + invoice.GetGrandTotal());
                    return false;
                }
            }


            return invoice.ValidatePaySchedule();
        }

        private bool InsertSchedulePOS(MInvoice invoice, int VA009_PaymentMethod_ID, decimal? payAmt, int payCur, int index = 0)
        {
            // MPaymentTerm payterm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), Get_Trx());
            MInvoicePaySchedule schedule = new MInvoicePaySchedule(GetCtx(), 0, Get_Trx());
            schedule.SetVA009_ExecutionStatus("A");
            schedule.SetAD_Client_ID(invoice.GetAD_Client_ID());
            schedule.SetAD_Org_ID(invoice.GetAD_Org_ID());
            schedule.SetC_Invoice_ID(invoice.GetC_Invoice_ID());
            schedule.SetC_DocType_ID(invoice.GetC_DocType_ID());

            MOrder order = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());

            schedule.SetVA009_PaymentMethod_ID(VA009_PaymentMethod_ID);

            schedule.SetC_PaymentTerm_ID(invoice.GetC_PaymentTerm_ID());

            schedule.SetVA009_GrandTotal(invoice.GetGrandTotal());

            //Set Trans Currency 
            schedule.SetVA009_TransCurrency(payCur);

            if (schedule.GetDueAmt() == 0)
            {
                schedule.SetDueDate(invoice.GetDateAcct());

                decimal dueAmt = MConversionRate.Convert(GetCtx(), payAmt ?? payAmt.Value, payCur, order.GetC_Currency_ID(),
                                                                    order.GetDateAcct(), order.GetC_ConversionType_ID(), order.GetAD_Client_ID(), order.GetAD_Org_ID());

                if (invoice.GetGrandTotal() < 0)
                {
                    dueAmt = 0 - dueAmt;
                }

                schedule.SetDueAmt(dueAmt);
                schedule.SetDiscountDate(invoice.GetDateAcct());
                //schedule.SetDiscountAmt((Util.GetValueOfDecimal((invoice.GetGrandTotal() * payterm.GetDiscount()) / 100)));

                //  schedule.SetDiscountDays2(invoice.GetDateInvoiced().Value.AddDays(Util.GetValueOfInt(payterm.GetDiscountDays2())));
                // schedule.SetDiscount2((Util.GetValueOfDecimal((invoice.GetGrandTotal() * payterm.GetDiscount2()) / 100)));
            }

            //  MPaymentTerm paytrm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), Get_Trx());
            //  int _graceDay = paytrm.GetGraceDays();
            //DateTime? _followUpDay = GetDueDate(invoice);
            schedule.SetVA009_FollowupDate(invoice.GetDateAcct());
            //schedule.SetVA009_PlannedDueDate(GetDueDate(invoice));
            schedule.SetVA009_PlannedDueDate(schedule.GetDueDate());
            //schedule.SetDueDate(GetDueDate(invoice));

            // set open amount in base currency
            basecurrency(invoice, schedule);

            schedule.SetC_Currency_ID(invoice.GetC_Currency_ID());
            schedule.SetVA009_OpnAmntInvce(schedule.GetDueAmt());
            schedule.SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            //end

            //string _sqlPaymentMthd = "Select va009_paymentmode, va009_paymenttype, va009_paymenttrigger  From va009_paymentmethod where va009_paymentmethod_ID=" + invoice.GetVA009_PaymentMethod_ID() + "   AND IsActive = 'Y' AND AD_Client_ID = " + invoice.GetAD_Client_ID();
            //DataSet dsPayMthd = new DataSet();
            //dsPayMthd = DB.ExecuteDataset(_sqlPaymentMthd);
            //if (dsPayMthd.Tables != null && dsPayMthd.Tables.Count > 0 && dsPayMthd.Tables[0].Rows.Count > 0)
            //{
            //    for (int j = 0; j < dsPayMthd.Tables[0].Rows.Count; j++)
            //    {
            //        schedule.SetVA009_PaymentMode(Util.GetValueOfString(dsPayMthd.Tables[0].Rows[j]["va009_paymentmode"]));
            //        if (!String.IsNullOrEmpty(Convert.ToString(dsPayMthd.Tables[0].Rows[j]["va009_paymenttype"])))
            //        {
            //            schedule.SetVA009_PaymentType(Util.GetValueOfString(dsPayMthd.Tables[0].Rows[j]["va009_paymenttype"]));
            //        }
            //        schedule.SetVA009_PaymentTrigger(Util.GetValueOfString(dsPayMthd.Tables[0].Rows[j]["va009_paymenttrigger"]));
            //        schedule.SetVA009_ExecutionStatus("A");
            //    }
            //}
            schedule.SetProcessed(true);

            //Set Uniquekey for POS payment reference

            schedule.SetVA009_Remarks(index + "" + VA009_PaymentMethod_ID);

            if (!schedule.Save())
                return false;
            return true;
        }

        /// <summary>
        /// Get Due Date based on the settings on Payment Term.
        /// </summary>
        /// <param name="invoice">Invoice</param>
        /// <returns>Datetime, Due Date</returns>
        private DateTime? GetDueDate(MInvoice invoice)
        {
            MPaymentTerm payterm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), Get_Trx());
            String _sql = "SELECT PAYMENTTERMDUEDATE (C_PaymentTerm_ID, DateInvoiced) AS DueDate FROM C_Invoice WHERE C_Invoice_ID=" + invoice.GetC_Invoice_ID();
            DateTime? _dueDate = Util.GetValueOfDateTime(DB.ExecuteScalar(_sql.ToString(), null, Get_Trx()));
            if (_dueDate == Util.GetValueOfDateTime("1/1/0001 12:00:00 AM"))
                _dueDate = DateTime.Now;
            return _dueDate;
        }

        /// <summary>
        /// Apply Payment Term with schedule to Invoice
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <returns>bool, true-false</returns>
        private bool ApplySchedule(MInvoice invoice)
        {
            //	Create Schedule
            DeleteInvoicePaySchedule(invoice.GetC_Invoice_ID(), invoice.Get_Trx());
            Decimal remainder = invoice.GetGrandTotal();
            MPaymentTerm payterm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), invoice.Get_Trx());
            MInvoicePaySchedule schedule = null;
            StringBuilder _sql = new StringBuilder();
            //int _CountVA009 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (Env.IsModuleInstalled("VA009_"))
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(C_PaySchedule_ID) FROM C_PaySchedule WHERE IsActive = 'Y' AND C_PaymentTerm_ID=" + invoice.GetC_PaymentTerm_ID())) < 1)
                {
                    schedule = new MInvoicePaySchedule(GetCtx(), 0, Get_Trx());

                    InsertSchedule(invoice, schedule);

                    if (!schedule.Save())
                    {
                        return false;
                    }
                    /* we can not commit record in between - otherwise impact can't be rollback 
                               SO (POS) --> shipment created successfully --> Invoice document created but not completed --> 
                               system need to be rollback all those record which are created or impacted */
                    //if (Get_Trx() != null)
                    //{
                    //    Get_Trx().Commit();
                    //}
                    remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
                }
                else
                {
                    // Get due Date based on Month offSet and Month Cutoff
                    DateTime? dueDate = GetDueDate(invoice);
                    DateTime? payDueDate = null;
                    for (int i = 0; i < _schedule.Length; i++)
                    {
                        schedule = new MInvoicePaySchedule(invoice, _schedule[i]);
                        MPaySchedule mschedule = new MPaySchedule(GetCtx(), _schedule[i].GetC_PaySchedule_ID(), Get_Trx());

                        InsertSchedule(invoice, schedule);

                        // Get Next Business Day if Next Business Days check box is set to true
                        if (payterm.IsNextBusinessDay())
                        {
                            payDueDate = GetNextBusinessDate(TimeUtil.AddDays(dueDate, mschedule.GetNetDays()));
                        }
                        else
                        {
                            payDueDate = TimeUtil.AddDays(dueDate, mschedule.GetNetDays());
                        }
                        schedule.SetDueDate(payDueDate);

                        // check next business days in case of Discount Date
                        if (payterm.IsNextBusinessDay())
                        {
                            payDueDate = GetNextBusinessDate(schedule.GetDiscountDate());
                        }
                        else
                        {
                            payDueDate = schedule.GetDiscountDate();
                        }
                        schedule.SetDiscountDate(payDueDate);

                        // Set Planned Due Date and Follow Up Date from Due Date
                        schedule.SetVA009_FollowupDate(schedule.GetDueDate().Value.AddDays(payterm.GetGraceDays()));
                        schedule.SetVA009_PlannedDueDate(schedule.GetDueDate());

                        if (!schedule.Save())
                        {
                            return false;
                        }
                        /* we can not commit record in between - otherwise impact can't be rollback 
                                SO (POS) --> shipment created successfully --> Invoice document created but not completed --> 
                                system need to be rollback all those record which are created or impacted */
                        //if (Get_Trx() != null)
                        //{
                        //    Get_Trx().Commit();
                        //}
                        remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
                    }
                }
                if (remainder.CompareTo(Env.ZERO) != 0 && schedule != null)
                {
                    schedule.SetDueAmt(Decimal.Add(schedule.GetDueAmt(), remainder));
                    schedule.Save(invoice.Get_Trx());
                    log.Fine("Remainder=" + remainder + " - " + schedule);
                }
                if (invoice.GetC_PaymentTerm_ID() != GetC_PaymentTerm_ID())
                    invoice.SetC_PaymentTerm_ID(GetC_PaymentTerm_ID());
                return invoice.ValidatePaySchedule();
            }

            return true;
        }

        /// <summary>
        /// Delete existing Invoice Payment Schedule
        /// </summary>
        /// <param name="C_Invoice_ID">Invoice ID</param>
        /// <param name="trxName">trxName</param>
        private void DeleteInvoicePaySchedule(int C_Invoice_ID, Trx trxName)
        {
            String sql = "DELETE FROM C_InvoicePaySchedule WHERE C_Invoice_ID=" + C_Invoice_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            log.Fine("C_Invoice_ID=" + C_Invoice_ID + " - #" + no);
        }

        /// <summary>
        /// Is used to update VA009_AllocatedAmt as (VA009_AllocatedAmt - DueAmt) On Order schedule
        /// </summary>
        /// <param name="C_Invoice_ID">Invoice ID reference</param>
        /// <param name="trxName">transaction</param>
        private void UpdateOrderSchedule(int C_Invoice_ID, Trx trxName)
        {
            int no = DB.ExecuteQuery(@"UPDATE VA009_OrderPaySchedule osch SET VA009_AllocatedAmt = (NVL(VA009_AllocatedAmt , 0) -
                             (SELECT SUM(NVL((DueAmt) , 0)) FROM C_InvoicePaySchedule isch
                             WHERE isch.VA009_OrderPaySchedule_ID = osch.VA009_OrderPaySchedule_ID and isch.C_Invoice_ID = " + C_Invoice_ID + @" ))
                             WHERE osch.VA009_OrderPaySchedule_ID IN (SELECT VA009_OrderPaySchedule_ID FROM C_InvoicePaySchedule WHERE C_Invoice_ID = " + C_Invoice_ID + @"
                             AND VA009_OrderPaySchedule_ID > 0 )", null, trxName);
            log.Fine("Updated VA009_AllocatedAmt on Order Schedule record against C_Invoice_ID=" + C_Invoice_ID + " - #" + no);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPaymentTerm[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append(",Valid=").Append(IsValid())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            // set zero in net days if schedules exists.
            string sql = "SELECT COUNT(C_PaySchedule_ID) FROM C_PaySchedule WHERE IsActive = 'Y' AND C_PaymentTerm_ID = " + Get_ID();
            int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            if (count > 0)
            {
                SetNetDays(0);
            }

            if (IsDueFixed())
            {
                int dd = GetFixMonthDay();
                if (dd < 1 || dd > 31)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@Invalid@ @FixMonthDay@"));
                    return false;
                }
                dd = GetFixMonthCutoff();
                if (dd < 1 || dd > 31)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@Invalid@ @FixMonthCutoff@"));
                    return false;
                }

                // if user try to set Fixed Due Date on Payment Term
                // and Schedule exist against this payment term
                // Then don't save record               

                if (count > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "PayScheduleExist"));
                    return false;
                }
            }

            // when Payment Management module exist 
            // user try to make IsAdvance on Payment Term
            // and Schedule exist against this payment term
            // Then don't save record 
            if (Env.IsModuleInstalled("VA009_"))
            {
                if (IsVA009_Advance() && count > 0)
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "VIS_ScheduleExist"));
                    return false;
                }
            }

            if (!newRecord || !IsValid())
                Validate();

            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            //SI_0646_4 : System should give the warning message on any change on payment term,if  payment term is used in transaction.
            if (Is_Changed() && !newRecord)
            {
                string sql = @" SELECT SUM(COUNT) FROM (
                              SELECT COUNT(*) AS COUNT FROM C_Order  WHERE IsActive = 'Y' AND DocStatus NOT IN ('RE' , 'VO') AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                              @" UNION ALL 
                              SELECT COUNT(*) AS COUNT FROM C_Invoice  WHERE IsActive = 'Y' AND DocStatus NOT IN ('RE' , 'VO') AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                              @" UNION ALL 
                              SELECT COUNT(*) AS COUNT FROM C_Project  WHERE IsActive = 'Y' AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                              @" UNION ALL 
                              SELECT COUNT(*) AS COUNT FROM C_Contract  WHERE IsActive = 'Y' AND DocStatus NOT IN ('RE' , 'VO') AND C_PaymentTerm_ID = " + GetC_PaymentTerm_ID() +
                               " ) t";
                int no = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
                if (no > 0)
                {
                    log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "VIS_ConflictChanges"));
                }
            }
            return base.AfterSave(newRecord, success);
        }

        /// <summary>
        /// Get Next Business Day in case the Date is Non Business Days
        /// </summary>
        /// <param name="DueDate">Due Date</param>
        /// <returns>DateTime, Next Business Day</returns>
        public DateTime? GetNextBusinessDate(DateTime? DueDate)
        {
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            return MNonBusinessDay.IsNonBusinessDay(GetCtx(), DueDate, GetAD_Org_ID()) ? GetNextBusinessDate(TimeUtil.AddDays(DueDate, 1)) : DueDate;
        }

        /// <summary>
        /// Insert records into invoicepayment schedule according to schedule
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <param name="schedule">Invoice Schedule</param>
        private void InsertSchedule(MInvoice invoice, MInvoicePaySchedule schedule)
        {
            MPaymentTerm payterm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), Get_Trx());
            schedule.SetVA009_ExecutionStatus("A");
            schedule.SetAD_Client_ID(invoice.GetAD_Client_ID());
            schedule.SetAD_Org_ID(invoice.GetAD_Org_ID());
            schedule.SetC_Invoice_ID(invoice.GetC_Invoice_ID());
            schedule.SetC_DocType_ID(invoice.GetC_DocType_ID());

            //MOrder _Order = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());
            schedule.SetVA009_PaymentMethod_ID(invoice.GetVA009_PaymentMethod_ID());
            schedule.SetC_PaymentTerm_ID(invoice.GetC_PaymentTerm_ID());
            schedule.SetVA009_GrandTotal(invoice.GetGrandTotal());

            if (schedule.GetDueAmt() == 0)
            {
                // Get Next Business Day if Next Business Days check box is set to true
                DateTime? dueDate = GetDueDate(invoice);
                DateTime? payDueDate = null;
                if (payterm.IsNextBusinessDay())
                {
                    payDueDate = GetNextBusinessDate(dueDate);
                }
                else
                {
                    payDueDate = dueDate;
                }
                schedule.SetDueDate(payDueDate);

                //schedule.SetDueDate(GetDueDate(invoice));
                schedule.SetDueAmt(invoice.GetGrandTotal());

                // check next business days in case of Discount Date                
                if (payterm.IsNextBusinessDay())
                {
                    payDueDate = GetNextBusinessDate(invoice.GetDateInvoiced().Value.AddDays(Util.GetValueOfInt(payterm.GetDiscountDays())));
                }
                else
                {
                    payDueDate = invoice.GetDateInvoiced().Value.AddDays(Util.GetValueOfInt(payterm.GetDiscountDays()));
                }
                schedule.SetDiscountDate(payDueDate);

                //schedule.SetDiscountDate(invoice.GetDateInvoiced().Value.AddDays(Util.GetValueOfInt(payterm.GetDiscountDays())));
                schedule.SetDiscountAmt((Util.GetValueOfDecimal((invoice.GetGrandTotal() * payterm.GetDiscount()) / 100)));

                if (payterm.IsNextBusinessDay())
                {
                    payDueDate = GetNextBusinessDate(invoice.GetDateInvoiced().Value.AddDays(Util.GetValueOfInt(payterm.GetDiscountDays2())));
                }
                else
                {
                    payDueDate = invoice.GetDateInvoiced().Value.AddDays(Util.GetValueOfInt(payterm.GetDiscountDays2()));
                }
                schedule.SetDiscountDays2(payDueDate);

                //schedule.SetDiscountDays2(invoice.GetDateInvoiced().Value.AddDays(Util.GetValueOfInt(payterm.GetDiscountDays2())));
                schedule.SetDiscount2((Util.GetValueOfDecimal((invoice.GetGrandTotal() * payterm.GetDiscount2()) / 100)));
            }

            MPaymentTerm paytrm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), Get_Trx());
            int _graceDay = paytrm.GetGraceDays();
            //DateTime? _followUpDay = GetDueDate(invoice);
            schedule.SetVA009_FollowupDate(schedule.GetDueDate().Value.AddDays(_graceDay));
            //schedule.SetVA009_PlannedDueDate(GetDueDate(invoice));
            schedule.SetVA009_PlannedDueDate(schedule.GetDueDate());
            //schedule.SetDueDate(GetDueDate(invoice));

            // set open amount in base currency
            basecurrency(invoice, schedule);

            schedule.SetC_Currency_ID(invoice.GetC_Currency_ID());
            schedule.SetVA009_OpnAmntInvce(schedule.GetDueAmt());

            //Set Business Partner on Invoice Payment Schedule from Invoicing Schedule in case of POC Construction Module installed 
            if (Env.IsModuleInstalled("VA052_"))
            {
                int _bPartner_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VA052_INVOICESCHEDULE.c_bpartner_id
                    FROM C_INVOICELINE
                    INNER JOIN VA052_INVOICESCHEDULE
                    ON VA052_InvoiceSchedule.VA052_InvoiceSchedule_id=C_INVOICELINE.VA052_InvoiceSchedule_id WHERE C_INVOICELINE.C_Invoice_ID="
                    + invoice.GetC_Invoice_ID(), null, Get_Trx()));
                if (_bPartner_ID > 0)
                {
                    schedule.SetC_BPartner_ID(_bPartner_ID);
                }
                else
                {
                    schedule.SetC_BPartner_ID(invoice.GetC_BPartner_ID());
                }
            }
            else
            {
                schedule.SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            }
            //end

            string _sqlPaymentMthd = "SELECT VA009_PaymentMode, VA009_PaymentType, VA009_PaymentTrigger FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID="
                + invoice.GetVA009_PaymentMethod_ID() + " AND IsActive = 'Y' AND AD_Client_ID = " + invoice.GetAD_Client_ID();
            DataSet dsPayMthd = new DataSet();
            dsPayMthd = DB.ExecuteDataset(_sqlPaymentMthd);
            if (dsPayMthd != null && dsPayMthd.Tables != null && dsPayMthd.Tables.Count > 0 && dsPayMthd.Tables[0].Rows.Count > 0)
            {
                schedule.SetVA009_PaymentMode(Util.GetValueOfString(dsPayMthd.Tables[0].Rows[0]["VA009_PaymentMode"]));
                if (!String.IsNullOrEmpty(Convert.ToString(dsPayMthd.Tables[0].Rows[0]["VA009_PaymentType"])))
                {
                    schedule.SetVA009_PaymentType(Util.GetValueOfString(dsPayMthd.Tables[0].Rows[0]["VA009_PaymentType"]));
                }
                schedule.SetVA009_PaymentTrigger(Util.GetValueOfString(dsPayMthd.Tables[0].Rows[0]["VA009_PaymentTrigger"]));
                schedule.SetVA009_ExecutionStatus("A");
            }
            schedule.SetProcessed(true);
        }

        /// <summary>
        /// Convert amount in base currency
        /// </summary>
        /// <param name="invoice">MInvoice class object</param>
        /// <param name="schedule">MInvoicePaySchedule class object</param>
        private void basecurrency(MInvoice invoice, MInvoicePaySchedule schedule)
        {
            int BaseCurrency = 0;
            //            StringBuilder _sqlBsCrrncy = new StringBuilder();
            //            _sqlBsCrrncy.Append(@"SELECT UNIQUE asch.C_Currency_ID FROM c_acctschema asch INNER JOIN ad_clientinfo ci ON ci.c_acctschema1_id = asch.c_acctschema_id
            //                                 INNER JOIN ad_client c ON c.ad_client_id = ci.ad_client_id INNER JOIN c_invoice i ON c.ad_client_id    = i.ad_client_id
            //                                 WHERE i.ad_client_id = " + invoice.GetAD_Client_ID());
            //            BaseCurrency = Util.GetValueOfInt(DB.ExecuteScalar(_sqlBsCrrncy.ToString(), null, null));
            BaseCurrency = MClientInfo.Get(GetCtx(), invoice.GetAD_Client_ID()).GetC_Currency_ID();
            if (BaseCurrency != invoice.GetC_Currency_ID())
            {
                decimal multiplyRate = MConversionRate.GetRate(invoice.GetC_Currency_ID(), BaseCurrency, invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                schedule.SetVA009_OpenAmnt(schedule.GetDueAmt() * multiplyRate);
            }
            else
            {
                schedule.SetVA009_OpenAmnt(schedule.GetDueAmt());
            }
            schedule.SetVA009_BseCurrncy(BaseCurrency);
        }

        /// <summary>
        /// Copy Order schedules at invoice schedule
        /// </summary>
        /// <param name="schedule">invoice pay shedule object in which order schedule to be copied</param>
        /// <param name="invoice">invoice object</param>
        /// <param name="_ds">datarow of orderschedule</param>
        private void copyorderschedules(MInvoicePaySchedule schedule, MInvoice invoice, DataRow _ds, Decimal LineTotalAmt, Decimal schedulePercentage)
        {
            schedule.SetAD_Client_ID(Util.GetValueOfInt(_ds["AD_Client_ID"]));
            schedule.SetAD_Org_ID(Util.GetValueOfInt(_ds["AD_Org_ID"]));
            schedule.SetVA009_OrderPaySchedule_ID(Util.GetValueOfInt(_ds["VA009_OrderPaySchedule_ID"]));
            schedule.SetC_Invoice_ID(invoice.GetC_Invoice_ID());
            schedule.SetC_DocType_ID(invoice.GetC_DocType_ID());
            schedule.SetC_PaymentTerm_ID(Util.GetValueOfInt(_ds["C_PaymentTerm_ID"]));
            schedule.SetC_PaySchedule_ID(Util.GetValueOfInt(_ds["C_PaySchedule_ID"]));

            schedule.SetVA009_PaymentMethod_ID(Util.GetValueOfInt(_ds["VA009_PaymentMethod_ID"]));
            // when order schedule dont have payment method, then need to update payment method from invoice header
            if (schedule.GetVA009_PaymentMethod_ID() <= 0)
                schedule.SetVA009_PaymentMethod_ID(invoice.GetVA009_PaymentMethod_ID());

            schedule.SetDueDate(invoice.GetDateInvoiced());
            schedule.SetDiscountDate(Util.GetValueOfDateTime(_ds["DiscountDate"]));

            // amount which is to be left whose invoice schedule not created 
            Decimal remainingDueAmount = Decimal.Subtract(Util.GetValueOfDecimal(_ds["DueAmt"]), Util.GetValueOfDecimal(_ds["VA009_AllocatedAmt"]));
            // percentage of due amount which is to be distribute
            //remainingDueAmount = Decimal.Multiply(remainingDueAmount, Decimal.Divide(schedulePercentage,
            //               Decimal.Round(HUNDRED, MCurrency.GetStdPrecision(GetCtx(), invoice.GetC_Currency_ID()), MidpointRounding.AwayFromZero)));
            if (remainingDueAmount > 0)
            {
                // if invoice line having less amount then order schedule amount, than invoice schedule created with invoice line amount
                if (LineTotalAmt < remainingDueAmount)
                {
                    remainingDueAmount = LineTotalAmt;
                }
            }

            Decimal baseCurencyAmt = remainingDueAmount;
            if (invoice.GetC_Currency_ID() != GetCtx().GetContextAsInt("$C_Currency_ID")) //(invoice currency != base Currency)
            {
                baseCurencyAmt = MConversionRate.Convert(GetCtx(), baseCurencyAmt, invoice.GetC_Currency_ID(),
                    GetCtx().GetContextAsInt("$C_Currency_ID"), invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(),
                    invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
            }

            // For reverse record
            if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
            {
                schedule.SetVA009_GrandTotal(Decimal.Negate(Util.GetValueOfDecimal(_ds["VA009_GrandTotal"])));
                schedule.SetDueAmt(Decimal.Negate(remainingDueAmount));
                schedule.SetDiscountAmt(Decimal.Negate(Util.GetValueOfDecimal(_ds["DiscountAmt"])));
                schedule.SetDiscount2(Decimal.Negate(Util.GetValueOfDecimal(_ds["Discount2"])));
                // set Open Amount (Invoice)
                //schedule.SetVA009_OpnAmntInvce(Decimal.Negate(Util.GetValueOfDecimal(_ds["VA009_OpnAmntInvce"])));
                schedule.SetVA009_OpnAmntInvce(Decimal.Negate(remainingDueAmount));
                // set Open Amount (Base)
                //schedule.SetVA009_OpenAmnt(Decimal.Negate(Util.GetValueOfDecimal(_ds["VA009_OpenAmnt"])));
                schedule.SetVA009_OpenAmnt(Decimal.Negate(baseCurencyAmt));
                // Paid Amount (Base)
                //schedule.SetVA009_PaidAmnt(Decimal.Negate(Util.GetValueOfDecimal(_ds["VA009_PaidAmnt"])));
                schedule.SetVA009_PaidAmnt(Decimal.Negate(baseCurencyAmt));
                // Paid Amount (Invoice)
                //schedule.SetVA009_PaidAmntInvce(Decimal.Negate(Util.GetValueOfDecimal(_ds["VA009_PaidAmntInvce"])));
                schedule.SetVA009_PaidAmntInvce(Decimal.Negate(remainingDueAmount));
                // Variance
                // schedule.SetVA009_Variance(Decimal.Negate(Util.GetValueOfDecimal(_ds["VA009_Variance"])));
            }
            else
            {
                schedule.SetVA009_GrandTotal(Util.GetValueOfDecimal(_ds["VA009_GrandTotal"]));
                schedule.SetDueAmt(remainingDueAmount);
                schedule.SetDiscountAmt(Util.GetValueOfDecimal(_ds["DiscountAmt"]));
                schedule.SetDiscount2(Util.GetValueOfDecimal(_ds["Discount2"]));
                // set Open Amount (Invoice)
                schedule.SetVA009_OpnAmntInvce(remainingDueAmount);
                // set Open Amount (Base)
                schedule.SetVA009_OpenAmnt(baseCurencyAmt);
                // Paid Amount (Base)
                schedule.SetVA009_PaidAmnt(baseCurencyAmt);
                // Paid Amount (Invoice)
                schedule.SetVA009_PaidAmntInvce(remainingDueAmount);
                // Variance
                // schedule.SetVA009_Variance(Util.GetValueOfDecimal(_ds["VA009_Variance"]));
            }

            schedule.SetVA009_IsPaid(Util.GetValueOfString(_ds["VA009_IsPaid"]).Equals("Y"));

            schedule.SetC_Payment_ID(Util.GetValueOfInt(_ds["C_Payment_ID"]));
            schedule.SetDiscountDays2(Util.GetValueOfDateTime(_ds["DiscountDays2"]));

            schedule.SetVA009_PlannedDueDate(Util.GetValueOfDateTime(_ds["VA009_PlannedDueDate"]));
            schedule.SetC_Currency_ID(Util.GetValueOfInt(_ds["C_Currency_ID"]));
            schedule.SetVA009_BseCurrncy(Util.GetValueOfInt(_ds["VA009_bseCurrncy"]));

            schedule.SetC_BPartner_ID(Util.GetValueOfInt(_ds["C_Bpartner_ID"]));
            schedule.SetVA009_FollowupDate(Util.GetValueOfDateTime(_ds["VA009_FollowUpDate"]));
            schedule.SetVA009_PaymentMode(Util.GetValueOfString(_ds["va009_paymentmode"]));
            if (!String.IsNullOrEmpty(Convert.ToString(_ds["va009_paymenttype"])))
            {
                schedule.SetVA009_PaymentType(Util.GetValueOfString(_ds["va009_paymenttype"]));
            }
            schedule.SetVA009_PaymentTrigger(Util.GetValueOfString(_ds["va009_paymenttrigger"]));
            schedule.SetVA009_ExecutionStatus(Util.GetValueOfString(_ds["VA009_ExecutionStatus"]));
            schedule.SetProcessed(true);
        }

    }
}