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
/** Generated Model for VAF_AlertRecipient
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_AlertRecipient : PO
{
public X_VAF_AlertRecipient (Context ctx, int VAF_AlertRecipient_ID, Trx trxName) : base (ctx, VAF_AlertRecipient_ID, trxName)
{
/** if (VAF_AlertRecipient_ID == 0)
{
SetVAF_AlertRecipient_ID (0);
SetVAF_Alert_ID (0);
}
 */
}
public X_VAF_AlertRecipient (Ctx ctx, int VAF_AlertRecipient_ID, Trx trxName) : base (ctx, VAF_AlertRecipient_ID, trxName)
{
/** if (VAF_AlertRecipient_ID == 0)
{
SetVAF_AlertRecipient_ID (0);
SetVAF_Alert_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AlertRecipient (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AlertRecipient (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_AlertRecipient (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_AlertRecipient()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360386L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043597L;
/** VAF_TableView_ID=592 */
public static int Table_ID;
 // =592;

/** TableName=VAF_AlertRecipient */
public static String Table_Name="VAF_AlertRecipient";

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
StringBuilder sb = new StringBuilder ("X_VAF_AlertRecipient[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Alert Recipient.
@param VAF_AlertRecipient_ID Recipient of the Alert Notification */
public void SetVAF_AlertRecipient_ID (int VAF_AlertRecipient_ID)
{
if (VAF_AlertRecipient_ID < 1) throw new ArgumentException ("VAF_AlertRecipient_ID is mandatory.");
Set_ValueNoCheck ("VAF_AlertRecipient_ID", VAF_AlertRecipient_ID);
}
/** Get Alert Recipient.
@return Recipient of the Alert Notification */
public int GetVAF_AlertRecipient_ID() 
{
Object ii = Get_Value("VAF_AlertRecipient_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Alert.
@param VAF_Alert_ID Vienna Alert */
public void SetVAF_Alert_ID (int VAF_Alert_ID)
{
if (VAF_Alert_ID < 1) throw new ArgumentException ("VAF_Alert_ID is mandatory.");
Set_ValueNoCheck ("VAF_Alert_ID", VAF_Alert_ID);
}
/** Get Alert.
@return Vienna Alert */
public int GetVAF_Alert_ID() 
{
Object ii = Get_Value("VAF_Alert_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
