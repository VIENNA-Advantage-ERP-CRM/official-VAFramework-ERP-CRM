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
/** Generated Model for AD_UserDef_Win
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserDef_Win : PO
{
public X_AD_UserDef_Win (Context ctx, int AD_UserDef_Win_ID, Trx trxName) : base (ctx, AD_UserDef_Win_ID, trxName)
{
/** if (AD_UserDef_Win_ID == 0)
{
SetAD_UserDef_Win_ID (0);
SetAD_Window_ID (0);
SetCustomizationName (null);
}
 */
}
public X_AD_UserDef_Win (Ctx ctx, int AD_UserDef_Win_ID, Trx trxName) : base (ctx, AD_UserDef_Win_ID, trxName)
{
/** if (AD_UserDef_Win_ID == 0)
{
SetAD_UserDef_Win_ID (0);
SetAD_Window_ID (0);
SetCustomizationName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Win (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Win (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserDef_Win (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserDef_Win()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365385L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048596L;
/** AD_Table_ID=467 */
public static int Table_ID;
 // =467;

/** TableName=AD_UserDef_Win */
public static String Table_Name="AD_UserDef_Win";

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
StringBuilder sb = new StringBuilder ("X_AD_UserDef_Win[").Append(Get_ID()).Append("]");
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
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User defined Window.
@param AD_UserDef_Win_ID User defined Window */
public void SetAD_UserDef_Win_ID (int AD_UserDef_Win_ID)
{
if (AD_UserDef_Win_ID < 1) throw new ArgumentException ("AD_UserDef_Win_ID is mandatory.");
Set_ValueNoCheck ("AD_UserDef_Win_ID", AD_UserDef_Win_ID);
}
/** Get User defined Window.
@return User defined Window */
public int GetAD_UserDef_Win_ID() 
{
Object ii = Get_Value("AD_UserDef_Win_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID < 1) throw new ArgumentException ("AD_Window_ID is mandatory.");
Set_Value ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Customization Name.
@param CustomizationName Name of the customization */
public void SetCustomizationName (String CustomizationName)
{
if (CustomizationName == null) throw new ArgumentException ("CustomizationName is mandatory.");
if (CustomizationName.Length > 60)
{
log.Warning("Length > 60 - truncated");
CustomizationName = CustomizationName.Substring(0,60);
}
Set_Value ("CustomizationName", CustomizationName);
}
/** Get Customization Name.
@return Name of the customization */
public String GetCustomizationName() 
{
return (String)Get_Value("CustomizationName");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetCustomizationName());
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}

/** IsDefault AD_Reference_ID=319 */
public static int ISDEFAULT_AD_Reference_ID=319;
/** No = N */
public static String ISDEFAULT_No = "N";
/** Yes = Y */
public static String ISDEFAULT_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsDefaultValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (String IsDefault)
{
if (!IsIsDefaultValid(IsDefault))
throw new ArgumentException ("IsDefault Invalid value - " + IsDefault + " - Reference_ID=319 - N - Y");
if (IsDefault != null && IsDefault.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsDefault = IsDefault.Substring(0,1);
}
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public String GetIsDefault() 
{
return (String)Get_Value("IsDefault");
}

/** IsReadOnly AD_Reference_ID=319 */
public static int ISREADONLY_AD_Reference_ID=319;
/** No = N */
public static String ISREADONLY_No = "N";
/** Yes = Y */
public static String ISREADONLY_Yes = "Y";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsIsReadOnlyValid (String test)
{
return test == null || test.Equals("N") || test.Equals("Y");
}
/** Set Read Only.
@param IsReadOnly Field is read only */
public void SetIsReadOnly (String IsReadOnly)
{
if (!IsIsReadOnlyValid(IsReadOnly))
throw new ArgumentException ("IsReadOnly Invalid value - " + IsReadOnly + " - Reference_ID=319 - N - Y");
if (IsReadOnly != null && IsReadOnly.Length > 1)
{
log.Warning("Length > 1 - truncated");
IsReadOnly = IsReadOnly.Substring(0,1);
}
Set_Value ("IsReadOnly", IsReadOnly);
}
/** Get Read Only.
@return Field is read only */
public String GetIsReadOnly() 
{
return (String)Get_Value("IsReadOnly");
}
/** Set System Default.
@param IsSystemDefault System Default value */
public void SetIsSystemDefault (Boolean IsSystemDefault)
{
Set_Value ("IsSystemDefault", IsSystemDefault);
}
/** Get System Default.
@return System Default value */
public Boolean IsSystemDefault() 
{
Object oo = Get_Value("IsSystemDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set User updateable.
@param IsUserUpdateable The field can be updated by the user */
public void SetIsUserUpdateable (Boolean IsUserUpdateable)
{
Set_Value ("IsUserUpdateable", IsUserUpdateable);
}
/** Get User updateable.
@return The field can be updated by the user */
public Boolean IsUserUpdateable() 
{
Object oo = Get_Value("IsUserUpdateable");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Window Height.
@param WinHeight Window Height */
public void SetWinHeight (int WinHeight)
{
Set_Value ("WinHeight", WinHeight);
}
/** Get Window Height.
@return Window Height */
public int GetWinHeight() 
{
Object ii = Get_Value("WinHeight");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window Width.
@param WinWidth Window Width */
public void SetWinWidth (int WinWidth)
{
Set_Value ("WinWidth", WinWidth);
}
/** Get Window Width.
@return Window Width */
public int GetWinWidth() 
{
Object ii = Get_Value("WinWidth");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
