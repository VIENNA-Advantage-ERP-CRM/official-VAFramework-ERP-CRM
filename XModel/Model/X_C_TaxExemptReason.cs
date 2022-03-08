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
    using System.Data;/** Generated Model for C_TaxExemptReason
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_TaxExemptReason : PO
    {
        public X_C_TaxExemptReason(Context ctx, int C_TaxExemptReason_ID, Trx trxName) : base(ctx, C_TaxExemptReason_ID, trxName)
        {/** if (C_TaxExemptReason_ID == 0){SetC_TaxExemptReason_ID (0);SetValue (null);} */
        }
        public X_C_TaxExemptReason(Ctx ctx, int C_TaxExemptReason_ID, Trx trxName) : base(ctx, C_TaxExemptReason_ID, trxName)
        {/** if (C_TaxExemptReason_ID == 0){SetC_TaxExemptReason_ID (0);SetValue (null);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_TaxExemptReason(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_TaxExemptReason(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_C_TaxExemptReason(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_C_TaxExemptReason() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27921107985452L;/** Last Updated Timestamp 12/8/2021 4:57:48 PM */
        public static long updatedMS = 1638982668663L;/** AD_Table_ID=1000554 */
        public static int Table_ID; // =1000554;
        /** TableName=C_TaxExemptReason */
        public static String Table_Name = "C_TaxExemptReason";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_C_TaxExemptReason[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Tax Exemption Reason.
@param C_TaxExemptReason_ID Tax Exemption reason indicates the reason for the exemption for the items those are exempted from tax. */
        public void SetC_TaxExemptReason_ID(int C_TaxExemptReason_ID) { if (C_TaxExemptReason_ID < 1) throw new ArgumentException("C_TaxExemptReason_ID is mandatory."); Set_ValueNoCheck("C_TaxExemptReason_ID", C_TaxExemptReason_ID); }/** Get Tax Exemption Reason.
@return Tax Exemption reason indicates the reason for the exemption for the items those are exempted from tax. */
        public int GetC_TaxExemptReason_ID() { Object ii = Get_Value("C_TaxExemptReason_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 500) { log.Warning("Length > 500 - truncated"); Description = Description.Substring(0, 500); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Default.
@param IsDefault Default value */
        public void SetIsDefault(Boolean IsDefault) { Set_Value("IsDefault", IsDefault); }/** Get Default.
@return Default value */
        public Boolean IsDefault() { Object oo = Get_Value("IsDefault"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Name.
@param Name Alphanumeric identifier of the entity */
        public void SetName(String Name) { if (Name != null && Name.Length > 500) { log.Warning("Length > 500 - truncated"); Name = Name.Substring(0, 500); } Set_Value("Name", Name); }/** Get Name.
@return Alphanumeric identifier of the entity */
        public String GetName() { return (String)Get_Value("Name"); }/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value) { if (Value == null) throw new ArgumentException("Value is mandatory."); if (Value.Length > 60) { log.Warning("Length > 60 - truncated"); Value = Value.Substring(0, 60); } Set_Value("Value", Value); }/** Get Search Key.
@return Search key for the record in the format required - must be unique */
        public String GetValue() { return (String)Get_Value("Value"); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetValue()); }
    }
}