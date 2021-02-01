/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MConversionType
 * Purpose        : Currency Conversion Type using VAB_CurrencyType table
 * Class Used     : X_VAB_CurrencyType
 * Chronological    Development
 * Raghunandan      28-04-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Common;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABCurrencyType : X_VAB_CurrencyType
    {
        //Cache Client-ID				
        private static CCache<int, int> s_cache = new CCache<int, int>("VAB_CurrencyType", 4);

        /// <summary>
        ///	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_CurrencyType_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABCurrencyType(Ctx ctx, int VAB_CurrencyType_ID, Trx trxName)
            : base(ctx, VAB_CurrencyType_ID, trxName)
        {

        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVABCurrencyType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        ///Get Default Conversion Rate for Client/Org
        /// </summary>
        /// <param name="VAF_Client_ID">client</param>
        /// <returns>VAB_CurrencyType_ID or 0 if not found</returns>
        public static int GetDefault(int VAF_Client_ID)
        {
            //	Try Cache
            int key = VAF_Client_ID;
            int ii = (int)s_cache[key];
            if (ii != 0)
            {
                //return ii.intValue();
                return ii;
            }

            //	Get from DB
            int VAB_CurrencyType_ID = 0;
            String sql = "SELECT VAB_CurrencyType_ID "
                + "FROM VAB_CurrencyType "
                + "WHERE IsActive='Y'"
                + " AND VAF_Client_ID IN (0, @param1)"		//	#1
                + "ORDER BY IsDefault DESC, VAF_Client_ID DESC";
            VAB_CurrencyType_ID = CoreLibrary.DataBase.DB.GetSQLValue(null, sql, VAF_Client_ID);

            //	Return
            s_cache.Add(key, VAB_CurrencyType_ID);
            return VAB_CurrencyType_ID;
        }

        /// <summary>
        /// return id
        /// </summary>
        /// <param name="trxName">transaction</param>
        /// <param name="sql">Query</param>
        /// <returns>return id</returns>
        //public static int GetSQLValue(string trxName, string sql)
        //{
        //    int retValue = -1;
        //    DataSet ds = null;
        //    try
        //    {
        //        ds = BaseLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            DataRow dr = ds.Tables[0].Rows[i];
        //            //retValue = dr.getInt(1);
        //            retValue = Utility.Util.GetValueOfInt(dr[0].ToString());
        //        }
        //        ds = null;
        //    }
        //    catch (Exception e)
        //    {
        //        log.log(Level.SEVERE, sql + " - Param1=" + str_param1, e);
        //    }
        //    finally
        //    {
        //        ds = null;
        //    }
        //    return retValue;
        //}
    }
}
