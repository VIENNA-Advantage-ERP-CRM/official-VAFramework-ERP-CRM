/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_AllocationTax
 * Purpose        : Allocation Document Tax Handing
 * Class Used     : none
 * Chronological    Development
 * Raghunandan      20-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
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
    public class Doc_AllocationTax
    {
        #region Private Variables

        private VLogger log = null;

        private MAccount _DiscountAccount;
        private Decimal _DiscountAmt;
        private MAccount _WriteOffAccount;
        private Decimal _WriteOffAmt;

        private List<MFactAcct> _facts = new List<MFactAcct>();
        private int _totalIndex = 0;
        #endregion


        /// <summary>
        /// Allocation Tax Adjustmentdiscount acct
        /// </summary>
        /// <param name="DiscountAccount">DiscountAccount </param>
        /// <param name="DiscountAmt">discount amt</param>
        /// <param name="WriteOffAccount">write off acct</param>
        /// <param name="WriteOffAmt">write off amt</param>
        public Doc_AllocationTax(MAccount DiscountAccount, Decimal DiscountAmt, MAccount WriteOffAccount, Decimal WriteOffAmt)
        {

            log = VLogger.GetVLogger(this.GetType().FullName);//getClass());
            _DiscountAccount = DiscountAccount;
            _DiscountAmt = DiscountAmt;
            _WriteOffAccount = WriteOffAccount;
            _WriteOffAmt = WriteOffAmt;
        }


        /// <summary>
        /// Add Invoice Fact Line
        /// </summary>
        /// <param name="fact">fact line</param>
        public void AddInvoiceFact(MFactAcct fact)
        {
            _facts.Add(fact);
        }

        /// <summary>
        /// Get Line Count
        /// </summary>
        /// <returns>number of lines</returns>
        public int GetLineCount()
        {
            return _facts.Count;
        }

        /// <summary>
        /// Create Accounting Entries
        /// </summary>
        /// <param name="as1">account schema</param>
        /// <param name="fact">fact to add lines</param>
        /// <param name="line">line</param>
        /// <returns>true if created</returns>
        public bool CreateEntries(MAcctSchema as1, Fact fact, DocLine line)
        {
            //	get total index (the Receivables/Liabilities line)
            Decimal total = Env.ZERO;
            for (int i = 0; i < _facts.Count; i++)
            {
                MFactAcct factAcct = (MFactAcct)_facts[i];
                if (factAcct.GetAmtSourceDr().CompareTo(total) > 0)
                {
                    total = factAcct.GetAmtSourceDr();
                    _totalIndex = i;
                }
                if (factAcct.GetAmtSourceCr().CompareTo(total) > 0)
                {
                    total = factAcct.GetAmtSourceCr();
                    _totalIndex = i;
                }
            }

            MFactAcct factAcct1 = (MFactAcct)_facts[_totalIndex];
            log.Info("Total Invoice = " + total + " - " + factAcct1);
            int precision = as1.GetStdPrecision();
            for (int i = 0; i < _facts.Count; i++)
            {
                //	No Tax Line
                if (i == _totalIndex)
                {
                    continue;
                }

                factAcct1 = (MFactAcct)_facts[i];
                log.Info(i + ": " + factAcct1);

                //	Create Tax Account
                MAccount taxAcct = factAcct1.GetMAccount();
                if (taxAcct == null || taxAcct.Get_ID() == 0)
                {
                    log.Severe("Tax Account not found/created");
                    return false;
                }


                //	Discount Amount	
                if (Env.Signum(_DiscountAmt) != 0)
                {
                    //	Original Tax is DR - need to correct it CR
                    if (Env.ZERO.CompareTo(factAcct1.GetAmtSourceDr()) != 0)
                    {
                        Decimal amount = CalcAmount(factAcct1.GetAmtSourceDr(),
                            total, _DiscountAmt, precision);
                        if (Env.Signum(amount) != 0)
                        {
                            fact.CreateLine(line, _DiscountAccount,
                                as1.GetC_Currency_ID(), amount, null);
                            fact.CreateLine(line, taxAcct,
                                as1.GetC_Currency_ID(), null, amount);
                        }
                    }
                    //	Original Tax is CR - need to correct it DR
                    else
                    {
                        Decimal amount = CalcAmount(factAcct1.GetAmtSourceCr(),
                            total, _DiscountAmt, precision);
                        if (Env.Signum(amount) != 0)
                        {
                            fact.CreateLine(line, taxAcct,
                                as1.GetC_Currency_ID(), amount, null);
                            fact.CreateLine(line, _DiscountAccount,
                                as1.GetC_Currency_ID(), null, amount);
                        }
                    }
                }	//	Discount

                //	WriteOff Amount	
                if (Env.Signum(_WriteOffAmt) != 0)
                {
                    //	Original Tax is DR - need to correct it CR
                    if (Env.ZERO.CompareTo(factAcct1.GetAmtSourceDr()) != 0)
                    {
                        Decimal amount = CalcAmount(factAcct1.GetAmtSourceDr(),
                            total, _WriteOffAmt, precision);
                        if (Env.Signum(amount) != 0)
                        {
                            fact.CreateLine(line, _WriteOffAccount,
                                as1.GetC_Currency_ID(), amount, null);
                            fact.CreateLine(line, taxAcct,
                                as1.GetC_Currency_ID(), null, amount);
                        }
                    }
                    //	Original Tax is CR - need to correct it DR
                    else
                    {
                        Decimal amount = CalcAmount(factAcct1.GetAmtSourceCr(),
                            total, _WriteOffAmt, precision);
                        if (Env.Signum(amount) != 0)
                        {
                            fact.CreateLine(line, taxAcct,
                                as1.GetC_Currency_ID(), amount, null);
                            fact.CreateLine(line, _WriteOffAccount,
                                as1.GetC_Currency_ID(), null, amount);
                        }
                    }
                }	//	WriteOff

            }	//	for all lines
            return true;
        }
        /// <summary>
        /// Calc Amount tax / (total-tax) * amt
        /// </summary>
        /// <param name="tax"> tax</param>
        /// <param name="total">total</param>
        /// <param name="amt">reduction amt</param>
        /// <param name="precision">precision</param>
        /// <returns>tax / total * amt</returns>
        private Decimal CalcAmount(Decimal tax, Decimal total, Decimal amt, int precision)
        {
            log.Fine("Amt=" + amt + " - Total=" + total + ", Tax=" + tax);
            if (Env.Signum(tax) == 0
                || Env.Signum(total) == 0
                || Env.Signum(amt) == 0)
            {
                return Env.ZERO;
            }
            //
            Decimal devisor = Decimal.Subtract(total, tax);
            Decimal multiplier = Decimal.Round(Decimal.Divide(tax, devisor), 10, MidpointRounding.AwayFromZero);
            Decimal retValue = Decimal.Multiply(multiplier, amt);
            if (Env.Scale(retValue) > precision)
            {
                retValue = Decimal.Round(retValue, precision, MidpointRounding.AwayFromZero);
            }
            log.Fine(retValue + " (Mult=" + multiplier + "(Prec=" + precision + ")");
            return retValue;
        }
    }
}
