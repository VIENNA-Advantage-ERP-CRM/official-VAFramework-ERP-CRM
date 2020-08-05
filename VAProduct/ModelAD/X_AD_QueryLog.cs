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
/** Generated Model for AD_QueryLog
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_QueryLog : PO
{
public X_AD_QueryLog (Context ctx, int AD_QueryLog_ID, Trx trxName) : base (ctx, AD_QueryLog_ID, trxName)
{
/** if (AD_QueryLog_ID == 0)
{
SetAD_QueryLog_ID (0);
SetAD_Session_ID (0);
SetRecordCount (0);
SetWhereClause (null);
}
 */
}
public X_AD_QueryLog (Ctx ctx, int AD_QueryLog_ID, Trx trxName) : base (ctx, AD_QueryLog_ID, trxName)
{
/** if (AD_QueryLog_ID == 0)
{
SetAD_QueryLog_ID (0);
SetAD_Session_ID (0);
SetRecordCount (0);
SetWhereClause (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_QueryLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_QueryLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_QueryLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_QueryLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363269L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046480L;
/** AD_Table_ID=942 */
public static int Table_ID;
 // =942;

/** TableName=AD_QueryLog */
public static String Table_Name="AD_QueryLog";

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
StringBuilder sb = new StringBuilder ("X_AD_QueryLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Query Log.
@param AD_QueryLog_ID Database query log */
public void SetAD_QueryLog_ID (int AD_QueryLog_ID)
{
if (AD_QueryLog_ID < 1) throw new ArgumentException ("AD_QueryLog_ID is mandatory.");
Set_ValueNoCheck ("AD_QueryLog_ID", AD_QueryLog_ID);
}
/** Get Query Log.
@return Database query log */
public int GetAD_QueryLog_ID() 
{
Object ii = Get_Value("AD_QueryLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_QueryLog_ID().ToString());
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
/** Set Session.
@param AD_Session_ID User Session Online or Web */
public void SetAD_Session_ID (int AD_Session_ID)
{
if (AD_Session_ID < 1) throw new ArgumentException ("AD_Session_ID is mandatory.");
Set_ValueNoCheck ("AD_Session_ID", AD_Session_ID);
}
/** Get Session.
@return User Session Online or Web */
public int GetAD_Session_ID() 
{
Object ii = Get_Value("AD_Session_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Parameter.
@param Parameter Parameter */
public void SetParameter (String Parameter)
{
if (Parameter != null && Parameter.Length > 60)
{
log.Warning("Length > 60 - truncated");
Parameter = Parameter.Substring(0,60);
}
Set_Value ("Parameter", Parameter);
}
/** Get Parameter.
@return Parameter */
public String GetParameter() 
{
return (String)Get_Value("Parameter");
}
/** Set Record Count.
@param RecordCount Number of Records */
public void SetRecordCount (int RecordCount)
{
Set_Value ("RecordCount", RecordCount);
}
/** Get Record Count.
@return Number of Records */
public int GetRecordCount() 
{
Object ii = Get_Value("RecordCount");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sql WHERE.
@param WhereClause Fully qualified SQL WHERE clause */
public void SetWhereClause (String WhereClause)
{
if (WhereClause == null) throw new ArgumentException ("WhereClause is mandatory.");
if (WhereClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WhereClause = WhereClause.Substring(0,2000);
}
Set_Value ("WhereClause", WhereClause);
}
/** Get Sql WHERE.
@return Fully qualified SQL WHERE clause */
public String GetWhereClause() 
{
return (String)Get_Value("WhereClause");
}
}

}
