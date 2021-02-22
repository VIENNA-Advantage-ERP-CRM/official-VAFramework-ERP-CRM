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
    public class MVARResolution : X_VAR_Resolution
    {
        /**
         * 	Get MVARResolution from Cache
         *	@param ctx context
         *	@param VAR_Resolution_ID id
         *	@return MVARResolution
         */
        public static MVARResolution Get(Ctx ctx, int VAR_Resolution_ID)
        {
            int key = VAR_Resolution_ID;
            MVARResolution retValue = (MVARResolution)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVARResolution(ctx, VAR_Resolution_ID, null);
            if (retValue.Get_ID() != 0)
            {
                _cache.Add(key, retValue);
            }
            return retValue;
        }	//	get

        /**	Cache						*/
        private static CCache<int, MVARResolution> _cache = new CCache<int, MVARResolution>("VAR_Resolution", 10);



        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAR_Resolution_ID id
         *	@param trxName
         */
        public MVARResolution(Ctx ctx, int VAR_Resolution_ID, Trx trxName) :
            base(ctx, VAR_Resolution_ID, trxName)
        {
            //super(ctx, VAR_Resolution_ID, trxName);
        }	//	MVARResolution

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MVARResolution(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, dr, trxName);
        }	//	MVARResolution
    }
}
