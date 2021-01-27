/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DoVAB_Payment
 * Purpose        : Post Allocation Documents.
 *                  <pre>
 *                  Table:  VAB_DocAllocation
 *                  Document Types:     CMA
 *                  </pre>
 * Class Used      : Doc
 * Chronological    Development
 * Raghunandan      19-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Acct
{
    public class Doc_Allocation : Doc
    {
        //Tolearance G&L				
        private static Decimal TOLERANCE = new Decimal(0.02);
        //Facts						
        private List<Fact> _facts = null;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Allocation(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MAllocationHdr), idr, MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, trxName)
        {

        }
        public Doc_Allocation(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MAllocationHdr), dr, MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, trxName)
        {

        }

        /// <summary>
        /// Load Specific Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MAllocationHdr alloc = (MAllocationHdr)GetPO();
            SetDateDoc(alloc.GetDateTrx());
            //	Contained Objects
            _lines = LoadLines(alloc);
            return null;
        }

        /// <summary>
        /// Load Invoice Line
        /// </summary>
        /// <param name="alloc">header</param>
        /// <returns>DocLine Array</returns>
        private DocLine[] LoadLines(MAllocationHdr alloc)
        {
            List<DocLine> list = new List<DocLine>();
            MAllocationLine[] lines = alloc.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MAllocationLine line = lines[i];
                if (!lines[i].IsActive())
                {
                    continue;
                }
                DocLine_Allocation docLine = new DocLine_Allocation(line, this);

                //	Get Payment Conversion Rate
                if (line.GetVAB_Payment_ID() != 0)
                {
                    MPayment payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), GetTrxName());
                    int VAB_CurrencyType_ID = payment.GetVAB_CurrencyType_ID();
                    docLine.SetVAB_CurrencyType_ID(VAB_CurrencyType_ID);
                }
                //
                log.Fine(docLine.ToString());
                list.Add(docLine);
            }

            //	Return Array
            DocLine[] dls = new DocLine[list.Count];
            dls = list.ToArray();
            return dls;
        }


        /// <summary>
        /// Get Source Currency Balance - subtracts line and tax amounts from total - no rounding
        /// </summary>
        /// <returns>positive amount, if total invoice is bigger than lines</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  CMA.
        ///  <pre>
        ///  AR_Invoice_Payment
        ///      UnAllocatedCash DR
        ///      or C_Prepayment
        ///      DiscountExp     DR
        ///      WriteOff        DR
        ///      Receivables             CR
        ///  AR_Invoice_Cash
        ///      CashTransfer    DR
        ///      DiscountExp     DR
        ///      WriteOff        DR
        ///      Receivables             CR
        /// 
        ///  AP_Invoice_Payment
        ///      Liability       DR
        ///      DiscountRev             CR
        ///      WriteOff                CR
        ///      PaymentSelect           CR
        ///      or V_Prepayment
        ///  AP_Invoice_Cash
        ///      Liability       DR
        ///      DiscountRev             CR
        ///      WriteOff                CR
        ///      CashTransfer            CR
        ///  CashBankTransfer
        ///      -
        /// ==============================
        ///  Realized Gain & Loss
        /// 		AR/AP			DR		CR
        /// 		Realized G/L	DR		CR
        /// 
        ///
        ///  </pre>
        ///  Tax needs to be corrected for discount & write-off;
        ///  Currency gain & loss is realized here.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        //public override List<Fact> CreateFacts(MAcctSchema as1)
        //{
        //    _facts = new List<Fact>();

        //    //  create Fact Header
        //    Fact fact = new Fact(this, as1, Fact.POST_Actual);

        //    for (int i = 0; i < _lines.Length; i++)
        //    {
        //        DocLine_Allocation line = (DocLine_Allocation)_lines[i];
        //        SetVAB_BusinessPartner_ID(line.GetVAB_BusinessPartner_ID());

        //        //  CashBankTransfer - all references null and Discount/WriteOff = 0
        //        if (line.GetVAB_Payment_ID() != 0
        //            && line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Order_ID() == 0
        //            && line.GetVAB_CashJRNLLine_ID() == 0 && line.GetVAB_BusinessPartner_ID() == 0
        //            && Env.ZERO.CompareTo(line.GetDiscountAmt()) == 0
        //            && Env.ZERO.CompareTo(line.GetWriteOffAmt()) == 0)
        //        {
        //            continue;
        //        }

        //        //	Receivables/Liability Amt
        //        Decimal allocationSource = Decimal.Add(Decimal.Add(line.GetAmtSource(), line.GetDiscountAmt()), line.GetWriteOffAmt());
        //        Decimal? allocationAccounted = null;	// AR/AP balance corrected

        //        FactLine fl = null;
        //        MAccount bpAcct = null;		//	Liability/Receivables
        //        //
        //        MPayment payment = null;
        //        if (line.GetVAB_Payment_ID() != 0)
        //        {
        //            payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), GetTrxName());
        //        }
        //        MInvoice invoice = null;
        //        if (line.GetVAB_Invoice_ID() != 0)
        //        {
        //            invoice = new MInvoice(GetCtx(), line.GetVAB_Invoice_ID(), null);
        //        }

        //        //	No Invoice
        //        if (invoice == null)
        //        {
        //            //	Payment Only
        //            if (line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Payment_ID() != 0)
        //            {
        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
        //                if (fl != null && payment != null)
        //                {
        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
        //                }
        //            }
        //            else
        //            {
        //                _error = "Cannot determine SO/PO";
        //                log.Log(Level.SEVERE, _error);
        //                return null;
        //            }
        //        }
        //        //	Sales Invoice	
        //        else if (invoice.IsSOTrx())
        //        {
        //            //	Payment/Cash	DR
        //            if (line.GetVAB_Payment_ID() != 0)
        //            {
        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
        //                if (fl != null && payment != null)
        //                {
        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
        //                }
        //            }
        //            else if (line.GetVAB_CashJRNLLine_ID() != 0)
        //            {
        //                fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
        //                MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
        //                if (fl != null && cashLine.Get_ID() != 0)
        //                {
        //                    fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
        //                }
        //            }
        //            //	Discount		DR
        //            if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
        //            {
        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountExp, as1),
        //                    GetVAB_Currency_ID(), line.GetDiscountAmt(), null);
        //                if (fl != null && payment != null)
        //                {
        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
        //                }
        //            }
        //            //	Write off		DR
        //            if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
        //            {
        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
        //                    GetVAB_Currency_ID(), line.GetWriteOffAmt(), null);
        //                if (fl != null && payment != null)
        //                {
        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
        //                }
        //            }

        //            //	AR Invoice Amount	CR
        //            if (as1.IsAccrual())
        //            {
        //                bpAcct = GetAccount(Doc.ACCTTYPE_C_Receivable, as1);
        //                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, allocationSource);		//	payment currency 
        //                if (fl != null)
        //                {
        //                    allocationAccounted = Decimal.Negate(fl.GetAcctBalance());
        //                }
        //                if (fl != null && invoice != null)
        //                {
        //                    fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
        //                }
        //            }
        //            else	//	Cash Based
        //            {
        //                allocationAccounted = CreateCashBasedAcct(as1, fact, invoice, allocationSource);
        //            }
        //        }
        //        //	Purchase Invoice
        //        else
        //        {
        //            allocationSource = Decimal.Negate(allocationSource);	//	allocation is negative
        //            //	AP Invoice Amount	DR
        //            if (as1.IsAccrual())
        //            {
        //                bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
        //                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), allocationSource, null);		//	payment currency
        //                if (fl != null)
        //                {
        //                    allocationAccounted = fl.GetAcctBalance();
        //                }
        //                if (fl != null && invoice != null)
        //                {
        //                    fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
        //                }
        //            }
        //            else	//	Cash Based
        //            {
        //                allocationAccounted = CreateCashBasedAcct(as1, fact,
        //                    invoice, allocationSource);
        //            }

        //            //	Discount		CR
        //            if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
        //            {
        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountRev, as1),
        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetDiscountAmt()));
        //                if (fl != null && payment != null)
        //                {
        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
        //                }
        //            }
        //            //	Write off		CR
        //            if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
        //            {
        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetWriteOffAmt()));
        //                if (fl != null && payment != null)
        //                {
        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
        //                }
        //            }
        //            //	Payment/Cash	CR
        //            if (line.GetVAB_Payment_ID() != 0)
        //            {
        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
        //                if (fl != null && payment != null)
        //                {
        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
        //                }
        //            }
        //            else if (line.GetVAB_CashJRNLLine_ID() != 0)
        //            {
        //                fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
        //                MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
        //                if (fl != null && cashLine.Get_ID() != 0)
        //                {
        //                    fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
        //                }
        //            }
        //        }

        //        //	VAT Tax Correction
        //        if (invoice != null && as1.IsTaxCorrection())
        //        {
        //            Decimal taxCorrectionAmt = Env.ZERO;
        //            if (as1.IsTaxCorrectionDiscount())
        //            {
        //                taxCorrectionAmt = line.GetDiscountAmt();
        //            }
        //            if (as1.IsTaxCorrectionWriteOff())
        //            {
        //                taxCorrectionAmt = Decimal.Add(taxCorrectionAmt, line.GetWriteOffAmt());
        //            }
        //            //
        //            if (Env.Signum(taxCorrectionAmt) != 0)
        //            {
        //                if (!CreateTaxCorrection(as1, fact, line,
        //                    GetAccount(invoice.IsSOTrx() ? Doc.ACCTTYPE_DiscountExp : Doc.ACCTTYPE_DiscountRev, as1),
        //                    GetAccount(Doc.ACCTTYPE_WriteOff, as1)))
        //                {
        //                    _error = "Cannot create Tax correction";
        //                    return null;
        //                }
        //            }
        //        }

        //        //	Realized Gain & Loss
        //        if (invoice != null
        //            && (GetVAB_Currency_ID() != as1.GetVAB_Currency_ID()			//	payment allocation in foreign currency
        //                || GetVAB_Currency_ID() != line.GetInvoiceVAB_Currency_ID()))	//	allocation <> invoice currency
        //        {
        //            _error = CreateRealizedGainLoss(as1, fact, bpAcct, invoice, allocationSource, allocationAccounted);
        //            if (_error != null)
        //            {
        //                return null;
        //            }
        //        }

        //    }	//	for all lines

        //    //	reset line info
        //    SetVAB_BusinessPartner_ID(0);
        //    //
        //    _facts.Add(fact);
        //    return _facts;
        //}

        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            _facts = new List<Fact>();

            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);

            for (int i = 0; i < _lines.Length; i++)
            {
                Decimal paymentAount = 0, invoiceAmt = 0, payPercent = 0;
                int C_Currecy_ID = 0, VAB_CurrencyType_id = 0;
                string CurrencyDate = "";
                DocLine_Allocation line = (DocLine_Allocation)_lines[i];
                SetVAB_BusinessPartner_ID(line.GetVAB_BusinessPartner_ID());

                //  CashBankTransfer - all references null and Discount/WriteOff = 0
                if (line.GetVAB_Payment_ID() != 0
                    && line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Order_ID() == 0
                    && line.GetVAB_CashJRNLLine_ID() == 0 && line.GetVAB_BusinessPartner_ID() == 0
                    && Env.ZERO.CompareTo(line.GetDiscountAmt()) == 0
                    && Env.ZERO.CompareTo(line.GetWriteOffAmt()) == 0)
                {
                    continue;
                }

                //	Receivables/Liability Amt
                Decimal allocationSource = Decimal.Add(Decimal.Add(line.GetAmtSource(), line.GetDiscountAmt()), line.GetWriteOffAmt());
                Decimal? allocationAccounted = null;	// AR/AP balance corrected

                FactLine fl = null;
                MAccount bpAcct = null;		//	Liability/Receivables
                //
                MPayment payment = null;
                if (line.GetVAB_Payment_ID() != 0)
                {
                    payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), GetTrxName());
                }
                if (payment != null)
                {
                    paymentAount = payment.GetPayAmt();
                    if (payment.GetVAB_Currency_ID() != GetVAB_Currency_ID())
                    {
                        string CurrencyRate = " SELECT VAB_CurrencyType_id FROM VAB_ExchangeRate" +
                                             " WHERE VAB_Currency_id=" + payment.GetVAB_Currency_ID() + " AND VAB_Currency_to_ID =" + GetVAB_Currency_ID() + " AND isactive ='Y'" +
                                             " and validfrom<(select dateacct from VAB_Payment WHERE VAB_Payment_id=" + line.GetVAB_Payment_ID() + ")" +
                                             " and validto>(select dateacct from VAB_Payment WHERE VAB_Payment_id=" + line.GetVAB_Payment_ID() + ")";
                        int conversiontype_id = DB.GetSQLValue(null, CurrencyRate);
                        if (conversiontype_id == -1)
                        {
                            _error = "Conversion Not Available";
                            log.Log(Level.SEVERE, _error);
                            return null;
                        }
                        allocationSource = MConversionRate.Convert(Env.GetCtx(), allocationSource
                                  , payment.GetVAB_Currency_ID(), GetVAB_Currency_ID(), null, conversiontype_id, GetVAF_Client_ID(), GetVAF_Org_ID());
                    }
                }
                MInvoice invoice = null;
                if (line.GetVAB_Invoice_ID() != 0)
                {
                    invoice = new MInvoice(GetCtx(), line.GetVAB_Invoice_ID(), null);
                }
                if (invoice != null)
                {
                    C_Currecy_ID = DB.GetSQLValue(null, "select VAB_Currency_id from VAB_Invoice  WHERE VAB_Invoice_id     =" + line.GetVAB_Invoice_ID());
                    if (C_Currecy_ID != GetVAB_Currency_ID())
                    {
                        CurrencyDate = " SELECT VAB_CurrencyType_id FROM VAB_ExchangeRate" +
                                              " WHERE VAB_Currency_id=" + C_Currecy_ID + " AND VAB_Currency_to_ID =" + GetVAB_Currency_ID() + " AND isactive ='Y'" +
                                              " and validfrom<(select dateacct from VAB_Invoice where VAB_Invoice_id=" + line.GetVAB_Invoice_ID() + ")" +
                                              " and validto>(select dateacct from VAB_Invoice where VAB_Invoice_id=" + line.GetVAB_Invoice_ID() + ")";
                        VAB_CurrencyType_id = DB.GetSQLValue(null, CurrencyDate);
                        if (VAB_CurrencyType_id == -1)
                        {
                            _error = "Conversion Not Available";
                            log.Log(Level.SEVERE, _error);
                            return null;
                        }
                    }
                }
                int receivables_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable, as1);
                int receivablesServices_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable_Services, as1);
                string sql = " SELECT SUM(cl.linenetamt),  prod.producttype, tx.rate  FROM VAB_InvoiceLine cl INNER JOIN VAB_TaxRate tx ON (cl.VAB_TaxRate_ID=tx.VAB_TaxRate_ID) " +
                             " INNER JOIN M_product prod      ON prod.m_product_id=cl.m_product_id   WHERE VAB_Invoice_id=" + line.GetVAB_Invoice_ID() +
                             " GROUP BY prod.producttype,tx.rate UNION SELECT SUM(cl.linenetamt),  'CH',tx.rate  FROM VAB_InvoiceLine cl INNER JOIN VAB_TaxRate tx ON (cl.VAB_TaxRate_ID=tx.VAB_TaxRate_ID) " +
                             " INNER JOIN VAB_Charge prod ON prod.VAB_Charge_id=cl.VAB_Charge_id  WHERE VAB_Invoice_id     =" + line.GetVAB_Invoice_ID() + " GROUP BY tx.rate";
                string newsql = " SELECT SUM(al.amount) FROM VAB_DocAllocationLine al INNER JOIN VAB_DocAllocation alh" +
                                " ON al.VAB_DocAllocation_id=alh.VAB_DocAllocation_id WHERE  alh.posted   ='Y' and VAB_Invoice_id=" + line.GetVAB_Invoice_ID();
                //	No Invoice
                if (invoice == null)
                {
                    //	Payment Only
                    if (line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Payment_ID() != 0)
                    {
                        MDocType dbt = new MDocType(GetCtx(), payment.GetVAB_DocTypes_ID(), GetTrxName());
                        if (dbt.GetDocBaseType() == "ARR" && line.GetAmtSource() < 0)
                        {
                            fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
                                GetVAB_Currency_ID(), null, line.GetAmtSource());
                        }
                        else if (dbt.GetDocBaseType() == "APP" && line.GetAmtSource() < 0)
                        {
                            fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
                                GetVAB_Currency_ID(), null, line.GetAmtSource());
                        }
                        else
                        {
                            fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
                                GetVAB_Currency_ID(), line.GetAmtSource(), null);
                        }
                        if (fl != null && payment != null)
                        {
                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
                        }
                    }
                    else
                    {
                        _error = "Cannot determine SO/PO";
                        log.Log(Level.SEVERE, _error);
                        return null;
                    }
                }
                //	Sales Invoice	
                else if (invoice.IsSOTrx())
                {
                    Decimal? rate = Env.ZERO;
                    Decimal taxAmt = Env.ZERO;
                    Decimal serviceAmt = Env.ZERO;
                    Decimal itemAmt = Env.ZERO;
                    //	Payment/Cash	DR
                    if (line.GetVAB_Payment_ID() != 0)
                    {
                        Tuple<String, String, String> aInfo = null;
                        if (Env.HasModulePrefix("ED008_", out aInfo))
                        {
                            if ("E".Equals(payment.GetTenderType()))
                            {
                                MAccount acct = null;
                                int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT boe.ED000_BOETransit_Acct FROM ED008_BOEAccounting boe inner join VAB_Payment pmt on boe.ED008_BOE_ID=pmt.ED008_BOE_ID 
                                                                                        WHERE pmt.VAB_Payment_ID=" + line.GetVAB_Payment_ID() + " AND pmt.VAF_Client_ID = " + GetVAF_Client_ID()));
                                if (validComID > 0)
                                {
                                    acct = MAccount.Get(Env.GetCtx(), validComID);
                                }

                                if (acct == null)
                                {
                                    validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOETransit_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                    acct = MAccount.Get(Env.GetCtx(), validComID);
                                }
                                fl = fact.CreateLine(line, acct, GetVAB_Currency_ID(), line.GetAmtSource(), null);
                            }
                            else
                            {
                                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()), GetVAB_Currency_ID(), line.GetAmtSource(), null);
                            }
                        }
                        else
                        {
                            fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
                                GetVAB_Currency_ID(), line.GetAmtSource(), null);
                        }
                        if (fl != null && payment != null)
                        {
                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
                        }
                    }
                    else if (line.GetVAB_CashJRNLLine_ID() != 0)
                    {
                        fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
                            GetVAB_Currency_ID(), line.GetAmtSource(), null);
                        MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
                        if (fl != null && cashLine.Get_ID() != 0)
                        {
                            fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
                        }
                    }
                    //	Discount		DR
                    if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
                    {
                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountExp, as1),
                            GetVAB_Currency_ID(), line.GetDiscountAmt(), null);
                        if (fl != null && payment != null)
                        {
                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
                        }
                    }
                    //	Write off		DR
                    if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
                    {
                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
                            GetVAB_Currency_ID(), line.GetWriteOffAmt(), null);
                        if (fl != null && payment != null)
                        {
                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
                        }
                    }

                    if (as1.IsAccrual())
                    {
                        IDataReader idr = DB.ExecuteReader(sql);
                        MProduct product = null;
                        Decimal value = Env.ZERO;
                        value = line.GetAmtSource();
                        Decimal postedValue = Util.GetValueOfDecimal(DB.ExecuteScalar(newsql));
                        Decimal Oldpostedvalue = postedValue;
                        Decimal ConvertedValue = 0;

                        /*****************************************/
                        //New Change 29Sep2011 allcation without payment
                        if (payment != null)
                        {
                            paymentAount = payment.GetPayAmt();
                        }

                        if (invoice != null)
                        {
                            if (C_Currecy_ID != GetVAB_Currency_ID())
                            {
                                invoiceAmt = MConversionRate.Convert(Env.GetCtx(), invoice.GetGrandTotal()
                                      , C_Currecy_ID, GetVAB_Currency_ID(), null, VAB_CurrencyType_id, GetVAF_Client_ID(), GetVAF_Org_ID());
                            }
                            else
                            {
                                invoiceAmt = invoice.GetGrandTotal();
                            }
                            payPercent = Math.Abs((allocationSource / invoiceAmt) * 100);
                        }

                        /******************************************/
                        while (idr.Read())
                        {
                            ///********/
                            Decimal baseAmt = Utility.Util.GetValueOfDecimal(idr[0]);
                            rate = Utility.Util.GetValueOfDecimal(idr[2]);
                            //	TaxAmt                                   
                            taxAmt = Util.GetValueOfDecimal((baseAmt * rate) / 100);
                            int precision = MCurrency.GetStdPrecision(GetCtx(), GetVAB_Currency_ID());
                            if (taxAmt != 0 && Env.Scale(taxAmt) > precision)
                            {
                                taxAmt = Decimal.Round(taxAmt, precision, MidpointRounding.AwayFromZero);
                            }
                            if (GetVAB_Currency_ID() != C_Currecy_ID)
                            {
                                ConvertedValue = MConversionRate.Convert(Env.GetCtx(), baseAmt + taxAmt
                                      , C_Currecy_ID, GetVAB_Currency_ID(), null, VAB_CurrencyType_id, GetVAF_Client_ID(), GetVAF_Org_ID());
                            }
                            else
                            {
                                ConvertedValue = baseAmt + taxAmt;
                            }
                            // Commented By bharat as OverUnderPayment Check Box is Removed
                            //to restict Suspance ValnacePosting
                            //if (payment != null)
                            //{
                            //    if (payment.IsOverUnderPayment())
                            //    {
                            //        if (paymentAount > ConvertedValue)
                            //        {
                            //            paymentAount = paymentAount - ConvertedValue;

                            //        }
                            //        else
                            //        {
                            //            ConvertedValue = paymentAount;
                            //        }

                            //    }
                            //    else
                            //    {
                            //        //final account balanace
                            //        if (paymentAount < ConvertedValue)
                            //        {
                            //            ConvertedValue = paymentAount;
                            //        }
                            //    }
                            //}
                            //else//if payment null means only line amount adjustment so pic only line Amt 29Sep2011 to handel Suspence Balance
                            //{
                            //    if (idr[0] != DBNull.Value)
                            //    {
                            //        ConvertedValue = line.GetAmtSource();
                            //    }
                            //}

                            /***********************New Logic****************/
                            //logic to handel Invoice CreditMemo // DR
                            if (idr[0] != DBNull.Value)
                            {
                                //Account Receivable service trad aginst charge
                                //Debit in case ARCredit Memo
                                if (idr[1].ToString().ToUpper() == "S" || idr[1].ToString().ToUpper() == "R" || idr[1].ToString().ToUpper() == "E" || idr[1].ToString().ToUpper() == "CH")
                                {
                                    serviceAmt = serviceAmt + ConvertedValue;
                                    //fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
                                    //     GetVAB_Currency_ID(), ConvertedValue, null);// - line.GetAmtSource(), null);
                                }
                                else
                                {
                                    itemAmt = itemAmt + ConvertedValue;
                                    //fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
                                    //     GetVAB_Currency_ID(), ConvertedValue, null);// - line.GetAmtSource(), null);
                                }
                            }
                        }
                        if (invoice.IsCreditMemo())
                        {
                            if (serviceAmt > 0)
                            {
                                serviceAmt = (serviceAmt * payPercent) / 100;

                                fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
                                     GetVAB_Currency_ID(), serviceAmt, null);// - line.GetAmtSource(), null);
                            }
                            if (itemAmt > 0)
                            {
                                itemAmt = (itemAmt * payPercent) / 100;

                                fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
                                     GetVAB_Currency_ID(), itemAmt, null);// - line.GetAmtSource(), null);
                            }

                        }
                        else
                        {
                            if (serviceAmt > 0)
                            {
                                serviceAmt = (serviceAmt * payPercent) / 100;
                                fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
                                         GetVAB_Currency_ID(), null, serviceAmt);// Util.GetValueOfDecimal(idr[0]));
                            }
                            if (itemAmt > 0)
                            {
                                itemAmt = (itemAmt * payPercent) / 100;
                                fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
                                         GetVAB_Currency_ID(), null, itemAmt);// Util.GetValueOfDecimal(idr[0]));
                            }
                        }
                        if (idr != null)
                        {
                            idr.Close();
                            idr = null;
                        }

                        #region LogicByRaghu
                        //int receivables_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable, as1);
                        //int receivablesServices_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable_Services, as1);
                        //if (_allLinesItem)//if (_allLinesItem || !as1.IsPostServices() || receivables_ID == receivablesServices_ID)
                        //{
                        //    //here we have to diffrent entries of amount
                        //    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
                        //       GetVAB_Currency_ID(), null, allocationSource);
                        //}
                        //if (_allLinesService)
                        //{
                        //    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
                        //         GetVAB_Currency_ID(), null, allocationSource);
                        //}

                        #endregion

                        //Old code comment by raghu on 21March
                        //bpAcct = GetAccount(Doc.ACCTTYPE_C_Receivable, as1);
                        //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, allocationSource);		//	payment currency 
                        if (fl != null)
                        {
                            allocationAccounted = Decimal.Negate(fl.GetAcctBalance());
                        }
                        if (fl != null && invoice != null)
                        {
                            fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
                        }

                    }
                    else	//	Cash Based
                    {
                        allocationAccounted = CreateCashBasedAcct(as1, fact, invoice, allocationSource);
                    }
                }
                //	Purchase Invoice
                else
                {
                    Decimal? rate = Env.ZERO;
                    Decimal taxAmt = Env.ZERO;
                    Decimal serviceAmt = Env.ZERO;
                    Decimal itemAmt = Env.ZERO;
                    if (allocationSource < 0)
                    {
                        allocationSource = Decimal.Negate(allocationSource);	//	allocation is negative
                    }

                    //	AP Invoice Amount	DR
                    if (as1.IsAccrual())
                    {

                        Decimal ConvertedValue = 0;
                        if (invoice != null)
                        {
                            if (C_Currecy_ID != GetVAB_Currency_ID())
                            {
                                invoiceAmt = MConversionRate.Convert(Env.GetCtx(), invoice.GetGrandTotal()
                                      , C_Currecy_ID, GetVAB_Currency_ID(), null, VAB_CurrencyType_id, GetVAF_Client_ID(), GetVAF_Org_ID());
                            }
                            else
                            {
                                invoiceAmt = invoice.GetGrandTotal();
                            }
                            payPercent = (allocationSource / invoiceAmt) * 100;
                        }
                        /***********************New Logic****************/
                        //logic to handel Invoice CreditMemo // DR
                        if (invoice.IsCreditMemo())
                        {

                            IDataReader idr = DB.ExecuteReader(sql);
                            while (idr.Read())
                            {
                                Decimal baseAmt = Utility.Util.GetValueOfDecimal(idr[0]);
                                rate = Utility.Util.GetValueOfDecimal(idr[2]);
                                //	TaxAmt                                   
                                taxAmt = Util.GetValueOfDecimal((baseAmt * rate) / 100);
                                int precision = MCurrency.GetStdPrecision(GetCtx(), GetVAB_Currency_ID());
                                if (taxAmt != 0 && Env.Scale(taxAmt) > precision)
                                {
                                    taxAmt = Decimal.Round(taxAmt, precision, MidpointRounding.AwayFromZero);
                                }
                                if (GetVAB_Currency_ID() != C_Currecy_ID)
                                {
                                    ConvertedValue = MConversionRate.Convert(Env.GetCtx(), baseAmt + taxAmt
                                          , C_Currecy_ID, GetVAB_Currency_ID(), null, VAB_CurrencyType_id, GetVAF_Client_ID(), GetVAF_Org_ID());
                                }
                                else
                                {
                                    ConvertedValue = baseAmt + taxAmt;
                                }
                                //if payment null means only line amount adjustment so 
                                //pic only line Amt 29Sep2011 to handel Suspence Balance
                                //if (payment == null)
                                //{
                                //    if (idr[0] != DBNull.Value)
                                //    {
                                //        ConvertedValue = allocationSource;
                                //    }
                                //}
                                if (idr[0] != DBNull.Value)
                                {
                                    //Account Receivable service trad aginst charge
                                    //Debit in case ARCredit Memo
                                    if (idr[1].ToString().ToUpper() == "S" || idr[1].ToString().ToUpper() == "R" || idr[1].ToString().ToUpper() == "E" || idr[1].ToString().ToUpper() == "CH")
                                    {
                                        serviceAmt = serviceAmt + ConvertedValue;
                                        //bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability_Services, as1);//payables services agianst charge.
                                        //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, ConvertedValue);
                                    }
                                    else
                                    {
                                        itemAmt = itemAmt + ConvertedValue;
                                        //bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
                                        //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, ConvertedValue);
                                    }
                                }
                            }
                            idr.Close();
                            if (serviceAmt > 0)
                            {
                                serviceAmt = (serviceAmt * payPercent) / 100;

                                bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability_Services, as1);//payables services agianst charge.
                                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, serviceAmt);
                            }
                            if (itemAmt > 0)
                            {
                                itemAmt = (itemAmt * payPercent) / 100;

                                bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
                                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, itemAmt);
                            }
                        }
                        else/***********************New Logic****************/
                        {
                            IDataReader idr = DB.ExecuteReader(sql);
                            while (idr.Read())
                            {
                                Decimal baseAmt = Utility.Util.GetValueOfDecimal(idr[0]);
                                rate = Utility.Util.GetValueOfDecimal(idr[2]);
                                //	TaxAmt                                   
                                taxAmt = Util.GetValueOfDecimal((baseAmt * rate) / 100);
                                int precision = MCurrency.GetStdPrecision(GetCtx(), GetVAB_Currency_ID());
                                if (taxAmt != null && Env.Scale(taxAmt) > precision)
                                {
                                    taxAmt = Decimal.Round(taxAmt, precision, MidpointRounding.AwayFromZero);
                                }
                                if (GetVAB_Currency_ID() != C_Currecy_ID)
                                {
                                    ConvertedValue = MConversionRate.Convert(Env.GetCtx(), baseAmt + taxAmt
                                          , C_Currecy_ID, GetVAB_Currency_ID(), null, VAB_CurrencyType_id, GetVAF_Client_ID(), GetVAF_Org_ID());
                                }
                                else
                                {
                                    ConvertedValue = baseAmt + taxAmt;
                                }
                                //if payment null means only line amount adjustment so 
                                //pic only line Amt 29Sep2011 to handel Suspence Balance
                                //if (payment == null)
                                //{
                                //    if (idr[0] != DBNull.Value)
                                //    {
                                //        ConvertedValue = allocationSource;
                                //    }
                                //}
                                if (idr[0] != DBNull.Value)
                                {
                                    //Account Receivable service trad aginst charge
                                    //Debit in case ARCredit Memo
                                    if (idr[1].ToString().ToUpper() == "S" || idr[1].ToString().ToUpper() == "R" || idr[1].ToString().ToUpper() == "E" || idr[1].ToString().ToUpper() == "CH")
                                    {
                                        serviceAmt = serviceAmt + ConvertedValue;
                                        //bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability_Services, as1);//payables services agianst charge.
                                        //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, ConvertedValue);
                                    }
                                    else
                                    {
                                        itemAmt = itemAmt + ConvertedValue;
                                        //bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
                                        //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, ConvertedValue);
                                    }
                                }
                            }
                            idr.Close();
                            if (serviceAmt > 0)
                            {
                                serviceAmt = (serviceAmt * payPercent) / 100;

                                bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability_Services, as1);//payables services agianst charge.
                                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), serviceAmt, null);
                            }
                            if (itemAmt > 0)
                            {
                                itemAmt = (itemAmt * payPercent) / 100;

                                bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
                                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), itemAmt, null);
                            }
                            //bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
                            //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), allocationSource, null);		//	payment currency
                        }
                        if (fl != null)
                        {
                            allocationAccounted = fl.GetAcctBalance();
                        }
                        if (fl != null && invoice != null)
                        {
                            fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
                        }
                    }
                    else	//	Cash Based
                    {
                        allocationAccounted = CreateCashBasedAcct(as1, fact,
                            invoice, allocationSource);
                    }

                    //	Discount		CR
                    if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
                    {
                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountRev, as1),
                            GetVAB_Currency_ID(), null, Decimal.Negate(line.GetDiscountAmt()));
                        if (fl != null && payment != null)
                        {
                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
                        }
                    }
                    //	Write off		CR
                    if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
                    {
                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
                            GetVAB_Currency_ID(), null, Decimal.Negate(line.GetWriteOffAmt()));
                        if (fl != null && payment != null)
                        {
                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
                        }
                    }
                    //	Payment/Cash	CR
                    if (line.GetVAB_Payment_ID() != 0)
                    {
                        Tuple<String, String, String> aInfo = null;
                        if (Env.HasModulePrefix("ED008_", out aInfo))
                        {
                            if ("E".Equals(payment.GetTenderType()))
                            {
                                MAccount acct = null;
                                int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT boe.ED000_BOETransit_Acct FROM ED008_BOEAccounting boe inner join VAB_Payment pmt on boe.ED008_BOE_ID=pmt.ED008_BOE_ID 
                                                                                        WHERE pmt.VAB_Payment_ID=" + line.GetVAB_Payment_ID() + " AND pmt.VAF_Client_ID = " + GetVAF_Client_ID()));
                                if (validComID > 0)
                                {
                                    acct = MAccount.Get(Env.GetCtx(), validComID);
                                }

                                if (acct == null)
                                {
                                    validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT ED000_BOETransit_Acct FROM VAB_AccountBook_Default WHERE VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID() + " AND VAF_Client_ID = " + GetVAF_Client_ID()));
                                    acct = MAccount.Get(Env.GetCtx(), validComID);
                                }
                                fl = fact.CreateLine(line, acct, GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
                            }
                            else
                            {
                                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
                                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
                            }
                        }
                        else
                        {
                            fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
                                GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
                        }
                        if (fl != null && payment != null)
                        {
                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
                        }
                    }
                    else if (line.GetVAB_CashJRNLLine_ID() != 0)
                    {
                        fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
                            GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
                        MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
                        if (fl != null && cashLine.Get_ID() != 0)
                        {
                            fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
                        }
                    }
                }

                //	VAT Tax Correction
                if (invoice != null && as1.IsTaxCorrection())
                {
                    Decimal taxCorrectionAmt = Env.ZERO;
                    if (as1.IsTaxCorrectionDiscount())
                    {
                        taxCorrectionAmt = line.GetDiscountAmt();
                    }
                    if (as1.IsTaxCorrectionWriteOff())
                    {
                        taxCorrectionAmt = Decimal.Add(taxCorrectionAmt, line.GetWriteOffAmt());
                    }
                    //
                    if (Env.Signum(taxCorrectionAmt) != 0)
                    {
                        if (!CreateTaxCorrection(as1, fact, line,
                            GetAccount(invoice.IsSOTrx() ? Doc.ACCTTYPE_DiscountExp : Doc.ACCTTYPE_DiscountRev, as1),
                            GetAccount(Doc.ACCTTYPE_WriteOff, as1)))
                        {
                            _error = "Cannot create Tax correction";
                            return null;
                        }
                    }
                }

                //	Realized Gain & Loss
                if (invoice != null
                    && (GetVAB_Currency_ID() != as1.GetVAB_Currency_ID()			//	payment allocation in foreign currency
                        || GetVAB_Currency_ID() != line.GetInvoiceVAB_Currency_ID()))	//	allocation <> invoice currency
                {
                    //posting against currency diffrence is risticted 28-March-2011
                    //_error = CreateRealizedGainLoss(as1, fact, bpAcct, invoice, allocationSource, allocationAccounted);
                    if (_error != null)
                    {
                        return null;
                    }
                }

            }	//	for all lines

            //	reset line info
            SetVAB_BusinessPartner_ID(0);
            //
            _facts.Add(fact);
            return _facts;
        }


        /// <summary>
        /// 	Create Cash Based Acct
        /// </summary>
        /// <param name="as1"></param>
        /// <param name="fact"></param>
        /// <param name="invoice"></param>
        /// <param name="allocationSource">allocation amount (incl discount, writeoff)</param>
        /// <returns> Accounted Amt</returns>
        private Decimal? CreateCashBasedAcct(MAcctSchema as1, Fact fact, MInvoice invoice, Decimal? allocationSource)
        {
            Decimal allocationAccounted = Env.ZERO;
            //	Multiplier
            double percent = Convert.ToDouble(invoice.GetGrandTotal()) / Convert.ToDouble(allocationSource);
            if (percent > 0.99 && percent < 1.01)
            {
                percent = 1.0;
            }
            log.Config("Multiplier=" + percent + " - GrandTotal=" + invoice.GetGrandTotal()
                + " - Allocation Source=" + allocationSource);

            //	Get Invoice Postings
            DoVAB_Invoice docInvoice = (DoVAB_Invoice)Doc.Get(new MAcctSchema[] { as1 },
                MInvoice.Table_ID, invoice.GetVAB_Invoice_ID(), GetTrxName());
            docInvoice.LoadDocumentDetails();
            allocationAccounted = docInvoice.CreateFactCash(as1, fact, new Decimal(percent));
            log.Config("Allocation Accounted=" + allocationAccounted);

            //	Cash Based Commitment Release 
            if (as1.IsCreateCommitment() && !invoice.IsSOTrx())
            {
                MInvoiceLine[] lines = invoice.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    Fact factC = DoVAB_Order.GetCommitmentRelease(as1, this,
                        lines[i].GetQtyInvoiced(), lines[i].GetVAB_InvoiceLine_ID(), new Decimal(percent));
                    if (factC == null)
                    {
                        return null;
                    }
                    _facts.Add(factC);
                }
            }	//	Commitment

            return allocationAccounted;
        }

        /// <summary>
        /// Get Payment (Unallocated Payment or Payment Selection) Acct of Bank Account 
        /// </summary>
        /// <param name="as1">accounting schema</param>
        /// <param name="VAB_Payment_ID">payment</param>
        /// <returns>acct</returns>
        private MAccount GetPaymentAcct(MAcctSchema as1, int VAB_Payment_ID)
        {
            SetVAB_Bank_Acct_ID(0);
            //	Doc.ACCTTYPE_UnallocatedCash (AR) or C_Prepayment 
            //	or Doc.ACCTTYPE_PaymentSelect (AP) or V_Prepayment
            int accountType = Doc.ACCTTYPE_UnallocatedCash;
            //
            //String sql = "SELECT p.VAB_Bank_Acct_ID, d.DocBaseType, p.IsReceipt, p.IsPrepayment "
            //    + "FROM VAB_Payment p INNER JOIN VAB_DocTypes d ON (p.VAB_DocTypes_ID=d.VAB_DocTypes_ID) "
            //    + "WHERE VAB_Payment_ID=" + VAB_Payment_ID;

            // Change to set posting of Charge against Prepayment and Charge
            String sql = "SELECT p.VAB_Bank_Acct_ID, d.DocBaseType, p.IsReceipt, p.IsPrepayment, p.VAB_Charge_ID "
               + "FROM VAB_Payment p INNER JOIN VAB_DocTypes d ON (p.VAB_DocTypes_ID=d.VAB_DocTypes_ID) "
               + "WHERE VAB_Payment_ID=" + VAB_Payment_ID;

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, GetTrxName());
                if (idr.Read())
                {
                    SetVAB_Bank_Acct_ID(Utility.Util.GetValueOfInt(idr[0]));//.getInt(1));
                    if (MDocBaseType.DOCBASETYPE_APPAYMENT.Equals(Utility.Util.GetValueOfString(idr[1])))//.getString(2)))
                    {
                        accountType = Doc.ACCTTYPE_PaymentSelect;
                    }
                    //	Prepayment
                    if ("Y".Equals(Utility.Util.GetValueOfString(idr[3])))//.getString(4)))		//	Prepayment
                    {
                        // Change to set posting of Charge against Prepayment and Charge
                        int charge_ID = Util.GetValueOfInt(idr[4]);
                        if (charge_ID != 0)
                        {
                            string sqlGetCombID = "";
                            if ("Y".Equals(Utility.Util.GetValueOfString(idr[2])))
                            {
                                sqlGetCombID = "SELECT CH_Revenue_Acct FROM VAB_Charge_Acct WHERE VAB_Charge_ID=" + charge_ID + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();
                            }
                            else
                            {
                                sqlGetCombID = "SELECT CH_Expense_Acct FROM VAB_Charge_Acct WHERE VAB_Charge_ID=" + charge_ID + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();


                            }
                            int CH_VAB_Acct_ValidParameter_ID = Util.GetValueOfInt(DB.ExecuteScalar(sqlGetCombID, null, null));
                            MAccount acct = MAccount.Get(as1.GetCtx(), CH_VAB_Acct_ValidParameter_ID);
                            return acct;
                        }
                        // Change to set posting of Charge against Prepayment and Charge
                        else
                        {
                            if ("Y".Equals(Utility.Util.GetValueOfString(idr[2])))	//	Receipt
                            {
                                accountType = Doc.ACCTTYPE_C_Prepayment;
                            }
                            else
                            {
                                accountType = Doc.ACCTTYPE_V_Prepayment;
                            }
                        }
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }

            if (GetVAB_Bank_Acct_ID() <= 0)
            {
                log.Log(Level.SEVERE, "NONE for VAB_Payment_ID=" + VAB_Payment_ID);
                return null;
            }
            return GetAccount(accountType, as1);
        }

        /// <summary>
        /// 	Get Cash (Transfer) Acct of CashBook
        /// </summary>
        /// <param name="as1">accounting schema</param>
        /// <param name="VAB_CashJRNLLine_ID"></param>
        /// <returns>acct</returns>
        private MAccount GetCashAcct(MAcctSchema as1, int VAB_CashJRNLLine_ID)
        {
            String sql = "SELECT c.VAB_CashBook_ID "
                + "FROM VAB_CashJRNL c, VAB_CashJRNLLine cl "
                + "WHERE c.VAB_CashJRNL_ID=cl.VAB_CashJRNL_ID AND cl.VAB_CashJRNLLine_ID=@param1";
            SetVAB_CashBook_ID(DataBase.DB.GetSQLValue(null, sql, VAB_CashJRNLLine_ID));
            if (GetVAB_CashBook_ID() <= 0)
            {
                log.Log(Level.SEVERE, "NONE for VAB_CashJRNLLine_ID=" + VAB_CashJRNLLine_ID);
                return null;
            }
            return GetAccount(Doc.ACCTTYPE_CashTransfer, as1);
        }

        /// <summary>
        /// Create Realized Gain & Loss.
        /// Compares the Accounted Amount of the Invoice to the
        /// Accounted Amount of the Allocation
        /// </summary>
        /// <param name="as1">accounting schema</param>
        /// <param name="fact"></param>
        /// <param name="acct"></param>
        /// <param name="invoice"></param>
        /// <param name="allocationSource">source amt</param>
        /// <param name="allocationAccounted">acct amt</param>
        /// <returns>Error Message or null if OK</returns>
        private String CreateRealizedGainLoss(MAcctSchema as1, Fact fact, MAccount acct,
            MInvoice invoice, Decimal? allocationSource, Decimal? allocationAccounted)
        {
            Decimal? invoiceSource = null;
            Decimal? invoiceAccounted = null;
            //
            String sql = "SELECT "
                + (invoice.IsSOTrx()
                    ? "SUM(AmtSourceDr), SUM(AmtAcctDr)"	//	so 
                    : "SUM(AmtSourceCr), SUM(AmtAcctCr)")	//	po
                + " FROM Actual_Acct_Detail "
                + "WHERE VAF_TableView_ID=318 AND Record_ID=" + invoice.GetVAB_Invoice_ID()	//	Invoice
                + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID()
                + " AND PostingType='A'";
            //AND VAB_Currency_ID=102
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, GetTrxName());
                if (idr.Read())
                {
                    invoiceSource = Utility.Util.GetValueOfDecimal(idr[0]);///.getBigDecimal(1);
                    invoiceAccounted = Utility.Util.GetValueOfDecimal(idr[1]);//.getBigDecimal(2);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }

            // 	Requires that Invoice is Posted
            if (invoiceSource == null || invoiceAccounted == null)
            {
                return "Gain/Loss - Invoice not posted yet";
            }
            //
            String description = "Invoice=(" + invoice.GetVAB_Currency_ID() + ")" + invoiceSource + "/" + invoiceAccounted
                + " - Allocation=(" + GetVAB_Currency_ID() + ")" + allocationSource + "/" + allocationAccounted;
            log.Fine(description);
            //	Allocation not Invoice Currency
            if (GetVAB_Currency_ID() != invoice.GetVAB_Currency_ID())
            {
                Decimal allocationSourceNew = MConversionRate.Convert(GetCtx(),
                    allocationSource.Value, GetVAB_Currency_ID(),
                    invoice.GetVAB_Currency_ID(), GetDateAcct(),
                    invoice.GetVAB_CurrencyType_ID(), invoice.GetVAF_Client_ID(), invoice.GetVAF_Org_ID());
                if (allocationSourceNew == null)
                {
                    return "Gain/Loss - No Conversion from Allocation->Invoice";
                }
                String d2 = "Allocation=(" + GetVAB_Currency_ID() + ")" + allocationSource
                    + "->(" + invoice.GetVAB_Currency_ID() + ")" + allocationSourceNew;
                log.Fine(d2);
                description += " - " + d2;
                allocationSource = allocationSourceNew;
            }

            Decimal? acctDifference = null;	//	gain is negative
            //	Full Payment in currency
            if (allocationSource.Value.CompareTo(invoiceSource.Value) == 0)
            {
                acctDifference = Decimal.Subtract(invoiceAccounted.Value, allocationAccounted.Value);	//	gain is negative
                String d2 = "(full) = " + acctDifference;
                log.Fine(d2);
                description += " - " + d2;
            }
            else	//	partial or MC
            {
                //	percent of total payment
                double multiplier = Convert.ToDouble(allocationSource) / Convert.ToDouble(invoiceSource);
                //	Reduce Orig Invoice Accounted
                invoiceAccounted = Decimal.Multiply(invoiceAccounted.Value, Convert.ToDecimal(multiplier));
                //	Difference based on percentage of Orig Invoice
                acctDifference = Decimal.Subtract(invoiceAccounted.Value, allocationAccounted.Value);	//	gain is negative
                //	ignore Tolerance
                if (Math.Abs(acctDifference.Value).CompareTo(TOLERANCE) < 0)
                {
                    acctDifference = Env.ZERO;
                }
                //	Round
                int precision = as1.GetStdPrecision();
                if (Env.Scale(acctDifference.Value) > precision)
                {
                    acctDifference = Decimal.Round(acctDifference.Value, precision, MidpointRounding.AwayFromZero);
                }
                String d2 = "(partial) = " + acctDifference + " - Multiplier=" + multiplier;
                log.Fine(d2);
                description += " - " + d2;
            }

            if (Env.Signum(acctDifference.Value) == 0)
            {
                log.Fine("No Difference");
                return null;
            }

            MAccount gain = MAccount.Get(as1.GetCtx(), as1.GetAcctSchemaDefault().GetRealizedGain_Acct());
            MAccount loss = MAccount.Get(as1.GetCtx(), as1.GetAcctSchemaDefault().GetRealizedLoss_Acct());
            //
            if (invoice.IsSOTrx())
            {
                FactLine fl = fact.CreateLine(null, loss, gain,
                    as1.GetVAB_Currency_ID(), acctDifference);
                fl.SetDescription(description);
                fact.CreateLine(null, acct,
                    as1.GetVAB_Currency_ID(), Decimal.Negate(acctDifference.Value));
                fl.SetDescription(description);
            }
            else
            {
                fact.CreateLine(null, acct,
                    as1.GetVAB_Currency_ID(), acctDifference);
                fact.CreateLine(null, loss, gain,
                    as1.GetVAB_Currency_ID(), Decimal.Negate(acctDifference.Value));
            }
            return null;
        }


        /// <summary>
        /// Create Tax Correction.
        /// Requirement: Adjust the tax amount, if you did not receive the full
        /// amount of the invoice (payment discount, write-off).
        /// Applies to many countries with VAT.
        /// Example:
        /// Invoice:	Net $100 + Tax1 $15 + Tax2 $5 = Total $120
        /// Payment:	$115 (i.e. $5 underpayment)
        /// Tax Adjustment = Tax1 = 0.63 (15/120*5) Tax2 = 0.21 (5/120/5) 
        /// </summary>
        /// <param name="as1"></param>
        /// <param name="fact"></param>
        /// <param name="line">Allocation line</param>
        /// <param name="DiscountAccount">discount acct</param>
        /// <param name="WriteOffAccoint">write off acct</param>
        /// <returns>true if created</returns>
        private bool CreateTaxCorrection(MAcctSchema as1, Fact fact,
            DocLine_Allocation line,
            MAccount DiscountAccount, MAccount WriteOffAccoint)
        {
            log.Info(line.ToString());
            Decimal discount = Env.ZERO;
            if (as1.IsTaxCorrectionDiscount())
            {
                discount = line.GetDiscountAmt();
            }
            Decimal writeOff = Env.ZERO;
            if (as1.IsTaxCorrectionWriteOff())
            {
                writeOff = line.GetWriteOffAmt();
            }

            Doc_AllocationTax tax = new Doc_AllocationTax(
                DiscountAccount, discount, WriteOffAccoint, writeOff);

            //	Get Source Amounts with account
            String sql = "SELECT * "
                + "FROM Actual_Acct_Detail "
                + "WHERE VAF_TableView_ID=318 AND Record_ID=" + line.GetVAB_Invoice_ID()	//	Invoice
                + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID()
                + " AND Line_ID IS NULL";	//	header lines like tax or total

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, GetTrxName());
                while (idr.Read())
                {
                    tax.AddInvoiceFact(new MFactAcct(GetCtx(), idr, fact.Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }

            //	Invoice Not posted
            if (tax.GetLineCount() == 0)
            {
                log.Warning("Invoice not posted yet - " + line);
                return false;
            }
            //	size = 1 if no tax
            if (tax.GetLineCount() < 2)
            {
                return true;
            }
            return tax.CreateEntries(as1, fact, line);

        }

    }
}




#region Code BackUp
///********************************************************
// * Project Name   : VAdvantage
// * Class Name     : DoVAB_Payment
// * Purpose        : Post Allocation Documents.
// *                  <pre>
// *                  Table:  VAB_DocAllocation
// *                  Document Types:     CMA
// *                  </pre>
// * Class Used      : Doc
// * Chronological    Development
// * Raghunandan      19-Jan-2010
//  ******************************************************/
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using VAdvantage.Classes;
//using VAdvantage.Common;
//using VAdvantage.Process;
//////using System.Windows.Forms;
//using VAdvantage.Model;
//using VAdvantage.DataBase;
//using VAdvantage.SqlExec;
//using VAdvantage.Utility;
//using System.Data;
//using VAdvantage.Logging;
//using System.Data.SqlClient;
//using VAdvantage.Acct;

//namespace VAdvantage.Acct
//{
//    public class Doc_Allocation : Doc
//    {
//        //Tolearance G&L				
//        private static Decimal TOLERANCE = new Decimal(0.02);
//        //Facts						
//        private List<Fact> _facts = null;
//        // All lines are Service			
//        private bool _allLinesService = true;
//        //All lines are product item		
//        private bool _allLinesItem = true;

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="ass"></param>
//        /// <param name="idr"></param>
//        /// <param name="trxName"></param>
//        public Doc_Allocation(MAcctSchema[] ass, IDataReader idr, String trxName)
//            : base(ass, typeof(MAllocationHdr), idr, MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, trxName)
//        {

//        }
//        public Doc_Allocation(MAcctSchema[] ass, DataRow dr, String trxName)
//            : base(ass, typeof(MAllocationHdr), dr, MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION, trxName)
//        {

//        }

//        /// <summary>
//        /// Load Specific Document Details
//        /// </summary>
//        /// <returns>error message or null</returns>
//        public override String LoadDocumentDetails()
//        {
//            MAllocationHdr alloc = (MAllocationHdr)GetPO();
//            SetDateDoc(alloc.GetDateTrx());
//            //	Contained Objects
//            _lines = LoadLines(alloc);
//            return null;
//        }

//        /// <summary>
//        /// Load Invoice Line
//        /// </summary>
//        /// <param name="alloc">header</param>
//        /// <returns>DocLine Array</returns>
//        private DocLine[] LoadLines(MAllocationHdr alloc)
//        {
//            List<DocLine> list = new List<DocLine>();
//            MAllocationLine[] lines = alloc.GetLines(false);
//            for (int i = 0; i < lines.Length; i++)
//            {
//                MAllocationLine line = lines[i];
//                if (!lines[i].IsActive())
//                {
//                    continue;
//                }
//                DocLine_Allocation docLine = new DocLine_Allocation(line, this);

//                //	Get Payment Conversion Rate
//                if (line.GetVAB_Payment_ID() != 0)
//                {
//                    MPayment payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), GetTrxName());
//                    int VAB_CurrencyType_ID = payment.GetVAB_CurrencyType_ID();
//                    docLine.SetVAB_CurrencyType_ID(VAB_CurrencyType_ID);
//                }

//                //#region ViewAllocation21-March-2011-Raghu
//                ////logic for View Allocation-21march2011by raghu
//                //string sql = "select m_product_id from VAB_InvoiceLine WHERE VAB_Invoice_id=" + line.GetVAB_Invoice_ID() + " and Rownum=" + (i + 1);
//                ////logic for View Allocation-21march2011by raghu
//                //int id = DB.GetSQLValue(null, sql);
//                //MProduct product = MProduct.Get(Env.GetCtx(), id);
//                //if (product.GetProductType().ToUpper() == "I")
//                //{
//                //    _allLinesService = false;
//                //}
//                //else
//                //{
//                //    _allLinesItem = false;
//                //}
//                //#endregion
//                //
//                log.Fine(docLine.ToString());
//                list.Add(docLine);
//            }

//            //	Return Array
//            DocLine[] dls = new DocLine[list.Count];
//            dls = list.ToArray();
//            return dls;
//        }


//        /// <summary>
//        /// Get Source Currency Balance - subtracts line and tax amounts from total - no rounding
//        /// </summary>
//        /// <returns>positive amount, if total invoice is bigger than lines</returns>
//        public override Decimal GetBalance()
//        {
//            Decimal retValue = Env.ZERO;
//            return retValue;
//        }

//        /// <summary>
//        /// Create Facts (the accounting logic) for
//        ///  CMA.
//        ///  <pre>
//        ///  AR_Invoice_Payment
//        ///      UnAllocatedCash DR
//        ///      or C_Prepayment
//        ///      DiscountExp     DR
//        ///      WriteOff        DR
//        ///      Receivables             CR
//        ///  AR_Invoice_Cash
//        ///      CashTransfer    DR
//        ///      DiscountExp     DR
//        ///      WriteOff        DR
//        ///      Receivables             CR
//        /// 
//        ///  AP_Invoice_Payment
//        ///      Liability       DR
//        ///      DiscountRev             CR
//        ///      WriteOff                CR
//        ///      PaymentSelect           CR
//        ///      or V_Prepayment
//        ///  AP_Invoice_Cash
//        ///      Liability       DR
//        ///      DiscountRev             CR
//        ///      WriteOff                CR
//        ///      CashTransfer            CR
//        ///  CashBankTransfer
//        ///      -
//        /// ==============================
//        ///  Realized Gain & Loss
//        /// 		AR/AP			DR		CR
//        /// 		Realized G/L	DR		CR
//        /// 
//        ///
//        ///  </pre>
//        ///  Tax needs to be corrected for discount & write-off;
//        ///  Currency gain & loss is realized here.
//        /// </summary>
//        /// <param name="?"></param>
//        /// <returns></returns>
//        public override List<Fact> CreateFacts(MAcctSchema as1)
//        {
//            _facts = new List<Fact>();

//            //  create Fact Header
//            Fact fact = new Fact(this, as1, Fact.POST_Actual);

//            for (int i = 0; i < _lines.Length; i++)
//            {
//                DocLine_Allocation line = (DocLine_Allocation)_lines[i];
//                SetVAB_BusinessPartner_ID(line.GetVAB_BusinessPartner_ID());

//                //  CashBankTransfer - all references null and Discount/WriteOff = 0
//                if (line.GetVAB_Payment_ID() != 0
//                    && line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Order_ID() == 0
//                    && line.GetVAB_CashJRNLLine_ID() == 0 && line.GetVAB_BusinessPartner_ID() == 0
//                    && Env.ZERO.CompareTo(line.GetDiscountAmt()) == 0
//                    && Env.ZERO.CompareTo(line.GetWriteOffAmt()) == 0)
//                {
//                    continue;
//                }

//                //	Receivables/Liability Amt
//                Decimal allocationSource = Decimal.Add(Decimal.Add(line.GetAmtSource(), line.GetDiscountAmt()), line.GetWriteOffAmt());
//                Decimal? allocationAccounted = null;	// AR/AP balance corrected

//                FactLine fl = null;
//                MAccount bpAcct = null;		//	Liability/Receivables
//                //
//                MPayment payment = null;
//                if (line.GetVAB_Payment_ID() != 0)
//                {
//                    payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), GetTrxName());
//                }
//                MInvoice invoice = null;
//                if (line.GetVAB_Invoice_ID() != 0)
//                {
//                    invoice = new MInvoice(GetCtx(), line.GetVAB_Invoice_ID(), null);
//                }

//                //	No Invoice
//                if (invoice == null)
//                {
//                    //	Payment Only
//                    if (line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Payment_ID() != 0)
//                    {
//                        fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//                            GetVAB_Currency_ID(), line.GetAmtSource(), null);
//                        if (fl != null && payment != null)
//                        {
//                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//                        }
//                    }
//                    else
//                    {
//                        _error = "Cannot determine SO/PO";
//                        log.Log(Level.SEVERE, _error);
//                        return null;
//                    }
//                }
//                //	Sales Invoice	
//                else if (invoice.IsSOTrx())
//                {


//                    //	Payment/Cash	DR
//                    if (line.GetVAB_Payment_ID() != 0)
//                    {
//                        fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//                            GetVAB_Currency_ID(), line.GetAmtSource(), null);
//                        if (fl != null && payment != null)
//                        {
//                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//                        }
//                    }
//                    else if (line.GetVAB_CashJRNLLine_ID() != 0)
//                    {
//                        fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
//                            GetVAB_Currency_ID(), line.GetAmtSource(), null);
//                        MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
//                        if (fl != null && cashLine.Get_ID() != 0)
//                        {
//                            fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
//                        }
//                    }
//                    //	Discount		DR
//                    if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
//                    {
//                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountExp, as1),
//                            GetVAB_Currency_ID(), line.GetDiscountAmt(), null);
//                        if (fl != null && payment != null)
//                        {
//                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//                        }
//                    }
//                    //	Write off		DR
//                    if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
//                    {
//                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
//                            GetVAB_Currency_ID(), line.GetWriteOffAmt(), null);
//                        if (fl != null && payment != null)
//                        {
//                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//                        }
//                    }

//                    //	AR Invoice Amount	CR//ACCTTYPE_C_Receivable_Services
//                    if (as1.IsAccrual())
//                    {
//                        //MInvoiceLine invLine = new MInvoiceLine(invoice);
//                        // string sql = "select m_product_id from VAB_InvoiceLine WHERE VAB_InvoiceLine_id=" + line.GetVAB_Invoice_ID();// +" and Rownum=" + (i + 1);

//                        //string sql = "select sum(cl.linenetamt),prod.producttype  from VAB_InvoiceLine cl inner join" +
//                        //        " M_product prod on prod.m_product_id=cl.m_product_id WHERE VAB_Invoice_id=" + invoice.GetVAB_Invoice_ID() +
//                        //        " GROUP BY prod.producttype";

//                        string sql = " SELECT SUM(cl.linenetamt),  prod.producttype   FROM VAB_InvoiceLine cl" +
//" INNER JOIN M_product prod      ON prod.m_product_id=cl.m_product_id   WHERE VAB_Invoice_id=" + line.GetVAB_Invoice_ID() +
//" GROUP BY prod.producttype UNION SELECT SUM(cl.linenetamt),  'CH'   FROM VAB_InvoiceLine cl INNER JOIN VAB_Charge prod" +
//"     ON prod.VAB_Charge_id=cl.VAB_Charge_id  WHERE VAB_Invoice_id     =" + line.GetVAB_Invoice_ID();

//                        IDataReader idr = null;
//                        try
//                        {
//                            idr = DB.ExecuteReader(sql);
//                            MProduct product = null;
//                            int receivables_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable, as1);
//                            int receivablesServices_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable_Services, as1);
//                            Decimal value = Env.ZERO;
//                            value = line.GetAmtSource();
//                            string newsql = " SELECT SUM(al.amount) FROM VAB_DocAllocationLine al INNER JOIN VAB_DocAllocation alh" +
//                                            " ON al.VAB_DocAllocation_id=alh.VAB_DocAllocation_id WHERE  alh.posted   ='Y' and VAB_Invoice_id=" + line.GetVAB_Invoice_ID();
//                            Decimal postedValue = Util.GetValueOfDecimal(DB.ExecuteScalar(newsql));

//                            Decimal Oldpostedvalue = postedValue;
//                            while (idr.Read())
//                            {
//                                if (Oldpostedvalue > 0)
//                                {
//                                    if (Util.GetValueOfDecimal(idr[0]) <= postedValue)
//                                    {
//                                        postedValue = postedValue - Util.GetValueOfDecimal(idr[0]);
//                                        continue;
//                                    }
//                                    else
//                                    {
//                                        postedValue = Util.GetValueOfDecimal(idr[0]) - postedValue;
//                                    }
//                                }
//                                else
//                                {

//                                    postedValue = Util.GetValueOfDecimal(idr[0]);
//                                }


//                                //if (idr[1].ToString().ToUpper() == "I")
//                                //{

//                                //}
//                                Decimal amount = Decimal.Zero;
//                                if (idr[0] != DBNull.Value)
//                                {
//                                    value = value - postedValue;
//                                    if (value < 0)
//                                    {
//                                        amount = postedValue + value;
//                                        if (amount == 0 || amount < -1)
//                                            break;
//                                    }
//                                    else
//                                    {
//                                        amount = postedValue;
//                                    }

//                                    // amount = Util.GetValueOfDecimal(idr[0]) + (line.GetAmtSource() - Util.GetValueOfDecimal(idr[0]));


//                                    if (idr[1].ToString().ToUpper() == "S" || idr[1].ToString().ToUpper() == "R" || idr[1].ToString().ToUpper() == "E" || idr[1].ToString().ToUpper() == "CH")
//                                    {


//                                        fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
//                                             GetVAB_Currency_ID(), null, amount);// Util.GetValueOfDecimal(idr[0]));
//                                    }
//                                    else
//                                    {

//                                        fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
//                                             GetVAB_Currency_ID(), null, amount);// Util.GetValueOfDecimal(idr[0]));
//                                    }
//                                }

//                            }
//                            idr.Close();
//                        }
//                        catch
//                        {
//                            if (idr != null)
//                            {
//                                idr.Close();
//                            }
//                        }

//                        #region LogicByRaghu
//                        //int receivables_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable, as1);
//                        //int receivablesServices_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable_Services, as1);
//                        //if (_allLinesItem)//if (_allLinesItem || !as1.IsPostServices() || receivables_ID == receivablesServices_ID)
//                        //{
//                        //    //here we have to diffrent entries of amount
//                        //    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
//                        //       GetVAB_Currency_ID(), null, allocationSource);
//                        //}
//                        //if (_allLinesService)
//                        //{
//                        //    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
//                        //         GetVAB_Currency_ID(), null, allocationSource);
//                        //}

//                        #endregion

//                        //Old code comment by raghu on 21March
//                        //bpAcct = GetAccount(Doc.ACCTTYPE_C_Receivable, as1);
//                        //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, allocationSource);		//	payment currency 
//                        if (fl != null)
//                        {
//                            allocationAccounted = Decimal.Negate(fl.GetAcctBalance());
//                        }
//                        if (fl != null && invoice != null)
//                        {
//                            fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
//                        }
//                    }
//                    else	//	Cash Based
//                    {
//                        allocationAccounted = CreateCashBasedAcct(as1, fact, invoice, allocationSource);
//                    }


//                }
//                //	Purchase Invoice
//                else
//                {
//                    allocationSource = Decimal.Negate(allocationSource);	//	allocation is negative
//                    //	AP Invoice Amount	DR
//                    if (as1.IsAccrual())
//                    {
//                        bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
//                        fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), allocationSource, null);		//	payment currency
//                        if (fl != null)
//                        {
//                            allocationAccounted = fl.GetAcctBalance();
//                        }
//                        if (fl != null && invoice != null)
//                        {
//                            fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
//                        }
//                    }
//                    else	//	Cash Based
//                    {
//                        allocationAccounted = CreateCashBasedAcct(as1, fact,
//                            invoice, allocationSource);
//                    }

//                    //	Discount		CR
//                    if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
//                    {
//                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountRev, as1),
//                            GetVAB_Currency_ID(), null, Decimal.Negate(line.GetDiscountAmt()));
//                        if (fl != null && payment != null)
//                        {
//                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//                        }
//                    }
//                    //	Write off		CR
//                    if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
//                    {
//                        fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
//                            GetVAB_Currency_ID(), null, Decimal.Negate(line.GetWriteOffAmt()));
//                        if (fl != null && payment != null)
//                        {
//                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//                        }
//                    }
//                    //	Payment/Cash	CR
//                    if (line.GetVAB_Payment_ID() != 0)
//                    {
//                        fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//                            GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
//                        if (fl != null && payment != null)
//                        {
//                            fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//                        }
//                    }
//                    else if (line.GetVAB_CashJRNLLine_ID() != 0)
//                    {
//                        fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
//                            GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
//                        MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
//                        if (fl != null && cashLine.Get_ID() != 0)
//                        {
//                            fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
//                        }
//                    }
//                }

//                //	VAT Tax Correction
//                if (invoice != null && as1.IsTaxCorrection())
//                {
//                    Decimal taxCorrectionAmt = Env.ZERO;
//                    if (as1.IsTaxCorrectionDiscount())
//                    {
//                        taxCorrectionAmt = line.GetDiscountAmt();
//                    }
//                    if (as1.IsTaxCorrectionWriteOff())
//                    {
//                        taxCorrectionAmt = Decimal.Add(taxCorrectionAmt, line.GetWriteOffAmt());
//                    }
//                    //
//                    if (Env.Signum(taxCorrectionAmt) != 0)
//                    {
//                        if (!CreateTaxCorrection(as1, fact, line,
//                            GetAccount(invoice.IsSOTrx() ? Doc.ACCTTYPE_DiscountExp : Doc.ACCTTYPE_DiscountRev, as1),
//                            GetAccount(Doc.ACCTTYPE_WriteOff, as1)))
//                        {
//                            _error = "Cannot create Tax correction";
//                            return null;
//                        }
//                    }
//                }

//                //	Realized Gain & Loss
//                if (invoice != null
//                    && (GetVAB_Currency_ID() != as1.GetVAB_Currency_ID()			//	payment allocation in foreign currency
//                        || GetVAB_Currency_ID() != line.GetInvoiceVAB_Currency_ID()))	//	allocation <> invoice currency
//                {
//                    //posting against currency diffrence is risticted 28-March-2011
//                    //_error = CreateRealizedGainLoss(as1, fact, bpAcct, invoice, allocationSource, allocationAccounted);
//                    if (_error != null)
//                    {
//                        return null;
//                    }
//                }

//            }	//	for all lines

//            //	reset line info
//            SetVAB_BusinessPartner_ID(0);
//            //
//            _facts.Add(fact);
//            return _facts;
//        }

//        #region 28th-March-2011 by Raghu

//        /// commented by raghu
//        //public override List<Fact> CreateFacts(MAcctSchema as1)
//        //{
//        //    _facts = new List<Fact>();

//        //    //  create Fact Header
//        //    Fact fact = new Fact(this, as1, Fact.POST_Actual);

//        //    for (int i = 0; i < _lines.Length; i++)
//        //    {
//        //        DocLine_Allocation line = (DocLine_Allocation)_lines[i];
//        //        SetVAB_BusinessPartner_ID(line.GetVAB_BusinessPartner_ID());

//        //        //  CashBankTransfer - all references null and Discount/WriteOff = 0
//        //        if (line.GetVAB_Payment_ID() != 0
//        //            && line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Order_ID() == 0
//        //            && line.GetVAB_CashJRNLLine_ID() == 0 && line.GetVAB_BusinessPartner_ID() == 0
//        //            && Env.ZERO.CompareTo(line.GetDiscountAmt()) == 0
//        //            && Env.ZERO.CompareTo(line.GetWriteOffAmt()) == 0)
//        //        {
//        //            continue;
//        //        }

//        //        //	Receivables/Liability Amt
//        //        Decimal allocationSource = Decimal.Add(Decimal.Add(line.GetAmtSource(), line.GetDiscountAmt()), line.GetWriteOffAmt());
//        //        Decimal? allocationAccounted = null;	// AR/AP balance corrected

//        //        FactLine fl = null;
//        //        MAccount bpAcct = null;		//	Liability/Receivables
//        //        //
//        //        MPayment payment = null;
//        //        if (line.GetVAB_Payment_ID() != 0)
//        //        {
//        //            payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), GetTrxName());
//        //        }
//        //        MInvoice invoice = null;
//        //        if (line.GetVAB_Invoice_ID() != 0)
//        //        {
//        //            invoice = new MInvoice(GetCtx(), line.GetVAB_Invoice_ID(), null);
//        //        }

//        //        //	No Invoice
//        //        if (invoice == null)
//        //        {
//        //            //	Payment Only
//        //            if (line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Payment_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else
//        //            {
//        //                _error = "Cannot determine SO/PO";
//        //                log.Log(Level.SEVERE, _error);
//        //                return null;
//        //            }
//        //        }
//        //        //	Sales Invoice	
//        //        else if (invoice.IsSOTrx())
//        //        {
//        //            //	Payment/Cash	DR
//        //            if (line.GetVAB_Payment_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else if (line.GetVAB_CashJRNLLine_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
//        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
//        //                MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
//        //                if (fl != null && cashLine.Get_ID() != 0)
//        //                {
//        //                    fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            //	Discount		DR
//        //            if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountExp, as1),
//        //                    GetVAB_Currency_ID(), line.GetDiscountAmt(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            //	Write off		DR
//        //            if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
//        //                    GetVAB_Currency_ID(), line.GetWriteOffAmt(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }

//        //            //	AR Invoice Amount	CR
//        //            if (as1.IsAccrual())
//        //            {
//        //                #region LogicByRaghu
//        //                int receivables_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable, as1);
//        //                int receivablesServices_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable_Services, as1);
//        //                if (_allLinesItem || !as1.IsPostServices() || receivables_ID == receivablesServices_ID)
//        //                {
//        //                    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
//        //                       GetVAB_Currency_ID(), null, allocationSource);
//        //                }
//        //                else if (_allLinesService)
//        //                {
//        //                    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
//        //                         GetVAB_Currency_ID(), null, allocationSource);
//        //                }

//        //                #endregion

//        //                //OLD CODE
//        //                //bpAcct = GetAccount(Doc.ACCTTYPE_C_Receivable, as1);
//        //                //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, allocationSource);		//	payment currency 
//        //                if (fl != null)
//        //                {
//        //                    allocationAccounted = Decimal.Negate(fl.GetAcctBalance());
//        //                }
//        //                if (fl != null && invoice != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else	//	Cash Based
//        //            {
//        //                allocationAccounted = CreateCashBasedAcct(as1, fact, invoice, allocationSource);
//        //            }
//        //        }
//        //        //	Purchase Invoice
//        //        else
//        //        {
//        //            allocationSource = Decimal.Negate(allocationSource);	//	allocation is negative
//        //            //	AP Invoice Amount	DR
//        //            if (as1.IsAccrual())
//        //            {
//        //                bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
//        //                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), allocationSource, null);		//	payment currency
//        //                if (fl != null)
//        //                {
//        //                    allocationAccounted = fl.GetAcctBalance();
//        //                }
//        //                if (fl != null && invoice != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else	//	Cash Based
//        //            {
//        //                allocationAccounted = CreateCashBasedAcct(as1, fact,
//        //                    invoice, allocationSource);
//        //            }

//        //            //	Discount		CR
//        //            if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountRev, as1),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetDiscountAmt()));
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            //	Write off		CR
//        //            if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetWriteOffAmt()));
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            //	Payment/Cash	CR
//        //            if (line.GetVAB_Payment_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else if (line.GetVAB_CashJRNLLine_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
//        //                MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
//        //                if (fl != null && cashLine.Get_ID() != 0)
//        //                {
//        //                    fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
//        //                }
//        //            }
//        //        }

//        //        //	VAT Tax Correction
//        //        if (invoice != null && as1.IsTaxCorrection())
//        //        {
//        //            Decimal taxCorrectionAmt = Env.ZERO;
//        //            if (as1.IsTaxCorrectionDiscount())
//        //            {
//        //                taxCorrectionAmt = line.GetDiscountAmt();
//        //            }
//        //            if (as1.IsTaxCorrectionWriteOff())
//        //            {
//        //                taxCorrectionAmt = Decimal.Add(taxCorrectionAmt, line.GetWriteOffAmt());
//        //            }
//        //            //
//        //            if (Env.Signum(taxCorrectionAmt) != 0)
//        //            {
//        //                if (!CreateTaxCorrection(as1, fact, line,
//        //                    GetAccount(invoice.IsSOTrx() ? Doc.ACCTTYPE_DiscountExp : Doc.ACCTTYPE_DiscountRev, as1),
//        //                    GetAccount(Doc.ACCTTYPE_WriteOff, as1)))
//        //                {
//        //                    _error = "Cannot create Tax correction";
//        //                    return null;
//        //                }
//        //            }
//        //        }

//        //        //	Realized Gain & Loss
//        //        if (invoice != null
//        //            && (GetVAB_Currency_ID() != as1.GetVAB_Currency_ID()			//	payment allocation in foreign currency
//        //                || GetVAB_Currency_ID() != line.GetInvoiceVAB_Currency_ID()))	//	allocation <> invoice currency
//        //        {
//        //            _error = CreateRealizedGainLoss(as1, fact, bpAcct, invoice, allocationSource, allocationAccounted);
//        //            if (_error != null)
//        //            {
//        //                return null;
//        //            }
//        //        }

//        //    }	//	for all lines

//        //    //	reset line info
//        //    SetVAB_BusinessPartner_ID(0);
//        //    //
//        //    _facts.Add(fact);
//        //    return _facts;
//        //}
//        ///////////////////
//        //public override List<Fact> CreateFacts(MAcctSchema as1)
//        //{
//        //    _facts = new List<Fact>();

//        //    //  create Fact Header
//        //    Fact fact = new Fact(this, as1, Fact.POST_Actual);

//        //    for (int i = 0; i < _lines.Length; i++)
//        //    {
//        //        DocLine_Allocation line = (DocLine_Allocation)_lines[i];
//        //        SetVAB_BusinessPartner_ID(line.GetVAB_BusinessPartner_ID());

//        //        //  CashBankTransfer - all references null and Discount/WriteOff = 0
//        //        if (line.GetVAB_Payment_ID() != 0
//        //            && line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Order_ID() == 0
//        //            && line.GetVAB_CashJRNLLine_ID() == 0 && line.GetVAB_BusinessPartner_ID() == 0
//        //            && Env.ZERO.CompareTo(line.GetDiscountAmt()) == 0
//        //            && Env.ZERO.CompareTo(line.GetWriteOffAmt()) == 0)
//        //        {
//        //            continue;
//        //        }

//        //        // Receivables/Liability Amt
//        //        Decimal allocationSource = Decimal.Add(Decimal.Add(line.GetAmtSource(), line.GetDiscountAmt()), line.GetWriteOffAmt());
//        //        Decimal? allocationAccounted = null; // AR/AP balance corrected

//        //        FactLine fl = null;
//        //        MAccount bpAcct = null;  // Liability/Receivables
//        //        //
//        //        MPayment payment = null;
//        //        if (line.GetVAB_Payment_ID() != 0)
//        //        {
//        //            payment = new MPayment(GetCtx(), line.GetVAB_Payment_ID(), GetTrxName());
//        //        }
//        //        MInvoice invoice = null;
//        //        if (line.GetVAB_Invoice_ID() != 0)
//        //        {
//        //            invoice = new MInvoice(GetCtx(), line.GetVAB_Invoice_ID(), null);
//        //        }

//        //        // No Invoice
//        //        if (invoice == null)
//        //        {
//        //            // Payment Only
//        //            if (line.GetVAB_Invoice_ID() == 0 && line.GetVAB_Payment_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else
//        //            {
//        //                _error = "Cannot determine SO/PO";
//        //                log.Log(Level.SEVERE, _error);
//        //                return null;
//        //            }
//        //        }
//        //        // Sales Invoice 
//        //        else if (invoice.IsSOTrx())
//        //        {


//        //            // Payment/Cash DR
//        //            if (line.GetVAB_Payment_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else if (line.GetVAB_CashJRNLLine_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
//        //                    GetVAB_Currency_ID(), line.GetAmtSource(), null);
//        //                MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
//        //                if (fl != null && cashLine.Get_ID() != 0)
//        //                {
//        //                    fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            // Discount  DR
//        //            if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountExp, as1),
//        //                    GetVAB_Currency_ID(), line.GetDiscountAmt(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            // Write off  DR
//        //            if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
//        //                    GetVAB_Currency_ID(), line.GetWriteOffAmt(), null);
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }

//        //            // AR Invoice Amount CR//ACCTTYPE_C_Receivable_Services
//        //            if (as1.IsAccrual())
//        //            {
//        //                //MInvoiceLine invLine = new MInvoiceLine(invoice);
//        //                // string sql = "select m_product_id from VAB_InvoiceLine WHERE VAB_InvoiceLine_id=" + line.GetVAB_Invoice_ID();// +" and Rownum=" + (i + 1);

//        //                string sql = "select sum(cl.linenetamt),prod.producttype  from VAB_InvoiceLine cl inner join" +
//        //                        " M_product prod on prod.m_product_id=cl.m_product_id WHERE VAB_Invoice_id=" + invoice.GetVAB_Invoice_ID() +
//        //                        " GROUP BY prod.producttype";

//        //                IDataReader idr = DB.ExecuteReader(sql);
//        //                MProduct product = null;
//        //                int receivables_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable, as1);
//        //                int receivablesServices_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable_Services, as1);
//        //                while (idr.Read())
//        //                {

//        //                    //if (idr[1].ToString().ToUpper() == "I")
//        //                    //{

//        //                    //}
//        //                    if (idr[1].ToString().ToUpper() == "S" || idr[1].ToString().ToUpper() == "R" || idr[1].ToString().ToUpper() == "E")
//        //                    {
//        //                        fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
//        //                             GetVAB_Currency_ID(), null, Util.GetValueOfDecimal(idr[0]));
//        //                    }
//        //                    else
//        //                    {
//        //                        fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
//        //                             GetVAB_Currency_ID(), null, Util.GetValueOfDecimal(idr[0]));
//        //                    }

//        //                }


//        //                #region LogicByRaghu
//        //                //int receivables_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable, as1);
//        //                //int receivablesServices_ID = GetValidCombination_ID(Doc.ACCTTYPE_C_Receivable_Services, as1);
//        //                //if (_allLinesItem)//if (_allLinesItem || !as1.IsPostServices() || receivables_ID == receivablesServices_ID)
//        //                //{
//        //                //    //here we have to diffrent entries of amount
//        //                //    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivables_ID),
//        //                //       GetVAB_Currency_ID(), null, allocationSource);
//        //                //}
//        //                //if (_allLinesService)
//        //                //{
//        //                //    fl = fact.CreateLine(line, MAccount.Get(GetCtx(), receivablesServices_ID),
//        //                //         GetVAB_Currency_ID(), null, allocationSource);
//        //                //}

//        //                #endregion

//        //                //Old code comment by raghu on 21March
//        //                //bpAcct = GetAccount(Doc.ACCTTYPE_C_Receivable, as1);
//        //                //fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), null, allocationSource);  // payment currency 
//        //                if (fl != null)
//        //                {
//        //                    allocationAccounted = Decimal.Negate(fl.GetAcctBalance());
//        //                }
//        //                if (fl != null && invoice != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else // Cash Based
//        //            {
//        //                allocationAccounted = CreateCashBasedAcct(as1, fact, invoice, allocationSource);
//        //            }


//        //        }
//        //        // Purchase Invoice
//        //        else
//        //        {
//        //            allocationSource = Decimal.Negate(allocationSource); // allocation is negative
//        //            // AP Invoice Amount DR
//        //            if (as1.IsAccrual())
//        //            {
//        //                bpAcct = GetAccount(Doc.ACCTTYPE_V_Liability, as1);
//        //                fl = fact.CreateLine(line, bpAcct, GetVAB_Currency_ID(), allocationSource, null);  // payment currency
//        //                if (fl != null)
//        //                {
//        //                    allocationAccounted = fl.GetAcctBalance();
//        //                }
//        //                if (fl != null && invoice != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(invoice.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else // Cash Based
//        //            {
//        //                allocationAccounted = CreateCashBasedAcct(as1, fact,
//        //                    invoice, allocationSource);
//        //            }

//        //            // Discount  CR
//        //            if (Env.ZERO.CompareTo(line.GetDiscountAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_DiscountRev, as1),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetDiscountAmt()));
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            // Write off  CR
//        //            if (Env.ZERO.CompareTo(line.GetWriteOffAmt()) != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetAccount(Doc.ACCTTYPE_WriteOff, as1),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetWriteOffAmt()));
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            // Payment/Cash CR
//        //            if (line.GetVAB_Payment_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetPaymentAcct(as1, line.GetVAB_Payment_ID()),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
//        //                if (fl != null && payment != null)
//        //                {
//        //                    fl.SetVAF_Org_ID(payment.GetVAF_Org_ID());
//        //                }
//        //            }
//        //            else if (line.GetVAB_CashJRNLLine_ID() != 0)
//        //            {
//        //                fl = fact.CreateLine(line, GetCashAcct(as1, line.GetVAB_CashJRNLLine_ID()),
//        //                    GetVAB_Currency_ID(), null, Decimal.Negate(line.GetAmtSource()));
//        //                MCashLine cashLine = new MCashLine(GetCtx(), line.GetVAB_CashJRNLLine_ID(), GetTrxName());
//        //                if (fl != null && cashLine.Get_ID() != 0)
//        //                {
//        //                    fl.SetVAF_Org_ID(cashLine.GetVAF_Org_ID());
//        //                }
//        //            }
//        //        }

//        //        // VAT Tax Correction
//        //        if (invoice != null && as1.IsTaxCorrection())
//        //        {
//        //            Decimal taxCorrectionAmt = Env.ZERO;
//        //            if (as1.IsTaxCorrectionDiscount())
//        //            {
//        //                taxCorrectionAmt = line.GetDiscountAmt();
//        //            }
//        //            if (as1.IsTaxCorrectionWriteOff())
//        //            {
//        //                taxCorrectionAmt = Decimal.Add(taxCorrectionAmt, line.GetWriteOffAmt());
//        //            }
//        //            //
//        //            if (Env.Signum(taxCorrectionAmt) != 0)
//        //            {
//        //                if (!CreateTaxCorrection(as1, fact, line,
//        //                    GetAccount(invoice.IsSOTrx() ? Doc.ACCTTYPE_DiscountExp : Doc.ACCTTYPE_DiscountRev, as1),
//        //                    GetAccount(Doc.ACCTTYPE_WriteOff, as1)))
//        //                {
//        //                    _error = "Cannot create Tax correction";
//        //                    return null;
//        //                }
//        //            }
//        //        }

//        //        // Realized Gain & Loss
//        //        if (invoice != null
//        //            && (GetVAB_Currency_ID() != as1.GetVAB_Currency_ID()   // payment allocation in foreign currency
//        //                || GetVAB_Currency_ID() != line.GetInvoiceVAB_Currency_ID())) // allocation <> invoice currency
//        //        {
//        //            _error = CreateRealizedGainLoss(as1, fact, bpAcct, invoice, allocationSource, allocationAccounted);
//        //            if (_error != null)
//        //            {
//        //                return null;
//        //            }
//        //        }

//        //    } // for all lines

//        //    // reset line info
//        //    SetVAB_BusinessPartner_ID(0);
//        //    //
//        //    _facts.Add(fact);
//        //    return _facts;
//        //}
//        #endregion
//        /// <summary>
//        /// 	Create Cash Based Acct
//        /// </summary>
//        /// <param name="as1"></param>
//        /// <param name="fact"></param>
//        /// <param name="invoice"></param>
//        /// <param name="allocationSource">allocation amount (incl discount, writeoff)</param>
//        /// <returns> Accounted Amt</returns>
//        private Decimal? CreateCashBasedAcct(MAcctSchema as1, Fact fact, MInvoice invoice, Decimal? allocationSource)
//        {
//            Decimal allocationAccounted = Env.ZERO;
//            //	Multiplier
//            double percent = Convert.ToDouble(invoice.GetGrandTotal()) / Convert.ToDouble(allocationSource);
//            if (percent > 0.99 && percent < 1.01)
//            {
//                percent = 1.0;
//            }
//            log.Config("Multiplier=" + percent + " - GrandTotal=" + invoice.GetGrandTotal()
//                + " - Allocation Source=" + allocationSource);

//            //	Get Invoice Postings
//            DoVAB_Invoice docInvoice = (DoVAB_Invoice)Doc.Get(new MAcctSchema[] { as1 },
//                MInvoice.Table_ID, invoice.GetVAB_Invoice_ID(), GetTrxName());
//            docInvoice.LoadDocumentDetails();
//            allocationAccounted = docInvoice.CreateFactCash(as1, fact, new Decimal(percent));
//            log.Config("Allocation Accounted=" + allocationAccounted);

//            //	Cash Based Commitment Release 
//            if (as1.IsCreateCommitment() && !invoice.IsSOTrx())
//            {
//                MInvoiceLine[] lines = invoice.GetLines();
//                for (int i = 0; i < lines.Length; i++)
//                {
//                    Fact factC = DoVAB_Order.GetCommitmentRelease(as1, this,
//                        lines[i].GetQtyInvoiced(), lines[i].GetVAB_InvoiceLine_ID(), new Decimal(percent));
//                    if (factC == null)
//                    {
//                        return null;
//                    }
//                    _facts.Add(factC);
//                }
//            }	//	Commitment

//            return allocationAccounted;
//        }

//        /// <summary>
//        /// Get Payment (Unallocated Payment or Payment Selection) Acct of Bank Account
//        /// </summary>
//        /// <param name="as1">accounting schema</param>
//        /// <param name="VAB_Payment_ID">payment</param>
//        /// <returns>acct</returns>
//        private MAccount GetPaymentAcct(MAcctSchema as1, int VAB_Payment_ID)
//        {
//            SetVAB_Bank_Acct_ID(0);
//            //	Doc.ACCTTYPE_UnallocatedCash (AR) or C_Prepayment 
//            //	or Doc.ACCTTYPE_PaymentSelect (AP) or V_Prepayment
//            int accountType = Doc.ACCTTYPE_UnallocatedCash;
//            //
//            String sql = "SELECT p.VAB_Bank_Acct_ID, d.DocBaseType, p.IsReceipt, p.IsPrepayment "
//                + "FROM VAB_Payment p INNER JOIN VAB_DocTypes d ON (p.VAB_DocTypes_ID=d.VAB_DocTypes_ID) "
//                + "WHERE VAB_Payment_ID=" + VAB_Payment_ID;
//            IDataReader idr = null;
//            try
//            {
//                idr = DataBase.DB.ExecuteReader(sql, null, GetTrxName());
//                if (idr.Read())
//                {
//                    SetVAB_Bank_Acct_ID(Utility.Util.GetValueOfInt(idr[0]));//.getInt(1));
//                    if (MDocBaseType.DOCBASETYPE_APPAYMENT.Equals(Utility.Util.GetValueOfString(idr[1])))//.getString(2)))
//                    {
//                        accountType = Doc.ACCTTYPE_PaymentSelect;
//                    }
//                    //	Prepayment
//                    if ("Y".Equals(Utility.Util.GetValueOfString(idr[3])))//.getString(4)))		//	Prepayment
//                    {
//                        if ("Y".Equals(Utility.Util.GetValueOfString(idr[2])))	//	Receipt
//                        {
//                            accountType = Doc.ACCTTYPE_C_Prepayment;
//                        }
//                        else
//                        {
//                            accountType = Doc.ACCTTYPE_V_Prepayment;
//                        }
//                    }
//                }
//                idr.Close();
//            }
//            catch (Exception e)
//            {
//                if (idr != null)
//                {
//                    idr.Close();
//                    idr = null;
//                }
//                log.Log(Level.SEVERE, sql, e);
//            }

//            if (GetVAB_Bank_Acct_ID() <= 0)
//            {
//                log.Log(Level.SEVERE, "NONE for VAB_Payment_ID=" + VAB_Payment_ID);
//                return null;
//            }
//            return GetAccount(accountType, as1);
//        }

//        /// <summary>
//        /// 	Get Cash (Transfer) Acct of CashBook
//        /// </summary>
//        /// <param name="as1">accounting schema</param>
//        /// <param name="VAB_CashJRNLLine_ID"></param>
//        /// <returns>acct</returns>
//        private MAccount GetCashAcct(MAcctSchema as1, int VAB_CashJRNLLine_ID)
//        {
//            String sql = "SELECT c.VAB_CashBook_ID "
//                + "FROM VAB_CashJRNL c, VAB_CashJRNLLine cl "
//                + "WHERE c.VAB_CashJRNL_ID=cl.VAB_CashJRNL_ID AND cl.VAB_CashJRNLLine_ID=@param1";
//            SetVAB_CashBook_ID(DataBase.DB.GetSQLValue(null, sql, VAB_CashJRNLLine_ID));
//            if (GetVAB_CashBook_ID() <= 0)
//            {
//                log.Log(Level.SEVERE, "NONE for VAB_CashJRNLLine_ID=" + VAB_CashJRNLLine_ID);
//                return null;
//            }
//            return GetAccount(Doc.ACCTTYPE_CashTransfer, as1);
//        }

//        /// <summary>
//        /// Create Realized Gain & Loss.
//        /// Compares the Accounted Amount of the Invoice to the
//        /// Accounted Amount of the Allocation
//        /// </summary>
//        /// <param name="as1">accounting schema</param>
//        /// <param name="fact"></param>
//        /// <param name="acct"></param>
//        /// <param name="invoice"></param>
//        /// <param name="allocationSource">source amt</param>
//        /// <param name="allocationAccounted">acct amt</param>
//        /// <returns>Error Message or null if OK</returns>
//        private String CreateRealizedGainLoss(MAcctSchema as1, Fact fact, MAccount acct,
//            MInvoice invoice, Decimal? allocationSource, Decimal? allocationAccounted)
//        {
//            Decimal? invoiceSource = null;
//            Decimal? invoiceAccounted = null;
//            //
//            String sql = "SELECT "
//                + (invoice.IsSOTrx()
//                    ? "SUM(AmtSourceDr), SUM(AmtAcctDr)"	//	so 
//                    : "SUM(AmtSourceCr), SUM(AmtAcctCr)")	//	po
//                + " FROM Actual_Acct_Detail "
//                + "WHERE VAF_TableView_ID=318 AND Record_ID=" + invoice.GetVAB_Invoice_ID()	//	Invoice
//                + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID()
//                + " AND PostingType='A'";
//            //AND VAB_Currency_ID=102
//            IDataReader idr = null;
//            try
//            {
//                idr = DataBase.DB.ExecuteReader(sql, null, GetTrxName());
//                if (idr.Read())
//                {
//                    invoiceSource = Utility.Util.GetValueOfDecimal(idr[0]);///.getBigDecimal(1);
//                    invoiceAccounted = Utility.Util.GetValueOfDecimal(idr[1]);//.getBigDecimal(2);
//                }
//                idr.Close();
//            }
//            catch (Exception e)
//            {
//                if (idr != null)
//                {
//                    idr.Close();
//                    idr = null;
//                }
//                log.Log(Level.SEVERE, sql, e);
//            }

//            // 	Requires that Invoice is Posted
//            if (invoiceSource == null || invoiceAccounted == null)
//            {
//                return "Gain/Loss - Invoice not posted yet";
//            }
//            //
//            String description = "Invoice=(" + invoice.GetVAB_Currency_ID() + ")" + invoiceSource + "/" + invoiceAccounted
//                + " - Allocation=(" + GetVAB_Currency_ID() + ")" + allocationSource + "/" + allocationAccounted;
//            log.Fine(description);
//            //	Allocation not Invoice Currency
//            if (GetVAB_Currency_ID() != invoice.GetVAB_Currency_ID())
//            {
//                Decimal allocationSourceNew = MConversionRate.Convert(GetCtx(),
//                    allocationSource.Value, GetVAB_Currency_ID(),
//                    invoice.GetVAB_Currency_ID(), GetDateAcct(),
//                    invoice.GetVAB_CurrencyType_ID(), invoice.GetVAF_Client_ID(), invoice.GetVAF_Org_ID());
//                if (allocationSourceNew == null)
//                {
//                    return "Gain/Loss - No Conversion from Allocation->Invoice";
//                }
//                String d2 = "Allocation=(" + GetVAB_Currency_ID() + ")" + allocationSource
//                    + "->(" + invoice.GetVAB_Currency_ID() + ")" + allocationSourceNew;
//                log.Fine(d2);
//                description += " - " + d2;
//                allocationSource = allocationSourceNew;
//            }

//            Decimal? acctDifference = null;	//	gain is negative
//            //	Full Payment in currency
//            if (allocationSource.Value.CompareTo(invoiceSource.Value) == 0)
//            {
//                acctDifference = Decimal.Subtract(invoiceAccounted.Value, allocationAccounted.Value);	//	gain is negative
//                String d2 = "(full) = " + acctDifference;
//                log.Fine(d2);
//                description += " - " + d2;
//            }
//            else	//	partial or MC
//            {
//                //	percent of total payment
//                double multiplier = Convert.ToDouble(allocationSource) / Convert.ToDouble(invoiceSource);
//                //	Reduce Orig Invoice Accounted
//                invoiceAccounted = Decimal.Multiply(invoiceAccounted.Value, Convert.ToDecimal(multiplier));
//                //	Difference based on percentage of Orig Invoice
//                acctDifference = Decimal.Subtract(invoiceAccounted.Value, allocationAccounted.Value);	//	gain is negative
//                //	ignore Tolerance
//                if (Math.Abs(acctDifference.Value).CompareTo(TOLERANCE) < 0)
//                {
//                    acctDifference = Env.ZERO;
//                }
//                //	Round
//                int precision = as1.GetStdPrecision();
//                if (Env.Scale(acctDifference.Value) > precision)
//                {
//                    acctDifference = Decimal.Round(acctDifference.Value, precision, MidpointRounding.AwayFromZero);
//                }
//                String d2 = "(partial) = " + acctDifference + " - Multiplier=" + multiplier;
//                log.Fine(d2);
//                description += " - " + d2;
//            }

//            if (Env.Signum(acctDifference.Value) == 0)
//            {
//                log.Fine("No Difference");
//                return null;
//            }

//            MAccount gain = MAccount.Get(as1.GetCtx(), as1.GetAcctSchemaDefault().GetRealizedGain_Acct());
//            MAccount loss = MAccount.Get(as1.GetCtx(), as1.GetAcctSchemaDefault().GetRealizedLoss_Acct());
//            //
//            if (invoice.IsSOTrx())
//            {
//                FactLine fl = fact.CreateLine(null, loss, gain,
//                    as1.GetVAB_Currency_ID(), acctDifference);
//                fl.SetDescription(description);
//                fact.CreateLine(null, acct,
//                    as1.GetVAB_Currency_ID(), Decimal.Negate(acctDifference.Value));
//                fl.SetDescription(description);
//            }
//            else
//            {
//                fact.CreateLine(null, acct,
//                    as1.GetVAB_Currency_ID(), acctDifference);
//                fact.CreateLine(null, loss, gain,
//                    as1.GetVAB_Currency_ID(), Decimal.Negate(acctDifference.Value));
//            }
//            return null;
//        }


//        /// <summary>
//        /// Create Tax Correction.
//        /// Requirement: Adjust the tax amount, if you did not receive the full
//        /// amount of the invoice (payment discount, write-off).
//        /// Applies to many countries with VAT.
//        /// Example:
//        /// Invoice:	Net $100 + Tax1 $15 + Tax2 $5 = Total $120
//        /// Payment:	$115 (i.e. $5 underpayment)
//        /// Tax Adjustment = Tax1 = 0.63 (15/120*5) Tax2 = 0.21 (5/120/5) 
//        /// </summary>
//        /// <param name="as1"></param>
//        /// <param name="fact"></param>
//        /// <param name="line">Allocation line</param>
//        /// <param name="DiscountAccount">discount acct</param>
//        /// <param name="WriteOffAccoint">write off acct</param>
//        /// <returns>true if created</returns>
//        private bool CreateTaxCorrection(MAcctSchema as1, Fact fact,
//            DocLine_Allocation line,
//            MAccount DiscountAccount, MAccount WriteOffAccoint)
//        {
//            log.Info(line.ToString());
//            Decimal discount = Env.ZERO;
//            if (as1.IsTaxCorrectionDiscount())
//            {
//                discount = line.GetDiscountAmt();
//            }
//            Decimal writeOff = Env.ZERO;
//            if (as1.IsTaxCorrectionWriteOff())
//            {
//                writeOff = line.GetWriteOffAmt();
//            }

//            Doc_AllocationTax tax = new Doc_AllocationTax(
//                DiscountAccount, discount, WriteOffAccoint, writeOff);

//            //	Get Source Amounts with account
//            String sql = "SELECT * "
//                + "FROM Actual_Acct_Detail "
//                + "WHERE VAF_TableView_ID=318 AND Record_ID=" + line.GetVAB_Invoice_ID()	//	Invoice
//                + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID()
//                + " AND Line_ID IS NULL";	//	header lines like tax or total

//            IDataReader idr = null;
//            try
//            {
//                idr = DataBase.DB.ExecuteReader(sql, null, GetTrxName());
//                while (idr.Read())
//                {
//                    tax.AddInvoiceFact(new MFactAcct(GetCtx(), idr, fact.Get_TrxName()));
//                }
//                idr.Close();
//            }
//            catch (Exception e)
//            {
//                if (idr != null)
//                {
//                    idr.Close();
//                    idr = null;
//                }
//                log.Log(Level.SEVERE, sql, e);
//            }

//            //	Invoice Not posted
//            if (tax.GetLineCount() == 0)
//            {
//                log.Warning("Invoice not posted yet - " + line);
//                return false;
//            }
//            //	size = 1 if no tax
//            if (tax.GetLineCount() < 2)
//            {
//                return true;
//            }
//            return tax.CreateEntries(as1, fact, line);

//        }

//    }
//}
#endregion