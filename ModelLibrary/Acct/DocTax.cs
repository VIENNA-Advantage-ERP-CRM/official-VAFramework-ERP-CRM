/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocTax
 * Purpose        : Document Tax Line
 * Class Used     : none
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
    /// <summary>
    /// Document Tax Line
    /// </summary>
    public class DocTax
    {
        #region Private variables

        // Tax ID           
        private int _C_Tax_ID = 0;
        // Amount           
        private Decimal? _amount = null;
        //Tax Rate          
        private Decimal? _rate = null;
        // Name             
        private String _name = null;
        //Base Tax Amt      
        private Decimal? _taxBaseAmt = null;
        // Included Tax		
        private Decimal _includedTax = Env.ZERO;
        // Sales Tax		
        private bool _salesTax = false;
        //	Logger			
        private static VLogger log = VLogger.GetVLogger(typeof(DocTax));
        // Tax Due Acct      
        public static int ACCTTYPE_TaxDue = 0;
        //Tax Liability      
        public static int ACCTTYPE_TaxLiability = 1;
        // Tax Credit        
        public static int ACCTTYPE_TaxCredit = 2;
        // Tax Receivables   
        public static int ACCTTYPE_TaxReceivables = 3;
        //Tax Expense        
        public static int ACCTTYPE_TaxExpense = 4;
        #endregion

        /// <summary>
        /// Create Tax
        /// </summary>
        /// <param name="C_Tax_ID">tax</param>
        /// <param name="name">name</param>
        /// <param name="rate">rate</param>
        /// <param name="taxBaseAmt">tax base amount</param>
        /// <param name="amount">amount</param>
        /// <param name="salesTax">sales tax flag</param>
        public DocTax(int C_Tax_ID, String name, Decimal rate,
            Decimal taxBaseAmt, Decimal amount, bool salesTax)
        {
            _C_Tax_ID = C_Tax_ID;
            _name = name;
            _rate = rate;
            _amount = amount;
            _salesTax = salesTax;
        }

        /// <summary>
        /// Get Account
        /// </summary>
        /// <param name="AcctType"></param>
        /// <param name="as1"></param>
        /// <returns>Account</returns>
        public MAccount GetAccount(int AcctType, MAcctSchema as1)
        {
            if (AcctType < 0 || AcctType > 4)
            {
                return null;
            }
            //
            String sql = "SELECT T_Due_Acct, T_Liability_Acct, T_Credit_Acct, T_Receivables_Acct, T_Expense_Acct "
                + "FROM C_Tax_Acct WHERE C_Tax_ID=" + _C_Tax_ID + " AND C_AcctSchema_ID=" + as1.GetC_AcctSchema_ID();
            int validCombination_ID = 0;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    validCombination_ID = Utility.Util.GetValueOfInt(idr[AcctType]);// .getInt(AcctType + 1);    //  1..5
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
            if (validCombination_ID == 0)
            {
                return null;
            }
            return MAccount.Get(as1.GetCtx(), validCombination_ID);
        }

        /// <summary>
        /// Get Amount
        /// </summary>
        /// <returns>gross amount</returns>
        public Decimal GetAmount()
        {
            return _amount.Value;
        }

        /// <summary>
        ///   Get Base Amount
        /// </summary>
        /// <returns>net amount</returns>
        public Decimal GetTaxBaseAmt()
        {
            return _taxBaseAmt.Value;
        }

        /// <summary>
        /// Get Rate
        /// </summary>
        /// <returns>tax rate in percent</returns>
        public Decimal GetRate()
        {
            return _rate.Value;
        }

        /// <summary>
        /// Get Name of Tax
        /// </summary>
        /// <returns>name</returns>
        public String GetName()
        {
            return _name;
        }

        /// <summary>
        /// Get C_Tax_ID
        /// </summary>
        /// <returns>tax id</returns>
        public int GetC_Tax_ID()
        {
            return _C_Tax_ID;
        }

        /// <summary>
        /// Get Description (Tax Name and Base Amount)
        /// </summary>
        /// <returns>tax name and base amount</returns>
        public String GetDescription()
        {
            return _name + " " + _taxBaseAmt.ToString();
        }

        /// <summary>
        /// Add to Included Tax
        /// </summary>
        /// <param name="amt">amt amount</param>
        public void AddIncludedTax(Decimal amt)
        {
            _includedTax = Decimal.Add(_includedTax, amt);
        }

        /// <summary>
        /// Get Included Tax
        /// </summary>
        /// <returns>tax amount</returns>
        public Decimal GetIncludedTax()
        {
            return _includedTax;
        }

        /// <summary>
        /// Get Included Tax Difference
        /// </summary>
        /// <returns>tax amount - included amount</returns>
        public Decimal GetIncludedTaxDifference()
        {
            return Decimal.Subtract(_amount.Value, _includedTax);
        }

        /// <summary>
        /// Included Tax differs from tax amount
        /// </summary>
        /// <returns>true if difference</returns>
        public bool IsIncludedTaxDifference()
        {
            return Env.ZERO.CompareTo(GetIncludedTaxDifference()) != 0;
        }

        /// <summary>
        /// Get AP Tax Type
        /// </summary>
        /// <returns>AP tax type (Credit or Expense)</returns>
        public int GetAPTaxType()
        {
            if (IsSalesTax())
            {
                return ACCTTYPE_TaxExpense;
            }
            return ACCTTYPE_TaxCredit;
        }

        /// <summary>
        /// Is Sales Tax
        /// </summary>
        /// <returns>sales tax</returns>
        public bool IsSalesTax()
        {
            return _salesTax;
        }


        /// <summary>
        /// Return String representation
        /// </summary>
        /// <returns>tax anme and base amount</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("Tax=(");
            sb.Append(_name);
            sb.Append(" Amt=").Append(_amount);
            sb.Append(")");
            return sb.ToString();
        }

    }

}
