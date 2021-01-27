/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MGoalRestriction
 * Purpose        : Performance Goal Restriction
 * Class Used     : X_VAPA_TargetRestriction
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
    public class MGoalRestriction : X_VAPA_TargetRestriction
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param VAPA_TargetRestriction_ID id
	 *	@param trxName trx
	 */
        public MGoalRestriction(Ctx ctx, int VAPA_TargetRestriction_ID,
            Trx trxName) :
            base(ctx, VAPA_TargetRestriction_ID, trxName)
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
