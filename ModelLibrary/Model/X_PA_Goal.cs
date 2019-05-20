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
/** Generated Model for PA_Goal
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_Goal : PO
{
public X_PA_Goal (Context ctx, int PA_Goal_ID, Trx trxName) : base (ctx, PA_Goal_ID, trxName)
{
/** if (PA_Goal_ID == 0)
{
SetIsSummary (false);
SetMeasureScope (null);
SetMeasureTarget (0.0);
SetName (null);
SetPA_ColorSchema_ID (0);
SetPA_Goal_ID (0);
SetSeqNo (0);
}
 */
}
public X_PA_Goal (Ctx ctx, int PA_Goal_ID, Trx trxName) : base (ctx, PA_Goal_ID, trxName)
{
/** if (PA_Goal_ID == 0)
{
SetIsSummary (false);
SetMeasureScope (null);
SetMeasureTarget (0.0);
SetName (null);
SetPA_ColorSchema_ID (0);
SetPA_Goal_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Goal (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Goal (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_Goal (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_Goal()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381810L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065021L;
/** AD_Table_ID=440 */
public static int Table_ID;
 // =440;

/** TableName=PA_Goal */
public static String Table_Name="PA_Goal";

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
StringBuilder sb = new StringBuilder ("X_PA_Goal[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param AD_Role_ID Responsibility Role */
public void SetAD_Role_ID (int AD_Role_ID)
{
if (AD_Role_ID <= 0) Set_Value ("AD_Role_ID", null);
else
Set_Value ("AD_Role_ID", AD_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetAD_Role_ID() 
{
Object ii = Get_Value("AD_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Date From.
@param DateFrom Starting date for a range */
public void SetDateFrom (DateTime? DateFrom)
{
Set_Value ("DateFrom", (DateTime?)DateFrom);
}
/** Get Date From.
@return Starting date for a range */
public DateTime? GetDateFrom() 
{
return (DateTime?)Get_Value("DateFrom");
}
/** Set Date last run.
@param DateLastRun Date the process was last run. */
public void SetDateLastRun (DateTime? DateLastRun)
{
Set_ValueNoCheck ("DateLastRun", (DateTime?)DateLastRun);
}
/** Get Date last run.
@return Date the process was last run. */
public DateTime? GetDateLastRun() 
{
return (DateTime?)Get_Value("DateLastRun");
}
/** Set Date To.
@param DateTo End date of a date range */
public void SetDateTo (DateTime? DateTo)
{
Set_Value ("DateTo", (DateTime?)DateTo);
}
/** Get Date To.
@return End date of a date range */
public DateTime? GetDateTo() 
{
return (DateTime?)Get_Value("DateTo");
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
/** Set Performance Goal.
@param GoalPerformance Target achievement from 0..1 */
public void SetGoalPerformance (Decimal? GoalPerformance)
{
Set_ValueNoCheck ("GoalPerformance", (Decimal?)GoalPerformance);
}
/** Get Performance Goal.
@return Target achievement from 0..1 */
public Decimal GetGoalPerformance() 
{
Object bd =Get_Value("GoalPerformance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Measure Actual.
@param MeasureActual Actual value that has been measured. */
public void SetMeasureActual (Decimal? MeasureActual)
{
Set_ValueNoCheck ("MeasureActual", (Decimal?)MeasureActual);
}
/** Get Measure Actual.
@return Actual value that has been measured. */
public Decimal GetMeasureActual() 
{
Object bd =Get_Value("MeasureActual");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** MeasureDisplay AD_Reference_ID=367 */
public static int MEASUREDISPLAY_AD_Reference_ID=367;
/** Total = 0 */
public static String MEASUREDISPLAY_Total = "0";
/** Year = 1 */
public static String MEASUREDISPLAY_Year = "1";
/** Quarter = 3 */
public static String MEASUREDISPLAY_Quarter = "3";
/** Month = 5 */
public static String MEASUREDISPLAY_Month = "5";
/** Week = 7 */
public static String MEASUREDISPLAY_Week = "7";
/** Day = 8 */
public static String MEASUREDISPLAY_Day = "8";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMeasureDisplayValid (String test)
{
return test == null || test.Equals("0") || test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("8");
}
/** Set Measure Display.
@param MeasureDisplay Measure Scope initially displayed */
public void SetMeasureDisplay (String MeasureDisplay)
{
if (!IsMeasureDisplayValid(MeasureDisplay))
throw new ArgumentException ("MeasureDisplay Invalid value - " + MeasureDisplay + " - Reference_ID=367 - 0 - 1 - 3 - 5 - 7 - 8");
if (MeasureDisplay != null && MeasureDisplay.Length > 1)
{
log.Warning("Length > 1 - truncated");
MeasureDisplay = MeasureDisplay.Substring(0,1);
}
Set_Value ("MeasureDisplay", MeasureDisplay);
}
/** Get Measure Display.
@return Measure Scope initially displayed */
public String GetMeasureDisplay() 
{
return (String)Get_Value("MeasureDisplay");
}

/** MeasureScope AD_Reference_ID=367 */
public static int MEASURESCOPE_AD_Reference_ID=367;
/** Total = 0 */
public static String MEASURESCOPE_Total = "0";
/** Year = 1 */
public static String MEASURESCOPE_Year = "1";
/** Quarter = 3 */
public static String MEASURESCOPE_Quarter = "3";
/** Month = 5 */
public static String MEASURESCOPE_Month = "5";
/** Week = 7 */
public static String MEASURESCOPE_Week = "7";
/** Day = 8 */
public static String MEASURESCOPE_Day = "8";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMeasureScopeValid (String test)
{
return test.Equals("0") || test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("8");
}
/** Set Measure Scope.
@param MeasureScope Performance Measure Scope */
public void SetMeasureScope (String MeasureScope)
{
if (MeasureScope == null) throw new ArgumentException ("MeasureScope is mandatory");
if (!IsMeasureScopeValid(MeasureScope))
throw new ArgumentException ("MeasureScope Invalid value - " + MeasureScope + " - Reference_ID=367 - 0 - 1 - 3 - 5 - 7 - 8");
if (MeasureScope.Length > 1)
{
log.Warning("Length > 1 - truncated");
MeasureScope = MeasureScope.Substring(0,1);
}
Set_Value ("MeasureScope", MeasureScope);
}
/** Get Measure Scope.
@return Performance Measure Scope */
public String GetMeasureScope() 
{
return (String)Get_Value("MeasureScope");
}
/** Set Measure Target.
@param MeasureTarget Target value for measure */
public void SetMeasureTarget (Decimal? MeasureTarget)
{
if (MeasureTarget == null) throw new ArgumentException ("MeasureTarget is mandatory.");
Set_Value ("MeasureTarget", (Decimal?)MeasureTarget);
}
/** Get Measure Target.
@return Target value for measure */
public Decimal GetMeasureTarget() 
{
Object bd =Get_Value("MeasureTarget");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Note.
@param Note Optional additional user defined information */
public void SetNote (String Note)
{
if (Note != null && Note.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Note = Note.Substring(0,2000);
}
Set_Value ("Note", Note);
}
/** Get Note.
@return Optional additional user defined information */
public String GetNote() 
{
return (String)Get_Value("Note");
}
/** Set Color Schema.
@param PA_ColorSchema_ID Performance Color Schema */
public void SetPA_ColorSchema_ID (int PA_ColorSchema_ID)
{
if (PA_ColorSchema_ID < 1) throw new ArgumentException ("PA_ColorSchema_ID is mandatory.");
Set_Value ("PA_ColorSchema_ID", PA_ColorSchema_ID);
}
/** Get Color Schema.
@return Performance Color Schema */
public int GetPA_ColorSchema_ID() 
{
Object ii = Get_Value("PA_ColorSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PA_GoalParent_ID AD_Reference_ID=230 */
public static int PA_GOALPARENT_ID_AD_Reference_ID=230;
/** Set Parent Goal.
@param PA_GoalParent_ID Parent Goal */
public void SetPA_GoalParent_ID (int PA_GoalParent_ID)
{
if (PA_GoalParent_ID <= 0) Set_Value ("PA_GoalParent_ID", null);
else
Set_Value ("PA_GoalParent_ID", PA_GoalParent_ID);
}
/** Get Parent Goal.
@return Parent Goal */
public int GetPA_GoalParent_ID() 
{
Object ii = Get_Value("PA_GoalParent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Goal.
@param PA_Goal_ID Performance Goal */
public void SetPA_Goal_ID (int PA_Goal_ID)
{
if (PA_Goal_ID < 1) throw new ArgumentException ("PA_Goal_ID is mandatory.");
Set_ValueNoCheck ("PA_Goal_ID", PA_Goal_ID);
}
/** Get Goal.
@return Performance Goal */
public int GetPA_Goal_ID() 
{
Object ii = Get_Value("PA_Goal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Measure.
@param PA_Measure_ID Concrete Performance Measurement */
public void SetPA_Measure_ID (int PA_Measure_ID)
{
if (PA_Measure_ID <= 0) Set_Value ("PA_Measure_ID", null);
else
Set_Value ("PA_Measure_ID", PA_Measure_ID);
}
/** Get Measure.
@return Concrete Performance Measurement */
public int GetPA_Measure_ID() 
{
Object ii = Get_Value("PA_Measure_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Relative Weight.
@param RelativeWeight Relative weight of this step (0 = ignored) */
public void SetRelativeWeight (Decimal? RelativeWeight)
{
Set_Value ("RelativeWeight", (Decimal?)RelativeWeight);
}
/** Get Relative Weight.
@return Relative weight of this step (0 = ignored) */
public Decimal GetRelativeWeight() 
{
Object bd =Get_Value("RelativeWeight");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
