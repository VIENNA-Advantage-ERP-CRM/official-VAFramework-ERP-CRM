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
    /** Generated Model for C_AcctSchema_GL
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_AcctSchema_GL : PO
    {
        public X_C_AcctSchema_GL(Context ctx, int C_AcctSchema_GL_ID, Trx trxName)
            : base(ctx, C_AcctSchema_GL_ID, trxName)
        {
            /** if (C_AcctSchema_GL_ID == 0)
            {
            SetC_AcctSchema_ID (0);
            SetCommitmentOffset_Acct (0);
            SetIncomeSummary_Acct (0);
            SetIntercompanyDueFrom_Acct (0);
            SetIntercompanyDueTo_Acct (0);
            SetPPVOffset_Acct (0);
            SetRetainedEarning_Acct (0);
            SetUseCurrencyBalancing (false);
            SetUseSuspenseBalancing (false);
            SetUseSuspenseError (false);
            }
             */
        }
        public X_C_AcctSchema_GL(Ctx ctx, int C_AcctSchema_GL_ID, Trx trxName)
            : base(ctx, C_AcctSchema_GL_ID, trxName)
        {
            /** if (C_AcctSchema_GL_ID == 0)
            {
            SetC_AcctSchema_ID (0);
            SetCommitmentOffset_Acct (0);
            SetIncomeSummary_Acct (0);
            SetIntercompanyDueFrom_Acct (0);
            SetIntercompanyDueTo_Acct (0);
            SetPPVOffset_Acct (0);
            SetRetainedEarning_Acct (0);
            SetUseCurrencyBalancing (false);
            SetUseSuspenseBalancing (false);
            SetUseSuspenseError (false);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema_GL(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema_GL(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema_GL(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_AcctSchema_GL()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514369617L;
        /** Last Updated Timestamp 7/29/2010 1:07:32 PM */
        public static long updatedMS = 1280389052828L;
        /** AD_Table_ID=266 */
        public static int Table_ID;
        // =266;

        /** TableName=C_AcctSchema_GL */
        public static String Table_Name = "C_AcctSchema_GL";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(2);
        /** AccessLevel
        @return 2 - Client 
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
            StringBuilder sb = new StringBuilder("X_C_AcctSchema_GL[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Accounting Schema.
        @param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory.");
            Set_ValueNoCheck("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_AcctSchema_ID().ToString());
        }
        /** Set Commitment Offset.
        @param CommitmentOffset_Acct Budgetary Commitment Offset Account */
        public void SetCommitmentOffset_Acct(int CommitmentOffset_Acct)
        {
            Set_Value("CommitmentOffset_Acct", CommitmentOffset_Acct);
        }
        /** Get Commitment Offset.
        @return Budgetary Commitment Offset Account */
        public int GetCommitmentOffset_Acct()
        {
            Object ii = Get_Value("CommitmentOffset_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency Balancing Acct.
        @param CurrencyBalancing_Acct Account used when a currency is out of balance */
        public void SetCurrencyBalancing_Acct(int CurrencyBalancing_Acct)
        {
            Set_Value("CurrencyBalancing_Acct", CurrencyBalancing_Acct);
        }
        /** Get Currency Balancing Acct.
        @return Account used when a currency is out of balance */
        public int GetCurrencyBalancing_Acct()
        {
            Object ii = Get_Value("CurrencyBalancing_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Income Summary Acct.
        @param IncomeSummary_Acct Income Summary Account */
        public void SetIncomeSummary_Acct(int IncomeSummary_Acct)
        {
            Set_Value("IncomeSummary_Acct", IncomeSummary_Acct);
        }
        /** Get Income Summary Acct.
        @return Income Summary Account */
        public int GetIncomeSummary_Acct()
        {
            Object ii = Get_Value("IncomeSummary_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Intercompany Due From Acct.
        @param IntercompanyDueFrom_Acct Intercompany Due From / Receivables Account */
        public void SetIntercompanyDueFrom_Acct(int IntercompanyDueFrom_Acct)
        {
            Set_Value("IntercompanyDueFrom_Acct", IntercompanyDueFrom_Acct);
        }
        /** Get Intercompany Due From Acct.
        @return Intercompany Due From / Receivables Account */
        public int GetIntercompanyDueFrom_Acct()
        {
            Object ii = Get_Value("IntercompanyDueFrom_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Intercompany Due To Acct.
        @param IntercompanyDueTo_Acct Intercompany Due To / Payable Account */
        public void SetIntercompanyDueTo_Acct(int IntercompanyDueTo_Acct)
        {
            Set_Value("IntercompanyDueTo_Acct", IntercompanyDueTo_Acct);
        }
        /** Get Intercompany Due To Acct.
        @return Intercompany Due To / Payable Account */
        public int GetIntercompanyDueTo_Acct()
        {
            Object ii = Get_Value("IntercompanyDueTo_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set PPV Offset.
        @param PPVOffset_Acct Purchase Price Variance Offset Account */
        public void SetPPVOffset_Acct(int PPVOffset_Acct)
        {
            Set_Value("PPVOffset_Acct", PPVOffset_Acct);
        }
        /** Get PPV Offset.
        @return Purchase Price Variance Offset Account */
        public int GetPPVOffset_Acct()
        {
            Object ii = Get_Value("PPVOffset_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Retained Earning Acct.
        @param RetainedEarning_Acct Retained Earning Acct */
        public void SetRetainedEarning_Acct(int RetainedEarning_Acct)
        {
            Set_Value("RetainedEarning_Acct", RetainedEarning_Acct);
        }
        /** Get Retained Earning Acct.
        @return Retained Earning Acct */
        public int GetRetainedEarning_Acct()
        {
            Object ii = Get_Value("RetainedEarning_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Suspense Balancing Acct.
        @param SuspenseBalancing_Acct Suspense Balancing Acct */
        public void SetSuspenseBalancing_Acct(int SuspenseBalancing_Acct)
        {
            Set_Value("SuspenseBalancing_Acct", SuspenseBalancing_Acct);
        }
        /** Get Suspense Balancing Acct.
        @return Suspense Balancing Acct */
        public int GetSuspenseBalancing_Acct()
        {
            Object ii = Get_Value("SuspenseBalancing_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Suspense Error Acct.
        @param SuspenseError_Acct Suspense Error Acct */
        public void SetSuspenseError_Acct(int SuspenseError_Acct)
        {
            Set_Value("SuspenseError_Acct", SuspenseError_Acct);
        }
        /** Get Suspense Error Acct.
        @return Suspense Error Acct */
        public int GetSuspenseError_Acct()
        {
            Object ii = Get_Value("SuspenseError_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Use Currency Balancing.
        @param UseCurrencyBalancing Use Currency Balancing */
        public void SetUseCurrencyBalancing(Boolean UseCurrencyBalancing)
        {
            Set_Value("UseCurrencyBalancing", UseCurrencyBalancing);
        }
        /** Get Use Currency Balancing.
        @return Use Currency Balancing */
        public Boolean IsUseCurrencyBalancing()
        {
            Object oo = Get_Value("UseCurrencyBalancing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Use Suspense Balancing.
        @param UseSuspenseBalancing Use Suspense Balancing */
        public void SetUseSuspenseBalancing(Boolean UseSuspenseBalancing)
        {
            Set_Value("UseSuspenseBalancing", UseSuspenseBalancing);
        }
        /** Get Use Suspense Balancing.
        @return Use Suspense Balancing */
        public Boolean IsUseSuspenseBalancing()
        {
            Object oo = Get_Value("UseSuspenseBalancing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Use Suspense Error.
        @param UseSuspenseError Use Suspense Error */
        public void SetUseSuspenseError(Boolean UseSuspenseError)
        {
            Set_Value("UseSuspenseError", UseSuspenseError);
        }
        /** Get Use Suspense Error.
        @return Use Suspense Error */
        public Boolean IsUseSuspenseError()
        {
            Object oo = Get_Value("UseSuspenseError");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Realized Gain Acct.
        @param FRPT_RealizedGain_Acct Realized Gain Acct */
        public void SetFRPT_RealizedGain_Acct(int FRPT_RealizedGain_Acct)
        {
            Set_Value("FRPT_RealizedGain_Acct", FRPT_RealizedGain_Acct);
        }

        /** Get Realized Gain Acct.
        @return Realized Gain Acct */
        public int GetFRPT_RealizedGain_Acct()
        {
            Object ii = Get_Value("FRPT_RealizedGain_Acct");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Realized Loss Acct.
        @param FRPT_RealizedLoss_Acct Realized Loss Acct */
        public void SetFRPT_RealizedLoss_Acct(int FRPT_RealizedLoss_Acct)
        {
            Set_Value("FRPT_RealizedLoss_Acct", FRPT_RealizedLoss_Acct);
        }

        /** Get Realized Loss Acct.
        @return Realized Loss Acct */
        public int GetFRPT_RealizedLoss_Acct()
        {
            Object ii = Get_Value("FRPT_RealizedLoss_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
