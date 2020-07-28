/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_ProjectType
 * Chronological Development
 * Veena Pandey     19-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProjectType : X_C_ProjectType
    {
        /**	Cache						*/
        private static CCache<int, MProjectType> _cache
            = new CCache<int, MProjectType>("C_ProjectType", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ProjectType_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MProjectType(Ctx ctx, int C_ProjectType_ID, Trx trxName)
            : base(ctx, C_ProjectType_ID, trxName)
        {
            /**
            if (C_ProjectType_ID == 0)
            {
                setC_ProjectType_ID (0);
                setName (null);
            }
            **/
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MProjectType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get MProjectType from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ProjectType_ID">id</param>
        /// <returns>MProjectType</returns>
        public static MProjectType Get(Ctx ctx, int C_ProjectType_ID)
        {
            int key = C_ProjectType_ID;
            MProjectType retValue = (MProjectType)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MProjectType(ctx, C_ProjectType_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Project Type Phases
        /// </summary>
        /// <returns>Array of phases</returns>
        public MProjectTypePhase[] GetPhases()
        {
            List<MProjectTypePhase> list = new List<MProjectTypePhase>();
            String sql = "SELECT * FROM C_Phase WHERE C_ProjectType_ID=" + GetC_ProjectType_ID() + " ORDER BY SeqNo";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MProjectTypePhase(GetCtx(), dr, Get_TrxName()));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
            }
            //
            MProjectTypePhase[] retValue = new MProjectTypePhase[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Zoom Query
        /// </summary>
        /// <param name="restrictions">restrictions</param>
        /// <param name="measureDisplay">display</param>
        /// <param name="date">date</param>
        /// <param name="C_Phase_ID">phase</param>
        /// <param name="role">role</param>
        /// <returns>query</returns>
        public Query GetQuery(MGoalRestriction[] restrictions,
            String measureDisplay, DateTime? date, int C_Phase_ID, MRole role)
        {
            String dateColumn = "Created";
            String orgColumn = "AD_Org_ID";
            String bpColumn = "C_BPartner_ID";
            String pColumn = null;
            //
            Query query = new Query("C_Project");
            query.AddRangeRestriction("C_ProjectType_ID", "=", GetC_ProjectType_ID());
            //
            String where = null;
            if (C_Phase_ID != 0)
                where = "C_Phase_ID=" + C_Phase_ID;
            else
            {
                String trunc = "D";
                if (MGoal.MEASUREDISPLAY_Year.Equals(measureDisplay))
                    trunc = "Y";
                else if (MGoal.MEASUREDISPLAY_Quarter.Equals(measureDisplay))
                    trunc = "Q";
                else if (MGoal.MEASUREDISPLAY_Month.Equals(measureDisplay))
                    trunc = "MM";
                else if (MGoal.MEASUREDISPLAY_Week.Equals(measureDisplay))
                    trunc = "W";
                //	else if (MGoal.MEASUREDISPLAY_Day.equals(measureDisplay))
                //		trunc = "D";
                where = "TRUNC(" + dateColumn + ",'" + trunc
                    + "')=TRUNC(" + DataBase.DB.TO_DATE(date) + ",'" + trunc + "')";
            }
            String sql = MMeasureCalc.AddRestrictions(where + " AND Processed<>'Y' ",
                true, restrictions, role,
                "C_Project", orgColumn, bpColumn, pColumn,GetCtx());
            query.AddRestriction(sql);
            query.SetRecordCount(1);
            return query;
        }

        /// <summary>
        /// Get Sql to value for the bar chart
        /// </summary>
        /// <param name="restrictions">array of goal restrictions</param>
        /// <param name="measureDisplay">scope of this value</param>
        /// <param name="measureDataType">data type</param>
        /// <param name="startDate">optional report start date</param>
        /// <param name="role">role</param>
        /// <returns>sql for Bar Chart</returns>
        public String GetSqlBarChart(MGoalRestriction[] restrictions, String measureDisplay,
            String measureDataType, DateTime? startDate, MRole role)
        {
            String dateColumn = "Created";
            String orgColumn = "AD_Org_ID";
            String bpColumn = "C_BPartner_ID";
            String pColumn = null;
            //
            StringBuilder sb = new StringBuilder("SELECT COALESCE(SUM(PlannedAmt),COALESCE(SUM(PlannedQty),COUNT(*))), ");
            String orderBy = null;
            String groupBy = null;
            //
            if (MMeasure.MEASUREDATATYPE_QtyAmountInTime.Equals(measureDataType)
                && !MGoal.MEASUREDISPLAY_Total.Equals(measureDisplay))
            {
                String trunc = "D";
                if (MGoal.MEASUREDISPLAY_Year.Equals(measureDisplay))
                    trunc = "Y";
                else if (MGoal.MEASUREDISPLAY_Quarter.Equals(measureDisplay))
                    trunc = "Q";
                else if (MGoal.MEASUREDISPLAY_Month.Equals(measureDisplay))
                    trunc = "MM";
                else if (MGoal.MEASUREDISPLAY_Week.Equals(measureDisplay))
                    trunc = "W";
                //	else if (MGoal.MEASUREDISPLAY_Day.equals(measureDisplay))
                //		;
                orderBy = "TRUNC(" + dateColumn + ",'" + trunc + "')";
                groupBy = orderBy + ", 0 ";
                sb.Append(groupBy)
                    .Append("FROM C_Project ");
            }
            else
            {
                orderBy = "p.SeqNo";
                groupBy = "COALESCE(p.Name,TO_NCHAR('-')), p.C_Phase_ID, p.SeqNo ";
                sb.Append(groupBy)
                    .Append("FROM C_Project LEFT OUTER JOIN C_Phase p ON (C_Project.C_Phase_ID=p.C_Phase_ID) ");
            }
            //	Where
            sb.Append("WHERE C_Project.C_ProjectType_ID=").Append(GetC_ProjectType_ID())
                .Append(" AND C_Project.Processed<>'Y'");
            //	Date Restriction
            if (startDate != null
                && !MGoal.MEASUREDISPLAY_Total.Equals(measureDisplay))
            {
                String dateString = DataBase.DB.TO_DATE(startDate);
                sb.Append(" AND ").Append(dateColumn)
                    .Append(">=").Append(dateString);
            }	//	date
            //
            String sql = MMeasureCalc.AddRestrictions(sb.ToString(), false, restrictions, role,
                "C_Project", orgColumn, bpColumn, pColumn,GetCtx());
            if (groupBy != null)
                sql += " GROUP BY " + groupBy + " ORDER BY " + orderBy;
            //
           log.Fine(sql);
            return sql;
        }

        /// <summary>
        /// Get Sql to return single value for the Performance Indicator
        /// </summary>
        /// <param name="restrictions">array of goal restrictions</param>
        /// <param name="measureScope">scope of this value</param>
        /// <param name="measureDataType">data type</param>
        /// <param name="reportDate">optional report date</param>
        /// <param name="role">role</param>
        /// <returns>sql for performance indicator</returns>
        public String GetSqlPI(MGoalRestriction[] restrictions,
            String measureScope, String measureDataType, DateTime? reportDate, MRole role)
        {
            String dateColumn = "Created";
            String orgColumn = "AD_Org_ID";
            String bpColumn = "C_BPartner_ID";
            String pColumn = null;
            //	PlannedAmt -> PlannedQty -> Count
            StringBuilder sb = new StringBuilder("SELECT COALESCE(SUM(PlannedAmt),COALESCE(SUM(PlannedQty),COUNT(*))) "
                + "FROM C_Project WHERE C_ProjectType_ID=" + GetC_ProjectType_ID()
                + " AND Processed<>'Y')");
            //	Date Restriction

            if (MMeasure.MEASUREDATATYPE_QtyAmountInTime.Equals(measureDataType)
                && !MGoal.MEASUREDISPLAY_Total.Equals(measureScope))
            {
                if (reportDate == null)
                    reportDate = DateTime.Now;
                String dateString = DataBase.DB.TO_DATE((DateTime?)reportDate);
                String trunc = "D";
                if (MGoal.MEASUREDISPLAY_Year.Equals(measureScope))
                    trunc = "Y";
                else if (MGoal.MEASUREDISPLAY_Quarter.Equals(measureScope))
                    trunc = "Q";
                else if (MGoal.MEASUREDISPLAY_Month.Equals(measureScope))
                    trunc = "MM";
                else if (MGoal.MEASUREDISPLAY_Week.Equals(measureScope))
                    trunc = "W";
                //	else if (MGoal.MEASUREDISPLAY_Day.equals(MeasureDisplay))
                //		;
                sb.Append(" AND TRUNC(")
                    .Append(dateColumn).Append(",'").Append(trunc).Append("')=TRUNC(")
                    .Append(DataBase.DB.TO_DATE((DateTime?)reportDate)).Append(",'").Append(trunc).Append("')");
            }	//	date
            //
            String sql = MMeasureCalc.AddRestrictions(sb.ToString(), false, restrictions, role,
                "C_Project", orgColumn, bpColumn, pColumn,GetCtx());

            log.Fine(sql);
            return sql;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProjectType[")
                .Append(Get_ID())
                .Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
        }

    }
}