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
    public class MOrderLineModel
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

            MOrderLine orderline = new MOrderLine(ctx, id, null);

            retDic["C_Tax_ID"] = Util.GetValueOfString(orderline.GetC_Tax_ID());
            retDic["PriceList"] = Util.GetValueOfString(orderline.GetPriceList());
            retDic["PriceLimit"] = Util.GetValueOfString(orderline.GetPriceLimit());
            retDic["PriceActual"] = Util.GetValueOfString(orderline.GetPriceActual());
            retDic["PriceEntered"] = Util.GetValueOfString(orderline.GetPriceEntered());
            retDic["C_Currency_ID"] = Util.GetValueOfString(orderline.GetC_Currency_ID());
            retDic["Discount"] = Util.GetValueOfString(orderline.GetDiscount());
            retDic["Discount"] = Util.GetValueOfString(orderline.GetDiscount());
            retDic["M_Product_ID"] = Util.GetValueOfString(orderline.GetM_Product_ID());

            // when order line contains charge, it will be selected on Shipment Line on selection of Order Line
            retDic["C_Charge_ID"] = Util.GetValueOfString(orderline.GetC_Charge_ID());
            retDic["Qty"] = Util.GetValueOfString(orderline.GetQtyEntered());
            retDic["C_UOM_ID"] = Util.GetValueOfString(orderline.GetC_UOM_ID());
            retDic["C_BPartner_ID"] = Util.GetValueOfString(orderline.GetC_BPartner_ID());
            retDic["PlannedHours"] = Util.GetValueOfString(orderline.GetQtyOrdered());
            retDic["M_AttributeSetInstance_ID"] = Util.GetValueOfString(orderline.GetM_AttributeSetInstance_ID());
            retDic["QtyOrdered"] = Util.GetValueOfString(orderline.GetQtyOrdered());
            retDic["QtyDelivered"] = Util.GetValueOfString(orderline.GetQtyDelivered());
            retDic["QtyEntered"] = Util.GetValueOfString(orderline.GetQtyEntered());
            retDic["C_Activity_ID"] = Util.GetValueOfString(orderline.GetC_Activity_ID());
            retDic["C_Campaign_ID"] = Util.GetValueOfString(orderline.GetC_Campaign_ID());
            retDic["C_Project_ID"] = Util.GetValueOfString(orderline.GetC_Project_ID());
            retDic["C_ProjectPhase_ID"] = Util.GetValueOfString(orderline.GetC_ProjectPhase_ID());
            retDic["C_ProjectTask_ID"] = Util.GetValueOfString(orderline.GetC_ProjectTask_ID());
            retDic["AD_OrgTrx_ID"] = Util.GetValueOfString(orderline.GetAD_OrgTrx_ID());
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
            int M_Warehouse_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int M_AttributeSetInstance_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int C_OrderLine_ID = Util.GetValueOfInt(paramValue[3].ToString());
            //End Assign parameter value
            return MOrderLine.GetNotReserved(ctx, M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, C_OrderLine_ID);

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
            int C_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int C_Charge_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int taxCategory = 0;
            string sql = "";
            if ((M_Product_ID == 0 && C_Charge_ID == 0) || C_Order_ID == 0)
            {
                return C_Tax_ID;
            }
            DataSet dsLoc = null;
            MOrder inv = new MOrder(ctx, C_Order_ID, null);
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

                if (taxCat.GetVATAX_Location() == "I")
                {
                    sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                        + inv.GetBill_Location_ID() + " AND bpl.IsActive = 'Y'";
                }
                else
                {
                    sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                        + inv.GetC_BPartner_Location_ID() + " AND bpl.IsActive = 'Y'";
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
            int C_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int C_Charge_ID = Util.GetValueOfInt(paramValue[2].ToString());
            int taxCategory = 0;
            string sql = "";
            if ((M_Product_ID == 0 && C_Charge_ID == 0) || C_Order_ID == 0)
            {
                return C_Tax_ID;
            }
            DataSet dsLoc = null;
            MOrder inv = new MOrder(ctx, C_Order_ID, null);
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

                if (taxCat.GetVATAX_Location() == "I")
                {
                    sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                        + inv.GetBill_Location_ID() + " AND bpl.IsActive = 'Y'";
                }
                else
                {
                    sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                        + inv.GetC_BPartner_Location_ID() + " AND bpl.IsActive = 'Y'";
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

                    // if Tax Preference is Tax Class

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
                    " AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= " + Postal + " AND tcr.postal_to >= " + Postal + " THEN 1 ELSE 2" +
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
                        " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND tcr.C_Region_ID IS NULL AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= " + Postal +
                        " AND tcr.postal_to >= " + Postal + " THEN 1 ELSE 2" + " END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
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
                            " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID IS NULL " + " AND tcr.C_Region_ID IS NULL AND (CASE WHEN (tcr.vatax_ispostal = 'Y') THEN CASE WHEN tcr.postal <= "
                            + Postal + " AND tcr.postal_to >= " + Postal + " THEN 1 ELSE 2 END ELSE  CASE WHEN tcr.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
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
                " AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= " + Postal + " AND trl.postal_to >= " + Postal + " THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '"
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
                    + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= "
                    + Postal + " AND trl.postal_to >= " + Postal + " THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
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
                        + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID IS NULL AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= "
                        + Postal + " AND trl.postal_to >= " + Postal + " THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
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
                " AND NVL(trl.C_Region_ID,0) = " + Region_ID + " AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= " + Postal + " AND trl.postal_to >= " + Postal +
                " THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
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
                    + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= "
                    + Postal + " AND trl.postal_to >= " + Postal + " THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
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
                        + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID IS NULL AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.vatax_ispostal = 'Y') THEN CASE WHEN trl.postal <= "
                        + Postal + " AND trl.postal_to >= " + Postal + " THEN 1 ELSE 2 END ELSE  CASE WHEN trl.postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
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
            int _C_Order_Id = Util.GetValueOfInt(paramValue[1].ToString());
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

            if (countEd011 > 0)
            {
                MOrderModel objOrder = new MOrderModel();
                _m_PriceList_ID = objOrder.GetM_PriceList(ctx, _C_Order_Id.ToString());

                MPriceListVersionModel objPLV = new MPriceListVersionModel();
                _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID(ctx, _m_PriceList_ID.ToString());


                MBPartnerModel objBPartner = new MBPartnerModel();
                Dictionary<String, String> bpartner1 = objBPartner.GetBPartner(ctx, _c_BPartner_Id.ToString());
                _m_DiscountSchema_ID = Util.GetValueOfInt(bpartner1["M_DiscountSchema_ID"]);
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
                        sql.Append("SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                    + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                    + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id
                                    + "  AND C_UOM_ID=" + _c_Uom_Id);
                    }
                    else
                    {
                        sql.Append("SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
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
                            PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                            _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                            //end
                            PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                        }
                    }
                }
                else //if (_m_AttributeSetInstance_Id > 0 && countrecord == 0)
                {
                    sql.Clear();
                    sql.Append("SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                + "  AND C_UOM_ID=" + _c_Uom_Id);
                    DataSet ds1 = DB.ExecuteDataset(sql.ToString());
                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        //Flat Discount
                        PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]),
                       _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                        //End
                        PriceList = Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceList"]);
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
                            sql.Append("SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                        + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                        + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id
                                        + "  AND C_UOM_ID=" + prodC_UOM_ID);
                        }
                        else
                        {
                            sql.Append("SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
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
                                PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                                _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                                //end
                                PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                            }
                            else if (_m_AttributeSetInstance_Id > 0)
                            {
                                sql.Clear();
                                sql.Append("SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                            + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                            + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                            + "  AND C_UOM_ID=" + prodC_UOM_ID);
                                DataSet ds2 = DB.ExecuteDataset(sql.ToString());
                                if (ds2 != null && ds.Tables.Count > 0)
                                {
                                    if (ds2.Tables[0].Rows.Count > 0)
                                    {
                                        //Flat Discount
                                        PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                                    _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                                        //End
                                        PriceList = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceList"]);
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
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            //int _priceListVersion_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _c_Order_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _m_AttributeSetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _c_Uom_Id = Util.GetValueOfInt(paramValue[3].ToString());
            int _ad_Client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            //int _m_DiscountSchema_ID = Util.GetValueOfInt(paramValue[5].ToString());
            //decimal _flatDiscount = Util.GetValueOfInt(paramValue[6].ToString());
            int _c_BPartner_Id = Util.GetValueOfInt(paramValue[5].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[6].ToString());
            int countEd011 = Util.GetValueOfInt(paramValue[7].ToString());
            int countVAPRC = Util.GetValueOfInt(paramValue[8].ToString());

            //End Assign parameter value
            StringBuilder sql = new StringBuilder();
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            int _m_PriceList_ID = 0;
            int _priceListVersion_Id = 0;
            int _m_DiscountSchema_ID = 0;
            decimal _flatDiscount = 0;

            MOrderModel objOrder = new MOrderModel();
            _m_PriceList_ID = objOrder.GetM_PriceList(ctx, _c_Order_ID.ToString());

            MPriceListVersionModel objPLV = new MPriceListVersionModel();
            _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID(ctx, _m_PriceList_ID.ToString());

            MBPartnerModel objBPartner = new MBPartnerModel();
            Dictionary<String, String> bpartner1 = objBPartner.GetBPartner(ctx, _c_BPartner_Id.ToString());
            _m_DiscountSchema_ID = Util.GetValueOfInt(bpartner1["M_DiscountSchema_ID"]);
            _flatDiscount = Util.GetValueOfInt(bpartner1["FlatDiscount"]);

            if (countEd011 > 0)
            {
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
            }
            else if (countVAPRC > 0)
            {
                if (_m_AttributeSetInstance_Id > 0)
                {
                    sql.Append("SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                     + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                     + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id);
                }
                else
                {
                    sql.Append("SELECT COUNT(*) FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                   + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                   + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ");
                }
            }
            int countrecord = Util.GetValueOfInt(DB.ExecuteScalar(sql.ToString(), null, null));
            if (countrecord > 0)
            {
                // Selected UOM Price Exist
                sql.Clear();
                if (countEd011 > 0)
                {
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
                }
                else if (countVAPRC > 0)
                {
                    if (_m_AttributeSetInstance_Id > 0)
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                    + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                    + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id);
                    }
                    else
                    {
                        sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                  + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                  + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ");
                    }
                }
                DataSet ds = DB.ExecuteDataset(sql.ToString());
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //Flat Discount
                        PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                        _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                        //end
                        PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                        PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                    }
                }
            }
            else        //if (_m_AttributeSetInstance_Id > 0 && countrecord == 0)
            {
                sql.Clear();
                if (countEd011 > 0)
                {
                    sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                + "  AND C_UOM_ID=" + _c_Uom_Id);
                }
                else if (countVAPRC > 0)
                {
                    sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                               + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                               + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ");
                }
                DataSet ds1 = DB.ExecuteDataset(sql.ToString());
                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    //Flat Discount
                    PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds1.Tables[0].Rows[0]["PriceStd"]),
                   _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                    //End
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
                    if (countEd011 > 0)
                    {
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
                    }
                    else if (countVAPRC > 0)
                    {
                        if (_m_AttributeSetInstance_Id > 0)
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                        + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                        + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id);
                        }
                        else
                        {
                            sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                      + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ");
                        }
                    }
                    DataSet ds = DB.ExecuteDataset(sql.ToString());
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                            _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                            //end
                            PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                            PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                        }
                        else if (_m_AttributeSetInstance_Id > 0)
                        {
                            sql.Clear();
                            if (countEd011 > 0)
                            {
                                sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                            + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                            + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                            + "  AND C_UOM_ID=" + prodC_UOM_ID);
                            }
                            else if (countVAPRC > 0)
                            {
                                sql.Append("SELECT PriceStd , PriceList, PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                           + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                           + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ");
                            }
                            DataSet ds2 = DB.ExecuteDataset(sql.ToString());
                            if (ds2 != null && ds.Tables.Count > 0)
                            {
                                if (ds2.Tables[0].Rows.Count > 0)
                                {
                                    //Flat Discount
                                    PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                                _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                                    //End
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
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _c_Order_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _m_AttributeSetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _c_Uom_Id = Util.GetValueOfInt(paramValue[3].ToString());
            int _ad_Client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            int _m_DiscountSchema_ID = Util.GetValueOfInt(paramValue[5].ToString());
            decimal _flatDiscount = Util.GetValueOfInt(paramValue[6].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[7].ToString());
            int prodC_UOM_ID = Util.GetValueOfInt(paramValue[8].ToString());
            //End Assign parameter value

            MOrderModel objOrder = new MOrderModel();
            int _m_PriceList_ID = objOrder.GetM_PriceList(ctx, _c_Order_ID.ToString());

            MPriceListVersionModel objPLV = new MPriceListVersionModel();
            int _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID(ctx, _m_PriceList_ID.ToString());

            string sql;
            decimal PriceEntered = 0;
            decimal PriceList = 0;

            if (_m_AttributeSetInstance_Id > 0)
            {
                sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                            + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                            + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id
                            + "  AND C_UOM_ID=" + prodC_UOM_ID;
            }
            else
            {
                sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                          + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                          + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                          + "  AND C_UOM_ID=" + prodC_UOM_ID;
            }
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //Flat Discount
                    PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                    _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                    //end
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                }
                else if (_m_AttributeSetInstance_Id > 0)
                {
                    sql = "SELECT PriceStd , PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                + "  AND C_UOM_ID=" + prodC_UOM_ID;
                    DataSet ds2 = DB.ExecuteDataset(sql);
                    if (ds2 != null && ds.Tables.Count > 0)
                    {
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                        _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                            //End
                            PriceList = Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceList"]);
                        }
                    }
                }
            }

            sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                                       " AND con.M_Product_ID = " + _m_Product_Id +
                                       " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + _c_Uom_Id;
            var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            if (rate == 0)
            {
                sql = "SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                      " AND con.C_UOM_ID = " + prodC_UOM_ID + " AND con.C_UOM_To_ID = " + _c_Uom_Id;
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
            int _c_BPartner_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int _c_Order_ID = Util.GetValueOfInt(paramValue[1].ToString());
            int _m_AttributeSetInstance_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _c_UOM_To_ID = Util.GetValueOfInt(paramValue[3].ToString());
            int _ad_Client_Id = Util.GetValueOfInt(paramValue[4].ToString());
            int _m_Product_Id = Util.GetValueOfInt(paramValue[5].ToString());
            decimal _qtyEntered = Util.GetValueOfDecimal(paramValue[6].ToString());
            //End Assign parameter value

            string sql;
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            int countEd011 = 0;
            int isAttributeValue = 0;
            int _m_PriceList_ID = 0;
            int _priceListVersion_Id = 0;

            countEd011 = Env.IsModuleInstalled("ED011_") ? 1 : 0;
            retDic["countEd011"] = countEd011.ToString();
            if (countEd011 > 0)
            {
                MBPartnerModel objBPartner = new MBPartnerModel();
                Dictionary<String, String> bpartner = objBPartner.GetBPartner(ctx, _c_BPartner_ID.ToString());

                MOrderModel objOrder = new MOrderModel();
                _m_PriceList_ID = objOrder.GetM_PriceList(ctx, _c_Order_ID.ToString());

                MPriceListVersionModel objPLV = new MPriceListVersionModel();
                _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID(ctx, _m_PriceList_ID.ToString());

                sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                      + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                      + " AND  M_AttributeSetInstance_ID = " + _m_AttributeSetInstance_Id
                                      + "  AND C_UOM_ID=" + _c_UOM_To_ID;
                DataSet ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isAttributeValue = 1;
                    //Flat Discount
                    PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                                       Util.GetValueOfInt(bpartner["M_DiscountSchema_ID"]), Util.GetValueOfDecimal(bpartner["FlatDiscount"]), _qtyEntered);
                    //End
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                    PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);
                }
                else
                {
                    sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                      + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                      + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) AND C_UOM_ID=" + _c_UOM_To_ID;
                    ds = DB.ExecuteDataset(sql, null, null);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        isAttributeValue = 2;
                        //Flat Discount
                        PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                                           Util.GetValueOfInt(bpartner["M_DiscountSchema_ID"]), Util.GetValueOfDecimal(bpartner["FlatDiscount"]), _qtyEntered);
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
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _ad_Client_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _C_Order_Id = Util.GetValueOfInt(paramValue[2].ToString());
            int _c_BPartner_Id = Util.GetValueOfInt(paramValue[3].ToString());
            decimal _qtyEntered = Util.GetValueOfInt(paramValue[4].ToString());
            int purchasingUom = Util.GetValueOfInt(paramValue[5].ToString());

            //End Assign parameter value
            StringBuilder sql = new StringBuilder();
            decimal PriceEntered = 0;
            decimal PriceList = 0;
            decimal PriceLimit = 0;
            decimal QtyOrdered = 0;
            int _m_DiscountSchema_ID = 0;
            decimal _flatDiscount = 0;
            int _c_Uom_Id = 0;
            int _priceListVersion_Id = 0;
            int _m_PriceList_ID = 0;
            int _standardPrecision = 0;

            MProductModel objProduct = new MProductModel();
            _c_Uom_Id = objProduct.GetC_UOM_ID(ctx, _m_Product_Id.ToString());

            MOrderModel objOrder = new MOrderModel();
            _m_PriceList_ID = objOrder.GetM_PriceList(ctx, _C_Order_Id.ToString());

            MPriceListVersionModel objPLV = new MPriceListVersionModel();
            _priceListVersion_Id = objPLV.GetM_PriceList_Version_ID(ctx, _m_PriceList_ID.ToString());

            MUOMModel objUOM = new MUOMModel();
            _standardPrecision = objUOM.GetPrecision(ctx, _c_Uom_Id.ToString());

            MBPartnerModel objBPartner = new MBPartnerModel();
            Dictionary<String, String> bpartner1 = objBPartner.GetBPartner(ctx, _c_BPartner_Id.ToString());
            _m_DiscountSchema_ID = Util.GetValueOfInt(bpartner1["M_DiscountSchema_ID"]);
            _flatDiscount = Util.GetValueOfInt(bpartner1["FlatDiscount"]);

            sql.Append("SELECT PriceStd , PriceList , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                      + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                      + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                      + "  AND C_UOM_ID=" + purchasingUom);
            DataSet ds = DB.ExecuteDataset(sql.ToString());
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //Flat Discount
                    PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]),
                    _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                    //end
                    PriceList = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]);
                    PriceLimit = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]);

                    //set Conversion Ordered Qty
                    sql.Clear();
                    sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                                      " AND con.M_Product_ID = " + _m_Product_Id +
                                      " AND con.C_UOM_ID = " + _c_Uom_Id + " AND con.C_UOM_To_ID = " + purchasingUom);
                    var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                    if (rate == 0)
                    {
                        sql.Clear();
                        sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                              " AND con.C_UOM_ID = " + _c_Uom_Id + " AND con.C_UOM_To_ID = " + purchasingUom);
                        rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                    }
                    if ((_c_Uom_Id == purchasingUom) && (rate == 0))
                    {
                        rate = 1;
                    }
                    QtyOrdered = _qtyEntered * rate;
                }
                else
                {
                    sql.Clear();
                    sql.Append("SELECT PriceStd , PriceList , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + _m_Product_Id
                                + " AND M_PriceList_Version_ID = " + _priceListVersion_Id
                                + " AND  ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) "
                                + "  AND C_UOM_ID=" + _c_Uom_Id);
                    DataSet ds2 = DB.ExecuteDataset(sql.ToString());
                    if (ds2 != null && ds.Tables.Count > 0)
                    {
                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            //Flat Discount
                            PriceEntered = FlatDiscount(_m_Product_Id, _ad_Client_Id, Util.GetValueOfDecimal(ds2.Tables[0].Rows[0]["PriceStd"]),
                                                        _m_DiscountSchema_ID, _flatDiscount, _qtyEntered);
                            //End

                            //set Conversion Ordered Qty
                            sql.Clear();
                            sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                                              " AND con.M_Product_ID = " + _m_Product_Id +
                                              " AND con.C_UOM_ID = " + _c_Uom_Id + " AND con.C_UOM_To_ID = " + purchasingUom);
                            var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                            if (rate == 0)
                            {
                                sql.Clear();
                                sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                                      " AND con.C_UOM_ID = " + _c_Uom_Id + " AND con.C_UOM_To_ID = " + purchasingUom);
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
                            sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' " +
                                              " AND con.M_Product_ID = " + _m_Product_Id +
                                              " AND con.C_UOM_ID = " + _c_Uom_Id + " AND con.C_UOM_To_ID = " + purchasingUom);
                            var rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                            if (rate == 0)
                            {
                                sql.Clear();
                                sql.Append("SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                                      " AND con.C_UOM_ID = " + _c_Uom_Id + " AND con.C_UOM_To_ID = " + purchasingUom);
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
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _c_BPartner_Id = Util.GetValueOfInt(paramValue[1].ToString());
            //End Assign parameter value
            string sql = null;

            sql = "SELECT producttype FROM m_product where isactive = 'Y' AND M_Product_ID = " + _m_Product_Id;
            string productType = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            retDic["productType"] = Convert.ToString(productType);

            sql = "SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'";
            int countEd011 = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            retDic["countEd011"] = Convert.ToString(countEd011);

            sql = "SELECT C_UOM_ID FROM M_Product_PO WHERE IsActive = 'Y' AND  C_BPartner_ID = " + _c_BPartner_Id +
                    " AND M_Product_ID = " + _m_Product_Id;
            int purchasingUom = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            retDic["purchasingUom"] = Convert.ToString(purchasingUom);

            sql = "SELECT C_UOM_ID FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = " + _m_Product_Id;
            int headerUom = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            retDic["headerUom"] = Convert.ToString(headerUom);

            sql = "SELECT IsDropShip FROM M_Product WHERE IsActive = 'Y' AND M_Product_ID = " + _m_Product_Id;
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
            var sql = "SELECT M_Product_Category_ID FROM M_Product WHERE IsActive='Y' AND M_Product_ID = " + ProductId;
            var productCategoryId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
            var isCalulate = false;
            DataSet dsDiscountBreak = null;
            string discountType = "";
            decimal discountBreakValue = 0;

            // Is flat Discount
            // JID_0487: Not considering Business Partner Flat Discout Checkbox value while calculating the Discount
            sql = "SELECT  DiscountType, IsBPartnerFlatDiscount, FlatDiscount FROM M_DiscountSchema WHERE "
                      + "M_DiscountSchema_ID = " + DiscountSchemaId + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId;
            dsDiscountBreak = DB.ExecuteDataset(sql);

            if (dsDiscountBreak != null && dsDiscountBreak.Tables.Count > 0 && dsDiscountBreak.Tables[0].Rows.Count > 0)
            {
                discountType = Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["DiscountType"]);
                if (discountType == MDiscountSchema.DISCOUNTTYPE_FlatPercent)
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

                else if (discountType == MDiscountSchema.DISCOUNTTYPE_Breaks)
                {
                    // Product Based
                    sql = "SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE "
                               + "M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_ID = " + ProductId
                               + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC";
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
                    sql = "SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE "
                               + " M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID = " + productCategoryId
                               + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC";
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
                    sql = "SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE "
                               + " M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID IS NULL AND m_product_id IS NULL "
                               + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC";
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
            int _m_Product_Id = Util.GetValueOfInt(paramValue[0].ToString());
            int _c_Order_Id = Util.GetValueOfInt(paramValue[1].ToString());
            int _c_Charge_Id = Util.GetValueOfInt(paramValue[2].ToString());
            //End Assign parameter value

            Dictionary<String, object> retDic = new Dictionary<string, object>();
            string sql = null;
            int taxId = 0;
            int _c_BPartner_Id = 0;
            int _c_Bill_Location_Id = 0;
            int _CountVATAX = 0;

            _CountVATAX = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX IN ('VATAX_' )", null, null));
            retDic["_CountVATAX"] = _CountVATAX.ToString();

            MOrderModel objOrder = new MOrderModel();
            Dictionary<String, object> order = objOrder.GetOrder(ctx, _c_Order_Id.ToString());
            _c_BPartner_Id = Util.GetValueOfInt(order["C_BPartner_ID"]);
            _c_Bill_Location_Id = Util.GetValueOfInt(order["Bill_Location_ID"]);

            // Added by Bharat on 26 Feb 2018 to check Exempt Tax on Business Partner
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
                    var paramString = (_c_Order_Id).ToString() + "," + (_m_Product_Id).ToString() + "," + (_c_Charge_Id).ToString();

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
                    var prodtaxCategory = objProduct.GetTaxCategory(ctx, _m_Product_Id.ToString());
                    sql = "SELECT C_Tax_ID FROM VATAX_TaxCatRate WHERE C_TaxCategory_ID = " + prodtaxCategory + " AND IsActive ='Y' AND VATAX_TaxType_ID =" + taxType;
                    taxId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                }
            }
            retDic["taxId"] = taxId.ToString();

            return retDic;
        }

        // Change by mohit to remove client side queries - 16 May 2017
        public decimal GetChargeAmount(Ctx ctx, string fields)
        {
            MCharge charge = new MCharge(ctx, Util.GetValueOfInt(fields), null);
            return charge.GetChargeAmt();

        }
        public Dictionary<string, object> GetEnforcePriceLimit(string fields)
        {
            Dictionary<string, object> retValue = new Dictionary<string, object>();
            string[] paramValues = fields.Split(',');

            //string sql = "SELECT EnforcePriceLimit FROM M_PriceList WHERE IsActive = 'Y' AND M_PriceList_ID = " + Util.GetValueOfInt(fields[0]);
            retValue["EnforcePriceLimit"] = DB.ExecuteScalar("SELECT EnforcePriceLimit FROM M_PriceList WHERE IsActive = 'Y' AND M_PriceList_ID = " + Util.GetValueOfInt(paramValues[0]), null, null);
            retValue["OverwritePriceLimit"] = DB.ExecuteScalar("SELECT OverwritePriceLimit FROM AD_Role WHERE IsActive = 'Y' AND AD_Role_ID = " + Util.GetValueOfInt(paramValues[1]), null, null);
            return retValue;

        }

        public Dictionary<string, object> GetProdAttributeSetInstance(string fields)
        {
            string[] paramValues = fields.Split(',');
            Dictionary<string, object> retValue = new Dictionary<string, object>();

            try
            {
                retValue["M_AttributeSet_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + Util.GetValueOfInt(paramValues[0]), null, null));

                if (Util.GetValueOfInt(retValue["M_AttributeSet_ID"]) > 0)
                {
                    retValue["Attribute_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE UPC = '" + Util.GetValueOfString(paramValues[1]) + "' AND M_Product_ID = " + Util.GetValueOfInt(paramValues[0]), null, null));
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
                string _sql = "SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE IsActive='Y' AND M_Product_ID = " + Util.GetValueOfInt(paramString[0]) +
                                          " AND M_PriceList_Version_ID = " + Util.GetValueOfInt(paramString[1]);
                if (Util.GetValueOfInt(paramString[2]) > 0)
                {
                    _sql = _sql + " AND C_UOM_ID =" + Util.GetValueOfInt(paramString[2]);
                }
                if (Util.GetValueOfInt(paramString[3]) > 0)
                {
                    _sql = _sql + " AND M_AttributeSetInstance_ID = " + Util.GetValueOfInt(paramString[3]);
                }
                else
                {
                    _sql = _sql + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL )";
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
            int _m_Product_Id = Util.GetValueOfInt(param);
            //End Assign parameter value
            string sql = null;

            sql = "SELECT GETPRODUCTCOST( M_Product_ID ,AD_Client_ID ) as Cost FROM M_Product Where isactive = 'Y' AND M_Product_ID = " + _m_Product_Id;
            decimal Cost = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
            retDic["Cost"] = Convert.ToString(Cost);

            return retDic;
        }

        //Get No of Months---Neha
        public int GetNoOfMonths(int _frequency_Id)
        {
            return Util.GetValueOfInt(DB.ExecuteScalar("Select NoOfMonths from C_Frequency where C_Frequency_ID=" + _frequency_Id, null, null));

        }
    }
}