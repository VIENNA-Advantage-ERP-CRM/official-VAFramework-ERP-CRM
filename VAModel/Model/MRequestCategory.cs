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

namespace VAdvantage.Model
{
    public class MRequestCategory : X_R_Category
    {
        /**	Cache						*/
        private static CCache<int, MRequestCategory> _cache = new CCache<int, MRequestCategory>("R_Category", 20);

        /**
         * 	Get MCategory from Cache
         *	@param ctx context
         *	@param R_Category_ID id
         *	@return MCategory
         */
        public static MRequestCategory Get(Ctx ctx, int R_Category_ID)
        {
            int key =R_Category_ID;
            MRequestCategory retValue = (MRequestCategory)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MRequestCategory(ctx, R_Category_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        } 

        /***
         * 	Standard Constructor
         *	@param ctx context
         *	@param R_Category_ID id
         *	@param trxName trx
         */
        public MRequestCategory(Ctx ctx, int R_Category_ID, Trx trxName) :
            base(ctx, R_Category_ID, trxName)
        {
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MRequestCategory(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }	
    }
}
