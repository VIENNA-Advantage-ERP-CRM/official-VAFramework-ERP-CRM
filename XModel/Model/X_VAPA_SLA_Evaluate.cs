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
/** Generated Model for VAPA_SLA_Evaluate
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPA_SLA_Evaluate : PO
{
public X_VAPA_SLA_Evaluate (Context ctx, int VAPA_SLA_Evaluate_ID, Trx trxName) : base (ctx, VAPA_SLA_Evaluate_ID, trxName)
{
/** if (VAPA_SLA_Evaluate_ID == 0)
{
SetDateTrx (DateTime.Now);
SetMeasureActual (0.0);
SetVAPA_SLA_Target_ID (0);
SetVAPA_SLA_Evaluate_ID (0);
SetProcessed (false);	// N
}
 */
}
public X_VAPA_SLA_Evaluate (Ctx ctx, int VAPA_SLA_Evaluate_ID, Trx trxName) : base (ctx, VAPA_SLA_Evaluate_ID, trxName)
{
/** if (VAPA_SLA_Evaluate_ID == 0)
{
SetDateTrx (DateTime.Now);
SetMeasureActual (0.0);
SetVAPA_SLA_Target_ID (0);
SetVAPA_SLA_Evaluate_ID (0);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Evaluate (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Evaluate (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_SLA_Evaluate (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPA_SLA_Evaluate()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382374L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065585L;
/** VAF_TableView_ID=743 */
public static int Table_ID;
 // =743;

/** TableName=VAPA_SLA_Evaluate */
public static String Table_Name="VAPA_SLA_Evaluate";

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
StringBuilder sb = new StringBuilder ("X_VAPA_SLA_Evaluate[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);
else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
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
@param VAPA_SLA_Target_ID Service Level Agreement Goal */
public void SetVAPA_SLA_Target_ID (int VAPA_SLA_Target_ID)
{
if (VAPA_SLA_Target_ID < 1) throw new ArgumentException ("VAPA_SLA_Target_ID is mandatory.");
Set_ValueNoCheck ("VAPA_SLA_Target_ID", VAPA_SLA_Target_ID);
}
/** Get SLA Goal.
@return Service Level Agreement Goal */
public int GetVAPA_SLA_Target_ID() 
{
Object ii = Get_Value("VAPA_SLA_Target_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set SLA Measure.
@param VAPA_SLA_Evaluate_ID Service Level Agreement Measure */
public void SetVAPA_SLA_Evaluate_ID (int VAPA_SLA_Evaluate_ID)
{
if (VAPA_SLA_Evaluate_ID < 1) throw new ArgumentException ("VAPA_SLA_Evaluate_ID is mandatory.");
Set_ValueNoCheck ("VAPA_SLA_Evaluate_ID", VAPA_SLA_Evaluate_ID);
}
/** Get SLA Measure.
@return Service Level Agreement Measure */
public int GetVAPA_SLA_Evaluate_ID() 
{
Object ii = Get_Value("VAPA_SLA_Evaluate_ID");
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
