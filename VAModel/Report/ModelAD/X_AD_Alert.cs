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
/** Generated Model for AD_Alert
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Alert : PO
{
public X_AD_Alert (Context ctx, int AD_Alert_ID, Trx trxName) : base (ctx, AD_Alert_ID, trxName)
{
/** if (AD_Alert_ID == 0)
{
SetAD_AlertProcessor_ID (0);
SetAD_Alert_ID (0);
SetAlertMessage (null);
SetAlertSubject (null);
SetEnforceClientSecurity (true);	// Y
SetEnforceRoleSecurity (true);	// Y
SetIsValid (true);	// Y
SetName (null);
}
 */
}
public X_AD_Alert (Ctx ctx, int AD_Alert_ID, Trx trxName) : base (ctx, AD_Alert_ID, trxName)
{
/** if (AD_Alert_ID == 0)
{
SetAD_AlertProcessor_ID (0);
SetAD_Alert_ID (0);
SetAlertMessage (null);
SetAlertSubject (null);
SetEnforceClientSecurity (true);	// Y
SetEnforceRoleSecurity (true);	// Y
SetIsValid (true);	// Y
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Alert (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Alert (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Alert (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Alert()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360213L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043424L;
/** AD_Table_ID=594 */
public static int Table_ID;
 // =594;

/** TableName=AD_Alert */
public static String Table_Name="AD_Alert";

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
StringBuilder sb = new StringBuilder ("X_AD_Alert[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Alert Processor.
@param AD_AlertProcessor_ID Alert Processor/Server Parameter */
public void SetAD_AlertProcessor_ID (int AD_AlertProcessor_ID)
{
if (AD_AlertProcessor_ID < 1) throw new ArgumentException ("AD_AlertProcessor_ID is mandatory.");
Set_Value ("AD_AlertProcessor_ID", AD_AlertProcessor_ID);
}
/** Get Alert Processor.
@return Alert Processor/Server Parameter */
public int GetAD_AlertProcessor_ID() 
{
Object ii = Get_Value("AD_AlertProcessor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Alert.
@param AD_Alert_ID Vienna Alert */
public void SetAD_Alert_ID (int AD_Alert_ID)
{
if (AD_Alert_ID < 1) throw new ArgumentException ("AD_Alert_ID is mandatory.");
Set_ValueNoCheck ("AD_Alert_ID", AD_Alert_ID);
}
/** Get Alert.
@return Vienna Alert */
public int GetAD_Alert_ID() 
{
Object ii = Get_Value("AD_Alert_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Alert Message.
@param AlertMessage Message of the Alert */
public void SetAlertMessage (String AlertMessage)
{
if (AlertMessage == null) throw new ArgumentException ("AlertMessage is mandatory.");
if (AlertMessage.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
AlertMessage = AlertMessage.Substring(0,2000);
}
Set_Value ("AlertMessage", AlertMessage);
}
/** Get Alert Message.
@return Message of the Alert */
public String GetAlertMessage() 
{
return (String)Get_Value("AlertMessage");
}
/** Set Alert Subject.
@param AlertSubject Subject of the Alert */
public void SetAlertSubject (String AlertSubject)
{
if (AlertSubject == null) throw new ArgumentException ("AlertSubject is mandatory.");
if (AlertSubject.Length > 60)
{
log.Warning("Length > 60 - truncated");
AlertSubject = AlertSubject.Substring(0,60);
}
Set_Value ("AlertSubject", AlertSubject);
}
/** Get Alert Subject.
@return Subject of the Alert */
public String GetAlertSubject() 
{
return (String)Get_Value("AlertSubject");
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
/** Set Enforce Tenant Security.
@param EnforceClientSecurity Send alerts to recipient only if the Tenant security rules of the role allows */
public void SetEnforceClientSecurity (Boolean EnforceClientSecurity)
{
Set_Value ("EnforceClientSecurity", EnforceClientSecurity);
}
/** Get Enforce Tenant Security.
@return Send alerts to recipient only if the Tenant security rules of the role allows */
public Boolean IsEnforceClientSecurity() 
{
Object oo = Get_Value("EnforceClientSecurity");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Enforce Role Security.
@param EnforceRoleSecurity Send alerts to recipient only if the data security rules of the role allows */
public void SetEnforceRoleSecurity (Boolean EnforceRoleSecurity)
{
Set_Value ("EnforceRoleSecurity", EnforceRoleSecurity);
}
/** Get Enforce Role Security.
@return Send alerts to recipient only if the data security rules of the role allows */
public Boolean IsEnforceRoleSecurity() 
{
Object oo = Get_Value("EnforceRoleSecurity");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
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
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
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
}

}
