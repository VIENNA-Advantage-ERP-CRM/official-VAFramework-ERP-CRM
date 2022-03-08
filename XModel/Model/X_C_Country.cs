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
/** Generated Model for C_Country
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Country : PO
{
public X_C_Country (Context ctx, int C_Country_ID, Trx trxName) : base (ctx, C_Country_ID, trxName)
{
/** if (C_Country_ID == 0)
{
SetC_Country_ID (0);
SetCountryCode (null);
SetIsSummary (false);	// N
SetName (null);
}
 */
}
public X_C_Country (Ctx ctx, int C_Country_ID, Trx trxName) : base (ctx, C_Country_ID, trxName)
{
/** if (C_Country_ID == 0)
{
SetC_Country_ID (0);
SetCountryCode (null);
SetIsSummary (false);	// N
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Country (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Country (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Country (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Country()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371498L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054709L;
/** AD_Table_ID=170 */
public static int Table_ID;
 // =170;

/** TableName=C_Country */
public static String Table_Name="C_Country";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_Country[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Language AD_Reference_ID=106 */
public static int AD_LANGUAGE_AD_Reference_ID=106;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language != null && AD_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_Value ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Set Country.
@param C_Country_ID Country */
public void SetC_Country_ID (int C_Country_ID)
{
if (C_Country_ID < 1) throw new ArgumentException ("C_Country_ID is mandatory.");
Set_ValueNoCheck ("C_Country_ID", C_Country_ID);
}
/** Get Country.
@return Country */
public int GetC_Country_ID() 
{
Object ii = Get_Value("C_Country_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);
else
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set ISO Country Code.
@param CountryCode Upper-case two-letter alphabetic ISO Country code according to ISO 3166-1 - http://www.chemie.fu-berlin.de/diverse/doc/ISO_3166.html */
public void SetCountryCode (String CountryCode)
{
if (CountryCode == null) throw new ArgumentException ("CountryCode is mandatory.");
if (CountryCode.Length > 2)
{
log.Warning("Length > 2 - truncated");
CountryCode = CountryCode.Substring(0,2);
}
Set_Value ("CountryCode", CountryCode);
}
/** Get ISO Country Code.
@return Upper-case two-letter alphabetic ISO Country code according to ISO 3166-1 - http://www.chemie.fu-berlin.de/diverse/doc/ISO_3166.html */
public String GetCountryCode() 
{
return (String)Get_Value("CountryCode");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Address Print Format.
@param DisplaySequence Format for printing this Address */
public void SetDisplaySequence (String DisplaySequence)
{
if (DisplaySequence != null && DisplaySequence.Length > 20)
{
log.Warning("Length > 20 - truncated");
DisplaySequence = DisplaySequence.Substring(0,20);
}
Set_Value ("DisplaySequence", DisplaySequence);
}
/** Get Address Print Format.
@return Format for printing this Address */
public String GetDisplaySequence() 
{
return (String)Get_Value("DisplaySequence");
}
/** Set Local Address Format.
@param DisplaySequenceLocal Format for printing this Address locally */
public void SetDisplaySequenceLocal (String DisplaySequenceLocal)
{
if (DisplaySequenceLocal != null && DisplaySequenceLocal.Length > 20)
{
log.Warning("Length > 20 - truncated");
DisplaySequenceLocal = DisplaySequenceLocal.Substring(0,20);
}
Set_Value ("DisplaySequenceLocal", DisplaySequenceLocal);
}
/** Get Local Address Format.
@return Format for printing this Address locally */
public String GetDisplaySequenceLocal() 
{
return (String)Get_Value("DisplaySequenceLocal");
}
/** Set Bank Account No Format.
@param ExpressionBankAccountNo Format of the Bank Account */
public void SetExpressionBankAccountNo (String ExpressionBankAccountNo)
{
if (ExpressionBankAccountNo != null && ExpressionBankAccountNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
ExpressionBankAccountNo = ExpressionBankAccountNo.Substring(0,20);
}
Set_Value ("ExpressionBankAccountNo", ExpressionBankAccountNo);
}
/** Get Bank Account No Format.
@return Format of the Bank Account */
public String GetExpressionBankAccountNo() 
{
return (String)Get_Value("ExpressionBankAccountNo");
}
/** Set Bank Routing No Format.
@param ExpressionBankRoutingNo Format of the Bank Routing Number */
public void SetExpressionBankRoutingNo (String ExpressionBankRoutingNo)
{
if (ExpressionBankRoutingNo != null && ExpressionBankRoutingNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
ExpressionBankRoutingNo = ExpressionBankRoutingNo.Substring(0,20);
}
Set_Value ("ExpressionBankRoutingNo", ExpressionBankRoutingNo);
}
/** Get Bank Routing No Format.
@return Format of the Bank Routing Number */
public String GetExpressionBankRoutingNo() 
{
return (String)Get_Value("ExpressionBankRoutingNo");
}
/** Set Phone Format.
@param ExpressionPhone Format of the phone;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetExpressionPhone (String ExpressionPhone)
{
if (ExpressionPhone != null && ExpressionPhone.Length > 20)
{
log.Warning("Length > 20 - truncated");
ExpressionPhone = ExpressionPhone.Substring(0,20);
}
Set_Value ("ExpressionPhone", ExpressionPhone);
}
/** Get Phone Format.
@return Format of the phone;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetExpressionPhone() 
{
return (String)Get_Value("ExpressionPhone");
}
/** Set Postal Code Format.
@param ExpressionPostal Format of the postal code;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetExpressionPostal (String ExpressionPostal)
{
if (ExpressionPostal != null && ExpressionPostal.Length > 20)
{
log.Warning("Length > 20 - truncated");
ExpressionPostal = ExpressionPostal.Substring(0,20);
}
Set_Value ("ExpressionPostal", ExpressionPostal);
}
/** Get Postal Code Format.
@return Format of the postal code;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetExpressionPostal() 
{
return (String)Get_Value("ExpressionPostal");
}
/** Set Additional Postal Format.
@param ExpressionPostal_Add Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetExpressionPostal_Add (String ExpressionPostal_Add)
{
if (ExpressionPostal_Add != null && ExpressionPostal_Add.Length > 20)
{
log.Warning("Length > 20 - truncated");
ExpressionPostal_Add = ExpressionPostal_Add.Substring(0,20);
}
Set_Value ("ExpressionPostal_Add", ExpressionPostal_Add);
}
/** Get Additional Postal Format.
@return Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetExpressionPostal_Add() 
{
return (String)Get_Value("ExpressionPostal_Add");
}
/** Set Additional Postal code.
@param HasPostal_Add Has Additional Postal Code */
public void SetHasPostal_Add (Boolean HasPostal_Add)
{
Set_Value ("HasPostal_Add", HasPostal_Add);
}
/** Get Additional Postal code.
@return Has Additional Postal Code */
public Boolean IsHasPostal_Add() 
{
Object oo = Get_Value("HasPostal_Add");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Country has Region.
@param HasRegion Country contains Regions */
public void SetHasRegion (Boolean HasRegion)
{
Set_Value ("HasRegion", HasRegion);
}
/** Get Country has Region.
@return Country contains Regions */
public Boolean IsHasRegion() 
{
Object oo = Get_Value("HasRegion");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Reverse Local Address Lines.
@param IsAddressLinesLocalReverse Print Local Address in reverse Order */
public void SetIsAddressLinesLocalReverse (Boolean IsAddressLinesLocalReverse)
{
Set_ValueNoCheck ("IsAddressLinesLocalReverse", IsAddressLinesLocalReverse);
}
/** Get Reverse Local Address Lines.
@return Print Local Address in reverse Order */
public Boolean IsAddressLinesLocalReverse() 
{
Object oo = Get_Value("IsAddressLinesLocalReverse");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Reverse Address Lines.
@param IsAddressLinesReverse Print Address in reverse Order */
public void SetIsAddressLinesReverse (Boolean IsAddressLinesReverse)
{
Set_Value ("IsAddressLinesReverse", IsAddressLinesReverse);
}
/** Get Reverse Address Lines.
@return Print Address in reverse Order */
public Boolean IsAddressLinesReverse() 
{
Object oo = Get_Value("IsAddressLinesReverse");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
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
/** Set Media Size.
@param MediaSize Java Media Size */
public void SetMediaSize (String MediaSize)
{
if (MediaSize != null && MediaSize.Length > 40)
{
log.Warning("Length > 40 - truncated");
MediaSize = MediaSize.Substring(0,40);
}
Set_Value ("MediaSize", MediaSize);
}
/** Get Media Size.
@return Java Media Size */
public String GetMediaSize() 
{
return (String)Get_Value("MediaSize");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
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
/** Set Region.
@param RegionName Name of the Region */
public void SetRegionName (String RegionName)
{
if (RegionName != null && RegionName.Length > 60)
{
log.Warning("Length > 60 - truncated");
RegionName = RegionName.Substring(0,60);
}
Set_Value ("RegionName", RegionName);
}
/** Get Region.
@return Name of the Region */
public String GetRegionName() 
{
return (String)Get_Value("RegionName");
}
}

}
