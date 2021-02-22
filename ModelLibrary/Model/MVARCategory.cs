using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class MVARCategory : X_VAR_Category
    {
        /**	Cache						*/
        private static CCache<int, MVARCategory> _cache = new CCache<int, MVARCategory>("VAR_Category", 20);

        /**
         * 	Get MCategory from Cache
         *	@param ctx context
         *	@param VAR_Category_ID id
         *	@return MCategory
         */
        public static MVARCategory Get(Ctx ctx, int VAR_Category_ID)
        {
            int key =VAR_Category_ID;
            MVARCategory retValue = (MVARCategory)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVARCategory(ctx, VAR_Category_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        } 

        /***
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAR_Category_ID id
         *	@param trxName trx
         */
        public MVARCategory(Ctx ctx, int VAR_Category_ID, Trx trxName) :
            base(ctx, VAR_Category_ID, trxName)
        {
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MVARCategory(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }	
    }
}
