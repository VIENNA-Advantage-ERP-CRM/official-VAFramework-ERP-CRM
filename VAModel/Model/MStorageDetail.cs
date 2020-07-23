using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.WF;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Print;
using System.Diagnostics;
using VAdvantage.Controller;
//using VAdvantage.Server;
using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Tcp;
using System.Data;

namespace VAdvantage.Model
{
    public class MStorageDetail : X_M_StorageDetail
    {
        #region Private Variables
        // Logger for class MStorageDetail 
        private static VLogger s_log = VLogger.GetVLogger(typeof(MStorageDetail).FullName);
        private const long SERIALVERSIONUID = 1L;
        private Boolean isBulkUpdate = false;

        //Warehouse 
        private int _M_Warehouse_ID = 0;

        #endregion

        /// <summary>
        /// Standard Constructors
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="type"></param>
        public MStorageDetail(MLocator locator, int M_Product_ID,
                int M_AttributeSetInstance_ID, String type)
            : this(locator.GetCtx(), 0, locator.Get_TrxName())
        {
            SetClientOrg(locator);
            SetM_Locator_ID(locator.GetM_Locator_ID());
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetQtyType(type);//type.GetValue()
            SetQty(Env.ZERO);
        }

        public MStorageDetail(Ctx ctx, int M_Locator_ID, int M_Product_ID,
                int M_AttributeSetInstance_ID, X_Ref_Quantity_Type type, Trx trx)
            : this(ctx, 0, trx)
        {
            SetM_Locator_ID(M_Locator_ID);
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetQtyType(type.GetValue());
            SetQty(Env.ZERO);
        }

        private MStorageDetail(Ctx ctx, int M_StorageDetail_ID, Trx trx)
            : base(ctx, M_StorageDetail_ID, trx)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trx"></param>
        public MStorageDetail(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //Check for lock on Locator and Product
            //if (Is_ValueChanged("Qty") && IsType(X_Ref_Quantity_Type.ON_HAND) &&
            //        MCycleCountLock.LockExists(GetCtx(), GetM_Product_ID(), GetM_Locator_ID(), Get_TrxName()))
            //{
            //    s_log.SaveError("Error", Msg.GetMsg(GetCtx(), "LocatorLocked"));
            //    return false;
            //}

            if (isBulkUpdate)
            {
                // validation must already done at storage level not detail/type level
                // see Storage.Record.validate() 
                return base.BeforeSave(newRecord);
            }

            //	Negative Inventory check
            if (newRecord || (Is_ValueChanged("Qty") &&
                        (IsType(X_Ref_Quantity_Type.ON_HAND)
                         || IsType(X_Ref_Quantity_Type.DEDICATED)
                         || IsType(X_Ref_Quantity_Type.ALLOCATED)
                         || IsType(X_Ref_Quantity_Type.EXPECTED)))
                )
            {

                MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());

                if (wh.IsDisallowNegativeInv())
                {
                    if (Env.Signum(GetQty()) < 0)
                    {
                        s_log.SaveError("Error", Msg.GetMsg(GetCtx(), "NegativeInventoryDisallowed"));
                        return false;
                    }
                    Decimal qtyOnHand = Env.ZERO;
                    Decimal qtyDedicated = Env.ZERO;
                    Decimal qtyAllocated = Env.ZERO;

                    String sql = "SELECT SUM(QtyOnHand),SUM(QtyDedicated),SUM(QtyAllocated) "
                        + "FROM M_Storage_V s"
                        + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) "
                        + "WHERE s.M_Product_ID=" + GetM_Product_ID()		//	#1
                        + " AND l.M_Warehouse_ID=" + GetM_Warehouse_ID()
                        + " AND l.M_Locator_ID=" + GetM_Locator_ID();
                    IDataReader idr = null;
                    try
                    {
                        idr = DB.ExecuteReader(sql, null, Get_TrxName());
                        if (idr.Read())
                        {
                            qtyOnHand = VAdvantage.Utility.Util.GetValueOfDecimal(idr[0]);
                            if (idr.IsDBNull(0))//if (idr.wasNull())
                            {
                                qtyOnHand = Env.ZERO;
                            }

                            qtyDedicated = VAdvantage.Utility.Util.GetValueOfDecimal(idr[1]);
                            if (idr.IsDBNull(1))// wasNull())
                                qtyDedicated = Env.ZERO;

                            qtyAllocated = VAdvantage.Utility.Util.GetValueOfDecimal(idr[2]);
                            if (idr.IsDBNull(2))// wasNull())
                                qtyAllocated = Env.ZERO;
                        }
                        idr.Close();
                    }
                    catch (Exception e)
                    {
                        s_log.Log(Level.SEVERE, sql, e);
                    }
                    finally
                    {
                        if (idr != null)
                        {
                            idr.Close();
                            idr = null;
                        }
                    }
                    Decimal? asiQtyOnHand = Decimal.Zero;
                    Decimal? asiQtyDedicated = Decimal.Zero;
                    Decimal? asiQtyAllocated = Decimal.Zero;

                    if (IsType(X_Ref_Quantity_Type.ON_HAND))
                    {
                        if (newRecord)
                        {
                            qtyOnHand = Decimal.Add(qtyOnHand, GetQty());
                        }
                        else
                        {
                            qtyOnHand = Decimal.Subtract(Decimal.Add(qtyOnHand, GetQty()), Util.GetValueOfDecimal(Get_ValueOld("Qty")));
                        }
                        asiQtyOnHand = GetQty();
                        asiQtyAllocated = MStorageDetail.GetQty(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), X_Ref_Quantity_Type.ALLOCATED, Get_TrxName());
                        asiQtyDedicated = MStorageDetail.GetQty(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), X_Ref_Quantity_Type.DEDICATED, Get_TrxName());
                    }
                    else if (IsType(X_Ref_Quantity_Type.DEDICATED))
                    {
                        if (newRecord)
                            qtyDedicated = Decimal.Add(qtyDedicated, GetQty());
                        else
                        {
                            qtyDedicated = Decimal.Subtract(Decimal.Add(qtyDedicated, GetQty()), Util.GetValueOfDecimal(Get_ValueOld("Qty")));
                        }
                        asiQtyOnHand = MStorageDetail.GetQty(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), X_Ref_Quantity_Type.ON_HAND, Get_TrxName());
                        asiQtyAllocated = MStorageDetail.GetQty(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), X_Ref_Quantity_Type.ALLOCATED, Get_TrxName());
                        asiQtyDedicated = GetQty();
                    }
                    else if (IsType(X_Ref_Quantity_Type.ALLOCATED))
                    {
                        if (newRecord)
                        {
                            qtyAllocated = Decimal.Add(qtyAllocated, GetQty());
                        }
                        else
                        {
                            qtyAllocated = Decimal.Subtract(Decimal.Add(qtyAllocated, GetQty()), Util.GetValueOfDecimal(Get_ValueOld("Qty")));
                        }
                        asiQtyOnHand = MStorageDetail.GetQty(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), X_Ref_Quantity_Type.ON_HAND, Get_TrxName());
                        asiQtyAllocated = GetQty();
                        asiQtyDedicated = MStorageDetail.GetQty(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), X_Ref_Quantity_Type.DEDICATED, Get_TrxName());
                    }
                    asiQtyOnHand = asiQtyOnHand == null ? Env.ZERO : asiQtyOnHand;
                    asiQtyDedicated = asiQtyDedicated == null ? Env.ZERO : asiQtyDedicated;
                    asiQtyAllocated = asiQtyAllocated == null ? Env.ZERO : asiQtyAllocated;

                    if (qtyOnHand.CompareTo(Decimal.Add(qtyDedicated, qtyAllocated)) < 0 ||
                            asiQtyOnHand.Value.CompareTo(Decimal.Add(asiQtyDedicated.Value, asiQtyAllocated.Value)) < 0)
                    {
                        s_log.SaveError("Error", Msg.GetMsg(GetCtx(), "NegativeInventoryDisallowed"));
                        return false;
                    }
                }
            }
            return base.BeforeSave(newRecord);
        }

        /// <summary>
        /// Static Get/update
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="type"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static MStorageDetail GetForRead(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                String type, Trx trx)
        {
            return Get(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    type, false, trx);
        }

        public static MStorageDetail GetForUpdate(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                String type, Trx trx)
        {
            return Get(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    type, true, trx);
        }

        private static MStorageDetail Get(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                String type, Boolean forUpdate, Trx trx)
        {
            MStorageDetail retValue = null;
            String sql = "SELECT * FROM M_StorageDetail "
                    + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND QtyType='" + type + "' AND ";//type.GetValue()
            if (M_AttributeSetInstance_ID == 0)
            {
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            }
            else
            {
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
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
                    retValue = new MStorageDetail(ctx, dt.Rows[0], trx);
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
                s_log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                        + ", M_Product_ID=" + M_Product_ID
                        + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            else
            {
                s_log.Fine("M_Locator_ID=" + M_Locator_ID + ", M_Product_ID="
                        + M_Product_ID + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            }
            return retValue;
        }

        public static Decimal? GetQty(Ctx ctx, int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
                String type, Trx trx)
        {
            Decimal? retValue = Decimal.Zero; ;
            String sql = "SELECT Qty FROM M_StorageDetail "
                    + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND QtyType='" + type + "' AND ";//type.GetValue()
            if (M_AttributeSetInstance_ID == 0)
            {
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            }
            else
            {
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            }

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                if (idr.Read())
                {
                    retValue = Util.GetValueOfDecimal(idr[0]);
                }
                idr.Close();
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
                s_log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                        + ", M_Product_ID=" + M_Product_ID
                        + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            else
            {
                s_log.Fine("M_Locator_ID=" + M_Locator_ID + ", M_Product_ID="
                        + M_Product_ID + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            }
            return retValue;
        }
        //Collection
        public static List<MStorageDetail> GetMultipleForUpdate(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                List<String> types, Trx trx)
        {
            return GetMultiple(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, types, true, trx);
        }
        public static List<MStorageDetail> GetMultipleForRead(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                List<String> types, Trx trx)
        {
            return GetMultiple(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, types, false, trx);
        }

        private static List<MStorageDetail> GetMultiple(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                List<String> types, Boolean forUpdate, Trx trx)
        {
            List<MStorageDetail> retValue = new List<MStorageDetail>();
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

            String sql = "SELECT * FROM M_StorageDetail "
                    + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND ";
            if (M_AttributeSetInstance_ID == 0)
            {
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            }
            else
            {
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
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
                    retValue.Add(new MStorageDetail(ctx, dr, trx));
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
                s_log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                        + ", M_Product_ID=" + M_Product_ID
                        + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            }
            else
            {
                s_log.Fine("M_Locator_ID=" + M_Locator_ID + ", M_Product_ID="
                        + M_Product_ID + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            }
            return retValue;
        }

        public static MStorageDetail GetCreate(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID, String type, Trx trx)
        {
            if (M_Locator_ID == 0)
            {
                throw new ArgumentException("M_Locator_ID=0");
            }
            if (M_Product_ID == 0)
            {
                throw new ArgumentException("M_Product_ID=0");
            }
            MStorageDetail retValue = GetForUpdate(ctx, M_Locator_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, type, trx);
            if (retValue != null)
            {
                return retValue;
            }
            // Insert row based on locator
            MLocator locator = new MLocator(ctx, M_Locator_ID, trx);
            if (locator.Get_ID() != M_Locator_ID)
            {
                throw new ArgumentException("Not found M_Locator_ID="
                        + M_Locator_ID);
            }
            //
            retValue = new MStorageDetail(locator, M_Product_ID,
                    M_AttributeSetInstance_ID, type);
            retValue.Save(trx);
            s_log.Fine("New " + retValue);
            return retValue;
        }

        public static Boolean Set(Ctx ctx, int M_Warehouse_ID, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal newQty,
                String type, Trx trx)
        {
            MStorageDetail retValue = null;
            String sql = "UPDATE M_StorageDetail " + "SET QTY=" + newQty
                    + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND QtyType='" + type + "' AND ";//type.GetValue()
            if (M_AttributeSetInstance_ID == 0)
            {
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            }
            else
            {
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            }

            try
            {
                int i = DB.ExecuteQuery(sql, null, trx);
                if (i != 1)
                {
                    s_log.SaveError("M_StorageDetailNotUpdated", sql);
                }
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }

            if (retValue == null)
                s_log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                        + ", M_Product_ID=" + M_Product_ID
                        + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            else
                s_log.Fine("M_Locator_ID=" + M_Locator_ID + ", M_Product_ID="
                        + M_Product_ID + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            return true;
        }

        /// <summary>
        /// Get all Storages for Product
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="qtyType"></param>
        /// <param name="isFIFO"></param>
        /// <param name="withASI"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static List<MStorageDetail> GetAll(Ctx ctx, int M_Product_ID,
                int M_Locator_ID, X_Ref_Quantity_Type qtyType, Boolean isFIFO,
                Boolean withASI, Trx trx)
        {
            List<MStorageDetail> list = new List<MStorageDetail>();
            String sql = "select s.* "
                    + "from M_STORAGEDETAIL  s INNER JOIN M_STORAGEDETAIL  t "
                    + "on ( s.AD_CLIENT_ID = t.AD_CLIENT_ID "
                    + "and s.AD_ORG_ID    = t.AD_ORG_ID "
                    + "and s.M_PRODUCT_ID = t.M_PRODUCT_ID "
                    + "and s.M_LOCATOR_ID = t.M_LOCATOR_ID) "
                    + "where s.QTYTYPE ='" + qtyType.GetValue() + "' " + "and s.M_PRODUCT_ID =" + M_Product_ID
                    + "and s.M_LOCATOR_ID =" + M_Locator_ID + " and t.QTYTYPE = 'H' ";
            if (withASI)
            {
                sql += "and t.QTY > 0 " + "and s.M_ATTRIBUTESETINSTANCE_ID > 0 ";
            }
            else
            {
                sql += "and t.QTY <> 0 ";
            }
            sql += " order by s.M_ATTRIBUTESETINSTANCE_ID ";

            if (!isFIFO)
            {
                sql += " DESC";
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
                    list.Add(new MStorageDetail(ctx, dr, trx));
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
            return list;
        }


        /*  public static Map<X_Ref_Quantity_Type, MStorageDetail> getAllForUpdate(
                Ctx ctx, int M_Locator_ID, int M_Product_ID,
                int M_AttributeSetInstance_ID,
                Set<X_Ref_Quantity_Type> updateQtyTypes, Trx trx) {
		Map<X_Ref_Quantity_Type, MStorageDetail> storageDetails = new HashMap<X_Ref_Quantity_Type, MStorageDetail>();
		for (X_Ref_Quantity_Type qtyType : X_Ref_Quantity_Type.values()) {
			MStorageDetail store = MStorageDetail.Get(ctx, M_Locator_ID,
					M_Product_ID, M_AttributeSetInstance_ID, qtyType,
					(updateQtyTypes == null ? false : updateQtyTypes
							.contains(qtyType)), trx);
			storageDetails.put(qtyType, store);
		}
		return storageDetails;
	}*/
        public static Dictionary<String, MStorageDetail> GetAllForUpdate(Ctx ctx, int M_Locator_ID, int M_Product_ID,
                int M_AttributeSetInstance_ID, List<String> updateQtyTypes, Trx trx)
        {
            Dictionary<String, MStorageDetail> storageDetails = new Dictionary<String, MStorageDetail>();
            //Array qtyType = Enum.GetValues(typeof(X_Ref_Quantity_Type.X_Ref_Quantity_TypeEnum));
            Array qtyType = X_Ref_Quantity_Type.Get().ToArray();
            for (int i = 0; i < qtyType.Length; i++)
            {
                MStorageDetail store = MStorageDetail.Get(ctx, M_Locator_ID,
                        M_Product_ID, M_AttributeSetInstance_ID, qtyType.GetValue(i).ToString(),
                        (updateQtyTypes == null ? false : updateQtyTypes.Contains(qtyType.GetValue(i).ToString())), trx);
                //storageDetails.put(qtyType, store);
                storageDetails[qtyType.GetValue(i).ToString()] = store;
            }
            return storageDetails;
        }

        /// <summary>
        /// detail Add
        /// Warehouse must already be validated
        /// diffQty must always be positive; negative values are not processed
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Warehouse_ID"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="reservationAttributeSetInstance_ID"></param>
        /// <param name="diffQty"></param>
        /// <param name="type"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static Boolean Add(Ctx ctx, int M_Warehouse_ID, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal diffQty,
                String type, Trx trx)
        {
            StringBuilder diffText = new StringBuilder("(");
            MStorageDetail storage = null;
            storage = MStorageDetail.GetCreate(ctx, M_Locator_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, type, trx);
            // Verify
            if (storage.GetM_Locator_ID() != M_Locator_ID
                    && storage.GetM_Product_ID() != M_Product_ID
                    && storage.GetM_AttributeSetInstance_ID() != M_AttributeSetInstance_ID)
            {
                s_log.Severe("No Storage found - M_Locator_ID=" + M_Locator_ID
                        + ",M_Product_ID=" + M_Product_ID + ",ASI="
                        + M_AttributeSetInstance_ID);
                return false;
            }

            MStorageDetail storageASI = null;
            if ((M_AttributeSetInstance_ID != reservationAttributeSetInstance_ID)
                    && (type == X_Ref_Quantity_Type.RESERVED || type == X_Ref_Quantity_Type.ORDERED))
            {
                int reservationM_Locator_ID = M_Locator_ID;
                if (reservationAttributeSetInstance_ID == 0)
                {
                    MWarehouse wh = MWarehouse.Get(ctx, M_Warehouse_ID);
                    reservationM_Locator_ID = wh.GetDefaultM_Locator_ID();
                }
                storageASI = MStorageDetail.Get(ctx, reservationM_Locator_ID,
                        M_Product_ID, reservationAttributeSetInstance_ID, type,
                        true, trx);
                if (storageASI == null) // create if not existing - should not happen
                {
                    MProduct product = MProduct.Get(ctx, M_Product_ID);
                    int xM_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
                    if (xM_Locator_ID == 0)
                    {
                        MWarehouse wh = MWarehouse.Get(ctx, M_Warehouse_ID);
                        xM_Locator_ID = wh.GetDefaultM_Locator_ID();
                    }
                    storageASI = MStorageDetail.GetCreate(ctx, xM_Locator_ID,
                            M_Product_ID, reservationAttributeSetInstance_ID, type,
                            trx);
                }
            }
            Boolean changed = false;
            if ( Env.Signum(diffQty) != 0)
            {
                if (storageASI == null)
                {
                    storage.SetQty(Decimal.Add(storage.GetQty(), diffQty));
                }
                else
                {
                    storageASI.SetQty(Decimal.Add(storageASI.GetQty(), diffQty));
                }
                diffText.Append(type.ToString()).Append("=").Append(diffQty);
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                s_log.Fine(diffText.ToString());
                if (storageASI != null)
                {
                    storageASI.Save(trx); // No AttributeSetInstance
                }
                // (reserved/ordered)
                return storage.Save(trx);
            }
            return true;
        }

        public static void Transfer(Ctx ctx, int M_Warehouse_ID, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal diffQty,
                String subtractFromType,
                String addToType, Trx trx)
        {
            Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, reservationAttributeSetInstance_ID,
                    Decimal.Negate(diffQty), subtractFromType, trx);

            Add(ctx, M_Warehouse_ID, M_Locator_ID, M_Product_ID,
                    M_AttributeSetInstance_ID, reservationAttributeSetInstance_ID,
                    Decimal.Negate(diffQty), addToType, trx);
        }

        public static Decimal GetForUpdate(Ctx ctx, int M_Warehouse_ID,
                int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
                int reservationAttributeSetInstance_ID, X_Ref_Quantity_Type type,
                Trx trx)
        {
            return Decimal.Zero;
        }

        /// <summary>
        /// Get M_Warehouse_ID of Locator
        /// </summary>
        /// <returns>warehouse</returns>
        public int GetM_Warehouse_ID()
        {
            if (_M_Warehouse_ID == 0)
            {
                MLocator loc = MLocator.Get(GetCtx(), GetM_Locator_ID());
                _M_Warehouse_ID = loc.GetM_Warehouse_ID();
            }
            return _M_Warehouse_ID;
        }

        ///**
        // * Get Storage Info for Warehouse
        // * 
        // * @param ctx
        // *            context
        // * @param M_Warehouse_ID
        // * @param M_Product_ID
        // *            product
        // * @param M_AttributeSetInstance_ID
        // *            instance
        // * @param M_AttributeSet_ID
        // *            attribute set
        // * @param allAttributeInstances
        // *            if true, all attribute set instances
        // * @param minGuaranteeDate
        // *            optional minimum guarantee date if all attribute instances
        // * @param FiFo
        // *            first in-first-out
        // * @param trx
        // *            transaction
        // * @return existing - ordered by location priority (desc) and/or guarantee
        // *         date
        // */
        //public static List<Storage.VO> GetWarehouse(Ctx ctx, int M_Warehouse_ID,
        //        int M_Product_ID, int M_AttributeSetInstance_ID,
        //        int M_AttributeSet_ID, Boolean allAttributeInstances,
        //        Timestamp minGuaranteeDate, Boolean FiFo, Boolean allocationCheck,
        //        int M_SourceZone_ID, Trx trx)
        //{
        //    if (M_Warehouse_ID == 0 || M_Product_ID == 0)
        //        return null;

        //    if (M_AttributeSet_ID == 0)
        //        allAttributeInstances = true;
        //    else
        //    {
        //        MAttributeSet mas = MAttributeSet.Get(ctx, M_AttributeSet_ID);
        //        if (!mas.isInstanceAttribute())
        //            allAttributeInstances = true;
        //    }

        //    List<Storage.VO> list = new ArrayList<Storage.VO>();
        //    // Specific Attribute Set Instance
        //    String sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
        //            + "COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END),0) QtyOnhand,"
        //            + "COALESCE(SUM(CASE WHEN QtyType LIKE 'D' THEN Qty ELSE 0 END),0) QtyDedicated,"
        //            + "COALESCE(SUM(CASE WHEN QtyType LIKE 'A' THEN Qty ELSE 0 END),0) QtyAllocated "
        //            + "FROM M_StorageDetail s"
        //            + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) "
        //            + "WHERE l.M_Warehouse_ID=?"
        //            + " AND s.M_Product_ID=?"
        //            + " AND COALESCE(s.M_AttributeSetInstance_ID,0)=? ";

        //    if (allocationCheck)
        //        sql += "AND l.IsAvailableForAllocation='Y' ";

        //    if (M_SourceZone_ID != 0)
        //        sql += "AND l.M_Locator_ID IN "
        //                + " (SELECT M_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID = ? ) ";
        //    sql += "GROUP BY l.PriorityNo, s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID "
        //            + "ORDER BY l.PriorityNo DESC, M_AttributeSetInstance_ID";

        //    if (!FiFo)
        //        sql += " DESC";
        //    // All Attribute Set Instances
        //    if (allAttributeInstances)
        //    {
        //        sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
        //                + "COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END),0) QtyOnhand,"
        //                + "COALESCE(SUM(CASE WHEN QtyType LIKE 'D' THEN Qty ELSE 0 END),0) QtyDedicated,"
        //                + "COALESCE(SUM(CASE WHEN QtyType LIKE 'A' THEN Qty ELSE 0 END),0) QtyAllocated "
        //                + "FROM M_StorageDetail s"
        //                + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID)"
        //                + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID) "
        //                + "WHERE l.M_Warehouse_ID=?" + " AND s.M_Product_ID=? ";

        //        if (allocationCheck)
        //            sql += "AND l.IsAvailableForAllocation='Y' ";

        //        if (M_SourceZone_ID != 0)
        //            sql += "AND l.M_Locator_ID IN "
        //                    + " (SELECT M_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID = ? ) ";

        //        if (minGuaranteeDate != null)
        //        {
        //            sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>?) "
        //                    + "GROUP BY asi.GuaranteeDate, l.PriorityNo, s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID "
        //                    + "ORDER BY asi.GuaranteeDate, l.PriorityNo DESC, M_AttributeSetInstance_ID";
        //        }
        //        else
        //        {
        //            sql += "GROUP BY l.PriorityNo, s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID "
        //                    + "ORDER BY l.PriorityNo DESC, s.M_AttributeSetInstance_ID";
        //        }
        //        if (!FiFo)
        //            sql += " DESC";
        //        sql += ", COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END),0) DESC";
        //    }
        //    PreparedStatement pstmt = null;
        //    ResultSet dr = null;
        //    try
        //    {
        //        pstmt = DB.prepareStatement(sql, trx);
        //        int index = 1;
        //        pstmt.setInt(index++, M_Warehouse_ID);
        //        pstmt.setInt(index++, M_Product_ID);
        //        if (M_SourceZone_ID != 0)
        //            pstmt.setInt(index++, M_SourceZone_ID);
        //        if (!allAttributeInstances)
        //            pstmt.setInt(index++, M_AttributeSetInstance_ID);
        //        else if (minGuaranteeDate != null)
        //            pstmt.setTimestamp(index++, minGuaranteeDate);
        //        dr = pstmt.executeQuery();
        //        while (dr.next())
        //        {
        //            index = 1;
        //            int rs_M_Product_ID = dr.getInt(1);
        //            int rs_M_Locator_ID = dr.getInt(index++);
        //            int rs_M_AttributeSetInstance_ID = dr.getInt(index++);
        //            Decimal rs_QtyOnhand = dr.getBigDecimal(index++);
        //            Decimal rs_QtyDedicated = dr.getBigDecimal(index++);
        //            Decimal rs_QtyAllocated = dr.getBigDecimal(index++);
        //            list.add(new Storage.VO(rs_M_Product_ID, rs_M_Locator_ID,
        //                    rs_M_AttributeSetInstance_ID, rs_QtyOnhand,
        //                    rs_QtyDedicated, rs_QtyAllocated));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        s_log.s_log(Level.SEVERE, sql, e);
        //    }
        //    finally
        //    {
        //        DB.closeResultSet(dr);
        //        DB.closeStatement(pstmt);
        //    }
        //    return list;
        //} // getWarehouse

        //public static List<Storage.VO> getWarehouse(Ctx ctx,
        //        int M_Warehouse_ID, int M_Product_ID,
        //        int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
        //        Boolean allAttributeInstances, Timestamp minGuaranteeDate,
        //        Boolean FiFo, Trx trx)
        //{
        //    return getWarehouse(ctx, M_Warehouse_ID, M_Product_ID,
        //            M_AttributeSetInstance_ID, M_AttributeSet_ID,
        //            allAttributeInstances, minGuaranteeDate, FiFo, false, 0, trx);
        //}

        /// <summary>
        /// Trace back from storage record to original receipt line.
        /// </summary>
        /// <returns>MInOutLine or null</returns>
        public MInOutLine GetInOutLineOf()
        {
            // Don't try to trace back to receipt line for ASI=0 records
            if (GetM_AttributeSetInstance_ID() == 0)
            {
                return null;
            }
            MInOutLine retValue = null;
            String sql = "SELECT * FROM M_InOutLine line "
                + "WHERE M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID()
                + "OR EXISTS (SELECT 1 FROM "
                + "M_InOutLineMA ma WHERE line.M_InOutLine_ID = ma.M_InOutLine_ID "
                + "AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID() + ")";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)//idr.Read())
                {
                    retValue = new MInOutLine(GetCtx(), dt.Rows[0], Get_TrxName());
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

            return retValue;
        }

        /// <summary>
        ///  String info
        /// </summary>
        /// <returns>String info</returns>
        public override String ToString()
        {
            return base.ToString() + " Qty = " + this.GetQty();
        }

        public Boolean IsType(String refType)
        {
            return this.GetQtyType().Equals(refType);//refType.GetValue()
        }

        public void SetIsBulkUpdate(Boolean bulkUpdate)
        {
            isBulkUpdate = bulkUpdate;
        }

        public Boolean IsBulkUpdate()
        {
            return isBulkUpdate;
        }
    }

}
