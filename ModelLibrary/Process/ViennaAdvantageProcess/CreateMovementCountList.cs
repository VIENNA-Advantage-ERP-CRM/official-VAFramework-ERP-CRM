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
        private int _m_Movement_ID = 0;
        /** Physical Inventory					*/
        private MMovement _movement = null;
        /** From Locator Parameter			*/
        private int _m_Locator_ID = 0;
        /** To Locator Parameter			*/
        private int _m_LocatorTo_ID = 0;
        /** Product 			*/
        private String _m_product_ID = null;
        /** Product Category Parameter	*/
        private int _m_Product_Category_ID = 0;
        /** Qty Range Parameter			*/
        private String _qtyRange = null;
        /** Delete Parameter			*/
        private Boolean _deleteOld = false;

        /** Movement Line				*/
        private MMovementLine _line = null;

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
                else if (name.Equals("M_LocatorTo_ID"))
                    _m_LocatorTo_ID = para[i].GetParameterAsInt();
                else if (name.Equals("M_Product_ID"))
                    _m_product_ID = (String)para[i].GetParameter();
                else if (name.Equals("M_Product_Category_ID"))
                    _m_Product_Category_ID = para[i].GetParameterAsInt();
                else if (name.Equals("QtyRange"))
                    _qtyRange = (String)para[i].GetParameter();
                else if (name.Equals("DeleteOld"))
                    _deleteOld = "Y".Equals(para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _m_Movement_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("M_Movement_ID=" + _m_Movement_ID
                + ", M_Locator_ID=" + _m_Locator_ID + ", M_LocatorTo_ID=" + _m_LocatorTo_ID
                + ", M_Product_ID=" + _m_product_ID
                + ", M_Product_Category_ID=" + _m_Product_Category_ID
                + ", QtyRange=" + _qtyRange + ", DeleteOld=" + _deleteOld);
            _movement = new MMovement(GetCtx(), _m_Movement_ID, Get_Trx());
            if (_movement.Get_ID() == 0)
                throw new SystemException("Not found: M_Movement_ID=" + _m_Movement_ID);
            if (_movement.IsProcessed())
                throw new SystemException("@M_Movement_ID@ @Processed@");
            //
            String sqlQry = "";
            if (_deleteOld)
            {
                sqlQry = "DELETE FROM M_MovementLine WHERE Processed='N' "
                    + "AND M_Movement_ID=" + _m_Movement_ID;
                int no = DB.ExecuteQuery(sqlQry, null, Get_Trx());
                log.Fine("doIt - Deleted #" + no);
            }

            //	Create Null Storage records
            if (_qtyRange != null && _qtyRange.Equals("="))
            {
                sqlQry = "INSERT INTO M_Storage "
                    + "(AD_Client_ID, AD_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                    + " M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,"
                    + " qtyOnHand, QtyReserved, QtyOrdered, DateLastInventory) "
                    + "SELECT l.AD_CLIENT_ID, l.AD_ORG_ID, 'Y', SysDate, 0,SysDate, 0,"
                    + " l.M_Locator_ID, p.M_Product_ID, 0,"
                    + " 0,0,0,null "
                    + "FROM M_Locator l"
                    + " INNER JOIN M_Product p ON (l.AD_Client_ID=p.AD_Client_ID) "
                    + "WHERE l.M_Warehouse_ID=" + _movement.GetM_Warehouse_ID();
                if (_m_Locator_ID != 0)
                    sqlQry += " AND l.M_Locator_ID=" + _m_Locator_ID;
                sqlQry += " AND l.IsDefault='Y'"
                    + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'"
                    + " AND NOT EXISTS (SELECT * FROM M_Storage s"
                        + " INNER JOIN M_Locator sl ON (s.M_Locator_ID=sl.M_Locator_ID) "
                        + "WHERE sl.M_Warehouse_ID=l.M_Warehouse_ID"
                        + " AND s.M_Product_ID=p.M_Product_ID)";
                int no = DB.ExecuteQuery(sqlQry, null, Get_Trx());
                log.Fine("'0' Inserted #" + no);
            }

            StringBuilder sql = new StringBuilder(
                "SELECT s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID,"
                + " s.qtyOnHand, p.M_AttributeSet_ID "
                + "FROM M_Product p"
                + " INNER JOIN M_Storage s ON (s.M_Product_ID=p.M_Product_ID)"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) "
                + "WHERE l.M_Warehouse_ID=" + _movement.GetDTD001_MWarehouseSource_ID()
                + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            //
            if (_m_Locator_ID != 0)
                sql.Append(" AND s.M_Locator_ID=" + _m_Locator_ID);

            //
            if (_m_product_ID != null && !string.IsNullOrEmpty(_m_product_ID))
                sql.Append(" AND  p.M_Product_ID IN ( " + _m_product_ID + ") ");
            //
            if (_m_Product_Category_ID != 0)
                sql.Append(" AND p.M_Product_Category_ID=" + _m_Product_Category_ID);

            //	Do not overwrite existing records
            if (!_deleteOld)
                sql.Append(" AND NOT EXISTS (SELECT * FROM M_MovementLine il "
                + "WHERE il.M_Movement_ID=" + _m_Movement_ID
                + " AND il.M_Product_ID=s.M_Product_ID"
                + " AND il.M_Locator_ID=s.M_Locator_ID"
                + " AND COALESCE(il.M_AttributeSetInstance_ID,0)=COALESCE(s.M_AttributeSetInstance_ID,0))");
            //	+ " AND il.M_AttributeSetInstance_ID=s.M_AttributeSetInstance_ID)");
            //
            sql.Append(" ORDER BY l.Value, p.Value, s.qtyOnHand DESC");	//	Locator/Product
            //
            int count = 0;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, Get_Trx());
                while (idr.Read())
                {
                    int M_Product_ID = Util.GetValueOfInt(idr[0]);
                    int M_Locator_ID = Util.GetValueOfInt(idr[1]);
                    int M_AttributeSetInstance_ID = Util.GetValueOfInt(idr[2]);
                    Decimal qtyOnHand = Util.GetValueOfDecimal(idr[3]);
                    if (qtyOnHand == null)
                        qtyOnHand = Env.ZERO;
                    int M_AttributeSet_ID = Util.GetValueOfInt(idr[4]);
                    //
                    int compare = qtyOnHand.CompareTo(Env.ZERO);
                    if (_qtyRange == null
                        || (_qtyRange.Equals(">") && compare > 0)
                        || (_qtyRange.Equals("<") && compare < 0)
                        || (_qtyRange.Equals("=") && compare == 0)
                        || (_qtyRange.Equals("N") && compare != 0))
                    {
                        //Save data on Movement Line
                        _line = new MMovementLine(GetCtx(), 0, Get_Trx());
                        _line.SetAD_Client_ID(_movement.GetAD_Client_ID());
                        _line.SetAD_Org_ID(_movement.GetAD_Org_ID());
                        _line.SetM_Movement_ID(_m_Movement_ID);
                        _line.SetM_Locator_ID(_m_Locator_ID);
                        _line.SetM_LocatorTo_ID(_m_LocatorTo_ID);
                        _line.SetMovementQty(qtyOnHand);
                        _line.SetProcessed(false);
                        _line.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
                        _line.SetM_Product_ID(M_Product_ID);
                        if (!_line.Save())
                        {
                            log.Info("Movement Line Not Created for M_Product_ID = " + M_Product_ID + " M_AttributeSetInstance =  " + M_AttributeSetInstance_ID);
                        }
                        else
                        {
                            count = count + 1;
                        }
                    }
                }
                idr.Close();
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
            return "@M_MovementLine_ID@ - #" + count;
        }
    }
}
