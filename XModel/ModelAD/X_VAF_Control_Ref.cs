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
/** Generated Model for VAF_Control_Ref
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Control_Ref : PO
{
public X_VAF_Control_Ref (Context ctx, int VAF_Control_Ref_ID, Trx trxName) : base (ctx, VAF_Control_Ref_ID, trxName)
{
/** if (VAF_Control_Ref_ID == 0)
{
SetVAF_Control_Ref_ID (0);
SetRecordType (null);	// U
SetName (null);
SetValidationType (null);
}
 */
}
public X_VAF_Control_Ref (Ctx ctx, int VAF_Control_Ref_ID, Trx trxName) : base (ctx, VAF_Control_Ref_ID, trxName)
{
/** if (VAF_Control_Ref_ID == 0)
{
SetVAF_Control_Ref_ID (0);
SetRecordType (null);	// U
SetName (null);
SetValidationType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Control_Ref (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Control_Ref (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Control_Ref (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Control_Ref()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363426L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046637L;
/** VAF_TableView_ID=102 */
public static int Table_ID;
 // =102;

/** TableName=VAF_Control_Ref */
public static String Table_Name="VAF_Control_Ref";

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
StringBuilder sb = new StringBuilder ("X_VAF_Control_Ref[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Reference.
@param VAF_Control_Ref_ID System Reference and Validation */
public void SetVAF_Control_Ref_ID (int VAF_Control_Ref_ID)
{
if (VAF_Control_Ref_ID < 1) throw new ArgumentException ("VAF_Control_Ref_ID is mandatory.");
Set_ValueNoCheck ("VAF_Control_Ref_ID", VAF_Control_Ref_ID);
}
/** Get Reference.
@return System Reference and Validation */
public int GetVAF_Control_Ref_ID() 
{
Object ii = Get_Value("VAF_Control_Ref_ID");
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
/** Set Value Format.
@param VFormat Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetVFormat (String VFormat)
{
if (VFormat != null && VFormat.Length > 40)
{
log.Warning("Length > 40 - truncated");
VFormat = VFormat.Substring(0,40);
}
Set_Value ("VFormat", VFormat);
}
/** Get Value Format.
@return Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetVFormat() 
{
return (String)Get_Value("VFormat");
}

/** ValidationType VAF_Control_Ref_ID=2 */
public static int VALIDATIONTYPE_VAF_Control_Ref_ID=2;
/** DataType = D */
public static String VALIDATIONTYPE_DataType = "D";
/** List Validation = L */
public static String VALIDATIONTYPE_ListValidation = "L";
/** Table Validation = T */
public static String VALIDATIONTYPE_TableValidation = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsValidationTypeValid (String test)
{
return test.Equals("D") || test.Equals("L") || test.Equals("T");
}
/** Set Validation type.
@param ValidationType Different method of validating data */
public void SetValidationType (String ValidationType)
{
if (ValidationType == null) throw new ArgumentException ("ValidationType is mandatory");
if (!IsValidationTypeValid(ValidationType))
throw new ArgumentException ("ValidationType Invalid value - " + ValidationType + " - Reference_ID=2 - D - L - T");
if (ValidationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ValidationType = ValidationType.Substring(0,1);
}
Set_Value ("ValidationType", ValidationType);
}
/** Get Validation type.
@return Different method of validating data */
public String GetValidationType() 
{
return (String)Get_Value("ValidationType");
}
}

}
