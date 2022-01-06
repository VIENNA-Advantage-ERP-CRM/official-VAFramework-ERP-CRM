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
    using System.Data;/** Generated Model for C_ProvisionalInvoiceTax
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_ProvisionalInvoiceTax : PO
    {
        public X_C_ProvisionalInvoiceTax(Context ctx, int C_ProvisionalInvoiceTax_ID, Trx trxName) : base(ctx, C_ProvisionalInvoiceTax_ID, trxName)
        {/** if (C_ProvisionalInvoiceTax_ID == 0){SetC_ProvisionalInvoiceTax_ID (0);SetC_ProvisionalInvoice_ID (0);SetC_Tax_ID (0);} */
        }
        public X_C_ProvisionalInvoiceTax(Ctx ctx, int C_ProvisionalInvoiceTax_ID, Trx trxName) : base(ctx, C_ProvisionalInvoiceTax_ID, trxName)
        {/** if (C_ProvisionalInvoiceTax_ID == 0){SetC_ProvisionalInvoiceTax_ID (0);SetC_ProvisionalInvoice_ID (0);SetC_Tax_ID (0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoiceTax(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoiceTax(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_ProvisionalInvoiceTax(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_ProvisionalInvoiceTax() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27908190113491L;/** Last Updated Timestamp 7/12/2021 4:39:56 AM */
        public static long updatedMS = 1626064796702L;/** AD_Table_ID=1000553 */
        public static int Table_ID; // =1000553;
        /** TableName=C_ProvisionalInvoiceTax */
        public static String Table_Name = "C_ProvisionalInvoiceTax";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_ProvisionalInvoiceTax[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set C_ProvisionalInvoiceTax_ID.
@param C_ProvisionalInvoiceTax_ID C_ProvisionalInvoiceTax_ID */
        public void SetC_ProvisionalInvoiceTax_ID(int C_ProvisionalInvoiceTax_ID) { if (C_ProvisionalInvoiceTax_ID < 1) throw new ArgumentException("C_ProvisionalInvoiceTax_ID is mandatory."); Set_ValueNoCheck("C_ProvisionalInvoiceTax_ID", C_ProvisionalInvoiceTax_ID); }/** Get C_ProvisionalInvoiceTax_ID.
@return C_ProvisionalInvoiceTax_ID */
        public int GetC_ProvisionalInvoiceTax_ID() { Object ii = Get_Value("C_ProvisionalInvoiceTax_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Provisional Invoice.
@param C_ProvisionalInvoice_ID Provisional Invoice */
        public void SetC_ProvisionalInvoice_ID(int C_ProvisionalInvoice_ID) { if (C_ProvisionalInvoice_ID < 1) throw new ArgumentException("C_ProvisionalInvoice_ID is mandatory."); Set_ValueNoCheck("C_ProvisionalInvoice_ID", C_ProvisionalInvoice_ID); }/** Get Provisional Invoice.
@return Provisional Invoice */
        public int GetC_ProvisionalInvoice_ID() { Object ii = Get_Value("C_ProvisionalInvoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Tax.
@param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID) { if (C_Tax_ID < 1) throw new ArgumentException("C_Tax_ID is mandatory."); Set_ValueNoCheck("C_Tax_ID", C_Tax_ID); }/** Get Tax.
@return Tax identifier */
        public int GetC_Tax_ID() { Object ii = Get_Value("C_Tax_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Price includes Tax.
@param IsTaxIncluded Tax is included in the price  */
        public void SetIsTaxIncluded(Boolean IsTaxIncluded) { Set_Value("IsTaxIncluded", IsTaxIncluded); }/** Get Price includes Tax.
@return Tax is included in the price  */
        public Boolean IsTaxIncluded() { Object oo = Get_Value("IsTaxIncluded"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Taxable Amount.
@param TaxAbleAmt Taxable amount in Transaction currency */
        public void SetTaxAbleAmt(Decimal? TaxAbleAmt) { Set_Value("TaxAbleAmt", (Decimal?)TaxAbleAmt); }/** Get Taxable Amount.
@return Taxable amount in Transaction currency */
        public Decimal GetTaxAbleAmt() { Object bd = Get_Value("TaxAbleAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Tax Amount.
@param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt) { Set_Value("TaxAmt", (Decimal?)TaxAmt); }/** Get Tax Amount.
@return Tax Amount for a document */
        public Decimal GetTaxAmt() { Object bd = Get_Value("TaxAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}