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
/** Generated Model for AD_Replication
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Replication : PO
{
public X_AD_Replication (Context ctx, int AD_Replication_ID, Trx trxName) : base (ctx, AD_Replication_ID, trxName)
{
/** if (AD_Replication_ID == 0)
{
SetAD_ReplicationStrategy_ID (0);
SetAD_Replication_ID (0);
SetHostAddress (null);
SetHostPort (0);	// 80
SetIsRMIoverHTTP (true);	// Y
SetName (null);
}
 */
}
public X_AD_Replication (Ctx ctx, int AD_Replication_ID, Trx trxName) : base (ctx, AD_Replication_ID, trxName)
{
/** if (AD_Replication_ID == 0)
{
SetAD_ReplicationStrategy_ID (0);
SetAD_Replication_ID (0);
SetHostAddress (null);
SetHostPort (0);	// 80
SetIsRMIoverHTTP (true);	// Y
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Replication()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363473L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046684L;
/** AD_Table_ID=605 */
public static int Table_ID;
 // =605;

/** TableName=AD_Replication */
public static String Table_Name="AD_Replication";

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
StringBuilder sb = new StringBuilder ("X_AD_Replication[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Replication Strategy.
@param AD_ReplicationStrategy_ID Data Replication Strategy */
public void SetAD_ReplicationStrategy_ID (int AD_ReplicationStrategy_ID)
{
if (AD_ReplicationStrategy_ID < 1) throw new ArgumentException ("AD_ReplicationStrategy_ID is mandatory.");
Set_Value ("AD_ReplicationStrategy_ID", AD_ReplicationStrategy_ID);
}
/** Get Replication Strategy.
@return Data Replication Strategy */
public int GetAD_ReplicationStrategy_ID() 
{
Object ii = Get_Value("AD_ReplicationStrategy_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Replication.
@param AD_Replication_ID Data Replication Target */
public void SetAD_Replication_ID (int AD_Replication_ID)
{
if (AD_Replication_ID < 1) throw new ArgumentException ("AD_Replication_ID is mandatory.");
Set_ValueNoCheck ("AD_Replication_ID", AD_Replication_ID);
}
/** Get Replication.
@return Data Replication Target */
public int GetAD_Replication_ID() 
{
Object ii = Get_Value("AD_Replication_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Date last run.
@param DateLastRun Date the process was last run. */
public void SetDateLastRun (DateTime? DateLastRun)
{
Set_ValueNoCheck ("DateLastRun", (DateTime?)DateLastRun);
}
/** Get Date last run.
@return Date the process was last run. */
public DateTime? GetDateLastRun() 
{
return (DateTime?)Get_Value("DateLastRun");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Host Address.
@param HostAddress Host Address URL or DNS */
public void SetHostAddress (String HostAddress)
{
if (HostAddress == null) throw new ArgumentException ("HostAddress is mandatory.");
if (HostAddress.Length > 60)
{
log.Warning("Length > 60 - truncated");
HostAddress = HostAddress.Substring(0,60);
}
Set_Value ("HostAddress", HostAddress);
}
/** Get Host Address.
@return Host Address URL or DNS */
public String GetHostAddress() 
{
return (String)Get_Value("HostAddress");
}
/** Set Host port.
@param HostPort Host Communication Port */
public void SetHostPort (int HostPort)
{
Set_Value ("HostPort", HostPort);
}
/** Get Host port.
@return Host Communication Port */
public int GetHostPort() 
{
Object ii = Get_Value("HostPort");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set ID Range End.
@param IDRangeEnd End of the ID Range used */
public void SetIDRangeEnd (Decimal? IDRangeEnd)
{
Set_Value ("IDRangeEnd", (Decimal?)IDRangeEnd);
}
/** Get ID Range End.
@return End of the ID Range used */
public Decimal GetIDRangeEnd() 
{
Object bd =Get_Value("IDRangeEnd");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set ID Range Start.
@param IDRangeStart Start of the ID Range used */
public void SetIDRangeStart (Decimal? IDRangeStart)
{
Set_Value ("IDRangeStart", (Decimal?)IDRangeStart);
}
/** Get ID Range Start.
@return Start of the ID Range used */
public Decimal GetIDRangeStart() 
{
Object bd =Get_Value("IDRangeStart");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Tunnel via HTTP.
@param IsRMIoverHTTP Connect to Server via HTTP Tunnel */
public void SetIsRMIoverHTTP (Boolean IsRMIoverHTTP)
{
Set_Value ("IsRMIoverHTTP", IsRMIoverHTTP);
}
/** Get Tunnel via HTTP.
@return Connect to Server via HTTP Tunnel */
public Boolean IsRMIoverHTTP() 
{
Object oo = Get_Value("IsRMIoverHTTP");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Prefix.
@param Prefix Prefix before the sequence number */
public void SetPrefix (String Prefix)
{
if (Prefix != null && Prefix.Length > 10)
{
log.Warning("Length > 10 - truncated");
Prefix = Prefix.Substring(0,10);
}
Set_Value ("Prefix", Prefix);
}
/** Get Prefix.
@return Prefix before the sequence number */
public String GetPrefix() 
{
return (String)Get_Value("Prefix");
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** Remote_Client_ID AD_Reference_ID=129 */
public static int REMOTE_CLIENT_ID_AD_Reference_ID=129;
/** Set Remote Tenant.
@param Remote_Client_ID Remote Tenant to be used to replicate / synchronize data with. */
public void SetRemote_Client_ID (int Remote_Client_ID)
{
if (Remote_Client_ID <= 0) Set_ValueNoCheck ("Remote_Client_ID", null);
else
Set_ValueNoCheck ("Remote_Client_ID", Remote_Client_ID);
}
/** Get Remote Tenant.
@return Remote Tenant to be used to replicate / synchronize data with. */
public int GetRemote_Client_ID() 
{
Object ii = Get_Value("Remote_Client_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Remote_Org_ID AD_Reference_ID=276 */
public static int REMOTE_ORG_ID_AD_Reference_ID=276;
/** Set Remote Organization.
@param Remote_Org_ID Remote Organization to be used to replicate / synchronize data with. */
public void SetRemote_Org_ID (int Remote_Org_ID)
{
if (Remote_Org_ID <= 0) Set_ValueNoCheck ("Remote_Org_ID", null);
else
Set_ValueNoCheck ("Remote_Org_ID", Remote_Org_ID);
}
/** Get Remote Organization.
@return Remote Organization to be used to replicate / synchronize data with. */
public int GetRemote_Org_ID() 
{
Object ii = Get_Value("Remote_Org_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Suffix.
@param Suffix Suffix after the number */
public void SetSuffix (String Suffix)
{
if (Suffix != null && Suffix.Length > 10)
{
log.Warning("Length > 10 - truncated");
Suffix = Suffix.Substring(0,10);
}
Set_Value ("Suffix", Suffix);
}
/** Get Suffix.
@return Suffix after the number */
public String GetSuffix() 
{
return (String)Get_Value("Suffix");
}
}

}
