/********************************************************
 * Module Name    : VIS
 * Purpose        : Model class for Pallet / Product Container Functionality
 * Class Used     : 
 * Chronological Development
 * Amit Bansal     26 June 2019
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class ProductContainerModel
    {
        Ctx _ctx = null;
        static VLogger _log = VLogger.GetVLogger("ProductContainerModel");

        public ProductContainerModel(Ctx ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        ///  use to Get Product Container on the basis of given paameter
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="WarehouseId"></param>
        /// <param name="LocatorId"></param>
        /// <returns></returns>
        public List<PContainer> ProductContainer(string Name, int WarehouseId, int LocatorId)
        {
            List<PContainer> listOverHead = new List<PContainer>();
            string sqlQry = @"SELECT M_ProductContainer_ID,
                                     Name, 
                                     Ref_M_Container_ID ,
                                     (SELECT NAME FROM M_ProductContainer ipc WHERE ipc.M_ProductContainer_ID=opc.Ref_M_Container_ID) AS ParentPath ,
                                     Width, 
                                     Height 
                              FROM M_ProductContainer opc WHERE IsActive='Y' AND M_Warehouse_ID=" + WarehouseId +
                              @" AND M_Locator_ID=" + LocatorId;
            if (!String.IsNullOrEmpty(Name))
            {
                sqlQry += "  AND UPPER(Name) LIKE UPPER('%" + Name + "%')";
            }
            DataSet ds = DB.ExecuteDataset(sqlQry, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PContainer objContainer = new PContainer();
                    objContainer.ContainerName = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    objContainer.ParentPath = Util.GetValueOfString(ds.Tables[0].Rows[i]["ParentPath"]);
                    objContainer.Width = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Width"]);
                    objContainer.Height = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Height"]);
                    objContainer.M_ProductContainer_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductContainer_ID"]);
                    objContainer.Ref_M_Container_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["Ref_M_Container_ID"]);
                    listOverHead.Add(objContainer);
                }
            }
            return listOverHead;
        }

        /// <summary>
        /// Update Container reference on the basis of selected parameter
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="RecordId"></param>
        /// <param name="ContainerId"></param>
        /// <returns></returns>
        public bool UpdateProductContainer(string TableName, int RecordId, int ContainerId)
        {
            int no = DB.ExecuteQuery("UPDATE " + TableName.Substring(0, TableName.Length - 3) + " SET M_ProductContainer_ID = " + ContainerId + " WHERE " + TableName + " = " + RecordId, null, null);
            if (no > 0)
                return true;
            else
                return false;
        }

        /**************************** Move Container Form*************************************/

        /// <summary>
        /// Get Warehouses
        /// </summary>
        /// <returns></returns>
        public List<MoveKeyVal> GetWarehouse(int Warehouse_ID)
        {
            List<MoveKeyVal> keyVal = new List<MoveKeyVal>();
            string sql = "SELECT M_Warehouse_ID,Name FROM M_Warehouse WHERE IsActive = 'Y'";
            if (Warehouse_ID > 0)
            {
                sql += " AND M_Warehouse_ID = " + Warehouse_ID;
            }
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_Warehouse", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO); // fully qualidfied - RO

            sql += "order by Name asc";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    keyVal.Add(new MoveKeyVal() { ID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_Warehouse_ID"]), Name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]) });
                }
                ds.Dispose();
            }
            return keyVal;
        }

        /// <summary>
        /// Get Locators based on warehouse
        /// </summary>
        /// <param name="fromWarehouse_ID"></param>
        /// <returns></returns>
        public List<MoveKeyVal> GetLocator(int Warehouse_ID)
        {
            List<MoveKeyVal> keyVal = new List<MoveKeyVal>();
            string sql = "SELECT M_Locator_ID,Value FROM M_Locator WHERE IsActive = 'Y'";
            if (Warehouse_ID > 0)
            {
                sql += " AND M_Warehouse_ID = " + Warehouse_ID;
            }
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_Locator", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO); // fully qualidfied - RO
            sql += " order by value asc";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    keyVal.Add(new MoveKeyVal() { ID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_Locator_ID"]), Name = Convert.ToString(ds.Tables[0].Rows[i]["Value"]) });
                }
                ds.Dispose();
            }
            return keyVal;
        }

        /// <summary>
        /// used to load product container
        /// </summary>
        /// <param name="warehouse"></param>
        /// <param name="locator"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public List<MoveKeyVal> GetContainer(int warehouse, int locator, int container)
        {
            List<MoveKeyVal> keyVal = new List<MoveKeyVal>();
            string sql = "SELECT M_ProductContainer_ID,Name || '_' || Value AS Value FROM M_ProductContainer WHERE IsActive = 'Y' AND M_Warehouse_ID = " + warehouse;
            if (locator > 0)
            {
                sql += "  AND M_Locator_ID = " + locator;
            }
            if (container > 0)
            {
                sql += " AND M_ProductContainer_ID != " + container;
            }
            sql += " ORDER BY Value";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_ProductContainer", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO); // fully qualidfied - RO
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    keyVal.Add(new MoveKeyVal()
                    {
                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_ProductContainer_ID"]),
                        Name = Convert.ToString(ds.Tables[0].Rows[i]["Value"])
                    });
                }
                ds.Dispose();
            }
            return keyVal;
        }

        /// <summary>
        /// Get Product Container in term of Tree Structure 
        /// </summary>
        /// <param name="warehouse">Warehouse -- which warehouse container we have to show</param>
        /// <param name="locator">Locato - which locator container we have to show</param>
        /// <param name="container">not used this parameter functionality -- Tree structure mismatched</param>
        /// <param name="validation"></param>
        /// <returns></returns>
        public List<TreeContainer> GetContainerAsTree(int warehouse, int locator, int container, string validation)
        {
            if (warehouse == 0 && locator > 0)
            {
                MLocator m_locator = MLocator.Get(_ctx, locator);
                warehouse = m_locator.GetM_Warehouse_ID();
            }
            List<TreeContainer> keyVal = new List<TreeContainer>();
            string sql = "";
            DataSet ds = null;
            if (DatabaseType.IsOracle)
            {
                sql = @"SELECT value ,   '.'   || LPAD (' ', LEVEL * 1)   || Name AS Name,
                            LEVEL ,   name   || '_'   || value AS ContainerName , m_productcontainer_id
                         FROM m_productcontainer WHERE IsActive = 'Y'";
                if (warehouse > 0)
                {
                    sql += " AND m_warehouse_id = " + warehouse;
                }
                if (locator > 0)
                {
                    sql += "  AND M_Locator_ID = " + locator;
                }
                if (container > 0)
                {
                    sql += " AND M_ProductContainer_ID != " + container;
                }
                if (!String.IsNullOrEmpty(validation))
                {
                    sql += " AND " + validation;
                }
                sql += "  START WITH NVL(ref_m_container_id,0) =0  CONNECT BY NVL(ref_m_container_id,0) = PRIOR m_productcontainer_id";
                sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_ProductContainer", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO); // fully qualidfied - RO
                ds = DB.ExecuteDataset(sql);
            }
            else if (DatabaseType.IsPostgre)
            {
                ds = GetContainerAsTreeForPostgre(warehouse, locator, container, validation);

            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    keyVal.Add(new TreeContainer()
                    {
                        Value = Util.GetValueOfString(ds.Tables[0].Rows[i]["value"]),
                        Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]),
                        Level = Util.GetValueOfInt(ds.Tables[0].Rows[i]["LEVEL"]),
                        ContainerName = Util.GetValueOfString(ds.Tables[0].Rows[i]["ContainerName"]),
                        M_ProductContainer_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductContainer_ID"])
                    });
                }
                ds.Dispose();
            }
            return keyVal;
        }

        /// <summary>
        /// Get Product Container as tree in Postgre
        /// Enable Extenstion as tablefunc
        /// </summary>
        /// <param name="warehouse">warehouse id</param>
        /// <param name="locator">locator id</param>
        /// <param name="container">container id</param>
        /// <param name="validation">validation</param>
        /// <returns>Product Container Tree</returns>
        private DataSet GetContainerAsTreeForPostgre(int warehouse, int locator, int container, string validation)
        {
            DataSet finalContainer = null;
            // Get List of parent Container
            String sql = "SELECT M_ProductContainer_ID FROM M_ProductContainer WHERE Ref_M_Container_ID IS NULL AND IsActive = 'Y' ";
            if (warehouse > 0)
            {
                sql += "  AND m_warehouse_id = " + warehouse;
            }
            if (locator > 0)
            {
                sql += "  AND M_Locator_ID = " + locator;
            }
            if (container > 0)
            {
                sql += " AND M_ProductContainer_ID != " + container;
            }
            if (!String.IsNullOrEmpty(validation))
            {
                sql += " AND " + validation;
            }
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // loop parent Container, 
                DataSet childContainer = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int parentId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductContainer_ID"]);
                    sql = @"select (select tt.value from m_productcontainer tt where tt.M_ProductContainer_ID = t.M_ProductContainer_ID) as value,
                                   (select tt.name from m_productcontainer tt where tt.M_ProductContainer_ID = t.M_ProductContainer_ID) as Name , 
                                   t.LEVEL + 1 AS LEVEL, 
                                   (select tt.name   || '_'   || tt.value from m_productcontainer tt where tt.M_ProductContainer_ID = t.M_ProductContainer_ID) as ContainerName
                                  , t.M_ProductContainer_ID
                            FROM connectby('M_ProductContainer', 'M_ProductContainer_ID', 'Ref_M_Container_ID', " + parentId + @"::text,0)
                            AS t(M_ProductContainer_ID int , Ref_M_Container_ID int, level int)  
                             JOIN M_ProductContainer pt on pt.M_ProductContainer_ID =" + parentId;
                    childContainer = DB.ExecuteDataset(sql, null, null);
                    if (childContainer != null && childContainer.Tables.Count > 0 && childContainer.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < childContainer.Tables[0].Rows.Count; j++)
                        {
                            if (finalContainer == null)
                            {
                                finalContainer = childContainer.Copy();
                                break;
                            }
                            else
                            {
                                DataRow dr = childContainer.Tables[0].Rows[j];
                                finalContainer.Tables[0].ImportRow(dr);
                                finalContainer.AcceptChanges();
                            }
                        }
                    }
                }
            }
            if (finalContainer == null)
            {
                // Copy or clone of Parent container 
                finalContainer = ds.Copy();
            }
            return finalContainer;
        }

        /// <summary>
        /// Get Product from Transaction (Container Wise)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="movementDate"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="locator"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<MoveContainer> GetProductContainerFromTransaction(int container, DateTime? movementDate, int AD_Org_ID, int locator, int page, int size)
        {
            int countRecord = 0;
            List<MoveContainer> moveContainer = new List<MoveContainer>();

            string sql = @"SELECT * FROM (
                            SELECT DISTINCT p.M_PRODUCT_ID, p.NAME, p.C_UOM_ID, u.Name AS UomName,  t.M_ATTRIBUTESETINSTANCE_ID, t.M_ProductContainer_ID,
                             First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS ContainerCurrentQty, 
                            (SELECT DESCRIPTION FROM M_ATTRIBUTESETINSTANCE WHERE NVL(M_ATTRIBUTESETINSTANCE_ID, 0) = t.M_ATTRIBUTESETINSTANCE_ID) AS ASI
                            FROM M_Transaction t
                            INNER JOIN M_Product p ON p.M_Product_ID = t.M_Product_ID
                            INNER JOIN C_UOM u ON u.C_UOM_ID = p.C_UOM_ID
                            WHERE t.IsActive = 'Y' AND NVL(t.M_ProductContainer_ID, 0) = " + container +
                            @" AND t.MovementDate <=" + GlobalVariable.TO_DATE(movementDate, true) + @" 
                               AND t.M_Locator_ID  = " + locator + @"
                               AND t.AD_Client_ID  = " + _ctx.GetAD_Client_ID() +
                          //AND t.AD_Org_ID  = " + AD_Org_ID +
                          //@" GROUP BY p.M_PRODUCT_ID, p.NAME, p.C_UOM_ID, u.Name, t.M_ATTRIBUTESETINSTANCE_ID, t.M_ProductContainer_ID 
                          @" )t WHERE ContainerCurrentQty <> 0 ";


            // count record for paging
            if (page == 1)
            {
                string sql1 = @"SELECT COUNT(*) FROM (
                            SELECT DISTINCT p.M_PRODUCT_ID, p.NAME, p.C_UOM_ID, u.Name AS UomName,  t.M_ATTRIBUTESETINSTANCE_ID, t.M_ProductContainer_ID,
                            First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS ContainerCurrentQty,  
                            (SELECT DESCRIPTION FROM M_ATTRIBUTESETINSTANCE WHERE NVL(M_ATTRIBUTESETINSTANCE_ID, 0) = t.M_ATTRIBUTESETINSTANCE_ID) AS ASI
                            FROM M_Transaction t
                            INNER JOIN M_Product p ON p.M_Product_ID = t.M_Product_ID
                            INNER JOIN C_UOM u ON u.C_UOM_ID = p.C_UOM_ID
                            WHERE t.IsActive = 'Y' AND NVL(t.M_ProductContainer_ID, 0) = " + container +
                            @" AND t.MovementDate <=" + GlobalVariable.TO_DATE(movementDate, true) + @" 
                               AND t.M_Locator_ID  = " + locator + @"
                               AND t.AD_Client_ID  = " + _ctx.GetAD_Client_ID() +
                          //AND t.AD_Org_ID  = " + AD_Org_ID +
                          // @" GROUP BY p.M_PRODUCT_ID, p.NAME, p.C_UOM_ID, u.Name, t.M_ATTRIBUTESETINSTANCE_ID, t.M_ProductContainer_ID 
                          @" )t WHERE ContainerCurrentQty <> 0 ";
                countRecord = Util.GetValueOfInt(DB.ExecuteScalar(sql1, null, null));
            }

            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, page, size);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    moveContainer.Add(new MoveContainer()
                    {
                        M_Product_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_PRODUCT_ID"]),
                        M_AttributeSetInstance_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["M_ATTRIBUTESETINSTANCE_ID"]),
                        M_ProductContainer_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductContainer_ID"]),
                        C_Uom_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["C_UOM_ID"]),
                        ProductName = Util.GetValueOfString(ds.Tables[0].Rows[i]["NAME"]),
                        ContainerQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["ContainerCurrentQty"]),
                        UomName = Util.GetValueOfString(ds.Tables[0].Rows[i]["UomName"]),
                        ASI = Util.GetValueOfString(ds.Tables[0].Rows[i]["ASI"]),
                        ContainerRecord = countRecord,
                    });
                }
                ds.Dispose();
            }
            return moveContainer;
        }

        /// <summary>
        /// Is used to save data on movememt line 
        /// </summary>
        /// <param name="mData"></param>
        /// <returns></returns>
        public String SaveMovementLine(List<Dictionary<string, string>> mData)
        {
            StringBuilder error = new StringBuilder();
            bool isMoveFullContainer = false;
            bool isMoveFullContainerQty = false;

            Trx trx = Trx.GetTrx("Movement");
            if (mData.Count > 0)
            {
                isMoveFullContainer = Util.GetValueOfBool(mData[0]["IsFullMoveContainer"]);

                //to delete all the movement lines where MoveFullContainer is False
                if (isMoveFullContainer)
                {
                    DB.ExecuteQuery("DELETE FROM M_MovementLine WHERE M_Movement_ID = " + Util.GetValueOfInt(mData[0]["M_Movement_ID"]) + " AND MoveFullContainer= 'N' ", null, null);
                }

                // Lines not inserted, as movement line already has a full move container line.
                if (!isMoveFullContainer)
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(MoveFullContainer) FROM M_MovementLine WHERE MoveFullContainer= 'Y' AND M_Movement_ID = "
                                                              + Util.GetValueOfInt(mData[0]["M_Movement_ID"]), null, trx)) > 0)
                    {
                        trx.Close();
                        return Msg.GetMsg(_ctx, "VIS_LinehaveFullContainer");
                    }
                }

                // Get Line No
                int lineNo = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT NVL(MAX(Line),0) AS DefaultValue FROM M_MovementLine WHERE M_Movement_ID="
                    + Util.GetValueOfInt(mData[0]["M_Movement_ID"]), null, trx));


                isMoveFullContainerQty = Util.GetValueOfBool(mData[0]["IsfullContainerQtyWise"]);

                if (!isMoveFullContainer && !isMoveFullContainerQty)
                    goto moveFullContainer;
                else if (isMoveFullContainer || isMoveFullContainerQty)
                {
                    error.Clear();
                    error.Append(SaveMoveLinewithFullContainer(Util.GetValueOfInt(mData[0]["M_Movement_ID"]),
                                                   Util.GetValueOfInt(mData[0]["FromLocator"]),
                                                   Util.GetValueOfInt(mData[0]["FromContainer"]),
                                                   Util.GetValueOfInt(mData[0]["ToLocator"]),
                                                   Util.GetValueOfInt(mData[0]["ToContainer"]),
                                                   lineNo, isMoveFullContainerQty,
                                                   trx));
                    trx.Close();
                    return error.ToString();
                }

            moveFullContainer:
                MMovementLine moveline = null;
                MProduct product = null;
                int moveId = 0;
                for (int i = 0; i < mData.Count; i++)
                {
                    #region Quantity Only
                    MMovement move = new MMovement(_ctx, Util.GetValueOfInt(mData[i]["M_Movement_ID"]), null);

                    moveId = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT NVL(M_MovementLine_ID, 0) AS M_Movement_ID FROM M_MovementLine WHERE 
                             M_Movement_ID = " + Util.GetValueOfInt(mData[i]["M_Movement_ID"]) +
                             @" AND M_Product_ID = " + Util.GetValueOfInt(mData[i]["M_Product_ID"]) +
                             @" AND NVL(M_AttributeSetInstance_ID, 0) = " + Util.GetValueOfInt(mData[i]["M_AttributeSetInstance_ID"]) +
                             @" AND M_Locator_ID = " + Util.GetValueOfInt(mData[i]["FromLocator"]) +
                             @" AND NVL(M_ProductContainer_ID, 0) = " + Util.GetValueOfInt(mData[i]["FromContainer"]) +
                             @" AND M_LocatorTo_ID = " + Util.GetValueOfInt(mData[i]["ToLocator"]) +
                             @" AND NVL(Ref_M_ProductContainerTo_ID, 0) = " + Util.GetValueOfInt(mData[i]["ToContainer"]) +
                             @" AND AD_Org_ID = " + move.GetAD_Org_ID()));

                    if (moveId > 0)
                    {
                        moveline = new MMovementLine(_ctx, moveId, trx);
                    }
                    else
                    {
                        moveline = new MMovementLine(_ctx, 0, trx);
                    }
                    moveline.SetParent(move);
                    if (moveId == 0)
                    {
                        #region Create new record of movement line
                        lineNo += 10;
                        moveline.SetAD_Client_ID(move.GetAD_Client_ID());
                        moveline.SetAD_Org_ID(move.GetAD_Org_ID());                        
                        moveline.SetM_Movement_ID(move.GetM_Movement_ID());
                        moveline.SetLine(lineNo);
                        moveline.SetM_Product_ID(Util.GetValueOfInt(mData[i]["M_Product_ID"]));
                        moveline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(mData[i]["M_AttributeSetInstance_ID"]));
                        moveline.SetC_UOM_ID(Util.GetValueOfInt(mData[i]["C_UOM_ID"]));
                        moveline.SetM_Locator_ID(Util.GetValueOfInt(mData[i]["FromLocator"]));
                        moveline.SetM_LocatorTo_ID(Util.GetValueOfInt(mData[i]["ToLocator"]));
                        moveline.SetM_ProductContainer_ID(Util.GetValueOfInt(mData[i]["FromContainer"]));
                        moveline.SetRef_M_ProductContainerTo_ID(Util.GetValueOfInt(mData[i]["ToContainer"]));
                        moveline.SetQtyEntered(Util.GetValueOfDecimal(mData[i]["MoveQty"]));
                        moveline.SetMovementQty(Util.GetValueOfDecimal(mData[i]["MoveQty"]));
                        moveline.SetMoveFullContainer(Util.GetValueOfBool(mData[i]["IsFullMoveContainer"]));
                        #endregion
                    }
                    else
                    {
                        #region Update record of movement line
                        moveline.SetQtyEntered(Decimal.Add(moveline.GetQtyEntered(), Util.GetValueOfDecimal(mData[i]["MoveQty"])));
                        moveline.SetMovementQty(Decimal.Add(moveline.GetMovementQty(), Util.GetValueOfDecimal(mData[i]["MoveQty"])));
                        moveline.SetMoveFullContainer(Util.GetValueOfBool(mData[i]["IsFullMoveContainer"]));
                        #endregion
                    }
                    if (!moveline.Save(trx))
                    {
                        #region Save error catch and rollback
                        product = MProduct.Get(_ctx, Util.GetValueOfInt(mData[i]["M_Product_ID"]));
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            _log.SaveError("Movement line not inserted through Move Container Form : ", pp.GetName());
                            if (String.IsNullOrEmpty(error.ToString()))
                            {
                                error.Append(Msg.GetMsg(_ctx, "VIS_MoveLineNotSaveFor") + product.GetName() + Msg.GetMsg(_ctx, "VIS_DueTo") + pp.GetName());
                            }
                            else
                            {
                                error.Append(" , " + Msg.GetMsg(_ctx, "VIS_MoveLineNotSaveFor") + product.GetName() + Msg.GetMsg(_ctx, "VIS_DueTo") + pp.GetName());
                            }
                        }
                        trx.Rollback();
                        #endregion
                    }
                    else
                    {
                        trx.Commit();
                    }

                    #endregion
                }
            }
            trx.Close();
            return String.IsNullOrEmpty(error.ToString()) ? "VIS_SuccessFullyInserted" : error.ToString();
        }

        /// <summary>
        /// is used to save data in case of Full move container / full qty move
        /// </summary>
        /// <param name="movementId">movement header refernce</param>
        /// <param name="fromLocatorId">From Locator - from where we have to move product</param>
        /// <param name="fromContainerId">From Container - from which container, we have to move product</param>
        /// <param name="toLocatorId">To Locator - where we are moving product</param>
        /// <param name="toContainerId">To container - in which container we are moving product</param>
        /// <param name="lineNo"></param>
        /// <param name="isMoveFullContainerQty">Is Container also move with Product</param>
        /// <param name="trx">Self created Trx</param>
        /// <returns>Message : lines inserted or not</returns>
        public String SaveMoveLinewithFullContainer(int movementId, int fromLocatorId, int fromContainerId, int toLocatorId, int toContainerId, int lineNo, bool isMoveFullContainerQty, Trx trx)
        {
            MMovement movement = new MMovement(_ctx, movementId, trx);
            string childContainerId = null;
            StringBuilder error = new StringBuilder();
            bool ispostgerSql = DatabaseType.IsPostgre;
            string sql = "";
            string pathContainer = "";

            // Get Path upto selected container
            if (ispostgerSql)
            {
                sql = @"WITH RECURSIVE pops (M_ProductContainer_id, level, name_path) AS (
                        SELECT  M_ProductContainer_id, 0,  ARRAY[M_ProductContainer_id]
                        FROM    M_ProductContainer
                        WHERE   Ref_M_Container_ID is null
                        UNION ALL
                        SELECT  p.M_ProductContainer_id, t0.level + 1, ARRAY_APPEND(t0.name_path, p.M_ProductContainer_id)
                        FROM    M_ProductContainer p
                                INNER JOIN pops t0 ON t0.M_ProductContainer_id = p.Ref_M_Container_ID
                    )
                    SELECT   ARRAY_TO_STRING(name_path, '->')
                    FROM    pops  where m_productcontainer_id = " + fromContainerId;
            }
            else
            {
                sql = @"SELECT sys_connect_by_path(m_productcontainer_id,'->') tree
                            FROM m_productcontainer 
                           WHERE m_productcontainer_id = " + fromContainerId + @"
                            START WITH ref_m_container_id IS NULL CONNECT BY prior m_productcontainer_id = ref_m_container_id
                           ORDER BY tree ";
            }
            pathContainer = Util.GetValueOfString(DB.ExecuteScalar(sql, null, trx));

            // child records with Parent Id
            if (!isMoveFullContainerQty)
            {
                if (ispostgerSql)
                {
                    sql = @"WITH RECURSIVE pops (M_ProductContainer_id, level, name_path) AS (
                                SELECT  M_ProductContainer_id, 0,  ARRAY[M_ProductContainer_id]
                                FROM    M_ProductContainer
                                WHERE   Ref_M_Container_ID is null
                                UNION ALL
                                SELECT  p.M_ProductContainer_id, t0.level + 1, ARRAY_APPEND(t0.name_path, p.M_ProductContainer_id)
                                FROM    M_ProductContainer p
                                        INNER JOIN pops t0 ON t0.M_ProductContainer_id = p.Ref_M_Container_ID )
                            SELECT  M_ProductContainer_id, level,  ARRAY_TO_STRING(name_path, '->')
                            FROM    pops  where ARRAY_TO_STRING(name_path, '->') like '" + pathContainer + "%'";
                }
                else
                {
                    sql = @"SELECT tree, m_productcontainer_id FROM
                            (SELECT sys_connect_by_path(m_productcontainer_id,'->') tree , m_productcontainer_id
                             FROM m_productcontainer
                             START WITH ref_m_container_id IS NULL
                             CONNECT BY prior m_productcontainer_id = ref_m_container_id
                             ORDER BY tree  
                             )
                           WHERE tree LIKE ('" + pathContainer + "%') ";
                }
                DataSet ds = DB.ExecuteDataset(sql, null, trx);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (String.IsNullOrEmpty(childContainerId))
                            childContainerId = Util.GetValueOfString(ds.Tables[0].Rows[i]["m_productcontainer_id"]);
                        else
                            childContainerId += "," + Util.GetValueOfString(ds.Tables[0].Rows[i]["m_productcontainer_id"]);
                    }
                }
                ds.Dispose();
            }
            else
            {
                childContainerId = fromContainerId.ToString();
            }

            // check is same container already moved to another container
            // Ex :: p1 -> c1 and p1 -> c2
            // OR also check -- is any other container is moving into exist target container 
            // Ex :: p1 -> c1 and p2 -> p1
            if (!isMoveFullContainerQty)
            {
                if (!IsContainerMoved(movementId, pathContainer, childContainerId, toContainerId, trx))
                {
                    return Msg.GetMsg(_ctx, "VIS_AlreadyMoved");
                }
            }

            // not to move Parent container to its child container
            if (toContainerId > 0 && childContainerId.Contains(toContainerId.ToString()))
            {
                //Parent cant be Move to its own child
                return Msg.GetMsg(_ctx, "VIS_cantMoveParentTochild");
            }

            // Get All records of Parent Container and child container
            sql = @"SELECT * FROM (
                            SELECT Distinct p.M_PRODUCT_ID, p.NAME, p.C_UOM_ID, u.Name AS UomName,  t.M_ATTRIBUTESETINSTANCE_ID, t.M_ProductContainer_ID,
                            First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID, t.M_ProductContainer_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS ContainerCurrentQty
                            FROM M_Transaction t
                            INNER JOIN M_Product p ON p.M_Product_ID = t.M_Product_ID
                            INNER JOIN C_UOM u ON u.C_UOM_ID = p.C_UOM_ID
                            WHERE t.IsActive = 'Y' AND NVL(t.M_ProductContainer_ID, 0) IN ( " + childContainerId +
                           @" ) AND t.MovementDate <=" + GlobalVariable.TO_DATE(movement.GetMovementDate(), true) + @" 
                               AND t.M_Locator_ID  = " + fromLocatorId + @"
                               AND t.AD_Client_ID  = " + movement.GetAD_Client_ID() + @"
                          ) t WHERE ContainerCurrentQty <> 0 ";
            DataSet dsRecords = DB.ExecuteDataset(sql, null, trx);
            if (dsRecords != null && dsRecords.Tables.Count > 0 && dsRecords.Tables[0].Rows.Count > 0)
            {
                int movementlineId = 0;
                MMovementLine moveline = null;
                MProduct product = null;
                for (int i = 0; i < dsRecords.Tables[0].Rows.Count; i++)
                {
                    movementlineId = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT NVL(M_MovementLine_ID, 0) AS M_Movement_ID FROM M_MovementLine WHERE 
                             M_Movement_ID = " + Util.GetValueOfInt(movementId) +
                            @" AND M_Product_ID = " + Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_Product_ID"]) +
                            @" AND NVL(M_AttributeSetInstance_ID, 0) = " + Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]) +
                            @" AND M_Locator_ID = " + Util.GetValueOfInt(fromLocatorId) +
                            @" AND NVL(M_ProductContainer_ID, 0) = " + Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_ProductContainer_ID"]) +
                            @" AND M_LocatorTo_ID = " + Util.GetValueOfInt(toLocatorId) +
                            @" AND NVL(Ref_M_ProductContainerTo_ID, 0) = " + toContainerId +
                            @" AND AD_Org_ID = " + movement.GetAD_Org_ID()));

                    if (movementlineId > 0)
                    {
                        moveline = new MMovementLine(_ctx, movementlineId, trx);
                    }
                    else
                    {
                        moveline = new MMovementLine(_ctx, 0, trx);
                    }
                    moveline.SetParent(movement);
                    if (movementlineId == 0)
                    {
                        #region Create new record of movement line
                        lineNo += 10;
                        moveline.SetAD_Client_ID(movement.GetAD_Client_ID());
                        moveline.SetAD_Org_ID(movement.GetAD_Org_ID());
                        moveline.SetM_Movement_ID(movement.GetM_Movement_ID());
                        moveline.SetLine(lineNo);
                        moveline.SetM_Product_ID(Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_Product_ID"]));
                        moveline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));
                        moveline.SetC_UOM_ID(Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["C_UOM_ID"]));
                        moveline.SetM_Locator_ID(fromLocatorId);
                        moveline.SetM_LocatorTo_ID(toLocatorId);
                        moveline.SetM_ProductContainer_ID(Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_ProductContainer_ID"]));
                        moveline.SetRef_M_ProductContainerTo_ID(toContainerId);
                        moveline.SetQtyEntered(Util.GetValueOfDecimal(dsRecords.Tables[0].Rows[i]["ContainerCurrentQty"]));
                        moveline.SetMovementQty(Util.GetValueOfDecimal(dsRecords.Tables[0].Rows[i]["ContainerCurrentQty"]));
                        moveline.SetMoveFullContainer(isMoveFullContainerQty ? false : true);
                        // when move full container, then only need to update IsParentMove and Target container (which represent - to which container we are moving)
                        // and set true value on those line which container are moving, not on its child container
                        if (!isMoveFullContainerQty)
                        {
                            moveline.SetIsParentMove(Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_ProductContainer_ID"]) == fromContainerId ? true : false);
                            moveline.SetTargetContainer_ID(fromContainerId);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Update record of movement line
                        moveline.SetQtyEntered(Decimal.Add(moveline.GetQtyEntered(), Util.GetValueOfDecimal(dsRecords.Tables[0].Rows[i]["ContainerCurrentQty"])));
                        moveline.SetMovementQty(Decimal.Add(moveline.GetMovementQty(), Util.GetValueOfDecimal(dsRecords.Tables[0].Rows[i]["ContainerCurrentQty"])));
                        moveline.SetMoveFullContainer(true);
                        #endregion
                    }
                    if (!moveline.Save(trx))
                    {
                        #region Save error catch and rollback
                        product = MProduct.Get(_ctx, Util.GetValueOfInt(dsRecords.Tables[0].Rows[i]["M_Product_ID"]));
                        ValueNamePair pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            _log.SaveError("Movement line not inserted through Move Container Form : ", pp.GetName());
                            if (String.IsNullOrEmpty(error.ToString()))
                            {
                                error.Append(Msg.GetMsg(_ctx, "VIS_MoveLineNotSaveFor") + product.GetName() + Msg.GetMsg(_ctx, "VIS_DueTo") + pp.GetName());
                            }
                            else
                            {
                                error.Append(" , " + Msg.GetMsg(_ctx, "VIS_MoveLineNotSaveFor") + product.GetName() + Msg.GetMsg(_ctx, "VIS_DueTo") + pp.GetName());
                            }
                        }
                        trx.Rollback();
                        #endregion
                    }
                    else
                    {
                        trx.Commit();
                    }
                }
            }
            else
            {
                return Msg.GetMsg(_ctx, "VIS_ContainerhaveNoRecord");
            }

            return String.IsNullOrEmpty(error.ToString()) ? "VIS_SuccessFullyInserted" : error.ToString();

        }

        /// <summary>
        /// is used to check :: after move P1 container into another container, we can't move its child again into another container 
        /// :: after move P1 container into another container like C1, again we can't move same P1 container into anothe container like C2
        /// :: after moving child container into another container, we can't move its parent container into another container
        /// :: when we move any container into "To Container" - system will check "To container or its parent conatiner is already moved or not -- if moved then not possible to move into "To Container"
        /// </summary>
        /// <param name="movementId">movement Header Reference</param>
        /// <param name="pathUptoFromContainer">path from parent container to "From Container"</param>
        /// <param name="allChildIncludeParentFromContainer">path from "From container" to "All its child container"</param>
        /// <param name="toContainerId">To Container Reference</param>
        /// <param name="trx"></param>
        /// <returns>True/False</returns>
        public bool IsContainerMoved(int movementId, string pathUptoFromContainer, string allChildIncludeParentFromContainer, int toContainerId, Trx trx)
        {
            // Get Path upto selected To container
            string pathUptoToContainer = "";
            if (DatabaseType.IsPostgre)
            {
                String sql = @"WITH RECURSIVE pops (M_ProductContainer_id, level, name_path) AS (
                        SELECT  M_ProductContainer_id, 0,  ARRAY[M_ProductContainer_id]
                        FROM    M_ProductContainer
                        WHERE   Ref_M_Container_ID is null
                        UNION ALL
                        SELECT  p.M_ProductContainer_id, t0.level + 1, ARRAY_APPEND(t0.name_path, p.M_ProductContainer_id)
                        FROM    M_ProductContainer p
                                INNER JOIN pops t0 ON t0.M_ProductContainer_id = p.Ref_M_Container_ID
                    )
                        SELECT    ARRAY_TO_STRING(name_path, '->')
                        FROM    pops  where m_productcontainer_id = " + toContainerId;
                pathUptoToContainer = Util.GetValueOfString(DB.ExecuteScalar(sql, null, trx));
            }
            else
            {
                pathUptoToContainer = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT sys_connect_by_path(m_productcontainer_id,'->') tree
                            FROM m_productcontainer 
                           WHERE m_productcontainer_id = " + toContainerId + @"
                            START WITH ref_m_container_id IS NULL CONNECT BY prior m_productcontainer_id = ref_m_container_id
                           ORDER BY tree ", null, trx));
            }
            DataSet dsTragetContainer = DB.ExecuteDataset(@"SELECT DISTINCT targetcontainer_id FROM M_MovementLine 
                                        WHERE MoveFullContainer='Y' AND IsActive = 'Y' AND M_Movement_ID = " + movementId, null, trx);
            if (dsTragetContainer != null && dsTragetContainer.Tables.Count > 0 && dsTragetContainer.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTragetContainer.Tables[0].Rows.Count; i++)
                {
                    // after move P1 container into another container, we can't move its child again into another container 
                    // after moving child container into another container, we can't move its parent container into another container
                    if (pathUptoFromContainer.Contains(Util.GetValueOfString(dsTragetContainer.Tables[0].Rows[i]["targetcontainer_id"])))
                    {
                        return false;
                    }
                    if (allChildIncludeParentFromContainer.Contains(Util.GetValueOfString(dsTragetContainer.Tables[0].Rows[i]["targetcontainer_id"])))
                    {
                        return false;
                    }

                    // when we move any container into "To Container" - system will check "To container or its parent conatiner is already moved or not 
                    // if moved then not possible to move into "To Container"
                    if (pathUptoToContainer.Contains(Util.GetValueOfString(dsTragetContainer.Tables[0].Rows[i]["targetcontainer_id"])))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string GetProductContainerInfo(int ID)
        {
            string sql = " SELECT  Value || '_' || Name as des from M_ProductContainer WHERE M_ProductContainer_ID=" + ID;
            object des = DB.ExecuteScalar(sql);
            if (des != null && des != DBNull.Value)
            {
                return des.ToString();
            }

            return "";
        }


        public string GetProductContainer(string text, string validation)
        {
            text = text.ToUpper();
            string sql = @"SELECT Value || '_' || Name as des, M_ProductContainer_ID FROM M_ProductContainer WHERE IsActive='Y'
            AND (Upper(VALUE) like '%" + text + "%' OR UPPER(Name) LIKE '%" + text + "%')";
            sql = MRole.GetDefault(_ctx).AddAccessSQL(sql, "M_ProductContainer", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);

            sql += " AND " + validation;

            VLogger.Get().Log(Level.SEVERE, sql);
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows.Count > 1)
            {
                return null;
            }
            return Convert.ToString(ds.Tables[0].Rows[0]["M_ProductContainer_ID"]);
            // return new KeyNamePair(Convert.ToString(ds.Tables[0].Rows[0]["M_ProductContainer_ID"]), Convert.ToString(ds.Tables[0].Rows[0]["desc"]));
        }

        /// <summary>
        /// is used to save Product container
        /// </summary>
        /// <param name="warehouseId">Warehouse where we create container</param>
        /// <param name="locatorId">Locator - in which locator we place container</param>
        /// <param name="value">Search key of the container</param>
        /// <param name="name">name of teh container</param>
        /// <param name="height">height of the container</param>
        /// <param name="width">width of the container</param>
        /// <param name="parentContainerId">Parent of the nw container</param>
        /// <returns>Save Or Not Saved message</returns>
        /// <writer>Amit Bansal</writer>
        public string SaveProductContainer(int warehouseId, int locatorId, string value, string name, Decimal height, Decimal width, int parentContainerId)
        {
            MLocator m_locator = null;
            MWarehouse m_warehouse = null;

            // when warehouse ID is ZERO, then extract it from Locator
            if (warehouseId == 0 && locatorId > 0)
            {
                m_locator = MLocator.Get(_ctx, locatorId);
                warehouseId = m_locator.GetM_Warehouse_ID();
            }
            // when locator ID is ZERO, then extract it either from Parent Conatiner or from Warehouse
            else if (warehouseId > 0 && locatorId == 0)
            {
                if (parentContainerId == 0)
                {
                    m_warehouse = MWarehouse.Get(_ctx, warehouseId);
                    locatorId = m_warehouse.GetDefaultM_Locator_ID();
                }
                else
                {
                    locatorId = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Locator_ID FROM M_ProductContainer WHERE M_ProductContainer_ID = " + parentContainerId, null, null));
                }
            }

            // need to check warehouse and locator shoyld be active during ceation of Product Container
            m_warehouse = MWarehouse.Get(_ctx, warehouseId);
            m_locator = MLocator.Get(_ctx, locatorId);
            if (!m_warehouse.IsActive())
            {
                return Msg.GetMsg(_ctx, "VIS_WarehouseNotActive");
            }
            else if (!m_locator.IsActive())
            {
                return Msg.GetMsg(_ctx, "VIS_LocatorNotActive");
            }

            // Create Product Container in Locator Organization
            MProductContainer container = new MProductContainer(_ctx, 0, null);
            container.SetAD_Org_ID(m_locator.GetAD_Org_ID());
            container.SetValue(value);
            container.SetName(name);
            container.SetM_Warehouse_ID(warehouseId);
            container.SetM_Locator_ID(locatorId);
            container.SetHeight(height);
            container.SetWidth(width);
            container.SetRef_M_Container_ID(parentContainerId);
            if (!container.Save())
            {
                ValueNamePair pp = VLogger.RetrieveError();
                return Msg.GetMsg(_ctx, "VIS_ContainernotSaved") + " " + (pp != null ? pp.GetName() : "");
            }
            else
            {
                return "";
            }
        }

    }

    public class MoveContainer
    {
        public int M_Product_ID { get; set; }
        public int M_AttributeSetInstance_ID { get; set; }
        public int M_ProductContainer_ID { get; set; }
        public int C_Uom_ID { get; set; }
        public String ProductName { get; set; }
        public Decimal ContainerQty { get; set; }
        public String UomName { get; set; }
        public String ASI { get; set; }
        public int ContainerRecord { get; set; }
    }

    public class PContainer
    {
        public int M_ProductContainer_ID { get; set; }
        public int Ref_M_Container_ID { get; set; }
        public string ContainerName { get; set; }
        public string ParentPath { get; set; }
        public Decimal Width { get; set; }
        public Decimal Height { get; set; }
    }

    public class MoveKeyVal
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class TreeContainer
    {
        public String Value { get; set; }
        public String Name { get; set; }
        public int Level { get; set; }
        public String ContainerName { get; set; }
        public int M_ProductContainer_ID { get; set; }
    }
}