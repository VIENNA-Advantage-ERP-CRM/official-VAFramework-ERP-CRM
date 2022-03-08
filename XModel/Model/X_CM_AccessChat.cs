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
/** Generated Model for CM_AccessChat
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_AccessChat : PO
{
public X_CM_AccessChat (Context ctx, int CM_AccessChat_ID, Trx trxName) : base (ctx, CM_AccessChat_ID, trxName)
{
/** if (CM_AccessChat_ID == 0)
{
SetCM_AccessProfile_ID (0);
SetCM_Chat_ID (0);
}
 */
}
public X_CM_AccessChat (Ctx ctx, int CM_AccessChat_ID, Trx trxName) : base (ctx, CM_AccessChat_ID, trxName)
{
/** if (CM_AccessChat_ID == 0)
{
SetCM_AccessProfile_ID (0);
SetCM_Chat_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessChat (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessChat (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessChat (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_AccessChat()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367626L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050837L;
/** AD_Table_ID=907 */
public static int Table_ID;
 // =907;

/** TableName=CM_AccessChat */
public static String Table_Name="CM_AccessChat";

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
StringBuilder sb = new StringBuilder ("X_CM_AccessChat[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Web Access Profile.
@param CM_AccessProfile_ID Web Access Profile */
public void SetCM_AccessProfile_ID (int CM_AccessProfile_ID)
{
if (CM_AccessProfile_ID < 1) throw new ArgumentException ("CM_AccessProfile_ID is mandatory.");
Set_ValueNoCheck ("CM_AccessProfile_ID", CM_AccessProfile_ID);
}
/** Get Web Access Profile.
@return Web Access Profile */
public int GetCM_AccessProfile_ID() 
{
Object ii = Get_Value("CM_AccessProfile_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetCM_AccessProfile_ID().ToString());
}
/** Set Chat.
@param CM_Chat_ID Chat or discussion thread */
public void SetCM_Chat_ID (int CM_Chat_ID)
{
if (CM_Chat_ID < 1) throw new ArgumentException ("CM_Chat_ID is mandatory.");
Set_ValueNoCheck ("CM_Chat_ID", CM_Chat_ID);
}
/** Get Chat.
@return Chat or discussion thread */
public int GetCM_Chat_ID() 
{
Object ii = Get_Value("CM_Chat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
