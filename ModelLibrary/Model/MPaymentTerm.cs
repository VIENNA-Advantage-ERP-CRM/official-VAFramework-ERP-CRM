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

        /**
	     * 	Get Payment Schedule
	     * 	@param requery if true re-query
	     *	@return array of schedule
	     */
        public MPaySchedule[] GetSchedule(bool requery)
        {
            if (_schedule != null && !requery)
                return _schedule;
            String sql = "SELECT * FROM C_PaySchedule WHERE IsActive = 'Y' AND C_PaymentTerm_ID=" + GetC_PaymentTerm_ID() + " ORDER BY NetDays";
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

        /**
         * 	Apply Payment Term to Invoice
         *	@param invoice invoice
         *	@return true if payment schedule is valid
         */
        public bool Apply(MInvoice invoice)
        {
            if (invoice == null || invoice.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "No valid invoice - " + invoice);
                return false;
            }

            //Added By Vivek on 17/5/2016 fro Advance Payment    
            GetSchedule(true);
            if (invoice.GetC_Order_ID() != 0)
            {
                return ApplyNoSchedule(invoice);
            }
            else
                return ApplySchedule(invoice);
            //End Vivek
        }

        /// <summary>
        /// Apply Payment Term without schedule to Invoice
        /// </summary>
        /// <param name="invoice">invoice</param>
        /// <returns>bool, true-false</returns>
        #region Advance Payment by Vivek on 16/06/2016

        private bool ApplyNoSchedule(MInvoice invoice)
        {
            DeleteInvoicePaySchedule(invoice.GetC_Invoice_ID(), invoice.Get_Trx());
            StringBuilder _sql = new StringBuilder();
            MInvoicePaySchedule schedule = null;
            MPaymentTerm payterm = new MPaymentTerm(GetCtx(), invoice.GetC_PaymentTerm_ID(), invoice.Get_Trx());
            MPaySchedule sched = new MPaySchedule(GetCtx(), invoice.GetC_PaymentTerm_ID(), Get_Trx());

            Decimal remainder = invoice.GetGrandTotal();

            //int _CountVA009 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (Env.IsModuleInstalled("VA009_"))
            {
                // check if payment term is advance then copy order schedules
                if (payterm.IsVA009_Advance())
                {
                    schedule = new MInvoicePaySchedule(GetCtx(), 0, invoice.Get_Trx());
                    String sql = "SELECT * FROM VA009_OrderPaySchedule WHERE C_Order_ID=" + invoice.GetC_Order_ID() + " AND VA009_IsPaid='Y' ORDER BY Created";
                    DataSet _ds = DB.ExecuteDataset(sql, null);

                    if (_ds != null && _ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < _ds.Tables[0].Rows.Count; j++)
                        {
                            copyorderschedules(schedule, invoice, _ds);

                            if (!schedule.Save(invoice.Get_Trx()))
                            {
                                return false;
                            }
                            remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
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
                            String sql = "SELECT * FROM VA009_OrderPaySchedule WHERE C_Order_ID=" + invoice.GetC_Order_ID() + " AND VA009_IsPaid='Y' AND C_PaySchedule_ID="
                                + _sch.GetC_PaySchedule_ID() + " ORDER BY Created";
                            DataSet _ds = DB.ExecuteDataset(sql, null);

                            if (_ds != null && _ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < _ds.Tables[0].Rows.Count; j++)
                                {
                                    schedule = new MInvoicePaySchedule(GetCtx(), 0, invoice.Get_Trx());
                                    copyorderschedules(schedule, invoice, _ds);

                                    if (!schedule.Save(invoice.Get_Trx()))
                                    {
                                        return false;
                                    }
                                    remainder = Decimal.Subtract(remainder, schedule.GetDueAmt());
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


        #endregion

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
        #region Advance Payment by Vivek on 16/06/2016

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

        #endregion
        /**
         * 	Delete existing Invoice Payment Schedule
         *	@param C_Invoice_ID id
         *	@param trxName transaction
         */
        private void DeleteInvoicePaySchedule(int C_Invoice_ID, Trx trxName)
        {
            String sql = "DELETE FROM C_InvoicePaySchedule WHERE C_Invoice_ID=" + C_Invoice_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            log.Fine("C_Invoice_ID=" + C_Invoice_ID + " - #" + no);
        }


        /**************************************************************************
         * 	String Representation
         *	@return info
         */
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
                               " ) ";
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
            return MNonBusinessDay.IsNonBusinessDay(GetCtx(), DueDate) ? GetNextBusinessDate(TimeUtil.AddDays(DueDate, 1)) : DueDate;
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

            MOrder _Order = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());
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

        /**
         * 	Convert amount in base currency
         *	@param invoice invoice
         * 	@param Invoicepayschedule schedule         
         */

        private void basecurrency(MInvoice invoice, MInvoicePaySchedule schedule)
        {
            int BaseCurrency = 0;
            StringBuilder _sqlBsCrrncy = new StringBuilder();

            _sqlBsCrrncy.Append(@"SELECT UNIQUE asch.C_Currency_ID FROM c_acctschema asch INNER JOIN ad_clientinfo ci ON ci.c_acctschema1_id = asch.c_acctschema_id
                                 INNER JOIN ad_client c ON c.ad_client_id = ci.ad_client_id INNER JOIN c_invoice i ON c.ad_client_id    = i.ad_client_id
                                 WHERE i.ad_client_id = " + invoice.GetAD_Client_ID());
            BaseCurrency = Util.GetValueOfInt(DB.ExecuteScalar(_sqlBsCrrncy.ToString(), null, null));

            if (BaseCurrency != invoice.GetC_Currency_ID())
            {
                //_sqlBsCrrncy.Clear();
                //_sqlBsCrrncy.Append(@"SELECT multiplyrate FROM c_conversion_rate WHERE AD_Client_ID = " + invoice.GetAD_Client_ID() + " AND  c_currency_id  = " + invoice.GetC_Currency_ID() +
                //              " AND c_currency_to_id = " + BaseCurrency + " AND " + GlobalVariable.TO_DATE(invoice.GetDateAcct(), true) + " BETWEEN ValidFrom AND ValidTo");
                //decimal multiplyRate = Util.GetValueOfDecimal(DB.ExecuteScalar(_sqlBsCrrncy.ToString(), null, null));
                //if (multiplyRate == 0)
                //{
                //    _sqlBsCrrncy.Clear();
                //    _sqlBsCrrncy.Append(@"SELECT dividerate FROM c_conversion_rate WHERE AD_Client_ID = " + invoice.GetAD_Client_ID() + " AND c_currency_id  = " + BaseCurrency +
                //                  " AND c_currency_to_id = " + invoice.GetC_Currency_ID() + " AND " + GlobalVariable.TO_DATE(invoice.GetDateAcct(), true) + " BETWEEN ValidFrom AND ValidTo");
                //    multiplyRate = Util.GetValueOfDecimal(DB.ExecuteScalar(_sqlBsCrrncy.ToString(), null, null));
                //}
                decimal multiplyRate = MConversionRate.GetRate(invoice.GetC_Currency_ID(), BaseCurrency, invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                schedule.SetVA009_OpenAmnt(schedule.GetDueAmt() * multiplyRate);
            }
            else
            {
                schedule.SetVA009_OpenAmnt(schedule.GetDueAmt());
            }
            schedule.SetVA009_BseCurrncy(BaseCurrency);
        }


        /**
         * 	Copy Order schedules at invoice schedule
         * 	@param Invoicepayschedule schedule
         *	@param invoice invoice
         *	@param Dataset ds
         */

        private void copyorderschedules(MInvoicePaySchedule schedule, MInvoice invoice, DataSet _ds)
        {
            if (_ds.Tables[0].Rows.Count > 0 && _ds.Tables[0].Rows != null)
            {
                for (int j = 0; j < _ds.Tables[0].Rows.Count; j++)
                {
                    Boolean _isPaid = false;
                    if (Util.GetValueOfString(_ds.Tables[0].Rows[j]["VA009_IsPaid"]) == "Y")
                    {
                        _isPaid = true;
                    }
                    schedule.SetAD_Client_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["AD_Client_ID"]));
                    schedule.SetAD_Org_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["AD_Org_ID"]));
                    schedule.SetVA009_OrderPaySchedule_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA009_OrderPaySchedule_ID"]));
                    schedule.SetC_Invoice_ID(invoice.GetC_Invoice_ID());
                    schedule.SetC_DocType_ID(invoice.GetC_DocType_ID());
                    schedule.SetC_PaymentTerm_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["C_PaymentTerm_ID"]));
                    schedule.SetC_PaySchedule_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["C_PaySchedule_ID"]));
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetVA009_GrandTotal(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_GrandTotal"])));
                    }
                    else
                    {
                        schedule.SetVA009_GrandTotal(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_GrandTotal"]));
                    }
                    schedule.SetVA009_PaymentMethod_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA009_PaymentMethod_ID"]));
                    // when order schedule dont have payment method, then need to update payment method from invoice header
                    if (schedule.GetVA009_PaymentMethod_ID() <= 0)
                        schedule.SetVA009_PaymentMethod_ID(invoice.GetVA009_PaymentMethod_ID());
                    schedule.SetDueDate(invoice.GetDateInvoiced());
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetDueAmt(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["DueAmt"])));
                    }
                    else
                    {
                        schedule.SetDueAmt(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["DueAmt"]));
                    }
                    schedule.SetDiscountDate(Util.GetValueOfDateTime(_ds.Tables[0].Rows[j]["DiscountDate"]));
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetDiscountAmt(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["DiscountAmt"])));
                    }
                    else
                    {
                        schedule.SetDiscountAmt(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["DiscountAmt"]));
                    }
                    schedule.SetVA009_IsPaid(_isPaid);
                    schedule.SetC_Payment_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["C_Payment_ID"]));
                    schedule.SetDiscountDays2(Util.GetValueOfDateTime(_ds.Tables[0].Rows[j]["DiscountDays2"]));
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetDiscount2(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["Discount2"])));
                    }
                    else
                    {
                        schedule.SetDiscount2(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["Discount2"]));
                    }
                    schedule.SetVA009_PlannedDueDate(Util.GetValueOfDateTime(_ds.Tables[0].Rows[j]["VA009_PlannedDueDate"]));
                    schedule.SetC_Currency_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["C_Currency_ID"]));
                    schedule.SetVA009_BseCurrncy(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["VA009_bseCurrncy"]));
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetVA009_OpnAmntInvce(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_OpnAmntInvce"])));
                    }
                    else
                    {
                        schedule.SetVA009_OpnAmntInvce(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_OpnAmntInvce"]));
                    }
                    // set Open Amount (Base)
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetVA009_OpenAmnt(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_OpenAmnt"])));
                    }
                    else
                    {
                        schedule.SetVA009_OpenAmnt(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_OpenAmnt"]));
                    }
                    // Paid Amount (Base)
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetVA009_PaidAmnt(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_PaidAmnt"])));
                    }
                    else
                    {
                        schedule.SetVA009_PaidAmnt(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_PaidAmnt"]));
                    }
                    // Paid Amount (Invoice)
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetVA009_PaidAmntInvce(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_PaidAmntInvce"])));
                    }
                    else
                    {
                        schedule.SetVA009_PaidAmntInvce(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_PaidAmntInvce"]));
                    }
                    // Variance
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        schedule.SetVA009_Variance(Decimal.Negate(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_Variance"])));
                    }
                    else
                    {
                        schedule.SetVA009_Variance(Util.GetValueOfDecimal(_ds.Tables[0].Rows[j]["VA009_Variance"]));
                    }
                    schedule.SetC_BPartner_ID(Util.GetValueOfInt(_ds.Tables[0].Rows[j]["C_Bpartner_ID"]));
                    schedule.SetVA009_FollowupDate(Util.GetValueOfDateTime(_ds.Tables[0].Rows[j]["VA009_FollowUpDate"]));
                    schedule.SetVA009_PaymentMode(Util.GetValueOfString(_ds.Tables[0].Rows[j]["va009_paymentmode"]));
                    if (!String.IsNullOrEmpty(Convert.ToString(_ds.Tables[0].Rows[j]["va009_paymenttype"])))
                    {
                        schedule.SetVA009_PaymentType(Util.GetValueOfString(_ds.Tables[0].Rows[j]["va009_paymenttype"]));
                    }
                    schedule.SetVA009_PaymentTrigger(Util.GetValueOfString(_ds.Tables[0].Rows[j]["va009_paymenttrigger"]));
                    schedule.SetVA009_ExecutionStatus(Util.GetValueOfString(_ds.Tables[0].Rows[j]["VA009_ExecutionStatus"]));
                    schedule.SetProcessed(true);
                }
            }
        }

    }
}