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
/** Generated Model for AD_Preference
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Preference : PO
{
public X_AD_Preference (Context ctx, int AD_Preference_ID, Trx trxName) : base (ctx, AD_Preference_ID, trxName)
{
/** if (AD_Preference_ID == 0)
{
SetAD_Preference_ID (0);
SetAttribute (null);
SetValue (null);
}
 */
}
public X_AD_Preference (Ctx ctx, int AD_Preference_ID, Trx trxName) : base (ctx, AD_Preference_ID, trxName)
{
/** if (AD_Preference_ID == 0)
{
SetAD_Preference_ID (0);
SetAttribute (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Preference (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Preference (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Preference (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Preference()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362502L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045713L;
/** AD_Table_ID=195 */
public static int Table_ID;
 // =195;

/** TableName=AD_Preference */
public static String Table_Name="AD_Preference";

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
StringBuilder sb = new StringBuilder ("X_AD_Preference[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Preference.
@param AD_Preference_ID Personal Value Preference */
public void SetAD_Preference_ID (int AD_Preference_ID)
{
if (AD_Preference_ID < 1) throw new ArgumentException ("AD_Preference_ID is mandatory.");
Set_ValueNoCheck ("AD_Preference_ID", AD_Preference_ID);
}
/** Get Preference.
@return Personal Value Preference */
public int GetAD_Preference_ID() 
{
Object ii = Get_Value("AD_Preference_ID");
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
if (AD_Window_ID <= 0) Set_Value ("AD_Window_ID", null);
else
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
/** Set Attribute.
@param Attribute Attribute */
public void SetAttribute (String Attribute)
{
if (Attribute == null) throw new ArgumentException ("Attribute is mandatory.");
if (Attribute.Length > 60)
{
log.Warning("Length > 60 - truncated");
Attribute = Attribute.Substring(0,60);
}
Set_Value ("Attribute", Attribute);
}
/** Get Attribute.
@return Attribute */
public String GetAttribute() 
{
return (String)Get_Value("Attribute");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAttribute());
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 60)
{
log.Warning("Length > 60 - truncated");
Value = Value.Substring(0,60);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}
