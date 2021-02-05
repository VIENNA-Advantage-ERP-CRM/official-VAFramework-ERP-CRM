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
                    int VAM_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                    int VAM_Locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                    int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                    int VAM_PFeature_Set_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][5]);
                    if (isContainerApplicable)
                    {
                        container_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j]["VAM_ProductContainer_ID"]);
                    }
                    currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][3]);
                    lineNo = lineNo + 10;
                    MInventoryLine line = new MInventoryLine(ctx, 0, trx);
                    int line_ID = DB.GetNextID(ctx, "VAM_InventoryLine", trx);
                    string qry = "select VAM_Warehouse_id from VAM_Locator where VAM_Locator_id=" + VAM_Locator_ID;
                    int VAM_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, trx));
                    MWarehouse wh = MWarehouse.Get(ctx, VAM_Warehouse_ID);
                    if (wh.IsDisallowNegativeInv() == true)
                    {
                        if (currentQty < 0)
                        {
                            continue;
                        }
                    }
                    MProduct product = MProduct.Get(ctx, VAM_Product_ID);
                    if (product != null)
                    {
                        int precision = product.GetUOMPrecision();
                        if (Env.Signum(currentQty) != 0)
                            currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
                    }
                    sql.Clear();
                    sql.Append(@"INSERT INTO VAM_InventoryLine (VAF_Client_ID, VAF_Org_ID,IsActive, Created, CreatedBy, Updated, UpdatedBy, Line, VAM_Inventory_ID, VAM_InventoryLine_ID,  
                                VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, QtyBook, QtyCount, OpeningStock, AsOnDateCount, DifferenceQty, AdjustmentType");

                    if (line.Get_ColumnIndex("VAB_UOM_ID") > 0)
                    {
                        sql.Append(", QtyEntered, VAB_UOM_ID");
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",IsFromProcess");
                    }
                    if (line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
                    {
                        sql.Append(",VAM_ProductContainer_ID ");
                    }

                    sql.Append(" ) VALUES ( " + _inventory.GetVAF_Client_ID() + "," + _inventory.GetVAF_Org_ID() + ",'Y'," + GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," +
                        GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," + lineNo + "," + _inventory.Get_ID() + "," + line_ID + "," + VAM_Locator_ID + "," + VAM_Product_ID + "," +
                        VAM_PFeature_SetInstance_ID + "," + currentQty + "," + currentQty + "," + currentQty + "," + currentQty + "," + 0 + ",'A'");

                    if (line.Get_ColumnIndex("VAB_UOM_ID") > 0)
                    {
                        sql.Append("," + currentQty + "," + product.GetVAB_UOM_ID());
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",'Y'");
                    }
                    if (line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
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
                    int VAM_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                    int VAM_Locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                    int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                    int VAM_PFeature_Set_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][5]);
                    if (isContainerApplicable)
                    {
                        container_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j]["VAM_ProductContainer_ID"]);
                    }
                    currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][3]);
                    lineNo = lineNo + 10;
                    MInventoryLine line = new MInventoryLine(ctx, 0, trx);
                    int line_ID = DB.GetNextID(ctx, "VAM_InventoryLine", trx);
                    string qry = "select VAM_Warehouse_id from VAM_Locator where VAM_Locator_id=" + VAM_Locator_ID;
                    int VAM_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, trx));
                    MWarehouse wh = MWarehouse.Get(ctx, VAM_Warehouse_ID);
                    if (wh.IsDisallowNegativeInv() == true)
                    {
                        if (currentQty < 0)
                        {
                            continue;
                        }
                    }
                    MProduct product = MProduct.Get(ctx, VAM_Product_ID);
                    if (product != null)
                    {
                        int precision = product.GetUOMPrecision();
                        if (Env.Signum(currentQty) != 0)
                            currentQty = Decimal.Round(currentQty, precision, MidpointRounding.AwayFromZero);
                    }
                    sql.Clear();
                    sql.Append(@"INSERT INTO VAM_InventoryLine (VAF_Client_ID, VAF_Org_ID,IsActive, Created, CreatedBy, Updated, UpdatedBy, Line, VAM_Inventory_ID, VAM_InventoryLine_ID,  
                                VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, QtyBook, QtyCount, OpeningStock, AsOnDateCount, DifferenceQty, AdjustmentType");

                    if (line.Get_ColumnIndex("VAB_UOM_ID") > 0)
                    {
                        sql.Append(", QtyEntered, VAB_UOM_ID");
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",IsFromProcess");
                    }
                    if (line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
                    {
                        sql.Append(",VAM_ProductContainer_ID ");
                    }

                    sql.Append(" ) VALUES ( " + _inventory.GetVAF_Client_ID() + "," + _inventory.GetVAF_Org_ID() + ",'Y'," + GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," +
                        GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," + lineNo + "," + _inventory.Get_ID() + "," + line_ID + "," + VAM_Locator_ID + "," + VAM_Product_ID + "," +
                        VAM_PFeature_SetInstance_ID + "," + currentQty + "," + currentQty + "," + currentQty + "," + currentQty + "," + 0 + ",'A'");

                    if (line.Get_ColumnIndex("VAB_UOM_ID") > 0)
                    {
                        sql.Append("," + currentQty + "," + product.GetVAB_UOM_ID());
                    }

                    if (line.Get_ColumnIndex("IsFromProcess") > 0)
                    {
                        sql.Append(",'Y'");
                    }
                    if (line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
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
        /// <param name="VAM_InventoryLine_ID">Inventory Line</param>
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
                    sql.Append(@"UPDATE VAM_InventoryLine SET QtyBook = " + currentQty + ",QtyCount = " + QtyCount + ",OpeningStock = " + currentQty + ",AsOnDateCount = " + AsonDateCount +
                        ",DifferenceQty = " + DiffQty + " WHERE VAM_InventoryLine_ID = " + line_ID);
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
                    sql.Append(@"UPDATE VAM_InventoryLine SET QtyBook = " + currentQty + ",QtyCount = " + QtyCount + ",OpeningStock = " + currentQty + ",AsOnDateCount = " + AsonDateCount +
                        ",DifferenceQty = " + DiffQty + " WHERE VAM_InventoryLine_ID = " + line_ID);
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
                                 + " (SELECT DISTINCT value FROM VAM_Locator WHERE VAM_Locator_ID IN(" + locators.ToString().Trim().Trim(',') + "))) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT DISTINCT string_agg(Value, ', ') AS resultColumn FROM VAM_Locator WHERE VAM_Locator_ID IN (" + locators.ToString().Trim().Trim(',') + ")");
            }
            return sql.ToString();
        }

        public static string ConcatinateListOfProducts(string products)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append("SELECT SUBSTR(SYS_CONNECT_BY_PATH(Name, ', '), 2) CSV FROM(SELECT Name, ROW_NUMBER() OVER(ORDER BY Name) rn, COUNT(*) over() CNT FROM "
                                + " VAM_Product WHERE VAM_Product_ID IN (" + products.ToString().Trim().Trim(',') + ") ) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT DISTINCT string_agg(Name, ', ') AS resultColumn FROM VAM_Product WHERE VAM_Product_ID IN (" + products.ToString().Trim().Trim(',') + ")");
            }
            return sql.ToString();
        }

        public static string MInOutContainerNotMatched(int VAM_Inv_InOut_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '), ',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER() OVER(ORDER BY NotMatched) RN, COUNT(*) OVER() CNT  FROM
                        (SELECT DISTINCT
                        CASE  WHEN p.VAM_Warehouse_id <> i.VAM_Warehouse_id  THEN pr.Name || '_' || il.line
                              WHEN p.VAM_Locator_id <> il.VAM_Locator_id THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM VAM_Inv_InOut i INNER JOIN VAM_Inv_InOutLine il ON i.VAM_Inv_InOut_id = il.VAM_Inv_InOut_id
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID = il.VAM_ProductContainer_ID
                        WHERE il.VAM_ProductContainer_ID > 0 AND i.VAM_Inv_InOut_id = " + VAM_Inv_InOut_ID + @" AND ROWNUM <= 100)  WHERE notmatched IS NOT NULL)
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM (SELECT DISTINCT CASE WHEN p.VAM_Warehouse_id <> i.VAM_Warehouse_id  THEN pr.Name || '_' || il.line
                        WHEN p.VAM_Locator_id <> il.VAM_Locator_id THEN pr.Name || '_' || il.line END AS NotMatched 
                        FROM VAM_Inv_InOut i INNER JOIN VAM_Inv_InOutLine il ON i.VAM_Inv_InOut_id = il.VAM_Inv_InOut_id
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID = il.VAM_ProductContainer_ID
                        WHERE il.VAM_ProductContainer_ID > 0 AND i.VAM_Inv_InOut_id = " + VAM_Inv_InOut_ID + @" AND ROWNUM <= 100) t WHERE notmatched IS NOT NULL");
            }
            return sql.ToString();
        }

        /// <summary>
        /// based on Cash_ID it will check the reference Invoice Schedule's 
        /// are paid or not in the CashLines and returns those if it is paid.
        /// </summary>
        /// <param name="cash_ID">VAB_CashJRNL_ID</param>
        /// <returns>string query</returns>
        public static string CashLineRefInvScheduleDuePaidOrNot(int cash_ID)
        {
            string sql = null;
            if (DB.IsOracle())
            {
                sql = @"SELECT LTRIM(SYS_CONNECT_BY_PATH( PaidSchedule, ' , '),',') PaidSchedule FROM
                                              (SELECT PaidSchedule, ROW_NUMBER () OVER (ORDER BY PaidSchedule ) RN, COUNT (*) OVER () CNT FROM 
                                                (SELECT cs.duedate || '_' || cs.dueamt AS PaidSchedule FROM VAB_CashJRNL c 
                                                 INNER JOIN VAB_CashJRNLLine cl ON c.VAB_CashBook_id = cl.VAB_CashBook_id 
                                                 INNER JOIN VAB_sched_InvoicePayment cs ON cs.VAB_sched_InvoicePayment_ID = cl.VAB_sched_InvoicePayment_ID 
                                                 INNER JOIN VAB_Invoice inv ON inv.VAB_Invoice_ID = cl.VAB_Invoice_ID AND inv.DocStatus NOT IN ('RE' , 'VO') 
                                                 WHERE cl.CashType = 'I' AND  cl.IsActive = 'Y' AND c.IsActive = 'Y' AND cs.IsActive = 'Y' 
                                                 AND NVL(cl.VAB_Invoice_ID , 0)  <> 0 AND (NVL(cs.VAB_Payment_id,0)  != 0
                                                 OR NVL(cs.VAB_CashJRNLLine_id , 0) != 0 OR cs.VA009_IsPaid = 'Y') AND c.VAB_CashBook_id = " + cash_ID +
                                                 @" AND ROWNUM <= 100 )  )
                                                 WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1";
            }
            else if (DB.IsPostgreSQL())
            {
                sql = @"SELECT string_agg(PaidSchedule, ', ') PaidSchedule FROM
                                              (SELECT PaidSchedule, ROW_NUMBER () OVER (ORDER BY PaidSchedule ) RN, COUNT (*) OVER () CNT FROM 
                                                (SELECT cs.duedate || '_' || cs.dueamt AS PaidSchedule FROM VAB_CashJRNL c 
                                                 INNER JOIN VAB_CashJRNLLine cl ON c.VAB_CashBook_id = cl.VAB_CashBook_id 
                                                 INNER JOIN VAB_sched_InvoicePayment cs ON cs.VAB_sched_InvoicePayment_ID = cl.VAB_sched_InvoicePayment_ID 
                                                 INNER JOIN VAB_Invoice inv ON inv.VAB_Invoice_ID = cl.VAB_Invoice_ID AND inv.DocStatus NOT IN ('RE' , 'VO') 
                                                 WHERE cl.CashType = 'I' AND  cl.IsActive = 'Y' AND c.IsActive = 'Y' AND cs.IsActive = 'Y' 
                                                 AND NVL(cl.VAB_Invoice_ID , 0)  <> 0 AND (NVL(cs.VAB_Payment_id,0)  != 0
                                                 OR NVL(cs.VAB_CashJRNLLine_id , 0) != 0 OR cs.VA009_IsPaid = 'Y') AND c.VAB_CashBook_id = " + cash_ID +
                                                 @") t ORDER BY PaidSchedule LIMIT 100) tb
                                                 WHERE PaidSchedule IS NOT NULL";
            }
            return sql;
        }


        public static string MInOutContainerNotAvailable(int VAM_Inv_InOut_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( name, ' , '),',') name FROM
                          (SELECT Name , ROW_NUMBER () OVER (ORDER BY Name) RN, COUNT (*) OVER () CNT FROM
                          (SELECT (SELECT NAME FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID =IL.VAM_ProductContainer_ID ) AS Name ,
                          CASE WHEN IL.VAM_ProductContainer_ID > 0 
                          AND (SELECT DATELASTINVENTORY  FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)<=I.MOVEMENTDATE
                          THEN 1 WHEN IL.VAM_ProductContainer_ID > 0
                          AND (SELECT DATELASTINVENTORY FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)>I.MOVEMENTDATE
                          THEN 0 ELSE 1 END AS COUNT
                          FROM VAM_Inv_InOut I INNER JOIN VAM_Inv_InOutLine IL ON I.VAM_Inv_InOut_ID = IL.VAM_Inv_InOut_ID
                          WHERE I.VAM_Inv_InOut_ID =" + VAM_Inv_InOut_ID + @" AND ROWNUM <= 100)
                          WHERE COUNT = 0 )
                          WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Name FROM                           
                          (SELECT DISTINCT (SELECT NAME FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID =IL.VAM_ProductContainer_ID ) AS Name ,
                          CASE WHEN IL.VAM_ProductContainer_ID > 0 
                          AND (SELECT DATELASTINVENTORY  FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)<=I.MOVEMENTDATE
                          THEN 1 WHEN IL.VAM_ProductContainer_ID > 0
                          AND (SELECT DATELASTINVENTORY FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)>I.MOVEMENTDATE
                          THEN 0 ELSE 1 END AS COUNT
                          FROM VAM_Inv_InOut I INNER JOIN VAM_Inv_InOutLine IL ON I.VAM_Inv_InOut_ID = IL.VAM_Inv_InOut_ID
                          WHERE I.VAM_Inv_InOut_ID =" + VAM_Inv_InOut_ID + @") t WHERE COUNT = 0 AND ROWNUM <= 100");
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
                            INNER JOIN VAM_Product Prod ON prod.VAM_Product_id = shp.VAM_Product_id 
                            WHERE ( NVL(Shp.Va010_Actualvalue,0)) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.VAM_Inv_InOutLineConfirm_Id IN (" + mInOutLinesConfirm + @"))))
                            WHERE rn = cnt START WITH rn = 1 CONNECT BY Rn = Prior Rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Product FROM 
                            (SELECT DISTINCT Prod.Name AS Name FROM Va010_Shipconfparameters Shp 
                            INNER JOIN VAM_Product Prod ON prod.VAM_Product_id = shp.VAM_Product_id 
                            WHERE NVL(Shp.Va010_Actualvalue,0) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.VAM_Inv_InOutLineConfirm_Id IN (" + mInOutLinesConfirm + "))s t");
            }
            return sql.ToString();
        }

        public static string ConcatnatedListOfRequisition(string delReq)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append("SELECT SUBSTR(SYS_CONNECT_BY_PATH(Name, ', '), 2) CSV FROM(SELECT ord.DocumentNo || '_' || ol.Line AS Name, ROW_NUMBER() OVER(ORDER BY ord.DocumentNo, ol.Line) rn, COUNT(*) over() CNT FROM "
                         + " VAM_RequisitionLine ol INNER JOIN VAM_Requisition ord ON ol.VAM_Requisition_ID = ord.VAM_Requisition_ID WHERE VAM_RequisitionLine_ID IN (" + delReq.ToString().Trim().Trim(',') + ") ) WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append("SELECT DISTINCT string_agg(ord.DocumentNo || '_' || ol.Line, ', ') AS Name FROM VAM_RequisitionLine ol INNER JOIN VAM_Requisition ord ON ol.VAM_Requisition_ID = ord.VAM_Requisition_ID WHERE VAM_RequisitionLine_ID IN (" + delReq.ToString().Trim().Trim(',') + ")");
            }
            return sql.ToString();
        }

        public static string MInventoryContainerNotMatched(int VAM_Inventory_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '),',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER () OVER (ORDER BY NotMatched ) RN, COUNT (*) OVER () CNT  FROM
                        (SELECT DISTINCT 
                        CASE  WHEN p.VAM_Warehouse_id <> i.VAM_Warehouse_id  THEN pr.Name || '_' || il.line
                              WHEN p.VAM_Locator_id <> il.VAM_Locator_id THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM VAM_Inventory i INNER JOIN VAM_InventoryLine il ON i.VAM_Inventory_ID = il.VAM_Inventory_ID
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID  = il.VAM_ProductContainer_ID
                        WHERE il.VAM_ProductContainer_ID > 0 AND i.VAM_Inventory_ID = " + VAM_Inventory_ID + @" AND ROWNUM <= 100) WHERE NotMatched IS NOT NULL ) 
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM (SELECT DISTINCT 
                        CASE  WHEN p.VAM_Warehouse_id <> i.VAM_Warehouse_id  THEN pr.Name || '_' || il.line
                              WHEN p.VAM_Locator_id <> il.VAM_Locator_id THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM VAM_Inventory i INNER JOIN VAM_InventoryLine il ON i.VAM_Inventory_ID = il.VAM_Inventory_ID
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID  = il.VAM_ProductContainer_ID
                        WHERE il.VAM_ProductContainer_ID > 0 AND i.VAM_Inventory_ID = " + VAM_Inventory_ID + @" ) t WHERE NotMatched IS NOT NULL AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        public static string MInventoryContainerNotAvailable(int VAM_Inventory_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( name, ' , '),',') name FROM
                            (SELECT Name , ROW_NUMBER () OVER (ORDER BY name ) RN, COUNT (*) OVER () CNT FROM
                            (SELECT (SELECT NAME FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID =IL.VAM_ProductContainer_ID ) AS Name ,
                            CASE WHEN IL.VAM_ProductContainer_ID>0 
                            AND (SELECT DATELASTINVENTORY  FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)<=I.MOVEMENTDATE
                            THEN 1 WHEN IL.VAM_ProductContainer_ID>0
                            AND (SELECT DATELASTINVENTORY FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)>I.MOVEMENTDATE
                            THEN 0 ELSE 1 END AS COUNT
                            FROM VAM_Inventory I INNER JOIN VAM_InventoryLine IL ON I.VAM_Inventory_ID = IL.VAM_Inventory_ID
                            WHERE I.VAM_Inventory_ID =" + VAM_Inventory_ID + @" AND ROWNUM <= 100 )
                            WHERE COUNT = 0) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Name FROM 
                            (SELECT DISTINCT (SELECT NAME FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID =IL.VAM_ProductContainer_ID ) AS Name ,
                            CASE WHEN IL.VAM_ProductContainer_ID>0 
                            AND (SELECT DATELASTINVENTORY  FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)<=I.MOVEMENTDATE
                            THEN 1 WHEN IL.VAM_ProductContainer_ID>0
                            AND (SELECT DATELASTINVENTORY FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=IL.VAM_ProductContainer_ID)>I.MOVEMENTDATE
                            THEN 0 ELSE 1 END AS COUNT
                            FROM VAM_Inventory I INNER JOIN VAM_InventoryLine IL ON I.VAM_Inventory_ID = IL.VAM_Inventory_ID
                            WHERE I.VAM_Inventory_ID =" + VAM_Inventory_ID + @" ) t WHERE COUNT = 0 AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        public static string MovementContainerNotMatched(int VAM_InventoryTransfer_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '),',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER () OVER (ORDER BY NotMatched ) RN, COUNT (*) OVER () CNT  FROM
                        (SELECT DISTINCT 
                        CASE  WHEN p.VAM_Warehouse_id <> i.DTD001_MWarehouseSource_ID  THEN pr.Name || '_' || il.line
                              WHEN p.VAM_Locator_ID <> il.VAM_Locator_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM VAM_InventoryTransfer i INNER JOIN VAM_InvTrf_Line il ON i.VAM_InventoryTransfer_ID = il.VAM_InventoryTransfer_ID
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID  = il.VAM_ProductContainer_ID
                        WHERE il.VAM_ProductContainer_ID > 0 AND i.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @" AND ROWNUM <= 100 )  WHERE notmatched IS NOT NULL ) 
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM  (SELECT DISTINCT 
                        CASE  WHEN p.VAM_Warehouse_id <> i.DTD001_MWarehouseSource_ID  THEN pr.Name || '_' || il.line
                              WHEN p.VAM_Locator_ID <> il.VAM_Locator_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM VAM_InventoryTransfer i INNER JOIN VAM_InvTrf_Line il ON i.VAM_InventoryTransfer_ID = il.VAM_InventoryTransfer_ID
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID  = il.VAM_ProductContainer_ID
                        WHERE il.VAM_ProductContainer_ID > 0 AND i.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @" ) t WHERE NotMatched IS NOT NULL AND ROWNUM <= 100 ");
            }
            return sql.ToString();
        }

        public static string MovementContainerToNotMatched(int VAM_InventoryTransfer_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( NotMatched, ' , '),',') NotMatched FROM
                      (SELECT NotMatched, ROW_NUMBER () OVER (ORDER BY NotMatched ) RN, COUNT (*) OVER () CNT  FROM
                        (SELECT DISTINCT 
                        CASE  WHEN p.VAM_Warehouse_id <> i.DTD001_MWarehouseSource_ID  THEN pr.Name || '_' || il.line
                              WHEN p.VAM_Locator_ID <> il.VAM_Locator_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM VAM_InventoryTransfer i INNER JOIN VAM_InvTrf_Line il ON i.VAM_InventoryTransfer_ID = il.VAM_InventoryTransfer_ID
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID  = il.VAM_ProductContainer_ID
                        WHERE il.VAM_ProductContainer_ID > 0 AND i.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @" AND ROWNUM <= 100 )  WHERE notmatched IS NOT NULL ) 
                        WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(NotMatched, ', ') FROM (SELECT DISTINCT 
                        CASE  WHEN p.VAM_Warehouse_id <> l.VAM_Warehouse_ID  THEN pr.Name || '_' || il.line
                              WHEN p.VAM_Locator_ID <> il.VAM_LocatorTo_ID THEN pr.Name || '_' || il.line  END AS NotMatched
                        FROM VAM_InventoryTransfer i INNER JOIN VAM_InvTrf_Line il ON i.VAM_InventoryTransfer_ID = il.VAM_InventoryTransfer_ID
                        INNER JOIN VAM_Locator l ON l.VAM_Locator_ID = il.VAM_LocatorTo_ID
                        INNER JOIN VAM_Product pr ON pr.VAM_Product_id = il.VAM_Product_id
                        INNER JOIN VAM_ProductContainer p ON p.VAM_ProductContainer_ID  = il.Ref_VAM_ProductContainerTo_ID
                        WHERE il.Ref_VAM_ProductContainerTo_ID > 0 AND i.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @" ) t WHERE notmatched IS NOT NULL AND ROWNUM <= 100 ");
            }
            return sql.ToString();
        }

        public static string MovementContainerNotAvailable(int VAM_InventoryTransfer_ID)
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
                                   FROM VAM_ProductContainer
                                   WHERE VAM_ProductContainer_ID =ML.VAM_ProductContainer_ID
                                   ) AS NAME ,
                                   (SELECT NAME
                                   FROM VAM_ProductContainer
                                   WHERE VAM_ProductContainer_ID =ML.REF_VAM_ProductContainerTO_ID
                                   ) AS NAMETO ,
                                   CASE
                                     WHEN ML.VAM_ProductContainer_ID>0
                                     THEN
                                       CASE
                                         WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY'))
                                           FROM VAM_ProductContainer
                                           WHERE VAM_ProductContainer_ID=ML.VAM_ProductContainer_ID ) <=M.MOVEMENTDATE
                                         THEN 1
                                         ELSE 0
                                       END
                                     ELSE 1
                                   END AS PCFROM ,
                                   CASE
                                     WHEN ML.REF_VAM_ProductContainerTO_ID>0
                                     THEN
                                       CASE
                                         WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY'))
                                           FROM VAM_ProductContainer
                                           WHERE VAM_ProductContainer_ID=ML.REF_VAM_ProductContainerTO_ID ) <=M.MOVEMENTDATE
                                         THEN 1
                                         ELSE 0
                                       END
                                     ELSE 1
                                   END AS PCTO
                                 FROM VAM_InventoryTransfer M
                                 INNER JOIN VAM_InvTrf_Line ML
                                 ON M.VAM_InventoryTransfer_ID    =ML.VAM_InventoryTransfer_ID
                                 WHERE M.VAM_InventoryTransfer_ID =" + VAM_InventoryTransfer_ID
                                     + @" AND ROWNUM <= 100
                                 )
                               WHERE (PCFROM = 0 OR PCTO =0)
                               ) WHERE RN  = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(CASE WHEN PCFROM = 0 AND PCTO = 0 THEN TRIM(NAME) || ' ' ||TRIM(NAMETO) 
                            WHEN PCFROM=0 THEN TRIM(NAME) WHEN PCTO=0 THEN TRIM(NAMETO) ELSE NULL END, ', ') AS PCNAME 
                            FROM (SELECT (SELECT NAME FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID =ML.VAM_ProductContainer_ID ) AS NAME , 
                            (SELECT NAME FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID =ML.REF_VAM_ProductContainerTO_ID ) AS NAMETO , 
                            CASE WHEN ML.VAM_ProductContainer_ID>0 THEN CASE WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY')) 
                            FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=ML.VAM_ProductContainer_ID ) <=M.MOVEMENTDATE THEN 1 ELSE 0 END ELSE 1 END AS PCFROM, 
                            CASE WHEN ML.REF_VAM_ProductContainerTO_ID>0 THEN CASE WHEN (SELECT COALESCE(DATELASTINVENTORY,TO_DATE('01/01/1900','DD/MM/YYYY')) 
                            FROM VAM_ProductContainer WHERE VAM_ProductContainer_ID=ML.REF_VAM_ProductContainerTO_ID ) <= M.MOVEMENTDATE THEN 1 ELSE 0 END ELSE 1 END AS PCTO 
                            FROM VAM_InventoryTransfer M INNER JOIN VAM_InvTrf_Line ML ON M.VAM_InventoryTransfer_ID = ML.VAM_InventoryTransfer_ID WHERE M.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @" ) t 
                            WHERE (PCFROM = 0 OR PCTO = 0) AND ROWNUM <= 100");
            }
            return sql.ToString();
        }

        public static string CheckContainerQty(int VAM_InventoryTransfer_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( PName, ' , '),',') PName FROM 
                                               (SELECT PName, ROW_NUMBER () OVER (ORDER BY PName ) RN, COUNT (*) OVER () CNT FROM 
                                               (
                                                SELECT p.Name || '_' || asi.description || '_' || ml.line  AS PName 
                                                FROM VAM_InvTrf_Line ml INNER JOIN VAM_InventoryTransfer m ON m.VAM_InventoryTransfer_id = ml.VAM_InventoryTransfer_id
                                                INNER JOIN VAM_Product p ON p.VAM_Product_id = ml.VAM_Product_id
                                                LEFT JOIN VAM_PFeature_SetInstance asi ON NVL(asi.VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID,0)
                                                 WHERE ml.MoveFullContainer ='Y' AND m.VAM_InventoryTransfer_ID =" + VAM_InventoryTransfer_ID + @"
                                                    AND abs(ml.movementqty) <>
                                                     NVL((SELECT SUM(t.ContainerCurrentQty) keep (dense_rank last ORDER BY t.MovementDate, t.VAM_Inv_Trx_ID) AS CurrentQty
                                                     FROM VAM_Inv_Trx t INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID
                                                      WHERE t.MovementDate <= (Select MAX(movementdate) from VAM_Inv_Trx where 
                                                            VAF_Client_ID = m.VAF_Client_ID  AND VAM_Locator_ID = ml.VAM_LocatorTo_ID
                                                            AND VAM_Product_ID = ml.VAM_Product_ID AND NVL(VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID,0)
                                                            AND NVL(VAM_ProductContainer_ID, 0) = NVL(ml.VAM_ProductContainer_ID, 0) )
                                                       AND t.VAF_Client_ID                     = m.VAF_Client_ID
                                                       AND t.VAM_Locator_ID                     = ml.VAM_LocatorTo_ID
                                                       AND t.VAM_Product_ID                     = ml.VAM_Product_ID
                                                       AND NVL(t.VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID,0)
                                                       AND NVL(t.VAM_ProductContainer_ID, 0)    = NVL(ml.VAM_ProductContainer_ID, 0)  ), 0) 
                                                       AND ROWNUM <= 100 )
                                               ) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ");
            }

            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(PName, ', ') FROM 
                        (SELECT p.Name || '_' || asi.description || '_' || ml.line  AS PName 
                        FROM VAM_InvTrf_Line ml INNER JOIN VAM_InventoryTransfer m ON m.VAM_InventoryTransfer_id = ml.VAM_InventoryTransfer_id
                        INNER JOIN VAM_Product p ON p.VAM_Product_id = ml.VAM_Product_id
                        LEFT JOIN VAM_PFeature_SetInstance asi ON NVL(asi.VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID,0)
                        WHERE ml.MoveFullContainer ='Y' AND m.VAM_InventoryTransfer_ID =" + VAM_InventoryTransfer_ID + @"
                        AND abs(ml.movementqty) <>
                        NVL((SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, 
                        t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty
                        FROM VAM_Inv_Trx t INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID
                        WHERE t.MovementDate <= (Select MAX(movementdate) from VAM_Inv_Trx where 
                        VAF_Client_ID = m.VAF_Client_ID  AND VAM_Locator_ID = ml.VAM_LocatorTo_ID
                        AND VAM_Product_ID = ml.VAM_Product_ID AND NVL(VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID,0)
                        AND NVL(VAM_ProductContainer_ID, 0) = NVL(ml.VAM_ProductContainer_ID, 0) )
                        AND t.VAF_Client_ID                     = m.VAF_Client_ID
                        AND t.VAM_Locator_ID                     = ml.VAM_LocatorTo_ID
                        AND t.VAM_Product_ID                     = ml.VAM_Product_ID
                        AND NVL(t.VAM_PFeature_SetInstance_ID,0) = NVL(ml.VAM_PFeature_SetInstance_ID,0)
                        AND NVL(t.VAM_ProductContainer_ID, 0)    = NVL(ml.VAM_ProductContainer_ID, 0)), 0)
                        ) final_ WHERE PName is not null AND ROWNUM <= 100 ");
            }
            return sql.ToString();
        }

        public static string CheckMoveContainer(int VAM_InventoryTransfer_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"SELECT LTRIM(SYS_CONNECT_BY_PATH( PName, ' , '),',') PName FROM
                             (SELECT PName, ROW_NUMBER() OVER(ORDER BY PName) RN, COUNT(*) OVER() CNT FROM(
                               SELECT ttt.PName, tt.CurrentQty, ttt.CurrentQty
                                  FROM(SELECT '' AS PName, VAM_InvTrf_Line_id, (ContainerCurrentQty)CurrentQty FROM(SELECT ml.VAM_InvTrf_Line_id,
                                  t.ContainerCurrentQty, t.MovementDate, t.VAM_Inv_Trx_ID,
                                  row_number() OVER(PARTITION BY ml.VAM_InvTrf_Line_id ORDER BY t.VAM_Inv_Trx_ID DESC, t.MovementDate DESC) RN_
                                FROM VAM_Inv_Trx T INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID
                                INNER JOIN VAM_InvTrf_Line ml ON(t.VAF_Client_ID = ml.VAF_Client_ID
                                AND t.VAM_Locator_ID = ml.VAM_Locator_ID
                                AND t.VAM_Product_ID = ml.VAM_Product_ID
                                AND NVL(t.VAM_PFeature_SetInstance_ID, 0) = NVL(ml.VAM_PFeature_SetInstance_ID, 0)
                                AND NVL(t.VAM_ProductContainer_ID, 0) = NVL(ml.VAM_ProductContainer_ID, 0))
                                WHERE ml.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @") WHERE RN_ = 1) tt
                             INNER JOIN
                              (SELECT p.Name || '_' || asi.description || '_' || ml.line AS PName, ml.VAM_InvTrf_Line_id, ml.movementqty AS CurrentQty
                               FROM VAM_InvTrf_Line ml INNER JOIN VAM_InventoryTransfer m  ON m.VAM_InventoryTransfer_id = ml.VAM_InventoryTransfer_id
                               INNER JOIN VAM_Product p ON p.VAM_Product_id = ml.VAM_Product_id
                               LEFT JOIN VAM_PFeature_SetInstance asi ON NVL(asi.VAM_PFeature_SetInstance_ID, 0) = NVL(ml.VAM_PFeature_SetInstance_ID, 0)
                               WHERE ml.MoveFullContainer = 'Y'
                               AND m.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @") ttt ON tt.VAM_InvTrf_Line_id = ttt.VAM_InvTrf_Line_id
                               WHERE tt.CurrentQty <> ttt.CurrentQty AND ROWNUM <= 100))
                           WHERE RN = CNT START WITH RN = 1   CONNECT BY RN = PRIOR RN + 1 ");
            }

            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(PName, ', ') FROM
                                (SELECT ttt.PName , tt.CurrentQty, ttt.CurrentQty 
                                FROM (SELECT '' AS PName, VAM_InvTrf_Line_id, (ContainerCurrentQty) CurrentQty FROM (SELECT ml.VAM_InvTrf_Line_id,
                                t.ContainerCurrentQty, t.MovementDate,  t.VAM_Inv_Trx_ID,
                                row_number() OVER (PARTITION BY ml.VAM_InvTrf_Line_id ORDER BY t.VAM_Inv_Trx_ID DESC, t.MovementDate DESC) RN_
                                FROM VAM_Inv_Trx T INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID
                                INNER JOIN VAM_InvTrf_Line ml ON ( t.VAF_Client_ID = ml.VAF_Client_ID
                                AND t.VAM_Locator_ID = ml.VAM_Locator_ID
                                AND t.VAM_Product_ID = ml.VAM_Product_ID
                                AND NVL (t.VAM_PFeature_SetInstance_ID, 0) = NVL (ml.VAM_PFeature_SetInstance_ID, 0)
                                AND NVL (t.VAM_ProductContainer_ID, 0) = NVL (ml.VAM_ProductContainer_ID, 0))
                                WHERE ml.VAM_InventoryTransfer_ID = " + VAM_InventoryTransfer_ID + @" ) tt_ WHERE RN_ = 1 ) tt
                                INNER JOIN 
                                (SELECT p.Name || '_' || asi.description || '_'  || ml.line AS PName, ml.VAM_InvTrf_Line_id ,  ml.movementqty AS CurrentQty
                                FROM VAM_InvTrf_Line ml INNER JOIN VAM_InventoryTransfer m  ON m.VAM_InventoryTransfer_id = ml.VAM_InventoryTransfer_id
                                INNER JOIN VAM_Product p ON p.VAM_Product_id = ml.VAM_Product_id
                                LEFT JOIN VAM_PFeature_SetInstance asi ON NVL (asi.VAM_PFeature_SetInstance_ID, 0) = NVL (ml.VAM_PFeature_SetInstance_ID, 0)
                                WHERE ml.MoveFullContainer = 'Y'
                                AND m.VAM_InventoryTransfer_ID   = " + VAM_InventoryTransfer_ID + @" ) ttt ON tt.VAM_InvTrf_Line_id = ttt.VAM_InvTrf_Line_id
                                WHERE tt.CurrentQty <> ttt.CurrentQty) final_ WHERE PName IS NOT NULL AND ROWNUM <= 100");
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
                            INNER JOIN VAM_Product Prod ON prod.VAM_Product_id = shp.VAM_Product_id 
                            WHERE ( NVL(Shp.Va010_Actualvalue,0)) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.VAM_InvTrf_LineConfirm_ID IN (" + mMovementLinesConfirm + @"))))
                            WHERE rn = cnt START WITH rn = 1 CONNECT BY Rn = Prior Rn + 1");
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"SELECT string_agg(Name, ', ') AS Product FROM 
                            (SELECT DISTINCT Prod.Name AS Name FROM VA010_MoveConfParameters Shp 
                            INNER JOIN VAM_Product Prod ON prod.VAM_Product_id = shp.VAM_Product_id 
                            WHERE NVL(Shp.Va010_Actualvalue,0) = 0 AND Shp.Isactive = 'Y' 
                            AND Shp.VAM_InvTrf_LineConfirm_ID IN (" + mMovementLinesConfirm + "))s t");
            }
            return sql.ToString();
        }

        /// <summary>
        /// Return Regulax expression to used to sql for gettng obscure data
        /// </summary>
        /// <param name="obscureType"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns>SQL</returns>
        public static string GetObscureColumn(string obscureType, string tableName, string columnName)
        {
            if (DB.IsOracle())
            {
                if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButLast4))
                {
                    return " REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-4) ,'[[:digit:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3) ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButFirstLast4))
                {
                    return "SUBSTR(" + tableName + "." + columnName + ",0,4) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButLast4))
                {
                    return " REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-4) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3) ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButFirstLast4))
                {
                    return "SUBSTR(" + tableName + "." + columnName + ",0,4) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)";
                }
            }
            else if (DB.IsPostgreSQL())
            {
                if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 4 then REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-3) ,'[[:digit:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3) when LENGTH(" + tableName + "." + columnName + ")<=4 then " + tableName + "." + columnName + " END ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureDigitsButFirstLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 8 then SUBSTR(" + tableName + "." + columnName + ",0,5) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)  when LENGTH(" + tableName + "." + columnName + ")<=8 then " + tableName + "." + columnName + " END ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 4 then REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",0,LENGTH(" + tableName + "." + columnName + ")-3) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)  when LENGTH(" + tableName + "." + columnName + ")<=4 then " + tableName + "." + columnName + " END ";
                }
                else if (obscureType.Equals(X_VAF_Column.OBSCURETYPE_ObscureAlphaNumericButFirstLast4))
                {
                    return " Case when LENGTH(" + tableName + "." + columnName + ") > 8 then SUBSTR(" + tableName + "." + columnName + ",0,5) || REGEXP_REPLACE(SUBSTR(" + tableName + "." + columnName + ",4,LENGTH(" + tableName + "." + columnName + ")-8) ,'[[:digit:]]|[[:alpha:]]|[^A-Z0-9 ]|[[:space:]]','*','g') || SUBSTR(" + tableName + "." + columnName + ",LENGTH(" + tableName + "." + columnName + ")-3)  when LENGTH(" + tableName + "." + columnName + ")<=8 then " + tableName + "." + columnName + " END ";
                }

            }

            return tableName + "." + columnName;
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
        /// <param name="VAB_BusinessPartner_ID">VAB_BusinessPartner_ID.</param>
        /// <returns>
        ///   Sql query
        /// </returns>
        public static string UpdateLocAndNameOnBPHeader(int VAB_BusinessPartner_ID)
        {
            StringBuilder sql = new StringBuilder();
            if (DB.IsOracle())
            {
                sql.Append(@"UPDATE VAB_BusinessPartner SET VA077_CustLocNo = (SELECT SUBSTR(SYS_CONNECT_BY_PATH(VA077_LocNo, ', '), 2) CSV 
                             FROM(SELECT VA077_LocNo, ROW_NUMBER() OVER(ORDER BY VA077_LocNo ASC) rn, COUNT(*) over() CNT 
                             FROM (SELECT DISTINCT VA077_LocNo FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + VAB_BusinessPartner_ID + @"))
                             WHERE rn = cnt START WITH RN = 1 CONNECT BY rn = PRIOR rn + 1) , 
                             VA077_SalesRep = (SELECT SUBSTR(SYS_CONNECT_BY_PATH(Name, ', '), 2)
                             CSP FROM(SELECT Name, ROW_NUMBER() OVER(ORDER BY Name ASC) rn, COUNT(*) over() CNT 
                             FROM (SELECT us.Name FROM VAF_UserContact us JOIN VAB_BPart_Location bp ON bp.VAF_UserContact_ID = us.VAF_UserContact_ID 
                             WHERE bp.VAB_BusinessPartner_ID =" + VAB_BusinessPartner_ID + @")) WHERE rn = cnt START WITH RN = 1 
                             CONNECT BY rn = PRIOR rn + 1) WHERE VAB_BusinessPartner_ID = " + VAB_BusinessPartner_ID);
            }
            else if (DB.IsPostgreSQL())
            {
                sql.Append(@"UPDATE VAB_BusinessPartner SET VA077_CustLocNo = (SELECT  string_agg(VA077_LocNo, ', ') AS Name 
                             FROM (SELECT DISTINCT VA077_LocNo FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + VAB_BusinessPartner_ID + @") AS ABC), 
                             VA077_SalesRep = (SELECT string_agg(Name, ', ') CSP FROM (SELECT us.Name FROM VAF_UserContact us 
                             JOIN VAB_BPart_Location bp ON bp.VAF_UserContact_ID = us.VAF_UserContact_ID 
                             WHERE bp.VAB_BusinessPartner_ID =" + VAB_BusinessPartner_ID + @") AS U) WHERE VAB_BusinessPartner_ID = " + VAB_BusinessPartner_ID);
            }
            return sql.ToString();
        }
    }
}
