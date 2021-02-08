/********************************************************
 * Module Name    : Framework
 * Purpose        : 
 * Class Used     : X_VAM_ProductContainer
 * Chronological Development
 * Neha Thkaur      28-08-2018  
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace VAdvantage.Model
{
    public class MProductContainer : X_VAM_ProductContainer
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductContainer).FullName);

        public MProductContainer(Ctx ctx, int VAM_ProductContainer_ID, Trx trxName)
            : base(ctx, VAM_ProductContainer_ID, trxName)
        {
        }
        public MProductContainer(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }
        /// <summary>
        /// System should allow to change the warehouse and locator if Container is having 0 qty
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (!newRecord)
            {
                // System should allow to change the warehouse and locator if Container is having 0 qty
                // when we mark container as False, then it must be blank.
                if (Is_ValueChanged("VAM_Locator_ID") || Is_ValueChanged("Ref_M_Container_ID") || (Is_ValueChanged("IsActive") && !IsActive()))
                {
                    string _sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, 
                        t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty
                             FROM VAM_Inv_Trx t " +
                                     @" INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID 
                            WHERE t.VAF_Client_ID = " + GetVAF_Client_ID()
                                   + " AND T.VAM_Locator_ID = " + Get_ValueOld("VAM_Locator_ID")//get previous selected value
                                    + @" AND NVL(t.VAM_ProductContainer_ID, 0)    = " + GetVAM_ProductContainer_ID();
                    Decimal no = Util.GetValueOfDecimal(DB.ExecuteScalar(_sql, null, Get_Trx()));
                    if (no != 0)
                    {
                        //Can't change Warehouse, Locator, Parent Container or not mark In-Active as Quantity is present in container.
                        log.SaveWarning("VIS_PCQtyNotZero", "");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get Storage Info for Container
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="VAM_Warehouse_ID">Warehouse ID</param>
        /// <param name="VAM_Locator_ID">optional locator id</param>
        /// <param name="VAM_ProductContainer_ID">Product Container</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">VAM_PFeature_SetInstance_ID instance</param>
        /// <param name="VAM_PFeature_Set_ID">attribute set</param>
        /// <param name="allAttributeInstances">if true, all attribute set instances</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="greater"></param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static X_VAM_ContainerStorage[] GetContainerStorage(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID, int VAM_ProductContainer_ID,
                                                               int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool FiFo, bool greater, Trx trxName)
        {
            return GetContainerStorage(Ctx, VAM_Warehouse_ID, VAM_Locator_ID, VAM_ProductContainer_ID,
                                        VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                                        VAM_PFeature_Set_ID, allAttributeInstances, minGuaranteeDate,
                                        FiFo, greater, trxName, true);
        }

        /// <summary>
        /// Get Storage Info for Container
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="VAM_Warehouse_ID">Warehouse ID</param>
        /// <param name="VAM_Locator_ID">optional locator id</param>
        /// <param name="VAM_ProductContainer_ID">Product Container</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">VAM_PFeature_SetInstance_ID instance</param>
        /// <param name="VAM_PFeature_Set_ID">attribute set</param>
        /// <param name="allAttributeInstances">if true, all attribute set instances</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="greater"></param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static X_VAM_ContainerStorage[] GetContainerStorage(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID, int VAM_ProductContainer_ID,
                                                               int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool FiFo, bool greater, Trx trxName, bool isContainerConsider)
        {
            if ((VAM_Warehouse_ID == 0 && VAM_Locator_ID == 0) || VAM_Product_ID == 0)
                return new X_VAM_ContainerStorage[0];

            if (VAM_PFeature_Set_ID == 0)
                allAttributeInstances = true;
            else
            {
                MAttributeSet mas = MAttributeSet.Get(Ctx, VAM_PFeature_Set_ID);
                if (!mas.IsInstanceAttribute())
                    allAttributeInstances = true;
            }

            List<X_VAM_ContainerStorage> list = new List<X_VAM_ContainerStorage>();
            //	Specific Attribute Set Instance
            String sql = "SELECT s.VAM_Locator_ID,s.VAM_ProductContainer_ID,s.VAM_Product_ID,s.VAM_PFeature_SetInstance_ID,"
                + "s.VAF_Client_ID,s.VAF_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                + "s.Qty,s.MMPolicyDate "
                + "FROM VAM_ContainerStorage s"
                + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID) ";
            if (VAM_Locator_ID > 0)
                sql += "WHERE l.VAM_Locator_ID = " + VAM_Locator_ID;
            else
                sql += "WHERE l.VAM_Warehouse_ID= " + VAM_Warehouse_ID;
            sql += " AND s.VAM_Product_ID=" + VAM_Product_ID
              + " AND COALESCE(s.VAM_PFeature_SetInstance_ID,0)= " + VAM_PFeature_SetInstance_ID;

            // consider Container
            if (isContainerConsider)
            {
                sql += "AND NVL(s.VAM_ProductContainer_ID , 0) = " + VAM_ProductContainer_ID;
            }

            if (!FiFo && minGuaranteeDate != null)
                sql += " AND MMPolicyDate " + (greater ? ">" : "<=") + GlobalVariable.TO_DATE(minGuaranteeDate, true);
            sql += " ORDER BY l.PriorityNo DESC, s.MMPolicyDate";
            if (!FiFo)
            {
                //sql += " DESC";
                sql += (greater ? " ASC" : " DESC");
            }

            //	All Attribute Set Instances
            if (allAttributeInstances)
            {
                sql = "SELECT s.VAM_Locator_ID,s.VAM_ProductContainer_ID,s.VAM_Product_ID,s.VAM_PFeature_SetInstance_ID,"
                    + "s.VAF_Client_ID,s.VAF_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                    + "s.Qty,s.MMPolicyDate "
                    + "FROM VAM_ContainerStorage s"
                    + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID)"
                    + " LEFT OUTER JOIN VAM_PFeature_SetInstance asi ON (s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) ";
                if (VAM_Locator_ID > 0)
                    sql += "WHERE l.VAM_Locator_ID = " + VAM_Locator_ID;
                else
                    sql += "WHERE l.VAM_Warehouse_ID= " + VAM_Warehouse_ID;

                // consider Container
                if (isContainerConsider)
                {
                    sql += "  AND NVL(s.VAM_ProductContainer_ID, 0) = " + VAM_ProductContainer_ID;
                }

                // product
                sql += " AND s.VAM_Product_ID=" + VAM_Product_ID;

                // consider Attribute Set Instance
                if (VAM_PFeature_SetInstance_ID > 0)
                {
                    sql += " AND COALESCE(s.VAM_PFeature_SetInstance_ID,0)= " + VAM_PFeature_SetInstance_ID;
                }

                if (!FiFo && minGuaranteeDate != null)
                    sql += " AND MMPolicyDate " + (greater ? ">" : "<=") + GlobalVariable.TO_DATE(minGuaranteeDate, true);

                if (minGuaranteeDate != null)
                {
                    // when gurantee date is null then record to be filtered based on material Policy date
                    sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>" + GlobalVariable.TO_DATE(minGuaranteeDate, true) + ")"
                        + @" ORDER BY l.PriorityNo DESC , asi.GuaranteeDate, CASE  WHEN asi.GuaranteeDate IS NULL THEN s.MMPolicyDate ELSE asi.GuaranteeDate END ";
                    if (!FiFo)
                        sql += (greater ? " ASC" : " DESC");

                    sql += " , NVL(s.VAM_PFeature_SetInstance_ID , 0)";	//	Has Prior over Locator
                    if (!FiFo)
                        sql += " DESC";

                    sql += ", s.MMPolicyDate ";
                    if (!FiFo)
                        //sql += " DESC";
                        sql += (greater ? " ASC" : " DESC");
                }
                else
                {
                    sql += "ORDER BY l.PriorityNo DESC, l.VAM_Locator_ID, s.MMPolicyDate";
                    if (!FiFo)
                        //sql += " DESC";
                        sql += (greater ? " ASC" : " DESC");
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
                    list.Add(new X_VAM_ContainerStorage(Ctx, dr, trxName));
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
            X_VAM_ContainerStorage[] retValue = new X_VAM_ContainerStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Storage Info for Container
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="VAM_Warehouse_ID">warehouse</param>
        /// <param name="VAM_Locator_ID">optional locator id</param>
        /// <param name="VAM_ProductContainer_ID"></param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">VAM_PFeature_SetInstance_ID instance</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static X_VAM_ContainerStorage[] GetContainerStorageNegative(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID, int VAM_ProductContainer_ID,
                                                               int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, DateTime? minGuaranteeDate, bool FiFo, Trx trxName)
        {
            if ((VAM_Warehouse_ID == 0 && VAM_Locator_ID == 0) || VAM_Product_ID == 0)
                return new X_VAM_ContainerStorage[0];

            List<X_VAM_ContainerStorage> list = new List<X_VAM_ContainerStorage>();
            String sql = "SELECT s.VAM_Locator_ID,s.VAM_ProductContainer_ID,s.VAM_Product_ID,s.VAM_PFeature_SetInstance_ID,"
                + "s.VAF_Client_ID,s.VAF_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                + "s.Qty,s.MMPolicyDate "
                + "FROM VAM_ContainerStorage s"
                + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID)"
                + " LEFT OUTER JOIN VAM_PFeature_SetInstance asi ON (s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) ";
            if (VAM_Locator_ID > 0)
                sql += "WHERE l.VAM_Locator_ID = " + VAM_Locator_ID;
            else
                sql += "WHERE l.VAM_Warehouse_ID= " + VAM_Warehouse_ID;
            //if (VAM_ProductContainer_ID > 0)
                sql += " AND NVL(s.VAM_ProductContainer_ID , 0) = " + VAM_ProductContainer_ID;
            sql += " AND s.VAM_Product_ID=" + VAM_Product_ID + " AND s.Qty < 0 ";
            // PT-225, PT-224 : get record specific attribte wise which is to be selected on document
            if (VAM_PFeature_SetInstance_ID > 0)
            {
                sql += "AND s.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            }
            else if (VAM_PFeature_SetInstance_ID == 0)
            {
                sql += "AND (s.VAM_PFeature_SetInstance_ID=0 OR s.VAM_PFeature_SetInstance_ID IS NULL) ";
            }
            if (minGuaranteeDate != null)
            {
                sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>" + GlobalVariable.TO_DATE(minGuaranteeDate, true) + ")";
                sql += "ORDER BY l.PriorityNo DESC, asi.GuaranteeDate, s.VAM_PFeature_SetInstance_ID";	//	Has Prior over Locator
                if (!FiFo)
                    sql += " DESC";
                //sql += ", l.PriorityNo DESC";
                sql += ", s.MMPolicyDate ";
                if (!FiFo)
                    sql += " DESC";
            }
            else
            {
                sql += "ORDER BY l.PriorityNo DESC, l.VAM_Locator_ID, s.MMPolicyDate";
                if (!FiFo)
                    sql += " DESC , s.VAM_PFeature_SetInstance_ID DESC";
                else
                    sql += ", s.VAM_PFeature_SetInstance_ID ";
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
                    list.Add(new X_VAM_ContainerStorage(Ctx, dr, trxName));
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
            X_VAM_ContainerStorage[] retValue = new X_VAM_ContainerStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Sum of total Quantity available till current date
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="VAM_Warehouse_ID">Warehouse ID</param>
        /// <param name="VAM_Locator_ID">optional locator id</param>
        /// <param name="VAM_ProductContainer_ID">Product Container</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">VAM_PFeature_SetInstance_ID instance</param>
        /// <param name="VAM_PFeature_Set_ID">attribute set</param>
        /// <param name="allAttributeInstances">if true, all attribute set instances</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances  (mostly movement date)</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="greater"></param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static Decimal GetContainerQtyFromStorage(Ctx Ctx, int VAM_Warehouse_ID, int VAM_Locator_ID, int VAM_ProductContainer_ID,
                                                               int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool greater, Trx trxName)
        {
            Decimal currentStock = 0;

            if ((VAM_Warehouse_ID == 0 && VAM_Locator_ID == 0) || VAM_Product_ID == 0)
                return currentStock;

            if (VAM_PFeature_Set_ID == 0)
                allAttributeInstances = true;
            else
            {
                MAttributeSet mas = MAttributeSet.Get(Ctx, VAM_PFeature_Set_ID);
                if (!mas.IsInstanceAttribute())
                    allAttributeInstances = true;
            }

            //	Specific Attribute Set Instance
            String sql = "SELECT SUM(s.Qty) "
                + "FROM VAM_ContainerStorage s"
                + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID) ";

            // when check for specific Locator
            if (VAM_Locator_ID > 0)
                sql += "WHERE l.VAM_Locator_ID = " + VAM_Locator_ID;
            else
                sql += "WHERE l.VAM_Warehouse_ID= " + VAM_Warehouse_ID;

            // for specific product
            sql += " AND s.VAM_Product_ID=" + VAM_Product_ID + " AND COALESCE(s.VAM_PFeature_SetInstance_ID,0)= " + VAM_PFeature_SetInstance_ID;

            // when check for specific container
            if (VAM_ProductContainer_ID > 0)
            {
                sql += "  AND NVL(s.VAM_ProductContainer_ID, 0) = " + VAM_ProductContainer_ID;
            }

            if (minGuaranteeDate != null)
                sql += " AND MMPolicyDate " + (greater ? ">" : "<=") + GlobalVariable.TO_DATE(minGuaranteeDate, true);

            //	All Attribute Set Instances
            if (allAttributeInstances)
            {
                sql = "SELECT SUM(s.Qty) "
                    + "FROM VAM_ContainerStorage s"
                    + " INNER JOIN VAM_Locator l ON (l.VAM_Locator_ID=s.VAM_Locator_ID)"
                    + " LEFT OUTER JOIN VAM_PFeature_SetInstance asi ON (s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID) ";

                // when check for specific Locator
                if (VAM_Locator_ID > 0)
                    sql += "WHERE l.VAM_Locator_ID = " + VAM_Locator_ID;
                else
                    sql += "WHERE l.VAM_Warehouse_ID= " + VAM_Warehouse_ID;

                // for specific product
                sql += " AND s.VAM_Product_ID=" + VAM_Product_ID;

                // when check for specific container
                if (VAM_ProductContainer_ID > 0)
                {
                    sql += "  AND NVL(s.VAM_ProductContainer_ID, 0) = " + VAM_ProductContainer_ID;
                }
                if (minGuaranteeDate != null)
                    sql += " AND MMPolicyDate " + (greater ? ">" : "<=") + GlobalVariable.TO_DATE(minGuaranteeDate, true);
                if (minGuaranteeDate != null)
                {
                    sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>" + GlobalVariable.TO_DATE(minGuaranteeDate, true) + ")";
                }
            }
            try
            {
                currentStock = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return currentStock;
        }

    }
}
