namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for C_BankAccount
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_BankAccount : PO
    {
        public X_C_BankAccount(Context ctx, int C_BankAccount_ID, Trx trxName)
            : base(ctx, C_BankAccount_ID, trxName)
        {
            /** if (C_BankAccount_ID == 0)
            {
            SetAccountNo (null);
            SetBankAccountType (null);
            SetC_BankAccount_ID (0);
            SetC_Bank_ID (0);
            SetC_Currency_ID (0);	// @$C_Currency_ID@
            SetCreditLimit (0.0);
            SetCurrentBalance (0.0);
            SetIsDefault (false);
            SetUnMatchedBalance (0.0);
            }
             */
        }
        public X_C_BankAccount(Ctx ctx, int C_BankAccount_ID, Trx trxName)
            : base(ctx, C_BankAccount_ID, trxName)
        {
            /** if (C_BankAccount_ID == 0)
            {
            SetAccountNo (null);
            SetBankAccountType (null);
            SetC_BankAccount_ID (0);
            SetC_Bank_ID (0);
            SetC_Currency_ID (0);	// @$C_Currency_ID@
            SetCreditLimit (0.0);
            SetCurrentBalance (0.0);
            SetIsDefault (false);
            SetUnMatchedBalance (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BankAccount(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BankAccount(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BankAccount(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_BankAccount()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721596534964L;
        /** Last Updated Timestamp 8/13/2015 6:36:58 PM */
        public static long updatedMS = 1439471218175L;
        /** AD_Table_ID=297 */
        public static int Table_ID;
        // =297;

        /** TableName=C_BankAccount */
        public static String Table_Name = "C_BankAccount";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(3);
        /** AccessLevel
        @return 3 - Client - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_C_BankAccount[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Account No.
        @param AccountNo Account Number */
        public void SetAccountNo(String AccountNo)
        {
            if (AccountNo == null) throw new ArgumentException("AccountNo is mandatory.");
            if (AccountNo.Length > 250)
            {
                log.Warning("Length > 250 - truncated");
                AccountNo = AccountNo.Substring(0, 250);
            }
            Set_Value("AccountNo", AccountNo);
        }
        /** Get Account No.
        @return Account Number */
        public String GetAccountNo()
        {
            return (String)Get_Value("AccountNo");
        }
        /** Set BBAN.
        @param BBAN Basic Bank Account Number */
        public void SetBBAN(String BBAN)
        {
            if (BBAN != null && BBAN.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                BBAN = BBAN.Substring(0, 40);
            }
            Set_Value("BBAN", BBAN);
        }
        /** Get BBAN.
        @return Basic Bank Account Number */
        public String GetBBAN()
        {
            return (String)Get_Value("BBAN");
        }

        /** BankAccountType AD_Reference_ID=216 */
        public static int BANKACCOUNTTYPE_AD_Reference_ID = 216;
        /** Checking = C */
        public static String BANKACCOUNTTYPE_Checking = "C";
        /** Savings = S */
        public static String BANKACCOUNTTYPE_Savings = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsBankAccountTypeValid(String test)
        {
            return test.Equals("C") || test.Equals("S");
        }
        /** Set Bank Account Type.
        @param BankAccountType Bank Account Type */
        public void SetBankAccountType(String BankAccountType)
        {
            if (BankAccountType == null) throw new ArgumentException("BankAccountType is mandatory");
            if (!IsBankAccountTypeValid(BankAccountType))
                throw new ArgumentException("BankAccountType Invalid value - " + BankAccountType + " - Reference_ID=216 - C - S");
            if (BankAccountType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                BankAccountType = BankAccountType.Substring(0, 1);
            }
            Set_Value("BankAccountType", BankAccountType);
        }
        /** Get Bank Account Type.
        @return Bank Account Type */
        public String GetBankAccountType()
        {
            return (String)Get_Value("BankAccountType");
        }
        /** Set Bank Account.
        @param C_BankAccount_ID Account at the Bank */
        public void SetC_BankAccount_ID(int C_BankAccount_ID)
        {
            if (C_BankAccount_ID < 1) throw new ArgumentException("C_BankAccount_ID is mandatory.");
            Set_ValueNoCheck("C_BankAccount_ID", C_BankAccount_ID);
        }
        /** Get Bank Account.
        @return Account at the Bank */
        public int GetC_BankAccount_ID()
        {
            Object ii = Get_Value("C_BankAccount_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank.
        @param C_Bank_ID Bank */
        public void SetC_Bank_ID(int C_Bank_ID)
        {
            if (C_Bank_ID < 1) throw new ArgumentException("C_Bank_ID is mandatory.");
            Set_ValueNoCheck("C_Bank_ID", C_Bank_ID);
        }
        /** Get Bank.
        @return Bank */
        public int GetC_Bank_ID()
        {
            Object ii = Get_Value("C_Bank_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_Bank_ID().ToString());
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory.");
            Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Credit limit.
        @param CreditLimit Amount of Credit allowed */
        public void SetCreditLimit(Decimal? CreditLimit)
        {
            if (CreditLimit == null) throw new ArgumentException("CreditLimit is mandatory.");
            Set_Value("CreditLimit", (Decimal?)CreditLimit);
        }
        /** Get Credit limit.
        @return Amount of Credit allowed */
        public Decimal GetCreditLimit()
        {
            Object bd = Get_Value("CreditLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Current balance.
        @param CurrentBalance Current Balance */
        public void SetCurrentBalance(Decimal? CurrentBalance)
        {
            if (CurrentBalance == null) throw new ArgumentException("CurrentBalance is mandatory.");
            Set_Value("CurrentBalance", (Decimal?)CurrentBalance);
        }
        /** Get Current balance.
        @return Current Balance */
        public Decimal GetCurrentBalance()
        {
            Object bd = Get_Value("CurrentBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set IBAN.
        @param IBAN International Bank Account Number */
        public void SetIBAN(String IBAN)
        {
            if (IBAN != null && IBAN.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                IBAN = IBAN.Substring(0, 40);
            }
            Set_Value("IBAN", IBAN);
        }
        /** Get IBAN.
        @return International Bank Account Number */
        public String GetIBAN()
        {
            return (String)Get_Value("IBAN");
        }
        /** Set Default.
        @param IsDefault Default value */
        public void SetIsDefault(Boolean IsDefault)
        {
            Set_Value("IsDefault", IsDefault);
        }
        /** Get Default.
        @return Default value */
        public Boolean IsDefault()
        {
            Object oo = Get_Value("IsDefault");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set UnMatched Balance.
        @param UnMatchedBalance UnMatched Balance */
        public void SetUnMatchedBalance(Decimal? UnMatchedBalance)
        {
            if (UnMatchedBalance == null) throw new ArgumentException("UnMatchedBalance is mandatory.");
            Set_Value("UnMatchedBalance", (Decimal?)UnMatchedBalance);
        }
        /** Get UnMatched Balance.
        @return UnMatched Balance */
        public Decimal GetUnMatchedBalance()
        {
            Object bd = Get_Value("UnMatchedBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Check No Auto Controlled.
        @param ChkNoAutoControl Check Number Auto Controlled */
        public void SetChkNoAutoControl(Boolean ChkNoAutoControl) { Set_Value("ChkNoAutoControl", ChkNoAutoControl); }/** Get Check No Auto Controlled.
        @return Check Number Auto Controlled */
        public Boolean IsChkNoAutoControl() { Object oo = Get_Value("ChkNoAutoControl"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
    }

}
