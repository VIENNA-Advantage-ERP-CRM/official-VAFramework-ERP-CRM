using System;
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
using System.Data.SqlClient;
using VAdvantage.Logging;


namespace VAdvantage.Model
{

    public class Storage
    {
        #region Private Variables
        //Logger 
        private static VLogger _log = VLogger.GetVLogger(typeof(Storage).FullName);
        #endregion

        //////////////////////// Value Object Class
        public class VO
        {
            #region Private Variables
            private int productID;
            private int attributeSetInstanceID;
            private int locatorID;
            private Decimal onHandQty;
            private Decimal dedicatedQty;
            private Decimal allocatedQty;
            #endregion

            public VO(int M_Product_ID, int M_Locator_ID, int M_AttributeSetInstance_ID, Decimal onhandQty,
                    Decimal dedicatedQty, Decimal allocatedQty)
            {
                this.productID = M_Product_ID;
                this.attributeSetInstanceID = M_AttributeSetInstance_ID;
                this.locatorID = M_Locator_ID;
                this.onHandQty = onhandQty;
                this.dedicatedQty = dedicatedQty;
                this.allocatedQty = allocatedQty;
            }

            public int GetM_AttributeSetInstance_ID()
            {
                return attributeSetInstanceID;
            }

            public int GetM_Locator_ID()
            {
                return locatorID;
            }

            public int GetM_Product_ID()
            {
                return productID;
            }

            public Decimal GetAvailableQty()
            {
                return Decimal.Subtract(Decimal.Subtract(GetQtyOnHand(), GetQtyDedicated()),
                        GetQtyAllocated());
            }

            public Decimal GetQtyOnHand()
            {
                return onHandQty;
            }

            public void SetQtyOnHand(Decimal qty)
            {
                onHandQty = qty;
            }

            public Decimal GetQtyDedicated()
            {
                return dedicatedQty;
            }

            public Decimal GetQtyAllocated()
            {
                return allocatedQty;
            }
        }

        //////////////////////// Record Class

        public class Record
        {
            #region Private Variables
            private Dictionary<String, MStorage> storageMap = new Dictionary<String, MStorage>();
            // reference storage detail for location, product and ASI info
            private MStorage refDetail;

            #endregion

            public Record(Ctx ctx, int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
                    List<String> types, Trx trx)
            {
                if ((types == null) || types.Count < 1)
                {
                    throw new Exception("Quantity types must not be null or empty");
                }
                List<MStorage> storages = MStorage.GetMultipleForUpdate(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, types, trx);
                foreach (MStorage storage in storages)
                {
                    //storageMap.put(X_Ref_Quantity_Type.getEnum(storage.getQtyType()), storage);
                    storageMap[X_Ref_Quantity_Type.GetEnum(storage.GetQtyType())] = storage;
                    refDetail = storage;
                }
            }

            public Record(List<MStorage> storageDetails)
            {
                foreach (MStorage storage in storageDetails)
                {
                    storageMap[X_Ref_Quantity_Type.GetEnum(storage.GetQtyType())] = storage;
                    refDetail = storage;
                }
            }

            /// <summary>
            /// Gets a storage detail of specificied qty type
            /// If none found, create it
            /// </summary>
            /// <param name="qtyType">quantity type to get</param>
            /// <returns>storage detail for the quantity type</returns>
            public MStorage GetDetail(String qtyType)
            {
                return GetDetail(qtyType, true);
            }

            /// <summary>
            /// Gets a storage detail of specificied qty type
            /// </summary>
            /// <param name="qtyType">Quantity type to get</param>
            /// <param name="toCreate">if true, create detail if not found</param>
            /// <returns>storage detail for the quantity type</returns>
            public MStorage GetDetail(String qtyType, Boolean toCreate)
            {
                MStorage detail = storageMap[qtyType];
                if (detail == null)
                {
                    detail = MStorage.GetForRead(refDetail.GetCtx(), refDetail.GetM_Locator_ID(),
                            refDetail.GetM_Product_ID(), refDetail.GetM_AttributeSetInstance_ID(),
                            qtyType, refDetail.Get_Trx());

                    if (detail == null && toCreate)
                    {
                        detail = MStorage.GetCreate(refDetail.GetCtx(), refDetail.GetM_Locator_ID(),
                                refDetail.GetM_Product_ID(), refDetail.GetM_AttributeSetInstance_ID(),
                                 refDetail.Get_Trx());// qtyType, refDetail.Get_Trx());

                    }
                    storageMap[qtyType] = detail;
                }
                return detail;
            }

            public int GetM_AttributeSetInstance_ID()
            {
                return refDetail.GetM_AttributeSetInstance_ID();
            }

            public int GetM_Locator_ID()
            {
                return refDetail.GetM_Locator_ID();
            }

            public int GetM_Product_ID()
            {
                return refDetail.GetM_Product_ID();
            }

            public int GetM_Warehouse_ID()
            {
                return refDetail.GetM_Warehouse_ID();
            }

            public Decimal GetAvailableQty()
            {
                return Decimal.Subtract(Decimal.Subtract(GetQtyOnHand(), GetQtyAllocated()), GetQtyDedicated());
            }

            public Decimal GetQtyOnHand() { return GetQty(X_Ref_Quantity_Type.ON_HAND); }
            public Decimal GetQtyAllocated() { return GetQty(X_Ref_Quantity_Type.ALLOCATED); }
            public Decimal GetQtyDedicated() { return GetQty(X_Ref_Quantity_Type.DEDICATED); }
            public Decimal GetQtyOrdered() { return GetQty(X_Ref_Quantity_Type.ORDERED); }
            public Decimal GetQtyExpected() { return GetQty(X_Ref_Quantity_Type.EXPECTED); }

            public Decimal GetQty(String qtyType)
            {
                MStorage detail = storageMap[qtyType];
                return ((detail == null) ? Env.ZERO : detail.GetQty());
            }

            public void SetDetailsBulkUpdate(Boolean bulkUpdate)
            {
                foreach (MStorage storage in storageMap.Values)
                {
                    //storage.SetIsBulkUpdate(bulkUpdate);
                    MessageBox.Show("bulkUpdate of StorageClass");
                }
            }

            public Boolean Validate()
            {
                MWarehouse wh = MWarehouse.Get(refDetail.GetCtx(), GetM_Warehouse_ID());

                if (wh.IsDisallowNegativeInv())
                {
                    if (Env.Signum(GetQtyOnHand()) < 0 ||
                            Env.Signum(GetQtyDedicated()) < 0 ||
                            Env.Signum(GetQtyAllocated()) < 0 ||
                            Env.Signum(GetQtyExpected()) < 0)
                    {
                        _log.SaveError("Error", Msg.GetMsg(refDetail.GetCtx(), "NegativeInventoryDisallowed"));
                        return false;
                    }

                    Decimal qtyOnHand = Env.ZERO;
                    Decimal qtyDedicated = Env.ZERO;
                    Decimal qtyAllocated = Env.ZERO;

                    String sql = "SELECT COALESCE(SUM(QtyOnHand),0),COALESCE(SUM(QtyDedicated),0),COALESCE(SUM(QtyAllocated),0) "
                        + "FROM M_Storage_V s"
                        + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) "
                        + "WHERE s.M_Product_ID=" + GetM_Product_ID()		//	#1
                        + " AND l.M_Warehouse_ID=" + GetM_Warehouse_ID()
                        + " AND l.M_Locator_ID=" + GetM_Locator_ID()
                        + " AND M_AttributeSetInstance_ID<>" + GetM_AttributeSetInstance_ID();

                    IDataReader idr = null;

                    try
                    {
                        idr = DB.ExecuteReader(sql, null, refDetail.Get_TrxName());
                        if (idr.Read())
                        {
                            qtyOnHand = Util.GetValueOfDecimal(idr[0]);
                            qtyDedicated = Util.GetValueOfDecimal(idr[1]);
                            qtyAllocated = Util.GetValueOfDecimal(idr[2]);
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

                    qtyOnHand = Decimal.Add(qtyOnHand, GetQtyOnHand());
                    qtyDedicated = Decimal.Add(qtyDedicated, GetQtyDedicated());
                    qtyAllocated = Decimal.Add(qtyAllocated, GetQtyAllocated());

                    if (Env.Signum(qtyOnHand) < 0 ||
                            qtyOnHand.CompareTo(Decimal.Add(qtyDedicated, qtyAllocated)) < 0 ||
                            GetQtyOnHand().CompareTo(Decimal.Add(GetQtyDedicated(), GetQtyAllocated())) < 0)
                    {
                        _log.SaveError("Error", Msg.GetMsg(refDetail.GetCtx(), "NegativeInventoryDisallowed"));
                        return false;
                    }
                }
                return true;
            }
        }

        /////////////////////////////////////////////
        public static List<Storage.VO> GetWarehouse(Ctx ctx,
                int M_Warehouse_ID, int M_Product_ID,
                int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
                Boolean allAttributeInstances, DateTime? minGuaranteeDate,
                Boolean FiFo, Trx trx)
        {
            return GetWarehouse(ctx, M_Warehouse_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, M_AttributeSet_ID,
                    allAttributeInstances, minGuaranteeDate, FiFo, false, 0, trx);
        }

        /// <summary>
        /// Create storage for qty types that don't exist and return specified qty type 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="qtyTypeToGet"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static MStorage GetCreateDetails(Ctx ctx, int M_Locator_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, String qtyTypeToGet, Trx trx)
        {
            MStorage detailToGet = null;
            // create storage for all qty types
            foreach (var qtyType in X_Ref_Quantity_Type.Get())
            {
                MStorage detail = MStorage.GetCreate(ctx, M_Locator_ID, M_Product_ID,
                        M_AttributeSetInstance_ID, trx);// M_AttributeSetInstance_ID, qtyType, trx);
                detail.Save(trx);
                if (qtyType == qtyTypeToGet)
                    detailToGet = detail;
            }
            return detailToGet;
        }

        /// <summary>
        /// Create storage for qty types that don't exist and return specified qty type
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static Storage.Record GetCreateRecord(Ctx ctx, int M_Locator_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, Trx trx)
        {
            List<MStorage> details = new List<MStorage>();
            // create storage for all qty types
            foreach (var qtyType in X_Ref_Quantity_Type.Get())
            {
                MStorage detail = MStorage.GetCreate(ctx, M_Locator_ID, M_Product_ID,
                        M_AttributeSetInstance_ID, trx);//M_AttributeSetInstance_ID, qtyType, trx);
                detail.Save(trx);
                details.Add(detail);
            }
            return new Storage.Record(details);
        }

        /////////////////////////////////////////////

        /// <summary>
        /// Get all Storages for Product
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_Locator_ID"><locator/param>
        /// <param name="qtyToDeliver"></param>
        /// <param name="trx"><transaction/param>
        /// <returns>existing or null</returns>
        public static List<Storage.Record> GetAll(Ctx ctx,
                int M_Product_ID, int M_Locator_ID, Decimal qtyToDeliver, Trx trx)
        {
            List<MStorage> details = new List<MStorage>();
            List<Storage.Record> storages = new List<Storage.Record>();

            String sql = "select s.* "
                + " from M_STORAGE  s "
                + " where s.M_PRODUCT_ID =" + M_Product_ID + " and s.M_LOCATOR_ID = " + M_Locator_ID
                + " and s.QTYTYPE in ('H','A','D') "
                + " and exists (select 1 from M_STORAGE t "
                                + " where t.QTYTYPE = 'H' "
                                + " and t.QTY <> 0 "
                                + " and s.AD_CLIENT_ID = t.AD_CLIENT_ID "
                                + " and s.AD_ORG_ID    = t.AD_ORG_ID "
                                + " and s.M_PRODUCT_ID = t.M_PRODUCT_ID "
                                + " and s.M_LOCATOR_ID = t.M_LOCATOR_ID  "
                                + " and s.M_AttributeSetInstance_ID = t.M_AttributeSetInstance_ID ) "
                + " order by s.M_ATTRIBUTESETINSTANCE_ID ";

            if (DB.IsOracle())
                sql += " FOR UPDATE OF s.QTY ";
            else
                sql += " FOR UPDATE OF s ";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                int prevASI = -1;
                foreach (DataRow dr in dt.Rows)
                {

                    MStorage detail = new MStorage(ctx, dr, trx);
                    if (prevASI == -1)
                    {
                        prevASI = detail.GetM_AttributeSetInstance_ID();
                    }
                    if (detail.GetM_AttributeSetInstance_ID() != prevASI)
                    {
                        Storage.Record record = new Storage.Record(details);
                        storages.Add(record);
                        details.Clear();
                    }
                    details.Add(detail);
                }
                if (details.Count > 0)
                {
                    Storage.Record record = new Storage.Record(details);
                    storages.Add(record);
                    details.Clear();
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }

            List<Storage.Record> availableStorages = new List<Storage.Record>();
            foreach (Storage.Record storage in storages)
            {
                if (storage.GetAvailableQty().CompareTo(Env.ZERO) <= 0)
                {
                    continue;
                }
                availableStorages.Add(storage);
                //if (qtyToDeliver != null)
                {
                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetAvailableQty());
                    if (qtyToDeliver.CompareTo(Env.ZERO) <= 0)
                    {
                        break;
                    }
                }
            }
            return availableStorages;
        }

        /// <summary>
        /// Get all Storages for Product with ASI
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="qtyToDeliver">the quantity to deliver, or null if no limit</param>
        /// <param name="trx">transaction</param>
        /// <returns>existing or null</returns>
        public static List<Storage.Record> GetAllWithASI(Ctx ctx, int M_Product_ID, int M_Locator_ID,
                Boolean FiFo, Decimal qtyToDeliver, Trx trx)
        {
            List<MStorage> details = new List<MStorage>();
            List<Storage.Record> storages = new List<Storage.Record>();
            //String sql = "select s.* "
            //    + " from M_STORAGEDETAIL  s "
            //    + " where s.M_PRODUCT_ID =" + M_Product_ID + "  and s.M_LOCATOR_ID =" + M_Locator_ID
            //    + " and s.QTYTYPE in ('H','A','D') "
            //    + " and s.M_AttributeSetInstance_ID > 0 "
            //    + " and exists (select 1 from M_STORAGEDETAIL t "
            //                    + " where t.QTYTYPE = 'H' "
            //                    + " and t.QTY <> 0 "
            //                    + " and s.AD_CLIENT_ID = t.AD_CLIENT_ID "
            //                    + " and s.AD_ORG_ID    = t.AD_ORG_ID "
            //                    + " and s.M_PRODUCT_ID = t.M_PRODUCT_ID "
            //                    + " and s.M_LOCATOR_ID = t.M_LOCATOR_ID  "
            //                    + " and s.M_AttributeSetInstance_ID = t.M_AttributeSetInstance_ID ) "
            //    + " order by s.M_ATTRIBUTESETINSTANCE_ID ";
            String sql = " SELECT s.*" +
   " FROM M_STORAGE s" +
  " where s.M_PRODUCT_ID =" + M_Product_ID + "  and s.M_LOCATOR_ID =" + M_Locator_ID +
" AND s.M_AttributeSetInstance_ID > 0 " +
" AND EXISTS  (SELECT 1     FROM M_STORAGE t WHERE t.qtyonhand <> 0" +
  " AND s.AD_CLIENT_ID              = t.AD_CLIENT_ID" +
  " AND s.AD_ORG_ID                 = t.AD_ORG_ID" +
  " AND s.M_PRODUCT_ID              = t.M_PRODUCT_ID" +
  " AND s.M_LOCATOR_ID              = t.M_LOCATOR_ID" +
  " AND s.M_AttributeSetInstance_ID = t.M_AttributeSetInstance_ID" +
            " )ORDER BY s.M_ATTRIBUTESETINSTANCE_ID";
            if (!FiFo)
            {
                sql += " DESC ";
            }
            if (DB.IsOracle())
            {
                //sql += " FOR UPDATE of s.QTY ";
                sql += " FOR UPDATE of s.qtyonhand ";//for Storage test
            }
            else
            {
                sql += " FOR UPDATE of s";
            }

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                int prevASI = -1;
                foreach (DataRow dr in dt.Rows)
                {
                    //MStorage detail = new MStorage(ctx, dr, trx);
                    //if (prevASI == -1)
                    //{
                    //    prevASI = detail.GetM_AttributeSetInstance_ID();
                    //}

                    //if (detail.GetM_AttributeSetInstance_ID() != prevASI)
                    //{
                    //    Storage.Record record = new Storage.Record(details);
                    //    storages.Add(record);
                    //    details.Clear();
                    //    prevASI = detail.GetM_AttributeSetInstance_ID();
                    //}
                    //details.Add(detail);
                    MStorage detail = new MStorage(ctx, dr, trx);
                    if (prevASI == -1)
                    {
                        prevASI = detail.GetM_AttributeSetInstance_ID();
                    }

                    if (detail.GetM_AttributeSetInstance_ID() != prevASI)
                    {
                        Storage.Record record = new Storage.Record(details);
                        storages.Add(record);
                        details.Clear();
                        prevASI = detail.GetM_AttributeSetInstance_ID();
                    }
                    details.Add(detail);
                }
                if (details.Count > 0)
                {
                    Storage.Record record = new Storage.Record(details);
                    storages.Add(record);
                    details.Clear();
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }

            List<Storage.Record> availableStorages = new List<Storage.Record>();
            foreach (Storage.Record storage in storages)
            {
                if (storage.GetAvailableQty().CompareTo(Env.ZERO) <= 0)
                {
                    continue;
                }
                availableStorages.Add(storage);
               // if (qtyToDeliver != null)
                {
                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetAvailableQty());
                    if (qtyToDeliver.CompareTo(Env.ZERO) <= 0)
                    {
                        break;
                    }
                }
            }
            return availableStorages;
        }

        /// <summary>
        /// Get Storage Info for Warehouse
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Warehouse_ID"></param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <param name="allAttributeInstances">if true, all attribute set instances</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="allocationCheck"></param>
        /// <param name="M_SourceZone_ID"></param>
        /// <param name="trx">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static List<Storage.VO> GetWarehouse(Ctx ctx, int M_Warehouse_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                int M_AttributeSet_ID, Boolean allAttributeInstances,
                DateTime? minGuaranteeDate, Boolean FiFo, Boolean allocationCheck,
                int M_SourceZone_ID, Trx trx)
        {
            if (M_Warehouse_ID == 0 || M_Product_ID == 0)
            {
                return null;
            }

            if (M_AttributeSet_ID == 0)
            {
                allAttributeInstances = true;
            }
            else
            {
                MAttributeSet mas = MAttributeSet.Get(ctx, M_AttributeSet_ID);
                if (!mas.IsInstanceAttribute())
                {
                    allAttributeInstances = true;
                }
            }

            List<Storage.VO> list = new List<Storage.VO>();
            String sql = null;
            // All Attribute Set Instances
            if (allAttributeInstances)
            {
                sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
                        + "COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END),0) QtyOnhand,"
                        + "COALESCE(SUM(CASE WHEN QtyType LIKE 'D' THEN Qty ELSE 0 END),0) QtyDedicated,"
                        + "COALESCE(SUM(CASE WHEN QtyType LIKE 'A' THEN Qty ELSE 0 END),0) QtyAllocated "
                        + "FROM M_STORAGE s"
                        + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID)"
                        + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID) "
                        + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID + " AND s.M_Product_ID=" + M_Product_ID;

                if (allocationCheck)
                {
                    sql += "AND l.IsAvailableForAllocation='Y' ";
                }

                if (M_SourceZone_ID != 0)
                {
                    sql += "AND l.M_Locator_ID IN "
                            + " (SELECT M_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID =" + M_SourceZone_ID + " ) ";
                }

                if (minGuaranteeDate != null)
                {
                    sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>'" + minGuaranteeDate + "') "//check date formate minGuaranteeDate.ToString("dd-MMM-yyyy")
                            + "GROUP BY asi.GuaranteeDate, l.PriorityNo, s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID "
                            + "ORDER BY asi.GuaranteeDate, l.PriorityNo DESC, M_AttributeSetInstance_ID";
                }
                else
                {
                    sql += "GROUP BY l.PriorityNo, s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID "
                            + "ORDER BY l.PriorityNo DESC, s.M_AttributeSetInstance_ID";
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
                sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
                        + "COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END),0) QtyOnhand,"
                        + "COALESCE(SUM(CASE WHEN QtyType LIKE 'D' THEN Qty ELSE 0 END),0) QtyDedicated,"
                        + "COALESCE(SUM(CASE WHEN QtyType LIKE 'A' THEN Qty ELSE 0 END),0) QtyAllocated "
                        + "FROM M_STORAGE s"
                        + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) "
                        + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                        + " AND s.M_Product_ID=" + M_Product_ID
                        + " AND COALESCE(s.M_AttributeSetInstance_ID,0)=" + M_AttributeSetInstance_ID;

                if (allocationCheck)
                {
                    sql += "AND l.IsAvailableForAllocation='Y' ";
                }

                if (M_SourceZone_ID != 0)
                {
                    sql += "AND l.M_Locator_ID IN "
                            + " (SELECT M_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID =" + M_SourceZone_ID + ") ";
                }
                sql += "GROUP BY l.PriorityNo, s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID "
                        + "ORDER BY l.PriorityNo DESC, M_AttributeSetInstance_ID";

                if (!FiFo)
                {
                    sql += " DESC";
                }
            }
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                while (idr.Read())
                {
                    int index = 1;
                    int rs_M_Product_ID = Util.GetValueOfInt(idr[index++]);
                    int rs_M_Locator_ID = Util.GetValueOfInt(idr[index++]);
                    int rs_M_AttributeSetInstance_ID = Util.GetValueOfInt(idr[index++]);
                    Decimal rs_QtyOnhand = Util.GetValueOfDecimal(idr[index++]);
                    Decimal rs_QtyDedicated = Util.GetValueOfDecimal(idr[index++]);
                    Decimal rs_QtyAllocated = Util.GetValueOfDecimal(idr[index++]);

                    list.Add(new Storage.VO(rs_M_Product_ID, rs_M_Locator_ID,
                            rs_M_AttributeSetInstance_ID,
                            rs_QtyOnhand,
                            rs_QtyDedicated, rs_QtyAllocated));
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
            return list;
        }

        /// <summary>
        /// Get Available Qty. The call is accurate only if there is a storage record
        /// and assumes that the product is stocked
        /// </summary>
        /// <param name="M_Warehouse_ID"></param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="trx"></param>
        /// <returns>qty available (QtyOnHand-QtyReserved) or null</returns>
        public static Decimal GetQtyAvailable(int M_Warehouse_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID, Trx trx)
        {
            Decimal QtyOnHand = Env.ZERO;
            Decimal QtyReserved = Env.ZERO;

            String sql = "SELECT COALESCE(SUM(CASE WHEN QtyType LIKE 'H' AND l.IsAvailableToPromise = 'Y' THEN Qty ELSE 0 END), 0) QtyOnHand, "
                    + "COALESCE(SUM(CASE WHEN QtyType LIKE 'R' THEN Qty ELSE 0 END), 0) QtyReserved "
                    + "FROM M_STORAGE s "
                    + "INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) "
                    + "WHERE s.IsActive = 'Y' AND s.M_Product_ID= " + M_Product_ID // #1
                    + "AND l.IsActive = 'Y' AND l.M_Warehouse_ID= " + M_Warehouse_ID;
            if (M_AttributeSetInstance_ID != 0)
            {
                sql += "AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            }

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                if (idr.Read())
                {
                    QtyOnHand = Util.GetValueOfDecimal(idr[0]);
                    QtyReserved = Util.GetValueOfDecimal(idr[1]);

                    if (idr == null)//if (idr.wasNull())
                    {
                        QtyOnHand = Env.ZERO;
                    }
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

            _log.Fine("M_Warehouse_ID=" + M_Warehouse_ID + ",M_Product_ID="
                    + M_Product_ID + " : " + " QtyOnHand=" + QtyOnHand
                    + ", QtyReserved=" + QtyReserved);
            return Decimal.Subtract(QtyOnHand, QtyReserved);
        }

        /// <summary>
        /// Get Location with highest Locator Priority and a sufficient OnHand Qty
        /// </summary>
        /// <param name="M_Warehouse_ID">warehouse</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="Qty"></param>
        /// <param name="trx">transaction</param>
        /// <returns></returns>
        public static int GetLocatorID(int M_Warehouse_ID, int M_Product_ID,
                int M_AttributeSetInstance_ID, Decimal Qty, Trx trx)
        {
            int M_Locator_ID = 0;
            int firstM_Locator_ID = 0;
            String sql = "SELECT s.M_Locator_ID, s.Qty "
                    + "FROM M_STORAGE s"
                    + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
                    + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
                    + " LEFT OUTER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
                    + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                    + " AND s.M_Product_ID=" + M_Product_ID
                    + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ")"
                    + " AND l.IsActive='Y' " + " AND s.QtyType='H' "
                    + "ORDER BY l.PriorityNo DESC, s.Qty DESC";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                while (idr.Read())
                {
                    Decimal QtyOnHand = Util.GetValueOfDecimal(idr[1]);
                    if ( Qty.CompareTo(QtyOnHand) <= 0)
                    {
                        M_Locator_ID = Util.GetValueOfInt(idr[0]);
                        break;
                    }
                    if (firstM_Locator_ID == 0)
                    {
                        firstM_Locator_ID = Util.GetValueOfInt(idr[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }

            if (M_Locator_ID != 0)
                return M_Locator_ID;
            return firstM_Locator_ID;
        }


        public static Decimal GetProductQty(Ctx ctx, int M_Product_ID,
                X_Ref_Quantity_Type qtyType, Trx trx)
        {
            String sql = "SELECT COALESCE(SUM(QTY),0) FROM M_STORAGE "
                   + " WHERE M_Product_ID=" + M_Product_ID + " AND QtyType='" + qtyType.GetValue() + "'";
            Decimal qty = Decimal.Zero;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                if (idr.Read())
                {
                    qty = Util.GetValueOfDecimal(idr[0]);
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return qty;
        }

        /// <summary>
        /// ADD QUANTITIES
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Warehouse_ID"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="diffQtyOnHand"></param>
        /// <param name="diffQtyReserved"></param>
        /// <param name="diffQtyOrdered"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static Boolean AddQtys(Ctx ctx, int M_Warehouse_ID,
                int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
                Decimal diffQtyOnHand,
                Decimal diffQtyReserved, Decimal diffQtyOrdered, Trx trx)
        {
            return AddQtys(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, M_AttributeSetInstance_ID,
                    diffQtyOnHand, diffQtyReserved, diffQtyOrdered,
                    Env.ZERO, Env.ZERO, Env.ZERO, trx);
        }

        public static Boolean AddQtys(Ctx ctx, int M_Warehouse_ID,
                int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
                Decimal diffQtyOnHand, Decimal diffQtyReserved, Decimal diffQtyOrdered,
                Decimal diffQtyDedicated, Decimal diffQtyExpected, Decimal diffQtyAllocated,
                Trx trx)
        {
            return AddQtys(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, M_AttributeSetInstance_ID,
                    diffQtyOnHand, diffQtyReserved, diffQtyOrdered,
                    diffQtyDedicated, diffQtyExpected, diffQtyAllocated,
                    trx);
        }

        public static Boolean AddQtys(Ctx ctx, int M_Warehouse_ID,
                int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal diffQtyOnHand,
                Decimal diffQtyReserved, Decimal diffQtyOrdered, Trx trx)
        {
            return AddQtys(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, reservationAttributeSetInstance_ID,
                    diffQtyOnHand, diffQtyReserved, diffQtyOrdered, Env.ZERO,
                    Env.ZERO, Env.ZERO, trx);
        }

        public static Boolean AddQtys(Ctx ctx, int M_Warehouse_ID,
                int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal diffQtyOnHand,
                Decimal diffQtyReserved, Decimal diffQtyOrdered,
                Decimal diffQtyDedicated, Decimal diffQtyExpected,
                Decimal diffQtyAllocated, Trx trx)
        {
            if (diffQtyOnHand.CompareTo(Env.ZERO) != 0)
            {
                if (!MStorage.Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                        M_AttributeSetInstance_ID,
                        reservationAttributeSetInstance_ID, diffQtyOnHand, 0, 0
                        , trx))
                    return false;
            }
            if ( diffQtyReserved.CompareTo(Env.ZERO) != 0)
            {
                if (!MStorage.Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                        M_AttributeSetInstance_ID,
                        reservationAttributeSetInstance_ID, 0, diffQtyReserved, 0
                        , trx))
                    return false;
            }
            if (diffQtyOrdered.CompareTo(Env.ZERO) != 0)
            {
                if (!MStorage.Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                        M_AttributeSetInstance_ID,
                        reservationAttributeSetInstance_ID, 0, 0, diffQtyOrdered,
                         trx))
                    return false;
            }
            if ( diffQtyDedicated.CompareTo(Env.ZERO) != 0)
            {
                //if (!MStorage.Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                //        M_AttributeSetInstance_ID,
                //        reservationAttributeSetInstance_ID, diffQtyDedicated,
                //        X_Ref_Quantity_Type.DEDICATED, trx))
                //    return false;
                MessageBox.Show("Storage-AddQtys-if (diffQtyDedicated != null && diffQtyDedicated.CompareTo(Env.ZERO) != 0)");
            }
            if (diffQtyAllocated.CompareTo(Env.ZERO) != 0)
            {
                //if (!MStorage.Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                //        M_AttributeSetInstance_ID,
                //        reservationAttributeSetInstance_ID, diffQtyAllocated,
                //        X_Ref_Quantity_Type.ALLOCATED, trx))
                //    return false;
                MessageBox.Show("Storage-AddQtys-if (diffQtyAllocated != null && diffQtyAllocated.CompareTo(Env.ZERO) != 0)");
            }
            if ( diffQtyExpected.CompareTo(Env.ZERO) != 0)
            {
                //if (!MStorage.Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                //        M_AttributeSetInstance_ID,
                //        reservationAttributeSetInstance_ID, diffQtyExpected,
                //        X_Ref_Quantity_Type.EXPECTED, trx))
                //    return false;
                MessageBox.Show("Storage-AddQtys- if (diffQtyExpected != null && diffQtyExpected.CompareTo(Env.ZERO) != 0)");
            }
            return true;
        }
    }
}
