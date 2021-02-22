using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Web;

namespace VAdvantage.Model
{
    public class MVAMLocator : X_VAM_Locator
    {
        //	Logger						
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMLocator).FullName);
        //	Cache
        private static CCache<int, MVAMLocator> cache;

        Tuple<String, String, String> mInfo = null;

        /// <summary>
        /// Standard Locator Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Locator_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMLocator(Ctx ctx, int VAM_Locator_ID, Trx trxName)
            : base(ctx, VAM_Locator_ID, trxName)
        {
            if (VAM_Locator_ID == 0)
            {
                //SetVAM_Locator_ID(0);		//	PK
                //SetVAM_Warehouse_ID(0);		//	Parent
                SetIsDefault(false);
                SetPriorityNo(50);
                //SetValue(null);
                //SetX(null);
                //SetY(null);
                //SetZ(null);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MVAMLocator(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        public MVAMLocator(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /// <summary>
        /// New Locator Constructor with XYZ=000
        /// </summary>
        /// <param name="warehouse">parent</param>
        /// <param name="value">value</param>
        public MVAMLocator(MWarehouse warehouse, String value)
            : this(warehouse.GetCtx(), 0, warehouse.Get_TrxName())
        {
            SetClientOrg(warehouse);
            SetVAM_Warehouse_ID(warehouse.GetVAM_Warehouse_ID());		//	Parent
            SetValue(value);
            // Added by Mohit VAWMS 20-8-2015
            if (Env.HasModulePrefix("VAWMS_", out mInfo))
            {
                SetXYZ("0", "0", "0", "0", "0");
            }
            else
            {
                SetXYZ("0", "0", "0");
            }
            //End
        }

        /// <summary>
        /// Set Location
        /// </summary>
        /// <param name="X">x</param>
        /// <param name="Y">y</param>
        /// <param name="Z">z</param>
        public void SetXYZ(String X, String Y, String Z)
        {
            SetX(X);
            SetY(Y);
            SetZ(Z);
        }

        /// <summary>
        /// Get Warehouse Name
        /// </summary>
        /// <returns>name</returns>
        public String GetWarehouseName()
        {
            MWarehouse wh = MWarehouse.Get(GetCtx(), GetVAM_Warehouse_ID());
            if (wh.Get_ID() == 0)
                return "<" + GetVAM_Warehouse_ID() + ">";
            return wh.GetName();
        }

        /// <summary>
        /// Get Locator from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Locator_ID">id</param>
        /// <returns>MVAMLocator</returns>
        public static MVAMLocator Get(Ctx ctx, int VAM_Locator_ID)
        {
            if (cache == null)
                cache = new CCache<int, MVAMLocator>("VAM_Locator", 20);
            int key = VAM_Locator_ID;
            MVAMLocator retValue = null;
            if (cache.ContainsKey(key))
            {
                retValue = (MVAMLocator)cache[key];
            }
            if (retValue != null)
                return retValue;
            retValue = new MVAMLocator(ctx, VAM_Locator_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get the Locator with the combination or create new one
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Warehouse_ID">id</param>
        /// <param name="value">value</param>
        /// <param name="X">x</param>
        /// <param name="Y">y</param>
        /// <param name="Z">z</param>
        /// <returns>locator</returns>
        public static MVAMLocator Get(Ctx ctx, int VAM_Warehouse_ID, String value,
            String X, String Y, String Z)
        {
            MVAMLocator retValue = null;
            String sql = "SELECT * FROM VAM_Locator WHERE VAM_Warehouse_ID=" + VAM_Warehouse_ID + " AND " +
                "X='" + X + "' AND Y='" + Y + "' AND Z='" + Z + "'";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow rs = ds.Tables[0].Rows[0];
                    retValue = new MVAMLocator(ctx, rs, null);
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, "get", ex);
            }

            //
            if (retValue == null)
            {

                MWarehouse wh = MWarehouse.Get(ctx, VAM_Warehouse_ID);
                retValue = new MVAMLocator(wh, HttpUtility.HtmlEncode(value));
                retValue.SetXYZ(HttpUtility.HtmlEncode(X), HttpUtility.HtmlEncode(Y), HttpUtility.HtmlEncode(Z));
                if (!retValue.Save())
                    retValue = null;
            }
            return retValue;
        }

        /// <summary>
        /// Get oldest Default Locator of warehouse with locator
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Locator_ID">locator</param>
        /// <returns>locator or null</returns>
        public static MVAMLocator GetDefault(Ctx ctx, int VAM_Locator_ID)
        {
            Trx trxName = null;
            MVAMLocator retValue = null;
            String sql = "SELECT * FROM VAM_Locator l "
                + "WHERE IsDefault='Y'"
                + " AND EXISTS (SELECT * FROM VAM_Locator lx "
                    + "WHERE l.VAM_Warehouse_ID=lx.VAM_Warehouse_ID AND lx.VAM_Locator_ID=" + VAM_Locator_ID + ") "
                + "ORDER BY Created";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    DataRow rs = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        rs = ds.Tables[0].Rows[i];
                        retValue = new MVAMLocator(ctx, rs, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        public override String ToString()
        {
            return GetValue();
        }
        public static MVAMLocator GetDefaultLocatorOfOrg(Ctx ctx, int VAF_Org_ID)
        {
            MVAMLocator retValue = null;
            List<int> defaultlocators = new List<int>();
            List<int> locators = new List<int>();
            String sql = "SELECT VAM_Locator_ID, IsDefault FROM VAM_Locator WHERE (VAF_Org_ID=" + VAF_Org_ID + " OR 0=" + VAF_Org_ID + ")";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql);
                while (idr != null && idr.Read())
                {
                    if (Util.GetValueOfString(idr[1]) == "Y")
                    {
                        defaultlocators.Add(Util.GetValueOfInt(idr[0]));
                        break;
                    }
                    else
                    {
                        locators.Add(Util.GetValueOfInt(idr[0]));
                    }
                }
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                if (defaultlocators.Count > 0)
                {
                    retValue = MVAMLocator.Get(ctx, Util.GetValueOfInt(defaultlocators[0]));
                    return retValue;
                }
                if (locators.Count > 0)
                {
                    retValue = MVAMLocator.Get(ctx, Util.GetValueOfInt(locators[0]));
                    return retValue;
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
            return retValue;
        }

        // Added by Mohit VAWMS 20-8-2015
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (Env.HasModulePrefix("VAWMS_", out mInfo))
            {
                //  Check Storage
                if (Is_ValueChanged("IsActive") && IsActive())  // now not active 
                {
                    if (checkStock(GetCtx(), Get_ID(), Get_TrxName()))
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "LocatorHasStock"));
                        return false;
                    }

                } // storage

                // JID:1888 Checks for the duplicate Searchkey

                int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(Value) FROM VAM_Locator WHERE Value= '" + GetValue() + "' AND VAM_Locator_ID !=" + GetVAM_Locator_ID()));
                if (count > 0)
                {
                    log.SaveError("", Msg.GetMsg(GetCtx(), "SearchKeyUnique"));
                    return false;
                }

                if (newRecord
                        || Is_ValueChanged("X")
                        || Is_ValueChanged("Y")
                        || Is_ValueChanged("Z")
                        || Is_ValueChanged("POSITION")
                        || Is_ValueChanged("Bin"))
                {
                    MWarehouse wh = new MWarehouse(GetCtx(), GetVAM_Warehouse_ID(), Get_TrxName());

                    //if (GetBin() != null && GetPOSITION() != null || GetPOSITION()==null)
                    //{
                    //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "PositionMandatorySegment"));
                    //    return false;
                    //}



                    //if (GetX() == null || GetX().Length == 0 || !Util.isAlphaNumeric(GetX()) ||
                    //    GetY() == null || GetY().Length == 0 || !Util.isAlphaNumeric(GetY()) ||
                    //    GetZ() == null || GetZ().Length == 0 || !Util.isAlphaNumeric(GetZ()) ||
                    //    (GetBin() != null && GetBin().Length != 0 && !Util.isAlphaNumeric(GetBin())) ||
                    //    (GetPOSITION() != null && GetPOSITION().Length != 0 && !Util.isAlphaNumeric(GetPOSITION())))
                    //if (GetX() == null || GetX().Length == 0 ||
                    //   GetY() == null || GetY().Length == 0 ||
                    //   GetZ() == null || GetZ().Length == 0 ||
                    //   (GetBin() != null && GetBin().Length != 0) ||
                    //   (GetPOSITION() != null && GetPOSITION().Length != 0))
                    //{
                    //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "SegmentNotAlphaNumeric"));
                    //    return false;
                    //}

                    //String combination = GetX().concat(wh.GetSeparator());
                    //combination = combination.(GetY()).concat(wh.GetSeparator());
                    //combination = combination.concat(GetZ());

                    //if (GetPOSITION() != null && GetPOSITION().Length != 0)
                    //    combination = combination.concat(wh.GetSeparator()).concat(GetPOSITION());

                    //if (GetBin() != null && GetBin().Length != 0)
                    //    combination = combination.concat(wh.GetSeparator()).concat(GetBin());
                    StringBuilder combination = new StringBuilder();
                    combination.Append(GetX()).Append(wh.GetSeparator());
                    combination = combination.Append(GetY()).Append(wh.GetSeparator());
                    combination = combination.Append(GetZ());
                    if (GetPOSITION() != null && GetPOSITION().Length != 0)
                    {
                        combination = combination.Append(wh.GetSeparator()).Append(GetPOSITION());
                    }
                    if (GetBin() != null && GetBin().Length != 0)
                    {
                        combination = combination.Append(wh.GetSeparator()).Append(GetBin());
                    }
                    log.Fine("Set Locator Combination :" + combination);

                    //String sql = "SELECT count(*) FROM VAM_Locator WHERE VAM_Locator_ID<>?" +
                    //                " AND VAM_Warehouse_ID = ?" +
                    //                " AND UPPER(LocatorCombination) = UPPER(?)";
                    String sql = "SELECT count(*) FROM VAM_Locator WHERE VAM_Locator_ID<>" + GetVAM_Locator_ID() +
                                  " AND VAM_Warehouse_ID =" + GetVAM_Warehouse_ID() +
                                  " AND UPPER(LocatorCombination) = UPPER('" + combination + "')";
                    //int ii = QueryUtil.GetSQLValue(Get_TrxName(), sql, GetVAM_Locator_ID(), GetVAM_Warehouse_ID(), combination);
                    int ii = Convert.ToInt32(DB.ExecuteScalar(sql, null, Get_TrxName()));
                    if (ii != 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "CombinationNotUnique"));
                        return false;
                    }

                    SetLocatorCombination(combination.ToString());
                }

                if (newRecord
                        || Is_ValueChanged("IsAvailableToPromise")
                        || Is_ValueChanged("IsAvailableForAllocation"))
                {
                    if (IsAvailableForAllocation() && !IsAvailableToPromise())
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "InvalidCombination"));
                        return false;
                    }
                }
                return true;
            }
            return true;
        }
        public static Boolean checkStock(Ctx ctx, int VAM_Locator_ID, Trx trx)
        {
            //ArrayList<MVAMStorage> list = new ArrayList<MVAMStorage>();
            String sql = "SELECT COALESCE(SUM(CASE WHEN QtyType LIKE 'H' THEN Qty ELSE 0 END), 0) QtyOnHand, " +
                                "COALESCE(SUM(CASE WHEN QtyType LIKE 'O' THEN Qty ELSE 0 END), 0) QtyOrdered, " +
                                "COALESCE(SUM(CASE WHEN QtyType LIKE 'R' THEN Qty ELSE 0 END), 0) QtyReserved " +
                    "FROM VAM_StorageDetail WHERE VAM_Locator_ID=" + VAM_Locator_ID;
            Decimal OnHand = Env.ZERO;
            Decimal Ordered = Env.ZERO;
            Decimal Reserved = Env.ZERO;

            //PreparedStatement pstmt = null;
            //ResultSet rs = null;
            DataSet dstmt = new DataSet();
            try
            {

                //pstmt = DB.prepareStatement(sql, trx);
                //pstmt.setInt(1, VAM_Locator_ID);
                //rs = pstmt.executeQuery();
                //while (rs.next())
                //{
                //    OnHand = rs.getBigDecimal("QtyOnHand");
                //    Ordered = rs.getBigDecimal("QtyOrdered");
                //    Reserved = rs.getBigDecimal("QtyReserved");
                //}
                dstmt = DB.ExecuteDataset(sql, null);
                if (dstmt != null && dstmt.Tables.Count > 0 && dstmt.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dstmt.Tables[0].Rows.Count; i++)
                    {
                        OnHand = Convert.ToDecimal(dstmt.Tables[0].Rows[0]["QtyOnHand"]);
                        Ordered = Convert.ToDecimal(dstmt.Tables[0].Rows[0]["QtyOrdered"]);
                        Reserved = Convert.ToDecimal(dstmt.Tables[0].Rows[0]["QtyReserved"]);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (dstmt != null)
                    dstmt.Dispose();
            }

            //if ((OnHand.signum() != 0) || (Ordered.signum() != 0) || (Reserved.signum() != 0))
            if ((OnHand != 0) || (Ordered != 0) || (Reserved != 0))
                return true;
            else
                return false;
        }
        public void SetXYZ(String X, String Y, String Z, string Position, String Bin)
        {
            SetX(X);
            SetY(Y);
            SetZ(Z);
            SetBin(Bin);
            SetPOSITION(Position);
        }
        public static MVAMLocator Get(Ctx ctx, int VAM_Warehouse_ID, String value, String X, String Y, String Z, String Position, String Bin)
        {
            MVAMLocator retValue = null;
            String sql = "SELECT * FROM VAM_Locator WHERE VAM_Warehouse_ID=" + VAM_Warehouse_ID + " AND " +
                "X='" + X + "' AND Y='" + Y + "' AND Z='" + Z + "'";
            DataSet ds = null;
            try
            {
                ds = DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow idr = ds.Tables[0].Rows[0];
                    retValue = new MVAMLocator(ctx, idr, null);
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, "get", ex);
            }

            //
            if (retValue == null)
            {
                MWarehouse wh = MWarehouse.Get(ctx, VAM_Warehouse_ID);
                retValue = new MVAMLocator(wh, value);
                retValue.SetXYZ(X, Y, Z, Position, Bin);
                if (!retValue.Save())
                    retValue = null;
            }
            return retValue;
        }
        public Boolean IsFixed()
        {
            String sql = "SELECT count(*) FROM VAM_ProductLocator pl " +
                            "WHERE pl.VAM_Locator_ID = @param1 ";

            int ii = DB.GetSQLValue(Get_TrxName(), sql, GetVAM_Locator_ID());
            if (ii != 0)
            {
                return true;
            }
            return false;
        }
        //END
    }
}
