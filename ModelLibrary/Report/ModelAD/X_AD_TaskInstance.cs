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
/** Generated Model for AD_TaskInstance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_TaskInstance : PO
{
public X_AD_TaskInstance (Context ctx, int AD_TaskInstance_ID, Trx trxName) : base (ctx, AD_TaskInstance_ID, trxName)
{
/** if (AD_TaskInstance_ID == 0)
{
SetAD_TaskInstance_ID (0);
SetAD_Task_ID (0);
}
 */
}
public X_AD_TaskInstance (Ctx ctx, int AD_TaskInstance_ID, Trx trxName) : base (ctx, AD_TaskInstance_ID, trxName)
{
/** if (AD_TaskInstance_ID == 0)
{
SetAD_TaskInstance_ID (0);
SetAD_Task_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TaskInstance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TaskInstance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_TaskInstance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_TaskInstance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364508L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047719L;
/** AD_Table_ID=125 */
public static int Table_ID;
 // =125;

/** TableName=AD_TaskInstance */
public static String Table_Name="AD_TaskInstance";

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
StringBuilder sb = new StringBuilder ("X_AD_TaskInstance[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Task Instance.
@param AD_TaskInstance_ID Task Instance */
public void SetAD_TaskInstance_ID (int AD_TaskInstance_ID)
{
if (AD_TaskInstance_ID < 1) throw new ArgumentException ("AD_TaskInstance_ID is mandatory.");
Set_ValueNoCheck ("AD_TaskInstance_ID", AD_TaskInstance_ID);
}
/** Get Task Instance.
@return Task Instance */
public int GetAD_TaskInstance_ID() 
{
Object ii = Get_Value("AD_TaskInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_TaskInstance_ID().ToString());
}
/** Set OS Task.
@param AD_Task_ID Operation System Task */
public void SetAD_Task_ID (int AD_Task_ID)
{
if (AD_Task_ID < 1) throw new ArgumentException ("AD_Task_ID is mandatory.");
Set_Value ("AD_Task_ID", AD_Task_ID);
}
/** Get OS Task.
@return Operation System Task */
public int GetAD_Task_ID() 
{
Object ii = Get_Value("AD_Task_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
