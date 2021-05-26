/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductPrice
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MProductPrice : X_M_ProductPrice
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductPrice).FullName);

        /// <summary>
        /// Get Product Price
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PriceList_Version_ID">id</param>
        /// <param name="M_Product_ID">id</param>
        /// <param name="trxName">transction</param>
        /// <returns>product price or null</returns>
        public static MProductPrice Get(Ctx ctx, int M_PriceList_Version_ID, int M_Product_ID, Trx trxName)
        {

            MProductPrice retValue = null;
            String sql = "SELECT * FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + M_PriceList_Version_ID + " AND M_Product_ID=" + M_Product_ID;
            DataSet ds = new DataSet();
            try
            {
                //ds = DataBase.DB.ExecuteQuery (sql, trxName);
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MProductPrice(ctx, dr, trxName);
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
        /// <param name="C_UOM_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_PriceList_Version_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public static MProductPrice Get(Ctx ctx, int C_UOM_ID, int M_Product_ID, int M_PriceList_Version_ID,
             int M_AttributeSetInstance_ID, Trx trxName)
        {
            MProductPrice retValue = null;
            String sql = " SELECT * FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + M_PriceList_Version_ID + " AND C_UOM_ID = " + C_UOM_ID
                + " AND M_AttributeSetInstance_ID =" + M_AttributeSetInstance_ID + " AND M_Product_ID=" + M_Product_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MProductPrice(ctx, dr, trxName);
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
        /// <param name="M_Product_ID"></param>
        /// <param name="M_PriceList_Version_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="trxName"></param>
        /// <returns></returns>
        public static MProductPrice Get(Ctx ctx, int M_Product_ID, int M_PriceList_Version_ID,
             int M_AttributeSetInstance_ID, Trx trxName)
        {
            MProductPrice retValue = null;
            String sql = " SELECT * FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + M_PriceList_Version_ID
                + " AND M_AttributeSetInstance_ID =" + M_AttributeSetInstance_ID + " AND M_Product_ID=" + M_Product_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MProductPrice(ctx, dr, trxName);
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
        public MProductPrice(Ctx ctx, int ignored, Trx trxName)
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
        public MProductPrice(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PriceList_Version_ID">Price List Version</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="trxName">transaction</param>
        public MProductPrice(Ctx ctx, int M_PriceList_Version_ID, int M_Product_ID, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetM_PriceList_Version_ID(M_PriceList_Version_ID);	//	FK
            SetM_Product_ID(M_Product_ID);						//	FK
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PriceList_Version_ID">Price List Version</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="PriceList">list price</param>
        /// <param name="PriceStd">standard price</param>
        /// <param name="PriceLimit">limit price</param>
        /// <param name="trxName">transaction</param>
        public MProductPrice(Ctx ctx, int M_PriceList_Version_ID, int M_Product_ID,
            Decimal PriceList, Decimal PriceStd, Decimal PriceLimit, Trx trxName)
            : this(ctx, M_PriceList_Version_ID, M_Product_ID, trxName)
        {
            SetPrices(PriceList, PriceStd, PriceLimit);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="plv">list version</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="PriceList">list price</param>
        /// <param name="PriceStd">standard price</param>
        /// <param name="PriceLimit">limit price</param>
        public MProductPrice(MPriceListVersion plv, int M_Product_ID,
            Decimal PriceList, Decimal PriceStd, Decimal PriceLimit)
            : this(plv.GetCtx(), 0, plv.Get_TrxName())
        {
            SetClientOrg(plv);
            SetM_PriceList_Version_ID(plv.GetM_PriceList_Version_ID());
            SetM_Product_ID(M_Product_ID);
            SetPrices(PriceList, PriceStd, PriceLimit);
        }

        /// <summary>
        /// Before Save Logic Implement
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>true, when success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            SetPrices(GetPriceList(), GetPriceStd(), GetPriceLimit());
            return true;
        }

        /// <summary>
        /// Set Prices
        /// </summary>
        /// <param name="PriceList">list price</param>
        /// <param name="PriceStd">standard price</param>
        /// <param name="PriceLimit">limit price</param>
        public void SetPrices(Decimal PriceList, Decimal PriceStd, Decimal PriceLimit)
        {
            int PricePrecision = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT PricePrecision FROM M_PriceList WHERE M_PriceList_ID =
                                (SELECT M_PriceList_ID FROM M_PriceList_Version WHERE M_PriceList_Version_ID = " + GetM_PriceList_Version_ID() + ")", null, Get_Trx()));
            SetPriceLimit(Decimal.Round(PriceLimit, PricePrecision, MidpointRounding.AwayFromZero));
            SetPriceList(Decimal.Round(PriceList, PricePrecision, MidpointRounding.AwayFromZero));
            SetPriceStd(Decimal.Round(PriceStd, PricePrecision, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProductPrice[");
            sb.Append(GetM_PriceList_Version_ID())
                .Append(",M_Product_ID=").Append(GetM_Product_ID())
                .Append(",PriceList=").Append(GetPriceList())
                .Append("]");
            return sb.ToString();
        }

    }
}
