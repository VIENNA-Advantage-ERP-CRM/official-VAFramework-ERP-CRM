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
/** Generated Model for DeletedUserLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_DeletedUserLog : PO
{
public X_DeletedUserLog (Context ctx, int DeletedUserLog_ID, Trx trxName) : base (ctx, DeletedUserLog_ID, trxName)
{
/** if (DeletedUserLog_ID == 0)
{
SetDeletedUserLog_ID (0);
}
 */
}
public X_DeletedUserLog (Ctx ctx, int DeletedUserLog_ID, Trx trxName) : base (ctx, DeletedUserLog_ID, trxName)
{
/** if (DeletedUserLog_ID == 0)
{
SetDeletedUserLog_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_DeletedUserLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_DeletedUserLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_DeletedUserLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_DeletedUserLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27626181111253L;
/** Last Updated Timestamp 8/4/2012 10:19:55 AM */
public static long updatedMS = 1344055794464L;
/** AD_Table_ID=1000339 */
public static int Table_ID;
 // =1000339;

/** TableName=DeletedUserLog */
public static String Table_Name="DeletedUserLog";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_DeletedUserLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set DeletedUserLog_ID.
@param DeletedUserLog_ID DeletedUserLog_ID */
public void SetDeletedUserLog_ID (int DeletedUserLog_ID)
{
if (DeletedUserLog_ID < 1) throw new ArgumentException ("DeletedUserLog_ID is mandatory.");
Set_ValueNoCheck ("DeletedUserLog_ID", DeletedUserLog_ID);
}
/** Get DeletedUserLog_ID.
@return DeletedUserLog_ID */
public int GetDeletedUserLog_ID() 
{
Object ii = Get_Value("DeletedUserLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Deleted From Gmail.
@param IsDeletedFromGmail Deleted From Gmail */
public void SetIsDeletedFromGmail (Boolean IsDeletedFromGmail)
{
Set_Value ("IsDeletedFromGmail", IsDeletedFromGmail);
}
/** Get Deleted From Gmail.
@return Deleted From Gmail */
public Boolean IsDeletedFromGmail() 
{
Object oo = Get_Value("IsDeletedFromGmail");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
