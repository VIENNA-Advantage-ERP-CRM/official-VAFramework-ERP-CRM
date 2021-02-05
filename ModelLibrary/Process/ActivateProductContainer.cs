/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : ActivateProductContainer
    * Purpose        : Is used to enable "Product Container" into the system
                       this process update the "ContainerCurentQty" on "VAM_Inv_Trx" with "CurrentQty"
                       also update the VAM_ContainerStorage with QtyOnHand
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
                int no = DB.ExecuteQuery(@"UPDATE VAM_Inv_Trx SET ContainerCurrentQty = CurrentQty", null, Get_Trx());
                if (no > 0)
                {
                    // delete all record from container storage
                    no = DB.ExecuteQuery(@"DELETE FROM  VAM_ContainerStorage", null, Get_Trx());

                    // get all data from storage having OnHandQty != 0, and create recod on container storage with locator, product and qty details
                    sql.Clear();
                    sql.Append(@"SELECT VAM_Inv_Trx.vaf_client_ID , VAM_Inv_Trx.VAF_Org_ID , VAM_Inv_Trx.VAM_Locator_ID , VAM_Inv_Trx.VAM_Product_ID ,
                                  NVL(VAM_Inv_Trx.VAM_PFeature_SetInstance_ID , 0) AS VAM_PFeature_SetInstance_ID , VAM_Inv_Trx.VAM_Inv_Trx_ID ,
                                  VAM_Inv_Trx.MovementDate , NVL(VAM_Inv_Trx.MovementQty , 0) AS MovementQty , VAM_Inv_Trx.CurrentQty , VAM_Inv_Trx.MovementType ,
                                  CASE WHEN VAM_Inv_Trx.VAM_InventoryLine_ID > 0
                                    AND (SELECT IsInternalUse FROM VAM_InventoryLine
                                      WHERE VAM_InventoryLine.VAM_InventoryLine_ID = VAM_Inv_Trx.VAM_InventoryLine_ID ) = 'N'
                                    THEN 'Y' ELSE 'N' END AS PhysicalInventory,
                                    (SELECT MMPolicy FROM VAM_ProductCategory WHERE VAM_ProductCategory.VAM_ProductCategory_ID = VAM_Product.VAM_ProductCategory_ID) AS MMPolicy
                                FROM VAM_Inv_Trx 
                                INNER JOIN VAM_Product ON VAM_Product.VAM_Product_ID = VAM_Inv_Trx.VAM_Product_ID  
                                ORDER BY VAM_Inv_Trx.VAF_Client_ID , VAM_Inv_Trx.VAM_Locator_ID , VAM_Inv_Trx.VAM_Product_ID ,
                                VAM_Inv_Trx.VAM_PFeature_SetInstance_ID , VAM_Inv_Trx.MovementDate , VAM_Inv_Trx.VAM_Inv_Trx_ID ASC");
                    DataSet dsStorage = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                    if (dsStorage != null && dsStorage.Tables.Count > 0 && dsStorage.Tables[0].Rows.Count > 0)
                    {
                        X_VAM_ContainerStorage containerStorage = null;
                        int VAM_ContainerStorage_ID = 0;
                        for (int i = 0; i < dsStorage.Tables[0].Rows.Count; i++)
                        {
                            // get dataRow from dataset
                            DataRow dr = dsStorage.Tables[0].Rows[i];

                            if (Convert.ToInt32(dr["MovementQty"]) > 0)
                            {
                                // Quantity IN
                                VAM_ContainerStorage_ID = GetContainerStorage(Convert.ToInt32(dr["VAM_Locator_ID"]), Convert.ToInt32(dr["VAM_Product_ID"]),
                                                    Convert.ToInt32(dr["VAM_PFeature_SetInstance_ID"]), Convert.ToDateTime(dr["MovementDate"]),
                                                    Convert.ToString(dr["PhysicalInventory"]));

                                containerStorage = new X_VAM_ContainerStorage(GetCtx(), VAM_ContainerStorage_ID, Get_Trx()); // object of container storage                                                                                          
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
                no = DB.ExecuteQuery("UPDATE VAF_SysConfig SET VALUE = 'Y' WHERE Name='PRODUCT_CONTAINER_APPLICABLE'", null, Get_Trx());
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
        /// <param name="VAM_Locator_ID">Locator ID</param>
        /// <param name="VAM_Product_ID">Product ID</param>
        /// <param name="VAM_PFeature_SetInstance_ID">AttributeSetInstance ID</param>
        /// <param name="MovementDate">Movement Date / Policy Date</param>
        /// <param name="IsPhysicalInventory">Is physical Inventory record</param>
        /// <returns>record Id, if found else 0</returns>
        private int GetContainerStorage(int VAM_Locator_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, DateTime? MovementDate, String IsPhysicalInventory)
        {
            int VAM_ContainerStorage_ID = 0;
            String sql = "SELECT VAM_ContainerStorage_ID FROM VAM_ContainerStorage WHERE VAM_Locator_ID = " + VAM_Locator_ID +
                            @" AND VAM_Product_ID = " + VAM_Product_ID + @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + VAM_PFeature_SetInstance_ID +
                            @" AND MMPolicyDate = " + GlobalVariable.TO_DATE(MovementDate, true);
            //@" AND IsPhysicalInventory = '" + IsPhysicalInventory + "'";
            VAM_ContainerStorage_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            return VAM_ContainerStorage_ID;
        }

        /// <summary>
        /// this function is used to check all document must be either voided, revesed, closed
        /// </summary>
        /// <returns>TRUE/FALSE</returns>
        private bool VerifyDocumentStatus()
        {
            //int no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAB_Order_ID) FROM VAB_Order WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
            //if (no > 0)
            //{
            //    return false;
            //}

            int no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAM_Inv_InOut_ID) FROM VAM_Inv_InOut WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
            if (no > 0)
            {
                return false;
            }

            no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAM_Inventory_ID) FROM VAM_Inventory WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
            if (no > 0)
            {
                return false;
            }

            no = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAM_InventoryTransfer_ID) FROM VAM_InventoryTransfer WHERE IsActive = 'Y' AND DocStatus NOT IN ('CL' , 'RE' , 'VO')", null, Get_Trx()));
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
        private X_VAM_ContainerStorage InsertContainerStorage(X_VAM_ContainerStorage containerStorage, DataRow dr, bool isPhysicalInventory)
        {
            containerStorage.SetVAF_Client_ID(Convert.ToInt32(dr["VAF_Client_ID"]));
            containerStorage.SetVAF_Org_ID(Convert.ToInt32(dr["VAF_Org_ID"]));
            containerStorage.SetVAM_Locator_ID(Convert.ToInt32(dr["VAM_Locator_ID"]));
            containerStorage.SetVAM_Product_ID(Convert.ToInt32(dr["VAM_Product_ID"]));
            containerStorage.SetVAM_PFeature_SetInstance_ID(Convert.ToInt32(dr["VAM_PFeature_SetInstance_ID"]));

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
        private X_VAM_ContainerStorage InsertContainerStorage(X_VAM_ContainerStorage containerStorage, DataRow dr)
        {
            if (containerStorage.Get_ID() <= 0)
            {
                containerStorage.SetVAF_Client_ID(Convert.ToInt32(dr["VAF_Client_ID"]));
                containerStorage.SetVAF_Org_ID(MLocator.Get(containerStorage.GetCtx(), Convert.ToInt32(dr["VAM_Locator_ID"])).GetVAF_Org_ID());
                containerStorage.SetVAM_Locator_ID(Convert.ToInt32(dr["VAM_Locator_ID"]));
                containerStorage.SetVAM_Product_ID(Convert.ToInt32(dr["VAM_Product_ID"]));
                containerStorage.SetVAM_PFeature_SetInstance_ID(Convert.ToInt32(dr["VAM_PFeature_SetInstance_ID"]));
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
            String sql = @"SELECT VAM_ContainerStorage_ID , Qty , IsPhysicalInventory FROM VAM_ContainerStorage WHERE Qty > 0 
                               AND VAM_Locator_ID = " + Convert.ToInt32(dr["VAM_Locator_ID"]) +
                            @" AND VAM_Product_ID = " + Convert.ToInt32(dr["VAM_Product_ID"]) +
                            @" AND NVL(VAM_PFeature_SetInstance_ID , 0) = " + Convert.ToInt32(dr["VAM_PFeature_SetInstance_ID"]);
            if (Convert.ToString(dr["MMPolicy"]).Equals(MProductCategory.MMPOLICY_LiFo))
            {
                sql += " ORDER BY MMPolicyDate  DESC , VAM_ContainerStorage_ID DESC";
            }
            else
            {
                sql += " ORDER BY MMPolicyDate , VAM_ContainerStorage_ID ASC";
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
                            no = DB.ExecuteQuery("Update VAM_ContainerStorage SET QTY = 0 , IsPhysicalInventory = 'Y' WHERE VAM_ContainerStorage_ID = "
                                                + Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["VAM_ContainerStorage_ID"]), null, Get_Trx());
                        }
                        else
                        {
                            // delete record
                            no = DB.ExecuteQuery("DELETE FROM  VAM_ContainerStorage WHERE VAM_ContainerStorage_ID = "
                                               + Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["VAM_ContainerStorage_ID"]), null, Get_Trx());
                        }

                        // set remaining qty which is to be reduce 
                        Qty = decimal.Subtract(Qty, qtyStorage);
                    }
                    else
                    {
                        // subtract qty from the current traverse record
                        no = DB.ExecuteQuery("Update VAM_ContainerStorage SET QTY = Qty - " + Qty + @" WHERE VAM_ContainerStorage_ID = "
                                                + Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["VAM_ContainerStorage_ID"]), null, Get_Trx());
                        Qty = 0;
                        break;
                    }
                }
            }

            // after consumption, if any qty left than need to create record there with -ve remaining qty
            if (Qty != 0)
            {
                X_VAM_ContainerStorage containerStorage = new X_VAM_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage  
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
        //                int no = DB.ExecuteQuery(@"UPDATE VAM_Inv_Trx SET ContainerCurrentQty = CurrentQty", null, Get_Trx());
        //                if (no > 0)
        //                {
        //                    // delete all record from container storage
        //                    no = DB.ExecuteQuery(@"DELETE FROM  VAM_ContainerStorage", null, Get_Trx());

        //                    // get all data from storage having OnHandQty != 0, and create recod on container storage with locator, product and qty details
        //                    sql.Clear();
        //                    sql.Append(@"WITH conSTORAGE AS
        //  (SELECT il.VAM_Locator_ID, il.VAM_Product_id, il.VAM_PFeature_SetInstance_ID, i.MovementDate, SUM(il.QtyCount - il.QtyBook) AS qty
        //  FROM VAM_Inventory i INNER JOIN VAM_InventoryLine il ON i.VAM_Inventory_id = il.VAM_Inventory_id WHERE i.isinternaluse = 'N' 
        //  GROUP BY il.VAM_Locator_ID, il.VAM_Product_id, il.VAM_PFeature_SetInstance_ID, i.MovementDate  )
        //SELECT VAF_Client_ID, VAF_Org_ID, VAM_Storage.VAM_Locator_ID, VAM_Storage.VAM_Product_ID,  NVL(VAM_Storage.VAM_PFeature_SetInstance_ID , 0) AS VAM_PFeature_SetInstance_ID,
        //  SUM(QtyOnHand) AS QtyOnHand, DateLastInventory, NVL(qty, 0) AS qty
        //FROM VAM_Storage LEFT JOIN conSTORAGE ON ( conSTORAGE.VAM_Locator_id = VAM_Storage.VAM_Locator_id
        //AND conSTORAGE.VAM_Product_id  = VAM_Storage.VAM_Product_id AND NVL(conSTORAGE.VAM_PFeature_SetInstance_ID , 0) = NVL(VAM_Storage.VAM_PFeature_SetInstance_ID , 0)
        //AND conSTORAGE.MovementDate = VAM_Storage.DateLastInventory)
        //GROUP BY VAF_Client_ID, VAF_Org_ID, VAM_Storage.VAM_Locator_ID, VAM_Storage.VAM_Product_ID, VAM_Storage.VAM_PFeature_SetInstance_ID, DateLastInventory, qty
        //HAVING SUM(QtyOnHand) != 0");
        //                    _log.Info(sql.ToString());
        //                    DataSet dsStorage = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
        //                    if (dsStorage != null && dsStorage.Tables.Count > 0 && dsStorage.Tables[0].Rows.Count > 0)
        //                    {
        //                        X_VAM_ContainerStorage containerStorage = null;
        //                        bool isPhysicalInventory = false;
        //                        for (int i = 0; i < dsStorage.Tables[0].Rows.Count; i++)
        //                        {
        //                            // get dataRow from dataset
        //                            DataRow dr = dsStorage.Tables[0].Rows[i];

        //                            // when storage dont have DateLastInventory -- means not any physical inventory
        //                            if (dr["DateLastInventory"] == DBNull.Value)
        //                            {
        //                                isPhysicalInventory = false; // is record save for Physical inventory

        //                                containerStorage = new X_VAM_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage

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
        //                                    containerStorage = new X_VAM_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage

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
        //                                    containerStorage = new X_VAM_ContainerStorage(GetCtx(), 0, Get_Trx()); // object of container storage

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
        //                no = DB.ExecuteQuery("UPDATE VAF_SysConfig SET VALUE = 'Y' WHERE Name='PRODUCT_CONTAINER_APPLICABLE'", null, Get_Trx());
        //            }
        //            else
        //            {
        //                return Msg.GetMsg(GetCtx(), "VIS_AlreadyActivateContainer");
        //            }
        //            return Msg.GetMsg(GetCtx(), "VIS_ActivatedContainer");
        //        }
    }
}
