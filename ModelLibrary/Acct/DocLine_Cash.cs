/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocLine_Cash
 * Purpose        : Cash Journal Line
 * Class Used      : DocLine
 * Chronological    Development
 * Raghunandan      21-Jan-2010
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
    /// Cash Journal Line
    /// </summary>
    public class DocLine_Cash : DocLine
    {
        #region

        // Cash Type               
        private String _CashType = "";

        //  AD_Reference_ID=217
        // Charge - C		
        public static String CASHTYPE_CHARGE = "C";
        // Difference - D	
        public static String CASHTYPE_DIFFERENCE = "D";
        // Expense - E	
        public static String CASHTYPE_EXPENSE = "E";
        // Onvoice - I 	
        public static String CASHTYPE_INVOICE = "I";
        // Receipt - R	
        public static String CASHTYPE_RECEIPT = "R";
        // Transfer - T	
        public static String CASHTYPE_TRANSFER = "T";
        // Transfer - T	
        public static String CASHTYPE_BUSINESSPARTNER = "B";
        //CashBook Transfer
        public static string CASHTYPE_CASHBOOKTRANSFER = "A";
        //Cash Recieved From
        public static string CASHTYPE_CASHRECIEVEDFROM = "F";

        //  References
        private int _C_BankAccount_ID = 0;
        private int _C_CashBook_ID = 0;
        private int _C_Invoice_ID = 0;

        //  Amounts
        private Decimal _Amount = Env.ZERO;
        private Decimal _DiscountAmt = Env.ZERO;
        private Decimal _WriteOffAmt = Env.ZERO;
        private int _C_CashLine_Ref_ID = 0;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="line"></param>
        /// <param name="doc"></param>
        public DocLine_Cash(MCashLine line, Doc_Cash doc)
            : base(line, doc)
        {
            _CashType = line.GetCashType();
            _C_BankAccount_ID = line.GetC_BankAccount_ID();
            _C_CashBook_ID = line.GetC_CashBook_ID();
            _C_Invoice_ID = line.GetC_Invoice_ID();
            //
            if (_C_Invoice_ID != 0)
            {
                MInvoice invoice = MInvoice.Get(line.GetCtx(), _C_Invoice_ID);
                SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            }

            //
            _Amount = line.GetAmount();
            _DiscountAmt = line.GetDiscountAmt();
            _WriteOffAmt = line.GetWriteOffAmt();
            _C_CashLine_Ref_ID = line.GetC_CashLine_ID_1();
            SetAmount(_Amount);


        }

        /// <summary>
        /// Get Cash Type
        /// </summary>
        /// <returns>cash type</returns>
        public String GetCashType()
        {
            return _CashType;
        }

        /// <summary>
        /// Get Bank Account
        /// </summary>
        /// <returns>Bank Account</returns>
        public int GetC_BankAccount_ID()
        {
            return _C_BankAccount_ID;
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <returns>C_Invoice_ID</returns>
        public int GetC_Invoice_ID()
        {
            return _C_Invoice_ID;
        }

        /// <summary>
        /// Get Amount
        /// </summary>
        /// <returns>Payment Amount</returns>
        public Decimal GetAmount()
        {
            return _Amount;
        }

        /// <summary>
        /// Get Discount
        /// </summary>
        /// <returns>Discount Amount</returns>
        public Decimal GetDiscountAmt()
        {
            return _DiscountAmt;
        }

        /// <summary>
        /// Get WriteOff
        /// </summary>
        /// <returns>Write-Off Amount</returns>
        public Decimal GetWriteOffAmt()
        {
            return _WriteOffAmt;
        }
        /// <summary>
        /// Get Currency
        /// </summary>
        /// <returns>Currency</returns>
        public int Get_C_CashBook_ID()
        {
            return _C_CashBook_ID;
        }
        public int Get_C_CashLine_Ref_ID()
        {
            return  _C_CashLine_Ref_ID;
        }

    }
}
