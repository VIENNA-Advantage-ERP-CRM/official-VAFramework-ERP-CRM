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
    /** Generated Model for AppointmentsInfo
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AppointmentsInfo : PO
    {
        public X_AppointmentsInfo(Context ctx, int AppointmentsInfo_ID, Trx trxName)
            : base(ctx, AppointmentsInfo_ID, trxName)
        {
            /** if (AppointmentsInfo_ID == 0)
            {
            SetAppointmentsInfo_ID (0);
            SetSubject (null);
            }
             */
        }
        public X_AppointmentsInfo(Ctx ctx, int AppointmentsInfo_ID, Trx trxName)
            : base(ctx, AppointmentsInfo_ID, trxName)
        {
            /** if (AppointmentsInfo_ID == 0)
            {
            SetAppointmentsInfo_ID (0);
            SetSubject (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AppointmentsInfo(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AppointmentsInfo(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_AppointmentsInfo(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_AppointmentsInfo()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27629734074384L;
        /** Last Updated Timestamp 9/14/2012 1:15:57 PM */
        public static long updatedMS = 1347608757595L;
        /** AD_Table_ID=1000001 */
        public static int Table_ID;
        // =1000001;

        /** TableName=AppointmentsInfo */
        public static String Table_Name = "AppointmentsInfo";

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
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_AppointmentsInfo[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Table.
        @param AD_Table_ID Database Table information */
        public void SetAD_Table_ID(Decimal? AD_Table_ID)
        {
            Set_Value("AD_Table_ID", (Decimal?)AD_Table_ID);
        }
        /** Get Table.
        @return Database Table information */
        public Decimal GetAD_Table_ID()
        {
            Object bd = Get_Value("AD_Table_ID");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Customer/Prospect Contact. */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
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
        public void SetAllDay(Boolean AllDay)
        {
            Set_Value("AllDay", AllDay);
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
        /** Set AppointmentCategory_ID.
        @param AppointmentCategory_ID AppointmentCategory_ID */
        public void SetAppointmentCategory_ID(int AppointmentCategory_ID)
        {
            if (AppointmentCategory_ID <= 0) Set_Value("AppointmentCategory_ID", null);
            else
                Set_Value("AppointmentCategory_ID", AppointmentCategory_ID);
        }
        /** Get AppointmentCategory_ID.
        @return AppointmentCategory_ID */
        public int GetAppointmentCategory_ID()
        {
            Object ii = Get_Value("AppointmentCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AppointmentsInfo_ID.
        @param AppointmentsInfo_ID AppointmentsInfo_ID */
        public void SetAppointmentsInfo_ID(int AppointmentsInfo_ID)
        {
            if (AppointmentsInfo_ID < 1) throw new ArgumentException("AppointmentsInfo_ID is mandatory.");
            Set_ValueNoCheck("AppointmentsInfo_ID", AppointmentsInfo_ID);
        }
        /** Get AppointmentsInfo_ID.
        @return AppointmentsInfo_ID */
        public int GetAppointmentsInfo_ID()
        {
            Object ii = Get_Value("AppointmentsInfo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set AttendeeInfo.
        @param AttendeeInfo AttendeeInfo */
        public void SetAttendeeInfo(String AttendeeInfo)
        {
            if (AttendeeInfo != null && AttendeeInfo.Length > 3500)
            {
                log.Warning("Length > 3500 - truncated");
                AttendeeInfo = AttendeeInfo.Substring(0, 3500);
            }
            Set_Value("AttendeeInfo", AttendeeInfo);
        }
        /** Get AttendeeInfo.
        @return AttendeeInfo */
        public String GetAttendeeInfo()
        {
            return (String)Get_Value("AttendeeInfo");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 4000)
            {
                log.Warning("Length > 4000 - truncated");
                Description = Description.Substring(0, 4000);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set EmailToInfo.
        @param EmailToInfo EmailToInfo */
        public void SetEmailToInfo(String EmailToInfo)
        {
            if (EmailToInfo != null && EmailToInfo.Length > 3500)
            {
                log.Warning("Length > 3500 - truncated");
                EmailToInfo = EmailToInfo.Substring(0, 3500);
            }
            Set_Value("EmailToInfo", EmailToInfo);
        }
        /** Get EmailToInfo.
        @return EmailToInfo */
        public String GetEmailToInfo()
        {
            return (String)Get_Value("EmailToInfo");
        }
        /** Set End Date.
        @param EndDate Last effective date (inclusive) */
        public void SetEndDate(DateTime? EndDate)
        {
            Set_ValueNoCheck("EndDate", (DateTime?)EndDate);
        }
        /** Get End Date.
        @return Last effective date (inclusive) */
        public DateTime? GetEndDate()
        {
            return (DateTime?)Get_Value("EndDate");
        }
        /** Set IsCallAttendee.
        @param IsCallAttendee IsCallAttendee */
        public void SetIsCallAttendee(Boolean IsCallAttendee)
        {
            Set_Value("IsCallAttendee", IsCallAttendee);
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
        public void SetIsClosed(Boolean IsClosed)
        {
            Set_Value("IsClosed", IsClosed);
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
        /** Set IsPrivate.
        @param IsPrivate IsPrivate */
        public void SetIsPrivate(Boolean IsPrivate)
        {
            Set_Value("IsPrivate", IsPrivate);
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
        public void SetIsRead(Boolean IsRead)
        {
            Set_Value("IsRead", IsRead);
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
        /** Set IsTask.
        @param IsTask IsTask */
        public void SetIsTask(Boolean IsTask)
        {
            Set_Value("IsTask", IsTask);
        }
        /** Get IsTask.
        @return IsTask */
        public Boolean IsTask()
        {
            Object oo = Get_Value("IsTask");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Label.
        @param Label Label */
        public void SetLabel(Decimal? Label)
        {
            Set_Value("Label", (Decimal?)Label);
        }
        /** Get Label.
        @return Label */
        public Decimal GetLabel()
        {
            Object bd = Get_Value("Label");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Last Gmail Updated.
        @param LastGmailUpdated Last Gmail Updated */
        public void SetLastGmailUpdated(DateTime? LastGmailUpdated)
        {
            Set_Value("LastGmailUpdated", (DateTime?)LastGmailUpdated);
        }
        /** Get Last Gmail Updated.
        @return Last Gmail Updated */
        public DateTime? GetLastGmailUpdated()
        {
            return (DateTime?)Get_Value("LastGmailUpdated");
        }
        /** Set Last Local Updated.
        @param LastLocalUpdated Last Local Updated */
        public void SetLastLocalUpdated(DateTime? LastLocalUpdated)
        {
            Set_Value("LastLocalUpdated", (DateTime?)LastLocalUpdated);
        }
        /** Get Last Local Updated.
        @return Last Local Updated */
        public DateTime? GetLastLocalUpdated()
        {
            return (DateTime?)Get_Value("LastLocalUpdated");
        }
        /** Set Location.
        @param Location Location */
        public void SetLocation(String Location)
        {
            if (Location != null && Location.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Location = Location.Substring(0, 60);
            }
            Set_Value("Location", Location);
        }
        /** Get Location.
        @return Location */
        public String GetLocation()
        {
            return (String)Get_Value("Location");
        }

        /** Set PriorityKey.
      @param PriorityKey PriorityKey of the document */
        public void SetPriorityKey(Decimal? PriorityKey)
        {
            Set_Value("PriorityKey", (Decimal?)PriorityKey);
        }
        /** Get PriorityKey.
        @return PriorityKey of the document */
        public Decimal GetPriorityKey()
        {
            Object bd = Get_Value("PriorityKey");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Priority.
        @param Priority Indicates if this request is of a high, medium or low priority. */
        public void SetPriority(String Priority)
        {
            if (Priority != null && Priority.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Priority = Priority.Substring(0, 50);
            }
            Set_Value("Priority", Priority);
        }
        /** Get Priority.
        @return Indicates if this request is of a high, medium or low priority. */
        public String GetPriority()
        {
            return (String)Get_Value("Priority");
        }
        /** Set Properties.
        @param Properties Properties */
        public void SetProperties(String Properties)
        {
            if (Properties != null && Properties.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Properties = Properties.Substring(0, 2000);
            }
            Set_Value("Properties", Properties);
        }
        /** Get Properties.
        @return Properties */
        public String GetProperties()
        {
            return (String)Get_Value("Properties");
        }
        /** Set Record ID.
        @param Record_ID Direct internal record ID */
        public void SetRecord_ID(int Record_ID)
        {
            if (Record_ID <= 0) Set_Value("Record_ID", null);
            else
                Set_Value("Record_ID", Record_ID);
        }
        /** Get Record ID.
        @return Direct internal record ID */
        public int GetRecord_ID()
        {
            Object ii = Get_Value("Record_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set RecurrenceInfo.
        @param RecurrenceInfo RecurrenceInfo */
        public void SetRecurrenceInfo(String RecurrenceInfo)
        {
            if (RecurrenceInfo != null && RecurrenceInfo.Length > 3500)
            {
                log.Warning("Length > 3500 - truncated");
                RecurrenceInfo = RecurrenceInfo.Substring(0, 3500);
            }
            Set_Value("RecurrenceInfo", RecurrenceInfo);
        }
        /** Get RecurrenceInfo.
        @return RecurrenceInfo */
        public String GetRecurrenceInfo()
        {
            return (String)Get_Value("RecurrenceInfo");
        }
        /** Set ReminderInfo.
        @param ReminderInfo ReminderInfo */
        public void SetReminderInfo(String ReminderInfo)
        {
            if (ReminderInfo != null && ReminderInfo.Length > 3500)
            {
                log.Warning("Length > 3500 - truncated");
                ReminderInfo = ReminderInfo.Substring(0, 3500);
            }
            Set_Value("ReminderInfo", ReminderInfo);
        }
        /** Get ReminderInfo.
        @return ReminderInfo */
        public String GetReminderInfo()
        {
            return (String)Get_Value("ReminderInfo");
        }
        /** Set Result.
        @param Result Result of the action taken */
        public void SetResult(String Result)
        {
            if (Result != null && Result.Length > 3500)
            {
                log.Warning("Length > 3500 - truncated");
                Result = Result.Substring(0, 3500);
            }
            Set_Value("Result", Result);
        }
        /** Get Result.
        @return Result of the action taken */
        public String GetResult()
        {
            return (String)Get_Value("Result");
        }
        /** Set Start Date.
        @param StartDate First effective day (inclusive) */
        public void SetStartDate(DateTime? StartDate)
        {
            Set_ValueNoCheck("StartDate", (DateTime?)StartDate);
        }
        /** Get Start Date.
        @return First effective day (inclusive) */
        public DateTime? GetStartDate()
        {
            return (DateTime?)Get_Value("StartDate");
        }

        /** Set Recurrence End Date
       @param RecurrenceEndDate First effective day (inclusive) */
        public void SetRecurrenceEndDate(DateTime? RecurrenceEndDate)
        {
            Set_ValueNoCheck("RecurrenceEndDate", (DateTime?)RecurrenceEndDate);
        }
        /** Get Recurrence End Date.
        @return First effective day (inclusive) */
        public DateTime? GetRecurrenceEndDate()
        {
            return (DateTime?)Get_Value("RecurrenceEndDate");
        }

        /** Set Status.
        @param Status Status of the document */
        public void SetStatus(Decimal? Status)
        {
            Set_Value("Status", (Decimal?)Status);
        }
        /** Get Status.
        @return Status of the document */
        public Decimal GetStatus()
        {
            Object bd = Get_Value("Status");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Subject.
        @param Subject Email Message Subject */
        public void SetSubject(String Subject)
        {
            if (Subject == null) throw new ArgumentException("Subject is mandatory.");
            if (Subject.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Subject = Subject.Substring(0, 100);
            }
            Set_Value("Subject", Subject);
        }
        /** Get Subject.
        @return Email Message Subject */
        public String GetSubject()
        {
            return (String)Get_Value("Subject");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetSubject());
        }
        /** Set Task Status.
        @param TaskStatus Status of the Task */
        public void SetTaskStatus(Decimal? TaskStatus)
        {
            Set_Value("TaskStatus", (Decimal?)TaskStatus);
        }
        /** Get Task Status.
        @return Status of the Task */
        public Decimal GetTaskStatus()
        {
            Object bd = Get_Value("TaskStatus");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Code Type.
        @param Type Type of Code/Validation (SQL, Java Script, Java Language) */
        public void SetType(Decimal? Type)
        {
            Set_Value("Type", (Decimal?)Type);
        }
        /** Get Code Type.
        @return Type of Code/Validation (SQL, Java Script, Java Language) */
        public new Decimal GetType()
        {
            Object bd = Get_Value("Type");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Unique Task ID.
        @param UTaskID Unique Task ID */
        public void SetUTaskID(String UTaskID)
        {
            if (UTaskID != null && UTaskID.Length > 200)
            {
                log.Warning("Length > 200 - truncated");
                UTaskID = UTaskID.Substring(0, 200);
            }
            Set_Value("UTaskID", UTaskID);
        }
        /** Get Unique Task ID.
        @return Unique Task ID */
        public String GetUTaskID()
        {
            return (String)Get_Value("UTaskID");
        }

        /** Set EndTimeZone.
       @param EndTimeZone EndTimeZone */
        public void SetEndTimeZone(String EndTimeZone)
        {
            if (EndTimeZone != null && EndTimeZone.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                EndTimeZone = EndTimeZone.Substring(0, 100);
            }
            Set_Value("EndTimeZone", EndTimeZone);
        }
        /** Get EndTimeZone.
        @return EndTimeZone */
        public String GetEndTimeZone()
        {
            return (String)Get_Value("EndTimeZone");
        }

        /** Set RecurrenceException.
        @param RecurrenceException RecurrenceException */
        public void SetRecurrenceException(String RecurrenceException)
        {
            if (RecurrenceException != null && RecurrenceException.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                RecurrenceException = RecurrenceException.Substring(0, 100);
            }
            Set_Value("RecurrenceException", RecurrenceException);
        }
        /** Get RecurrenceException.
        @return RecurrenceException */
        public String GetRecurrenceException()
        {
            return (String)Get_Value("RecurrenceException");
        }
        /** Set RecurrenceID.
        @param RecurrenceID RecurrenceID */
        public void SetRecurrenceID(Decimal? RecurrenceID)
        {
            Set_Value("RecurrenceID", (Decimal?)RecurrenceID);
        }
        /** Get RecurrenceID.
        @return RecurrenceID */
        public Decimal GetRecurrenceID()
        {
            Object bd = Get_Value("RecurrenceID");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set RecurrenceRule.
        @param RecurrenceRule RecurrenceRule */
        public void SetRecurrenceRule(String RecurrenceRule)
        {
            if (RecurrenceRule != null && RecurrenceRule.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                RecurrenceRule = RecurrenceRule.Substring(0, 2000);
            }
            Set_Value("RecurrenceRule", RecurrenceRule);
        }
        /** Get RecurrenceRule.
        @return RecurrenceRule */
        public String GetRecurrenceRule()
        {
            return (String)Get_Value("RecurrenceRule");
        }


        /** Set StartTimeZone.
         @param StartTimeZone StartTimeZone */
        public void SetStartTimeZone(String StartTimeZone)
        {
            if (StartTimeZone != null && StartTimeZone.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                StartTimeZone = StartTimeZone.Substring(0, 100);
            }
            Set_Value("StartTimeZone", StartTimeZone);
        }
        /** Get StartTimeZone.
        @return StartTimeZone */
        public String GetStartTimeZone()
        {
            return (String)Get_Value("StartTimeZone");
        }

        /** Set RefAppointmentsInfo_ID.
        @param RefAppointmentsInfo_ID RefAppointmentsInfo_ID */
        public void SetRefAppointmentsInfo_ID(Decimal? RefAppointmentsInfo_ID)
        {
            Set_Value("RefAppointmentsInfo_ID", (Decimal?)RefAppointmentsInfo_ID);
        }
        /** Get RefAppointmentsInfo_ID.
        @return RefAppointmentsInfo_ID */
        public Decimal GetRefAppointmentsInfo_ID()
        {
            Object bd = Get_Value("RefAppointmentsInfo_ID");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

    }

}
