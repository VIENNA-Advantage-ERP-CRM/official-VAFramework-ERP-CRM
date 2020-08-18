/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : ActivateProductContainer
    * Purpose        : Is used to enable "Product Container" into the system
                       this process update the "ContainerCurentQty" on "M_Transaction" with "CurrentQty"
                       also update the M_ContainerStorage with QtyOnHand
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     21-Jan-2019
******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;


namespace VAdvantage.Process
{
    public class ActivateProductContainer : SvrProcess
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(ActivateProductContainer).FullName);
        private StringBuilder sql = new StringBuilder();

        protected override void Prepare()
        {
            ;
        }

        /// <summary>
        /// This Method is used to Activate Product container functionlaity into Vienna System
        /// </summary>
        /// <returns>Message - Container is Applicabled or not</returns>
        protected override string DoIt()
        {
            // check container applicable or not
            if (Util.GetValueOfString(GetCtx().GetContext("#PRODUCT_CONTAINER_APPLICABLE")).Equals("N"))
            {
                // verify document is closed or not
                if (!VerifyDocumentStatus())
                {
                    return Msg.GetMsg(GetCtx(), "VIS_NotClosedocument");
                }

                // update container qty with current qty on all records on transaction
                int no = DB.ExecuteQuery(@"UPDATE M_TRANSACTION SET ContainerCurrentQty = CurrentQty", null, Get_Trx());
                if (no > 0)
                {
                    // delete all record from container storage
                    no = DB.ExecuteQuery(@"DELETE FROM  M_ContainerStorage", null, Get_Trx());

                    // get all data from storage having OnHandQty != 0, and create recod on container storage with locator, product and qty details
                    sql.Clear();
                    sql.Append(@"SELECT M_Transaction.AD_client_ID , M_Transaction.AD_Org_ID , M_Transaction.M_Locator_ID , M_Transaction.M_Product_ID ,
                                  NVL(M_Transaction.M_AttributeSetInstance_ID , 0) AS M_AttributeSetInstance_ID , M_Transaction.M_Transaction_ID ,
                                  M_Transaction.MovementDate , NVL(M_Transaction.MovementQty , 0) AS MovementQty , M_Transaction.CurrentQty , M_Transaction.MovementType ,
                                  CASE WHEN M_Transaction.M_InventoryLine_ID > 0
                                    AND (SELECT IsInternalUse FROM M_InventoryLine
                                      WHERE M_InventoryLine.M_InventoryLine_ID = M_Transaction.M_InventoryLine_ID ) = 'N'
                                    THEN 'Y' ELSE 'N' END AS PhysicalInventory,
                                    (SELECT MMPolicy FROM M_Product_Category WHERE M_Product_Category.M_Product_Category_ID = M_Product.M_Product_Category_ID) AS MMPolicy
                                FROM M_Transaction 
                                INNER JOIN M_Product ON M_Product.M_Product_ID = M_Transaction.M_Product_ID  
                                ORDER BY M_Transaction.AD_Client_ID , M_Transaction.M_Locator_ID , M_Transaction.M_Product_ID ,
                                M_Transaction.M_AttributeSetInstance_ID , M_Transaction.MovementDate , M_Transaction.M_Transaction_ID ASC");
                    DataSet dsStorage = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                    if (dsStorage != null && dsStorage.Tables.Count > 0 && dsStorage.Tables[0].Rows.Count > 0)
                    {
                        X_M_ContainerStorage containerStorage = null;
                        int M_ContainerStorage_ID = 0;
                        for (int i = 0; i < dsStorage.Tables[0].Rows.Count; i++)
                        {
                            // get dataRow from dataset
                            DataRow dr = dsStorage.Tables[0].Rows[i];

                            if (Convert.ToInt32(dr["MovementQty"]) > 0)
                            {
                                // Quantity IN
                                M_ContainerStorage_ID = GetContainerStorage(Convert.ToInt32(dr["M_Locator_ID"]), Convert.ToInt32(dr["M_Product_ID"]),
                                                    Convert.ToInt32(dr["M_AttributeSetInstance_ID"]), Convert.ToDateTime(dr["MovementDate"]),
                                                    Convert.ToString(dr["PhysicalInventory"]));

                                containerStorage = new X_M_ContainerStorage(GetCtx(), M_ContainerStorage_ID, Get_Trx()); // object of container storage                                                                                          
                                // Set or Update Values 
                                containerStorage = InsertContainerStorage(containerStorage, dr);
                                if (!containerStorage.Save(Get_Trx()))
                                {
                                    Get_Trx().Rollback();
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    throw new ArgumentException((Msg.GetMsg(GetCtx(), "VIS_ContainerStorageNotSave") + (pp != null && !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : " ")));
                                }
                                else
                                {
                                    // after insertion, system will check if qty = 0 and not a physical inventory record then delete record
                                    if (containerStorage.GetQty() == 0 && !containerStorage.IsPhysicalInventory())
                                    {
                                        containerStorage.Delete(true, Get_Trx());
                                    }
                                }
                            }
                            else
                            {
                                // Quantity Out
                                ConsumeQtyFromStorage(dr);
                            }
                        }
                    }
                }
                // Update setting as Container Applicable into the system
                no = DB.ExecuteQuery("UPDATE AD_SysConfig SET VALUE = 'Y' WHERE Name='PRODUCT_CONTAINER_APPLICABLE'", null, Get_Trx());
            }
            else
            {
                return Msg.GetMsg(GetCtx(), "VIS_AlreadyActivateContainer");
            }
            return Msg.GetMsg(GetCtx(), "VIS_ActivatedContainer");
        }

        /// <summary>
        /// This function is used to get record id refernce if created on specified reference
        /// </summary>
        /// <param name="M_Locator_ID">Locator ID</param>
        /// <param name="M_Product_ID">Product ID</param>
        /// <param name="M_AttributeSetInstance_ID">AttributeSetInstance ID</param>
        /// <param name="MovementDate">Movement Date / Policy Date</param>
        /// <param name="IsPhysicalInventory">Is physical Inventory record</param>
        /// <returns>record Id, if found else 0</returns>
        private int GetContainerStorage(int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID, DateTime? MovementDate, String IsPhysicalInventory)
        {
            int M_ContainerStorage_ID = 0;
            String sql = "SELECT M_ContainerStorage_ID FROM M_ContainerStorage WHERE M_Locator_ID = " + M_Locator_ID +
                            @" AND M_Product_ID = " + M_Product_ID + @" AND NVL(M_AttributeSetInstance_ID , 0) = " + M_AttributeSetInstance_ID +
                            @" AND MMPolicyDate = " + GlobalVariable.TO_DATE(MovementDate, true);
            //@" AND IsPhysicalInventory = '" + IsPhysicalInventory + "'";
            M_ContainerStorage_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            return M_ContainerStorage_ID;
        }

        /// <summary>
        /// this function is used to check all document must be either voided, revesed, closed
        /// </summary>
        /// <returns>TRUE/FALSE</returns>
        private bool VerifyDocumentStatus()
        {
            //int no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_Order_ID) FROM C_Order WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
            //if (no > 0)
            //{
            //    return false;
            //}

            int no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_InOut_ID) FROM M_InOut WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
            if (no > 0)
            {
                return false;
            }

            no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_Inventory_ID) FROM M_Inventory WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
            if (no > 0)
            {
                return false;
            }

            no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(M_Movement_ID) FROM M_Movement WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
            if (no > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// is used to set value for Container Storage
        /// </summary>
        /// <param name="containerStorage">object of container storage</param>
        /// <param name="dr">data row</param>
        /// <param name="isPhysicalInventory">is record created for physical inventory</param>
        /// <returns>object of container storage</returns>
        private X_M_ContainerStorage InsertContainerStorage(X_M_ContainerStorage containerStorage, DataRow dr, bool isPhysicalInventory)
        {
            containerStorage.SetAD_Client_ID(Convert.ToInt32(dr["AD_Client_ID"]));
            containerStorage.SetAD_Org_ID(Convert.ToInt32(dr["AD_Org_ID"]));
            containerStorage.SetM_Locator_ID(Convert.ToInt32(dr["M_Locator_ID"]));
            containerStorage.SetM_Product_ID(Convert.ToInt32(dr["M_Product_ID"]));
            containerStorage.SetM_AttributeSetInstance_ID(Convert.ToInt32(dr["M_AttributeSetInstance_ID"]));

            // when storage dont have DateLastInventory -- means not any physical inventory
            if (dr["DateLastInventory"] == DBNull.Value)
            {
                containerStorage.SetMMPolicyDate(DateTime.Now);
                containerStorage.SetQty(Convert.ToDecimal(dr["QtyOnHand"]));
            }
            else
            {
                // when (QtyOnhand - qty) > 0 means storage having more qty than last physical inventory
                Decimal qty = Decimal.Subtract(Convert.ToDecimal(dr["QtyOnHand"]), Convert.ToDecimal(dr["qty"]));

                if (isPhysicalInventory)
                {
                    containerStorage.SetMMPolicyDate(Convert.ToDateTime(dr["DateLastInventory"]));
                    containerStorage.SetIsPhysicalInventory(true);

                    if (qty >= 0)
                    {
                        // when storage having more qty than last physical inventory then did entry on container storage with (Physical Inventory Qty)
                        containerStorage.SetQty(Convert.ToDecimal(dr["qty"]));
                    }
                    else
                    {
                        // when storage having less qty than last physical inventory then did entry on container storage with (Physical Inventory Qty - qty on storage)
                        //containerStorage.SetQty(Decimal.Subtract(Convert.ToDecimal(dr["qty"]), Convert.ToDecimal(dr["QtyOnHand"])));
                        containerStorage.SetQty(Convert.ToDecimal(dr["QtyOnHand"]));
                    }
                }
                else
                {
                    containerStorage.SetMMPolicyDate(DateTime.Now);
                    containerStorage.SetQty(qty);
                }
            }
            return containerStorage;
        }

        /// <summary>
        /// This function is used to create or update record on Container Storage
        /// </summary>
        /// <param name="containerStorage">Container Storage Object</param>
        /// <param name="dr">data row</param>
        /// <returns>container Storage object</returns>
        private X_M_ContainerStorage InsertContainerStorage(X_M_ContainerStorage containerStorage, DataRow dr)
        {
            if (containerStorage.Get_ID() <= 0)
            {
                containerStorage.SetAD_Client_ID(Convert.ToInt32(dr["AD_Client_ID"]));
                containerStorage.SetAD_Org_ID(MLocator.Get(containerStorage.GetCtx(), Convert.ToInt32(dr["M_Locator_ID"])).GetAD_Org_ID());
                containerStorage.SetM_Locator_ID(Convert.ToInt32(dr["M_Locator_ID"]));
                containerStorage.SetM_Product_ID(Convert.ToInt32(dr["M_Product_ID"]));
                containerStorage.SetM_AttributeSetInstance_ID(Convert.ToInt32(dr["M_AttributeSetInstance_ID"]));
                containerStorage.SetMMPolicyDate(Convert.ToDateTime(dr["MovementDate"]));
                containerStorage.SetIsPhysicalInventory(Convert.ToString(dr["PhysicalInventory"]).Equals("Y") ? true : false);
                containerStorage.SetQty(Convert.ToDecimal(dr["MovementQty"]));
            }
            else
            {
                containerStorage.SetQty(Decimal.Add(containerStorage.GetQty(), Convert.ToDecimal(dr["MovementQty"])));
            }
            return containerStorage;
        }

        /// <summary>
        /// This funcion is used to get record from container storage based on Material Policy
        /// </summary>
        /// <param name="dr">data row object</param>
        /// <returns>true/false</returns>
        private bool ConsumeQtyFromStorage(DataRow dr)
        {
            Decimal Qty = Math.Abs(Convert.ToDecimal(dr["MovementQty"]));
            int no;
            String sql = @"SELECT M_ContainerStorage_ID , Qty , IsPhysicalInventory FROM M_ContainerStorage WHERE Qty > 0 
                               AND M_Locator_ID = " + Convert.ToInt32(dr["M_Locator_ID"]) +
                            @" AND M_Product_ID = " + Convert.ToInt32(dr["M_Product_ID"]) +
                            @" AND NVL(M_AttributeSetInstance_ID , 0) = " + Convert.ToInt32(dr["M_AttributeSetInstance_ID"]);
            if (Convert.ToString(dr["MMPolicy"]).Equals(MProductCategory.MMPOLICY_LiFo))
            {
                sql += " ORDER BY MMPolicyDate  DESC , M_ContainerStorage_ID DESC";
            }
            else
            {
                sql += " ORDER BY MMPolicyDate , M_ContainerStorage_ID ASC";
            }
            DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Decimal qtyStorage = 0.0M;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    qtyStorage = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Qty"]);

                    if (Qty >= qtyStorage)
                    {
                        // when consume qty is greater than current traverse row qty, then delete this record if not physical record else set qty = 0

                        if (Util.GetValueOfString(ds.Tables[0].Rows[i]["IsPhysicalInventory"]).Equals("Y"))
                        {
                            // update record - bcz it is physical inventory record
                            no = DB.ExecuteQuery("Update M_ContainerStorage SET QTY = 0 , IsPhysicalInventory = 'Y' WHERE M_ContainerStorage_ID = "
                                                + Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["M_ContainerStorage_ID"]), null, Get_Trx());
                        }
                        else
                        {
                            // delete record
                            no = DB.ExecuteQuery("DELETE FROM  M_ContainerStorage WHERE M_ContainerStorage_ID = "
                                               + Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["M_ContainerStorage_ID"]), null, Get_Trx());
                        }

                        // set remaining qty which is to be reduce 
                        Qty = decimal.Subtract(Qty, qtyStorage);
                    }
                    else
                    {
                        // subtract qty from the current traverse record
                        no = DB.ExecuteQuery("Update M_ContainerStorage SET QTY = Qty - " + Qty + @" WHERE M_ContainerStorage_ID = "
                                                + Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["M_ContainerStorage_ID"]), null, Get_Trx());
                        Qty = 0;
                        break;
                    }
                }
            }

            // after consumption, if any qty left than need to create record there with -ve remaining qty
            if (Qty != 0)
            {
                X_M_ContainerStorage containerStorage = new X_M_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage  
                containerStorage = InsertContainerStorage(containerStorage, dr);
                containerStorage.SetQty(Decimal.Negate(Qty));
                if (!containerStorage.Save(Get_Trx()))
                {
                    Get_Trx().Rollback();
                    ValueNamePair pp = VLogger.RetrieveError();
                    throw new ArgumentException((Msg.GetMsg(GetCtx(), "VIS_ContainerStorageNotSave") + (pp != null && !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : " ")));
                }
            }
            return true;
        }

        //        protected override string DoIt()
        //        {
        //            // check container applicable or not
        //            if (Util.GetValueOfString(GetCtx().GetContext("#PRODUCT_CONTAINER_APPLICABLE")) == "N")
        //            {
        //                if (!VerifyDocumentStatus())
        //                {
        //                    return Msg.GetMsg(GetCtx(), "VIS_NotClosedocument");
        //                }
        //                // update container qty with current qty on all records on transaction
        //                int no = DB.ExecuteQuery(@"UPDATE M_TRANSACTION SET ContainerCurrentQty = CurrentQty", null, Get_Trx());
        //                if (no > 0)
        //                {
        //                    // delete all record from container storage
        //                    no = DB.ExecuteQuery(@"DELETE FROM  M_ContainerStorage", null, Get_Trx());

        //                    // get all data from storage having OnHandQty != 0, and create recod on container storage with locator, product and qty details
        //                    sql.Clear();
        //                    sql.Append(@"WITH conSTORAGE AS
        //  (SELECT il.M_Locator_ID, il.m_product_id, il.M_AttributeSetInstance_ID, i.MovementDate, SUM(il.QtyCount - il.QtyBook) AS qty
        //  FROM m_inventory i INNER JOIN m_inventoryline il ON i.m_inventory_id = il.m_inventory_id WHERE i.isinternaluse = 'N' 
        //  GROUP BY il.M_Locator_ID, il.m_product_id, il.M_AttributeSetInstance_ID, i.MovementDate  )
        //SELECT AD_Client_ID, AD_Org_ID, M_Storage.M_Locator_ID, M_Storage.M_Product_ID,  NVL(M_Storage.M_AttributeSetInstance_ID , 0) AS M_AttributeSetInstance_ID,
        //  SUM(QtyOnHand) AS QtyOnHand, DateLastInventory, NVL(qty, 0) AS qty
        //FROM M_Storage LEFT JOIN conSTORAGE ON ( conSTORAGE.m_locator_id = M_Storage.m_locator_id
        //AND conSTORAGE.m_product_id  = M_Storage.m_product_id AND NVL(conSTORAGE.M_AttributeSetInstance_ID , 0) = NVL(M_Storage.M_AttributeSetInstance_ID , 0)
        //AND conSTORAGE.MovementDate = M_Storage.DateLastInventory)
        //GROUP BY AD_Client_ID, AD_Org_ID, M_Storage.M_Locator_ID, M_Storage.M_Product_ID, M_Storage.M_AttributeSetInstance_ID, DateLastInventory, qty
        //HAVING SUM(QtyOnHand) != 0");
        //                    _log.Info(sql.ToString());
        //                    DataSet dsStorage = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
        //                    if (dsStorage != null && dsStorage.Tables.Count > 0 && dsStorage.Tables[0].Rows.Count > 0)
        //                    {
        //                        X_M_ContainerStorage containerStorage = null;
        //                        bool isPhysicalInventory = false;
        //                        for (int i = 0; i < dsStorage.Tables[0].Rows.Count; i++)
        //                        {
        //                            // get dataRow from dataset
        //                            DataRow dr = dsStorage.Tables[0].Rows[i];

        //                            // when storage dont have DateLastInventory -- means not any physical inventory
        //                            if (dr["DateLastInventory"] == DBNull.Value)
        //                            {
        //                                isPhysicalInventory = false; // is record save for Physical inventory

        //                                containerStorage = new X_M_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage

        //                                // Set Values 
        //                                containerStorage = InsertContainerStorage(containerStorage, dr, isPhysicalInventory);
        //                                if (!containerStorage.Save(Get_Trx()))
        //                                {
        //                                    Get_Trx().Rollback();
        //                                    ValueNamePair pp = VLogger.RetrieveError();
        //                                    throw new ArgumentException((Msg.GetMsg(GetCtx(), "VIS_ContainerStorageNotSave") + (pp != null && !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : " ")));
        //                                }
        //                            }
        //                            else
        //                            {
        //                                // get difference between qty on storage and on last physical inventory
        //                                Decimal qty = Decimal.Subtract(Convert.ToDecimal(dr["QtyOnHand"]), Convert.ToDecimal(dr["qty"]));

        //                                if (qty > 0)
        //                                {
        //                                    bool iscontinue = true; // is continue in goto statement fo next iteration
        //                                    isPhysicalInventory = false;

        //                                newRecord:
        //                                    containerStorage = new X_M_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage

        //                                    // Set Values 
        //                                    containerStorage = InsertContainerStorage(containerStorage, dr, isPhysicalInventory);
        //                                    if (!containerStorage.Save(Get_Trx()))
        //                                    {
        //                                        Get_Trx().Rollback();
        //                                        ValueNamePair pp = VLogger.RetrieveError();
        //                                        throw new ArgumentException((Msg.GetMsg(GetCtx(), "VIS_ContainerStorageNotSave") + (pp != null && !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : " ")));
        //                                    }
        //                                    else
        //                                    {
        //                                        if (iscontinue)
        //                                        {
        //                                            isPhysicalInventory = true;
        //                                            iscontinue = false;
        //                                            goto newRecord;
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    containerStorage = new X_M_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage

        //                                    // Set Values 
        //                                    containerStorage = InsertContainerStorage(containerStorage, dr, true);
        //                                    if (!containerStorage.Save(Get_Trx()))
        //                                    {
        //                                        Get_Trx().Rollback();
        //                                        ValueNamePair pp = VLogger.RetrieveError();
        //                                        throw new ArgumentException((Msg.GetMsg(GetCtx(), "VIS_ContainerStorageNotSave") + (pp != null && !String.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : " ")));
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                // Update setting as Container Applicable into the system
        //                no = DB.ExecuteQuery("UPDATE AD_SysConfig SET VALUE = 'Y' WHERE Name='PRODUCT_CONTAINER_APPLICABLE'", null, Get_Trx());
        //            }
        //            else
        //            {
        //                return Msg.GetMsg(GetCtx(), "VIS_AlreadyActivateContainer");
        //            }
        //            return Msg.GetMsg(GetCtx(), "VIS_ActivatedContainer");
        //        }
    }
}
