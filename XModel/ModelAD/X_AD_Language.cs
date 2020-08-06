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
/** Generated Model for AD_Language
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Language : PO
{
public X_AD_Language (Context ctx, int AD_Language_ID, Trx trxName) : base (ctx, AD_Language_ID, trxName)
{
/** if (AD_Language_ID == 0)
{
SetAD_Language (null);
SetAD_Language_ID (0);	// @SQL=SELECT NVL(MAX(AD_Language_ID),999999)+1 FROM AD_Language WHERE AD_Language_ID > 1000
SetCountryCode (null);
SetIsBaseLanguage (false);	// N
SetIsDecimalPoint (false);
SetIsSystemLanguage (false);
SetLanguageISO (null);
SetName (null);
}
 */
}
public X_AD_Language (Ctx ctx, int AD_Language_ID, Trx trxName) : base (ctx, AD_Language_ID, trxName)
{
/** if (AD_Language_ID == 0)
{
SetAD_Language (null);
SetAD_Language_ID (0);	// @SQL=SELECT NVL(MAX(AD_Language_ID),999999)+1 FROM AD_Language WHERE AD_Language_ID > 1000
SetCountryCode (null);
SetIsBaseLanguage (false);	// N
SetIsDecimalPoint (false);
SetIsSystemLanguage (false);
SetLanguageISO (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Language (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Language (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Language (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Language()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362031L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045242L;
/** AD_Table_ID=111 */
public static int Table_ID;
 // =111;

/** TableName=AD_Language */
public static String Table_Name="AD_Language";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_Language[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language == null) throw new ArgumentException ("AD_Language is mandatory.");
if (AD_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_ValueNoCheck ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Set Language ID.
@param AD_Language_ID Language ID */
public void SetAD_Language_ID (int AD_Language_ID)
{
if (AD_Language_ID < 1) throw new ArgumentException ("AD_Language_ID is mandatory.");
Set_ValueNoCheck ("AD_Language_ID", AD_Language_ID);
}
/** Get Language ID.
@return Language ID */
public int GetAD_Language_ID() 
{
Object ii = Get_Value("AD_Language_ID");
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
/** Set Date Pattern.
@param DatePattern Java Date Pattern */
public void SetDatePattern (String DatePattern)
{
if (DatePattern != null && DatePattern.Length > 20)
{
log.Warning("Length > 20 - truncated");
DatePattern = DatePattern.Substring(0,20);
}
Set_Value ("DatePattern", DatePattern);
}
/** Get Date Pattern.
@return Java Date Pattern */
public String GetDatePattern() 
{
return (String)Get_Value("DatePattern");
}
/** Set Base Language.
@param IsBaseLanguage The system information is maintained in this language */
public void SetIsBaseLanguage (Boolean IsBaseLanguage)
{
Set_ValueNoCheck ("IsBaseLanguage", IsBaseLanguage);
}
/** Get Base Language.
@return The system information is maintained in this language */
public Boolean IsBaseLanguage() 
{
Object oo = Get_Value("IsBaseLanguage");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Decimal Point.
@param IsDecimalPoint The number notation has a decimal point (no decimal comma) */
public void SetIsDecimalPoint (Boolean IsDecimalPoint)
{
Set_Value ("IsDecimalPoint", IsDecimalPoint);
}
/** Get Decimal Point.
@return The number notation has a decimal point (no decimal comma) */
public Boolean IsDecimalPoint() 
{
Object oo = Get_Value("IsDecimalPoint");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set System Language.
@param IsSystemLanguage The screens, etc. are maintained in this Language */
public void SetIsSystemLanguage (Boolean IsSystemLanguage)
{
Set_Value ("IsSystemLanguage", IsSystemLanguage);
}
/** Get System Language.
@return The screens, etc. are maintained in this Language */
public Boolean IsSystemLanguage() 
{
Object oo = Get_Value("IsSystemLanguage");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set ISO Language Code.
@param LanguageISO Lower-case two-letter ISO-3166 code - http://www.ics.uci.edu/pub/ietf/http/related/iso639.txt */
public void SetLanguageISO (String LanguageISO)
{
if (LanguageISO == null) throw new ArgumentException ("LanguageISO is mandatory.");
if (LanguageISO.Length > 2)
{
log.Warning("Length > 2 - truncated");
LanguageISO = LanguageISO.Substring(0,2);
}
Set_Value ("LanguageISO", LanguageISO);
}
/** Get ISO Language Code.
@return Lower-case two-letter ISO-3166 code - http://www.ics.uci.edu/pub/ietf/http/related/iso639.txt */
public String GetLanguageISO() 
{
return (String)Get_Value("LanguageISO");
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Time Pattern.
@param TimePattern Java Time Pattern */
public void SetTimePattern (String TimePattern)
{
if (TimePattern != null && TimePattern.Length > 20)
{
log.Warning("Length > 20 - truncated");
TimePattern = TimePattern.Substring(0,20);
}
Set_Value ("TimePattern", TimePattern);
}
/** Get Time Pattern.
@return Java Time Pattern */
public String GetTimePattern() 
{
return (String)Get_Value("TimePattern");
}
}

}
