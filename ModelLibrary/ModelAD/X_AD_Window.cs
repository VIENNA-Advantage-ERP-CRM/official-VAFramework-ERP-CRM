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
/** Generated Model for AD_Window
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Window : PO
{
public X_AD_Window (Context ctx, int AD_Window_ID, Trx trxName) : base (ctx, AD_Window_ID, trxName)
{
/** if (AD_Window_ID == 0)
{
SetAD_Window_ID (0);
SetEntityType (null);	// U
SetIsBetaFunctionality (false);
SetIsDefault (false);
SetName (null);
SetWindowType (null);	// M
}
 */
}
public X_AD_Window (Ctx ctx, int AD_Window_ID, Trx trxName) : base (ctx, AD_Window_ID, trxName)
{
/** if (AD_Window_ID == 0)
{
SetAD_Window_ID (0);
SetEntityType (null);	// U
SetIsBetaFunctionality (false);
SetIsDefault (false);
SetName (null);
SetWindowType (null);	// M
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Window (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Window (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Window (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Window()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514366388L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049599L;
/** AD_Table_ID=105 */
public static int Table_ID;
 // =105;

/** TableName=AD_Window */
public static String Table_Name="AD_Window";

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
StringBuilder sb = new StringBuilder ("X_AD_Window[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Color.
@param AD_Color_ID Color for backgrounds or indicators */
public void SetAD_Color_ID (int AD_Color_ID)
{
if (AD_Color_ID <= 0) Set_Value ("AD_Color_ID", null);
else
Set_Value ("AD_Color_ID", AD_Color_ID);
}
/** Get System Color.
@return Color for backgrounds or indicators */
public int GetAD_Color_ID() 
{
Object ii = Get_Value("AD_Color_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Context Area.
@param AD_CtxArea_ID Business Domain Area Terminology */
public void SetAD_CtxArea_ID (int AD_CtxArea_ID)
{
if (AD_CtxArea_ID <= 0) Set_Value ("AD_CtxArea_ID", null);
else
Set_Value ("AD_CtxArea_ID", AD_CtxArea_ID);
}
/** Get Context Area.
@return Business Domain Area Terminology */
public int GetAD_CtxArea_ID() 
{
Object ii = Get_Value("AD_CtxArea_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Image.
@param AD_Image_ID Image or Icon */
public void SetAD_Image_ID (int AD_Image_ID)
{
if (AD_Image_ID <= 0) Set_Value ("AD_Image_ID", null);
else
Set_Value ("AD_Image_ID", AD_Image_ID);
}
/** Get Image.
@return Image or Icon */
public int GetAD_Image_ID() 
{
Object ii = Get_Value("AD_Image_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID < 1) throw new ArgumentException ("AD_Window_ID is mandatory.");
Set_ValueNoCheck ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
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
/** Set Beta Functionality.
@param IsBetaFunctionality This functionality is considered Beta */
public void SetIsBetaFunctionality (Boolean IsBetaFunctionality)
{
Set_Value ("IsBetaFunctionality", IsBetaFunctionality);
}
/** Get Beta Functionality.
@return This functionality is considered Beta */
public Boolean IsBetaFunctionality() 
{
Object oo = Get_Value("IsBetaFunctionality");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Customization Default.
@param IsCustomDefault Default Customization */
public void SetIsCustomDefault (Boolean IsCustomDefault)
{
Set_Value ("IsCustomDefault", IsCustomDefault);
}
/** Get Customization Default.
@return Default Customization */
public Boolean IsCustomDefault() 
{
Object oo = Get_Value("IsCustomDefault");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
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

/** WindowType AD_Reference_ID=108 */
public static int WINDOWTYPE_AD_Reference_ID=108;
/** Maintain = M */
public static String WINDOWTYPE_Maintain = "M";
/** Query Only = Q */
public static String WINDOWTYPE_QueryOnly = "Q";
/** Single Record = S */
public static String WINDOWTYPE_SingleRecord = "S";
/** Transaction = T */
public static String WINDOWTYPE_Transaction = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsWindowTypeValid (String test)
{
return test.Equals("M") || test.Equals("Q") || test.Equals("S") || test.Equals("T");
}
/** Set WindowType.
@param WindowType Type or classification of a Window */
public void SetWindowType (String WindowType)
{
if (WindowType == null) throw new ArgumentException ("WindowType is mandatory");
if (!IsWindowTypeValid(WindowType))
throw new ArgumentException ("WindowType Invalid value - " + WindowType + " - Reference_ID=108 - M - Q - S - T");
if (WindowType.Length > 1)
{
log.Warning("Length > 1 - truncated");
WindowType = WindowType.Substring(0,1);
}
Set_Value ("WindowType", WindowType);
}
/** Get WindowType.
@return Type or classification of a Window */
public String GetWindowType() 
{
return (String)Get_Value("WindowType");
}
}

}
