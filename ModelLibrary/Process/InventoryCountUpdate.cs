/********************************************************
 * Module  Name   : 
 * Purpose        : Update existing Inventory Count List with current Book value
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Veena        21-Oct-2009
  ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.Data;
using System.Data.SqlClient;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    /// <summary>
    /// Update existing Inventory Count List with current Book value
    /// </summary>
    public class InventoryCountUpdate : ProcessEngine.SvrProcess
    {
        /** Physical Inventory		*/
        private int _m_Inventory_ID = 0;
        /** Update to What			*/
        private Boolean _inventoryCountSetZero = false;
        private Boolean _AdjustinventoryCount = false;
        private Boolean _SkipBL = false;
        /** Physical Inventory					*/
        private MInventory inventory = null;

        private bool isContainerApplicable = false;
        /// <summary>
        /// Prepare - e.g., Get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                    ;
                else if (name.Equals("InventoryCountSet"))
                {
                    _inventoryCountSetZero = "Z".Equals(para[i].GetParameter());
                    _AdjustinventoryCount = "A".Equals(para[i].GetParameter());
                }
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _m_Inventory_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            log.Info("M_Inventory_ID=" + _m_Inventory_ID);
            inventory = new MInventory(GetCtx(), _m_Inventory_ID, Get_TrxName());
            if (inventory.Get_ID() == 0)
                throw new SystemException("Not found: M_Inventory_ID=" + _m_Inventory_ID);

            //	Multiple Lines for one item
            //jz simple the SQL so that Derby also like it. To avoid testing Oracle by now, leave no change for Oracle
            String sql = null;
            if (DataBase.DB.IsOracle())
            {
                sql = "UPDATE M_InventoryLine SET IsActive='N' "
                    + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                    + " AND (M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID) IN "
                        + "(SELECT M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID "
                        + "FROM M_InventoryLine "
                        + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                        + " GROUP BY M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID "
                        + (isContainerApplicable ? " , M_ProductContainer_ID" : "")
                        + "HAVING COUNT(*) > 1)";
            }
            else
            {
                sql = "UPDATE M_InventoryLine SET IsActive='N' "
                    + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                    + " AND EXISTS "
                        + "(SELECT COUNT(*) "
                        + "FROM M_InventoryLine "
                        + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                        + " GROUP BY M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID "
                        + (isContainerApplicable ? " , M_ProductContainer_ID" : "")
                        + "HAVING COUNT(*) > 1)";
            }
            int multiple = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Info("Multiple=" + multiple);

            int delMA = MInventoryLineMA.DeleteInventoryMA(_m_Inventory_ID, Get_TrxName());
            log.Info("DeletedMA=" + delMA);

            //	ASI
            sql = "UPDATE M_InventoryLine l "
                + "SET (QtyBook,QtyCount) = "
                    + "(SELECT QtyOnHand,QtyOnHand FROM M_Storage s "
                    + "WHERE s.M_Product_ID=l.M_Product_ID AND s.M_Locator_ID=l.M_Locator_ID"
                    + " AND s.M_AttributeSetInstance_ID=l.M_AttributeSetInstance_ID),"
                + " Updated=SysDate,"
                + " UpdatedBy=" + GetAD_User_ID()
                //
                + " WHERE M_Inventory_ID=" + _m_Inventory_ID
                + " AND EXISTS (SELECT * FROM M_Storage s "
                    + "WHERE s.M_Product_ID=l.M_Product_ID AND s.M_Locator_ID=l.M_Locator_ID"
                    + " AND s.M_AttributeSetInstance_ID=l.M_AttributeSetInstance_ID)";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Info("Update with ASI =" + no);

            //	No ASI
            //int noMA = UpdateWithMA();

            //	Set Count to Zero
            if (_inventoryCountSetZero)
            {
                sql = "UPDATE M_InventoryLine l "
                    + "SET QtyCount=0 "
                    + "WHERE M_Inventory_ID=" + _m_Inventory_ID;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                log.Info("Set Count to Zero =" + no);
            }
            if (_AdjustinventoryCount)
            {
                //                MInventoryLine[] lines = inventory.GetLines(true);
                //                for (int i = 0; i < lines.Length; i++)
                //                {
                //                    decimal currentQty = 0;
                //                    string query = "", qry = "";
                //                    int result = 0;
                //                    MInventoryLine iLine = lines[i];
                //                    int M_Product_ID = Utility.Util.GetValueOfInt(iLine.GetM_Product_ID());
                //                    int M_Locator_ID = Utility.Util.GetValueOfInt(iLine.GetM_Locator_ID());
                //                    int M_AttributeSetInstance_ID = Util.GetValueOfInt(iLine.GetM_AttributeSetInstance_ID());

                //                    query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                           AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                //                    result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                //                    if (result > 0)
                //                    {
                //                        qry = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                //                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                //                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                //                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                //                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                //                        currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry));
                //                    }
                //                    else
                //                    {
                //                        query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                //                        result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                //                        if (result > 0)
                //                        {
                //                            qry = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                //                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                //                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                //                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                //                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                //                            currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry));
                //                        }
                //                    }
                //                    iLine.SetQtyBook(currentQty);
                //                    iLine.SetOpeningStock(currentQty);
                //                    if (iLine.GetAdjustmentType() == "A")
                //                    {
                //                        iLine.SetDifferenceQty(Util.GetValueOfDecimal(iLine.GetOpeningStock()) - Util.GetValueOfDecimal(iLine.GetAsOnDateCount()));
                //                    }
                //                    else if (iLine.GetAdjustmentType() == "D")
                //                    {
                //                        iLine.SetAsOnDateCount(Util.GetValueOfDecimal(iLine.GetOpeningStock()) - Util.GetValueOfDecimal(iLine.GetDifferenceQty()));
                //                    }
                //                    iLine.SetQtyCount(Util.GetValueOfDecimal(iLine.GetQtyBook()) - Util.GetValueOfDecimal(iLine.GetDifferenceQty()));
                //                    if (!iLine.Save())
                //                    {

                //                    }
                //                }

                // Work done by Bharat on 26/12/2016 for optimization
                if (isContainerApplicable)
                {
                    sql = @"SELECT m.M_InventoryLine_ID, m.M_Locator_ID, m.M_Product_ID, m.M_AttributeSetInstance_ID, m.AdjustmentType, m.AsOnDateCount, m.DifferenceQty,
                nvl(mt.CurrentQty, 0) as CurrentQty FROM M_InventoryLine m LEFT JOIN (SELECT DISTINCT t.M_Locator_ID, t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_ProductContainer_ID,
                FIRST_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID, NVL(t.M_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM M_Transaction t
                INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) +
                    " AND t.AD_Client_ID = " + inventory.GetAD_Client_ID() + @") mt ON m.M_Product_ID = mt.M_Product_ID AND nvl(m.M_AttributeSetInstance_ID, 0) = nvl(mt.M_AttributeSetInstance_ID, 0) 
                AND m.M_Locator_ID = mt.M_Locator_ID AND nvl(m.M_ProductContainer_ID, 0) = nvl(mt.M_ProductContainer_ID, 0) 
                WHERE m.M_Inventory_ID = " + _m_Inventory_ID + " ORDER BY m.Line";
                }
                else
                {
                    sql = @"SELECT m.M_InventoryLine_ID, m.M_Locator_ID, m.M_Product_ID, m.M_AttributeSetInstance_ID, m.AdjustmentType, m.AsOnDateCount, m.DifferenceQty,
                nvl(mt.CurrentQty, 0) as CurrentQty FROM M_InventoryLine m LEFT JOIN (SELECT DISTINCT t.M_Locator_ID, t.M_Product_ID, t.M_AttributeSetInstance_ID, 
                FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_Locator_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM M_Transaction t
                INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) +
                    " AND t.AD_Client_ID = " + inventory.GetAD_Client_ID() + @") mt ON m.M_Product_ID = mt.M_Product_ID AND nvl(m.M_AttributeSetInstance_ID, 0) = nvl(mt.M_AttributeSetInstance_ID, 0) 
                AND m.M_Locator_ID = mt.M_Locator_ID WHERE m.M_Inventory_ID = " + _m_Inventory_ID + " ORDER BY m.Line";
                }

                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_InventoryLine_ID) FROM ( " + sql + " ) t", null, null));
                int pageSize = 500;
                int TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
                int count = 0;
                DataSet ds = null;
                StringBuilder updateSql = new StringBuilder();
                try
                {
                    if (totalRec > 0)
                    {
                        log.Info(" =====> Physical Inventory update process Started at " + DateTime.Now.ToString());
                        if (!_SkipBL)
                        {
                            for (int pageNo = 1; pageNo <= TotalPage; pageNo++)
                            {
                                ds = DB.GetDatabase().ExecuteDatasetPaging(sql, pageNo, pageSize, 0);
                                if (ds != null && ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                                    {
                                        decimal currentQty = 0;
                                        int line_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                                        currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][7]);
                                        MInventoryLine iLine = new MInventoryLine(GetCtx(), line_ID, Get_TrxName());
                                        iLine.SetParent(inventory);
                                        iLine.SetQtyBook(currentQty);
                                        iLine.SetOpeningStock(currentQty);
                                        if (iLine.GetAdjustmentType() == "A")
                                        {
                                            iLine.SetDifferenceQty(Util.GetValueOfDecimal(iLine.GetOpeningStock()) - Util.GetValueOfDecimal(iLine.GetAsOnDateCount()));
                                        }
                                        else if (iLine.GetAdjustmentType() == "D")
                                        {
                                            iLine.SetAsOnDateCount(Util.GetValueOfDecimal(iLine.GetOpeningStock()) - Util.GetValueOfDecimal(iLine.GetDifferenceQty()));
                                        }
                                        iLine.SetQtyCount(Util.GetValueOfDecimal(iLine.GetQtyBook()) - Util.GetValueOfDecimal(iLine.GetDifferenceQty()));
                                        if (!iLine.Save())
                                        {

                                        }
                                        else
                                        {
                                            count++;
                                        }
                                    }
                                    ds.Dispose();
                                    log.Info(" =====>  records updated at " + DateTime.Now.ToString() + " are = " + count + " <===== ");
                                }
                            }
                        }
                        else
                        {
                            for (int pageNo = 1; pageNo <= TotalPage; pageNo++)
                            {
                                //updateSql.Clear();
                                ds = DB.GetDatabase().ExecuteDatasetPaging(sql, pageNo, pageSize, 0);
                                if (ds != null && ds.Tables[0].Rows.Count > 0)
                                {
                                    //updateSql.Append("BEGIN ");
                                    //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                                    //{
                                    //    int line_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                                    //    int locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                                    //    int product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                                    //    string AdjustType = Util.GetValueOfString(ds.Tables[0].Rows[j][4]);
                                    //    decimal AsonDateCount = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][5]);
                                    //    decimal DiffQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][6]);
                                    //    decimal currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][7]);
                                    //    string updateQry = UpdateInventoryLine(line_ID, product_ID, locator_ID, currentQty, AdjustType, AsonDateCount, DiffQty);
                                    //    if (updateQry != "")
                                    //    {
                                    //        updateSql.Append(updateQry);
                                    //    }
                                    //}
                                    //ds.Dispose();
                                    //updateSql.Append(" END;");
                                    //int cnt = DB.ExecuteQuery(updateSql.ToString(), null, Get_Trx());
                                    //log.Info(" =====>  records updated at " + DateTime.Now.ToString() + " are = " + count + " <===== ");

                                    string updateQry = DBFunctionCollection.UpdateInventoryLine(GetCtx(), ds, Get_Trx());                                    
                                    ds.Dispose();
                                    int cnt = DB.ExecuteQuery(updateQry, null, Get_Trx());
                                }
                            }
                        }
                        log.Info(" =====>  Physical Inventory update process end at " + DateTime.Now.ToString());
                    }
                }
                catch (Exception e)
                {
                    if (ds != null)
                    {
                        ds.Dispose();
                    }
                    log.Log(Level.SEVERE, sql.ToString(), e);
                }
            }
            inventory.SetIsAdjusted(true);
            if (!inventory.Save())
            {

            }
            if (multiple > 0)
                return "@M_InventoryLine_ID@ - #" + no + " --> @InventoryProductMultiple@";

            //return "@M_InventoryLine_ID@ - #" + (no + noMA);
            return "Physical Inventory Updated";
        }

        /// <summary>
        /// Update Inventory Lines With Material Allocation
        /// </summary>
        /// <returns>no update</returns>
        private int UpdateWithMA()
        {
            int no = 0;
            //
            String sql = "SELECT * FROM M_InventoryLine WHERE M_Inventory_ID=@iid AND COALESCE(M_AttributeSetInstance_ID,0)=0 ";

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@iid", _m_Inventory_ID);
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, null);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MInventoryLine il = new MInventoryLine(GetCtx(), dr, Get_TrxName());
                    Decimal onHand = Env.ZERO;
                    MStorage[] storages = MStorage.GetAll(GetCtx(), il.GetM_Product_ID(), il.GetM_Locator_ID(), Get_TrxName());
                    MInventoryLineMA ma = null;
                    for (int i = 0; i < storages.Length; i++)
                    {
                        MStorage storage = storages[i];
                        if (Env.Signum(storage.GetQtyOnHand()) == 0)
                            continue;
                        onHand = Decimal.Add(onHand, storage.GetQtyOnHand());
                        //	No ASI
                        if (storage.GetM_AttributeSetInstance_ID() == 0
                            && storages.Length == 1)
                            continue;
                        //	Save ASI
                        ma = new MInventoryLineMA(il,
                            storage.GetM_AttributeSetInstance_ID(), storage.GetQtyOnHand());
                        if (!ma.Save())
                            ;
                    }
                    il.SetQtyBook(onHand);
                    il.SetQtyCount(onHand);
                    if (il.Save())
                        no++;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            log.Info("#" + no);
            return no;
        }


        /// <summary>
        /// Create/Add to Inventory Line Query
        /// </summary>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="currentQty">quantity</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <returns>lines added</returns>
        private string UpdateInventoryLine(int M_InventoryLine_ID, int M_Product_ID, int M_Locator_ID, Decimal currentQty, string AdjustType, Decimal AsOnDateCount, Decimal DiffQty)
        {
            string qry = "select m_warehouse_id from m_locator where m_locator_id=" + M_Locator_ID;
            int M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, Get_Trx()));
            MWarehouse wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
            if (wh.IsDisallowNegativeInv() == true)
            {
                if (currentQty < 0)
                {
                    return "";
                }
            }
            MProduct product = MProduct.Get(GetCtx(), M_Product_ID);
            if (product != null)
            {
                int precision = product.GetUOMPrecision();
                if (Env.Signum(currentQty) != 0)
                    currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
            }
            if (AdjustType == "A")
            {
                DiffQty = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, AsOnDateCount));
            }
            else if (AdjustType == "D")
            {
                AsOnDateCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));
            }
            Decimal QtyCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));

            // Changes by Bharat on 02 Aug 2017 as issue given by Ravikant
            string sql = @"UPDATE M_InventoryLine SET QtyBook = " + currentQty + ",QtyCount = " + QtyCount + ",OpeningStock = " + currentQty + ",AsOnDateCount = " + AsOnDateCount +
                ",DifferenceQty = " + DiffQty + " WHERE M_InventoryLine_ID = " + M_InventoryLine_ID;

            string updateQry = " BEGIN execute immediate('" + sql.Replace("'", "''") + "'); exception when others then null; END;";
            return updateQry;
        }
    }
}
