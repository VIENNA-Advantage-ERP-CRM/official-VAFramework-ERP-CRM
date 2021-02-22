/********************************************************
 * Module  Name   : 
 * Purpose        : Create Movement Count List
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Amit Bansal        06-Mar-2015
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

namespace ViennaAdvantage.Process
{
    /// <summary>
    ///Create Movement Count List
    /// </summary>
    public class CreateMovementCountList : VAdvantage.ProcessEngine.SvrProcess
    {
        /** Inventory Movement Parameter		*/
        private int _VAM_InventoryTransfer_ID = 0;
        /** Physical Inventory					*/
        private MVAMInventoryTransfer _movement = null;
        /** From Locator Parameter			*/
        private int _VAM_Locator_ID = 0;
        /** To Locator Parameter			*/
        private int _VAM_LocatorTo_ID = 0;
        /** Product 			*/
        private String _VAM_Product_ID = null;
        /** Product Category Parameter	*/
        private int _VAM_ProductCategory_ID = 0;
        /** Qty Range Parameter			*/
        private String _qtyRange = null;
        /** Delete Parameter			*/
        private Boolean _deleteOld = false;

        /** Movement Line				*/
        private MVAMInvTrfLine _line = null;
        /**is container applicable */
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
                else if (name.Equals("VAM_LocatorTo_ID"))
                    _VAM_LocatorTo_ID = para[i].GetParameterAsInt();
                else if (name.Equals("VAM_Product_ID"))
                    _VAM_Product_ID = para[i].GetParameter().ToString();
                else if (name.Equals("VAM_ProductCategory_ID"))
                    _VAM_ProductCategory_ID = para[i].GetParameterAsInt();
                else if (name.Equals("QtyRange"))
                    _qtyRange = (String)para[i].GetParameter();
                else if (name.Equals("DeleteOld"))
                    _deleteOld = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _VAM_InventoryTransfer_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("VAM_InventoryTransfer_ID=" + _VAM_InventoryTransfer_ID
                + ", VAM_Locator_ID=" + _VAM_Locator_ID + ", VAM_LocatorTo_ID=" + _VAM_LocatorTo_ID
                + ", VAM_Product_ID=" + _VAM_Product_ID
                + ", VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID
                + ", QtyRange=" + _qtyRange + ", DeleteOld=" + _deleteOld);
            _movement = new MVAMInventoryTransfer(GetCtx(), _VAM_InventoryTransfer_ID, Get_Trx());
            if (_movement.Get_ID() == 0)
                throw new SystemException("Not found: VAM_InventoryTransfer_ID=" + _VAM_InventoryTransfer_ID);
            if (_movement.IsProcessed())
                throw new SystemException("@VAM_InventoryTransfer_ID@ @Processed@");

            // is used to check Container applicable into system
            isContainerApplicable = MVAMInvTrx.ProductContainerApplicable(GetCtx());

            //
            String sqlQry = "";
            if (_deleteOld)
            {
                sqlQry = "DELETE FROM VAM_InvTrf_Line WHERE Processed='N' "
                    + "AND VAM_InventoryTransfer_ID=" + _VAM_InventoryTransfer_ID;
                int no = DB.ExecuteQuery(sqlQry, null, Get_Trx());
                log.Fine("doIt - Deleted #" + no);
            }

            //	Create Null Storage records
            if (_qtyRange != null && _qtyRange.Equals("="))
            {
                sqlQry = "INSERT INTO VAM_Storage "
                    + "(VAF_Client_ID, VAF_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                    + " VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,"
                    + " qtyOnHand, QtyReserved, QtyOrdered, DateLastInventory) "
                    + "SELECT l.VAF_CLIENT_ID, l.VAF_ORG_ID, 'Y', SysDate, 0,SysDate, 0,"
                    + " l.VAM_Locator_ID, p.VAM_Product_ID, 0,"
                    + " 0,0,0,null "
                    + "FROM VAM_Locator l"
                    + " INNER JOIN VAM_Product p ON (l.VAF_Client_ID=p.VAF_Client_ID) "
                    + "WHERE l.VAM_Warehouse_ID=" + _movement.GetVAM_Warehouse_ID();
                if (_VAM_Locator_ID != 0)
                    sqlQry += " AND l.VAM_Locator_ID=" + _VAM_Locator_ID;
                sqlQry += " AND l.IsDefault='Y'"
                    + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'"
                    + " AND NOT EXISTS (SELECT * FROM VAM_Storage s"
                        + " INNER JOIN VAM_Locator sl ON (s.VAM_Locator_ID=sl.VAM_Locator_ID) "
                        + "WHERE sl.VAM_Warehouse_ID=l.VAM_Warehouse_ID"
                        + " AND s.VAM_Product_ID=p.VAM_Product_ID)";
                int no = DB.ExecuteQuery(sqlQry, null, Get_Trx());
                log.Fine("'0' Inserted #" + no);
            }

            StringBuilder sql = null;
            if (!isContainerApplicable)
            {
                sql = new StringBuilder(
                    "SELECT s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID,"
                    + " s.qtyOnHand, p.VAM_PFeature_Set_ID, 0 AS VAM_ProductContainer_ID "
                    + "FROM VAM_Product p"
                    + " INNER JOIN VAM_Storage s ON (s.VAM_Product_ID=p.VAM_Product_ID)"
                    + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                    + "WHERE l.VAM_Warehouse_ID=" + _movement.GetDTD001_MWarehouseSource_ID()
                    + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            }
            else
            {
                sql = new StringBuilder(
                      "SELECT s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID,"
                      + " NVL(SUM(s.Qty) , 0) AS qtyOnHand , p.VAM_PFeature_Set_ID, s.VAM_ProductContainer_ID "
                      + "FROM VAM_Product p"
                      + " INNER JOIN VAM_ContainerStorage s ON (s.VAM_Product_ID=p.VAM_Product_ID)"
                      + " INNER JOIN VAM_Locator l ON (s.VAM_Locator_ID=l.VAM_Locator_ID) "
                      + "WHERE l.VAM_Warehouse_ID=" + _movement.GetDTD001_MWarehouseSource_ID()
                      + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            }
            //
            if (_VAM_Locator_ID != 0)
                sql.Append(" AND s.VAM_Locator_ID=" + _VAM_Locator_ID);

            //
            if (_VAM_Product_ID != null && !string.IsNullOrEmpty(_VAM_Product_ID))
                sql.Append(" AND  p.VAM_Product_ID IN ( " + _VAM_Product_ID + ") ");
            //
            if (_VAM_ProductCategory_ID != 0)
                sql.Append(" AND p.VAM_ProductCategory_ID=" + _VAM_ProductCategory_ID);

            //	Do not overwrite existing records
            if (!_deleteOld)
            {
                sql.Append(" AND NOT EXISTS (SELECT * FROM VAM_InvTrf_Line il "
                + "WHERE il.VAM_InventoryTransfer_ID=" + _VAM_InventoryTransfer_ID
                + " AND il.VAM_Product_ID=s.VAM_Product_ID"
                + " AND il.VAM_Locator_ID=s.VAM_Locator_ID"
                + " AND COALESCE(il.VAM_PFeature_SetInstance_ID,0)=COALESCE(s.VAM_PFeature_SetInstance_ID,0)");
                if (!isContainerApplicable)
                {
                    sql.Append(@" )  ");
                }
                else
                {
                    sql.Append(@" AND COALESCE(il.VAM_ProductContainer_ID,0)=COALESCE(s.VAM_ProductContainer_ID,0) )  ");
                }
            }
            //
            if (!isContainerApplicable)
            {
                sql.Append(" ORDER BY l.Value, p.Value, s.qtyOnHand DESC");	//	Locator/Product
            }
            else
            {
                sql.Append(@" GROUP BY s.VAM_Product_ID, s.VAM_Locator_ID, s.VAM_PFeature_SetInstance_ID, p.VAM_PFeature_Set_ID, s.VAM_ProductContainer_ID, s.Qty
ORDER BY s.VAM_Locator_ID, s.VAM_Product_ID, s.Qty DESC, s.VAM_PFeature_SetInstance_ID, p.VAM_PFeature_Set_ID,s.VAM_ProductContainer_ID");	//	Locator/Product
            }
            //
            int count = 0;
            IDataReader idr = null;
            DataTable dt = null;
            MVAMProduct product = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                   // while (idr.Read())
                {
                    int VAM_Product_ID = Util.GetValueOfInt(dr[0]);
                    product = MVAMProduct.Get(GetCtx(), VAM_Product_ID);
                    int VAM_Locator_ID = Util.GetValueOfInt(dr[1]);
                    int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(dr[2]);
                    Decimal qtyOnHand = Util.GetValueOfDecimal(dr[3]);
                    //if (qtyOnHand == null) commented by manjot Because Decimal is Never equals to Null 
                    //  qtyOnHand = Env.ZERO;
                    int VAM_PFeature_Set_ID = Util.GetValueOfInt(dr[4]);
                    //container
                    int container_Id = Util.GetValueOfInt(dr[5]);
                    //
                    int compare = qtyOnHand.CompareTo(Env.ZERO);
                    if (_qtyRange == null
                        || (_qtyRange.Equals(">") && compare > 0)
                        || (_qtyRange.Equals("<") && compare < 0)
                        || (_qtyRange.Equals("=") && compare == 0)
                        || (_qtyRange.Equals("N") && compare != 0))
                    {
                        //Save data on Movement Line
                        _line = new MVAMInvTrfLine(GetCtx(), 0, Get_Trx());
                        _line.SetVAF_Client_ID(_movement.GetVAF_Client_ID());
                        _line.SetVAF_Org_ID(_movement.GetVAF_Org_ID());
                        _line.SetVAM_InventoryTransfer_ID(_VAM_InventoryTransfer_ID);
                        _line.SetVAM_Locator_ID(_VAM_Locator_ID);
                        _line.SetVAM_LocatorTo_ID(_VAM_LocatorTo_ID);
                        _line.SetMovementQty(qtyOnHand);
                        _line.SetProcessed(false);
                        _line.SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
                        _line.SetVAM_Product_ID(VAM_Product_ID);
                        if (isContainerApplicable && _line.Get_ColumnIndex("VAM_ProductContainer_ID") > 0)
                        {
                            _line.SetVAM_ProductContainer_ID(container_Id);
                        }
                        if (_line.Get_ColumnIndex("VAB_UOM_ID") > 0 && product != null)
                        {
                            _line.SetVAB_UOM_ID(product.GetVAB_UOM_ID());
                        }
                        if (_line.Get_ColumnIndex("QtyEntered") > 0)
                        {
                            _line.SetQtyEntered(qtyOnHand);
                        }
                        if (!_line.Save())
                        {
                            return GetRetrievedError(_line, "Movement Line Not Created for VAM_Product_ID = " + VAM_Product_ID + " VAM_PFeature_SetInstance =  " + VAM_PFeature_SetInstance_ID);
                            //log.Info("Movement Line Not Created for VAM_Product_ID = " + VAM_Product_ID + " VAM_PFeature_SetInstance =  " + VAM_PFeature_SetInstance_ID);
                        }
                        else
                        {
                            count = count + 1;
                        }
                    }
                }
                //idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            //
            return "@VAM_InvTrf_Line_ID@ - #" + count;
        }
    }
}
