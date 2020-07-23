/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MChatType
 * Purpose        : Chat Type Model
 * Class Used     : X_CM_ChatType
 * Chronological    Development
 * Deepak           12-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MChatType : X_CM_ChatType
    {
        /// <summary>
        /// Get MChatType from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_ChatType_ID">id</param>
        /// <returns>MChatType</returns>
        public static MChatType Get(Ctx ctx, int CM_ChatType_ID)
        {
            int key = Utility.Util.GetValueOfInt(CM_ChatType_ID);
            MChatType retValue = (MChatType)s_cache[key];//.get (key);
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MChatType(ctx, CM_ChatType_ID, null);
            if (retValue.Get_ID() != CM_ChatType_ID)
            {
                s_cache.Add(key, retValue);// .put(key, retValue);
            }
            return retValue;
        }	//	get

        /**	Cache						*/
        private static CCache<int, MChatType> s_cache
            = new CCache<int, MChatType>("CM_ChatType", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="CM_ChatType_ID">id</param>
        /// <param name="trxName">trx</param>
        public MChatType(Ctx ctx, int CM_ChatType_ID, Trx trxName)
            : base(ctx, CM_ChatType_ID, trxName)
        {

            if (CM_ChatType_ID == 0)
            {
                SetModerationType(MODERATIONTYPE_NotModerated);
            }
        }	//	MChatType

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MChatType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MChatType
        public MChatType(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }
    }	//	MChatType
}
