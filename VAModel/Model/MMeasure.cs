/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMeasure
 * Purpose        : Performance Measure
 * Class Used     : X_PA_Measure
 * Chronological    Development
 * Raghunandan     17-Jun-2009
  ******************************************************/
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
using System.Data.SqlClient;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
namespace VAdvantage.Model
{
    public class MMeasure : X_PA_Measure
    {
        /**
	 * 	Get MMeasure from Cache
	 *	@param ctx context
	 *	@param PA_Measure_ID id
	 *	@return MMeasure
	 */
        public static MMeasure Get(Ctx ctx, int PA_Measure_ID)
        {
            int key = PA_Measure_ID;
            MMeasure retValue = (MMeasure)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MMeasure(ctx, PA_Measure_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**	Cache						*/
        private static CCache<int, MMeasure> _cache
            = new CCache<int, MMeasure>("PA_Measure", 10);

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param PA_Measure_ID id
         *	@param trxName trx
         */
        public MMeasure(Ctx ctx, int PA_Measure_ID, Trx trxName) :
            base(ctx, PA_Measure_ID, trxName)
        {
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result Set
         *	@param trxName trx
         */
        public MMeasure(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }


        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MMeasure[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }	//	toString

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (MEASURETYPE_Calculated.Equals(GetMeasureType())
                && GetPA_MeasureCalc_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "PA_MeasureCalc_ID"));
                return false;
            }
            else if (MEASURETYPE_Ratio.Equals(GetMeasureType())
                && GetPA_Ratio_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "PA_Ratio_ID"));
                return false;
            }
            else if (MEASURETYPE_UserDefined.Equals(GetMeasureType())
                && (GetCalculationClass() == null || GetCalculationClass().Length == 0))
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "CalculationClass"));
                return false;
            }
            else if (MEASURETYPE_Request.Equals(GetMeasureType())
                && GetR_RequestType_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "R_RequestType_ID"));
                return false;
            }
            else if (MEASURETYPE_Project.Equals(GetMeasureType())
                && GetC_ProjectType_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "C_ProjectType_ID"));
                return false;
            }
            return true;
        }	//	beforeSave

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return succes
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            //	Update Goals with Manual Measure
            if (success && MEASURETYPE_Manual.Equals(GetMeasureType()))
                UpdateManualGoals();

            return success;
        }	//	afterSave

        /**
         * 	Update/save Goals
         * 	@return true if updated
         */
        public Boolean UpdateGoals()
        {
            String mt = GetMeasureType();
            try
            {
                if (MEASURETYPE_Manual.Equals(mt))
                    return UpdateManualGoals();
                else if (MEASURETYPE_Achievements.Equals(mt))
                    return UpdateAchievementGoals();
                else if (MEASURETYPE_Calculated.Equals(mt))
                    return UpdateCalculatedGoals();
                else if (MEASURETYPE_Ratio.Equals(mt))
                    return UpdateRatios();
                else if (MEASURETYPE_Request.Equals(mt))
                    return UpdateRequests();
                else if (MEASURETYPE_Project.Equals(mt))
                    return UpdateProjects();
                //	Projects
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "MeasureType=" + mt, e);
            }
            return false;
        }	//	updateGoals

        /**
         * 	Update/save Manual Goals
         * 	@return true if updated
         */
        private Boolean UpdateManualGoals()
        {
            if (!MEASURETYPE_Manual.Equals(GetMeasureType()))
                return false;
            MGoal[] goals = MGoal.GetMeasureGoals(GetCtx(), GetPA_Measure_ID());
            for (int i = 0; i < goals.Length; i++)
            {
                MGoal goal = goals[i];
                goal.SetMeasureActual(GetManualActual());
                goal.Save();
            }
            return true;
        }	//	updateManualGoals

        /**
         * 	Update/save Goals with Achievement
         * 	@return true if updated
         */
        private Boolean UpdateAchievementGoals()
        {
            if (!MEASURETYPE_Achievements.Equals(GetMeasureType()))
                return false;
            DateTime today = DateTime.Now;
            MGoal[] goals = MGoal.GetMeasureGoals(GetCtx(), GetPA_Measure_ID());
            for (int i = 0; i < goals.Length; i++)
            {
                MGoal goal = goals[i];
                String MeasureScope = goal.GetMeasureScope();
                String trunc = TimeUtil.TRUNC_DAY;
                if (MGoal.MEASUREDISPLAY_Year.Equals(MeasureScope))
                    trunc = TimeUtil.TRUNC_YEAR;
                else if (MGoal.MEASUREDISPLAY_Quarter.Equals(MeasureScope))
                    trunc = TimeUtil.TRUNC_QUARTER;
                else if (MGoal.MEASUREDISPLAY_Month.Equals(MeasureScope))
                    trunc = TimeUtil.TRUNC_MONTH;
                else if (MGoal.MEASUREDISPLAY_Week.Equals(MeasureScope))
                    trunc = TimeUtil.TRUNC_WEEK;
                DateTime compare = TimeUtil.Trunc(today, trunc);
                //
                MAchievement[] achievements = MAchievement.GetOfMeasure(GetCtx(), GetPA_Measure_ID());
                Decimal ManualActual = Env.ZERO;
                for (int j = 0; j < achievements.Length; j++)
                {
                    MAchievement achievement = achievements[j];
                    if (achievement.IsAchieved() && achievement.GetDateDoc() != null)
                    {
                        DateTime ach = TimeUtil.Trunc(achievement.GetDateDoc(), trunc);
                        if (compare.Equals(ach))
                            ManualActual = Decimal.Add(ManualActual, achievement.GetManualActual());
                    }
                }
                goal.SetMeasureActual(ManualActual);
                goal.Save();
            }
            return true;
        }

        /**
         * 	Update/save Goals with Calculation
         * 	@return true if updated
         */
        private Boolean UpdateCalculatedGoals()
        {
            if (!MEASURETYPE_Calculated.Equals(GetMeasureType()))
                return false;
            MGoal[] goals = MGoal.GetMeasureGoals(GetCtx(), GetPA_Measure_ID());
            for (int i = 0; i < goals.Length; i++)
            {
                MGoal goal = goals[i];
                //	Find Role
                MRole role = null;
                if (goal.GetAD_Role_ID() != 0)
                    role = MRole.Get(GetCtx(), goal.GetAD_Role_ID());
                else if (goal.GetAD_User_ID() != 0)
                {
                    MUser user = MUser.Get(GetCtx(), goal.GetAD_User_ID());
                    MRole[] roles = user.GetRoles(goal.GetAD_Org_ID());
                    if (roles.Length > 0)
                        role = roles[0];
                }
                if (role == null)
                    role = MRole.GetDefault(GetCtx(), false);	//	could result in wrong data
                //
                MMeasureCalc mc = MMeasureCalc.Get(GetCtx(), GetPA_MeasureCalc_ID());
                if (mc == null || mc.Get_ID() == 0 || mc.Get_ID() != GetPA_MeasureCalc_ID())
                {
                    log.Log(Level.SEVERE, "Not found PA_MeasureCalc_ID=" + GetPA_MeasureCalc_ID());
                    return false;
                }

                Decimal? ManualActual = null;
                String sql = mc.GetSqlPI(goal.GetRestrictions(false),
                    goal.GetMeasureScope(), GetMeasureDataType(), null, role);
                IDataReader idr = null;
                try		//	SQL statement could be wrong
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (idr.Read())
                        ManualActual = Utility.Util.GetValueOfDecimal(idr[0]);
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

                //	SQL may return no rows or null
                if (ManualActual == null)
                {
                    ManualActual = Env.ZERO;
                    log.Fine("No Value = " + sql);
                }
                goal.SetMeasureActual(ManualActual);
                goal.Save();
            }
            return true;
        }

        /**
         * 	Update/save Goals with Ratios
         * 	@return true if updated
         */
        private Boolean UpdateRatios()
        {
            if (!MEASURETYPE_Ratio.Equals(GetMeasureType()))
                return false;
            return false;
        }

        /**
         * 	Update/save Goals with Requests
         * 	@return true if updated
         */
        private Boolean UpdateRequests()
        {
            if (!MEASURETYPE_Request.Equals(GetMeasureType())
                || GetR_RequestType_ID() == 0)
                return false;
            MGoal[] goals = MGoal.GetMeasureGoals(GetCtx(), GetPA_Measure_ID());
            for (int i = 0; i < goals.Length; i++)
            {
                MGoal goal = goals[i];
                //	Find Role
                MRole role = null;
                if (goal.GetAD_Role_ID() != 0)
                    role = MRole.Get(GetCtx(), goal.GetAD_Role_ID());
                else if (goal.GetAD_User_ID() != 0)
                {
                    MUser user = MUser.Get(GetCtx(), goal.GetAD_User_ID());
                    MRole[] roles = user.GetRoles(goal.GetAD_Org_ID());
                    if (roles.Length > 0)
                        role = roles[0];
                }
                if (role == null)
                    role = MRole.GetDefault(GetCtx(), false);	//	could result in wrong data
                //
                Decimal? ManualActual = null;
                MRequestType rt = MRequestType.Get(GetCtx(), GetR_RequestType_ID());
                String sql = rt.GetSqlPI(goal.GetRestrictions(false),
                    goal.GetMeasureScope(), GetMeasureDataType(), null, role);
                //PreparedStatement pstmt = null;
                IDataReader idr = null;
                try		//	SQL statement could be wrong
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (idr.Read())
                    {
                        ManualActual = Utility.Util.GetValueOfDecimal(idr[0]);
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

                //	SQL may return no rows or null
                if (ManualActual == null)
                {
                    ManualActual = Env.ZERO;
                    log.Fine("No Value = " + sql);
                }
                goal.SetMeasureActual(ManualActual);
                goal.Save();
            }
            return true;
        }

        /**
         * 	Update/save Goals with Projects
         * 	@return true if updated
         */
        private Boolean UpdateProjects()
        {
            if (!MEASURETYPE_Project.Equals(GetMeasureType())
                || GetC_ProjectType_ID() == 0)
                return false;
            MGoal[] goals = MGoal.GetMeasureGoals(GetCtx(), GetPA_Measure_ID());
            for (int i = 0; i < goals.Length; i++)
            {
                MGoal goal = goals[i];
                //	Find Role
                MRole role = null;
                if (goal.GetAD_Role_ID() != 0)
                    role = MRole.Get(GetCtx(), goal.GetAD_Role_ID());
                else if (goal.GetAD_User_ID() != 0)
                {
                    MUser user = MUser.Get(GetCtx(), goal.GetAD_User_ID());
                    MRole[] roles = user.GetRoles(goal.GetAD_Org_ID());
                    if (roles.Length > 0)
                        role = roles[0];
                }
                if (role == null)
                    role = MRole.GetDefault(GetCtx(), false);	//	could result in wrong data
                //
                Decimal? ManualActual = null;
                MProjectType pt = MProjectType.Get(GetCtx(), GetC_ProjectType_ID());
                String sql = pt.GetSqlPI(goal.GetRestrictions(false),
                    goal.GetMeasureScope(), GetMeasureDataType(), null, role);
                IDataReader idr = null;
                try		//	SQL statement could be wrong
                {

                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (idr.Read())
                        ManualActual = Utility.Util.GetValueOfDecimal(idr[0]);
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
                //	SQL may return no rows or null
                if (ManualActual == null)
                {
                    ManualActual = Env.ZERO;
                    log.Fine("No Value = " + sql);
                }
                goal.SetMeasureActual(ManualActual);
                goal.Save();
            }
            return true;
        }
    }
}