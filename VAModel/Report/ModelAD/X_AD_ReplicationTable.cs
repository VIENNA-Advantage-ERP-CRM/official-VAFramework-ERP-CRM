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
/** Generated Model for AD_ReplicationTable
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ReplicationTable : PO
{
public X_AD_ReplicationTable (Context ctx, int AD_ReplicationTable_ID, Trx trxName) : base (ctx, AD_ReplicationTable_ID, trxName)
{
/** if (AD_ReplicationTable_ID == 0)
{
SetAD_ReplicationStrategy_ID (0);
SetAD_ReplicationTable_ID (0);
SetAD_Table_ID (0);
SetEntityType (null);	// U
SetReplicationType (null);
}
 */
}
public X_AD_ReplicationTable (Ctx ctx, int AD_ReplicationTable_ID, Trx trxName) : base (ctx, AD_ReplicationTable_ID, trxName)
{
/** if (AD_ReplicationTable_ID == 0)
{
SetAD_ReplicationStrategy_ID (0);
SetAD_ReplicationTable_ID (0);
SetAD_Table_ID (0);
SetEntityType (null);	// U
SetReplicationType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReplicationTable (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReplicationTable (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReplicationTable (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ReplicationTable()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363520L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046731L;
/** AD_Table_ID=601 */
public static int Table_ID;
 // =601;

/** TableName=AD_ReplicationTable */
public static String Table_Name="AD_ReplicationTable";

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
StringBuilder sb = new StringBuilder ("X_AD_ReplicationTable[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Replication Strategy.
@param AD_ReplicationStrategy_ID Data Replication Strategy */
public void SetAD_ReplicationStrategy_ID (int AD_ReplicationStrategy_ID)
{
if (AD_ReplicationStrategy_ID < 1) throw new ArgumentException ("AD_ReplicationStrategy_ID is mandatory.");
Set_ValueNoCheck ("AD_ReplicationStrategy_ID", AD_ReplicationStrategy_ID);
}
/** Get Replication Strategy.
@return Data Replication Strategy */
public int GetAD_ReplicationStrategy_ID() 
{
Object ii = Get_Value("AD_ReplicationStrategy_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_ReplicationStrategy_ID().ToString());
}
/** Set Replication Table.
@param AD_ReplicationTable_ID Data Replication Strategy Table Info */
public void SetAD_ReplicationTable_ID (int AD_ReplicationTable_ID)
{
if (AD_ReplicationTable_ID < 1) throw new ArgumentException ("AD_ReplicationTable_ID is mandatory.");
Set_ValueNoCheck ("AD_ReplicationTable_ID", AD_ReplicationTable_ID);
}
/** Get Replication Table.
@return Data Replication Strategy Table Info */
public int GetAD_ReplicationTable_ID() 
{
Object ii = Get_Value("AD_ReplicationTable_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_ValueNoCheck ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}

/** ReplicationType AD_Reference_ID=126 */
public static int REPLICATIONTYPE_AD_Reference_ID=126;
/** Local = L */
public static String REPLICATIONTYPE_Local = "L";
/** Merge = M */
public static String REPLICATIONTYPE_Merge = "M";
/** Reference = R */
public static String REPLICATIONTYPE_Reference = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsReplicationTypeValid (String test)
{
return test.Equals("L") || test.Equals("M") || test.Equals("R");
}
/** Set Replication Type.
@param ReplicationType Type of Data Replication */
public void SetReplicationType (String ReplicationType)
{
if (ReplicationType == null) throw new ArgumentException ("ReplicationType is mandatory");
if (!IsReplicationTypeValid(ReplicationType))
throw new ArgumentException ("ReplicationType Invalid value - " + ReplicationType + " - Reference_ID=126 - L - M - R");
if (ReplicationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ReplicationType = ReplicationType.Substring(0,1);
}
Set_Value ("ReplicationType", ReplicationType);
}
/** Get Replication Type.
@return Type of Data Replication */
public String GetReplicationType() 
{
return (String)Get_Value("ReplicationType");
}
}

}
