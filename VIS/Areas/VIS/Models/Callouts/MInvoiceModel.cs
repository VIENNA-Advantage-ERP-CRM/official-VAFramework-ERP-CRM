﻿using System;
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
    public class MInvoiceModel
    {
        /// <summary>
        /// GetInvoice
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetInvoice(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Invoice_ID;

            //Assign parameter value
            C_Invoice_ID = Util.GetValueOfInt(paramValue[0].ToString());

            MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["IsSOTrx"] = inv.IsSOTrx().ToString();
            result["IsReturnTrx"] = inv.IsReturnTrx().ToString();
            result["C_BPartner_ID"] = inv.GetC_BPartner_ID().ToString();
            result["M_PriceList_ID"] = inv.GetM_PriceList_ID().ToString();
            result["AD_Org_ID"] = inv.GetAD_Org_ID().ToString();
            result["C_BPartner_Location_ID"] = inv.GetC_BPartner_Location_ID().ToString();
            result["C_Currency_ID"] = inv.GetC_Currency_ID().ToString();
            //result["DateAcct"] = Util.GetValueOfString(inv.GetDateAcct());
            return result;
        }

        /// <summary>
        /// Get Product Detail with Price 
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Parameters</param>
        /// <writer>VIS_0045</writer>
        /// <returns>Product Info</returns>
        public Dictionary<string, object> GetInvoiceProductInfo(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Invoice_ID, _m_Product_Id = 0, M_AttributeSetInstance_ID = 0, M_PriceList_Version_ID = 0;
            decimal Qty = 0;

            //Assign parameter value
            C_Invoice_ID = Util.GetValueOfInt(paramValue[0].ToString());
            _m_Product_Id = Util.GetValueOfInt(paramValue[1].ToString());
            M_AttributeSetInstance_ID = Util.GetValueOfInt(paramValue[2].ToString());
            Qty = Util.GetValueOfDecimal(paramValue[3].ToString());

            MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["IsSOTrx"] = inv.IsSOTrx().ToString();
            result["IsReturnTrx"] = inv.IsReturnTrx().ToString();
            result["C_BPartner_ID"] = inv.GetC_BPartner_ID().ToString();
            result["M_PriceList_ID"] = inv.GetM_PriceList_ID().ToString();
            result["AD_Org_ID"] = inv.GetAD_Org_ID().ToString();
            result["C_BPartner_Location_ID"] = inv.GetC_BPartner_Location_ID().ToString();
            result["C_Currency_ID"] = inv.GetC_Currency_ID().ToString();

            if (_m_Product_Id > 0)
            {
                StringBuilder sql = new StringBuilder();
                DataSet dsProductInfo;
                int C_UOM_ID = 0;

                // check Module existense
                result["countEd011"] = Env.IsModuleInstalled("ED011_") ? 1 : 0;

                // Get Purchasing UOM
                sql.Append("SELECT C_UOM_ID FROM M_Product_PO WHERE IsActive = 'Y' AND C_BPartner_ID = " + inv.GetC_BPartner_ID() +
                        " AND M_Product_ID = " + _m_Product_Id);
                result["purchasingUom"] = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));

                // Get Product Detail
                sql.Clear();
                sql.Append(@"SELECT ProductType, C_UOM_ID, IsDropShip, DocumentNote, C_RevenueRecognition_ID FROM M_Product WHERE
                    IsActive = 'Y' AND M_Product_ID = " + _m_Product_Id);
                dsProductInfo = DB.ExecuteDataset(sql.ToString());
                if (dsProductInfo != null && dsProductInfo.Tables[0].Rows.Count > 0)
                {
                    result["productType"] = Util.GetValueOfString(dsProductInfo.Tables[0].Rows[0]["ProductType"]);
                    result["headerUom"] = Util.GetValueOfInt(dsProductInfo.Tables[0].Rows[0]["C_UOM_ID"]);
                    result["IsDropShip"] = Util.GetValueOfString(dsProductInfo.Tables[0].Rows[0]["IsDropShip"]);
                    result["DocumentNote"] = Util.GetValueOfString(dsProductInfo.Tables[0].Rows[0]["DocumentNote"]);
                    result["C_RevenueRecognition_ID"] = Util.GetValueOfInt(dsProductInfo.Tables[0].Rows[0]["C_RevenueRecognition_ID"]);
                }

                // Get Purchasing or Base UOM
                if (Util.GetValueOfInt(result["purchasingUom"]) > 0 && !inv.IsSOTrx())
                {
                    C_UOM_ID = Util.GetValueOfInt(result["purchasingUom"]);
                }
                else
                {
                    C_UOM_ID = Util.GetValueOfInt(result["headerUom"]);
                }

                // Get Price Precision
                MPriceList prcLst = MPriceList.Get(ctx, inv.GetM_PriceList_ID(), null);
                result["PriceListPrecision"] = prcLst.GetPricePrecision();
                result["StdPrecision"] = prcLst.GetStandardPrecision();

                // UOM Precision
                result["UOMPrecision"] = MUOM.Get(ctx, Util.GetValueOfInt(result["headerUom"])).GetStdPrecision();

                if (inv.GetM_PriceList_ID() > 0 && _m_Product_Id > 0
                    && inv.GetDateInvoiced() != null)
                {
                    sql.Clear();
                    sql.Append("SELECT plv.M_PriceList_Version_ID FROM M_PriceList_Version plv "
                                + " JOIN M_ProductPrice pp ON plv.M_PriceList_Version_ID = pp.M_PriceList_Version_ID "
                                + " WHERE plv.IsActive = 'Y' AND pp.IsActive = 'Y' AND plv.M_PriceList_ID = " + inv.GetM_PriceList_ID()
                                + @" AND plv.VALIDFROM <= " + GlobalVariable.TO_DATE(inv.GetDateInvoiced(), true)
                                + " AND NVL(pp.M_Product_ID, 0) = " + Util.GetValueOfInt(_m_Product_Id));

                    if (C_UOM_ID > 0)
                        sql.Append(" AND NVL(pp.C_UOM_ID, 0) = " + C_UOM_ID);

                    if (M_AttributeSetInstance_ID > 0)
                        sql.Append(" AND NVL(pp.M_AttributeSetInstance_ID, 0) = " + M_AttributeSetInstance_ID);

                    sql.Append(" ORDER BY plv.VALIDFROM DESC, plv.M_PriceList_Version_ID DESC");

                    M_PriceList_Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString()));
                }

                MProductPricing pp = new MProductPricing(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(),
                      _m_Product_Id, inv.GetC_BPartner_ID(), Qty, inv.IsSOTrx());
                pp.SetM_PriceList_ID(inv.GetM_PriceList_ID());
                pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                pp.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
                pp.SetPriceDate(inv.GetDateInvoiced());
                pp.SetPriceDate1(inv.GetDateInvoiced());
                pp.SetC_UOM_ID(C_UOM_ID);

                result["PriceList"] = M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceList();
                result["PriceLimit"] = M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceLimit();
                result["PriceActual"] = M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceStd();
                result["PriceEntered"] = M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceStd();
                result["PriceStd"] = M_PriceList_Version_ID == 0 ? 0 : pp.GetPriceStd();
                result["Discount"] = pp.GetDiscount();
                result["C_UOM_ID"] = pp.GetC_UOM_ID();
                result["EnforcePriceLimit"] = pp.IsEnforcePriceLimit();
                result["DiscountSchema"] = pp.IsDiscountSchema();
                result["M_PriceList_Version_ID"] = M_PriceList_Version_ID;

                if (Util.GetValueOfInt(result["purchasingUom"]) > 0 && !inv.IsSOTrx())
                {
                    // Set QtyInvoiced when purchasing UOM != Product Header UOM
                    if (Util.GetValueOfInt(result["headerUom"]) != Util.GetValueOfInt(result["purchasingUom"]))
                    {
                        sql.Clear();
                        sql.Append(@"SELECT con.DivideRate FROM C_UOM_Conversion con 
                            INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID 
                            WHERE con.IsActive = 'Y' AND con.M_Product_ID = " + _m_Product_Id +
                            " AND con.C_UOM_ID = " + Util.GetValueOfInt(result["headerUom"]) +
                            " AND con.C_UOM_To_ID = " + Util.GetValueOfInt(result["purchasingUom"]));
                        Decimal rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString()));
                        if (rate == 0)
                        {
                            sql.Clear();
                            sql.Append(@"SELECT con.DivideRate FROM C_UOM_Conversion con 
                                INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID 
                                WHERE con.IsActive = 'Y'" +
                                " AND con.C_UOM_ID = " + Util.GetValueOfInt(result["headerUom"]) +
                                " AND con.C_UOM_To_ID = " + Util.GetValueOfInt(result["purchasingUom"]));
                            rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString()));
                        }
                        if ((Util.GetValueOfInt(result["headerUom"]) == Util.GetValueOfInt(result["purchasingUom"])) && (rate == 0))
                        {
                            rate = 1;
                        }
                        result["QtyInvoiced"] = Decimal.Round((Qty * rate), Util.GetValueOfInt(result["UOMPrecision"]), MidpointRounding.AwayFromZero);
                    }
                }

                // Roundup Value with Price List Precision
                result["PriceActual"] = Decimal.Round(Util.GetValueOfDecimal(result["PriceActual"]), prcLst.GetPricePrecision(), MidpointRounding.AwayFromZero);
                result["PriceEntered"] = result["PriceActual"];
                result["PriceStd"] = result["PriceActual"];
                result["PriceList"] = Decimal.Round(Util.GetValueOfDecimal(result["PriceList"]), prcLst.GetPricePrecision(), MidpointRounding.AwayFromZero);
                result["PriceLimit"] = Decimal.Round(Util.GetValueOfDecimal(result["PriceLimit"]), prcLst.GetPricePrecision(), MidpointRounding.AwayFromZero);
            }

            return result;

        }

        /// <summary>
        /// Get Tax ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Int32 GetTaxNew(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int C_Tax_ID = 0;
            int C_Invoice_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int C_Charge_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int taxCategory = 0;
            string sql = "";
            if ((M_Product_ID == 0 && C_Charge_ID == 0) || C_Invoice_ID == 0)
            {
                return C_Tax_ID;
            }
            DataSet dsLoc = null;
            MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            MBPartner bp = new MBPartner(ctx, inv.GetC_BPartner_ID(), null);
            if (bp.IsTaxExempt())
            {
                C_Tax_ID = GetExemptTax(ctx, inv.GetAD_Org_ID());
                return C_Tax_ID;
            }
            if (M_Product_ID > 0)
            {
                MProduct prod = new MProduct(ctx, M_Product_ID, null);
                taxCategory = Util.GetValueOfInt(prod.GetC_TaxCategory_ID());
            }
            if (C_Charge_ID > 0)
            {
                MCharge chrg = new MCharge(ctx, C_Charge_ID, null);
                taxCategory = Util.GetValueOfInt(chrg.GetC_TaxCategory_ID());
            }
            if (taxCategory > 0)
            {
                MTaxCategory taxCat = new MTaxCategory(ctx, taxCategory, null);
                int Country_ID = 0, Region_ID = 0, orgCountry = 0, orgRegion = 0, taxRegion = 0;
                string Postal = "", orgPostal = "";
                sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                    + inv.GetC_BPartner_Location_ID() + " AND bpl.IsActive = 'Y'";
                dsLoc = DB.ExecuteDataset(sql, null, null);
                if (dsLoc != null)
                {
                    if (dsLoc.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsLoc.Tables[0].Rows.Count; j++)
                        {
                            Country_ID = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][0]);
                            Region_ID = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][1]);
                            Postal = Util.GetValueOfString(dsLoc.Tables[0].Rows[j][2]);
                        }
                    }
                }
                dsLoc = null;
                sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc LEFT JOIN AD_OrgInfo org ON loc.C_Location_ID = org.C_Location_ID WHERE org.AD_Org_ID ="
                        + inv.GetAD_Org_ID() + " AND org.IsActive = 'Y'";
                dsLoc = DB.ExecuteDataset(sql, null, null);
                if (dsLoc != null)
                {
                    if (dsLoc.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsLoc.Tables[0].Rows.Count; j++)
                        {
                            orgCountry = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][0]);
                            orgRegion = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][1]);
                            orgPostal = Util.GetValueOfString(dsLoc.Tables[0].Rows[j][2]);
                        }
                    }
                }
                // if Tax Preference 1 is Tax Class
                if (taxCat.GetVATAX_Preference1() == "T")
                {
                    sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                                   " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                    int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (taxType == 0)
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                        taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (taxType == 0)
                        {
                            if (taxCat.GetVATAX_Preference2() == "L")
                            {
                                C_Tax_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (C_Tax_ID > 0)
                                {
                                    return C_Tax_ID;
                                }
                                else
                                {
                                    if (taxCat.GetVATAX_Preference3() == "R")
                                    {
                                        if (Country_ID > 0)
                                        {
                                            dsLoc = null;
                                            sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE C_TaxCategory_ID = " + taxCategory +
                                                " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND C_Country_ID = " + Country_ID;
                                            dsLoc = DB.ExecuteDataset(sql, null, null);
                                            if (dsLoc != null)
                                            {
                                                if (dsLoc.Tables[0].Rows.Count > 0)
                                                {

                                                }
                                                else
                                                {
                                                    C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                    if (C_Tax_ID > 0)
                                                    {
                                                        return C_Tax_ID;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                if (C_Tax_ID > 0)
                                                {
                                                    return C_Tax_ID;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                            if (C_Tax_ID > 0)
                                            {
                                                return C_Tax_ID;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (taxCat.GetVATAX_Preference2() == "R")
                            {
                                if (Country_ID > 0)
                                {
                                    dsLoc = null;
                                    sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE C_TaxCategory_ID = " + taxCategory +
                                        " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND C_Country_ID = " + Country_ID;
                                    dsLoc = DB.ExecuteDataset(sql, null, null);
                                    if (dsLoc != null)
                                    {
                                        if (dsLoc.Tables[0].Rows.Count > 0)
                                        {

                                        }
                                        else
                                        {
                                            C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                            if (C_Tax_ID > 0)
                                            {
                                                return C_Tax_ID;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                        if (C_Tax_ID > 0)
                                        {
                                            return C_Tax_ID;
                                        }
                                    }
                                }
                                else
                                {
                                    C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (C_Tax_ID > 0)
                                    {
                                        return C_Tax_ID;
                                    }
                                }
                                if (taxCat.GetVATAX_Preference3() == "L")
                                {
                                    C_Tax_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (C_Tax_ID > 0)
                                    {
                                        return C_Tax_ID;
                                    }
                                }
                            }
                        }
                    }
                    if (taxType > 0)
                    {
                        sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID  WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                            " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                        C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                    }
                }
                // if Tax Preference 1 is Location
                else if (taxCat.GetVATAX_Preference1() == "L")
                {
                    C_Tax_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                    if (C_Tax_ID > 0)
                    {
                        return C_Tax_ID;
                    }
                    else
                    {
                        if (taxCat.GetVATAX_Preference2() == "T")
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                                           " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                            int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (taxType == 0)
                            {
                                sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                                taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (taxType == 0)
                                {
                                    if (taxCat.GetVATAX_Preference3() == "R")
                                    {
                                        if (Country_ID > 0)
                                        {
                                            dsLoc = null;
                                            sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE C_TaxCategory_ID = " + taxCategory +
                                                " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND C_Country_ID = " + Country_ID;
                                            dsLoc = DB.ExecuteDataset(sql, null, null);
                                            if (dsLoc != null)
                                            {
                                                if (dsLoc.Tables[0].Rows.Count > 0)
                                                {

                                                }
                                                else
                                                {
                                                    C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                    if (C_Tax_ID > 0)
                                                    {
                                                        return C_Tax_ID;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                if (C_Tax_ID > 0)
                                                {
                                                    return C_Tax_ID;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                            if (C_Tax_ID > 0)
                                            {
                                                return C_Tax_ID;
                                            }
                                        }
                                    }
                                }
                            }
                            if (taxType > 0)
                            {
                                sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                    " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (C_Tax_ID > 0)
                                {
                                    return C_Tax_ID;
                                }
                            }
                        }
                        else if (taxCat.GetVATAX_Preference2() == "R")
                        {
                            if (Country_ID > 0)
                            {
                                dsLoc = null;
                                sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE C_TaxCategory_ID = " + taxCategory +
                                    " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND C_Country_ID = " + Country_ID;
                                dsLoc = DB.ExecuteDataset(sql, null, null);
                                if (dsLoc != null)
                                {
                                    if (dsLoc.Tables[0].Rows.Count > 0)
                                    {

                                    }
                                    else
                                    {
                                        C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                        if (C_Tax_ID > 0)
                                        {
                                            return C_Tax_ID;
                                        }
                                    }
                                }
                                else
                                {
                                    C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (C_Tax_ID > 0)
                                    {
                                        return C_Tax_ID;
                                    }
                                }
                            }
                            else
                            {
                                C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (C_Tax_ID > 0)
                                {
                                    return C_Tax_ID;
                                }
                            }
                            if (taxCat.GetVATAX_Preference3() == "T")
                            {
                                sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                                               " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                                int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (taxType == 0)
                                {
                                    sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                                    taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                }
                                if (taxType > 0)
                                {
                                    sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                        " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                    C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                    if (C_Tax_ID > 0)
                                    {
                                        return C_Tax_ID;
                                    }
                                }
                            }
                        }
                    }
                }
                // if Tax Preference 1 is Tax Region
                else if (taxCat.GetVATAX_Preference1() == "R")
                {
                    if (Country_ID > 0)
                    {
                        dsLoc = null;
                        sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE C_TaxCategory_ID = " + taxCategory +
                            " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND C_Country_ID = " + Country_ID;
                        dsLoc = DB.ExecuteDataset(sql, null, null);
                        if (dsLoc != null)
                        {
                            if (dsLoc.Tables[0].Rows.Count > 0)
                            {

                            }
                            else
                            {
                                C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (C_Tax_ID > 0)
                                {
                                    return C_Tax_ID;
                                }
                            }
                        }
                        else
                        {
                            C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                            if (C_Tax_ID > 0)
                            {
                                return C_Tax_ID;
                            }
                        }
                    }
                    else
                    {
                        C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                    }
                    if (taxCat.GetVATAX_Preference2() == "T")
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                                       " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                        int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (taxType == 0)
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                            taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (taxType == 0)
                            {
                                if (taxCat.GetVATAX_Preference3() == "L")
                                {
                                    C_Tax_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (C_Tax_ID > 0)
                                    {
                                        return C_Tax_ID;
                                    }
                                }
                            }
                        }
                        if (taxType > 0)
                        {
                            sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (C_Tax_ID > 0)
                            {
                                return C_Tax_ID;
                            }
                        }
                    }
                    else if (taxCat.GetVATAX_Preference2() == "L")
                    {
                        C_Tax_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                        if (taxCat.GetVATAX_Preference3() == "T")
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                                           " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                            int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (taxType == 0)
                            {
                                sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                                taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            }
                            if (taxType > 0)
                            {
                                sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                    " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (C_Tax_ID > 0)
                                {
                                    return C_Tax_ID;
                                }
                            }
                        }
                    }
                }
                if (taxCat.GetVATAX_Preference1() == "R" || taxCat.GetVATAX_Preference2() == "R" || taxCat.GetVATAX_Preference3() == "R")
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxRegion tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.IsDefault = 'Y' AND tcr.IsActive = 'Y' 
                    AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "') ORDER BY tcr.Updated";
                    C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (C_Tax_ID > 0)
                    {
                        return C_Tax_ID;
                    }
                }
                sql = @"SELECT tcr.C_Tax_ID FROM C_TaxCategory tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory + " AND tcr.IsActive = 'Y'";
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                return C_Tax_ID;
            }
            return C_Tax_ID;
        }

        // on change of Tax
        // Added by Bharat on 27 Feb 2018 to check Exempt Tax on Business Partner
        public Dictionary<String, object> GetTaxId(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            //Assign parameter value            
            int C_Invoice_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int C_Charge_ID = Util.GetValueOfInt(paramValue[2].ToString());
            //End Assign parameter value

            Dictionary<String, object> retDic = new Dictionary<string, object>();
            string sql = null;
            int taxId = 0;
            int _c_BPartner_Id = 0;
            int _c_Bill_Location_Id = 0;
            int _CountVATAX = 0;

            _CountVATAX = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX IN ('VATAX_' )", null, null));
            retDic["_CountVATAX"] = _CountVATAX.ToString();

            Dictionary<string, string> order = GetInvoice(ctx, C_Invoice_ID.ToString());
            _c_BPartner_Id = Util.GetValueOfInt(order["C_BPartner_ID"]);
            _c_Bill_Location_Id = Util.GetValueOfInt(order["C_BPartner_Location_ID"]);

            MBPartner bp = new MBPartner(ctx, _c_BPartner_Id, null);
            retDic["TaxExempt"] = bp.IsTaxExempt() ? "Y" : "N";

            if (_CountVATAX > 0)
            {
                sql = "SELECT VATAX_TaxRule FROM AD_OrgInfo WHERE AD_Org_ID=" + Util.GetValueOfInt(order["AD_Org_ID"]) + " AND IsActive ='Y' AND AD_Client_ID =" + ctx.GetAD_Client_ID();
                string taxRule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
                retDic["taxRule"] = taxRule.ToString();

                sql = "SELECT Count(*) FROM AD_Column WHERE ColumnName = 'C_Tax_ID' AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName = 'C_TaxCategory')";
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                {
                    var paramString = (C_Invoice_ID).ToString() + "," + (M_Product_ID).ToString() + "," + (C_Charge_ID).ToString();

                    taxId = GetTax(ctx, paramString);
                }
                else
                {
                    sql = "SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + Util.GetValueOfInt(_c_BPartner_Id) +
                                      " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + Util.GetValueOfInt(_c_Bill_Location_Id);
                    var taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (taxType == 0)
                    {
                        sql = "SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + Util.GetValueOfInt(_c_BPartner_Id) + " AND IsActive = 'Y'";
                        taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }

                    MProductModel objProduct = new MProductModel();
                    var prodtaxCategory = objProduct.GetTaxCategory(ctx, M_Product_ID.ToString());
                    sql = "SELECT C_Tax_ID FROM VATAX_TaxCatRate WHERE C_TaxCategory_ID = " + prodtaxCategory + " AND IsActive ='Y' AND VATAX_TaxType_ID =" + taxType;
                    taxId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                }
            }
            retDic["taxId"] = taxId.ToString();

            return retDic;
        }


        /// <summary>
        /// Get Tax ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Int32 GetTax(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int C_Tax_ID = 0;
            int C_Invoice_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int C_Charge_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int taxCategory = 0;
            string sql = "";
            if ((M_Product_ID == 0 && C_Charge_ID == 0) || C_Invoice_ID == 0)
            {
                return C_Tax_ID;
            }
            DataSet dsLoc = null;
            MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            MBPartner bp = new MBPartner(ctx, inv.GetC_BPartner_ID(), null);
            if (bp.IsTaxExempt())
            {
                C_Tax_ID = GetExemptTax(ctx, inv.GetAD_Org_ID());
                return C_Tax_ID;
            }
            if (M_Product_ID > 0)
            {
                MProduct prod = new MProduct(ctx, M_Product_ID, null);
                taxCategory = Util.GetValueOfInt(prod.GetC_TaxCategory_ID());
            }
            if (C_Charge_ID > 0)
            {
                MCharge chrg = new MCharge(ctx, C_Charge_ID, null);
                taxCategory = Util.GetValueOfInt(chrg.GetC_TaxCategory_ID());
            }
            if (taxCategory > 0)
            {
                MTaxCategory taxCat = new MTaxCategory(ctx, taxCategory, null);
                int Country_ID = 0, Region_ID = 0, orgCountry = 0, orgRegion = 0, taxRegion = 0;
                string Postal = "", orgPostal = "";
                sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                    + inv.GetC_BPartner_Location_ID() + " AND bpl.IsActive = 'Y'";
                dsLoc = DB.ExecuteDataset(sql, null, null);
                if (dsLoc != null)
                {
                    if (dsLoc.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsLoc.Tables[0].Rows.Count; j++)
                        {
                            Country_ID = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][0]);
                            Region_ID = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][1]);
                            Postal = Util.GetValueOfString(dsLoc.Tables[0].Rows[j][2]);
                        }
                    }
                }
                dsLoc = null;
                sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc LEFT JOIN AD_OrgInfo org ON loc.C_Location_ID = org.C_Location_ID WHERE org.AD_Org_ID ="
                        + inv.GetAD_Org_ID() + " AND org.IsActive = 'Y'";
                dsLoc = DB.ExecuteDataset(sql, null, null);
                if (dsLoc != null)
                {
                    if (dsLoc.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsLoc.Tables[0].Rows.Count; j++)
                        {
                            orgCountry = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][0]);
                            orgRegion = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][1]);
                            orgPostal = Util.GetValueOfString(dsLoc.Tables[0].Rows[j][2]);
                        }
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    string pref = taxCat.GetVATAX_Preference1();
                    if (i == 1)
                    {
                        pref = taxCat.GetVATAX_Preference2();
                    }
                    else if (i == 2)
                    {
                        pref = taxCat.GetVATAX_Preference3();
                    }

                    // if Tax Preference  is Tax Class
                    if (pref == "T")
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                                       " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                        int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (taxType == 0)
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                            taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        }
                        if (taxType > 0)
                        {
                            sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID  WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxBase = 'T' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (C_Tax_ID > 0)
                            {
                                return C_Tax_ID;
                            }
                        }
                    }
                    // if Tax Preference is Location
                    else if (pref == "L")
                    {
                        C_Tax_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                    }
                    // if Tax Preference is Tax Region
                    else if (pref == "R")
                    {
                        if (Country_ID > 0)
                        {
                            dsLoc = null;
                            sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE C_TaxCategory_ID = " + taxCategory +
                                " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND C_Country_ID = " + Country_ID;
                            dsLoc = DB.ExecuteDataset(sql, null, null);
                            if (dsLoc != null)
                            {
                                if (dsLoc.Tables[0].Rows.Count > 0)
                                {

                                }
                                else
                                {
                                    C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (C_Tax_ID > 0)
                                    {
                                        return C_Tax_ID;
                                    }
                                }
                            }
                            else
                            {
                                C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (C_Tax_ID > 0)
                                {
                                    return C_Tax_ID;
                                }
                            }
                        }
                        else
                        {
                            C_Tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                            if (C_Tax_ID > 0)
                            {
                                return C_Tax_ID;
                            }
                        }
                    }

                    // if Tax Preference is Document Type
                    else if (pref == "D")
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM C_DocType WHERE C_DocType_ID = " + inv.GetC_DocTypeTarget_ID();
                        int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        if (taxType > 0)
                        {
                            sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID  WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxBase = 'T' AND tcr.VATAX_TaxType_ID = " + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (C_Tax_ID > 0)
                            {
                                return C_Tax_ID;
                            }
                        }
                    }
                }
                if (taxCat.GetVATAX_Preference1() == "R" || taxCat.GetVATAX_Preference2() == "R" || taxCat.GetVATAX_Preference3() == "R")
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxRegion tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.IsDefault = 'Y' AND tcr.IsActive = 'Y' 
                    AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "') ORDER BY tcr.Updated";
                    C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (C_Tax_ID > 0)
                    {
                        return C_Tax_ID;
                    }
                }
                sql = @"SELECT tcr.C_Tax_ID FROM C_TaxCategory tcr WHERE tcr.C_TaxCategory_ID = " + taxCategory + " AND tcr.IsActive = 'Y'";
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                return C_Tax_ID;
            }
            return C_Tax_ID;
        }

        // Return Exempted Tax Fro the Organization
        VLogger log = VLogger.GetVLogger("Tax");
        private int GetExemptTax(Ctx ctx, int AD_Org_ID)
        {
            int C_Tax_ID = 0;
            String sql = "SELECT t.C_Tax_ID "
                + "FROM C_Tax t"
                + " INNER JOIN AD_Org o ON (t.AD_Client_ID=o.AD_Client_ID) "
                + "WHERE t.IsActive='Y' AND t.IsTaxExempt='Y' AND o.AD_Org_ID= " + AD_Org_ID
                + "ORDER BY t.Rate DESC";
            bool found = false;
            try
            {
                DataSet pstmt = DB.ExecuteDataset(sql, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    C_Tax_ID = Util.GetValueOfInt(dr[0]);
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

        private int GetTaxFromLocation(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal)
        {
            string sql = "";
            int C_Tax_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                    " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND NVL(tcr.C_Region_ID,0) = " + Region_ID +
                    " AND tcr.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                    " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND NVL(tcr.C_Region_ID,0) = " + Region_ID +
                    " AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= '" + Postal + "' AND tcr.postal_to >= '" + Postal + "' THEN 1 ELSE 2" +
                    " END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            if (C_Tax_ID > 0)
            {
                return C_Tax_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                        " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND tcr.C_Region_ID IS NULL AND tcr.Postal IS NULL AND tx.SOPOType IN ('B','"
                        + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                        " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND tcr.C_Region_ID IS NULL AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= '" + Postal +
                        "' AND tcr.postal_to >= '" + Postal + "' THEN 1 ELSE 2" + " END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                        + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (C_Tax_ID > 0)
                {
                    return C_Tax_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                            " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID IS NULL " + " AND tcr.C_Region_ID IS NULL AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= '"
                            + Postal + "' AND tcr.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                            + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                    if (C_Tax_ID > 0)
                    {
                        return C_Tax_ID;
                    }
                }
            }
            return C_Tax_ID;
        }

        private int GetTaxFromRegion(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal)
        {
            string sql = "";
            int C_Tax_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND NVL(trl.C_Region_ID,0) = " + Region_ID +
                " AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND NVL(trl.C_Region_ID,0) = " + Region_ID +
                " AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '" + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '"
                + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            if (C_Tax_ID > 0)
            {
                return C_Tax_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND trl.C_Region_ID IS NULL AND trl.Postal IS NULL AND tx.SOPOType IN ('B','"
                    + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                    + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (C_Tax_ID > 0)
                {
                    return C_Tax_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                        + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID IS NULL AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                        + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                    }
                }
            }
            return C_Tax_ID;
        }

        private int GetTaxFromRegion(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal, int taxRegion, int toCountry)
        {
            string sql = "";
            int C_Tax_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID +
                " AND NVL(trl.C_Region_ID,0) = " + Region_ID + " AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID +
                " AND NVL(trl.C_Region_ID,0) = " + Region_ID + " AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '" + Postal + "' AND trl.postal_to >= '" + Postal +
                "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            if (C_Tax_ID > 0)
            {
                return C_Tax_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID +
                    " AND trl.C_Region_ID IS NULL AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                    + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (C_Tax_ID > 0)
                {
                    return C_Tax_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                        + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID IS NULL AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                        + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                    }
                }
            }
            return C_Tax_ID;
        }
        // Added by mohit to remove client side queries- 12 May 2017
        /// <summary>
        /// Get Invoice payment Schedule Details
        /// </summary>        
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetInvPaySchedDetail(string fields)
        {
            Dictionary<string, object> retValue = null;
            int Invoice_ID = Util.GetValueOfInt(fields);
            DataSet _ds = null;
            string _Sql;
            //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'", null, null)) > 0)
            if (Env.IsModuleInstalled("VA009_"))
            {
                //Added 2 new fields to get VA009_PaymentMethod_ID and VA009_PaymentBaseType To Set the corrosponding value on Payment Window..
                //(1052) apply IsHoldPayment check
                _Sql = "SELECT * FROM (SELECT ips.C_InvoicePaySchedule_ID,"
                             + " NVL(ips.DueAmt , 0) - NVL(ips.va009_paidamntinvce , 0) AS DueAmt, i.IsReturnTrx, IPS.VA009_PaymentMethod_ID, PM.VA009_PaymentBaseType FROM C_Invoice i"
                             + " INNER JOIN C_InvoicePaySchedule ips ON (i.C_Invoice_ID = ips.C_Invoice_ID) "
                             + " INNER JOIN VA009_PAYMENTMETHOD PM  ON (ips.VA009_PaymentMethod_ID = PM.VA009_paymentMethod_ID) WHERE i.IsPayScheduleValid='Y' "
                             + " AND ips.IsValid ='Y' AND ips.isactive ='Y' "
                             + " AND i.C_Invoice_ID = " + Invoice_ID
                             + " AND ips.IsHoldPayment = 'N'"
                             + " AND ips.C_InvoicePaySchedule_ID NOT IN"
                             + "(SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE c_payment_id IN"
                             + "(SELECT NVL(c_payment_id,0) FROM C_InvoicePaySchedule)  union "
                             + " SELECT NVL(C_InvoicePaySchedule_id,0) FROM C_InvoicePaySchedule WHERE c_cashline_id IN"
                             + "(SELECT NVL(c_cashline_id,0) FROM C_InvoicePaySchedule )) "
                             + " ORDER BY ips.duedate ASC) t WHERE rownum=1";
            }
            else
            {
                _Sql = "SELECT * FROM (SELECT ips.C_InvoicePaySchedule_ID,"
                                + " ips.DueAmt, i.IsReturnTrx FROM C_Invoice i  INNER JOIN C_InvoicePaySchedule ips "
                                + " ON (i.C_Invoice_ID = ips.C_Invoice_ID)  WHERE i.IsPayScheduleValid='Y' "
                                + " AND ips.IsValid = 'Y' AND ips.isactive = 'Y' "
                            + " AND i.C_Invoice_ID = " + Invoice_ID
                            + " AND ips.IsHoldPayment = 'N'"
                            + "  AND ips.C_InvoicePaySchedule_ID NOT IN"
                            + "(SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE c_payment_id IN"
                            + "(SELECT NVL(c_payment_id,0) FROM C_InvoicePaySchedule)  union "
                            + " SELECT NVL(C_InvoicePaySchedule_id,0) FROM C_InvoicePaySchedule WHERE c_cashline_id IN"
                            + "(SELECT NVL(c_cashline_id,0) FROM C_InvoicePaySchedule )) "
                            + " ORDER BY ips.duedate ASC) t WHERE rownum=1";
            }
            try
            {
                _ds = DB.ExecuteDataset(_Sql, null, null);
                if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = new Dictionary<string, object>();
                    retValue["C_InvoicePaySchedule_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["c_invoicepayschedule_id"]);
                    retValue["dueAmount"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["dueamt"]);
                    retValue["IsReturnTrx"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["IsReturnTrx"]);

                    //Added 2 new fields to get VA009_PaymentMethod_ID and VA009_PaymentBaseType To Set the corrosponding value on Payment Window..
                    if (Env.IsModuleInstalled("VA009_"))
                    {
                        retValue["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                        retValue["VA009_PaymentBaseType"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["VA009_PaymentBaseType"]);
                    }
                }
            }
            catch (Exception e)
            {
                if (_ds != null)
                {
                    _ds.Dispose();
                }
            }
            return retValue;
        }
        /// <summary>
        /// Get Invoice Details
        /// </summary>        
        /// <param name="fields">Parameters</param>
        /// <returns>Dictionaty, Invoice Data</returns>
        public Dictionary<string, object> GetInvoiceDetails(string fields)
        {
            DataSet _ds = null;
            string[] paramValue = fields.Split(',');
            Dictionary<string, object> retValue = null;
            string sql = "SELECT C_BPartner_ID, C_Currency_ID, C_ConversionType_ID, invoiceOpen(C_Invoice_ID, " + Util.GetValueOfInt(paramValue[3]) + @") as invoiceOpen, IsSOTrx, 
            paymentTermDiscount(invoiceOpen(C_Invoice_ID, 0),C_Currency_ID,C_PaymentTerm_ID,DateInvoiced, " + paramValue[1] + "," + paramValue[2]
            + " ) as paymentTermDiscount, C_DocTypeTarget_ID,C_BPartner_Location_ID FROM C_Invoice WHERE C_Invoice_ID=" + Util.GetValueOfInt(paramValue[0]);
            try
            {
                _ds = DB.ExecuteDataset(sql, null, null);
                if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = new Dictionary<string, object>();
                    retValue["C_BPartner_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_BPartner_ID"]);
                    retValue["C_Currency_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_Currency_ID"]);

                    // JID_1208: System should set Currency Type that is defined on Invoice.
                    retValue["C_ConversionType_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_ConversionType_ID"]);
                    retValue["invoiceOpen"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["invoiceOpen"]);
                    retValue["IsSOTrx"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["IsSOTrx"]);
                    retValue["paymentTermDiscount"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["paymentTermDiscount"]);
                    retValue["docbaseType"] = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocBaseType FROM C_DocType WHERE C_DocType_ID = "
                        + Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_DocTypeTarget_ID"]), null, null));
                    retValue["C_BPartner_Location_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_BPartner_Location_ID"]); //Arpit
                }
            }
            catch (Exception e)
            {
                if (_ds != null)
                {
                    _ds.Dispose();
                }
            }

            return retValue;

        }

        // Added by Bharat on 24 May 2017
        /// <summary>
        /// Get Invoice Open Amount
        /// </summary>        
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetInvoiceAmount(string fields)
        {
            DataSet _ds = null;
            string[] paramValue = fields.Split(',');
            int C_Invoice_ID = Util.GetValueOfInt(paramValue[0]);
            int C_BankAccount_ID = Util.GetValueOfInt(paramValue[1]);
            DateTime? payDate = Util.GetValueOfDateTime(paramValue[2]);
            Dictionary<string, object> retValue = null;
            string sql = "SELECT currencyConvert(invoiceOpen(i.C_Invoice_ID, 0), i.C_Currency_ID,"
                + "ba.C_Currency_ID, i.DateInvoiced, i.C_ConversionType_ID, i.AD_Client_ID, i.AD_Org_ID) as OpenAmt,"
            + " paymentTermDiscount(i.GrandTotal,i.C_Currency_ID,i.C_PaymentTerm_ID,i.DateInvoiced,'" + payDate + "') As DiscountAmt, i.IsSOTrx "
            + "FROM C_Invoice_v i, C_BankAccount ba "
            + "WHERE i.C_Invoice_ID = " + C_Invoice_ID + " AND ba.C_BankAccount_ID = " + C_BankAccount_ID;
            try
            {
                _ds = DB.ExecuteDataset(sql, null, null);
                if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = new Dictionary<string, object>();
                    retValue["OpenAmt"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["OpenAmt"]);
                    retValue["DiscountAmt"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["DiscountAmt"]);
                    retValue["IsSOTrx"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["IsSOTrx"]);
                }
            }
            catch (Exception e)
            {
                if (_ds != null)
                {
                    _ds.Dispose();
                }
            }
            return retValue;
        }

        /// <summary>
        /// Get Price of Product
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="param">List of Parameters</param>
        /// <returns>List of Price Data</returns>
        public Dictionary<String, Object> GetPrices(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            Dictionary<String, Object> retDic = new Dictionary<String, Object>();

            //Assign parameter value
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            //int _priceListVersion_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _c_invoice_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _m_AttributeSetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _c_Uom_Id = Util.GetValueOfInt(paramValue[3].ToString());
            int _ad_Client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            int _c_BPartner_Id = Util.GetValueOfInt(paramValue[5].ToString());
            //int _m_DiscountSchema_ID = Util.GetValueOfInt(paramValue[5].ToString());
            //decimal _flatDiscount = Util.GetValueOfInt(paramValue[6].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[6].ToString());
            //End Assign parameter value

            StringBuilder sql = new StringBuilder();
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            int _m_PriceList_ID = 0;
            int _priceListVersion_Id = 0;
            int _m_DiscountSchema_ID = 0;
            decimal _flatDiscount = 0;
            int countEd011 = 0;
            int countVAPRC = 0;

            countEd011 = Env.IsModuleInstalled("ED011_") ? 1 : 0;
            retDic["countEd011"] = countEd011.ToString();

            countVAPRC = Env.IsModuleInstalled("VAPRC_") ? 1 : 0;
            retDic["countVAPRC"] = countVAPRC.ToString();

            retDic["IsDiscountApplied"] = "N";

            if (countEd011 > 0)
            {
                MOrderLineModel objOrd = new MOrderLineModel();
                bool isSOtrx = false;
                DataSet dsInvoice = DB.ExecuteDataset("SELECT M_PriceList_ID, IsSOTrx FROM C_Invoice WHERE C_Invoice_ID = " + _c_invoice_Id, null, null);
                if (dsInvoice != null && dsInvoice.Tables.Count > 0 && dsInvoice.Tables[0].Rows.Count > 0)
                {
                    _m_PriceList_ID = Util.GetValueOfInt(dsInvoice.Tables[0].Rows[0]["M_PriceList_ID"]);
                    isSOtrx = Util.GetValueOfString(dsInvoice.Tables[0].Rows[0]["IsSOTrx"]).Equals("Y");
                }

                /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
                StringBuilder sbparams = new StringBuilder();
                sbparams.Append(Util.GetValueOfInt(_m_PriceList_ID));
                sbparams.Append(",").Append(Util.GetValueOfInt(_c_invoice_Id));
                sbparams.Append(",").Append(Util.GetValueOfInt(_m_Product_Id));
                sbparams.Append(",").Append(Util.GetValueOfInt(_c_Uom_Id));
                sbparams.Append(",").Append(Util.GetValueOfInt(_m_AttributeSetInstance_Id));

                MPriceListVersionModel objPLV = new MPriceListVersionModel();
                _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID(ctx, sbparams.ToString(), ScreenType.Invoice);


                MBPartnerModel objBPartner = new MBPartnerModel();
                Dictionary<String, String> bpartner1 = objBPartner.GetBPartner(ctx, _c_BPartner_Id.ToString());
                if (isSOtrx)
                {
                    _m_DiscountSchema_ID = Util.GetValueOfInt(bpartner1["M_DiscountSchema_ID"]);
                }
                else
                {
                    _m_DiscountSchema_ID = Util.GetValueOfInt(bpartner1["PO_DiscountSchema_ID"]);
                }
                _flatDiscount = Util.GetValueOfInt(bpartner1["FlatDiscount"]);

                if (_m_AttributeSetInstance_Id > 0)
                {
                    sql.Append("SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                     + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                     + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id
                                     + "  AND C_UOM_ID=" + _c_Uom_Id);
                }
                else
                {
                    sql.Append("SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                   + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                   + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                   + "  AND C_UOM_ID=" + _c_Uom_Id);
                }
                int countrecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
                if (countrecord > 0)
                {
                    // Selected UOM Price Exist
                    sql.Clear();
                    if (_m_AttributeSetInstance_Id > 0)
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                    + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                    + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id
                                    + "  AND C_UOM_ID=" + _c_Uom_Id);
                    }
                    else
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                  + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                  + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                  + "  AND C_UOM_ID=" + _c_Uom_Id);
                    }
                    DataSet ds = DB.ExecuteDataset(sql.ToString());
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = objOrd.FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                            _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                            // check is Discount Applied or not
                            if (Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]) != 0 &&
                                Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]) != PriceEntered)
                            {
                                retDic["IsDiscountApplied"] = "Y";
                            }
                            PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                            PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                        }
                    }
                }
                else //if (_m_AttributeSetInstance_Id > 0 && countrecord == 0)
                {
                    sql.Clear();
                    sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                + "  AND C_UOM_ID=" + _c_Uom_Id);
                    DataSet ds1 = DB.ExecuteDataset(sql.ToString());
                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        //Flat Discount
                        PriceEntered = objOrd.FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]),
                       _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                        // check is Discount Applied or not
                        if (Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]) != 0 &&
                            Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]) != PriceEntered)
                        {
                            retDic["IsDiscountApplied"] = "Y";
                        }
                        PriceList = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceList"]);
                        PriceLimit = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceLimit"]);
                    }
                    else
                    {
                        // get uom from product
                        var paramStr = _m_Product_Id.ToString();
                        MProductModel objProduct = new MProductModel();
                        var prodC_UOM_ID = objProduct.GetC_UOM_ID(ctx, paramStr);
                        sql.Clear();
                        if (_m_AttributeSetInstance_Id > 0)
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                        + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                        + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id
                                        + "  AND C_UOM_ID=" + prodC_UOM_ID);
                        }
                        else
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                      + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                      + "  AND C_UOM_ID=" + prodC_UOM_ID);
                        }
                        DataSet ds = DB.ExecuteDataset(sql.ToString());
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                //Flat Discount
                                PriceEntered = objOrd.FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                                _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                                // check is Discount Applied or not
                                if (Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]) != 0 &&
                                    Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]) != PriceEntered)
                                {
                                    retDic["IsDiscountApplied"] = "Y";
                                }
                                PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                                PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                            }
                            else if (_m_AttributeSetInstance_Id > 0)
                            {
                                sql.Clear();
                                sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                            + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                            + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                            + "  AND C_UOM_ID=" + prodC_UOM_ID);
                                DataSet ds2 = DB.ExecuteDataset(sql.ToString());
                                if (ds2 != null && ds.Tables.Count > 0)
                                {
                                    if (ds2.Tables[0].Rows.Count > 0)
                                    {
                                        //Flat Discount
                                        PriceEntered = objOrd.FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                                    _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                                        // check is Discount Applied or not
                                        if (Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]) != 0 &&
                                            Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]) != PriceEntered)
                                        {
                                            retDic["IsDiscountApplied"] = "Y";
                                        }
                                        PriceList = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceList"]);
                                        PriceLimit = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceLimit"]);
                                    }
                                }
                            }
                        }
                        sql.Clear();
                        sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                                                   " AND con.M_Product_ID = " + _m_Product_Id +
                                                   " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + _c_Uom_Id);
                        var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                        if (rate == 0)
                        {
                            sql.Clear();
                            sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                                  " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + _c_Uom_Id);
                            rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                        }
                        if (rate == 0)
                        {
                            rate = 1;
                        }
                        PriceEntered = PriceEntered * rate;
                        PriceList = PriceList * rate;
                        PriceLimit = PriceLimit * rate;
                    }
                }
            }

            retDic["DiscountSchema"] = _m_DiscountSchema_ID > 0 ? "Y" : "N";
            retDic["PriceEntered"] = PriceEntered;
            retDic["PriceList"] = PriceList;
            retDic["PriceLimit"] = PriceLimit;
            return retDic;
        }

        /// <summary>
        /// Getting the percision values
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns>get Percision value</returns>
        public int GetPrecision(Ctx ctx, string fields)
        {
            string sql = "SELECT CC.StdPrecision FROM C_Invoice CI INNER JOIN C_Currency CC on CC.C_Currency_Id = CI.C_Currency_Id where CI.C_Invoice_ID= " + Util.GetValueOfInt(fields);
            int stdPrecision = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            return stdPrecision;

        }

        /// <summary>
        /// Get Invoice Details
        /// </summary>
        /// <param name="fields">Invoice Line ID</param>
        /// <returns>InvoiceLineDetail</returns>
        /// <writer>VIS_0045</writer>
        public Dictionary<string, object> GetInvoiceLineDetail(string fields)
        {
            DataSet _ds = null;
            string[] paramValue = fields.Split(',');
            int C_InvoiceLine_ID = Util.GetValueOfInt(paramValue[0]);
            Dictionary<string, object> retValue = null;
            string sql = @"SELECT M_Product_ID, M_AttributeSetInstance_ID, C_UOM_ID , QtyEntered, QtyInvoiced 
                           FROM C_InvoiceLine WHERE C_InvoiceLine_ID = " + C_InvoiceLine_ID;
            _ds = DB.ExecuteDataset(sql, null, null);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                retValue = new Dictionary<string, object>();
                retValue["M_Product_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["M_Product_ID"]);
                retValue["M_AttributeSetInstance_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["M_AttributeSetInstance_ID"]);
                retValue["C_UOM_ID"] = Util.GetValueOfInt(_ds.Tables[0].Rows[0]["C_UOM_ID"]);
                retValue["QtyEntered"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["QtyEntered"]);
                retValue["QtyInvoiced"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["QtyInvoiced"]);
            }
            return retValue;
        }
    }
}