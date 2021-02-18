/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Tax
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     04-Jun-2009
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
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class Tax
    {
        //	Logger						
        private static VLogger log = VLogger.GetVLogger(typeof(Tax).FullName);
        /// <summary>
        /// Get Tax ID - converts parameters to call Get Tax.
        ///   <pre>
        ///   VAM_Product_ID/VAB_Charge_ID	->	VAB_TaxCategory_ID
        ///   billDate, shipDate			->	billDate, shipDate
        ///   VAF_Org_ID					->	billFromVAB_Address_ID
        ///   VAM_Warehouse_ID				->	shipFromVAB_Address_ID
        ///   billVAB_BPart_Location_ID  ->	billToVAB_Address_ID
        ///   shipVAB_BPart_Location_ID 	->	shipToVAB_Address_ID
        ///   if IsSOTrx is false, bill and ship are reversed
        ///   </pre>
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAB_Charge_ID">product</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="shipDate">ship date</param>
        /// <param name="VAF_Org_ID"></param>
        /// <param name="VAM_Warehouse_ID">warehouse</param>
        /// <param name="billVAB_BPart_Location_ID">invoice location</param>
        /// <param name="shipVAB_BPart_Location_ID">ship location</param>
        /// <param name="IsSOTrx">is a sales trx</param>
        /// <returns>VAB_TaxRate_ID If error it returns 0 and sets error log (TaxCriteriaNotFound)</returns>
        public static int Get(Ctx ctx, int VAM_Product_ID, int VAB_Charge_ID, DateTime? billDate, DateTime? shipDate,
            int VAF_Org_ID, int VAM_Warehouse_ID, int billVAB_BPart_Location_ID, int shipVAB_BPart_Location_ID, bool IsSOTrx)
        {
            if (VAM_Product_ID != 0)
            {
                return GetProduct(ctx, VAM_Product_ID, billDate, shipDate, VAF_Org_ID, VAM_Warehouse_ID,
                    billVAB_BPart_Location_ID, shipVAB_BPart_Location_ID, IsSOTrx);
            }
            else if (VAB_Charge_ID != 0)
            {
                return GetCharge(ctx, VAB_Charge_ID, billDate, shipDate, VAF_Org_ID, VAM_Warehouse_ID,
                    billVAB_BPart_Location_ID, shipVAB_BPart_Location_ID, IsSOTrx);
            }
            else
            {
                return GetExemptTax(ctx, VAF_Org_ID);
            }
        }

        /// <summary>
        /// Get Tax ID - converts parameters to call Get Tax.
        ///   <pre>
        ///   VAM_Product_ID/VAB_Charge_ID	->	VAB_TaxCategory_ID
        ///   billDate, shipDate			->	billDate, shipDate
        ///   VAF_Org_ID					->	billFromVAB_Address_ID
        ///   VAM_Warehouse_ID				->	shipFromVAB_Address_ID
        ///   billVAB_BPart_Location_ID  ->	billToVAB_Address_ID
        ///   shipVAB_BPart_Location_ID 	->	shipToVAB_Address_ID
        ///   if IsSOTrx is false, bill and ship are reversed
        ///   </pre>
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAB_Charge_ID">product</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="shipDate">ship date</param>
        /// <param name="VAF_Org_ID"></param>
        /// <param name="VAM_Warehouse_ID">warehouse</param>
        /// <param name="billVAB_BPart_Location_ID">invoice location</param>
        /// <param name="shipVAB_BPart_Location_ID">ship location</param>
        /// <param name="IsSOTrx">is a sales trx</param>
        /// <returns>VAB_TaxRate_ID If error it returns 0 and sets error log (TaxCriteriaNotFound)</returns>
        public static int GetCharge(Ctx ctx, int VAB_Charge_ID, DateTime? billDate, DateTime? shipDate, int VAF_Org_ID, int VAM_Warehouse_ID,
            int billVAB_BPart_Location_ID, int shipVAB_BPart_Location_ID, bool IsSOTrx)
        {
            if (VAM_Warehouse_ID == 0)
                VAM_Warehouse_ID = ctx.GetContextAsInt("VAM_Warehouse_ID");
            if (VAM_Warehouse_ID == 0)
            {
                log.Warning("No Warehouse - VAB_Charge_ID=" + VAB_Charge_ID);
                return 0;
            }
           // String variable = "";
            int VAB_TaxCategory_ID = 0;
            int shipFromVAB_Address_ID = 0;
            int shipToVAB_Address_ID = 0;
            int billFromVAB_Address_ID = 0;
            int billToVAB_Address_ID = 0;
            String IsTaxExempt = null;

            //	Get all at once
            String sql = "SELECT c.VAB_TaxCategory_ID, o.VAB_Address_ID, il.VAB_Address_ID, b.IsTaxExempt,"
                 + " w.VAB_Address_ID, sl.VAB_Address_ID "
                 + "FROM VAB_Charge c, VAF_OrgDetail o,"
                 + " VAB_BPart_Location il INNER JOIN VAB_BusinessPartner b ON (il.VAB_BusinessPartner_ID=b.VAB_BusinessPartner_ID),"
                 + " VAM_Warehouse w, VAB_BPart_Location sl "
                 + "WHERE c.VAB_Charge_ID=" + VAB_Charge_ID
                 + " AND o.VAF_Org_ID=" + VAF_Org_ID
                 + " AND il.VAB_BPart_Location_ID=" + billVAB_BPart_Location_ID
                 + " AND w.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                 + " AND sl.VAB_BPart_Location_ID=" + shipVAB_BPart_Location_ID;
            try
            {
                DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
                bool found = false;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    VAB_TaxCategory_ID = Utility.Util.GetValueOfInt(dr[0]);
                    billFromVAB_Address_ID = Utility.Util.GetValueOfInt(dr[1]);
                    billToVAB_Address_ID = Utility.Util.GetValueOfInt(dr[2]);
                    IsTaxExempt = dr[3].ToString();
                    shipFromVAB_Address_ID = Utility.Util.GetValueOfInt(dr[4]);
                    shipToVAB_Address_ID = Utility.Util.GetValueOfInt(dr[5]);
                    found = true;
                }
                //
                if (!found)
                {
                    log.Warning("Not found for VAB_Charge_ID=" + VAB_Charge_ID 
                        + ", VAF_Org_ID=" + VAF_Org_ID + ", VAM_Warehouse_ID=" + VAM_Warehouse_ID
                        + ", VAB_BPart_Location_ID=" + billVAB_BPart_Location_ID 
                       + "/" + shipVAB_BPart_Location_ID);
                    return 0;
                }
                else if ("Y".Equals(IsTaxExempt))
                    return GetExemptTax(ctx, VAF_Org_ID);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
                return 0;
            }

            //	Reverese for PO
            if (!IsSOTrx)
            {
                int temp = billFromVAB_Address_ID;
                billFromVAB_Address_ID = billToVAB_Address_ID;
                billToVAB_Address_ID = temp;
                temp = shipFromVAB_Address_ID;
                shipFromVAB_Address_ID = shipToVAB_Address_ID;
                shipToVAB_Address_ID = temp;
            }
            //
            log.Fine("VAB_TaxCategory_ID=" + VAB_TaxCategory_ID
              + ", billFromVAB_Address_ID=" + billFromVAB_Address_ID
              + ", billToVAB_Address_ID=" + billToVAB_Address_ID
              + ", shipFromVAB_Address_ID=" + shipFromVAB_Address_ID
              + ", shipToVAB_Address_ID=" + shipToVAB_Address_ID);
            return Get(ctx, VAB_TaxCategory_ID, IsSOTrx,
              shipDate, shipFromVAB_Address_ID, shipToVAB_Address_ID,
              billDate, billFromVAB_Address_ID, billToVAB_Address_ID);
        }

        /// <summary>
        /// Get Tax ID - converts parameters to call Get Tax.
        ///   <pre>
        ///   VAM_Product_ID/VAB_Charge_ID	->	VAB_TaxCategory_ID
        ///   billDate, shipDate			->	billDate, shipDate
        ///   VAF_Org_ID					->	billFromVAB_Address_ID
        ///   VAM_Warehouse_ID				->	shipFromVAB_Address_ID
        ///   billVAB_BPart_Location_ID  ->	billToVAB_Address_ID
        ///   shipVAB_BPart_Location_ID 	->	shipToVAB_Address_ID
        ///   if IsSOTrx is false, bill and ship are reversed
        ///   </pre>
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAB_Charge_ID">product</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="shipDate">ship date</param>
        /// <param name="VAF_Org_ID"></param>
        /// <param name="VAM_Warehouse_ID">warehouse</param>
        /// <param name="billVAB_BPart_Location_ID">invoice location</param>
        /// <param name="shipVAB_BPart_Location_ID">ship location</param>
        /// <param name="IsSOTrx">is a sales trx</param>
        /// <returns>VAB_TaxRate_ID If error it returns 0 and sets error log (TaxCriteriaNotFound)</returns>
        public static int GetProduct(Ctx ctx, int VAM_Product_ID, DateTime? billDate, DateTime? shipDate,
            int VAF_Org_ID, int VAM_Warehouse_ID, int billVAB_BPart_Location_ID, int shipVAB_BPart_Location_ID,
            bool IsSOTrx)
        {
            String variable = "";
            int VAB_TaxCategory_ID = 0;
            int shipFromVAB_Address_ID = 0;
            int shipToVAB_Address_ID = 0;
            int billFromVAB_Address_ID = 0;
            int billToVAB_Address_ID = 0;
            String IsTaxExempt = null;

            try
            {
                //	Get all at once
                String sql = "SELECT p.VAB_TaxCategory_ID, o.VAB_Address_ID, il.VAB_Address_ID, b.IsTaxExempt,"
                    + " w.VAB_Address_ID, sl.VAB_Address_ID "
                    + "FROM VAM_Product p, VAF_OrgDetail o,"
                    + " VAB_BPart_Location il INNER JOIN VAB_BusinessPartner b ON (il.VAB_BusinessPartner_ID=b.VAB_BusinessPartner_ID),"
                    + " VAM_Warehouse w, VAB_BPart_Location sl "
                    + "WHERE p.VAM_Product_ID=" + VAM_Product_ID
                    + " AND o.VAF_Org_ID=" + VAF_Org_ID
                    + " AND il.VAB_BPart_Location_ID=" + billVAB_BPart_Location_ID
                    + " AND w.VAM_Warehouse_ID=" + VAM_Warehouse_ID
                    + " AND sl.VAB_BPart_Location_ID=" + shipVAB_BPart_Location_ID;
                DataSet pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                bool found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    VAB_TaxCategory_ID = Utility.Util.GetValueOfInt(dr[0]);
                    billFromVAB_Address_ID = Utility.Util.GetValueOfInt(dr[1]);
                    billToVAB_Address_ID = Utility.Util.GetValueOfInt(dr[2]);
                    IsTaxExempt = dr[3].ToString();
                    shipFromVAB_Address_ID = Utility.Util.GetValueOfInt(dr[4]);
                    shipToVAB_Address_ID = Utility.Util.GetValueOfInt(dr[5]);
                    found = true;

                }

                if (found && "Y".Equals(IsTaxExempt))
                {
                    log.Fine("Business Partner is Tax exempt");
                    return GetExemptTax(ctx, VAF_Org_ID);
                }
                else if (found)
                {
                    if (!IsSOTrx)
                    {
                        int temp = billFromVAB_Address_ID;
                        billFromVAB_Address_ID = billToVAB_Address_ID;
                        billToVAB_Address_ID = temp;
                        temp = shipFromVAB_Address_ID;
                        shipFromVAB_Address_ID = shipToVAB_Address_ID;
                        shipToVAB_Address_ID = temp;
                    }
                    log.Fine("VAB_TaxCategory_ID=" + VAB_TaxCategory_ID
                        + ", billFromVAB_Address_ID=" + billFromVAB_Address_ID
                        + ", billToVAB_Address_ID=" + billToVAB_Address_ID
                        + ", shipFromVAB_Address_ID=" + shipFromVAB_Address_ID
                        + ", shipToVAB_Address_ID=" + shipToVAB_Address_ID);
                    return Get(ctx, VAB_TaxCategory_ID, IsSOTrx,
                        shipDate, shipFromVAB_Address_ID, shipToVAB_Address_ID,
                        billDate, billFromVAB_Address_ID, billToVAB_Address_ID);
                }

                // ----------------------------------------------------------------

                //	Detail for error isolation

                //	VAM_Product_ID				->	VAB_TaxCategory_ID
                sql = "SELECT VAB_TaxCategory_ID FROM VAM_Product "
                    + "WHERE VAM_Product_ID=" + VAM_Product_ID;
                variable = "VAM_Product_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    VAB_TaxCategory_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (VAB_TaxCategory_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(ctx, variable)
                       + (found ? "" : " (Product=" + VAM_Product_ID + " not found)"));
                    return 0;
                }
                log.Fine("VAB_TaxCategory_ID=" + VAB_TaxCategory_ID);
                //	VAF_Org_ID					->	billFromVAB_Address_ID
                sql = "SELECT VAB_Address_ID FROM VAF_OrgDetail "
                    + "WHERE VAF_Org_ID=" + VAF_Org_ID;
                variable = "VAF_Org_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    billFromVAB_Address_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (billFromVAB_Address_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetVAF_Language(ctx), variable)
                      + (found ? "" : " (Info/Org=" + VAF_Org_ID + " not found)"));
                    return 0;
                }
                //	billVAB_BPart_Location_ID  ->	billToVAB_Address_ID
                sql = "SELECT l.VAB_Address_ID, b.IsTaxExempt "
                    + "FROM VAB_BPart_Location l INNER JOIN VAB_BusinessPartner b ON (l.VAB_BusinessPartner_ID=b.VAB_BusinessPartner_ID) "
                    + "WHERE VAB_BPart_Location_ID=" + billVAB_BPart_Location_ID;
                variable = "BillTo_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    billToVAB_Address_ID = Utility.Util.GetValueOfInt(dr[0]);
                    IsTaxExempt = Convert.ToString(dr[1]);
                    found = true;
                }
                if (billToVAB_Address_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetVAF_Language(ctx), variable)
                        + (found ? "" : " (BPLocation=" + billVAB_BPart_Location_ID + " not found)"));
                    return 0;
                }
                if ("Y".Equals(IsTaxExempt))
                    return GetExemptTax(ctx, VAF_Org_ID);

                //  Reverse for PO
                if (!IsSOTrx)
                {
                    int temp = billFromVAB_Address_ID;
                    billFromVAB_Address_ID = billToVAB_Address_ID;
                    billToVAB_Address_ID = temp;
                }
                log.Fine("billFromVAB_Address_ID = " + billFromVAB_Address_ID);
                log.Fine("billToVAB_Address_ID = " + billToVAB_Address_ID);
                
                //-----------------------------------------------------------------

                //	VAM_Warehouse_ID				->	shipFromVAB_Address_ID
                sql = "SELECT VAB_Address_ID FROM VAM_Warehouse "
                    + "WHERE VAM_Warehouse_ID=" + VAM_Warehouse_ID;
                variable = "VAM_Warehouse_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    shipFromVAB_Address_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (shipFromVAB_Address_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetVAF_Language(ctx), variable)
                        + (found ? "" : " (Warehouse=" + VAM_Warehouse_ID + " not found)"));
                    return 0;
                }
                //	shipVAB_BPart_Location_ID 	->	shipToVAB_Address_ID
                sql = "SELECT VAB_Address_ID FROM VAB_BPart_Location "
                    + "WHERE VAB_BPart_Location_ID=" + shipVAB_BPart_Location_ID;
                variable = "VAB_BPart_Location_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;

                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    shipToVAB_Address_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (shipToVAB_Address_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetVAF_Language(ctx), variable)
                        + (found ? "" : " (BPLocation=" + shipVAB_BPart_Location_ID + " not found)"));
                    return 0;
                }

                //  Reverse for PO
                if (!IsSOTrx)
                {
                    int temp = shipFromVAB_Address_ID;
                    shipFromVAB_Address_ID = shipToVAB_Address_ID;
                    shipToVAB_Address_ID = temp;
                }
                log.Fine("shipFromVAB_Address_ID = " + shipFromVAB_Address_ID);
                log.Fine("shipToVAB_Address_ID = " + shipToVAB_Address_ID);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "getProduct (" + variable + ")", e);
            }

            return Get(ctx, VAB_TaxCategory_ID, IsSOTrx,
                shipDate, shipFromVAB_Address_ID, shipToVAB_Address_ID,
                billDate, billFromVAB_Address_ID, billToVAB_Address_ID);
        }

        /// <summary>
        /// Get Exempt Tax Code
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Org_ID">org to find client</param>
        /// <returns>VAB_TaxRate_ID</returns>
        private static int GetExemptTax(Ctx ctx, int VAF_Org_ID)
        {
            int VAB_TaxRate_ID = 0;
            String sql = "SELECT t.VAB_TaxRate_ID "
                + "FROM VAB_TaxRate t"
                + " INNER JOIN VAF_Org o ON (t.VAF_Client_ID=o.VAF_Client_ID) "
                + "WHERE t.IsTaxExempt='Y' AND o.VAF_Org_ID= " + VAF_Org_ID
                + "ORDER BY t.Rate DESC";
            bool found = false;
            try
            {
                DataSet pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    VAB_TaxRate_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            log.Fine("TaxExempt=Y - VAB_TaxRate_ID=" + VAB_TaxRate_ID);
            if (VAB_TaxRate_ID == 0)
            {
                log.SaveError("TaxCriteriaNotFound", Msg.GetMsg(ctx, "TaxNoExemptFound")
                    + (found ? "" : " (Tax/Org=" + VAF_Org_ID + " not found)"));
            }
            return VAB_TaxRate_ID;
        }

        /// <summary>
        /// Get Tax ID (Detail).
        /// If error return 0 and set error log (TaxNotFound)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_TaxCategory_ID">tax category</param>
        /// <param name="IsSOTrx">Sales Order Trx</param>
        /// <param name="shipDate">ship date (ignored)</param>
        /// <param name="shipFromC_Locction_ID">ship from (ignored)</param>
        /// <param name="shipToVAB_Address_ID">ship to (ignored)</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="billFromVAB_Address_ID">invoice from</param>
        /// <param name="billToVAB_Address_ID">invoice to</param>
        /// <returns>VAB_TaxRate_ID</returns>
        protected static int Get(Ctx ctx, int VAB_TaxCategory_ID, bool IsSOTrx,
            DateTime? shipDate, int shipFromC_Locction_ID, int shipToVAB_Address_ID,
            DateTime? billDate, int billFromVAB_Address_ID, int billToVAB_Address_ID)
        {
            //	VAB_TaxCategory contains CommodityCode
            //	API to Tax Vendor comes here

            //if (CLogMgt.IsLevelFinest())
            {
                log.Info("(Detail) - Category=" + VAB_TaxCategory_ID 
                   + ", SOTrx=" + IsSOTrx);
                log.Config("(Detail) - BillFrom=" + billFromVAB_Address_ID 
                    + ", BillTo=" + billToVAB_Address_ID + ", BillDate=" + billDate);
            }

            MVABTaxRate[] taxes = MVABTaxRate.GetAll(ctx);
            MVABAddress lFrom = new MVABAddress(ctx, billFromVAB_Address_ID, null);
            MVABAddress lTo = new MVABAddress(ctx, billToVAB_Address_ID, null);
            log.Finer("From=" + lFrom);
            log.Finer("To=" + lTo);

            List<MVABTaxRate> results = new List<MVABTaxRate>();
            for (int i = 0; i < taxes.Length; i++)
            {
                MVABTaxRate tax = taxes[i];
                if (tax.IsTaxExempt()
                    || !tax.IsActive()
                    || tax.GetVAB_TaxCategory_ID() != VAB_TaxCategory_ID
                    || tax.GetParent_Tax_ID() != 0)	//	user parent tax
                    continue;
                if (IsSOTrx && MVABTaxRate.SOPOTYPE_PurchaseTax.Equals(tax.GetSOPOType()))
                    continue;
                if (!IsSOTrx && MVABTaxRate.SOPOTYPE_SalesTax.Equals(tax.GetSOPOType()))
                    continue;

               // if (CLogMgt.IsLevelFinest())
                {
                    log.Finest(tax.ToString());
                    log.Finest("From Country - " + (tax.GetVAB_Country_ID() == lFrom.GetVAB_Country_ID()
                        || tax.GetVAB_Country_ID() == 0));
                    log.Finest("From Region - " + (tax.GetVAB_RegionState_ID() == lFrom.GetVAB_RegionState_ID()
                       || tax.GetVAB_RegionState_ID() == 0));
                    log.Finest("To Country - " + (tax.GetTo_Country_ID() == lTo.GetVAB_Country_ID()
                        || tax.GetTo_Country_ID() == 0));
                    log.Finest("To Region - " + (tax.GetTo_Region_ID() == lTo.GetVAB_RegionState_ID()
                        || tax.GetTo_Region_ID() == 0));
                    //log.Finest("Date valid - " + (!tax.GetValidFrom().after(billDate)));
                    log.Finest("Date valid - " + (!(tax.GetValidFrom() > (billDate))));
                }

                //	From Country
                if ((tax.GetVAB_Country_ID() == lFrom.GetVAB_Country_ID()
                        || tax.GetVAB_Country_ID() == 0)
                    //	From Region
                    && (tax.GetVAB_RegionState_ID() == lFrom.GetVAB_RegionState_ID()
                        || tax.GetVAB_RegionState_ID() == 0)
                    //	To Country
                    && (tax.GetTo_Country_ID() == lTo.GetVAB_Country_ID()
                        || tax.GetTo_Country_ID() == 0)
                    //	To Region
                    && (tax.GetTo_Region_ID() == lTo.GetVAB_RegionState_ID()
                        || tax.GetTo_Region_ID() == 0)
                    //	Date
                    //&& !tax.GetValidFrom().after(billDate)
                    && tax.GetValidFrom() > (billDate)
                    )
                {
                    if (!tax.IsPostal())
                    {
                        results.Add(tax);
                        continue;
                    }
                    //
                    MVABTaxZIP[] postals = tax.GetPostals(false);
                    for (int j = 0; j < postals.Length; j++)
                    {
                        MVABTaxZIP postal = postals[j];
                        if (postal.IsActive()
                            //	Postal From is mandatory
                            && postal.GetPostal().StartsWith(lFrom.GetPostal())
                            //	Postal To is optional
                            && (postal.GetPostal_To() == null
                                || postal.GetPostal_To().StartsWith(lTo.GetPostal()))
                            )
                        {
                            results.Add(tax);
                            continue;
                        }
                    }	//	for all postals
                }
            }	//	for all taxes

            //	One Result
            if (results.Count == 1)
            {
                return results[0].GetVAB_TaxRate_ID();
            }
            //	Multiple results - different valid from dates
            if (results.Count > 1)
            {
                MVABTaxRate latest = null;
                for (int i = 0; i < results.Count; i++)
                {
                    MVABTaxRate tax = results[i];
                    if (latest == null
                        //|| tax.GetValidFrom().after(latest.GetValidFrom()))
                        || tax.GetValidFrom() > (latest.GetValidFrom()))
                        latest = tax;
                }
                return latest.GetVAB_TaxRate_ID();
            }

            //	Default Tax
            for (int i = 0; i < taxes.Length; i++)
            {
                MVABTaxRate tax = taxes[i];
                if (!tax.IsDefault() || !tax.IsActive()
                    || tax.GetParent_Tax_ID() != 0)	//	user parent tax
                    continue;
                if (IsSOTrx && MVABTaxRate.SOPOTYPE_PurchaseTax.Equals(tax.GetSOPOType()))
                    continue;
                if (!IsSOTrx && MVABTaxRate.SOPOTYPE_SalesTax.Equals(tax.GetSOPOType()))
                    continue;
                log.Fine("(default) - " + tax);
                return tax.GetVAB_TaxRate_ID();
            }	//	for all taxes

            log.SaveError("TaxNotFound", "");
            return 0;
        }

    }
}
