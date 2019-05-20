/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : 
 * Chronological Development
 * 
 * Veena Pandey     Added functions
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Common;
using VAdvantage.Logging;
using VAdvantage.Classes;

namespace VAdvantage.Utility
{
    public class TimeUtil
    {
        /** Truncate Day - D			*/
        public const String TRUNC_DAY = "D";
        /** Truncate Week - W			*/
        public const String TRUNC_WEEK = "W";
        /** Truncate Month - MM			*/
        public const String TRUNC_MONTH = "MM";
        /** Truncate Quarter - Q		*/
        public const String TRUNC_QUARTER = "Q";
        /** Truncate Year - Y			*/
        public const String TRUNC_YEAR = "Y";

        public const int JANUARY = 1;
        public const int FEBRUARY = 2;
        public const int MARCH = 3;
        public const int APRIL = 4;
        public const int MAY = 5;
        public const int JUNE = 6;
        public const int JULY = 7;
        public const int AUGUST = 8;
        public const int SEPTEMBER = 9;
        public const int OCTOBER = 10;
        public const int NOVEMBER = 11;
        public const int DECEMBER = 12;


        /**
	     * 	Is it the same hour
	     * 	@param one day/time
	     * 	@param two compared day/time
	     * 	@return true if the same day
	     */
        public static Boolean IsSameHour(DateTime? one, DateTime? two)
        {
            int retValue = -1;
            if (one != null && two != null)
            {
                retValue = ((DateTime)one).CompareTo((DateTime)two);
            }
            if (retValue == 0)
                return true;
            return false;

            //GregorianCalendar calOne = new GregorianCalendar();
            //if (one != null)
            //    calOne.setTimeInMillis(one.getTime());
            //GregorianCalendar calTwo = new GregorianCalendar();
            //if (two != null)
            //    calTwo.setTimeInMillis(two.getTime());
            //if (calOne.get(Calendar.YEAR) == calTwo.get(Calendar.YEAR)
            //    && calOne.get(Calendar.MONTH) == calTwo.get(Calendar.MONTH)
            //    && calOne.get(Calendar.DAY_OF_MONTH) == calTwo.get(Calendar.DAY_OF_YEAR)
            //    && calOne.get(Calendar.HOUR_OF_DAY) == calTwo.get(Calendar.HOUR_OF_DAY))
            //    return true;
            //return false;


        }

        /// <summary>
        /// Is it the same day
        /// </summary>
        /// <param name="one">one day</param>
        /// <param name="two">compared day</param>
        /// <returns>true if the same day</returns>
        public static bool IsSameDay(DateTime? one, DateTime? two)
        {
            //retValue = ((DateTime)one).CompareTo((DateTime)two);

            if (one == null && two == null)
            {
                return true;
            }
            if (one == null && two != null)
            {
                return false;
            }
            if (one != null && two == null)
            {
                return false;
            }

            if (one.Value.Year == two.Value.Year && one.Value.Month == two.Value.Month
                && one.Value.Day == two.Value.Day)
            {
                return true;
            }
            return false;
            //GregorianCalendar calOne = new GregorianCalendar();
            //if (one != null)
            //    calOne.setTimeInMillis(one.getTime());
            //GregorianCalendar calTwo = new GregorianCalendar();
            //if (two != null)
            //    calTwo.setTimeInMillis(two.getTime());
            //if (calOne.get(Calendar.YEAR) == calTwo.get(Calendar.YEAR)
            //    && calOne.get(Calendar.MONTH) == calTwo.get(Calendar.MONTH)
            //    && calOne.get(Calendar.DAY_OF_MONTH) == calTwo.get(Calendar.DAY_OF_YEAR))
            //    return true;
            //return false;
        }

        public static DateTime AddDays(DateTime? date, int offset)
        {
            DateTime day;
            if (date == null)
                day = DateTime.Now;

            day = (DateTime)date;
            day = day.AddDays(offset);

            DateTime retValue = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);

            return retValue;
        }

        /// <summary>
        /// Is it valid today?
        /// </summary>
        /// <param name="validFrom">valid from</param>
        /// <param name="validTo">valid to</param>
        /// <returns>true if valid</returns>
        /// <author>Veena</author>
        public static bool IsValid(DateTime? validFrom, DateTime? validTo)
        {
            return IsValid(validFrom, validTo, DateTime.Now);
        }

        /// <summary>
        /// Is it valid on test date
        /// </summary>
        /// <param name="validFrom">valid from</param>
        /// <param name="validTo">valid to</param>
        /// <param name="testDate">date</param>
        /// <returns>true if valid</returns>
        /// <author>Veena</author>
        public static bool IsValid(DateTime? validFrom, DateTime? validTo, DateTime? testDate)
        {
            if (testDate == null)
                return true;
            if (validFrom == null && validTo == null)
                return true;
            //	(validFrom)	ok
            if (validFrom != null && (DateTime)validFrom > (DateTime)testDate)
                return false;
            //	ok	(validTo)
            if (validTo != null && (DateTime)validTo < (DateTime)testDate)
                return false;
            return true;
        }

        /// <summary>
        /// Get earliest time of a day (truncate)
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="month">month 1..12</param>
        /// <param name="day">day 1..31</param>
        /// <returns>day with 00:00</returns>
        /// <author>Veena</author>
        public static DateTime GetDay(int year, int month, int day)
        {
            if (year < 50)
                year += 2000;
            else if (year < 100)
                year += 1900;
            if (month < 1 || month > 12)
                throw new ArgumentException("Invalid Month: " + month);
            if (day < 1 || day > 31)
                throw new ArgumentException("Invalid Day: " + month);
            //GregorianCalendar cal = new GregorianCalendar(year, month - 1, day);
            //return new Timestamp(cal.getTimeInMillis());

            return GetDay(new DateTime(year, month, day));
        }

        /// <summary>
        /// Get earliest time of a day (truncate)
        /// </summary>
        /// <param name="dayTime">date</param>
        /// <returns>day with 00:00</returns>
        /// <author>Veena</author>
        public static DateTime GetDay(DateTime? dayTime)
        {
            if (dayTime == null)
                return DateTime.Now.Date;
            return dayTime.Value.Date;

            //if (dayTime == null)
            //    return getDay(System.currentTimeMillis());
            //return getDay(dayTime.getTime());
        }

        /// <summary>
        /// Gets month's last date
        /// </summary>
        /// <param name="day">date</param>
        /// <returns>month's last date</returns>
        /// <author>Veena</author>
        public static DateTime GetMonthLastDay(DateTime? day)
        {
            DateTime dt;
            if (day == null)
                dt = DateTime.Now;
            else
                dt = day.Value;
           // dt = (DateTime)dt;
            return dt.AddMonths(1).AddDays(-dt.Day).Date;
        }
        /**
    * 	Return Day + offset month (truncates)
    * 	@param day Day
    * 	@param offset month offset
    * 	@return Day + offset at 00:00
    */
        static public DateTime AddMonths(DateTime? day, int offset)
        {
            if (day == null)
            {
                day = DateTime.Now.Date;
            }

            //add days in date
            if (offset == 0)
            {
                return DateTime.Now.Date;
            }

            return day.Value;
        }

        /// <summary>
        /// Get truncated day/time
        /// </summary>
        /// <param name="dayTime">day</param>
        /// <param name="trunc">how to truncate TRUNC_*</param>
        /// <returns>next day with 00:00</returns>
        /// <author>Veena</author>
        public static DateTime Trunc(DateTime? dayTime, String trunc)
        {
            //return DateTime.Now.Date;
            if (dayTime == null)
                dayTime = DateTime.Now;
            //	D (Return same date.Date)
            if (trunc == null || trunc.Equals(TRUNC_DAY))
                return DateTime.Now.Date; // return same date.Date

            //	W (Returns date of 1st day of current week(i.e. sunday's date))
            if (trunc.Equals(TRUNC_WEEK))
            {
                DayOfWeek today = dayTime.Value.DayOfWeek;
                return dayTime.Value.AddDays(-(int)today); // returns date of 1st day of current week(i.e. sunday's date)

                //CultureInfo info = Thread.CurrentThread.CurrentCulture;
                //DayOfWeek firstday = info.DateTimeFormat.FirstDayOfWeek;
                //DayOfWeek today = info.Calendar.GetDayOfWeek(dayTime);
                //int diff = today - firstday;
                //return dayTime.AddDays(-diff); // returns first day of week(i.e. sunday's date)
            }
            // MM (Returns 1st day of month)
            if (trunc.Equals(TRUNC_MONTH))
            {
                return new DateTime(dayTime.Value.Year, dayTime.Value.Month, 1); // returns 1st day of month
            }
            //	Q
            if (trunc.Equals(TRUNC_QUARTER))
            {
                /*******************************************************************
                (jan-april) -- 01 feb
                (may-july)  -- 01 may
                (aug-oct) -- 01 aug
                (nov-dec)  -- 01 nov
                 * ******************************************************************/

                int mm = dayTime.Value.Month;
                if (mm < MAY)
                    mm = FEBRUARY;
                else if (mm < AUGUST)
                    mm = MAY;
                else if (mm < NOVEMBER)
                    mm = AUGUST;
                else
                    mm = NOVEMBER;
                return new DateTime(dayTime.Value.Year, mm, 1);
            }

            // Y (Returns this years 1st day)
            return new DateTime(dayTime.Value.Year, 1, 1);  // returns this years 1st day
        }

        /// <summary>
        /// Calculate the number of days between start and end.
        /// </summary>
        /// <param name="start">start date</param>
        /// <param name="end">end date</param>
        /// <returns>number of days (0 = same)</returns>
        /// /* modify by Harwinder
        public static int GetDaysBetween(DateTime? start, DateTime? end)
        {
            //
            DateTime dstart = DateTime.Now;
            DateTime dend = DateTime.Now;

            if (start == null || end == null)
                return 0;

            dstart = (DateTime)start.Value.Date;
            dend = (DateTime)end.Value.Date;

            Boolean negative = false;
            if (end < start)//(end.before(start))
            {
                negative = true;
                //swaping date
                DateTime temp = dstart;
                dstart = dend;
                dend = temp;
            }
            //	in same year
            if (dend.Year == dstart.Year)
            {
                if (negative)
                {
                    /* dend.Subtract(dstart).Days changed  to */
                    //return ((dstart.Date.Subtract(dend.Date).Days) * -1);
                    return ((dend.Date.Subtract(dstart.Date).Days) * -1);
                }
                return dend.Date.Subtract(dstart.Date).Days;
            }

            //	not very efficient, but correct
            int counter = 0;
            if (dend > dstart)//(calEnd.after(cal))
            {
                //cal.add(Calendar.DAY_OF_YEAR, 1);
                counter = dend.Date.Subtract(dstart.Date).Days;
            }
            if (negative)
            {
                if (counter < 0)
                {
                    return counter * -1;
                }
            }
            return counter;
            /**
             boolean negative = false;
		if (end.before(start))
		{
			negative = true;
			Timestamp temp = start;
			start = end;
			end = temp;
		}
		//
		GregorianCalendar cal = new GregorianCalendar();
		cal.setTime(start);
		cal.set(Calendar.HOUR_OF_DAY, 0);
		cal.set(Calendar.MINUTE, 0);
		cal.set(Calendar.SECOND, 0);
		cal.set(Calendar.MILLISECOND, 0);
		GregorianCalendar calEnd = new GregorianCalendar();
		calEnd.setTime(end);
		calEnd.set(Calendar.HOUR_OF_DAY, 0);
		calEnd.set(Calendar.MINUTE, 0);
		calEnd.set(Calendar.SECOND, 0);
		calEnd.set(Calendar.MILLISECOND, 0);

	//	System.out.println("Start=" + start + ", End=" + end + ", dayStart=" + cal.get(Calendar.DAY_OF_YEAR) + ", dayEnd=" + calEnd.get(Calendar.DAY_OF_YEAR));

		//	in same year
		if (cal.get(Calendar.YEAR) == calEnd.get(Calendar.YEAR))
		{
			if (negative)
				return (calEnd.get(Calendar.DAY_OF_YEAR) - cal.get(Calendar.DAY_OF_YEAR)) * -1;
			return calEnd.get(Calendar.DAY_OF_YEAR) - cal.get(Calendar.DAY_OF_YEAR);
		}

		//	not very efficient, but correct
		int counter = 0;
		while (calEnd.after(cal))
		{
			cal.add (Calendar.DAY_OF_YEAR, 1);
			counter++;
		}
		if (negative)
			return counter * -1;
		return counter;
          **/
        }

        /// <summary>
        /// Format Elapsed Time
        /// </summary>
        /// <param name="start">start time or null for now</param>
        /// <param name="end">end time or null for now</param>
        /// <returns>formatted time string 1'23:59:59.999</returns>
        public static String FormatElapsed(DateTime start, DateTime end)
        {
            long startTime = 0;
            if (start == null)
            {
                startTime = CommonFunctions.CurrentTimeMillis();
            }


            long endTime = 0;
            if (end == null)
            {
                endTime = CommonFunctions.CurrentTimeMillis();
            }

            TimeSpan ts = end.Subtract(start);

            return FormatElapsed(ts.TotalMilliseconds);

            //DateTime dt = Convert.ToDateTime(end.Date - start.Date);
            //return dt.ToString();
        }


        public static String FormatElapsed(DateTime start)
        {
            if (start == null)
                return "NoStartTime";
            long startTime = CommonFunctions.CurrentTimeMillis(start);
            long endTime = CommonFunctions.CurrentTimeMillis();
            return FormatElapsed(endTime - startTime);
        }	//	formatElapsed


        /// <summary>
        /// Format Elapsed TimeUtil now
        /// </summary>
        /// <param name="start">start time</param>
        /// <returns>formatted time string 1'23:59:59.999</returns>
        public static String FormatElapsed(long elapsedMS)
        {
            if (elapsedMS == 0)
                return "0";
            StringBuilder sb = new StringBuilder();
            if (elapsedMS < 0)
            {
                elapsedMS = -elapsedMS;
                sb.Append("-");
            }
            //
            long miliSeconds = elapsedMS % 1000;
            elapsedMS = elapsedMS / 1000;
            long seconds = elapsedMS % 60;
            elapsedMS = elapsedMS / 60;
            long minutes = elapsedMS % 60;
            elapsedMS = elapsedMS / 60;
            long hours = elapsedMS % 24;
            long days = elapsedMS / 24;
            //
            if (days != 0)
                sb.Append(days).Append("'");
            //	hh
            if (hours != 0)
                sb.Append(Get2digits(hours)).Append(":");
            else if (days != 0)
                sb.Append("00:");
            //	mm
            if (minutes != 0)
                sb.Append(Get2digits(minutes)).Append(":");
            else if (hours != 0 || days != 0)
                sb.Append("00:");
            //	ss
            sb.Append(Get2digits(seconds))
                .Append(".").Append(miliSeconds);
            return sb.ToString();
        }

        /**
	 * 	Format Elapsed Time
	 *	@param elapsedMS time in ms
	 *	@return formatted time string 1'23:59:59.999 - d'hh:mm:ss.xxx
	 */
        public static String FormatElapsed(double elapsedMS)
        {


            if (elapsedMS == 0)
                return "0";
            StringBuilder sb = new StringBuilder();
            if (elapsedMS < 0)
            {
                elapsedMS = -elapsedMS;
                sb.Append("-");
            }
            //
            long miliSeconds = System.Convert.ToInt64(elapsedMS % 1000);
            elapsedMS = elapsedMS / 1000;
            long seconds = System.Convert.ToInt64(elapsedMS % 60);
            elapsedMS = elapsedMS / 60;
            long minutes = System.Convert.ToInt64(elapsedMS % 60);
            elapsedMS = elapsedMS / 60;
            long hours = System.Convert.ToInt64(elapsedMS % 24);
            long days = System.Convert.ToInt64(elapsedMS / 24);
            //
            if (days != 0)
                sb.Append(days).Append("'");
            //	hh
            if (hours != 0)
                sb.Append(Get2digits(hours)).Append(":");
            else if (days != 0)
                sb.Append("00:");
            //	mm
            if (minutes != 0)
                sb.Append(Get2digits(minutes)).Append(":");
            else if (hours != 0 || days != 0)
                sb.Append("00:");
            //	ss
            sb.Append(Get2digits(seconds))
                .Append(".").Append(miliSeconds);
            return sb.ToString();
        }
        private static String Get2digits(long no)
        {
            String s = no.ToString();
            if (s.Length > 1)
                return s;
            return "0" + s;
        }	//	get2digits


        /// <summary>
        ///Return DateTime + offset in minutes
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        static public DateTime AddMinutes(DateTime? dateTime, int offset)
        {
            if (dateTime == null)
                dateTime = DateTime.Now;
            if (offset == 0)
                return dateTime.Value;
            //
            //GregorianCalendar cal = new GregorianCalendar();
            return dateTime.Value.AddMinutes(offset);
            //cal.setTime(dateTime);
            //cal.add(Calendar.MINUTE, offset);			//	may have a problem with negative
            //return new Timestamp(cal.getTimeInMillis());
        }	//	addMinutes

        /**
	 * 	Get earliest time of next day
	 *  @param day day
	 *  @return next day with 00:00
	 */
        static public DateTime GetNextDay(DateTime? day)
        {
            if (day == null)
                day = DateTime.Now;
            DateTime cDay = (DateTime)day;
            cDay = new DateTime(cDay.Year, cDay.Month, cDay.Day);
            return cDay.Date.AddDays(1);//  .Date.Day+1;
            ////GregorianCalendar cal = new GregorianCalendar(Language.getLoginLanguage().getLocale());
            //DateTime retDay = new DateTime(day
            //cal.setTimeInMillis(day.getTime());
            //cal.add(Calendar.DAY_OF_YEAR, +1);	//	next
            //cal.set(Calendar.HOUR_OF_DAY, 0);
            //cal.set(Calendar.MINUTE, 0);
            //cal.set(Calendar.SECOND, 0);
            //cal.set(Calendar.MILLISECOND, 0);
            //return new  DateTime(cDay.Year,cDay.Month,cDay.Day+1)   Timestamp(cal.getTimeInMillis());
        }	//	getNextDay


        /**
         * 	Return the day and time
         * 	@param day day part
         * 	@param time time part
         * 	@return day + time
         */
        static public DateTime GetDayTime(DateTime? day, DateTime? time)
        {
            if (day == null)
            {
                day = DateTime.Now;
            }
            if (time == null)
            {
                time = DateTime.Now;
            }

            DateTime day_1 = (DateTime)day;
            DateTime time_1 = (DateTime)time;

            //GregorianCalendar cal_1 = new GregorianCalendar();
            //cal_1.setTimeInMillis(day.getTime());
            //GregorianCalendar cal_2 = new GregorianCalendar();
            //cal_2.setTimeInMillis(time.getTime());
            ////
            //GregorianCalendar cal = new GregorianCalendar(Language.getLoginLanguage().getLocale());
            //cal.set(cal_1.get(Calendar.YEAR),
            //    cal_1.get(Calendar.MONTH),
            //    cal_1.get(Calendar.DAY_OF_MONTH),
            //    cal_2.get(Calendar.HOUR_OF_DAY),
            //    cal_2.get(Calendar.MINUTE),
            //    cal_2.get(Calendar.SECOND));
            //cal.set(Calendar.MILLISECOND, 0);
            //Timestamp retValue = new Timestamp(cal.getTimeInMillis());
            //	log.fine( "TimeUtil.getDayTime", "Day=" + day + ", Time=" + time + " => " + retValue);
            DateTime retValue = new DateTime(day_1.Year, day_1.Month, day_1.Day, time_1.Hour, time_1.Minute, time_1.Second, 0);
            return retValue;
        }	//	getDayTime


        /**
	 * 	Is the _1 in the Range of _2
	 *  <pre>
	 * 		Time_1         +--x--+
	 * 		Time_2   +a+      +---b---+   +c+
	 * 	</pre>
	 *  The function returns true for b and false for a/b.
	 *  @param start_1 start (1)
	 *  @param end_1 not included end (1)
	 *  @param start_2 start (2)
	 *  @param end_2 not included (2)
	 *  @return true if in range
	 */
        static public bool InRange(DateTime? start1, DateTime? end1, DateTime? start2, DateTime? end2)
        {
            if (start1 == null || end1 == null || start2 == null || end2 == null)
            {
                return false;
            }
            DateTime start_1, end_1, start_2, end_2;
            start_1 = (DateTime)start1;
            end_1 = (DateTime)end1;
            start_2 = (DateTime)start2;
            end_2 = (DateTime)end2;

            //	validity check
            if (end_1 < start_1)
                throw new ArgumentOutOfRangeException("TimeUtil.inRange End_1=" + end_1 + " before Start_1=" + start_1);
            if (end_2 < start_2)
                throw new ArgumentOutOfRangeException("TimeUtil.inRange End_2=" + end_2 + " before Start_2=" + start_2);
            //	case a
            if (!(end_2 > start_1))		//	end not including
            {
                //		log.fine( "TimeUtil.InRange - No", start_1 + "->" + end_1 + " <??> " + start_2 + "->" + end_2);
                return false;
            }
            //	case c
            if (!(start_2 < end_1))		//	 end not including
            {
                //		log.fine( "TimeUtil.InRange - No", start_1 + "->" + end_1 + " <??> " + start_2 + "->" + end_2);
                return false;
            }
            //	log.fine( "TimeUtil.InRange - Yes", start_1 + "->" + end_1 + " <??> " + start_2 + "->" + end_2);
            return true;
        }	//	inRange

        /**
         * 	Is start..end on one of the days ?
         *  @param start start day
         *  @param end end day (not including)
         *  @param OnMonday true if OK
         *  @param OnTuesday true if OK
         *  @param OnWednesday true if OK
         *  @param OnThursday true if OK
         *  @param OnFriday true if OK
         *  @param OnSaturday true if OK
         *  @param OnSunday true if OK
         *  @return true if on one of the days
         */
        static public bool InRange(DateTime? start, DateTime? end,
            bool OnMonday, bool OnTuesday, bool OnWednesday,
            bool OnThursday, bool OnFriday, bool OnSaturday, bool OnSunday)
        {
            //	are there restrictions?
            if (OnSaturday && OnSunday && OnMonday && OnTuesday && OnWednesday && OnThursday && OnFriday)
                return false;

            DateTime calStart = new DateTime(start.Value.Ticks);// GregorianCalendar();
            //calStart.setTimeInMillis(start.getTime());
            int dayStart = (int)calStart.DayOfWeek;
            ////
            DateTime calEnd = new DateTime(end.Value.Ticks);// GregorianCalendar();
            //calEnd.setTimeInMillis(end.getTime());
            //calEnd.add(Calendar.DAY_OF_YEAR, -1);	//	not including
            calEnd = calEnd.AddDays(-1);
            int dayEnd = (int)calEnd.DayOfWeek;

            ////	On same day
            if (calStart.Year == calEnd.Year
                && calStart.Month == calEnd.Month
                && (int)calStart.DayOfWeek == calEnd.DayOfYear)
            {
                if ((!OnSaturday && dayStart.Equals(DayOfWeek.Saturday))
                    || (!OnSunday && dayStart.Equals(DayOfWeek.Sunday))
                    || (!OnMonday && dayStart.Equals(DayOfWeek.Monday))
                    || (!OnTuesday && dayStart.Equals(DayOfWeek.Tuesday))
                    || (!OnWednesday && dayStart.Equals(DayOfWeek.Wednesday))
                    || (!OnThursday && dayStart.Equals(DayOfWeek.Thursday))
                    || (!OnFriday && dayStart.Equals(DayOfWeek.Friday)))
                {
                    //		log.fine( "TimeUtil.InRange - SameDay - Yes", start + "->" + end + " - "
                    //        //			+ OnMonday+"-"+OnTuesday+"-"+OnWednesday+"-"+OnThursday+"-"+OnFriday+"="+OnSaturday+"-"+OnSunday);
                    return true;
                }
                //    //	log.fine( "TimeUtil.InRange - SameDay - No", start + "->" + end + " - "
                //    //		+ OnMonday+"-"+OnTuesday+"-"+OnWednesday+"-"+OnThursday+"-"+OnFriday+"="+OnSaturday+"-"+OnSunday);
                return false;
            }
            ////
            ////	log.fine( "TimeUtil.inRange - WeekDay Start=" + dayStart + ", Incl.End=" + dayEnd);

            ////	Calendar.SUNDAY=1 ... SATURDAY=7
            //bit days = new BitSet(8);
            System.Collections.BitArray days = new System.Collections.BitArray(8, false);
            //	Set covered days in BitArray
            if (dayEnd <= dayStart)
                dayEnd += 7;
            for (int i = dayStart; i < dayEnd; i++)
            {
                int index = i;
                if (index > 7)
                    index -= 7;
                days.Set(index, true);
                //		System.out.println("Set index=" + index + " i=" + i);
            }

            //	for (int i = Calendar.SUNDAY; i <= Calendar.SATURDAY; i++)
            ////		System.out.println("Result i=" + i + " - " + days.get(i));

            //	Compare days to availability
            if ((!OnSaturday && days.Get((int)DayOfWeek.Saturday))
                || (!OnSunday && days.Get((int)DayOfWeek.Sunday))
                || (!OnMonday && days.Get((int)DayOfWeek.Monday))
                || (!OnTuesday && days.Get((int)DayOfWeek.Tuesday))
                || (!OnWednesday && days.Get((int)DayOfWeek.Wednesday))
                || (!OnThursday && days.Get((int)DayOfWeek.Thursday))
                || (!OnFriday && days.Get((int)DayOfWeek.Friday)))
            {
                //    //		log.fine( "MAssignment.InRange - Yes",	start + "->" + end + " - "
                //    //			+ OnMonday+"-"+OnTuesday+"-"+OnWednesday+"-"+OnThursday+"-"+OnFriday+"="+OnSaturday+"-"+OnSunday);
                return true;
            }

            ////	log.fine( "MAssignment.InRange - No", start + "->" + end + " - "
            ////		+ OnMonday+"-"+OnTuesday+"-"+OnWednesday+"-"+OnThursday+"-"+OnFriday+"="+OnSaturday+"-"+OnSunday);
            return false;
        }	//	isRange

        /**
	 * 	Is all day
	 * 	@param start start date
	 * 	@param end end date
	 * 	@return true if all day (00:00-00:00 next day)
	 */
        static public bool IsAllDay(DateTime? start_1, DateTime? end_1)
        {
            if (start_1 == null || end_1 == null)
            {
                return false;
            }
            DateTime start = (DateTime)start_1;
            DateTime end = (DateTime)end_1;

            //GregorianCalendar calStart = new GregorianCalendar();
            // calStart.setTimeInMillis(start.getTime());
            //GregorianCalendar calEnd = new GregorianCalendar();
            //calEnd.setTimeInMillis(end.getTime());
            //if (calStart.get(Calendar.HOUR_OF_DAY) == calEnd.get(Calendar.HOUR_OF_DAY)
            //    && calStart.get(Calendar.MINUTE) == calEnd.get(Calendar.MINUTE)
            //    && calStart.get(Calendar.SECOND) == calEnd.get(Calendar.SECOND)
            //    && calStart.get(Calendar.MILLISECOND) == calEnd.get(Calendar.MILLISECOND)
            //    && calStart.get(Calendar.HOUR_OF_DAY) == 0
            //    && calStart.get(Calendar.MINUTE) == 0
            //    && calStart.get(Calendar.SECOND) == 0
            //    && calStart.get(Calendar.MILLISECOND) == 0
            //    && start.before(end))
            //    return true;
            ////
            //return false;
            if (start.Hour == end.Hour
                && start.Minute == end.Minute
                && start.Second == end.Second
                && start.Millisecond == end.Millisecond
                && start.Hour == 0
                && start.Minute == 0
                && start.Second == 0
                && start.Millisecond == 0
                && start < end)
                return true;
            //
            return false;
        }	//	isAllDay

        public static DateTime FirstDayOfWeek(DateTime date)
        {
            DayOfWeek today = date.DayOfWeek;
            DateTime firstDate = date.AddDays(-(int)today);
            return firstDate;
        }
        public static DateTime FirstDayOfMonth(DateTime? date)
        {
            DateTime dt;
            if (date == null)
                dt = DateTime.Now;
            else
                dt = date.Value;
            // dt = (DateTime)dt;
            return dt.AddDays(-dt.Day + 1);
        }

        /// <summary>
        /// Max date
        /// </summary>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns></returns>
        public static DateTime? Max(DateTime? ts1, DateTime? ts2)
        {
            if (ts1 == null)
                return ts2;
            if (ts2 == null)
                return ts1;

            if (ts2.Value.Millisecond > ts1.Value.Millisecond)
                return ts2;
            return ts1;
        }


    }
}
