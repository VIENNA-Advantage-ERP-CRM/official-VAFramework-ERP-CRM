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
/** Generated Model for AD_LabelPrinterFunction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_LabelPrinterFunction : PO
{
public X_AD_LabelPrinterFunction (Context ctx, int AD_LabelPrinterFunction_ID, Trx trxName) : base (ctx, AD_LabelPrinterFunction_ID, trxName)
{
/** if (AD_LabelPrinterFunction_ID == 0)
{
SetAD_LabelPrinterFunction_ID (0);
SetAD_LabelPrinter_ID (0);
SetIsXYPosition (false);
SetName (null);
}
 */
}
public X_AD_LabelPrinterFunction (Ctx ctx, int AD_LabelPrinterFunction_ID, Trx trxName) : base (ctx, AD_LabelPrinterFunction_ID, trxName)
{
/** if (AD_LabelPrinterFunction_ID == 0)
{
SetAD_LabelPrinterFunction_ID (0);
SetAD_LabelPrinter_ID (0);
SetIsXYPosition (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_LabelPrinterFunction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_LabelPrinterFunction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_LabelPrinterFunction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_LabelPrinterFunction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361984L;
/** Last Updated Timestamp 7/29/2010 1:07:25 PM */
public static long updatedMS = 1280389045195L;
/** AD_Table_ID=624 */
public static int Table_ID;
 // =624;

/** TableName=AD_LabelPrinterFunction */
public static String Table_Name="AD_LabelPrinterFunction";

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
StringBuilder sb = new StringBuilder ("X_AD_LabelPrinterFunction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Label printer Function.
@param AD_LabelPrinterFunction_ID Function of Label Printer */
public void SetAD_LabelPrinterFunction_ID (int AD_LabelPrinterFunction_ID)
{
if (AD_LabelPrinterFunction_ID < 1) throw new ArgumentException ("AD_LabelPrinterFunction_ID is mandatory.");
Set_ValueNoCheck ("AD_LabelPrinterFunction_ID", AD_LabelPrinterFunction_ID);
}
/** Get Label printer Function.
@return Function of Label Printer */
public int GetAD_LabelPrinterFunction_ID() 
{
Object ii = Get_Value("AD_LabelPrinterFunction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Label printer.
@param AD_LabelPrinter_ID Label Printer Definition */
public void SetAD_LabelPrinter_ID (int AD_LabelPrinter_ID)
{
if (AD_LabelPrinter_ID < 1) throw new ArgumentException ("AD_LabelPrinter_ID is mandatory.");
Set_ValueNoCheck ("AD_LabelPrinter_ID", AD_LabelPrinter_ID);
}
/** Get Label printer.
@return Label Printer Definition */
public int GetAD_LabelPrinter_ID() 
{
Object ii = Get_Value("AD_LabelPrinter_ID");
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
/** Set Function Prefix.
@param FunctionPrefix Data sent before the function */
public void SetFunctionPrefix (String FunctionPrefix)
{
if (FunctionPrefix != null && FunctionPrefix.Length > 40)
{
log.Warning("Length > 40 - truncated");
FunctionPrefix = FunctionPrefix.Substring(0,40);
}
Set_Value ("FunctionPrefix", FunctionPrefix);
}
/** Get Function Prefix.
@return Data sent before the function */
public String GetFunctionPrefix() 
{
return (String)Get_Value("FunctionPrefix");
}
/** Set Function Suffix.
@param FunctionSuffix Data sent after the function */
public void SetFunctionSuffix (String FunctionSuffix)
{
if (FunctionSuffix != null && FunctionSuffix.Length > 40)
{
log.Warning("Length > 40 - truncated");
FunctionSuffix = FunctionSuffix.Substring(0,40);
}
Set_Value ("FunctionSuffix", FunctionSuffix);
}
/** Get Function Suffix.
@return Data sent after the function */
public String GetFunctionSuffix() 
{
return (String)Get_Value("FunctionSuffix");
}
/** Set XY Position.
@param IsXYPosition The Function is XY position */
public void SetIsXYPosition (Boolean IsXYPosition)
{
Set_Value ("IsXYPosition", IsXYPosition);
}
/** Get XY Position.
@return The Function is XY position */
public Boolean IsXYPosition() 
{
Object oo = Get_Value("IsXYPosition");
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
/** Set XY Separator.
@param XYSeparator The separator between the X and Y function. */
public void SetXYSeparator (String XYSeparator)
{
if (XYSeparator != null && XYSeparator.Length > 20)
{
log.Warning("Length > 20 - truncated");
XYSeparator = XYSeparator.Substring(0,20);
}
Set_Value ("XYSeparator", XYSeparator);
}
/** Get XY Separator.
@return The separator between the X and Y function. */
public String GetXYSeparator() 
{
return (String)Get_Value("XYSeparator");
}
}

}
