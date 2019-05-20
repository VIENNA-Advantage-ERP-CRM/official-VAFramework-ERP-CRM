/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : FinReportPeriod
 * Purpose        : Financial Report Periods
 * Class Used     : --
 * Chronological    Development
 * Deepak           18-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;


namespace VAdvantage.Report
{
    public class FinReportPeriod
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="C_Period_ID">period</param>
        /// <param name="Name">name</param>
        /// <param name="StartDate">period start date</param>
        /// <param name="EndDate">period end date</param>
        /// <param name="YearStartDate">YearStartDate year start date</param>
        public FinReportPeriod(int C_Period_ID, String Name, DateTime? StartDate, DateTime? EndDate,
            DateTime? YearStartDate)
        {
            _C_Period_ID = C_Period_ID;
            _Name = Name;
            _StartDate = StartDate;
            _EndDate = EndDate;
            _YearStartDate = YearStartDate;
        }	//

        private int _C_Period_ID;
        private String _Name;
        private DateTime? _StartDate;
        private DateTime? _EndDate;
        private DateTime? _YearStartDate;


        /// <summary>
        ///	Get Period Info
        /// </summary>
        /// <returns>BETWEEN start AND end</returns>
        public String GetPeriodWhere()
        {
            StringBuilder sql = new StringBuilder("BETWEEN ");
            sql.Append(DataBase.DB.TO_DATE(_StartDate))
                .Append(" AND ")
                .Append(DataBase.DB.TO_DATE(_EndDate));
            return sql.ToString();
        }	//	getPeriodWhere

        /// <summary>
        ///	Get Year Info
        /// </summary>
        /// <returns>BETWEEN start AND end</returns>
        public String GetYearWhere()
        {
            StringBuilder sql = new StringBuilder("BETWEEN ");
            sql.Append(DataBase.DB.TO_DATE(_YearStartDate))
                  .Append(" AND ")
                  .Append(DataBase.DB.TO_DATE(_EndDate));
            return sql.ToString();
        }	//	getPeriodWhere

        /// <summary>
        ///	Get Total Info
        /// </summary>
        /// <returns><= end</returns>
        public String GetTotalWhere()
        {
            StringBuilder sql = new StringBuilder("<= ");
            sql.Append(DataBase.DB.TO_DATE(_EndDate));
            return sql.ToString();
        }	//	getPeriodWhere

        /// <summary>
        /// Is date in period
        /// </summary>
        /// <param name="date">date</param>
        /// <returns> true if in period</returns>
        public Boolean InPeriod(DateTime? date)
        {
            if (date == null)
            {
                return false;
            }
            if (date > _StartDate)
            {
                return false;
            }
            if (date < _EndDate)
            {
                return false;
            }
            return true;
        }	//	inPeriod

        /// <summary>
        ///	Get Name
        /// </summary>
        /// <returns>name</returns>
        public String GetName()
        {
            return _Name;
        }
        /// <summary>
        ///	Get C_Period_ID
        /// </summary>
        /// <returns>period</returns>
        public int GetC_Period_ID()
        {
            return _C_Period_ID;
        }
        /// <summary>
        ///	Get End Date
        /// </summary>
        /// <returns>end date</returns>
        public DateTime? GetEndDate()
        {
            return _EndDate;
        }
        /// <summary>
        ///	Get Start Date
        /// </summary>
        /// <returns>start date</returns>
        public DateTime? GetStartDate()
        {
            return _StartDate;
        }
        /// <summary>
        ///	Get Year Start Date
        /// </summary>
        /// <returns>year start date</returns>
        public DateTime? GetYearStartDate()
        {
            return _YearStartDate;
        }

    }	//	FinReportPeriod

}
