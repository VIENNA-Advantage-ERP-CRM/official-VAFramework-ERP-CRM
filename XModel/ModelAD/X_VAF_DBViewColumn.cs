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
/** Generated Model for VAF_DBViewColumn
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_DBViewColumn : PO
{
public X_VAF_DBViewColumn (Context ctx, int VAF_DBViewColumn_ID, Trx trxName) : base (ctx, VAF_DBViewColumn_ID, trxName)
{
/** if (VAF_DBViewColumn_ID == 0)
{
SetVAF_DBViewColumn_ID (0);
SetVAF_DBViewElement_ID (0);
SetColumnName (null);
SetRecordType (null);	// U
}
 */
}
public X_VAF_DBViewColumn (Ctx ctx, int VAF_DBViewColumn_ID, Trx trxName) : base (ctx, VAF_DBViewColumn_ID, trxName)
{
/** if (VAF_DBViewColumn_ID == 0)
{
SetVAF_DBViewColumn_ID (0);
SetVAF_DBViewElement_ID (0);
SetColumnName (null);
SetRecordType (null);	// U
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_DBViewColumn (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_DBViewColumn (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_DBViewColumn (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_DBViewColumn()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365887L;
/** Last Updated Timestamp 7/29/2010 1:07:29 PM */
public static long updatedMS = 1280389049098L;
/** VAF_TableView_ID=935 */
public static int Table_ID;
 // =935;

/** TableName=VAF_DBViewColumn */
public static String Table_Name="VAF_DBViewColumn";

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
StringBuilder sb = new StringBuilder ("X_VAF_DBViewColumn[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set View Column.
@param VAF_DBViewColumn_ID Select column in View */
public void SetVAF_DBViewColumn_ID (int VAF_DBViewColumn_ID)
{
if (VAF_DBViewColumn_ID < 1) throw new ArgumentException ("VAF_DBViewColumn_ID is mandatory.");
Set_ValueNoCheck ("VAF_DBViewColumn_ID", VAF_DBViewColumn_ID);
}
/** Get View Column.
@return Select column in View */
public int GetVAF_DBViewColumn_ID() 
{
Object ii = Get_Value("VAF_DBViewColumn_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set View Component.
@param VAF_DBViewElement_ID Component (Select statement) of the view */
public void SetVAF_DBViewElement_ID (int VAF_DBViewElement_ID)
{
if (VAF_DBViewElement_ID < 1) throw new ArgumentException ("VAF_DBViewElement_ID is mandatory.");
Set_ValueNoCheck ("VAF_DBViewElement_ID", VAF_DBViewElement_ID);
}
/** Get View Component.
@return Component (Select statement) of the view */
public int GetVAF_DBViewElement_ID() 
{
Object ii = Get_Value("VAF_DBViewElement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetVAF_DBViewElement_ID().ToString());
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

/** DBDataType VAF_Control_Ref_ID=422 */
public static int DBDATATYPE_VAF_Control_Ref_ID=422;
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

/** RecordType VAF_Control_Ref_ID=389 */
public static int RecordType_VAF_Control_Ref_ID=389;
/** Set Entity Type.
@param RecordType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetRecordType (String RecordType)
{
if (RecordType.Length > 4)
{
log.Warning("Length > 4 - truncated");
RecordType = RecordType.Substring(0,4);
}
Set_Value ("RecordType", RecordType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetRecordType() 
{
return (String)Get_Value("RecordType");
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
