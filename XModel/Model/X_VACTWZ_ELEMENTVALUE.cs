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
    /** Generated Model for VACTWZ_ELEMENTVALUE
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VACTWZ_ELEMENTVALUE : PO
    {
        public X_VACTWZ_ELEMENTVALUE(Context ctx, int VACTWZ_ELEMENTVALUE_ID, Trx trxName)
            : base(ctx, VACTWZ_ELEMENTVALUE_ID, trxName)
        {
            /** if (VACTWZ_ELEMENTVALUE_ID == 0)
            {
            SetVACTWZ_ELEMENTVALUE_ID (0);
            }
             */
        }
        public X_VACTWZ_ELEMENTVALUE(Ctx ctx, int VACTWZ_ELEMENTVALUE_ID, Trx trxName)
            : base(ctx, VACTWZ_ELEMENTVALUE_ID, trxName)
        {
            /** if (VACTWZ_ELEMENTVALUE_ID == 0)
            {
            SetVACTWZ_ELEMENTVALUE_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VACTWZ_ELEMENTVALUE(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VACTWZ_ELEMENTVALUE(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VACTWZ_ELEMENTVALUE(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VACTWZ_ELEMENTVALUE()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27630248191084L;
        /** Last Updated Timestamp 9/20/2012 12:04:34 PM */
        public static long updatedMS = 1348122874295L;
        /** AD_Table_ID=1000378 */
        public static int Table_ID;
        // =1000378;

        /** TableName=VACTWZ_ELEMENTVALUE */
        public static String Table_Name = "VACTWZ_ELEMENTVALUE";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(7);
        /** AccessLevel
        @return 7 - System - Client - Org 
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
            StringBuilder sb = new StringBuilder("X_VACTWZ_ELEMENTVALUE[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsAccountSignValid(String test)
        {
            return test.Equals("C") || test.Equals("D") || test.Equals("N");
        }
        /** Set Account Sign.
        @param AccountSign Indicates the Natural Sign of the Account as a Debit or Credit */
        public void SetAccountSign(String AccountSign)
        {
            if (AccountSign == null) throw new ArgumentException("AccountSign is mandatory");
            if (!IsAccountSignValid(AccountSign))
                throw new ArgumentException("AccountSign Invalid value - " + AccountSign + " - Reference_ID=118 - C - D - N");
            if (AccountSign.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                AccountSign = AccountSign.Substring(0, 1);
            }
            Set_Value("AccountSign", AccountSign);
        }
        /** Get Account Sign.
        @return Indicates the Natural Sign of the Account as a Debit or Credit */
        public String GetAccountSign()
        {
            return (String)Get_Value("AccountSign");
        }
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsAccountTypeValid(String test)
        {
            return test.Equals("A") || test.Equals("E") || test.Equals("L") || test.Equals("M") || test.Equals("O") || test.Equals("R");
        }
        /** Set Account Type.
        @param AccountType Indicates the type of account */
        public void SetAccountType(String AccountType)
        {
            if (AccountType == null) throw new ArgumentException("AccountType is mandatory");
            if (!IsAccountTypeValid(AccountType))
                throw new ArgumentException("AccountType Invalid value - " + AccountType + " - Reference_ID=117 - A - E - L - M - O - R");
            if (AccountType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                AccountType = AccountType.Substring(0, 1);
            }
            Set_Value("AccountType", AccountType);
        }
        /** Get Account Type.
        @return Indicates the type of account */
        public String GetAccountType()
        {
            return (String)Get_Value("AccountType");
        }
        /** Set C_AccountGroup_ID.
        @param C_AccountGroup_ID C_AccountGroup_ID */
        public void SetC_AccountGroup_ID(int C_AccountGroup_ID)
        {
            if (C_AccountGroup_ID <= 0) Set_Value("C_AccountGroup_ID", null);
            else
                Set_Value("C_AccountGroup_ID", C_AccountGroup_ID);
        }
        /** Get C_AccountGroup_ID.
        @return C_AccountGroup_ID */
        public int GetC_AccountGroup_ID()
        {
            Object ii = Get_Value("C_AccountGroup_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set C_AccountSubGroup_ID.
        @param C_AccountSubGroup_ID C_AccountSubGroup_ID */
        public void SetC_AccountSubGroup_ID(int C_AccountSubGroup_ID)
        {
            if (C_AccountSubGroup_ID <= 0) Set_Value("C_AccountSubGroup_ID", null);
            else
                Set_Value("C_AccountSubGroup_ID", C_AccountSubGroup_ID);
        }
        /** Get C_AccountSubGroup_ID.
        @return C_AccountSubGroup_ID */
        public int GetC_AccountSubGroup_ID()
        {
            Object ii = Get_Value("C_AccountSubGroup_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Account.
        @param C_BankAccount_ID Account at the Bank */
        public void SetC_BankAccount_ID(int C_BankAccount_ID)
        {
            if (C_BankAccount_ID <= 0) Set_Value("C_BankAccount_ID", null);
            else
                Set_Value("C_BankAccount_ID", C_BankAccount_ID);
        }
        /** Get Bank Account.
        @return Account at the Bank */
        public int GetC_BankAccount_ID()
        {
            Object ii = Get_Value("C_BankAccount_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
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
        /** Set Element.
        @param C_Element_ID Accounting Element */
        public void SetC_Element_ID(int C_Element_ID)
        {
            if (C_Element_ID <= 0) Set_Value("C_Element_ID", null);
            else
                Set_Value("C_Element_ID", C_Element_ID);
        }
        /** Get Element.
        @return Accounting Element */
        public int GetC_Element_ID()
        {
            Object ii = Get_Value("C_Element_ID");
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
        /** Set Has Group.
        @param HasGroup Has Group */
        public void SetHasGroup(Boolean HasGroup)
        {
            Set_Value("HasGroup", HasGroup);
        }
        /** Get Has Group.
        @return Has Group */
        public Boolean IsHasGroup()
        {
            Object oo = Get_Value("HasGroup");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Bank Account.
        @param IsBankAccount Indicates if this is the Bank Account */
        public void SetIsBankAccount(Boolean IsBankAccount)
        {
            Set_Value("IsBankAccount", IsBankAccount);
        }
        /** Get Bank Account.
        @return Indicates if this is the Bank Account */
        public Boolean IsBankAccount()
        {
            Object oo = Get_Value("IsBankAccount");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Document Controlled.
        @param IsDocControlled Control account - If an account is controlled by a document, you cannot post manually to it */
        public void SetIsDocControlled(Boolean IsDocControlled)
        {
            Set_Value("IsDocControlled", IsDocControlled);
        }
        /** Get Document Controlled.
        @return Control account - If an account is controlled by a document, you cannot post manually to it */
        public Boolean IsDocControlled()
        {
            Object oo = Get_Value("IsDocControlled");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Foreign Currency Account.
        @param IsForeignCurrency Balances in foreign currency accounts are held in the nominated currency */
        public void SetIsForeignCurrency(Boolean IsForeignCurrency)
        {
            Set_Value("IsForeignCurrency", IsForeignCurrency);
        }
        /** Get Foreign Currency Account.
        @return Balances in foreign currency accounts are held in the nominated currency */
        public Boolean IsForeignCurrency()
        {
            Object oo = Get_Value("IsForeignCurrency");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Summary Level.
        @param IsSummary This is a summary entity */
        public void SetIsSummary(Boolean IsSummary)
        {
            Set_Value("IsSummary", IsSummary);
        }
        /** Get Summary Level.
        @return This is a summary entity */
        public Boolean IsSummary()
        {
            Object oo = Get_Value("IsSummary");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Master Account Type.
        @param MasterAccountType Master Account Type */
        public void SetMasterAccountType(String MasterAccountType)
        {
            if (MasterAccountType != null && MasterAccountType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                MasterAccountType = MasterAccountType.Substring(0, 2);
            }
            Set_Value("MasterAccountType", MasterAccountType);
        }
        /** Get Master Account Type.
        @return Master Account Type */
        public String GetMasterAccountType()
        {
            return (String)Get_Value("MasterAccountType");
        }
        /** Set Name.
        @param Name Name */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                Name = Name.Substring(0, 120);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Name */
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
        /** Set Post Actual.
        @param PostActual Actual Values can be posted */
        public void SetPostActual(Boolean PostActual)
        {
            Set_Value("PostActual", PostActual);
        }
        /** Get Post Actual.
        @return Actual Values can be posted */
        public Boolean IsPostActual()
        {
            Object oo = Get_Value("PostActual");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Post Budget.
        @param PostBudget Budget values can be posted */
        public void SetPostBudget(Boolean PostBudget)
        {
            Set_Value("PostBudget", PostBudget);
        }
        /** Get Post Budget.
        @return Budget values can be posted */
        public Boolean IsPostBudget()
        {
            Object oo = Get_Value("PostBudget");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Post Encumbrance.
        @param PostEncumbrance Post commitments to this account */
        public void SetPostEncumbrance(Boolean PostEncumbrance)
        {
            Set_Value("PostEncumbrance", PostEncumbrance);
        }
        /** Get Post Encumbrance.
        @return Post commitments to this account */
        public Boolean IsPostEncumbrance()
        {
            Object oo = Get_Value("PostEncumbrance");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Post Statistical.
        @param PostStatistical Post statistical quantities to this account? */
        public void SetPostStatistical(Boolean PostStatistical)
        {
            Set_Value("PostStatistical", PostStatistical);
        }
        /** Get Post Statistical.
        @return Post statistical quantities to this account? */
        public Boolean IsPostStatistical()
        {
            Object oo = Get_Value("PostStatistical");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set MPG Primary Group.
        @param Ref_C_AccountGroup_ID MPG Primary Group */
        public void SetRef_C_AccountGroup_ID(int Ref_C_AccountGroup_ID)
        {
            if (Ref_C_AccountGroup_ID <= 0) Set_Value("Ref_C_AccountGroup_ID", null);
            else
                Set_Value("Ref_C_AccountGroup_ID", Ref_C_AccountGroup_ID);
        }
        /** Get MPG Primary Group.
        @return MPG Primary Group */
        public int GetRef_C_AccountGroup_ID()
        {
            Object ii = Get_Value("Ref_C_AccountGroup_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set MPG Sub Group.
        @param Ref_C_AccountSubGroup_ID MPG Sub Group */
        public void SetRef_C_AccountSubGroup_ID(int Ref_C_AccountSubGroup_ID)
        {
            if (Ref_C_AccountSubGroup_ID <= 0) Set_Value("Ref_C_AccountSubGroup_ID", null);
            else
                Set_Value("Ref_C_AccountSubGroup_ID", Ref_C_AccountSubGroup_ID);
        }
        /** Get MPG Sub Group.
        @return MPG Sub Group */
        public int GetRef_C_AccountSubGroup_ID()
        {
            Object ii = Get_Value("Ref_C_AccountSubGroup_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Element Value.
        @param Ref_C_ElementValue_ID Element Value */
        public void SetRef_C_ElementValue_ID(int Ref_C_ElementValue_ID)
        {
            if (Ref_C_ElementValue_ID <= 0) Set_Value("Ref_C_ElementValue_ID", null);
            else
                Set_Value("Ref_C_ElementValue_ID", Ref_C_ElementValue_ID);
        }
        /** Get Element Value.
        @return Element Value */
        public int GetRef_C_ElementValue_ID()
        {
            Object ii = Get_Value("Ref_C_ElementValue_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VACTWZ_ELEMENTVALUE_ID.
        @param VACTWZ_ELEMENTVALUE_ID VACTWZ_ELEMENTVALUE_ID */
        public void SetVACTWZ_ELEMENTVALUE_ID(int VACTWZ_ELEMENTVALUE_ID)
        {
            if (VACTWZ_ELEMENTVALUE_ID < 1) throw new ArgumentException("VACTWZ_ELEMENTVALUE_ID is mandatory.");
            Set_ValueNoCheck("VACTWZ_ELEMENTVALUE_ID", VACTWZ_ELEMENTVALUE_ID);
        }
        /** Get VACTWZ_ELEMENTVALUE_ID.
        @return VACTWZ_ELEMENTVALUE_ID */
        public int GetVACTWZ_ELEMENTVALUE_ID()
        {
            Object ii = Get_Value("VACTWZ_ELEMENTVALUE_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Valid from.
        @param ValidFrom Valid from including this date (first day) */
        public void SetValidFrom(DateTime? ValidFrom)
        {
            Set_ValueNoCheck("ValidFrom", (DateTime?)ValidFrom);
        }
        /** Get Valid from.
        @return Valid from including this date (first day) */
        public DateTime? GetValidFrom()
        {
            return (DateTime?)Get_Value("ValidFrom");
        }
        /** Set Valid to.
        @param ValidTo Valid to including this date (last day) */
        public void SetValidTo(DateTime? ValidTo)
        {
            Set_ValueNoCheck("ValidTo", (DateTime?)ValidTo);
        }
        /** Get Valid to.
        @return Valid to including this date (last day) */
        public DateTime? GetValidTo()
        {
            return (DateTime?)Get_Value("ValidTo");
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value != null && Value.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Value = Value.Substring(0, 40);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }
    }

}
