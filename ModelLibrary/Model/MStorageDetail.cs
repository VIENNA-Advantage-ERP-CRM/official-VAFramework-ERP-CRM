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

//using VAdvantage.Server;
using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Tcp;
using System.Data;

namespace VAdvantage.Model
{
    public class MStorageDetail : X_VAM_StorageDetail
    {
        #region Private Variables
        // Logger for class MStorageDetail 
        private static VLogger s_log = VLogger.GetVLogger(typeof(MStorageDetail).FullName);
        private const long SERIALVERSIONUID = 1L;
        private Boolean isBulkUpdate = false;

        //Warehouse 
        private int _VAM_Warehouse_ID = 0;

        #endregion

        /// <summary>
        /// Standard Constructors
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="type"></param>
        public MStorageDetail(MLocator locator, int VAM_Product_ID,
                int VAM_PFeature_SetInstance_ID, String type)
            : this(locator.GetCtx(), 0, locator.Get_TrxName())
        {
            SetClientOrg(locator);
            SetVAM_Locator_ID(locator.GetVAM_Locator_ID());
            SetVAM_Product_ID(VAM_Product_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetQtyType(type);//type.GetValue()
            SetQty(Env.ZERO);
        }

        public MStorageDetail(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
                int VAM_PFeature_SetInstance_ID, X_Ref_Quantity_Type type, Trx trx)
            : this(ctx, 0, trx)
        {
            SetVAM_Locator_ID(VAM_Locator_ID);
            SetVAM_Product_ID(VAM_Product_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetQtyType(type.GetValue());
            SetQty(Env.ZERO);
        }

        private MStorageDetail(Ctx ctx, int VAM_StorageDetail_ID, Trx trx)
            : base(ctx, VAM_StorageDetail_ID, trx)
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
            //        MCycleCountLock.LockExists(GetCtx(), GetVAM_Product_ID(), GetVAM_Locator_ID(), Get_TrxName()))
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

                MWarehouse wh = MWarehouse.Get(GetCtx(), GetVAM_Warehouse_ID());

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
                        + "FROM VAM_Storage_V s"
                        + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                        + "WHERE s.VAM_Product_ID=" + GetVAM_Product_ID()		//	#1
                        + " AND l.VAM_Warehouse_ID=" + GetVAM_Warehouse_ID()
                        + " AND l.VAM_Locator_ID=" + GetVAM_Locator_ID();
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
                        asiQtyAllocated = MStorageDetail.GetQty(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), X_Ref_Quantity_Type.ALLOCATED, Get_TrxName());
                        asiQtyDedicated = MStorageDetail.GetQty(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), X_Ref_Quantity_Type.DEDICATED, Get_TrxName());
                    }
                    else if (IsType(X_Ref_Quantity_Type.DEDICATED))
                    {
                        if (newRecord)
                            qtyDedicated = Decimal.Add(qtyDedicated, GetQty());
                        else
                        {
                            qtyDedicated = Decimal.Subtract(Decimal.Add(qtyDedicated, GetQty()), Util.GetValueOfDecimal(Get_ValueOld("Qty")));
                        }
                        asiQtyOnHand = MStorageDetail.GetQty(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), X_Ref_Quantity_Type.ON_HAND, Get_TrxName());
                        asiQtyAllocated = MStorageDetail.GetQty(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), X_Ref_Quantity_Type.ALLOCATED, Get_TrxName());
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
                        asiQtyOnHand = MStorageDetail.GetQty(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), X_Ref_Quantity_Type.ON_HAND, Get_TrxName());
                        asiQtyAllocated = GetQty();
                        asiQtyDedicated = MStorageDetail.GetQty(GetCtx(), GetVAM_Locator_ID(), GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(), X_Ref_Quantity_Type.DEDICATED, Get_TrxName());
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
        /// <param name="VAM_Locator_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="type"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static MStorageDetail GetForRead(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                String type, Trx trx)
        {
            return Get(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    type, false, trx);
        }

        public static MStorageDetail GetForUpdate(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                String type, Trx trx)
        {
            return Get(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    type, true, trx);
        }

        private static MStorageDetail Get(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                String type, Boolean forUpdate, Trx trx)
        {
            MStorageDetail retValue = null;
            String sql = "SELECT * FROM VAM_StorageDetail "
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
                s_log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                        + ", VAM_Product_ID=" + VAM_Product_ID
                        + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            else
            {
                s_log.Fine("VAM_Locator_ID=" + VAM_Locator_ID + ", VAM_Product_ID="
                        + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            }
            return retValue;
        }

        public static Decimal? GetQty(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                String type, Trx trx)
        {
            Decimal? retValue = Decimal.Zero; ;
            String sql = "SELECT Qty FROM VAM_StorageDetail "
                    + "WHERE VAM_Locator_ID=" + VAM_Locator_ID + " AND VAM_Product_ID=" + VAM_Product_ID + " AND QtyType='" + type + "' AND ";//type.GetValue()
            if (VAM_PFeature_SetInstance_ID == 0)
            {
                sql += "(VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR VAM_PFeature_SetInstance_ID IS NULL)";
            }
            else
            {
                sql += "VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
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
                s_log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                        + ", VAM_Product_ID=" + VAM_Product_ID
                        + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            else
            {
                s_log.Fine("VAM_Locator_ID=" + VAM_Locator_ID + ", VAM_Product_ID="
                        + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            }
            return retValue;
        }
        //Collection
        public static List<MStorageDetail> GetMultipleForUpdate(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                List<String> types, Trx trx)
        {
            return GetMultiple(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, types, true, trx);
        }
        public static List<MStorageDetail> GetMultipleForRead(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                List<String> types, Trx trx)
        {
            return GetMultiple(ctx, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, types, false, trx);
        }

        private static List<MStorageDetail> GetMultiple(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
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

            String sql = "SELECT * FROM VAM_StorageDetail "
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
                s_log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                        + ", VAM_Product_ID=" + VAM_Product_ID
                        + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            }
            else
            {
                s_log.Fine("VAM_Locator_ID=" + VAM_Locator_ID + ", VAM_Product_ID="
                        + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            }
            return retValue;
        }

        public static MStorageDetail GetCreate(Ctx ctx, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, String type, Trx trx)
        {
            if (VAM_Locator_ID == 0)
            {
                throw new ArgumentException("VAM_Locator_ID=0");
            }
            if (VAM_Product_ID == 0)
            {
                throw new ArgumentException("VAM_Product_ID=0");
            }
            MStorageDetail retValue = GetForUpdate(ctx, VAM_Locator_ID, VAM_Product_ID,
                    VAM_PFeature_SetInstance_ID, type, trx);
            if (retValue != null)
            {
                return retValue;
            }
            // Insert row based on locator
            MLocator locator = new MLocator(ctx, VAM_Locator_ID, trx);
            if (locator.Get_ID() != VAM_Locator_ID)
            {
                throw new ArgumentException("Not found VAM_Locator_ID="
                        + VAM_Locator_ID);
            }
            //
            retValue = new MStorageDetail(locator, VAM_Product_ID,
                    VAM_PFeature_SetInstance_ID, type);
            retValue.Save(trx);
            s_log.Fine("New " + retValue);
            return retValue;
        }

        public static Boolean Set(Ctx ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal newQty,
                String type, Trx trx)
        {
            MStorageDetail retValue = null;
            String sql = "UPDATE VAM_StorageDetail " + "SET QTY=" + newQty
                    + "WHERE VAM_Locator_ID=" + VAM_Locator_ID + " AND VAM_Product_ID=" + VAM_Product_ID + " AND QtyType='" + type + "' AND ";//type.GetValue()
            if (VAM_PFeature_SetInstance_ID == 0)
            {
                sql += "(VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR VAM_PFeature_SetInstance_ID IS NULL)";
            }
            else
            {
                sql += "VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            }

            try
            {
                int i = DB.ExecuteQuery(sql, null, trx);
                if (i != 1)
                {
                    s_log.SaveError("VAM_StorageDetailNotUpdated", sql);
                }
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }

            if (retValue == null)
                s_log.Fine("Not Found - VAM_Locator_ID=" + VAM_Locator_ID
                        + ", VAM_Product_ID=" + VAM_Product_ID
                        + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            else
                s_log.Fine("VAM_Locator_ID=" + VAM_Locator_ID + ", VAM_Product_ID="
                        + VAM_Product_ID + ", VAM_PFeature_SetInstance_ID="
                        + VAM_PFeature_SetInstance_ID);
            return true;
        }

        /// <summary>
        /// Get all Storages for Product
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_Locator_ID"></param>
        /// <param name="qtyType"></param>
        /// <param name="isFIFO"></param>
        /// <param name="withASI"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static List<MStorageDetail> GetAll(Ctx ctx, int VAM_Product_ID,
                int VAM_Locator_ID, X_Ref_Quantity_Type qtyType, Boolean isFIFO,
                Boolean withASI, Trx trx)
        {
            List<MStorageDetail> list = new List<MStorageDetail>();
            String sql = "select s.* "
                    + "from VAM_StorageDETAIL  s INNER JOIN VAM_StorageDETAIL  t "
                    + "on ( s.VAF_CLIENT_ID = t.VAF_CLIENT_ID "
                    + "and s.VAF_ORG_ID    = t.VAF_ORG_ID "
                    + "and s.VAM_Product_ID = t.VAM_Product_ID "
                    + "and s.VAM_Locator_ID = t.VAM_Locator_ID) "
                    + "where s.QTYTYPE ='" + qtyType.GetValue() + "' " + "and s.VAM_Product_ID =" + VAM_Product_ID
                    + "and s.VAM_Locator_ID =" + VAM_Locator_ID + " and t.QTYTYPE = 'H' ";
            if (withASI)
            {
                sql += "and t.QTY > 0 " + "and s.VAM_PFeature_SetInstance_ID > 0 ";
            }
            else
            {
                sql += "and t.QTY <> 0 ";
            }
            sql += " order by s.VAM_PFeature_SetInstance_ID ";

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
                Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
                int VAM_PFeature_SetInstance_ID,
                Set<X_Ref_Quantity_Type> updateQtyTypes, Trx trx) {
		Map<X_Ref_Quantity_Type, MStorageDetail> storageDetails = new HashMap<X_Ref_Quantity_Type, MStorageDetail>();
		for (X_Ref_Quantity_Type qtyType : X_Ref_Quantity_Type.values()) {
			MStorageDetail store = MStorageDetail.Get(ctx, VAM_Locator_ID,
					VAM_Product_ID, VAM_PFeature_SetInstance_ID, qtyType,
					(updateQtyTypes == null ? false : updateQtyTypes
							.contains(qtyType)), trx);
			storageDetails.put(qtyType, store);
		}
		return storageDetails;
	}*/
        public static Dictionary<String, MStorageDetail> GetAllForUpdate(Ctx ctx, int VAM_Locator_ID, int VAM_Product_ID,
                int VAM_PFeature_SetInstance_ID, List<String> updateQtyTypes, Trx trx)
        {
            Dictionary<String, MStorageDetail> storageDetails = new Dictionary<String, MStorageDetail>();
            //Array qtyType = Enum.GetValues(typeof(X_Ref_Quantity_Type.X_Ref_Quantity_TypeEnum));
            Array qtyType = X_Ref_Quantity_Type.Get().ToArray();
            for (int i = 0; i < qtyType.Length; i++)
            {
                MStorageDetail store = MStorageDetail.Get(ctx, VAM_Locator_ID,
                        VAM_Product_ID, VAM_PFeature_SetInstance_ID, qtyType.GetValue(i).ToString(),
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
        /// <param name="VAM_Warehouse_ID"></param>
        /// <param name="VAM_Locator_ID"></param>
        /// <param name="VAM_Product_ID"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="reservationAttributeSetInstance_ID"></param>
        /// <param name="diffQty"></param>
        /// <param name="type"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static Boolean Add(Ctx ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal diffQty,
                String type, Trx trx)
        {
            StringBuilder diffText = new StringBuilder("(");
            MStorageDetail storage = null;
            storage = MStorageDetail.GetCreate(ctx, VAM_Locator_ID, VAM_Product_ID,
                    VAM_PFeature_SetInstance_ID, type, trx);
            // Verify
            if (storage.GetVAM_Locator_ID() != VAM_Locator_ID
                    && storage.GetVAM_Product_ID() != VAM_Product_ID
                    && storage.GetVAM_PFeature_SetInstance_ID() != VAM_PFeature_SetInstance_ID)
            {
                s_log.Severe("No Storage found - VAM_Locator_ID=" + VAM_Locator_ID
                        + ",VAM_Product_ID=" + VAM_Product_ID + ",ASI="
                        + VAM_PFeature_SetInstance_ID);
                return false;
            }

            MStorageDetail storageASI = null;
            if ((VAM_PFeature_SetInstance_ID != reservationAttributeSetInstance_ID)
                    && (type == X_Ref_Quantity_Type.RESERVED || type == X_Ref_Quantity_Type.ORDERED))
            {
                int reservationVAM_Locator_ID = VAM_Locator_ID;
                if (reservationAttributeSetInstance_ID == 0)
                {
                    MWarehouse wh = MWarehouse.Get(ctx, VAM_Warehouse_ID);
                    reservationVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                }
                storageASI = MStorageDetail.Get(ctx, reservationVAM_Locator_ID,
                        VAM_Product_ID, reservationAttributeSetInstance_ID, type,
                        true, trx);
                if (storageASI == null) // create if not existing - should not happen
                {
                    MProduct product = MProduct.Get(ctx, VAM_Product_ID);
                    int xVAM_Locator_ID = MProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
                    if (xVAM_Locator_ID == 0)
                    {
                        MWarehouse wh = MWarehouse.Get(ctx, VAM_Warehouse_ID);
                        xVAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                    }
                    storageASI = MStorageDetail.GetCreate(ctx, xVAM_Locator_ID,
                            VAM_Product_ID, reservationAttributeSetInstance_ID, type,
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

        public static void Transfer(Ctx ctx, int VAM_Warehouse_ID, int VAM_Locator_ID,
                int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                int reservationAttributeSetInstance_ID, Decimal diffQty,
                String subtractFromType,
                String addToType, Trx trx)
        {
            Add(ctx, VAM_Warehouse_ID, VAM_Locator_ID, VAM_Product_ID,
                    VAM_PFeature_SetInstance_ID, reservationAttributeSetInstance_ID,
                    Decimal.Negate(diffQty), subtractFromType, trx);

            Add(ctx, VAM_Warehouse_ID, VAM_Locator_ID, VAM_Product_ID,
                    VAM_PFeature_SetInstance_ID, reservationAttributeSetInstance_ID,
                    Decimal.Negate(diffQty), addToType, trx);
        }

        public static Decimal GetForUpdate(Ctx ctx, int VAM_Warehouse_ID,
                int VAM_Locator_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
                int reservationAttributeSetInstance_ID, X_Ref_Quantity_Type type,
                Trx trx)
        {
            return Decimal.Zero;
        }

        /// <summary>
        /// Get VAM_Warehouse_ID of Locator
        /// </summary>
        /// <returns>warehouse</returns>
        public int GetVAM_Warehouse_ID()
        {
            if (_VAM_Warehouse_ID == 0)
            {
                MLocator loc = MLocator.Get(GetCtx(), GetVAM_Locator_ID());
                _VAM_Warehouse_ID = loc.GetVAM_Warehouse_ID();
            }
            return _VAM_Warehouse_ID;
        }

        ///**
        // * Get Storage Info for Warehouse
        // * 
        // * @param ctx
        // *            context
        // * @param VAM_Warehouse_ID
        // * @param VAM_Product_ID
        // *            product
        // * @param VAM_PFeature_SetInstance_ID
        // *            instance
        // * @param VAM_PFeature_Set_ID
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
        //public static List<Storage.VO> GetWarehouse(Ctx ctx, int VAM_Warehouse_ID,
        //        int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
        //        int VAM_PFeature_Set_ID, Boolean allAttributeInstances,
        //        Timestamp minGuaranteeDate, Boolean FiFo, Boolean allocationCheck,
        //        int M_SourceZone_ID, Trx trx)
        //{
        //    if (VAM_Warehouse_ID == 0 || VAM_Product_ID == 0)
        //        return null;

        //    if (VAM_PFeature_Set_ID == 0)
        //        allAttributeInstances = true;
        //    else
        //    {
        //        MVAMPFeatureSet mas = MVAMPFeatureSet.Get(ctx, VAM_PFeature_Set_ID);
        //        if (!mas.isInstanceAttribute())
        //            allAttributeInstances = true;
        //    }

        //    List<Storage.VO> list = new ArrayList<Storage.VO>();
        //    // Specific Attribute Set Instance
        //    String sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
        //            + "COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END),0) QtyOnhand,"
        //            + "COALESCE(SUM(CASE WHEN QtyType LIKE 'D' THEN Qty ELSE 0 END),0) QtyDedicated,"
        //            + "COALESCE(SUM(CASE WHEN QtyType LIKE 'A' THEN Qty ELSE 0 END),0) QtyAllocated "
        //            + "FROM VAM_StorageDetail s"
        //            + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID) "
        //            + "WHERE l.VAM_Warehouse_ID=?"
        //            + " AND s.VAM_Product_ID=?"
        //            + " AND COALESCE(s.VAM_PFeature_SetInstance_ID,0)=? ";

        //    if (allocationCheck)
        //        sql += "AND l.IsAvailableForAllocation='Y' ";

        //    if (M_SourceZone_ID != 0)
        //        sql += "AND l.VAM_Locator_ID IN "
        //                + " (SELECT VAM_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID = ? ) ";
        //    sql += "GROUP BY l.PriorityNo, s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID "
        //            + "ORDER BY l.PriorityNo DESC, VAM_PFeature_SetInstance_ID";

        //    if (!FiFo)
        //        sql += " DESC";
        //    // All Attribute Set Instances
        //    if (allAttributeInstances)
        //    {
        //        sql = "SELECT s.VAM_Product_ID,s.VAM_Locator_ID,s.VAM_PFeature_SetInstance_ID,"
        //                + "COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END),0) QtyOnhand,"
        //                + "COALESCE(SUM(CASE WHEN QtyType LIKE 'D' THEN Qty ELSE 0 END),0) QtyDedicated,"
        //                + "COALESCE(SUM(CASE WHEN QtyType LIKE 'A' THEN Qty ELSE 0 END),0) QtyAllocated "
        //                + "FROM VAM_StorageDetail s"
        //                + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID)"
        //                + " LEFT OUTER JOIN VAM_PFeature_SetInstance asi ON (s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) "
        //                + "WHERE l.VAM_Warehouse_ID=?" + " AND s.VAM_Product_ID=? ";

        //        if (allocationCheck)
        //            sql += "AND l.IsAvailableForAllocation='Y' ";

        //        if (M_SourceZone_ID != 0)
        //            sql += "AND l.VAM_Locator_ID IN "
        //                    + " (SELECT VAM_Locator_ID FROM M_ZoneLocator WHERE M_Zone_ID = ? ) ";

        //        if (minGuaranteeDate != null)
        //        {
        //            sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>?) "
        //                    + "GROUP BY asi.GuaranteeDate, l.PriorityNo, s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID "
        //                    + "ORDER BY asi.GuaranteeDate, l.PriorityNo DESC, VAM_PFeature_SetInstance_ID";
        //        }
        //        else
        //        {
        //            sql += "GROUP BY l.PriorityNo, s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID "
        //                    + "ORDER BY l.PriorityNo DESC, s.VAM_PFeature_SetInstance_ID";
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
        //        pstmt.setInt(index++, VAM_Warehouse_ID);
        //        pstmt.setInt(index++, VAM_Product_ID);
        //        if (M_SourceZone_ID != 0)
        //            pstmt.setInt(index++, M_SourceZone_ID);
        //        if (!allAttributeInstances)
        //            pstmt.setInt(index++, VAM_PFeature_SetInstance_ID);
        //        else if (minGuaranteeDate != null)
        //            pstmt.setTimestamp(index++, minGuaranteeDate);
        //        dr = pstmt.executeQuery();
        //        while (dr.next())
        //        {
        //            index = 1;
        //            int rs_VAM_Product_ID = dr.getInt(1);
        //            int rs_VAM_Locator_ID = dr.getInt(index++);
        //            int rs_VAM_PFeature_SetInstance_ID = dr.getInt(index++);
        //            Decimal rs_QtyOnhand = dr.getBigDecimal(index++);
        //            Decimal rs_QtyDedicated = dr.getBigDecimal(index++);
        //            Decimal rs_QtyAllocated = dr.getBigDecimal(index++);
        //            list.add(new Storage.VO(rs_VAM_Product_ID, rs_VAM_Locator_ID,
        //                    rs_VAM_PFeature_SetInstance_ID, rs_QtyOnhand,
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
        //        int VAM_Warehouse_ID, int VAM_Product_ID,
        //        int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
        //        Boolean allAttributeInstances, Timestamp minGuaranteeDate,
        //        Boolean FiFo, Trx trx)
        //{
        //    return getWarehouse(ctx, VAM_Warehouse_ID, VAM_Product_ID,
        //            VAM_PFeature_SetInstance_ID, VAM_PFeature_Set_ID,
        //            allAttributeInstances, minGuaranteeDate, FiFo, false, 0, trx);
        //}

        /// <summary>
        /// Trace back from storage record to original receipt line.
        /// </summary>
        /// <returns>MVAMInvInOutLine or null</returns>
        public MVAMInvInOutLine GetInOutLineOf()
        {
            // Don't try to trace back to receipt line for ASI=0 records
            if (GetVAM_PFeature_SetInstance_ID() == 0)
            {
                return null;
            }
            MVAMInvInOutLine retValue = null;
            String sql = "SELECT * FROM VAM_Inv_InOutLine line "
                + "WHERE VAM_PFeature_SetInstance_ID=" + GetVAM_PFeature_SetInstance_ID()
                + "OR EXISTS (SELECT 1 FROM "
                + "VAM_Inv_InOutLineMP ma WHERE line.VAM_Inv_InOutLine_ID = ma.VAM_Inv_InOutLine_ID "
                + "AND VAM_PFeature_SetInstance_ID=" + GetVAM_PFeature_SetInstance_ID() + ")";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)//idr.Read())
                {
                    retValue = new MVAMInvInOutLine(GetCtx(), dt.Rows[0], Get_TrxName());
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
