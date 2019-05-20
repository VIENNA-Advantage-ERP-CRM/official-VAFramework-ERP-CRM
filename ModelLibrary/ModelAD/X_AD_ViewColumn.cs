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
/** Generated Model for AD_ViewColumn
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ViewColumn : PO
{
public X_AD_ViewColumn (Context ctx, int AD_ViewColumn_ID, Trx trxName) : base (ctx, AD_ViewColumn_ID, trxName)
{
/** if (AD_ViewColumn_ID == 0)
{
SetAD_ViewColumn_ID (0);
SetAD_ViewComponent_ID (0);
SetColumnName (null);
SetEntityType (null);	// U
}
 */
}
public X_AD_ViewColumn (Ctx ctx, int AD_ViewColumn_ID, Trx trxName) : base (ctx, AD_ViewColumn_ID, trxName)
{
/** if (AD_ViewColumn_ID == 0)
{
SetAD_ViewColumn_ID (0);
SetAD_ViewComponent_ID (0);
SetColumnName (null);
SetEntityType (null);	// U
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ViewColumn (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ViewColumn (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ViewColumn (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ViewColumn()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365887L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049098L;
/** AD_Table_ID=935 */
public static int Table_ID;
 // =935;

/** TableName=AD_ViewColumn */
public static String Table_Name="AD_ViewColumn";

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
StringBuilder sb = new StringBuilder ("X_AD_ViewColumn[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set View Column.
@param AD_ViewColumn_ID Select column in View */
public void SetAD_ViewColumn_ID (int AD_ViewColumn_ID)
{
if (AD_ViewColumn_ID < 1) throw new ArgumentException ("AD_ViewColumn_ID is mandatory.");
Set_ValueNoCheck ("AD_ViewColumn_ID", AD_ViewColumn_ID);
}
/** Get View Column.
@return Select column in View */
public int GetAD_ViewColumn_ID() 
{
Object ii = Get_Value("AD_ViewColumn_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set View Component.
@param AD_ViewComponent_ID Component (Select statement) of the view */
public void SetAD_ViewComponent_ID (int AD_ViewComponent_ID)
{
if (AD_ViewComponent_ID < 1) throw new ArgumentException ("AD_ViewComponent_ID is mandatory.");
Set_ValueNoCheck ("AD_ViewComponent_ID", AD_ViewComponent_ID);
}
/** Get View Component.
@return Component (Select statement) of the view */
public int GetAD_ViewComponent_ID() 
{
Object ii = Get_Value("AD_ViewComponent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_ViewComponent_ID().ToString());
}
/** Set DB Column Name.
@param ColumnName Name of the column in the database */
public void SetColumnName (String ColumnName)
{
if (ColumnName == null) throw new ArgumentException ("ColumnName is mandatory.");
if (ColumnName.Length > 60)
{
log.Warning("Length > 60 - truncated");
ColumnName = ColumnName.Substring(0,60);
}
Set_Value ("ColumnName", ColumnName);
}
/** Get DB Column Name.
@return Name of the column in the database */
public String GetColumnName() 
{
return (String)Get_Value("ColumnName");
}
/** Set Column SQL.
@param ColumnSQL Virtual Column (r/o) */
public void SetColumnSQL (String ColumnSQL)
{
if (ColumnSQL != null && ColumnSQL.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
ColumnSQL = ColumnSQL.Substring(0,2000);
}
Set_Value ("ColumnSQL", ColumnSQL);
}
/** Get Column SQL.
@return Virtual Column (r/o) */
public String GetColumnSQL() 
{
return (String)Get_Value("ColumnSQL");
}

/** DBDataType AD_Reference_ID=422 */
public static int DBDATATYPE_AD_Reference_ID=422;
/** Binary LOB = B */
public static String DBDATATYPE_BinaryLOB = "B";
/** Character Fixed = C */
public static String DBDATATYPE_CharacterFixed = "C";
/** Decimal = D */
public static String DBDATATYPE_Decimal = "D";
/** Integer = I */
public static String DBDATATYPE_Integer = "I";
/** Character LOB = L */
public static String DBDATATYPE_CharacterLOB = "L";
/** Number = N */
public static String DBDATATYPE_Number = "N";
/** Timestamp = T */
public static String DBDATATYPE_Timestamp = "T";
/** Character Variable = V */
public static String DBDATATYPE_CharacterVariable = "V";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDBDataTypeValid (String test)
{
return test == null || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("I") || test.Equals("L") || test.Equals("N") || test.Equals("T") || test.Equals("V");
}
/** Set Data Type.
@param DBDataType Database Data Type */
public void SetDBDataType (String DBDataType)
{
if (!IsDBDataTypeValid(DBDataType))
throw new ArgumentException ("DBDataType Invalid value - " + DBDataType + " - Reference_ID=422 - B - C - D - I - L - N - T - V");
if (DBDataType != null && DBDataType.Length > 1)
{
log.Warning("Length > 1 - truncated");
DBDataType = DBDataType.Substring(0,1);
}
Set_Value ("DBDataType", DBDataType);
}
/** Get Data Type.
@return Database Data Type */
public String GetDBDataType() 
{
return (String)Get_Value("DBDataType");
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
}

}
