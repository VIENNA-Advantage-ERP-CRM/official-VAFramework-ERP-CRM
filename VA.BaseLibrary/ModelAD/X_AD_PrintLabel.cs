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
/** Generated Model for AD_PrintLabel
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_PrintLabel : PO
{
public X_AD_PrintLabel (Context ctx, int AD_PrintLabel_ID, Trx trxName) : base (ctx, AD_PrintLabel_ID, trxName)
{
/** if (AD_PrintLabel_ID == 0)
{
SetAD_LabelPrinter_ID (0);
SetAD_PrintLabel_ID (0);
SetAD_Table_ID (0);
SetIsLandscape (false);
SetLabelHeight (0);
SetLabelWidth (0);
SetName (null);
}
 */
}
public X_AD_PrintLabel (Ctx ctx, int AD_PrintLabel_ID, Trx trxName) : base (ctx, AD_PrintLabel_ID, trxName)
{
/** if (AD_PrintLabel_ID == 0)
{
SetAD_LabelPrinter_ID (0);
SetAD_PrintLabel_ID (0);
SetAD_Table_ID (0);
SetIsLandscape (false);
SetLabelHeight (0);
SetLabelWidth (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintLabel (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintLabel (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_PrintLabel (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_PrintLabel()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514362925L;
/** Last Updated Timestamp 7/29/2010 1:07:26 PM */
public static long updatedMS = 1280389046136L;
/** AD_Table_ID=570 */
public static int Table_ID;
 // =570;

/** TableName=AD_PrintLabel */
public static String Table_Name="AD_PrintLabel";

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
StringBuilder sb = new StringBuilder ("X_AD_PrintLabel[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Label printer.
@param AD_LabelPrinter_ID Label Printer Definition */
public void SetAD_LabelPrinter_ID (int AD_LabelPrinter_ID)
{
if (AD_LabelPrinter_ID < 1) throw new ArgumentException ("AD_LabelPrinter_ID is mandatory.");
Set_Value ("AD_LabelPrinter_ID", AD_LabelPrinter_ID);
}
/** Get Label printer.
@return Label Printer Definition */
public int GetAD_LabelPrinter_ID() 
{
Object ii = Get_Value("AD_LabelPrinter_ID");
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
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
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
/** Set Landscape.
@param IsLandscape Landscape orientation */
public void SetIsLandscape (Boolean IsLandscape)
{
Set_Value ("IsLandscape", IsLandscape);
}
/** Get Landscape.
@return Landscape orientation */
public Boolean IsLandscape() 
{
Object oo = Get_Value("IsLandscape");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Label Height.
@param LabelHeight Height of the label */
public void SetLabelHeight (int LabelHeight)
{
Set_Value ("LabelHeight", LabelHeight);
}
/** Get Label Height.
@return Height of the label */
public int GetLabelHeight() 
{
Object ii = Get_Value("LabelHeight");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Label Width.
@param LabelWidth Width of the Label */
public void SetLabelWidth (int LabelWidth)
{
Set_Value ("LabelWidth", LabelWidth);
}
/** Get Label Width.
@return Width of the Label */
public int GetLabelWidth() 
{
Object ii = Get_Value("LabelWidth");
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
/** Set Printer Name.
@param PrinterName Name of the Printer */
public void SetPrinterName (String PrinterName)
{
if (PrinterName != null && PrinterName.Length > 40)
{
log.Warning("Length > 40 - truncated");
PrinterName = PrinterName.Substring(0,40);
}
Set_Value ("PrinterName", PrinterName);
}
/** Get Printer Name.
@return Name of the Printer */
public String GetPrinterName() 
{
return (String)Get_Value("PrinterName");
}
}

}
