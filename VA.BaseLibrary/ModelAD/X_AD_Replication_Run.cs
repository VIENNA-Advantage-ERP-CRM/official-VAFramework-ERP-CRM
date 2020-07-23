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
/** Generated Model for AD_Replication_Run
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Replication_Run : PO
{
public X_AD_Replication_Run (Context ctx, int AD_Replication_Run_ID, Trx trxName) : base (ctx, AD_Replication_Run_ID, trxName)
{
/** if (AD_Replication_Run_ID == 0)
{
SetAD_Replication_ID (0);
SetAD_Replication_Run_ID (0);
SetIsReplicated (false);	// N
SetName (null);
}
 */
}
public X_AD_Replication_Run (Ctx ctx, int AD_Replication_Run_ID, Trx trxName) : base (ctx, AD_Replication_Run_ID, trxName)
{
/** if (AD_Replication_Run_ID == 0)
{
SetAD_Replication_ID (0);
SetAD_Replication_Run_ID (0);
SetIsReplicated (false);	// N
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication_Run (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication_Run (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Replication_Run (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Replication_Run()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363567L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046778L;
/** AD_Table_ID=603 */
public static int Table_ID;
 // =603;

/** TableName=AD_Replication_Run */
public static String Table_Name="AD_Replication_Run";

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
StringBuilder sb = new StringBuilder ("X_AD_Replication_Run[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Replicated.
@param IsReplicated The data is successfully replicated */
public void SetIsReplicated (Boolean IsReplicated)
{
Set_ValueNoCheck ("IsReplicated", IsReplicated);
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
}

}
