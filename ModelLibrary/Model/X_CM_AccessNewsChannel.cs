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
/** Generated Model for CM_AccessNewsChannel
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_AccessNewsChannel : PO
{
public X_CM_AccessNewsChannel (Context ctx, int CM_AccessNewsChannel_ID, Trx trxName) : base (ctx, CM_AccessNewsChannel_ID, trxName)
{
/** if (CM_AccessNewsChannel_ID == 0)
{
SetCM_AccessProfile_ID (0);
SetCM_NewsChannel_ID (0);
}
 */
}
public X_CM_AccessNewsChannel (Ctx ctx, int CM_AccessNewsChannel_ID, Trx trxName) : base (ctx, CM_AccessNewsChannel_ID, trxName)
{
/** if (CM_AccessNewsChannel_ID == 0)
{
SetCM_AccessProfile_ID (0);
SetCM_NewsChannel_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessNewsChannel (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessNewsChannel (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessNewsChannel (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_AccessNewsChannel()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368065L;
/** Last Updated Timestamp 7/29/2010 1:07:31 PM */
public static long updatedMS = 1280389051276L;
/** AD_Table_ID=891 */
public static int Table_ID;
 // =891;

/** TableName=CM_AccessNewsChannel */
public static String Table_Name="CM_AccessNewsChannel";

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
StringBuilder sb = new StringBuilder ("X_CM_AccessNewsChannel[").Append(Get_ID()).Append("]");
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
/** Set News Channel.
@param CM_NewsChannel_ID News channel for rss feed */
public void SetCM_NewsChannel_ID (int CM_NewsChannel_ID)
{
if (CM_NewsChannel_ID < 1) throw new ArgumentException ("CM_NewsChannel_ID is mandatory.");
Set_ValueNoCheck ("CM_NewsChannel_ID", CM_NewsChannel_ID);
}
/** Get News Channel.
@return News channel for rss feed */
public int GetCM_NewsChannel_ID() 
{
Object ii = Get_Value("CM_NewsChannel_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
