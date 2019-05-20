/********************************************************
 * Module Name    : Schedule
 * Purpose        : Schedule Modelo
 * Author         : Jagmohan Bhatt
 * Date           : 16-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Common;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Process;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Logging;
using System.Net;

namespace VAdvantage.Model
{
    /// <summary>
    /// Schedule  Model
    /// </summary>
    public class MSchedule : X_AD_Schedule
    {
        /// <summary>
        /// Get Schedule from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Schedule_ID">id</param>
        /// <returns>MSchedule</returns>
        public static MSchedule Get(Ctx ctx, int AD_Schedule_ID)
        {
            int key = AD_Schedule_ID;
            MSchedule retValue = null;

            if(s_cache.ContainsKey(key))
                retValue = (MSchedule)s_cache[key];

            if (retValue != null)
                return retValue;
            retValue = new MSchedule(ctx, AD_Schedule_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache[key] = retValue;
            return retValue;
        }

        /**	Cache						*/
        private static CCache<int, MSchedule> s_cache = new CCache<int, MSchedule>("AD_Schedule", 20);

       // int s = 0;
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Schedule_ID">id</param>
        /// <param name="trxName">optional transaction</param>
        public MSchedule(Ctx ctx, int AD_Schedule_ID, Trx trxName)
            : base(ctx, AD_Schedule_ID, trxName)
        {

            if (AD_Schedule_ID == 0)
            {
                //	setName (null);
                SetScheduleType(SCHEDULETYPE_Frequency);
                SetFrequencyType(FREQUENCYTYPE_Day);
                SetFrequency(1);
                //
                SetOnMonday(true);	// Y
                SetOnTuesday(true);	// Y
                SetOnWednesday(true);	// Y
                SetOnThursday(true);	// Y
                SetOnFriday(true);	// Y
                SetOnSaturday(false);	// N
                SetOnSunday(false);	// N
                //
                SetRunOnlySpecifiedTime(false);	// N
            }
        }	//	MSchedule

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">optional transaction</param>
        public MSchedule(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MSchedule

        /// <summary>
        /// Get Month Day
        /// </summary>
        /// <returns>1 .. 31</returns>
        public new int GetMonthDay()
        {
            int day = base.GetMonthDay();
            if (day < 1)
                day = 1;
            else if (day > 31)
                day = 31;
            return day;
        }	//	getMonthDay


        /// <summary>
        /// Get Weekday
        /// </summary>
        /// <returns>Weekday</returns>
        public new String GetWeekDay()
        {
            String wd = base.GetWeekDay();
            if (wd == null || IsWeekDayValid(wd))
                wd = WEEKDAY_Monday;
            return wd;
        }	//	getWeekDay


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetScheduleType() == null)
                SetScheduleType(SCHEDULETYPE_Frequency);

            //	Set Schedule Type & Frequencies
            if (SCHEDULETYPE_Frequency.Equals(GetScheduleType()))
            {
                if (GetFrequencyType() == null)
                    SetFrequencyType(FREQUENCYTYPE_Day);
                if (GetFrequency() < 1)
                    SetFrequency(1);
            }
            else if (SCHEDULETYPE_MonthDay.Equals(GetScheduleType()))
            {
                if (base.GetMonthDay() < 1 || base.GetMonthDay() > 31)
                    SetMonthDay(1);
            }
            else	//	SCHEDULETYPE_WeekDay
            {
                if (GetScheduleType() == null)
                    SetScheduleType(SCHEDULETYPE_WeekDay);
                if (base.GetWeekDay() == null)
                    SetWeekDay(WEEKDAY_Monday);
            }
            //	Hour/Minute
            if (GetScheduleHour() > 23 || GetScheduleHour() < 0)
                SetScheduleHour(0);
            if (GetScheduleMinute() > 59 || GetScheduleMinute() < 0)
                SetScheduleMinute(0);
            return true;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>Info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MSchedule[");
            sb.Append(Get_ID()).Append("-").Append(GetName());
            String scheduleType = GetScheduleType();
            sb.Append(",Type=").Append(scheduleType);
            if (SCHEDULETYPE_Frequency.Equals(scheduleType))
            {
                sb.Append(",Frequency=").Append(GetFrequencyType())
                    .Append("*").Append(GetFrequency());
                if (IsOnMonday())
                    sb.Append(",Mo");
                if (IsOnTuesday())
                    sb.Append(",Tu");
                if (IsOnWednesday())
                    sb.Append(",We");
                if (IsOnThursday())
                    sb.Append(",Th");
                if (IsOnFriday())
                    sb.Append(",Fr");
                if (IsOnSaturday())
                    sb.Append(",Sa");
                if (IsOnSunday())
                    sb.Append(",Su");
            }
            else if (SCHEDULETYPE_MonthDay.Equals(scheduleType))
                sb.Append(",Day=").Append(GetMonthDay());
            else
                sb.Append(",Day=").Append(GetWeekDay());
            //
            sb.Append(",HH=").Append(GetScheduleHour())
                .Append(",MM=").Append(GetScheduleMinute());
            //
            sb.Append("]");
            return sb.ToString();
        }	//	toString


        public const int MONDAY = 2;

        public const int SUNDAY = 1;
        public const int THURSDAY = 5;
        public const int TUESDAY = 3;
        public const int WEDNESDAY = 4;
        public const int SATURDAY = 7;
        public const int FRIDAY = 6;


        /// <summary>
        /// Is it OK to Run process On IP of this box
        /// </summary>
        /// <returns></returns>
        public bool IsOKtoRunOnIP()
        {
            String ipOnly = GetRunOnlyOnIP();
            if (ipOnly == null || ipOnly.Length == 0)
                return true;

            StringTokenizer st = new StringTokenizer(ipOnly, ";");
            while (st.HasMoreElements())
            {
                String ip = st.NextToken();
                if (CheckIP(ip))
                    return true;
            }
            return false;
        }	//	isOKtoRunOnIP



        /// <summary>
        /// Check whether this IP is allowed to process
        /// </summary>
        /// <param name="ipOnly">IP Address</param>
        /// <returns>true if IP is correct</returns>
        private bool CheckIP(String ipOnly)
        {
            try
            {
                // Getting Ip address of local machine...
                // First get the host name of local machine.
                string strHostName = Dns.GetHostName();
                // Then using host name, get the IP address list..
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                IPAddress[] addr = ipEntry.AddressList;

                String ip = "";
                for (int i = 0; i < addr.Length; i++)
                {
                    ip = addr[i].ToString();
                }
                if (ipOnly.IndexOf(ip) == -1)
                {
                    // TODO: We need to handle this better, for the moment reduced to fine.
                    log.Fine("Not allowed here - IP=" + ip + " does not match " + ipOnly);
                    return false;
                }
                log.Fine("Allowed here - IP=" + ip + " matches " + ipOnly);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "", e);
                return false;
            }
            return true;
        }	// checkIP


        const long TOTAL_TICKS = 621355968000000000L;
        /// <summary>
        /// Get Next Run
        /// </summary>
        /// <param name="last">in MS</param>
        /// <returns>next run in MS</returns>
        public long GetNextRunMS(long last)
        {
            DateTime calNow = CommonFunctions.CovertMilliToDate(last);

            DateTime calNext = CommonFunctions.CovertMilliToDate(last);

            calNext = calNext.Subtract(new TimeSpan(0, 0, 0, calNext.Second, calNext.Millisecond));
            calNext = calNext.AddSeconds(0);
            calNext = calNext.AddMilliseconds(0);

            //
            int hour = GetScheduleHour();
            int minute = GetScheduleMinute();
            //
            String scheduleType = GetScheduleType();
            if (SCHEDULETYPE_Frequency.Equals(scheduleType))
            {
                String frequencyType = GetFrequencyType();
                int frequency = GetFrequency();

                List<int> validDays = new List<int>();
                if (IsOnMonday())
                    validDays.Add(MONDAY);
                if (IsOnTuesday())
                    validDays.Add(TUESDAY);
                if (IsOnWednesday())
                    validDays.Add(WEDNESDAY);
                if (IsOnThursday())
                    validDays.Add(THURSDAY);
                if (IsOnFriday())
                    validDays.Add(FRIDAY);
                if (IsOnSaturday())
                    validDays.Add(SATURDAY);
                if (IsOnSunday())
                    validDays.Add(SUNDAY);

                if (validDays.Count() <= 0)
                {
                    log.Warning("Incorrect Schedule setup. Please enable at least one of the weekdays");
                    validDays.Add((int)DayOfWeek.Monday);
                }

                bool increment = true;
                int ct = 0;
                while ((ct < 8) && !(validDays.Contains(((int)calNext.DayOfWeek) + 1)))
                {
                    calNext = calNext.AddDays(1);
                    ct++;
                    increment = false;
                }


                /*****	DAY		******/
                if (X_R_RequestProcessor.FREQUENCYTYPE_Day.Equals(frequencyType))
                {
                    //calNext.set(java.util.Calendar.HOUR_OF_DAY, hour);
                    //calNext.set(java.util.Calendar.MINUTE, minute);
                    //calNext.add(java.util.Calendar.DAY_OF_YEAR, frequency);

                    calNext = calNext.Subtract(new TimeSpan(calNext.Hour, 0, 0));
                    calNext = calNext.AddHours(hour);

                    calNext = calNext.Subtract(new TimeSpan(0, calNext.Minute, 0));
                    calNext = calNext.AddMinutes(minute);
                    if (increment)
                    {
                        calNext = calNext.AddDays(frequency);
                    }
                }	//	Day

                /*****	HOUR	******/
                else if (X_R_RequestProcessor.FREQUENCYTYPE_Hour.Equals(frequencyType))
                {
                    //calNext.set(java.util.Calendar.MINUTE, minute);
                    //calNext.add(java.util.Calendar.HOUR_OF_DAY, frequency);

                    calNext = calNext.Subtract(new TimeSpan(0, calNext.Minute, 0));
                    calNext = calNext.AddMinutes(minute);
                    if(increment)
                        calNext = calNext.AddHours(frequency);
                }	//	Hour

                /*****	MINUTE	******/
                else if (X_R_RequestProcessor.FREQUENCYTYPE_Minute.Equals(frequencyType))
                {
                    //calNext.add(java.util.Calendar.MINUTE, frequency);
                    if(increment)
                        calNext = calNext.AddMinutes(frequency);
                }	//	Minute
            }

            /*****	MONTH	******/
            else if (SCHEDULETYPE_MonthDay.Equals(scheduleType))
            {
                //calNext.set(java.util.Calendar.HOUR, hour);
                //calNext.set(java.util.Calendar.MINUTE, minute);
                calNext = calNext.Subtract(new TimeSpan(calNext.Hour, calNext.Minute, 0));
                calNext = calNext.AddHours(hour);
                calNext = calNext.AddMinutes(minute);
                
                //
                int day = GetMonthDay();
                int dd = calNext.Day; //.get(java.util.Calendar.DAY_OF_MONTH);
                int max = DateTime.DaysInMonth(calNext.Year, calNext.Month); //calNext.t .getActualMaximum(java.util.Calendar.DAY_OF_MONTH);
                int dayUsed = Math.Min(day, max);
                //	Same Month
                if (dd < dayUsed)
                {
                    //calNext.set(java.util.Calendar.DAY_OF_MONTH, dayUsed);
                    calNext = calNext.Subtract(new TimeSpan(calNext.Day, 0, 0, 0));
                    calNext = calNext.AddDays(dayUsed);
                }
                else
                {
                    if (calNext.Month == 12)
                    {
                        //calNext.AddYears(1);
                        calNext = calNext.AddMonths(1);
                    }
                    else
                    {
                        calNext = calNext.AddMonths(1);
                    }

                    //if (calNext.get(java.util.Calendar.MONTH) == java.util.Calendar.DECEMBER)
                    //{
                    //    calNext.add(java.util.Calendar.YEAR, 1);
                    //    calNext.set(java.util.Calendar.MONTH, java.util.Calendar.JANUARY);
                    //}
                    //else
                    //    calNext.add(java.util.Calendar.MONTH, 1);
                    max = 31;
                    dayUsed = Math.Min(day, max);
                    //calNext.set(java.util.Calendar.DAY_OF_MONTH, dayUsed);
                    calNext = calNext.Subtract(new TimeSpan(calNext.Day, 0, 0, 0, 0));
                    calNext = calNext.AddDays(dayUsed);
                }
            }	//	month

            /*****	WEEK	******/
            else // if (SCHEDULETYPE_WeekDay.Equals(scheduleType))
            {
                String weekDay = GetWeekDay();
                int dayOfWeek = 0;
                if (WEEKDAY_Monday.Equals(weekDay))
                    dayOfWeek = MONDAY;
                else if (WEEKDAY_Tuesday.Equals(weekDay))
                    dayOfWeek = TUESDAY;
                else if (WEEKDAY_Wednesday.Equals(weekDay))
                    dayOfWeek = WEDNESDAY;
                else if (WEEKDAY_Thursday.Equals(weekDay))
                    dayOfWeek = THURSDAY;
                else if (WEEKDAY_Friday.Equals(weekDay))
                    dayOfWeek = FRIDAY;
                else if (WEEKDAY_Saturday.Equals(weekDay))
                    dayOfWeek = SATURDAY;
                else if (WEEKDAY_Sunday.Equals(weekDay))
                    dayOfWeek = SUNDAY;

                calNext = calNext.Subtract(new TimeSpan(((int)calNext.DayOfWeek)+ 1, calNext.Hour, calNext.Minute, calNext.Second, calNext.Millisecond));
                calNext = calNext.AddDays(dayOfWeek);
                calNext = calNext.AddHours(hour);
                calNext = calNext.AddMinutes(minute);
                calNext = calNext.AddSeconds(0);
                calNext = calNext.AddMilliseconds(0);

                //calNext.set(java.util.Calendar.DAY_OF_WEEK, dayOfWeek);
                //calNext.set(java.util.Calendar.HOUR, hour);
                //calNext.set(java.util.Calendar.MINUTE, minute);
                //calNext.set(java.util.Calendar.SECOND, 0);
                //calNext.set(java.util.Calendar.MILLISECOND, 0);
                //
                //if (calNext > calNow)
                {
                    calNext = calNext.AddDays(7);
                }
            }	//	week

            
            long delta = CommonFunctions.CurrentTimeMillis(calNext) - CommonFunctions.CurrentTimeMillis(calNow);
            //long delta = calNext.getTimeInMillis() - calNow.getTimeInMillis();
            String info = "Now=" + calNow.ToString()
                + ", Next=" + calNext.ToString()
                + ", Delta=" + delta
                + " " + ToString();
            if (delta < 0)
            {
                log.Warning(info);
            }
            else
                log.Info(info);

            return CommonFunctions.CurrentTimeMillis(calNext);
        }	//	getNextRunMS


        /// <summary>
        /// Get Next
        /// </summary>
        /// <param name="start">start time</param>
        /// <param name="iterations">number of iteration</param>
        /// <returns>Array of next</returns>
        public DateTime[] GetNext(DateTime start, int iterations)
        {
            DateTime[] nexts = new DateTime[iterations];
            long startMS = CommonFunctions.CurrentTimeMillis(start);
            for (int i = 0; i < nexts.Length; i++)
            {
                //DateTime next = new DateTime()/10000);
                DateTime next = CommonFunctions.CovertMilliToDate(GetNextRunMS(startMS));
                startMS = CommonFunctions.CurrentTimeMillis(next);
                nexts[i] = next;
            }
            return nexts;
        }	//	getNext


        /// <summary>
        /// Only for test purpose
        /// </summary>
        public static void TestMain()
        {
            VAdvantage.DataBase.Ini.StartUp(true, true);
            VLogMgt.SetLevel(Level.FINE);
            MSchedule s = null;
            DateTime start = TimeUtil.GetDay(DateTime.Parse("11/26/2009 05:01:00 PM"));

            /**	Test Case - Days    	**/
            //s = new MSchedule(Env.GetContext(), 1000103, null);
            //PO.log.Info("*** Day 2 ***");
            //s.SetScheduleType(SCHEDULETYPE_Frequency);
            //s.SetFrequencyType(FREQUENCYTYPE_Day);
            //s.SetFrequency(2);
            //////	start = new Timestamp(System.currentTimeMillis());
            //s.GetNext(start, 10);

            ///**	Test Case - Weekdays 	**/
            //s = new MSchedule(Env.GetContext(), 0, null);
            //PO.log.Info("*** WeekDay Mo ***");
            //s.SetScheduleType(SCHEDULETYPE_WeekDay);
            //s.SetWeekDay(WEEKDAY_Monday);
            ////	start = new Timestamp(System.currentTimeMillis());
            //s.GetNext(start, 92);


            /**	Test Case - Hour 	**/
            //s = new MSchedule(Env.GetContext(), 0, null);
            //PO.log.Info("*** Hour 5 ***");
            //s.SetScheduleType(SCHEDULETYPE_Frequency);
            //s.SetFrequencyType(FREQUENCYTYPE_Hour);
            //s.SetFrequency(5);
            ////	start = new Timestamp(System.currentTimeMillis());
            //s.GetNext(start, 10);

            /**	Test Case - Minutes 	**/
            s = new MSchedule(Env.GetContext(), 1000201, null);
            //PO.log.Info("*** Minute 15 ***");
            //s.SetScheduleType(SCHEDULETYPE_Frequency);
            //s.SetFrequencyType(FREQUENCYTYPE_Minute);
            //s.SetFrequency(15);

            //start = new Timestamp(System.currentTimeMillis());
            s.GetNext(start, 10);
        }
    }

}
