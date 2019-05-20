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
/** Generated Model for AD_MigrationStep
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_MigrationStep : PO
{
public X_AD_MigrationStep (Context ctx, int AD_MigrationStep_ID, Trx trxName) : base (ctx, AD_MigrationStep_ID, trxName)
{
/** if (AD_MigrationStep_ID == 0)
{
SetAD_MigrationStep_ID (0);
SetAD_Version_ID (0);
SetIsOkToFail (true);	// Y
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(Max(SeqNo),0)+10 FROM AD_MigrationStep WHERE AD_Version_ID=@AD_Version_ID@
SetTimingType (null);	// B
SetType (null);	// S
}
 */
}
public X_AD_MigrationStep (Ctx ctx, int AD_MigrationStep_ID, Trx trxName) : base (ctx, AD_MigrationStep_ID, trxName)
{
/** if (AD_MigrationStep_ID == 0)
{
SetAD_MigrationStep_ID (0);
SetAD_Version_ID (0);
SetIsOkToFail (true);	// Y
SetName (null);
SetSeqNo (0);	// @SQL=SELECT COALESCE(Max(SeqNo),0)+10 FROM AD_MigrationStep WHERE AD_Version_ID=@AD_Version_ID@
SetTimingType (null);	// B
SetType (null);	// S
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_MigrationStep (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_MigrationStep (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_MigrationStep (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_MigrationStep()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362235L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045446L;
/** AD_Table_ID=922 */
public static int Table_ID;
 // =922;

/** TableName=AD_MigrationStep */
public static String Table_Name="AD_MigrationStep";

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
StringBuilder sb = new StringBuilder ("X_AD_MigrationStep[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Migration Step.
@param AD_MigrationStep_ID Migration Step */
public void SetAD_MigrationStep_ID (int AD_MigrationStep_ID)
{
if (AD_MigrationStep_ID < 1) throw new ArgumentException ("AD_MigrationStep_ID is mandatory.");
Set_ValueNoCheck ("AD_MigrationStep_ID", AD_MigrationStep_ID);
}
/** Get Migration Step.
@return Migration Step */
public int GetAD_MigrationStep_ID() 
{
Object ii = Get_Value("AD_MigrationStep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Entity Version.
@param AD_Version_ID Entity Version */
public void SetAD_Version_ID (int AD_Version_ID)
{
if (AD_Version_ID < 1) throw new ArgumentException ("AD_Version_ID is mandatory.");
Set_ValueNoCheck ("AD_Version_ID", AD_Version_ID);
}
/** Get Entity Version.
@return Entity Version */
public int GetAD_Version_ID() 
{
Object ii = Get_Value("AD_Version_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Classname.
@param Classname Java Classname */
public void SetClassname (String Classname)
{
if (Classname != null && Classname.Length > 60)
{
log.Warning("Length > 60 - truncated");
Classname = Classname.Substring(0,60);
}
Set_Value ("Classname", Classname);
}
/** Get Classname.
@return Java Classname */
public String GetClassname() 
{
return (String)Get_Value("Classname");
}
/** Set Code.
@param Code Code to execute or to validate */
public void SetCode (String Code)
{
if (Code != null && Code.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Code = Code.Substring(0,2000);
}
Set_Value ("Code", Code);
}
/** Get Code.
@return Code to execute or to validate */
public String GetCode() 
{
return (String)Get_Value("Code");
}

/** DBType AD_Reference_ID=426 */
public static int DBTYPE_AD_Reference_ID=426;
/** IBM DB/2 = D */
public static String DBTYPE_IBMDB2 = "D";
/** Enterpise DB = E */
public static String DBTYPE_EnterpiseDB = "E";
/** Oracle = O */
public static String DBTYPE_Oracle = "O";
/** MS SQL Server = S */
public static String DBTYPE_MSSQLServer = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDBTypeValid (String test)
{
return test == null || test.Equals("D") || test.Equals("E") || test.Equals("O") || test.Equals("S");
}
/** Set Database Type.
@param DBType Database Type */
public void SetDBType (String DBType)
{
if (!IsDBTypeValid(DBType))
throw new ArgumentException ("DBType Invalid value - " + DBType + " - Reference_ID=426 - D - E - O - S");
if (DBType != null && DBType.Length > 1)
{
log.Warning("Length > 1 - truncated");
DBType = DBType.Substring(0,1);
}
Set_Value ("DBType", DBType);
}
/** Get Database Type.
@return Database Type */
public String GetDBType() 
{
return (String)Get_Value("DBType");
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
/** Set OK to Fail.
@param IsOkToFail It is OK for this step to fail */
public void SetIsOkToFail (Boolean IsOkToFail)
{
Set_Value ("IsOkToFail", IsOkToFail);
}
/** Get OK to Fail.
@return It is OK for this step to fail */
public Boolean IsOkToFail() 
{
Object oo = Get_Value("IsOkToFail");
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
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** TimingType AD_Reference_ID=416 */
public static int TIMINGTYPE_AD_Reference_ID=416;
/** Before Structure = 1 */
public static String TIMINGTYPE_BeforeStructure = "1";
/** Before Data = 3 */
public static String TIMINGTYPE_BeforeData = "3";
/** After Data = 4 */
public static String TIMINGTYPE_AfterData = "4";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTimingTypeValid (String test)
{
return test.Equals("1") || test.Equals("3") || test.Equals("4");
}
/** Set Timing Type.
@param TimingType Migration Timing Type */
public void SetTimingType (String TimingType)
{
if (TimingType == null) throw new ArgumentException ("TimingType is mandatory");
if (!IsTimingTypeValid(TimingType))
throw new ArgumentException ("TimingType Invalid value - " + TimingType + " - Reference_ID=416 - 1 - 3 - 4");
if (TimingType.Length > 1)
{
log.Warning("Length > 1 - truncated");
TimingType = TimingType.Substring(0,1);
}
Set_Value ("TimingType", TimingType);
}
/** Get Timing Type.
@return Migration Timing Type */
public String GetTimingType() 
{
return (String)Get_Value("TimingType");
}

/** Type AD_Reference_ID=101 */
public static int TYPE_AD_Reference_ID=101;
/** Java Script = E */
public static String TYPE_JavaScript = "E";
/** Java Language = J */
public static String TYPE_JavaLanguage = "J";
/** SQL = S */
public static String TYPE_SQL = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTypeValid (String test)
{
return test.Equals("E") || test.Equals("J") || test.Equals("S");
}
/** Set Code Type.
@param Type Type of Code/Validation (SQL, Java Script, Java Language) */
public void SetType (String Type)
{
if (Type == null) throw new ArgumentException ("Type is mandatory");
if (!IsTypeValid(Type))
throw new ArgumentException ("Type Invalid value - " + Type + " - Reference_ID=101 - E - J - S");
if (Type.Length > 1)
{
log.Warning("Length > 1 - truncated");
Type = Type.Substring(0,1);
}
Set_Value ("Type", Type);
}
/** Get Code Type.
@return Type of Code/Validation (SQL, Java Script, Java Language) */
public new String GetType() 
{
return (String)Get_Value("Type");
}
}

}
