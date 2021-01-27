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
/** Generated Model for VAPA_Target
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAPA_Target : PO
{
public X_VAPA_Target (Context ctx, int VAPA_Target_ID, Trx trxName) : base (ctx, VAPA_Target_ID, trxName)
{
/** if (VAPA_Target_ID == 0)
{
SetIsSummary (false);
SetMeasureScope (null);
SetMeasureTarget (0.0);
SetName (null);
SetVAPA_Color_ID (0);
SetVAPA_Target_ID (0);
SetSeqNo (0);
}
 */
}
public X_VAPA_Target (Ctx ctx, int VAPA_Target_ID, Trx trxName) : base (ctx, VAPA_Target_ID, trxName)
{
/** if (VAPA_Target_ID == 0)
{
SetIsSummary (false);
SetMeasureScope (null);
SetMeasureTarget (0.0);
SetName (null);
SetVAPA_Color_ID (0);
SetVAPA_Target_ID (0);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Target (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Target (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAPA_Target (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAPA_Target()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381810L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065021L;
/** VAF_TableView_ID=440 */
public static int Table_ID;
 // =440;

/** TableName=VAPA_Target */
public static String Table_Name="VAPA_Target";

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
StringBuilder sb = new StringBuilder ("X_VAPA_Target[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Role.
@param VAF_Role_ID Responsibility Role */
public void SetVAF_Role_ID (int VAF_Role_ID)
{
if (VAF_Role_ID <= 0) Set_Value ("VAF_Role_ID", null);
else
Set_Value ("VAF_Role_ID", VAF_Role_ID);
}
/** Get Role.
@return Responsibility Role */
public int GetVAF_Role_ID() 
{
Object ii = Get_Value("VAF_Role_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User/Contact.
@param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
public void SetVAF_UserContact_ID (int VAF_UserContact_ID)
{
if (VAF_UserContact_ID <= 0) Set_Value ("VAF_UserContact_ID", null);
else
Set_Value ("VAF_UserContact_ID", VAF_UserContact_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public int GetVAF_UserContact_ID() 
{
Object ii = Get_Value("VAF_UserContact_ID");
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

/** MeasureDisplay VAF_Control_Ref_ID=367 */
public static int MEASUREDISPLAY_VAF_Control_Ref_ID=367;
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

/** MeasureScope VAF_Control_Ref_ID=367 */
public static int MEASURESCOPE_VAF_Control_Ref_ID=367;
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
@param VAPA_Color_ID Performance Color Schema */
public void SetVAPA_Color_ID (int VAPA_Color_ID)
{
if (VAPA_Color_ID < 1) throw new ArgumentException ("VAPA_Color_ID is mandatory.");
Set_Value ("VAPA_Color_ID", VAPA_Color_ID);
}
/** Get Color Schema.
@return Performance Color Schema */
public int GetVAPA_Color_ID() 
{
Object ii = Get_Value("VAPA_Color_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** VAPA_TargetParent_ID VAF_Control_Ref_ID=230 */
public static int VAPA_TARGETPARENT_ID_VAF_Control_Ref_ID=230;
/** Set Parent Goal.
@param VAPA_TargetParent_ID Parent Goal */
public void SetVAPA_TargetParent_ID (int VAPA_TargetParent_ID)
{
if (VAPA_TargetParent_ID <= 0) Set_Value ("VAPA_TargetParent_ID", null);
else
Set_Value ("VAPA_TargetParent_ID", VAPA_TargetParent_ID);
}
/** Get Parent Goal.
@return Parent Goal */
public int GetVAPA_TargetParent_ID() 
{
Object ii = Get_Value("VAPA_TargetParent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Goal.
@param VAPA_Target_ID Performance Goal */
public void SetVAPA_Target_ID (int VAPA_Target_ID)
{
if (VAPA_Target_ID < 1) throw new ArgumentException ("VAPA_Target_ID is mandatory.");
Set_ValueNoCheck ("VAPA_Target_ID", VAPA_Target_ID);
}
/** Get Goal.
@return Performance Goal */
public int GetVAPA_Target_ID() 
{
Object ii = Get_Value("VAPA_Target_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Measure.
@param VAPA_Evaluate_ID Concrete Performance Measurement */
public void SetVAPA_Evaluate_ID (int VAPA_Evaluate_ID)
{
if (VAPA_Evaluate_ID <= 0) Set_Value ("VAPA_Evaluate_ID", null);
else
Set_Value ("VAPA_Evaluate_ID", VAPA_Evaluate_ID);
}
/** Get Measure.
@return Concrete Performance Measurement */
public int GetVAPA_Evaluate_ID() 
{
Object ii = Get_Value("VAPA_Evaluate_ID");
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
