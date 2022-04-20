using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.DataBase
{
    public static class DBFunctionCollection
    {
        /// <summary>
        /// To insert Data on Physical Inventory Line from Create Inventory Count Process in case of Skip Bussiness Logic
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="_inventory">Physical Inventory</param>
        /// <param name="ds">DataSet</param>
        /// <param name="lineNo">Line No</param>
        /// <param name="isContainerApplicable">Container Applicable</param>
        /// <param name="trx">Transaction</param>
        /// <returns>Return query as string based on connected Database</returns>
        public static string InsertInventoryLine(Ctx ctx, MInventory _inventory, DataSet ds, int lineNo, bool isContainerApplicable, Trx trx)
        {
            StringBuilder insertSql = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                insertSql.Append("BEGIN ");
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    decimal currentQty = 0;
                    int container_ID = 0;
                    int M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                    int M_Locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                    int M_AttributeSetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                    int M_AttributeSet_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][5]);
                    if (isContainerApplicable)
                    {
                        container_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j]["M_ProductContainer_ID"]);
                    }
                    currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][3]);
                    lineNo = lineNo + 10;
                    MInventoryLine line = new MInventoryLine(ctx, 0, trx);
                    int line_ID = DB.GetNextID(ctx, "M_InventoryLine", trx);
                    string qry = "select m_warehouse_id from m_locator where m_locator_id=" + M_Locator_ID;
                    int M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, trx));
                    MWarehouse wh = MWarehouse.Get(ctx, M_Warehouse_ID);
                    if (wh.IsDisallowNegativeInv() == true)
                    {
                        if (currentQty < 0)
                        {
                            continue;
                        }
                    }
                    MProduct product = MProduct.Get(ctx, M_Product_ID);
                    if (product != null)
                    {
                        int precision = product.GetUOMPrecision();
                        if (Env.Signum(currentQty) != 0)
                            currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
                    }
                    sql.Clear();
                    sql.Append(@"INSERT INTO M_InventoryLine (AD_Client_ID, AD_Org_ID,IsActive, Created, CreatedBy, Updated, UpdatedBy, Line, M_Inventory_ID, M_InventoryLine_ID,  
                                M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, QtyBook, QtyCount, OpeningStock, AsOnDateCount, DifferenceQty, AdjustmentType");

                    if (line.Get_ColumnIndex("C_UOM_ID") > 0)
                    {
                        sql.Append(", QtyEntered, C_UOM_ID");
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",IsFromProcess");
                    }
                    if (line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
                    {
                        sql.Append(",M_ProductContainer_ID ");
                    }

                    sql.Append(" ) VALUES ( " + _inventory.GetAD_Client_ID() + "," + _inventory.GetAD_Org_ID() + ",'Y'," + GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," +
                        GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," + lineNo + "," + _inventory.Get_ID() + "," + line_ID + "," + M_Locator_ID + "," + M_Product_ID + "," +
                        M_AttributeSetInstance_ID + "," + currentQty + "," + currentQty + "," + currentQty + "," + currentQty + "," + 0 + ",'A'");

                    if (line.Get_ColumnIndex("C_UOM_ID") > 0)
                    {
                        sql.Append("," + currentQty + "," + product.GetC_UOM_ID());
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",'Y'");
                    }
                    if (line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
                    {
                        sql.Append("," + container_ID);
                    }

                    insertSql.Append(" BEGIN execute immediate('" + sql.Replace("'", "''") + ")'); exception when others then null; END;");
                }
                insertSql.Append(" END;");
            }
            else if (DB.IsPostgreSQL())
            {
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    decimal currentQty = 0;
                    int container_ID = 0;
                    int M_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                    int M_Locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                    int M_AttributeSetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                    int M_AttributeSet_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][5]);
                    if (isContainerApplicable)
                    {
                        container_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j]["M_ProductContainer_ID"]);
                    }
                    currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][3]);
                    lineNo = lineNo + 10;
                    MInventoryLine line = new MInventoryLine(ctx, 0, trx);
                    int line_ID = DB.GetNextID(ctx, "M_InventoryLine", trx);
                    string qry = "select m_warehouse_id from m_locator where m_locator_id=" + M_Locator_ID;
                    int M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, trx));
                    MWarehouse wh = MWarehouse.Get(ctx, M_Warehouse_ID);
                    if (wh.IsDisallowNegativeInv() == true)
                    {
                        if (currentQty < 0)
                        {
                            continue;
                        }
                    }
                    MProduct product = MProduct.Get(ctx, M_Product_ID);
                    if (product != null)
                    {
                        int precision = product.GetUOMPrecision();
                        if (Env.Signum(currentQty) != 0)
                            currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
                    }
                    sql.Clear();
                    sql.Append(@"INSERT INTO M_InventoryLine (AD_Client_ID, AD_Org_ID,IsActive, Created, CreatedBy, Updated, UpdatedBy, Line, M_Inventory_ID, M_InventoryLine_ID,  
                                M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, QtyBook, QtyCount, OpeningStock, AsOnDateCount, DifferenceQty, AdjustmentType");

                    if (line.Get_ColumnIndex("C_UOM_ID") > 0)
                    {
                        sql.Append(", QtyEntered, C_UOM_ID");
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",IsFromProcess");
                    }
                    if (line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
                    {
                        sql.Append(",M_ProductContainer_ID ");
                    }

                    sql.Append(" ) VALUES ( " + _inventory.GetAD_Client_ID() + "," + _inventory.GetAD_Org_ID() + ",'Y'," + GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," +
                        GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," + lineNo + "," + _inventory.Get_ID() + "," + line_ID + "," + M_Locator_ID + "," + M_Product_ID + "," +
                        M_AttributeSetInstance_ID + "," + currentQty + "," + currentQty + "," + currentQty + "," + currentQty + "," + 0 + ",'A'");

                    if (line.Get_ColumnIndex("C_UOM_ID") > 0)
                    {
                        sql.Append("," + currentQty + "," + product.GetC_UOM_ID());
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",'Y'");
                    }
                    if (line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
                    {
                        sql.Append("," + container_ID);
                    }
                    insertSql.Append(" SELECT ExecuteImmediate('" + sql.Replace("'", "''") + ")') FROM DUAL;");
                }
            }
            return insertSql.ToString();
        }

        /// <summary>
        /// To Update Data on Physical Inventory Line from Update Inventory Quantity Process in case of Skip Bussiness Logic
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="M_InventoryLine_ID">Inventory Line</param>
        /// <param name="ds">DataSet</param>
        /// <param name="trx">Transaction</param>
        /// <returns>Return query as string based on connected Database</returns>
        public static string UpdateInventoryLine(Ctx ctx, DataSet ds, Trx trx)
        {
            StringBuilder updateSql = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                updateSql.Append("BEGIN ");
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    int line_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                    int locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                    int product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                    string AdjustType = Util.GetValueOfString(ds.Tables[0].Rows[j][4]);
                    decimal AsonDateCount = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][5]);
                    decimal DiffQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][6]);
                    decimal currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][7]);
                    MProduct product = MProduct.Get(ctx, product_ID);
                    if (product != null)
                    {
                        int precision = product.GetUOMPrecision();
                        if (Env.Signum(currentQty) != 0)
                            currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
                    }
                    if (AdjustType == "A")
                    {
                        DiffQty = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, AsonDateCount));
                    }
                    else if (AdjustType == "D")
                    {
                        AsonDateCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));
                    }
                    Decimal QtyCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));

                    sql.Clear();
                    sql.Append(@"UPDATE M_InventoryLine SET QtyBook = " + currentQty + ",QtyCount = " + QtyCount + ",OpeningStock = " + currentQty + ",AsOnDateCount = " + AsonDateCount +
                        ",DifferenceQty = " + DiffQty + " WHERE M_InventoryLine_ID = " + line_ID);
                    updateSql.Append(" BEGIN execute immediate('" + sql.Replace("'", "''") + "'); exception when others then null; END;");
                }
                updateSql.Append(" END;");
            }
            else if (DB.IsPostgreSQL())
            {
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    int line_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                    int locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                    int product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                    string AdjustType = Util.GetValueOfString(ds.Tables[0].Rows[j][4]);
                    decimal AsonDateCount = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][5]);
                    decimal DiffQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][6]);
                    decimal currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][7]);
                    MProduct product = MProduct.Get(ctx, product_ID);
                    if (product != null)
                    {
                        int precision = product.GetUOMPrecision();
                        if (Env.Signum(currentQty) != 0)
                            currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
                    }
                    if (AdjustType == "A")
                    {
                        DiffQty = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, AsonDateCount));
                    }
                    else if (AdjustType == "D")
                    {
                        AsonDateCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));
                    }
                    Decimal QtyCount = Util.GetValueOfDecimal(Decimal.Subtract(currentQty, DiffQty));

                    sql.Clear();
                    sql.Append(@"UPDATE M_InventoryLine SET QtyBook = " + currentQty + ",QtyCount = " + QtyCount + ",OpeningStock = " + currentQty + ",AsOnDateCount = " + AsonDateCount +
                        ",DifferenceQty = " + DiffQty + " WHERE M_InventoryLine_ID = " + line_ID);
                    updateSql.Append(" SELECT ExecuteImmediate('" + sql.Replace("'", "''") + "') FROM DUAL;");
                }
            }
            return updateSql.ToString();
        }

        public static string ConcatinateListOfLocators(string locators)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append("SELECT SUBSTR(SYS_CONNECT_BY_PATH(value, ', '), 2) CSV FROM(SELECT value, ROW_NUMBER() OVER(ORDER BY value) rn, COUNT(*) over() CNT FROM "
                                 + " (SELECT DISTINCT value FROM m_locator WHERE M_Locator_ID IN(" + locators.ToString().Trim().Trim(',') + "))) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT DISTINCT string_agg(Value, ', ') AS resultColumn FROM M_Locator WHERE M_Locator_ID IN (" + locators.ToString().Trim().Trim(',') + ")");
            }
            return sql.ToString();
        }

        public static string ConcatinateListOfProducts(string products)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append("SELECT SUBSTR(SYS_CONNECT_BY_PATH(Name, ', '), 2) CSV FROM(SELECT Name, ROW_NUMBER() OVER(ORDER BY Name) rn, COUNT(*) over() CNT FROM "
                                + " M_Product WHERE M_Product_ID IN (" + products.ToString().Trim().Trim(',') + ") ) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT DISTINCT string_agg(Name, ', ') AS resultColumn FROM M_Product WHERE M_Product_ID IN (" + products.ToString().Trim().Trim(',') + ")");
            }
            return sql.ToString();
        }

        public static string MInOutContainerNotMatched(int M_InOut_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '), ',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER() OVER(ORDER BY NotMatched) RN, COUNT(*) OVER() CNT  FROM
                        (SELECT DISTINCT
                        CASE  WHEN p.m_warehouse_id <> i.m_warehouse_id  THEN pr.Name || '_' || il.line
                              WHEN p.m_locator_id <> il.m_locator_id THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM m_inout i INNER JOIN m_inoutline il ON i.m_inout_id = il.m_inout_id
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID = il.M_ProductContainer_ID
                        WHERE il.M_ProductContainer_ID > 0 AND i.m_inout_id = " + M_InOut_ID + @" AND ROWNUM <= 100)  WHERE notmatched IS NOT NULL)
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM (SELECT DISTINCT CASE WHEN p.m_warehouse_id <> i.m_warehouse_id  THEN pr.Name || '_' || il.line
                        WHEN p.m_locator_id <> il.m_locator_id THEN pr.Name || '_' || il.line END AS NotMatched 
                        FROM m_inout i INNER JOIN m_inoutline il ON i.m_inout_id = il.m_inout_id
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID = il.M_ProductContainer_ID
                        WHERE il.M_ProductContainer_ID > 0 AND i.m_inout_id = " + M_InOut_ID + @" AND ROWNUM <= 100) t WHERE notmatched IS NOT NULL");
            }
            return sql.ToString();
        }

        /// <summary>
        /// based on Cash_ID it will check the reference Invoice Schedule's 
        /// are paid or not in the CashLines and returns those if it is paid.
        /// </summary>
        /// <param name="cash_ID">C_Cash_ID</param>
        /// <returns>string query</returns>
        public static string CashLineRefInvScheduleDuePaidOrNot(int cash_ID)
        {
            string sql = null;
            if (DB.IsOracle())
            {
                sql = @"SELECT LTRIM(SYS_CONNECT_BY_PATH( PaidSchedule, ' , '),',') PaidSchedule FROM
                                              (SELECT PaidSchedule, ROW_NUMBER () OVER (ORDER BY PaidSchedule ) RN, COUNT (*) OVER () CNT FROM 
                                                (SELECT cs.duedate || '_' || cs.dueamt AS PaidSchedule FROM C_Cash c 
                                                 INNER JOIN C_CashLine cl ON c.c_cash_id = cl.c_cash_id 
                                                 INNER JOIN C_InvoicePaySchedule cs ON cs.C_InvoicePaySchedule_ID = cl.C_InvoicePaySchedule_ID 
                                                 INNER JOIN C_Invoice inv ON inv.C_Invoice_ID = cl.C_Invoice_ID AND inv.DocStatus NOT IN ('RE' , 'VO') 
                                                 WHERE cl.CashType = 'I' AND  cl.IsActive = 'Y' AND c.IsActive = 'Y' AND cs.IsActive = 'Y' 
                                                 AND NVL(cl.C_Invoice_ID , 0)  <> 0 AND (NVL(cs.c_payment_id,0)  != 0
                                                 OR NVL(cs.c_cashline_id , 0) != 0 OR cs.VA009_IsPaid = 'Y') AND c.c_cash_id = " + cash_ID +
                                                 @" AND ROWNUM <= 100 )  )
                                                 WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1";
            }
            else if (DB.IsPostgreSQL())
            {
                sql = @"SELECT string_agg(PaidSchedule, ', ') PaidSchedule FROM
                                              (SELECT PaidSchedule, ROW_NUMBER () OVER (ORDER BY PaidSchedule ) RN, COUNT (*) OVER () CNT FROM 
                                                (SELECT cs.duedate || '_' || cs.dueamt AS PaidSchedule FROM C_Cash c 
                                                 INNER JOIN C_CashLine cl ON c.c_cash_id = cl.c_cash_id 
                                                 INNER JOIN C_InvoicePaySchedule cs ON cs.C_InvoicePaySchedule_ID = cl.C_InvoicePaySchedule_ID 
                                                 INNER JOIN C_Invoice inv ON inv.C_Invoice_ID = cl.C_Invoice_ID AND inv.DocStatus NOT IN ('RE' , 'VO') 
                                                 WHERE cl.CashType = 'I' AND  cl.IsActive = 'Y' AND c.IsActive = 'Y' AND cs.IsActive = 'Y' 
                                                 AND NVL(cl.C_Invoice_ID , 0)  <> 0 AND (NVL(cs.c_payment_id,0)  != 0
                                                 OR NVL(cs.c_cashline_id , 0) != 0 OR cs.VA009_IsPaid = 'Y') AND c.c_cash_id = " + cash_ID +
                                                 @") t ORDER BY PaidSchedule LIMIT 100) tb
                                                 WHERE PaidSchedule IS NOT NULL";
            }
            return sql;
        }

        /// <summary>
        /// Based on Cash_ID it will check the reference Order Schedule's 
        /// are paid or not in the CashLines and returns those if it is paid.
        /// Author: VA230
        /// </summary>
        /// <param name="cash_ID">C_Cash_ID</param>
        /// <returns>string query</returns>
        public static string CashLineRefOrerPayScheduleDuePaidOrNot(int cash_ID)
        {
            string sql = null;
            if (DB.IsOracle())
            {
                sql = @" SELECT LTRIM(SYS_CONNECT_BY_PATH( PaidSchedule, ' , '),',') PaidSchedule FROM
                                              (SELECT PaidSchedule, ROW_NUMBER () OVER (ORDER BY PaidSchedule ) RN, COUNT (*) OVER () CNT FROM 
                                                (SELECT cs.duedate || '_' || cs.dueamt AS PaidSchedule FROM C_Cash c 
                                                 INNER JOIN C_CashLine cl ON c.c_cash_id = cl.c_cash_id 
                                                 INNER JOIN VA009_OrderPaySchedule cs ON cs.VA009_OrderPaySchedule_ID = cl.VA009_OrderPaySchedule_ID 
                                                 INNER JOIN C_Order ord ON ord.C_Order = cl.C_Order_ID AND ord.DocStatus NOT IN ('RE' , 'VO') 
                                                 WHERE cl.CashType = 'I' AND  cl.IsActive = 'Y' AND c.IsActive = 'Y' AND cs.IsActive = 'Y' 
                                                 AND NVL(cl.C_Order_ID , 0)  <> 0 AND (NVL(cs.c_payment_id,0)  != 0
                                                 OR NVL(cs.c_cashline_id , 0) != 0 OR cs.VA009_IsPaid = 'Y') AND c.c_cash_id = " + cash_ID + @"
                                                 AND ROWNUM <= 100 )  )
                                                 WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1";
            }
            else if (DB.IsPostgreSQL())
            {
                sql = @"SELECT string_agg(PaidSchedule, ', ') PaidSchedule FROM
                                              (SELECT PaidSchedule, ROW_NUMBER () OVER (ORDER BY PaidSchedule ) RN, COUNT (*) OVER () CNT FROM 
                                                (SELECT cs.duedate || '_' || cs.dueamt AS PaidSchedule FROM C_Cash c 
                                                 INNER JOIN C_CashLine cl ON c.c_cash_id = cl.c_cash_id 
                                                 INNER JOIN VA009_OrderPaySchedule cs ON cs.VA009_OrderPaySchedule_ID = cl.VA009_OrderPaySchedule_ID 
                                                 INNER JOIN C_Order ord ON ord.C_Order_ID = cl.C_Order_ID AND ord.DocStatus NOT IN ('RE' , 'VO') 
                                                 WHERE cl.CashType = 'I' AND  cl.IsActive = 'Y' AND c.IsActive = 'Y' AND cs.IsActive = 'Y' 
                                                 AND NVL(cl.C_Order_ID , 0)  <> 0 AND (NVL(cs.c_payment_id,0)  != 0
                                                 OR NVL(cs.c_cashline_id , 0) != 0 OR cs.VA009_IsPaid = 'Y') AND c.c_cash_id = " + cash_ID +
                                                 @") t ORDER BY PaidSchedule LIMIT 100) tb
                                                 WHERE PaidSchedule IS NOT NULL";
            }
            return sql;
        }

        public static string MInOutContainerNotAvailable(int M_InOut_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( name, ' , '),',') name FROM
                          (SELECT Name , ROW_NUMBER () OVER (ORDER BY Name) RN, COUNT (*) OVER () CNT FROM
                          (SELECT (SELECT NAME FROM M_ProductContainer WHERE M_ProductContainer_ID =IL.M_PRODUCTCONTAINER_ID ) AS Name ,
                          CASE WHEN IL.M_PRODUCTCONTAINER_ID > 0 
                          AND (SELECT DATELASTINVENTORY  FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)<=I.MOVEMENTDATE
                          THEN 1 WHEN IL.M_PRODUCTCONTAINER_ID > 0
                          AND (SELECT DATELASTINVENTORY FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)>I.MOVEMENTDATE
                          THEN 0 ELSE 1 END AS COUNT
                          FROM M_InOut I INNER JOIN M_InOutLINE IL ON I.M_InOut_ID = IL.M_InOut_ID
                          WHERE I.M_InOut_ID =" + M_InOut_ID + @" AND ROWNUM <= 100)
                          WHERE COUNT = 0 )
                          WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Name FROM                           
                          (SELECT DISTINCT (SELECT NAME FROM M_ProductContainer WHERE M_ProductContainer_ID =IL.M_PRODUCTCONTAINER_ID ) AS Name ,
                          CASE WHEN IL.M_PRODUCTCONTAINER_ID > 0 
                          AND (SELECT DATELASTINVENTORY  FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)<=I.MOVEMENTDATE
                          THEN 1 WHEN IL.M_PRODUCTCONTAINER_ID > 0
                          AND (SELECT DATELASTINVENTORY FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)>I.MOVEMENTDATE
                          THEN 0 ELSE 1 END AS COUNT
                          FROM M_InOut I INNER JOIN M_InOutLINE IL ON I.M_InOut_ID = IL.M_InOut_ID
                          WHERE I.M_InOut_ID =" + M_InOut_ID + @") t WHERE COUNT = 0 AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        public static string ShipConfirmNoActualValue(string mInOutLinesConfirm)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT SUBSTR(Sys_Connect_By_Path (Name, ','),2) AS Product FROM
                            (SELECT Name, Row_Number () Over (Order By Name ) Rn, COUNT (*) OVER () cnt 
                            FROM ((SELECT DISTINCT Prod.Name AS Name FROM Va010_Shipconfparameters Shp 
                            INNER JOIN M_Product Prod ON prod.m_product_id = shp.m_product_id 
                            WHERE ( NVL(Shp.Va010_Actualvalue,0)) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.M_Inoutlineconfirm_Id IN (" + mInOutLinesConfirm + @"))))
                            WHERE rn = cnt START WITH rn = 1 CONNECT BY Rn = Prior Rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Product FROM 
                            (SELECT DISTINCT Prod.Name AS Name FROM Va010_Shipconfparameters Shp 
                            INNER JOIN M_Product Prod ON prod.m_product_id = shp.m_product_id 
                            WHERE NVL(Shp.Va010_Actualvalue,0) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.M_Inoutlineconfirm_Id IN (" + mInOutLinesConfirm + "))s t");
            }
            return sql.ToString();
        }

        public static string ConcatnatedListOfRequisition(string delReq)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append("SELECT SUBSTR(SYS_CONNECT_BY_PATH(Name, ', '), 2) CSV FROM(SELECT ord.DocumentNo || '_' || ol.Line AS Name, ROW_NUMBER() OVER(ORDER BY ord.DocumentNo, ol.Line) rn, COUNT(*) over() CNT FROM "
                         + " M_RequisitionLine ol INNER JOIN M_Requisition ord ON ol.M_Requisition_ID = ord.M_Requisition_ID WHERE M_RequisitionLine_ID IN (" + delReq.ToString().Trim().Trim(',') + ") ) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT DISTINCT string_agg(ord.DocumentNo || '_' || ol.Line, ', ') AS Name FROM M_RequisitionLine ol INNER JOIN M_Requisition ord ON ol.M_Requisition_ID = ord.M_Requisition_ID WHERE M_RequisitionLine_ID IN (" + delReq.ToString().Trim().Trim(',') + ")");
            }
            return sql.ToString();
        }

        public static string MInventoryContainerNotMatched(int M_Inventory_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '),',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER () OVER (ORDER BY NotMatched ) RN, COUNT (*) OVER () CNT  FROM
                        (SELECT DISTINCT 
                        CASE  WHEN p.m_warehouse_id <> i.m_warehouse_id  THEN pr.Name || '_' || il.line
                              WHEN p.m_locator_id <> il.m_locator_id THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM M_Inventory i INNER JOIN M_Inventoryline il ON i.M_Inventory_ID = il.M_Inventory_ID
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID  = il.M_ProductContainer_ID
                        WHERE il.M_ProductContainer_ID > 0 AND i.M_Inventory_ID = " + M_Inventory_ID + @" AND ROWNUM <= 100) WHERE NotMatched IS NOT NULL ) 
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM (SELECT DISTINCT 
                        CASE  WHEN p.m_warehouse_id <> i.m_warehouse_id  THEN pr.Name || '_' || il.line
                              WHEN p.m_locator_id <> il.m_locator_id THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM M_Inventory i INNER JOIN M_Inventoryline il ON i.M_Inventory_ID = il.M_Inventory_ID
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID  = il.M_ProductContainer_ID
                        WHERE il.M_ProductContainer_ID > 0 AND i.M_Inventory_ID = " + M_Inventory_ID + @" ) t WHERE NotMatched IS NOT NULL AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        public static string MInventoryContainerNotAvailable(int M_Inventory_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( name, ' , '),',') name FROM
                            (SELECT Name , ROW_NUMBER () OVER (ORDER BY name ) RN, COUNT (*) OVER () CNT FROM
                            (SELECT (SELECT NAME FROM M_ProductContainer WHERE M_ProductContainer_ID =IL.M_PRODUCTCONTAINER_ID ) AS Name ,
                            CASE WHEN IL.M_PRODUCTCONTAINER_ID>0 
                            AND (SELECT DATELASTINVENTORY  FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)<=I.MOVEMENTDATE
                            THEN 1 WHEN IL.M_PRODUCTCONTAINER_ID>0
                            AND (SELECT DATELASTINVENTORY FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)>I.MOVEMENTDATE
                            THEN 0 ELSE 1 END AS COUNT
                            FROM M_INVENTORY I INNER JOIN M_INVENTORYLINE IL ON I.M_INVENTORY_ID = IL.M_INVENTORY_ID
                            WHERE I.M_INVENTORY_ID =" + M_Inventory_ID + @" AND ROWNUM <= 100 )
                            WHERE COUNT = 0) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Name FROM 
                            (SELECT DISTINCT (SELECT NAME FROM M_ProductContainer WHERE M_ProductContainer_ID =IL.M_PRODUCTCONTAINER_ID ) AS Name ,
                            CASE WHEN IL.M_PRODUCTCONTAINER_ID>0 
                            AND (SELECT DATELASTINVENTORY  FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)<=I.MOVEMENTDATE
                            THEN 1 WHEN IL.M_PRODUCTCONTAINER_ID>0
                            AND (SELECT DATELASTINVENTORY FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=IL.M_PRODUCTCONTAINER_ID)>I.MOVEMENTDATE
                            THEN 0 ELSE 1 END AS COUNT
                            FROM M_INVENTORY I INNER JOIN M_INVENTORYLINE IL ON I.M_INVENTORY_ID = IL.M_INVENTORY_ID
                            WHERE I.M_INVENTORY_ID =" + M_Inventory_ID + @" ) t WHERE COUNT = 0 AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        public static string MovementContainerNotMatched(int M_Movement_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '),',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER () OVER (ORDER BY NotMatched ) RN, COUNT (*) OVER () CNT  FROM
                        (SELECT DISTINCT 
                        CASE  WHEN p.m_warehouse_id <> i.DTD001_MWarehouseSource_ID  THEN pr.Name || '_' || il.line
                              WHEN p.M_Locator_ID <> il.M_Locator_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM M_Movement i INNER JOIN M_Movementline il ON i.M_Movement_ID = il.M_Movement_ID
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID  = il.M_ProductContainer_ID
                        WHERE il.M_ProductContainer_ID > 0 AND i.M_Movement_ID = " + M_Movement_ID + @" AND ROWNUM <= 100 )  WHERE notmatched IS NOT NULL ) 
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM  (SELECT DISTINCT 
                        CASE  WHEN p.m_warehouse_id <> i.DTD001_MWarehouseSource_ID  THEN pr.Name || '_' || il.line
                              WHEN p.M_Locator_ID <> il.M_Locator_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM M_Movement i INNER JOIN M_Movementline il ON i.M_Movement_ID = il.M_Movement_ID
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID  = il.M_ProductContainer_ID
                        WHERE il.M_ProductContainer_ID > 0 AND i.M_Movement_ID = " + M_Movement_ID + @" ) t WHERE NotMatched IS NOT NULL AND ROWNUM <= 100 ");
            }
            return sql.ToString();
        }

        public static string MovementContainerToNotMatched(int M_Movement_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '),',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER () OVER (ORDER BY NotMatched ) RN, COUNT (*) OVER () CNT  FROM
                        (SELECT DISTINCT 
                        CASE  WHEN p.m_warehouse_id <> i.DTD001_MWarehouseSource_ID  THEN pr.Name || '_' || il.line
                              WHEN p.M_Locator_ID <> il.M_Locator_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM M_Movement i INNER JOIN M_Movementline il ON i.M_Movement_ID = il.M_Movement_ID
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID  = il.M_ProductContainer_ID
                        WHERE il.M_ProductContainer_ID > 0 AND i.M_Movement_ID = " + M_Movement_ID + @" AND ROWNUM <= 100 )  WHERE notmatched IS NOT NULL ) 
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM (SELECT DISTINCT 
                        CASE  WHEN p.m_warehouse_id <> l.M_Warehouse_ID  THEN pr.Name || '_' || il.line
                              WHEN p.M_Locator_ID <> il.M_LocatorTo_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM M_Movement i INNER JOIN M_Movementline il ON i.M_Movement_ID = il.M_Movement_ID
                        INNER JOIN M_Locator l ON l.M_Locator_ID = il.M_LocatorTo_ID
                        INNER JOIN m_product pr ON pr.m_product_id = il.m_product_id
                        INNER JOIN M_ProductContainer p ON p.M_ProductContainer_ID  = il.Ref_M_ProductContainerTo_ID
                        WHERE il.Ref_M_ProductContainerTo_ID > 0 AND i.M_Movement_ID = " + M_Movement_ID + @" ) t WHERE notmatched IS NOT NULL AND ROWNUM <= 100 ");
            }
            return sql.ToString();
        }

        public static string MovementContainerNotAvailable(int M_Movement_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append("SELECT LTRIM(SYS_CONNECT_BY_PATH( PCNAME, ' , '),',') NAME"
                                  + @" FROM (
                               SELECT NAME ,NAMETO ,
                                 CASE
                                   WHEN PCFROM =0
                                   AND PCTO=0
                                   THEN TRIM(NAME)
                                   || ' '
                                   ||TRIM(NAMETO)
                                   WHEN PCFROM=0
                                   THEN TRIM(NAME)
                                   WHEN PCTO=0
                                   THEN TRIM(NAMETO)
                                   ELSE NULL
                                 END AS PCNAME ,
                                 ROW_NUMBER () OVER (ORDER BY NAME ) RN,
                                 COUNT (*) OVER () CNT
                               FROM
                                 (SELECT
                                   (SELECT NAME
                                   FROM M_PRODUCTCONTAINER
                                   WHERE M_PRODUCTCONTAINER_ID =ML.M_PRODUCTCONTAINER_ID
                                   ) AS NAME ,
                                   (SELECT NAME
                                   FROM M_PRODUCTCONTAINER
                                   WHERE M_PRODUCTCONTAINER_ID =ML.REF_M_PRODUCTCONTAINERTO_ID
                                   ) AS NAMETO ,
                                   CASE
                                     WHEN ML.M_PRODUCTCONTAINER_ID>0
                                     THEN
                                       CASE
                                         WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY'))
                                           FROM M_PRODUCTCONTAINER
                                           WHERE M_PRODUCTCONTAINER_ID=ML.M_PRODUCTCONTAINER_ID ) <=M.MOVEMENTDATE
                                         THEN 1
                                         ELSE 0
                                       END
                                     ELSE 1
                                   END AS PCFROM ,
                                   CASE
                                     WHEN ML.REF_M_PRODUCTCONTAINERTO_ID>0
                                     THEN
                                       CASE
                                         WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY'))
                                           FROM M_PRODUCTCONTAINER
                                           WHERE M_PRODUCTCONTAINER_ID=ML.REF_M_PRODUCTCONTAINERTO_ID ) <=M.MOVEMENTDATE
                                         THEN 1
                                         ELSE 0
                                       END
                                     ELSE 1
                                   END AS PCTO
                                 FROM M_MOVEMENT M
                                 INNER JOIN M_MOVEMENTLINE ML
                                 ON M.M_MOVEMENT_ID    =ML.M_MOVEMENT_ID
                                 WHERE M.M_MOVEMENT_ID =" + M_Movement_ID
                                     + @" AND ROWNUM <= 100
                                 )
                               WHERE (PCFROM = 0 OR PCTO =0)
                               ) WHERE RN  = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(CASE WHEN PCFROM = 0 AND PCTO = 0 THEN TRIM(NAME) || ' ' ||TRIM(NAMETO) 
                            WHEN PCFROM=0 THEN TRIM(NAME) WHEN PCTO=0 THEN TRIM(NAMETO) ELSE NULL END, ', ') AS PCNAME 
                            FROM (SELECT (SELECT NAME FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID =ML.M_PRODUCTCONTAINER_ID ) AS NAME , 
                            (SELECT NAME FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID =ML.REF_M_PRODUCTCONTAINERTO_ID ) AS NAMETO , 
                            CASE WHEN ML.M_PRODUCTCONTAINER_ID>0 THEN CASE WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY')) 
                            FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=ML.M_PRODUCTCONTAINER_ID ) <=M.MOVEMENTDATE THEN 1 ELSE 0 END ELSE 1 END AS PCFROM, 
                            CASE WHEN ML.REF_M_PRODUCTCONTAINERTO_ID>0 THEN CASE WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY')) 
                            FROM M_PRODUCTCONTAINER WHERE M_PRODUCTCONTAINER_ID=ML.REF_M_PRODUCTCONTAINERTO_ID ) <= M.MOVEMENTDATE THEN 1 ELSE 0 END ELSE 1 END AS PCTO 
                            FROM M_MOVEMENT M INNER JOIN M_MOVEMENTLINE ML ON M.M_MOVEMENT_ID = ML.M_MOVEMENT_ID WHERE M.M_MOVEMENT_ID = " + M_Movement_ID + @" ) t 
                            WHERE (PCFROM = 0 OR PCTO = 0) AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        public static string CheckContainerQty(int M_Movement_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( PName, ' , '),',') PName FROM 
                                               (SELECT PName, ROW_NUMBER () OVER (ORDER BY PName ) RN, COUNT (*) OVER () CNT FROM 
                                               (
                                                SELECT p.Name || '_' || asi.description || '_' || ml.line  AS PName 
                                                FROM m_movementline ml INNER JOIN m_movement m ON m.m_movement_id = ml.m_movement_id
                                                INNER JOIN m_product p ON p.m_product_id = ml.m_product_id
                                                LEFT JOIN m_attributesetinstance asi ON NVL(asi.M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID,0)
                                                 WHERE ml.MoveFullContainer ='Y' AND m.M_Movement_ID =" + M_Movement_ID + @"
                                                    AND abs(ml.movementqty) <>
                                                     NVL((SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.M_Transaction_ID) AS CurrentQty
                                                     FROM m_transaction t INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID
                                                      WHERE t.MovementDate <= (Select MAX(movementdate) from m_transaction where 
                                                            AD_Client_ID = m.AD_Client_ID  AND M_Locator_ID = ml.M_LocatorTo_ID
                                                            AND M_Product_ID = ml.M_Product_ID AND NVL(M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID,0)
                                                            AND NVL(M_ProductContainer_ID, 0) = NVL(ml.M_ProductContainer_ID, 0) )
                                                       AND t.AD_Client_ID                     = m.AD_Client_ID
                                                       AND t.M_Locator_ID                     = ml.M_LocatorTo_ID
                                                       AND t.M_Product_ID                     = ml.M_Product_ID
                                                       AND NVL(t.M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID,0)
                                                       AND NVL(t.M_ProductContainer_ID, 0)    = NVL(ml.M_ProductContainer_ID, 0)  ), 0) 
                                                       AND ROWNUM <= 100 )
                                               ) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }

            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(PName, ', ') FROM 
                        (SELECT p.Name || '_' || asi.description || '_' || ml.line  AS PName 
                        FROM m_movementline ml INNER JOIN m_movement m ON m.m_movement_id = ml.m_movement_id
                        INNER JOIN m_product p ON p.m_product_id = ml.m_product_id
                        LEFT JOIN m_attributesetinstance asi ON NVL(asi.M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID,0)
                        WHERE ml.MoveFullContainer ='Y' AND m.M_Movement_ID =" + M_Movement_ID + @"
                        AND abs(ml.movementqty) <>
                        NVL((SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, 
                        t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty
                        FROM m_transaction t INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID
                        WHERE t.MovementDate <= (Select MAX(movementdate) from m_transaction where 
                        AD_Client_ID = m.AD_Client_ID  AND M_Locator_ID = ml.M_LocatorTo_ID
                        AND M_Product_ID = ml.M_Product_ID AND NVL(M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID,0)
                        AND NVL(M_ProductContainer_ID, 0) = NVL(ml.M_ProductContainer_ID, 0) )
                        AND t.AD_Client_ID                     = m.AD_Client_ID
                        AND t.M_Locator_ID                     = ml.M_LocatorTo_ID
                        AND t.M_Product_ID                     = ml.M_Product_ID
                        AND NVL(t.M_AttributeSetInstance_ID,0) = NVL(ml.M_AttributeSetInstance_ID,0)
                        AND NVL(t.M_ProductContainer_ID, 0)    = NVL(ml.M_ProductContainer_ID, 0)), 0)
                        ) final_ WHERE PName is not null AND ROWNUM <= 100 ");
            }
            return sql.ToString();
        }

        public static string CheckMoveContainer(int M_Movement_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( PName, ' , '),',') PName FROM
                             (SELECT PName, ROW_NUMBER() OVER(ORDER BY PName) RN, COUNT(*) OVER() CNT FROM(
                               SELECT ttt.PName, tt.CurrentQty, ttt.CurrentQty
                                  FROM(SELECT '' AS PName, m_movementline_id, (ContainerCurrentQty)CurrentQty FROM(SELECT ml.m_movementline_id,
                                  t.ContainerCurrentQty, t.MovementDate, t.M_Transaction_ID,
                                  row_number() OVER(PARTITION BY ml.m_movementline_id ORDER BY t.M_Transaction_ID DESC, t.MovementDate DESC) RN_
                                FROM m_transaction T INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID
                                INNER JOIN M_movementLine ml ON(t.AD_Client_ID = ml.AD_Client_ID
                                AND t.M_Locator_ID = ml.M_Locator_ID
                                AND t.M_Product_ID = ml.M_Product_ID
                                AND NVL(t.M_AttributeSetInstance_ID, 0) = NVL(ml.M_AttributeSetInstance_ID, 0)
                                AND NVL(t.M_ProductContainer_ID, 0) = NVL(ml.M_ProductContainer_ID, 0))
                                WHERE ml.M_Movement_ID = " + M_Movement_ID + @") WHERE RN_ = 1) tt
                             INNER JOIN
                              (SELECT p.Name || '_' || asi.description || '_' || ml.line AS PName, ml.m_movementline_id, ml.movementqty AS CurrentQty
                               FROM m_movementline ml INNER JOIN m_movement m  ON m.m_movement_id = ml.m_movement_id
                               INNER JOIN m_product p ON p.m_product_id = ml.m_product_id
                               LEFT JOIN m_attributesetinstance asi ON NVL(asi.M_AttributeSetInstance_ID, 0) = NVL(ml.M_AttributeSetInstance_ID, 0)
                               WHERE ml.MoveFullContainer = 'Y'
                               AND m.M_Movement_ID = " + M_Movement_ID + @") ttt ON tt.m_movementline_id = ttt.m_movementline_id
                               WHERE tt.CurrentQty <> ttt.CurrentQty AND ROWNUM <= 100))
                           WHERE RN = CNT START WITH RN = 1   CONNECT BY RN = PRIOR RN + 1 ");
            }

            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(PName, ', ') FROM
                                (SELECT ttt.PName , tt.CurrentQty, ttt.CurrentQty 
                                FROM (SELECT '' AS PName, m_movementline_id, (ContainerCurrentQty) CurrentQty FROM (SELECT ml.m_movementline_id,
                                t.ContainerCurrentQty, t.MovementDate,  t.M_Transaction_ID,
                                row_number() OVER (PARTITION BY ml.m_movementline_id ORDER BY t.M_Transaction_ID DESC, t.MovementDate DESC) RN_
                                FROM m_transaction T INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID
                                INNER JOIN M_movementLine ml ON ( t.AD_Client_ID = ml.AD_Client_ID
                                AND t.M_Locator_ID = ml.M_Locator_ID
                                AND t.M_Product_ID = ml.M_Product_ID
                                AND NVL (t.M_AttributeSetInstance_ID, 0) = NVL (ml.M_AttributeSetInstance_ID, 0)
                                AND NVL (t.M_ProductContainer_ID, 0) = NVL (ml.M_ProductContainer_ID, 0))
                                WHERE ml.M_Movement_ID = " + M_Movement_ID + @" ) tt_ WHERE RN_ = 1 ) tt
                                INNER JOIN 
                                (SELECT p.Name || '_' || asi.description || '_'  || ml.line AS PName, ml.m_movementline_id ,  ml.movementqty AS CurrentQty
                                FROM m_movementline ml INNER JOIN m_movement m  ON m.m_movement_id = ml.m_movement_id
                                INNER JOIN m_product p ON p.m_product_id = ml.m_product_id
                                LEFT JOIN m_attributesetinstance asi ON NVL (asi.M_AttributeSetInstance_ID, 0) = NVL (ml.M_AttributeSetInstance_ID, 0)
                                WHERE ml.MoveFullContainer = 'Y'
                                AND m.M_Movement_ID   = " + M_Movement_ID + @" ) ttt ON tt.m_movementline_id = ttt.m_movementline_id
                                WHERE tt.CurrentQty <> ttt.CurrentQty) final_ WHERE PName IS NOT NULL AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        /// <summary>
        /// This function is used to create query as per DB Support
        /// </summary>
        /// <param name="C_Payment_ID">Payment ID</param>
        /// <returns>SQL</returns>
        public static string CheckPaidScheduleAgainstPayment(int C_Payment_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( PaidSchedule, ' , '),',') PaidSchedule FROM
                                              (SELECT PaidSchedule, ROW_NUMBER () OVER (ORDER BY PaidSchedule ) RN, COUNT (*) OVER () CNT FROM 
                                                (SELECT duedate || '_' || dueamt AS PaidSchedule FROM C_PaymentAllocate pa
                                                INNER JOIN C_invoice i ON i.c_invoice_id = pa.c_invoice_id
                                                INNER JOIN C_InvoicePaySchedule ips ON (ips.C_Invoice_ID = i.C_Invoice_ID AND pa.C_InvoicePaySchedule_id = ips.C_InvoicePaySchedule_id) 
                                                 WHERE pa.IsActive = 'Y' AND ips.IsActive = 'Y' AND NVL(pa.C_Invoice_ID , 0)  <> 0 AND (NVL(ips.c_payment_id,0)  != 0
                                                 OR NVL(ips.c_cashline_id , 0) != 0 OR ips.VA009_IsPaid = 'Y') AND pa.C_Payment_ID = " + C_Payment_ID +
                                                   @" AND ROWNUM <= 100 )  )
                                                 WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(PaidSchedule, ', ') FROM 
                                (SELECT ips.duedate || '_' || ips.dueamt AS PaidSchedule FROM C_PaymentAllocate pa
                                INNER JOIN C_invoice i ON i.c_invoice_id = pa.c_invoice_id
                                INNER JOIN C_InvoicePaySchedule ips ON (ips.C_Invoice_ID = i.C_Invoice_ID 
                                AND pa.C_InvoicePaySchedule_id = ips.C_InvoicePaySchedule_id)
                                WHERE pa.IsActive = 'Y' AND ips.IsActive = 'Y' AND NVL(pa.C_Invoice_ID , 0)  <> 0 
                                AND (NVL(ips.c_payment_id,0)  != 0
                                OR NVL(ips.c_cashline_id , 0) != 0 OR ips.VA009_IsPaid = 'Y')
                                AND pa.C_Payment_ID =" + C_Payment_ID + " LIMIT 100 )t WHERE PaidSchedule IS NOT NULL");
            }
            return sql.ToString();
        }

        public static string MoveConfirmNoActualValue(string mMovementLinesConfirm)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT SUBSTR(Sys_Connect_By_Path (Name, ','),2) AS Product FROM
                            (SELECT Name, Row_Number () Over (Order By Name ) Rn, COUNT (*) OVER () cnt 
                            FROM ((SELECT DISTINCT Prod.Name AS Name FROM VA010_MoveConfParameters Shp 
                            INNER JOIN M_Product Prod ON prod.m_product_id = shp.m_product_id 
                            WHERE ( NVL(Shp.Va010_Actualvalue,0)) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.M_MovementLineConfirm_ID IN (" + mMovementLinesConfirm + @"))))
                            WHERE rn = cnt START WITH rn = 1 CONNECT BY Rn = Prior Rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Product FROM 
                            (SELECT DISTINCT Prod.Name AS Name FROM VA010_MoveConfParameters Shp 
                            INNER JOIN M_Product Prod ON prod.m_product_id = shp.m_product_id 
                            WHERE NVL(Shp.Va010_Actualvalue,0) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.M_MovementLineConfirm_ID IN (" + mMovementLinesConfirm + "))s t");
            }
            return sql.ToString();
        }

       

        /// <summary>
        /// Create Query, which will check table exitence into Schema
        /// </summary>
        /// <param name="table_catalog">For Oracle - User_ID, For PostGre -- DataBase Name</param>
        /// <param name="tableName">tableName</param>
        /// <returns></returns>
        public static string CheckTableExistence(string table_catalog, string tableName)
        {
            if (DB.IsPostgreSQL())
            {
                return "SELECT COUNT(*) FROM information_schema.tables WHERE  UPPER(table_catalog) = UPPER('" + table_catalog + "')" +
                    " AND UPPER(table_name)   = UPPER('" + tableName + "')";
            }
            else
            {
                return "SELECT COUNT(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('" + tableName + "')" +
                   " AND OWNER LIKE '" + table_catalog + "'";
            }
        }

        static Classes.CCache<string, bool> _cacheTblName = new Classes.CCache<string, bool>("DBColl_TablesExist", 100);

        /// <summary>
        /// Check whether table exist in database
        /// </summary>
        /// <param name="table_catalog">For Oracle - User_ID, For PostGre -- DataBase Name</param>
        /// <param name="tableName">tableName</param>
        /// <returns>true/false</returns>
        public static bool IsTableExists(string table_catalog, string tableName)
        {
            bool tblExists = true;
            if (_cacheTblName.ContainsKey(tableName.ToUpper()))
            {
                return _cacheTblName[tableName.ToUpper()];
            }
            else
            {
                if (DB.IsPostgreSQL())
                {
                    tblExists = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM information_schema.tables WHERE  UPPER(table_catalog) = UPPER('" + table_catalog + "')" +
                        " AND UPPER(table_name)   = UPPER('" + tableName + "')")) > 0;
                }
                else
                {
                    tblExists = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM all_objects WHERE object_type IN ('TABLE') AND (object_name)  = UPPER('" + tableName + "')" +
                       " AND OWNER LIKE '" + table_catalog + "'")) > 0;
                }
                _cacheTblName.Add(tableName.ToUpper(), tblExists);
            }
            return tblExists;
        }

        /// <summary>
        /// This function is used to type columen name as intezer
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <returns>type cast column</returns>
        public static string TypecastColumnAsInt(string columnName)
        {
            if (DB.IsOracle())
            {
                return columnName;
            }
            else if (DB.IsPostgreSQL())
            {
                return columnName + " :: INT ";
            }
            return columnName;
        }

        /// <summary>
        /// This Function is used to get auto sequence no
        /// </summary>
        /// <param name="aggregation">rownum keyword</param>
        /// <returns>aggregation syntax</returns>
        public static string RowNumAggregation(string aggregation)
        {
            if (DB.IsOracle())
            {
                return aggregation;
            }
            else if (DB.IsPostgreSQL())
            {
                return "row_number()over()";
            }
            return aggregation;
        }

        /// <summary>
        /// This function is used to convert ListAgg to String_Agg
        /// </summary>
        /// <param name="listAggregation">aggregated to</param>
        /// <returns>aggregation syntax</returns>
        public static string ListAggregationAmountDimesnionLine(string listAggregation)
        {
            if (DB.IsOracle())
            {
                return listAggregation;
            }
            else if (DB.IsPostgreSQL())
            {
                return "STRING_AGG('ac.'||main.Colname,'||''_''||' order by main.ColId)";
            }
            return listAggregation;
        }

        /// <summary>Updates the loc and name on bp header.</summary>
        /// <param name="C_BPartner_ID">C_BPartner_ID.</param>
        /// <returns>
        ///   Sql query
        /// </returns>
        public static string UpdateLocAndNameOnBPHeader(int C_BPartner_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"UPDATE C_BPartner SET VA077_CustLocNo = (SELECT SUBSTR(SYS_CONNECT_BY_PATH(VA077_LocNo, ', '), 2) CSV 
                             FROM(SELECT VA077_LocNo, ROW_NUMBER() OVER(ORDER BY VA077_LocNo ASC) rn, COUNT(*) over() CNT 
                             FROM (SELECT DISTINCT VA077_LocNo FROM C_BPartner_Location WHERE C_BPartner_ID =" + C_BPartner_ID + @"))
                             WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1) , 
                             VA077_SalesRep = (SELECT SUBSTR(SYS_CONNECT_BY_PATH(Name, ', '), 2)
                             CSP FROM(SELECT Name, ROW_NUMBER() OVER(ORDER BY Name ASC) rn, COUNT(*) over() CNT 
                             FROM (SELECT us.Name FROM AD_User us JOIN C_BPartner_Location bp ON bp.AD_User_ID = us.AD_User_ID 
                             WHERE bp.C_BPartner_ID =" + C_BPartner_ID + @")) WHERE rn = cnt START WITH RN = 1 
                             CONNECT BY rn = PRIOR rn + 1) WHERE C_BPartner_ID = " + C_BPartner_ID);
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"UPDATE C_BPartner SET VA077_CustLocNo = (SELECT  string_agg(VA077_LocNo, ', ') AS Name 
                             FROM (SELECT DISTINCT VA077_LocNo FROM C_BPartner_Location WHERE C_BPartner_ID =" + C_BPartner_ID + @") AS ABC), 
                             VA077_SalesRep = (SELECT string_agg(Name, ', ') CSP FROM (SELECT us.Name FROM AD_User us 
                             JOIN C_BPartner_Location bp ON bp.AD_User_ID = us.AD_User_ID 
                             WHERE bp.C_BPartner_ID =" + C_BPartner_ID + @") AS U) WHERE C_BPartner_ID = " + C_BPartner_ID);
            }
            return sql.ToString();
        }

        /// <summary>
        /// Query for getting Table Name and ColumnName and Identifier ColumnName
        /// </summary>
        /// <param name="AD_Column_ID">Ad_Column_ID</param>
        /// <returns>query</returns>
        public static String GetTableAndColumnName(int AD_Column_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append("SELECT tab.TableName, tab.AD_Table_ID, col.ColumnName," +
                            " (select ColumnName from  " +
                              "(SELECT ColumnName , isactive from AD_Column where ad_table_id = tab.ad_table_id" +
                                  " and isidentifier = 'Y' ORDER  BY seqno)t WHERE isactive = 'Y' AND ROWNUM = 1) AS identfierColumnName " +
                            " FROM AD_Column col INNER JOIN AD_Table tab ON col.AD_Table_ID = tab.AD_Table_ID " +
                            " WHERE AD_Column_ID = " + AD_Column_ID);
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT tab.TableName, tab.AD_Table_ID, col.ColumnName," +
                               " (select ColumnName from  " +
                                 "(SELECT ColumnName , isactive from AD_Column where ad_table_id = tab.ad_table_id" +
                                     " and isidentifier = 'Y' ORDER  BY seqno)t WHERE isactive = 'Y' LIMIT 1) AS identfierColumnName " +
                               " FROM AD_Column col INNER JOIN AD_Table tab ON col.AD_Table_ID = tab.AD_Table_ID " +
                               " WHERE AD_Column_ID = " + AD_Column_ID);
            }
            return sql.ToString();
        }

        /// <summary>
        /// Query for getting contract detail to renew contract
        /// Author: 228
        /// </summary>
        /// <param name="C_Contract_ID">Old C_Contract_ID</param>
        /// <param name="AD_Client_ID">Tenent Id</param>
        /// <returns>query</returns>
        public static String GetContract(int C_Contract_ID, int AD_Client_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (C_Contract_ID > 0)
            {
                if (DB.IsPostgreSQL())
                {
                    sql.Append("SELECT * FROM C_Contract WHERE date_trunc('DAY',EndDate- make_interval(DAYS=> COALESCE(CancelBeforeDays,0))) <= date_trunc('DAY', CURRENT_DATE) AND C_Contract_ID=" + C_Contract_ID
                                            + " AND RenewContract = 'N' AND AD_Client_ID = " + AD_Client_ID);
                }
                else
                {
                    sql.Append("SELECT * FROM C_Contract WHERE (EndDate- NVL(CancelBeforeDays,0)) <= " + GlobalVariable.TO_DATE(DateTime.Now, true) + " AND C_Contract_ID=" + C_Contract_ID
                                            + " AND RenewContract = 'N' AND AD_Client_ID = " + AD_Client_ID);
                }
            }
            else
            {
                if (DB.IsPostgreSQL())
                {
                    sql.Append("SELECT * FROM C_Contract WHERE date_trunc('DAY',EndDate- make_interval(DAYS=> COALESCE(CancelBeforeDays,0))) <= date_trunc('DAY', CURRENT_DATE) AND IsActive='Y' AND RenewalType='A' AND RenewContract = 'N' AND AD_Client_ID = " + AD_Client_ID);
                }
                else
                {
                    sql.Append("SELECT * FROM C_Contract WHERE (EndDate- NVL(CancelBeforeDays,0)) <= " + GlobalVariable.TO_DATE(DateTime.Now, true) + " AND IsActive='Y' AND RenewalType='A' AND RenewContract = 'N' AND AD_Client_ID = " + AD_Client_ID);
                }
            }
            return sql.ToString();
        }

        /// <summary>
        /// convert main query to subquery 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="column">multiple columnname sperated by comma</param>
        /// <returns></returns>
        public static String convertToSubQuery(string query, string column)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT " + column + " FROM (" + query + ") AS tbl ");
            }
            else
            {
                sql.Append("SELECT " + column + " FROM (" + query + ") ");
            }
            return sql.ToString();
        }

        /// <summary>
        /// This function is used to type cast column from String to Decimal
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <returns>type cast column</returns>
        /// <writer>VIS_0045</writer>
        public static string TypecastColumnAsDecimal(string columnName)
        {
            if (DB.IsOracle())
            {
                return "CAST(" + columnName + " AS NUMBER)";
            }
            else if (DB.IsPostgreSQL())
            {
                return "Cast(" + columnName + " AS DECIMAL)";
            }
            return columnName;
        }
    }
}
