/********************************************************
 * Module Name    : Framework
 * Purpose        : 
 * Class Used     : X_M_ProductContainer
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
    public class MProductContainer : X_M_ProductContainer
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductContainer).FullName);

        public MProductContainer(Ctx ctx, int M_ProductContainer_ID, Trx trxName)
            : base(ctx, M_ProductContainer_ID, trxName)
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
                if (Is_ValueChanged("M_Locator_ID") || Is_ValueChanged("Ref_M_Container_ID") || (Is_ValueChanged("IsActive") && !IsActive()))
                {
                    string _sql = @"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, 
                        t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty
                             FROM m_transaction t " +
                                     @" INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID 
                            WHERE t.AD_Client_ID = " + GetAD_Client_ID()
                                   + " AND T.M_LOCATOR_ID = " + Get_ValueOld("M_Locator_ID")//get previous selected value
                                    + @" AND NVL(t.M_ProductContainer_ID, 0)    = " + GetM_ProductContainer_ID();
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
        /// <param name="M_Warehouse_ID">Warehouse ID</param>
        /// <param name="M_Locator_ID">optional locator id</param>
        /// <param name="M_ProductContainer_ID">Product Container</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">M_AttributeSetInstance_ID instance</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <param name="allAttributeInstances">if true, all attribute set instances</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="greater"></param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static X_M_ContainerStorage[] GetContainerStorage(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID, int M_ProductContainer_ID,
                                                               int M_Product_ID, int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool FiFo, bool greater, Trx trxName)
        {
            return GetContainerStorage(Ctx, M_Warehouse_ID, M_Locator_ID, M_ProductContainer_ID,
                                        M_Product_ID, M_AttributeSetInstance_ID,
                                        M_AttributeSet_ID, allAttributeInstances, minGuaranteeDate,
                                        FiFo, greater, trxName, true);
        }

        /// <summary>
        /// Get Storage Info for Container
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="M_Warehouse_ID">Warehouse ID</param>
        /// <param name="M_Locator_ID">optional locator id</param>
        /// <param name="M_ProductContainer_ID">Product Container</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">M_AttributeSetInstance_ID instance</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <param name="allAttributeInstances">if true, all attribute set instances</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="greater"></param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static X_M_ContainerStorage[] GetContainerStorage(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID, int M_ProductContainer_ID,
                                                               int M_Product_ID, int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool FiFo, bool greater, Trx trxName, bool isContainerConsider)
        {
            if ((M_Warehouse_ID == 0 && M_Locator_ID == 0) || M_Product_ID == 0)
                return new X_M_ContainerStorage[0];

            if (M_AttributeSet_ID == 0)
                allAttributeInstances = true;
            else
            {
                MAttributeSet mas = MAttributeSet.Get(Ctx, M_AttributeSet_ID);
                if (!mas.IsInstanceAttribute())
                    allAttributeInstances = true;
            }

            List<X_M_ContainerStorage> list = new List<X_M_ContainerStorage>();
            //	Specific Attribute Set Instance
            String sql = "SELECT s.M_Locator_ID,s.M_ProductContainer_ID,s.M_Product_ID,s.M_AttributeSetInstance_ID,"
                + "s.AD_Client_ID,s.AD_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                + "s.Qty,s.MMPolicyDate "
                + "FROM M_ContainerStorage s"
                + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) ";
            if (M_Locator_ID > 0)
                sql += "WHERE l.M_Locator_ID = " + M_Locator_ID;
            else
                sql += "WHERE l.M_Warehouse_ID= " + M_Warehouse_ID;
            sql += " AND s.M_Product_ID=" + M_Product_ID
              + " AND COALESCE(s.M_AttributeSetInstance_ID,0)= " + M_AttributeSetInstance_ID;

            // consider Container
            if (isContainerConsider)
            {
                sql += "AND NVL(s.M_ProductContainer_ID , 0) = " + M_ProductContainer_ID;
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
                sql = "SELECT s.M_Locator_ID,s.M_ProductContainer_ID,s.M_Product_ID,s.M_AttributeSetInstance_ID,"
                    + "s.AD_Client_ID,s.AD_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                    + "s.Qty,s.MMPolicyDate "
                    + "FROM M_ContainerStorage s"
                    + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID)"
                    + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID) ";
                if (M_Locator_ID > 0)
                    sql += "WHERE l.M_Locator_ID = " + M_Locator_ID;
                else
                    sql += "WHERE l.M_Warehouse_ID= " + M_Warehouse_ID;

                // consider Container
                if (isContainerConsider)
                {
                    sql += "  AND NVL(s.M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
                }

                // product
                sql += " AND s.M_Product_ID=" + M_Product_ID;

                // consider Attribute Set Instance
                if (M_AttributeSetInstance_ID > 0)
                {
                    sql += " AND COALESCE(s.M_AttributeSetInstance_ID,0)= " + M_AttributeSetInstance_ID;
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

                    sql += " , NVL(s.M_AttributeSetInstance_ID , 0)";	//	Has Prior over Locator
                    if (!FiFo)
                        sql += " DESC";

                    sql += ", s.MMPolicyDate ";
                    if (!FiFo)
                        //sql += " DESC";
                        sql += (greater ? " ASC" : " DESC");
                }
                else
                {
                    sql += "ORDER BY l.PriorityNo DESC, l.M_Locator_ID, s.MMPolicyDate";
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
                    list.Add(new X_M_ContainerStorage(Ctx, dr, trxName));
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
            X_M_ContainerStorage[] retValue = new X_M_ContainerStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Storage Info for Container
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="M_Warehouse_ID">warehouse</param>
        /// <param name="M_Locator_ID">optional locator id</param>
        /// <param name="M_ProductContainer_ID"></param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">M_AttributeSetInstance_ID instance</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static X_M_ContainerStorage[] GetContainerStorageNegative(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID, int M_ProductContainer_ID,
                                                               int M_Product_ID, int M_AttributeSetInstance_ID, DateTime? minGuaranteeDate, bool FiFo, Trx trxName)
        {
            if ((M_Warehouse_ID == 0 && M_Locator_ID == 0) || M_Product_ID == 0)
                return new X_M_ContainerStorage[0];

            List<X_M_ContainerStorage> list = new List<X_M_ContainerStorage>();
            String sql = "SELECT s.M_Locator_ID,s.M_ProductContainer_ID,s.M_Product_ID,s.M_AttributeSetInstance_ID,"
                + "s.AD_Client_ID,s.AD_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                + "s.Qty,s.MMPolicyDate "
                + "FROM M_ContainerStorage s"
                + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID)"
                + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID) ";
            if (M_Locator_ID > 0)
                sql += "WHERE l.M_Locator_ID = " + M_Locator_ID;
            else
                sql += "WHERE l.M_Warehouse_ID= " + M_Warehouse_ID;
            //if (M_ProductContainer_ID > 0)
                sql += " AND NVL(s.M_ProductContainer_ID , 0) = " + M_ProductContainer_ID;
            sql += " AND s.M_Product_ID=" + M_Product_ID + " AND s.Qty < 0 ";
            // PT-225, PT-224 : get record specific attribte wise which is to be selected on document
            if (M_AttributeSetInstance_ID > 0)
            {
                sql += "AND s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            }
            else if (M_AttributeSetInstance_ID == 0)
            {
                sql += "AND (s.M_AttributeSetInstance_ID=0 OR s.M_AttributeSetInstance_ID IS NULL) ";
            }
            if (minGuaranteeDate != null)
            {
                sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>" + GlobalVariable.TO_DATE(minGuaranteeDate, true) + ")";
                sql += "ORDER BY l.PriorityNo DESC, asi.GuaranteeDate, s.M_AttributeSetInstance_ID";	//	Has Prior over Locator
                if (!FiFo)
                    sql += " DESC";
                //sql += ", l.PriorityNo DESC";
                sql += ", s.MMPolicyDate ";
                if (!FiFo)
                    sql += " DESC";
            }
            else
            {
                sql += "ORDER BY l.PriorityNo DESC, l.M_Locator_ID, s.MMPolicyDate";
                if (!FiFo)
                    sql += " DESC , s.M_AttributeSetInstance_ID DESC";
                else
                    sql += ", s.M_AttributeSetInstance_ID ";
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
                    list.Add(new X_M_ContainerStorage(Ctx, dr, trxName));
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
            X_M_ContainerStorage[] retValue = new X_M_ContainerStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Sum of total Quantity available till current date
        /// </summary>
        /// <param name="Ctx">context</param>
        /// <param name="M_Warehouse_ID">Warehouse ID</param>
        /// <param name="M_Locator_ID">optional locator id</param>
        /// <param name="M_ProductContainer_ID">Product Container</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">M_AttributeSetInstance_ID instance</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <param name="allAttributeInstances">if true, all attribute set instances</param>
        /// <param name="minGuaranteeDate">optional minimum guarantee date if all attribute instances  (mostly movement date)</param>
        /// <param name="FiFo">first in-first-out</param>
        /// <param name="greater"></param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing - ordered by location priority (desc) and/or guarantee date</returns>
        public static Decimal GetContainerQtyFromStorage(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID, int M_ProductContainer_ID,
                                                               int M_Product_ID, int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool greater, Trx trxName)
        {
            Decimal currentStock = 0;

            if ((M_Warehouse_ID == 0 && M_Locator_ID == 0) || M_Product_ID == 0)
                return currentStock;

            if (M_AttributeSet_ID == 0)
                allAttributeInstances = true;
            else
            {
                MAttributeSet mas = MAttributeSet.Get(Ctx, M_AttributeSet_ID);
                if (!mas.IsInstanceAttribute())
                    allAttributeInstances = true;
            }

            //	Specific Attribute Set Instance
            String sql = "SELECT SUM(s.Qty) "
                + "FROM M_ContainerStorage s"
                + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) ";

            // when check for specific Locator
            if (M_Locator_ID > 0)
                sql += "WHERE l.M_Locator_ID = " + M_Locator_ID;
            else
                sql += "WHERE l.M_Warehouse_ID= " + M_Warehouse_ID;

            // for specific product
            sql += " AND s.M_Product_ID=" + M_Product_ID + " AND COALESCE(s.M_AttributeSetInstance_ID,0)= " + M_AttributeSetInstance_ID;

            // when check for specific container
            if (M_ProductContainer_ID > 0)
            {
                sql += "  AND NVL(s.M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
            }

            if (minGuaranteeDate != null)
                sql += " AND MMPolicyDate " + (greater ? ">" : "<=") + GlobalVariable.TO_DATE(minGuaranteeDate, true);

            //	All Attribute Set Instances
            if (allAttributeInstances)
            {
                sql = "SELECT SUM(s.Qty) "
                    + "FROM M_ContainerStorage s"
                    + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID)"
                    + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID) ";

                // when check for specific Locator
                if (M_Locator_ID > 0)
                    sql += "WHERE l.M_Locator_ID = " + M_Locator_ID;
                else
                    sql += "WHERE l.M_Warehouse_ID= " + M_Warehouse_ID;

                // for specific product
                sql += " AND s.M_Product_ID=" + M_Product_ID;

                // when check for specific container
                if (M_ProductContainer_ID > 0)
                {
                    sql += "  AND NVL(s.M_ProductContainer_ID, 0) = " + M_ProductContainer_ID;
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
