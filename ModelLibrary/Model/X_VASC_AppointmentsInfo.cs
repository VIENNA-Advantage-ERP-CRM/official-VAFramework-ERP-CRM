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
/** Generated Model for VASC_AppointmentsInfo
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VASC_AppointmentsInfo : PO
{
public X_VASC_AppointmentsInfo (Context ctx, int VASC_AppointmentsInfo_ID, Trx trxName) : base (ctx, VASC_AppointmentsInfo_ID, trxName)
{
/** if (VASC_AppointmentsInfo_ID == 0)
{
SetEndDate (DateTime.Now);
SetStartDate (DateTime.Now);
SetVASC_AppointmentsInfo_ID (0);
}
 */
}
public X_VASC_AppointmentsInfo (Ctx ctx, int VASC_AppointmentsInfo_ID, Trx trxName) : base (ctx, VASC_AppointmentsInfo_ID, trxName)
{
/** if (VASC_AppointmentsInfo_ID == 0)
{
SetEndDate (DateTime.Now);
SetStartDate (DateTime.Now);
SetVASC_AppointmentsInfo_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VASC_AppointmentsInfo (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VASC_AppointmentsInfo (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VASC_AppointmentsInfo (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VASC_AppointmentsInfo()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27685227049069L;
/** Last Updated Timestamp 2014-06-18 7:58:52 PM */
public static long updatedMS = 1403101732280L;
/** AD_Table_ID=1000466 */
public static int Table_ID;
 // =1000466;

/** TableName=VASC_AppointmentsInfo */
public static String Table_Name="VASC_AppointmentsInfo";

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
StringBuilder sb = new StringBuilder ("X_VASC_AppointmentsInfo[").Append(Get_ID()).Append("]");
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
/** Set User/Contact.
@param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
public void SetAD_User_ID (int AD_User_ID)
{
if (AD_User_ID <= 0) Set_Value ("AD_User_ID", null);
else
Set_Value ("AD_User_ID", AD_User_ID);
}
/** Get User/Contact.
@return User within the system - Internal or Customer/Prospect Contact. */
public int GetAD_User_ID() 
{
Object ii = Get_Value("AD_User_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set AttendeeInfo.
@param AttendeeInfo AttendeeInfo */
public void SetAttendeeInfo (String AttendeeInfo)
{
if (AttendeeInfo != null && AttendeeInfo.Length > 3500)
{
log.Warning("Length > 3500 - truncated");
AttendeeInfo = AttendeeInfo.Substring(0,3500);
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
if (Description != null && Description.Length > 3500)
{
log.Warning("Length > 3500 - truncated");
Description = Description.Substring(0,3500);
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
if (EndDate == null) throw new ArgumentException ("EndDate is mandatory.");
Set_Value ("EndDate", (DateTime?)EndDate);
}
/** Get End Date.
@return Last effective date (inclusive) */
public DateTime? GetEndDate() 
{
return (DateTime?)Get_Value("EndDate");
}
/** Set EndTimeZone.
@param EndTimeZone EndTimeZone */
public void SetEndTimeZone (String EndTimeZone)
{
if (EndTimeZone != null && EndTimeZone.Length > 100)
{
log.Warning("Length > 100 - truncated");
EndTimeZone = EndTimeZone.Substring(0,100);
}
Set_Value ("EndTimeZone", EndTimeZone);
}
/** Get EndTimeZone.
@return EndTimeZone */
public String GetEndTimeZone() 
{
return (String)Get_Value("EndTimeZone");
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
/** Set Closed Status.
@param IsClosed The status is closed */
public void SetIsClosed (Boolean IsClosed)
{
Set_Value ("IsClosed", IsClosed);
}
/** Get Closed Status.
@return The status is closed */
public Boolean IsClosed() 
{
Object oo = Get_Value("IsClosed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Deleted.
@param IsDeleted Deleted */
public void SetIsDeleted (Boolean IsDeleted)
{
Set_Value ("IsDeleted", IsDeleted);
}
/** Get Deleted.
@return Deleted */
public Boolean IsDeleted() 
{
Object oo = Get_Value("IsDeleted");
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
/** Set RecurrenceException.
@param RecurrenceException RecurrenceException */
public void SetRecurrenceException (String RecurrenceException)
{
if (RecurrenceException != null && RecurrenceException.Length > 100)
{
log.Warning("Length > 100 - truncated");
RecurrenceException = RecurrenceException.Substring(0,100);
}
Set_Value ("RecurrenceException", RecurrenceException);
}
/** Get RecurrenceException.
@return RecurrenceException */
public String GetRecurrenceException() 
{
return (String)Get_Value("RecurrenceException");
}
/** Set RecurrenceID.
@param RecurrenceID RecurrenceID */
public void SetRecurrenceID (Decimal? RecurrenceID)
{
Set_Value ("RecurrenceID", (Decimal?)RecurrenceID);
}
/** Get RecurrenceID.
@return RecurrenceID */
public Decimal GetRecurrenceID() 
{
Object bd =Get_Value("RecurrenceID");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set RecurrenceRule.
@param RecurrenceRule RecurrenceRule */
public void SetRecurrenceRule (String RecurrenceRule)
{
if (RecurrenceRule != null && RecurrenceRule.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
RecurrenceRule = RecurrenceRule.Substring(0,2000);
}
Set_Value ("RecurrenceRule", RecurrenceRule);
}
/** Get RecurrenceRule.
@return RecurrenceRule */
public String GetRecurrenceRule() 
{
return (String)Get_Value("RecurrenceRule");
}
/** Set Start Date.
@param StartDate First effective day (inclusive) */
public void SetStartDate (DateTime? StartDate)
{
if (StartDate == null) throw new ArgumentException ("StartDate is mandatory.");
Set_Value ("StartDate", (DateTime?)StartDate);
}
/** Get Start Date.
@return First effective day (inclusive) */
public DateTime? GetStartDate() 
{
return (DateTime?)Get_Value("StartDate");
}
/** Set StartTimeZone.
@param StartTimeZone StartTimeZone */
public void SetStartTimeZone (String StartTimeZone)
{
if (StartTimeZone != null && StartTimeZone.Length > 100)
{
log.Warning("Length > 100 - truncated");
StartTimeZone = StartTimeZone.Substring(0,100);
}
Set_Value ("StartTimeZone", StartTimeZone);
}
/** Get StartTimeZone.
@return StartTimeZone */
public String GetStartTimeZone() 
{
return (String)Get_Value("StartTimeZone");
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
/** Set VASC_AppointmentsInfo_ID.
@param VASC_AppointmentsInfo_ID VASC_AppointmentsInfo_ID */
public void SetVASC_AppointmentsInfo_ID (int VASC_AppointmentsInfo_ID)
{
if (VASC_AppointmentsInfo_ID < 1) throw new ArgumentException ("VASC_AppointmentsInfo_ID is mandatory.");
Set_ValueNoCheck ("VASC_AppointmentsInfo_ID", VASC_AppointmentsInfo_ID);
}
/** Get VASC_AppointmentsInfo_ID.
@return VASC_AppointmentsInfo_ID */
public int GetVASC_AppointmentsInfo_ID() 
{
Object ii = Get_Value("VASC_AppointmentsInfo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
