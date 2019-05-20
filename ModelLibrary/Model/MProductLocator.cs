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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProductLocator : X_M_ProductLocator
    {
        /**
         * 	Get array of active Locators for Product and warehouse ordered by priority
         *	@param product product
         *	@param M_Warehouse_ID wh
         *	@return product locators
         */
        public static MLocator[] GetLocators(MProduct product, int M_Warehouse_ID)
        {
            List<MLocator> list = new List<MLocator>();
            String sql = "SELECT * FROM M_Locator l "
                + "WHERE l.IsActive='Y'"
                + " AND (M_Locator_ID IN (SELECT M_Locator_ID FROM M_Product WHERE M_Product_ID=" + product.GetM_Product_ID() + ")"
                + " OR M_Locator_ID IN (SELECT M_Locator_ID FROM M_ProductLocator WHERE M_Product_ID=" + product.GetM_Product_ID() + " AND IsActive='Y'))"
                + " AND M_Warehouse_ID=" + M_Warehouse_ID
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
                    list.Add(new MLocator(product.GetCtx(), dr, product.Get_TrxName()));
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
            MLocator[] retValue = new MLocator[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get First M_Locator_ID for product and Warehouse ordered by priority of Locator
         *	@param product product
         *	@param M_Warehouse_ID wh
         *	@return locator or 0 if none
         */
        public static int GetFirstM_Locator_ID(MProduct product, int M_Warehouse_ID)
        {
            if (product == null || M_Warehouse_ID == 0)
                return 0;
            //
            int M_Locator_ID = 0;
            String sql = "SELECT M_Locator_ID FROM M_Locator l "
                + "WHERE l.IsActive='Y'"
                + " AND (M_Locator_ID IN (SELECT M_Locator_ID FROM M_Product WHERE M_Product_ID=" + product.GetM_Product_ID() + ")"
                + " OR M_Locator_ID IN (SELECT M_Locator_ID FROM M_ProductLocator WHERE M_Product_ID=" + product.GetM_Product_ID() + " AND IsActive='Y'))"
                + " AND M_Warehouse_ID= " + M_Warehouse_ID
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
                    M_Locator_ID = Convert.ToInt32(dr[0]);// rs.getInt(1);
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
            return M_Locator_ID;
        }


        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductLocator).FullName);

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_ProductLocator_ID id
         *	@param trxName trx
         */
        public MProductLocator(Ctx ctx, int M_ProductLocator_ID, Trx trxName)
            : base(ctx, M_ProductLocator_ID, trxName)
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
        public static MProductLocator GetOfProductLocator(Ctx ctx, int M_Product_ID, int M_Locator_ID)
        {
            if (M_Product_ID == 0 || M_Locator_ID == 0)
                return null;

            int M_ProductLocator_ID = 0;
            String sql = "SELECT M_ProductLocator_ID FROM M_ProductLocator l "
                + "WHERE l.IsActive='Y'"
                + " AND M_Locator_ID=" + M_Locator_ID
                + " AND M_Product_ID=" + M_Product_ID;

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null);
                if (idr.Read())
                {
                    M_ProductLocator_ID = Util.GetValueOfInt(idr[0]);
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
            if (M_ProductLocator_ID == 0)
            {
                return null;
            }
            return new MProductLocator(ctx, M_ProductLocator_ID, null);
        }
        //end

    }
}
