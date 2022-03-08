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
    /** Generated Model for GL_JournalLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_GL_JournalLine : PO
    {
        public X_GL_JournalLine(Context ctx, int GL_JournalLine_ID, Trx trxName)
            : base(ctx, GL_JournalLine_ID, trxName)
        {
            /** if (GL_JournalLine_ID == 0)
            {
            SetAmtAcctCr (0.0);
            SetAmtAcctDr (0.0);
            SetAmtSourceCr (0.0);
            SetAmtSourceDr (0.0);
            SetC_ConversionType_ID (0);
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_ValidCombination_ID (0);
            SetCurrencyRate (0.0);	// @CurrencyRate@;
            1
            SetDateAcct (DateTime.Now);	// @DateAcct@
            SetGL_JournalLine_ID (0);
            SetGL_Journal_ID (0);
            SetIsGenerated (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM GL_JournalLine WHERE GL_Journal_ID=@GL_Journal_ID@
            SetProcessed (false);	// N
            }
             */
        }
        public X_GL_JournalLine(Ctx ctx, int GL_JournalLine_ID, Trx trxName)
            : base(ctx, GL_JournalLine_ID, trxName)
        {
            /** if (GL_JournalLine_ID == 0)
            {
            SetAmtAcctCr (0.0);
            SetAmtAcctDr (0.0);
            SetAmtSourceCr (0.0);
            SetAmtSourceDr (0.0);
            SetC_ConversionType_ID (0);
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_ValidCombination_ID (0);
            SetCurrencyRate (0.0);	// @CurrencyRate@;
            1
            SetDateAcct (DateTime.Now);	// @DateAcct@
            SetGL_JournalLine_ID (0);
            SetGL_Journal_ID (0);
            SetIsGenerated (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM GL_JournalLine WHERE GL_Journal_ID=@GL_Journal_ID@
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GL_JournalLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GL_JournalLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_GL_JournalLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_GL_JournalLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514376654L;
        /** Last Updated Timestamp 7/29/2010 1:07:39 PM */
        public static long updatedMS = 1280389059865L;
        /** AD_Table_ID=226 */
        public static int Table_ID;
        // =226;

        /** TableName=GL_JournalLine */
        public static String Table_Name = "GL_JournalLine";

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
            StringBuilder sb = new StringBuilder("X_GL_JournalLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Accounted Credit.
        @param AmtAcctCr Accounted Credit Amount */
        public void SetAmtAcctCr(Decimal? AmtAcctCr)
        {
            if (AmtAcctCr == null) throw new ArgumentException("AmtAcctCr is mandatory.");
            Set_ValueNoCheck("AmtAcctCr", (Decimal?)AmtAcctCr);
        }
        /** Get Accounted Credit.
        @return Accounted Credit Amount */
        public Decimal GetAmtAcctCr()
        {
            Object bd = Get_Value("AmtAcctCr");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Accounted Debit.
        @param AmtAcctDr Accounted Debit Amount */
        public void SetAmtAcctDr(Decimal? AmtAcctDr)
        {
            if (AmtAcctDr == null) throw new ArgumentException("AmtAcctDr is mandatory.");
            Set_ValueNoCheck("AmtAcctDr", (Decimal?)AmtAcctDr);
        }
        /** Get Accounted Debit.
        @return Accounted Debit Amount */
        public Decimal GetAmtAcctDr()
        {
            Object bd = Get_Value("AmtAcctDr");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Source Credit.
        @param AmtSourceCr Source Credit Amount */
        public void SetAmtSourceCr(Decimal? AmtSourceCr)
        {
            if (AmtSourceCr == null) throw new ArgumentException("AmtSourceCr is mandatory.");
            Set_Value("AmtSourceCr", (Decimal?)AmtSourceCr);
        }
        /** Get Source Credit.
        @return Source Credit Amount */
        public Decimal GetAmtSourceCr()
        {
            Object bd = Get_Value("AmtSourceCr");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Source Debit.
        @param AmtSourceDr Source Debit Amount */
        public void SetAmtSourceDr(Decimal? AmtSourceDr)
        {
            if (AmtSourceDr == null) throw new ArgumentException("AmtSourceDr is mandatory.");
            Set_Value("AmtSourceDr", (Decimal?)AmtSourceDr);
        }
        /** Get Source Debit.
        @return Source Debit Amount */
        public Decimal GetAmtSourceDr()
        {
            Object bd = Get_Value("AmtSourceDr");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Currency Type.
        @param C_ConversionType_ID Currency Conversion Rate Type */
        public void SetC_ConversionType_ID(int C_ConversionType_ID)
        {
            if (C_ConversionType_ID < 1) throw new ArgumentException("C_ConversionType_ID is mandatory.");
            Set_Value("C_ConversionType_ID", C_ConversionType_ID);
        }
        /** Get Currency Type.
        @return Currency Conversion Rate Type */
        public int GetC_ConversionType_ID()
        {
            Object ii = Get_Value("C_ConversionType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Combination.
        @param C_ValidCombination_ID Valid Account Combination */
        public void SetC_ValidCombination_ID(int C_ValidCombination_ID)
        {
            if (C_ValidCombination_ID < 1) throw new ArgumentException("C_ValidCombination_ID is mandatory.");
            Set_Value("C_ValidCombination_ID", C_ValidCombination_ID);
        }
        /** Get Combination.
        @return Valid Account Combination */
        public int GetC_ValidCombination_ID()
        {
            Object ii = Get_Value("C_ValidCombination_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Rate.
        @param CurrencyRate Currency Conversion Rate */
        public void SetCurrencyRate(Decimal? CurrencyRate)
        {
            if (CurrencyRate == null) throw new ArgumentException("CurrencyRate is mandatory.");
            Set_ValueNoCheck("CurrencyRate", (Decimal?)CurrencyRate);
        }
        /** Get Rate.
        @return Currency Conversion Rate */
        public Decimal GetCurrencyRate()
        {
            Object bd = Get_Value("CurrencyRate");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory.");
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
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







        /** ElementType AD_Reference_ID=181 */
        public static int ELEMENTTYPE_AD_Reference_ID = 181;/** Organization = OO */
        public static String ELEMENTTYPE_Organization = "OO";/** Account = AC */
        public static String ELEMENTTYPE_Account = "AC";/** Product = PR */
        public static String ELEMENTTYPE_Product = "PR";/** BPartner = BP */
        public static String ELEMENTTYPE_BPartner = "BP";/** Org Trx = OT */
        public static String ELEMENTTYPE_OrgTrx = "OT";/** Location From = LF */
        public static String ELEMENTTYPE_LocationFrom = "LF";/** Location To = LT */
        public static String ELEMENTTYPE_LocationTo = "LT";/** Sales Region = SR */
        public static String ELEMENTTYPE_SalesRegion = "SR";/** Project = PJ */
        public static String ELEMENTTYPE_Project = "PJ";/** Campaign = MC */
        public static String ELEMENTTYPE_Campaign = "MC";/** User List 1 = U1 */
        public static String ELEMENTTYPE_UserList1 = "U1";/** User List 2 = U2 */
        public static String ELEMENTTYPE_UserList2 = "U2";/** Activity = AY */
        public static String ELEMENTTYPE_Activity = "AY";/** Sub Account = SA */
        public static String ELEMENTTYPE_SubAccount = "SA";/** User Element 1 = X1 */
        public static String ELEMENTTYPE_UserElement1 = "X1";/** User Element 2 = X2 */
        public static String ELEMENTTYPE_UserElement2 = "X2";/** User Element 3 = X3 */
        public static String ELEMENTTYPE_UserElement3 = "X3";/** User Element 4 = X4 */
        public static String ELEMENTTYPE_UserElement4 = "X4";/** User Element 5 = X5 */
        public static String ELEMENTTYPE_UserElement5 = "X5";/** User Element 6 = X6 */
        public static String ELEMENTTYPE_UserElement6 = "X6";/** User Element 7 = X7 */
        public static String ELEMENTTYPE_UserElement7 = "X7";/** User Element 8 = X8 */
        public static String ELEMENTTYPE_UserElement8 = "X8";/** User Element 9 = X9 */
        public static String ELEMENTTYPE_UserElement9 = "X9";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsElementTypeValid(String test) { return test == null || test.Equals("OO") || test.Equals("AC") || test.Equals("PR") || test.Equals("BP") || test.Equals("OT") || test.Equals("LF") || test.Equals("LT") || test.Equals("SR") || test.Equals("PJ") || test.Equals("MC") || test.Equals("U1") || test.Equals("U2") || test.Equals("AY") || test.Equals("SA") || test.Equals("X1") || test.Equals("X2") || test.Equals("X3") || test.Equals("X4") || test.Equals("X5") || test.Equals("X6") || test.Equals("X7") || test.Equals("X8") || test.Equals("X9"); }/** Set Type.
@param ElementType Element Type (account or user defined) FRPT_PeriodType */
        public void SetElementType(String ElementType)
        {
            if (!IsElementTypeValid(ElementType))
                throw new ArgumentException("ElementType Invalid value - " + ElementType + " - Reference_ID=181 - OO - AC - PR - BP - OT - LF - LT - SR - PJ - MC - U1 - U2 - AY - SA - X1 - X2 - X3 - X4 - X5 - X6 - X7 - X8 - X9"); if (ElementType != null && ElementType.Length > 2) { log.Warning("Length > 2 - truncated"); ElementType = ElementType.Substring(0, 2); }
            Set_Value("ElementType", ElementType);
        }/** Get Type.
@return Element Type (account or user defined) FRPT_PeriodType */
        public String GetElementType() { return (String)Get_Value("ElementType"); }













        /** Set Export.

                                                                                    * 
                                                                                    * 
                                                                                    * 
                                                                                    * 
                                                                                    * 
                                                                                    * @param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Journal Line.
@param GL_JournalLine_ID General Ledger Journal Line */
















        /** Set Journal Line.
        @param GL_JournalLine_ID General Ledger Journal Line */
        public void SetGL_JournalLine_ID(int GL_JournalLine_ID)
        {
            if (GL_JournalLine_ID < 1) throw new ArgumentException("GL_JournalLine_ID is mandatory.");
            Set_ValueNoCheck("GL_JournalLine_ID", GL_JournalLine_ID);
        }
        /** Get Journal Line.
        @return General Ledger Journal Line */
        public int GetGL_JournalLine_ID()
        {
            Object ii = Get_Value("GL_JournalLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Journal.
        @param GL_Journal_ID General Ledger Journal */
        public void SetGL_Journal_ID(int GL_Journal_ID)
        {
            if (GL_Journal_ID < 1) throw new ArgumentException("GL_Journal_ID is mandatory.");
            Set_ValueNoCheck("GL_Journal_ID", GL_Journal_ID);
        }
        /** Get Journal.
        @return General Ledger Journal */
        public int GetGL_Journal_ID()
        {
            Object ii = Get_Value("GL_Journal_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Generated.
        @param IsGenerated This Line is generated */
        public void SetIsGenerated(Boolean IsGenerated)
        {
            Set_ValueNoCheck("IsGenerated", IsGenerated);
        }
        /** Get Generated.
        @return This Line is generated */
        public Boolean IsGenerated()
        {
            Object oo = Get_Value("IsGenerated");
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetLine().ToString());
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
        /** Set Quantity.
        @param Qty Quantity */
        public void SetQty(Decimal? Qty)
        {
            Set_Value("Qty", (Decimal?)Qty);
        }
        /** Get Quantity.
        @return Quantity */
        public Decimal GetQty()
        {
            Object bd = Get_Value("Qty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Reversal Document.
        @param ReversalDoc_ID Reference of its original document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }
        /** Get Reversal Document.
        @return Reference of its original document */
        public int GetReversalDoc_ID()
        {
            Object ii = Get_Value("ReversalDoc_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /// <summary>
        /// AD_OrgTrx_ID AD_Reference_ID=130 
        /// </summary>
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /// <summary>
        ///  Set Trx Organization.
        /// </summary>
        /// <param name="AD_OrgTrx_ID">Performing or initiating organization</param>
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
        }
        /// <summary>
        /// Get Trx Organization.
        /// </summary>
        /// <returns>Performing or initiating organization</returns>
        public int GetAD_OrgTrx_ID() { Object ii = Get_Value("AD_OrgTrx_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

    }

}
