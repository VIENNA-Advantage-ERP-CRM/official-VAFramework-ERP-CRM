/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MGroup
 * Purpose        : Request Group Model
 * Class Used     : X_R_Group
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
    public class MGroup : X_R_Group
    {
        /**
         * 	Get MGroup from Cache
         *	@param ctx context
         *	@param R_Group_ID id
         *	@return MGroup
         */
        public static MGroup Get(Ctx ctx, int R_Group_ID)
        {
            int key = R_Group_ID;
            MGroup retValue = (MGroup)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MGroup(ctx, R_Group_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        } //	get

        /**	Cache						*/
        private static CCache<int, MGroup> _cache = new CCache<int, MGroup>("R_Group", 20);


        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param R_Group_ID group
         *	@param trxName trx
         */
        public MGroup(Ctx ctx, int R_Group_ID, Trx trxName) :
            base(ctx, R_Group_ID, trxName)
        {
            //super(ctx, R_Group_ID, trxName);
        }	//	MGroup

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MGroup(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MGroup

    }
}
