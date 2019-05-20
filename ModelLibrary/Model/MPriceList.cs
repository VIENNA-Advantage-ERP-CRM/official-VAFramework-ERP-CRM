/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPriceList
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     04-Jun-2009
  ******************************************************/
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
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MPriceList : X_M_PriceList
    {
        #region Private Variable
        //Static Logger		
        private static VLogger _log = VLogger.GetVLogger(typeof(MPriceList).FullName);
        //private static CLogger 	s_log = CLogger.getCLogger(MPriceList.class);
        // Cache of Price Lists	
        private static CCache<int, MPriceList> _cache = new CCache<int, MPriceList>("M_PriceList", 5);
        //	Cached PLV				
        private MPriceListVersion _plv = null;
        // Cached Precision			
        private int? _precision = null;
        #endregion

        /// <summary>
        /// Get Price List (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PriceList_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>PriceList</returns>
        public static MPriceList Get(Ctx ctx, int M_PriceList_ID, Trx trxName)
        {
            int key = M_PriceList_ID;
            MPriceList retValue = (MPriceList)_cache[key];
            try
            {
                if (retValue == null)
                {
                    retValue = new MPriceList(ctx, M_PriceList_ID, trxName);
                    _cache.Add(key, retValue);
                }
            }
            catch 
            {
               
            }
            return retValue;
        }

        /// <summary>
        /// Get Default Price List for Client (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="IsSOPriceList">SO or PO</param>
        /// <returns>PriceList or null</returns>
        public static MPriceList GetDefault(Ctx ctx, bool IsSOPriceList)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            MPriceList retValue = null;
            //	Search for it in cache
            //Iterator<MPriceList> it = _cache.values().iterator();
            IEnumerator<MPriceList> it = _cache.Values.GetEnumerator();
            while (it.MoveNext())
            {
                retValue = (MPriceList)it.Current;
                if (retValue.IsDefault() && retValue.GetAD_Client_ID() == AD_Client_ID)
                    return retValue;
            }

            //Get from DB 
            retValue = null;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM M_PriceList "
                + "WHERE AD_Client_ID=" + AD_Client_ID + " AND IsDefault='Y'");
            if (IsSOPriceList)
            {
                //pstmt.setString(2, "Y");
                sql.Append(" AND IsSOPriceList='Y'"); // YS: Changed from hard code to Parameter
            }
            else
            {
                //pstmt.setString(2, "N");
                sql.Append(" AND IsSOPriceList='N'"); // YS: Changed from hard code to Parameter
            }
            sql.Append("ORDER BY M_PriceList_ID");

            //String sql = "SELECT * FROM M_PriceList "
            //    + "WHERE AD_Client_ID=" + AD_Client_ID
            //    + " AND IsDefault='Y'"
            //    + " AND IsSOPriceList=?" // YS: Changed from hard code to Parameter
            //    + "ORDER BY M_PriceList_ID";
            DataSet ds = null;
            try
            {
                ds = ExecuteQuery.ExecuteDataset(sql.ToString(), null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MPriceList(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
                ds = null;
                _log.Log(Level.SEVERE, sql.ToString(), e);
            }
            //	Return value
            if (retValue != null)
            {
                int key = retValue.GetM_PriceList_ID();
                _cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Get Standard Currency Precision
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="M_PriceList_ID">price list</param>
        /// <returns>precision</returns>
        public static int GetStandardPrecision(Ctx ctx, int M_PriceList_ID)
        {
            MPriceList pl = MPriceList.Get(ctx, M_PriceList_ID, null);
            return pl.GetStandardPrecision();
        }

        /// <summary>
        /// Get Price List Precision
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="M_PriceList_ID">price list</param>
        /// <returns>precision</returns>
        public static int GetPricePrecision(Ctx ctx, int M_PriceList_ID)
        {
            MPriceList pl = MPriceList.Get(ctx, M_PriceList_ID, null);
            return pl.GetPricePrecision();
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PriceList_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPriceList(Ctx ctx, int M_PriceList_ID, Trx trxName)
            : base(ctx, M_PriceList_ID, trxName)
        {
            if (M_PriceList_ID == 0)
            {
                SetEnforcePriceLimit(false);
                SetIsDefault(false);
                SetIsSOPriceList(false);
                SetIsTaxIncluded(false);
                SetPricePrecision(2);	// 2
                //	setName (null);
                //	setC_Currency_ID (0);
            }
        }

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MPriceList(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Price List Version
        /// </summary>
        /// <param name="valid">date where PLV must be valid or today if null</param>
        /// <returns>PLV</returns>
        public MPriceListVersion GetPriceListVersion(DateTime? valid)
        {
            if (valid == null)
                valid = new DateTime(CommonFunctions.CurrentTimeMillis());
            //	Assume there is no later
            //if (_plv != null && _plv.GetValidFrom().before(valid))
            if (_plv != null && _plv.GetValidFrom() < valid)
            {
                return _plv;
            }

            String sql = "SELECT * FROM M_PriceList_Version "
                + "WHERE M_PriceList_ID=" + GetM_PriceList_ID()
                + " AND TRUNC(ValidFrom,'DD')<='" + valid + "' AND IsActive='Y'"
                + "ORDER BY ValidFrom DESC";
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    _plv = new MPriceListVersion(GetCtx(), dr, Get_TrxName());
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            if (_plv == null)
            {
                log.Warning("None found M_PriceList_ID="
                 + GetM_PriceList_ID() + " - " + valid + " - " + sql);
            }
            else
            {
                log.Fine(_plv.ToString());
            }
            return _plv;
        }

        /// <summary>
        /// Get Standard Currency Precision
        /// </summary>
        /// <returns>precision</returns>
        public int GetStandardPrecision()
        {
            if (_precision == null)
            {
                MCurrency c = MCurrency.Get(GetCtx(), GetC_Currency_ID());
                _precision = c.GetStdPrecision();
            }
            return (int)_precision;
        }
    }
}
