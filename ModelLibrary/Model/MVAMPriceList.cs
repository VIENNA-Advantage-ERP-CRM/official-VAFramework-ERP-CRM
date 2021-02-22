/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMPriceList
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MVAMPriceList : X_VAM_PriceList
    {
        #region Private Variable
        //Static Logger		
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMPriceList).FullName);
        //private static CLogger 	s_log = CLogger.getCLogger(MVAMPriceList.class);
        // Cache of Price Lists	
        private static CCache<int, MVAMPriceList> _cache = new CCache<int, MVAMPriceList>("VAM_PriceList", 5);
        //	Cached PLV				
        private MVAMPriceListVersion _plv = null;
        // Cached Precision			
        private int? _precision = null;
        #endregion

        /// <summary>
        /// Get Price List (cached)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_PriceList_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>PriceList</returns>
        public static MVAMPriceList Get(Ctx ctx, int VAM_PriceList_ID, Trx trxName)
        {
            int key = VAM_PriceList_ID;
            MVAMPriceList retValue = (MVAMPriceList)_cache[key];
            try
            {
                if (retValue == null)
                {
                    retValue = new MVAMPriceList(ctx, VAM_PriceList_ID, trxName);
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
        public static MVAMPriceList GetDefault(Ctx ctx, bool IsSOPriceList)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            MVAMPriceList retValue = null;
            //	Search for it in cache
            //Iterator<MVAMPriceList> it = _cache.values().iterator();
            IEnumerator<MVAMPriceList> it = _cache.Values.GetEnumerator();
            while (it.MoveNext())
            {
                retValue = (MVAMPriceList)it.Current;
                if (retValue.IsDefault() && retValue.GetVAF_Client_ID() == VAF_Client_ID)
                    return retValue;
            }

            //Get from DB 
            retValue = null;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM VAM_PriceList "
                + "WHERE VAF_Client_ID=" + VAF_Client_ID + " AND IsDefault='Y'");
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
            sql.Append("ORDER BY VAM_PriceList_ID");

            //String sql = "SELECT * FROM VAM_PriceList "
            //    + "WHERE VAF_Client_ID=" + VAF_Client_ID
            //    + " AND IsDefault='Y'"
            //    + " AND IsSOPriceList=?" // YS: Changed from hard code to Parameter
            //    + "ORDER BY VAM_PriceList_ID";
            DataSet ds = null;
            try
            {
                ds = ExecuteQuery.ExecuteDataset(sql.ToString(), null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MVAMPriceList(ctx, dr, null);
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
                int key = retValue.GetVAM_PriceList_ID();
                _cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Get Standard Currency Precision
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="VAM_PriceList_ID">price list</param>
        /// <returns>precision</returns>
        public static int GetStandardPrecision(Ctx ctx, int VAM_PriceList_ID)
        {
            MVAMPriceList pl = MVAMPriceList.Get(ctx, VAM_PriceList_ID, null);
            return pl.GetStandardPrecision();
        }

        /// <summary>
        /// Get Price List Precision
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="VAM_PriceList_ID">price list</param>
        /// <returns>precision</returns>
        public static int GetPricePrecision(Ctx ctx, int VAM_PriceList_ID)
        {
            MVAMPriceList pl = MVAMPriceList.Get(ctx, VAM_PriceList_ID, null);
            return pl.GetPricePrecision();
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_PriceList_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMPriceList(Ctx ctx, int VAM_PriceList_ID, Trx trxName)
            : base(ctx, VAM_PriceList_ID, trxName)
        {
            if (VAM_PriceList_ID == 0)
            {
                SetEnforcePriceLimit(false);
                SetIsDefault(false);
                SetIsSOPriceList(false);
                SetIsTaxIncluded(false);
                SetPricePrecision(2);	// 2
                //	setName (null);
                //	setVAB_Currency_ID (0);
            }
        }

        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAMPriceList(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Price List Version
        /// </summary>
        /// <param name="valid">date where PLV must be valid or today if null</param>
        /// <returns>PLV</returns>
        public MVAMPriceListVersion GetPriceListVersion(DateTime? valid)
        {
            if (valid == null)
                valid = new DateTime(CommonFunctions.CurrentTimeMillis());
            //	Assume there is no later
            //if (_plv != null && _plv.GetValidFrom().before(valid))
            if (_plv != null && _plv.GetValidFrom() < valid)
            {
                return _plv;
            }

            String sql = "SELECT * FROM VAM_PriceListVersion "
                + "WHERE VAM_PriceList_ID=" + GetVAM_PriceList_ID()
                + " AND TRUNC(ValidFrom,'DD')<='" + valid + "' AND IsActive='Y'"
                + "ORDER BY ValidFrom DESC";
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    _plv = new MVAMPriceListVersion(GetCtx(), dr, Get_TrxName());
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            if (_plv == null)
            {
                log.Warning("None found VAM_PriceList_ID="
                 + GetVAM_PriceList_ID() + " - " + valid + " - " + sql);
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
                MVABCurrency c = MVABCurrency.Get(GetCtx(), GetVAB_Currency_ID());
                _precision = c.GetStdPrecision();
            }
            return (int)_precision;
        }
    }
}
