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
/** Generated Model for PA_SLA_Measure
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_SLA_Measure : PO
{
public X_PA_SLA_Measure (Context ctx, int PA_SLA_Measure_ID, Trx trxName) : base (ctx, PA_SLA_Measure_ID, trxName)
{
/** if (PA_SLA_Measure_ID == 0)
{
SetDateTrx (DateTime.Now);
SetMeasureActual (0.0);
SetPA_SLA_Goal_ID (0);
SetPA_SLA_Measure_ID (0);
SetProcessed (false);	// N
}
 */
}
public X_PA_SLA_Measure (Ctx ctx, int PA_SLA_Measure_ID, Trx trxName) : base (ctx, PA_SLA_Measure_ID, trxName)
{
/** if (PA_SLA_Measure_ID == 0)
{
SetDateTrx (DateTime.Now);
SetMeasureActual (0.0);
SetPA_SLA_Goal_ID (0);
SetPA_SLA_Measure_ID (0);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_SLA_Measure (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_SLA_Measure (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_SLA_Measure (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_SLA_Measure()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382374L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065585L;
/** AD_Table_ID=743 */
public static int Table_ID;
 // =743;

/** TableName=PA_SLA_Measure */
public static String Table_Name="PA_SLA_Measure";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_PA_SLA_Measure[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
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
/** Set Transaction Date.
@param DateTrx Transaction Date */
public void SetDateTrx (DateTime? DateTrx)
{
if (DateTrx == null) throw new ArgumentException ("DateTrx is mandatory.");
Set_Value ("DateTrx", (DateTime?)DateTrx);
}
/** Get Transaction Date.
@return Transaction Date */
public DateTime? GetDateTrx() 
{
return (DateTime?)Get_Value("DateTrx");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDateTrx().ToString());
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
/** Set Measure Actual.
@param MeasureActual Actual value that has been measured. */
public void SetMeasureActual (Decimal? MeasureActual)
{
if (MeasureActual == null) throw new ArgumentException ("MeasureActual is mandatory.");
Set_Value ("MeasureActual", (Decimal?)MeasureActual);
}
/** Get Measure Actual.
@return Actual value that has been measured. */
public Decimal GetMeasureActual() 
{
Object bd =Get_Value("MeasureActual");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set SLA Goal.
@param PA_SLA_Goal_ID Service Level Agreement Goal */
public void SetPA_SLA_Goal_ID (int PA_SLA_Goal_ID)
{
if (PA_SLA_Goal_ID < 1) throw new ArgumentException ("PA_SLA_Goal_ID is mandatory.");
Set_ValueNoCheck ("PA_SLA_Goal_ID", PA_SLA_Goal_ID);
}
/** Get SLA Goal.
@return Service Level Agreement Goal */
public int GetPA_SLA_Goal_ID() 
{
Object ii = Get_Value("PA_SLA_Goal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set SLA Measure.
@param PA_SLA_Measure_ID Service Level Agreement Measure */
public void SetPA_SLA_Measure_ID (int PA_SLA_Measure_ID)
{
if (PA_SLA_Measure_ID < 1) throw new ArgumentException ("PA_SLA_Measure_ID is mandatory.");
Set_ValueNoCheck ("PA_SLA_Measure_ID", PA_SLA_Measure_ID);
}
/** Get SLA Measure.
@return Service Level Agreement Measure */
public int GetPA_SLA_Measure_ID() 
{
Object ii = Get_Value("PA_SLA_Measure_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_Value ("Record_ID", null);
else
Set_Value ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
