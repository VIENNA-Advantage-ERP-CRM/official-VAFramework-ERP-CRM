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
        private int _VAM_Inventory_ID = 0;
        /** Physical Inventory					*/
        private MInventory _inventory = null;
        /** Locator Parameter			*/
        private int _VAM_Locator_ID = 0;
        /** Locator Parameter			*/
        private String _locatorValue = null;
        /** Product Parameter			*/
        private String _productValue = null;
        /** Product Category Parameter	*/
        private int _VAM_ProductCategory_ID = 0;
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
                else if (name.Equals("VAM_Locator_ID"))
                    _VAM_Locator_ID = para[i].GetParameterAsInt();
                else if (name.Equals("LocatorValue"))
                    _locatorValue = (String)para[i].GetParameter();
                else if (name.Equals("ProductValue"))
                    _productValue = (String)para[i].GetParameter();
                else if (name.Equals("VAM_ProductCategory_ID"))
                    _VAM_ProductCategory_ID = para[i].GetParameterAsInt();
                else if (name.Equals("QtyRange"))
                    _qtyRange = (String)para[i].GetParameter();
                else if (name.Equals("InventoryCountSet"))
                    _inventoryCountSetZero = "Z".Equals(para[i].GetParameter());
                else if (name.Equals("DeleteOld"))
                    _deleteOld = "Y".Equals(para[i].GetParameter());
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
            // is used to check Container applicable into system
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            log.Info("VAM_Inventory_ID=" + _VAM_Inventory_ID
                + ", VAM_Locator_ID=" + _VAM_Locator_ID + ", LocatorValue=" + _locatorValue
                + ", ProductValue=" + _productValue
                + ", VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID
                + ", QtyRange=" + _qtyRange + ", DeleteOld=" + _deleteOld);
            _inventory = new MInventory(GetCtx(), _VAM_Inventory_ID, Get_TrxName());
            if (_inventory.Get_ID() == 0)
                throw new SystemException("Not found: VAM_Inventory_ID=" + _VAM_Inventory_ID);
            if (_inventory.IsProcessed())
                throw new SystemException("@VAM_Inventory_ID@ @Processed@");
            //
            String sqlQry = "";
            StringBuilder sql = null;
            int count = 0;
            if (_deleteOld)
            {
                sqlQry = "DELETE FROM VAM_InventoryLine WHERE Processed='N' "
                    + "AND VAM_Inventory_ID=" + _VAM_Inventory_ID;
                int no = DataBase.DB.ExecuteQuery(sqlQry, null, Get_TrxName());
                log.Fine("doIt - Deleted #" + no);
            }

            if (!isContainerApplicable)
            {
                sql = new StringBuilder(
                @"WITH mt AS (SELECT VAM_Product_id, VAM_Locator_ID, VAM_PFeature_SetInstance_ID, SUM(CurrentQty) AS CurrentQty FROM
                (SELECT DISTINCT t.VAM_Product_ID, t.VAM_Locator_ID, t.VAM_PFeature_SetInstance_ID, FIRST_VALUE(t.CurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, t.VAM_Locator_ID
                ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID
                WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) +
                @" AND t.VAF_Client_ID = " + _inventory.GetVAF_Client_ID() + " AND l.VAF_Org_ID = " + _inventory.GetVAF_Org_ID() +
                @" AND l.VAM_Warehouse_ID = " + _inventory.GetVAM_Warehouse_ID() + @") t GROUP BY VAM_Product_id, VAM_Locator_ID, VAM_PFeature_SetInstance_ID )
                SELECT DISTINCT s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID, mt.currentqty AS Qty, s.QtyOnHand, p.VAM_PFeature_Set_ID FROM VAM_Product p 
                INNER JOIN VAM_Storage s ON (s.VAM_Product_ID=p.VAM_Product_ID) INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) 
                JOIN mt ON (mt.VAM_Product_ID = s.VAM_Product_ID AND mt.VAM_Locator_ID = s.VAM_Locator_ID AND mt.VAM_PFeature_SetInstance_ID = NVL(s.VAM_PFeature_SetInstance_ID,0))
                WHERE l.VAM_Warehouse_ID = " + _inventory.GetVAM_Warehouse_ID() + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            }
            else
            {
                sql = new StringBuilder(@"WITH mt AS (SELECT VAM_Product_id, VAM_Locator_ID, VAM_PFeature_SetInstance_ID, SUM(CurrentQty) AS CurrentQty, VAM_ProductContainer_ID
                FROM (SELECT DISTINCT t.VAM_Product_ID, t.VAM_Locator_ID, t.VAM_PFeature_SetInstance_ID, NVL(t.VAM_ProductContainer_ID , 0) AS VAM_ProductContainer_ID,
                FIRST_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID, t.VAM_Locator_ID, NVL(t.VAM_ProductContainer_ID, 0) ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty
                FROM VAM_Inv_Trx t INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID 
                WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) +
                @" AND t.VAF_Client_ID = " + _inventory.GetVAF_Client_ID() + " AND l.VAF_Org_ID = " + _inventory.GetVAF_Org_ID() +
                @" AND l.VAM_Warehouse_ID = " + _inventory.GetVAM_Warehouse_ID() + @") t GROUP BY VAM_Product_id, VAM_Locator_ID, VAM_PFeature_SetInstance_ID, VAM_ProductContainer_ID ) 
                SELECT DISTINCT s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID, mt.currentqty AS Qty, mt.VAM_ProductContainer_ID, p.VAM_PFeature_Set_ID FROM VAM_Product p 
                INNER JOIN VAM_ContainerStorage s ON (s.VAM_Product_ID=p.VAM_Product_ID) INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) 
                JOIN mt ON (mt.VAM_Product_ID = s.VAM_Product_ID AND mt.VAM_Locator_ID = s.VAM_Locator_ID AND mt.VAM_PFeature_SetInstance_ID = NVL(s.VAM_PFeature_SetInstance_ID,0) AND mt.VAM_ProductContainer_ID = NVL(s.VAM_ProductContainer_ID , 0))
                WHERE l.VAM_Warehouse_ID = " + _inventory.GetVAM_Warehouse_ID() + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            }
            //
            if (_VAM_Locator_ID != 0)
                sql.Append(" AND s.VAM_Locator_ID=" + _VAM_Locator_ID);
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

            if (_VAM_ProductCategory_ID != 0)
                sql.Append(" AND p.VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID);
            //	Do not overwrite existing records
            if (!_deleteOld)
            {
                sql.Append(" AND NOT EXISTS (SELECT * FROM VAM_InventoryLine il "
                + "WHERE il.VAM_Inventory_ID=" + _VAM_Inventory_ID
                + " AND il.VAM_Product_ID=s.VAM_Product_ID"
                + " AND il.VAM_Locator_ID=s.VAM_Locator_ID"
                + " AND COALESCE(il.VAM_PFeature_SetInstance_ID,0)=COALESCE(s.VAM_PFeature_SetInstance_ID,0)");
                if (!isContainerApplicable)
                {
                    sql.Append(" ) ");
                }
                else
                {
                    sql.Append(@" AND COALESCE(il.VAM_ProductContainer_ID,0)=COALESCE(s.VAM_ProductContainer_ID,0) )");
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

            int totalRec = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAM_Product_ID) FROM ( " + sql.ToString() + " ) t", null, null));
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
                                    int VAM_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                                    int VAM_Locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                                    int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                                    int VAM_PFeature_Set_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][5]);
                                    if (isContainerApplicable)
                                    {
                                        container_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j]["VAM_ProductContainer_ID"]);
                                    }
                                    currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][3]);

                                    count += CreateInventoryLine(VAM_Locator_ID, VAM_Product_ID,
                                        VAM_PFeature_SetInstance_ID, currentQty, currentQty, VAM_PFeature_Set_ID, container_ID);
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
                            //insertSql.Clear();
                            ds = DB.GetDatabase().ExecuteDatasetPaging(sql.ToString(), pageNo, pageSize, 0);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                sqlQry = "SELECT COALESCE(MAX(Line),0) AS DefaultValue FROM VAM_InventoryLine WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID;
                                int lineNo = DB.GetSQLValue(Get_Trx(), sqlQry);
                                //insertSql.Append("BEGIN ");
                                //for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                                //{
                                //    decimal currentQty = 0;
                                //    int container_ID = 0;
                                //    int VAM_Product_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][0]);
                                //    int VAM_Locator_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][1]);
                                //    int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][2]);
                                //    int VAM_PFeature_Set_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j][5]);
                                //    if (isContainerApplicable)
                                //    {
                                //        container_ID = Util.GetValueOfInt(ds.Tables[0].Rows[j]["VAM_ProductContainer_ID"]);
                                //    }
                                //    currentQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[j][3]);
                                //    lineNo = lineNo + 10;
                                //    string insertQry = InsertInventoryLine(VAM_Locator_ID, VAM_Product_ID, lineNo, VAM_PFeature_SetInstance_ID, currentQty, VAM_PFeature_Set_ID, container_ID);
                                //    if (insertQry != "")
                                //    {
                                //        insertSql.Append(insertQry);
                                //    }
                                //}
                                //ds.Dispose();
                                //insertSql.Append(" END;");

                                string insertQry = DBFunctionCollection.InsertInventoryLine(GetCtx(), _inventory, ds, lineNo, isContainerApplicable, Get_Trx());
                                ds.Dispose();
                                int no = DB.ExecuteQuery(insertQry, null, Get_Trx());
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
                    sql1 = "UPDATE VAM_InventoryLine l "
                     + "SET QtyCount=0,AsOnDateCount=0 "
                     + "WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID + " AND IsFromProcess = 'Y'";
                }
                else
                {
                    sql1 = "UPDATE VAM_InventoryLine l "
                     + "SET QtyCount=0,AsOnDateCount=0 "
                     + "WHERE VAM_Inventory_ID=" + _VAM_Inventory_ID;
                }
                int no = DataBase.DB.ExecuteQuery(sql1, null, Get_TrxName());
                log.Info("Set Cont to Zero=" + no);
            }

            return "Physical Inventory Created";
        }

        /// <summary>
        /// Create/Add to Inventory Line
        /// </summary>
        /// <param name="VAM_Locator_ID">locator</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">asi</param>
        /// <param name="qtyOnHand">quantity</param>
        /// <param name="VAM_PFeature_Set_ID">attribute set</param>
        /// <returns>lines added</returns>
        private int CreateInventoryLine(int VAM_Locator_ID, int VAM_Product_ID,
            int VAM_PFeature_SetInstance_ID, Decimal qtyOnHand, Decimal currentQty, int VAM_PFeature_Set_ID, int container_ID)
        {
            Boolean oneLinePerASI = false;
            if (VAM_PFeature_Set_ID != 0)
            {
                MAttributeSet mas = MAttributeSet.Get(GetCtx(), VAM_PFeature_Set_ID);
                oneLinePerASI = mas.IsInstanceAttribute();
            }
            if (oneLinePerASI)
            {
                MInventoryLine line = new MInventoryLine(_inventory, VAM_Locator_ID,
                    VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    qtyOnHand, qtyOnHand);		//	book/count
                line.SetOpeningStock(currentQty);
                line.SetAsOnDateCount(line.GetQtyCount());

                // JID_0903: On creating the lines from physical inventory count system is not updating the UOM and Qty on physical inventory line.
                if (line.Get_ColumnIndex("QtyEntered") > 0)
                {
                    line.Set_Value("QtyEntered", line.GetQtyCount());
                }
                if (line.Get_ColumnIndex("VAB_UOM_ID") > 0)
                {
                    MProduct prd = new MProduct(GetCtx(), VAM_Product_ID, Get_Trx());
                    line.Set_Value("VAB_UOM_ID", prd.GetVAB_UOM_ID());
                }

                if (isContainerApplicable && line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
                {
                    line.Set_Value("VAM_ProductContainer_ID", container_ID);
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
                VAM_PFeature_SetInstance_ID = 0;

            if (_line != null
                && _line.GetVAM_Locator_ID() == VAM_Locator_ID
                && _line.GetVAM_Product_ID() == VAM_Product_ID
                && (!isContainerApplicable || (isContainerApplicable && _line.GetVAM_ProductContainer_ID() == container_ID)))
            {
                if (Env.Signum(qtyOnHand) == 0)
                    return 0;
                //	Same ASI (usually 0)
                if (_line.GetVAM_PFeature_SetInstance_ID() == VAM_PFeature_SetInstance_ID)
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
                    if (_line.Get_ColumnIndex("VAB_UOM_ID") > 0)
                    {
                        MProduct prd = new MProduct(GetCtx(), VAM_Product_ID, Get_Trx());
                        _line.Set_Value("VAB_UOM_ID", prd.GetVAB_UOM_ID());
                    }
                    if (isContainerApplicable && _line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
                    {
                        _line.Set_Value("VAM_ProductContainer_ID", container_ID);
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
                //else if (_line.GetVAM_PFeature_SetInstance_ID() != 0)
                //{
                //if (!isContainerApplicable)
                //{
                //    MInventoryLineMA ma = new MInventoryLineMA(_line,
                //        _line.GetVAM_PFeature_SetInstance_ID(), _line.GetQtyBook());
                //    if (!ma.Save())
                //        ;
                //}
                //}

                _line.SetVAM_PFeature_SetInstance_ID(0);
                _line.SetQtyBook(Decimal.Add(_line.GetQtyBook(), qtyOnHand));
                _line.SetQtyCount(Decimal.Add(_line.GetQtyCount(), qtyOnHand));
                _line.SetOpeningStock((Decimal.Add(_line.GetOpeningStock(), currentQty)));
                _line.SetAsOnDateCount(_line.GetQtyCount());

                // JID_0903: On creating the lines from physical inventory count system is not updating the UOM and Qty on physical inventory line.
                if (_line.Get_ColumnIndex("QtyEntered") > 0)
                {
                    _line.Set_Value("QtyEntered", _line.GetQtyCount());
                }
                if (_line.Get_ColumnIndex("VAB_UOM_ID") > 0)
                {
                    MProduct prd = new MProduct(GetCtx(), VAM_Product_ID, Get_Trx());
                    _line.Set_Value("VAB_UOM_ID", prd.GetVAB_UOM_ID());
                }
                if (isContainerApplicable && _line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
                {
                    _line.Set_Value("VAM_ProductContainer_ID", container_ID);
                }

                if (_line.Get_ColumnIndex("IsFromProcess") >= 0)
                {
                    _line.SetIsFromProcess(true);
                }
                _line.Save();

                // not require to create entry here, will do it on completion
                //if (!isContainerApplicable)
                //{
                //    MInventoryLineMA ma1 = new MInventoryLineMA(_line, VAM_PFeature_SetInstance_ID, qtyOnHand);
                //    if (!ma1.Save())
                //        ;
                //}
                return 0;
            }
            //	new line
            _line = new MInventoryLine(_inventory, VAM_Locator_ID, VAM_Product_ID,
                VAM_PFeature_SetInstance_ID, qtyOnHand, qtyOnHand);		//	book/count
            _line.SetOpeningStock(currentQty);
            _line.SetAsOnDateCount(_line.GetQtyCount());

            // JID_0903: On creating the lines from physical inventory count system is not updating the UOM and Qty on physical inventory line.
            if (_line.Get_ColumnIndex("QtyEntered") > 0)
            {
                _line.Set_Value("QtyEntered", _line.GetQtyCount());
            }
            if (_line.Get_ColumnIndex("VAB_UOM_ID") > 0)
            {
                MProduct prd = new MProduct(GetCtx(), VAM_Product_ID, Get_Trx());
                _line.Set_Value("VAB_UOM_ID", prd.GetVAB_UOM_ID());
            }
            if (isContainerApplicable && _line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
            {
                _line.Set_Value("VAM_ProductContainer_ID", container_ID);
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
        /// <param name="VAM_Locator_ID">locator</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">asi</param>
        /// <param name="qtyOnHand">quantity</param>
        /// <param name="VAM_PFeature_Set_ID">attribute set</param>
        /// <returns>lines added</returns>
        private string InsertInventoryLine(int VAM_Locator_ID, int VAM_Product_ID, int lineNo,
            int VAM_PFeature_SetInstance_ID, Decimal qtyOnHand, int VAM_PFeature_Set_ID, int container_ID)
        {
            MInventoryLine line = new MInventoryLine(GetCtx(), 0, Get_Trx());
            int line_ID = DB.GetNextID(GetCtx(), "VAM_InventoryLine", Get_Trx());
            string qry = "select VAM_Warehouse_id from VAM_Locator where VAM_Locator_id=" + VAM_Locator_ID;
            int VAM_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, Get_Trx()));
            MWarehouse wh = MWarehouse.Get(GetCtx(), VAM_Warehouse_ID);
            if (wh.IsDisallowNegativeInv() == true)
            {
                if (qtyOnHand < 0)
                {
                    return "";
                }
            }
            MProduct product = MProduct.Get(GetCtx(), VAM_Product_ID);
            if (product != null)
            {
                int precision = product.GetUOMPrecision();
                if (Env.Signum(qtyOnHand) != 0)
                    qtyOnHand = Decimal.Round(qtyOnHand, precision, MidpointRounding.AwayFromZero);
            }
            string sql = @"INSERT INTO VAM_InventoryLine (VAF_Client_ID, VAF_Org_ID,IsActive, Created, CreatedBy, Updated, UpdatedBy, Line, VAM_Inventory_ID, VAM_InventoryLine_ID,  
                VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, QtyBook, QtyCount, OpeningStock, AsOnDateCount, DifferenceQty, AdjustmentType";

            if (line.Get_ColumnIndex("VAB_UOM_ID") > 0)
            {
                sql += ", QtyEntered, VAB_UOM_ID";
            }

            if (line.Get_ColumnIndex("IsFromProcess") > 0)
            {
                sql += ",IsFromProcess";
            }
            if (line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
            {
                sql += ",VAM_ProductContainer_ID ";
            }

            sql += " ) VALUES ( " + _inventory.GetVAF_Client_ID() + "," + _inventory.GetVAF_Org_ID() + ",'Y'," + GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," +
                GlobalVariable.TO_DATE(DateTime.Now, true) + "," + 0 + "," + lineNo + "," + _VAM_Inventory_ID + "," + line_ID + "," + VAM_Locator_ID + "," + VAM_Product_ID + "," +
                VAM_PFeature_SetInstance_ID + "," + qtyOnHand + "," + qtyOnHand + "," + qtyOnHand + "," + qtyOnHand + "," + 0 + ",'A'";

            if (line.Get_ColumnIndex("VAB_UOM_ID") > 0)
            {
                sql += "," + qtyOnHand + "," + product.GetVAB_UOM_ID();
            }

            if (line.Get_ColumnIndex("IsFromProcess") > 0)
            {
                sql += ",'Y'";
            }
            if (line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
            {
                sql += "," + container_ID;
            }
            string insertQry = " BEGIN execute immediate('" + sql.Replace("'", "''") + ")'); exception when others then null; END;";
            return insertQry;
        }
    }
}
