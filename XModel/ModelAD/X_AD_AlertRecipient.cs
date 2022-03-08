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
/** Generated Model for AD_AlertRecipient
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_AlertRecipient : PO
{
public X_AD_AlertRecipient (Context ctx, int AD_AlertRecipient_ID, Trx trxName) : base (ctx, AD_AlertRecipient_ID, trxName)
{
/** if (AD_AlertRecipient_ID == 0)
{
SetAD_AlertRecipient_ID (0);
SetAD_Alert_ID (0);
}
 */
}
public X_AD_AlertRecipient (Ctx ctx, int AD_AlertRecipient_ID, Trx trxName) : base (ctx, AD_AlertRecipient_ID, trxName)
{
/** if (AD_AlertRecipient_ID == 0)
{
SetAD_AlertRecipient_ID (0);
SetAD_Alert_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AlertRecipient (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AlertRecipient (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_AlertRecipient (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_AlertRecipient()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360386L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043597L;
/** AD_Table_ID=592 */
public static int Table_ID;
 // =592;

/** TableName=AD_AlertRecipient */
public static String Table_Name="AD_AlertRecipient";

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
StringBuilder sb = new StringBuilder ("X_AD_AlertRecipient[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Alert Recipient.
@param AD_AlertRecipient_ID Recipient of the Alert Notification */
public void SetAD_AlertRecipient_ID (int AD_AlertRecipient_ID)
{
if (AD_AlertRecipient_ID < 1) throw new ArgumentException ("AD_AlertRecipient_ID is mandatory.");
Set_ValueNoCheck ("AD_AlertRecipient_ID", AD_AlertRecipient_ID);
}
/** Get Alert Recipient.
@return Recipient of the Alert Notification */
public int GetAD_AlertRecipient_ID() 
{
Object ii = Get_Value("AD_AlertRecipient_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_User_ID().ToString());
}
}

}
