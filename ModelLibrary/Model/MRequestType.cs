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
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MRequestType : X_R_RequestType
    {
        /**
         * 	Get Request Type (cached)
         *	@param ctx context
         *	@param R_RequestType_ID id
         *	@return Request Type
         */
        public static MRequestType Get(Ctx ctx, int R_RequestType_ID)
        {
            int key = R_RequestType_ID;
            MRequestType retValue = (MRequestType)_cache[key];
            if (retValue == null)
            {
                retValue = new MRequestType(ctx, R_RequestType_ID, null);
                _cache.Add(key, retValue);
            }
            return retValue;
        }

        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MRequestType).FullName);
        /**	Cache							*/
        static private CCache<int, MRequestType> _cache = new CCache<int, MRequestType>("R_RequestType", 10);

        /**
         * 	Get Default Request Type
         *	@param ctx context
         *	@return Request Type
         */
        public static MRequestType GetDefault(Ctx ctx)
        {
            MRequestType retValue = null;
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM R_RequestType "
                + "WHERE AD_Client_ID IN (0,11) AND IsActive='Y'"
                + "ORDER BY IsDefault DESC, AD_Client_ID DESC, R_Request_ID DESC";
            DataSet ds;
            try
            {
                ds = new DataSet();
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    retValue = new MRequestType(ctx, dr, null);
                    if (!retValue.IsDefault())
                        retValue = null;
                    break;
                }
                ds = null;
            }
            catch (SqlException ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                ds = null;
            }
            return retValue;
        }


        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param R_RequestType_ID id
         *	@param trxName transaction
         */
        public MRequestType(Ctx ctx, int R_RequestType_ID, Trx trxName) :
            base(ctx, R_RequestType_ID, trxName)
        {
            if (R_RequestType_ID == 0)
            {
                //	SetR_RequestType_ID (0);
                //	SetName (null);
                SetDueDateTolerance(7);
                SetIsDefault(false);
                SetIsEMailWhenDue(false);
                SetIsEMailWhenOverdue(false);
                SetIsSelfService(true);	// Y
                SetAutoDueDateDays(0);
                SetConfidentialType(CONFIDENTIALTYPE_PublicInformation);
                SetIsAutoChangeRequest(false);
                SetIsConfidentialInfo(false);
                SetIsIndexed(true);
                SetIsInvoiced(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result Set
         *	@param trxName transaction
         */
        public MRequestType(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /** Next time stats to be created		*/
        private long _nextStats = 0;

        private int _openNo = 0;
        private int _totalNo = 0;
        private int _new30No = 0;
        private int _closed30No = 0;

        /**
         * 	Update Statistics
         */
        //synchronized
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateStatistics()
        {

            if (CommonFunctions.CurrentTimeMillis() < _nextStats)
                return;

            String sql = "SELECT "
                + "(SELECT COUNT(*) FROM R_Request r"
                + " INNER JOIN R_Status s ON (r.R_Status_ID=s.R_Status_ID AND s.IsOpen='Y') "
                + "WHERE r.R_RequestType_ID=x.R_RequestType_ID) AS OpenNo, "
                + "(SELECT COUNT(*) FROM R_Request r "
                + "WHERE r.R_RequestType_ID=x.R_RequestType_ID) AS TotalNo, "
                + "(SELECT COUNT(*) FROM R_Request r "
                //jz + "WHERE r.R_RequestType_ID=x.R_RequestType_ID AND Created>SysDate-30) AS New30No, "
                + "WHERE r.R_RequestType_ID=x.R_RequestType_ID AND Created>addDays(SysDate,-30)) AS New30No, "
                + "(SELECT COUNT(*) FROM R_Request r"
                + " INNER JOIN R_Status s ON (r.R_Status_ID=s.R_Status_ID AND s.IsClosed='Y') "
                //jz + "WHERE r.R_RequestType_ID=x.R_RequestType_ID AND r.Updated>SysDate-30) AS Closed30No "
                + "WHERE r.R_RequestType_ID=x.R_RequestType_ID AND r.Updated>addDays(SysDate,-30)) AS Closed30No "
                //
                + "FROM R_RequestType x WHERE R_RequestType_ID=" + GetR_RequestType_ID();
            
            IDataReader idr=null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    _openNo =  Utility.Util.GetValueOfInt(idr[0]);
                    _totalNo = Utility.Util.GetValueOfInt(idr[1]);
                    _new30No = Utility.Util.GetValueOfInt(idr[2]);
                    _closed30No = Utility.Util.GetValueOfInt(idr[3]);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log (Level.SEVERE, sql, e);
            }
            
            _nextStats = CommonFunctions.CurrentTimeMillis() + 3600000;		//	every hour
        }

        /**
         * 	Get Total No of requests of type
         *	@return no
         */
        public int GetTotalNo()
        {
            UpdateStatistics();
            return _totalNo;
        }

        /**
         * 	Get Open No of requests of type
         *	@return no
         */
        public int GetOpenNo()
        {
            UpdateStatistics();
            return _openNo;
        }

        /**
         * 	Get Closed in last 30 days of type
         *	@return no
         */
        public int GetClosed30No()
        {
            UpdateStatistics();
            return _closed30No;
        }

        /**
         * 	Get New in the last 30 days of type
         *	@return no
         */
        public int GetNew30No()
        {
            UpdateStatistics();
            return _new30No;
        }

        /**
         * 	Get Requests of Type
         *	@param selfService self service
         *	@param C_BPartner_ID id or 0 for public
         *	@return array of requests
         */
        public MRequest[] GetRequests(Boolean selfService, int C_BPartner_ID)
        {
            String sql = "SELECT * FROM R_Request WHERE R_RequestType_ID=" + GetR_RequestType_ID();
            if (selfService)
                sql += " AND IsSelfService='Y'";
            if (C_BPartner_ID == 0)
                sql += " AND ConfidentialType='A'";
            else
                sql += " AND (ConfidentialType='A' OR C_BPartner_ID=" + C_BPartner_ID + ")";
            sql += " ORDER BY DocumentNo DESC";
            //
            List<MRequest> list = new List<MRequest>();
            DataSet ds;
            try
            {
                ds = new DataSet();
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MRequest(GetCtx(), dr, null));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log (Level.SEVERE, sql, e);
            }
            finally { ds = null; }

            MRequest[] retValue = new MRequest[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get public Requests of Type
         *	@return array of requests
         */
        public MRequest[] GetRequests()
        {
            return GetRequests(true, 0);
        }

        /**
         * 	Get Default R_Status_ID for Type
         *	@return status or 0
         */
        public int GetDefaultR_Status_ID()
        {
            if (GetR_StatusCategory_ID() == 0)
            {
                MStatusCategory sc = MStatusCategory.GetDefault(GetCtx());
                if (sc == null)
                    sc = MStatusCategory.CreateDefault(GetCtx());
                if (sc != null && sc.GetR_StatusCategory_ID() != 0)
                    SetR_StatusCategory_ID(sc.GetR_StatusCategory_ID());
            }
            if (GetR_StatusCategory_ID() != 0)
            {
                MStatusCategory sc = MStatusCategory.Get(GetCtx(), GetR_StatusCategory_ID());
                return sc.GetDefaultR_Status_ID();
            }
            return 0;
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (GetR_StatusCategory_ID() == 0)
            {
                MStatusCategory sc = MStatusCategory.GetDefault(GetCtx());
                if (sc != null && sc.GetR_StatusCategory_ID() != 0)
                    SetR_StatusCategory_ID(sc.GetR_StatusCategory_ID());
            }
            return true;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRequestType[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append("]");
            return sb.ToString();
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
        public String GetSqlPI(MGoalRestriction[] restrictions, String measureScope, 
            String measureDataType, DateTime? reportDate, MRole role)
        {
            String dateColumn = "Created";
            String orgColumn = "AD_Org_ID";
            String bpColumn = "C_BPartner_ID";
            String pColumn = "M_Product_ID";
            //	PlannedAmt -> PlannedQty -> Count
            StringBuilder sb = new StringBuilder("SELECT COUNT(*) "
                + "FROM R_Request WHERE R_RequestType_ID=" + GetR_RequestType_ID()
                + " AND Processed<>'Y'");
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
                //	else if (MGoal.MEASUREDISPLAY_Day.equals(measureDisplay))
                //		;
                sb.Append(" AND TRUNC(")
                    .Append(dateColumn).Append(",'").Append(trunc).Append("')=TRUNC(")
                    .Append(DataBase.DB.TO_DATE((DateTime?)reportDate)).Append(",'").Append(trunc).Append("')");
            }	//	date
            //
            String sql = MMeasureCalc.AddRestrictions(sb.ToString(), false, restrictions, role,
                "R_Request", orgColumn, bpColumn, pColumn,GetCtx());

            log.Fine(sql);
            return sql;
        }

        /**
         * 	Get Sql to value for the bar chart
         *	@param restrictions array of goal restrictions
         *	@param MeasureDisplay scope of this value  
         *	@param MeasureDataType data type
         *	@param startDate optional report start date
         *	@param role role
         *	@return sql for Bar Chart
         */
        public String GetSqlBarChart(MGoalRestriction[] restrictions, String measureDisplay, 
            String measureDataType, DateTime? startDate, MRole role)
        {
            String dateColumn = "Created";
            String orgColumn = "AD_Org_ID";
            String bpColumn = "C_BPartner_ID";
            String pColumn = "M_Product_ID";
            //
            StringBuilder sb = new StringBuilder("SELECT COUNT(*), ");
            String groupBy = null;
            String orderBy = null;
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
                //	else if (MGoal.MEASUREDISPLAY_Day.equals(MeasureDisplay))
                //		;
                orderBy = "TRUNC(" + dateColumn + ",'" + trunc + "')";
                //jz 0 is column position in EDB, Oracle doesn't take alias in group by
                //			groupBy = orderBy + ", 0 ";
                //			sb.append(groupBy)
                groupBy = orderBy + ", CAST(0 AS INTEGER) ";
                sb.Append(groupBy)
                    .Append("FROM R_Request ");
            }
            else
            {
                orderBy = "s.SeqNo";
                groupBy = "COALESCE(s.Name,TO_NCHAR('-')), s.R_Status_ID, s.SeqNo ";
                sb.Append(groupBy)
                    .Append("FROM R_Request LEFT OUTER JOIN R_Status s ON (R_Request.R_Status_ID=s.R_Status_ID) ");
            }
            //	Where
            sb.Append("WHERE R_Request.R_RequestType_ID=").Append(GetR_RequestType_ID())
                .Append(" AND R_Request.Processed<>'Y'");
            //	Date Restriction
            if (startDate != null
                && !MGoal.MEASUREDISPLAY_Total.Equals(measureDisplay))
            {
                String dateString = DataBase.DB.TO_DATE((DateTime?)startDate);
                sb.Append(" AND ").Append(dateColumn)
                    .Append(">=").Append(dateString);
            }	//	date
            //
            String sql = MMeasureCalc.AddRestrictions(sb.ToString(), false, restrictions, role,
                "R_Request", orgColumn, bpColumn, pColumn,GetCtx());
            if (groupBy != null)
                sql += " GROUP BY " + groupBy + " ORDER BY " + orderBy;
            //
            log.Fine(sql);
            return sql;
        }

        /**
         * 	Get Zoom Query
         * 	@param restrictions array of restrictions
         * 	@param MeasureDisplay display
         * 	@param date date
         * 	@param R_Status_ID status
         * 	@param role role
         *	@return query
         */
        public Query GetQuery(MGoalRestriction[] restrictions, String measureDisplay, 
            DateTime? date, int R_Status_ID, MRole role)
        {
            String dateColumn = "Created";
            String orgColumn = "AD_Org_ID";
            String bpColumn = "C_BPartner_ID";
            String pColumn = "M_Product_ID";
            //
            Query query = new Query("R_Request");
            query.AddRestriction("R_RequestType_ID", "=", GetR_RequestType_ID());
            //
            String where = null;
            if (R_Status_ID != 0)
                where = "R_Status_ID=" + R_Status_ID;
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
                //	else if (MGoal.MEASUREDISPLAY_Day.equals(MeasureDisplay))
                //		trunc = "D";
                where = "TRUNC(" + dateColumn + ",'" + trunc
                    + "')=TRUNC(" + DataBase.DB.TO_DATE(date) + ",'" + trunc + "')";
            }
            String whereRestriction = MMeasureCalc.AddRestrictions(where + " AND Processed<>'Y' ",
                true, restrictions, role,
                "R_Request", orgColumn, bpColumn, pColumn,GetCtx());
            query.AddRestriction(whereRestriction);
            query.SetRecordCount(1);
            return query;
        }
    }
}
