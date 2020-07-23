/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MConversionType
 * Purpose        : Currency Conversion Type using C_ConversionType table
 * Class Used     : X_C_ConversionType
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
    public class MConversionType : X_C_ConversionType
    {
        //Cache Client-ID				
        private static CCache<int, int> s_cache = new CCache<int, int>("C_ConversionType", 4);

        /// <summary>
        ///	Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_ConversionType_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MConversionType(Ctx ctx, int C_ConversionType_ID, Trx trxName)
            : base(ctx, C_ConversionType_ID, trxName)
        {

        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MConversionType(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        ///Get Default Conversion Rate for Client/Org
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <returns>C_ConversionType_ID or 0 if not found</returns>
        public static int GetDefault(int AD_Client_ID)
        {
            //	Try Cache
            int key = AD_Client_ID;
            int ii = (int)s_cache[key];
            if (ii != 0)
            {
                //return ii.intValue();
                return ii;
            }

            //	Get from DB
            int C_ConversionType_ID = 0;
            String sql = "SELECT C_ConversionType_ID "
                + "FROM C_ConversionType "
                + "WHERE IsActive='Y'"
                + " AND AD_Client_ID IN (0, @param1)"		//	#1
                + "ORDER BY IsDefault DESC, AD_Client_ID DESC";
            C_ConversionType_ID = DataBase.DB.GetSQLValue(null, sql, AD_Client_ID);

            //	Return
            s_cache.Add(key, C_ConversionType_ID);
            return C_ConversionType_ID;
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
        //        ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
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
