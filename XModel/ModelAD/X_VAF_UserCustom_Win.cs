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
/** Generated Model for VAF_UserCustom_Win
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_UserCustom_Win : PO
{
public X_VAF_UserCustom_Win (Context ctx, int VAF_UserCustom_Win_ID, Trx trxName) : base (ctx, VAF_UserCustom_Win_ID, trxName)
{
/** if (VAF_UserCustom_Win_ID == 0)
{
SetVAF_UserCustom_Win_ID (0);
SetVAF_Screen_ID (0);
SetCustomizationName (null);
}
 */
}
public X_VAF_UserCustom_Win (Ctx ctx, int VAF_UserCustom_Win_ID, Trx trxName) : base (ctx, VAF_UserCustom_Win_ID, trxName)
{
/** if (VAF_UserCustom_Win_ID == 0)
{
SetVAF_UserCustom_Win_ID (0);
SetVAF_Screen_ID (0);
SetCustomizationName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserCustom_Win (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserCustom_Win (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_UserCustom_Win (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_UserCustom_Win()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365385L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048596L;
/** VAF_TableView_ID=467 */
public static int Table_ID;
 // =467;

/** TableName=VAF_UserCustom_Win */
public static String Table_Name="VAF_UserCustom_Win";

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
StringBuilder sb = new StringBuilder ("X_VAF_UserCustom_Win[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_Language VAF_Control_Ref_ID=106 */
public static int VAF_LANGUAGE_VAF_Control_Ref_ID=106;
/** Set Language.
@param VAF_Language Language for this entity */
public void SetVAF_Language (String VAF_Language)
{
if (VAF_Language != null && VAF_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
VAF_Language = VAF_Language.Substring(0,5);
}
Set_Value ("VAF_Language", VAF_Language);
}
/** Get Language.
@return Language for this entity */
public String GetVAF_Language() 
{
return (String)Get_Value("VAF_Language");
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID <= 0) Set_Value ("VAF_Role_ID", null);
else
Set_Value ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User defined Window.
@param VAF_UserCustom_Win_ID User defined Window */
public void SetVAF_UserCustom_Win_ID (int VAF_UserCustom_Win_ID)
{
if (VAF_UserCustom_Win_ID < 1) throw new ArgumentException ("VAF_UserCustom_Win_ID is mandatory.");
Set_ValueNoCheck ("VAF_UserCustom_Win_ID", VAF_UserCustom_Win_ID);
}
/** Get User defined Window.
@return User defined Window */
public int GetVAF_UserCustom_Win_ID() 
{
Object ii = Get_Value("VAF_UserCustom_Win_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param VAF_Screen_ID Data entry or display window */
public void SetVAF_Screen_ID (int VAF_Screen_ID)
{
if (VAF_Screen_ID < 1) throw new ArgumentException ("VAF_Screen_ID is mandatory.");
Set_Value ("VAF_Screen_ID", VAF_Screen_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetVAF_Screen_ID() 
{
Object ii = Get_Value("VAF_Screen_ID");
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

/** IsDefault VAF_Control_Ref_ID=319 */
public static int ISDEFAULT_VAF_Control_Ref_ID=319;
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

/** IsReadOnly VAF_Control_Ref_ID=319 */
public static int ISREADONLY_VAF_Control_Ref_ID=319;
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
