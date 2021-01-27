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
/** Generated Model for VAPA_Std_BMData
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPA_Std_BMData : PO
{
public X_VAPA_Std_BMData (Context ctx, int VAPA_Std_BMData_ID, Trx trxName) : base (ctx, VAPA_Std_BMData_ID, trxName)
{
/** if (VAPA_Std_BMData_ID == 0)
{
SetBenchmarkDate (DateTime.Now);
SetBenchmarkValue (0.0);
SetName (null);
SetVAPA_Std_BMData_ID (0);
SetVAPA_Std_BM_ID (0);
}
 */
}
public X_VAPA_Std_BMData (Ctx ctx, int VAPA_Std_BMData_ID, Trx trxName) : base (ctx, VAPA_Std_BMData_ID, trxName)
{
/** if (VAPA_Std_BMData_ID == 0)
{
SetBenchmarkDate (DateTime.Now);
SetBenchmarkValue (0.0);
SetName (null);
SetVAPA_Std_BMData_ID (0);
SetVAPA_Std_BM_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Std_BMData (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Std_BMData (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Std_BMData (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPA_Std_BMData()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381638L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064849L;
/** VAF_TableView_ID=834 */
public static int Table_ID;
 // =834;

/** TableName=VAPA_Std_BMData */
public static String Table_Name="VAPA_Std_BMData";

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
StringBuilder sb = new StringBuilder ("X_VAPA_Std_BMData[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Date.
@param BenchmarkDate Benchmark Date */
public void SetBenchmarkDate (DateTime? BenchmarkDate)
{
if (BenchmarkDate == null) throw new ArgumentException ("BenchmarkDate is mandatory.");
Set_Value ("BenchmarkDate", (DateTime?)BenchmarkDate);
}
/** Get Date.
@return Benchmark Date */
public DateTime? GetBenchmarkDate() 
{
return (DateTime?)Get_Value("BenchmarkDate");
}
/** Set Value.
@param BenchmarkValue Benchmark Value */
public void SetBenchmarkValue (Decimal? BenchmarkValue)
{
if (BenchmarkValue == null) throw new ArgumentException ("BenchmarkValue is mandatory.");
Set_Value ("BenchmarkValue", (Decimal?)BenchmarkValue);
}
/** Get Value.
@return Benchmark Value */
public Decimal GetBenchmarkValue() 
{
Object bd =Get_Value("BenchmarkValue");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Benchmark Data.
@param VAPA_Std_BMData_ID Performance Benchmark Data Point */
public void SetVAPA_Std_BMData_ID (int VAPA_Std_BMData_ID)
{
if (VAPA_Std_BMData_ID < 1) throw new ArgumentException ("VAPA_Std_BMData_ID is mandatory.");
Set_ValueNoCheck ("VAPA_Std_BMData_ID", VAPA_Std_BMData_ID);
}
/** Get Benchmark Data.
@return Performance Benchmark Data Point */
public int GetVAPA_Std_BMData_ID() 
{
Object ii = Get_Value("VAPA_Std_BMData_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Benchmark.
@param VAPA_Std_BM_ID Performance Benchmark */
public void SetVAPA_Std_BM_ID (int VAPA_Std_BM_ID)
{
if (VAPA_Std_BM_ID < 1) throw new ArgumentException ("VAPA_Std_BM_ID is mandatory.");
Set_ValueNoCheck ("VAPA_Std_BM_ID", VAPA_Std_BM_ID);
}
/** Get Benchmark.
@return Performance Benchmark */
public int GetVAPA_Std_BM_ID() 
{
Object ii = Get_Value("VAPA_Std_BM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
