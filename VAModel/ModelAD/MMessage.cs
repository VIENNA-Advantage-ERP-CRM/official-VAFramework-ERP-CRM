/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMessage
 * Purpose        : To return the message from chache
 * Class Used     : X_AD_Message
 * Chronological    Development
 * Raghunandan      27-04-2009
  ******************************************************/
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MMessage : X_AD_Message
    {
        //Cache
        private static CCache<String, MMessage> _cache = new CCache<String, MMessage>("AD_Message", 100);

        //Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MMessage).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Message_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MMessage(Ctx ctx, int AD_Message_ID, Trx trxName)
            : base(ctx, AD_Message_ID, trxName)
        {
        }

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MMessage(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get Message (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Value">message value</param>
        /// <returns>message</returns>
        public static MMessage Get(Ctx ctx, string value)
        {
            if (value == null || value.Length == 0)
                return null;
            MMessage retValue = (MMessage)_cache[value];
            
            if (retValue == null)
            {
                string sql = "SELECT * FROM AD_Message WHERE Value='" + value + "'";
                DataSet ds = null;
                try
                {
                    ds = DataBase.DB.ExecuteDataset(sql, null, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        retValue = new MMessage(ctx, dr, null);
                        
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                   _log.Log(Level.SEVERE, "get", e);
                }

                if (retValue != null)
                {
                    _cache.Add(value, retValue);
                }
            }
            return retValue;
        }	

        /// <summary>
        ///Get Message (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Message_ID">id</param>
        /// <returns>message</returns>
        public static MMessage Get(Ctx ctx, int AD_Message_ID)
        {
            string key = AD_Message_ID.ToString();
            MMessage retValue = (MMessage)_cache[key];
            if (retValue == null)
            {
                retValue = new MMessage(ctx, AD_Message_ID, null);
                _cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        ///Get Message ID (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Value">message value</param>
        /// <returns>AD_Message_ID</returns>
        public static int GetAD_Message_ID(Ctx ctx, string value)
        {
            MMessage msg = Get(ctx, value);
            if (msg == null)
                return 0;
            return msg.GetAD_Message_ID();
        }

    }
}
