/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : ImportedDataCompletion
    * Purpose        : Complete Records
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     24-May-2016
******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class ImportedDataCompletion : SvrProcess
    {

        private static VLogger _log = VLogger.GetVLogger(typeof(ImportedDataCompletion).FullName);
        StringBuilder sql = new StringBuilder();
        DataSet dsInOut = new DataSet();
        DataSet dsRecord = new DataSet();
        DataRow[] dataRow = null;
        MInOut inout = null;
        MOrder order = null;
        MInvoice invoice = null;
        MMovement movement = null;
        MInventory inventory = null;

        DateTime? currentDate;
        DateTime? minDateRecord;

        /// <summary>
        /// Prepare Parameters
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {

                }
                else if (name.Equals("ToDate"))
                {
                    currentDate = Util.GetValueOfDateTime(para[i].GetParameter());
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Implement Functionality
        /// </summary>
        /// <returns>String Message</returns>
        protected override string DoIt()
        {
            try
            {
                //DevOps Task ID: 1732 -  min date record from the transaction window
                minDateRecord = SerachMinDate();

                int diff = (int)(Math.Ceiling((currentDate.Value.Date - minDateRecord.Value.Date).TotalDays));

                for (int days = 0; days <= diff; days++)
                {
                    if (days != 0)
                    {
                        minDateRecord = minDateRecord.Value.AddDays(1);
                    }

                    _log.Info("Imported Record completion Start for " + minDateRecord);


                    sql.Clear();
                    sql.Append(@"SELECT * FROM ( 
                         SELECT AD_Client_ID, AD_Org_ID, IsActive, to_char(Created, 'DD-MON-YY HH24:MI:SS') AS Created, CreatedBy, to_char(Updated, 'DD-MON-YY HH24:MI:SS') AS Updated, UpdatedBy,  
                           DocumentNo, C_Order_ID AS Record_Id, IsSOTrx,  IsReturnTrx, '' AS IsInternalUse, 'C_Order' AS TableName,
                           DocStatus, DateOrdered AS DateAcct
                         FROM C_Order WHERE DateAcct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')
                         UNION
                         SELECT AD_Client_ID, AD_Org_ID, IsActive, to_char(Created, 'DD-MON-YY HH24:MI:SS') AS Created, CreatedBy, to_char(Updated, 'DD-MON-YY HH24:MI:SS') AS Updated, UpdatedBy,  
                           DocumentNo, M_InOut_ID AS Record_Id, IsSOTrx,  IsReturnTrx, '' AS IsInternalUse, 'M_InOut' AS TableName,
                           DocStatus, MovementDate AS DateAcct
                         FROM M_InOut WHERE DateAcct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')
                         UNION
                         SELECT AD_Client_ID, AD_Org_ID, IsActive, to_char(Created, 'DD-MON-YY HH24:MI:SS') AS Created, CreatedBy, to_char(Updated, 'DD-MON-YY HH24:MI:SS') AS Updated, UpdatedBy,
                                DocumentNo, C_Invoice_ID AS Record_Id, IsSOTrx, IsReturnTrx, '' AS IsInternalUse, 'C_Invoice' AS TableName,
                                DocStatus, DateAcct AS DateAcct
                         FROM C_Invoice WHERE DateAcct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')
                         UNION 
                         SELECT AD_Client_ID, AD_Org_ID, IsActive, to_char(Created, 'DD-MON-YY HH24:MI:SS') AS Created, CreatedBy, to_char(Updated, 'DD-MON-YY HH24:MI:SS') AS Updated, UpdatedBy,
                                DocumentNo, M_Inventory_ID AS Record_Id, '' AS IsSOTrx, '' AS IsReturnTrx, IsInternalUse, 'M_Inventory' AS TableName,
                                DocStatus, MovementDate AS DateAcct
                         FROM M_Inventory WHERE MovementDate = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')
                         UNION
                         SELECT AD_Client_ID, AD_Org_ID, IsActive, to_char(Created, 'DD-MON-YY HH24:MI:SS') AS Created, CreatedBy, to_char(Updated, 'DD-MON-YY HH24:MI:SS') AS Updated, UpdatedBy, 
                                DocumentNo,  M_Movement_ID AS Record_Id, '' AS IsSOTrx, '' AS IsReturnTrx, '' AS IsInternalUse, 'M_Movement' AS TableName,
                                DocStatus, MovementDate AS DateAcct
                         FROM M_Movement WHERE MovementDate = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')
                         UNION
                         SELECT p.AD_Client_ID, p.AD_Org_ID, p.IsActive, to_char(p.Created, 'DD-MON-YY HH24:MI:SS') AS Created, p.CreatedBy, to_char(p.Updated, 'DD-MON-YY HH24:MI:SS') AS Updated, p.UpdatedBy, 
                                p.name AS DocumentNo, ap.AD_PInstance_ID AS Record_Id, p.IsReversed AS IsSOTrx, '' AS IsReturnTrx, '' AS IsInternalUse, 'M_Production' AS TableName,
                                '' AS DocStatus, p.MovementDate AS DateAcct
                         FROM M_Production p INNER JOIN AD_PInstance ap ON (ap.Record_ID = p.M_Production_ID AND ap.AD_Process_ID = 
                         (SELECT AD_Process_ID FROM AD_Process WHERE Value='M_Production_WithoutFRPT')) WHERE p.MovementDate = " + GlobalVariable.TO_DATE(minDateRecord, true)
                         + @" AND p.IsActive = 'Y' AND p.IsCreated = 'Y' AND p.Processed = 'N'");

                    sql.Append(@" ) t ");
                    if (GetAD_Client_ID() > 0)
                    {
                        sql.Append(" WHERE AD_Client_ID = " + GetAD_Client_ID());
                    }
                    sql.Append(" ORDER BY DateAcct, Created");
                    dsRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());

                    // Complete Record
                    if (dsRecord != null && dsRecord.Tables.Count > 0 && dsRecord.Tables[0].Rows.Count > 0)
                    {
                        for (int z = 0; z < dsRecord.Tables[0].Rows.Count; z++)
                        {
                            #region complete Sales/Purchase Order Record
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "C_Order" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["IsReturnTrx"]) == "N")
                            {
                                order = new MOrder(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                order.CompleteIt();
                                if (order.GetDocAction() == "CL")
                                {
                                    order.SetDocStatus("CO");
                                    order.SetDocAction("CL");
                                    if (!order.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for saving C_Order Record ID = " + order.GetC_Order_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    }
                                    else
                                    {
                                        Get_Trx().Commit();
                                    }
                                }
                                else
                                {
                                    Rollback();
                                    _log.Info("Order not completed for this Record ID = " + order.GetC_Order_ID());
                                    return "Order not completed: " + order.GetDocumentNo() + " - " + order.GetProcessMsg();
                                }
                            }
                            #endregion

                            #region complete Invoice Record
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "C_Invoice")
                            {
                                invoice = new MInvoice(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                invoice.CompleteIt();
                                if (invoice.GetDocAction() == "CL")
                                {
                                    invoice.SetDocStatus("CO");
                                    invoice.SetDocAction("CL");
                                    if (!invoice.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for saving C_Invoice Record ID = " + invoice.GetC_Invoice_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    }
                                    else
                                    {
                                        Get_Trx().Commit();
                                    }
                                }
                                else
                                {
                                    Rollback();
                                    _log.Info("Invoice not completed for this Record ID = " + invoice.GetC_Invoice_ID());
                                    return "Invoice not completed: " + invoice.GetDocumentNo() + " - " + invoice.GetProcessMsg();
                                }
                            }
                            #endregion

                            #region complete material receipt/ Shipment
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["IsReturnTrx"]) == "N")
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                inout.CompleteIt();
                                if (inout.GetDocAction() == "CL")
                                {
                                    inout.SetDocStatus("CO");
                                    inout.SetDocAction("CL");
                                    if (!inout.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for saving M_InOut Record ID = " + inout.GetM_InOut_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    }
                                    else
                                    {
                                        Get_Trx().Commit();
                                    }
                                }
                                else
                                {
                                    Rollback();
                                    _log.Info("Shipment/Material Receipt not completed for this Record ID = " + inout.GetM_InOut_ID());
                                    return "Shipment/Receipt not completed: " + inout.GetDocumentNo() + " - " + inout.GetProcessMsg();
                                }
                            }
                            #endregion

                            #region complete Movement Record
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Movement")
                            {
                                movement = new MMovement(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                movement.CompleteIt();
                                if (movement.GetDocAction() == "CL")
                                {
                                    movement.SetDocStatus("CO");
                                    movement.SetDocAction("CL");
                                    if (!movement.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for saving C_Invoice Record ID = " + movement.GetM_Movement_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    }
                                    else
                                    {
                                        Get_Trx().Commit();
                                    }
                                }
                                else
                                {
                                    Rollback();
                                    _log.Info("Movement not completed for this Record ID = " + movement.GetM_Movement_ID());
                                    return "Movement not completed: " + movement.GetDocumentNo() + " - " + movement.GetProcessMsg();
                                }
                            }
                            #endregion

                            #region complete Inventory Record
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Inventory")
                            {
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                inventory.CompleteIt();
                                if (inventory.GetDocAction() == "CL")
                                {
                                    inventory.SetDocStatus("CO");
                                    inventory.SetDocAction("CL");
                                    if (!inventory.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for saving C_Invoice Record ID = " + inventory.GetM_Inventory_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    }
                                    else
                                    {
                                        Get_Trx().Commit();
                                    }
                                }
                                else
                                {
                                    Rollback();
                                    _log.Info("Inventory not completed for this Record ID = " + inventory.GetM_Inventory_ID());
                                    return "Inventory not completed: " + inventory.GetDocumentNo() + " - " + inventory.GetProcessMsg();
                                }
                            }
                            #endregion

                            #region complete Customer/Vendor Return
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["IsReturnTrx"]) == "Y")
                            {
                                inout = new MInOut(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                inout.CompleteIt();
                                if (inout.GetDocAction() == "CL")
                                {
                                    inout.SetDocStatus("CO");
                                    inout.SetDocAction("CL");
                                    if (!inout.Save(Get_Trx()))
                                    {
                                        Rollback();
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for saving C_Order Record ID = " + inout.GetM_InOut_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    }
                                    else
                                    {
                                        Get_Trx().Commit();
                                    }
                                }
                                else
                                {
                                    Rollback();
                                    _log.Info("Customer/Vendor return not completed for this Record ID = " + inout.GetM_InOut_ID());
                                    return "Customer/Vendor return not completed: " + inout.GetDocumentNo() + " - " + inout.GetProcessMsg();
                                }
                            }
                            #endregion

                            #region Complete Production
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Production")
                            {
                                #region Create & Open connection and Execute Procedure
                                // execute procedure for calculating cost
                                SqlParameter[] param = new SqlParameter[1];
                                param[0] = new SqlParameter("pinstance_id", Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_ID"]));
                                param[0].SqlDbType = SqlDbType.Int;
                                param[0].Direction = ParameterDirection.Input;

                                DB.ExecuteProcedure("M_PRODUCTION_RUN_NotFRPT", param, Get_Trx());
                                string error = Util.GetValueOfString(DB.ExecuteScalar("SELECT ErrorMsg FROM AD_PInstance WHERE AD_PInstance_ID = "
                                    + Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_ID"]), null, Get_Trx()));
                                if (!String.IsNullOrEmpty(error))
                                {
                                    Rollback();
                                    _log.Info("Production not completed for this Record ID = " + Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["DocumentNo"]));
                                    return "Production not completed: " + Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["DocumentNo"]) + " - " + error;
                                }
                                else
                                {
                                    Get_Trx().Commit();
                                }
                                #endregion                                
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Rollback();
                _log.Info("Error Occured during completion of record by using  ImportedDataCompletion Process - " + ex.ToString());
                return Msg.GetMsg(GetCtx(), "NotCompleted") + ex.ToString();
            }
            return Msg.GetMsg(GetCtx(), "SucessfullyCompleted");
        }

        /// <summary>
        /// Search Minimum Date from Transactions
        /// </summary>
        /// <returns>Minimum Transaction Date</returns>
        public DateTime? SerachMinDate()
        {
            DateTime? minDate;
            DateTime? tempDate;
            try
            {
                sql.Clear();
                sql.Append("SELECT Min(MovementDate) FROM M_Inventory WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')");
                minDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                sql.Clear();
                sql.Append("SELECT Min(MovementDate) FROM M_Movement WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')");
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                // Production 
                sql.Clear();
                sql.Append(@"SELECT Min(MovementDate) FROM M_Production WHERE IsActive = 'Y' AND IsCreated = 'Y'");
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                //Order
                sql.Clear();
                sql.Append("SELECT Min(DateAcct) FROM C_Order WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')");
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                //Invoice
                sql.Clear();
                sql.Append("SELECT Min(DateAcct) FROM C_Invoice WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')");
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                //Shipment/Receipt
                sql.Clear();
                sql.Append("SELECT Min(DateAcct) FROM M_InOut WHERE IsActive = 'Y' AND DocStatus NOT IN ('CO', 'CL', 'RE', 'VO')");
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                //if (count > 0)
                //{
                //    try
                //    {
                //        // when Manufacuring Module is downloaded
                //        sql.Clear();
                //        sql.Append(@"SELECT Min(VAMFG_DateAcct) FROM VAMFG_M_WrkOdrTransaction WHERE VAMFG_WorkOrderTxnType IN ('CI', 'CR') AND  IsActive = 'Y' AND
                //             ((DocStatus IN ('CO', 'CL')");
                //        tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                //    }
                //    catch { }
                //    if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                //    {
                //        minDate = tempDate;
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return minDate;
        }
    }
}
