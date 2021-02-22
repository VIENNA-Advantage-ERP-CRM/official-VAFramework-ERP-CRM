/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMProductPrice
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     09-Jun-2009
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
    public class MVAMProductPrice : X_VAM_ProductPrice
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMProductPrice).FullName);

        /// <summary>
        /// Get Product Price
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_PriceListVersion_ID">id</param>
        /// <param name="VAM_Product_ID">id</param>
        /// <param name="trxName">transction</param>
        /// <returns>product price or null</returns>
        public static MVAMProductPrice Get(Ctx ctx, int VAM_PriceListVersion_ID, int VAM_Product_ID, Trx trxName)
        {

            MVAMProductPrice retValue = null;
            String sql = "SELECT * FROM VAM_ProductPrice WHERE VAM_PriceListVersion_ID=" + VAM_PriceListVersion_ID + " AND VAM_Product_ID=" + VAM_Product_ID;
            DataSet ds = new DataSet();
            try
            {
                //ds = DataBase.DB.ExecuteQuery (sql, trxName);
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MVAMProductPrice(ctx, dr, trxName);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        /// Get Product Price (Only in case when UOM Pricing and Vienna Advance Pricing Modules are available)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_UOM_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PriceListVersion_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public static MVAMProductPrice Get(Ctx ctx, int VAB_UOM_ID, int VAM_Product_ID, int VAM_PriceListVersion_ID,
             int VAM_PFeature_SetInstance_ID, Trx trxName)
        {
            MVAMProductPrice retValue = null;
            String sql = " SELECT * FROM VAM_ProductPrice WHERE VAM_PriceListVersion_ID=" + VAM_PriceListVersion_ID + " AND VAB_UOM_ID = " + VAB_UOM_ID
                + " AND VAM_PFeature_SetInstance_ID =" + VAM_PFeature_SetInstance_ID + " AND VAM_Product_ID=" + VAM_Product_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MVAMProductPrice(ctx, dr, trxName);
                }
                ds = null;
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }

            return retValue;
        }

        /// <summary>
        /// Get Product Price (Only in case when Vienna Advance Pricing Module is available)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PriceListVersion_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public static MVAMProductPrice Get(Ctx ctx, int VAM_Product_ID, int VAM_PriceListVersion_ID,
             int VAM_PFeature_SetInstance_ID, Trx trxName)
        {
            MVAMProductPrice retValue = null;
            String sql = " SELECT * FROM VAM_ProductPrice WHERE VAM_PriceListVersion_ID=" + VAM_PriceListVersion_ID
                + " AND VAM_PFeature_SetInstance_ID =" + VAM_PFeature_SetInstance_ID + " AND VAM_Product_ID=" + VAM_Product_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MVAMProductPrice(ctx, dr, trxName);
                }
                ds = null;
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }

            return retValue;
        }


        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductPrice(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            SetPriceLimit(Env.ZERO);
            SetPriceList(Env.ZERO);
            SetPriceStd(Env.ZERO);
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductPrice(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_PriceListVersion_ID">Price List Version</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductPrice(Ctx ctx, int VAM_PriceListVersion_ID, int VAM_Product_ID, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);	//	FK
            SetVAM_Product_ID(VAM_Product_ID);						//	FK
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_PriceListVersion_ID">Price List Version</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="PriceList">list price</param>
        /// <param name="PriceStd">standard price</param>
        /// <param name="PriceLimit">limit price</param>
        /// <param name="trxName">transaction</param>
        public MVAMProductPrice(Ctx ctx, int VAM_PriceListVersion_ID, int VAM_Product_ID,
            Decimal PriceList, Decimal PriceStd, Decimal PriceLimit, Trx trxName)
            : this(ctx, VAM_PriceListVersion_ID, VAM_Product_ID, trxName)
        {
            SetPrices(PriceList, PriceStd, PriceLimit);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="plv">list version</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="PriceList">list price</param>
        /// <param name="PriceStd">standard price</param>
        /// <param name="PriceLimit">limit price</param>
        public MVAMProductPrice(MVAMPriceListVersion plv, int VAM_Product_ID,
            Decimal PriceList, Decimal PriceStd, Decimal PriceLimit)
            : this(plv.GetCtx(), 0, plv.Get_TrxName())
        {
            SetClientOrg(plv);
            SetVAM_PriceListVersion_ID(plv.GetVAM_PriceListVersion_ID());
            SetVAM_Product_ID(VAM_Product_ID);
            SetPrices(PriceList, PriceStd, PriceLimit);
        }

        /// <summary>
        /// Set Prices
        /// </summary>
        /// <param name="PriceList">list price</param>
        /// <param name="PriceStd">standard price</param>
        /// <param name="PriceLimit">limit price</param>
        public void SetPrices(Decimal PriceList, Decimal PriceStd, Decimal PriceLimit)
        {
            SetPriceLimit(PriceLimit);
            SetPriceList(PriceList);
            SetPriceStd(PriceStd);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMProductPrice[");
            sb.Append(GetVAM_PriceListVersion_ID())
                .Append(",VAM_Product_ID=").Append(GetVAM_Product_ID())
                .Append(",PriceList=").Append(GetPriceList())
                .Append("]");
            return sb.ToString();
        }

    }
}
