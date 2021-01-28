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
/** Generated Model for VAF_ClientShare
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ClientShare : PO
{
public X_VAF_ClientShare (Context ctx, int VAF_ClientShare_ID, Trx trxName) : base (ctx, VAF_ClientShare_ID, trxName)
{
/** if (VAF_ClientShare_ID == 0)
{
SetVAF_ClientShare_ID (0);
SetVAF_TableView_ID (0);
SetName (null);
SetShareType (null);
}
 */
}
public X_VAF_ClientShare (Ctx ctx, int VAF_ClientShare_ID, Trx trxName) : base (ctx, VAF_ClientShare_ID, trxName)
{
/** if (VAF_ClientShare_ID == 0)
{
SetVAF_ClientShare_ID (0);
SetVAF_TableView_ID (0);
SetName (null);
SetShareType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ClientShare (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ClientShare (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ClientShare (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ClientShare()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360919L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044130L;
/** VAF_TableView_ID=827 */
public static int Table_ID;
 // =827;

/** TableName=VAF_ClientShare */
public static String Table_Name="VAF_ClientShare";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_VAF_ClientShare[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tenant Share.
@param VAF_ClientShare_ID Force (not) sharing of tenant/org entities */
public void SetVAF_ClientShare_ID (int VAF_ClientShare_ID)
{
if (VAF_ClientShare_ID < 1) throw new ArgumentException ("VAF_ClientShare_ID is mandatory.");
Set_ValueNoCheck ("VAF_ClientShare_ID", VAF_ClientShare_ID);
}
/** Get Tenant Share.
@return Force (not) sharing of tenant/org entities */
public int GetVAF_ClientShare_ID() 
{
Object ii = Get_Value("VAF_ClientShare_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");
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

/** ShareType VAF_Control_Ref_ID=365 */
public static int SHARETYPE_VAF_Control_Ref_ID=365;
/** Client (all shared) = C */
public static String SHARETYPE_ClientAllShared = "C";
/** Org (not shared) = O */
public static String SHARETYPE_OrgNotShared = "O";
/** Client or Org = x */
public static String SHARETYPE_ClientOrOrg = "x";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsShareTypeValid (String test)
{
return test.Equals("C") || test.Equals("O") || test.Equals("x");
}
/** Set Share Type.
@param ShareType Type of sharing */
public void SetShareType (String ShareType)
{
if (ShareType == null) throw new ArgumentException ("ShareType is mandatory");
if (!IsShareTypeValid(ShareType))
throw new ArgumentException ("ShareType Invalid value - " + ShareType + " - Reference_ID=365 - C - O - x");
if (ShareType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ShareType = ShareType.Substring(0,1);
}
Set_Value ("ShareType", ShareType);
}
/** Get Share Type.
@return Type of sharing */
public String GetShareType() 
{
return (String)Get_Value("ShareType");
}
}

}
