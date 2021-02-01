/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMessage
 * Purpose        : To return the message from chache
 * Class Used     : X_VAF_Msg_Lable
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
    public class MVAFMsgLable : X_VAF_Msg_Lable
    {
        //Cache
        private static CCache<String, MVAFMsgLable> _cache = new CCache<String, MVAFMsgLable>("VAF_Msg_Lable", 100);

        //Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAFMsgLable).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Msg_Lable_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFMsgLable(Ctx ctx, int VAF_Msg_Lable_ID, Trx trxName)
            : base(ctx, VAF_Msg_Lable_ID, trxName)
        {
        }

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAFMsgLable(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Get Message (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Value">message value</param>
        /// <returns>message</returns>
        public static MVAFMsgLable Get(Ctx ctx, string value)
        {
            if (value == null || value.Length == 0)
                return null;
            MVAFMsgLable retValue = (MVAFMsgLable)_cache[value];
            
            if (retValue == null)
            {
                string sql = "SELECT * FROM VAF_Msg_Lable WHERE Value='" + value + "'";
                DataSet ds = null;
                try
                {
                    ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        retValue = new MVAFMsgLable(ctx, dr, null);
                        
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
        /// <param name="VAF_Msg_Lable_ID">id</param>
        /// <returns>message</returns>
        public static MVAFMsgLable Get(Ctx ctx, int VAF_Msg_Lable_ID)
        {
            string key = VAF_Msg_Lable_ID.ToString();
            MVAFMsgLable retValue = (MVAFMsgLable)_cache[key];
            if (retValue == null)
            {
                retValue = new MVAFMsgLable(ctx, VAF_Msg_Lable_ID, null);
                _cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        ///Get Message ID (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Value">message value</param>
        /// <returns>VAF_Msg_Lable_ID</returns>
        public static int GetVAF_Msg_Lable_ID(Ctx ctx, string value)
        {
            MVAFMsgLable msg = Get(ctx, value);
            if (msg == null)
                return 0;
            return msg.GetVAF_Msg_Lable_ID();
        }

    }
}
