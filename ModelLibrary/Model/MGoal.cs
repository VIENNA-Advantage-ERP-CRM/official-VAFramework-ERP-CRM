/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MGoal
 * Purpose        : Performance Goal
 * Class Used     : X_PA_Goal
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
    public class MGoal : X_PA_Goal
    {
        /**
         * 	Get User Goals
         *	@param ctx context
         *	@param AD_User_ID user
         *	@return array of goals
         */
        public static MGoal[] GetUserGoals(Ctx ctx, int AD_User_ID)
        {
            if (AD_User_ID < 0)
                return GetTestGoals(ctx);
            List<MGoal> list = new List<MGoal>();
            String sql = "SELECT * FROM PA_Goal g "
                + "WHERE IsActive='Y'"
                + " AND AD_Client_ID=@ADClientID"		//	#1
                + " AND ((AD_User_ID IS NULL AND AD_Role_ID IS NULL)"
                    + " OR AD_User_ID=@ADUserID"	//	#2
                    + " OR EXISTS (SELECT * FROM AD_User_Roles ur "
                        + "WHERE g.AD_User_ID=ur.AD_User_ID AND g.AD_Role_ID=ur.AD_Role_ID AND ur.IsActive='Y')) "
                + "ORDER BY SeqNo";
            DataTable dt;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@ADClientID", ctx.GetAD_Client_ID());
                param[1] = new SqlParameter("@ADUserID", AD_User_ID);

                idr = DataBase.DB.ExecuteReader(sql, null, null);

                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MGoal goal = new MGoal(ctx, dr, null);
                    goal.UpdateGoal(false);
                    list.Add(goal);
                }


            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log (Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            MGoal[] retValue = new MGoal[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Accessible Goals
         *	@param ctx context
         *	@return array of goals
         */
        public static MGoal[] GetGoals(Ctx ctx)
        {
            List<MGoal> list = new List<MGoal>();
            String sql = "SELECT * FROM PA_Goal WHERE IsActive='Y' "
                + "ORDER BY SeqNo";
            sql = MRole.GetDefault(ctx, false).AddAccessSQL(sql, "PA_Goal",
                false, true);	//	RW to restrict Access
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MGoal goal = new MGoal(ctx, dr, null);
                    goal.UpdateGoal(false);
                    list.Add(goal);

                }


            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log (Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }
            MGoal[] retValue = new MGoal[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /**
         * 	Create Test Goals
         *	@param ctx context
         *	@return array of goals
         */
        public static MGoal[] GetTestGoals(Ctx ctx)
        {
            MGoal[] retValue = new MGoal[4];
            retValue[0] = new MGoal(ctx, "Test 1", "Description 1", new Decimal(1000), null);
            retValue[0].SetMeasureActual(new Decimal(200));
            retValue[1] = new MGoal(ctx, "Test 2", "Description 2", new Decimal(1000), null);
            retValue[1].SetMeasureActual(new Decimal(900));
            retValue[2] = new MGoal(ctx, "Test 3", "Description 3", new Decimal(1000), null);
            retValue[2].SetMeasureActual(new Decimal(1200));
            retValue[3] = new MGoal(ctx, "Test 4", "Description 4", new Decimal(1000), null);
            retValue[3].SetMeasureActual(new Decimal(3200));
            return retValue;
        }

        /**
         * 	Get Goals with Measure
         *	@param ctx context
         *	@param PA_Measure_ID measure
         *	@return goals
         */
        public static MGoal[] GetMeasureGoals(Ctx ctx, int PA_Measure_ID)
        {
            List<MGoal> list = new List<MGoal>();
            String sql = "SELECT * FROM PA_Goal WHERE IsActive='Y' AND PA_Measure_ID= " + PA_Measure_ID
                + " ORDER BY SeqNo";
            DataTable dt;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MGoal(ctx, dr, null));
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log (Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            MGoal[] retValue = new MGoal[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Multiplier from Scope to Display
         *	@param goal goal
         *	@return null if error or multiplier
         */
        public static Decimal? GetMultiplier(MGoal goal)
        {
            String MeasureScope = goal.GetMeasureScope();
            String MeasureDisplay = goal.GetMeasureDisplay();
            if (MeasureDisplay == null
                || MeasureScope.Equals(MeasureDisplay))
                return Env.ONE;		//	1:1

            if (MeasureScope.Equals(MEASURESCOPE_Total)
                || MeasureDisplay.Equals(MEASUREDISPLAY_Total))
                return null;		//	Error

            Decimal? Multiplier = null;
            if (MeasureScope.Equals(MEASURESCOPE_Year))
            {
                if (MeasureDisplay.Equals(MEASUREDISPLAY_Quarter))
                    Multiplier = new Decimal(1.0 / 4.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Month))
                    Multiplier = new Decimal(1.0 / 12.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Week))
                    Multiplier = new Decimal(1.0 / 52.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Day))
                    Multiplier = new Decimal(1.0 / 364.0);
            }
            else if (MeasureScope.Equals(MEASURESCOPE_Quarter))
            {
                if (MeasureDisplay.Equals(MEASUREDISPLAY_Year))
                    Multiplier = new Decimal(4.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Month))
                    Multiplier = new Decimal(1.0 / 3.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Week))
                    Multiplier = new Decimal(1.0 / 13.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Day))
                    Multiplier = new Decimal(1.0 / 91.0);
            }
            else if (MeasureScope.Equals(MEASURESCOPE_Month))
            {
                if (MeasureDisplay.Equals(MEASUREDISPLAY_Year))
                    Multiplier = new Decimal(12.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Quarter))
                    Multiplier = new Decimal(3.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Week))
                    Multiplier = new Decimal(1.0 / 4.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Day))
                    Multiplier = new Decimal(1.0 / 30.0);
            }
            else if (MeasureScope.Equals(MEASURESCOPE_Week))
            {
                if (MeasureDisplay.Equals(MEASUREDISPLAY_Year))
                    Multiplier = new Decimal(52.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Quarter))
                    Multiplier = new Decimal(13.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Month))
                    Multiplier = new Decimal(4.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Day))
                    Multiplier = new Decimal(1.0 / 7.0);
            }
            else if (MeasureScope.Equals(MEASURESCOPE_Day))
            {
                if (MeasureDisplay.Equals(MEASUREDISPLAY_Year))
                    Multiplier = new Decimal(364.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Quarter))
                    Multiplier = new Decimal(91.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Month))
                    Multiplier = new Decimal(30.0);
                else if (MeasureDisplay.Equals(MEASUREDISPLAY_Week))
                    Multiplier = new Decimal(7.0);
            }
            return Multiplier;
        }

        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MGoal).FullName);

        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param PA_Goal_ID id
         *	@param trxName trx
         */
        public MGoal(Ctx ctx, int PA_Goal_ID, Trx trxName) :
            base(ctx, PA_Goal_ID, trxName)
        {
            //super ();
            if (PA_Goal_ID == 0)
            {
                //	SetName (null);
                //	SetAD_User_ID (0);
                //	SetPA_ColorSchema_ID (0);
                SetSeqNo(0);
                SetIsSummary(false);
                SetMeasureScope(MEASUREDISPLAY_Year);
                SetGoalPerformance(Env.ZERO);
                SetRelativeWeight(Env.ONE);
                SetMeasureTarget(Env.ZERO);
                SetMeasureActual(Env.ZERO);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName trx
         */
        public MGoal(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Base Constructor
         *	@param ctx context
         *	@param Name Name
         *	@param Description Decsription
         *	@param MeasureTarGet tarGet
         *	@param trxName trx
         */
        public MGoal(Ctx ctx, String Name, String Description,
            Decimal MeasureTarGet, Trx trxName) :
            base(ctx, 0, trxName)
        {

            SetName(Name);
            SetDescription(Description);
            SetMeasureTarget(MeasureTarGet);
        }


        /** Restrictions					*/
        private MGoalRestriction[] _restrictions = null;

        /**
         * 	Get Restriction Lines
         *	@param reload reload data
         *	@return array of lines
         */
        public MGoalRestriction[] GetRestrictions(Boolean reload)
        {
            if (_restrictions != null && !reload)
                return _restrictions;
            List<MGoalRestriction> list = new List<MGoalRestriction>();
            //
            String sql = "SELECT * FROM PA_GoalRestriction "
                + "WHERE PA_Goal_ID=@PA_Goal_ID AND IsActive='Y' "
                + "ORDER BY Org_ID, C_BPartner_ID, M_Product_ID";
            DataTable dt;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MGoalRestriction(GetCtx(), dr, Get_TrxName()));
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
               log.Log (Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }
            //
            _restrictions = new MGoalRestriction[list.Count];
            _restrictions = list.ToArray();
            return _restrictions;
        }

        /**
         * 	Get Measure
         *	@return measure or null
         */
        public MMeasure GetMeasure()
        {
            if (GetPA_Measure_ID() != 0)
                return MMeasure.Get(GetCtx(), GetPA_Measure_ID());
            return null;
        }


        /**************************************************************************
         * 	Update/save Goals for the same measure
         * 	@param force force to update goal (default once per day)
         * 	@return true if updated
         */
        public Boolean UpdateGoal(Boolean force)
        {
           log.Config("Force=" + force);
            MMeasure measure = MMeasure.Get(GetCtx(), GetPA_Measure_ID());
            if (force
                || GetDateLastRun() == null
                || !TimeUtil.IsSameHour(GetDateLastRun(), null))
            {
                if (measure.UpdateGoals())		//	saves
                {
                    Load(Get_ID(), Get_TrxName());
                    return true;
                }
            }
            return false;
        }

        /**
         * 	Set Measure Actual
         *	@param MeasureActual actual
         */
        public new void SetMeasureActual(Decimal? MeasureActual)
        {
            if (MeasureActual == null)
                return;
            base.SetMeasureActual((Decimal)MeasureActual);
            SetDateLastRun(DateTime.Now);
            SetGoalPerformance();
        }

        /**
         * 	Calculate Performance Goal as multiplier
         */
        public void SetGoalPerformance()
        {
            Decimal MeasureTarGet = GetMeasureTarget();
            Decimal MeasureActual = GetMeasureActual();
            Decimal GoalPerformance = Env.ZERO;
            if (Env.Signum(MeasureTarGet) != 0)
            {
                //GoalPerformance
                GoalPerformance = Decimal.Round(GoalPerformance, 6, MidpointRounding.AwayFromZero);
            }

            base.SetGoalPerformance(GoalPerformance);
        }

        /**
         * 	Get Goal Performance as Double
         *	@return performance as multipier
         */
        public double GetGoalPerformanceDouble()
        {
            Decimal bd = GetGoalPerformance();
            return Decimal.ToDouble(bd);
        }

        /**
         * 	Get Goal Performance in Percent
         *	@return performance in percent
         */
        public int GetPercent()
        {
            Decimal bd = Decimal.Multiply(GetGoalPerformance(), Env.ONEHUNDRED);
            return Decimal.ToInt32(bd);
        }

        /**
         * 	Get Color
         *	@return color - white if no tarGet
         */
        //public Color GetColor()
        //{
        //    if (Env.Signum(GetMeasureTarGet()) == 0)
        //        return Color.white;
        //    else
        //        return MColorSchema.GetColor(GetCtx(), GetPA_ColorSchema_ID(), GetPercent());
        //}

        /**
         * Get the color schema for this goal.
         * 
         * @return the color schema, or null if the measure targer is 0
         */
        //public MColorSchema GetColorSchema()
        //{
        //    return (Env.Signum(GetMeasureTarget()) == 0) ?
        //        null : MColorSchema.Get(GetCtx(), GetPA_ColorSchema_ID());
        //}

        /**
         * 	Get Measure Display
         *	@return Measure Display
         */
        public new String GetMeasureDisplay()
        {
            String s = base.GetMeasureDisplay();
            if (s == null)
            {
                if (MEASURESCOPE_Week.Equals(GetMeasureScope()))
                    s = MEASUREDISPLAY_Week;
                else if (MEASURESCOPE_Day.Equals(GetMeasureScope()))
                    s = MEASUREDISPLAY_Day;
                else
                    s = MEASUREDISPLAY_Month;
            }
            return s;
        }

        /**
         * 	Get Measure Display Text
         *	@return Measure Display Text
         */
        public String GetXAxisText()
        {
            MMeasure measure = GetMeasure();
            if (measure != null
                && MMeasure.MEASUREDATATYPE_StatusQtyAmount.Equals(measure.GetMeasureDataType()))
            {
                if (MMeasure.MEASURETYPE_Request.Equals(measure.GetMeasureType()))
                    return Msg.GetElement(GetCtx(), "R_Status_ID");
                if (MMeasure.MEASURETYPE_Project.Equals(measure.GetMeasureType()))
                    return Msg.GetElement(GetCtx(), "C_Phase_ID");
            }
            String value = GetMeasureDisplay();
            String display = MRefList.GetListName(GetCtx(), MEASUREDISPLAY_AD_Reference_ID, value);
            return display == null ? value : display;
        }

        /**
         * 	Goal has TarGet
         *	@return true if tarGet
         */
        public Boolean IsTarGet()
        {
            return Env.Signum(GetMeasureTarget()) != 0;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MGoal[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",").Append(GetGoalPerformance())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Measure required if nor Summary
            if (!IsSummary() && GetPA_Measure_ID() == 0)
            {
               log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "PA_Measure_ID"));
                return false;
            }
            if (IsSummary() && GetPA_Measure_ID() != 0)
                SetPA_Measure_ID(0);

            //	User/Role Check
            if ((newRecord || Is_ValueChanged("AD_User_ID") || Is_ValueChanged("AD_Role_ID"))
                && GetAD_User_ID() != 0)
            {
                MUser user = MUser.Get(GetCtx(), GetAD_User_ID());
                MRole[] roles = user.GetRoles(GetAD_Org_ID());
                if (roles.Length == 0)		//	No Role
                    SetAD_Role_ID(0);
                else if (roles.Length == 1)	//	One
                    SetAD_Role_ID(roles[0].GetAD_Role_ID());
                else
                {
                    int AD_Role_ID = GetAD_Role_ID();
                    if (AD_Role_ID != 0)	//	validate
                    {
                        Boolean found = false;
                        for (int i = 0; i < roles.Length; i++)
                        {
                            if (AD_Role_ID == roles[i].GetAD_Role_ID())
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            AD_Role_ID = 0;
                    }
                    if (AD_Role_ID == 0)		//	Set to first one
                        SetAD_Role_ID(roles[0].GetAD_Role_ID());
                }	//	multiple roles
            }

            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return true
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
                return success;

            //	Update Goal if TarGet / Scope Changed
            if (newRecord
                || Is_ValueChanged("MeasureTarGet")
                || Is_ValueChanged("MeasureScope"))
                UpdateGoal(true);

            return success;
        }
    }
}