/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductLocator
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     16-Jun-2009
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
    public class MProductLocator : X_VAM_ProductLocator
    {
        /**
         * 	Get array of active Locators for Product and warehouse ordered by priority
         *	@param product product
         *	@param VAM_Warehouse_ID wh
         *	@return product locators
         */
        public static MVAMLocator[] GetLocators(MProduct product, int VAM_Warehouse_ID)
        {
            List<MVAMLocator> list = new List<MVAMLocator>();
            String sql = "SELECT * FROM VAM_Locator l "
                + "WHERE l.IsActive='Y'"
                + " AND (VAM_Locator_ID IN (SELECT VAM_Locator_ID FROM VAM_Product WHERE VAM_Product_ID=" + product.GetVAM_Product_ID() + ")"
                + " OR VAM_Locator_ID IN (SELECT VAM_Locator_ID FROM VAM_ProductLocator WHERE VAM_Product_ID=" + product.GetVAM_Product_ID() + " AND IsActive='Y'))"
                + " AND VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + "ORDER BY PriorityNo DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, product.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMLocator(product.GetCtx(), dr, product.Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log (Level.SEVERE, sql, e);
            }
            finally {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MVAMLocator[] retValue = new MVAMLocator[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get First VAM_Locator_ID for product and Warehouse ordered by priority of Locator
         *	@param product product
         *	@param VAM_Warehouse_ID wh
         *	@return locator or 0 if none
         */
        public static int GetFirstVAM_Locator_ID(MProduct product, int VAM_Warehouse_ID)
        {
            if (product == null || VAM_Warehouse_ID == 0)
                return 0;
            //
            int VAM_Locator_ID = 0;
            String sql = "SELECT VAM_Locator_ID FROM VAM_Locator l "
                + "WHERE l.IsActive='Y'"
                + " AND (VAM_Locator_ID IN (SELECT VAM_Locator_ID FROM VAM_Product WHERE VAM_Product_ID=" + product.GetVAM_Product_ID() + ")"
                + " OR VAM_Locator_ID IN (SELECT VAM_Locator_ID FROM VAM_ProductLocator WHERE VAM_Product_ID=" + product.GetVAM_Product_ID() + " AND IsActive='Y'))"
                + " AND VAM_Warehouse_ID= " + VAM_Warehouse_ID
                + "ORDER BY PriorityNo DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, product.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    VAM_Locator_ID = Convert.ToInt32(dr[0]);// rs.getInt(1);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            return VAM_Locator_ID;
        }


        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductLocator).FullName);

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAM_ProductLocator_ID id
         *	@param trxName trx
         */
        public MProductLocator(Ctx ctx, int VAM_ProductLocator_ID, Trx trxName)
            : base(ctx, VAM_ProductLocator_ID, trxName)
        {

        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MProductLocator(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        // added mohit 20-8-2015 VAWMS
        public static MProductLocator GetOfProductLocator(Ctx ctx, int VAM_Product_ID, int VAM_Locator_ID)
        {
            if (VAM_Product_ID == 0 || VAM_Locator_ID == 0)
                return null;

            int VAM_ProductLocator_ID = 0;
            String sql = "SELECT VAM_ProductLocator_ID FROM VAM_ProductLocator l "
                + "WHERE l.IsActive='Y'"
                + " AND VAM_Locator_ID=" + VAM_Locator_ID
                + " AND VAM_Product_ID=" + VAM_Product_ID;

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null);
                if (idr.Read())
                {
                    VAM_ProductLocator_ID = Util.GetValueOfInt(idr[0]);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            if (VAM_ProductLocator_ID == 0)
            {
                return null;
            }
            return new MProductLocator(ctx, VAM_ProductLocator_ID, null);
        }
        //end

    }
}
