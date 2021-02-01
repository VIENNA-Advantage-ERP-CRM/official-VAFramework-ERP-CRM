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
/** Generated Model for VAF_ValuePreference
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ValuePreference : PO
{
public X_VAF_ValuePreference (Context ctx, int VAF_ValuePreference_ID, Trx trxName) : base (ctx, VAF_ValuePreference_ID, trxName)
{
/** if (VAF_ValuePreference_ID == 0)
{
SetVAF_ValuePreference_ID (0);
SetAttribute (null);
SetValue (null);
}
 */
}
public X_VAF_ValuePreference (Ctx ctx, int VAF_ValuePreference_ID, Trx trxName) : base (ctx, VAF_ValuePreference_ID, trxName)
{
/** if (VAF_ValuePreference_ID == 0)
{
SetVAF_ValuePreference_ID (0);
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
public X_VAF_ValuePreference (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ValuePreference (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ValuePreference (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ValuePreference()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362502L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045713L;
/** VAF_TableView_ID=195 */
public static int Table_ID;
 // =195;

/** TableName=VAF_ValuePreference */
public static String Table_Name="VAF_ValuePreference";

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
StringBuilder sb = new StringBuilder ("X_VAF_ValuePreference[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Preference.
@param VAF_ValuePreference_ID Personal Value Preference */
public void SetVAF_ValuePreference_ID (int VAF_ValuePreference_ID)
{
if (VAF_ValuePreference_ID < 1) throw new ArgumentException ("VAF_ValuePreference_ID is mandatory.");
Set_ValueNoCheck ("VAF_ValuePreference_ID", VAF_ValuePreference_ID);
}
/** Get Preference.
@return Personal Value Preference */
public int GetVAF_ValuePreference_ID() 
{
Object ii = Get_Value("VAF_ValuePreference_ID");
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
if (VAF_Screen_ID <= 0) Set_Value ("VAF_Screen_ID", null);
else
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
