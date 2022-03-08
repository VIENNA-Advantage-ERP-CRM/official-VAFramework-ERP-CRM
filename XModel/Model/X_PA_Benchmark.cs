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
/** Generated Model for PA_Benchmark
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_Benchmark : PO
{
public X_PA_Benchmark (Context ctx, int PA_Benchmark_ID, Trx trxName) : base (ctx, PA_Benchmark_ID, trxName)
{
/** if (PA_Benchmark_ID == 0)
{
SetAccumulationType (null);
SetName (null);
SetPA_Benchmark_ID (0);
}
 */
}
public X_PA_Benchmark (Ctx ctx, int PA_Benchmark_ID, Trx trxName) : base (ctx, PA_Benchmark_ID, trxName)
{
/** if (PA_Benchmark_ID == 0)
{
SetAccumulationType (null);
SetName (null);
SetPA_Benchmark_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Benchmark (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Benchmark (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Benchmark (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_Benchmark()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381591L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064802L;
/** AD_Table_ID=833 */
public static int Table_ID;
 // =833;

/** TableName=PA_Benchmark */
public static String Table_Name="PA_Benchmark";

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
StringBuilder sb = new StringBuilder ("X_PA_Benchmark[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AccumulationType AD_Reference_ID=370 */
public static int ACCUMULATIONTYPE_AD_Reference_ID=370;
/** Average = A */
public static String ACCUMULATIONTYPE_Average = "A";
/** Sum = S */
public static String ACCUMULATIONTYPE_Sum = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAccumulationTypeValid (String test)
{
return test.Equals("A") || test.Equals("S");
}
/** Set Accumulation Type.
@param AccumulationType How to accumulate data on time axis */
public void SetAccumulationType (String AccumulationType)
{
if (AccumulationType == null) throw new ArgumentException ("AccumulationType is mandatory");
if (!IsAccumulationTypeValid(AccumulationType))
throw new ArgumentException ("AccumulationType Invalid value - " + AccumulationType + " - Reference_ID=370 - A - S");
if (AccumulationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
AccumulationType = AccumulationType.Substring(0,1);
}
Set_Value ("AccumulationType", AccumulationType);
}
/** Get Accumulation Type.
@return How to accumulate data on time axis */
public String GetAccumulationType() 
{
return (String)Get_Value("AccumulationType");
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
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
