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
    /** Generated Model for VAB_Bank_AcctDoc
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_Bank_AcctDoc : PO
    {
        public X_VAB_Bank_AcctDoc(Context ctx, int VAB_Bank_AcctDoc_ID, Trx trxName)
            : base(ctx, VAB_Bank_AcctDoc_ID, trxName)
        {
            /** if (VAB_Bank_AcctDoc_ID == 0)
            {
            SetVAB_Bank_AcctDoc_ID (0);
            SetVAB_Bank_Acct_ID (0);
            SetCurrentNext (0);
            SetName (null);
            SetPaymentRule (null);
            }
             */
        }
        public X_VAB_Bank_AcctDoc(Ctx ctx, int VAB_Bank_AcctDoc_ID, Trx trxName)
            : base(ctx, VAB_Bank_AcctDoc_ID, trxName)
        {
            /** if (VAB_Bank_AcctDoc_ID == 0)
            {
            SetVAB_Bank_AcctDoc_ID (0);
            SetVAB_Bank_Acct_ID (0);
            SetCurrentNext (0);
            SetName (null);
            SetPaymentRule (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Bank_AcctDoc(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Bank_AcctDoc(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_Bank_AcctDoc(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAB_Bank_AcctDoc()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514370636L;
        /** Last Updated Timestamp 7/29/2010 1:07:33 PM */
        public static long updatedMS = 1280389053847L;
        /** VAF_TableView_ID=455 */
        public static int Table_ID;
        // =455;

        /** TableName=VAB_Bank_AcctDoc */
        public static String Table_Name = "VAB_Bank_AcctDoc";

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
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
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
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAB_Bank_AcctDoc[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Bank Account Document.
        @param VAB_Bank_AcctDoc_ID Checks, Transfers, etc. */
        public void SetVAB_Bank_AcctDoc_ID(int VAB_Bank_AcctDoc_ID)
        {
            if (VAB_Bank_AcctDoc_ID < 1) throw new ArgumentException("VAB_Bank_AcctDoc_ID is mandatory.");
            Set_ValueNoCheck("VAB_Bank_AcctDoc_ID", VAB_Bank_AcctDoc_ID);
        }
        /** Get Bank Account Document.
        @return Checks, Transfers, etc. */
        public int GetVAB_Bank_AcctDoc_ID()
        {
            Object ii = Get_Value("VAB_Bank_AcctDoc_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Account.
        @param VAB_Bank_Acct_ID Account at the Bank */
        public void SetVAB_Bank_Acct_ID(int VAB_Bank_Acct_ID)
        {
            if (VAB_Bank_Acct_ID < 1) throw new ArgumentException("VAB_Bank_Acct_ID is mandatory.");
            Set_ValueNoCheck("VAB_Bank_Acct_ID", VAB_Bank_Acct_ID);
        }
        /** Get Bank Account.
        @return Account at the Bank */
        public int GetVAB_Bank_Acct_ID()
        {
            Object ii = Get_Value("VAB_Bank_Acct_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Check_PrintFormat_ID VAF_Control_Ref_ID=268 */
        public static int CHECK_PRINTFORMAT_ID_VAF_Control_Ref_ID = 268;
        /** Set Check Print Format.
        @param Check_PrintFormat_ID Print Format for printing Checks */
        public void SetCheck_PrintFormat_ID(int Check_PrintFormat_ID)
        {
            if (Check_PrintFormat_ID <= 0) Set_Value("Check_PrintFormat_ID", null);
            else
                Set_Value("Check_PrintFormat_ID", Check_PrintFormat_ID);
        }
        /** Get Check Print Format.
        @return Print Format for printing Checks */
        public int GetCheck_PrintFormat_ID()
        {
            Object ii = Get_Value("Check_PrintFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Current Next.
        @param CurrentNext The next number to be used */
        public void SetCurrentNext(int CurrentNext)
        {
            Set_Value("CurrentNext", CurrentNext);
        }
        /** Get Current Next.
        @return The next number to be used */
        public int GetCurrentNext()
        {
            Object ii = Get_Value("CurrentNext");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetName());
        }

        /** PaymentRule VAF_Control_Ref_ID=195 */
        public static int PAYMENTRULE_VAF_Control_Ref_ID = 195;
        /** Cash = B */
        public static String PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        public static String PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        public static String PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        public static String PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        public static String PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        public static String PAYMENTRULE_DirectDeposit = "T";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPaymentRuleValid(String test)
        {
            return test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("P") || test.Equals("S") || test.Equals("T");
        }
        /** Set Payment Method.
        @param PaymentRule How you pay the invoice */
        public void SetPaymentRule(String PaymentRule)
        {
            if (PaymentRule == null) throw new ArgumentException("PaymentRule is mandatory");
            //if (!IsPaymentRuleValid(PaymentRule))
            //throw new ArgumentException ("PaymentRule Invalid value - " + PaymentRule + " - Reference_ID=195 - B - D - K - P - S - T");
            if (PaymentRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PaymentRule = PaymentRule.Substring(0, 1);
            }
            Set_Value("PaymentRule", PaymentRule);
        }
        /** Get Payment Method.
        @return How you pay the invoice */
        public String GetPaymentRule()
        {
            return (String)Get_Value("PaymentRule");
        }

        /** Set Start Check No..
@param StartChkNumber Denotes Start Check No. */
        public void SetStartChkNumber(int StartChkNumber) { Set_Value("StartChkNumber", StartChkNumber); }/** Get Start Check No..
@return Denotes Start Check No. */
        public int GetStartChkNumber() { Object ii = Get_Value("StartChkNumber"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Set End Check No..
@param EndChkNumber Denotes End Check No. */
        public void SetEndChkNumber(int EndChkNumber) { Set_Value("EndChkNumber", EndChkNumber); }/** Get End Check No..
@return Denotes End Check No. */
        public int GetEndChkNumber() { Object ii = Get_Value("EndChkNumber"); if (ii == null) return 0; return Convert.ToInt32(ii); }


        /** Set Priority.
        @param Priority Indicates if this request is of a high, medium or low priority. */
        public void SetPriority(int Priority) { Set_Value("Priority", Priority); }/** Get Priority.
        @return Indicates if this request is of a high, medium or low priority. */
        public int GetPriority() { Object ii = Get_Value("Priority"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}
