using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MMeasureCalc : X_PA_MeasureCalc
    {
        /**
         * 	Get MMeasureCalc from Cache
         *	@param ctx Ctx
         *	@param PA_MeasureCalc_ID id
         *	@return MMeasureCalc
         */
        public static MMeasureCalc Get(Ctx ctx, int PA_MeasureCalc_ID)
        {
            int key = PA_MeasureCalc_ID;
            MMeasureCalc retValue = (MMeasureCalc)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MMeasureCalc(ctx, PA_MeasureCalc_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**	Cache						*/
        private static CCache<int, MMeasureCalc> _cache
            = new CCache<int, MMeasureCalc>("PA_MeasureCalc", 10);

        /**
         * 	Standard Constructor
         *	@param ctx Ctx
         *	@param PA_MeasureCalc_ID id
         *	@param trxName trx
         */
        public MMeasureCalc(Ctx ctx, int PA_MeasureCalc_ID, Trx trxName) :
            base(ctx, PA_MeasureCalc_ID, trxName)
        {

        }

        /**
         * 	Load Constructor
         *	@param ctx Ctx
         *	@param rs result set
         *	@param trxName trx
         */
        public MMeasureCalc(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }


        /**
         * 	Get Sql to return single value for the Performance Indicator
         *	@param restrictions array of goal restrictions
         *	@param MeasureScope scope of this value  
         *	@param MeasureDataType data type
         *	@param reportDate optional report date
         *	@param role role
         *	@return sql for performance indicator
         */
        public String GetSqlPI(MGoalRestriction[] restrictions,
            String MeasureScope, String MeasureDataType, DateTime? reportDate, MRole role)
        {
            StringBuilder sb = new StringBuilder(GetSelectClause())
                .Append(" ")
                .Append(GetWhereClause());
            //	Date Restriction
            if (GetDateColumn() != null
                && MMeasure.MEASUREDATATYPE_QtyAmountInTime.Equals(MeasureDataType)
                && !MGoal.MEASUREDISPLAY_Total.Equals(MeasureScope))
            {
                if (reportDate == null)
                    reportDate = Convert.ToDateTime(DateTime.Now);
                String dateString = DataBase.DB.TO_DATE((DateTime?)reportDate);
                // http://download-west.oracle.com/docs/cd/B14117_01/server.101/b10759/functions207.htm#i1002084
                String trunc = "DD";
                if (MGoal.MEASUREDISPLAY_Year.Equals(MeasureScope))
                    trunc = "Y";
                else if (MGoal.MEASUREDISPLAY_Quarter.Equals(MeasureScope))
                    trunc = "Q";
                else if (MGoal.MEASUREDISPLAY_Month.Equals(MeasureScope))
                    trunc = "MM";
                else if (MGoal.MEASUREDISPLAY_Week.Equals(MeasureScope))
                    trunc = "D";
                //	else if (MGoal.MEASUREDISPLAY_Day.Equals(MeasureDisplay))
                //		;
                sb.Append(" AND TRUNC(")
                    .Append(GetDateColumn()).Append(",'").Append(trunc).Append("')=TRUNC(")
                    .Append(DataBase.DB.TO_DATE((DateTime?)reportDate)).Append(",'").Append(trunc).Append("')");
            }	//	date
            String sql = AddRestrictions(sb.ToString(), restrictions, role);

            log.Fine(sql);
            return sql;
        }

        /**
         * 	Get Sql to value for the bar chart
         *	@param restrictions array of goal restrictions
         *	@param MeasureDisplay scope of this value  
         *	@param startDate optional report start date
         *	@param role role
         *	@return sql for Bar Chart
         */
        public String GetSqlBarChart(MGoalRestriction[] restrictions,
            String measureDisplay, DateTime? startDate, MRole role)
        {
            StringBuilder sb = new StringBuilder();
            String dateCol = null;
            String groupBy = null;
            if (GetDateColumn() != null
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
                //	else if (MGoal.MEASUREDISPLAY_Day.Equals(MeasureDisplay))
                //		;
                dateCol = "TRUNC(" + GetDateColumn() + ",'" + trunc + "') ";
                groupBy = dateCol;
            }
            else
                dateCol = "MAX(" + GetDateColumn() + ") ";
            //
            String selectFrom = GetSelectClause();
            int index = selectFrom.IndexOf("FROM ");
            if (index == -1)
                index = selectFrom.ToUpper().IndexOf("FROM ");
            if (index == -1)
                throw new ArgumentException("Cannot find FROM in sql - " + selectFrom);
            sb.Append(selectFrom.Substring(0, index))
                .Append(",").Append(dateCol)
                .Append(selectFrom.Substring(index));

            //	** WHERE
            sb.Append(" ")
                .Append(GetWhereClause());
            //	Date Restriction
            if (GetDateColumn() != null
                && startDate != null
                && !MGoal.MEASUREDISPLAY_Total.Equals(measureDisplay))
            {
                String dateString = DataBase.DB.TO_DATE((DateTime?)startDate);
                sb.Append(" AND ").Append(GetDateColumn())
                    .Append(">=").Append(dateString);
            }	//	date
            String sql = AddRestrictions(sb.ToString(), restrictions, role);
            if (groupBy != null)
                sql += " GROUP BY " + groupBy;
            //
            log.Fine(sql);
            return sql;
        }

        /**
         * 	Get Zoom Query
         * 	@param restrictions restrictions
         * 	@param MeasureDisplay display
         * 	@param date date
         * 	@param role role
         *	@return query
         */
        public Query GetQuery(MGoalRestriction[] restrictions,
            String measureDisplay, DateTime? date, MRole role)
        {
            Query query = new Query(GetAD_Table_ID().ToString());
            //
            StringBuilder sql = new StringBuilder("SELECT ").Append(GetKeyColumn()).Append(" ");
            String from = GetSelectClause();
            int index = from.IndexOf("FROM ");
            if (index == -1)
                throw new ArgumentException("Cannot find FROM " + from);
            sql.Append(from.Substring(index)).Append(" ")
                .Append(GetWhereClause());
            //	Date Range
            if (GetDateColumn() != null
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
                //	else if (MGoal.MEASUREDISPLAY_Day.Equals(MeasureDisplay))
                //		trunc = "D";
                sql.Append(" AND TRUNC(").Append(GetDateColumn()).Append(",'").Append(trunc)
                    .Append("')=TRUNC(").Append(DataBase.DB.TO_DATE(date)).Append(",'").Append(trunc).Append("')");
            }
            String finalSQL = AddRestrictions(sql.ToString(), restrictions, role);
            //	Execute
            StringBuilder where = new StringBuilder();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(finalSQL);
                while (idr.Read())
                {
                    int id = Utility.Util.GetValueOfInt(idr[0].ToString());
                    if (where.Length > 0)
                        where.Append(",");
                    where.Append(id);
                }
                idr.Close();

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, finalSQL, e);
            }

            if (where.Length == 0)
                return Query.GetNoRecordQuery(query.GetTableName(), false);
            //
            StringBuilder whereClause = new StringBuilder(GetKeyColumn())
                .Append(" IN (").Append(where).Append(")");
            query.AddRestriction(whereClause.ToString());
            query.SetRecordCount(1);
            return query;
        }

        /**
         * 	Add Restrictions
         *	@param sql existing sql
         *	@param restrictions restrictions
         *	@param role role
         *	@return updated sql
         */
        private String AddRestrictions(String sql, MGoalRestriction[] restrictions, MRole role)
        {
            return AddRestrictions(sql, false, restrictions, role,
                GetTableName(), GetOrgColumn(), GetBPartnerColumn(), GetProductColumn(), GetCtx());
        }

        /**
         * 	Add Restrictions to SQL
         *	@param sql orig sql
         *	@param queryOnly incomplete sql for query restriction
         *	@param restrictions restrictions
         *	@param role role
         *	@param tableName table name
         *	@param orgColumn org column
         *	@param bpColumn bpartner column
         *	@param pColumn product column
         *	@return updated sql
         */
        public static String AddRestrictions(String sql, Boolean queryOnly,
            MGoalRestriction[] restrictions, MRole role,
            String tableName, String orgColumn, String bpColumn, String pColumn, Ctx ctx)
        {
            StringBuilder sb = new StringBuilder(sql);
            //	Org Restrictions
            if (orgColumn != null)
            {
                List<int> list = new List<int>();
                for (int i = 0; i < restrictions.Length; i++)
                {
                    if (MGoalRestriction.GOALRESTRICTIONTYPE_Organization.Equals(restrictions[i].GetGoalRestrictionType()))
                        list.Add(restrictions[i].GetOrg_ID());
                    //	Hierarchy comes here
                }
                if (list.Count == 1)
                    sb.Append(" AND ").Append(orgColumn)
                        .Append("=").Append(list[0]);
                else if (list.Count > 1)
                {
                    sb.Append(" AND ").Append(orgColumn).Append(" IN (");
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(list[i]);
                    }
                    sb.Append(")");
                }
            }	//	org

            //	BPartner Restrictions
            if (bpColumn != null)
            {

                List<int> listBP = new List<int>();
                List<int> listBPG = new List<int>();
                for (int i = 0; i < restrictions.Length; i++)
                {
                    if (MGoalRestriction.GOALRESTRICTIONTYPE_BusinessPartner.Equals(restrictions[i].GetGoalRestrictionType()))
                        listBP.Add(restrictions[i].GetC_BPartner_ID());
                    //	Hierarchy comes here
                    if (MGoalRestriction.GOALRESTRICTIONTYPE_BusPartnerGroup.Equals(restrictions[i].GetGoalRestrictionType()))
                        listBPG.Add(restrictions[i].GetC_BP_Group_ID());
                }
                //	BP
                if (listBP.Count == 1)
                    sb.Append(" AND ").Append(bpColumn)
                        .Append("=").Append(listBP[0]);
                else if (listBP.Count > 1)
                {
                    sb.Append(" AND ").Append(bpColumn).Append(" IN (");
                    for (int i = 0; i < listBP.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listBP[i]);
                    }
                    sb.Append(")");
                }
                //	BPG
                if (bpColumn.IndexOf(".") == -1)
                    bpColumn = tableName + "." + bpColumn;
                if (listBPG.Count == 1)
                    sb.Append(" AND EXISTS (SELECT * FROM C_BPartner bpx WHERE ")
                        .Append(bpColumn)
                        .Append("=bpx.C_BPartner_ID AND bpx.C_BP_GROUP_ID=")
                        .Append(listBPG[0]).Append(")");
                else if (listBPG.Count > 1)
                {
                    sb.Append(" AND EXISTS (SELECT * FROM C_BPartner bpx WHERE ")
                        .Append(bpColumn)
                        .Append("=bpx.C_BPartner_ID AND bpx.C_BP_GROUP_ID IN (");
                    for (int i = 0; i < listBPG.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listBPG[i]);
                    }
                    sb.Append("))");
                }
            }	//	bp

            //	Product Restrictions
            if (pColumn != null)
            {
                List<int> listP = new List<int>();
                List<int> listPC = new List<int>();
                for (int i = 0; i < restrictions.Length; i++)
                {
                    if (MGoalRestriction.GOALRESTRICTIONTYPE_Product.Equals(restrictions[i].GetGoalRestrictionType()))
                        listP.Add(restrictions[i].GetM_Product_ID());
                    //	Hierarchy comes here
                    if (MGoalRestriction.GOALRESTRICTIONTYPE_ProductCategory.Equals(restrictions[i].GetGoalRestrictionType()))
                        listPC.Add(restrictions[i].GetM_Product_Category_ID());
                }
                //	Product
                if (listP.Count == 1)
                    sb.Append(" AND ").Append(pColumn)
                        .Append("=").Append(listP[0]);
                else if (listP.Count > 1)
                {
                    sb.Append(" AND ").Append(pColumn).Append(" IN (");
                    for (int i = 0; i < listP.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listP[i]);
                    }
                    sb.Append(")");
                }
                //	Category
                if (pColumn.IndexOf(".") == -1)
                    pColumn = tableName + "." + pColumn;
                if (listPC.Count == 1)
                    sb.Append(" AND EXISTS (SELECT * FROM M_Product px WHERE ")
                        .Append(pColumn)
                        .Append("=px.M_Product_ID AND px.M_Product_Category_ID=")
                        .Append(listPC[0]).Append(")");
                else if (listPC.Count > 1)
                {
                    sb.Append(" AND EXISTS (SELECT * FROM M_Product px WHERE ")
                    .Append(pColumn)
                    .Append("=px.M_Product_ID AND px.M_Product_Category_ID IN (");
                    for (int i = 0; i < listPC.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(",");
                        sb.Append(listPC[i]);
                    }
                    sb.Append("))");
                }
            }	//	product
            String finalSQL = sb.ToString();
            if (queryOnly)
                return finalSQL;
            if (role == null)
                role = MRole.GetDefault(ctx);
            String retValue = role.AddAccessSQL(finalSQL, tableName, true, false);
            return retValue;
        }

        /**
         * 	Get Table Name
         *	@return Table Name
         */
        public String GetTableName()
        {
            return MTable.GetTableName(GetCtx(), GetAD_Table_ID());
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MMeasureCalc[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }

    }
}
