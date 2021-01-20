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
/** Generated Model for VAF_TableViewIndex
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_TableViewIndex : PO
{
public X_VAF_TableViewIndex (Context ctx, int VAF_TableViewIndex_ID, Trx trxName) : base (ctx, VAF_TableViewIndex_ID, trxName)
{
/** if (VAF_TableViewIndex_ID == 0)
{
SetVAF_TableViewIndex_ID (0);
SetVAF_TableView_ID (0);
SetEntityType (null);
SetIsCreateConstraint (false);	// N
SetIsUnique (false);
SetName (null);
}
 */
}
public X_VAF_TableViewIndex (Ctx ctx, int VAF_TableViewIndex_ID, Trx trxName) : base (ctx, VAF_TableViewIndex_ID, trxName)
{
/** if (VAF_TableViewIndex_ID == 0)
{
SetVAF_TableViewIndex_ID (0);
SetVAF_TableView_ID (0);
SetEntityType (null);
SetIsCreateConstraint (false);	// N
SetIsUnique (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TableViewIndex (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TableViewIndex (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_TableViewIndex (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_TableViewIndex()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514364367L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047578L;
/** VAF_TableView_ID=909 */
public static int Table_ID;
 // =909;

/** TableName=VAF_TableViewIndex */
public static String Table_Name="VAF_TableViewIndex";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_VAF_TableViewIndex[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table Index.
@param VAF_TableViewIndex_ID Table Index */
public void SetVAF_TableViewIndex_ID (int VAF_TableViewIndex_ID)
{
if (VAF_TableViewIndex_ID < 1) throw new ArgumentException ("VAF_TableViewIndex_ID is mandatory.");
Set_ValueNoCheck ("VAF_TableViewIndex_ID", VAF_TableViewIndex_ID);
}
/** Get Table Index.
@return Table Index */
public int GetVAF_TableViewIndex_ID() 
{
Object ii = Get_Value("VAF_TableViewIndex_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID < 1) throw new ArgumentException ("VAF_TableView_ID is mandatory.");
Set_ValueNoCheck ("VAF_TableView_ID", VAF_TableView_ID);
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
/** Set Create Constraint.
@param IsCreateConstraint Create Database Constraint */
public void SetIsCreateConstraint (Boolean IsCreateConstraint)
{
Set_Value ("IsCreateConstraint", IsCreateConstraint);
}
/** Get Create Constraint.
@return Create Database Constraint */
public Boolean IsCreateConstraint() 
{
Object oo = Get_Value("IsCreateConstraint");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Unique.
@param IsUnique Unique */
public void SetIsUnique (Boolean IsUnique)
{
Set_Value ("IsUnique", IsUnique);
}
/** Get Unique.
@return Unique */
public Boolean IsUnique() 
{
Object oo = Get_Value("IsUnique");
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
}

}
