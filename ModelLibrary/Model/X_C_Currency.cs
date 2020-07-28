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
/** Generated Model for C_Currency
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Currency : PO
{
public X_C_Currency (Context ctx, int C_Currency_ID, Trx trxName) : base (ctx, C_Currency_ID, trxName)
{
/** if (C_Currency_ID == 0)
{
SetC_Currency_ID (0);
SetCostingPrecision (0);	// 4
SetDescription (null);
SetISO_Code (null);
SetIsEMUMember (false);	// N
SetIsEuro (false);	// N
SetStdPrecision (0);	// 2
}
 */
}
public X_C_Currency (Ctx ctx, int C_Currency_ID, Trx trxName) : base (ctx, C_Currency_ID, trxName)
{
/** if (C_Currency_ID == 0)
{
SetC_Currency_ID (0);
SetCostingPrecision (0);	// 4
SetDescription (null);
SetISO_Code (null);
SetIsEMUMember (false);	// N
SetIsEuro (false);	// N
SetStdPrecision (0);	// 2
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Currency (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Currency (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Currency (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Currency()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371529L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054740L;
/** AD_Table_ID=141 */
public static int Table_ID;
 // =141;

/** TableName=C_Currency */
public static String Table_Name="C_Currency";

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
StringBuilder sb = new StringBuilder ("X_C_Currency[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
Set_ValueNoCheck ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Costing Precision.
@param CostingPrecision Rounding used costing calculations */
public void SetCostingPrecision (int CostingPrecision)
{
Set_Value ("CostingPrecision", CostingPrecision);
}
/** Get Costing Precision.
@return Rounding used costing calculations */
public int GetCostingPrecision() 
{
Object ii = Get_Value("CostingPrecision");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Symbol.
@param CurSymbol Symbol of the currency (opt used for printing only) */
public void SetCurSymbol (String CurSymbol)
{
if (CurSymbol != null && CurSymbol.Length > 10)
{
log.Warning("Length > 10 - truncated");
CurSymbol = CurSymbol.Substring(0,10);
}
Set_Value ("CurSymbol", CurSymbol);
}
/** Get Symbol.
@return Symbol of the currency (opt used for printing only) */
public String GetCurSymbol() 
{
return (String)Get_Value("CurSymbol");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description == null) throw new ArgumentException ("Description is mandatory.");
if (Description.Length > 255)
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
/** Set EMU Entry Date.
@param EMUEntryDate Date when the currency joined / will join the EMU */
public void SetEMUEntryDate (DateTime? EMUEntryDate)
{
Set_Value ("EMUEntryDate", (DateTime?)EMUEntryDate);
}
/** Get EMU Entry Date.
@return Date when the currency joined / will join the EMU */
public DateTime? GetEMUEntryDate() 
{
return (DateTime?)Get_Value("EMUEntryDate");
}
/** Set EMU Rate.
@param EMURate Official rate to the Euro */
public void SetEMURate (Decimal? EMURate)
{
Set_Value ("EMURate", (Decimal?)EMURate);
}
/** Get EMU Rate.
@return Official rate to the Euro */
public Decimal GetEMURate() 
{
Object bd =Get_Value("EMURate");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set ISO Currency Code.
@param ISO_Code Three letter ISO 4217 Code of the Currency */
public void SetISO_Code (String ISO_Code)
{
if (ISO_Code == null) throw new ArgumentException ("ISO_Code is mandatory.");
if (ISO_Code.Length > 3)
{
log.Warning("Length > 3 - truncated");
ISO_Code = ISO_Code.Substring(0,3);
}
Set_Value ("ISO_Code", ISO_Code);
}
/** Get ISO Currency Code.
@return Three letter ISO 4217 Code of the Currency */
public String GetISO_Code() 
{
return (String)Get_Value("ISO_Code");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetISO_Code());
}
/** Set EMU Member.
@param IsEMUMember This currency is member if the European Monetary Union */
public void SetIsEMUMember (Boolean IsEMUMember)
{
Set_Value ("IsEMUMember", IsEMUMember);
}
/** Get EMU Member.
@return This currency is member if the European Monetary Union */
public Boolean IsEMUMember() 
{
Object oo = Get_Value("IsEMUMember");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set The Euro Currency.
@param IsEuro This currency is the Euro */
public void SetIsEuro (Boolean IsEuro)
{
Set_Value ("IsEuro", IsEuro);
}
/** Get The Euro Currency.
@return This currency is the Euro */
public Boolean IsEuro() 
{
Object oo = Get_Value("IsEuro");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Standard Precision.
@param StdPrecision Rule for rounding  calculated amounts */
public void SetStdPrecision (int StdPrecision)
{
Set_Value ("StdPrecision", StdPrecision);
}
/** Get Standard Precision.
@return Rule for rounding  calculated amounts */
public int GetStdPrecision() 
{
Object ii = Get_Value("StdPrecision");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
