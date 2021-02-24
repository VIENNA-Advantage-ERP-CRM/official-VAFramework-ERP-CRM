/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_Storage
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
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MVAMStorage : X_VAM_Storage
    {
        #region Private variables
        // Log		
        private static VLogger s_log = VLogger.GetVLogger(typeof(MVAMStorage).FullName);

        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMStorage).FullName);
        //private static CLogger		s_log = CLogger.getCLogger (MVAMStorage.class);
        // Warehouse					
        private int _VAM_Warehouse_ID = 0;
        static Tuple<String, String, String> mInfo1 = null;
        #endregion


        public static MVAMStorage Get(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, Trx trxName)
        {
            return Get(ctx, VAM_Locator_ID, VAM_Product_ID,
              VAM_PFeature_SetInstance_ID, true, trxName);
        }



        /*	Get Storage Info
        *	@param ctx context
        *	@param VAM_Locator_ID locator
        *	@param VAM_Product_ID product
        *	@param VAM_PFeature_SetInstance_ID instance
        *	@param trxName transaction
        *	@return existing or null
        */
        private static MVAMStorage Get(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
             int VAM_PFeature_SetInstance_ID, bool forUpdate, Trx trxName)
        {
            MVAMStorage retValue = null;
            String sql = "SELECT * FROM VAM_Storage "
                + "WHERE VAM_Locator_ID=" + VAM_Locator_ID + " AND VAM_Product_ID=" + VAM_Product_ID + " AND ";
            if (VAM_PFeature_SetInstance_ID == 0)
                sql += "(VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR VAM_PFeature_SetInstance_ID IS NULL)";
            else
                sql += "VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            if (forUpdate)
                sql += " FOR UPDATE";

            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVAMStorage(ctx, dr, trxName);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            if (retValue == null)
            {
                _log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                    + ", VAM_Product_ID=" + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID);
            }
            else
            {
                _log.Fine("VAM_Locator_ID=" + VAM_Locator_ID
                    + ", VAM_Product_ID=" + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID);
            }
            return retValue;
        }

        /**
       * 	Get all Storages for Product with ASI
       *	@param ctx context
       *	@param VAM_Product_ID product
       *	@param VAM_Locator_ID locator
       *	@param FiFo first in-first-out
       *	@param trxName transaction
       *	@return existing or null
       */
        public static MVAMStorage[] GetAllWithASI(Ctx ctx, int VAM_Product_ID, int VAM_Locator_ID,
            bool FiFo, Trx trxName)
        {
            List<MVAMStorage> list = new List<MVAMStorage>();
            String sql = "SELECT * FROM VAM_Storage "
                + "WHERE VAM_Product_ID=" + VAM_Product_ID + " AND VAM_Locator_ID=" + VAM_Locator_ID
                + " AND VAM_PFeature_SetInstance_ID > 0"
                + " AND QtyOnHand > 0 "
                + "ORDER BY VAM_PFeature_SetInstance_ID";
            if (!FiFo)
                sql += " DESC";

            sql += " FOR UPDATE ";

            DataSet ds = new DataSet();
            try
            {
                ds = DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMStorage(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;
            MVAMStorage[] retValue = new MVAMStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
       * 	Get all Storages for Product without ASI
       *	@param ctx context
       *	@param VAM_Product_ID product
       *	@param VAM_Locator_ID locator
       *	@param FiFo first in-first-out
       *	@param trxName transaction
       *	@return existing or null
       */
        public static MVAMStorage[] GetAllWithoutASI(Ctx ctx, int VAM_Product_ID, int VAM_Locator_ID,
            bool FiFo, Trx trxName)
        {
            List<MVAMStorage> list = new List<MVAMStorage>();
            String sql = "SELECT * FROM VAM_Storage "
                + "WHERE VAM_Product_ID=" + VAM_Product_ID + " AND VAM_Locator_ID=" + VAM_Locator_ID
                + " AND NVL(VAM_PFeature_SetInstance_ID , 0) = 0"
                + " AND QtyOnHand > 0 "
                + "ORDER BY VAM_PFeature_SetInstance_ID";
            if (!FiFo)
                sql += " DESC";
            DataSet ds = new DataSet();
            try
            {
                ds = DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMStorage(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;
            MVAMStorage[] retValue = new MVAMStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get all Storages for Product
        *	@param ctx context
        *	@param VAM_Product_ID product
        *	@param VAM_Locator_ID locator
        *	@param trxName transaction
        *	@return existing or null
        */
        public static MVAMStorage[] GetAll(Ctx ctx, int VAM_Product_ID, int VAM_Locator_ID, Trx trxName)
        {
            List<MVAMStorage> list = new List<MVAMStorage>();
            String sql = "SELECT * FROM VAM_Storage "
                + "WHERE VAM_Product_ID=" + VAM_Product_ID + " AND VAM_Locator_ID=" + VAM_Locator_ID
                + " AND QtyOnHand <> 0 "
                + "ORDER BY VAM_PFeature_SetInstance_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMStorage(ctx, dr, trxName));
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MVAMStorage[] retValue = new MVAMStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get Storage Info for Product across warehouses
        *	@param ctx context
        *	@param VAM_Product_ID product
        *	@param trxName transaction
        *	@return existing or null
        */
        public static MVAMStorage[] GetOfProduct(Ctx ctx, int VAM_Product_ID, Trx trxName)
        {
            List<MVAMStorage> list = new List<MVAMStorage>();
            String sql = "SELECT * FROM VAM_Storage "
                + "WHERE VAM_Product_ID=" + VAM_Product_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMStorage(ctx, dr, trxName));
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MVAMStorage[] retValue = new MVAMStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /* 	Get Storage Info for Warehouse
       *	@param Ctx context
       *	@param VAM_Warehouse_ID 
       *	@param VAM_Product_ID product
       *	@param VAM_PFeature_SetInstance_ID instance
       *	@param VAM_PFeature_Set_ID attribute set
       *	@param allAttributeInstances if true, all attribute set instances
       *	@param minGuaranteeDate optional minimum guarantee date if all attribute instances
       *	@param FiFo first in-first-out
       *	@param trxName transaction
       *	@return existing - ordered by location priority (desc) and/or guarantee date
       */
        public static MVAMStorage[] GetWarehouse(Ctx Ctx, int VAM_Warehouse_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
            bool allAttributeInstances, DateTime? minGuaranteeDate,
            bool FiFo, Trx trxName)
        {
            if (VAM_Warehouse_ID == 0 || VAM_Product_ID == 0)
                return new MVAMStorage[0];

            if (VAM_PFeature_Set_ID == 0)
                allAttributeInstances = true;
            else
            {
                MVAMPFeatureSet mas = MVAMPFeatureSet.Get(Ctx, VAM_PFeature_Set_ID);
                if (!mas.IsInstanceAttribute())
                    allAttributeInstances = true;
            }

            List<MVAMStorage> list = new List<MVAMStorage>();
            //	Specific Attribute Set Instance
            String sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
                + "s.VAF_Client_ID,s.VAF_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID) "
                + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + " AND s.VAM_Product_ID=" + VAM_Product_ID
                + " AND COALESCE(s.VAM_PFeature_SetInstance_ID,0)= " + VAM_PFeature_SetInstance_ID
                + "ORDER BY l.PriorityNo DESC, VAM_PFeature_SetInstance_ID";
            if (!FiFo)
            {
                sql += " DESC";
            }
            //	All Attribute Set Instances
            if (allAttributeInstances)
            {
                sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
                    + "s.VAF_Client_ID,s.VAF_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                    + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
                    + "FROM VAM_Storage s"
                    + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID)"
                    + " LEFT OUTER JOIN VAM_PFeature_SetInstance asi ON (s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) "
                    + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                    + " AND s.VAM_Product_ID=" + VAM_Product_ID;
                if (minGuaranteeDate != null)
                {
                    //sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>'" + String.Format("{0:dd-MMM-yy}", minGuaranteeDate) + "') "
                    //    + "ORDER BY asi.GuaranteeDate, VAM_PFeature_SetInstance_ID";	//	Has Prio over Locator
                    sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>" + GlobalVariable.TO_DATE(minGuaranteeDate, true) + ")"         // Changes done by Bharat for Italian Language
                        + "ORDER BY asi.GuaranteeDate, VAM_PFeature_SetInstance_ID";	//	Has Prio over Locator
                    if (!FiFo)
                        sql += " DESC";
                    sql += ", l.PriorityNo DESC, s.QtyOnHand DESC";
                }
                else
                {
                    sql += "ORDER BY l.PriorityNo DESC, l.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID";
                    if (!FiFo)
                        sql += " DESC";
                    sql += ", s.QtyOnHand DESC";
                }
            }
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMStorage(Ctx, dr, trxName));
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
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MVAMStorage[] retValue = new MVAMStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        //Added by Mohit 20-8-2015 VAWMS
        public static List<MVAMStorage> GetWarehouse(Ctx ctx, int VAM_Warehouse_ID,
         int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
         bool allAttributeInstances, DateTime? minGuaranteeDate,
         bool FiFo, Boolean allocationCheck,
              int M_SourceZone_ID, Trx trxName, bool CreateWave)
        {
            if (VAM_Warehouse_ID == 0 || VAM_Product_ID == 0)
                return null;

            if (VAM_PFeature_Set_ID == 0)
            {
                allAttributeInstances = true;
            }
            else
            {
                MVAMPFeatureSet mas = MVAMPFeatureSet.Get(ctx, VAM_PFeature_Set_ID);
                if (!mas.IsInstanceAttribute())
                    allAttributeInstances = true;
            }

            List<MVAMStorage> list = new List<MVAMStorage>();
            String sql = null;
            // All Attribute Set Instances
            if (allAttributeInstances)
            {
                sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
                        + "QtyOnhand,"
                        + "QtyDedicated,"
                        + "QtyAllocated "
                        + " FROM VAM_Storage s"
                        + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID)"
                        + " LEFT OUTER JOIN VAM_PFeature_SetInstance asi ON (s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) "
                        + " WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID + " AND s.VAM_Product_ID= " + VAM_Product_ID;

                if (allocationCheck)
                {
                    sql += " AND l.IsAvailableForAllocation='Y' ";
                }

                if (M_SourceZone_ID != 0)
                {
                    sql += " AND l.VAM_Locator_ID IN "
                            + " (SELECT VAM_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID =" + M_SourceZone_ID + "  ) ";
                }

                if (minGuaranteeDate != null)
                {
                    sql += " AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>'" + String.Format("{0:dd-MMM-yy}", minGuaranteeDate) + "') "
                        // + " GROUP BY asi.GuaranteeDate, l.PriorityNo, s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID "
                            + " ORDER BY asi.GuaranteeDate, l.PriorityNo DESC, VAM_PFeature_SetInstance_ID";
                }
                else
                {
                    // sql += "GROUP BY l.PriorityNo, s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID "
                    sql += " ORDER BY l.PriorityNo DESC, s.VAM_PFeature_SetInstance_ID";
                }
                if (!FiFo)
                {
                    sql += " DESC";
                }
                sql += ", QtyOnHand DESC";
            }
            else
            {
                // Specific Attribute Set Instance
                sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
                        + " QtyOnhand,"
                        + "QtyDedicated,"
                        + " QtyAllocated "
                        + " FROM VAM_Storage s"
                        + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID) "
                        + " WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                        + " AND s.VAM_Product_ID=" + VAM_Product_ID
                        + " AND COALESCE(s.VAM_PFeature_SetInstance_ID,0)=" + VAM_PFeature_SetInstance_ID;

                if (allocationCheck)
                {
                    sql += " AND l.IsAvailableForAllocation='Y' ";
                }

                if (M_SourceZone_ID != 0)
                {
                    sql += " AND l.VAM_Locator_ID IN "
                            + " (SELECT VAM_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID =" + M_SourceZone_ID + " ) ";
                }
                //sql += "GROUP BY l.PriorityNo, s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID "
                sql += " ORDER BY l.PriorityNo DESC, VAM_PFeature_SetInstance_ID";

                if (!FiFo)
                {
                    sql += " DESC";
                }
            }
            IDataReader idr = null;
            DataTable dt = new DataTable();
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt.Load(idr);
                idr.Close();
                //while (idr.Read())
                //{
                //    index = 0;
                //    int rs_VAM_Product_ID = Util.GetValueOfInt(idr[index++]);
                //    int rs_VAM_Locator_ID = Util.GetValueOfInt(idr[index++]);
                //    int rs_VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(idr[index++]);
                //    Decimal rs_QtyOnhand = Util.GetValueOfDecimal(idr[index++]);
                //    Decimal rs_QtyDedicated = Util.GetValueOfDecimal(idr[index++]);
                //    Decimal rs_QtyAllocated = Util.GetValueOfDecimal(idr[index++]);
                //    list.Add(new MVAMStorage(rs_VAM_Locator_ID, rs_VAM_Product_ID,
                //            rs_VAM_PFeature_SetInstance_ID,
                //            rs_QtyOnhand,
                //            rs_QtyDedicated, rs_QtyAllocated));
                //}
                //idr.Close();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(new MVAMStorage(ctx, dt.Rows[i], trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            return list;
        }
        //End

        /*	Create or Get Storage Info
        *	@param ctx context
        *	@param VAM_Locator_ID locator
        *	@param VAM_Product_ID product
        *	@param VAM_PFeature_SetInstance_ID instance
        *	@param trxName transaction
        *	@return existing/new or null
        */
        public static MVAMStorage GetCreate(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
             int VAM_PFeature_SetInstance_ID, Trx trxName)
        {
            if (VAM_Locator_ID == 0)
            {
                throw new ArgumentException("VAM_Locator_ID=0");
            }
            if (VAM_Product_ID == 0)
            {
                throw new ArgumentException("VAM_Product_ID=0");
            }
            MVAMStorage retValue = Get(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
            if (retValue != null)
                return retValue;

            //	Insert row based on locator
            MVAMLocator locator = new MVAMLocator(ctx, VAM_Locator_ID, trxName);
            if (locator.Get_ID() != VAM_Locator_ID)
            {
                throw new ArgumentException("Not found VAM_Locator_ID=" + VAM_Locator_ID);
            }
            retValue = new MVAMStorage(locator, VAM_Product_ID, VAM_PFeature_SetInstance_ID);
            retValue.Save(trxName);
            _log.Fine("New " + retValue);
            return retValue;
        }

        /* 	Update Storage Info add.
       * 	Called from MProjectIssue
       *	@param Ctx context
       *	@param VAM_Warehouse_ID warehouse
       *	@param VAM_Locator_ID locator
       *	@param VAM_Product_ID product
       *	@param VAM_PFeature_SetInstance_ID AS Instance
       *	@param reservationAttributeSetInstance_ID reservation AS Instance
       *	@param diffQtyOnHand add on hand
       *	@param diffQtyReserved add reserved
       *	@param diffQtyOrdered add order
       *	@param trxName transaction
       *	@return true if updated
       */

        public static bool Add(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int reservationAttributeSetInstance_ID,
            Decimal? diffQtyOnHand,
            Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Trx trxName)
        {
            MVAMStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
                && storage.GetVAM_Product_ID() != VAM_Product_ID
                && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
            {
                _log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
                + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI=" + VAM_PFeature_SetInstance_ID);
                return false;
            }
            MVAMStorage storageASI = null;
            if (VAM_PFeature_SetInstance_ID != reservationAttributeSetInstance_ID && reservationAttributeSetInstance_ID != 0)
            {
                int reservationVAM_Locator_ID = VAM_Locator_ID;
                //if (reservationAttributeSetInstance_ID == 0)
                //{
                //    MWarehouse wh = MWarehouse.Get(Ctx, VAM_Warehouse_ID);
                //    reservationVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                //}
                storageASI = Get(Ctx, reservationVAM_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                if (storageASI == null)	//	create if not existing - should not happen
                {
                    MVAMProduct product = MVAMProduct.Get(Ctx, VAM_Product_ID);
                    int xVAM_Locator_ID = MVAMProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
                    if (xVAM_Locator_ID == 0)
                    {
                        MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, VAM_Warehouse_ID);
                        xVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                    }
                    storageASI = GetCreate(Ctx, xVAM_Locator_ID,
                        VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                }
            }
            bool changed = false;
            bool qtyOnHandChanged = false;
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand.Value));
                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
                qtyOnHandChanged = true;
            }
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (storageASI == null)
                {
                    storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                else
                {
                    storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (storageASI == null)
                    storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                else
                    storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                _log.Fine(diffText.ToString());
                if (storageASI != null)
                    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
                if (Env.HasModulePrefix("VAMFG_", out mInfo1))
                {
                    MVAMWarehouse wh = new MVAMWarehouse(Env.GetCtx(), VAM_Warehouse_ID, trxName);
                    if (wh.IsDisallowNegativeInv())
                    {
                        if (qtyOnHandChanged)
                        {
                            String sql = "SELECT SUM(QtyOnHand) "
                                      + "FROM VAM_Storage s"
                                      + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                                      + "WHERE s.VAM_Product_ID=" + VAM_Product_ID  // #1
                                       + " AND l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                                       + " AND l.VAM_Locator_ID=" + VAM_Locator_ID
                                       + " AND VAM_PFeature_SetInstance_ID<>" + VAM_PFeature_SetInstance_ID;

                            Decimal qtyOnHand = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
                            qtyOnHand = Decimal.Add(qtyOnHand, storage.GetQtyOnHand());

                            string sql1 = "select VAM_PFeature_Set_ID from VAM_Product where VAM_Product_id=" + VAM_Product_ID;
                            Int32 attributeId = Util.GetValueOfInt(DB.ExecuteScalar(sql1, null, trxName));
                            if (attributeId != 0)
                            {
                                if (storage.GetQtyOnHand().CompareTo(Decimal.Zero) < 0)
                                {
                                    return false;
                                }
                                else if (qtyOnHand.CompareTo(Env.ZERO) < 0)
                                {
                                    return false;
                                }
                            }
                            return storage.Save(trxName);
                        }
                        return storage.Save(trxName);
                    }
                    else
                    {
                        return storage.Save(trxName);
                    }
                }
                else
                {
                    return storage.Save(trxName);
                }
            }

            return true;
        }
        // Change for Day to Day (DTD001) add paramete req reserved qty

        public static bool Add(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int reservationAttributeSetInstance_ID,
            Decimal? diffQtyOnHand,
            Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Decimal? ReqReservedQty, Trx trxName)
        {
            MVAMStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
                && storage.GetVAM_Product_ID() != VAM_Product_ID
                && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
            {
                s_log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
                + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI=" + VAM_PFeature_SetInstance_ID);
                return false;
            }
            MVAMStorage storageASI = null;
            if (VAM_PFeature_SetInstance_ID != reservationAttributeSetInstance_ID)
            {
                int reservationVAM_Locator_ID = VAM_Locator_ID;
                if (reservationAttributeSetInstance_ID == 0)
                {
                    MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, VAM_Warehouse_ID);
                    reservationVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                }
                storageASI = Get(Ctx, reservationVAM_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                if (storageASI == null)	//	create if not existing - should not happen
                {
                    MVAMProduct product = MVAMProduct.Get(Ctx, VAM_Product_ID);
                    int xVAM_Locator_ID = MVAMProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
                    if (xVAM_Locator_ID == 0)
                    {
                        MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, VAM_Warehouse_ID);
                        xVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                    }
                    storageASI = GetCreate(Ctx, xVAM_Locator_ID,
                        VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                }
            }
            bool changed = false;
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand.Value));
                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
            }
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (storageASI == null)
                {
                    storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                else
                {
                    storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }

            //Requisition Reserved Quantity
            if (ReqReservedQty != null && ReqReservedQty != 0)
            {
                if (storageASI == null)
                {
                    storage.SetDTD001_QtyReserved(Decimal.Add(storage.GetDTD001_QtyReserved(), (Decimal)ReqReservedQty));
                }
                else
                {
                    storageASI.SetDTD001_QtyReserved(Decimal.Add(storageASI.GetDTD001_QtyReserved(), (Decimal)ReqReservedQty));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (storageASI == null)
                    storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                else
                    storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                s_log.Fine(diffText.ToString());
                if (storageASI != null)
                    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
                return storage.Save(trxName);
            }

            return true;
        }

        //Added By Bharat
        public static bool Add(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID, int Ord_Warehouse_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int reservationAttributeSetInstance_ID,
            Decimal? diffQtyOnHand,
            Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Trx trxName)
        {
            MVAMStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
                && storage.GetVAM_Product_ID() != VAM_Product_ID
                && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
            {
                _log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
                + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI=" + VAM_PFeature_SetInstance_ID);
                return false;
            }
            //MVAMStorage storageASI = null;
            //if (VAM_PFeature_SetInstance_ID != reservationAttributeSetInstance_ID)
            //{
            //    int reservationVAM_Locator_ID = VAM_Locator_ID;
            //    if (reservationAttributeSetInstance_ID == 0)
            //    {
            //        MWarehouse wh = MWarehouse.Get(Ctx, VAM_Warehouse_ID);
            //        reservationVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
            //    }
            //    storageASI = Get(Ctx, reservationVAM_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
            //    if (storageASI == null)	//	create if not existing - should not happen
            //    {
            //        MVAMProduct product = MVAMProduct.Get(Ctx, VAM_Product_ID);
            //        int xVAM_Locator_ID = MVAMProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
            //        if (xVAM_Locator_ID == 0)
            //        {
            //            MWarehouse wh = MWarehouse.Get(Ctx, VAM_Warehouse_ID);
            //            xVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
            //        }
            //        storageASI = GetCreate(Ctx, xVAM_Locator_ID,
            //            VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
            //    }
            //}
            bool changed = false;
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand.Value));
                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
            }

            MVAMProduct product = new MVAMProduct(Ctx, VAM_Product_ID, trxName);
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (Ord_Warehouse_ID != 0)
                {
                    MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, Ord_Warehouse_ID);
                    int Ord_Locator_ID = GetResLocator_ID(Ord_Warehouse_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, diffQtyReserved.Value, trxName);
                    // JID_0982: On shipment completion if system system does not find enough reserved qty to decraese it will give message "Not enough reserved quanity in warehouse for product"
                    if (Ord_Locator_ID == 0)
                    {
                        _log.SaveError("", Msg.GetMsg(Ctx, "NoReserveQty") + ": " + product.GetName());
                        return false;
                    }
                    MVAMStorage ordStorage = GetCreate(Ctx, Ord_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                    ordStorage.SetQtyReserved(Decimal.Add(ordStorage.GetQtyReserved(), (Decimal)diffQtyReserved));
                    if (ordStorage.GetQtyReserved() < 0)
                    {
                        // due to management of stock datewise (Material Policy) - we can't set default as ZERO
                        //  ordStorage.SetQtyReserved(0);
                    }
                    ordStorage.Save(trxName);
                }
                //else
                //{
                //    if (storageASI == null)
                //    {
                //        storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                //    }
                //    else
                //    {
                //        storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                //    }
                //    diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                //}
                changed = true;
            }

            // Ordered Qty
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (Ord_Warehouse_ID != 0)
                {
                    MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, Ord_Warehouse_ID);
                    int Ord_Locator_ID = GetLocator_ID(Ord_Warehouse_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, diffQtyOrdered.Value, trxName);
                    // JID_0982: On Receipt completion if system system does not find enough ordered qty to decraese it will give message "Not enough ordered quanity in warehouse for product"
                    if (Ord_Locator_ID == 0)
                    {
                        _log.SaveError("", Msg.GetMsg(Ctx, "NoOrderedQty") + ": " + product.GetName());
                        return false;
                    }
                    MVAMStorage ordStorage = GetCreate(Ctx, Ord_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                    ordStorage.SetQtyOrdered(Decimal.Add(ordStorage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                    if (ordStorage.GetQtyOrdered() < 0)
                    {
                        ordStorage.SetQtyOrdered(0);
                    }
                    ordStorage.Save(trxName);
                }
                //else
                //{
                //    if (storageASI == null)
                //        storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                //    else
                //        storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                //    diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                //}
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                _log.Fine(diffText.ToString());
                //if (storageASI != null)
                //    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
                return storage.Save(trxName);
            }

            return true;
        }

        // Added By amit 3-8-2015 VAMRP
        public static bool Add(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
         int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int reservationAttributeSetInstance_ID,
         Decimal diffQtyOnHand,
         Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Trx trxName, int VAM_Inv_InOutLine_ID)
        {
            MVAMStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
                && storage.GetVAM_Product_ID() != VAM_Product_ID
                && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
            {
                _log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
                + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI=" + VAM_PFeature_SetInstance_ID);
                return false;
            }
            MVAMStorage storageASI = null;
            if (VAM_PFeature_SetInstance_ID != reservationAttributeSetInstance_ID)
            {
                int reservationVAM_Locator_ID = VAM_Locator_ID;
                if (reservationAttributeSetInstance_ID == 0)
                {
                    MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, VAM_Warehouse_ID);
                    reservationVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                }
                storageASI = Get(Ctx, reservationVAM_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                if (storageASI == null)	//	create if not existing - should not happen
                {
                    MVAMProduct product = MVAMProduct.Get(Ctx, VAM_Product_ID);
                    int xVAM_Locator_ID = MVAMProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
                    if (xVAM_Locator_ID == 0)
                    {
                        MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, VAM_Warehouse_ID);
                        xVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                    }
                    storageASI = GetCreate(Ctx, xVAM_Locator_ID,
                        VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                }
            }
            bool changed = false;
            bool qtyOnHandChanged = true;
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                MVAMWarehouse wh = new MVAMWarehouse(Env.GetCtx(), VAM_Warehouse_ID, trxName);
                int a = 0;
                if (wh.IsDisallowNegativeInv())
                {
                    if (qtyOnHandChanged)
                    {
                        string sqlmt = "select VAM_PFeature_SetInstance_id from VAM_Inv_InOutLineMP where VAM_Inv_InOutLine_id=" + VAM_Inv_InOutLine_ID;
                        IDataReader idrattr = null;
                        try
                        {

                            idrattr = DB.ExecuteReader(sqlmt, null, null);
                            while (idrattr.Read())
                            {
                                a++;
                                int VAM_PFeature_SetInstance_id = Util.GetValueOfInt(idrattr[0]);

                                string sqlqty = "select movementqty from VAM_Inv_InOutLineMP where VAM_PFeature_SetInstance_id=" + VAM_PFeature_SetInstance_id;
                                int attributlineqty = Util.GetValueOfInt(DB.ExecuteScalar(sqlqty.ToString(), null, null));

                                String sql1 = "SELECT QtyOnHand "
                          + "FROM VAM_Storage s"
                             + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                              + "WHERE s.VAM_Product_ID=" + storage.GetVAM_Product_ID() // #1
                              + " AND s.VAM_PFeature_SetInstance_id=" + VAM_PFeature_SetInstance_id
                              + " AND l.VAM_Locator_ID=" + storage.GetVAM_Locator_ID();

                                int storageQty = Util.GetValueOfInt(DB.ExecuteScalar(sql1.ToString(), null, null));

                                if (storageQty < attributlineqty)
                                {
                                    return false;
                                }
                                else
                                {
                                    int qtyonhand = storageQty - attributlineqty;
                                    MVAMStorage storageF = MVAMStorage.Get(Ctx, storage.GetVAM_Locator_ID(),
                        storage.GetVAM_Product_ID(), VAM_PFeature_SetInstance_id, trxName);
                                    storageF.SetQtyOnHand(qtyonhand);

                                    if (!storageF.Save())
                                    {
                                        //ShowMessage.Info(null, true, "StorageFromNotUpdated", null, null);
                                        //_processMsg = "Storage From not updated";
                                        return false;
                                    }
                                }
                            }
                            if (a == 0)
                            {
                                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand));//here no check of -ve qty
                            }

                        }
                        catch
                        {
                        }
                    }
                }


                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
                qtyOnHandChanged = true;
            }
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (storageASI == null)
                {
                    storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                else
                {
                    storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (storageASI == null)
                    storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                else
                    storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                _log.Fine(diffText.ToString());
                if (storageASI != null)
                    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
                return storage.Save(trxName);
            }

            return true;
        }

        public static bool AddQtys(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
           int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int reservationAttributeSetInstance_ID,
           Decimal? diffQtyOnHand,
           Decimal? diffQtyReserved, Decimal? diffQtyOrdered,
             Decimal? diffQtyDedicated, Decimal? diffQtyExpected,
                Decimal? diffQtyAllocated, Trx trxName)
        {
            MVAMStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
                && storage.GetVAM_Product_ID() != VAM_Product_ID
                && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
            {
                _log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
                + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI=" + VAM_PFeature_SetInstance_ID);
                return false;
            }
            MVAMStorage storageASI = null;
            if (VAM_PFeature_SetInstance_ID != reservationAttributeSetInstance_ID)
            {
                int reservationVAM_Locator_ID = VAM_Locator_ID;
                if (reservationAttributeSetInstance_ID == 0)
                {
                    MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, VAM_Warehouse_ID);
                    reservationVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                }
                storageASI = Get(Ctx, reservationVAM_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                if (storageASI == null)	//	create if not existing - should not happen
                {
                    MVAMProduct product = MVAMProduct.Get(Ctx, VAM_Product_ID);
                    int xVAM_Locator_ID = MVAMProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
                    if (xVAM_Locator_ID == 0)
                    {
                        MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, VAM_Warehouse_ID);
                        xVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                    }
                    storageASI = GetCreate(Ctx, xVAM_Locator_ID,
                        VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
                }
            }
            bool changed = false;

            //OnHand Qty
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand.Value));
                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
            }
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (storageASI == null)
                {
                    storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                else
                {
                    storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }
            //Ordered Qty
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (storageASI == null)
                    storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                else
                    storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                changed = true;
            }
            //Dedecated Qty
            if (diffQtyDedicated != null && diffQtyDedicated != 0)
            {
                if (storageASI == null)
                    storage.SetQtyDedicated(Decimal.Add(storage.GetQtyDedicated(), (Decimal)diffQtyDedicated));
                else
                    storageASI.SetQtyDedicated(Decimal.Add(storageASI.GetQtyDedicated(), (Decimal)diffQtyDedicated));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyDedicated);
                changed = true;
            }
            //Expected Qty
            if (diffQtyExpected != null && diffQtyExpected != 0)
            {
                if (storageASI == null)
                {
                    //lone is commented for warehouse task list from putway process--14 Oct,2011  
                    //storage.SetQtyExpected(Decimal.Add(storage.GetQtyExpected(), (Decimal)diffQtyExpected));
                }
                else
                {
                    storageASI.SetQtyExpected(Decimal.Add(storageASI.GetQtyExpected(), (Decimal)diffQtyExpected));
                }
                diffText.Append(" Ordered=").Append((Decimal)diffQtyExpected);
                changed = true;
            }
            //Allocated Qty
            if (diffQtyAllocated != null && diffQtyAllocated != 0)
            {
                if (storageASI == null)
                    storage.SetQtyAllocated(Decimal.Add(storage.GetQtyAllocated(), (Decimal)diffQtyAllocated));
                else
                    storageASI.SetQtyAllocated(Decimal.Add(storageASI.GetQtyAllocated(), (Decimal)diffQtyAllocated));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyAllocated);
                changed = true;
            }

            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                _log.Fine(diffText.ToString());
                if (storageASI != null)
                    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)

                String sql = "SELECT SUM(QtyOnHand) "
      + "FROM VAM_Storage s"
      + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
      + "WHERE s.VAM_Product_ID=" + VAM_Product_ID  // #1
      + " AND l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
      + " AND l.VAM_Locator_ID=" + VAM_Locator_ID
      + " AND VAM_PFeature_SetInstance_ID<>" + VAM_PFeature_SetInstance_ID;

                Decimal qtyOnHand = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
                qtyOnHand = Decimal.Add(qtyOnHand, storage.GetQtyOnHand());
                string sql1 = "select VAM_PFeature_Set_ID from VAM_Product where VAM_Product_id=" + VAM_Product_ID;
                Int32 attributeId = Util.GetValueOfInt(DB.ExecuteScalar(sql1, null, trxName));
                if (storage.GetQtyOnHand().CompareTo(Decimal.Zero) < 0 || qtyOnHand.CompareTo(Env.ZERO) < 0)
                {
                    _log.SaveError("Error", Msg.GetMsg(Env.GetCtx(), "NegativeInventoryDisallowed"));
                    // _processMsg = "Storage From not updated";
                    return false;
                }

                if (attributeId != 0)
                {
                    if (storage.GetQtyOnHand().CompareTo(Decimal.Zero) < 0 || qtyOnHand.CompareTo(Env.ZERO) < 0)
                    {
                        _log.SaveError("Error", Msg.GetMsg(Env.GetCtx(), "NegativeInventoryDisallowed"));
                        // _processMsg = "Storage From not updated";
                        return false;
                    }


                }
                else if (qtyOnHand.CompareTo(Env.ZERO) < 0)
                {
                    _log.SaveError("Error", Msg.GetMsg(Env.GetCtx(), "NegativeInventoryDisallowed"));
                    // _processMsg = "Storage From not updated";
                    return false;
                }


                return storage.Save(trxName);
            }

            return true;
        }
        //End

        // Added by Vivek on 13/11/2017
        /* 	Update Storage Info add.
       * 	Called from Closing of order
       *	@param Ctx context
       *	@param VAM_Warehouse_ID warehouse
       *	@param VAM_Locator_ID locator
       *	@param VAM_Product_ID product
       *	@param VAM_PFeature_SetInstance_ID AS Instance
       *	@param diffQtyReserved add reserved
       *	@param diffQtyOrdered add order
       *	@param trxName transaction
       *	@return true if updated
       */
        public static bool Add(Ctx Ctx, int Ord_Warehouse_ID, int VAM_Locator_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
            Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Trx trxName)
        {
            MVAMStorage storage = null;

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
                && storage.GetVAM_Product_ID() != VAM_Product_ID
                && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
            {
                _log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
                + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI=" + VAM_PFeature_SetInstance_ID);
                return false;
            }

            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                storage.SetQtyReserved(Decimal.Subtract(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                storage.Save(trxName);
            }

            // Ordered Qty
            MVAMProduct product = new MVAMProduct(Ctx, VAM_Product_ID, trxName);
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (Ord_Warehouse_ID != 0)
                {
                    MVAMWarehouse wh = MVAMWarehouse.Get(Ctx, Ord_Warehouse_ID);
                    // JID_0982: On Receipt completion if system system does not find enough ordered qty to decraese it will give message "Not enough ordered quanity in warehouse for product"
                    int Ord_Locator_ID = GetLocator_ID(Ord_Warehouse_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, diffQtyOrdered.Value, trxName);
                    if (Ord_Locator_ID == 0)
                    {
                        _log.SaveError("", Msg.GetMsg(Ctx, "NoOrderedQty") + ": " + product.GetName());
                        return false;
                    }
                    MVAMStorage ordStorage = GetCreate(Ctx, Ord_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
                    ordStorage.SetQtyOrdered(Decimal.Subtract(ordStorage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                    if (ordStorage.GetQtyOrdered() < 0)
                    {
                        ordStorage.SetQtyOrdered(0);
                    }
                    ordStorage.Save(trxName);
                }
            }
            return true;
        }

        // Added By Mohit 20-8-2015 VAWMS
        public Decimal GetAvailableQty()
        {
            SetQtyDedicated(Decimal.Zero);
            return Decimal.Subtract(Decimal.Subtract(GetQtyOnHand(), GetQtyAllocated()), GetQtyDedicated());
        }
        //END

        /// <summary>
        /// Get Location with highest Locator Priority and a sufficient OnHand Qty
        /// </summary>
        /// <param name="VAM_Warehouse_ID">warehouse</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">VAM_PFeature_SetInstance_ID</param>
        /// <param name="Qty">qty</param>
        /// <param name="trxName">transaction</param>
        /// <returns>locator id</returns>
        public static int GetVAM_Locator_ID(int VAM_Warehouse_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, Decimal Qty, Trx trxName)
        {
            int VAM_Locator_ID = 0;
            int firstVAM_Locator_ID = 0;
            String sql = "SELECT s.VAM_Locator_ID, s.QtyOnHand "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID)"
                + " INNER JOIN VAM_Product p ON (s.VAM_Product_ID=p.VAM_Product_ID)"
                + " LEFT OUTER JOIN VAM_PFeature_Set mas ON (p.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID) "
                + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + " AND s.VAM_Product_ID=" + VAM_Product_ID
                /* Set IsInstanceAttribute as True, when it is false then control not working
                   system was picking those locator, where record created with ZERO ASI, while system should pick high Priority Locator*/
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='Y' OR s.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + ")"
                + " AND l.IsActive='Y' "
                + "ORDER BY l.PriorityNo DESC, s.QtyOnHand DESC";
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Decimal QtyOnHand = Convert.ToDecimal(dr[1]);
                    if (QtyOnHand != null && Qty.CompareTo(QtyOnHand) <= 0)
                    {
                        VAM_Locator_ID = Convert.ToInt32(dr[0]);
                        break;
                    }
                    if (firstVAM_Locator_ID == 0)
                        firstVAM_Locator_ID = Convert.ToInt32(dr[0]); 
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            if (VAM_Locator_ID != 0)
                return VAM_Locator_ID;
            return firstVAM_Locator_ID;
        }


        public static int GetLocator_ID(int VAM_Warehouse_ID, int VAM_Product_ID,
           int VAM_PFeature_SetInstance_ID, Decimal Qty, Trx trxName)
        {
            if (Qty < 0)
            {
                Qty = Decimal.Negate(Qty);
            }
            else
            {
                Qty = 0;
            }
            int VAM_Locator_ID = 0;
            IDataReader idr = null;
            DataTable dt = null;
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param", Qty);

            String sql = "SELECT s.VAM_Locator_ID "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID)"
                + " INNER JOIN VAM_Product p ON (s.VAM_Product_ID=p.VAM_Product_ID)"
                + " LEFT OUTER JOIN VAM_PFeature_Set mas ON (p.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID) "
                + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + " AND s.VAM_Product_ID=" + VAM_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='Y' AND s.QtyOrdered >= @param ";
            VAM_Locator_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, param, trxName));
            if (VAM_Locator_ID == 0)
            {
                string qry = "SELECT s.VAM_Locator_ID "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID)"
                + " INNER JOIN VAM_Product p ON (s.VAM_Product_ID=p.VAM_Product_ID)"
                + " LEFT OUTER JOIN VAM_PFeature_Set mas ON (p.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID) "
                + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + " AND s.VAM_Product_ID=" + VAM_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='N' AND s.QtyOrdered >= @param ";
                try
                {
                    idr = DB.ExecuteReader(qry, param, trxName);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        VAM_Locator_ID = Convert.ToInt32(dr[0]);//.getInt(1);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    _log.Log(Level.SEVERE, sql, ex);
                }
                finally
                {
                    dt = null;
                    if (idr != null)
                    {
                        idr.Close();
                    }
                }
            }
            return VAM_Locator_ID;
        }

        public static int GetResLocator_ID(int VAM_Warehouse_ID, int VAM_Product_ID,
           int VAM_PFeature_SetInstance_ID, Decimal Qty, Trx trxName)
        {
            //if (Qty < 0)
            //{
            int AbsoluteRequired = 0;
            if (Qty > 0)
                AbsoluteRequired = 1;

            Qty = Decimal.Negate(Qty);
            //}
            //else
            //{
            //    Qty = 0;
            //}
            int VAM_Locator_ID = 0;
            IDataReader idr = null;
            DataTable dt = null;
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param", Qty);

            String sql = "SELECT s.VAM_Locator_ID "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID)"
                + " INNER JOIN VAM_Product p ON (s.VAM_Product_ID=p.VAM_Product_ID)"
                + " LEFT OUTER JOIN VAM_PFeature_Set mas ON (p.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID) "
                + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + " AND s.VAM_Product_ID=" + VAM_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='Y' AND CASE WHEN " + AbsoluteRequired + " = 1 THEN ABS(s.QtyReserved) ELSE s.QtyReserved END >= @param ";
            VAM_Locator_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, param, trxName));
            if (VAM_Locator_ID == 0)
            {
                string qry = "SELECT s.VAM_Locator_ID "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID)"
                + " INNER JOIN VAM_Product p ON (s.VAM_Product_ID=p.VAM_Product_ID)"
                + " LEFT OUTER JOIN VAM_PFeature_Set mas ON (p.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID) "
                + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + " AND s.VAM_Product_ID=" + VAM_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='N' AND CASE WHEN " + AbsoluteRequired + " = 1 THEN  ABS(s.QtyReserved) ELSE s.QtyReserved END >= @param ";
                try
                {
                    idr = DB.ExecuteReader(qry, param, trxName);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        VAM_Locator_ID = Convert.ToInt32(dr[0]);//.getInt(1);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    _log.Log(Level.SEVERE, sql, ex);
                }
                finally
                {
                    dt = null;
                    if (idr != null)
                    {
                        idr.Close();
                    }
                }
            }
            return VAM_Locator_ID;
        }

        /**
	 * 	Get Available Qty.
	 * 	The call is accurate only if there is a storage record 
	 * 	and assumes that the product is stocked 
	 *	@param VAM_Warehouse_ID wh
	 *	@param VAM_Product_ID product
	 *	@param VAM_PFeature_SetInstance_ID masi
	 *	@param trxName transaction
	 *	@return qty available (QtyOnHand-QtyReserved) or null
	 */
        public static Decimal? GetQtyAvailable(int VAM_Warehouse_ID, int VAM_Product_ID,
             int VAM_PFeature_SetInstance_ID, Trx trxName)
        {
            Decimal? retValue = null;
            DataSet ds = null;
            String sql = "SELECT SUM(QtyOnHand-QtyReserved) "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                + "WHERE s.VAM_Product_ID=" + VAM_Product_ID		//	#1
                + " AND l.VAM_Warehouse_ID=" + VAM_Warehouse_ID;
            if (VAM_PFeature_SetInstance_ID != 0)
            {
                sql += " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            }
            try
            {
                ds = DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    if (dr[0] == DBNull.Value)
                    {
                        //retValue = DBNull.Value;
                        retValue = null;
                    }
                    else
                    {
                        retValue = Util.GetValueOfDecimal(dr[0]);
                    }
                    if (dr[i] == null)
                    {
                        retValue = null;
                    }
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            _log.Fine("VAM_Warehouse_ID=" + VAM_Warehouse_ID
               + ",VAM_Product_ID=" + VAM_Product_ID + " = " + retValue);
            return retValue;
        }


        /**
    * 	Get Available Qty.
    * 	The call is accurate only if there is a storage record 
    * 	and assumes that the product is stocked 
    *	@param VAM_Warehouse_ID wh
    *	@param VAM_Product_ID product
    *	@param VAM_PFeature_SetInstance_ID masi
    *	@param trxName transaction
    *	@return qty available (QtyOnHand) or null 
         */
        public static Decimal? GetQtyAvailableWithoutReserved(int VAM_Warehouse_ID, int VAM_Product_ID,
         int VAM_PFeature_SetInstance_ID, Trx trxName)
        {
            Decimal? retValue = null;
            String sql = "SELECT SUM(QtyOnHand) "
                + "FROM VAM_Storage s"
                + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                + "WHERE s.VAM_Product_ID=" + VAM_Product_ID		//	#1
                + " AND l.VAM_Warehouse_ID=" + VAM_Warehouse_ID;
            if (VAM_PFeature_SetInstance_ID != 0)
            {
                sql += " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            }
            try
            {
                retValue = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            _log.Fine("VAM_Warehouse_ID=" + VAM_Warehouse_ID
               + ",VAM_Product_ID=" + VAM_Product_ID + " = " + retValue);
            return retValue;
        }

        /*	Persistency Constructor
        *	@param ctx context
        *	@param ignored ignored
        *	@param trxName transaction
        */
        public MVAMStorage(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            //
            SetQtyOnHand(Env.ZERO);
            SetQtyOrdered(Env.ZERO);
            SetQtyReserved(Env.ZERO);
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVAMStorage(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
         * 	Full NEW Constructor
         *	@param locator (parent) locator
         *	@param VAM_Product_ID product
         *	@param VAM_PFeature_SetInstance_ID attribute
         */
        private MVAMStorage(MVAMLocator locator, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID)
            : this(locator.GetCtx(), 0, locator.Get_TrxName())
        {

            SetClientOrg(locator);
            SetVAM_Locator_ID(locator.GetVAM_Locator_ID());
            SetVAM_Product_ID(VAM_Product_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
        }

        /**
         * 	Change Qty OnHand
         *	@param qty quantity
         *	@param add add if true 
         */
        public void ChangeQtyOnHand(Decimal qty, bool add)
        {
            if (qty == null || Env.Signum(qty) == 0)
                return;
            if (add)
                SetQtyOnHand(Decimal.Add(GetQtyOnHand(), qty));
            else
                SetQtyOnHand(Decimal.Subtract(GetQtyOnHand(), qty));
        }

        /**
         * 	Get VAM_Warehouse_ID of Locator
         *	@return warehouse
         */
        public int GetVAM_Warehouse_ID()
        {
            if (_VAM_Warehouse_ID == 0)
            {
                MVAMLocator loc = MVAMLocator.Get(GetCtx(), GetVAM_Locator_ID());
                _VAM_Warehouse_ID = loc.GetVAM_Warehouse_ID();
            }
            return _VAM_Warehouse_ID;
        }

        /**
         *	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMStorage[")
                .Append("VAM_Locator_ID=").Append(GetVAM_Locator_ID())
                    .Append(",VAM_Product_ID=").Append(GetVAM_Product_ID())
                    .Append(",VAM_PFeature_SetInstance_ID=").Append(GetVAM_PFeature_SetInstance_ID())
                .Append(": OnHand=").Append(GetQtyOnHand())
                .Append(",Reserved=").Append(GetQtyReserved())
                .Append(",Ordered=").Append(GetQtyOrdered())
                .Append("]");
            return sb.ToString();
        }

        //Get Product Qty From Storage Where Attribute Is Not Selected.
        public static MVAMStorage Get(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID, bool isfifo, Trx trxName)
        {
            MVAMStorage retValue = null;
            String sql = "SELECT * FROM VAM_Storage "
                + "WHERE VAM_Product_ID=" + VAM_Product_ID + " AND VAM_Locator_ID=" + VAM_Locator_ID
                + " AND VAM_PFeature_SetInstance_ID > 0"
                + " AND QtyOnHand >= 0 "
                + "ORDER BY VAM_PFeature_SetInstance_ID";
            if (!isfifo)
                sql += " DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVAMStorage(ctx, dr, trxName);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            if (retValue == null)
            {
                s_log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                    + ", VAM_Product_ID=" + VAM_Product_ID);
            }
            else
            {
                s_log.Fine("VAM_Locator_ID=" + VAM_Locator_ID
                    + ", VAM_Product_ID=" + VAM_Product_ID);
            }
            return retValue;
        }

        protected override bool BeforeSave(bool newRecord)
        {
            //	Negative Inventory check
            if (newRecord || (Is_ValueChanged("QtyOnHand") ||
                        Is_ValueChanged("QtyDedicated")
                         || Is_ValueChanged("QtyAllocated")
                      ))
            {
                MVAMWarehouse wh = MVAMWarehouse.Get(GetCtx(), GetVAM_Warehouse_ID());

                if (wh.IsDisallowNegativeInv())
                {
                    MVAMProduct product = MVAMProduct.Get(GetCtx(), GetVAM_Product_ID());

                    if (Math.Sign(GetQty()) < 0)
                    {
                        s_log.SaveError("Error", Msg.GetMsg(GetCtx(), "NegativeInventoryDisallowed"));
                        return false;
                    }
                    Decimal qtyOnHand = Env.ZERO;
                    Decimal qtyDedicated = Env.ZERO;
                    Decimal qtyAllocated = Env.ZERO;

                    String sql = "SELECT SUM(QtyOnHand),SUM(QtyDedicated),SUM(QtyAllocated) "
                        + "FROM VAM_Storage s"
                        + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                        + "WHERE s.VAM_Product_ID=@p1"		//	#1
                        + " AND l.VAM_Warehouse_ID=@w1"
                        + " AND l.VAM_Locator_ID=@l1 "
                        + " AND NVL(s.VAM_PFeature_SetInstance_ID, 0) = @a1";
                    IDataReader dr = null;
                    try
                    {
                        SqlParameter[] param = new SqlParameter[4];
                        param[0] = new SqlParameter("@p1", GetVAM_Product_ID());
                        param[1] = new SqlParameter("@w1", GetVAM_Warehouse_ID());
                        param[2] = new SqlParameter("@l1", GetVAM_Locator_ID());
                        param[3] = new SqlParameter("@a1", GetVAM_PFeature_SetInstance_ID());

                        dr = DB.ExecuteReader(sql, param, Get_Trx());
                        if (dr.Read())
                        {
                            qtyOnHand = Util.GetValueOfDecimal(dr[0]);
                            qtyDedicated = Util.GetValueOfDecimal(dr[1]);
                            qtyAllocated = Util.GetValueOfDecimal(dr[2]);
                        }
                    }
                    catch (Exception e)
                    {
                        s_log.Log(Level.SEVERE, sql, e);
                    }
                    finally
                    {
                        if (dr != null)
                            dr.Close();
                    }

                    Decimal? asiQtyOnHand = null;
                    Decimal? asiQtyDedicated = null;
                    Decimal? asiQtyAllocated = null;

                    if (newRecord)
                    {
                        if (Is_ValueChanged("QtyOnHand"))
                        {
                            qtyOnHand = qtyOnHand + GetQtyOnHand();
                        }
                        if (Is_ValueChanged("QtyDedicated"))
                        {
                            qtyDedicated = qtyDedicated + GetQtyDedicated();
                        }
                        if (Is_ValueChanged("QtyAllocated"))
                        {
                            qtyAllocated = qtyAllocated + GetQtyAllocated();
                        }
                    }
                    else
                    {
                        if (Is_ValueChanged("QtyOnHand"))
                        {
                            qtyOnHand = (qtyOnHand + GetQtyOnHand()) - Util.GetValueOfDecimal(Get_ValueOld("QtyOnHand"));

                            asiQtyOnHand = GetQtyOnHand();
                        }
                        if (Is_ValueChanged("QtyDedicated"))
                        {
                            qtyDedicated = (qtyDedicated + GetQtyDedicated()) - Util.GetValueOfDecimal(Get_ValueOld("QtyDedicated"));
                        }
                        if (Is_ValueChanged("QtyAllocated"))
                        {
                            qtyAllocated = (qtyAllocated + GetQtyAllocated()) - Util.GetValueOfDecimal(Get_ValueOld("QtyAllocated"));
                        }
                    }

                    if (qtyOnHand < 0)
                    {
                        s_log.SaveError("Error", product.GetName() + " , " + Msg.GetMsg(GetCtx(), "NegativeInventoryDisallowed") + " : " + qtyOnHand + " , "
                                                + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + Util.GetValueOfDecimal(Get_ValueOld("QtyOnHand")));
                        return false;
                    }

                    //TODO: Check Negative qunatity
                    //if (qtyOnHand.CompareTo(qtyDedicated + qtyAllocated) < 0 ||
                    //        asiQtyOnHand.Value.CompareTo(asiQtyDedicated + asiQtyAllocated) < 0)
                    //{
                    //    s_log.SaveError("Error", Msg.GetMsg(GetCtx(), "NegativeInventoryDisallowed"));
                    //     return false;
                    //}
                }
            }

            return base.BeforeSave(newRecord);
        }


        /* Not Used */

        public static List<MVAMStorage> GetMultipleForUpdate(Ctx ctx, int VAM_Locator_ID,
               int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
               List<String> types, Trx trx)
        {
            return GetMultiple(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, types, true, trx);
        }

        private static List<MVAMStorage> GetMultiple(Ctx ctx, int VAM_Locator_ID,
              int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
              List<String> types, Boolean forUpdate, Trx trx)
        {
            List<MVAMStorage> retValue = new List<MVAMStorage>();
            StringBuilder qtyTypesSql = new StringBuilder(" ");
            //if (!types.isEmpty())
            if (types.Count > 0)
            {
                qtyTypesSql.Append("AND QtyType in (");
                foreach (var type in types)
                {
                    qtyTypesSql.Append("'");
                    qtyTypesSql.Append(type);//type.GetValue()
                    qtyTypesSql.Append("',");
                }
                //qtyTypesSql.deleteCharAt(qtyTypesSql.Length - 1);
                qtyTypesSql.Remove(qtyTypesSql.Length - 1, qtyTypesSql.Length);
                qtyTypesSql.Append("') ");
            }

            String sql = "SELECT * FROM VAM_Storage "
                    + "WHERE VAM_Locator_ID=" + VAM_Locator_ID + " AND VAM_Product_ID=" + VAM_Product_ID + " AND ";
            if (VAM_PFeature_SetInstance_ID == 0)
            {
                sql += "(VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR VAM_PFeature_SetInstance_ID IS NULL)";
            }
            else
            {
                sql += "VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            }
            if (qtyTypesSql.Length > 1)
            {
                sql += qtyTypesSql.ToString();
            }
            if (forUpdate)
            {
                sql += " FOR UPDATE ";
            }
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue.Add(new MVAMStorage(ctx, dr, trx));
                }
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            if (retValue.Count == 0)//   isEmpty())
            {
                _log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                        + ", VAM_Product_ID=" + VAM_Product_ID
                        + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            }
            else
            {
                _log.Fine("VAM_Locator_ID=" + VAM_Locator_ID + ", VAM_Product_ID="
                        + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            }
            return retValue;
        }

        /// <summary>
        /// Static Get/update
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Locator_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="type"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static MVAMStorage GetForRead(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                String type, Trx trx)
        {
            return Get(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    type, false, trx);
        }

        private static MVAMStorage Get(Ctx ctx, int VAM_Locator_ID,
               int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
               String type, Boolean forUpdate, Trx trx)
        {
            MVAMStorage retValue = null;
            String sql = "SELECT * FROM VAM_Storage "
                    + "WHERE VAM_Locator_ID=" + VAM_Locator_ID + " AND VAM_Product_ID=" + VAM_Product_ID + " AND QtyType='" + type + "' AND ";//type.GetValue()
            if (VAM_PFeature_SetInstance_ID == 0)
            {
                sql += "(VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR VAM_PFeature_SetInstance_ID IS NULL)";
            }
            else
            {
                sql += "VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            }
            if (forUpdate)
            {
                sql += " FOR UPDATE ";
            }
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    retValue = new MVAMStorage(ctx, dt.Rows[0], trx);
                }
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            if (retValue == null)
                _log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                        + ", VAM_Product_ID=" + VAM_Product_ID
                        + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            else
            {
                _log.Fine("VAM_Locator_ID=" + VAM_Locator_ID + ", VAM_Product_ID="
                        + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            }
            return retValue;
        }

        public static Decimal GetForUpdate(Ctx ctx, int VAM_Warehouse_ID,
               int VAM_Locator_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
               int reservationAttributeSetInstance_ID, X_Ref_Quantity_Type type,
               Trx trx)
        {
            return Decimal.Zero;
        }
        public static MVAMStorage GetForUpdate(Ctx ctx, int VAM_Locator_ID,
               int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
               String type, Trx trx)
        {
            return Get(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    type, true, trx);
        }

    }


    //public class MVAMStorage : X_VAM_Storage
    //{ 
    //    #region Private variables
    //    // Log		
    //    private static VLogger _log = VLogger.GetVLogger(typeof(MVAMStorage).FullName);		
    //    //private static CLogger		s_log = CLogger.getCLogger (MVAMStorage.class);
    //    // Warehouse					
    //    private int _VAM_Warehouse_ID = 0;
    //    #endregion

    //    /*	Get Storage Info
    //    *	@param ctx context
    //    *	@param VAM_Locator_ID locator
    //    *	@param VAM_Product_ID product
    //    *	@param VAM_PFeature_SetInstance_ID instance
    //    *	@param trxName transaction
    //    *	@return existing or null
    //    */
    //    public static MVAMStorage Get(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
    //         int VAM_PFeature_SetInstance_ID, String trxName)
    //    {
    //        MVAMStorage retValue = null;
    //        String sql = "SELECT * FROM VAM_Storage "
    //            + "WHERE VAM_Locator_ID=" + VAM_Locator_ID + " AND VAM_Product_ID=" + VAM_Product_ID + " AND ";
    //        if (VAM_PFeature_SetInstance_ID == 0)
    //            sql += "(VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR VAM_PFeature_SetInstance_ID IS NULL)";
    //        else
    //            sql += "VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //            idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                retValue = new MVAMStorage(ctx, dr, trxName);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }

    //        if (retValue == null)
    //        {
    //            _log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
    //                + ", VAM_Product_ID=" + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID);
    //        }
    //        else
    //        {
    //            _log.Fine("VAM_Locator_ID=" + VAM_Locator_ID
    //                + ", VAM_Product_ID=" + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID);
    //        }
    //        return retValue;
    //    }

    //    /**
    //   * 	Get all Storages for Product with ASI
    //   *	@param ctx context
    //   *	@param VAM_Product_ID product
    //   *	@param VAM_Locator_ID locator
    //   *	@param FiFo first in-first-out
    //   *	@param trxName transaction
    //   *	@return existing or null
    //   */
    //    public static MVAMStorage[] GetAllWithASI(Ctx ctx, int VAM_Product_ID, int VAM_Locator_ID,
    //        bool FiFo, String trxName)
    //    {
    //        List<MVAMStorage> list = new List<MVAMStorage>();
    //        String sql = "SELECT * FROM VAM_Storage "
    //            + "WHERE VAM_Product_ID=" + VAM_Product_ID + " AND VAM_Locator_ID=" + VAM_Locator_ID
    //            + " AND VAM_PFeature_SetInstance_ID > 0"
    //            + " AND QtyOnHand > 0 "
    //            + "ORDER BY VAM_PFeature_SetInstance_ID";
    //        if (!FiFo)
    //            sql += " DESC";
    //        DataSet ds = new DataSet();
    //        try
    //        {
    //            ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
    //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
    //            {
    //                DataRow dr = ds.Tables[0].Rows[i];
    //                list.Add(new MVAMStorage(ctx, dr, trxName));
    //            }
    //            ds = null;
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        ds = null;
    //        MVAMStorage[] retValue = new MVAMStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /*	Get all Storages for Product
    //    *	@param ctx context
    //    *	@param VAM_Product_ID product
    //    *	@param VAM_Locator_ID locator
    //    *	@param trxName transaction
    //    *	@return existing or null
    //    */
    //    public static MVAMStorage[] GetAll(Ctx ctx, int VAM_Product_ID, int VAM_Locator_ID, String trxName)
    //    {
    //        List<MVAMStorage> list = new List<MVAMStorage>();
    //        String sql = "SELECT * FROM VAM_Storage "
    //            + "WHERE VAM_Product_ID=" + VAM_Product_ID + " AND VAM_Locator_ID=" + VAM_Locator_ID
    //            + " AND QtyOnHand <> 0 "
    //            + "ORDER BY VAM_PFeature_SetInstance_ID";
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //             idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                list.Add(new MVAMStorage(ctx, dr, trxName));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }
    //        MVAMStorage[] retValue = new MVAMStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /*	Get Storage Info for Product across warehouses
    //    *	@param ctx context
    //    *	@param VAM_Product_ID product
    //    *	@param trxName transaction
    //    *	@return existing or null
    //    */
    //    public static MVAMStorage[] GetOfProduct(Ctx ctx, int VAM_Product_ID, String trxName)
    //    {
    //        List<MVAMStorage> list = new List<MVAMStorage>();
    //        String sql = "SELECT * FROM VAM_Storage "
    //            + "WHERE VAM_Product_ID=" + VAM_Product_ID;
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //             idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                list.Add(new MVAMStorage(ctx, dr, trxName));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }

    //        MVAMStorage[] retValue = new MVAMStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /* 	Get Storage Info for Warehouse
    //   *	@param Ctx context
    //   *	@param VAM_Warehouse_ID 
    //   *	@param VAM_Product_ID product
    //   *	@param VAM_PFeature_SetInstance_ID instance
    //   *	@param VAM_PFeature_Set_ID attribute set
    //   *	@param allAttributeInstances if true, all attribute set instances
    //   *	@param minGuaranteeDate optional minimum guarantee date if all attribute instances
    //   *	@param FiFo first in-first-out
    //   *	@param trxName transaction
    //   *	@return existing - ordered by location priority (desc) and/or guarantee date
    //   */
    //    public static MVAMStorage[] GetWarehouse(Ctx Ctx, int VAM_Warehouse_ID,
    //        int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
    //        bool allAttributeInstances, DateTime? minGuaranteeDate,
    //        bool FiFo, String trxName)
    //    {
    //        if (VAM_Warehouse_ID == 0 || VAM_Product_ID == 0)
    //            return new MVAMStorage[0];

    //        if (VAM_PFeature_Set_ID == 0)
    //            allAttributeInstances = true;
    //        else
    //        {
    //            MVAMPFeatureSet mas = MVAMPFeatureSet.Get(Ctx, VAM_PFeature_Set_ID);
    //            if (!mas.IsInstanceAttribute())
    //                allAttributeInstances = true;
    //        }

    //        List<MVAMStorage> list = new List<MVAMStorage>();
    //        //	Specific Attribute Set Instance
    //        String sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
    //            + "s.VAF_Client_ID,s.VAF_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
    //            + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
    //            + "FROM VAM_Storage s"
    //            + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID) "
    //            + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
    //            + " AND s.VAM_Product_ID=" + VAM_Product_ID
    //            + " AND COALESCE(s.VAM_PFeature_SetInstance_ID,0)= " + VAM_PFeature_SetInstance_ID
    //            + "ORDER BY l.PriorityNo DESC, VAM_PFeature_SetInstance_ID";
    //        if (!FiFo)
    //        {
    //            sql += " DESC";
    //        }
    //        //	All Attribute Set Instances
    //        if (allAttributeInstances)
    //        {
    //            sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
    //                + "s.VAF_Client_ID,s.VAF_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
    //                + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
    //                + "FROM VAM_Storage s"
    //                + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID)"
    //                + " LEFT OUTER JOIN VAM_PFeature_SetInstance asi ON (s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) "
    //                + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
    //                + " AND s.VAM_Product_ID=" + VAM_Product_ID;
    //            if (minGuaranteeDate != null)
    //            {
    //                sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>'" + String.Format("{0:dd-MMM-yy}", minGuaranteeDate) + "') "
    //                    + "ORDER BY asi.GuaranteeDate, VAM_PFeature_SetInstance_ID";	//	Has Prio over Locator
    //                if (!FiFo)
    //                    sql += " DESC";
    //                sql += ", l.PriorityNo DESC, s.QtyOnHand DESC";
    //            }
    //            else
    //            {
    //                sql += "ORDER BY l.PriorityNo DESC, l.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID";
    //                if (!FiFo)
    //                    sql += " DESC";
    //                sql += ", s.QtyOnHand DESC";
    //            }
    //        }
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //            idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                list.Add(new MVAMStorage(Ctx, dr, trxName));
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, e);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }
    //        MVAMStorage[] retValue = new MVAMStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /*	Create or Get Storage Info
    //    *	@param ctx context
    //    *	@param VAM_Locator_ID locator
    //    *	@param VAM_Product_ID product
    //    *	@param VAM_PFeature_SetInstance_ID instance
    //    *	@param trxName transaction
    //    *	@return existing/new or null
    //    */
    //    public static MVAMStorage GetCreate(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
    //         int VAM_PFeature_SetInstance_ID, String trxName)
    //    {
    //        if (VAM_Locator_ID == 0)
    //        {
    //            throw new ArgumentException("VAM_Locator_ID=0");
    //        }
    //        if (VAM_Product_ID == 0)
    //        {
    //            throw new ArgumentException("VAM_Product_ID=0");
    //        }
    //        MVAMStorage retValue = Get(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
    //        if (retValue != null)
    //            return retValue;

    //        //	Insert row based on locator
    //        MVAMLocator locator = new MVAMLocator(ctx, VAM_Locator_ID, trxName);
    //        if (locator.Get_ID() != VAM_Locator_ID)
    //        {
    //            throw new ArgumentException("Not found VAM_Locator_ID=" + VAM_Locator_ID);
    //        }
    //        retValue = new MVAMStorage(locator, VAM_Product_ID, VAM_PFeature_SetInstance_ID);
    //        retValue.Save(trxName);
    //        _log.Fine("New " + retValue);
    //        return retValue;
    //    }

    //    /* 	Update Storage Info add.
    //   * 	Called from MProjectIssue
    //   *	@param Ctx context
    //   *	@param VAM_Warehouse_ID warehouse
    //   *	@param VAM_Locator_ID locator
    //   *	@param VAM_Product_ID product
    //   *	@param VAM_PFeature_SetInstance_ID AS Instance
    //   *	@param reservationAttributeSetInstance_ID reservation AS Instance
    //   *	@param diffQtyOnHand add on hand
    //   *	@param diffQtyReserved add reserved
    //   *	@param diffQtyOrdered add order
    //   *	@param trxName transaction
    //   *	@return true if updated
    //   */
    //    public static bool Add(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
    //        int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int reservationAttributeSetInstance_ID,
    //        Decimal diffQtyOnHand,
    //        Decimal? diffQtyReserved, Decimal? diffQtyOrdered, String trxName)
    //    {
    //        MVAMStorage storage = null;
    //        StringBuilder diffText = new StringBuilder("(");

    //        //	Get Storage
    //        if (storage == null)
    //        {
    //            storage = GetCreate(Ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, trxName);
    //        }
    //        //	Verify
    //        if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
    //            && storage.GetVAM_Product_ID() != VAM_Product_ID
    //            && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
    //        {
    //            _log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
    //            + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI=" + VAM_PFeature_SetInstance_ID);
    //            return false;
    //        }
    //        MVAMStorage storageASI = null;
    //        if (VAM_PFeature_SetInstance_ID != reservationAttributeSetInstance_ID)
    //        {
    //            int reservationVAM_Locator_ID = VAM_Locator_ID;
    //            if (reservationAttributeSetInstance_ID == 0)
    //            {
    //                MWarehouse wh = MWarehouse.Get(Ctx, VAM_Warehouse_ID);
    //                reservationVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
    //            }
    //            storageASI = Get(Ctx, reservationVAM_Locator_ID, VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
    //            if (storageASI == null)	//	create if not existing - should not happen
    //            {
    //                MVAMProduct product = MVAMProduct.Get(Ctx, VAM_Product_ID);
    //                int xVAM_Locator_ID = MVAMProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
    //                if (xVAM_Locator_ID == 0)
    //                {
    //                    MWarehouse wh = MWarehouse.Get(Ctx, VAM_Warehouse_ID);
    //                    xVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
    //                }
    //                storageASI = GetCreate(Ctx, xVAM_Locator_ID,
    //                    VAM_Product_ID, reservationAttributeSetInstance_ID, trxName);
    //            }
    //        }
    //        bool changed = false;
    //        if (diffQtyOnHand != null && diffQtyOnHand != 0)
    //        {
    //            storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand));
    //            diffText.Append("OnHand=").Append(diffQtyOnHand);
    //            changed = true;
    //        }
    //        //	Reserved Qty
    //        if (diffQtyReserved != null && diffQtyReserved != 0)
    //        {
    //            if (storageASI == null)
    //            {
    //                storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
    //            }
    //            else
    //            {
    //                storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
    //            }
    //            diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
    //            changed = true;
    //        }
    //        if (diffQtyOrdered != null && diffQtyOrdered != 0)
    //        {
    //            if (storageASI == null)
    //                storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
    //            else
    //                storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
    //            diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
    //            changed = true;
    //        }
    //        if (changed)
    //        {
    //            diffText.Append(") -> ").Append(storage.ToString());
    //            _log.Fine(diffText.ToString());
    //            if (storageASI != null)
    //                storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
    //            return storage.Save(trxName);
    //        }

    //        return true;
    //    }

    //    /*	Get Location with highest Locator Priority and a sufficient OnHand Qty
    //    * 	@param VAM_Warehouse_ID warehouse
    //    * 	@param VAM_Product_ID product
    //    * 	@param VAM_PFeature_SetInstance_ID asi
    //    * 	@param Qty qty
    //    *	@param trxName transaction
    //    * 	@return id
    //    */
    //    public static int GetVAM_Locator_ID(int VAM_Warehouse_ID, int VAM_Product_ID, 
    //        int VAM_PFeature_SetInstance_ID, Decimal Qty, String trxName)
    //    {
    //        int VAM_Locator_ID = 0;
    //        int firstVAM_Locator_ID = 0;
    //        String sql = "SELECT s.VAM_Locator_ID, s.QtyOnHand "
    //            + "FROM VAM_Storage s"
    //            + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID)"
    //            + " INNER JOIN VAM_Product p ON (s.VAM_Product_ID=p.VAM_Product_ID)"
    //            + " LEFT OUTER JOIN VAM_PFeature_Set mas ON (p.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID) "
    //            + "WHERE l.VAM_Warehouse_ID=" + VAM_Warehouse_ID
    //            + " AND s.VAM_Product_ID=" + VAM_Product_ID
    //            + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + ")"
    //            + " AND l.IsActive='Y' "
    //            + "ORDER BY l.PriorityNo DESC, s.QtyOnHand DESC";
    //        IDataReader idr = null;
    //        DataTable dt = null;
    //        try
    //        {
    //            idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                Decimal QtyOnHand = Convert.ToDecimal(dr[1]);//.getBigDecimal(2);
    //                if (QtyOnHand != null && Qty.CompareTo(QtyOnHand) <= 0)
    //                {
    //                    VAM_Locator_ID = Convert.ToInt32(dr[0]);//.getInt(1);
    //                    break;
    //                }
    //                if (firstVAM_Locator_ID == 0)
    //                    firstVAM_Locator_ID = Convert.ToInt32(dr[0]); //dr.getInt(1);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            dt = null;
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //        }
    //        if (VAM_Locator_ID != 0)
    //            return VAM_Locator_ID;
    //        return firstVAM_Locator_ID;
    //    }

    //    /**
    // * 	Get Available Qty.
    // * 	The call is accurate only if there is a storage record 
    // * 	and assumes that the product is stocked 
    // *	@param VAM_Warehouse_ID wh
    // *	@param VAM_Product_ID product
    // *	@param VAM_PFeature_SetInstance_ID masi
    // *	@param trxName transaction
    // *	@return qty available (QtyOnHand-QtyReserved) or null
    // */
    //    public static Decimal? GetQtyAvailable(int VAM_Warehouse_ID, int VAM_Product_ID,
    //         int VAM_PFeature_SetInstance_ID, String trxName)
    //    {
    //        Decimal? retValue = null;
    //        DataSet ds = null;
    //        String sql = "SELECT SUM(QtyOnHand-QtyReserved) "
    //            + "FROM VAM_Storage s"
    //            + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
    //            + "WHERE s.VAM_Product_ID=" + VAM_Product_ID		//	#1
    //            + " AND l.VAM_Warehouse_ID=" + VAM_Warehouse_ID;
    //        if (VAM_PFeature_SetInstance_ID != 0)
    //        {
    //            sql += " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
    //        }
    //        try
    //        {
    //            ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
    //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
    //            {
    //                DataRow dr = ds.Tables[0].Rows[i];
    //                if (dr[0] == DBNull.Value)
    //                {
    //                    //retValue = DBNull.Value;
    //                    retValue = null;
    //                }
    //                else
    //                {
    //                    retValue = Utility.Util.GetValueOfDecimal(dr[0]);
    //                }
    //                if (dr[i] == null)
    //                {
    //                    retValue = null;
    //                }
    //            }
    //            ds = null;
    //        }
    //        catch (Exception e)
    //        {
    //            _log.Log(Level.SEVERE, sql, e);
    //        }
    //        _log.Fine("VAM_Warehouse_ID=" + VAM_Warehouse_ID
    //           + ",VAM_Product_ID=" + VAM_Product_ID + " = " + retValue);
    //        return retValue;
    //    }

    //    /*	Persistency Constructor
    //    *	@param ctx context
    //    *	@param ignored ignored
    //    *	@param trxName transaction
    //    */
    //    public MVAMStorage(Ctx ctx, int ignored, String trxName)
    //        : base(ctx, 0, trxName)
    //    {
    //        if (ignored != 0)
    //            throw new ArgumentException("Multi-Key");
    //        //
    //        SetQtyOnHand(Env.ZERO);
    //        SetQtyOrdered(Env.ZERO);
    //        SetQtyReserved(Env.ZERO);
    //    }

    //    /**
    //     * 	Load Constructor
    //     *	@param ctx context
    //     *	@param dr result set
    //     *	@param trxName transaction
    //     */
    //    public MVAMStorage(Ctx ctx, DataRow dr, String trxName)
    //        : base(ctx, dr, trxName)
    //    {
    //    }

    //    /**
    //     * 	Full NEW Constructor
    //     *	@param locator (parent) locator
    //     *	@param VAM_Product_ID product
    //     *	@param VAM_PFeature_SetInstance_ID attribute
    //     */
    //    private MVAMStorage(MVAMLocator locator, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID)
    //        : this(locator.GetCtx(), 0, locator.Get_TrxName())
    //    {

    //        SetClientOrg(locator);
    //        SetVAM_Locator_ID(locator.GetVAM_Locator_ID());
    //        SetVAM_Product_ID(VAM_Product_ID);
    //        SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
    //    }

    //    /**
    //     * 	Change Qty OnHand
    //     *	@param qty quantity
    //     *	@param add add if true 
    //     */
    //    public void ChangeQtyOnHand(Decimal qty, bool add)
    //    {
    //        if (qty == null || Env.Signum(qty) == 0)
    //            return;
    //        if (add)
    //            SetQtyOnHand(Decimal.Add(GetQtyOnHand(), qty));
    //        else
    //            SetQtyOnHand(Decimal.Subtract(GetQtyOnHand(), qty));
    //    }

    //    /**
    //     * 	Get VAM_Warehouse_ID of Locator
    //     *	@return warehouse
    //     */
    //    public int GetVAM_Warehouse_ID()
    //    {
    //        if (_VAM_Warehouse_ID == 0)
    //        {
    //            MVAMLocator loc = MVAMLocator.Get(GetCtx(), GetVAM_Locator_ID());
    //            _VAM_Warehouse_ID = loc.GetVAM_Warehouse_ID();
    //        }
    //        return _VAM_Warehouse_ID;
    //    }

    //    /**
    //     *	String Representation
    //     * 	@return info
    //     */
    //    public override String ToString()
    //    {
    //        StringBuilder sb = new StringBuilder("MVAMStorage[")
    //            .Append("VAM_Locator_ID=").Append(GetVAM_Locator_ID())
    //                .Append(",VAM_Product_ID=").Append(GetVAM_Product_ID())
    //                .Append(",VAM_PFeature_SetInstance_ID=").Append(GetVAM_PFeature_SetInstance_ID())
    //            .Append(": OnHand=").Append(GetQtyOnHand())
    //            .Append(",Reserved=").Append(GetQtyReserved())
    //            .Append(",Ordered=").Append(GetQtyOrdered())
    //            .Append("]");
    //        return sb.ToString();
    //    }
    //}
}
