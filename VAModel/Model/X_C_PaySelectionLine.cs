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
    /** Generated Model for C_PaySelectionLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_PaySelectionLine : PO
    {
        public X_C_PaySelectionLine(Context ctx, int C_PaySelectionLine_ID, Trx trxName)
            : base(ctx, C_PaySelectionLine_ID, trxName)
        {
            /** if (C_PaySelectionLine_ID == 0)
            {
            SetC_Invoice_ID (0);
            SetC_PaySelectionLine_ID (0);
            SetC_PaySelection_ID (0);
            SetDifferenceAmt (0.0);
            SetDiscountAmt (0.0);
            SetIsManual (false);
            SetIsSOTrx (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_PaySelectionLine WHERE C_PaySelection_ID=@C_PaySelection_ID@
            SetOpenAmt (0.0);
            SetPayAmt (0.0);
            SetPaymentRule (null);	// S
            SetProcessed (false);	// N
            }
             */
        }
        public X_C_PaySelectionLine(Ctx ctx, int C_PaySelectionLine_ID, Trx trxName)
            : base(ctx, C_PaySelectionLine_ID, trxName)
        {
            /** if (C_PaySelectionLine_ID == 0)
            {
            SetC_Invoice_ID (0);
            SetC_PaySelectionLine_ID (0);
            SetC_PaySelection_ID (0);
            SetDifferenceAmt (0.0);
            SetDiscountAmt (0.0);
            SetIsManual (false);
            SetIsSOTrx (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_PaySelectionLine WHERE C_PaySelection_ID=@C_PaySelection_ID@
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
        public X_C_PaySelectionLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_PaySelectionLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_PaySelectionLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_PaySelectionLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514373660L;
        /** Last Updated Timestamp 7/29/2010 1:07:36 PM */
        public static long updatedMS = 1280389056871L;
        /** AD_Table_ID=427 */
        public static int Table_ID;
        // =427;

        /** TableName=C_PaySelectionLine */
        public static String Table_Name = "C_PaySelectionLine";

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
            StringBuilder sb = new StringBuilder("X_C_PaySelectionLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID < 1) throw new ArgumentException("C_Invoice_ID is mandatory.");
            Set_Value("C_Invoice_ID", C_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetC_Invoice_ID()
        {
            Object ii = Get_Value("C_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Pay Selection Check.
        @param C_PaySelectionCheck_ID Payment Selection Check */
        public void SetC_PaySelectionCheck_ID(int C_PaySelectionCheck_ID)
        {
            if (C_PaySelectionCheck_ID <= 0) Set_Value("C_PaySelectionCheck_ID", null);
            else
                Set_Value("C_PaySelectionCheck_ID", C_PaySelectionCheck_ID);
        }
        /** Get Pay Selection Check.
        @return Payment Selection Check */
        public int GetC_PaySelectionCheck_ID()
        {
            Object ii = Get_Value("C_PaySelectionCheck_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Selection Line.
        @param C_PaySelectionLine_ID Payment Selection Line */
        public void SetC_PaySelectionLine_ID(int C_PaySelectionLine_ID)
        {
            if (C_PaySelectionLine_ID < 1) throw new ArgumentException("C_PaySelectionLine_ID is mandatory.");
            Set_ValueNoCheck("C_PaySelectionLine_ID", C_PaySelectionLine_ID);
        }
        /** Get Payment Selection Line.
        @return Payment Selection Line */
        public int GetC_PaySelectionLine_ID()
        {
            Object ii = Get_Value("C_PaySelectionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_PaySelectionLine_ID().ToString());
        }
        /** Set Payment Selection.
        @param C_PaySelection_ID Payment Selection */
        public void SetC_PaySelection_ID(int C_PaySelection_ID)
        {
            if (C_PaySelection_ID < 1) throw new ArgumentException("C_PaySelection_ID is mandatory.");
            Set_ValueNoCheck("C_PaySelection_ID", C_PaySelection_ID);
        }
        /** Get Payment Selection.
        @return Payment Selection */
        public int GetC_PaySelection_ID()
        {
            Object ii = Get_Value("C_PaySelection_ID");
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

        /** PaymentRule AD_Reference_ID=195 */
        public static int PAYMENTRULE_AD_Reference_ID = 195;
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
        @param C_InvoicePaySchedule_ID Invoice Payment Schedule */
        public void SetC_InvoicePaySchedule_ID(int C_InvoicePaySchedule_ID)
        {
            if (C_InvoicePaySchedule_ID < 1) throw new ArgumentException("C_InvoicePaySchedule_ID is mandatory.");
            Set_ValueNoCheck("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
        }
        /** Get Invoice Payment Schedule.
        @return Invoice Payment Schedule */
        public int GetC_InvoicePaySchedule_ID()
        {
            Object ii = Get_Value("C_InvoicePaySchedule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
