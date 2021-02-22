/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMVAMProductCost
 * Purpose        : Cost calculation
 * Class Used     : X_VAM_ProductCost
 * Chronological    Development
 * Raghunandan     15-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMVAMProductCost : X_VAM_ProductCost
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMVAMProductCost).FullName);
        // Data is entered Manually		
        private bool _manual = true;

        /// <summary>
        /// Is used to get cost from product costs based on parameter
        /// </summary>
        /// <param name="client_Id">clinet</param>
        /// <param name="org_Id">org</param>
        /// <param name="product_id">product</param>
        /// <param name="M_ASI_Id">attributesetinstance</param>
        /// <param name="trxName">trx</param>
        /// <returns>current cost</returns>
        public static Decimal GetproductCosts(int client_Id, int org_Id, int product_id, int M_ASI_Id, Trx trxName)
        {
            return GetproductCosts(client_Id, org_Id, product_id, M_ASI_Id, trxName, 0);
        }

        /// <summary>
        /// Is used to get cost from product costs based on parameter
        /// </summary>
        /// <param name="client_Id">clinet</param>
        /// <param name="org_Id">org</param>
        /// <param name="product_id">product</param>
        /// <param name="M_ASI_Id">attributesetinstance</param>
        /// <param name="trxName">trx</param>
        /// <param name="VAM_Warehouse_ID"> warehouse id -- when to get cost against costing level - "Warehouse + Batch"</param>
        /// <returns>current cost</returns>
        public static Decimal GetproductCosts(int client_Id, int org_Id, int product_id, int M_ASI_Id, Trx trxName, int VAM_Warehouse_ID)
        {
            Decimal cost = 0;
            string sql = "";
            try
            {
                sql = @"SELECT ROUND(AVG(CST.CURRENTCOSTPRICE), 10)   FROM VAM_Product P   INNER JOIN VAM_ProductCost CST   ON P.VAM_Product_ID=CST.VAM_Product_ID
                               LEFT JOIN VAM_ProductCategory PC   ON P.VAM_ProductCategory_ID=PC.VAM_ProductCategory_ID
                               INNER JOIN VAB_ACCOUNTBOOK ACC   ON CST.VAB_ACCOUNTBOOK_ID=ACC.VAB_ACCOUNTBOOK_ID
                               INNER JOIN VAM_ProductCostType ct ON ct.VAM_ProductCostType_ID = acc.VAM_ProductCostType_ID
                               INNER JOIN VAM_ProductCostElement CE  ON CST.VAM_ProductCostElement_ID=CE.VAM_ProductCostElement_ID
                              WHERE (( CASE WHEN PC.COSTINGMETHOD IS NOT NULL  THEN PC.COSTINGMETHOD
                                            ELSE ACC.COSTINGMETHOD  END) = CE.COSTINGMETHOD )
                              AND ((   CASE WHEN PC.COSTINGMETHOD IS NOT NULL  AND PC.COSTINGMETHOD   = 'C'  THEN PC.VAM_ProductCostElement_id
                                            WHEN PC.COSTINGMETHOD IS NOT NULL  THEN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement 
                                             WHERE COSTINGMETHOD = pc.COSTINGMETHOD AND vaf_client_id    = " + client_Id + @" )
                                            WHEN ACC.COSTINGMETHOD IS NOT NULL AND ACC.COSTINGMETHOD   = 'C' THEN ACC.VAM_ProductCostElement_id ELSE
                                             (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE COSTINGMETHOD = acc.COSTINGMETHOD 
                                             AND vaf_client_id    = " + client_Id + @" ) END) = ce.VAM_ProductCostElement_id)
                             AND ((    CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D')  THEN " + org_Id + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('B' , 'C')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D') THEN " + org_Id + @"
                                            ELSE 0  END) = CST.VAF_Org_ID)
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'B', 'D')  THEN " + M_ASI_Id + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('C' , 'O', 'W')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'B', 'D') THEN " + M_ASI_Id + @"
                                            ELSE 0   END) = NVL(CST.VAM_PFeature_SetInstance_ID , 0))
                             AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('W' ,'D')  THEN " + VAM_Warehouse_ID + @"
                                             WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' ,'B' , 'C' ,'O')  THEN 0 
                                             WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('W' ,'D') THEN " + VAM_Warehouse_ID + @"
                                            ELSE 0   END) = NVL(CST.VAM_Warehouse_ID , 0))
                            AND P.VAM_Product_ID      =" + product_id + @"
                            AND CST.VAB_ACCOUNTBOOK_ID = (SELECT VAB_AccountBook1_id FROM VAF_ClientDetail WHERE vaf_client_id = " + client_Id + " )";
                cost = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
            }
            catch
            {
                _log.Info("GetproductCosts : " + sql);
                throw new ArgumentException("Error in getting cost from GetProductCosts");
            }
            return cost;
        }

        /// <summary>
        /// This function is used to get cost against costing method (material type) which is to be define on Cost Combination
        /// </summary>
        /// <param name="client_Id">clinet</param>
        /// <param name="org_Id">org</param>
        /// <param name="product_id">product</param>
        /// <param name="M_ASI_Id">attributesetinstance</param>
        /// <param name="trxName">trx</param>
        /// <param name="VAM_Warehouse_ID"> warehouse id -- when to get cost against costing level - "Warehouse + Batch, Warehouse"</param>
        /// <param name="IsRequiredQty">when true, give current qty else current cost"</param>
        /// <returns>cost</returns>
        public static Decimal GetproductCostAndQtyMaterial(int client_Id, int org_Id, int product_id, int M_ASI_Id, Trx trxName, int VAM_Warehouse_ID, bool IsRequiredQty)
        {
            Decimal cost = 0;
            string sql = "";
            try
            {
                sql = @"SELECT ROUND(" + (IsRequiredQty ? "AVG(CST.CURRENTQTY)" : "AVG(CST.CURRENTCOSTPRICE)") + @", 10)   FROM VAM_Product P   INNER JOIN VAM_ProductCost CST   ON P.VAM_Product_ID=CST.VAM_Product_ID
                               LEFT JOIN VAM_ProductCategory PC   ON P.VAM_ProductCategory_ID=PC.VAM_ProductCategory_ID
                               INNER JOIN VAB_ACCOUNTBOOK ACC   ON CST.VAB_ACCOUNTBOOK_ID=ACC.VAB_ACCOUNTBOOK_ID
                               INNER JOIN VAM_ProductCostType ct ON ct.VAM_ProductCostType_ID = acc.VAM_ProductCostType_ID
                               INNER JOIN VAM_ProductCostElement CE  ON CST.VAM_ProductCostElement_ID=CE.VAM_ProductCostElement_ID
                              WHERE ((   CASE WHEN PC.COSTINGMETHOD IS NOT NULL  AND PC.COSTINGMETHOD   = 'C'  THEN (SELECT CAST( Cel.M_Ref_Costelement AS INTEGER)
                                                  FROM VAM_ProductCostElement ced  INNER JOIN VAM_ProductCostElementLine Cel ON Ced.VAM_ProductCostElement_Id = CAST( Cel.M_Ref_Costelement AS INTEGER)
                                                  WHERE Ced.vaf_client_Id  =" + client_Id + @" AND Ced.Isactive ='Y' AND ced.CostElementType ='M'
                                                  AND Cel.Isactive ='Y' AND Cel.VAM_ProductCostElement_Id=PC.VAM_ProductCostElement_id AND ced.CostingMethod  IS NOT NULL )
                                            WHEN PC.COSTINGMETHOD IS NOT NULL  THEN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement 
                                                  WHERE COSTINGMETHOD = pc.COSTINGMETHOD AND vaf_client_id    = " + client_Id + @" )
                                            WHEN ACC.COSTINGMETHOD IS NOT NULL AND ACC.COSTINGMETHOD   = 'C' THEN (SELECT CAST( Cel.M_Ref_Costelement AS INTEGER)
                                                  FROM VAM_ProductCostElement ced  INNER JOIN VAM_ProductCostElementLine Cel ON Ced.VAM_ProductCostElement_Id = CAST( Cel.M_Ref_Costelement AS INTEGER)
                                                  WHERE Ced.vaf_client_Id  =" + client_Id + @" AND Ced.Isactive ='Y' AND ced.CostElementType ='M'
                                                  AND Cel.Isactive ='Y' AND Cel.VAM_ProductCostElement_Id= ACC.VAM_ProductCostElement_id AND ced.CostingMethod  IS NOT NULL ) 
                                            ELSE
                                                  (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE COSTINGMETHOD = acc.COSTINGMETHOD 
                                                  AND vaf_client_id    = " + client_Id + @" ) END) = ce.VAM_ProductCostElement_id)
                             AND ((    CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D')  THEN " + org_Id + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('B' , 'C')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D') THEN " + org_Id + @"
                                            ELSE 0  END) = CST.VAF_Org_ID)
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'B', 'D')  THEN " + M_ASI_Id + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('C' , 'O', 'W')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'B', 'D') THEN " + M_ASI_Id + @"
                                            ELSE 0   END) = NVL(CST.VAM_PFeature_SetInstance_ID , 0))
                             AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('W' ,'D')  THEN " + VAM_Warehouse_ID + @"
                                             WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' ,'B' , 'C' ,'O')  THEN 0 
                                             WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('W' ,'D') THEN " + VAM_Warehouse_ID + @"
                                            ELSE 0   END) = NVL(CST.VAM_Warehouse_ID , 0))
                            AND P.VAM_Product_ID      =" + product_id + @"
                            AND CST.VAB_ACCOUNTBOOK_ID = (SELECT VAB_AccountBook1_id FROM VAF_ClientDetail WHERE vaf_client_id = " + client_Id + " )";
                cost = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
            }
            catch
            {
                _log.Info("GetproductCosts : " + sql);
                throw new ArgumentException("Error in getting cost from GetProductCosts");
            }
            return cost;
        }

        /// <summary>
        /// Get Product Cost based on Accouting schema
        /// </summary>
        /// <param name="client_Id">client</param>
        /// <param name="org_Id">org</param>
        /// <param name="AcctSchemaId">accounting schema</param>
        /// <param name="product_id">product</param>
        /// <param name="M_ASI_Id">attribute set instance</param>
        /// <param name="trxName">transaction</param>
        /// <returns>current cost of costing method -- which is to be binded either on product category or on respective accounting schema</returns>
        public static Decimal GetproductCostBasedonAcctSchema(int client_Id, int org_Id, int AcctSchemaId, int product_id, int M_ASI_Id, Trx trxName)
        {
            return GetproductCostBasedonAcctSchema(client_Id, org_Id, AcctSchemaId, product_id, M_ASI_Id, trxName, 0);
        }

        /// <summary>
        /// Get Product Cost based on Accouting schema
        /// </summary>
        /// <param name="client_Id">client</param>
        /// <param name="org_Id">org</param>
        /// <param name="AcctSchemaId">accounting schema</param>
        /// <param name="product_id">product</param>
        /// <param name="M_ASI_Id">attribute set instance</param>
        /// <param name="trxName">transaction</param>
        /// <param name="VAM_Warehouse_ID"> warehouse id -- when to get cost against costing level - "Warehouse + Batch"</param>
        /// <returns>current cost of costing method -- which is to be binded either on product category or on respective accounting schema</returns>
        public static Decimal GetproductCostBasedonAcctSchema(int client_Id, int org_Id, int AcctSchemaId, int product_id, int M_ASI_Id, Trx trxName, int VAM_Warehouse_ID)
        {
            Decimal cost = 0;
            string sql = null;
            try
            {
                sql = @"SELECT ROUND(AVG(CST.CURRENTCOSTPRICE), 10)   FROM VAM_Product P   INNER JOIN VAM_ProductCost CST   ON P.VAM_Product_ID=CST.VAM_Product_ID
                               LEFT JOIN VAM_ProductCategory PC   ON P.VAM_ProductCategory_ID=PC.VAM_ProductCategory_ID
                               INNER JOIN VAB_ACCOUNTBOOK ACC   ON CST.VAB_ACCOUNTBOOK_ID=ACC.VAB_ACCOUNTBOOK_ID
                               INNER JOIN VAM_ProductCostType ct ON ct.VAM_ProductCostType_ID = acc.VAM_ProductCostType_ID
                               INNER JOIN VAM_ProductCostElement CE  ON CST.VAM_ProductCostElement_ID=CE.VAM_ProductCostElement_ID
                              WHERE (( CASE WHEN PC.COSTINGMETHOD IS NOT NULL  THEN PC.COSTINGMETHOD
                                            ELSE ACC.COSTINGMETHOD  END) = CE.COSTINGMETHOD )
                              AND ((   CASE WHEN PC.COSTINGMETHOD IS NOT NULL  AND PC.COSTINGMETHOD   = 'C'  THEN PC.VAM_ProductCostElement_id
                                            WHEN PC.COSTINGMETHOD IS NOT NULL  THEN (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement 
                                             WHERE COSTINGMETHOD = pc.COSTINGMETHOD AND vaf_client_id    = " + client_Id + @" )
                                            WHEN ACC.COSTINGMETHOD IS NOT NULL AND ACC.COSTINGMETHOD   = 'C' THEN ACC.VAM_ProductCostElement_id ELSE
                                             (SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE COSTINGMETHOD = acc.COSTINGMETHOD 
                                             AND vaf_client_id    = " + client_Id + @" ) END) = ce.VAM_ProductCostElement_id)
                             AND ((    CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D')  THEN " + org_Id + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('B' , 'C')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'O' , 'W' , 'D') THEN " + org_Id + @"
                                            ELSE 0  END) = CST.VAF_Org_ID)
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' , 'B', 'D')  THEN " + M_ASI_Id + @"
                                            WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('C' , 'O', 'W')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('A' , 'B', 'D') THEN " + M_ASI_Id + @"
                                            ELSE 0   END) = NVL(CST.VAM_PFeature_SetInstance_ID , 0))
                            AND ((     CASE WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('W' ,'D')  THEN " + VAM_Warehouse_ID + @"
                                             WHEN PC.COSTINGLEVEL IS NOT NULL  AND PC.COSTINGLEVEL   IN ('A' ,'B' , 'C' ,'O')  THEN 0 
                                            WHEN ACC.COSTINGLEVEL IS NOT NULL AND ACC.COSTINGLEVEL   IN ('W' ,'D') THEN " + VAM_Warehouse_ID + @"
                                            ELSE 0   END) = NVL(CST.VAM_Warehouse_ID , 0))
                            AND P.VAM_Product_ID      =" + product_id + @"
                            AND CST.VAB_ACCOUNTBOOK_ID = " + AcctSchemaId;
                cost = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, trxName));
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
                throw new ArgumentException("Error in getting cost from GetProductCosts");
            }
            return cost;
        }

        /* 	Retrieve/Calculate Current Cost Price
        *	@param product product
        *	@param VAM_PFeature_SetInstance_ID real asi
        *	@param as1 accounting schema	
        *	@param VAF_Org_ID real org																													
        *	@param costingMethod AcctSchema.COSTINGMETHOD_*
        *	@param qty qty
        *	@param VAB_OrderLine_ID optional order line
        *	@param zeroCostsOK zero/no costs are OK
        *	@param trxName trx
        *	@return current cost price or null
        */
        public static Decimal GetCurrentCost(MVAMProduct product, int VAM_PFeature_SetInstance_ID,
            VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID, String costingMethod, Decimal qty, int VAB_OrderLine_ID,
            bool zeroCostsOK, Trx trxName)
        {
            String CostingLevel = as1.GetCostingLevel();

            dynamic pca = null;
            if (as1.GetFRPT_LocAcct_ID() > 0)
            {
                pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                if (pca == null)
                    throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                        + product.GetVAM_ProductCategory_ID()
                        + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                //	Costing Level

                // change by Amit 7-4-2016
                //if (pca.GetFRPT_CostingLevel() != null)
                //    CostingLevel = pca.GetFRPT_CostingLevel();
                if (pca.GetCostingLevel() != null)
                    CostingLevel = pca.GetCostingLevel();
                //end

                //	Costing Method
                if (costingMethod == null)
                {
                    // change by Amit 7-4-2016
                    //costingMethod = pca.GetFRPT_CostingMethod();
                    costingMethod = pca.GetCostingMethod();
                    //end
                    if (costingMethod == null)
                    {
                        costingMethod = as1.GetCostingMethod();
                        if (costingMethod == null)
                            throw new ArgumentException("No Costing Method");
                        //		costingMethod = VAdvantage.Model.MAcctSchema.COSTINGMETHOD_StandardCosting;
                    }
                }
            }
            else
            {
                // Amit - 21-9-2016  
                // bcz Costing level consider from product category not from accounting tab
                //pca = MVAMProductCategoryAcct.Get(product.GetCtx(),
                //   product.GetVAM_ProductCategory_ID(), as1.GetVAB_AccountBook_ID(), null);
                pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                //end
                if (pca == null)
                    throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                        + product.GetVAM_ProductCategory_ID()
                        + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                //	Costing Level
                if (pca.GetCostingLevel() != null)
                    CostingLevel = pca.GetCostingLevel();
                //	Costing Method
                if (costingMethod == null)
                {
                    costingMethod = pca.GetCostingMethod();
                    if (costingMethod == null)
                    {
                        costingMethod = as1.GetCostingMethod();
                        if (costingMethod == null)
                            throw new ArgumentException("No Costing Method");
                        //		costingMethod = VAdvantage.Model.MAcctSchema.COSTINGMETHOD_StandardCosting;
                    }
                }
            }

            if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_Client.Equals(CostingLevel))
            {
                VAF_Org_ID = 0;
                VAM_PFeature_SetInstance_ID = 0;
            }
            else if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_Organization.Equals(CostingLevel))
                VAM_PFeature_SetInstance_ID = 0;
            else if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_BatchLot.Equals(CostingLevel))
                VAF_Org_ID = 0;

            //	Create/Update Costs
            MVAMVAMProductCostDetail.ProcessProduct(product, trxName);

            return Util.GetValueOfDecimal(GetCurrentCost(
                product, VAM_PFeature_SetInstance_ID,
                as1, VAF_Org_ID, as1.GetVAM_ProductCostType_ID(), costingMethod, qty,
                VAB_OrderLine_ID, zeroCostsOK, trxName));
        }

        /**
         * 	Get Current Cost Price for Costing Level
         *	@param product product
         *	@param M_ASI_ID costing level asi
         *	@param Org_ID costing level org
         *	@param VAM_ProductCostType_ID cost type
         *	@param as1 AcctSchema
         *	@param costingMethod method
         *	@param qty quantity
         *	@param VAB_OrderLine_ID optional order line
         *	@param zeroCostsOK zero/no costs are OK
         *	@param trxName trx
         *	@return cost price or null
         */
        private static Decimal? GetCurrentCost(MVAMProduct product, int M_ASI_ID,
            VAdvantage.Model.MVABAccountBook as1, int Org_ID, int VAM_ProductCostType_ID,
            String costingMethod, Decimal qty, int VAB_OrderLine_ID,
            bool zeroCostsOK, Trx trxName)
        {
            /**	Any Transactions not costed		*
            String sql1 = "SELECT * FROM VAM_Inv_Trx t " 
                + "WHERE t.VAM_Product_ID=?"
                + " AND NOT EXISTS (SELECT * FROM VAM_ProductCostDetail cd "
                    + "WHERE t.VAM_Product_ID=cd.VAM_Product_ID"
                    + " AND (t.VAM_Inv_InOutLine_ID=cd.VAM_Inv_InOutLine_ID))";
            PreparedStatement pstmt1 = null;
            List<MVAMInvTrx> list = new List<MVAMInvTrx>();
            try
            {
                pstmt1 = DataBase.prepareStatement (sql1, null);.
                pstmt1.setInt (1, product.getVAM_Product_ID());
                ResultSet dr = pstmt1.executeQuery ();
                while (dr.next ())
                {
                    MVAMInvTrx trx = new MVAMInvTrx(product.GetCtx(), dr, null);
                    list.Add (trx);
                }
                dr.close ();
                pstmt1.close ();
                pstmt1 = null;
            }
            catch (Exception e)
            {
                s_log.log (Level.SEVERE, sql1, e);
            }
            try
            {
                if (pstmt1 != null)
                    pstmt1.close ();
                pstmt1 = null;
            }
            catch (Exception e)
            {
                pstmt1 = null;
            }
            /**	*/

            // change by amit 18-4-2016
            // handling cost combination
            int costCombinationElement_ID = 0;
            dynamic pca = null;
            if (costingMethod == "C")
            {
                if (as1.GetFRPT_LocAcct_ID() > 0)
                {
                    pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                    if (pca == null)
                        throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                    //	Costing Element
                    if (Util.GetValueOfInt(pca.GetVAM_ProductCostElement_ID()) != 0)
                    {
                        costCombinationElement_ID = Util.GetValueOfInt(pca.GetVAM_ProductCostElement_ID());
                    }
                    else
                    {
                        costCombinationElement_ID = as1.GetVAM_ProductCostElement_ID();
                        if (costCombinationElement_ID == 0)
                            throw new ArgumentException("No Costing Element Selected");
                    }
                }
            }
            //end

            //	**
            Decimal? currentCostPrice = null;
            String costElementType = null;
            int VAM_ProductCostElement_ID = 0;
            Decimal? percent = null;
            //
            Decimal materialCostEach = Env.ZERO;
            Decimal otherCostEach = Env.ZERO;
            Decimal percentage = Env.ZERO;
            int count = 0;

            //Added by Amit 30-Mar-2017
            //pick current cost price against Asset if Asset ID available on VAM_Inv_InOut against VAB_Orderline
            int VAA_Asset_ID = 0;
            if (VAB_OrderLine_ID > 0)
            {
                VAA_Asset_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT inl.VAA_Asset_ID from  VAM_Inv_InOutLine inl 
                            INNER JOIN VAB_OrderLine odl ON (inl.VAB_OrderLine_ID = odl.VAB_OrderLine_ID) Where inl.IsActive='Y' 
                            AND inl.VAM_Product_ID=" + product.GetVAM_Product_ID() + " AND inl.VAB_OrderLine_ID=" + VAB_OrderLine_ID, null, trxName));
            }

            //
            //String sql = "SELECT SUM(c.CurrentCostPrice), ce.CostElementType, ce.CostingMethod,"
            //    + " NVL(c.PercentCost,0), c.VAM_ProductCostElement_ID "					//	4..5
            //    + "FROM VAM_ProductCost c"
            //    + " LEFT OUTER JOIN VAM_ProductCostElement ce ON (c.VAM_ProductCostElement_ID=ce.VAM_ProductCostElement_ID) "
            //    + "WHERE c.VAF_Client_ID=" + product.GetVAF_Client_ID() + " AND c.VAF_Org_ID=" + Org_ID		//	#1/2
            //    + " AND c.VAM_Product_ID=" + product.GetVAM_Product_ID()							//	#3
            //    + " AND (c.VAM_PFeature_SetInstance_ID=" + M_ASI_ID + " OR c.VAM_PFeature_SetInstance_ID=0)"	//	#4
            //    + " AND c.VAM_ProductCostType_ID=" + VAM_ProductCostType_ID + " AND c.VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID()	//	#5/6
            //    + " AND (ce.CostingMethod IS NULL OR ce.CostingMethod=@costingMethod) "	//	#7
            //    + "GROUP BY ce.CostElementType, ce.CostingMethod, c.PercentCost, c.VAM_ProductCostElement_ID";
            String sql = "SELECT SUM(c.CurrentCostPrice), ce.CostElementType, ce.CostingMethod,"
                + " NVL(c.PercentCost,0), c.VAM_ProductCostElement_ID "					//	4..5
                + "FROM VAM_ProductCost c"
                + " LEFT OUTER JOIN VAM_ProductCostElement ce ON (c.VAM_ProductCostElement_ID=ce.VAM_ProductCostElement_ID) "
                + "WHERE c.VAF_Client_ID=" + product.GetVAF_Client_ID() + " AND c.VAF_Org_ID=" + Org_ID		//	#1/2
                + " AND c.VAM_Product_ID=" + product.GetVAM_Product_ID()							//	#3
                + " AND (c.VAM_PFeature_SetInstance_ID=" + M_ASI_ID + " OR c.VAM_PFeature_SetInstance_ID=0)"	//	#4
                + " AND c.VAM_ProductCostType_ID=" + VAM_ProductCostType_ID + " AND c.VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();	//	#5/6
            if (!string.IsNullOrEmpty(costingMethod))
            {
                sql += " AND (ce.CostingMethod=@costingMethod) ";	//	#7
            }
            else
            {
                sql += " AND (ce.CostingMethod IS NULL OR ce.CostingMethod=@costingMethod) ";	//	#7
            }
            if (costingMethod == "C" && costCombinationElement_ID != 0)
            {
                sql += " AND c.VAM_ProductCostElement_ID = " + costCombinationElement_ID;
            }
            if (VAA_Asset_ID > 0)
            {
                sql += " AND c.VAA_Asset_ID = " + VAA_Asset_ID;
            }
            sql += " GROUP BY ce.CostElementType, ce.CostingMethod, c.PercentCost, c.VAM_ProductCostElement_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@costingMethod", costingMethod);
                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    currentCostPrice = Convert.ToDecimal(dr[0]);//.getBigDecimal(1);
                    costElementType = dr[1].ToString();
                    String cm = dr[2].ToString();
                    percent = Convert.ToDecimal(dr[3]);
                    VAM_ProductCostElement_ID = Convert.ToInt32(dr[4]);
                    _log.Finest("CurrentCostPrice=" + currentCostPrice
                        + ", CostElementType=" + costElementType
                       + ", CostingMethod=" + cm
                        + ", Percent=" + percent
                        + ", VAM_ProductCostElement_ID=" + VAM_ProductCostElement_ID);

                    if (currentCostPrice != null && Env.Signum((Decimal)currentCostPrice) != 0)
                    {
                        if (cm != null)
                            materialCostEach = Decimal.Add(materialCostEach, (Decimal)currentCostPrice);
                        else
                            otherCostEach = Decimal.Add(otherCostEach, (Decimal)currentCostPrice);
                    }
                    if (percent != null && Env.Signum((Decimal)percent) != 0)
                        percentage = Decimal.Add(percentage, (Decimal)percent);
                    count++;
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            if (count > 1)	//	Print summary
            {
                _log.Finest("MaterialCost=" + materialCostEach
                    + ", OtherCosts=" + otherCostEach
                    + ", Percentage=" + percentage);
            }

            //	Seed Initial Costs
            if (Env.Signum(materialCostEach) == 0)		//	no costs
            {
                if (zeroCostsOK)
                {
                    return Env.ZERO;
                }
                materialCostEach = Util.GetValueOfDecimal(GetSeedCosts(product, M_ASI_ID,
                    as1, Org_ID, costingMethod, VAB_OrderLine_ID));
            }
            if (materialCostEach == null || materialCostEach == 0)
            {
                return null;
            }

            //	Material Costs
            Decimal materialCost = Decimal.Multiply(materialCostEach, qty);
            //	Standard costs - just Material Costs
            if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting.Equals(costingMethod))
            {
                _log.Finer("MaterialCosts = " + materialCost);
                return materialCost;
            }
            if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_Fifo.Equals(costingMethod)
                || VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_Lifo.Equals(costingMethod))
            {
                VAdvantage.Model.MVAMVAMProductCostElement ce = VAdvantage.Model.MVAMVAMProductCostElement.GetMaterialCostElement(as1, costingMethod);
                materialCost = Util.GetValueOfDecimal(MVAMVAMProductCostQueue.GetCosts(product, M_ASI_ID, as1, Org_ID, ce, qty, trxName));
            }

            //	Other Costs
            Decimal otherCost = Decimal.Multiply(otherCostEach, qty);

            //	Costs
            Decimal costs = Decimal.Add(otherCost, materialCost);
            if (Env.Signum(costs) == 0)
            {
                return null;
            }

            _log.Finer("Sum Costs = " + costs);
            int precision = as1.GetCostingPrecision();
            if (Env.Signum(percentage) == 0)	//	no percentages
            {
                if (Env.Scale(costs) > precision)
                {
                    costs = Decimal.Round(costs, precision, MidpointRounding.AwayFromZero);
                    //costs = costs.setScale(precision, Decimal.ROUND_HALF_UP);
                }
                return costs;
            }
            //
            Decimal percentCost = Decimal.Multiply(costs, percentage);
            //percentCost = percentCost.divide(Env.ONEHUNDRED, precision, Decimal.ROUND_HALF_UP);
            percentCost = Decimal.Divide(percentCost, Decimal.Round(Env.ONEHUNDRED, precision, MidpointRounding.AwayFromZero));
            costs = Decimal.Add(costs, percentCost);
            if (Env.Scale(costs) > precision)
            {
                //costs = costs.setScale(precision, Decimal.ROUND_HALF_UP);
                costs = Decimal.Round(costs, precision, MidpointRounding.AwayFromZero);
            }
            _log.Finer("Sum Costs = " + costs + " (Add=" + percentCost + ")");
            return costs;
        }

        /**
         * 	Get Seed Costs
         *	@param product product
         *	@param M_ASI_ID costing level asi
         *	@param as1 accounting schema
         *	@param Org_ID costing level org
         *	@param costingMethod costing method
         *	@param VAB_OrderLine_ID optional order line
         *	@return price or null
         */
        public static Decimal? GetSeedCosts(MVAMProduct product, int M_ASI_ID,
            VAdvantage.Model.MVABAccountBook as1, int Org_ID, String costingMethod, int VAB_OrderLine_ID)
        {
            Decimal? retValue = null;
            //	Direct Data
            if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_AverageInvoice.Equals(costingMethod))
                retValue = CalculateAverageInv(product, M_ASI_ID, as1, Org_ID);
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_AveragePO.Equals(costingMethod))
                retValue = CalculateAveragePO(product, M_ASI_ID, as1, Org_ID);
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_Fifo.Equals(costingMethod))
                retValue = CalculateFiFo(product, M_ASI_ID, as1, Org_ID);
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_Lifo.Equals(costingMethod))
                retValue = CalculateLiFo(product, M_ASI_ID, as1, Org_ID);
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_LastInvoice.Equals(costingMethod))
                retValue = GetLastInvoicePrice(product, M_ASI_ID, Org_ID, as1.GetVAB_Currency_ID());
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_LastPOPrice.Equals(costingMethod))
            {
                if (VAB_OrderLine_ID != 0)
                    retValue = GetPOPrice(product, VAB_OrderLine_ID, as1.GetVAB_Currency_ID());
                if (retValue == null || Env.Signum((Decimal)retValue) == 0)
                    retValue = GetLastPOPrice(product, M_ASI_ID, Org_ID, as1.GetVAB_Currency_ID());
            }
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting.Equals(costingMethod))
            {
                //	migrate old costs
                MVAMProductCosting pc = MVAMProductCosting.Get(product.GetCtx(), product.GetVAM_Product_ID(),
                    as1.GetVAB_AccountBook_ID(), null);
                if (pc != null)
                    retValue = pc.GetCurrentCostPrice();
            }
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_UserDefined.Equals(costingMethod))
            {
                ;
            }
            else if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_CostCombination.Equals(costingMethod))
            {
                return 0;
            }
            else
                throw new ArgumentException("Unknown Costing Method = " + costingMethod);
            if (retValue != null && Env.Signum((Decimal)retValue) != 0)
            {
                _log.Fine(product.GetName() + ", CostingMethod=" + costingMethod + " - " + retValue);
                return retValue;
            }

            //	Look for exact Order Line
            if (VAB_OrderLine_ID != 0)
            {
                retValue = GetPOPrice(product, VAB_OrderLine_ID, as1.GetVAB_Currency_ID());
                if (retValue != null && Env.Signum((Decimal)retValue) != 0)
                {
                    _log.Fine(product.GetName() + ", VAdvantage.Model.PO - " + retValue);
                    return retValue;
                }
            }

            //	Look for Standard Costs first
            if (!VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting.Equals(costingMethod))
            {
                VAdvantage.Model.MVAMVAMProductCostElement ce = VAdvantage.Model.MVAMVAMProductCostElement.GetMaterialCostElement(as1, VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting);
                MVAMVAMProductCost cost = Get(product, M_ASI_ID, as1, Org_ID, ce.GetVAM_ProductCostElement_ID());
                if (cost != null && Env.Signum(cost.GetCurrentCostPrice()) != 0)
                {
                    _log.Fine(product.GetName() + ", Standard - " + retValue);
                    return cost.GetCurrentCostPrice();
                }
            }

            //	We do not have a price
            //	VAdvantage.Model.PO first
            if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_AveragePO.Equals(costingMethod)
                || VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_LastPOPrice.Equals(costingMethod)
                || VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting.Equals(costingMethod))
            {
                //	try Last VAdvantage.Model.PO
                retValue = GetLastPOPrice(product, M_ASI_ID, Org_ID, as1.GetVAB_Currency_ID());
                if (Org_ID != 0 && (retValue == null || Env.Signum(System.Convert.ToDecimal(retValue)) == 0))
                    retValue = GetLastPOPrice(product, M_ASI_ID, 0, as1.GetVAB_Currency_ID());
                if (retValue != null && Env.Signum(System.Convert.ToDecimal(retValue)) != 0)
                {
                    _log.Fine(product.GetName() + ", LastPO = " + retValue);
                    return retValue;
                }
            }
            else	//	Inv first
            {
                //	try last Inv
                retValue = GetLastInvoicePrice(product, M_ASI_ID, Org_ID, as1.GetVAB_Currency_ID());
                if (Org_ID != 0 && (retValue == null || Env.Signum((Decimal)retValue) == 0))
                    retValue = GetLastInvoicePrice(product, M_ASI_ID, 0, as1.GetVAB_Currency_ID());
                if (retValue != null && Env.Signum(System.Convert.ToDecimal(retValue)) != 0)
                {
                    _log.Fine(product.GetName() + ", LastInv = " + retValue);
                    return retValue;
                }
            }

            //	Still Nothing
            //	Inv second
            if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_AveragePO.Equals(costingMethod)
                || VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_LastPOPrice.Equals(costingMethod)
                || VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting.Equals(costingMethod))
            {
                //	try last Inv
                retValue = GetLastInvoicePrice(product, M_ASI_ID, Org_ID, as1.GetVAB_Currency_ID());
                if (Org_ID != 0 && (retValue == null || Env.Signum(System.Convert.ToDecimal(retValue)) == 0))
                    retValue = GetLastInvoicePrice(product, M_ASI_ID, 0, as1.GetVAB_Currency_ID());
                if (retValue != null && Env.Signum(System.Convert.ToDecimal(retValue)) != 0)
                {
                    _log.Fine(product.GetName() + ", LastInv = " + retValue);
                    return System.Convert.ToDecimal(retValue);
                }
            }
            else	//	VAdvantage.Model.PO second
            {
                //	try Last VAdvantage.Model.PO
                retValue = GetLastPOPrice(product, M_ASI_ID, Org_ID, as1.GetVAB_Currency_ID());
                if (Org_ID != 0 && (retValue == null || Env.Signum(System.Convert.ToDecimal(retValue)) == 0))
                    retValue = GetLastPOPrice(product, M_ASI_ID, 0, as1.GetVAB_Currency_ID());
                if (retValue != null && Env.Signum(System.Convert.ToDecimal(retValue)) != 0)
                {
                    _log.Fine(product.GetName() + ", LastPO = " + retValue);
                    return System.Convert.ToDecimal(retValue);
                }
            }

            //	Still nothing try ProductPO
            MVAMProductPO[] pos = MVAMProductPO.GetOfProduct(product.GetCtx(), product.GetVAM_Product_ID(), null);
            for (int i = 0; i < pos.Length; i++)
            {
                Decimal price = pos[i].GetPricePO();
                if (price == null || Env.Signum(price) == 0)
                    price = pos[0].GetPriceList();
                if (price != null && Env.Signum(price) != 0)
                {
                    price = VAdvantage.Model.MVABExchangeRate.Convert(product.GetCtx(), price,
                        pos[0].GetVAB_Currency_ID(), as1.GetVAB_Currency_ID(),
                        as1.GetVAF_Client_ID(), Org_ID);
                    if (price != null && Env.Signum(price) != 0)
                    {
                        retValue = price;
                        _log.Fine(product.GetName() + ", Product_PO = " + retValue);
                        return System.Convert.ToDecimal(retValue);
                    }
                }
            }

            //	Still nothing try Purchase Price List
            //	....

            _log.Fine(product.GetName() + " = " + retValue);
            return System.Convert.ToDecimal(retValue);
        }


        /**
         * 	Get Last Invoice Price in currency
         *	@param product product
         *	@param M_ASI_ID attribute set instance
         *	@param VAF_Org_ID org
         *	@param VAB_Currency_ID accounting currency
         *	@return last invoice price in currency
         */
        public static Decimal? GetLastInvoicePrice(MVAMProduct product,
            int M_ASI_ID, int VAF_Org_ID, int VAB_Currency_ID)
        {
            Decimal? retValue = null;
            String sql = "SELECT currencyConvert(il.PriceActual, i.VAB_Currency_ID," + VAB_Currency_ID + ", i.DateAcct, i.VAB_CurrencyType_ID, il.VAF_Client_ID, il.VAF_Org_ID) "
                // ,il.PriceActual, il.QtyInvoiced, i.DateInvoiced, il.Line
                + "FROM VAB_InvoiceLine il "
                + " INNER JOIN VAB_Invoice i ON (il.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                + "WHERE il.VAM_Product_ID=" + product.GetVAM_Product_ID()
                + " AND i.IsSOTrx='N'";
            if (VAF_Org_ID != 0)
                sql += " AND il.VAF_Org_ID=" + VAF_Org_ID;
            else if (M_ASI_ID != 0)
                sql += " AND il.VAM_PFeature_SetInstance_ID=" + M_ASI_ID;
            sql += " ORDER BY i.DateInvoiced DESC, il.Line DESC";
            //
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, product.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = Util.GetValueOfDecimal(dr[0].ToString());
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            if (retValue != null)
            {
                _log.Finer(product.GetName() + " = " + retValue);
                return retValue;
            }
            return null;
        }

        /**
         * 	Get Last VAdvantage.Model.PO Price in currency
         *	@param product product
         *	@param M_ASI_ID attribute set instance
         *	@param VAF_Org_ID org
         *	@param VAB_Currency_ID accounting currency
         *	@return last VAdvantage.Model.PO price in currency or null
         */
        public static Decimal? GetLastPOPrice(MVAMProduct product, int M_ASI_ID, int VAF_Org_ID, int VAB_Currency_ID)
        {
            Decimal? retValue = null;
            String sql = "SELECT currencyConvert(ol.PriceCost, o.VAB_Currency_ID," + VAB_Currency_ID + ", o.DateAcct, o.VAB_CurrencyType_ID, ol.VAF_Client_ID, ol.VAF_Org_ID),"
                + " currencyConvert(ol.PriceActual, o.VAB_Currency_ID," + VAB_Currency_ID + ", o.DateAcct, o.VAB_CurrencyType_ID, ol.VAF_Client_ID, ol.VAF_Org_ID) "
                //	,ol.PriceCost,ol.PriceActual, ol.QtyOrdered, o.DateOrdered, ol.Line
                + "FROM VAB_OrderLine ol"
                + " INNER JOIN VAB_Order o ON (ol.VAB_Order_ID=o.VAB_Order_ID) "
                + "WHERE ol.VAM_Product_ID=" + product.GetVAM_Product_ID()
                + " AND o.IsSOTrx='N'";
            if (VAF_Org_ID != 0)
                sql += " AND ol.VAF_Org_ID=" + VAF_Org_ID;
            else if (M_ASI_ID != 0)
                sql += " AND t.VAM_PFeature_SetInstance_ID=" + M_ASI_ID;
            sql += " ORDER BY o.DateOrdered DESC, ol.Line DESC";
            //
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, product.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = Util.GetValueOfDecimal(dr[0].ToString());
                    if (retValue == null || Env.Signum(System.Convert.ToDecimal(retValue)) == 0)
                    {
                        retValue = Util.GetValueOfDecimal(dr[1].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            if (retValue != null)
            {
                _log.Finer(product.GetName() + " = " + retValue);
                return retValue;
            }
            return null;
        }

        /**
         * 	Get VAdvantage.Model.PO Price in currency
         * 	@param product product
         *	@param VAB_OrderLine_ID order line
         *	@param VAB_Currency_ID accounting currency
         *	@return last VAdvantage.Model.PO price in currency or null
         */
        public static Decimal? GetPOPrice(MVAMProduct product, int VAB_OrderLine_ID, int VAB_Currency_ID)
        {
            Decimal? retValue = null;
            String sql = "SELECT currencyConvert(ol.PriceCost, o.VAB_Currency_ID, " + VAB_Currency_ID + ", o.DateAcct, o.VAB_CurrencyType_ID, ol.VAF_Client_ID, ol.VAF_Org_ID),"
                + " currencyConvert(ol.PriceActual, o.VAB_Currency_ID, " + VAB_Currency_ID + ", o.DateAcct, o.VAB_CurrencyType_ID, ol.VAF_Client_ID, ol.VAF_Org_ID) "
                //	,ol.PriceCost,ol.PriceActual, ol.QtyOrdered, o.DateOrdered, ol.Line
                + "FROM VAB_OrderLine ol"
                + " INNER JOIN VAB_Order o ON (ol.VAB_Order_ID=o.VAB_Order_ID) "
                + "WHERE ol.VAB_OrderLine_ID=" + VAB_OrderLine_ID
                + " AND o.IsSOTrx='N'";
            //
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, product.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = Util.GetValueOfDecimal(dr[0].ToString());
                    if (retValue == null || Env.Signum(System.Convert.ToDecimal(retValue)) == 0)
                    {
                        retValue = Util.GetValueOfDecimal(dr[1].ToString());
                    }
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            if (retValue != null)
            {
                _log.Finer(product.GetName() + " = " + retValue);
                return retValue;
            }
            return null;
        }

        /***
         * 	Create costing for client.
         * 	Handles Transaction if not in a transaction
         *	@param client client
         */
        public static void Create(VAdvantage.Model.MVAFClient client)
        {
            VAdvantage.Model.MVABAccountBook[] ass = VAdvantage.Model.MVABAccountBook.GetClientAcctSchema(client.GetCtx(), client.GetVAF_Client_ID());
            Trx trx = client.Get_Trx();
            //String trxNameUsed = trxName;
            if (trx == null)
            {
                //trxNameUsed = Trx.CreateTrxName("Cost");
                trx = Trx.Get("Cost");
            }
            bool success = true;
            //	For all Products
            String sql = "SELECT * FROM VAM_Product p "
                + "WHERE VAF_Client_ID=" + client.GetVAF_Client_ID()
                + " AND EXISTS (SELECT * FROM VAM_ProductCostDetail cd "
                    + "WHERE p.VAM_Product_ID=cd.VAM_Product_ID AND Processed='N')";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MVAMProduct product = new MVAMProduct(client.GetCtx(), dr, trx);
                    for (int i = 0; i < ass.Length; i++)
                    {
                        Decimal cost = GetCurrentCost(product, 0, ass[i], 0,
                            null, Env.ONE, 0, false, trx);		//	create non-zero costs
                        _log.Info(product.GetName() + " = " + cost);
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
                success = false;
            }
            finally { dt = null; }

            //	Transaction
            if (trx != null)
            {
                if (success)
                {
                    trx.Commit();
                }
                else
                {
                    trx.Rollback();
                }
                trx.Close();
            }
        }

        /**
         * 	Create standard Costing records for Product
         *	@param product product
         */
        public static void Create(MVAMProduct product)
        {
            _log.Config(product.GetName());
            //	Cost Elements
            VAdvantage.Model.MVAMVAMProductCostElement[] ces = VAdvantage.Model.MVAMVAMProductCostElement.GetCostingMethods(product);
            VAdvantage.Model.MVAMVAMProductCostElement ce = null;
            for (int i = 0; i < ces.Length; i++)
            {
                if (VAdvantage.Model.MVAMVAMProductCostElement.COSTINGMETHOD_StandardCosting.Equals(ces[i].GetCostingMethod()))
                {
                    ce = ces[i];
                    break;
                }
            }
            if (ce == null)
            {
                _log.Fine("No Standard Costing in System");
                return;
            }

            VAdvantage.Model.MVABAccountBook[] mass = VAdvantage.Model.MVABAccountBook.GetClientAcctSchema(product.GetCtx(),
                product.GetVAF_Client_ID(), product.Get_TrxName());
            VAdvantage.Model.MVAFOrg[] orgs = null;

            int M_ASI_ID = 0;		//	No Attribute
            for (int i = 0; i < mass.Length; i++)
            {
                VAdvantage.Model.MVABAccountBook as1 = mass[i];

                String cl = null;
                dynamic pca = null;
                if (as1.GetFRPT_LocAcct_ID() > 0)
                {
                    pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                    if (pca != null)
                    {
                        //cl = pca.GetFRPT_CostingLevel();
                        cl = pca.GetCostingLevel();
                    }
                }
                else
                {
                    pca = MVAMProductCategoryAcct.Get(product.GetCtx(),
                       product.GetVAM_ProductCategory_ID(), as1.GetVAB_AccountBook_ID(), product.Get_TrxName());
                    if (pca != null)
                    {
                        cl = pca.GetCostingLevel();
                    }
                }

                if (cl == null)
                    cl = as1.GetCostingLevel();
                //	Create Std Costing
                if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_Client.Equals(cl))
                {
                    MVAMVAMProductCost cost = MVAMVAMProductCost.Get(product, M_ASI_ID,
                        as1, 0, ce.GetVAM_ProductCostElement_ID());
                    if (cost.Is_New())
                    {
                        if (cost.Save())
                        {
                            _log.Config("Std.Cost for " + product.GetName() + " - " + as1.GetName());
                        }
                        else
                        {
                            _log.Warning("Not created: Std.Cost for " + product.GetName() + " - " + as1.GetName());
                        }
                    }
                }
                else if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_Organization.Equals(cl))
                {
                    if (orgs == null)
                        orgs = VAdvantage.Model.MVAFOrg.GetOfClient(product);
                    for (int o = 0; o < orgs.Length; o++)
                    {
                        MVAMVAMProductCost cost = MVAMVAMProductCost.Get(product, M_ASI_ID,
                            as1, orgs[o].GetVAF_Org_ID(), ce.GetVAM_ProductCostElement_ID());
                        if (cost.Is_New())
                        {
                            if (cost.Save())
                            {
                                _log.Config("Std.Cost for " + product.GetName()
                                   + " - " + orgs[o].GetName()
                                   + " - " + as1.GetName());
                            }
                            else
                            {
                                _log.Warning("Not created: Std.Cost for " + product.GetName()
                                    + " - " + orgs[o].GetName()
                                    + " - " + as1.GetName());
                            }
                        }
                    }	//	for all orgs
                }
                else
                {
                    _log.Warning("Not created: Std.Cost for " + product.GetName() + " - Costing Level on Batch/Lot");
                }
            }	//	accounting schema loop

        }

        // By Amit 23-12-2015
        public static void CreateRecords(MVAMProduct product)
        {
            _log.Config(product.GetName());
            //	Cost Elements
            VAdvantage.Model.MVAMVAMProductCostElement[] ces = VAdvantage.Model.MVAMVAMProductCostElement.GetCostingMethods(product);
            VAdvantage.Model.MVAMVAMProductCostElement ce = null;
            for (int j = 0; j < ces.Length; j++)
            {
                ce = ces[j];
                VAdvantage.Model.MVABAccountBook[] mass = VAdvantage.Model.MVABAccountBook.GetClientAcctSchemas(product.GetCtx(),
            product.GetVAF_Client_ID(), product.Get_TrxName());
                VAdvantage.Model.MVAFOrg[] orgs = null;

                int M_ASI_ID = 0;		//	No Attribute
                for (int i = 0; i < mass.Length; i++)
                {
                    VAdvantage.Model.MVABAccountBook as1 = mass[i];

                    String cl = null;
                    dynamic pca = null;
                    if (as1.GetFRPT_LocAcct_ID() > 0)
                    {
                        pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                        if (pca != null)
                        {
                            //cl = pca.GetFRPT_CostingLevel();
                            cl = pca.GetCostingLevel();
                        }
                    }
                    else
                    {
                        //pca = MVAMProductCategoryAcct.Get(product.GetCtx(),
                        //   product.GetVAM_ProductCategory_ID(), as1.GetVAB_AccountBook_ID(), product.Get_TrxName());
                        pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                        if (pca != null)
                        {
                            cl = pca.GetCostingLevel();
                        }
                    }

                    if (cl == null)
                        cl = as1.GetCostingLevel();

                    if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_Client.Equals(cl))
                    {
                        MVAMVAMProductCost cost = MVAMVAMProductCost.Get(product, M_ASI_ID,
                            as1, 0, ce.GetVAM_ProductCostElement_ID());
                        if (cost.Is_New())
                        {
                            if (cost.Save())
                            {
                                _log.Config("Std.Cost for " + product.GetName() + " - " + as1.GetName());
                            }
                            else
                            {
                                _log.Warning("Not created: Std.Cost for " + product.GetName() + " - " + as1.GetName());
                            }
                        }
                    }
                    else if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_Organization.Equals(cl))
                    {
                        if (orgs == null)
                            orgs = VAdvantage.Model.MVAFOrg.GetOfClient(product);
                        for (int o = 0; o < orgs.Length; o++)
                        {
                            MVAMVAMProductCost cost = MVAMVAMProductCost.Get(product, M_ASI_ID,
                                as1, orgs[o].GetVAF_Org_ID(), ce.GetVAM_ProductCostElement_ID());
                            if (cost.Is_New())
                            {
                                if (cost.Save())
                                {
                                    _log.Config("Cost Element for " + product.GetName()
                                       + " - " + orgs[o].GetName()
                                       + " - " + as1.GetName());
                                }
                                else
                                {
                                    _log.Warning("Not created: Cost Element for " + product.GetName()
                                        + " - " + orgs[o].GetName()
                                        + " - " + as1.GetName());
                                }
                            }
                        }	//	for all orgs
                    }
                    else
                    {
                        _log.Warning("Not created: Cost Element for " + product.GetName() + " - Costing Level on Batch/Lot");
                    }
                }	//	accounting schema loop
            }
            if (ce == null)
            {
                _log.Fine("No Standard Costing in System");
                return;
            }
        }

        /**
         * 	Calculate Average Invoice from Trx
         *	@param product product
         *	@param VAM_PFeature_SetInstance_ID optional asi
         *	@param as1 acct schema
         *	@param VAF_Org_ID optonal org
         *	@return average costs or null
         */
        public static Decimal? CalculateAverageInv(MVAMProduct product, int VAM_PFeature_SetInstance_ID, VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID)
        {
            String sql = "SELECT t.MovementQty, mi.Qty, il.QtyInvoiced, il.PriceActual,"
                + " i.VAB_Currency_ID, i.DateAcct, i.VAB_CurrencyType_ID, i.VAF_Client_ID, i.VAF_Org_ID, t.VAM_Inv_Trx_ID "
                + "FROM VAM_Inv_Trx t"
                + " INNER JOIN VAM_MatchInvoice mi ON (t.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID)"
                + " INNER JOIN VAB_InvoiceLine il ON (mi.VAB_InvoiceLine_ID=il.VAB_InvoiceLine_ID)"
                + " INNER JOIN VAB_Invoice i ON (il.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                + "WHERE t.VAM_Product_ID=" + product.GetVAM_Product_ID();
            if (VAF_Org_ID != 0)
                sql += " AND t.VAF_Org_ID=" + VAF_Org_ID;
            else if (VAM_PFeature_SetInstance_ID != 0)
                sql += " AND t.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            sql += " ORDER BY t.VAM_Inv_Trx_ID";

            DataTable dt = null;
            Decimal newStockQty = Env.ZERO;
            //
            Decimal newAverageAmt = Env.ZERO;
            int oldTransaction_ID = 0;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Decimal oldStockQty = newStockQty;
                    Decimal movementQty = Util.GetValueOfDecimal(dr[0].ToString());
                    int VAM_Inv_Trx_ID = Util.GetValueOfInt(dr[9].ToString());//.getInt(10);
                    if (VAM_Inv_Trx_ID != oldTransaction_ID)
                    {
                        newStockQty = Decimal.Add(oldStockQty, movementQty);
                    }
                    VAM_Inv_Trx_ID = oldTransaction_ID;
                    //
                    Decimal matchQty = Util.GetValueOfDecimal(dr[1].ToString());
                    if (matchQty == null)
                    {
                        _log.Finer("Movement=" + movementQty + ", StockQty=" + newStockQty);
                        continue;
                    }
                    //	Assumption: everything is matched
                    Decimal price = Util.GetValueOfDecimal(dr[3].ToString());
                    int VAB_Currency_ID = Util.GetValueOfInt(dr[4].ToString());
                    DateTime? DateAcct = Util.GetValueOfDateTime(dr[5]);
                    int VAB_CurrencyType_ID = Util.GetValueOfInt(dr[6].ToString());
                    int Client_ID = Util.GetValueOfInt(dr[7].ToString());
                    int Org_ID = Util.GetValueOfInt(dr[8].ToString());
                    Decimal cost = VAdvantage.Model.MVABExchangeRate.Convert(product.GetCtx(), price,
                        VAB_Currency_ID, as1.GetVAB_Currency_ID(),
                        DateAcct, VAB_CurrencyType_ID, Client_ID, Org_ID);
                    //
                    Decimal oldAverageAmt = newAverageAmt;
                    Decimal averageCurrent = Decimal.Multiply(oldStockQty, oldAverageAmt);
                    Decimal averageIncrease = Decimal.Multiply(matchQty, cost);
                    Decimal newAmt = Decimal.Add(averageCurrent, averageIncrease);
                    newAmt = Decimal.Round(newAmt, as1.GetCostingPrecision());
                    newAverageAmt = Decimal.Divide(newAmt, Decimal.Round(newStockQty, as1.GetCostingPrecision(), MidpointRounding.AwayFromZero));
                    _log.Finer("Movement=" + movementQty + ", StockQty=" + newStockQty
                       + ", Match=" + matchQty + ", Cost=" + cost + ", NewAvg=" + newAverageAmt);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            //
            if (Env.Signum(newAverageAmt) != 0)
            {
                _log.Finer(product.GetName() + " = " + newAverageAmt);
                return newAverageAmt;
            }
            return null;
        }

        /**
         * 	Calculate Average VAdvantage.Model.PO
         *	@param product product
         *	@param VAM_PFeature_SetInstance_ID asi
         *	@param as1 acct schema
         *	@param VAF_Org_ID org
         *	@return costs or null
         */
        public static Decimal? CalculateAveragePO(MVAMProduct product, int VAM_PFeature_SetInstance_ID,
            VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID)
        {
            String sql = "SELECT t.MovementQty, mp.Qty, ol.QtyOrdered, ol.PriceCost, ol.PriceActual,"	//	1..5
                + " o.VAB_Currency_ID, o.DateAcct, o.VAB_CurrencyType_ID,"	//	6..8
                + " o.VAF_Client_ID, o.VAF_Org_ID, t.VAM_Inv_Trx_ID "		//	9..11
                + "FROM VAM_Inv_Trx t"
                + " INNER JOIN VAM_MatchPO mp ON (t.VAM_Inv_InOutLine_ID=mp.VAM_Inv_InOutLine_ID)"
                + " INNER JOIN VAB_OrderLine ol ON (mp.VAB_OrderLine_ID=ol.VAB_OrderLine_ID)"
                + " INNER JOIN VAB_Order o ON (ol.VAB_Order_ID=o.VAB_Order_ID) "
                + "WHERE t.VAM_Product_ID=" + product.GetVAM_Product_ID();
            if (VAF_Org_ID != 0)
                sql += " AND t.VAF_Org_ID=" + VAF_Org_ID;
            else if (VAM_PFeature_SetInstance_ID != 0)
                sql += " AND t.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            sql += " ORDER BY t.VAM_Inv_Trx_ID";

            DataTable dt = null;
            Decimal newStockQty = Env.ZERO;
            //
            Decimal newAverageAmt = Env.ZERO;
            int oldTransaction_ID = 0;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Decimal oldStockQty = newStockQty;
                    Decimal movementQty = Util.GetValueOfDecimal(dr[0].ToString());
                    int VAM_Inv_Trx_ID = Util.GetValueOfInt(dr[10].ToString());
                    if (VAM_Inv_Trx_ID != oldTransaction_ID)
                    {
                        newStockQty = Decimal.Add(oldStockQty, movementQty);
                    }
                    VAM_Inv_Trx_ID = oldTransaction_ID;
                    //
                    Decimal matchQty = Util.GetValueOfDecimal(dr[1].ToString());
                    if (matchQty == null)
                    {
                        _log.Finer("Movement=" + movementQty + ", StockQty=" + newStockQty);
                        continue;
                    }
                    //	Assumption: everything is matched
                    Decimal price = Util.GetValueOfDecimal(dr[3].ToString());
                    if (price == null || Env.Signum(price) == 0)	//	VAdvantage.Model.PO Cost
                    {
                        price = Util.GetValueOfDecimal(dr[4].ToString());
                    }
                    int VAB_Currency_ID = Util.GetValueOfInt(dr[5].ToString());
                    DateTime? DateAcct = Util.GetValueOfDateTime(dr[6]);
                    int VAB_CurrencyType_ID = Util.GetValueOfInt(dr[7].ToString());
                    int Client_ID = Util.GetValueOfInt(dr[8].ToString());
                    int Org_ID = Util.GetValueOfInt(dr[9].ToString());
                    Decimal cost = VAdvantage.Model.MVABExchangeRate.Convert(product.GetCtx(), price,
                        VAB_Currency_ID, as1.GetVAB_Currency_ID(),
                        DateAcct, VAB_CurrencyType_ID, Client_ID, Org_ID);
                    //
                    Decimal oldAverageAmt = newAverageAmt;
                    Decimal averageCurrent = Decimal.Multiply(oldStockQty, oldAverageAmt);
                    Decimal averageIncrease = Decimal.Multiply(matchQty, cost);
                    Decimal newAmt = Decimal.Add(averageCurrent, averageIncrease);
                    newAmt = Decimal.Round(newAmt, (as1.GetCostingPrecision()));
                    newAverageAmt = Decimal.Divide(newAmt, Decimal.Round(newStockQty, as1.GetCostingPrecision(), MidpointRounding.AwayFromZero));
                    _log.Finer("Movement=" + movementQty + ", StockQty=" + newStockQty
                        + ", Match=" + matchQty + ", Cost=" + cost + ", NewAvg=" + newAverageAmt);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            if (newAverageAmt != null & Env.Signum(newAverageAmt) != 0)
            {
                _log.Finer(product.GetName() + " = " + newAverageAmt);
                return newAverageAmt;
            }
            return null;
        }

        /**
         * 	Calculate FiFo Cost
         *	@param product product
         *	@param VAM_PFeature_SetInstance_ID asi
         *	@param as1 acct schema
         *	@param VAF_Org_ID org
         *	@return costs or null
         */
        public static Decimal? CalculateFiFo(MVAMProduct product, int VAM_PFeature_SetInstance_ID, VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID)
        {
            String sql = "SELECT t.MovementQty, mi.Qty, il.QtyInvoiced, il.PriceActual,"
                + " i.VAB_Currency_ID, i.DateAcct, i.VAB_CurrencyType_ID, i.VAF_Client_ID, i.VAF_Org_ID, t.VAM_Inv_Trx_ID "
                + "FROM VAM_Inv_Trx t"
                + " INNER JOIN VAM_MatchInvoice mi ON (t.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID)"
                + " INNER JOIN VAB_InvoiceLine il ON (mi.VAB_InvoiceLine_ID=il.VAB_InvoiceLine_ID)"
                + " INNER JOIN VAB_Invoice i ON (il.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                + "WHERE t.VAM_Product_ID=" + product.GetVAM_Product_ID();
            if (VAF_Org_ID != 0)
                sql += " AND t.VAF_Org_ID=" + VAF_Org_ID;
            else if (VAM_PFeature_SetInstance_ID != 0)
                sql += " AND t.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            sql += " ORDER BY t.VAM_Inv_Trx_ID";

            DataTable dt = null;
            //
            int oldTransaction_ID = 0;
            List<QtyCost> fifo = new List<QtyCost>();
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Decimal movementQty = Util.GetValueOfDecimal(dr[0].ToString());
                    int VAM_Inv_Trx_ID = Util.GetValueOfInt(dr[9].ToString());
                    if (VAM_Inv_Trx_ID == oldTransaction_ID)
                    {
                        continue;	//	assuming same price for receipt
                    }
                    VAM_Inv_Trx_ID = oldTransaction_ID;
                    //
                    Decimal matchQty = Util.GetValueOfDecimal(dr[1].ToString());
                    if (matchQty == null)	//	out (negative)
                    {
                        if (fifo.Count > 0)
                        {
                            QtyCost pp = (QtyCost)fifo[0];
                            pp.Qty = Decimal.Add((Decimal)pp.Qty, movementQty);
                            Decimal remainder = (Decimal)pp.Qty;
                            if (Env.Signum(remainder) == 0)
                            {
                                fifo.RemoveAt(0);
                            }
                            else
                            {
                                while (Env.Signum(remainder) != 0)
                                {
                                    if (fifo.Count == 1)	//	Last
                                    {
                                        pp.Cost = Env.ZERO;
                                        remainder = Env.ZERO;
                                    }
                                    else
                                    {
                                        fifo.RemoveAt(0);
                                        pp = (QtyCost)fifo[0];
                                        pp.Qty = Decimal.Add((Decimal)pp.Qty, movementQty);
                                        remainder = (Decimal)pp.Qty;
                                    }
                                }
                            }
                        }
                        else
                        {
                            QtyCost pp = new QtyCost(movementQty, Env.ZERO);
                            fifo.Add(pp);
                        }
                        _log.Finer("Movement=" + movementQty + ", Size=" + fifo.Count);
                        continue;
                    }
                    //	Assumption: everything is matched
                    Decimal price = Util.GetValueOfDecimal(dr[3].ToString());
                    int VAB_Currency_ID = Util.GetValueOfInt(dr[4].ToString());
                    DateTime? DateAcct = Util.GetValueOfDateTime(dr[5]);
                    int VAB_CurrencyType_ID = Util.GetValueOfInt(dr[6].ToString());
                    int Client_ID = Util.GetValueOfInt(dr[7].ToString());
                    int Org_ID = Util.GetValueOfInt(dr[8].ToString());
                    Decimal cost = VAdvantage.Model.MVABExchangeRate.Convert(product.GetCtx(), price,
                        VAB_Currency_ID, as1.GetVAB_Currency_ID(),
                        DateAcct, VAB_CurrencyType_ID, Client_ID, Org_ID);

                    //	Add Stock
                    bool used = false;
                    if (fifo.Count == 1)
                    {
                        QtyCost pp = (QtyCost)fifo[0];
                        //if (pp.Qty.signum() < 0)
                        if (Env.Signum(System.Convert.ToDecimal(pp.Qty)) < 0)
                        {
                            pp.Qty = Decimal.Add(System.Convert.ToDecimal(pp.Qty), movementQty);
                            if (Env.Signum(System.Convert.ToDecimal(pp.Qty)) == 0)
                            {
                                fifo.RemoveAt(0);
                            }
                            else
                            {
                                pp.Cost = cost;
                            }
                            used = true;
                        }
                    }
                    if (!used)
                    {
                        QtyCost pp = new QtyCost(movementQty, cost);
                        fifo.Add(pp);
                    }
                    _log.Finer("Movement=" + movementQty + ", Size=" + fifo.Count);
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }

            if (fifo.Count == 0)
                return null;
            QtyCost pp1 = (QtyCost)fifo[0];
            _log.Finer(product.GetName() + " = " + pp1.Cost);
            return pp1.Cost;
        }

        /**
         * 	Calculate LiFo costs
         *	@param product product
         *	@param VAM_PFeature_SetInstance_ID asi
         *	@param as1 acct schema
         *	@param VAF_Org_ID org
         *	@return costs or null
         */
        public static Decimal? CalculateLiFo(MVAMProduct product, int VAM_PFeature_SetInstance_ID,
            VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID)
        {
            String sql = "SELECT t.MovementQty, mi.Qty, il.QtyInvoiced, il.PriceActual,"
                + " i.VAB_Currency_ID, i.DateAcct, i.VAB_CurrencyType_ID, i.VAF_Client_ID, i.VAF_Org_ID, t.VAM_Inv_Trx_ID "
                + "FROM VAM_Inv_Trx t"
                + " INNER JOIN VAM_MatchInvoice mi ON (t.VAM_Inv_InOutLine_ID=mi.VAM_Inv_InOutLine_ID)"
                + " INNER JOIN VAB_InvoiceLine il ON (mi.VAB_InvoiceLine_ID=il.VAB_InvoiceLine_ID)"
                + " INNER JOIN VAB_Invoice i ON (il.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                + "WHERE t.VAM_Product_ID=" + product.GetVAM_Product_ID();
            if (VAF_Org_ID != 0)
                sql += " AND t.VAF_Org_ID=" + VAF_Org_ID;
            else if (VAM_PFeature_SetInstance_ID != 0)
                sql += " AND t.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            //	Starting point?
            sql += " ORDER BY t.VAM_Inv_Trx_ID DESC";

            DataTable dt = null;
            //
            int oldTransaction_ID = 0;
            List<QtyCost> lifo = new List<QtyCost>();
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Decimal movementQty = Util.GetValueOfDecimal(dr[0].ToString());
                    int VAM_Inv_Trx_ID = Util.GetValueOfInt(dr[9].ToString());
                    if (VAM_Inv_Trx_ID == oldTransaction_ID)
                    {
                        continue;	//	assuming same price for receipt
                    }
                    VAM_Inv_Trx_ID = oldTransaction_ID;
                    //
                    Decimal matchQty = Util.GetValueOfDecimal(dr[1].ToString());
                    if (matchQty == null)	//	out (negative)
                    {
                        if (lifo.Count > 0)
                        {
                            QtyCost pp = (QtyCost)lifo[lifo.Count - 1];
                            pp.Qty = Decimal.Add((Decimal)pp.Qty, movementQty);
                            Decimal remainder = (Decimal)pp.Qty;
                            if (Env.Signum(remainder) == 0)
                            {
                                lifo.RemoveAt(lifo.Count - 1);
                            }
                            else
                            {
                                while (Env.Signum(remainder) != 0)
                                {
                                    if (lifo.Count == 1)	//	Last
                                    {
                                        pp.Cost = Env.ZERO;
                                        remainder = Env.ZERO;
                                    }
                                    else
                                    {
                                        lifo.RemoveAt(lifo.Count - 1);
                                        pp = (QtyCost)lifo[lifo.Count - 1];
                                        pp.Qty = Decimal.Add((Decimal)pp.Qty, movementQty);
                                        remainder = (Decimal)pp.Qty;
                                    }
                                }
                            }
                        }
                        else
                        {
                            QtyCost pp = new QtyCost(movementQty, Env.ZERO);
                            lifo.Add(pp);
                        }
                        _log.Finer("Movement=" + movementQty + ", Size=" + lifo.Count);
                        continue;
                    }
                    //	Assumption: everything is matched
                    Decimal price = Util.GetValueOfDecimal(dr[3].ToString());
                    int VAB_Currency_ID = Util.GetValueOfInt(dr[4].ToString());
                    DateTime? DateAcct = Util.GetValueOfDateTime(dr[5]);
                    int VAB_CurrencyType_ID = Util.GetValueOfInt(dr[6].ToString());
                    int Client_ID = Util.GetValueOfInt(dr[7].ToString());
                    int Org_ID = Util.GetValueOfInt(dr[8].ToString());
                    Decimal cost = VAdvantage.Model.MVABExchangeRate.Convert(product.GetCtx(), price,
                        VAB_Currency_ID, as1.GetVAB_Currency_ID(),
                        DateAcct, VAB_CurrencyType_ID, Client_ID, Org_ID);
                    //
                    QtyCost pp1 = new QtyCost(movementQty, cost);
                    lifo.Add(pp1);
                    _log.Finer("Movement=" + movementQty + ", Size=" + lifo.Count);
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            if (lifo.Count == 0)
            {
                return null;
            }
            QtyCost pp2 = (QtyCost)lifo[lifo.Count - 1];
            _log.Finer(product.GetName() + " = " + pp2.Cost);
            return pp2.Cost;
        }


        /**
         *	MVAMVAMProductCost Qty-Cost Pair
         */
        /****************************************************************************************
        In java instance of static class is creted not in .net also .net no parametrised constructor used
        /****************************************************************************************/
        //static class QtyCost
        public class QtyCost
        {
            //Qty		
            public Decimal? Qty = null;
            // Cost	
            public Decimal? Cost = null;

            /**
             * 	Constructor
             *	@param qty qty
             *	@param cost cost
             */
            public QtyCost(Decimal qty, Decimal cost)
            {
                Qty = qty;
                Cost = cost;
            }


            /**
             * 	String Representation
             *	@return info
             */
            public override String ToString()
            {
                StringBuilder sb = new StringBuilder("Qty=").Append(Qty)
                    .Append(",Cost=").Append(Cost);
                return sb.ToString();
            }
        }


        /**
         * 	Get/Create Cost Record.
         * 	CostingLevel is not validated
         *	@param product product
         *	@param VAM_PFeature_SetInstance_ID costing level asi
         *	@param as1 accounting schema
         *	@param VAF_Org_ID costing level org
         *	@param VAM_ProductCostElement_ID element
         *	@return cost price or null
         */

        /* Addes By Bharat 08/July/2014 */
        public static MVAMVAMProductCost Get(MVAMProduct product, int VAM_PFeature_SetInstance_ID,
            VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID, int VAM_ProductCostElement_ID, int VAA_Asset_ID, int VAM_Warehouse_ID = 0)
        {
            MVAMVAMProductCost cost = null;
            String sql = "SELECT * "
                + "FROM VAM_ProductCost c "
                + "WHERE VAF_Client_ID=" + product.GetVAF_Client_ID() + " AND VAF_Org_ID=" + VAF_Org_ID
                + " AND VAM_Product_ID=" + product.GetVAM_Product_ID()
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID
                + " AND VAM_ProductCostType_ID=" + as1.GetVAM_ProductCostType_ID() + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID()
                + " AND VAM_ProductCostElement_ID=" + VAM_ProductCostElement_ID + "AND VAA_Asset_ID=" + VAA_Asset_ID + "AND ISAssetCost= 'Y'";
            sql += " AND NVL(VAM_Warehouse_ID,0) = " + VAM_Warehouse_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, product.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    cost = new MVAMVAMProductCost(product.GetCtx(), dr, product.Get_TrxName());
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            //	New
            if (cost == null)
            {
                cost = new MVAMVAMProductCost(product, VAM_PFeature_SetInstance_ID,
                    as1, VAF_Org_ID, VAM_ProductCostElement_ID, VAA_Asset_ID);
                cost.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
            }
            return cost;
        }
        /*  Addes By Bharat 08/July/2014   */

        /// <summary>
        /// is used to get or create object of product costs based on respective parameter
        /// CostingLevel is not validated manually
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="VAM_PFeature_SetInstance_ID">Costing Level - AttributeSetInstance</param>
        /// <param name="as1">Accounting schema</param>
        /// <param name="VAF_Org_ID">Costing level - Organization</param>
        /// <param name="VAM_ProductCostElement_ID">cost element</param>
        /// <param name="VAM_Warehouse_ID">costing level - warehouse</param>
        /// <returns>MVAMVAMProductCost Object</returns>
        public static MVAMVAMProductCost Get(MVAMProduct product, int VAM_PFeature_SetInstance_ID,
            VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID, int VAM_ProductCostElement_ID, int VAM_Warehouse_ID = 0)
        {
            MVAMVAMProductCost cost = null;
            String sql = "SELECT * "
                + "FROM VAM_ProductCost c "
                + "WHERE VAF_Client_ID=" + product.GetVAF_Client_ID() + " AND VAF_Org_ID=" + VAF_Org_ID
                + " AND VAM_Product_ID=" + product.GetVAM_Product_ID()
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID
                + " AND VAM_ProductCostType_ID=" + as1.GetVAM_ProductCostType_ID() + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID()
                + " AND VAM_ProductCostElement_ID=" + VAM_ProductCostElement_ID + " AND ISAssetCost= 'N'";
            sql += " AND NVL(VAM_Warehouse_ID, 0) = " + VAM_Warehouse_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, product.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    cost = new MVAMVAMProductCost(product.GetCtx(), dr, product.Get_TrxName());
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            //	New
            if (cost == null)
            {
                cost = new MVAMVAMProductCost(product, VAM_PFeature_SetInstance_ID,
                    as1, VAF_Org_ID, VAM_ProductCostElement_ID);
                cost.SetVAM_Warehouse_ID(VAM_Warehouse_ID);
            }
            return cost;
        }

        public static MVAMVAMProductCost[] Get(int VAM_PFeature_SetInstance_ID,
        VAdvantage.Model.MVABAccountBook as1, int VAM_ProductCostType_ID, int VAF_Org_ID, MVAMProduct product)
        {
            String CostingMethod = as1.GetCostingMethod();
            String CostingLevel = as1.GetCostingLevel();

            dynamic pca = null;
            if (as1.GetFRPT_LocAcct_ID() > 0)
            {
                pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                if (pca == null)
                {
                    throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                }
                //	Costing Level
                //if (pca.GetFRPT_CostingLevel() != null)
                //    CostingLevel = pca.GetFRPT_CostingLevel();
                if (pca.GetCostingLevel() != null)
                    CostingLevel = pca.GetCostingLevel();

                if (pca.GetCostingMethod() != null)
                    CostingMethod = pca.GetCostingMethod();
            }
            else
            {
                // Amit - 21-9-2016  
                // bcz Costing level consider from product category not from accounting tab
                pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                //pca = MVAMProductCategoryAcct.Get(product.GetCtx(),
                //       product.GetVAM_ProductCategory_ID(), as1.GetVAB_AccountBook_ID(), null);
                //end
                if (pca == null)
                {
                    throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                }
                //	Costing Level
                if (pca.GetCostingLevel() != null)
                    CostingLevel = pca.GetCostingLevel();

                if (pca.GetCostingMethod() != null)
                    CostingMethod = pca.GetCostingMethod();
            }

            if (X_VAB_AccountBook.COSTINGLEVEL_Client.Equals(CostingLevel))
            {
                VAF_Org_ID = 0;
                VAM_PFeature_SetInstance_ID = 0;
            }
            else if (X_VAB_AccountBook.COSTINGLEVEL_Organization.Equals(CostingLevel))
                VAM_PFeature_SetInstance_ID = 0;
            else if (X_VAB_AccountBook.COSTINGLEVEL_BatchLot.Equals(CostingLevel))
                VAF_Org_ID = 0;
            //	Costing Method is standard only right now. Will have to change this once others are included.

            //	TODO Create/Update Costs Do we need this

            // handling cost combination
            int costCombinationElement_ID = 0;
            pca = null;
            if (CostingMethod == "C")
            {
                if (as1.GetFRPT_LocAcct_ID() > 0)
                {
                    pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                    if (pca == null)
                        throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                    //	Costing Element
                    if (Util.GetValueOfInt(pca.GetVAM_ProductCostElement_ID()) != 0)
                    {
                        costCombinationElement_ID = Util.GetValueOfInt(pca.GetVAM_ProductCostElement_ID());
                    }
                    else
                    {
                        costCombinationElement_ID = as1.GetVAM_ProductCostElement_ID();
                        if (costCombinationElement_ID == 0)
                            throw new ArgumentException("No Costing Element Selected");
                    }
                }
            }
            //end

            MVAMVAMProductCost cost = null;
            List<MVAMVAMProductCost> list = new List<MVAMVAMProductCost>();
            String sql = "SELECT c.* "
                + "FROM VAM_ProductCost c "
                + " LEFT OUTER JOIN VAM_ProductCostElement ce ON (c.VAM_ProductCostElement_ID=ce.VAM_ProductCostElement_ID) "
                + "WHERE c.VAF_Client_ID=" + product.GetVAF_Client_ID() + " AND c.VAF_Org_ID=" + VAF_Org_ID
                + " AND c.VAM_Product_ID=" + product.GetVAM_Product_ID()
                + " AND (c.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR c.VAM_PFeature_SetInstance_ID=0) ";
            if (VAM_ProductCostType_ID == 0)
            {
                //pstmt.setInt (5, as1.GetVAM_ProductCostType_ID());
                sql += " AND c.VAM_ProductCostType_ID=" + as1.GetVAM_ProductCostType_ID();
            }
            else
            {
                //pstmt.setInt (5, VAM_ProductCostType_ID);
                sql += " AND c.VAM_ProductCostType_ID= " + VAM_ProductCostType_ID;
            }

            //sql += " AND (ce.CostingMethod IS NULL OR ce.CostingMethod='" + X_VAM_ProductCostElement.COSTINGMETHOD_StandardCosting + "') "
            //+ " AND c.IsActive = 'Y' ";

            sql += " AND (ce.CostingMethod = '" + CostingMethod + "') "
            + " AND c.IsActive = 'Y' ";

            if (CostingMethod == "C" && costCombinationElement_ID != 0)
            {
                sql += " AND c.VAM_ProductCostElement_ID = " + costCombinationElement_ID;
            }

            if (as1 != null)
            {
                sql = sql + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();
            }

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, product.Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    cost = new MVAMVAMProductCost(product.GetCtx(), dr, null);
                    list.Add(cost);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            MVAMVAMProductCost[] costs = new MVAMVAMProductCost[list.Count];
            costs = list.ToArray();
            return costs;
        }

        public static MVAMVAMProductCost[] Get(int VAM_PFeature_SetInstance_ID,
       VAdvantage.Model.MVABAccountBook as1, int VAM_ProductCostType_ID, int VAF_Org_ID, int productID)
        {
            MVAMProduct product = new MVAMProduct(Env.GetCtx(), productID, null);
            String CostingLevel = as1.GetCostingLevel();

            dynamic pca = null;
            if (as1.GetFRPT_LocAcct_ID() > 0)
            {
                pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                if (pca == null)
                {
                    throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                }
                //	Costing Level
                //if (pca.GetFRPT_CostingLevel() != null)
                //    CostingLevel = pca.GetFRPT_CostingLevel();
                if (pca.GetCostingLevel() != null)
                    CostingLevel = pca.GetCostingLevel();
            }
            else
            {
                // Amit - 21-9-2016  
                // bcz Costing level consider from product category not from accounting tab
                pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                //pca = MVAMProductCategoryAcct.Get(product.GetCtx(),
                //        product.GetVAM_ProductCategory_ID(), as1.GetVAB_AccountBook_ID(), null);
                //end
                if (pca == null)
                {
                    throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                }
                //	Costing Level
                if (pca.GetCostingLevel() != null)
                    CostingLevel = pca.GetCostingLevel();

            }

            if (X_VAB_AccountBook.COSTINGLEVEL_Client.Equals(CostingLevel))
            {
                VAF_Org_ID = 0;
                VAM_PFeature_SetInstance_ID = 0;
            }
            else if (X_VAB_AccountBook.COSTINGLEVEL_Organization.Equals(CostingLevel))
                VAM_PFeature_SetInstance_ID = 0;
            else if (X_VAB_AccountBook.COSTINGLEVEL_BatchLot.Equals(CostingLevel))
                VAF_Org_ID = 0;
            //	Costing Method is standard only right now. Will have to change this once others are included.

            //	TODO Create/Update Costs Do we need this

            MVAMVAMProductCost cost = null;
            List<MVAMVAMProductCost> list = new List<MVAMVAMProductCost>();
            String sql = "SELECT c.* "
                + "FROM VAM_ProductCost c "
                + " LEFT OUTER JOIN VAM_ProductCostElement ce ON (c.VAM_ProductCostElement_ID=ce.VAM_ProductCostElement_ID) "
                + "WHERE c.VAF_Client_ID=" + product.GetVAF_Client_ID() + " AND c.VAF_Org_ID=" + VAF_Org_ID
                + " AND c.VAM_Product_ID=" + product.GetVAM_Product_ID()
                + " AND (c.VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR c.VAM_PFeature_SetInstance_ID=0) ";
            if (VAM_ProductCostType_ID == 0)
            {
                //pstmt.setInt (5, as1.GetVAM_ProductCostType_ID());
                sql += " AND c.VAM_ProductCostType_ID=" + as1.GetVAM_ProductCostType_ID();
            }
            else
            {
                //pstmt.setInt (5, VAM_ProductCostType_ID);
                sql += " AND c.VAM_ProductCostType_ID= " + VAM_ProductCostType_ID;
            }

            sql += " AND (ce.CostingMethod IS NULL OR ce.CostingMethod='" + X_VAM_ProductCostElement.COSTINGMETHOD_StandardCosting + "') "
            + " AND c.IsActive = 'Y' ";
            if (as1 != null)
            {
                sql = sql + " AND VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID();
            }

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, product.Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    cost = new MVAMVAMProductCost(product.GetCtx(), dr, null);
                    list.Add(cost);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            MVAMVAMProductCost[] costs = new MVAMVAMProductCost[list.Count];
            costs = list.ToArray();
            return costs;
        }

        public static MVAMVAMProductCost GetDefaultCost(Ctx ctx, int VAM_Product_ID)
        {
            //String sql = "SELECT VAM_ProductCategory_ID FROM VAM_Product WHERE IsActive='Y' AND VAM_Product_ID=" + VAM_Product_ID;
            //int pCategory = Util.GetValueOfInt(DB.ExecuteScalar(sql));

            string sql = "SELECT VAB_AccountBook1_id FROM VAF_ClientDetail WHERE isactive='Y' AND VAF_Client_ID= " + ctx.GetVAF_Client_ID();
            int accSchemaID = Util.GetValueOfInt(DB.ExecuteScalar(sql));



            MVAMProduct MVAMProduct = new MVAMProduct(ctx, VAM_Product_ID, null);
            MVAMProductCategory pc = MVAMProductCategory.Get(MVAMProduct.GetCtx(), MVAMProduct.GetVAM_ProductCategory_ID());

            String cl = null;
            int VAF_Org_ID = 0;
            int M_ASI_ID = 0;

            if (pc != null)
            {
                cl = pc.GetCostingMethod();
            }

            MVABAccountBook acctSchema = new MVABAccountBook(ctx, accSchemaID, null);

            if (cl == null)
                cl = acctSchema.GetCostingMethod();

            //if (cl == "C" || cl == "B")
            //{
            VAF_Org_ID = 0;
            //}
            //else// Suggested By Amit
            //{
            //    VAF_Org_ID = cd.GetVAF_Org_ID();
            //}
            //if (cl != "B")
            //{
            M_ASI_ID = 0;
            //}
            string costMethod = pc.GetCostingMethod();
            if (string.IsNullOrEmpty(costMethod))
            {
                costMethod = acctSchema.GetCostingMethod();
            }

            sql = "SELECT VAM_ProductCostElement_ID FROM VAM_ProductCostElement WHERE IsActive='Y' AND costingmethod='" + costMethod + "'";
            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAM_ProductCostElement", true, true);
            int costElementID = Convert.ToInt32(DB.ExecuteScalar(sql));

            MVAMVAMProductCost cost = MVAMVAMProductCost.Get(MVAMProduct, M_ASI_ID, acctSchema, VAF_Org_ID, costElementID);

            return cost;
        }
        /*  Addes By Bharat 08/July/2014   */


        /**
         * 	Get Costs
         * 	@param ctx context
         *	@param VAF_Client_ID client
         *	@param VAF_Org_ID org
         *	@param VAM_Product_ID product
         *	@param VAM_ProductCostType_ID cost type
         *	@param VAB_AccountBook_ID as1
         *	@param VAM_ProductCostElement_ID cost element
         *	@param VAM_PFeature_SetInstance_ID asi
         *	@return cost or null
         */
        public static MVAMVAMProductCost Get(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, int VAM_Product_ID,
            int VAM_ProductCostType_ID, int VAB_AccountBook_ID, int VAM_ProductCostElement_ID,
            int VAM_PFeature_SetInstance_ID)
        {
            MVAMVAMProductCost retValue = null;
            String sql = "SELECT * FROM VAM_ProductCost "
                + "WHERE VAF_Client_ID=" + VAF_Client_ID + " AND VAF_Org_ID=" + VAF_Org_ID + " AND VAM_Product_ID=" + VAM_Product_ID
                + " AND VAM_ProductCostType_ID=" + VAM_ProductCostType_ID + " AND VAB_AccountBook_ID=" + VAB_AccountBook_ID + " AND VAM_ProductCostElement_ID=" + VAM_ProductCostElement_ID
                + " AND VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVAMVAMProductCost(ctx, dr, null);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            return retValue;
        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param ignored multi-key
         *	@param trxName trx
         */
        public MVAMVAMProductCost(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, ignored, trxName)
        {

            if (ignored == 0)
            {
                //	setVAB_AccountBook_ID (0);
                //	setVAM_ProductCostElement_ID (0);
                //	setVAM_ProductCostType_ID (0);
                //	setVAM_Product_ID (0);
                SetVAM_PFeature_SetInstance_ID(0);
                SetVAM_Warehouse_ID(0);// set default value on Warehouse as ZERO
                //
                SetCurrentCostPrice(Env.ZERO);
                SetFutureCostPrice(Env.ZERO);
                SetCurrentQty(Env.ZERO);
                SetCumulatedAmt(Env.ZERO);
                SetCumulatedQty(Env.ZERO);
            }
            else
                throw new ArgumentException("Multi-Key");
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MVAMVAMProductCost(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            _manual = false;
        }


        /**                      Added By Bharat 10-July-2014 
         * 	Parent Constructor
         *	@param product Product
         *	@param VAM_PFeature_SetInstance_ID asi
         *	@param as1 Acct Schema
         *	@param VAF_Org_ID org
         *	@param VAM_ProductCostElement_ID cost element
         */
        public MVAMVAMProductCost(MVAMProduct product, int VAM_PFeature_SetInstance_ID,
            VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID, int VAM_ProductCostElement_ID, int VAA_Asset_ID)
            : this(product.GetCtx(), 0, product.Get_TrxName())
        {
            SetClientOrg(product.GetVAF_Client_ID(), VAF_Org_ID);
            SetVAB_AccountBook_ID(as1.GetVAB_AccountBook_ID());
            SetVAM_ProductCostType_ID(as1.GetVAM_ProductCostType_ID());
            SetVAM_Product_ID(product.GetVAM_Product_ID());
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
            if (VAA_Asset_ID > 0)
                SetA_Asset_ID(VAA_Asset_ID);
            //
            _manual = false;
        }




        /**
         * 	Parent Constructor
         *	@param product Product
         *	@param VAM_PFeature_SetInstance_ID asi
         *	@param as1 Acct Schema
         *	@param VAF_Org_ID org
         *	@param VAM_ProductCostElement_ID cost element
         */
        public MVAMVAMProductCost(MVAMProduct product, int VAM_PFeature_SetInstance_ID,
            VAdvantage.Model.MVABAccountBook as1, int VAF_Org_ID, int VAM_ProductCostElement_ID)
            : this(product.GetCtx(), 0, product.Get_TrxName())
        {
            SetClientOrg(product.GetVAF_Client_ID(), VAF_Org_ID);
            SetVAB_AccountBook_ID(as1.GetVAB_AccountBook_ID());
            SetVAM_ProductCostType_ID(as1.GetVAM_ProductCostType_ID());
            SetVAM_Product_ID(product.GetVAM_Product_ID());
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetVAM_ProductCostElement_ID(VAM_ProductCostElement_ID);
            //
            _manual = false;
        }

        /**
         * 	Add Cumulative Amt/Qty and Current Qty
         *	@param amt amt
         *	@param qty qty
         */
        public void Add(Decimal amt, Decimal qty)
        {
            SetCumulatedAmt(Decimal.Add(GetCumulatedAmt(), amt));
            SetCumulatedQty(Decimal.Add(GetCumulatedQty(), qty));
            if (Decimal.Add(GetCurrentQty(), qty) < 0)
            {
                SetCurrentQty(0);
            }
            else
            {
                SetCurrentQty(Decimal.Add(GetCurrentQty(), qty));
            }
        }	//	Add

        /**
         * 	Add Amt/Qty and calculate weighted average.
         * 	((OldAvg*OldQty)+(Price*Qty)) / (OldQty+Qty)
         *	@param amt total amt (price * qty)
         *	@param qty qty
         */
        public void SetWeightedAverage(Decimal amt, Decimal qty)
        {
            Decimal oldSum = Decimal.Multiply(GetCurrentCostPrice(), GetCurrentQty());
            Decimal newSum = amt;	//	is total already
            Decimal sumAmt = Decimal.Add(oldSum, newSum);
            Decimal sumQty = Decimal.Add(GetCurrentQty(), qty);
            if (Env.Signum(sumQty) != 0)
            {
                Decimal cost = Decimal.Round(Decimal.Divide(sumAmt, sumQty), GetPrecision(), MidpointRounding.AwayFromZero);
                SetCurrentCostPrice(cost);
            }
            //
            SetCumulatedAmt(Decimal.Add(GetCumulatedAmt(), amt));
            SetCumulatedQty(Decimal.Add(GetCumulatedQty(), qty));
            SetCurrentQty(Decimal.Add(GetCurrentQty(), qty));
        }

        /**
         * 	Get Costing Precision
         *	@return precision (6)
         */
        private int GetPrecision()
        {
            VAdvantage.Model.MVABAccountBook as1 = VAdvantage.Model.MVABAccountBook.Get(GetCtx(), GetVAB_AccountBook_ID());
            if (as1 != null)
                return as1.GetCostingPrecision();
            return 6;
        }

        /**
         * 	Set Current Cost Price
         *	@param currentCostPrice if null set to 0
         */
        public new void SetCurrentCostPrice(Decimal? currentCostPrice)
        {
            if (currentCostPrice != null)
            {
                //   base.SetCurrentCostPrice((Decimal)Convert.ToDecimal(currentCostPrice));
                base.SetCurrentCostPrice(Decimal.Round(currentCostPrice.Value, GetPrecision(), MidpointRounding.AwayFromZero));
            }
            else
            {
                base.SetCurrentCostPrice(Env.ZERO);
            }
        }

        /**
         * 	Get History Average (Amt/Qty)
         *	@return average if amt/aty <> 0 otherwise null
         */
        public Decimal? GetHistoryAverage()
        {
            Decimal? retValue = null;
            if (Env.Signum(GetCumulatedQty()) != 0
                && Env.Signum(GetCumulatedAmt()) != 0)
                retValue = Decimal.Divide(GetCumulatedAmt(), Decimal.Round(GetCumulatedQty(), GetPrecision(), MidpointRounding.AwayFromZero));
            return retValue;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMVAMProductCost[");
            sb.Append("VAF_Client_ID=").Append(GetVAF_Client_ID());
            if (GetVAF_Org_ID() != 0)
                sb.Append(",VAF_Org_ID=").Append(GetVAF_Org_ID());
            sb.Append(",VAM_Product_ID=").Append(GetVAM_Product_ID());
            if (GetVAM_PFeature_SetInstance_ID() != 0)
                sb.Append(",AD_ASI_ID=").Append(GetVAM_PFeature_SetInstance_ID());
            //	sb.Append (",VAB_AccountBook_ID=").Append (getVAB_AccountBook_ID());
            //	sb.Append (",VAM_ProductCostType_ID=").Append (getVAM_ProductCostType_ID());
            sb.Append(",VAM_ProductCostElement_ID=").Append(GetVAM_ProductCostElement_ID());
            //
            sb.Append(", CurrentCost=").Append(GetCurrentCostPrice())
                .Append(", C.Amt=").Append(GetCumulatedAmt())
                .Append(",C.Qty=").Append(GetCumulatedQty())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Cost Element
         *	@return cost element
         */
        public VAdvantage.Model.MVAMVAMProductCostElement GetCostElement()
        {
            int VAM_ProductCostElement_ID = GetVAM_ProductCostElement_ID();
            if (VAM_ProductCostElement_ID == 0)
                return null;
            return VAdvantage.Model.MVAMVAMProductCostElement.Get(GetCtx(), VAM_ProductCostElement_ID);
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true if can be saved
         */
        protected override bool BeforeSave(bool newRecord)
        {
            VAdvantage.Model.MVAMVAMProductCostElement ce = GetCostElement();
            //	Check if data entry makes sense
            if (_manual)
            {
                VAdvantage.Model.MVABAccountBook as1 = new VAdvantage.Model.MVABAccountBook(GetCtx(), GetVAB_AccountBook_ID(), null);
                String CostingLevel = as1.GetCostingLevel();
                MVAMProduct product = MVAMProduct.Get(GetCtx(), GetVAM_Product_ID());

                dynamic pca = null;
                if (as1.GetFRPT_LocAcct_ID() > 0)
                {
                    pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                    if (pca == null)
                        throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                    //	Costing Level
                    //if (pca.GetFRPT_CostingLevel() != null)
                    //    CostingLevel = pca.GetFRPT_CostingLevel();
                    if (pca.GetCostingLevel() != null)
                        CostingLevel = pca.GetCostingLevel();
                }
                else
                {
                    // change by Amit 21-Sep-2016
                    // Now we consider Costing level From Product Category
                    //pca = MVAMProductCategoryAcct.Get(GetCtx(),
                    //   product.GetVAM_ProductCategory_ID(), as1.GetVAB_AccountBook_ID(), null);
                    pca = MVAMProductCategory.Get(product.GetCtx(), product.GetVAM_ProductCategory_ID());
                    //end

                    if (pca == null)
                        throw new Exception("Cannot find Acct for VAM_ProductCategory_ID="
                            + product.GetVAM_ProductCategory_ID()
                            + ", VAB_AccountBook_ID=" + as1.GetVAB_AccountBook_ID());
                    //	Costing Level
                    if (pca.GetCostingLevel() != null)
                        CostingLevel = pca.GetCostingLevel();
                }

                if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_Client.Equals(CostingLevel))
                {
                    if (GetVAF_Org_ID() != 0 || GetVAM_PFeature_SetInstance_ID() != 0)
                    {
                        log.SaveError("CostingLevelClient", "");
                        return false;
                    }
                }
                else if (VAdvantage.Model.MVABAccountBook.COSTINGLEVEL_BatchLot.Equals(CostingLevel))
                {
                    if (GetVAM_PFeature_SetInstance_ID() == 0
                        && ce.IsCostingMethod())
                    {
                        log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "VAM_PFeature_SetInstance_ID"));
                        return false;
                    }
                    if (GetVAF_Org_ID() != 0)
                        SetVAF_Org_ID(0);
                }
            }

            //	Cannot enter calculated
            if (_manual && ce != null && ce.IsCalculated())
            {
                log.SaveError("Error", Msg.GetElement(GetCtx(), "IsCalculated"));
                return false;
            }
            //	Percentage
            if (ce != null)
            {
                if (ce.IsCalculated()
                    || VAdvantage.Model.MVAMVAMProductCostElement.COSTELEMENTTYPE_Material.Equals(ce.GetCostElementType())
                    && Env.Signum(GetPercentCost()) != 0)
                    SetPercentCost(Env.ZERO);
            }
            if (Env.Signum(GetPercentCost()) != 0)
            {
                if (Env.Signum(GetCurrentCostPrice()) != 0)
                    SetCurrentCostPrice(Env.ZERO);
                if (Env.Signum(GetFutureCostPrice()) != 0)
                    SetFutureCostPrice(Env.ZERO);
                if (Env.Signum(GetCumulatedAmt()) != 0)
                    SetCumulatedAmt(Env.ZERO);
                if (Env.Signum(GetCumulatedQty()) != 0)
                    SetCumulatedQty(Env.ZERO);
            }
            return true;
        }


        /**
         * 	Before Delete
         *	@return true
         */
        protected override bool BeforeDelete()
        {
            return true;
        }

        /**
         * 	Test
         *	@param args ignored
         */
        //public static void main (String[] args)
        //{
        //    /**
        //    DELETE VAM_ProductCost c
        //    WHERE EXISTS (SELECT * FROM VAM_ProductCostElement ce 
        //        WHERE c.VAM_ProductCostElement_ID=ce.VAM_ProductCostElement_ID AND ce.IsCalculated='Y')
        //    /
        //    UPDATE VAM_ProductCost
        //      SET CumulatedAmt=0, CumulatedQty=0
        //    /  
        //    UPDATE VAM_ProductCostDetail
        //      SET Processed='N'
        //    WHERE Processed='Y'
        //    /
        //    COMMIT
        //    /
        //    **/

        //    Vienna.startup(true);
        //    VAdvantage.Model.MClient client = VAdvantage.Model.MClient.Get(Env.GetCtx(), 11);	//	GardenWorld
        //    create(client);

        //}	//	main


    }
}
