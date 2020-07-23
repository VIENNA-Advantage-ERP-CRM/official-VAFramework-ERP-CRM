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
/** Generated Model for Appointments
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_Appointments : PO
{
public X_Appointments (Context ctx, int Appointments_ID, Trx trxName) : base (ctx, Appointments_ID, trxName)
{
/** if (Appointments_ID == 0)
{
SetAppointments_ID (0);
}
 */
}
public X_Appointments (Ctx ctx, int Appointments_ID, Trx trxName) : base (ctx, Appointments_ID, trxName)
{
/** if (Appointments_ID == 0)
{
SetAppointments_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Appointments (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Appointments (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Appointments (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_Appointments()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514367235L;
/** Last Updated Timestamp 7/29/2010 1:07:30 PM */
public static long updatedMS = 1280389050446L;
/** AD_Table_ID=1000000 */
public static int Table_ID;
 // =1000000;

/** TableName=Appointments */
public static String Table_Name="Appointments";

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
StringBuilder sb = new StringBuilder ("X_Appointments[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Business Partner Contact */
public void SetAD_User_ID (Decimal? AD_User_ID)
{
Set_Value ("AD_User_ID", (Decimal?)AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Business Partner Contact */
public Decimal GetAD_User_ID() 
{
Object bd =Get_Value("AD_User_ID");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set AllDay.
@param AllDay AllDay */
public void SetAllDay (Boolean AllDay)
{
Set_Value ("AllDay", AllDay);
}
/** Get AllDay.
@return AllDay */
public Boolean IsAllDay() 
{
Object oo = Get_Value("AllDay");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Appointments_ID.
@param Appointments_ID Appointments_ID */
public void SetAppointments_ID (int Appointments_ID)
{
if (Appointments_ID < 1) throw new ArgumentException ("Appointments_ID is mandatory.");
Set_ValueNoCheck ("Appointments_ID", Appointments_ID);
}
/** Get Appointments_ID.
@return Appointments_ID */
public int GetAppointments_ID() 
{
Object ii = Get_Value("Appointments_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set AttendeeInfo.
@param AttendeeInfo AttendeeInfo */
public void SetAttendeeInfo (String AttendeeInfo)
{
if (AttendeeInfo != null && AttendeeInfo.Length > 4000)
{
log.Warning("Length > 4000 - truncated");
AttendeeInfo = AttendeeInfo.Substring(0,4000);
}
Set_Value ("AttendeeInfo", AttendeeInfo);
}
/** Get AttendeeInfo.
@return AttendeeInfo */
public String GetAttendeeInfo() 
{
return (String)Get_Value("AttendeeInfo");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 4000)
{
log.Warning("Length > 4000 - truncated");
Description = Description.Substring(0,4000);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set End Date.
@param EndDate Last effective date (inclusive) */
public void SetEndDate (DateTime? EndDate)
{
Set_ValueNoCheck ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
}
/** Set IsCallAttendee.
@param IsCallAttendee IsCallAttendee */
public void SetIsCallAttendee (Boolean IsCallAttendee)
{
Set_Value ("IsCallAttendee", IsCallAttendee);
}
/** Get IsCallAttendee.
@return IsCallAttendee */
public Boolean IsCallAttendee() 
{
Object oo = Get_Value("IsCallAttendee");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsPrivate.
@param IsPrivate IsPrivate */
public void SetIsPrivate (Boolean IsPrivate)
{
Set_Value ("IsPrivate", IsPrivate);
}
/** Get IsPrivate.
@return IsPrivate */
public Boolean IsPrivate() 
{
Object oo = Get_Value("IsPrivate");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set IsRead.
@param IsRead IsRead */
public void SetIsRead (Boolean IsRead)
{
Set_Value ("IsRead", IsRead);
}
/** Get IsRead.
@return IsRead */
public Boolean IsRead() 
{
Object oo = Get_Value("IsRead");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Label.
@param Label Label */
public void SetLabel (Decimal? Label)
{
Set_Value ("Label", (Decimal?)Label);
}
/** Get Label.
@return Label */
public Decimal GetLabel() 
{
Object bd =Get_Value("Label");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Location.
@param Location Location */
public void SetLocation (String Location)
{
if (Location != null && Location.Length > 60)
{
log.Warning("Length > 60 - truncated");
Location = Location.Substring(0,60);
}
Set_Value ("Location", Location);
}
/** Get Location.
@return Location */
public String GetLocation() 
{
return (String)Get_Value("Location");
}
/** Set RecurrenceInfo.
@param RecurrenceInfo RecurrenceInfo */
public void SetRecurrenceInfo (String RecurrenceInfo)
{
if (RecurrenceInfo != null && RecurrenceInfo.Length > 4000)
{
log.Warning("Length > 4000 - truncated");
RecurrenceInfo = RecurrenceInfo.Substring(0,4000);
}
Set_Value ("RecurrenceInfo", RecurrenceInfo);
}
/** Get RecurrenceInfo.
@return RecurrenceInfo */
public String GetRecurrenceInfo() 
{
return (String)Get_Value("RecurrenceInfo");
}
/** Set ReminderInfo.
@param ReminderInfo ReminderInfo */
public void SetReminderInfo (String ReminderInfo)
{
if (ReminderInfo != null && ReminderInfo.Length > 4000)
{
log.Warning("Length > 4000 - truncated");
ReminderInfo = ReminderInfo.Substring(0,4000);
}
Set_Value ("ReminderInfo", ReminderInfo);
}
/** Get ReminderInfo.
@return ReminderInfo */
public String GetReminderInfo() 
{
return (String)Get_Value("ReminderInfo");
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
Set_ValueNoCheck ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
/** Set Status.
@param Status Status of the currently running check */
public void SetStatus (Decimal? Status)
{
Set_Value ("Status", (Decimal?)Status);
}
/** Get Status.
@return Status of the currently running check */
public Decimal GetStatus() 
{
Object bd =Get_Value("Status");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Subject.
@param Subject Email Message Subject */
public void SetSubject (String Subject)
{
if (Subject != null && Subject.Length > 60)
{
log.Warning("Length > 60 - truncated");
Subject = Subject.Substring(0,60);
}
Set_Value ("Subject", Subject);
}
/** Get Subject.
@return Email Message Subject */
public String GetSubject() 
{
return (String)Get_Value("Subject");
}
/** Set Type.
@param TypeApp Type */
public void SetTypeApp (Decimal? TypeApp)
{
Set_Value ("TypeApp", (Decimal?)TypeApp);
}
/** Get Type.
@return Type */
public Decimal GetTypeApp() 
{
Object bd =Get_Value("TypeApp");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
