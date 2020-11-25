/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_Period
 * Chronological Development
 * Veena Pandey     07-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.SqlExec;
using VAdvantage.Process;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPeriod : X_C_Period
    {
        //	Cache
        private static CCache<int, MPeriod> cache = new CCache<int, MPeriod>("C_Period", 10);
        //	Logger
        private static VLogger _log = VLogger.GetVLogger(typeof(MPeriod).FullName);
        //private static CLogger s_log = CLogger.getCLogger (MPeriod.class);
        //	Period Controls
        private MPeriodControl[] _controls = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Period_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPeriod(Ctx ctx, int C_Period_ID, Trx trxName)
            : base(ctx, C_Period_ID, trxName)
        {
            if (C_Period_ID == 0)
            {
                //	setC_Period_ID (0);		//	PK
                //  setC_Year_ID (0);		//	Parent
                //  setName (null);
                //  setPeriodNo (0);
                //  setStartDate (new Timestamp(System.currentTimeMillis()));
                SetPeriodType(PERIODTYPE_StandardCalendarPeriod);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MPeriod(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent constructor
        /// </summary>
        /// <param name="year">year</param>
        /// <param name="periodNo">no</param>
        /// <param name="name">name</param>
        /// <param name="startDate">start</param>
        /// <param name="endDate">end</param>
        public MPeriod(MYear year, int periodNo, String name, DateTime? startDate, DateTime? endDate)
            : this(year.GetCtx(), 0, year.Get_TrxName())
        {
            SetClientOrg(year);
            SetC_Year_ID(year.GetC_Year_ID());
            SetPeriodNo(periodNo);
            SetName(name);
            SetStartDate(startDate);
            SetEndDate(endDate);
        }

        /// <summary>
        /// Get Period from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Period_ID">id</param>
        /// <returns>MPeriod</returns>
        public static MPeriod Get(Ctx ctx, int C_Period_ID)
        {
            int key = C_Period_ID;
            MPeriod retValue = (MPeriod)cache[key];
            if (retValue != null)
                return retValue;
            //
            retValue = new MPeriod(ctx, C_Period_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Find standard Period of DateAcct based on Client Calendar
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <returns>active Period or null</returns>
        public static MPeriod Get(Ctx ctx, DateTime? dateAcct)
        {
            //if (dateAcct == null)
            //    return null;
            ////	Search in Cache first
            //IEnumerator<MPeriod> it = cache.Values.GetEnumerator();
            //it.Reset();
            //while (it.MoveNext())
            //{
            //    MPeriod period = it.Current;
            //    if (period.GetAD_Client_ID() == ctx.GetAD_Client_ID() && period.IsStandardPeriod() && period.IsInPeriod((DateTime?)dateAcct))
            //        return period;
            //}

            ////	Get it from DB
            //MPeriod retValue = null;
            //int AD_Client_ID = ctx.GetAD_Client_ID();
            //String sql = "SELECT * "
            //    + "FROM C_Period "
            //    + "WHERE C_Year_ID IN "
            //        + "(SELECT C_Year_ID FROM C_Year WHERE IsActive = 'Y' AND C_Calendar_ID= "
            //            + "(SELECT C_Calendar_ID FROM AD_ClientInfo WHERE  IsActive = 'Y' AND AD_Client_ID=@clientid))"
            //    + " AND @dateAcc BETWEEN TRUNC(StartDate,'DD') AND TRUNC(EndDate,'DD')"
            //    + " AND IsActive='Y' AND PeriodType='S'";
            //try
            //{

            //    //DateTime? dt = ((DateTime?)dateAcct).Date;
            //    DateTime? dt = ((DateTime?)dateAcct).Value.Date;

            //    SqlParameter[] param = new SqlParameter[2];
            //    param[0] = new SqlParameter("@clientid", AD_Client_ID);
            //    param[1] = new SqlParameter("@dateAcc", TimeUtil.GetDay((DateTime?)dateAcct));

            //    DataSet ds = DataBase.DB.ExecuteDataset(sql, param, null);
            //    if (ds.Tables.Count > 0)
            //    {
            //        foreach (DataRow dr in ds.Tables[0].Rows)
            //        {
            //            MPeriod period = new MPeriod(ctx, dr, null);
            //            int key = period.GetC_Period_ID();
            //            cache.Add(key, period);
            //            if (period.IsStandardPeriod())
            //                retValue = period;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    _log.Log(Level.SEVERE, "dateAcct=" + dateAcct, e);
            //}
            //if (retValue == null)
            //{
            //    _log.Warning("No Standard Period for " + dateAcct + " (AD_Client_ID=" + AD_Client_ID + ")");
            //}
            //return retValue;

            return Get(ctx, dateAcct, 0);
        }

        /// <summary>
        /// Find standard Period of DateAcct based on Organization Calendar
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>active Period or null</returns>
        public static MPeriod Get(Ctx ctx, DateTime? dateAcct, int AD_Org_ID)
        {
            if (dateAcct == null)
                return null;
            //	Search in Cache first
            //IEnumerator<MPeriod> it = cache.Values.GetEnumerator();
            //it.Reset();
            //while (it.MoveNext())
            //{
            //    MPeriod period = it.Current;
            //    if (period.GetAD_Client_ID() == ctx.GetAD_Client_ID() && period.IsStandardPeriod() && period.IsInPeriod((DateTime?)dateAcct))
            //        return period;
            //}

            // Get Calender ID
            StringBuilder qry = new StringBuilder("");
            int Calender_ID = 0;
            int AD_Client_ID = ctx.GetAD_Client_ID();

            if (AD_Org_ID > 0)
            {
                MOrgInfo orgInfo = MOrgInfo.Get(ctx, AD_Org_ID, null);
                if (orgInfo.Get_ColumnIndex("C_Calendar_ID") >= 0)
                {
                    Calender_ID = orgInfo.GetC_Calendar_ID();
                }
            }

            if (Calender_ID == 0)
            {
                qry.Append("SELECT C_Calendar_ID FROM AD_ClientInfo WHERE  IsActive = 'Y' AND AD_Client_ID=" + AD_Client_ID);
                Calender_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry.ToString()));
            }

            if (Calender_ID == 0)
            {
                return null;
            }

            //	Get it from DB   
            MPeriod retValue = null;
            String sql = "SELECT * "
            + "FROM C_Period "
            + "WHERE C_Year_ID IN "
                + "(SELECT C_Year_ID FROM C_Year WHERE IsActive = 'Y' AND C_Calendar_ID= @calendarID)"
            + " AND @dateAcc BETWEEN TRUNC(StartDate,'DD') AND TRUNC(EndDate,'DD')"
            + " AND IsActive='Y' AND PeriodType='S'";
            try
            {

                //DateTime? dt = ((DateTime?)dateAcct).Date;
                DateTime? dt = ((DateTime?)dateAcct).Value.Date;

                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@calendarID", Calender_ID);
                param[1] = new SqlParameter("@dateAcc", TimeUtil.GetDay((DateTime?)dateAcct));

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MPeriod period = new MPeriod(ctx, dr, null);
                        int key = period.GetC_Period_ID();
                        //cache.Add(key, period);
                        if (period.IsStandardPeriod())
                            retValue = period;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "dateAcct=" + dateAcct, e);
            }
            if (retValue == null)
            {
                _log.Warning("No Standard Period for " + dateAcct + " (AD_Client_ID=" + AD_Client_ID + ")");
            }
            return retValue;
        }

       

        /// <summary>
        /// Find valid standard Period of DateAcct based on Client Calendar
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <returns>C_Period_ID or 0</returns>
        public static int GetC_Period_ID(Ctx ctx, DateTime? dateAcct)
        {
            //MPeriod period = Get(ctx, dateAcct);
            //if (period == null)
            //    return 0;
            //return period.GetC_Period_ID();

            return GetC_Period_ID(ctx, dateAcct, 0);
        }

        /// <summary>
        /// Find valid standard Period of DateAcct based on Organization Calendar
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>C_Period_ID or 0</returns>
        public static int GetC_Period_ID(Ctx ctx, DateTime? dateAcct, int AD_Org_ID)
        {
            MPeriod period = Get(ctx, dateAcct, AD_Org_ID);
            if (period == null)
                return 0;
            return period.GetC_Period_ID();
        }

    
        /// <summary>
        /// Find first Year Period of DateAcct based on Client Calendar
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <returns>active first Period</returns>
        public static MPeriod GetFirstInYear(Ctx ctx, DateTime? dateAcct)
        {
            //MPeriod retValue = null;
            //int AD_Client_ID = ctx.GetAD_Client_ID();
            //String sql = "SELECT * "
            //    + "FROM C_Period "
            //    + "WHERE C_Year_ID IN "
            //        + "(SELECT p.C_Year_ID "
            //        + "FROM AD_ClientInfo c"
            //        + " INNER JOIN C_Year y ON (c.C_Calendar_ID=y.C_Calendar_ID)"
            //        + " INNER JOIN C_Period p ON (y.C_Year_ID=p.C_Year_ID) "
            //        + "WHERE c.AD_Client_ID=@clientid"
            //        + "	AND @date BETWEEN StartDate AND EndDate)"
            //    + " AND IsActive='Y' AND PeriodType='S' "
            //    + "ORDER BY StartDate";
            //try
            //{
            //    SqlParameter[] param = new SqlParameter[2];
            //    param[0] = new SqlParameter("@clientid", AD_Client_ID);
            //    param[1] = new SqlParameter("@date", dateAcct);

            //    DataSet ds = DataBase.DB.ExecuteDataset(sql, param, null);
            //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //    {
            //        DataRow dr = ds.Tables[0].Rows[0];  //	first only
            //        retValue = new MPeriod(ctx, dr, null);
            //    }
            //}
            //catch (Exception e)
            //{
            //    _log.Log(Level.SEVERE, sql, e);
            //}
            //return retValue;

            return GetFirstInYear(ctx, dateAcct, 0);
        }

        /// <summary>
        /// Find first Year Period of DateAcct based on Organization Calendar
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <param name="AD_Org_ID">Organization</param>
        /// <returns>active first Period</returns>
        public static MPeriod GetFirstInYear(Ctx ctx, DateTime? dateAcct, int AD_Org_ID)
        {
            MPeriod retValue = null;

            // Get Calender ID
            string qry;
            int AD_Client_ID = ctx.GetAD_Client_ID();
            int Calender_ID = 0;

            if (AD_Org_ID > 0)
            {
                MOrgInfo orgInfo = MOrgInfo.Get(ctx, AD_Org_ID, null);
                if (orgInfo.Get_ColumnIndex("C_Calendar_ID") >= 0)
                {
                    Calender_ID = orgInfo.GetC_Calendar_ID();
                }
            }

            if (Calender_ID == 0)
            {
                qry = "SELECT C_Calendar_ID FROM AD_ClientInfo WHERE  IsActive = 'Y' AND AD_Client_ID=" + AD_Client_ID;
                Calender_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry.ToString()));
            }

            if (Calender_ID == 0)
            {
                return null;
            }

            String sql = "SELECT * "
                + "FROM C_Period "
                + "WHERE C_Year_ID IN "
                     + "(SELECT C_Year_ID FROM C_Year WHERE IsActive = 'Y' AND C_Calendar_ID= @calendarID)"
                    + "	AND @date BETWEEN StartDate AND EndDate)"
                + " AND IsActive='Y' AND PeriodType='S' "
                + "ORDER BY StartDate";
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@calendarID", Calender_ID);
                param[1] = new SqlParameter("@date", dateAcct);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, null);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0]; 	//	first only
                    retValue = new MPeriod(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get Period Control
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>period controls</returns>
        public MPeriodControl[] GetPeriodControls(bool requery)
        {
            //if (_controls != null && !requery)
            //    return _controls;
            ////
            //List<MPeriodControl> list = new List<MPeriodControl>();
            //String sql = "SELECT * FROM C_PeriodControl "
            //    + "WHERE C_Period_ID=" + GetC_Period_ID();
            //try
            //{
            //    DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
            //    if (ds.Tables.Count > 0)
            //    {
            //        foreach (DataRow dr in ds.Tables[0].Rows)
            //        {
            //            list.Add(new MPeriodControl(GetCtx(), dr, null));
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    log.Log(Level.SEVERE, sql, e);
            //}
            ////
            //_controls = new MPeriodControl[list.Count];
            //_controls = list.ToArray();
            //return _controls;

            return GetPeriodControls(requery, 0);
        }

        /// <summary>
        /// Get Period Control
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>period controls</returns>
        public MPeriodControl[] GetPeriodControls(bool requery, int AD_Org_ID)
        {
            if (_controls != null && !requery)
                return _controls;
            //
            List<MPeriodControl> list = new List<MPeriodControl>();
            String sql = "SELECT * FROM C_PeriodControl "
                + "WHERE IsActive = 'Y' AND C_Period_ID=" + GetC_Period_ID();

            if (AD_Org_ID > 0)
            {
                sql += " AND AD_Org_ID IN (0, " + AD_Org_ID + ") ORDER BY AD_Org_ID DESC";
            }

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MPeriodControl(GetCtx(), dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            _controls = new MPeriodControl[list.Count];
            _controls = list.ToArray();
            return _controls;
        }

        /// <summary>
        /// Get Period Control
        /// </summary>
        /// <param name="docBaseType">Document Base Type</param>
        /// <returns>period control or null</returns>
        public MPeriodControl GetPeriodControl(String docBaseType)
        {
            //if (docBaseType == null)
            //    return null;
            //GetPeriodControls(false);
            //for (int i = 0; i < _controls.Length; i++)
            //{
            //    //	log.fine("getPeriodControl - " + 1 + " - " + _controls[i]);
            //    if (docBaseType.Equals(_controls[i].GetDocBaseType()))
            //        return _controls[i];
            //}
            //return null;
            return GetPeriodControl(docBaseType, 0);
        }

        /// <summary>
        /// Get Period Control
        /// </summary>
        /// <param name="docBaseType">Document Base Type</param>
        /// <returns>period control or null</returns>
        public MPeriodControl GetPeriodControl(String docBaseType, int AD_Org_ID)
        {
            if (docBaseType == null)
                return null;
            GetPeriodControls(false, AD_Org_ID);
            for (int i = 0; i < _controls.Length; i++)
            {
                //	log.fine("getPeriodControl - " + 1 + " - " + _controls[i]);
                if (docBaseType.Equals(_controls[i].GetDocBaseType()))
                    return _controls[i];
            }
            return null;
        }

        /// <summary>
        /// Date In Period
        /// </summary>
        /// <param name="date">date</param>
        /// <returns>true if in period</returns>
        public bool IsInPeriod(DateTime? date)
        {
            if (date == null)
                return false;
            DateTime? dateOnly = TimeUtil.GetDay(date);
            DateTime? from = TimeUtil.GetDay(GetStartDate());
            if (dateOnly < from)
                return false;
            DateTime? to = TimeUtil.GetDay(GetEndDate());
            if (dateOnly > to)
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// Standard
        /// </summary>
        /// <returns>true if standard calendar period</returns>
        public bool IsStandardPeriod()
        {
            return PERIODTYPE_StandardCalendarPeriod.Equals(GetPeriodType());
        }

        /// <summary>
        /// Before Save. Truncate Dates
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Truncate Dates
            DateTime? date = GetStartDate();
            if (date != null)
            {
                SetStartDate(TimeUtil.GetDay(date));
            }
            else
                return false;
            //
            date = GetEndDate();
            if (date != null)
            {
                SetEndDate(TimeUtil.GetDay(date));
            }
            else
            {
                SetEndDate(TimeUtil.GetMonthLastDay(GetStartDate()));
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord)
            {
                //	SELECT Value FROM AD_Ref_List WHERE AD_Reference_ID=183
                MDocType[] types = MDocType.GetOfClient(GetCtx());
                int count = 0;
                List<String> baseTypes = new List<String>();
                for (int i = 0; i < types.Length; i++)
                {
                    MDocType type = types[i];
                    String docBaseType = type.GetDocBaseType();
                    if (baseTypes.Contains(docBaseType))
                        continue;
                    MPeriodControl pc = new MPeriodControl(this, docBaseType);
                    pc.SetAD_Org_ID(GetAD_Org_ID());
                    if (pc.Save())
                        count++;
                    baseTypes.Add(docBaseType);
                }
                log.Fine("PeriodControl #" + count);
            }
            return success;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPeriod[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(", ").Append(GetStartDate()).Append("-").Append(GetEndDate())
                .Append("]");
            return sb.ToString();
        }
        /// <summary>
        /// Find standard Period of DateAcct based on Client Calendar
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_Calendar_ID">calendar</param>
        /// <param name="DateAcct">DateAcct date</param>
        /// <returns> active Period or null</returns> 
        /// <date>07-March-2011</date>
        /// <writer>raghu</writer>
        public static MPeriod GetOfCalendar(Ctx ctx, int C_Calendar_ID, DateTime? DateAcct)
        {
            if (DateAcct == null)
            {
                _log.Warning("No DateAcct");
                return null;
            }
            if (C_Calendar_ID == 0)
            {
                _log.Warning("No Calendar");
                return null;
            }
            //	Search in Cache first
            IEnumerator<MPeriod> it = cache.Values.GetEnumerator();
            while (it.MoveNext())
            {
                MPeriod period = it.Current;
                if (period.GetC_Calendar_ID() == C_Calendar_ID
                        && period.IsStandardPeriod()
                        && period.IsInPeriod(DateAcct))
                    return period;
            }

            //	Get it from DB
            MPeriod retValue = null;
            // mohit 28-9-2015
            //String sql = "SELECT * FROM C_Period "
            //    + "WHERE C_Year_ID IN "
            //    + "(SELECT C_Year_ID FROM C_Year WHERE C_Calendar_ID=" + C_Calendar_ID + ")"
            //    + " AND '" + TimeUtil.GetDay(DateAcct) + "' BETWEEN TRUNC(StartDate,'DD') AND TRUNC(EndDate,'DD')"
            //    + " AND IsActive='Y' AND PeriodType='S'";
            String sql = "SELECT * FROM C_Period "
               + "WHERE C_Year_ID IN "
               + "(SELECT C_Year_ID FROM C_Year WHERE C_Calendar_ID=" + C_Calendar_ID + ")"
               + " AND " + GlobalVariable.TO_DATE(DateAcct, true) + " BETWEEN TRUNC(StartDate,'DD') AND TRUNC(EndDate,'DD')"
               + " AND IsActive='Y' AND PeriodType='S'";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MPeriod period = new MPeriod(ctx, dt.Rows[i], null);
                    int key = Util.GetValueOfInt(period.GetC_Period_ID());

                    cache[key] = period;

                    if (period.IsStandardPeriod())
                    {
                        retValue = period;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "DateAcct=" + DateAcct, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            if (retValue == null)
                _log.Warning("No Standard Period for " + DateAcct
                        + " (C_Calendar_ID=" + C_Calendar_ID + ")");
            return retValue;
        }
        /// <summary>
        /// Is standard Period Open for Document Base Type
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <param name="docBaseType">base type</param>
        /// <returns>true if open</returns>
        public static bool IsOpen(Ctx ctx, DateTime? dateAcct, String docBaseType)
        {
            //if (dateAcct == null)
            //{
            //    _log.Warning("No DateAcct");
            //    return false;
            //}
            //if (docBaseType == null)
            //{
            //    _log.Warning("No DocBaseType");
            //    return false;
            //}
            //MPeriod period = MPeriod.Get(ctx, dateAcct);
            //if (period == null)
            //{
            //    _log.Warning("No Period for " + dateAcct + " (" + docBaseType + ")");
            //    return false;
            //}
            //bool open = (period.IsOpen(docBaseType, dateAcct) == null);
            //if (!open)
            //{
            //    _log.Warning(period.GetName() + ": Not open for " + docBaseType + " (" + dateAcct + ")");
            //}
            //return open;

            return IsOpen(ctx, dateAcct, docBaseType, 0);
        }

        /// <summary>
        /// Is standard Period Open for Document Base Type
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dateAcct">date</param>
        /// <param name="docBaseType">base type</param>
        /// <param name="AD_Org_ID">Optional Organization</param>
        /// <returns>true if open</returns>
        public static bool IsOpen(Ctx ctx, DateTime? dateAcct, String docBaseType, int AD_Org_ID)
        {
            if (dateAcct == null)
            {
                _log.Warning("No DateAcct");
                return false;
            }
            if (docBaseType == null)
            {
                _log.Warning("No DocBaseType");
                return false;
            }

            MPeriod period = MPeriod.Get(ctx, dateAcct, AD_Org_ID);
            if (period == null)
            {
                _log.Warning("No Period for " + dateAcct + " (" + docBaseType + ")");
                return false;
            }
            bool open = (period.IsOpen(docBaseType, dateAcct, AD_Org_ID) == null);
            if (!open)
            {
                _log.Warning(period.GetName() + ": Not open for " + docBaseType + " (" + dateAcct + ")");
            }
            return open;
        }

        

        /// <summary>
        /// Is Period Open for Doc Base Type
        /// </summary>
        /// <param name="docBaseType">document base type</param>
        /// <returns>true if open</returns>
        public bool IsOpen(String docBaseType)
        {
            if (!IsActive())
            {
                _log.Warning("Period not active: " + GetName());
                return false;
            }

            MAcctSchema mas = MClient.Get(GetCtx(), GetAD_Client_ID()).GetAcctSchema();
            if (mas != null && mas.IsAutoPeriodControl())
            {
                //	if (as.getC_Period_ID() == getC_Period_ID())
                //		return true;
                DateTime today = DateTime.Now;// new DateTime(CommonFunctions.CurrentTimeMillis());
                DateTime first = TimeUtil.AddDays(today, -mas.GetPeriod_OpenHistory());
                DateTime last = TimeUtil.AddDays(today, mas.GetPeriod_OpenFuture());
                //if (today.before(first))
                if (today < first)
                {
                    log.Warning("Today before first day - " + first);
                    return false;
                }
                //if (today.after(last))
                if (today > last)
                {
                    log.Warning("Today after last day - " + first);
                    return false;
                }
                //	We are OK
                if (IsInPeriod(today))
                {
                    mas.SetC_Period_ID(GetC_Period_ID());
                    mas.Save();
                }
                return true;
            }

            //	Standard Period Control
            if (docBaseType == null)
            {
                log.Warning(GetName() + " - No DocBaseType");
                return false;
            }
            MPeriodControl pc = GetPeriodControl(docBaseType);
            if (pc == null)
            {
                log.Warning(GetName() + " - Period Control not found for " + docBaseType);
                return false;
            }
            log.Fine(GetName() + ": " + docBaseType);
            return pc.IsOpen();
        }

        //Added By Amit 4-8-2015 VAMRP
        public static MPeriod GetPreviousPeriod(MPeriod period, Ctx ctx, Trx trx)
        {

            MPeriod newPeriod = null;
            String sql = "SELECT * FROM C_Period WHERE " +
            "C_Period.IsActive='Y' AND PeriodType='S' " +
            "AND C_Period.C_Year_ID IN " +
            "(SELECT C_Year_ID FROM C_Year WHERE C_Year.C_Calendar_ID = @param1 ) " +
            "AND ((C_Period.C_Year_ID * 1000) + C_Period.PeriodNo) " +
            " < ((@param2 * 1000) + @param3) ORDER BY C_Period.C_Year_ID DESC, C_Period.PeriodNo DESC";

            SqlParameter[] param = null;
            IDataReader idr = null;
            DataTable dt = new DataTable();
            try
            {
                param = new SqlParameter[3];
                param[0] = new SqlParameter("@param1", period.GetC_Calendar_ID());
                param[1] = new SqlParameter("@param2", period.GetC_Year_ID());
                param[2] = new SqlParameter("@param3", period.GetPeriodNo());
                idr = DB.ExecuteReader(sql, param, null);
                dt.Load(idr);
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                if (dt.Rows.Count > 0)
                {
                    newPeriod = new MPeriod(ctx, dt.Rows[0], trx);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            return newPeriod;
        }

        public static MPeriod GetNextPeriod(MPeriod period, Ctx ctx, Trx trx)
        {

            MPeriod newPeriod = null;
            String sql = "SELECT * FROM C_Period WHERE " +
            "C_Period.IsActive='Y' AND PeriodType='S' " +
            "AND C_Period.C_Year_ID IN " +
            "(SELECT C_Year_ID FROM C_Year WHERE C_Year.C_Calendar_ID = @param1 ) " +
            "AND ((C_Period.C_Year_ID * 1000) + C_Period.PeriodNo) " +
            " > ((@param2 * 1000) + @param3) ORDER BY C_Period.C_Year_ID ASC, C_Period.PeriodNo ASC";

            IDataReader idr = null;
            SqlParameter[] param = null;
            DataTable dt = new DataTable();
            try
            {
                param = new SqlParameter[3];
                param[0] = new SqlParameter("@param1", period.GetC_Calendar_ID());
                param[1] = new SqlParameter("@param2", period.GetC_Year_ID());
                param[2] = new SqlParameter("@param3", period.GetPeriodNo());
                idr = DB.ExecuteReader(sql, param, null);
                dt.Load(idr);
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                if (dt.Rows.Count > 0)
                {
                    newPeriod = new MPeriod(ctx, dt.Rows[0], trx);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            return newPeriod;
        }

        public static MPeriod[] GetAllPeriodsInRange(MPeriod startPeriod, MPeriod endPeriod, int calendar_ID, Ctx ctx, Trx trx)
        {
            if ((startPeriod.GetC_Calendar_ID() != calendar_ID) ||
                    (endPeriod.GetC_Calendar_ID() != calendar_ID))
            {
                _log.SaveError("Error", "Periods do not belong to the calendar");
                return null;
            }
            List<MPeriod> periods = new List<MPeriod>();
            String sql = "SELECT * FROM C_Period WHERE " +
            "C_Period.IsActive='Y' AND PeriodType='S' " +
            "AND C_Period.C_Year_ID IN " +
            "(SELECT C_Year_ID FROM C_Year WHERE C_Year.C_Calendar_ID = @param1 ) " + //calendar_ID
            "AND ((C_Period.C_Year_ID * 1000) + C_Period.PeriodNo) BETWEEN" +
            " (@param2 * 1000 + @param3) AND (@param4 * 1000 + @param5 )" + //start Period year ID, Period Number , End Period Year ID, Period Number
            " ORDER BY C_Period.C_Year_ID ASC, C_Period.PeriodNo ASC";

            SqlParameter[] param = null;
            IDataReader idr = null;
            DataTable dt = new DataTable();
            try
            {
                param = new SqlParameter[5];
                param[0] = new SqlParameter("@param1", calendar_ID);
                param[1] = new SqlParameter("@param2", startPeriod.GetC_Year_ID());
                param[2] = new SqlParameter("@param3", startPeriod.GetPeriodNo());
                param[3] = new SqlParameter("@param4", endPeriod.GetC_Year_ID());
                param[4] = new SqlParameter("@param5", endPeriod.GetPeriodNo());
                idr = DB.ExecuteReader(sql, param, trx);
                dt.Load(idr);
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        periods.Add(new MPeriod(ctx, dt.Rows[i], trx));
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            MPeriod[] retValue = new MPeriod[periods.Count];
            retValue = periods.ToArray();
            return retValue;
        }
        //End


        /** Calendar				*/
        private int C_Calendar_ID = 0;

        /// <summary>
        /// Get Calendar of Period
        /// </summary>
        /// <returns>calendar</returns>
        public int GetC_Calendar_ID()
        {
            if (C_Calendar_ID == 0)
            {
                MYear year = MYear.Get(GetCtx(), GetC_Year_ID());
                if (year != null)
                {
                    C_Calendar_ID = year.GetC_Calendar_ID();
                }
                else
                {
                    log.Severe("@NotFound@ C_Year_ID=" + GetC_Year_ID());
                }
            }
            return C_Calendar_ID;
        }


        /// <summary>
        /// Is standard Period Open for specified orgs for the client. For best
        /// performance, ensure that the list of orgs does not contain duplicates.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="orgs"></param>
        /// <param name="DateAcct">accounting date</param>
        /// <param name="DocBaseType">document base type</param>
        /// <returns>error message or null</returns>
        /// <date>07-March-2011</date>
        /// <writer>raghu</writer>
        public static String IsOpen(Ctx ctx, int AD_Client_ID, List<int> orgs, DateTime? DateAcct,
                String DocBaseType)
        {
            if (DateAcct == null)
                return "@NotFound@ @DateAcct@";
            if (DocBaseType == null)
                return "@NotFound@ @DocBaseType@";

            MAcctSchema as1 = MClient.Get(ctx, AD_Client_ID).GetAcctSchema();
            if (as1 == null)
                return "@NotFound@ @C_AcctSchema_ID@ for AD_Client_ID=" + AD_Client_ID;
            if (as1.IsAutoPeriodControl())
            {
                if (as1.IsAutoPeriodControlOpen(DateAcct))
                    return null;
                else
                    return "@PeriodClosed@ - @AutoPeriodControl@";
            }

            //	Get all Calendars in line with Organizations
            MClientInfo clientInfo = MClientInfo.Get(ctx, AD_Client_ID, null);
            List<int> orgCalendars = new List<int>();
            List<int> calendars = new List<int>();
            foreach (int org in orgs)
            {
                MOrgInfo orgInfo = MOrgInfo.Get(ctx, org, null);

                int C_Calendar_ID = orgInfo.GetC_Calendar_ID();
                if (C_Calendar_ID == 0)
                    C_Calendar_ID = clientInfo.GetC_Calendar_ID();
                orgCalendars.Add(C_Calendar_ID);
                if (!calendars.Contains(C_Calendar_ID))
                    calendars.Add(C_Calendar_ID);
            }
            //	Should not happen
            if (calendars.Count == 0)
                return "@NotFound@ @C_Calendar_ID@";

            //	For all Calendars get Periods
            for (int i = 0; i < calendars.Count; i++)
            {
                int C_Calendar_ID = calendars[i];
                MPeriod period = MPeriod.GetOfCalendar(ctx, C_Calendar_ID, DateAcct);
                //	First Org for Calendar
                int AD_Org_ID = 0;
                for (int j = 0; j < orgCalendars.Count; j++)
                {
                    if (orgCalendars[j] == C_Calendar_ID)
                    {
                        AD_Org_ID = orgs[j];
                        break;
                    }
                }
                if (period == null)
                {
                    MCalendar cal = MCalendar.Get(ctx, C_Calendar_ID);
                    String date = DisplayType.GetDateFormat(DisplayType.Date).Format(DateAcct);
                    if (cal != null)
                        return "@NotFound@ @C_Period_ID@: " + date
                        + " - " + MOrg.Get(ctx, AD_Org_ID).GetName()
                        + " -> " + cal.GetName();
                    else
                        return "@NotFound@ @C_Period_ID@: " + date
                        + " - " + MOrg.Get(ctx, AD_Org_ID).GetName()
                        + " -> C_Calendar_ID=" + C_Calendar_ID;
                }
                String error = period.IsOpen(DocBaseType, DateAcct);
                if (error != null)
                    return error
                    + " - " + MOrg.Get(ctx, AD_Org_ID).GetName()
                    + " -> " + MCalendar.Get(ctx, C_Calendar_ID).GetName();
            }
            return null;	//	open
        }

        /// <summary>
        ///  	Is Period Open for Doc Base Type
        /// </summary>
        /// <param name="DocBaseType">document base type</param>
        /// <param name="dateAcct">accounting date</param>
        /// <returns>error message or null</returns>
        /// <date>07-March-2011</date>
        /// <writer>raghu</writer>
        public String IsOpen(String DocBaseType, DateTime? dateAcct)
        {
            //if (!IsActive())
            //{
            //    _log.Warning("Period not active: " + GetName());
            //    return "@C_Period_ID@ <> @IsActive@";
            //}

            //MAcctSchema as1 = MClient.Get(GetCtx(), GetAD_Client_ID()).GetAcctSchema();
            //if (as1 != null && as1.IsAutoPeriodControl())
            //{
            //    if (!as1.IsAutoPeriodControlOpen(dateAcct))
            //        return "@PeriodClosed@ - @AutoPeriodControl@";
            //    //	We are OK
            //    DateTime today = DateTime.Now.Date;
            //    if (IsInPeriod(today) && as1.GetC_Period_ID() != GetC_Period_ID())
            //    {
            //        as1.SetC_Period_ID(GetC_Period_ID());
            //        as1.Save();
            //    }
            //    return null;
            //}

            ////	Standard Period Control
            //if (DocBaseType == null)
            //{
            //    log.Warning(GetName() + " - No DocBaseType");
            //    return "@NotFound@ @DocBaseType@";
            //}
            //MPeriodControl pc = GetPeriodControl(DocBaseType);
            //if (pc == null)
            //{
            //    log.Warning(GetName() + " - Period Control not found for " + DocBaseType);
            //    return "@NotFound@ @C_PeriodControl_ID@: " + DocBaseType;
            //}
            //log.Fine(GetName() + ": " + DocBaseType);
            //if (pc.IsOpen())
            //    return null;
            //return "@PeriodClosed@ - @C_PeriodControl_ID@ ("
            //+ DocBaseType + ", " + dateAcct + ")";

            return IsOpen(DocBaseType, dateAcct, 0);
        }

        /// <summary>
        /// Is Period Open for Doc Base Type in selected Organization
        /// </summary>
        /// <param name="DocBaseType">document base type</param>
        /// <param name="dateAcct">accounting date</param>
        /// <returns>error message or null</returns>
        /// <date>07-March-2011</date>
        /// <writer>raghu</writer>
        public String IsOpen(String DocBaseType, DateTime? dateAcct, int AD_Org_ID)
        {
            if (!IsActive())
            {
                _log.Warning("Period not active: " + GetName());
                return "@C_Period_ID@ <> @IsActive@";
            }

            MAcctSchema as1 = null;

            if (AD_Org_ID > 0)
            {
                as1 = MOrg.Get(GetCtx(), AD_Org_ID).GetAcctSchema();
            }
            else
            {
                as1 = MClient.Get(GetCtx(), GetAD_Client_ID()).GetAcctSchema();
            }
            if (as1 != null && as1.IsAutoPeriodControl())
            {
                if (!as1.IsAutoPeriodControlOpen(dateAcct))
                    return "@PeriodClosed@ - @AutoPeriodControl@";
                //	We are OK
                DateTime today = DateTime.Now.Date;
                if (IsInPeriod(today) && as1.GetC_Period_ID() != GetC_Period_ID())
                {
                    as1.SetC_Period_ID(GetC_Period_ID());
                    as1.Save();
                }
                return null;
            }

            //	Standard Period Control
            if (DocBaseType == null)
            {
                log.Warning(GetName() + " - No DocBaseType");
                return "@NotFound@ @DocBaseType@";
            }
            MPeriodControl pc = GetPeriodControl(DocBaseType, AD_Org_ID);
            if (pc == null)
            {
                log.Warning(GetName() + " - Period Control not found for " + DocBaseType);
                return "@NotFound@ @C_PeriodControl_ID@: " + DocBaseType;
            }
            log.Fine(GetName() + ": " + DocBaseType);
            if (pc.IsOpen())
                return null;
            return "@PeriodClosed@ - @C_PeriodControl_ID@ ("
            + DocBaseType + ", " + dateAcct + ")";
        }
    }
}