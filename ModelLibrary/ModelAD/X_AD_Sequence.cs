namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for AD_Sequence
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Sequence : PO{public X_AD_Sequence (Context ctx, int AD_Sequence_ID, Trx trxName) : base (ctx, AD_Sequence_ID, trxName){/** if (AD_Sequence_ID == 0){SetAD_Sequence_ID (0);SetCurrentNext (0);// 1000000
SetCurrentNextSys (0);// 100
SetExport_ID (0);// 1000000
SetIncrementNo (0);// 1
SetIsAutoSequence (false);SetName (null);SetStartNo (0);// 1000000
} */
}public X_AD_Sequence (Ctx ctx, int AD_Sequence_ID, Trx trxName) : base (ctx, AD_Sequence_ID, trxName){/** if (AD_Sequence_ID == 0){SetAD_Sequence_ID (0);SetCurrentNext (0);// 1000000
SetCurrentNextSys (0);// 100
SetExport_ID (0);// 1000000
SetIncrementNo (0);// 1
SetIsAutoSequence (false);SetName (null);SetStartNo (0);// 1000000
} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Sequence (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Sequence (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Sequence (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Sequence(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27888339363964L;/** Last Updated Timestamp 11/24/2020 11:34:07 AM */
public static long updatedMS = 1606214047175L;/** AD_Table_ID=115 */
public static int Table_ID; // =115;
/** TableName=AD_Sequence */
public static String Table_Name="AD_Sequence";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(6);/** AccessLevel
@return 6 - System - Client 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_AD_Sequence[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Sequence.
@param AD_Sequence_ID Document Sequence */
public void SetAD_Sequence_ID (int AD_Sequence_ID){if (AD_Sequence_ID < 1) throw new ArgumentException ("AD_Sequence_ID is mandatory.");Set_ValueNoCheck ("AD_Sequence_ID", AD_Sequence_ID);}/** Get Sequence.
@return Document Sequence */
public int GetAD_Sequence_ID() {Object ii = Get_Value("AD_Sequence_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Append Month After Year.
@param AddMonthYear Append the Month after the year in document number */
public void SetAddMonthYear (Boolean AddMonthYear){Set_Value ("AddMonthYear", AddMonthYear);}/** Get Append Month After Year.
@return Append the Month after the year in document number */
public Boolean IsAddMonthYear() {Object oo = Get_Value("AddMonthYear");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Append Year as Prefix.
@param AddYearPrefix Append the Year to the Prefix in document number */
public void SetAddYearPrefix (Boolean AddYearPrefix){Set_Value ("AddYearPrefix", AddYearPrefix);}/** Get Append Year as Prefix.
@return Append the Year to the Prefix in document number */
public Boolean IsAddYearPrefix() {Object oo = Get_Value("AddYearPrefix");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Current Next.
@param CurrentNext The next number to be used */
public void SetCurrentNext (int CurrentNext){Set_Value ("CurrentNext", CurrentNext);}/** Get Current Next.
@return The next number to be used */
public int GetCurrentNext() {Object ii = Get_Value("CurrentNext");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Current Next (System).
@param CurrentNextSys Next sequence for system use */
public void SetCurrentNextSys (int CurrentNextSys){Set_Value ("CurrentNextSys", CurrentNextSys);}/** Get Current Next (System).
@return Next sequence for system use */
public int GetCurrentNextSys() {Object ii = Get_Value("CurrentNextSys");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Date Column.
@param DateColumn Fully qualified date column */
public void SetDateColumn (String DateColumn){if (DateColumn != null && DateColumn.Length > 60){log.Warning("Length > 60 - truncated");DateColumn = DateColumn.Substring(0,60);}Set_Value ("DateColumn", DateColumn);}/** Get Date Column.
@return Fully qualified date column */
public String GetDateColumn() {return (String)Get_Value("DateColumn");}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (int Export_ID){if (Export_ID < 1) throw new ArgumentException ("Export_ID is mandatory.");Set_ValueNoCheck ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public int GetExport_ID() {Object ii = Get_Value("Export_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Increment.
@param IncrementNo The number to increment the last document number by */
public void SetIncrementNo (int IncrementNo){Set_Value ("IncrementNo", IncrementNo);}/** Get Increment.
@return The number to increment the last document number by */
public int GetIncrementNo() {Object ii = Get_Value("IncrementNo");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Activate Audit.
@param IsAudited Activate Audit Trail of what numbers are generated */
public void SetIsAudited (Boolean IsAudited){Set_Value ("IsAudited", IsAudited);}/** Get Activate Audit.
@return Activate Audit Trail of what numbers are generated */
public Boolean IsAudited() {Object oo = Get_Value("IsAudited");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Auto Numbering.
@param IsAutoSequence Automatically assign the next number */
public void SetIsAutoSequence (Boolean IsAutoSequence){Set_Value ("IsAutoSequence", IsAutoSequence);}/** Get Auto Numbering.
@return Automatically assign the next number */
public Boolean IsAutoSequence() {Object oo = Get_Value("IsAutoSequence");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Organization Level.
@param IsOrgLevelSequence This sequence can be defined for each organization */
public void SetIsOrgLevelSequence (Boolean IsOrgLevelSequence){Set_Value ("IsOrgLevelSequence", IsOrgLevelSequence);}/** Get Organization Level.
@return This sequence can be defined for each organization */
public Boolean IsOrgLevelSequence() {Object oo = Get_Value("IsOrgLevelSequence");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Used for Record ID.
@param IsTableID The document number  will be used as the record key */
public void SetIsTableID (Boolean IsTableID){Set_Value ("IsTableID", IsTableID);}/** Get Used for Record ID.
@return The document number  will be used as the record key */
public Boolean IsTableID() {Object oo = Get_Value("IsTableID");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 60){log.Warning("Length > 60 - truncated");Name = Name.Substring(0,60);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetName());}/** Set Org Column.
@param OrgColumn Fully qualified Organization column (AD_Org_ID) */
public void SetOrgColumn (String OrgColumn){if (OrgColumn != null && OrgColumn.Length > 60){log.Warning("Length > 60 - truncated");OrgColumn = OrgColumn.Substring(0,60);}Set_Value ("OrgColumn", OrgColumn);}/** Get Org Column.
@return Fully qualified Organization column (AD_Org_ID) */
public String GetOrgColumn() {return (String)Get_Value("OrgColumn");}/** Set Prefix.
@param Prefix Prefix before the sequence number */
public void SetPrefix (String Prefix){if (Prefix != null && Prefix.Length > 10){log.Warning("Length > 10 - truncated");Prefix = Prefix.Substring(0,10);}Set_Value ("Prefix", Prefix);}/** Get Prefix.
@return Prefix before the sequence number */
public String GetPrefix() {return (String)Get_Value("Prefix");}/** Set Prefix(Year & Month) and Doc No Seperator.
@param PrefixAndDocNoSeperator Format of the value; Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetPrefixAndDocNoSeperator (String PrefixAndDocNoSeperator){if (PrefixAndDocNoSeperator != null && PrefixAndDocNoSeperator.Length > 40){log.Warning("Length > 40 - truncated");PrefixAndDocNoSeperator = PrefixAndDocNoSeperator.Substring(0,40);}Set_Value ("PrefixAndDocNoSeperator", PrefixAndDocNoSeperator);}/** Get Prefix(Year & Month) and Doc No Seperator.
@return Format of the value; Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetPrefixAndDocNoSeperator() {return (String)Get_Value("PrefixAndDocNoSeperator");}/** Set Element Separator.
@param Separator Element Separator */
public void SetSeparator (String Separator){if (Separator != null && Separator.Length > 1){log.Warning("Length > 1 - truncated");Separator = Separator.Substring(0,1);}Set_Value ("Separator", Separator);}/** Get Element Separator.
@return Element Separator */
public String GetSeparator() {return (String)Get_Value("Separator");}/** Set Restart Sequence Every Month.
@param StartNewMonth Restart the sequence with Start on every month */
public void SetStartNewMonth (Boolean StartNewMonth){Set_Value ("StartNewMonth", StartNewMonth);}/** Get Restart Sequence Every Month.
@return Restart the sequence with Start on every month */
public Boolean IsStartNewMonth() {Object oo = Get_Value("StartNewMonth");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Restart Sequence Every Year.
@param StartNewYear Restart the sequence with Start on every 1/1 */
public void SetStartNewYear (Boolean StartNewYear){Set_Value ("StartNewYear", StartNewYear);}/** Get Restart Sequence Every Year.
@return Restart the sequence with Start on every 1/1 */
public Boolean IsStartNewYear() {Object oo = Get_Value("StartNewYear");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Start No.
@param StartNo Starting number/position */
public void SetStartNo (int StartNo){Set_Value ("StartNo", StartNo);}/** Get Start No.
@return Starting number/position */
public int GetStartNo() {Object ii = Get_Value("StartNo");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Suffix.
@param Suffix Suffix after the number */
public void SetSuffix (String Suffix){if (Suffix != null && Suffix.Length > 10){log.Warning("Length > 10 - truncated");Suffix = Suffix.Substring(0,10);}Set_Value ("Suffix", Suffix);}/** Get Suffix.
@return Suffix after the number */
public String GetSuffix() {return (String)Get_Value("Suffix");}/** Set Value Format.
@param VFormat Format of the value; Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetVFormat (String VFormat){if (VFormat != null && VFormat.Length > 40){log.Warning("Length > 40 - truncated");VFormat = VFormat.Substring(0,40);}Set_Value ("VFormat", VFormat);}/** Get Value Format.
@return Format of the value; Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetVFormat() {return (String)Get_Value("VFormat");}
/** YearMonthFormat AD_Reference_ID=1000340 */
public static int YEARMONTHFORMAT_AD_Reference_ID=1000340;/** YYYYMM = 1 */
public static String YEARMONTHFORMAT_YYYYMM = "1";/** YYMM = 2 */
public static String YEARMONTHFORMAT_YYMM = "2";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsYearMonthFormatValid (String test){return test == null || test.Equals("1") || test.Equals("2");}/** Set Year and Month Format.
@param YearMonthFormat The Format in which the year and month will be displayed in Prefix. */
public void SetYearMonthFormat (String YearMonthFormat){if (!IsYearMonthFormatValid(YearMonthFormat))
throw new ArgumentException ("YearMonthFormat Invalid value - " + YearMonthFormat + " - Reference_ID=1000340 - 1 - 2");if (YearMonthFormat != null && YearMonthFormat.Length > 1){log.Warning("Length > 1 - truncated");YearMonthFormat = YearMonthFormat.Substring(0,1);}Set_Value ("YearMonthFormat", YearMonthFormat);}/** Get Year and Month Format.
@return The Format in which the year and month will be displayed in Prefix. */
public String GetYearMonthFormat() {return (String)Get_Value("YearMonthFormat");}
/** Set Maintain Separate Trx.
@param IsUseSeparateTrx Restrict to maintain separate transaction for document no. */
public void SetIsUseSeparateTrx(Boolean IsUseSeparateTrx) { Set_Value("IsUseSeparateTrx", IsUseSeparateTrx); }
/** Get Maintain Separate Trx.
@return Restrict to maintain separate transaction for document no. */
public Boolean IsUseSeparateTrx() { Object oo = Get_Value("IsUseSeparateTrx"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
}
}