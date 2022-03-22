/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : ReCostingCalculation
    * Purpose        : Re Calculate Cost (Product category / Products wise)
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Amit Bansal     08-June-2018
******************************************************/


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//using System.Data.OracleClient;

using System.Linq;
using System.Reflection;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class ReCostingCalculation : SvrProcess
    {
        private StringBuilder sql = new StringBuilder();
        private static VLogger _log = VLogger.GetVLogger(typeof(ReCostingCalculation).FullName);

        //Parameters
        string productCategoryID = null;
        string productID = null;
        String onlyDeleteCosting = "N";

        // for load assembly
        private static Assembly asm = null;
        private static Type type = null;
        private static MethodInfo methodInfo = null;

        DateTime? currentDate = DateTime.Now;
        DateTime? minDateRecord;

        DataSet dsRecord = null;
        DataSet dsChildRecord = null;

        Decimal quantity = 0;
        decimal currentCostPrice = 0;
        decimal amt = 0;
        // Order Line amt included (taxable amt + tax amt + surcharge amt)
        Decimal ProductOrderLineCost = 0;
        // Order Line Price actual
        Decimal ProductOrderPriceActual = 0;
        // Invoice Line amt included (taxable amt + tax amt + surcharge amt)
        Decimal ProductInvoiceLineCost = 0;

        MClient client = null;

        MInventory inventory = null;
        MInventoryLine inventoryLine = null;

        MMovement movement = null;
        MMovementLine movementLine = null;
        //MWarehouse warehouse = null;
        MLocator locatorTo = null; // is used to get "to warehouse" reference and "to org" reference for getting cost from prodyc costs 
        Decimal toCurrentCostPrice = 0; // is used to maintain cost of "move to" 

        MInOut inout = null;
        MInOutLine inoutLine = null;
        MOrderLine orderLine = null;
        MOrder order = null;

        MInvoice invoice = null;
        MInvoiceLine invoiceLine = null;
        bool isCostAdjustableOnLost = false;
        MLandedCostAllocation landedCostAllocation = null;

        MProvisionalInvoice provisionalInvoice = null;
        MProvisionalInvoiceLine provisionalInvoiceLine = null;

        MProduct product = null;

        MMatchInv matchInvoice = null;
        X_M_MatchInvCostTrack matchInvCostReverse = null;

        int table_WrkOdrTrnsctionLine = 0;
        MTable tbl_WrkOdrTrnsctionLine = null;
        int table_WrkOdrTransaction = 0;
        MTable tbl_WrkOdrTransaction = null;
        PO po_WrkOdrTransaction = null;
        PO po_WrkOdrTrnsctionLine = null;
        String woTrxType = null;
        int table_AssetDisposal = 0;
        MTable tbl_AssetDisposal = null;
        PO po_AssetDisposal = null;

        //Production
        int CountCostNotAvialable = 1;

        int countColumnExist = 0; //check IsCostAdjustmentOnLost exist on product 
        int countGOM01 = 0;  // check Gomel Modeule exist or not
        int count = 0; //check Manufacturing Modeule exist or not

        string conversionNotFoundInvoice = "";
        string conversionNotFoundInOut = "";
        string conversionNotFoundInventory = "";
        string conversionNotFoundMovement = "";
        string conversionNotFoundProductionExecution = "";
        string conversionNotFoundInvoice1 = "";
        string conversionNotFoundInOut1 = "";
        string conversionNotFoundInventory1 = "";
        string conversionNotFoundMovement1 = "";
        string conversionNotFoundProductionExecution1 = "";
        string conversionNotFound = "";
        string conversionNotFoundProvisionalInvoice = "";

        List<ReCalculateRecord> ListReCalculatedRecords = new List<ReCalculateRecord>();
        public enum windowName { Shipment = 0, CustomerReturn, ReturnVendor, MaterialReceipt, MatchInvoice, Inventory, Movement, CreditMemo, AssetDisposal }

        private String costingMethod = string.Empty;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("M_Product_Category_ID"))
                {
                    productCategoryID = Util.GetValueOfString(para[i].GetParameter());
                    //int[] productCategoryArr = Array.ConvertAll(productCategoryID.Split(','), int.Parse);
                }
                else if (name.Equals("M_Product_ID"))
                {
                    productID = Util.GetValueOfString(para[i].GetParameter());
                }
                else if (name.Equals("IsDeleteCosting"))
                {
                    onlyDeleteCosting = (string)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        protected override string DoIt()
        {
            try
            {
                _log.Info("RE Cost Calculation Start on " + DateTime.Now);

                // check Manufacturing Modeule exist or not
                //int count = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VAMFG_' AND Isactive = 'Y' "));
                count = Env.IsModuleInstalled("VAMFG_") ? 1 : 0;

                // check VAFAM Modeule exist or not
                int countVAFAM = Env.IsModuleInstalled("VAFAM_") ? 1 : 0;

                // check Gomel Modeule exist or not
                //int countGOM01 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='GOM01_' AND Isactive = 'Y' "));
                countGOM01 = Env.IsModuleInstalled("GOM01_") ? 1 : 0;

                // update / delete query
                UpdateAndDeleteCostImpacts(count);

                // when user not selected anything, then not to calculate cost, it will be calculated by Costing Calculation process
                if (String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID) && onlyDeleteCosting.Equals("N"))
                    return Msg.GetMsg(GetCtx(), "VIS_NoImpacts");

                if (onlyDeleteCosting.Equals("Y"))
                    return Msg.GetMsg(GetCtx(), "VIS_DeleteCostingImpacts");

                // check IsCostAdjustmentOnLost exist on product 
                sql.Clear();
                sql.Append(@"SELECT COUNT(*) FROM AD_Column WHERE IsActive = 'Y' AND 
                                       AD_Table_ID =  ( SELECT AD_Table_ID FROM AD_Table WHERE IsActive = 'Y' AND TableName LIKE 'M_Product' ) 
                                       AND ColumnName = 'IsCostAdjustmentOnLost' ");
                countColumnExist = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
                sql.Clear();

                // min date record from the transaction window
                minDateRecord = SerachMinDate(count);

                if (count > 0)
                {
                    table_WrkOdrTransaction = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_TABLE_ID  FROM AD_TABLE WHERE tablename = 'VAMFG_M_WrkOdrTransaction' AND IsActive = 'Y' "));
                    tbl_WrkOdrTransaction = new MTable(GetCtx(), table_WrkOdrTransaction, null);

                    table_WrkOdrTrnsctionLine = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_TABLE_ID  FROM AD_TABLE WHERE tablename = 'VAMFG_M_WrkOdrTrnsctionLine' AND IsActive = 'Y' "));
                    tbl_WrkOdrTrnsctionLine = new MTable(GetCtx(), table_WrkOdrTrnsctionLine, null);
                }
                if (countVAFAM > 0)
                {
                    table_AssetDisposal = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_TABLE_ID  FROM AD_TABLE WHERE tablename = 'VAFAM_AssetDisposal' AND IsActive = 'Y' "));
                    tbl_AssetDisposal = new MTable(GetCtx(), table_AssetDisposal, null);
                }

                int diff = (int)(Math.Ceiling((DateTime.Now.Date - minDateRecord.Value.Date).TotalDays));

                for (int days = 0; days <= diff; days++)
                {
                    if (days != 0)
                    {
                        minDateRecord = minDateRecord.Value.AddDays(1);
                    }

                    _log.Info("RE Cost Calculation Start for " + minDateRecord);
                    var pc = "(SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )";
                    sql.Clear();
                    sql.Append(@"SELECT * FROM (
                        SELECT ad_client_id , ad_org_id , isactive , to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby , to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby ,  
                               documentno , m_inout_id AS Record_Id , issotrx ,  isreturntrx , ''  AS IsInternalUse, 'M_InOut' AS TableName,
                               docstatus, movementdate AS DateAcct , iscostcalculated , isreversedcostcalculated 
                         FROM M_InOut WHERE dateacct   = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND isactive = 'Y'
                               AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N' ) OR (docstatus IN ('RE') AND iscostcalculated = 'Y'
                               AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))
                         UNION
                         SELECT ad_client_id , ad_org_id , isactive ,to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby ,  to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby ,
                                documentno , C_Invoice_id AS Record_Id , issotrx , isreturntrx , '' AS IsInternalUse, 'C_Invoice' AS TableName,
                                docstatus, DateAcct AS DateAcct, iscostcalculated , isreversedcostcalculated 
                         FROM C_Invoice WHERE dateacct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND isactive     = 'Y'
                              AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N' ) OR (docstatus  IN ('RE') AND iscostcalculated = 'Y'
                              AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%')) 
                         UNION 
                         SELECT i.ad_client_id ,  i.ad_org_id , i.isactive ,to_char(i.created, 'DD-MON-YY HH24:MI:SS') as  created ,  i.createdby ,  TO_CHAR(mi.updated, 'DD-MON-YY HH24:MI:SS') AS updated ,
                                i.updatedby ,  mi.documentno ,  M_MatchInv_Id AS Record_Id ,  i.issotrx ,  i.isreturntrx ,  ''           AS IsInternalUse,  'M_MatchInv' AS TableName,
                                i.docstatus,i.DateAcct AS DateAcct,  mi.iscostcalculated ,  i.isreversedcostcalculated
                         FROM M_MatchInv mi INNER JOIN c_invoiceline il ON il.c_invoiceline_id = mi.c_invoiceline_id INNER JOIN C_Invoice i ON i.c_invoice_id       = il.c_invoice_id
                              WHERE mi.M_Product_ID IN ( " + ((!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID)) ? pc : productID) + @" )
                              AND mi.dateacct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND i.isactive        = 'Y' AND (i.docstatus       IN ('CO' , 'CL') AND mi.iscostcalculated = 'N' )

                         UNION 
                         SELECT i.ad_client_id ,  i.ad_org_id , i.isactive ,to_char(mi.created, 'DD-MON-YY HH24:MI:SS') as  created ,  i.createdby ,  TO_CHAR(mi.updated, 'DD-MON-YY HH24:MI:SS') AS updated ,
                                i.updatedby ,  null AS documentno ,  M_MatchInvCostTrack_Id AS Record_Id ,  i.issotrx ,  i.isreturntrx ,  ''           AS IsInternalUse,  'M_MatchInvCostTrack' AS TableName,
                                i.docstatus, i.DateAcct AS DateAcct,  mi.iscostcalculated ,  mi.iscostimmediate AS isreversedcostcalculated
                          FROM M_MatchInvCostTrack mi INNER JOIN c_invoiceline il ON il.c_invoiceline_id = mi.c_invoiceline_id INNER JOIN C_Invoice i ON i.c_invoice_id       = il.c_invoice_id
                          WHERE  mi.M_Product_ID IN ( " + ((!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID)) ? pc : productID) + @" )
                           AND mi.updated >= " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND mi.updated < " + GlobalVariable.TO_DATE(minDateRecord.Value.AddDays(1), true) + @"  AND i.isactive        = 'Y' AND (i.docstatus       IN ('RE' , 'VO') )

                        UNION
                         SELECT DISTINCT il.ad_client_id ,il.ad_org_id ,il.isactive ,to_char(LCA.created, 'DD-MON-YY HH24:MI:SS') as created ,   i.createdby , TO_CHAR(i.updated, 'DD-MON-YY HH24:MI:SS') AS updated ,
                              i.updatedby , I.DOCUMENTNO , LCA.C_LANDEDCOSTALLOCATION_ID AS RECORD_ID ,   '' AS ISSOTRX , '' AS ISRETURNTRX , '' AS ISINTERNALUSE,
                              'LandedCost' as TABLENAME,  I.DOCSTATUS, DATEACCT as DATEACCT , LCA.ISCOSTCALCULATED , 'N' as ISREVERSEDCOSTCALCULATED
                        FROM C_LANDEDCOSTALLOCATION LCA INNER JOIN C_INVOICELINE IL ON IL.C_INVOICELINE_ID = LCA.C_INVOICELINE_ID
                        INNER JOIN c_invoice i ON I.C_INVOICE_ID = IL.C_INVOICE_ID
                        WHERE i.dateacct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND il.isactive     = 'Y' AND 
                        LCA.M_PRODUCT_ID IN ( " + ((!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID)) ? pc : productID) + @" ) 
                        AND lca.iscostcalculated = 'N' AND I.DOCSTATUS IN ('CO' , 'CL', 'RE', 'VO') AND I.ISSOTRX = 'N' AND I.ISRETURNTRX = 'N' 

                         UNION
                         SELECT ad_client_id , ad_org_id , isactive ,to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby ,  to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby ,
                                documentno , m_Inventory_id AS Record_Id , '' AS issotrx , '' AS isreturntrx , IsInternalUse, 'M_Inventory' AS TableName,
                                docstatus, movementdate AS DateAcct ,  iscostcalculated ,  isreversedcostcalculated 
                         FROM m_Inventory WHERE movementdate = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND isactive       = 'Y'
                              AND ((docstatus   IN ('CO' , 'CL') AND iscostcalculated = 'N' ) OR (docstatus IN ('RE') AND iscostcalculated = 'Y'
                              AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%')) 
                         UNION
                         SELECT ad_client_id , ad_org_id , isactive ,to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby ,  to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby , 
                                documentno ,  M_Movement_id AS Record_Id , '' AS issotrx , ''  AS isreturntrx , ''  AS IsInternalUse,  'M_Movement'  AS TableName,
                                docstatus,  movementdate AS DateAcct ,  iscostcalculated ,  isreversedcostcalculated 
                         FROM M_Movement WHERE movementdate = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND isactive       = 'Y'
                               AND ((docstatus   IN ('CO' , 'CL') AND iscostcalculated = 'N' ) OR (docstatus IN ('RE') AND iscostcalculated        = 'Y'
                               AND ISREVERSEDCOSTCALCULATED= 'N' AND description LIKE '%{->%'))
                         UNION
                         SELECT ad_client_id , ad_org_id , isactive ,to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby ,  to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby , 
                                name AS documentno ,  M_Production_ID AS Record_Id , IsReversed AS issotrx , ''  AS isreturntrx , ''  AS IsInternalUse,  'M_Production'  AS TableName,
                                '' AS docstatus, movementdate AS DateAcct ,  iscostcalculated ,  isreversedcostcalculated 
                         FROM M_Production WHERE movementdate = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND isactive       = 'Y'
                               AND ((PROCESSED = 'Y' AND iscostcalculated = 'N' AND IsReversed = 'N' ) OR (PROCESSED = 'Y' AND iscostcalculated  = 'Y'
                               AND ISREVERSEDCOSTCALCULATED= 'N' AND IsReversed = 'Y' AND Name LIKE '%{->%'))");
                    sql.Append(@"UNION 
                         SELECT ad_client_id , ad_org_id , isactive , to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby ,  to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby ,
                                documentno , C_ProvisionalInvoice_id AS Record_Id , issotrx , isreturntrx , '' AS IsInternalUse, 'C_ProvisionalInvoice' AS TableName,
                                docstatus, DateAcct AS DateAcct, iscostcalculated , isreversedcostcalculated 
                         FROM C_ProvisionalInvoice WHERE dateacct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND isactive     = 'Y'
                              AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N' ) OR (docstatus  IN ('RE') AND iscostcalculated = 'Y'
                              AND ISREVERSEDCOSTCALCULATED= 'N' AND IsReversal ='Y')) ");
                    if (count > 0)
                    {
                        sql.Append(@" UNION
                         SELECT ad_client_id , ad_org_id , isactive ,to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby ,  to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby , 
                                DOCUMENTNO ,  VAMFG_M_WrkOdrTransaction_id AS Record_Id ,  
                                 CASE WHEN  " + (String.IsNullOrEmpty(productID) ? "1 = 0" : "1 = 1") + " AND (SELECT COUNT(*) FROM m_product WHERE m_product_category_ID IN (" + (String.IsNullOrEmpty(productCategoryID) ? 0.ToString() : productCategoryID) + @") 
                                            AND m_product_id = VAMFG_M_WrkOdrTransaction.m_product_id) > 0 THEN 'Y'
                                      WHEN (SELECT COUNT(*) FROM m_product WHERE m_product_ID IN (" + (String.IsNullOrEmpty(productID) ? 0.ToString() : productID) + @") 
                                            AND m_product_id = VAMFG_M_WrkOdrTransaction.m_product_id) > 0 THEN 'Y' END AS issotrx ,
                                '' AS isreturntrx  , '' AS IsInternalUse,  'VAMFG_M_WrkOdrTransaction'  AS TableName,
                                docstatus, vamfg_dateacct AS DateAcct , iscostcalculated ,  isreversedcostcalculated 
                         FROM VAMFG_M_WrkOdrTransaction WHERE VAMFG_WorkOrderTxnType IN ('CI', 'CR' , 'AR' , 'AI') AND vamfg_dateacct = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" 
                              AND isactive  = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N' ) OR (docstatus IN ('RE') AND iscostcalculated = 'Y'
                              AND ISREVERSEDCOSTCALCULATED  = 'N' AND VAMFG_description LIKE '%{->%')) ");
                    }
                    if (countVAFAM > 0)
                    {
                        sql.Append(@" UNION
                        SELECT ad_client_id , ad_org_id , isactive ,to_char(created, 'DD-MON-YY HH24:MI:SS') as created , createdby ,  to_char(updated, 'DD-MON-YY HH24:MI:SS') as updated , updatedby ,
                                documentno , VAFAM_AssetDisposal_ID AS Record_Id , '' AS issotrx , '' AS isreturntrx ,'' AS IsInternalUse ,'VAFAM_AssetDisposal' AS TableName,
                                docstatus , vafam_trxdate AS DateAcct ,  iscostcalculated ,  isreversedcostcalculated 
                         FROM VAFAM_AssetDisposal WHERE vafam_trxdate = " + GlobalVariable.TO_DATE(minDateRecord, true) + @" AND isactive = 'Y'
                              AND ((docstatus   IN ('CO' , 'CL') AND iscostcalculated = 'N' ) OR (docstatus IN ('RE' , 'VO') AND iscostcalculated ='Y'
                              AND ISREVERSEDCOSTCALCULATED= 'N' AND ReversalDoc_ID!= 0) )");
                    }
                    sql.Append(@" ) t WHERE AD_Client_ID = " + GetCtx().GetAD_Client_ID() + "  order by dateacct , created");
                    dsRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());

                    // Complete Record
                    if (dsRecord != null && dsRecord.Tables.Count > 0 && dsRecord.Tables[0].Rows.Count > 0)
                    {
                        for (int z = 0; z < dsRecord.Tables[0].Rows.Count; z++)
                        {
                            // for checking - costing calculate on completion or not
                            // IsCostImmediate = true - calculate cost on completion else through process
                            client = MClient.Get(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["ad_client_id"]));

                            #region Cost Calculation For Material Receipt --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "N" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL"))
                                {

                                    CalculateCostForMaterial(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Provisional Invoice
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]).Equals("C_ProvisionalInvoice"))
                            {
                                provisionalInvoice = new MProvisionalInvoice(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());

                                sql.Clear();
                                sql.Append(@"SELECT * FROM C_ProvisionalInvoiceLine WHERE IsActive = 'Y' 
                                                AND " + (provisionalInvoice.IsReversal() ? " iscostcalculated = 'Y' AND IsReversedCostCalculated = 'N' "
                                                : " iscostcalculated = 'N' ") +
                                                " AND C_ProvisionalInvoice_ID = " + provisionalInvoice.GetC_ProvisionalInvoice_ID());
                                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                                {
                                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                                }
                                else
                                {
                                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                                }
                                sql.Append(" ORDER BY Line ");
                                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                    {
                                        provisionalInvoiceLine = new MProvisionalInvoiceLine(GetCtx(), dsChildRecord.Tables[0].Rows[j], Get_Trx());
                                        if (client.IsCostImmediate() && provisionalInvoiceLine.GetM_Product_ID() > 0)
                                        {
                                            if (!provisionalInvoice.CostingCalculation(client, provisionalInvoiceLine, true))
                                            {
                                                if (!conversionNotFoundProvisionalInvoice.Contains(provisionalInvoice.GetDocumentNo()))
                                                {
                                                    conversionNotFoundProvisionalInvoice += provisionalInvoice.GetDocumentNo() + " , ";
                                                }
                                                _log.Info("Cost not Calculated for Provisional Invoice for this Line ID = " + provisionalInvoiceLine.GetC_ProvisionalInvoiceLine_ID());
                                            }
                                            else
                                            {
                                                _log.Fine("Cost Calculation updated for C_ProvisionalLine = " + provisionalInvoiceLine.GetC_ProvisionalInvoiceLine_ID());
                                                Get_Trx().Commit();
                                            }
                                        }
                                    }

                                    // update Provisional Invoice Header
                                    sql.Clear();
                                    sql.Append(@"SELECT COUNT(C_ProvisionalInvoiceLine_ID) FROM C_ProvisionalInvoiceLine
                                        WHERE " + (provisionalInvoice.IsReversal() ? "IsReversedCostCalculated = 'N'" : "IsCostCalculated = 'N'") + @"
                                         AND IsActive = 'Y' AND C_ProvisionalInvoice_ID = " + provisionalInvoice.GetC_ProvisionalInvoice_ID());
                                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
                                    {
                                        //Update  IsCostCalculated, IsReversedCostCalculated  as True on Header
                                        DB.ExecuteQuery(@"UPDATE C_ProvisionalInvoice SET IsCostCalculated = 'Y' "
                                          + (provisionalInvoice.IsReversal() ? ", IsReversedCostCalculated = 'Y'" : "") + @"
                                          WHERE C_ProvisionalInvoice_ID = " + provisionalInvoice.GetC_ProvisionalInvoice_ID(), null, Get_Trx());

                                        _log.Fine("Cost Calculation updated for C_provisionalInvoice_ID = " + provisionalInvoice.GetC_ProvisionalInvoice_ID());
                                        Get_Trx().Commit();
                                    }
                                    continue;
                                }
                            }
                            #endregion

                            #region Cost Calculation for SO / PO / CRMA / VRMA
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "C_Invoice" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL"))
                                {
                                    invoice = new MInvoice(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());

                                    sql.Clear();
                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                    {
                                        sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'N' " +
                                                     " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                        if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                                        {
                                            sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                                        }
                                        else
                                        {
                                            sql.Append(" AND M_Product_ID IN (" + productID + " )");
                                        }
                                        sql.Append(" ORDER BY Line");
                                    }
                                    else
                                    {
                                        sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                                                     " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                        if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                                        {
                                            sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                                        }
                                        else
                                        {
                                            sql.Append(" AND M_Product_ID IN (" + productID + " )");
                                        }
                                        sql.Append(" ORDER BY Line");
                                    }
                                    dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                    if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                    {
                                        for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                        {
                                            try
                                            {
                                                product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                                invoiceLine = new MInvoiceLine(GetCtx(), dsChildRecord.Tables[0].Rows[j], Get_Trx());
                                                if (invoiceLine != null && invoiceLine.Get_ID() > 0)
                                                {
                                                    ProductInvoiceLineCost = invoiceLine.GetProductLineCost(invoiceLine, true);
                                                }
                                                if (invoiceLine != null && invoiceLine.GetC_Invoice_ID() > 0 && invoiceLine.GetQtyInvoiced() == 0)
                                                    continue;
                                                if (invoiceLine.GetC_OrderLine_ID() > 0)
                                                {
                                                    if (invoiceLine.GetC_Charge_ID() > 0)
                                                    {
                                                        #region Landed Cost Allocation
                                                        if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null,
                                                                0, "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        #region for Expense type product
                                                        if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                                 "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region  for Item Type product
                                                        else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                        {
                                                            if (countColumnExist > 0)
                                                            {
                                                                isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                                                            }
                                                            MOrderLine ol1 = null;
                                                            MOrder order1 = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());
                                                            ol1 = new MOrderLine(GetCtx(), invoiceLine.GetC_OrderLine_ID(), Get_Trx());
                                                            if (ol1 != null && ol1.Get_ID() > 0)
                                                            {
                                                                ProductOrderLineCost = ol1.GetProductLineCost(ol1);
                                                                ProductOrderPriceActual = ProductOrderLineCost / ol1.GetQtyEntered();
                                                            }
                                                            if (order1.GetC_Order_ID() == 0)
                                                            {
                                                                //ol1 = new MOrderLine(GetCtx(), invoiceLine.GetC_OrderLine_ID(), Get_Trx());
                                                                order1 = new MOrder(GetCtx(), ol1.GetC_Order_ID(), Get_Trx());
                                                            }

                                                            #region  Sales Cycle
                                                            if (order1.IsSOTrx() && !order1.IsReturnTrx())
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                      "Invoice(Customer)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                      Get_Trx(), out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                                else
                                                                {
                                                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                    {
                                                                        invoiceLine.SetIsReversedCostCalculated(true);
                                                                    }
                                                                    if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                    {
                                                                        DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                    FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                    WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                        {
                                                                            invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                            currentCostPrice = MCost.GetproductCosts(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                       invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                       Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]));
                                                                            invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                        }
                                                                    }
                                                                    invoiceLine.SetIsCostCalculated(true);
                                                                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                    {
                                                                        invoiceLine.SetIsCostImmediate(true);
                                                                    }
                                                                    if (!invoiceLine.Save(Get_Trx()))
                                                                    {
                                                                        ValueNamePair pp = VLogger.RetrieveError();
                                                                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                        Get_Trx().Rollback();
                                                                    }
                                                                    else
                                                                    {
                                                                        _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                        Get_Trx().Commit();
                                                                    }
                                                                }
                                                            }
                                                            #endregion

                                                            #region Purchase Cycle (not to be executed)
                                                            else if (!order1.IsSOTrx() && !order1.IsReturnTrx() && 0 == 1)
                                                            {
                                                                // calculate cost of MR first if not calculate which is linked with that invoice line
                                                                if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                {
                                                                    inoutLine = new MInOutLine(GetCtx(), invoiceLine.GetM_InOutLine_ID(), Get_Trx());
                                                                    if (!inoutLine.IsCostCalculated())
                                                                    {
                                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                                    "Material Receipt", null, inoutLine, null, invoiceLine, null,
                                                                    order1 != null && order1.GetDocStatus() != "VO" ? Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, ol1.GetQtyOrdered()), inoutLine.GetMovementQty())
                                                                    : Decimal.Multiply(ProductOrderPriceActual, inoutLine.GetQtyEntered()),
                                                                    inoutLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut))
                                                                        {
                                                                            if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                                            {
                                                                                conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                                            }
                                                                            _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                                            continue;
                                                                        }
                                                                        else
                                                                        {
                                                                            inoutLine.SetIsCostCalculated(true);
                                                                            if (!inoutLine.Save(Get_Trx()))
                                                                            {
                                                                                ValueNamePair pp = VLogger.RetrieveError();
                                                                                _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                           " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                                Get_Trx().Rollback();
                                                                                continue;
                                                                            }
                                                                            else
                                                                            {
                                                                                Get_Trx().Commit();
                                                                            }
                                                                        }
                                                                    }

                                                                    // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                          "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost,
                                                                          countColumnExist > 0 && isCostAdjustableOnLost && invoiceLine.GetM_InOutLine_ID() > 0 && inoutLine.GetMovementQty() < invoiceLine.GetQtyInvoiced() ? inoutLine.GetMovementQty() : invoiceLine.GetQtyInvoiced(),
                                                                          Get_Trx(), out conversionNotFoundInvoice))
                                                                    {
                                                                        if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                        {
                                                                            conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                        }
                                                                        _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    }
                                                                    else
                                                                    {
                                                                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                        {
                                                                            invoiceLine.SetIsReversedCostCalculated(true);
                                                                        }
                                                                        invoiceLine.SetIsCostCalculated(true);
                                                                        if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                        {
                                                                            invoiceLine.SetIsCostImmediate(true);
                                                                        }
                                                                        if (!invoiceLine.Save(Get_Trx()))
                                                                        {
                                                                            ValueNamePair pp = VLogger.RetrieveError();
                                                                            _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                            Get_Trx().Rollback();
                                                                        }
                                                                        else
                                                                        {
                                                                            _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                            Get_Trx().Commit();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            #endregion

                                                            #region CRMA
                                                            else if (order1.IsSOTrx() && order1.IsReturnTrx())
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Customer)", null, null, null, invoiceLine, null, ProductInvoiceLineCost,
                                                                  invoiceLine.GetQtyInvoiced(), Get_Trx(), out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                                else
                                                                {
                                                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                    {
                                                                        invoiceLine.SetIsReversedCostCalculated(true);
                                                                    }
                                                                    if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                    {
                                                                        DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                    FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                    WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                        {
                                                                            invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                            currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                       invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                       Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]), false);
                                                                            invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                        }
                                                                    }
                                                                    invoiceLine.SetIsCostCalculated(true);
                                                                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                    {
                                                                        invoiceLine.SetIsCostImmediate(true);
                                                                    }
                                                                    if (!invoiceLine.Save(Get_Trx()))
                                                                    {
                                                                        ValueNamePair pp = VLogger.RetrieveError();
                                                                        _log.Info("Error found for saving Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                        Get_Trx().Rollback();
                                                                    }
                                                                    else
                                                                    {
                                                                        _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                        Get_Trx().Commit();
                                                                    }
                                                                }
                                                            }
                                                            #endregion

                                                            #region VRMA
                                                            else if (!order1.IsSOTrx() && order1.IsReturnTrx())
                                                            {
                                                                //change 12-5-2016
                                                                // when Ap Credit memo is alone then we will do a impact on costing.
                                                                // this is bcz of giving discount for particular product
                                                                // discount is given only when document type having setting as "Treat As Discount" = True
                                                                MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                                if (docType.GetDocBaseType() == "APC" && docType.IsTreatAsDiscount() && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                                {
                                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                      "Invoice(Vendor)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                      Get_Trx(), out conversionNotFoundInvoice))
                                                                    {
                                                                        if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                        {
                                                                            conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                        }
                                                                        _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    }
                                                                    else
                                                                    {
                                                                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                        {
                                                                            invoiceLine.SetIsReversedCostCalculated(true);
                                                                        }
                                                                        if (invoiceLine.Get_ColumnIndex("PostCurrentCostPrice") >= 0 && invoiceLine.GetPostCurrentCostPrice() == 0)
                                                                        {
                                                                            // get post cost after invoice cost calculation and update on invoice
                                                                            currentCostPrice = MCost.GetproductCosts(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                                            product.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                                                                            invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                        }
                                                                        invoiceLine.SetIsCostCalculated(true);
                                                                        if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                        {
                                                                            invoiceLine.SetIsCostImmediate(true);
                                                                        }
                                                                        if (!invoiceLine.Save(Get_Trx()))
                                                                        {
                                                                            ValueNamePair pp = VLogger.RetrieveError();
                                                                            _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                            Get_Trx().Rollback();
                                                                        }
                                                                        else
                                                                        {
                                                                            _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                            Get_Trx().Commit();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                else
                                                {
                                                    #region for Landed Cost Allocation
                                                    if (invoiceLine.GetC_Charge_ID() > 0)
                                                    {
                                                        if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null, 0,
                                                                "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_TrxName(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region for Expense type product
                                                    if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                            "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_TrxName(), out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                        else
                                                        {
                                                            if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                            {
                                                                invoiceLine.SetIsReversedCostCalculated(true);
                                                            }
                                                            invoiceLine.SetIsCostCalculated(true);
                                                            if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                            {
                                                                invoiceLine.SetIsCostImmediate(true);
                                                            }
                                                            if (!invoiceLine.Save(Get_Trx()))
                                                            {
                                                                ValueNamePair pp = VLogger.RetrieveError();
                                                                _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                           " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                Get_Trx().Rollback();
                                                            }
                                                            else
                                                            {
                                                                _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                Get_Trx().Commit();
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region  for Item Type product
                                                    else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                    {
                                                        if (countColumnExist > 0)
                                                        {
                                                            isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                                                        }

                                                        #region Sales Order
                                                        if (invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Customer)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                  Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                {
                                                                    DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                    {
                                                                        invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                   invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                   Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]), false);
                                                                        invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                    }
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region Purchase Cycle (not to be executed)
                                                        else if (!invoice.IsSOTrx() && !invoice.IsReturnTrx() && 0 == 1)
                                                        {
                                                            // calculate cost of MR first if not calculate which is linked with that invoice line
                                                            if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                            {
                                                                inoutLine = new MInOutLine(GetCtx(), invoiceLine.GetM_InOutLine_ID(), Get_Trx());
                                                                if (!inoutLine.IsCostCalculated())
                                                                {
                                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                                                "Material Receipt", null, inoutLine, null, invoiceLine, null, 0, inoutLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut))
                                                                    {
                                                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                                                        {
                                                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                                                        }
                                                                        _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                                                        continue;
                                                                    }
                                                                    else
                                                                    {
                                                                        inoutLine.SetIsCostCalculated(true);
                                                                        if (!inoutLine.Save(Get_Trx()))
                                                                        {
                                                                            ValueNamePair pp = VLogger.RetrieveError();
                                                                            _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                            Get_Trx().Rollback();
                                                                            continue;
                                                                        }
                                                                        else
                                                                        {
                                                                            Get_Trx().Commit();
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost,
                                                                  countColumnExist > 0 && isCostAdjustableOnLost && invoiceLine.GetM_InOutLine_ID() > 0 && inoutLine.GetMovementQty() < invoiceLine.GetQtyInvoiced() ? inoutLine.GetMovementQty() : invoiceLine.GetQtyInvoiced(),
                                                                  Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region CRMA
                                                        else if (invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Customer)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, invoiceLine.GetQtyInvoiced(),
                                                              Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                {
                                                                    DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                    {
                                                                        invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                   invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                   Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]), false);
                                                                        invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                    }
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region VRMA
                                                        else if (!invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                        {
                                                            // when Ap Credit memo is alone then we will do a impact on costing.
                                                            // this is bcz of giving discount for particular product
                                                            // discount is given only when document type having setting as "Treat As Discount" = True
                                                            MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                            if (docType.GetDocBaseType() == "APC" && docType.IsTreatAsDiscount() && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                  Get_Trx(), out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                                else
                                                                {
                                                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                    {
                                                                        invoiceLine.SetIsReversedCostCalculated(true);
                                                                    }
                                                                    if (invoiceLine.Get_ColumnIndex("PostCurrentCostPrice") >= 0 && invoiceLine.GetPostCurrentCostPrice() == 0)
                                                                    {
                                                                        // get post cost after invoice cost calculation and update on invoice
                                                                        currentCostPrice = MCost.GetproductCosts(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                                        product.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                                                                        invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                    }
                                                                    invoiceLine.SetIsCostCalculated(true);
                                                                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                    {
                                                                        invoiceLine.SetIsCostImmediate(true);
                                                                    }
                                                                    if (!invoiceLine.Save(Get_Trx()))
                                                                    {
                                                                        ValueNamePair pp = VLogger.RetrieveError();
                                                                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                        Get_Trx().Rollback();
                                                                    }
                                                                    else
                                                                    {
                                                                        _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                        Get_Trx().Commit();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    #endregion
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                    sql.Clear();
                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                    {
                                        sql.Append("SELECT COUNT(*) FROM C_InvoiceLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                    }
                                    else
                                    {
                                        sql.Append("SELECT COUNT(*) FROM C_InvoiceLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                    }
                                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
                                    {
                                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                        {
                                            invoice.SetIsReversedCostCalculated(true);
                                        }
                                        invoice.SetIsCostCalculated(true);
                                        if (!invoice.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for saving C_Invoice for this Record ID = " + invoice.GetC_Invoice_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for m_invoice = " + invoice.GetC_Invoice_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Landed cost Allocation
                            if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "LandedCost")
                            {
                                landedCostAllocation = new MLandedCostAllocation(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                MInvoiceLine invoiceLine = new MInvoiceLine(GetCtx(), landedCostAllocation.GetC_InvoiceLine_ID(), Get_Trx());
                                ProductInvoiceLineCost = invoiceLine.GetProductLineCost(invoiceLine, true);
                                MProduct product = MProduct.Get(GetCtx(), landedCostAllocation.GetM_Product_ID());
                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), landedCostAllocation.GetAD_Client_ID(), landedCostAllocation.GetAD_Org_ID(), product,
                                                               0, "LandedCost", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_Trx(), out conversionNotFoundInvoice))
                                {
                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                    {
                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                    }
                                    _log.Info("Cost not Calculated for landedCost for this Line ID = " + landedCostAllocation.GetC_InvoiceLine_ID());
                                }
                                else
                                {
                                    _log.Fine("Cost Calculation updated for m_invoiceline = " + landedCostAllocation.GetC_InvoiceLine_ID());
                                    Get_Trx().Commit();
                                }
                                continue;
                            }
                            #endregion

                            #region Cost Calculation for  PO Cycle --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_MatchInv" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "N"))
                                {

                                    CalculateCostForMatchInvoiced(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Physical Inventory --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Inventory" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL") &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["IsInternalUse"]) == "N")
                                {

                                    CalculateCostForInventory(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for  Internal use inventory --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Inventory" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL") &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["IsInternalUse"]) == "Y")
                                {

                                    CalculateCostForInventory(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Asset Disposal
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]).Equals("VAFAM_AssetDisposal"))
                                {
                                    po_AssetDisposal = tbl_AssetDisposal.GetPO(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());
                                    CalculateCostForAssetDisposal(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Util.GetValueOfInt(Util.GetValueOfInt(po_AssetDisposal.Get_Value("M_Product_ID"))));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Inventory Move --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Movement" &&
                                   (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL"))
                                {

                                    CalculateCostForMovement(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Production
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Production")
                                {
                                    #region calculate/update cost of components (Here IsSotrx means IsReversed --> on production header)
                                    if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]).Equals("N"))
                                    {
                                        SqlParameter[] param = new SqlParameter[1];
                                        param[0] = new SqlParameter("p_Record_Id", Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));
                                        param[0].SqlDbType = SqlDbType.Int;
                                        param[0].Direction = ParameterDirection.Input;

                                        DB.ExecuteProcedure("UpdateProductionLineWithCost", param, Get_Trx());
                                    }
                                    #endregion

                                    // count -> is there any record having cost not available on production line except finished good
                                    // if not found, then we will calculate cost of finished good else not.
                                    CountCostNotAvialable = 1;
                                    CountCostNotAvialable = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM m_productionline WHERE NVL(amt ,0) = 0  AND isactive = 'Y' AND m_product_id NOT IN
                                                         (SELECT M_product_ID FROM M_productionplan WHERE m_production_id = "
                                                          + Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]) + @"  AND isactive = 'Y' )
                                                        AND m_production_id = " + Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), null, Get_Trx()));

                                    if (CountCostNotAvialable == 0 || countGOM01 > 0)
                                    {
                                        // get record from production line based on production id
                                        sql.Clear();
                                        sql.Append(@"SELECT pl.M_ProductionLine_ID, pl.AD_Client_ID, pl.AD_Org_ID, p.MovementDate,  pl.M_Product_ID, 
                                                        pl.M_AttributeSetInstance_ID, pl.MovementQty, pl.M_Locator_ID, wh.IsDisallowNegativeInv,  pl.M_Warehouse_ID ,
                                                        p.IsCostCalculated, p.IsReversedCostCalculated,  p.IsReversed
                                                FROM M_Production p INNER JOIN M_ProductionPlan pp  ON pp.M_Production_id = pp.M_Production_id
                                                     INNER JOIN M_ProductionLine pl ON pl.M_ProductionPlan_id = pp.M_ProductionPlan_id
                                                     INNER JOIN M_Product prod  ON pl.M_Product_id = prod.M_Product_id
                                                     INNER JOIN M_Locator loc ON loc.M_Locator_id = pl.M_Locator_id
                                                     INNER JOIN M_Warehouse wh ON loc.M_Warehouse_id     = wh.M_Warehouse_id
                                                WHERE p.M_Production_ID   =pp.M_Production_ID AND pp.M_ProductionPlan_ID=pl.M_ProductionPlan_ID
                                                      AND pp.M_Production_ID    =" + Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]) + @"
                                                      AND pl.M_Product_ID = prod.M_Product_ID AND prod.ProductType ='I' 
                                                      AND pl.M_Locator_ID = loc.M_Locator_ID AND loc.M_Warehouse_ID    = wh.M_Warehouse_ID");
                                        if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                                        {
                                            sql.Append(" AND pl.M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                                        }
                                        else
                                        {
                                            sql.Append(" AND pl.M_Product_ID IN (" + productID + " )");
                                        }
                                        sql.Append(" ORDER BY  pp.Line,  pl.Line");
                                        dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                        if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                        {
                                            for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                            {
                                                #region Create & Open connection and Execute Procedure
                                                try
                                                {
                                                    SqlParameter[] param = new SqlParameter[7];
                                                    param[0] = new SqlParameter("p_M_Product_ID", Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]));
                                                    param[0].SqlDbType = SqlDbType.Int;
                                                    param[0].Direction = ParameterDirection.Input;

                                                    param[1] = new SqlParameter("p_M_AttributeSetInstance_ID", Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_AttributeSetInstance_ID"]));
                                                    param[1].SqlDbType = SqlDbType.Int;
                                                    param[1].Direction = ParameterDirection.Input;

                                                    param[2] = new SqlParameter("p_AD_Org_ID", Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["AD_Org_ID"]));
                                                    param[2].SqlDbType = SqlDbType.Int;
                                                    param[2].Direction = ParameterDirection.Input;

                                                    param[3] = new SqlParameter("p_AD_Client_ID", Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["AD_Client_ID"]));
                                                    param[3].SqlDbType = SqlDbType.Int;
                                                    param[3].Direction = ParameterDirection.Input;

                                                    param[4] = new SqlParameter("p_Quantity", Util.GetValueOfDecimal(dsChildRecord.Tables[0].Rows[j]["MovementQty"]));
                                                    param[4].SqlDbType = SqlDbType.Decimal;
                                                    param[4].Direction = ParameterDirection.Input;

                                                    param[5] = new SqlParameter("p_M_ProductionLine_ID", Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_ProductionLine_ID"]));
                                                    param[5].SqlDbType = SqlDbType.Int;
                                                    param[5].Direction = ParameterDirection.Input;

                                                    param[6] = new SqlParameter("p_M_Warehouse_ID", Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Warehouse_ID"]));
                                                    param[6].SqlDbType = SqlDbType.Int;
                                                    param[6].Direction = ParameterDirection.Input;

                                                    DB.ExecuteProcedure("createcostqueueNotFRPT", param, Get_Trx());

                                                    // update prodution header 
                                                    if (Util.GetValueOfString(dsChildRecord.Tables[0].Rows[j]["IsCostCalculated"]).Equals("N"))
                                                        DB.ExecuteQuery("UPDATE M_Production SET IsCostCalculated='Y' WHERE M_Production_ID= " + Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), null, Get_Trx());

                                                    if (Util.GetValueOfString(dsChildRecord.Tables[0].Rows[j]["IsCostCalculated"]).Equals("Y") &&
                                                        !Util.GetValueOfString(dsChildRecord.Tables[0].Rows[j]["IsReversedCostCalculated"]).Equals("N") && Util.GetValueOfString(dsChildRecord.Tables[0].Rows[j]["IsReversed"]).Equals("Y"))
                                                        DB.ExecuteQuery("UPDATE M_Production SET IsReversedCostCalculated='Y' WHERE M_Production_ID= " + Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), null, Get_Trx());
                                                }
                                                catch
                                                {
                                                    Get_Trx().Rollback();
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                    Get_Trx().Commit();
                                    continue;
                                }
                            }
                            catch
                            {
                                Get_Trx().Rollback();
                            }
                            #endregion

                            #region Cost Calculation For  Return to Vendor --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "Y" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL"))
                                {

                                    CalculateCostForReturnToVendor(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation Against AP Credit Memo - During Return Cycle of Purchase --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_MatchInv" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "Y"))
                                {

                                    CalculationCostCreditMemo(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation For shipment --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "Y" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "N" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL"))
                                {

                                    CalculateCostForShipment(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation For Customer Return --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "Y" &&
                                    Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "Y" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL"))
                                {

                                    CalculateCostForCustomerReturn(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Component Reduce for Production Execution --
                            try
                            {
                                if (count > 0)
                                {
                                    if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "VAMFG_M_WrkOdrTransaction" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CO" ||
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "CL"))
                                    {

                                        _log.Info("costng calculation start for production execution for document no =  " + Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["documentno"]));
                                        CalculateCostForProduction(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]));

                                        continue;
                                    }
                                }
                            }
                            catch (Exception exProductionExecution)
                            {
                                _log.Info("Error Occured during Production Execution costing " + exProductionExecution.ToString());
                            }


                            #endregion

                            //Reverse Record

                            #region Component Reduce for Production Execution --
                            try
                            {
                                if (count > 0)
                                {
                                    if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "VAMFG_M_WrkOdrTransaction" &&
                                        Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE")
                                    {

                                        CalculateCostForProduction(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]));

                                        continue;
                                    }
                                }
                            }
                            catch (Exception exProductionExecution)
                            {
                                _log.Info("Error Occured during Production Execution costing " + exProductionExecution.ToString());
                            }


                            #endregion

                            #region Cost Calculation For Customer Return --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "Y" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "Y" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE")
                                {

                                    CalculateCostForCustomerReturn(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation For shipment --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "Y" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "N" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE")
                                {

                                    CalculateCostForShipment(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation Against AP Credit Memo - During Return Cycle of Purchase - Reverse --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_MatchInvCostTrack" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "Y"))
                                {

                                    CalculateCostCreditMemoreversal(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation For  Return to Vendor --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "Y" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE")
                                {

                                    CalculateCostForCustomerReturn(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Inventory Move --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Movement" &&
                                  Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE")
                                {

                                    CalculateCostForMovement(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for  Internal use inventory --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Inventory" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["IsInternalUse"]) == "Y")
                                {

                                    CalculateCostForInventory(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for Physical Inventory --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_Inventory" &&
                                  Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE" &&
                                  Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["IsInternalUse"]) == "N")
                                {

                                    CalculateCostForInventory(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for  PO Cycle Reverse --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_MatchInvCostTrack" &&
                                    (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                     Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "N"))
                                {

                                    CalculateCostForMatchInvoiceReversal(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation for SO / PO / CRMA / VRMA
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "C_Invoice" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE")
                                {
                                    invoice = new MInvoice(GetCtx(), Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]), Get_Trx());

                                    sql.Clear();
                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                    {
                                        sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'N' " +
                                                     " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                        if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                                        {
                                            sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                                        }
                                        else
                                        {
                                            sql.Append(" AND M_Product_ID IN (" + productID + " )");
                                        }
                                        sql.Append(" ORDER BY Line");
                                    }
                                    else
                                    {
                                        sql.Append("SELECT * FROM C_InvoiceLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                                                     " AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                        if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                                        {
                                            sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                                        }
                                        else
                                        {
                                            sql.Append(" AND M_Product_ID IN (" + productID + " )");
                                        }
                                        sql.Append(" ORDER BY Line");
                                    }
                                    dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
                                    if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
                                    {
                                        for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                                        {
                                            try
                                            {
                                                product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                                                invoiceLine = new MInvoiceLine(GetCtx(), dsChildRecord.Tables[0].Rows[j], Get_Trx());
                                                if (invoiceLine != null && invoiceLine.Get_ID() > 0)
                                                {
                                                    ProductInvoiceLineCost = invoiceLine.GetProductLineCost(invoiceLine, true);
                                                }
                                                if (invoiceLine != null && invoiceLine.GetC_Invoice_ID() > 0 && invoiceLine.GetQtyInvoiced() == 0)
                                                    continue;
                                                if (invoiceLine.GetC_OrderLine_ID() > 0)
                                                {
                                                    if (invoiceLine.GetC_Charge_ID() > 0)
                                                    {
                                                        #region Landed Cost Allocation
                                                        if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null, 0, "Invoice(Vendor)",
                                                                null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        #region for Expense type product
                                                        if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                                 "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region  for Item Type product
                                                        else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                        {
                                                            if (countColumnExist > 0)
                                                            {
                                                                isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                                                            }

                                                            MOrder order1 = new MOrder(GetCtx(), invoice.GetC_Order_ID(), Get_Trx());
                                                            if (order1.GetC_Order_ID() == 0)
                                                            {
                                                                MOrderLine ol1 = new MOrderLine(GetCtx(), invoiceLine.GetC_OrderLine_ID(), Get_Trx());
                                                                order1 = new MOrder(GetCtx(), ol1.GetC_Order_ID(), Get_Trx());
                                                            }

                                                            #region  Sales Order
                                                            if (order1.IsSOTrx() && !order1.IsReturnTrx())
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                      "Invoice(Customer)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                      Get_Trx(), out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                                else
                                                                {
                                                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                    {
                                                                        invoiceLine.SetIsReversedCostCalculated(true);
                                                                    }
                                                                    if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                    {
                                                                        DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                        {
                                                                            invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                            currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                       invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                       Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]), false);
                                                                            invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                        }
                                                                    }
                                                                    invoiceLine.SetIsCostCalculated(true);
                                                                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                    {
                                                                        invoiceLine.SetIsCostImmediate(true);
                                                                    }
                                                                    if (!invoiceLine.Save(Get_Trx()))
                                                                    {
                                                                        ValueNamePair pp = VLogger.RetrieveError();
                                                                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                        Get_Trx().Rollback();
                                                                    }
                                                                    else
                                                                    {
                                                                        _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                        Get_Trx().Commit();
                                                                    }
                                                                }
                                                            }
                                                            #endregion

                                                            #region Purchase Order
                                                            else if (!order1.IsSOTrx() && !order1.IsReturnTrx() && 0 == 1)
                                                            {
                                                                // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                                                                if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                {
                                                                    inoutLine = new MInOutLine(GetCtx(), invoiceLine.GetM_InOutLine_ID(), Get_Trx());
                                                                }
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                      "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost,
                                                                      countColumnExist > 0 && isCostAdjustableOnLost && invoiceLine.GetM_InOutLine_ID() > 0 && inoutLine.GetMovementQty() < (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") ? decimal.Negate(invoiceLine.GetQtyInvoiced()) : invoiceLine.GetQtyInvoiced()) ? (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") ? Decimal.Negate(inoutLine.GetMovementQty()) : inoutLine.GetMovementQty()) : invoiceLine.GetQtyInvoiced(),
                                                                      Get_Trx(), out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                                else
                                                                {
                                                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                    {
                                                                        invoiceLine.SetIsReversedCostCalculated(true);
                                                                    }
                                                                    invoiceLine.SetIsCostCalculated(true);
                                                                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                    {
                                                                        invoiceLine.SetIsCostImmediate(true);
                                                                    }
                                                                    if (!invoiceLine.Save(Get_Trx()))
                                                                    {
                                                                        ValueNamePair pp = VLogger.RetrieveError();
                                                                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                        Get_Trx().Rollback();
                                                                    }
                                                                    else
                                                                    {
                                                                        _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                        Get_Trx().Commit();
                                                                    }
                                                                }
                                                            }
                                                            #endregion

                                                            #region CRMA
                                                            else if (order1.IsSOTrx() && order1.IsReturnTrx())
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Customer)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, invoiceLine.GetQtyInvoiced(), Get_Trx(), out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                                else
                                                                {
                                                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                    {
                                                                        invoiceLine.SetIsReversedCostCalculated(true);
                                                                    }
                                                                    if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                    {
                                                                        DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                        {
                                                                            invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                            currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                       invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                       Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]), false);
                                                                            invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                        }
                                                                    }
                                                                    invoiceLine.SetIsCostCalculated(true);
                                                                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                    {
                                                                        invoiceLine.SetIsCostImmediate(true);
                                                                    }
                                                                    if (!invoiceLine.Save(Get_Trx()))
                                                                    {
                                                                        ValueNamePair pp = VLogger.RetrieveError();
                                                                        _log.Info("Error found for saving Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                        Get_Trx().Rollback();
                                                                    }
                                                                    else
                                                                    {
                                                                        _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                        Get_Trx().Commit();
                                                                    }
                                                                }
                                                            }
                                                            #endregion

                                                            #region VRMA
                                                            else if (!order1.IsSOTrx() && order1.IsReturnTrx())
                                                            {
                                                                //change 12-5-2016
                                                                // when Ap Credit memo is alone then we will do a impact on costing.
                                                                // this is bcz of giving discount for particular product
                                                                // discount is given only when document type having setting as "Treat As Discount" = True
                                                                MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                                if (docType.GetDocBaseType() == "APC" && docType.IsTreatAsDiscount() && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                                {
                                                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                      "Invoice(Vendor)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                      Get_Trx(), out conversionNotFoundInvoice))
                                                                    {
                                                                        if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                        {
                                                                            conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                        }
                                                                        _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    }
                                                                    else
                                                                    {
                                                                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                        {
                                                                            invoiceLine.SetIsReversedCostCalculated(true);
                                                                        }
                                                                        invoiceLine.SetIsCostCalculated(true);
                                                                        if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                        {
                                                                            invoiceLine.SetIsCostImmediate(true);
                                                                        }
                                                                        if (!invoiceLine.Save(Get_Trx()))
                                                                        {
                                                                            ValueNamePair pp = VLogger.RetrieveError();
                                                                            _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                            Get_Trx().Rollback();
                                                                        }
                                                                        else
                                                                        {
                                                                            _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                            Get_Trx().Commit();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                else
                                                {
                                                    #region for Landed Cost Allocation
                                                    if (invoiceLine.GetC_Charge_ID() > 0)
                                                    {
                                                        if (!invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), null, 0,
                                                                "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_TrxName(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region for Expense type product
                                                    if (product.GetProductType() == "E" && product.GetM_Product_ID() > 0)
                                                    {
                                                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, 0,
                                                            "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, 0, Get_TrxName(), out conversionNotFoundInvoice))
                                                        {
                                                            if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                            {
                                                                conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                            }
                                                            _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                        }
                                                        else
                                                        {
                                                            if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                            {
                                                                invoiceLine.SetIsReversedCostCalculated(true);
                                                            }
                                                            invoiceLine.SetIsCostCalculated(true);
                                                            if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                            {
                                                                invoiceLine.SetIsCostImmediate(true);
                                                            }
                                                            if (!invoiceLine.Save(Get_Trx()))
                                                            {
                                                                ValueNamePair pp = VLogger.RetrieveError();
                                                                _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                           " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                Get_Trx().Rollback();
                                                            }
                                                            else
                                                            {
                                                                _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                Get_Trx().Commit();
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region  for Item Type product
                                                    else if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
                                                    {
                                                        if (countColumnExist > 0)
                                                        {
                                                            isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                                                        }

                                                        #region Sales Order
                                                        if (invoice.IsSOTrx() && !invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Customer)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                  Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                {
                                                                    DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                    {
                                                                        invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                   invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                   Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]), false);
                                                                        invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                    }
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region Purchase Order
                                                        else if (!invoice.IsSOTrx() && !invoice.IsReturnTrx() && 0 == 1)
                                                        {
                                                            if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                            {
                                                                inoutLine = new MInOutLine(GetCtx(), invoiceLine.GetM_InOutLine_ID(), Get_Trx());
                                                            }
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, null, ProductInvoiceLineCost,
                                                                  countColumnExist > 0 && isCostAdjustableOnLost && invoiceLine.GetM_InOutLine_ID() > 0 && inoutLine.GetMovementQty() < (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") ? decimal.Negate(invoiceLine.GetQtyInvoiced()) : invoiceLine.GetQtyInvoiced()) ? (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->") ? Decimal.Negate(inoutLine.GetMovementQty()) : inoutLine.GetMovementQty()) : invoiceLine.GetQtyInvoiced(),
                                                                  Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region CRMA
                                                        else if (invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                        {
                                                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                              "Invoice(Customer)", null, null, null, invoiceLine, null, ProductInvoiceLineCost, invoiceLine.GetQtyInvoiced(),
                                                              Get_Trx(), out conversionNotFoundInvoice))
                                                            {
                                                                if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                {
                                                                    conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                }
                                                                _log.Info("Cost not Calculated for Invoice(Customer) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                            }
                                                            else
                                                            {
                                                                if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                {
                                                                    invoiceLine.SetIsReversedCostCalculated(true);
                                                                }
                                                                if (invoiceLine.GetM_InOutLine_ID() > 0)
                                                                {
                                                                    DataSet ds = DB.ExecuteDataset(@"SELECT M_InOutLine.CurrentCostPrice, M_InOut.M_Warehouse_ID 
                                                                                FROM M_InOutLine INNER JOIN M_InOut ON M_InOut.M_InOut_ID = M_InOutLine.M_InOut_ID
                                                                                WHERE M_InOutLine.M_InOutLIne_ID = " + invoiceLine.GetM_InOutLine_ID(), null, Get_Trx());
                                                                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                                                    {
                                                                        invoiceLine.SetCurrentCostPrice(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CurrentCostPrice"]));

                                                                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                                                                                                   invoiceLine.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(),
                                                                                                   Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]), false);
                                                                        invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                                                                    }
                                                                }
                                                                invoiceLine.SetIsCostCalculated(true);
                                                                if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                {
                                                                    invoiceLine.SetIsCostImmediate(true);
                                                                }
                                                                if (!invoiceLine.Save(Get_Trx()))
                                                                {
                                                                    ValueNamePair pp = VLogger.RetrieveError();
                                                                    _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                    Get_Trx().Rollback();
                                                                }
                                                                else
                                                                {
                                                                    _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                    Get_Trx().Commit();
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region VRMA
                                                        else if (!invoice.IsSOTrx() && invoice.IsReturnTrx())
                                                        {
                                                            // when Ap Credit memo is alone then we will do a impact on costing.
                                                            // this is bcz of giving discount for particular product
                                                            // discount is given only when document type having setting as "Treat As Discount" = True
                                                            MDocType docType = new MDocType(GetCtx(), invoice.GetC_DocTypeTarget_ID(), Get_Trx());
                                                            if (docType.GetDocBaseType() == "APC" && docType.IsTreatAsDiscount() && invoiceLine.GetC_OrderLine_ID() == 0 && invoiceLine.GetM_InOutLine_ID() == 0 && invoiceLine.GetM_Product_ID() > 0)
                                                            {
                                                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                                                                  "Invoice(Vendor)", null, null, null, invoiceLine, null, Decimal.Negate(ProductInvoiceLineCost), Decimal.Negate(invoiceLine.GetQtyInvoiced()),
                                                                  Get_Trx(), out conversionNotFoundInvoice))
                                                                {
                                                                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                                                                    {
                                                                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                                                                    }
                                                                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                }
                                                                else
                                                                {
                                                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                                                    {
                                                                        invoiceLine.SetIsReversedCostCalculated(true);
                                                                    }
                                                                    invoiceLine.SetIsCostCalculated(true);
                                                                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                                                                    {
                                                                        invoiceLine.SetIsCostImmediate(true);
                                                                    }
                                                                    if (!invoiceLine.Save(Get_Trx()))
                                                                    {
                                                                        ValueNamePair pp = VLogger.RetrieveError();
                                                                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                                                        Get_Trx().Rollback();
                                                                    }
                                                                    else
                                                                    {
                                                                        _log.Fine("Cost Calculation updated for C_InvoiceLine = " + invoiceLine.GetC_InvoiceLine_ID());
                                                                        Get_Trx().Commit();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion
                                                    }
                                                    #endregion
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                    sql.Clear();
                                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                    {
                                        sql.Append("SELECT COUNT(*) FROM C_InvoiceLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                    }
                                    else
                                    {
                                        sql.Append("SELECT COUNT(*) FROM C_InvoiceLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND C_Invoice_ID = " + invoice.GetC_Invoice_ID());
                                    }
                                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
                                    {
                                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                                        {
                                            invoice.SetIsReversedCostCalculated(true);
                                        }
                                        invoice.SetIsCostCalculated(true);
                                        if (!invoice.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for saving C_Invoice for this Record ID = " + invoice.GetC_Invoice_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for C_Invoice = " + invoice.GetC_Invoice_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                            #region Cost Calculation For Material Receipt --
                            try
                            {
                                if (Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["TableName"]) == "M_InOut" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["issotrx"]) == "N" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["isreturntrx"]) == "N" &&
                                   Util.GetValueOfString(dsRecord.Tables[0].Rows[z]["docstatus"]) == "RE")
                                {

                                    CalculateCostForMaterialReversal(Util.GetValueOfInt(dsRecord.Tables[0].Rows[z]["Record_Id"]));

                                    continue;
                                }
                            }
                            catch { }
                            #endregion

                        }
                    }
                }

                // Re-Calculate Cost of those line whose costing not calculate 
                if (ListReCalculatedRecords != null && ListReCalculatedRecords.Count > 0)
                {
                    ReVerfyAndCalculateCost(ListReCalculatedRecords);
                }
            }
            catch (Exception ex)
            {
                _log.Info("Error Occured during costing " + ex.ToString());
                if (dsRecord != null)
                    dsRecord.Dispose();
                if (dsChildRecord != null)
                    dsChildRecord.Dispose();
            }
            finally
            {
                if (!string.IsNullOrEmpty(conversionNotFoundInOut1))
                {
                    conversionNotFoundInOut = Msg.GetMsg(GetCtx(), "ConvNotForMinout") + conversionNotFoundInOut1;
                }
                if (!string.IsNullOrEmpty(conversionNotFoundInvoice1))
                {
                    conversionNotFoundInvoice = Msg.GetMsg(GetCtx(), "ConvNotForInvoice") + conversionNotFoundInvoice1;
                }
                if (!string.IsNullOrEmpty(conversionNotFoundInventory1))
                {
                    conversionNotFoundInventory = Msg.GetMsg(GetCtx(), "ConvNotForInventry") + conversionNotFoundInventory1;
                }
                if (!string.IsNullOrEmpty(conversionNotFoundMovement1))
                {
                    conversionNotFoundMovement = Msg.GetMsg(GetCtx(), "ConvNotForMove") + conversionNotFoundMovement1;
                }
                if (!string.IsNullOrEmpty(conversionNotFoundProductionExecution1))
                {
                    conversionNotFoundProductionExecution = Msg.GetMsg(GetCtx(), "ConvNotForProduction") + conversionNotFoundProductionExecution1;
                }

                conversionNotFound = conversionNotFoundInOut + "\n" + conversionNotFoundInvoice + "\n" +
                                     conversionNotFoundInventory + "\n" + conversionNotFoundMovement + "\n" +
                                     conversionNotFoundProductionExecution;

                if (dsRecord != null)
                    dsRecord.Dispose();
                if (dsChildRecord != null)
                    dsChildRecord.Dispose();
                _log.Info("Successfully Ended Cost Calculation ");
            }
            return conversionNotFound;
        }

        /// <summary>
        ///  get min date for costing calculation
        /// </summary>
        /// <param name="count">Manufacturing Module is updated or not</param>
        /// <returns>DateTime -- form which date cost calculation process to be started </returns>
        public DateTime? SerachMinDate(int count)
        {
            DateTime? minDate;
            DateTime? tempDate;
            try
            {
                sql.Clear();
                sql.Append("SELECT Min(MovementDate) FROM m_Inventory WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N' AND description like '%{->%')) AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                minDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));

                sql.Clear();
                sql.Append("SELECT Min(MovementDate) FROM m_movement WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N' AND description like '%{->%'))  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                // Production 
                sql.Clear();
                sql.Append(@"SELECT Min(MovementDate) FROM M_Production WHERE isactive = 'Y' AND 
                              ((PROCESSED = 'Y' AND iscostcalculated = 'N' AND IsReversed = 'N' ) OR 
                               (PROCESSED = 'Y' AND iscostcalculated  = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N' AND IsReversed = 'Y' AND Name LIKE '%{->%'))  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                sql.Clear();
                sql.Append("SELECT Min(DateAcct) FROM m_matchinv WHERE isactive = 'Y' AND iscostcalculated = 'N'  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                try
                {
                    sql.Clear();
                    sql.Append("SELECT Min(Updated) FROM M_MatchInvCostTrack WHERE isactive = 'Y'  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                    tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                    if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                    {
                        minDate = tempDate;
                    }
                }
                catch { }
                try
                {
                    sql.Clear();
                    sql.Append("SELECT Min(VAFAM_trxdate) FROM VAFAM_AssetDisposal WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N'))  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                    tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                    if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                    {
                        minDate = tempDate;
                    }
                }
                catch { }

                sql.Clear();
                sql.Append("SELECT Min(DateAcct) FROM C_Invoice WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N' AND description like '%{->%'))  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                sql.Clear();
                sql.Append(@"SELECT Min(DateAcct) FROM C_ProvisionalInvoice WHERE isactive = 'Y' AND
                ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR 
                (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N' AND IsReversal ='Y'))  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                sql.Clear();
                sql.Append("SELECT Min(DateAcct) FROM m_inout WHERE isactive = 'Y' AND ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N' AND description like '%{->%'))  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                {
                    minDate = tempDate;
                }

                if (count > 0)
                {
                    try
                    {
                        // when Manufacuring Module is downloaded
                        sql.Clear();
                        sql.Append(@"SELECT Min(VAMFG_DateAcct) FROM VAMFG_M_WrkOdrTransaction WHERE VAMFG_WorkOrderTxnType IN ('CI', 'CR') AND  isactive = 'Y' AND
                             ((docstatus IN ('CO' , 'CL') AND iscostcalculated = 'N') 
                             OR (docstatus IN ('RE') AND iscostcalculated = 'Y' AND ISREVERSEDCOSTCALCULATED= 'N' AND VAMFG_description like '%{->%'))  AND AD_Client_ID = " + GetCtx().GetAD_Client_ID());
                        tempDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql.ToString(), null, Get_Trx()));
                    }
                    catch { }
                    if (minDate == null || (Util.GetValueOfDateTime(minDate) > Util.GetValueOfDateTime(tempDate) && tempDate != null))
                    {
                        minDate = tempDate;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return minDate;
        }

        /// <summary>
        /// we will update IsCostCalculation / IsReversedCostCalculation / IsCostImmediate on the Tansaction
        /// we will delete records from the Product Costs 
        /// </summary>
        /// <param name="count">Manufacturing Module is updated or not</param>
        private void UpdateAndDeleteCostImpacts(int count)
        {
            int countRecord = 0;

            if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
            {
                #region when we select product Category only
                // for M_Inout / M_Inoutline
                countRecord = DB.ExecuteQuery(@"UPDATE m_inoutline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0, PostCurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE M_Inout  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE M_Inout_ID IN ( 
                                        SELECT M_Inout_ID FROM m_inoutline WHERE  M_Product_ID IN 
                                        (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) )", null, Get_Trx());

                // for C_Invoice / C_InvoiceLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE c_invoiceline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , PostCurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE C_Invoice  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE C_Invoice_ID IN ( 
                                        SELECT C_Invoice_ID FROM C_Invoiceline WHERE  M_Product_ID IN 
                                        (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) )", null, Get_Trx());

                // for C_ProvisonalInvoice / C_ProvisionalInvoiceLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE c_Provisionalinvoiceline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' 
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE C_ProvisionalInvoice  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE C_ProvisionalInvoice_ID IN ( 
                                        SELECT C_ProvisionalInvoice_ID FROM C_ProvisionalInvoiceline WHERE  M_Product_ID IN 
                                        (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) )", null, Get_Trx());

                // For Landed Cost Allocation
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE C_LANDEDCOSTALLOCATION SET  iscostcalculated = 'N' 
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());

                // expected landed cost
                //countRecord = 0;
                //countRecord = DB.ExecuteQuery(@"UPDATE C_Expectedcostdistribution Set Iscostcalculated  = 'N'
                //                                    Where C_Orderline_Id In (Select C_Orderline_Id
                //                                      FROM C_OrderLine WHERE M_Product_Id IN 
                //                                      ((SELECT M_Product_Id FROM M_Product WHERE M_Product_Category_Id IN (" + productCategoryID + "))))", null, Get_Trx());

                // for M_Inventory / M_InventoryLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE m_inventoryline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N', CurrentCostPrice = 0 , PostCurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE m_inventory  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE m_inventory_ID IN ( 
                                        SELECT m_inventory_ID FROM m_inventoryline WHERE  M_Product_ID IN 
                                        (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) )", null, Get_Trx());

                // for VAFAM_AssetDisposal
                DB.ExecuteQuery(@"UPDATE VAFAM_AssetDisposal SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());

                // for M_Movement / M_MovementLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE m_movementline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0
                                                  , PostCurrentCostPrice = 0 , ToCurrentCostPrice = 0 , ToPostCurrentCostPrice = 0  WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE m_movement  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE m_movement_ID IN ( 
                                        SELECT m_movement_ID FROM m_movementline WHERE  M_Product_ID IN 
                                        (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) )", null, Get_Trx());

                // for M_Production / M_ProductionLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE m_productionline SET Amt = 0 
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE M_Production  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE M_Production_ID IN ( 
                                        SELECT M_Production_ID FROM M_Productionline WHERE  M_Product_ID IN 
                                        (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) )", null, Get_Trx());

                // for M_MatchInv
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE M_MatchInv SET iscostimmediate = 'N' , iscostcalculated = 'N'
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());

                // for M_MatchInvCostTrack
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"DELETE FROM M_MatchInvCostTrack
                                                 WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());

                // for m_freightimpact
                DB.ExecuteQuery(@"DELETE FROM m_freightimpact WHERE M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());

                // for VAMFG_M_WrkOdrTransaction  / VAMFG_M_WrkOdrTransactionLine
                if (count > 0)
                {
                    countRecord = 0;
                    countRecord = DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTrnsctionLine SET  iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N', CurrentCostPrice = 0 
                                                 WHERE  M_Product_ID IN 
                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                    if (countRecord > 0)
                        countRecord = DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTransaction  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' 
                                                              WHERE VAMFG_M_WrkOdrTransaction_ID IN ( 
                                                               SELECT VAMFG_M_WrkOdrTransaction_ID FROM VAMFG_M_WrkOdrTrnsctionLine 
                                                                WHERE M_Product_ID IN 
                                                                 (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) )", null, Get_Trx());

                    countRecord = DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTransaction SET   iscostcalculated = 'N',  isreversedcostcalculated = 'N', CurrentCostPrice = 0 
                                                         WHERE  M_Product_ID IN 
                                                         ( SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                }

                // Delete Query 
                DB.ExecuteQuery(@"delete from m_cost where m_product_id IN 
                                   (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costdetail  where m_product_id IN 
                                   (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                DB.ExecuteQuery(@"delete from M_CostQueueTransaction WHERE  m_product_id IN 
                                   (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costqueue  where m_product_id IN 
                                   (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costelementdetail  where m_product_id IN 
                                   (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) )", null, Get_Trx());
                #endregion
            }
            else if (!String.IsNullOrEmpty(productID))
            {
                #region when we select product
                // for M_Inout / M_Inoutline
                countRecord = DB.ExecuteQuery(@"UPDATE m_inoutline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0 , PostCurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN (" + productID + " )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE M_Inout  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE M_Inout_ID IN ( 
                                        SELECT M_Inout_ID FROM m_inoutline WHERE  M_Product_ID IN (" + productID + " ) )", null, Get_Trx());

                // for C_Invoice / C_InvoiceLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE c_invoiceline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , PostCurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN  (" + productID + " ) ", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE C_Invoice  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE C_Invoice_ID IN ( 
                                        SELECT C_Invoice_ID FROM C_Invoiceline WHERE  M_Product_ID IN  (" + productID + " ) )", null, Get_Trx());

                // for C_ProvisionalInvoice / C_ProvisionalInvoiceLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE c_Provisionalinvoiceline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' 
                                                 WHERE M_Product_ID IN  (" + productID + " ) ", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE C_ProvisionalInvoice  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE C_ProvisionalInvoice_ID IN ( 
                                        SELECT C_ProvisionalInvoice_ID FROM C_ProvisionalInvoiceline WHERE  M_Product_ID IN  (" + productID + " ) )", null, Get_Trx());

                // For Landed Cost Allocation
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE C_LANDEDCOSTALLOCATION SET  iscostcalculated = 'N' 
                                                 WHERE M_Product_ID IN (" + productID + " ) ", null, Get_Trx());

                // expected landed cost
                //countRecord = 0;
                //countRecord = DB.ExecuteQuery(@"UPDATE C_Expectedcostdistribution Set IsCostCalculated  = 'N'
                //                                    Where C_Orderline_ID IN (Select C_Orderline_ID
                //                                      FROM C_OrderLine WHERE M_Product_ID IN (" + productID + "))", null, Get_Trx());

                // for M_Inventory / M_InventoryLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE m_inventoryline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0 , PostCurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN (" + productID + " ) ", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE m_inventory  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE m_inventory_ID IN ( 
                                        SELECT m_inventory_ID FROM m_inventoryline WHERE  M_Product_ID IN (" + productID + " ) )", null, Get_Trx());

                // for M_Movement / M_MovementLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE m_movementline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0
                                                , PostCurrentCostPrice = 0 , ToCurrentCostPrice = 0 , ToPostCurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN  (" + productID + " ) ", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE m_movement  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE m_movement_ID IN ( 
                                        SELECT m_movement_ID FROM m_movementline WHERE  M_Product_ID IN (" + productID + " ) )", null, Get_Trx());
                // for VAFAM_AssetDisposal
                DB.ExecuteQuery(@"UPDATE vafam_assetdisposal SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' 
                                                 WHERE M_Product_ID IN  (" + productID + " ) ", null, Get_Trx());

                // for M_Production / M_ProductionLine
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE m_productionline SET Amt = 0 
                                                 WHERE M_Product_ID IN  (" + productID + " )", null, Get_Trx());
                if (countRecord > 0)
                    DB.ExecuteQuery(@"UPDATE M_Production  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N'
                                       WHERE M_Production_ID IN ( 
                                        SELECT M_Production_ID FROM M_Productionline WHERE  M_Product_ID IN  (" + productID + " ) )", null, Get_Trx());

                // for M_MatchInv
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"UPDATE M_MatchInv SET iscostimmediate = 'N' , iscostcalculated = 'N'
                                                 WHERE M_Product_ID IN  (" + productID + " ) ", null, Get_Trx());

                // for M_MatchInvCostTrack
                countRecord = 0;
                countRecord = DB.ExecuteQuery(@"DELETE FROM M_MatchInvCostTrack
                                                 WHERE M_Product_ID IN  (" + productID + " )", null, Get_Trx());

                // for m_freightimpact
                DB.ExecuteQuery(@"DELETE FROM m_freightimpact WHERE M_Product_ID IN  (" + productID + " )", null, Get_Trx());

                // for VAMFG_M_WrkOdrTransaction  / VAMFG_M_WrkOdrTransactionLine
                if (count > 0)
                {
                    countRecord = 0;
                    countRecord = DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTrnsctionLine SET  iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N', CurrentCostPrice = 0 
                                                 WHERE M_Product_ID IN  (" + productID + " ) ", null, Get_Trx());
                    if (countRecord > 0)
                        countRecord = DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTransaction  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' 
                                                                            WHERE VAMFG_M_WrkOdrTransaction_ID IN ( 
                                                                             SELECT VAMFG_M_WrkOdrTransaction_ID FROM VAMFG_M_WrkOdrTrnsctionLine
                                                                              WHERE  M_Product_ID IN  (" + productID + " ) )", null, Get_Trx());
                    //else
                    countRecord = DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTransaction  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N', CurrentCostPrice = 0 
                                                        WHERE  M_Product_ID IN  (" + productID + " )", null, Get_Trx());
                }

                // Delete Query 
                DB.ExecuteQuery(@"delete from m_cost where m_product_id IN  (" + productID + " )", null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costdetail  where m_product_id IN  (" + productID + " ) ", null, Get_Trx());
                DB.ExecuteQuery(@"delete from M_CostQueueTransaction WHERE  m_product_id IN (" + productID + " )", null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costqueue  where m_product_id IN  (" + productID + " )", null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costelementdetail  where m_product_id IN  (" + productID + " )", null, Get_Trx());
                #endregion
            }
            else if (onlyDeleteCosting.Equals("Y"))
            {
                #region when user select "Only Delete Costing"
                // for M_Inout / M_Inoutline
                DB.ExecuteQuery(@"UPDATE m_inoutline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0, PostCurrentCostPrice = 0  WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"UPDATE M_Inout  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for C_Invoice / C_InvoiceLine
                DB.ExecuteQuery(@"UPDATE c_invoiceline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N', PostCurrentCostPrice = 0   WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"UPDATE C_Invoice  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for C_ProvisionalInvoice / C_ProvisionalInvoiceLine
                DB.ExecuteQuery(@"UPDATE c_Provisionalinvoiceline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N'   WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"UPDATE C_ProvisionalInvoice  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // For Landed Cost Allocation
                DB.ExecuteQuery(@"UPDATE C_LANDEDCOSTALLOCATION SET  iscostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // expected landed cost
                //DB.ExecuteQuery(@"UPDATE C_Expectedcostdistribution Set IsCostCalculated  = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for M_Inventory / M_InventoryLine
                DB.ExecuteQuery(@"UPDATE m_inventoryline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0 , PostCurrentCostPrice = 0  WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"UPDATE m_inventory  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for VAFAM_AssetDisposal
                DB.ExecuteQuery(@"UPDATE vafam_assetdisposal SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N'  WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for M_Movement / M_MovementLine
                DB.ExecuteQuery(@"UPDATE m_movementline SET iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0 
                                , PostCurrentCostPrice = 0 , ToCurrentCostPrice = 0 , ToPostCurrentCostPrice = 0  WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"UPDATE m_movement  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for M_Production / M_ProductionLine
                DB.ExecuteQuery(@"UPDATE m_productionline SET Amt = 0 WHERE NVL(C_Charge_ID,0) = 0 AND AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"UPDATE M_Production  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for M_MatchInv
                DB.ExecuteQuery(@"UPDATE M_MatchInv SET iscostimmediate = 'N' , iscostcalculated = 'N' WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for M_MatchInvCostTrack
                DB.ExecuteQuery(@"DELETE FROM M_MatchInvCostTrack WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for m_freightimpact
                DB.ExecuteQuery(@"DELETE FROM m_freightimpact WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());

                // for VAMFG_M_WrkOdrTransaction  / VAMFG_M_WrkOdrTransactionLine
                if (count > 0)
                {
                    DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTrnsctionLine SET  iscostimmediate = 'N' , iscostcalculated = 'N',  isreversedcostcalculated = 'N', CurrentCostPrice = 0 WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                    DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTransaction  SET  iscostcalculated = 'N',  isreversedcostcalculated = 'N' , CurrentCostPrice = 0 WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                }

                // Delete Query 
                DB.ExecuteQuery(@"delete from m_cost WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costdetail WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"delete from M_CostQueueTransaction WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costqueue WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                DB.ExecuteQuery(@"delete from m_costelementdetail WHERE AD_client_ID =  " + GetAD_Client_ID(), null, Get_Trx());
                #endregion
            }

            Get_Trx().Commit();

        }

        /// <summary>
        /// is used to calculate cost agianst shipment
        /// </summary>
        /// <param name="M_Inout_IDinout referenceparam>
        private void CalculateCostForShipment(int M_Inout_ID)
        {
            inout = new MInOut(GetCtx(), M_Inout_ID, Get_Trx());

            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'N' " +
                          " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            else
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    try
                    {
                        inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                        orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                        if (orderLine != null && orderLine.GetC_Order_ID() > 0)
                        {
                            order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), null);
                            if (order.GetDocStatus() != "VO")
                            {
                                if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                    continue;
                            }
                            ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                            ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();
                        }
                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                        if (product.GetProductType() == "I") // for Item Type product
                        {
                            #region shipment
                            if (inout.IsSOTrx() && !inout.IsReturnTrx())
                            {
                                if (inout.GetC_Order_ID() <= 0)
                                {
                                    break;
                                }

                                costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());

                                #region get price from m_cost (Current Cost Price)
                                if (!client.IsCostImmediate() || inoutLine.GetCurrentCostPrice() == 0)
                                {
                                    // get price from m_cost (Current Cost Price)
                                    currentCostPrice = 0;
                                    currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                        inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                    DB.ExecuteQuery("UPDATE M_Inoutline SET CurrentCostPrice = " + currentCostPrice + " WHERE M_Inoutline_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                                #endregion

                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                     "Shipment", null, inoutLine, null, null, null,
                                     order.GetDocStatus() != "VO" ? Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(inoutLine.GetMovementQty()))
                                     : Decimal.Multiply(ProductOrderPriceActual, Decimal.Negate(inoutLine.GetQtyEntered())),
                                     Decimal.Negate(inoutLine.GetMovementQty()),
                                     Get_Trx(), out conversionNotFoundInOut))
                                {
                                    if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                    {
                                        conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                    }
                                    _log.Info("Cost not Calculated for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                    ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.Shipment, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = false });
                                }
                                else
                                {
                                    // when costing method is LIFO or FIFO
                                    if (!string.IsNullOrEmpty(costingMethod))
                                    {
                                        currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), inoutLine.GetAD_Client_ID(),
                                            inoutLine.GetAD_Org_ID(), inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), 0,
                                            inoutLine.GetM_InOutLine_ID(), costingMethod, inout.GetM_Warehouse_ID(), true, Get_Trx());
                                        inoutLine.SetCurrentCostPrice(currentCostPrice);
                                    }
                                    else if (inoutLine.GetCurrentCostPrice() == 0)
                                    {
                                        // get price from m_cost (Current Cost Price)
                                        currentCostPrice = 0;
                                        currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                            inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                        inoutLine.SetCurrentCostPrice(currentCostPrice);
                                    }
                                    if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                    {
                                        inoutLine.SetIsReversedCostCalculated(true);
                                    }
                                    inoutLine.SetIsCostCalculated(true);
                                    if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                    {
                                        inoutLine.SetIsCostImmediate(true);
                                    }
                                    if (!inoutLine.Save(Get_Trx()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                        Get_Trx().Rollback();
                                    }
                                    else
                                    {
                                        _log.Fine("Cost Calculation updated for M_InoutLine = " + inoutLine.GetM_InOutLine_ID());
                                        Get_Trx().Commit();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    catch { }
                }
            }
            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                {
                    inout.SetIsReversedCostCalculated(true);
                }
                inout.SetIsCostCalculated(true);
                if (!inout.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error found for saving M_inout for this Record ID = " + inout.GetM_InOut_ID() +
                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                }
                else
                {
                    _log.Fine("Cost Calculation updated for M_Inout = " + inout.GetM_InOut_ID());
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Is used to calculate cost against Customer return
        /// </summary>
        /// <param name="M_Inout_ID">inout reference</param>
        private void CalculateCostForCustomerReturn(int M_Inout_ID)
        {
            inout = new MInOut(GetCtx(), M_Inout_ID, Get_Trx());

            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'N' " +
                          " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            else
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    try
                    {
                        inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                        orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                        if (orderLine != null && orderLine.GetC_Order_ID() > 0)
                        {
                            order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), null);
                            if (order.GetDocStatus() != "VO")
                            {
                                if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                    continue;
                            }
                            ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                            ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();
                        }
                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                        if (product.GetProductType() == "I") // for Item Type product
                        {
                            #region Customer Return
                            if (inout.IsSOTrx() && inout.IsReturnTrx())
                            {
                                if (inout.GetOrig_Order_ID() <= 0)
                                {
                                    break;
                                }

                                costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());

                                #region get price from m_cost (Current Cost Price)
                                if (!client.IsCostImmediate() || inoutLine.GetCurrentCostPrice() == 0)
                                {
                                    // get price from m_cost (Current Cost Price)
                                    currentCostPrice = 0;
                                    currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                        inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID(), false);
                                    DB.ExecuteQuery("UPDATE M_Inoutline SET CurrentCostPrice = " + currentCostPrice + " WHERE M_Inoutline_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                                #endregion

                                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                      "Customer Return", null, inoutLine, null, null, null,
                                      order.GetDocStatus() != "VO" ? Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), inoutLine.GetMovementQty())
                                    : Decimal.Multiply(ProductOrderPriceActual, inoutLine.GetQtyEntered()),
                                      inoutLine.GetMovementQty(),
                                      Get_Trx(), out conversionNotFoundInOut))
                                {
                                    if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                    {
                                        conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                    }
                                    _log.Info("Cost not Calculated for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                    ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.CustomerReturn, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = false });
                                }
                                else
                                {
                                    // when costing method is LIFO or FIFO
                                    if (!string.IsNullOrEmpty(costingMethod))
                                    {
                                        currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), inoutLine.GetAD_Client_ID(),
                                            inoutLine.GetAD_Org_ID(), inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), 0,
                                            inoutLine.GetM_InOutLine_ID(), costingMethod, inout.GetM_Warehouse_ID(), false, Get_Trx());
                                        inoutLine.SetCurrentCostPrice(currentCostPrice);
                                    }
                                    else if (inoutLine.GetCurrentCostPrice() == 0)
                                    {
                                        // get price from m_cost (Current Cost Price)
                                        currentCostPrice = 0;
                                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                            inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID(), false);
                                        inoutLine.SetCurrentCostPrice(currentCostPrice);
                                    }
                                    if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                    {
                                        inoutLine.SetIsReversedCostCalculated(true);
                                    }
                                    inoutLine.SetIsCostCalculated(true);
                                    if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                    {
                                        inoutLine.SetIsCostImmediate(true);
                                    }
                                    if (!inoutLine.Save(Get_Trx()))
                                    {
                                        ValueNamePair pp = VLogger.RetrieveError();
                                        _log.Info("Error found for Customer Return for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                        Get_Trx().Rollback();
                                    }
                                    else
                                    {
                                        _log.Fine("Cost Calculation updated for M_InoutLine = " + inoutLine.GetM_InOutLine_ID());
                                        Get_Trx().Commit();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    catch { }
                }
            }
            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                {
                    inout.SetIsReversedCostCalculated(true);
                }
                inout.SetIsCostCalculated(true);
                if (!inout.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error found for saving M_inout for this Record ID = " + inout.GetM_InOut_ID() +
                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                }
                else
                {
                    _log.Fine("Cost Calculation updated for M_Inout = " + inout.GetM_InOut_ID());
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Is used to calculate cost against Return to Vendor
        /// </summary>
        /// <param name="M_Inout_ID">inout reference</param>
        private void CalculateCostForReturnToVendor(int M_Inout_ID)
        {
            inout = new MInOut(GetCtx(), M_Inout_ID, Get_Trx());

            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'N' " +
                          " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            else
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    try
                    {
                        inoutLine = new MInOutLine(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_InOutLine_ID"]), Get_Trx());
                        orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                        if (orderLine != null && orderLine.GetC_Order_ID() > 0)
                        {
                            order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), null);
                            if (order.GetDocStatus() != "VO")
                            {
                                if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                    continue;
                            }
                            ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                            ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();
                        }
                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                        if (product.GetProductType() == "I") // for Item Type product
                        {
                            #region  Return To Vendor
                            if (!inout.IsSOTrx() && inout.IsReturnTrx())
                            {
                                #region get price from m_cost (Current Cost Price)
                                if (!client.IsCostImmediate() || inoutLine.GetCurrentCostPrice() == 0)
                                {
                                    // get price from m_cost (Current Cost Price)
                                    currentCostPrice = 0;
                                    currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                        inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                    DB.ExecuteQuery("UPDATE M_Inoutline SET CurrentCostPrice = " + currentCostPrice + " WHERE M_Inoutline_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                                #endregion

                                if (inout.GetOrig_Order_ID() == 0 || orderLine == null || orderLine.GetC_OrderLine_ID() == 0)
                                {
                                    #region Return to Vendor against without Vendor RMA
                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                   "Return To Vendor", null, inoutLine, null, null, null, 0, Decimal.Negate(inoutLine.GetMovementQty()), Get_TrxName(), out conversionNotFoundInOut))
                                    {
                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                        {
                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                        }
                                        _log.Info("Cost not Calculated for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.ReturnVendor, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = false });
                                    }
                                    else
                                    {
                                        if (inoutLine.GetCurrentCostPrice() == 0)
                                        {
                                            // get price from m_cost (Current Cost Price)
                                            currentCostPrice = 0;
                                            currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                                inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                            inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        }
                                        if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                        {
                                            inoutLine.SetIsReversedCostCalculated(true);
                                        }
                                        inoutLine.SetIsCostCalculated(true);
                                        if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                        {
                                            inoutLine.SetIsCostImmediate(true);
                                        }
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for M_InoutLine = " + inoutLine.GetM_InOutLine_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Return to Vendor against with Vendor RMA

                                    amt = 0;
                                    if (isCostAdjustableOnLost && inoutLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                    {
                                        if (inoutLine.GetMovementQty() < 0)
                                            amt = ProductOrderLineCost;
                                        else
                                            amt = Decimal.Negate(ProductOrderLineCost);
                                    }
                                    else if (!isCostAdjustableOnLost && inoutLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                    {
                                        amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(inoutLine.GetMovementQty()));
                                    }
                                    else if (order.GetDocStatus() != "VO")
                                    {
                                        amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), Decimal.Negate(inoutLine.GetMovementQty()));
                                    }
                                    else if (order.GetDocStatus() == "VO")
                                    {
                                        amt = Decimal.Multiply(ProductOrderPriceActual, Decimal.Negate(inoutLine.GetQtyEntered()));
                                    }

                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                        "Return To Vendor", null, inoutLine, null, null, null, amt,
                                        Decimal.Negate(inoutLine.GetMovementQty()),
                                        Get_TrxName(), out conversionNotFoundInOut))
                                    {
                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                        {
                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                        }
                                        _log.Info("Cost not Calculated for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.ReturnVendor, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = false });
                                    }
                                    else
                                    {
                                        if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                        {
                                            inoutLine.SetIsReversedCostCalculated(true);
                                        }
                                        inoutLine.SetIsCostCalculated(true);
                                        if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                        {
                                            inoutLine.SetIsCostImmediate(true);
                                        }
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Return To Vendor for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for M_InoutLine = " + inoutLine.GetM_InOutLine_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }
                    catch { }
                }
            }
            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                {
                    inout.SetIsReversedCostCalculated(true);
                }
                inout.SetIsCostCalculated(true);
                if (!inout.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error found for saving M_inout for this Record ID = " + inout.GetM_InOut_ID() +
                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                }
                else
                {
                    _log.Fine("Cost Calculation updated for M_Inout = " + inout.GetM_InOut_ID());
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Is used to calculate cost against Material receipt
        /// </summary>
        /// <param name="M_Inout_ID">inout refreence</param>
        private void CalculateCostForMaterial(int M_Inout_ID)
        {
            inout = new MInOut(GetCtx(), M_Inout_ID, Get_Trx());

            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'N' " +
                            " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            else
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    try
                    {
                        inoutLine = new MInOutLine(GetCtx(), dsChildRecord.Tables[0].Rows[j], Get_Trx());
                        orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                        if (orderLine != null && orderLine.GetC_Order_ID() > 0)
                        {
                            order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), null);
                            if (order.GetDocStatus() != "VO")
                            {
                                if (orderLine != null && orderLine.GetC_Order_ID() > 0 && orderLine.GetQtyOrdered() == 0)
                                    continue;
                            }
                            ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                            ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();
                        }
                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                        if (product.GetProductType() == "I") // for Item Type product
                        {
                            bool isUpdatePostCurrentcostPriceFromMR = MCostElement.IsPOCostingmethod(GetCtx(), inout.GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());

                            #region Material Receipt
                            if (!inout.IsSOTrx() && !inout.IsReturnTrx())
                            {
                                if (orderLine == null || orderLine.GetC_OrderLine_ID() == 0) //MR Without PO
                                {
                                    #region MR Without PO
                                    if (!client.IsCostImmediate() || !inoutLine.IsCostImmediate() || inoutLine.GetCurrentCostPrice() == 0)
                                    {
                                        // get price from m_cost (Current Cost Price)
                                        currentCostPrice = 0;
                                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                            inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID(), false);
                                        DB.ExecuteQuery("UPDATE M_Inoutline SET CurrentCostPrice = " + currentCostPrice + " WHERE M_Inoutline_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx());
                                    }
                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                   "Material Receipt", null, inoutLine, null, null, null, 0, inoutLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut))
                                    {
                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                        {
                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                        }
                                        _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.MaterialReceipt, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = false });
                                    }
                                    else
                                    {
                                        if (inoutLine.GetCurrentCostPrice() == 0 || isUpdatePostCurrentcostPriceFromMR)
                                        {
                                            // get price from m_cost (Current Cost Price)
                                            currentCostPrice = 0;
                                            currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                                inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID(), false);
                                        }
                                        if (inoutLine.GetCurrentCostPrice() == 0)
                                        {
                                            inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        }
                                        if (isUpdatePostCurrentcostPriceFromMR && inoutLine.GetPostCurrentCostPrice() == 0)
                                        {
                                            inoutLine.SetPostCurrentCostPrice(currentCostPrice);
                                        }
                                        if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                        {
                                            inoutLine.SetIsReversedCostCalculated(true);
                                        }
                                        inoutLine.SetIsCostCalculated(true);
                                        if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                        {
                                            inoutLine.SetIsCostImmediate(true);
                                        }
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for m_inoutline = " + inoutLine.GetM_InOutLine_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region MR With PO
                                    if (!client.IsCostImmediate() || !inoutLine.IsCostImmediate() || inoutLine.GetCurrentCostPrice() == 0)
                                    {
                                        // get price from m_cost (Current Cost Price)
                                        currentCostPrice = 0;
                                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                            inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID(), false);
                                        inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        _log.Info("product cost " + inoutLine.GetM_Product_ID() + " - " + currentCostPrice);
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                    }

                                    amt = 0;
                                    if (isCostAdjustableOnLost && inoutLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                    {
                                        amt = ProductOrderLineCost;
                                    }
                                    else if (!isCostAdjustableOnLost && inoutLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                    {
                                        amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), inoutLine.GetMovementQty());
                                    }
                                    else if (order.GetDocStatus() != "VO")
                                    {
                                        amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), inoutLine.GetMovementQty());
                                    }
                                    else if (order.GetDocStatus() == "VO")
                                    {
                                        amt = Decimal.Multiply(ProductOrderPriceActual, inoutLine.GetQtyEntered());
                                    }
                                    _log.Info("product cost " + inoutLine.GetM_Product_ID());
                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                       "Material Receipt", null, inoutLine, null, null, null, amt,
                                       inoutLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut))
                                    {
                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                        {
                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                        }
                                        _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.MaterialReceipt, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = false });
                                    }
                                    else
                                    {
                                        _log.Info("product cost 1 " + inoutLine.GetM_Product_ID() + "- " + inoutLine.GetCurrentCostPrice());
                                        if (inoutLine.GetCurrentCostPrice() == 0 || isUpdatePostCurrentcostPriceFromMR)
                                        {
                                            // get price from m_cost (Current Cost Price)
                                            currentCostPrice = 0;
                                            currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                                inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID(), false);
                                        }
                                        if (inoutLine.GetCurrentCostPrice() == 0)
                                        {
                                            inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        }
                                        if (isUpdatePostCurrentcostPriceFromMR && inoutLine.GetPostCurrentCostPrice() == 0)
                                        {
                                            inoutLine.SetPostCurrentCostPrice(currentCostPrice);
                                        }
                                        if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                        {
                                            inoutLine.SetIsReversedCostCalculated(true);
                                        }
                                        inoutLine.SetIsCostCalculated(true);
                                        if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                        {
                                            inoutLine.SetIsCostImmediate(true);
                                        }
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for m_inoutline = " + inoutLine.GetM_InOutLine_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }
                    catch
                    {

                    }
                }
            }
            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                {
                    inout.SetIsReversedCostCalculated(true);
                }
                inout.SetIsCostCalculated(true);
                if (!inout.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error found for saving M_inout for this Record ID = " + inout.GetM_InOut_ID() +
                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());

                }
                else
                {
                    _log.Fine("Cost Calculation updated for m_inout = " + inout.GetM_InOut_ID());
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Is used to reverese the impact of cost which comes through Material Reeceipt
        /// </summary>
        /// <param name="M_Inout_ID">Inout reference</param>
        private void CalculateCostForMaterialReversal(int M_Inout_ID)
        {
            inout = new MInOut(GetCtx(), M_Inout_ID, Get_Trx());

            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'N' " +
                            " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            else
            {
                sql.Append("SELECT * FROM M_InoutLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND M_Inout_ID = " + inout.GetM_InOut_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    try
                    {
                        inoutLine = new MInOutLine(GetCtx(), dsChildRecord.Tables[0].Rows[j], Get_Trx());
                        orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), null);
                        if (orderLine != null && orderLine.GetC_Order_ID() > 0)
                        {
                            order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), null);
                            if (order.GetDocStatus() != "VO")
                            {
                                if (orderLine.GetQtyOrdered() == 0)
                                    continue;
                            }
                            ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                            ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();
                        }
                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                        if (product.GetProductType() == "I") // for Item Type product
                        {
                            #region Material Receipt
                            if (!inout.IsSOTrx() && !inout.IsReturnTrx())
                            {
                                if (orderLine == null || orderLine.GetC_OrderLine_ID() == 0)
                                {
                                    #region get price from m_cost (Current Cost Price)
                                    if (!client.IsCostImmediate() || inoutLine.GetCurrentCostPrice() == 0)
                                    {
                                        // get price from m_cost (Current Cost Price)
                                        currentCostPrice = 0;
                                        currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                            inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                        inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                    }
                                    #endregion

                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                   "Material Receipt", null, inoutLine, null, null, null, 0, inoutLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut))
                                    {
                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                        {
                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                        }
                                        _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.MaterialReceipt, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = true });
                                    }
                                    else
                                    {
                                        if (inoutLine.GetCurrentCostPrice() == 0)
                                        {
                                            // get price from m_cost (Current Cost Price)
                                            currentCostPrice = 0;
                                            currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                                inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                            inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        }
                                        if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                        {
                                            inoutLine.SetIsReversedCostCalculated(true);
                                        }
                                        inoutLine.SetIsCostCalculated(true);
                                        if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                        {
                                            inoutLine.SetIsCostImmediate(true);
                                        }
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for M_InoutLine = " + inoutLine.GetM_InOutLine_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                }
                                else
                                {
                                    #region get price from m_cost (Current Cost Price)
                                    if (!client.IsCostImmediate() || inoutLine.GetCurrentCostPrice() == 0)
                                    {
                                        // get price from m_cost (Current Cost Price)
                                        currentCostPrice = 0;
                                        currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                            inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                        inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                    }
                                    #endregion

                                    amt = 0;
                                    if (isCostAdjustableOnLost && inoutLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                    {
                                        if (inoutLine.GetMovementQty() > 0)
                                            amt = ProductOrderLineCost;
                                        else
                                            amt = Decimal.Negate(ProductOrderLineCost);
                                    }
                                    else if (!isCostAdjustableOnLost && inoutLine.GetMovementQty() < orderLine.GetQtyOrdered() && order.GetDocStatus() != "VO")
                                    {
                                        amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), inoutLine.GetMovementQty());
                                    }
                                    else if (order.GetDocStatus() != "VO")
                                    {
                                        amt = Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), inoutLine.GetMovementQty());
                                    }
                                    else if (order.GetDocStatus() == "VO")
                                    {
                                        amt = Decimal.Multiply(ProductOrderPriceActual, inoutLine.GetQtyEntered());
                                    }

                                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inout.GetAD_Client_ID(), inout.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                                       "Material Receipt", null, inoutLine, null, null, null, amt, inoutLine.GetMovementQty(),
                                       Get_Trx(), out conversionNotFoundInOut))
                                    {
                                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                                        {
                                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                                        }
                                        _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.MaterialReceipt, HeaderId = M_Inout_ID, LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = true });
                                    }
                                    else
                                    {
                                        if (inoutLine.GetCurrentCostPrice() == 0)
                                        {
                                            // get price from m_cost (Current Cost Price)
                                            currentCostPrice = 0;
                                            currentCostPrice = MCost.GetproductCosts(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                                inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inout.GetM_Warehouse_ID());
                                            inoutLine.SetCurrentCostPrice(currentCostPrice);
                                        }
                                        if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                                        {
                                            inoutLine.SetIsReversedCostCalculated(true);
                                        }
                                        inoutLine.SetIsCostCalculated(true);
                                        if (client.IsCostImmediate() && !inoutLine.IsCostImmediate())
                                        {
                                            inoutLine.SetIsCostImmediate(true);
                                        }
                                        if (!inoutLine.Save(Get_Trx()))
                                        {
                                            ValueNamePair pp = VLogger.RetrieveError();
                                            _log.Info("Error found for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID() +
                                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                            Get_Trx().Rollback();
                                        }
                                        else
                                        {
                                            _log.Fine("Cost Calculation updated for M_InoutLine = " + inoutLine.GetM_InOutLine_ID());
                                            Get_Trx().Commit();
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    catch { }
                }
            }
            sql.Clear();
            if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM M_InOutLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND M_InOut_ID = " + inout.GetM_InOut_ID());
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (inout.GetDescription() != null && inout.GetDescription().Contains("{->"))
                {
                    inout.SetIsReversedCostCalculated(true);
                }
                inout.SetIsCostCalculated(true);
                if (!inout.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error found for saving M_inout for this Record ID = " + inout.GetM_InOut_ID() +
                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                }
                else
                {
                    _log.Fine("Cost Calculation updated for M_Inout = " + inout.GetM_InOut_ID());
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Is used to caculate cost agaisnt match invoiced
        /// </summary>
        /// <param name="M_MatchInv_ID">match invoice reference</param>
        private void CalculateCostForMatchInvoiced(int M_MatchInv_ID)
        {
            matchInvoice = new MMatchInv(GetCtx(), M_MatchInv_ID, Get_Trx());
            inoutLine = new MInOutLine(GetCtx(), matchInvoice.GetM_InOutLine_ID(), Get_Trx());
            invoiceLine = new MInvoiceLine(GetCtx(), matchInvoice.GetC_InvoiceLine_ID(), Get_Trx());
            invoice = new MInvoice(GetCtx(), invoiceLine.GetC_Invoice_ID(), Get_Trx());
            product = new MProduct(GetCtx(), invoiceLine.GetM_Product_ID(), Get_Trx());
            int M_Warehouse_Id = inoutLine.GetM_Warehouse_ID();
            if (invoiceLine != null && invoiceLine.Get_ID() > 0)
            {
                ProductInvoiceLineCost = invoiceLine.GetProductLineCost(invoiceLine, true);
            }
            if (inoutLine.GetC_OrderLine_ID() > 0)
            {
                orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), Get_Trx());
                order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), Get_Trx());
                ProductOrderLineCost = orderLine.GetProductLineCost(orderLine);
                ProductOrderPriceActual = ProductOrderLineCost / orderLine.GetQtyEntered();
            }
            if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
            {
                bool isUpdatePostCurrentcostPriceFromMR = MCostElement.IsPOCostingmethod(GetCtx(), product.GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());
                if (countColumnExist > 0)
                {
                    isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                }

                // calculate cost of MR first if not calculate which is linked with that invoice line
                if (!inoutLine.IsCostCalculated())
                {
                    if (inoutLine.GetCurrentCostPrice() == 0)
                    {
                        // get price from m_cost (Current Cost Price)
                        currentCostPrice = 0;
                        currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                            inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), M_Warehouse_Id, false);
                        _log.Info("product cost " + inoutLine.GetM_Product_ID() + " - " + currentCostPrice);
                        DB.ExecuteQuery("UPDATE M_Inoutline SET CurrentCostPrice = " + currentCostPrice + " WHERE M_Inoutline_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx());
                    }

                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(), product, inoutLine.GetM_AttributeSetInstance_ID(),
                        "Material Receipt", null, inoutLine, null, invoiceLine, null,
                        order != null && order.GetDocStatus() != "VO" ? Decimal.Multiply(Decimal.Divide(ProductOrderLineCost, orderLine.GetQtyOrdered()), inoutLine.GetMovementQty())
                        : Decimal.Multiply(ProductOrderPriceActual, inoutLine.GetQtyEntered()),
                inoutLine.GetMovementQty(), Get_Trx(), out conversionNotFoundInOut))
                    {
                        if (!conversionNotFoundInOut1.Contains(conversionNotFoundInOut))
                        {
                            conversionNotFoundInOut1 += conversionNotFoundInOut + " , ";
                        }
                        _log.Info("Cost not Calculated for Material Receipt for this Line ID = " + inoutLine.GetM_InOutLine_ID());
                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.MaterialReceipt, HeaderId = inoutLine.GetM_InOut_ID(), LineId = inoutLine.GetM_InOutLine_ID(), IsReversal = false });
                        return;
                    }
                    else
                    {
                        if (isUpdatePostCurrentcostPriceFromMR || inoutLine.GetCurrentCostPrice() == 0)
                        {
                            // get price from m_cost (Current Cost Price)
                            currentCostPrice = 0;
                            currentCostPrice = MCost.GetproductCostAndQtyMaterial(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID(),
                                inoutLine.GetM_Product_ID(), inoutLine.GetM_AttributeSetInstance_ID(), Get_Trx(), M_Warehouse_Id, false);
                        }
                        if (inoutLine.GetCurrentCostPrice() == 0)
                        {
                            _log.Info("product cost " + inoutLine.GetM_Product_ID() + " - " + currentCostPrice);
                            //DB.ExecuteQuery("UPDATE M_Inoutline SET CurrentCostPrice = " + currentCostPrice + " WHERE M_Inoutline_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx());
                            inoutLine.SetCurrentCostPrice(currentCostPrice);
                        }
                        if (isUpdatePostCurrentcostPriceFromMR && inoutLine.GetPostCurrentCostPrice() == 0)
                        {
                            inoutLine.SetPostCurrentCostPrice(currentCostPrice);
                        }
                        inoutLine.SetIsCostCalculated(true);
                        if (!inoutLine.Save(Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                            Get_Trx().Rollback();
                            return;
                        }
                        else
                        {
                            Get_Trx().Commit();
                        }
                    }
                }

                if (matchInvoice.Get_ColumnIndex("CurrentCostPrice") >= 0)
                {
                    // get pre cost before invoice cost calculation and update on match invoice
                    //currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                    //                                                product.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(), M_Warehouse_Id, false);
                    //DB.ExecuteQuery(@"UPDATE M_MatchInv SET CurrentCostPrice = 
                    //                                          CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                    //                 @" END WHERE M_MatchInv_ID = " + matchInvoice.GetM_MatchInv_ID(), null, Get_Trx());
                }

                // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                      "Invoice(Vendor)", null, inoutLine, null, invoiceLine, null,
                    isCostAdjustableOnLost && matchInvoice.GetQty() < invoiceLine.GetQtyInvoiced() ? ProductInvoiceLineCost : Decimal.Multiply(Decimal.Divide(ProductInvoiceLineCost, invoiceLine.GetQtyInvoiced()), matchInvoice.GetQty()),
                      matchInvoice.GetQty(),
                      Get_Trx(), out conversionNotFoundInvoice))
                {
                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                    {
                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                    }
                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                    ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.MatchInvoice, HeaderId = M_MatchInv_ID, LineId = invoiceLine.GetC_InvoiceLine_ID(), IsReversal = false });
                }
                else
                {
                    if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                    {
                        invoiceLine.SetIsReversedCostCalculated(true);
                    }

                    invoiceLine.SetCurrentCostPrice(currentCostPrice);

                    // get cost from Product Cost after cost calculation
                    currentCostPrice = MCost.GetproductCostAndQtyMaterial(GetAD_Client_ID(), GetAD_Org_ID(),
                                                             product.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(), M_Warehouse_Id, false);
                    invoiceLine.SetPostCurrentCostPrice(currentCostPrice);
                    if (!isUpdatePostCurrentcostPriceFromMR)
                    {
                        invoiceLine.SetCurrentCostPrice(currentCostPrice);
                    }

                    invoiceLine.SetIsCostCalculated(true);
                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                    {
                        invoiceLine.SetIsCostImmediate(true);
                    }
                    if (!invoiceLine.Save(Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                        Get_Trx().Rollback();
                    }
                    else
                    {
                        _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                        Get_Trx().Commit();

                        if (matchInvoice.Get_ColumnIndex("PostCurrentCostPrice") >= 0)
                        {
                            // get post cost after invoice cost calculation and update on match invoice
                            //currentCostPrice = MCost.GetproductCostAndQtyMaterial(invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(),
                            //                                                product.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(), M_Warehouse_Id, false);
                            matchInvoice.SetPostCurrentCostPrice(currentCostPrice);
                        }
                        // set is cost calculation true on match invoice
                        matchInvoice.SetIsCostCalculated(true);
                        if (!matchInvoice.Save(Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + matchInvoice.GetC_InvoiceLine_ID() +
                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                            Get_Trx().Rollback();
                        }
                        else
                        {
                            Get_Trx().Commit();
                            // update the latest cost ON MR (Post Cost)
                            if (!isUpdatePostCurrentcostPriceFromMR)
                            {
                                DB.ExecuteQuery("UPDATE M_InoutLine SET PostCurrentCostPrice = " + matchInvoice.GetPostCurrentCostPrice() +
                                        @" WHERE M_InoutLine_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx());
                                Get_Trx().Commit();
                            }

                            // calculate Pre Cost - means cost before updation price impact of current record
                            if (matchInvoice != null && matchInvoice.GetM_MatchInv_ID() > 0 && matchInvoice.Get_ColumnIndex("CurrentCostPrice") >= 0)
                            {
                                currentCostPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT M_InOutLine.PostCurrentCostPrice FROM M_InOutLine 
                                                WHERE M_InOutLine.M_InOutLIne_ID = " + inoutLine.GetM_InOutLine_ID(), null, Get_Trx()));
                                DB.ExecuteQuery("UPDATE M_MatchInv SET CurrentCostPrice = " + currentCostPrice +
                                                 @" WHERE M_MatchInv_ID = " + matchInvoice.GetM_MatchInv_ID(), null, Get_Trx());

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cost Calculation for  PO Cycle - Reversal 
        /// Is used to reverse the impact of cost which is to be calculated througt invoice vendor (Match Invoiced)
        /// </summary>
        /// <param name="M_MatchInvCostTrack_ID">M_MatchInvCostTrack_ID reference</param>
        private void CalculateCostForMatchInvoiceReversal(int M_MatchInvCostTrack_ID)
        {
            matchInvCostReverse = new X_M_MatchInvCostTrack(GetCtx(), M_MatchInvCostTrack_ID, Get_Trx());
            inoutLine = new MInOutLine(GetCtx(), matchInvCostReverse.GetM_InOutLine_ID(), Get_Trx());
            invoiceLine = new MInvoiceLine(GetCtx(), matchInvCostReverse.GetRev_C_InvoiceLine_ID(), Get_Trx());
            invoice = new MInvoice(GetCtx(), invoiceLine.GetC_Invoice_ID(), Get_Trx());
            if (invoiceLine != null && invoiceLine.Get_ID() > 0)
            {
                ProductInvoiceLineCost = invoiceLine.GetProductLineCost(invoiceLine, true);
            }
            product = new MProduct(GetCtx(), invoiceLine.GetM_Product_ID(), Get_Trx());
            if (inoutLine.GetC_OrderLine_ID() > 0)
            {
                orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), Get_Trx());
                order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), Get_Trx());
            }
            if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
            {
                if (countColumnExist > 0)
                {
                    isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                }
                // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                      "Invoice(Vendor)", null, inoutLine, null, invoiceLine, null,
                    isCostAdjustableOnLost && matchInvCostReverse.GetQty() < Decimal.Negate(invoiceLine.GetQtyInvoiced()) ? ProductInvoiceLineCost : Decimal.Negate(Decimal.Multiply(Decimal.Divide(ProductInvoiceLineCost, invoiceLine.GetQtyInvoiced()), matchInvCostReverse.GetQty())),
                     decimal.Negate(matchInvCostReverse.GetQty()),
                      Get_Trx(), out conversionNotFoundInvoice))
                {
                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                    {
                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                    }
                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                    ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.MatchInvoice, HeaderId = M_MatchInvCostTrack_ID, LineId = invoiceLine.GetC_InvoiceLine_ID(), IsReversal = true });
                }
                else
                {
                    invoiceLine.SetIsReversedCostCalculated(true);
                    invoiceLine.SetIsCostCalculated(true);
                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                    {
                        invoiceLine.SetIsCostImmediate(true);
                    }
                    if (!invoiceLine.Save(Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                        Get_Trx().Rollback();
                    }
                    else
                    {
                        _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                        Get_Trx().Commit();

                        // update the Post current price after Invoice receving on inoutline
                        DB.ExecuteQuery(@"UPDATE M_InoutLine SET PostCurrentCostPrice = 0 
                                                                  WHERE M_InoutLine_ID = " + matchInvCostReverse.GetM_InOutLine_ID(), null, Get_Trx());

                        // set is cost calculation true on match invoice
                        if (!matchInvCostReverse.Delete(true, Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Info(" Delete Record M_MatchInvCostTrack -- Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                            Get_Trx().Rollback();
                        }
                        else
                        {
                            Get_Trx().Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is used to calculate cost for Physical Inventory or for Internal Use Inventory
        /// </summary>
        /// <param name="M_Inventory_ID">Inventory reference</param>
        private void CalculateCostForInventory(int M_Inventory_ID)
        {
            inventory = new MInventory(GetCtx(), M_Inventory_ID, Get_Trx());
            sql.Clear();
            if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'N' " +
                          " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            else
            {
                sql.Append("SELECT * FROM M_InventoryLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND M_Inventory_ID = " + inventory.GetM_Inventory_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                    inventoryLine = new MInventoryLine(GetCtx(), dsChildRecord.Tables[0].Rows[j], Get_Trx());
                    if (product.GetProductType() == "I") // for Item Type product
                    {
                        costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());
                        quantity = 0;
                        if (inventory.IsInternalUse())
                        {
                            #region for Internal use inventory

                            #region get price from m_cost (Current Cost Price)
                            if (!client.IsCostImmediate() || inventoryLine.GetCurrentCostPrice() == 0)
                            {
                                // get price from m_cost (Current Cost Price)
                                currentCostPrice = 0;
                                currentCostPrice = MCost.GetproductCosts(inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                    inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inventory.GetM_Warehouse_ID());
                                DB.ExecuteQuery("UPDATE M_InventoryLine SET CurrentCostPrice = " + currentCostPrice + @"
                                                   WHERE M_InventoryLine_ID = " + inventoryLine.GetM_InventoryLine_ID(), null, Get_Trx());
                            }
                            #endregion

                            quantity = Decimal.Negate(inventoryLine.GetQtyInternalUse());
                            // Change by mohit - Client id and organization was passed from context but neede to be passed from document itself as done in several other documents.-27/06/2017
                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inventory.GetAD_Client_ID(), inventory.GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                           "Internal Use Inventory", inventoryLine, null, null, null, null, 0, quantity, Get_Trx(), out conversionNotFoundInventory))
                            {
                                if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                {
                                    conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                }
                                _log.Info("Cost not Calculated for Internal Use Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.Inventory, HeaderId = M_Inventory_ID, LineId = inventoryLine.GetM_InventoryLine_ID(), IsReversal = false });
                            }
                            else
                            {
                                if (costingMethod != "" && quantity < 0)
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                                       inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), 1,
                                                       inventoryLine.GetM_InventoryLine_ID(), costingMethod,
                                                       inventory.GetM_Warehouse_ID(), true, Get_Trx());
                                    inventoryLine.SetCurrentCostPrice(currentCostPrice);
                                }

                                // when post current cost price is ZERO, than need to update cost here 
                                if (inventoryLine.GetPostCurrentCostPrice() == 0)
                                {
                                    currentCostPrice = MCost.GetproductCosts(inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                      inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inventory.GetM_Warehouse_ID());
                                    inventoryLine.SetPostCurrentCostPrice(currentCostPrice);
                                }
                                if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
                                {
                                    inventoryLine.SetIsReversedCostCalculated(true);
                                }
                                inventoryLine.SetIsCostCalculated(true);
                                if (client.IsCostImmediate() && !inventoryLine.IsCostImmediate())
                                {
                                    inventoryLine.SetIsCostImmediate(true);
                                }
                                if (!inventoryLine.Save(Get_Trx()))
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Info("Error found for saving Internal Use Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID() +
                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    Get_Trx().Rollback();
                                }
                                else
                                {
                                    _log.Fine("Cost Calculation updated for M_InventoryLine = " + inventoryLine.GetM_InventoryLine_ID());
                                    Get_Trx().Commit();
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region for Physical Inventory

                            quantity = Decimal.Subtract(inventoryLine.GetQtyCount(), inventoryLine.GetQtyBook());

                            #region get price from m_cost (Current Cost Price)
                            if (!client.IsCostImmediate() || inventoryLine.GetCurrentCostPrice() == 0)
                            {
                                // get price from m_cost (Current Cost Price)
                                currentCostPrice = 0;
                                //if (quantity < 0)
                                //{
                                currentCostPrice = MCost.GetproductCosts(inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                    inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inventory.GetM_Warehouse_ID());
                                //}
                                //else
                                //{
                                //    currentCostPrice = MCost.GetproductCostAndQtyMaterial(inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                //    inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inventory.GetM_Warehouse_ID(), false);
                                //}
                                DB.ExecuteQuery("UPDATE M_InventoryLine SET CurrentCostPrice = " + currentCostPrice + @"
                                                   WHERE M_InventoryLine_ID = " + inventoryLine.GetM_InventoryLine_ID(), null, Get_Trx());
                            }
                            #endregion

                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), inventory.GetAD_Client_ID(), inventory.GetAD_Org_ID(), product, inventoryLine.GetM_AttributeSetInstance_ID(),
                           "Physical Inventory", inventoryLine, null, null, null, null, 0, quantity, Get_Trx(), out conversionNotFoundInventory))
                            {
                                if (!conversionNotFoundInventory1.Contains(conversionNotFoundInventory))
                                {
                                    conversionNotFoundInventory1 += conversionNotFoundInventory + " , ";
                                }
                                _log.Info("Cost not Calculated for Physical Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID());
                                ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.Inventory, HeaderId = M_Inventory_ID, LineId = inventoryLine.GetM_InventoryLine_ID(), IsReversal = false });
                            }
                            else
                            {
                                if (costingMethod != "" && quantity < 0)
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                                       inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), 1,
                                                       inventoryLine.GetM_InventoryLine_ID(), costingMethod,
                                                       inventory.GetM_Warehouse_ID(), true, Get_Trx());
                                    inventoryLine.SetCurrentCostPrice(currentCostPrice);
                                }

                                // when post current cost price is ZERO, than need to update cost here 
                                if (inventoryLine.GetPostCurrentCostPrice() == 0)
                                {
                                    //if (quantity < 0)
                                    //{
                                    currentCostPrice = MCost.GetproductCosts(inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                  inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inventory.GetM_Warehouse_ID());
                                    //}
                                    //else
                                    //{
                                    //    currentCostPrice = MCost.GetproductCostAndQtyMaterial(inventoryLine.GetAD_Client_ID(), inventoryLine.GetAD_Org_ID(),
                                    // inventoryLine.GetM_Product_ID(), inventoryLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inventory.GetM_Warehouse_ID(), false);
                                    //}
                                    inventoryLine.SetPostCurrentCostPrice(currentCostPrice);
                                }
                                if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
                                {
                                    inventoryLine.SetIsReversedCostCalculated(true);
                                }
                                inventoryLine.SetIsCostCalculated(true);
                                if (client.IsCostImmediate() && !inventoryLine.IsCostImmediate())
                                {
                                    inventoryLine.SetIsCostImmediate(true);
                                }
                                if (!inventoryLine.Save(Get_Trx()))
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Info("Error found for saving Internal Use Inventory for this Line ID = " + inventoryLine.GetM_InventoryLine_ID() +
                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    Get_Trx().Rollback();
                                }
                                else
                                {
                                    _log.Fine("Cost Calculation updated for M_InventoryLine = " + inventoryLine.GetM_InventoryLine_ID());
                                    Get_Trx().Commit();
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            sql.Clear();
            if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT COUNT(*) FROM M_InventoryLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND M_Inventory_ID = " + inventory.GetM_Inventory_ID());
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM M_InventoryLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND M_Inventory_ID = " + inventory.GetM_Inventory_ID());
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (inventory.GetDescription() != null && inventory.GetDescription().Contains("{->"))
                {
                    inventory.SetIsReversedCostCalculated(true);
                }
                inventory.SetIsCostCalculated(true);
                if (!inventory.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null)
                        _log.Info("Error found for saving Internal Use Inventory for this Record ID = " + inventory.GetM_Inventory_ID() +
                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                }
                else
                {
                    _log.Fine("Cost Calculation updated for M_Inventory = " + inventory.GetM_Inventory_ID());
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Is used to calculate cost for AssetDisposal
        /// </summary>
        /// <param name="VAFAM_AssetDisposal_ID">AssetDisposal reference</param>
        /// <param name="M_Product_ID">AssetDisposal reference</param>
        private void CalculateCostForAssetDisposal(int VAFAM_AssetDisposal_ID, int M_Product_ID)
        {
            #region for Asset Disposal
            try
            {

                product = new MProduct(GetCtx(), M_Product_ID, Get_Trx());
                if (product.GetProductType() == "I") // for Item Type product
                {
                    quantity = 0;
                    quantity = Decimal.Negate(Util.GetValueOfDecimal(po_AssetDisposal.Get_Value("VAFAM_Qty")));
                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), Util.GetValueOfInt(po_AssetDisposal.Get_Value("AD_Client_ID")), Util.GetValueOfInt(po_AssetDisposal.Get_Value("AD_Org_ID")),
                        product, Util.GetValueOfInt(po_AssetDisposal.Get_Value("M_AttributeSetInstance_ID")), "AssetDisposal", null, null, null, null, po_AssetDisposal, 0, quantity, Get_Trx(), out conversionNotFoundInventory))
                    {
                        _log.Info("Cost not Calculated for Asset Dispose ID = " + VAFAM_AssetDisposal_ID);
                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.AssetDisposal, HeaderId = VAFAM_AssetDisposal_ID, LineId = VAFAM_AssetDisposal_ID, IsReversal = false });
                    }
                    else
                    {
                        if (Util.GetValueOfInt(po_AssetDisposal.Get_Value("ReversalDoc_ID")) > 0)
                        {
                            DB.ExecuteQuery("UPDATE VAFAM_AssetDisposal SET IsReversedCostCalculated='Y' WHERE VAFAM_AssetDisposal_ID = " + VAFAM_AssetDisposal_ID, null, Get_Trx());
                        }
                        else
                        {
                            DB.ExecuteQuery("UPDATE VAFAM_AssetDisposal SET ISCostCalculated='Y' WHERE VAFAM_AssetDisposal_ID = " + VAFAM_AssetDisposal_ID, null, Get_Trx());
                        }
                        _log.Fine("Cost Calculation updated for VAFAM_AssetDispoal= " + VAFAM_AssetDisposal_ID);
                        Get_Trx().Commit();
                    }
                }
            }
            catch { }
            #endregion

        }

        /// <summary>
        /// Is used to calculate cost againt Inventory Move
        /// </summary>
        /// <param name="M_Movement_ID">Movement id reference</param>
        private void CalculateCostForMovement(int M_Movement_ID)
        {
            movement = new MMovement(GetCtx(), M_Movement_ID, Get_Trx());

            sql.Clear();
            if (movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT * FROM M_MovementLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y'  AND IsReversedCostCalculated = 'N' " +
                          " AND M_Movement_ID = " + movement.GetM_Movement_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            else
            {
                sql.Append("SELECT * FROM M_MovementLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND M_Movement_ID = " + movement.GetM_Movement_ID());
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY Line");
            }
            dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                    movementLine = new MMovementLine(GetCtx(), dsChildRecord.Tables[0].Rows[j], Get_Trx());
                    locatorTo = MLocator.Get(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_LocatorTo_ID"]));
                    costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());

                    #region get price from m_cost (Current Cost Price)
                    if (!client.IsCostImmediate() || movementLine.GetCurrentCostPrice() == 0 || movementLine.GetToCurrentCostPrice() == 0)
                    {
                        currentCostPrice = MCost.GetproductCosts(movementLine.GetAD_Client_ID(), movementLine.GetAD_Org_ID(),
                            movementLine.GetM_Product_ID(), movementLine.GetM_AttributeSetInstance_ID(), Get_Trx(), movement.GetDTD001_MWarehouseSource_ID());

                        // For To Warehouse
                        toCurrentCostPrice = MCost.GetproductCosts(movementLine.GetAD_Client_ID(), locatorTo.GetAD_Org_ID(),
                           movementLine.GetM_Product_ID(), movementLine.GetM_AttributeSetInstance_ID(), Get_Trx(), locatorTo.GetM_Warehouse_ID());

                        DB.ExecuteQuery("UPDATE M_MovementLine SET  CurrentCostPrice = CASE WHEN CurrentCostPrice <> 0 THEN CurrentCostPrice ELSE " + currentCostPrice +
                            @" END , ToCurrentCostPrice = CASE WHEN ToCurrentCostPrice <> 0 THEN ToCurrentCostPrice ELSE " + toCurrentCostPrice + @"
                                                END  WHERE M_MovementLine_ID = " + movementLine.GetM_MovementLine_ID(), null, Get_Trx());
                    }
                    #endregion

                    // for Item Type product
                    if (product.GetProductType() == "I") //  && movement.GetAD_Org_ID() != warehouse.GetAD_Org_ID()
                    {
                        #region for inventory move
                        if (!MCostQueue.CreateProductCostsDetails(GetCtx(), movement.GetAD_Client_ID(), movement.GetAD_Org_ID(), product, movementLine.GetM_AttributeSetInstance_ID(),
                            "Inventory Move", null, null, movementLine, null, null, 0, movementLine.GetMovementQty(), Get_Trx(), out conversionNotFoundMovement))
                        {
                            if (!conversionNotFoundMovement1.Contains(conversionNotFoundMovement))
                            {
                                conversionNotFoundMovement1 += conversionNotFoundMovement + " , ";
                            }
                            _log.Info("Cost not Calculated for Inventory Move for this Line ID = " + movementLine.GetM_MovementLine_ID());
                            ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.Movement, HeaderId = M_Movement_ID, LineId = movementLine.GetM_MovementLine_ID(), IsReversal = false });
                        }
                        else
                        {
                            if (movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
                            {
                                movementLine.SetIsReversedCostCalculated(true);
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(costingMethod))
                                {
                                    if (movementLine.GetMovementQty() > 0)
                                    {
                                        currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), movementLine.GetAD_Client_ID(), movementLine.GetAD_Org_ID(),
                                                           movementLine.GetM_Product_ID(), movementLine.GetM_AttributeSetInstance_ID(), 2, movementLine.GetM_MovementLine_ID(), costingMethod,
                                                           movement.GetDTD001_MWarehouseSource_ID(), true, Get_Trx());
                                        movementLine.SetCurrentCostPrice(currentCostPrice);
                                    }

                                    if (movementLine.GetMovementQty() < 0)
                                    {
                                        toCurrentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(), movementLine.GetAD_Client_ID(), movementLine.GetAD_Org_ID(),
                                                               movementLine.GetM_Product_ID(), movementLine.GetM_AttributeSetInstance_ID(), 2, movementLine.GetM_MovementLine_ID(), costingMethod,
                                                               locatorTo.GetM_Warehouse_ID(), true, Get_Trx());
                                        movementLine.SetToCurrentCostPrice(currentCostPrice);
                                    }
                                }

                                if (movementLine.GetPostCurrentCostPrice() == 0)
                                {
                                    // For From warehouse
                                    currentCostPrice = MCost.GetproductCosts(movementLine.GetAD_Client_ID(), movementLine.GetAD_Org_ID(),
                                        movementLine.GetM_Product_ID(), movementLine.GetM_AttributeSetInstance_ID(), Get_Trx(), movement.GetDTD001_MWarehouseSource_ID());
                                    movementLine.SetPostCurrentCostPrice(currentCostPrice);
                                }
                                if (movementLine.GetToPostCurrentCostPrice() == 0)
                                {
                                    // For To Warehouse
                                    toCurrentCostPrice = MCost.GetproductCosts(movementLine.GetAD_Client_ID(), locatorTo.GetAD_Org_ID(),
                                       movementLine.GetM_Product_ID(), movementLine.GetM_AttributeSetInstance_ID(), Get_Trx(), locatorTo.GetM_Warehouse_ID());
                                    movementLine.SetToPostCurrentCostPrice(toCurrentCostPrice);
                                }
                            }
                            movementLine.SetIsCostCalculated(true);
                            if (client.IsCostImmediate() && !movementLine.IsCostImmediate())
                            {
                                movementLine.SetIsCostImmediate(true);
                            }
                            if (!movementLine.Save(Get_Trx()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Info("Error found for saving Inventory Move for this Line ID = " + movementLine.GetM_MovementLine_ID() +
                                           " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                Get_Trx().Rollback();
                            }
                            else
                            {
                                _log.Fine("Cost Calculation updated for M_MovementLine = " + movementLine.GetM_MovementLine_ID());
                                Get_Trx().Commit();
                            }
                        }
                        #endregion
                    }
                }
            }
            sql.Clear();
            if (movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
            {
                sql.Append("SELECT COUNT(*) FROM M_MovementLine WHERE IsReversedCostCalculated = 'N' AND IsActive = 'Y' AND M_Movement_ID = " + movement.GetM_Movement_ID());
            }
            else
            {
                sql.Append("SELECT COUNT(*) FROM M_MovementLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y' AND M_Movement_ID = " + movement.GetM_Movement_ID());
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (movement.GetDescription() != null && movement.GetDescription().Contains("{->"))
                {
                    movement.SetIsReversedCostCalculated(true);
                }
                movement.SetIsCostCalculated(true);
                if (!movement.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error found for saving Inventory Move for this Record ID = " + movement.GetM_Movement_ID() +
                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                }
                else
                {
                    _log.Fine("Cost Calculation updated for M_Movement = " + movement.GetM_Movement_ID());
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Component Reduce of those product which is to be consumed during Production Execution
        /// </summary>
        /// <param name="VAMFG_M_WrkOdrTransaction_ID">Production Execution Reference</param>
        /// <param name="IsSoTrx">During recalculation -- that product is available on product execution line</param>
        private void CalculateCostForProduction(int VAMFG_M_WrkOdrTransaction_ID, String IsSoTrx)
        {
            po_WrkOdrTransaction = tbl_WrkOdrTransaction.GetPO(GetCtx(), VAMFG_M_WrkOdrTransaction_ID, Get_Trx());

            // Production Execution Transaction Type
            woTrxType = Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_WorkOrderTxnType"));

            sql.Clear();
            if (Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")) != null &&
                Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")).Contains("{->") &&
                (woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_1_ComponentIssueToWorkOrder)
                || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_ComponentReturnFromWorkOrder) ||
                (countGOM01 <= 0 && (woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore)
                || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory)))))
            {
                sql.Append("SELECT * FROM VAMFG_M_WrkOdrTrnsctionLine WHERE IsActive = 'Y' AND iscostcalculated = 'Y' AND IsReversedCostCalculated = 'N' " +
                            " AND VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID);
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY VAMFG_Line");
            }
            else if (woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_1_ComponentIssueToWorkOrder)
                || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_ComponentReturnFromWorkOrder)
                || (countGOM01 <= 0 && (woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore)
                || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory))))
            {
                sql.Append("SELECT * FROM VAMFG_M_WrkOdrTrnsctionLine WHERE IsActive = 'Y' AND iscostcalculated = 'N' " +
                             " AND VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID);
                if (!String.IsNullOrEmpty(productCategoryID) && String.IsNullOrEmpty(productID))
                {
                    sql.Append(" AND M_Product_ID IN (SELECT M_Product_ID FROM M_Product WHERE M_Product_Category_ID IN (" + productCategoryID + " ) ) ");
                }
                else
                {
                    sql.Append(" AND M_Product_ID IN (" + productID + " )");
                }
                sql.Append(" ORDER BY VAMFG_Line");
            }
            if (!String.IsNullOrEmpty(sql.ToString()))
                dsChildRecord = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            else
                dsChildRecord = null;
            if (dsChildRecord != null && dsChildRecord.Tables.Count > 0 && dsChildRecord.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dsChildRecord.Tables[0].Rows.Count; j++)
                {
                    try
                    {
                        po_WrkOdrTrnsctionLine = tbl_WrkOdrTrnsctionLine.GetPO(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]), Get_Trx());

                        product = new MProduct(GetCtx(), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Get_Trx());
                        costingMethod = MCostElement.CheckLifoOrFifoMethod(GetCtx(), GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());

                        #region get price from m_cost (Current Cost Price)
                        // get price from m_cost (Current Cost Price)
                        if (Util.GetValueOfDecimal(po_WrkOdrTrnsctionLine.Get_Value("CurrentCostPrice")) == 0 &&
                           !(woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore)
                            || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory)))
                        {
                            currentCostPrice = 0;
                            currentCostPrice = MCost.GetproductCosts(Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Client_ID")), Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Org_ID")),
                                Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_Product_ID"]), Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["M_AttributeSetInstance_ID"]), Get_Trx());
                            po_WrkOdrTrnsctionLine.Set_Value("CurrentCostPrice", currentCostPrice);
                            if (!po_WrkOdrTrnsctionLine.Save(Get_Trx()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Info("Error found for Production execution Line for this Line ID = " + Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]) +
                                           " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                Get_Trx().Rollback();
                            }
                        }
                        else if (woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore)
                                || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory))
                        {
                            // when product having checkbox "IsBAsedOnRollup" then not to calculate cot of finished Good
                            if (product.IsBasedOnRollup())
                            {
                                continue;
                            }

                            currentCostPrice = GetCostForProductionFinishedGood(Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("VAMFG_M_WorkOrder_ID")), Get_Trx());

                            // if currentCostPrice is ZERO, then not to calculate cost of finished Good
                            if (currentCostPrice == 0)
                            {
                                continue;
                            }

                            // Update cost on Record
                            DB.ExecuteQuery(@"UPDATE VAMFG_M_WrkOdrTransaction SET CurrentCostPrice = " + currentCostPrice + @" 
                                                                        WHERE VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID, null, Get_Trx());
                        }
                        #endregion

                        if (woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_1_ComponentIssueToWorkOrder)
                            || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory))
                        {
                            #region 1_Component Issue to Work Order (CI)  / Assembly Return from Inventory(AR)
                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Client_ID")),
                                Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Org_ID")), product, Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("M_AttributeSetInstance_ID")),
                                woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory) ? "PE-FinishGood" : "Production Execution", null, null, null, null, po_WrkOdrTrnsctionLine,
                                woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory) ? currentCostPrice : 0,
                                countGOM01 > 0 ? Decimal.Negate(Util.GetValueOfDecimal(po_WrkOdrTrnsctionLine.Get_Value("GOM01_ActualQuantity"))) :
                                Decimal.Negate(Util.GetValueOfDecimal(po_WrkOdrTrnsctionLine.Get_Value("VAMFG_QtyEntered"))), Get_Trx(), out conversionNotFoundInOut))
                            {
                                if (!conversionNotFoundProductionExecution1.Contains(conversionNotFoundProductionExecution))
                                {
                                    conversionNotFoundProductionExecution1 += conversionNotFoundProductionExecution + " , ";
                                }
                                _log.Info("Cost not Calculated for Production Execution for this Line ID = " + Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]));
                            }
                            else
                            {
                                if (Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")) != null && Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")).Contains("{->"))
                                {
                                    po_WrkOdrTrnsctionLine.Set_Value("IsReversedCostCalculated", true);
                                }
                                if (!string.IsNullOrEmpty(costingMethod) && !(woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore)
                                     || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_AssemblyReturnFromInventory)))
                                {
                                    currentCostPrice = MCost.GetLifoAndFifoCurrentCostFromCostQueueTransaction(GetCtx(),
                                        Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Client_ID")),
                                        Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Org_ID")),
                                        product.GetM_Product_ID(),
                                        Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("M_AttributeSetInstance_ID")),
                                        6, Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID")),
                                        costingMethod, Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("M_Warehouse_ID")),
                                        true, Get_TrxName());
                                    po_WrkOdrTrnsctionLine.Set_Value("CurrentCostPrice", currentCostPrice);
                                }

                                po_WrkOdrTrnsctionLine.Set_Value("IsCostCalculated", true);
                                if (!po_WrkOdrTrnsctionLine.Save(Get_Trx()))
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Info("Error found for Production Execution for this Line ID = " + Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]) +
                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    Get_Trx().Rollback();
                                }
                                else
                                {
                                    _log.Fine("Cost Calculation updated for Production Execution line ID = " + Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]));
                                    Get_Trx().Commit();
                                }
                            }
                            #endregion
                        }
                        else if (woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_ComponentReturnFromWorkOrder)
                            || woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore))
                        {
                            #region  Component Return from Work Order (CR) / Assembly Return to Store(AI)
                            if (!MCostQueue.CreateProductCostsDetails(GetCtx(), Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Client_ID")),
                                Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("AD_Org_ID")), product, Util.GetValueOfInt(po_WrkOdrTrnsctionLine.Get_Value("M_AttributeSetInstance_ID")),
                                woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore) ? "PE-FinishGood" : "Production Execution", null, null, null, null, po_WrkOdrTrnsctionLine,
                                woTrxType.Equals(ViennaAdvantage.Model.X_VAMFG_M_WrkOdrTransaction.VAMFG_WORKORDERTXNTYPE_3_TransferAssemblyToStore) ? currentCostPrice : 0,
                                countGOM01 > 0 ? Util.GetValueOfDecimal(po_WrkOdrTrnsctionLine.Get_Value("GOM01_ActualQuantity")) :
                                Util.GetValueOfDecimal(po_WrkOdrTrnsctionLine.Get_Value("VAMFG_QtyEntered")), Get_Trx(), out conversionNotFoundInOut))
                            {
                                if (!conversionNotFoundProductionExecution1.Contains(conversionNotFoundProductionExecution))
                                {
                                    conversionNotFoundProductionExecution1 += conversionNotFoundProductionExecution + " , ";
                                }
                                _log.Info("Cost not Calculated for Production Execution for this Line ID = " + Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]));
                            }
                            else
                            {
                                if (Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")) != null && Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")).Contains("{->"))
                                {
                                    po_WrkOdrTrnsctionLine.Set_Value("IsReversedCostCalculated", true);
                                }
                                po_WrkOdrTrnsctionLine.Set_Value("IsCostCalculated", true);
                                if (!po_WrkOdrTrnsctionLine.Save(Get_Trx()))
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    _log.Info("Error found for Production Execution for this Line ID = " + Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]) +
                                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                    Get_Trx().Rollback();
                                }
                                else
                                {
                                    _log.Fine("Cost Calculation updated for Production Execution line ID = " + Util.GetValueOfInt(dsChildRecord.Tables[0].Rows[j]["VAMFG_M_WrkOdrTrnsctionLine_ID"]));
                                    Get_Trx().Commit();
                                }
                            }
                            #endregion
                        }
                    }
                    catch { }
                }
            }

            #region Calcuale Cost of Finished Good - (Process Manufacturing) -- gulfoil specific
            if (countGOM01 > 0 && IsSoTrx.Equals("Y") &&
                ((String)po_WrkOdrTransaction.Get_Value("VAMFG_WorkOrderTxnType") == "CI"
                || (String)po_WrkOdrTransaction.Get_Value("VAMFG_WorkOrderTxnType") == "CR"))
            {

                #region get price from m_cost (Current Cost Price) - update cost on header
                if (Util.GetValueOfDecimal(po_WrkOdrTransaction.Get_Value("CurrentCostPrice")) == 0)
                {
                    currentCostPrice = 0;
                    currentCostPrice = MCost.GetproductCosts(
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("AD_Client_ID")),
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("AD_Org_ID")),
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("M_Product_ID")),
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("M_AttributeSetInstance_ID")), Get_Trx());
                    DB.ExecuteQuery("UPDATE VAMFG_M_WrkOdrTransaction SET CurrentCostPrice = " + currentCostPrice +
                                @" WHERE VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID, null, Get_Trx());
                    Get_Trx().Commit();
                }
                #endregion

                #region calling for calculate cost of finished good.
                string className = "ViennaAdvantage.CMFG.Model.MVAMFGMWrkOdrTransaction";
                asm = System.Reflection.Assembly.Load("VAMFGSvc");
                type = asm.GetType(className);
                if (type != null)
                {
                    methodInfo = type.GetMethod("CalculateFinishedGoodCost");
                    if (methodInfo != null)
                    {
                        object result = "";

                        object[] parametersArrayConstructor = new object[] { GetCtx(),
                                                                VAMFG_M_WrkOdrTransaction_ID,
                                                                Get_Trx() };
                        object classInstance = Activator.CreateInstance(type, parametersArrayConstructor);

                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if (parameters.Length == 9)
                        {
                            object[] parametersArray = new object[] { GetCtx(),
                                                                Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("AD_Client_ID")),
                                                                Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("AD_Org_ID")),
                                                                Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("M_Product_ID")),
                                                                Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("M_AttributeSetInstance_ID")),
                                                                Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("VAMFG_M_WorkOrder_ID")),
                                                                Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("GOM01_BatchNo")),
                                                                Util.GetValueOfDecimal(po_WrkOdrTransaction.Get_Value("GOM01_ActualLiter")),
                                                                Get_Trx() };
                            result = methodInfo.Invoke(classInstance, parametersArray);
                            if (!(bool)result)
                            {
                                _log.Info("Cost not Calculated for Production Execution for Finished Good = " + VAMFG_M_WrkOdrTransaction_ID);
                                Get_Trx().Rollback();
                            }
                            else
                            {
                                Get_Trx().Commit();
                            }
                        }
                    }
                }
                #endregion

                #region get price from m_cost (Current Cost Price) - update cost on header
                if (Util.GetValueOfDecimal(po_WrkOdrTransaction.Get_Value("CurrentCostPrice")) == 0)
                {
                    currentCostPrice = 0;
                    currentCostPrice = MCost.GetproductCosts(
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("AD_Client_ID")),
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("AD_Org_ID")),
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("M_Product_ID")),
                        Util.GetValueOfInt(po_WrkOdrTransaction.Get_Value("M_AttributeSetInstance_ID")), Get_Trx());
                    DB.ExecuteQuery("UPDATE VAMFG_M_WrkOdrTransaction SET CurrentCostPrice = " + currentCostPrice +
                                @" WHERE VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID, null, Get_Trx());
                    Get_Trx().Commit();
                }
                #endregion
            }
            else if (countGOM01 > 0 && IsSoTrx.Equals("Y") &&
               ((String)po_WrkOdrTransaction.Get_Value("VAMFG_WorkOrderTxnType") == "AR"
               || (String)po_WrkOdrTransaction.Get_Value("VAMFG_WorkOrderTxnType") == "AI"))
            {
                #region 3_TransferAssemblyToStore | AssemblyReturnFromInventory
                // update Current cost price for Transfer Assembly to store as well as for Assembly Return form Inventory
                // calculation process is : 
                // get Actual qty in KG from Component Transaction Line
                // get cost from Component line where transaction type is "CI" (Component Issue to Work Order)
                // then divide the calculated value with Actual Qty in Kg fromProduction Execution Header
                // after that  we multiple density with  (sum of (qty * cost of each line) / Actual Qty in Kg from Production Execution Header)
                var sql1 = @"SELECT ROUND( wot.GOM01_ActualDensity * (SUM(wotl.GOM01_ActualQuantity * wotl.CurrentCostPrice) / wot.GOM01_ActualQuantity) , 10) as Currenctcost
                                                         FROM VAMFG_M_WrkOdrTransaction wot
                                                         INNER JOIN VAMFG_M_WorkOrder wo ON wo.VAMFG_M_WorkOrder_ID = wot.VAMFG_M_WorkOrder_ID
                                                         INNER JOIN VAMFG_M_WrkOdrTrnsctionLine wotl ON wot.VAMFG_M_WrkOdrTransaction_ID = wotl.VAMFG_M_WrkOdrTransaction_ID
                                                         WHERE wotl.IsActive = 'Y' AND wot.VAMFG_M_WorkOrder_ID = " + (int)po_WrkOdrTransaction.Get_Value("VAMFG_M_WorkOrder_ID") +
                             @" AND wot.VAMFG_WorkOrderTxnType = 'CI' " +
                              " AND wot.GOM01_BatchNo = '" + Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("GOM01_BatchNo")) + @"' GROUP BY wot.GOM01_ActualQuantity ,  wot.GOM01_ActualDensity";
                decimal currentcostprice = VAdvantage.Utility.Util.GetValueOfDecimal(DB.ExecuteScalar(sql1, null, Get_TrxName()));
                currentcostprice = Decimal.Round(currentcostprice, 10);
                DB.ExecuteQuery("UPDATE VAMFG_M_WrkOdrTransaction SET CurrentCostPrice = " + currentcostprice +
                                 @" WHERE VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID, null, Get_Trx());
                Get_Trx().Commit();
                #endregion
            }
            #endregion

            sql.Clear();
            if (Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")) != null &&
                Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")).Contains("{->"))
            {
                sql.Append(@"SELECT COUNT(*) FROM VAMFG_M_WrkOdrTrnsctionLine WHERE IsReversedCostCalculated = 'N'
                                                     AND IsActive = 'Y' AND VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID);
            }
            else
            {
                sql.Append(@"SELECT COUNT(*) FROM VAMFG_M_WrkOdrTrnsctionLine WHERE IsCostCalculated = 'N' AND IsActive = 'Y'
                                           AND VAMFG_M_WrkOdrTransaction_ID = " + VAMFG_M_WrkOdrTransaction_ID);
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, Get_Trx())) <= 0)
            {
                if (Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")) != null &&
                    Util.GetValueOfString(po_WrkOdrTransaction.Get_Value("VAMFG_Description")).Contains("{->"))
                {
                    po_WrkOdrTransaction.Set_Value("IsReversedCostCalculated", true);
                }
                po_WrkOdrTransaction.Set_Value("IsCostCalculated", true);
                if (!po_WrkOdrTransaction.Save(Get_Trx()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    _log.Info("Error found for saving Production execution for this Record ID = " + VAMFG_M_WrkOdrTransaction_ID +
                               " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                }
                else
                {
                    _log.Fine("Cost Calculation updated for Production Execution = " + VAMFG_M_WrkOdrTransaction_ID);
                    Get_Trx().Commit();
                }
            }
        }

        /// <summary>
        /// Get - sum of all component whose available on "Component Issue To Work Order" transaction
        /// </summary>
        /// <param name="VAMFG_M_WorkOrder_ID">production Order</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost of finished good</returns>
        private Decimal GetCostForProductionFinishedGood(int VAMFG_M_WorkOrder_ID, Trx trxName)
        {
            decimal currentcostprice = 0;

            // check any record havoing Zero cost, then return with ZERO Value
            String sql = @"SELECT COUNT(VAMFG_M_WrkOdrTrnsctionLine_ID) as NotFoundCurrentCost
                             FROM VAMFG_M_WrkOdrTransaction wot
                             INNER JOIN VAMFG_M_WorkOrder wo ON wo.VAMFG_M_WorkOrder_ID = wot.VAMFG_M_WorkOrder_ID
                             INNER JOIN VAMFG_M_WrkOdrTrnsctionLine wotl ON wot.VAMFG_M_WrkOdrTransaction_ID = wotl.VAMFG_M_WrkOdrTransaction_ID
                           WHERE wotl.IsActive = 'Y' AND wot.VAMFG_M_WorkOrder_ID = " + VAMFG_M_WorkOrder_ID +
                             @" AND wot.VAMFG_WorkOrderTxnType = 'CI' AND NVL(wotl.currentcostprice , 0) = 0 AND wot.DocStatus IN ('CO'  , 'CL') GROUP BY wot.VAMFG_QtyEntered ";
            if (VAdvantage.Utility.Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName)) == 0)
            {
                // sum of all component whose available on "Component Issue To Work Order" transaction
                sql = @"SELECT ROUND((SUM(wotl.VAMFG_QtyEntered * wotl.CurrentCostPrice) / wot.VAMFG_QtyEntered) , 10) as Currenctcost
                             FROM VAMFG_M_WrkOdrTransaction wot
                             INNER JOIN VAMFG_M_WorkOrder wo ON wo.VAMFG_M_WorkOrder_ID = wot.VAMFG_M_WorkOrder_ID
                             INNER JOIN VAMFG_M_WrkOdrTrnsctionLine wotl ON wot.VAMFG_M_WrkOdrTransaction_ID = wotl.VAMFG_M_WrkOdrTransaction_ID
                           WHERE wotl.IsActive = 'Y' AND wot.VAMFG_M_WorkOrder_ID = " + VAMFG_M_WorkOrder_ID +
                                 @" AND wot.VAMFG_WorkOrderTxnType = 'CI' AND wot.DocStatus IN ('CO'  , 'CL') GROUP BY wot.VAMFG_QtyEntered ";
                currentcostprice = VAdvantage.Utility.Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
            }
            return currentcostprice;
        }

        /// <summary>
        /// Cost Calculation Against AP Credit Memo - During Return Cycle of Purchase 
        /// </summary>
        /// <param name="M_MatchInv_ID">Match Invoice reference</param>
        private void CalculationCostCreditMemo(int M_MatchInv_ID)
        {
            matchInvoice = new MMatchInv(GetCtx(), M_MatchInv_ID, Get_Trx());
            inoutLine = new MInOutLine(GetCtx(), matchInvoice.GetM_InOutLine_ID(), Get_Trx());
            invoiceLine = new MInvoiceLine(GetCtx(), matchInvoice.GetC_InvoiceLine_ID(), Get_Trx());
            invoice = new MInvoice(GetCtx(), invoiceLine.GetC_Invoice_ID(), Get_Trx());
            product = new MProduct(GetCtx(), invoiceLine.GetM_Product_ID(), Get_Trx());
            bool isUpdatePostCurrentcostPriceFromMR = MCostElement.IsPOCostingmethod(GetCtx(), GetAD_Client_ID(), product.GetM_Product_ID(), Get_Trx());
            if (invoiceLine != null && invoiceLine.Get_ID() > 0)
            {
                ProductInvoiceLineCost = invoiceLine.GetProductLineCost(invoiceLine);
            }
            if (inoutLine.GetC_OrderLine_ID() > 0)
            {
                orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), Get_Trx());
                order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), Get_Trx());
            }
            if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
            {
                if (countColumnExist > 0)
                {
                    isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                }

                if (inoutLine.IsCostCalculated())
                {
                    // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                    if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                          "Invoice(Vendor)-Return", null, inoutLine, null, invoiceLine, null,
                        isCostAdjustableOnLost && matchInvoice.GetQty() < invoiceLine.GetQtyInvoiced() ? Decimal.Negate(ProductInvoiceLineCost) : Decimal.Negate(Decimal.Multiply(Decimal.Divide(ProductInvoiceLineCost, invoiceLine.GetQtyInvoiced()), matchInvoice.GetQty())),
                         Decimal.Negate(matchInvoice.GetQty()), Get_Trx(), out conversionNotFoundInvoice))
                    {
                        if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                        {
                            conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                        }
                        _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                        ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.CreditMemo, HeaderId = M_MatchInv_ID, LineId = invoiceLine.GetC_InvoiceLine_ID(), IsReversal = false });
                    }
                    else
                    {
                        if (invoice.GetDescription() != null && invoice.GetDescription().Contains("{->"))
                        {
                            invoiceLine.SetIsReversedCostCalculated(true);
                        }
                        invoiceLine.SetIsCostCalculated(true);
                        if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                        {
                            invoiceLine.SetIsCostImmediate(true);
                        }
                        if (!invoiceLine.Save(Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                       " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                            Get_Trx().Rollback();
                        }
                        else
                        {
                            _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                            Get_Trx().Commit();

                            // set is cost calculation true on match invoice
                            matchInvoice.SetIsCostCalculated(true);
                            if (matchInvoice.Get_ColumnIndex("PostCurrentCostPrice") >= 0)
                            {
                                // get cost from Product Cost after cost calculation
                                currentCostPrice = MCost.GetproductCosts(GetAD_Client_ID(), GetAD_Org_ID(),
                                                                         product.GetM_Product_ID(), invoiceLine.GetM_AttributeSetInstance_ID(), Get_Trx(), inoutLine.GetM_Warehouse_ID());
                                matchInvoice.SetPostCurrentCostPrice(currentCostPrice);
                            }
                            if (!matchInvoice.Save(Get_Trx()))
                            {
                                ValueNamePair pp = VLogger.RetrieveError();
                                _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + matchInvoice.GetC_InvoiceLine_ID() +
                                           " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                                Get_Trx().Rollback();
                            }
                            else
                            {
                                Get_Trx().Commit();
                                // update the Post current price after Invoice receving on inoutline
                                if (!isUpdatePostCurrentcostPriceFromMR)
                                {
                                    DB.ExecuteQuery(@"UPDATE M_InoutLine SET PostCurrentCostPrice =   " + currentCostPrice +
                                                    @" WHERE M_InoutLine_ID = " + matchInvoice.GetM_InOutLine_ID(), null, Get_Trx());
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Cost Calculation Against AP Credit Memo - During Return Cycle of Purchase - Reverse 
        /// </summary>
        /// <param name="M_MatchInvCostTrack_ID">etamporary table record reference -- M_MatchInvCostTrack_ID </param>
        private void CalculateCostCreditMemoreversal(int M_MatchInvCostTrack_ID)
        {
            matchInvCostReverse = new X_M_MatchInvCostTrack(GetCtx(), M_MatchInvCostTrack_ID, Get_Trx());
            inoutLine = new MInOutLine(GetCtx(), matchInvCostReverse.GetM_InOutLine_ID(), Get_Trx());
            invoiceLine = new MInvoiceLine(GetCtx(), matchInvCostReverse.GetRev_C_InvoiceLine_ID(), Get_Trx());
            invoice = new MInvoice(GetCtx(), invoiceLine.GetC_Invoice_ID(), Get_Trx());
            if (invoiceLine != null && invoiceLine.Get_ID() > 0)
            {
                ProductInvoiceLineCost = invoiceLine.GetProductLineCost(invoiceLine);
            }
            product = new MProduct(GetCtx(), invoiceLine.GetM_Product_ID(), Get_Trx());
            if (inoutLine.GetC_OrderLine_ID() > 0)
            {
                orderLine = new MOrderLine(GetCtx(), inoutLine.GetC_OrderLine_ID(), Get_Trx());
                order = new MOrder(GetCtx(), orderLine.GetC_Order_ID(), Get_Trx());
            }
            if (product.GetProductType() == "I" && product.GetM_Product_ID() > 0)
            {
                if (countColumnExist > 0)
                {
                    isCostAdjustableOnLost = product.IsCostAdjustmentOnLost();
                }
                // when isCostAdjustableOnLost = true on product and movement qty on MR is less than invoice qty then consider MR qty else invoice qty
                if (!MCostQueue.CreateProductCostsDetails(GetCtx(), invoiceLine.GetAD_Client_ID(), invoiceLine.GetAD_Org_ID(), product, invoiceLine.GetM_AttributeSetInstance_ID(),
                      "Invoice(Vendor)-Return", null, inoutLine, null, invoiceLine, null,
                    isCostAdjustableOnLost && matchInvCostReverse.GetQty() < Decimal.Negate(invoiceLine.GetQtyInvoiced()) ? Decimal.Negate(ProductInvoiceLineCost) : (Decimal.Multiply(Decimal.Divide(ProductInvoiceLineCost, invoiceLine.GetQtyInvoiced()), matchInvCostReverse.GetQty())),
                     matchInvCostReverse.GetQty(),
                      Get_Trx(), out conversionNotFoundInvoice))
                {
                    if (!conversionNotFoundInvoice1.Contains(conversionNotFoundInvoice))
                    {
                        conversionNotFoundInvoice1 += conversionNotFoundInvoice + " , ";
                    }
                    _log.Info("Cost not Calculated for Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID());
                    ListReCalculatedRecords.Add(new ReCalculateRecord { WindowName = (int)windowName.CreditMemo, HeaderId = M_MatchInvCostTrack_ID, LineId = invoiceLine.GetC_InvoiceLine_ID(), IsReversal = true });
                }
                else
                {
                    invoiceLine.SetIsReversedCostCalculated(true);
                    invoiceLine.SetIsCostCalculated(true);
                    if (client.IsCostImmediate() && !invoiceLine.IsCostImmediate())
                    {
                        invoiceLine.SetIsCostImmediate(true);
                    }
                    if (!invoiceLine.Save(Get_Trx()))
                    {
                        ValueNamePair pp = VLogger.RetrieveError();
                        _log.Info("Error found for saving Invoice(Vendor) for this Line ID = " + invoiceLine.GetC_InvoiceLine_ID() +
                                   " Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                        Get_Trx().Rollback();
                    }
                    else
                    {
                        _log.Fine("Cost Calculation updated for m_invoiceline = " + invoiceLine.GetC_InvoiceLine_ID());
                        Get_Trx().Commit();

                        // set is cost calculation true on match invoice
                        if (!matchInvCostReverse.Delete(true, Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            _log.Info(" Delete Record M_MatchInvCostTrack -- Error Name is " + pp.GetName() + " And Error Type is " + pp.GetType());
                            Get_Trx().Rollback();
                        }
                        else
                        {
                            Get_Trx().Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is used to calculate cost those record which are not calculate in first iteration due to qty unavialablity or other reason
        /// </summary>
        /// <param name="list">list of class -- ReCalculateRecord </param>
        private void ReVerfyAndCalculateCost(List<ReCalculateRecord> list)
        {
            ReCalculateRecord objReCalculateRecord = null;
            int loopCount = list.Count;
            for (int i = 0; i < loopCount; i++)
            {
                objReCalculateRecord = list[i];
                if (objReCalculateRecord.WindowName == (int)windowName.Shipment)
                {
                    CalculateCostForShipment(objReCalculateRecord.HeaderId);
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.ReturnVendor)
                {
                    CalculateCostForReturnToVendor(objReCalculateRecord.HeaderId);
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.Movement)
                {
                    CalculateCostForMovement(objReCalculateRecord.HeaderId);
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.MaterialReceipt)
                {
                    if (objReCalculateRecord.IsReversal)
                    {
                        CalculateCostForMaterialReversal(objReCalculateRecord.HeaderId);
                    }
                    else
                    {
                        CalculateCostForMaterial(objReCalculateRecord.HeaderId);
                    }
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.MatchInvoice)
                {
                    if (objReCalculateRecord.IsReversal)
                    {
                        CalculateCostForMatchInvoiceReversal(objReCalculateRecord.HeaderId);
                    }
                    else
                    {
                        CalculateCostForMatchInvoiced(objReCalculateRecord.HeaderId);
                    }
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.Inventory)
                {
                    CalculateCostForInventory(objReCalculateRecord.HeaderId);
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.CustomerReturn)
                {
                    CalculateCostForCustomerReturn(objReCalculateRecord.HeaderId);
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.CreditMemo)
                {
                    if (objReCalculateRecord.IsReversal)
                    {
                        CalculateCostCreditMemoreversal(objReCalculateRecord.HeaderId);
                    }
                    else
                    {
                        CalculationCostCreditMemo(objReCalculateRecord.HeaderId);
                    }
                }
                else if (objReCalculateRecord.WindowName == (int)windowName.AssetDisposal)
                {
                    po_AssetDisposal = tbl_AssetDisposal.GetPO(GetCtx(), objReCalculateRecord.HeaderId, Get_Trx());
                    CalculateCostForAssetDisposal(objReCalculateRecord.HeaderId, Util.GetValueOfInt(Util.GetValueOfInt(po_AssetDisposal.Get_Value("M_Product_ID"))));
                }
            }
        }

    }

    public class ReCalculateRecord
    {
        public int WindowName { get; set; }
        public bool IsReversal { get; set; }
        public int HeaderId { get; set; }
        public int LineId { get; set; }
    }
}
