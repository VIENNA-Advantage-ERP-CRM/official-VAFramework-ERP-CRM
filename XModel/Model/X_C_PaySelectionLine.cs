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
    /** Generated Model for VAB_PaymentOptionLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_PaymentOptionLine : PO
    {
        public X_VAB_PaymentOptionLine(Context ctx, int VAB_PaymentOptionLine_ID, Trx trxName)
            : base(ctx, VAB_PaymentOptionLine_ID, trxName)
        {
            /** if (VAB_PaymentOptionLine_ID == 0)
            {
            SetVAB_Invoice_ID (0);
            SetVAB_PaymentOptionLine_ID (0);
            SetVAB_PaymentOption_ID (0);
            SetDifferenceAmt (0.0);
            SetDiscountAmt (0.0);
            SetIsManual (false);
            SetIsSOTrx (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_PaymentOptionLine WHERE VAB_PaymentOption_ID=@VAB_PaymentOption_ID@
            SetOpenAmt (0.0);
            SetPayAmt (0.0);
            SetPaymentRule (null);	// S
            SetProcessed (false);	// N
            }
             */
        }
        public X_VAB_PaymentOptionLine(Ctx ctx, int VAB_PaymentOptionLine_ID, Trx trxName)
            : base(ctx, VAB_PaymentOptionLine_ID, trxName)
        {
            /** if (VAB_PaymentOptionLine_ID == 0)
            {
            SetVAB_Invoice_ID (0);
            SetVAB_PaymentOptionLine_ID (0);
            SetVAB_PaymentOption_ID (0);
            SetDifferenceAmt (0.0);
            SetDiscountAmt (0.0);
            SetIsManual (false);
            SetIsSOTrx (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_PaymentOptionLine WHERE VAB_PaymentOption_ID=@VAB_PaymentOption_ID@
            SetOpenAmt (0.0);
            SetPayAmt (0.0);
            SetPaymentRule (null);	// S
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_PaymentOptionLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_PaymentOptionLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAB_PaymentOptionLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAB_PaymentOptionLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514373660L;
        /** Last Updated Timestamp 7/29/2010 1:07:36 PM */
        public static long updatedMS = 1280389056871L;
        /** VAF_TableView_ID=427 */
        public static int Table_ID;
        // =427;

        /** TableName=VAB_PaymentOptionLine */
        public static String Table_Name = "VAB_PaymentOptionLine";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(1);
        /** AccessLevel
        @return 1 - Org 
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
            StringBuilder sb = new StringBuilder("X_VAB_PaymentOptionLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Invoice.
        @param VAB_Invoice_ID Invoice Identifier */
        public void SetVAB_Invoice_ID(int VAB_Invoice_ID)
        {
            if (VAB_Invoice_ID < 1) throw new ArgumentException("VAB_Invoice_ID is mandatory.");
            Set_Value("VAB_Invoice_ID", VAB_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetVAB_Invoice_ID()
        {
            Object ii = Get_Value("VAB_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Pay Selection Check.
        @param VAB_PaymentOptionCheck_ID Payment Selection Check */
        public void SetVAB_PaymentOptionCheck_ID(int VAB_PaymentOptionCheck_ID)
        {
            if (VAB_PaymentOptionCheck_ID <= 0) Set_Value("VAB_PaymentOptionCheck_ID", null);
            else
                Set_Value("VAB_PaymentOptionCheck_ID", VAB_PaymentOptionCheck_ID);
        }
        /** Get Pay Selection Check.
        @return Payment Selection Check */
        public int GetVAB_PaymentOptionCheck_ID()
        {
            Object ii = Get_Value("VAB_PaymentOptionCheck_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Selection Line.
        @param VAB_PaymentOptionLine_ID Payment Selection Line */
        public void SetVAB_PaymentOptionLine_ID(int VAB_PaymentOptionLine_ID)
        {
            if (VAB_PaymentOptionLine_ID < 1) throw new ArgumentException("VAB_PaymentOptionLine_ID is mandatory.");
            Set_ValueNoCheck("VAB_PaymentOptionLine_ID", VAB_PaymentOptionLine_ID);
        }
        /** Get Payment Selection Line.
        @return Payment Selection Line */
        public int GetVAB_PaymentOptionLine_ID()
        {
            Object ii = Get_Value("VAB_PaymentOptionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetVAB_PaymentOptionLine_ID().ToString());
        }
        /** Set Payment Selection.
        @param VAB_PaymentOption_ID Payment Selection */
        public void SetVAB_PaymentOption_ID(int VAB_PaymentOption_ID)
        {
            if (VAB_PaymentOption_ID < 1) throw new ArgumentException("VAB_PaymentOption_ID is mandatory.");
            Set_ValueNoCheck("VAB_PaymentOption_ID", VAB_PaymentOption_ID);
        }
        /** Get Payment Selection.
        @return Payment Selection */
        public int GetVAB_PaymentOption_ID()
        {
            Object ii = Get_Value("VAB_PaymentOption_ID");
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
        /** Set Difference.
        @param DifferenceAmt Difference Amount */
        public void SetDifferenceAmt(Decimal? DifferenceAmt)
        {
            if (DifferenceAmt == null) throw new ArgumentException("DifferenceAmt is mandatory.");
            Set_ValueNoCheck("DifferenceAmt", (Decimal?)DifferenceAmt);
        }
        /** Get Difference.
        @return Difference Amount */
        public Decimal GetDifferenceAmt()
        {
            Object bd = Get_Value("DifferenceAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Discount Amount.
        @param DiscountAmt Calculated amount of discount */
        public void SetDiscountAmt(Decimal? DiscountAmt)
        {
            if (DiscountAmt == null) throw new ArgumentException("DiscountAmt is mandatory.");
            Set_ValueNoCheck("DiscountAmt", (Decimal?)DiscountAmt);
        }
        /** Get Discount Amount.
        @return Calculated amount of discount */
        public Decimal GetDiscountAmt()
        {
            Object bd = Get_Value("DiscountAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Manual.
        @param IsManual This is a manual process */
        public void SetIsManual(Boolean IsManual)
        {
            Set_Value("IsManual", IsManual);
        }
        /** Get Manual.
        @return This is a manual process */
        public Boolean IsManual()
        {
            Object oo = Get_Value("IsManual");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sales Transaction.
        @param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx)
        {
            Set_Value("IsSOTrx", IsSOTrx);
        }
        /** Get Sales Transaction.
        @return This is a Sales Transaction */
        public Boolean IsSOTrx()
        {
            Object oo = Get_Value("IsSOTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line No.
        @param Line Unique line for this document */
        public void SetLine(int Line)
        {
            Set_Value("Line", Line);
        }
        /** Get Line No.
        @return Unique line for this document */
        public int GetLine()
        {
            Object ii = Get_Value("Line");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Open Amount.
        @param OpenAmt Open item amount */
        public void SetOpenAmt(Decimal? OpenAmt)
        {
            if (OpenAmt == null) throw new ArgumentException("OpenAmt is mandatory.");
            Set_ValueNoCheck("OpenAmt", (Decimal?)OpenAmt);
        }
        /** Get Open Amount.
        @return Open item amount */
        public Decimal GetOpenAmt()
        {
            Object bd = Get_Value("OpenAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Payment amount.
        @param PayAmt Amount being paid */
        public void SetPayAmt(Decimal? PayAmt)
        {
            if (PayAmt == null) throw new ArgumentException("PayAmt is mandatory.");
            Set_Value("PayAmt", (Decimal?)PayAmt);
        }
        /** Get Payment amount.
        @return Amount being paid */
        public Decimal GetPayAmt()
        {
            Object bd = Get_Value("PayAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Invoice Payment Schedule.
        @param VAB_sched_InvoicePayment_ID Invoice Payment Schedule */
        public void SetVAB_sched_InvoicePayment_ID(int VAB_sched_InvoicePayment_ID)
        {
            if (VAB_sched_InvoicePayment_ID < 1) throw new ArgumentException("VAB_sched_InvoicePayment_ID is mandatory.");
            Set_ValueNoCheck("VAB_sched_InvoicePayment_ID", VAB_sched_InvoicePayment_ID);
        }
        /** Get Invoice Payment Schedule.
        @return Invoice Payment Schedule */
        public int GetVAB_sched_InvoicePayment_ID()
        {
            Object ii = Get_Value("VAB_sched_InvoicePayment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
