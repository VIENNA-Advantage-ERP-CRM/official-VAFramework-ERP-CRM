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
/** Generated Model for AD_UserPreference
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_UserPreference : PO
{
public X_AD_UserPreference (Context ctx, int AD_UserPreference_ID, Trx trxName) : base (ctx, AD_UserPreference_ID, trxName)
{
/** if (AD_UserPreference_ID == 0)
{
SetAD_UserPreference_ID (0);
SetAD_User_ID (0);
}
 */
}
public X_AD_UserPreference (Ctx ctx, int AD_UserPreference_ID, Trx trxName) : base (ctx, AD_UserPreference_ID, trxName)
{
/** if (AD_UserPreference_ID == 0)
{
SetAD_UserPreference_ID (0);
SetAD_User_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserPreference (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserPreference (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_UserPreference (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_UserPreference()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365542L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048753L;
/** AD_Table_ID=989 */
public static int Table_ID;
 // =989;

/** TableName=AD_UserPreference */
public static String Table_Name="AD_UserPreference";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_AD_UserPreference[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User Preference.
@param AD_UserPreference_ID Application User Preferences */
public void SetAD_UserPreference_ID (int AD_UserPreference_ID)
{
if (AD_UserPreference_ID < 1) throw new ArgumentException ("AD_UserPreference_ID is mandatory.");
Set_ValueNoCheck ("AD_UserPreference_ID", AD_UserPreference_ID);
}
/** Get User Preference.
@return Application User Preferences */
public int GetAD_UserPreference_ID() 
{
Object ii = Get_Value("AD_UserPreference_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID < 1) throw new ArgumentException ("AD_User_ID is mandatory.");
Set_ValueNoCheck ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Auto Commit.
@param IsAutoCommit Automatically save changes */
public void SetIsAutoCommit (Boolean IsAutoCommit)
{
Set_Value ("IsAutoCommit", IsAutoCommit);
}
/** Get Auto Commit.
@return Automatically save changes */
public Boolean IsAutoCommit() 
{
Object oo = Get_Value("IsAutoCommit");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Show Accounting.
@param IsShowAcct Users with this role can see accounting information */
public void SetIsShowAcct (Boolean IsShowAcct)
{
Set_Value ("IsShowAcct", IsShowAcct);
}
/** Get Show Accounting.
@return Users with this role can see accounting information */
public Boolean IsShowAcct() 
{
Object oo = Get_Value("IsShowAcct");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Show Advanced.
@param IsShowAdvanced Show Advanced Tabs */
public void SetIsShowAdvanced (Boolean IsShowAdvanced)
{
Set_Value ("IsShowAdvanced", IsShowAdvanced);
}
/** Get Show Advanced.
@return Show Advanced Tabs */
public Boolean IsShowAdvanced() 
{
Object oo = Get_Value("IsShowAdvanced");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Show Translation.
@param IsShowTrl Show Translation Tabs */
public void SetIsShowTrl (Boolean IsShowTrl)
{
Set_Value ("IsShowTrl", IsShowTrl);
}
/** Get Show Translation.
@return Show Translation Tabs */
public Boolean IsShowTrl() 
{
Object oo = Get_Value("IsShowTrl");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Printer Name.
@param PrinterName Name of the Printer */
public void SetPrinterName (String PrinterName)
{
if (PrinterName != null && PrinterName.Length > 60)
{
log.Warning("Length > 60 - truncated");
PrinterName = PrinterName.Substring(0,60);
}
Set_Value ("PrinterName", PrinterName);
}
/** Get Printer Name.
@return Name of the Printer */
public String GetPrinterName() 
{
return (String)Get_Value("PrinterName");
}
/** Set UI Theme.
@param UITheme User Interface Theme */
public void SetUITheme (String UITheme)
{
if (UITheme != null && UITheme.Length > 60)
{
log.Warning("Length > 60 - truncated");
UITheme = UITheme.Substring(0,60);
}
Set_Value ("UITheme", UITheme);
}
/** Get UI Theme.
@return User Interface Theme */
public String GetUITheme() 
{
return (String)Get_Value("UITheme");
}
}

}
