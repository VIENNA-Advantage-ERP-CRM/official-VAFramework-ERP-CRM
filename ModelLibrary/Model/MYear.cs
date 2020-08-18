/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_Year
 * Chronological Development
 * Veena Pandey     07-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Utility;
using System.Globalization;
using System.Threading;
using VAdvantage.Logging;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MYear : X_C_Year
    {
        //Cache						
        private static CCache<int, MYear> _years = new CCache<int, MYear>("C_Year", 10);
        private static VLogger s_log = VLogger.GetVLogger(typeof(MYear).FullName);
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Year_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MYear(Ctx ctx, int C_Year_ID, Trx trxName)
            : base(ctx, C_Year_ID, trxName)
        {
            if (C_Year_ID == 0)
            {
                //	setC_Calendar_ID (0);
                //	setYear (null);
                SetProcessing(false);	// N
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MYear(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="calendar">parent</param>
        public MYear(MCalendar calendar)
            : this(calendar.GetCtx(), 0, calendar.Get_TrxName())
        {
            SetClientOrg(calendar);
            SetC_Calendar_ID(calendar.GetC_Calendar_ID());
            SetYear();
        }

        /// <summary>
        /// Set current Year
        /// </summary>
        private void SetYear()
        {
            //GregorianCalendar cal = new GregorianCalendar(Language.getLoginLanguage().getLocale());
            //String Year = (cal.get(Calendar.YEAR)).ToString();
            //base.SetFiscalYear(Year);

            CultureInfo info = Thread.CurrentThread.CurrentCulture;
            GregorianCalendar cal = new GregorianCalendar(System.Globalization.GregorianCalendarTypes.Localized);
            String year = cal.GetYear(DateTime.Now).ToString();
            base.SetFiscalYear(year);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            int yy = GetYearAsInt();
            if (yy == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "Year")
                    + " -> " + yy + " (2006 - 2006/07 - 2006-07 - ...)");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Year As Int
        /// </summary>
        /// <returns>year as int or 0</returns>
        public int GetYearAsInt()
        {
            String year = GetFiscalYear();
            try
            {
                return int.Parse(year);
            }
            catch (Exception e)
            {
                StringTokenizer st = new StringTokenizer(year, "/-, \t\n\r\f");
                if (st.HasMoreTokens())
                {
                    String year2 = st.NextToken();
                    try
                    {
                        return int.Parse(year2);
                    }
                    catch (Exception e2)
                    {
                        log.Log(Level.WARNING, year + "->" + year2 + " - " + e2.ToString());
                    }
                }
                else
                {
                   log.Log(Level.WARNING, year + " - " + e.ToString());
                }
            }
            return 0;
        }

        /// <summary>
        /// Get last two characters of year
        /// </summary>
        /// <returns>01</returns>
        public String GetYY()
        {
            int yy = GetYearAsInt();
            String year = yy.ToString();
            if (year.Length == 4)
                return year.Substring(2, 2);
            return GetFiscalYear();
        }

        /// <summary>
        /// Create 12 Standard (Jan-Dec) Periods.
        /// Creates also Period Control from DocType.
        /// See DocumentTypeVerify#createPeriodControls(Ctx, int, SvrProcess, String)
        /// </summary>
        /// <param name="locale">locale</param>
        /// <returns>true if created</returns>
        //public Boolean CreateStdPeriods(Locale locale)
        public Boolean CreateStdPeriods(CultureInfo locale)
        {
            if (locale == null)
            {
                //MClient client = MClient.Get(GetCtx());
                //locale = client.getLocale();

                locale = Thread.CurrentThread.CurrentCulture;
            }

            //if (locale == null && Language.getLoginLanguage() != null)
            //    locale = Language.getLoginLanguage().getLocale();
            //if (locale == null)
            //    locale = Env.getLanguage(GetCtx()).getLocale();
            //
            String[] months = null;
            try
            {
                months = DateTimeFormatInfo.CurrentInfo.MonthNames;

                //DateFormatSymbols symbols = new DateFormatSymbols(locale);
                //months = symbols.getShortMonths();
            }
            catch 
            {
                months = new String[]{"Jan", "Feb", "Nar",
                "Apr", "May", "Jun",
                "Jul", "Aug", "Sep",
                "Oct", "Nov", "Dec"};
            }
            //
            int year = GetYearAsInt();

            //CultureInfo info = Thread.CurrentThread.CurrentCulture;
            //System.Globalization.GregorianCalendar cal = new System.Globalization.GregorianCalendar(System.Globalization.GregorianCalendarTypes.Localized);
            //
            for (int month = 1; month < 13; month++)
            {
                DateTime start = new DateTime(year, month, 1).Date;
                String name = months[month - 1] + "-" + GetYY();
                //
                int day = TimeUtil.GetMonthLastDay(new DateTime(year, month, 1)).Day;
                DateTime end = new DateTime(year, month, day).Date;
                //
                MPeriod period = new MPeriod(this, month, name, start, end);
                if (!period.Save(Get_TrxName()))	//	Creates Period Control
                    return false;
            }

            //GregorianCalendar cal = new GregorianCalendar(locale);
            //cal.set(Calendar.HOUR_OF_DAY, 0);
            //cal.set(Calendar.MINUTE, 0);
            //cal.set(Calendar.SECOND, 0);
            //cal.set(Calendar.MILLISECOND, 0);
            ////
            //for (int month = 0; month < 12; month++)
            //{
            //    cal.set(Calendar.YEAR, year);
            //    cal.set(Calendar.MONTH, month);
            //    cal.set(Calendar.DAY_OF_MONTH, 1);
            //    DateTime start = new Timestamp(cal.getTimeInMillis());
            //    String name = months[month] + "-" + getYY();
            //    //
            //    cal.add(Calendar.MONTH, 1);
            //    cal.add(Calendar.DAY_OF_YEAR, -1);
            //    Timestamp end = new Timestamp(cal.getTimeInMillis());
            //    //
            //    MPeriod period = new MPeriod(this, month + 1, name, start, end);
            //    if (!period.Save(Get_TrxName()))	//	Creates Period Control
            //        return false;
            //}
            return true;
        }


        /// <summary>
        /// Create 12 Periods On the basis of User's Choice If User select March-15 then it creates (March-15 to Feb-16).
        /// Creates also Period Control from DocType.
        /// See DocumentTypeVerify#createPeriodControls(Ctx, int, SvrProcess, String)
        /// </summary>
        /// <param name="Month_ID">Month_ID</param>
        /// <returns>true if created</returns>
        //public Boolean CreateStdPeriods(string Month_ID)
        #region Create Custom Periods
        public Boolean CreateCustomPeriods(string Month_ID)
        {
            if (Month_ID == null)
            {
                return false;
            }

            Int32 Mnth_ID = Convert.ToInt32(Month_ID);
            Int32 count = 1;
            String[] months = null;
            try
            {
                months = DateTimeFormatInfo.CurrentInfo.MonthNames;

                //DateFormatSymbols symbols = new DateFormatSymbols(locale);
                //months = symbols.getShortMonths();
            }
            catch (Exception e)
            {
                months = new String[]{"Jan", "Feb", "Nar",
                "Apr", "May", "Jun",
                "Jul", "Aug", "Sep",
                "Oct", "Nov", "Dec"};
            }
            //
            int year = GetYearAsInt();

            //CultureInfo info = Thread.CurrentThread.CurrentCulture;
            //System.Globalization.GregorianCalendar cal = new System.Globalization.GregorianCalendar(System.Globalization.GregorianCalendarTypes.Localized);
            //
            for (int month = Mnth_ID; month < 13; month++)
            {

                DateTime start = new DateTime(year, month, 1).Date;
                String name = months[month - 1] + "-" + GetYY();
                //
                int day = TimeUtil.GetMonthLastDay(new DateTime(year, month, 1)).Day;
                DateTime end = new DateTime(year, month, day).Date;
                //

                MPeriod period = new MPeriod(this, count, name, start, end);
                if (!period.Save(Get_TrxName())) // Creates Period Control
                {
                    return false;
                }
                count++;
            }
            for (int month = 1; month < Mnth_ID; month++)
            {
                DateTime start = new DateTime(year + 1, month, 1).Date;
                string yearname = Convert.ToString(Convert.ToInt32(GetYY()) + 1);
                String name = months[month - 1] + "-" + yearname;
                //
                int day = TimeUtil.GetMonthLastDay(new DateTime(year + 1, month, 1)).Day;
                DateTime end = new DateTime(year + 1, month, day).Date;
                //
                MPeriod period = new MPeriod(this, count, name, start, end);
                if (!period.Save(Get_TrxName())) // Creates Period Control
                {
                    return false;
                }
                count++;
            }
            return true;
        }
        #endregion



        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MYear[");
            sb.Append(Get_ID()).Append("-")
                .Append(GetFiscalYear())
                .Append("]");
            return sb.ToString();
        }
        /// <summary>
        /// Get Year
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Year_ID"></param>
        /// <returns>year or null</returns>
        /// <date>07-march-2011</date>
        /// <writer>raghu</writer>
        public static MYear Get(Ctx ctx, int C_Year_ID)
        {
            MYear year = _years.Get(ctx, C_Year_ID);
            if (year != null)
                return year;
            //
            String sql = "SELECT * FROM C_Year WHERE C_Year_ID=" + C_Year_ID;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                if (dt.Rows.Count > 0)	//	first only
                {
                    year = new MYear(ctx, dt.Rows[0], null);
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return year;
        }
    }
}
