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
/** Generated Model for AD_Replication_Log
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Replication_Log : PO
{
public X_AD_Replication_Log (Context ctx, int AD_Replication_Log_ID, Trx trxName) : base (ctx, AD_Replication_Log_ID, trxName)
{
/** if (AD_Replication_Log_ID == 0)
{
SetAD_Replication_Log_ID (0);
SetAD_Replication_Run_ID (0);
SetIsReplicated (false);	// N
}
 */
}
public X_AD_Replication_Log (Ctx ctx, int AD_Replication_Log_ID, Trx trxName) : base (ctx, AD_Replication_Log_ID, trxName)
{
/** if (AD_Replication_Log_ID == 0)
{
SetAD_Replication_Log_ID (0);
SetAD_Replication_Run_ID (0);
SetIsReplicated (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication_Log (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication_Log (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication_Log (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Replication_Log()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363552L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046763L;
/** AD_Table_ID=604 */
public static int Table_ID;
 // =604;

/** TableName=AD_Replication_Log */
public static String Table_Name="AD_Replication_Log";

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
StringBuilder sb = new StringBuilder ("X_AD_Replication_Log[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Replication Table.
@param AD_ReplicationTable_ID Data Replication Strategy Table Info */
public void SetAD_ReplicationTable_ID (int AD_ReplicationTable_ID)
{
if (AD_ReplicationTable_ID <= 0) Set_Value ("AD_ReplicationTable_ID", null);
else
Set_Value ("AD_ReplicationTable_ID", AD_ReplicationTable_ID);
}
/** Get Replication Table.
@return Data Replication Strategy Table Info */
public int GetAD_ReplicationTable_ID() 
{
Object ii = Get_Value("AD_ReplicationTable_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Replication Log.
@param AD_Replication_Log_ID Data Replication Log Details */
public void SetAD_Replication_Log_ID (int AD_Replication_Log_ID)
{
if (AD_Replication_Log_ID < 1) throw new ArgumentException ("AD_Replication_Log_ID is mandatory.");
Set_ValueNoCheck ("AD_Replication_Log_ID", AD_Replication_Log_ID);
}
/** Get Replication Log.
@return Data Replication Log Details */
public int GetAD_Replication_Log_ID() 
{
Object ii = Get_Value("AD_Replication_Log_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Replication Run.
@param AD_Replication_Run_ID Data Replication Run */
public void SetAD_Replication_Run_ID (int AD_Replication_Run_ID)
{
if (AD_Replication_Run_ID < 1) throw new ArgumentException ("AD_Replication_Run_ID is mandatory.");
Set_ValueNoCheck ("AD_Replication_Run_ID", AD_Replication_Run_ID);
}
/** Get Replication Run.
@return Data Replication Run */
public int GetAD_Replication_Run_ID() 
{
Object ii = Get_Value("AD_Replication_Run_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Replication_Run_ID().ToString());
}
/** Set Replicated.
@param IsReplicated The data is successfully replicated */
public void SetIsReplicated (Boolean IsReplicated)
{
Set_Value ("IsReplicated", IsReplicated);
}
/** Get Replicated.
@return The data is successfully replicated */
public Boolean IsReplicated() 
{
Object oo = Get_Value("IsReplicated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Process Message.
@param P_Msg Process Message */
public void SetP_Msg (String P_Msg)
{
if (P_Msg != null && P_Msg.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
P_Msg = P_Msg.Substring(0,2000);
}
Set_Value ("P_Msg", P_Msg);
}
/** Get Process Message.
@return Process Message */
public String GetP_Msg() 
{
return (String)Get_Value("P_Msg");
}
}

}
