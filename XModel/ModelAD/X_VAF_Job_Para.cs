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
/** Generated Model for VAF_Job_Para
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_Job_Para : PO
{
public X_VAF_Job_Para (Context ctx, int VAF_Job_Para_ID, Trx trxName) : base (ctx, VAF_Job_Para_ID, trxName)
{
/** if (VAF_Job_Para_ID == 0)
{
SetVAF_Job_ID (0);
SetVAF_Job_Para_ID (0);
SetVAF_Control_Ref_ID (0);
SetColumnName (null);
SetRecordType (null);	// U
SetFieldLength (0);
SetIsCentrallyMaintained (true);	// Y
SetIsMandatory (false);
SetIsRange (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_Job_Para WHERE VAF_Job_ID=@VAF_Job_ID@
}
 */
}
public X_VAF_Job_Para (Ctx ctx, int VAF_Job_Para_ID, Trx trxName) : base (ctx, VAF_Job_Para_ID, trxName)
{
/** if (VAF_Job_Para_ID == 0)
{
SetVAF_Job_ID (0);
SetVAF_Job_Para_ID (0);
SetVAF_Control_Ref_ID (0);
SetColumnName (null);
SetRecordType (null);	// U
SetFieldLength (0);
SetIsCentrallyMaintained (true);	// Y
SetIsMandatory (false);
SetIsRange (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_Job_Para WHERE VAF_Job_ID=@VAF_Job_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Job_Para (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Job_Para (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_Job_Para (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_Job_Para()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363238L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046449L;
/** VAF_TableView_ID=285 */
public static int Table_ID;
 // =285;

/** TableName=VAF_Job_Para */
public static String Table_Name="VAF_Job_Para";

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
StringBuilder sb = new StringBuilder ("X_VAF_Job_Para[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Element.
@param VAF_ColumnDic_ID System Element enables the central maintenance of column description and help. */
public void SetVAF_ColumnDic_ID (int VAF_ColumnDic_ID)
{
if (VAF_ColumnDic_ID <= 0) Set_Value ("VAF_ColumnDic_ID", null);
else
Set_Value ("VAF_ColumnDic_ID", VAF_ColumnDic_ID);
}
/** Get System Element.
@return System Element enables the central maintenance of column description and help. */
public int GetVAF_ColumnDic_ID() 
{
Object ii = Get_Value("VAF_ColumnDic_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process.
@param VAF_Job_ID Process or Report */
public void SetVAF_Job_ID (int VAF_Job_ID)
{
if (VAF_Job_ID < 1) throw new ArgumentException ("VAF_Job_ID is mandatory.");
Set_ValueNoCheck ("VAF_Job_ID", VAF_Job_ID);
}
/** Get Process.
@return Process or Report */
public int GetVAF_Job_ID() 
{
Object ii = Get_Value("VAF_Job_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process Parameter.
@param VAF_Job_Para_ID Process Parameter */
public void SetVAF_Job_Para_ID (int VAF_Job_Para_ID)
{
if (VAF_Job_Para_ID < 1) throw new ArgumentException ("VAF_Job_Para_ID is mandatory.");
Set_ValueNoCheck ("VAF_Job_Para_ID", VAF_Job_Para_ID);
}
/** Get Process Parameter.
@return Process Parameter */
public int GetVAF_Job_Para_ID() 
{
Object ii = Get_Value("VAF_Job_Para_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_Control_Ref_ID VAF_Control_Ref_ID=1 */
public static int VAF_CONTROL_REF_ID_VAF_Control_Ref_ID=1;
/** Set Reference.
@param VAF_Control_Ref_ID System Reference and Validation */
public void SetVAF_Control_Ref_ID (int VAF_Control_Ref_ID)
{
if (VAF_Control_Ref_ID < 1) throw new ArgumentException ("VAF_Control_Ref_ID is mandatory.");
Set_Value ("VAF_Control_Ref_ID", VAF_Control_Ref_ID);
}
/** Get Reference.
@return System Reference and Validation */
public int GetVAF_Control_Ref_ID() 
{
Object ii = Get_Value("VAF_Control_Ref_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAF_Control_Ref_Value_ID VAF_Control_Ref_ID=4 */
public static int VAF_CONTROL_REF_VALUE_ID_VAF_Control_Ref_ID=4;
/** Set Reference Key.
@param VAF_Control_Ref_Value_ID Required to specify, if data type is Table or List */
public void SetVAF_Control_Ref_Value_ID (int VAF_Control_Ref_Value_ID)
{
if (VAF_Control_Ref_Value_ID <= 0) Set_Value ("VAF_Control_Ref_Value_ID", null);
else
Set_Value ("VAF_Control_Ref_Value_ID", VAF_Control_Ref_Value_ID);
}
/** Get Reference Key.
@return Required to specify, if data type is Table or List */
public int GetVAF_Control_Ref_Value_ID() 
{
Object ii = Get_Value("VAF_Control_Ref_Value_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Dynamic Validation.
@param VAF_DataVal_Rule_ID Dynamic Validation Rule */
public void SetVAF_DataVal_Rule_ID (int VAF_DataVal_Rule_ID)
{
if (VAF_DataVal_Rule_ID <= 0) Set_Value ("VAF_DataVal_Rule_ID", null);
else
Set_Value ("VAF_DataVal_Rule_ID", VAF_DataVal_Rule_ID);
}
/** Get Dynamic Validation.
@return Dynamic Validation Rule */
public int GetVAF_DataVal_Rule_ID() 
{
Object ii = Get_Value("VAF_DataVal_Rule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set DB Column Name.
@param ColumnName Name of the column in the database */
public void SetColumnName (String ColumnName)
{
if (ColumnName == null) throw new ArgumentException ("ColumnName is mandatory.");
if (ColumnName.Length > 40)
{
log.Warning("Length > 40 - truncated");
ColumnName = ColumnName.Substring(0,40);
}
Set_Value ("ColumnName", ColumnName);
}
/** Get DB Column Name.
@return Name of the column in the database */
public String GetColumnName() 
{
return (String)Get_Value("ColumnName");
}
/** Set Default Logic.
@param DefaultValue Default value hierarchy, separated by ;
 */
public void SetDefaultValue (String DefaultValue)
{
if (DefaultValue != null && DefaultValue.Length > 255)
{
log.Warning("Length > 255 - truncated");
DefaultValue = DefaultValue.Substring(0,255);
}
Set_Value ("DefaultValue", DefaultValue);
}
/** Get Default Logic.
@return Default value hierarchy, separated by ;
 */
public String GetDefaultValue() 
{
return (String)Get_Value("DefaultValue");
}
/** Set Default Logic 2.
@param DefaultValue2 Default value hierarchy, separated by ;
 */
public void SetDefaultValue2 (String DefaultValue2)
{
if (DefaultValue2 != null && DefaultValue2.Length > 255)
{
log.Warning("Length > 255 - truncated");
DefaultValue2 = DefaultValue2.Substring(0,255);
}
Set_Value ("DefaultValue2", DefaultValue2);
}
/** Get Default Logic 2.
@return Default value hierarchy, separated by ;
 */
public String GetDefaultValue2() 
{
return (String)Get_Value("DefaultValue2");
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
/** Set Length.
@param FieldLength Length of the column in the database */
public void SetFieldLength (int FieldLength)
{
Set_Value ("FieldLength", FieldLength);
}
/** Get Length.
@return Length of the column in the database */
public int GetFieldLength() 
{
Object ii = Get_Value("FieldLength");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Centrally maintained.
@param IsCentrallyMaintained Information maintained in System Element table */
public void SetIsCentrallyMaintained (Boolean IsCentrallyMaintained)
{
Set_Value ("IsCentrallyMaintained", IsCentrallyMaintained);
}
/** Get Centrally maintained.
@return Information maintained in System Element table */
public Boolean IsCentrallyMaintained() 
{
Object oo = Get_Value("IsCentrallyMaintained");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Displayed.
@param IsDisplayed Determines, if this field is displayed */
public void SetIsDisplayed (Boolean IsDisplayed)
{
Set_Value ("IsDisplayed", IsDisplayed);
}
/** Get Displayed.
@return Determines, if this field is displayed */
public Boolean IsDisplayed() 
{
Object oo = Get_Value("IsDisplayed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Mandatory.
@param IsMandatory Data is required in this column */
public void SetIsMandatory (Boolean IsMandatory)
{
Set_Value ("IsMandatory", IsMandatory);
}
/** Get Mandatory.
@return Data is required in this column */
public Boolean IsMandatory() 
{
Object oo = Get_Value("IsMandatory");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Range.
@param IsRange The parameter is a range of values */
public void SetIsRange (Boolean IsRange)
{
Set_Value ("IsRange", IsRange);
}
/** Get Range.
@return The parameter is a range of values */
public Boolean IsRange() 
{
Object oo = Get_Value("IsRange");
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
/** Set Value Format.
@param VFormat Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetVFormat (String VFormat)
{
if (VFormat != null && VFormat.Length > 20)
{
log.Warning("Length > 20 - truncated");
VFormat = VFormat.Substring(0,20);
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
/** Set Max. Value.
@param ValueMax Maximum Value for a field */
public void SetValueMax (String ValueMax)
{
if (ValueMax != null && ValueMax.Length > 20)
{
log.Warning("Length > 20 - truncated");
ValueMax = ValueMax.Substring(0,20);
}
Set_Value ("ValueMax", ValueMax);
}
/** Get Max. Value.
@return Maximum Value for a field */
public String GetValueMax() 
{
return (String)Get_Value("ValueMax");
}
/** Set Min. Value.
@param ValueMin Minimum Value for a field */
public void SetValueMin (String ValueMin)
{
if (ValueMin != null && ValueMin.Length > 20)
{
log.Warning("Length > 20 - truncated");
ValueMin = ValueMin.Substring(0,20);
}
Set_Value ("ValueMin", ValueMin);
}
/** Get Min. Value.
@return Minimum Value for a field */
public String GetValueMin() 
{
return (String)Get_Value("ValueMin");
}
/** Set Info Window.
@param VAF_QuickSearchWindow_ID Info and search/select Window */
public void SetVAF_QuickSearchWindow_ID(int VAF_QuickSearchWindow_ID)
{
    if (VAF_QuickSearchWindow_ID <= 0) Set_Value("VAF_QuickSearchWindow_ID", null);
    else
        Set_Value("VAF_QuickSearchWindow_ID", VAF_QuickSearchWindow_ID);
}
/** Get Info Window.
@return Info and search/select Window */
public int GetVAF_QuickSearchWindow_ID()
{
    Object ii = Get_Value("VAF_QuickSearchWindow_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
