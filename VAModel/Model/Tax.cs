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
using System.Windows.Forms;
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
        ///   M_Product_ID/C_Charge_ID	->	C_TaxCategory_ID
        ///   billDate, shipDate			->	billDate, shipDate
        ///   AD_Org_ID					->	billFromC_Location_ID
        ///   M_Warehouse_ID				->	shipFromC_Location_ID
        ///   billC_BPartner_Location_ID  ->	billToC_Location_ID
        ///   shipC_BPartner_Location_ID 	->	shipToC_Location_ID
        ///   if IsSOTrx is false, bill and ship are reversed
        ///   </pre>
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="C_Charge_ID">product</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="shipDate">ship date</param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="M_Warehouse_ID">warehouse</param>
        /// <param name="billC_BPartner_Location_ID">invoice location</param>
        /// <param name="shipC_BPartner_Location_ID">ship location</param>
        /// <param name="IsSOTrx">is a sales trx</param>
        /// <returns>C_Tax_ID If error it returns 0 and sets error log (TaxCriteriaNotFound)</returns>
        public static int Get(Ctx ctx, int M_Product_ID, int C_Charge_ID, DateTime? billDate, DateTime? shipDate,
            int AD_Org_ID, int M_Warehouse_ID, int billC_BPartner_Location_ID, int shipC_BPartner_Location_ID, bool IsSOTrx)
        {
            if (M_Product_ID != 0)
            {
                return GetProduct(ctx, M_Product_ID, billDate, shipDate, AD_Org_ID, M_Warehouse_ID,
                    billC_BPartner_Location_ID, shipC_BPartner_Location_ID, IsSOTrx);
            }
            else if (C_Charge_ID != 0)
            {
                return GetCharge(ctx, C_Charge_ID, billDate, shipDate, AD_Org_ID, M_Warehouse_ID,
                    billC_BPartner_Location_ID, shipC_BPartner_Location_ID, IsSOTrx);
            }
            else
            {
                return GetExemptTax(ctx, AD_Org_ID);
            }
        }

        /// <summary>
        /// Get Tax ID - converts parameters to call Get Tax.
        ///   <pre>
        ///   M_Product_ID/C_Charge_ID	->	C_TaxCategory_ID
        ///   billDate, shipDate			->	billDate, shipDate
        ///   AD_Org_ID					->	billFromC_Location_ID
        ///   M_Warehouse_ID				->	shipFromC_Location_ID
        ///   billC_BPartner_Location_ID  ->	billToC_Location_ID
        ///   shipC_BPartner_Location_ID 	->	shipToC_Location_ID
        ///   if IsSOTrx is false, bill and ship are reversed
        ///   </pre>
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="C_Charge_ID">product</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="shipDate">ship date</param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="M_Warehouse_ID">warehouse</param>
        /// <param name="billC_BPartner_Location_ID">invoice location</param>
        /// <param name="shipC_BPartner_Location_ID">ship location</param>
        /// <param name="IsSOTrx">is a sales trx</param>
        /// <returns>C_Tax_ID If error it returns 0 and sets error log (TaxCriteriaNotFound)</returns>
        public static int GetCharge(Ctx ctx, int C_Charge_ID, DateTime? billDate, DateTime? shipDate, int AD_Org_ID, int M_Warehouse_ID,
            int billC_BPartner_Location_ID, int shipC_BPartner_Location_ID, bool IsSOTrx)
        {
            if (M_Warehouse_ID == 0)
                M_Warehouse_ID = ctx.GetContextAsInt("M_Warehouse_ID");
            if (M_Warehouse_ID == 0)
            {
                log.Warning("No Warehouse - C_Charge_ID=" + C_Charge_ID);
                return 0;
            }
           // String variable = "";
            int C_TaxCategory_ID = 0;
            int shipFromC_Location_ID = 0;
            int shipToC_Location_ID = 0;
            int billFromC_Location_ID = 0;
            int billToC_Location_ID = 0;
            String IsTaxExempt = null;

            //	Get all at once
            String sql = "SELECT c.C_TaxCategory_ID, o.C_Location_ID, il.C_Location_ID, b.IsTaxExempt,"
                 + " w.C_Location_ID, sl.C_Location_ID "
                 + "FROM C_Charge c, AD_OrgInfo o,"
                 + " C_BPartner_Location il INNER JOIN C_BPartner b ON (il.C_BPartner_ID=b.C_BPartner_ID),"
                 + " M_Warehouse w, C_BPartner_Location sl "
                 + "WHERE c.C_Charge_ID=" + C_Charge_ID
                 + " AND o.AD_Org_ID=" + AD_Org_ID
                 + " AND il.C_BPartner_Location_ID=" + billC_BPartner_Location_ID
                 + " AND w.M_Warehouse_ID=" + M_Warehouse_ID
                 + " AND sl.C_BPartner_Location_ID=" + shipC_BPartner_Location_ID;
            try
            {
                DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
                bool found = false;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    C_TaxCategory_ID = Utility.Util.GetValueOfInt(dr[0]);
                    billFromC_Location_ID = Utility.Util.GetValueOfInt(dr[1]);
                    billToC_Location_ID = Utility.Util.GetValueOfInt(dr[2]);
                    IsTaxExempt = dr[3].ToString();
                    shipFromC_Location_ID = Utility.Util.GetValueOfInt(dr[4]);
                    shipToC_Location_ID = Utility.Util.GetValueOfInt(dr[5]);
                    found = true;
                }
                //
                if (!found)
                {
                    log.Warning("Not found for C_Charge_ID=" + C_Charge_ID 
                        + ", AD_Org_ID=" + AD_Org_ID + ", M_Warehouse_ID=" + M_Warehouse_ID
                        + ", C_BPartner_Location_ID=" + billC_BPartner_Location_ID 
                       + "/" + shipC_BPartner_Location_ID);
                    return 0;
                }
                else if ("Y".Equals(IsTaxExempt))
                    return GetExemptTax(ctx, AD_Org_ID);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
                return 0;
            }

            //	Reverese for PO
            if (!IsSOTrx)
            {
                int temp = billFromC_Location_ID;
                billFromC_Location_ID = billToC_Location_ID;
                billToC_Location_ID = temp;
                temp = shipFromC_Location_ID;
                shipFromC_Location_ID = shipToC_Location_ID;
                shipToC_Location_ID = temp;
            }
            //
            log.Fine("C_TaxCategory_ID=" + C_TaxCategory_ID
              + ", billFromC_Location_ID=" + billFromC_Location_ID
              + ", billToC_Location_ID=" + billToC_Location_ID
              + ", shipFromC_Location_ID=" + shipFromC_Location_ID
              + ", shipToC_Location_ID=" + shipToC_Location_ID);
            return Get(ctx, C_TaxCategory_ID, IsSOTrx,
              shipDate, shipFromC_Location_ID, shipToC_Location_ID,
              billDate, billFromC_Location_ID, billToC_Location_ID);
        }

        /// <summary>
        /// Get Tax ID - converts parameters to call Get Tax.
        ///   <pre>
        ///   M_Product_ID/C_Charge_ID	->	C_TaxCategory_ID
        ///   billDate, shipDate			->	billDate, shipDate
        ///   AD_Org_ID					->	billFromC_Location_ID
        ///   M_Warehouse_ID				->	shipFromC_Location_ID
        ///   billC_BPartner_Location_ID  ->	billToC_Location_ID
        ///   shipC_BPartner_Location_ID 	->	shipToC_Location_ID
        ///   if IsSOTrx is false, bill and ship are reversed
        ///   </pre>
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="C_Charge_ID">product</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="shipDate">ship date</param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="M_Warehouse_ID">warehouse</param>
        /// <param name="billC_BPartner_Location_ID">invoice location</param>
        /// <param name="shipC_BPartner_Location_ID">ship location</param>
        /// <param name="IsSOTrx">is a sales trx</param>
        /// <returns>C_Tax_ID If error it returns 0 and sets error log (TaxCriteriaNotFound)</returns>
        public static int GetProduct(Ctx ctx, int M_Product_ID, DateTime? billDate, DateTime? shipDate,
            int AD_Org_ID, int M_Warehouse_ID, int billC_BPartner_Location_ID, int shipC_BPartner_Location_ID,
            bool IsSOTrx)
        {
            String variable = "";
            int C_TaxCategory_ID = 0;
            int shipFromC_Location_ID = 0;
            int shipToC_Location_ID = 0;
            int billFromC_Location_ID = 0;
            int billToC_Location_ID = 0;
            String IsTaxExempt = null;

            try
            {
                //	Get all at once
                String sql = "SELECT p.C_TaxCategory_ID, o.C_Location_ID, il.C_Location_ID, b.IsTaxExempt,"
                    + " w.C_Location_ID, sl.C_Location_ID "
                    + "FROM M_Product p, AD_OrgInfo o,"
                    + " C_BPartner_Location il INNER JOIN C_BPartner b ON (il.C_BPartner_ID=b.C_BPartner_ID),"
                    + " M_Warehouse w, C_BPartner_Location sl "
                    + "WHERE p.M_Product_ID=" + M_Product_ID
                    + " AND o.AD_Org_ID=" + AD_Org_ID
                    + " AND il.C_BPartner_Location_ID=" + billC_BPartner_Location_ID
                    + " AND w.M_Warehouse_ID=" + M_Warehouse_ID
                    + " AND sl.C_BPartner_Location_ID=" + shipC_BPartner_Location_ID;
                DataSet pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                bool found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    C_TaxCategory_ID = Utility.Util.GetValueOfInt(dr[0]);
                    billFromC_Location_ID = Utility.Util.GetValueOfInt(dr[1]);
                    billToC_Location_ID = Utility.Util.GetValueOfInt(dr[2]);
                    IsTaxExempt = dr[3].ToString();
                    shipFromC_Location_ID = Utility.Util.GetValueOfInt(dr[4]);
                    shipToC_Location_ID = Utility.Util.GetValueOfInt(dr[5]);
                    found = true;

                }

                if (found && "Y".Equals(IsTaxExempt))
                {
                    log.Fine("Business Partner is Tax exempt");
                    return GetExemptTax(ctx, AD_Org_ID);
                }
                else if (found)
                {
                    if (!IsSOTrx)
                    {
                        int temp = billFromC_Location_ID;
                        billFromC_Location_ID = billToC_Location_ID;
                        billToC_Location_ID = temp;
                        temp = shipFromC_Location_ID;
                        shipFromC_Location_ID = shipToC_Location_ID;
                        shipToC_Location_ID = temp;
                    }
                    log.Fine("C_TaxCategory_ID=" + C_TaxCategory_ID
                        + ", billFromC_Location_ID=" + billFromC_Location_ID
                        + ", billToC_Location_ID=" + billToC_Location_ID
                        + ", shipFromC_Location_ID=" + shipFromC_Location_ID
                        + ", shipToC_Location_ID=" + shipToC_Location_ID);
                    return Get(ctx, C_TaxCategory_ID, IsSOTrx,
                        shipDate, shipFromC_Location_ID, shipToC_Location_ID,
                        billDate, billFromC_Location_ID, billToC_Location_ID);
                }

                // ----------------------------------------------------------------

                //	Detail for error isolation

                //	M_Product_ID				->	C_TaxCategory_ID
                sql = "SELECT C_TaxCategory_ID FROM M_Product "
                    + "WHERE M_Product_ID=" + M_Product_ID;
                variable = "M_Product_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    C_TaxCategory_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (C_TaxCategory_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(ctx, variable)
                       + (found ? "" : " (Product=" + M_Product_ID + " not found)"));
                    return 0;
                }
                log.Fine("C_TaxCategory_ID=" + C_TaxCategory_ID);
                //	AD_Org_ID					->	billFromC_Location_ID
                sql = "SELECT C_Location_ID FROM AD_OrgInfo "
                    + "WHERE AD_Org_ID=" + AD_Org_ID;
                variable = "AD_Org_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    billFromC_Location_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (billFromC_Location_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetAD_Language(ctx), variable)
                      + (found ? "" : " (Info/Org=" + AD_Org_ID + " not found)"));
                    return 0;
                }
                //	billC_BPartner_Location_ID  ->	billToC_Location_ID
                sql = "SELECT l.C_Location_ID, b.IsTaxExempt "
                    + "FROM C_BPartner_Location l INNER JOIN C_BPartner b ON (l.C_BPartner_ID=b.C_BPartner_ID) "
                    + "WHERE C_BPartner_Location_ID=" + billC_BPartner_Location_ID;
                variable = "BillTo_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    billToC_Location_ID = Utility.Util.GetValueOfInt(dr[0]);
                    IsTaxExempt = Convert.ToString(dr[1]);
                    found = true;
                }
                if (billToC_Location_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetAD_Language(ctx), variable)
                        + (found ? "" : " (BPLocation=" + billC_BPartner_Location_ID + " not found)"));
                    return 0;
                }
                if ("Y".Equals(IsTaxExempt))
                    return GetExemptTax(ctx, AD_Org_ID);

                //  Reverse for PO
                if (!IsSOTrx)
                {
                    int temp = billFromC_Location_ID;
                    billFromC_Location_ID = billToC_Location_ID;
                    billToC_Location_ID = temp;
                }
                log.Fine("billFromC_Location_ID = " + billFromC_Location_ID);
                log.Fine("billToC_Location_ID = " + billToC_Location_ID);
                
                //-----------------------------------------------------------------

                //	M_Warehouse_ID				->	shipFromC_Location_ID
                sql = "SELECT C_Location_ID FROM M_Warehouse "
                    + "WHERE M_Warehouse_ID=" + M_Warehouse_ID;
                variable = "M_Warehouse_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    shipFromC_Location_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (shipFromC_Location_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetAD_Language(ctx), variable)
                        + (found ? "" : " (Warehouse=" + M_Warehouse_ID + " not found)"));
                    return 0;
                }
                //	shipC_BPartner_Location_ID 	->	shipToC_Location_ID
                sql = "SELECT C_Location_ID FROM C_BPartner_Location "
                    + "WHERE C_BPartner_Location_ID=" + shipC_BPartner_Location_ID;
                variable = "C_BPartner_Location_ID";
                pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                found = false;

                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    shipToC_Location_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
                if (shipToC_Location_ID == 0)
                {
                    log.SaveError("TaxCriteriaNotFound", Msg.Translate(Env.GetAD_Language(ctx), variable)
                        + (found ? "" : " (BPLocation=" + shipC_BPartner_Location_ID + " not found)"));
                    return 0;
                }

                //  Reverse for PO
                if (!IsSOTrx)
                {
                    int temp = shipFromC_Location_ID;
                    shipFromC_Location_ID = shipToC_Location_ID;
                    shipToC_Location_ID = temp;
                }
                log.Fine("shipFromC_Location_ID = " + shipFromC_Location_ID);
                log.Fine("shipToC_Location_ID = " + shipToC_Location_ID);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "getProduct (" + variable + ")", e);
            }

            return Get(ctx, C_TaxCategory_ID, IsSOTrx,
                shipDate, shipFromC_Location_ID, shipToC_Location_ID,
                billDate, billFromC_Location_ID, billToC_Location_ID);
        }

        /// <summary>
        /// Get Exempt Tax Code
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Org_ID">org to find client</param>
        /// <returns>C_Tax_ID</returns>
        private static int GetExemptTax(Ctx ctx, int AD_Org_ID)
        {
            int C_Tax_ID = 0;
            String sql = "SELECT t.C_Tax_ID "
                + "FROM C_Tax t"
                + " INNER JOIN AD_Org o ON (t.AD_Client_ID=o.AD_Client_ID) "
                + "WHERE t.IsTaxExempt='Y' AND o.AD_Org_ID= " + AD_Org_ID
                + "ORDER BY t.Rate DESC";
            bool found = false;
            try
            {
                DataSet pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    C_Tax_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            log.Fine("TaxExempt=Y - C_Tax_ID=" + C_Tax_ID);
            if (C_Tax_ID == 0)
            {
                log.SaveError("TaxCriteriaNotFound", Msg.GetMsg(ctx, "TaxNoExemptFound")
                    + (found ? "" : " (Tax/Org=" + AD_Org_ID + " not found)"));
            }
            return C_Tax_ID;
        }

        /// <summary>
        /// Get Tax ID (Detail).
        /// If error return 0 and set error log (TaxNotFound)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_TaxCategory_ID">tax category</param>
        /// <param name="IsSOTrx">Sales Order Trx</param>
        /// <param name="shipDate">ship date (ignored)</param>
        /// <param name="shipFromC_Locction_ID">ship from (ignored)</param>
        /// <param name="shipToC_Location_ID">ship to (ignored)</param>
        /// <param name="billDate">invoice date</param>
        /// <param name="billFromC_Location_ID">invoice from</param>
        /// <param name="billToC_Location_ID">invoice to</param>
        /// <returns>C_Tax_ID</returns>
        protected static int Get(Ctx ctx, int C_TaxCategory_ID, bool IsSOTrx,
            DateTime? shipDate, int shipFromC_Locction_ID, int shipToC_Location_ID,
            DateTime? billDate, int billFromC_Location_ID, int billToC_Location_ID)
        {
            //	C_TaxCategory contains CommodityCode
            //	API to Tax Vendor comes here

            //if (CLogMgt.IsLevelFinest())
            {
                log.Info("(Detail) - Category=" + C_TaxCategory_ID 
                   + ", SOTrx=" + IsSOTrx);
                log.Config("(Detail) - BillFrom=" + billFromC_Location_ID 
                    + ", BillTo=" + billToC_Location_ID + ", BillDate=" + billDate);
            }

            MTax[] taxes = MTax.GetAll(ctx);
            MLocation lFrom = new MLocation(ctx, billFromC_Location_ID, null);
            MLocation lTo = new MLocation(ctx, billToC_Location_ID, null);
            log.Finer("From=" + lFrom);
            log.Finer("To=" + lTo);

            List<MTax> results = new List<MTax>();
            for (int i = 0; i < taxes.Length; i++)
            {
                MTax tax = taxes[i];
                if (tax.IsTaxExempt()
                    || !tax.IsActive()
                    || tax.GetC_TaxCategory_ID() != C_TaxCategory_ID
                    || tax.GetParent_Tax_ID() != 0)	//	user parent tax
                    continue;
                if (IsSOTrx && MTax.SOPOTYPE_PurchaseTax.Equals(tax.GetSOPOType()))
                    continue;
                if (!IsSOTrx && MTax.SOPOTYPE_SalesTax.Equals(tax.GetSOPOType()))
                    continue;

               // if (CLogMgt.IsLevelFinest())
                {
                    log.Finest(tax.ToString());
                    log.Finest("From Country - " + (tax.GetC_Country_ID() == lFrom.GetC_Country_ID()
                        || tax.GetC_Country_ID() == 0));
                    log.Finest("From Region - " + (tax.GetC_Region_ID() == lFrom.GetC_Region_ID()
                       || tax.GetC_Region_ID() == 0));
                    log.Finest("To Country - " + (tax.GetTo_Country_ID() == lTo.GetC_Country_ID()
                        || tax.GetTo_Country_ID() == 0));
                    log.Finest("To Region - " + (tax.GetTo_Region_ID() == lTo.GetC_Region_ID()
                        || tax.GetTo_Region_ID() == 0));
                    //log.Finest("Date valid - " + (!tax.GetValidFrom().after(billDate)));
                    log.Finest("Date valid - " + (!(tax.GetValidFrom() > (billDate))));
                }

                //	From Country
                if ((tax.GetC_Country_ID() == lFrom.GetC_Country_ID()
                        || tax.GetC_Country_ID() == 0)
                    //	From Region
                    && (tax.GetC_Region_ID() == lFrom.GetC_Region_ID()
                        || tax.GetC_Region_ID() == 0)
                    //	To Country
                    && (tax.GetTo_Country_ID() == lTo.GetC_Country_ID()
                        || tax.GetTo_Country_ID() == 0)
                    //	To Region
                    && (tax.GetTo_Region_ID() == lTo.GetC_Region_ID()
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
                    MTaxPostal[] postals = tax.GetPostals(false);
                    for (int j = 0; j < postals.Length; j++)
                    {
                        MTaxPostal postal = postals[j];
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
                return results[0].GetC_Tax_ID();
            }
            //	Multiple results - different valid from dates
            if (results.Count > 1)
            {
                MTax latest = null;
                for (int i = 0; i < results.Count; i++)
                {
                    MTax tax = results[i];
                    if (latest == null
                        //|| tax.GetValidFrom().after(latest.GetValidFrom()))
                        || tax.GetValidFrom() > (latest.GetValidFrom()))
                        latest = tax;
                }
                return latest.GetC_Tax_ID();
            }

            //	Default Tax
            for (int i = 0; i < taxes.Length; i++)
            {
                MTax tax = taxes[i];
                if (!tax.IsDefault() || !tax.IsActive()
                    || tax.GetParent_Tax_ID() != 0)	//	user parent tax
                    continue;
                if (IsSOTrx && MTax.SOPOTYPE_PurchaseTax.Equals(tax.GetSOPOType()))
                    continue;
                if (!IsSOTrx && MTax.SOPOTYPE_SalesTax.Equals(tax.GetSOPOType()))
                    continue;
                log.Fine("(default) - " + tax);
                return tax.GetC_Tax_ID();
            }	//	for all taxes

            log.SaveError("TaxNotFound", "");
            return 0;
        }

    }
}
