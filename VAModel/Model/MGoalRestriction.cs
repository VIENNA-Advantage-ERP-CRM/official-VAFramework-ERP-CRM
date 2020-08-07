/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MGoalRestriction
 * Purpose        : Performance Goal Restriction
 * Class Used     : X_PA_GoalRestriction
 * Chronological    Development
 * Raghunandan     17-Jun-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGoalRestriction : X_PA_GoalRestriction
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param PA_GoalRestriction_ID id
	 *	@param trxName trx
	 */
        public MGoalRestriction(Ctx ctx, int PA_GoalRestriction_ID,
            Trx trxName) :
            base(ctx, PA_GoalRestriction_ID, trxName)
        {
            // super();
        }	//	MGoalRestriction

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MGoalRestriction(Ctx ctx, DataRow rs, Trx trxName) :
            base(ctx, rs, trxName)
        {
            //super();
        }	//	MGoalRestriction


        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MGoalRestriction[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }	//	toString
    }
}
