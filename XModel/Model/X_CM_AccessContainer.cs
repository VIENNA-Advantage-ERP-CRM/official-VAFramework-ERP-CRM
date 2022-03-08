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
/** Generated Model for CM_AccessContainer
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_AccessContainer : PO
{
public X_CM_AccessContainer (Context ctx, int CM_AccessContainer_ID, Trx trxName) : base (ctx, CM_AccessContainer_ID, trxName)
{
/** if (CM_AccessContainer_ID == 0)
{
SetCM_AccessProfile_ID (0);
SetCM_Container_ID (0);
}
 */
}
public X_CM_AccessContainer (Ctx ctx, int CM_AccessContainer_ID, Trx trxName) : base (ctx, CM_AccessContainer_ID, trxName)
{
/** if (CM_AccessContainer_ID == 0)
{
SetCM_AccessProfile_ID (0);
SetCM_Container_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessContainer (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessContainer (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_AccessContainer (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_AccessContainer()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367705L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050916L;
/** AD_Table_ID=888 */
public static int Table_ID;
 // =888;

/** TableName=CM_AccessContainer */
public static String Table_Name="CM_AccessContainer";

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
StringBuilder sb = new StringBuilder ("X_CM_AccessContainer[").Append(Get_ID()).Append("]");
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
/** Set Web Container.
@param CM_Container_ID Web Container contains content like images, text etc. */
public void SetCM_Container_ID (int CM_Container_ID)
{
if (CM_Container_ID < 1) throw new ArgumentException ("CM_Container_ID is mandatory.");
Set_ValueNoCheck ("CM_Container_ID", CM_Container_ID);
}
/** Get Web Container.
@return Web Container contains content like images, text etc. */
public int GetCM_Container_ID() 
{
Object ii = Get_Value("CM_Container_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
