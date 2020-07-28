using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;

namespace VAdvantage.Model
{
    public class MSource : X_R_Source
    {
        /**
        * 	Get MSource from Cache
        *	@param ctx context
        *	@param R_Source_ID id
        *	@return MSource
        */
        public static MSource Get(Ctx ctx, int R_Source_ID)
        {
            int key = R_Source_ID;
            MSource retValue = (MSource)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MSource(ctx, R_Source_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        } //	get

        /**	Cache						*/
        private static CCache<int, MSource> _cache
            = new CCache<int, MSource>("R_Source", 20);

        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param R_Source_ID id
         *	@param trxName trx
         */
        public MSource(Ctx ctx, int R_Source_ID, Trx trxName) :
            base(ctx, R_Source_ID, trxName)
        {
            //super(ctx, R_Source_ID, trxName);
        }	//	MSource

        /**
         * 	Load Constructor
         *	@param ctx ctx
         *	@param dr result set
         *	@param trxName trx
         */
        public MSource(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, dr, trxName);
        }	//	MSource

    }
}
