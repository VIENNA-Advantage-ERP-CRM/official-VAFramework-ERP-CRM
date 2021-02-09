using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class PeriodCloseScheduler : SvrProcess
    {
        private String OrgId=null;
        private int day = 0;
        private int calendar_id = 0;
        DateTime date = DateTime.Today;


        /// <summary>
        /// prepare
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    OrgId = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (name.Equals("MonthDay"))
                {
                    day = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_Calendar_ID"))
                {
                    calendar_id = Util.GetValueOfInt(para[i].GetParameter());
                }
            }
        }

        /// <summary>
        /// Close the periods for all dcocument type except Gl Journal of previous month
        /// </summary>
        /// <returns>info</returns>
        protected override string DoIt()
        {
            int currentDay=date.Day;
            DateTime month = new DateTime(date.Year, date.Month, 1);
            DateTime lastMonth = month.AddMonths(-1);          
        
            //close period if scheduler date is a current date.here day is scheduled day
            if (currentDay == day)
            {                  
               string sql = "UPDATE C_PeriodControl SET PeriodStatus= '" + MPeriodControl.PERIODSTATUS_Closed + "'" +
                    " WHERE C_Period_ID= (SELECT C_period_ID FROM C_Period p INNER JOIN C_year y ON y.c_year_ID = p.C_year_ID " +
                    "WHERE y.C_Calendar_ID=" + calendar_id + " AND Startdate=" + GlobalVariable.TO_DATE(lastMonth, true)+ ") AND DocBaseType NOT IN ('" + MDocBaseType.DOCBASETYPE_GLJOURNAL + "')"+
                   " AND PeriodStatus<>'"+MPeriodControl.PERIODSTATUS_PermanentlyClosed+"' AND AD_Org_ID IN (" + OrgId + ")";

                sql = MRole.GetDefault(GetCtx()).AddAccessSQL(sql, "C_PeriodControl", true, false); // fully qualified - RO
                if ( Util.GetValueOfInt( DB.ExecuteQuery(sql, null, null)) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "PeriodNotClosed");
                }
            }
            else
            {
                return Msg.GetMsg(GetCtx(), "NotScheduled");
            }
            return Msg.GetMsg(GetCtx(), "PeriodClosed");
        }
    }
}
