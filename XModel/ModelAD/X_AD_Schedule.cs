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
/** Generated Model for AD_Schedule
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Schedule : PO
{
public X_AD_Schedule (Context ctx, int AD_Schedule_ID, Trx trxName) : base (ctx, AD_Schedule_ID, trxName)
{
/** if (AD_Schedule_ID == 0)
{
SetAD_Schedule_ID (0);
SetName (null);
SetOnFriday (true);	// Y
SetOnMonday (true);	// Y
SetOnSaturday (false);	// N
SetOnSunday (false);	// N
SetOnThursday (true);	// Y
SetOnTuesday (true);	// Y
SetOnWednesday (true);	// Y
SetRunOnlySpecifiedTime (false);	// N
SetScheduleType (null);	// F
}
 */
}
public X_AD_Schedule (Ctx ctx, int AD_Schedule_ID, Trx trxName) : base (ctx, AD_Schedule_ID, trxName)
{
/** if (AD_Schedule_ID == 0)
{
SetAD_Schedule_ID (0);
SetName (null);
SetOnFriday (true);	// Y
SetOnMonday (true);	// Y
SetOnSaturday (false);	// N
SetOnSunday (false);	// N
SetOnThursday (true);	// Y
SetOnTuesday (true);	// Y
SetOnWednesday (true);	// Y
SetRunOnlySpecifiedTime (false);	// N
SetScheduleType (null);	// F
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Schedule (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Schedule (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Schedule (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Schedule()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514363912L;
/** Last Updated Timestamp 7/29/2010 1:07:27 PM */
public static long updatedMS = 1280389047123L;
/** AD_Table_ID=916 */
public static int Table_ID;
 // =916;

/** TableName=AD_Schedule */
public static String Table_Name="AD_Schedule";

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
StringBuilder sb = new StringBuilder ("X_AD_Schedule[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Schedule.
@param AD_Schedule_ID Execution Schedule */
public void SetAD_Schedule_ID (int AD_Schedule_ID)
{
if (AD_Schedule_ID < 1) throw new ArgumentException ("AD_Schedule_ID is mandatory.");
Set_ValueNoCheck ("AD_Schedule_ID", AD_Schedule_ID);
}
/** Get Schedule.
@return Execution Schedule */
public int GetAD_Schedule_ID() 
{
Object ii = Get_Value("AD_Schedule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Frequency.
@param Frequency Frequency of events */
public void SetFrequency (int Frequency)
{
Set_Value ("Frequency", Frequency);
}
/** Get Frequency.
@return Frequency of events */
public int GetFrequency() 
{
Object ii = Get_Value("Frequency");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** FrequencyType AD_Reference_ID=221 */
public static int FREQUENCYTYPE_AD_Reference_ID=221;
/** Day = D */
public static String FREQUENCYTYPE_Day = "D";
/** Hour = H */
public static String FREQUENCYTYPE_Hour = "H";
/** Minute = M */
public static String FREQUENCYTYPE_Minute = "M";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsFrequencyTypeValid (String test)
{
return test == null || test.Equals("D") || test.Equals("H") || test.Equals("M");
}
/** Set Frequency Type.
@param FrequencyType Frequency of event */
public void SetFrequencyType (String FrequencyType)
{
if (!IsFrequencyTypeValid(FrequencyType))
throw new ArgumentException ("FrequencyType Invalid value - " + FrequencyType + " - Reference_ID=221 - D - H - M");
if (FrequencyType != null && FrequencyType.Length > 1)
{
log.Warning("Length > 1 - truncated");
FrequencyType = FrequencyType.Substring(0,1);
}
Set_Value ("FrequencyType", FrequencyType);
}
/** Get Frequency Type.
@return Frequency of event */
public String GetFrequencyType() 
{
return (String)Get_Value("FrequencyType");
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
/** Set Day of the Month.
@param MonthDay Day of the month 1 to 28/29/30/31 */
public void SetMonthDay (int MonthDay)
{
Set_Value ("MonthDay", MonthDay);
}
/** Get Day of the Month.
@return Day of the month 1 to 28/29/30/31 */
public int GetMonthDay() 
{
Object ii = Get_Value("MonthDay");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 120)
{
log.Warning("Length > 120 - truncated");
Name = Name.Substring(0,120);
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
/** Set Friday.
@param OnFriday Available on Fridays */
public void SetOnFriday (Boolean OnFriday)
{
Set_Value ("OnFriday", OnFriday);
}
/** Get Friday.
@return Available on Fridays */
public Boolean IsOnFriday() 
{
Object oo = Get_Value("OnFriday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Monday.
@param OnMonday Available on Mondays */
public void SetOnMonday (Boolean OnMonday)
{
Set_Value ("OnMonday", OnMonday);
}
/** Get Monday.
@return Available on Mondays */
public Boolean IsOnMonday() 
{
Object oo = Get_Value("OnMonday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Saturday.
@param OnSaturday Available on Saturday */
public void SetOnSaturday (Boolean OnSaturday)
{
Set_Value ("OnSaturday", OnSaturday);
}
/** Get Saturday.
@return Available on Saturday */
public Boolean IsOnSaturday() 
{
Object oo = Get_Value("OnSaturday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Sunday.
@param OnSunday Available on Sundays */
public void SetOnSunday (Boolean OnSunday)
{
Set_Value ("OnSunday", OnSunday);
}
/** Get Sunday.
@return Available on Sundays */
public Boolean IsOnSunday() 
{
Object oo = Get_Value("OnSunday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Thursday.
@param OnThursday Available on Thursdays */
public void SetOnThursday (Boolean OnThursday)
{
Set_Value ("OnThursday", OnThursday);
}
/** Get Thursday.
@return Available on Thursdays */
public Boolean IsOnThursday() 
{
Object oo = Get_Value("OnThursday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Tuesday.
@param OnTuesday Available on Tuesdays */
public void SetOnTuesday (Boolean OnTuesday)
{
Set_Value ("OnTuesday", OnTuesday);
}
/** Get Tuesday.
@return Available on Tuesdays */
public Boolean IsOnTuesday() 
{
Object oo = Get_Value("OnTuesday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Wednesday.
@param OnWednesday Available on Wednesdays */
public void SetOnWednesday (Boolean OnWednesday)
{
Set_Value ("OnWednesday", OnWednesday);
}
/** Get Wednesday.
@return Available on Wednesdays */
public Boolean IsOnWednesday() 
{
Object oo = Get_Value("OnWednesday");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Only IP.
@param RunOnlyOnIP Run only on this IP address */
public void SetRunOnlyOnIP (String RunOnlyOnIP)
{
if (RunOnlyOnIP != null && RunOnlyOnIP.Length > 60)
{
log.Warning("Length > 60 - truncated");
RunOnlyOnIP = RunOnlyOnIP.Substring(0,60);
}
Set_Value ("RunOnlyOnIP", RunOnlyOnIP);
}
/** Get Only IP.
@return Run only on this IP address */
public String GetRunOnlyOnIP() 
{
return (String)Get_Value("RunOnlyOnIP");
}
/** Set Only Specified Time.
@param RunOnlySpecifiedTime Run the Process only at Specified Time */
public void SetRunOnlySpecifiedTime (Boolean RunOnlySpecifiedTime)
{
Set_Value ("RunOnlySpecifiedTime", RunOnlySpecifiedTime);
}
/** Get Only Specified Time.
@return Run the Process only at Specified Time */
public Boolean IsRunOnlySpecifiedTime() 
{
Object oo = Get_Value("RunOnlySpecifiedTime");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Tolerance Minutes.
@param RunOnlySpecifiedTolMin The tolerance in Minutes */
public void SetRunOnlySpecifiedTolMin (int RunOnlySpecifiedTolMin)
{
Set_Value ("RunOnlySpecifiedTolMin", RunOnlySpecifiedTolMin);
}
/** Get Tolerance Minutes.
@return The tolerance in Minutes */
public int GetRunOnlySpecifiedTolMin() 
{
Object ii = Get_Value("RunOnlySpecifiedTolMin");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Target Hour.
@param ScheduleHour 24 hour target start time of the process */
public void SetScheduleHour (int ScheduleHour)
{
Set_Value ("ScheduleHour", ScheduleHour);
}
/** Get Target Hour.
@return 24 hour target start time of the process */
public int GetScheduleHour() 
{
Object ii = Get_Value("ScheduleHour");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Target Minute.
@param ScheduleMinute Minute of process start time */
public void SetScheduleMinute (int ScheduleMinute)
{
Set_Value ("ScheduleMinute", ScheduleMinute);
}
/** Get Target Minute.
@return Minute of process start time */
public int GetScheduleMinute() 
{
Object ii = Get_Value("ScheduleMinute");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** ScheduleType AD_Reference_ID=318 */
public static int SCHEDULETYPE_AD_Reference_ID=318;
/** Frequency = F */
public static String SCHEDULETYPE_Frequency = "F";
/** Month Day = M */
public static String SCHEDULETYPE_MonthDay = "M";
/** Week Day = W */
public static String SCHEDULETYPE_WeekDay = "W";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsScheduleTypeValid (String test)
{
return test.Equals("F") || test.Equals("M") || test.Equals("W");
}
/** Set Schedule Type.
@param ScheduleType Type of schedule */
public void SetScheduleType (String ScheduleType)
{
if (ScheduleType == null) throw new ArgumentException ("ScheduleType is mandatory");
if (!IsScheduleTypeValid(ScheduleType))
throw new ArgumentException ("ScheduleType Invalid value - " + ScheduleType + " - Reference_ID=318 - F - M - W");
if (ScheduleType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ScheduleType = ScheduleType.Substring(0,1);
}
Set_Value ("ScheduleType", ScheduleType);
}
/** Get Schedule Type.
@return Type of schedule */
public String GetScheduleType() 
{
return (String)Get_Value("ScheduleType");
}

/** WeekDay AD_Reference_ID=167 */
public static int WEEKDAY_AD_Reference_ID=167;
/** Monday = 1 */
public static String WEEKDAY_Monday = "1";
/** Tuesday = 2 */
public static String WEEKDAY_Tuesday = "2";
/** Wednesday = 3 */
public static String WEEKDAY_Wednesday = "3";
/** Thursday = 4 */
public static String WEEKDAY_Thursday = "4";
/** Friday = 5 */
public static String WEEKDAY_Friday = "5";
/** Saturday = 6 */
public static String WEEKDAY_Saturday = "6";
/** Sunday = 7 */
public static String WEEKDAY_Sunday = "7";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsWeekDayValid (String test)
{
return test == null || test.Equals("1") || test.Equals("2") || test.Equals("3") || test.Equals("4") || test.Equals("5") || test.Equals("6") || test.Equals("7");
}
/** Set Day of the Week.
@param WeekDay Day of the Week */
public void SetWeekDay (String WeekDay)
{
if (!IsWeekDayValid(WeekDay))
throw new ArgumentException ("WeekDay Invalid value - " + WeekDay + " - Reference_ID=167 - 1 - 2 - 3 - 4 - 5 - 6 - 7");
if (WeekDay != null && WeekDay.Length > 1)
{
log.Warning("Length > 1 - truncated");
WeekDay = WeekDay.Substring(0,1);
}
Set_Value ("WeekDay", WeekDay);
}
/** Get Day of the Week.
@return Day of the Week */
public String GetWeekDay() 
{
return (String)Get_Value("WeekDay");
}
}

}
