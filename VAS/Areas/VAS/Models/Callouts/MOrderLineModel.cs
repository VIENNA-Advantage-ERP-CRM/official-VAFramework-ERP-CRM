using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class MVABOrderLineModel
    {
        /// <summary>
        /// GetOrderLine
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetOrderLine(Ctx ctx, string param)
        {

            string[] paramValue = param.Split(',');

            Dictionary<String, String> retDic = new Dictionary<string, string>();

            //Assign parameter value
            int id;
            id = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value

            MVABOrderLine orderline = new MVABOrderLine(ctx, id, null);

            retDic["VAB_TaxRate_ID"] = Util.GetValueOfString(orderline.GetVAB_TaxRate_ID());
            retDic["PriceList"] = Util.GetValueOfString(orderline.GetPriceList());
            retDic["PriceLimit"] = Util.GetValueOfString(orderline.GetPriceLimit());
            retDic["PriceActual"] = Util.GetValueOfString(orderline.GetPriceActual());
            retDic["PriceEntered"] = Util.GetValueOfString(orderline.GetPriceEntered());
            retDic["VAB_Currency_ID"] = Util.GetValueOfString(orderline.GetVAB_Currency_ID());
            retDic["Discount"] = Util.GetValueOfString(orderline.GetDiscount());
            retDic["Discount"] = Util.GetValueOfString(orderline.GetDiscount());
            retDic["VAM_Product_ID"] = Util.GetValueOfString(orderline.GetVAM_Product_ID());

            // when order line contains charge, it will be selected on Shipment Line on selection of Order Line
            retDic["VAB_Charge_ID"] = Util.GetValueOfString(orderline.GetVAB_Charge_ID());
            retDic["Qty"] = Util.GetValueOfString(orderline.GetQtyEntered());
            retDic["VAB_UOM_ID"] = Util.GetValueOfString(orderline.GetVAB_UOM_ID());
            retDic["VAB_BusinessPartner_ID"] = Util.GetValueOfString(orderline.GetVAB_BusinessPartner_ID());
            retDic["PlannedHours"] = Util.GetValueOfString(orderline.GetQtyOrdered());
            retDic["VAM_PFeature_SetInstance_ID"] = Util.GetValueOfString(orderline.GetVAM_PFeature_SetInstance_ID());
            retDic["QtyOrdered"] = Util.GetValueOfString(orderline.GetQtyOrdered());
            retDic["QtyDelivered"] = Util.GetValueOfString(orderline.GetQtyDelivered());
            retDic["QtyEntered"] = Util.GetValueOfString(orderline.GetQtyEntered());
            retDic["VAB_BillingCode_ID"] = Util.GetValueOfString(orderline.GetVAB_BillingCode_ID());
            retDic["VAB_Promotion_ID"] = Util.GetValueOfString(orderline.GetVAB_Promotion_ID());
            retDic["VAB_Project_ID"] = Util.GetValueOfString(orderline.GetVAB_Project_ID());
            retDic["VAB_ProjectStage_ID"] = Util.GetValueOfString(orderline.GetVAB_ProjectStage_ID());
            retDic["VAB_ProjectJob_ID"] = Util.GetValueOfString(orderline.GetVAB_ProjectJob_ID());
            retDic["VAF_OrgTrx_ID"] = Util.GetValueOfString(orderline.GetVAF_OrgTrx_ID());
            retDic["User1_ID"] = Util.GetValueOfString(orderline.GetUser1_ID());
            retDic["User2_ID"] = Util.GetValueOfString(orderline.GetUser2_ID());
            retDic["IsReturnTrx"] = Util.GetValueOfString(orderline.GetParent().IsReturnTrx()).ToLower();
            retDic["Orig_InOutLine_ID"] = Util.GetValueOfString(orderline.GetOrig_InOutLine_ID());
            retDic["Orig_OrderLine_ID"] = Util.GetValueOfString(orderline.GetOrig_OrderLine_ID());
            retDic["GetID"] = Util.GetValueOfString(orderline.Get_ID());

            retDic["QtyReleased"] = Util.GetValueOfString(orderline.GetQtyReleased());
            retDic["IsDropShip"] = orderline.IsDropShip() ? "Y" : "N";

            return retDic;
        }
        /// <summary>
        /// Get Not Reserved
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal GetNotReserved(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int VAM_Warehouse_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int VAB_OrderLine_ID = Util.GetValueOfInt(paramValue[3].ToString());
            //End Assign parameter value
            return MVABOrderLine.GetNotReserved(ctx, VAM_Warehouse_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAB_OrderLine_ID);

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
            int VAB_TaxRate_ID = 0;
            int VAB_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int VAB_Charge_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int taxCategory = 0;
            string sql = "";
            if ((VAM_Product_ID == 0 && VAB_Charge_ID == 0) || VAB_Order_ID == 0)
            {
                return VAB_TaxRate_ID;
            }
            DataSet dsLoc = null;
            MVABOrder inv = new MVABOrder(ctx, VAB_Order_ID, null);
            MVABBusinessPartner bp = new MVABBusinessPartner(ctx, inv.GetVAB_BusinessPartner_ID(), null);
            if (bp.IsTaxExempt())
            {
                VAB_TaxRate_ID = GetExemptTax(ctx, inv.GetVAF_Org_ID());
                return VAB_TaxRate_ID;
            }
            if (VAM_Product_ID > 0)
            {
                MProduct prod = new MProduct(ctx, VAM_Product_ID, null);
                taxCategory = Util.GetValueOfInt(prod.GetVAB_TaxCategory_ID());
            }
            if (VAB_Charge_ID > 0)
            {
                MVABCharge chrg = new MVABCharge(ctx, VAB_Charge_ID, null);
                taxCategory = Util.GetValueOfInt(chrg.GetVAB_TaxCategory_ID());
            }
            if (taxCategory > 0)
            {
                MTaxCategory taxCat = new MTaxCategory(ctx, taxCategory, null);
                int Country_ID = 0, Region_ID = 0, orgCountry = 0, orgRegion = 0, taxRegion = 0;
                string Postal = "", orgPostal = "";

                if (taxCat.GetVATAX_Location() == "I")
                {
                    sql = @"SELECT loc.VAB_Country_ID,loc.VAB_RegionState_ID,loc.Postal FROM VAB_Address loc INNER JOIN VAB_BPart_Location bpl ON loc.VAB_Address_ID = bpl.VAB_Address_ID WHERE bpl.VAB_BPart_Location_ID ="
                        + inv.GetBill_Location_ID() + " AND bpl.IsActive = 'Y'";
                }
                else
                {
                    sql = @"SELECT loc.VAB_Country_ID,loc.VAB_RegionState_ID,loc.Postal FROM VAB_Address loc INNER JOIN VAB_BPart_Location bpl ON loc.VAB_Address_ID = bpl.VAB_Address_ID WHERE bpl.VAB_BPart_Location_ID ="
                        + inv.GetVAB_BPart_Location_ID() + " AND bpl.IsActive = 'Y'";
                }
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
                sql = @"SELECT loc.VAB_Country_ID,loc.VAB_RegionState_ID,loc.Postal FROM VAB_Address loc LEFT JOIN VAF_OrgDetail org ON loc.VAB_Address_ID = org.VAB_Address_ID WHERE org.VAF_Org_ID ="
                        + inv.GetVAF_Org_ID() + " AND org.IsActive = 'Y'";
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
                    sql = @"SELECT VATAX_TaxType_ID FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() +
                                   " AND IsActive = 'Y'  AND VAB_BPart_Location_ID = " + inv.GetVAB_BPart_Location_ID();
                    int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (taxType == 0)
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() + " AND IsActive = 'Y'";
                        taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (taxType == 0)
                        {
                            if (taxCat.GetVATAX_Preference2() == "L")
                            {
                                VAB_TaxRate_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (VAB_TaxRate_ID > 0)
                                {
                                    return VAB_TaxRate_ID;
                                }
                                else
                                {
                                    if (taxCat.GetVATAX_Preference3() == "R")
                                    {
                                        if (Country_ID > 0)
                                        {
                                            dsLoc = null;
                                            sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE VAB_TaxCategory_ID = " + taxCategory +
                                                " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND VAB_Country_ID = " + Country_ID;
                                            dsLoc = DB.ExecuteDataset(sql, null, null);
                                            if (dsLoc != null)
                                            {
                                                if (dsLoc.Tables[0].Rows.Count > 0)
                                                {

                                                }
                                                else
                                                {
                                                    VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                    if (VAB_TaxRate_ID > 0)
                                                    {
                                                        return VAB_TaxRate_ID;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                if (VAB_TaxRate_ID > 0)
                                                {
                                                    return VAB_TaxRate_ID;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                            if (VAB_TaxRate_ID > 0)
                                            {
                                                return VAB_TaxRate_ID;
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
                                    sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE VAB_TaxCategory_ID = " + taxCategory +
                                        " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND VAB_Country_ID = " + Country_ID;
                                    dsLoc = DB.ExecuteDataset(sql, null, null);
                                    if (dsLoc != null)
                                    {
                                        if (dsLoc.Tables[0].Rows.Count > 0)
                                        {

                                        }
                                        else
                                        {
                                            VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                            if (VAB_TaxRate_ID > 0)
                                            {
                                                return VAB_TaxRate_ID;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                        if (VAB_TaxRate_ID > 0)
                                        {
                                            return VAB_TaxRate_ID;
                                        }
                                    }
                                }
                                else
                                {
                                    VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (VAB_TaxRate_ID > 0)
                                    {
                                        return VAB_TaxRate_ID;
                                    }
                                }
                                if (taxCat.GetVATAX_Preference3() == "L")
                                {
                                    VAB_TaxRate_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (VAB_TaxRate_ID > 0)
                                    {
                                        return VAB_TaxRate_ID;
                                    }
                                }
                            }
                        }
                    }
                    if (taxType > 0)
                    {
                        sql = "SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID  WHERE tcr.VAB_TaxCategory_ID = " + taxCategory +
                            " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                        VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (VAB_TaxRate_ID > 0)
                        {
                            return VAB_TaxRate_ID;
                        }
                    }
                }
                // if Tax Preference 1 is Location
                else if (taxCat.GetVATAX_Preference1() == "L")
                {
                    VAB_TaxRate_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                    if (VAB_TaxRate_ID > 0)
                    {
                        return VAB_TaxRate_ID;
                    }
                    else
                    {
                        if (taxCat.GetVATAX_Preference2() == "T")
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() +
                                           " AND IsActive = 'Y'  AND VAB_BPart_Location_ID = " + inv.GetVAB_BPart_Location_ID();
                            int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (taxType == 0)
                            {
                                sql = @"SELECT VATAX_TaxType_ID FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() + " AND IsActive = 'Y'";
                                taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (taxType == 0)
                                {
                                    if (taxCat.GetVATAX_Preference3() == "R")
                                    {
                                        if (Country_ID > 0)
                                        {
                                            dsLoc = null;
                                            sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE VAB_TaxCategory_ID = " + taxCategory +
                                                " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND VAB_Country_ID = " + Country_ID;
                                            dsLoc = DB.ExecuteDataset(sql, null, null);
                                            if (dsLoc != null)
                                            {
                                                if (dsLoc.Tables[0].Rows.Count > 0)
                                                {

                                                }
                                                else
                                                {
                                                    VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                    if (VAB_TaxRate_ID > 0)
                                                    {
                                                        return VAB_TaxRate_ID;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                                if (VAB_TaxRate_ID > 0)
                                                {
                                                    return VAB_TaxRate_ID;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                            if (VAB_TaxRate_ID > 0)
                                            {
                                                return VAB_TaxRate_ID;
                                            }
                                        }
                                    }
                                }
                            }
                            if (taxType > 0)
                            {
                                sql = "SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = " + taxCategory +
                                    " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (VAB_TaxRate_ID > 0)
                                {
                                    return VAB_TaxRate_ID;
                                }
                            }
                        }
                        else if (taxCat.GetVATAX_Preference2() == "R")
                        {
                            if (Country_ID > 0)
                            {
                                dsLoc = null;
                                sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE VAB_TaxCategory_ID = " + taxCategory +
                                    " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND VAB_Country_ID = " + Country_ID;
                                dsLoc = DB.ExecuteDataset(sql, null, null);
                                if (dsLoc != null)
                                {
                                    if (dsLoc.Tables[0].Rows.Count > 0)
                                    {

                                    }
                                    else
                                    {
                                        VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                        if (VAB_TaxRate_ID > 0)
                                        {
                                            return VAB_TaxRate_ID;
                                        }
                                    }
                                }
                                else
                                {
                                    VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (VAB_TaxRate_ID > 0)
                                    {
                                        return VAB_TaxRate_ID;
                                    }
                                }
                            }
                            else
                            {
                                VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (VAB_TaxRate_ID > 0)
                                {
                                    return VAB_TaxRate_ID;
                                }
                            }
                            if (taxCat.GetVATAX_Preference3() == "T")
                            {
                                sql = @"SELECT VATAX_TaxType_ID FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() +
                                               " AND IsActive = 'Y'  AND VAB_BPart_Location_ID = " + inv.GetVAB_BPart_Location_ID();
                                int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (taxType == 0)
                                {
                                    sql = @"SELECT VATAX_TaxType_ID FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() + " AND IsActive = 'Y'";
                                    taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                }
                                if (taxType > 0)
                                {
                                    sql = "SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = " + taxCategory +
                                        " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                    VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                    if (VAB_TaxRate_ID > 0)
                                    {
                                        return VAB_TaxRate_ID;
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
                        sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE VAB_TaxCategory_ID = " + taxCategory +
                            " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND VAB_Country_ID = " + Country_ID;
                        dsLoc = DB.ExecuteDataset(sql, null, null);
                        if (dsLoc != null)
                        {
                            if (dsLoc.Tables[0].Rows.Count > 0)
                            {

                            }
                            else
                            {
                                VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (VAB_TaxRate_ID > 0)
                                {
                                    return VAB_TaxRate_ID;
                                }
                            }
                        }
                        else
                        {
                            VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                            if (VAB_TaxRate_ID > 0)
                            {
                                return VAB_TaxRate_ID;
                            }
                        }
                    }
                    else
                    {
                        VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                        if (VAB_TaxRate_ID > 0)
                        {
                            return VAB_TaxRate_ID;
                        }
                    }
                    if (taxCat.GetVATAX_Preference2() == "T")
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() +
                                       " AND IsActive = 'Y'  AND VAB_BPart_Location_ID = " + inv.GetVAB_BPart_Location_ID();
                        int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (taxType == 0)
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() + " AND IsActive = 'Y'";
                            taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (taxType == 0)
                            {
                                if (taxCat.GetVATAX_Preference3() == "L")
                                {
                                    VAB_TaxRate_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (VAB_TaxRate_ID > 0)
                                    {
                                        return VAB_TaxRate_ID;
                                    }
                                }
                            }
                        }
                        if (taxType > 0)
                        {
                            sql = "SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = " + taxCategory +
                                " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                            VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (VAB_TaxRate_ID > 0)
                            {
                                return VAB_TaxRate_ID;
                            }
                        }
                    }
                    else if (taxCat.GetVATAX_Preference2() == "L")
                    {
                        VAB_TaxRate_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                        if (VAB_TaxRate_ID > 0)
                        {
                            return VAB_TaxRate_ID;
                        }
                        if (taxCat.GetVATAX_Preference3() == "T")
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() +
                                           " AND IsActive = 'Y'  AND VAB_BPart_Location_ID = " + inv.GetVAB_BPart_Location_ID();
                            int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (taxType == 0)
                            {
                                sql = @"SELECT VATAX_TaxType_ID FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() + " AND IsActive = 'Y'";
                                taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            }
                            if (taxType > 0)
                            {
                                sql = "SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = " + taxCategory +
                                    " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                                if (VAB_TaxRate_ID > 0)
                                {
                                    return VAB_TaxRate_ID;
                                }
                            }
                        }
                    }
                }
                if (taxCat.GetVATAX_Preference1() == "R" || taxCat.GetVATAX_Preference2() == "R" || taxCat.GetVATAX_Preference3() == "R")
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxRegion tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.IsDefault = 'Y' AND tcr.IsActive = 'Y' 
                    AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "') ORDER BY tcr.Updated";
                    VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (VAB_TaxRate_ID > 0)
                    {
                        return VAB_TaxRate_ID;
                    }
                }
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VAB_TaxCategory tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID =" + taxCategory + " AND tcr.IsActive = 'Y'";
                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                return VAB_TaxRate_ID;
            }
            return VAB_TaxRate_ID;
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
            int VAB_TaxRate_ID = 0;
            int VAB_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int VAB_Charge_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int taxCategory = 0;
            string sql = "";
            if ((VAM_Product_ID == 0 && VAB_Charge_ID == 0) || VAB_Order_ID == 0)
            {
                return VAB_TaxRate_ID;
            }
            DataSet dsLoc = null;
            MVABOrder inv = new MVABOrder(ctx, VAB_Order_ID, null);
            MVABBusinessPartner bp = new MVABBusinessPartner(ctx, inv.GetVAB_BusinessPartner_ID(), null);
            if (bp.IsTaxExempt())
            {
                VAB_TaxRate_ID = GetExemptTax(ctx, inv.GetVAF_Org_ID());
                return VAB_TaxRate_ID;
            }
            if (VAM_Product_ID > 0)
            {
                MProduct prod = new MProduct(ctx, VAM_Product_ID, null);
                taxCategory = Util.GetValueOfInt(prod.GetVAB_TaxCategory_ID());
            }
            if (VAB_Charge_ID > 0)
            {
                MVABCharge chrg = new MVABCharge(ctx, VAB_Charge_ID, null);
                taxCategory = Util.GetValueOfInt(chrg.GetVAB_TaxCategory_ID());
            }
            if (taxCategory > 0)
            {
                MTaxCategory taxCat = new MTaxCategory(ctx, taxCategory, null);
                int Country_ID = 0, Region_ID = 0, orgCountry = 0, orgRegion = 0, taxRegion = 0;
                string Postal = "", orgPostal = "";

                if (taxCat.GetVATAX_Location() == "I")
                {
                    sql = @"SELECT loc.VAB_Country_ID,loc.VAB_RegionState_ID,loc.Postal FROM VAB_Address loc INNER JOIN VAB_BPart_Location bpl ON loc.VAB_Address_ID = bpl.VAB_Address_ID WHERE bpl.VAB_BPart_Location_ID ="
                        + inv.GetBill_Location_ID() + " AND bpl.IsActive = 'Y'";
                }
                else
                {
                    sql = @"SELECT loc.VAB_Country_ID,loc.VAB_RegionState_ID,loc.Postal FROM VAB_Address loc INNER JOIN VAB_BPart_Location bpl ON loc.VAB_Address_ID = bpl.VAB_Address_ID WHERE bpl.VAB_BPart_Location_ID ="
                        + inv.GetVAB_BPart_Location_ID() + " AND bpl.IsActive = 'Y'";
                }
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
                sql = @"SELECT loc.VAB_Country_ID,loc.VAB_RegionState_ID,loc.Postal FROM VAB_Address loc LEFT JOIN VAF_OrgDetail org ON loc.VAB_Address_ID = org.VAB_Address_ID WHERE org.VAF_Org_ID ="
                        + inv.GetVAF_Org_ID() + " AND org.IsActive = 'Y'";
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

                    // if Tax Preference is Tax Class

                    if (pref == "T")
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() +
                                       " AND IsActive = 'Y'  AND VAB_BPart_Location_ID = " + inv.GetVAB_BPart_Location_ID();
                        int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (taxType == 0)
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID =" + inv.GetVAB_BusinessPartner_ID() + " AND IsActive = 'Y'";
                            taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        }
                        if (taxType > 0)
                        {
                            sql = "SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID  WHERE tcr.VAB_TaxCategory_ID = " + taxCategory +
                                " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxBase = 'T' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                            VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (VAB_TaxRate_ID > 0)
                            {
                                return VAB_TaxRate_ID;
                            }
                        }
                    }
                    // if Tax Preference is Location
                    else if (pref == "L")
                    {
                        VAB_TaxRate_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                        if (VAB_TaxRate_ID > 0)
                        {
                            return VAB_TaxRate_ID;
                        }
                    }
                    // if Tax Preference is Tax Region
                    else if (pref == "R")
                    {
                        if (Country_ID > 0)
                        {
                            dsLoc = null;
                            sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE VAB_TaxCategory_ID = " + taxCategory +
                                " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND VAB_Country_ID = " + Country_ID;
                            dsLoc = DB.ExecuteDataset(sql, null, null);
                            if (dsLoc != null)
                            {
                                if (dsLoc.Tables[0].Rows.Count > 0)
                                {

                                }
                                else
                                {
                                    VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (VAB_TaxRate_ID > 0)
                                    {
                                        return VAB_TaxRate_ID;
                                    }
                                }
                            }
                            else
                            {
                                VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (VAB_TaxRate_ID > 0)
                                {
                                    return VAB_TaxRate_ID;
                                }
                            }
                        }
                        else
                        {
                            VAB_TaxRate_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                            if (VAB_TaxRate_ID > 0)
                            {
                                return VAB_TaxRate_ID;
                            }
                        }
                    }

                    // if Tax Preference is Document Type
                    else if (pref == "D")
                    {
                        sql = @"SELECT VATAX_TaxType_ID FROM VAB_DocTypes WHERE VAB_DocTypes_ID = " + inv.GetVAB_DocTypesTarget_ID();
                        int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        if (taxType > 0)
                        {
                            sql = "SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID  WHERE tcr.VAB_TaxCategory_ID = " + taxCategory +
                                " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxBase = 'T' AND tcr.VATAX_TaxType_ID = " + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                            VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                            if (VAB_TaxRate_ID > 0)
                            {
                                return VAB_TaxRate_ID;
                            }
                        }
                    }
                }
                if (taxCat.GetVATAX_Preference1() == "R" || taxCat.GetVATAX_Preference2() == "R" || taxCat.GetVATAX_Preference3() == "R")
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxRegion tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.IsDefault = 'Y' AND tcr.IsActive = 'Y' 
                    AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "') ORDER BY tcr.Updated";
                    VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (VAB_TaxRate_ID > 0)
                    {
                        return VAB_TaxRate_ID;
                    }
                }
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VAB_TaxCategory tcr WHERE tcr.VAB_TaxCategory_ID = " + taxCategory + " AND tcr.IsActive = 'Y'";
                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                return VAB_TaxRate_ID;
            }
            return VAB_TaxRate_ID;
        }

        // Return Exempted Tax Fro the Organization
        VLogger log = VLogger.GetVLogger("Tax");
        private int GetExemptTax(Ctx ctx, int VAF_Org_ID)
        {
            int VAB_TaxRate_ID = 0;
            String sql = "SELECT t.VAB_TaxRate_ID "
                + "FROM VAB_TaxRate t"
                + " INNER JOIN VAF_Org o ON (t.VAF_Client_ID=o.VAF_Client_ID) "
                + "WHERE t.IsActive='Y' AND t.IsTaxExempt='Y' AND o.VAF_Org_ID= " + VAF_Org_ID
                + "ORDER BY t.Rate DESC";
            bool found = false;
            try
            {
                DataSet pstmt = DB.ExecuteDataset(sql, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    VAB_TaxRate_ID = Util.GetValueOfInt(dr[0]);
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

        private int GetTaxFromLocation(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal)
        {
            string sql = "";
            int VAB_TaxRate_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID =" + taxCategory +
                    " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.VAB_Country_ID = " + Country_ID + " AND NVL(tcr.VAB_RegionState_ID,0) = " + Region_ID +
                    " AND tcr.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID =" + taxCategory +
                    " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.VAB_Country_ID = " + Country_ID + " AND NVL(tcr.VAB_RegionState_ID,0) = " + Region_ID +
                    " AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= '" + Postal + "' AND tcr.postal_to >= '" + Postal + "' THEN 1 ELSE 2" +
                    " END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            if (VAB_TaxRate_ID > 0)
            {
                return VAB_TaxRate_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID =" + taxCategory +
                        " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.VAB_Country_ID = " + Country_ID + " AND tcr.VAB_RegionState_ID IS NULL AND tcr.Postal IS NULL AND tx.SOPOType IN ('B','"
                        + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID =" + taxCategory +
                        " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.VAB_Country_ID = " + Country_ID + " AND tcr.VAB_RegionState_ID IS NULL AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= '" + Postal +
                        "' AND tcr.postal_to >= '" + Postal + "' THEN 1 ELSE 2" + " END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                        + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (VAB_TaxRate_ID > 0)
                {
                    return VAB_TaxRate_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID =" + taxCategory +
                            " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.VAB_Country_ID IS NULL " + " AND tcr.VAB_RegionState_ID IS NULL AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= '"
                            + Postal + "' AND tcr.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                            + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }
                    if (VAB_TaxRate_ID > 0)
                    {
                        return VAB_TaxRate_ID;
                    }
                }
            }
            return VAB_TaxRate_ID;
        }

        private int GetTaxFromRegion(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal)
        {
            string sql = "";
            int VAB_TaxRate_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID + " AND NVL(trl.VAB_RegionState_ID,0) = " + Region_ID +
                " AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID + " AND NVL(trl.VAB_RegionState_ID,0) = " + Region_ID +
                " AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '" + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '"
                + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            if (VAB_TaxRate_ID > 0)
            {
                return VAB_TaxRate_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID + " AND trl.VAB_RegionState_ID IS NULL AND trl.Postal IS NULL AND tx.SOPOType IN ('B','"
                    + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID + " AND trl.VAB_RegionState_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                    + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (VAB_TaxRate_ID > 0)
                {
                    return VAB_TaxRate_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                        + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID IS NULL AND trl.VAB_RegionState_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                        + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                        if (VAB_TaxRate_ID > 0)
                        {
                            return VAB_TaxRate_ID;
                        }
                    }
                }
            }
            return VAB_TaxRate_ID;
        }

        private int GetTaxFromRegion(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal, int taxRegion, int toCountry)
        {
            string sql = "";
            int VAB_TaxRate_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.VAB_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID +
                " AND NVL(trl.VAB_RegionState_ID,0) = " + Region_ID + " AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.VAB_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID +
                " AND NVL(trl.VAB_RegionState_ID,0) = " + Region_ID + " AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '" + Postal + "' AND trl.postal_to >= '" + Postal +
                "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            if (VAB_TaxRate_ID > 0)
            {
                return VAB_TaxRate_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.VAB_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID +
                    " AND trl.VAB_RegionState_ID IS NULL AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.VAB_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID = " + Country_ID + " AND trl.VAB_RegionState_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                    + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (VAB_TaxRate_ID > 0)
                {
                    return VAB_TaxRate_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.VAB_TaxRate_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN VAB_TaxRate tx ON tcr.VAB_TaxRate_ID = tx.VAB_TaxRate_ID WHERE tcr.VAB_TaxCategory_ID = "
                        + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.VAB_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.VAB_Country_ID IS NULL AND trl.VAB_RegionState_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= '"
                        + Postal + "' AND trl.postal_to >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (VAB_TaxRate_ID > 0)
                        {
                            return VAB_TaxRate_ID;
                        }
                    }
                }
            }
            return VAB_TaxRate_ID;
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
            int _VAM_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            //int _priceListVersion_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAB_Order_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAM_PFeature_SetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _VAB_UOM_Id = Util.GetValueOfInt(paramValue[3].ToString());
            int _vaf_client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            int _VAB_BusinessPartner_Id = Util.GetValueOfInt(paramValue[5].ToString());
            //int _VAM_DiscountCalculation_ID = Util.GetValueOfInt(paramValue[5].ToString());
            //decimal _flatDiscount = Util.GetValueOfInt(paramValue[6].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[6].ToString());
            //End Assign parameter value

            StringBuilder sql = new StringBuilder();
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            int _VAM_PriceList_ID = 0;
            int _priceListVersion_Id = 0;
            int _VAM_DiscountCalculation_ID = 0;
            decimal _flatDiscount = 0;
            int countEd011 = 0;
            int countVAPRC = 0;

            countEd011 = Env.IsModuleInstalled("ED011_") ? 1 : 0;
            retDic["countEd011"] = countEd011.ToString();

            countVAPRC = Env.IsModuleInstalled("VAPRC_") ? 1 : 0;
            retDic["countVAPRC"] = countVAPRC.ToString();

            if (countEd011 > 0)
            {
                MOrderModel objOrder = new MOrderModel();
                _VAM_PriceList_ID = objOrder.GetVAM_PriceList(ctx, _VAB_Order_Id.ToString());

                MPriceListVersionModel objPLV = new MPriceListVersionModel();
                _priceListVersion_Id = objPLV.GetVAM_PriceListVersion_ID(ctx, _VAM_PriceList_ID.ToString());


                MBPartnerModel objBPartner = new MBPartnerModel();
                Dictionary<String, String> bpartner1 = objBPartner.GetBPartner(ctx, _VAB_BusinessPartner_Id.ToString());
                _VAM_DiscountCalculation_ID = Util.GetValueOfInt(bpartner1["VAM_DiscountCalculation_ID"]);
                _flatDiscount = Util.GetValueOfInt(bpartner1["FlatDiscount"]);

                if (_VAM_PFeature_SetInstance_Id > 0)
                {
                    sql.Append("SELECT COUNT(*) FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                     + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                     + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                                     + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                }
                else
                {
                    sql.Append("SELECT COUNT(*) FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                   + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                   + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                   + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                }
                int countrecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
                if (countrecord > 0)
                {
                    // Selected UOM Price Exist
                    sql.Clear();
                    if (_VAM_PFeature_SetInstance_Id > 0)
                    {
                        sql.Append("SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                    + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                    + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                                    + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                    }
                    else
                    {
                        sql.Append("SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                  + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                  + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                  + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                    }
                    DataSet ds = DB.ExecuteDataset(sql.ToString());
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                            _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                            //end
                            PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                        }
                    }
                }
                else //if (_VAM_PFeature_SetInstance_Id > 0 && countrecord == 0)
                {
                    sql.Clear();
                    sql.Append("SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                    DataSet ds1 = DB.ExecuteDataset(sql.ToString());
                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        //Flat Discount
                        PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]),
                       _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                        //End
                        PriceList = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceList"]);
                    }
                    else
                    {
                        // get uom from product
                        var paramStr = _VAM_Product_Id.ToString();
                        MProductModel objProduct = new MProductModel();
                        var prodVAB_UOM_ID = objProduct.GetVAB_UOM_ID(ctx, paramStr);
                        sql.Clear();
                        if (_VAM_PFeature_SetInstance_Id > 0)
                        {
                            sql.Append("SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                        + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                        + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                                        + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID);
                        }
                        else
                        {
                            sql.Append("SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                      + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                      + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                      + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID);
                        }
                        DataSet ds = DB.ExecuteDataset(sql.ToString());
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                //Flat Discount
                                PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                                _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                                //end
                                PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                            }
                            else if (_VAM_PFeature_SetInstance_Id > 0)
                            {
                                sql.Clear();
                                sql.Append("SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                            + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                            + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                            + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID);
                                DataSet ds2 = DB.ExecuteDataset(sql.ToString());
                                if (ds2 != null && ds.Tables.Count > 0)
                                {
                                    if (ds2.Tables[0].Rows.Count > 0)
                                    {
                                        //Flat Discount
                                        PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                                    _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                                        //End
                                        PriceList = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceList"]);
                                    }
                                }
                            }
                        }
                        sql.Clear();
                        sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y' " +
                                                   " AND con.VAM_Product_ID = " + _VAM_Product_Id +
                                                   " AND con.VAB_UOM_ID = " + prodVAB_UOM_ID + " AND con.VAB_UOM_To_ID = " + _VAB_UOM_Id);
                        var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                        if (rate == 0)
                        {
                            sql.Clear();
                            sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y'" +
                                  " AND con.VAB_UOM_ID = " + prodVAB_UOM_ID + " AND con.VAB_UOM_To_ID = " + _VAB_UOM_Id);
                            rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                        }
                        if (rate == 0)
                        {
                            rate = 1;
                        }
                        PriceEntered = PriceEntered * rate;
                        PriceList = PriceList * rate;
                    }
                }
            }

            retDic["PriceEntered"] = PriceEntered;
            retDic["PriceList"] = PriceList;

            return retDic;
        }

        /// <summary>
        /// when we change qtyOrder, product , QtyEntered or Attribute
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="param">List of Parameters</param>
        /// <returns>List of Price Data</returns>
        public Dictionary<String, Object> GetPricesOnChange(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            Dictionary<String, Object> retDic = new Dictionary<string, Object>();

            //Assign parameter value
            int _VAM_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            //int _priceListVersion_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAB_Order_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAM_PFeature_SetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _VAB_UOM_Id = Util.GetValueOfInt(paramValue[3].ToString());
            int _vaf_client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            //int _VAM_DiscountCalculation_ID = Util.GetValueOfInt(paramValue[5].ToString());
            //decimal _flatDiscount = Util.GetValueOfInt(paramValue[6].ToString());
            int _VAB_BusinessPartner_Id = Util.GetValueOfInt(paramValue[5].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[6].ToString());
            int countEd011 = Util.GetValueOfInt(paramValue[7].ToString());
            int countVAPRC = Util.GetValueOfInt(paramValue[8].ToString());

            //End Assign parameter value
            StringBuilder sql = new StringBuilder();
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            int _VAM_PriceList_ID = 0;
            int _priceListVersion_Id = 0;
            int _VAM_DiscountCalculation_ID = 0;
            decimal _flatDiscount = 0;

            MOrderModel objOrder = new MOrderModel();
            _VAM_PriceList_ID = objOrder.GetVAM_PriceList(ctx, _VAB_Order_ID.ToString());

            MPriceListVersionModel objPLV = new MPriceListVersionModel();
            _priceListVersion_Id = objPLV.GetVAM_PriceListVersion_ID(ctx, _VAM_PriceList_ID.ToString());

            MBPartnerModel objBPartner = new MBPartnerModel();
            Dictionary<String, String> bpartner1 = objBPartner.GetBPartner(ctx, _VAB_BusinessPartner_Id.ToString());
            _VAM_DiscountCalculation_ID = Util.GetValueOfInt(bpartner1["VAM_DiscountCalculation_ID"]);
            _flatDiscount = Util.GetValueOfInt(bpartner1["FlatDiscount"]);

            if (countEd011 > 0)
            {
                if (_VAM_PFeature_SetInstance_Id > 0)
                {
                    sql.Append("SELECT COUNT(*) FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                     + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                     + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                                     + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                }
                else
                {
                    sql.Append("SELECT COUNT(*) FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                   + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                   + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                   + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                }
            }
            else if (countVAPRC > 0)
            {
                if (_VAM_PFeature_SetInstance_Id > 0)
                {
                    sql.Append("SELECT COUNT(*) FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                     + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                     + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id);
                }
                else
                {
                    sql.Append("SELECT COUNT(*) FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                   + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                   + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ");
                }
            }
            int countrecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            if (countrecord > 0)
            {
                // Selected UOM Price Exist
                sql.Clear();
                if (countEd011 > 0)
                {
                    if (_VAM_PFeature_SetInstance_Id > 0)
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                    + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                    + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                                    + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                    }
                    else
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                  + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                  + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                  + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                    }
                }
                else if (countVAPRC > 0)
                {
                    if (_VAM_PFeature_SetInstance_Id > 0)
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                    + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                    + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id);
                    }
                    else
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                  + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                  + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ");
                    }
                }
                DataSet ds = DB.ExecuteDataset(sql.ToString());
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //Flat Discount
                        PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                        _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                        //end
                        PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                        PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                    }
                }
            }
            else        //if (_VAM_PFeature_SetInstance_Id > 0 && countrecord == 0)
            {
                sql.Clear();
                if (countEd011 > 0)
                {
                    sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                }
                else if (countVAPRC > 0)
                {
                    sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                               + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                               + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ");
                }
                DataSet ds1 = DB.ExecuteDataset(sql.ToString());
                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    //Flat Discount
                    PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]),
                   _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                    //End
                    PriceList = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceList"]);
                    PriceLimit = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceLimit"]);
                }
                else
                {
                    // get uom from product
                    var paramStr = _VAM_Product_Id.ToString();
                    MProductModel objProduct = new MProductModel();
                    var prodVAB_UOM_ID = objProduct.GetVAB_UOM_ID(ctx, paramStr);
                    sql.Clear();
                    if (countEd011 > 0)
                    {
                        if (_VAM_PFeature_SetInstance_Id > 0)
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                        + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                        + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                                        + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID);
                        }
                        else
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                      + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                      + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                      + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID);
                        }
                    }
                    else if (countVAPRC > 0)
                    {
                        if (_VAM_PFeature_SetInstance_Id > 0)
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                        + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                        + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id);
                        }
                        else
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                      + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                      + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ");
                        }
                    }
                    DataSet ds = DB.ExecuteDataset(sql.ToString());
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                            _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                            //end
                            PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                            PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                        }
                        else if (_VAM_PFeature_SetInstance_Id > 0)
                        {
                            sql.Clear();
                            if (countEd011 > 0)
                            {
                                sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                            + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                            + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                            + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID);
                            }
                            else if (countVAPRC > 0)
                            {
                                sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                           + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                           + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) ");
                            }
                            DataSet ds2 = DB.ExecuteDataset(sql.ToString());
                            if (ds2 != null && ds.Tables.Count > 0)
                            {
                                if (ds2.Tables[0].Rows.Count > 0)
                                {
                                    //Flat Discount
                                    PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                                _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                                    //End
                                    PriceList = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceList"]);
                                    PriceLimit = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceLimit"]);
                                }
                            }
                        }
                    }

                    sql.Clear();
                    sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y' " +
                                               " AND con.VAM_Product_ID = " + _VAM_Product_Id +
                                               " AND con.VAB_UOM_ID = " + prodVAB_UOM_ID + " AND con.VAB_UOM_To_ID = " + _VAB_UOM_Id);
                    var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                    if (rate == 0)
                    {
                        sql.Clear();
                        sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y'" +
                              " AND con.VAB_UOM_ID = " + prodVAB_UOM_ID + " AND con.VAB_UOM_To_ID = " + _VAB_UOM_Id);
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

            retDic["PriceEntered"] = PriceEntered;
            retDic["PriceList"] = PriceList;
            retDic["PriceLimit"] = PriceLimit;

            return retDic;
        }

        //when we change the UOM
        public Dictionary<String, Object> GetPricesOnUomChange(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            Dictionary<String, Object> retDic = new Dictionary<String, Object>();

            //Assign parameter value
            int _VAM_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _VAB_Order_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAM_PFeature_SetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _VAB_UOM_Id = Util.GetValueOfInt(paramValue[3].ToString());
            int _vaf_client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            int _VAM_DiscountCalculation_ID = Util.GetValueOfInt(paramValue[5].ToString());
            decimal _flatDiscount = Util.GetValueOfInt(paramValue[6].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[7].ToString());
            int prodVAB_UOM_ID = Util.GetValueOfInt(paramValue[8].ToString());
            //End Assign parameter value

            MOrderModel objOrder = new MOrderModel();
            int _VAM_PriceList_ID = objOrder.GetVAM_PriceList(ctx, _VAB_Order_ID.ToString());

            MPriceListVersionModel objPLV = new MPriceListVersionModel();
            int _priceListVersion_Id = objPLV.GetVAM_PriceListVersion_ID(ctx, _VAM_PriceList_ID.ToString());

            string sql;
            decimal PriceEntered = 0;
            decimal PriceList = 0;

            if (_VAM_PFeature_SetInstance_Id > 0)
            {
                sql = "SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                            + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                            + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                            + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID;
            }
            else
            {
                sql = "SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                          + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                          + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                          + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID;
            }
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //Flat Discount
                    PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                    _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                    //end
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                }
                else if (_VAM_PFeature_SetInstance_Id > 0)
                {
                    sql = "SELECT PriceStd , PriceList FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                + "  AND VAB_UOM_ID=" + prodVAB_UOM_ID;
                    DataSet ds2 = DB.ExecuteDataset(sql);
                    if (ds2 != null && ds.Tables.Count > 0)
                    {
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                        _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                            //End
                            PriceList = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceList"]);
                        }
                    }
                }
            }

            sql = "SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y' " +
                                       " AND con.VAM_Product_ID = " + _VAM_Product_Id +
                                       " AND con.VAB_UOM_ID = " + prodVAB_UOM_ID + " AND con.VAB_UOM_To_ID = " + _VAB_UOM_Id;
            var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            if (rate == 0)
            {
                sql = "SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y'" +
                      " AND con.VAB_UOM_ID = " + prodVAB_UOM_ID + " AND con.VAB_UOM_To_ID = " + _VAB_UOM_Id;
                rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            }
            PriceEntered = PriceEntered * rate;
            PriceList = PriceList * rate;

            retDic["PriceEntered"] = PriceEntered;
            retDic["PriceList"] = PriceList;

            return retDic;
        }

        /// <summary>
        /// Get Product Prices on change of UOM
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="param">List of Parameters</param>
        /// <returns>List of Price Data</returns>
        public Dictionary<String, Object> GetProductPriceOnUomChange(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            Dictionary<String, Object> retDic = new Dictionary<String, Object>();

            //Assign parameter value
            int _VAB_BusinessPartner_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int _VAB_Order_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAM_PFeature_SetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _VAB_UOM_To_ID = Util.GetValueOfInt(paramValue[3].ToString());
            int _vaf_client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            int _VAM_Product_Id = Util.GetValueOfInt(paramValue[5].ToString());
            decimal _qtyEntered = Util.GetValueOfDecimal(paramValue[6].ToString());
            //End Assign parameter value

            string sql;
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            int countEd011 = 0;
            int isAttributeValue = 0;
            int _VAM_PriceList_ID = 0;
            int _priceListVersion_Id = 0;

            countEd011 = Env.IsModuleInstalled("ED011_") ? 1 : 0;
            retDic["countEd011"] = countEd011.ToString();
            if (countEd011 > 0)
            {
                MBPartnerModel objBPartner = new MBPartnerModel();
                Dictionary<String, String> bpartner = objBPartner.GetBPartner(ctx, _VAB_BusinessPartner_ID.ToString());

                MOrderModel objOrder = new MOrderModel();
                _VAM_PriceList_ID = objOrder.GetVAM_PriceList(ctx, _VAB_Order_ID.ToString());

                MPriceListVersionModel objPLV = new MPriceListVersionModel();
                _priceListVersion_Id = objPLV.GetVAM_PriceListVersion_ID(ctx, _VAM_PriceList_ID.ToString());

                sql = "SELECT PriceList , PriceStd , PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                      + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                      + " AND  VAM_PFeature_SetInstance_ID = " + _VAM_PFeature_SetInstance_Id
                                      + "  AND VAB_UOM_ID=" + _VAB_UOM_To_ID;
                DataSet ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isAttributeValue = 1;
                    //Flat Discount
                    PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                                       Util.GetValueOfInt(bpartner["VAM_DiscountCalculation_ID"]), Util.GetValueOfDecimal(bpartner["FlatDiscount"]), _qtyEntered);
                    //End
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                    PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                }
                else
                {
                    sql = "SELECT PriceList , PriceStd , PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                      + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                      + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) AND VAB_UOM_ID=" + _VAB_UOM_To_ID;
                    ds = DB.ExecuteDataset(sql, null, null);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        isAttributeValue = 2;
                        //Flat Discount
                        PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                                           Util.GetValueOfInt(bpartner["VAM_DiscountCalculation_ID"]), Util.GetValueOfDecimal(bpartner["FlatDiscount"]), _qtyEntered);
                        //End
                        PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                        PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                    }
                    else
                    {
                        isAttributeValue = 0;
                    }
                }
            }
            retDic["PriceList"] = PriceList;
            retDic["PriceEntered"] = PriceEntered;
            retDic["PriceLimit"] = PriceLimit;
            retDic["isAttributeValue"] = isAttributeValue;

            return retDic;
        }

        /// <summary>
        /// on change of new Product
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="param">List of Parameters</param>
        /// <returns>List of Price Data</returns>
        public Dictionary<String, Object> GetPricesOnProductChange(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            Dictionary<String, Object> retDic = new Dictionary<String, Object>();

            //Assign parameter value
            int _VAM_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _vaf_client_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAB_Order_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _VAB_BusinessPartner_Id = Util.GetValueOfInt(paramValue[3].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[4].ToString());
            int purchasingUom = Util.GetValueOfInt(paramValue[5].ToString());

            //End Assign parameter value
            StringBuilder sql = new StringBuilder();
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            decimal QtyOrdered = 0;
            int _VAM_DiscountCalculation_ID = 0;
            decimal _flatDiscount = 0;
            int _VAB_UOM_Id = 0;
            int _priceListVersion_Id = 0;
            int _VAM_PriceList_ID = 0;
            int _standardPrecision = 0;

            MProductModel objProduct = new MProductModel();
            _VAB_UOM_Id = objProduct.GetVAB_UOM_ID(ctx, _VAM_Product_Id.ToString());

            MOrderModel objOrder = new MOrderModel();
            _VAM_PriceList_ID = objOrder.GetVAM_PriceList(ctx, _VAB_Order_Id.ToString());

            MPriceListVersionModel objPLV = new MPriceListVersionModel();
            _priceListVersion_Id = objPLV.GetVAM_PriceListVersion_ID(ctx, _VAM_PriceList_ID.ToString());

            MUOMModel objUOM = new MUOMModel();
            _standardPrecision = objUOM.GetPrecision(ctx, _VAB_UOM_Id.ToString());

            MBPartnerModel objBPartner = new MBPartnerModel();
            Dictionary<String, String> bpartner1 = objBPartner.GetBPartner(ctx, _VAB_BusinessPartner_Id.ToString());
            _VAM_DiscountCalculation_ID = Util.GetValueOfInt(bpartner1["VAM_DiscountCalculation_ID"]);
            _flatDiscount = Util.GetValueOfInt(bpartner1["FlatDiscount"]);

            sql.Append("SELECT PriceStd , PriceList , PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                      + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                      + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                      + "  AND VAB_UOM_ID=" + purchasingUom);
            DataSet ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //Flat Discount
                    PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                    _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                    //end
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                    PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);

                    //set Conversion Ordered Qty
                    sql.Clear();
                    sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y' " +
                                      " AND con.VAM_Product_ID = " + _VAM_Product_Id +
                                      " AND con.VAB_UOM_ID = " + _VAB_UOM_Id + " AND con.VAB_UOM_To_ID = " + purchasingUom);
                    var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                    if (rate == 0)
                    {
                        sql.Clear();
                        sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y'" +
                              " AND con.VAB_UOM_ID = " + _VAB_UOM_Id + " AND con.VAB_UOM_To_ID = " + purchasingUom);
                        rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                    }
                    if ((_VAB_UOM_Id == purchasingUom) && (rate == 0))
                    {
                        rate = 1;
                    }
                    QtyOrdered = _qtyEntered * rate;
                }
                else
                {
                    sql.Clear();
                    sql.Append("SELECT PriceStd , PriceList , PriceLimit FROM VAM_ProductPrice WHERE Isactive='Y' AND VAM_Product_ID = " + _VAM_Product_Id
                                + " AND VAM_PriceListVersion_ID = " + _priceListVersion_Id
                                + " AND  ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL ) "
                                + "  AND VAB_UOM_ID=" + _VAB_UOM_Id);
                    DataSet ds2 = DB.ExecuteDataset(sql.ToString());
                    if (ds2 != null && ds.Tables.Count > 0)
                    {
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = FlatDiscount(_VAM_Product_Id, _vaf_client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                        _VAM_DiscountCalculation_ID, _flatDiscount, _qtyEntered);
                            //End

                            //set Conversion Ordered Qty
                            sql.Clear();
                            sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y' " +
                                              " AND con.VAM_Product_ID = " + _VAM_Product_Id +
                                              " AND con.VAB_UOM_ID = " + _VAB_UOM_Id + " AND con.VAB_UOM_To_ID = " + purchasingUom);
                            var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                            if (rate == 0)
                            {
                                sql.Clear();
                                sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y'" +
                                      " AND con.VAB_UOM_ID = " + _VAB_UOM_Id + " AND con.VAB_UOM_To_ID = " + purchasingUom);
                                rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                            }

                            PriceList = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceList"]) * rate;
                            PriceLimit = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceLimit"]) * rate;
                            PriceEntered = PriceEntered * rate;
                            // If rate = zero then change it to 1 By Vaibhav
                            if (rate == 0)
                            {
                                rate = 1;
                            }
                            QtyOrdered = _qtyEntered * rate;
                        }
                        else
                        {
                            //set Conversion Ordered Qty
                            sql.Clear();
                            sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y' " +
                                              " AND con.VAM_Product_ID = " + _VAM_Product_Id +
                                              " AND con.VAB_UOM_ID = " + _VAB_UOM_Id + " AND con.VAB_UOM_To_ID = " + purchasingUom);
                            var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                            if (rate == 0)
                            {
                                sql.Clear();
                                sql.Append("SELECT con.DivideRate FROM VAB_UOM_Conversion con INNER JOIN VAB_UOM uom ON con.VAB_UOM_ID = uom.VAB_UOM_ID WHERE con.IsActive = 'Y'" +
                                      " AND con.VAB_UOM_ID = " + _VAB_UOM_Id + " AND con.VAB_UOM_To_ID = " + purchasingUom);
                                rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                            }
                            if (rate == 0)
                            {
                                rate = 1;
                            }
                            QtyOrdered = _qtyEntered * rate;
                        }
                    }
                }
            }

            retDic["PriceEntered"] = PriceEntered;
            retDic["PriceList"] = PriceList;
            retDic["PriceLimit"] = PriceLimit;
            retDic["QtyOrdered"] = QtyOrdered;
            return retDic;
        }

        public Dictionary<String, String> GetProductInfo(Ctx ctx, string param)
        {
            Dictionary<String, String> retDic = new Dictionary<string, string>();

            string[] paramValue = param.Split(',');

            //Assign parameter value
            int _VAM_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _VAB_BusinessPartner_Id = Util.GetValueOfInt(paramValue[1].ToString());
            //End Assign parameter value
            string sql = null;

            sql = "SELECT producttype FROM VAM_Product where isactive = 'Y' AND VAM_Product_ID = " + _VAM_Product_Id;
            string productType = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            retDic["productType"] = Convert.ToString(productType);

            sql = "SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='ED011_'";
            int countEd011 = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            retDic["countEd011"] = Convert.ToString(countEd011);

            sql = "SELECT VAB_UOM_ID FROM VAM_Product_PO WHERE IsActive = 'Y' AND  VAB_BusinessPartner_ID = " + _VAB_BusinessPartner_Id +
                    " AND VAM_Product_ID = " + _VAM_Product_Id;
            int purchasingUom = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            retDic["purchasingUom"] = Convert.ToString(purchasingUom);

            sql = "SELECT VAB_UOM_ID FROM VAM_Product WHERE IsActive = 'Y' AND VAM_Product_ID = " + _VAM_Product_Id;
            int headerUom = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            retDic["headerUom"] = Convert.ToString(headerUom);

            sql = "SELECT IsDropShip FROM VAM_Product WHERE IsActive = 'Y' AND VAM_Product_ID = " + _VAM_Product_Id;
            string IsDropShip = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            retDic["IsDropShip"] = Convert.ToString(IsDropShip);

            return retDic;
        }

        /// <summary>
        /// Calculate Discout based on Discount Schema selected on Business Partner
        /// </summary>
        /// <param name="ProductId">Product ID</param>
        /// <param name="ClientId">Tenant</param>
        /// <param name="amount">Amount on which discount is to be calculated</param>
        /// <param name="DiscountSchemaId">Discount Schema of Business Partner</param>
        /// <param name="FlatDiscount">Flat Discount % on Business Partner</param>
        /// <param name="QtyEntered">Quantity on which discount is to be calculated</param>
        /// <returns>Discount value</returns>
        public decimal FlatDiscount(int ProductId, int ClientId, decimal amount, int DiscountSchemaId, decimal FlatDiscount, decimal QtyEntered)
        {
            decimal amountAfterBreak = amount;
            var sql = "SELECT VAM_ProductCategory_ID FROM VAM_Product WHERE IsActive='Y' AND VAM_Product_ID = " + ProductId;
            var productCategoryId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            var isCalulate = false;
            DataSet dsDiscountBreak = null;
            string discountType = "";
            decimal discountBreakValue = 0;

            // Is flat Discount
            // JID_0487: Not considering Business Partner Flat Discout Checkbox value while calculating the Discount
            sql = "SELECT  DiscountType, IsBPartnerFlatDiscount, FlatDiscount FROM VAM_DiscountCalculation WHERE "
                      + "VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId;
            dsDiscountBreak = DB.ExecuteDataset(sql);

            if (dsDiscountBreak != null && dsDiscountBreak.Tables.Count > 0 && dsDiscountBreak.Tables[0].Rows.Count > 0)
            {
                discountType = Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["DiscountType"]);
                if (discountType == MVAMDiscountCalculation.DISCOUNTTYPE_FlatPercent)
                {
                    isCalulate = true;
                    if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                    {
                        discountBreakValue = (amount - ((amount * Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[0]["FlatDiscount"])) / 100));
                    }
                    else
                    {
                        discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
                    }

                    amountAfterBreak = discountBreakValue;
                    return amountAfterBreak;
                }

                else if (discountType == MVAMDiscountCalculation.DISCOUNTTYPE_Breaks)
                {
                    // Product Based
                    sql = "SELECT VAM_ProductCategory_ID , VAM_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM VAM_BreakDiscount WHERE "
                               + "VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND VAM_Product_ID = " + ProductId
                               + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId + "Order BY BreakValue DESC";
                    dsDiscountBreak = DB.ExecuteDataset(sql);
                    if (dsDiscountBreak != null && dsDiscountBreak.Tables.Count > 0 && dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        var m = 0;
                        discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered < Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"]))
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[m]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = (amount - (amount * Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"]) / 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                    //

                    // Product Category Based
                    sql = "SELECT VAM_ProductCategory_ID , VAM_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM VAM_BreakDiscount WHERE "
                               + " VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND VAM_ProductCategory_ID = " + productCategoryId
                               + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId + "Order BY BreakValue DESC";
                    dsDiscountBreak = DB.ExecuteDataset(sql);
                    if (dsDiscountBreak != null && dsDiscountBreak.Tables.Count > 0 && dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        var m = 0;
                        discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered < Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"]))
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[m]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = (amount - (amount * Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"]) / 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                    //

                    // Otherwise
                    sql = "SELECT VAM_ProductCategory_ID , VAM_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM VAM_BreakDiscount WHERE "
                               + " VAM_DiscountCalculation_ID = " + DiscountSchemaId + " AND VAM_ProductCategory_ID IS NULL AND VAM_Product_id IS NULL "
                               + " AND IsActive='Y'  AND VAF_Client_ID=" + ClientId + "Order BY BreakValue DESC";
                    dsDiscountBreak = DB.ExecuteDataset(sql);
                    if (dsDiscountBreak != null && dsDiscountBreak.Tables.Count > 0 && dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        var m = 0;
                        discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered < Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"]))
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[m]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = (amount - (amount * Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"]) / 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                }
            }

            return amountAfterBreak;
        }

        // on change of Tax
        public Dictionary<String, object> GetTaxId(Ctx ctx, string param)
        {
            string[] paramValue = param.Split(',');

            //Assign parameter value
            int _VAM_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _VAB_Order_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _VAB_Charge_Id = Util.GetValueOfInt(paramValue[2].ToString());
            //End Assign parameter value

            Dictionary<String, object> retDic = new Dictionary<string, object>();
            string sql = null;
            int taxId = 0;
            int _VAB_BusinessPartner_Id = 0;
            int _c_Bill_Location_Id = 0;
            int _CountVATAX = 0;

            _CountVATAX = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX IN ('VATAX_' )", null, null));
            retDic["_CountVATAX"] = _CountVATAX.ToString();

            MOrderModel objOrder = new MOrderModel();
            Dictionary<String, object> order = objOrder.GetOrder(ctx, _VAB_Order_Id.ToString());
            _VAB_BusinessPartner_Id = Util.GetValueOfInt(order["VAB_BusinessPartner_ID"]);
            _c_Bill_Location_Id = Util.GetValueOfInt(order["Bill_Location_ID"]);

            // Added by Bharat on 26 Feb 2018 to check Exempt Tax on Business Partner
            MVABBusinessPartner bp = new MVABBusinessPartner(ctx, _VAB_BusinessPartner_Id, null);
            retDic["TaxExempt"] = bp.IsTaxExempt() ? "Y" : "N";

            if (_CountVATAX > 0)
            {
                sql = "SELECT VATAX_TaxRule FROM VAF_OrgDetail WHERE VAF_Org_ID=" + Util.GetValueOfInt(order["VAF_Org_ID"]) + " AND IsActive ='Y' AND VAF_Client_ID =" + ctx.GetVAF_Client_ID();
                string taxRule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
                retDic["taxRule"] = taxRule.ToString();

                sql = "SELECT Count(*) FROM VAF_Column WHERE ColumnName = 'VAB_TaxRate_ID' AND VAF_TableView_ID = (SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName = 'VAB_TaxCategory')";
                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                {
                    var paramString = (_VAB_Order_Id).ToString() + "," + (_VAM_Product_Id).ToString() + "," + (_VAB_Charge_Id).ToString();

                    taxId = GetTax(ctx, paramString);
                }
                else
                {
                    sql = "SELECT VATAX_TaxType_ID FROM VAB_BPart_Location WHERE VAB_BusinessPartner_ID =" + Util.GetValueOfInt(_VAB_BusinessPartner_Id) +
                                      " AND IsActive = 'Y'  AND VAB_BPart_Location_ID = " + Util.GetValueOfInt(_c_Bill_Location_Id);
                    var taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (taxType == 0)
                    {
                        sql = "SELECT VATAX_TaxType_ID FROM VAB_BusinessPartner WHERE VAB_BusinessPartner_ID =" + Util.GetValueOfInt(_VAB_BusinessPartner_Id) + " AND IsActive = 'Y'";
                        taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    }

                    MProductModel objProduct = new MProductModel();
                    var prodtaxCategory = objProduct.GetTaxCategory(ctx, _VAM_Product_Id.ToString());
                    sql = "SELECT VAB_TaxRate_ID FROM VATAX_TaxCatRate WHERE VAB_TaxCategory_ID = " + prodtaxCategory + " AND IsActive ='Y' AND VATAX_TaxType_ID =" + taxType;
                    taxId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                }
            }
            retDic["taxId"] = taxId.ToString();

            return retDic;
        }

        // Change by mohit to remove client side queries - 16 May 2017
        public decimal GetChargeAmount(Ctx ctx, string fields)
        {
            MVABCharge charge = new MVABCharge(ctx, Util.GetValueOfInt(fields), null);
            return charge.GetChargeAmt();

        }
        public Dictionary<string, object> GetEnforcePriceLimit(string fields)
        {
            Dictionary<string, object> retValue = new Dictionary<string, object>();
            string[] paramValues = fields.Split(',');

            //string sql = "SELECT EnforcePriceLimit FROM VAM_PriceList WHERE IsActive = 'Y' AND VAM_PriceList_ID = " + Util.GetValueOfInt(fields[0]);
            retValue["EnforcePriceLimit"] = DB.ExecuteScalar("SELECT EnforcePriceLimit FROM VAM_PriceList WHERE IsActive = 'Y' AND VAM_PriceList_ID = " + Util.GetValueOfInt(paramValues[0]), null, null);
            retValue["OverwritePriceLimit"] = DB.ExecuteScalar("SELECT OverwritePriceLimit FROM VAF_Role WHERE IsActive = 'Y' AND VAF_Role_ID = " + Util.GetValueOfInt(paramValues[1]), null, null);
            return retValue;

        }

        public Dictionary<string, object> GetProdAttributeSetInstance(string fields)
        {
            string[] paramValues = fields.Split(',');
            Dictionary<string, object> retValue = new Dictionary<string, object>();

            try
            {
                retValue["VAM_PFeature_Set_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_PFeature_Set_ID FROM VAM_Product WHERE VAM_Product_ID = " + Util.GetValueOfInt(paramValues[0]), null, null));

                if (Util.GetValueOfInt(retValue["VAM_PFeature_Set_ID"]) > 0)
                {
                    retValue["Attribute_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT VAM_PFeature_SetInstance_ID FROM VAM_ProductFeatures WHERE UPC = '" + Util.GetValueOfString(paramValues[1]) + "' AND VAM_Product_ID = " + Util.GetValueOfInt(paramValues[0]), null, null));
                }
                else
                {
                    retValue["Attribute_ID"] = 0;
                }
            }
            catch (Exception e)
            {

            }
            return retValue;
        }

        // Get Prices From Pricelist
        public Dictionary<string, object> GetProductprices(string fields)
        {
            string[] paramString = fields.Split(',');
            Dictionary<string, object> retValue = null;
            DataSet _ds = null;
            try
            {
                string _sql = "SELECT PriceList , PriceStd , PriceLimit FROM VAM_ProductPrice WHERE IsActive='Y' AND VAM_Product_ID = " + Util.GetValueOfInt(paramString[0]) +
                                          " AND VAM_PriceListVersion_ID = " + Util.GetValueOfInt(paramString[1]);
                if (Util.GetValueOfInt(paramString[2]) > 0)
                {
                    _sql = _sql + " AND VAB_UOM_ID =" + Util.GetValueOfInt(paramString[2]);
                }
                if (Util.GetValueOfInt(paramString[3]) > 0)
                {
                    _sql = _sql + " AND VAM_PFeature_SetInstance_ID = " + Util.GetValueOfInt(paramString[3]);
                }
                else
                {
                    _sql = _sql + " AND ( VAM_PFeature_SetInstance_ID = 0 OR VAM_PFeature_SetInstance_ID IS NULL )";
                }
                _ds = DB.ExecuteDataset(_sql, null, null);
                if (_ds != null && _ds.Tables[0].Rows.Count > 0)
                {
                    retValue = new Dictionary<string, object>();
                    retValue["PriceList"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["PriceList"]);
                    retValue["PriceStd"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["PriceStd"]);
                    retValue["PriceLimit"] = Util.GetValueOfDecimal(_ds.Tables[0].Rows[0]["PriceLimit"]);
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

        //Get Product Cost
        public Dictionary<String, String> GetProductCost(Ctx ctx, int param)
        {
            Dictionary<String, String> retDic = new Dictionary<String, String>();


            //Assign parameter value
            int _VAM_Product_Id = Util.GetValueOfInt(param);
            //End Assign parameter value
            string sql = null;

            sql = "SELECT GETPRODUCTCOST( VAM_Product_ID ,VAF_Client_ID ) as Cost FROM VAM_Product Where isactive = 'Y' AND VAM_Product_ID = " + _VAM_Product_Id;
            decimal Cost = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            retDic["Cost"] = Convert.ToString(Cost);

            return retDic;
        }

        //Get No of Months---Neha
        public int GetNoOfMonths(int _frequency_Id)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("Select NoOfMonths from VAB_Frequency where VAB_Frequency_ID=" + _frequency_Id, null, null));

        }
    }
}