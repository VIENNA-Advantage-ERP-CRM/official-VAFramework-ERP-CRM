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
/** Generated Model for AD_PInstance_Log
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PInstance_Log : PO
{
public X_AD_PInstance_Log (Context ctx, int AD_PInstance_Log_ID, Trx trxName) : base (ctx, AD_PInstance_Log_ID, trxName)
{
/** if (AD_PInstance_Log_ID == 0)
{
SetAD_PInstance_ID (0);
SetLog_ID (0);
}
 */
}
public X_AD_PInstance_Log (Ctx ctx, int AD_PInstance_Log_ID, Trx trxName) : base (ctx, AD_PInstance_Log_ID, trxName)
{
/** if (AD_PInstance_Log_ID == 0)
{
SetAD_PInstance_ID (0);
SetLog_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PInstance_Log (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PInstance_Log (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PInstance_Log (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PInstance_Log()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362439L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045650L;
/** AD_Table_ID=578 */
public static int Table_ID;
 // =578;

/** TableName=AD_PInstance_Log */
public static String Table_Name="AD_PInstance_Log";

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
StringBuilder sb = new StringBuilder ("X_AD_PInstance_Log[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param AD_PInstance_ID Instance of the process */
public void SetAD_PInstance_ID (int AD_PInstance_ID)
{
if (AD_PInstance_ID < 1) throw new ArgumentException ("AD_PInstance_ID is mandatory.");
Set_ValueNoCheck ("AD_PInstance_ID", AD_PInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetAD_PInstance_ID() 
{
Object ii = Get_Value("AD_PInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_PInstance_ID().ToString());
}
/** Set Log.
@param Log_ID Log */
public void SetLog_ID (int Log_ID)
{
if (Log_ID < 1) throw new ArgumentException ("Log_ID is mandatory.");
Set_ValueNoCheck ("Log_ID", Log_ID);
}
/** Get Log.
@return Log */
public int GetLog_ID() 
{
Object ii = Get_Value("Log_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process Date.
@param P_Date Process Parameter */
public void SetP_Date (DateTime? P_Date)
{
Set_ValueNoCheck ("P_Date", (DateTime?)P_Date);
}
/** Get Process Date.
@return Process Parameter */
public DateTime? GetP_Date() 
{
return (DateTime?)Get_Value("P_Date");
}
/** Set Process ID.
@param P_ID Process ID */
public void SetP_ID (int P_ID)
{
if (P_ID <= 0) Set_ValueNoCheck ("P_ID", null);
else
Set_ValueNoCheck ("P_ID", P_ID);
}
/** Get Process ID.
@return Process ID */
public int GetP_ID() 
{
Object ii = Get_Value("P_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
Set_ValueNoCheck ("P_Msg", P_Msg);
}
/** Get Process Message.
@return Process Message */
public String GetP_Msg() 
{
return (String)Get_Value("P_Msg");
}
/** Set Process Number.
@param P_Number Process Parameter */
public void SetP_Number (Decimal? P_Number)
{
Set_ValueNoCheck ("P_Number", (Decimal?)P_Number);
}
/** Get Process Number.
@return Process Parameter */
public Decimal GetP_Number() 
{
Object bd =Get_Value("P_Number");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
