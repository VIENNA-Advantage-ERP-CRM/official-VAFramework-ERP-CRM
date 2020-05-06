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

    }
}
