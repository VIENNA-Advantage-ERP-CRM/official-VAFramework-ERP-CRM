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
/** Generated Model for PA_BenchmarkData
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_BenchmarkData : PO
{
public X_PA_BenchmarkData (Context ctx, int PA_BenchmarkData_ID, Trx trxName) : base (ctx, PA_BenchmarkData_ID, trxName)
{
/** if (PA_BenchmarkData_ID == 0)
{
SetBenchmarkDate (DateTime.Now);
SetBenchmarkValue (0.0);
SetName (null);
SetPA_BenchmarkData_ID (0);
SetPA_Benchmark_ID (0);
}
 */
}
public X_PA_BenchmarkData (Ctx ctx, int PA_BenchmarkData_ID, Trx trxName) : base (ctx, PA_BenchmarkData_ID, trxName)
{
/** if (PA_BenchmarkData_ID == 0)
{
SetBenchmarkDate (DateTime.Now);
SetBenchmarkValue (0.0);
SetName (null);
SetPA_BenchmarkData_ID (0);
SetPA_Benchmark_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_BenchmarkData (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_BenchmarkData (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_BenchmarkData (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_BenchmarkData()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381638L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064849L;
/** AD_Table_ID=834 */
public static int Table_ID;
 // =834;

/** TableName=PA_BenchmarkData */
public static String Table_Name="PA_BenchmarkData";

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
StringBuilder sb = new StringBuilder ("X_PA_BenchmarkData[").Append(Get_ID()).Append("]");
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
@param PA_BenchmarkData_ID Performance Benchmark Data Point */
public void SetPA_BenchmarkData_ID (int PA_BenchmarkData_ID)
{
if (PA_BenchmarkData_ID < 1) throw new ArgumentException ("PA_BenchmarkData_ID is mandatory.");
Set_ValueNoCheck ("PA_BenchmarkData_ID", PA_BenchmarkData_ID);
}
/** Get Benchmark Data.
@return Performance Benchmark Data Point */
public int GetPA_BenchmarkData_ID() 
{
Object ii = Get_Value("PA_BenchmarkData_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Benchmark.
@param PA_Benchmark_ID Performance Benchmark */
public void SetPA_Benchmark_ID (int PA_Benchmark_ID)
{
if (PA_Benchmark_ID < 1) throw new ArgumentException ("PA_Benchmark_ID is mandatory.");
Set_ValueNoCheck ("PA_Benchmark_ID", PA_Benchmark_ID);
}
/** Get Benchmark.
@return Performance Benchmark */
public int GetPA_Benchmark_ID() 
{
Object ii = Get_Value("PA_Benchmark_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
