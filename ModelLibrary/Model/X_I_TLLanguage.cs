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
/** Generated Model for I_TLLanguage
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_TLLanguage : PO
{
public X_I_TLLanguage (Context ctx, int I_TLLanguage_ID, Trx trxName) : base (ctx, I_TLLanguage_ID, trxName)
{
/** if (I_TLLanguage_ID == 0)
{
SetCountryCode (null);
SetI_TLLanguage_ID (0);	// @SQL=SELECT NVL(MAX(I_TLLanguage_ID),999999)+1 FROM I_TLLanguage WHERE I_TLLanguage_ID > 1000
SetIsBaseLanguage (false);
SetIsDecimalPoint (false);
SetIsSystemLanguage (false);
SetLanguageISO (null);
}
 */
}
public X_I_TLLanguage (Ctx ctx, int I_TLLanguage_ID, Trx trxName) : base (ctx, I_TLLanguage_ID, trxName)
{
/** if (I_TLLanguage_ID == 0)
{
SetCountryCode (null);
SetI_TLLanguage_ID (0);	// @SQL=SELECT NVL(MAX(I_TLLanguage_ID),999999)+1 FROM I_TLLanguage WHERE I_TLLanguage_ID > 1000
SetIsBaseLanguage (false);
SetIsDecimalPoint (false);
SetIsSystemLanguage (false);
SetLanguageISO (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLLanguage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLLanguage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLLanguage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_TLLanguage()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27638800839375L;
/** Last Updated Timestamp 12/28/2012 11:48:42 AM */
public static long updatedMS = 1356675522586L;
/** AD_Table_ID=1000403 */
public static int Table_ID;
 // =1000403;

/** TableName=I_TLLanguage */
public static String Table_Name="I_TLLanguage";

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
StringBuilder sb = new StringBuilder ("X_I_TLLanguage[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Language.
@param I_TLLanguage Language */
public void SetI_TLLanguage (String I_TLLanguage)
{
if (I_TLLanguage != null && I_TLLanguage.Length > 5)
{
log.Warning("Length > 5 - truncated");
I_TLLanguage = I_TLLanguage.Substring(0,5);
}
Set_Value ("I_TLLanguage", I_TLLanguage);
}
/** Get Language.
@return Language */
public String GetI_TLLanguage() 
{
return (String)Get_Value("I_TLLanguage");
}
/** Set I_TLLanguage_ID.
@param I_TLLanguage_ID I_TLLanguage_ID */
public void SetI_TLLanguage_ID (int I_TLLanguage_ID)
{
if (I_TLLanguage_ID < 1) throw new ArgumentException ("I_TLLanguage_ID is mandatory.");
Set_ValueNoCheck ("I_TLLanguage_ID", I_TLLanguage_ID);
}
/** Get I_TLLanguage_ID.
@return I_TLLanguage_ID */
public int GetI_TLLanguage_ID() 
{
Object ii = Get_Value("I_TLLanguage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Base Language.
@param IsBaseLanguage The system information is maintained in this language */
public void SetIsBaseLanguage (Boolean IsBaseLanguage)
{
Set_Value ("IsBaseLanguage", IsBaseLanguage);
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
@param LanguageISO Lower-case two-letter ISO-3166 code - http://www.ics.uci.edu/pub/ietf/http/related/iso639.txt  */
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
@return Lower-case two-letter ISO-3166 code - http://www.ics.uci.edu/pub/ietf/http/related/iso639.txt  */
public String GetLanguageISO() 
{
return (String)Get_Value("LanguageISO");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name != null && Name.Length > 60)
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
