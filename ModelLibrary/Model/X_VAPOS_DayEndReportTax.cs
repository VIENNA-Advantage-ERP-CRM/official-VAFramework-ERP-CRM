namespace ViennaAdvantage.Model
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
/** Generated Model for VAPOS_DayEndReportTax
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPOS_DayEndReportTax : PO
{
public X_VAPOS_DayEndReportTax (Context ctx, int VAPOS_DayEndReportTax_ID, Trx trxName) : base (ctx, VAPOS_DayEndReportTax_ID, trxName)
{
/** if (VAPOS_DayEndReportTax_ID == 0)
{
SetVAPOS_DayEndReportTax_ID (0);
SetVAPOS_DayEndReport_ID (0);
}
 */
}
public X_VAPOS_DayEndReportTax (Ctx ctx, int VAPOS_DayEndReportTax_ID, Trx trxName) : base (ctx, VAPOS_DayEndReportTax_ID, trxName)
{
/** if (VAPOS_DayEndReportTax_ID == 0)
{
SetVAPOS_DayEndReportTax_ID (0);
SetVAPOS_DayEndReport_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPOS_DayEndReportTax (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPOS_DayEndReportTax (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPOS_DayEndReportTax (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPOS_DayEndReportTax()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27696788867491L;
/** Last Updated Timestamp 10/30/2014 3:35:50 PM */
public static long updatedMS = 1414663550702L;
/** AD_Table_ID=1000567 */
public static int Table_ID;
 // =1000567;

/** TableName=VAPOS_DayEndReportTax */
public static String Table_Name="VAPOS_DayEndReportTax";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
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
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_VAPOS_DayEndReportTax[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tax.
@param C_Tax_ID Tax identifier */
public void SetC_Tax_ID (int C_Tax_ID)
{
if (C_Tax_ID <= 0) Set_Value ("C_Tax_ID", null);
else
Set_Value ("C_Tax_ID", C_Tax_ID);
}
/** Get Tax.
@return Tax identifier */
public int GetC_Tax_ID() 
{
Object ii = Get_Value("C_Tax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set VAPOS_DayEndReportTax_ID.
@param VAPOS_DayEndReportTax_ID VAPOS_DayEndReportTax_ID */
public void SetVAPOS_DayEndReportTax_ID (int VAPOS_DayEndReportTax_ID)
{
if (VAPOS_DayEndReportTax_ID < 1) throw new ArgumentException ("VAPOS_DayEndReportTax_ID is mandatory.");
Set_ValueNoCheck ("VAPOS_DayEndReportTax_ID", VAPOS_DayEndReportTax_ID);
}
/** Get VAPOS_DayEndReportTax_ID.
@return VAPOS_DayEndReportTax_ID */
public int GetVAPOS_DayEndReportTax_ID() 
{
Object ii = Get_Value("VAPOS_DayEndReportTax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Day End Report.
@param VAPOS_DayEndReport_ID Day End Report */
public void SetVAPOS_DayEndReport_ID (int VAPOS_DayEndReport_ID)
{
if (VAPOS_DayEndReport_ID < 1) throw new ArgumentException ("VAPOS_DayEndReport_ID is mandatory.");
Set_ValueNoCheck ("VAPOS_DayEndReport_ID", VAPOS_DayEndReport_ID);
}
/** Get Day End Report.
@return Day End Report */
public int GetVAPOS_DayEndReport_ID() 
{
Object ii = Get_Value("VAPOS_DayEndReport_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Amount.
@param VAPOS_TaxAmount Tax Amount */
public void SetVAPOS_TaxAmount (Decimal? VAPOS_TaxAmount)
{
Set_Value ("VAPOS_TaxAmount", (Decimal?)VAPOS_TaxAmount);
}
/** Get Tax Amount.
@return Tax Amount */
public Decimal GetVAPOS_TaxAmount() 
{
Object bd =Get_Value("VAPOS_TaxAmount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
