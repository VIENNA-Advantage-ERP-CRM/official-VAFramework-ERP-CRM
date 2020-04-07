/********************************************************
 * Module  Name   : 
 * Purpose        : Create Inventory Count List with current Book value
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
    /// Create Inventory Count List with current Book value
    /// </summary>
    public class InventoryCountCreate : ProcessEngine.SvrProcess
    {
        /** Physical Inventory Parameter		*/
        private int _m_Inventory_ID = 0;
        /** Physical Inventory					*/
        private MInventory _inventory = null;
        /** Locator Parameter			*/
        private int _m_Locator_ID = 0;
        /** Locator Parameter			*/
        private String _locatorValue = null;
        /** Product Parameter			*/
        private String _productValue = null;
        /** Product Category Parameter	*/
        private int _m_Product_Category_ID = 0;
        /** Qty Range Parameter			*/
        private String _qtyRange = null;
        /** Update to What			*/
        private Boolean _inventoryCountSetZero = false;
        /** Delete Parameter			*/
        private Boolean _deleteOld = false;
        private Boolean _SkipBL = false;
        /** Inventory Line				*/
        private MInventoryLine _line = null;
        /** Is Container Applicable*/
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
                else if (name.Equals("M_Locator_ID"))
                    _m_Locator_ID = para[i].GetParameterAsInt();
                else if (name.Equals("LocatorValue"))
                    _locatorValue = (String)para[i].GetParameter();
                else if (name.Equals("ProductValue"))
                    _productValue = (String)para[i].GetParameter();
                else if (name.Equals("M_Product_Category_ID"))
                    _m_Product_Category_ID = para[i].GetParameterAsInt();
                else if (name.Equals("QtyRange"))
                    _qtyRange = (String)para[i].GetParameter();
                else if (name.Equals("InventoryCountSet"))
                    _inventoryCountSetZero = "Z".Equals(para[i].GetParameter());
                else if (name.Equals("DeleteOld"))
                    _deleteOld = "Y".Equals(para[i].GetParameter());
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
            // is used to check Container applicable into system
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            log.Info("M_Inventory_ID=" + _m_Inventory_ID
                + ", M_Locator_ID=" + _m_Locator_ID + ", LocatorValue=" + _locatorValue
                + ", ProductValue=" + _productValue
                + ", M_Product_Category_ID=" + _m_Product_Category_ID
                + ", QtyRange=" + _qtyRange + ", DeleteOld=" + _deleteOld);
            _inventory = new MInventory(GetCtx(), _m_Inventory_ID, Get_TrxName());
            if (_inventory.Get_ID() == 0)
                throw new SystemException("Not found: M_Inventory_ID=" + _m_Inventory_ID);
            if (_inventory.IsProcessed())
                throw new SystemException("@M_Inventory_ID@ @Processed@");
            //
            String sqlQry = "";
            StringBuilder sql = null;
            int count = 0;
            if (_deleteOld)
            {
                sqlQry = "DELETE FROM M_InventoryLine WHERE Processed='N' "
                    + "AND M_Inventory_ID=" + _m_Inventory_ID;
                int no = DataBase.DB.ExecuteQuery(sqlQry, null, Get_TrxName());
                log.Fine("doIt - Deleted #" + no);
            }

            if (!isContainerApplicable)
            {
                sql = new StringBuilder(
                @"WITH mt AS (SELECT m_product_id, M_Locator_ID, M_AttributeSetInstance_ID, SUM(CurrentQty) AS CurrentQty FROM
                (SELECT t.M_Product_ID, t.M_Locator_ID, t.M_AttributeSetInstance_ID, SUM(t.CurrentQty) keep (dense_rank last
                ORDER BY t.MovementDate, t.M_Transaction_ID) AS CurrentQty FROM m_transaction t INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID
                WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) +
                @" AND t.AD_Client_ID = " + _inventory.GetAD_Client_ID() + " AND l.AD_Org_ID = " + _inventory.GetAD_Org_ID() +
                @" AND l.M_Warehouse_ID = " + _inventory.GetM_Warehouse_ID() + @" GROUP BY t.M_Product_ID,l.M_Warehouse_ID, t.M_Locator_ID, t.M_AttributeSetInstance_ID) 
                GROUP BY m_product_id, M_Locator_ID, M_AttributeSetInstance_ID )
                SELECT s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID, mt.currentqty AS Qty, s.QtyOnHand, p.M_AttributeSet_ID FROM M_Product p 
                INNER JOIN M_Storage s ON (s.M_Product_ID=p.M_Product_ID) INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) 
                JOIN mt ON (mt.M_Product_ID = s.M_Product_ID AND mt.M_Locator_ID = s.M_Locator_ID AND mt.M_AttriButeSetInstance_ID = NVL(s.M_AttriButeSetInstance_ID,0))
                WHERE l.M_Warehouse_ID = " + _inventory.GetM_Warehouse_ID() + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            }
            else
            {
                sql = new StringBuilder(@"WITH mt AS (SELECT m_product_id, M_Locator_ID, M_AttributeSetInstance_ID, SUM(CurrentQty) AS CurrentQty, M_ProductContainer_ID
                      FROM (SELECT t.M_Product_ID, t.M_Locator_ID, t.M_AttributeSetInstance_ID, NVL(t.M_ProductContainer_ID , 0) AS M_ProductContainer_ID,
                          SUM(t.containercurrentqty) keep (dense_rank last ORDER BY t.MovementDate, t.M_Transaction_ID) AS CurrentQty
                        FROM m_transaction t INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID 
                         WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) +
                @" AND t.AD_Client_ID = " + _inventory.GetAD_Client_ID() + " AND l.AD_Org_ID = " + _inventory.GetAD_Org_ID() +
                @" AND l.M_Warehouse_ID = " + _inventory.GetM_Warehouse_ID() + @" GROUP BY t.M_Product_ID,l.M_Warehouse_ID, t.M_Locator_ID,t.M_ProductContainer_ID, t.M_AttributeSetInstance_ID) 
                GROUP BY m_product_id, M_Locator_ID, M_AttributeSetInstance_ID, M_ProductContainer_ID ) 
                SELECT DISTINCT s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID, mt.currentqty AS Qty, mt.M_ProductContainer_ID, p.M_AttributeSet_ID FROM M_Product p 
                INNER JOIN M_ContainerStorage s ON (s.M_Product_ID=p.M_Product_ID) INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) 
                JOIN mt ON (mt.M_Product_ID = s.M_Product_ID AND mt.M_Locator_ID = s.M_Locator_ID AND mt.M_AttriButeSetInstance_ID = NVL(s.M_AttriButeSetInstance_ID,0) AND mt.M_ProductContainer_ID = NVL(s.M_ProductContainer_ID , 0))
                WHERE l.M_Warehouse_ID = " + _inventory.GetM_Warehouse_ID() + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            }
            //
            if (_m_Locator_ID != 0)
                sql.Append(" AND s.M_Locator_ID=" + _m_Locator_ID);
            if (_locatorValue != null &&
                (_locatorValue.Trim().Length == 0 || _locatorValue.Equals("%")))
                _locatorValue = null;
            if (_locatorValue != null)
                sql.Append(" AND UPPER(l.Value) LIKE '" + _locatorValue.ToUpper() + "'");
            //
            if (_productValue != null &&
                (_productValue.Trim().Length == 0 || _productValue.Equals("%")))
                _productValue = null;
            if (_productValue != null)
                sql.Append(" AND UPPER(p.Value) LIKE '" + _productValue.ToUpper() + "'");

            if (_m_Product_Category_ID != 0)
                sql.Append(" AND p.M_Product_Category_ID=" + _m_Product_Category_ID);
            //	Do not overwrite existing records
            if (!_deleteOld)
            {
                sql.Append(" AND NOT EXISTS (SELECT * FROM M_InventoryLine il "
                + "WHERE il.M_Inventory_ID=" + _m_Inventory_ID
                + " AND il.M_Product_ID=s.M_Product_ID"
                + " AND il.M_Locator_ID=s.M_Locator_ID"
                + " AND COALESCE(il.M_AttributeSetInstance_ID,0)=COALESCE(s.M_AttributeSetInstance_ID,0)");
                if (!isContainerApplicable)
                {
                    sql.Append(" ) ");
                }
                else
                {
                    sql.Append(@" AND COALESCE(il.M_ProductContainer_ID,0)=COALESCE(s.M_ProductContainer_ID,0) )");
                }
            }
            //
            if (_qtyRange == "N")
            {
                sql.Append(" AND mt.currentqty != 0");
            }
            /*SI_0631 : System is giving error ORA-00920: invalid relational operator if User does not select any value from Inventory Quantity list.
                        Physical Inventory Window -- button -- Create inventory count list*/
            else if (!String.IsNullOrEmpty(_qtyRange))
            {
                sql.Append(" AND mt.currentqty " + _qtyRange + " 0");
            }

            int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_Product_ID) FROM ( " + sql.ToString() + " )", null, null));
            int pageSize = 500;
            int TotalPage = (totalRec % pageSize) == 0 ? (totalRec / pageSize) : ((totalRec / pageSize) + 1);
            StringBuilder insertSql = new StringBuilder();
            DataSet ds = null;
            try
            {
                if (totalRec > 0)
                {
                    log.Info(" =====> Physical Inventory process Started at " + DateTime.Now.ToString());
                    if (!_SkipBL)
                    {
                        for (int pageNo = 1; pageNo <= TotalPage; pageNo++)
                        {
                            ds = DB.GetDatabase().ExecuteDatasetPaging(sql.ToString(), pageNo, pageSize, 0);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
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

                                    count += CreateInventoryLine(M_Locator_ID, M_Product_ID,
                                        M_AttributeSetInstance_ID, currentQty, currentQty, M_AttributeSet_ID, container_ID);
                                }
                                ds.Dispose();
                                log.Info(" =====>  records inserted at " + DateTime.Now.ToString() + " are = " + count + " <===== ");
                            }
                        }
                    }
                    else
                    {
                        for (int pageNo = 1; pageNo <= TotalPage; pageNo++)
                        {
                            insertSql.Clear();
                            ds = DB.GetDatabase().ExecuteDatasetPaging(sql.ToString(), pageNo, pageSize, 0);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                sqlQry = "SELECT COALESCE(MAX(Line),0) AS DefaultValue FROM M_InventoryLine WHERE M_Inventory_ID=" + _m_Inventory_ID;
                                int lineNo = DB.GetSQLValue(Get_Trx(), sqlQry);
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
                                    string insertQry = InsertInventoryLine(M_Locator_ID, M_Product_ID, lineNo, M_AttributeSetInstance_ID, currentQty, M_AttributeSet_ID, container_ID);
                                    if (insertQry != "")
                                    {
                                        insertSql.Append(insertQry);
                                    }
                                }
                                ds.Dispose();
                                insertSql.Append(" END;");
                                int no = DB.ExecuteQuery(insertSql.ToString(), null, Get_Trx());
                            }
                        }
                    }

                    log.Info(" =====>  Physical Inventory process end at " + DateTime.Now.ToString());
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

            //	Set Count to Zero
            if (_inventoryCountSetZero)
            {
                String sql1 = "";
                MInventoryLine inv = new MInventoryLine(GetCtx(), 0, null);
                if (inv.Get_ColumnIndex("IsFromProcess") >= 0)
                {
                    sql1 = "UPDATE M_InventoryLine l "
                     + "SET QtyCount=0,AsOnDateCount=0 "
                     + "WHERE M_Inventory_ID=" + _m_Inventory_ID + " AND IsFromProcess = 'Y'";
                }
                else
                {
                    sql1 = "UPDATE M_InventoryLine l "
                     + "SET QtyCount=0,AsOnDateCount=0 "
                     + "WHERE M_Inventory_ID=" + _m_Inventory_ID;
                }
                int no = DataBase.DB.ExecuteQuery(sql1, null, Get_TrxName());
                log.Info("Set Cont to Zero=" + no);
            }

            return "Physical Inventory Created";
        }

        /// <summary>
        /// Create/Add to Inventory Line
        /// </summary>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="qtyOnHand">quantity</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <returns>lines added</returns>
        private int CreateInventoryLine(int M_Locator_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, Decimal qtyOnHand, Decimal currentQty, int M_AttributeSet_ID, int container_ID)
        {
            Boolean oneLinePerASI = false;
            if (M_AttributeSet_ID != 0)
            {
                MAttributeSet mas = MAttributeSet.Get(GetCtx(), M_AttributeSet_ID);
                oneLinePerASI = mas.IsInstanceAttribute();
            }
            if (oneLinePerASI)
            {
                MInventoryLine line = new MInventoryLine(_inventory, M_Locator_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    qtyOnHand, qtyOnHand);		//	book/count
                line.SetOpeningStock(currentQty);
                line.SetAsOnDateCount(line.GetQtyCount());

                // JID_0903: On creating the lines from physical inventory count system is not updating the UOM and Qty on physical inventory line.
                if (line.Get_ColumnIndex("QtyEntered") > 0)
                {
                    line.Set_Value("QtyEntered", line.GetQtyCount());
                }
                if (line.Get_ColumnIndex("C_UOM_ID") > 0)
                {
                    MProduct prd = new MProduct(GetCtx(), M_Product_ID, Get_Trx());
                    line.Set_Value("C_UOM_ID", prd.GetC_UOM_ID());
                }

                if (isContainerApplicable && line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
                {
                    line.Set_Value("M_ProductContainer_ID", container_ID);
                }
                if (line.Get_ColumnIndex("IsFromProcess") >= 0)
                {
                    line.SetIsFromProcess(true);
                }
                if (line.Save())
                    return 1;
                return 0;
            }

            if (Env.Signum(qtyOnHand) == 0)
                M_AttributeSetInstance_ID = 0;

            if (_line != null
                && _line.GetM_Locator_ID() == M_Locator_ID
                && _line.GetM_Product_ID() == M_Product_ID
                && (!isContainerApplicable || (isContainerApplicable && _line.GetM_ProductContainer_ID() == container_ID)))
            {
                if (Env.Signum(qtyOnHand) == 0)
                    return 0;
                //	Same ASI (usually 0)
                if (_line.GetM_AttributeSetInstance_ID() == M_AttributeSetInstance_ID)
                {
                    _line.SetQtyBook(Decimal.Add(_line.GetQtyBook(), qtyOnHand));
                    _line.SetQtyCount(Decimal.Add(_line.GetQtyCount(), qtyOnHand));
                    _line.SetOpeningStock((Decimal.Add(_line.GetOpeningStock(), currentQty)));
                    _line.SetAsOnDateCount(_line.GetQtyCount());

                    // JID_0903: On creating the lines from physical inventory count system is not updating the UOM and Qty on physical inventory line.
                    if (_line.Get_ColumnIndex("QtyEntered") > 0)
                    {
                        _line.Set_Value("QtyEntered", _line.GetQtyCount());
                    }
                    if (_line.Get_ColumnIndex("C_UOM_ID") > 0)
                    {
                        MProduct prd = new MProduct(GetCtx(), M_Product_ID, Get_Trx());
                        _line.Set_Value("C_UOM_ID", prd.GetC_UOM_ID());
                    }
                    if (isContainerApplicable && _line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
                    {
                        _line.Set_Value("M_ProductContainer_ID", container_ID);
                    }

                    if (_line.Get_ColumnIndex("IsFromProcess") >= 0)
                    {
                        _line.SetIsFromProcess(true);
                    }
                    _line.Save();
                    return 0;
                }
                //	Save Old Line info
                // not require to create entry here, will do it on completion
                //else if (_line.GetM_AttributeSetInstance_ID() != 0)
                //{
                //if (!isContainerApplicable)
                //{
                //    MInventoryLineMA ma = new MInventoryLineMA(_line,
                //        _line.GetM_AttributeSetInstance_ID(), _line.GetQtyBook());
                //    if (!ma.Save())
                //        ;
                //}
                //}

                _line.SetM_AttributeSetInstance_ID(0);
                _line.SetQtyBook(Decimal.Add(_line.GetQtyBook(), qtyOnHand));
                _line.SetQtyCount(Decimal.Add(_line.GetQtyCount(), qtyOnHand));
                _line.SetOpeningStock((Decimal.Add(_line.GetOpeningStock(), currentQty)));
                _line.SetAsOnDateCount(_line.GetQtyCount());

                // JID_0903: On creating the lines from physical inventory count system is not updating the UOM and Qty on physical inventory line.
                if (_line.Get_ColumnIndex("QtyEntered") > 0)
                {
                    _line.Set_Value("QtyEntered", _line.GetQtyCount());
                }
                if (_line.Get_ColumnIndex("C_UOM_ID") > 0)
                {
                    MProduct prd = new MProduct(GetCtx(), M_Product_ID, Get_Trx());
                    _line.Set_Value("C_UOM_ID", prd.GetC_UOM_ID());
                }
                if (isContainerApplicable && _line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
                {
                    _line.Set_Value("M_ProductContainer_ID", container_ID);
                }

                if (_line.Get_ColumnIndex("IsFromProcess") >= 0)
                {
                    _line.SetIsFromProcess(true);
                }
                _line.Save();

                // not require to create entry here, will do it on completion
                //if (!isContainerApplicable)
                //{
                //    MInventoryLineMA ma1 = new MInventoryLineMA(_line, M_AttributeSetInstance_ID, qtyOnHand);
                //    if (!ma1.Save())
                //        ;
                //}
                return 0;
            }
            //	new line
            _line = new MInventoryLine(_inventory, M_Locator_ID, M_Product_ID,
                M_AttributeSetInstance_ID, qtyOnHand, qtyOnHand);		//	book/count
            _line.SetOpeningStock(currentQty);
            _line.SetAsOnDateCount(_line.GetQtyCount());

            // JID_0903: On creating the lines from physical inventory count system is not updating the UOM and Qty on physical inventory line.
            if (_line.Get_ColumnIndex("QtyEntered") > 0)
            {
                _line.Set_Value("QtyEntered", _line.GetQtyCount());
            }
            if (_line.Get_ColumnIndex("C_UOM_ID") > 0)
            {
                MProduct prd = new MProduct(GetCtx(), M_Product_ID, Get_Trx());
                _line.Set_Value("C_UOM_ID", prd.GetC_UOM_ID());
            }
            if (isContainerApplicable && _line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
            {
                _line.Set_Value("M_ProductContainer_ID", container_ID);
            }

            if (_line.Get_ColumnIndex("IsFromProcess") >= 0)
            {
                _line.SetIsFromProcess(true);
            }
            if (_line.Save())
                return 1;
            return 0;
        }


        /// <summary>
        /// Create/Add to Inventory Line Query
        /// </summary>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="qtyOnHand">quantity</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <returns>lines added</returns>
        private string InsertInventoryLine(int M_Locator_ID, int M_Product_ID, int lineNo,
            int M_AttributeSetInstance_ID, Decimal qtyOnHand, int M_AttributeSet_ID, int container_ID)
        {
            MInventoryLine line = new MInventoryLine(GetCtx(), 0, Get_Trx());
            int line_ID = DB.GetNextID(GetCtx(), "M_InventoryLine", Get_Trx());
            string qry = "select m_warehouse_id from m_locator where m_locator_id=" + M_Locator_ID;
            int M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, Get_Trx()));
            MWarehouse wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
            if (wh.IsDisallowNegativeInv() == true)
            {
                if (qtyOnHand < 0)
                {
                    return "";
                }
            }
            MProduct product = MProduct.Get(GetCtx(), M_Product_ID);
            if (product != null)
            {
                int precision = product.GetUOMPrecision();
                if (Env.Signum(qtyOnHand) != 0)
                    qtyOnHand = Decimal.Round(qtyOnHand, precision, MidpointRounding.AwayFromZero);
            }
            string sql = @"INSERT INTO M_InventoryLine (AD_Client_ID, AD_Org_ID,IsActive, Created, CreatedBy, Updated, UpdatedBy, Line, M_Inventory_ID, M_InventoryLine_ID,  
                M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, QtyBook, QtyCount, OpeningStock, AsOnDateCount, DifferenceQty, AdjustmentType";

            if (line.Get_ColumnIndex("C_UOM_ID") > 0)
            {
                sql += ", QtyEntered, C_UOM_ID";
            }

            if (line.Get_ColumnIndex("IsFromProcess") > 0)
            {
                sql += ",IsFromProcess";
            }
            if (line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
            {
                sql += ",M_ProductContainer_ID ";
            }

            sql += " ) VALUES ( " + _inventory.GetAD_Client_ID() + "," + _inventory.GetAD_Org_ID() + ",'Y'," + GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," +
                GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," + lineNo + "," + _m_Inventory_ID + "," + line_ID + "," + M_Locator_ID + "," + M_Product_ID + "," +
                M_AttributeSetInstance_ID + "," + qtyOnHand + "," + qtyOnHand + "," + qtyOnHand + "," + qtyOnHand + "," + 0 + ",'A'";

            if (line.Get_ColumnIndex("C_UOM_ID") > 0)
            {
                sql += "," + qtyOnHand + "," + product.GetC_UOM_ID();
            }

            if (line.Get_ColumnIndex("IsFromProcess") > 0)
            {
                sql += ",'Y'";
            }
            if (line.Get_ColumnIndex("M_ProductContainer_ID") > 0)
            {
                sql += "," + container_ID;
            }
            string insertQry = " BEGIN execute immediate('" + sql.Replace("'", "''") + ")'); exception when others then null; END;";
            return insertQry;
        }
    }
}
