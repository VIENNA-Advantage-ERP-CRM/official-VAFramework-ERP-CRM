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
    public class MVACMChatType : X_VACM_ChatType
    {
        /// <summary>
        /// Get MChatType from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VACM_ChatType_ID">id</param>
        /// <returns>MChatType</returns>
        public static MVACMChatType Get(Ctx ctx, int VACM_ChatType_ID)
        {
            int key = Utility.Util.GetValueOfInt(VACM_ChatType_ID);
            MVACMChatType retValue = (MVACMChatType)s_cache[key];//.get (key);
            if (retValue != null)
            {
                return retValue;
            }
            retValue = new MVACMChatType(ctx, VACM_ChatType_ID, null);
            if (retValue.Get_ID() != VACM_ChatType_ID)
            {
                s_cache.Add(key, retValue);// .put(key, retValue);
            }
            return retValue;
        }	//	get

        /**	Cache						*/
        private static CCache<int, MVACMChatType> s_cache
            = new CCache<int, MVACMChatType>("VACM_ChatType", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VACM_ChatType_ID">id</param>
        /// <param name="trxName">trx</param>
        public MVACMChatType(Ctx ctx, int VACM_ChatType_ID, Trx trxName)
            : base(ctx, VACM_ChatType_ID, trxName)
        {

            if (VACM_ChatType_ID == 0)
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
        public MVACMChatType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MChatType
        public MVACMChatType(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        { }
    }	//	MChatType
}
