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
/** Generated Model for AD_ReportView_Col
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ReportView_Col : PO
{
public X_AD_ReportView_Col (Context ctx, int AD_ReportView_Col_ID, Trx trxName) : base (ctx, AD_ReportView_Col_ID, trxName)
{
/** if (AD_ReportView_Col_ID == 0)
{
SetAD_ReportView_Col_ID (0);
SetAD_ReportView_ID (0);
SetFunctionColumn (null);
SetIsGroupFunction (false);
}
 */
}
public X_AD_ReportView_Col (Ctx ctx, int AD_ReportView_Col_ID, Trx trxName) : base (ctx, AD_ReportView_Col_ID, trxName)
{
/** if (AD_ReportView_Col_ID == 0)
{
SetAD_ReportView_Col_ID (0);
SetAD_ReportView_ID (0);
SetFunctionColumn (null);
SetIsGroupFunction (false);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReportView_Col (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReportView_Col (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ReportView_Col (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ReportView_Col()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363818L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047029L;
/** AD_Table_ID=428 */
public static int Table_ID;
 // =428;

/** TableName=AD_ReportView_Col */
public static String Table_Name="AD_ReportView_Col";

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
StringBuilder sb = new StringBuilder ("X_AD_ReportView_Col[").Append(Get_ID()).Append("]");
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
/** Set Report view Column.
@param AD_ReportView_Col_ID Report view Column */
public void SetAD_ReportView_Col_ID (int AD_ReportView_Col_ID)
{
if (AD_ReportView_Col_ID < 1) throw new ArgumentException ("AD_ReportView_Col_ID is mandatory.");
Set_ValueNoCheck ("AD_ReportView_Col_ID", AD_ReportView_Col_ID);
}
/** Get Report view Column.
@return Report view Column */
public int GetAD_ReportView_Col_ID() 
{
Object ii = Get_Value("AD_ReportView_Col_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report View.
@param AD_ReportView_ID View used to generate this report */
public void SetAD_ReportView_ID (int AD_ReportView_ID)
{
if (AD_ReportView_ID < 1) throw new ArgumentException ("AD_ReportView_ID is mandatory.");
Set_ValueNoCheck ("AD_ReportView_ID", AD_ReportView_ID);
}
/** Get Report View.
@return View used to generate this report */
public int GetAD_ReportView_ID() 
{
Object ii = Get_Value("AD_ReportView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_ReportView_ID().ToString());
}
/** Set Function Column.
@param FunctionColumn Overwrite Column with Function */
public void SetFunctionColumn (String FunctionColumn)
{
if (FunctionColumn == null) throw new ArgumentException ("FunctionColumn is mandatory.");
if (FunctionColumn.Length > 60)
{
log.Warning("Length > 60 - truncated");
FunctionColumn = FunctionColumn.Substring(0,60);
}
Set_Value ("FunctionColumn", FunctionColumn);
}
/** Get Function Column.
@return Overwrite Column with Function */
public String GetFunctionColumn() 
{
return (String)Get_Value("FunctionColumn");
}
/** Set SQL Group Function.
@param IsGroupFunction This function will generate a Group By Clause */
public void SetIsGroupFunction (Boolean IsGroupFunction)
{
Set_Value ("IsGroupFunction", IsGroupFunction);
}
/** Get SQL Group Function.
@return This function will generate a Group By Clause */
public Boolean IsGroupFunction() 
{
Object oo = Get_Value("IsGroupFunction");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
