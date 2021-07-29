using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MBPartnerModel
    {

        public Dictionary<String, String> GetBPartner(Ctx ctx, string fields)
        {

            string[] paramValue = fields.Split(',');
            int C_BPartner_ID;

            //Assign parameter value
            C_BPartner_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MBPartner bpartner = new MBPartner(ctx, C_BPartner_ID, null);
            Dictionary<String, String> retDic = new Dictionary<string, string>();

            retDic["M_ReturnPolicy_ID"] = bpartner.GetM_ReturnPolicy_ID().ToString();
            retDic["PO_ReturnPolicy_ID"] = bpartner.GetPO_ReturnPolicy_ID().ToString();

            // Added By Amit 
            if (bpartner.GetM_DiscountSchema_ID() != 0)
            {
                retDic["M_DiscountSchema_ID"] = bpartner.GetM_DiscountSchema_ID().ToString();
            }
            else
            {
                retDic["M_DiscountSchema_ID"] = "0";
            }
            if (bpartner.GetC_BP_Group_ID() > 0)
            {
                retDic["C_BP_Group_ID"] = bpartner.GetC_BP_Group_ID().ToString();
            }
            else
            {
                retDic["C_BP_Group_ID"] = "0";
            }
            if (bpartner.GetFlatDiscount() > 0)
            {
                retDic["FlatDiscount"] = bpartner.GetFlatDiscount().ToString();
            }
            else
            {
                retDic["FlatDiscount"] = "0";
            }
            //30-4-2016
            //VA025
            //if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE ISACTIVE = 'Y' AND  PREFIX='VA025_'")) > 0)
            //{ 
            //    if (bpartner.GetVA025_DiscountCalculation() != null)
            //    {
            //        retDic["VA025_DiscountCalculation"] = bpartner.GetVA025_DiscountCalculation().ToString();
            //    }
            //    else
            //    {
            //        retDic["VA025_DiscountCalculation"] = null;
            //    }
            //    if (bpartner.GetVA025_PromotionalDiscount() != 0)
            //    {
            //        retDic["VA025_PromotionalDiscount"] = bpartner.GetVA025_PromotionalDiscount().ToString();
            //    }
            //    else
            //    {
            //        retDic["VA025_PromotionalDiscount"] = "0";
            //    }
            //}
            //Amit

            return retDic;
        }

        // Added By Bharat on 10/05/2017 
        public Dictionary<String, String> GetBPGroup(Ctx ctx, string fields)
        {
            int C_BPGroup_ID = 0;
            //Assign parameter value
            C_BPGroup_ID = Util.GetValueOfInt(fields);
            MBPGroup bpartner = new MBPGroup(ctx, C_BPGroup_ID, null);
            Dictionary<String, String> retDic = new Dictionary<string, string>();

            retDic["M_ReturnPolicy_ID"] = bpartner.GetM_ReturnPolicy_ID().ToString();
            retDic["PO_ReturnPolicy_ID"] = bpartner.GetPO_ReturnPolicy_ID().ToString();

            if (bpartner.GetM_DiscountSchema_ID() != 0)
            {
                retDic["M_DiscountSchema_ID"] = bpartner.GetM_DiscountSchema_ID().ToString();
            }
            else
            {
                retDic["M_DiscountSchema_ID"] = "0";
            }
            if (bpartner.GetM_PriceList_ID() > 0)
            {
                retDic["M_PriceList_ID"] = bpartner.GetM_PriceList_ID().ToString();
            }
            else
            {
                retDic["M_PriceList_ID"] = "0";
            }
            if (bpartner.GetPO_PriceList_ID() > 0)
            {
                retDic["PO_PriceList_ID"] = bpartner.GetPO_PriceList_ID().ToString();
            }
            else
            {
                retDic["PO_PriceList_ID"] = "0";
            }
            if (bpartner.GetPO_DiscountSchema_ID() > 0)
            {
                retDic["PO_DiscountSchema_ID"] = bpartner.GetPO_DiscountSchema_ID().ToString();
            }
            else
            {
                retDic["PO_DiscountSchema_ID"] = "0";
            }
            //Bharat

            return retDic;
        }

        // Added by Bharat on 11/May/2017
        public Dictionary<string, int> GetBPData(Ctx ctx, string fields)
        {
            int C_BPartner_ID = Util.GetValueOfInt(fields);
            Dictionary<string, int> retDic = null;
            string sql = "SELECT au.Ad_User_ID,  cl.C_BPartner_Location_ID FROM C_BPartner cp INNER JOIN C_BPartner_Location cl ON cl.C_BPartner_ID = cp.C_BPartner_ID " +
                "INNER JOIN Ad_User au ON au.C_BPartner_ID = cp.C_BPartner_ID WHERE cp.C_BPartner_ID = " + C_BPartner_ID + " AND cp.IsActive ='Y' ORDER BY cp.Created";
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, int>();
                retDic["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0][0]);
                retDic["C_BPartner_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0][1]);
            }
            return retDic;
        }

        // Added by Bharat on 12/May/2017
        public Dictionary<string, object> GetBPartnerData(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool countVA009 = Util.GetValueOfBool(paramValue[0]);
            int C_BPartner_ID = Util.GetValueOfInt(paramValue[1]);
            Dictionary<string, object> retDic = null;
            string sql = "SELECT p.AD_Language, p.C_PaymentTerm_ID, COALESCE(p.M_PriceList_ID, g.M_PriceList_ID) AS M_PriceList_ID,"
                + "p.PaymentRule, p.POReference, p.SO_Description, p.IsDiscountPrinted, ";
            if (countVA009)
            {
                sql += " p.VA009_PaymentMethod_ID, ";
            }
            sql += "p.CreditStatusSettingOn,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0)-NVL(p.SO_CreditUsed,0) AS CreditAvailable,"
                + " l.C_BPartner_Location_ID,c.AD_User_ID, p.SOCreditStatus,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["C_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_PaymentTerm_ID"]);
                retDic["M_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_ID"]);
                retDic["PaymentRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRule"]);
                retDic["POReference"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["POReference"]);
                retDic["SO_Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["SO_Description"]);
                retDic["IsDiscountPrinted"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsDiscountPrinted"]);
                if (countVA009)
                {
                    retDic["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                    retDic["VA009_PaymentBaseType"] = Util.GetValueOfString(DB.ExecuteScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID="
                        + Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]), null, null));
                }
                retDic["CreditStatusSettingOn"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["CreditStatusSettingOn"]);
                retDic["SO_CreditLimit"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["SO_CreditLimit"]);
                retDic["CreditAvailable"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CreditAvailable"]);
                retDic["PO_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PriceList_ID"]);
                retDic["PaymentRulePO"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRulePO"]);
                retDic["PO_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PaymentTerm_ID"]);
                retDic["C_BPartner_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_Location_ID"]);
                retDic["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_User_ID"]);
                retDic["SOCreditStatus"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["SOCreditStatus"]);
            }
            return retDic;
        }

        // Added by Bharat on 12/May/2017
        public Dictionary<string, object> GetLocationData(Ctx ctx, string fields)
        {
            int C_BPartner_Location_ID = Util.GetValueOfInt(fields);
            Dictionary<string, object> retDic = null;
            // JID_0161 // change here now will check credit settings on field only on Business Partner Header // Lokesh Chauhan 15 July 2019 
            string sql = "SELECT bp.CreditStatusSettingOn, p.SO_CreditLimit, NVL(p.SO_CreditLimit,0)-NVL(p.SO_CreditUsed,0) AS CreditAvailable" +
                            " FROM C_BPartner_Location p INNER JOIN C_BPartner bp ON (bp.C_BPartner_ID = p.C_BPartner_ID) WHERE p.C_BPartner_Location_ID = " + C_BPartner_Location_ID;
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["CreditStatusSettingOn"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["CreditStatusSettingOn"]);
                retDic["SO_CreditLimit"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["SO_CreditLimit"]);
                retDic["CreditAvailable"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CreditAvailable"]);
            }
            return retDic;
        }

        // Added by Bharat on 13/May/2017
        public Dictionary<string, object> GetBPartnerOrderData(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool countVA009 = Util.GetValueOfBool(paramValue[0]);
            int C_BPartner_ID = Util.GetValueOfInt(paramValue[1]);
            Dictionary<string, object> retDic = null;
            string sql = "SELECT p.AD_Language, p.C_PaymentTerm_ID, COALESCE(p.M_PriceList_ID, g.M_PriceList_ID) AS M_PriceList_ID,"
                + "p.PaymentRule, p.POReference, p.SO_Description, p.SalesRep_ID,p.C_IncoTerm_ID,p.C_IncoTermPO_ID, ";
            if (countVA009)
            {
                //VA009_PO_PaymentMethod_ID added new column for enhancement.. Google Sheet ID-- SI_0036
                sql += " p.VA009_PaymentMethod_ID, p.VA009_PO_PaymentMethod_ID,";
            }
            sql += "p.IsDiscountPrinted, p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + " p.CreditStatusSettingOn,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0)-NVL(p.SO_CreditUsed,0) AS CreditAvailable,"
                + " lship.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID,"
                + " lbill.C_BPartner_Location_ID AS Bill_Location_ID, p.SOCreditStatus, lbill.IsShipTo"
                + " FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN C_BPartner_Location lship ON (p.C_BPartner_ID=lship.C_BPartner_ID AND lship.IsShipTo='Y' AND lship.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["C_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_PaymentTerm_ID"]);
                retDic["M_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_ID"]);
                retDic["PaymentRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRule"]);
                retDic["POReference"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["POReference"]);
                retDic["SO_Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["SO_Description"]);
                retDic["SalesRep_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["SalesRep_ID"]);
                retDic["IsDiscountPrinted"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsDiscountPrinted"]);
                if (countVA009)
                {
                    retDic["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                    retDic["VA009_PaymentBaseType"] = Util.GetValueOfString(DB.ExecuteScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID="
                        + Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]), null, null));
                    //VA009_PO_PaymentMethod_ID added new column for enhancement.. Google Sheet ID-- SI_0036
                    retDic["VA009_PO_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PO_PaymentMethod_ID"]);
                }
                retDic["InvoiceRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["InvoiceRule"]);
                retDic["DeliveryRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["DeliveryRule"]);
                retDic["FreightCostRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["FreightCostRule"]);
                retDic["DeliveryViaRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["DeliveryViaRule"]);
                retDic["CreditStatusSettingOn"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["CreditStatusSettingOn"]);
                retDic["SO_CreditLimit"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["SO_CreditLimit"]);
                retDic["CreditAvailable"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CreditAvailable"]);
                retDic["PO_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PriceList_ID"]);
                retDic["PaymentRulePO"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRulePO"]);
                retDic["PO_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PaymentTerm_ID"]);
                retDic["C_BPartner_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_Location_ID"]);
                retDic["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_User_ID"]);
                retDic["Bill_BPartner_ID"] = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_BPartnerRelation_ID FROM C_BP_Relation WHERE C_BPartner_ID = " + C_BPartner_ID, null, null));
                retDic["Bill_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_Location_ID"]);
                retDic["SOCreditStatus"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["SOCreditStatus"]);
                retDic["IsShipTo"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsShipTo"]);
                retDic["C_IncoTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_IncoTerm_ID"]);
                retDic["C_IncoTermPO_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_IncoTermPO_ID"]);

            }
            return retDic;
        }

        // Added by Bharat on 13/May/2017
        public Dictionary<string, object> GetBPartnerBillData(Ctx ctx, string fields)
        {
            int bill_BPartner_ID = Util.GetValueOfInt(fields);
            Dictionary<string, object> retDic = null;
            string sql = sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + "p.M_PriceList_ID,p.PaymentRule,p.POReference,"
                + "p.SO_Description,p.IsDiscountPrinted,"
                + "p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + "p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "c.AD_User_ID,"
                + "p.PO_PriceList_ID, p.PaymentRulePO, p.PO_PaymentTerm_ID,"
                + "lbill.C_BPartner_Location_ID AS Bill_Location_ID "
                + "FROM C_BPartner p"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + bill_BPartner_ID + " AND p.IsActive='Y'";		//	#1
            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["C_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_PaymentTerm_ID"]);
                retDic["M_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_ID"]);
                retDic["PaymentRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRule"]);
                retDic["POReference"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["POReference"]);
                retDic["SO_Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["SO_Description"]);
                retDic["IsDiscountPrinted"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsDiscountPrinted"]);
                retDic["InvoiceRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["InvoiceRule"]);
                retDic["DeliveryRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["DeliveryRule"]);
                retDic["FreightCostRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["FreightCostRule"]);
                retDic["DeliveryViaRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["DeliveryViaRule"]);
                //retDic["CreditStatusSettingOn"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["CreditStatusSettingOn"]);
                retDic["SO_CreditLimit"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["SO_CreditLimit"]);
                retDic["CreditAvailable"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CreditAvailable"]);
                retDic["PO_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PriceList_ID"]);
                retDic["PaymentRulePO"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRulePO"]);
                retDic["PO_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PaymentTerm_ID"]);
                retDic["Bill_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_Location_ID"]);
                retDic["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_User_ID"]);
            }
            return retDic;
        }

        // Added by Bharat on 16/May/2017
        public Dictionary<string, object> GetBPDocTypeData(Ctx ctx, string fields)
        {
            int C_BPartner_ID = Util.GetValueOfInt(fields);
            Dictionary<string, object> retDic = new Dictionary<string, object>();
            MBPartner bp = new MBPartner(ctx, C_BPartner_ID, null);
            retDic["C_PaymentTerm_ID"] = bp.GetC_PaymentTerm_ID();
            retDic["PaymentRule"] = bp.GetPaymentRule();
            retDic["InvoiceRule"] = bp.GetInvoiceRule();
            retDic["DeliveryRule"] = bp.GetDeliveryRule();
            retDic["FreightCostRule"] = bp.GetFreightCostRule();
            retDic["PaymentRulePO"] = bp.GetPaymentRulePO();
            retDic["DeliveryViaRule"] = bp.GetDeliveryViaRule();
            retDic["PO_PaymentTerm_ID"] = bp.GetPO_PaymentTerm_ID();
            return retDic;
        }

        //Added by Manjot on 11/june/2018
        //VA009_PO_PaymentMethod_ID added new column for enhancement.. Google Sheet ID-- SI_0036
        public Dictionary<string, object> GetBPDetails(Ctx ctx, string BPartner_ID)
        {
            Dictionary<string, object> retDic = null;
            int C_BPartner_ID = Util.GetValueOfInt(BPartner_ID);

            StringBuilder _sql = new StringBuilder();
            _sql.Append("SELECT b.ISVENDOR, b.ISCUSTOMER  ");

            if (Env.IsModuleInstalled("VA009_"))
                _sql.Append(" , b.VA009_PAYMENTMETHOD_ID, B.VA009_PO_PAYMENTMETHOD_ID, PM.VA009_PAYMENTBASETYPE, PMM.VA009_PAYMENTBASETYPE AS VA009_PAYMENTBASETYPEPO");

            _sql.Append(" FROM C_BPartner b");

            if (Env.IsModuleInstalled("VA009_"))
                _sql.Append(" LEFT JOIN VA009_PAYMENTMETHOD PM on B.VA009_PAYMENTMETHOD_ID=PM.VA009_PAYMENTMETHOD_ID LEFT JOIN VA009_PAYMENTMETHOD PMM ON B.VA009_PO_PAYMENTMETHOD_ID = PMM.VA009_PAYMENTMETHOD_ID ");

            _sql.Append(" WHERE b.C_BPartner_ID=" + C_BPartner_ID);

            DataSet ds = DB.ExecuteDataset(_sql.ToString(), null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["IsVendor"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsVendor"]);
                retDic["IsCustomer"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsCustomer"]);

                if (Env.IsModuleInstalled("VA009_"))
                {
                    retDic["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                    retDic["VA009_PO_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PO_PaymentMethod_ID"]);
                    retDic["VA009_PAYMENTBASETYPE"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA009_PAYMENTBASETYPE"]);
                    retDic["VA009_PAYMENTBASETYPEPO"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA009_PAYMENTBASETYPEPO"]);
                }

            }
            return retDic;
        }

        /// <summary>
        /// Get Data from BPartner
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="fields">Input Parameters</param>
        /// <returns>returns collection Data from BPartner</returns>
        public Dictionary<string, object> GetBPDataForProvisionalInvoice(Ctx ctx, string fields)
        {
            Dictionary<string, object> retDic = null;
            int C_BPartner_ID = Util.GetValueOfInt(fields);
            //if VA009_ Module is installed then count the AD_MODULEINFO_ID
            int countVA009 = Env.IsModuleInstalled("VA009_") ? 1 : 0;

            string sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted, p.C_IncoTerm_ID,p.C_IncoTermPO_ID, ";
            //If count is more than zero then the query will add this line
            if (countVA009 > 0)
            {
                sql += " p.VA009_PaymentMethod_ID, p.VA009_PO_PaymentMethod_ID, pm.VA009_PaymentBaseType, pmm.VA009_PaymentBaseType AS VA009_PaymentBaseTypePO,";
            }
            sql += "p.CreditStatusSettingOn,p.SO_CreditLimit, NVL(p.SO_CreditLimit,0) - NVL(p.SO_CreditUsed,0) AS CreditAvailable,"
                + " l.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID, p.IsCustomer, p.IsVendor "
                + " FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) ";
            if (countVA009 > 0)
            {
                sql += " LEFT JOIN VA009_PAYMENTMETHOD pm on p.VA009_PAYMENTMETHOD_ID = pm.VA009_PAYMENTMETHOD_ID" +
                    " LEFT JOIN VA009_PAYMENTMETHOD pmm ON p.VA009_PO_PAYMENTMETHOD_ID = pmm.VA009_PAYMENTMETHOD_ID";
            }
            sql += " WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";

            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["C_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_PaymentTerm_ID"]);
                retDic["M_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_ID"]);
                retDic["PaymentRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRule"]);
                retDic["POReference"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["POReference"]);
                retDic["SO_Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["SO_Description"]);
                retDic["IsDiscountPrinted"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsDiscountPrinted"]);
                if (countVA009 > 0)
                {
                    retDic["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                    retDic["VA009_PaymentBaseType"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA009_PaymentBaseType"]);
                    retDic["VA009_PaymentBaseTypePO"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA009_PaymentBaseTypePO"]);
                    retDic["VA009_PO_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PO_PaymentMethod_ID"]);
                }
                retDic["CreditStatusSettingOn"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["CreditStatusSettingOn"]);
                retDic["SO_CreditLimit"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["SO_CreditLimit"]);
                retDic["CreditAvailable"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["CreditAvailable"]);
                retDic["PO_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PriceList_ID"]);
                retDic["PaymentRulePO"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PaymentRulePO"]);
                retDic["PO_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["PO_PaymentTerm_ID"]);
                retDic["C_BPartner_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_Location_ID"]);
                retDic["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_User_ID"]);
                retDic["C_IncoTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_IncoTerm_ID"]);
                retDic["C_IncoTermPO_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_IncoTermPO_ID"]);
                retDic["IsCustomer"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsCustomer"]);
                retDic["IsVendor"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsVendor"]);
                retDic["countVA009"] = countVA009;

            }
            return retDic;
        }
    }
}