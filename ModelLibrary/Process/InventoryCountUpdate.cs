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
        private int _VAM_Inventory_ID = 0;
        /** Update to What			*/
        private Boolean _inventoryCountSetZero = false;
        private Boolean _AdjustinventoryCount = false;
        private Boolean _SkipBL = false;
        /** Physical Inventory					*/
        private MVAMInventory inventory = null;

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
            _VAM_Inventory_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            log.Info("VAM_Inventory_ID=" + _VAM_Inventory_ID);
            inventory = new MVAMInventory(GetCtx(), _VAM_Inventory_ID, Get_TrxName());
            if (inventory.Get_ID() == 0)
                throw new SystemException("Not found: VAM_Inventory_ID=" + _VAM_Inventory_ID);

            //	Multiple Lines for one item
            //jz simple the SQL so that Derby also like it. To avoid testing Oracle by now, leave no change for Oracle
            String sql = null;
            if (DataBase.DB.IsOracle())
            {
                sql = "UPDATE VAM_InventoryLine SET IsActive='N' "
                    + "WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID
                    + " AND (VAM_Product_ID, VAM_Locator_ID, VAM_PFeature_SetInstance_ID) IN "
                        + "(SELECT VAM_Product_ID, VAM_Locator_ID, VAM_PFeature_SetInstance_ID "
                        + "FROM VAM_InventoryLine "
                        + "WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID
                        + " GROUP BY VAM_Product_ID, VAM_Locator_ID, VAM_PFeature_SetInstance_ID "
                        + (isContainerApplicable ? " , VAM_ProductContainer_ID" : "")
                        + "HAVING COUNT(*) > 1)";
            }
            else
            {
                sql = "UPDATE VAM_InventoryLine SET IsActive='N' "
                    + "WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID
                    + " AND EXISTS "
                        + "(SELECT COUNT(*) "
                        + "FROM VAM_InventoryLine "
                        + "WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID
                        + " GROUP BY VAM_Product_ID, VAM_Locator_ID, VAM_PFeature_SetInstance_ID "
                        + (isContainerApplicable ? " , VAM_ProductContainer_ID" : "")
                        + "HAVING COUNT(*) > 1)";
            }
            int multiple = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Info("Multiple=" + multiple);

            int delMA = MVAMInventoryLineMP.DeleteInventoryMA(_VAM_Inventory_ID, Get_TrxName());
            log.Info("DeletedMA=" + delMA);

            //	ASI
            sql = "UPDATE VAM_InventoryLine l "
                + "SET (QtyBook,QtyCount) = "
                    + "(SELECT QtyOnHand,QtyOnHand FROM VAM_Storage s "
                    + "WHERE s.VAM_Product_ID=l.VAM_Product_ID AND s.VAM_Locator_ID=l.VAM_Locator_ID"
                    + " AND s.VAM_PFeature_SetInstance_ID=l.VAM_PFeature_SetInstance_ID),"
                + " Updated=SysDate,"
                + " UpdatedBy=" + GetVAF_UserContact_ID()
                //
                + " WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID
                + " AND EXISTS (SELECT * FROM VAM_Storage s "
                    + "WHERE s.VAM_Product_ID=l.VAM_Product_ID AND s.VAM_Locator_ID=l.VAM_Locator_ID"
                    + " AND s.VAM_PFeature_SetInstance_ID=l.VAM_PFeature_SetInstance_ID)";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Info("Update with ASI =" + no);

            //	No ASI
            //int noMA = UpdateWithMA();

            //	Set Count to Zero
            if (_inventoryCountSetZero)
            {
                sql = "UPDATE VAM_InventoryLine l "
                    + "SET QtyCount=0 "
                    + "WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                log.Info("Set Count to Zero =" + no);
            }
            if (_AdjustinventoryCount)
            {
                //                MVAMInventoryLine[] lines = inventory.GetLines(true);
                //                for (int i = 0; i < lines.Length; i++)
                //                {
                //                    decimal currentQty = 0;
                //                    string query = "", qry = "";
                //                    int result = 0;
                //                    MVAMInventoryLine iLine = lines[i];
                //                    int VAM_Product_ID = Utility.Util.GetValueOfInt(iLine.GetVAM_Product_ID());
                //                    int VAM_Locator_ID = Utility.Util.GetValueOfInt(iLine.GetVAM_Locator_ID());
                //                    int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(iLine.GetVAM_PFeature_SetInstance_ID());

                //                    query = "SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate = " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                           AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID;
                //                    result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                //                    if (result > 0)
                //                    {
                //                        qry = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_ID =
                //                            (SELECT MAX(VAM_Inv_Trx_ID)   FROM VAM_Inv_Trx
                //                            WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                            AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID + @")
                //                            AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID + @")
                //                            AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID;
                //                        currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry));
                //                    }
                //                    else
                //                    {
                //                        query = "SELECT COUNT(*) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                            AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID;
                //                        result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                //                        if (result > 0)
                //                        {
                //                            qry = @"SELECT currentqty FROM VAM_Inv_Trx WHERE VAM_Inv_Trx_ID =
                //                            (SELECT MAX(VAM_Inv_Trx_ID)   FROM VAM_Inv_Trx
                //                            WHERE movementdate =     (SELECT MAX(movementdate) FROM VAM_Inv_Trx WHERE movementdate < " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                //                            AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID + @")
                //                            AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID + @")
                //                            AND  VAM_Product_ID = " + VAM_Product_ID + " AND VAM_Locator_ID = " + VAM_Locator_ID + " AND VAM_PFeature_SetInstance_ID = " + VAM_PFeature_SetInstance_ID;
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
                    sql = @"SELECT m.VAM_InventoryLine_ID, m.VAM_Locator_ID, m.VAM_Product_ID, m.VAM_PFeature_SetInstance_ID, m.AdjustmentType, m.AsOnDateCount, m.DifferenceQty,
                nvl(mt.CurrentQty, 0) as CurrentQty FROM VAM_InventoryLine m LEFT JOIN (SELECT DISTINCT t.VAM_Locator_ID, t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, t.VAM_ProductContainer_ID,
                FIRST_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, t.VAM_Locator_ID, NVL(t.VAM_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t
                INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) +
                    " AND t.VAF_Client_ID = " + inventory.GetVAF_Client_ID() + @") mt ON m.VAM_Product_ID = mt.VAM_Product_ID AND nvl(m.VAM_PFeature_SetInstance_ID, 0) = nvl(mt.VAM_PFeature_SetInstance_ID, 0) 
                AND m.VAM_Locator_ID = mt.VAM_Locator_ID AND nvl(m.VAM_ProductContainer_ID, 0) = nvl(mt.VAM_ProductContainer_ID, 0) 
                WHERE m.VAM_Inventory_ID = " + _VAM_Inventory_ID + " ORDER BY m.Line";
                }
                else
                {
                    sql = @"SELECT m.VAM_InventoryLine_ID, m.VAM_Locator_ID, m.VAM_Product_ID, m.VAM_PFeature_SetInstance_ID, m.AdjustmentType, m.AsOnDateCount, m.DifferenceQty,
                nvl(mt.CurrentQty, 0) as CurrentQty FROM VAM_InventoryLine m LEFT JOIN (SELECT DISTINCT t.VAM_Locator_ID, t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, 
                FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, t.VAM_Locator_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t
                INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) +
                    " AND t.VAF_Client_ID = " + inventory.GetVAF_Client_ID() + @") mt ON m.VAM_Product_ID = mt.VAM_Product_ID AND nvl(m.VAM_PFeature_SetInstance_ID, 0) = nvl(mt.VAM_PFeature_SetInstance_ID, 0) 
                AND m.VAM_Locator_ID = mt.VAM_Locator_ID WHERE m.VAM_Inventory_ID = " + _VAM_Inventory_ID + " ORDER BY m.Line";
                }

                int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAM_InventoryLine_ID) FROM ( " + sql + " ) t", null, null));
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
                                        MVAMInventoryLine iLine = new MVAMInventoryLine(GetCtx(), line_ID, Get_TrxName());
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
                return "@VAM_InventoryLine_ID@ - #" + no + " --> @InventoryProductMultiple@";

            //return "@VAM_InventoryLine_ID@ - #" + (no + noMA);
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
            String sql = "SELECT * FROM VAM_InventoryLine WHERE VAM_Inventory_ID=@iid AND COALESCE(VAM_PFeature_SetInstance_ID,0)=0 ";

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@iid", _VAM_Inventory_ID);
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, null);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MVAMInventoryLine il = new MVAMInventoryLine(GetCtx(), dr, Get_TrxName());
                    Decimal onHand = Env.ZERO;
                    MVAMStorage[] storages = MVAMStorage.GetAll(GetCtx(), il.GetVAM_Product_ID(), il.GetVAM_Locator_ID(), Get_TrxName());
                    MVAMInventoryLineMP ma = null;
                    for (int i = 0; i < storages.Length; i++)
                    {
                        MVAMStorage storage = storages[i];
                        if (Env.Signum(storage.GetQtyOnHand()) == 0)
                            continue;
                        onHand = Decimal.Add(onHand, storage.GetQtyOnHand());
                        //	No ASI
                        if (storage.GetVAM_PFeature_SetInstance_ID() == 0
                            && storages.Length == 1)
                            continue;
                        //	Save ASI
                        ma = new MVAMInventoryLineMP(il,
                            storage.GetVAM_PFeature_SetInstance_ID(), storage.GetQtyOnHand());
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
        /// <param name="VAM_Locator_ID">locator</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">asi</param>
        /// <param name="currentQty">quantity</param>
        /// <param name="VAM_PFeature_Set_ID">attribute set</param>
        /// <returns>lines added</returns>
        private string UpdateInventoryLine(int VAM_InventoryLine_ID, int VAM_Product_ID, int VAM_Locator_ID, Decimal currentQty, string AdjustType, Decimal AsOnDateCount, Decimal DiffQty)
        {
            string qry = "select VAM_Warehouse_id from VAM_Locator where VAM_Locator_id=" + VAM_Locator_ID;
            int VAM_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, Get_Trx()));
            MWarehouse wh = MWarehouse.Get(GetCtx(), VAM_Warehouse_ID);
            if (wh.IsDisallowNegativeInv() == true)
            {
                if (currentQty < 0)
                {
                    return "";
                }
            }
            MVAMProduct product = MVAMProduct.Get(GetCtx(), VAM_Product_ID);
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
            string sql = @"UPDATE VAM_InventoryLine SET QtyBook = " + currentQty + ",QtyCount = " + QtyCount + ",OpeningStock = " + currentQty + ",AsOnDateCount = " + AsOnDateCount +
                ",DifferenceQty = " + DiffQty + " WHERE VAM_InventoryLine_ID = " + VAM_InventoryLine_ID;

            string updateQry = " BEGIN execute immediate('" + sql.Replace("'", "''") + "'); exception when others then null; END;";
            return updateQry;
        }
    }
}
