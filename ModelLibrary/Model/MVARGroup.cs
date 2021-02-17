/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVARGroup
 * Purpose        : Request Group Model
 * Class Used     : X_VAR_Group
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
    public class MVARGroup : X_VAR_Group
    {
        /**
         * 	Get MVARGroup from Cache
         *	@param ctx context
         *	@param VAR_Group_ID id
         *	@return MVARGroup
         */
        public static MVARGroup Get(Ctx ctx, int VAR_Group_ID)
        {
            int key = VAR_Group_ID;
            MVARGroup retValue = (MVARGroup)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVARGroup(ctx, VAR_Group_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        } //	get

        /**	Cache						*/
        private static CCache<int, MVARGroup> _cache = new CCache<int, MVARGroup>("VAR_Group", 20);


        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAR_Group_ID group
         *	@param trxName trx
         */
        public MVARGroup(Ctx ctx, int VAR_Group_ID, Trx trxName) :
            base(ctx, VAR_Group_ID, trxName)
        {
            //super(ctx, VAR_Group_ID, trxName);
        }	//	MVARGroup

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MVARGroup(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MVARGroup

    }
}
