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
/** Generated Model for AD_PrintLabelLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PrintLabelLine : PO
{
public X_AD_PrintLabelLine (Context ctx, int AD_PrintLabelLine_ID, Trx trxName) : base (ctx, AD_PrintLabelLine_ID, trxName)
{
/** if (AD_PrintLabelLine_ID == 0)
{
SetAD_LabelPrinterFunction_ID (0);
SetAD_PrintLabelLine_ID (0);
SetAD_PrintLabel_ID (0);
SetLabelFormatType (null);	// F
SetName (null);
SetSeqNo (0);
SetXPosition (0);
SetYPosition (0);
}
 */
}
public X_AD_PrintLabelLine (Ctx ctx, int AD_PrintLabelLine_ID, Trx trxName) : base (ctx, AD_PrintLabelLine_ID, trxName)
{
/** if (AD_PrintLabelLine_ID == 0)
{
SetAD_LabelPrinterFunction_ID (0);
SetAD_PrintLabelLine_ID (0);
SetAD_PrintLabel_ID (0);
SetLabelFormatType (null);	// F
SetName (null);
SetSeqNo (0);
SetXPosition (0);
SetYPosition (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintLabelLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintLabelLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintLabelLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PrintLabelLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362956L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046167L;
/** AD_Table_ID=569 */
public static int Table_ID;
 // =569;

/** TableName=AD_PrintLabelLine */
public static String Table_Name="AD_PrintLabelLine";

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
StringBuilder sb = new StringBuilder ("X_AD_PrintLabelLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);
else
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Label printer Function.
@param AD_LabelPrinterFunction_ID Function of Label Printer */
public void SetAD_LabelPrinterFunction_ID (int AD_LabelPrinterFunction_ID)
{
if (AD_LabelPrinterFunction_ID < 1) throw new ArgumentException ("AD_LabelPrinterFunction_ID is mandatory.");
Set_Value ("AD_LabelPrinterFunction_ID", AD_LabelPrinterFunction_ID);
}
/** Get Label printer Function.
@return Function of Label Printer */
public int GetAD_LabelPrinterFunction_ID() 
{
Object ii = Get_Value("AD_LabelPrinterFunction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Print Label Line.
@param AD_PrintLabelLine_ID Print Label Line Format */
public void SetAD_PrintLabelLine_ID (int AD_PrintLabelLine_ID)
{
if (AD_PrintLabelLine_ID < 1) throw new ArgumentException ("AD_PrintLabelLine_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintLabelLine_ID", AD_PrintLabelLine_ID);
}
/** Get Print Label Line.
@return Print Label Line Format */
public int GetAD_PrintLabelLine_ID() 
{
Object ii = Get_Value("AD_PrintLabelLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Print Label.
@param AD_PrintLabel_ID Label Format to print */
public void SetAD_PrintLabel_ID (int AD_PrintLabel_ID)
{
if (AD_PrintLabel_ID < 1) throw new ArgumentException ("AD_PrintLabel_ID is mandatory.");
Set_ValueNoCheck ("AD_PrintLabel_ID", AD_PrintLabel_ID);
}
/** Get Print Label.
@return Label Format to print */
public int GetAD_PrintLabel_ID() 
{
Object ii = Get_Value("AD_PrintLabel_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** LabelFormatType AD_Reference_ID=280 */
public static int LABELFORMATTYPE_AD_Reference_ID=280;
/** Field = F */
public static String LABELFORMATTYPE_Field = "F";
/** Text = T */
public static String LABELFORMATTYPE_Text = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsLabelFormatTypeValid (String test)
{
return test.Equals("F") || test.Equals("T");
}
/** Set Label Format Type.
@param LabelFormatType Label Format Type */
public void SetLabelFormatType (String LabelFormatType)
{
if (LabelFormatType == null) throw new ArgumentException ("LabelFormatType is mandatory");
if (!IsLabelFormatTypeValid(LabelFormatType))
throw new ArgumentException ("LabelFormatType Invalid value - " + LabelFormatType + " - Reference_ID=280 - F - T");
if (LabelFormatType.Length > 1)
{
log.Warning("Length > 1 - truncated");
LabelFormatType = LabelFormatType.Substring(0,1);
}
Set_Value ("LabelFormatType", LabelFormatType);
}
/** Get Label Format Type.
@return Label Format Type */
public String GetLabelFormatType() 
{
return (String)Get_Value("LabelFormatType");
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
/** Set Print Text.
@param PrintName The label text to be printed on a document or correspondence. */
public void SetPrintName (String PrintName)
{
if (PrintName != null && PrintName.Length > 60)
{
log.Warning("Length > 60 - truncated");
PrintName = PrintName.Substring(0,60);
}
Set_Value ("PrintName", PrintName);
}
/** Get Print Text.
@return The label text to be printed on a document or correspondence. */
public String GetPrintName() 
{
return (String)Get_Value("PrintName");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetSeqNo().ToString());
}
/** Set X Position.
@param XPosition Absolute X (horizontal) position in 1/72 of an inch */
public void SetXPosition (int XPosition)
{
Set_Value ("XPosition", XPosition);
}
/** Get X Position.
@return Absolute X (horizontal) position in 1/72 of an inch */
public int GetXPosition() 
{
Object ii = Get_Value("XPosition");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Y Position.
@param YPosition Absolute Y (vertical) position in 1/72 of an inch */
public void SetYPosition (int YPosition)
{
Set_Value ("YPosition", YPosition);
}
/** Get Y Position.
@return Absolute Y (vertical) position in 1/72 of an inch */
public int GetYPosition() 
{
Object ii = Get_Value("YPosition");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
