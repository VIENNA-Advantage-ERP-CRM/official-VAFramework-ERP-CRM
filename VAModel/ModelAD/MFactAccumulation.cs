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
    //Accounting Balances Accumulation Model
    public class MFactAccumulation : X_Fact_Accumulation
    {
        #region Private Variables
        /** Logger for class MFactAccumulation */
        private static VLogger s_log = VLogger.GetVLogger(typeof(MFactAccumulation).FullName);
        /**	Logger	*/
        //private static long serialVersionUID = 4694907481460935528L;
        #endregion

        /**
     * 	Get All for Tenant
     *	@param ctx context
     *	@param C_AcctSchema_ID optional acct schema
     *	@return list of Accumulation Rules ordered by AcctSchema and DateTo
     */
        public static List<MFactAccumulation> GetAll(Ctx ctx, int C_AcctSchema_ID)
        {
            StringBuilder sql = new StringBuilder("SELECT * FROM Fact_Accumulation "
                + "WHERE IsActive='Y' AND AD_Client_ID=" + ctx.GetAD_Client_ID());
            if (C_AcctSchema_ID > 0)
            {
                sql.Append("AND C_AcctSchema_ID= " + C_AcctSchema_ID);
            }
            sql.Append("ORDER BY C_AcctSchema_ID ");

            List<MFactAccumulation> list = new List<MFactAccumulation>();
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MFactAccumulation(ctx, dr, null));
                }
            }
            catch (Exception e)
            {
               s_log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return list;
        }

        /**
         * 	Get Date From.
         * 	Assumes that closed periods are not changed
         *	@param accums accumulations with same acct schema ordered by dateTo
         *	@param dateFrom original
         *	@return dateFrom
         */
        public static DateTime? GetDateFrom(MFactAccumulation accum, DateTime? dateFrom)
        {
            if (accum == null || dateFrom == null)
                return dateFrom;
            //
            DateTime? earliestOK = null;// new DateTime(0L);
            if (dateFrom != null)
                earliestOK = dateFrom;

            earliestOK = accum.GetDateFrom(dateFrom);	//	fix start date;

            if (dateFrom != null && !dateFrom.Equals(earliestOK))
                s_log.Info("Changed from " + dateFrom + " to " + earliestOK);
            return earliestOK;
        }

        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param Fact_Accumulation_ID id
         *	@param trx p_trx
         */
        public MFactAccumulation(Ctx ctx, int Fact_Accumulation_ID, Trx trx)
            : base(ctx, Fact_Accumulation_ID, trx)
        {

        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trx p_trx
         */
        public MFactAccumulation(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        /**
         * 	Get the first Date From date based on Accumulation
         *	@param from date
         *	@return  first date
         */
        public DateTime? GetDateFrom(DateTime? from)
        {
            if (from == null)
                return from;

            if (BALANCEACCUMULATION_CalendarMonth.Equals(GetBALANCEACCUMULATION()))
            {
                return TimeUtil.Trunc(from, TimeUtil.TRUNC_MONTH);
            }
            else if (BALANCEACCUMULATION_CalendarWeek.Equals(GetBALANCEACCUMULATION()))
            {
                return TimeUtil.Trunc(from, TimeUtil.TRUNC_WEEK);
            }
            else if (BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(GetBALANCEACCUMULATION())
                && GetC_Calendar_ID() != 0)
            {
                MPeriod period = MPeriod.GetOfCalendar(GetCtx(), GetC_Calendar_ID(), from);
                if (period != null)
                {
                    return period.GetStartDate();
                }
            }
            return from;
        }

        /**
         * 	Get the first Date From date of the next period based on Accumulation
         *	@param from date
         *	@return  first date of next period
         */
        public DateTime? GetDateFromNext(DateTime? from)
        {
            if (from == null)
                return from;

            DateTime? retValue = from;
            if (BALANCEACCUMULATION_Daily.Equals(GetBALANCEACCUMULATION()))
                return null;
            if (BALANCEACCUMULATION_CalendarMonth.Equals(GetBALANCEACCUMULATION()))
            { 
                retValue = TimeUtil.AddMonths(from, 1);
                retValue = TimeUtil.Trunc(retValue, TimeUtil.TRUNC_MONTH);
            }
            else if (BALANCEACCUMULATION_CalendarWeek.Equals(GetBALANCEACCUMULATION()))
            {
                retValue = TimeUtil.AddDays(from, 7);
                retValue = TimeUtil.Trunc(retValue, TimeUtil.TRUNC_WEEK);
            }
            else if (BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(GetBALANCEACCUMULATION())
                && GetC_Calendar_ID() != 0)
            {

            }
            return retValue;
        }

        /**
         * 	Before Save - Check Calendar
         * 	@param newRecord new
         * 	@return true if it can be saved
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Calendar
            if (BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(GetBALANCEACCUMULATION()))
            {
                if (GetC_Calendar_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "C_Calendar_ID"));
                    return false;
                }
            }
            else if (GetC_Calendar_ID() != 0)
                SetC_Calendar_ID(0);

            if (IsDefault())
            {
                Boolean exists = false;
                String sql = "SELECT * FROM Fact_Accumulation "
                    + " WHERE IsDefault = 'Y'"
                    + " AND IsActive = 'Y' "
                    + " AND AD_Client_ID = " + GetAD_Client_ID()
                    + " AND Fact_Accumulation_ID <> " + GetFACT_ACCUMULATION_ID();
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr.Read())
                    {
                        exists = true;
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }
                if (exists)
                {
                    log.SaveError("DefaultExists", "Default Balance aggregation already exists");
                    return false;
                }
            }
            if (!newRecord)
            {
                if (Is_ValueChanged("C_AcctSchema_ID") ||
                        Is_ValueChanged("BalanceAccumulation") ||
                        Is_ValueChanged("C_Calendar_ID") ||
                        Is_ValueChanged("IsActivity") ||
                        Is_ValueChanged("IsBudget") ||
                        Is_ValueChanged("IsBusinessPartner") ||
                        Is_ValueChanged("IsCampaign") ||
                        Is_ValueChanged("IsLocationFrom") ||
                        Is_ValueChanged("IsLocationTo") ||
                        Is_ValueChanged("IsProduct") ||
                        Is_ValueChanged("IsProject") ||
                        Is_ValueChanged("IsSalesRegion") ||
                        Is_ValueChanged("IsUserList1") ||
                        Is_ValueChanged("IsUserList2"))
                {
                    Boolean exists = false;
                    String sql = "SELECT * FROM Fact_Acct_Balance "
                        + " WHERE Fact_Accumulation_ID = " + GetFACT_ACCUMULATION_ID();
                    IDataReader idr = null;
                    try
                    {
                        idr = DB.ExecuteReader(sql, null, Get_TrxName());
                        if (idr.Read())
                        {
                            exists = true;
                        }
                        idr.Close();
                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, sql, e);
                    }
                    finally
                    {
                        if (idr != null)
                        {
                            idr.Close();
                            idr = null;
                        }
                    }
                    if (exists)
                    {
                        log.SaveError("BalanceExists", "Updates not allowed when balances exists for the Aggregation");
                        return false;
                    }
                }
            }

            return true;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MFactAccumulation[")
                .Append(Get_ID())
                .Append(",BalanceAccumulation=").Append(GetBALANCEACCUMULATION());
            if (GetC_Calendar_ID() != 0)
                sb.Append(",C_Calendar_ID=").Append(GetC_Calendar_ID());
            sb.Append("]");
            return sb.ToString();
        }
    }
}
