namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Classes;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for VAF_DBQueryLog
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_DBQueryLog : PO
{
public X_VAF_DBQueryLog (Context ctx, int VAF_DBQueryLog_ID, Trx trxName) : base (ctx, VAF_DBQueryLog_ID, trxName)
{
/** if (VAF_DBQueryLog_ID == 0)
{
SetVAF_DBQueryLog_ID (0);
SetVAF_Session_ID (0);
SetRecordCount (0);
SetWhereClause (null);
}
 */
}
public X_VAF_DBQueryLog (Ctx ctx, int VAF_DBQueryLog_ID, Trx trxName) : base (ctx, VAF_DBQueryLog_ID, trxName)
{
/** if (VAF_DBQueryLog_ID == 0)
{
SetVAF_DBQueryLog_ID (0);
SetVAF_Session_ID (0);
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
public X_VAF_DBQueryLog (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_DBQueryLog (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_DBQueryLog (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_DBQueryLog()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363269L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046480L;
/** VAF_TableView_ID=942 */
public static int Table_ID;
 // =942;

/** TableName=VAF_DBQueryLog */
public static String Table_Name="VAF_DBQueryLog";

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
StringBuilder sb = new StringBuilder ("X_VAF_DBQueryLog[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Query Log.
@param VAF_DBQueryLog_ID Database query log */
public void SetVAF_DBQueryLog_ID (int VAF_DBQueryLog_ID)
{
if (VAF_DBQueryLog_ID < 1) throw new ArgumentException ("VAF_DBQueryLog_ID is mandatory.");
Set_ValueNoCheck ("VAF_DBQueryLog_ID", VAF_DBQueryLog_ID);
}
/** Get Query Log.
@return Database query log */
public int GetVAF_DBQueryLog_ID() 
{
Object ii = Get_Value("VAF_DBQueryLog_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_DBQueryLog_ID().ToString());
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
/** Set Session.
@param VAF_Session_ID User Session Online or Web */
public void SetVAF_Session_ID (int VAF_Session_ID)
{
if (VAF_Session_ID < 1) throw new ArgumentException ("VAF_Session_ID is mandatory.");
Set_ValueNoCheck ("VAF_Session_ID", VAF_Session_ID);
}
/** Get Session.
@return User Session Online or Web */
public int GetVAF_Session_ID() 
{
Object ii = Get_Value("VAF_Session_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);
else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
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
