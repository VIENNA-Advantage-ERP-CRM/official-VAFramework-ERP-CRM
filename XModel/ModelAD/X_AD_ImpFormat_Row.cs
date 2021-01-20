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
/** Generated Model for VAF_ImportFormat_Row
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ImportFormat_Row : PO
{
public X_VAF_ImportFormat_Row (Context ctx, int VAF_ImportFormat_Row_ID, Trx trxName) : base (ctx, VAF_ImportFormat_Row_ID, trxName)
{
/** if (VAF_ImportFormat_Row_ID == 0)
{
SetVAF_Column_ID (0);
SetVAF_ImportFormat_ID (0);
SetVAF_ImportFormat_Row_ID (0);
SetDataType (null);
SetDecimalPoint (null);	// .
SetDivideBy100 (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_ImportFormat_Row WHERE VAF_ImportFormat_ID=@VAF_ImportFormat_ID@
}
 */
}
public X_VAF_ImportFormat_Row (Ctx ctx, int VAF_ImportFormat_Row_ID, Trx trxName) : base (ctx, VAF_ImportFormat_Row_ID, trxName)
{
/** if (VAF_ImportFormat_Row_ID == 0)
{
SetVAF_Column_ID (0);
SetVAF_ImportFormat_ID (0);
SetVAF_ImportFormat_Row_ID (0);
SetDataType (null);
SetDecimalPoint (null);	// .
SetDivideBy100 (false);
SetName (null);
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM VAF_ImportFormat_Row WHERE VAF_ImportFormat_ID=@VAF_ImportFormat_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ImportFormat_Row (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ImportFormat_Row (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ImportFormat_Row (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ImportFormat_Row()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361640L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044851L;
/** VAF_TableView_ID=382 */
public static int Table_ID;
 // =382;

/** TableName=VAF_ImportFormat_Row */
public static String Table_Name="VAF_ImportFormat_Row";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAF_ImportFormat_Row[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param VAF_Column_ID Column in the table */
public void SetVAF_Column_ID (int VAF_Column_ID)
{
if (VAF_Column_ID < 1) throw new ArgumentException ("VAF_Column_ID is mandatory.");
Set_Value ("VAF_Column_ID", VAF_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetVAF_Column_ID() 
{
Object ii = Get_Value("VAF_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Import Format.
@param VAF_ImportFormat_ID Import Format */
public void SetVAF_ImportFormat_ID (int VAF_ImportFormat_ID)
{
if (VAF_ImportFormat_ID < 1) throw new ArgumentException ("VAF_ImportFormat_ID is mandatory.");
Set_ValueNoCheck ("VAF_ImportFormat_ID", VAF_ImportFormat_ID);
}
/** Get Import Format.
@return Import Format */
public int GetVAF_ImportFormat_ID() 
{
Object ii = Get_Value("VAF_ImportFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Format Field.
@param VAF_ImportFormat_Row_ID Format Field */
public void SetVAF_ImportFormat_Row_ID (int VAF_ImportFormat_Row_ID)
{
if (VAF_ImportFormat_Row_ID < 1) throw new ArgumentException ("VAF_ImportFormat_Row_ID is mandatory.");
Set_ValueNoCheck ("VAF_ImportFormat_Row_ID", VAF_ImportFormat_Row_ID);
}
/** Get Format Field.
@return Format Field */
public int GetVAF_ImportFormat_Row_ID() 
{
Object ii = Get_Value("VAF_ImportFormat_Row_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Callout Code.
@param Callout External Callout Code - Fully qualified class names and method - separated by semicolons */
public void SetCallout (String Callout)
{
if (Callout != null && Callout.Length > 60)
{
log.Warning("Length > 60 - truncated");
Callout = Callout.Substring(0,60);
}
Set_Value ("Callout", Callout);
}
/** Get Callout Code.
@return External Callout Code - Fully qualified class names and method - separated by semicolons */
public String GetCallout() 
{
return (String)Get_Value("Callout");
}
/** Set Constant Value.
@param ConstantValue Constant value */
public void SetConstantValue (String ConstantValue)
{
if (ConstantValue != null && ConstantValue.Length > 60)
{
log.Warning("Length > 60 - truncated");
ConstantValue = ConstantValue.Substring(0,60);
}
Set_Value ("ConstantValue", ConstantValue);
}
/** Get Constant Value.
@return Constant value */
public String GetConstantValue() 
{
return (String)Get_Value("ConstantValue");
}
/** Set Data Format.
@param DataFormat Format String in Java Notation, e.g. ddMMyy */
public void SetDataFormat (String DataFormat)
{
if (DataFormat != null && DataFormat.Length > 20)
{
log.Warning("Length > 20 - truncated");
DataFormat = DataFormat.Substring(0,20);
}
Set_Value ("DataFormat", DataFormat);
}
/** Get Data Format.
@return Format String in Java Notation, e.g. ddMMyy */
public String GetDataFormat() 
{
return (String)Get_Value("DataFormat");
}

/** DataType AD_Reference_ID=210 */
public static int DATATYPE_AD_Reference_ID=210;
/** Constant = C */
public static String DATATYPE_Constant = "C";
/** Date = D */
public static String DATATYPE_Date = "D";
/** Number = N */
public static String DATATYPE_Number = "N";
/** String = S */
public static String DATATYPE_String = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDataTypeValid (String test)
{
return test.Equals("C") || test.Equals("D") || test.Equals("N") || test.Equals("S");
}
/** Set Data Type.
@param DataType Type of data */
public void SetDataType (String DataType)
{
if (DataType == null) throw new ArgumentException ("DataType is mandatory");
if (!IsDataTypeValid(DataType))
throw new ArgumentException ("DataType Invalid value - " + DataType + " - Reference_ID=210 - C - D - N - S");
if (DataType.Length > 1)
{
log.Warning("Length > 1 - truncated");
DataType = DataType.Substring(0,1);
}
Set_Value ("DataType", DataType);
}
/** Get Data Type.
@return Type of data */
public String GetDataType() 
{
return (String)Get_Value("DataType");
}
/** Set Decimal Point.
@param DecimalPoint Decimal Point in the data file - if any */
public void SetDecimalPoint (String DecimalPoint)
{
if (DecimalPoint == null) throw new ArgumentException ("DecimalPoint is mandatory.");
if (DecimalPoint.Length > 1)
{
log.Warning("Length > 1 - truncated");
DecimalPoint = DecimalPoint.Substring(0,1);
}
Set_Value ("DecimalPoint", DecimalPoint);
}
/** Get Decimal Point.
@return Decimal Point in the data file - if any */
public String GetDecimalPoint() 
{
return (String)Get_Value("DecimalPoint");
}
/** Set Divide by 100.
@param DivideBy100 Divide number by 100 to get correct amount */
public void SetDivideBy100 (Boolean DivideBy100)
{
Set_Value ("DivideBy100", DivideBy100);
}
/** Get Divide by 100.
@return Divide number by 100 to get correct amount */
public Boolean IsDivideBy100() 
{
Object oo = Get_Value("DivideBy100");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set End No.
@param EndNo End No */
public void SetEndNo (int EndNo)
{
Set_Value ("EndNo", EndNo);
}
/** Get End No.
@return End No */
public int GetEndNo() 
{
Object ii = Get_Value("EndNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Script.
@param Script Dynamic Java Language Script to calculate result */
public void SetScript (String Script)
{
if (Script != null && Script.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Script = Script.Substring(0,2000);
}
Set_Value ("Script", Script);
}
/** Get Script.
@return Dynamic Java Language Script to calculate result */
public String GetScript() 
{
return (String)Get_Value("Script");
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
/** Set Start No.
@param StartNo Starting number/position */
public void SetStartNo (int StartNo)
{
Set_Value ("StartNo", StartNo);
}
/** Get Start No.
@return Starting number/position */
public int GetStartNo() 
{
Object ii = Get_Value("StartNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
