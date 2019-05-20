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
/** Generated Model for R_IssueUser
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_IssueUser : PO
{
public X_R_IssueUser (Context ctx, int R_IssueUser_ID, Trx trxName) : base (ctx, R_IssueUser_ID, trxName)
{
/** if (R_IssueUser_ID == 0)
{
SetR_IssueUser_ID (0);
SetUserName (null);
}
 */
}
public X_R_IssueUser (Ctx ctx, int R_IssueUser_ID, Trx trxName) : base (ctx, R_IssueUser_ID, trxName)
{
/** if (R_IssueUser_ID == 0)
{
SetR_IssueUser_ID (0);
SetUserName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueUser (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueUser (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_IssueUser (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_IssueUser()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382954L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066165L;
/** AD_Table_ID=841 */
public static int Table_ID;
 // =841;

/** TableName=R_IssueUser */
public static String Table_Name="R_IssueUser";

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
StringBuilder sb = new StringBuilder ("X_R_IssueUser[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Issue User.
@param R_IssueUser_ID User who reported issues */
public void SetR_IssueUser_ID (int R_IssueUser_ID)
{
if (R_IssueUser_ID < 1) throw new ArgumentException ("R_IssueUser_ID is mandatory.");
Set_ValueNoCheck ("R_IssueUser_ID", R_IssueUser_ID);
}
/** Get Issue User.
@return User who reported issues */
public int GetR_IssueUser_ID() 
{
Object ii = Get_Value("R_IssueUser_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Registered EMail.
@param UserName Email of the responsible for the System */
public void SetUserName (String UserName)
{
if (UserName == null) throw new ArgumentException ("UserName is mandatory.");
if (UserName.Length > 60)
{
log.Warning("Length > 60 - truncated");
UserName = UserName.Substring(0,60);
}
Set_ValueNoCheck ("UserName", UserName);
}
/** Get Registered EMail.
@return Email of the responsible for the System */
public String GetUserName() 
{
return (String)Get_Value("UserName");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetUserName());
}
}

}
