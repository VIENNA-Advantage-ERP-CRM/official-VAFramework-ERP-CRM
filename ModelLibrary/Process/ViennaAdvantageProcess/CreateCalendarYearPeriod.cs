using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Model;
using System.Globalization;
using System.Data;

namespace ViennaAdvantage.Process
{
    class CreateCalendarYearPeriod : VAdvantage.ProcessEngine.SvrProcess
    {
        //int daysPast = 0;
        //int dayFuture = 0;
        int MonthFrom = 0;
        int MonthTo = 0;
        int DateFrom = 0;
        int DateTo = 0;
       // char EntireYear='N';
        protected override void Prepare()
        {
            //ProcessInfoParameter[] para = GetParameter();
            //for (int i = 0; i < para.Length; i++)
            //{
            //    String name = para[i].GetParameterName();
            //    //	log.fine("prepare - " + para[i]);
            //    if (para[i].GetParameter() == null)
            //    {
            //        ;
            //    }
            //    else if (name.Equals("MonthFrom"))
            //    {
            //        MonthFrom = para[i].GetParameterAsInt();
            //    }
            //    else if (name.Equals("MonthTo"))
            //    {
            //        MonthTo = para[i].GetParameterAsInt();
            //    }
            //    else if (name.Equals("EntireYear"))
            //    {
            //        EntireYear = Convert.ToChar(para[i].GetParameter());
            //    }
            //    else
            //    {
            //        log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            //    }
            //}
        }

        protected override string DoIt()
        {
            string status = "OK";
            Trx trx = Trx.Get("CreateCalYearPeriod");
            StringBuilder sql = new StringBuilder();
            //GetTenantInfo
            sql.Append(@"SELECT 
                            periodstartfrommonth,
                            periodstartfromday,
                            periodendMonth,
                            periodendsAtDay,                           
                            period_openhistory,
                            period_openfuture
                         FROM c_acctschema                            
                         WHERE IsActive='Y' 
                           AND IsActive='Y'
                           AND AD_Client_ID=" + GetCtx().GetAD_Client_ID());
            DataSet ds = DB.ExecuteDataset(sql.ToString(),null);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return "SettingsNotFound";
            }
            try 
            {
                try
                {
                    MonthFrom = Convert.ToInt32(ds.Tables[0].Rows[0]["periodstartfrommonth"]);
                }
                catch { }
                try
                {
                    MonthTo = Convert.ToInt32(ds.Tables[0].Rows[0]["periodendMonth"]);
                }
                catch { }
                //try
                //{
                //    EntireYear = ds.Tables[0].Rows[0]["periodendMonth"].ToString().Equals("Y") ? 'Y' : 'N';
                //}
                //catch { }
                try
                {
                    DateFrom = Convert.ToInt32(ds.Tables[0].Rows[0]["periodstartfromday"]);
                }
                catch { }
                try
                {
                    DateTo = Convert.ToInt32(ds.Tables[0].Rows[0]["periodendsAtDay"]);
                }
                catch { }
                //try
                //{
                //    daysPast = Convert.ToInt32(ds.Tables[0].Rows[0]["period_openhistory"]);
                //}
                //catch { }
                //try
                //{
                //    dayFuture = Convert.ToInt32(ds.Tables[0].Rows[0]["period_openfuture"]);
                //}
                //catch { }
            }
            catch { }
            string YearName = "";
            bool isNextYear = false;
            if (MonthFrom < MonthTo)
            {
                YearName = DateTime.Now.Year.ToString();
            }
            else
            {
                YearName = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Year + 1).ToString();
                isNextYear = true;
            }
            sql.Clear();
            sql.Append(@"SELECT C_Calendar_ID FROM C_Calendar
                                        WHERE ISACTIVE='Y' AND AD_CLIENT_ID=" + GetCtx().GetAD_Client_ID() + @"
                                        AND AD_ORG_ID=(SELECT AD_ORG_ID FROM AD_ORG WHERE NAME ='*' )");
            int calendarID = 0;
            try
            {
                calendarID = Convert.ToInt32(DB.ExecuteScalar(sql.ToString()));
            }
            catch { }
            if (calendarID > 0)
            {
                sql.Clear();
                sql.Append(@"SELECT C_YEAR_ID FROM C_YEAR WHERE ISACTIVE='Y' AND FiscalYear='" + YearName + "' AND C_CALENDAR_ID=" + calendarID);
                int yearID = 0;
                try
                {
                    yearID = Convert.ToInt32(DB.ExecuteScalar(sql.ToString()));
                }
                catch { }
                MYear year = new MYear(GetCtx(), yearID, trx);
                year.SetC_Calendar_ID(calendarID);
                year.SetFiscalYear(YearName);
                year.SetIsActive(true);
                year.SetAD_Org_ID(0);
                if (!year.Save(trx))
                {
                    status = "YearNotSaved";
                }
                if (!isNextYear)
                {
                    for (int month = MonthFrom; month <= MonthTo; month++)
                    {
                        DateTime? start = null;
                        if (month == MonthFrom)
                        {
                           start= new DateTime(DateTime.Now.Year, month, DateFrom).Date;
                        }
                        else
                        {
                            start = new DateTime(DateTime.Now.Year, month, 1).Date;
                        }
                        String name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "-" + YearName;
                        //
                        int day =0;

                        if (month == MonthTo)
                        {
                            day = DateTo;//TimeUtil.GetMonthLastDay(new DateTime(DateTime.Now.Year, month, DateTo)).Day;
                        }
                        else
                        {
                            day = TimeUtil.GetMonthLastDay(new DateTime(DateTime.Now.Year, month, 1)).Day;
                        }
                        DateTime end = new DateTime(DateTime.Now.Year, month, day).Date;
                        //
                        MPeriod period = new MPeriod(year, month, name, start, end);
                        if (!period.Save(trx))	//	Creates Period Control
                            status= "PeriodNotSaved";
                        //if (EntireYear.Equals('Y'))//open Period for Entire Year
                        //{

                        //    if (period.Get_ID() == 0)
                        //    {
                        //        continue;
                        //    }
                        //    if (!OpenPeriod(period, trx))
                        //    {
                        //        status = "PeriodNotOpened";
                        //    }
                        //}
                        //else
                        //{
                        //    if (month == DateTime.Now.Month)
                        //    {
                        //        if (period.Get_ID() == 0)
                        //        {
                        //            continue;
                        //        }
                        //        if (!OpenPeriod(period, trx))
                        //        {
                        //            status = "PeriodNotOpened";
                        //        }
                        //    }
                        //}
                    }
                }
                else
                {
                    for (int month = MonthFrom; month < 13; month++)
                    {
                        DateTime? start = null;
                        if (month == MonthFrom)
                        {
                            start = new DateTime(DateTime.Now.Year, month, DateFrom).Date;
                        }
                        else
                        {
                            start = new DateTime(DateTime.Now.Year, month, 1).Date;
                        }
                        String name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "-" + DateTime.Now.Year.ToString();
                        //
                        int day = 0;

                        if (month == MonthTo)
                        {
                            day = DateTo;//TimeUtil.GetMonthLastDay(new DateTime(DateTime.Now.Year, month, DateTo)).Day;
                        }
                        else
                        {
                            day = TimeUtil.GetMonthLastDay(new DateTime(DateTime.Now.Year, month, 1)).Day;
                        }
                        DateTime end = new DateTime(DateTime.Now.Year, month, day).Date;
                        //
                        MPeriod period = new MPeriod(year, month, name, start, end);
                        if (!period.Save(trx))	//	Creates Period Control
                            status = "PeriodNotSaved";
                        //if (EntireYear.Equals('Y'))//open Period for Entire Year
                        //{

                        //    if (!OpenPeriod(period, trx))
                        //    {
                        //        status = "PeriodNotOpened";
                        //    }
                        //}
                        //else 
                        //{
                        //    if (month == DateTime.Now.Month)
                        //    {
                        //        if (period.Get_ID() == 0)
                        //        {
                        //            continue;
                        //        }
                        //        if (!OpenPeriod(period, trx))
                        //        {
                        //            status = "PeriodNotOpened";
                        //        }
                        //    }
                        //}
                       
                    }
                    for (int month = 1; month <= MonthTo; month++)
                    {
                        DateTime? start = null;
                        if (month == MonthFrom)
                        {
                            start = new DateTime(DateTime.Now.Year+1, month, DateFrom).Date;
                        }
                        else
                        {
                            start = new DateTime(DateTime.Now.Year+1, month, 1).Date;
                        }
                        String name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + "-" +( DateTime.Now.Year+1).ToString();
                        //
                        int day = 0;

                        if (month == MonthTo)
                        {
                            day = DateTo;// TimeUtil.GetMonthLastDay(new DateTime(DateTime.Now.Year, month, DateTo)).Day;
                        }
                        else
                        {
                            day = TimeUtil.GetMonthLastDay(new DateTime(DateTime.Now.Year+1, month, 1)).Day;
                        }
                        DateTime end = new DateTime(DateTime.Now.Year+1, month, day).Date;
                        //
                        MPeriod period = new MPeriod(year, month, name, start, end);
                        if (!period.Save(trx))	//	Creates Period Control
                            status= "PeriodNotSaved";
                        //if (EntireYear.Equals('Y'))//open Period for Entire Year
                        //{

                        //    if (period.Get_ID() == 0)
                        //    {
                        //        continue;
                        //    }
                        //    if (!OpenPeriod(period, trx))
                        //    {
                        //        status = "PeriodNotOpened";
                        //    }
                        //}
                        //else
                        //{
                        //    if (month == DateTime.Now.Month)
                        //    {
                        //        if (period.Get_ID() == 0)
                        //        {
                        //            continue;
                        //        }
                        //        if (!OpenPeriod(period, trx))
                        //        {
                        //            status = "PeriodNotOpened";
                        //        }
                        //    }
                        //}
                    }

                }
            }

            //Open Pereiod for specified Past Days 
//            if (daysPast > 0)
//            {
//                DateTime PeriodEndDate = new DateTime(DateTime.Now.Year, MonthFrom, DateFrom);
//                DateTime from = PeriodEndDate.AddDays(-daysPast);
//                sql.Clear();

//                sql.Append(@" SELECT C_Period_ID  
//                                   FROM C_period
//                                  WHERE to_date(startdate,'dd-MM-yyyy') BETWEEN to_date('"+from+@"','dd-MM-yyyy') AND to_date('"+PeriodEndDate+@"','dd-MM-yyyy')
//                                  or to_date(enddate,'dd-MM-yyyy') BETWEEN to_date('" + from + @"','dd-MM-yyyy') AND to_date('" + PeriodEndDate + @"','dd-MM-yyyy')");
//                ds=null;
//                ds=new DataSet();
//                ds=DB.ExecuteDataset(sql.ToString());
//                MPeriod period=null;
//                if (ds != null)
//                {
//                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
//                    {
//                        int periodID = Convert.ToInt32(ds.Tables[0].Rows[i][0]);
//                        period = new MPeriod(GetCtx(), periodID, trx);
//                        if (!OpenPeriod(period, trx))
//                        {
//                            status = "PeriodNotOpened";
//                        }
//                    }
//                }
                
//            }

            //Open Pereiod for specified Future Days 
//            if (dayFuture > 0)
//            {
//                DateTime from=DateTime.Now;
//                if (isNextYear)
//                {
//                    from = new DateTime(DateTime.Now.Year + 1, MonthTo, DateTo);
//                }
//                else
//                {
//                    from = new DateTime(DateTime.Now.Year , MonthTo, DateTo);
//                }
//                DateTime PeriodEndDate = from.AddDays(dayFuture);
               
//                sql.Clear();

//                sql.Append(@" SELECT C_Period_ID  
//                                   FROM C_period
//                                  WHERE to_date(startdate,'dd-MM-yyyy') BETWEEN to_date('" + from + @"','dd-MM-yyyy') AND to_date('" + PeriodEndDate + @"','dd-MM-yyyy')
//                                  or to_date(enddate,'dd-MM-yyyy') BETWEEN to_date('" + from + @"','dd-MM-yyyy') AND to_date('" + PeriodEndDate + @"','dd-MM-yyyy')");
//                ds = null;
//                ds = new DataSet();
//                ds = DB.ExecuteDataset(sql.ToString());
//                MPeriod period = null;
//                if (ds != null)
//                {
//                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
//                    {
//                        int periodID = Convert.ToInt32(ds.Tables[0].Rows[i][0]);
//                        period = new MPeriod(GetCtx(), periodID, trx);
//                        if (!OpenPeriod(period, trx))
//                        {
//                            status = "PeriodNotOpened";
//                        }
//                    }
//                }

//            }


            if (status == "OK")
            {
                trx.Commit();

            }
            else
            {
                trx.Rollback();
            }
            trx.Close();
            return status;
        }
        private bool OpenPeriod(MPeriod period,Trx trx)
        {
            if (period.Get_ID() == 0)
            {
                return false;
            }
            //throw new Exception("@NotFound@  @C_Period_ID@=" + _C_Period_ID);
           StringBuilder  sql=new StringBuilder();
            sql = new StringBuilder("UPDATE C_PeriodControl ");
            sql.Append("SET PeriodStatus='");
            //	Open                          
            sql.Append(MPeriodControl.PERIODSTATUS_Open);

            sql.Append("', PeriodAction='N', Updated=SysDate,UpdatedBy=").Append(GetAD_User_ID());
            //	WHERE
            sql.Append(" WHERE C_Period_ID=").Append(period.GetC_Period_ID());

            int no = DB.ExecuteQuery(sql.ToString(), null, trx);
            if (no == -1)
                return false;
            else
                return true;
        }

    }
}
