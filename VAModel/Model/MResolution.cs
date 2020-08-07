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
    public class MResolution : X_R_Resolution
    {
        /**
         * 	Get MResolution from Cache
         *	@param ctx context
         *	@param R_Resolution_ID id
         *	@return MResolution
         */
        public static MResolution Get(Ctx ctx, int R_Resolution_ID)
        {
            int key = R_Resolution_ID;
            MResolution retValue = (MResolution)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MResolution(ctx, R_Resolution_ID, null);
            if (retValue.Get_ID() != 0)
            {
                _cache.Add(key, retValue);
            }
            return retValue;
        }	//	get

        /**	Cache						*/
        private static CCache<int, MResolution> _cache = new CCache<int, MResolution>("R_Resolution", 10);



        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param R_Resolution_ID id
         *	@param trxName
         */
        public MResolution(Ctx ctx, int R_Resolution_ID, Trx trxName) :
            base(ctx, R_Resolution_ID, trxName)
        {
            //super(ctx, R_Resolution_ID, trxName);
        }	//	MResolution

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MResolution(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, dr, trxName);
        }	//	MResolution
    }
}
